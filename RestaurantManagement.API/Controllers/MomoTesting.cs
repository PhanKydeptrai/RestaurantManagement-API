// using RestaurantManagement.API.Abstractions;

// namespace RestaurantManagement.API.Controllers;

// #region Model
// public class MomoRequest
// {
//     public string partnerCode { get; set; } = string.Empty;
//     public string? partnerName { get; set; }
//     public string? storeId { get; set; }
//     public string requestId { get; set; } = string.Empty;
//     public string amount { get; set; } = string.Empty;
//     public string orderId { get; set; } = string.Empty;
//     public string orderInfo { get; set; } = string.Empty;
//     public string redirectUrl { get; set; } = string.Empty;
//     public string ipnUrl { get; set; } = string.Empty;
//     public string lang { get; set; }
//     public string? extraData { get; set; }
//     public string requestType { get; set; } = string.Empty;
//     public string signature { get; set; } = string.Empty;

// }

// public class MomoResponse
// {
//     public string partnerCode { get; set; }
//     public string orderId { get; set; }
//     public string requestId { get; set; }
//     public int amount { get; set; }
//     public long responseTime { get; set; }
//     public string message { get; set; }
//     public int resultCode { get; set; }
//     public string payUrl { get; set; }
//     public string deeplink { get; set; }
//     public string deeplinkMiniApp { get; set; }
// }
// #endregion
// public class MomoTesting : IEndpoint
// {
//     public void MapEndpoint(IEndpointRouteBuilder app)
//     {
//         var endpoints = app.MapGroup("api/momo").WithTags("Payment Test").DisableAntiforgery();

//         endpoints.MapPost("payment", async () =>
//         {
//             MomoRequest momoRequest = new MomoRequest();
//             momoRequest.partnerCode = "MOMO5RGX20191128";
//             momoRequest.partnerName = "Test Momo API Payment";
//             momoRequest.storeId = "Momo Test Store";
//             string accessKey = "M8brj9K6E22vXoDB";
//             string serectkey = "nqQiVSgDMy809JoPF6OzP5OdBUB550Y4";
//             momoRequest.orderInfo = "Payment with momo";
//             momoRequest.redirectUrl = "https://localhost:7062/api/momo/return";
//             momoRequest.ipnUrl = "https://localhost:7062/api/Momo/ipn";
//             momoRequest.requestType = "captureWallet";

//             momoRequest.amount = "50000";
//             momoRequest.orderId = Guid.NewGuid().ToString();
//             momoRequest.requestId = Guid.NewGuid().ToString();
//             momoRequest.extraData = "";
//             //Before sign HMAC SHA256 signature
//             string rawHash = "accessKey=" + accessKey +
//                 "&amount=" + momoRequest.amount +
//                 "&extraData=" + momoRequest.extraData +
//                 "&ipnUrl=" + momoRequest.ipnUrl +
//                 "&orderId=" + momoRequest.orderId +
//                 "&orderInfo=" + momoRequest.orderInfo +
//                 "&partnerCode=" + momoRequest.partnerCode +
//                 "&redirectUrl=" + momoRequest.redirectUrl +
//                 "&requestId=" + momoRequest.requestId +
//                 "&requestType=" + momoRequest.requestType
//                 ;
//             MomoSecurity crypto = new MomoSecurity();
//             //sign signature SHA256
//             momoRequest.signature = crypto.signSHA256(rawHash, serectkey);

//             var request = new HttpRequestMessage(HttpMethod.Post, "https://test-payment.momo.vn/v2/gateway/api/create");
//             var stringJson = JsonConvert.SerializeObject(momoRequest);
//             request.Content = new StringContent(stringJson, System.Text.Encoding.UTF8, "application/json");
//             var response = await _client.SendAsync(request);
//             if (response.IsSuccessStatusCode)
//             {
//                 string data = await response.Content.ReadAsStringAsync();
//                 var MomoResponse = JsonConvert.DeserializeObject<MomoResponse>(data);
//                 return Ok(MomoResponse.payUrl);
//             }
//             return BadRequest("Bad");
//             var response = await momoService.PaymentAsync();
//             return response;
//         });
//     }
// }
