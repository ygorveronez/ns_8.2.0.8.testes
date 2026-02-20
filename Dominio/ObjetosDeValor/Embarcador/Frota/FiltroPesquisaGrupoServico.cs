using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public class FiltroPesquisaGrupoServico
    {
        public string Descricao { get; set; }
        public int CodigoTipoOrdemServico { get; set; }
        public double CpfCnpjLocalManutencao { get; set; }
        public SituacaoAtivoPesquisa Status { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
    }
}
