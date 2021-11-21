using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dm.Shibalana.Data.Models
{
    public class AddressInfo
    {
        [JsonIgnore]
        public int AddressInfoId { get; set; }
        public string Label { get; set; }
        public bool IsPartOfTeam { get; set; }
        public string Address { get; set; }
        [Column(TypeName = "decimal(20, 9)")]
        public decimal Amount { get; set; }
    }
}
