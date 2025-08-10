using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entites
{
    public class Application_Result_Question
    {
        public int Id { get; set; }
        public int AnswerCorrectChoiceNum { get; set; }
        public int Application_ResultId { get; set; }

        public Application_Result Application_Result = new Application_Result();
    }
}
