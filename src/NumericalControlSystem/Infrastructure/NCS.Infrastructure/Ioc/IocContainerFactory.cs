using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace NCS.Infrastructure.Ioc
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
