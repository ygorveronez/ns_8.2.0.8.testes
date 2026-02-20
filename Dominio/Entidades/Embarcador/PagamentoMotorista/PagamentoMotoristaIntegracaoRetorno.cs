using System;

namespace Dominio.Entidades.Embarcador.PagamentoMotorista
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_MOTORISTA_TMS_INTEGRACAO_RETORNO", EntityName = "PagamentoMotoristaIntegracaoRetorno", Name = "Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno", NameType = typeof(PagamentoMotoristaIntegracaoRetorno))]
    public class PagamentoMotoristaIntegracaoRetorno : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PMR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PMR_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRetorno", Column = "PMR_CODIGO_RETORNO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoRetorno", Column = "PME_DESCRICAO_RETORNO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string DescricaoRetorno { get; set; }

        [Obsolete("Usuar ArquivoResposta")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "ArquivoRetorno", Column = "PME_ARQUIVO_RETORNO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string ArquivoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTMS", Column = "PAM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PagamentoMotoristaTMS PagamentoMotoristaTMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaIntegracaoEnvio", Column = "PME_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PagamentoMotoristaIntegracaoEnvio PagamentoMotoristaIntegracaoEnvio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_REQUISICAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_RESPOSTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Integracao.ArquivoIntegracao ArquivoResposta { get; set; }

        public virtual bool Equals(PagamentoMotoristaIntegracaoRetorno other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
