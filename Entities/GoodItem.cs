using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryApp.Entities
{
    public enum GoodType
    {
        Лист,
        Укорененный_лист,
        Пасынок,
        Детка,
        Укорененный_лист_с_деткой,
        Укорененный_лист_с_детками,
        Крупная_детка,
        Стартер,
        Крупный_стартер,
        Взрослая_розетка,
        Цветущая_розетка
    }

    public enum GoodLocation
    {
        Поселковая,
        Солодунова,
        Фитиль,
        Зип
    }

    [Table(name: "gooditems")]
    public class GoodItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(name:"Id")]
        public int GoodItemId { get; set; }
        public byte Articul { get; set; }//Артикул товара, пример формата: 140-07
        public byte Shelf { get; set; }//Номер полки
        public byte? Case { get; set; }//Номер ящика
        public int Price { get; set; }//Цена за единицу товара
        public bool IsOnSale { get; set; }//Продается ли экземпляр
        public byte Discount { get; set; }//Скидка на товар
        public DateTime? LastSearchDate { get; set; }//Время последнего просмотра
        public DateTime? PlantDate { get; set; }//Дата посадки
        public GoodType Type { get; set; }
        public GoodLocation GoodLocation { get; set; }//Место нахождения

        public Good Good { get; set; }
    }
}
