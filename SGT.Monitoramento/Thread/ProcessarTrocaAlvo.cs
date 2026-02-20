using System;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.Specialized;
using System.Data.Common;
using System.Linq;
using System.IO;
using Servicos.Embarcador.Monitoramento;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.Monitoramento.Thread
{

    public class MonitoramentoCarga
    {
        public int CodigoMonitoramento { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoVeiculo { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
    }

    public class PosicaoEmAlvo
    {
        public DateTime Data { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class PermanenciasClientesCarga
    {
        public int CodigoCarga;
        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> PermanenciaClientes;
    }

    public class PermanenciasSubareasCarga
    {
        public int CodigoCarga;
        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> PermanenciaSubareas;
    }

    public class PermanenciasLocalCarga
    {
        public int CodigoCarga;
        public List<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal> PermanenciaLocais;
    }

    public class CargaEntregasCarga
    {
        public int CodigoCarga;
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> CargaEntregas;
    }

    public class ClienteOrigemCarga
    {
        public int CodigoCarga;
        public double? CodigoCliente;
    }

    public class ProcessarTrocaAlvo : AbstractThreadProcessamento
    {

        #region Atributos privados

        private static ProcessarTrocaAlvo Instante;
        private static System.Threading.Thread ProcessarTrocasAlvoThread;

        private int tempoSleep = 5;
        private bool enable = true;
        private int limiteRegistros = 100;
        private string diretorioFila;
        private string arquivoFilaPrefixo;
        private string arquivoNivelLog;
        private DateTime dataAtual;
        private bool geolocalizacaoApenasJuridico = false;
        private bool gerarEventoPermanenciaLocal = false;

        private List<PermanenciasClientesCarga> permanenciasClientesCargaCache = null;
        private List<PermanenciasLocalCarga> permanenciasLocaisCargaCache = null;
        private List<PermanenciasSubareasCarga> permanenciasSubareasCargaCache = null;
        private List<CargaEntregasCarga> cargaEntregasCargaCache = null;
        private List<ClienteOrigemCarga> clienteOrigemCargaCache = null;

        private List<string> arquivosEmProcessamento;

        #endregion

        #region Métodos públicos

        // Singleton
        public static ProcessarTrocaAlvo GetInstance(string stringConexao)
        {
            if (Instante == null)
                Instante = new ProcessarTrocaAlvo(stringConexao);
            return Instante;
        }

        public System.Threading.Thread Iniciar(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            if (enable)
                ProcessarTrocasAlvoThread = base.IniciarThread(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware, arquivoNivelLog, tempoSleep);

            return ProcessarTrocasAlvoThread;
        }

        public void Finalizar()
        {
            if (enable)
                Parar();
        }

        #endregion

        #region Implementação dos métodos abstratos

        override protected void Executar(Repositorio.UnitOfWork unitOfWork, string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            BuscarProcessarTrocaAlvo(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware);
        }

        override protected void Parar()
        {
            if (ProcessarTrocasAlvoThread != null)
            {
                ProcessarTrocasAlvoThread.Abort();
                ProcessarTrocasAlvoThread = null;
            }
        }

        #endregion

        #region Construtor privado

        private ProcessarTrocaAlvo(string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = string.IsNullOrWhiteSpace(stringConexao) ? null : new Repositorio.UnitOfWork(stringConexao);
            try
            {
                enable = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarTrocaAlvo().Ativo;
                tempoSleep = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarTrocaAlvo().TempoSleepThread;
                limiteRegistros = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarTrocaAlvo().LimiteRegistros;
                arquivoNivelLog = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarTrocaAlvo().ArquivoNivelLog;
                diretorioFila = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarTrocaAlvo().DiretorioFila;
                arquivoFilaPrefixo = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarTrocaAlvo().ArquivoFilaPrefixo;
                geolocalizacaoApenasJuridico = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarTrocaAlvo().GeolocalizacaoApenasJuridico;
                gerarEventoPermanenciaLocal = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarTrocaAlvo().GerarPermanenciaLocais;

            }
            catch (Exception e)
            {
                Log(e.Message);
                throw e;
            }
            finally
            {
                unitOfWork?.Dispose();
            }
        }

        #endregion

        #region Métodos privados

        private void BuscarProcessarTrocaAlvo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            this.dataAtual = DateTime.Now;

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo> trocasDeAlvo = BuscarPendenciasTrocasDeAlvo(unitOfWork, limiteRegistros);
            Log(trocasDeAlvo.Count + " trocas de alvo pendentes");

            cargaEntregasCargaCache = new List<CargaEntregasCarga>();
            permanenciasSubareasCargaCache = new List<PermanenciasSubareasCarga>();
            permanenciasLocaisCargaCache = new List<PermanenciasLocalCarga>();
            permanenciasClientesCargaCache = new List<PermanenciasClientesCarga>();
            clienteOrigemCargaCache = new List<ClienteOrigemCarga>();

            Dominio.ObjetosDeValor.Cliente[] clientesFronteira = null;

            DateTime inicio, inicio1;
            int total = trocasDeAlvo.Count;
            if (total > 0)
            {
                Log($"{total} trocas de alvo em processamento");

                // Extrair os monitoramentos únicos da lista
                List<MonitoramentoCarga> monitoramentosCarga = ExtrairMonitoramentos(trocasDeAlvo);
                total = monitoramentosCarga.Count;
                Log($"{total} monitoramentos distintos", 1);

                // Processa as saídas das fronteireas
                if (configuracao.MonitorarPassagensFronteiras)
                {

                    clientesFronteira = BuscarClientesFronteiraAlfandegaComGeolocalizacao(unitOfWork);
                    if (clientesFronteira.Length > 0)
                    {
                        inicio = DateTime.UtcNow;
                        Log("MonitorarPassagensFronteiras", 1);

                        unitOfWork.Start(); //Não tem problemas com o start aqui.
                        try
                        {
                            inicio1 = DateTime.UtcNow;
                            for (int i = 0; i < total; i++)
                            {
                                ProcessarPassagemFronteira(unitOfWork, monitoramentosCarga[i], trocasDeAlvo, clientesFronteira, configuracao, tipoServicoMultisoftware, clienteMultisoftware);
                            }
                            Log("ProcessarPassagemFronteira", inicio1, 2);

                            inicio1 = DateTime.UtcNow;
                            unitOfWork.CommitChanges();
                            Log("CommitChanges ProcessarPassagemFronteira", inicio1, 2);

                        }
                        catch (Exception e)
                        {
                            unitOfWork.Rollback();
                            Servicos.Log.TratarErro(e);
                            throw e;
                        }

                        Log("MonitorarPassagensFronteiras", inicio, 1);
                    }
                }

                // Processa as trocas dos alvos dos monitoramentos
                inicio = DateTime.UtcNow;
                Log("ProcessarTrocasDeAlvoMonitoramento", 1);

                unitOfWork.Start(); //Não tem problemas com o start aqui.

                try
                {
                    inicio1 = DateTime.UtcNow;
                    for (int i = 0; i < total; i++)
                    {
                        if (!unitOfWork.IsActiveTransaction()) unitOfWork.Start();
                        ProcessarTrocasDeAlvoMonitoramento(unitOfWork, monitoramentosCarga[i], trocasDeAlvo, clientesFronteira, configuracao, tipoServicoMultisoftware, clienteMultisoftware, configuracaoMonitoramento);
                    }

                    Log("ProcessarTrocasDeAlvoMonitoramento", inicio1, 2);

                    inicio1 = DateTime.UtcNow;
                    unitOfWork.CommitChanges();
                    Log("CommitChanges ProcessarTrocasDeAlvoMonitoramento", inicio1, 2);

                }
                catch (Exception e)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(e);
                    throw e;
                }

                Log("ProcessarTrocasDeAlvoMonitoramento", inicio, 2);

                // Processa as saídas dos alvos dos monitoramentos
                if (configuracao.DistanciaMinimaPercorridaParaSaidaDoAlvo > 0)
                {
                    Repositorio.Embarcador.Logistica.MonitoramentoSaidaAlvo repMonitoramentoSaidaAlvo = new Repositorio.Embarcador.Logistica.MonitoramentoSaidaAlvo(unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoSaidaDeAlvo> saidasDeAlvo = repMonitoramentoSaidaAlvo.BuscarSaidasDeAlvoPendentes((from obj in monitoramentosCarga select obj.CodigoMonitoramento).ToList(), limiteRegistros);
                    int totalSaidasDeAlvo = saidasDeAlvo.Count;
                    Log($"ProcessarSaidasDeAlvoMonitoramento {totalSaidasDeAlvo}", 1);

                    if (totalSaidasDeAlvo > 0)
                    {
                        List<int> codigosExcluir = new List<int>();
                        inicio = DateTime.UtcNow;

                        try
                        {
                            inicio1 = DateTime.UtcNow;
                            for (int i = 0; i < total; i++)
                            {
                                List<int> codigosSaidasAlvoExcluir = ProcessarSaidasDeAlvoMonitoramento(unitOfWork, monitoramentosCarga[i], saidasDeAlvo, clientesFronteira, configuracao, configuracaoIntegracao, tipoServicoMultisoftware, clienteMultisoftware);
                                codigosExcluir = codigosExcluir.Concat(codigosSaidasAlvoExcluir).ToList();
                            }
                            Log("ProcessarSaidasDeAlvoMonitoramento", inicio1, 2);

                            inicio1 = DateTime.UtcNow;

                            Log("CommitChanges ProcessarSaidasDeAlvoMonitoramento", inicio1, 2);

                        }
                        catch (Exception e)
                        {
                            Servicos.Log.TratarErro(e);
                            throw e;
                        }

                        Log("ProcessarSaidasDeAlvoMonitoramento", inicio, 1);

                        total = codigosExcluir.Count;
                        if (total > 0)
                        {
                            inicio = DateTime.UtcNow;
                            Log($"Excluindo {total} pendencias saidas", 1);
                            DbConnection connection1 = unitOfWork.GetConnection();
                            DbTransaction transaction1 = connection1.BeginTransaction();
                            try
                            {
                                inicio1 = DateTime.UtcNow;
                                repMonitoramentoSaidaAlvo.ExcluiPorCodigos(codigosExcluir, connection1, transaction1);
                                Log("repMonitoramentoSaidaAlvo.ExcluiPorCodigo", inicio1, 2);

                                inicio1 = DateTime.UtcNow;
                                transaction1.Commit();
                                Log("CommitChanges repMonitoramentoSaidaAlvo.ExcluiPorCodigo", inicio1, 2);

                            }
                            catch (Exception e)
                            {
                                transaction1.Rollback();
                                Servicos.Log.TratarErro(e);
                                throw e;
                            }
                            Log($"Excluidas {total} pendencias saidas", inicio, 1);
                        }
                    }
                }

                // Exclui o registro de pendência de processamento da troca de alvo
                ExcluirArquivosDePendencias();

                Log($"{trocasDeAlvo.Count} trocas de alvo processadas com sucesso");
            }
        }

        private void ProcessarTrocasDeAlvoMonitoramento(Repositorio.UnitOfWork unitOfWork, MonitoramentoCarga monitoramentoCarga, List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo> trocasDeAlvo, Dominio.ObjetosDeValor.Cliente[] clientesFronteira, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento)
        {
            DateTime inicio = DateTime.UtcNow;

            // Extrair e processa as trocas de alvo do monitoramento
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo> trocasDeAlvoMonitoramento = ExtrairTrocasDeAlvoMonitoramento(trocasDeAlvo, monitoramentoCarga.CodigoMonitoramento);
            int totalTrocasDeAlvo = trocasDeAlvoMonitoramento.Count;
            Log($"Monitoramento {monitoramentoCarga.CodigoMonitoramento} com {totalTrocasDeAlvo} trocas de alvo", 3);
            if (totalTrocasDeAlvo > 0)
            {
                // Verifica se há alguma troca de alvo elegível
                if (ExisteAlgumaTrocaDeAlvoElegivel(trocasDeAlvoMonitoramento))
                {
                    // Processa cada uma das trocas de alvo
                    for (int i = 0; i < totalTrocasDeAlvo; i++)
                    {
                        // Processa possível troca de alvo
                        ProcessaTrocaDeAlvo(unitOfWork, trocasDeAlvoMonitoramento[i], clientesFronteira, configuracao, tipoServicoMultisoftware, clienteMultisoftware, configuracaoMonitoramento);
                    }

                }

                // Verifica se há alguma troca de local elegível
                if (gerarEventoPermanenciaLocal && ExisteAlgumaTrocaDeLocalElegivel(trocasDeAlvoMonitoramento))
                {
                    // Processa cada uma das trocas de alvo
                    for (int i = 0; i < totalTrocasDeAlvo; i++)
                    {
                        // Processa possível troca de alvo
                        ProcessaTrocaDeLocal(unitOfWork, trocasDeAlvoMonitoramento[i], clientesFronteira, configuracao, tipoServicoMultisoftware, clienteMultisoftware);
                    }
                }
                else
                {
                    Log("Nenhuma troca de alvo elegivel", 4);
                }
            }
            Log("ProcessarTrocasDeAlvoMonitoramento", inicio, 3);
        }

        /**
         * Processa uma possível troca de alvo
         */
        private void ProcessaTrocaDeAlvo(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo trocaDeAlvo, Dominio.ObjetosDeValor.Cliente[] clientesFronteira, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento)
        {
            if (trocaDeAlvo.CodigoMonitoramento == 0 || trocaDeAlvo.CodigoCarga == 0 || trocaDeAlvo.CodigoPosicaoAtual == 0) return;

            Log($"ProcessandoTrocaDeAlvo {trocaDeAlvo.CodigoPosicaoAtual}", 4);

            DateTime inicio = DateTime.UtcNow, inicio1;
            Repositorio.Embarcador.Logistica.MonitoramentoSaidaAlvo repMonitoramentoSaidaAlvo = new Repositorio.Embarcador.Logistica.MonitoramentoSaidaAlvo(unitOfWork);

            // Código dos clientes das entregas
            string[] codigosClientesEntregas = (!string.IsNullOrEmpty(trocaDeAlvo.CodigosClientesEntregas)) ? trocaDeAlvo.CodigosClientesEntregas.Split(',') : null;

            string[] codigosClientesAlvosAtuais = null;
            string[] codigosSubareasAlvosAtuais = null;
            int total;

            // Entrada no raio/área de algum cliente
            if (trocaDeAlvo.EmAlvoPosicaoAtual ?? false)
            {

                // Entrada nos clientes
                if (!string.IsNullOrWhiteSpace(trocaDeAlvo.CodigosClientesAlvoPosicaoAtual))
                {
                    inicio1 = DateTime.UtcNow;

                    codigosClientesAlvosAtuais = trocaDeAlvo.CodigosClientesAlvoPosicaoAtual.Split(',');
                    total = codigosClientesAlvosAtuais.Length;
                    Log($"{total} alvos atuais", 6);
                    for (int i = 0; i < total; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(codigosClientesAlvosAtuais[i]))
                        {
                            double codigoClienteAlvoAtual = double.Parse(codigosClientesAlvosAtuais[i]);
                            //trocaDeAlvo.RealizarBaixaEntradaNoRaio = ObterRegraTipoOperacao(unitOfWork, trocaDeAlvo.CodigoCarga);

                            // É um dos clientes das entregas?
                            if (ClienteEstaNaLista(codigoClienteAlvoAtual, codigosClientesEntregas))
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = BuscarCargaEntregasCache(unitOfWork, trocaDeAlvo.CodigoCarga);
                                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = ObterEntregaParaOCliente(cargaEntregas, codigoClienteAlvoAtual);
                                if (cargaEntrega != null)
                                {
                                    // Consulta uma entrada no cliente que está em aberta (sem saída registrada)
                                    List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasCliente = BuscarPermanenciasClientesCache(unitOfWork, trocaDeAlvo.CodigoCarga);
                                    Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente permanenciaCliente = ObterPermanenciaNoClienteAberta(permanenciasCliente, codigoClienteAlvoAtual);

                                    // Se não há nenhuma permanência iniciada OU da data da posição for menor que a data registrada
                                    if (permanenciaCliente == null || !cargaEntrega.DataEntradaRaio.HasValue || trocaDeAlvo.DataVeiculoPosicaoAtual < cargaEntrega.DataEntradaRaio)
                                    {
                                        RegistrarEntradaNoAlvo(trocaDeAlvo, permanenciasCliente, cargaEntrega, codigoClienteAlvoAtual, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork, configuracaoMonitoramento);
                                    }
                                }
                            }
                            else
                            {
                                // ... ou é a origem(expedidor ou remetente) da carga?
                                double? codigoClienteOrigem = BuscarCodigoClienteOrigemDaCargaPeloPedido(unitOfWork, trocaDeAlvo.CodigoCarga);
                                if (codigoClienteOrigem != null && codigoClienteOrigem == codigoClienteAlvoAtual)
                                {
                                    RegistrarEntradaNoAlvo(trocaDeAlvo, null, null, codigoClienteAlvoAtual, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork, configuracaoMonitoramento);
                                }
                            }
                        }
                    }
                    Log("EntradaNoAlvo", inicio1, 5);
                }

                // Entrada nas subáreas dos clientes
                if (!string.IsNullOrWhiteSpace(trocaDeAlvo.CodigosSubareasAlvoPosicaoAtual))
                {
                    inicio1 = DateTime.UtcNow;
                    codigosSubareasAlvosAtuais = trocaDeAlvo.CodigosSubareasAlvoPosicaoAtual.Split(',');
                    total = codigosSubareasAlvosAtuais.Length;
                    for (int i = 0; i < total; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(codigosSubareasAlvosAtuais[i]))
                        {
                            string[] partes = codigosSubareasAlvosAtuais[i].Split('-');
                            int codigoSubareaAlvoAtual = int.Parse(partes[0]);
                            double codigoClienteSubareaAlvoAtual = double.Parse(partes[1]);

                            // É um dos clientes das entregas?
                            if (ClienteEstaNaLista(codigoClienteSubareaAlvoAtual, codigosClientesEntregas))
                            {
                                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = BuscarCargaEntregasCache(unitOfWork, trocaDeAlvo.CodigoCarga);
                                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = ObterEntregaParaOCliente(cargaEntregas, codigoClienteSubareaAlvoAtual);
                                if (cargaEntrega != null)
                                {

                                    // Consulta uma entrada na subárea que está em aberta (sem saída registrada)
                                    List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubarea = BuscarPermanenciasSubareasCache(unitOfWork, trocaDeAlvo.CodigoCarga);
                                    Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea permanenciaSubarea = ObterPermanenciaNaSubareaAberta(permanenciasSubarea, codigoSubareaAlvoAtual);

                                    // Se não há nenhuma permanência iniciada, cria uma nova
                                    if (permanenciaSubarea == null)
                                    {
                                        PosicaoEmAlvo posicaoNaSubarea = null;
                                        if (configuracao.TempoMinutosPermanenciaSubareaCliente > 0)
                                        {
                                            posicaoNaSubarea = BuscarPrimeiraPosicaoEntradaAlvo(0, codigoSubareaAlvoAtual, trocaDeAlvo.CodigoCarga, cargaEntrega.Codigo, trocaDeAlvo.CodigoVeiculo, trocaDeAlvo.DataInicioMonitoramento ?? trocaDeAlvo.DataCriacaoMonitoramento ?? DateTime.MinValue, trocaDeAlvo.DataVeiculoPosicaoAtual.Value, true, configuracao, unitOfWork);
                                        }
                                        if (posicaoNaSubarea == null)
                                        {
                                            posicaoNaSubarea = new PosicaoEmAlvo
                                            {
                                                Data = trocaDeAlvo.DataVeiculoPosicaoAtual.Value
                                            };
                                        }

                                        // Está na subárea pelo tempo mínimo configurado
                                        TimeSpan permacenciaNaSubarea = trocaDeAlvo.DataVeiculoPosicaoAtual.Value - posicaoNaSubarea.Data;
                                        int tempoPermanenciaSubareaClienteMinutos = (int)permacenciaNaSubarea.TotalMinutes;
                                        if (tempoPermanenciaSubareaClienteMinutos >= configuracao.TempoMinutosPermanenciaSubareaCliente)
                                        {
                                            EntradaNaSubarea(permanenciasSubarea, cargaEntrega, posicaoNaSubarea.Data, codigoSubareaAlvoAtual, unitOfWork);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Log("EntradaNaSubarea", inicio1, 5);
                }

            }

            // Saída do raio/área de algum cliente
            if (trocaDeAlvo.CodigoPosicaoAnterior > 0 && (trocaDeAlvo.EmAlvoPosicaoAnterior ?? false))
            {
                // Saídas dos clientes
                if (!string.IsNullOrWhiteSpace(trocaDeAlvo.CodigosClientesAlvoPosicaoAnterior))
                {
                    inicio1 = DateTime.UtcNow;
                    string[] codigosClientesAlvosAnterior = trocaDeAlvo.CodigosClientesAlvoPosicaoAnterior.Split(',');
                    total = codigosClientesAlvosAnterior.Length;
                    Log($"{total} alvos anteriores", 6);
                    for (int i = 0; i < total; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(codigosClientesAlvosAnterior[i]))
                        {
                            double codigoClienteAlvoAnterior = double.Parse(codigosClientesAlvosAnterior[i]);
                            if (SaiuDoAlvo(codigoClienteAlvoAnterior, codigosClientesAlvosAtuais))
                            {
                                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = null;
                                bool registrarSaida = false;

                                // É uma fronteira/alfândega?
                                if (ClienteEstaNaLista(codigoClienteAlvoAnterior, clientesFronteira))
                                {
                                    registrarSaida = true;
                                }
                                else
                                {
                                    // ... ou é uma das entregas?
                                    if (ClienteEstaNaLista(codigoClienteAlvoAnterior, codigosClientesEntregas))
                                    {
                                        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = BuscarCargaEntregasCache(unitOfWork, trocaDeAlvo.CodigoCarga);
                                        cargaEntrega = ObterEntregaParaOCliente(cargaEntregas, codigoClienteAlvoAnterior);
                                        if (cargaEntrega != null)
                                        {
                                            registrarSaida = true;
                                        }
                                    }
                                    else
                                    {
                                        // ... ou é a origem(expedidor ou remetente) da carga?
                                        double? codigoClienteOrigem = BuscarCodigoClienteOrigemDaCargaPeloPedido(unitOfWork, trocaDeAlvo.CodigoCarga);
                                        if (codigoClienteOrigem != null && codigoClienteOrigem == codigoClienteAlvoAnterior)
                                        {
                                            registrarSaida = true;
                                        }
                                    }
                                }

                                if (registrarSaida)
                                {

                                    // Há distância mínima configurada para confirmar a saída do alvo?
                                    if (configuracao.DistanciaMinimaPercorridaParaSaidaDoAlvo > 0)
                                    {
                                        Dominio.Entidades.Embarcador.Logistica.MonitoramentoSaidaAlvo monitoramentoSaidaAlvo = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoSaidaAlvo
                                        {
                                            Monitoramento = new Dominio.Entidades.Embarcador.Logistica.Monitoramento { Codigo = trocaDeAlvo.CodigoMonitoramento },
                                            Posicao = new Dominio.Entidades.Embarcador.Logistica.Posicao { Codigo = trocaDeAlvo.CodigoPosicaoAtual },
                                            Latitude = trocaDeAlvo.LatitudePosicaoAtual.HasValue ? trocaDeAlvo.LatitudePosicaoAtual.Value : 0,
                                            Longitude = trocaDeAlvo.LongitudePosicaoAtual.HasValue ? trocaDeAlvo.LongitudePosicaoAtual.Value : 0,
                                            Data = trocaDeAlvo.DataVeiculoPosicaoAtual.HasValue ? trocaDeAlvo.DataVeiculoPosicaoAtual.Value : DateTime.Now,
                                            CargaEntrega = cargaEntrega,
                                            Cliente = new Dominio.Entidades.Cliente { CPF_CNPJ = codigoClienteAlvoAnterior }
                                        };
                                        repMonitoramentoSaidaAlvo.Inserir(monitoramentoSaidaAlvo);
                                    }
                                    // ... sem distância mínima, processa a saída diretamente
                                    else
                                    {
                                        List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasCliente = BuscarPermanenciasClientesCache(unitOfWork, trocaDeAlvo.CodigoCarga);
                                        SaidaDoAlvo(permanenciasCliente, cargaEntrega, codigoClienteAlvoAnterior, trocaDeAlvo.DataVeiculoPosicaoAtual.Value, trocaDeAlvo.LatitudePosicaoAnterior.Value, trocaDeAlvo.LongitudePosicaoAnterior.Value, trocaDeAlvo.CodigoMonitoramento, trocaDeAlvo.CodigoCarga, trocaDeAlvo.NaoProcessarTrocaAlvoViaMonitoramentoTipoOperacao, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);
                                    }
                                }
                            }
                        }
                    }
                    Log("SaidaDosAlvos", inicio1, 5);
                }

                // Saídas das subáreas dos clientes
                if (!string.IsNullOrWhiteSpace(trocaDeAlvo.CodigosSubareasAlvoPosicaoAnterior))
                {
                    inicio1 = DateTime.UtcNow;
                    string[] codigosSubareasAlvosAnterior = trocaDeAlvo.CodigosSubareasAlvoPosicaoAnterior.Split(',');
                    total = codigosSubareasAlvosAnterior.Length;
                    int totalAlvoSubareasAtuais = codigosSubareasAlvosAtuais?.Length ?? 0;
                    for (int i = 0; i < total; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(codigosSubareasAlvosAnterior[i]))
                        {
                            string[] partes = codigosSubareasAlvosAnterior[i].Split('-');
                            int codigoSubareaAlvoAnterior = int.Parse(partes[0]);
                            double codigoClienteSubareaAlvoAnterior = double.Parse(partes[1]);
                            if (ClienteEstaNaLista(codigoClienteSubareaAlvoAnterior, codigosClientesEntregas))
                            {
                                bool saiuDaSubarea = true;
                                for (int j = 0; j < totalAlvoSubareasAtuais; j++)
                                {
                                    if (!string.IsNullOrWhiteSpace(codigosSubareasAlvosAtuais[j]))
                                    {
                                        partes = codigosSubareasAlvosAtuais[j].Split('-');
                                        int codigoSubareaAlvoAtual = int.Parse(partes[0]);
                                        if (codigoSubareaAlvoAnterior == codigoSubareaAlvoAtual)
                                        {
                                            saiuDaSubarea = false;
                                            break;
                                        }
                                    }
                                }
                                if (saiuDaSubarea)
                                {
                                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = BuscarCargaEntregasCache(unitOfWork, trocaDeAlvo.CodigoCarga);
                                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = ObterEntregaParaOCliente(cargaEntregas, codigoClienteSubareaAlvoAnterior);
                                    if (cargaEntrega != null)
                                    {
                                        List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubarea = BuscarPermanenciasSubareasCache(unitOfWork, trocaDeAlvo.CodigoCarga);
                                        SaidaDaSubarea(permanenciasSubarea, cargaEntrega, trocaDeAlvo.DataVeiculoPosicaoAtual.Value, codigoSubareaAlvoAnterior, unitOfWork);
                                    }
                                }
                            }
                        }
                    }
                    Log("SaidaDasSubareas", inicio1, 5);
                }
            }

            Log($"ProcessaTrocaDeAlvo {trocaDeAlvo.CodigoPosicaoAtual}", inicio, 4);

        }

        private void ProcessaTrocaDeLocal(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo trocaDeLocal, Dominio.ObjetosDeValor.Cliente[] clientesFronteira, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            if (trocaDeLocal.CodigoMonitoramento == 0 || trocaDeLocal.CodigoCarga == 0 || trocaDeLocal.CodigoPosicaoAtual == 0) return;

            Log($"ProcessandoTrocaDeLocal {trocaDeLocal.CodigoPosicaoAtual}", 4);

            DateTime inicio = DateTime.UtcNow, inicio1;

            // Código dos locais

            string[] codigosLocaisAtuais = null;
            int total;

            // Entrada em local
            if (trocaDeLocal.EmLocalPosicaoAtual ?? false)
            {
                // Entrada no local
                if (!string.IsNullOrWhiteSpace(trocaDeLocal.CodigosLocaisPosicaoAtual))
                {
                    inicio1 = DateTime.UtcNow;

                    codigosLocaisAtuais = trocaDeLocal.CodigosLocaisPosicaoAtual.Split(',');
                    total = codigosLocaisAtuais.Length;
                    Log($"{total} local atuais", 6);
                    for (int i = 0; i < total; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(codigosLocaisAtuais[i]))
                        {
                            int codigoLocalAtual = int.Parse(codigosLocaisAtuais[i]);

                            // Consulta local que está em aberto (sem saída registrada)
                            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal> permanenciasLocal = BuscarPermanenciasLocaisCache(unitOfWork, trocaDeLocal.CodigoCarga);
                            Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal permanenciaLocal = ObterPermanenciaNoLocalAberta(permanenciasLocal, codigoLocalAtual);

                            // Se não há nenhuma permanência iniciada OU da data da posição for menor que a data registrada
                            if (permanenciaLocal == null)
                                RegistrarEntradaNoLocal(trocaDeLocal, permanenciasLocal, codigoLocalAtual, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);

                        }
                    }
                    Log("EntradaNoLocal", inicio1, 5);
                }
            }


            if (trocaDeLocal.CodigoPosicaoAnterior > 0 && (trocaDeLocal.EmLocalPosicaoAnterior ?? false))
            {
                // Saídas do local
                if (!string.IsNullOrWhiteSpace(trocaDeLocal.CodigosLocaisPosicaoAnterior))
                {
                    inicio1 = DateTime.UtcNow;
                    string[] codigosLocaisPosicaoAnterior = trocaDeLocal.CodigosLocaisPosicaoAnterior.Split(',');
                    total = codigosLocaisPosicaoAnterior.Length;
                    Log($"{total} alvos anteriores", 6);
                    for (int i = 0; i < total; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(codigosLocaisPosicaoAnterior[i]))
                        {
                            int codigoLocalAnterior = int.Parse(codigosLocaisPosicaoAnterior[i]);
                            if (SaiuDoLocal(codigoLocalAnterior, codigosLocaisAtuais))
                            {
                                List<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal> permanenciasLocal = BuscarPermanenciasLocaisCache(unitOfWork, trocaDeLocal.CodigoCarga);
                                Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal permanenciaLocal = ObterPermanenciaNoLocalAberta(permanenciasLocal, codigoLocalAnterior);
                                if (permanenciaLocal != null)
                                {
                                    TimeSpan tempo = trocaDeLocal.DataVeiculoPosicaoAtual.Value - permanenciaLocal.DataInicio;
                                    permanenciaLocal.DataFim = trocaDeLocal.DataVeiculoPosicaoAtual.Value;
                                    permanenciaLocal.TempoSegundos = Absoluto(tempo.TotalSeconds);

                                    Repositorio.Embarcador.Logistica.PermanenciaLocal repPermanenciaLocal = new Repositorio.Embarcador.Logistica.PermanenciaLocal(unitOfWork);
                                    inicio = DateTime.UtcNow;
                                    repPermanenciaLocal.Atualizar(permanenciaLocal);
                                    Log("repPermanenciaCliente.Atualizar", inicio, 6);
                                }
                            }
                        }
                    }
                    Log("SaidaDoLocal", inicio1, 5);
                }
            }

            Log($"ProcessaTrocaDeLocal {trocaDeLocal.CodigoPosicaoAtual}", inicio, 4);

        }

        private void ProcessarPassagemFronteira(Repositorio.UnitOfWork unitOfWork, MonitoramentoCarga monitoramentoCarga, List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo> trocasDeAlvo, Dominio.ObjetosDeValor.Cliente[] clientesFronteira, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            if (monitoramentoCarga.CodigoMonitoramento == 0 || monitoramentoCarga.CodigoCarga == 0 || monitoramentoCarga.CodigoVeiculo == 0) return;

            DateTime inicio = DateTime.UtcNow, inicio1;

            // Extrair e processa as trocas de alvo do monitoramento
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo> trocasDeAlvoMonitoramento = ExtrairTrocasDeAlvoMonitoramento(trocasDeAlvo, monitoramentoCarga.CodigoMonitoramento);
            int totalTrocasDeAlvo = trocasDeAlvoMonitoramento.Count;
            Log($"Monitoramento {monitoramentoCarga.CodigoMonitoramento} com {totalTrocasDeAlvo} trocas de alvo", 3);
            if (totalTrocasDeAlvo > 0)
            {
                // Verifica se há alguma saída de alvo elegível
                if (ExisteAlgumaSaidaDeAlvoElegivel(trocasDeAlvoMonitoramento))
                {
                    Repositorio.Embarcador.Logistica.MonitoramentoSaidaAlvo repMonitoramentoSaidaAlvo = new Repositorio.Embarcador.Logistica.MonitoramentoSaidaAlvo(unitOfWork);
                    for (int i = 0; i < totalTrocasDeAlvo; i++)
                    {
                        if (trocasDeAlvoMonitoramento[i].CodigoPosicaoAnterior > 0 && (trocasDeAlvoMonitoramento[i].EmAlvoPosicaoAnterior ?? false) && !string.IsNullOrWhiteSpace(trocasDeAlvoMonitoramento[i].CodigosClientesAlvoPosicaoAnterior))
                        {
                            inicio1 = DateTime.UtcNow;

                            string[] codigosClientesAlvosAtuais = (!string.IsNullOrWhiteSpace(trocasDeAlvoMonitoramento[i].CodigosClientesAlvoPosicaoAtual)) ? trocasDeAlvoMonitoramento[i].CodigosClientesAlvoPosicaoAtual.Split(',') : null;
                            int totalAlvosAtuais = codigosClientesAlvosAtuais?.Length ?? 0;
                            Log($"{totalAlvosAtuais} alvos atuais", 6);

                            string[] codigosClientesAlvosAnterior = trocasDeAlvoMonitoramento[i].CodigosClientesAlvoPosicaoAnterior.Split(',');
                            int totalAlvosAnteriores = codigosClientesAlvosAnterior.Length;
                            Log($"{totalAlvosAnteriores} alvos anteriores", 6);

                            for (int j = 0; j < totalAlvosAnteriores; j++)
                            {
                                if (!string.IsNullOrWhiteSpace(codigosClientesAlvosAnterior[j]))
                                {
                                    double codigoClienteAlvoAnterior = double.Parse(codigosClientesAlvosAnterior[j]);
                                    if (ClienteEstaNaLista(codigoClienteAlvoAnterior, clientesFronteira) && SaiuDoAlvo(codigoClienteAlvoAnterior, codigosClientesAlvosAtuais))
                                    {
                                        if (configuracao.DistanciaMinimaPercorridaParaSaidaDoAlvo > 0)
                                        {
                                            Dominio.Entidades.Embarcador.Logistica.MonitoramentoSaidaAlvo monitoramentoSaidaAlvo = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoSaidaAlvo
                                            {
                                                Monitoramento = new Dominio.Entidades.Embarcador.Logistica.Monitoramento { Codigo = trocasDeAlvoMonitoramento[i].CodigoMonitoramento },
                                                Posicao = new Dominio.Entidades.Embarcador.Logistica.Posicao { Codigo = trocasDeAlvoMonitoramento[i].CodigoPosicaoAtual },
                                                Latitude = trocasDeAlvoMonitoramento[i].LatitudePosicaoAtual.HasValue ? trocasDeAlvoMonitoramento[i].LatitudePosicaoAtual.Value : 0,
                                                Longitude = trocasDeAlvoMonitoramento[i].LongitudePosicaoAtual.HasValue ? trocasDeAlvoMonitoramento[i].LongitudePosicaoAtual.Value : 0,
                                                Data = trocasDeAlvoMonitoramento[i].DataVeiculoPosicaoAtual.HasValue ? trocasDeAlvoMonitoramento[i].DataVeiculoPosicaoAtual.Value : DateTime.Now,
                                                Cliente = new Dominio.Entidades.Cliente { CPF_CNPJ = codigoClienteAlvoAnterior }
                                            };
                                            repMonitoramentoSaidaAlvo.Inserir(monitoramentoSaidaAlvo);
                                        }
                                        else
                                        {
                                            SaidaDaFronteira(codigoClienteAlvoAnterior, trocasDeAlvoMonitoramento[i].DataVeiculoPosicaoAtual.Value, trocasDeAlvoMonitoramento[i].CodigoCarga, trocasDeAlvoMonitoramento[i].CodigoVeiculo, trocasDeAlvoMonitoramento[i].DataInicioMonitoramento ?? trocasDeAlvoMonitoramento[i].DataCriacaoMonitoramento ?? DateTime.MinValue, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);
                                        }
                                    }
                                }
                            }
                            Log("SaidaDosAlvos", inicio1, 5);
                        }
                    }
                }
                else
                {
                    Log("Nenhuma troca de alvo elegivel", 4);
                }
            }
            Log("ProcessarPassagensFronteiras", inicio, 3);

        }

        private List<int> ProcessarSaidasDeAlvoMonitoramento(Repositorio.UnitOfWork unitOfWork, MonitoramentoCarga monitoramentoCarga, List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoSaidaDeAlvo> saidasDeAlvo, Dominio.ObjetosDeValor.Cliente[] clientesFronteira, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            List<int> codigosSaidasAlvoExcluir = new List<int>();
            if (monitoramentoCarga.CodigoMonitoramento == 0 || monitoramentoCarga.CodigoCarga == 0 || monitoramentoCarga.CodigoVeiculo == 0) return codigosSaidasAlvoExcluir;

            DateTime inicio, inicio1;

            // Localiza as saídas de alvo deste monitoramento
            inicio = DateTime.UtcNow;
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoSaidaDeAlvo> saidasDeAlvoMonitoramento = ExtrairSaidasDeAlvoMonitoramento(saidasDeAlvo, monitoramentoCarga.CodigoMonitoramento);
            int totalSaidasDeAlvo = saidasDeAlvoMonitoramento.Count;
            Log($"Monitoramento {monitoramentoCarga.CodigoMonitoramento} com {totalSaidasDeAlvo} saidas de alvo", inicio, 3);
            if (totalSaidasDeAlvo > 0)
            {
                inicio = DateTime.UtcNow;
                for (int i = 0; i < totalSaidasDeAlvo; i++)
                {
                    if (monitoramentoCarga.CodigoMonitoramento > 0 && saidasDeAlvoMonitoramento[i].CodigoPosicao > 0 && saidasDeAlvoMonitoramento[i].CodigoCliente > 0 && saidasDeAlvoMonitoramento[i].CodigoMonitoramento > 0)
                    {
                        // Apenas quando a posição for diferente da última posição do monitoramento
                        if (saidasDeAlvoMonitoramento[i].CodigoUltimaPosicaoMonitoramento != saidasDeAlvoMonitoramento[i].CodigoPosicao)
                        {

                            // Código dos clientes das entregas
                            string[] codigosClientesEntregas = (!string.IsNullOrEmpty(saidasDeAlvoMonitoramento[i].CodigosClientesEntregas)) ? saidasDeAlvoMonitoramento[i].CodigosClientesEntregas.Split(',') : null;

                            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = null;
                            bool registrarSaida = false;

                            // É uma saída de fronteira/alfândega?
                            if (ClienteEstaNaLista(saidasDeAlvoMonitoramento[i].CodigoCliente, clientesFronteira))
                            {
                                SaidaDaFronteira(saidasDeAlvoMonitoramento[i].CodigoCliente, saidasDeAlvoMonitoramento[i].DataVeiculoPosicao, monitoramentoCarga.CodigoCarga, monitoramentoCarga.CodigoVeiculo, monitoramentoCarga.DataInicio ?? monitoramentoCarga.DataFim ?? DateTime.MinValue, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);
                                codigosSaidasAlvoExcluir.Add(saidasDeAlvoMonitoramento[i].CodigoSaidaAlvo);
                            }
                            else
                            {
                                // ... ou é uma das entregas?
                                if (ClienteEstaNaLista(saidasDeAlvoMonitoramento[i].CodigoCliente, codigosClientesEntregas))
                                {
                                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = BuscarCargaEntregasCache(unitOfWork, monitoramentoCarga.CodigoCarga);
                                    cargaEntrega = ObterEntregaParaOCliente(cargaEntregas, saidasDeAlvoMonitoramento[i].CodigoCliente);
                                    if (cargaEntrega != null)
                                    {
                                        registrarSaida = true;
                                    }
                                }
                                else
                                {
                                    // ... ou é a origem (expedidor ou remetente) da carga?
                                    double? codigoClienteOrigem = BuscarCodigoClienteOrigemDaCargaPeloPedido(unitOfWork, monitoramentoCarga.CodigoCarga);
                                    if (codigoClienteOrigem != null && codigoClienteOrigem == saidasDeAlvoMonitoramento[i].CodigoCliente)
                                    {
                                        registrarSaida = true;
                                    }
                                }
                            }

                            if (registrarSaida)
                            {
                                // Consulta as posições do veículo a partir da saída do alvo até a última posição
                                inicio1 = DateTime.UtcNow;
                                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = BuscarPosicoesVeiculo(unitOfWork, monitoramentoCarga.CodigoVeiculo, saidasDeAlvoMonitoramento[i].DataVeiculoPosicao, monitoramentoCarga.DataFim ?? this.dataAtual);
                                Log($"BuscarPosicoesVeiculo {posicoes.Count}", inicio1, 4);
                                if (posicoes.Count >= 2)
                                {
                                    // Verifica a distância da rota realizada, confirmando que percorreu a distância mínima
                                    inicio1 = DateTime.UtcNow;
                                    Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao resposta = Servicos.Embarcador.Logistica.Monitoramento.ControleDistancia.ObterRespostaRoteirizacao(posicoes, configuracaoIntegracao.ServidorRouteOSM, false);
                                    if (resposta != null)
                                    {
                                        int distanciaPercorridaKm = (int)resposta.Distancia;
                                        Log($"Roteirizacao {distanciaPercorridaKm}km", inicio1, 5);
                                        if (distanciaPercorridaKm >= configuracao.DistanciaMinimaPercorridaParaSaidaDoAlvo)
                                        {

                                            // Encontra todas as permanências em cliente da carga
                                            inicio1 = DateTime.UtcNow;
                                            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasCliente = BuscarPermanenciasClientesCache(unitOfWork, monitoramentoCarga.CodigoCarga);
                                            Log($"BuscarPermanenciasClientesCache {permanenciasCliente.Count}", inicio1, 5);

                                            SaidaDoAlvo(permanenciasCliente, cargaEntrega, saidasDeAlvoMonitoramento[i].CodigoCliente, saidasDeAlvoMonitoramento[i].DataVeiculoPosicao, saidasDeAlvoMonitoramento[i].LatitudePosicao, saidasDeAlvoMonitoramento[i].LongitudePosicao, saidasDeAlvoMonitoramento[i].CodigoMonitoramento, monitoramentoCarga.CodigoCarga, saidasDeAlvoMonitoramento[i].NaoProcessarTrocaAlvoViaMonitoramentoTipoOperacao, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);

                                            codigosSaidasAlvoExcluir.Add(saidasDeAlvoMonitoramento[i].CodigoSaidaAlvo);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                codigosSaidasAlvoExcluir.Add(saidasDeAlvoMonitoramento[i].CodigoSaidaAlvo);
                            }
                        }
                    }
                    else
                    {
                        codigosSaidasAlvoExcluir.Add(saidasDeAlvoMonitoramento[i].CodigoSaidaAlvo);
                    }
                }
                Log("ProcessarSaidasDeAlvoMonitoramento", inicio, 3);
            }
            return codigosSaidasAlvoExcluir;
        }

        private void RegistrarEntradaNoAlvo(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo trocaDeAlvo, List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasCliente, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, double codigoCliente, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento)
        {
            PosicaoEmAlvo posicaoEmAlvo = IdentificarPrimeiraPosicaoEntradaAlvo(codigoCliente, trocaDeAlvo, configuracao, unitOfWork);

            // Está no alvo pelo tempo mínimo configurado
            TimeSpan permacenciaNoCliente = trocaDeAlvo.DataVeiculoPosicaoAtual.Value - posicaoEmAlvo.Data;
            int tempoPermanenciaClienteMinutos = (int)permacenciaNoCliente.TotalMinutes;
            //#76419 ... ou está configurado para registrar entrada com posição única no alvo e ignorar tempo de permanência.
            if ((configuracaoMonitoramento?.IdentificarEntradaEmAlvoComPosicaoUnicaIgnorandoTemposDePermanencia ?? false) || tempoPermanenciaClienteMinutos >= configuracao.TempoMinutosPermanenciaCliente)
            {

                if (permanenciasCliente == null && cargaEntrega == null) // é a origem(expedidor ou remetente)
                    VerificarFinalizaMonitoramentoRetornoOrigem(posicaoEmAlvo.Data, trocaDeAlvo.CodigoMonitoramento, trocaDeAlvo.CodigoCarga, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);


                EntradaNoAlvo(permanenciasCliente, cargaEntrega, codigoCliente, posicaoEmAlvo.Data, posicaoEmAlvo.Latitude, posicaoEmAlvo.Longitude, trocaDeAlvo.CodigoMonitoramento, trocaDeAlvo.CodigoCarga, trocaDeAlvo.CodigoVeiculo, trocaDeAlvo.NaoProcessarTrocaAlvoViaMonitoramentoTipoOperacao, trocaDeAlvo.RealizarBaixaEntradaNoRaio, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);
            }

        }

        private void RegistrarEntradaNoLocal(Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo trocaDeLocal, List<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal> permanenciasLocal, int codigoLocal, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            PosicaoEmAlvo posicaoEmLocal = IdentificarPrimeiraPosicaoEntradaLocal(codigoLocal, trocaDeLocal, configuracao, unitOfWork);
            EntradaNoLocal(permanenciasLocal, codigoLocal, posicaoEmLocal.Data, posicaoEmLocal.Latitude, posicaoEmLocal.Longitude, trocaDeLocal.CodigoMonitoramento, trocaDeLocal.CodigoCarga, trocaDeLocal.CodigoVeiculo, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);
        }

        private void VerificarFinalizaMonitoramentoRetornoOrigem(DateTime data, int codigoMonitoramento, int codigoCarga, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (codigoMonitoramento > 0 && codigoCarga > 0)
            {
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigoMonitoramento);

                if (monitoramento != null && monitoramento.Carga != null && monitoramento.Carga.DataInicioViagem.HasValue && monitoramento.StatusViagem != null && monitoramento.StatusViagem.TipoRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.Retornando)
                {
                    DateTime inicio;
                    inicio = DateTime.UtcNow;
                    Log("Finalizar Viagem ao chegar na origem em retorno", inicio, 6);

                    string mensagemAuditoria = "Finalizar Viagem ao chegar na origem em retorno";
                    Servicos.Embarcador.Monitoramento.Carga.FinalizarViagem(monitoramento, data, new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado(), mensagemAuditoria, tipoServicoMultisoftware, clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.MonitoramentoAutomaticamente, unitOfWork);

                }

            }
        }


        private void EntradaNoAlvo(List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasCliente, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, double codigoCliente, DateTime data, double latitude, double longitude, int codigoMonitoramento, int codigoCarga, int codigoVeiculo, bool naoProcessarTrocaAlvoViaMonitoramentoTipoOperacao, bool FinalizarEntregaEntradaRaioTipoOperacao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (data != null && codigoMonitoramento > 0)
            {
                DateTime inicio;
                if (cargaEntrega != null)
                {
                    if (DeveProcessarTrocaDeAlvoEntrega(cargaEntrega, configuracao, naoProcessarTrocaAlvoViaMonitoramentoTipoOperacao, unitOfWork))
                    {
                        if (cargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.NaoEntregue || (cargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado && cargaEntrega.PermitirEntregarMaisTarde))
                        {
                            inicio = DateTime.UtcNow;
                            Servicos.Embarcador.Monitoramento.Entrega.IniciarEntrega(cargaEntrega, codigoMonitoramento, data, latitude, longitude, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);
                            Log("IniciarEntrega", inicio, 6);
                        }
                        // ... se já já existe data de início de entrega mas recebeu uma posição fora de ordem que aconteceu antes do registrado anteriormente
                        else if (configuracao.MonitoramentoConsiderarPosicaoTardiaParaAtualizarInicioFimEntregaViagem && cargaEntrega.DataInicio.HasValue && data < cargaEntrega.DataInicio)
                        {
                            inicio = DateTime.UtcNow;
                            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarInicioEntrega(cargaEntrega, data, latitude, longitude, unitOfWork);
                            Log("AtualizarInicioEntrega", inicio, 6);
                        }
                    }

                    if (!cargaEntrega.DataEntradaRaio.HasValue || (cargaEntrega.DataEntradaRaio.HasValue && data < cargaEntrega.DataEntradaRaio))
                    {
                        inicio = DateTime.UtcNow;
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarEntradaNoRaio(cargaEntrega, data, unitOfWork);
                        Log("AtualizarEntradaNoRaio", inicio, 6);
                    }

                    // Consulta uma entrada no cliente que está em aberta (sem saída registrada) para, se não existir permanência iniciada, cria uma nova
                    Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente permanenciaCliente = ObterPermanenciaNoClienteAberta(permanenciasCliente, codigoCliente);
                    if (permanenciaCliente == null)
                    {
                        permanenciaCliente = new Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente();
                        permanenciaCliente.CargaEntrega = cargaEntrega;
                        permanenciaCliente.Cliente = cargaEntrega.Cliente;
                        permanenciaCliente.DataInicio = data;

                        Repositorio.Embarcador.Logistica.PermanenciaCliente repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(unitOfWork);

                        inicio = DateTime.UtcNow;
                        repPermanenciaCliente.Inserir(permanenciaCliente);
                        Log("repPermanenciaCliente.Inserir", inicio, 6);

                        permanenciasCliente.Add(permanenciaCliente);
                    }

                    GerarEventoChegadaNoRaio(cargaEntrega, codigoCarga, codigoVeiculo, latitude, longitude, data, unitOfWork);
                    GerarEventoChegadaNoRaioEntrega(cargaEntrega, codigoCarga, codigoVeiculo, latitude, longitude, data, unitOfWork);
                    GerarEventoAtrasoDescargaNaEntradaRaioEntrega(cargaEntrega, codigoCarga, codigoVeiculo, latitude, longitude, data, unitOfWork);

                    if (configuracao?.UtilizaAppTrizy ?? false)
                    {
                        Servicos.Embarcador.Monitoramento.Monitoramento servMonitoramento = new Servicos.Embarcador.Monitoramento.Monitoramento();
                        servMonitoramento.RegistrarPosicaoEventosRelevantesTrizy(codigoCarga, data, latitude, longitude, EventoRelevanteMonitoramento.ChegadaRaio, unitOfWork);
                    }

                    if (FinalizarEntregaEntradaRaioTipoOperacao)
                        Servicos.Embarcador.Monitoramento.Entrega.Finalizar(cargaEntrega, codigoMonitoramento, data, latitude, longitude, codigoCliente, configuracao, tipoServicoMultisoftware, unitOfWork);

                }
                else if (DeveProcessarTrocaDeAlvoEntrega(null, configuracao, naoProcessarTrocaAlvoViaMonitoramentoTipoOperacao, unitOfWork) && configuracao.QuandoIniciarViagemViaMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarViagemViaMonitoramento.AoChegarNaOrigem)
                {
                    inicio = DateTime.UtcNow;
                    Servicos.Embarcador.Monitoramento.Carga.IniciarViagem(codigoMonitoramento, data, latitude, longitude, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);
                    Log("IniciarViagem", inicio, 6);
                }
            }
        }

        private void EntradaNoLocal(List<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal> permanenciasLocal, int codigoLocal, DateTime data, double latitude, double longitude, int codigoMonitoramento, int codigoCarga, int codigoVeiculo, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (data != DateTime.MinValue && codigoMonitoramento > 0)
            {
                DateTime inicio;

                Repositorio.Embarcador.Logistica.Locais repLocais = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                // Consulta uma entrada no cliente que está em aberta (sem saída registrada) para, se não existir permanência iniciada, cria uma nova
                Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal permanenciaLocal = ObterPermanenciaNoLocalAberta(permanenciasLocal, codigoLocal);
                if (permanenciaLocal == null)
                {
                    var carga = repCarga.BuscarPorCodigo(codigoCarga);
                    if (carga == null)
                        return;

                    permanenciaLocal = new Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal();
                    permanenciaLocal.Carga = carga;
                    permanenciaLocal.DataInicio = data;
                    permanenciaLocal.Local = repLocais.BuscarPorCodigo(codigoLocal);


                    Repositorio.Embarcador.Logistica.PermanenciaLocal repPermanenciaLocal = new Repositorio.Embarcador.Logistica.PermanenciaLocal(unitOfWork);

                    inicio = DateTime.UtcNow;
                    repPermanenciaLocal.Inserir(permanenciaLocal);
                    Log("repPermanenciaLocal.Inserir", inicio, 6);

                    permanenciasLocal.Add(permanenciaLocal);
                }
            }
        }

        private void SaidaDoAlvo(List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasCliente, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, double codigoCliente, DateTime data, double latitude, double longitude, int codigoMonitoramento, int codigoCarga, bool naoProcessarTrocaAlvoViaMonitoramentoTipoOperacao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (data != null && codigoMonitoramento > 0 && codigoCarga > 0)
            {
                DateTime inicio;
                // Saída de um dos cientes da entrega
                if (cargaEntrega != null)
                {
                    if (DeveProcessarTrocaDeAlvoEntrega(cargaEntrega, configuracao, naoProcessarTrocaAlvoViaMonitoramentoTipoOperacao, unitOfWork))
                    {
                        if (cargaEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmCliente)
                        {

                            // Quando há controle de coleta a origem da carga é o primeiro registro do controle de entrega
                            if (cargaEntrega.Ordem == 0 && cargaEntrega.Coleta && configuracao.QuandoIniciarViagemViaMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarViagemViaMonitoramento.AoSairDaOrigem)
                            {
                                inicio = DateTime.UtcNow;
                                Servicos.Embarcador.Monitoramento.Carga.IniciarViagem(codigoMonitoramento, data, latitude, longitude, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);
                                Log("IniciarViagem", inicio, 6);
                            }
                            inicio = DateTime.UtcNow;
                            Servicos.Embarcador.Monitoramento.Entrega.Finalizar(cargaEntrega, codigoMonitoramento, data, latitude, longitude, codigoCliente, configuracao, tipoServicoMultisoftware, unitOfWork);
                            Log("FinalizarEntrega", inicio, 6);
                        }

                        // ... se já já existe data de entrega mas recebeu uma posição fora de ordem que aconteceu antes do registrado anteriormente
                        else if (configuracao.MonitoramentoConsiderarPosicaoTardiaParaAtualizarInicioFimEntregaViagem && cargaEntrega.DataFim.HasValue && data < cargaEntrega.DataFim)
                        {
                            inicio = DateTime.UtcNow;
                            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarFinalizacaoEntrega(cargaEntrega, data, latitude, longitude, unitOfWork);
                            Log("AtualizarFinalizacaoEntrega", inicio, 6);

                        }
                    }

                    if (!cargaEntrega.DataSaidaRaio.HasValue || (cargaEntrega.DataSaidaRaio.HasValue && data < cargaEntrega.DataSaidaRaio))
                    {
                        inicio = DateTime.UtcNow;
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarSaidaDoRaio(cargaEntrega, data, unitOfWork);
                        Log("AtualizarSaidaDoRaio", inicio, 6);
                    }
                    DateTime? dataEntrada = cargaEntrega.DataEntradaRaio;

                    // Registra a saída do alvo
                    Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente permanenciaCliente = ObterPermanenciaNoClienteAberta(permanenciasCliente, codigoCliente);
                    if (permanenciaCliente != null)
                    {
                        TimeSpan tempo = data - permanenciaCliente.DataInicio;
                        permanenciaCliente.DataFim = data;
                        permanenciaCliente.TempoSegundos = Absoluto(tempo.TotalSeconds);

                        Repositorio.Embarcador.Logistica.PermanenciaCliente repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(unitOfWork);
                        inicio = DateTime.UtcNow;
                        repPermanenciaCliente.Atualizar(permanenciaCliente);
                        Log("repPermanenciaCliente.Atualizar", inicio, 6);

                        if (dataEntrada == null) dataEntrada = permanenciaCliente.DataInicio;

                        if (configuracao?.UtilizaAppTrizy ?? false)
                        {
                            Servicos.Embarcador.Monitoramento.Monitoramento servMonitoramento = new Servicos.Embarcador.Monitoramento.Monitoramento();
                            servMonitoramento.RegistrarPosicaoEventosRelevantesTrizy(cargaEntrega.Carga.Codigo, data, latitude, longitude, EventoRelevanteMonitoramento.SaidaRaio, unitOfWork);
                        }
                    }
                    if (dataEntrada == null) dataEntrada = data;

                    try
                    {
                        inicio = DateTime.UtcNow;
                        Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo serOcorrenciaAutomaticaPorPeriodo = new Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo(unitOfWork);

                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoGatilhoTracking tipoAplicacaoGatilhoTracking = cargaEntrega.Coleta ? TipoAplicacaoGatilhoTracking.Coleta : TipoAplicacaoGatilhoTracking.Entrega;

                        if (cargaEntrega.Fronteira)
                            serOcorrenciaAutomaticaPorPeriodo.GerarOcorrenciaPorTracking(cargaEntrega.Carga, cargaEntrega.Cliente?.CPF_CNPJ ?? 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.GatilhoFinalTraking.SaidaFronteira, dataEntrada.Value, data, tipoServicoMultisoftware, clienteMultisoftware, tipoAplicacaoGatilhoTracking);
                        else if (cargaEntrega.Parqueamento)
                            serOcorrenciaAutomaticaPorPeriodo.GerarOcorrenciaPorTracking(cargaEntrega.Carga, cargaEntrega.Cliente?.CPF_CNPJ ?? 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.GatilhoFinalTraking.SaidaParqueamento, dataEntrada.Value, data, tipoServicoMultisoftware, clienteMultisoftware, tipoAplicacaoGatilhoTracking);
                        else
                            serOcorrenciaAutomaticaPorPeriodo.GerarOcorrenciaPorTracking(cargaEntrega.Carga, (cargaEntrega.Fronteira ? cargaEntrega.Cliente?.CPF_CNPJ ?? 0 : 0), Dominio.ObjetosDeValor.Embarcador.Enumeradores.GatilhoFinalTraking.SaidaCliente, dataEntrada.Value, data, tipoServicoMultisoftware, clienteMultisoftware, tipoAplicacaoGatilhoTracking);

                        Log("OcorrenciaAutomaticaPorPeriodo.GerarOcorrenciaPorTracking", inicio, 6);
                    }
                    catch (Exception e)
                    {
                        Log("OcorrenciaAutomaticaPorPeriodo.GerarOcorrenciaPorTracking Exception: " + e.Message, 7);
                    }
                }
                else if (DeveProcessarTrocaDeAlvo(configuracao, naoProcessarTrocaAlvoViaMonitoramentoTipoOperacao))
                {
                    // Saída da origem da carga
                    double? codigoClienteOrigem = BuscarCodigoClienteOrigemDaCargaPeloPedido(unitOfWork, codigoCarga);
                    if (codigoClienteOrigem != null && codigoClienteOrigem == codigoCliente && configuracao.QuandoIniciarViagemViaMonitoramento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoIniciarViagemViaMonitoramento.AoSairDaOrigem)
                    {
                        inicio = DateTime.UtcNow;
                        Servicos.Embarcador.Monitoramento.Carga.IniciarViagem(codigoMonitoramento, data, latitude, longitude, configuracao, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);
                        Log("IniciarViagem", inicio, 6);
                    }
                }
            }
        }

        private void EntradaNaSubarea(List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubarea, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime data, int codigoSubarea, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaEntrega != null && data != null && codigoSubarea > 0)
            {
                Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea permanenciaSubarea = new Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea();
                permanenciaSubarea.CargaEntrega = cargaEntrega;
                permanenciaSubarea.Subarea = new Dominio.Entidades.Embarcador.Logistica.SubareaCliente { Codigo = codigoSubarea };
                permanenciaSubarea.DataInicio = data;

                Repositorio.Embarcador.Logistica.PermanenciaSubarea repoPermanenciaSubarea = new Repositorio.Embarcador.Logistica.PermanenciaSubarea(unitOfWork);
                repoPermanenciaSubarea.Inserir(permanenciaSubarea);
                permanenciasSubarea.Add(permanenciaSubarea);

                ProcessarAcoesFluxoDePatioSubarea(permanenciaSubarea, MonitoramentoEventoData.EntradaCliente, unitOfWork);
            }
        }

        private void SaidaDaSubarea(List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubarea, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime data, int codigoSubarea, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaEntrega != null && data != null && codigoSubarea > 0)
            {
                // Consulta uma entrada na subárea que está em aberta (sem saída registrada)
                Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea permanenciaSubarea = ObterPermanenciaNaSubareaAberta(permanenciasSubarea, codigoSubarea);

                // Se há nenhuma permanência iniciada, registra a saída
                if (permanenciaSubarea != null)
                {
                    permanenciaSubarea.DataFim = data;
                    TimeSpan tempo = permanenciaSubarea.DataFim.Value - permanenciaSubarea.DataInicio;
                    permanenciaSubarea.TempoSegundos = Absoluto(tempo.TotalSeconds);

                    Repositorio.Embarcador.Logistica.PermanenciaSubarea repoPermanenciaSubarea = new Repositorio.Embarcador.Logistica.PermanenciaSubarea(unitOfWork);
                    repoPermanenciaSubarea.Atualizar(permanenciaSubarea);

                    ProcessarAcoesFluxoDePatioSubarea(permanenciaSubarea, MonitoramentoEventoData.SaidaCliente, unitOfWork);
                }
            }
        }

        private void SaidaDaFronteira(double codigoClienge, DateTime dataSaida, int codigoCarga, int codigoVeiculo, DateTime dataInicioMonitoramento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (configuracao.MonitorarPassagensFronteiras && codigoClienge > 0 && codigoCarga > 0 && dataSaida > DateTime.MinValue)
            {
                PosicaoEmAlvo posicaoEmtradaNoAlvo = BuscarPrimeiraPosicaoEntradaAlvo(codigoClienge, 0, codigoCarga, 0, codigoVeiculo, dataInicioMonitoramento, dataSaida, false, configuracao, unitOfWork);
                DateTime dataEntrada = posicaoEmtradaNoAlvo?.Data ?? dataSaida;

                int minutosNaFronteira = (int)(dataSaida - dataEntrada).TotalMinutes;
                if (minutosNaFronteira > configuracao.TempoMinutosPermanenciaCliente)
                {
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                    if (carga != null)
                    {
                        DateTime inicio = DateTime.UtcNow;
                        Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo serOcorrenciaAutomaticaPorPeriodo = new Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo(unitOfWork);
                        serOcorrenciaAutomaticaPorPeriodo.GerarOcorrenciaPorTracking(carga, codigoClienge, Dominio.ObjetosDeValor.Embarcador.Enumeradores.GatilhoFinalTraking.SaidaFronteira, dataEntrada, dataSaida, tipoServicoMultisoftware, clienteMultisoftware, TipoAplicacaoGatilhoTracking.AplicarSempre);
                        Log("SaidaDaFronteira", inicio, 6);
                    }
                }
            }
        }

        private double? BuscarCodigoClienteOrigemDaCargaPeloPedido(Repositorio.UnitOfWork unitOfWork, int codigoCarga)
        {
            if (codigoCarga > 0)
            {
                int total = clienteOrigemCargaCache.Count;
                for (int i = 0; i < total; i++)
                {
                    if (clienteOrigemCargaCache[i].CodigoCarga == codigoCarga)
                    {
                        return clienteOrigemCargaCache[i].CodigoCliente;
                    }
                }

                // Se não encontrar, consulta e adiciona ao cache
                DateTime inicio = DateTime.UtcNow;
                double? codigoCliente = Servicos.Embarcador.Monitoramento.Monitoramento.BuscarCodigoClienteOrigemDaCargaPeloPedido(unitOfWork, codigoCarga);
                Log("BuscarCodigoClienteOrigemDaCargaPeloPedido", inicio, 5);
                clienteOrigemCargaCache.Add(new ClienteOrigemCarga
                {
                    CodigoCarga = codigoCarga,
                    CodigoCliente = codigoCliente
                });
                return codigoCliente;
            }
            return null;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> BuscarCargaEntregasCache(Repositorio.UnitOfWork unitOfWork, int codigoCarga)
        {
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = null;
            if (codigoCarga > 0)
            {
                // Localiza as entregas no cache
                //int total = cargaEntregasCargaCache.Count;
                //for (int i = 0; i < total; i++)
                //{
                //    if (cargaEntregasCargaCache[i].CodigoCarga == codigoCarga)
                //    {
                //        return cargaEntregasCargaCache[i].CargaEntregas;
                //    }
                //}

                // Se não encontrar, consulta e adiciona ao cache
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                DateTime inicio = DateTime.UtcNow;
                cargaEntregas = repCargaEntrega.BuscarPorCarga(codigoCarga);
                Log("repCargaEntrega.BuscarPorCarga", inicio, 5);
                cargaEntregasCargaCache.Add(new CargaEntregasCarga
                {
                    CodigoCarga = codigoCarga,
                    CargaEntregas = cargaEntregas
                });

            }
            return cargaEntregas;
        }

        private Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarCargaEntregaCache(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas, int codigoCargaEntrega)
        {
            if (cargaEntregas != null && codigoCargaEntrega > 0)
            {
                int total = cargaEntregas.Count;
                for (int i = 0; i < total; i++)
                {
                    if (cargaEntregas[i].Codigo == codigoCargaEntrega)
                    {
                        return cargaEntregas[i];
                    }
                }
            }
            return null;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> BuscarPermanenciasClientesCache(Repositorio.UnitOfWork unitOfWork, int codigoCarga)
        {
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasCliente = null;
            if (codigoCarga > 0)
            {
                // Localiza as permanencias no cache
                int total = permanenciasClientesCargaCache.Count;
                for (int i = 0; i < total; i++)
                {
                    if (permanenciasClientesCargaCache[i].CodigoCarga == codigoCarga)
                    {
                        permanenciasCliente = permanenciasClientesCargaCache[i].PermanenciaClientes;
                        break;
                    }
                }

                // Se não encontrar, consulta e adiciona ao cache
                if (permanenciasCliente == null)
                {
                    Repositorio.Embarcador.Logistica.PermanenciaCliente repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(unitOfWork);
                    permanenciasCliente = repPermanenciaCliente.BuscarPorCarga(codigoCarga);
                    permanenciasClientesCargaCache.Add(new PermanenciasClientesCarga
                    {
                        CodigoCarga = codigoCarga,
                        PermanenciaClientes = permanenciasCliente
                    });
                }
            }
            return permanenciasCliente;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> BuscarPermanenciasSubareasCache(Repositorio.UnitOfWork unitOfWork, int codigoCarga)
        {
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubarea = null;
            if (codigoCarga > 0)
            {
                // Localiza as permanencias no cache
                int total = permanenciasSubareasCargaCache.Count;
                for (int i = 0; i < total; i++)
                {
                    if (permanenciasSubareasCargaCache[i].CodigoCarga == codigoCarga)
                    {
                        permanenciasSubarea = permanenciasSubareasCargaCache[i].PermanenciaSubareas;
                        break;
                    }
                }

                // Se não encontrar, consulta e adiciona ao cache
                if (permanenciasSubarea == null)
                {
                    Repositorio.Embarcador.Logistica.PermanenciaSubarea repPermanenciaSubarea = new Repositorio.Embarcador.Logistica.PermanenciaSubarea(unitOfWork);
                    permanenciasSubarea = repPermanenciaSubarea.BuscarPorCarga(codigoCarga);
                    permanenciasSubareasCargaCache.Add(new PermanenciasSubareasCarga
                    {
                        CodigoCarga = codigoCarga,
                        PermanenciaSubareas = permanenciasSubarea
                    });
                }
            }
            return permanenciasSubarea;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal> BuscarPermanenciasLocaisCache(Repositorio.UnitOfWork unitOfWork, int codigoCarga)
        {
            List<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal> permanenciasLocais = null;
            if (codigoCarga > 0)
            {
                // Localiza as permanencias no cache
                int total = permanenciasLocaisCargaCache.Count;
                for (int i = 0; i < total; i++)
                {
                    if (permanenciasLocaisCargaCache[i].CodigoCarga == codigoCarga)
                    {
                        permanenciasLocais = permanenciasLocaisCargaCache[i].PermanenciaLocais;
                        break;
                    }
                }

                // Se não encontrar, consulta e adiciona ao cache
                if (permanenciasLocais == null)
                {
                    Repositorio.Embarcador.Logistica.PermanenciaLocal repPermanenciaLocal = new Repositorio.Embarcador.Logistica.PermanenciaLocal(unitOfWork);
                    permanenciasLocais = repPermanenciaLocal.BuscarPorCarga(codigoCarga);
                    permanenciasLocaisCargaCache.Add(new PermanenciasLocalCarga
                    {
                        CodigoCarga = codigoCarga,
                        PermanenciaLocais = permanenciasLocais
                    });
                }
            }
            return permanenciasLocais;
        }

        private Dominio.ObjetosDeValor.Cliente[] BuscarClientesFronteiraAlfandegaComGeolocalizacao(Repositorio.UnitOfWork unitOfWork)
        {
            DateTime inicio = DateTime.UtcNow;
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.ObjetosDeValor.Cliente[] clientes = repCliente.BuscarTodosComGeolocalizacao(true, false, geolocalizacaoApenasJuridico);
            Log($"BuscarClientesFronteiraAlfandegaComGeolocalizacao {clientes.Length}", inicio, 1);
            return clientes;
        }

        private bool DeveProcessarTrocaDeAlvo(Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool naoProcessarTrocaAlvoViaMonitoramentoTipoOperacao)
        {
            return (!configuracao.NaoProcessarTrocaAlvoViaMonitoramento && !naoProcessarTrocaAlvoViaMonitoramentoTipoOperacao);
        }

        private bool DeveProcessarTrocaDeAlvoEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, bool naoProcessarTrocaAlvoViaMonitoramentoTipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            if (DeveProcessarTrocaDeAlvo(configuracao, naoProcessarTrocaAlvoViaMonitoramentoTipoOperacao))
            {
                if (configuracao.RegistrarEntregasApenasAposAtenderTodasColetas)
                {
                    if (cargaEntrega != null && !cargaEntrega.Coleta && HaColetaPendentesControleEntrega(cargaEntrega.Carga.Codigo, unitOfWork))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private bool HaColetaPendentesControleEntrega(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            return repCargaEntrega.ExisteColetaNaoEntregueNaOrigemPorCarga(codigoCarga);
        }

        private List<MonitoramentoCarga> ExtrairMonitoramentos(List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo> trocasDeAlvo)
        {
            List<MonitoramentoCarga> monitoramentosCarga = new List<MonitoramentoCarga>();
            int total = trocasDeAlvo.Count;
            for (int i = 0; i < total; i++)
            {
                if (trocasDeAlvo[i].CodigoMonitoramento > 0 && trocasDeAlvo[i].CodigoCarga > 0 && trocasDeAlvo[i].CodigoVeiculo > 0)
                {
                    bool existe = false;
                    int totalMonitoramentos = monitoramentosCarga.Count;
                    for (int j = 0; j < totalMonitoramentos; j++)
                    {
                        if (trocasDeAlvo[i].CodigoMonitoramento == monitoramentosCarga[j].CodigoMonitoramento)
                        {
                            existe = true;
                            break;
                        }
                    }

                    if (!existe)
                    {
                        monitoramentosCarga.Add(new MonitoramentoCarga
                        {
                            CodigoMonitoramento = trocasDeAlvo[i].CodigoMonitoramento,
                            CodigoCarga = trocasDeAlvo[i].CodigoCarga,
                            CodigoVeiculo = trocasDeAlvo[i].CodigoVeiculo,
                            DataInicio = trocasDeAlvo[i].DataInicioMonitoramento,
                            DataFim = trocasDeAlvo[i].DataFimMonitoramento
                        });
                    }
                }
            }
            return monitoramentosCarga;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo> ExtrairTrocasDeAlvoMonitoramento(List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo> trocasDeAlvo, int codigoMonitoramento)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo> trocasDeAlvoEncontradas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo>();
            int total = trocasDeAlvo.Count;
            for (int i = 0; i < total; i++)
            {
                if (trocasDeAlvo[i].CodigoMonitoramento > 0 && trocasDeAlvo[i].CodigoMonitoramento == codigoMonitoramento)
                {
                    trocasDeAlvoEncontradas.Add(trocasDeAlvo[i]);
                }
            }
            return trocasDeAlvoEncontradas;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoSaidaDeAlvo> ExtrairSaidasDeAlvoMonitoramento(List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoSaidaDeAlvo> saidasDeAlvo, int codigoMonitoramento)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoSaidaDeAlvo> saidasDeAlvoEncontradas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoSaidaDeAlvo>();
            int total = saidasDeAlvo.Count;
            for (int i = 0; i < total; i++)
            {
                if (saidasDeAlvo[i].CodigoMonitoramento > 0 && saidasDeAlvo[i].CodigoMonitoramento == codigoMonitoramento)
                {
                    saidasDeAlvoEncontradas.Add(saidasDeAlvo[i]);
                }
            }
            return saidasDeAlvoEncontradas;
        }

        private Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega ObterEntregaParaOCliente(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas, double codigoCliente)
        {
            int total = cargaEntregas.Count;
            for (int i = 0; i < total; i++)
            {
                if (cargaEntregas[i].Cliente != null && cargaEntregas[i].Cliente.CPF_CNPJ == codigoCliente)
                {
                    return cargaEntregas[i];
                }
            }
            return null;

            /*
            // Identifica entregas respeitando a ordem das coletas já finalizadas
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = null;
            int total = cargaEntregas.Count;
            for (int i = 0; i < total; i++)
            {
                if (cargaEntregas[i].Cliente != null && cargaEntregas[i].Cliente.CPF_CNPJ == codigoCliente)
                {
                    cargaEntrega = cargaEntregas[i];
                    break;
                }
            }

            // Todas as coletas anteriores devem ter sido finalizadas
            if (cargaEntrega != null && ExisteColetaAnteriorEmAberto(cargaEntregas, cargaEntrega.Ordem))
            {
                Log($"ObterEntregaParaOCliente ExisteColetaAnteriorEmAberto ({codigoCliente} ordem {cargaEntrega.Ordem})", 7);
                cargaEntrega = null;
            }

            return cargaEntrega;
            */
        }

        private bool ExisteColetaAnteriorEmAberto(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas, int ordemMaxima)
        {
            int total = cargaEntregas.Count;
            for (int i = 0; i < total; i++)
            {
                if (cargaEntregas[i].Cliente != null && cargaEntregas[i].Coleta && cargaEntregas[i].DataConfirmacao == null && cargaEntregas[i].Ordem < ordemMaxima)
                {
                    return true;
                }
            }
            return false;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> ExtrairCargaEntregasDosClientes(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas, List<double> clientes)
        {
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregasDosClientes = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();
            int total = cargaEntregas.Count;
            int totalClientes = clientes.Count;
            for (int i = 0; i < total; i++)
            {
                for (int j = 0; totalClientes < total; j++)
                {
                    if (cargaEntregas[i].Cliente != null && cargaEntregas[i].Cliente.CPF_CNPJ == clientes[j])
                    {
                        cargaEntregasDosClientes.Add(cargaEntregas[i]);
                    }
                }
            }
            return cargaEntregasDosClientes;
        }

        private Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente ObterPermanenciaNoClienteAberta(List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasCliente, double codigoCliente)
        {
            int total = permanenciasCliente?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (permanenciasCliente[i].Cliente.CPF_CNPJ == codigoCliente && permanenciasCliente[i].DataFim == null)
                {
                    return permanenciasCliente[i];
                }
            }
            return null;
        }

        private Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal ObterPermanenciaNoLocalAberta(List<Dominio.Entidades.Embarcador.Logistica.PermanenciaLocal> permanenciasLocal, int codigoLocal)
        {
            int total = permanenciasLocal?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (permanenciasLocal[i].Local.Codigo == codigoLocal && permanenciasLocal[i].DataFim == null)
                {
                    return permanenciasLocal[i];
                }
            }
            return null;
        }

        private Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea ObterPermanenciaNaSubareaAberta(List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciasSubarea, int codigoSubarea)
        {
            int total = permanenciasSubarea.Count;
            for (int i = 0; i < total; i++)
            {
                if (permanenciasSubarea[i].Subarea.Codigo == codigoSubarea && permanenciasSubarea[i].DataFim == null)
                {
                    return permanenciasSubarea[i];
                }
            }
            return null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> BuscarPosicoesVeiculo(Repositorio.UnitOfWork unitOfWork, int codigoVeiculo, DateTime dataInicial, DateTime dataFim)
        {
            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = repPosicao.BuscarWaypointsPorVeiculoDataInicialeFinal(codigoVeiculo, dataInicial, dataFim);
            return ValidarPosicoes(posicoes);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ValidarPosicoes(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesValidas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            int total = posicoes.Count;
            for (int i = 0; i < total; i++)
            {
                if (Servicos.Embarcador.Logistica.WayPointUtil.ValidarCoordenadas(posicoes[i].Latitude, posicoes[i].Longitude))
                {
                    posicoesValidas.Add(posicoes[i]);
                }
            }
            return posicoesValidas;
        }

        private bool ExisteAlgumaTrocaDeAlvoElegivel(List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo> trocasDeAlvoMonitoramento)
        {
            // Verifica se há alguma troca de alvo elegível
            int total = trocasDeAlvoMonitoramento?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                // A posição atual deve estar em alvo OU a posição anterior deve estar em alvo
                if ((trocasDeAlvoMonitoramento[i].EmAlvoPosicaoAtual ?? false) || (trocasDeAlvoMonitoramento[i].CodigoPosicaoAnterior > 0 && (trocasDeAlvoMonitoramento[i].EmAlvoPosicaoAnterior ?? false)))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ExisteAlgumaTrocaDeLocalElegivel(List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo> trocasDeLocalMonitoramento)
        {
            // Verifica se há alguma troca de alvo elegível
            int total = trocasDeLocalMonitoramento?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                // A posição atual deve estar em alvo OU a posição anterior deve estar em alvo
                if ((trocasDeLocalMonitoramento[i].EmLocalPosicaoAtual ?? false) || (trocasDeLocalMonitoramento[i].CodigoPosicaoAnterior > 0 && (trocasDeLocalMonitoramento[i].EmLocalPosicaoAnterior ?? false)))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ExisteAlgumaSaidaDeAlvoElegivel(List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo> trocasDeAlvoMonitoramento)
        {
            // Verifica se há alguma troca de alvo elegível
            int total = trocasDeAlvoMonitoramento?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                // A posição atual deve estar em alvo OU a posição anterior deve estar em alvo
                if (trocasDeAlvoMonitoramento[i].CodigoPosicaoAnterior > 0 && (trocasDeAlvoMonitoramento[i].EmAlvoPosicaoAnterior ?? false))
                {
                    return true;
                }
            }
            return false;
        }

        private bool SaiuDoAlvo(double codigoClienteAlvoAnterior, string[] codigosClientesAlvosAtuais)
        {
            return !ClienteEstaNaLista(codigoClienteAlvoAnterior, codigosClientesAlvosAtuais);
        }

        private bool ClienteEstaNaLista(double codigoClienteAlvoAnterior, string[] codigosClientesAlvosAtuais)
        {
            int totalAlvosAtuais = codigosClientesAlvosAtuais?.Length ?? 0;
            for (int i = 0; i < totalAlvosAtuais; i++)
            {
                if (!string.IsNullOrWhiteSpace(codigosClientesAlvosAtuais[i]))
                {
                    double codigoClienteAlvoAtual = double.Parse(codigosClientesAlvosAtuais[i]);
                    if (codigoClienteAlvoAnterior == codigoClienteAlvoAtual)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ClienteEstaNaLista(double codigoCliente, Dominio.ObjetosDeValor.Cliente[] clientes)
        {
            int total = clientes?.Length ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (clientes[i].Codigo == codigoCliente) return true;
            }
            return false;
        }

        private bool SaiuDoLocal(int codigoLocalAnterior, string[] codigosLocalAtual)
        {
            return !LocalEstaNaLista(codigoLocalAnterior, codigosLocalAtual);
        }

        private bool LocalEstaNaLista(int codigoLocalAnterior, string[] codigosLocalAtual)
        {
            int totalAlvosAtuais = codigosLocalAtual?.Length ?? 0;
            for (int i = 0; i < totalAlvosAtuais; i++)
            {
                if (!string.IsNullOrWhiteSpace(codigosLocalAtual[i]))
                {
                    int codigoLocalAtual = int.Parse(codigosLocalAtual[i]);
                    if (codigoLocalAnterior == codigoLocalAtual)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private PosicaoEmAlvo IdentificarPrimeiraPosicaoEntradaAlvo(double codigoCliente, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo trocaDeAlvo, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            PosicaoEmAlvo posicaoEmAlvo = null;
            if (configuracao.TempoMinutosPermanenciaCliente > 0)
            {
                posicaoEmAlvo = BuscarPrimeiraPosicaoEntradaAlvo(codigoCliente, 0, trocaDeAlvo.CodigoCarga, 0, trocaDeAlvo.CodigoVeiculo, trocaDeAlvo.DataInicioMonitoramento ?? trocaDeAlvo.DataCriacaoMonitoramento ?? DateTime.MinValue, trocaDeAlvo.DataVeiculoPosicaoAtual.Value, true, configuracao, unitOfWork);
            }

            if (posicaoEmAlvo == null)
            {
                posicaoEmAlvo = new PosicaoEmAlvo
                {
                    Data = trocaDeAlvo.DataVeiculoPosicaoAtual.Value,
                    Latitude = trocaDeAlvo.LatitudePosicaoAtual.Value,
                    Longitude = trocaDeAlvo.LongitudePosicaoAtual.Value
                };
            }

            return posicaoEmAlvo;
        }

        private PosicaoEmAlvo IdentificarPrimeiraPosicaoEntradaLocal(int codigoLocal, Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo trocaDeLocal, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            PosicaoEmAlvo posicaoEmAlvo = null;
            if (configuracao.TempoMinutosPermanenciaCliente > 0)
            {
                posicaoEmAlvo = BuscarPrimeiraPosicaoEntradaLocal(codigoLocal, trocaDeLocal.CodigoCarga, 0, trocaDeLocal.CodigoVeiculo, trocaDeLocal.DataInicioMonitoramento ?? trocaDeLocal.DataCriacaoMonitoramento ?? DateTime.MinValue, trocaDeLocal.DataVeiculoPosicaoAtual.Value, true, configuracao, unitOfWork);
            }
            if (posicaoEmAlvo == null)
            {
                posicaoEmAlvo = new PosicaoEmAlvo
                {
                    Data = trocaDeLocal.DataVeiculoPosicaoAtual.Value,
                    Latitude = trocaDeLocal.LatitudePosicaoAtual.Value,
                    Longitude = trocaDeLocal.LongitudePosicaoAtual.Value
                };
            }
            return posicaoEmAlvo;
        }

        private PosicaoEmAlvo BuscarPrimeiraPosicaoEntradaAlvo(double codigoCliente, int codigoSubarea, int codigoCarga, int codigoCargaEntrega, int codigoVeiculo, DateTime dataInicial, DateTime dataFinal, bool verificarPermancencia, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            DateTime inicio = DateTime.UtcNow;
            if (verificarPermancencia)
            {
                // Ajusta a data inicial para considerar a partir da última permanência no cliente OU na subárea
                if (codigoCliente > 0)
                {
                    Repositorio.Embarcador.Logistica.PermanenciaCliente repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(unitOfWork);
                    DateTime dataSaidaUltimaPermanencia = repPermanenciaCliente.BuscarDataUltimaSaidaDoClienteECarga(codigoCliente, codigoCarga);
                    if (dataSaidaUltimaPermanencia != null && dataSaidaUltimaPermanencia > dataInicial)
                    {
                        dataInicial = dataSaidaUltimaPermanencia.AddSeconds(1);
                    }
                }
                else
                {
                    Repositorio.Embarcador.Logistica.PermanenciaSubarea repPermanenciaSubarea = new Repositorio.Embarcador.Logistica.PermanenciaSubarea(unitOfWork);
                    DateTime dataSaidaUltimaPermanencia = repPermanenciaSubarea.BuscarDataUltimaSaidaDaSubareaECargaEntrega(codigoSubarea, codigoCargaEntrega);
                    if (dataSaidaUltimaPermanencia != null && dataSaidaUltimaPermanencia > dataInicial)
                    {
                        dataInicial = dataSaidaUltimaPermanencia.AddSeconds(1);
                    }
                }
                Log("BuscarPrimeiraPosicaoEntradaAlvo verificarPermancencia", inicio, 6);
            }

            inicio = DateTime.UtcNow;

            // Primeira posição da entrada do veículo no cliente OU na subárea do cliente
            DateTime hoje = DateTime.Now;
            if (dataInicial <= hoje.AddMonths(-1)) //new DateTime(hoje.Year, hoje.Month - 1, hoje.Day)
            {
                dataInicial = hoje.AddMonths(-1); //new DateTime(hoje.Year, hoje.Month - 1, hoje.Day)
            }

            if (dataFinal < dataInicial)
                return null;

            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Posicao posicaoEntradaCliente;
            if (codigoCliente > 0)
            {
                //List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = repPosicao.BuscarWaypointsPorVeiculoDataInicialeFinal(codigoVeiculo, dataInicial, dataFinal);

                //if (posicoes != null && posicoes.Count > 0)
                //{
                posicaoEntradaCliente = repPosicao.BuscarPrimeiraPosicaoEmAlvoPorVeiculoDataInicialeFinal(codigoCliente, codigoVeiculo, dataInicial, dataFinal);
                Log("BuscarPrimeiraPosicaoEmAlvoPorVeiculoDataInicialeFinal", inicio, 6);
                //}
            }
            else
            {
                posicaoEntradaCliente = repPosicao.BuscarPrimeiraPosicaoEmAlvoSubareaPorVeiculoDataInicialeFinal(codigoSubarea, codigoVeiculo, dataInicial, dataFinal);
                Log("BuscarPrimeiraPosicaoEmAlvoSubareaPorVeiculoDataInicialeFinal", inicio, 6);
            }

            if (posicaoEntradaCliente != null)
            {
                return new PosicaoEmAlvo
                {
                    Data = posicaoEntradaCliente.DataVeiculo,
                    Latitude = posicaoEntradaCliente.Latitude,
                    Longitude = posicaoEntradaCliente.Longitude
                };
            }
            return null;
        }

        private PosicaoEmAlvo BuscarPrimeiraPosicaoEntradaLocal(int codigoLocal, int codigoCarga, int codigoCargaEntrega, int codigoVeiculo, DateTime dataInicial, DateTime dataFinal, bool verificarPermancencia, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            DateTime inicio = DateTime.UtcNow;
            if (verificarPermancencia)
            {
                // Ajusta a data inicial para considerar a partir da última permanência no local
                if (codigoLocal > 0)
                {
                    Repositorio.Embarcador.Logistica.PermanenciaLocal repPermanenciaLocal = new Repositorio.Embarcador.Logistica.PermanenciaLocal(unitOfWork);
                    DateTime dataSaidaUltimaPermanencia = repPermanenciaLocal.BuscarDataUltimaSaidaDoLocal(codigoLocal, codigoCarga);
                    if (dataSaidaUltimaPermanencia != null && dataSaidaUltimaPermanencia > dataInicial)
                    {
                        dataInicial = dataSaidaUltimaPermanencia.AddSeconds(1);
                    }
                }
                Log("BuscarPrimeiraPosicaoEntradaLocal verificarPermancencia", inicio, 6);
            }

            inicio = DateTime.UtcNow;

            // Primeira posição da entrada do veículo no local
            DateTime hoje = DateTime.Now;
            if (dataInicial <= hoje.AddMonths(-1)) //new DateTime(hoje.Year, hoje.Month - 1, hoje.Day)
            {
                dataInicial = hoje.AddMonths(-1); //new DateTime(hoje.Year, hoje.Month - 1, hoje.Day)
            }

            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Posicao posicaoEntradaLocal = null;
            if (codigoLocal > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = repPosicao.BuscarWaypointsPorVeiculoDataInicialeFinal(codigoVeiculo, dataInicial, dataFinal);

                if (posicoes != null && posicoes.Count > 0)
                {
                    posicaoEntradaLocal = repPosicao.BuscarPrimeiraPosicaoEmLocalPorVeiculoDataInicialeFinal(posicoes.Select(x => x.ID).ToList(), codigoLocal);
                    Log("BuscarPrimeiraPosicaoEmlocalPorVeiculoDataInicialeFinal", inicio, 6);
                }
            }

            if (posicaoEntradaLocal != null)
            {
                return new PosicaoEmAlvo
                {
                    Data = posicaoEntradaLocal.DataVeiculo,
                    Latitude = posicaoEntradaLocal.Latitude,
                    Longitude = posicaoEntradaLocal.Longitude
                };
            }
            return null;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo> BuscarPendenciasTrocasDeAlvo(Repositorio.UnitOfWork unitOfWork, int quantidadeRegistros)
        {
            DateTime inicio = DateTime.UtcNow;
            this.arquivosEmProcessamento = new List<string>();
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo> trocasDeAlvo = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo>();

            // Busca os arquivos com as pendências da fila de trocas de alvo
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendenciaArquivo> pendencias = base.BuscarPendenciasFila(this.diretorioFila, quantidadeRegistros);
            int totalArquivos = pendencias.Count;
            if (totalArquivos > 0)
            {
                Repositorio.Embarcador.Logistica.MonitoramentoTrocaAlvo repMonitoramentoTrocaAlvo = new Repositorio.Embarcador.Logistica.MonitoramentoTrocaAlvo(unitOfWork);
                for (int i = 0; i < totalArquivos; i++)
                {

                    // Adiciona na lista de arquivos em processamento
                    this.arquivosEmProcessamento.Add(pendencias[i].CaminhoArquivo);

                    // Consulta os dados do monitoramento e das posições das pendências
                    int totalPendencias = pendencias[i].Pendencias?.Count ?? 0;
                    for (int j = 0; j < totalPendencias; j++)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoTrocaDeAlvo pendencia = repMonitoramentoTrocaAlvo.BuscarTrocasDeAlvoPendentes(pendencias[i].Pendencias[j].Monitoramento, pendencias[i].Pendencias[j].PosicaoAtual ?? 0, pendencias[i].Pendencias[j].PosicaoAnterior ?? 0);
                        if (pendencia != null) trocasDeAlvo.Add(pendencia);
                    }

                }
                Log("BuscarPendenciasTrocasDeAlvo", inicio, 6);
            }
            return trocasDeAlvo;
        }

        private void ExcluirArquivosDePendencias()
        {
            int total = this.arquivosEmProcessamento.Count;
            if (total > 0)
            {
                DateTime inicio = DateTime.UtcNow;
                Log($"Excluindo {total} arquivos de pendencias", 1);
                for (int i = 0; i < total; i++)
                {
                    File.Delete(this.arquivosEmProcessamento[i]);
                }
                Log($"Excluidos {total} arquivos de pendencias", inicio, 1);
            }
        }

        private int Absoluto(double numero)
        {
            int inteiro;
            try
            {
                inteiro = Math.Abs((int)numero);
            }
            catch
            {
                return int.MaxValue;
            }
            return inteiro;
        }

        private void GerarEventoAtrasoDescargaNaEntradaRaioEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, int codigoCarga, int codigoVeiculo, double latitude, double longitude, DateTime data, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaEntrega != null && !cargaEntrega.Coleta && cargaEntrega.DataInicio.HasValue)
            {
                Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento evento = repMonitoramentoEvento.BuscarAtivo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.AtrasoNaDescarga);
                if (evento != null && evento.Gatilho != null)
                {
                    if (evento.Gatilho.DataBase != Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData.EntradaCliente) // aqui só processa se a data base é entrada cliente.
                        return;


                    DateTime dataBase = data;

                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    DateTime? dataReferencia = repCargaPedido.BuscarMaiorPrevisaoEntrega(codigoCarga, cargaEntrega.Cliente?.Codigo ?? 0);

                    if (dataReferencia != null)
                    {
                        // Adiciona a tolerância
                        dataReferencia = dataReferencia.Value.AddMinutes(evento.Gatilho.TempoEvento);
                        // Atrasou?
                        if (dataBase > dataReferencia)
                        {
                            // Cria o alerta para a carga se não existir algum
                            TimeSpan atraso = dataBase - dataReferencia.Value;
                            string texto = $"Descarga {cargaEntrega.Cliente?.CPF_CNPJ_Formatado ?? ""} atrasada por " + Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(atraso);
                            GerarEvento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.AtrasoNaDescarga, texto, codigoCarga, codigoVeiculo, cargaEntrega.Codigo, latitude, longitude, data, unitOfWork);
                        }
                    }
                }
            }
        }

        private void GerarEventoChegadaNoRaio(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, int codigoCarga, int codigoVeiculo, double latitude, double longitude, DateTime data, Repositorio.UnitOfWork unitOfWork)
        {
            GerarEvento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ChegadaNoRaio, cargaEntrega.Cliente?.Descricao ?? "", codigoCarga, codigoVeiculo, cargaEntrega.Codigo, latitude, longitude, data, unitOfWork);
        }

        private void GerarEventoPermanenciaLocal(int codigoCarga, int codigoVeiculo, double latitude, double longitude, DateTime data, Repositorio.UnitOfWork unitOfWork)
        {
            TimeSpan atraso = DateTime.Now - data;
            string texto = (atraso.TotalMinutes < 60) ? $"Há {(int)atraso.TotalMinutes} minutos" : Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(atraso);
            texto += ", desde " + data.ToString("dd/MM/yyyy HH:mm");
            GerarEvento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.PermanenciaNoPontoApoio, texto, codigoCarga, codigoVeiculo, 0, latitude, longitude, data, unitOfWork);
        }

        private void GerarEventoChegadaNoRaioEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, int codigoCarga, int codigoVeiculo, double latitude, double longitude, DateTime data, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaEntrega != null && !cargaEntrega.Coleta)
            {
                GerarEvento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ChegadaNoRaioEntrega, cargaEntrega.Cliente?.Descricao ?? "", codigoCarga, codigoVeiculo, cargaEntrega.Codigo, latitude, longitude, data, unitOfWork);
            }
        }

        private void GerarEvento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta, string descricaoAlerta, int codigoCarga, int codigoVeiculo, int codigoEntrega, double latitude, double longitude, DateTime data, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento evento = repMonitoramentoEvento.BuscarAtivo(tipoAlerta);

            if (evento != null && evento.Gatilho != null)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoEntrega);
                if (cargaEntrega != null && cargaEntrega.Fronteira)
                    return;

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta>();
                tiposAlerta.Add(tipoAlerta);

                Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertaMonitor> alertas = repAlertaMonitor.BuscarUltimoAlertaCargaETipoDeAlerta(codigoCarga, tiposAlerta);

                if (alertas == null || alertas.Count() == 0 || (alertas[0].Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado && alertas[0].Data.AddMinutes(evento.Gatilho.Tempo) < data))
                {
                    Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta = new Dominio.Entidades.Embarcador.Logistica.AlertaMonitor()
                    {
                        TipoAlerta = tipoAlerta,
                        MonitoramentoEvento = evento,
                        Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto,
                        DataCadastro = DateTime.Now,
                        Data = data,
                        Latitude = (decimal)latitude,
                        Longitude = (decimal)longitude,
                        Veiculo = new Dominio.Entidades.Veiculo { Codigo = codigoVeiculo },
                        CargaEntrega = codigoEntrega > 0 ? new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega { Codigo = codigoEntrega } : null,
                        Carga = new Dominio.Entidades.Embarcador.Cargas.Carga { Codigo = codigoCarga },
                        AlertaDescricao = descricaoAlerta.Length > 50 ? descricaoAlerta.Substring(0, 50) : descricaoAlerta
                    };
                    Repositorio.Embarcador.Logistica.AlertaMonitor repAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                    repAlerta.Inserir(alerta);
                }
            }
        }

        private void ProcessarAcoesFluxoDePatioSubarea(Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea permanenciaSubarea, MonitoramentoEventoData eventoMonitoramento, Repositorio.UnitOfWork unitOfWork)
        {
            string processo = "ProcessarAcoesFluxoDePatioSubarea - " + (eventoMonitoramento == MonitoramentoEventoData.EntradaCliente ? "Entrada no Alvo" : "Saida do Alvo");
            Log(processo, DateTime.UtcNow, 7);
            try
            {
                Repositorio.Embarcador.Logistica.SubareaCliente repSubAreaCliente = new Repositorio.Embarcador.Logistica.SubareaCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.SubareaCliente subAreaCliente = repSubAreaCliente.BuscarPorCodigo(permanenciaSubarea.Subarea.Codigo, false);
                if (subAreaCliente.TipoSubarea.PermiteMovimentacaoDoPatioPorEntradaOuSaidaDaArea ?? false)
                {
                    Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado());
                    Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio;

                    //Para permanencia em subarea de Coleta, busca Fluxo de Gestão de Pátio do tipo Origem.
                    if (permanenciaSubarea.CargaEntrega.Coleta)
                        fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(permanenciaSubarea.CargaEntrega.Carga);
                    //... Se não busca do tipo destino.
                    else
                        fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatioDestino(permanenciaSubarea.CargaEntrega.Carga);

                    if (fluxoGestaoPatio != null)
                    {
                        //Busca ações de Fluxo de Pátio cadastradas para a Subárea, com o tipo de evento (Entrada ou Saida do Alvo).
                        List<Dominio.Entidades.Embarcador.Logistica.SubareaClienteAcoesFluxoDePatio> acoesFluxoDePatio = subAreaCliente.AcoesFluxoPatio.ToList().FindAll(x => x.AcaoMonitoramento == eventoMonitoramento);
                        foreach (Dominio.Entidades.Embarcador.Logistica.SubareaClienteAcoesFluxoDePatio acao in acoesFluxoDePatio)
                        {
                            Log("Processar Acoes Fluxo de Patio: " + AcaoFluxoGestaoPatioHelper.ObterDescricao(acao.AcaoFluxoPatio) + processo, DateTime.UtcNow, 8);

                            switch (acao.AcaoFluxoPatio)
                            {
                                case AcaoFluxoGestaoPatio.Confirmar:
                                    servicoFluxoGestaoPatio.LiberarProximaEtapa(fluxoGestaoPatio, acao.EtapaFluxoPatio);
                                    break;

                                case AcaoFluxoGestaoPatio.Voltar:
                                    servicoFluxoGestaoPatio.VoltarEtapa(fluxoGestaoPatio, acao.EtapaFluxoPatio, null);
                                    break;
                            }
                        }
                    }
                    else
                        Log("Fluxo Gestão de Patio não encontrado para CargaEntrega: " + permanenciaSubarea.CargaEntrega.Codigo.ToString(), DateTime.UtcNow, 8);
                }
                else
                    Log("Subárea " + subAreaCliente.Descricao + " não permite Movimentacao do Patio", DateTime.UtcNow, 8);
            }
            catch (Exception ex)
            {
                Log("Exception: " + ex.Message, DateTime.UtcNow, 8);
            }
            Log("ProcessarAcoesFluxoDePatioSubarea - " + processo, DateTime.UtcNow, 7);
        }
        #endregion

    }

}
