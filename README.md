### SharpRapidTestingFramework

> Test scripts should be simple and readable!

This set of utilities is not BDD, but they will also allow you to write simple tests, but you will have to add your pointers to the items in the pageObject class and modify the utilities if necessary.

but now we turn to examples

Desktop test

```
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebTestScada2.CodedUI
{

    [CodedUITest]
    public class CodedUITest1
    {
        [TestMethod]
        public void Plus_2_3()
        {
            CalcApp calc = new CalcApp();
            CUI action = new CUI();
            
            action.Click(calc.Клавиатура.button2);
            action.Click(calc.Клавиатура.Add);
            action.Click(calc.Клавиатура.button3);
            action.Click(calc.Клавиатура.Equal);

            Assert.AreEqual("5", calc.Клавиатура.txtResult.DisplayText);
        }
    }
}

```

Web test
```
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using RapidSharpTestTools.Web;

namespace WebTestScada2
{
    [TestClass]
    public class MainPage
    {
        Web w = new Web(@"C:\Users\bezrukov\AppData\Local\Temp\_GeminiCs\", @"\\bezrukovya\shared\_GeminiCsTemplate\", false, @"\\bezrukovya\shared\_GeminiCsTestResult\");
        PageObjectPattern p = new PageObjectPattern();

        [TestMethod()]
        public void Авторизация_после()
        {
        w.Browser.Go(p.url.root);
            w.Browser.SendKeys(p.Авторизация.Логин, "mir");
            w.Browser.SendKeys(p.Авторизация.Пароль, "mirpass");
            w.Browser.Click(p.Авторизация.Войти);
            Assert.IsTrue(w.ScreenShotter.Compare("ПослеАвторизации"));
        }
    }
}
```
