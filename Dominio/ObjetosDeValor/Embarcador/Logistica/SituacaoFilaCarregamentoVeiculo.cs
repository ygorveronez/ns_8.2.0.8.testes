using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFilaCarregamentoVeiculo
    {
        AguardandoConfirmacao = 1,
        AguardandoConjuntos = 2,
        CargaCancelada = 3,
        Disponivel = 4,
        EmTransicao = 5,
        EmViagem = 6,
        Finalizada = 7,
        Removida = 8,
        EmRemocao = 9,
        AguardandoAceiteCarga = 10,
        AceiteCargaRecusado = 11,
        EmChecklist = 12,
        AguardandoChegadaVeiculo = 13,
        AguardandoCarga = 14,
        ReboqueAtrelado = 15,
        AguardandoAceitePreCarga = 16
    }

    public static class SituacaoFilaCarregamentoVeiculoHelper
    {
        public static string ObterDescricao(this SituacaoFilaCarregamentoVeiculo situacao)
        {
            switch (situacao)
            {
                case SituacaoFilaCarregamentoVeiculo.AceiteCargaRecusado: return "Aceite da Carga Recusado";
                case SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga: return "Aguardando Aceite da Carga";
                case SituacaoFilaCarregamentoVeiculo.AguardandoAceitePreCarga: return "Aguardando Aceite do Pré Planejamento";
                case SituacaoFilaCarregamentoVeiculo.AguardandoCarga: return "Aguardando Carga";
                case SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo: return "Aguardando Chegada de Veículo";
                case SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao: return "Aguardando Confirmação";
                case SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos: return "Aguardando Conjuntos";
                case SituacaoFilaCarregamentoVeiculo.CargaCancelada: return "Carga Cancelada";
                case SituacaoFilaCarregamentoVeiculo.Disponivel: return "Disponível";
                case SituacaoFilaCarregamentoVeiculo.EmChecklist: return "Em Checklist";
                case SituacaoFilaCarregamentoVeiculo.EmRemocao: return "Em Remoção";
                case SituacaoFilaCarregamentoVeiculo.EmViagem: return "Em Viagem";
                case SituacaoFilaCarregamentoVeiculo.EmTransicao: return "Em Transição";
                case SituacaoFilaCarregamentoVeiculo.Finalizada: return "Finalizada";
                case SituacaoFilaCarregamentoVeiculo.ReboqueAtrelado: return "Reboque Atrelado";
                case SituacaoFilaCarregamentoVeiculo.Removida: return "Removida";
                default: return string.Empty;
            }
        }

        public static List<SituacaoFilaCarregamentoVeiculo> ObterSituacoesAtiva()
        {
            return new List<SituacaoFilaCarregamentoVeiculo>()
            {
                SituacaoFilaCarregamentoVeiculo.AceiteCargaRecusado,
                SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga,
                SituacaoFilaCarregamentoVeiculo.AguardandoAceitePreCarga,
                SituacaoFilaCarregamentoVeiculo.AguardandoCarga,
                SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo,
                SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao,
                SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos,
                SituacaoFilaCarregamentoVeiculo.CargaCancelada,
                SituacaoFilaCarregamentoVeiculo.Disponivel,
                SituacaoFilaCarregamentoVeiculo.EmChecklist,
                SituacaoFilaCarregamentoVeiculo.EmRemocao,
                SituacaoFilaCarregamentoVeiculo.EmTransicao,
                SituacaoFilaCarregamentoVeiculo.EmViagem,
                SituacaoFilaCarregamentoVeiculo.ReboqueAtrelado
            };
        }

        public static List<SituacaoFilaCarregamentoVeiculo> ObterSituacoesAtivaSemEmTrasicao()
        {
            return new List<SituacaoFilaCarregamentoVeiculo>()
            {
                SituacaoFilaCarregamentoVeiculo.AceiteCargaRecusado,
                SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga,
                SituacaoFilaCarregamentoVeiculo.AguardandoAceitePreCarga,
                SituacaoFilaCarregamentoVeiculo.AguardandoCarga,
                SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo,
                SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao,
                SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos,
                SituacaoFilaCarregamentoVeiculo.CargaCancelada,
                SituacaoFilaCarregamentoVeiculo.Disponivel,
                SituacaoFilaCarregamentoVeiculo.EmChecklist,
                SituacaoFilaCarregamentoVeiculo.EmRemocao,
                SituacaoFilaCarregamentoVeiculo.EmViagem,
                SituacaoFilaCarregamentoVeiculo.ReboqueAtrelado
            };
        }

        public static List<SituacaoFilaCarregamentoVeiculo> ObterSituacoesCargaAtiva()
        {
            return new List<SituacaoFilaCarregamentoVeiculo>()
            {
                SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga,
                SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao,
                SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos,
                SituacaoFilaCarregamentoVeiculo.EmViagem
            };
        }

        public static List<SituacaoFilaCarregamentoVeiculo> ObterSituacoesNaFila()
        {
            return new List<SituacaoFilaCarregamentoVeiculo>()
            {
                SituacaoFilaCarregamentoVeiculo.AceiteCargaRecusado,
                SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga,
                SituacaoFilaCarregamentoVeiculo.AguardandoAceitePreCarga,
                SituacaoFilaCarregamentoVeiculo.AguardandoCarga,
                SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo,
                SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao,
                SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos,
                SituacaoFilaCarregamentoVeiculo.CargaCancelada,
                SituacaoFilaCarregamentoVeiculo.Disponivel,
                SituacaoFilaCarregamentoVeiculo.EmChecklist,
                SituacaoFilaCarregamentoVeiculo.EmRemocao,
                SituacaoFilaCarregamentoVeiculo.ReboqueAtrelado
            };
        }

        public static List<SituacaoFilaCarregamentoVeiculo> ObterSituacoesPreCargaAtiva()
        {
            return new List<SituacaoFilaCarregamentoVeiculo>()
            {
                SituacaoFilaCarregamentoVeiculo.AguardandoAceitePreCarga,
                SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos,
                SituacaoFilaCarregamentoVeiculo.AguardandoCarga,
                SituacaoFilaCarregamentoVeiculo.EmViagem
            };
        }

        public static List<SituacaoFilaCarregamentoVeiculo> ObterSituacoesVincularFilaCarregamentoMotorista()
        {
            return new List<SituacaoFilaCarregamentoVeiculo>() {
                SituacaoFilaCarregamentoVeiculo.AceiteCargaRecusado,
                SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga,
                SituacaoFilaCarregamentoVeiculo.AguardandoAceitePreCarga,
                SituacaoFilaCarregamentoVeiculo.AguardandoCarga,
                SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo,
                SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos,
                SituacaoFilaCarregamentoVeiculo.CargaCancelada,
                SituacaoFilaCarregamentoVeiculo.Disponivel,
                SituacaoFilaCarregamentoVeiculo.EmChecklist,
                SituacaoFilaCarregamentoVeiculo.EmTransicao
            };
        }

        public static List<TipoFilaCarregamentoAlteracao> ObterTiposFilaCarregamentoAlteracao(this SituacaoFilaCarregamentoVeiculo situacao)
        {
            List<TipoFilaCarregamentoAlteracao> tipos = new List<TipoFilaCarregamentoAlteracao>();

            switch (situacao)
            {
                case SituacaoFilaCarregamentoVeiculo.AceiteCargaRecusado:
                case SituacaoFilaCarregamentoVeiculo.AguardandoAceiteCarga:
                case SituacaoFilaCarregamentoVeiculo.AguardandoAceitePreCarga:
                case SituacaoFilaCarregamentoVeiculo.AguardandoCarga:
                case SituacaoFilaCarregamentoVeiculo.AguardandoChegadaVeiculo:
                case SituacaoFilaCarregamentoVeiculo.AguardandoConfirmacao:
                case SituacaoFilaCarregamentoVeiculo.AguardandoConjuntos:
                case SituacaoFilaCarregamentoVeiculo.CargaCancelada:
                case SituacaoFilaCarregamentoVeiculo.EmChecklist:
                case SituacaoFilaCarregamentoVeiculo.EmRemocao:
                    tipos.Add(TipoFilaCarregamentoAlteracao.FilaCarregamentoVeiculo);
                    break;

                case SituacaoFilaCarregamentoVeiculo.EmTransicao:
                    tipos.Add(TipoFilaCarregamentoAlteracao.FilaCarregamentoVeiculoEmTransicao);
                    break;

                case SituacaoFilaCarregamentoVeiculo.Disponivel:
                case SituacaoFilaCarregamentoVeiculo.ReboqueAtrelado:
                    tipos.Add(TipoFilaCarregamentoAlteracao.FilaCarregamentoVeiculo);
                    tipos.Add(TipoFilaCarregamentoAlteracao.FilaCarregamentoMotorista);
                    break;
            }

            return tipos;
        }
    }
}
