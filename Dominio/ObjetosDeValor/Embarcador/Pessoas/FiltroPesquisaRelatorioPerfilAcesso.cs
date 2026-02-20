namespace Dominio.ObjetosDeValor.Embarcador.Pessoas
{
    public class FiltroPesquisaRelatorioPerfilAcesso
    {
        public int CodigoPerfil { get; set; }
        public bool? Ativo { get; set; }
        public int Cliente { get; set; }
        public int ClienteAcesso { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
        public string AdminStringConexao { get; set; }
    }
}
