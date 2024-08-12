using SonicSpectrum.Domain.Entities;

namespace SonicSpectrum.Application.Repository.Abstract
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDto>> GetMessagesAsync(string userId, string otherUserId, DateTime? fromDate = null);
        Task<MessageDto> SendMessageAsync(MessageDto messageDto);
        Task MarkAsReadAsync(Guid messageId);
    }
}
