using System;

namespace Dominio.Entidades.Embarcador.Atendimento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ATENDIMENTO_TIPO", EntityName = "AtendimentoTipo", Name = "Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo", NameType = typeof(AtendimentoTipo))]
    public class AtendimentoTipo : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTipo>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ATT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ATT_DESCRICAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ATT_STATUS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EnviarEmailAutomatico", Column = "ATT_ENVIAR_EMAIL_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EnviarEmailAutomatico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPadrao", Column = "ATT_TIPO_PADRAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool TipoPadrao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "ATT_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.AtendimentoTipo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.AtendimentoTipo Tipo { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                if (this.Status)
                    return "Ativo";
                else
                    return "Inativo";
            }
        }

        public virtual bool Equals(AtendimentoTipo other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
