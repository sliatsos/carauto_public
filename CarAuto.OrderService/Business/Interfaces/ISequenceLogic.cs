namespace CarAuto.OrderService.Business.Interfaces
{
    public interface ISequenceLogic
    {
        Task<string> GetNextNoAsync();
    }
}