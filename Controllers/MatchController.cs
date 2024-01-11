using API.Commons;
using API.Data;
using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Policy = "IsUser")]
    [Route("/api/v1/matches")]
    public class MatchController : BaseApiController
    {
        private readonly DataContext _dataContext;
        // private readonly EmailsCommon _emailsCommon;
        private readonly IMapper _mapper;

        public MatchController(DataContext context, IMapper mapper, IConfiguration configuration, IMailService mailService)
        {
            _dataContext = context;
            _mapper = mapper;
        }

        [HttpPost("")]
        public async Task<ActionResult<MatchesResultDto>> AddMatch(AddMatchDto data)
        {
            try
            {
                
            }
            catch (Exception e)
            {
                return BadRequest("An error occurred when trying to add " + e);
            }
        }
    }
}