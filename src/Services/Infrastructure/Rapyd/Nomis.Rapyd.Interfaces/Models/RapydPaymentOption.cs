﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="RapydPaymentOption.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Nomis.Rapyd.Interfaces.Models
{
    /// <summary>
    /// Rapyd payment option.
    /// </summary>
    public class RapydPaymentOption
    {
        /// <summary>
        /// Name of the field.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Type of the field. One of the following values:
        /// <para>boolean</para>
        /// <para>number</para>
        /// <para>string</para>
        /// <para>object - The type of object, for example, customer.</para>
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// A regular expression that defines the format when type is string.
        /// Note: Rapyd uses a unique variant of the regex standard. See <see href="https://docs.rapyd.net/build-with-rapyd/reference/payment-method-type#rapyd-regex">Rapyd Regex</see>.
        /// </summary>
        [JsonPropertyName("regex")]
        public string? Regex { get; set; }

        /// <summary>
        /// Description of the field.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Whether the field is required for using the payment method.
        /// </summary>
        [JsonPropertyName("is_required")]
        public bool IsRequired { get; set; }

        /// <summary>
        /// Indicates whether the field can be updated with Update Payment Method.
        /// </summary>
        [JsonPropertyName("is_updatable")]
        public bool IsUpdatable { get; set; }
    }
}