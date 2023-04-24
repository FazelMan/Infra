using System;

namespace Infra.Shared.Http.Attributes
{
    /// <summary>
    /// Transient members should not be persisted in a persistent storage
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class Transient : Attribute
    {
    }
}
