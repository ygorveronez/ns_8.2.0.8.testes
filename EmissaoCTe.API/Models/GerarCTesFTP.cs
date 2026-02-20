using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Script.Serialization;

namespace EmissaoCTe.API
{
    public class GerarCTesFTP
    {
        #region Atributos

        private int Tempo = 300000;
        private ConcurrentDictionary<int, Task> ListaTasks;
        private ConcurrentQueue<int> ListaConsultaEmailCaixaEntrada;
        private static GerarCTesFTP Instance;

        #endregion

        #region Construtor

        public static GerarCTesFTP GetInstance()
        {
            if (Instance == null)
                Instance = new GerarCTesFTP();

            return Instance;
        }

        #endregion

        #region Métodos Público

        public void QueueItem(int idEmpresa, string stringConexao)
        {
            if (ListaTasks == null)
                ListaTasks = new ConcurrentDictionary<int, Task>();

            if (ListaConsultaEmailCaixaEntrada == null)
                ListaConsultaEmailCaixaEntrada = new ConcurrentQueue<int>();

            if (!ListaTasks.ContainsKey(idEmpresa))
            {
                this.IniciarThread(idEmpresa, stringConexao);
            }
        }

        #endregion

        #region Métodos Privados

        private void IniciarThread(int idEmpresa, string stringConexao)
        {
            var filaConsulta = new ConcurrentQueue<int>();

            filaConsulta.Enqueue(idEmpresa);

            Task task = new Task(() =>
            {
#if DEBUG
                Tempo = 6000;
#endif

                while (true)
                {
                    try
                    {
                        filaConsulta.Enqueue(idEmpresa);

                        using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                        {
                            VerificarFTP(unidadeDeTrabalho, stringConexao);
                            //Testar(unidadeDeTrabalho, stringConexao);
                        }

                        GC.Collect();

                        System.Threading.Thread.Sleep(Tempo);

                        if (!filaConsulta.TryDequeue(out idEmpresa))
                        {
                            Servicos.Log.TratarErro("Task parou a execução", "GerarFTP");
                            break;
                        }

                    }
                    catch (TaskCanceledException abort)
                    {
                        Servicos.Log.TratarErro(string.Concat("Task de consulta de objetos cancelada: ", abort.ToString()), "GerarFTP");
                        break;
                    }
                    catch (System.Threading.ThreadAbortException abortThread)
                    {
                        Servicos.Log.TratarErro(string.Concat("Thread de consulta de objetos cancelada: ", abortThread), "GerarFTP");
                        break;
                    }
                    catch (System.Exception ex)
                    {
                        Servicos.Log.TratarErro(ex, "GerarFTP");
                        System.Threading.Thread.Sleep(Tempo);
                    }
                }
            });

            if (ListaTasks.TryAdd(idEmpresa, task))
                task.Start();
            else
                Servicos.Log.TratarErro("Não foi possível adicionar a task à fila.", "GerarFTP");
        }

        private void Testar(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.LayoutEDI repEdiLayout = new Repositorio.LayoutEDI(unitOfWork);

            Dominio.Entidades.ImportacaoFTP importacaoFTP = new Dominio.Entidades.ImportacaoFTP()
            {
                Empresa = repEmpresa.BuscarPorCodigo(3),
                Cliente = repCliente.BuscarPorCPFCNPJ(13969629000196),
                LayoutEDI = repEdiLayout.BuscarPorCodigo(15137),
            };

            importacaoFTP.ArquivoSalvo = Servicos.FS.GetPath(@"C:\Temp\Mandae.TXT");
            importacaoFTP.TipoArquivo = Dominio.Enumeradores.TipoProcessamentoArquivoFTP.Texto;
            ProcessarArquivoTxt(ref importacaoFTP, false, false, Dominio.Enumeradores.TipoRateioFTP.PorNFe, stringConexao, unitOfWork);

            //importacaoFTP.ArquivoSalvo = @"C:\Arquivos\ImportacaoFTP\arquivo.xlsx";
            //importacaoFTP.ExtencaoArquivo = ".xlsx";
            //importacaoFTP.TipoArquivo = Dominio.Enumeradores.TipoProcessamentoArquivoFTP.XLSXRiachuelo;
            //ProcessarArquivoRiachuelo(ref importacaoFTP, false, false, Dominio.Enumeradores.TipoRateioFTP.Destinatario, stringConexao, unitOfWork);
        }

        private Dominio.Enumeradores.TipoProcessamentoArquivoFTP? ObterTipoPorcessamentoArquivo(Dominio.Entidades.ImportacaoFTP importacaoFTP)
        {
            Dominio.Enumeradores.TipoProcessamentoArquivoFTP tipoProcessamentoArquivo = importacaoFTP.TipoArquivo;

            if (tipoProcessamentoArquivo != Dominio.Enumeradores.TipoProcessamentoArquivoFTP.PorExtensao)
                return tipoProcessamentoArquivo;

            string extensao = importacaoFTP.ExtencaoArquivo.ToLower();

            if (extensao == ".txt")
                return Dominio.Enumeradores.TipoProcessamentoArquivoFTP.Texto;
            else if (extensao == ".xml")
                return Dominio.Enumeradores.TipoProcessamentoArquivoFTP.XML;
            else if (extensao == ".csv")
                return Dominio.Enumeradores.TipoProcessamentoArquivoFTP.CSV;
            else if (extensao == ".xls" || extensao == ".xlsx")
                return Dominio.Enumeradores.TipoProcessamentoArquivoFTP.XLSX;

            return null;
        }

        private void ProcessarArquivoTxt(ref Dominio.Entidades.ImportacaoFTP importacaoFTP, bool gerarNFSe, bool emitirDocumento, Dominio.Enumeradores.TipoRateioFTP rateio, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Stream stream = Utilidades.IO.FileStorageService.Storage.OpenRead(importacaoFTP.ArquivoSalvo);

            var encoding = Encoding.UTF8;
            //if (importacaoFTP.Cliente.CPF_CNPJ == 19782476000150) //Mandae usa encoding diferente
            //    encoding = Encoding.ASCII;// Encoding.GetEncoding("iso-8859-1");

            Servicos.LeituraEDI serLeituraEDI = new Servicos.LeituraEDI(importacaoFTP.Empresa, importacaoFTP.LayoutEDI, stream, unitOfWork, 0, 0, 0, 0, 0, 0, 0, 0, true, true, encoding);
            List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais = serLeituraEDI.GerarNotasFiscais();

            stream.Close();

            if (notasFiscais == null || notasFiscais.Count() == 0)
            {
                importacaoFTP.Status = Dominio.Enumeradores.StatusImportacaoFTP.ErroProcessamento;
                importacaoFTP.MensagemProcessamento = "Arquivo " + importacaoFTP.ArquivoSalvo + " não localizado.";

                return;
            }

            if (!GerarCTes(importacaoFTP.Empresa, importacaoFTP.Cliente, importacaoFTP.UtilizarContratanteComoTomador, notasFiscais, gerarNFSe, emitirDocumento, rateio, stringConexao, unitOfWork, out string erro))
            {
                importacaoFTP.Status = Dominio.Enumeradores.StatusImportacaoFTP.ErroProcessamento;
                importacaoFTP.MensagemProcessamento = erro;

                return;
            }

            importacaoFTP.Status = Dominio.Enumeradores.StatusImportacaoFTP.Processado;
            importacaoFTP.MensagemProcessamento = "";
        }

        private void ProcessarArquivoXml(ref Dominio.Entidades.ImportacaoFTP importacaoFTP, bool emitirDocumento, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);

            Stream stream = Utilidades.IO.FileStorageService.Storage.OpenRead(importacaoFTP.ArquivoSalvo);
            Servicos.NFe serNFe = new Servicos.NFe(unitOfWork);
            dynamic xmlNotaFiscal = serNFe.ObterDocumentoPorXML(stream, importacaoFTP.Empresa.Codigo, null, unitOfWork);
            stream.Close();

            if (xmlNotaFiscal == null)
            {
                importacaoFTP.Status = Dominio.Enumeradores.StatusImportacaoFTP.ErroProcessamento;
                importacaoFTP.MensagemProcessamento = "Arquivo " + importacaoFTP.ArquivoSalvo + " invalido para geração de CTe.";
                return;
            }

            string chaveNFe = (string)xmlNotaFiscal.GetType().GetProperty("Chave").GetValue(xmlNotaFiscal, null);
            string stringXmlNFe = Newtonsoft.Json.JsonConvert.SerializeObject(xmlNotaFiscal);

            Dominio.ObjetosDeValor.XMLNFe xmlNFe = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.XMLNFe>(stringXmlNFe);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorNFePlaca(importacaoFTP.Empresa.Codigo, importacaoFTP.Empresa.TipoAmbiente, "", "", chaveNFe, "", DateTime.MinValue, new string[] { "S", "A", "R" });

            if (cte != null)
            {
                importacaoFTP.Status = Dominio.Enumeradores.StatusImportacaoFTP.ErroProcessamento;
                importacaoFTP.MensagemProcessamento = "Já existe CTe salvo com a NFe " + chaveNFe + " .";
                return;
            }

            bool notaSalva = false;
            List<Dominio.ObjetosDeValor.XMLNFe> documentos = new List<Dominio.ObjetosDeValor.XMLNFe> { xmlNFe };
            List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = serCTe.SalvarCTePorObjetoNFe(importacaoFTP.Empresa, null, null, false, false, false, false, documentos, unitOfWork, 0, 0, 0, 0, !emitirDocumento, 0, 0, 0, Dominio.Enumeradores.TipoRateioTabelaFreteValor.Nenhum, null, 0, string.Empty, string.Empty, string.Empty, 0, ref notaSalva);

            if ((listaCTes != null && listaCTes.Count > 0) || notaSalva)
            {
                importacaoFTP.Status = Dominio.Enumeradores.StatusImportacaoFTP.Processado;
                importacaoFTP.MensagemProcessamento = "";
                return;
            }

            importacaoFTP.Status = Dominio.Enumeradores.StatusImportacaoFTP.ErroProcessamento;
            importacaoFTP.MensagemProcessamento = "Não foi possível gerar CTe da NFe " + chaveNFe + " .";
        }

        private void ProcessarArquivoRiachuelo(ref Dominio.Entidades.ImportacaoFTP importacaoFTP, bool gerarNFSe, bool emitirDocumento, Dominio.Enumeradores.TipoRateioFTP rateio, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            string extensao = importacaoFTP.ExtencaoArquivo.ToLower();
            Stream stream = Utilidades.IO.FileStorageService.Storage.OpenRead(importacaoFTP.ArquivoSalvo);
            List<List<string>> linhas;
            if (extensao == ".csv")
                linhas = Servicos.Arquivo.ObterArquivoCSV(stream);
            else
                linhas = Servicos.Arquivo.ObterArquivoExcel(stream, extensao == ".xlsx");
            stream.Close();

            List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
            bool isPrimeiraLinhaCabecalho = false;
            var mapeamentoNomeIndex = new
            {
                Emissor = 0,
                NumeroPedido = 2,

                DestinatarioCPFCNPJ = 8,
                DestinatarioNome = 5,
                DestinatarioEmail = 6,
                DestinatarioNumeroTelefone = 7,
                DestinatarioIE = 9,
                DestinatarioRua = 10,
                DestinatarioNumero = 11,
                DestinatarioComplemento = 12,
                DestinatarioCidadeIBGE = 28,
                DestinatarioCEP = 16,

                NotaFiscalData = 17,
                NotaFiscalChave = 18,
                NotaFiscalSerie = 19,
                NotaFiscalNumero = 20,
                NotaFiscalTotalVolumes = 22,
                NotaFiscalValorTotal = 23,

                ProdutoPesoEmG = 24,
            };

            for (int i = isPrimeiraLinhaCabecalho ? 1 : 0, s = linhas.Count(); i < s; i++)
            {
                List<string> celulas = linhas[i];

                double cnpjEmitente = double.Parse(Utilidades.String.OnlyNumbers(celulas[mapeamentoNomeIndex.Emissor]));
                Dominio.Entidades.Cliente emitente = repCliente.BuscarPorCPFCNPJ(cnpjEmitente);

                if (emitente == null)
                    throw new Exception($"Não foi possível encontrar o Emitente {celulas[mapeamentoNomeIndex.Emissor]}");

                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa destinatario = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa()
                {
                    CPFCNPJ = Utilidades.String.OnlyNumbers(celulas[mapeamentoNomeIndex.DestinatarioCPFCNPJ]),
                    RazaoSocial = celulas[mapeamentoNomeIndex.DestinatarioNome],
                    RGIE = celulas[mapeamentoNomeIndex.DestinatarioIE],
                    Email = celulas[mapeamentoNomeIndex.DestinatarioEmail],
                    Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco()
                    {
                        Telefone = celulas[mapeamentoNomeIndex.DestinatarioNumeroTelefone],
                        Numero = celulas[mapeamentoNomeIndex.DestinatarioNumero],
                        Complemento = celulas[mapeamentoNomeIndex.DestinatarioComplemento],
                        CEP = celulas[mapeamentoNomeIndex.DestinatarioCEP],
                        Logradouro = celulas[mapeamentoNomeIndex.DestinatarioRua],
                        Cidade = new Dominio.ObjetosDeValor.Localidade()
                        {
                            IBGE = ParseInt(celulas[mapeamentoNomeIndex.DestinatarioCidadeIBGE])
                        }
                    },
                };

                Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal nota = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal()
                {
                    Emitente = ConverterClienteEmObjetoPessoa(emitente, unitOfWork),
                    Destinatario = BuscarOuCriarCliente(destinatario, importacaoFTP.Empresa.Codigo, unitOfWork),
                    DataEmissao = celulas[mapeamentoNomeIndex.NotaFiscalData],
                    Chave = celulas[mapeamentoNomeIndex.NotaFiscalChave],
                    Serie = celulas[mapeamentoNomeIndex.NotaFiscalSerie],
                    Numero = ParseInt(celulas[mapeamentoNomeIndex.NotaFiscalNumero]),
                    VolumesTotal = ParseInt(celulas[mapeamentoNomeIndex.NotaFiscalTotalVolumes]),
                    Valor = ParseDecimal(celulas[mapeamentoNomeIndex.NotaFiscalValorTotal]),
                    NumeroPedido = celulas[mapeamentoNomeIndex.NumeroPedido],
                    Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>()
                    {
                        new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto()
                        {
                            DescricaoProduto = "Diversos",
                            Quantidade = ParseInt(celulas[mapeamentoNomeIndex.NotaFiscalTotalVolumes]),
                            PesoUnitario = ParseDecimal(celulas[mapeamentoNomeIndex.ProdutoPesoEmG]),
                        }
                    }
                };

                notasFiscais.Add(nota);
            }

            if (!GerarCTes(importacaoFTP.Empresa, importacaoFTP.Cliente, importacaoFTP.UtilizarContratanteComoTomador, notasFiscais, gerarNFSe, emitirDocumento, rateio, stringConexao, unitOfWork, out string erro))
            {
                importacaoFTP.Status = Dominio.Enumeradores.StatusImportacaoFTP.ErroProcessamento;
                importacaoFTP.MensagemProcessamento = erro;
                return;
            }

            importacaoFTP.Status = Dominio.Enumeradores.StatusImportacaoFTP.Processado;
            importacaoFTP.MensagemProcessamento = "";
        }

        private void ProcessarIntegracao(ref Dominio.Entidades.ImportacaoFTP importacaoFTP, string stringConexao, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Enumeradores.TipoProcessamentoArquivoFTP? tipoProcessamentoArquivo = ObterTipoPorcessamentoArquivo(importacaoFTP);
            Repositorio.ConfiguracaoFTP repConfigFTP = new Repositorio.ConfiguracaoFTP(unitOfWork);

            if (!Utilidades.IO.FileStorageService.Storage.Exists(importacaoFTP.ArquivoSalvo))
            {
                importacaoFTP.Status = Dominio.Enumeradores.StatusImportacaoFTP.ErroProcessamento;
                importacaoFTP.MensagemProcessamento = "Arquivo " + importacaoFTP.ArquivoSalvo + " não encontrado.";
                return;
            }

            Dominio.Entidades.ConfiguracaoFTP configFTP = repConfigFTP.BuscarPorConfiguracaoClienteTipo(importacaoFTP.Empresa.Configuracao.Codigo, importacaoFTP.Cliente.CPF_CNPJ, Dominio.Enumeradores.TipoArquivoFTP.ImportacaoNOTFIS);
            if (configFTP == null)
            {
                importacaoFTP.Status = Dominio.Enumeradores.StatusImportacaoFTP.ErroProcessamento;
                importacaoFTP.MensagemProcessamento = "Sem configuração da empresa para importação do FTP.";
                return;
            }

            if (tipoProcessamentoArquivo == Dominio.Enumeradores.TipoProcessamentoArquivoFTP.Texto)
            {
                ProcessarArquivoTxt(ref importacaoFTP, configFTP.GerarNFSe, configFTP.EmitirDocumento, configFTP.Rateio, stringConexao, unitOfWork);
                return;
            }

            if (tipoProcessamentoArquivo == Dominio.Enumeradores.TipoProcessamentoArquivoFTP.XML)
            {
                ProcessarArquivoXml(ref importacaoFTP, configFTP.EmitirDocumento, stringConexao, unitOfWork);
                return;
            }

            if (tipoProcessamentoArquivo == Dominio.Enumeradores.TipoProcessamentoArquivoFTP.XLSXRiachuelo)
            {
                ProcessarArquivoRiachuelo(ref importacaoFTP, configFTP.GerarNFSe, configFTP.EmitirDocumento, configFTP.Rateio, stringConexao, unitOfWork);
                return;
            }

            importacaoFTP.Status = Dominio.Enumeradores.StatusImportacaoFTP.ErroProcessamento;
            importacaoFTP.MensagemProcessamento = "Extensão do arquivo não implementada.";
        }

        private void MoverArquivoPosProcessamento(ref Dominio.Entidades.ImportacaoFTP importacaoFTP, string caminhoBase)
        {
            try
            {
                string diretorioArquivos;

                if (importacaoFTP.Status == Dominio.Enumeradores.StatusImportacaoFTP.Processado)
                    diretorioArquivos = Utilidades.IO.FileStorageService.Storage.Combine(caminhoBase, "Importados");
                else
                    diretorioArquivos = Utilidades.IO.FileStorageService.Storage.Combine(caminhoBase, "NaoImportados");

                string arquivoImportado = Utilidades.IO.FileStorageService.Storage.Combine(diretorioArquivos, Path.GetFileName(importacaoFTP.ArquivoSalvo));

                if (Utilidades.IO.FileStorageService.Storage.Exists(importacaoFTP.ArquivoSalvo))
                    Utilidades.IO.FileStorageService.Storage.Move(importacaoFTP.ArquivoSalvo, arquivoImportado);
            }
            catch (System.Exception ex)
            {
                Servicos.Log.TratarErro("Falha na Mover Arquivo Processado - FTP: " + ex, "GerarFTP");

                importacaoFTP.Status = Dominio.Enumeradores.StatusImportacaoFTP.ErroProcessamento;
                importacaoFTP.MensagemProcessamento = ex.Message;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ConverterClienteEmObjetoPessoa(Dominio.Entidades.Cliente cliente, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica srvNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unitOfWork);

            return srvNotaFiscalEletronica.ConverterObjetoPessoa(cliente);
        }

        private Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa BuscarOuCriarCliente(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Cliente svsCliente = new Servicos.Cliente(unitOfWork.StringConexao);

            var pessoaDestinatario = svsCliente.ConverterObjetoValorPessoa(pessoa, "Destinatario", unitOfWork, codigoEmpresa, false, true);

            if (!pessoaDestinatario.Status)
                throw new Exception(pessoaDestinatario.Mensagem);

            return ConverterClienteEmObjetoPessoa(pessoaDestinatario.cliente, unitOfWork);
        }

        private void VerificarFTP(Repositorio.UnitOfWork unitOfWork, string stringConexao)
        {
            string diretorioArquivos = System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivos"];

            if (!string.IsNullOrWhiteSpace(diretorioArquivos))
                diretorioArquivos = Utilidades.IO.FileStorageService.Storage.Combine(diretorioArquivos, "FTP");

            Repositorio.ImportacaoFTP repImportacaoFTP = new Repositorio.ImportacaoFTP(unitOfWork);
            List<Dominio.Entidades.ImportacaoFTP> importacoesFTP = repImportacaoFTP.BuscarPorStatus(Dominio.Enumeradores.StatusImportacaoFTP.Salvo);

            for (var i = 0; i < importacoesFTP.Count; i++)
            {
                Dominio.Entidades.ImportacaoFTP importacaoFTP = importacoesFTP[i];

                importacaoFTP.DataProcessamento = DateTime.Now;

                try
                {
                    ProcessarIntegracao(ref importacaoFTP, stringConexao, unitOfWork);
                }
                catch (System.Exception ex)
                {
                    Servicos.Log.TratarErro("Falha na importação EDI - FTP: " + ex, "GerarFTP");

                    importacaoFTP.Status = Dominio.Enumeradores.StatusImportacaoFTP.ErroProcessamento;
                    importacaoFTP.MensagemProcessamento = ex.Message;
                }

                MoverArquivoPosProcessamento(ref importacaoFTP, diretorioArquivos);

                repImportacaoFTP.Atualizar(importacaoFTP);
            }
        }

        private bool GerarCTes(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Cliente contratante, bool utilizarContratanteComoTomador, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais, bool gerarNFSe, bool emitirDocumento, Dominio.Enumeradores.TipoRateioFTP rateio, string stringConexao, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            try
            {
                // Repositorios e Servicos
                Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);
                Servicos.NFSe serNFSe = new Servicos.NFSe(unitOfWork);
                Servicos.Cliente serCliente = new Servicos.Cliente(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unitOfWork);
                Repositorio.XMLNotaFiscalEletronica repXMLNotaFiscalEletronica = new Repositorio.XMLNotaFiscalEletronica(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                // Lista de ctes para retorno
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                List<Dominio.Entidades.NFSe> listaNFSes = new List<Dominio.Entidades.NFSe>();

                // Cada grupo de lista que é retornado, se refere a um CTe/NFSe
                List<List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>> notasParaDOC = AgruparNotasPorCriterios(notasFiscais, rateio);

                for (var i = 0; i < notasParaDOC.Count; i++)
                {
                    decimal peso = 0;
                    int volumes = 0;
                    string chaveNFe = string.Empty;

                    Dominio.ObjetosDeValor.CTe.CTeNFSe cteNFSe = new Dominio.ObjetosDeValor.CTe.CTeNFSe();
                    cteNFSe.Documentos = new List<Dominio.ObjetosDeValor.CTe.Documento>();

                    int.TryParse(notasParaDOC[i].FirstOrDefault().IBGEInicioPrestacao, out int ibgeLocalidadeInicio);
                    if (utilizarContratanteComoTomador && contratante != null)
                        ibgeLocalidadeInicio = contratante.Localidade.CodigoIBGE;

                    cteNFSe.Emitente = Servicos.Empresa.ObterEmpresaCTE(empresa);
                    cteNFSe.Remetente = this.GerarClienteCTe(notasParaDOC[i].FirstOrDefault().Emitente, empresa, stringConexao, unitOfWork);
                    cteNFSe.Destinatario = this.GerarClienteCTe(notasParaDOC[i].FirstOrDefault().Destinatario, empresa, stringConexao, unitOfWork);
                    cteNFSe.CodigoIBGECidadeInicioPrestacao = ibgeLocalidadeInicio > 0 && repLocalidade.BuscarPorCodigoIBGE(ibgeLocalidadeInicio) != null ? ibgeLocalidadeInicio : cteNFSe.Remetente.CodigoIBGECidade;
                    cteNFSe.CodigoIBGECidadeTerminoPrestacao = cteNFSe.Destinatario.CodigoIBGECidade;

                    bool falhaSubcontracao = false;
                    if (!string.IsNullOrWhiteSpace(notasParaDOC[i].FirstOrDefault().ChaveCTe))
                    {
                        if (!Utilidades.Validate.ValidarChave(notasParaDOC[i].FirstOrDefault().ChaveCTe))
                        {
                            Servicos.Log.TratarErro("Chave do CTe " + notasParaDOC[i].FirstOrDefault().ChaveCTe + " não é valida.", "GerarFTP");
                            falhaSubcontracao = true;
                        }

                        string cnpjEmissorCTeAnterior = notasParaDOC[i].FirstOrDefault().ChaveCTe.Substring(6, 14);
                        Dominio.Entidades.Cliente emissorCTeAnterior = repCliente.BuscarPorCPFCNPJ(double.Parse(cnpjEmissorCTeAnterior));
                        if (string.IsNullOrWhiteSpace(cnpjEmissorCTeAnterior) || emissorCTeAnterior == null)
                        {
                            Servicos.Log.TratarErro("Emissor do CTe " + notasParaDOC[i].FirstOrDefault().ChaveCTe + " não possui cadastro como cliente.", "GerarFTP");
                            falhaSubcontracao = true;
                        }

                        cteNFSe.DocumentosTransporteAnteriores = new List<Dominio.ObjetosDeValor.CTe.DocumentoTransporteAnterior>();
                        Dominio.ObjetosDeValor.CTe.DocumentoTransporteAnterior docAnterior = new Dominio.ObjetosDeValor.CTe.DocumentoTransporteAnterior();
                        docAnterior.Chave = notasParaDOC[i].FirstOrDefault().ChaveCTe;
                        docAnterior.Emissor = new Dominio.ObjetosDeValor.CTe.Cliente();
                        docAnterior.Emissor.CPFCNPJ = emissorCTeAnterior.CPF_CNPJ_SemFormato;
                        docAnterior.Emissor.CodigoAtividade = emissorCTeAnterior.Atividade.Codigo;
                        docAnterior.Emissor.RGIE = emissorCTeAnterior.IE_RG;
                        docAnterior.Emissor.RazaoSocial = emissorCTeAnterior.Nome;
                        docAnterior.Emissor.NomeFantasia = emissorCTeAnterior.NomeFantasia;
                        docAnterior.Emissor.Endereco = emissorCTeAnterior.Endereco;
                        docAnterior.Emissor.Numero = emissorCTeAnterior.Numero;
                        docAnterior.Emissor.Bairro = emissorCTeAnterior.Bairro;
                        docAnterior.Emissor.CEP = emissorCTeAnterior.CEP;
                        docAnterior.Emissor.CodigoIBGECidade = emissorCTeAnterior.Localidade.CodigoIBGE;
                        cteNFSe.DocumentosTransporteAnteriores.Add(docAnterior);

                        if (notasParaDOC[i].FirstOrDefault().TipoDocumento == "RED")
                            cteNFSe.TipoServico = Dominio.Enumeradores.TipoServico.Redespacho;
                        else
                            cteNFSe.TipoServico = Dominio.Enumeradores.TipoServico.SubContratacao;

                        cteNFSe.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                        cteNFSe.Tomador = docAnterior.Emissor;
                    }
                    else if (utilizarContratanteComoTomador && contratante != null)
                    {
                        cteNFSe.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
                        Dominio.ObjetosDeValor.CTe.Cliente tomador = new Dominio.ObjetosDeValor.CTe.Cliente();
                        tomador.CPFCNPJ = contratante.CPF_CNPJ_SemFormato;
                        tomador.CodigoAtividade = contratante.Atividade.Codigo;
                        tomador.RGIE = contratante.IE_RG;
                        tomador.RazaoSocial = contratante.Nome;
                        tomador.NomeFantasia = contratante.NomeFantasia;
                        tomador.Endereco = contratante.Endereco;
                        tomador.Numero = contratante.Numero;
                        tomador.Bairro = contratante.Bairro;
                        tomador.CEP = contratante.CEP;
                        tomador.CodigoIBGECidade = contratante.Localidade.CodigoIBGE;

                        cteNFSe.Tomador = tomador;
                    }

                    if (!falhaSubcontracao)
                    {
                        Dominio.Entidades.DadosCliente dadosCliente = null;
                        if (utilizarContratanteComoTomador && contratante != null)
                            dadosCliente = repDadosCliente.Buscar(empresa.Codigo, contratante.CPF_CNPJ);
                        else
                            dadosCliente = cteNFSe.Remetente == null ? null : repDadosCliente.Buscar(empresa.Codigo, double.Parse(cteNFSe.Remetente.CPFCNPJ));

                        //foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal nota in notasDOC)
                        for (var j = 0; j < notasParaDOC[i].Count; j++)
                        {
                            Dominio.ObjetosDeValor.CTe.Documento docNF = GerarDocumentoCTe(notasParaDOC[i][j], unitOfWork);
                            chaveNFe = docNF.ChaveNFE; //Informação utilizada para verificar se já existe CTe/NFSe com a nota
                            cteNFSe.Documentos.Add(docNF);

                            peso += docNF.Peso;
                            volumes += docNF.Volume;
                            cteNFSe.ValorTotalMercadoria += docNF.Valor;
                            cteNFSe.ValorFrete += notasParaDOC[i][j].ValorFrete;
                        }

                        if (peso > 0)
                        {
                            cteNFSe.QuantidadesCarga = new List<Dominio.ObjetosDeValor.CTe.QuantidadeCarga>();

                            Dominio.ObjetosDeValor.CTe.QuantidadeCarga quantidadeCarga = new Dominio.ObjetosDeValor.CTe.QuantidadeCarga();
                            quantidadeCarga.Descricao = "Kilogramas";
                            quantidadeCarga.Quantidade = peso;
                            quantidadeCarga.UnidadeMedida = "01";
                            cteNFSe.QuantidadesCarga.Add(quantidadeCarga);
                        }

                        if (volumes > 0)
                        {
                            if (cteNFSe.QuantidadesCarga == null)
                                cteNFSe.QuantidadesCarga = new List<Dominio.ObjetosDeValor.CTe.QuantidadeCarga>();

                            Dominio.ObjetosDeValor.CTe.QuantidadeCarga quantidadeCarga = new Dominio.ObjetosDeValor.CTe.QuantidadeCarga();
                            quantidadeCarga.Descricao = "Volumes";
                            quantidadeCarga.Quantidade = volumes;
                            quantidadeCarga.UnidadeMedida = "03";
                            cteNFSe.QuantidadesCarga.Add(quantidadeCarga);
                        }

                        // Quando for prestação de serviço na mesma cidade, automaticamente gera um NFSe
                        // Mas isso ocorre somente quando a empresa possui configuração para emitir NFSe
                        if (cteNFSe.CodigoIBGECidadeInicioPrestacao == cteNFSe.CodigoIBGECidadeTerminoPrestacao && gerarNFSe && !string.IsNullOrWhiteSpace(empresa.Configuracao.SerieRPSNFSe) && cteNFSe.TipoServico != Dominio.Enumeradores.TipoServico.SubContratacao)
                        {
                            bool armazenaNotasParaGerarPorPeriodo = dadosCliente != null && dadosCliente.ArmazenaNotasParaGerarPorPeriodo;
                            if (armazenaNotasParaGerarPorPeriodo)
                            {
                                for (var k = 0; k < cteNFSe.Documentos.Count(); k++)
                                {
                                    Dominio.Entidades.XMLNotaFiscalEletronica notaImportada = repXMLNotaFiscalEletronica.BuscarPorChaveNFe(cteNFSe.Documentos[k].ChaveNFE);
                                    if (notaImportada == null)
                                    {
                                        notaImportada = new Dominio.Entidades.XMLNotaFiscalEletronica();
                                        notaImportada.Chave = cteNFSe.Documentos[k].ChaveNFE;
                                        DateTime dataEmissao;
                                        if (!DateTime.TryParseExact(cteNFSe.Documentos[k].DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                                            dataEmissao = DateTime.Now;
                                        notaImportada.DataEmissao = dataEmissao;
                                        notaImportada.Emitente = serCTe.ObterCliente(empresa, cteNFSe.Remetente, unitOfWork); // repCliente.BuscarPorCPFCNPJ(double.Parse(cteNFSe.Remetente.CPFCNPJ));
                                        notaImportada.Destinatario = serCTe.ObterCliente(empresa, cteNFSe.Destinatario, unitOfWork); //repCliente.BuscarPorCPFCNPJ(double.Parse(cteNFSe.Destinatario.CPFCNPJ));
                                        notaImportada.Empresa = empresa;
                                        notaImportada.FormaDePagamento = ((int)cteNFSe.TipoPagamento).ToString();
                                        notaImportada.Numero = cteNFSe.Documentos[k].Numero;
                                        notaImportada.Peso = cteNFSe.Documentos[k].Peso;
                                        notaImportada.Valor = cteNFSe.Documentos[k].Valor;
                                        notaImportada.GeradoDocumento = false;
                                        notaImportada.Volumes = cteNFSe.Documentos[k].Volume;
                                        notaImportada.Pedido = cteNFSe.Documentos[k].NumeroPedido;
                                        notaImportada.Modalidade = cteNFSe.Documentos[k].Modalidade;
                                        notaImportada.Contratante = contratante;
                                        notaImportada.UtilizarContratanteComoTomador = utilizarContratanteComoTomador;
                                        notaImportada.ValorDoFrete = serNFSe.CalcularFretePorNotaImportada(notaImportada, empresa.Codigo, "", unitOfWork);
                                        repXMLNotaFiscalEletronica.Inserir(notaImportada);

                                        Servicos.LsTranslog svcLsTranslog = new Servicos.LsTranslog(unitOfWork);
                                        svcLsTranslog.SalvarNFeParaIntegracao(notaImportada, empresa.Codigo, unitOfWork);
                                    }
                                }
                            }
                            else
                            {
                                // NFSe
                                Dominio.Entidades.NFSe nfseAnterior = null;
                                if (!string.IsNullOrWhiteSpace(chaveNFe))
                                    nfseAnterior = repNFSe.BuscarPorNFe(empresa.Codigo, empresa.TipoAmbiente, chaveNFe, DateTime.MinValue, new Dominio.Enumeradores.StatusNFSe[] { Dominio.Enumeradores.StatusNFSe.EmDigitacao, Dominio.Enumeradores.StatusNFSe.Autorizado, Dominio.Enumeradores.StatusNFSe.Rejeicao });

                                if (nfseAnterior == null)
                                {
                                    Dominio.Entidades.NFSe notaServico = serNFSe.GerarNFSePorObjetoObjetoCTe(cteNFSe, unitOfWork, Dominio.Enumeradores.StatusNFSe.EmDigitacao);
                                    listaNFSes.Add(notaServico);
                                }
                                else
                                    Servicos.Log.TratarErro("NFe (" + chaveNFe + ") já importada na NFSe " + nfseAnterior.Numero + ".", "GerarFTP");
                            }
                        }
                        else
                        {
                            // CTe
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteAnterior = null;
                            if (!string.IsNullOrWhiteSpace(chaveNFe))
                                cteAnterior = repCTe.BuscarPorNFePlaca(empresa.Codigo, empresa.TipoAmbiente, "", "", chaveNFe, "", DateTime.MinValue, new string[] { "S", "A", "R" });
                            if (cteAnterior == null)
                            {
                                Dominio.ObjetosDeValor.CTe.CTe cte = serCTe.ConverteObjetoCTeNFSe(cteNFSe);
                                Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = serCTe.GerarCTePorObjeto(cte, 0, unitOfWork, "1", 0, "S");
                                listaCTes.Add(conhecimento);
                            }
                            else
                                Servicos.Log.TratarErro("NFe (" + chaveNFe + ") já importada no CTe " + cteAnterior.Numero + ".", "GerarFTP");
                        }
                    }
                }

                // Emitir documentos
                if (emitirDocumento)
                {
                    // Emite os ctes
                    //foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento in (from o in listaCTes where o.ValorFrete > 0 select o).ToList())
                    for (var i = 0; i < listaCTes.Count; i++)
                    {
                        if (listaCTes[i].ValorFrete > 0)
                        {
                            if (!serCTe.Emitir(listaCTes[i].Codigo, empresa.Codigo, unitOfWork))
                                Servicos.Log.TratarErro("O CT-e " + listaCTes[i].Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porem, ocorreu uma falha ao enviar-lo ao Sefaz.", "GerarFTP");
                            else if (!serCTe.AdicionarCTeNaFilaDeConsulta(listaCTes[i], unitOfWork))
                                Servicos.Log.TratarErro("O CT-e " + listaCTes[i].Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porem, nao foi possivel adiciona-lo na fila de consulta.", "GerarFTP");
                        }
                    }

                    // Emite os nfse
                    //foreach (Dominio.Entidades.NFSe notaFiscal in (from o in listaNFSes where o.ValorServicos > 0 select o).ToList())
                    for (var i = 0; i < listaNFSes.Count; i++)
                    {
                        if (listaNFSes[i].ValorServicos > 0)
                        {
                            if (!serNFSe.Emitir(listaNFSes[i], unitOfWork))
                                Servicos.Log.TratarErro("A NFS-e " + listaNFSes[i].Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porem, ocorreu uma falha ao enviar-la ao Sefaz.", "GerarFTP");
                            else if (!serNFSe.AdicionarNFSeNaFilaDeConsulta(listaNFSes[i], unitOfWork))
                                Servicos.Log.TratarErro("A NFS-e " + listaNFSes[i].Numero.ToString() + " da empresa " + empresa.CNPJ + " foi salvo, porem, nao foi possivel adiciona-la na fila de consulta.", "GerarFTP");
                        }
                    }
                }

                erro = string.Empty;
                return true;
            }
            catch (System.Exception ex)
            {
                erro = "Falha na GerarCTes FTP: " + ex.Message;

                Servicos.Log.TratarErro(erro, "GerarFTP");

                return false;
            }
            //finally
            //{
            //    unitOfWork.Dispose();
            //}
        }

        private List<List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>> AgruparNotasPorCriterios(List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais, Dominio.Enumeradores.TipoRateioFTP rateio)
        {
            // Configuração de emissao
            bool agruparRemetente = false;
            bool agruparDestinatario = false;
            bool ctePorNFe = false;

            if (rateio == Dominio.Enumeradores.TipoRateioFTP.Destinatario)
            {
                agruparDestinatario = true;
                agruparRemetente = false;
                ctePorNFe = false;
            }
            else if (rateio == Dominio.Enumeradores.TipoRateioFTP.Remetente)
            {
                agruparRemetente = true;
                agruparDestinatario = false;
                ctePorNFe = false;
            }
            else if (rateio == Dominio.Enumeradores.TipoRateioFTP.RemetenteDestinatario)
            {
                agruparDestinatario = true;
                agruparRemetente = true;
                ctePorNFe = false;
            }
            else
            {
                ctePorNFe = true;
                agruparRemetente = false;
                agruparDestinatario = false;
            }

            // Metodo de separacao
            List<List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>> notasAgrupadas = new List<List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>>();
            Dictionary<string, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>> notasFiscaisAgrupadas = new Dictionary<string, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>>();

            for (var i = 0; i < notasFiscais.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = notasFiscais[i];

                // Chave de agrupamento:
                // Destinatario: Destinatario.CPFCNPJ
                // Remetente: Emitente.CPFCNPJ
                // DestinatarioRemetente: Destinatario.CPFCNPJ+Emitente.CPFCNPJ
                // CTePorNF: counter
                string chave = string.Empty;
                if (agruparDestinatario) chave += notaFiscal.Destinatario.CPFCNPJ;
                if (agruparRemetente) chave += notaFiscal.Emitente.CPFCNPJ;
                if (ctePorNFe) chave = (i + 1).ToString();

                // Auxiliar das notas do dicionario
                List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasDicionario = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();

                // Verifica se existe uma composição no dicionario
                // Caso existe, pegar as avarias ja agrupadas
                if (notasFiscaisAgrupadas.ContainsKey(chave)) notasDicionario = notasFiscaisAgrupadas[chave];

                // Adiciona a nova avaria
                notasDicionario.Add(notaFiscal);

                // Seta novamente no dicionario
                notasFiscaisAgrupadas[chave] = notasDicionario;
            }

            //-- Gera a lista de lista
            // Converte os dados do dicionario numa lista de lista de notas
            List<string> agrupamentos = notasFiscaisAgrupadas.Keys.ToList();

            // Itera as chaves para pegar as listas do dicionario
            foreach (string chave in agrupamentos)
                notasAgrupadas.Add(notasFiscaisAgrupadas[chave]);

            return notasAgrupadas;
        }

        private Dominio.ObjetosDeValor.CTe.Cliente GerarClienteCTe(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa pessoa, Dominio.Entidades.Empresa empresa, string stringConexao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.ObjetosDeValor.CTe.Cliente cliente = new Dominio.ObjetosDeValor.CTe.Cliente();

            string telefone1 = pessoa.Endereco != null ? Utilidades.String.OnlyNumbers(pessoa.Endereco.Telefone) : string.Empty;
            string telefone2 = pessoa.Endereco != null ? Utilidades.String.OnlyNumbers(pessoa.Endereco.Telefone2) : string.Empty;

            cliente.CPFCNPJ = pessoa.CPFCNPJ;
            cliente.RGIE = !string.IsNullOrWhiteSpace(pessoa.RGIE) ? pessoa.RGIE : "ISENTO";
            cliente.RazaoSocial = pessoa.RazaoSocial;
            cliente.NomeFantasia = pessoa.NomeFantasia;
            cliente.Endereco = pessoa.Endereco != null && !string.IsNullOrWhiteSpace(pessoa.Endereco.Logradouro) ? pessoa.Endereco.Logradouro : "RUA";
            cliente.Bairro = pessoa.Endereco != null && !string.IsNullOrWhiteSpace(pessoa.Endereco.Bairro) ? pessoa.Endereco.Bairro : "BAIRRO";
            cliente.Complemento = pessoa.Endereco?.Complemento;
            cliente.Numero = pessoa.Endereco != null && !string.IsNullOrWhiteSpace(pessoa.Endereco.Numero) ? pessoa.Endereco.Numero : "S/N";
            cliente.CEP = pessoa.Endereco?.CEP;
            cliente.Telefone1 = !string.IsNullOrWhiteSpace(telefone1) && telefone1 != "00" ? telefone1 : string.Empty;
            cliente.Telefone2 = !string.IsNullOrWhiteSpace(telefone2) && telefone2 != "00" ? telefone2 : string.Empty;

            string tipoCPFCNPJ = "J";
            if (Utilidades.Validate.ValidarCNPJ(cliente.CPFCNPJ) && !Utilidades.Validate.ValidarCPF(cliente.CPFCNPJ.Length > 11 ? cliente.CPFCNPJ.Substring(cliente.CPFCNPJ.Length - 11, 11) : cliente.CPFCNPJ))
                tipoCPFCNPJ = "J";
            else if (Utilidades.Validate.ValidarCPF(cliente.CPFCNPJ.Length > 11 ? cliente.CPFCNPJ.Substring(cliente.CPFCNPJ.Length - 11, 11) : cliente.CPFCNPJ))
            {
                tipoCPFCNPJ = "F";
                cliente.CPFCNPJ = cliente.CPFCNPJ.Length > 11 ? cliente.CPFCNPJ.Substring(cliente.CPFCNPJ.Length - 11, 11) : cliente.CPFCNPJ;
                cliente.RGIE = "ISENTO";
            }
            else
                tipoCPFCNPJ = cliente.CPFCNPJ.Length == 14 ? "J" : "F";

            cliente.CodigoAtividade = Servicos.Atividade.ObterAtividade(empresa.Codigo, tipoCPFCNPJ, stringConexao, 0, unidadeDeTrabalho).Codigo;
            cliente.Cidade = pessoa.Endereco?.Cidade?.Descricao;

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Dominio.Entidades.Localidade localidade = null;
            if (pessoa.Endereco != null && pessoa.Endereco.Cidade != null && pessoa.Endereco.Cidade.IBGE > 0)
                localidade = repLocalidade.BuscarPorCodigoIBGE(pessoa.Endereco.Cidade.IBGE);
            if (localidade == null && pessoa.Endereco != null && pessoa.Endereco.Cidade != null && !string.IsNullOrWhiteSpace(pessoa.Endereco.Cidade.Descricao) && !string.IsNullOrWhiteSpace(pessoa.Endereco.Cidade.SiglaUF))
                localidade = repLocalidade.BuscarPorDescricaoEUF(pessoa.Endereco.Cidade.Descricao, pessoa.Endereco.Cidade.SiglaUF);
            if (localidade == null && pessoa.Endereco != null && pessoa.Endereco.Cidade != null && !string.IsNullOrWhiteSpace(pessoa.Endereco.CEP) && pessoa.Endereco.CEP.Length > 3)
                localidade = repLocalidade.BuscarPorCEP(Utilidades.String.OnlyNumbers(pessoa.Endereco.CEP.Substring(0, 3)));
            if (localidade == null)
                localidade = empresa.Localidade;

            cliente.CodigoIBGECidade = localidade.CodigoIBGE;

            return cliente;
        }

        private Dominio.ObjetosDeValor.CTe.Documento GerarDocumentoCTe(Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unitOfWork);

            Dominio.ObjetosDeValor.CTe.Documento docNF = new Dominio.ObjetosDeValor.CTe.Documento();
            docNF.Valor = notaFiscal.Valor;
            docNF.Numero = notaFiscal.Numero.ToString();
            docNF.Serie = notaFiscal.Serie;
            docNF.CFOP = "0";
            docNF.ChaveNFE = notaFiscal.Chave;
            docNF.DataEmissao = notaFiscal.DataEmissao;
            docNF.ModeloDocumentoFiscal = notaFiscal.Modelo;
            docNF.Peso = notaFiscal.PesoBruto;
            docNF.Volume = (int)notaFiscal.VolumesTotal;
            docNF.ValorICMS = notaFiscal.ValorICMS;
            docNF.ValorICMSST = notaFiscal.ValorST;
            docNF.ValorProdutos = notaFiscal.ValorTotalProdutos;
            docNF.BaseCalculoICMS = notaFiscal.BaseCalculoICMS;
            docNF.BaseCalculoICMSST = notaFiscal.BaseCalculoST;
            docNF.protocoloNFe = notaFiscal.Protocolo;
            docNF.NumeroReferenciaEDI = notaFiscal.NumeroReferenciaEDI;
            docNF.NumeroControleCliente = notaFiscal.NumeroControleCliente;
            docNF.NumeroPedido = notaFiscal.NumeroPedido;
            docNF.NumeroRomaneio = notaFiscal.NumeroRomaneio;

            if (!string.IsNullOrWhiteSpace(docNF.ChaveNFE) && docNF.ChaveNFE.Length == 44)
            {
                docNF.Tipo = Dominio.Enumeradores.TipoDocumentoCTe.NFe;
                docNF.ModeloDocumentoFiscal = "55";
            }
            else
            {
                docNF.Tipo = Dominio.Enumeradores.TipoDocumentoCTe.Outros;
                docNF.ModeloDocumentoFiscal = "99";
                docNF.Descricao = "NOTA FISCAL";
            }
            return docNF;
        }

        private void EnviarEmailProblemaCTe(string remetente, string conteudo, string stringConexao, Dominio.Entidades.PreConhecimentoDeTransporteEletronico preCte = null, Repositorio.UnitOfWork unitOfWork = null, System.Net.Mail.Attachment xmlAnexo = null)
        {
            unitOfWork = unitOfWork ?? new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();
            if (email != null)
            {
                Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                Servicos.PreCTe serPreCte = new Servicos.PreCTe(unitOfWork);
                List<System.Net.Mail.Attachment> anexos = null;
                if (preCte != null)
                {
                    anexos = new List<System.Net.Mail.Attachment>();
                    string xml = serPreCte.BuscarXMLPreCte(preCte);
                    MemoryStream stream = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(xml));
                    System.Net.Mail.Attachment anexo = new System.Net.Mail.Attachment(stream, string.Concat("pre_cte_" + preCte.Codigo, ".xml"));
                    anexos.Add(anexo);
                }

                conteudo += "<hr/>";

                if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                {
                    conteudo += email.MensagemRodape.Replace("#qLinha#", "<br/>");
                }

                serEmail.EnviarEmail(email.Email, email.Email, email.Senha, remetente, "", "", "Problemas emissão automática CTe", conteudo, email.Smtp, anexos, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp);
            }
        }

        private bool AdicionarCTeNaFilaDeConsulta(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            try
            {
                if (cte.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                    return true;

                string postData = "CodigoCTe=" + cte.Codigo;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(WebConfigurationManager.AppSettings["UriSistemaEmissaoCTe"], "/IntegracaoCTe/AdicionarNaFilaDeConsulta"));

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();

                Stream stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(stream);
                var result = reader.ReadToEnd();

                stream.Dispose();
                reader.Dispose();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var retorno = (System.Collections.Generic.Dictionary<string, object>)serializer.DeserializeObject(result);

                return (bool)retorno["Sucesso"];
            }
            catch (System.Exception ex)
            {
                Servicos.Log.TratarErro(ex, "GerarFTP");
                return false;
            }
        }

        private bool AdicionarNFSeNaFilaDeConsulta(Dominio.Entidades.NFSe nfse)
        {
            try
            {
                string postData = "CodigoNFSe=" + nfse.Codigo;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(WebConfigurationManager.AppSettings["UriSistemaEmissaoCTe"], "/IntegracaoNFSe/AdicionarNaFilaDeConsulta"));

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();

                Stream stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(stream);
                var result = reader.ReadToEnd();

                stream.Dispose();
                reader.Dispose();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var retorno = (System.Collections.Generic.Dictionary<string, object>)serializer.DeserializeObject(result);

                return (bool)retorno["Sucesso"];
            }
            catch (System.Exception ex)
            {
                Servicos.Log.TratarErro(ex, "GerarFTP");
                return false;
            }
        }

        private int ParseInt(string valor)
        {
            int.TryParse(valor, out int valorInt);

            return valorInt;
        }

        private decimal ParseDecimal(string valor)
        {
            decimal.TryParse(valor, out decimal valorDecimal);

            return valorDecimal;
        }

        #endregion
    }
}

