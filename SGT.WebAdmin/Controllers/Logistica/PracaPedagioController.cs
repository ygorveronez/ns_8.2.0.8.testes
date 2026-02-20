using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/PracaPedagio")]
    public class PracaPedagioController : BaseController
    {
		#region Construtores

		public PracaPedagioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var pracaPedagio = new Dominio.Entidades.Embarcador.Logistica.PracaPedagio();

                try
                {
                    PreencherPracaPedagio(pracaPedagio, unitOfWork);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                var repositorio = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork);

                repositorio.Inserir(pracaPedagio, Auditado);

                AdicionarOuAtualizarPracaPedagioTarifa(pracaPedagio, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork);
                var pracaPedagio = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (pracaPedagio == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                try
                {
                    PreencherPracaPedagio(pracaPedagio, unitOfWork);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                repositorio.Atualizar(pracaPedagio, Auditado);

                AdicionarOuAtualizarPracaPedagioTarifa(pracaPedagio, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork);
                var pracaPedagio = repositorio.BuscarPorCodigo(codigo);

                if (pracaPedagio == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Logistica.PracaPedagioTarifa repPracaPedagioTarifa = new Repositorio.Embarcador.Logistica.PracaPedagioTarifa(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifa> tarifas = repPracaPedagioTarifa.BuscarPorPracaPedagio(codigo);

                return new JsonpResult(new
                {
                    pracaPedagio.Codigo,
                    pracaPedagio.Descricao,
                    pracaPedagio.Rodovia,
                    pracaPedagio.Latitude,
                    pracaPedagio.Longitude,
                    KM = pracaPedagio.KM.ToString("n2"),
                    pracaPedagio.Concessionaria,
                    pracaPedagio.CodigoIntegracao,
                    Status = pracaPedagio.Ativo,
                    pracaPedagio.Observacao,
                    PracaPedagioTarifa = (
                        from tarifa in tarifas
                        select new
                        {
                            tarifa.Codigo,
                            Data = tarifa.Data.ToString("dd/MM/yyyy HH:mm"),
                            Tarifa = tarifa.Tarifa.ToString("n4"),
                            CodigoModeloVeicularCarga = tarifa.ModeloVeicularCarga.Codigo,
                            DescricaoModeloVeicularCarga = tarifa.ModeloVeicularCarga.Descricao
                        }).ToList()
                });
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork);
                var pracaPedagio = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (pracaPedagio == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                repositorio.Deletar(pracaPedagio, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRemover);
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
                var grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();
                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ListaPracasPedagio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.PracaPedagio repositorio = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> itens = repositorio.BuscarTodosAtivas();

                var result = (
                   from reg in itens
                   select new
                   {
                       reg.Codigo,
                       reg.CodigoIntegracao,
                       reg.Descricao,
                       reg.Latitude,
                       reg.Longitude,
                       reg.Concessionaria,
                       reg.KM,
                       Tarifas = (from v in reg.PracaPedagioTarifa
                                  where v.Data == (from vi in reg.PracaPedagioTarifa
                                                   where vi.ModeloVeicularCarga.Codigo == v.ModeloVeicularCarga.Codigo
                                                   orderby v.Data descending
                                                   select v.Data).FirstOrDefault()
                                  orderby v.ModeloVeicularCarga.Descricao
                                  select new
                                  {
                                      ModeloVeicularCarga = v.ModeloVeicularCarga.Descricao,
                                      Tarifa = v.Tarifa.ToString("C")
                                  }).ToList()
                   }
                ).ToList();

                return new JsonpResult(result);
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
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaIntegracao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoSituacaoIntegracao", false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.PracaPedagio.TipoIntegracao, "TipoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.PracaPedagio.DataEnvio, "DataIntegracao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.PracaPedagio.Tentativas, "NumeroTentativas", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.PracaPedagio.Situacao, "SituacaoIntegracao", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.PracaPedagio.Retorno, "ProblemaIntegracao", 30, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPracaPedagioTarifaIntegracao filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPracaPedagioTarifaIntegracao()
                {
                    DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                    DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                    SituacaoIntegracao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("SituacaoIntegracao")
                };

                Repositorio.Embarcador.Logistica.PracaPedagioTarifaIntegracao repPracaPedagioTarifaIntegracao = new Repositorio.Embarcador.Logistica.PracaPedagioTarifaIntegracao(unitOfWork);
                int totalRegistros = repPracaPedagioTarifaIntegracao.ContarConsulta(filtroPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao> listaPracaPedagioTarifaIntegracao = (totalRegistros > 0) ? repPracaPedagioTarifaIntegracao.Consultar(filtroPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao>();

                var listaRetornar = (
                    from row in listaPracaPedagioTarifaIntegracao
                    select new
                    {
                        row.Codigo,
                        TipoIntegracao = row.TipoIntegracao.Descricao,
                        DataIntegracao = row.DataIntegracao.ToString("dd/MM/yyyy HH:mm"),
                        NumeroTentativas = row.NumeroTentativas,
                        SituacaoIntegracao = row.DescricaoSituacaoIntegracao,
                        CodigoSituacaoIntegracao = (int)row.SituacaoIntegracao,
                        ProblemaIntegracao = row.ProblemaIntegracao,
                        DT_RowColor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoHelper.ObterCorLinha(row.SituacaoIntegracao),
                        DT_FontColor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoHelper.ObterCorFonte(row.SituacaoIntegracao),
                    }
                ).ToList();

                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

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
        public async Task<IActionResult> PesquisaIntegracaoHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Data, "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Tipo, "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.PracaPedagio.Retorno, "Mensagem", 40, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.PracaPedagioTarifaIntegracao repPracaPedagioTarifaIntegracao = new Repositorio.Embarcador.Logistica.PracaPedagioTarifaIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao pracaPedagioTarifaIntegracao = repPracaPedagioTarifaIntegracao.BuscarPorCodigo(codigo, auditavel: true);
                if (pracaPedagioTarifaIntegracao == null) return new JsonpResult(false, Localization.Resources.Logistica.PracaPedagio.IntegracaoNaoEncontrada);

                var arquivosRetornar = (
                    from arquivoTransacao in pracaPedagioTarifaIntegracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                    select new
                    {
                        arquivoTransacao.Codigo,
                        Data = arquivoTransacao.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                        arquivoTransacao.DescricaoTipo,
                        arquivoTransacao.Mensagem
                    }
                ).ToList();

                grid.AdicionaRows(arquivosRetornar);
                grid.setarQuantidadeTotal(pracaPedagioTarifaIntegracao.ArquivosTransacao.Count());

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.PracaPedagio.OcorreuUmaFalhaAoConsultarHistoricoIntegracoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadArquivosIntegracaoHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.PracaPedagioTarifaIntegracao repPracaPedagioTarifaIntegracao = new Repositorio.Embarcador.Logistica.PracaPedagioTarifaIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao pracaPedagioTarifaIntegracao = repPracaPedagioTarifaIntegracao.BuscarPorCodigoArquivo(codigo);
                if (pracaPedagioTarifaIntegracao == null) return new JsonpResult(false, Localization.Resources.Logistica.PracaPedagio.HistoricoIntegracaoNaoEncontrado);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = pracaPedagioTarifaIntegracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();
                if ((arquivoIntegracao == null) || ((arquivoIntegracao.ArquivoRequisicao == null) && (arquivoIntegracao.ArquivoResposta == null)))
                    return new JsonpResult(true, false, Localization.Resources.Logistica.PracaPedagio.NaoHaRegistroDeArquivosSalvosParaEsseHistorico);

                byte[] arquivoCompactado = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });
                return Arquivo(arquivoCompactado, "application/zip", $"Arquivos do {pracaPedagioTarifaIntegracao.Descricao}.zip");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, Localization.Resources.Logistica.PracaPedagio.OcorreuUmaFalhaAoRealizarDownloadDosArquivosDaIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = BuscarTipoIntegracaoSemParar(unitOfWork);
                if (tipoIntegracao != null)
                {
                    unitOfWork.Start();
                    int codigo = Request.GetIntParam("Codigo");
                    Repositorio.Embarcador.Logistica.PracaPedagioTarifaIntegracao repPracaPedagioTarifaIntegracao = new Repositorio.Embarcador.Logistica.PracaPedagioTarifaIntegracao(unitOfWork);
                    Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao pracaPedagioTarifaIntegracao = repPracaPedagioTarifaIntegracao.BuscarPorCodigo(codigo, auditavel: true);
                    if (pracaPedagioTarifaIntegracao == null)
                        return new JsonpResult(false, Localization.Resources.Logistica.PracaPedagio.IntegracaoNaoEncontrada);
                    pracaPedagioTarifaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, Localization.Resources.Logistica.PracaPedagio.IntegracaoSemPararNaoConfigurada);
                }
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
                return new JsonpResult(false, Localization.Resources.Logistica.PracaPedagio.OcorreuUmaFalhaAoReenviarIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Integrar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = BuscarTipoIntegracaoSemParar(unitOfWork);
                if (tipoIntegracao != null)
                {
                    unitOfWork.Start();
                    Repositorio.Embarcador.Logistica.PracaPedagioTarifaIntegracao repPracaPedagioTarifaIntegracao = new Repositorio.Embarcador.Logistica.PracaPedagioTarifaIntegracao(unitOfWork);
                    Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao pracaPedagioTarifaIntegracao = new Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifaIntegracao();
                    pracaPedagioTarifaIntegracao.DataIntegracao = DateTime.Now;
                    pracaPedagioTarifaIntegracao.TipoIntegracao = tipoIntegracao;
                    pracaPedagioTarifaIntegracao.ProblemaIntegracao = string.Empty;
                    pracaPedagioTarifaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    repPracaPedagioTarifaIntegracao.Inserir(pracaPedagioTarifaIntegracao);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, Localization.Resources.Logistica.PracaPedagio.IntegracaoSemPararNaoConfigurada);
                }
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
                return new JsonpResult(false, Localization.Resources.Logistica.PracaPedagio.OcorreuUmaFalhaAoReenviarIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherPracaPedagio(Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaPedagio, Repositorio.UnitOfWork unitOfWork)
        {
            bool ativo = Request.GetBoolParam("Status");
            string descricao = Request.Params("Descricao");
            string rodovia = Request.Params("Rodovia");
            string Concessionaria = Request.Params("Concessionaria");
            string observacao = Request.Params("Observacao");
            string codigoIntegracao = Request.Params("CodigoIntegracao");
            decimal km = 0;
            decimal.TryParse(Request.Params("KM"), out km);

            string latitude = Request.Params("Latitude");
            string longitude = Request.Params("Longitude");

            pracaPedagio.Ativo = ativo;
            pracaPedagio.Descricao = descricao;
            pracaPedagio.Observacao = observacao;
            pracaPedagio.CodigoIntegracao = codigoIntegracao;
            pracaPedagio.Rodovia = rodovia;
            pracaPedagio.Concessionaria = Concessionaria;
            pracaPedagio.Latitude = latitude;
            pracaPedagio.Longitude = longitude;
            pracaPedagio.KM = km;

            Repositorio.Embarcador.Logistica.PracaPedagio repPracaPedagio = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaPedagioExiste = repPracaPedagio.BuscarPorCodigoIntegracao(codigoIntegracao);

            if (pracaPedagioExiste != null && pracaPedagioExiste.Codigo != pracaPedagio.Codigo)
            {
                throw new Exception(string.Format(Localization.Resources.Logistica.PracaPedagio.JaExisteUmaPracaPedagioCadastradaComCodigo, codigoIntegracao));
            }

        }

        public async Task<IActionResult> AtualizarLatLngPracas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.PracaPedagio repPracaPedagio = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork);

                dynamic pracasPontos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PracasPontos"));
                if (pracasPontos != null)
                {
                    foreach (var pracaPonto in pracasPontos)
                    {
                        Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaPedagio = repPracaPedagio.BuscarPorCodigo((int)pracaPonto.Codigo);

                        if (pracaPedagio != null)
                        {
                            pracaPedagio.Latitude = (string)pracaPonto.Latitude;
                            pracaPedagio.Longitude = (string)pracaPonto.Longitude;
                            repPracaPedagio.Atualizar(pracaPedagio);
                        }
                    }
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Logistica.PracaPedagio.OcorreuUmaFalhaAoAtualizarLatitudeLongitudeClientes);
            }
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "CodigoIntegracao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.PracaPedagio.Rodovia, "Rodovia", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.PracaPedagio.Km, "KM", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "DescricaoAtivo", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Latitude", false);
                grid.AdicionarCabecalho("Longitude", false);
                grid.AdicionarCabecalho("Sentido", false);
                grid.AdicionarCabecalho("Ordem", false);

                string descricao = Request.Params("Descricao");
                string concessionaria = Request.Params("Concessionaria");
                string rodovia = Request.Params("Rodovia");
                string codigoIntegracao = Request.Params("CodigoIntegracao");

                var status = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Status");
                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);
                var repositorio = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork);
                var listaPracaPedagio = repositorio.Consultar(descricao, concessionaria, rodovia, codigoIntegracao, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                var totalRegistros = repositorio.ContarConsulta(descricao, concessionaria, rodovia, codigoIntegracao, status);

                var listaPracaPedagioRetornar = (
                    from pracaPedagio in listaPracaPedagio
                    select new
                    {
                        pracaPedagio.CodigoIntegracao,
                        pracaPedagio.Codigo,
                        pracaPedagio.Descricao,
                        KM = pracaPedagio.KM.ToString("n2"),
                        pracaPedagio.Rodovia,
                        pracaPedagio.DescricaoAtivo,
                        pracaPedagio.Latitude,
                        pracaPedagio.Longitude,
                        Sentido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspensoHelper.ObterDescricao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspenso.Nenhum),
                        Ordem = 0
                    }
                ).ToList();

                grid.AdicionaRows(listaPracaPedagioRetornar);
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

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        private void AdicionarOuAtualizarPracaPedagioTarifa(Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaPedagio, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic pracaPedagioTarifa = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PracaPedagioTarifa"));
            ExcluirPracaPedagioTarifaRemovidos(pracaPedagio, pracaPedagioTarifa, unitOfWork);
            InserirPracaPedagioTarifaAdicionados(pracaPedagio, pracaPedagioTarifa, unitOfWork);
        }

        private void ExcluirPracaPedagioTarifaRemovidos(Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaPedagio, dynamic pracaPedagioTarifa, Repositorio.UnitOfWork unitOfWork)
        {
            if (pracaPedagio.PracaPedagioTarifa?.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();
                foreach (var tarifa in pracaPedagioTarifa)
                {
                    int? codigo = ((string)tarifa.Codigo).ToNullableInt();
                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                Repositorio.Embarcador.Logistica.PracaPedagioTarifa repPracaPedagioTarifa = new Repositorio.Embarcador.Logistica.PracaPedagioTarifa(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifa> listaPracaPedagioTarifaRemover = (from tarifa in pracaPedagio.PracaPedagioTarifa where !listaCodigosAtualizados.Contains(tarifa.Codigo) select tarifa).ToList();
                foreach (var tarifa in listaPracaPedagioTarifaRemover)
                {
                    repPracaPedagioTarifa.Deletar(tarifa);
                }
            }
        }

        private void InserirPracaPedagioTarifaAdicionados(Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaPedagio, dynamic tarifas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PracaPedagioTarifa repPracaPedagioTarifa = new Repositorio.Embarcador.Logistica.PracaPedagioTarifa(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            foreach (var tarifa in tarifas)
            {
                int? codigo = ((string)tarifa.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifa pracaPedagioTarifa;
                if (codigo.HasValue)
                    pracaPedagioTarifa = repPracaPedagioTarifa.BuscarPorCodigo(codigo.Value) ?? throw new ControllerException(Localization.Resources.Logistica.PracaPedagio.TarifaDePracaPedagioNaoEncontrada);
                else
                    pracaPedagioTarifa = new Dominio.Entidades.Embarcador.Logistica.PracaPedagioTarifa();

                pracaPedagioTarifa.PracaPedagio = pracaPedagio;
                pracaPedagioTarifa.Tarifa = ((string)tarifa.Tarifa).ToDecimal(default(decimal));
                pracaPedagioTarifa.Data = ((string)tarifa.Data).ToDateTime(default(DateTime));
                pracaPedagioTarifa.ModeloVeicularCarga = repModeloVeicularCarga.BuscarPorCodigo(((string)tarifa.CodigoModeloVeicularCarga).ToInt(), false) ?? throw new ControllerException("Modelo veicular de carga não encontrado");

                if (codigo.HasValue)
                    repPracaPedagioTarifa.Atualizar(pracaPedagioTarifa);
                else
                    repPracaPedagioTarifa.Inserir(pracaPedagioTarifa);
            }
        }

        private Dominio.Entidades.Embarcador.Cargas.TipoIntegracao BuscarTipoIntegracaoSemParar(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar);
            return tipoIntegracao;
        }

        #endregion

    }
}
