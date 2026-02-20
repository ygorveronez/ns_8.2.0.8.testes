using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LICITACAO_PARTICIPACAO", EntityName = "LicitacaoParticipacao", Name = "Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao", NameType = typeof(LicitacaoParticipacao))]
    public class LicitacaoParticipacao : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade, IEquatable<LicitacaoParticipacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LIP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LIP_DATA_ENVIO_OFERTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEnvioOferta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LIP_NUMERO", TypeType = typeof(int), UniqueKey = "UK_T_LICITACAO_PARTICIPACAO_NUMERO", NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "LIP_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoRetorno", Column = "LIP_OBSERVACAO_RETORNO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string ObservacaoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "LIP_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoLicitacaoParticipacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoLicitacaoParticipacao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Licitacao", Column = "LIC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Licitacao Licitacao { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFreteCliente TabelaFreteCliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LIP_RANKING", TypeType = typeof(int), NotNull = false)]
        public virtual int Ranking { get; set; }

        public virtual string Descricao
        {
            get { return Numero.ToString(); }
        }

        public virtual bool Equals(LicitacaoParticipacao other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
