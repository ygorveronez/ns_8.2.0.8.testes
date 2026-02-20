using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco
{
    public class ConsultaMotoristaResponse
    {
        public string retornoWs { get; set; }
        public string mensagemRetorno { get; set; }
        public string cpf { get; set; }
        public string nomeMotorista { get; set; }
        public string tipoMotorista { get; set; }
        public string consulta { get; set; }
        public string cnpjLocalizado { get; set; }
        public string transpLocalizado { get; set; }
        public string pracaLocalizado { get; set; }
        public string ufPracaLocalizado { get; set; }
        public string cnpjEmbarcador { get; set; }
        public string nomeEmbarcador { get; set; }
        public string dataConsulta { get; set; }
        public string horaConsulta { get; set; }
        public string placaVeiculo { get; set; }
        public string placaCarreta { get; set; }
        public string protocolo { get; set; }
        public string consultaMensagem { get; set; }
        public string categoriaResultado { get; set; }
        public string dataPrazo { get; set; }
        public string resultado { get; set; }
        public List<GrupoEconomico> grupoEconomicos { get; set; }
        public List<RestricaoMotorista> restricaoMotoristas { get; set; }
    }
}
