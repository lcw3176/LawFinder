using System.Collections.Generic;
using System.Text;

namespace LawFinders.Model
{
    public class Laws
    {

        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();


        public string GetFullData()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach(string key in Data.Keys)
            {
                stringBuilder.Append(Data[key]);
            }

            return stringBuilder.ToString();
        }
    }
}
