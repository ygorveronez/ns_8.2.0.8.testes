using System;
    
namespace Dominio.Entidades.WebService
{   
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERMISSAO_WEBSERVICE", EntityName = "PermissaoWebservice", Name = "Dominio.Entidades.WebService.PermissaoWebservice", NameType = typeof(PermissaoWebservice))]
    public class PermissaoWebservice : EntidadeBase, IEquatable<Dominio.Entidades.WebService.PermissaoWebservice>
    {   
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEW_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "RequisicoesMinuto", Column = "PEW_REQUISICOES_POR_MINUTO", TypeType = typeof(int), NotNull = true)]
        public virtual int RequisicoesMinuto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeMetodo", Column = "PEW_NOME_METODO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string NomeMetodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UltimoReset", Column = "PEW_DATA_ULTIMA_RESET", TypeType = typeof(DateTime), Length = 100, NotNull = true)]
        public virtual DateTime UltimoReset { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdRequisicoes", Column = "PEW_QTD_REQUISICOES", TypeType = typeof(int), NotNull = true)]
        public virtual int QtdRequisicoes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Integradora", Column = "INT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.WebService.Integradora Integradora { get; set; }

        public virtual bool Equals(PermissaoWebservice other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
