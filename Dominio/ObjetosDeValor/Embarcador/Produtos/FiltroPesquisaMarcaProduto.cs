using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class FiltroPesquisaMarcaProduto
    {
        public string Descricao { get; set; }
        public SituacaoAtivoPesquisa Status { get; set; }
        public int CodigoEmpresa { get; set; }
    }
}
