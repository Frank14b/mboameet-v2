using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace API.Controllers;

[Authorize(Policy = "IsUser")]
[Route("api/v1/groups/chats")]
public class GroupChatController : BaseApiController
{

    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IMatchService _matchService;

    public GroupChatController(DataContext context, IChatService chatService, IMatchService matchService, IMapper mapper, IUserService userService, IConfiguration configuration, IMailService mailService)
    {
        _dataContext = context;
        _mapper = mapper;
        // _chatService = chatService;
        _userService = userService;
        _matchService = matchService;
    }

    [HttpPost("")]
    public async Task<ActionResult<MessageResultDto>> SendMessage(SendMessageDto data)
    {
        try
        {
            int id = _userService.GetConnectedUser(User);

            if (!(await _matchService.CheckIfUserIsMatch(id, data.Receiver)).Status) return BadRequest("You must be matching with this user to send a message");

            var newChat = _mapper.Map<Chat>(data);
            newChat.MessageType = (int)EnumMessageType.text;
            newChat.Sender = id;

            _dataContext.Add(newChat);
            await _dataContext.SaveChangesAsync();

            var result = _mapper.Map<MessageResultDto>(newChat);

            return result;
        }
        catch (Exception e)
        {
            return BadRequest("An error occured or couldn't send your message" + e);
        }
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<ResultPaginate<MessageResultDto>>> GetMessages(int userId, int skip = 0, int limit = 50, string sort = "desc")
    {
        try
        {
            int id = _userService.GetConnectedUser(User);

            var query = _dataContext.Chats.Where(m => m.Status == (int)StatusEnum.enable && ((m.Sender == id && m.Receiver == userId) || (m.Receiver == id && m.Sender == userId)));

            int totalMessage = await query.CountAsync();

            query = sort == "desc" ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt);

            var _result = await query.Skip(skip).Take(limit).ToListAsync();

            var result = _mapper.Map<IEnumerable<MessageResultDto>>(_result);

            return Ok(new ResultPaginate<MessageResultDto>
            {
                Data = result,
                Limit = limit,
                Skip = skip,
                Total = totalMessage
            });
        }
        catch (Exception e)
        {
            return BadRequest("An error occured or no message found " + e);
        }
    }
}