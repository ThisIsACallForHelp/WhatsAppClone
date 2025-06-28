namespace Web_Service
{
    public interface IEncryptor
    {
        public byte[] GetSalt();
        public string GetHash(byte[] Salt, string Pass);
    }
}
