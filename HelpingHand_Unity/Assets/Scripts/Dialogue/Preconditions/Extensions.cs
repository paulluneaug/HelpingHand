using System.Collections.Generic;

using UnityUtility.Extensions;

public static partial class Extensions
{
    public static List<TPrecondition> SearchFor<TPrecondition>(this PreconditionBase precondition) where TPrecondition : PreconditionBase
    {
        List<TPrecondition> results = new();
        SearchForRecur(results, precondition);
        return results;
    }

    private static void SearchForRecur<TPrecondition>(List<TPrecondition> results, PreconditionBase precondition)
    {
        switch (precondition)
        {
            case TPrecondition tprec:
                results.Add(tprec);
                break;
            case PreconditionAnd precAnd:
                precAnd.Preconditions?.ForEach(p => SearchForRecur<TPrecondition>(results, p));
                break;
            case PreconditionOr precOr:
                precOr.Preconditions?.ForEach(p => SearchForRecur<TPrecondition>(results, p));
                break;
        }
    }
}