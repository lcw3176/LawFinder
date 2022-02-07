using LawFinders.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LawFinders.Service
{
    internal class ExtractService
    {

        public List<Laws> GetRelatedLaws(string innerText)
        {
            List<Laws> lawList = new List<Laws>();
            StringBuilder sb = new StringBuilder();
            bool isRun = false;

            foreach (char i in innerText)
            {

                if (i == '제' && !isRun)
                {
                    sb.Append("<data>");
                    sb.Append(i);
                    isRun = true;
                    continue;
                }

                if (isRun)
                {
                    if (!char.IsDigit(i) && (i != '조' && i != '항' && i != '호' && i != '의' && i != '제'))
                    {
                        isRun = false;
                    }

                    else
                    {
                        sb.Append(i);
                    }
                }

            }


            foreach (string i in sb.ToString().Split(new string[] {"<data>"}, StringSplitOptions.RemoveEmptyEntries))
            {
                Laws laws = new Laws();
                StringBuilder temp = new StringBuilder();
                
                foreach(string j in i.Split(new string[] { "제" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if(j.Contains("조의") || j.Contains("조"))
                    {
                        laws.Jo = j;
                    }

                    if(j.Contains("항"))
                    {
                        laws.Hang = j;
                    }

                    if(j.Contains("호"))
                    {
                        laws.Ho = j;
                    }
                }

                //string temp = i;

                //if (temp.Contains("조의"))
                //{
                //    laws.Jo = temp.Split(new string[] { "조의" }, StringSplitOptions.RemoveEmptyEntries)[0];
                //    temp.Replace(laws.Jo, "");
                //}

                //else if(temp.Contains("조"))
                //{
                //    laws.Jo = temp.Split(new string[] { "조" }, StringSplitOptions.RemoveEmptyEntries)[0];
                //    temp.Replace(laws.Jo, "");
                //}

                //if (temp.Contains("항"))
                //{
                //    laws.Hang = temp.Split(new string[] { "항" }, StringSplitOptions.RemoveEmptyEntries)[0];
                //    temp.Replace(laws.Hang, "");
                //}

                //if (temp.Contains("호"))
                //{
                //    laws.Ho = temp.Split(new string[] { "호" }, StringSplitOptions.RemoveEmptyEntries)[0];
                //    temp.Replace(laws.Ho, "");
                //}


                lawList.Add(laws);
            }

            return lawList;
            
        }
    }
}
