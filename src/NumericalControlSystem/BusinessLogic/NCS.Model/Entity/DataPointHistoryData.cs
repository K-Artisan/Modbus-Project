using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Domain;

namespace NCS.Model.Entity
{
    public partial class DataPointHistoryData : EntityBase<string>, IAggregateRoot
    {
        //Id属性用Guid生成

        public virtual DataPoint DataPoint { get; set; }
        public virtual DateTime DateTime { get; set; }
        public virtual double Value { get; set; }

        #region EntityBase members

        protected override void Validate()
        {
        }

        #endregion
    }
}
