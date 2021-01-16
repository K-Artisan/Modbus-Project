using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Azir.Infrastructure.Ioc
{
    public static class IocContainerFactory
    {
        private static IUnityContainer unityContainer;

        public static void InitializeUnityContainer(IUnityContainer unityContainer)
        {
            IocContainerFactory.unityContainer = unityContainer;
        }

        public static IUnityContainer GetUnityContainer()
        {
            return unityContainer;
        }
    }
}
