using System.Collections.Generic;
using System.Collections.Specialized;

namespace vivoautotestwifi.Pages
{
    public class ObservableList<T> : List<T>,INotifyCollectionChanged
    {

        public ObservableList(int size) : base(size)
        {

        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public new void AddRange(IEnumerable<T> items)
        {
            base.AddRange(items);
            if (CollectionChanged != null)
            {
                CollectionChanged.Invoke(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public new void RemoveRange(int index,int count)
        {
            base.RemoveRange(index, count);
            if (CollectionChanged != null)
            {
                CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

        }

        /// <summary>
        /// add by wzt at 2021.05.13
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            if (CollectionChanged != null)
            {
                CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,item));
            }
        }

        /// <summary>
        /// add by wzt at 2021.05.13
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public new bool Remove(T item)
        {
            bool rebool = base.Remove(item);
            if (CollectionChanged != null)
            {
                CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,item));
            }
            return rebool;
        }

    }
}
