namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTROLE_NOTA_DEVOLUCAO", EntityName = "ControleNotaDevolucao", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ControleNotaDevolucao", NameType = typeof(ControleNotaDevolucao))]
    public class ControleNotaDevolucao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CND_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CND_CHAVE_NFE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string ChaveNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CND_MOTIVO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CND_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusControleNotaDevolucao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusControleNotaDevolucao Status { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Chamados.Chamado Chamado { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntregaNFeDevolucao", Column = "CED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao CargaEntregaNFeDevolucao { get; set; }

        public virtual string Descricao { get { return ChaveNFe?.ToString() ?? ""; } }
    }
}
