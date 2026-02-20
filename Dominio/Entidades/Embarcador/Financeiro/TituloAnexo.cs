using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TITULO_ANEXO", EntityName = "TituloAnexo", Name = "Dominio.Entidades.Embarcador.Financeiro.TituloAnexo", NameType = typeof(TituloAnexo))]
    public class TituloAnexo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.TituloAnexo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TFA_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "TFA_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoArquivo", Column = "TFA_CAMINHO_ARQUIVO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string CaminhoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Titulo Titulo { get; set; }

        public virtual string ExtensaoArquivo
        {
            get { return System.IO.Path.GetExtension(NomeArquivo).ToLower().Replace(".", ""); }
        }

        public virtual bool Equals(TituloAnexo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
