using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Localidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGIAO_PRAZO_ENTREGA", EntityName = "RegiaoPrazoEntrega", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Localidades.RegiaoPrazoEntrega", NameType = typeof(RegiaoPrazoEntrega))]
    public class RegiaoPrazoEntrega : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Regiao", Column = "REG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidades.Regiao Regiao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [Obsolete("Migrado para um campo único.")]
        [NHibernate.Mapping.Attributes.Set(0, Name = "Filiais", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRAZO_ENTREGA_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Filial", Column = "FIL_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Filiais.Filial> Filiais { get; set; }

        [Obsolete("Migrado para um campo único.")]
        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOperacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRAZO_ENTREGA_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> TiposOperacoes { get; set; }

        [Obsolete("Migrado para um campo único.")]
        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PRAZO_ENTREGA_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoDeCarga", Column = "TCG_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> TiposCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RPE_PADRAO_TEMPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.PadraoTempoDiasMinutos), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.PadraoTempoDiasMinutos PadraoTempo { get; set; }

        ///<summary>
        ///Esse campo pode ser por dias ou minutos, dependendo do valor do campo "PadraoTempo"
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoDeViagemEmMinutos", Column = "RPE_TEMPO_VIAGEM_EM_MINUTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoDeViagemEmMinutos { get; set; }
    }
}
