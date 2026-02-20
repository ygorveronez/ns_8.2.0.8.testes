using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/DocumentosTransporte", "GestaoPatio/FluxoPatio")]
    public class DocumentosTransporteController : BaseController
    {
		#region Construtores

		public DocumentosTransporteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AvancarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.GestaoPatio.DocumentosTransporte servicoDocumentosTransporte = new Servicos.Embarcador.GestaoPatio.DocumentosTransporte(unitOfWork, Auditado, Cliente);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.DocumentosTransporteAvancar documentosTransporteAvancar = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.DocumentosTransporteAvancar()
                {
                    Codigo = Request.GetIntParam("Codigo"),
                    NumeroCTe = Request.GetStringParam("NumeroCTe"),
                    NumeroMDFe = Request.GetStringParam("NumeroMDFe"),
                    Brix = Request.GetDecimalParam("Brix"),
                    Ratio = Request.GetDecimalParam("Ratio"),
                    Oleo = Request.GetDecimalParam("Oleo"),
                };

                servicoDocumentosTransporte.Avancar(documentosTransporteAvancar);

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
                return new JsonpResult(false, "Ocorreu uma falha ao informar os documentos de transporte.");
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
                int codigoDocumentosTransporte = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.DocumentosTransporte repositorioDocumentosTransporte = new Repositorio.Embarcador.GestaoPatio.DocumentosTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte documentosTransporte = null;

                if (codigoDocumentosTransporte > 0)
                    documentosTransporte = repositorioDocumentosTransporte.BuscarPorCodigo(codigoDocumentosTransporte);
                else if (codigoFluxoGestaoPatio > 0)
                    documentosTransporte = repositorioDocumentosTransporte.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (documentosTransporte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (documentosTransporte.Carga != null)
                    return new JsonpResult(ObterDocumentosTransportePorCarga(unitOfWork, documentosTransporte));

                return new JsonpResult(ObterDocumentosTransportePorPreCarga(unitOfWork, documentosTransporte));
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

        public async Task<IActionResult> ReabrirFluxo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.DocumentosTransporte repositorioDocumentosTransporte = new Repositorio.Embarcador.GestaoPatio.DocumentosTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte documentosTransporte = repositorioDocumentosTransporte.BuscarPorCodigo(codigo);

                if (documentosTransporte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                if (documentosTransporte.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível reabrir o fluxo nessa situação.");

                unitOfWork.Start();

                servicoFluxoGestaoPatio.ReabrirFluxo(documentosTransporte.FluxoGestaoPatio);

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
                Repositorio.Embarcador.GestaoPatio.DocumentosTransporte repositorioDocumentosTransporte = new Repositorio.Embarcador.GestaoPatio.DocumentosTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte documentosTransporte = repositorioDocumentosTransporte.BuscarPorCodigo(codigo);

                if (documentosTransporte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                servicoFluxoGestaoPatio.RejeitarEtapa(documentosTransporte.FluxoGestaoPatio, EtapaFluxoGestaoPatio.DocumentosTransporte);
                servicoFluxoGestaoPatio.Auditar(documentosTransporte.FluxoGestaoPatio, "Rejeitou o fluxo.");

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
                Repositorio.Embarcador.GestaoPatio.DocumentosTransporte repositorioDocumentosTransporte = new Repositorio.Embarcador.GestaoPatio.DocumentosTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte documentosTransporte = repositorioDocumentosTransporte.BuscarPorCodigo(codigo);

                if (documentosTransporte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(documentosTransporte.FluxoGestaoPatio, EtapaFluxoGestaoPatio.DocumentosTransporte, this.Usuario, permissoesPersonalizadasFluxoPatio);

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

        private dynamic ObterDocumentosTransportePorCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte documentosTransporte)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
            bool permitirEditarEtapa = IsPermitirEditarEtapa(documentosTransporte.FluxoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                documentosTransporte.Codigo,
                documentosTransporte.NumeroCTe,
                documentosTransporte.NumeroMDFe,
                documentosTransporte.Brix,
                documentosTransporte.Ratio,
                documentosTransporte.Oleo,
                Carga = documentosTransporte.Carga.Codigo,
                PreCarga = documentosTransporte.PreCarga?.Codigo ?? 0,
                NumeroCarga = servicoCarga.ObterNumeroCarga(documentosTransporte.Carga, unitOfWork),
                NumeroPreCarga = documentosTransporte.PreCarga?.NumeroPreCarga ?? "",
                CargaData = "",
                CargaHora = "",
                Transportador = documentosTransporte.Carga.Empresa?.Descricao ?? string.Empty,
                Veiculo = documentosTransporte.Carga.RetornarPlacas,
                Remetente = documentosTransporte.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = documentosTransporte.Carga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = documentosTransporte.Carga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = documentosTransporte.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                CodigoIntegracaoDestinatario = documentosTransporte.Carga.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? string.Empty,
                PermitirEditarEtapa = permitirEditarEtapa
            };

            return docaCarregamentoRetornar;
        }

        private dynamic ObterDocumentosTransportePorPreCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.DocumentosTransporte documentosTransporte)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
            bool permitirEditarEtapa = IsPermitirEditarEtapa(documentosTransporte.FluxoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                documentosTransporte.Codigo,
                documentosTransporte.NumeroCTe,
                documentosTransporte.NumeroMDFe,
                documentosTransporte.Brix,
                documentosTransporte.Ratio,
                documentosTransporte.Oleo,
                Carga = 0,
                PreCarga = documentosTransporte.PreCarga.Codigo,
                NumeroCarga = "",
                NumeroPreCarga = documentosTransporte.PreCarga.NumeroPreCarga ?? "",
                CargaData = "",
                CargaHora = "",
                Transportador = documentosTransporte.PreCarga.Empresa?.Descricao ?? string.Empty,
                Veiculo = documentosTransporte.PreCarga.RetornarPlacas,
                Remetente = documentosTransporte.PreCarga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = documentosTransporte.PreCarga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = documentosTransporte.PreCarga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = documentosTransporte.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.Nome ?? string.Empty,
                CodigoIntegracaoDestinatario = documentosTransporte.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.CodigoIntegracao ?? string.Empty,
                PermitirEditarEtapa = permitirEditarEtapa
            };

            return docaCarregamentoRetornar;
        }

        private bool IsPermitirEditarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            return (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.DocumentosTransporte);
        }

        #endregion
    }
}
