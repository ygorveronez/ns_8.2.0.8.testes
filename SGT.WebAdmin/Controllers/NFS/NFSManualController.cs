using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System.Linq.Dynamic.Core;

namespace SGT.WebAdmin.Controllers.NFS
{
    [CustomAuthorize(new string[] { "DownloadXML", "DownloadDANFSE", "DownloadAnexo", "PesquisaAutorizacoes", "DetalhesAutorizacao", "ConsultarNFS" }, "NFS/NFSManual")]
    public class NFSManualController : BaseController
    {
        #region Construtores

        public NFSManualController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
                bool usaValorResidual = repTabelaFrete.ExisteTabelaResidual();

                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(usaValorResidual);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, false, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaParaCancelamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
                bool usaValorResidual = repTabelaFrete.ExisteTabelaResidual();
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(usaValorResidual);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, true, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return ObterGridPesquisaDocumento(unitOfWork);
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
                return ObterGridPesquisaDocumento(unitOfWork, true);
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

        public async Task<IActionResult> ConfiguracaoImportacaoValorFrete()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoValorFrete();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ImportarValorFretePorNumeroPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoValorFrete();
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);


                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                        List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> listaDocumentos = null;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna NumeroPedido = (from obj in linha.Colunas where obj.NomeCampo == "NumeroPedido" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna ValorDesconto = (from obj in linha.Colunas where obj.NomeCampo == "ValorFrete" select obj).FirstOrDefault();

                        if (NumeroPedido != null)
                            listaDocumentos = repositorioCargaDocumentoParaEmissaoNFSManual.BuscarListaDocumentosPorNumeroPedidoDescontoFrete(NumeroPedido.Valor);


                        if (listaDocumentos != null && listaDocumentos.Count > 0)
                        {
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual doc in listaDocumentos)
                            {
                                if (ValorDesconto != null)
                                {
                                    doc.ValorFrete = Utilidades.Decimal.Converter(ValorDesconto.Valor);
                                    doc.AlterouValorFreteNFsManual = true;
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, doc, null, "Alterou valor frete NFS Manual por importacao de arquivo", unitOfWork);
                                    repositorioCargaDocumentoParaEmissaoNFSManual.Atualizar(doc);
                                }
                            }
                        }

                        contador++;
                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" };
                        retornoImportacao.Retornolinhas.Add(retornoLinha);

                        unitOfWork.CommitChanges();

                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorreu uma falha ao processar a linha.", i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarValorFreteNFSManualmente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                if (configuracaoEmbarcador.PermiteImportarPlanilhaValoresFreteNFSManual)
                {
                    Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual Documento = null;

                    int codigoDocumento = Request.GetIntParam("Codigo");
                    decimal valorFrete = Request.GetDecimalParam("Valor");

                    Documento = repositorioCargaDocumentoParaEmissaoNFSManual.BuscarPorCodigo(codigoDocumento, true);

                    if (Documento == null)
                        return new JsonpResult(false, false, "Documento não encontrado.");

                    unitOfWork.Start();

                    Documento.ValorFrete = valorFrete;
                    Documento.AlterouValorFreteNFsManual = true;
                    repositorioCargaDocumentoParaEmissaoNFSManual.Atualizar(Documento);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, Documento, null, "Alterou Valor Frete NFs Manual.", unitOfWork);

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a observação do título.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDescontos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Carga", "Carga", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 50, Models.Grid.Align.right, true);

                int codigoLancamentoNFSManual = Request.GetIntParam("Codigo");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarDescontos);
                Repositorio.Embarcador.NFS.LancamentoNFSManualDesconto repositorioLancamentoNFSManualDesconto = new Repositorio.Embarcador.NFS.LancamentoNFSManualDesconto(unitOfWork);
                int totalRegistros = repositorioLancamentoNFSManualDesconto.ContarConsulta(codigoLancamentoNFSManual);
                List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManualDesconto> listaDescontos = (totalRegistros > 0) ? repositorioLancamentoNFSManualDesconto.Consultar(codigoLancamentoNFSManual, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManualDesconto>();

                var listaDescontosRetornar = (
                    from o in listaDescontos
                    select new
                    {
                        o.Codigo,
                        Carga = o.Carga.CodigoCargaEmbarcador,
                        Valor = o.Carga.ValorFreteResidual.ToString("n2")
                    }
                ).ToList();

                grid.AdicionaRows(listaDescontosRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os descontos do lançamento de NFS Manual.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDocumentoExterno()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Carga", "Carga", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Ocorrência", "Ocorrencia", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Fechamento", "FechamentoFrete", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Localidade", "LocalidadePrestacao", 13, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "ValorFrete", 13, Models.Grid.Align.right, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                    grid.AdicionarCabecalho("NFS-e", "NFSe", 8, Models.Grid.Align.right, true);
                else
                    grid.AdicionarCabecalho("NFSe", false);

                int codigoLancamentoNFSManual = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.NFS.LancamentoNFSManual repositorioLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = repositorioLancamentoNFSManual.BuscarPorCodigo(codigoLancamentoNFSManual);

                if (lancamentoNFSManual == null)
                    return new JsonpResult(false, true, "É necessário um lançamento de NFS-e Manual para realizar a consulta.");

                if (lancamentoNFSManual.Situacao != SituacaoLancamentoNFSManual.DadosNota)
                    return new JsonpResult(false, true, "A situação do lançamento de NFS-e Manual não permite que esta consulta seja realizada.");

                int numeroInicial = Request.GetIntParam("NumeroInicial");
                int numeroFinal = Request.GetIntParam("NumeroFinal");
                DateTime? dataInicio = Request.GetNullableDateTimeParam("DataInicial");
                DateTime? dataFim = Request.GetNullableDateTimeParam("DataFinal");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarDocumento);
                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                int totalRegistros = repositorioCargaDocumentoParaEmissaoNFSManual.ContarConsultaSelecaoNFSManualExterna(lancamentoNFSManual, numeroInicial, numeroFinal, dataInicio, dataFim);
                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> listaDocumentos = (totalRegistros > 0) ? repositorioCargaDocumentoParaEmissaoNFSManual.ConsultarSelecaoNFSManualExterna(lancamentoNFSManual, numeroInicial, numeroFinal, dataInicio, dataFim, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

                var listaDocumentosRetornar = (
                    from o in listaDocumentos
                    select new
                    {
                        Codigo = o.Codigo,
                        NFSe = o.DocumentosNFSe?.NFSe.Numero.ToString() ?? string.Empty,
                        DataEmissao = o.DataEmissao.ToString("dd/MM/yyyy"),
                        LocalidadePrestacao = o.LocalidadePrestacao.DescricaoCidadeEstado,
                        Carga = o.Carga.CodigoCargaEmbarcador,
                        Numero = o.Numero.ToString(),
                        Ocorrencia = o.CargaOcorrencia?.NumeroOcorrencia.ToString() ?? string.Empty,
                        FechamentoFrete = o.FechamentoFrete?.Descricao ?? string.Empty,
                        Destinatario = o.Destinatario.NomeCNPJ,
                        ValorFrete = o.ValorFrete.ToString("n2")
                    }
                ).ToList();

                grid.AdicionaRows(listaDocumentosRetornar);
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
        public async Task<IActionResult> PesquisaNFSGerados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.NFS.LancamentoNFSManual repNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = repNFSManual.BuscarPorCodigo(codigo);
                dynamic lista = new List<object>();
                int total = 0;

                if (nfsManual != null)
                {
                    if (nfsManual.CargasMultiCTe)
                        GridNFSeVinculadas(nfsManual, ref grid, out lista, out total, unitOfWork);
                    else
                        GridNFSeGerada(nfsManual, ref grid, out lista, out total, unitOfWork);
                }

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(total);

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
                // Respositorios
                Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao repLancamentoNFSAutorizacao = new Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);
                grid.AdicionarCabecalho("Justificativa", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Prioridade", "PrioridadeAprovacao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);

                // Ordenacao
                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                // Busca
                List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao> listaAutorizacao = repLancamentoNFSAutorizacao.ConsultarAutorizacoesPorNSF(codigo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repLancamentoNFSAutorizacao.ContarConsultaAutorizacoesPorNSF(codigo));

                var lista = (from obj in listaAutorizacao
                             select new
                             {
                                 obj.Codigo,
                                 PrioridadeAprovacao = obj.RegrasAutorizacaoNFSManual?.PrioridadeAprovacao ?? 0,
                                 Situacao = obj.DescricaoSituacao,
                                 Usuario = obj.Usuario?.Nome,
                                 Regra = obj.RegrasAutorizacaoNFSManual.Descricao,
                                 Data = obj.Data != null ? obj.Data.ToString() : string.Empty,
                                 Motivo = !string.IsNullOrWhiteSpace(obj.Motivo) ? obj.Motivo : string.Empty,
                                 Justificativa = obj.MotivoRejeicao?.Descricao ?? string.Empty,
                                 DT_RowColor = CorAprovacao(obj)
                             }).ToList();
                grid.AdicionaRows(lista);

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

        //public async Task<IActionResult> BuscarDocumentoPorNota()
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
        //    try
        //    {
        //        int.TryParse(Request.Params("NFSe"), out int NFSe);

        //        // Busca Dados
        //        int totalRegistros = 0;
        //        var listaTodosDocumentos = ExecutaPesquisaDocumento(ref totalRegistros, false, "", "", 0, 0, unitOfWork);

        //        // Seta valores na grid
        //        var listaCompativel = (from o in listaTodosDocumentos where o).to;

        //        return new JsonpResult(grid);
        //    }
        //    catch (Exception ex)
        //    {
        //        Servicos.Log.TratarErro(ex);
        //        return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //}

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.NFS.LancamentoNFSManual repNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = repNFSManual.BuscarPorCodigo(codigo);

                // Valida
                if (nfsManual == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!string.IsNullOrWhiteSpace(nfsManual.DadosNFS.AnexoNFS))
                {
                    string caminhoAnexo = Utilidades.IO.FileStorageService.Storage.Combine(CaminhoArquivo(unitOfWork), nfsManual.DadosNFS.AnexoNFS);
                    byte[] pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoAnexo);
                    return Arquivo(pdf, "application/pdf", System.IO.Path.GetFileName(caminhoAnexo));
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum arquivo anexado na NFS.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao Baixar da DANFSE.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadDANFSE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.NFS.LancamentoNFSManual repNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = repNFSManual.BuscarPorCodigo(codigo);

                // Valida
                if (nfsManual == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!string.IsNullOrWhiteSpace(nfsManual.DadosNFS.ImagemNFS))
                {
                    byte[] pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nfsManual.DadosNFS.ImagemNFS);
                    return Arquivo(pdf, "application/pdf", System.IO.Path.GetFileName(nfsManual.DadosNFS.ImagemNFS));
                }
                else
                {
                    return new JsonpResult(false, false, "Ainda não foi enviada a imagem da NFS gerada.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao Baixar da DANFSE.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.NFS.LancamentoNFSManual repNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = repNFSManual.BuscarPorCodigo(codigo);

                // Valida
                if (nfsManual == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!string.IsNullOrWhiteSpace(nfsManual.DadosNFS.XML))
                {
                    byte[] data = System.Text.Encoding.Default.GetBytes(nfsManual.DadosNFS.XML);
                    return Arquivo(data, "text/xml", string.Concat("NFSe_", nfsManual.DadosNFS.Numero, ".xml"));
                }
                else
                {
                    return new JsonpResult(false, false, "Ainda não foi enviado o xml da NFS gerada.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao Baixar o xml.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.NFS.LancamentoNFSManual repNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = repNFSManual.BuscarPorCodigo(codigo);

                // Valida
                if (nfsManual == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Exibe resumo quando esta nas seguites situacoes
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual[] exibirQuando = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual[]
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Todas,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.SemRegra,
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.DadosNota
                };
                bool exibeResumo = exibirQuando.Contains(nfsManual.Situacao);
                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoLayoutEDI.RetornoRPSNotaServico);
                decimal valorTotalFreteBruto = nfsManual.Documentos.Sum(obj => obj.ValorFrete);
                decimal valorTotalFrete = valorTotalFreteBruto - nfsManual.DadosNFS.ValorDescontos;
                Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documentoReferencia = nfsManual.Documentos.FirstOrDefault();

                // Formata retorno
                var retorno = new
                {
                    ContemEDI = layoutEDI != null,
                    nfsManual.Codigo,
                    nfsManual.Situacao,
                    nfsManual.SituacaoNoCancelamento,
                    nfsManual.CargasMultiCTe,
                    Filial = new { Codigo = nfsManual.Filial?.Codigo ?? 0, Descricao = nfsManual.Filial?.Descricao ?? string.Empty },
                    Filiais = new { Codigo = nfsManual.Filial?.Codigo ?? 0, Descricao = nfsManual.Filial?.Descricao ?? string.Empty },
                    TipoOperacao = new { Codigo = nfsManual.TipoOperacao?.Codigo ?? 0, Descricao = nfsManual.TipoOperacao?.Descricao ?? string.Empty },
                    Transportador = new { Codigo = nfsManual.Transportador?.Codigo ?? 0, Descricao = nfsManual.Transportador?.RazaoSocial ?? string.Empty },
                    Tomador = new { Codigo = nfsManual.Tomador?.Codigo ?? 0, Descricao = nfsManual.Tomador?.Nome ?? string.Empty },
                    FechamentoFrete = new { Codigo = nfsManual.FechamentoFrete?.Codigo ?? 0, Descricao = nfsManual.FechamentoFrete?.Descricao ?? "" },
                    CodigoServico = nfsManual.CodigoServico ?? "",
                    DadosNFS = nfsManual.DadosNFS == null ? null : new
                    {
                        ValorTotalFrete = valorTotalFrete.ToString("n2"),
                        ValorTotalFreteBruto = valorTotalFreteBruto.ToString("n2"),
                        Descontos = nfsManual.DadosNFS.ValorDescontos.ToString("n2"),
                        Numero = nfsManual.DadosNFS.Numero.ToString() ?? string.Empty,
                        CSTIBSCBS = nfsManual.DadosNFS.CSTIBSCBS,
                        ClassificacaoTributariaIBSCBS = nfsManual.DadosNFS.ClassificacaoTributariaIBSCBS,
                        BaseCalculoIBSCBS = nfsManual.DadosNFS.BaseCalculoIBSCBS,
                        AliquotaIBSEstadual = nfsManual.DadosNFS.AliquotaIBSEstadual,
                        AliquotaIBSMunicipal = nfsManual.DadosNFS.AliquotaIBSMunicipal,
                        ValorIBSEstadual = nfsManual.DadosNFS.ValorIBSEstadual,
                        ValorIBSMunicipal = nfsManual.DadosNFS.ValorIBSMunicipal,
                        nfsManual.DadosNFS.PercentualReducaoCBS,
                        nfsManual.DadosNFS.PercentualReducaoIBSEstadual,
                        nfsManual.DadosNFS.PercentualReducaoIBSMunicipal,
                        AliquotaCBS = nfsManual.DadosNFS.AliquotaCBS,
                        ValorCBS = nfsManual.DadosNFS.ValorCBS,
                        NBS = nfsManual.DadosNFS.NBS,
                        IndicadorOperacao = nfsManual.DadosNFS.IndicadorOperacao,
                        Serie = nfsManual.DadosNFS.Serie?.Numero.ToString() ?? string.Empty,
                        ValorPrestacaoServico = nfsManual.DadosNFS.ValorFrete.ToString("n2"),
                        IncluirValorBC = nfsManual.DadosNFS.IncluirISSBC,
                        ConsiderarLocalidadeCarga = nfsManual.DadosNFS.ConsiderarLocalidadeCarga,
                        DataEmissao = nfsManual.DadosNFS.DataEmissao.HasValue ? nfsManual.DadosNFS.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                        AliquotaISS = nfsManual.DadosNFS.AliquotaISS.ToString("n2"),
                        ValorISS = nfsManual.DadosNFS.ValorISS.ToString("n2"),
                        nfsManual.DadosNFS.TipoArredondamentoISS,
                        BaseCalculo = nfsManual.DadosNFS.ValorBaseCalculo.ToString("n2"),
                        PercentualRetencao = nfsManual.DadosNFS.PercentualRetencao.ToString("n2"),
                        ValorRetencao = nfsManual.DadosNFS.ValorRetido.ToString("n2"),
                        ValorPIS = nfsManual.DadosNFS.ValorPIS.ToString("n2"),
                        ValorCOFINS = nfsManual.DadosNFS.ValorCOFINS.ToString("n2"),
                        ValorPISIBSCBS = nfsManual.DadosNFS.ValorPISIBSCBS.ToString("n2"),
                        ValorCOFINSIBSCBS = nfsManual.DadosNFS.ValorCOFINSIBSCBS.ToString("n2"),
                        ValorIR = nfsManual.DadosNFS.ValorIR.ToString("n2"),
                        ValorCSLL = nfsManual.DadosNFS.ValorCSLL.ToString("n2"),
                        ValorReceber = nfsManual.DadosNFS.ValorReceber.ToString("n2"),
                        Observacao = nfsManual.DadosNFS.Observacoes,
                        NumeroRPS = nfsManual.DadosNFS.NumeroRPS.ToString() ?? string.Empty,
                        Moeda = nfsManual.DadosNFS.Moeda ?? MoedaCotacaoBancoCentral.Real,
                        ValorTotalMoeda = (nfsManual.DadosNFS.ValorTotalMoeda ?? 0m).ToString("n2"),
                        ModeloDocumentoFiscal = new { Codigo = nfsManual.DadosNFS.ModeloDocumentoFiscal?.Codigo ?? 0, Descricao = nfsManual.DadosNFS.ModeloDocumentoFiscal?.Descricao ?? string.Empty },
                        LocalidadePrestacao = new { Codigo = nfsManual.LocalidadePrestacao?.Codigo ?? 0, Descricao = nfsManual.LocalidadePrestacao?.DescricaoCidadeEstado ?? string.Empty },
                        Transportador = nfsManual.Transportador != null ? new { nfsManual.Transportador.Codigo, Descricao = nfsManual.Transportador.RazaoSocial } : new { Codigo = documentoReferencia?.Carga?.Empresa?.Codigo ?? 0, Descricao = documentoReferencia?.Carga?.Empresa?.RazaoSocial ?? "" },
                        Filial = nfsManual.Filial != null ? new { nfsManual.Filial.Codigo, nfsManual.Filial.Descricao } : new { Codigo = documentoReferencia?.Carga?.Filial?.Codigo ?? 0, Descricao = documentoReferencia?.Carga?.Filial?.Descricao ?? "" },
                        TipoOperacao = nfsManual.TipoOperacao != null ? new { nfsManual.TipoOperacao.Codigo, nfsManual.TipoOperacao.Descricao } : new { Codigo = documentoReferencia?.Carga?.TipoOperacao?.Codigo ?? 0, Descricao = documentoReferencia?.Carga?.TipoOperacao?.Descricao ?? "" },
                        Tomador = nfsManual.Tomador != null ? new { nfsManual.Tomador.Codigo, Descricao = nfsManual.Tomador.Nome } : new { Codigo = documentoReferencia?.Tomador?.Codigo ?? 0, Descricao = documentoReferencia?.Tomador?.Nome ?? "" },
                        ServicoNFSe = new { Codigo = nfsManual.DadosNFS.ServicoNFSe?.Codigo ?? 0, Descricao = nfsManual.DadosNFS.ServicoNFSe?.Descricao ?? string.Empty },
                    },
                    Resumo = exibeResumo ? null : ResumoAutorizacao(nfsManual, unitOfWork)
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

        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia
                Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao repLancamentoNFSAutorizacao = new Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao(unitOfWork);

                // Converte dados
                int codigoAutorizacao = int.Parse(Request.Params("Codigo"));

                // Busca a autorizacao
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao autorizacao = repLancamentoNFSAutorizacao.BuscarPorCodigo(codigoAutorizacao);

                var retorno = new
                {
                    autorizacao.Codigo,
                    Regra = autorizacao.RegrasAutorizacaoNFSManual.Descricao,
                    Situacao = autorizacao.DescricaoSituacao,
                    Usuario = autorizacao.Usuario.Nome,

                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Justificativa = autorizacao.MotivoRejeicao?.Descricao ?? string.Empty,
                    Motivo = !string.IsNullOrWhiteSpace(autorizacao.Motivo) ? autorizacao.Motivo : string.Empty,
                };

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

        public async Task<IActionResult> Emitir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();
                Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = repLancamentoNFSManual.BuscarPorCodigo(codigo);

                // Valida
                if (nfsManual == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
                if (nfsManual.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.DadosNota)
                    return new JsonpResult(false, true, "A situação do lançamento não permite essa operação.");

                // Valida entidade
                if (!ValidaEntidade(nfsManual.DadosNFS, out string erro))
                    return new JsonpResult(false, true, erro);

                if (string.IsNullOrEmpty(nfsManual.DadosNFS.ImagemNFS) && (!nfsManual.DadosNFS.Moeda.HasValue || nfsManual.DadosNFS.Moeda == MoedaCotacaoBancoCentral.Real) &&
                    (nfsManual.DadosNFS.ModeloDocumentoFiscal?.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.Outros && nfsManual.DadosNFS.ModeloDocumentoFiscal?.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe))
                    return new JsonpResult(false, true, "É obrigatório enviar DANFSE antes de finalizar o processo de envio.");

                if (string.IsNullOrEmpty(nfsManual.DadosNFS.ImagemNFS) && (!nfsManual.DadosNFS.Moeda.HasValue || nfsManual.DadosNFS.Moeda == MoedaCotacaoBancoCentral.Real) &&
                    ((nfsManual.DadosNFS.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros || nfsManual.DadosNFS.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe) && !configuracaoGeral.NaoBloquearEmissaoNFSeManualSemDANFSE))
                    return new JsonpResult(false, true, "É obrigatório enviar DANFSE antes de finalizar o processo de envio.");

                if (repLancamentoNFSManual.ExisteNFSHabilitadaComMesmoNumero(nfsManual.DadosNFS.Numero, nfsManual.DadosNFS.Serie.Numero, (nfsManual.DadosNFS.ModeloDocumentoFiscal?.Codigo ?? 0), nfsManual.Transportador.Codigo))
                    return new JsonpResult(false, true, "Existe uma NFS habilitada com esta mesma numeração, não sendo possível realizar a emissão da mesma.");

                // Busca as regras
                bool possuiRegras = true;
                if (ShouldGerarAprovacaoNFSManual(nfsManual, unitOfWork))
                {
                    possuiRegras = Servicos.Embarcador.NFSe.NFSManual.VerificarRegrasAutorizacaoNFS(nfsManual, TipoServicoMultisoftware, unitOfWork);
                    if (!possuiRegras)
                        nfsManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.SemRegra;
                    else
                    {
                        nfsManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.AgAprovacao;
                        Servicos.Embarcador.NFSe.NFSManual.VerificarSituacaoNFS(nfsManual, unitOfWork, TipoServicoMultisoftware, _conexao.StringConexao, this.Usuario);
                    }
                }
                else
                {
                    nfsManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.AgEmissao;
                }

                List<Dominio.Entidades.NFSe> nfsMultiCTe = (from o in nfsManual.Documentos where o.DocumentosNFSe != null select o.DocumentosNFSe.NFSe).Distinct().ToList();
                foreach (Dominio.Entidades.NFSe nota in nfsMultiCTe)
                {
                    nota.Status = Dominio.Enumeradores.StatusNFSe.AgAprovacaoNFSeManual;
                    repNFSe.Atualizar(nota);
                }

                // Integracao com SignalR
                svcNFSManual.InformarLancamentoNFSManualAtualizada(codigo);

                // Persiste dados
                repLancamentoNFSManual.Atualizar(nfsManual);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfsManual, null, "Solicitou Emissão da NFS-e.", unitOfWork);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    nfsManual.Codigo,
                    PossuiRegra = possuiRegras
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao emitir a nota.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);

                // Intancia Serviços
                Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = repLancamentoNFSManual.BuscarPorCodigo(codigo);

                // Valida
                if (nfsManual == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
                if (nfsManual.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.SemRegra)
                    return new JsonpResult(false, true, "A situação do lançamento não permite essa operação.");

                // Busca as regras
                bool possuiRegra = Servicos.Embarcador.NFSe.NFSManual.VerificarRegrasAutorizacaoNFS(nfsManual, TipoServicoMultisoftware, unitOfWork);
                if (possuiRegra)
                {
                    nfsManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.AgAprovacao;
                    Servicos.Embarcador.NFSe.NFSManual.VerificarSituacaoNFS(nfsManual, unitOfWork, TipoServicoMultisoftware, unitOfWork.StringConexao, Usuario);
                }

                // Integracao com SignalR
                svcNFSManual.InformarLancamentoNFSManualAtualizada(nfsManual.Codigo);

                // Persiste dados
                repLancamentoNFSManual.Atualizar(nfsManual);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    nfsManual.Codigo,
                    PossuiRegra = possuiRegra,
                    Situacao = nfsManual.Situacao,
                    Resumo = !possuiRegra ? null : ResumoAutorizacao(nfsManual, unitOfWork)
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarNFS()
        {
            //LancamentoNFSManual
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);

                int codigoLancamentoNFSManual = int.Parse(Request.Params("LancamentoNFSManual"));

                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = repLancamentoNFSManual.BuscarPorCodigo(codigoLancamentoNFSManual);


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("Desabilitado", false);
                grid.AdicionarCabecalho("NumeroModeloDocumentoFiscal", false);
                grid.AdicionarCabecalho("TipoDocumentoEmissao", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Serie", "Serie", 5, Models.Grid.Align.center, true);

                grid.AdicionarCabecalho("Doc.", "AbreviacaoModeloDocumentoFiscal", 10, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    grid.AdicionarCabecalho("T. Pagamento", "DescricaoTipoPagamento", 10, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, true);
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                {
                    grid.AdicionarCabecalho("Emissão", "DataEmissao", 10, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, true);
                }
                grid.AdicionarCabecalho("T. Serviço", "DescricaoTipoServico", 10, Models.Grid.Align.center, true);

                grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true, false, false, false, true);

                grid.AdicionarCabecalho("ISS", "AliquotaISS", 8, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("IBS Município", "IBSMunicipio", 8, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("IBS UF", "IBSUF", 8, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("CBS", "CBS", 8, Models.Grid.Align.right, true, false, false, false, true);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                {
                    grid.AdicionarCabecalho("Status", "Status", 8, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho("Retorno Sefaz", "RetornoSefaz", 15, Models.Grid.Align.left, false);
                }

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                    propOrdenacao += ".Nome";
                if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                if (propOrdenacao == "DescricaoTipoPagamento")
                    propOrdenacao = "TipoPagamento";

                if (propOrdenacao == "DescricaoTipoServico")
                    propOrdenacao = "TipoServico";

                if (propOrdenacao == "AbreviacaoModeloDocumentoFiscal")
                    propOrdenacao = "ModeloDocumentoFiscal.Abreviacao";

                propOrdenacao = "CTe." + propOrdenacao;

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                int quantidade = 0;

                if (lancamentoNFSManual.CTe != null)
                {
                    ctes.Add(lancamentoNFSManual.CTe);
                    quantidade = 1;
                }

                grid.setarQuantidadeTotal(quantidade);
                var lista = (from obj in ctes
                             select new
                             {
                                 lancamentoNFSManual.Codigo,
                                 CodigoCTE = obj.Codigo,
                                 obj.DescricaoTipoServico,
                                 NumeroModeloDocumentoFiscal = obj.ModeloDocumentoFiscal.Numero,
                                 TipoDocumentoEmissao = obj.ModeloDocumentoFiscal.TipoDocumentoEmissao,
                                 AbreviacaoModeloDocumentoFiscal = obj.ModeloDocumentoFiscal.Abreviacao,
                                 CodigoEmpresa = obj.Empresa.Codigo,
                                 obj.Numero,
                                 obj.DataEmissao,
                                 SituacaoCTe = obj.Status,
                                 Serie = obj.Serie.Numero,
                                 obj.DescricaoTipoPagamento,
                                 Remetente = obj.Remetente != null ? obj.Remetente.Nome + "(" + obj.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty,
                                 Destinatario = obj.Destinatario != null ? obj.Destinatario.Nome + "(" + obj.Destinatario.CPF_CNPJ_Formatado + ")" : string.Empty,
                                 Destino = obj.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                 ValorFrete = obj.ValorAReceber.ToString("n2"),
                                 AliquotaISS = obj.AliquotaISS.ToString("n2"),
                                 IBSMunicipio = obj.AliquotaIBSMunicipal,
                                 IBSUF = obj.AliquotaIBSEstadual,
                                 CBS = obj.AliquotaCBS,
                                 Status = obj.DescricaoStatus,
                                 NumeroNotas = obj.NumeroNotas,
                                 RetornoSefaz = !string.IsNullOrWhiteSpace(obj.MensagemRetornoSefaz) ? obj.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.MensagemRetornoSefaz) : "" : "",
                                 DT_RowColor = (obj.Status == "A" ? "#dff0d8" : obj.Status == "R" ? "rgba(193, 101, 101, 1)" : (obj.Status == "C" || obj.Status == "I" || obj.Status == "D") ? "#777" : ""),
                                 DT_FontColor = ((obj.Status == "R" || obj.Status == "C" || obj.Status == "I" || obj.Status == "D") ? "#FFFFFF" : ""),
                                 Desabilitado = obj.Desabilitado ?? false
                             }).ToList();
                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string codigoServico = Request.GetStringParam("CodigoServico");

                // Instancia repositorios
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Repositorio.Embarcador.NFS.DadosNFSManual repDadosNFSManual = new Repositorio.Embarcador.NFS.DadosNFSManual(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);

                Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();

                Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual filtrosPesquisa = ObterFiltroPesquisaCargaDocumentoParaEmissaoNFSManual(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentos = BuscarDocumentosSelecionados(filtrosPesquisa, unitOfWork);

                if (documentos.Count == 0)
                    throw new ControllerException("Nenhum documento selecionado.");

                if (documentos.Any(o => o.Carga.SituacaoCarga == SituacaoCarga.Cancelada || o.Carga.SituacaoCarga == SituacaoCarga.Anulada))
                    throw new ControllerException("Enquanto as notas estavam sendo selecionadas uma carga das notas selecionadas foi cancelada. Por favor, refaça o processo.");

                if (documentos.Where(o => o.CargaOcorrencia != null).Any(o => o.CargaOcorrencia.SituacaoOcorrencia == SituacaoOcorrencia.Cancelada || o.CargaOcorrencia.SituacaoOcorrencia == SituacaoOcorrencia.Anulada))
                    throw new ControllerException("Enquanto as notas estavam sendo selecionadas uma ocorrência das notas selecionadas foi cancelada. Por favor, refaça o processo.");

                if (documentos.Any(o => o.FechamentoFrete == null) && documentos.Any(o => o.FechamentoFrete != null))
                    throw new ControllerException("Não é possivel gerar NFS manual com documentos sem fechamento quando existem documentos com fechamento.");

                List<MoedaCotacaoBancoCentral> moedas = documentos.Select(o => o.Moeda ?? MoedaCotacaoBancoCentral.Real).Distinct().ToList();

                if (moedas.Count > 1)
                    throw new ControllerException($"Não é possível gerar uma NFS manual com mais de um tipo de moeda ({string.Join(", ", moedas.Select(o => o.ObterDescricao()))}).");

                // Valida se não tá misturando documentos que são gerados a partir de uma ocorrência (são complementos) com que não são
                bool primeiroDocumentoEhComplemento = documentos[0].CargaOcorrencia != null;
                bool temComplementosMisturadosComNormais = documentos.Any(o => (o.CargaOcorrencia != null) != primeiroDocumentoEhComplemento);

                if (temComplementosMisturadosComNormais)
                    throw new ControllerException($"Não é possível gerar uma NFS manual misturando documentos que são complementos de ocorrência com que não são.");

                unitOfWork.Start();

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento repConfiguracaoCargaEmissaoDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCargaEmissaoDocumento configuracaoCargaEmissaoDocumento = repConfiguracaoCargaEmissaoDocumento.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                if (configuracaoCargaEmissaoDocumento?.NaoPermitirNFSComMultiplosCentrosResultado ?? false)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = documentos.Where(o => o.PedidoXMLNotaFiscal != null && o.PedidoCTeParaSubContratacao == null).Select(o => o.PedidoXMLNotaFiscal.CargaPedido).Distinct().ToList();
                    cargaPedidos.AddRange(documentos.Where(o => o.PedidoCTeParaSubContratacao != null && o.PedidoXMLNotaFiscal == null && o.CargaCTe == null).Select(o => o.PedidoCTeParaSubContratacao.CargaPedido).Distinct().ToList());
                    cargaPedidos.AddRange(documentos.Where(o => o.CargaCTe != null && o.PedidoXMLNotaFiscal == null && o.PedidoCTeParaSubContratacao == null).Select(o => o.CargaCTe.NotasFiscais.Select(p => p.PedidoXMLNotaFiscal.CargaPedido)).SelectMany(o => o).Distinct().ToList());

                    if (cargaPedidos.Select(o => o.Pedido.CentroResultado).Distinct().Count() > 1)
                        throw new ControllerException("Existe mais de um centro de resultado nos documentos selecionados, não sendo possível gerar a NFS.");
                }

                Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual referencia = documentos[0];
                Dominio.Entidades.Embarcador.NFS.DadosNFSManual dadosNFS = new Dominio.Entidades.Embarcador.NFS.DadosNFSManual()
                {
                    ValorFrete = documentos.Sum(obj => obj.ValorFrete),
                    NumeroRPS = repDadosNFSManual.BuscarProximoNumeroRPS(),
                    ValorTotalMoeda = documentos.Sum(obj => obj.ValorTotalMoeda ?? 0m),
                    Moeda = moedas.FirstOrDefault(),
                    IncluirISSBC = !ConfiguracaoEmbarcador.PadraoInclusaiISSDesmarcado
                };


                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && documentos != null && documentos.Count > 0)
                {
                    if (ConfiguracaoEmbarcador.PossuiWMS)
                        dadosNFS.Observacoes = "Minutas " + string.Join(" ", (from o in documentos select o.Numero.ToString("D")).ToList());
                    else
                        dadosNFS.Observacoes = "DESCARGA DE MERCADORIA MIN.: " + string.Join(", ", (from o in documentos select o.Numero.ToString("D")).ToList());
                }

                Dominio.Entidades.Empresa transportador = filtrosPesquisa.CodigoTransportador > 0 ? referencia.Carga.Empresa : null;
                Dominio.Entidades.Embarcador.Filiais.Filial filial = filtrosPesquisa.CodigoFilial > 0 ? referencia.Carga.Filial : null;
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? referencia.Carga.TipoOperacao : null;
                Dominio.Entidades.Cliente tomador = filtrosPesquisa.CpfCnpjTomador > 0d ? referencia.Tomador : null;

                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = new Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual()
                {
                    Transportador = transportador,
                    Situacao = SituacaoLancamentoNFSManual.DadosNota,
                    Tomador = tomador,
                    Filial = filial,
                    TipoOperacao = tipoOperacao,
                    FechamentoFrete = referencia.FechamentoFrete,
                    DadosNFS = dadosNFS,
                    LocalidadePrestacao = referencia.LocalidadePrestacao,
                    NFSResidual = referencia.DocResidual,
                    Usuario = this.Usuario,
                    DataCriacao = DateTime.Now,
                    CargasMultiCTe = (from o in documentos where o.DocumentosNFSe != null select o).Count() > 0,
                    CodigoServico = codigoServico
                };

                // Persiste dados
                repDadosNFSManual.Inserir(dadosNFS);
                repLancamentoNFSManual.Inserir(nfsManual, Auditado);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual documento in documentos)
                {
                    documento.LancamentoNFSManual = nfsManual;
                    repCargaDocumentoParaEmissaoNFSManual.Atualizar(documento);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Criou uma NFS-e manual com o documento.", unitOfWork);
                }

                AdicionarDescontos(nfsManual, configuracaoEmbarcador, unitOfWork);
                Servicos.Embarcador.NFSe.NFSManual.CalcularValores(dadosNFS, unitOfWork);
                Servicos.Embarcador.NFSe.NFSManual.CalcularISS(dadosNFS);

                repDadosNFSManual.Atualizar(dadosNFS);

                // Integracao com SignalR
                svcNFSManual.InformarLancamentoNFSManualAtualizada(nfsManual.Codigo);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    Codigo = nfsManual.Codigo
                });
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Repositorio.Embarcador.NFS.DadosNFSManual repDadosNFSManual = new Repositorio.Embarcador.NFS.DadosNFSManual(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = repLancamentoNFSManual.BuscarPorCodigo(codigo, true);

                // Valida
                if (nfsManual == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Instancia dados da nfs
                Dominio.Entidades.Embarcador.NFS.DadosNFSManual dados = nfsManual.DadosNFS;

                // Preenche os dados
                dados.Initialize();
                string retorno = PreencherEntidade(ref dados, nfsManual, unitOfWork);

                if (!string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(false, true, retorno);

                // Valida entidade
                if (!ValidaEntidade(dados, out string erro))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && dados.Numero <= 0)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, "Informe o número maior que 0.");
                }

                repDadosNFSManual.Atualizar(dados, Auditado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfsManual, dados.GetChanges(), "Atualizou.", unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Anexar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Repositorio.Embarcador.NFS.DadosNFSManual repDadosNFSManual = new Repositorio.Embarcador.NFS.DadosNFSManual(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoSGT = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = repLancamentoNFSManual.BuscarPorCodigo(codigo);

                // Valida
                if (nfsManual == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                arquivos = arquivos.GroupBy(file => file.FileName).Select(group => group.First()).ToList();
                string tipoParametro = Request.GetStringParam("Tipo");
                string[] tipos = tipoParametro.Split(',');
                if (arquivos.Count <= 0 || arquivos.Count > 3)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                if (nfsManual.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.DadosNota)
                    return new JsonpResult(false, true, "Situação da nota não permite anexar arquivos.");

                for (var i = 0; i < arquivos.Count(); i++)
                {
                    // Extrai dados
                    Servicos.DTO.CustomFile file = arquivos[i];
                    string tipo = i < tipos.Length ? tipos[i] : string.Empty;

                    if (tipo.Equals("XML"))
                    {
                        // Valida arquivo
                        if (Path.GetExtension(file.FileName).ToLower() != ".xml")
                            return new JsonpResult(false, true, "Arquivo de XML inválido.");

                        // Converte o xml em string
                        List<string> xmlData = new List<string>();
                        StreamReader reader = new StreamReader(file.InputStream);
                        while (!reader.EndOfStream)
                            xmlData.Add(reader.ReadLine());
                        string xml = String.Join("", xmlData);
                        nfsManual.DadosNFS.XML = xml;
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, nfsManual, null, "Anexou o XML.", unitOfWork);
                    }
                    else if (tipo.Equals("DANFSE"))
                    {
                        // Valida arquivo
                        if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
                            return new JsonpResult(false, true, "Arquivo de PDF inválido.");

                        // Converte arquivo em bytes
                        var reader = new BinaryReader(file.InputStream);
                        byte[] pdfData = reader.ReadBytes((int)file.Length);

                        // Salva DANFSE
                        nfsManual.DadosNFS.ImagemNFS = this.SalvarDANFSE(nfsManual.DadosNFS.Numero, nfsManual.DadosNFS.Serie.Numero, nfsManual.Transportador, pdfData, unitOfWork);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, nfsManual, null, "Anexou a DANFSE.", unitOfWork);
                    }
                    else if (tipo.Equals("Anexo"))
                    {
                        // Converte arquivo em bytes
                        var reader = new BinaryReader(file.InputStream);
                        byte[] pdfData = reader.ReadBytes((int)file.Length);

                        string extensao = Path.GetExtension(file.FileName).ToLower();

                        // Salva DANFSE
                        nfsManual.DadosNFS.AnexoNFS = this.SalvarAnexo(nfsManual.DadosNFS.Numero, nfsManual.DadosNFS.Serie.Numero, nfsManual.Transportador, extensao, pdfData, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, nfsManual, null, "Anexou um arquivo.", unitOfWork);
                    }

                }
                repDadosNFSManual.Atualizar(nfsManual.DadosNFS);

                // Persiste dados
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Repositorio.Embarcador.NFS.DadosNFSManual repDadosNFSManual = new Repositorio.Embarcador.NFS.DadosNFSManual(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual = repCargaDocumentoParaEmissaoNFSManual.BuscarPorCodigo(codigo);

                if (cargaDocumentoParaEmissaoNFSManual == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o documento.");

                if (cargaDocumentoParaEmissaoNFSManual.LancamentoNFSManual?.DadosNFS == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o lançamento da NFS-e Manual.");

                if (cargaDocumentoParaEmissaoNFSManual.LancamentoNFSManual.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.DadosNota)
                    return new JsonpResult(false, true, "Não é possível remover um documento na situação atual do lançamento (" + cargaDocumentoParaEmissaoNFSManual.LancamentoNFSManual.DescricaoSituacao + ").");

                Dominio.Entidades.Embarcador.NFS.DadosNFSManual dadosNFS = repDadosNFSManual.BuscarPorCodigo(cargaDocumentoParaEmissaoNFSManual.LancamentoNFSManual.DadosNFS.Codigo, true);

                if (repCargaDocumentoParaEmissaoNFSManual.ContarConsultaPorLancamento(cargaDocumentoParaEmissaoNFSManual.LancamentoNFSManual.Codigo) <= 1)
                    return new JsonpResult(false, true, "Não é permitido remover todos os documentos do lançamento, neste caso cancele o lançamento e faça um novo.");

                unitOfWork.Start();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaDocumentoParaEmissaoNFSManual.LancamentoNFSManual, null, "Removeu o documento " + cargaDocumentoParaEmissaoNFSManual.Numero + " do lançamento.", unitOfWork);

                cargaDocumentoParaEmissaoNFSManual.LancamentoNFSManual = null;
                repCargaDocumentoParaEmissaoNFSManual.Atualizar(cargaDocumentoParaEmissaoNFSManual);

                Servicos.Embarcador.NFSe.NFSManual.CalcularValores(dadosNFS, unitOfWork);
                Servicos.Embarcador.NFSe.NFSManual.CalcularISS(dadosNFS);



                if (dadosNFS.ValorReceber < 0m)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Não é possível remover o documento pois o valor a receber ficará negativo. Verifique os valores já configurados nos dados NFS.");
                }

                repDadosNFSManual.Atualizar(dadosNFS, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover o documento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoNFSManual = Request.GetIntParam("NFSManual");
                Repositorio.Embarcador.NFS.LancamentoNFSManual repositorioLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = repositorioLancamentoNFSManual.BuscarPorCodigo(codigoNFSManual);

                if (lancamentoNFSManual?.DadosNFS == null)
                    throw new ControllerException("Não foi possível encontrar o lançamento da NFS-e Manual.");

                if (lancamentoNFSManual.Situacao != SituacaoLancamentoNFSManual.DadosNota)
                    throw new ControllerException($"Não é possível adicionar um documento na situação atual do lançamento ({lancamentoNFSManual.DescricaoSituacao}).");

                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                Repositorio.Embarcador.NFS.DadosNFSManual repositorioDadosNFSManual = new Repositorio.Embarcador.NFS.DadosNFSManual(unitOfWork);
                Dominio.Entidades.Embarcador.NFS.DadosNFSManual dadosNFS = repositorioDadosNFSManual.BuscarPorCodigo(lancamentoNFSManual.DadosNFS.Codigo, true);
                List<int> documentos = Request.GetListParam<int>("Documentos");

                foreach (int codigoDocumento in documentos)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual cargaDocumentoParaEmissaoNFSManual = repositorioCargaDocumentoParaEmissaoNFSManual.BuscarPorCodigo(codigoDocumento);

                    if (cargaDocumentoParaEmissaoNFSManual.LancamentoNFSManual != null)
                        throw new ControllerException($"Não é possível adicionar o documento {cargaDocumentoParaEmissaoNFSManual.Numero} pois ele está em outra NFS-e Manual.");

                    if ((lancamentoNFSManual.FechamentoFrete != null) && (cargaDocumentoParaEmissaoNFSManual.FechamentoFrete == null))
                        throw new ControllerException($"Não é possível adicionar o documento {cargaDocumentoParaEmissaoNFSManual.Numero} pois ele não possui um fechamento.");

                    if ((lancamentoNFSManual.FechamentoFrete == null) && (cargaDocumentoParaEmissaoNFSManual.FechamentoFrete != null))
                        throw new ControllerException($"Não é possível adicionar o documento {cargaDocumentoParaEmissaoNFSManual.Numero} pois ele possui um fechamento.");

                    cargaDocumentoParaEmissaoNFSManual.LancamentoNFSManual = lancamentoNFSManual;

                    repositorioCargaDocumentoParaEmissaoNFSManual.Atualizar(cargaDocumentoParaEmissaoNFSManual);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaDocumentoParaEmissaoNFSManual.LancamentoNFSManual, null, $"Adicionou o documento {cargaDocumentoParaEmissaoNFSManual.Numero} ao lançamento.", unitOfWork);
                }

                Servicos.Embarcador.NFSe.NFSManual.CalcularValores(dadosNFS, unitOfWork);
                Servicos.Embarcador.NFSe.NFSManual.CalcularISS(dadosNFS);
                repositorioDadosNFSManual.Atualizar(dadosNFS, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o documento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ImprimirRelacaoDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);

                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = repLancamentoNFSManual.BuscarPorCodigo(codigo);

                return Arquivo(Servicos.Embarcador.NFSe.NFSManual.GerarImpressaoRelacaoDocumentos(lancamentoNFSManual, unitOfWork), "application/pdf", "Documentos da NFS Manual " + lancamentoNFSManual.DadosNFS.Numero.ToString() + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar a impressão da relação de documentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ContemEDI()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoLayoutEDI.RetornoRPSNotaServico);

                // Formata retorno
                var retorno = new
                {
                    ContemEDI = layoutEDI != null
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

        [AllowAuthenticate]
        public async Task<IActionResult> GerarMultiploRPS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

                int totalRegistros = 0;
                List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> lista = ExecutaPesquisa(ref totalRegistros, false, "", "", 0, 0, unitOfWork, true);

                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoLayoutEDI.RPSNotaServico);
                if (layoutEDI == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro do layout do EDI para RPS.");
                if (lista == null || lista.Count == 0)
                    return new JsonpResult(false, true, "Nenhuma nota localizada.");

                Servicos.Embarcador.Integracao.EDI.RPSNotaServico svcRPSNotaServico = new Servicos.Embarcador.Integracao.EDI.RPSNotaServico();

                Dominio.ObjetosDeValor.EDI.RPS.NotaServico notaServico = svcRPSNotaServico.ConverterRPSNotaServico(lista, unitOfWork);

                Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(unitOfWork, layoutEDI, null);
                System.IO.MemoryStream edi = serGeracaoEDI.GerarArquivoRecursivo(notaServico);

                return Arquivo(edi, "text/txt", string.Concat("Arquivo descarga Original ", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o arquivo E-contab.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarArquivoRPS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual = repLancamentoNFSManual.BuscarPorCodigo(codigo);

                // Valida
                if (nfsManual == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
                if (nfsManual.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.DadosNota)
                    return new JsonpResult(false, true, "A situação do lançamento não permite essa operação.");

                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoLayoutEDI.RPSNotaServico);
                if (layoutEDI == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro do layout do EDI para RPS.");

                Servicos.Embarcador.Integracao.EDI.RPSNotaServico svcRPSNotaServico = new Servicos.Embarcador.Integracao.EDI.RPSNotaServico();
                Dominio.ObjetosDeValor.EDI.RPS.NotaServico notaServico = svcRPSNotaServico.ConverterRPSNotaServico(nfsManual, nfsManual.Transportador, unitOfWork);

                Servicos.GeracaoEDI serGeracaoEDI = new Servicos.GeracaoEDI(unitOfWork, layoutEDI, null);
                System.IO.MemoryStream edi = serGeracaoEDI.GerarArquivoRecursivo(notaServico);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfsManual, null, "Processado arquivo de RPS", unitOfWork);
                return Arquivo(edi, "text/txt", string.Concat("Arquivo descarga Original ", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o arquivo E-contab.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ProcessarRetornoNotaServico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoLayoutEDI.RetornoRPSNotaServico);
                if (layoutEDI == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro do layout do EDI para o Retorno do RPS.");

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        Servicos.DTO.CustomFile file = files[i];
                        string extensao = System.IO.Path.GetExtension(file.FileName).ToLower();
                        string nomeArquivo = System.IO.Path.GetFileName(file.FileName);

                        if (extensao.Equals(".txt"))
                        {
                            Servicos.Embarcador.Integracao.EDI.RPSNotaServico svcRPSNotaServico = new Servicos.Embarcador.Integracao.EDI.RPSNotaServico();
                            string retorno = svcRPSNotaServico.ProcessarRetornoRPSNotaServico(unitOfWork, file.InputStream, layoutEDI, Auditado, TipoServicoMultisoftware, this.Usuario, _conexao.StringConexao);
                            if (!string.IsNullOrWhiteSpace(retorno))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, retorno);
                            }
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "O arquivo não está na extensão correta.");
                        }
                    }

                    unitOfWork.CommitChanges();

                    return new JsonpResult(true, true, "Sucesso");
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Não foi enviado o arquivo.");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar a retorno do RPS, verifique se o arquivo selecionado é realmente o retorno do RPS.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string chave = Utilidades.String.OnlyNumbers(Request.Params("ChaveDocumento"));

                if (!Utilidades.Validate.ValidarChaveNFe(chave))
                    return new JsonpResult(false, true, "A chave informada é inválida.");

                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repositorioXMLNotaFiscal.BuscarPorChave(chave);

                if (xmlNotaFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar uma nota para a chave informada.");

                var retorno = new Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaNFeSelecaoDocumentosNFS()
                {
                    Codigo = xmlNotaFiscal.Codigo,
                    Numero = xmlNotaFiscal.Numero,
                    Chave = xmlNotaFiscal.Chave
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar a chave.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarFiltroDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();

                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoFiltros();

                string erro = string.Empty;
                int contador = 0;
                string dados = Request.Params("Dados");

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                int totalLinhas = linhas.Count;
                List<Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaNFeSelecaoDocumentosNFS> retorno = new List<Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaNFeSelecaoDocumentosNFS>();

                for (int i = 0; i < totalLinhas; i++)
                {
                    try
                    {
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colChaveDocumento = linha.Colunas?.Where(o => o.NomeCampo == "ChaveDocumento").FirstOrDefault();

                        string chaveDocumento = Utilidades.String.OnlyNumbers(((string)colChaveDocumento?.Valor ?? string.Empty));

                        if (contador >= 500)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("O limite de chaves importadas foi atingido.", i));
                            continue;
                        }

                        if (!Utilidades.Validate.ValidarChaveNFe(chaveDocumento))
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("A chave informada é inválida.", i));
                            continue;
                        }

                        Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = repositorioXMLNotaFiscal.BuscarPorChave(chaveDocumento);

                        if (xmlNotaFiscal == null)
                        {
                            retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Não foi possível encontrar uma nota para a chave informada.", i));
                            continue;
                        }

                        if (!retorno.Any(o => o.Codigo == xmlNotaFiscal.Codigo))
                            retorno.Add(new Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaNFeSelecaoDocumentosNFS() { Codigo = xmlNotaFiscal.Codigo, Numero = xmlNotaFiscal.Numero, Chave = chaveDocumento });

                        contador++;

                        retornoImportacao.Retornolinhas.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = i, processou = true, mensagemFalha = "" });

                        if ((i % 10) == 0)
                        {
                            unitOfWork.FlushAndClear();
                        }
                    }
                    catch (Exception ex2)
                    {
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(RetornarFalhaLinha("Ocorre uma falha ao processar a linha.", i));
                        continue;
                    }
                }

                retornoImportacao.Retorno = retorno;
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterConfiguracaoImportacaoFiltros()
        {
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoFiltros();

                return new JsonpResult(configuracoes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoObterAsConfiguracoesParaImportacao);
            }
        }

        public async Task<IActionResult> ObterAliquotaRetancaoISS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int.TryParse(Request.Params("Localidade"), out int localidade);
                int.TryParse(Request.Params("ServicoNFSe"), out int codigoServico);
                int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);

                Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe configuracoesTransportadorConfiguracaoNFSe = new Servicos.Embarcador.NFSe.NFSe(unitOfWork).BuscarPorLocalidadeEmpresaServico(localidade, codigoEmpresa, codigoServico, unitOfWork); ;

                var retorno = new
                {
                    CodigoServico = configuracoesTransportadorConfiguracaoNFSe?.ServicoNFSe?.Codigo ?? 0,
                    DescricaoServico = configuracoesTransportadorConfiguracaoNFSe?.ServicoNFSe?.Descricao ?? "",
                    AliquotaISS = configuracoesTransportadorConfiguracaoNFSe?.AliquotaISS.ToString().Replace('.', ',') ?? "0,00",
                    RetencaoISS = configuracoesTransportadorConfiguracaoNFSe?.RetencaoISS.ToString().Replace('.', ',') ?? "0,00"
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, Localization.Resources.Canhotos.Canhoto.OcorreuUmaFalhaAoObterAsConfiguracoesParaImportacao);
            }
        }
        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDescontoParaEmissaoNFSManual ObterFiltroPesquisaCargaDescontoParaEmissaoNFSManual(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDescontoParaEmissaoNFSManual filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDescontoParaEmissaoNFSManual()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("Carga"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                Codigosfilial = Request.GetListParam<int>("Filiais"),
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                DataFinal = Request.GetNullableDateTimeParam("DataFim"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio")
            };

            if (configuracaoEmbarcador.UtilizaMoedaEstrangeira)
                filtrosPesquisa.Moeda = Request.GetEnumParam<MoedaCotacaoBancoCentral>("Moeda");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.CodigoTransportador = this.Empresa.Codigo;
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCNPJ(this.Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);

                filtrosPesquisa.CodigoTransportador = empresa.Codigo;
                filtrosPesquisa.CodigoFilial = 0;
            }

            return filtrosPesquisa;
        }

        private Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual ObterFiltroPesquisaCargaDocumentoParaEmissaoNFSManual(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual()
            {
                CodigoCargaEmbarcador = ((ConfiguracaoEmbarcador.PermitirGerarNotaMesmoPedidoCarga || ConfiguracaoEmbarcador.PermitirGerarNotaMesmaCarga) && !Request.GetBoolParam("Residuais")) ? Request.GetStringParam("NumeroCarga") : Request.GetStringParam("Carga"),
                CodigoDestinatario = Request.GetDoubleParam("Destinatario"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoLancamentoNFSManual = Request.GetIntParam("Codigo"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                Codigosfilial = Request.GetListParam<int>("Filiais"),
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                DataFinal = Request.GetNullableDateTimeParam("DataFim"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                Residuais = Request.GetBoolParam("Residuais"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                ComplementoOcorrencia = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.NFSManualTipoComplemento>("ComplementoOcorrencia"),
                RetornarFiliaisTransportador = ConfiguracaoEmbarcador.EmitirNFSManualParaTransportadorEFiliais
            };

            if (ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
                filtrosPesquisa.Moeda = Request.GetEnumParam<MoedaCotacaoBancoCentral>("Moeda");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.CodigoTransportador = this.Empresa.Codigo;
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCNPJ(this.Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);

                filtrosPesquisa.CodigoTransportador = empresa.Codigo;
                filtrosPesquisa.CodigoFilial = 0;
            }

            if (filtrosPesquisa.CodigoLancamentoNFSManual > 0)
            {
                Repositorio.Embarcador.NFS.LancamentoNFSManual repositorioLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = repositorioLancamentoNFSManual.BuscarPorCodigo(filtrosPesquisa.CodigoLancamentoNFSManual);

                filtrosPesquisa.LancamentoNFSManualCancelado = (
                    lancamentoNFSManual.Situacao == SituacaoLancamentoNFSManual.Cancelada ||
                    lancamentoNFSManual.Situacao == SituacaoLancamentoNFSManual.Anulada ||
                    lancamentoNFSManual.Situacao == SituacaoLancamentoNFSManual.Reprovada
                );
            }

            List<Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaNFeSelecaoDocumentosNFS> listaDocumentos = Request.GetListParam<Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaNFeSelecaoDocumentosNFS>("ListaChaves");
            filtrosPesquisa.CodigosDocumentos = listaDocumentos?.Count > 0 ? (from obj in listaDocumentos select obj.Codigo).ToList() : new List<int>();

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenarDocumento(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Carga")
                return "Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "Ocorrencia")
                return "CargaOcorrencia.NumeroOcorrencia";

            if (propriedadeOrdenar == "FechamentoFrete")
                return "FechamentoFrete.Numero";

            if (propriedadeOrdenar == "Destinatario")
                return "Destinatario.Nome";

            if (propriedadeOrdenar == "Tomador")
                return "Tomador.Nome";

            return propriedadeOrdenar;
        }

        private void AdicionarDescontos(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            //descontos residuais só podem ser aplicados em notas com valores residuais positivos.
            if (!lancamentoNFSManual.NFSResidual)
                return;

            Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDescontoParaEmissaoNFSManual filtrosPesquisa = ObterFiltroPesquisaCargaDescontoParaEmissaoNFSManual(configuracaoEmbarcador, unitOfWork);
            Repositorio.Embarcador.NFS.LancamentoNFSManualDesconto repositorioLancamentoNFSManualDesconto = new Repositorio.Embarcador.NFS.LancamentoNFSManualDesconto(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repositorioLancamentoNFSManualDesconto.BuscarCargasParaLancamentoNFSManualDesconto(filtrosPesquisa);
            decimal ValorFreteBruto = lancamentoNFSManual.DadosNFS.ValorFrete;
            decimal valorDescontos = 0m;

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
            {
                decimal desconto = -(valorDescontos + carga.ValorFreteResidual);
                if (desconto <= lancamentoNFSManual.DadosNFS.ValorFrete)//se o valor do desconto for superior ao valor da nota não adiciona na nota, será utilizado em outra nota.
                {
                    Dominio.Entidades.Embarcador.NFS.LancamentoNFSManualDesconto lancamentoNFSManualDesconto = new Dominio.Entidades.Embarcador.NFS.LancamentoNFSManualDesconto()
                    {
                        Carga = carga,
                        LancamentoNFSManual = lancamentoNFSManual
                    };

                    repositorioLancamentoNFSManualDesconto.Inserir(lancamentoNFSManualDesconto);
                    valorDescontos += carga.ValorFreteResidual;
                }
            }

            valorDescontos = -valorDescontos;

            lancamentoNFSManual.DadosNFS.ValorFrete -= valorDescontos;
            lancamentoNFSManual.DadosNFS.ValorDescontos = valorDescontos;

            if (lancamentoNFSManual.DadosNFS.ValorFrete <= 0)
                throw new ControllerException($"O valor total do frete (R$ {ValorFreteBruto.ToString("n2")}) deve ser superior ao valor dos descontos (R$ {valorDescontos.ToString("n2")})");
        }

        private string ObterPropriedadeOrdenarDescontos(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Carga")
                return "Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "Valor")
                return "Carga.ValorFreteResidual";

            return propriedadeOrdenar;
        }

        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa(bool usaValorResidual)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descricao", false);
            grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Local da Prestação", "LocalidadePrestacao", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Nº do Pedido no Cliente", "NumeroPedidoCliente", 20, Models.Grid.Align.left, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                grid.AdicionarCabecalho("Transportador", false);
            else
                grid.AdicionarCabecalho("Transportador", "Transportador", 25, Models.Grid.Align.left, true);

            if (usaValorResidual)
                grid.AdicionarCabecalho("Residual", "Residual", 10, Models.Grid.Align.center, false);

            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, true);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, bool somenteAtivo, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork, bool retornarListaCompleta = false)
        {
            // Instancia repositorios
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);

            // Dados do filtro
            int.TryParse(Request.Params("Empresa"), out int transportador);
            int.TryParse(Request.Params("Filial"), out int filial);
            List<int> filiais = Request.GetListParam<int>("Filiais");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                transportador = this.Empresa.Codigo;
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            {
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(this.Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato);
                transportador = empresa.Codigo;
            }

            DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicio);
            DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFim);


            int.TryParse(Request.Params("Carga"), out int carga);
            int.TryParse(Request.Params("Numero"), out int numero);
            int.TryParse(Request.Params("NumeroDOC"), out int numeroDOC);
            int.TryParse(Request.Params("LocalidadePrestacao"), out int localidadePrestacao);
            double.TryParse(Request.Params("Tomador"), out double tomador);
            int.TryParse(Request.Params("Situacao"), out int situacao);

            int.TryParse(Request.Params("Residuais"), out int pessquisaResidual);

            string numeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente");
            int.TryParse(Request.Params("Ocorrencia"), out int ocorrencia);

            bool? residual = null;
            if (pessquisaResidual == 1)
                residual = true;
            else if (pessquisaResidual == 2)
                residual = false;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual situacaoLancamentoNFSManual = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual)situacao;

            // Consulta
            List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> listaGrid = repLancamentoNFSManual.Consultar(filiais, dataInicio, dataFim, transportador, filial, carga, numero, numeroDOC, localidadePrestacao, tomador, somenteAtivo, situacaoLancamentoNFSManual, numeroPedidoCliente, ocorrencia, residual, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repLancamentoNFSManual.ContarConsulta(filiais, dataInicio, dataFim, transportador, filial, carga, numero, numeroDOC, localidadePrestacao, tomador, somenteAtivo, situacaoLancamentoNFSManual, numeroPedidoCliente, ocorrencia, residual);

            if (retornarListaCompleta)
                return listaGrid;
            else
            {
                var lista = from obj in listaGrid
                            select new
                            {
                                Codigo = obj.Codigo,
                                Numero = obj.DadosNFS?.Numero.ToString() ?? string.Empty,
                                Tomador = obj.Tomador?.Descricao ?? string.Empty,
                                Transportador = obj.Transportador?.Descricao ?? string.Empty,
                                Descricao = ((obj.DadosNFS?.Numero.ToString() + " - ") ?? string.Empty) + (obj.Transportador?.Descricao ?? string.Empty),
                                Situacao = obj.DescricaoSituacao,
                                LocalidadePrestacao = obj.LocalidadePrestacao.DescricaoCidadeEstado,
                                Residual = obj.NFSResidual == true ? "Sim" : "Não",
                                obj.NumeroPedidoCliente,
                            };

                return lista.ToList();
            }
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> BuscarDocumentosSelecionados(Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual filtrosPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> listaBusca = new List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

            //if (filtrosPesquisa.CodigoTransportador == 0)
            //    camposObricatoriosNaoInformados.Add("Transportador");

            //if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            //{
            //    if (filtrosPesquisa.CodigoTipoOperacao == 0)
            //        camposObricatoriosNaoInformados.Add("Tipo de Operação");

            //    if ((filtrosPesquisa.CodigoFilial == 0) && ((filtrosPesquisa.Codigosfilial?.Count ?? 0) == 0))
            //        camposObricatoriosNaoInformados.Add("Filial");
            //}

            //if (filtrosPesquisa.CpfCnpjTomador == 0d)
            //    camposObricatoriosNaoInformados.Add("Tomador");

            //if (camposObricatoriosNaoInformados.Count > 0)
            //{
            //    if (camposObricatoriosNaoInformados.Count > 1)
            //        throw new ControllerException($"{string.Join(", ", camposObricatoriosNaoInformados.ToArray(), startIndex: 0, count: camposObricatoriosNaoInformados.Count - 1)} e {camposObricatoriosNaoInformados.Last()} são obrigatórios.");

            //    throw new ControllerException($"{camposObricatoriosNaoInformados.First()} é obrigatório.");
            //}

            if (Request.GetBoolParam("SelecionarTodos"))
            {
                try
                {
                    if (!filtrosPesquisa.LancamentoNFSManualCancelado)
                    {
                        Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                        listaBusca = repositorioCargaDocumentoParaEmissaoNFSManual.ConsultarSelecaoNFSManual(filtrosPesquisa, parametrosConsulta: null);
                    }
                    else
                    {
                        Repositorio.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada repositorioCargaDocumentoParaEmissaoNFSManualCancelada = new Repositorio.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada(unitOfWork);
                        listaBusca = repositorioCargaDocumentoParaEmissaoNFSManualCancelada.ConsultarSelecaoNFSManual(filtrosPesquisa, parametrosConsulta: null);
                    }
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    throw new ControllerException("Ocorreu uma falha ao obter os documentos selecionados.");
                }

                dynamic listaNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosNaoSelecionadas"));

                foreach (var dynNaoSelecionada in listaNaoSelecionadas)
                    listaBusca.Remove(new Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual() { Codigo = (int)dynNaoSelecionada.Codigo });
            }
            else
            {
                dynamic listaSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("DocumentosSelecionadas"));

                foreach (var dynSelecionada in listaSelecionadas)
                    listaBusca.Add(repCargaDocumentoParaEmissaoNFSManual.BuscarPorCodigoComFetch((int)dynSelecionada.Codigo));
            }

            return FiltraSelecaoPorAmbiente(listaBusca, unitOfWork);
        }

        private List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> FiltraSelecaoPorAmbiente(List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> listaBusca, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> listaFiltrada = listaBusca;

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            {
                List<int> notasSelecionadas = (from o in listaFiltrada where o.DocumentosNFSe != null select o.DocumentosNFSe.NFSe.Codigo).ToList();
                List<int> documentosSelecionados = (from o in listaFiltrada where o.DocumentosNFSe != null select o.Codigo).ToList();

                listaFiltrada.AddRange(repCargaDocumentoParaEmissaoNFSManual.BuscarDocumentosDiferentesPertencentesAsNotas(notasSelecionadas, documentosSelecionados));
            }

            return listaFiltrada;
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Tomador") propOrdenar = "Tomador.Nome";
            else if (propOrdenar == "Transportador") propOrdenar = "Empresa.RazaoSocial";
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.NFS.DadosNFSManual dados, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            //if (dados.Numero == 0)
            //{
            //    msgErro = "Número é obrigatório.";
            //    return false;
            //}

            if (dados.Serie == null)
            {
                msgErro = "Série é obrigatório.";
                return false;
            }

            if (dados.ValorFrete == 0)
            {
                msgErro = "Valor Prestação do Serviço é obrigatório.";
                return false;
            }

            //if (dados.AliquotaISS == 0 && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (!dados.Moeda.HasValue || dados.Moeda == MoedaCotacaoBancoCentral.Real))
            //{
            //    msgErro = "Aliquota ISS é obrigatório.";
            //    return false;
            //}

            /*if (dados.ValorISS == 0)
            {
                msgErro = "Valor ISS é obrigatório.";
                return false;
            }

            if (dados.ValorBaseCalculo == 0)
            {
                msgErro = "Base de Cálculo é obrigatório.";
                return false;
            }*/

            //if (dados.PercentualRetencao == 0)
            //{
            //    msgErro = "Percentual de Retenção é obrigatório.";
            //    return false;
            //}

            return true;
        }

        private string PreencherEntidade(ref Dominio.Entidades.Embarcador.NFS.DadosNFSManual dados, Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual, Repositorio.UnitOfWork unitOfWork)
        {
            // Repositorios
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.NFS.DadosNFSManual repDadosNFSManual = new Repositorio.Embarcador.NFS.DadosNFSManual(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
            Repositorio.ServicoNFSe repServicoNFSe = new Repositorio.ServicoNFSe(unitOfWork);

            // Dados CTe
            int codigoModeloDocumentoFiscal = Request.GetIntParam("ModeloDocumentoFiscal");
            int codigoLocalidadePrestacao = Request.GetIntParam("LocalidadePrestacao");
            int.TryParse(Request.Params("NumeroRPS"), out int numeroRPS);
            int.TryParse(Request.Params("Numero"), out int numero);
            int.TryParse(Request.Params("Serie"), out int numeroSerie);

            bool.TryParse(Request.Params("IncluirValorBC"), out bool incluirValorBC);
            bool.TryParse(Request.Params("ConsiderarLocalidadeCarga"), out bool considerarLocalidadeCarga);

            decimal.TryParse(Request.Params("ValorPrestacaoServico"), out decimal valorPrestacao);
            decimal.TryParse(Request.Params("AliquotaISS"), out decimal aliquotaISS);
            decimal.TryParse(Request.Params("ValorISS"), out decimal valorISS);
            decimal.TryParse(Request.Params("BaseCalculo"), out decimal baseCalculo);
            decimal.TryParse(Request.Params("ValorRetencao"), out decimal valorRetencao);
            decimal.TryParse(Request.Params("PercentualRetencao"), out decimal percentualRetencao);
            decimal.TryParse(Request.Params("ValorPIS"), out decimal valorPIS);
            decimal.TryParse(Request.Params("ValorCOFINS"), out decimal valorCOFINS);
            decimal.TryParse(Request.Params("ValorPISIBSCBS"), out decimal valorPISIBSCBS);
            decimal.TryParse(Request.Params("ValorCOFINSIBSCBS"), out decimal valorCOFINSIBSCBS);
            decimal.TryParse(Request.Params("ValorIR"), out decimal valorIR);
            decimal.TryParse(Request.Params("ValorCSLL"), out decimal valorCSLL);
            decimal.TryParse(Request.Params("ValorReceber"), out decimal valorReceber);

            string CSTIBSCBS = Request.Params("CSTIBSCBS");
            string classificacaoTributariaIBSCBS = Request.Params("ClassificacaoTributariaIBSCBS");
            decimal.TryParse(Request.Params("BaseCalculoIBSCBS"), out decimal baseCalculoIBSCBS);
            decimal.TryParse(Request.Params("AliquotaCBS"), out decimal aliquotaCBS);
            decimal.TryParse(Request.Params("ValorCBS"), out decimal valorCBS);
            decimal.TryParse(Request.Params("AliquotaIBSEstadual"), out decimal aliquotaIBSEstadual);
            decimal.TryParse(Request.Params("ValorIBSEstadual"), out decimal valorIBSEstadual);
            decimal.TryParse(Request.Params("AliquotaIBSMunicipal"), out decimal aliquotaIBSMunicipal);
            decimal.TryParse(Request.Params("ValorIBSMunicipal"), out decimal valorIBSMunicipal);

            string NBS = Request.Params("NBS");
            string IndicadorOperacao = Request.Params("IndicadorOperacao");
            decimal percentualReducaoIBSMunicipal = Request.GetDecimalParam("PercentualReducaoIBSMunicipal");
            decimal percentualReducaoIBSEstadual = Request.GetDecimalParam("PercentualReducaoIBSEstadual");
            decimal percentualReducaoCBS = Request.GetDecimalParam("PercentualReducaoCBS");

            int codigoTransportador = Request.GetIntParam("Transportador");
            int codigoFilial = Request.GetIntParam("Filial");
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            double cpfCnpjTomador = Request.GetDoubleParam("Tomador");
            int codigoServicoNFSe = Request.GetIntParam("ServicoNFSe");

            Dominio.Entidades.Empresa transportador = repositorioEmpresa.BuscarPorCodigo(codigoTransportador);
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo(codigoFilial);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);
            Dominio.Entidades.Cliente tomador = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjTomador);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

            if (transportador != null)
                nfsManual.Transportador = transportador;
            else
                throw new ControllerException("É necessário informar uma Transportadora!");

            if (filial != null || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                nfsManual.Filial = filial;
            else
                throw new ControllerException("É necessário informar uma Filial!");

            if (tipoOperacao != null)
                nfsManual.TipoOperacao = tipoOperacao;
            else
                throw new ControllerException("É necessário informar um Tipo de Operação!");

            if (tomador != null)
                nfsManual.Tomador = tomador;
            else
                throw new ControllerException("É necessário informar um Tomador!");

            Dominio.Entidades.Empresa empresa = nfsManual.Transportador;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamentoNFSManual tipoArredondamentoISS = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamentoNFSManual>("TipoArredondamentoISS");

            DateTime? dataEmissao = null;
            if (DateTime.TryParse(Request.Params("DataEmissao"), out DateTime dataEmissaoAux))
                dataEmissao = dataEmissaoAux;

            string observacao = Request.Params("Observacao") ?? string.Empty;

            Dominio.Entidades.Localidade localidade = null;
            if (ConfiguracaoEmbarcador.EmitirNFSManualParaTransportadorEFiliais)
            {
                localidade = repLocalidade.BuscarPorCodigo(codigoLocalidadePrestacao);
                if (localidade == null)
                    return "É obrigatório informar a localidade da prestação do serviço.";

                nfsManual.LocalidadePrestacao = localidade;
                repLancamentoNFSManual.Atualizar(nfsManual);
            }

            if ((ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira || configGeral.AlterarModeloDocumentoNFSManual) && codigoModeloDocumentoFiscal > 0)
                dados.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorCodigo(codigoModeloDocumentoFiscal, false);
            else
                dados.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorTipoDocumento(Dominio.Enumeradores.TipoDocumento.NFS); //todo: depois implementar emissão NFS-e

            if (dados.ModeloDocumentoFiscal == null)
                return "Modelo de Documento não configurado, é necessário configurar para seguir com a operação";

            Dominio.Enumeradores.TipoSerie tipoSerie = Dominio.Enumeradores.TipoSerie.NFSe;
            if (dados.ModeloDocumentoFiscal?.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros)
                tipoSerie = Dominio.Enumeradores.TipoSerie.OutrosDocumentos;

            // Cria serie
            Dominio.Entidades.EmpresaSerie serie = repEmpresaSerie.BuscarPorSerie(empresa.Codigo, numeroSerie, tipoSerie);
            if (serie == null && numeroSerie > 0)
            {
                serie = new Dominio.Entidades.EmpresaSerie()
                {
                    Empresa = empresa,
                    Numero = numeroSerie,
                    Status = "A",
                    Tipo = tipoSerie,
                };
                repEmpresaSerie.Inserir(serie, Auditado);
            }

            Dominio.Entidades.Embarcador.NFS.DadosNFSManual dadosExiste = null;
            if (numero > 0)
                dadosExiste = repDadosNFSManual.BuscarPorNumeroSerieEmpresa(numero, serie.Numero, empresa.Codigo);
            else if (numeroRPS > 0)
                dadosExiste = repDadosNFSManual.BuscarPorNumeroRPSSerieEmpresa(numeroRPS, serie.Numero, empresa.Codigo);

            if (dadosExiste != null && dadosExiste.Codigo != dados.Codigo)
                return "Já foi informado uma NFS com esse número e serie.";

            dados.TipoArredondamentoISS = tipoArredondamentoISS;
            dados.Numero = numero;
            dados.Serie = serie;

            dados.IncluirISSBC = incluirValorBC;
            dados.ConsiderarLocalidadeCarga = considerarLocalidadeCarga;
            dados.DataEmissao = dataEmissao;
            dados.ServicoNFSe = repServicoNFSe.BuscarPorCodigo(codigoServicoNFSe);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                dados.ValorFrete = valorPrestacao;

            if (aliquotaISS > 100)
                return "A aliquota do ISS não pode ser superior a 100%.";

            if (percentualRetencao > 100)
                return "O percentual de retenção não pode ser superior a 100%.";

            if ((valorPIS + valorCOFINS + valorCSLL + valorIR) > dados.ValorFrete)
                return "O valor dos impostos retidos (PIS, COFINS, CSLL e IR) não pode ser maior que o valor total do frete.";

            dados.AliquotaISS = aliquotaISS;
            dados.ValorISS = valorISS;
            dados.ValorBaseCalculo = baseCalculo;
            dados.PercentualRetencao = percentualRetencao;
            dados.ValorRetido = valorRetencao;
            dados.Observacoes = observacao;
            dados.ValorCOFINS = valorCOFINS;
            dados.ValorPISIBSCBS = valorPISIBSCBS;
            dados.ValorCOFINSIBSCBS = valorCOFINSIBSCBS;
            dados.ValorCSLL = valorCSLL;
            dados.ValorPIS = valorPIS;
            dados.ValorIR = valorIR;
            dados.ValorReceber = valorReceber;
            dados.CSTIBSCBS = CSTIBSCBS;
            dados.ClassificacaoTributariaIBSCBS = classificacaoTributariaIBSCBS;
            dados.BaseCalculoIBSCBS = baseCalculoIBSCBS;
            dados.AliquotaCBS = aliquotaCBS;
            dados.ValorCBS = valorCBS;
            dados.AliquotaIBSEstadual = aliquotaIBSEstadual;
            dados.ValorIBSEstadual = valorIBSEstadual;
            dados.AliquotaIBSMunicipal = aliquotaIBSMunicipal;
            dados.ValorIBSMunicipal = valorIBSMunicipal;
            dados.NBS = NBS;
            dados.IndicadorOperacao = IndicadorOperacao;
            dados.PercentualReducaoIBSMunicipal = percentualReducaoIBSMunicipal;
            dados.PercentualReducaoIBSEstadual = percentualReducaoIBSEstadual;
            dados.PercentualReducaoCBS = percentualReducaoCBS;

            return "";
        }

        private string CorAprovacao(Dominio.Entidades.Embarcador.NFS.LancamentoNFSAutorizacao autorizacao)
        {
            if (autorizacao.Bloqueada)
                return "#BEBEBE";

            if (autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;

            if (autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;

            if (autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAutorizacaoAlcada.Pendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Warning;

            return "";
        }

        private dynamic ResumoAutorizacao(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao repLancamentoNFSAutorizacao = new Repositorio.Embarcador.NFS.LancamentoNFSAutorizacao(unitOfWork);

            int aprovacoesNecessarias = repLancamentoNFSAutorizacao.ContarAprovacoesNecessarias(nfsManual.Codigo);
            int aprovacoes = repLancamentoNFSAutorizacao.ContarAprovacoes(nfsManual.Codigo);
            int reprovacoes = repLancamentoNFSAutorizacao.ContarReprovacoes(nfsManual.Codigo);
            return new
            {
                Solicitante = nfsManual.Usuario?.Nome ?? "",
                DataSolicitacao = nfsManual.DataCriacao.ToString("dd/MM/yyyy"),
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                Reprovacoes = reprovacoes,
                Situacao = nfsManual.DescricaoSituacao,
            };
        }

        private string SalvarDANFSE(int numero, int serie, Dominio.Entidades.Empresa empresa, byte[] pdfData, Repositorio.UnitOfWork unitOfWork)
        {
            string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatorios;

            if (!string.IsNullOrWhiteSpace(caminhoRelatorios))
            {
                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, "NFSe", empresa.CNPJ, numero.ToString() + "_" + serie.ToString()) + ".pdf";

                string pasta = Path.GetDirectoryName(caminhoPDF);

                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoPDF, pdfData);

                return caminhoPDF;
            }
            else
            {
                return "";
            }
        }

        private string SalvarAnexo(int numero, int serie, Dominio.Entidades.Empresa empresa, string extensao, byte[] pdfData, Repositorio.UnitOfWork unitOfWork)
        {

            string nomeArquivo = empresa.CNPJ + "_" + numero.ToString() + "_" + serie.ToString() + extensao;
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(CaminhoArquivo(unitOfWork), nomeArquivo);

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminho, pdfData);

            return nomeArquivo;
        }

        private bool ShouldGerarAprovacaoNFSManual(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            bool possuiIntegracaoSaintgobin = repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.SaintGobain);

            if (possuiIntegracaoSaintgobin)
                return nfsManual.Tomador?.GrupoPessoas == null || nfsManual.Tomador.GrupoPessoas.ControlaPagamentos;

            return true;
        }

        private string CaminhoArquivo(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "NFSManual", "Anexo" });
        }

        private void GridNFSeGerada(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual, ref Models.Grid.Grid grid, out dynamic lista, out int totalRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NFS.LancamentoNFSManual repNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("SituacaoCTe", false);
            grid.AdicionarCabecalho("CodigoCTE", false);
            grid.AdicionarCabecalho("NumeroModeloDocumentoFiscal", false);
            grid.AdicionarCabecalho("TipoDocumentoEmissao", false);
            grid.AdicionarCabecalho("CodigoEmpresa", false);
            grid.AdicionarCabecalho("HabilitarSincronizarDocumento", false);
            grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Serie", "Serie", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Nota(s) Fiscai(s)", "NumeroNotas", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("ISS", "Aliquota", 5, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("IBS Município", "IBSMunicipio", 5, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("IBS UF", "IBSUF", 5, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("CBS", "CBS", 5, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Status", "Status", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Retorno Sefaz", "RetornoSefaz", 15, Models.Grid.Align.left, false, false);


            // Busca Dados
            List<Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual> listaGrid = repNFSManual.ConsultarPorLancamento(nfsManual.Codigo, grid.inicio, grid.limite);
            totalRegistros = repNFSManual.ContarConsultaPorLancamento(nfsManual.Codigo);

            // Converte dados
            lista = (from obj in listaGrid
                     select new
                     {
                         obj.Codigo,
                         CodigoCTE = obj.CTe.Codigo,
                         obj.CTe.DescricaoTipoServico,
                         NumeroModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Numero,
                         AbreviacaoModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Abreviacao,
                         obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao,
                         CodigoEmpresa = obj.CTe.Empresa.Codigo,
                         obj.CTe.Numero,
                         SituacaoCTe = obj.CTe.Status,
                         Serie = obj.CTe.Serie.Numero,
                         obj.CTe.DescricaoTipoPagamento,
                         Remetente = obj.CTe.Remetente.Nome + "(" + obj.CTe.Remetente.CPF_CNPJ_Formatado + ")",
                         Destinatario = obj.CTe.Destinatario.Nome + "(" + obj.CTe.Destinatario.CPF_CNPJ_Formatado + ")",
                         Destino = obj.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                         ValorFrete = obj.CTe.ValorAReceber.ToString("n2"),
                         Aliquota = obj.CTe.AliquotaImposto.ToString("n2"),
                         IBSMunicipio = obj.CTe.AliquotaIBSMunicipal.ToString("n2"),
                         IBSUF = obj.CTe.AliquotaIBSEstadual.ToString("n2"),
                         CBS = obj.CTe.AliquotaCBS.ToString("n2"),
                         obj.CTe.NumeroNotas,
                         Status = obj.CTe.DescricaoStatus,
                         RetornoSefaz = !string.IsNullOrWhiteSpace(obj.CTe.MensagemRetornoSefaz) ? (obj.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.CTe.MensagemRetornoSefaz) : "") : "",
                         HabilitarSincronizarDocumento = obj.CTe.Status == "E" && obj.CTe.DataIntegracao != null && (System.DateTime.Now.AddMinutes(-30) > obj.CTe.DataIntegracao) && ((obj.CTe.CodigoCTeIntegrador != 0 && obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe) || (obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe && obj.CTe.SistemaEmissor == TipoEmissorDocumento.Migrate)) ? true : false,
                     }).ToList();
        }

        private void GridNFSeVinculadas(Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual nfsManual, ref Models.Grid.Grid grid, out dynamic lista, out int total, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NFS.LancamentoNFSManual repNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("SituacaoNFSe", false);
            grid.AdicionarCabecalho("CodigoNFSe", false);
            grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Serie", "Serie", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Localidade Prest.", "LocalidadePrestacaoServico", 18, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tomador", "Tomador", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Valor a Receber", "ValorServicos", 8, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Alíquota ISS", "AliquotaISS", 5, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Documentos", "NumeroDocumentos", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Observação", "OutrasInformacoes", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Status", "Status", 8, Models.Grid.Align.center, true);


            // Busca Dados
            List<Dominio.Entidades.NFSe> listaGrid = (from o in nfsManual.Documentos where o.DocumentosNFSe != null select o.DocumentosNFSe.NFSe).Distinct().ToList();
            total = listaGrid.Count();

            // Converte dados
            lista = (from obj in listaGrid
                     select new
                     {
                         obj.Codigo,
                         CodigoNFSe = obj.Codigo,
                         obj.Numero,
                         SituacaoNFSe = obj.Status,
                         Serie = obj.Serie.Numero,
                         Tomador = obj.Tomador.Nome + "(" + obj.Tomador.CPF_CNPJ_Formatado + ")",
                         LocalidadePrestacaoServico = obj.LocalidadePrestacaoServico.DescricaoCidadeEstado,
                         ValorServicos = obj.ValorServicos.ToString("n2"),
                         AliquotaISS = obj.AliquotaISS.ToString("n2"),
                         obj.OutrasInformacoes,
                         obj.NumeroDocumentos,
                         Status = obj.DescricaoStatus,
                     }).ToList();
        }

        private IActionResult ObterGridPesquisaDocumento(Repositorio.UnitOfWork unitOfWork, bool exportacao = false)
        {
            Models.Grid.EditableCell editableValorString = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 9);

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("TipoOperacao", false);
            grid.AdicionarCabecalho("CodigoFilial", false);
            grid.AdicionarCabecalho("CodigoTipoOperacao", false);
            grid.AdicionarCabecalho("Transportador", false);
            grid.AdicionarCabecalho("CodigoTransportador", false);
            grid.AdicionarCabecalho("CodigoTomador", false);
            grid.AdicionarCabecalho("CodigoDestinatario", false);
            grid.AdicionarCabecalho("CodigoFechamentoFrete", false);
            grid.AdicionarCabecalho("Carga", "Carga", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Ocorrência", "Ocorrencia", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Fechamento", "FechamentoFrete", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tomador", "Tomador", 18, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Localidade", "LocalidadePrestacao", 13, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Nº do Pedido no Cliente", "NumeroPedidoCliente", 13, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 8, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("CNPJ Filial", "CNPJFilial", 10, Models.Grid.Align.center, true);

            if (ConfiguracaoEmbarcador.UtilizaMoedaEstrangeira)
            {
                grid.AdicionarCabecalho("Moeda", "Moeda", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Moeda", "ValorTotalMoeda", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "ValorFrete", 8, Models.Grid.Align.right, true, false, false, false, true, editableValorString);
            }
            else
            {
                grid.AdicionarCabecalho("Valor", "ValorFrete", 13, Models.Grid.Align.right, true, false, false, false, true, editableValorString);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                grid.AdicionarCabecalho("NFS-e", "NFSe", 8, Models.Grid.Align.right, true);
            else
                grid.AdicionarCabecalho("NFSe", false);

            if (ConfiguracaoEmbarcador.GerarPagamentoBloqueado)
                grid.AdicionarCabecalho("Digitalização Canhoto", "SituacaoDigitalizacaoCanhoto", 8, Models.Grid.Align.left, false);

            int totalRegistros = 0;
            List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> listaDocumentos = null;
            Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual filtrosPesquisa = ObterFiltroPesquisaCargaDocumentoParaEmissaoNFSManual(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarDocumento);

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "NFSManual/PesquisaDocumento", "grid-lancamento-nfs-manual");
            grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            if (!filtrosPesquisa.LancamentoNFSManualCancelado)
            {
                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);

                totalRegistros = repositorioCargaDocumentoParaEmissaoNFSManual.ContarConsultaSelecaoNFSManual(filtrosPesquisa);
                listaDocumentos = (totalRegistros > 0) ? repositorioCargaDocumentoParaEmissaoNFSManual.ConsultarSelecaoNFSManual(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            }
            else
            {
                Repositorio.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada repositorioCargaDocumentoParaEmissaoNFSManualCancelada = new Repositorio.Embarcador.NFS.CargaDocumentoParaEmissaoNFSManualCancelada(unitOfWork);

                totalRegistros = repositorioCargaDocumentoParaEmissaoNFSManualCancelada.ContarConsultaSelecaoNFSManual(filtrosPesquisa);
                listaDocumentos = (totalRegistros > 0) ? repositorioCargaDocumentoParaEmissaoNFSManualCancelada.ConsultarSelecaoNFSManual(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            }

            var listaDocumentosRetornar = (
                from o in listaDocumentos
                select new
                {
                    Codigo = o.Codigo,
                    CodigoDestinatario = o.Destinatario.CPF_CNPJ,
                    SituacaoDigitalizacaoCanhoto = o.PedidoXMLNotaFiscal?.XMLNotaFiscal.Canhoto?.DescricaoDigitalizacao ?? "",
                    CodigoFilial = o.CargaOrigem?.Filial?.Codigo,
                    CodigoTipoOperacao = o.CargaOrigem?.TipoOperacao?.Codigo ?? 0,
                    TipoOperacao = o.CargaOrigem?.TipoOperacao?.Descricao ?? string.Empty,
                    Transportador = o.CargaOrigem?.Empresa?.RazaoSocial,
                    CodigoTransportador = o.CargaOrigem?.Empresa?.Codigo,
                    CodigoTomador = o.Tomador.Codigo,
                    CodigoFechamentoFrete = o.FechamentoFrete?.Codigo ?? 0,
                    NFSe = o.DocumentosNFSe?.NFSe.Numero.ToString() ?? string.Empty,
                    DataEmissao = o.DataEmissao.ToString("dd/MM/yyyy"),
                    LocalidadePrestacao = o.LocalidadePrestacao.DescricaoCidadeEstado,
                    Carga = o.CargaOrigem?.CodigoCargaEmbarcador ?? "",
                    Numero = o.Numero.ToString(),
                    Ocorrencia = o.CargaOcorrencia?.NumeroOcorrencia.ToString() ?? string.Empty,
                    FechamentoFrete = o.FechamentoFrete?.Descricao ?? string.Empty,
                    Destinatario = o.Destinatario.NomeCNPJ,
                    Tomador = o.Tomador.NomeCNPJ,
                    Moeda = o.Moeda?.ObterDescricao() ?? MoedaCotacaoBancoCentral.Real.ObterDescricao(),
                    ValorTotalMoeda = (o.ValorTotalMoeda ?? 0m).ToString("n2"),
                    ValorFrete = o.ValorFrete.ToString("n2"),
                    DT_RowColor = o.AlterouValorFreteNFsManual ? "#FFFFE0" : "#FFFFFF",
                    NumeroPedidoCliente = o.NumeroPedidoCliente,
                    Filial = o.CargaOrigem?.Filial?.Descricao ?? string.Empty,
                    CNPJFilial = o.CargaOrigem?.Filial?.CNPJ_Formatado ?? string.Empty
                }
            ).ToList();

            grid.AdicionaRows(listaDocumentosRetornar);
            grid.setarQuantidadeTotal(totalRegistros);

            if (exportacao)
            {
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            else
            {
                return new JsonpResult(grid);
            }

        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoFiltros()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Chave do Documento", Propriedade = "ChaveDocumento", Tamanho = 100, Obrigatorio = true, Regras = new List<string> { "required" } });

            return configuracoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacaoValorFrete()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Número Pedido", Propriedade = "NumeroPedido", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Valor Frete", Propriedade = "ValorFrete", Tamanho = 9, Obrigatorio = true, Regras = new List<string> { "required" } });

            return configuracoes;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        public async Task<IActionResult> ValidarLocalidadePrestacaoConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoNFSeManual repConfiguracaoNFSeManual = new Repositorio.Embarcador.Configuracoes.ConfiguracaoNFSeManual(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoNFSeManual configuracaoNFSeManual = repConfiguracaoNFSeManual.BuscarConfiguracaoPadrao();

                string mensagem = "";

                if (configuracaoNFSeManual?.ValidarLocalidadePrestacaoTransportadorConfiguracaoNFSe ?? false)
                {
                    Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaCargaDocumentoParaEmissaoNFSManual filtrosPesquisa = ObterFiltroPesquisaCargaDocumentoParaEmissaoNFSManual(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentos = BuscarDocumentosSelecionados(filtrosPesquisa, unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual referencia = documentos[0];

                    Dominio.Entidades.Empresa transportador = filtrosPesquisa.CodigoTransportador > 0 ? referencia.Carga.Empresa : null;

                    if (transportador.TransportadorConfiguracoesNFSe.Where(x => x.LocalidadePrestacao == referencia.LocalidadePrestacao).Count() == 0)
                    {
                        mensagem = "AVISO: Localidade de prestação (" + referencia.LocalidadePrestacao.DescricaoCidadeEstado + ") não cadastrada na tela de transportador (" + transportador.CNPJ_Formatado + "), favor cadastrar a mesma para que seja informado a alíquota! ,";
                    }

                }
                // Retorna sucesso
                return new JsonpResult(new
                {
                    MensagemAviso = mensagem
                });
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
