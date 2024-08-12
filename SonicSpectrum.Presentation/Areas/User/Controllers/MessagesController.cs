using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonicSpectrum.Application.Repository.Abstract;
using SonicSpectrum.Application.WebSockets;
using SonicSpectrum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonicSpectrum.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly WebSocketHandler _webSocketHandler;

        public MessagesController(IUnitOfWork unitOfWork, WebSocketHandler webSocketHandler)
        {
            _unitOfWork = unitOfWork;
            _webSocketHandler = webSocketHandler;
        }

        [HttpPost("send")]
        public async Task<ActionResult<MessageDto>> SendMessage([FromBody] MessageDto messageDto)
        {
            var sentMessageDto = await _unitOfWork.MessageService.SendMessageAsync(messageDto);
            await _webSocketHandler.NotifyClientsAsync(sentMessageDto); 
            return Ok(sentMessageDto);
        }

        [HttpGet("{userId}/{otherUserId}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(string userId, string otherUserId, DateTime? fromDate = null)
        {
            var messages = await _unitOfWork.MessageService.GetMessagesAsync(userId, otherUserId, fromDate);
            return Ok(messages);
        }


        [HttpPost("mark-as-read")]
        public async Task<IActionResult> MarkAsRead([FromBody] Guid messageId)
        {
            await _unitOfWork.MessageService.MarkAsReadAsync(messageId);
            return Ok();
        }
    }
}
