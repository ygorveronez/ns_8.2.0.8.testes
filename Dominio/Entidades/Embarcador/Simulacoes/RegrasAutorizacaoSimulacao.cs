using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Simulacoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_SIMULACAO", EntityName = "RegrasAutorizacaoSimulacao", Name = "Dominio.Entidades.Embarcador.Simulacoes.RegrasAutorizacaoSimulacao", NameType = typeof(RegrasAutorizacaoSimulacao))]
    public class RegrasAutorizacaoSimulacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RAS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RAS_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vigencia", Column = "RAS_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Vigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAprovadores", Column = "RAS_NUMERO_APROVADORES", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroAprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroReprovadores", Column = "RAS_NUMERO_REPROVADORES", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroReprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacoes", Column = "RAS_OBSERVACOES", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarLinkParaAprovacaoPorEmail", Column = "RAS_ENVIAR_LINK_PARA_APROVACAO_POR_EMAIL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarLinkParaAprovacaoPorEmail { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "RAS_SITUACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilialEmissao", Column = "RAS_FILIAL_EMISSAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilialEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoOperacao", Column = "RAS_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorOrigem", Column = "RAS_ORIGEM", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorDestino", Column = "RAS_DESTINO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTransportador", Column = "RAS_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_SIMULACAO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasFilialEmissao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_SIMULACAO_FILIAL_EMISSAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasSimulacaoFilialEmissao", Column = "RSF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoFilialEmissao> RegrasFilialEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_SIMULACAO_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasSimulacaoTipoOperacao", Column = "RTO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTipoOperacao> RegrasTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasOrigem", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_SIMULACAO_ORIGEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasSimulacaoOrigem", Column = "RSO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoOrigem> RegrasOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasDestino", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_SIMULACAO_DESTINO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasSimulacaoDestino", Column = "RSD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoDestino> RegrasDestino { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "RegrasTransportador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_SIMULACAO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RegrasSimulacaoTransportador", Column = "RST_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTransportador> RegrasTransportador { get; set; }


        public virtual string DescricaoSituacao
        {
            get { return (this.Situacao) ? "Ativo" : "Inativo"; }
        }

        public virtual bool Equals(RegrasAutorizacaoSimulacao other)
        {
            return (this.Codigo == other.Codigo);
        }
    }

}
