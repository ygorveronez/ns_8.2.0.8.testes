using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OPERACAO_CONFIGURACAO_COMPONENTE_FRETE", EntityName = "TipoOperacaoConfiguracaoComponentes", Name = "Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes", NameType = typeof(TipoOperacaoConfiguracaoComponentes))]
    public class TipoOperacaoConfiguracaoComponentes : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoConfiguracaoComponentes>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TOC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutraDescricaoCTe", Column = "TOC_OUTRA_DESCRICAO_CTE", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string OutraDescricaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOC_INCLUIR_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TOC_INCLUIR_INTEGRALMENTE_CONTRATO_FRETE_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirIntegralmenteContratoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioFormula", Column = "RFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Rateio.RateioFormula RateioFormula { get; set; }

        public virtual bool Equals(TipoOperacaoConfiguracaoComponentes other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string Descricao
        {
            get
            {
                return this.ComponenteFrete?.Descricao ?? string.Empty;
            }
        }
    }
}
