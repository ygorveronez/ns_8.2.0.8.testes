using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum GatilhoAutomatizacaoNaoComparecimento
    {
        CargaSemVeiculoInformado = 1,
        CargaNaoAgendada = 2,
        VeiculoSemRegistroChegada = 3
    }

    public static class GatilhoAutomatizacaoNaoComparecimentoHelper
    {
        public static string ObterDescricao(this GatilhoAutomatizacaoNaoComparecimento gatilho)
        {
            switch (gatilho)
            {
                case GatilhoAutomatizacaoNaoComparecimento.CargaNaoAgendada: return "Carga não Agendada";
                case GatilhoAutomatizacaoNaoComparecimento.CargaSemVeiculoInformado: return "Carga sem Veículo Informado";
                case GatilhoAutomatizacaoNaoComparecimento.VeiculoSemRegistroChegada: return "Veículo sem Registro de Chegada";
                default: return string.Empty;
            }
        }

        public static TipoMensagemAlerta ObterTipoMensagemAlerta(this GatilhoAutomatizacaoNaoComparecimento gatilho)
        {
            switch (gatilho)
            {
                case GatilhoAutomatizacaoNaoComparecimento.CargaNaoAgendada: return TipoMensagemAlerta.CargaNaoAgendada;
                case GatilhoAutomatizacaoNaoComparecimento.VeiculoSemRegistroChegada: return TipoMensagemAlerta.VeiculoSemRegistroChegada;
                default: return TipoMensagemAlerta.CargaSemVeiculoInformado;
            }
        }

        public static List<TipoMensagemAlerta> ObterTiposMensagemAlerta()
        {
            return new List<TipoMensagemAlerta>() {
                TipoMensagemAlerta.CargaNaoAgendada,
                TipoMensagemAlerta.CargaSemVeiculoInformado,
                TipoMensagemAlerta.VeiculoSemRegistroChegada
            };
        }
    }
}
