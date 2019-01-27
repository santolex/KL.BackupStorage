using System.Collections.Generic;

namespace KL.Backup.Core.Abstractions
{
    public interface IStoragePolicyRule
    {
        IEnumerable<IBackup> Apply(IEnumerable<IBackup> backups);
    }
}