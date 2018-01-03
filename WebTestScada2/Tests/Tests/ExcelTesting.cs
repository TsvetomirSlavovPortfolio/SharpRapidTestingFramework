using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidSharpTestTools.Excel;

namespace WebTestScada2
{
    [TestClass]
    class ExcelTesting
    {
        Excel excel = new Excel(@"C:\Distrib\Test.xlsx");
        bool init = false;

        [TestMethod()]
        public void Наличие_записей_профиля()
        {
            Assert.IsTrue(excel.GetCountBySelect("Профиль", "Индекс > 40") == 0, "С индексоб более 40 не было событий");
        }

        [TestMethod()]
        public void Значение_из_ячейки()
        {
            Assert.IsFalse(excel.GetValueFromRow("Профиль", "Индекс > 41", "Время события") == "00:00:00", "Событие было не вовремя");
        }

        [TestMethod()]
        public void Количество_строк_с_определенным_значением()
        {
            var selectedRow = excel.GetRowBySelect("Профиль", "Индекс > 41");
            Assert.IsFalse(selectedRow.Length>30, "Событие было не вовремя");
        }



        [TestInitialize]
        public void TestInit()
        {
            if (!init)
            {
                excel.ImportTableFromExcel("Профиль");
                excel.ImportTableFromExcel("Профиль фаза C");
                init = true;
            }
        }

    }
}
