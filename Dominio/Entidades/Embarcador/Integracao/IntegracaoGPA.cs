using System;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_GPA", EntityName = "IntegracaoGPA", Name = "Dominio.Entidades.Embarcador.Integracao.IntegracaoGPA", NameType = typeof(IntegracaoGPA))]
    public class IntegracaoGPA : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "GPA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GPA_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GPA_REQUISICAO", Type = "StringClob", NotNull = false)]
        public virtual string Requisicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "GPA_RESPOSTA", Type = "StringClob", NotNull = false)]
        public virtual string Resposta { get; set; }
    }
}
