using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.EDI
{
    public class StartupEDIWalmart : ServicoBase
    {
        public StartupEDIWalmart(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        private int tempoThread = 1000;
        private string caminhoArquivos = Servicos.FS.GetPath(@"D:\Arquivos\FTP");
        private string caminhoRaiz = Servicos.FS.GetPath(@"D:\Arquivos");
        private string TipoArmazenamento = "pasta";
        private string EnderecoFTP = "";
        private string UsuarioFTP = "";
        private string SenhaFTP = "";
        private string CaminhoRaizFTP = "";
        private string CaminhoRaizRoteirizacao = "";
        private bool FTPPassivo = true;
        private string PortaFTP = "21";
        private bool UtilizaSFTP = false;
        private string AdminStringConexao = "";
        private string CaminhoBatReiniciar = "";
        private int ExceptionsSeguidas = 0;

        public void Iniciar(string caminhoRaizArquivos, string tipoArmazenamento, string enderecoFTP, string usuarioFTP, string senhaFTP, string caminhoRaizFTP, string caminhoRaizRoteirizacao, bool ftpPassivo, string portaFTP, bool utilizaSFTP, string adminMultisoftware, string caminhoBatReiniciar, int tamanhoStack)
        {
            Thread thread = new Thread(new ThreadStart(ExecutarThread), tamanhoStack);
            thread.CurrentUICulture = new System.Globalization.CultureInfo("pt-BR");
            thread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            thread.IsBackground = true;
            caminhoRaiz = caminhoRaizArquivos;
            caminhoArquivos = caminhoRaizArquivos + @"\FTP";
            TipoArmazenamento = tipoArmazenamento;
            EnderecoFTP = enderecoFTP;
            UsuarioFTP = usuarioFTP;
            CaminhoRaizRoteirizacao = caminhoRaizRoteirizacao;
            SenhaFTP = senhaFTP;
            CaminhoRaizFTP = caminhoRaizFTP;
            FTPPassivo = ftpPassivo;
            CaminhoBatReiniciar = caminhoBatReiniciar;
            PortaFTP = portaFTP;
            UtilizaSFTP = utilizaSFTP;
            AdminStringConexao = adminMultisoftware;

            thread.Start();
        }

        private void ExecutarThread()
        {
            Servicos.Log.TratarErro("Iniciou Task");
            while (true)
            {
                try
                {
                    System.Threading.Thread.Sleep(tempoThread);
                    
                    using (Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(StringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                    {
                        Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho);
                        //IntegrarNotFisPendente(unidadeDeTrabalho);
                        IntegrarINTDNEPendente(unidadeDeTrabalho);
                        BuscarNOTFIS(unidadeDeTrabalho);
                        unidadeDeTrabalho.Dispose();
                    }
                }
                catch (System.ServiceModel.CommunicationException com)
                {
                    Servicos.Log.TratarErro("Comunication: " + com);
                    System.Threading.Thread.Sleep(tempoThread);
                }
                catch (TimeoutException ti)
                {
                    Servicos.Log.TratarErro("Time out: " + ti);
                    System.Threading.Thread.Sleep(tempoThread);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    System.Threading.Thread.Sleep(tempoThread);
                }
            }
        }



        private void BuscarNOTFIS(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);

                string pasta = @"\Enviados\Notfis\";
                //string pastaFTP = "input";
                string caminho = caminhoArquivos + pasta;

                if (TipoArmazenamento == "ftp")
                {
                    string caminhoFTP = CaminhoRaizFTP; //+ pastaFTP.Replace(@"\", "/");
                    string erro = "";
                    Servicos.FTP.DownloadArquivosPasta(EnderecoFTP, PortaFTP, caminhoFTP, UsuarioFTP, SenhaFTP, FTPPassivo, false, caminho, out erro, UtilizaSFTP, false, "", true, false, true);
                    if (!string.IsNullOrWhiteSpace(erro))
                    {
                        Servicos.Log.TratarErro(erro);
                        if (!string.IsNullOrWhiteSpace(CaminhoBatReiniciar))
                        {
                            Servicos.Log.TratarErro("Solicitou bat reinicia");
                            System.Diagnostics.Process.Start(CaminhoBatReiniciar);
                        }
                        return;
                    }

                    caminhoFTP = CaminhoRaizRoteirizacao; //+ pastaFTP.Replace(@"\", "/");
                    Servicos.FTP.DownloadArquivosPasta(EnderecoFTP, PortaFTP, caminhoFTP, UsuarioFTP, SenhaFTP, FTPPassivo, false, caminho, out erro, UtilizaSFTP, false, "", true, false, true);
                    if (!string.IsNullOrWhiteSpace(erro))
                    {
                        Servicos.Log.TratarErro(erro);
                        if (!string.IsNullOrWhiteSpace(CaminhoBatReiniciar))
                        {
                            Servicos.Log.TratarErro("Solicitou bat reinicia");
                            System.Diagnostics.Process.Start(CaminhoBatReiniciar);
                        }
                        return;
                    }
                }

                IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, "*", SearchOption.AllDirectories).AsParallel();
                
                foreach (string arquivo in arquivos)
                {
                    string fileName = Path.GetFileName(arquivo);
                    string fileExtension = Path.GetExtension(arquivo);
                    using System.IO.MemoryStream ms = new MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo));
                    try
                    {
                        string extensao = fileExtension.Length > 5 ? "" : fileExtension;
                        Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI = new Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI();
                        controleIntegracaoCargaEDI.Data = DateTime.Now;
                        controleIntegracaoCargaEDI.MensagemRetorno = "";
                        controleIntegracaoCargaEDI.NumeroDT = "";
                        controleIntegracaoCargaEDI.NomeArquivo = fileName;
                        controleIntegracaoCargaEDI.GuidArquivo = Guid.NewGuid().ToString() + extensao;
                        controleIntegracaoCargaEDI.NumeroTentativas = 0;
                        controleIntegracaoCargaEDI.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.AgIntegracao;
                        repControleIntegracaoCargaEDI.Inserir(controleIntegracaoCargaEDI);
                        MoverParaPastaProcessados(controleIntegracaoCargaEDI.GuidArquivo, arquivo);
                    }
                    catch (Exception ex2)
                    {
                        Servicos.Log.TratarErro("Não foi possível interpretar o arquivo . " + fileName + " de contingencia da Natura");
                        Servicos.Log.TratarErro(ex2);
                    }
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);
                    ms.Close();
                    ms.Dispose();

                }
            }
            finally
            {
                unitOfWork.FlushAndClear();
            }
        }

        private void MoverParaPastaProcessados(string nomeArquivo, string fullName)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(Utilidades.IO.FileStorageService.Storage.ReadAllText(fullName));

            string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRaiz, "Processados", nomeArquivo);

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoCompleto, bytes);
        }

        private void IntegrarINTDNEPendente(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                if (ExceptionsSeguidas > 5)
                {
                    Servicos.Log.TratarErro("Solicitou bat reinicia por exceções seguidas");
                    System.Diagnostics.Process.Start(CaminhoBatReiniciar);
                }

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRaiz, "Processados");

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.PreCargas.PreCarga repPrecarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig repTipoCargaPrioridadeCargaAutoConfig = new Repositorio.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig repTipoCargaModeloVeicularAutoConfig = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = repositorioConfiguracaoPedido.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Carga.Carga serCarga = new Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Carga.Rota serRota = new Servicos.Embarcador.Carga.Rota(unitOfWork);
                Servicos.Embarcador.Hubs.Carga servicoHubCarga = new Servicos.Embarcador.Hubs.Carga();

                List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> controleIntegracoesCargaEDIs = repControleIntegracaoCargaEDI.BuscarPendenteIntegracao(0, 1);

                List<int> codigosControle = (from obj in controleIntegracoesCargaEDIs select obj.Codigo).ToList();


                foreach (int codigoControle in codigosControle)
                {
                    Dominio.ObjetosDeValor.WebService.Carga.DadosIntegracaoEDISumarizados dadosIntegracaoEDISumarizados = null;
                    try
                    {
                        bool integrou = true;
                        bool LayoutPedido = false;
                        bool layoutImportacao = false;
                        Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI = repControleIntegracaoCargaEDI.BuscarPorCodigo(codigoControle);
                        List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> cargasIntegracoes = new List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>();


                        string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, controleIntegracaoCargaEDI.GuidArquivo);
                        System.IO.MemoryStream ms = new MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoCompleto));
                        ms.Position = 0;
                        Dominio.Entidades.LayoutEDI layoutEDI = null;
                        bool layoutLTM = false;
                        bool layoutB2B = controleIntegracaoCargaEDI.NomeArquivo.Contains("B2B");
                        bool layoutB2C = controleIntegracaoCargaEDI.NomeArquivo.Contains("B2C");
                        if (controleIntegracaoCargaEDI.NomeArquivo.Contains("LTM") || controleIntegracaoCargaEDI.NomeArquivo.Contains("SNF"))
                        {
                            layoutLTM = true;
                            layoutEDI = repLayoutEDI.BuscarPorTipoDescricao(Dominio.Enumeradores.TipoLayoutEDI.NOTFIS, "LTM").FirstOrDefault();
                        }
                        else if (controleIntegracaoCargaEDI.NomeArquivo.Contains("SGA"))
                            layoutEDI = repLayoutEDI.BuscarPorTipoDescricao(Dominio.Enumeradores.TipoLayoutEDI.NOTFIS, "SGA").FirstOrDefault();
                        else if (controleIntegracaoCargaEDI.NomeArquivo.Contains("SGD"))
                            layoutEDI = repLayoutEDI.BuscarPorTipoDescricao(Dominio.Enumeradores.TipoLayoutEDI.NOTFIS, "SGD").FirstOrDefault();
                        else if (controleIntegracaoCargaEDI.NomeArquivo.Contains("PEDIDO") || controleIntegracaoCargaEDI.NomeArquivo.Contains("SOLICITACAO"))
                        {
                            layoutEDI = repLayoutEDI.BuscarPorTipoDescricao(Dominio.Enumeradores.TipoLayoutEDI.NOTFIS, "PEDIDO").FirstOrDefault();
                            LayoutPedido = true;
                        }
                        else if (controleIntegracaoCargaEDI.NomeArquivo.Contains("FRDNE_IMP"))
                        {
                            layoutImportacao = true;
                            layoutLTM = true;
                            layoutEDI = repLayoutEDI.BuscarPorTipoDescricao(Dominio.Enumeradores.TipoLayoutEDI.NOTFIS, "FRDNE_IMP").FirstOrDefault();
                        }
                        else
                            layoutEDI = repLayoutEDI.BuscarPorTipoDescricao(Dominio.Enumeradores.TipoLayoutEDI.NOTFIS, "Webinvoice").FirstOrDefault();


                        Servicos.LeituraEDI leituraEDI = new Servicos.LeituraEDI(null, layoutEDI, ms, unitOfWork, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.GetEncoding("iso-8859-1"));
                        Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis = leituraEDI.GerarNotasFis();
                        string retorno = "";

                        List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaParaCancelamento = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                        List<Dominio.Entidades.Embarcador.Cargas.Carga> cargaParaAgrupamento = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                        List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasParaFechamento = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                        List<string> chavesNF = new List<string>();

                        if (LayoutPedido)
                        {
                            AdicionarPedido(controleIntegracaoCargaEDI, notfis, layoutEDI, unitOfWork);
                            continue;
                        }
                        else
                        {
                            if (integrou)
                            {
                                Servicos.Log.TratarErro("Iniciou processsamento arquivo" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");

                                if ((notfis.CabecalhosDocumento == null || notfis.CabecalhosDocumento.Count == 0) && notfis.NotasFiscais != null && notfis.NotasFiscais.Count > 0)
                                {
                                    notfis.CabecalhosDocumento = new List<Dominio.ObjetosDeValor.EDI.Notfis.CabecalhoDocumento>();
                                    Dominio.ObjetosDeValor.EDI.Notfis.CabecalhoDocumento cabecalho = new Dominio.ObjetosDeValor.EDI.Notfis.CabecalhoDocumento();
                                    cabecalho.Embarcador = new Dominio.ObjetosDeValor.EDI.Notfis.Embarcador();
                                    cabecalho.Embarcador.Pessoa = notfis.NotasFiscais.FirstOrDefault().NFe.Emitente;
                                    cabecalho.Veiculo = new Dominio.ObjetosDeValor.Veiculo();
                                    cabecalho.CodigoRemessa = notfis.NotasFiscais.FirstOrDefault().NumeroRomaneio;
                                    cabecalho.NotasFiscais = notfis.NotasFiscais;

                                    notfis.CabecalhosDocumento.Add(cabecalho);
                                }

                                int sequenciaGlobal = 0;

                                List<string> NumeroDTs = (from obj in notfis.CabecalhosDocumento where !string.IsNullOrWhiteSpace(obj.CodigoRemessa) select obj.CodigoRemessa).Distinct().ToList();
                                if (NumeroDTs.Count == 0)
                                    NumeroDTs = (from obj in notfis.CabecalhosDocumento where !string.IsNullOrWhiteSpace(obj.NumeroRomaneio) select obj.NumeroRomaneio).Distinct().ToList();

                                for (int cab = 0; cab < notfis.CabecalhosDocumento.Count; cab++)
                                {
                                    if (!integrou)
                                        break;

                                    Dominio.ObjetosDeValor.EDI.Notfis.CabecalhoDocumento cabecalhoDocumento = notfis.CabecalhosDocumento[cab];
                                    Dominio.ObjetosDeValor.EDI.Notfis.Embarcador embarcador = cabecalhoDocumento.Embarcador;

                                    List<Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal> notasFiscais = null;

                                    if (cabecalhoDocumento.Destinatarios != null && cabecalhoDocumento.Destinatarios.Count > 0)
                                        notasFiscais = cabecalhoDocumento.Destinatarios.SelectMany(obj => obj.NotasFiscais).ToList();
                                    else
                                        notasFiscais = cabecalhoDocumento.NotasFiscais.ToList();


                                    string[] splitArquivo = controleIntegracaoCargaEDI.NomeArquivo.Split('_');

                                    dadosIntegracaoEDISumarizados = new Dominio.ObjetosDeValor.WebService.Carga.DadosIntegracaoEDISumarizados();

                                    if (splitArquivo.Length > 2)
                                        dadosIntegracaoEDISumarizados.IDDOC = splitArquivo[2];

                                    dadosIntegracaoEDISumarizados.MeioTransporte = "";//string.Join(",", (from obj in TiposOperacao select obj).ToList());

                                    long cnpfil = 0;
                                    long.TryParse(embarcador.Pessoa.CPFCNPJ, out cnpfil);
                                    embarcador.Pessoa.CPFCNPJ = cnpfil.ToString("d14");

                                    dadosIntegracaoEDISumarizados.Filial = embarcador.Pessoa.CPFCNPJ;
                                    dadosIntegracaoEDISumarizados.ModeloVeicular = cabecalhoDocumento.Veiculo.CodigoModeloVeicularEmbarcador;
                                    dadosIntegracaoEDISumarizados.NumeroDT = string.Join(",", (from obj in NumeroDTs select obj).ToList());
                                    dadosIntegracaoEDISumarizados.NumerosDT = NumeroDTs;
                                    dadosIntegracaoEDISumarizados.Placa = cabecalhoDocumento.Veiculo.Placa;

                                    List<string> cnpjDestinatarioAgrupado = null;

                                    if (cabecalhoDocumento.Destinatarios != null && cabecalhoDocumento.Destinatarios.Count > 0)
                                        cnpjDestinatarioAgrupado = (from obj in cabecalhoDocumento.Destinatarios select obj.Pessoa.CPFCNPJ).Distinct().ToList();
                                    else
                                    {
                                        if (!layoutImportacao)
                                            cnpjDestinatarioAgrupado = (from obj in notasFiscais select obj.NFe.Destinatario.CPFCNPJ).Distinct().ToList();
                                        else
                                            cnpjDestinatarioAgrupado = (from obj in notasFiscais select obj.NFe.Destinatario.CodigoIntegracao).Distinct().ToList();
                                    }


                                    for (int idxCNPJDest = 0; idxCNPJDest < cnpjDestinatarioAgrupado.Count; idxCNPJDest++)
                                    {
                                        sequenciaGlobal++;

                                        if (!integrou)
                                            break;

                                        List<Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal> notasFiscaisdestinatario = null; // (from obj in notasFiscais where obj.NFe.Destinatario.CPFCNPJ == cnpjDestinatarioAgrupado[idxCNPJDest] select obj).ToList();

                                        Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa destinatario = null; //notasFiscaisdestinatario.Select(obj => obj.NFe.Destinatario).FirstOrDefault();

                                        if (cabecalhoDocumento.Destinatarios != null && cabecalhoDocumento.Destinatarios.Count > 0)
                                        {
                                            List<Dominio.ObjetosDeValor.EDI.Notfis.Destinatario> destinatariosCNPJ = (from obj in cabecalhoDocumento.Destinatarios where obj.Pessoa.CPFCNPJ == cnpjDestinatarioAgrupado[idxCNPJDest] select obj).ToList();
                                            destinatario = destinatariosCNPJ.FirstOrDefault().Pessoa;
                                            notasFiscaisdestinatario = (from obj in destinatariosCNPJ select obj.NotasFiscais).SelectMany(obj => obj).ToList();

                                        }
                                        else
                                        {
                                            if (!layoutImportacao)
                                                notasFiscaisdestinatario = (from obj in notasFiscais where obj.NFe.Destinatario.CPFCNPJ == cnpjDestinatarioAgrupado[idxCNPJDest] select obj).ToList();
                                            else
                                                notasFiscaisdestinatario = (from obj in notasFiscais where obj.NFe.Destinatario.CodigoIntegracao == cnpjDestinatarioAgrupado[idxCNPJDest] select obj).ToList();

                                            destinatario = notasFiscaisdestinatario.Select(obj => obj.NFe.Destinatario).FirstOrDefault();
                                        }

                                        //destinatario.Pessoa.AtualizarEnderecoPessoa = true;
                                        Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();
                                        cargaIntegracao.DataCriacaoCarga = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                        string RaizDestinatario = "";
                                        Dominio.Entidades.Cliente clienteDestinatario = null;

                                        if (!layoutImportacao)
                                        {
                                            double cnpjDestinatario;
                                            bool isDouble = double.TryParse(destinatario.CPFCNPJ, out cnpjDestinatario);


                                            if (isDouble)
                                            {
                                                long cnpjformatadoDest = 0;
                                                long.TryParse(destinatario.CPFCNPJ, out cnpjformatadoDest);
                                                string cnpjFormatado = cnpjformatadoDest.ToString("d14");

                                                destinatario.CPFCNPJ = cnpjFormatado;
                                                clienteDestinatario = repCliente.BuscarPorCPFCNPJ(cnpjDestinatario);
                                            }
                                            RaizDestinatario = destinatario.CPFCNPJ.Remove(8, 6);
                                        }
                                        else
                                        {
                                            clienteDestinatario = repCliente.BuscarPorCodigoIntegracao(destinatario.CodigoIntegracao);
                                            destinatario.ClienteExterior = true;
                                            destinatario.CPFCNPJ = "";
                                            destinatario.Endereco.Cidade.SiglaUF = "EX";
                                        }

                                        string RaizRemetente = embarcador.Pessoa.CPFCNPJ.Remove(8, 6);

                                        bool RaizDiferente = false;

                                        if (clienteDestinatario == null)
                                        {
                                            if (RaizRemetente == RaizDestinatario)
                                            {
                                                integrou = false;
                                                retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.NaoEncontradoCadastroDestinatario, destinatario.CPFCNPJ);
                                                break;
                                            }
                                            else
                                                destinatario.CodigoCategoria = "Cliente";

                                            if (clienteDestinatario == null && !string.IsNullOrEmpty(destinatario.Endereco?.Logradouro))
                                                retorno = ValidarEndereco(destinatario.Endereco, unitOfWork);

                                            if (!string.IsNullOrWhiteSpace(retorno))
                                            {
                                                integrou = false;
                                                break;
                                            }
                                            else
                                                destinatario.CodigoCategoria = "Cliente";
                                        }

                                        if (RaizRemetente != RaizDestinatario)
                                            RaizDiferente = true;

                                        cargaIntegracao.Distancia = cabecalhoDocumento.Distancia;


                                        if (string.IsNullOrWhiteSpace(retorno))
                                        {
                                            cargaIntegracao.Destinatario = destinatario;

                                            cargaIntegracao.Filial = new Dominio.ObjetosDeValor.Embarcador.Filial.Filial() { CodigoIntegracao = embarcador.Pessoa.CPFCNPJ };

                                            cargaIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();

                                            if (!string.IsNullOrWhiteSpace(cabecalhoDocumento.Veiculo.CodigoModeloVeicularEmbarcador))
                                                cargaIntegracao.ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular { CodigoIntegracao = cabecalhoDocumento.Veiculo.CodigoModeloVeicularEmbarcador };

                                            int codEmpresa = 0;
                                            if (!string.IsNullOrWhiteSpace(cabecalhoDocumento.Empresa?.CNPJ ?? ""))
                                            {
                                                long n;
                                                bool isNumeric = long.TryParse(cabecalhoDocumento.Empresa.CNPJ, out n);

                                                if (isNumeric)
                                                {
                                                    string cnpjFormatado = n.ToString("d14");
                                                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpjFormatado);
                                                    cabecalhoDocumento.Empresa.CNPJ = cnpjFormatado;
                                                    if (empresa != null)
                                                    {
                                                        if (empresa.Status == "I")
                                                        {
                                                            integrou = false;
                                                            retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.TransportadorInativo, empresa.Descricao);
                                                            break;
                                                        }
                                                        Servicos.WebService.Empresa.Empresa serEmpresa = new Servicos.WebService.Empresa.Empresa(unitOfWork);
                                                        cargaIntegracao.TransportadoraEmitente = serEmpresa.ConverterObjetoEmpresa(empresa);
                                                        codEmpresa = empresa.Codigo;
                                                        dadosIntegracaoEDISumarizados.Empresa = codEmpresa;
                                                    }
                                                    else
                                                    {
                                                        cargaIntegracao.TransportadoraEmitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa() { CNPJ = cnpjFormatado };
                                                    }
                                                }
                                                else
                                                {
                                                    integrou = false;
                                                    retorno = Localization.Resources.Cargas.ControleGeracaoEDI.NaoInformadoCNPJ;
                                                    break;
                                                }
                                            }
                                            //else
                                            //{
                                            //    integrou = false;
                                            //    retorno = "Não foi informado o CNPJ do transportador no EDI";
                                            //    break;
                                            //}

                                            Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlacaVarrendoFiliais(codEmpresa, cabecalhoDocumento.Veiculo.Placa); //destinatario.NotasFiscais.FirstOrDefault().Placa);
                                            if (veiculo != null && veiculo.ModeloVeicularCarga != null && cargaIntegracao.ModeloVeicular == null)
                                                cargaIntegracao.ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular { CodigoIntegracao = veiculo.ModeloVeicularCarga.CodigoIntegracao };

                                            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;

                                            if (clienteDestinatario?.TipoOperacaoPadrao != null)
                                                tipoOperacao = clienteDestinatario.TipoOperacaoPadrao;
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(cabecalhoDocumento.TipoOperacao))
                                                    tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(cabecalhoDocumento.TipoOperacao);

                                                if (tipoOperacao == null)
                                                    tipoOperacao = layoutEDI.TipoOperacao;

                                                if (veiculo != null)
                                                {
                                                    Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                                                    Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador = repContratoFreteTransportador.BuscarContratosPorVeiculo(DateTime.Now, veiculo.Codigo);
                                                    if (contratoFreteTransportador != null && contratoFreteTransportador.TipoOperacoes.Count > 0)
                                                    {
                                                        tipoOperacao = contratoFreteTransportador.TipoOperacoes.Select(obj => obj.TipoOperacao).FirstOrDefault();
                                                    }
                                                }
                                            }

                                            if (veiculo == null)
                                                cargaIntegracao.CodigoAgrupamento = cabecalhoDocumento.Veiculo.Placa;

                                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete modalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;

                                            if (notasFiscaisdestinatario.FirstOrDefault().NFe.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar || layoutImportacao)
                                                modalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;
                                            else
                                                modalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;

                                            List<string> tiposDeCarga = (from obj in notasFiscaisdestinatario select obj.NFe.TipoCarga?.CodigoIntegracao ?? "").Distinct().ToList();

                                            bool layoutsSGA = false;
                                            if (controleIntegracaoCargaEDI.NomeArquivo.Contains("SGD"))
                                            {
                                                layoutsSGA = true;
                                                tiposDeCarga.Add("0003");
                                            }
                                            else if (controleIntegracaoCargaEDI.NomeArquivo.Contains("SGA"))
                                            {
                                                layoutsSGA = true;
                                                tiposDeCarga.Add("0001");
                                            }


                                            string tipoCargastr = "";
                                            if (tiposDeCarga.Count > 0)
                                            {
                                                Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig tipoCargaModeloVeicularAutoConfig = repTipoCargaModeloVeicularAutoConfig.Buscar();
                                                if (tipoCargaModeloVeicularAutoConfig != null)
                                                {
                                                    List<Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig> tipoCargaPrioridadeCargaAutoConfigs = repTipoCargaPrioridadeCargaAutoConfig.BuscarPorTipoCargaModeloAutoConfig(tipoCargaModeloVeicularAutoConfig.Codigo);
                                                    foreach (Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig tipoCargaPrioridadeCargaAutoConfig in tipoCargaPrioridadeCargaAutoConfigs)
                                                    {
                                                        tipoCargastr = (from obj in tiposDeCarga where obj.Trim() == tipoCargaPrioridadeCargaAutoConfig.TipoDeCarga.CodigoTipoCargaEmbarcador select obj).FirstOrDefault();
                                                        if (!string.IsNullOrWhiteSpace(tipoCargastr))
                                                        {
                                                            break;
                                                        }
                                                    }
                                                }
                                            }

                                            if (!string.IsNullOrWhiteSpace(tipoCargastr))
                                            {
                                                cargaIntegracao.TipoCargaEmbarcador = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador() { CodigoIntegracao = tipoCargastr };
                                            }
                                            else
                                                cargaIntegracao.TipoCargaEmbarcador = notasFiscaisdestinatario.FirstOrDefault().NFe.TipoCarga;

                                            if (tipoOperacao != null && (!layoutsSGA || !RaizDiferente))
                                            {
                                                Servicos.Log.TratarErro("Tipo Operação Código: " + tipoOperacao.CodigoIntegracao);
                                                cargaIntegracao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao() { CodigoIntegracao = tipoOperacao.CodigoIntegracao };
                                            }
                                            else if (layoutsSGA && RaizDiferente)
                                                cargaIntegracao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao() { CodigoIntegracao = "DISTRIBUIDOR" };

                                            if (veiculo != null)
                                            {
                                                cargaIntegracao.Veiculo = new Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo();
                                                cargaIntegracao.Veiculo.Placa = veiculo.Placa;

                                                Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
                                                if (veiculoMotorista != null)
                                                {
                                                    Dominio.ObjetosDeValor.Embarcador.Carga.Motorista motorista = new Dominio.ObjetosDeValor.Embarcador.Carga.Motorista();
                                                    motorista.CPF = veiculoMotorista.CPF;
                                                    motorista.Nome = veiculoMotorista.Nome;
                                                    motorista.Transportador = cargaIntegracao.TransportadoraEmitente;
                                                    cargaIntegracao.Motoristas.Add(motorista);
                                                }
                                            }

                                            cargaIntegracao.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();

                                            cargaIntegracao.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
                                            cargaIntegracao.CTes = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();
                                            cargaIntegracao.NumeroCarga = !string.IsNullOrWhiteSpace(cabecalhoDocumento.CodigoRemessa) ? cabecalhoDocumento.CodigoRemessa : cabecalhoDocumento.NumeroRomaneio;
                                            if (layoutB2C)
                                                cargaIntegracao.NumeroCarga += "B2C";

                                            if (cabecalhoDocumento.ComplementoNotaFiscal != null)
                                            {
                                                if (cabecalhoDocumento.ComplementoNotaFiscal.CGCEntrega1 == "LACRE" && !string.IsNullOrWhiteSpace(cabecalhoDocumento.ComplementoNotaFiscal.SerieEntrega1))
                                                {
                                                    cargaIntegracao.Lacres = new List<string>();
                                                    cargaIntegracao.Lacres.Add(cabecalhoDocumento.ComplementoNotaFiscal.SerieEntrega1);
                                                }
                                            }

                                            //string codigoRota = "";
                                            //string canalEntrega = "";

                                            decimal quantidadePallets = 0;
                                            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa expedidor = null;
                                            Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa recebedor = null;
                                            for (int nf = 0; nf < notasFiscaisdestinatario.Count; nf++)
                                            {
                                                Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal notaFiscal = notasFiscaisdestinatario[nf];

                                                notaFiscal.NFe.Valor = 0;
                                                notaFiscal.NFe.PesoBruto = 0;
                                                notaFiscal.NFe.PesoLiquido = 0;
                                                cargaIntegracao.Adicional1 = notaFiscal.CodigoDocumentoVinculado;

                                                for (int me = 0; me < notaFiscal.Produtos.Count; me++)
                                                {
                                                    Dominio.ObjetosDeValor.Embarcador.NFe.Produtos mercadoria = notaFiscal.Produtos[me];

                                                    Dominio.ObjetosDeValor.Embarcador.NFe.ComplementoProduto complementoProduto = mercadoria.ComplementoProduto;
                                                    if (complementoProduto == null && notaFiscal.ComplementoProdutos != null)
                                                    {
                                                        List<Dominio.ObjetosDeValor.Embarcador.NFe.ComplementoProduto> complementoProdutos = (from obj in notaFiscal.ComplementoProdutos where obj.Codigo == mercadoria.Codigo select obj).ToList();
                                                        if (complementoProdutos.Count > 0)
                                                        {
                                                            complementoProduto = new Dominio.ObjetosDeValor.Embarcador.NFe.ComplementoProduto();
                                                            complementoProduto.Cubagem = complementoProdutos.Sum(obj => obj.Cubagem);
                                                            complementoProduto.PesoBruto = complementoProdutos.Sum(obj => obj.PesoBruto);
                                                            complementoProduto.PesoLiquido = complementoProdutos.Sum(obj => obj.PesoLiquido);
                                                            complementoProduto.Quantidade = complementoProdutos.Sum(obj => obj.Quantidade);
                                                            complementoProduto.Valor = complementoProdutos.Sum(obj => obj.Valor);
                                                            complementoProduto.Volumes = complementoProdutos.Sum(obj => obj.Volumes);
                                                        }
                                                    }


                                                    if (complementoProduto == null)
                                                    {
                                                        retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.FaltandoComplementosProdutosNF, notaFiscal.NFe.Chave);
                                                        integrou = false;
                                                        break;
                                                    }

                                                    Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = CriarProduto(mercadoria, complementoProduto, unitOfWork);
                                                    cargaIntegracao.Produtos.Add(produto);

                                                    if (notaFiscal.ComplementoNotaFiscal != null && notaFiscal.ComplementoNotaFiscal.CGCEntrega1.Trim().ToUpper() == "TIPO DE NOTA"
                                                    && notaFiscal.ComplementoNotaFiscal.SerieEntrega1.Trim().ToUpper() == "EQUIPAMENTOS")
                                                    {
                                                        if (produto.DescricaoProduto.ToUpper().Trim().Contains("PALLET"))
                                                            quantidadePallets += produto.Quantidade;
                                                    }

                                                    notaFiscal.NFe.Valor += complementoProduto.Valor;
                                                    notaFiscal.NFe.PesoBruto += complementoProduto.PesoBruto;
                                                    notaFiscal.NFe.PesoLiquido += complementoProduto.PesoLiquido;
                                                    cargaIntegracao.QuantidadeVolumes += (int)complementoProduto.Volumes;
                                                }

                                                if (!integrou)
                                                    break;

                                                if (notaFiscal.NFe.PesoBruto == 0)
                                                    notaFiscal.NFe.PesoBruto = 1;

                                                if (notaFiscal.NFe.PesoLiquido == 0)
                                                    notaFiscal.NFe.PesoLiquido = 1;

                                                cargaIntegracao.PesoBruto += notaFiscal.NFe.PesoBruto;
                                                cargaIntegracao.PesoLiquido += notaFiscal.NFe.PesoLiquido;
                                                cargaIntegracao.CubagemTotal += notaFiscal.PesoCubagem;
                                                notaFiscal.NFe.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
                                                notaFiscal.NFe.TipoOperacaoNotaFiscal = !layoutImportacao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada;

                                                long cnpDest = 0;
                                                long.TryParse(notaFiscal.NFe.Destinatario.CPFCNPJ, out cnpDest);
                                                notaFiscal.NFe.Destinatario.CPFCNPJ = cnpDest.ToString("d14");

                                                if (layoutImportacao)
                                                {
                                                    notaFiscal.NFe.Destinatario.ClienteExterior = true;
                                                }

                                                long cnpEmit = 0;
                                                long.TryParse(notaFiscal.NFe.Emitente.CPFCNPJ, out cnpEmit);
                                                notaFiscal.NFe.Emitente.CPFCNPJ = cnpEmit.ToString("d14");

                                                if (notaFiscal.NFe.Expedidor != null)
                                                {
                                                    long cnpExp = 0;
                                                    long.TryParse(notaFiscal.NFe.Expedidor.CPFCNPJ, out cnpExp);
                                                    notaFiscal.NFe.Expedidor.CPFCNPJ = cnpExp.ToString("d14");
                                                    expedidor = notaFiscal.NFe.Expedidor;
                                                }

                                                if (notaFiscal.NFe.Recebedor != null)
                                                {
                                                    long cnpRec = 0;
                                                    long.TryParse(notaFiscal.NFe.Recebedor.CPFCNPJ, out cnpRec);
                                                    notaFiscal.NFe.Recebedor.CPFCNPJ = cnpRec.ToString("d14");
                                                    recebedor = notaFiscal.NFe.Recebedor;
                                                }

                                                if (layoutImportacao)
                                                {
                                                    notaFiscal.NFe.Expedidor = null;
                                                    notaFiscal.NFe.Recebedor = null;
                                                    cargaIntegracao.NumeroDI = notaFiscal.NumeroDI;
                                                }

                                                if (!string.IsNullOrWhiteSpace(notaFiscal.NFe.Chave))
                                                {
                                                    string tipoDocumento = notaFiscal.NFe.Chave.Substring(20, 2);

                                                    if (tipoDocumento == "55")
                                                    {
                                                        chavesNF.Add(notaFiscal.NFe.Chave);
                                                        notaFiscal.NFe.Modelo = "55";
                                                    }
                                                    else
                                                    {
                                                        integrou = false;
                                                        retorno = Localization.Resources.Cargas.ControleGeracaoEDI.ChaveInformadaNaoPertenceNF;
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    notaFiscal.NFe.Modelo = "99";
                                                }
                                                cargaIntegracao.NotasFiscais.Add(notaFiscal.NFe);

                                                dadosIntegracaoEDISumarizados.QuantidadeNfs++;
                                            }

                                            if (!integrou)
                                                break;


                                            if (layoutImportacao)
                                            {
                                                cargaIntegracao.Remetente = cargaIntegracao.Destinatario;
                                                cargaIntegracao.Destinatario = embarcador.Pessoa;
                                                if (expedidor != null)
                                                {
                                                    double cnpjexpedidor;
                                                    bool isDouble = double.TryParse(expedidor.CPFCNPJ, out cnpjexpedidor);
                                                    Dominio.Entidades.Cliente clienteExpedidor = null;

                                                    if (cnpjexpedidor > 0)
                                                    {
                                                        if (isDouble)
                                                        {
                                                            long cnpjformatadoExp = 0;
                                                            long.TryParse(expedidor.CPFCNPJ, out cnpjformatadoExp);
                                                            string cnpjFormatado = cnpjformatadoExp.ToString("d14");
                                                            clienteExpedidor = repCliente.BuscarPorCPFCNPJ(cnpjformatadoExp);
                                                        }
                                                        if (clienteExpedidor == null)
                                                            retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.NaoEncontradoCadastroExpedidor, expedidor.CPFCNPJ);

                                                        cargaIntegracao.Expedidor = expedidor;
                                                    }
                                                }

                                                if (recebedor != null)
                                                {
                                                    double cnpjrecebedor;
                                                    bool isDouble = double.TryParse(recebedor.CPFCNPJ, out cnpjrecebedor);
                                                    Dominio.Entidades.Cliente clienteRecebedor = null;

                                                    if (cnpjrecebedor > 0)
                                                    {
                                                        if (isDouble)
                                                        {
                                                            long cnpjformatadoRec = 0;
                                                            long.TryParse(recebedor.CPFCNPJ, out cnpjformatadoRec);
                                                            string cnpjFormatado = cnpjformatadoRec.ToString("d14");
                                                            clienteRecebedor = repCliente.BuscarPorCPFCNPJ(cnpjformatadoRec);
                                                        }
                                                        if (clienteRecebedor == null)
                                                            retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.NaoEncontradoCadastroRecebedor, recebedor.CPFCNPJ);

                                                        if (embarcador.Pessoa?.CPFCNPJ != recebedor.CPFCNPJ)
                                                            cargaIntegracao.Recebedor = recebedor;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                cargaIntegracao.Remetente = embarcador.Pessoa;
                                            }

                                            cargaIntegracao.NumeroPaletesFracionado = quantidadePallets;
                                            if (!layoutLTM)
                                                cargaIntegracao.NumeroPedidoEmbarcador = cargaIntegracao.NumeroCarga + "_" + sequenciaGlobal;
                                            else
                                                cargaIntegracao.NumeroPedidoEmbarcador = cabecalhoDocumento.NumeroRomaneio;

                                            if (modalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar)
                                            {
                                                cargaIntegracao.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                                                cargaIntegracao.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                                            }
                                            else
                                            {
                                                cargaIntegracao.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                                                cargaIntegracao.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                                            }


                                            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasExistentes = repCarga.BuscarTodasCargasPorCodigoEmbarcador(cargaIntegracao.NumeroCarga, cargaIntegracao.Filial.CodigoIntegracao, true);

                                            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaExiste in cargasExistentes)
                                            {
                                                if (layoutB2B)
                                                {
                                                    if (!cargaExiste.CargaFechada && cargaExiste.CargaVinculada == null)
                                                    {
                                                        cargasParaFechamento.Add(cargaExiste);
                                                        continue;
                                                    }
                                                    else if (cargaExiste.CargaVinculada != null)
                                                    {
                                                        retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.CargaJaAgrupada, cargaIntegracao.NumeroCarga, cargaExiste.CargaVinculada?.CodigoCargaEmbarcador ?? "");
                                                        integrou = false;
                                                        break;
                                                    }
                                                }

                                                if (!repCargaPedido.VerificarPorCargaSePendenteDadosRecebedor(cargaExiste.Codigo))
                                                {
                                                    if (cargaExiste.CargaAgrupamento != null)
                                                    {
                                                        retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.CargaAgrupada, cargaIntegracao.NumeroCarga, cargaExiste.CargaAgrupamento.CodigoCargaEmbarcador);
                                                        integrou = false;
                                                        break;
                                                    }

                                                    if (!serCarga.VerificarSeCargaEstaNaLogistica(cargaExiste, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador))
                                                    {
                                                        retorno = String.Format(Localization.Resources.Cargas.ControleGeracaoEDI.CargaNaoPodeMaisSerAtualizadaCancelarMesma, cargaIntegracao.NumeroCarga);

                                                        integrou = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        bool existeNumeracao = true;
                                                        if (layoutLTM)
                                                            existeNumeracao = repCargaPedido.BuscarPorCargaENumerPedido(cargaExiste.Codigo, cargaIntegracao.NumeroPedidoEmbarcador);

                                                        if (existeNumeracao)
                                                        {
                                                            if (!cargaExiste.CalculandoFrete)
                                                            {
                                                                cargaParaCancelamento.Add(cargaExiste);
                                                            }
                                                            else
                                                            {
                                                                retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.CargaNaoPodeSerCanceladaCalculandoFrete, cargaIntegracao.NumeroCarga);
                                                                integrou = false;
                                                                break;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (!cargaExiste.CalculandoFrete)
                                                            {
                                                                cargaParaAgrupamento.Add(cargaExiste);
                                                            }
                                                            else
                                                            {
                                                                retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.CargaNaoPodeSerAlteradaCalculandoFrete, cargaIntegracao.NumeroCarga);
                                                                integrou = false;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.CargaNaoPodeSerCanceladaAguardandoInformacoesExpedidor, cargaIntegracao.NumeroCarga);
                                                    integrou = false;
                                                    break;
                                                }
                                            }
                                            cargasIntegracoes.Add(cargaIntegracao);
                                        }
                                        else
                                        {
                                            integrou = false;
                                            break;
                                        }
                                    }
                                }

                                Servicos.Log.TratarErro("Finalizou Leitura Arquivo" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");
                            }
                            else
                            {
                                break;
                            }
                            if (integrou)
                            {

                                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaExiste in cargaParaCancelamento)
                                {
                                    unitOfWork.Start();

                                    if (!cargaExiste.CalculandoFrete)
                                    {
                                        if (serCarga.VerificarSeCargaEstaNaLogistica(cargaExiste, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador))
                                        {
                                            Servicos.Log.TratarErro("Iniciou Cancelamento" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");

                                            Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                                            {
                                                Carga = cargaExiste,
                                                GerarIntegracoes = false,
                                                MotivoCancelamento = "Viagem atualizada",
                                                TipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador
                                            };

                                            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, configuracaoTMS, unitOfWork);
                                            Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, unitOfWork, StringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                                            if (cargaCancelamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.Cancelada)
                                            {
                                                retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.CargaNaoPodeMaisSerAtualizadaCancelarManualmente, cargaExiste.CodigoCargaEmbarcador);
                                                integrou = false;
                                            }
                                        }
                                        else
                                            retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.SituacaoCargaNaoPermiteAtualizar, cargaExiste.CodigoCargaEmbarcador);
                                    }
                                    else
                                        retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.CargaNaoPodeSerCanceladaCalculandoFrete, cargaExiste.CodigoCargaEmbarcador);

                                    if (string.IsNullOrWhiteSpace(retorno))
                                        unitOfWork.CommitChanges();
                                    else
                                    {
                                        unitOfWork.Rollback();
                                        Servicos.Log.TratarErro("Rollback Cancelamento" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");
                                        integrou = false;
                                        break;
                                    }
                                }

                                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaExiste in cargaParaAgrupamento)
                                {
                                    serRota.DeletarPercursoDestinosCarga(cargaExiste, unitOfWork);
                                    cargaExiste.CargaFechada = false;
                                    cargaExiste.CalculandoFrete = false;
                                    cargaExiste.DataInicioCalculoFrete = null;
                                    cargaExiste.CalcularFreteSemEstornarComplemento = false;
                                    repCarga.Atualizar(cargaExiste);
                                }

                                //if (integrou)
                                //{
                                //    List<string> chavesExistente = repXMLNotaFiscal.BuscarNotasAtivasPorChave(chavesNF);
                                //    if (chavesExistente.Count > 0)
                                //    {
                                //        retorno = "As notas fiscais já estão vinculadas a outras cargas (" + string.Join(", ", chavesExistente.ToList()) + ")";
                                //        integrou = false;
                                //    }
                                //}

                                if (!integrou)
                                    break;

                                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                                if (cargasParaFechamento.Count > 0)
                                    cargas.AddRange(cargasParaFechamento);

                                List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> preCargas = new List<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();

                                for (int ci = 0; ci < cargasIntegracoes.Count; ci++)
                                {
                                    Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = cargasIntegracoes[ci];

                                    Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> protocolo = AdicionarCarga(cargaIntegracao, unitOfWork);
                                    if (protocolo.Status)
                                    {
                                        if (cargaIntegracao.CanalEntrega == null || cargaIntegracao.CanalEntrega.CodigoIntegracao != "PRODUTOR")
                                        {
                                            //Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(protocolo.Objeto.protocoloIntegracaoCarga, protocolo.Objeto.protocoloIntegracaoPedido);
                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorProtocoloCargaEProtocoloPedido(protocolo.Objeto.protocoloIntegracaoCarga, protocolo.Objeto.protocoloIntegracaoPedido);
                                            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = listaCargaPedido != null ? listaCargaPedido.FirstOrDefault() : null;

                                            if (!cargas.Any(obj => obj.Codigo == cargaPedido.Carga.Codigo))
                                                cargas.Add(cargaPedido.Carga);
                                        }
                                        else
                                        {
                                            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(protocolo.Objeto.protocoloIntegracaoPedido);
                                            if (pedido.PreCarga != null)
                                            {
                                                if (!preCargas.Any(obj => obj.Codigo == pedido.PreCarga.Codigo))
                                                    preCargas.Add(pedido.PreCarga);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (protocolo.CodigoMensagem != Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao)
                                        {
                                            integrou = false;
                                            RejeicaoEDI(codigoControle, protocolo.Mensagem, dadosIntegracaoEDISumarizados, unitOfWork);

                                            break;
                                        }
                                        else
                                        {
                                            //Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(protocolo.Objeto.protocoloIntegracaoCarga, protocolo.Objeto.protocoloIntegracaoPedido);
                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorProtocoloCargaEProtocoloPedido(protocolo.Objeto.protocoloIntegracaoCarga, protocolo.Objeto.protocoloIntegracaoPedido);
                                            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = listaCargaPedido != null ? listaCargaPedido.FirstOrDefault() : null;

                                            if (!cargas.Any(obj => obj.Codigo == cargaPedido.Carga.Codigo))
                                                cargas.Add(cargaPedido.Carga);
                                        }
                                    }
                                }
                                if (integrou)
                                {
                                    Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI nControleIntegracaoCargaEDI = repControleIntegracaoCargaEDI.BuscarPorCodigo(codigoControle);
                                    nControleIntegracaoCargaEDI.Cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                                    Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                                    Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Embarcador.Carga.RateioFrete(unitOfWork);
                                    bool fechou = true;

                                    for (int c = 0; c < cargas.Count; c++)
                                    {
                                        Dominio.Entidades.Embarcador.Cargas.Carga cargaInformacao = cargas[c];

                                        unitOfWork.FlushAndClear();
                                        try
                                        {
                                            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaInformacao.Codigo);
                                            if (!carga.CargaFechada)
                                            {
                                                Servicos.Log.TratarErro("Inciou Fechamento" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");
                                                decimal valorICMS = repCargaPedido.BuscarValorICMSParaComponenteCarga(carga.Codigo);
                                                decimal valorICMSIncluso = repCargaPedido.BuscarValorICMSInclusoParaComponenteCarga(carga.Codigo);
                                                decimal valorISS = repCargaPedido.BuscarValorISSParaComponenteCarga(carga.Codigo);
                                                decimal valorPISCONFIS = repCargaPedido.BuscarValorPISCONFINSParaComponenteCarga(carga.Codigo);
                                                unitOfWork.Start();
                                                carga.CargaFechada = true;
                                                Servicos.Log.TratarErro("4 - Fechou Carga (" + carga.CodigoCargaEmbarcador + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");


                                                //if (carga.TipoOperacao == null)
                                                //    carga.TipoOperacao = tipoOperacao;


                                                bool EmissaoFilialEmissora = false;
                                                if (carga.Filial != null && carga.TipoOperacao != null)
                                                {
                                                    if (carga.Filial.EmpresaEmissora != null && carga.TipoOperacao.EmiteCTeFilialEmissora)
                                                    {
                                                        EmissaoFilialEmissora = true;
                                                        if (repCargaPedido.VerificarSeOperacaoTeraEmissaoFilialEmissoraPorCarga(carga.Codigo))
                                                        {
                                                            carga.EmpresaFilialEmissora = carga.Filial.EmpresaEmissora;
                                                            carga.EmiteMDFeFilialEmissora = carga.Filial.EmiteMDFeFilialEmissora;

                                                            if (carga.Filial.UtilizarCtesAnterioresComoCteFilialEmissora)
                                                                carga.UtilizarCTesAnterioresComoCTeFilialEmissora = true;
                                                        }
                                                    }
                                                }

                                                serCarga.SetarTipoContratacaoCarga(carga, unitOfWork);

                                                serRateioFrete.GerarComponenteICMS(carga, valorICMS, false, unitOfWork, valorICMSIncluso);

                                                if (EmissaoFilialEmissora)
                                                    serRateioFrete.GerarComponenteICMS(carga, valorICMS, true, unitOfWork, valorICMSIncluso);

                                                serRateioFrete.GerarComponentePisCofins(carga, valorPISCONFIS, unitOfWork);
                                                serRateioFrete.GerarComponenteISS(carga, valorISS, unitOfWork);

                                                carga.DataEnvioUltimaNFe = DateTime.Now;
                                                carga.DataInicioEmissaoDocumentos = DateTime.Now;
                                                carga.PossuiPendencia = false;
                                                carga.MotivoPendencia = "";

                                                if (carga.Filial?.EmitirMDFeManualmente ?? false)
                                                    carga.NaoGerarMDFe = true;

                                                if (carga.Filial != null && carga.TipoDeCarga == null)
                                                    carga.TipoDeCarga = carga.Filial.TipoDeCarga;

                                                repCarga.Atualizar(carga);

                                                carga.ExigeNotaFiscalParaCalcularFrete = true;

                                                if (!carga.NaoExigeVeiculoParaEmissao)
                                                    AdicionarCargaJanelaCarregamento(carga, unitOfWork);

                                                if (carga.TipoDeCarga != null && carga.ModeloVeicularCarga != null)
                                                {
                                                    if (!carga.ExigeNotaFiscalParaCalcularFrete)
                                                    {
                                                        carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete; //Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;
                                                        carga.CalculandoFrete = true;
                                                        carga.DataInicioCalculoFrete = DateTime.Now;
                                                    }
                                                    else
                                                    {
                                                        Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unitOfWork);
                                                        List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> cargaMotoristas = repCargaMotorista.BuscarPorCarga(carga.Codigo);
                                                        if (cargaMotoristas.Count > 0 && carga.Empresa != null && carga.Veiculo != null && carga.TipoOperacao != null)
                                                        {
                                                            if (!carga.TipoOperacao.ExigePlacaTracao || (carga.VeiculosVinculados.Count >= carga.ModeloVeicularCarga.NumeroReboques))
                                                            {
                                                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.CalculoFrete; //Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgTransportador;
                                                                carga.CalculandoFrete = true;
                                                                carga.DataInicioCalculoFrete = DateTime.Now;
                                                            }
                                                            else
                                                            {
                                                                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;
                                                        }
                                                    }
                                                }
                                                else
                                                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;

                                                if (carga.Rota?.FinalizarViagemAutomaticamente ?? false)
                                                {
                                                    carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada;
                                                    carga = serCarga.AtualizarStatusCustoExtra(carga, servicoHubCarga, repCarga);
                                                    Servicos.Auditoria.Auditoria.Auditar(new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado { OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema, TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema }, carga, $"Alterou carga para situação {Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada.ObterDescricao()}", unitOfWork);
                                                }

                                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                                                Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga dynRota = ObterDadosRotas(carga, unitOfWork, configuracaoPedido);
                                                Servicos.Embarcador.Carga.RotaFrete.SetarRotaFreteCarga(carga, cargaPedidos, configuracaoTMS, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                                                serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracaoTMS, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);

                                                nControleIntegracaoCargaEDI.Cargas.Add(carga);
                                                nControleIntegracaoCargaEDI.NumeroDT += carga.CodigoCargaEmbarcador;
                                                if (cargas.Count > 1 && cargas.LastOrDefault().Codigo != carga.Codigo)
                                                    nControleIntegracaoCargaEDI.NumeroDT += ", ";
                                                unitOfWork.CommitChanges();

                                                Servicos.Log.TratarErro("Finalizou Fechamento" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            unitOfWork.Rollback();
                                            Servicos.Log.TratarErro(ex);
                                            if (!ex.ToString().Contains("deadlocked on lock") && !ex.ToString().Contains("current state is closed") && !ex.ToString().Contains("name is no longer available"))
                                            {
                                                fechou = false;
                                                break;
                                            }
                                            else
                                            {
                                                Servicos.Log.TratarErro("pegou deadlocked");
                                                if (!string.IsNullOrWhiteSpace(CaminhoBatReiniciar))
                                                {
                                                    Servicos.Log.TratarErro("Solicitou bat reinicia");
                                                    System.Diagnostics.Process.Start(CaminhoBatReiniciar);
                                                }
                                                return;
                                            }
                                        }
                                    }
                                    for (int p = 0; p < preCargas.Count; p++)
                                    {
                                        Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPrecarga.BuscarPorCodigo(preCargas[p].Codigo);
                                        preCarga.CalculandoFrete = true;
                                        repPrecarga.Atualizar(preCarga);
                                    }
                                    if (fechou)
                                    {
                                        nControleIntegracaoCargaEDI.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Integrado;
                                        nControleIntegracaoCargaEDI.NumeroTentativas++;
                                        nControleIntegracaoCargaEDI.MensagemRetorno = "";
                                        SetarDadosContidosEDI(ref nControleIntegracaoCargaEDI, dadosIntegracaoEDISumarizados, cargas, unitOfWork);

                                        repControleIntegracaoCargaEDI.Atualizar(nControleIntegracaoCargaEDI);
                                    }
                                    else
                                    {
                                        RejeicaoEDI(codigoControle, "Não foi possível finalizar a viagem do EDI", dadosIntegracaoEDISumarizados, unitOfWork);
                                    }
                                }
                            }
                            else
                            {
                                RejeicaoEDI(codigoControle, retorno, dadosIntegracaoEDISumarizados, unitOfWork);
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        unitOfWork.Clear();
                        RejeicaoEDI(codigoControle, "O arquivo está inconsistente, por favor verifique.", dadosIntegracaoEDISumarizados, unitOfWork);
                        Servicos.Log.TratarErro(ex);
                    }
                }

                ExceptionsSeguidas = 0;
            }
            catch (Exception ex2)
            {
                ExceptionsSeguidas++;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex2);
            }
            finally
            {
                unitOfWork.FlushAndClear();
            }

        }

        private void AdicionarPedido(Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI, Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis, Dominio.Entidades.LayoutEDI layoutEDI, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.ObjetosDeValor.WebService.Carga.DadosIntegracaoEDISumarizados dadosIntegracaoEDISumarizados = new Dominio.ObjetosDeValor.WebService.Carga.DadosIntegracaoEDISumarizados();

            Dominio.ObjetosDeValor.EDI.Notfis.Embarcador embarcador = notfis.CabecalhoDocumento.Embarcador;

            dadosIntegracaoEDISumarizados.IDDOC = "";

            dadosIntegracaoEDISumarizados.MeioTransporte = "";//string.Join(",", (from obj in TiposOperacao select obj).ToList());

            long cnpfil = 0;
            long.TryParse(Utilidades.String.OnlyNumbers(embarcador.Pessoa.CPFCNPJ), out cnpfil);
            embarcador.Pessoa.CPFCNPJ = cnpfil.ToString("d14");
            dadosIntegracaoEDISumarizados.Filial = embarcador.Pessoa.CPFCNPJ;

            dadosIntegracaoEDISumarizados.ModeloVeicular = "";

            dadosIntegracaoEDISumarizados.NumerosDT = (from obj in notfis.CabecalhoDocumento.Destinatarios select obj.NotasFiscais.FirstOrDefault().NumeroPedido).Distinct().ToList();

            dadosIntegracaoEDISumarizados.NumeroDT = string.Join(",", (from obj in dadosIntegracaoEDISumarizados.NumerosDT select obj).ToList()); //notfis.CabecalhoDocumento.Destinatarios.FirstOrDefault().NotaFiscal.NumeroPedido;
            dadosIntegracaoEDISumarizados.Placa = "";
            string retorno = "";
            bool integrou = true;
            List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> cargasIntegracoes = new List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>();


            foreach (Dominio.ObjetosDeValor.EDI.Notfis.Destinatario pedido in notfis.CabecalhoDocumento.Destinatarios)
            {
                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa destinatario = pedido.Pessoa;

                Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();
                cargaIntegracao.DataCriacaoCarga = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                cargaIntegracao.DataInicioCarregamento = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                double cnpjDestinatario;
                bool isDouble = double.TryParse(Utilidades.String.OnlyNumbers(destinatario.CPFCNPJ), out cnpjDestinatario);
                Dominio.Entidades.Cliente clienteDestinatario = null;

                if (isDouble)
                {
                    long cnpjformatadoDest = 0;
                    long.TryParse(Utilidades.String.OnlyNumbers(destinatario.CPFCNPJ), out cnpjformatadoDest);
                    string cnpjFormatado = cnpjformatadoDest.ToString("d14");

                    destinatario.CPFCNPJ = cnpjFormatado;
                    clienteDestinatario = repCliente.BuscarPorCPFCNPJ(cnpjDestinatario);
                }

                string RaizRemetente = embarcador.Pessoa.CPFCNPJ.Remove(8, 6);
                string RaizDestinatario = destinatario.CPFCNPJ.Remove(8, 6);

                if (clienteDestinatario == null)
                {
                    if (RaizRemetente == RaizDestinatario)
                    {
                        integrou = false;
                        retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.NaoEncontradoCadastroDestinatario, destinatario.CPFCNPJ);
                        break;
                    }
                    else
                        destinatario.CodigoCategoria = "Cliente";

                    if (clienteDestinatario == null && !string.IsNullOrEmpty(destinatario.Endereco?.Logradouro))
                        retorno = ValidarEndereco(destinatario.Endereco, unitOfWork);

                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        integrou = false;
                        break;
                    }
                    else
                        destinatario.CodigoCategoria = "Cliente";
                }

                bool RaizDiferente = false;
                if (RaizRemetente != RaizDestinatario)
                    RaizDiferente = true;

                if (string.IsNullOrWhiteSpace(retorno))
                {
                    cargaIntegracao.Destinatario = destinatario;
                    cargaIntegracao.Filial = new Dominio.ObjetosDeValor.Embarcador.Filial.Filial() { CodigoIntegracao = embarcador.Pessoa.CPFCNPJ };

                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = layoutEDI.TipoOperacao;
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete modalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;

                    if (tipoOperacao != null && !RaizDiferente)
                        cargaIntegracao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao() { CodigoIntegracao = tipoOperacao.CodigoIntegracao };
                    else
                        cargaIntegracao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao() { CodigoIntegracao = "DISTRIBUIDOR" };


                    cargaIntegracao.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();

                    cargaIntegracao.NumeroPedidoEmbarcador = pedido.NotaFiscal.NumeroPedido;

                    foreach (Dominio.ObjetosDeValor.Embarcador.NFe.Produtos mercadoria in pedido.Produtos)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = CriarProduto(mercadoria, mercadoria.ComplementoProduto, unitOfWork);
                        cargaIntegracao.Produtos.Add(produto);
                    }

                    decimal quantidadePallets = 0;


                    cargaIntegracao.PesoBruto = pedido.NotaFiscal.NFe.PesoBruto;
                    cargaIntegracao.PesoLiquido = pedido.NotaFiscal.NFe.PesoLiquido;
                    cargaIntegracao.CubagemTotal = pedido.NotaFiscal.NFe.Cubagem;

                    dadosIntegracaoEDISumarizados.QuantidadeNfs++;

                    if (!integrou)
                        break;

                    cargaIntegracao.Remetente = embarcador.Pessoa;
                    cargaIntegracao.NumeroPaletesFracionado = quantidadePallets;
                    if (modalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar)
                    {
                        cargaIntegracao.TipoPagamento = Dominio.Enumeradores.TipoPagamento.A_Pagar;
                        cargaIntegracao.TipoTomador = Dominio.Enumeradores.TipoTomador.Destinatario;
                    }
                    else
                    {
                        cargaIntegracao.TipoPagamento = Dominio.Enumeradores.TipoPagamento.Pago;
                        cargaIntegracao.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
                    }
                }

                cargasIntegracoes.Add(cargaIntegracao);
            }

            if (integrou)
            {

                Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
                for (int ci = 0; ci < cargasIntegracoes.Count; ci++)
                {
                    Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = cargasIntegracoes[ci];

                    Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> protocolo = AdicionarCarga(cargaIntegracao, unitOfWork);

                    if (!protocolo.Status)
                    {
                        RejeicaoEDI(controleIntegracaoCargaEDI.Codigo, protocolo.Mensagem, dadosIntegracaoEDISumarizados, unitOfWork);
                        integrou = false;
                        break;
                    }
                }
                if (integrou)
                {
                    controleIntegracaoCargaEDI = repControleIntegracaoCargaEDI.BuscarPorCodigo(controleIntegracaoCargaEDI.Codigo);
                    controleIntegracaoCargaEDI.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Integrado;
                    controleIntegracaoCargaEDI.NumeroTentativas++;
                    controleIntegracaoCargaEDI.MensagemRetorno = "";

                    SetarDadosContidosEDI(ref controleIntegracaoCargaEDI, dadosIntegracaoEDISumarizados, null, unitOfWork);

                    repControleIntegracaoCargaEDI.Atualizar(controleIntegracaoCargaEDI);
                }
            }
            else
                RejeicaoEDI(controleIntegracaoCargaEDI.Codigo, retorno, dadosIntegracaoEDISumarizados, unitOfWork);

        }

        private void AdicionarCargaJanelaCarregamento(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (carga.TipoDeCarga == null)
                return;

            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unidadeDeTrabalho);

            if (repositorioCentroCarregamento.ContarPorTipoCargaEFilial(carga.TipoDeCarga.Codigo, carga.Filial.Codigo) == 0)
                return;

            Servicos.Embarcador.Logistica.CargaJanelaCarregamento servicoCargaJanelaCarregamento = new Servicos.Embarcador.Logistica.CargaJanelaCarregamento(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(carga.Codigo);

            if (cargaJanelaCarregamento == null)
                cargaJanelaCarregamento = servicoCargaJanelaCarregamento.AdicionarPorCarga(carga.Codigo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            else
                servicoCargaJanelaCarregamento.AtualizarPorCarga(cargaJanelaCarregamento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga ObterDadosRotas(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido)
        {
            Servicos.Embarcador.Carga.Rota serCargaRota = new Servicos.Embarcador.Carga.Rota(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPercurso repCargaPercurso = new Repositorio.Embarcador.Cargas.CargaPercurso(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga dadosRota = null;

            if (repCargaPercurso.ContarPorCarga(carga.Codigo) <= 0)
            {
                dadosRota = serCargaRota.CriarRota(carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, unitOfWork, configuracaoPedido);
            }
            else
            {
                dadosRota = new Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga();
                dadosRota.situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRetornoRotas.Valida;
            }

            return dadosRota;
        }

        public void RejeicaoEDI(int codigo, string mensagem, Dominio.ObjetosDeValor.WebService.Carga.DadosIntegracaoEDISumarizados dadosIntegracaoEDISumarizados, Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.Start();
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI rControleIntegracaoCargaEDI = repControleIntegracaoCargaEDI.BuscarPorCodigo(codigo);
            rControleIntegracaoCargaEDI.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Falha;
            rControleIntegracaoCargaEDI.MensagemRetorno = mensagem;
            rControleIntegracaoCargaEDI.NumeroTentativas++;
            SetarDadosContidosEDI(ref rControleIntegracaoCargaEDI, dadosIntegracaoEDISumarizados, null, unitOfWork);
            rControleIntegracaoCargaEDI.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEDI.NaoImportada;
            repControleIntegracaoCargaEDI.Atualizar(rControleIntegracaoCargaEDI);
            unitOfWork.CommitChanges();
        }

        private void SetarDadosContidosEDI(ref Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI, Dominio.ObjetosDeValor.WebService.Carga.DadosIntegracaoEDISumarizados dadosIntegracaoEDISumarizados, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, Repositorio.UnitOfWork unitOfWork)
        {
            if (dadosIntegracaoEDISumarizados != null)
            {
                Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao repControleIntegracaoCargaEDIAlteracao = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao controleAnterior = repControleIntegracaoCargaEDIAlteracao.BuscarUltimaPorCargaEFilial(dadosIntegracaoEDISumarizados.NumeroDT, dadosIntegracaoEDISumarizados.Filial);
                controleIntegracaoCargaEDI.NumeroDT = dadosIntegracaoEDISumarizados.NumeroDT;
                controleIntegracaoCargaEDI.NumerosDTs = dadosIntegracaoEDISumarizados.NumerosDT;
                controleIntegracaoCargaEDI.IDOC = dadosIntegracaoEDISumarizados.IDDOC;
                controleIntegracaoCargaEDI.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEDI.AguardandoEmissao;
                controleIntegracaoCargaEDI.DataAtualizacaoSituacaoCarga = DateTime.Now;
                controleIntegracaoCargaEDI.Placa = dadosIntegracaoEDISumarizados.Placa;
                controleIntegracaoCargaEDI.Transportador = dadosIntegracaoEDISumarizados.Empresa > 0 ? new Dominio.Entidades.Empresa() { Codigo = dadosIntegracaoEDISumarizados.Empresa } : null;

                if (cargas != null && cargas.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                        GerarDadosSumarizados(controleIntegracaoCargaEDI, controleAnterior, dadosIntegracaoEDISumarizados, carga, unitOfWork);
                }
                else
                    GerarDadosSumarizados(controleIntegracaoCargaEDI, controleAnterior, dadosIntegracaoEDISumarizados, null, unitOfWork);
            }
        }

        private void GerarDadosSumarizados(Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI, Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao controleAnterior, Dominio.ObjetosDeValor.WebService.Carga.DadosIntegracaoEDISumarizados dadosIntegracaoEDISumarizados, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao repControleIntegracaoCargaEDIAlteracao = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao(unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao controleIntegracaoCargaEDIAlteracao = new Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDIAlteracao();
            controleIntegracaoCargaEDIAlteracao.Carga = carga;
            controleIntegracaoCargaEDIAlteracao.ControleIntegracaoCargaEDI = controleIntegracaoCargaEDI;
            controleIntegracaoCargaEDIAlteracao.MeioTransporteAnterior = controleAnterior?.MeioTransporteAtual ?? "";
            controleIntegracaoCargaEDIAlteracao.MeioTransporteAtual = dadosIntegracaoEDISumarizados.MeioTransporte;
            controleIntegracaoCargaEDIAlteracao.ModeloVeicularAnterior = controleAnterior?.ModeloVeicularAtual ?? "";
            controleIntegracaoCargaEDIAlteracao.ModeloVeicularAtual = dadosIntegracaoEDISumarizados.ModeloVeicular;
            controleIntegracaoCargaEDIAlteracao.PlacaAnterior = controleAnterior?.PlacaAtual ?? "";
            controleIntegracaoCargaEDIAlteracao.PlacaAtual = dadosIntegracaoEDISumarizados.Placa;
            controleIntegracaoCargaEDIAlteracao.QuantidadeNfsAnterior = controleAnterior?.QuantidadeNfsAtual ?? 0;
            controleIntegracaoCargaEDIAlteracao.QuantidadeNfsAtual = dadosIntegracaoEDISumarizados.QuantidadeNfs;
            controleIntegracaoCargaEDIAlteracao.RoteiroAnterior = controleAnterior?.RoteiroAtual ?? "";
            controleIntegracaoCargaEDIAlteracao.RoteiroAtual = dadosIntegracaoEDISumarizados.Roteiro;
            repControleIntegracaoCargaEDIAlteracao.Inserir(controleIntegracaoCargaEDIAlteracao);
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> AdicionarCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();
            Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>();

            StringBuilder stMensagem = new StringBuilder();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);
            Servicos.WebService.Carga.Carga serCargaWS = new Servicos.WebService.Carga.Carga(unitOfWork);
            Servicos.WebService.Carga.ProdutosPedido serProdutoPedidoWS = new Servicos.WebService.Carga.ProdutosPedido(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Embarcador.Carga.RateioFrete(unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(cargaIntegracao.Filial?.CodigoIntegracao ?? "");
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = (cargaIntegracao.TipoOperacao != null) ? repTipoOperacao.BuscarPorCodigoIntegracao(cargaIntegracao.TipoOperacao.CodigoIntegracao) : null;
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                unitOfWork.Start();
                int codigoCargaExistente = 0;
                int protocoloPedidoExistente = 0;

                Servicos.Log.TratarErro("Iniciou Criar Pedido" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = serPedidoWS.CriarPedido(cargaIntegracao, filial, tipoOperacao, ref stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, ref protocoloPedidoExistente, ref codigoCargaExistente, false);
                Servicos.Log.TratarErro("Finalizou Criar Pedido" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");
                int cargaCodigo = 0;
                if (stMensagem.Length == 0 || protocoloPedidoExistente > 0)
                {
                    if (protocoloPedidoExistente == 0)
                        serProdutoPedidoWS.AdicionarProdutosPedido(pedido, configuracao, cargaIntegracao, ref stMensagem, unitOfWork);

                    if (!string.IsNullOrWhiteSpace(cargaIntegracao.NumeroCarga))
                    {
                        Servicos.Log.TratarErro("Iniciou Gerar Carga" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");
                        Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = serCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref stMensagem, ref codigoCargaExistente, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, false, false, null, configuracao, null, "", filial, tipoOperacao);
                        Servicos.Log.TratarErro("Finalizou Gerar Carga" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");

                        int codCarga = cargaPedido != null ? cargaPedido.Carga.Codigo : 0;

                        if (cargaPedido != null)
                        {
                            serRateioFrete.GerarComponenteICMS(cargaPedido, false, unitOfWork);
                            if (cargaPedido.CargaPedidoFilialEmissora)
                                serRateioFrete.GerarComponenteICMS(cargaPedido, true, unitOfWork);

                            serRateioFrete.GerarComponenteISS(cargaPedido, false, unitOfWork);
                            serCargaWS.AdicionarProdutosCarga(cargaPedido, cargaIntegracao, ref stMensagem, unitOfWork, false);
                            cargaCodigo = cargaPedido.Carga.Codigo;
                        }
                    }
                }

                if (stMensagem.Length > 0)
                {
                    unitOfWork.Rollback();
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = stMensagem.ToString();
                    retorno.Objeto = null;
                    if (codigoCargaExistente > 0 && protocoloPedidoExistente > 0)
                    {
                        retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = codigoCargaExistente, protocoloIntegracaoPedido = protocoloPedidoExistente };
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                    }
                }
                else
                {
                    unitOfWork.CommitChanges();
                    retorno.Status = true;
                    retorno.Mensagem = "";
                    retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Carga.Protocolos() { protocoloIntegracaoCarga = cargaCodigo, protocoloIntegracaoPedido = pedido.Codigo };
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                Servicos.Log.TratarErro("Carga: " + cargaIntegracao.NumeroCarga + " Retornou exceção a seguir:");
                retorno.Mensagem = "Ocorreu uma falha ao obter os dados das integrações. " + stMensagem.ToString();
                retorno.Objeto = null;
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            return retorno;
        }


        private Dominio.ObjetosDeValor.Embarcador.Pedido.Produto CriarProduto(Dominio.ObjetosDeValor.Embarcador.NFe.Produtos produtoNF, Dominio.ObjetosDeValor.Embarcador.NFe.ComplementoProduto complementoProduto, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = null;
            if (produtoNF != null && !string.IsNullOrWhiteSpace(produtoNF.Descricao))
            {
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repProdutoEmbarcador.buscarPorCodigoEmbarcador(produtoNF.Codigo);
                produto = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();
                if (produtoEmbarcador == null)
                {
                    if (produtoNF.GrupoProduto == null)
                    {
                        produto.CodigoGrupoProduto = "1";
                        produto.DescricaoGrupoProduto = "Walmart";
                    }
                    else
                    {
                        produto.CodigoGrupoProduto = produtoNF.GrupoProduto.Descricao;
                        produto.DescricaoGrupoProduto = produtoNF.GrupoProduto.Descricao;
                    }

                }
                else
                {
                    produto.CodigoGrupoProduto = produtoEmbarcador.GrupoProduto != null ? produtoEmbarcador.GrupoProduto.CodigoGrupoProdutoEmbarcador : "1";
                    produto.DescricaoGrupoProduto = produtoEmbarcador.GrupoProduto != null ? produtoEmbarcador.GrupoProduto.Descricao : "Walmart";
                }

                produto.CodigoProduto = produtoNF.Codigo;
                produto.DescricaoProduto = produtoNF.Descricao;
                produto.Quantidade = complementoProduto.Quantidade;
                produto.PesoTotalEmbalagem = complementoProduto.PesoBruto - complementoProduto.PesoLiquido;
                if (produto.Quantidade > 0)
                {
                    produto.ValorUnitario = complementoProduto.Valor / produto.Quantidade;
                    produto.PesoUnitario = complementoProduto.PesoLiquido / produto.Quantidade;
                }
                else
                    produto.PesoUnitario = produtoEmbarcador?.PesoUnitario ?? 1;


            }
            return produto;
        }

        private string ValidarEndereco(Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco endereco, Repositorio.UnitOfWork unitOfWork)
        {
            string retorno = "";

            using (AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(AdminStringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoSessaoBancoDados.Nova))
            {
                AdminMultisoftware.Repositorio.Localidades.Endereco repEndereco = new AdminMultisoftware.Repositorio.Localidades.Endereco(adminUnitOfWork);

                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                if (endereco.Cidade.IBGE == 0 || string.IsNullOrWhiteSpace(endereco.Bairro))
                {
                    AdminMultisoftware.Dominio.Entidades.Localidades.Endereco enderecoCEP = endereco.CEP != null ? repEndereco.BuscarCEP(int.Parse(Utilidades.String.OnlyNumbers(endereco.CEP)).ToString()) : null;
                    if (enderecoCEP != null)
                    {
                        if (endereco.Cidade.IBGE == 0)
                            endereco.Cidade.IBGE = int.Parse(enderecoCEP.Localidade.CodigoIBGE);

                        if (endereco.Cidade.IBGE == 0)
                        {
                            if (endereco.Cidade.IBGE == 0)
                            {
                                Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(endereco.Cidade.Descricao), endereco.Cidade.SiglaUF);
                                if (localidade != null)
                                    endereco.Cidade.IBGE = localidade.CodigoIBGE;
                                else
                                    retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.NaoExisteCidadeCadastrada, endereco.Cidade.Descricao, endereco.Cidade.SiglaUF);
                            }
                        }

                        if (string.IsNullOrWhiteSpace(endereco.Bairro))
                            endereco.Bairro = enderecoCEP.Bairro?.Descricao;
                    }
                    else
                    {
                        if (endereco.Cidade.IBGE == 0 || endereco.Cidade.IBGE == 9999999)
                        {
                            Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(endereco.Cidade.Descricao), endereco.Cidade.SiglaUF);
                            if (localidade != null)
                            {
                                if (localidade.CodigoIBGE != 9999999)
                                    endereco.Cidade.IBGE = localidade.CodigoIBGE;
                                else
                                    endereco.Cidade.CodigoIntegracao = localidade.CodigoLocalidadeEmbarcador;
                            }
                            else
                                retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.NaoExisteCidadeCadastrada, endereco.Cidade.Descricao, endereco.Cidade.SiglaUF);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(endereco.Bairro))
                        endereco.Bairro = "INDEFINIDO";

                    if (endereco.Bairro.Length < 3)
                        endereco.Bairro = "Bairro " + endereco.Bairro;
                }

                if (string.IsNullOrWhiteSpace(endereco.Telefone) || endereco.Telefone.Length < 7)
                    endereco.Telefone = "";
            }

            return retorno;
        }

    }
}
