// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedbackControlSystemSection.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Photon.Common.LoadBalancer.LoadShedding.Configuration
{
    internal class FeedbackControlSystemSection
    {

        /// <summary>
        /// How many values we keep in CPU values history.
        /// amount of values affects speed of reaction on peaks. shorter history faster reaction
        /// </summary>
        public int CPUValuesHistoryLen { get; set; } = 50;

        public List<FeedbackControllerElement> FeedbackControllers { get; } = new List<FeedbackControllerElement>();
    }
}
