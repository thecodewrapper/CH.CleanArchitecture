using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Application.DTOs;
using CH.CleanArchitecture.Infrastructure.Models;
using CH.Data.Abstractions;
using Microsoft.Extensions.Logging;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    /// <inheritdoc cref="IApplicationConfigurationService"/>
    internal class ApplicationConfigurationService : IApplicationConfigurationService
    {
        private readonly ILogger<ApplicationConfigurationService> _logger;
        private readonly IEntityRepository<ApplicationConfigurationEntity, string> _appConfigRepository;
        private readonly IMapper _mapper;
        private static List<ApplicationConfigurationEntity> _appConfigs;
        private const string MULTIPLE_VALUE_DELIMITER = "|";

        public ApplicationConfigurationService(ILogger<ApplicationConfigurationService> logger, IEntityRepository<ApplicationConfigurationEntity, string> appConfigRepository, IMapper mapper) {
            _logger = logger;
            _appConfigRepository = appConfigRepository;
            _mapper = mapper;
        }

        #region Public Methods

        public Result<IQueryable<ApplicationConfigurationDTO>> GetAll() {
            var serviceResult = new Result<IQueryable<ApplicationConfigurationDTO>>();
            try {
                serviceResult.Data = _mapper.ProjectTo<ApplicationConfigurationDTO>(_appConfigRepository.GetAll());
                return serviceResult;
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to retrieve application configurations from database");
                return serviceResult;
            }
        }

        public async Task<Result> CreateAsync(ApplicationConfigurationDTO dto) {
            var serviceResult = new Result<ApplicationConfigurationDTO>();
            try {
                _appConfigs = null;
                if (dto.IsEncrypted)
                    dto.Value = StringCipherHelper.EncryptWithRandomSalt(dto.Value);

                await _appConfigRepository.AddAsync(_mapper.Map<ApplicationConfigurationEntity>(dto));
                await _appConfigRepository.UnitOfWork.SaveChangesAsync();
                serviceResult.Succeed();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to create application configuration");
            }
            return serviceResult;
        }

        public async Task<Result<ApplicationConfigurationDTO>> DetailsAsync(string id, bool decrypt) {
            var serviceResult = new Result<ApplicationConfigurationDTO>();
            try {
                if (id == null) {
                    throw new Exception("Config not found");
                }

                var entity = await _appConfigRepository.GetSingleAsync(ac => ac.Id == id);

                if (entity == null) {
                    throw new Exception("Config not found");
                }

                if (decrypt && entity != null && entity.IsEncrypted)
                    entity.Value = StringCipherHelper.DecryptWithRandomSalt(entity.Value);

                serviceResult.Data = _mapper.Map<ApplicationConfigurationDTO>(entity);
                serviceResult.Succeed();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to retrieve application configuration");
            }
            return serviceResult;
        }

        public async Task<Result> EditAsync(ApplicationConfigurationDTO dto) {
            var serviceResult = new Result();
            try {
                _logger.LogDebug($"Editing application configuration {nameof(dto.Id)}: {dto.Id}");
                _appConfigs = null;
                var originalEntity = await _appConfigRepository.FindAsync(dto.Id);
                if ((dto.IsEncrypted && !originalEntity.IsEncrypted) || (dto.IsEncrypted && dto.Value != originalEntity.Value))
                    dto.Value = StringCipherHelper.EncryptWithRandomSalt(dto.Value);

                string originalEntityId = originalEntity.Id;
                var updatedEntity = _mapper.Map(dto, originalEntity);
                if (originalEntity == null) {
                    throw new Exception("Config not found");
                }

                updatedEntity.Id = originalEntityId; //to avoid lower case discrepancy during IMapper mapping
                _appConfigRepository.Update(updatedEntity);
                await _appConfigRepository.UnitOfWork.SaveChangesAsync();

                //reset the appconfigs in order to refresh on next request
                _appConfigs = null;
                _logger.LogDebug($"Application configuration '{dto.Id}' edited succesfully");
                serviceResult.Succeed();
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to edit application configuration");
            }
            return serviceResult;
        }

        public async Task<Result> DeleteAsync(string id) {
            var serviceResult = new Result();
            try {
                var entity = await _appConfigRepository.FindAsync(id);
                if (entity == null) {
                    throw new Exception("Page not found");
                }
                _appConfigRepository.Delete(entity);
                await _appConfigRepository.UnitOfWork.SaveChangesAsync();

                serviceResult.Succeed();
                return serviceResult;
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to delete application configuration");
                return serviceResult;
            }
        }

        public Result<string> GetValue(string key) {
            var serviceResult = new Result<string>();
            try {
                var entity = GetConfigurations().Where(m => m.Id == key).FirstOrDefault();
                string value = entity.Value;
                if (entity.IsEncrypted)
                    value = StringCipherHelper.DecryptWithRandomSalt(entity.Value);

                serviceResult.Data = value;
                return serviceResult;
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to retrieve application configuration value");
                return serviceResult;
            }
        }

        public Result<int> GetValueInt(string key) {
            var serviceResult = new Result<int>();
            try {
                var entity = GetConfigurations().Where(m => m.Id == key).FirstOrDefault();
                string value = entity.Value;
                if (entity.IsEncrypted)
                    value = StringCipherHelper.DecryptWithRandomSalt(entity.Value);

                if (int.TryParse(value, out int valueInt)) {
                    serviceResult.Data = valueInt;
                    return serviceResult;
                }

                throw new Exception("Conversion of application configuration value to int was not successful");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to retrieve application configuration value");
                return serviceResult;
            }
        }

        public Result<bool> GetValueBool(string key) {
            var serviceResult = new Result<bool>();
            try {
                var entity = GetConfigurations().Where(m => m.Id == key).FirstOrDefault();
                string value = entity.Value;
                if (entity.IsEncrypted)
                    value = StringCipherHelper.DecryptWithRandomSalt(entity.Value);

                if (bool.TryParse(value, out bool valueBool)) {
                    serviceResult.Data = valueBool;
                    return serviceResult;
                }

                throw new Exception("Convertion of application configuration value to bool is not successful");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to retrieve application configuration value from the database.");
                return serviceResult;
            }
        }

        public Result<DateTime> GetValueDateTime(string key) {
            var serviceResult = new Result<DateTime>();
            try {
                var entity = GetConfigurations().Where(m => m.Id == key).FirstOrDefault();
                string value = entity.Value;
                if (entity.IsEncrypted)
                    value = StringCipherHelper.DecryptWithRandomSalt(entity.Value);

                if (DateTime.TryParse(value, out DateTime valueDate)) {
                    serviceResult.Data = valueDate;
                    return serviceResult;
                }

                throw new Exception("Convertion of application configuration value to bool is not successful");
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to retrieve application configuration value");
                return serviceResult;
            }
        }
        public Result<string[]> GetMultiple(string key) {
            return GetMultiple(key, MULTIPLE_VALUE_DELIMITER);
        }

        public Result<string[]> GetMultiple(string key, string delimiter) {
            var serviceResult = new Result<string[]>();
            try {
                var valueResult = GetValue(key);
                if (!valueResult.IsSuccessful) {
                    return serviceResult.Fail().WithMessage("Unable to fetch multiple keys from");
                }
                serviceResult.Data = valueResult.Data.Split(delimiter);
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to retrieve application configuration value");
            }
            return serviceResult;
        }

        #endregion Public Methods

        private List<ApplicationConfigurationEntity> GetConfigurations() {
            if (_appConfigs == null || _appConfigs.Count == 0) {
                _appConfigs = _appConfigRepository.GetAll().ToList();
            }

            return _appConfigs;
        }
    }
}
