using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Monitoramento.Integracoes.Abstract
{

    public abstract class AbstractIntegracao : AbstractThread
    {

        #region Atributos protegidos

        protected bool habilitada = true;

        protected System.Threading.Thread _threadIntegracao;

        protected AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente;
        protected int minutosDiferencaMinimaEntrePosicoes = 0;
        protected Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao;
        protected string configSectionName = "";
        protected Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento configuracao;
        protected List<Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao> contasIntegracao;
        protected List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar> monitorar;
        protected List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao> opcoes;

        protected string stringConexao = "";
        protected Repositorio.UnitOfWork unitOfWork;
        protected List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> ListaVeiculos;
        protected List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> ListaVeiculosMonitorados;
        protected List<string> ListaPlacaVeiculosRemovidosMonitorados;
        protected IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlertaSensor> ListaPosicaoAlertaSensor;
        private DateTime? dataConsultaVeiculos;

        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS;

        protected List<Dominio.ObjetosDeValor.Monitoramento.UltimaAtualizacao> UltimasPosicoesVeiculos;

        #endregion

        #region Atributos privados

        private char _separadorChaveValor = '=';

        private DateTime _dataUltimaPosicaoRecebida;
        private DateTime _dataUltimoAvisoSemNovaPosicao;


        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_MONITORAR_TEMPO_MAXIMO_SEM_NOVA_POSICAO = "TempoMaximoSemNovaPosicao";
        private const string KEY_MONITORAR_TEMPO_ENTRE_AVISOS_SEM_NOVA_POSICAO = "TempoEntreAvisosSemNovaPosicao";
        private const string KEY_MONITORAR_EMAILS_PARA_AVISO = "EmailsParaAviso";

        #endregion

        #region Métodos públicos

        /**
         * Construtor
         */
        public AbstractIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipo, string configSection, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            this.tipoIntegracao = tipo;
            this.configSectionName = configSection;
            this.cliente = cliente;
            this.UltimasPosicoesVeiculos = new List<Dominio.ObjetosDeValor.Monitoramento.UltimaAtualizacao>();
            LogNewLine();
            Log($"Iniciando integracao {this.tipoIntegracao}");
        }

        /**
         * Início da execução da integração         */
        public void Iniciar(string stringConexao, CancellationToken? cancellationToken = null)
        {
            this.stringConexao = stringConexao;

            using (unitOfWork = new Repositorio.UnitOfWork(this.stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                InicializandoConfiguracoes();

                if (habilitada)
                {
                    PreparandoContasIntegracao();
                    PreparandoListaOpcoesIntegracao();
                    PreparandoListaMonitorarIntegracao();

                    InicializandoDiretorioEArquivos();

                    ComplementandoConfiguracoes();
                    Validando();
                    Preparando();
                }
            }

            if (habilitada)
                _threadIntegracao = IniciandoThread(cancellationToken);
        }

        public void Finalizar()
        {
            if (_threadIntegracao != null)
            {
                _threadIntegracao.Abort();
                _threadIntegracao = null;
            }

        }

        /**
         * Atualiza as configurações da integração a partir do banco de dados
         * Este método é chamado periodicamente pelo ConfiguracaoRefreshService
         */
        public void AtualizarConfiguracoes(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Log($"Atualizando configurações");

                Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento repConfiguracaoIntegracaoTecnologiaMonitoramento = new(unitOfWork);
                Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta repConfiguracaoIntegracaoTecnologiaMonitoramentoConta = new(unitOfWork);
                Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar repConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar = new(unitOfWork);
                Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao = new(unitOfWork);

                configuracao = repConfiguracaoIntegracaoTecnologiaMonitoramento.BuscarPorTipo(tipoIntegracao);
                if (configuracao != null)
                {
                    habilitada = configuracao.Habilitada;
                    minutosDiferencaMinimaEntrePosicoes = configuracao.MinutosDiferencaMinimaEntrePosicoes;

                    if (!habilitada)
                    {
                        Log("Integração desabilitada após atualização", 1, true);
                        return;
                    }

                    if (!repConfiguracaoIntegracaoTecnologiaMonitoramentoConta.ExistePorConfiguracao(configuracao))
                    {
                        Log("Nenhuma conta configurada após atualização", 1, true);
                        habilitada = false;
                        return;
                    }

                    contasIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao>();

                    List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta> configuracaoIntegracaoTecnologiaMonitoramentoContas = repConfiguracaoIntegracaoTecnologiaMonitoramentoConta.BuscarPorConfiguracao(configuracao);
                    foreach (Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta conta in configuracaoIntegracaoTecnologiaMonitoramentoContas)
                    {
                        if (conta.Habilitada)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao = new Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao
                            {
                                Nome = conta.Nome,
                                Habilitada = conta.Habilitada,
                                TipoIntegracao = tipoIntegracao,
                                TipoComunicacaoIntegracao = conta.TipoComunicacaoIntegracao,
                                Protocolo = conta.Protocolo,
                                Servidor = conta.Servidor,
                                Porta = conta.Porta,
                                URI = conta.URI,
                                Usuario = conta.Usuario,
                                Senha = conta.Senha,
                                BancoDeDados = conta.BancoDeDados,
                                Diretorio = conta.Diretorio,
                                ArquivoControle = conta.ArquivoControle,
                                ParametrosAdicionais = conta.ParametrosAdicionais,
                                ListaParametrosAdicionais = Servicos.Embarcador.Logistica.ContaIntegracao.ObterListaParametrosAdicionais(conta.ParametrosAdicionais),
                                RastreadorId = conta.RastreadorId,
                                SolicitanteId = conta.SolicitanteId,
                                SolicitanteSenha = conta.SolicitanteSenha,
                                BuscarDadosVeiculos = conta.BuscarDadosVeiculos,
                                UsaPosicaoFrota = conta.UsaPosicaoFrota
                            };

                            contasIntegracao.Add(contaIntegracao);
                        }
                        Log($"Conta: {conta.Nome} atualizada", 1);
                    }

                    opcoes = repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao.BuscarPorConfiguracao(configuracao);
                    Log($"Opcoes atualizadas", 1);

                    monitorar = repConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar.BuscarPorConfiguracao(configuracao);
                    Log($"Monitorar atualizado", 1);

                    ComplementarConfiguracoes();
                    Log($"Configurações complementares atualizadas", 1);

                    Log($"Configurações atualizadas com sucesso. {contasIntegracao.Count} contas habilitadas");
                }
                else
                {
                    Log("Configuração não encontrada após atualização");
                    habilitada = false;
                }
            }
            catch (Exception e)
            {
                Log($"Erro ao atualizar configurações: {e.Message}");
            }
        }

        #endregion

        #region Métodos abstratos

        /**
         * Complementa configurações
         */
        protected abstract void ComplementarConfiguracoes();

        /**
         * Faz devidas verificações para garantir parâmetros corretos
         */
        protected abstract void Validar();

        /**
         * Preparação para iniciar a execução, executada apenas uma vez
         */
        protected abstract void Preparar();

        /**
         * Execução principal de cada iteração da thread, repetida infinitamente
         */
        protected abstract void Executar(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao configuracao);

        #endregion

        #region Métodos protegidos

        protected string ObterValorOpcao(string identificador)
        {
            return opcoes.Find(o => o.Key == identificador)?.Value;
        }

        protected string ObterValorMonitorar(string identificador)
        {
            return monitorar.Find(o => o.Key == identificador)?.Value;
        }

        /**
         * Busca um veículo na lista de cache, pela placa
         */
        protected List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> ObterVeiculoPorPlaca(string placa)
        {
            return ListaVeiculos.Where(s => s.Placa == placa).ToList();
        }

        /**
         * Busca um veículo na lista de cache, pelo código do equipamento rastreador
         */
        protected List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> ObterVeiculoPorEquipamento(string equipamento)
        {
            return ListaVeiculos.Where(s => s.NumeroEquipamentoRastreador == equipamento).ToList();
        }

        /**
         * Busca um veículo na lista de cache, pelo código do equipamento rastreador
         */
        protected List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> ObterVeiculoPorEquipamentos(string[] listaEquipamentos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> veiculos = new List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento>();
            int total = listaEquipamentos.Length;
            for (int i = 0; i < total; i++)
            {
                veiculos = veiculos.Concat(ObterVeiculoPorEquipamento(listaEquipamentos[i])).ToList();
            }
            return veiculos;
        }

        /**
         * Busca um veículo na lista de cache, pelo código do equipamento rastreador ou pela placa
         */
        protected List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> ObterVeiculoPorEquipamentoOuPlaca(string equipamento, string placa)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> veiculos = new List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento>();
            if (!string.IsNullOrEmpty(equipamento))
            {
                veiculos = ObterVeiculoPorEquipamento(equipamento);
            }

            veiculos.AddRange(ObterVeiculoPorPlaca(placa));

            return veiculos.DistinctBy(x => x.Codigo).ToList();
        }

        /**
         * Insere a lista de posições recebidas replicando para os veículos com mesma placa ou número de rastreador
         */
        protected void InserirPosicoes(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes)
        {

            // Converte para array
            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao[] posicoesArray = (posicoes != null) ? posicoes.ToArray() : null;

            // Mantém a lista com posições únicas
            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao[] posicoesUnicas = RemoverPosicoesRepetidas(posicoesArray);

            // Processa as posições distribuindo para os veículos encontrados
            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao[] posicoesProcessadas = ProcessarPosicoes(posicoesUnicas);

            // Insere a lista de posições recebidas já replicadas entre os veículos com a mesma placa
            InserirPosicoesProntas(posicoesProcessadas);

        }

        /**
         * Mantém a lista com posições únicas
         */
        private Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao[] RemoverPosicoesRepetidas(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao[] posicoes)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesUnicas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            int total = posicoes?.Length ?? 0;
            if (total > 0)
            {
                Log($"Remover posicoes repetidas de {total} posicoes", 3);
                for (int i = 0; i < total; i++)
                {
                    bool jaExiste = false;
                    int totalUnicas = posicoesUnicas.Count;
                    for (int j = 0; j < totalUnicas; j++)
                    {
                        if (
                            posicoes[i].IDEquipamento == posicoesUnicas[j].IDEquipamento &&
                            posicoes[i].Placa == posicoesUnicas[j].Placa &&
                            posicoes[i].DataVeiculo == posicoesUnicas[j].DataVeiculo &&
                            posicoes[i].Latitude == posicoesUnicas[j].Latitude &&
                            posicoes[i].Longitude == posicoesUnicas[j].Longitude &&
                            posicoes[i].Temperatura == posicoesUnicas[j].Temperatura &&
                            posicoes[i].Velocidade == posicoesUnicas[j].Velocidade &&
                            posicoes[i].SensorTemperatura == posicoesUnicas[j].SensorTemperatura &&
                            posicoes[i].NivelSinalGPS == posicoesUnicas[j].NivelSinalGPS &&
                            posicoes[i].NivelBateria == posicoesUnicas[j].NivelBateria
                        )
                        {
                            jaExiste = true;
                            break;
                        }
                    }
                    if (!jaExiste) posicoesUnicas.Add(posicoes[i]);
                }
            }
            posicoesUnicas.Sort((x, y) => x.DataVeiculo.CompareTo(y.DataVeiculo));
            return posicoesUnicas.ToArray();
        }

        /**
         * Processa as posições distribuindo para os veículos encontrados
         */
        protected Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao[] ProcessarPosicoes(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao[] posicoes)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesProcessadas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            int total = posicoes?.Length ?? 0;
            if (total > 0)
            {
                Log($"Processando {total} posicoes", 3);
                for (int i = 0; i < total; i++)
                {
                    // Localiza os veículos cadastrados no sistema a partir do ID do equipamentou ou pela placa
                    List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> veiculos = ObterVeiculoPorEquipamentoOuPlaca(posicoes[i].IDEquipamento, posicoes[i].Placa);
                    int totalVeiculos = veiculos?.Count ?? 0;
                    for (int j = 0; j < totalVeiculos; j++)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao novaPosicao = new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                        {
                            Data = posicoes[i].Data,
                            DataVeiculo = posicoes[i].DataVeiculo,
                            CodigoVeiculo = veiculos[j].Codigo,
                            IDEquipamento = posicoes[i].IDEquipamento,
                            Placa = veiculos[j].Placa,
                            Velocidade = posicoes[i].Velocidade,
                            Temperatura = posicoes[i].Temperatura,
                            Descricao = posicoes[i].Descricao,
                            Latitude = posicoes[i].Latitude,
                            Longitude = posicoes[i].Longitude,
                            Ignicao = posicoes[i].Ignicao,
                            km = posicoes[i].km,
                            SensorDeDesengate = posicoes[i].SensorDeDesengate,
                            Rastreador = posicoes[i].Rastreador,
                            Gerenciadora = posicoes[i].Gerenciadora
                        };
                        posicoesProcessadas.Add(novaPosicao);
                    }
                }
            }
            return posicoesProcessadas.ToArray();
        }

        /**
         * Insere a lista de posições recebidas já replicadas entre os veículos com a mesma placa
         */
        protected void InserirPosicoesProntas(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao[] posicoes)
        {
            int total = posicoes?.Length ?? 0;
            if (total > 0)
            {
                Log($"Inserindo {total} posicoes", 3);
                DateTime dataAtual = DateTime.Now;
                int totalInseridas = 0, totalIgnoradas = 0, totalInvalidas = 0;

                Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                for (int i = 0; i < total; i++)
                {
                    try
                    {
                        if (Servicos.Embarcador.Logistica.WayPointUtil.ValidarCoordenadas(posicoes[i].Latitude, posicoes[i].Longitude))
                        {

                            // Identifica a ultima posição recebida para o veículo
                            Dominio.ObjetosDeValor.Monitoramento.UltimaAtualizacao ultima = Servicos.Embarcador.Monitoramento.UltimaAtualizacao.ObterUltimaAtualizacao(posicoes[i].CodigoVeiculo, ref this.UltimasPosicoesVeiculos);
                            if (Servicos.Embarcador.Monitoramento.UltimaAtualizacao.VerificaSeJaExpirouEAtualiza(ref ultima, posicoes[i].DataVeiculo, this.minutosDiferencaMinimaEntrePosicoes))
                            {
                                Dominio.Entidades.Embarcador.Logistica.Posicao novaPosicao = new Dominio.Entidades.Embarcador.Logistica.Posicao
                                {
                                    Data = posicoes[i].Data,
                                    DataVeiculo = posicoes[i].DataVeiculo,
                                    DataCadastro = dataAtual,
                                    IDEquipamento = posicoes[i].IDEquipamento,
                                    Veiculo = new Dominio.Entidades.Veiculo { Codigo = posicoes[i].CodigoVeiculo, Placa = posicoes[i].Placa },
                                    Velocidade = (posicoes[i].Velocidade >= this.configuracaoTMS.VelocidadeMinimaAceitaDasTecnologias && posicoes[i].Velocidade < this.configuracaoTMS.VelocidadeMaximaAceitaDasTecnologias) ? posicoes[i].Velocidade : 0,
                                    Temperatura = (posicoes[i].Temperatura != null && posicoes[i].Temperatura >= this.configuracaoTMS.TemperaturaMinimaAceitaDasTecnologias && posicoes[i].Temperatura < this.configuracaoTMS.TemperaturaMaximaAceitaDasTecnologias) ? posicoes[i].Temperatura : null,
                                    SensorTemperatura = posicoes[i].SensorTemperatura.HasValue && posicoes[i].SensorTemperatura.Value == true ? true : posicoes[i].Temperatura != null,
                                    Descricao = posicoes[i].Descricao,
                                    Latitude = posicoes[i].Latitude,
                                    Quilometros = posicoes[i].km,
                                    Longitude = posicoes[i].Longitude,
                                    Ignicao = posicoes[i].Ignicao,
                                    Rastreador = posicoes[i].Rastreador,
                                    Gerenciadora = posicoes[i].Gerenciadora
                                };
                                repPosicao.Inserir(novaPosicao);
                                totalInseridas++;

                                if (posicoes[i].DataVeiculo > _dataUltimaPosicaoRecebida) _dataUltimaPosicaoRecebida = posicoes[i].DataVeiculo;

                                Log($"Posição inserida - Descrição {novaPosicao.Descricao}, Veículo: {novaPosicao.Veiculo.Placa}");

                                verificarPosicaoAlertaSensor(posicoes[i], novaPosicao);
                            }
                            else
                            {
                                totalIgnoradas++;
                            }
                        }
                        else
                        {
                            totalInvalidas++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(ex.ToString());
                    }
                }

                Log($"{totalInseridas} posicoes inseridas", 4);
                Log($"{totalIgnoradas} posicoes ignoradas", 4);
                Log($"{totalInvalidas} posicoes invalidas", 4);
            }
        }

        protected void InserirPosicoesProntas(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao[] posicoesArray = (posicoes != null) ? posicoes.ToArray() : null;
            InserirPosicoesProntas(posicoesArray);
        }

        protected List<KeyValuePair<string, string>> CarregarDadosDeControleDoArquivo(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta)
        {
            List<KeyValuePair<string, string>> dadosControle = new List<KeyValuePair<string, string>>();
            if (!string.IsNullOrWhiteSpace(conta.Diretorio) && !string.IsNullOrWhiteSpace(conta.ArquivoControle))
            {
#if DEBUG
                string path = "C:\\GerenciadorApp\\Producao\\" + conta.ArquivoControle;
#else
                string path = conta.Diretorio + conta.ArquivoControle;
#endif
                if (Utilidades.IO.FileStorageService.Storage.Exists(path))
                {
                    string content = Utilidades.IO.FileStorageService.Storage.ReadAllText(path);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        string[] lines = content.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        int total = lines.Length;
                        for (int i = 0; i < total; i++)
                        {
                            string[] keyValue = lines[i].Split(_separadorChaveValor);
                            if (keyValue.Length > 0)
                            {
                                dadosControle.Add(new KeyValuePair<string, string>(keyValue[0], (keyValue.Length > 1) ? keyValue[1] : string.Empty));
                            }
                        }
                    }
                }
            }
            return dadosControle;
        }

        protected void SalvarDadosDeControleNoArquivo(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta, List<KeyValuePair<string, string>> dadosControle)
        {
            if (!string.IsNullOrWhiteSpace(conta.Diretorio) && !string.IsNullOrWhiteSpace(conta.ArquivoControle))
            {
#if DEBUG
                string path = "C:\\GerenciadorApp\\Producao\\" + conta.ArquivoControle;
#else
                string path = conta.Diretorio + conta.ArquivoControle;
#endif
                string content = "";
                int total = dadosControle.Count;
                for (int i = 0; i < total; i++)
                {
                    content += dadosControle[i].Key + _separadorChaveValor + dadosControle[i].Value + Environment.NewLine;
                }
                Utilidades.IO.FileStorageService.Storage.WriteAllText(path, content);
            }
        }

        protected void ValidarConfiguracaoObrigatoriaConta(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta)
        {
            string msg;
            bool invalida = false;
            switch (conta.TipoComunicacaoIntegracao)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComunicacaoIntegracao.WebService:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComunicacaoIntegracao.WebServiceSOAP:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComunicacaoIntegracao.WebServiceREST:
                    invalida = (conta.Protocolo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.Protocolo.HTTP && conta.Protocolo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.Protocolo.HTTPS) ||
                               string.IsNullOrWhiteSpace(conta.Servidor);
                    break;

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComunicacaoIntegracao.DatabaseMSSQL:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComunicacaoIntegracao.DatabaseMySQL:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComunicacaoIntegracao.DatabaseOracle:
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComunicacaoIntegracao.DatabasePostgreSQL:
                    invalida = conta.Protocolo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.Protocolo.TCP ||
                               string.IsNullOrWhiteSpace(conta.Servidor) ||
                               conta.Porta <= 0 ||
                               string.IsNullOrWhiteSpace(conta.Usuario) ||
                               string.IsNullOrWhiteSpace(conta.Senha) ||
                               string.IsNullOrWhiteSpace(conta.BancoDeDados);
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComunicacaoIntegracao.ActiveMQ:
                    invalida = conta.Protocolo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.Protocolo.TCP ||
                               string.IsNullOrWhiteSpace(conta.Servidor) ||
                               conta.Porta <= 0 ||
                               string.IsNullOrWhiteSpace(conta.Usuario) ||
                               string.IsNullOrWhiteSpace(conta.Senha);
                    break;
            }

            if (invalida)
            {
                msg = $"Parametros obrigatorios nao informados nas configuracoes para a conta {conta.Nome} do tipo {conta.TipoComunicacaoIntegracao}";
                LogErro(msg);
                throw new Exception(msg);
            }
        }

        protected void ValidarConfiguracaoDiretorioEArquivoControle(List<Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao> contas)
        {
            int total = contas.Count;
            for (int i = 0; i < total; i++)
            {
                ValidarConfiguracaoDiretorioEArquivoControle(contas[i]);
            }
        }

        protected void ValidarConfiguracaoDiretorioEArquivoControle(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta)
        {
            ValidarConfiguracaoDiretorio(conta);
            ValidarConfiguracaoArquivoControle(conta);
        }

        protected void ValidarConfiguracaoDiretorio(List<Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao> contas)
        {
            int total = contas.Count;
            for (int i = 0; i < total; i++)
            {
                ValidarConfiguracaoDiretorio(contas[i]);
            }
        }

        protected void ValidarConfiguracaoDiretorio(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta)
        {
            string msg;

            string diretorio = conta.Diretorio;
#if DEBUG
            diretorio = "C:\\GerenciadorApp\\Producao";
#endif
            if (string.IsNullOrWhiteSpace(diretorio))
            {
                msg = $"Diretorio da conta {conta.Nome} nao informado.";
                LogErro(msg);
                throw new Exception(msg);
            }
        }

        protected void ValidarConfiguracaoArquivoControle(List<Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao> contas)
        {
            int total = contas.Count;
            for (int i = 0; i < total; i++)
            {
                ValidarConfiguracaoArquivoControle(contas[i]);
            }
        }

        protected void ValidarConfiguracaoArquivoControle(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta)
        {
            string msg;
            string diretorio = conta.Diretorio;
#if DEBUG
            diretorio = "C:\\GerenciadorApp\\Producao";
#endif

            if (string.IsNullOrWhiteSpace(conta.ArquivoControle))
            {
                msg = $"Diretorio ou arquivo de controle da conta {conta.Nome} nao informado.";
                LogErro(msg);
                throw new Exception(msg);
            }

            if (!Utilidades.IO.FileStorageService.Storage.Exists(Utilidades.IO.FileStorageService.Storage.Combine(diretorio, conta.ArquivoControle)))
            {
                msg = $"Arquivo de controle {conta.ArquivoControle} da conta {conta.Nome} nao existe.";
                LogErro(msg);
                throw new Exception(msg);
            }
        }

        protected void ValidarConfiguracaoDeContasNSTech(List<Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao> contas)
        {
            int total = contas.Count;
            for (int i = 0; i < total; i++)
            {
                ValidarConfiguracaoContaNSTech(contas[i]);
            }
        }

        protected void ValidarConfiguracaoDeContasRastrear(List<Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao> contas)
        {
            int total = contas.Count;
            for (int i = 0; i < total; i++)
            {
                ValidarConfiguracaoContaRastrear(contas[i]);
            }
        }


        protected void ValidarConfiguracaoContaRastrear(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta)
        {
            string msg;
            if (string.IsNullOrEmpty(conta.RastreadorId))
            {
                msg = $"Rastreador ID da conta {conta.Nome} nao informado.";
                LogErro(msg);
                throw new Exception(msg);
            }

            if (string.IsNullOrWhiteSpace(conta.Usuario))
            {
                msg = $"Usuario da conta {conta.Nome} nao informado.";
                LogErro(msg);
                throw new Exception(msg);
            }

            if (string.IsNullOrWhiteSpace(conta.Senha))
            {
                msg = $"Senha Senha ID da conta {conta.Nome} nao informado.";
                LogErro(msg);
                throw new Exception(msg);
            }

        }

        protected void ValidarConfiguracaoContaNSTech(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta)
        {
            string msg;
            if (string.IsNullOrEmpty(conta.RastreadorId))
            {
                msg = $"Rastreador ID da conta {conta.Nome} nao informado.";
                LogErro(msg);
                throw new Exception(msg);
            }

            if (string.IsNullOrWhiteSpace(conta.SolicitanteId))
            {
                msg = $"Solicitante ID da conta {conta.Nome} nao informado.";
                LogErro(msg);
                throw new Exception(msg);
            }

            if (string.IsNullOrWhiteSpace(conta.SolicitanteSenha))
            {
                msg = $"Solicitante Senha ID da conta {conta.Nome} nao informado.";
                LogErro(msg);
                throw new Exception(msg);
            }

            if (string.IsNullOrWhiteSpace(conta.Usuario))
            {
                msg = $"Usuario da conta {conta.Nome} nao informado.";
                LogErro(msg);
                throw new Exception(msg);
            }

            if (string.IsNullOrWhiteSpace(conta.Senha))
            {
                msg = $"Senha Senha ID da conta {conta.Nome} nao informado.";
                LogErro(msg);
                throw new Exception(msg);
            }


        }

        protected void verificarPosicaoAlertaSensor(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao, Dominio.Entidades.Embarcador.Logistica.Posicao novaPosicao)
        {
            //no momento apenas para o sensor de desengate;
            if (novaPosicao.Veiculo != null && posicao.SensorDeDesengate.HasValue)
            {
                DateTime inicio = DateTime.UtcNow;
                if (ListaPosicaoAlertaSensor.Count == 0)
                {
                    CriarNovaPosicaoSensor(posicao, novaPosicao);
                    AcionarListaPosicaoAlertaSensor(posicao, novaPosicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSensor.SensorDesengate);
                }

                int total = ListaPosicaoAlertaSensor.Count;
                bool encontrou = false;
                for (int i = 0; i < total; i++)
                {
                    if (ListaPosicaoAlertaSensor[i].IDVeiculo == novaPosicao.Veiculo.Codigo)
                    {
                        encontrou = true;
                        if (ListaPosicaoAlertaSensor[i].TipoSensor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSensor.SensorDesengate && ListaPosicaoAlertaSensor[i].ValorSensor != posicao.SensorDeDesengate.Value)//alterou valor de como estava na lista.
                        {
                            CriarNovaPosicaoSensor(posicao, novaPosicao);

                            ListaPosicaoAlertaSensor[i].ValorSensor = posicao.SensorDeDesengate.Value;
                            ListaPosicaoAlertaSensor[i].DataVeiculo = novaPosicao.DataVeiculo;
                            ListaPosicaoAlertaSensor[i].DataCadastro = DateTime.Now;
                            break;
                        }
                    }
                }

                if (!encontrou)
                {
                    CriarNovaPosicaoSensor(posicao, novaPosicao);
                    AcionarListaPosicaoAlertaSensor(posicao, novaPosicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSensor.SensorDesengate);
                }

                Log("verificarPosicaoAlertaSensor", inicio, 2);
            }
        }

        protected void CriarNovaPosicaoSensor(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao, Dominio.Entidades.Embarcador.Logistica.Posicao novaPosicao)
        {
            Repositorio.Embarcador.Logistica.PosicaoAlertaSensor repPosicaoSensor = new Repositorio.Embarcador.Logistica.PosicaoAlertaSensor(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.PosicaoAlertaSensor novaPosicaoAlertaSensor = new Dominio.Entidades.Embarcador.Logistica.PosicaoAlertaSensor()
            {
                DataCadastro = DateTime.Now,
                DataVeiculo = novaPosicao.DataVeiculo,
                //Posicao = novaPosicao,
                Latitude = novaPosicao.Latitude,
                Longitude = novaPosicao.Longitude,
                ValorSensor = posicao.SensorDeDesengate.Value,
                TipoSensor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSensor.SensorDesengate,
                Veiculo = novaPosicao.Veiculo,
            };
            repPosicaoSensor.Inserir(novaPosicaoAlertaSensor);
        }

        protected void AcionarListaPosicaoAlertaSensor(Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao posicao, Dominio.Entidades.Embarcador.Logistica.Posicao novaPosicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSensor tipoSensor)
        {
            if (ListaPosicaoAlertaSensor == null)
                ListaPosicaoAlertaSensor = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlertaSensor>();

            Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlertaSensor posicaoAlertaSensor = new Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlertaSensor()
            {
                DataCadastro = DateTime.Now,
                DataVeiculo = novaPosicao.DataVeiculo,
                IDPosicao = novaPosicao.Codigo,
                IDVeiculo = novaPosicao.Veiculo.Codigo,
                ValorSensor = posicao.SensorDeDesengate.Value,
                TipoSensor = tipoSensor
            };
            ListaPosicaoAlertaSensor.Add(posicaoAlertaSensor);
        }

        /**
         * Extrai os números únicos dos equipamento
         */
        protected List<string> ObterNumerosEquipamentosDosVeiculos(bool unico = true)
        {
            List<string> numerosEquipamentos = new List<string>();
            int total = ListaVeiculos?.Count ?? 0;
            for (int i = 0; i < total; i++)
            {
                if (unico)
                {
                    if (!numerosEquipamentos.Contains(ListaVeiculos[i].NumeroEquipamentoRastreador)) numerosEquipamentos.Add(ListaVeiculos[i].NumeroEquipamentoRastreador);
                }
                else
                {
                    numerosEquipamentos.Add(ListaVeiculos[i].NumeroEquipamentoRastreador);
                }

            }
            return numerosEquipamentos;
        }

        protected Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador ObterEnumRastreadorDescricao(string descricao)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreadorHelper.ObterEnumPorDescricao(descricao);
            if (rastreador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.NaoDefinido)
                Log($"Rastreador nao definido: " + descricao, 1);

            return rastreador;
        }

        #endregion

        #region Métodos privados

        private void Validando()
        {
            Log("Validando");
            Validar();
        }

        private void ComplementandoConfiguracoes()
        {
            Log("Complementando configuracoes");
            ComplementarConfiguracoes();
        }

        private void Preparando()
        {
            Log("Preparando");
            Preparar();
        }

        private void ExecutandoIntegracao()
        {
            Log($"Executando integracao");

            CarregarConfiguracaoTMS();

            // Percorre as contas
            int total = this.contasIntegracao.Count;
            for (int i = 0; i < total; i++)
            {
                Log($"Conta {this.contasIntegracao[i].Nome}", 1);

                // Executa a integração da conta
                Executar(this.contasIntegracao[i]);

                // Aguarda o tempo entre requisições de contas
                System.Threading.Thread.Sleep(Math.Abs(this.configuracao.TempoSleepEntreContas) * 1000);

            }
        }

        /**
         * Criação e start da thread
         */
        private System.Threading.Thread IniciandoThread(CancellationToken? cancellationToken = null)
        {
            System.Threading.Thread task = new System.Threading.Thread(() =>
           {
               System.Threading.Thread.CurrentThread.Name = this.GetType().Name;
               while ((!cancellationToken?.IsCancellationRequested) ?? true)
               {
                   try
                   {
                       Log("Inicio");
                       DateTime inicio = DateTime.UtcNow;

                       using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(this.stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                       {
                           this.unitOfWork = unidadeDeTrabalho;
                           if (ValidandoIntegracao())
                           {
                               if (PossuiMonitoramento(unidadeDeTrabalho))
                               {
                                   InicializandoVeiculos();
                                   InicializandoPosicaoAlertaSensor();
                                   MonitorarSemPosicaoRecebida();
                                   ExecutandoIntegracao();

                               }
                               else
                               {
                                   Log($"Configuracao \"Possui monitoramento\" desativada");
                               }
                           }
                           else
                           {
                               Log($"TipoIntegracao nao encontrada");
                           }
                       }

                       Log("Fim", inicio, 0, true);

                       System.Threading.Thread.Sleep(this.configuracao.TempoSleepThread * 1000);

                   }
                   catch (TaskCanceledException abort)
                   {
                       Log(string.Concat("Task de integracao da aleta cancelada: ", abort.ToString()));
                       break;
                   }
                   catch (System.Threading.ThreadAbortException abortThread)
                   {
                       Log(string.Concat("Task de integracao da alerta cancelada: ", abortThread.ToString()));
                       break;
                   }
                   catch (Exception ex)
                   {
                       Log(ex.ToString());

                       MonitorarSemPosicaoRecebida();
                       System.Threading.Thread.Sleep(this.configuracao.TempoSleepThread * 1000);
                   }
               }
           });

            task.Start();

            return task;
        }

        /**
         * Leitura da seção das configurações do App.config
         */
        private void InicializandoConfiguracoes()
        {
            Log($"Inicializando configuracoes da section \"{configSectionName}\"");
            try
            {
                Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento repConfiguracaoIntegracaoTecnologiaMonitoramento = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento(unitOfWork);
                Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta repConfiguracaoIntegracaoTecnologiaMonitoramentoConta = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta(unitOfWork);

                configuracao = repConfiguracaoIntegracaoTecnologiaMonitoramento.BuscarPorTipo(tipoIntegracao);

                if (configuracao == null)
                    configuracao = CriarConfiguracaoTecnologiaMonitoramento();

                habilitada = configuracao?.Habilitada ?? false;
                minutosDiferencaMinimaEntrePosicoes = configuracao?.MinutosDiferencaMinimaEntrePosicoes ?? 0;

                if (!habilitada)
                {
                    Log("Integracao desabilitada", 0, true);
                    return;
                }

                if (!repConfiguracaoIntegracaoTecnologiaMonitoramentoConta.ExistePorConfiguracao(configuracao))
                {
                    Log("Nenhuma conta configurada", 0, true);
                    habilitada = false;
                }
            }
            catch (Exception e)
            {
                Log(e.Message);
                habilitada = false;
            }
        }

        private Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento CriarConfiguracaoTecnologiaMonitoramento()
        {
            Dominio.ObjetosDeValor.Embarcador.Monitoramento.Configuracao.Integracao.IntegracaoConfigSection integracaoConfigSection = (Dominio.ObjetosDeValor.Embarcador.Monitoramento.Configuracao.Integracao.IntegracaoConfigSection)ConfigurationManager.GetSection(this.configSectionName);

            if (integracaoConfigSection == null)
                return null;

            Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento repConfiguracaoIntegracaoTecnologiaMonitoramento = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento(unitOfWork);
            Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar repConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar(unitOfWork);
            Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao(unitOfWork);
            Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta repConfiguracaoIntegracaoTecnologiaMonitoramentoConta = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta(unitOfWork);

            Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento configuracaoTecnologia = new Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramento()
            {
                CodigoIntegracao = integracaoConfigSection.CodigoIntegracao,
                Habilitada = integracaoConfigSection.Habilitada,
                MinutosDiferencaMinimaEntrePosicoes = integracaoConfigSection.MinutosDiferencaMinimaEntrePosicoes,
                ProcessarSensores = integracaoConfigSection.ProcessarSensores,
                TempoSleepEntreContas = integracaoConfigSection.TempoSleepEntreContas,
                TempoSleepThread = integracaoConfigSection.TempoSleepThread,
                Tipo = tipoIntegracao
            };

            repConfiguracaoIntegracaoTecnologiaMonitoramento.Inserir(configuracaoTecnologia);

            if (integracaoConfigSection.Contas?.Count > 0)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Monitoramento.Configuracao.Integracao.ContaElement contaElement in integracaoConfigSection.Contas)
                {
                    Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta configuracaoTecnologiaConta = new Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta()
                    {
                        Configuracao = configuracaoTecnologia,
                        ArquivoControle = contaElement.ArquivoControle,
                        BancoDeDados = contaElement.BancoDeDados,
                        BuscarDadosVeiculos = !string.IsNullOrEmpty(contaElement.BuscarDadosVeiculos) && Convert.ToBoolean(contaElement.BuscarDadosVeiculos),
                        Charset = contaElement.Charset,
                        Diretorio = contaElement.Diretorio,
                        Habilitada = contaElement.Habilitada,
                        Nome = contaElement.Nome,
                        ParametrosAdicionais = contaElement.ParametrosAdicionais,
                        Porta = contaElement.Porta,
                        Protocolo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.Protocolo)Enum.Parse(typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.Protocolo), contaElement.Protocolo.ToUpper()),
                        RastreadorId = contaElement.RastreadorId,
                        Senha = contaElement.Senha,
                        Servidor = contaElement.Servidor,
                        SolicitanteId = contaElement.SolicitanteId,
                        SolicitanteSenha = contaElement.SoliccitanteSenha,
                        TipoComunicacaoIntegracao = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComunicacaoIntegracao)Enum.Parse(typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComunicacaoIntegracao), contaElement.Tipo),
                        URI = contaElement.URI,
                        UsaPosicaoFrota = contaElement.UsaPosicaoFrota,
                        Usuario = contaElement.Usuario
                    };

                    repConfiguracaoIntegracaoTecnologiaMonitoramentoConta.Inserir(configuracaoTecnologiaConta);
                }
            }

            if (integracaoConfigSection.Monitorar?.Count > 0)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Monitoramento.Configuracao.Integracao.OpcaoElement monitorarElement in integracaoConfigSection.Monitorar)
                {
                    Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar configuracaoTecnologiaMonitorar = new Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar()
                    {
                        Configuracao = configuracaoTecnologia,
                        Key = monitorarElement.Key,
                        Value = monitorarElement.Value
                    };

                    repConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar.Inserir(configuracaoTecnologiaMonitorar);
                }
            }

            if (integracaoConfigSection.Opcoes?.Count > 0)
            {
                foreach (Dominio.ObjetosDeValor.Embarcador.Monitoramento.Configuracao.Integracao.OpcaoElement monitorarElement in integracaoConfigSection.Opcoes)
                {
                    Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao configuracaoTecnologiaOpcao = new Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao()
                    {
                        Configuracao = configuracaoTecnologia,
                        Key = monitorarElement.Key,
                        Value = monitorarElement.Value
                    };

                    repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao.Inserir(configuracaoTecnologiaOpcao);
                }
            }

            return configuracaoTecnologia;
        }

        /**
         * Percorre as contas do arquivo e gera as configurações a serem usadas pela integração
         */
        private void PreparandoContasIntegracao()
        {
            Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta repConfiguracaoIntegracaoTecnologiaMonitoramentoConta = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta(unitOfWork);

            List<Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta> configuracaoIntegracaoTecnologiaMonitoramentoContas = repConfiguracaoIntegracaoTecnologiaMonitoramentoConta.BuscarPorConfiguracao(configuracao);

            Log("Preparando contas para integracao");
            Log($"{configuracaoIntegracaoTecnologiaMonitoramentoContas.Count} conta(s) configurada(s)", 1);

            contasIntegracao = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao>();

            foreach (Dominio.Entidades.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoConta conta in configuracaoIntegracaoTecnologiaMonitoramentoContas)
            {
                if (conta.Habilitada)
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao = new Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao
                    {
                        Nome = conta.Nome,
                        Habilitada = conta.Habilitada,
                        TipoIntegracao = tipoIntegracao,
                        TipoComunicacaoIntegracao = conta.TipoComunicacaoIntegracao,
                        Protocolo = conta.Protocolo,
                        Servidor = conta.Servidor,
                        Porta = conta.Porta,
                        URI = conta.URI,
                        Usuario = conta.Usuario,
                        Senha = conta.Senha,
                        BancoDeDados = conta.BancoDeDados,
                        Diretorio = conta.Diretorio,
                        ArquivoControle = conta.ArquivoControle,
                        ParametrosAdicionais = conta.ParametrosAdicionais,
                        ListaParametrosAdicionais = Servicos.Embarcador.Logistica.ContaIntegracao.ObterListaParametrosAdicionais(conta.ParametrosAdicionais),
                        RastreadorId = conta.RastreadorId,
                        SolicitanteId = conta.SolicitanteId,
                        SolicitanteSenha = conta.SolicitanteSenha,
                        BuscarDadosVeiculos = conta.BuscarDadosVeiculos,
                        UsaPosicaoFrota = conta.UsaPosicaoFrota
                    };

                    ValidarConfiguracaoObrigatoriaConta(contaIntegracao);

                    contasIntegracao.Add(contaIntegracao);
                }
            }

            Log($"{contasIntegracao.Count} conta(s) habilitada(s)", 1);
        }

        private void PreparandoListaOpcoesIntegracao()
        {
            Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao(unitOfWork);

            opcoes = repConfiguracaoIntegracaoTecnologiaMonitoramentoOpcao.BuscarPorConfiguracao(configuracao);
        }

        private void PreparandoListaMonitorarIntegracao()
        {
            Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar repConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar = new Repositorio.Embarcador.Integracao.Monitoramento.ConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar(unitOfWork);

            monitorar = repConfiguracaoIntegracaoTecnologiaMonitoramentoMonitorar.BuscarPorConfiguracao(configuracao);
        }

        /**
         * Verifica se a integração está cadastrada
         */
        private bool ValidandoIntegracao()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            return repTipoIntegracao.BuscarPorTipo(this.tipoIntegracao) != null;
        }

        /**
         * Preparação inicial do diretório e arquivos básicos
         */
        private void InicializandoDiretorioEArquivos()
        {
            int total = this.contasIntegracao.Count;
            Log($"Inicializando diretorio e arquivos");
            for (int i = 0; i < total; i++)
            {
                Log($"Conta {this.contasIntegracao[i].Nome}", 1);

                string diretorio = this.contasIntegracao[i].Diretorio;

#if DEBUG
                diretorio = "C:\\GerenciadorApp\\Producao";
#endif

                // Arquivo com o controle do ID/Sequencial/Data da mensagem da tecnologia
                if (!string.IsNullOrWhiteSpace(this.contasIntegracao[i].ArquivoControle) && !Utilidades.IO.FileStorageService.Storage.Exists(Utilidades.IO.FileStorageService.Storage.Combine(diretorio, this.contasIntegracao[i].ArquivoControle)))
                {
                    Log($"Criando arquivo {this.contasIntegracao[i].ArquivoControle}", 2);
                    try
                    {
                        using Stream f = Utilidades.IO.FileStorageService.Storage.Create(diretorio + this.contasIntegracao[i].ArquivoControle);
                    }
                    catch (Exception ex)
                    {
                        Log(string.Concat("", ex.ToString()));
                        throw;
                    }
                }
            }
        }

        /**
         * Carrega os veículos para um vetor de cache
         */
        private void InicializandoVeiculos()
        {
            Log("Inicializando veiculos");
            ListaVeiculos = new List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento>();
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.TecnologiaRastreador repTecnologia = new Repositorio.Embarcador.Veiculos.TecnologiaRastreador(unitOfWork);

            Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador tecnologiaRastreador = repTecnologia.BuscarAtivoPorCodigoIntegracao(this.configuracao.CodigoIntegracao);

            if (tecnologiaRastreador != null)
                ListaVeiculos = repVeiculo.BuscarTodosParaIntegracao(tecnologiaRastreador);
            else
            {
                if (this.configuracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskPosicoes ||
                    this.configuracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Rastrear ||
                    this.configuracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.ApisulLog ||
                    this.configuracao.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskPosicoesPlaca)
                {
                    IncializandoVeiculosMonitorados(this.configuracao.Tipo);
                    Log(ListaVeiculosMonitorados?.Count > 0 ? "Veiculos Monitorados:" + ListaVeiculosMonitorados?.Count : "", 2);
                }

                ListaVeiculos = repVeiculo.BuscarTodosParaIntegracao();
            }

            Log(ListaVeiculos.Count + " veiculos", 2);
        }

        private void IncializandoVeiculosMonitorados(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);

            if (dataConsultaVeiculos == null || (DateTime.Now - dataConsultaVeiculos.Value).TotalMinutes > 15)
            {
                // TODO: ToList cast
                ListaVeiculosMonitorados = repMonitoramento.BuscarVeiculosMonitorados(this.configuracao.Tipo).ToList();
                dataConsultaVeiculos = DateTime.Now;
                ListaPlacaVeiculosRemovidosMonitorados = new List<string>();
            }

            if (ListaPlacaVeiculosRemovidosMonitorados.Count > 0)
            {
                for (int i = 0; i <= ListaVeiculosMonitorados.Count - 1; i++)
                {
                    if (ListaPlacaVeiculosRemovidosMonitorados.Contains(ListaVeiculosMonitorados[i].Placa))
                        ListaVeiculosMonitorados.RemoveAll(x => x.Placa == ListaVeiculosMonitorados[i].Placa);
                }

                if (ListaVeiculosMonitorados.Count <= 0)
                {
                    // TODO: ToList cast
                    ListaVeiculosMonitorados = repMonitoramento.BuscarVeiculosMonitorados(this.configuracao.Tipo).ToList();
                    dataConsultaVeiculos = DateTime.Now;
                    ListaPlacaVeiculosRemovidosMonitorados = new List<string>();
                }
            }
        }

        private void InicializandoPosicaoAlertaSensor()
        {
            Log("Inicializando posicao alerta Sensor");
            ListaPosicaoAlertaSensor = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoAlertaSensor>();
            Repositorio.Embarcador.Logistica.PosicaoAlertaSensor repPosicaoAlerta = new Repositorio.Embarcador.Logistica.PosicaoAlertaSensor(unitOfWork);
            ListaPosicaoAlertaSensor = repPosicaoAlerta.BuscarListaUltimasPosicoesAlertaSensorPorVeiculos();
            Log(ListaPosicaoAlertaSensor.Count + " posicoes alerta sensor", 2);
        }

        private void CarregarConfiguracaoTMS()
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            this.configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
        }

        private void MonitorarSemPosicaoRecebida()
        {
            string auxString = ObterValorMonitorar(KEY_MONITORAR_TEMPO_MAXIMO_SEM_NOVA_POSICAO);
            int tempoMaximoSemNovaPosicao = (!string.IsNullOrWhiteSpace(auxString)) ? int.Parse(auxString) : 0;
            if (tempoMaximoSemNovaPosicao > 0)
            {
                string emailsParaAviso = ObterValorMonitorar(KEY_MONITORAR_EMAILS_PARA_AVISO);

                if (!string.IsNullOrWhiteSpace(emailsParaAviso))
                {
                    auxString = ObterValorMonitorar(KEY_MONITORAR_TEMPO_ENTRE_AVISOS_SEM_NOVA_POSICAO);
                    int intervaloEntreAvisosSemNovasPosicao = (!string.IsNullOrWhiteSpace(auxString)) ? int.Parse(auxString) : 0;

                    DateTime dataAtual = DateTime.Now;
                    TimeSpan atrasoPosicao = dataAtual - _dataUltimaPosicaoRecebida;
                    TimeSpan atrasoAviso = dataAtual - _dataUltimoAvisoSemNovaPosicao;
                    if (_dataUltimaPosicaoRecebida != DateTime.MinValue && atrasoPosicao.TotalMinutes > tempoMaximoSemNovaPosicao && atrasoAviso.TotalMinutes > intervaloEntreAvisosSemNovasPosicao)
                    {
                        _dataUltimoAvisoSemNovaPosicao = dataAtual;
                        AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = this.cliente;
                        if (cliente != null)
                        {
                            string maskDate = "dd/MM/yyyy HH:mm:ss";

                            string nomeCliente = cliente.NomeFantasia;
                            string subject = "Problemas no monitoramento " + this.tipoIntegracao.ToString();
                            string body = $"<h1>Atenção!</h1>";
                            body += "<p>";
                            body += $"Ambiente: {nomeCliente}<br/>";
                            body += $"Serviço Multisoftware: {cliente.ClienteConfiguracao.TipoServicoMultisoftware.ToString()}<br/>";
                            body += $"Rastreador: {this.tipoIntegracao.ToString()}<br/>";
                            body += $"Data atual: {dataAtual.ToString(maskDate)}<br/>";
                            body += $"Última posição: {_dataUltimaPosicaoRecebida.ToString(maskDate)}<br/>";
                            body += $"Atraso (>{tempoMaximoSemNovaPosicao}min): {atrasoPosicao.ToString(@"d\.hh\:mm\:ss")}";
                            body += "</p>";

                            List<string> emails = new List<string>();
                            string[] emailsDestino = emailsParaAviso.Split(',');
#if !DEBUG
                            emails = emailsDestino.ToList();
#else
                            emails.Add("fernando@multisoftware.com.br");
#endif
                            if (emails.Count() > 0)
                            {
                                Servicos.Email svcEmail = new Servicos.Email(unitOfWork);
                                svcEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, nomeCliente + " - " + subject, body, string.Empty, null, string.Empty, true, "cte@multisoftware.com.br", 0, unitOfWork, 0, true, emails, false);
                            }
                        }
                    }
                }
            }
        }

        #endregion

    }

}
