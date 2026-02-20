using System;

namespace Dominio.Entidades.Embarcador.PagamentoMotorista
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PAGAMENTO_MOTORISTA_TMS_AUTORIZACAO", EntityName = "PagamentoMotoristaAutorizacao", Name = "Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao", NameType = typeof(PagamentoMotoristaAutorizacao))]
    public class PagamentoMotoristaAutorizacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PMA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PMA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivo", Column = "PMA_MOTIVO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PMA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PMA_ETAPA_AUTORIZACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia EtapaAutorizacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }        

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PagamentoMotoristaTMS", Column = "PAM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PagamentoMotoristaTMS PagamentoMotoristaTMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasPagamentoMotorista", Column = "RPM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasPagamentoMotorista RegrasPagamentoMotorista { get; set; }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada:
                        return "Aprovada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente:
                        return "Pendente";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada:
                        return "Rejeitada";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoEtapaAutorizacaoOcorrencia
        {
            get
            {
                switch (EtapaAutorizacaoOcorrencia)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.AprovacaoOcorrencia:
                        return "Aprovação da Ocorrência";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.EmissaoOcorrencia:
                        return "Emissão da Ocorrência";
                    default:
                        return "";
                }
            }
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
