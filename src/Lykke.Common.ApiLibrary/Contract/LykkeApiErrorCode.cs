using System;

namespace Lykke.Common.ApiLibrary.Contract
{
    /// <summary>
    ///     Implementation of <see cref="ILykkeApiErrorCode" />.
    /// </summary>
    public class LykkeApiErrorCode : ILykkeApiErrorCode
    {
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string DefaultMessage { get; }

        /// <summary>
        ///     Constructs a new instance of LykkeApiErrorCode.
        /// </summary>
        /// <param name="name">Error code name.</param>
        /// <param name="defaultMessage">Optional, default error message to show for this error code.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name" /> is null or empty.</exception>
        public LykkeApiErrorCode(string name, string defaultMessage = "")
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            DefaultMessage = string.IsNullOrEmpty(defaultMessage) ? string.Empty : defaultMessage;

            Name = name;
        }

        /// <summary>
        ///     Returns name of the error code.
        /// </summary>
        /// <returns><see cref="Name" /> of the error code.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}