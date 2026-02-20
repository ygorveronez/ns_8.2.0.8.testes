using System;

namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOAS_CONFIGURACAO_COMPONENTE_FRETE", EntityName = "GrupoPessoasConfiguracaoComponentesFrete", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete", NameType = typeof(GrupoPessoasConfiguracaoComponentesFrete))]
    public class GrupoPessoasConfiguracaoComponentesFrete : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasConfiguracaoComponentesFrete>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutraDescricaoCTe", Column = "GRC_OUTRA_DESCRICAO_CTE", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string OutraDescricaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRC_INCLUIR_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GRC_INCLUIR_INTEGRALMENTE_CONTRATO_FRETE_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirIntegralmenteContratoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RateioFormula", Column = "RFO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Rateio.RateioFormula RateioFormula { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.ComponenteFrete?.Descricao ?? string.Empty;
            }
        }

        public virtual bool Equals(GrupoPessoasConfiguracaoComponentesFrete other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
