using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public class FiltroPesquisaRegrasMultaAtrasoRetirada
    {
        public string Descricao { get; set; }
        public SituacaoAtivoPesquisa Ativo { get; set; }
        public int CodigoFilial { get; set; }
        public int CodigoTipoOcorrencia { get; set; }
    }
}
