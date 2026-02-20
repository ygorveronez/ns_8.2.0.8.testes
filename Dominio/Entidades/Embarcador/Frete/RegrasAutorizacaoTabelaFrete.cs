using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_VALOR_FRETE", EntityName = "RegrasAutorizacaoTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.RegrasAutorizacaoTabelaFrete", NameType = typeof(RegrasAutorizacaoTabelaFrete))]
    public class RegrasAutorizacaoTabelaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RAF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAF_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAF_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Vigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAF_NUMERO_APROVADORES", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroAprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrioridadeAprovacao", Column = "RAF_PRIORIDADE_APROVACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrioridadeAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RAF_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAF_OBSERVACOES", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAF_ETAPA_AUTORIZACAO", TypeType = typeof(EtapaAutorizacaoTabelaFrete), NotNull = false)]
        public virtual EtapaAutorizacaoTabelaFrete EtapaAutorizacaoTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAF_TIPO_APROVADOR_REGRA", TypeType = typeof(TipoAprovadorRegra), NotNull = false)]
        public virtual TipoAprovadorRegra TipoAprovadorRegra { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAF_MOTIVA_REAJUSTE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorMotivoReajuste { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAF_ORIGEM", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorOrigemFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAF_DESTINO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorDestinoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAF_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAF_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAF_VALOR_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAF_VALOR_PEDAGIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAF_ADVALOREM", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorAdValorem{ get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAF_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarLinkParaAprovacaoPorEmail", Column = "RAF_ENVIAR_LINK_PARA_APROVACAO_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarLinkParaAprovacaoPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_VALOR_FRETE_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasMotivoReajuste", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_VALOR_FRETE_MOTIVO_REAJUSTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasMotivoReajuste", Column = "RFM_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.RegrasMotivoReajuste> RegrasMotivoReajuste { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasOrigemFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_VALOR_FRETE_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasOrigemFrete", Column = "RFO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.RegrasOrigemFrete> RegrasOrigemFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasDestinoFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_VALOR_FRETE_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasDestinoFrete", Column = "RFD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.RegrasDestinoFrete> RegrasDestinoFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasTransportador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_VALOR_FRETE_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasTransportador", Column = "RFT_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.RegrasTransportador> RegrasTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_VALOR_FRETE_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasTipoOperacaoFrete", Column = "RTO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.RegrasTipoOperacao> RegrasTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasValorFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_VALOR_FRETE_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasValorFrete", Column = "RFV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.RegrasValorFrete> RegrasValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasValorPedagio", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_VALOR_FRETE_VALOR_PEDAGIO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasValorPedagio", Column = "RFP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.RegrasValorPedagio> RegrasValorPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasAdValorem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_VALOR_FRETE_ADVALOREM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasAdValorem", Column = "RFA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.RegrasAdValorem> RegrasAdValorem { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_VALOR_FRETE_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasAjusteFilial", Column = "RFF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.RegrasFilial> RegrasFilial { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        public virtual string DescricaoEtapaAutorizacaoTabelaFrete
        {
            get { return EtapaAutorizacaoTabelaFrete.ObterDescricao(); }
        }

        public virtual bool Equals(RegrasAutorizacaoTabelaFrete other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}