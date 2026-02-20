using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoTratativaFluxoCompra
    {
        Todos = 0,
        Pendente = 1,
        Concluido = 2,
    }

    public static class TratativaFluxoCompraHelper
    {
        public static string ObterDescricao(this SituacaoTratativaFluxoCompra situacaoTratativa)
        {
            switch (situacaoTratativa)
            {
                case SituacaoTratativaFluxoCompra.Pendente: return "Pendente";
                case SituacaoTratativaFluxoCompra.Concluido: return "Concluído";
                default: return string.Empty;
            }
        }
    }
}
