using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using GraphQL;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Policy = "IsUser")]
    [Route("api/v1/chats")]
    public class ChatController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IMatchService _matchService;
        private readonly IChatService _chatService;

        public ChatController(
            IChatService chatService, 
            IMatchService matchService, 
            IMapper mapper, 
            IUserService userService)
        {
            _mapper = mapper;
            _chatService = chatService;
            _userService = userService;
            _matchService = matchService;
        }

        [HttpPost("")]
        public async Task<ActionResult<MessageResultDto>> SendMessage(SendMessageDto data)
        {
            try
            {
                int id = _userService.GetConnectedUser(User);

                // if (!(await _matchService.CheckIfUserIsMatch(id, data.Receiver.ToString())).Status) return BadRequest("You must be matching with this user to send a message");

                Chat? newChat = await _chatService.SendMessage(data, id);

                var result = _mapper.Map<MessageResultDto>(newChat);

                return Ok(result);
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

                var data = await _chatService.GetMessages(id, userId, skip, limit, sort);

                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest("An error occured or no message found " + e);
            }
        }
    }
}