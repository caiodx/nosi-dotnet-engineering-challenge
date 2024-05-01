using System.Net;
using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.Mvc;
using NOS.Engineering.Challenge.API.Models;
using NOS.Engineering.Challenge.Database;
using NOS.Engineering.Challenge.Managers;
using NOS.Engineering.Challenge.Models;
using Microsoft.Extensions.Caching.Memory;

namespace NOS.Engineering.Challenge.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ContentController : Controller
{
    private readonly IContentsManager _manager;

    public ContentController(IContentsManager manager)
    {
        _manager = manager;
    }

    [Obsolete("Use V2 instead")]
    [HttpGet]
    public async Task<IActionResult> GetManyContents()
    {
        var contents = await _manager.GetManyContents().ConfigureAwait(false);

        if (!contents.Any())
            return NotFound();

        return Ok(contents);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetContent(Guid id)
    {
        var content = await _manager.GetContent(id).ConfigureAwait(false);

        if (content == null)
            return NotFound();
        
        return Ok(content);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateContent(
        [FromBody] ContentInput content
        )
    {
        var createdContent = await _manager.CreateContent(content.ToDto()).ConfigureAwait(false);
        return createdContent == null ? Problem() : CreatedAtAction(nameof(GetContent), new { id = createdContent.Id }, createdContent);
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateContent(
        Guid id,
        [FromBody] ContentInput content
        )
    {
        var updatedContent = await _manager.UpdateContent(id, content.ToDto()).ConfigureAwait(false);

        return updatedContent == null ? NotFound() : Ok(updatedContent);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContent(
        Guid id
    )
    {
        var deletedId = await _manager.DeleteContent(id).ConfigureAwait(false);
        return Ok(deletedId);
    }
    
    [HttpPost("{id}/genre")]
    public async Task<IActionResult> AddGenres(
        Guid id,
        [FromBody] IEnumerable<string> genre
    )
    {
        var result = await _manager.AddGenres(id, genre);
        return result != null ? Ok(result) : NotFound();
    }
    
    [HttpDelete("{id}/genre")]
    public async Task<IActionResult> RemoveGenres(
        Guid id,
        [FromBody] IEnumerable<string> genre
    )
    {
        var result = await _manager.RemoveGenres(id, genre);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpGet("v2")] //corrigir a rota
    public async Task<IActionResult> GetManyContents(string Title, string[] Genres)
    {
        var contents = await _manager.GetManyContents(Title, Genres).ConfigureAwait(false);

        if (!contents.Any())
            return NotFound();

        return Ok(contents);
    }


}