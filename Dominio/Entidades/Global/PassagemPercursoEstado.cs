namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERCURSO_ESTADO_PASSAGEM", EntityName = "PassagemPercursoEstado", Name = "Dominio.Entidades.PassagemPercursoEstado", NameType = typeof(PassagemPercursoEstado))]
    public class PassagemPercursoEstado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PercursoEstado", Column = "PRC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PercursoEstado Percurso { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_PASSAGEM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado EstadoDePassagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "PEP_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }
    }
}
