namespace SonicSpectrum.Application.Responses
{
    public class ServiceResponses
    {
        public record class GeneralResponse(bool Flag, string Message);
        public record class LoginResponse(bool Flag, string AccsesToken,string RefreshToken, string Message);
    }
}
