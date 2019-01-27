using System;
using System.Threading;
using KL.Backup.Core.Abstractions;

namespace KL.Backup.Core
{
    public class KLBackupWatcher : BackupWatcher
    {
        private readonly Timer _timer;
        private readonly TimeSpan _period = TimeSpan.FromHours(1.0);


        public KLBackupWatcher(IStoragePolicy policy, IStorageRepository storeRepository,TimeSpan? period = null) : base(policy, storeRepository)
        {
            _timer = new Timer(TimerCallback);
            
            if (period.HasValue)
                _period = period.Value;
        }

        private void TimerCallback(object state)
        {
            ApplyPolicy();
        }

        public override void Start()
        {
            _timer.Change(TimeSpan.Zero, _period);
        }

        public override void Stop()
        {
            _timer?.Change(Timeout.Infinite,Timeout.Infinite);
        }

        public override void Dispose()
        {
            base.Dispose();
            _timer.Dispose();
        }
    }
}