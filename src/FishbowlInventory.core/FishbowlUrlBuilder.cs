namespace FishbowlInventory.Core.Starter
{
    /// <summary>
    /// Builds URL to send data to and from HubSpot.
    /// </summary>
    public class FishbowlUrlBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hubSpotObjectName">Contact, Deal, Product, etc.</param>
        /// <param name="query">paging, fields to return.</param>
        /// <returns></returns>
        public string Build(string requestUri, string id, IDictionary<string, string> query)
        {
            if (!String.IsNullOrEmpty(id))
                requestUri += $"/{id}";

            return BuildParameters(requestUri, query);
        }

        private string BuildParameters(string path, IDictionary<string, string> query)
        {
            var parameters = new Dictionary<string, string>(query ?? new Dictionary<string, string>());
            var queryString = String.Join("&", parameters.Select(param => $"{param.Key}={param.Value}"));
            var pathAndQuery = path.IndexOf("?") == -1 ? path + "?" + queryString : path + "&" + queryString;

            return pathAndQuery;
        }

    }
}
