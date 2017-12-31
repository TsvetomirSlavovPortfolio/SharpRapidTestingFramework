using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;
using System.Drawing;

namespace RapidSharpTestTools.Web
{
    public class Browser
    {
        ChromeDriver Chrome;
        ChromeOptions chromeOptions = new ChromeOptions();

        public Browser(ChromeDriver Chrome, bool headLess = true)
        {
            this.Chrome = Chrome;
            InitChromeOptions(headLess);
            this.Chrome.Manage().Window.Size = new Size(1100, 800);
        }

        public ChromeDriver GetChrome()
        {
            return Chrome;
        }

        ~Browser()
        {
            Chrome.Quit();
        }

        private void InitChromeOptions(bool headLess)
        {
            if (headLess)
                chromeOptions.AddArguments(new List<string>() { "no-sandbox", "headless", "disable-gpu" });
            Chrome = new ChromeDriver(chromeOptions);
        }

        public void setSizeWindow(int Width, int Height)
        {
            Chrome.Manage().Window.Size = new Size(Width, Height);
        }

        public void Go(string url)
        {
            Chrome.Navigate().GoToUrl(url);
        }

        public void Refresh()
        {
            Chrome.Navigate().Refresh();
        }

        public void Back()
        {
            Chrome.Navigate().Back();
        }

        public void Forward()
        {
            Chrome.Navigate().Forward();
        }

        public void Click(string XPath)
        {
            Chrome.FindElementByXPath(XPath).Click();
        }

        public void Clear(string XPath)
        {
            Chrome.FindElementByXPath(XPath).Clear();
        }

        public void SendKeys(string XPath, string SendingText)
        {
            Chrome.FindElementByXPath(XPath).SendKeys(SendingText);
        }

        public void Submit(string XPath)
        {
            Chrome.FindElementByXPath(XPath).Submit();
        }

        public void DragAndDrop(string sourceXPath, string targetXPath)
        {
            IWebElement source1 = Chrome.FindElementByXPath(sourceXPath);
            IWebElement target1 = Chrome.FindElementByXPath(targetXPath);
            Actions operation = new Actions(Chrome);
            operation.DragAndDrop(source1, target1).Perform();
        }

        public void DragAndDrop(string sourceXPath, int x, int y)
        {
            IWebElement source1 = Chrome.FindElementByXPath(sourceXPath);
            Actions operation = new Actions(Chrome);
            operation.DragAndDropToOffset(source1, x, y).Perform();
        }


    }
}
