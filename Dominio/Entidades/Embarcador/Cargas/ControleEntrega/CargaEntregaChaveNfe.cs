using System;

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_ENTREGA_CHAVE_NFE", EntityName = "CargaEntregaChaveNfe", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe", NameType = typeof(CargaEntregaChaveNfe))]
    public class CargaEntregaChaveNfe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_CHAVE_NFE", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string ChaveNfe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        public virtual string Descricao =>  Codigo.ToString();
    }
}

