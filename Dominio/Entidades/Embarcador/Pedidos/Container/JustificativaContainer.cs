using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_JUSTIFICATIVA_CONTAINER", EntityName = "JustificativaContainer", Name = "Dominio.Entidades.Embarcador.Pedidos.JustificativaContainer", NameType = typeof(JustificativaContainer))]
    public class JustificativaContainer : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JSC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "JSC_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "JSC_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "JSC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusContainer", Column = "JSC_STATUS_CONTAINER", TypeType = typeof(StatusColetaContainer), NotNull = false)]
        public virtual StatusColetaContainer StatusContainer { get; set; }

        public virtual string DescricaoAtivo
        {
            get
            {
                switch (this.Ativo)
                {
                    case true: return "Ativo";
                    case false: return "Inativo";
                    default: return string.Empty;
                }
            }
        }
    }
}
