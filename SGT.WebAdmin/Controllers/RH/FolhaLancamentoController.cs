using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.RH
{
    [CustomAuthorize("RH/FolhaLancamento")]
    public class FolhaLancamentoController : BaseController
    {
		#region Construtores

		public FolhaLancamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Funcionario"), out int funcionario);
                int.TryParse(Request.Params("FolhaInformacao"), out int folhaInformacao);
                int.TryParse(Request.Params("NumeroEvento"), out int numeroEvento);
                int.TryParse(Request.Params("NumeroContrato"), out int numeroContrato);

                string descricao = Request.Params("Descricao");

                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 28, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Número Evento", "NumeroEvento", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Número Contrato", "NumeroContrato", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Inicial", "DataInicial", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Final", "DataFinal", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Funcionário", "Funcionario", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, true);

                Repositorio.Embarcador.RH.FolhaLancamento repFolhaLancamento = new Repositorio.Embarcador.RH.FolhaLancamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.RH.FolhaLancamento> informacoesFolha = repFolhaLancamento.Consultar(descricao, numeroEvento, numeroContrato, dataInicial, dataFinal, funcionario, folhaInformacao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFolhaLancamento.ContarConsulta(descricao, numeroEvento, numeroContrato, dataInicial, dataFinal, funcionario, folhaInformacao));

                var lista = (from p in informacoesFolha
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.NumeroEvento,
                                 p.NumeroContrato,
                                 DataInicial = p.DataInicial.ToString("dd/MM/yyyy"),
                                 DataFinal = p.DataFinal.ToString("dd/MM/yyyy"),
                                 Funcionario = p.Funcionario != null ? p.Funcionario.Descricao : string.Empty,
                                 Valor = p.Valor.ToString("n2")
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
                unitOfWork.Start();

                Repositorio.Embarcador.RH.FolhaLancamento repFolhaLancamento = new Repositorio.Embarcador.RH.FolhaLancamento(unitOfWork);
                Dominio.Entidades.Embarcador.RH.FolhaLancamento folhaLancamento = new Dominio.Entidades.Embarcador.RH.FolhaLancamento();

                PreencherFolhaLancamento(folhaLancamento, unitOfWork);

                if (folhaLancamento.DataCompetencia.HasValue && folhaLancamento.DataCompetencia.Value > DateTime.MinValue && folhaLancamento.DataCompetencia.Value.Date < DateTime.Now.Date)
                    throw new ControllerException("A data de competência não pode ser retroativa.");

                repFolhaLancamento.Inserir(folhaLancamento, Auditado);

                Servicos.Embarcador.RH.FolhaLancamento svcFolhaLancamento = new Servicos.Embarcador.RH.FolhaLancamento(unitOfWork);
                if (ConfiguracaoEmbarcador.GerarTituloFolhaPagamento)
                    svcFolhaLancamento.GerarTituloFolhaLancamento(folhaLancamento, Usuario, TipoServicoMultisoftware, Empresa.TipoAmbiente);
                else
                    svcFolhaLancamento.GerarMovimentoFinanceiroFolhaLancamento(folhaLancamento, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.RH.FolhaLancamento repFolhaLancamento = new Repositorio.Embarcador.RH.FolhaLancamento(unitOfWork);
                Dominio.Entidades.Embarcador.RH.FolhaLancamento folhaLancamento = repFolhaLancamento.BuscarPorCodigo(codigo, true);

                if (folhaLancamento.Titulo != null)
                    return new JsonpResult(false, true, "Este lançamento já possui títulos gerados, impossível alterar o mesmo.");

                PreencherFolhaLancamento(folhaLancamento, unitOfWork);
                repFolhaLancamento.Atualizar(folhaLancamento, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.RH.FolhaLancamento repFolhaLancamento = new Repositorio.Embarcador.RH.FolhaLancamento(unitOfWork);
                Dominio.Entidades.Embarcador.RH.FolhaLancamento folhaLancamento = repFolhaLancamento.BuscarPorCodigo(codigo, false);

                if (folhaLancamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynFolhaLancamento = new
                {
                    folhaLancamento.Codigo,
                    folhaLancamento.Descricao,
                    folhaLancamento.NumeroEvento,
                    folhaLancamento.NumeroContrato,
                    DataInicial = folhaLancamento.DataInicial.ToString("dd/MM/yyyy"),
                    DataFinal = folhaLancamento.DataFinal.ToString("dd/MM/yyyy"),
                    DataCompetencia = folhaLancamento.DataCompetencia.HasValue ? folhaLancamento.DataCompetencia.Value.ToString("dd/MM/yyyy") : "",
                    Base = folhaLancamento.Base.ToString("n2"),
                    Referencia = folhaLancamento.Referencia.ToString("n2"),
                    Valor = folhaLancamento.Valor.ToString("n2"),
                    Funcionario = folhaLancamento.Funcionario != null ? new { folhaLancamento.Funcionario.Codigo, folhaLancamento.Funcionario.Descricao } : null,
                    FolhaInformacao = folhaLancamento.FolhaInformacao != null ? new { folhaLancamento.FolhaInformacao.Codigo, folhaLancamento.FolhaInformacao.Descricao } : null
                };

                return new JsonpResult(dynFolhaLancamento);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.RH.FolhaLancamento repFolhaLancamento = new Repositorio.Embarcador.RH.FolhaLancamento(unitOfWork);
                Dominio.Entidades.Embarcador.RH.FolhaLancamento folhaLancamento = repFolhaLancamento.BuscarPorCodigo(codigo, true);

                if (folhaLancamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (folhaLancamento.Titulo != null)
                    return new JsonpResult(false, true, "Este lançamento já possui títulos gerados, impossível excluir o mesmo.");

                repFolhaLancamento.Deletar(folhaLancamento, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ImportarFolha()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.RH.FolhaLancamento repFolhaLancamento = new Repositorio.Embarcador.RH.FolhaLancamento(unitOfWork);

                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);
                DateTime.TryParse(Request.Params("DataCompetencia"), out DateTime dataCompetencia);

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count > 0)
                {
                    Servicos.DTO.CustomFile file = files[0];
                    string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
                    List<Dominio.Entidades.Embarcador.RH.FolhaLancamento> listaFolha = null;
                    int qtdRegistrosNaoImportado = 0, qtdRegistrosInseridos = 0, qtdRegistrosDuplicados = 0;

                    if (fileExtension.ToLower() == ".xlsx" || fileExtension.ToLower() == ".txt")
                    {
                        Servicos.Embarcador.RH.FolhaLancamento svcFolhaLancamento = new Servicos.Embarcador.RH.FolhaLancamento(unitOfWork);
                        listaFolha = svcFolhaLancamento.ProcessarArquivoFolha(file.InputStream, fileExtension, out qtdRegistrosNaoImportado);

                        if (listaFolha != null && listaFolha.Count() > 0)
                        {
                            unitOfWork.Start();

                            foreach (Dominio.Entidades.Embarcador.RH.FolhaLancamento folha in listaFolha)
                            {
                                if (!repFolhaLancamento.ContemFolha(folha.Funcionario.Codigo, folha.FolhaInformacao.Codigo, dataInicial, dataFinal, folha.Valor))
                                {
                                    folha.DataInicial = dataInicial;
                                    folha.DataFinal = dataFinal;
                                    folha.DataCompetencia = dataCompetencia;

                                    repFolhaLancamento.Inserir(folha, Auditado);

                                    if (ConfiguracaoEmbarcador.GerarTituloFolhaPagamento)
                                        svcFolhaLancamento.GerarTituloFolhaLancamento(folha, Usuario, TipoServicoMultisoftware, Empresa.TipoAmbiente);
                                    else
                                        svcFolhaLancamento.GerarMovimentoFinanceiroFolhaLancamento(folha, TipoServicoMultisoftware);

                                    qtdRegistrosInseridos++;
                                }
                                else
                                    qtdRegistrosDuplicados++;
                            }

                            unitOfWork.CommitChanges();

                            var mensagem = "Registros inseridos: " + qtdRegistrosInseridos.ToString() +
                                    "<br/>Registros sem configuração: " + qtdRegistrosNaoImportado.ToString() +
                                    "<br/>Registros já cadastrados: " + qtdRegistrosDuplicados.ToString();

                            if (qtdRegistrosInseridos == 0)
                                return new JsonpResult(false, true, mensagem);
                            else
                                return new JsonpResult(true, mensagem);
                        }
                        else
                            return new JsonpResult(false, true, "Nenhum registro para importação encontrado!");
                    }
                    else
                        return new JsonpResult(false, "Formato do arquivo selecionado é inválido! Somente é permitido importar nos formatos .xlsx e .txt");
                }
                else
                    return new JsonpResult(false, "Arquivo não encontrado, por favor verifique!");
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar as folhas. <br/>Erro: " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherFolhaLancamento(Dominio.Entidades.Embarcador.RH.FolhaLancamento folhaLancamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.RH.FolhaInformacao repFolhaInformacao = new Repositorio.Embarcador.RH.FolhaInformacao(unitOfWork);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unitOfWork);

            int codigoFuncionario = Request.GetIntParam("Funcionario");
            int codigoFolhaInformacao = Request.GetIntParam("FolhaInformacao");

            folhaLancamento.Descricao = Request.GetStringParam("Descricao");
            folhaLancamento.NumeroEvento = Request.GetIntParam("NumeroEvento");
            folhaLancamento.NumeroContrato = Request.GetIntParam("NumeroContrato");
            folhaLancamento.DataInicial = Request.GetDateTimeParam("DataInicial");
            folhaLancamento.DataFinal = Request.GetDateTimeParam("DataFinal");
            folhaLancamento.Base = Request.GetDecimalParam("Base");
            folhaLancamento.Referencia = Request.GetDecimalParam("Referencia");
            folhaLancamento.Valor = Request.GetDecimalParam("Valor");
            folhaLancamento.DataCompetencia = Request.GetNullableDateTimeParam("DataCompetencia");

            folhaLancamento.FolhaInformacao = codigoFolhaInformacao > 0 ? repFolhaInformacao.BuscarPorCodigo(codigoFolhaInformacao) : null;
            folhaLancamento.Funcionario = codigoFuncionario > 0 ? repFuncionario.BuscarPorCodigo(codigoFuncionario) : null;
        }

        #endregion
    }
}
