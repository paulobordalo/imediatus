using Ardalis.Specification;

namespace imediatus.Framework.Core.Specifications;

public class OrderedSpecificationBuilder<T> : IOrderedSpecificationBuilder<T>
{
    public Specification<T> Specification { get; }

    public bool IsChainDiscarded { get; set; }

    public OrderedSpecificationBuilder(Specification<T> specification)
        : this(specification, isChainDiscarded: false)
    {
    }

    public OrderedSpecificationBuilder(Specification<T> specification, bool isChainDiscarded)
    {
        Specification = specification;
        IsChainDiscarded = isChainDiscarded;
    }
}
