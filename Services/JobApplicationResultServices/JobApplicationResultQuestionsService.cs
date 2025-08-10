using Database.Entites;
using DTOs.ApplicationResultQuestion;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.JobApplicationResultServices
{
    public class JobApplicationResultQuestionsService
    {
        private readonly AppDbContext _context;

        private readonly int NumberOfAllowedWrongAnswers = 1;

        public JobApplicationResultQuestionsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddQuestionForTheJobApplicationResult(int ResultId,List<ApplicationResultQuestionDTO> questions)
        {
            List<Application_Result_Question> application_Result_Questions = questions.Select(q => new Application_Result_Question()
            {
                AnswerCorrectChoiceNum = q.CorrectAnswerNumber,
                Application_ResultId = ResultId,
                
            })
            .ToList();

            await _context.Application_Result_Questions.AddRangeAsync(application_Result_Questions);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsHasPassedTheTestQuestions(List<ApplicationResultQuestionRequestDTO> questions,int resultId)
        {
            int NumberOfWrongAnswers = 0;

            List<Application_Result_Question> seekerResultQuestions = await _context.Application_Result_Questions.Where(q => q.Application_ResultId == resultId)
                                                                                                                 .OrderBy(q=>q.Id)
                                                                                                                 .AsNoTracking()
                                                                                                                 .ToListAsync();

            questions = questions.OrderBy(q => q.questionNumber).ToList();

            
            for(int i=0;i<seekerResultQuestions.Count;i++)
            {
                if (seekerResultQuestions[i].AnswerCorrectChoiceNum != questions[i].questionAnswerNumber)
                    NumberOfWrongAnswers++;
            }

            return NumberOfWrongAnswers <= NumberOfAllowedWrongAnswers;
        }
    }
}
