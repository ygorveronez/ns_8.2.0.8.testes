using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.TorreControle
{
    [CustomAuthorize("TorreControle/TendenciaParadas")]
    public class TendenciaParadasController : BaseController
    {
        #region Construtores
        public TendenciaParadasController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarConfiguracao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao repositorioAcompanhamentoEntregaTempoConfiguracao = new Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao acompanhamentoEntregaTempoConfiguracao = await repositorioAcompanhamentoEntregaTempoConfiguracao.BuscarConfiguracaoAsync();

                var retorno = new
                {
                    Codigo = acompanhamentoEntregaTempoConfiguracao?.Codigo ?? 0,
                    DataReferencia = acompanhamentoEntregaTempoConfiguracao?.DataReferencia ?? 0,
                    ConsiderarDataEntradaComoDestino = acompanhamentoEntregaTempoConfiguracao?.ConsiderarDataEntradaComoDestino ?? false,
                    TempoAdiantado = acompanhamentoEntregaTempoConfiguracao?.HabilitarTendenciaAdiantamento ?? true,
                    TempoAdiantadoColeta = acompanhamentoEntregaTempoConfiguracao?.SaidaEmTempo ?? TimeSpan.Zero,
                    TempoAdiantadoEntrega = acompanhamentoEntregaTempoConfiguracao?.DestinoEmTempo ?? TimeSpan.Zero,
                    TempoAtrasado = acompanhamentoEntregaTempoConfiguracao?.HabilitarTendenciaAtraso ?? true,
                    TempoAtrasadoColeta = acompanhamentoEntregaTempoConfiguracao?.SaidaAtraso3 ?? TimeSpan.Zero,
                    TempoAtrasadoEntrega = acompanhamentoEntregaTempoConfiguracao?.DestinoAtraso3 ?? TimeSpan.Zero
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.FalhaBuscarConfiguracao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarConfiguracao(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            int codigo = Request.GetIntParam("Codigo");
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseAlerta DataReferencia = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DataBaseAlerta>("DataReferencia");
            bool ConsiderarDataEntradaComoDestino = Request.GetBoolParam("ConsiderarDataEntradaComoDestino");
            bool TempoAdiantado = Request.GetBoolParam("TempoAdiantado");
            TimeSpan TempoAdiantadoColeta = Request.GetTimeParam("TempoAdiantadoColeta");
            TimeSpan TempoAdiantadoEntrega = Request.GetTimeParam("TempoAdiantadoEntrega");
            bool TempoAtrasado = Request.GetBoolParam("TempoAtrasado");
            TimeSpan TempoAtrasadoColeta = Request.GetTimeParam("TempoAtrasadoColeta");
            TimeSpan TempoAtrasadoEntrega = Request.GetTimeParam("TempoAtrasadoEntrega");

            try
            {
                await unitOfWork.StartAsync();

                Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao repositorioAcompanhamentoEntregaTempoConfiguracao = new Repositorio.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao acompanhamentoEntregaTempoConfiguracao = await repositorioAcompanhamentoEntregaTempoConfiguracao.BuscarPorCodigoAsync(codigo, false) ?? new Dominio.Entidades.Embarcador.Cargas.AcompanhamentoEntregaTempoConfiguracao.AcompanhamentoEntregaTempoConfiguracao();

                acompanhamentoEntregaTempoConfiguracao.Initialize();

                acompanhamentoEntregaTempoConfiguracao.DataReferencia = DataReferencia;
                acompanhamentoEntregaTempoConfiguracao.ConsiderarDataEntradaComoDestino = ConsiderarDataEntradaComoDestino;
                acompanhamentoEntregaTempoConfiguracao.HabilitarTendenciaAdiantamento = TempoAdiantado;
                acompanhamentoEntregaTempoConfiguracao.SaidaEmTempo = TempoAdiantadoColeta;
                acompanhamentoEntregaTempoConfiguracao.DestinoEmTempo = TempoAdiantadoEntrega;
                acompanhamentoEntregaTempoConfiguracao.HabilitarTendenciaAtraso = TempoAtrasado;
                acompanhamentoEntregaTempoConfiguracao.SaidaAtraso3 = TempoAtrasadoColeta;
                acompanhamentoEntregaTempoConfiguracao.DestinoAtraso3 = TempoAtrasadoEntrega;

                if (acompanhamentoEntregaTempoConfiguracao.Codigo > 0)
                    await repositorioAcompanhamentoEntregaTempoConfiguracao.AtualizarAsync(acompanhamentoEntregaTempoConfiguracao, Auditado);
                else
                    await repositorioAcompanhamentoEntregaTempoConfiguracao.InserirAsync(acompanhamentoEntregaTempoConfiguracao, Auditado);

                await unitOfWork.CommitChangesAsync();
                return new JsonpResult(true, Localization.Resources.Cargas.ControleEntrega.ConfiguracaoSalvaSucesso);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, Localization.Resources.Cargas.ControleEntrega.FalhaSalvarConfiguracao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion
    }
}
