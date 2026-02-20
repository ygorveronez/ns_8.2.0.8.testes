using Dominio.ObjetosDeValor;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace Servicos.Embarcador.Monitoramento.Integracoes
{
    public class IntegracaoOnixSat : Abstract.AbstractIntegracaoREST
    {

        #region Atributos privados

        private static IntegracaoOnixSat Instance;
        private static readonly string nameConfigSection = "Onixsat";
        Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao conta;

        #endregion

        #region Atributos privados específicos para a Onixsat

        private static int TempoMinimoRequisicaoMensagem = 60;
        private static int TempoMinimoRequisicaoVeiculo = 60;
        private static int TempoMinimoRequisicaoEspelhamento = 60;

        private static string DiretorioRecebimento = @"Recebido\";
        private static string DiretorioListaVeiculos = @"ListaVeiculos\";
        private static string DiretorioLido = @"Lido\";
        private static string DiretorioErro = @"Erro\";

        private DateTime dataAtual;

        private DateTime DataUltimaRequisicaoMensagem;
        private DateTime DataUltimaRequisicaoVeiculo;
        private DateTime DataUltimaRequisicaoEspelhamento;
        private List<Dominio.ObjetosDeValor.Embarcador.Logistica.VeiculoOnixat> VeiculosOnixSat;
        private long ultimoSequencial_mID;

        private const string KEY_ULTIMO_SEQUENCIAL_MID = "UltimoSequencial_mID";
        private const int ZIP_LEAD_BYTES = 0x04034b50;
        private const ushort GZIP_LEAD_BYTES = 0x8b1f;

        #endregion

        #region Métodos públicos

        public static IntegracaoOnixSat GetInstance(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (Instance == null) Instance = new IntegracaoOnixSat(cliente);
            return Instance;
        }

        #endregion

        #region Construtor privado

        private IntegracaoOnixSat(AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente) : base(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OnixSat, nameConfigSection, cliente) { }

        #endregion

        #region Métodos abstratos obrigatórios

        /**
         * Complementa configurações gerais
         */
        override protected void ComplementarConfiguracoes()
        {
            string opcao;

            opcao = ObterValorOpcao("TempoMinimoRequisicaoMensagem");
            if (!string.IsNullOrWhiteSpace(opcao)) TempoMinimoRequisicaoMensagem = Int16.Parse(opcao);

            opcao = ObterValorOpcao("TempoMinimoRequisicaoVeiculo");
            if (!string.IsNullOrWhiteSpace(opcao)) TempoMinimoRequisicaoVeiculo = Int16.Parse(opcao);

            opcao = ObterValorOpcao("TempoMinimoRequisicaoEspelhamento");
            if (!string.IsNullOrWhiteSpace(opcao)) TempoMinimoRequisicaoEspelhamento = Int16.Parse(opcao);

            opcao = ObterValorOpcao("DiretorioRecebido");
            if (!string.IsNullOrWhiteSpace(opcao)) DiretorioRecebimento = opcao;

            opcao = ObterValorOpcao("DiretorioListaVeiculos");
            if (!string.IsNullOrWhiteSpace(opcao)) DiretorioListaVeiculos = opcao;

            opcao = ObterValorOpcao("DiretorioLido");
            if (!string.IsNullOrWhiteSpace(opcao)) DiretorioLido = opcao;

            opcao = ObterValorOpcao("DiretorioErro");
            if (!string.IsNullOrWhiteSpace(opcao)) DiretorioErro = opcao;
        }

        /**
         * Confirmação de parâmetros corretos, executada apenas uma vez
         */
        override protected void Validar()
        {
            base.ValidarConfiguracaoDiretorio(base.contasIntegracao);
        }

        /**
         * Preparação para iniciar a execução, executada apenas uma vez
         */
        override protected void Preparar()
        {
            dataAtual = DateTime.Now;
            DataUltimaRequisicaoMensagem = dataAtual.AddSeconds(-TempoMinimoRequisicaoMensagem);
            DataUltimaRequisicaoVeiculo = dataAtual.AddSeconds(-TempoMinimoRequisicaoVeiculo);
            DataUltimaRequisicaoEspelhamento = dataAtual.AddSeconds(-TempoMinimoRequisicaoEspelhamento);
        }

        /**
         * Execução principal de cada iteração da thread, repetida infinitamente
         */
        override protected void Executar(Dominio.ObjetosDeValor.Embarcador.Logistica.ContaIntegracao contaIntegracao)
        {            
            this.conta = contaIntegracao;
            dataAtual = DateTime.Now;
            ObterVeiculosEspelhados();
            GravarXMLMensagem();
            IntegrarDadosMensagem();
            IntegrarEspelhamentoPendente();
        }

        #endregion

        #region Métodos privados

        private byte[] ObterXMLMensagem()
        {
            var request = $@"<RequestMensagemCB><login>{this.conta.Usuario}</login><senha>{this.conta.Senha}</senha><mId>{this.ultimoSequencial_mID}</mId></RequestMensagemCB>";
            return ObterXML(request);
        }

        private byte[] ObterXMLEspelhamentoPendente()
        {
            var request = $@"<RequestEspelhamentoPendenteVeiculo><login>{this.conta.Usuario}</login><senha>{this.conta.Senha}</senha></RequestEspelhamentoPendenteVeiculo>";
            return ObterXML(request);
        }

        private void GravarXMLMensagem()
        {
            if (VeiculosOnixSat != null && VeiculosOnixSat.Count > 0)
            {
                Log($"Gravando xml mensagens/posicao", 2);
                if (DataUltimaRequisicaoMensagem.AddSeconds(TempoMinimoRequisicaoMensagem) < dataAtual)
                {
                    DataUltimaRequisicaoMensagem = dataAtual;
                    ObterUltimoSequencialDoArquivo();
                    byte[] xml = ObterXMLMensagem();
                    GravarXMLPosicao(xml);
                }
            }
        }

        private void ProcessarVeiculosOnixSat(List<Dominio.ObjetosDeValor.Embarcador.Logistica.VeiculoOnixat> veiculos)
        {

            Repositorio.Embarcador.Veiculos.TecnologiaRastreador repositorioTecnologiaRastreador = new Repositorio.Embarcador.Veiculos.TecnologiaRastreador(unitOfWork);
            Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador tecnologiaRastreador = repositorioTecnologiaRastreador.BuscarAtivaPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OnixSat);

            List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> veiculosAddUpdate = new List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento>();
            if (base.ListaVeiculos.Count > 0)
            {
                foreach (var v in veiculos)
                {
                    var veiculo = base.ListaVeiculos.FirstOrDefault(x => x.Placa == v.Placa);
                    if (veiculo == null)
                        veiculosAddUpdate.Add(new Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento()
                        {
                            NumeroEquipamentoRastreador = v.VeiID,
                            Placa = v.Placa
                        });
                    else if (veiculo.NumeroEquipamentoRastreador != v.VeiID)
                    {
                        veiculo.NumeroEquipamentoRastreador = v.VeiID;
                        veiculosAddUpdate.Add(veiculo);
                    }
                }

                if (veiculosAddUpdate.Count > 0 && this.conta.BuscarDadosVeiculos)
                {
                    AtualizarVeiculosME(veiculosAddUpdate, tecnologiaRastreador);
                }
            }
        }

        private void AtualizarVeiculosME(List<Dominio.ObjetosDeValor.Embarcador.Veiculos.VeiculoMonitoramento> veiculos, Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador tecnologiaRastreador)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            int atualizados = 0;
            foreach (var veiculo in veiculos)
            {

                Dominio.Entidades.Veiculo veiculoME = repVeiculo.BuscarPorPlaca(veiculo.Placa);
                if (veiculoME == null || (veiculoME.NumeroEquipamentoRastreador == veiculo.NumeroEquipamentoRastreador && veiculoME.TecnologiaRastreador?.Codigo == tecnologiaRastreador.Codigo))
                    continue;

                veiculoME.PossuiRastreador = true;
                veiculoME.NumeroEquipamentoRastreador = veiculo.NumeroEquipamentoRastreador;
                veiculoME.TecnologiaRastreador = tecnologiaRastreador;
                atualizados++;
                repVeiculo.Atualizar(veiculoME);
            }
            Log($"{atualizados} veiculos atualizados", 3);
        }

        private bool GravarPosicoes(Dominio.ObjetosDeValor.ResponseMensagemOnixSat respostas)
        {
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoes = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
                foreach (var mensagem in respostas.Mensagem)
                {
                    var endereco = mensagem.Endereco != string.Empty ? mensagem.Endereco : string.Empty;
                    var local = endereco + " " + mensagem.Cidade + "/" + mensagem.Estado;

                    posicoes.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao
                    {
                        ID = mensagem.MId,
                        Data = mensagem.Data,
                        DataVeiculo = mensagem.DataVeiculo,
                        IDEquipamento = mensagem.IdVeiculo.ToString(),
                        Velocidade = mensagem.Velocidade,
                        Temperatura = mensagem.Temperatura1,
                        SensorTemperatura = mensagem.Temperatura1 != null && !mensagem.ErroSensorTemperatura1,
                        Descricao = local,
                        Latitude = mensagem.Latitude,
                        Longitude = mensagem.Longitude,
                        Ignicao = mensagem.Ignicao,
                        Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.OnixSat
                    });
                    if (mensagem.MId > ultimoSequencial_mID)
                        ultimoSequencial_mID = mensagem.MId;
                }

                base.InserirPosicoes(posicoes);
                SalvarUltimoSequencialNoArquivo();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
            return true;
        }

        private string ObterXMLVeiculo()
        {
            var request = $@"<RequestVeiculo><login>{this.conta.Usuario}</login><senha>{this.conta.Senha}</senha></RequestVeiculo>";
            var retorno = ObterXML(request);
            GravarXMLVeiculos(retorno);
            return Descompactar(retorno);
        }

        private bool ValidarXML(string xml)
        {
            return xml?.IndexOf("ErrorRequest") == -1;
        }

        private void ObterVeiculosEspelhados()
        {
            if (DataUltimaRequisicaoVeiculo.AddSeconds(TempoMinimoRequisicaoVeiculo) < dataAtual)
            {
                Log($"Buscando dados dos veiculos espelhados", 2);
                string xml = "";
                try
                {
                    xml = ObterXMLVeiculo();
                    if (ValidarXML(xml))
                    {
                        DataUltimaRequisicaoVeiculo = dataAtual;
                        XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.ResponseVeiculoOnixsat));
                        StringReader stringReader = new StringReader(xml);
                        var response = (Dominio.ObjetosDeValor.Embarcador.Logistica.ResponseVeiculoOnixsat)serializer.Deserialize(stringReader);
                        VeiculosOnixSat = response.Veiculo;
                        if (VeiculosOnixSat == null || VeiculosOnixSat.Count == 0)
                        {
                            Log($"Nenhum veiculo espelhado", 1);
                        }
                        else
                        {
                            if (this.conta.BuscarDadosVeiculos)
                                ProcessarVeiculosOnixSat(VeiculosOnixSat);

                            Log($"{VeiculosOnixSat.Count} veiculos espelhados", 3);
                        }
                    }
                    else
                    {
                        Log($"Nenhum veiculo espelhado, problemas ao validar arquivo xml veiculos", 1);
                    }
                }
                catch (Exception e)
                {
                    Log($"Erro ao ObterVeiculosEspelhados {xml}" + e, 3);
                }
            }
        }

        private void AceitarEspelhamento(Dominio.ObjetosDeValor.ResponseEspelhamentoPendenteVeiculo respostaEspelhamentoPendente)
        {
            if (respostaEspelhamentoPendente.EspelhamentoPendenteVeiculo.Count == 0)
                return;

            var request = $"<RequestAREspelhamentoVeiculo login=\"{this.conta.Usuario}\" senha=\"{this.conta.Senha}\">";
            foreach (var respostaEspelhamento in respostaEspelhamentoPendente.EspelhamentoPendenteVeiculo)
            {
                var id = 1;
                request += $@"<espelhamento tipo=""1"">
                                <id>{id}</id>
                                <veiID>{respostaEspelhamento.VeiID}</veiID>
                                <desc> Aceite espelhamento automatico</desc>
                                <usuario>Integracao</usuario>
                              </espelhamento>";
                id++;
            }
            request += "</RequestAREspelhamentoVeiculo>";

            byte[] respostaRequest = ObterXML(request);
            string resposta = System.Text.Encoding.UTF8.GetString(respostaRequest);
        }

        private void IntegrarEspelhamentoPendente()
        {
            if (DataUltimaRequisicaoEspelhamento.AddSeconds(TempoMinimoRequisicaoEspelhamento) < dataAtual)
            {
                Log($"Integrando dados do espelhamento", 2);
                DataUltimaRequisicaoEspelhamento = dataAtual;
                var xmlcompactado = ObterXMLEspelhamentoPendente();
                var xml = Descompactar(xmlcompactado);

                if (ValidarXML(xml))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.ResponseEspelhamentoPendenteVeiculo));
                    StringReader stringReader = new StringReader(xml);
                    var respostas = (Dominio.ObjetosDeValor.ResponseEspelhamentoPendenteVeiculo)serializer.Deserialize(stringReader);
                    AceitarEspelhamento(respostas);
                }
            }

        }

        private void IntegrarDadosMensagem()
        {
            Log($"Integrando dados de mensagem/posicao", 2);

            var ListaArquivos = Utilidades.File.ListarArquivosDiretorio(this.conta.Diretorio + DiretorioRecebimento);
            ListaArquivos.Sort();

            XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.ResponseMensagemOnixSat));

            foreach (var arquivo in ListaArquivos)
            {
                var xmlcompactado = Utilidades.File.LerArquivo(arquivo);

                if (xmlcompactado.Length > 0)
                {

                    var fileName = Path.GetFileName(arquivo);
                    ResponseMensagemOnixSat respostas = null;
                    var xml = string.Empty;
                    
                    try
                    {
                        xml = Descompactar(xmlcompactado);

                        if (!ValidarXML(xml))
                            throw new Exception("XML inválido");
              
                        using (StringReader stringReader = new StringReader(xml))
                        {
                            respostas = (Dominio.ObjetosDeValor.ResponseMensagemOnixSat)serializer.Deserialize(stringReader);
                        }
                    }
                    catch
                    {
                        var destino = this.conta.Diretorio + DiretorioErro + fileName;
                        if (Utilidades.IO.FileStorageService.Storage.Exists(destino))
                            Utilidades.File.RemoverArquivo(destino);

                        Utilidades.File.MoverArquivoParaDiretorio(arquivo, this.conta.Diretorio + DiretorioErro);
                        continue;
                    }
                    
                    var gravouPosicao = GravarPosicoes(respostas);
                    if (gravouPosicao)
                    {
                        var destino = this.conta.Diretorio + DiretorioLido + fileName;
                        if (Utilidades.IO.FileStorageService.Storage.Exists(destino))
                            Utilidades.File.RemoverArquivo(destino);

                        Utilidades.File.MoverArquivoParaDiretorio(arquivo, this.conta.Diretorio + DiretorioLido);
                    }
                }
                else
                {
                    var fileName = Path.GetFileName(arquivo);
                    var destino = this.conta.Diretorio + DiretorioErro + fileName;

                    if (Utilidades.IO.FileStorageService.Storage.Exists(destino))
                        Utilidades.File.RemoverArquivo(destino);

                    Utilidades.File.MoverArquivoParaDiretorio(arquivo, this.conta.Diretorio + DiretorioErro);
                }

            }

        }

        private void GravarXMLPosicao(byte[] xml)
        {
            var arquivo = $@"{this.conta.Diretorio}{DiretorioRecebimento}Onixsat{dataAtual.ToString("yyyyMMdd_HHmmssfff")}.zip";
            Utilidades.File.SalvarTextoEmArquivo(arquivo, xml);
        }

        private void GravarXMLVeiculos(byte[] xml)
        {
            var arquivo = $@"{this.conta.Diretorio}{DiretorioListaVeiculos}Veiculos{dataAtual.ToString("yyyyMMdd_HHmmssfff")}.zip";
            Utilidades.File.SalvarTextoEmArquivo(arquivo, xml);
        }

        private byte[] ObterXML(string request)
        {
            byte[] respostaCompactada = RequestXml(request);
            return respostaCompactada;
        }

        private byte[] RequestXml(string strRequest)
        {
            byte[] result;
            DateTime inicio = DateTime.UtcNow;
            try
            {
                byte[] sendData = UTF8Encoding.UTF8.GetBytes(strRequest);

                HttpWebRequest request = CreateRequest();
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(sendData, 0, sendData.Length);
                requestStream.Flush();
                requestStream.Dispose();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                MemoryStream output = new MemoryStream();

                byte[] buffer = new byte[256];
                int byteReceived = -1;
                do
                {
                    byteReceived = responseStream.Read(buffer, 0, buffer.Length);
                    output.Write(buffer, 0, byteReceived);
                } while (byteReceived > 0);

                responseStream.Dispose();
                response.Close();
                buffer = output.ToArray();
                output.Dispose();

                result = buffer;

            }
            catch (Exception e)
            {
                String msg = "Erro ao fazer o download do arquivo: " + e.Message;
                Log(msg, 3);
                throw new Exception(msg);
            }
            Log("Requisicao ", inicio, 3);
            return result;

        }

        private HttpWebRequest CreateRequest()
        {
            string url = Servicos.Embarcador.Logistica.ContaIntegracao.ObterURL(this.conta);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "text/xml";
            return request;
        }

        /**
         * Descompacta o arquivo que pode estar em formato GZIP ou ZIP.
         * O content-type da resposta não indica qual o formato da resposta.
         */
        private string Descompactar(byte[] data)
        {
            if (IsGZipCompressedData(data))
            {
                // Tenta descompactar a partir de um formato GZIP
                return DescompactarGZIP(data);
            }
            else if (IsZipCompressedData(data))
            {
                // Tenta descompactar a partir de um formato ZIP
                return DescompactarZIP(data);
            }
            return null;
        }

        private string DescompactarGZIP(byte[] data)
        {
            MemoryStream input = new MemoryStream();
            input.Write(data, 0, data.Length);
            input.Position = 0;
            GZipStream gzip = new GZipStream(input, CompressionMode.Decompress,
            true);
            byte[] buff = new byte[256];
            MemoryStream output = new MemoryStream();
            int read = gzip.Read(buff, 0, buff.Length);
            while (read > 0)
            {
                output.Write(buff, 0, read);
                read = gzip.Read(buff, 0, buff.Length);
            }
            gzip.Close();
            byte[] buffer = output.ToArray();
            output.Dispose();
            return Encoding.UTF8.GetString(buffer);
        }

        private string DescompactarZIP(byte[] data)
        {
            using (var outputStream = new MemoryStream())
            using (var inputStream = new MemoryStream(data))
            {
                using (var zipInputStream = new ZipInputStream(inputStream))
                {
                    zipInputStream.GetNextEntry();
                    zipInputStream.CopyTo(outputStream);
                }
                return Encoding.UTF8.GetString(outputStream.ToArray());
            }

        }

        /**
         * Verifica se os primeiros 4 bytes do array é uma assinatura ZIP
         */
        private bool IsZipCompressedData(byte[] data)
        {
            return BitConverter.ToInt32(data, 0) == ZIP_LEAD_BYTES;
        }

        /**
         * Verifica se os primeiros 2 bytes do array é uma assinatura GZIP
         */
        private bool IsGZipCompressedData(byte[] data)
        {
            return BitConverter.ToUInt16(data, 0) == GZIP_LEAD_BYTES;
        }

        /**
         * Busca no arquivo os último mID dos veículos.
         */
        private void ObterUltimoSequencialDoArquivo()
        {
            string value;
            List<KeyValuePair<string, string>> dadosControle = base.CarregarDadosDeControleDoArquivo(this.conta);
            value = Servicos.Embarcador.Logistica.ContaIntegracao.ObterValorListaParametrosAdicionais(KEY_ULTIMO_SEQUENCIAL_MID, dadosControle);
            this.ultimoSequencial_mID = long.Parse((String.IsNullOrWhiteSpace(value)) ? "1" : value);
            Log($"Lido ultimo sequencial {ultimoSequencial_mID}", 2);
        }

        /**
         * Registra no arquivo os último mID dos veículos.
         */
        private void SalvarUltimoSequencialNoArquivo()
        {
            List<KeyValuePair<string, string>> dadosControle = new List<KeyValuePair<string, string>>();
            dadosControle.Add(new KeyValuePair<string, string>(KEY_ULTIMO_SEQUENCIAL_MID, ultimoSequencial_mID.ToString()));
            base.SalvarDadosDeControleNoArquivo(this.conta, dadosControle);
            Log($"Salvo ultimo sequencial {ultimoSequencial_mID}", 2);
        }

        #endregion

    }
}
