using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SEPARACAO_MERCADORIA", EntityName = "SeparacaoMercadoria", Name = "Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoria", NameType = typeof(SeparacaoMercadoria))]
    public class SeparacaoMercadoria : EntidadeCargaBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SMR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SMR_DATA_SEPARACAO_MERCADORIA_INFORMADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSeparacaoMercadoriaInformada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaSeparacaoMercadoriaLiberada", Column = "SMR_SEPARACAO_MERCADORIA_LIBERADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EtapaSeparacaoMercadoriaLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SMR_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoSeparacaoMercadoria), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoSeparacaoMercadoria Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCarregadores", Column = "SMR_NUMERO_CARREGADORES", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroCarregadores { get; set; }
        
        [Obsolete("Migrado para uma lista", true)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSeparadores", Column = "SMR_NUMERO_SEPARADORES", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroSeparadores { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "SMR_RESPONSAVEL_CARREGAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario ResponsavelCarregamento { get; set; }
        
        [Obsolete("Migrado para uma lista", true)]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "SMR_RESPONSAVEL_SEPARACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario ResponsavelSeparacao { get; set; }

        public virtual string Descricao
        {
            get { return Carga != null ? $"Separação de mercadoria da carga {Carga.CodigoCargaEmbarcador}" : $"Separação de mercadoria da pré carga {PreCarga.NumeroPreCarga}"; }
        }
    }
}
