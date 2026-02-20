using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    public class ControleDasIntegracoesController : BaseController
    {
        #region Construtores

        public ControleDasIntegracoesController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Metodos Publicos
        public async Task<IActionResult> Pesquisar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Integracao.ControleDasIntegracoes repositorioControleDasIntegracoes = new Repositorio.Embarcador.Integracao.ControleDasIntegracoes(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltrosPesquisaControleDasIntegracoes filtroPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = ObterGridPesquisa();

                List<Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoes> listaContreIntegracoes = repositorioControleDasIntegracoes.ObterResumoIntegracoes(filtroPesquisa, grid.ObterParametrosConsulta());
                int qteTotal = repositorioControleDasIntegracoes.ContarConsulta(filtroPesquisa);

                grid.setarQuantidadeTotal(qteTotal);
                grid.AdicionaRows((from obj in listaContreIntegracoes
                                   select new
                                   {
                                       obj.Codigo,
                                       Integradora = obj?.Integradora?.Descricao ?? string.Empty,
                                       DataRequisicao = obj.DataRequisicao.ToString("dd/MM/yyyy hh:mm:ss"),
                                       NomeMetodo = obj.NomeMetodo,
                                       Origem = obj.Origem.ObterDescricao(),
                                       Sucesso = obj.DescricaoSituacao,
                                       Situacao = obj.SituacaoIntegracao.ObterDescricao(),
                                       Mensagem = obj?.MensagemRetorno ?? string.Empty

                                   }).ToList());
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
        }

        public async Task<IActionResult> DownloadArquivosIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                long codigoControleIntegracao = Request.GetLongParam("Codigo");
                Repositorio.Embarcador.Integracao.ControleDasIntegracoes repositorioControleDasIntregacoes = new Repositorio.Embarcador.Integracao.ControleDasIntegracoes(unitOfWork);
                Repositorio.Embarcador.Integracao.ControleDasIntegracoesAnexo repositorioControleDasIntregacoesAnexo = new Repositorio.Embarcador.Integracao.ControleDasIntegracoesAnexo(unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoes controleDasIntegracoes = repositorioControleDasIntregacoes.BuscarPorCodigo(codigoControleIntegracao);

                if (controleDasIntegracoes == null)
                    throw new ControllerException("Registro não existente");


                List<Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoesAnexo> arquivos = repositorioControleDasIntregacoesAnexo.BuscarPorControleIntegracao(codigoControleIntegracao);

                if (DateTime.Now.Subtract(controleDasIntegracoes.DataRequisicao).Days > 2 || arquivos.Count == 0)
                    throw new ControllerException("Os arquivos ficam armazenados por somente 2 dias.");

                List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao> arquivosBaixar = new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>();

                foreach (Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoesAnexo arquivo in arquivos)
                    arquivosBaixar.Add(new Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao()
                    {
                        NomeArquivo = arquivo.NomeArquivo
                    });

                byte[] arquivoZip = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(arquivosBaixar, ObterCaminhoArquivos(unitOfWork));

                return Arquivo(arquivoZip, "application/zip", $"Arquivos do metodo {controleDasIntegracoes.NomeMetodo}{controleDasIntegracoes.DataRequisicao.ToString("dd-MM-yyyy")}.zip");
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
        }
        #endregion

        #region Metodo Privados
        private Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltrosPesquisaControleDasIntegracoes ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltrosPesquisaControleDasIntegracoes()
            {
                CodigoIntegradora = Request.GetIntParam("Integradora"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                CodigoMetodo = Request.GetIntParam("Metodo"),
                Sitaucao = Request.GetBoolParam("Situacao")
            };
        }
        private Models.Grid.Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Integradora", "Integradora", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data", "DataRequisicao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Nome Metodo", "NomeMetodo", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Origem", "Origem", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Sucesso", "Sucesso", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Mensagem", "Mensagem", 30, Models.Grid.Align.left, true);
            return grid;
        }

        private string ObterCaminhoArquivos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoAquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoAquivo.BuscarPrimeiroRegistro();

            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "ControleDasIntegracoes");

            return caminho;
        }
        #endregion
    }
}
