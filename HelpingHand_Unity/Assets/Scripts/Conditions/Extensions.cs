using System.Collections.Generic;

using UnityUtility.Extensions;

public static partial class Extensions
{
    public static List<TPrecondition> SearchFor<TPrecondition>(this ConditionBase condition) where TPrecondition : ConditionBase
    {
        List<TPrecondition> results = new();
        SearchForRecur(results, condition);
        return results;
    }

    private static void SearchForRecur<TPrecondition>(List<TPrecondition> results, ConditionBase condition)
    {
        switch (condition)
        {
            case TPrecondition tprec:
                results.Add(tprec);
                break;
            case ConditionAnd precAnd:
                precAnd.Preconditions?.ForEach(p => SearchForRecur<TPrecondition>(results, p));
                break;
            case ConditionOr precOr:
                precOr.Preconditions?.ForEach(p => SearchForRecur<TPrecondition>(results, p));
                break;
        }
    }
}