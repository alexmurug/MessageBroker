using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBroker
{
    public class SubjectDeterminator
    {
        //Extragerea comenzii
        public static string GetSubject(string fullCommand)
        {
            var splittedCommandArray = fullCommand.Split(Convert.ToChar(" "));
            return splittedCommandArray.Length > 0 ? splittedCommandArray[0] : string.Empty;
        }

        //Extragerea parametrului comenzii
        public static string GetUnicast(string fullCommand)
        {
            var splitedStrings = fullCommand.Split(Convert.ToChar(" "));
            return splitedStrings.Length > 1 ? splitedStrings[1] : string.Empty;
        }
    }
}
