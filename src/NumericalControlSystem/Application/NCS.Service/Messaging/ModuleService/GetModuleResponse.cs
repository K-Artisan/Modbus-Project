﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NCS.Service.ViewModel.Modules;

namespace NCS.Service.Messaging.ModuleService
{
    public class GetModuleResponse : AbstracttResponseBase
    {
        public ModuleView ModuleView { get; set; }
    }
}
