namespace CarAuto.NewOrderService.DAL.DTOs;

public class HeaderDto
{
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; }
    public Guid QuoteId { get; set; }
}
