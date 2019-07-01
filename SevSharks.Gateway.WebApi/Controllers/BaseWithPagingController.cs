using SolarLab.BusManager.Abstraction;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace SevSharks.Gateway.WebApi.Controllers
{
    /// <summary>
    /// Базовый контроллер с пэйджинацией
    /// </summary>
    public class BaseWithPagingController<T> : BaseController<T>
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public BaseWithPagingController(ILogger<T> logger, IBusManager busManager, IMapper mapper) : base(logger, busManager, mapper)
        {
        }

        /*
        /// <summary>
        /// Запрос в шину на получение данных
        /// </summary>
        /// <typeparam name="TRequest">Запрос</typeparam>
        /// <typeparam name="TResponse">Ответ</typeparam>
        /// <typeparam name="TResponseData">Тип данных в ответе</typeparam>
        /// <param name="request"></param>
        /// <returns>IActionResult</returns>
        protected async Task<IActionResult> RequestResponsePaginate<TRequest, TResponse, TResponseData>(TRequest request)
            where TResponse : BaseResponse<SearchResult<TResponseData>> 
            where TRequest : class, IWithQueueName
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
                return BadRequest($"Error during send Request. Please contact administator. Error #{errorId}");
            }
            if (response == null)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
            if (!response.IsSuccess)
            {
                return BadRequest(response.ErrorMessages);
            }

            if (response.Result == null)
            {
                return NotFound();
            }

            var apiSearchResult = new ApiSearchResult<TResponseData>
            {
                Infos = response.Result.Infos,
                TotalRecords = response.Result.TotalRecords,
                CurrentPage = response.Result.CurrentPage,
                TotalPages = response.Result.TotalPages
            };
            return Ok(apiSearchResult);
        }

        /// <summary>
        /// Проверить и проинициализировать фильтр если он null или неправильная пагинация
        /// </summary>
        /// <typeparam name="T">Тип фильтра</typeparam>
        /// <param name="filter">Фильтр</param>
        /// <returns>Корректный фильтр</returns>
        protected T CheckAndGetFilter<T>(T filter) where T : Models.BaseFilter, new()
        {
            if (filter == null)
            {
                filter = new T();
                filter.SetDefault();
            }
            if (filter.Page <= 0)
            {
                throw new Exception("Номер страницы должен быть больше нуля");
            }
            if (filter.ItemsPerPage <= 0)
            {
                throw new Exception("Значение количества позиций на страницу должено быть больше нуля");
            }

            TrimFilter(filter);

            return filter;
        }

        /// <summary>
        /// Удаляет лишние пробелы у всех полей или свойств переданного DTO которые отмечены аттрибутом TrimFilterAttribute
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private void TrimFilter(object filter)
        {
            var trimableMembers = filter.GetType()
                .GetMembers(BindingFlags.GetProperty | BindingFlags.GetField | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance)
                .Where(MemberCanTrim)
                .ToList();

            foreach (var memberToTrim in trimableMembers)
            {
                memberToTrim.SetValue(filter, memberToTrim.GetValue<string>(filter)?.Trim());
            }
        }

        /// <summary>
        /// Проверяет возможность применить фильтр для члена класса
        /// </summary>
        /// <param name="member">проверяемый член класса, поле или свойство</param>
        /// <returns></returns>
        private bool MemberCanTrim(MemberInfo member)
        {
            if (!member.GetCustomAttributes(typeof(TrimFilterAttribute)).Any())
            {
                return false;
            }

            if (member is FieldInfo asFieldInfo && asFieldInfo.FieldType == typeof(string))
            {
                return true;
            }

            if (member is PropertyInfo asPropertyInfo && asPropertyInfo.CanWrite && asPropertyInfo.CanRead &&
                asPropertyInfo.PropertyType == typeof(string))
            {
                return true;
            }

            return false;
        }
        */
    }
}
