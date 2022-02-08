using LawFinders.Command;
using LawFinders.Service;
using System;
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

                sb.Append(htmlParseService.GetChildType(elem.GetAttribute("className")));
                sb.Append(" => ");

                foreach (var i in htmlParseService.GetRelatedLawsFromInnerText(elem.InnerText))
                {
                    // 여기 수정
                    sb.Append(i.type[0]);
                    sb.Append(",");
                }

                TargetLawText = sb.ToString();
            }

        }
    }
}
