﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="NearController.cs" company="Nomis">
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
using Nomis.Nearblocks.Interfaces;
using Nomis.Nearblocks.Interfaces.Models;
using Nomis.Utils.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Nomis.Api.Near
{
    /// <summary>
    /// A controller to aggregate all Near-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Near blockchain.")]
    public sealed class NearController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/near";

        /// <summary>
        /// Common tag for Near actions.
        /// </summary>
        internal const string NearTag = "Near";

        private readonly ILogger<NearController> _logger;
        private readonly INearScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="NearController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="INearScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public NearController(
            INearScoringService scoringService,
            ILogger<NearController> logger)
        {
            _scoringService = scoringService ?? throw new ArgumentNullException(nameof(scoringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get Nomis Score for given wallet address.
        /// </summary>
        /// <param name="address" example="haismse54.near">Near wallet address to get Nomis Score.</param>
        /// <returns>An Nomis Score value and corresponding statistical data.</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /api/v1/near/wallet/haismse54.near/score
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetNearWalletScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetNearWalletScore",
            Tags = new[] { NearTag })]
        [ProducesResponseType(typeof(Result<NearWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetNearWalletScoreAsync(
            [Required(ErrorMessage = "Wallet address should be set")] string address)
        {
            var result = await _scoringService.GetWalletStatsAsync(address);
            return Ok(result);
        }
    }
}