// ------------------------------------------------------------------------------------------------------
// <copyright file="ArbitrumOneController.cs" company="Nomis">
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
using Nomis.Arbiscan.Interfaces;
using Nomis.Arbiscan.Interfaces.Models;
using Nomis.Utils.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Nomis.Api.ArbitrumOne
{
    /// <summary>
    /// A controller to aggregate all Arbitrum One-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Arbitrum One blockchain.")]
    public sealed class ArbitrumOneController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/arbitrumone";

        /// <summary>
        /// Common tag for Arbitrum One actions.
        /// </summary>
        internal const string ArbitrumOneTag = "ArbitrumOne";

        private readonly ILogger<ArbitrumOneController> _logger;
        private readonly IArbitrumOneScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="ArbitrumOneController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="IArbitrumOneScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public ArbitrumOneController(
            IArbitrumOneScoringService scoringService,
            ILogger<ArbitrumOneController> logger)
        {
            _scoringService = scoringService ?? throw new ArgumentNullException(nameof(scoringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get Nomis Score for given wallet address.
        /// </summary>
        /// <param name="address" example="0xc9ee9eE4E1346b50ce08cB6ddB81481cc22E4b9b">Arbitrum One wallet address to get Nomis Score.</param>
        /// <returns>An Nomis Score value and corresponding statistical data.</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /api/v1/arbitrumone/wallet/0xc9ee9eE4E1346b50ce08cB6ddB81481cc22E4b9b/score
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetArbitrumOneWalletScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetArbitrumOneWalletScore",
            Tags = new[] { ArbitrumOneTag })]
        [ProducesResponseType(typeof(Result<ArbitrumOneWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetArbitrumOneWalletScoreAsync(
            [Required(ErrorMessage = "Wallet address should be set")] string address)
        {
            var result = await _scoringService.GetWalletStatsAsync(address);
            return Ok(result);
        }
    }
}