using System.Diagnostics;
using System.Net.Http;

namespace FishbowlInventory.Core.Starter
{
    public class FishbowlRestHttpClient
    {
        private readonly string requestUri, userName, password, accessToken, baseAddress;
        private FishbowlUrlBuilder builder = new FishbowlUrlBuilder();

        private int requestCount = 0;
        private double milliSecondsElapsed;

        /// <summary>
        /// You can perform 4 requests per second.
        /// </summary>
        private readonly int maximumRequests = 4;
        private readonly int maximumMillisecondsForMaxRequests = 1000;

        /// <summary>
        /// CTOR for basic auth
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="requestUri"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public FishbowlRestHttpClient(string baseAddress, string requestUri, string userName, string password)
        {
            this.baseAddress = baseAddress;
            this.requestUri = requestUri;
            this.userName = userName;
            this.password = password;
        }

        /// <summary>
        /// CTOR for access token bearer auth
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="requestUri"></param>
        /// <param name="accessToken"></param>
        public FishbowlRestHttpClient(string baseAddress, string requestUri, string accessToken)
        {
            this.baseAddress = baseAddress;
            this.requestUri = requestUri;
            this.accessToken = accessToken;
        }

        public async Task<string> GetAsync(IDictionary<string, string> query)
        {
            using HttpClient client = GetClient(string.Empty);
            string url = builder.Build(requestUri, null, query);
            return await client.GetStringAsync(url);
        }

        public async Task<string> PostCsvAsync(string csvFilePath)
        {
            HttpClient client = new HttpClient(new ConnexLoggingHandler(new HttpClientHandler()));
            client.BaseAddress = new Uri(baseAddress);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //cache
            CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
            cacheControl.NoCache = true;
            client.DefaultRequestHeaders.CacheControl = cacheControl;

            byte[] bytes = File.ReadAllBytes(csvFilePath); //c://Temp/test.csv
            HttpContent fileContent = new ByteArrayContent(bytes);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");
            try
            {

                var response = await client.PostAsync(baseAddress, new MultipartFormDataContent
                        {
                            {fileContent, "\"file\"", "\"test.csv\""}
                        });

                return await response.Content.ReadAsStringAsync();

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                Console.WriteLine(message);
                return message;
            }
        }

        public async Task<string> PostAsync(object serializedObj)
        {
            Stopwatch stopWatch = new Stopwatch();
            using HttpClient client = GetClient(string.Empty);
            string url = builder.Build(requestUri, null, null);

            var buffer = Encoding.UTF8.GetBytes(serializedObj.ToString());
            using var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            Throttle();

            stopWatch.Start();

            using var rs = await client.PostAsync(url, byteContent);

            stopWatch.Stop();

            requestCount++;
            milliSecondsElapsed += stopWatch.Elapsed.TotalMilliseconds;

            return await rs.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Prevents throttling errors.
        /// </summary>
        private void Throttle()
        {
            bool isMaxRequests = requestCount >= maximumRequests;

            bool willGetThrottled = milliSecondsElapsed >= maximumMillisecondsForMaxRequests;

            if (isMaxRequests && willGetThrottled)
            {
                Thread.Sleep(new TimeSpan(0, 0, 3));
                requestCount = 0;
                milliSecondsElapsed = 0;
            }
            else if (!isMaxRequests && willGetThrottled)
            {
                milliSecondsElapsed = 0;
            }
        }

        public async Task<string> PutAsync(object serializedObj, string id)
        {
            using HttpClient client = GetClient(string.Empty);
            string url = builder.Build(requestUri, id, null);
            var buffer = Encoding.UTF8.GetBytes(serializedObj.ToString());
            using var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using var rs = await client.PutAsync(baseAddress + url, byteContent);
            return await rs.Content.ReadAsStringAsync();
        }

        public async Task<string> PatchAsync(object serializedObj, string id)
        {
            string url = builder.Build(requestUri, id, null);
            using HttpClient client = GetClient(url);
            var buffer = Encoding.UTF8.GetBytes(serializedObj.ToString());
            using var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using var rs = await client.PatchAsync(baseAddress + url, byteContent);

            return await rs.Content.ReadAsStringAsync();
        }

        private HttpClient GetClient(string urlParam)
        {
            HttpClient client = new HttpClient(new ConnexLoggingHandler(new HttpClientHandler()));
            client.BaseAddress = new Uri(baseAddress + urlParam);

            if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(password))
            {
                var authenticationString = $"{userName}:{password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            }
            else if (!String.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            return client;
        }
    }
}
