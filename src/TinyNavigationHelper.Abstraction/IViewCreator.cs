using System;
using System.Collections.Generic;
using System.Text;

namespace TinyNavigationHelper.Abstraction
{
    public interface IViewCreator<T>
    {
        T Create(Type type);
        T Create(Type type, object parameter);
    }
}
