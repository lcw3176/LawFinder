using LawFinders.Command;
using LawFinders.Model;
using LawFinders.Service;
using System;
using System.Collections.Generic;
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
                // 테스트
                foreach(Laws law in lawList)
                {
                    foreach(var i in law.Data.Keys)
                    {
                        if(i == "jo")
                        {
                            string item = law.Data[i];

                            foreach (HtmlElement k in browser.Document.GetElementsByTagName("label"))
                            {                                
                                if (!string.IsNullOrEmpty(k.InnerText) && k.InnerText.Split('(')[0] == item)
                                {
                                    TargetLawText += "\n";
                                    TargetLawText += item;
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
                sb.Append(htmlParseService.GetTitle(elem));
                sb.Append(" ");
                sb.Append(elem.InnerText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0]);

                sb.Append(htmlParseService.GetChildTypeFromClassName(elem.GetAttribute("className")));
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
