using System;
using System.Linq;

using FluentAssertions.Extensions;

using Market.Api.Models.Products;
using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Models.WorkOrders;

namespace Market.CustomersAndStaff.FunctionalTests.Helpers.WorkOrders
{
    public class WorkOrderBuilder
    {
        private WorkOrderBuilder(Guid customerId)
        {
            var today = DateTime.UtcNow.Date;
            workOrder = new WorkOrder
                {
                    ClientId = customerId,
                    Number = new WorkOrderNumber("АА", 1),
                    ReceptionDate = today,
                    CompletionDatePlanned = today + 1.Days(),
                    ShopServices = new ShopService[0],
                    ShopProducts = new ShopProduct[0],
                    CustomerProducts = new CustomerProduct[0],
                    Status = WorkOrderStatus.InProgress,
                    DocumentStatus = WorkOrderDocumentStatus.Saved,
                };
        }

        public static WorkOrderBuilder CreateWithCustomer(Customer customer)
        {
            return new WorkOrderBuilder(customer.Id);
        }

        public WorkOrderBuilder WithNumber(WorkOrderNumber number)
        {
            workOrder.Number = number;
            return this;
        }

        public WorkOrderBuilder WithReceptionDate(DateTime date)
        {
            workOrder.ReceptionDate = date;
            return this;
        }

        public WorkOrderBuilder WithCompletionDatePlanned(DateTime date)
        {
            workOrder.CompletionDatePlanned = date;
            return this;
        }

        public WorkOrderBuilder WithCompletionDateFact(DateTime date)
        {
            workOrder.CompletionDateFact = date;
            return this;
        }

        public WorkOrderBuilder WithStatus(WorkOrderStatus status)
        {
            workOrder.Status = status;
            return this;
        }

        public WorkOrderBuilder WithReceptionWorker(Worker worker)
        {
            workOrder.ReceptionWorkerId = worker.Id;
            return this;
        }

        public WorkOrderBuilder WithShopPhone(string phone)
        {
            workOrder.ShopRequisites = new ShopRequisites {Phone = phone, Name = "", Address = "", Inn = ""};
            return this;
        }

        public WorkOrderBuilder WithWarrantyNumber(string warrantyNumber)
        {
            workOrder.WarrantyNumber = warrantyNumber;
            return this;
        }

        public WorkOrderBuilder WithAdditionalText(string additionalText)
        {
            workOrder.AdditionalText = additionalText;
            return this;
        }

        public WorkOrderBuilder WithTotalSum(decimal totalSum)
        {
            workOrder.TotalSum = totalSum;
            return this;
        }

        public WorkOrderBuilder WithFirstProduct(Product firstProduct)
        {
            workOrder.FirstProductId = firstProduct.Id.Value;
            return this;
        }

        public WorkOrderBuilder AddShopServiceFromMarketProduct(Product product, Worker worker = null, decimal? quantity = null)
        {
            workOrder.ShopServices = workOrder.ShopServices
                                              .Append(new ShopService
                                                  {
                                                      ProductId = product.Id.Value,
                                                      Price = product.PricesInfo.SellPrice,
                                                      Quantity = quantity ?? 1,
                                                      WorkerId = worker?.Id,
                                                  })
                                              .ToArray();
            return this;
        }

        public WorkOrderBuilder AddShopProductFromMarketProduct(Product product, decimal? quantity = null)
        {
            workOrder.ShopProducts = workOrder.ShopProducts
                                              .Append(new ShopProduct
                                                  {
                                                      ProductId = product.Id.Value,
                                                      Price = product.PricesInfo.SellPrice,
                                                      Quantity = quantity ?? 1,
                                                  })
                                              .ToArray();
            return this;
        }

        public WorkOrderBuilder AddCustomerProduct(string name, string quantity)
        {
            workOrder.CustomerProducts = workOrder.CustomerProducts
                                                  .Append(new CustomerProduct
                                                      {
                                                          Name = name,
                                                          Quantity = quantity,
                                                      })
                                                  .ToArray();
            return this;
        }

        public WorkOrderBuilder WithVehicleCustomerValue(Action<VehicleCustomerValueBuilder> builderAction = null)
        {
            var vehicleCustomerValueBuilder = new VehicleCustomerValueBuilder();
            builderAction?.Invoke(vehicleCustomerValueBuilder);
            workOrder.CustomerValues = new CustomerValueList
                {
                    CustomerValueType = CustomerValueType.Vehicle,
                    CustomerValues = new BaseCustomerValue[] {vehicleCustomerValueBuilder.Build()},
                };
            return this;
        }

        public WorkOrder Build()
        {
            return workOrder;
        }

        private readonly WorkOrder workOrder;
    }

    public class VehicleCustomerValueBuilder
    {
        public VehicleCustomerValueBuilder()
        {
            vehicleCustomerValue = new VehicleCustomerValue();
        }

        public VehicleCustomerValueBuilder WithBrand(string brand)
        {
            vehicleCustomerValue.Brand = brand;
            return this;
        }

        public VehicleCustomerValueBuilder WithModel(string model)
        {
            vehicleCustomerValue.Model = model;
            return this;
        }

        public VehicleCustomerValueBuilder WithBodyNumber(string bodyNumber)
        {
            vehicleCustomerValue.BodyNumber = bodyNumber;
            return this;
        }

        public VehicleCustomerValueBuilder WithEngineNumber(string engineNumber)
        {
            vehicleCustomerValue.EngineNumber = engineNumber;
            return this;
        }

        public VehicleCustomerValueBuilder WithRegisterSign(string registerSign)
        {
            vehicleCustomerValue.RegisterSign = registerSign;
            return this;
        }

        public VehicleCustomerValueBuilder WithVin(string vin)
        {
            vehicleCustomerValue.Vin = vin;
            return this;
        }

        public VehicleCustomerValueBuilder WithYear(int year)
        {
            vehicleCustomerValue.Year = year;
            return this;
        }

        public VehicleCustomerValueBuilder WithAdditionalInfo(string additionalInfo)
        {
            vehicleCustomerValue.AdditionalInfo = additionalInfo;
            return this;
        }

        public VehicleCustomerValue Build()
        {
            return vehicleCustomerValue;
        }

        private readonly VehicleCustomerValue vehicleCustomerValue;
    }
}