using System;

namespace Dominio.Entidades.Embarcador.Cargas.ControleEntrega
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_ENTREGA_NFE_DEVOLUCAO", EntityName = "CargaEntregaNFeDevolucao", Name = "Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao", NameType = typeof(CargaEntregaNFeDevolucao))]
    public class CargaEntregaNFeDevolucao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CND_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaEntrega", Column = "CEN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaEntrega CargaEntrega { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Chamado", Column = "CHA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Chamados.Chamado Chamado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CND_CHAVE_NFE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string ChaveNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CND_OBSERVACAO_MOTORISTA", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string ObservacaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CND_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string GuidArquivo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CND_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CND_SERIE", TypeType = typeof(int), NotNull = false)]
        public virtual int Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CND_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CND_VALOR_TOTAL_PRODUTOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CND_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CND_PESO_DEVOLVIDO", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PesoDevolvido { get; set; }


        public virtual string Descricao { get { return !string.IsNullOrWhiteSpace(ChaveNFe) ? ChaveNFe : (Numero + " - " + Serie); } }

    }
}
