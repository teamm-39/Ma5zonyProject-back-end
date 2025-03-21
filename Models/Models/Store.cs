using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Store
    {
        public int StoreId { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<StoreProducts> StoreProducts { get; set; }
        [JsonIgnore]
        public ICollection<OperationStoreProduct> FromOperations { get; set; }
        [JsonIgnore]
        public ICollection<OperationStoreProduct> ToOperations { get; set; }
    }
}
