namespace CarAuto.UserService.DAL.DTOs;
public class PayloadDto
{
    public List<CustomerDto> Customer { get; set; } = new List<CustomerDto>();

    public List<SalespersonDto> Salesperson { get; set; } = new List<SalespersonDto>();

    //public List<CurrencyDto> Currency { get; set; } = new List<CurrencyDto>();

    //public List<CompanyInformationDto> CompanyInformation { get; set; } = new List<CompanyInformationDto>();

    //public List<BusinessTaxGroup> BusinessTaxGroup { get; set; } = new List<BusinessTaxGroup>();

    //public List<ProductTaxGroup> ProductTaxGroup { get; set; } = new List<ProductTaxGroup>();

    //public List<TaxSetup> TaxSetup { get; set; } = new List<TaxSetup>();
}
