using System;

namespace Telia.GraphQL.Client
{
    public class ArgumentEvaluationException : Exception
    {
        public ArgumentEvaluationException(string argumentName, Exception innerException)
            : base($"Evaluating argument \"{argumentName}\" failed. See inner exception for details.", innerException)
        {

        }
    }
}
