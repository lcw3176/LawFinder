using LawFinders.Model;
using System;
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

        public List<HtmlElement> FindAllTitleDocument(WebBrowser browser)
        {
            List<HtmlElement> titles = new List<HtmlElement>();

            foreach (HtmlElement i in browser.Document.GetElementsByTagName("label"))
            {
                if (string.IsNullOrEmpty(i.InnerText))
                {
                    continue;
                }

                titles.Add(i);

            }

            return titles;
        }

        public List<HtmlElement> FindChildrenByClassName(HtmlElement elem, string findClass)
        {
            List<HtmlElement> lst = new List<HtmlElement>();

            foreach (HtmlElement child in elem.Children)
            {
                if (child.GetAttribute("className") == findClass)
                {
                    lst.Add(child);
                }
            }

            return lst;
        }

        public bool IsHaveAnySameData(Laws target, List<Laws> dataLst)
        {
            foreach (Laws vaildation in dataLst)
            {
                if (vaildation.GetFullData() == target.GetFullData())
                {
                    return true;
                }
            }

            return false;
        }

        public List<string> FindRelatedLawsTextFromHtml(Laws law, HtmlElement parent)
        {
            List<string> textLst = new List<string>();

            int size = law.TypeQueue.Count;

            for (int k = 0; k < size; k++)
            {
                string nowType = law.TypeQueue.Dequeue();
                int nowNumb = law.NumQueue.Dequeue();
                int index = 1;
                int beforeIndex = -1;

                if (nowType == "hang")
                {
                    index++;
                }

                string findClass = GetClassNameByLawType(nowType);

                if (nowNumb == 1 && nowType == "hang")
                {
                    textLst.Add(parent.Children[0].InnerText);
                    continue;
                }


                foreach (HtmlElement child in parent.Children)
                {
                    if (child.GetAttribute("className") == findClass)
                    {
                        if (index == nowNumb && index >= beforeIndex)
                        {
                            textLst.Add(child.InnerText);
                            break;
                        }

                        beforeIndex = index++;
                    }
                }
            }

            return textLst;
        }

        public string GetLawTypeByClassName(string className)
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

        public string GetClassNameByLawType(string lawType)
        {
            switch (lawType)
            {
                case "hang":
                    return "pty1_de2_1";
                case "ho":
                    return "pty1_de2h";
                case "mok":
                    return "pty1_de3";
                default:
                    return string.Empty;
            }

        }


        public List<Laws> GetRelatedLawsFromInnerText(string text, string parentName)
        {
            List<Laws> lawList = new List<Laws>();

            string pattern = @"제([0-9]*[제조항호목])(제[0-9]*[제조항호목])*(의[0-9]*)*(제[0-9]*[제조항호목])*";
            Regex regex = new Regex(pattern);

            string joPattern = @"제[0-9]*[조의]+[0-9]*";
            Regex joRegex = new Regex(joPattern);

            string hangPattern = @"제[0-9]*[항]+";
            Regex hangRegex = new Regex(hangPattern);

            string hoPattern = @"제[0-9]*[호]+";
            Regex hoRegex = new Regex(hoPattern);

            string mokPattern = @"제[0-9]*[목]+";
            Regex mokRegex = new Regex(mokPattern);

            foreach (Match i in regex.Matches(text))
            {

                if (string.IsNullOrEmpty(i.Value))
                {
                    continue;
                }

                Laws laws = new Laws();

                foreach(Match j in joRegex.Matches(i.Value))
                {
                    if (!string.IsNullOrEmpty(j.Value) && j.Value != parentName)
                    {
                        laws.Data.Add("jo", j.Value);
                    }
                }

                foreach (Match j in hangRegex.Matches(i.Value))
                {
                    if (!string.IsNullOrEmpty(j.Value))
                    {
                        laws.Data.Add("hang", j.Value);
                        laws.NumQueue.Enqueue(int.Parse(Regex.Replace(j.Value, @"[^0-9]", "")));
                        laws.TypeQueue.Enqueue("hang");
                    }
                }


                foreach (Match j in hoRegex.Matches(i.Value))
                {
                    if (!string.IsNullOrEmpty(j.Value))
                    {
                        laws.Data.Add("ho", j.Value);
                        laws.NumQueue.Enqueue(int.Parse(Regex.Replace(j.Value, @"[^0-9]", "")));
                        laws.TypeQueue.Enqueue("ho");
                    }
                }


                foreach (Match j in mokRegex.Matches(i.Value))
                {
                    if (!string.IsNullOrEmpty(j.Value))
                    {
                        laws.Data.Add("mok", j.Value);
                        laws.NumQueue.Enqueue(int.Parse(Regex.Replace(j.Value, @"[^0-9]", "")));
                        laws.TypeQueue.Enqueue("mok");
                    }
                }

                if (!string.IsNullOrEmpty(laws.GetFullData()))
                {
                    lawList.Add(laws);
                }
                
            }



            return lawList;

        }
    }
}
