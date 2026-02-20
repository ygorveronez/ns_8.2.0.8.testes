using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Escrituracao
{
    public class Provisao
    {
        #region Atributos

        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro _configuracaoFinanceiro;

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Repositorio.Embarcador.Escrituracao.CancelamentoProvisao _repositorioCancelamentoProvisao;
        private Repositorio.Embarcador.Ocorrencias.CargaOcorrencia _repositorioCargaOcorrencia;
        private Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao _repositorioProvisaoIntegracao;
        private Repositorio.Embarcador.Financeiro.DocumentoFaturamento _repositorioDocumentoFaturamento;
        private Repositorio.Embarcador.Escrituracao.PagamentoIntegracao _repositorioPagamentoIntegracao;
        private Repositorio.Embarcador.Escrituracao.DocumentoProvisao _repositorioDocumentoProvisao;
        private Repositorio.Embarcador.Escrituracao.Provisao _repositorioProvisao;

        #endregion Atributos

        #region Construtores

        public Provisao(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null, configuracaoFinanceiro: null) { }

        public Provisao(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, configuracaoFinanceiro: null) { }

        public Provisao(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
            _configuracaoFinanceiro = configuracaoFinanceiro;
        }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> LayoutEDIProvisao(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);

            List<Dominio.Entidades.Cliente> tomadores = repDocumentoProvisao.BuscarTomadoresProvisao(provisao.Codigo);

            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas> grupoPessoas = (from tomador in tomadores where tomador.GrupoPessoas != null select tomador.GrupoPessoas).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> layouts = new List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI>();
            for (int i = 0, s = grupoPessoas.Count(); i < s; i++)
                if (grupoPessoas[i].LayoutsEDI != null)
                    layouts.AddRange(grupoPessoas[i].LayoutsEDI.ToList());

            return layouts;
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> LayoutEDIProvisaoCliente(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);

            List<Dominio.Entidades.Cliente> tomadores = repDocumentoProvisao.BuscarTomadoresProvisao(provisao.Codigo);

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> layouts = new List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI>();

            for (int i = 0, s = tomadores.Count(); i < s; i++)
                if (tomadores[i].LayoutsEDI != null)
                    layouts.AddRange(tomadores[i].LayoutsEDI.ToList());

            return layouts;
        }

        public DateTime ObterDataLancamento()
        {
            Dominio.Entidades.Embarcador.Escrituracao.ConfiguracaoProvisao configuracaoProvisao = ObterConfiguracaoProvisao();
            DateTime dataAtual = DateTime.Now.Date;
            DateTime ultimoDiaMesAnterior = new DateTime(dataAtual.Year, dataAtual.Month, day: 1).AddDays(-1);

            DateTime dataFormaMes = dataAtual.AddDays(-configuracaoProvisao?.DiasForaMes ?? 0);

            if (dataFormaMes <= ultimoDiaMesAnterior)
                return ultimoDiaMesAnterior;

            return dataAtual;
        }

        public void ProcessarProvisoesEmFechamento()
        {
            try
            {
                Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(_unitOfWork);
                Servicos.Embarcador.Contabeis.ImpostoValorAgregado servicoImpostoValorAgregado = new Servicos.Embarcador.Contabeis.ImpostoValorAgregado(_unitOfWork);

                List<Dominio.Entidades.Embarcador.Escrituracao.Provisao> provisoes = repositorioProvisao.BuscarProvisaoEmFechamento();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork).BuscarPrimeiroRegistro();

                foreach (Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao in provisoes)
                {
                    servicoImpostoValorAgregado.DefinirImpostoValorAgregadoPorStage(provisao);
                    GerarFechamentoDocumentosProvisao(provisao);
                    new Servicos.Embarcador.Escrituracao.RateioProvisaoProduto(_unitOfWork, _auditado).ReatearPorGrupoDeProduto(provisao);

                    if (configuracaoFinanceiro.UtilizarFechamentoAutomaticoProvisao ?? false)
                        GerarFechamentoProvisao(provisao, "Solicitou o fechamento automático da provisão");
                }
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Log.TratarErro(excecao);
            }
        }

        public void ReprocessarProvisaoPorStage(int codigoProvisao, int codigoStage, int codigoImpostoValorAgregado)
        {
            Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repositorioDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(_unitOfWork);
            Repositorio.Embarcador.Contabeis.ImpostoValorAgregado repositorioImpostoValorAgregado = new Repositorio.Embarcador.Contabeis.ImpostoValorAgregado(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Stage repositorioStage = new Repositorio.Embarcador.Pedidos.Stage(_unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = repositorioProvisao.BuscarPorCodigo(codigoProvisao);
            Dominio.Entidades.Embarcador.Pedidos.Stage stage = (codigoStage > 0) ? repositorioStage.BuscarPorCodigo(codigoStage, auditavel: false) : null;
            Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado impostoValorAgregado = (codigoImpostoValorAgregado > 0) ? repositorioImpostoValorAgregado.BuscarPorCodigo(codigoImpostoValorAgregado) : null;

            repositorioDocumentoProvisao.SetarDocumentosRefazerProvisaoPorStage(provisao.Codigo, stage?.Codigo ?? 0, impostoValorAgregado?.Codigo ?? 0);
            repositorioDocumentoContabil.ExcluirTodosPorProvisaoEStage(provisao.Codigo, stage?.Codigo ?? 0);

            GerarFechamentoDocumentosProvisao(provisao);

            Auditoria.Auditoria.Auditar(_auditado, provisao, $"{((impostoValorAgregado == null) ? "Removido o IVA dos" : $"Alterado o IVA para {impostoValorAgregado.CodigoIVA} nos")} documentos {(string.IsNullOrWhiteSpace(stage?.NumeroStage) ? "sem stage" : $" da stage {stage.NumeroStage}")}", _unitOfWork);
        }

        public void FecharProvisoesAutomaticamente()
        {
            //Fechamento automatico para unilever. Feita a validação pelo tipo para identificação;
            Repositorio.Embarcador.Cargas.TipoIntegracao repostirioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            if (!repostirioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever))
                return;

            Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Escrituracao.Provisao> provisoes = repositorioProvisao.BuscarProvisoesEmFechamentoPendentes(registrosPorVez: 100, minutosCadaTentaivas: 5);

            foreach (var provisao in provisoes)
                GerarFechamentoProvisao(provisao, "Fechamento da provisão gerado automaticamente");
        }

        public string GerarFechamentoProvisaoIndividual(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao)
        {
            return GerarFechamentoProvisao(provisao, "Solicitou o fechamento da provisão");
        }

        public void GerarLotesProvisao()
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = ObterConfiguracaoFinanceiro();

            if (!configuracaoFinanceiro.AutomatizarGeracaoLoteProvisao)
                return;

            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);

            List<(int CodigoCarga, bool CargaTrechoDefinida)> listaDadosCarga = repositorioDocumentoProvisao.BuscarDadosCargasPorDocumentosAguardandoProvisao(configuracaoFinanceiro.GerarLotesProvisaoAposEmissaoDaCarga, configuracaoFinanceiro.NaoGerarLoteProvisaoParaOcorrencia, configuracaoFinanceiro.NaoGerarLoteProvisaoParaCargaAguardandoImportarCTeOuLancarNFS);

            foreach ((int CodigoCarga, bool CargaTrechoDefinida) dadosCarga in listaDadosCarga)
            {
                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisao = repositorioDocumentoProvisao.BuscarPorCarga(dadosCarga.CodigoCarga, configuracaoFinanceiro.NaoGerarLoteProvisaoParaOcorrencia);

                if (dadosCarga.CargaTrechoDefinida)
                {
                    List<int> codigosXmlNotaFiscalComCTeConfirmado = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarCodigosXmlNotaFiscalComCTeConfirmadoPorCarga(dadosCarga.CodigoCarga);

                    documentosProvisao = documentosProvisao.Where(documento => codigosXmlNotaFiscalComCTeConfirmado.Contains(documento.XMLNotaFiscal?.Codigo ?? 0)).ToList();
                }

                if (documentosProvisao.Count == 0)
                    continue;

                Dominio.Entidades.Cliente tomador = documentosProvisao.FirstOrDefault()?.Tomador;

                if ((tomador == null) || !(tomador.GrupoPessoas?.GerarSomenteUmaProvisaoCadaCargaCompleta ?? false))
                    continue;

                if (!(configuracaoFinanceiro.GerarLoteProvisaoIndividualNfe ?? false))
                    GerarProvisao(dadosCarga.CodigoCarga, documentosProvisao);
                else
                {
                    foreach (var documentoProvisao in documentosProvisao)
                    {
                        GerarProvisao(dadosCarga.CodigoCarga, new List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao>() { documentoProvisao }, false);
                    }
                }
            }
        }

        public void VerificarProvisoesPendentesAprovacaoTermo(Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro termoQuitacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = ObterConfiguracaoFinanceiro();

            if (termoQuitacao.SituacaoAprovacaoTransportador != SituacaoAprovacaoTermoQuitacaoTransportador.Aprovado && !termoQuitacao.DataInicial.HasValue && !termoQuitacao.DataFinal.HasValue)
                return;

            Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repCancelamentoeProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.Provisao repProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(_unitOfWork);

            Servicos.Embarcador.Escrituracao.EstornoProvisaoAprovacao servEstornoProvisaoAprovacao = new EstornoProvisaoAprovacao(_unitOfWork);

            IList<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ProvisaoAbertaParaEstorno> listaDadosProvisaoParaEstorno = repDocumentoProvisao.BuscarDadosProvisaoAbertaParaEstornoDeTermoQuitacao(termoQuitacao.DataInicial.Value, termoQuitacao.DataFinal.Value, termoQuitacao.Transportador);

            List<int> codigosProvisoes = listaDadosProvisaoParaEstorno.Select(x => x.CodigoProvisao).Distinct().ToList();

            foreach (int codigoProvisao in codigosProvisoes)
            {
                List<int> codigoDocumentoProvisao = listaDadosProvisaoParaEstorno.Where(x => x.CodigoProvisao == codigoProvisao).Select(obj => obj.CodigoDocumentoProvisao).ToList();

                Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = repProvisao.BuscarPorCodigo(codigoProvisao, false);

                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao();

                cancelamentoProvisao.CancelamentoProvisaoContraPartida = false;
                cancelamentoProvisao.DataInicial = provisao.DataInicial;
                cancelamentoProvisao.DataFinal = provisao.DataFinal;
                cancelamentoProvisao.DataCriacao = DateTime.Now;
                cancelamentoProvisao.Numero = repProvisao.ObterProximoNumero();

                if (provisao.Tomador != null)
                    cancelamentoProvisao.Tomador = provisao.Tomador;

                cancelamentoProvisao.Filial = provisao.Filial;
                cancelamentoProvisao.Empresa = provisao.Transportadores.FirstOrDefault();

                if (provisao.Carga != null)
                    cancelamentoProvisao.Carga = provisao.Carga;

                if (provisao.CargaOcorrencia != null)
                    cancelamentoProvisao.CargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(provisao.CargaOcorrencia.Codigo);

                cancelamentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.EmCancelamento;
                repCancelamentoeProvisao.Inserir(cancelamentoProvisao);

                Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentosParaCancelamento(cancelamentoProvisao, codigoDocumentoProvisao, _unitOfWork);
                servEstornoProvisaoAprovacao.CriarAprovacao(cancelamentoProvisao, TipoGeracaoRegraProvisao.TermoQuitacao, tipoServicoMultisoftware);
            }
        }

        public void VerificarProvisoesAbertasGeracaoEstornoAutomatico(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = ObterConfiguracaoFinanceiro();

            if (configuracaoFinanceiro.QuantidadeDiasAbertoEstornoProvisao <= 0)
                return;

            Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repCancelamentoeProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.Provisao repProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(_unitOfWork);

            Servicos.Embarcador.Escrituracao.EstornoProvisaoAprovacao servEstornoProvisaoAprovacao = new EstornoProvisaoAprovacao(_unitOfWork);

            IList<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ProvisaoAbertaParaEstorno> listaDadosProvisaoParaEstorno = repDocumentoProvisao.BuscarDadosProvisaoAbertaParaEstorno(configuracaoFinanceiro.QuantidadeDiasAbertoEstornoProvisao);

            List<int> codigosProvisoes = listaDadosProvisaoParaEstorno.Select(x => x.CodigoProvisao).Distinct().ToList();

            foreach (int codigoProvisao in codigosProvisoes)
            {
                List<int> codigoDocumentoProvisao = listaDadosProvisaoParaEstorno.Where(x => x.CodigoProvisao == codigoProvisao).Select(obj => obj.CodigoDocumentoProvisao).ToList();

                Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = repProvisao.BuscarPorCodigo(codigoProvisao, false);

                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao();

                cancelamentoProvisao.CancelamentoProvisaoContraPartida = false;
                cancelamentoProvisao.DataInicial = provisao.DataInicial;
                cancelamentoProvisao.DataFinal = provisao.DataFinal;
                cancelamentoProvisao.DataCriacao = DateTime.Now;
                cancelamentoProvisao.Numero = repProvisao.ObterProximoNumero();

                if (provisao.Tomador != null)
                    cancelamentoProvisao.Tomador = provisao.Tomador;

                cancelamentoProvisao.Filial = provisao.Filial;
                cancelamentoProvisao.Empresa = provisao.Transportadores.FirstOrDefault();

                if (provisao.Carga != null)
                    cancelamentoProvisao.Carga = provisao.Carga;

                if (cancelamentoProvisao.Filial == null && cancelamentoProvisao.Carga != null)
                    cancelamentoProvisao.Filial = cancelamentoProvisao.Carga.Filial;

                if (cancelamentoProvisao.Empresa == null && cancelamentoProvisao.Carga != null)
                    cancelamentoProvisao.Empresa = cancelamentoProvisao.Carga.Empresa;

                if (provisao.CargaOcorrencia != null)
                    cancelamentoProvisao.CargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(provisao.CargaOcorrencia.Codigo);

                cancelamentoProvisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.EmCancelamento;

                if (repTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever))
                    cancelamentoProvisao.Situacao = SituacaoCancelamentoProvisao.AgAprovacaoSolicitacao;

                repCancelamentoeProvisao.Inserir(cancelamentoProvisao);

                Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentosParaCancelamento(cancelamentoProvisao, codigoDocumentoProvisao, _unitOfWork);
                servEstornoProvisaoAprovacao.CriarAprovacao(cancelamentoProvisao, TipoGeracaoRegraProvisao.PrazoExcedido, tipoServicoMultisoftware);
            }
        }

        public void RemoverDocumentosCanceladosDaProvisao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repositorioDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisao;

            if (cargaCancelamento.TipoCancelamentoCargaDocumento == TipoCancelamentoCargaDocumento.Documentos)
                documentosProvisao = repositorioDocumentoProvisao.BuscarPorCargaECTe(carga.Codigo, cargaCancelamento.CTe.Codigo);
            else
                documentosProvisao = repositorioDocumentoProvisao.BuscarComNotaFiscalPorCarga(carga.Codigo);

            List<Dominio.Entidades.Embarcador.Escrituracao.Provisao> provisoes = documentosProvisao.Select(documento => documento.Provisao).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao in provisoes)
            {
                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisaoPorProvisao = documentosProvisao.Where(documento => documento.Provisao?.Codigo == provisao?.Codigo).ToList();

                foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao in documentosProvisaoPorProvisao)
                {

                    if (provisao == null)
                    {
                        repositorioDocumentoProvisao.Deletar(documentoProvisao);
                        continue;
                    }

                    provisao.QuantidadeDocsProvisao -= 1;
                    if ((provisao.TipoProvisao == TipoProvisao.ProvisaoPorCTe) && (documentoProvisao.CTe != null))
                    {
                        if (documentoProvisao.CTe.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == TipoDocumentoCreditoDebito.Debito)
                            provisao.ValorProvisao += documentoProvisao.ObterValorLiquido();
                        else
                            provisao.ValorProvisao -= documentoProvisao.ObterValorLiquido();
                    }
                    else
                        provisao.ValorProvisao -= documentoProvisao.ValorProvisao;

                    repositorioDocumentoContabil.ExcluirTodosPorDocumentoProvisao(documentoProvisao.Codigo);
                    repositorioDocumentoProvisao.Deletar(documentoProvisao);
                }

                if (provisao != null)
                    repositorioProvisao.Atualizar(provisao);
            }
        }

        public void ConfirmarEstornoProvisao(Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.EstornoProvisaoRequest request, Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao integracaoCancelamentoProvisao)
        {
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao repositorioCancelamentoProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisaoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repositorioCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(_unitOfWork);

            if (integracaoCancelamentoProvisao == null)
                throw new ServicoException("Registro não encontrado");

            if (integracaoCancelamentoProvisao.SituacaoIntegracao != SituacaoIntegracao.AgRetorno)
                throw new ServicoException("Registro não está aguardando retorno.");

            if (integracaoCancelamentoProvisao.DocumentoProvisao == null)
                throw new ServicoException("Documento da provisão ainda não gerado para a confirmação do estorno.");

            if (request.ProcessadoSucesso)
            {
                integracaoCancelamentoProvisao.CancelamentoProvisao.Situacao = SituacaoCancelamentoProvisao.Estornado;
                integracaoCancelamentoProvisao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
            else
            {
                integracaoCancelamentoProvisao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoCancelamentoProvisao.CancelamentoProvisao.Situacao = SituacaoCancelamentoProvisao.FalhaIntegracao;
            }

            integracaoCancelamentoProvisao.DataIntegracao = DateTime.Now;
            integracaoCancelamentoProvisao.ProblemaIntegracao = request.MensagemRetorno;

            repositorioCancelamentoProvisaoIntegracao.Atualizar(integracaoCancelamentoProvisao);
            repositorioCancelamentoProvisao.Atualizar(integracaoCancelamentoProvisao.CancelamentoProvisao);
        }

        public void GerarEstornoProvisaoAposLiberacaoPagamento(Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao pagamentoIntegracao)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = ObterConfiguracaoFinanceiro();
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(_unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = pagamentoIntegracao.Pagamento;

            if (!(configuracaoFinanceiro.GerarEstornoProvisaoAutomaticoAposLiberacaoPagamento ?? false))
                return;

            if (pagamento.DocumentosFaturamento == null)
                return;

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento in pagamento.DocumentosFaturamento)
            {
                if (documentoFaturamento.CargaOcorrenciaPagamento != null)
                    continue;

                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisao = ObterDocumentosProvisaoPorDocumentoFaturamento(pagamento, documentoFaturamento, ObterRepositorioDocumentoProvisao());

                foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao in documentosProvisao)
                {
                    Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao = documentoProvisao.CancelamentoProvisao;

                    if (cancelamentoProvisao == null)
                    {
                        cancelamentoProvisao = GerarCancelamentoProvisaoIndividual(documentoProvisao, (configuracaoFinanceiro.UtilizarEstornoProvisaoDeFormaAutomatizada ?? false), gerarIntegracoes: true);
                        documentoProvisao.CancelamentoProvisao = cancelamentoProvisao;
                    }

                    List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = ObterCanhotosPorDocumentoProvisao(documentoProvisao.Carga?.Codigo, documentoProvisao.XMLNotaFiscal?.Codigo ?? 0, repositorioCanhoto);
                    GerarDesbloqueioTituloGeradoPorCanhotos(canhotos, repositorioCanhoto, documentoFaturamento);

                    if (cancelamentoProvisao.Situacao == SituacaoCancelamentoProvisao.Estornado)
                        continue;

                    documentoProvisao.Cancelado = true;

                    ObterRepositorioDocumentoProvisao().Atualizar(documentoProvisao);
                    ObterRepositorioProvisao().Atualizar(documentoProvisao.Provisao);
                }
            }
        }

        private void GerarDesbloqueioTituloGeradoPorCanhotos(List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos, Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto, Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento)
        {
            if (canhotos.Count == 0)
                Servicos.Embarcador.Escrituracao.Pagamento.GerarEstornoPagamentoPorDFAs(new List<int> { documentoFaturamento.Codigo }, _unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto in canhotos)
            {
                if (canhoto != null)
                {
                    canhoto.SituacaoGeracaoCancelamentoAutomatico = SituacaoGeracaoCancelamentoAutomatico.PendenteGerarDesbloqueioTitulo;
                    repositorioCanhoto.Atualizar(canhoto);
                }
            }
        }

        public static List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> ObterCanhotosPorDocumentoProvisao(int? codigoCarga, int codigoNotaFiscal, Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto)
        {
            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = new List<Dominio.Entidades.Embarcador.Canhotos.Canhoto>();

            if (codigoCarga.HasValue)
            {
                Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = repositorioCanhoto.BuscarPorCargaENF(codigoCarga.Value, codigoNotaFiscal);

                if (canhoto != null)
                    canhotos.Add(canhoto);
            }

            if (canhotos.Count == 0)
                canhotos = repositorioCanhoto.BuscarCanhotosPorNFCanhotoAvulso(codigoNotaFiscal);

            return canhotos;
        }

        public static List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> ObterDocumentosProvisaoPorDocumentoFaturamento(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento, Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao)
        {
            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisao = repositorioDocumentoProvisao
                                .BuscarPorDocumentoFaturamentoEPagamento(documentoFaturamento.Codigo, pagamento.Codigo)
                                .Where(documentoProvisao => documentoProvisao.Provisao != null)
                                .ToList();

            if (documentosProvisao.Count == 0 && documentoFaturamento.CTe != null && documentoFaturamento.CTe.XMLNotaFiscais != null)
            {
                int codigoCarga = documentoFaturamento.Carga?.Codigo
                    ?? documentoFaturamento.CTe?.CargaCTes?.FirstOrDefault()?.Carga?.Codigo
                    ?? 0;

                foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotafiscal in documentoFaturamento.CTe.XMLNotaFiscais)
                {
                    Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao = repositorioDocumentoProvisao
                        .BuscarPorCargaENota(codigoCarga, xmlNotafiscal.Codigo);

                    if (documentoProvisao != null && documentoProvisao.Provisao != null)
                        documentosProvisao.Add(documentoProvisao);
                }
            }

            return documentosProvisao;
        }

        public void GerarExtornoProvisaoAposEscrituracao(List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoEscrituracao> documentoEscrituracaos)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = ObterConfiguracaoFinanceiro();

            if (!(configuracaoFinanceiro.GerarEstornoProvisaoAutomaticoAposEscrituracao ?? false))
                return;

            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisao = repDocumentoProvisao.BuscarDocumentoProvisaoPorDocumentoEscituracao(documentoEscrituracaos);

            foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao in documentosProvisao)
                GerarCancelamentoProvisaoIndividual(documentoProvisao);
        }

        public bool EhPrimeiroOuSegundoDiaUtilMes()
        {
            DateTime primeiroDiaUtil = ObterDiaUtil(DateTime.Now.FirstDayOfMonth());

            DateTime segundoDiaUtil = ObterDiaUtil(primeiroDiaUtil.AddDays(1));

            return primeiroDiaUtil.Day == DateTime.Now.Day || segundoDiaUtil.Day == DateTime.Now.Day;
        }

        public DateTime ObterDiaUtil(DateTime dia)
        {
            Servicos.Embarcador.Configuracoes.Feriado servicoFeriado = new Servicos.Embarcador.Configuracoes.Feriado(_unitOfWork);

            while (dia.DayOfWeek == DayOfWeek.Saturday || dia.DayOfWeek == DayOfWeek.Sunday || servicoFeriado.VerificarSePossuiFeriado(dia))
            {
                dia = dia.AddDays(dia.DayOfWeek == DayOfWeek.Saturday ? 2 : 1);
            }

            return dia;
        }

        public List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.CargaProvisao> BuscarDadosProvisao(Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.RequisicaoBuscarDadosProvisao request)
        {
            List<string> objetosFetchCarga = new List<string>() { "CargaCancelamento", "Filial", "Empresa", "TabelaFrete", "TipoDeCarga", "TipoOperacao", "Carregamento", "CargaAgrupamento", "ModeloVeicularCarga", "Veiculo", "Rota" };
            List<string> objetosFetchProvisao = new List<string>() { "Pagamento", "Remetente", "Destinatario", "Tomador", "Origem", "Destino", "Recebedor", "CTe", "CargaOcorrencia", "Canhoto" };
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new();
            List<Dominio.Entidades.Embarcador.Escrituracao.Provisao> lotesProvisao = new();
            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamento> cancelamentos = new();
            List<(int CodigoCarga, Dominio.Entidades.Embarcador.Cargas.CargaDadosSumarizados DadosSumarizados)> dadosSumarizados = new();
            List<(int codigoDocumentoProvisao, decimal valorContabilizacao)> totaisAReceber = new();

            List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.CargaProvisao> cargasProvisao = new();

            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new(_unitOfWork);
                Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new(_unitOfWork);

                if (request.LimiteRegistros - request.InicioRegistros > 50)
                {
                    throw new ServicoException("O limite de registros não deve ser superior a 50.");
                }

                if (request.ProtocoloIntegracaoCarga > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigoComFetch(request.ProtocoloIntegracaoCarga, objetosFetchCarga);

                    if (carga != null)
                        cargas.Add(carga);    
                }
                else if (request.DataEmissaoProvisaoInicial != null && request.DataEmissaoProvisaoFinal != null)
                {
                    ValidarPeriodo(request.DataEmissaoProvisaoInicial, request.DataEmissaoProvisaoFinal);

                    cargas = repositorioCarga.ConsultarCargasComProvisaoPorPeriodo(
                        request.DataEmissaoProvisaoInicial.Value,
                        request.DataEmissaoProvisaoFinal.Value,
                        objetosFetchCarga,
                        request.InicioRegistros,
                        request.LimiteRegistros
                    );

                    if (cargas.IsNullOrEmpty())
                    {
                        throw new ServicoException("Não existem provisões para esse intervalo de tempo.");
                    }
                }
                else if (request.DataCriacaoCargaInicial != null && request.DataCriacaoCargaFinal != null)
                {
                    ValidarPeriodo(request.DataCriacaoCargaInicial, request.DataCriacaoCargaFinal);

                    cargas = repositorioCarga.ConsultarCargasComProvisaoPorPeriodo(
                        request.DataCriacaoCargaInicial.Value,
                        request.DataCriacaoCargaFinal.Value,
                        objetosFetchCarga,
                        request.InicioRegistros,
                        request.LimiteRegistros,
                        true
                    );

                    if (cargas.IsNullOrEmpty())
                    {
                        throw new ServicoException("Não existem provisões para esse intervalo de tempo.");
                    }
                }
                else
                {
                    throw new ServicoException("Parâmetros inválidos");
                }

                List<int> codigoscarga = cargas.Select(carga => carga.Codigo).ToList();

                lotesProvisao = repositorioProvisao.BuscarTodosPorCargas(codigoscarga);
                List<int> codigosProvisao = lotesProvisao.Select(provisao => provisao.Codigo).ToList();

                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new(_unitOfWork);

                List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisoes = repositorioDocumentoProvisao.BuscarPorProvisoesComFetch(codigosProvisao, objetosFetchProvisao);
                List<int> codigosDocumentosProvisao = documentosProvisoes.Select(documento => documento.Codigo).ToList();

                Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new(_unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDadosSumarizados repositorioCargaDadosSumarizadsos = new(_unitOfWork);
                Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repositorioDocumentoContabil = new(_unitOfWork);

                cancelamentos = repositorioCargaCancelamento.BuscarPorCargas(codigoscarga);
                dadosSumarizados = repositorioCargaDadosSumarizadsos.BuscarPorCargas(codigoscarga);
                totaisAReceber = repositorioDocumentoContabil.BuscarTotalReceberPorProvisoes(codigosDocumentosProvisao);
                decimal totalAReceber = 0;

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.CargaProvisao cargaProvisao = new()
                    {
                        NumeroCarga = carga.CodigoCargaEmbarcador,
                        SituacaoCarga = carga.SituacaoCarga.ObterDescricao(),
                        Filial = carga.Filial?.Descricao,
                        CnpjTransportador = carga.Empresa?.CNPJ,
                        CodigoIntegracaoTransportador = carga.Empresa?.CodigoIntegracao,
                        Transportador = carga.Empresa?.RazaoSocial,
                        TabelaFrete = carga.TabelaFrete?.Descricao,
                        TipoCarga = carga.TipoDeCarga?.Descricao,
                        TipoOperacao = carga.TipoOperacao?.Descricao,
                        CargaAgrupamento = carga.CargaAgrupamento?.Descricao,
                        DataCriacaoCarga = carga.DataCriacaoCarga,
                        DataCriacaoCargaAgrupada = carga.CargaAgrupamento?.DataCriacaoCarga ?? null,
                        Carregamento = carga.Carregamento?.Descricao,
                        ModeloVeicular = carga.ModeloVeicularCarga?.Descricao,
                        Placa = carga.Veiculo?.Placa,
                        Rota = carga.Rota?.Descricao,
                        LotesProvisao = new List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.LoteProvisao>(),
                        CancelamentosCarga = new List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.CancelamentoCarga>(),
                    };

                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga> situacoesCancelamentos = cancelamentos?.Where(cancelamento => cancelamento?.Codigo == carga.CargaCancelamento?.Codigo)?
                        .Select(cancelamento => cancelamento.Situacao)?.ToList() ?? null;

                    if (!situacoesCancelamentos.IsNullOrEmpty())
                    {
                        foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga situacaocancelamento in situacoesCancelamentos)
                        {
                            Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.CancelamentoCarga cancelamentoCarga = new()
                            {
                                NumeroSituacao = (int)situacaocancelamento,
                                DescricaoSituacao = situacaocancelamento.Descricao(),
                            };

                            cargaProvisao.CancelamentosCarga.Add(cancelamentoCarga);
                        }
                    }

                    List<Dominio.Entidades.Embarcador.Escrituracao.Provisao> lotesProvisaoDaCargaProvisao = lotesProvisao.Where(provisao => provisao.Carga.Codigo == carga.Codigo).ToList();

                    foreach (Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao in lotesProvisaoDaCargaProvisao)
                    {
                        Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.LoteProvisao loteProvisao = new()
                        {
                            Provisao = provisao.Numero,
                            TipoProvisao = provisao.TipoProvisao.ObterDescricao(),
                            SituacaoPagamento = provisao.DocumentosProvisao?.First().Pagamento?.Situacao.ObterDescricao(),
                            SituacaoProvisao = provisao.DescricaoSituacao,
                            CancelamentoProvisao = provisao.DocumentosProvisao?.First().Cancelado ?? false,
                            SituacaoCancelamentoProvisao = provisao.DocumentosProvisao?.First().CancelamentoProvisao?.Situacao.ObterDescricao(),
                            Documentos = new List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.DocumentoProvisao>()
                        };

                        List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisoesDoLoteProvisao = documentosProvisoes.Where(documento => documento.Provisao.Codigo == provisao.Codigo).ToList();
                        bool? reentrega = dadosSumarizados.Where(dadosSumarizados => dadosSumarizados.CodigoCarga == carga.Codigo)?.Select(dadosSumarizados => dadosSumarizados.DadosSumarizados.Reentrega).FirstOrDefault();

                        foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documento in documentosProvisoesDoLoteProvisao)
                        {
                            Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.DadosProvisao.DocumentoProvisao documentoProvisao = new()
                            {
                                NumeroDocumento = documento.NumeroDocumento,
                                DataEmissao = documento.DataEmissao.Date,
                                CpfCnpjRemetente = documento.Remetente?.CPF_CNPJ.ToString(),
                                Remetente = documento.Remetente?.Nome,
                                CpfCnpjDestinatario = documento.Destinatario?.CPF_CNPJ.ToString(),
                                Origem = documento.Origem?.DescricaoCidadeEstado,
                                Destino = documento.Destino?.DescricaoCidadeEstado,
                                CpfCnpjTomador = documento.Tomador?.CPF_CNPJ_Formatado.ToString(),
                                Tomador = documento.Tomador?.Nome,
                                CpfCnpjExpedidor = documento.Expedidor?.CPF_CNPJ.ToString(),
                                Expedidor = documento.Expedidor?.Nome,
                                CpfCnpjRecebedor = documento.Recebedor?.CPF_CNPJ.ToString(),
                                Recebedor = documento.Recebedor?.Nome,
                                Reentrega = reentrega,
                                NumeroCTe = documento.CTe?.Numero ?? 0,
                                SerieCTe = documento.CTe?.Serie?.Descricao,
                                DataEmissaoCte = documento.CTe?.DataEmissao?.Date,
                                DataEmissaoNfsManual = documento.LancamentoNFSManual?.DataCriacao.Date,
                                Ocorrencia = documento.CargaOcorrencia?.NumeroOcorrencia ?? 0,
                                TipoOcorrencia = documento.CargaOcorrencia?.TipoOcorrencia?.Descricao,
                                DataEmissaoOcorrencia = documento.CargaOcorrencia?.DataOcorrencia.Date,
                                Cst = documento.CST,
                                TipoCanhoto = documento.XMLNotaFiscal?.Canhoto?.TipoCanhoto.ObterDescricao(),
                                CanhotoSerie = documento.XMLNotaFiscal?.Canhoto?.Numero ?? 0,
                                Situacao = documento.XMLNotaFiscal?.Canhoto?.SituacaoDigitalizacaoCanhoto.ObterDescricao(),
                                PesoBruto = documento.XMLNotaFiscal?.Peso ?? 0,
                                ValorFrete = documento.ValorFrete,
                                Icms = documento.ValorICMS,
                                ValorIss = documento.ValorISS,
                                ValorIssRetido = documento.ValorRetencaoISS,
                                Aliquota = documento.PercentualAliquota,
                                AliquotaIss = documento.PercentualAliquotaISS,
                                AliquotaPis = documento.Empresa?.EmpresaPai?.Configuracao?.AliquotaPIS ?? 0,
                                AliquotaCofins = documento.Empresa?.EmpresaPai?.Configuracao?.AliquotaCOFINS ?? 0,
                                ValorProvisao = documento.ValorProvisao,
                                IcmsInclusoNaBc = documento.ICMSInclusoBC,
                                IssInclusoNaBc = documento.ISSInclusoBC,
                                TaxaDescarga = documento.ValorDescarga,
                                TotalReceber = totaisAReceber.Where(total => total.codigoDocumentoProvisao == documento.Codigo)?.Select(total => total.valorContabilizacao).FirstOrDefault(),
                            };

                            loteProvisao.Documentos.Add(documentoProvisao);
                        }
                        cargaProvisao.LotesProvisao.Add(loteProvisao);
                    }
                    cargasProvisao.Add(cargaProvisao);
                }

                if (cargas == null)
                    throw new ServicoException("Nenhum parâmetro enviado.");

                return cargasProvisao;
            }

            catch (Exception e)
            {
                Servicos.Log.TratarErro(e.Message);
                throw;
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao GerarCancelamentoProvisaoIndividual(Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao, bool situacaoEmCancelamento = true, bool gerarIntegracoes = false)
        {
            Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = documentoProvisao.Provisao;
            Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao
            {
                CancelamentoProvisaoContraPartida = false,
                DataInicial = provisao.DataInicial,
                DataFinal = provisao.DataFinal,
                DataCriacao = DateTime.Now,
                Numero = ObterRepositorioCancelamentoProvisao().ObterProximoNumero(),
                Tomador = provisao.Tomador,
                Filial = provisao.Filial,
                Empresa = provisao.Transportadores.FirstOrDefault(),
                Carga = provisao.Carga,
                Situacao = situacaoEmCancelamento ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.EmCancelamento : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoProvisao.NaoProcessado
            };

            if (cancelamentoProvisao.Filial == null && cancelamentoProvisao.Carga != null)
                cancelamentoProvisao.Filial = cancelamentoProvisao.Carga.Filial;

            if (cancelamentoProvisao.Empresa == null && cancelamentoProvisao.Carga != null)
                cancelamentoProvisao.Empresa = cancelamentoProvisao.Carga.Empresa;

            if (provisao.CargaOcorrencia != null)
                cancelamentoProvisao.CargaOcorrencia = ObterRepositorioCargaOcorrencia().BuscarPorCodigo(provisao.CargaOcorrencia.Codigo);

            ObterRepositorioCancelamentoProvisao().Inserir(cancelamentoProvisao);

            Servicos.Embarcador.Escrituracao.DocumentoProvisao.AdicionarDocumentoParaCancelamento(cancelamentoProvisao, documentoProvisao, _unitOfWork);
            if (gerarIntegracoes)
                Servicos.Embarcador.Escrituracao.CancelamentoProvisao.EfetuarIntegracaoCancelamentoProvisao(cancelamentoProvisao, _unitOfWork);

            cancelamentoProvisao.ValorCancelamentoProvisao = documentoProvisao.ValorProvisao;
            cancelamentoProvisao.QuantidadeDocsProvisao = 1;
            cancelamentoProvisao.GerandoMovimentoFinanceiro = true;

            return cancelamentoProvisao;
        }

        private string GerarFechamentoProvisao(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao, string textoAuditoria)
        {
            string retorno = string.Empty;
            Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(_unitOfWork);
            try
            {

                Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
                Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repositorioDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(_unitOfWork);
                bool possuiStage = repositorioDocumentoContabil.ExisteComStage(provisao.Codigo);

                if (possuiStage && repositorioDocumentoContabil.ExisteSemImpostoValorAgregado(provisao.Codigo))
                    throw new ServicoException("Não é possivel confirmar o fechamento com documentos sem o IVA definido");

                provisao.DataLancamento = ObterDataLancamento();
                repositorioDocumentoContabil.ConfirmarMovimentoPorProvisao(provisao.Codigo, provisao.DataLancamento.Value);
                repositorioDocumentoProvisao.ConfirmarProvisaoDocumentos(provisao.Codigo);

                ValidacaoRelevanciaCustos(provisao);

                new Servicos.Embarcador.Integracao.IntegracaoProvisao(_unitOfWork).Adicionar(provisao);
                Servicos.Auditoria.Auditoria.Auditar(_auditado, provisao, null, textoAuditoria, _unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
            catch (ServicoException exception)
            {
                retorno = exception.Message;
                provisao.DataUltimoIntentoFechamento = DateTime.Now;
                repositorioProvisao.Atualizar(provisao);
            }

            return retorno;
        }

        private void ValidacaoRelevanciaCustos(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao)
        {
            if (!(provisao.Carga?.TipoOperacao?.ConfiguracaoCarga?.AguardarRecebimentoProdutoParaProvisionar ?? false))
                return;

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal respositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.NotaFiscalMigo repositoNotaFiscalMigo = new Repositorio.Embarcador.Pedidos.NotaFiscalMigo(_unitOfWork);
            List<int> codigosXmlNotasFiscais = respositorioXmlNotaFiscal.BuscarCodigosRelevantesParaFretePorCarga(provisao.Carga.Codigo);

            if (provisao.Carga.DadosSumarizados?.CargaTrecho != null)
            {
                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(_unitOfWork);
                List<int> codigosXmlNotaFiscalComCTeConfirmado = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarCodigosXmlNotaFiscalComCTeConfirmadoPorCarga(provisao.Carga.Codigo);

                codigosXmlNotasFiscais = codigosXmlNotasFiscais.Where(codigoNota => codigosXmlNotaFiscalComCTeConfirmado.Contains(codigoNota)).ToList();
            }

            foreach (int codigoXmlNotaFiscal in codigosXmlNotasFiscais)
            {
                Dominio.Entidades.Embarcador.Pedidos.NotaFiscalMigo notaMigo = repositoNotaFiscalMigo.BuscarPorXmlNotaFiscal(codigoXmlNotaFiscal);

                if (notaMigo == null)
                    throw new ServicoException($"Não foi recibida integrações MIGO para as notas com prioridades");
            }
        }

        private void GerarFechamentoDocumentosProvisao(Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repProvisaoDocumentos = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.Provisao repProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(_unitOfWork);

            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado serConfiguracaoCentroResultado = new ConfiguracaoContabil.ConfiguracaoCentroResultado();
            Servicos.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil serConfiguracaoContaContabil = new ConfiguracaoContabil.ConfiguracaoContaContabil();
            Servicos.Embarcador.Hubs.Provisao servicoNotificacaoProvisao = new Hubs.Provisao();

            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ProvisaoSumarizada> provisoesSumarizadas = repProvisaoDocumentos.BuscarCodigosPorProvisaoEmFechamento(provisao.Codigo);

            int quantidadeDocumentos = provisao.QuantidadeDocsProvisao;
            int quantidadeGerados = quantidadeDocumentos - provisoesSumarizadas.Count;

            List<int?> empresas = (from obj in provisoesSumarizadas select obj.Empresa).Distinct().ToList();

            int quantidadeTotal = 0;
            for (int e = 0; e < empresas.Count; e++)
            {
                int? codigoEmpresa = empresas[e];
                decimal aliquotaCOFINS = 0;
                decimal aliquotaPIS = 0;
                Dominio.Entidades.Empresa empresa = null;
                if (codigoEmpresa != null)
                {
                    empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa.Value);
                    aliquotaCOFINS = empresa.EmpresaPai?.Configuracao?.AliquotaCOFINS ?? 0;
                    aliquotaPIS = empresa.EmpresaPai?.Configuracao?.AliquotaPIS ?? 0;
                }
                else
                {
                    Dominio.Entidades.Empresa empresaPai = repEmpresa.BuscarEmpresaPai();
                    aliquotaCOFINS = empresaPai?.Configuracao?.AliquotaCOFINS ?? 0;
                    aliquotaPIS = empresaPai?.Configuracao?.AliquotaPIS ?? 0;
                }

                List<int?> modelosDocumentosFiscal = (from obj in provisoesSumarizadas where obj.Empresa == codigoEmpresa select obj.ModeloDocumentoFiscal).Distinct().ToList();
                for (int md = 0; md < modelosDocumentosFiscal.Count; md++)
                {
                    int? codigoModeloDocumento = modelosDocumentosFiscal[md];
                    Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = codigoModeloDocumento.HasValue ? new Dominio.Entidades.ModeloDocumentoFiscal() { Codigo = codigoModeloDocumento.Value } : null;

                    List<int?> tiposOcorrencia = (from obj in provisoesSumarizadas where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.Empresa == codigoEmpresa select obj.TipoOcorrencia).Distinct().ToList();

                    for (int tpo = 0; tpo < tiposOcorrencia.Count; tpo++)
                    {
                        int? codigoTipoOcorrencia = tiposOcorrencia[tpo];
                        Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = codigoTipoOcorrencia.HasValue ? new Dominio.Entidades.TipoDeOcorrenciaDeCTe() { Codigo = codigoTipoOcorrencia.Value } : null;

                        List<int?> grupoTomadores = (from obj in provisoesSumarizadas where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Empresa == codigoEmpresa select obj.GrupoTomador).Distinct().ToList();
                        for (int gt = 0; gt < grupoTomadores.Count; gt++)
                        {
                            int? codigoGrupoTomador = grupoTomadores[gt];
                            List<double?> tomadores = (from obj in provisoesSumarizadas where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.GrupoTomador == codigoGrupoTomador && obj.Empresa == codigoEmpresa select obj.Tomador).Distinct().ToList();
                            for (int t = 0; t < tomadores.Count; t++)
                            {
                                double? cnpjTomador = tomadores[t];
                                Dominio.Entidades.Cliente tomador = null;
                                if (cnpjTomador.HasValue)
                                {
                                    tomador = new Dominio.Entidades.Cliente() { CPF_CNPJ = cnpjTomador.Value };
                                    tomador.GrupoPessoas = codigoGrupoTomador.HasValue ? new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas() { Codigo = codigoGrupoTomador.Value } : null;
                                }

                                List<int?> tiposOperacao = (from obj in provisoesSumarizadas where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa select obj.TipoOperacao).Distinct().ToList();
                                for (int o = 0; o < tiposOperacao.Count; o++)
                                {
                                    int? codigoTipoOperacao = tiposOperacao[o];
                                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = codigoTipoOperacao.HasValue ? new Dominio.Entidades.Embarcador.Pedidos.TipoOperacao() { Codigo = codigoTipoOperacao.Value } : null;

                                    List<int?> grupoRemetentes = (from obj in provisoesSumarizadas where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.TipoOperacao == codigoTipoOperacao && obj.Empresa == codigoEmpresa select obj.GrupoRemetente).Distinct().ToList();

                                    for (int gr = 0; gr < grupoRemetentes.Count; gr++)
                                    {
                                        int? codigoGrupoRemetente = grupoRemetentes[gr];
                                        List<double?> remetentes = (from obj in provisoesSumarizadas where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.GrupoRemetente == codigoGrupoRemetente && obj.TipoOperacao == codigoTipoOperacao select obj.Remetente).Distinct().ToList();

                                        for (int r = 0; r < remetentes.Count; r++)
                                        {
                                            double? cnpjRemetente = remetentes[r];
                                            Dominio.Entidades.Cliente remetente = null;
                                            if (cnpjRemetente.HasValue)
                                            {
                                                remetente = new Dominio.Entidades.Cliente() { CPF_CNPJ = cnpjRemetente.Value };
                                                remetente.GrupoPessoas = codigoGrupoRemetente.HasValue ? new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas() { Codigo = codigoGrupoRemetente.Value } : null;
                                            }

                                            List<int?> grupoDestinatarios = (from obj in provisoesSumarizadas where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.TipoOperacao == codigoTipoOperacao && obj.Remetente == cnpjRemetente select obj.GrupoDestinatario).Distinct().ToList();
                                            for (int gd = 0; gd < grupoDestinatarios.Count; gd++)
                                            {
                                                int? codigoGrupoDestinatario = grupoDestinatarios[gd];
                                                List<double?> destinatarios = (from obj in provisoesSumarizadas where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.Remetente == cnpjRemetente && obj.TipoOperacao == codigoTipoOperacao && obj.GrupoDestinatario == codigoGrupoDestinatario select obj.Destinatario).Distinct().ToList();

                                                for (int d = 0; d < destinatarios.Count; d++)
                                                {
                                                    double? cnpjDestinatario = destinatarios[d];
                                                    Dominio.Entidades.Cliente destinatario = null;
                                                    if (cnpjDestinatario.HasValue)
                                                    {
                                                        destinatario = new Dominio.Entidades.Cliente() { CPF_CNPJ = cnpjDestinatario.Value };
                                                        destinatario.GrupoPessoas = codigoGrupoDestinatario.HasValue ? new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas() { Codigo = codigoGrupoDestinatario.Value } : null;
                                                    }

                                                    List<int?> rotasFrete = (from obj in provisoesSumarizadas where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.Remetente == cnpjRemetente && obj.TipoOperacao == codigoTipoOperacao && obj.Destinatario == cnpjDestinatario select obj.RotaFrete).Distinct().ToList();
                                                    for (int rf = 0; rf < rotasFrete.Count; rf++)
                                                    {
                                                        int? codigoRotaFrete = rotasFrete[rf];
                                                        Dominio.Entidades.RotaFrete rotaFrete = codigoRotaFrete.HasValue ? new Dominio.Entidades.RotaFrete() { Codigo = codigoRotaFrete.Value } : null;

                                                        List<int?> origens = (from obj in provisoesSumarizadas where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.Remetente == cnpjRemetente && obj.TipoOperacao == codigoTipoOperacao && obj.Destinatario == cnpjDestinatario && obj.RotaFrete == codigoRotaFrete select obj.Origem).Distinct().ToList();

                                                        for (int or = 0; or < origens.Count; or++)
                                                        {
                                                            int? codigoOrigem = origens[or];
                                                            Dominio.Entidades.Localidade origem = codigoOrigem.HasValue ? new Dominio.Entidades.Localidade() { Codigo = codigoOrigem.Value } : null;

                                                            List<int?> filiais = (from obj in provisoesSumarizadas where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.Remetente == cnpjRemetente && obj.TipoOperacao == codigoTipoOperacao && obj.Destinatario == cnpjDestinatario && obj.RotaFrete == codigoRotaFrete && obj.Origem == codigoOrigem select obj.Filial).Distinct().ToList();
                                                            for (int fil = 0; fil < filiais.Count; fil++)
                                                            {
                                                                int? codigoFilial = filiais[fil];
                                                                Dominio.Entidades.Embarcador.Filiais.Filial filial = codigoFilial.HasValue ? new Dominio.Entidades.Embarcador.Filiais.Filial() { Codigo = codigoFilial.Value } : null;
                                                                //Configuração Conta Contabil não utiliza os parametros dos for abaixo, portanto pode ser consultado menos vezes
                                                                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabil configuracaoContaContabil = serConfiguracaoContaContabil.ObterConfiguracaoContaContabil(remetente, destinatario, tomador, null, empresa, tipoOperacao, rotaFrete, modeloDocumentoFiscal, tipoOcorrencia, _unitOfWork);

                                                                List<double?> expedidores = (from obj in provisoesSumarizadas where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.Remetente == cnpjRemetente && obj.TipoOperacao == codigoTipoOperacao && obj.Destinatario == cnpjDestinatario && obj.RotaFrete == codigoRotaFrete && obj.Origem == codigoOrigem && obj.Filial == codigoFilial select obj.Expedidor).Distinct().ToList();

                                                                for (int exp = 0; exp < expedidores.Count; exp++)
                                                                {
                                                                    double? cnpjExpedidor = expedidores[exp];

                                                                    Dominio.Entidades.Cliente expedidor = cnpjExpedidor.HasValue ? new Dominio.Entidades.Cliente() { CPF_CNPJ = cnpjExpedidor.Value } : null;

                                                                    List<double?> recebedores = (from obj in provisoesSumarizadas where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.Remetente == cnpjRemetente && obj.TipoOperacao == codigoTipoOperacao && obj.Destinatario == cnpjDestinatario && obj.RotaFrete == codigoRotaFrete && obj.Origem == codigoOrigem && obj.Filial == codigoFilial && obj.Expedidor == cnpjExpedidor select obj.Recebedor).Distinct().ToList();
                                                                    for (int rec = 0; rec < recebedores.Count; rec++)
                                                                    {
                                                                        double? cnpjRecebedor = recebedores[rec];

                                                                        Dominio.Entidades.Cliente recebedor = cnpjRecebedor.HasValue ? new Dominio.Entidades.Cliente() { CPF_CNPJ = cnpjRecebedor.Value } : null;

                                                                        Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoCentroResultado configuracaoCentroResultado = serConfiguracaoCentroResultado.ObterConfiguracaoCentroResultado(remetente, destinatario, expedidor, recebedor, tomador, null, null, empresa, tipoOperacao, tipoOcorrencia, rotaFrete, filial, origem, _unitOfWork);

                                                                        if (configuracaoContaContabil != null && configuracaoCentroResultado != null)
                                                                        {
                                                                            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.ProvisaoSumarizada> provisoesDocumentosFiltradas = (from obj in provisoesSumarizadas where obj.ModeloDocumentoFiscal == codigoModeloDocumento && obj.TipoOcorrencia == codigoTipoOcorrencia && obj.Tomador == cnpjTomador && obj.Empresa == codigoEmpresa && obj.Remetente == cnpjRemetente && obj.TipoOperacao == codigoTipoOperacao && obj.Destinatario == cnpjDestinatario && obj.RotaFrete == codigoRotaFrete && obj.Origem == codigoOrigem && obj.Filial == codigoFilial && obj.Expedidor == cnpjExpedidor && obj.Recebedor == cnpjRecebedor select obj).ToList();

                                                                            int codigoCentroResultado = configuracaoCentroResultado.CentroResultadoContabilizacao.Codigo;
                                                                            List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil> configuracoesContabeis = new List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil>();
                                                                            foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContaContabilProvisao configuracaoProvisao in configuracaoContaContabil.ConfiguracaoContaContabilProvisoes)
                                                                            {
                                                                                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil configuracaoContabil = new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil();
                                                                                if (configuracaoProvisao.CodigoPlanoConta > 0)
                                                                                    configuracaoContabil.PlanoConta = configuracaoProvisao.CodigoPlanoConta;
                                                                                configuracaoContabil.TipoContabilizacao = configuracaoProvisao.TipoContabilizacao;
                                                                                configuracaoContabil.TipoContaContabil = configuracaoProvisao.TipoContaContabil;
                                                                                configuracaoContabil.ComponentesDeFreteDoTipoDescontoNaoDevemSomar = configuracaoProvisao.ComponentesDeFreteDoTipoDescontoNaoDevemSomarNaProvisao ?? false;
                                                                                configuracoesContabeis.Add(configuracaoContabil);
                                                                            }

                                                                            bool componentesDeFreteDoTipoDescontoNaoDevemSomarNaProvisao = configuracoesContabeis.Exists(configuracao => configuracao.ComponentesDeFreteDoTipoDescontoNaoDevemSomar);

                                                                            for (int i = 0; i < provisoesDocumentosFiltradas.Count; i++)
                                                                            {
                                                                                bool abriuTransacao = false;
                                                                                if (!_unitOfWork.IsActiveTransaction())
                                                                                {
                                                                                    _unitOfWork.Start();
                                                                                    abriuTransacao = true;
                                                                                }

                                                                                Dominio.ObjetosDeValor.Embarcador.Escrituracao.ProvisaoSumarizada provisaoSumarizada = provisoesDocumentosFiltradas[i];

                                                                                GerarDocumentoContabil(provisaoSumarizada, configuracoesContabeis, codigoCentroResultado, aliquotaCOFINS, aliquotaPIS, provisao, componentesDeFreteDoTipoDescontoNaoDevemSomarNaProvisao);
                                                                                repProvisaoDocumentos.SetarDocumentoMovimentoGeradoProvisionado(provisaoSumarizada.Codigo);

                                                                                if (abriuTransacao)
                                                                                    _unitOfWork.CommitChanges();

                                                                                if (quantidadeDocumentos < 10 || ((quantidadeTotal + 1) % 5) == 0)
                                                                                {
                                                                                    if (abriuTransacao)
                                                                                        _unitOfWork.FlushAndClear();
                                                                                    servicoNotificacaoProvisao.InformarQuantidadeDocumentosProcessadosFechamentoProvisao(provisao.Codigo, quantidadeDocumentos, ((quantidadeGerados + quantidadeTotal) + 1));
                                                                                }
                                                                                quantidadeTotal++;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            string mensagem = "Não foi possível localizar a configuração de ";
                                                                            if (configuracaoCentroResultado == null)
                                                                                mensagem += " Centro de Resultado ";
                                                                            if (configuracaoContaContabil == null && configuracaoCentroResultado == null)
                                                                                mensagem += " e ";
                                                                            if (configuracaoContaContabil == null)
                                                                                mensagem += " Conta Contábil ";
                                                                            mensagem += " para as configurações a seguir: ";
                                                                            if (empresa != null)
                                                                                mensagem += " (Empresa " + repEmpresa.BuscarPorCodigo(empresa.Codigo).Descricao + ") ";
                                                                            if (tipoOperacao != null)
                                                                                mensagem += " (Tipo de Operação " + repTipoOperacao.BuscarPorCodigo(tipoOperacao.Codigo).Descricao + ") ";
                                                                            if (tomador != null)
                                                                                mensagem += " (Tomador " + repCliente.BuscarPorCPFCNPJ(tomador.CPF_CNPJ).Descricao + ") ";
                                                                            if (remetente != null)
                                                                                mensagem += " (Remetente " + repCliente.BuscarPorCPFCNPJ(remetente.CPF_CNPJ).Descricao + ") ";
                                                                            if (destinatario != null)
                                                                                mensagem += " (Destinatário " + repCliente.BuscarPorCPFCNPJ(destinatario.CPF_CNPJ).Descricao + ") ";
                                                                            if (rotaFrete != null)
                                                                                mensagem += " (Rota " + repRotaFrete.BuscarPorCodigo(rotaFrete.Codigo).CodigoIntegracao + ") ";

                                                                            provisao.MotivoRejeicaoFechamentoProvisao = mensagem;
                                                                            provisao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProvisao.PendenciaFechamento;
                                                                            repProvisao.Atualizar(provisao);
                                                                            if (_unitOfWork.IsActiveTransaction())
                                                                                _unitOfWork.CommitChanges();
                                                                            servicoNotificacaoProvisao.InformarProvisaoAtualizada(provisao.Codigo);
                                                                            return;
                                                                        }
                                                                    }
                                                                }


                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


            }

            provisao.GerandoMovimentoFinanceiroProvisao = false;
            repProvisao.Atualizar(provisao);

            servicoNotificacaoProvisao.InformarProvisaoAtualizada(provisao.Codigo);
        }

        private void GerarDocumentoContabil(Dominio.ObjetosDeValor.Embarcador.Escrituracao.ProvisaoSumarizada provisaoSumarizada, List<Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil> configuracoesContabeis, int codigoCentroResultado, decimal aliquotaCOFINS, decimal aliquotaPIS, Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao, bool componentesDeFreteDoTipoDescontoNaoDevemSomarNaProvisao)
        {
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = ObterConfiguracaoFinanceiro();

            foreach (Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.ConfiguracaoContabil configuracaoContabil in configuracoesContabeis)
            {
                Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil documentoContabil = new Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil();

                documentoContabil.Carga = provisaoSumarizada.Carga.HasValue ? new Dominio.Entidades.Embarcador.Cargas.Carga() { Codigo = provisaoSumarizada.Carga.Value } : null;
                documentoContabil.CargaOcorrencia = provisaoSumarizada.CargaOcorrencia.HasValue ? new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia() { Codigo = provisaoSumarizada.CargaOcorrencia.Value } : null;
                documentoContabil.CentroResultado = new Dominio.Entidades.Embarcador.Financeiro.CentroResultado() { Codigo = codigoCentroResultado };
                documentoContabil.CTeTerceiro = provisaoSumarizada.CTeTerceiro.HasValue ? new Dominio.Entidades.Embarcador.CTe.CTeTerceiro() { Codigo = provisaoSumarizada.CTeTerceiro.Value } : null;
                documentoContabil.PedidoCTeParaSubContratacao = provisaoSumarizada.PedidoCTeParaSubContratacao.HasValue ? new Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao() { Codigo = provisaoSumarizada.PedidoCTeParaSubContratacao.Value } : null;
                documentoContabil.DataEmissao = provisaoSumarizada.DataEmissao.HasValue ? provisaoSumarizada.DataEmissao.Value : DateTime.Now;
                documentoContabil.DataRegistro = DateTime.Now;
                documentoContabil.DataLancamento = provisao.DataLancamento.HasValue ? provisao.DataLancamento.Value : provisao.DataCriacao;
                documentoContabil.DataEmissaoCTe = DateTime.Now;
                documentoContabil.DocumentoProvisao = new Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao() { Codigo = provisaoSumarizada.Codigo };
                documentoContabil.DocumentoProvisaoReferencia = documentoContabil.DocumentoProvisao;
                documentoContabil.Empresa = provisaoSumarizada.Empresa.HasValue ? new Dominio.Entidades.Empresa() { Codigo = provisaoSumarizada.Empresa.Value } : null;
                documentoContabil.Filial = provisaoSumarizada.Filial.HasValue ? new Dominio.Entidades.Embarcador.Filiais.Filial() { Codigo = provisaoSumarizada.Filial.Value } : null;
                documentoContabil.ModeloDocumentoFiscal = provisaoSumarizada.ModeloDocumentoFiscal.HasValue ? new Dominio.Entidades.ModeloDocumentoFiscal() { Codigo = provisaoSumarizada.ModeloDocumentoFiscal.Value } : null;
                documentoContabil.NumeroDocumento = provisaoSumarizada.NumeroDocumento;
                documentoContabil.Provisao = new Dominio.Entidades.Embarcador.Escrituracao.Provisao() { Codigo = provisao.Codigo };
                documentoContabil.TipoOperacao = provisaoSumarizada.TipoOperacao.HasValue ? new Dominio.Entidades.Embarcador.Pedidos.TipoOperacao() { Codigo = provisaoSumarizada.TipoOperacao.Value } : null;
                documentoContabil.Tomador = provisaoSumarizada.Tomador.HasValue ? new Dominio.Entidades.Cliente() { CPF_CNPJ = provisaoSumarizada.Tomador.Value } : null;
                documentoContabil.XMLNotaFiscal = provisaoSumarizada.XMLNotaFiscal.HasValue ? new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal() { Codigo = provisaoSumarizada.XMLNotaFiscal.Value } : null;
                documentoContabil.Stage = provisaoSumarizada.Stage.HasValue ? new Dominio.Entidades.Embarcador.Pedidos.Stage() { Codigo = provisaoSumarizada.Stage.Value } : null;
                documentoContabil.ImpostoValorAgregado = provisaoSumarizada.ImpostoValorAgregado.HasValue ? new Dominio.Entidades.Embarcador.Contabeis.ImpostoValorAgregado() { Codigo = provisaoSumarizada.ImpostoValorAgregado.Value } : null;
                documentoContabil.Remetente = provisaoSumarizada.Remetente.HasValue ? new Dominio.Entidades.Cliente() { CPF_CNPJ = provisaoSumarizada.Remetente.Value } : null;
                documentoContabil.Destinatario = provisaoSumarizada.Destinatario.HasValue ? new Dominio.Entidades.Cliente() { CPF_CNPJ = provisaoSumarizada.Destinatario.Value } : null;
                documentoContabil.Origem = provisaoSumarizada.Origem.HasValue ? new Dominio.Entidades.Localidade() { Codigo = provisaoSumarizada.Origem.Value } : null;
                documentoContabil.Destino = provisaoSumarizada.Destino.HasValue ? new Dominio.Entidades.Localidade() { Codigo = provisaoSumarizada.Destino.Value } : null;
                documentoContabil.PesoBruto = provisaoSumarizada.PesoBruto.HasValue ? provisaoSumarizada.PesoBruto.Value : 0;
                documentoContabil.PlanoConta = new Dominio.Entidades.Embarcador.Financeiro.PlanoConta() { Codigo = configuracaoContabil.PlanoConta };
                documentoContabil.SerieDocumento = provisaoSumarizada.NumeroDocumento;
                documentoContabil.Situacao = SituacaoDocumentoContabil.AgConsolidacao;
                documentoContabil.TipoContabilizacao = configuracaoContabil.TipoContabilizacao;
                documentoContabil.TipoContaContabil = configuracaoContabil.TipoContaContabil;
                documentoContabil.AliquotaIss = provisaoSumarizada.PercentualAliquotaISS;
                documentoContabil.AliquotaCofins = aliquotaCOFINS;
                documentoContabil.AliquotaPis = aliquotaPIS;
                documentoContabil.OutrasAliquotas = provisaoSumarizada.OutrasAliquotas;
                documentoContabil.AliquotaCBS = provisaoSumarizada.AliquotaCBS;
                documentoContabil.AliquotaIBSEstadual = provisaoSumarizada.AliquotaIBSEstadual;
                documentoContabil.AliquotaIBSMunicipal = provisaoSumarizada.AliquotaIBSMunicipal;

                if ((provisaoSumarizada.CodigoImpostoValorAgregado == "FI") || (provisaoSumarizada.CodigoImpostoValorAgregado == "XE") || (provisaoSumarizada.CodigoImpostoValorAgregado == "XG"))
                {
                    documentoContabil.AliquotaCofins = 0;
                    documentoContabil.AliquotaPis = 0;
                }

                decimal valorICMS = provisaoSumarizada.CST != "60" ? provisaoSumarizada.ValorICMS : 0;
                decimal valorICMSST = provisaoSumarizada.CST == "60" ? provisaoSumarizada.ValorICMS : 0;
                decimal valorISS = provisaoSumarizada.ValorISS - provisaoSumarizada.ValorRetencaoISS;
                decimal valorISSRetido = provisaoSumarizada.ValorRetencaoISS;
                decimal totalReceber = 0;
                decimal totalPrestacao = 0;

                bool provisaoNotaServico = (
                    (provisaoSumarizada.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS) ||
                    (provisaoSumarizada.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe) ||
                    ((provisaoSumarizada.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros) && ((valorISS + valorISSRetido) > 0m))
                );

                if (provisaoNotaServico)
                {
                    totalReceber = provisaoSumarizada.ISSInclusoBC ? provisaoSumarizada.Valor + valorISS : provisaoSumarizada.Valor;
                    totalPrestacao = totalReceber + valorISSRetido;
                }
                else
                {
                    totalPrestacao = provisaoSumarizada.ICMSInclusoBC ? provisaoSumarizada.Valor + provisaoSumarizada.ValorICMS : provisaoSumarizada.Valor;

                    if (provisaoSumarizada.CST != "60")
                        totalReceber = provisaoSumarizada.ICMSInclusoBC ? provisaoSumarizada.Valor + provisaoSumarizada.ValorICMS : provisaoSumarizada.Valor;
                    else
                        totalReceber = provisaoSumarizada.Valor;
                }

                decimal basePisCofins = ObterBasePisCofins(provisaoSumarizada, componentesDeFreteDoTipoDescontoNaoDevemSomarNaProvisao, configuracaoFinanceiro.NaoIncluirICMSBaseCalculoPisCofins, valorICMS, totalPrestacao);

                decimal valorCOFINS = Math.Round(basePisCofins * (documentoContabil.AliquotaCofins / 100), 2, MidpointRounding.AwayFromZero);
                decimal valorPIS = Math.Round(basePisCofins * (documentoContabil.AliquotaPis / 100), 2, MidpointRounding.AwayFromZero);

                if (configuracaoContabil.TipoContaContabil == TipoContaContabil.ICMS)
                {
                    if (valorICMS <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = valorICMS;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.ICMSST)
                {
                    if (valorICMSST <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = valorICMSST;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.ISS)
                {
                    if (valorISS <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = valorISS;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.ISSRetido)
                {
                    if (valorISSRetido <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = valorISSRetido;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.PIS)
                {
                    if (valorPIS <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = valorPIS;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.COFINS)
                {
                    if (valorCOFINS <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = valorCOFINS;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.CBS)
                {
                    if (provisaoSumarizada.ValorCBS <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = provisaoSumarizada.ValorCBS;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.IBSEstadual)
                {
                    if (provisaoSumarizada.ValorIBSEstadual <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = provisaoSumarizada.ValorIBSEstadual;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.IBSMunicipal)
                {
                    if (provisaoSumarizada.ValorIBSMunicipal <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = provisaoSumarizada.ValorIBSMunicipal;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.FreteLiquido || configuracaoContabil.TipoContaContabil == TipoContaContabil.FreteLiquido2 || configuracaoContabil.TipoContaContabil == TipoContaContabil.FreteLiquido9)
                {
                    decimal freteLiquido = totalReceber - valorPIS - valorCOFINS - valorICMS;

                    if (configuracaoFinanceiro.SomarValorISSNoTotalReceberGeracaoLoteProvisao)
                        freteLiquido += ((provisaoNotaServico && provisaoSumarizada.ISSInclusoBC ? 0 : valorISS) + valorISSRetido);
                    else
                        freteLiquido -= valorISS;

                    if (configuracaoContabil.ComponentesDeFreteDoTipoDescontoNaoDevemSomar)
                        freteLiquido = Math.Max(0, freteLiquido - provisaoSumarizada.ValorDesconto);

                    if (freteLiquido <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = freteLiquido;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.FreteLiquidoSemComponentesFrete)
                {
                    decimal freteLiquido = totalReceber - valorPIS - valorCOFINS - valorICMS;

                    if (configuracaoFinanceiro.SomarValorISSNoTotalReceberGeracaoLoteProvisao)
                        freteLiquido += ((provisaoNotaServico && provisaoSumarizada.ISSInclusoBC ? 0 : valorISS) + valorISSRetido);
                    else
                        freteLiquido -= valorISS;

                    freteLiquido -= (provisaoSumarizada.ValorAdValorem + provisaoSumarizada.ValorDescarga + provisaoSumarizada.ValorPedagio + provisaoSumarizada.ValorGris + provisaoSumarizada.ValorEntrega + provisaoSumarizada.ValorPernoite);

                    if (freteLiquido <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = freteLiquido;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.TotalReceber)
                {
                    decimal valorTotalReceber = totalReceber;

                    if (configuracaoFinanceiro.SomarValorISSNoTotalReceberGeracaoLoteProvisao)
                        valorTotalReceber += ((provisaoNotaServico && provisaoSumarizada.ISSInclusoBC ? 0 : valorISS) + valorISSRetido);

                    if (configuracaoContabil.ComponentesDeFreteDoTipoDescontoNaoDevemSomar)
                        valorTotalReceber = Math.Max(0, valorTotalReceber - provisaoSumarizada.ValorDesconto);

                    if (valorTotalReceber <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = valorTotalReceber;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.FreteValor)
                {
                    decimal freteValor = totalReceber - valorICMS - valorISS + valorISSRetido;

                    if (freteValor <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = freteValor;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.AdValorem)
                {
                    if (provisaoSumarizada.ValorAdValorem <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = provisaoSumarizada.ValorAdValorem;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.GRIS)
                {
                    if (provisaoSumarizada.ValorGris <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = provisaoSumarizada.ValorGris;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.Pedagio)
                {
                    if (provisaoSumarizada.ValorPedagio <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = provisaoSumarizada.ValorPedagio;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.TaxaDescarga)
                {
                    if (provisaoSumarizada.ValorDescarga <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = provisaoSumarizada.ValorDescarga;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.TaxaEntrega)
                {
                    if (provisaoSumarizada.ValorEntrega <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = provisaoSumarizada.ValorEntrega;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.ImpostoValorAgregado)
                {
                    continue;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.CustoFixo)
                {
                    documentoContabil.ValorContabilizacao = provisaoSumarizada.ValorContratoFrete;

                    if (documentoContabil.ValorContabilizacao <= 0m)
                        continue;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.FreteCaixa)
                {
                    documentoContabil.ValorContabilizacao = (provisaoSumarizada.TipoValorFrete == TipoValorFreteDocumentoProvisao.TipoEmbalagem) ? provisaoSumarizada.ValorFrete : 0m;

                    if (documentoContabil.ValorContabilizacao <= 0m)
                        continue;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.FreteKM)
                {
                    documentoContabil.ValorContabilizacao = (provisaoSumarizada.TipoValorFrete == TipoValorFreteDocumentoProvisao.Distancia) ? provisaoSumarizada.ValorFrete : 0m;

                    if (documentoContabil.ValorContabilizacao <= 0m)
                        continue;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.FretePeso)
                {
                    documentoContabil.ValorContabilizacao = (provisaoSumarizada.TipoValorFrete == TipoValorFreteDocumentoProvisao.Peso) ? provisaoSumarizada.ValorFrete : 0m;

                    if (documentoContabil.ValorContabilizacao <= 0m)
                        continue;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.FreteViagem)
                {
                    documentoContabil.ValorContabilizacao = (provisaoSumarizada.TipoValorFrete == TipoValorFreteDocumentoProvisao.TipoCarga) ? provisaoSumarizada.ValorFrete : 0m;

                    if (documentoContabil.ValorContabilizacao <= 0m)
                        continue;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.TaxaTotal)
                {
                    decimal taxaTotal = valorICMS + valorPIS + valorCOFINS + valorICMSST;

                    if (taxaTotal <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = taxaTotal;
                }
                else if (configuracaoContabil.TipoContaContabil == TipoContaContabil.Pernoite)
                {
                    if (provisaoSumarizada.ValorPernoite <= 0)
                        continue;

                    documentoContabil.ValorContabilizacao = provisaoSumarizada.ValorPernoite;
                }

                repDocumentoContabil.Inserir(documentoContabil);
            }
        }

        private static decimal ObterBasePisCofins(Dominio.ObjetosDeValor.Embarcador.Escrituracao.ProvisaoSumarizada provisaoSumarizada, bool componentesDeFreteDoTipoDescontoNaoDevemSomarNaProvisao, bool naoIncluirICMSBaseCalculoPisCofins, decimal valorICMS, decimal totalPrestacao)
        {
            decimal basePisCofins = totalPrestacao;

            if (naoIncluirICMSBaseCalculoPisCofins && provisaoSumarizada.ICMSInclusoBC)
                basePisCofins = totalPrestacao - valorICMS;

            if (componentesDeFreteDoTipoDescontoNaoDevemSomarNaProvisao)
                basePisCofins = Math.Max(0, basePisCofins - provisaoSumarizada.ValorDesconto);

            return basePisCofins;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro ObterConfiguracaoFinanceiro()
        {
            if (_configuracaoFinanceiro == null)
                _configuracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoFinanceiro;
        }

        private Dominio.Entidades.Embarcador.Escrituracao.ConfiguracaoProvisao ObterConfiguracaoProvisao()
        {
            Repositorio.Embarcador.Escrituracao.ConfiguracaoProvisao repositorio = new Repositorio.Embarcador.Escrituracao.ConfiguracaoProvisao(_unitOfWork);

            return repositorio.BuscarConfiguracao();
        }

        private void GerarProvisao(int codigoCarga, List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentosProvisao, bool buscarProvisaoExistente = true)
        {
            Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(_unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);

            Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = null;

            if (buscarProvisaoExistente)
                provisao = repositorioProvisao.BuscarNaoFinalizadaPorCarga(codigoCarga);

            if (provisao == null)
            {
                provisao = new Dominio.Entidades.Embarcador.Escrituracao.Provisao();
                provisao.Carga = documentosProvisao.FirstOrDefault()?.Carga ?? null;
                provisao.DataCriacao = DateTime.Now;
                provisao.Numero = Servicos.Embarcador.Escrituracao.ProvisaoSequencial.GetInstance().ObterProximoNumeroSequencial(_unitOfWork);
                provisao.Tomador = documentosProvisao.FirstOrDefault()?.Tomador ?? null;
                provisao.Situacao = SituacaoProvisao.EmFechamento;
                provisao.GerandoMovimentoFinanceiroProvisao = true;

                repositorioProvisao.Inserir(provisao);
            }
            else
            {
                provisao.Situacao = SituacaoProvisao.EmFechamento;
                provisao.GerandoMovimentoFinanceiroProvisao = true;
            }

            foreach (Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao documentoProvisao in documentosProvisao)
            {
                documentoProvisao.Provisao = provisao;
                documentoProvisao.Situacao = SituacaoProvisaoDocumento.EmFechamento;

                repositorioDocumentoProvisao.Atualizar(documentoProvisao);

                provisao.ValorProvisao += documentoProvisao.ValorProvisao;
                provisao.QuantidadeDocsProvisao++;
            }

            repositorioProvisao.Atualizar(provisao);
        }


        private Repositorio.Embarcador.Escrituracao.CancelamentoProvisao ObterRepositorioCancelamentoProvisao()
        {
            if (_repositorioCancelamentoProvisao == null)
                _repositorioCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(_unitOfWork);

            return _repositorioCancelamentoProvisao;
        }

        private Repositorio.Embarcador.Ocorrencias.CargaOcorrencia ObterRepositorioCargaOcorrencia()
        {
            if (_repositorioCargaOcorrencia == null)
                _repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);

            return _repositorioCargaOcorrencia;
        }

        private Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao ObterRepositorioProvisaoIntegracao()
        {
            if (_repositorioProvisaoIntegracao == null)
                _repositorioProvisaoIntegracao = new Repositorio.Embarcador.Escrituracao.ProvisaoIntegracao(_unitOfWork);

            return _repositorioProvisaoIntegracao;
        }

        private Repositorio.Embarcador.Financeiro.DocumentoFaturamento ObterRepositorioDocumentoFaturamento()
        {
            if (_repositorioDocumentoFaturamento == null)
                _repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);

            return _repositorioDocumentoFaturamento;
        }

        private Repositorio.Embarcador.Escrituracao.PagamentoIntegracao ObterRepositorioPagamentoIntegracao()
        {
            if (_repositorioPagamentoIntegracao == null)
                _repositorioPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(_unitOfWork);

            return _repositorioPagamentoIntegracao;
        }

        private Repositorio.Embarcador.Escrituracao.DocumentoProvisao ObterRepositorioDocumentoProvisao()
        {
            if (_repositorioDocumentoProvisao == null)
                _repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(_unitOfWork);

            return _repositorioDocumentoProvisao;
        }

        private Repositorio.Embarcador.Escrituracao.Provisao ObterRepositorioProvisao()
        {
            if (_repositorioProvisao == null)
                _repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(_unitOfWork);

            return _repositorioProvisao;
        }

        public void LiberarDocumentosFaturamentoComProvisaoGerada()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracaoLiberar = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>() {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Camil
            };

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tiposIntegracao = repTipoIntegracao.BuscarPorTipos(tiposIntegracaoLiberar);

            if (tiposIntegracao.Count == 0)
                return;

            ObterRepositorioDocumentoFaturamento().LiberarDocumentosFaturamentoComProvisaoGerada();
        }

        private bool ValidarPeriodo(DateTime? dataInicial, DateTime? dataFinal)
        {
            bool retorno = false;

            if (dataInicial == DateTime.MinValue && dataFinal == DateTime.MinValue)
                throw new WebServiceException("Nenhum período informado!");

            if (dataInicial > dataFinal)
                throw new WebServiceException("Período inválido!");

            retorno = true;

            return retorno;
        }
        #endregion Métodos Privados
    }
}
