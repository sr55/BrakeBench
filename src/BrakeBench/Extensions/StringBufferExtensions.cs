// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringBufferExtensions.cs" company="HandBrake Project (https://handbrake.fr)">
//   This file is part of the BrakeBench source code - It may be used under the terms of the 3-Clause BSD License
// </copyright>
// <summary>
//   Defines the StringBufferExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BrakeBench.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class StringBufferExtensions
    {
        public static List<string> ToList(this StringBuilder stringBuilder)
        {
            return stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
        }
    }
}
