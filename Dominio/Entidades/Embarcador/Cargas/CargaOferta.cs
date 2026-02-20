using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_OFERTA", EntityName = "CargaOferta", Name = "Dominio.Entidades.Embarcador.Cargas.CargaOferta", NameType = typeof(CargaOferta))]
    public class CargaOferta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "CAO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "CAO_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaOferta), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaOferta Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CAO_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "CAO_DATA_HORA_CRIACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "CAO_DATA_HORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAceite", Column = "CAO_DATA_HORA_ACEITE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAceite { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "CAO_SITUACAO_INTEGRACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParametrosOfertas", Column = "POF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Ofertas.ParametrosOfertas ParametrosOfertas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimOferta", Column = "CAO_DATA_FIM_OFERTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimOferta { get; set; }

        public virtual string Descricao { get; set; }
    }
}
