using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaApuracaoBonificacao
    {
        public int Ano { get; set; }
        public int Numero { get; set; }
        public List<int> CodigosRegraApuracao { get; set; }
        public Enumeradores.Mes? Mes { get; set; }
        public Enumeradores.Frete.SituacaoApuracaoBonificacao? Situacao { get; set; }
    }
}