using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAgendamentoColeta
    {
        AguardandoConfirmacao = 1,
        Agendado = 2,
        Finalizado = 3,
        Cancelado = 4,
        CanceladoEmbarcador = 5,
        NaoComparecimento = 6,
        CargaDevolvida = 7,
        NaoComparecimentoConfirmadoPeloFornecedor = 8,
        AguardandoGeracaoSenha = 9,
        AguardandoCTes = 10
    }

    public static class SituacaoAgendamentoColetaHelper
    {
        public static string ObterDescricao(this SituacaoAgendamentoColeta situacao)
        {
            switch (situacao)
            {
                case SituacaoAgendamentoColeta.AguardandoConfirmacao: return "Aguardando Confirmação";
                case SituacaoAgendamentoColeta.Agendado: return "Agendado";
                case SituacaoAgendamentoColeta.Finalizado: return "Finalizado";
                case SituacaoAgendamentoColeta.Cancelado: return "Cancelada pelo Fornecedor";
                case SituacaoAgendamentoColeta.CanceladoEmbarcador: return "Cancelada pelo Embarcador";
                case SituacaoAgendamentoColeta.NaoComparecimento: return "Não Comparecido";
                case SituacaoAgendamentoColeta.CargaDevolvida: return "Carga Devolvida";
                case SituacaoAgendamentoColeta.NaoComparecimentoConfirmadoPeloFornecedor: return "Cancelada pelo Fornecedor fora do prazo (suscetível a multa de No Show)";
                case SituacaoAgendamentoColeta.AguardandoGeracaoSenha: return "Aguardando Geração Senha";
                case SituacaoAgendamentoColeta.AguardandoCTes: return "Aguardando CT-es";
                default: return string.Empty;
            }
        }

        public static List<SituacaoAgendamentoColeta> Canceladas => new()
        {
            SituacaoAgendamentoColeta.Cancelado,
            SituacaoAgendamentoColeta.CanceladoEmbarcador,
            SituacaoAgendamentoColeta.NaoComparecimento,
            SituacaoAgendamentoColeta.CargaDevolvida,
            SituacaoAgendamentoColeta.NaoComparecimentoConfirmadoPeloFornecedor
        };
    }
}
