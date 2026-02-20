using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaCentroCarregamento
    {
        public string Descricao { get; set; }
        public List<int> CodigosFilial { get; set; }
        public int CodigoTipoCarga { get; set; }
        public SituacaoAtivoPesquisa Ativo { get; set; }
        public int CodigoOperadorLogistica { get; set; }
        public bool SomenteCentrosOperadorLogistica { get; set; }
        public bool SomenteCentrosManobra { get; set; }
    }
}
