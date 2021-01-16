using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NCS.Service.ViewModel.Modules;
using NCS.Model.Entity;

namespace NCS.Service.Mapping
{
    public static class ModuleMapper
    {
        public static ModuleView ConverToModuleView(this Module module)
        {
            ModuleView moduleView = new ModuleView();

            moduleView.ModuleId = module.Id;
            moduleView.Number = module.Number;
            moduleView.Name = module.Name;
            moduleView.Description = module.Description;     

            return moduleView;
        }

        public static IEnumerable<ModuleView> ConverToModuleViews(this IEnumerable<Module> modules)
        {
            List<ModuleView> moduleViews = new List<ModuleView>();

            foreach (var module in modules)
            {
                ModuleView moduleView = module.ConverToModuleView();
                moduleViews.Add(moduleView);
            }

            return moduleViews;
        }
    }
}
