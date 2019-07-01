using System.Collections.Generic;
using Newtonsoft.Json;

namespace SevSharks.Gateway.WebApi.Models
{
    /// <summary>
    /// Результаты поиска
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiSearchResult<T>
    {
        /// <summary>
        /// Всего страниц
        /// </summary>
        [JsonProperty(PropertyName = "totalpages")]
        public int TotalPages { get; set; }

        /// <summary>
        /// Текущая страница
        /// </summary>
        [JsonProperty(PropertyName = "currpage")]
        public int CurrentPage { get; set; }

        /// <summary>
        /// Всего записей
        /// </summary>
        [JsonProperty(PropertyName = "totalrecords")]
        public int TotalRecords { get; set; }

        /// <summary>
        /// Все записи по запросу
        /// </summary>
        [JsonProperty(PropertyName = "invdata")]
        public IEnumerable<T> Infos { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ApiSearchResult()
        {
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ApiSearchResult(int totalPages, int currentPage, int totalRecords, IEnumerable<T> infos)
        {
            TotalPages = totalPages;
            CurrentPage = currentPage;
            TotalRecords = totalRecords;
            Infos = infos;
        }
    }
}
