using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Features.Bookings.Dtos;
using BookingRoom.Application.Features.Bookings.Mapper;
using BookingRoom.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingRoom.Application.Features.Bookings.Queries.GetBookingsForMember;

public sealed class GetBookingsForMemberQueryHandler (IAppDbContext context ,IIdentityService identityService ,IUser user):
    IRequestHandler<GetBookingsForMemberQuery,Result<List<BookingDto>>>
{
    private readonly IAppDbContext _context = context;
    private readonly IIdentityService _identityService = identityService;
    private readonly IUser _user = user;

    public async Task<Result<List<BookingDto>>> Handle(GetBookingsForMemberQuery request, CancellationToken cancellationToken)
    {
        var memberId = _user.Id;
        var bookings = await _context.Bookings
            .AsNoTracking().Include(r=>r.Room).Where(x => x.UserId == memberId).ToListAsync(cancellationToken);
        return await bookings.ToDtosAsync(_identityService.GetUserNamesAsync);
    }
}