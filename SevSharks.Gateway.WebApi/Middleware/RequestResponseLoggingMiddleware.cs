using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SevSharks.Gateway.WebApi.Middleware
{
    /// <summary>
    /// Middleware to log all request and response data
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        /// <summary>
        /// constructor
        /// </summary>
        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        [UsedImplicitly]
        public async Task Invoke(HttpContext context)
        {
            _logger.LogDebug("We in RequestResponseLoggingMiddleware");
            var requestResponseLogInfo = new RequestResponseLogInfo();
            context.Request.EnableRewind();

            var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
            await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            var requestBody = Encoding.UTF8.GetString(buffer);
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            requestResponseLogInfo.Request = requestBody;
            requestResponseLogInfo.RequestDate = DateTime.UtcNow;

            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var response = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                requestResponseLogInfo.Response = response;
                requestResponseLogInfo.ResponseStatusCode = context.Response.StatusCode;
                _logger.LogDebug(JsonConvert.SerializeObject(requestResponseLogInfo));

                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        /// <summary>
        /// RequestResponseLogInfo
        /// </summary>
        public class RequestResponseLogInfo
        {
            /// <summary>
            /// Request
            /// </summary>
            public string Request { get; set; }

            /// <summary>
            /// RequestDate
            /// </summary>
            public DateTime RequestDate { get; set; }

            /// <summary>
            /// Response
            /// </summary>
            public string Response { get; set; }

            /// <summary>
            /// ResponseStatusCode
            /// </summary>
            public int ResponseStatusCode { get; set; }
        }
    }
}
