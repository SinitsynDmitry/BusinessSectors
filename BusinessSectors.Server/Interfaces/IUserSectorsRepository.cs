using BusinessSectors.Server.Data.Models;

namespace BusinessSectors.Server.Interfaces;

/// <summary>
/// Interface for user sectors repository.
/// </summary>
public interface IUserSectorsRepository
{
    /// <summary>
    /// Gets by name async.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>A ValueTask.</returns>
    ValueTask<UserSectors?> GetByNameAsync(string name);

    /// <summary>
    /// Gets by id async.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns>A ValueTask.</returns>
    ValueTask<UserSectors?> GetByIdAsync(int id);

    /// <summary>
    /// Updates the sectors async.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="sectorsIds">The sectors ids.</param>
    /// <returns>A ValueTask.</returns>
    ValueTask<bool> UpdateSectorsAsync(int id, string sectorsIds);
}