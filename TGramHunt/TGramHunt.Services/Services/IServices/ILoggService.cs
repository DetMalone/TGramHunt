using System;
using System.Threading.Tasks;

namespace TGramHunt.Services.Services.IServices
{
    public interface ILoggService
    {
        Task Log(Exception ex, string userName);
        Task Log(string message, string userName);
    }
}