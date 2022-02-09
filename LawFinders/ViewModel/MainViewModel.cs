using LawFinders.Command;
using LawFinders.Model;
using LawFinders.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;

namespace LawFinders.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private WebBrowser browser;
        private bool isLoaded = false;
        private readonly HtmlParseService htmlParseService = new HtmlParseService();
        private List<Laws> lawList;
        private string nowTitle;

        public ICommand ExtractCommand { get; set; }

        private string targetLawText;
        public string TargetLawText
        {
            get { return targetLawText; }
            set
            {
                targetLawText = value;
                OnPropertyChanged("TargetLawText");
            }
        }

        public MainViewModel(WebBrowser browser)
        {
            this.browser = browser;

            ExtractCommand = new RelayCommand(ExtractExecuteMethod);
            InitBrowser();
        }


        /// <summary>
        /// 데이터 외부 문서로 추출 커맨드
        /// </summary>
        /// <param name="obj"></param>
        private void ExtractExecuteMethod(object obj)
        {
            if(lawList.Count != 0)
            {
                test(lawList, nowTitle);
                File.WriteAllText(@"C:\Users\THE-LEE\Desktop\data.txt", TargetLawText);
            }
        }

        private void test(List<Laws> data, string executejoName)
        {
            // 테스트
            foreach (Laws law in data)
            {
                if (law.Data.ContainsKey("jo") && executejoName != law.Data["jo"])
                {
                    foreach (HtmlElement i in browser.Document.GetElementsByTagName("label"))
                    {
                        if (string.IsNullOrEmpty(i.InnerText))
                        {
                            continue;
                        }

                        string targetJo = i.InnerText.Split('(')[0];
                        

                        if (targetJo == law.Data["jo"])
                        {
                            TargetLawText += "\n\n\n";
                            TargetLawText += targetJo;
                            TargetLawText += "\n";

                            int size = law.TypeQueue.Count;
                            HtmlElement parent = i.Parent.Parent.Parent;
                            int beforeIndex = -1;

                            if(size == 0)
                            {
                                foreach (HtmlElement child in parent.Children)
                                {
                                    if (!string.IsNullOrEmpty(child.InnerText))
                                    {
                                        TargetLawText += "\n";
                                        TargetLawText += child.InnerText;
                                        List<Laws> temp = htmlParseService.GetRelatedLawsFromInnerText(child.InnerText);
                                        string tempTitle = htmlParseService.GetTitle(child);
                                        if (temp.Count != 0)
                                        {
                                            test(temp, tempTitle);
                                        }

                                    }
                                }

                                break;
                            }

                            for (int k = 0; k < size; k++)
                            {
                                string nowType = law.TypeQueue.Dequeue();
                                int nowNumb = law.NumQueue.Dequeue();
                                int index = 1;

                                if(nowType == "hang")
                                {
                                    index++;
                                }

                                string findClass = htmlParseService.GetClassTypeToEn(nowType);
                                
                                if(nowNumb == 1 && nowType == "hang")
                                {
                                    TargetLawText += "\n";
                                    TargetLawText += parent.Children[0].InnerText;
                                    continue;
                                }
                                

                                foreach (HtmlElement child in parent.Children)
                                {
                                    if(child.GetAttribute("className") == findClass)
                                    {
                                        if(index == nowNumb && index >= beforeIndex)
                                        {
                                            TargetLawText += "\n";
                                            TargetLawText += child.InnerText;
                                            List<Laws> temp = htmlParseService.GetRelatedLawsFromInnerText(child.InnerText);
                                            string tempTitle = htmlParseService.GetTitle(child);
                                            if (temp.Count != 0)
                                            {
                                                test(temp, tempTitle);
                                            }

                                            beforeIndex = index;
                                            break;
                                        }

                                        index++;
                                    }
                                }
                            }
                        }
                    }
                }

                if (!law.Data.ContainsKey("jo"))
                {
                    foreach (HtmlElement i in browser.Document.GetElementsByTagName("label"))
                    {
                        if (string.IsNullOrEmpty(i.InnerText))
                        {
                            continue;
                        }

                        string targetJo = i.InnerText.Split('(')[0];


                        if (targetJo == executejoName)
                        {
                            int size = law.TypeQueue.Count;
                            HtmlElement parent = i.Parent.Parent.Parent;
                            int beforeIndex = -1;

                            for (int k = 0; k < size; k++)
                            {
                                string nowType = law.TypeQueue.Dequeue();
                                int nowNumb = law.NumQueue.Dequeue();
                                int index = 1;

                                if (nowType == "hang")
                                {
                                    index++;
                                }

                                string findClass = htmlParseService.GetClassTypeToEn(nowType);

                                if (nowNumb == 1 && nowType == "hang")
                                {
                                    TargetLawText += "\n";
                                    TargetLawText += parent.Children[0].InnerText;
                                    continue;
                                }


                                foreach (HtmlElement child in parent.Children)
                                {
                                    if (child.GetAttribute("className") == findClass)
                                    {
                                        if (index == nowNumb && index >= beforeIndex)
                                        {
                                            TargetLawText += "\n";
                                            TargetLawText += child.InnerText;
                                            beforeIndex = index;

                                            List<Laws> temp = htmlParseService.GetRelatedLawsFromInnerText(child.InnerText);
                                            string tempTitle = htmlParseService.GetTitle(child);
                                            if (temp.Count != 0)
                                            {
                                                test(temp, tempTitle);
                                            }
                                            break;
                                        }

                                        index++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        private void InitBrowser()
        {
            browser.Navigate("https://www.law.go.kr/");
            browser.ScriptErrorsSuppressed = true;
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
        }

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (!isLoaded)
            {
                browser.Document.Click += Document_Click;
            }

            isLoaded = true;
        }

        private void Document_Click(object sender, HtmlElementEventArgs e)
        {
            HtmlElement elem = browser.Document.GetElementFromPoint(e.ClientMousePosition);
            StringBuilder sb = new StringBuilder();

            if (elem != null && elem.Parent != null && !string.IsNullOrEmpty(elem.InnerText))
            {
                nowTitle = htmlParseService.GetTitle(elem);
                sb.Append(nowTitle);
                sb.Append(" ");
                sb.Append(elem.InnerText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0]);

                sb.Append(htmlParseService.GetLawTypeToKorean(elem.GetAttribute("className")));
                sb.Append(" => ");

                lawList = htmlParseService.GetRelatedLawsFromInnerText(elem.InnerText);
                foreach (var i in lawList)
                {
                    sb.Append(i.GetFullData());
                    sb.Append(", ");
                }

                TargetLawText = sb.ToString();
            }

        }
    }
}
