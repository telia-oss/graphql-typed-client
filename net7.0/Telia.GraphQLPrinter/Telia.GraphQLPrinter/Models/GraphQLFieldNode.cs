using GraphQLParser.AST;

namespace Telia.GraphQLPrinter.Models;

public class GraphQLFieldNode : ASTNode, IHasSelectionSetNode, IHasArgumentsNode, IHasDirectivesNode, INamedNode
{
    public GraphQLName Alias { get; set; }

    public override ASTNodeKind Kind => ASTNodeKind.Field;

    public GraphQLName Name { get; set; }

    public GraphQLSelectionSet SelectionSet { get; set; }

    public GraphQLDirectives Directives { get; set; }

    public GraphQLArguments Arguments { get; set; }
}