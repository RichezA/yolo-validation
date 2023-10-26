using System.Collections.Generic;
using System.Linq;

namespace yolovalidation;

public abstract class BaseValidationRule
{
    protected IEnumerable<BaseValidationRule> DependentRules {get;} = new List<BaseValidationRule>();

    public Action Rule { get; init; }

    public void Validate()
    {
        var exceptions = new List<Exception>();

        ExceptionsCatchingHelper.TryCatch(exceptions, Rule);

        if (DependentRules.Any())
        {
            foreach (var dependentRule in DependentRules)
            {
                ExceptionsCatchingHelper.TryCatch(exceptions, dependentRule.Validate);
            }
        }

        if (exceptions.Any())
        {
            throw new DomainException("TODO: Aggregate exceptions");
        }
    }
}

public class MessageEmployeurObligatoireValidationRule : BaseValidationRule
{
    public MessageEmployeurObligatoireValidationRule(string messageEmployeur)
        : base()
    {
        Rule = () =>
        {
            if (string.IsNullOrWhiteSpace(messageEmployeur))
            {
                throw new DomainException("C'est de la merde");
            }
        };
    }
}

public static class ExceptionsCatchingHelper
{
    public static void TryCatch(List<Exception> exceptions, Action action)
    {
        try
        {
            action.Invoke();
        }
        catch (Exception e)
        {
            exceptions.Add(e);
        }
    }
}