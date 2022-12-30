﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="CubeController.cs" company="Nomis">
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
using Nomis.Cubescan.Interfaces;
using Nomis.Cubescan.Interfaces.Models;
using Nomis.Utils.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Nomis.Api.Cube
{
    /// <summary>
    /// A controller to aggregate all Cube-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Cube blockchain.")]
    public sealed class CubeController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/cube";

        /// <summary>
        /// Common tag for Cube actions.
        /// </summary>
        internal const string CubeTag = "Cube";

        private readonly ILogger<CubeController> _logger;
        private readonly ICubeScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="CubeController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="ICubeScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public CubeController(
            ICubeScoringService scoringService,
            ILogger<CubeController> logger)
        {
            _scoringService = scoringService ?? throw new ArgumentNullException(nameof(scoringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get Nomis Score for given wallet address.
        /// </summary>
        /// <param name="address" example="0xc800fe5a2294c5ddce06f1be363cb3cd474a6972">Cube wallet address to get Nomis Score.</param>
        /// <returns>An Nomis Score value and corresponding statistical data.</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /api/v1/cube/wallet/0xc800fe5a2294c5ddce06f1be363cb3cd474a6972/score
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetCubeWalletScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetCubeWalletScore",
            Tags = new[] { CubeTag })]
        [ProducesResponseType(typeof(Result<CubeWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetCubeWalletScoreAsync(
            [Required(ErrorMessage = "Wallet address should be set")] string address)
        {
            var result = await _scoringService.GetWalletStatsAsync(address);
            return Ok(result);
        }
    }
}