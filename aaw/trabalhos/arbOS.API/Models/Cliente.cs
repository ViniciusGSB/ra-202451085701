using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ArbOS.API.Models
{
    public class Cliente
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Nome { get; set; } = string.Empty;
        public string Telemovel { get; set; } = string.Empty;
        public string Morada { get; set; } = string.Empty;
        public string TipoZona { get; set; } = string.Empty; // Residencial, Comercial, Publica
    }
}