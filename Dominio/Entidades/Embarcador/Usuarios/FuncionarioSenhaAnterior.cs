namespace Dominio.Entidades.Embarcador.Usuarios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FUNCIONARIO_SENHA_ANTERIOR", EntityName = "FuncionarioSenhaAnterior", Name = "Dominio.Entidades.Embarcador.Usuarios.FuncionarioSenhaAnterior", NameType = typeof(FuncionarioSenhaAnterior))]
    public class FuncionarioSenhaAnterior : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FSA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SenhaCriptografada", Column = "FUN_SENHA_CRIPTOGRAFADA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool SenhaCriptografada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PAC_SENHA", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Senha { get; set; }
    }
}
