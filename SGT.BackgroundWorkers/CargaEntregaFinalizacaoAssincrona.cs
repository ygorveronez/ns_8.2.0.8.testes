using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]

    public class CargaEntregaFinalizacaoAssincrona : LongRunningProcessBase<CargaEntregaFinalizacaoAssincrona>
    {
        #region Metodos Publicos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona repositorioCargaEntregaFinalizacaoAssincrona = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona> listaFinalizacoes = repositorioCargaEntregaFinalizacaoAssincrona.BuscarPendentesProcessamento(10);

#if DEBUG
            listaFinalizacoes = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona>() { repositorioCargaEntregaFinalizacaoAssincrona.BuscarPorCodigo(922, false), repositorioCargaEntregaFinalizacaoAssincrona.BuscarPorCodigo(916, false), repositorioCargaEntregaFinalizacaoAssincrona.BuscarPorCodigo(917, false) };
#endif

            Servicos.Log.TratarErro($"___________________INICIO {listaFinalizacoes.Count} Registros____________________", "CargaEntregaFinalizacaoAssincrona");
            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFinalizacaoAssincrona finalizacao in listaFinalizacoes)
            {
                Servicos.Log.TratarErro($"Início processamento: {finalizacao.Codigo}", "CargaEntregaFinalizacaoAssincrona");
                string mensagemProcessamento = string.Empty;
                SituacaoProcessamentoIntegracao situacaoProcesasmento = SituacaoProcessamentoIntegracao.AguardandoProcessamento;
                try
                {
                    Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaAssincronaParametros parametrosFinalizacaoAssincrona = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaAssincronaParametros>(finalizacao.ParametrosFinalizacao);
                    Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros parametros = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ObterParametrosFinalizacaoAssincrona(parametrosFinalizacaoAssincrona, unitOfWork);
                    if (parametros == null || parametros.cargaEntrega == null) throw new ServicoException("Falha ao obter parametros para finalização da entrega.");

                    parametros.ExecutarValidacoes = false;
                    parametros.TornarFinalizacaoDeEntregasAssincrona = false;

                    unitOfWork.Start();
                    Servicos.Log.TratarErro($"INI - FinalizarEntrega Coleta/Entrega {parametros.cargaEntrega.Codigo}", "CargaEntregaFinalizacaoAssincrona");

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(parametros, unitOfWork);

                    Servicos.Log.TratarErro($"FIM - FinalizarEntrega Coleta/Entrega {parametros.cargaEntrega.Codigo}", "CargaEntregaFinalizacaoAssincrona");
                    unitOfWork.CommitChanges();
                    //unitOfWork.FlushAndClear();

                    mensagemProcessamento = SituacaoProcessamentoIntegracao.Processado.ObterDescricao();
                    situacaoProcesasmento = SituacaoProcessamentoIntegracao.Processado;
                }
                catch (ServicoException ex)
                {
                    mensagemProcessamento = ex.Message;
                    situacaoProcesasmento = SituacaoProcessamentoIntegracao.ErroProcessamento;
                    Servicos.Log.TratarErro(ex, "CargaEntregaFinalizacaoAssincrona");
                    unitOfWork.Rollback();
                }
                catch (Exception ex)
                {
                    mensagemProcessamento = "Falha genérica ao processar finalização da entrega.";
                    situacaoProcesasmento = SituacaoProcessamentoIntegracao.ErroProcessamento;
                    Servicos.Log.TratarErro(ex, "CargaEntregaFinalizacaoAssincrona");
                    unitOfWork.Rollback();
                }
                finally
                {
                    finalizacao.SituacaoProcessamento = situacaoProcesasmento;
                    finalizacao.DetalhesProcessamento = mensagemProcessamento;
                    finalizacao.DataProcessamento = DateTime.Now;
                    finalizacao.NumeroTentativas++;
                    repositorioCargaEntregaFinalizacaoAssincrona.Atualizar(finalizacao);
                    Servicos.Log.TratarErro($"Fim processamento: {finalizacao.Codigo}", "CargaEntregaFinalizacaoAssincrona");
                }
            }
            Servicos.Log.TratarErro($"_____________________FIM_____________________", "CargaEntregaFinalizacaoAssincrona");
        }

        #endregion
    }
}