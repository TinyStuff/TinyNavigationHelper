using System;
namespace TinyNavigationHelper.Abstraction
{
    public class ViewCreationException : Exception
    {
        public ViewCreationException(string message) : base(message)
        {
        }
    }
}
