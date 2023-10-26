using System.Collections.Generic;
using System.Linq;

namespace yolovalidation;

public abstract class BaseValidationStrategy
{
    protected List<BaseValidationRule> ValidationRules { get; init;}

    protected BaseValidationStrategy()
    {
        ValidationRules = new List<BaseValidationRule>();
    }

    public void Apply()
    {
        var exceptions = new List<Exception>();

        ValidationRules.ForEach(rule => ExceptionsCatchingHelper.TryCatch(exceptions, rule.Validate));

        if (exceptions.Any())
        {
            throw new DomainException("TODO: Find a way to insert the exception list while trying to preserve the stack trace.");
        }
    }
}

public class StatutValidationMessageStrategy : BaseValidationStrategy
{
    public StatutValidationMessageStrategy(int demandeStatut, int newStatut, string message, decimal lastV, decimal VRef)
        : base()
    {
        ValidationRules = new List<BaseValidationRule>
        {
            new MessageEmployeurObligatoireValidationRule(message, demandeStatut, newStatut),
            new DynamicValidationRule(() => {
                if ((newStatut == 7
                    || (newStatut == 14 && lastV != VRef))
                    && string.IsNullOrWhiteSpace(message))
                    {
                        throw new DomainException("C'est requis dans cette situation");
                    }
            }),
            new DynamicValidationRule(() => {
                if (newStatut != 14 && !string.IsNullOrWhiteSpace(message))
                {
                    throw new DomainException("Message non nécessaire");
                }
            }),
        };
    }
}
