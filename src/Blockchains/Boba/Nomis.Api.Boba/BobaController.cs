﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="BobaController.cs" company="Nomis">
// Copyright (c) Nomis, 2022. All rights reserved.
// The Application under the MIT license. See LICENSE file in the solution root for full license information.
// </copyright>
// ------------------------------------------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nomis.Bobascan.Interfaces;
using Nomis.Bobascan.Interfaces.Models;
using Nomis.Utils.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Nomis.Api.Boba
{
    /// <summary>
    /// A controller to aggregate all Boba-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Boba blockchain.")]
    public sealed class BobaController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/boba";

        /// <summary>
        /// Common tag for Boba actions.
        /// </summary>
        internal const string BobaTag = "Boba";

        private readonly ILogger<BobaController> _logger;
        private readonly IBobaScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="BobaController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="IBobaScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public BobaController(
            IBobaScoringService scoringService,
            ILogger<BobaController> logger)
        {
            _scoringService = scoringService ?? throw new ArgumentNullException(nameof(scoringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get Nomis Score for given wallet address.
        /// </summary>
        /// <param name="address" example="0x3229bEfFbbB1aF41369d5fdd8436E9D535055363">Boba wallet address to get Nomis Score.</param>
        /// <returns>An Nomis Score value and corresponding statistical data.</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /api/v1/boba/wallet/0x3229bEfFbbB1aF41369d5fdd8436E9D535055363/score
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetBobaWalletScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetBobaWalletScore",
            Tags = new[] { BobaTag })]
        [ProducesResponseType(typeof(Result<BobaWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetBobaWalletScoreAsync(
            [Required(ErrorMessage = "Wallet address should be set")] string address)
        {
            var result = await _scoringService.GetWalletStatsAsync(address);
            return Ok(result);
        }
    }
}