using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MONTAGEM_CARGA_PATIO", EntityName = "MontagemCargaPatio", Name = "Dominio.Entidades.Embarcador.GestaoPatio.MontagemCargaPatio", NameType = typeof(MontagemCargaPatio))]
    public class MontagemCargaPatio : EntidadeCargaBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FluxoGestaoPatio", Column = "FGP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FluxoGestaoPatio FluxoGestaoPatio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCP_DATA_MONTAGEM_CARGA_INICIADA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataMontagemCargaIniciada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EtapaMontagemCargaLiberada", Column = "MCP_MONTAGEM_CARGA_LIBERADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool EtapaMontagemCargaLiberada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCaixas", Column = "MCP_QUANTIDADE_CAIXAS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeCaixas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeItens", Column = "MCP_QUANTIDADE_ITENS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeItens { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePalletsFracionados", Column = "MCP_QUANTIDADE_PALLETS_FRACIONADOS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadePalletsFracionados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePalletsInteiros", Column = "MCP_QUANTIDADE_PALLETS_INTEIROS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadePalletsInteiros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "MCP_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCargaPatio), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoMontagemCargaPatio Situacao { get; set; }

        public virtual string Descricao
        {
            get { return Carga != null ? $"Montagem da carga {Carga.CodigoCargaEmbarcador}" : $"Montagem da pré carga {PreCarga.NumeroPreCarga}"; }
        }
    }
}
