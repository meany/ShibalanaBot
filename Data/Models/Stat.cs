using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace dm.Shibalana.Data.Models
{
    public class Stat
    {
        [JsonIgnore]
        public int StatId { get; set; }
        public DateTime Date { get; set; }
        [JsonIgnore]
        public Guid Group { get; set; }
        [Column(TypeName = "decimal(20, 9)")]
        public decimal Supply { get; set; }
        [Column(TypeName = "decimal(20, 9)")]
        public decimal Circulation { get; set; }
        public int Holders { get; set; }
    }
}
