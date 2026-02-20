using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.FaixaTemperatura
{
    [CustomAuthorize("Cargas/FaixaTemperatura", "Pedidos/TipoDeCarga", "Cargas/TipoCarga")]
    public class FaixaTemperaturaController : BaseController
    {
		#region Construtores

		public FaixaTemperaturaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = new Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura();

                PreencherFaixaTemperatura(faixaTemperatura, unitOfWork);

                unitOfWork.Start();

                new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork).Inserir(faixaTemperatura, Auditado);

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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.FaixaTemperatura repositorio = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (faixaTemperatura == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherFaixaTemperatura(faixaTemperatura, unitOfWork);

                unitOfWork.Start();

                repositorio.Atualizar(faixaTemperatura, Auditado);

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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarFaixaPorTipoOperacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int tipoOperacao = Request.GetIntParam("codigoOperacao");


                Repositorio.Embarcador.Cargas.FaixaTemperatura repositorio = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> faixasTemperatura = repositorio.BuscarPorTipoOperacao(tipoOperacao);

                if (faixasTemperatura == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");


                return new JsonpResult(new
                {
                    FaixasTemperatura = from faixaTemperatura in faixasTemperatura
                                        select new
                                        {
                                            text = faixaTemperatura.Descricao + $" {faixaTemperatura.FaixaInicial} à {faixaTemperatura.FaixaFinal}",
                                            value = faixaTemperatura.Codigo
                                        }
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
        public async Task<IActionResult> BuscarTodas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.FaixaTemperatura repositorio = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> faixasTemperatura = repositorio.BuscarTodos();

                if (faixasTemperatura == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");


                return new JsonpResult(new
                {
                    FaixasTemperatura = from faixaTemperatura in faixasTemperatura
                                        select new
                                        {
                                            text = faixaTemperatura.Descricao + $" {faixaTemperatura.FaixaInicial} à {faixaTemperatura.FaixaFinal}",
                                            value = faixaTemperatura.Codigo
                                        }
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.FaixaTemperatura repositorio = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = repositorio.BuscarPorCodigo(codigo);

                if (faixaTemperatura == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    FaixaTemperatura = new
                    {
                        faixaTemperatura.Codigo,
                        faixaTemperatura.Descricao,
                        FaixaFinal = faixaTemperatura.FaixaFinal.ToString("n2"),
                        FaixaInicial = faixaTemperatura.FaixaInicial.ToString("n2"),
                        Carimbo = faixaTemperatura.Carimbo,
                        CarimboDescricao = faixaTemperatura.CarimboDescricao,
                        CodigoIntegracao = faixaTemperatura.CodigoIntegracao,
                        TipoOperacao = new { Codigo = faixaTemperatura?.TipoOperacao?.Codigo ?? 0, Descricao = faixaTemperatura?.TipoOperacao?.Descricao ?? "" },
                        Remetente = new { Codigo = faixaTemperatura.Remetente?.CPF_CNPJ ?? 0, Descricao = faixaTemperatura.Remetente?.Descricao ?? "" },
                        ProcedimentoEmbarque = new { Codigo = faixaTemperatura?.ProcedimentoEmbarque?.Codigo ?? 0, Descricao = faixaTemperatura?.ProcedimentoEmbarque?.Descricao ?? "" }
                    },
                    FaixaTemperaturaMensagemValidacao = new
                    {
                        faixaTemperatura.MensagemLicencaReprovadaEmbarcador,
                        faixaTemperatura.MensagemLicencaReprovadaTransportador,
                        faixaTemperatura.MensagemLicencaVencidaEmbarcador,
                        faixaTemperatura.MensagemLicencaVencidaTransportador
                    }
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.FaixaTemperatura repositorio = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (faixaTemperatura == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(faixaTemperatura, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
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
                var grid = ObterGridPesquisa(true);

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
            try
            {
                return new JsonpResult(ObterGridPesquisa(false));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherFaixaTemperatura(Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura, Repositorio.UnitOfWork unitOfWork)
        {
            faixaTemperatura.Descricao = Request.GetNullableStringParam("Descricao") ?? throw new ControllerException("Descrição da faixa de temperatura é obrigatória.");
            faixaTemperatura.FaixaFinal = Request.GetDecimalParam("FaixaFinal");
            faixaTemperatura.FaixaInicial = Request.GetDecimalParam("FaixaInicial");
            faixaTemperatura.Carimbo = Request.GetIntParam("Carimbo");
            faixaTemperatura.CarimboDescricao = Request.GetStringParam("CarimboDescricao");
            faixaTemperatura.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            faixaTemperatura.MensagemLicencaVencidaEmbarcador = Request.GetStringParam("MensagemLicencaVencidaEmbarcador");
            faixaTemperatura.MensagemLicencaVencidaTransportador = Request.GetStringParam("MensagemLicencaVencidaTransportador");
            faixaTemperatura.MensagemLicencaReprovadaEmbarcador = Request.GetStringParam("MensagemLicencaReprovadaEmbarcador");
            faixaTemperatura.MensagemLicencaReprovadaTransportador = Request.GetStringParam("MensagemLicencaReprovadaTransportador");
            faixaTemperatura.DataUltimaModificacao = DateTime.Now;
            faixaTemperatura.Remetente = null;

            int paramTipoOperacao = Request.GetIntParam("TipoOperacao");
            if (paramTipoOperacao > 0)
            {
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(paramTipoOperacao);
                faixaTemperatura.TipoOperacao = tipoOperacao;
            }

            int codigoProcedimentoEmbarque = Request.GetIntParam("ProcedimentoEmbarque");
            if (codigoProcedimentoEmbarque > 0)
            {
                Repositorio.Embarcador.Logistica.ProcedimentoEmbarque repProcedimentoEmbarque = new Repositorio.Embarcador.Logistica.ProcedimentoEmbarque(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque procedimentoEmbarque = repProcedimentoEmbarque.BuscarPorCodigo(codigoProcedimentoEmbarque);
                faixaTemperatura.ProcedimentoEmbarque = procedimentoEmbarque;
            }

            if (faixaTemperatura.FaixaInicial >= faixaTemperatura.FaixaFinal)
                throw new ControllerException("A faixa inicial deve ser menor que a inicial.");

            double codigoRemetente = Request.GetDoubleParam("Remetente");
            if (codigoRemetente > 0)
            {
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                faixaTemperatura.Remetente = repositorioCliente.BuscarPorCPFCNPJ(codigoRemetente);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaFaixaTemperatura ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoCarga = Request.GetIntParam("Carga");
            int procedimentoEmbarque = Request.GetIntParam("ProcedimentoEmbarque");
            if (codigoCarga > 0)
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                if (carga?.ProcedimentoEmbarque != null)
                    procedimentoEmbarque = carga.ProcedimentoEmbarque.Codigo;
            }
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaFaixaTemperatura()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                ProcedimentoEmbarque = procedimentoEmbarque
            };
        }

        private Models.Grid.Grid ObterGridPesquisa(bool showLastUpdateDate)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.GetStringParam("Descricao");
                SituacaoAtivoPesquisa situacaoAtivo = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.FaixaTemperatura.CodigoDeIntegracao, "CodigoIntegracao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.FaixaTemperatura.ProcedimentoDeEmbarque, "ProcedimentoEmbarque", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.FaixaTemperatura.FaixaInical, "FaixaInicial", 20, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.FaixaTemperatura.FaixaFinal, "FaixaFinal", 20, Models.Grid.Align.center, false);
                if (showLastUpdateDate)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.FaixaTemperatura.DataUltimaModificacao, "LastUpdateDate");
                }

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaFaixaTemperatura filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Cargas.FaixaTemperatura repositorio = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> listaFaixaTemperatura = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>();

                var listaFaixaTemperaturaRetornar = (
                    from faixa in listaFaixaTemperatura
                    select new
                    {
                        faixa.Codigo,
                        faixa.Descricao,
                        faixa.CodigoIntegracao,
                        ProcedimentoEmbarque = faixa.ProcedimentoEmbarque?.Descricao ?? "",
                        FaixaFinal = faixa.FaixaFinal.ToString("n2"),
                        FaixaInicial = faixa.FaixaInicial.ToString("n2"),
                        LastUpdateDate = faixa.DataUltimaModificacao.HasValue ? faixa.DataUltimaModificacao.Value.ToString("dd/MM/yyyy") : ""
                    }
                ).ToList();

                grid.AdicionaRows(listaFaixaTemperaturaRetornar);
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
