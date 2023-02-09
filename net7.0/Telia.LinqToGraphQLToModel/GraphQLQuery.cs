using System.Linq.Expressions;

using GraphQLParser.AST;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Telia.LinqToGraphQLToModel.GraphQLParser;
using Telia.LinqToGraphQLToModel.Response;

namespace Telia.LinqToGraphQLToModel;

public class GraphQLQuery<TQueryRoot>
{
    /// <summary>
    /// Query&lt;Schema&gt;(q => q.Countries);
    /// </summary>
    /// <example>
    /// 
    /// // Assume schema:
    /// public class Country
    /// {
    ///     public string CountryCode { get;set; }
    /// }
    /// 
    /// [GraphQlTypeAttribute(name: "Query")]   //Root class must be type 'Query'
    /// public class Schema
    /// {
    ///     [GraphQlFieldAttribute(name: "Countries")]
    ///     public List&lt;Country&gt; Countries() 
    ///     {
    ///         throw new Exception("Do not invoke this, its name is read through the expression-tree");
    ///     }
    /// }
    /// 
    /// var query = new GraphQLLinqQUery&gt;Schema&lt;();
    /// 
    /// var graphQl = query.Query(q => q.Countries);
    /// 
    /// // graphQl variable equals:
    /// // { query: "Query { Countries { CountryCode } } ", variables: { }
    /// </example>
    public string Query<TSelector>(Expression<Func<TQueryRoot, TSelector>> selector)
    {
        return CreateOperation<TQueryRoot, TSelector, string>(selector, OperationType.Query);
    }

    public TSelector Query<TSelector>(Expression<Func<TQueryRoot, TSelector>> selector, Func<string, string> onSendGraphQl)
    {
        return CreateOperation<TQueryRoot, TSelector, TSelector>(selector, OperationType.Query, onSendGraphQl);
    }

    internal TResult CreateOperation<TType, TSelector, TResult>(Expression<Func<TType, TSelector>> selector, OperationType operationType, Func<string, string> onSendGraphQl = null)
    {
        var context = new QueryContext();
        var grouping = new SelectionChainGrouping(context);
        var converter = new SelectionChainConverter(context);
        var visitor = new PathGatheringVisitor(context);
        var expander = new SelectionChainExpander(context);

        visitor.Visit(selector);

        var expanded = expander.Expand();

        context.SelectionChains.AddRange(expanded);

        var groupedChains = grouping.Group();

        var variableDefinitions = new GraphQLVariablesDefinition();
        variableDefinitions.Items = new List<GraphQLVariableDefinition>();
        var variableValues = new Dictionary<string, object>();

        var printer = new Printer();

        var operationDefinition = new GraphQLOperationDefinition
        {
            Name = operationType.ToString().ToGraphQlName(),
            Operation = operationType,
            Variables = variableDefinitions,
            SelectionSet = converter.Convert(groupedChains, variableDefinitions, variableValues)
        };

        var query = printer.Print(operationDefinition);

        var temp = new GraphQLQueryData(query, variableValues);

        var graphqlQuery = JsonConvert.SerializeObject(temp, new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter>()
                {
                    new GraphQLObjectConverter(),
                    new StringEnumConverter()
                }
        });

        if(onSendGraphQl == null)
        {
            return (TResult)(object)graphqlQuery;
        }

        var composer = new ResponseComposer<TType, TSelector>(selector, context);

        var response = onSendGraphQl(graphqlQuery);

        if (response.IsNot()) return default;
        
        var graphQlResponse = JsonConvert.DeserializeObject<GraphQLResponse>(response);

        if (graphQlResponse?.Data == null) return default;

        return (TResult)(object)composer.Compose(graphQlResponse.Data);
    }
}
