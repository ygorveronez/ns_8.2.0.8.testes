using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_CONTA_EXPORTACAO", EntityName = "ConfiguracaoContaExportacao", Name = "Dominio.Entidades.Embarcador.Financeiro.ConfiguracaoContaExportacao", NameType = typeof(ConfiguracaoContaExportacao))]
    public class ConfiguracaoContaExportacao: EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.ConfiguracaoContaExportacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoPagamentoRecebimento", Column = "TPR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento TipoPagamentoRecebimento { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_CONTA_CONTABIL", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_CODIGO_CENTRO_RESULTADO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoCentroResultado { get; set; }

        /// <summary>
        /// Utilizado apenas no tipo de pagamento / recebimento para indicar se a configuração é de geração ou reversão (cancelamento)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "CCE_REVERSAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? Reversao { get; set; }

        public virtual bool Equals(ConfiguracaoContaExportacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
