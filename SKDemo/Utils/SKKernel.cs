using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKDemo.Utils
{
    public static class SKKernel
    {
        private static Kernel mKernel;

        public static Kernel GetKernel()
        {
            if (mKernel == null)
            {
                mKernel = Kernel.CreateBuilder()
                      .AddAzureOpenAIChatCompletion(MyConfig.AzureOpenAIDeploymentId, MyConfig.AzureOpenAIEndPoint, MyConfig.AzureOpenAIKey)
               
                      .Build();
            }


            return mKernel;
        }
    }
}
