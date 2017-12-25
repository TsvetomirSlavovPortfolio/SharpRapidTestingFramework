using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Drawing;
using System.IO;


namespace RapidSharpTestTools
{
    public class ScreenShotter
    {
        string PatchSource;
        string PatchTemplate;
        string PatchToSave;
        ChromeDriver Chrome;

        public ScreenShotter(ChromeDriver Chrome, string PatchSource, string PatchTemplate, string PatchToSave = "")
        {
            this.Chrome = Chrome;
            this.PatchSource = PatchSource;
            this.PatchTemplate = PatchTemplate;
            this.PatchToSave = PatchToSave;
            InitFolderCreate();
        }

        public bool Compare(string NameFile)
        {
            string fileSource = PatchSource + NameFile + ".png";
            string fileTemplate = PatchTemplate + NameFile + ".png";
            Bitmap Template = new Bitmap(fileTemplate);
            Bitmap EditArea;
            Chrome.Manage().Window.Size = new Size(Template.Width, Template.Height);
            Chrome.GetScreenshot().SaveAsFile(fileSource, ScreenshotImageFormat.Png);
            Bitmap Source = new Bitmap(fileSource);
            int difference = getCompare(Source, Template, out EditArea);
            if (PatchToSave != "" && difference > 0)
                EditArea.Save(PatchToSave+NameFile+"jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            return difference < 1;
        }

        public void Generate(string NameFile, string url, int width, int height)
        {
            Chrome.Manage().Window.Size = new Size(width, height);
            Chrome.Navigate().GoToUrl(url);
            Chrome.GetScreenshot().SaveAsFile(PatchTemplate + NameFile + ".png", ScreenshotImageFormat.Png);
        }

        private int getCompare(Bitmap Source, Bitmap Template, out Bitmap EditArea)
        {
            int i, j = 0, k = 0;
            EditArea = Source;
            for (i = 0; i < Source.Width; i++)
            {
                for (j = 0; j < Source.Height; j++)
                {
                    if (Source.GetPixel(i, j) != Template.GetPixel(i, j))
                    {
                        EditArea.SetPixel(i, j, Color.Red);
                        k++;
                    } 
                }
            }
            return k / (i * j);
        }

        private void InitFolderCreate()
        {
            if (Directory.Exists(PatchSource))
                Directory.Delete(PatchSource, true);
            Directory.CreateDirectory(PatchSource);
            if (PatchToSave != "")
            {
                if (Directory.Exists(PatchToSave))
                    Directory.Delete(PatchToSave, true);
                Directory.CreateDirectory(PatchToSave);
            }
        }
    }
}
