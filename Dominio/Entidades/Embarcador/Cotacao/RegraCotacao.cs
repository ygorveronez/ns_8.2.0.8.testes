using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cotacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_COTACAO", EntityName = "RegraCotacao", Name = "Dominio.Entidades.Embarcador.Cotacao.RegraCotacao", NameType = typeof(RegraCotacao))]
    public class RegraCotacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RCO_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vigencia", Column = "RCO_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Vigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RCO_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAplicacao", Column = "RCO_TIPO_APLICACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoCotacao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoCotacao TipoAplicacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualCobranca", Column = "RCO_PERCENTUAL_COBRANCA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal? PercentualCobranca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFixoCotacaoFrete", Column = "RCO_VALOR_FIXO_COTACAO_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFixoCotacaoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCobranca", Column = "RCO_VALOR_COBRANCA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCobranca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCO_DIAS_FRETE", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroDiasFrete { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Transportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_TRANSPORTADORES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual ICollection<Empresa> Transportadores { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloVeicularCarga", Column = "MVC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCO_PRIORIDADE_REGRA", TypeType = typeof(int), NotNull = false)]
        public virtual int? PrioridadeRegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacoes", Column = "RCO_OBSERVACOES", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorPeso", Column = "RCO_PESO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorDistancia", Column = "RCO_DISTANCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorDistancia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValorMercadoria", Column = "RCO_VALOR_MERCADORIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorValorMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorGrupoProduto", Column = "RCO_GRUPO_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorGrupoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorEstadoDestino", Column = "RCO_ESTADO_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorEstadoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorExpedidor", Column = "RCO_EXPEDIDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorExpedidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTransportador", Column = "RCO_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorProduto", Column = "RCO_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorCubagem", Column = "RCO_CUBAGEM", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorCEPDestino", Column = "RCO_CEP_DESTINO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorCEPDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorMarcaProduto", Column = "RCO_MARCA_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorMarcaProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorVolume", Column = "RCO_VOLUME", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorVolume { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorLinhaSeparacao", Column = "RCO_LINHA_SEPARACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorLinhaSeparacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorArestaProduto", Column = "RCO_ARESTA_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorArestaProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValorCotacao", Column = "RCO_VALOR_COTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorValorCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorDestinatario", Column = "RCO_DESTINATARIO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasPeso", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_PESO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasPeso")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cotacao.RegrasPeso> RegrasPeso { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasDistancia", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_DISTANCIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasDistancia")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cotacao.RegrasDistancia> RegrasDistancia { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasValorMercadoria", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_VALOR_MERCADORIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasValorMercadoria")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cotacao.RegrasValorMercadoria> RegrasValorMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasGrupoProduto", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_GRUPO_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasGrupoProduto")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cotacao.RegrasGrupoProduto> RegrasGrupoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasEstadoDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_ESTADO_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasEstadoDestino")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cotacao.RegrasEstadoDestino> RegrasEstadoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasExpedidor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_EXPEDIDOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasExpedidor")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cotacao.RegrasExpedidor> RegrasExpedidor { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasDestinatario", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_DESTINATARIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasDestinatario")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cotacao.RegrasDestinatario> RegrasDestinatario { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasCotacaoTransportador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasCotacaoTransportador")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cotacao.RegrasCotacaoTransportador> RegrasCotacaoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasProduto", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasProduto")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cotacao.RegrasProduto> RegrasProduto { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasCubagem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_CUBAGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasCubagem")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cotacao.RegrasCubagem> RegrasCubagem { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasCepDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_CEP_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasCepDestino")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cotacao.RegrasCepDestino> RegrasCepDestino { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasMarcaProduto", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_MARCA_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasMarcaProduto")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cotacao.RegrasMarcaProduto> RegrasMarcaProduto { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasVolume", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_VOLUME")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasVolume")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cotacao.RegrasVolume> RegrasVolume { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasLinhaSeparacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_LINHA_SEPARACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasLinhaSeparacao")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cotacao.RegrasLinhaSeparacao> RegrasLinhaSeparacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasArestaProduto", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_ARESTA_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasArestaProduto")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cotacao.RegrasArestaProduto> RegrasArestaProduto { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RegrasValorCotacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_COTACAO_VALOR_COTACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "RegrasValorCotacao")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cotacao.RegrasValorCotacao> RegrasValorCotacao { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return (this.Ativo) ? "Ativo" : "Inativo"; }
        }
    }
}
