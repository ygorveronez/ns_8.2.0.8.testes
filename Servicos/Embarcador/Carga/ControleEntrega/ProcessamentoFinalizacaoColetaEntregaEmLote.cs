using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos.Embarcador.Notificacao;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga.ControleEntrega
{
    public class ProcessamentoFinalizacaoColetaEntregaEmLote
    {
        public void FinalizarColetaEntregaEmMassa(List<int> codigosCargasSelecionadas, TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, int codigoempresa, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote repositorioProcessamentoFinalizacao = new Repositorio.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaEntregasPorCarga = repositorioCargaEntrega.BuscarPorCargas(codigosCargasSelecionadas);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarPorCodigos(codigosCargasSelecionadas);

                foreach (int codigo in codigosCargasSelecionadas)
                {
                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargas.Where(x => x.Codigo == codigo).FirstOrDefault();

                    if (carga == null)
                    {
                        Servicos.Log.TratarErro("Carga não encontrada");
                        continue;
                    }

                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ProcessamentoFinalizacaoColetaEntregaEmLote processamentoFinalizacaoColetaEntregaEmLote = repositorioProcessamentoFinalizacao.BuscarPorCodigoCarga(codigo);

                    processamentoFinalizacaoColetaEntregaEmLote.Situacao = SituacaoProcessamentoFinalizacaoColetaEntregaEmLote.EmFinalizacao;
                    processamentoFinalizacaoColetaEntregaEmLote.Tentativas++;
                    processamentoFinalizacaoColetaEntregaEmLote.Descricao = "Coleta/entrega enviada para processamento";

                    repositorioProcessamentoFinalizacao.Atualizar(processamentoFinalizacaoColetaEntregaEmLote);

                    if (carga.DataInicioViagem == DateTime.MinValue || carga.DataInicioViagem == null)
                        IniciarViagemDaCarga(unitOfWork, carga, tipoServicoMultisoftware, auditado, configuracaoEmbarcador, cliente);

                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaEntregasDaCarga = listaEntregasPorCarga.Where(c => c.Carga.Codigo == codigo).ToList();


                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido> pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Pedido>();

                    OrigemSituacaoEntrega origemSituacaoEntrega = (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) ?
                                    OrigemSituacaoEntrega.UsuarioMultiEmbarcador : OrigemSituacaoEntrega.UsuarioPortalTransportador;

                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(listaEntregasDaCarga.FirstOrDefault()?.Carga.TipoOperacao?.Codigo ?? 0);
                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in listaEntregasDaCarga)
                    {
                        if (cargaEntrega.Situacao == SituacaoEntrega.Entregue)
                            continue;

                        var parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros
                        {
                            cargaEntrega = cargaEntrega,
                            dataInicioEntrega = cargaEntrega?.DataInicio ?? DateTime.Now,
                            dataTerminoEntrega = DateTime.Now,
                            dataConfirmacao = DateTime.Now,
                            dataSaidaRaio = null,
                            wayPoint = null,
                            wayPointDescarga = null,
                            pedidos = pedidos,
                            motivoRetificacao = 0,
                            justificativaEntregaForaRaio = "",
                            motivoFalhaGTA = 0,
                            configuracaoEmbarcador = configuracaoEmbarcador,
                            tipoServicoMultisoftware = tipoServicoMultisoftware,
                            sistemaOrigem = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                            dadosRecebedor = null,
                            OrigemSituacaoEntrega = origemSituacaoEntrega,
                            ClienteAreaRedex = 0,
                            Container = 0,
                            DataColetaContainer = null,
                            auditado = auditado,
                            configuracaoControleEntrega = configuracaoControleEntrega,
                            tipoOperacaoParametro = tipoOperacaoParametro,
                            TornarFinalizacaoDeEntregasAssincrona = configuracaoControleEntrega.TornarFinalizacaoDeEntregasAssincrona
                        };

                        if (!(cargaEntrega?.Cliente?.NaoExigirDigitalizacaoDoCanhotoParaEsteCliente ?? false))
                        {
                            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega svcControleEntrega = new Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);
                            svcControleEntrega.SetarCanhotosComoPendente(cargaEntrega, auditado, unitOfWork);
                        }

                        try
                        {
                            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(parametros, unitOfWork);

                        }
                        catch (ServicoException ex)
                        {
                            processamentoFinalizacaoColetaEntregaEmLote.Descricao = ex.Message;
                            processamentoFinalizacaoColetaEntregaEmLote.Situacao = SituacaoProcessamentoFinalizacaoColetaEntregaEmLote.FalhaNaFinalizacao;
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                        }

                        cargaEntrega.FinalizadaManualmente = false;

                        repositorioCargaEntrega.Atualizar(cargaEntrega);
                        Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntrega, $"Coleta/entrega finalizada através da tela de finalização em lote", unitOfWork);

                        var serNotificacaoMTrack = new NotificacaoMTrack(unitOfWork);
                        serNotificacaoMTrack.NotificarMudancaCargaEntrega(cargaEntrega, cargaEntrega.Carga.Motoristas.ToList(), AdminMultisoftware.Dominio.Enumeradores.MobileHubs.EntregaConfirmadaNoEmbarcador, notificarSignalR: true, codigoClienteMultisoftware: codigoempresa);

                    }
                    processamentoFinalizacaoColetaEntregaEmLote.Situacao = SituacaoProcessamentoFinalizacaoColetaEntregaEmLote.Finalizado;
                    processamentoFinalizacaoColetaEntregaEmLote.Descricao = "Coleta/entrega Finalizada com sucesso";

                    repositorioProcessamentoFinalizacao.Atualizar(processamentoFinalizacaoColetaEntregaEmLote);

                    unitOfWork.CommitChanges();
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw new ServicoException("Ocorreu uma falha ao processsar");
            }

            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void IniciarViagemDaCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Cargas.Carga carga, TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            int codigoCarga = carga.Codigo;
            try
            {
                if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(codigoCarga, DateTime.Now, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, null, configuracaoEmbarcador, tipoServicoMultisoftware, cliente, auditado, unitOfWork))
                {
                    Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Início de viagem informado manualmente", unitOfWork);

                    List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> listMonitoramento = repMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculoPlaca(carga.Veiculo?.Placa);

                    if (listMonitoramento.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(auditado, carga, $"Monitoramento encerrado na carga {carga.CodigoCargaEmbarcador}, visto que foi iniciada viagem na carga {carga.CodigoCargaEmbarcador}.", unitOfWork);
                }

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                throw new Exception("Ocorreu uma falha ao informar inicio de viagem.");
            }

        }
    }
}
