using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKDemo.Models
{
    public class AzureDocEntity
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Text { get; set; } = "";
        public ReadOnlyMemory<float> TextVector { get; set; }
    }
}
