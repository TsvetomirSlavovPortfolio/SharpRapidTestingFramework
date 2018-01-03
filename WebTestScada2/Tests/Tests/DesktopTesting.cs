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
