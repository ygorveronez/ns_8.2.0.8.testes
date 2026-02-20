namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_PARAMETRO_OCORRENCIA_GERADO", EntityName = "OcorrenciaParametroOcorrenciaGerado", Name = "Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrenciaGerado", NameType = typeof(OcorrenciaParametroOcorrenciaGerado))]
    public class OcorrenciaParametroOcorrenciaGerado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OPG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OcorrenciaParametroOcorrencia", Column = "OPO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OcorrenciaParametroOcorrencia OcorrenciaParametroOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OPG_DESCRICAO", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OPG_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OPG_VALOR_ALTERADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ValorAlterado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }
    }
}
