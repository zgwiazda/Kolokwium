using WebApplication1.ModelsDTOs;

namespace WebApplication1.Repositories;

public interface IBookRepository
{
    // zobaczyc gatunki przypisane do danej ksioazki

    Task<bool> DoesBookExist(int id);

    Task<List<GenreDTO>> GetGenres(int bookId);
    
    

    //dodac nowa ksiazke i przypisac gatunki
    Task<bool> DoesGenreExist(int id);

    Task AddNewBookWithGenres(int bookid, string bookTitle, List<GenreDTO> genres);

}