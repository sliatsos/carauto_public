using CarAuto.Protos.Vehicle;
using Microsoft.AspNetCore.SignalR;

namespace CarAuto.VehicleService.SignalR
{
    public class VehicleHub : Hub<Vehicle>
    {
    }
}
