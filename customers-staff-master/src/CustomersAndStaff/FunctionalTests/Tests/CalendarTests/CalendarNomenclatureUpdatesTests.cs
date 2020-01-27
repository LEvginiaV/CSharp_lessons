using System;
using System.Linq;
using System.Threading.Tasks;

using GroboContainer.NUnitExtensions;

using Market.Api.Models.Products;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.CalendarPageExtensions;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.Utils.Extensions;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.CalendarTests
{
    public class CalendarNomenclatureUpdatesTests : CalendarTestBase
    {
        [GroboSetUp]
        public override void SetUp()
        {
            base.SetUp();
            products = marketApiClient.Products.GetAll(shop.Id).Result;
            serviceCard = products.First(x => x.ProductCategory == ProductCategory.Service);
            workerId = CreateWorker().Result;
            CreateOneWorkingDay(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 16)).Wait();
            CreateSingleRecord(workerId, DateHelper.Tomorrow(), new ServiceCalendarRecord
                {
                    Period = TimePeriod.CreateByHours(13, 14),
                    ProductIds = new [] {serviceCard.Id.Value}
                }).Wait();
        }
        
        [Description("Изменения карточки услуги")]
        [TestCase(UpdateCardType.SwitchFromServiceToProduct, Description = "Изменение услуги на товар")]
        [TestCase(UpdateCardType.ArchiveCard, Description = "Архивирование карточки услуги")]
        [TestCase(UpdateCardType.UpdatePrice, Description = "Изменения цены в карточке услуги")]
        [TestCase(UpdateCardType.DeletePrice, Description = "Удаление цены в карточке услуги")]
        public async Task CardUpdates(UpdateCardType updateCardType)
        {
            await marketApiClient.Products.Put(shop.Id, serviceCard.Id.Value, UpdateCard(serviceCard, updateCardType));
            
            var page = GoToTomorrowCalendarPage();

            var serviceCardInfo = (serviceCard.Name, serviceCard.PricesInfo.SellPrice);
            
            page.ShowRecordTooltip(0, 0).CheckServiceItems(serviceCardInfo);
            page.ChangeRecord(0, 0).CheckServiceTokens(serviceCardInfo);
        }

        [Test, Description("Удаление карточки услуги")]
        public async Task CardDelete()
        {
            await marketApiClient.Products.Put(shop.Id, serviceCard.Id.Value, UpdateCard(serviceCard, UpdateCardType.DeleteCard));
            var page = GoToTomorrowCalendarPage();
            
            page.ShowRecordTooltip(0, 0).CheckServiceItems();
            page.ChangeRecord(0, 0).CheckServiceTokens();
        }

        private static Product UpdateCard(Product p, UpdateCardType updateCardType)
        {
            // Если эта конструкция будет вызывать сложности, то перепишем на тесты без ветвлений.
            if(updateCardType == UpdateCardType.SwitchFromServiceToProduct)
                p.ProductCategory = ProductCategory.NonAlcoholic;

            if(updateCardType == UpdateCardType.ArchiveCard)
                p.IsArchived = true;

            if(updateCardType == UpdateCardType.DeleteCard)
                p.IsDeleted = true;

            if(updateCardType == UpdateCardType.UpdatePrice)
                p.PricesInfo = new PriceInfo
                    {
                        SellPrice = 500,
                        PriceType = PriceType.FixPrice,
                    };

            if(updateCardType == UpdateCardType.DeletePrice)
                p.PricesInfo = new PriceInfo
                    {
                        SellPrice = null,
                        PriceType = PriceType.WithoutPrice,
                    };

            return p;
        }

        private Product[] products;
        private Product serviceCard;
        private Guid workerId;
    }
}
