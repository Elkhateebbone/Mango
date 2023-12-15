using Mango.Web.Models;
using Mango.Web.Service.IService;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory httpClientFactory,ITokenProvider tokenProvider)
        {

            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;

        }
        public async Task<ResponseDTO?> SendAsync(RequestDTO requestDTO, bool withBearer = true)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("ManageAPI");
                HttpRequestMessage message = new();
                if(requestDTO.ContentType==Utility.SD.ContentType.MultipartFromData)
                {
                    message.Headers.Add("Accept", "*/*");
                }
                else
                {
                    message.Headers.Add("Accept", "application/json");
                }
                //Token

                if (withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization",$"Bearer {token}");

                }

                
                message.RequestUri = new Uri(requestDTO.Url);

                if (requestDTO.ContentType == Utility.SD.ContentType.MultipartFromData)
                {
                    var content = new MultipartFormDataContent();
                    foreach(var prop in requestDTO.Data.GetType().GetProperties())
                    {
                        var value = prop.GetValue(requestDTO.Data);
                        if(value is FormFile)
                        {
                            var file = (FormFile)value;
                            if(file != null)
                            {
                                content.Add(new StreamContent(file.OpenReadStream()),prop.Name);
                            }
                        }
                        else
                        {
                            content.Add(new StringContent(value==null ?"":value.ToString()),prop.Name);
                        }
                    }
                    message.Content = content;
                }


                else
                {
                    if (requestDTO.Data != null)
                    {
                        message.Content = new StringContent(JsonConvert.SerializeObject(requestDTO.Data), Encoding.UTF8, "application/json");
                    }
                }
              

                HttpResponseMessage? apiResponse = null;
                switch (requestDTO.ApiType)
                {
                    case Utility.SD.ApiType.POST:
                        message.Method = HttpMethod.Post; break;
                    case Utility.SD.ApiType.PUT:
                        message.Method = HttpMethod.Put; break;
                    case Utility.SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete; break;
                    default:
                        message.Method = HttpMethod.Get; break;
                }
                try
                {
                    // Your existing code...
                    apiResponse = await client.SendAsync(message);
                }
                catch (HttpRequestException ex)
                {
                    return new ResponseDTO { IsSuccess = false, Message = $"HTTP Request Exception: {ex.Message}" };
                }
                catch (TaskCanceledException ex)
                {
                    return new ResponseDTO { IsSuccess = false, Message = $"Request Timed Out: {ex.Message}" };
                }


                switch (apiResponse.StatusCode)
                {             
                    case System.Net.HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };
                    case System.Net.HttpStatusCode.Forbidden:
                        return new()
                        {
                            IsSuccess = false,
                            Message = "Forbidden"
                        };
                    case System.Net.HttpStatusCode.Unauthorized:
                        return new()
                        {
                            IsSuccess = false,
                            Message = "UnAuthorized"
                        };
                    case System.Net.HttpStatusCode.InternalServerError:
                        return new()
                        {
                            IsSuccess = false,
                            Message = "Internal Server Error"
                        };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                var dto = new ResponseDTO
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false,

                };
                return dto;
            }

        }
    }
}
