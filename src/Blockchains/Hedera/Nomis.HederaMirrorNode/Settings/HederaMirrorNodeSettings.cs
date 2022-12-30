﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="HederaMirrorNodeSettings.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using Nomis.Utils.Contracts.Common;

namespace Nomis.HederaMirrorNode.Settings
{
    /// <summary>
    /// Hedera Mirror Node settings.
    /// </summary>
    internal class HederaMirrorNodeSettings :
        ISettings
    {
        /// <summary>
        /// API base URL.
        /// </summary>
        /// <remarks>
        /// <see href="https://github.com/hashgraph/hedera-docs/blob/master/docs/mirror-node-api/rest-api.md"/>
        /// </remarks>
        public string? ApiBaseUrl { get; set; }
    }
}