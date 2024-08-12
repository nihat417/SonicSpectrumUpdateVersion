using SonicSpectrum.Application.Models;

namespace SonicSpectrum.Application.Repository.Abstract
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
