using System.Collections.Generic;
using System.Linq;

namespace yolovalidation;

public interface IValidationStrategy
{
    protected abstract List<BaseValidationRule> ValidationRules { get; }

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
