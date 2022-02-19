using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.MemberChanged
{
    public interface ChangedName
    {
        Guid CorrelationId { get; }
        string Name { get; }
        DateTime TriggerAt { get; }
    }
}
