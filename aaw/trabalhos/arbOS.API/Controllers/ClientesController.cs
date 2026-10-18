using ArbOS.API.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly IMongoCollection<Cliente> _clientesCollection;

    public ClientesController(IConfiguration configuration)
    {
        var mongoClient = new MongoClient(configuration["DatabaseSettings:ConnectionString"]);
        var mongoDatabase = mongoClient.GetDatabase(configuration["DatabaseSettings:DatabaseName"]);
        _clientesCollection = mongoDatabase.GetCollection<Cliente>(configuration["DatabaseSettings:CollectionName"]);
    }

    [HttpGet]
    public async Task<ActionResult<List<Cliente>>> GetAll()
    {
        var clientes = await _clientesCollection.Find(_ => true).ToListAsync();
        return Ok(clientes);
    }

    [HttpGet("{id:length(24)}", Name = "GetClienteById")]
    public async Task<ActionResult<Cliente>> GetById(string id)
    {
        var cliente = await _clientesCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        if (cliente == null) return NotFound(new { mensagem = "Cliente não encontrado." });
        return Ok(cliente);
    }

    [HttpPost]
    public async Task<ActionResult<Cliente>> Create(Cliente novoCliente)
    {
        await _clientesCollection.InsertOneAsync(novoCliente);
        return CreatedAtRoute("GetClienteById", new { id = novoCliente.Id }, novoCliente);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Cliente clienteAtualizado)
    {
        var cliente = await _clientesCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
        if (cliente == null) return NotFound(new { mensagem = "Cliente não encontrado." });

        clienteAtualizado.Id = cliente.Id;
        await _clientesCollection.ReplaceOneAsync(c => c.Id == id, clienteAtualizado);
        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var resultado = await _clientesCollection.DeleteOneAsync(c => c.Id == id);
        if (resultado.DeletedCount == 0) return NotFound(new { mensagem = "Cliente não encontrado." });
        return NoContent();
    }
}