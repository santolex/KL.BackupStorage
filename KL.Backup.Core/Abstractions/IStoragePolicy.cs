using System.Collections.Generic;

namespace KL.Backup.Core.Abstractions
{
    public interface IStoragePolicy
    {
        IEnumerable<IBackup> Apply(IEnumerable<IBackup> backups);
    }
}