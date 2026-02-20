using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.RegraAutorizacao
{
    public abstract class AprovacaoAlcada<TEntidade, TRegra> : EntidadeBase
        where TEntidade: EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade 
        where TRegra : RegraAutorizacao
    {
        #region Propriedades Abstratas

        public abstract TEntidade OrigemAprovacao { get; set; }

        public abstract TRegra RegraAutorizacao { get; set; }

        #endregion Propriedades Abstratas

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AAL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAL_BLOQUEADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Bloqueada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAL_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAL_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCriacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAL_DELEGADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Delegada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAL_MOTIVO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAprovadores", Column = "AAL_NUMERO_APROVADORES", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroAprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAL_SITUACAO", TypeType = typeof(SituacaoAlcadaRegra), NotNull = true)]
        public virtual SituacaoAlcadaRegra Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        #endregion Propriedades Sobrescritas

        #region Propriedades com Regras

        public virtual string Descricao
        {
            get
            {
                return Delegada ? "(Delegada)" : RegraAutorizacao.Descricao;
            }
        }

        #endregion Propriedades com Regras

        #region Métodos Públicos

        public virtual bool Equals(AprovacaoAlcada<TEntidade, TRegra> other)
        {
            return other.Codigo == this.Codigo;
        }

        public virtual bool IsPermitirAprovacaoOuReprovacao(int codigoUsuario)
        {
            return !Bloqueada && (Usuario != null) && (Usuario.Codigo == codigoUsuario) && (Situacao == SituacaoAlcadaRegra.Pendente);
        }

        public virtual string ObterCorGrid()
        {
            return Bloqueada ? "#BEBEBE" : Situacao.ObterCorGrid();
        }

        public virtual string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }

        #endregion Métodos Públicos
    }
}
