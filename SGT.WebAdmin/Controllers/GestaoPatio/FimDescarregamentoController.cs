using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/FimDescarregamento", "GestaoPatio/FluxoPatio")]
    public class FimDescarregamentoController : BaseController
    {
		#region Construtores

		public FimDescarregamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AvancarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.GestaoPatio.FimDescarregamento servicoFimDescarregamento = new Servicos.Embarcador.GestaoPatio.FimDescarregamento(unitOfWork, Auditado, Cliente);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FimDescarregamentoAvancarEtapa fimDescarregamentoAvancarEtapa = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FimDescarregamentoAvancarEtapa()
                {
                    Codigo = Request.GetIntParam("Codigo"),
                    Pesagem = Request.GetDecimalParam("Pesagem")
                };

                servicoFimDescarregamento.Avancar(fimDescarregamentoAvancarEtapa);

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
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar o descarregamento.");
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
                int codigoFimDescarregamento = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.FimDescarregamento repositorioFimDescarregamento = new Repositorio.Embarcador.GestaoPatio.FimDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento fimDescarregamento = null;

                if (codigoFimDescarregamento > 0)
                    fimDescarregamento = repositorioFimDescarregamento.BuscarPorCodigo(codigoFimDescarregamento);
                else if (codigoFluxoGestaoPatio > 0)
                    fimDescarregamento = repositorioFimDescarregamento.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (fimDescarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (fimDescarregamento.Carga != null)
                    return new JsonpResult(ObterFimDescarregamentoPorCarga(unitOfWork, fimDescarregamento));

                return new JsonpResult(ObterFimDescarregamentoPorPreCarga(unitOfWork, fimDescarregamento));
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
                Repositorio.Embarcador.GestaoPatio.FimDescarregamento repositorioFimDescarregamento = new Repositorio.Embarcador.GestaoPatio.FimDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento fimDescarregamento = repositorioFimDescarregamento.BuscarPorCodigo(codigo);

                if (fimDescarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                if (fimDescarregamento.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível reabrir o fluxo nessa situação.");

                unitOfWork.Start();

                servicoFluxoGestaoPatio.ReabrirFluxo(fimDescarregamento.FluxoGestaoPatio);

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
                Repositorio.Embarcador.GestaoPatio.FimDescarregamento repositorioFimDescarregamento = new Repositorio.Embarcador.GestaoPatio.FimDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento fimDescarregamento = repositorioFimDescarregamento.BuscarPorCodigo(codigo);

                if (fimDescarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                servicoFluxoGestaoPatio.RejeitarEtapa(fimDescarregamento.FluxoGestaoPatio, EtapaFluxoGestaoPatio.FimDescarregamento);
                servicoFluxoGestaoPatio.Auditar(fimDescarregamento.FluxoGestaoPatio, "Rejeitou o fluxo.");

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
                Repositorio.Embarcador.GestaoPatio.FimDescarregamento repositorioFimDescarregamento = new Repositorio.Embarcador.GestaoPatio.FimDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento fimDescarregamento = repositorioFimDescarregamento.BuscarPorCodigo(codigo);

                if (fimDescarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(fimDescarregamento.FluxoGestaoPatio, EtapaFluxoGestaoPatio.FimDescarregamento, this.Usuario, permissoesPersonalizadasFluxoPatio);

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

        private dynamic ObterFimDescarregamentoPorCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento fimDescarregamento)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fimDescarregamento.FluxoGestaoPatio);
            bool permitirEditarEtapa = IsPermitirEditarEtapa(fimDescarregamento.FluxoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                fimDescarregamento.Codigo,
                fimDescarregamento.Situacao,
                Carga = fimDescarregamento.Carga.Codigo,
                PreCarga = fimDescarregamento.PreCarga?.Codigo ?? 0,
                NumeroCarga = servicoCarga.ObterNumeroCarga(fimDescarregamento.Carga, unitOfWork),
                NumeroPreCarga = fimDescarregamento.PreCarga?.NumeroPreCarga ?? "",
                CargaData = "",
                CargaHora = "",
                Transportador = fimDescarregamento.Carga.Empresa?.Descricao ?? string.Empty,
                Veiculo = fimDescarregamento.Carga.RetornarPlacas,
                Remetente = fimDescarregamento.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = fimDescarregamento.Carga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = fimDescarregamento.Carga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = fimDescarregamento.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                CodigoIntegracaoDestinatario = fimDescarregamento.Carga.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? string.Empty,
                Pesagem = (fimDescarregamento.Pesagem > 0m) ? fimDescarregamento.Pesagem.ToString("n2") : "",
                PermitirEditarEtapa = permitirEditarEtapa,
                PermiteInformarPesagem = sequenciaGestaoPatio?.FimDescarregamentoPermiteInformarPesagem ?? false
            };

            return docaCarregamentoRetornar;
        }

        private dynamic ObterFimDescarregamentoPorPreCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento fimDescarregamento)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fimDescarregamento.FluxoGestaoPatio);
            bool permitirEditarEtapa = IsPermitirEditarEtapa(fimDescarregamento.FluxoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                fimDescarregamento.Codigo,
                fimDescarregamento.Situacao,
                Carga = 0,
                PreCarga = fimDescarregamento.PreCarga.Codigo,
                NumeroCarga = "",
                NumeroPreCarga = fimDescarregamento.PreCarga.NumeroPreCarga ?? "",
                CargaData = "",
                CargaHora = "",
                Transportador = fimDescarregamento.PreCarga.Empresa?.Descricao ?? string.Empty,
                Veiculo = fimDescarregamento.PreCarga.RetornarPlacas,
                Remetente = fimDescarregamento.PreCarga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = fimDescarregamento.PreCarga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = fimDescarregamento.PreCarga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = fimDescarregamento.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.Nome ?? string.Empty,
                CodigoIntegracaoDestinatario = fimDescarregamento.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.CodigoIntegracao ?? string.Empty,
                Pesagem = (fimDescarregamento.Pesagem > 0m) ? fimDescarregamento.Pesagem.ToString("n2") : "",
                PermitirEditarEtapa = permitirEditarEtapa,
                PermiteInformarPesagem = sequenciaGestaoPatio?.FimDescarregamentoPermiteInformarPesagem ?? false
            };

            return docaCarregamentoRetornar;
        }

        private bool IsPermitirEditarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            return (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.FimDescarregamento);
        }

        #endregion
    }
}

