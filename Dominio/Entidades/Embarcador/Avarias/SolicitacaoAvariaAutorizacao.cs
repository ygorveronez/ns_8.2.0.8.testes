using System;

namespace Dominio.Entidades.Embarcador.Avarias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SOLICITACAO_AVARIA_AUTORIZACAO", EntityName = "SolicitacaoAvariaAutorizacao", Name = "Dominio.Entidades.Embarcador.Ocorrencias.SolicitacaoAvariaAutorizacao", NameType = typeof(SolicitacaoAvariaAutorizacao))]
    public class SolicitacaoAvariaAutorizacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>
    {
        public SolicitacaoAvariaAutorizacao() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SAA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "SolicitacaoAvaria", Column = "SAV_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvaria SolicitacaoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoAvaria", Column = "RAA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Avarias.RegrasAutorizacaoAvaria RegrasAutorizacaoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoAvaria", Column = "MAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoAvaria MotivoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_ETAPA_AUTORIZACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria EtapaAutorizacaoAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_ORIGEM_REGRA_AVARIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.OrigemRegraAvaria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.OrigemRegraAvaria OrigemRegraAvaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivo", Column = "SAV_MOTIVO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "SAV_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAprovadores", Column = "SAV_NUMERO_APROVADORES", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroAprovadores { get; set; }


        public virtual string Descricao
        {
            get
            {
                return this.RegrasAutorizacaoAvaria?.Descricao ?? "";
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Aprovada:
                        return "Aprovada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Pendente:
                        return "Pendente";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAvariaAutorizacao.Rejeitada:
                        return "Rejeitada";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoEtapaAutorizacao
        {
            get
            {
                switch (EtapaAutorizacaoAvaria)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria.Aprovacao:
                        return "Aprovação";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria.Integracao:
                        return "Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria.Lote:
                        return "Lote";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoOrigemRegraAvaria
        {
            get
            {
                switch (OrigemRegraAvaria)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.OrigemRegraAvaria.Alcada:
                        return "Alçada";
                    case ObjetosDeValor.Embarcador.Enumeradores.OrigemRegraAvaria.Delegada:
                        return "Delegada";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(SolicitacaoAvariaAutorizacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }

}
