using System;
using System.Collections.Generic;
using System.Linq;
using KL.Backup.Core.Abstractions;

namespace KL.Backup.Core.StoragePolicyRules
{
    public class NotOlderThanRule : IStoragePolicyRule
    {
        private readonly TimeSpan _interval;

        public NotOlderThanRule(TimeSpan inteval)
        {
            _interval = inteval;
        }
        
        public IEnumerable<IBackup> Apply(IEnumerable<IBackup> backups)
        {
            var now = DateTime.Now;

            return backups.Where(x => now - x.Created < _interval);
        }
    }
}