namespace Fishbowl.net.Models
{
    public class FishbowlSalesOrder 
    {
        public string SONum { get; set; }
        public int Status { get; set; }
        public string CustomerName { get; set; }
        public string BillToName { get; set; }

        public string BillToAddress { get; set; }
        public string BillToCity { get; set; }
        public string BillToState { get; set; }
        public string BillToZip { get; set; }
        public string BillToCountry { get; set; }

        public string ShipToName { get; set; }
        public string ShipToAddress { get; set; }
        public string ShipToCity { get; set; }
        public string ShipToState { get; set; }
        public string ShipToZip { get; set; }

        public string ShipToCountry { get; set; }
        public string CarrierName { get; set; }
        public string TaxRateName { get; set; }
        public int SOItemTypeID { get; set; }
        public string ProductNumber { get; set; }
        public int ProductQuantity { get; set; }

    }
}
