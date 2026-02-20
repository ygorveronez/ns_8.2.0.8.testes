using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/FluxoPatio")]
    public class FaturamentoController : BaseController
    {
		#region Construtores

		public FaturamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCodigo(codigoFluxoGestaoPatio, false);

                if (fluxoGestaoPatio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (fluxoGestaoPatio.CargaBase.IsCarga())
                    return new JsonpResult(ObterInformacoesPorCarga(unitOfWork, fluxoGestaoPatio));

                return new JsonpResult(ObterInformacoesPorPreCarga(unitOfWork, fluxoGestaoPatio));
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

        public async Task<IActionResult> VoltarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FluxoGestaoPatio_PermiteVoltarEtapaFaturamento))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoPatio = repositorioFluxoGestaoPatio.BuscarPorCodigo(codigo, false);

                if (fluxoPatio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(fluxoPatio, EtapaFluxoGestaoPatio.Faturamento, this.Usuario, permissoesPersonalizadasFluxoPatio);

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

        public async Task<IActionResult> ImprimirCapaViagem()
        {
            try
            {
                return Arquivo(ObterPdfCapaViagem(Request.GetIntParam("Codigo")), "application/pdf", "CapaViagem.pdf");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao imprimir a capa da viagem.");
            }
        }

        #endregion

        #region Métodos Privados

        private bool IsPermitirEditarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            return (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.Faturamento);
        }

        private dynamic ObterInformacoesPorCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(fluxoGestaoPatio.Carga.Codigo);
            bool permitirEditarEtapa = IsPermitirEditarEtapa(fluxoGestaoPatio);

            var informacoes = new
            {
                fluxoGestaoPatio.Codigo,
                Carga = fluxoGestaoPatio.Carga.Codigo,
                PreCarga = fluxoGestaoPatio.PreCarga?.Codigo ?? 0,
                NumeroCarga = servicoCarga.ObterNumeroCarga(fluxoGestaoPatio.Carga, unitOfWork),
                NumeroPreCarga = fluxoGestaoPatio.PreCarga?.NumeroPreCarga ?? "",
                CargaData = fluxoGestaoPatio.Carga.DataCarregamentoCarga?.ToString($"dd/MM/yyyy") ?? "",
                CargaHora = fluxoGestaoPatio.Carga.DataCarregamentoCarga?.ToString($"HH:mm") ?? "",
                Transportador = fluxoGestaoPatio.Carga.Empresa?.Descricao ?? string.Empty,
                Veiculo = fluxoGestaoPatio.Carga.RetornarPlacas,
                Remetente = fluxoGestaoPatio.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = fluxoGestaoPatio.Carga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = fluxoGestaoPatio.Carga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = fluxoGestaoPatio.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                CodigoIntegracaoDestinatario = fluxoGestaoPatio.Carga.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? string.Empty,
                PermitirEditarEtapa = permitirEditarEtapa,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? ""
            };

            return informacoes;
        }

        private dynamic ObterInformacoesPorPreCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(fluxoGestaoPatio.PreCarga.Codigo);
            DateTime? dataCarregamento = cargaJanelaCarregamento?.InicioCarregamento;
            bool permitirEditarEtapa = IsPermitirEditarEtapa(fluxoGestaoPatio);

            var informacoes = new
            {
                fluxoGestaoPatio.Codigo,
                Carga = 0,
                PreCarga = fluxoGestaoPatio.PreCarga.Codigo,
                NumeroCarga = "",
                NumeroPreCarga = fluxoGestaoPatio.PreCarga.NumeroPreCarga ?? "",
                CargaData = dataCarregamento?.ToString($"dd/MM/yyyy") ?? "",
                CargaHora = dataCarregamento?.ToString($"HH:mm") ?? "",
                Transportador = fluxoGestaoPatio.PreCarga.Empresa?.Descricao ?? string.Empty,
                Veiculo = fluxoGestaoPatio.PreCarga.RetornarPlacas,
                Remetente = fluxoGestaoPatio.PreCarga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = fluxoGestaoPatio.PreCarga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = fluxoGestaoPatio.PreCarga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = fluxoGestaoPatio.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.Nome ?? string.Empty,
                CodigoIntegracaoDestinatario = fluxoGestaoPatio.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.CodigoIntegracao ?? string.Empty,
                PermitirEditarEtapa = permitirEditarEtapa,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? ""
            };

            return informacoes;
        }

        private byte[] ObterPdfCapaViagem(int codigoFluxoGestaoPatio)
        {
            return ReportRequest.WithType(ReportType.CapaViagem)
                .WithExecutionType(ExecutionType.Sync)
                .AddExtraData("codigoFluxoGestaoPatio",codigoFluxoGestaoPatio.ToString())
                .CallReport()
                .GetContentFile();
        }

        #endregion
    }
}
