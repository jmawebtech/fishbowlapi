namespace FishbowlInventory.Core.Starter
{
    public class FishbowlResponseHandler
    {
        public static T BuildResults<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, FishbowlSerialization.Settings);
        }
    }
}