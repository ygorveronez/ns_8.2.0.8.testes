namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class Funcionario
    {
        public string CPF { get; set; }
        public string CNPJ { get; set; }
        public string Login { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Nome { get; set; }
        public string RG { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComercial TipoComercial { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Usuarios.PerfilAcesso PerfilAcesso { get; set; }
    }
}
