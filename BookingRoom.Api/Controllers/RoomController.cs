using Asp.Versioning;
using BookingRoom.Application.Features.Rooms.Dtos;
using BookingRoom.Application.Features.Rooms.Queries.GetRooms;
using BookingRoom.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookingRoom.Api.Controllers;
[ApiVersion("1.0")]
[Route("api/rooms")]
public sealed class RoomController (ISender sender):ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet] 
    [ProducesResponseType(typeof(Result<List<RoomDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRoomsAsync(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetRoomsQuery(), cancellationToken);
        return result.Match(
            onValue: rooms =>
            {
                Result<List<RoomDto>> response = rooms;
                return Ok(response);
            },
            onError: error => Problem(error));
    }
}