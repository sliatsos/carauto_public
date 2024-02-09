namespace CarAuto.NewOrderService.DAL.DTOs;

public class InvoiceHeaderDto
{
    public string Id { get; set; }
    public string TransactionId { get; set; }
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; }
    public string DocumentType { get; set; }
    public string DocumentNo { get; set; }
    public string Date { get; set; }
    public string CustomerType { get; set; }
    public string BusinessPartnerId { get; set; }
    public string CustomerNo { get; set; }
    public string CustomerName { get; set; }
    public string CurrencyId { get; set; }
    public string BusinessTaxGroupId { get; set; }
    public string BusinessPartnerGroupId { get; set; }
    public string BranchId { get; set; }
    public string ShipmentMethod { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentTerms { get; set; }
    public string DueDate { get; set; }
    public string Status { get; set; }
    public string ReferenceId { get; set; }
    public string Pdf { get; set; }
    public VehicleDto Vehicle { get; set; }
    public List<LineDto> Lines { get; set; }
}
