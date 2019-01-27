using System;

namespace KL.Backup.Core.Abstractions
{
    public interface IBackupWatcher : IDisposable
    {
        void Start();
        void Stop();
    }
}