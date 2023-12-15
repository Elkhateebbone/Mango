using Mango.Web.Utility;
using System.Net.Mime;
using static Mango.Web.Utility.SD;
using ContentType = Mango.Web.Utility.SD.ContentType;

namespace Mango.Web.Models
{
    public class RequestDTO
    {
        public ApiType ApiType { get; set; } = SD.ApiType.GET;
        public string Url { get; set; }
        public object? Data { get; set; }
        private string AccessToken  { get; set; }
        public ContentType ContentType { get; set; } = ContentType.Json;
    }
}
