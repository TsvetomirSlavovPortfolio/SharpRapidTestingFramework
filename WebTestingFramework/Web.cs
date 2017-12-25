using OpenQA.Selenium.Chrome;

namespace RapidSharpTestTools
{
    public class Web
    {
        private ChromeDriver Chrome;
        public string PatchSource;
        public string PatchTemplate;
        public Browser Browser;
        public ScreenShotter ScreenShotter;

        public Web(string PatchSource, string PatchTemplate, bool headLess = true, string PatchToSave = "")
        {
            this.PatchSource = PatchSource;
            this.PatchTemplate = PatchTemplate;
            Browser = new Browser(Chrome, headLess);
            Chrome = Browser.GetChrome();
            ScreenShotter = new ScreenShotter(Chrome, PatchSource, PatchTemplate, PatchToSave);
        }
    }
}
