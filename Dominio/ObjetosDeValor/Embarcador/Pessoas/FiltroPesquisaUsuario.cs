using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pessoas
{
    public class FiltroPesquisaUsuario
    {
        public FiltroPesquisaUsuario()
        {
            TipoUsuario = TipoUsuario.Todos;
            SituacaoColaborador = SituacaoColaborador.Todos;
        }

        public string Nome { get; set; }
        public string Status { get; set; }
        public SituacaoAtivoPesquisa Ativo { get; set; }
        public Dominio.Enumeradores.TipoAcesso? TipoAcesso { get; set; }
        public TipoComercial TipoComercial { get; set; }
        public int PerfilAcesso { get; set; }
        public string TipoPessoa { get; set; }
        public string CpfCnpj { get; set; }
        public string Usuario { get; set; }
        public string Tipo { get; set; }
        public bool UsuarioMultisoftware { get; set; }
        public bool SomenteFuncionarios { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public List<TipoCargoFuncionario> TipoCargoFuncionario { get; set; }
        public bool IgnorarSituacaoMotorista { get; set; }
        public SituacaoColaborador SituacaoColaborador { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoSetor { get; set; }
        public string NumeroMatricula { get; set; }
        public string CodigoIntegracao { get; set; }
        public bool OcultarUsuarioMultiCTe { get; set; }
        public int Localidade { get; set; }
    }
}
