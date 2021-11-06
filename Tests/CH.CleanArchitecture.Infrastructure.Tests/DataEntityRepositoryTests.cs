using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Infrastructure.Models;
using CH.CleanArchitecture.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CH.CleanArchitecture.Infrastructure.Tests
{
    public class DataEntityRepositoryTests : TestBase
    {
        private readonly IEntityRepository<ApplicationConfigurationEntity, string> _appConfigRepo;

        public DataEntityRepositoryTests() {
            _appConfigRepo = ServiceProvider.GetService<IEntityRepository<ApplicationConfigurationEntity, string>>();
        }

        [Fact]
        public void ApplicationConfigurationRepo_Add_AddsEntityToContext() {
            _appConfigRepo.Add(new ApplicationConfigurationEntity() { Id = "test", Value = "test" });
            _appConfigRepo.UnitOfWork.SaveChanges();
            var appConfig = ApplicationContext.ApplicationConfigurations.SingleOrDefault(a => a.Id == "test");

            Assert.NotNull(appConfig);
        }

        [Fact]
        public async Task ApplicationConfigurationRepo_AddAsync_AddsEntityToContext() {
            await _appConfigRepo.AddAsync(new ApplicationConfigurationEntity() { Id = "test", Value = "test" });
            await _appConfigRepo.UnitOfWork.SaveChangesAsync();
            var appConfig = ApplicationContext.ApplicationConfigurations.SingleOrDefault(a => a.Id == "test");

            Assert.NotNull(appConfig);
        }

        [Fact]
        public void ApplicationConfigurationRepo_AddRange_AddsEntitiesToContext() {
            _appConfigRepo.AddRange(new List<ApplicationConfigurationEntity>()
            {
                new ApplicationConfigurationEntity() { Id = "test", Value = "test" },
                new ApplicationConfigurationEntity() { Id = "test2", Value = "test2" }
            });
            _appConfigRepo.UnitOfWork.SaveChanges();
            var appConfigs = ApplicationContext.ApplicationConfigurations.ToList();
            var appConfig1 = appConfigs.SingleOrDefault(a => a.Id == "test");
            var appConfig2 = appConfigs.SingleOrDefault(a => a.Id == "test2");

            Assert.NotNull(appConfig1);
            Assert.NotNull(appConfig2);
        }

        [Fact]
        public async Task ApplicationConfigurationRepo_AddRangeAsync_AddsEntitiesToContext() {
            await _appConfigRepo.AddRangeAsync(new List<ApplicationConfigurationEntity>()
            {
                new ApplicationConfigurationEntity() { Id = "test", Value = "test" },
                new ApplicationConfigurationEntity() { Id = "test2", Value = "test2" }
            });
            await _appConfigRepo.UnitOfWork.SaveChangesAsync();
            var appConfigs = ApplicationContext.ApplicationConfigurations.ToList();
            var appConfig1 = appConfigs.SingleOrDefault(a => a.Id == "test");
            var appConfig2 = appConfigs.SingleOrDefault(a => a.Id == "test2");

            Assert.NotNull(appConfig1);
            Assert.NotNull(appConfig2);
        }

        [Fact]
        public void ApplicationConfigurationRepo_Delete_RemovesEntityFromContext() {
            AddApplicationData(ApplicationContext);
            var dummyAppConfig = ApplicationContext.ApplicationConfigurations.Single(a => a.Id == "dummy1");
            _appConfigRepo.Delete(dummyAppConfig);
            _appConfigRepo.UnitOfWork.SaveChanges();

            var appConfig = ApplicationContext.ApplicationConfigurations.SingleOrDefault(a => a.Id == "dummy1");

            Assert.Null(appConfig);
        }

        [Fact]
        public void ApplicationConfigurationRepo_DeleteRange_RemovesEntitiesFromContext() {
            AddApplicationData(ApplicationContext);
            var dummyAppConfig1 = ApplicationContext.ApplicationConfigurations.Single(a => a.Id == "dummy1");
            var dummyAppConfig2 = ApplicationContext.ApplicationConfigurations.Single(a => a.Id == "dummy2");
            _appConfigRepo.DeleteRange(new List<ApplicationConfigurationEntity>() { dummyAppConfig1, dummyAppConfig2 });
            _appConfigRepo.UnitOfWork.SaveChanges();

            var appConfig1 = ApplicationContext.ApplicationConfigurations.SingleOrDefault(a => a.Id == "dummy1");
            var appConfig2 = ApplicationContext.ApplicationConfigurations.SingleOrDefault(a => a.Id == "dummy2");

            Assert.Null(appConfig1);
            Assert.Null(appConfig2);
        }

        [Fact]
        public void ApplicationConfigurationRepo_Exists_IfEntityExists_ReturnsTrue() {
            AddApplicationData(ApplicationContext);
            bool exists = _appConfigRepo.Exists(a => a.Id == "dummy1");
            Assert.True(exists);
        }

        [Fact]
        public void ApplicationConfigurationRepo_Exists_IfEntityDoesNotExist_ReturnsFalse() {
            bool exists = _appConfigRepo.Exists(a => a.Id == "some entity which doesnt exist");
            Assert.False(exists);
        }

        [Fact]
        public async Task ApplicationConfigurationRepo_ExistsAsync_IfEntityExists_ReturnsTrue() {
            AddApplicationData(ApplicationContext);
            bool exists = await _appConfigRepo.ExistsAsync(a => a.Id == "dummy1");
            Assert.True(exists);
        }

        [Fact]
        public async Task ApplicationConfigurationRepo_ExistsAsync_IfEntityDoesNotExist_ReturnsFalse() {
            bool exists = await _appConfigRepo.ExistsAsync(a => a.Id == "some entity which doesnt exist");
            Assert.False(exists);
        }

        [Fact]
        public void ApplicationConfigurationRepo_GetAll_FetchesAllRecords() {
            AddApplicationData(ApplicationContext);
            int count = ApplicationContext.ApplicationConfigurations.Count();
            var allConfigs = _appConfigRepo.GetAll();

            Assert.NotEmpty(allConfigs);
            Assert.Equal(count, allConfigs.Count());
        }

        [Fact]
        public void ApplicationConfigurationRepo_GetBy_IfSatisfiesPredicate_ReturnsListWithEntity() {
            AddApplicationData(ApplicationContext);
            var appConfigs = _appConfigRepo.GetBy(a => a.Id == "dummy1" && a.Value == "dummyvalue1");

            var appConfigInList = appConfigs.First(a => a.Id == "dummy1" && a.Value == "dummyvalue1");

            Assert.NotNull(appConfigs);
            Assert.NotEmpty(appConfigs);
            Assert.Contains(appConfigs, (a) => a.Id == "dummy1" && a.Value == "dummyvalue1");
        }

        [Fact]
        public void ApplicationConfigurationRepo_GetBy_IfDoesNotSatisfyPredicate_ReturnsEmptyList() {
            AddApplicationData(ApplicationContext);
            var appConfigs = _appConfigRepo.GetBy(a => a.Id == "dummy1" && a.Value == "dummyValue1NotExists");

            Assert.NotNull(appConfigs);
            Assert.Empty(appConfigs);
        }

        [Fact]
        public void ApplicationConfigurationRepo_GetFirst_IfExists_ReturnsFirstEntity() {
            string duplicateValue = "appConfigValue";
            ApplicationContext.ApplicationConfigurations.Add(new ApplicationConfigurationEntity() { Id = "appconfig1", Value = duplicateValue });
            ApplicationContext.ApplicationConfigurations.Add(new ApplicationConfigurationEntity() { Id = "appconfig2", Value = duplicateValue });
            ApplicationContext.SaveChanges();

            var retrievedAppConfig = _appConfigRepo.GetFirst(a => a.Value == duplicateValue);

            Assert.Equal("appconfig1", retrievedAppConfig.Id);
        }

        [Fact]
        public void ApplicationConfigurationRepo_GetFirst_IfDoesNotExist_ReturnsNull() {
            ApplicationContext.Add(new ApplicationConfigurationEntity() { Id = "appconfig1", Value = "appConfig1Value" });

            var retrievedAppConfig = _appConfigRepo.GetFirst(a => a.Value == "some value which does not exist");

            Assert.Null(retrievedAppConfig);
        }

        [Fact]
        public async Task ApplicationConfigurationRepo_GetFirstAsync_IfExists_ReturnsFirstEntity() {
            string duplicateValue = "appConfigValue";
            ApplicationContext.ApplicationConfigurations.Add(new ApplicationConfigurationEntity() { Id = "appconfig1", Value = duplicateValue });
            ApplicationContext.ApplicationConfigurations.Add(new ApplicationConfigurationEntity() { Id = "appconfig2", Value = duplicateValue });
            ApplicationContext.SaveChanges();

            var retrievedAppConfig = await _appConfigRepo.GetFirstAsync(a => a.Value == duplicateValue);

            Assert.Equal("appconfig1", retrievedAppConfig.Id);
        }

        [Fact]
        public async Task ApplicationConfigurationRepo_GetFirstAsync_IfDoesNotExist_ReturnsNull() {
            ApplicationContext.Add(new ApplicationConfigurationEntity() { Id = "appconfig1", Value = "appConfig1Value" });

            var retrievedAppConfig = await _appConfigRepo.GetFirstAsync(a => a.Value == "some value which does not exist");

            Assert.Null(retrievedAppConfig);
        }

        [Fact]
        public void ApplicationConfigurationRepo_GetSingle_IfExists_ReturnsSingleEntity() {
            AddApplicationData(ApplicationContext);
            var appConfig = _appConfigRepo.GetSingle(a => a.Value == "dummyvalue1");

            Assert.NotNull(appConfig);
        }

        [Fact]
        public void ApplicationConfigurationRepo_GetSingle_IfDoesNotExist_ReturnsNull() {
            AddApplicationData(ApplicationContext);
            var appConfig = _appConfigRepo.GetSingle(a => a.Id == "does not exist");

            Assert.Null(appConfig);
        }

        [Fact]
        public void ApplicationConfigurationRepo_Update_UpdatesEntityCorrectly() {
            AddApplicationData(ApplicationContext);
            var appConfig = ApplicationContext.ApplicationConfigurations.Single(a => a.Value == "dummyvalue1");
            string changedValue = "changedValue";
            appConfig.Value = changedValue;
            _appConfigRepo.Update(appConfig);
            _appConfigRepo.UnitOfWork.SaveChanges(true);

            var retrievedAppConfig = ApplicationContext.ApplicationConfigurations.AsNoTracking().Single(a => a.Id == appConfig.Id);

            Assert.Equal(changedValue, retrievedAppConfig.Value);
        }

        [Fact]
        public void ApplicationConfigurationRepo_UpdateRange_UpdatesAllEntitiesCorrectly() {
            AddApplicationData(ApplicationContext);
            var appConfig1 = ApplicationContext.ApplicationConfigurations.Single(a => a.Value == "dummyvalue1");
            var appConfig2 = ApplicationContext.ApplicationConfigurations.Single(a => a.Value == "dummyvalue2");
            string changedValue1 = "changedValue1";
            string changedValue2 = "changedValue2";
            appConfig1.Value = changedValue1;
            appConfig2.Value = changedValue2;

            _appConfigRepo.UpdateRange(new List<ApplicationConfigurationEntity>() { appConfig1, appConfig2 });
            _appConfigRepo.UnitOfWork.SaveChanges(true);

            var retrievedAppConfig1 = ApplicationContext.ApplicationConfigurations.AsNoTracking().Single(a => a.Id == appConfig1.Id);
            var retrievedAppConfig2 = ApplicationContext.ApplicationConfigurations.AsNoTracking().Single(a => a.Id == appConfig2.Id);

            Assert.Equal(changedValue1, retrievedAppConfig1.Value);
            Assert.Equal(changedValue2, retrievedAppConfig2.Value);
        }
    }
}
