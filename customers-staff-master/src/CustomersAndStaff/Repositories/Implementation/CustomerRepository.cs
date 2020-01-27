using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Repositories.StoredModels;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.SessionTableQueryExtending.PrimitiveStoring;
using SKBKontur.Catalogue.CassandraUtils.DistributedLock.Locker;

namespace Market.CustomersAndStaff.Repositories.Implementation
{
    public class CustomerRepository : ICustomerRepository
    {
        public CustomerRepository(
            CassandraStorage<CustomerStorageElement> storage,
            IMapper mapper,
            ILocker locker,
            ICounterRepository counterRepository,
            CassandraStorage<CustomerIndexByPhoneStorageElement> phoneIndexStorage)
        {
            this.mapper = mapper;
            this.locker = locker;
            this.counterRepository = counterRepository;
            this.phoneIndexStorage = phoneIndexStorage;
            this.storage = storage;
        }

        public async Task<Customer> CreateAsync(Guid organizationId, Customer customer)
        {
            using(await locker.LockAsync(GetLockId(organizationId)).ConfigureAwait(false))
            {
                customer.OrganizationId = organizationId;
                customer.Id = Guid.NewGuid();

                var (element, indexElement) = ToStorageElement(customer);
                await storage.WriteAsync(element).ConfigureAwait(false);
                await IncrementVersion(organizationId).ConfigureAwait(false);
                if(!string.IsNullOrEmpty(indexElement.Phone))
                {
                    await phoneIndexStorage.WriteAsync(indexElement).ConfigureAwait(false);
                }
                return customer;
            }
        }

        public async Task<Customer[]> CreateManyAsync(Guid organizationId, Customer[] customers)
        {
            using(await locker.LockAsync(GetLockId(organizationId)).ConfigureAwait(false))
            {
                customers = customers.Select(x =>
                    {
                        x.Id = Guid.NewGuid();
                        x.OrganizationId = organizationId;
                        return x;
                    }).ToArray();
                var elements = customers.Select(ToStorageElement).ToArray();
                await storage.WriteAsync(elements.Select(x => x.Item1)).ConfigureAwait(false);
                await IncrementVersion(organizationId).ConfigureAwait(false);
                await phoneIndexStorage.WriteAsync(elements.Select(x => x.Item2).Where(x => !string.IsNullOrEmpty(x.Phone))).ConfigureAwait(false);
                return customers;
            }
        }

        public async Task UpdateAsync(Guid organizationId, Guid customerId, Action<Customer> update)
        {
            using(await locker.LockAsync(GetLockId(organizationId)).ConfigureAwait(false))
            {
                var storageElement = await storage.FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Id == customerId)
                                                  .ConfigureAwait(false);
                if(storageElement == null)
                {
                    throw new KeyNotFoundException($"Customer for organization {organizationId} and id {customerId} not found");
                }

                var customer = ToModel(storageElement);
                var phone = customer.Phone;
                update(customer);

                customer.Id = storageElement.Id;
                customer.OrganizationId = organizationId;

                var (element, indexElement) = ToStorageElement(customer);
                await storage.WriteAsync(element).ConfigureAwait(false);
                await IncrementVersion(organizationId).ConfigureAwait(false);
                if(phone != customer.Phone)
                {
                    if(!string.IsNullOrEmpty(phone))
                    {
                        await phoneIndexStorage.DeleteAsync(x => x.OrganizationId == organizationId && x.Phone == phone)
                                               .ConfigureAwait(false);
                    }

                    if(!string.IsNullOrEmpty(customer.Phone))
                    {
                        await phoneIndexStorage.WriteAsync(indexElement).ConfigureAwait(false);
                    }
                }
            }
        }

        public async Task<Customer[]> FindByPhoneAsync(Guid organizationId, string phone)
        {
            var phoneIndexElements = await phoneIndexStorage.WhereAsync(x => x.OrganizationId == organizationId && x.Phone == phone);
            var ids = phoneIndexElements.Select(x => x.CustomerId).ToArray();
            if(ids.Length == 0)
            {
                return new Customer[0];
            }

            return (await storage.WhereAsync(x => x.OrganizationId == organizationId && ids.Contains(x.Id))
                                 .ConfigureAwait(false))
                   .Select(ToModel)
                   .ToArray();
        }

        public async Task<Customer> ReadAsync(Guid organizationId, Guid id)
        {
            return ToModel(await storage.FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Id == id)
                                        .ConfigureAwait(false));
        }

        public async Task<Customer[]> ReadByOrganizationAsync(Guid organizationId, bool includeDeleted = false)
        {
            return (await storage.WhereAsync(x => x.OrganizationId == organizationId)
                                 .ConfigureAwait(false))
                   .Select(ToModel)
                   .Where(x => includeDeleted || !x.IsDeleted)
                   .ToArray();
        }

        public async Task UpdateIndexAsync(Guid organizationId)
        {
            using(await locker.LockAsync(GetLockId(organizationId)).ConfigureAwait(false))
            {
                var elements = await storage.WhereAsync(x => x.OrganizationId == organizationId);
                var indexElements = elements.Where(x => !string.IsNullOrEmpty(x.Phone))
                                            .Select(x => mapper.Map<CustomerIndexByPhoneStorageElement>(x))
                                            .ToArray();

                await phoneIndexStorage.DeleteAsync(x => x.OrganizationId == organizationId);

                if(indexElements.Length > 0)
                {
                    await phoneIndexStorage.WriteAsync(indexElements);
                }
            }
        }

        public IEnumerable<Customer> ReadAll()
        {
            return storage.Table.SetPageSize(1000).Execute().Select(ToModel);
        }

        public Task<int> GetVersionAsync(Guid organizationId)
        {
            return counterRepository.GetCurrentAsync(GetVersionKey(organizationId));
        }

        private Task IncrementVersion(Guid organizationId)
        {
            return counterRepository.IncrementAsync(GetVersionKey(organizationId));
        }

        private (CustomerStorageElement, CustomerIndexByPhoneStorageElement) ToStorageElement(Customer customer)
        {
            var element = mapper.Map<CustomerStorageElement>(customer);
            var indexElement = mapper.Map<CustomerIndexByPhoneStorageElement>(element);
            return (element, indexElement);
        }

        private Customer ToModel(CustomerStorageElement storageElement)
        {
            return mapper.Map<Customer>(storageElement);
        }

        private string GetLockId(Guid organizationId)
        {
            return $"customer/{organizationId}";
        }

        private string GetVersionKey(Guid organizationId)
        {
            return $"version/customer/{organizationId}";
        }

        private readonly CassandraStorage<CustomerStorageElement> storage;
        private readonly CassandraStorage<CustomerIndexByPhoneStorageElement> phoneIndexStorage;
        private readonly IMapper mapper;
        private readonly ILocker locker;
        private readonly ICounterRepository counterRepository;
    }
}