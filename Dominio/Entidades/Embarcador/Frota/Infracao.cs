using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INFRACAO", EntityName = "Infracao", Name = "Dominio.Entidades.Embarcador.Frota.Infracao", NameType = typeof(Infracao))]
    public class Infracao : EntidadeBase, IEquatable<Infracao>, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INF_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INF_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Local", Column = "INF_LOCAL", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Local { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INF_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroAtuacao", Column = "INF_NUMERO_ATUACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroAtuacao { get; set; }        

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "INF_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GravarDataAssinaturaMulta", Column = "INF_GRAVAR_DATA_ASSINATURA_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? GravarDataAssinaturaMulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAssinaturaMulta", Column = "INF_DATA_ASSINATURA_MOTORISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAssinaturaMulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INF_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoInfracao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoInfracao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INF_RESPONSAVEL_PAGAMENTO_INFRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPagamentoInfracao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ResponsavelPagamentoInfracao? ResponsavelPagamentoInfracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Cidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "InfracaoTitulo", Column = "IFT_CODIGO", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual InfracaoTitulo InfracaoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoInfracao", Column = "TIN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoInfracao TipoInfracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.Titulo Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarTituloEmpresa", Column = "IFT_GERAR_TITULO_EMPRESA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarTituloEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Titulo", Column = "TIT_CODIGO_EMPRESA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.Titulo TituloEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_EMPRESA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.TipoMovimento TipoMovimentoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_TITULO_EMPRESA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente PessoaTituloEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_ORGAO_EMISSOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente OrgaoEmissor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_FUNCIONARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Funcionario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INF_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissaoInfracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INF_DATA_LIMITE_INDICACAO_CONDUTOR", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLimiteIndicacaoCondutor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INF_TIPO_OCORRENCIA_INFRACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcorrenciaInfracao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOcorrenciaInfracao TipoOcorrenciaInfracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LancarDescontoMotorista", Column = "INF_LANCAR_DESCONTO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LancarDescontoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontoComissaoMotorista", Column = "INF_DESCONTO_COMISSAO_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal DescontoComissaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO_DESCONTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Justificativa JustificativaDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReduzirPercentualComissaoMotorista", Column = "INF_REDUZIR_PERCENTUAL_COMISSAO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReduzirPercentualComissaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoComissaoMotorista", Column = "INF_PERCENTUAL_REDUCAO_COMISSAO_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoComissaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FaturadoTitulosEmpresa", Column = "INF_FATURADO_TITULOS_EMPRESA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool? FaturadoTitulosEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Historicos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INFRACAO_HISTORICO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "InfracaoHistorico", Column = "IFH_CODIGO")]
        public virtual IList<InfracaoHistorico> Historicos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Parcelas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INFRACAO_PARCELA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "InfracaoParcela", Column = "IFP_CODIGO")]
        public virtual IList<InfracaoParcela> Parcelas { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "TitulosEmpresa", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INFRACAO_TITULO_EMPRESA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "InfracaoTituloEmpresa", Column = "ITE_CODIGO")]
        public virtual IList<InfracaoTituloEmpresa> TitulosEmpresa { get; set; }

        //Dados do Sinistro
        [NHibernate.Mapping.Attributes.Property(0, Column = "INF_DATA_SINISTRO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataSinistro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INF_DATA_EMBARQUE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmbarque { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INF_NUMERO_NOTA_FISCAL", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EMITENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Emitente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaCTe CargaCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Segurado", Column = "INF_SEGURADO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Segurado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LimpezaPista", Column = "INF_LIMPEZA_PISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool LimpezaPista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Seguradora", Column = "SEA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Seguros.Seguradora Seguradora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProdutoCarga", Column = "INF_PRODUTO_CARGA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ProdutoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorNotaFiscal", Column = "INF_VALOR_NOTA_FISCAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorEstimadoPrejuizo", Column = "INF_VALOR_ESTIMADO_PREJUIZO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorEstimadoPrejuizo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INF_CLASSIFICACAO_SINISTRO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoSinistro), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoSinistro ClassificacaoSinistro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CausaSinistro", Column = "INF_CAUSA_SINISTRO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string CausaSinistro { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Anexos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INFRACAO_ANEXOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "InfracaoAnexo", Column = "ANX_CODIGO")]
        public virtual IList<InfracaoAnexo> Anexos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarLancamentoAgregadoTerceiro", Column = "INF_DESCONTAR_LANCAMENTO_AGREGADO_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarLancamentoAgregadoTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "INF_TIPO_TITULO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo TipoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerarOcorrencia", Column = "INF_GERAR_OCORRENCIA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerarOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ArquivoIntegracao", Column = "INF_ARQUIVO_INTEGRACAO", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string ArquivoIntegracao { get; set; }

        #region Propriedades Virtuais

        public virtual string Descricao
        {
            get { return Numero.ToString(); }
        }

        public virtual bool OrigemIntegracao
        {
            get { return !string.IsNullOrWhiteSpace(ArquivoIntegracao); }
        }

        public virtual bool Equals(Infracao other)
        {
            return (other.Codigo == this.Codigo);
        }

        #endregion
    }
}
