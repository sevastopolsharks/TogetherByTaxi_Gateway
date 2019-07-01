using System;

namespace SevSharks.Gateway.WebApi.Models
{
    /// <summary>
    /// ДТО файла
    /// </summary>
    public class FileDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Содержимое
        /// </summary>
        public byte[] Content { get; set; }
    }
}
