using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    public class AcompanhamentoContaController : BaseController
    {
        #region Construtores

        public AcompanhamentoContaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisaPadrao(unitOfWork));
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

        public async Task<IActionResult> DownloadArquivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoConta = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.ContaPagar repositorioContaPagar = new Repositorio.Embarcador.Financeiro.ContaPagar(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContaPagar existeContaParaPagar = repositorioContaPagar.BuscarPorCodigo(codigoConta, false);

                if (existeContaParaPagar == null)
                    return new JsonpResult(false, "Não encontrado arquivo para este registro");

                string caminhoArquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.ObterCaminhoArquivoIntegracao(unitOfWork);
                string localArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivo, existeContaParaPagar.ArquivoAProcessar.NomeArquivo);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(localArquivo))
                    return new JsonpResult(true, "Arquivo não encontrado para Download");

                byte[] arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(localArquivo);

                return Arquivo(arquivo, "application/csv", existeContaParaPagar.NomeOriginalArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download da remessa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigoConta = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.ContaPagar repositorioContaPagar = new Repositorio.Embarcador.Financeiro.ContaPagar(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContaPagar existeContaParaPagar = repositorioContaPagar.BuscarPorCodigo(codigoConta, false);

                if (existeContaParaPagar == null)
                    return new JsonpResult(false, "Registro para cancelar não encontrado");

                existeContaParaPagar.Situacao = SituacaoProcessamentoArquivo.AguardandoProcessamento;
                existeContaParaPagar.MensagemProcessamento = "";
                existeContaParaPagar.DataIntegracao = DateTime.Now;

                List<string> mensagems = existeContaParaPagar.MensagensProcessamento.ToList();

                foreach (var mensagem in mensagems)
                    existeContaParaPagar.MensagensProcessamento.Remove(mensagem);

                repositorioContaPagar.Atualizar(existeContaParaPagar);
                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Registro enviado para reprocessamento");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Erros ao tentar reprocessar arquivos");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LogErros()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoConta = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.ContaPagar repositorioContaPagar = new Repositorio.Embarcador.Financeiro.ContaPagar(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContaPagar existeContaParaPagar = repositorioContaPagar.BuscarPorCodigo(codigoConta, false);

                if (existeContaParaPagar == null)
                    return new JsonpResult(false, "Registro para cancelar não encontrado");

                Models.Grid.Grid grid = new Grid
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Mensagem", "MensagemProcessamento", 10, Align.left, false, true);
                dynamic lista = (from obj in existeContaParaPagar.MensagensProcessamento
                                 select new
                                 {
                                     MensagemProcessamento = obj
                                 }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(existeContaParaPagar.MensagensProcessamento.Count);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter logs de erros");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelamentoProcessamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoConta = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.ContaPagar repositorioContaPagar = new Repositorio.Embarcador.Financeiro.ContaPagar(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.ContaPagar existeContaParaPagar = repositorioContaPagar.BuscarPorCodigo(codigoConta, false);

                if (existeContaParaPagar == null)
                    return new JsonpResult(false, "Regitro para cancelar não encontrado");

                if (existeContaParaPagar.Situacao != SituacaoProcessamentoArquivo.AguardandoProcessamento)
                    return new JsonpResult(false, "Regitro se encontra numa situação diferente que não é permitido cancelar");

                existeContaParaPagar.Situacao = SituacaoProcessamentoArquivo.Cancelado;
                repositorioContaPagar.Atualizar(existeContaParaPagar);

                return new JsonpResult(true, "Registro cancelado com sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download da remessa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarPlanilhaParaProcessamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.WebService.Financeiro.Financeiro servicoFinanceiro = new Servicos.WebService.Financeiro.Financeiro(unitOfWork);
                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("File");
                TipoRegistro tipoRegistro = Request.GetEnumParam<TipoRegistro>("TipoRegistro");

                List<string> erros = new List<string>();

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                for (var i = 0; i < arquivos.Count(); i++)
                {
                    Servicos.DTO.CustomFile file = arquivos[i];
                    var extensaoArquivo = System.IO.Path.GetExtension(file.FileName).ToLower();

                    if (extensaoArquivo != ".csv")
                        return new JsonpResult(false, "Extensão do arquivo não permitida");

                    string nomearquivo = file.FileName.ToLower();
                    if (!nomearquivo.Contains(tipoRegistro.ObterSimilitudNome().ToLower()))
                        return new JsonpResult(false, "Nome de arquivo diferente aos permitidos.");

                    if (!servicoFinanceiro.SalvarArquivoParaProcessamento(file.InputStream, file.FileName, tipoRegistro))
                        return new JsonpResult(false, "Ocorreu um erro ao salvar o arquivo");

                    return new JsonpResult(true, true, "Arquivo adicionado para processamento corretamente");
                }
                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisaPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo Registro", "TipoRegistro", 25, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Nome Arquivo", "NomeArquivo", 30, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Situação", "Situacao", 30, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Retorno Processamento", "Retorno", 20, Models.Grid.Align.left);
            grid.AdicionarCabecalho("Numero Termo", "NumeroTermo", 20, Models.Grid.Align.left);

            DateTime? dataInicial = Request.GetNullableDateTimeParam("DataInicial");
            DateTime? dataFinal = Request.GetNullableDateTimeParam("DataFinal");
            SituacaoProcessamentoArquivo situacao = Request.GetEnumParam<SituacaoProcessamentoArquivo>("Situacao");
            int codigoTermo = Request.GetIntParam("TermoQuitacaoFinanceiro");

            var parametros = grid.ObterParametrosConsulta();

            if (parametros.PropriedadeOrdenar == "Data")
                parametros.PropriedadeOrdenar = "DataIntegracao";

            Repositorio.Embarcador.Financeiro.ContaPagar repositorioContaPagar = new Repositorio.Embarcador.Financeiro.ContaPagar(unitOfWork);
            List<Dominio.Entidades.Embarcador.Financeiro.ContaPagar> contasPagarProcessamentos = repositorioContaPagar.Consultar(dataInicial, dataFinal, situacao, codigoTermo, parametros);
            int counter = repositorioContaPagar.ContarConsultar(dataInicial, dataFinal, situacao, codigoTermo);

            dynamic lista = (from obj in contasPagarProcessamentos
                             select new
                             {
                                 obj.Codigo,
                                 Data = obj?.DataIntegracao?.ToString() ?? string.Empty,
                                 TipoRegistro = obj.TipoRegistro.ObterDescricao(),
                                 NomeArquivo = obj?.NomeOriginalArquivo ?? string.Empty,
                                 Situacao = obj.Situacao.ObterDescricao(),
                                 Retorno = obj?.MensagemProcessamento ?? string.Empty,
                                 NumeroTermo = obj?.TermoQuitacaoFinanceiro?.NumeroTermo ?? 0,
                                 DT_RowColor = obj.Situacao.ObterCorLinha()
                             }).ToList();

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(counter);
            return grid;
        }


        #endregion
    }
}
