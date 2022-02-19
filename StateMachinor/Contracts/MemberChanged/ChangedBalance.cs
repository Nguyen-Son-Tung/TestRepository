using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.MemberChanged
{
    public interface ChangedBalance
    {
        Guid CorrelationId { get; }
        int Balance { get; }
        DateTime TriggerAt { get; }
    }
}
