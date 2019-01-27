using System.Collections.Generic;
using System.Linq;
using KL.Backup.Core.Abstractions;

namespace KL.Backup.Core.StoragePolicyRules
{
    public class NoMoreThanRule : IStoragePolicyRule
    {
        private readonly int _maxCount;

        public NoMoreThanRule(int maxCount)
        {
            _maxCount = maxCount;
        }
        
        public IEnumerable<IBackup> Apply(IEnumerable<IBackup> backups)
        {
            return backups.Take(_maxCount);
        }
    }
}