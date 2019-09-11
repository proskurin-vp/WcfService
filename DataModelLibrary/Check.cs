using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DataModelLibrary
{
    [DataContract]
    public class Check
    {
        [Key]
        [DataMember]
        public int Id { get; set; }

        [MaxLength(50)]
        [DataMember]
        public string CheckNumber { get; set; }

        [DataMember]
        public decimal Summ { get; set; }

        [DataMember]
        public decimal Discount { get; set; }

        [DataMember]
        public string Articles { get; set; }
    }
}
