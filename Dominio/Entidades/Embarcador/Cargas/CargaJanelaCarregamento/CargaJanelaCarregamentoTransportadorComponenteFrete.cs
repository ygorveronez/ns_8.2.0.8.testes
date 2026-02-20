using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_JANELA_CARREGAMENTO_TRANSPORTADOR_COMPONENTE_FRETE", EntityName = "CargaJanelaCarregamentoTransportadorComponenteFrete", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponenteFrete", NameType = typeof(CargaJanelaCarregamentoTransportadorComponenteFrete))]
    public class CargaJanelaCarregamentoTransportadorComponenteFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoTransportador", Column = "JCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaJanelaCarregamentoTransportador CargaJanelaCarregamentoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComponente", Column = "TCF_VALOR_COMPONENTE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = true)]
        public virtual decimal ValorComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Percentual", Column = "TCF_PERCENTUAL_NOTA_FISCAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = true)]
        public virtual decimal Percentual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoValor", Column = "TCF_TIPO_VALOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCF_MOEDA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCF_VALOR_COTACAO_MOEDA", TypeType = typeof(decimal), Scale = 10, Precision = 21, NotNull = false)]
        public virtual decimal ValorCotacaoMoeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCF_VALOR_TOTAL_MOEDA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotalMoeda { get; set; }

        public virtual string DescricaoComponente
        {
            get { return (ComponenteFrete == null) ? "" : ComponenteFrete.TipoComponenteFrete.ObterDescricao(); }
        }
    }
}
