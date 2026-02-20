using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/DocumentoFiscal", "GestaoPatio/FluxoPatio")]
    public class DocumentoFiscalController : BaseController
    {
		#region Construtores

		public DocumentoFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AvancarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.GestaoPatio.DocumentoFiscal servicoDocumentoFiscal = new Servicos.Embarcador.GestaoPatio.DocumentoFiscal(unitOfWork, Auditado, Cliente);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.DocumentoFiscalAvancar documentoFiscalAvancar = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.DocumentoFiscalAvancar()
                {
                    Codigo = Request.GetIntParam("Codigo"),
                    NumerosDocumentos = Request.GetListParam<string>("NumerosDocumentos")
                };             
            
                servicoDocumentoFiscal.Avancar(documentoFiscalAvancar);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao informar o documento fiscal.");
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
                int codigoDocumentoFiscal = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.DocumentoFiscal repositorioDocumentoFiscal = new Repositorio.Embarcador.GestaoPatio.DocumentoFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal documentoFiscal = null;

                if (codigoDocumentoFiscal > 0)
                    documentoFiscal = repositorioDocumentoFiscal.BuscarPorCodigo(codigoDocumentoFiscal);
                else if (codigoFluxoGestaoPatio > 0)
                    documentoFiscal = repositorioDocumentoFiscal.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (documentoFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = repConfiguracaoGestaoPatio.BuscarConfiguracao();

                if (documentoFiscal.Carga != null)
                    return new JsonpResult(ObterDocumentoFiscalPorCarga(unitOfWork, documentoFiscal, configuracaoGestaoPatio));

                return new JsonpResult(ObterDocumentoFiscalPorPreCarga(unitOfWork, documentoFiscal, configuracaoGestaoPatio));
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
                Repositorio.Embarcador.GestaoPatio.DocumentoFiscal repositorioDocumentoFiscal = new Repositorio.Embarcador.GestaoPatio.DocumentoFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal documentoFiscal = repositorioDocumentoFiscal.BuscarPorCodigo(codigo);

                if (documentoFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                if (documentoFiscal.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível reabrir o fluxo nessa situação.");

                unitOfWork.Start();

                servicoFluxoGestaoPatio.ReabrirFluxo(documentoFiscal.FluxoGestaoPatio);

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
                Repositorio.Embarcador.GestaoPatio.DocumentoFiscal repositorioDocumentoFiscal = new Repositorio.Embarcador.GestaoPatio.DocumentoFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal documentoFiscal = repositorioDocumentoFiscal.BuscarPorCodigo(codigo);

                if (documentoFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                servicoFluxoGestaoPatio.RejeitarEtapa(documentoFiscal.FluxoGestaoPatio, EtapaFluxoGestaoPatio.DocumentoFiscal);
                servicoFluxoGestaoPatio.Auditar(documentoFiscal.FluxoGestaoPatio, "Rejeitou o fluxo.");

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
                Repositorio.Embarcador.GestaoPatio.DocumentoFiscal repositorioDocumentoFiscal = new Repositorio.Embarcador.GestaoPatio.DocumentoFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal documentoFiscal = repositorioDocumentoFiscal.BuscarPorCodigo(codigo);

                if (documentoFiscal == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(documentoFiscal.FluxoGestaoPatio, EtapaFluxoGestaoPatio.DocumentoFiscal, this.Usuario, permissoesPersonalizadasFluxoPatio);

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

        private dynamic ObterDocumentoFiscalPorCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal documentoFiscal, Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
            bool permitirEditarEtapa = IsPermitirEditarEtapa(documentoFiscal.FluxoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                documentoFiscal.Codigo,
                NumerosDocumentos = (
                    from numeroDocumento in documentoFiscal.NumerosDocumentos
                    select new
                    {
                        Codigo = Guid.NewGuid().ToString(),
                        NumeroDocumento = numeroDocumento
                    }
                ).ToList(),
                Carga = documentoFiscal.Carga.Codigo,
                PreCarga = documentoFiscal.PreCarga?.Codigo ?? 0,
                NumeroCarga = servicoCarga.ObterNumeroCarga(documentoFiscal.Carga, unitOfWork),
                NumeroPreCarga = documentoFiscal.PreCarga?.NumeroPreCarga ?? "",
                CargaData = "",
                CargaHora = "",
                Transportador = documentoFiscal.Carga.Empresa?.Descricao ?? string.Empty,
                Veiculo = documentoFiscal.Carga.RetornarPlacas,
                Remetente = documentoFiscal.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = documentoFiscal.Carga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = documentoFiscal.Carga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = documentoFiscal.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                CodigoIntegracaoDestinatario = documentoFiscal.Carga.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? string.Empty,
                PermitirEditarEtapa = permitirEditarEtapa,
                DocumentoFiscalPermiteAvancarAutomaticamenteAposNotasFiscaisInseridas = configuracaoGestaoPatio?.DocumentoFiscalPermiteAvancarAutomaticamenteAposNotasFiscaisInseridas ?? false
            };

            return docaCarregamentoRetornar;
        }

        private dynamic ObterDocumentoFiscalPorPreCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.DocumentoFiscal documentoFiscal, Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
            bool permitirEditarEtapa = IsPermitirEditarEtapa(documentoFiscal.FluxoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                documentoFiscal.Codigo,
                NumerosDocumentos = (
                    from numeroDocumento in documentoFiscal.NumerosDocumentos
                    select new
                    {
                        Codigo = Guid.NewGuid().ToString(),
                        NumeroDocumento = numeroDocumento
                    }
                ).ToList(),
                Carga = 0,
                PreCarga = documentoFiscal.PreCarga.Codigo,
                NumeroCarga = "",
                NumeroPreCarga = documentoFiscal.PreCarga.NumeroPreCarga ?? "",
                CargaData = "",
                CargaHora = "",
                Transportador = documentoFiscal.PreCarga.Empresa?.Descricao ?? string.Empty,
                Veiculo = documentoFiscal.PreCarga.RetornarPlacas,
                Remetente = documentoFiscal.PreCarga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = documentoFiscal.PreCarga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = documentoFiscal.PreCarga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = documentoFiscal.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.Nome ?? string.Empty,
                CodigoIntegracaoDestinatario = documentoFiscal.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.CodigoIntegracao ?? string.Empty,
                PermitirEditarEtapa = permitirEditarEtapa,
                DocumentoFiscalPermiteAvancarAutomaticamenteAposNotasFiscaisInseridas = configuracaoGestaoPatio?.DocumentoFiscalPermiteAvancarAutomaticamenteAposNotasFiscaisInseridas ?? false
            };

            return docaCarregamentoRetornar;
        }

        private bool IsPermitirEditarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            return (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.DocumentoFiscal);
        }

        #endregion
    }
}
