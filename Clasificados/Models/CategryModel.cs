﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;

namespace Clasificados.Models
{
    public class CategryModel
    {
        public List<Classified> Clasificados { get; set; }
        public string Categoria { get; set; }
    }
}