using System;
using System.Collections.Generic;
using System.Linq;
using KL.Backup.Core.Abstractions;

namespace KL.Backup.Core.StoragePolicyRules
{
    public class OlderThanRule : IStoragePolicyRule
    {
        private readonly TimeSpan _interval;

        public OlderThanRule(TimeSpan interval)
        {
            _interval = interval;
        }
        
        public IEnumerable<IBackup> Apply(IEnumerable<IBackup> backups)
        {
            var now = DateTime.Now;

            return backups.Where(x => (now - x.Created) > _interval);
        }
    }
}