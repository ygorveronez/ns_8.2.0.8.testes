using System;

namespace Dominio.Entidades.Embarcador.ConfiguracaoContabil
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_CONTA_CONTABIL_ESCRITURACAO", EntityName = "ConfiguracaoContaContabilEscrituracao", Name = "Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao", NameType = typeof(ConfiguracaoContaContabilEscrituracao))]
    public class ConfiguracaoContaContabilEscrituracao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilEscrituracao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoContaContabil", Column = "CCC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil ConfiguracaoContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContaContabil", Column = "CCE_TIPO_CONTA_CONTABIL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContaContabil TipoContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoContabilizacao", Column = "CCE_TIPO_CONTABILIZACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoContabilizacao TipoContabilizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SempreGerarRegistro", Column = "DCB_SEMPRE_GERAR_REGISTRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SempreGerarRegistro { get; set; }


        public virtual string Descricao
        {
            get
            {
                return this.ConfiguracaoContaContabil.Descricao + " (" + PlanoConta.Descricao + ")";
            }
        }

        public virtual bool Equals(ConfiguracaoContaContabilEscrituracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
