using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_PAGAMENTO_RECEBIMENTO", EntityName = "TipoPagamentoRecebimento", Name = "Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento", NameType = typeof(TipoPagamentoRecebimento))]
    public class TipoPagamentoRecebimento : EntidadeBase, IEquatable<TipoPagamentoRecebimento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TPR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TPR_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PlanoConta PlanoConta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "TPR_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "TPR_OBSERVACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimiteConta", Column = "TPR_LIMITE_CONTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal LimiteConta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPR_EXPORTAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Exportar { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TPR_OBRIGA_CHEQUE_BAIXA_TITULO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ObrigaChequeBaixaTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ContasExportacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CONFIGURACAO_CONTA_EXPORTACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TPR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConfiguracaoContaExportacao", Column = "CCE_CODIGO")]
        public virtual IList<ConfiguracaoContaExportacao> ContasExportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TPR_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                switch (this.Ativo)
                {
                    case true:
                        return "Ativo";
                    case false:
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(TipoPagamentoRecebimento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
