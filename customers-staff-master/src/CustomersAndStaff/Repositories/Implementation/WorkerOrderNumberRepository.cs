using System;
using System.Linq;
using System.Threading.Tasks;

using Cassandra.Data.Linq;

using Kontur.Utilities.Convertions.Time;

using Market.CustomersAndStaff.Models.WorkOrders;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Repositories.StoredModels;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.SessionTableQueryExtending.PrimitiveStoring;
using SKBKontur.Catalogue.CassandraUtils.DistributedLock.Locker;
// ReSharper disable StringCompareToIsCultureSpecific

namespace Market.CustomersAndStaff.Repositories.Implementation
{
    public class WorkerOrderNumberRepository : IWorkerOrderNumberRepository
    {
        public WorkerOrderNumberRepository(
            CassandraStorage<WorkerOrderNumberStorageElement> storage,
            ILocker locker)
        {
            this.storage = storage;
            this.locker = locker;
        }

        public async Task<WorkOrderNumber> ReserveFirstAvailableNumberAsync(Guid shopId, WorkOrderNumber startNumber)
        {
            using(await locker.LockAsync(GetLockId(shopId)))
            {
                if (await GetStatus(shopId, startNumber) == null)
                {
                    await storage.WriteAsync(ToStorageElement(shopId, startNumber, WorkerOrderNumberStatus.Reserved));
                    return startNumber;
                }

                var findFromNumber = startNumber == WorkOrderNumber.Max ? WorkOrderNumber.Min : startNumber + 1;
                var number = await FindFirstAvailableNumber(shopId, findFromNumber);
                await storage.WriteAsync(ToStorageElement(shopId, number, WorkerOrderNumberStatus.Reserved), (int)72.Hours().TotalSeconds);

                return number;
            }
        }

        public async Task<bool> TryMakeNumberUsedAsync(Guid shopId, WorkOrderNumber number)
        {
            using(await locker.LockAsync(GetLockId(shopId)))
            {
                if (await GetStatus(shopId, number) == WorkerOrderNumberStatus.Used)
                    return false;
                await storage.WriteAsync(ToStorageElement(shopId, number, WorkerOrderNumberStatus.Used));
                return true;
            }
        }

        public async Task FreeNumberAsync(Guid shopId, WorkOrderNumber number)
        {
            using(await locker.LockAsync(GetLockId(shopId)))
            {
                var searchNumber = ToPointElement(number);
                await storage.DeleteAsync(x => x.ShopId == shopId && x.Point == searchNumber);
            }
        }

        private async Task<WorkerOrderNumberStatus?> GetStatus(Guid shopId, WorkOrderNumber number)
        {
            var searchNumber = ToPointElement(number);
            var pointElement = await storage.FirstOrDefaultAsync(x => x.ShopId == shopId && x.Point == searchNumber);
            return pointElement?.Status;
        }

        private async Task<WorkOrderNumber> FindFirstAvailableNumber(Guid shopId, WorkOrderNumber number)
        {
            while(true)
            {
                var searchNumber = ToPointElement(number);
                var points = (await storage.Table
                                           .Where(x => x.ShopId == shopId && x.Point.CompareTo(searchNumber) >= 0)
                                           .Take(1000)
                                           .ExecuteAsync())
                    .Select(ToModel)
                    .ToArray();

                foreach(var point in points)
                {
                    if(number < point)
                        return number;

                    if(number == WorkOrderNumber.Max)
                    {
                        number = WorkOrderNumber.Min;
                        break;
                    }

                    number += 1;
                }

                if(points.Length < 1000)
                {
                    return number;
                }
            }
        }

        private static WorkOrderNumber ToModel(WorkerOrderNumberStorageElement storageElement)
        {
            return WorkOrderNumber.Parse(storageElement.Point);
        }

        private static string ToPointElement(WorkOrderNumber point)
        {
            return point.ToString();
        }

        private static WorkerOrderNumberStorageElement ToStorageElement(Guid shopId, WorkOrderNumber point, WorkerOrderNumberStatus status)
        {
            return new WorkerOrderNumberStorageElement
                {
                    ShopId = shopId,
                    Point = ToPointElement(point),
                    Status = status,
                };
        }

        private string GetLockId(Guid shopId)
        {
            return $"WorkerOrderNumber/{shopId}";
        }

        private readonly CassandraStorage<WorkerOrderNumberStorageElement> storage;
        private readonly ILocker locker;
    }
}