﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ClassifiedImage:IEntity
    {
        public virtual long Id { get; set; }
        public virtual bool Archived { get; set; }
        public virtual long IdClassified { get; set; }
        public virtual string UrlIamgen { get; set; }
        public virtual void Archive()
        {
            Archived = true;
        }
        public virtual void Activate()
        {
            Archived = false;
        }
    }
}
