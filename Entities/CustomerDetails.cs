using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryApp.Entities
{
    [Table("customerdetails")]
    public class CustomerDetails
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Car { get; set; }
        public string WealthState { get; set; } //Насколько богат
        public bool IsCompetitor { get; set; }//Является ли конкурентом
        public string Notes { get; set; }//Прочие заметки

        //public Customer Customer { get; set; }
    }
}
