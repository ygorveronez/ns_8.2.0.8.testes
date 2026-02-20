using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Contabil
{
    public sealed class FiltroPesquisaDireitoFiscal
    {

        public int CodigoIVA { get; set; }

        public string Descricao { get; set; }

        public SituacaoAtivoPesquisa Situacao { get; set; }
    }
}
