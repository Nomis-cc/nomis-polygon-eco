// ------------------------------------------------------------------------------------------------------
// <copyright file="PolygonController.cs" company="Nomis">
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
using Nomis.Polygonscan.Interfaces;
using Nomis.Polygonscan.Interfaces.Enums;
using Nomis.Polygonscan.Interfaces.Models;
using Nomis.Utils.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Nomis.Api.Polygon
{
    /// <summary>
    /// A controller to aggregate all Polygon-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Polygon blockchain.")]
    public sealed class PolygonController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/polygon";

        /// <summary>
        /// Common tag for Polygon actions.
        /// </summary>
        internal const string PolygonTag = "Polygon";

        private readonly ILogger<PolygonController> _logger;
        private readonly IPolygonScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="PolygonController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="IPolygonScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public PolygonController(
            IPolygonScoringService scoringService,
            ILogger<PolygonController> logger)
        {
            _scoringService = scoringService ?? throw new ArgumentNullException(nameof(scoringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get Nomis Score for given wallet address.
        /// </summary>
        /// <param name="address" example="0xde0b295669a9fd93d5f28d9ec85e40f4cb697bae">Polygon wallet address to get Nomis Score.</param>
        /// <returns>An Nomis Score value and corresponding statistical data.</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /api/v1/polygon/wallet/0xde0b295669a9fd93d5f28d9ec85e40f4cb697bae/score
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetPolygonWalletScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetPolygonWalletScore",
            Tags = new[] { PolygonTag })]
        [ProducesResponseType(typeof(Result<PolygonWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetPolygonWalletScoreAsync(
            [Required(ErrorMessage = "Wallet address should be set")] string address)
        {
            var result = await _scoringService.GetWalletStatsAsync(address);
            return Ok(result);
        }

        /// <summary>
        /// Get Nomis eco Score for given wallet address.
        /// </summary>
        /// <param name="address" example="0xcAb2863cEFb8Bc7f6d0f1CFd8005011331DcBd3e">Polygon wallet address to get eco Nomis Score.</param>
        /// <param name="ecoToken" example="0">Eco token.</param>
        /// <returns>An Nomis eco Score value and corresponding statistical data.</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /api/v1/polygon/wallet/0x007cBb01a3DD8833Cc5e9e36C49E5Ad343C8F7bc/eco-score?ecoToken=1
        /// </remarks>
        /// <response code="200">Returns Nomis eco Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/eco-score", Name = "GetPolygonWalletEcoScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetPolygonWalletEcoScore",
            Tags = new[] { PolygonTag })]
        [ProducesResponseType(typeof(Result<PolygonWalletEcoScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetPolygonWalletEcoScoreAsync(
            [Required(ErrorMessage = "Wallet address should be set")] string address, PolygonEcoToken ecoToken)
        {
            var result = await _scoringService.GetWalletEcoStatsAsync(address, ecoToken);
            return Ok(result);
        }
    }
}