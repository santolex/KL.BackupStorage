using System.Collections.Generic;
using System.Linq;
using KL.Backup.Core.Abstractions;

namespace KL.Backup.Core
{
    public class StoragePolicy : IStoragePolicy
    {
        private readonly IStoragePolicyRule[] _rules;
        
        public StoragePolicy(IEnumerable<IStoragePolicyRule> rules)
        {
            _rules = rules.ToArray();
        }
        
        public virtual IEnumerable<IBackup> Apply(IEnumerable<IBackup> backups)
        {
            var orderedByDateDesc = backups.OrderBy(x => x.Created).ToArray();
            
            return _rules.SelectMany(x => x.Apply(orderedByDateDesc)).Distinct();
        }
    }
}