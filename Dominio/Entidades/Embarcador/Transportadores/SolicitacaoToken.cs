using Dominio.Interfaces.Embarcador.Entidade;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SOLICITACAO_TOKEN", EntityName = "SolicitacaoToken", Name = "Dominio.Entidades.Embarcador.Transportadores.SolicitacaoToken", NameType = typeof(SolicitacaoToken))]
    public class SolicitacaoToken : EntidadeBase,IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "STO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "STO_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroProtocolo", Column = "STO_NUMERO_PROTOCOLO", TypeType = typeof(int), Length = 100, NotNull = true)]
        public virtual int NumeroProtocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "STO_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioVigencia", Column = "STO_DATA_INICIO_VIGENCIA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataInicioVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimVigencia", Column = "STO_DATA_FIM_VIGENCIA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataFimVigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAutenticacao", Column = "STO_TIPO_AUTENTICACAO", TypeType = typeof(TipoAutenticacao), NotNull = true)]
        public virtual TipoAutenticacao TipoAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoAutenticacao", Column = "STO_SITUACAO_AUTENTICACAO", TypeType = typeof(SituacaoAutorizacaoToken), NotNull = false)]
        public virtual SituacaoAutorizacaoToken SituacaoAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "STO_SITUACAO", TypeType = typeof(EtapaAutorizacaoToken), NotNull = false)]
        public virtual EtapaAutorizacaoToken Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "STO_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Transportadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SOLICITACAO_TOKEN_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "STO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Empresa", Column = "EMP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Empresa> Transportadores { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "PermissoesWS", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SOLICITACAO_TOKEN_PERMISSAO_WEB_SERVICE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "STO_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PermissaoWebServiceSolicitacaoToken", Column = "PEW_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Transportadores.PermissaoWebServiceSolicitacaoToken> PermissoesWS { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "SolicitacoesTokenTransportador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_SOLICITACAO_TOKEN_GERADO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "STO_CODIGO")]
        [NHibernate.Mapping.Attributes.OneToMany(2, Class = "SolicitacaoTokenTransportador")]
        public virtual ICollection<SolicitacaoTokenTransportador> SolicitacoesTokenTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoExpiracao", Column = "STO_TEMPO_EXPIRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoExpiracao { get; set; }

    }
}

