using Microsoft.SemanticKernel;
using Newtonsoft.Json;
using SKDemo.Models;
using SKDemo.Utils;
using System.ComponentModel;

namespace SKDemo.Plugins
{
    public class FirstPlugin
    {
        [KernelFunction, Description("Get the answer for the even greater question. Not the great question, but the even greater one.")]
        public string GetTheAsnwerToTheGreatQuestion()
        {
            return "24 it is!";
        }        
    }
}
