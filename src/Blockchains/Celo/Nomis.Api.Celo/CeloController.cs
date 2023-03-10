// ------------------------------------------------------------------------------------------------------
// <copyright file="CeloController.cs" company="Nomis">
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
using Nomis.Celoscan.Interfaces;
using Nomis.Celoscan.Interfaces.Models;
using Nomis.Utils.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Nomis.Api.Celo
{
    /// <summary>
    /// A controller to aggregate all Celo-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Celo blockchain.")]
    public sealed class CeloController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/celo";

        /// <summary>
        /// Common tag for Celo actions.
        /// </summary>
        internal const string CeloTag = "Celo";

        private readonly ILogger<CeloController> _logger;
        private readonly ICeloScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="CeloController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="ICeloScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public CeloController(
            ICeloScoringService scoringService,
            ILogger<CeloController> logger)
        {
            _scoringService = scoringService ?? throw new ArgumentNullException(nameof(scoringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get Nomis Score for given wallet address.
        /// </summary>
        /// <param name="address" example="0x9e3De18883618E09f6696f1FAC05F991b912906F">Celo wallet address to get Nomis Score.</param>
        /// <returns>An Nomis Score value and corresponding statistical data.</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /api/v1/celo/wallet/0x9e3De18883618E09f6696f1FAC05F991b912906F/score
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetCeloWalletScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetCeloWalletScore",
            Tags = new[] { CeloTag })]
        [ProducesResponseType(typeof(Result<CeloWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetCeloWalletScoreAsync(
            [Required(ErrorMessage = "Wallet address should be set")] string address)
        {
            var result = await _scoringService.GetWalletStatsAsync(address);
            return Ok(result);
        }
    }
}