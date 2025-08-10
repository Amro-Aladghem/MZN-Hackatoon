using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.ApplicationResultQuestion
{
    public class ApplicationResultQuestionDTO
    {
        public int QuestionNumber { get; set; }
        public string QuestionText { get; set; }
        public string choice1 { get; set; }
        public string choice2 { get; set; }
        public string choice3 { get; set; }
        public string choice4 { get; set; }
        public int CorrectAnswerNumber { get; set; }
    }
}
