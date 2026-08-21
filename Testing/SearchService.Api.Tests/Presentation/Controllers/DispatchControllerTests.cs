using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SearchService.Api.Core.Interfaces;
using SearchService.Api.Models;
using SearchService.Api.Models.Enums;
using SearchService.Api.Models.Search;
using SearchService.Api.Presentation.Controllers;

namespace SearchService.Api.Tests.Presentation.Controllers;

public class DispatchControllerTests
{
    private readonly Mock<IDispatchIndexService> _indexService = new();
    private readonly Mock<IDispatchSearchService> _searchService = new();
    private readonly Mock<IValidator<DispatchWriterEvent>> _dispatchEventValidator = new();
    private readonly Mock<IValidator<DispatchSearchRequestModel>> _searchValidator = new();
    private readonly Mock<ILogger<DispatchController>> _logger = new();
    private DispatchController CreateController() => new(
        _indexService.Object,
        _searchService.Object,
        _dispatchEventValidator.Object,
        _searchValidator.Object,
        _logger.Object);

    private static object GetValue(IActionResult result) => ((ObjectResult)result).Value!;

    private static DispatchWriterEvent CreateEvent(Guid? dispatchId = null) => new(
        EventType.Create,
        dispatchId ?? Guid.NewGuid(),
        100m,
        DateTime.UtcNow,
        DateTime.UtcNow.AddDays(1),
        DispatchStatus.Delivered,
        [new DispatchWriterVehicle("VIN1")]);

    [Fact]
    public async Task Post_ValidDispatch_ReturnsOkWithId()
    {
        var dispatchEvent = CreateEvent();
        _dispatchEventValidator
            .Setup(v => v.ValidateAsync(dispatchEvent, default))
            .ReturnsAsync(new ValidationResult());
        _indexService
            .Setup(s => s.IndexAsync(It.Is<DispatchModel>(d => d.DispatchId == dispatchEvent.DispatchId)))
            .ReturnsAsync(new DispatchIndexResult(true, dispatchEvent.DispatchId.ToString(), null));

        var result = await CreateController().Post(dispatchEvent);

        var ok = Assert.IsType<OkObjectResult>(result);
        var idProperty = ok.Value!.GetType().GetProperty("Id")!.GetValue(ok.Value);
        Assert.Equal(dispatchEvent.DispatchId.ToString(), idProperty);
    }

    [Fact]
    public async Task Post_InvalidDispatch_ReturnsBadRequestAndDoesNotIndex()
    {
        var dispatchEvent = CreateEvent();
        var failures = new List<ValidationFailure> { new("DispatchStatus", "must not be empty") };
        _dispatchEventValidator
            .Setup(v => v.ValidateAsync(dispatchEvent, default))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await CreateController().Post(dispatchEvent);

        Assert.IsType<BadRequestObjectResult>(result);
        _indexService.Verify(s => s.IndexAsync(It.IsAny<DispatchModel>()), Times.Never);
    }

    [Fact]
    public async Task Post_IndexServiceFails_ReturnsProblem()
    {
        var dispatchEvent = CreateEvent();
        _dispatchEventValidator
            .Setup(v => v.ValidateAsync(dispatchEvent, default))
            .ReturnsAsync(new ValidationResult());
        _indexService
            .Setup(s => s.IndexAsync(It.Is<DispatchModel>(d => d.DispatchId == dispatchEvent.DispatchId)))
            .ReturnsAsync(new DispatchIndexResult(false, null, "cluster unavailable"));

        var result = await CreateController().Post(dispatchEvent);

        var problem = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, problem.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingDispatch_ReturnsNoContent()
    {
        var dispatchId = Guid.NewGuid();
        _indexService
            .Setup(s => s.DeleteAsync(dispatchId))
            .ReturnsAsync(new DispatchDeleteResult(true, false, null));

        var result = await CreateController().Delete(dispatchId);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_DispatchDoesNotExist_ReturnsNotFound()
    {
        var dispatchId = Guid.NewGuid();
        _indexService
            .Setup(s => s.DeleteAsync(dispatchId))
            .ReturnsAsync(new DispatchDeleteResult(false, true, null));

        var result = await CreateController().Delete(dispatchId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ServiceFails_ReturnsProblem()
    {
        var dispatchId = Guid.NewGuid();
        _indexService
            .Setup(s => s.DeleteAsync(dispatchId))
            .ReturnsAsync(new DispatchDeleteResult(false, false, "cluster unavailable"));

        var result = await CreateController().Delete(dispatchId);

        var problem = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, problem.StatusCode);
    }

    [Fact]
    public async Task Search_ValidRequest_ReturnsOkWithResults()
    {
        var request = new DispatchSearchRequestModel { DispatchStatus = "Delivered" };
        var response = new[] { Guid.NewGuid() };

        _searchValidator
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());

        _searchService
            .Setup(s => s.SearchAsync(request))
            .ReturnsAsync(response);

        var result = await CreateController().Search(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task Search_InvalidRequest_ReturnsBadRequestAndDoesNotSearch()
    {
        var request = new DispatchSearchRequestModel { PriceTotalMin = 500, PriceTotalMax = 100 };
        var failures = new List<ValidationFailure> { new("PriceTotalMax", "must be greater than or equal to PriceTotalMin") };
        _searchValidator
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult(failures));

        var result = await CreateController().Search(request);

        Assert.IsType<BadRequestObjectResult>(result);
        _searchService.Verify(s => s.SearchAsync(It.IsAny<DispatchSearchRequestModel>()), Times.Never);
    }
}
