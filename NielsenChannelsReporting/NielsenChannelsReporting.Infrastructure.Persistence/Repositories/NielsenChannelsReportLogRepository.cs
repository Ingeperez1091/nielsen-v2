using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NielsenChannelsReporting.Application.Abstractions;
using NielsenChannelsReporting.Application.Exceptions;
using NielsenChannelsReporting.Application.Models;
using NielsenChannelsReporting.Infrastructure.Persistence.Contexts;
using ContextEntities = NielsenChannelsReporting.Infrastructure.Persistence.Models;

namespace NielsenChannelsReporting.Infrastructure.Persistence.Repositories
{
    public class NielsenChannelsReportLogRepository : INielsenChannelsReportLogRepository
    {
        private readonly DbSet<ContextEntities.NielsenChannelsReportLog> _dbSet;
        private readonly IMapper _mapper;
        const string REPOSITORY_NAME = nameof(NielsenChannelsReportLogRepository);


        public NielsenChannelsReportLogRepository(UniversalContext universalContext, IMapper mapper)
        {
            _dbSet = universalContext.NielsenChannelsReportLogs;
            _mapper = mapper;
        }

        public async Task<NielsenChannelsReportLog> AddAsync(NielsenChannelsReportLog reportLog)
        {
            try
            {
                var dbModel = _mapper.Map<ContextEntities.NielsenChannelsReportLog>(reportLog);
                await _dbSet.AddAsync(dbModel);

                return _mapper.Map<NielsenChannelsReportLog>(dbModel);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(REPOSITORY_NAME, nameof(AddAsync), ex.Message);
            }

        }

        public async Task<NielsenChannelsReportLog> GetByIdAsync(long Id)
        {
            try
            {
                var channelSet = await _dbSet.FirstOrDefaultAsync(x => x.Id == Id);
                return _mapper.Map<NielsenChannelsReportLog>(channelSet);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(REPOSITORY_NAME, nameof(GetByIdAsync), ex.Message);
            }

        }

        public async Task<NielsenChannelsReportLog> GetLastSuccessReportAsync(string environment)
        {
            try
            {
                var reportLog = await _dbSet.AsQueryable()
                                        .Where(x => x.IsSuccess && x.Environment == environment)
                                        .OrderByDescending(x => x.ProcessDate)
                                        .FirstOrDefaultAsync();
                return _mapper.Map<NielsenChannelsReportLog>(reportLog);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(REPOSITORY_NAME, nameof(GetLastSuccessReportAsync), ex.Message);
            }

        }

    }
}
