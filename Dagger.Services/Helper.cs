using System;

namespace Dagger.Services
{
    /// <summary>
    /// Static methods for common actions. If an action is only ever required by on specific
    /// service, it should be defined within that class as a private method instead of going here.
    /// </summary>
    public static class Helper
    {
        public static bool DirectoryIsResources()
        {
            return true;
        }

        public static bool DirectoryContainsResources()
        {
            throw new NotImplementedException();
        }
    }
}
