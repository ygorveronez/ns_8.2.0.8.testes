using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/InicioHigienizacao", "GestaoPatio/FluxoPatio")]
    public class InicioHigienizacaoController : BaseController
    {
		#region Construtores

		public InicioHigienizacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AvancarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.GestaoPatio.InicioHigienizacao servicoInicioHigienizacao = new Servicos.Embarcador.GestaoPatio.InicioHigienizacao(unitOfWork, Auditado, Cliente);

                servicoInicioHigienizacao.Avancar(codigo);

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
                return new JsonpResult(false, "Ocorreu uma falha ao iniciar a higienização.");
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
                int codigoInicioHigienizacao = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.InicioHigienizacao repositorioInicioHigienizacao = new Repositorio.Embarcador.GestaoPatio.InicioHigienizacao(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao inicioHigienizacao = null;

                if (codigoInicioHigienizacao > 0)
                    inicioHigienizacao = repositorioInicioHigienizacao.BuscarPorCodigo(codigoInicioHigienizacao);
                else if (codigoFluxoGestaoPatio > 0)
                    inicioHigienizacao = repositorioInicioHigienizacao.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (inicioHigienizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (inicioHigienizacao.Carga != null)
                    return new JsonpResult(ObterInicioHigienizacaoPorCarga(unitOfWork, inicioHigienizacao));

                return new JsonpResult(ObterInicioHigienizacaoPorPreCarga(unitOfWork, inicioHigienizacao));
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
                Repositorio.Embarcador.GestaoPatio.InicioHigienizacao repositorioInicioHigienizacao = new Repositorio.Embarcador.GestaoPatio.InicioHigienizacao(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao inicioHigienizacao = repositorioInicioHigienizacao.BuscarPorCodigo(codigo);

                if (inicioHigienizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                if (inicioHigienizacao.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível reabrir o fluxo nessa situação.");

                unitOfWork.Start();

                servicoFluxoGestaoPatio.ReabrirFluxo(inicioHigienizacao.FluxoGestaoPatio);

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
                Repositorio.Embarcador.GestaoPatio.InicioHigienizacao repositorioInicioHigienizacao = new Repositorio.Embarcador.GestaoPatio.InicioHigienizacao(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao inicioHigienizacao = repositorioInicioHigienizacao.BuscarPorCodigo(codigo);

                if (inicioHigienizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                servicoFluxoGestaoPatio.RejeitarEtapa(inicioHigienizacao.FluxoGestaoPatio, EtapaFluxoGestaoPatio.InicioHigienizacao);
                servicoFluxoGestaoPatio.Auditar(inicioHigienizacao.FluxoGestaoPatio, "Rejeitou o fluxo.");

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
                Repositorio.Embarcador.GestaoPatio.InicioHigienizacao repositorioInicioHigienizacao = new Repositorio.Embarcador.GestaoPatio.InicioHigienizacao(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao inicioHigienizacao = repositorioInicioHigienizacao.BuscarPorCodigo(codigo);

                if (inicioHigienizacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(inicioHigienizacao.FluxoGestaoPatio, EtapaFluxoGestaoPatio.InicioHigienizacao, this.Usuario, permissoesPersonalizadasFluxoPatio);

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

        private dynamic ObterInicioHigienizacaoPorCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao inicioHigienizacao)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
            bool permitirEditarEtapa = IsPermitirEditarEtapa(inicioHigienizacao.FluxoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                inicioHigienizacao.Codigo,
                inicioHigienizacao.Situacao,
                Carga = inicioHigienizacao.Carga.Codigo,
                PreCarga = inicioHigienizacao.PreCarga?.Codigo ?? 0,
                NumeroCarga = servicoCarga.ObterNumeroCarga(inicioHigienizacao.Carga, unitOfWork),
                NumeroPreCarga = inicioHigienizacao.PreCarga?.NumeroPreCarga ?? "",
                CargaData = inicioHigienizacao.Carga.DataCarregamentoCarga?.ToString($"dd/MM/yyyy")?? "",
                CargaHora = inicioHigienizacao.Carga.DataCarregamentoCarga?.ToString($"HH:mm") ?? "",
                Transportador = inicioHigienizacao.Carga.Empresa?.Descricao ?? string.Empty,
                Veiculo = inicioHigienizacao.Carga.RetornarPlacas,
                Remetente = inicioHigienizacao.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = inicioHigienizacao.Carga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = inicioHigienizacao.Carga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = inicioHigienizacao.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                CodigoIntegracaoDestinatario = inicioHigienizacao.Carga.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? string.Empty,
                PermitirEditarEtapa = permitirEditarEtapa
            };

            return docaCarregamentoRetornar;
        }

        private dynamic ObterInicioHigienizacaoPorPreCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao inicioHigienizacao)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(inicioHigienizacao.PreCarga.Codigo);
            DateTime? dataCarregamento = cargaJanelaCarregamento?.InicioCarregamento;
            bool permitirEditarEtapa = IsPermitirEditarEtapa(inicioHigienizacao.FluxoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                inicioHigienizacao.Codigo,
                inicioHigienizacao.Situacao,
                Carga = 0,
                PreCarga = inicioHigienizacao.PreCarga.Codigo,
                NumeroCarga = "",
                NumeroPreCarga = inicioHigienizacao.PreCarga.NumeroPreCarga ?? "",
                CargaData = dataCarregamento?.ToString($"dd/MM/yyyy") ?? "",
                CargaHora = dataCarregamento?.ToString($"HH:mm") ?? "",
                Transportador = inicioHigienizacao.PreCarga.Empresa?.Descricao ?? string.Empty,
                Veiculo = inicioHigienizacao.PreCarga.RetornarPlacas,
                Remetente = inicioHigienizacao.PreCarga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = inicioHigienizacao.PreCarga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = inicioHigienizacao.PreCarga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = inicioHigienizacao.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.Nome ?? string.Empty,
                CodigoIntegracaoDestinatario = inicioHigienizacao.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.CodigoIntegracao ?? string.Empty,
                PermitirEditarEtapa = permitirEditarEtapa
            };

            return docaCarregamentoRetornar;
        }

        private bool IsPermitirEditarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            return (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.InicioHigienizacao);
        }

        #endregion
    }
}
