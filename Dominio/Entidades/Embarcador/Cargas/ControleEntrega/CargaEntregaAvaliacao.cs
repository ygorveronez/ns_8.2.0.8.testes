namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CARGA_ENTREGA_AVALIACAO", EntityName = "CargaEntregaAvaliacao", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaAvaliacao", NameType = typeof(CargaEntregaAvaliacao))]
    public class CargaEntregaAvaliacao : EntidadeBase, Dominio.Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CEA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEA_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEA_TITULO", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEA_CONTEUDO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Conteudo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CEA_RESPOSTA", TypeType = typeof(int), NotNull = false)]
        public virtual int? Resposta { get; set; }


        public virtual string Descricao => Titulo;
    }
}
