using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_OCORRENCIA_AUTORIZACAO", EntityName = "CargaOcorrenciaAutorizacao", Name = "Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao", NameType = typeof(CargaOcorrenciaAutorizacao))]
    public class CargaOcorrenciaAutorizacao : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao>
    {
        public CargaOcorrenciaAutorizacao() { }

        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoOcorrencia", Column = "RAO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.RegrasAutorizacaoOcorrencia RegrasAutorizacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoRejeicaoOcorrencia", Column = "MRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoRejeicaoOcorrencia MotivoRejeicaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_BLOQUEADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Bloqueada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ETAPA_AUTORIZACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia EtapaAutorizacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_ORIGEM_REGRA_OCORRENCIA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.OrigemRegraOcorrencia), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.OrigemRegraOcorrencia OrigemRegraOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_MOTIVO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_DATA_PRAZO_APROVACAO_AUTOMATICA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrazoAprovacaoAutomatica { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_DATA_PRAZO_APROVACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrazoAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_NUMERO_APROVADORES", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroAprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_NUMERO_REPROVADORES", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroReprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_PRIORIDADE_APROVACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? PrioridadeAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_OBSERVACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CentroResultado", Column = "CRE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.CentroResultado CentroResultado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COA_GUID_OCORRENCIA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string GuidOcorrencia { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public virtual string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

        public virtual string DescricaoEtapaAutorizacaoOcorrencia
        {
            get { return EtapaAutorizacaoOcorrencia.ObterDescricao(); }
        }

        public virtual int Prioridade
        {
            get
            {
                return PrioridadeAprovacao ?? RegrasAutorizacaoOcorrencia?.PrioridadeAprovacao ?? 0;
            }
        }

        #endregion Propriedades com Regras

        #region Métodos Públicos

        public virtual bool Equals(CargaOcorrenciaAutorizacao other)
        {
            return (other.Codigo == this.Codigo);
        }

        #endregion Métodos Públicos
    }
}
