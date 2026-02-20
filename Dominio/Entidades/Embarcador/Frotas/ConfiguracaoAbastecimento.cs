using Dominio.Entidades.Embarcador.Financeiro;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frotas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ABASTECIMENTO_CONFIGURACAO", EntityName = "ConfiguracaoAbastecimento", Name = "Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento", NameType = typeof(ConfiguracaoAbastecimento))]
    public class ConfiguracaoAbastecimento : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ABC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "ABC_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoImportacaoAbastecimento", Column = "ABC_TIPO_IMPORTACAO_ABASTECIMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento TipoImportacaoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "ABC_ATIVO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Posto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.LayoutEDI LayoutEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UtilizarPrecoDaTabelaDeValoresDoFornecedor", Column = "ABC_UTILIZAR_PRECO_TABELA_VALORES_FORNECEDOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarPrecoDaTabelaDeValoresDoFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarContasAPagarParaAbastecimentoExternos", Column = "ABC_GERAR_CONTAS_PAGAR_PARA_ABASTECIMENTO_EXTERNOS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarContasAPagarParaAbastecimentoExternos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoImportarAbastecimentoDuplicado", Column = "ABC_NAO_IMPORTAR_ABASTECIMENTO_DUPLICADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoImportarAbastecimentoDuplicado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoGerarMovimentoFinanceiroFechamentoExterno", Column = "ABC_NAO_GERAR_MOVIMENTO_FINANCEIRO_FECHAMENTO_EXTERNO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoGerarMovimentoFinanceiroFechamentoExterno { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ConfiguracoesPlanilhas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ABASTECIMENTO_CONFIGURACAO_PLANILHA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ABC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConfiguracaoAbastecimentoPlanilha", Column = "ABP_CODIGO")]
        public virtual IList<Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha> ConfiguracoesPlanilhas { get; set; }

        public virtual string DescricaoTipoImportacaoAbastecimento
        {
            get
            {
                if (this.TipoImportacaoAbastecimento == ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.EDI)
                    return "EDI";
                if (this.TipoImportacaoAbastecimento == ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.PostoAmigao)
                    return "Posto Amigão/Iguaçu";
                if (this.TipoImportacaoAbastecimento == ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.Planilha)
                    return "Planilha";
                if (this.TipoImportacaoAbastecimento == ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.Interno)
                    return "Interno";
                if (this.TipoImportacaoAbastecimento == ObjetosDeValor.Embarcador.Enumeradores.TipoImportacaoAbastecimento.PostoReforco)
                    return "Posto Reforço 4";
                else
                    return "Outro";
            }
        }


        public virtual bool Equals(ConfiguracaoAbastecimento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}