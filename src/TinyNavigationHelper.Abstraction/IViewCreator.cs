using System;
using System.Collections.Generic;
using System.Text;

namespace TinyNavigationHelper.Abstraction
{
    public interface IViewCreator<T>
    {
        T CreateView(Type type);
        T CreateView(Type type, object parameter);
    }
}
