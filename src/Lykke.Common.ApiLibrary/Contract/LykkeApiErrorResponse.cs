using Newtonsoft.Json;

namespace Lykke.Common.ApiLibrary.Contract
{
    public class LykkeApiErrorResponse
    {
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}