namespace Dominio.Entidades.Embarcador.Usuarios
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FUNCIONARIO_ANEXO", EntityName = "FuncionarioAnexo", Name = "Dominio.Entidades.Embarcador.Usuarios.FuncionarioAnexo", NameType = typeof(FuncionarioAnexo))]
    public class FuncionarioAnexo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FEA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "FEA_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "FEA_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GuidArquivo", Column = "FEA_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ImprimeNaFichaMotorista", Column = "FEA_IMPRIME_NA_FICHA_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ImprimeNaFichaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAnexoMotorista", Column = "FEA_TIPO_ANEXO_MOTORISTA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoAnexoMotorista), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoAnexoMotorista TipoAnexoMotorista { get; set; }
    }
}
