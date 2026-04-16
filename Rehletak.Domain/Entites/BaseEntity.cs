using System;
using System.Collections.Generic;
using System.Text;

namespace Rehletak.Domain.Entites
{
    public class BaseEntity<TKey>
    {
        public TKey id { get; set; }
    }
}
