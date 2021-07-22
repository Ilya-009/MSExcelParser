using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryApp.Entities
{
    [Table("ordergoods")]
    public class OrderGood
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public short Price { get; set; }

        public GoodType Type { get; set; }

        public byte Count { get; set; } = 1;

        public bool IsGift { get; set; } = false;//является ли подарком

        public GoodItem GoodItem { get; set; }//Соответствующий товар
    }
}
