using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class UltimaPosicaoVeiculo
    {
        public string placa;
        public DateTime data;
    }

    /**
     * WSDL Produção: http://ws.opentechgr.com.br/sgropentech/
     * WSDL Teste: http://ws.opentechgr.com.br/sgropenteste2/sgropentech.asmx
     * Help Online: http://ws.opentechgr.com.br/sgropentech/help/
     */
    public class IntegracaoOpentech : Abstract.AbstractIntegracaoSOAP
    {
        #region Atributos privados

        private static IntegracaoOpentech Instance;
        private static readonly string nameConfigSection = "Opentech";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a Opentech

        private List<UltimaPosicaoVeiculo> ultimasPosicoesVeiculos;

        private DateTime dataAtual;
        private DateTime ultimaDataConsultada;
        private DateTime ultimaDataTemperatura;

        private Servicos.ServicoOpenTech.sgrOpentechSoapClient svcOpenTech;
        private List<KeyValuePair<string, string>> chavesAcessoLogin;
        private DateTime dataLimiteConsulta;
        private int maximoHorasPassado = 12;

        #endregion

        #region Constantes com as chaves dos dados/configurações

        private const string KEY_ULTIMA_DATA = "UltimaData";
        private const string KEY_DOMINIO = "dominio";
        private const string KEY_CDPAS = "cdpas";
        private const string KEY_CDCLIENTE = "cdcliente";

        #endregion

        #region Construtor privado

        /**
         * Construtor 
         */
        private IntegracaoOpentech(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OpenTech, nameConfigSection, cliente) { }

        #endregion

        #region Métodos públicos

        /**
         * Implementação de Singleton
         */
        public static IntegracaoOpentech GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoOpentech(cliente);
            return Instance;
        }

        #endregion

        #region Métodos abstratos obrigatórios

        /**
         * Complementa configurações
         */
        protected override void ComplementarConfiguracoes()
        {

        }

        /**
         * Faz devidas verificações para garantir parâmetros corretos
         */
        protected override void Validar()
        {

        }

        /**
         * Preparação para iniciar a execução, executada apenas uma vez
         */
        protected override void Preparar()
        {
            this.ultimasPosicoesVeiculos = new List<UltimaPosicaoVeiculo>();
            this.chavesAcessoLogin = new List<KeyValuePair<string, string>>();

            this.dataAtual = DateTime.Now;
            this.ultimaDataTemperatura = dataAtual;

        }

        /**
         * Execução principal de cada iteração da thread, repetida infinitamente
         */
        override protected void Executar(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao)
        {
            this.conta = contaIntegracao;

            this.dataAtual = DateTime.Now;
            dataLimiteConsulta = dataAtual.AddHours(-this.maximoHorasPassado);
            ObterUltimaDataDoArquivo();
            Inicializar();
            Autenticar();
            IntegrarPosicao();
            SalvarUltimaDataNoArquivo();
        }

        #endregion

        #region Métodos privados

        /**
         * Inicialização do WebService com a criação do objeto cliente
         */
        private void Inicializar()
        {
            Log("Inicializando WS", 2);

            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);

            this.svcOpenTech = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<Servicos.ServicoOpenTech.sgrOpentechSoapClient, Servicos.ServicoOpenTech.sgrOpentechSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Opentech_SgrOpentech, url);
        }

        /**
         * Autenticação par recuperar a chave de acesso para as demais requisições
         */
        private void Login()
        {
            Servicos.ServicoOpenTech.sgrData retornoLogin = svcOpenTech.sgrLogin(this.conta.Usuario, this.conta.Senha, ObterDominioConta());
            Log($"Login {retornoLogin.ReturnDescription}", 3);
            DefinirChaveLogin(retornoLogin.ReturnKey);
        }

        /**
         * Executa a integração das posições, consultando no WebService e registrando na tabela T_POSICAO
         */
        private void IntegrarPosicao()
        {

            Log($"Consultando posicoes", 2);

            // Busca as posições do WebService
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = ObterPosicoes();

            // Registra as posições recebidas já replicadas entre os veículos com a mesma placa/equipamento
            Log($"Integrando {posicoes.Count} posicoes", 2);
            base.InserirPosicoesProntas(posicoes);

        }

        /**
         * Busca as posições de todos os veículos
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ObterPosicoes()
        {
            // Chave de login da conta
            string chaveLogin = ObterChaveLogin();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            if (!string.IsNullOrWhiteSpace(chaveLogin))
            {
                posicoes = RetornaPosicaoFrota(chaveLogin);
                Log($"Recebidas {posicoes.Count} posicoes de frota", 3);
            }

            return posicoes;
        }

        /**
         * Busca as posições de todos os veículos
         */
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> RetornaPosicaoFrota(string chaveLogin)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();

            try
            {
                Log($"Data Consulta {this.ultimaDataConsultada}", 3);
                Servicos.ServicoOpenTech.sgrData response = null;
                if (conta.UsaPosicaoFrota)
                    response = svcOpenTech.sgrRetornaPosicaoFrota(chaveLogin, ObterCdpasConta(), ObterCdclienteConta(), this.dataAtual, 1);
                else
                    response = svcOpenTech.srgRetornaPosicoesMonitoradas(chaveLogin, ObterCdpasConta(), ObterCdclienteConta(), this.ultimaDataConsultada);

                if (response != null && response.ReturnID == 0 && response.ReturnDataset != null && response.ReturnDataset.Nodes?.Count > 1)
                {
                    try
                    {
                        XElement xmlData = (XElement)response.ReturnDataset.Nodes[1].FirstNode;

                        IEnumerable<XElement> posicaoElementos;
                        
                        if (conta.UsaPosicaoFrota)
                            posicaoElementos = xmlData.Elements("sgrPosicaoFrota");
                        else
                            posicaoElementos = xmlData.Elements("sgrTB");

                        int total = posicaoElementos.Count();

                        Log($"Request Recebidas {total} linhas", 3);

                        foreach (XElement posicaoXml in posicaoElementos)
                        {
                            string placa = posicaoXml.Element("NRPLACACAVALO")?.Value?.Trim() ?? "";
                            string strCodigoViagem = posicaoXml.Element("CDVIAG")?.Value?.Trim() ?? "";
                            string posicao = posicaoXml.Element("POSICAO")?.Value?.Trim() ?? "";
                            string strVelocidade = posicaoXml.Element("VLVELOCIDADE")?.Value?.Trim() ?? "";
                            string strIgnicao = posicaoXml.Element("FLIGNICAO")?.Value?.Trim() ?? "";
                            string strnomeTecnologia = posicaoXml.Element("FLIGNICAO")?.Value?.Trim() ?? "";
                            string strData = "";
                            string strLatitude = "";
                            string strLongitude = "";
                            string strTemperatura = "";

                            if (conta.UsaPosicaoFrota)
                            {
                                strData = posicaoXml.Element("DTPOSICAO")?.Value?.Trim() ?? "";
                                strLatitude = posicaoXml.Element("VLLAT")?.Value?.Trim() ?? "";
                                strLongitude = posicaoXml.Element("VLLONG")?.Value?.Trim() ?? "";
                                strTemperatura = posicaoXml.Element("VLTEMPERATURASENSOR1")?.Value?.Trim() ?? "";
                            }
                            else
                            {
                                strData = posicaoXml.Element("DTDATAPOSICAO")?.Value?.Trim() ?? "";
                                strLatitude = posicaoXml.Element("VLLATITUDE")?.Value?.Trim() ?? "";
                                strLongitude = posicaoXml.Element("VLLONGITUDE")?.Value?.Trim() ?? "";
                                strTemperatura = posicaoXml.Element("VALORTEMPERATURA")?.Value?.Trim() ?? "";
                            }

                            if (!string.IsNullOrWhiteSpace(placa) && !string.IsNullOrWhiteSpace(strData) && !string.IsNullOrWhiteSpace(strLatitude) && !string.IsNullOrWhiteSpace(strLongitude))
                            {

                                UltimaPosicaoVeiculo ultimaPosicaoVeiculo = ObtemUltimaDataPosicaoVeiculo(placa);
                                DateTime data = DateTime.Parse(strData);

                                Log($"Data Recebida: {data} placa: {placa} Data Veiculo: {ultimaPosicaoVeiculo.data} ", 3);

                                if (data > ultimaPosicaoVeiculo.data)
                                {
                                    Log($"posicao inserida", 3);

                                    int ignicao = strIgnicao.ToInt();
                                    double latitude = double.Parse(strLatitude, CultureInfo.InvariantCulture);
                                    double longitude = double.Parse(strLongitude, CultureInfo.InvariantCulture);
                                    double.TryParse(strVelocidade, NumberStyles.Any, CultureInfo.InvariantCulture, out double dbVelocidade);
                                    int velocidade = (!string.IsNullOrWhiteSpace(strVelocidade)) ? (int)Math.Round(dbVelocidade, 0) : 0;
                                    decimal? temperatura = (!string.IsNullOrWhiteSpace(strTemperatura)) ? decimal.Parse(strTemperatura, CultureInfo.InvariantCulture) : null;

                                    List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> veiculos = base.ObterVeiculoPorPlaca(placa);
                                    int totalVeiculos = veiculos.Count;
                                    for (int j = 0; j < totalVeiculos; j++)
                                    {
                                        posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                                        {
                                            Data = data,
                                            DataVeiculo = data,
                                            CodigoVeiculo = veiculos[j].Codigo,
                                            IDEquipamento = !string.IsNullOrWhiteSpace(veiculos[j].NumeroEquipamentoRastreador) ? veiculos[j].NumeroEquipamentoRastreador.Left(20) : "00",
                                            Placa = veiculos[j].Placa,
                                            Latitude = latitude,
                                            Longitude = longitude,
                                            Descricao = posicao,
                                            Velocidade = velocidade,
                                            Temperatura = temperatura,
                                            Ignicao = ignicao,
                                            Gerenciadora = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaGerenciadora.Opentech,
                                            Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.OpenTech
                                        }); ;
                                    }
                                    ultimaPosicaoVeiculo.data = data;
                                }
                            }
                        }

                        AtualizarUltimaDataConsultada(posicoes);
                    }
                    catch (Exception ex)
                    {
                        Log($"Erro ao processar posições XML: {ex.Message}", 1);
                    }
                }
                else if (response.ReturnID == 2)
                {
                    RemoverChaveLogin();
                }
                else if (response.ReturnID != 3)
                {
                    Log($"sgrRetornaPosicaoFrota {response.ReturnDescription}", 5);
                }

            }
            catch (Exception ex)
            {
                Log("Erro sgrRetornaPosicaoFrota " + ex.Message);
            }
            return posicoes;
        }

        private void Autenticar()
        {
            string chaveLogin = ObterChaveLogin();
            if (string.IsNullOrWhiteSpace(chaveLogin))
            {
                Login();
            }
        }

        private void RemoverChaveLogin()
        {
            string chaveLogin = ObterChaveLogin();
            if (string.IsNullOrWhiteSpace(chaveLogin))
            {
                int total = this.chavesAcessoLogin.Count;
                for (int i = 0; i < total; i++)
                {
                    if (this.chavesAcessoLogin[i].Key == this.conta.Nome)
                    {
                        this.chavesAcessoLogin.RemoveAt(i);
                    }
                }
            }
        }

        private void DefinirChaveLogin(string chaveLogin)
        {
            if (!string.IsNullOrWhiteSpace(chaveLogin))
            {
                int total = this.chavesAcessoLogin.Count;
                for (int i = 0; i < total; i++)
                {
                    if (this.chavesAcessoLogin[i].Key == this.conta.Nome)
                    {
                        this.chavesAcessoLogin[i] = new KeyValuePair<string, string>(this.conta.Nome, chaveLogin);
                        return;
                    }
                }
                this.chavesAcessoLogin.Add(new KeyValuePair<string, string>(this.conta.Nome, chaveLogin));
            }
        }

        private string ObterChaveLogin()
        {
            int total = this.chavesAcessoLogin.Count;
            for (int i = 0; i < total; i++)
            {
                if (this.chavesAcessoLogin[i].Key == this.conta.Nome)
                {
                    return this.chavesAcessoLogin[i].Value;
                }
            }
            return string.Empty;
        }

        /**
         * Localiza a última data de posição do veículo
         */
        private UltimaPosicaoVeiculo ObtemUltimaDataPosicaoVeiculo(string placa)
        {
            UltimaPosicaoVeiculo ultimaPosicaoVeiculo = null;
            int total = this.ultimasPosicoesVeiculos.Count;
            for (int i = 0; i < total; i++)
            {
                if (this.ultimasPosicoesVeiculos[i].placa == placa)
                {
                    ultimaPosicaoVeiculo = this.ultimasPosicoesVeiculos[i];
                    break;
                }
            }

            // Se não há nenhuma, busca a última registrada anteriormente
            if (ultimaPosicaoVeiculo == null)
            {
                // Busca a última posição do veículo no banco de dados, se não encontrar, considera a data atual como a última data
                Repositorio.Embarcador.Logistica.PosicaoAtual repPosicao = new Repositorio.Embarcador.Logistica.PosicaoAtual(base.unitOfWork);
                DateTime dataUltimaPosicao = repPosicao.BuscarUltimaDataPorPlaca(placa);

                if (dataUltimaPosicao == DateTime.MinValue)
                    dataUltimaPosicao = dataAtual;

                // Adiciona na lista das últumas posições dos veículos
                ultimaPosicaoVeiculo = new UltimaPosicaoVeiculo()
                {
                    placa = placa,
                    data = dataUltimaPosicao
                };
                this.ultimasPosicoesVeiculos.Add(ultimaPosicaoVeiculo);
            }

            if (ultimaPosicaoVeiculo.data < dataLimiteConsulta)
            {
                ultimaPosicaoVeiculo.data = dataLimiteConsulta;
            }

            return ultimaPosicaoVeiculo;
        }

        private void DefinirUltimaDataPosicaoVeiculo(string placa, DateTime data)
        {
            int total = this.ultimasPosicoesVeiculos.Count;
            for (int i = 0; i < total; i++)
            {
                if (this.ultimasPosicoesVeiculos[i].placa == placa && this.ultimasPosicoesVeiculos[i].data < data)
                {
                    this.ultimasPosicoesVeiculos[i].data = data;
                    break;
                }
            }
        }

        private string ObterDominioConta()
        {
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_DOMINIO, this.conta.ListaParametrosAdicionais);
            return value;
        }

        private int ObterCdpasConta()
        {
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_CDPAS, this.conta.ListaParametrosAdicionais);
            return Int32.Parse(value);
        }

        private int ObterCdclienteConta()
        {
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_CDCLIENTE, this.conta.ListaParametrosAdicionais);
            return Int32.Parse(value);
        }

        private void ObterUltimaDataDoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = base.CarregarDadosDeControleDoArquivo(this.conta);
            string value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_ULTIMA_DATA, dadosControle);
            try
            {
                this.ultimaDataConsultada = DateTime.Parse(value);
            }
            catch
            {
                this.ultimaDataConsultada = this.dataAtual;
            }

            Log($"Ultima data consulta {ultimaDataConsultada}", 2);
        }

        private void AtualizarUltimaDataConsultada(List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes)
        {
            int cont = posicoes.Count;
            if (cont > 0)
            {
                this.ultimaDataConsultada = posicoes[0].Data;
                for (int i = 1; i < cont; i++)
                {
                    if (posicoes[i].Data > this.ultimaDataConsultada)
                    {
                        this.ultimaDataConsultada = posicoes[i].Data.AddSeconds(1);
                    }
                }
            }
        }

        private void SalvarUltimaDataNoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = new List<KeyValuePair<string, string>>();
            dadosControle.Add(new KeyValuePair<string, string>(KEY_ULTIMA_DATA, this.ultimaDataConsultada.ToString()));
            base.SalvarDadosDeControleNoArquivo(this.conta, dadosControle);
            Log($"Atualizando ultimo Timestamp {this.ultimaDataConsultada}", 2);
        }

        #endregion

    }

}
