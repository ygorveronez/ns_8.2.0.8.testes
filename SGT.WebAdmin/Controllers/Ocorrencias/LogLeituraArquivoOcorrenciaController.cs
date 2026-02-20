using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize("Ocorrencias/LogLeituraArquivoOcorrencia")]
    public class LogLeituraArquivoOcorrenciaController : BaseController
    {
        #region Construtores

        public LogLeituraArquivoOcorrenciaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivoEDI()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                if (codigo > 0)
                {
                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracaoEDI, "OcorenRecebidos");
                    Repositorio.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia repLogLeituraArquivoOcorrencia = new Repositorio.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia(unitOfWork);
                    Dominio.Entidades.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia integracao = repLogLeituraArquivoOcorrencia.BuscarPorCodigo(codigo);

                    if (integracao != null)
                    {

                        caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, integracao.GuidArquivo);
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
            finally
            {
                unitOfWork.Dispose();
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

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarArquivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                List<string> erros = new List<string>();
                List<string> extensoesValidas = new List<string>() { ".txt" };
                int adicionados = 0;

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPorTipo(Dominio.Enumeradores.TipoLayoutEDI.OCOREN).FirstOrDefault();
                for (int i = 0; i < arquivos.Count(); i++)
                {
                    Servicos.DTO.CustomFile file = arquivos[i];
                    string extensaoArquivo = System.IO.Path.GetExtension(file.FileName).ToLower();

                    if (!extensaoArquivo.Contains(extensaoArquivo))
                    {
                        erros.Add("Extensão " + extensaoArquivo + " não permitida.");
                        continue;
                    }
                    else
                    {
                        Servicos.Embarcador.CargaOcorrencia.Ocorrencia serOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia();
                        string retorno = serOcorrencia.ProcessarOcorren(layoutEDI, file.InputStream, System.IO.Path.GetFileName(file.FileName), TipoServicoMultisoftware, ConfiguracaoEmbarcador, Usuario, TipoEnvioArquivo.Manual, Cliente, unitOfWork, Auditado);
                        if (!string.IsNullOrEmpty(retorno))
                            return new JsonpResult(false, retorno);

                        adicionados++;
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
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa(bool isExportarPesquisa)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime DataFinal = Request.GetDateTimeParam("DataFinal");
                DateTime DataInicial = Request.GetDateTimeParam("DataInicial");
                string nomeArquivo = Request.GetStringParam("NomeArquivo");
                string ocorrencia = Request.GetStringParam("Ocorrencia");
                int empresa = Request.GetIntParam("Transportador");
                TipoEnvioArquivo tipoEnvioArquivo = Request.GetEnumParam<TipoEnvioArquivo>("TipoEnvioArquivo");

                empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Usuario.Empresa.Codigo : empresa;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);

                if (!ConfiguracaoEmbarcador.PedidoOcorrenciaColetaEntregaIntegracaoNova)
                    grid.AdicionarCabecalho("Ocorrência", "Ocorrencia", 8, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nome do Arquivo", "NomeArquivo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "MotivoInconsistencia", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data", "Data", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo Envio", "TipoEnvioArquivo", 8, Models.Grid.Align.left, true);

                string propriedadeOrdenar = grid.header[grid.indiceColunaOrdena].data;
                Repositorio.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia repositorioLogLeituraArquivoOcorrencia = new Repositorio.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia(unitOfWork);
                int totalRegistros = repositorioLogLeituraArquivoOcorrencia.ContarConsulta(nomeArquivo, ocorrencia, empresa, DataInicial, DataFinal, tipoEnvioArquivo);

                if (isExportarPesquisa && totalRegistros > 10000)
                    throw new ControllerException("O número de registros ultrapassa o limite para exportação de 10000 registros.");

                List<Dominio.Entidades.Embarcador.Ocorrencias.LogLeituraArquivoOcorrencia> listaIntegracoes = repositorioLogLeituraArquivoOcorrencia.Consultar(nomeArquivo, ocorrencia, empresa, DataInicial, DataFinal, tipoEnvioArquivo, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                var listaIntegracoesRetornar = (
                    from integracao in listaIntegracoes
                    select new
                    {
                        integracao.Codigo,
                        Data = integracao.DataRecebimento.ToString("dd/MM/yyyy HH:mm"),
                        Ocorrencia = integracao.OcorrenciasGeradas,
                        Usuario = integracao.Usuario?.Nome ?? "",
                        MotivoInconsistencia = integracao.MotivoInconsistencia,
                        TipoEnvioArquivo = integracao.TipoEnvioArquivo.ObterDescricao(),
                        integracao.NomeArquivo,
                        Transportador = integracao.Empresa?.Descricao ?? ""
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

        #endregion
    }
}
