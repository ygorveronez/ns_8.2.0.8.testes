using System;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FAIXA_TEMPERATURA", EntityName = "FaixaTemperatura", Name = "Dominio.Entidades.Embarcador.Logistica.FaixaTemperatura", NameType = typeof(FaixaTemperatura))]
    public class FaixaTemperatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FTE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "FTE_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FaixaInicial", Column = "FTE_FAIXA_INICIAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal FaixaInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FaixaFinal", Column = "FTE_FAIXA_FINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal FaixaFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Carimbo", Column = "FTE_CARIMBO", TypeType = typeof(int), NotNull = false)]
        public virtual int Carimbo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CarimboDescricao", Column = "FTE_CARIMBO_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CarimboDescricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "FTE_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProcedimentoEmbarque", Column = "PRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque ProcedimentoEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemLicencaVencidaEmbarcador", Column = "FTE_MENSAGEM_LICENCA_VENCIDA_EMBARCADOR", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string MensagemLicencaVencidaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemLicencaVencidaTransportador", Column = "FTE_MENSAGEM_LICENCA_VENCIDA_TRANSPORTADOR", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string MensagemLicencaVencidaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemLicencaReprovadaEmbarcador", Column = "FTE_MENSAGEM_LICENCA_REPROVADA_EMBARCADOR", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string MensagemLicencaReprovadaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemLicencaReprovadaTransportador", Column = "FTE_MENSAGEM_LICENCA_REPROVADA_TRANSPORTADOR", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string MensagemLicencaReprovadaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaModificacao", Column = "FTE_DATA_ULTIMA_MODIFICACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaModificacao { get; set; }

        #region Propriedades virtuais
        
        public virtual string DescricaoVariancia
        {
            get
            {
                return $"{this.FaixaInicial}° até {this.FaixaFinal}°";
            }
        }

        #endregion
    }
}
