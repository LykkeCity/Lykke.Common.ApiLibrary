namespace Lykke.Common.ApiLibrary.Contract
{
    /// <summary>
    /// Class for storing common error codes that may happen in API.
    /// </summary>
    public static class LykkeApiCommonErrorCodes
    {
        /// <summary>
        /// One of the provided values was not valid.
        /// </summary>
        public static readonly ILykkeApiErrorCode InvalidInput =
            new LykkeApiErrorCode(nameof(InvalidInput), "One of the provided values was not valid.");
    }
}