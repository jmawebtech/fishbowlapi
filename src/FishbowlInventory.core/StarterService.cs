namespace FishbowlInventory.Core.Starter
{
    public abstract class StarterService
    {
        protected readonly string userName;
        protected readonly string password;
        protected readonly string productionUrl;
        protected readonly string accessToken;

        public StarterService(string userName, string password, string productionUrl)
        {
            this.userName = userName;
            this.password = password;
            this.productionUrl = productionUrl;
        }

        public StarterService(string accessToken, string productionUrl)
        {
            this.accessToken = accessToken;
            this.productionUrl = productionUrl;
        }

        /// <summary>
        /// Pulls a list of data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endPoint">Like products or orders</param>
        /// <param name="query">List of parameters, like dateCreated or dateModified</param>
        /// <returns></returns>
        protected async Task<T> ListAsync<T>(string endPoint, IDictionary<string, string> query)
        {
            FishbowlRestHttpClient restHttpClient = new FishbowlRestHttpClient(productionUrl, endPoint, userName, password);

            string data = await restHttpClient.GetAsync(query);
            return FishbowlResponseHandler.BuildResults<T>(data);
        }

        /// <summary>
        /// Updates an object, like a stock quantity change on a product
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endPoint">Like products or orders</param>
        /// <param name="serialziedObject">JSON of object, like a product</param>
        /// <param name="id">ID in database of object to update, like a product ID</param>
        /// <returns></returns>
        protected async Task<T> PutAsync<T>(string endPoint, object serialziedObject, string id)
        {
            FishbowlRestHttpClient restHttpClient = new FishbowlRestHttpClient(productionUrl, endPoint, userName, password);
            string data = await restHttpClient.PutAsync(serialziedObject, id);
            return FishbowlResponseHandler.BuildResults<T>(data);
        }

        /// <summary>
        /// Creates an object, like a new product
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endPoint">Like products or orders</param>
        /// <param name="serialziedObject">JSON of object, like a product</param>
        /// <returns></returns>
        protected async Task<T> PostAsync<T>(string endPoint, object serialziedObject)
        {
            FishbowlRestHttpClient restHttpClient = new FishbowlRestHttpClient(productionUrl, endPoint, userName, password);
            string data = await restHttpClient.PostAsync(serialziedObject);
            return FishbowlResponseHandler.BuildResults<T>(data);
        }

        /// <summary>
        /// Creates an object, like a new product
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endPoint">Like products or orders</param>
        /// <param name="serialziedObject">JSON of object, like a product</param>
        /// <returns></returns>
        protected async Task<T> PostCSVAsync<T>(string endPoint, string filePath)
        {
            FishbowlRestHttpClient restHttpClient = new FishbowlRestHttpClient(productionUrl, endPoint, userName, password);
            string data = await restHttpClient.PostCsvAsync(endPoint);
            return FishbowlResponseHandler.BuildResults<T>(data);
        }

    }
}
