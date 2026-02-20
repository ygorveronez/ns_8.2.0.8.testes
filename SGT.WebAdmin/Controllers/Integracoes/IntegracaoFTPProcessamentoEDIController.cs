using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/IntegracaoFTPProcessamentoEDI")]
    public class IntegracaoFTPProcessamentoEDIController : BaseController
    {
		#region Construtores

		public IntegracaoFTPProcessamentoEDIController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> EnviarArquivo()
        {
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Servicos.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP servicoIntegracaoEDIFTP = new Servicos.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP(unitOfWork);


                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                int CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoaEnvioArquivo");
                int codigoLayout = Request.GetIntParam("LayoutEDI");

                Dominio.Entidades.LayoutEDI LayoutEDI = repLayoutEDI.BuscarPorCodigo(codigoLayout);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa = repGrupoPessoas.BuscarPorCodigo(CodigoGrupoPessoa);

                if (LayoutEDI == null || grupoPessoa == null)
                    return new JsonpResult(false, true, "Grupo Pessoa e Layout EDI é obrigatório");

                List<string> importados = new List<string>();
                List<string> erros = new List<string>();
                List<string> extensoesValidas = new List<string>() { ".txt" };
                int adicionados = 0;

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                for (var i = 0; i < arquivos.Count(); i++)
                {

                    Servicos.DTO.CustomFile file = arquivos[i];
                    var extensaoArquivo = System.IO.Path.GetExtension(file.FileName).ToLower();

                    if (!extensaoArquivo.Contains(extensaoArquivo))
                    {
                        erros.Add("Extensão " + extensaoArquivo + " não permitida.");
                        continue;
                    }
                    try
                    {
                        string nomeArquivo = System.IO.Path.GetFileName(file.FileName);

                        if (importados.Contains(nomeArquivo))
                            continue;

                        Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP integracaoProcessamentoEDIFTP = new Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP();
                        Repositorio.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP repIntegracao = new Repositorio.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP(unitOfWork);
                        dynamic retornoArquivo = servicoIntegracaoEDIFTP.SalvarArquivoImportacaoTemporario(file.InputStream, nomeArquivo, unitOfWork);

                        integracaoProcessamentoEDIFTP.Cliente = null;
                        integracaoProcessamentoEDIFTP.DataIntegracao = DateTime.Now;
                        integracaoProcessamentoEDIFTP.GrupoPessoas = grupoPessoa;
                        integracaoProcessamentoEDIFTP.LayoutEDI = LayoutEDI;
                        integracaoProcessamentoEDIFTP.GuidArquivo = retornoArquivo.Token;
                        integracaoProcessamentoEDIFTP.NomeArquivo = retornoArquivo.NomeOriginal;
                        integracaoProcessamentoEDIFTP.MensagemRetorno = "";
                        integracaoProcessamentoEDIFTP.NumeroTentativas = 0;
                        integracaoProcessamentoEDIFTP.SituacaoIntegracaoEDIFTP = SituacaoIntegracaoProcessamentoEDIFTP.AgIntegracao;

                        repIntegracao.Inserir(integracaoProcessamentoEDIFTP);

                        adicionados++;
                        importados.Add(nomeArquivo);
                    }
                    catch (Exception e)
                    {
                        erros.Add("Erro ao processar arquivo " + file.FileName + ".");
                        Servicos.Log.TratarErro(e);
                    }
                }

                return new JsonpResult(new
                {
                    Adicionados = adicionados,
                    Erros = erros
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao envio o arquivo.");
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa(isExportarPesquisa: true);

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(status: false, mensagem: "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(status: false, mensagem: "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa(isExportarPesquisa: false));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(status: false, mensagem: "Ocorreu uma falha ao consultar.");
            }
        }


        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP repIntegracaoProcessamentoEDIFTP = new Repositorio.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP(unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP integracao = repIntegracaoProcessamentoEDIFTP.BuscarPorCodigo(codigo);

                if (integracao.SituacaoIntegracaoEDIFTP != SituacaoIntegracaoProcessamentoEDIFTP.Falha)
                    return new JsonpResult(false, true, "Não é possível reenviar a integração na situação atual.");

                integracao.SituacaoIntegracaoEDIFTP = SituacaoIntegracaoProcessamentoEDIFTP.AgIntegracao;
                integracao.NumeroTentativas++;

                repIntegracaoProcessamentoEDIFTP.Atualizar(integracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivoEDI()
        {
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
                Servicos.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP servicoIntegracaoEDIFTP = new Servicos.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP(unitOfWork);

                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                if (codigo > 0)
                {

                    Repositorio.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP repIntegracaoProcessamentoEDIFTP = new Repositorio.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP(unitOfWork);
                    Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP integracao = repIntegracaoProcessamentoEDIFTP.BuscarPorCodigo(codigo);

                    if (integracao != null)
                    {

                        string caminho = servicoIntegracaoEDIFTP.ObterCaminhoCompletoArquivo(integracao.GuidArquivo, unitOfWork );
                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        {
                            byte[] data = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);
                            if (data != null)
                            {
                                return Arquivo(data, "text/txt", integracao.NomeArquivo);
                            }
                        }
                    }
                }
                return new JsonpResult(false, false, "EDI não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do EDI.");
            }
        }



        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa(bool isExportarPesquisa)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaProcessamentoEDIFTP filtrosPesquisa = ObterFiltrosPesquisa();

                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoa", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nome do Arquivo", "NomeArquivo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 4, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "MensagemRetorno", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data", "Data", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracaoProcessamentoEDIFTP", 8, Models.Grid.Align.left, true);

                string propriedadeOrdenar = grid.header[grid.indiceColunaOrdena].data;
                Repositorio.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP repIntegracaoprocessamentoEDI = new Repositorio.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP(unitOfWork);
                int totalRegistros = repIntegracaoprocessamentoEDI.ContarConsulta(filtrosPesquisa);

                if ((isExportarPesquisa) && (totalRegistros > 10000))
                    throw new ControllerException("O número de registros ultrapassa o limite para exportação de 10000 registros.");

                List<Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP> listaIntegracoes = repIntegracaoprocessamentoEDI.Consultar(filtrosPesquisa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                var listaIntegracoesRetornar = (
                    from integracao in listaIntegracoes
                    select new
                    {
                        integracao.Codigo,
                        Data = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm"),
                        MensagemRetorno = integracao.MensagemRetorno.Replace(";", " | "),
                        integracao.NomeArquivo,
                        integracao.NumeroTentativas,
                        SituacaoIntegracaoProcessamentoEDIFTP = integracao.SituacaoIntegracaoEDIFTP.ObterDescricao(),
                        GrupoPessoa = integracao.GrupoPessoas?.Descricao,
                    }
                ).ToList();

                grid.AdicionaRows(listaIntegracoesRetornar);
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

        private Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaProcessamentoEDIFTP ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaProcessamentoEDIFTP()
            {
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                NomeArquivo = Request.Params("NomeArquivo"),
                Situacao = Request.GetEnumParam<SituacaoIntegracaoProcessamentoEDIFTP>("Situacao")
            };
        }

        #endregion

    }
}
