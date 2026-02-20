using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_CONTROLE_REAJUSTE_FRETE", EntityName = "AprovacaoAlcadaControleReajusteFretePlanilha", Name = "Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaControleReajusteFretePlanilha", NameType = typeof(AprovacaoAlcadaControleReajusteFretePlanilha))]
    public class AprovacaoAlcadaControleReajusteFretePlanilha : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ControleReajusteFretePlanilha", Column = "RFP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ControleReajusteFretePlanilha ControleReajusteFretePlanilha { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraControleReajusteFretePlanilha", Column = "RRP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraControleReajusteFretePlanilha RegraControleReajusteFretePlanilha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAC_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAC_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAC_DELEGADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Delegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAC_MOTIVO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAprovadores", Column = "AAC_NUMERO_APROVADORES", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroAprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAC_SITUACAO", TypeType = typeof(SituacaoAlcadaRegra), NotNull = true)]
        public virtual SituacaoAlcadaRegra Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public virtual string Descricao
        {
            get
            {
                return Delegada ? "(Delegada)" : RegraControleReajusteFretePlanilha.Descricao;
            }
        }

        #endregion Propriedades com Regras

        #region Métodos Públicos

        public virtual bool IsPermitirAprovacaoOuReprovacao(int codigoUsuario)
        {
            return (Usuario != null) && (Usuario.Codigo == codigoUsuario) && (Situacao == SituacaoAlcadaRegra.Pendente);
        }

        #endregion Métodos Públicos
    }
}
