using Microsoft.AspNetCore.Mvc;
using PerfDemo.Models;
using PerfDemo.Services;
using Prometheus;

namespace PerfDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IDataService _svc;
    private static readonly Counter ApiReq = Metrics.CreateCounter("perf_demo_api_requests_total", "api requests", "method","endpoint");

    public ProductsController(IDataService svc) => _svc = svc;

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        ApiReq.WithLabels("GET","/api/products/{id}").Inc();
        var p = await _svc.GetProductAsync(id);
        if (p==null) return NotFound();
        return Ok(p);
    }

    [HttpGet]
    public async Task<IActionResult> List(int page=1,int pageSize=20)
    {
        ApiReq.WithLabels("GET","/api/products").Inc();
        var list = await _svc.GetProductsPageAsync(page,pageSize);
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductDto dto)
    {
        ApiReq.WithLabels("POST","/api/products").Inc();
        var created = await _svc.CreateProductAsync(dto.Name,dto.Payload);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductDto dto)
    {
        ApiReq.WithLabels("PUT","/api/products/{id}").Inc();
        var ok = await _svc.UpdateProductAsync(id,dto.Name,dto.Payload);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        ApiReq.WithLabels("DELETE","/api/products/{id}").Inc();
        var ok = await _svc.DeleteProductAsync(id);
        if (ok) return NotFound();
        return NoContent();
    }
}

public record ProductDto(string Name,string Payload);
