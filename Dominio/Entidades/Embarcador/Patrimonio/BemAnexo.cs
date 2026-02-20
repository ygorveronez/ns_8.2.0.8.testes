using System;

namespace Dominio.Entidades.Embarcador.Patrimonio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BEM_ANEXO", EntityName = "BemAnexo", Name = "Dominio.Entidades.Embarcador.Patrimonio.BemAnexo", NameType = typeof(BemAnexo))]
    public class BemAnexo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Patrimonio.BemAnexo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BEA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "BEA_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "BEA_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoArquivo", Column = "BEA_CAMINHO_ARQUIVO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string CaminhoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Bem", Column = "BEM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Bem Bem { get; set; }

        public virtual bool Equals(BemAnexo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
