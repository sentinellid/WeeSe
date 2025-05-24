using WeeSe.Models;

namespace WeeSe.Services
{
    public interface IAuthService
    {
        Task<bool> ValidateUserAsync(string username, string password);
        Task<AnagraficaAccesso?> GetUserAsync(string username);
    }
}
