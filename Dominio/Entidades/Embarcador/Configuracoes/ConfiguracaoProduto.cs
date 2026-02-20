namespace Dominio.Entidades.Embarcador.Configuracoes
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CONFIGURACAO_PRODUTO", EntityName = "ConfiguracaoProduto", Name = "Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoProduto", NameType = typeof(ConfiguracaoProduto))]
    public class ConfiguracaoProduto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LayoutEtiquetaProduto", Column = "CPR_LAYOUT_ETIQUETA_PRODUTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.LayoutEtiquetaProduto), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.LayoutEtiquetaProduto LayoutEtiquetaProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControlarEstoqueReserva", Column = "CPR_CONTROLAR_ESTOQUE_RESERVA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ControlarEstoqueReserva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RealizarValidacaoComEstoqueDePosicaoAoFecharOrdemDeServico", Column = "CPR_REALIZAR_VALIDACAO_ESTOQUE_POSICAO_FECHAR_ORDEM_DE_SERVICO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RealizarValidacaoComEstoqueDePosicaoAoFecharOrdemDeServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SalvarProdutosDaNotaFiscal", Column = "CPR_SALVA_PRODUTOS_DA_NOTA_FISCAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool SalvarProdutosDaNotaFiscal { get; set; }

        public virtual string Descricao
        {
            get { return "Configuração para produtos"; }
        }
    }
}
