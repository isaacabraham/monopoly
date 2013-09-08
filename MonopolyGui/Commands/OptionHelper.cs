using Microsoft.FSharp.Core;
using System;

namespace MonopolyGui
{
    public static class OptionHelper
    {
        public static Boolean IsSome<T>(this FSharpOption<T> optionType)
        {
            return FSharpOption<T>.get_IsSome(optionType);
        }

        public static Boolean IsNone<T>(this FSharpOption<T> optionType)
        {
            return FSharpOption<T>.get_IsNone(optionType);
        }
    }
}

