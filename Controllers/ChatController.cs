using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

namespace API.Controllers
{
    [Authorize(Policy = "IsUser")]
    [Route("api/v1/chats")]
    public class ChatController : BaseApiController
    {

        private readonly DataContext _dataContext;
        // private readonly EmailsCommon _emailsCommon;
        // private readonly IChatService _chatService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IMatchService _matchService;

        public ChatController(DataContext context, IChatService chatService, IMapper mapper, IUserService userService, IConfiguration configuration, IMailService mailService)
        {
            _dataContext = context;
            _mapper = mapper;
            // _chatService = chatService;
            _userService = userService;
        }

        [HttpPost("")]
        public async Task<ActionResult<MessageResultDto>> SendMessage(SendMessageDto data)
        {
            try
            {
                if (!await _matchService.CheckIfUserIsMatch(_userService.GetConnectedUser(User), data.Receiver.ToString())) return BadRequest("You must be matching with this user to send a message");

                var newChat = _mapper.Map<AppChat>(data);
                newChat.MessageType = (int)EnumMessageType.text;
                newChat.Sender = ObjectId.Parse(_userService.GetConnectedUser(User));

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
        public async Task<ActionResult<MessagePaginateResultDto>> GetMessages(string userId, int skip = 0, int limit = 50, string sort = "desc")
        {
            try
            {
                var query = _dataContext.Chats.Where(m => m.Status == (int)StatusEnum.enable && ((m.Sender.ToString() == _userService.GetConnectedUser(User) && m.Receiver.ToString() == userId) || (m.Receiver.ToString() == _userService.GetConnectedUser(User) && m.Sender.ToString() == userId)));

                IQueryable<AppChat> orderedQuery;
                if (sort == "desc")
                {
                    orderedQuery = query.OrderByDescending(x => x.CreatedAt);
                }
                else
                {
                    orderedQuery = query.OrderBy(x => x.CreatedAt);
                }

                var _result = await orderedQuery.Skip(skip).Take(limit).ToListAsync();
                var result = _mapper.Map<IEnumerable<MessageResultDto>>(_result);
                var message = new MessagePaginateResultDto
                {
                    Data = result,
                    Limit = limit,
                    Skip = skip,
                    Total = query.Count()
                };

                return Ok(message);
            }
            catch (Exception e)
            {
                return BadRequest("An error occured or no message found " + e);
            }
        }
    }
}