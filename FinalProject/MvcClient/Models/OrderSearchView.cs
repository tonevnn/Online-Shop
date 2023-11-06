using System.ComponentModel.DataAnnotations;

namespace MvcClient.Models
{
    public class OrderSearchView
    {
        [DataType(DataType.Date)]
        public DateTime? From { get; set; }
        [DataType(DataType.Date)]
        public DateTime? To { get; set; }
    }
}
