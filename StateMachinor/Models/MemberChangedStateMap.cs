using Automatonymous;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StateMachinor.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachinor.Models
{
    public class MemberChangedStateMap : SagaClassMap<MemberChangedState>
    {
        protected override void Configure(EntityTypeBuilder<MemberChangedState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState);
            entity.Property(x => x.Balance);
            entity.Property(x => x.FullName).HasMaxLength(100);
            entity.Property(x => x.MemberId);
            entity.Property(x => x.TriggerAt);
        }
    }
}
