using System.Data.SqlClient;
using WebApplication1.ModelsDTOs;


namespace WebApplication1.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IConfiguration _configuration;

    public BookRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> DoesBookExist(int id)
    {
        var query = "SELECT * FROM books WHERE PK = @id";
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@PK", id);
        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        int ifExists = (int)command.ExecuteScalar();
        

        if (ifExists > 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    
    
    
    public async Task<List<GenreDTO>>  GetGenres(int bookId)
    {
        var query = @"SELECT genres.name AS Genre
                        FROM genres
                        JOIN books_genres ON books_genres.FK_genre = genres.PK
                        JOIN books ON books.PK = books_genres.FK_book
                        WHERE books.PK = @bookId";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@PK", bookId);
	    
        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();
      
        var PKOrdinal = reader.GetOrdinal("PK");

        var nameOrdinal = reader.GetOrdinal("name");
        BookDTO bookDto = null;

        while (await reader.ReadAsync())
        {
            if (bookDto is not null)
            {
                bookDto.genres.Add(new GenreDTO()
                {
                    Id = reader.GetInt32(PKOrdinal),
                    Name = reader.GetString(nameOrdinal)
                        
                });
            }
            else
            {
                bookDto = new BookDTO();
                bookDto.genres.Add(new GenreDTO()
                {
                    Id = reader.GetInt32(PKOrdinal),
                    Name = reader.GetString(nameOrdinal)
                        
                });
            }
        }

        return bookDto.genres;
    }

    
    
    public async Task<bool> DoesGenreExist(int id)
    {
        var query = "SELECT * FROM genres WHERE PK = @id";
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@PK", id);
        await connection.OpenAsync();
        int ifExists = (int)command.ExecuteScalar();

        var res = await command.ExecuteScalarAsync();

        if (ifExists > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

 

    public async Task AddNewBookWithGenres(int bookid, string bookTitle, List<GenreDTO> genres)
    {
        foreach (var genre in genres)
        {
             bool doesGenreExist = await DoesGenreExist(genre.Id);
                    if (!doesGenreExist)
                    {
                      throw new Exception("Genre does not exist");
            
                    }
        }
       
        var insert = @"INSERT INTO books VALUES (@bookid, @bookTitle);";
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
	    
        command.Connection = connection;
        command.CommandText = insert;

        command.Parameters.AddWithValue("@PK", bookid);
        command.Parameters.AddWithValue("@name", bookTitle);
        
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        BookDTO bookDto = new BookDTO()
        {
            Id = bookid, Title = bookTitle, genres = genres
        };
        try
        {
            var id = await command.ExecuteScalarAsync();
    
            foreach (var genre in bookDto.genres)
            {
                command.Parameters.Clear();
                command.CommandText = "INSERT INTO books_genres VALUES(@FK_book, @FK_genre)";
                command.Parameters.AddWithValue("@FK_book", bookDto.Id);
                command.Parameters.AddWithValue("@FK_genre", genre.Id);

                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
        

        
    }
}