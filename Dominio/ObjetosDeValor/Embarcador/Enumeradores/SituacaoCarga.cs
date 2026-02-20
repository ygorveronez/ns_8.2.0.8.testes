using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCarga
    {
        NaLogistica = 0,
        Nova = 1,
        CalculoFrete = 2,
        //EmLeilao = 3,
        AgTransportador = 4,
        AgNFe = 5,
        PendeciaDocumentos = 6,
        AgImpressaoDocumentos = 7,
        ProntoTransporte = 8,
        EmTransporte = 9,
        LiberadoPagamento = 10,
        Encerrada = 11,
        //EmCancelamento = 12,
        Cancelada = 13,
        //RejeicaoCancelamento = 14,
        AgIntegracao = 15,
        EmTransbordo = 17,
        Anulada = 18,
        Todas = 99,
        PermiteCTeManual = 101
    }

    public static class SituacaoCargaHelper
    {
        public static bool IsSituacaoCargaEmitida(this SituacaoCarga situacaoCarga)
        {
            return !ObterSituacoesCargaNaoEmitida().Contains(situacaoCarga);
        }

        public static bool IsSituacaoCargaFaturada(this SituacaoCarga situacaoCarga)
        {
            return !ObterSituacoesCargaNaoFaturada().Contains(situacaoCarga);
        }

        public static bool IsSituacaoCargaNaoEmitida(this SituacaoCarga situacaoCarga)
        {
            return ObterSituacoesCargaNaoEmitida().Contains(situacaoCarga);
        }

        public static bool IsSituacaoCargaSemDocumentacao(this SituacaoCarga situacaoCarga)
        {
            return ObterSituacoesCargaSemDocumentacao().Contains(situacaoCarga);
        }

        public static bool IsSituacaoCargaNaoFaturada(this SituacaoCarga situacaoCarga)
        {
            return ObterSituacoesCargaNaoFaturada().Contains(situacaoCarga);
        }

        public static bool IsSituacaoCargaCancelada(this SituacaoCarga situacaoCarga)
        {
            return ObterSituacoesCargaCancelada().Contains(situacaoCarga);
        }

        public static string ObterDescricao(this SituacaoCarga situacaoCarga)
        {
            switch (situacaoCarga)
            {
                case SituacaoCarga.Nova: return Localization.Resources.Enumeradores.SituacoesCarga.NovaCarga;
                case SituacaoCarga.CalculoFrete: return Localization.Resources.Enumeradores.SituacoesCarga.CalculoDeFrete;
                case SituacaoCarga.AgTransportador: return Localization.Resources.Enumeradores.SituacoesCarga.AguardandoTransportador;
                case SituacaoCarga.AgNFe: return Localization.Resources.Enumeradores.SituacoesCarga.AguardandoNotasFiscais;
                case SituacaoCarga.PendeciaDocumentos: return Localization.Resources.Enumeradores.SituacoesCarga.PendenciasNaEmissao;
                case SituacaoCarga.ProntoTransporte: return Localization.Resources.Enumeradores.SituacoesCarga.ProntoParaTransporte;
                case SituacaoCarga.EmTransporte: return Localization.Resources.Enumeradores.SituacoesCarga.EmTransporte;
                case SituacaoCarga.AgImpressaoDocumentos: return Localization.Resources.Enumeradores.SituacoesCarga.AguardandoImpressaoDosDocumentos;
                case SituacaoCarga.Encerrada: return Localization.Resources.Enumeradores.SituacoesCarga.Finalizada;
                case SituacaoCarga.Cancelada: return Localization.Resources.Enumeradores.SituacoesCarga.Cancelada;
                case SituacaoCarga.LiberadoPagamento: return Localization.Resources.Enumeradores.SituacoesCarga.PagamentoLiberado;
                case SituacaoCarga.AgIntegracao: return Localization.Resources.Enumeradores.SituacoesCarga.AguardandoIntegracao;
                case SituacaoCarga.EmTransbordo: return Localization.Resources.Enumeradores.SituacoesCarga.EmTransbordo;
                case SituacaoCarga.Anulada: return Localization.Resources.Enumeradores.SituacoesCarga.Anulada;
                default: return "";
            }
        }

        public static string ObterDescricaoIntegracaoApisul(this SituacaoCarga situacaoCarga)
        {
            switch (situacaoCarga)
            {
                case SituacaoCarga.Nova: return Localization.Resources.Enumeradores.SituacoesCarga.DadosDaCarga;
                case SituacaoCarga.AgIntegracao: return Localization.Resources.Enumeradores.SituacoesCarga.Integracao;
                case SituacaoCarga.Todas: return Localization.Resources.Enumeradores.SituacoesCarga.Todas;
                default: return "";
            }
        }

        public static List<SituacaoCarga> ObterSituacoesCargaNaoEmitida()
        {
            return new List<SituacaoCarga>()
            {
                SituacaoCarga.AgNFe,
                SituacaoCarga.AgTransportador,
                SituacaoCarga.Anulada,
                SituacaoCarga.CalculoFrete,
                SituacaoCarga.Cancelada,
                SituacaoCarga.Nova
            };
        }

        public static List<SituacaoCarga> ObterSituacoesCargaNaoFaturada()
        {
            return new List<SituacaoCarga>()
            {
                SituacaoCarga.AgNFe,
                SituacaoCarga.AgTransportador,
                SituacaoCarga.Anulada,
                SituacaoCarga.CalculoFrete,
                SituacaoCarga.Cancelada,
                SituacaoCarga.Nova,
                SituacaoCarga.PendeciaDocumentos
            };
        }

        public static List<SituacaoCarga> ObterSituacoesCargaSemDocumentacao()
        {
            return new List<SituacaoCarga>()
            {
                SituacaoCarga.AgNFe,
                SituacaoCarga.AgTransportador,
                SituacaoCarga.CalculoFrete,
                SituacaoCarga.Nova,
                SituacaoCarga.NaLogistica,
                SituacaoCarga.PendeciaDocumentos
            };
        }

        public static List<SituacaoCarga> ObterSituacoesCargaPermiteAtualizar()
        {
            return new List<SituacaoCarga>()
            {
                SituacaoCarga.AgNFe,
                SituacaoCarga.CalculoFrete,
                SituacaoCarga.Nova
            };
        }

        public static List<SituacaoCarga> ObterSituacoesCargaCancelada()
        {
            return new List<SituacaoCarga>()
            {
                SituacaoCarga.Cancelada,
                SituacaoCarga.Anulada
            };
        }

        public static string ObterCorMonitoramento(this SituacaoCarga situacaoCarga)
        {
            switch (situacaoCarga)
            {
                case SituacaoCarga.Nova: return "#64EDED";
                case SituacaoCarga.CalculoFrete: return "#64CBED";
                case SituacaoCarga.AgTransportador: return "#64A8ED";
                case SituacaoCarga.AgNFe: return "#6495ED";
                case SituacaoCarga.PendeciaDocumentos: return "#ED8664";
                case SituacaoCarga.ProntoTransporte: return "#CBED64";
                case SituacaoCarga.EmTransporte: return "#ED64ED";
                case SituacaoCarga.AgImpressaoDocumentos: return "#64EDA8";
                case SituacaoCarga.Encerrada: return "#64ED64";
                case SituacaoCarga.Cancelada: return "#ED6464";
                case SituacaoCarga.LiberadoPagamento: return "#64EDCB";
                case SituacaoCarga.AgIntegracao: return "#CB64ED";
                case SituacaoCarga.EmTransbordo: return "#ED6464";
                case SituacaoCarga.Anulada: return "#EDA864";
                default: return "#F9E2D2";
            }
        }
    }
}