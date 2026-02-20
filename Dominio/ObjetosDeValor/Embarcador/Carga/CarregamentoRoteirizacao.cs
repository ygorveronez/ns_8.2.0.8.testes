using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class CarregamentoRoteirizacao
    {
        public int Protocolo { get; set; }

        public string NumeroCarregamento { get; set; }

        public string IdentificadorDeRota { get; set; }

        public Filial.Filial Filial { get; set; }

        public string CodigoIntegracaoTipoOperacao { get; set; }

        public string CodigoIntegracaoTipoCarga { get; set; }

        public string CodigoModeloVeicular { get; set; }

        public string Observacao { get; set; }

        public Frota.Veiculo Veiculo { get; set; }

        public Motorista Motorista { get; set; }

        public decimal DistanciaEmKm { get; set; }

        public List<CarregamentoRoteirizacaoPedido> Pedidos { get; set; }

        public Pessoas.Empresa Transportador { get; set; }

        public CarregamentoRoteirizacaoRotaFrete RotaFrete { get; set; }

        public string CodigoIntegracaoTipoCarregamento { get; set; }

        public DateTime? DataCarregamento { get; set; }

        public CarregamentoRoterizacaoValoresFrete ComponenteFrete { get; set; }
    }

    public sealed class CarregamentoRoteirizacaoPedido
    {
        public int Protocolo { get; set; }

        public DateTime? DataPrevisaoEntrega { get; set; }

        public DateTime? DataPrevisaoSaida { get; set; }

        public string CodigoIntegracaoCanalEntrega { get; set; }

        public string CodigoIntegracaoCanalVenda { get; set; }

        public int OrdemEntrega { get; set; }

        public string ObservacaoEntrega { get; set; }

        public Pessoas.Pessoa Expedidor { get; set; }

        public Pessoas.Pessoa Recebedor { get; set; }
    }

    public sealed class CarregamentoRoteirizacaoRotaFrete
    {
        public string Polilinha { get; set; }

        public int TempoViagemMinutos { get; set; }

        public Enumeradores.TipoUltimoPontoRoteirizacao? TipoUltimoPontoRoteirizacao { get; set; }
    }

    public sealed class CarregamentoRoterizacaoValoresFrete
    {
        public bool ValorFreteCalculado { get; set; }

        public Embarcador.Frete.FreteValor ValorFrete { get; set; }
    }
}

