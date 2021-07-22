using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryApp.Entities
{
    [Table(name: "customers")]
    public class Customer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Tel { get; set; }

        public List<Order> Orders { get; set; }
        public CustomerDetails CustomerDetails { get; set; }
    }
}
