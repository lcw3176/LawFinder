using LawFinders.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace LawFinders.ViewModel
{
    public class MainViewModel
    {
        WebBrowser browser;
        public ICommand SearchCommand { get; set; }
        public ICommand SelectCommand { get; set; }

        public MainViewModel(WebBrowser browser)
        {
            this.browser = browser;
            InitBrowser();
        }
        private void InitBrowser()
        {
            browser.Navigate("https://www.law.go.kr/");
            browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
            
        }

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            browser.Document.Click += Document_Click;
        }

        private void Document_Click(object sender, HtmlElementEventArgs e)
        {
            HtmlElement elem = browser.Document.GetElementFromPoint(e.ClientMousePosition);
            Console.WriteLine(elem.InnerText);
        }

    }
}
