using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.MediaProperties;

namespace Converter.Classes
{
    public class ContainerConfiguration
    {
        public string Subtype { get; set; }

        public ContainerEncodingProperties EncodingProperties(ContainerEncodingProperties source)
        {
            if (source == null)
            {
                return null;
            }

            var result = new ContainerEncodingProperties()
            {
                Subtype = Subtype ?? source.Subtype
            };

            return result;
        }
    }
}
