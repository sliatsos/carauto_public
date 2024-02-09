using CarAuto.UserService.DAL.DTOs;

namespace CarAuto.UserService.Worker.BusinessLogic
{
    public interface ISyncLogic
    {
        Task SyncChangesAsync(DataSynchronizationDto dataSync);
    }
}
