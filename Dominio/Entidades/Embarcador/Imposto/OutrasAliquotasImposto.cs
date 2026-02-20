using System;

namespace Dominio.Entidades.Embarcador.Imposto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OUTRAS_ALIQUOTAS_IMPOSTO", EntityName = "OutrasAliquotasImposto", Name = "Dominio.Entidades.OutrasAliquotasImposto", NameType = typeof(OutrasAliquotasImposto))]
    public class OutrasAliquotasImposto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OAI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "OAI_DESCRICAO", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OutrasAliquotas", Column = "TOA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OutrasAliquotas OutrasAliquotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoImposto", Column = "OAI_TIPO_IMPOSTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImposto), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImposto TipoImposto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Aliquota", Column = "OAI_ALIQUOTA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? Aliquota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaUf", Column = "OAI_ALIQUOTA_UF", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? AliquotaUf { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaMunicipio", Column = "OAI_ALIQUOTA_MUNICIPIO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? AliquotaMunicipio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVigenciaInicio", Column = "OAI_DATA_VIGENCIA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataVigenciaInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVigenciaFinal", Column = "OAI_DATA_VIGENCIA_FINAL", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataVigenciaFinal { get; set; }

        [Obsolete("O campo vai ser alterado para CalcularImpostosDocumento, na entidade T_OUTRAS_ALIQUOTAS")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "InclusaoDocumento", Column = "OAI_INCLUSAO_DOCUMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool InclusaoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Localidade Municipio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Estado UF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReducaoUf", Column = "OAI_REDUCAO_UF", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? ReducaoUf { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Reducao", Column = "OAI_REDUCAO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? Reducao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReducaoMunicipio", Column = "OAI_REDUCAO_MUNICIPIO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? ReducaoMunicipio { get; set; }
    }

}
