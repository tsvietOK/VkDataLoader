namespace VkDataLoader
{
    public static class VkConnectionChecker
    {
        private const string VK_URL = "https://vk.com";

        public static async Task<bool> IsConnectionAvailableAsync()
        {
            try
            {
                using HttpClient client = new();
                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, VK_URL));

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
