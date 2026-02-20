using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFilaCarregamentoMotorista
    {
        CargaAceita = 1,
        CargaAlocada = 2,
        CargaCancelada = 3,
        CargaRecusada = 4,
        Disponivel = 5,
        SenhaPerdida = 6,
        Finalizada = 7,
        Removido = 8,
        PreCargaAlocada = 9,
        ReboqueAtrelado = 10
    }

    public static class SituacaoFilaCarregamentoMotoristaHelper
    {
        public static string ObterDescricao(this SituacaoFilaCarregamentoMotorista situacao)
        {
            switch (situacao)
            {
                case SituacaoFilaCarregamentoMotorista.CargaAceita: return "Carga Aceita";
                case SituacaoFilaCarregamentoMotorista.CargaAlocada: return "Carga Alocada";
                case SituacaoFilaCarregamentoMotorista.CargaCancelada: return "Carga Cancelada";
                case SituacaoFilaCarregamentoMotorista.CargaRecusada: return "Carga Recusada";
                case SituacaoFilaCarregamentoMotorista.Disponivel: return "Disponível";
                case SituacaoFilaCarregamentoMotorista.Finalizada: return "Finalizada";
                case SituacaoFilaCarregamentoMotorista.PreCargaAlocada: return "Pré Planejamento Alocado";
                case SituacaoFilaCarregamentoMotorista.ReboqueAtrelado: return "Reboque Atrelado";
                case SituacaoFilaCarregamentoMotorista.Removido: return "Removido";
                case SituacaoFilaCarregamentoMotorista.SenhaPerdida: return "Senha Perdida";
                default: return string.Empty;
            }
        }

        public static List<SituacaoFilaCarregamentoMotorista> ObterSituacoesAtiva()
        {
            return new List<SituacaoFilaCarregamentoMotorista>()
            {
                SituacaoFilaCarregamentoMotorista.CargaAceita,
                SituacaoFilaCarregamentoMotorista.CargaAlocada,
                SituacaoFilaCarregamentoMotorista.CargaCancelada,
                SituacaoFilaCarregamentoMotorista.CargaRecusada,
                SituacaoFilaCarregamentoMotorista.Disponivel,
                SituacaoFilaCarregamentoMotorista.PreCargaAlocada,
                SituacaoFilaCarregamentoMotorista.ReboqueAtrelado,
                SituacaoFilaCarregamentoMotorista.SenhaPerdida
            };
        }

        public static List<SituacaoFilaCarregamentoMotorista> ObterSituacoesBloqueadas()
        {
            return new List<SituacaoFilaCarregamentoMotorista>()
            {
                SituacaoFilaCarregamentoMotorista.CargaCancelada,
                SituacaoFilaCarregamentoMotorista.CargaRecusada,
                SituacaoFilaCarregamentoMotorista.SenhaPerdida
            };
        }
    }
}
