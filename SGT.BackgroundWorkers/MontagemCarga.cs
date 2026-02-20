using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoMontagemCarga : LongRunningProcessBase<IntegracaoMontagemCarga>
    {
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _ConfiguracaoEmbarcador;
        private List<int> _sessoesGerandoCarregamento;
        private List<int> _sessoesGerandoCarga;

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            GerarMontagemCargaAutomatica(unitOfWork);
            GerarCargaEmLote(unitOfWork, _tipoServicoMultisoftware);
        }

        private void GerarMontagemCargaAutomatica(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido repMontagemCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido(unidadeDeTrabalho);
            try
            {
                if (_ConfiguracaoEmbarcador == null)
                {
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);
                    _ConfiguracaoEmbarcador = repConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                }

                List<int> sessoesRoteirizador = repMontagemCarregamentoPedido.SessoesRoteirizador();
                if (sessoesRoteirizador.Count == 0)
                {
                    var pedidosSemSessao = repMontagemCarregamentoPedido.BusarCodigosPedido(0);
                    if (pedidosSemSessao.Count == 0)
                        return;
                    else
                        sessoesRoteirizador.Add(0);
                }

                Servicos.Log.TratarErro("IntegracaoMontagemCarga: GerarMontagemCargaAutomatica - SessoesRoteirizador: " + string.Join(",", sessoesRoteirizador), "MontagemCarga");

                sessoesRoteirizador = sessoesRoteirizador.Distinct().ToList();

                foreach (int sessao in sessoesRoteirizador)
                {
                    //Cria uma nova thread, indicando qual método essa thread deverá executar
                    if (_sessoesGerandoCarregamento == null) _sessoesGerandoCarregamento = new List<int>();
                    if (_sessoesGerandoCarregamento.Count == (_ConfiguracaoEmbarcador.MaximoThreadsMontagemCarga <= 0 ? 1 : _ConfiguracaoEmbarcador.MaximoThreadsMontagemCarga)) return;
                    if (_sessoesGerandoCarregamento.Contains(sessao)) return;

                    _sessoesGerandoCarregamento.Add(sessao);
                    System.Threading.Thread thread = new System.Threading.Thread(() => GerarCarregamentos(sessao));
                    thread.Start();

                    System.Threading.Tasks.Task.Delay(3000).Wait();
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                new Servicos.Embarcador.Hubs.MontagemCarga().InformarCarregamentoAutomaticoFinalizado("Erro ao gerar carregamento automatico.", 0);
            }
        }

        private void GerarCarregamentos(int codigoSessaoRoteirizador)
        {
            Servicos.Embarcador.Hubs.MontagemCarga servicoNotificacaomontagemCarga = new Servicos.Embarcador.Hubs.MontagemCarga();

            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido repMontagemCarregamentoPedido = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamentoPedido(unitOfWork);

                    try
                    {
                        Servicos.Log.TratarErro("IntegracaoMontagemCarga: GerarMontagemCargaAutomatica - Sessao: " + codigoSessaoRoteirizador.ToString() + " INICIOU.", "MontagemCarga");
                        List<int> codigosPedidos = repMontagemCarregamentoPedido.BusarCodigosPedido(codigoSessaoRoteirizador);
                        codigosPedidos = codigosPedidos.Distinct().ToList();
                        repMontagemCarregamentoPedido.DeletarTodos(codigoSessaoRoteirizador);

                        Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork, null, _stringConexao);
                        servicoMontagemCarga.GerarCarregamentoAutomatico(codigosPedidos, codigoSessaoRoteirizador, _tipoServicoMultisoftware);

                    }
                    catch (Dominio.Excecoes.Embarcador.ServicoException see)
                    {
                        Servicos.Log.TratarErro(see);
                        repMontagemCarregamentoPedido.DeletarTodos(codigoSessaoRoteirizador);
                        servicoNotificacaomontagemCarga.InformarCarregamentoAutomaticoFinalizado(see.Message, codigoSessaoRoteirizador);
                    }
                    catch (Exception exx)
                    {
                        Servicos.Log.TratarErro(exx);
                        repMontagemCarregamentoPedido.DeletarTodos(codigoSessaoRoteirizador);
                        servicoNotificacaomontagemCarga.InformarCarregamentoAutomaticoFinalizado("Erro ao gerar carregamento automatico.", codigoSessaoRoteirizador);
                    }
                    finally
                    {
                        _sessoesGerandoCarregamento.Remove(codigoSessaoRoteirizador);
                        Servicos.Log.TratarErro("IntegracaoMontagemCarga: GerarMontagemCargaAutomatica - Sessao: " + codigoSessaoRoteirizador.ToString() + " FINALIZOU.", "MontagemCarga");
                    }
                }
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException se)
            {
                Servicos.Log.TratarErro("IntegracaoMontagemCarga: GerarMontagemCargaAutomatica - Sessao: " + codigoSessaoRoteirizador.ToString() + " Erro: " + se.ToString(), "MontagemCarga");
                servicoNotificacaomontagemCarga.InformarCarregamentoAutomaticoFinalizado(se.Message, codigoSessaoRoteirizador);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("IntegracaoMontagemCarga: GerarMontagemCargaAutomatica - Sessao: " + codigoSessaoRoteirizador.ToString() + " Erro: " + ex.ToString(), "MontagemCarga");
                servicoNotificacaomontagemCarga.InformarCarregamentoAutomaticoFinalizado("Erro ao gerar carregamento automatico.", codigoSessaoRoteirizador);
            }
        }

        private void GerarCargaEmLote(Repositorio.UnitOfWork unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
#if DEBUG
            return;
#endif
            Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamento repMontagemCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamento(unidadeDeTrabalho);
            int codigoSessaoRoteirizador = 0;
            try
            {
                List<int> sessoesRoteirizador = repMontagemCarregamento.SessoesRoteirizador();
                if (sessoesRoteirizador.Count == 0)
                {
                    var codigosCarregamento = repMontagemCarregamento.BusarCodigosCarregamento(0);
                    if (codigosCarregamento.Count == 0)
                        return;
                    else
                        sessoesRoteirizador.Add(0);
                }

                sessoesRoteirizador = sessoesRoteirizador.Distinct().ToList();

                Servicos.Log.TratarErro("IntegracaoMontagemCarga: GerarCargaEmLote - SessoesRoteirizador " + string.Join(",", sessoesRoteirizador), "MontagemCarga");

                foreach (int sessao in sessoesRoteirizador)
                {
                    try
                    {
                        //Cria uma nova thread, indicando qual método essa thread deverá executar
                        if (_sessoesGerandoCarga == null) _sessoesGerandoCarga = new List<int>();
                        if (_sessoesGerandoCarga.Count == (_ConfiguracaoEmbarcador.MaximoThreadsMontagemCarga <= 0 ? 1 : _ConfiguracaoEmbarcador.MaximoThreadsMontagemCarga)) return;
                        if (_sessoesGerandoCarga.Contains(sessao)) return;

                        _sessoesGerandoCarga.Add(sessao);
                        System.Threading.Thread thread = new System.Threading.Thread(() => GerarCargas(sessao, tipoServicoMultisoftware, _clienteMultisoftware));
                        thread.Start();

                        System.Threading.Tasks.Task.Delay(3000).Wait();

                    }
                    catch (Exception exx)
                    {
                        Servicos.Log.TratarErro(exx);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                new Servicos.Embarcador.Hubs.MontagemCarga().InformarCargaEmLoteFinalizado("Erro ao gerar cargas", codigoSessaoRoteirizador);
            }
        }

        private void GerarCargas(int codigoSessaoRoteirizador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente ClienteMultisoftware)
        {
            try
            {
                Servicos.Embarcador.Hubs.MontagemCarga servicoNotificacaomontagemCarga = new Servicos.Embarcador.Hubs.MontagemCarga();

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                {
                    Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamento repMontagemCarregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.MontagemCarregamento(unitOfWork);

                    List<int> codigos = new List<int>();
                    try
                    {
                        Servicos.Log.TratarErro("IntegracaoMontagemCarga: GerarCargaEmLote - Sessao " + codigoSessaoRoteirizador.ToString(), "MontagemCarga");

                        List<int> codigosCarregamento = repMontagemCarregamento.BusarCodigosCarregamento(codigoSessaoRoteirizador);
                        codigosCarregamento = codigosCarregamento.Distinct().ToList();
                        List<int> codigosCarregamentoBackground = repMontagemCarregamento.BusarCodigosCarregamento(codigoSessaoRoteirizador, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.GerandoCargaBackground);

                        codigos.AddRange(codigosCarregamento);
                        codigos.AddRange(codigosCarregamentoBackground);
                        repMontagemCarregamento.DeletarTodos(codigos);

                        Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unitOfWork);

                        if (codigosCarregamento.Count > 0)
                            servicoMontagemCarga.GerarCargaEmLote(codigosCarregamento, tipoServicoMultisoftware, ClienteMultisoftware, codigoSessaoRoteirizador, false, _urlAcesso);

                        if (codigosCarregamentoBackground.Count > 0)
                            servicoMontagemCarga.GerarCargaEmLote(codigosCarregamentoBackground, tipoServicoMultisoftware, ClienteMultisoftware, codigoSessaoRoteirizador, true, _urlAcesso);

                    }
                    catch (Exception exx)
                    {
                        Servicos.Log.TratarErro(exx);
                        servicoNotificacaomontagemCarga.InformarCarregamentoAutomaticoFinalizado("Erro ao gerar cargas.", codigoSessaoRoteirizador);
                    }
                    finally
                    {
                        _sessoesGerandoCarga.Remove(codigoSessaoRoteirizador);
                        Servicos.Log.TratarErro("IntegracaoMontagemCarga: GerarCargas - Sessao: " + codigoSessaoRoteirizador.ToString() + " FINALIZOU.", "MontagemCarga");
                        repMontagemCarregamento.DeletarTodos(codigos);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("IntegracaoMontagemCarga: GerarCargas - Sessao: " + codigoSessaoRoteirizador.ToString() + " Erro: " + ex.ToString(), "MontagemCarga");
            }
        }
    }
}