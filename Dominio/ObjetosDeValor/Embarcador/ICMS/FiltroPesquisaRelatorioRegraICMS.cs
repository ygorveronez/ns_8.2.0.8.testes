using System;

namespace Dominio.ObjetosDeValor.Embarcador.ICMS
{
    public sealed class FiltroPesquisaRelatorioRegraICMS
    {
        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa Ativo { get; set; }
    }
}
