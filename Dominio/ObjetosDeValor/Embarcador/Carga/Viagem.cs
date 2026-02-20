using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class Viagem
    {
        public int Codigo { get; set; }
        public int CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.DirecaoViagemMultimodal Direcao { get; set; }
        public Navio Navio { get; set; }
        public int NumeroViagem { get; set; }

        public TerminalPorto TerminalAtracacao { get; set; }
        public Porto PortoAtracacao { get; set; }
        public string DataPrevisaoChegadaNavio { get; set; }
        public string DataPrevisaoSaidaNavio { get; set; }
        public string DataDeadLine { get; set; }
        public bool ETAConfirmado { get; set; }
        public bool ETSConfirmado { get; set; }

        public List<Schedule> Schedules { get; set; }
        public bool InativarCadastro { get; set; }
        public bool Atualizar { get; set; }
    }
}
