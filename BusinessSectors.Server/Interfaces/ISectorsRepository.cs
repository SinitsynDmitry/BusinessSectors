using BusinessSectors.Server.Data.Models;

namespace BusinessSectors.Server.Interfaces;

/// <summary>
/// Interface for sectors repository.
/// </summary>
public interface ISectorsRepository
{
    /// <summary>
    /// Gets the sectors async.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns>A ValueTask.</returns>
    ValueTask<IEnumerable<Sector>> GetSectorsAsync(int? userId);

    /// <summary>
    /// Updates the sectors async.
    /// </summary>
    /// <param name="sectors">The sectors.</param>
    /// <returns>A ValueTask.</returns>
    ValueTask<bool> UpdateSectorsAsync(IEnumerable<Sector> sectors);
}