// ------------------------------------------------------------------------------------------------------
// <copyright file="EthereumController.cs" company="Nomis">
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
using Nomis.Etherscan.Interfaces;
using Nomis.Etherscan.Interfaces.Models;
using Nomis.Utils.Wrapper;
using Swashbuckle.AspNetCore.Annotations;

namespace Nomis.Api.Ethereum
{
    /// <summary>
    /// A controller to aggregate all Ethereum-related actions.
    /// </summary>
    [Route(BasePath)]
    [ApiVersion("1")]
    [SwaggerTag("Ethereum blockchain.")]
    public sealed class EthereumController :
        ControllerBase
    {
        /// <summary>
        /// Base path for routing.
        /// </summary>
        internal const string BasePath = "api/v{version:apiVersion}/ethereum";

        /// <summary>
        /// Common tag for Ethereum actions.
        /// </summary>
        internal const string EthereumTag = "Ethereum";

        private readonly ILogger<EthereumController> _logger;
        private readonly IEthereumScoringService _scoringService;

        /// <summary>
        /// Initialize <see cref="EthereumController"/>.
        /// </summary>
        /// <param name="scoringService"><see cref="IEthereumScoringService"/>.</param>
        /// <param name="logger"><see cref="ILogger{T}"/>.</param>
        public EthereumController(
            IEthereumScoringService scoringService,
            ILogger<EthereumController> logger)
        {
            _scoringService = scoringService ?? throw new ArgumentNullException(nameof(scoringService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get Nomis Score for given wallet address.
        /// </summary>
        /// <param name="address" example="0xde0b295669a9fd93d5f28d9ec85e40f4cb697bae">Ethereum wallet address to get Nomis Score.</param>
        /// <returns>An Nomis Score value and corresponding statistical data.</returns>
        /// <remarks>
        /// Sample request:
        ///     GET /api/v1/ethereum/wallet/0xde0b295669a9fd93d5f28d9ec85e40f4cb697bae/score
        /// </remarks>
        /// <response code="200">Returns Nomis Score and stats.</response>
        /// <response code="400">Address not valid.</response>
        /// <response code="404">No data found.</response>
        /// <response code="500">Unknown internal error.</response>
        [HttpGet("wallet/{address}/score", Name = "GetEthereumWalletScore")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "GetEthereumWalletScore",
            Tags = new[] { EthereumTag })]
        [ProducesResponseType(typeof(Result<EthereumWalletScore>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResult<string>), StatusCodes.Status500InternalServerError)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetEthereumWalletScoreAsync(
            [Required(ErrorMessage = "Wallet address should be set")] string address)
        {
            var result = await _scoringService.GetWalletStatsAsync(address);
            return Ok(result);
        }
    }
}