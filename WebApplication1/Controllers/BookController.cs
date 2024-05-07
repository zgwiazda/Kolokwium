using Microsoft.AspNetCore.Mvc;
using WebApplication1.ModelsDTOs;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly IBookRepository _bookRepository;
    
    public BookController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    // zobaczyc gatunki przypisane do danej ksioazki

    [HttpGet("{id}/genres")]
    public async Task<IActionResult> GetGenresFromBook(int id)
    {
        if (!await _bookRepository.DoesBookExist(id))
        {
            return NotFound($"Book with given ID does not exist");
        }

        var genres = await _bookRepository.GetGenres(id);
        return Ok(genres);
    }

    //dodac nowa ksiazke i przypisac gatunki

    [HttpPost]
    public async Task<IActionResult> AddBookWithGenres(BookDTO bookDto)
    {
        foreach (var genre in bookDto.genres)
        {
            if (!await _bookRepository.DoesGenreExist(genre.Id))
                    {
                        return NotFound($"This genre does not exist");
                    }
        }

        await _bookRepository.AddNewBookWithGenres(bookDto.Id, bookDto.Title,bookDto.genres);


        return Created(Request.Path.Value ?? "api/books", bookDto);
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}