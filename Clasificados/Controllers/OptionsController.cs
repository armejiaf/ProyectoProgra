﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clasificados.Controllers
{
    public class OptionsController:Controller
    {

        public ActionResult TermsConditions()
        {
            return View();
        }

        public ActionResult FrequentQuestions()
        {
            return View();
        }
    
    }
}