using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.ControleEntrega
{
    public class Entrega
    {
        public int ProtocoloEntrega { get; set; }
        public int ProtocoloCarga { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public string CodigoFilialCarga { get; set; }
        public string DataInicioViagemCarga { get; set; }
        public string DataFimViagemCarga { get; set; }
        public Dominio.ObjetosDeValor.WebService.Entrega.EntregaDetalhes DetalhesEntrega { get; set; }
        public List<int> ProtocolosPedidos { get; set; }
        public List<Dominio.ObjetosDeValor.WebService.Entrega.PedidoDetalhes> DetalhesPedidos { get; set; }
        public List<Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaDetalhes> OcorrenciasEntrega { get; set; }
        public string DataRegistro { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint Ponto { get; set; }
    }
}
