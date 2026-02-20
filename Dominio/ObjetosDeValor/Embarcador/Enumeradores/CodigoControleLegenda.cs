using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum CodigoControleLegenda
    {
        JanelaCarregamento_AguardandoConfirmacaoTransportador = 1,
        JanelaCarregamento_FOB = 2,
        JanelaDescarregamento_AguardandoConfirmacaoAgendamento = 3,
        FilaCarregamento_AguardandoAceite = 4,
        FilaCarregamento_AguardandoDesatrelar = 5,
        FilaCarregamento_EmChecklist = 6,
        FilaCarregamento_EmRemocao = 7,
        FilaCarregamento_EmReversa = 8,
        FilaCarregamento_PerdeuSenha = 9,
        FilaCarregamento_AguardandoConfirmacao = 10,
        FilaCarregamento_Vazio = 11,
        FilaCarregamento_AguardandoCarga = 12,
        FilaCarregamento_CargaCancelada = 13,
        FilaCarregamento_CargaRecusada = 14,
        FilaCarregamento_EmViagem = 15,
        JanelaCarregamento_AguardandoEncosta = 16,
        JanelaCarregamento_AguardandoLiberacaoTransportadores = 17,
        JanelaCarregamento_Faturada = 18,
        JanelaCarregamento_ProntaCarregamento = 19,
        JanelaCarregamento_SemTransportador = 20,
        JanelaCarregamento_SemValorFrete = 21
    }

    public static class CodigoControleLegendaHelper
    {
        public static List<CodigoControleLegenda> ObterCodigosControlePorGrupo(GrupoCodigoControleLegenda grupo)
        {
            switch (grupo)
            {
                case GrupoCodigoControleLegenda.JanelaCarregamento:
                    return new List<CodigoControleLegenda>()
                    {
                        CodigoControleLegenda.JanelaCarregamento_AguardandoConfirmacaoTransportador,
                        CodigoControleLegenda.JanelaCarregamento_AguardandoEncosta,
                        CodigoControleLegenda.JanelaCarregamento_AguardandoLiberacaoTransportadores,
                        CodigoControleLegenda.JanelaCarregamento_Faturada,
                        CodigoControleLegenda.JanelaCarregamento_FOB,
                        CodigoControleLegenda.JanelaCarregamento_ProntaCarregamento,
                        CodigoControleLegenda.JanelaCarregamento_SemTransportador,
                        CodigoControleLegenda.JanelaCarregamento_SemValorFrete
                    };

                case GrupoCodigoControleLegenda.JanelaDescarregamento:
                    return new List<CodigoControleLegenda>()
                    {
                        CodigoControleLegenda.JanelaDescarregamento_AguardandoConfirmacaoAgendamento
                    };

                case GrupoCodigoControleLegenda.FilaCarregamento:
                    return new List<CodigoControleLegenda>()
                    {
                        CodigoControleLegenda.FilaCarregamento_AguardandoAceite,
                        CodigoControleLegenda.FilaCarregamento_AguardandoDesatrelar,
                        CodigoControleLegenda.FilaCarregamento_EmChecklist,
                        CodigoControleLegenda.FilaCarregamento_EmRemocao,
                        CodigoControleLegenda.FilaCarregamento_EmReversa,
                        CodigoControleLegenda.FilaCarregamento_EmViagem,
                        CodigoControleLegenda.FilaCarregamento_PerdeuSenha,
                        CodigoControleLegenda.FilaCarregamento_AguardandoConfirmacao,
                        CodigoControleLegenda.FilaCarregamento_Vazio,
                        CodigoControleLegenda.FilaCarregamento_AguardandoCarga,
                        CodigoControleLegenda.FilaCarregamento_CargaCancelada,
                        CodigoControleLegenda.FilaCarregamento_CargaRecusada
                    };

                default:
                    return new List<CodigoControleLegenda>();
            }
        }

        public static string ObterNomePropriedade(this CodigoControleLegenda codigoControle)
        {
            switch (codigoControle)
            {
                case CodigoControleLegenda.JanelaCarregamento_AguardandoConfirmacaoTransportador: return "AguardandoConfirmacaoTransportador";
                case CodigoControleLegenda.JanelaCarregamento_AguardandoEncosta: return "AguardandoEncosta";
                case CodigoControleLegenda.JanelaCarregamento_AguardandoLiberacaoTransportadores: return "AguardandoLiberacaoTransportadores";
                case CodigoControleLegenda.JanelaCarregamento_Faturada: return "Faturada";
                case CodigoControleLegenda.JanelaCarregamento_FOB: return "FOB";
                case CodigoControleLegenda.JanelaCarregamento_ProntaCarregamento: return "ProntaCarregamento";
                case CodigoControleLegenda.JanelaCarregamento_SemTransportador: return "SemTransportador";
                case CodigoControleLegenda.JanelaCarregamento_SemValorFrete: return "SemValorFrete";
                case CodigoControleLegenda.JanelaDescarregamento_AguardandoConfirmacaoAgendamento: return "AguardandoConfirmacaoAgendamento";
                case CodigoControleLegenda.FilaCarregamento_AguardandoAceite: return "AguardandoAceite";
                case CodigoControleLegenda.FilaCarregamento_AguardandoDesatrelar: return "AguardandoDesatrelar";
                case CodigoControleLegenda.FilaCarregamento_EmChecklist: return "EmChecklist";
                case CodigoControleLegenda.FilaCarregamento_EmRemocao: return "EmRemocao";
                case CodigoControleLegenda.FilaCarregamento_EmReversa: return "EmReversa";
                case CodigoControleLegenda.FilaCarregamento_EmViagem: return "EmViagem";
                case CodigoControleLegenda.FilaCarregamento_PerdeuSenha: return "PerdeuSenha";
                case CodigoControleLegenda.FilaCarregamento_AguardandoConfirmacao: return "AguardandoConfirmacao";
                case CodigoControleLegenda.FilaCarregamento_Vazio: return "Vazio";
                case CodigoControleLegenda.FilaCarregamento_AguardandoCarga: return "AguardandoCarga";
                case CodigoControleLegenda.FilaCarregamento_CargaCancelada: return "CargaCancelada";
                case CodigoControleLegenda.FilaCarregamento_CargaRecusada: return "CargaRecusada";
                default: return string.Empty;
            }
        }
    }
}
