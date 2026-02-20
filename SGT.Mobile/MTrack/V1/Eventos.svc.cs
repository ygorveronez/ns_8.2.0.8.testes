using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.NovoApp.Comum;
using Dominio.ObjetosDeValor.NovoApp.Eventos;
using System;
using System.Collections.Generic;

namespace SGT.Mobile.MTrack.V1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Eventos" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Eventos.svc or Eventos.svc.cs at the Solution Explorer and start debugging.
    public class Eventos : BaseControllerNovoApp, IEventos
    {
        public ResponseBool IniciarEvento(RequestIniciarEvento request)
        {
            var usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out var unitOfWork);

            try
            {
                Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento = repMonitoramentoEvento.BuscarPorCodigo(request.codigoTipoEvento, false);

                if (monitoramentoEvento == null)
                {
                    throw new WebServiceException("Tipo de evento não encontrado");
                }

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(request.codigoCarga);

                DateTime dataEvento = DateTime.Now.AddMilliseconds(-request.coordenada.diferencaEmMilissegundos);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarAlerta(
                    monitoramentoEvento.TipoAlerta,
                    monitoramentoEvento,
                    (decimal)request.coordenada.latitude,
                    (decimal)request.coordenada.longitude,
                    dataEvento,
                    request.observacao,
                    carga,
                    unitOfWork
                );

                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu uma falha ao iniciar o evento", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }

        public ResponseBool FinalizarEvento(RequestFinalizarEvento request)
        {
            var usuarioMobileCliente = ValidarSessaoEObterUsuarioMobileCliente(request.clienteMultisoftware, out var unitOfWork);

            try
            {
                Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento = repMonitoramentoEvento.BuscarPorCodigo(request.codigoTipoEvento, false);

                if (monitoramentoEvento == null)
                {
                    throw new WebServiceException("Tipo de evento não encontrado");
                }

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(request.codigoCarga);
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
                Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertas = repAlertaMonitor.BuscarEmAbertoPorCarga(carga.Codigo, monitoramentoEvento.TipoAlerta);

                unitOfWork.Start();

                foreach (var alerta in alertas)
                {
                    alerta.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado;
                    alerta.DataFim = alerta.Data.AddMilliseconds(request.milisegundosEvento);
                    if (!string.IsNullOrWhiteSpace(request.observacao))
                        alerta.Observacao = request.observacao;

                    repAlertaMonitor.Atualizar(alerta);

                    servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(alerta, null);
                }

                unitOfWork.CommitChanges();

                return new ResponseBool
                {
                    Sucesso = true,
                };
            }
            catch (BaseException ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                RetornarErro("Ocorreu uma falha ao finalizar o evento", System.Net.HttpStatusCode.InternalServerError);
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return new ResponseBool
            {
                Sucesso = false,
            };
        }
    }
}
