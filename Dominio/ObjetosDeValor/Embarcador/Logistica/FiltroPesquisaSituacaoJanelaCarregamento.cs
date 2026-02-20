using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public abstract class FiltroPesquisaSituacaoJanelaCarregamento
    {
        public bool SituacaoFaturada { get; set; }

        public bool SituacaoNaoFaturada { get; set; }

        public List<Enumeradores.SituacaoCargaJanelaCarregamento> Situacoes { get; set; }
    }
}
