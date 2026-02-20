using Dominio.Entidades.Embarcador.Escrituracao;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Escrituracao;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repositorio;
using Repositorio.Embarcador.Financeiro;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/Pagamento")]
    public class PagamentoController : BaseController
    {
        #region Construtores

        public PagamentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Escrituracao.Provisao repProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioCanhoto.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

                if (!repTipoIntegracao.ExistePorTipo(TipoIntegracao.Unilever))
                {
                    Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = repProvisao.BuscarSeExisteProvisaoEmFechamento();
                    if (provisao != null)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Não é possível iniciar o pagamento enquanto a provisão " + provisao.Numero + " estiver em fechamento.");
                    }

                    Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao = repCancelamentoProvisao.BuscarSeExisteProvisaoEmCancelamento();
                    if (cancelamentoProvisao != null)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Não é possível iniciar o pagamento enquanto a provisão " + cancelamentoProvisao.Numero + " estiver em cancelamento.");
                    }
                }

                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento filtroPesquisaDocumento = ObterFiltroPesquisaDocumento(configuracaoCanhoto);

                Dominio.Entidades.Cliente tomador = null;
                Dominio.Entidades.Embarcador.Filiais.Filial filial = null;
                Dominio.Entidades.Empresa empresa = null;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = null;

                if (filtroPesquisaDocumento.CodigosTomador?.Count > 0)
                    tomador = repCliente.BuscarPorCPFCNPJ(filtroPesquisaDocumento.CodigosTomador.FirstOrDefault());
                if (filtroPesquisaDocumento.CodigosFilial?.Count > 0)
                    filial = repFilial.BuscarPorCodigo(filtroPesquisaDocumento.CodigosFilial.FirstOrDefault());
                if (filtroPesquisaDocumento.CodigosTransportador?.Count > 0)
                    empresa = repEmpresa.BuscarPorCodigo(filtroPesquisaDocumento.CodigosTransportador.FirstOrDefault());
                if (filtroPesquisaDocumento.CodigoCarga > 0)
                    carga = repCarga.BuscarPorCodigo(filtroPesquisaDocumento.CodigoCarga);
                if (filtroPesquisaDocumento.CodigoOcorrencia > 0)
                    ocorrencia = repCargaOcorrencia.BuscarPorCodigo(filtroPesquisaDocumento.CodigoOcorrencia);

                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = null;
                int pagamentosGerados = 1;

                IList<int> codigosDocumentosSelecionados = ObterCodigosDocumentosSelecionados(repDocumentoFaturamento, filtroPesquisaDocumento);

                bool existeDocumentoBloqueado = repDocumentoFaturamento.BuscarPorCodigos(codigosDocumentosSelecionados).Any(o => o.PagamentoDocumentoBloqueado);

                if (configuracaoFinanceiro.NaoPermitirGerarLotesPagamentosDocumentosBloqueados && existeDocumentoBloqueado)
                    return new JsonpResult(false, true, "Não é possível gerar um lote com documentos bloqueados.");

                if (configuracaoFinanceiro.GerarLotesPagamentoIndividuaisPorDocumento && repTipoIntegracao.ExistePorTipo(TipoIntegracao.Camil))
                {
                    foreach (int codigoDocumentoSelecionado in codigosDocumentosSelecionados)
                        pagamento = AdicionarPagamento(unitOfWork, filtroPesquisaDocumento, tomador, filial, empresa, carga, ocorrencia, codigoDocumentoSelecionado);

                    pagamentosGerados = codigosDocumentosSelecionados.Count;
                }
                else
                    pagamento = AdicionarPagamento(unitOfWork, filtroPesquisaDocumento, tomador, filial, empresa, carga, ocorrencia);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    Codigo = pagamentosGerados == 1 ? pagamento.Codigo : 0
                });
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private IList<int> ObterCodigosDocumentosSelecionados(DocumentoFaturamento repDocumentoFaturamento, FiltroPesquisaDocumento filtroPesquisaDocumento)
        {
            bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);
            IList<int> codigosDocumentosSelecionados;
            if (todosSelecionados)
            {
                TipoDocumentoGerado tipoDocumentoGerado = ObterTipoDocumentoGerado();
                List<int> codigosNaoSelecionadas = ObterCodigosDocumentosNaoSelecionados();

                codigosDocumentosSelecionados = repDocumentoFaturamento.ObterCodigosDocumentosParaPagamento(filtroPesquisaDocumento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.EmFechamento, codigosNaoSelecionadas, tipoDocumentoGerado);
            }
            else
            {
                codigosDocumentosSelecionados = new List<int>();
                dynamic listaSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosSelecionadas"));
                foreach (var dynSelecionada in listaSelecionadas)
                {
                    codigosDocumentosSelecionados.Add((int)dynSelecionada.Codigo);
                }
            }

            return codigosDocumentosSelecionados;
        }

        private Dominio.Entidades.Embarcador.Escrituracao.Pagamento AdicionarPagamento(UnitOfWork unitOfWork, FiltroPesquisaDocumento filtroPesquisaDocumento, Dominio.Entidades.Cliente tomador, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia, int codigoDocumentoIndividual = 0)
        {
            Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = Servicos.Embarcador.Escrituracao.Pagamento.AdicionarPagamento(filtroPesquisaDocumento.DataInicio, filtroPesquisaDocumento.DataFim, filtroPesquisaDocumento.DataInicialEmissao, filtroPesquisaDocumento.DataFinalEmissao, tomador, filial, empresa, carga, ocorrencia, false, filtroPesquisaDocumento.PagamentoLiberado, unitOfWork, Auditado);
            SetarDocumentosSelecionados(pagamento, filtroPesquisaDocumento, unitOfWork, codigoDocumentoIndividual);
            ValidarCargasNotasFilhas(pagamento, unitOfWork);
            ValidarNotaContaTransacao(pagamento, unitOfWork);

            return pagamento;
        }

        private void ValidarCargasNotasFilhas(Pagamento pagamento, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            List<int> codigosCargaPendentesNotasFilhas = repDocumentoFaturamento.BuscarCodigosCargasPorCodigoDocumentos(pagamento.Codigo);
            if (codigosCargaPendentesNotasFilhas.Count > 0)
            {
                foreach (int codigoCarga in codigosCargaPendentesNotasFilhas)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosPorCarga = repositorioPedidos.BuscarPorCarga(codigoCarga);
                    List<string> numeroControlePedidos = pedidosPorCarga.Select(p => p.NumeroControle).ToList();
                    bool existeNotaFilha = repositorioXmlNotaFiscal.BuscarPorNumeroControlePedido(numeroControlePedidos);

                    if (!existeNotaFilha)
                    {
                        string motivoRestricao = $"A carga com protocolo({codigoCarga}) ainda não recebeu as notas filhas.";
                        new Servicos.Embarcador.Escrituracao.Pagamento(unitOfWork).BloquearDocumentoComRestricao(codigoCarga, pagamento, motivoRestricao, unitOfWork);
                        throw new ControllerException(motivoRestricao);
                    }
                }
            }
        }

        private void ValidarNotaContaTransacao(Pagamento pagamento, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalContabilizacao repositorioXmlNotaFiscalContabilizacao = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalContabilizacao(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> cteDosDocumentos = repDocumentoFaturamento.BuscarCteDosDocumentoFaturamentoPorPagamento(pagamento.Codigo);
            foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in cteDosDocumentos)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao> xmlContabilicacao = repositorioXmlNotaFiscalContabilizacao.BuscarPorXMLNotasFiscais(cte.XMLNotaFiscais.Select(x => x.Codigo).ToList());
                bool existeNotaSemContaTransacao = xmlContabilicacao.Any(x => string.IsNullOrWhiteSpace(x.ContaTransacao));

                if (existeNotaSemContaTransacao)
                {
                    string motivoRestricao = $"Não é possivel gerar pagamento por que CTe {cte.Numero} não possui a nota de transferência";
                    new Servicos.Embarcador.Escrituracao.Pagamento(unitOfWork).BloquearDocumentoComRestricao(cte, pagamento, motivoRestricao, unitOfWork);
                    throw new ControllerException(motivoRestricao);
                }
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repPagamento.BuscarPorCodigo(codigo);

                // Valida
                if (pagamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    pagamento.Codigo,
                    pagamento.Situacao,
                    CargaEmCancelamento = pagamento.UltimaCargaEmCancelamento != null,
                    DataInicial = pagamento.DataInicial?.ToString("dd/MM/yyyy") ?? "",
                    DataFinal = pagamento.DataFinal?.ToString("dd/MM/yyyy") ?? "",
                    DataInicialEmissao = pagamento.DataInicialEmissao?.ToString("dd/MM/yyyy") ?? "",
                    DataFinalEmissao = pagamento.DataFinalEmissao?.ToString("dd/MM/yyyy") ?? "",
                    pagamento.Numero,
                    pagamento.GerandoMovimentoFinanceiro,
                    pagamento.MotivoRejeicaoFechamentoPagamento,
                    Filial = pagamento.Filial != null ? new { pagamento.Filial.Codigo, pagamento.Filial.Descricao } : new { Codigo = 0, Descricao = "" },
                    Transportador = new { Codigo = pagamento.Empresa?.Codigo ?? 0, Descricao = pagamento.Empresa?.Descricao ?? "" },
                    Tomador = new { Codigo = pagamento.Tomador?.Codigo ?? 0, Descricao = pagamento.Tomador?.Nome ?? "" },
                    Carga = new { Codigo = pagamento.Carga?.Codigo ?? 0, Descricao = pagamento.Carga?.Descricao ?? "" },
                    Ocorrencia = new { Codigo = pagamento.CargaOcorrencia?.Codigo ?? 0, Descricao = pagamento.CargaOcorrencia?.Descricao ?? "" },
                    pagamento.DescricaoSituacao,
                    Resumo = new
                    {
                        pagamento.Codigo,
                        Transportador = pagamento.Empresa?.Descricao ?? string.Empty,
                        Filial = pagamento.Filial?.Descricao ?? string.Empty,
                        Numero = pagamento.Numero.ToString(),
                        DataInicial = pagamento.DataInicial?.ToString("dd/MM/yyyy") ?? string.Empty,
                        DataFinal = pagamento.DataFinal?.ToString("dd/MM/yyyy") ?? string.Empty,
                        DataInicialEmissao = pagamento.DataInicialEmissao?.ToString("dd/MM/yyyy") ?? "",
                        DataFinalEmissao = pagamento.DataFinalEmissao?.ToString("dd/MM/yyyy") ?? "",
                        Carga = pagamento.Carga?.Descricao ?? "",
                        Ocorrencia = pagamento.CargaOcorrencia?.Descricao ?? "",
                        Situacao = pagamento.DescricaoSituacao,
                        QuantidadeDocumentos = pagamento.QuantidadeDocsPagamento.ToString(),
                        ValorPagamento = pagamento.ValorPagamento.ToString("n2"),
                    }
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarResumoAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPagamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escrituracao.Pagamento repositorioPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repositorioPagamento.BuscarPorCodigo(codigoPagamento);

                if (pagamento == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                if (pagamento.Situacao == SituacaoPagamento.SemRegraAprovacao)
                    return new JsonpResult(new
                    {
                        pagamento.Codigo,
                        DescricaoSituacao = pagamento.Situacao.ObterDescricao(),
                        PossuiAlcada = true,
                        PossuiRegras = false
                    });

                Repositorio.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento repositorioAprovacao = new Repositorio.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento(unitOfWork);
                int totalRegistros = repositorioAprovacao.ContarAutorizacoes(pagamento.Codigo);

                if (totalRegistros == 0)
                    return new JsonpResult(new
                    {
                        pagamento.Codigo,
                        PossuiAlcada = false
                    });

                int aprovacoes = repositorioAprovacao.ContarAprovacoes(pagamento.Codigo);
                int aprovacoesNecessarias = repositorioAprovacao.ContarAprovacoesNecessarias(pagamento.Codigo);
                int reprovacoes = repositorioAprovacao.ContarReprovacoes(pagamento.Codigo);

                return new JsonpResult(new
                {
                    pagamento.Codigo,
                    AprovacoesNecessarias = aprovacoesNecessarias,
                    Aprovacoes = aprovacoes,
                    Reprovacoes = reprovacoes,
                    DescricaoSituacao = pagamento.Situacao.ObterDescricao(),
                    PossuiAlcada = true,
                    PossuiRegras = true
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento repositorioAprovacao = new Repositorio.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento autorizacao = repositorioAprovacao.BuscarPorCodigo(codigo);

                if (autorizacao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    autorizacao.Codigo,
                    Regra = autorizacao.Descricao,
                    Situacao = autorizacao.Situacao.ObterDescricao(),
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,
                    PodeAprovar = autorizacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo),
                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                // Manipula grids
                Models.Grid.Grid grid = ObterGridPesquisa(configuracao);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                ObterPropriedadeOrdenar(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, false, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaDocumentos()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaDocumento(exportarPesquisa: true);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Models.Grid.Grid grid = ObterGridPesquisa(configuracao);
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                ObterPropriedadeOrdenar(ref propOrdenar);

                int totalRegistros = 0;
                dynamic lista = ExecutaPesquisa(ref totalRegistros, false, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Prioridade", "PrioridadeAprovacao", 20, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.center, false);

                int codigoPagamento = Request.GetIntParam("Codigo");
                int totalRegistros = 0;
                List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento> listaAutorizacao;

                if (codigoPagamento > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                    Repositorio.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento repositorioAprovacao = new Repositorio.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento(unitOfWork);
                    totalRegistros = repositorioAprovacao.ContarAutorizacoes(codigoPagamento);
                    listaAutorizacao = totalRegistros > 0 ? repositorioAprovacao.ConsultarAutorizacoes(codigoPagamento, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento>();
                }
                else
                    listaAutorizacao = new List<Dominio.Entidades.Embarcador.Escrituracao.AlcadasPagamento.AprovacaoAlcadaPagamento>();

                var lista = (
                    from autorizacao in listaAutorizacao
                    select new
                    {
                        autorizacao.Codigo,
                        PrioridadeAprovacao = autorizacao.RegraAutorizacao?.PrioridadeAprovacao ?? 0,
                        Situacao = autorizacao.Situacao.ObterDescricao(),
                        Usuario = autorizacao.Usuario?.Nome,
                        Regra = autorizacao.Descricao,
                        Data = autorizacao.Data.HasValue ? autorizacao.Data.ToString() : string.Empty,
                        Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
                        DT_RowColor = autorizacao.ObterCorGrid()
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDocumento()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaDocumento(exportarPesquisa: false));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escrituracao.Pagamento repositorio = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repositorio.BuscarPorCodigo(codigo);

                if (pagamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (pagamento.Situacao != SituacaoPagamento.SemRegraAprovacao)
                    return new JsonpResult(false, true, "A situação não permite esta operação.");

                unitOfWork.Start();

                new Servicos.Embarcador.Escrituracao.PagamentoAprovacao(unitOfWork, ClienteAcesso?.URLAcesso).CriarAprovacao(pagamento, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult(pagamento.Situacao != SituacaoPagamento.SemRegraAprovacao);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao reprocessar as regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
                Repositorio.Embarcador.Escrituracao.PagamentoIntegracao repPagamentoIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoIntegracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> pagamentosPendentes = repPagamento.BuscarPagamentosFalhaIntegracao();

                if (pagamentosPendentes.Count == 0)
                    return new JsonpResult(false, true, "Não há integrações com falha.");

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento in pagamentosPendentes)
                {
                    pagamento.Situacao = SituacaoPagamento.AguardandoIntegracao;

                    if (pagamento.Integracoes != null)
                    {
                        IList<PagamentoIntegracao> integracoesPendentes = pagamento.Integracoes
                            .Where(integracao => integracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                            .ToList();

                        foreach (PagamentoIntegracao integracaoPendente in integracoesPendentes)
                        {
                            integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                            integracaoPendente.ProblemaIntegracao = string.Empty;

                            repPagamentoIntegracao.Atualizar(integracaoPendente);
                        }
                    }

                    repPagamento.Atualizar(pagamento);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BloquearDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoDocumento = Request.GetIntParam("Codigo");
                string motivo = Request.GetStringParam("MotivoBloqueio");

                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repositorioDocumentoFaturamento.BuscarPorCodigo(codigoDocumento);

                if (documentoFaturamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                documentoFaturamento.PagamentoDocumentoBloqueado = true;
                documentoFaturamento.MotivoBloqueio = motivo;
                repositorioDocumentoFaturamento.Atualizar(documentoFaturamento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao bloquear o documento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DesbloquearDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoDocumento = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repositorioDocumentoFaturamento.BuscarPorCodigo(codigoDocumento);

                if (documentoFaturamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                documentoFaturamento.PagamentoDocumentoBloqueado = false;
                documentoFaturamento.MotivoBloqueio = string.Empty;
                repositorioDocumentoFaturamento.Atualizar(documentoFaturamento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao desbloquear o documento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DesbloquearDocumentoLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentosFaturamento = ObterDocumentosSelecionados(unitOfWork);

                if (documentosFaturamento.Count == 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar os registro.");

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documento in documentosFaturamento)
                {
                    documento.PagamentoDocumentoBloqueado = false;
                    documento.MotivoBloqueio = string.Empty;
                    repDocumentoFaturamento.Atualizar(documento);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao desbloquear os documentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDocumentosSelecionados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new DocumentoFaturamento(unitOfWork);

                List<int> codigosTitulos = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaDocumentos"));

                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> listaTitulos = repositorioDocumentoFaturamento.BuscarPorCodigos(codigosTitulos);

                return new JsonpResult(new
                {
                    ValorTotalPendente = listaTitulos.Count > 0 ? listaTitulos.Select(obj => obj.ValorDocumento).Sum().ToString("n2") : 0.ToString("n2"),
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarSomaTotalDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioCanhoto.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento filtroPesquisa = ObterFiltroPesquisaDocumento(configuracaoCanhoto);

                decimal totalRegistros = repositorioDocumentoFaturamento.SomarValorPagamentoTotalConsulta(filtroPesquisa.CodigoPagamento > 0 ? new List<int> { filtroPesquisa.CodigoPagamento } : new List<int>(), 0, filtroPesquisa, false, false, ConfiguracaoEmbarcador.GerarSomenteDocumentosDesbloqueados);

                return new JsonpResult(new
                {
                    ValorTotalDocumentos = totalRegistros.ToString("n2")
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> ObterDocumentosSelecionados(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioCanhoto.BuscarConfiguracaoPadrao();
            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento filtroPesquisa = ObterFiltroPesquisaDocumento(configuracaoCanhoto);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> documentoSelecionados = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");
            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosNaoSelecionados"));

                documentoSelecionados = repDocumentoFaturamento.Consultar(filtroPesquisa.CodigoPagamento > 0 ? new List<int> { filtroPesquisa.CodigoPagamento } : new List<int>(), 0, filtroPesquisa, false, false, ConfiguracaoEmbarcador.GerarSomenteDocumentosDesbloqueados, new ParametroConsulta { DirecaoOrdenar = "Codigo" }, null);

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    documentoSelecionados.Remove(repDocumentoFaturamento.BuscarPorCodigo((int)itemNaoSelecionado.Codigo));
            }
            else
            {
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosSelecionados"));

                foreach (var itemSelecionado in listaItensSelecionados)
                    documentoSelecionados.Add(repDocumentoFaturamento.BuscarPorCodigo((int)itemSelecionado.Codigo));
            }

            return documentoSelecionados;
        }

        private List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> BuscarDocumentosSelecionados(Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            erro = string.Empty;

            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> listaBusca = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();

            // Valida filtros
            int transportador = 1;
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                int.TryParse(Request.Params("Transportador"), out transportador);

            int.TryParse(Request.Params("Filial"), out int filial);
            double.TryParse(Request.Params("Tomador"), out double tomador);

            if (transportador == 0)
            {
                erro = "Transportador é obrigatórios.";
                return null;
            }

            bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);
            if (todosSelecionados)
            {
                // Reconsulta com os mesmos dados e remove apenas os desselecionados
                try
                {
                    int totalRegistros = 0;
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                    {
                        DirecaoOrdenar = "Codigo"
                    };
                    ExecutaPesquisaDocumento(ref listaBusca, ref totalRegistros, parametrosConsulta, exportarPesquisa: false, unitOfWork: unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                // Iterar ocorrencias desselecionados e remove da lista
                dynamic listaNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosNaoSelecionadas"));
                foreach (var dynNaoSelecionada in listaNaoSelecionadas)
                    listaBusca.Remove(new Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento() { Codigo = (int)dynNaoSelecionada.Codigo });
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosSelecionadas"));
                foreach (var dynSelecionada in listaSelecionadas)
                    listaBusca.Add(repDocumentoFaturamento.BuscarPorCodigo((int)dynSelecionada.Codigo, false));
            }

            // Retorna lista
            return listaBusca;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, bool somenteAtivo, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao repositorioPagamentoEDIIntegracao = new Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao(unitOfWork);
            Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaPagamento filtroPesquisa = ObterFiltroPesquisaPagamento();
            List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento> listaGrid = repPagamento.Consultar(filtroPesquisa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repPagamento.ContarConsulta(filtroPesquisa);

            List<int> codigoPagamento = listaGrid.Select(o => o.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao> listaPagamentoEDIIntegracao = repositorioPagamentoEDIIntegracao.BuscarPorPagamentos(codigoPagamento).ToList();

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Numero = obj.Numero.ToString(),
                            Descricao = obj.Descricao,
                            LotePagamentoLiberado = obj.LotePagamentoLiberado ? "Sim" : "Não",
                            DataInicialEmissao = obj.LotePagamentoLiberado && obj.DocumentosFaturamentoLiberado.Count > 0 ? obj.DocumentosFaturamentoLiberado.Min(doc => doc.DataEmissao).ToString("dd/MM/yyyy") : obj.DocumentosFaturamento.Count > 0 ? obj.DocumentosFaturamento.Min(doc => doc.DataEmissao).ToString("dd/MM/yyyy") : "",
                            DataFinalEmissao = obj.LotePagamentoLiberado && obj.DocumentosFaturamentoLiberado.Count > 0 ? obj.DocumentosFaturamentoLiberado.Max(doc => doc.DataEmissao).ToString("dd/MM/yyyy") : obj.DocumentosFaturamento.Count > 0 ? obj.DocumentosFaturamento.Max(doc => doc.DataEmissao).ToString("dd/MM/yyyy") : "",
                            Tomador = obj.Tomador?.Descricao ?? "",
                            QuantidadeDocumentos = obj.QuantidadeDocsPagamento,
                            ValorPagamento = obj.ValorPagamento.ToString("n2"),
                            Transportador = obj.Empresa?.Descricao ?? "",
                            Filial = obj.Filial?.Descricao ?? "",
                            obj.DescricaoSituacao,
                            NomeArquivo = string.Join(", ", listaPagamentoEDIIntegracao.Where(p => p.Pagamento.Codigo == obj.Codigo).Select(obj => obj.NomeArquivo)),
                        };

            return lista.ToList();
        }

        private dynamic ExecutaPesquisaDocumento(ref List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> listaGrid, ref int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, bool exportarPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
            Repositorio.Embarcador.Documentos.ControleDocumento repositorioControleDocumentos = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);
            Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioGuiaRecolhimentoTributoEstadual = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoCte = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repositorioModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repositorioConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);

            List<dynamic> listaRetornar = new List<dynamic>();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioCanhoto.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repositorioConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();

            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento filtroPesquisa = ObterFiltroPesquisaDocumento(configuracaoCanhoto);

            if (filtroPesquisa.CodigoPagamento > 0)
            {
                filtroPesquisa.SituacaoPagamentoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Todos;
                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repPagamento.BuscarPorCodigo(filtroPesquisa.CodigoPagamento);
                filtroPesquisa.PagamentoLiberado = pagamento.LotePagamentoLiberado;
            }

            if ((configuracaoFinanceiro?.GerarLotePagamentoSomenteParaCTe ?? false) && filtroPesquisa.CodigoPagamento == 0)
                filtroPesquisa.ModeloDocumentoFiscal = repositorioModeloDocumentoFiscal.BuscarPorModelo("57")?.Codigo ?? 0;

            listaGrid = repositorioDocumentoFaturamento.Consultar(filtroPesquisa.CodigoPagamento > 0 ? new List<int> { filtroPesquisa.CodigoPagamento } : new List<int>(), 0, filtroPesquisa, false, false, ConfiguracaoEmbarcador.GerarSomenteDocumentosDesbloqueados, parametrosConsulta, null);
            totalRegistros = repositorioDocumentoFaturamento.ContarConsulta(filtroPesquisa.CodigoPagamento > 0 ? new List<int> { filtroPesquisa.CodigoPagamento } : new List<int>(), 0, filtroPesquisa, false, false, ConfiguracaoEmbarcador.GerarSomenteDocumentosDesbloqueados, null);

            if (listaGrid.Count > 0)
            {
                string nomeArquivoEDI = ObterNomeArquivoEDI(unitOfWork, filtroPesquisa.CodigoPagamento, exportarPesquisa);

                if (configuracaoCanhoto.LiberarParaPagamentoAposDigitalizacaCanhoto)
                {
                    listaGrid = new Servicos.Embarcador.Financeiro.Pagamento(unitOfWork).ObterDocumentosPagarGeracaoAutomaticaDoPagamento(listaGrid);
                    totalRegistros = listaGrid.Count;

                }
                foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documento in listaGrid)
                {
                    listaRetornar.Add(new
                    {
                        TipoDocumentoEmissao = documento.ModeloDocumentoFiscal?.TipoDocumentoEmissao ?? Dominio.Enumeradores.TipoDocumento.Nenhum,
                        SituacaoCTe = documento.CTe?.Status ?? "",
                        CodigoEmpresa = documento.Empresa?.Codigo ?? 0,
                        CodigoCTE = documento.CTe?.Codigo ?? 0,
                        documento.Codigo,
                        CodigoFilial = documento.Filial?.Codigo ?? 0,
                        CodigoTomador = documento.Tomador?.Codigo ?? 0,
                        CodigoTransportador = documento.Empresa?.Codigo ?? 0,
                        Transportador = documento.Empresa?.Descricao ?? "",
                        Documento = documento.Numero + " - " + documento.EmpresaSerie?.Numero.ToString() ?? "",
                        Tipo = documento.ModeloDocumentoFiscal?.Abreviacao ?? "",
                        Carga = documento.CargaPagamento?.CodigoCargaEmbarcador ?? "",
                        Ocorrencia = documento.CargaOcorrenciaPagamento?.NumeroOcorrencia.ToString() ?? "",
                        FechamentoFrete = documento.FechamentoFrete?.Numero.ToString() ?? "",
                        DataEmissao = documento.DataEmissao.ToString("dd/MM/yyyy"),
                        DataLiberacaoPagamento = documento.DataLiberacaoPagamento?.ToString("dd/MM/yyyy") ?? "",
                        Tomador = documento.Tomador?.Descricao ?? "",
                        Filial = documento.Filial?.Descricao ?? "",
                        TipoDocumentoCreditoDebito = documento.CTe?.ModeloDocumentoFiscal?.TipoDocumentoCreditoDebitoDescricao ?? "",
                        ValorFrete = documento.ValorDocumento.ToString("n2"),
                        NomeArquivoEDI = nomeArquivoEDI,
                        BloqueioGeracaoAutomatica = documento.BloqueioGeracaoAutomaticaPagamento.ObterDescricao(),
                        SituacaoBloqueio = documento.PagamentoDocumentoBloqueado ? "Bloqueado" : "Liberado",
                        MotivoBloqueio = documento?.MotivoBloqueio ?? string.Empty,
                    });
                }
            }

            return listaRetornar;
        }

        private Models.Grid.Grid ObterGridPesquisa(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descricao", false);
            grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
            if (configuracao.GerarPagamentoBloqueado && !configuracao.GerarSomenteDocumentosDesbloqueados)
                grid.AdicionarCabecalho("Pagamento Liberado", "LotePagamentoLiberado", 10, Models.Grid.Align.center, true);

            grid.AdicionarCabecalho("Data Inicial Emissão", "DataInicialEmissao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Final Emissão", "DataFinalEmissao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Qnt. Docs", "QuantidadeDocumentos", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Valor", "ValorPagamento", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Nome Arquivo", "NomeArquivo", 10, Models.Grid.Align.center, false);

            return grid;
        }

        private Models.Grid.Grid ObterGridPesquisaDocumento(bool exportarPesquisa)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Fechamento.FechamentoFrete repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfigCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfigCanhoto.BuscarPrimeiroRegistro();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Filial", false);
                grid.AdicionarCabecalho("CodigoFilial", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("CodigoTomador", false);
                grid.AdicionarCabecalho("TipoDocumentoEmissao", false);
                grid.AdicionarCabecalho("CodigoTransportador", false);
                grid.AdicionarCabecalho("Documento", "Documento", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Carga", "Carga", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Ocorrência", "Ocorrencia", 8, Models.Grid.Align.left, true);

                if (repFechamentoFrete.VerificarExistemFechamento())
                    grid.AdicionarCabecalho("Fechamento de Contrato", "FechamentoFrete", 8, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Emissão NF-e", "DataEmissao", 8, Models.Grid.Align.center, true);

                if (ConfiguracaoEmbarcador.GerarPagamentoBloqueado)
                    grid.AdicionarCabecalho("Data Liberação", "DataLiberacaoPagamento", 8, Models.Grid.Align.center, true);

                if (configFinanceiro.NaoGerarAutomaticamenteLotesCancelados)
                    grid.AdicionarCabecalho("Bloq automático", "BloqueioGeracaoAutomatica", 8, Models.Grid.Align.center, true);

                grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Frete", "ValorFrete", 10, Models.Grid.Align.right, true);

                if (configuracaoCanhoto?.PermitirBloquearDocumentoManualmente ?? false)
                {
                    grid.AdicionarCabecalho("Situação", "SituacaoBloqueio", 10, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("Motivo Bloqueio", "MotivoBloqueio", 10, Models.Grid.Align.left, true);
                }

                if (exportarPesquisa)
                    grid.AdicionarCabecalho("Nome do Arquivo", "NomeArquivoEDI", 20, Models.Grid.Align.left);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarDocumentos);
                List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento> listaGrid = new List<Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento>();
                int totalRegistros = 0;
                var lista = ExecutaPesquisaDocumento(ref listaGrid, ref totalRegistros, parametrosConsulta, exportarPesquisa, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterNomeArquivoEDI(Repositorio.UnitOfWork unitOfWork, int codigoPagamento, bool exportarPesquisa)
        {
            if (exportarPesquisa & (codigoPagamento > 0))
            {
                Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao repositorio = new Repositorio.Embarcador.Escrituracao.PagamentoEDIIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao integracao = repositorio.BuscarPorPagamento(codigoPagamento).FirstOrDefault();

                if (integracao != null)
                    return Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(integracao, false, unitOfWork);
            }

            return "";
        }

        private void ObterPropriedadeOrdenar(ref string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Tomador")
                propriedadeOrdenar = "Tomador.Nome";
            else if (propriedadeOrdenar == "Transportador")
                propriedadeOrdenar = "Empresa.RazaoSocial";
            else if (propriedadeOrdenar == "DataInicio")
                propriedadeOrdenar = "DataInicial";
            else if (propriedadeOrdenar == "DataFim")
                propriedadeOrdenar = "DataFinal";
            else if (propriedadeOrdenar == "QuantidadeDocumentos")
                propriedadeOrdenar = "QuantidadeDocsPagamento";
            else if (propriedadeOrdenar == "DescricaoSituacao")
                propriedadeOrdenar = "Situacao";
            else if (propriedadeOrdenar == "Filial")
                propriedadeOrdenar = "Filial.Descricao";
        }

        private string ObterPropriedadeOrdenarDocumentos(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Documento")
                propriedadeOrdenar = "Numero";
            else if (propriedadeOrdenar == "FechamentoFrete")
                return "FechamentoFrete.Numero";
            else if (propriedadeOrdenar == "Carga")
                propriedadeOrdenar = "CargaPagamento.CodigoCargaEmbarcador";
            else if (propriedadeOrdenar == "Ocorrencia")
                propriedadeOrdenar = "CargaOcorrenciaPagamento.NumeroOcorrencia";
            else if (propriedadeOrdenar == "Tipo")
                propriedadeOrdenar = "ModeloDocumentoFiscal.Abreviacao";
            else if (propriedadeOrdenar == "Destinatario")
                propriedadeOrdenar = "Destinatario.Nome";
            else if (propriedadeOrdenar == "Tomador")
                propriedadeOrdenar = "Tomador.Nome";
            else if (propriedadeOrdenar == "Filial")
                propriedadeOrdenar = "Filial.Descricao";
            else if (propriedadeOrdenar == "Transportador")
                propriedadeOrdenar = "Empresa.RazaoSocial";
            else if (propriedadeOrdenar == "ValorFrete")
                propriedadeOrdenar = "ValorAFaturar";
            else if (propriedadeOrdenar == "TipoDocumentoCreditoDebito")
                propriedadeOrdenar = "CTe.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito";

            return propriedadeOrdenar;
        }

        private void SetarDocumentosSelecionados(Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento, FiltroPesquisaDocumento filtroPesquisaDocumento, Repositorio.UnitOfWork unitOfWork, int codigoDocumentoIndividual = 0)
        {
            Repositorio.Embarcador.Escrituracao.Pagamento repPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
            Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repConfiguracaoCanhoto.BuscarConfiguracaoPadrao();

            List<int> codigosDocumentoPagamento = new List<int>();
            List<int> listaDocumentosSelecionados = new List<int>();
            //totalRegistros = repDocumentoFaturamento.ContarConsulta(codigoPagamento, somenteSemPagamento, concatenarComDocumentosSemPrevisao, dataInicio, dataFim, transportador, filial, tomador, situacaoPagamentoDocumento);

            int totalRegistros = 0;
            decimal valorPagamento = 0;
            bool.TryParse(Request.Params("SelecionarTodos"), out bool todosSelecionados);
            TipoDocumentoGerado tipoDocumentoGerado = ObterTipoDocumentoGerado();

            if (todosSelecionados && codigoDocumentoIndividual == 0)
            {
                List<int> codigosNaoSelecionadas = ObterCodigosDocumentosNaoSelecionados();

                if (!pagamento.LotePagamentoLiberado)
                {
                    pagamento.QuantidadeDocsPagamento = repDocumentoFaturamento.SetarDocumentosParaPagamento(pagamento.Codigo, filtroPesquisaDocumento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.EmFechamento, codigosNaoSelecionadas, tipoDocumentoGerado);
                    pagamento.ValorPagamento = repDocumentoFaturamento.ValorTotalPorPagamento(pagamento.Codigo);

                    listaDocumentosSelecionados.AddRange(repDocumentoFaturamento.BuscarListaCodigosPagamentoNaoSelecionados(pagamento.Codigo, filtroPesquisaDocumento, codigosNaoSelecionadas, tipoDocumentoGerado));
                }
                else
                {
                    pagamento.QuantidadeDocsPagamento = repDocumentoFaturamento.SetarDocumentosParaPagamentoLiberado(pagamento.Codigo, filtroPesquisaDocumento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.EmFechamento, codigosNaoSelecionadas);
                    pagamento.ValorPagamento = repDocumentoFaturamento.ValorTotalPorPagamentoLiberado(pagamento.Codigo);
                    codigosDocumentoPagamento = repDocumentoFaturamento.BuscarCodigosPorPagamentoLiberado(pagamento.Codigo);

                    listaDocumentosSelecionados.AddRange(repDocumentoFaturamento.BuscarListaCodigosPagamentoNaoSelecionados(pagamento.Codigo, filtroPesquisaDocumento, codigosNaoSelecionadas, tipoDocumentoGerado));
                }
            }
            else
            {
                // Busca apenas itens selecionados
                dynamic listaSelecionadas;
                if (codigoDocumentoIndividual == 0)
                    listaSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosSelecionadas"));
                else
                {
                    listaSelecionadas = new List<object>();
                    listaSelecionadas.Add(new
                    {
                        Codigo = codigoDocumentoIndividual
                    });
                }

                foreach (var dynSelecionada in listaSelecionadas)
                {
                    Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repDocumentoFaturamento.BuscarPorCodigo((int)dynSelecionada.Codigo, false);
                    if (!pagamento.LotePagamentoLiberado)
                    {
                        documentoFaturamento.Pagamento = pagamento;
                        documentoFaturamento.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.EmFechamento;
                    }
                    else
                    {
                        documentoFaturamento.PagamentoLiberacao = pagamento;
                        codigosDocumentoPagamento.Add(documentoFaturamento.Codigo);
                    }

                    repDocumentoFaturamento.Atualizar(documentoFaturamento);
                    totalRegistros++;
                    valorPagamento += documentoFaturamento.ValorDocumento;

                    listaDocumentosSelecionados.Add((int)dynSelecionada.Codigo);
                }
                pagamento.ValorPagamento = valorPagamento;
                pagamento.QuantidadeDocsPagamento = totalRegistros;
            }

            if (configuracaoCanhoto.PermitirBloquearDocumentoManualmente && repDocumentoFaturamento.PossuiDocumentoBloqueado(listaDocumentosSelecionados))
                throw new ControllerException($"Há documento(s) bloqueado(s) na seleção. Consulte motivo na tabela.");

            pagamento.GerandoMovimentoFinanceiro = true;
            repPagamento.Atualizar(pagamento);

            if (pagamento.LotePagamentoLiberado)
            {
                if (codigosDocumentoPagamento.Count < 2000)
                    repDocumentoContabil.SetarPagamentosLiberacaoDocumentoContabil(codigosDocumentoPagamento, pagamento.Codigo);
                else
                {
                    foreach (int codigo in codigosDocumentoPagamento)
                        repDocumentoContabil.SetarPagamentoLiberacaoDocumentoContabil(codigo, pagamento.Codigo);
                }
            }

        }

        private TipoDocumentoGerado ObterTipoDocumentoGerado()
        {
            return Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoGerado>("TipoDocumentoGerado");
        }

        private List<int> ObterCodigosDocumentosNaoSelecionados()
        {
            List<int> codigosNaoSelecionadas = new List<int>();
            dynamic listaNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosNaoSelecionadas"));
            foreach (var dynNaoSelecionada in listaNaoSelecionadas)
                codigosNaoSelecionadas.Add((int)dynNaoSelecionada.Codigo);
            return codigosNaoSelecionadas;
        }

        private Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaPagamento ObterFiltroPesquisaPagamento()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaPagamento()
            {
                Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Empresa.Codigo : Request.GetIntParam("Empresa"),
                DataInicialEmissao = Request.GetDateTimeParam("DataInicialEmissao"),
                DataFinalEmissao = Request.GetDateTimeParam("DataFinalEmissao"),
                Filial = Request.GetIntParam("Filial"),
                Carga = Request.GetIntParam("Carga"),
                Ocorrencia = Request.GetIntParam("Ocorrencia"),
                Numero = Request.GetIntParam("Numero"),
                NumeroDOC = Request.GetIntParam("NumeroDOC"),
                LocalidadePrestacao = Request.GetIntParam("LocalidadePrestacao"),
                Tomador = Request.GetDoubleParam("Tomador"),
                ModeloDocumentoFiscal = Request.GetIntParam("ModeloDocumentoFiscal"),
                PagamentoLiberado = Request.GetBoolParam("PagamentoLiberado"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                SituacaoPagamento = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento>("Situacao")
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento ObterFiltroPesquisaDocumento(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto)
        {
            Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento filtro = new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaDocumento()
            {
                CodigoPagamento = Request.GetIntParam("Codigo"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                DataInicialEmissao = Request.GetDateTimeParam("DataInicialEmissao"),
                DataFinalEmissao = Request.GetDateTimeParam("DataFinalEmissao"),
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoOcorrencia = Request.GetIntParam("Ocorrencia"),
                ModeloDocumentoFiscal = Request.GetIntParam("ModeloDocumentoFiscal"),
                PagamentoLiberado = Request.GetBoolParam("PagamentoLiberado"),
                TipoOperacaoDocumento = Request.GetIntParam("TipoOperacaoDocumento"),
                PagamentoFinalizados = false,
                SituacaoPagamentoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoFaturamento.Autorizado,
                TipoDocumentoGerado = Request.GetNullableEnumParam<TipoDocumentoGerado>("TipoDocumentoGerado"),
                SituacaoDocumentoPagamento = Request.GetNullableEnumParam<SituacaoDocumentoPagamento>("SituacaoDocumento"),
            };

            if (configuracaoCanhoto?.PermitirMultiplaSelecaoLancamentoLotePagamento ?? false)
            {
                filtro.CodigosTransportador = Request.GetListParam<int>("Transportador");
                filtro.CodigosFilial = Request.GetListParam<int>("Filial");
                filtro.CodigosTomador = Request.GetListParam<double>("Tomador");
                filtro.CodigosTiposOperacao = Request.GetListParam<int>("TipoOperacao");
            }
            else
            {
                int codigoTransportador = Request.GetIntParam("Transportador");
                if (codigoTransportador > 0)
                    filtro.CodigosTransportador = new List<int> { codigoTransportador };

                int codigoFilial = Request.GetIntParam("Filial");
                if (codigoFilial > 0)
                    filtro.CodigosFilial = new List<int> { codigoFilial };

                double codigoTomador = Request.GetDoubleParam("Tomador");
                if (codigoTomador > 0D)
                    filtro.CodigosTomador = new List<double> { codigoTomador };

                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                if (codigoTipoOperacao > 0)
                    filtro.CodigosTiposOperacao = new List<int> { codigoTipoOperacao };
            }

            return filtro;
        }
        #endregion
    }
}
