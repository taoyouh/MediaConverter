using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Classes
{
    internal static class TranscodingManager
    {
        public static ObservableCollection<TranscodeTask> Tasks
        { get; } = new ObservableCollection<TranscodeTask>();
    }
}
