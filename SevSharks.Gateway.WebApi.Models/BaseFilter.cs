namespace SevSharks.Gateway.WebApi.Models
{
    /// <summary>
    /// Базовый фильтр
    /// </summary>
    public class BaseFilter
    {

        /// <summary>
        /// Номер страницы для пейджинга по умолчанию
        /// </summary>
        public static int DefaultPageNumber = 1;

        /// <summary>
        /// Количество страниц для пейджинга по умолчанию
        /// </summary>
        public static int DefaultItemsPerPage = 10;

        /// <summary>
        /// Проставить значения по умолчанию
        /// </summary>
        public void SetDefault()
        {
            Page = DefaultPageNumber;
            ItemsPerPage = DefaultItemsPerPage;
        }

        /// <summary>
        /// Номер страницы
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Количество позиций на страницу
        /// </summary>
        public int ItemsPerPage { get; set; }
    }
}
