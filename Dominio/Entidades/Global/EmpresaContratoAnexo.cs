using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_CONTRATO_ANEXO", EntityName = "EmpresaContratoAnexo", Name = "Dominio.Entidades.EmpresaContratoAnexo", NameType = typeof(EmpresaContratoAnexo))]
    public class EmpresaContratoAnexo : EntidadeBase, IEquatable<Dominio.Entidades.EmpresaContratoAnexo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ECA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ECA_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "ECA_NOME_ARQUIVO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoArquivo", Column = "ECA_CAMINHO_ARQUIVO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string CaminhoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaContrato", Column = "ECO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmpresaContrato EmpresaContrato { get; set; }

        public virtual string ExtensaoArquivo
        {
            get { return System.IO.Path.GetExtension(NomeArquivo).ToLower().Replace(".", ""); }
        }

        public virtual bool Equals(EmpresaContratoAnexo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
