﻿using System;
using System.Collections.Generic;

namespace Microsoft.Languages.Core.Utility {
    public static class ListHelpers {
        public static void RemoveDuplicates<T>(this List<T> list) where T : IComparable<T> {
            if (list == null) {
                throw new ArgumentNullException(nameof(list));
            }

            list.Sort();

            int lastFilledIndex = 0;
            for (int i = 1; i < list.Count; i++) {
                if (list[i].CompareTo(list[lastFilledIndex]) != 0) {
                    list[++lastFilledIndex] = list[i];
                }
            }

            int firstUnfilledIndex = lastFilledIndex + 1;
            if (firstUnfilledIndex < list.Count) {
                list.RemoveRange(firstUnfilledIndex, list.Count - firstUnfilledIndex);
            }
        }
    }
}
