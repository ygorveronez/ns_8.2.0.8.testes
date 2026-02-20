using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FLUXO_COMPRA", EntityName = "FluxoCompra", Name = "Dominio.Entidades.Embarcador.Compras.FluxoCompra", NameType = typeof(FluxoCompra))]
    public class FluxoCompra : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "FCO_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "FCO_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FCO_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoCompra), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFluxoCompra Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "FCO_ETAPA_ATUAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoCompra), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoCompra EtapaAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VoltouParaEtapaAtual", Column = "FCO_VOLTOU_PARA_ETAPA_ATUAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool VoltouParaEtapaAtual { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CotacaoCompra", Column = "COT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CotacaoCompra Cotacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RequisicoesMercadoria", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FLUXO_COMPRA_REQUISICAO_MERCADORIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RequisicaoMercadoria", Column = "RME_CODIGO")]
        public virtual ICollection<RequisicaoMercadoria> RequisicoesMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "OrdensCompra", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FLUXO_COMPRA_ORDEM_COMPRA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OrdemCompra", Column = "ORC_CODIGO")]
        public virtual ICollection<OrdemCompra> OrdensCompra { get; set; }

        public virtual string Descricao
        {
            get { return Numero.ToString(); }
        }

        public virtual decimal Valor
        {
            get
            {
                return (OrdensCompra.Count > 0) ? (from obj in OrdensCompra where obj.ValorTotal > 0 select obj).Sum(o => o.ValorTotal) : 0;
            }
        }

    }
}
