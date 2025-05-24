using Microsoft.EntityFrameworkCore;
using WeeSe.Data;
using WeeSe.Models;
using System.Security.Cryptography;
using System.Text;

namespace WeeSe.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var user = await _context.AnagraficaAccesso
                .FirstOrDefaultAsync(u => u.sUtente == username&&u.bAttivo);

            if (user == null) return false;

            // Verifica password (attualmente in chiaro)
            return VerifyPassword(password, user.sPassword);
        }

        public async Task<AnagraficaAccesso?> GetUserAsync(string username)
        {
            return await _context.AnagraficaAccesso
                .FirstOrDefaultAsync(u => u.sUtente == username&&u.bAttivo);
        }

        private bool VerifyPassword(string password, string storedPassword)
        {
            // Password in chiaro - confronto diretto
            return password == storedPassword;
            
            // Se in futuro vorrai usare MD5:
            // return CreateMD5(password) == storedPassword;
        }

        private string CreateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes).ToLower();
            }
        }

        // Metodo temporaneo per test database
        public async Task<string> TestDatabaseConnection()
        {
            try
            {
                var count = await _context.AnagraficaAccesso.CountAsync();
                return $"Connessione OK. Trovati {count} utenti nella tabella.";
            }
            catch (Exception ex)
            {
                return $"Errore connessione: {ex.Message}";
            }
        }
    }
}
