using System;

namespace Dominio.Entidades.Embarcador.Seguros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_APOLICE_SEGURO_AUTORIZACAO_CARGA", EntityName = "ApoliceSeguraAutorizacaoCarga", Name = "Dominio.Entidades.Embarcador.Seguros.ApoliceSeguraAutorizacaoCarga", NameType = typeof(ApoliceSeguraAutorizacaoCarga))]
    public class ApoliceSeguraAutorizacaoCarga : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ApoliceSeguro", Column = "APS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro ApoliceSeguro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalMercadoria", Column = "APS_VALOR_TOTAL_MERCADORIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorLimiteApolice", Column = "APS_VALOR_LIMITE_APOLICE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorLimiteApolice { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataHora", Column = "AAC_DATA_HORA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataHora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAutorizacaoApolice", Column = "AAC_SITUACAO_LIBERACAO_APOLICE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoApolice), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoApolice SituacaoAutorizacaoApolice { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MotivoExtornoAutorizacao", Column = "APS_MOTIVO_EXTORNO_AUTORIZACAO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string MotivoExtornoAutorizacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return (this.Carga?.CodigoCargaEmbarcador ?? string.Empty) + " - " + this.ApoliceSeguro.NumeroApolice.ToString();
            }
        }

    }
}
