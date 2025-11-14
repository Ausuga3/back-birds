using Microsoft.Data.Sqlite;
using Bird.Modules.Sightings.Domain.Entities;
using Bird.Modules.Sightings.Domain.Repositories;

namespace Bird.Modules.Sightings.Infrastructure;

public class SightingRepository : ISightingRepository
{
    private readonly string _connectionString;

    public SightingRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Sighting> AddAsync(Sighting sighting)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Sightings (Id, Latitude, Longitude, Country, BirdId, Notes, CreatedBy, CreatedAt, UpdatedAt)
            VALUES (@Id, @Latitude, @Longitude, @Country, @BirdId, @Notes, @CreatedBy, @CreatedAt, @UpdatedAt)
        ";

        command.Parameters.AddWithValue("@Id", sighting.Id);
        command.Parameters.AddWithValue("@Latitude", sighting.Latitude);
        command.Parameters.AddWithValue("@Longitude", sighting.Longitude);
        command.Parameters.AddWithValue("@Country", sighting.Country);
        command.Parameters.AddWithValue("@BirdId", sighting.BirdId);
        command.Parameters.AddWithValue("@Notes", sighting.Notes ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@CreatedBy", sighting.CreatedBy ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@CreatedAt", sighting.CreatedAt.ToString("o"));
        command.Parameters.AddWithValue("@UpdatedAt", sighting.UpdatedAt.ToString("o"));

        await command.ExecuteNonQueryAsync();
        return sighting;
    }

    public async Task<Sighting?> GetByIdAsync(string id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, Latitude, Longitude, Country, BirdId, Notes, CreatedBy, CreatedAt, UpdatedAt
            FROM Sightings
            WHERE Id = @Id
        ";
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapToSighting(reader);
        }

        return null;
    }

    public async Task<IEnumerable<Sighting>> GetAllAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, Latitude, Longitude, Country, BirdId, Notes, CreatedBy, CreatedAt, UpdatedAt
            FROM Sightings
            ORDER BY CreatedAt DESC
        ";

        var sightings = new List<Sighting>();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            sightings.Add(MapToSighting(reader));
        }

        return sightings;
    }

    public async Task<IEnumerable<Sighting>> GetByBirdIdAsync(string birdId)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, Latitude, Longitude, Country, BirdId, Notes, CreatedBy, CreatedAt, UpdatedAt
            FROM Sightings
            WHERE BirdId = @BirdId
            ORDER BY CreatedAt DESC
        ";
        command.Parameters.AddWithValue("@BirdId", birdId);

        var sightings = new List<Sighting>();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            sightings.Add(MapToSighting(reader));
        }

        return sightings;
    }

    public async Task<Sighting> UpdateAsync(Sighting sighting)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Sightings
            SET Latitude = @Latitude,
                Longitude = @Longitude,
                Country = @Country,
                BirdId = @BirdId,
                Notes = @Notes,
                CreatedAt = @CreatedAt,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id
        ";

        command.Parameters.AddWithValue("@Id", sighting.Id);
        command.Parameters.AddWithValue("@Latitude", sighting.Latitude);
        command.Parameters.AddWithValue("@Longitude", sighting.Longitude);
        command.Parameters.AddWithValue("@Country", sighting.Country);
        command.Parameters.AddWithValue("@BirdId", sighting.BirdId);
        command.Parameters.AddWithValue("@Notes", sighting.Notes ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@CreatedAt", sighting.CreatedAt.ToString("o"));
        command.Parameters.AddWithValue("@UpdatedAt", sighting.UpdatedAt.ToString("o"));

        await command.ExecuteNonQueryAsync();
        return sighting;
    }

    public async Task DeleteAsync(string id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Sightings WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        await command.ExecuteNonQueryAsync();
    }

    private static Sighting MapToSighting(SqliteDataReader reader)
    {
        return new Sighting
        {
            Id = reader.GetString(0),
            Latitude = reader.GetDouble(1),
            Longitude = reader.GetDouble(2),
            Country = reader.GetString(3),
            BirdId = reader.GetString(4),
            Notes = reader.IsDBNull(5) ? null : reader.GetString(5),
            CreatedBy = reader.IsDBNull(6) ? null : reader.GetString(6),
            CreatedAt = DateTime.Parse(reader.GetString(7)),
            UpdatedAt = DateTime.Parse(reader.GetString(8))
        };
    }
}
