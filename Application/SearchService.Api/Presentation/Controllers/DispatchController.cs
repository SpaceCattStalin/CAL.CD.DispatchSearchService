using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SearchService.Api.Core.Interfaces;
using SearchService.Api.Models;
using SearchService.Api.Models.Search;

namespace SearchService.Api.Presentation.Controllers;

[ApiController]
[Route("api/dispatch")]
public class DispatchController(
    IDispatchIndexService indexService,
    IDispatchSearchService searchService,
    IValidator<DispatchWriterEvent> dispatchEventValidator,
    IValidator<DispatchSearchRequestModel> searchValidator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] DispatchWriterEvent dispatchEvent)
    {
        var validation = await dispatchEventValidator.ValidateAsync(dispatchEvent);
        if (!validation.IsValid)
            return BadRequest(validation.Errors);

        var result = await indexService.IndexAsync(dispatchEvent.ToDispatchModel());
        return result.success
            ? Ok(new { result.Id })
            : Problem(result.Error);
    }

    [HttpDelete("{dispatchId:guid}")]
    public async Task<IActionResult> Delete(Guid dispatchId)
    {
        var result = await indexService.DeleteAsync(dispatchId);

        if (result.NotFound)
            return NotFound();

        return result.success ? NoContent() : Problem(result.Error);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] DispatchSearchRequestModel request)
    {
        var validation = await searchValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return BadRequest(validation.Errors);

        var response = await searchService.SearchAsync(request);
        return Ok(response);
    }
}
