using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lykke.Common.ApiLibrary.Authentication
{
    [Obsolete("Is used only for old token management")]
    public interface ILykkePrincipal
    {
        Task<ClaimsPrincipal> GetCurrent();
        void InvalidateCache(string token);
    }
}