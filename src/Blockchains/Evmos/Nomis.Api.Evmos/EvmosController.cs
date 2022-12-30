// ------------------------------------------------------------------------------------------------------
// <copyright file="EvmosController.cs" company="Nomis">
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
using Nomis.EvmosAPI.Interfaces;
using Nomis.EvmosAPI.Interfaces.Models;
using Nomis.Utils.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

// ReSharper disable InconsistentNaming

namespace Nomis.Api.Evmos
{
    /// <summary>
    /// A controller to aggregate all Evmos-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Evmos blockchain.")]
    public sealed class EvmosController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/evmos";

        /// <summary>
        /// Common tag for Evmos actions.
        /// </summary>
        internal const string EvmosTag = "Evmos";

        private readonly ILogger<EvmosController> _logger;
        private readonly IEvmosScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="EvmosController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="IEvmosScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public EvmosController(
            IEvmosScoringService scoringService,
            ILogger<EvmosController> logger)
        {
            _scoringService = scoringService ?? throw new ArgumentNullException(nameof(scoringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get Nomis Score for given wallet address.
        /// </summary>
        /// <param name="address" example="0x90Cbb44cEdFeA0c841A944CA2AAcc4f153Ae55Bc">Evmos wallet address to get Nomis Score.</param>
        /// <returns>An Nomis Score value and corresponding statistical data.</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /api/v1/evmos/wallet/0x90Cbb44cEdFeA0c841A944CA2AAcc4f153Ae55Bc/score
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetEvmosWalletScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetEvmosWalletScore",
            Tags = new[] { EvmosTag })]
        [ProducesResponseType(typeof(Result<EvmosWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetEvmosWalletScoreAsync(
            [Required(ErrorMessage = "Wallet address should be set")] string address)
        {
            var result = await _scoringService.GetWalletStatsAsync(address);
            return Ok(result);
        }
    }
}