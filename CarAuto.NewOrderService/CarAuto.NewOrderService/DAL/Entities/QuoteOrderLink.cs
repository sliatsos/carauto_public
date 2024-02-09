using CarAuto.EFCore.BaseEntity;
using CarAuto.OrderService.DAL.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarAuto.NewOrderService.DAL.Entities
{
    public class QuoteOrderLink : BaseEntity
    {
        public Order Quote { get; set; }

        [ForeignKey(nameof(Quote))]
        public Guid QuoteId { get; set; }

        public Guid OrderId { get; set; }

        public string OrderNumber { get; set; }
    }
}
