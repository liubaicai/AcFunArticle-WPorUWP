using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcFun.UWP.Control
{
    public class NotRepeatObservableCollection<T> : ObservableCollection<T>
    {
        private readonly Dictionary<object, T> _dictionary = new Dictionary<object, T>(); 

        public void Add(object key, T t)
        {
            if (!_dictionary.ContainsKey(key))
            {
                _dictionary.Add(key,t);
                base.Add(t);
            }
        }

        public new void Clear()
        {
            _dictionary.Clear();
            base.Clear();
        }
    }
}
