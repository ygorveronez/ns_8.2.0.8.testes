using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class Carregamento
    {
        public string NumeroCarregamento { get; set; }

        public string DataCarregamento { get; set; }

        public int ProtocoloIntegracaoCarga { get; set; }

        public int ProtocoloCarregamento { get; set; }

        public string ProtocoloPreCarga { get; set; }

        public Filial.Filial Filial { get; set; }

        public string CodigoIntegracaoFilial { get; set; }

        public string CodigoIntegracaoTipoOperacao { get; set; }

        public string CodigoIntegracaoTipoCarga { get; set; }

        public string CodigoModeloVeicular { get; set; }

        public decimal DistanciaEmKm { get; set; }

        public string Observacao { get; set; }

        public Frota.Veiculo Veiculo { get; set; }

        public Frota.Veiculo Reboque1 { get; set; }

        public Motorista Motorista { get; set; }

        public string TONSimulado { get; set; }

        public string NumeroCarga { get; set; }

        public List<int> ProtocolosPedidos { get; set; }

        public List<Embarcador.Pedido.BlocosPedidosCarregamento> BlocosPedidosCarregamento { get; set; }

        public Pessoas.Empresa TransportadoraEmitente { get; set; }

        public MontagemCarga.BlocoSimulacaoFreteCarregamento SimulacaoFrete { get; set; }

        public string CamposPersonalizados { get; set; }

        public List<Carregamento> CarregamentosRedespacho { get; set; }

        public int? CodigoTipoCarregamento { get; set; }

        public string DescricaoTipoCarregamento { get; set; }
    }
}
