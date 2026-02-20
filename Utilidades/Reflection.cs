using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Collections;
/// <summary>
/// A static class for reflection type functions
/// </summary>
public static class Reflection
{
    /// <summary>
    /// Extension for 'Object' that copies the properties to a destination object.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="destination">The destination.</param>
    public static void CopyProperties(this object source, object destination)
    {
        // If any this null throw an exception
        if (source == null || destination == null)
            throw new Exception("Source or/and Destination Objects are null");
        // Getting the Types of the objects
        Type typeDest = destination.GetType();
        Type typeSrc = source.GetType();
        // Collect all the valid properties to map
        var results = from srcProp in typeSrc.GetProperties()
                      let targetProperty = typeDest.GetProperty(srcProp.Name)
                      where srcProp.CanRead
                      && targetProperty != null
                      && (targetProperty.GetSetMethod(true) != null && !targetProperty.GetSetMethod(true).IsPrivate)
                      && (targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) == 0
                      && targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType)
                      select new { sourceProperty = srcProp, targetProperty = targetProperty };
        //map the properties
        foreach (var props in results)
        {
            props.targetProperty.SetValue(destination, props.sourceProperty.GetValue(source, null), null);
        }
    }

    public static (string Path, object Value) GetPathAndValue<T>(Expression<Func<T>> expression, params int[] positions)
    {
        var body = (expression?.Body) ?? throw new ArgumentException("Expression is null", nameof(expression));
        var countPositions = positions.Length - 1;
        var path = new StringBuilder();

        while (body != null)
        {
            switch (body)
            {
                case MemberExpression memberExpression:
                    if (memberExpression.Member.Name.Contains("<>"))
                    {
                        body = null;
                        break;
                    }
                    if (path.Length > 0 && path[0] != '[')
                    {
                        path.Insert(0, ".");
                    }
                    path.Insert(0, memberExpression.Member.Name);

                    body = memberExpression.Expression;
                    break;

                case MethodCallExpression methodCallExpression:
                    if (methodCallExpression.Method.Name == "get_Item" && methodCallExpression.Arguments.Count == 1)
                    {
                        if (countPositions > -1)
                        {
                            path.Insert(0, $"[{positions[countPositions]}].");
                        }
                        body = methodCallExpression.Object;
                        countPositions--;
                    }
                    else
                    {
                        throw new ArgumentException("Unsupported method call in expression", nameof(expression));
                    }
                    break;

                default:
                    body = null;
                    break;
            }
        }

        var compiledExpression = expression.Compile();
        var value = compiledExpression();

        return (path.ToString(), value);
    }

    public static object GetValueByPath(object obj, string path)
    {
        if (obj == null || string.IsNullOrEmpty(path))
            return null;

        string[] properties = path.Split('.');
        object currentValue = obj;

        foreach (string property in properties)
        {
            if (currentValue == null)
                return null;

            Type type = currentValue.GetType();
            PropertyInfo propInfo = type.GetProperty(property);

            if (propInfo == null)
                throw new ArgumentException($"Property '{property}' not found in type '{type.FullName}'");

            currentValue = propInfo.GetValue(currentValue);
        }

        return currentValue;
    }

    public static object GetPathAndValue<T>(T root, Expression<Func<T, object>> expression)
    {
        try
        {
            var visitor = new PathVisitor();
            visitor.Visit(expression);

            var path = string.Join(".", visitor.Path);
            path = string.Join(".", path.Split('.').Reverse());

            var result = GetValueByPath(root, path);

            return result;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public static int FindIndexInCollection<T>(IEnumerable<T> collection, Expression<Func<T, bool>> predicate)
    {
        var list = collection.ToList();
        return list.FindIndex(x => predicate.Compile()(x));
    }

    private class PathVisitor : ExpressionVisitor
    {
        public List<string> Path { get; } = new List<string>();

        protected override Expression VisitMember(MemberExpression node)
        {
            // Add to end instead of inserting at beginning
            Path.Add(node.Member.Name);
            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "get_Item")
            {
                var indexArg = node.Arguments[0];
                if (indexArg is ConstantExpression constExpr)
                {
                    // Add to end instead of inserting at beginning
                    Path.Add($"[{constExpr.Value}]");
                }
                return Visit(node.Object);
            }
            return base.VisitMethodCall(node);
        }
    }

    private static object SafeNavigate(object obj, List<string> propertyNames)
    {
        if (obj == null)
            return null;

        for (int i = 0; i < propertyNames.Count; i++)
        {
            if (obj == null) return null;

            var prop = propertyNames[i];

            if (prop == "[]")
            {
                if (!(obj is IEnumerable enumerable))
                    return null;

                // Get the index from the next property name
                if (i + 1 >= propertyNames.Count)
                    return null;

                var indexStr = propertyNames[i + 1];
                if (!int.TryParse(indexStr, out int index))
                    return null;

                // Skip the next property (index) in the next iteration
                i++;

                // Handle IList directly
                if (obj is IList list)
                {
                    if (index >= 0 && index < list.Count)
                    {
                        obj = list[index];
                        continue;
                    }
                    return null;
                }

                // Handle other IEnumerable types
                var enumerator = enumerable.GetEnumerator();
                var currentIndex = 0;
                while (enumerator.MoveNext())
                {
                    if (currentIndex == index)
                    {
                        obj = enumerator.Current;
                        break;
                    }
                    currentIndex++;
                }
                if (currentIndex != index)
                    return null;

                continue;
            }

            var type = obj.GetType();
            var propInfo = type.GetProperty(prop);
            if (propInfo == null) return null;
            obj = propInfo.GetValue(obj);
        }
        return obj;
    }
}