using LawFinders.Model;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LawFinders.Service
{
    internal class HtmlParseService
    {
        public string GetTitle(HtmlElement element)
        {
            HtmlElementCollection label = element.Parent.GetElementsByTagName("label");

            if (label.Count != 0)
            {
                string title = label[0].InnerText.Split('(')[0];

                if (title.Contains("제"))
                {
                    return title;
                }

            }

            return "";
        }

        public string GetChildType(string className)
        {
            switch (className)
            {
                case "pty1_de2_1":
                    return "항";
                case "pty1_de2h":
                    return "호";
                case "pty1_de3":
                    return "목";
                default:
                    return string.Empty;
            }

        }


        public List<Laws> GetRelatedLawsFromInnerText(string text)
        {
            List<Laws> lawList = new List<Laws>();

            string pattern = @"제([0-9]*[제조항호목])(제[0-9]*[제조항호목])*(의[0-9]*)*";
            Regex regex = new Regex(pattern);

            string numberPattern = @"([0-9])*";
            Regex numberRegex = new Regex(numberPattern);

            string typePattern = @"([조항호목])*";
            Regex typeRegex = new Regex(typePattern);

            foreach (Match i in regex.Matches(text))
            {
                Laws laws = new Laws();

                if (string.IsNullOrEmpty(i.Value))
                {
                    continue;
                }

                foreach(Match j in numberRegex.Matches(i.Value))
                {
                    if (string.IsNullOrEmpty(j.Value))
                    {
                        continue;
                    }

                    laws.number.Add(int.Parse(j.Value));
                }

                foreach(Match j in typeRegex.Matches(i.Value))
                {
                    if (string.IsNullOrEmpty(j.Value))
                    {
                        continue;
                    }


                    laws.type.Add(j.Value);
                }

                lawList.Add(laws);
            }

            return lawList;

        }
    }
}
