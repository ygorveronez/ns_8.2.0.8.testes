using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize(new string[] { "BuscarPorCodigo" }, "GestaoPatio/FluxoPatio")]
    public class ChegadaSaidaLojaController : BaseController
    {
		#region Construtores

		public ChegadaSaidaLojaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                EtapaFluxoGestaoPatio? etapaFluxoGestaoPatio = Request.GetNullableEnumParam<EtapaFluxoGestaoPatio>("Etapa");
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCodigo(codigoFluxoGestaoPatio, auditavel: false);

                if (fluxoGestaoPatio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                string loja = fluxoGestaoPatio.CargaBase.DadosSumarizados?.Destinatarios ?? "";

                var retorno = new
                {
                    fluxoGestaoPatio.Codigo,
                    Carga = fluxoGestaoPatio.Carga.Codigo,
                    CargaChegadaLoja = fluxoGestaoPatio.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                    Loja = loja,
                    VeiculoAindaNaoChegou = !fluxoGestaoPatio.DataChegadaLoja.HasValue,
                    VeiculoAindaNaoSaiu = !fluxoGestaoPatio.DataSaidaLoja.HasValue,
                    DataChegada = fluxoGestaoPatio.DataChegadaLoja?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataSaida = fluxoGestaoPatio.DataSaidaLoja?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    fluxoGestaoPatio.DevolucaoChegadaLoja,
                    fluxoGestaoPatio.NotaFiscalChegadaLoja,
                    PermitirEditarEtapa = IsPermitirEditarEtapa(fluxoGestaoPatio, etapaFluxoGestaoPatio)
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> SalvarInformacaoEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCodigo(codigoFluxoGestaoPatio, auditavel: false);

                if (fluxoGestaoPatio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o fluxo.");

                Dominio.Enumeradores.OpcaoSimNao devolucaoChegadaLoja = Request.GetEnumParam("DevolucaoChegadaLoja", Dominio.Enumeradores.OpcaoSimNao.Nao);
                string notaFiscalChegadaLoja = Request.GetStringParam("NotaFiscalChegadaLoja");

                if (devolucaoChegadaLoja == Dominio.Enumeradores.OpcaoSimNao.Sim)
                {
                    if (string.IsNullOrWhiteSpace(notaFiscalChegadaLoja))
                        return new JsonpResult(false, true, "Quando for Devolução deve-se informar uma Nota fiscal.");

                    if (notaFiscalChegadaLoja.Length < 6)
                        return new JsonpResult(false, true, "Quando for Devolução a Nota Fiscal precisa ter ao menos 6 caracteres (numéricos).");
                }

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                fluxoGestaoPatio.DevolucaoChegadaLoja = devolucaoChegadaLoja;
                fluxoGestaoPatio.NotaFiscalChegadaLoja = notaFiscalChegadaLoja;

                repositorioFluxoGestaoPatio.Atualizar(fluxoGestaoPatio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar as informações da etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarChegadaLoja()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCodigo(codigoFluxoGestaoPatio, auditavel: false);

                if (fluxoGestaoPatio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.ChegadaLoja);

                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarSaidaLoja()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCodigo(codigoFluxoGestaoPatio, auditavel: false);

                if (fluxoGestaoPatio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.SaidaLoja);

                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VoltarEtapaChegadaLoja()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCodigo(codigoFluxoGestaoPatio, auditavel: false);

                if (fluxoGestaoPatio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o fluxo.");

                unitOfWork.Start();

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.ChegadaLoja, this.Usuario, permissoesPersonalizadasFluxoPatio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao retornar a etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VoltarEtapaSaidaLoja()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCodigo(codigoFluxoGestaoPatio, auditavel: false);

                if (fluxoGestaoPatio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o fluxo.");

                unitOfWork.Start();

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.SaidaLoja, this.Usuario, permissoesPersonalizadasFluxoPatio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao retornar a etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private bool IsPermitirEditarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxo, EtapaFluxoGestaoPatio? etapaFluxoGestaoPatio)
        {
            if ((fluxo == null) || (fluxo.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            if (!etapaFluxoGestaoPatio.HasValue)
                return false;

            if (fluxo.EtapaFluxoGestaoPatioAtual == etapaFluxoGestaoPatio.Value)
                return true;

            return false;
        }

        #endregion
    }
}
