namespace CarAuto.VehicleService.DAL.DTOs;
public class PayloadDto
{
    public List<VehicleDto> Vehicles { get; set; } = new List<VehicleDto>();

    public List<BrandDto> Brands { get; set; } = new List<BrandDto>();

    public List<ModelDto> Models { get; set; } = new List<ModelDto>();

    public List<OptionDto> Options { get; set; } = new List<OptionDto>();

    //public List<CurrencyDto> Currency { get; set; } = new List<CurrencyDto>();

    //public List<CompanyInformationDto> CompanyInformation { get; set; } = new List<CompanyInformationDto>();

    //public List<BusinessTaxGroup> BusinessTaxGroup { get; set; } = new List<BusinessTaxGroup>();

    //public List<ProductTaxGroup> ProductTaxGroup { get; set; } = new List<ProductTaxGroup>();

    //public List<TaxSetup> TaxSetup { get; set; } = new List<TaxSetup>();
}
