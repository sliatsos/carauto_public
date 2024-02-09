using AutoMapper;
using CarAuto.EFCore.BaseEntity.UnitOfWork;
using CarAuto.UserService.DAL.DTOs;
using CarAuto.UserService.DAL.Entities;
using Action = CarAuto.UserService.DAL.DTOs.Action;

namespace CarAuto.UserService.Worker.BusinessLogic
{
    public class SyncLogic : ISyncLogic
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SyncLogic(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task SyncChangesAsync(DataSynchronizationDto dataSync)
        {
            var payload = dataSync.Payload;
            var action = dataSync.Action;
            var saveData = false;
            foreach (var customer in payload.Customer)
            {
                saveData = await ProcessCustomerAsync(customer, action);
            }

            foreach (var salesperson in payload.Salesperson)
            {
                saveData = await ProcessSalespersonAsync(salesperson, action);
            }

            if (saveData)
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task<bool> ProcessSalespersonAsync(SalespersonDto salespersonDto, Action action)
        {
            var salesperson = _mapper.Map<Salesperson>(salespersonDto);
            var salespersonRepo = await _unitOfWork.GetRepositoryAsync<Salesperson>();

            Salesperson salespersonEntity;

            // need to handle not found issues for insert.
            try
            {
                salespersonEntity = await salespersonRepo.GetByIdAsync(salesperson.Id);
            }
            catch (Exception)
            {

                salespersonEntity = null;
            }
            switch (action)
            {
                case Action.Insert:
                case Action.Update:
                    if (salespersonEntity == null)
                    {
                        await salespersonRepo.InsertAsync(salesperson);
                    }
                    else
                    {
                        UpdateSalespersonEntity(salespersonEntity, salesperson);
                    }

                    break;
                case Action.Delete:
                    await salespersonRepo.DeleteAsync(salespersonEntity.Id);
                    break;
            }

            return true;
        }

        private async Task<bool> ProcessCustomerAsync(CustomerDto customerDto, Action action)
        {
            var customer = _mapper.Map<Customer>(customerDto);
            var customerRepo = await _unitOfWork.GetRepositoryAsync<Customer>();

            Customer customerEntity;

            // need to handle not found issues for insert.
            try
            {
                customerEntity = await customerRepo.GetByIdAsync(customer.Id);
            }
            catch (Exception)
            {

                customerEntity = null;
            }
            switch (action)
            {
                case Action.Insert:
                case Action.Update:
                    if (customerEntity == null)
                    {
                        await customerRepo.InsertAsync(customer);
                    }
                    else
                    {
                        UpdateCustomerEntity(customerEntity, customer);
                    }

                    break;
                case Action.Delete:
                    await customerRepo.DeleteAsync(customerEntity.Id);
                    break;
            }

            return true;
        }

        private void UpdateCustomerEntity(Customer customerEntity, Customer customer)
        {
            customerEntity.Code = customer.Code;
            customerEntity.Country = customer.Country;
            customerEntity.DisplayName = customer.DisplayName;
            customerEntity.Email = customer.Email;
            customerEntity.ModifiedOn = customer.ModifiedOn;
            customerEntity.Image = customer.Image;
        }

        private void UpdateSalespersonEntity(Salesperson salespersonEntity, Salesperson salesperson)
        {
            salespersonEntity.Code = salesperson.Code;
            salespersonEntity.Phone = salesperson.Phone;
            salespersonEntity.DisplayName = salesperson.DisplayName;
            salespersonEntity.Email = salesperson.Email;
            salespersonEntity.Image = salesperson.Image;
        }
    }
}
