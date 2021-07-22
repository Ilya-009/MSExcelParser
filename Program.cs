using ExcelDataReader;
using ExceParserEF6.Database;
using InventoryApp.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ExceParserEF6
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable result = OpenExclDocument();
            ExecuteParser(result);
        }

        public static int createNewArticul(byte[] values, int startIndex)
        {
            if (values.Length == 0) return 1;

            Array.Sort(values);

            int pointer = startIndex;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == pointer)
                {
                    pointer++;
                }
                else
                {
                    return pointer;
                }
            }

            return ++values[values.Length - 1];
        }

        public static DataTable OpenExclDocument()
        {
            string document = @"C:\Users\Killer_hacker999\Рабочий стол\document.xlsx";

            var stream = File.Open(document, FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            StringBuilder stringBuilder = new StringBuilder();

            DataSet result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });


            //складской учет
            DataTable mainTable = result.Tables[2];

            stream.Close();
            return mainTable;
        }

        public static void ExecuteParser(DataTable table)
        {
            List<Good> allGoods = new List<Good>();
            StringBuilder problemGoods = new StringBuilder();

            List<GoodItem> currentGoodItems = new List<GoodItem>();

            foreach (DataRow row in table.Rows)
            {

                var cellsCollection = row.ItemArray;

                string goodName = cellsCollection[0].ToString();

                if (cellsCollection[0] != null)
                {
                    //Проверка на дублирование заголовков
                    if (goodName.Trim() == "Название сорта") { continue; }

                    if (cellsCollection[5].ToString().Trim() == string.Empty) { continue; }

                    //Если строка с названием сорта
                    if (goodName.Contains("№"))
                    {
                        Good createdGood = new Good();

                        if (allGoods.Count != 0)
                        {
                            allGoods[allGoods.Count - 1].GoodItems = new List<GoodItem>(currentGoodItems);
                            currentGoodItems.Clear();
                        }

                        int catalogNumber = int.Parse(Regex.Match(goodName, @"\d+").Value);
                        string name = Regex.Match(goodName, @"\.(\S|\s)+").Value.Replace(".", "").Trim();
                        string notes = cellsCollection[3].ToString().Trim();
                        short leafPrice = 0;

                        //Цена за лист может быть не определена
                        if (!cellsCollection[9].ToString().Contains("-"))
                        {
                            try
                            {
                                leafPrice = short.Parse(cellsCollection[9].ToString().Trim());
                            }
                            catch { }
                        }

                        createdGood.CatalogId = catalogNumber;
                        createdGood.Name = name;
                        createdGood.Notes = notes;
                        createdGood.LeafPrice = leafPrice;
                        allGoods.Add(createdGood);
                    }
                    else
                    {
                        //---Если GoodItem---//
                        DateTime? searchDate = null;//Дата просмотра
                        try
                        {
                            searchDate = DateTime.Parse(cellsCollection[4].ToString());
                        }
                        catch { }

                        byte articul = ParseArticulNumber(cellsCollection[5].ToString());

                        //Артикул не найден
                        if (articul == 0)
                        {
                            problemGoods.AppendLine(allGoods[allGoods.Count - 1].Name);
                        }

                        byte shelf = 0;
                        try
                        {
                            shelf = byte.Parse(cellsCollection[6].ToString());
                        }
                        catch { }

                        byte caseNumber = 0;
                        try
                        {
                            caseNumber = byte.Parse(cellsCollection[7].ToString());
                        }
                        catch { }

                        DateTime plantDate = DateTime.MinValue;
                        try
                        {
                            plantDate = DateTime.Parse(cellsCollection[12].ToString());
                        }
                        catch { }

                        GoodType goodType = GetGoodType(cellsCollection);

                        int price = -1;
                        try
                        {
                            price = int.Parse(cellsCollection[18].ToString());
                        }
                        catch { }

                        GoodLocation? goodLocation = null;
                        if (cellsCollection[21].ToString() != string.Empty)
                        {
                            goodLocation = GetGoodLocation(cellsCollection[21].ToString());
                        }

                        GoodItem createdGoodItem = new GoodItem();

                        if (searchDate != null)
                        {
                            createdGoodItem.LastSearchDate = searchDate.Value;
                        }

                        createdGoodItem.Articul = articul;
                        createdGoodItem.Shelf = shelf;
                        createdGoodItem.Case = caseNumber;
                        createdGoodItem.PlantDate = plantDate;
                        createdGoodItem.Type = goodType;

                        createdGoodItem.Price = price;

                        if (goodLocation.HasValue)
                        {
                            createdGoodItem.GoodLocation = goodLocation.Value;
                        }

                        currentGoodItems.Add(createdGoodItem);
                    }
                }
            }

            Console.WriteLine("All data read successfully");


            using (var context = new ApplicationDbContext())
            {
                context.Goods.AddRange(allGoods);
                context.SaveChanges();
            }

        }

        static byte ParseArticulNumber(string articul)
        {
            if (articul == null || articul == string.Empty) return 0;

            string parsed;

            try
            {
                parsed = articul.Replace(" ", string.Empty)
                    .Replace("*", string.Empty)
                    .Split('-')[1];
            }
            catch
            {
                return 0;
            }


            char[] values = parsed.ToCharArray();

            while (values[0] == '0')
            {
                values[0] = ' ';
            }

            return byte.Parse(new string(values));
        }

        static GoodType GetGoodType(object[] cellsCollection)
        {
            //Range 11, [13-17]

            //Тип - укорененный лист
            if (cellsCollection[11].ToString().Trim().Contains("*"))
            {
                return GoodType.Укорененный_лист;
            }

            Dictionary<int, GoodType> types = new Dictionary<int, GoodType>();
            types.Add(13, GoodType.Детка);
            types.Add(14, GoodType.Пасынок);
            types.Add(15, GoodType.Стартер);
            types.Add(16, GoodType.Взрослая_розетка);


            //Проход по оставшимся ячейкам
            if (cellsCollection[13].ToString().Trim() == "*")
            {
                return GoodType.Детка;
            }

            for (int i = 13; i <= 16; i++)
            {
                if (cellsCollection[i].ToString().Trim() == "*")
                {
                    return types.Where(el => el.Key == i).FirstOrDefault().Value;
                }
            }

            //Если тип не указан, то по умолчанию лист
            return GoodType.Лист;
        }

        static GoodLocation GetGoodLocation(string value)
        {
            string modifyValue = value.Trim().Replace(" ", string.Empty);

            switch (modifyValue)
            {
                case "з/п":
                    return GoodLocation.Зип;
                case "пос.":
                    return GoodLocation.Поселковая;
                case "ф/с":
                    return GoodLocation.Фитиль;
            }

            //Default
            return GoodLocation.Поселковая;
        }
    }
}