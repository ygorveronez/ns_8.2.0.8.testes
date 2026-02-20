using System;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoRecebimento
    {
        Iniciada = 0,
        Finalizada = 1,
        Cancelada = 3
    }

    public static class SituacaoRecebimentoHelper
    {
        public static string ObterDescricao(this SituacaoRecebimento situacao)
        {
            switch (situacao)
            {
                case SituacaoRecebimento.Iniciada: return "Iniciada";
                case SituacaoRecebimento.Finalizada: return "Finalizada";
                case SituacaoRecebimento.Cancelada: return "Cancelada";
                default: return String.Empty;
            }
        }
    }
}
