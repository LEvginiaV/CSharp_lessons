using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.Models.OnlineRecording;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Tests.Core;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using NUnit.Framework;

namespace Market.CustomersAndStaff.RepositoriesTests.OnlineRecordingTests
{
    public class PublicLinkTests : IMainSuite
    {
        [Test]
        public async Task WriteThenReadByShopId()
        {
            var publicLink = new PublicLink
                {
                    ShopId = Guid.NewGuid(),
                    Link = RandomStringGenerator.GenerateRandomLatin(30),
                    IsActive = true,
                };

            var result = await publicLinkRepository.WriteOrUpdateAsync(publicLink);
            var dbPublicLink = await publicLinkRepository.ReadByShopIdAsync(publicLink.ShopId);

            result.Should().BeTrue();
            dbPublicLink.Should().BeEquivalentTo(publicLink);
        }

        [Test]
        public async Task WriteThenReadByPublicLinkLink()
        {
            var publicLink = new PublicLink
                {
                    ShopId = Guid.NewGuid(),
                    Link = RandomStringGenerator.GenerateRandomLatin(30),
                    IsActive = true,
                };

            var result = await publicLinkRepository.WriteOrUpdateAsync(publicLink);
            var dbPublicLink = await publicLinkRepository.ReadByPublicLinkAsync(publicLink.Link);

            result.Should().BeTrue();
            dbPublicLink.Should().BeEquivalentTo(publicLink);
        }

        [Test]
        public async Task ReadEmptyLinkByShopId()
        {
            var dbPublicLink = await publicLinkRepository.ReadByShopIdAsync(Guid.NewGuid());
            dbPublicLink.Should().BeNull();
        }

        [Test]
        public async Task ReadEmptyLinkByPublicLink()
        {
            var dbPublicLink = await publicLinkRepository.ReadByPublicLinkAsync(RandomStringGenerator.GenerateRandomLatin(30));
            dbPublicLink.Should().BeNull();
        }

        [Test]
        public async Task WriteThenUpdateLink()
        {
            var firstLink = RandomStringGenerator.GenerateRandomLatin(30);
            var secondLink = RandomStringGenerator.GenerateRandomLatin(30);

            var publicLink = new PublicLink
                {
                    ShopId = Guid.NewGuid(),
                    Link = firstLink,
                    IsActive = true,
                };

            await publicLinkRepository.WriteOrUpdateAsync(publicLink);

            publicLink.Link = secondLink;

            var result = await publicLinkRepository.WriteOrUpdateAsync(publicLink);
            var dbLinkByShopId = await publicLinkRepository.ReadByShopIdAsync(publicLink.ShopId);
            var dbLinkByFirstLink = await publicLinkRepository.ReadByPublicLinkAsync(firstLink);
            var dbLinkBySecondLink = await publicLinkRepository.ReadByPublicLinkAsync(secondLink);

            result.Should().BeTrue();
            dbLinkByShopId.Should().BeEquivalentTo(publicLink);
            dbLinkByFirstLink.Should().BeNull();
            dbLinkBySecondLink.Should().BeEquivalentTo(publicLink);
        }

        [Test]
        public async Task WriteThenWriteToAnotherShop()
        {
            var link = RandomStringGenerator.GenerateRandomLatin(30);
            var firstShopId = Guid.NewGuid();
            var secondShopId = Guid.NewGuid();

            var firstPublicLink = new PublicLink
                {
                    ShopId = firstShopId,
                    Link = link,
                    IsActive = true,
                };

            var secondPublicLink = new PublicLink
                {
                    ShopId = secondShopId,
                    Link = link,
                    IsActive = true,
                };

            await publicLinkRepository.WriteOrUpdateAsync(firstPublicLink);

            var result = await publicLinkRepository.WriteOrUpdateAsync(secondPublicLink);
            var dbLinkByFirstShopId = await publicLinkRepository.ReadByShopIdAsync(firstShopId);
            var dbLinkBySecondShopId = await publicLinkRepository.ReadByShopIdAsync(secondShopId);
            var dbLinkByLink = await publicLinkRepository.ReadByPublicLinkAsync(link);

            result.Should().BeFalse();
            dbLinkByFirstShopId.Should().BeEquivalentTo(firstPublicLink);
            dbLinkBySecondShopId.Should().BeNull();
            dbLinkByLink.Should().BeEquivalentTo(firstPublicLink);
        }

        [Test]
        public async Task ParallelWrite()
        {
            var link = RandomStringGenerator.GenerateRandomLatin(30);
            var firstShopId = Guid.NewGuid();
            var secondShopId = Guid.NewGuid();

            var firstPublicLink = new PublicLink
                {
                    ShopId = firstShopId,
                    Link = link,
                    IsActive = true,
                };

            var secondPublicLink = new PublicLink
                {
                    ShopId = secondShopId,
                    Link = link,
                    IsActive = true,
                };

            var result = await Task.WhenAll(Task.Run(async () =>
                                                {
                                                    await Task.Delay(10);
                                                    return await publicLinkRepository.WriteOrUpdateAsync(firstPublicLink);
                                                }),
                                            Task.Run(async () =>
                                                {
                                                    await Task.Delay(10);
                                                    return await publicLinkRepository.WriteOrUpdateAsync(secondPublicLink);
                                                }));

            result.Count(x => x).Should().Be(1);
        }

        [Test]
        public async Task SaveLinkInLowerCase()
        {
            var link = RandomStringGenerator.GenerateRandomLatin(30);
            var publicLink = new PublicLink
                {
                    ShopId = Guid.NewGuid(),
                    Link = link.ToUpperInvariant(),
                    IsActive = true,
                };

            var result = await publicLinkRepository.WriteOrUpdateAsync(publicLink);
            var dbPublicLinkUpper = await publicLinkRepository.ReadByPublicLinkAsync(link.ToUpperInvariant());
            var dbPublicLinkLower = await publicLinkRepository.ReadByPublicLinkAsync(link);

            result.Should().BeTrue();
            dbPublicLinkUpper.Link.Should().BeEquivalentTo(link);
            dbPublicLinkLower.Should().BeEquivalentTo(dbPublicLinkUpper);
        }

        [Injected]
        private IPublicLinkRepository publicLinkRepository;
    }
}