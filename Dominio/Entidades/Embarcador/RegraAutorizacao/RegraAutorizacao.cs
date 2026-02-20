using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.RegraAutorizacao
{
    public abstract class RegraAutorizacao : EntidadeBase
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "RAT_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vigencia", Column = "RAT_VIGENCIA", TypeType = typeof(System.DateTime), NotNull = false)]
        public virtual System.DateTime? Vigencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAprovadores", Column = "RAT_NUMERO_APROVADORES", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroAprovadores { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacoes", Column = "RAT_OBSERVACOES", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrioridadeAprovacao", Column = "RAT_PRIORIDADE_APROVACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int PrioridadeAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "RAT_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        public virtual string DescricaoAtivo
        {
            get { return this.Ativo ? "Ativo" : "Inativo"; }
        }

        #endregion

        #region Propriedades Abstratas

        public abstract ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos

        public virtual bool Equals(RegraAutorizacao other)
        {
            return other.Codigo == this.Codigo;
        }

        public virtual void LimparAprovadores()
        {
            Aprovadores?.Clear();
        }

        #endregion

        #region Métodos Públicos Abstratos

        public abstract bool IsAlcadaAtiva();

        public abstract void LimparAlcadas();

        #endregion
    }
}
