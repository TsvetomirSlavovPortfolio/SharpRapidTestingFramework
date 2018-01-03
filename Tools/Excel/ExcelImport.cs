using System.Data;
using System.Data.OleDb;

namespace RapidSharpTestTools.Excel
{
    public class Excel
    {
        DataSet dataSet = new DataSet();
        string Patch;


        public Excel(string Patch)
        {
            this.Patch = Patch;
        }


        /// <summary>
        /// Указать название листа из Excel файла для добавления в DataSet объекта
        /// </summary>
        /// <param name="ListName">Название листа Excel, для добавления одноименной таблицы</param>
        /// <param name="Range">Диапазон ячеек, чтобы не учитывать заголовки и начать с 3 строки установлен A3:R200</param>
        public void ImportTableFromExcel(string ListName, string Range= "A3:R200")
        {
            var excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Patch + ";" + "Extended Properties='Excel 12.0 Xml;HDR=YES;'";
            var dataTable = new DataTable(ListName);

            using (var excelConnection = new OleDbConnection(excelConnectionString))
            {
                excelConnection.Open();
                var dataAdapter = new OleDbDataAdapter("Select * from [" + ListName + "$" + Range + "]", excelConnection);
                dataAdapter.Fill(dataTable);
                excelConnection.Close();
            }
            dataSet.Tables.Add(dataTable);
        }

        /// <summary>
        /// Узнать количество объектов таблицы, которые соответствуют запросу на выборку
        /// </summary>
        /// <param name="TableName">Имя таблицы</param>
        /// <param name="Select">Текст запроса для выборки</param>
        /// <returns>Количество записей соответствующих запросу</returns>
        public int GetCountBySelect(string TableName, string Select)
        {
            return dataSet.Tables[TableName].Select(Select).Length;
        }

        /// <summary>
        /// Получить массив из строк таблицы
        /// </summary>
        /// <param name="TableName">Имя таблицы</param>
        /// <param name="Select">Текст запроса для выборки</param>
        /// <returns>Массив строк таблицы соответствующий запросу Select</returns>
        public DataRow[] GetRowBySelect(string TableName, string Select)
        {
            return dataSet.Tables[TableName].Select(Select);
        }

        /// <summary>
        /// Значение из первой найденной строки таблицы, которая соответствует запросу Select
        /// </summary>
        /// <param name="TableName">Имя таблицы</param>
        /// <param name="Select">Текст запроса для выборки</param>
        /// <param name="ColumnName">Название столбца, значение которого хотим получить</param>
        /// <returns>Значение из первой строки, которая соответствует запросу</returns>
        public string GetValueFromRow(string TableName, string Select, string ColumnName)
        {
            int indexColumn = dataSet.Tables[TableName].Columns.IndexOf(ColumnName);
            return dataSet.Tables[TableName].Select(Select)[0][indexColumn].ToString();
        }

    }
}
