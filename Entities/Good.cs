using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryApp.Entities
{
    [Table(name: "goods")]
    public class Good
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int CatalogId { get; set; }//Номер в каталоге

        public int YearProfit { get; set; }//Годовая прибыль

        public int AllProfitSum { get; set; }//Общая прибыль

        public DateTime BuyDate { get; set; }//Дата покупки

        public string BoughtFrom { get; set; }//У кого купили

        public short BoughtPrice { get; set; }//За сколько купили

        public string Name { get; set; }//Название сорта

        public string Notes { get; set; }//Примечание, заметки сорта

        public bool IsLeavesAvailable { get; set; }//Есть ли в наличии листья

        public short LeafPrice { get; set; }//Цена за лист

        public byte LeafDiscount { get; set; }//Скидка на лист

        public List<GoodItem> GoodItems { get; set; }
    }
}
