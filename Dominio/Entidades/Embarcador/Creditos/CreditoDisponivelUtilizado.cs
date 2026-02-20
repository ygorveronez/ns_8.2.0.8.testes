using System;

namespace Dominio.Entidades.Embarcador.Creditos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CREDITO_DISPONIVEL_UTILIZADO", EntityName = "CreditoDisponivelUtilizado", Name = "Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado", NameType = typeof(CreditoDisponivelUtilizado))]
    public class CreditoDisponivelUtilizado : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Creditos.CreditoDisponivelUtilizado>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDU_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CreditoDisponivel", Column = "CDI_CODIGO_ORIGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CreditoDisponivel CreditoDisponivelOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CreditoDisponivel", Column = "CDI_CODIGO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CreditoDisponivel CreditoDisponivelDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaComplementoFrete", Column = "CCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.CargaComplementoFrete CargaComplementoFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CreditoExtra", Column = "CEX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Creditos.CreditoExtra CreditoExtra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SolicitacaoCredito", Column = "SCR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Creditos.SolicitacaoCredito SolicitacaoCredito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUtilizacao", Column = "CDU_DATA_UTILIZACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataUtilizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUtilizado", Column = "CDU_VALOR_UTILIZADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorUtilizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComprometido", Column = "CDU_VALOR_COMPROMETIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorComprometido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoCreditoUtilizado", Column = "CCF_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoUtilizado), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCreditoUtilizado SituacaoCreditoUtilizado { get; set; }

        public virtual bool Equals(CreditoDisponivelUtilizado other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
