using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaCargaJanelaCarregamento : FiltroPesquisaSituacaoJanelaCarregamento
    {
        public bool ApenasCargasNaoEmitidas { get; set; }

        public bool ApenasCargasPendentes { get; set; }

        public string NumeroBooking { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public List<int> CodigosCentroCarregamento { get; set; }

        public int CodigoDestino { get; set; }

        public int CodigoMotorista { get; set; }

        public int CodigoPaisDestino { get; set; }

        public int CodigoRota { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public int CodigoTransportador { get; set; }

        public int CodigoVeiculo { get; set; }

        public double CpfCnpjDestinatario { get; set; }

        public bool? EmReserva { get; set; }

        public bool? Excedente { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }

        public Entidades.Embarcador.Operacional.OperadorLogistica Operador { get; set; }

        public string Ordem { get; set; }

        public string PortoSaida { get; set; }

        public bool RetornarPreCargas { get; set; }

        public string TipoEmbarque { get; set; }

        public int CodigoModeloVeicularCarga { get; set; }

        public int CodigoOperador { get; set; }

        public List<int> CodigosFilial { get; set; }

        public List<int> CodigosFilialVenda { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public bool ExibirCargaQueNaoEstaoEmInicioViagem { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.RecomendacaoGR? RecomendacaoGR { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoCotacao> SituacaoCotacao { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacaoPesquisa> SituacaoLeilao { get; set; }

        public string UFOrigem { get; set; }

        public string UFDestino { get; set; }
    }
}
