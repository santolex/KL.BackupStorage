using System.Collections.Generic;
using System.Linq;
using KL.Backup.Core.Abstractions;

namespace KL.Backup.Core.StoragePolicyRules
{
    public class CompositeRule : IStoragePolicyRule
    {
        private readonly IEnumerable<IStoragePolicyRule> _rules;

        public CompositeRule(IEnumerable<IStoragePolicyRule> rules)
        {
            _rules = rules;
        }
        
        public IEnumerable<IBackup> Apply(IEnumerable<IBackup> backups)
        {
            return _rules.Aggregate(backups, (current, storingPolicy) => storingPolicy.Apply(current));
        }
    }
}