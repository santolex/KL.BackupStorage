using System;
using System.Linq;
using System.Threading.Tasks;

namespace KL.Backup.Core.Abstractions
{
    public abstract class BackupWatcher : IBackupWatcher
    {
        protected IStoragePolicy Policy { get; }
        protected IStorageRepository StoreRepository { get; }

        protected BackupWatcher(IStoragePolicy policy, IStorageRepository storeRepository)
        {
            Policy          = policy          ?? throw new ArgumentNullException(nameof(policy));
            StoreRepository = storeRepository ?? throw new ArgumentNullException(nameof(storeRepository));
        }

        public virtual void ApplyPolicy()
        {
            var currentBackups = StoreRepository.List().ToArray();
            
            var backupsToLeave = Policy.Apply(currentBackups);

            StoreRepository.Remove(currentBackups.Except(backupsToLeave));
        }

        public abstract void Start();

        public abstract void Stop();
        
        public virtual void Dispose()
        {
            Stop();
        }
    }
}