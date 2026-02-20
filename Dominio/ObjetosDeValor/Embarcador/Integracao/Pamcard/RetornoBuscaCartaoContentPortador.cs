using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Pamcard
{
    public class RetornoBuscaCartaoContentPortador
    {
        [JsonProperty(PropertyName = "cpf", Required = Required.AllowNull)]
        public string Cpf { get; set; }

        [JsonProperty(PropertyName = "nome", Required = Required.AllowNull)]
        public string Nome { get; set; }

        [JsonProperty(PropertyName = "id", Required = Required.AllowNull)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "dataNascimento", Required = Required.AllowNull)]
        public DateTime DataNascimento { get; set; }

        [JsonProperty(PropertyName = "tipoDocumento", Required = Required.AllowNull)]
        public string TipoDocumento { get; set; }

        [JsonProperty(PropertyName = "dddTelefone", Required = Required.AllowNull)]
        public string DddTelefone { get; set; }

        [JsonProperty(PropertyName = "telefoneCelular", Required = Required.AllowNull)]
        public string TelefoneCelular { get; set; }

        [JsonProperty(PropertyName = "email", Required = Required.AllowNull)]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "nomeMae", Required = Required.AllowNull)]
        public string NomeMae { get; set; }

        [JsonProperty(PropertyName = "numeroRg", Required = Required.AllowNull)]
        public string NumeroRg { get; set; }

        [JsonProperty(PropertyName = "ufEmissorRg", Required = Required.AllowNull)]
        public string UfEmissorRg { get; set; }

        [JsonProperty(PropertyName = "usuarioSolicitante", Required = Required.AllowNull)]
        public string UsuarioSolicitante { get; set; }
    }
}
