using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NielsenChannelsReporting.Application.Abstractions;
using NielsenChannelsReporting.Application.Configuration;
using NielsenChannelsReporting.Application.Exceptions;
using NielsenChannelsReporting.Application.Models;
using NielsenChannelsReporting.Infrastructure.Persistence.Contexts;
using System.Data;
using ContextEntities = NielsenChannelsReporting.Infrastructure.Persistence.Models;

namespace NielsenChannelsReporting.Infrastructure.Persistence.Repositories
{
    public class ChannelSetRepository : IChannelSetRepository
    {
        private readonly DbSet<ContextEntities.ChannelSet> _dbSet;
        private readonly IMapper _mapper;
        const string REPOSITORY_NAME = nameof(ChannelSetRepository);

        public ChannelSetRepository(UniversalContext universalContext, IMapper mapper)
        {
            _dbSet = universalContext.ChannelSets;
            _mapper = mapper;
        }


        public async Task<ChannelSet> GetChannelSetAsync(long channelId)
        {
            try
            {
                var channelSet = await _dbSet.FirstOrDefaultAsync(x => x.Id == channelId);

                return _mapper.Map<ChannelSet>(channelSet);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(REPOSITORY_NAME, nameof(GetChannelSetAsync), ex.Message);

            }

        }

        public async Task<ChannelSet> GetCurrentChannelSet(ChannelReportSettings _reportSettings)
        {
            try
            {

                DataTable dataTable = new();
                dataTable.Columns.Add("ScConfigName");

                foreach (var alias in _reportSettings.ChannelSetAliases)
                {
                    dataTable.Rows.Add(alias);
                }

                var query = await _dbSet.FromSqlRaw("exec dbo.GetCurrentChannelSet  @Environment, @ChannelSetAliases",
                                            new SqlParameter("@Environment", _reportSettings.Environment),
                                            new SqlParameter("@ChannelSetAliases", SqlDbType.Structured) { Value = dataTable, TypeName = "dbo.ChannelSetAliasType" })
                           .ToListAsync();

                var channelSet = query.SingleOrDefault();
                return _mapper.Map<ChannelSet>(channelSet);
            }
            catch (Exception ex)
            {
                throw new PersistenceException(REPOSITORY_NAME, nameof(GetCurrentChannelSet), ex.Message);
            }

        }
    }
}
