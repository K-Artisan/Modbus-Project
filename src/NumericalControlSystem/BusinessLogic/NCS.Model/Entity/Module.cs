using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Infrastructure.Domain;

namespace NCS.Model.Entity
{
    public partial class Module : EntityBase<int>,IAggregateRoot
    {
        public virtual int Number { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        private IList<DataPoint> _dataPoints = new List<DataPoint>();
        public virtual IEnumerable<DataPoint> DataPoints
        {
            get { return _dataPoints; }
        }


        #region EntityBase member

        protected override void Validate()
        {
        }

        #endregion
    }
}
