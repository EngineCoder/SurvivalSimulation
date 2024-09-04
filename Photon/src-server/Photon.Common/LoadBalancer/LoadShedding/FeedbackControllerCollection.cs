// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedbackControllerCollection.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FeedbackControllerCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Photon.Common.LoadBalancer.LoadShedding
{
    internal sealed class FeedbackControllerCollection
    {
        private readonly Dictionary<FeedbackName, FeedbackController> values;

        public FeedbackControllerCollection(params FeedbackController[] controller)
        {
            this.values = new Dictionary<FeedbackName, FeedbackController>(controller.Length);
            this.Output = FeedbackLevel.Lowest;
            foreach (var c in controller)
            {
                this.values.Add(c.FeedbackName, c);
                if (c.Output > this.Output)
                {
                    this.Output = c.Output;
                }
            }
        }

        public FeedbackLevel Output { get; private set; }

        public FeedbackLevel CalculateOutput()
        {
            return this.Output = this.values.Values.Max(controller => controller.Output);
        }

        public FeedbackLevel SetInput(FeedbackName key, int input)
        {
            return this.SetInput(key, input, out _);
        }

        public FeedbackLevel SetInput(FeedbackName key, int input, out FeedbackLevel level)
        {
            level = this.Output;

            // Controllers are optional, we don't need to configure them all. 
            if (this.values.TryGetValue(key, out var controller))
            {
                if (controller.SetInput(input))
                {
                    level = controller.Output;
                    if (controller.Output > this.Output)
                    {
                        return this.Output = controller.Output;
                    }

                    if (controller.Output < this.Output)
                    {
                        return this.CalculateOutput();
                    }
                }

                level = controller.Output;
            }

            return this.Output;
        }

        public int GetUpperBoundInput(FeedbackName key, FeedbackLevel level)
        {
            if (this.values.TryGetValue(key, out var controller))
            {
                return controller.GetUpperBoundInput(level);
            }

            throw new ArgumentOutOfRangeException(nameof(key), $"non used controller key '{key}' is used.");
        }

        internal void UpdateFeedbackController(FeedbackName name, SortedDictionary<FeedbackLevel, FeedbackLevelData> dict)
        {
            if (this.values.TryGetValue(name, out var controller))
            {
                controller.UpdateThresholds(dict);
                this.CalculateOutput();
            }
        }

        internal string ToString(FeedbackName name)
        {
            if (this.values.TryGetValue(name, out var controller))
            {
                return $"feedback:{name}, values:{controller}";
            }

            return $"no feedback with name: {name}";
        }
    }
}