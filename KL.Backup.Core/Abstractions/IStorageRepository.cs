using System;
using System.Collections.Generic;

namespace KL.Backup.Core.Abstractions
{
    public interface IStorageRepository
    {
        void Remove(IBackup backup);
        void Remove(IEnumerable<IBackup> backups);
        IEnumerable<IBackup> List();
    }
}