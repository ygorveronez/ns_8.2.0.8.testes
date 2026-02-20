using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_PEDIDO", EntityName = "RegrasPedido", Name = "Dominio.Entidades.Embarcador.Pedidos.RegrasPagamentoMotorista", NameType = typeof(RegrasPedido))]
    public class RegrasPedido : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RPE_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vigencia", Column = "RPE_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Vigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAprovadores", Column = "RPE_NUMERO_APROVADORES", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroAprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacoes", Column = "RPE_OBSERVACOES", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorGrupoPessoa", Column = "RPE_GRUPO_PESSOA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorGrupoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoCarga", Column = "RPE_TIPO_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoOperacao", Column = "RPE_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorModeloVeicular", Column = "RPE_TIPO_MODELO_VEICULAR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValorFrete", Column = "RPM_VALOR_FRETE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorValorFrete { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorDistancia", Column = "RPM_DISTANCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorDistancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorDiferencaFreteLiquidoParaFreteTerceiro", Column = "RPE_DIFERENCA_FRETE_LIQUIDO_PARA_FRETE_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorDiferencaFreteLiquidoParaFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorSituacaoColaborador", Column = "RPM_SITUACAO_COLABORADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorSituacaoColaborador { get; set; }        

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_PEDIDO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasPedidoGrupoPessoa", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PEDIDO_GRUPO_PESSOA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasPedidoGrupoPessoa", Column = "RPG_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoGrupoPessoa> RegrasPedidoGrupoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasPedidoTipoCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PEDIDO_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasPedidoTipoCarga", Column = "RTC_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoCarga> RegrasPedidoTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasPedidoTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PEDIDO_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasPedidoTipoOperacao", Column = "RTO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoTipoOperacao> RegrasPedidoTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasPedidoModeloVeicular", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PEDIDO_MODELO_VEICULAR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasPedidoModeloVeicular", Column = "RMV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoModeloVeicular> RegrasPedidoModeloVeicular { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasPedidoValorFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PEDIDO_VALOR_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasPedidoValorFrete", Column = "RVF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoValorFrete> RegrasPedidoValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasPedidoSituacaoColaborador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PEDIDO_SITUACAO_COLABORADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasPedidoSituacaoColaborador", Column = "RTO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoSituacaoColaborador> RegrasPedidoSituacaoColaborador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasPedidoDistancia", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PEDIDO_DISTANCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasPedidoDistancia", Column = "RPD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoDistancia> RegrasPedidoDistancia { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PEDIDO_PERCENTUAL_DIFERENCA_FRETE_LIQUIDO_PARA_FRETE_TERCEIRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro", Column = "RPP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Pedidos.RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro> RegrasPedidoPercentualDiferencaFreteLiquidoParaFreteTerceiro { get; set; }

        public virtual bool Equals(RegrasPedido other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }

}
