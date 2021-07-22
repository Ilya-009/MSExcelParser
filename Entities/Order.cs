using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryApp.Entities
{
    public enum OrderStatus
    {
        Новый,
        Обрабатывается,
        Обработан,
        Сбор,
        Готов
    }


    [Table(name: "orders")]
    public class Order
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }

        public List<OrderGood> Goods { get; set; }
        public Customer Customer { get; set; }
    }
}
