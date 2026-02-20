using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_COMPOSICAO_FRETE", EntityName = "CargaComposicaoFrete", Name = "Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete", NameType = typeof(CargaComposicaoFrete))]
    public class CargaComposicaoFrete : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.ComposicaoFrete.CargaComposicaoFrete>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreCarga", Column = "PCA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.PreCargas.PreCarga PreCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [Obsolete("Migrado para a lista de pedidos. Será removido.")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CargaPedidos", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_COMPOSICAO_FRETE_CARGA_PEDIDO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaPedido", Column = "CPE_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.CargaPedido> CargaPedidos { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PedidoXMLNotasFiscais", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_COMPOSICAO_FRETE_PEDIDO_XML_NOTA_FISCAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoXMLNotaFiscal", Column = "PNF_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> PedidoXMLNotasFiscais { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PedidoCTesParaSubContratacao", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_COMPOSICAO_FRETE_PEDIDO_CTE_PARA_SUBCONTRATACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CCF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoCTeParaSubContratacao", Column = "PSC_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> PedidoCTesParaSubContratacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComposicaoFreteFilialEmissora", Column = "CCF_COMPOSICAO_FRETE_FILIAL_EMISSORA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComposicaoFreteFilialEmissora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_FORMULA", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string Formula { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_VALORES_FORMULA", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string ValoresFormula { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CCF_DESCRICAO_COMPONENTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DescricaoComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "CCF_VALOR", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCalculado", Column = "CCF_VALOR_CALCULADO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCalculado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoParametro", Column = "CCF_TIPO_PARAMETRO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete TipoParametro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCampoValor", Column = "CCF_TIPO_CAMPO_VALOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoCampoValor { get; set; }

        public virtual string Descricao
        {
            get { return (TipoParametro == TipoParametroBaseTabelaFrete.ComponenteFrete || TipoParametro == TipoParametroBaseTabelaFrete.ValorFreteLiquido) ? DescricaoComponente : TipoParametro.ObterDescricao(); }
        }

        public virtual bool Equals(CargaComposicaoFrete other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
