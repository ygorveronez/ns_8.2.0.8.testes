

namespace Dominio.Entidades.Embarcador.GestaoEntregas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PORTAL_CLIENTE_PERGUNTA_AVALIACAO", EntityName = "PortalClientePerguntaAvaliacao", Name = "Dominio.Entidades.Embarcador.GestaoEntregas.PortalClientePerguntaAvaliacao", NameType = typeof(PortalClientePerguntaAvaliacao))]
    public class PortalClientePerguntaAvaliacao : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCP_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCP_TITULO", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PCP_CONTEUDO", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Conteudo { get; set; }


        public virtual string Descricao => Titulo;
    }
}
