using Asp.Versioning;
using BookingRoom.Application.Common.Security;
using BookingRoom.Application.Features.Rooms.Commands.CreateRoom;
using BookingRoom.Application.Features.Rooms.Commands.UpdateRoom;
using BookingRoom.Application.Features.Rooms.Dtos;
using BookingRoom.Application.Features.Rooms.Queries.GetRoomById;
using BookingRoom.Application.Features.Rooms.Queries.GetRooms;
using BookingRoom.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingRoom.Api.Controllers;
[ApiVersion("1.0")]
[Route("api/rooms")]
public sealed class RoomController (ISender sender):ApiController
{
    private readonly ISender _sender = sender;

    #region Get Rooms
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.RoomsRead)]
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
    #endregion
   #region Create Room
   [HttpPost]
   [Authorize(Policy = AuthorizationPolicies.RoomsWrite)]
   [ProducesResponseType(typeof(Result<RoomDto>), StatusCodes.Status201Created)]
   [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status400BadRequest)]
   [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status409Conflict)]
   [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status500InternalServerError)]
   public async Task<IActionResult> CreateRoomAsync([FromBody] CreateRoomCommand command, CancellationToken cancellationToken)
   {
       var result = await _sender.Send(command, cancellationToken);
       return result.Match(
            onValue: room =>
            {
                Result<RoomDto> response = room;
                return CreatedAtAction(nameof(GetById),new {id=room.RoomId}, response);
            },
            onError: Problem);
   }
   #endregion
   #region Get Room By Id
   [ProducesResponseType(typeof(Result<RoomDto>), StatusCodes.Status200OK)]
   [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status400BadRequest)]
   [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status404NotFound)]
   [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status500InternalServerError)]
   [HttpGet("{id:guid}")]
   [Authorize(Policy = AuthorizationPolicies.RoomsRead)]
   public async Task<IActionResult> GetById(Guid id)
   {
       var result = await _sender.Send(new GetRoomByIdQuery(id));
       return result.Match(
           onValue: room =>
           {
               Result<RoomDto> response = room;
               return Ok(response);
           },
           onError: errors => Problem(errors));
   }
   #endregion
    #region Update Room
    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.RoomsWrite)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateRoomAsync(
        Guid id,
        [FromBody] UpdateRoomCommand command,
        CancellationToken cancellationToken)
    {
        var request = command with { RoomId = id };
        var result = await _sender.Send(request, cancellationToken);

        return result.Match(
            onValue: _ => NoContent(),
            onError: errors => Problem(errors));
    }
    #endregion
    #region Delete Room
    #endregion

 
    
}
