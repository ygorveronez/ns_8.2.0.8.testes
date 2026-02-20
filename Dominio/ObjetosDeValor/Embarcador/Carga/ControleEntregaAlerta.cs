using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class ControleEntregaAlerta
    {
        public int CodigoAlerta { get; set; }
        public int Carga { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta TipoAlerta { get; set; }
        public string Descricao { get; set; }
        public string Imagem { get; set; }
        public string Data { get; set; }
        public string DataFim { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int CodigoEntrega { get; set; }
        public string Observacao { get; set; }
        public string ObservacaoMotorista { get; set; }
        public int Tratativa { get; set; }
        public string ValorAlerta { get; set; }
        public string ImagemStatus { get; set; }
        public AlertaMonitorStatus Status { get; set; }
        public bool ExibirNoControleEntrega { get; set; }
    }
}
