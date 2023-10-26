using System;

namespace yolovalidation;

class DomainException: Exception
{
    public DomainException(string message) : base(message) {}
}