namespace Dominio.ObjetosDeValor.Embarcador.Usuarios
{
    public class Usuario
    {
        public int Protocolo { get; set; }
        public string CodigoIntegracao { get; set; }
        public string DataCriacao { get; set; }
        public string DataUltimaAlteracao { get; set; }
        public string Setor { get; set; }
        public string DataAdmissao { get; set; }
        public string DataDemissao { get; set; }
        public bool UsuarioAdministrador { get; set; }
        public string CPF_CNPJ { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string Email { get; set; }     
        public int IBGEMunicipio { get; set; }
        public string Situacao { get; set; }
        public Perfil Perfil { get; set; }
        public PerfilAcesso PerfilAcesso { get; set; }
    }
}
