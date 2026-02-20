using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COLETA_CONTAINER_JUSTIFICATIVA", EntityName = "ColetaContainerJustificativa", Name = "Dominio.Entidades.Embarcador.Pedidos.Container.ColetaContainerJustificativa", NameType = typeof(ColetaContainerJustificativa))]
    public class ColetaContainerJustificativa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ColetaContainer", Column = "CCR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ColetaContainer ColetaContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CCR_STATUS", TypeType = typeof(StatusColetaContainer), NotNull = true)]
        public virtual StatusColetaContainer Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataJustificativa", Column = "CCR_DATA_JUSTIFICATIVA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataJustificativa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "JustificativaContainer", Column = "JSC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual JustificativaContainer JustificativaContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "JSC_JUSTIFICATIVA_DESCRITIVA", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string JustificativaDescritiva { get; set; }

        public virtual string Descricao
        {
            get { return $"{JustificativaDescritiva} - {Status.ObterDescricao()}"; }
        }
    }
}