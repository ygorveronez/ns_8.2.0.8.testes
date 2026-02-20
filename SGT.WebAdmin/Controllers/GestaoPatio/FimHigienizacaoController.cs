using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/FimHigienizacao", "GestaoPatio/FluxoPatio")]
    public class FimHigienizacaoController : BaseController
    {
		#region Construtores

		public FimHigienizacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AvancarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.GestaoPatio.FimHigienizacao servicoFimHigienizacao = new Servicos.Embarcador.GestaoPatio.FimHigienizacao(unitOfWork, Auditado, Cliente);

                servicoFimHigienizacao.Avancar(codigo);

                unitOfWork.CommitChanges();

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
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar a higienização.");
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
                int codigoFimHigienizacao = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.FimHigienizacao repositorioFimHigienizacao = new Repositorio.Embarcador.GestaoPatio.FimHigienizacao(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao fimHigienizacao = null;

                if (codigoFimHigienizacao > 0)
                    fimHigienizacao = repositorioFimHigienizacao.BuscarPorCodigo(codigoFimHigienizacao);
                else if (codigoFluxoGestaoPatio > 0)
                    fimHigienizacao = repositorioFimHigienizacao.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (fimHigienizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (fimHigienizacao.Carga != null)
                    return new JsonpResult(ObterFimHigienizacaoPorCarga(unitOfWork, fimHigienizacao));

                return new JsonpResult(ObterFimHigienizacaoPorPreCarga(unitOfWork, fimHigienizacao));
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

        public async Task<IActionResult> ReabrirFluxo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.FimHigienizacao repositorioFimHigienizacao = new Repositorio.Embarcador.GestaoPatio.FimHigienizacao(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao fimHigienizacao = repositorioFimHigienizacao.BuscarPorCodigo(codigo);

                if (fimHigienizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                if (fimHigienizacao.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível reabrir o fluxo nessa situação.");

                unitOfWork.Start();

                servicoFluxoGestaoPatio.ReabrirFluxo(fimHigienizacao.FluxoGestaoPatio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reabrir o fluxo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.FimHigienizacao repositorioFimHigienizacao = new Repositorio.Embarcador.GestaoPatio.FimHigienizacao(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao fimHigienizacao = repositorioFimHigienizacao.BuscarPorCodigo(codigo);

                if (fimHigienizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                servicoFluxoGestaoPatio.RejeitarEtapa(fimHigienizacao.FluxoGestaoPatio, EtapaFluxoGestaoPatio.FimHigienizacao);
                servicoFluxoGestaoPatio.Auditar(fimHigienizacao.FluxoGestaoPatio, "Rejeitou o fluxo.");

                unitOfWork.CommitChanges();

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
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar a etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VoltarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.FimHigienizacao repositorioFimHigienizacao = new Repositorio.Embarcador.GestaoPatio.FimHigienizacao(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao fimHigienizacao = repositorioFimHigienizacao.BuscarPorCodigo(codigo);

                if (fimHigienizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(fimHigienizacao.FluxoGestaoPatio, EtapaFluxoGestaoPatio.FimHigienizacao, this.Usuario, permissoesPersonalizadasFluxoPatio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao voltar a etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterFimHigienizacaoPorCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao fimHigienizacao)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
            bool permitirEditarEtapa = IsPermitirEditarEtapa(fimHigienizacao.FluxoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                fimHigienizacao.Codigo,
                fimHigienizacao.Situacao,
                Carga = fimHigienizacao.Carga.Codigo,
                PreCarga = fimHigienizacao.PreCarga?.Codigo ?? 0,
                NumeroCarga = servicoCarga.ObterNumeroCarga(fimHigienizacao.Carga, unitOfWork),
                NumeroPreCarga = fimHigienizacao.PreCarga?.NumeroPreCarga ?? "",
                CargaData = fimHigienizacao.Carga.DataCarregamentoCarga?.ToString($"dd/MM/yyyy")?? "",
                CargaHora = fimHigienizacao.Carga.DataCarregamentoCarga?.ToString($"HH:mm") ?? "",
                Transportador = fimHigienizacao.Carga.Empresa?.Descricao ?? string.Empty,
                Veiculo = fimHigienizacao.Carga.RetornarPlacas,
                Remetente = fimHigienizacao.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = fimHigienizacao.Carga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = fimHigienizacao.Carga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = fimHigienizacao.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                CodigoIntegracaoDestinatario = fimHigienizacao.Carga.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? string.Empty,
                PermitirEditarEtapa = permitirEditarEtapa
            };

            return docaCarregamentoRetornar;
        }

        private dynamic ObterFimHigienizacaoPorPreCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao fimHigienizacao)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(fimHigienizacao.PreCarga.Codigo);
            DateTime? dataCarregamento = cargaJanelaCarregamento?.InicioCarregamento;
            bool permitirEditarEtapa = IsPermitirEditarEtapa(fimHigienizacao.FluxoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                fimHigienizacao.Codigo,
                fimHigienizacao.Situacao,
                Carga = 0,
                PreCarga = fimHigienizacao.PreCarga.Codigo,
                NumeroCarga = "",
                NumeroPreCarga = fimHigienizacao.PreCarga.NumeroPreCarga ?? "",
                CargaData = dataCarregamento?.ToString($"dd/MM/yyyy") ?? "",
                CargaHora = dataCarregamento?.ToString($"HH:mm") ?? "",
                Transportador = fimHigienizacao.PreCarga.Empresa?.Descricao ?? string.Empty,
                Veiculo = fimHigienizacao.PreCarga.RetornarPlacas,
                Remetente = fimHigienizacao.PreCarga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = fimHigienizacao.PreCarga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = fimHigienizacao.PreCarga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = fimHigienizacao.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.Nome ?? string.Empty,
                CodigoIntegracaoDestinatario = fimHigienizacao.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.CodigoIntegracao ?? string.Empty,
                PermitirEditarEtapa = permitirEditarEtapa
            };

            return docaCarregamentoRetornar;
        }

        private bool IsPermitirEditarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            return (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.FimHigienizacao);
        }

        #endregion
    }
}
