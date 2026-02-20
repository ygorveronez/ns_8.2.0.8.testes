using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize(new string[] { "BuscarPorCarga" }, "GestaoPatio/FluxoPatio")]
    public class FimViagemController : BaseController
    {
        #region Construtores

        public FimViagemController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [Obsolete("Método utilizado somente no fluxo de entrega (DESCONTINUADO)")]
        public async Task<IActionResult> BuscarPorCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Carga");
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repFluxoGestaoPatio.BuscarPorCargaETipo(codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoPatio.Origem);

                if (fluxoGestaoPatio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    fluxoGestaoPatio.Codigo,
                    Carga = fluxoGestaoPatio.Carga.Codigo,
                    ViagemAberta = !fluxoGestaoPatio.DataFimViagem.HasValue,
                    DataFimViagem = (fluxoGestaoPatio.DataFimViagem?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty)
                });
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repositorioFluxoGestaoPatio.BuscarPorCodigo(codigoFluxoGestaoPatio, auditavel: false);

                if (fluxoGestaoPatio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fluxoGestaoPatio);

                return new JsonpResult(new
                {
                    fluxoGestaoPatio.Codigo,
                    Carga = fluxoGestaoPatio.Carga.Codigo,
                    ViagemAberta = !fluxoGestaoPatio.DataFimViagem.HasValue,
                    DataFimViagem = (fluxoGestaoPatio.DataFimViagem?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty),
                    ExibirDocumentosPesagem = sequenciaGestaoPatio.GuaritaEntradaPermiteInformacoesPesagem,
                    PermitirEditarEtapa = IsPermitirEditarEtapa(fluxoGestaoPatio)
                });
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

        public async Task<IActionResult> InformarFimViagem(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoGestaoPatio = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = await repositorioFluxoGestaoPatio.BuscarPorCodigoAsync(codigoFluxoGestaoPatio, false);

                if (fluxoGestaoPatio == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = await repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadraoAsync();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = fluxoGestaoPatio.Carga;

                await unitOfWork.StartAsync(cancellationToken);

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimViagem);
                Servicos.Embarcador.Monitoramento.Monitoramento.FinalizarMonitoramento(carga, DateTime.Now, configuracaoEmbarcador, base.Auditado, "Informado o fim de viagem", unitOfWork, MotivoFinalizacaoMonitoramento.FinalizadoAoFimDaViagem);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork, cancellationToken);

                if (servicoCarga.VerificarFinalizarCargaAoFinalizarGestaoPatio(carga, configuracaoEmbarcador))
                    servicoCarga.LiberarSituacaoDeCargaFinalizada(carga, unitOfWork, TipoServicoMultisoftware, Auditado, this.Usuario);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao informar o fim da viagem.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
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

                servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.SaidaLoja);

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

        public async Task<IActionResult> VoltarEtapaFimViagem()
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

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(fluxoGestaoPatio, EtapaFluxoGestaoPatio.FimViagem, this.Usuario, permissoesPersonalizadasFluxoPatio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
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

        private bool IsPermitirEditarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxo)
        {
            if ((fluxo == null) || (fluxo.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            return (fluxo.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.FimViagem);
        }

        #endregion
    }
}
