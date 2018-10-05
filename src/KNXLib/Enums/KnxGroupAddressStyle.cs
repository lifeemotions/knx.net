using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNXLib.Enums
{
    public enum KnxGroupAddressStyle
    {
        /// <summary>
        /// Group addresses in the style of 'MainGroup/SubGroup'
        /// Main    => [0-31] 
        /// Sub     => [0-2047]
        /// Example GA: 4/1025
        /// </summary>
        TwoLevel,

        /// <summary>
        /// Group addresses in the style of 'MainGroup/MiddleGroup/SubGroup'
        /// Main    => [0-31]
        /// Middle  => [0-7]
        /// Sub     => [0-255]
        /// Example GA: 4/1/25
        /// </summary>
        ThreeLevel,

        /// <summary>
        /// Group addresses in the style of 'SubGroup'
        /// Sub     => [1-65535]
        /// Example GA: 6541
        /// 
        /// Available starting with ETS4
        /// </summary>
        Free
    }
}
