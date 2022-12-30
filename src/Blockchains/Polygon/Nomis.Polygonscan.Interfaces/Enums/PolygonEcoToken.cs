// ------------------------------------------------------------------------------------------------------
// <copyright file="PolygonEcoToken.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.ComponentModel;

namespace Nomis.Polygonscan.Interfaces.Enums
{
    /// <summary>
    /// Polygon eco token.
    /// </summary>
    public enum PolygonEcoToken :
        byte
    {
        /// <summary>
        /// KLIMA.
        /// </summary>
        /// <remarks>
        /// <see href="https://klimadao.finance/"/>
        /// </remarks>
        [Description("klima-dao")]
        Klima = 0,

        /// <summary>
        /// ZRO.
        /// </summary>
        /// <remarks>
        /// <see href="https://www.carb0n.fi/"/>
        /// </remarks>
        [Description("zro")]
        Zro = 1,

        /// <summary>
        /// BCT.
        /// </summary>
        /// <remarks>
        /// <see href="https://toucan.earth/"/>
        /// </remarks>
        [Description("toucan-protocol-base-carbon-tonne")]
        Bct = 2
    }
}