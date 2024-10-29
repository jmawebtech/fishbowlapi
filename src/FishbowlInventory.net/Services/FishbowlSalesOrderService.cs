using Fishbowl.net.Models;
using FishbowlInventory.core;

namespace FishbowlInventory.net.Services
{
    public class FishbowlSalesOrderService : StarterService
    {

        public FishbowlSalesOrderService(string accessToken) : base(accessToken, StaticVariables.ProductionUrl)
        {

        }

        /// <summary>
        /// https://fishbowlhelp.com/files/apidocs/import.html
        /// </summary>
        /// <param name="salesOrders"></param>
        /// <returns></returns>
        public async Task<FishbowlSalesOrderResponse> CreateSalesOrdersAsync(List<FishbowlSalesOrder> salesOrders)
        {
            string endPoint = "/api/import/sales-order-details";
            string path = "sales-order-details";
            CsvCreator.CreateCSV(salesOrders, "sales-order-details");
            return await PostCSVAsync<FishbowlSalesOrderResponse>(endPoint, path);
        }

    }
}
