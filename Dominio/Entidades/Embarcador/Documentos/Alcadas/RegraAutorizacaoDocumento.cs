using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Documentos.Alcadas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_DOCUMENTO", EntityName = "RegraAutorizacaoDocumento", Name = "Dominio.Entidades.Embarcador.Documentos.Alcadas.RegraAutorizacaoDocumento", NameType = typeof(RegraAutorizacaoDocumento))]
    public class RegraAutorizacaoDocumento : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilial", Column = "RAD_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTransportador", Column = "RAD_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValorPagamento", Column = "RAD_VALOR_PAGAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValorPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoOperacao", Column = "RAD_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTomador", Column = "RAD_TOMADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorTomador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorMotivoRejeicao", Column = "RAD_MOTIVO_REJEICAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorMotivoRejeicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorCanalEntrega", Column = "RAD_CANAL_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorCanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorPeso", Column = "RAD_PESO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorPeso { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_DOCUMENTO_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Alcadas.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTransportador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_DOCUMENTO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Alcadas.AlcadaTransportador", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTransportador> AlcadasTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValorPagamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_DOCUMENTO_VALOR_PAGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Alcadas.AlcadaValorPagamento", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaValorPagamento> AlcadasValorPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_DOCUMENTO_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Alcadas.AlcadaTipoOperacao", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoOperacao> AlcadasTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTomador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_DOCUMENTO_TOMADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Alcadas.AlcadaTomador", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTomador> AlcadasTomador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasMotivoRejeicao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_DOCUMENTO_MOTIVO_REJEICAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Alcadas.AlcadaMotivoRejeicao", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaMotivoRejeicao> AlcadasMotivoRejeicao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasCanalEntrega", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_DOCUMENTO_CANAL_ENTREGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Alcadas.AlcadaCanalEntrega", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaCanalEntrega> AlcadasCanalEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasPeso", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_DOCUMENTO_PESO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Alcadas.AlcadaPeso", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaPeso> AlcadasPeso { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_DOCUMENTO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAD_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return (
                RegraPorValorPagamento ||
                RegraPorTomador ||
                RegraPorMotivoRejeicao ||
                RegraPorTipoOperacao ||
                RegraPorFilial ||
                RegraPorTransportador ||
                RegraPorCanalEntrega ||
                RegraPorPeso
            );
        }

        public override void LimparAlcadas()
        {
            AlcadasTipoOperacao?.Clear();
            AlcadasTomador?.Clear();
            AlcadasFilial?.Clear();
            AlcadasTransportador?.Clear();
            AlcadasValorPagamento?.Clear();
            AlcadasMotivoRejeicao?.Clear();
            AlcadasCanalEntrega?.Clear();
            AlcadasPeso?.Clear();
        }

        #endregion
    }
}
