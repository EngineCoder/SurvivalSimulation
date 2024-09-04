// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueHistory.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ValueHistory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Photon.Common.LoadBalancer.LoadShedding
{
    internal class ValueHistory : Queue<int>
    {
        private int capacity;

        public ValueHistory(int capacity)
            : base(capacity)
        {
            this.capacity = capacity;
        }

        public void Add(int value)
        {
            if (this.Count == this.capacity)
            {
                this.Dequeue();
            }

            this.Enqueue(value);
        }

        public void UpdateCapacity(int cpuValuesHistoryLen)
        {
            if (cpuValuesHistoryLen <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cpuValuesHistoryLen), "history len should be more than 0");
            }
            if (this.capacity <= cpuValuesHistoryLen)
            {
                this.capacity = cpuValuesHistoryLen;
                return;
            }

            this.capacity = cpuValuesHistoryLen;
            while (this.Count > this.capacity)
            {
                this.Dequeue();
            }

        }
    }
}