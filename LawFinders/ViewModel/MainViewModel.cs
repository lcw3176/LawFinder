using LawFinders.Command;
using LawFinders.Service;
using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace LawFinders.ViewModel
{
    public class MainViewModel
    {
        private WebBrowser browser;
        private bool isLoaded = false;
        private readonly ExtractService extractService = new ExtractService();

        public ICommand ExtractCommand { get; set; }

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

            if(elem != null && elem.InnerText != null)
            {
                foreach(var i in extractService.GetRelatedLaws(elem.InnerText))
                {
                    Console.WriteLine(i.Jo + i.Hang + i.Ho);

                }
                
            }
        }

    }
}
