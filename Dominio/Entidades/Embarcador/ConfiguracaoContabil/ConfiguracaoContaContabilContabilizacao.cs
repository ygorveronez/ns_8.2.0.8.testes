using System;

namespace Dominio.Entidades.Embarcador.ConfiguracaoContabil
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_CONTA_CONTABIL_CONTABILIZACAO", EntityName = "ConfiguracaoContaContabilContabilizacao", Name = "Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao", NameType = typeof(ConfiguracaoContaContabilContabilizacao))]
    public class ConfiguracaoContaContabilContabilizacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilContabilizacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoContaContabil", Column = "CCC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil ConfiguracaoContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoConta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO_CONTRA_PARTIDA_PROVISAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoContaContraPartidaProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaContabil", Column = "CCT_TIPO_CONTA_CONTABIL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil TipoContaContabil { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContabilizacao", Column = "CCT_TIPO_CONTABILIZACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao TipoContabilizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCT_COMPONENTES_DE_FRETE_DO_TIPO_DESCONTO_NAO_DEVEM_SOMAR_NA_CONTABILIZACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaContabilizacao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.ConfiguracaoContaContabil.Descricao + " (" + PlanoConta.Descricao + ")";
            }
        }

        public virtual bool Equals(ConfiguracaoContaContabilContabilizacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
