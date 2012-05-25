using System;

namespace SimpleCqrs.EventStore.MySql
{
    public static class TypeToStringHelperMethods
    {
        public static string GetString(Type type)
        {
            var typeArray = type.AssemblyQualifiedName.Split(" ".ToCharArray());
            var returnValue = typeArray[0] + " " + typeArray[1].Replace(",", "");
            return returnValue;
        }
    }
}