using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Data.Common;
using Servicos.Embarcador.Monitoramento;

namespace SGT.Monitoramento.Thread
{

    public class CargaClientePassagem
    {
        public int CodigoCarga;
        public DateTime Data;
        public List<Dominio.Entidades.Cliente> Clientes;
        public List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> PontosDePassagem;
        public CargaClientePassagem(int codigoCarga, DateTime data, List<Dominio.Entidades.Cliente> clientes, List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem)
        {
            CodigoCarga = codigoCarga;
            Data = data;
            Clientes = clientes;
            PontosDePassagem = pontosDePassagem;
        }
    }

    public class VeiculoPosicao
    {
        public int CodigoVeiculo;
        public Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao[] Posicoes;
        public VeiculoPosicao(int codigoVeiculo)
        {
            CodigoVeiculo = codigoVeiculo;
            Posicoes = new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao[] { };
        }
        public VeiculoPosicao(int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao[] posicoes)
        {
            CodigoVeiculo = codigoVeiculo;
            Posicoes = posicoes;
        }
    }

    public class MonitoramentoPendenteAtualizar
    {
        public int codigoMonitoramento;
        public DateTime dataPosicao;
        public long codigoPosicao;
        public decimal? temperatura;
    }

    public class MonitoramentoVeiculoPosicao
    {
        public int codigoMonitoramento;
        public int codigoVeiculo;
        public long codigoPosicao;
    }

    public class PosicaAlvoPosicaoAlvoSubarea
    {
        public Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo PosicaoAlvo;
        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea> PosicaoAlvoSubareas = new List<Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea>();
    }

    public class MonitorarPosicoes : AbstractThreadProcessamento
    {

        #region Atributos privados

        private static MonitorarPosicoes Instante;
        private static System.Threading.Thread MonitorarPosicoesThread;

        private int tempoSleep = 5;
        private bool enable = true;
        private int limiteRegistros = 100;
        private int minutosClientesCache = 0;
        private int horasPosicoesCache = 1;
        private int horasParaExpirarPosicoes = -24;
        private int distancaiAlvosMetros = 0;
        private string arquivoNivelLog;
        private bool confirmarValidadeDaPosicao = false;
        private bool registrarPosicaoInvalida = false;
        private bool excluirPosicaoInvalida = false;
        private bool geolocalizacaoApenasJuridico = false;
        private bool processarPosicoesDemaisPlacas = false;
        private bool GerarPermanenciaLocais = false;
        private bool processarMonitoramentos = false;

        private bool processarEventos = false;
        private string processarEventosDiretorioFila;
        private string processarEventosArquivoFilaPrefixo;

        private bool processarTrocaDeAlvo = false;
        private string processarTrocaDeAlvoDiretorioFila;
        private string processarTrocaDeAlvoArquivoFilaPrefixo;

        private DateTime dataAtual;
        private DateTime dataMenorValidade;
        private DateTime dataSanitizeCache;
        private DateTime? dataConsultaClientes;

        private bool clientesComEnderecosSecundarios = false;

        private List<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual> PosicoesAtuaisCache = new List<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual>();
        private List<CargaClientePassagem> CargasClientesPassagemCache = new List<CargaClientePassagem>();
        private List<VeiculoPosicao> VeiculosPosicoesCache = new List<VeiculoPosicao>();

        private Dominio.ObjetosDeValor.Cliente[] ClientesCache = null;
        private Dominio.Entidades.Embarcador.Logistica.Locais[] LocaisCache = null;
        private Dominio.ObjetosDeValor.Embarcador.Logistica.SubareaCliente[] SubareasCache = null;

        private List<PosicaAlvoPosicaoAlvoSubarea> PosicaoAlvosPosicaoAlvosSubareas;
        private List<Dominio.Entidades.Embarcador.Logistica.PosicaoLocal> PosicaoLocais = new List<Dominio.Entidades.Embarcador.Logistica.PosicaoLocal>();

        #endregion

        #region Métodos públicos

        // Singleton
        public static MonitorarPosicoes GetInstance(string stringConexao)
        {
            if (Instante == null)
                Instante = new MonitorarPosicoes(stringConexao);
            return Instante;
        }

        public System.Threading.Thread Iniciar(string stringConexao, string ambiente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            if (enable)
                MonitorarPosicoesThread = base.IniciarThread(stringConexao, ambiente, tipoServicoMultisoftware, clienteMultisoftware, arquivoNivelLog, tempoSleep);

            return MonitorarPosicoesThread;
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
            BuscarProcessarPosicoesPendentes(unitOfWork);
        }

        override protected void Parar()
        {
            if (MonitorarPosicoesThread != null)
            {
                MonitorarPosicoesThread.Abort();
                MonitorarPosicoesThread = null;
            }
        }

        #endregion

        #region Construtor privado

        private MonitorarPosicoes(string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = string.IsNullOrWhiteSpace(stringConexao) ? null : new Repositorio.UnitOfWork(stringConexao);
            try
            {
                tempoSleep = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoMonitorarPosicoes().TempoSleepThread;
                enable = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoMonitorarPosicoes().Ativo;
                limiteRegistros = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoMonitorarPosicoes().LimiteRegistros;
                minutosClientesCache = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoMonitorarPosicoes().MinutosClientesCache;
                horasPosicoesCache = Math.Abs(Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoMonitorarPosicoes().HorasPosicoesCache) * (-1);
                horasParaExpirarPosicoes = Math.Abs(Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoMonitorarPosicoes().HorasParaExpirarPosicoes) * (-1);
                clientesComEnderecosSecundarios = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoMonitorarPosicoes().ClientesSecundarios;
                confirmarValidadeDaPosicao = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoMonitorarPosicoes().ConfirmarValidadeDaPosicao;
                registrarPosicaoInvalida = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoMonitorarPosicoes().RegistrarPosicaoInvalida;
                excluirPosicaoInvalida = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoMonitorarPosicoes().ExcluirPosicaoInvalida;
                geolocalizacaoApenasJuridico = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoMonitorarPosicoes().GeolocalizacaoApenasJuridico;
                arquivoNivelLog = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoMonitorarPosicoes().ArquivoNivelLog;
                distancaiAlvosMetros = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoMonitorarPosicoes().DistanciaAlvoMetros;
                processarPosicoesDemaisPlacas = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoMonitorarPosicoes().ProcessarPosicoesDemaisPlacas;
                
                processarMonitoramentos = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarMonitoramentos().Ativo;

                processarEventos = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventos().Ativo;
                processarEventosDiretorioFila = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventos().DiretorioFila;
                processarEventosArquivoFilaPrefixo = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarEventos().ArquivoFilaPrefixo;

                processarTrocaDeAlvo = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarTrocaAlvo().Ativo;
                processarTrocaDeAlvoDiretorioFila = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarTrocaAlvo().DiretorioFila;
                processarTrocaDeAlvoArquivoFilaPrefixo = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarTrocaAlvo().ArquivoFilaPrefixo;
                GerarPermanenciaLocais = Servicos.Embarcador.Configuracoes.ConfiguracaoMonioramentoInstance.GetInstance(unitOfWork).ObterConfiguracaoMonitoramentoProcessarTrocaAlvo().GerarPermanenciaLocais;

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

        private void BuscarProcessarPosicoesPendentes(Repositorio.UnitOfWork unitOfWork)
        {

            // Buscar posições pendentes
            List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoesPendente = BuscarPosicoesPendentes(unitOfWork);
            Log(posicoesPendente.Count + " posicoes pendentes");

            // Marcá-las como "Processando"
            if (posicoesPendente.Count > 0)
            {
                try
                {
                    AlteraPosicoesProcessar(unitOfWork, posicoesPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processando);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    throw ex;
                }

                // Processa as posições
                try
                {
                    ProcessarPosicoes(unitOfWork, posicoesPendente);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    throw ex;
                }
            }
        }

        /**
         * Processa cada uma das posições.
         */
        private void ProcessarPosicoes(Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes)
        {

            // Atualização das datas de referência única para toda a execução
            this.dataAtual = DateTime.Now;
            this.dataMenorValidade = dataAtual.AddHours(horasParaExpirarPosicoes);

            // Lista de posições que foram processadas
            List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoesProcessado = new List<Dominio.Entidades.Embarcador.Logistica.Posicao>();
            int totalPosicoesAtuaisAtualizadas = 0, totalPosicoesAtuaisExcluidas = 0;
            DateTime inicio, inicio1, inicio2, inicio3;

            // Registra no log um resumo das informações que estão em cache
            LogInformacoesCache();

            // Extrai os veículos únicos envolvidos nas posições recebidas
            List<int> codigosVeiculos = ObtemCodigosVeiculosDistintos(posicoes);
            int total = codigosVeiculos.Count;
            if (total > 0)
            {

                Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                // Lista com os monitoramentos a serem processados
                List<MonitoramentoPendenteAtualizar> monitoramentosParaAtualizarRota = new List<MonitoramentoPendenteAtualizar>();

                // Lista de trocas de alvo pendentes
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendencia> monitoramentoPendenciasTrocasDeAlvo = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendencia>();

                // Lista de eventos pendentes
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendencia> monitoramentoPendenciasEventos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendencia>();

                // Carrega todas as posições atuais
                LoadPosicoesAtuaisCache(unitOfWork, codigosVeiculos);

                // Carrega todas as posições dos veículos envolvidos para o cache
                LoadPosicoesVeiculosCache(unitOfWork, codigosVeiculos);

                // Busca a lista de códigos de veículos que estão em contrato entre os veículos das posições a processar
                List<int> listaCodigosVeiculosEmContrato = BuscarCodigosVeiculosEmContratos(unitOfWork, dataAtual, codigosVeiculos, configuracao);

                // Busca os clientes e subáreas com geolocalização
                LoadClientesComGeolocalizacao(unitOfWork);
                LoadSubareasClientes(unitOfWork);

                // Busca os locais (pontos de apoio)
                LoadLocaisComGeoLocalizacao(unitOfWork);

                // Verifica a existência de eventos ativos e configurados para serem processados
                bool existemEventosAtivos = ExistemEventosAtivos(unitOfWork);

                //bool possuiEventoPercaOuSemSinal = ExistemEventosAtivosDePercaOuSemSinal(unitOfWork);

                // Busca uma lista de monitoramentos abertos durante as posições recebidas
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> monitoramentosAbertos = BuscarMonitoramentosAbertos(unitOfWork, codigosVeiculos, posicoes);

                this.PosicaoAlvosPosicaoAlvosSubareas = new List<PosicaAlvoPosicaoAlvoSubarea>();
                List<MonitoramentoVeiculoPosicao> monitoramentosVeiculosPosicoes = new List<MonitoramentoVeiculoPosicao>();

                inicio = DateTime.UtcNow;
                unitOfWork.Start();
                try
                {

                    for (int i = 0; i < total; i++)
                    {
                        if (!unitOfWork.IsActiveTransaction()) unitOfWork.Start();

                        // Extrai todas as posições recebidas do veículo
                        int codigoVeiculo = codigosVeiculos[i];
                        List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoesVeiculo = ObtemPosicoesDoVeiculo(codigoVeiculo, posicoes);
                        Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao ultimaPosicaoProcessada = null;

                        // Carrega, se ainda não existir, as últimas posições do veículo para o cache
                        LoadPosicoesCache(unitOfWork, codigoVeiculo);

                        // Busca uma possível posição atual do veículo
                        Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = ObtemPosicaoAtualDoCache(codigoVeiculo);

                        inicio1 = DateTime.UtcNow;
                        int contj = posicoesVeiculo.Count;
                        for (int j = 0; j < contj; j++)
                        {
                            inicio2 = DateTime.UtcNow;
                            Dominio.Entidades.Embarcador.Logistica.Posicao posicao = posicoesVeiculo[j];
                            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoObjetoDeValor = ConverterPosicaoParaObjetoDeValor(posicoesVeiculo[j]);

                            // Verifica se a posição recebida é válida
                            if (ConfirmarValidadeDaPosicao(unitOfWork, repPosicao, posicao, posicaoObjetoDeValor, configuracao))
                            {

                                // Verifica se o veículo está localizado dentro do raio/polígono de algum cliente
                                IdentificarAlvos(posicao, configuracao);
                                posicaoObjetoDeValor.EmAlvo = posicao.EmAlvo;

                                // Verifica se o veículo está localizado dentro de algum Local esperado
                                IndentificarLocal(posicao, configuracao);
                                posicaoObjetoDeValor.EmLocal = posicao.EmLocal;

                                // Busca algum monitoramento aberto
                                Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento monitoramento = BuscarMonitoramentoEmAberto(monitoramentosAbertos, posicaoObjetoDeValor);
                                if (monitoramento != null)
                                {
                                    AdicionaNaListaMonitoramentosParaAtualizar(monitoramento.Codigo, posicaoObjetoDeValor, monitoramentosParaAtualizarRota);
                                    ProcessaTrocaDeAlvo(unitOfWork, monitoramentoPendenciasTrocasDeAlvo, monitoramento.Codigo, posicaoObjetoDeValor, configuracao);
                                    monitoramentosVeiculosPosicoes.Add(new MonitoramentoVeiculoPosicao
                                    {
                                        codigoMonitoramento = monitoramento.Codigo,
                                        codigoVeiculo = codigoVeiculo,
                                        codigoPosicao = posicao.Codigo
                                    });

                                    // Processamento dos eventos
                                    if (existemEventosAtivos) ProcessarEventos(monitoramentoPendenciasEventos, posicao, monitoramento?.Codigo ?? 0);
                                }


                                // Adiciona o veículo e as posições recebidas ao cache
                                this.AddPosicaoCache(codigoVeiculo, posicaoObjetoDeValor);

                                // Confirma que, de acordo com a configuração, deve atualizar a posição atual do veículo
                                if (VerificaSeDeveAtualizarPosicaoAtual(codigoVeiculo, monitoramento?.Codigo ?? 0, listaCodigosVeiculosEmContrato, configuracao))
                                {
                                    // Processa e atualiza a posição atual do veículo
                                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao situacao = ((monitoramento != null) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao.EmViagem : Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao.SemViagem);
                                    posicaoAtual = ProcessaPosicaoAtual(unitOfWork, posicaoAtual, posicao, situacao);
                                    totalPosicoesAtuaisAtualizadas++;
                                }
                                else if (posicaoAtual != null)
                                {

                                    // A posição atual deve ser excluída
                                    inicio3 = DateTime.UtcNow;
                                    repPosicaoAtual.Deletar(posicaoAtual);
                                    this.PosicoesAtuaisCache.Remove(posicaoAtual);
                                    totalPosicoesAtuaisExcluidas++;
                                    Log("repPosicaoAtual.Deletar", inicio3, 6);
                                }

                                // Indica que a posição foi processada
                                posicao.Processar = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado;

                                inicio3 = DateTime.UtcNow;
                                repPosicao.Atualizar(posicao);
                                posicoesProcessado.Add(posicao);
                                Log("repPosicao.Atualizar", inicio3, 6);

                                ultimaPosicaoProcessada = posicaoObjetoDeValor;
                            }
                            Log("Posicao", inicio2, 5);
                        }

                        // Como pode ser recebida posição fora de ordem, processa possível troca de alvo da última posição
                        ProcessarTrocaDeAlvoDaUltimaPosicao(unitOfWork, monitoramentoPendenciasTrocasDeAlvo, ultimaPosicaoProcessada, posicaoAtual, monitoramentosAbertos, monitoramentosParaAtualizarRota, configuracao);

                        Log($"Veiculo {codigoVeiculo}", inicio1, 4);

                    }

                    // Salva os clientes e subáreas alvo das posições posições
                    SalvarPosicaoAlvos(unitOfWork);

                    // Salva os Locais das posições
                    SalvarPosicaoLocal(unitOfWork);

                    // Inclui as posições do veículo relacionadas ao monitoramento
                    SalvarMonitoramentosVeiculosPosicoes(unitOfWork, monitoramentosVeiculosPosicoes);

                    inicio1 = DateTime.UtcNow;
                    unitOfWork.CommitChanges();
                    Log("CommitChanges Posicoes", inicio1, 3);

                }
                catch (Exception e)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(e);
                    throw e;
                }

                Log($"Processar posicoes", inicio, 1);
                Log(totalPosicoesAtuaisAtualizadas + " posicoes atuais atualizadas", 2);
                Log(totalPosicoesAtuaisExcluidas + " posicoes atuais excluidas", 2);

                // Salva as trocas de alvo para serem processadas pela outra thread
                SalvarMonitoramentoTrocasDeAlvo(monitoramentoPendenciasTrocasDeAlvo);

                // Salva as posições para serem processadas pela outra thread
                SalvarMonitoramentoProcessarEventos(monitoramentoPendenciasEventos);

                // Marca os monitoramentos como pendentes para atualização da rota
                IndicarMonitoramentosParaAtualizarRota(unitOfWork, monitoramentosParaAtualizarRota);

                // Remove as posições antigas e descartáveis do cache
                DateTime maiorDataVeiculo = ObtemMaiorDataVeiculoDasPosicoes(posicoes);
                SanitizeCache(maiorDataVeiculo);
            }

            // Indica como processada posições recebidas sem veículo indicado
            ProcessarPosicoesSemVeiculo(unitOfWork, posicoes, posicoesProcessado);

            Log(posicoesProcessado.Count + " posicoes processadas com sucesso");
        }

        private List<Dominio.Entidades.Embarcador.Logistica.Posicao> BuscarPosicoesPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Posicao repositorioPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao> litaSituacoesProcessar = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao>();
            litaSituacoesProcessar.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Pendente);
            litaSituacoesProcessar.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processando);

            return repositorioPosicao.BuscarProcessarComLimite(litaSituacoesProcessar, limiteRegistros);
        }

        private void AlteraPosicoesProcessar(Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao processar)
        {
            List<long> codigosPosicoes = new List<long>();
            int total = posicoes.Count;

            //for (int i = 0; i < total; i++)
            //{
            //    posicoes[i].Processar = processar;
            //    codigosPosicoes.Add(posicoes[i].Codigo);
            //}
            AlteraPosicoesProcessar(unitOfWork, posicoes.Select(x => x.Codigo).ToList(), processar);
            Log(total + " posicoes em processamento");
        }

        private void AlteraPosicoesProcessar(Repositorio.UnitOfWork unitOfWork, List<long> codigosPosicoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao processar)
        {
            DbConnection connection = unitOfWork.GetConnection();
            DbTransaction transaction = connection.BeginTransaction();
            try
            {
                // Marca o monitoramento para ser atualizado
                Repositorio.Embarcador.Logistica.Posicao repositorioPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                repositorioPosicao.AtualizarProcessar(codigosPosicoes, processar, connection, transaction);
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Servicos.Log.TratarErro(e);
                throw e;
            }
        }

        /**
         * Marca os monitoramentos como pendentes para atualização da rota
         */
        private void IndicarMonitoramentosParaAtualizarRota(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoPendenteAtualizar> monitoramentosParaAtualizarRota)
        {
            Log(monitoramentosParaAtualizarRota.Count + " rotas para atualizar", 1);
            Repositorio.Embarcador.Cargas.FaixaTemperatura repFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);

            if (processarMonitoramentos && monitoramentosParaAtualizarRota.Count > 0)
            {
                DateTime inicio = DateTime.UtcNow;
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                int total = monitoramentosParaAtualizarRota.Count;
                for (int i = 0; i < total; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.FaixaTemperatura.FaixaTemperaturaMonitoramento faixaTemperatura = repFaixaTemperatura.BuscarPorCodigoMonitoramento(monitoramentosParaAtualizarRota[i].codigoMonitoramento);
                    bool naFaixa = false;
                    if (faixaTemperatura != null && faixaTemperatura.TemperaturaFaixaInicial != null && monitoramentosParaAtualizarRota[i].temperatura != null)
                    {
                        if (monitoramentosParaAtualizarRota[i].temperatura >= faixaTemperatura.TemperaturaFaixaInicial && monitoramentosParaAtualizarRota[i].temperatura <= faixaTemperatura.TemperaturaFaixaFinal)
                            naFaixa = true;
                    }

                    DbConnection connection = unitOfWork.GetConnection();
                    DbTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        // Marca o monitoramento para ser atualizado
                        repMonitoramento.AtualizarProcessarRota(monitoramentosParaAtualizarRota[i].codigoMonitoramento, monitoramentosParaAtualizarRota[i].codigoPosicao, monitoramentosParaAtualizarRota[i].temperatura, naFaixa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Pendente, connection, transaction);
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        Servicos.Log.TratarErro(e);
                        throw e;
                    }
                }
                Log("Marcar monitoramentos para atualizar rota", inicio, 2);
            }
        }

        /**
         * Altera para processado as posições recebidas sem veículo indicado
         */
        private void ProcessarPosicoesSemVeiculo(Repositorio.UnitOfWork unitOfWork, List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes, List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoesProcessadas)
        {
            DateTime inicio = DateTime.UtcNow;
            List<long> codigosPosicoes = new List<long>();
            int total = posicoes.Count;
            for (int i = 0; i < total; i++)
            {
                if (posicoes[i].Veiculo == null)
                {
                    codigosPosicoes.Add(posicoes[i].Codigo);
                    posicoesProcessadas.Add(posicoes[i]);
                }
            }
            AlteraPosicoesProcessar(unitOfWork, codigosPosicoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado);
            Log($"ProcessarPosicoesSemVeiculo {codigosPosicoes.Count}", inicio, 1);
        }

        /**
         * A partir de uma posição de um veículo recebida, cria um objeto PosicaoAtual
         */
        private Dominio.Entidades.Embarcador.Logistica.PosicaoAtual NovaPosicaoAtualApartirDaPosicao(Dominio.Entidades.Embarcador.Logistica.PosicaoAtual novaPosicaoAtual, Dominio.Entidades.Embarcador.Logistica.Posicao posicao, int codigoVeiculo)
        {
            novaPosicaoAtual.Data = posicao.Data;
            novaPosicaoAtual.DataVeiculo = posicao.DataVeiculo;
            novaPosicaoAtual.DataCadastro = posicao.DataCadastro;
            novaPosicaoAtual.Latitude = posicao.Latitude;
            novaPosicaoAtual.Longitude = posicao.Longitude;
            novaPosicaoAtual.Descricao = posicao.Descricao;
            novaPosicaoAtual.IDEquipamento = posicao.IDEquipamento;
            novaPosicaoAtual.Velocidade = posicao.Velocidade;
            novaPosicaoAtual.Temperatura = posicao.Temperatura;
            novaPosicaoAtual.NivelBateria = posicao.NivelBateria;
            novaPosicaoAtual.NivelSinalGPS = posicao.NivelSinalGPS;
            novaPosicaoAtual.Ignicao = posicao.Ignicao;
            novaPosicaoAtual.Veiculo = new Dominio.Entidades.Veiculo { Codigo = codigoVeiculo };
            novaPosicaoAtual.SensorTemperatura = posicao.SensorTemperatura;
            novaPosicaoAtual.EmAlvo = posicao.EmAlvo;
            novaPosicaoAtual.EmLocal = posicao.EmLocal;
            novaPosicaoAtual.Posicao = posicao;
            return novaPosicaoAtual;
        }

        /**
         * Converte um objeto POSICAO de entidade para objeto de valor
         */
        private Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao ConverterPosicaoParaObjetoDeValor(Dominio.Entidades.Embarcador.Logistica.Posicao posicao)
        {
            DateTime inicio = DateTime.UtcNow;
            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoObjetoDeValor = new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
            {
                ID = posicao.Codigo,
                Data = posicao.Data,
                DataCadastro = posicao.DataCadastro,
                DataVeiculo = posicao.DataVeiculo,
                CodigoVeiculo = posicao.Veiculo.Codigo,
                Placa = posicao.Veiculo.Placa,
                IDEquipamento = posicao.Veiculo.NumeroEquipamentoRastreador,
                Descricao = posicao.Descricao,
                Latitude = posicao.Latitude,
                Longitude = posicao.Longitude,
                Ignicao = posicao.Ignicao,
                Velocidade = posicao.Velocidade,
                Temperatura = posicao.Temperatura,
                SensorTemperatura = posicao.SensorTemperatura,
                EmAlvo = posicao.EmAlvo,
                NivelBateria = posicao.NivelBateria,
                NivelSinalGPS = posicao.NivelSinalGPS
            };
            Log("ConverterPosicaoParaObjetoDeValor", inicio, 6);
            return posicaoObjetoDeValor;
        }

        /**
         * Localiza os veículos que estão em algum contrato
         */
        private List<int> BuscarCodigosVeiculosEmContratos(Repositorio.UnitOfWork unitOfWork, DateTime dataHoraVigencia, List<int> codigosVeiculos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            DateTime inicio = DateTime.UtcNow;
            List<int> listaCodigoVeiculosEmContrato = new List<int>();
            if (configuracaoEmbarcador.MonitorarPosicaoAtualVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitorarPosicaoAtualVeiculo.PossuiContratoDeFrete ||
                configuracaoEmbarcador.MonitorarPosicaoAtualVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitorarPosicaoAtualVeiculo.ComMonitoramentoEmAndamentoOuPossuiContratoDeFrete)
            {
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                listaCodigoVeiculosEmContrato = repContratoFreteTransportador.BuscarCodigosVeiculosEmContratos(dataHoraVigencia, codigosVeiculos);
            }
            Log($"BuscarCodigosVeiculosEmContratos {listaCodigoVeiculosEmContrato.Count}", inicio, 1);
            return listaCodigoVeiculosEmContrato;
        }

        private void AdicionaNaListaMonitoramentosParaAtualizar(int codigoMonitoramento, Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao, List<MonitoramentoPendenteAtualizar> monitoramentosParaAtualizar)
        {
            if (codigoMonitoramento > 0 && posicao != null)
            {
                DateTime inicio = DateTime.UtcNow;
                int total = monitoramentosParaAtualizar.Count;
                bool encontrado = false;
                for (int i = 0; i < total; i++)
                {
                    if (monitoramentosParaAtualizar[i].codigoMonitoramento == codigoMonitoramento)
                    {
                        if (monitoramentosParaAtualizar[i].dataPosicao == null || posicao.DataVeiculo > monitoramentosParaAtualizar[i].dataPosicao)
                        {
                            monitoramentosParaAtualizar[i].dataPosicao = posicao.DataVeiculo;
                            monitoramentosParaAtualizar[i].codigoPosicao = posicao.ID;
                            monitoramentosParaAtualizar[i].temperatura = posicao.Temperatura;
                        }
                        encontrado = true;
                        break;
                    }
                }
                if (!encontrado)
                {
                    monitoramentosParaAtualizar.Add(new MonitoramentoPendenteAtualizar()
                    {
                        codigoMonitoramento = codigoMonitoramento,
                        dataPosicao = posicao.DataVeiculo,
                        codigoPosicao = posicao.ID,
                        temperatura = posicao.Temperatura
                    });
                }
                Log("AdicionaNaListaMonitoramentosParaAtualizar", inicio, 6);
            }
        }

        private void LoadClientesComGeolocalizacao(Repositorio.UnitOfWork unitOfWork)
        {
            DateTime inicio = DateTime.UtcNow;
            if (ClientesCache == null || minutosClientesCache == 0 || dataConsultaClientes == null || (dataAtual - dataConsultaClientes.Value).TotalMinutes > minutosClientesCache)
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                ClientesCache = repCliente.BuscarTodosComGeolocalizacao(false, clientesComEnderecosSecundarios, geolocalizacaoApenasJuridico);
                SubareasCache = null;
                dataConsultaClientes = dataAtual;
            }
            else
            {
                Log($"LoadClientesComGeolocalizacao Cache", inicio, 2);
            }
            Log($"LoadClientesComGeolocalizacao {ClientesCache.Length}", inicio, 1);
        }

        private void LoadLocaisComGeoLocalizacao(Repositorio.UnitOfWork unitOfWork)
        {
            DateTime inicio = DateTime.UtcNow;
            if (LocaisCache == null || minutosClientesCache == 0 || dataConsultaClientes == null || (dataAtual - dataConsultaClientes.Value).TotalMinutes > minutosClientesCache)
            {
                Repositorio.Embarcador.Logistica.Locais repLocais = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);
                LocaisCache = repLocais.BuscarPorTipoDeLocal(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.PontoDeApoio).ToArray();
            }
            else
            {
                Log($"LoadLocaisComGeoLocalizacao Cache", inicio, 2);
            }
            Log($"LoadLocaisComGeoLocalizacao {LocaisCache.Length}", inicio, 1);
        }

        /**
         * Busca as subáreas ativas dos clientes
         */
        private void LoadSubareasClientes(Repositorio.UnitOfWork unitOfWork)
        {
            DateTime inicio = DateTime.UtcNow, inicio1;
            if (SubareasCache == null)
            {
                Repositorio.Embarcador.Logistica.SubareaCliente repSubareaCliente = new Repositorio.Embarcador.Logistica.SubareaCliente(unitOfWork);
                SubareasCache = repSubareaCliente.BuscarAtivasObjetoDeValor();

                // Atribui as subáreas aos clientes
                inicio1 = DateTime.UtcNow;
                int totalClientes = ClientesCache?.Length ?? 0;
                int totalSubareas = SubareasCache?.Length ?? 0;
                for (int i = 0; i < totalClientes; i++)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.SubareaCliente> subareasCliente = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.SubareaCliente>();
                    for (int j = 0; j < totalSubareas; j++)
                    {
                        if (SubareasCache[j].CodigoCliente == ClientesCache[i].Codigo) subareasCliente.Add(SubareasCache[j]);
                    }
                    ClientesCache[i].Subareas = subareasCliente.ToArray();
                }
                Log($"LoadSubareasClientes atribuir", inicio1, 3);
            }
            else
            {
                Log($"LoadSubareasClientes Cache", inicio, 2);
            }
            Log($"LoadSubareasClientes {SubareasCache.Length}", inicio, 1);
        }

        /**
         * Verifica se a posição está no raio ou área de algum cliente
         */
        private List<Dominio.ObjetosDeValor.Cliente> BuscarClientesEmArea(Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.ObjetosDeValor.Cliente[] clientes, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            DateTime inicio = DateTime.UtcNow;
            List<Dominio.ObjetosDeValor.Cliente> clientesEmAlvo = Servicos.Embarcador.Monitoramento.Localizacao.ValidarArea.BuscarClientesEmArea(clientes, posicao.Latitude, posicao.Longitude, configuracaoEmbarcador);
            Log("BuscarClientesEmArea", inicio, 7);
            return clientesEmAlvo;
        }

        private List<Dominio.Entidades.Embarcador.Logistica.Locais> BuscarLocaisEmArea(Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.Entidades.Embarcador.Logistica.Locais[] locais)
        {
            DateTime inicio = DateTime.UtcNow;
            List<Dominio.Entidades.Embarcador.Logistica.Locais> locaisEmArea = Servicos.Embarcador.Monitoramento.Localizacao.ValidarArea.BuscarLocaisEmArea(locais, posicao.Latitude, posicao.Longitude);
            Log("BuscarLocaisEmArea", inicio, 8);
            return locaisEmArea;
        }

        /**
         * Verifica se a posição está em alguma subárea do cliente.
         */
        private Dominio.ObjetosDeValor.Embarcador.Logistica.SubareaCliente[] BuscarSubareasClienteEmArea(double codigoCliente, Dominio.ObjetosDeValor.Embarcador.Logistica.SubareaCliente[] subareas, Dominio.Entidades.Embarcador.Logistica.Posicao posicao)
        {
            DateTime inicio = DateTime.UtcNow;
            Dominio.ObjetosDeValor.Embarcador.Logistica.SubareaCliente[] subareasEmalvo = Servicos.Embarcador.Monitoramento.Localizacao.ValidarArea.BuscarSubareasClienteEmArea(subareas, posicao.Latitude, posicao.Longitude);
            Log($"BuscarSubareasClienteEmArea {codigoCliente} ", inicio, 7);
            return subareasEmalvo;
        }

        private bool ExistemEventosAtivos(Repositorio.UnitOfWork unitOfWork)
        {
            if (processarEventos)
            {
                Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                int total = repMonitoramentoEvento.BuscarTotalAtivos();
                return total > 0;
            }
            return false;
        }


        private bool ExistemEventosAtivosDePercaOuSemSinal(Repositorio.UnitOfWork unitOfWork)
        {
            if (processarEventos)
            {

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta>();
                tiposAlerta.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemSinal);
                tiposAlerta.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.PerdaDeSinal);

                Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                int total = repMonitoramentoEvento.BuscarTotalAtivosPorTipo(tiposAlerta);
                return total > 0;
            }
            return false;
        }

        /**
         * Percorre os eventos ativos e executa o método "Processar"
         */
        private void ProcessarEventos(List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendencia> monitoramentoPendenciasEventos, Dominio.Entidades.Embarcador.Logistica.Posicao posicao, int codigoMonitoramento)
        {
            monitoramentoPendenciasEventos.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendencia()
            {
                Data = dataAtual,
                PosicaoAtual = posicao.Codigo,
                Monitoramento = codigoMonitoramento
            });
        }

        /**
         * Adiciona um veículo à lista de cache
         */
        private VeiculoPosicao AddVeiculoCache(int codigoVeiculo)
        {
            VeiculoPosicao veiculoPosicao = ObtemVeiculoPosicaoCache(codigoVeiculo);
            if (veiculoPosicao == null)
            {
                veiculoPosicao = new VeiculoPosicao(codigoVeiculo);
                this.VeiculosPosicoesCache.Add(veiculoPosicao);
            }
            return veiculoPosicao;
        }

        /**
         * Adiciona uma posição à lista de posições em cache
         */
        private void AddPosicaoCache(int codigoVeiculo, Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao novaPosicao)
        {
            DateTime inicio = DateTime.UtcNow;
            VeiculoPosicao veiculoPosicao = AddVeiculoCache(codigoVeiculo);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = veiculoPosicao.Posicoes.ToList();
            posicoes.Add(novaPosicao);
            posicoes.Sort((x, y) => x.DataVeiculo.CompareTo(y.DataVeiculo));
            veiculoPosicao.Posicoes = posicoes.ToArray();
            Log($"AddPosicaoCache {codigoVeiculo} {veiculoPosicao.Posicoes.Length}", inicio, 5);
        }

        /**
         * Carrega a lista de posições do veículo para a lista de posições em cache
         */
        private void LoadPosicoesCache(Repositorio.UnitOfWork unitOfWork, int codigoVeiculo)
        {
            VeiculoPosicao veiculoPosicao = ObtemVeiculoPosicaoCache(codigoVeiculo);
            if (veiculoPosicao == null)
            {
                DateTime inicio = DateTime.UtcNow;
                Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = repPosicao.BuscarWaypointsPorVeiculoDataInicialeFinal(codigoVeiculo, this.dataAtual.AddHours(horasPosicoesCache), this.dataAtual);
                Log($"LoadPosicoesCache {posicoes.Count}", inicio, 5);

                inicio = DateTime.UtcNow;
                posicoes.Sort((x, y) => x.DataVeiculo.CompareTo(y.DataVeiculo));
                Log($"LoadPosicoesCache Sort", inicio, 5);

                this.VeiculosPosicoesCache.Add(new VeiculoPosicao(codigoVeiculo, posicoes.ToArray()));

            }
        }

        /**
         * Carrega a lista de posições dos veículos para a lista de posições em cache
         */
        private void LoadPosicoesVeiculosCache(Repositorio.UnitOfWork unitOfWork, List<int> codigosVeiculos)
        {
            DateTime inicio = DateTime.UtcNow, inicio1;

            VeiculoPosicao veiculoPosicao;
            List<int> codigosVeiculosSemCache = new List<int>();

            // Gera uma lista com os veículos que ainda não estão no cache
            int total = codigosVeiculos.Count;
            for (int i = 0; i < total; i++)
            {
                veiculoPosicao = ObtemVeiculoPosicaoCache(codigosVeiculos[i]);
                if (veiculoPosicao == null) codigosVeiculosSemCache.Add(codigosVeiculos[i]);
            }

            total = codigosVeiculosSemCache.Count;
            if (total > 0)
            {

                // Consulta todas as posições dos veículos de uma unica vez
                inicio1 = DateTime.UtcNow;
                Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = repPosicao.BuscarWaypointsPorVeiculosDataInicialeFinal(codigosVeiculosSemCache, this.dataAtual.AddHours(horasPosicoesCache), this.dataAtual);
                int totalp = posicoes.Count;
                Log($"BuscarPorVeiculosDataInicialeFinal {total} {totalp}", inicio1, 3);

                inicio1 = DateTime.UtcNow;
                if (totalp > 0)
                {
                    // Separa as posições recebidas por veículo e adiciona ao cache
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo;
                    for (int i = 0; i < total; i++)
                    {
                        posicoesVeiculo = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
                        for (int j = 0; j < totalp; j++)
                        {
                            if (posicoes[j].CodigoVeiculo == codigosVeiculosSemCache[i]) posicoesVeiculo.Add(posicoes[j]);
                        }
                        this.VeiculosPosicoesCache.Add(new VeiculoPosicao(codigosVeiculosSemCache[i], posicoesVeiculo.ToArray()));
                    }
                }
                Log($"Separar posicoes recebidas por veiculo", inicio1, 4);
            }
            Log($"LoadPosicoesVeiculosCache", inicio, 1);
        }

        /**
         * Carrega a lista de posições do veículo para a lista de posições em cache
         */
        private void LoadPosicoesAtuaisCache(Repositorio.UnitOfWork unitOfWork, List<int> codigosVeiculos)
        {
            DateTime inicio = DateTime.UtcNow;
            Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
            this.PosicoesAtuaisCache.Clear();
            this.PosicoesAtuaisCache = repPosicaoAtual.BuscarPorVeiculos(codigosVeiculos);
            Log($"LoadPosicoesAtuaisCache {codigosVeiculos.Count} {this.PosicoesAtuaisCache.Count}", inicio, 1);
        }

        /**
         * Remove as posições antigas do vetor de posições em cache e os veículos 
         */
        private void SanitizeCache(DateTime dataLimite)
        {
            if (dataSanitizeCache == null || dataSanitizeCache.AddMinutes(5) < this.dataAtual)
            {
                dataSanitizeCache = this.dataAtual;

                // Reduz a quantidade de horas que as posições devem ser removidas do cache
                dataLimite = dataLimite.AddHours(horasPosicoesCache);

                // Recria o cache com os veículos e respectivas posições com data válida
                List<VeiculoPosicao> newVeiculosPosicoes = new List<VeiculoPosicao>();
                int conti = this.VeiculosPosicoesCache.Count;
                for (int i = 0; i < conti; i++)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
                    int contj = this.VeiculosPosicoesCache[i].Posicoes?.Length ?? 0;
                    for (int j = 0; j < contj; j++)
                    {
                        if (this.VeiculosPosicoesCache[i].Posicoes[j].DataVeiculo > dataLimite)
                        {
                            posicoes.Add(this.VeiculosPosicoesCache[i].Posicoes[j]);
                        }
                    }

                    // Há posições ainda válidas
                    if (posicoes.Count > 0)
                    {
                        newVeiculosPosicoes.Add(new VeiculoPosicao(this.VeiculosPosicoesCache[i].CodigoVeiculo, posicoes.ToArray()));
                    }
                }
                this.VeiculosPosicoesCache.Clear();
                this.VeiculosPosicoesCache = newVeiculosPosicoes;

                // Recria o cache com as cargas e clientes do ponto de passagem
                List<CargaClientePassagem> newCargasClientesPassagem = new List<CargaClientePassagem>();
                int cont = this.CargasClientesPassagemCache.Count;
                for (int i = 0; i < cont; i++)
                {
                    if (this.CargasClientesPassagemCache[i].Data > dataLimite)
                    {
                        newCargasClientesPassagem.Add(this.CargasClientesPassagemCache[i]);
                    }
                }
                this.CargasClientesPassagemCache.Clear();
                this.CargasClientesPassagemCache = newCargasClientesPassagem;
            }
        }

        /**
         * Processa a posição atual para incluir ou alterar indicando o status da viagem
         */
        private Dominio.Entidades.Embarcador.Logistica.PosicaoAtual ProcessaPosicaoAtual(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual, Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao status)
        {
            DateTime inicio = DateTime.UtcNow;

            Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);

            // Ainda não existe posição atual para o veículo
            if (posicaoAtual == null)
            {
                posicaoAtual = new Dominio.Entidades.Embarcador.Logistica.PosicaoAtual();
                NovaPosicaoAtualApartirDaPosicao(posicaoAtual, posicao, posicao.Veiculo.Codigo);
                posicaoAtual.Status = status;
                repPosicaoAtual.Inserir(posicaoAtual);
                this.PosicoesAtuaisCache.Add(posicaoAtual);
            }

            // ... considera a posição recebida como posição atual apenas se for mais nova que a posição atual já existente (Há casos de posições recebidas fora de ordem)
            else if (posicao.DataVeiculo > posicaoAtual.DataVeiculo)
            {
                NovaPosicaoAtualApartirDaPosicao(posicaoAtual, posicao, posicao.Veiculo.Codigo);
                posicaoAtual.Status = status;
                repPosicaoAtual.Atualizar(posicaoAtual);
            }

            if (processarPosicoesDemaisPlacas)
                ValidarPosicaoAtualDemaisVeiculosDeMesmaPlaca(unitOfWork, posicaoAtual, posicao, status);

            Log("ProcessaPosicaoAtual", inicio, 6);

            return posicaoAtual;
        }

        private void ValidarPosicaoAtualDemaisVeiculosDeMesmaPlaca(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual, Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPosicao status)
        {
            DateTime inicio = DateTime.UtcNow;

            Log("ValidarPosicaoAtualDemaisVeiculosDeMesmaPlaca", inicio, 6);

            Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

            List<int> veiculosMesmaPlaca = new List<int>();
            veiculosMesmaPlaca = repVeiculo.BuscarCodigoVeiculoPorPlaca(posicaoAtual.Veiculo.Placa);

            foreach (var veiculo in veiculosMesmaPlaca)
            {
                if (veiculo != posicaoAtual.Veiculo.Codigo)
                {
                    Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtualVeiculoMesmaPlaca = ObtemPosicaoAtualDoCache(veiculo);

                    if (posicaoAtualVeiculoMesmaPlaca == null)
                    {
                        posicaoAtualVeiculoMesmaPlaca = new Dominio.Entidades.Embarcador.Logistica.PosicaoAtual();
                        NovaPosicaoAtualApartirDaPosicao(posicaoAtualVeiculoMesmaPlaca, posicao, veiculo);
                        posicaoAtualVeiculoMesmaPlaca.Veiculo = new Dominio.Entidades.Veiculo { Codigo = veiculo };
                        posicaoAtualVeiculoMesmaPlaca.Status = status;
                        repPosicaoAtual.Inserir(posicaoAtualVeiculoMesmaPlaca);

                        this.PosicoesAtuaisCache.Add(posicaoAtualVeiculoMesmaPlaca);
                    }

                    // ... considera a posição recebida como posição atual apenas se for mais nova que a posição atual já existente (Há casos de posições recebidas fora de ordem)
                    else if (posicao.DataVeiculo > posicaoAtualVeiculoMesmaPlaca.DataVeiculo)
                    {
                        NovaPosicaoAtualApartirDaPosicao(posicaoAtualVeiculoMesmaPlaca, posicao, veiculo);
                        posicaoAtualVeiculoMesmaPlaca.Status = status;
                        posicaoAtualVeiculoMesmaPlaca.Veiculo = new Dominio.Entidades.Veiculo { Codigo = veiculo };
                        repPosicaoAtual.Atualizar(posicaoAtualVeiculoMesmaPlaca);
                    }
                }
            }

            DateTime fim = DateTime.UtcNow;

            Log("ValidarPosicaoAtualDemaisVeiculosDeMesmaPlaca", fim, 6);

        }


        /**
         * Processa uma troca de alvo com a posição imediatamente anterior à posição atual
         */
        private void ProcessaTrocaDeAlvo(Repositorio.UnitOfWork unitOfWork, List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendencia> monitoramentoTrocasDeAlvo, int codigoMonitoramento, Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            DateTime inicio = DateTime.UtcNow;
            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoAnterior = ObtemPosicaoAnterior(unitOfWork, posicao);
            ProcessaTrocaDeAlvo(monitoramentoTrocasDeAlvo, codigoMonitoramento, posicaoAnterior, posicao, configuracao);
            Log("ProcessaTrocaDeAlvo", inicio, 6);
        }

        /**
         * As posições podem chegar fora de ordem. Desta forma, após processar todas as posições recebidas de um veículo, 
         * verifica se existe uma posição mais atual que a última que acabou de ser processada. Se houver, deve ser 
         * processada uma possível troca de alvo para atualização das datas entrada e saída.
         */
        private void ProcessarTrocaDeAlvoDaUltimaPosicao(Repositorio.UnitOfWork unitOfWork, List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendencia> monitoramentoTrocasDeAlvo, Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao ultimaPosicaoProcessada, Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> monitoramentosAbertos, List<MonitoramentoPendenteAtualizar> monitoramentosParaAtualizar, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (ultimaPosicaoProcessada == null) return;

            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoPosterior = ObtemPosicaoPosterior(unitOfWork, ultimaPosicaoProcessada, posicaoAtual);
            if (posicaoPosterior != null)
            {
                // Busca algum monitoramento aberto do cache
                Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento monitoramento = BuscarMonitoramentoEmAberto(monitoramentosAbertos, posicaoPosterior);
                if (monitoramento != null)
                {
                    AdicionaNaListaMonitoramentosParaAtualizar(monitoramento.Codigo, posicaoPosterior, monitoramentosParaAtualizar);
                    ProcessaTrocaDeAlvo(monitoramentoTrocasDeAlvo, monitoramento.Codigo, ultimaPosicaoProcessada, posicaoPosterior, configuracao);
                }
            }
        }

        /**
         * Processa uma possível troca de alvo
         */
        private void ProcessaTrocaDeAlvo(List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendencia> monitoramentoPendenciasTrocasDeAlvo, int codigoMonitoramento, Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoAnterior, Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoAtual, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            if (codigoMonitoramento > 0)
            {
                monitoramentoPendenciasTrocasDeAlvo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendencia()
                {
                    Data = dataAtual,
                    Monitoramento = codigoMonitoramento,
                    PosicaoAtual = posicaoAtual?.ID ?? 0,
                    PosicaoAnterior = posicaoAnterior?.ID ?? 0
                });
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> BuscarMonitoramentosAbertos(Repositorio.UnitOfWork unitOfWork, List<int> codigosVeiculos, List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes)
        {
            DateTime inicio = DateTime.UtcNow;

            // As posições estão ordenadas pela DataVeiculo, de forma crescente
            DateTime dataInicio = posicoes.First().DataVeiculo;
            if (dataInicio < this.dataMenorValidade) dataInicio = this.dataMenorValidade;

            Log($"BuscarMonitoramentosAbertos {dataInicio} a {dataAtual}", 2);
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> monitoramentos = repMonitoramento.BuscarMonitoramentosEmAbertoNoPeriodo(codigosVeiculos, dataInicio, dataAtual);
            Log($"BuscarMonitoramentosAbertos {monitoramentos.Count}", inicio, 1);
            return monitoramentos;
        }

        /**
         * Busca um possível monitoramento em aberto do veículo em uma determinada data
         */
        private Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento BuscarMonitoramentoEmAberto(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento> monitoramentos, Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao)
        {
            DateTime inicio = DateTime.UtcNow;
            Dominio.ObjetosDeValor.Embarcador.Logistica.Monitoramento monitoramento = null;
            if (monitoramentos != null)
            {
                int total = monitoramentos.Count;
                for (int i = 0; i < total; i++)
                {
                    if (monitoramentos[i].Veiculo > 0 && monitoramentos[i].Veiculo == posicao.CodigoVeiculo &&
                        (
                            (
                                monitoramentos[i].Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado &&
                                monitoramentos[i].DataInicioMonitoramento <= posicao.DataVeiculo
                            )
                            ||
                            (
                                monitoramentos[i].Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado &&
                                posicao.DataVeiculo >= monitoramentos[i].DataInicioMonitoramento && posicao.DataVeiculo <= monitoramentos[i].DataInicioMonitoramento
                            )
                        )
                    )
                    {
                        monitoramento = monitoramentos[i];
                        break;
                    }
                }
            }
            Log($"BuscarMonitoramentoEmAbertoCache {posicao.CodigoVeiculo}", inicio, 6);
            return monitoramento;
        }

        /**
         * Localiza, no cache, a posição imediatamente anterior à posição
         */
        private Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao ObtemPosicaoAnterior(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao)
        {
            DateTime inicio = DateTime.UtcNow, inicio1;
            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoAnterior = null;
            if (posicao != null)
            {
                VeiculoPosicao veiculoPosicao = ObtemVeiculoPosicaoCache(posicao.CodigoVeiculo);
                if (veiculoPosicao != null)
                {

                    // Percorre a lista ordenada por DataVeiculo
                    int count = veiculoPosicao.Posicoes?.Length ?? 0;
                    for (int i = 0; i < count; i++)
                    {
                        if (veiculoPosicao.Posicoes[i].DataVeiculo < posicao.DataVeiculo) posicaoAnterior = veiculoPosicao.Posicoes[i];
                    }
                }

                if (posicaoAnterior == null)
                {
                    inicio1 = DateTime.UtcNow;
                    Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                    posicaoAnterior = repPosicao.BuscarWaypointPosicaoVizinha(posicao.DataVeiculo, posicao.CodigoVeiculo, true);
                    Log("BuscarWaypointPosicaoVizinha", inicio1, 7);
                }
            }
            Log("ObtemPosicaoAnterior", inicio, 6);
            return posicaoAnterior;
        }

        /**
         * Localiza, no cache, a posição imediatamente posterior à posição
         */
        private Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao ObtemPosicaoPosterior(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao, Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual)
        {
            DateTime inicio = DateTime.UtcNow, inicio1;
            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoPosterior = null;
            if (posicao != null)
            {
                if (posicaoAtual == null || posicaoAtual.DataVeiculo > posicao.DataVeiculo)
                {
                    VeiculoPosicao veiculoPosicao = ObtemVeiculoPosicaoCache(posicao.CodigoVeiculo);
                    if (veiculoPosicao != null)
                    {

                        // Percorre a lista ordenada por DataVeiculo
                        int count = veiculoPosicao.Posicoes?.Length ?? 0;
                        for (int i = 0; i < count; i++)
                        {
                            // Encontrou uma data maior
                            if (veiculoPosicao.Posicoes[i].DataVeiculo > posicao.DataVeiculo)
                            {
                                posicaoPosterior = veiculoPosicao.Posicoes[i];
                                break;
                            }
                        }

                        if (posicaoPosterior == null)
                        {
                            inicio1 = DateTime.UtcNow;
                            Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                            posicaoPosterior = repPosicao.BuscarWaypointPosicaoVizinha(posicao.DataVeiculo, posicao.CodigoVeiculo, false);
                            Log("BuscarWaypointPosicaoVizinha", inicio1, 7);
                        }
                    }
                }
            }
            Log("ObtemPosicaoPosterior", inicio, 6);
            return posicaoPosterior;
        }

        /**
         * Localiza, no cache, as posições do veículo
         */
        private VeiculoPosicao ObtemVeiculoPosicaoCache(int codigoVeiculo)
        {
            int total = this.VeiculosPosicoesCache.Count;
            for (int i = 0; i < total; i++)
            {
                if (this.VeiculosPosicoesCache[i].CodigoVeiculo == codigoVeiculo)
                {
                    return this.VeiculosPosicoesCache[i];
                }
            }
            return null;
        }

        /**
         * Entre uma lista de posições, extrai as posições de um determinado veículo
         */
        private List<Dominio.Entidades.Embarcador.Logistica.Posicao> ObtemPosicoesDoVeiculo(int codigoVeiculo, List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes)
        {
            List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoesVeiculo = new List<Dominio.Entidades.Embarcador.Logistica.Posicao>();
            int cont = posicoes.Count;
            for (int i = 0; i < cont; i++)
            {
                if (posicoes[i].Veiculo != null && posicoes[i].Veiculo.Codigo == codigoVeiculo)
                {
                    posicoesVeiculo.Add(posicoes[i]);
                }
            }
            return posicoesVeiculo;
        }

        /**
         * Busca a posição atual do veículo do cache
         */
        private Dominio.Entidades.Embarcador.Logistica.PosicaoAtual ObtemPosicaoAtualDoCache(int codigoVeiculo)
        {
            DateTime inicio = DateTime.UtcNow;
            Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posicaoAtual = null;
            int cont = this.PosicoesAtuaisCache.Count;
            for (int i = 0; i < cont; i++)
            {
                if (PosicoesAtuaisCache[i].Veiculo != null && PosicoesAtuaisCache[i].Veiculo.Codigo == codigoVeiculo)
                {
                    posicaoAtual = PosicoesAtuaisCache[i];
                    break;
                }
            }
            Log("ObtemPosicaoAtualDoCache", inicio, 6);
            return posicaoAtual;
        }

        /**
         * Busca a posição atual do veículo do cache
         */
        private DateTime ObtemMaiorDataVeiculoDasPosicoes(List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes)
        {
            DateTime maiorDataVeiculo = DateTime.MinValue;
            int cont = posicoes.Count;
            for (int i = 0; i < cont; i++)
            {
                if (posicoes[i].DataVeiculo > maiorDataVeiculo)
                {
                    maiorDataVeiculo = posicoes[i].DataVeiculo;
                }
            }
            return maiorDataVeiculo;
        }

        /**
         * Extrai os códigos únicos dos veículos envolvidos nas posições recebidas
         */
        private List<int> ObtemCodigosVeiculosDistintos(List<Dominio.Entidades.Embarcador.Logistica.Posicao> posicoes)
        {
            List<int> codigosVeiculos = new List<int>();
            int cont = posicoes.Count;
            for (int i = 0; i < cont; i++)
            {
                if (posicoes[i].Veiculo != null && !codigosVeiculos.Contains(posicoes[i].Veiculo.Codigo))
                {
                    codigosVeiculos.Add(posicoes[i].Veiculo.Codigo);
                }
            }
            Log($"{codigosVeiculos.Count} veiculos", 1);
            return codigosVeiculos;
        }

        private bool ConfirmarValidadeDaPosicao(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Logistica.Posicao repPosicao, Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoObjetoDeValor, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            if (this.confirmarValidadeDaPosicao)
            {

                DateTime inicio = DateTime.UtcNow;
                dynamic retorno;

                // Verifica a validade da posição
                retorno = VerificaValidadeDataVeiculo(posicaoObjetoDeValor, unitOfWork);

                // Verifica as coordenadas
                if (retorno.Status) retorno = VerificaCoordenadas(posicaoObjetoDeValor);

                // Verifica o deslocamento em relação as posições anteriores
                if (retorno.Status) retorno = VerificaValidadeDistanciaTempoVelocidade(unitOfWork, posicaoObjetoDeValor, configuracaoEmbarcador);

                // Posição inválida
                if (!retorno.Status)
                {
                    // Inclui no histórico das posições inválidas
                    if (this.registrarPosicaoInvalida)
                    {
                        Repositorio.Embarcador.Logistica.PosicaoInvalida repPosicaoInvalida = new Repositorio.Embarcador.Logistica.PosicaoInvalida(unitOfWork);
                        Dominio.Entidades.Embarcador.Logistica.PosicaoInvalida posicaoInvalida = new Dominio.Entidades.Embarcador.Logistica.PosicaoInvalida();
                        posicaoInvalida.Descricao = posicao.Descricao;
                        posicaoInvalida.Data = posicao.Data;
                        posicaoInvalida.DataVeiculo = posicao.DataVeiculo;
                        posicaoInvalida.DataCadastro = posicao.DataCadastro;
                        posicaoInvalida.Veiculo = posicao.Veiculo;
                        posicaoInvalida.IDEquipamento = posicao.IDEquipamento;
                        posicaoInvalida.Latitude = posicao.Latitude;
                        posicaoInvalida.Longitude = posicao.Longitude;
                        posicaoInvalida.Ignicao = posicao.Ignicao;
                        posicaoInvalida.Velocidade = posicao.Velocidade;
                        posicaoInvalida.Temperatura = posicao.Temperatura;
                        posicaoInvalida.SensorTemperatura = posicao.SensorTemperatura;
                        posicaoInvalida.Motivo = retorno.Msg;
                        repPosicaoInvalida.Inserir(posicaoInvalida);
                    }

                    // Exclui a posição
                    if (excluirPosicaoInvalida)
                    {
                        repPosicao.Deletar(posicao);
                    }
                    else
                    {
                        posicao.Processar = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Processado;
                        repPosicao.Atualizar(posicao);
                    }
                }

                Log($"ConfirmarValidadeDaPosicao", inicio, 5);
                return retorno.Status;
            }
            return true;
        }

        /**
         * Verifica o deslocamento em relação as posições anteriores. Se a posição recebida está muito distante da 
         * posição anterior com uma diferença pequena de tempo, indica uma velocidade impossível de ser atingida
         */
        private dynamic VerificaValidadeDistanciaTempoVelocidade(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            DateTime inicio = DateTime.UtcNow;
            bool status = true;
            string msg = null;

            // A validação está ativada?
            if (posicao != null && configuracaoEmbarcador.VelocidadeMaximaExtremaEntrePosicoes > 0)
            {

                // Busca a posição imediatamente anterior para ser possível analisar o deslocamento
                Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicaoAnterior = ObtemPosicaoAnterior(unitOfWork, posicao);
                if (posicaoAnterior != null)
                {

                    // Calcula a distância percorrida
                    double distancia = Servicos.Embarcador.Logistica.Distancia.CalcularDistanciaKM(posicaoAnterior.Latitude, posicaoAnterior.Longitude, posicao.Latitude, posicao.Longitude);

                    // Diferença de tempo entre
                    TimeSpan tempo = posicao.DataVeiculo - posicaoAnterior.DataVeiculo;

                    // Cálculo da velocidade média
                    int velocidade = 0;
                    if (tempo.TotalHours > 0)
                    {
                        velocidade = (int)(distancia / Math.Abs(tempo.TotalHours));
                    }
                    else if (distancia > 0)
                    {
                        velocidade = 9999;
                    }

                    // Se a velocidade for maior que a velocidade extrema configurada
                    if (velocidade >= configuracaoEmbarcador.VelocidadeMaximaExtremaEntrePosicoes)
                    {
                        status = false;
                        msg = $"Atingido {velocidade}Km/h ({Math.Round(distancia, 2)}Km em {tempo}), " +
                                $"de ({posicaoAnterior.Latitude};{posicaoAnterior.Longitude}) até ({posicao.Latitude};{posicao.Longitude}), " +
                                $"entre {posicaoAnterior.DataVeiculo.ToString("dd/MM/yyyy HH:mm:ss")} e {posicao.DataVeiculo.ToString("dd/MM/yyyy HH:mm:ss")}";
                    }

                }

            }
            Log($"VerificaValidadeDistanciaTempoVelocidade", inicio, 6);
            return new
            {
                Status = status,
                Msg = msg
            };
        }

        /**
         * Verifica a data da posição recebida, se for muito antiga, de acordo com a configuração
         */
        private dynamic VerificaValidadeDataVeiculo(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao, Repositorio.UnitOfWork unitOfWork)
        {
            bool status = true;
            string msg = null;
            if (posicao != null)
            {
                //if (possuiAlertaPerdaOuSemSinal && posicao.DataVeiculo < this.dataAtual.AddMinutes(-15)) //posição retroativa de pelo menos 15 minutos.
                //{
                //    Servicos.Embarcador.Monitoramento.PerdaSinalMonitoramento servPerdaSinal = new Servicos.Embarcador.Monitoramento.PerdaSinalMonitoramento(unitOfWork);
                //    servPerdaSinal.VerificarRegistrosPerdaSinalPosicaoRetroativa(posicao);
                //}

                if (posicao.DataVeiculo < this.dataMenorValidade)
                {
                    status = false;
                    msg = $"Posição de {posicao.DataVeiculo.ToString("dd/MM/yyyy HH:mm:ss")}, antes de {Math.Abs(this.horasParaExpirarPosicoes)}h.";
                }
                else if (posicao.DataVeiculo > this.dataAtual)
                {
                    status = false;
                    msg = $"Posição de {posicao.DataVeiculo.ToString("dd/MM/yyyy HH:mm:ss")}, futura, maior que a data atual {this.dataAtual.ToString("dd/MM/yyyy HH:mm:ss")}.";
                }
            }
            return new
            {
                Status = status,
                Msg = msg
            };
        }

        /**
         * Verifica a data da posição recebida, se for muito antiga, de acordo com a configuração
         */
        private dynamic VerificaCoordenadas(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao)
        {
            bool status = true;
            string msg = null;
            if (posicao != null && !Servicos.Embarcador.Logistica.WayPointUtil.ValidarWayPoint(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(posicao.Latitude, posicao.Longitude)))
            {
                status = false;
                msg = $"Posição de {posicao.DataVeiculo.ToString("dd/MM/yyyy HH:mm:ss")}, com coordenadas inválidas: {posicao.Latitude}, {posicao.Longitude}.";
            }
            return new
            {
                Status = status,
                Msg = msg
            };
        }

        private bool VerificaSeDeveAtualizarPosicaoAtual(int codigoVeiculo, int codigoMonitoramento, List<int> listaCodigosVeiculosEmContrato, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            switch (configuracaoEmbarcador.MonitorarPosicaoAtualVeiculo)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitorarPosicaoAtualVeiculo.Todos:
                    return true;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitorarPosicaoAtualVeiculo.ComMonitoramentoEmAndamento:
                    return codigoMonitoramento > 0;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitorarPosicaoAtualVeiculo.PossuiContratoDeFrete:
                    return listaCodigosVeiculosEmContrato.Contains(codigoVeiculo);

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitorarPosicaoAtualVeiculo.ComMonitoramentoEmAndamentoOuPossuiContratoDeFrete:
                    return codigoMonitoramento > 0 || listaCodigosVeiculosEmContrato.Contains(codigoVeiculo);

            }
            return false;
        }

        private void IndentificarLocal(Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            if (!GerarPermanenciaLocais)
                return;

            DateTime inicio = DateTime.UtcNow;
            posicao.EmLocal = false;
            List<Dominio.Entidades.Embarcador.Logistica.Locais> listaLocais = BuscarLocaisEmArea(posicao, LocaisCache);

            listaLocais = listaLocais.Distinct().ToList();
            int total = listaLocais.Count;
            if (total > 0)
            {
                posicao.EmLocal = true;
                foreach (var local in listaLocais)
                {
                    Dominio.Entidades.Embarcador.Logistica.PosicaoLocal posicaoLocal = new Dominio.Entidades.Embarcador.Logistica.PosicaoLocal();
                    posicaoLocal.Posicao = posicao;
                    posicaoLocal.Local = local;
                    PosicaoLocais.Add(posicaoLocal);
                }
            }
            Log("IdentificarLocal", inicio, 6);
        }


        private void IdentificarAlvos(Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            DateTime inicio = DateTime.UtcNow;
            posicao.EmAlvo = false;
            Dominio.ObjetosDeValor.Cliente[] ListaClientesFiltrada = ClientesCache;
            if (distancaiAlvosMetros > 0)
                ListaClientesFiltrada = (from obj in ClientesCache where distancaiAlvosMetros >= Servicos.Embarcador.Logistica.Polilinha.CalcularDistancia(obj.Latitude, obj.Longitude, posicao.Latitude, posicao.Longitude) select obj).ToArray();

            List<Dominio.ObjetosDeValor.Cliente> clientesEmAlvo = BuscarClientesEmArea(posicao, ListaClientesFiltrada, configuracaoEmbarcador);
            clientesEmAlvo = clientesEmAlvo.Distinct().ToList();
            int total = clientesEmAlvo.Count;
            if (total > 0)
            {
                posicao.EmAlvo = true;
                for (int i = 0; i < total; i++)
                {
                    PosicaAlvoPosicaoAlvoSubarea posicaoAlvoPosicaoAlvoSubarea = new PosicaAlvoPosicaoAlvoSubarea();

                    posicaoAlvoPosicaoAlvoSubarea.PosicaoAlvo = new Dominio.Entidades.Embarcador.Logistica.PosicaoAlvo()
                    {
                        Posicao = posicao,
                        Cliente = new Dominio.Entidades.Cliente { CPF_CNPJ = clientesEmAlvo[i].Codigo }
                    };

                    if ((clientesEmAlvo[i].Subareas?.Length ?? 0) > 0)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Logistica.SubareaCliente[] subareasEmAlvo = BuscarSubareasClienteEmArea(clientesEmAlvo[i].Codigo, clientesEmAlvo[i].Subareas, posicao);
                        int totalSubarea = subareasEmAlvo?.Length ?? 0;
                        for (int j = 0; j < totalSubarea; j++)
                        {
                            Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea posicaoAlvoSubarea = new Dominio.Entidades.Embarcador.Logistica.PosicaoAlvoSubarea()
                            {
                                SubareaCliente = new Dominio.Entidades.Embarcador.Logistica.SubareaCliente { Codigo = subareasEmAlvo[j].Codigo }
                            };
                            posicaoAlvoPosicaoAlvoSubarea.PosicaoAlvoSubareas.Add(posicaoAlvoSubarea);
                        }
                    }
                    PosicaoAlvosPosicaoAlvosSubareas.Add(posicaoAlvoPosicaoAlvoSubarea);
                }
            }
            Log("IdentificarAlvos", inicio, 6);
        }

        private void SalvarPosicaoAlvos(Repositorio.UnitOfWork unitOfWork)
        {
            int total = this.PosicaoAlvosPosicaoAlvosSubareas.Count;
            if (total > 0)
            {
                DateTime inicio = DateTime.UtcNow;
                Repositorio.Embarcador.Logistica.PosicaoAlvo repPosicaoAlvo = new Repositorio.Embarcador.Logistica.PosicaoAlvo(unitOfWork);
                Repositorio.Embarcador.Logistica.PosicaoAlvoSubarea repPosicaoAlvoSubarea = new Repositorio.Embarcador.Logistica.PosicaoAlvoSubarea(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    repPosicaoAlvo.Inserir(PosicaoAlvosPosicaoAlvosSubareas[i].PosicaoAlvo);
                    int totalSubareas = PosicaoAlvosPosicaoAlvosSubareas[i].PosicaoAlvoSubareas.Count;
                    for (int j = 0; j < totalSubareas; j++)
                    {
                        PosicaoAlvosPosicaoAlvosSubareas[i].PosicaoAlvoSubareas[j].PosicaoAlvo = PosicaoAlvosPosicaoAlvosSubareas[i].PosicaoAlvo;
                        repPosicaoAlvoSubarea.Inserir(PosicaoAlvosPosicaoAlvosSubareas[i].PosicaoAlvoSubareas[j]);
                    }
                }
                Log("SalvarPosicaoAlvos", inicio, 4);
            }
        }

        private void SalvarPosicaoLocal(Repositorio.UnitOfWork unitOfWork)
        {
            if (!GerarPermanenciaLocais)
                return;

            int total = this.PosicaoLocais.Count;
            if (total > 0)
            {
                DateTime inicio = DateTime.UtcNow;
                Repositorio.Embarcador.Logistica.PosicaoLocal repPosicaoLocal = new Repositorio.Embarcador.Logistica.PosicaoLocal(unitOfWork);
                for (int i = 0; i < total; i++)
                    repPosicaoLocal.Inserir(PosicaoLocais[i]);

                Log("SalvarPosicaoLocal", inicio, 5);
            }
        }

        private void SalvarMonitoramentoTrocasDeAlvo(List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendencia> monitoramentoPendenciasTrocasDeAlvo)
        {
            DateTime inicio = DateTime.UtcNow;
            base.SalvarPendenciasFila(this.processarTrocaDeAlvoDiretorioFila, this.processarTrocaDeAlvoArquivoFilaPrefixo, this.dataAtual, monitoramentoPendenciasTrocasDeAlvo);
            Log($"SalvarMonitoramentosTrocasDeAlvo {monitoramentoPendenciasTrocasDeAlvo?.Count ?? 0} registradas", inicio, 2);
        }

        private void SalvarMonitoramentoProcessarEventos(List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoPendencia> monitoramentoPendenciasEventos)
        {
            DateTime inicio = DateTime.UtcNow;
            base.SalvarPendenciasFila(this.processarEventosDiretorioFila, this.processarEventosArquivoFilaPrefixo, this.dataAtual, monitoramentoPendenciasEventos);
            Log($"SalvarMonitoramentoProcessarEventos {monitoramentoPendenciasEventos?.Count ?? 0} registradas", inicio, 2);
        }

        private void SalvarMonitoramentosVeiculosPosicoes(Repositorio.UnitOfWork unitOfWork, List<MonitoramentoVeiculoPosicao> monitoramentosVeiculosPosicoesPendentes)
        {
            int total = monitoramentosVeiculosPosicoesPendentes?.Count ?? 0;
            Log($"SalvarMonitoramentosVeiculosPosicoes {total} posicoes para relacionar", 1);
            if (total > 0)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoVeiculo> monitoramentosVeiculosCache = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoVeiculo>();
                DateTime inicio = DateTime.UtcNow;
                Repositorio.Embarcador.Logistica.MonitoramentoVeiculoPosicao repMonitoramentoVeiculoPosicao = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculoPosicao(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoVeiculo monitoramentoVeiculo = ObtemMonitoramentoVeiculo(unitOfWork, monitoramentosVeiculosPosicoesPendentes[i].codigoMonitoramento, monitoramentosVeiculosPosicoesPendentes[i].codigoVeiculo, monitoramentosVeiculosCache);
                    if (monitoramentoVeiculo != null)
                    {
                        Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao monitoramentoVeiculoPosicao = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculoPosicao()
                        {
                            MonitoramentoVeiculo = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo { Codigo = monitoramentoVeiculo.Codigo },
                            Posicao = new Dominio.Entidades.Embarcador.Logistica.Posicao { Codigo = monitoramentosVeiculosPosicoesPendentes[i].codigoPosicao }
                        };
                        repMonitoramentoVeiculoPosicao.Inserir(monitoramentoVeiculoPosicao);
                    }
                }
                Log("SalvarMonitoramentosVeiculosPosicoes", inicio, 2);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoVeiculo ObtemMonitoramentoVeiculo(Repositorio.UnitOfWork unitOfWork, int codigoMonitoramento, int codigoVeiculo, List<Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoVeiculo> monitoramentosVeiculos)
        {
            int total = monitoramentosVeiculos.Count;
            for (int i = 0; i < total; i++)
            {
                if (monitoramentosVeiculos[i].CodigoMonitoramento == codigoMonitoramento && monitoramentosVeiculos[i].CodigoVeiculo == codigoVeiculo)
                {
                    return monitoramentosVeiculos[i];
                }
            }
            Repositorio.Embarcador.Logistica.MonitoramentoVeiculo repMonitoramentoVeiculo = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculo(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Logistica.MonitoramentoVeiculo monitoramentoVeiculo = repMonitoramentoVeiculo.BuscarUltimoPorMonitoramentoVeiculoOVSimples(codigoMonitoramento, codigoVeiculo);
            if (monitoramentoVeiculo != null) monitoramentosVeiculos.Add(monitoramentoVeiculo);
            return monitoramentoVeiculo;
        }

        private void LogInformacoesCache()
        {
            Process currentProcess = Process.GetCurrentProcess();
            long memoryUsage = currentProcess.WorkingSet64 / 1024 / 1024;
            Log($"Cache currentProcess.WorkingSet64 {memoryUsage}MB", 1);

            Log($"Cache {PosicoesAtuaisCache.Count} PosicoesAtuaisCache", 1);

            int total = 0;
            int cont = this.CargasClientesPassagemCache.Count;
            for (int i = 0; i < cont; i++) total += this.CargasClientesPassagemCache[i].Clientes.Count;
            Log($"Cache {cont} CargasClientesPassagemCache com {total} Clientes", 1);

            total = 0;
            cont = this.VeiculosPosicoesCache.Count;
            for (int i = 0; i < cont; i++) total += this.VeiculosPosicoesCache[i].Posicoes?.Length ?? 0;
            Log($"Cache {cont} VeiculosPosicoesCache com {total} Posicoes", 1);

        }

        #endregion

    }
}

