using AdminMultisoftware.Dominio.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FiltroPesquisaCategoria
    {
        public string Descricao { get; set; }
        public string CodigoIntegracao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa Situacao { get; set; }
    }
}
