// ------------------------------------------------------------------------------------------------------
// <copyright file="WavesController.cs" company="Nomis">
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
using Nomis.Utils.Wrapper;
using Nomis.WavesExplorer.Interfaces;
using Nomis.WavesExplorer.Interfaces.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace Nomis.Api.Waves
{
    /// <summary>
    /// A controller to aggregate all Waves-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Waves blockchain.")]
    public sealed class WavesController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/waves";

        /// <summary>
        /// Common tag for Waves actions.
        /// </summary>
        internal const string WavesTag = "Waves";

        private readonly ILogger<WavesController> _logger;
        private readonly IWavesScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="WavesController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="IWavesScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public WavesController(
            IWavesScoringService scoringService,
            ILogger<WavesController> logger)
        {
            _scoringService = scoringService ?? throw new ArgumentNullException(nameof(scoringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get Nomis Score for given wallet address.
        /// </summary>
        /// <param name="address" example="3P54ZaN6zfVhzTD8yYkrZWqbPXtMRnzHb3Y">Waves wallet address to get Nomis Score.</param>
        /// <returns>An Nomis Score value and corresponding statistical data.</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /api/v1/waves/wallet/3P54ZaN6zfVhzTD8yYkrZWqbPXtMRnzHb3Y/score
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetWavesWalletScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetWavesWalletScore",
            Tags = new[] { WavesTag })]
        [ProducesResponseType(typeof(Result<WavesWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetWavesWalletScoreAsync(
            [Required(ErrorMessage = "Wallet address should be set")] string address)
        {
            var result = await _scoringService.GetWalletStatsAsync(address);
            return Ok(result);
        }
    }
}