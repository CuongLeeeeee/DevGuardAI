using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/testcase")]
public class TestCaseController : ControllerBase
{
    private readonly IGeminiService _service;

    public TestCaseController(IGeminiService service)
    {
        _service = service;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(ContentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest("Code is required");

        var result = await _service.GenerateTestCases(request.Content);

        return Ok(result);
    }
}