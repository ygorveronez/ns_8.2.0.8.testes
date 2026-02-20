using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace SGT.GerenciadorApp.Monitoramento.Integracao
{
    public class IntegracaoOnixSat
    {
        private static int TempoMinimoPosicaoEmMinutos = 0;
        private static Int64 Usuario = 13969629000196;
        private static int Senha = 114417;
        //private static readonly int Tempo = 5000;
        private static readonly int TempoMinimoRequiscaoMenagem = 60;
        private static readonly int TempoMinimoRequisicaoVeiculo = 700;
        private static readonly int TempoMinimoRequisicaoEspelhamento = 60;
        private static readonly int tempoSleep = 60;
        
        private static readonly string DiretorioRecebimento = @"C:\Integracao\Onixat\Recebido\";
        private static readonly string DiretorioListaVeiculos = @"C:\Integracao\Onixat\ListaVeiculos\";
        private static readonly string DiretorioLido = @"C:\Integracao\Onixat\Lido\";
        private static readonly string DiretorioErro = @"C:\Integracao\Onixat\Erro\";
        private static IntegracaoOnixSat Instance;
        private Repositorio.Veiculo repVeiculo;
        private List<Dominio.Entidades.Veiculo> ListaVeiculos;
        private DateTime DataUltimaRequiscaoMenagem = DateTime.Now;
        private DateTime DataUltimaRequiscaoVeiculo = DateTime.Now;
        private DateTime DataUltimaRequiscaoEspelhamento = DateTime.Now;
        private ConcurrentDictionary<int, Task> ListaTasks;
        private Repositorio.UnitOfWork _unidadeDeTrabalho;
        private Repositorio.Embarcador.Logistica.PosicaoAtual repPosicaoAtual;
        private List<Dominio.Entidades.Embarcador.Logistica.PosicaoAtual> posicoesAtuais;
        
        private void IniciarThread(int idEmpresa, string stringConexao, string stringConexaoAdmin, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            var filaConsulta = new ConcurrentQueue<int>();

            filaConsulta.Enqueue(idEmpresa);

            Task task = new Task(() =>
            {
                while (true)
                {
                    try
                    {

                        filaConsulta.Enqueue(idEmpresa);

                        using (NHibernate.ISession session = Repositorio.SessionHelper.OpenSession(stringConexao))
                        {
                            using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(session))
                            {
                                _unidadeDeTrabalho = unidadeDeTrabalho;

                                if (ValidarIntegracao())
                                {
                                    SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Inicio", this.GetType().Name);

                                    InicializarVeiculos();

                                    GravarXMLMensagem();

                                    IntegrarDadosMensagem();

                                    IntegrarDadosVeiculo();

                                    IntegrarEspelhamentoPendente();

                                    SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Fim\r\n", this.GetType().Name);
                                }

                                unidadeDeTrabalho.Dispose();
                            }
                        }

                        GC.Collect();

                        System.Threading.Thread.Sleep(tempoSleep * 1000);

                        if (!filaConsulta.TryDequeue(out idEmpresa))
                        {
                            Servicos.Log.TratarErro("Task parou a execução");
                            break;
                        }
                    }
                    catch (TaskCanceledException abort)
                    {
                        Servicos.Log.TratarErro(string.Concat("Task de integração da aleta cancelada: ", abort.ToString()));
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Task de integração da alerta cancelada: ", abortThread.ToString()));
                        break;
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        System.Threading.Thread.Sleep(tempoSleep * 1000);
                    }
                }
            });

            if (ListaTasks.TryAdd(idEmpresa, task))
                task.Start();
            else
                Servicos.Log.TratarErro("Não foi possível adicionar a task de integrações à fila.");
        }
        private bool ValidarIntegracao()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unidadeDeTrabalho);
            return repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.OnixSat) != null;
        }
        private void InicializarVeiculos()
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Inicializando veículos", this.GetType().Name);
            repVeiculo = new Repositorio.Veiculo(_unidadeDeTrabalho);
            ListaVeiculos = repVeiculo.BuscarTodosVeiculos();
        }
        private byte[] ObterXMLMensagem()
        {
            var request = $@"<RequestMensagemCB>
                              <login>{Usuario}</login>
                              <senha>{Senha}</senha>
                              <mId>0</mId>
                            </RequestMensagemCB>";


            return ObterXML(request);

            
        }
        private byte[] ObterXML(string request)
        {

            byte[] respostaCompactada = RequestXml(request);

            return respostaCompactada;

        }
        private byte[] ObterXMLEspelhamentoPendente()
        {
            var request = $@"<RequestEspelhamentoPendenteVeiculo>
                              <login>{Usuario}</login>
                              <senha>{Senha}</senha>
                            </RequestEspelhamentoPendenteVeiculo>";

            return ObterXML(request);
        }
        private Dominio.Entidades.Embarcador.Logistica.PosicaoAtual InserirNovaPosicaoAtual(Dominio.Entidades.Embarcador.Logistica.Posicao posicao)
        {
            var novaPosicao = new Dominio.Entidades.Embarcador.Logistica.PosicaoAtual
            {
                Data = posicao.Data,
                DataVeiculo = posicao.Data,
                DataCadastro = DateTime.Now,
                IDEquipamento = posicao.IDEquipamento,
                Velocidade = posicao.Velocidade,
                Temperatura = posicao.Temperatura,
                SensorTemperatura = posicao.SensorTemperatura,
                Descricao = posicao.Descricao,
                Latitude = posicao.Latitude,
                Longitude = posicao.Longitude,
                Ignicao = posicao.Ignicao,
                Veiculo = posicao.Veiculo,
                Posicao = posicao
            };

            repPosicaoAtual.Inserir(novaPosicao);

            return novaPosicao;
        }
        private void AtualizarDadosPosicaoAtual(Dominio.Entidades.Embarcador.Logistica.PosicaoAtual posAtual, Dominio.Entidades.Embarcador.Logistica.Posicao novaPosicao)
        {
            posAtual.Data = novaPosicao.Data;
            posAtual.DataVeiculo = novaPosicao.DataVeiculo;
            posAtual.DataCadastro = DateTime.Now;
            posAtual.Descricao = novaPosicao.Descricao;
            posAtual.IDEquipamento = novaPosicao.IDEquipamento;
            posAtual.Latitude = novaPosicao.Latitude;
            posAtual.Longitude = novaPosicao.Longitude;
            posAtual.Velocidade = novaPosicao.Velocidade;
            posAtual.Temperatura = novaPosicao.Temperatura;
            posAtual.Ignicao = novaPosicao.Ignicao;
            posAtual.Veiculo = novaPosicao.Veiculo;
            posAtual.SensorTemperatura = novaPosicao.SensorTemperatura;
            posAtual.Posicao = novaPosicao;
        }
        private void GravarXMLMensagem()
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Gravando xml mensagens/posicao", this.GetType().Name);

            if (DataUltimaRequiscaoMenagem.AddSeconds(TempoMinimoRequiscaoMenagem) < DateTime.Now)
            {

                DataUltimaRequiscaoMenagem = DateTime.Now;

                byte[] xml = ObterXMLMensagem();

                GravarXMLPosicao(xml);
            }
        }
        private bool GravarPosicoes(Dominio.ObjetosDeValor.ResponseMensagemOnixSat respostas)
        {
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking($"Gravando posições total {respostas?.Mensagem?.Count}", this.GetType().Name);

            repPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(_unidadeDeTrabalho);
            posicoesAtuais = repPosicaoAtual.BuscarTodos();

            var repPosicao = new Repositorio.Embarcador.Logistica.Posicao(_unidadeDeTrabalho);
            _unidadeDeTrabalho.Start();
            try
            {
                foreach (var mensagem in respostas.Mensagem)
                {
                    var posicaoAtual = posicoesAtuais.Where(item => item.IDEquipamento == mensagem.IdVeiculo).FirstOrDefault();

                    if ((posicaoAtual != null) && posicaoAtual.DataVeiculo.AddMinutes(TempoMinimoPosicaoEmMinutos) > mensagem.Data)
                        continue;
                    

                    var Endereco = mensagem.Endereco != string.Empty ? mensagem.Endereco : string.Empty;
                    var Local = Endereco + " " + mensagem.Cidade + " " + mensagem.Estado;
                    var Veiculo = ObterVeiculoPorEquipamento(mensagem.IdVeiculo);


                    var posicao = new Dominio.Entidades.Embarcador.Logistica.Posicao
                    {
                        Data = mensagem.Data,
                        DataVeiculo = mensagem.Data,
                        DataCadastro = DateTime.Now,
                        IDEquipamento = mensagem.IdVeiculo,
                        Veiculo = Veiculo != null ? Veiculo : null,
                        Velocidade = mensagem.Velocidade,
                        Temperatura = mensagem.Temperatura1,
                        SensorTemperatura = mensagem.Temperatura1 != null && !mensagem.ErroSensorTemperatura1,
                        Descricao = Local,
                        Latitude = mensagem.Latitude,
                        Longitude = mensagem.Longitude,
                        Ignicao = mensagem.Ignicao

                    };

                    repPosicao.Inserir(posicao);

                    if (posicaoAtual == null)
                        posicoesAtuais.Add(InserirNovaPosicaoAtual(posicao));
                    else
                        AtualizarDadosPosicaoAtual(posicaoAtual, posicao);
                }
                _unidadeDeTrabalho.CommitChanges();


                return true;
            }
            catch (Exception excecao)
            {
                _unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(excecao);
                return false;
            }
        }
        private void GravarPosicaoAtual()
        {
           
            _unidadeDeTrabalho.Start();
            try
            {
                foreach(var posAtual in posicoesAtuais) { 
                    repPosicaoAtual.Atualizar(posAtual);
                }

                _unidadeDeTrabalho.CommitChanges();
            }
            catch (Exception excecao)
            {
                _unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(excecao);
            }
        }
        private string ObterXMLVeiculo()
        {
            var request = $@"<RequestVeiculo>
                              <login>{Usuario}</login>
                              <senha>{Senha}</senha>
                            </RequestVeiculo>";


            var retorno = ObterXML(request);

            GravarXMLVeiculos(retorno);

            return Descompactar(retorno);
        }
        private Dominio.Entidades.Veiculo ObterVeiculoPorPlaca(string Placa)
        {
            return ListaVeiculos.Where(s => s?.Placa == Placa).FirstOrDefault();
        }
        private Dominio.Entidades.Veiculo ObterVeiculoPorEquipamento(int Equipamento)
        {
            return ListaVeiculos.Where(s => s.NumeroEquipamentoRastreador == Equipamento.ToString()).FirstOrDefault();
        }
        private bool ValidarXML(string xml)
        {
            return xml?.IndexOf("ErrorRequest") == -1;
        }
        private void IntegrarDadosVeiculo()
        {
            if (DataUltimaRequiscaoVeiculo.AddSeconds(TempoMinimoRequisicaoVeiculo) < DateTime.Now)
            {
                SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Integrando dados do veículo", this.GetType().Name);
                DataUltimaRequiscaoVeiculo = DateTime.Now;
                string xml = "";
                try
                {
                   xml = ObterXMLVeiculo();

                    if (ValidarXML(xml))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.Embarcador.Logistica.ResponseVeiculoOnixsat));

                        StringReader stringReader = new StringReader(xml);

                        var respostas = (Dominio.ObjetosDeValor.Embarcador.Logistica.ResponseVeiculoOnixsat)serializer.Deserialize(stringReader);


                        foreach (var vei in respostas.Veiculo)
                        {
                            var veiculo = ObterVeiculoPorPlaca(vei.Placa);

                            if ((veiculo != null) && (string.IsNullOrWhiteSpace(veiculo?.NumeroEquipamentoRastreador)))
                            {
                                veiculo.PossuiRastreador = true;
                                veiculo.NumeroEquipamentoRastreador = vei.VeiID;

                                repVeiculo.Atualizar(veiculo);
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Servicos.Log.TratarErro($"Erro ao IntegrarDadosVeiculo {xml} - " +e);
                }
            }

        }
        private void AceitarEspelhamento(Dominio.ObjetosDeValor.ResponseEspelhamentoPendenteVeiculo respostaEspelhamentoPendente)
        {
            if (respostaEspelhamentoPendente.EspelhamentoPendenteVeiculo.Count == 0)
                return;

            var request = $"<RequestAREspelhamentoVeiculo login=\"{Usuario }\" senha=\"{Senha}\">";
            

            foreach (var respostaEspelhamento in respostaEspelhamentoPendente.EspelhamentoPendenteVeiculo)
            {
                var id = 1;
                request = request + $@"<espelhamento tipo=""1"">
                                         <id>{id}</id>
                                         <veiID>{respostaEspelhamento.VeiID}</veiID>
                                         <desc> Aceite espelhamento automatico</desc>
                                         <usuario>Integracao</usuario>
                                     </espelhamento>";

                id++;

            }

            request = request + "</RequestAREspelhamentoVeiculo>";

            var respostaRequest = ObterXML(request);

            var resposta = System.Text.Encoding.UTF8.GetString(respostaRequest); 
        }
        private void IntegrarEspelhamentoPendente()
        {
            if (DataUltimaRequiscaoEspelhamento.AddSeconds(TempoMinimoRequisicaoEspelhamento) < DateTime.Now)
            {
                SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Integrando dados do espelhamento", this.GetType().Name);
                DataUltimaRequiscaoEspelhamento = DateTime.Now;
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
            SGT.GerenciadorApp.Monitoramento.MonitoramentoUtils.GravarLogTracking("Integrando dados de mensagem/posição", this.GetType().Name);
            var ListaArquivos = Utilidades.File.ListarArquivosDiretorio(DiretorioRecebimento);
            ListaArquivos.Sort();

            foreach (var arquivo in ListaArquivos)
            {
               //ListaPosicaoAtualEquipamento.Clear();

                var xmlcompactado = Utilidades.File.LerArquivo(arquivo);

                var xml = Descompactar(xmlcompactado);

                if (!ValidarXML(xml))
                {
                    Utilidades.File.MoverArquivoParaDiretorio(arquivo, DiretorioErro);
                    continue;
                }

                XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.ResponseMensagemOnixSat));

                StringReader stringReader = new StringReader(xml);

                var respostas = (Dominio.ObjetosDeValor.ResponseMensagemOnixSat)serializer.Deserialize(stringReader);

                var gravouPosicao = GravarPosicoes(respostas);

                if (gravouPosicao)
                    Utilidades.File.MoverArquivoParaDiretorio(arquivo, DiretorioLido);

                GravarPosicaoAtual();

            }

        }
        private void GravarXMLPosicao(byte[] xml)
        {
            var arquivo = $@"{DiretorioRecebimento}Onixsat{DateTime.Now.ToString("yyyyMMdd_HHmmssfff")}.zip";
            Utilidades.File.SalvarTextoEmArquivo(arquivo, xml);
        }
        private void GravarXMLVeiculos(byte[] xml)
        {
            var arquivo = $@"{DiretorioListaVeiculos}Veiculos{DateTime.Now.ToString("yyyyMMdd_HHmmssfff")}.zip";
            Utilidades.File.SalvarTextoEmArquivo(arquivo, xml);
        }
        private static HttpWebRequest CreateRequest()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://webservice.onixsat.com.br");

            request.Method = "POST";

            request.ContentType = "text/xml";

            return request;
        }
        private static byte[] RequestXml(string strRequest)
        {
            byte[] result = null;

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

            catch
            {
                throw new Exception("Erro ao descompatcar arquivo");
            }

            return result;

        }
        private bool IsValidDecompress(byte[] data)
        {
            return true;
        }
        private string Descompactar(byte[] data)
        {
            if (IsValidDecompress(data))
            {
                try
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
                    return UTF8Encoding.UTF8.GetString(buffer);
                }
                catch 
                {
                    Servicos.Log.TratarErro("Falha ao descompactar arquivo no formato . gzip: "+ data);
                }
            }
            return null;
        }
        public static IntegracaoOnixSat GetInstance()
        {
            if (Instance == null)
                Instance = new IntegracaoOnixSat();


            return Instance;
        }
        public void QueueItem(int idEmpresa, string stringConexao, string stringConexaoAdmin, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

            if (!ListaTasks.ContainsKey(idEmpresa))
                this.IniciarThread(idEmpresa, stringConexao, stringConexaoAdmin, tipoServicoMultisoftware);
        }
    }
}


