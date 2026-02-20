using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public class FiltroPesquisaJanelaCarregamento : FiltroPesquisaSituacaoJanelaCarregamento
    {
        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoCentroCarregamento { get; set; }

        public int CodigoModeloVeicularCarga { get; set; }

        public int CodigoOperador { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int CodigoTransportador { get; set; }

        public int CodigoVeiculo { get; set; }

        public double CpfCnpjDestinatario { get; set; }

        public DateTime? DataCarregamento { get; set; }

        public bool ExibirCargaQueNaoEstaoEmInicioViagem { get; set; }

        public string NumeroExp { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }

        public TimeSpan? PeriodoFinal { get; set; }

        public TimeSpan? PeriodoInicial { get; set; }

        public Enumeradores.TipoJanelaCarregamento TipoJanelaCarregamento { get; set; }

        public List<Enumeradores.SituacaoCargaJanelaCarregamento> SituacaoCargaJanelaCarregamento { get; set; }

        public List<Enumeradores.SituacaoCargaJanelaCarregamentoCotacao> SituacaoCotacao { get; set; }

        public List<Enumeradores.SituacaoCotacaoPesquisa> SituacaoLeilao { get; set; }

        public TimeSpan? InicioPeriodoInicial { get; set; }

        public TimeSpan? InicioPeriodoFinal { get; set; }

        public Enumeradores.RecomendacaoGR? RecomendacaoGR { get; set; }

        public string UFOrigem { get; set; }

        public string UFDestino { get; set; }
        public int CodigoDestino { get; set; }
    }
}
