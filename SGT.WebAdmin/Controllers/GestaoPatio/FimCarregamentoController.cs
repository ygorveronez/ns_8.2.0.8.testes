using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/FimCarregamento", "GestaoPatio/FluxoPatio")]
    public class FimCarregamentoController : BaseController
    {
		#region Construtores

		public FimCarregamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AvancarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.GestaoPatio.FimCarregamento servicoFimCarregamento = new Servicos.Embarcador.GestaoPatio.FimCarregamento(unitOfWork, Auditado, Cliente);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FimCarregamentoAvancarEtapa fimCarregamentoAvancarEtapa = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FimCarregamentoAvancarEtapa()
                {
                    Codigo = Request.GetIntParam("Codigo"),
                    Pesagem = Request.GetDecimalParam("Pesagem")
                };

                servicoFimCarregamento.Avancar(fimCarregamentoAvancarEtapa);

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
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar o carregamento.");
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
                int codigoFimCarregamento = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.FimCarregamento repositorioFimCarregamento = new Repositorio.Embarcador.GestaoPatio.FimCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento fimCarregamento = null;

                if (codigoFimCarregamento > 0)
                    fimCarregamento = repositorioFimCarregamento.BuscarPorCodigo(codigoFimCarregamento);
                else if (codigoFluxoGestaoPatio > 0)
                    fimCarregamento = repositorioFimCarregamento.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (fimCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (fimCarregamento.Carga != null)
                    return new JsonpResult(ObterFimCarregamentoPorCarga(unitOfWork, fimCarregamento));

                return new JsonpResult(ObterFimCarregamentoPorPreCarga(unitOfWork, fimCarregamento));
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
                Repositorio.Embarcador.GestaoPatio.FimCarregamento repositorioFimCarregamento = new Repositorio.Embarcador.GestaoPatio.FimCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento fimCarregamento = repositorioFimCarregamento.BuscarPorCodigo(codigo);

                if (fimCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                if (fimCarregamento.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível reabrir o fluxo nessa situação.");

                unitOfWork.Start();

                servicoFluxoGestaoPatio.ReabrirFluxo(fimCarregamento.FluxoGestaoPatio);

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
                Repositorio.Embarcador.GestaoPatio.FimCarregamento repositorioFimCarregamento = new Repositorio.Embarcador.GestaoPatio.FimCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento fimCarregamento = repositorioFimCarregamento.BuscarPorCodigo(codigo);

                if (fimCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                servicoFluxoGestaoPatio.RejeitarEtapa(fimCarregamento.FluxoGestaoPatio, EtapaFluxoGestaoPatio.FimCarregamento);
                servicoFluxoGestaoPatio.Auditar(fimCarregamento.FluxoGestaoPatio, "Rejeitou o fluxo.");

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
                Repositorio.Embarcador.GestaoPatio.FimCarregamento repositorioFimCarregamento = new Repositorio.Embarcador.GestaoPatio.FimCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento fimCarregamento = repositorioFimCarregamento.BuscarPorCodigo(codigo);

                if (fimCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(fimCarregamento.FluxoGestaoPatio, EtapaFluxoGestaoPatio.FimCarregamento, this.Usuario, permissoesPersonalizadasFluxoPatio);

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

        public async Task<IActionResult> ImprimirSinteseMateriais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            
            try
            {
                Repositorio.Embarcador.GestaoPatio.FimCarregamento repositorioFimCarregamento = new Repositorio.Embarcador.GestaoPatio.FimCarregamento(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento fimCarregamento = repositorioFimCarregamento.BuscarPorCodigo(codigo);

                if (fimCarregamento == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                Servicos.Embarcador.GestaoPatio.FimCarregamento servicoFimCarregamento = new Servicos.Embarcador.GestaoPatio.FimCarregamento(unitOfWork, Auditado, Cliente);

                byte[] pdf = servicoFimCarregamento.ObterPdfSinteseMateriais(fimCarregamento);
                
                return Arquivo(pdf, "application/pdf", "SinteseMateriais.pdf");
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
                return new JsonpResult(false, "Ocorreu uma falha ao imprimir a síntese de materiais.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterFimCarregamentoPorCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento fimCarregamento)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fimCarregamento.FluxoGestaoPatio);
            bool permitirEditarEtapa = IsPermitirEditarEtapa(fimCarregamento.FluxoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                fimCarregamento.Codigo,
                fimCarregamento.Situacao,
                Carga = fimCarregamento.Carga.Codigo,
                PreCarga = fimCarregamento.PreCarga?.Codigo ?? 0,
                NumeroCarga = servicoCarga.ObterNumeroCarga(fimCarregamento.Carga, unitOfWork),
                NumeroPreCarga = fimCarregamento.PreCarga?.NumeroPreCarga ?? "",
                CargaData = fimCarregamento.Carga.DataCarregamentoCarga?.ToString($"dd/MM/yyyy") ?? "",
                CargaHora = fimCarregamento.Carga.DataCarregamentoCarga?.ToString($"HH:mm") ?? "",
                Transportador = fimCarregamento.Carga.Empresa?.Descricao ?? string.Empty,
                Veiculo = fimCarregamento.Carga.RetornarPlacas,
                Remetente = fimCarregamento.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = fimCarregamento.Carga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = fimCarregamento.Carga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = fimCarregamento.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                CodigoIntegracaoDestinatario = fimCarregamento.Carga.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? string.Empty,
                Pesagem = (fimCarregamento.Pesagem > 0m) ? fimCarregamento.Pesagem.ToString("n2") : "",
                PermitirEditarEtapa = permitirEditarEtapa,
                PermiteInformarPesagem = sequenciaGestaoPatio?.FimCarregamentoPermiteInformarPesagem ?? false
            };

            return docaCarregamentoRetornar;
        }

        private dynamic ObterFimCarregamentoPorPreCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento fimCarregamento)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(fimCarregamento.PreCarga.Codigo);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fimCarregamento.FluxoGestaoPatio);
            DateTime? dataCarregamento = cargaJanelaCarregamento?.InicioCarregamento;
            bool permitirEditarEtapa = IsPermitirEditarEtapa(fimCarregamento.FluxoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                fimCarregamento.Codigo,
                fimCarregamento.Situacao,
                Carga = 0,
                PreCarga = fimCarregamento.PreCarga.Codigo,
                NumeroCarga = "",
                NumeroPreCarga = fimCarregamento.PreCarga.NumeroPreCarga ?? "",
                CargaData = dataCarregamento?.ToString($"dd/MM/yyyy") ?? "",
                CargaHora = dataCarregamento?.ToString($"HH:mm") ?? "",
                Transportador = fimCarregamento.PreCarga.Empresa?.Descricao ?? string.Empty,
                Veiculo = fimCarregamento.PreCarga.RetornarPlacas,
                Remetente = fimCarregamento.PreCarga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = fimCarregamento.PreCarga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = fimCarregamento.PreCarga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = fimCarregamento.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.Nome ?? string.Empty,
                CodigoIntegracaoDestinatario = fimCarregamento.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.CodigoIntegracao ?? string.Empty,
                Pesagem = (fimCarregamento.Pesagem > 0m) ? fimCarregamento.Pesagem.ToString("n2") : "",
                PermitirEditarEtapa = permitirEditarEtapa,
                PermiteInformarPesagem = sequenciaGestaoPatio?.FimCarregamentoPermiteInformarPesagem ?? false
            };

            return docaCarregamentoRetornar;
        }

        private bool IsPermitirEditarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            return (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.FimCarregamento);
        }

        #endregion
    }
}
