using System;
using System.Collections.Generic;
using System.Linq;

using Market.Api.Models.Organizations;
using Market.Api.Models.Products;
using Market.Api.Models.Shops;
using Market.CustomersAndStaff.EvrikaPrintClient.Models.WorkOrderPrintDtos;
using Market.CustomersAndStaff.FrontApi.Converters.Mappers;
using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Models.WorkOrders;
using Market.CustomersAndStaff.Utils.Extensions;

namespace Market.CustomersAndStaff.FrontApi.Converters.WorkOrders
{
    public class WorkOrderPrintConverter : IWorkOrderPrintConverter
    {
        public WorkOrderPrintConverter(IMapperWrapper mapperWrapper)
        {
            this.mapperWrapper = mapperWrapper;
        }

        public WorkOrderPrintDto Convert(WorkOrder workOrder, Shop shop, Organization organization, Customer customer, Product[] products, Worker[] workers, bool invoice)
        {
            var workerDict = workers.ToDictionary(x => x.Id);
            var productDict = products.Where(x => x.Id != null).ToDictionary(x => x.Id.Value);

            var orderPrint = new WorkOrderPrintDto
                {
                    Title = $"{(invoice ? "Квитанция к заказ-наряду" : "Заказ-наряд")} {workOrder.Number}",
                    ReceptionDate = DateTime.SpecifyKind(workOrder.ReceptionDate, DateTimeKind.Utc),
                    CompletionDatePlanned = DateTime.SpecifyKind(workOrder.CompletionDatePlanned, DateTimeKind.Utc),
                    WarrantyNumber = workOrder.WarrantyNumber,
                    ReceptionWorkerName = GetWorkerName(workerDict, workOrder.ReceptionWorkerId),
                    Shop = new WorkOrderShopPrintDto
                        {
                            Name = shop.Name,
                            Address = shop.Address,
                            Inn = organization.Inn,
                            Phone = FormatPhone(workOrder.ShopRequisites.Phone),
                        },
                    Client = new WorkOrderClientPrintDto
                        {
                            Name = customer.Name,
                            Phone = FormatPhone(customer.Phone),
                            AdditionalInfo = customer.AdditionalInfo,
                        },
                    HasCustomerProducts = workOrder.CustomerProducts != null && workOrder.CustomerProducts.Length > 0,
                    CustomerProducts = workOrder.CustomerProducts?.Select(x => new CustomerProductPrintDto
                        {
                            Name = x.Name,
                            Quantity = x.Quantity,
                        }).ToArray(),
                    HasComment = !string.IsNullOrEmpty(workOrder.AdditionalText),
                    Comment = workOrder.AdditionalText,
                };

            FillVehicle(orderPrint, workOrder.CustomerValues);
            FillAppliance(orderPrint, workOrder.CustomerValues);
            FillOther(orderPrint, workOrder.CustomerValues);
            FillServices(orderPrint, workOrder.ShopServices, workerDict, productDict);
            FillProducts(orderPrint, workOrder.ShopProducts, productDict);

            if(orderPrint.HasServices)
            {
                orderPrint.ServicesProductsTotalSum = orderPrint.ProductsTotalSum + orderPrint.ServicesTotalSum;
            }

            return orderPrint;
        }

        private static string GetWorkerName(Dictionary<Guid, Worker> workerDict, Guid? workerId)
        {
            if(workerId != null && workerDict.TryGetValue(workerId.Value, out var worker))
            {
                return worker.FullName;
            }

            return null;
        }

        private void FillVehicle(WorkOrderPrintDto workOrderPrint, CustomerValueList customerValueList)
        {
            if(customerValueList.CustomerValueType != CustomerValueType.Vehicle || customerValueList.CustomerValues.Length == 0)
            {
                workOrderPrint.HasVehicleInfo = false;
                workOrderPrint.VehicleDescription = null;
                return;
            }

            var vehicleValue = (VehicleCustomerValue)customerValueList.CustomerValues.First();
            workOrderPrint.HasVehicleInfo = true;
            workOrderPrint.VehicleDescription = mapperWrapper.Map<WorkOrderVehiclePrintDto>(vehicleValue);
        }

        private void FillAppliance(WorkOrderPrintDto workOrderPrint, CustomerValueList customerValueList)
        {
            if(customerValueList.CustomerValueType != CustomerValueType.Appliances || customerValueList.CustomerValues.Length == 0)
            {
                workOrderPrint.HasApplianceInfo = false;
                workOrderPrint.ApplianceDescription = null;
                return;
            }

            var vehicleValue = (ApplianceCustomerValue)customerValueList.CustomerValues.First();
            workOrderPrint.HasApplianceInfo = true;
            workOrderPrint.ApplianceDescription = mapperWrapper.Map<WorkOrderAppliancePrintDto>(vehicleValue);
        }

        private void FillOther(WorkOrderPrintDto workOrderPrint, CustomerValueList customerValueList)
        {
            if(customerValueList.CustomerValueType != CustomerValueType.Other || customerValueList.CustomerValues.Length == 0)
            {
                workOrderPrint.HasCustomerValueDescription = false;
                workOrderPrint.CustomerValueDescription = null;
                return;
            }

            var vehicleValue = (OtherCustomerValue)customerValueList.CustomerValues.First();
            workOrderPrint.HasCustomerValueDescription = true;
            workOrderPrint.CustomerValueDescription = vehicleValue.AdditionalInfo;
        }

        private void FillServices(WorkOrderPrintDto workOrderPrint, ShopService[] shopServices, Dictionary<Guid, Worker> workerDict, Dictionary<Guid, Product> productDict)
        {
            if(shopServices == null || shopServices.Length == 0)
            {
                workOrderPrint.HasServices = false;
                return;
            }

            workOrderPrint.HasServices = true;
            workOrderPrint.Services = shopServices.Select(x => (Service : x, Card : productDict[x.ProductId]))
                                                  .Select(x => new WorkOrderServicePrintDto
                                                      {
                                                          Name = x.Card.Name,
                                                          NaturalId = x.Card.Nomenclature ?? 0,
                                                          Price = x.Service.Price ?? 0m,
                                                          Quantity = FormatQuantity(x.Service.Quantity),
                                                          Sum = decimal.Round((x.Service.Price ?? 0) * x.Service.Quantity, 2, MidpointRounding.AwayFromZero),
                                                          WorkerName = workerDict.GetOrDefault(x.Service.WorkerId)?.FullName,
                                                      }).ToArray();

            workOrderPrint.ServicesTotalSum = workOrderPrint.Services.Sum(x => x.Sum);
        }

        private void FillProducts(WorkOrderPrintDto workOrderPrint, ShopProduct[] shopProducts, Dictionary<Guid, Product> productDict)
        {
            if(shopProducts == null || shopProducts.Length == 0)
            {
                workOrderPrint.HasProducts = false;
                return;
            }

            workOrderPrint.HasProducts = true;
            workOrderPrint.Products = shopProducts.Select(x => (Product : x, Card : productDict[x.ProductId]))
                                                  .Select(x => new WorkOrderProductPrintDto
                                                      {
                                                          Name = x.Card.Name,
                                                          NaturalId = x.Card.Nomenclature ?? 0,
                                                          Price = x.Product.Price ?? 0m,
                                                          Quantity = FormatQuantity(x.Product.Quantity, x.Card.ProductUnit),
                                                          Unit = FormatProductUnit(x.Card.ProductUnit),
                                                          Sum = decimal.Round((x.Product.Price ?? 0) * x.Product.Quantity, 2, MidpointRounding.AwayFromZero),
                                                      }).ToArray();

            workOrderPrint.ProductsTotalSum = workOrderPrint.Products.Sum(x => x.Sum);
        }

        private string FormatPhone(string phone)
        {
            if(string.IsNullOrEmpty(phone))
            {
                return null;
            }

            return "+" + phone;
        }

        private string FormatQuantity(decimal quantity, ProductUnit? unit = null)
        {
            var str = unit == null || unit == ProductUnit.Piece ? quantity.ToString("0") : quantity.ToString("0.000");
            return str;
        }

        private string FormatProductUnit(ProductUnit unitType)
        {
            switch(unitType)
            {
            case ProductUnit.Kilogram:
                return "кг";
            case ProductUnit.Meter:
                return "м";
            case ProductUnit.Liter:
                return "л";
            case ProductUnit.SquareMeter:
                return "м²";
            case ProductUnit.CubicMeter:
                return "м³";
            case ProductUnit.Tonne:
                return "т";
            case ProductUnit.RunningMeter:
                return "пог. м";
            case ProductUnit.Piece:
                return "шт";
            default:
                throw new ArgumentOutOfRangeException(nameof(unitType), unitType.ToString());
            }
        }

        private readonly IMapperWrapper mapperWrapper;
    }
}