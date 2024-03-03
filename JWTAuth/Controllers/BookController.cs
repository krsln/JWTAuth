using JWTAuth.Models;
using JWTAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuth.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    readonly IBookService bookService;

    public BookController(IBookService bookService)
    {
        this.bookService = bookService;
    }

    [HttpPost("GetBooks")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Book>>> GetBooks()
    {
        var result = await bookService.GetBooksAsync();

        return Ok(result);
    }

    [HttpPost("GetBooksById")]
    public async Task<ActionResult<Book>> GetBooksById([FromBody] BookRequest request)
    {
        var result = await bookService.GetBooksByIdAsync(request);

        return Ok(result);
    }
}