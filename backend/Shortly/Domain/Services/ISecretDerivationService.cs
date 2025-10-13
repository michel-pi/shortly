namespace Shortly.Domain.Services;

public interface ISecretDerivationService
{
    byte[] Derive(string password, int length);
    byte[] GetJwtSigningKey();
}
