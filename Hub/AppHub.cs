using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;

namespace API.AppHub;

[Authorize]
public class AppHub : Hub
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IDistributedCache _redisCache;

    public AppHub(IUserService userService, IMapper mapper, IDistributedCache redisCache)
    {
        _userService = userService;
        _mapper = mapper;
        _redisCache = redisCache;
    }

    public override async Task OnConnectedAsync()
    {
        var connectionId = Context.ConnectionId;
        string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId != null)
        {
            Console.WriteLine("userId  === " + connectionId);
            _redisCache.SetString("SocketClients", connectionId, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1)
            });
        }
        await base.OnConnectedAsync();
    }

    public async Task GetProfile()
    {
        string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return;
        
        var cachedValue = _redisCache.GetString("SocketClients");
        Console.WriteLine("cachedValue === " + cachedValue);

        AppUser? userData = await _userService.GetUserById(userId);
        await Clients.Caller.SendAsync("UserProfile", _mapper.Map<ResultUserDto>(userData));
    }
}