using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class FiltroPesquisaLocalArmazenamentoProduto
    {
        public string Descricao { get; set; }
        public SituacaoAtivoPesquisa Status { get; set; }
        public int CodigoEmpresa { get; set; }
        public int TipoOleo { get; set; }
    }
}
