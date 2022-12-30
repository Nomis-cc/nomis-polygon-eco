﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="AeternityController.cs" company="Nomis">
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
using Nomis.AeternityExplorer.Interfaces;
using Nomis.AeternityExplorer.Interfaces.Models;
using Nomis.Utils.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Nomis.Api.Aeternity
{
    /// <summary>
    /// A controller to aggregate all Aeternity-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Aeternity blockchain.")]
    public sealed class AeternityController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/aeternity";

        /// <summary>
        /// Common tag for Aeternity actions.
        /// </summary>
        internal const string AeternityTag = "Aeternity";

        private readonly ILogger<AeternityController> _logger;
        private readonly IAeternityScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="AeternityController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="IAeternityScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public AeternityController(
            IAeternityScoringService scoringService,
            ILogger<AeternityController> logger)
        {
            _scoringService = scoringService ?? throw new ArgumentNullException(nameof(scoringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get Nomis Score for given wallet address.
        /// </summary>
        /// <param name="address" example="ak_E64bTuWTVj9Hu5EQSgyTGZp27diFKohTQWw3AYnmgVSWCnfnD">Aeternity wallet address to get Nomis Score.</param>
        /// <returns>An Nomis Score value and corresponding statistical data.</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /api/v1/aeternity/wallet/ak_E64bTuWTVj9Hu5EQSgyTGZp27diFKohTQWw3AYnmgVSWCnfnD/score
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetAeternityWalletScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetAeternityWalletScore",
            Tags = new[] { AeternityTag })]
        [ProducesResponseType(typeof(Result<AeternityWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetAeternityWalletScoreAsync(
            [Required(ErrorMessage = "Wallet address should be set")] string address)
        {
            var result = await _scoringService.GetWalletStatsAsync(address);
            return Ok(result);
        }
    }
}