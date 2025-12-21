using System;
using UnityEngine;
using antunity.GameData;

namespace antunity.ActionSystems.Rules
{
    public enum RuleFailureCode
    {
        None,
        UnknownFailure,
        InvalidValue,
        MinimumRequirementViolation,
        MaximumRequirementViolation,
        DataPresenceViolation,
    }

    public struct RuleResult
    {
        public static bool operator true(RuleResult original) => original.IsSuccess;

        public static bool operator false(RuleResult original) => !original.IsSuccess;

        public static RuleResult operator !(RuleResult original) => original.IsSuccess ? InvalidValue(original.RequiredData, original.ActualValue) : original;

        public static RuleResult operator & (RuleResult left, RuleResult right) => !left.IsSuccess ? left : right;

        public static RuleResult operator | (RuleResult left, RuleResult right) => left.IsSuccess ? left : right;

        public readonly bool IsSuccess;

        public readonly RuleFailureCode FailureCode;
        
        public readonly IGameDataBase RequiredData; 

        public readonly float ActualValue; 

        public RuleResult(bool success, RuleFailureCode failureCode, IGameDataBase requiredData = null, float actualValue = 0f)
        {
            IsSuccess = success;
            FailureCode = failureCode;
            RequiredData = requiredData;
            ActualValue = actualValue;
        }

        // Static constructors for ease of use
        public static RuleResult Success() => new(true, RuleFailureCode.None, null, 0);
        
        public static RuleResult Fail(RuleFailureCode code, IGameDataBase requiredData, float actualValue) => new(false, code, requiredData, actualValue);

        public static RuleResult DataPresenceViolation(IGameDataBase data, float actualValue) => new(false, RuleFailureCode.DataPresenceViolation, data, actualValue);

        public static RuleResult MaximumRequirementViolation(IGameDataBase data, float actualValue) => new(false, RuleFailureCode.MaximumRequirementViolation, data, actualValue);

        public static RuleResult MinimumRequirementViolation(IGameDataBase data, float actualValue) => new(false, RuleFailureCode.MinimumRequirementViolation, data, actualValue);

        public static RuleResult InvalidValue(IGameDataBase data, float actualValue) => new(false, RuleFailureCode.InvalidValue, data, actualValue);

        public static RuleResult UnknownFailure() => new(false, RuleFailureCode.UnknownFailure, null, 0);
    }

    public interface IRule
    {
        RuleResult Evaluate(IActionContext context);
    }

    [Serializable]
    public abstract class RuleAsset : GameDataAsset<uint>, IRule
    {
        public abstract RuleResult Evaluate(IActionContext context);
    }

    [Serializable]
    [GameDataDrawer(GameDataLayout.Vertical)]
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

        [SerializeField] private EnumDataValues<TAction, HasDataRule> hasData;

        [SerializeField] private EnumDataValues<TAction, CompareToValueRule> compareToValue;

        [SerializeField] private EnumDataValues<TAction, CompareToDataRule> compareToData;

        #region IRule

        public RuleResult Evaluate(IActionContext context)
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