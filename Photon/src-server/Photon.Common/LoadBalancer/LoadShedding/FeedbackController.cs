// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedbackController.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FeedbackController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using ExitGames.Logging;

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Photon.Common.LoadBalancer.LoadShedding
{
    [DebuggerDisplay("Upper={UpperBound}; Lower={LowerBound})")]
    public struct FeedbackLevelData
    {
        public int UpperBound;
        public int LowerBound;

        public FeedbackLevelData(int up, int down)
        {
            this.UpperBound = up;
            this.LowerBound = down == -1 ? up : down;
        }

        public override string ToString()
        {
            return $"UpperBound:{this.UpperBound},LowerBound:{this.LowerBound}";
        }
    }

    internal class FeedbackController
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly SortedDictionary<FeedbackLevel, FeedbackLevelData> thresholdValues;

        private int currentInput;

        public FeedbackController(
            FeedbackName feedbackName, SortedDictionary<FeedbackLevel, FeedbackLevelData> thresholdValues, int initialInput, FeedbackLevel initialFeedbackLevel)
        {
            this.thresholdValues = thresholdValues;
            this.FeedbackName = feedbackName;
            this.Output = initialFeedbackLevel;
            this.currentInput = initialInput;
        }

        public FeedbackName FeedbackName { get; }

        public FeedbackLevel Output { get; private set; }

        #region Publics

        public bool SetInput(int input)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("SetInput: {0} value {1}", this.FeedbackName, input);
            }

            if (input > this.currentInput)
            {
                if (this.Output == FeedbackLevel.Highest)
                {
                    return false;
                }

                FeedbackLevel last;
                var next = this.Output;
                do
                {
                    last = next;
                    this.GetUpperThreshold(last, out var threshold);
                    if (input > threshold)
                    {
                        next = this.GetNextHigherThreshold(last);
                    }
                } while (next != last);

                this.currentInput = input;
                if (last != this.Output)
                {
                    if (log.IsInfoEnabled)
                    {
                        log.InfoFormat("Transit {0} from {1} to {2} with input {3}", this.FeedbackName, this.Output, last, input);
                    }

                    this.Output = last;
                    return true;
                }
            }
            else if (input < this.currentInput && this.Output != FeedbackLevel.Lowest)
            {
                this.GetLowerThreshold(this.Output, out var threshold);

                var next = this.Output;

                while (input < threshold && next != FeedbackLevel.Lowest)
                {
                    next = this.GetNextLowerThreshold(next, out threshold);
                }

                this.currentInput = input;
                if (next != this.Output)
                {
                    if (log.IsInfoEnabled)
                    {
                        log.InfoFormat("Transit {0} from {1} to {2} with input {3}", this.FeedbackName, this.Output, next, input);
                    }

                    this.Output = next;
                    return true;
                }
            }

            return false;
        }


        public int GetUpperBoundInput(FeedbackLevel level)
        {
            if (this.thresholdValues.TryGetValue(level, out var levelData))
            {
                return levelData.UpperBound;
            }

            throw new ArgumentOutOfRangeException(nameof(level), $"Non existing level id '{level} was used");
        }

        internal void UpdateThresholds(SortedDictionary<FeedbackLevel, FeedbackLevelData> dict)
        {
            foreach (var feedbackLevelData in dict)
            {
                this.thresholdValues[feedbackLevelData.Key] = feedbackLevelData.Value;
            }
            var t = this.currentInput;
            this.currentInput = 0;
            this.Output = FeedbackLevel.Lowest;
            this.SetInput(t);
        }

        public override string ToString()
        {
            if (this.thresholdValues.Count == 0)
            {
                return "controller is empty";
            }
            var sb = new StringBuilder(100);

            foreach (var feedbackLevelData in this.thresholdValues)
            {
                sb.AppendFormat("Level:{0},upper:{1},lower:{2},", feedbackLevelData.Key, feedbackLevelData.Value.UpperBound,
                    feedbackLevelData.Value.LowerBound);
            }
            return sb.ToString();
        }

        #endregion

        #region Privates

        private FeedbackLevel GetNextHigherThreshold(FeedbackLevel level)
        {
            var next = level;

            while (next != FeedbackLevel.Highest)
            {
                next = this.GetNextHigher(next);
                if (this.GetUpperThreshold(next, out _))
                {
                    return next;
                }
            }

            this.GetUpperThreshold(level, out _);

            return level;
        }

        private FeedbackLevel GetNextLowerThreshold(FeedbackLevel level, out int result)
        {
            var next = level;

            while (next != FeedbackLevel.Lowest)
            {
                next = this.GetNextLower(next);
                if (this.GetLowerThreshold(next, out result))
                {
                    return next;
                }
            }

            this.GetLowerThreshold(next, out result);
            return level;
        }

        private FeedbackLevel GetNextHigher(FeedbackLevel next)
        {
            return next == FeedbackLevel.Highest ? next : next + 1;
        }

        private FeedbackLevel GetNextLower(FeedbackLevel current)
        {
            return current == FeedbackLevel.Lowest ? current : current - 1;
        }

        private bool GetUpperThreshold(FeedbackLevel level, out int result)
        {
            if (this.thresholdValues.TryGetValue(level, out var values))
            {
                result = values.UpperBound;
                return true;
            }

            if (level == FeedbackLevel.Highest)
            {
                result = int.MaxValue;
                return true;
            }

            result = 0;
            return false;
        }

        private bool GetLowerThreshold(FeedbackLevel level, out int result)
        {
            if (this.thresholdValues.TryGetValue(level, out var values))
            {
                result = values.LowerBound;
                return true;
            }
            result = 0;
            return false;
        }

        #endregion
    }
}