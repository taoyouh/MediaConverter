using Converter.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.ViewModels
{
    internal class TranscodeTaskListViewModel
    {
        private readonly TranscodeTaskManager manager;

        private readonly ObservableCollection<TranscodeTaskViewModel> taskViewModels =
            new ObservableCollection<TranscodeTaskViewModel>();

        public ObservableCollection<TranscodeTaskViewModel> TaskViewModels
        {
            get
            {
                return taskViewModels;
            }
        }

        public TranscodeTaskListViewModel(TranscodeTaskManager manager)
        {
            this.manager = manager;

            foreach (var item in manager.Tasks)
            {
                taskViewModels.Add(new TranscodeTaskViewModel(item));
            }

            manager.Tasks.CollectionChanged += Tasks_CollectionChanged;
        }

        private static TranscodeTaskListViewModel _current;

        public static TranscodeTaskListViewModel Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new TranscodeTaskListViewModel(TranscodeTaskManager.Current);
                }

                return _current;
            }
        }

        private void Tasks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (TranscodeTask item in e.OldItems)
                {
                    var toRemove = taskViewModels.First(x => x.Task == item);
                    taskViewModels.Remove(toRemove);
                }
            }

            if (e.NewItems != null)
            {
                foreach (TranscodeTask item in e.NewItems)
                {
                    taskViewModels.Add(new TranscodeTaskViewModel(item));
                }
            }
        }
    }
}
