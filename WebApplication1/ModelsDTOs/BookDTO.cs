namespace WebApplication1.ModelsDTOs;

public class BookDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<GenreDTO> genres {get; set;}
}

public class GenreDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Genre_Book_AssDTO
{
    public BookDTO Book { get; set; } = null!;
    public GenreDTO Genre { get; set; } = null!;
}
