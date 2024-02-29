using AutoMapper;
using NielsenChannelsReporting.Application.Abstractions;
using NielsenChannelsReporting.Application.Exceptions;
using NielsenChannelsReporting.Infrastructure.Persistence.Contexts;
using NielsenChannelsReporting.Infrastructure.Persistence.Repositories;

namespace NielsenChannelsReporting.Infrastructure.Persistence.UnitOfWorks
{
    public class UniversalUnitOfWork : IUniversalUnitOfWork
    {
        private readonly UniversalContext _dbContext;

        INielsenChannelsReportLogRepository? _nielsenChannelsReportLogRepository;
        IChannelSetRepository? _channelSetRepository;
        IMapper _mapper;
        private bool _disposed = false;

        public UniversalUnitOfWork(UniversalContext context, IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
        }

        public INielsenChannelsReportLogRepository NielsenChannelsReportLogRepository =>
            _nielsenChannelsReportLogRepository ??= new NielsenChannelsReportLogRepository(_dbContext, _mapper);

        public IChannelSetRepository ChannelSetRepository =>
            _channelSetRepository ??= new ChannelSetRepository(_dbContext, _mapper);

        public int Commit()
        {
            return _dbContext.SaveChanges();
        }

        public Task<int> CommitAsync()
        {
            try
            {
                return _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new PersistenceException("Error in database. ");
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing && _dbContext != null)
                {
                    _nielsenChannelsReportLogRepository = null;
                    _channelSetRepository = null;
                    _dbContext.Dispose();
                }
                this._disposed = true;
            }


        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
