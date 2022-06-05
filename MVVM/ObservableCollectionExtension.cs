using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CommentsPlus.MVVM
{
    public static class ObservableCollectionExtensions
    {
        public static ObservableCollection<T> Filter<T>(this ObservableCollection<T> source, Predicate<T> filter)
        {
            var result = new ObservableCollection<T>();
            foreach (var item in source)
            {
                if (filter(item))
                    result.Add(item);
            }

            return result;
        }
    }
}
