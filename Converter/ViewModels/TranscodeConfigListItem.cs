using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Converter.Classes;

namespace Converter.ViewModels
{
    internal class TranscodeConfigListItem
    {
        public string DisplayName { get; set; }

        public TranscodeConfiguration Configuration { get; set; }
    }
}
