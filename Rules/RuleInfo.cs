using System;
using UnityEngine;
using antunity.GameData;

namespace antunity.GameSystems.Rules
{
    [Serializable]
    public struct RuleInfo<TAction> : IRule, ICopyable<RuleInfo<TAction>> where TAction : struct
    {
        #region ICopyable

        public RuleInfo<TAction> Copy()
        {
            return new RuleInfo<TAction>
            {
                hasData = hasData.Copy(),
                compareToValue = compareToValue.Copy(),
                compareToData = compareToData.Copy()
            };
        }

        #endregion ICopyable

        [SerializeField] private EnumDataValues<TAction, HasDataRuleStruct> hasData;

        [SerializeField] private EnumDataValues<TAction, CompareToValueStruct> compareToValue;

        [SerializeField] private EnumDataValues<TAction, CompareToDataRuleStruct> compareToData;

        #region IRule

        public RuleResult Evaluate(IGameContext context)
        {
            foreach (var rule in hasData.Values)
            {
                var result = rule.Evaluate(context);
                if (!result.IsSuccess)
                    return result;
            }

            foreach (var rule in compareToValue.Values)
            {
                var result = rule.Evaluate(context);
                if (!result.IsSuccess)
                    return result;
            }

            foreach (var rule in compareToData.Values)
            {
                var result = rule.Evaluate(context);
                if (!result.IsSuccess)
                    return result;
            }

            return RuleResult.Success();
        }

        #endregion IRule
    }
}