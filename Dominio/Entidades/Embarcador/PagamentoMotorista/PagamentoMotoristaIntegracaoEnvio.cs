using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.PagamentoMotorista
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_MOTORISTA_TMS_INTEGRACAO_ENVIO", EntityName = "PagamentoMotoristaIntegracaoEnvio", Name = "Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio", NameType = typeof(PagamentoMotoristaIntegracaoEnvio))]
    public class PagamentoMotoristaIntegracaoEnvio : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PME_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PME_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Retorno", Column = "PME_RETORNO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Retorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ArquivoEnvio", Column = "PME_ARQUIVO_ENVIO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string ArquivoEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativas", Column = "PME_NUMERO_TENTATIVAS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracaoPagamentoMotorista", Column = "PME_TIPO_INTEGRACAO", TypeType = typeof(int), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista TipoIntegracaoPagamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "PME_SITUACAO_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTMS", Column = "PAM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PagamentoMotoristaTMS PagamentoMotoristaTMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PME_PROTOCOLO_ABERTURA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ProtocoloAbertura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Cancelado", Column = "PME_CANCELADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Cancelado { get; set; }

        public virtual string DescricaoTipoIntegracaoPagamentoMotorista
        {
            get { return TipoIntegracaoPagamentoMotorista.ObterDescricao(); }
        }

        public virtual string DescricaoSituacaoIntegracao
        {
            get { return SituacaoIntegracao.ObterDescricao(); }
        }

        public virtual bool Equals(PagamentoMotoristaAutorizacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
