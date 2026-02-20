using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Frotas
{
    public sealed class FiltroPesquisaRelatorioMotorista
    {
        public int CodigoMotorista { get; set; }
        public string CodigoIntegracao { get; set; }
        public int CodigoCentroResultado { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoTipoLicenca { get; set; }
        public int CodigoEmpresa { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public TipoMotorista TipoMotorista { get; set; }
        public SituacaoAtivoPesquisa Status { get; set; }
        public SituacaoColaborador SituacaoColaborador { get; set; }
        public Aposentadoria Aposentadoria { get; set; }
        public int CargoMotorista { get; set; }
        public bool? Bloqueado { get; set; }
        public bool? UsuarioMobile { get; set; }
        public bool? NaoBloquearAcessoSimultaneo { get; set; }
        public int CodigoGestor { get; set; }
    }
}
