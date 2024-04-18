using NHIABackendService.Core.Exceptions;
using NHIABackendService.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NHIABackendService.Core.Utility
{
    public static class EntityHelper
    {
        public static Type GetPrimaryKeyType(Type entityType)
        {
            foreach (var interfaceType in entityType.GetTypeInfo().GetInterfaces())
                if (interfaceType.GetTypeInfo().IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IEntity<>))
                    return interfaceType.GenericTypeArguments[0];

            throw new AppGenericException("Can not find primary key type of given entity type: " + entityType + ". Be sure that this entity type implements IEntity<TPrimaryKey> interface");
        }
    }
}
