using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/InicioCarregamento", "GestaoPatio/FluxoPatio")]
    public class InicioCarregamentoController : BaseController
    {
		#region Construtores

		public InicioCarregamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AvancarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.GestaoPatio.InicioCarregamento servicoInicioCarregamento = new Servicos.Embarcador.GestaoPatio.InicioCarregamento(unitOfWork, Auditado, Cliente);
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.InicioCarregamentoAvancarEtapa inicioCarregamentoAvancarEtapa = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.InicioCarregamentoAvancarEtapa()
                {
                    Codigo = Request.GetIntParam("Codigo"),
                    Pesagem = Request.GetDecimalParam("Pesagem")
                };

                servicoInicioCarregamento.Avancar(inicioCarregamentoAvancarEtapa);

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
                return new JsonpResult(false, "Ocorreu uma falha ao iniciar o carregamento.");
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
                int codigoInicioCarregamento = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.InicioCarregamento repositorioInicioCarregamento = new Repositorio.Embarcador.GestaoPatio.InicioCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento inicioCarregamento = null;

                if (codigoInicioCarregamento > 0)
                    inicioCarregamento = repositorioInicioCarregamento.BuscarPorCodigo(codigoInicioCarregamento);
                else if (codigoFluxoGestaoPatio > 0)
                    inicioCarregamento = repositorioInicioCarregamento.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (inicioCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (inicioCarregamento.Carga != null)
                    return new JsonpResult(ObterInicioCarregamentoPorCarga(unitOfWork, inicioCarregamento));

                return new JsonpResult(ObterInicioCarregamentoPorPreCarga(unitOfWork, inicioCarregamento));
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
                Repositorio.Embarcador.GestaoPatio.InicioCarregamento repositorioInicioCarregamento = new Repositorio.Embarcador.GestaoPatio.InicioCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento inicioCarregamento = repositorioInicioCarregamento.BuscarPorCodigo(codigo);

                if (inicioCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                if (inicioCarregamento.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível reabrir o fluxo nessa situação.");

                unitOfWork.Start();

                servicoFluxoGestaoPatio.ReabrirFluxo(inicioCarregamento.FluxoGestaoPatio);

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
                Repositorio.Embarcador.GestaoPatio.InicioCarregamento repositorioInicioCarregamento = new Repositorio.Embarcador.GestaoPatio.InicioCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento inicioCarregamento = repositorioInicioCarregamento.BuscarPorCodigo(codigo);

                if (inicioCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                servicoFluxoGestaoPatio.RejeitarEtapa(inicioCarregamento.FluxoGestaoPatio, EtapaFluxoGestaoPatio.InicioCarregamento);
                servicoFluxoGestaoPatio.Auditar(inicioCarregamento.FluxoGestaoPatio, "Rejeitou o fluxo.");
                
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
                Repositorio.Embarcador.GestaoPatio.InicioCarregamento repositorioInicioCarregamento = new Repositorio.Embarcador.GestaoPatio.InicioCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento inicioCarregamento = repositorioInicioCarregamento.BuscarPorCodigo(codigo);

                if (inicioCarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(inicioCarregamento.FluxoGestaoPatio, EtapaFluxoGestaoPatio.InicioCarregamento, this.Usuario, permissoesPersonalizadasFluxoPatio);

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

        private dynamic ObterInicioCarregamentoPorCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento inicioCarregamento)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(inicioCarregamento.FluxoGestaoPatio);
            bool permitirEditarEtapa = IsPermitirEditarEtapa(inicioCarregamento.FluxoGestaoPatio);

            var inicioCarregamentoRetornar = new
            {
                inicioCarregamento.Codigo,
                inicioCarregamento.Situacao,
                Carga = inicioCarregamento.Carga.Codigo,
                PreCarga = inicioCarregamento.PreCarga?.Codigo ?? 0,
                NumeroCarga = servicoCarga.ObterNumeroCarga(inicioCarregamento.Carga, unitOfWork),
                NumeroPreCarga = inicioCarregamento.PreCarga?.NumeroPreCarga ?? "",
                CargaData = inicioCarregamento.Carga.DataCarregamentoCarga?.ToString($"dd/MM/yyyy") ?? "",
                CargaHora = inicioCarregamento.Carga.DataCarregamentoCarga?.ToString($"HH:mm") ?? "",
                Transportador = inicioCarregamento.Carga.Empresa?.Descricao ?? string.Empty,
                Veiculo = inicioCarregamento.Carga.RetornarPlacas,
                Remetente = inicioCarregamento.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = inicioCarregamento.Carga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = inicioCarregamento.Carga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = inicioCarregamento.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                CodigoIntegracaoDestinatario = inicioCarregamento.Carga.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? string.Empty,
                Pesagem = (inicioCarregamento.Pesagem > 0m) ? inicioCarregamento.Pesagem.ToString("n2") : "",
                PermitirEditarEtapa = permitirEditarEtapa,
                PermiteInformarPesagem = sequenciaGestaoPatio?.InicioCarregamentoPermiteInformarPesagem ?? false,
                DataLacre = inicioCarregamento.DataLacreInicioCarregamento?.ToString($"dd/MM/yyyy") ?? "",
                NumeroDoca = inicioCarregamento.Carga.NumeroDoca ?? ""
            };

            return inicioCarregamentoRetornar;
        }

        private dynamic ObterInicioCarregamentoPorPreCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento inicioCarregamento)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(inicioCarregamento.PreCarga.Codigo);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(inicioCarregamento.FluxoGestaoPatio);
            DateTime? dataCarregamento = cargaJanelaCarregamento?.InicioCarregamento;
            bool permitirEditarEtapa = IsPermitirEditarEtapa(inicioCarregamento.FluxoGestaoPatio);

            var inicioCarregamentoRetornar = new
            {
                inicioCarregamento.Codigo,
                inicioCarregamento.Situacao,
                Carga = 0,
                PreCarga = inicioCarregamento.PreCarga.Codigo,
                NumeroCarga = "",
                NumeroPreCarga = inicioCarregamento.PreCarga.NumeroPreCarga ?? "",
                CargaData = dataCarregamento?.ToString($"dd/MM/yyyy") ?? "",
                CargaHora = dataCarregamento?.ToString($"HH:mm") ?? "",
                Transportador = inicioCarregamento.PreCarga.Empresa?.Descricao ?? string.Empty,
                Veiculo = inicioCarregamento.PreCarga.RetornarPlacas,
                Remetente = inicioCarregamento.PreCarga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = inicioCarregamento.PreCarga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = inicioCarregamento.PreCarga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = inicioCarregamento.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.Nome ?? string.Empty,
                CodigoIntegracaoDestinatario = inicioCarregamento.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.CodigoIntegracao ?? string.Empty,
                Pesagem = (inicioCarregamento.Pesagem > 0m) ? inicioCarregamento.Pesagem.ToString("n2") : "",
                PermitirEditarEtapa = permitirEditarEtapa,
                PermiteInformarPesagem = sequenciaGestaoPatio?.InicioCarregamentoPermiteInformarPesagem ?? false
            };

            return inicioCarregamentoRetornar;
        }

        private bool IsPermitirEditarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            return (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.InicioCarregamento);
        }

        #endregion
    }
}
