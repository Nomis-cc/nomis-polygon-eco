﻿// ------------------------------------------------------------------------------------------------------
// <copyright file="CscController.cs" company="Nomis">
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
using Nomis.CscExplorer.Interfaces;
using Nomis.CscExplorer.Interfaces.Models;
using Nomis.Utils.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Nomis.Api.Csc
{
    /// <summary>
    /// A controller to aggregate all CSC-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Csc blockchain.")]
    public sealed class CscController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/csc";

        /// <summary>
        /// Common tag for CSC actions.
        /// </summary>
        internal const string CscTag = "Csc";

        private readonly ILogger<CscController> _logger;
        private readonly ICscScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="CscController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="ICscScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public CscController(
            ICscScoringService scoringService,
            ILogger<CscController> logger)
        {
            _scoringService = scoringService ?? throw new ArgumentNullException(nameof(scoringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get Nomis Score for given wallet address.
        /// </summary>
        /// <param name="address" example="0x1c1BDADD6b167f4A60dfECcC525534Bf0f5BF323">CSC wallet address to get Nomis Score.</param>
        /// <returns>An Nomis Score value and corresponding statistical data.</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /api/v1/csc/wallet/0x1c1BDADD6b167f4A60dfECcC525534Bf0f5BF323/score
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetCscWalletScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetCscWalletScore",
            Tags = new[] { CscTag })]
        [ProducesResponseType(typeof(Result<CscWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetCscWalletScoreAsync(
            [Required(ErrorMessage = "Wallet address should be set")] string address)
        {
            var result = await _scoringService.GetWalletStatsAsync(address);
            return Ok(result);
        }
    }
}