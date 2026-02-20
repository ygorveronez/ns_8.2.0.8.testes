using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_PAGAMENTO_CONTRATO_FRETE", EntityName = "AutorizacaoPagamentoContratoFrete", Name = "Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete", NameType = typeof(AutorizacaoPagamentoContratoFrete))]
    public class AutorizacaoPagamentoContratoFrete : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Terceiros.AutorizacaoPagamentoContratoFrete>
    {
        public AutorizacaoPagamentoContratoFrete() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "APC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "APC_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "APC_TIPO_PAGAMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPagamentoAutorizacaoPagamento), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EnumTipoPagamentoAutorizacaoPagamento TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_CRIACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ContratoFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AUTORIZACAO_PAGAMENTO_CONTRATO_FRETE_DOCUMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "APC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ContratoFrete", Column = "CFT_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> ContratoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "APC_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCriacao { get; set; }

        public virtual AutorizacaoPagamentoContratoFrete Clonar()
        {
            return (AutorizacaoPagamentoContratoFrete)this.MemberwiseClone();
        }

        public virtual bool Equals(AutorizacaoPagamentoContratoFrete other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}