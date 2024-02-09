using CarAuto.EFCore.BaseEntity.UnitOfWork;
using CarAuto.OrderService.Business.Interfaces;
using CarAuto.OrderService.DAL.DTOs;
using CarAuto.OrderService.DAL.Entities;

namespace CarAuto.OrderService.Business;

public class SequenceLogic : ISequenceLogic
{
    private readonly NumberConfig _numberConfig;
    private readonly IUnitOfWork _unitOfWork;

    public SequenceLogic(NumberConfig numberConfig, IUnitOfWork unitOfWork)
    {
        _numberConfig = numberConfig ?? throw new ArgumentNullException(nameof(numberConfig));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<string> GetNextNoAsync()
    {
        var orderRepo = _unitOfWork.GetRepository<Order>();
        var order = orderRepo.Queryable.OrderByDescending(e => e.OrderNumber).FirstOrDefault();
        string orderNumber = _numberConfig.Initial;

        if (order != null)
        {
            orderNumber = order.OrderNumber;
        }

        return Increment(orderNumber);
    }

    private string Increment(string inputString)
    {
        // Extract the numeric part as an integer
        int numericPart = int.Parse(inputString.Substring(_numberConfig.FixedPart.Length)); // Assuming the numeric part always starts at position 2

        // Increment the numeric part
        numericPart++;

        // Format it back into the desired string format with leading zeros
        return _numberConfig.FixedPart + numericPart.ToString("D5");
    }
}
