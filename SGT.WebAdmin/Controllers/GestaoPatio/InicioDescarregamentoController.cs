using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/InicioDescarregamento", "GestaoPatio/FluxoPatio")]
    public class InicioDescarregamentoController : BaseController
    {
		#region Construtores

		public InicioDescarregamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AvancarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.GestaoPatio.InicioDescarregamento servicoInicioDescarregamento = new Servicos.Embarcador.GestaoPatio.InicioDescarregamento(unitOfWork, Auditado, Cliente);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.InicioDescarregamentoAvancarEtapa inicioDescarregamentoAvancarEtapa = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.InicioDescarregamentoAvancarEtapa()
                {
                    Codigo = Request.GetIntParam("Codigo"),
                    Pesagem = Request.GetDecimalParam("Pesagem")
                };

                servicoInicioDescarregamento.Avancar(inicioDescarregamentoAvancarEtapa);

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
                return new JsonpResult(false, "Ocorreu uma falha ao iniciar o descarregamento.");
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
                int codigoInicioDescarregamento = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.InicioDescarregamento repositorioInicioDescarregamento = new Repositorio.Embarcador.GestaoPatio.InicioDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento inicioDescarregamento = null;

                if (codigoInicioDescarregamento > 0)
                    inicioDescarregamento = repositorioInicioDescarregamento.BuscarPorCodigo(codigoInicioDescarregamento);
                else if (codigoFluxoGestaoPatio > 0)
                    inicioDescarregamento = repositorioInicioDescarregamento.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (inicioDescarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (inicioDescarregamento.Carga != null)
                    return new JsonpResult(ObterInicioDescarregamentoPorCarga(unitOfWork, inicioDescarregamento));

                return new JsonpResult(ObterInicioDescarregamentoPorPreCarga(unitOfWork, inicioDescarregamento));
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
                Repositorio.Embarcador.GestaoPatio.InicioDescarregamento repositorioInicioDescarregamento = new Repositorio.Embarcador.GestaoPatio.InicioDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento inicioDescarregamento = repositorioInicioDescarregamento.BuscarPorCodigo(codigo);

                if (inicioDescarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                if (inicioDescarregamento.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível reabrir o fluxo nessa situação.");

                unitOfWork.Start();

                servicoFluxoGestaoPatio.ReabrirFluxo(inicioDescarregamento.FluxoGestaoPatio);

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
                Repositorio.Embarcador.GestaoPatio.InicioDescarregamento repositorioInicioDescarregamento = new Repositorio.Embarcador.GestaoPatio.InicioDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento inicioDescarregamento = repositorioInicioDescarregamento.BuscarPorCodigo(codigo);

                if (inicioDescarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                servicoFluxoGestaoPatio.RejeitarEtapa(inicioDescarregamento.FluxoGestaoPatio, EtapaFluxoGestaoPatio.InicioDescarregamento);

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
                Repositorio.Embarcador.GestaoPatio.InicioDescarregamento repositorioInicioDescarregamento = new Repositorio.Embarcador.GestaoPatio.InicioDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento inicioDescarregamento = repositorioInicioDescarregamento.BuscarPorCodigo(codigo);

                if (inicioDescarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(inicioDescarregamento.FluxoGestaoPatio, EtapaFluxoGestaoPatio.InicioDescarregamento, this.Usuario, permissoesPersonalizadasFluxoPatio);

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

        private dynamic ObterInicioDescarregamentoPorCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento inicioDescarregamento)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(inicioDescarregamento.FluxoGestaoPatio);
            bool permitirEditarEtapa = IsPermitirEditarEtapa(inicioDescarregamento.FluxoGestaoPatio);

            var inicioDescarregamentoRetornar = new
            {
                inicioDescarregamento.Codigo,
                inicioDescarregamento.Situacao,
                Carga = inicioDescarregamento.Carga.Codigo,
                PreCarga = inicioDescarregamento.PreCarga?.Codigo ?? 0,
                NumeroCarga = servicoCarga.ObterNumeroCarga(inicioDescarregamento.Carga, unitOfWork),
                NumeroPreCarga = inicioDescarregamento.PreCarga?.NumeroPreCarga ?? "",
                CargaData = "",
                CargaHora = "",
                Transportador = inicioDescarregamento.Carga.Empresa?.Descricao ?? string.Empty,
                Veiculo = inicioDescarregamento.Carga.RetornarPlacas,
                Remetente = inicioDescarregamento.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = inicioDescarregamento.Carga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = inicioDescarregamento.Carga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = inicioDescarregamento.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                CodigoIntegracaoDestinatario = inicioDescarregamento.Carga.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? string.Empty,
                Pesagem = (inicioDescarregamento.Pesagem > 0m) ? inicioDescarregamento.Pesagem.ToString("n2") : "",
                PermitirEditarEtapa = permitirEditarEtapa,
                PermiteInformarPesagem = sequenciaGestaoPatio?.InicioDescarregamentoPermiteInformarPesagem ?? false
            };

            return inicioDescarregamentoRetornar;
        }

        private dynamic ObterInicioDescarregamentoPorPreCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento inicioDescarregamento)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(inicioDescarregamento.FluxoGestaoPatio);
            bool permitirEditarEtapa = IsPermitirEditarEtapa(inicioDescarregamento.FluxoGestaoPatio);

            var inicioDescarregamentoRetornar = new
            {
                inicioDescarregamento.Codigo,
                inicioDescarregamento.Situacao,
                Carga = 0,
                PreCarga = inicioDescarregamento.PreCarga.Codigo,
                NumeroCarga = "",
                NumeroPreCarga = inicioDescarregamento.PreCarga.NumeroPreCarga ?? "",
                CargaData = "",
                CargaHora = "",
                Transportador = inicioDescarregamento.PreCarga.Empresa?.Descricao ?? string.Empty,
                Veiculo = inicioDescarregamento.PreCarga.RetornarPlacas,
                Remetente = inicioDescarregamento.PreCarga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = inicioDescarregamento.PreCarga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = inicioDescarregamento.PreCarga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = inicioDescarregamento.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.Nome ?? string.Empty,
                CodigoIntegracaoDestinatario = inicioDescarregamento.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.CodigoIntegracao ?? string.Empty,
                Pesagem = (inicioDescarregamento.Pesagem > 0m) ? inicioDescarregamento.Pesagem.ToString("n2") : "",
                PermitirEditarEtapa = permitirEditarEtapa,
                PermiteInformarPesagem = sequenciaGestaoPatio?.InicioDescarregamentoPermiteInformarPesagem ?? false
            };

            return inicioDescarregamentoRetornar;
        }

        private bool IsPermitirEditarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            return (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.InicioDescarregamento);
        }

        #endregion
    }
}
