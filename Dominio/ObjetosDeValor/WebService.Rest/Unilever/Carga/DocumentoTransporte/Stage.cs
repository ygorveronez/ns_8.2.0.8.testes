using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever
{
    public class Stage
    {
        public string NumeroStage { get; set; }
        public string DataEntregaPlaneja { get; set; }
        public string HoraEntregaPlaneja { get; set; }
        public decimal Distancia { get; set; }
        public Vazio TipoPercurso { get; set; }
        public Cliente Expedidor { get; set; }
        public Cliente Recebedor { get; set; }
        public ModeloVeicular ModeloVeicular { get; set; }
        public int NumeroVeiculo { get; set; }
        public bool RelevanciaCusto { get; set; }
        public string Agrupamento { get; set; }
        public int OrdemEntrega { get; set; }
        public Embarcador.Pedido.CanalEntrega CanalEntrega { get; set; }
        public Embarcador.Pedido.CanalVenda CanalVenda { get; set; }
        public bool NaoPossuiValePedagio { get; set; }
        public TipoModal TipoModal { get; set; }
        public Embarcador.Pessoas.Empresa TransportadoraEmitente { get; set; }
        public Dominio.ObjetosDeValor.Veiculo Veiculo { get; set; }
        public int StatusVPEmbarcador { get; set; }
    }
}
