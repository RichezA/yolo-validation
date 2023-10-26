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

        ExceptionsCatchingHelper.TryCatch(exceptions, () => {
            Rule.Invoke();

            if (DependentRules.Any())
            {
                foreach (var dependentRule in DependentRules)
                {
                    ExceptionsCatchingHelper.TryCatch(exceptions, dependentRule.Validate);
                }
            }
        });

        if (exceptions.Any())
        {
            throw new DomainException("TODO: Aggregate exceptions");
        }
    }
}

public class DynamicValidationRule : BaseValidationRule
{
    public DynamicValidationRule(Action rule)
    {
        Rule = rule;
    }
}

public class MessageEmployeurObligatoireValidationRule : BaseValidationRule
{
    public MessageEmployeurObligatoireValidationRule(string messageEmployeur, int statutCurrent, int statutNew)
        : base()
    {
        Rule = () =>
        {
            if ((statutNew == 4
                || statutNew == 5
                || statutNew == 6
                || (statutNew == 7 && (statutCurrent == 9 || statutCurrent == 10)))
                && string.IsNullOrWhiteSpace(messageEmployeur))
            {
                throw new DomainException("C'est de la merde");
            }
        };
    }
}

public class MessageEmployeurPasAutoriseValidationRule : BaseValidationRule
{
    public MessageEmployeurPasAutoriseValidationRule(string messageEmployeur)
        : base()
    {
        Rule = () =>
        {
            if (!string.IsNullOrWhiteSpace(messageEmployeur))
            {
                throw new DomainException("Qu'est-ce que tu me fais la?");
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