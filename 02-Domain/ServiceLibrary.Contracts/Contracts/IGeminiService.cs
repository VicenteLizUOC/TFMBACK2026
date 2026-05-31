namespace TFGBack._02_Domain.ServiceLibrary.Contracts.Contracts
{
    public interface IGeminiService
    {
        Task<string> getTaskHelp(string taskName, string taskDescription);
    }
}
