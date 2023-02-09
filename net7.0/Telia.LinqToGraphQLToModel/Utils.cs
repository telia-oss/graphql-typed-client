using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Telia.LinqToGraphQLToModel;


internal class Utils
{
    public static MethodInfo[] SelectMethods = typeof(Enumerable)
       .GetMethods()
       .Where(e => e.Name == "Select")
       .ToArray();

    public static bool IsLinqSelectMethod(MethodCallExpression methodCallExpression)
    {
        return methodCallExpression.Method.IsGenericMethod &&
            SelectMethods.Contains(methodCallExpression.Method.GetGenericMethodDefinition());
    }

    public static bool IsEnumerable(Type t)
    {
        if (t.IsArray)
            return true;

        if (typeof(IEnumerable).IsAssignableFrom(t))
            return true;

        if (t.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(t.GetGenericTypeDefinition()))
            return true;

        return false;
    }
}
