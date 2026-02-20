namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum BotoesDetalheAcompanhamentoCarga
    {
        AdicionarPedidoReentrega = 1,
        DadosDaCarga = 2,
        VisualizarNoMapa = 3,
        BoletimDeEmbarque = 4,
        AdicionarEventos = 5,
        RaioXDaCarga = 6,
        Assumir = 7,
        Anotacoes = 8,
        OcorrenciasDeFrete = 9,
        HistoricoMonitoramento = 10,
        DetalhesPedidos = 11,
    }

    public static class BotoesDetalheAcompanhamentoCargaHelper
    {
        public static string ObterDescricao(this BotoesDetalheAcompanhamentoCarga status)
        {
            switch (status)
            {
                case BotoesDetalheAcompanhamentoCarga.AdicionarPedidoReentrega: return "Adicionar Pedido Reentrega";
                case BotoesDetalheAcompanhamentoCarga.DadosDaCarga: return "Dados da Carga";
                case BotoesDetalheAcompanhamentoCarga.VisualizarNoMapa: return "Visualizar no Mapa";
                case BotoesDetalheAcompanhamentoCarga.BoletimDeEmbarque: return "Boletim de Embarque";
                case BotoesDetalheAcompanhamentoCarga.AdicionarEventos: return "Adicionar Eventos";
                case BotoesDetalheAcompanhamentoCarga.RaioXDaCarga: return "Raio X da Carga";
                case BotoesDetalheAcompanhamentoCarga.Assumir: return "Assumir";
                case BotoesDetalheAcompanhamentoCarga.Anotacoes: return "Anotações";
                case BotoesDetalheAcompanhamentoCarga.OcorrenciasDeFrete: return "Ocorrência de Frete";
                case BotoesDetalheAcompanhamentoCarga.HistoricoMonitoramento: return "Histórico Monitoramento";
                case BotoesDetalheAcompanhamentoCarga.DetalhesPedidos: return "Detalhes dos Pedidos";

                default: return string.Empty;
            }
        }
    }
}
