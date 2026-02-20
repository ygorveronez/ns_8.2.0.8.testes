using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao" }, "Pessoas/Pessoa")]
    public class PessoaIntegracaoController : BaseController
    {
		#region Construtores

		public PessoaIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeDeTrabalho);
                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unidadeDeTrabalho);

                int.TryParse(Request.Params("Codigo"), out int codigo);


                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> integracoesArquivos = repPessoa.BuscarArquivosPorIntegacao(codigo, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repPessoa.ContarBuscarArquivosPorIntergacao(codigo));

                var retorno = (from obj in integracoesArquivos
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Cliente repPessoa = new Repositorio.Cliente(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = repPessoa.BuscarIntergacaoPorCodigo(codigo);

                if (arquivoIntegracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                if (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Pessoa.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Integrar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Pessoas.PessoaIntegracao repositorio = new Repositorio.Embarcador.Pessoas.PessoaIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao integracao = repositorio.BuscarPorCodigo(codigo);

                if (integracao == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas += 1;

                if (integracao.TipoIntegracao == null)
                {
                    integracao.ProblemaIntegracao = Localization.Resources.Gerais.Geral.IntegracaoNaoDisponivel;
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    if (integracao.TipoIntegracao.Tipo == TipoIntegracao.SIC)
                    {
                        Repositorio.Embarcador.Configuracoes.IntegracaoSIC repositorioIntegracaoSIC = new Repositorio.Embarcador.Configuracoes.IntegracaoSIC(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSIC configuracaoIntegracaoSIC = repositorioIntegracaoSIC.Buscar();

                        if (configuracaoIntegracaoSIC?.PossuiIntegracaoSIC ?? false)
                        {
                            if (string.IsNullOrEmpty(integracao.Protocolo))
                                new Servicos.Embarcador.Integracao.SIC.IntegracaoSIC(unitOfWork).SalvarListaClienteTerceiro(integracao, configuracaoIntegracaoSIC);
                            else
                                new Servicos.Embarcador.Integracao.SIC.IntegracaoSIC(unitOfWork).ConsultaInformacaoProtocoloInclusaoCadastro(integracao, configuracaoIntegracaoSIC);
                        }
                    }
                    else if (integracao.TipoIntegracao.Tipo == TipoIntegracao.KMM)
                    {
                        Servicos.Embarcador.Integracao.KMM.IntegracaoKMM servicoIntegracaoKMM = new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unitOfWork, TipoServicoMultisoftware);
                        servicoIntegracaoKMM.IntegrarPessoaMotorista(integracao);
                    }
                    else if (integracao.TipoIntegracao.Tipo == TipoIntegracao.Globus)
                    {
                        var globusService = new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(unitOfWork);
                        globusService.IntegrarPessoa(integracao);
                    }
                    else
                    {
                        integracao.ProblemaIntegracao = Localization.Resources.Gerais.Geral.IntegracaoNaoDisponivel;
                        integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    }
                }

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                //unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                //unitOfWork.Rollback();
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPessoaIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Integracao, "TipoIntegracao", 13, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Tentativas, "NumeroTentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.DataDoEnvio, "DataIntegracao", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "SituacaoIntegracao", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Mensagem, "Retorno", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Protocolo, "Protocolo", 13, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.StatusIntegracao, "StatusIntegracao", 13, Models.Grid.Align.left, true);




                long codigo = Request.GetLongParam("Codigo");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                Repositorio.Embarcador.Pessoas.PessoaIntegracao repositorioPessoaIntegracao = new Repositorio.Embarcador.Pessoas.PessoaIntegracao(unitOfWork);
                string propriedadeOrdenar = "TipoIntegracao.Tipo";  // ObterPropriedadeOrdenarPesquisaVeiculoIntegracoes(grid.header[grid.indiceColunaOrdena].data);
                List<Dominio.Entidades.Embarcador.Pessoas.PessoaIntegracao> listaIntegracoes = repositorioPessoaIntegracao.Consultar(codigo, situacao, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalIntegracoes = repositorioPessoaIntegracao.ContarConsulta(codigo, situacao);

                var listaIntegracoesRetornar = (
                    from integracao in listaIntegracoes
                    select new
                    {
                        integracao.Codigo,
                        Situacao = integracao.SituacaoIntegracao,
                        SituacaoIntegracao = integracao.DescricaoSituacaoIntegracao,
                        TipoIntegracao = integracao.TipoIntegracao.DescricaoTipo,
                        Retorno = integracao.ProblemaIntegracao,
                        integracao.NumeroTentativas,
                        DataIntegracao = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                        DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                        DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte(),
                        Protocolo = integracao.Protocolo,
                        StatusIntegracao = integracao.StatusIntegracao.ObterDescricao(),
                    }
                ).ToList();

                grid.AdicionaRows(listaIntegracoesRetornar);
                grid.setarQuantidadeTotal(totalIntegracoes);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTotaisIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Pessoas.PessoaIntegracao repositorioPessoaIntegracao = new Repositorio.Embarcador.Pessoas.PessoaIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repositorioPessoaIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repositorioPessoaIntegracao.ContarConsulta(codigo, SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repositorioPessoaIntegracao.ContarConsulta(codigo, SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repositorioPessoaIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgRetorno);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoObterOsTotaisDasIntegracoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ProblemaIntegracao()
        {
            return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            /*
            pendente 
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string motivo = Request.GetStringParam("Motivo");

                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, true, Localization.Resources.Veiculos.Veiculo.MotivoDeveSerInformado);

                Repositorio.Embarcador.Veiculos.VeiculoIntegracao repositorio = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao = repositorio.BuscarPorCodigo(codigo);

                if (integracao == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas += 1;
                integracao.ProblemaIntegracao = motivo.Trim();
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                repositorio.Atualizar(integracao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
            */
        }

        #endregion

        #region Métodos Privados

        private string ObterPropriedadeOrdenarPesquisaPessoaIntegracoes(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "TipoIntegracao")
                return "TipoIntegracao.Tipo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}


