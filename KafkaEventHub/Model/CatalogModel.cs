using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KafkaEventHub.Model
{
    public class CatalogModel
    {
        public string Type { get; set; }
        public CatalogFields Fields { get; set; }
    }

    public class CatalogFields
    {
        [Key]
        public string ProductID { get; set; }
        public string TitleEn { get; set; }
        public string TitleFr { get; set; }
        public string SaleRank { get; set; }
        public string Brand { get; set; }
        public List<String> CategoryEn { get; set; }
        public List<String> CategoryFr { get; set; }
        public string NetPrice { get; set; }
        public CatalogContractPrices ContractPrices { get; set; }
        public CatalogVisibility Visibility { get; set; }
        public CatalogAvailability Availability { get; set; }
    }

    public class CatalogAvailability
    {
        public string UW { get; set; }
    }

    public class CatalogVisibility
    {
        public string Default { get; set; }
        public string ContractA { get; set; }
        public string ContractC { get; set; }
    }

    public class CatalogContractPrices
    {
        public string ContractCategoryA { get; set; }
        public string ContractA { get; set; }
        public string ContractC { get; set; }
    }
}
