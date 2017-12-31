using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITesting.WinControls;

namespace WebTestScada2.CodedUI
{

    [CodedUITest]
    public class CodedUITest1
    {

        [TestMethod]
        public void CodedUITestMethod2()
        {
            var app = ApplicationUnderTest.Launch("C:\\Windows\\System32\\calc.exe", "%windir%\\System32\\calc.exe");
            WinWindow calWindow = app.SearchFor<WinWindow>(new { Name = "Calculator" }, new { ClassName = "CalcFrame" });
            WinButton buttonAdd = calWindow.Container.SearchFor<WinButton>(new { Name = "Сложение" });
            WinButton buttonEqual = calWindow.Container.SearchFor<WinButton>(new { Name = "Равно" });
            WinButton button1 = calWindow.Container.SearchFor<WinButton>(new { Name = "1" });
            WinButton button2 = calWindow.Container.SearchFor<WinButton>(new { Name = "2" });
            WinButton button3 = calWindow.Container.SearchFor<WinButton>(new { Name = "3" });
            WinText txtResult = calWindow.Container.SearchFor<WinText>(new { Name = "Результат" });

            Mouse.Click(button2);
            Mouse.Click(buttonAdd);
            Mouse.Click(button3);
            Mouse.Click(buttonEqual);

            Assert.AreEqual("5", txtResult.DisplayText);

            app.Close();
        }

        #region private property
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;

        public UIMap UIMap
        {
            get
            {
                if (this.map == null)
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
        #endregion
    }
}
