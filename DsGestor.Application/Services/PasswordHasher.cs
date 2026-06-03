using System.Security.Cryptography;

namespace DsGestor.Application.Services;

public static class PasswordHasher
{
    // Mesma regra do seu AuthService
    public static void ValidatePassword(string senha)
    {
        if (senha.Length < 3 ||
            !senha.Any(char.IsUpper) ||
            !senha.Any(char.IsDigit))
        {
            throw new ArgumentException(
                "A senha deve ter no mínimo 3 caracteres, conter letra maiúscula e número.");
        }
    }

    // Formato: {saltBase64}:{hashBase64}
    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split(':');
        if (parts.Length != 2)
            return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] stored = Convert.FromBase64String(parts[1]);

        byte[] computed = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        return stored.SequenceEqual(computed);
    }
}