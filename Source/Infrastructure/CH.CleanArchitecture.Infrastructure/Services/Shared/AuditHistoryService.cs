using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CH.CleanArchitecture.Common;
using CH.CleanArchitecture.Common.Constants;
using CH.CleanArchitecture.Core.Application;
using CH.CleanArchitecture.Core.Application.DTOs;
using CH.CleanArchitecture.Infrastructure.Auditing;
using CH.Data.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CH.CleanArchitecture.Infrastructure.Services
{
    public class AuditHistoryService : IAuditHistoryService
    {
        #region Private Fields

        private readonly IApplicationConfigurationService _appConfigService;
        private readonly IEntityRepository<AuditHistory, int> _auditHistoryRepo;
        private readonly ILogger<AuditHistoryService> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        #endregion Private Fields

        #region Public Constructors

        public AuditHistoryService(IMapper mapper, ILogger<AuditHistoryService> logger, IUnitOfWork unitOfWork, IApplicationConfigurationService appConfigService, IEntityRepository<AuditHistory, int> auditHistoryRepo) {
            _mapper = mapper;
            _logger = logger;
            _appConfigService = appConfigService;
            _auditHistoryRepo = auditHistoryRepo;
            _unitOfWork = unitOfWork;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Retrieves details for a specific audit record.
        /// </summary>
        /// <param name="id">Record Id</param>
        /// <returns>Audit record details.</returns>
        public async Task<Result<AuditHistoryDTO>> Details(int? id) {
            var serviceResult = new Result<AuditHistoryDTO>();
            try {
                if (id == null) {
                    throw new Exception("Page not found");
                }

                var entity = await _auditHistoryRepo.GetSingleAsync(h => h.Id == id);

                if (entity == null) {
                    throw new Exception("Page not found");
                }

                entity.AutoHistoryDetails = JsonConvert.DeserializeObject<AuditHistoryDetails>(entity.Changed);
                serviceResult.Data = _mapper.Map<AuditHistoryDTO>(entity);

                return serviceResult;
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to retrieve audit record");
                return serviceResult;
            }
        }

        /// <summary>
        /// Retrieves all audit records from the database.
        /// </summary>
        /// <returns>IQueryable with audit records.</returns>
        public Result<IQueryable<AuditHistoryDTO>> GetAll() {
            var serviceResult = new Result<IQueryable<AuditHistoryDTO>>();
            try {
                serviceResult.Data = _mapper.ProjectTo<AuditHistoryDTO>(_auditHistoryRepo.GetAll());
                return serviceResult;
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to retrieve data from database");
                return serviceResult;
            }
        }

        /// <summary>
        /// Purge the audit history DB table based on the configuration variable AuditPurgeHistoryTableInterval.
        /// </summary>
        /// <param name="cancellationToken">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>True if succeeds</returns>
        public async Task<Result<bool>> PurgeHistoryAsync(CancellationToken cancellationToken = default) {
            var serviceResult = new Result<bool>();
            try {
                int auditPurgeHistoryTableInterval = _appConfigService.GetValueInt(AppConfigKeys.AUDIT.PURGE_HISTORYTABLE_INTERVAL).Unwrap();
                if (auditPurgeHistoryTableInterval > 0) {
                    var limitDate = DateTime.Today.AddDays(-auditPurgeHistoryTableInterval);

                    var entitiesToDelete = _auditHistoryRepo.GetAll().Where(a => a.Created < limitDate).ToList();

                    _logger.LogDebug($"PurgeHistory: Found {entitiesToDelete.Count} to delete.");

                    _auditHistoryRepo.DeleteRange(entitiesToDelete);
                    if (!cancellationToken.IsCancellationRequested)
                        await _unitOfWork.SaveChangesAsync(cancellationToken);

                    serviceResult.Data = true;
                }
                return serviceResult;
            }
            catch (Exception ex) {
                ServicesHelper.HandleServiceError(ref serviceResult, _logger, ex, "Error while trying to purge audit history records");
                return serviceResult;
            }
        }

        #endregion Public Methods
    }
}
