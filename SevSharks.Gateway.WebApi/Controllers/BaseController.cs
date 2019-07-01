using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using SolarLab.BusManager.Abstraction;
using SolarLab.Common.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SevSharks.Gateway.WebApi.Controllers
{
    /// <summary>
    /// BaseController
    /// </summary>
    public abstract class BaseController<TLogger> : Controller
    {
        /// <summary>
        /// Logger
        /// </summary>
        protected ILogger<TLogger> Logger { get; }

        /// <summary>
        /// Mapper
        /// </summary>
        protected readonly IMapper Mapper;

        /// <summary>
        /// Менеджер для работы с шиной данных
        /// </summary>
        protected IBusManager BusManager { get; }

        /// <summary>
        /// RequestInitiatorName
        /// </summary>
        protected const string RequestInitiatorName = Constants.ApplicationInitiatorName;

        /// <summary>
        /// Constructor
        /// </summary>
        protected BaseController(ILogger<TLogger> logger, IBusManager busManager, IMapper mapper)
        {
            Logger = logger;
            BusManager = busManager;
            Mapper = mapper;
        }

        /// <summary>
        /// Возвращает идентификатор текущего пользователя
        /// </summary>
        public string CurrentUserId
        {
            get
            {
                var firstClaim = User?.Claims?.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(firstClaim))
                {
                    return User?.Claims?.Where(c => c.Type == "sub").Select(c => c.Value)
                        .FirstOrDefault();
                }

                return firstClaim;
            }
        }

        /// <summary>
        /// Возвращает язык из хедера запроса
        /// </summary>
        public string LanguageFromHeader => GetStringFromHeader("X-SevSharks-Language");

        /// <summary>
        /// Возвращает RequestId из хедера запроса
        /// </summary>
        public Guid RequestIdFromHeader => GetGuidFromHeader("X-IT2-RequestId");

        /// <summary>
        /// Возвращает ApplicationInitiatorName из хедера запроса
        /// </summary>
        public string ApplicationInitiatorNameFromHeader => GetStringFromHeader("X-IT2-ApplicationInitiatorName");
        
        /// <summary>
        /// Запрос в шину на получение данных
        /// </summary>
        /// <typeparam name="TRequest">Запрос</typeparam>
        /// <typeparam name="TResponse">Ответ</typeparam>
        /// <typeparam name="TResponseData">Тип данных в ответе</typeparam>
        /// <param name="request"></param>
        /// <returns>IActionResult</returns>
        protected async Task<IActionResult> RequestResponse<TRequest, TResponse, TResponseData>(TRequest request)
            where TResponse : BaseResponse<TResponseData> where TRequest : class, IWithQueueName
        {
            TResponse response;
            try
            {
                response = await BusManager.Request<TRequest, TResponse>(request);
            }
            catch (Exception e)
            {
                var errorId = Guid.NewGuid();
                Logger.LogError($"Error({errorId}) during send Request", e);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during send Request. Please contact administator. Error #{errorId}");
            }
            if (response == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            if (!response.IsSuccess)
            {
                return BadRequest(response.ErrorMessages);
            }
            return Ok(response.Result);
        }

        protected async Task<HttpResponseMessage> GetAsync(string url, string authToken = null)
        {
            using (var client = GetClient())
            {
                if (!string.IsNullOrEmpty(authToken))
                {
                    //Add the authorization header
                    client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + authToken);
                }

                return await client.GetAsync(url);
            }
        }

        protected async Task<IActionResult> PostAndReturnResultAsync<T, TModelToSend>(string urlToSend, TModelToSend modelToSend,
            string authToken = null)
        {
            var result = await PostAsync<BaseResponse<T>, TModelToSend>(urlToSend, modelToSend, authToken);
            if (result.IsSuccess && result.Result != null)
            {
                return Ok(result.Result);
            }
            return BadRequest(result.ErrorMessages);
        }

        protected async Task<T> PostAsync<T, TModelToSend>(string urlToSend, TModelToSend modelToSend, string authToken = null)
        {
            var myContent = JsonConvert.SerializeObject(modelToSend);
            Logger.LogDebug($"Try send to URL {urlToSend} next data {modelToSend}. Token = {authToken}");
            using (var client = GetClient())
            {
                if (!string.IsNullOrEmpty(authToken))
                {
                    //Add the authorization header
                    client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + authToken);
                }

                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var result = await client.PostAsync(urlToSend, byteContent);
                if (!result.IsSuccessStatusCode)
                {
                    Logger.LogDebug($"Result IsSuccessStatusCode false. Result: {JsonConvert.SerializeObject(result)}");
                    throw new Exception("Result IsSuccessStatusCode false");
                }
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Result StatusCode != HttpStatusCode.OK");
                }
                var json = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        protected async Task<HttpResponseMessage> PostAsync<TModelToSend>(string urlToSend, TModelToSend modelToSend, string authToken = null)
        {
            using (var client = GetClient())
            {
                if (!string.IsNullOrEmpty(authToken))
                {
                    //Add the authorization header
                    client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + authToken);
                }
                var byteContent = GetByteArrayContentForRequest(modelToSend);
                var result = await client.PostAsync(urlToSend, byteContent);
                return result;
            }
        }

        private ByteArrayContent GetByteArrayContentForRequest<TModelToSend>(TModelToSend modelToSend)
        {
            var myContent = JsonConvert.SerializeObject(modelToSend);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return byteContent;
        }

        protected HttpClient GetClient()
        {
            //if (IgnoreIncorrectCertificate)
            //{
            //    var handler = new WebRequestHandler { ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true };
            //    return new HttpClient(handler);
            //}
            return new HttpClient();
        }

        private string GetStringFromHeader(string nameToSearch)
        {
            var containsKey = Request?.Headers?.ContainsKey(nameToSearch);
            if (containsKey != null && containsKey.Value)
            {
                return Request.Headers[nameToSearch];
            }
            return null;
        }

        private Guid GetGuidFromHeader(string nameToSearch)
        {
            var stringFromHeader = GetStringFromHeader(nameToSearch);
            if (string.IsNullOrEmpty(stringFromHeader))
            {
                return Guid.Empty;
            }

            if (Guid.TryParse(stringFromHeader, out var result))
            {
                return result;
            }
            return Guid.Empty;
        }
    }
}