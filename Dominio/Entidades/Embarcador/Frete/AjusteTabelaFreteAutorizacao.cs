using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_AJUSTE_AUTORIZACAO", EntityName = "AjusteTabelaFreteAutorizacao", Name = "Dominio.Entidades.Embarcador.Ocorrencias.AjusteTabelaFreteAutorizacao", NameType = typeof(AjusteTabelaFreteAutorizacao))]
    public class AjusteTabelaFreteAutorizacao : EntidadeBase, IEquatable<AjusteTabelaFreteAutorizacao>
    {
        public AjusteTabelaFreteAutorizacao() { }
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ATA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AjusteTabelaFrete", Column = "TFA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AjusteTabelaFrete AjusteTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoTabelaFrete", Column = "RAF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasAutorizacaoTabelaFrete RegrasAutorizacaoTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ATA_BLOQUEADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Bloqueada { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoRejeicaoAjuste", Column = "MRA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoRejeicaoAjuste MotivoRejeicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ATA_SITUACAO", TypeType = typeof(SituacaoAjusteTabelaFreteAutorizacao), NotNull = true)]
        public virtual SituacaoAjusteTabelaFreteAutorizacao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ATA_ETAPA_AUTORIZACAO", TypeType = typeof(EtapaAutorizacaoTabelaFrete), NotNull = false)]
        public virtual EtapaAutorizacaoTabelaFrete EtapaAutorizacaoTabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ATA_TIPO_APROVADOR_REGRA", TypeType = typeof(TipoAprovadorRegra), NotNull = false)]
        public virtual TipoAprovadorRegra TipoAprovadorRegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivo", Column = "ATA_MOTIVO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ATA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ATA_GUID_TABELA_FRETE", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string GuidTabelaFrete { get; set; }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Aprovada:
                        return "Aprovada";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Pendente:
                        return "Pendente";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoAjusteTabelaFreteAutorizacao.Rejeitada:
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
                switch (EtapaAutorizacaoTabelaFrete)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete.AprovacaoReajuste:
                        return "Aprovação Reajuste";
                    case ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoTabelaFrete.IntegracaoReajuste:
                        return "Integração do Reajuste";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(AjusteTabelaFreteAutorizacao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }

}
