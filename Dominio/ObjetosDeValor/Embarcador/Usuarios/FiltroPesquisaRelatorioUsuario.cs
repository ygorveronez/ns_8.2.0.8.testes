using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Usuarios
{
    public class FiltroPesquisaRelatorioUsuario
    {
        public DateTime DataCadastroInicial { get; set; }
        public DateTime DataCadastroFinal { get; set; }
        public DateTime UltimoAcessoInicial { get; set; }
        public DateTime UltimoAcessoFinal { get; set; }
        public Dominio.Enumeradores.TipoAcesso? Ambiente { get; set; }
        public SituacaoColaborador SituacaoColaborador { get; set; }
        public Aposentadoria Aposentadoria { get; set; }
        public int CodigoLocalidade { get; set; }
        public int CodigoPerfilAcesso { get; set; }
        public int CodigoEmpresa { get; set; }
        public bool? Operador { get; set; }
        public bool? AcessoSistema { get; set; }
        public bool UsuarioMultisoftware { get; set; }
        public string Status { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public bool SomenteUsuariosAtivo { get; set; }
    }
}
