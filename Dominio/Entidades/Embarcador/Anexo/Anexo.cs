namespace Dominio.Entidades.Embarcador.Anexo
{
    public abstract class Anexo<TEntidadeAnexo> : EntidadeBase
        where TEntidadeAnexo : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ANX_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANX_DESCRICAO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANX_NOME_ARQUIVO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ANX_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = true)]
        public virtual string GuidArquivo { get; set; }

        public virtual string ExtensaoArquivo
        {
            get { return System.IO.Path.GetExtension(NomeArquivo).ToLower().Replace(".", ""); }
        }

        #endregion

        #region Propriedades Abstratas

        public abstract TEntidadeAnexo EntidadeAnexo { get; set; }

        #endregion

        #region Métodos Públicos

        public virtual bool Equals(Anexo<TEntidadeAnexo> other)
        {
            return (other.Codigo == this.Codigo);
        }

        #endregion
    }
}
