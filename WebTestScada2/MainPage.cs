using RapidSharpTestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace WebTestScada2
{
    [TestClass]
    public class MainPage
    {
        Web w = new Web(@"C:\Users\bezrukov\AppData\Local\Temp\_GeminiCs\", @"\\bezrukovya\shared\_GeminiCsTemplate\", false, @"\\bezrukovya\shared\_GeminiCsTestResult\");
        PageObjectPattern p = new PageObjectPattern();

        [TestMethod()]
        public void Авторизация_до()
        {
            w.Browser.Click(p.Главная.Выйти);
            Assert.IsTrue(w.ScreenShotter.Compare("ДоАвторизации"));
        }

        [TestMethod()]
        public void Авторизация_после()
        {
            Assert.IsTrue(w.ScreenShotter.Compare("ПослеАвторизации"));
        }

        [TestMethod()]
        public void Выезжающее_меню()
        {
            w.Browser.Click(p.Главная.Меню.Левое.Вызов);
            Assert.IsTrue(w.ScreenShotter.Compare("ВыпадающиеМеню"), "Меню не открылось");
            w.Browser.Click(p.Главная.Меню.Левое.Фиксатор);
            Thread.Sleep(300);
            Assert.IsTrue(w.ScreenShotter.Compare("ВыпадающиеМенюВыпадающиеМенюЗакрепка"), "Меню не удалось закрепиться");
        }

        #region Подготовка
        [TestInitialize()]
        public void Initialize()
        {
            w.Browser.Go(p.url.root);
            w.Browser.SendKeys(p.Авторизация.Логин, "mir");
            w.Browser.SendKeys(p.Авторизация.Пароль, "mirpass");
            w.Browser.Click(p.Авторизация.Войти);
            try
            {
                Thread.Sleep(1000);
                w.Browser.Click(p.Главная.Ок);
            }
            catch { }
        }
        #endregion


    }
}
