using System;
using System.Collections.Generic;

namespace Telia.GraphQL.Client
{
    public class InputObjectValue : Dictionary<string, object>
    {
        public Type ObjectType { get; }

        public InputObjectValue(Type objectType)
        {
            ObjectType = objectType;
        }
    }
}
