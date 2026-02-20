using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Servicos.EDI
{
    public class StartupEDIMagazineLuiza : ServicoBase
    {
        public StartupEDIMagazineLuiza(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        private int tempoThread = 1000;
        private string caminhoArquivos = Servicos.FS.GetPath(@"D:\Arquivos\FTP");
        private string caminhoRaiz = Servicos.FS.GetPath(@"D:\Arquivos");
        private string TipoArmazenamento = "pasta";
        private string EnderecoFTP = "";
        private string UsuarioFTP = "";
        private string SenhaFTP = "";
        private string CaminhoRaizFTP = "";
        private bool FTPPassivo = true;
        private string PortaFTP = "21";
        private bool UtilizaSFTP = false;
        private string AdminStringConexao = "";
        private string CaminhoBatReiniciar = "";

        public void Iniciar(string caminhoRaizArquivos, string tipoArmazenamento, string enderecoFTP, string usuarioFTP, string senhaFTP, string caminhoRaizFTP, bool ftpPassivo, string portaFTP, bool utilizaSFTP, string adminMultisoftware, string caminhoBatReiniciar, int tamanhoStack)
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
            SenhaFTP = senhaFTP;
            CaminhoRaizFTP = caminhoRaizFTP;
            FTPPassivo = ftpPassivo;
            CaminhoBatReiniciar = caminhoBatReiniciar;
            PortaFTP = portaFTP;
            utilizaSFTP = UtilizaSFTP;
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

                        IntegrarNotfisPendente(unidadeDeTrabalho);
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

        private void MoverParaPastaProcessados(string nomeArquivo, string fullName)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(Utilidades.IO.FileStorageService.Storage.ReadAllText(fullName));

            string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRaiz, "Processados", nomeArquivo);

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoCompleto, bytes);

        }

        private void BuscarNOTFIS(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Log.TratarErro("Inicio - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "BuscarNOTFIS");
                Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);

                string pastaFTP = "input";
                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, "Enviados", "Notfis\\");

                if (TipoArmazenamento == "ftp")
                {
                    string caminhoFTP = CaminhoRaizFTP + pastaFTP.Replace(@"\", "/");
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
                }

                IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, "*.txt", SearchOption.AllDirectories);

                foreach (string arquivo in arquivos)
                {
                    string fileName = Path.GetFileName(arquivo);
                    using System.IO.MemoryStream ms = new MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo));
                    try
                    {
                        Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI = new Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI();
                        controleIntegracaoCargaEDI.Data = DateTime.Now;
                        controleIntegracaoCargaEDI.MensagemRetorno = "";
                        controleIntegracaoCargaEDI.NumeroDT = "";
                        controleIntegracaoCargaEDI.NomeArquivo = fileName;
                        controleIntegracaoCargaEDI.GuidArquivo = Guid.NewGuid().ToString() + Path.GetExtension(arquivo);
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
                Servicos.Log.TratarErro("Fim - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "BuscarNOTFIS");
            }
            finally
            {
                unitOfWork.FlushAndClear();
            }
        }

        private bool IsProcessarSomentePrioritario(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI repositorioConfiguracaoControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ConfiguracaoControleIntegracaoCargaEDI(unitOfWork);
            bool processarSomentePrioritario = repositorioConfiguracaoControleIntegracaoCargaEDI.BuscarConfiguracao()?.ProcessarSomentePrioritario ?? false;

            return processarSomentePrioritario;
        }

        private void IntegrarNotfisPendente(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Log.TratarErro("1 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");
                string caminho = caminhoRaiz + @"\Processados\";

#if DEBUG
                caminho = Servicos.FS.GetPath(@"C:\Temp\");
#endif


                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracao.BuscarConfiguracaoPadrao();
                string codigoIntegracaoTipoOperacao = "IMP";
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(codigoIntegracaoTipoOperacao);

                Servicos.Embarcador.Carga.Carga serCarga = new Embarcador.Carga.Carga(unitOfWork);

                Servicos.Log.TratarErro("2 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");

                List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> controleIntegracoesCargaEDIs;

                Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa transportadorEmitente = null;
                if (!IsProcessarSomentePrioritario(unitOfWork))
                    controleIntegracoesCargaEDIs = repControleIntegracaoCargaEDI.BuscarPendenteIntegracaoSemPrioridade(0, 1);
                else
                    controleIntegracoesCargaEDIs = repControleIntegracaoCargaEDI.BuscarPendenteIntegracaoPrioritario(0, 1);

                //Desc
                //controleIntegracoesCargaEDIs = repControleIntegracaoCargaEDI.BuscarPendenteIntegracaoDesc(0, 1);

                Servicos.Log.TratarErro("3 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");

                List<int> codigosControle = (from obj in controleIntegracoesCargaEDIs select obj.Codigo).ToList();

                Servicos.Log.TratarErro("4 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");

                foreach (int codigoControle in codigosControle)
                {
                    try
                    {
                        Servicos.Log.TratarErro("5 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");

                        bool integrou = true;
                        Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI = repControleIntegracaoCargaEDI.BuscarPorCodigo(codigoControle);

                        string nomeArquivoLog = System.IO.Path.GetFileName(controleIntegracaoCargaEDI.NomeArquivo);

                        Servicos.Log.TratarErro("Inicio leitura EDI" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), nomeArquivoLog);

                        Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPorTipo(Dominio.Enumeradores.TipoLayoutEDI.NOTFIS).FirstOrDefault();

                        if (controleIntegracaoCargaEDI.NomeArquivo.Contains("NOTFIS_")) //EDI NETSHOES
                            layoutEDI = repLayoutEDI.BuscarPorDescricaoETipo("NETSHOES", Dominio.Enumeradores.TipoLayoutEDI.NOTFIS);

                        List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> cargasIntegracoes = new List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>();

                        string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, controleIntegracaoCargaEDI.GuidArquivo);
                        System.IO.MemoryStream ms = new MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoCompleto));
                        ms.Position = 0;
                        Servicos.LeituraEDI leituraEDI = new Servicos.LeituraEDI(null, layoutEDI, ms, unitOfWork, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.GetEncoding("iso-8859-1"));
                        List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> listaNotas = leituraEDI.GerarNotasFiscais();
                        string retorno = "";

                        Servicos.Log.TratarErro("6 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");

                        if (integrou)
                        {
                            Servicos.Log.TratarErro("7 - Quantidade Notas = " + listaNotas.Count.ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");

                            Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(listaNotas.FirstOrDefault().Emitente.CPFCNPJ);
                            if (filial != null && !string.IsNullOrWhiteSpace(filial.CodigoFilialEmbarcador))
                            {
                                if (listaNotas != null && listaNotas.Count >= 0)
                                {
                                    if (!string.IsNullOrWhiteSpace(listaNotas.FirstOrDefault().TipoOperacao))
                                        codigoIntegracaoTipoOperacao = listaNotas.FirstOrDefault().TipoOperacao.Trim();

                                    if (listaNotas.FirstOrDefault().Transportador != null && !string.IsNullOrWhiteSpace(listaNotas.FirstOrDefault().Transportador.CNPJ))
                                        transportadorEmitente = listaNotas.FirstOrDefault().Transportador;
                                }

                                string numeroCarga = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork, filial?.Codigo ?? 0).ToString();
                                for (int i = 0; i < listaNotas.Count; i++)
                                {
                                    Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();
                                    cargaIntegracao.DataCriacaoCarga = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                    cargaIntegracao.NumeroCarga = numeroCarga;

                                    cargaIntegracao.Filial = new Dominio.ObjetosDeValor.Embarcador.Filial.Filial() { CodigoIntegracao = filial.CodigoFilialEmbarcador };

                                    decimal pesoBruto = 0;
                                    decimal pesoLiquido = 0;

                                    double cnpjRemetente = double.Parse(Utilidades.String.OnlyNumbers(listaNotas[i].Emitente.CPFCNPJ));
                                    Dominio.Entidades.Cliente clienteRemetente = repCliente.BuscarPorCPFCNPJ(cnpjRemetente);

                                    double cnpjDestinatario = double.Parse(Utilidades.String.OnlyNumbers(listaNotas[i].Destinatario.CPFCNPJ));
                                    Dominio.Entidades.Cliente clienteDestinatario = repCliente.BuscarPorCPFCNPJ(cnpjDestinatario);

                                    retorno = clienteRemetente == null ? ValidarEndereco(listaNotas[i].Emitente.Endereco, unitOfWork) : "";

                                    if (string.IsNullOrWhiteSpace(retorno))
                                        retorno = clienteDestinatario == null ? ValidarEndereco(listaNotas[i].Destinatario.Endereco, unitOfWork) : "";

                                    if (string.IsNullOrWhiteSpace(retorno))
                                    {
                                        cargaIntegracao.Remetente = listaNotas[i].Emitente;
                                        cargaIntegracao.Destinatario = listaNotas[i].Destinatario;
                                        cargaIntegracao.Destinatario.AtualizarEnderecoPessoa = true;
                                        cargaIntegracao.FecharCargaAutomaticamente = false;

                                        cargaIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();

                                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete modalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;

                                        if (listaNotas[i].ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago)
                                            modalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;
                                        else
                                            modalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;

                                        if (!string.IsNullOrWhiteSpace(listaNotas[i].TipoOperacao))
                                            cargaIntegracao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao() { CodigoIntegracao = listaNotas[i].TipoOperacao.Trim() };
                                        else
                                            cargaIntegracao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao() { CodigoIntegracao = codigoIntegracaoTipoOperacao };

                                        cargaIntegracao.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
                                        cargaIntegracao.CTes = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();

                                        if (listaNotas[i].Transportador != null && !string.IsNullOrWhiteSpace(listaNotas[i].Transportador.CNPJ))
                                            cargaIntegracao.TransportadoraEmitente = listaNotas[i].Transportador;
                                        else if (transportadorEmitente != null)
                                            cargaIntegracao.TransportadoraEmitente = transportadorEmitente;

                                        cargaIntegracao.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();
                                        Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();
                                        produto.CodigoProduto = "DIVERSOS";
                                        produto.DescricaoProduto = "DIVERSOS";
                                        produto.CodigoGrupoProduto = "1";
                                        produto.DescricaoGrupoProduto = "DIVERSOS";
                                        produto.Quantidade = 1;
                                        produto.PesoUnitario = 1;
                                        cargaIntegracao.Produtos.Add(produto);

                                        Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal();
                                        notaFiscal.NFe = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();
                                        notaFiscal.NFe.Numero = listaNotas[i].Numero;
                                        notaFiscal.NFe.Emitente = listaNotas[i].Emitente;
                                        notaFiscal.NFe.Emitente.RazaoSocial = listaNotas[i].Emitente.RazaoSocial;
                                        notaFiscal.NFe.Destinatario = listaNotas[i].Destinatario;
                                        notaFiscal.NFe.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
                                        notaFiscal.NFe.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;
                                        notaFiscal.NFe.PesoBruto = listaNotas[i].PesoBruto;
                                        notaFiscal.NFe.Valor = listaNotas[i].Valor;

                                        pesoBruto += listaNotas[i].PesoBruto;
                                        pesoLiquido += listaNotas[i].PesoLiquido;

                                        if (!string.IsNullOrWhiteSpace(listaNotas[i].Chave))
                                        {
                                            notaFiscal.NFe.Chave = listaNotas[i].Chave;
                                            notaFiscal.NFe.Modelo = "55";
                                            notaFiscal.NFe.NumeroPedido = listaNotas[i].NumeroPedido;
                                        }
                                        else
                                        {
                                            integrou = false;
                                            retorno = Localization.Resources.Cargas.ControleGeracaoEDI.ChaveInformadaNaoPertenceNF;
                                            break;
                                        }

                                        cargaIntegracao.NumeroPedidoEmbarcador = listaNotas[i].Chave;// listaNotas[i].Numero.ToString() + "_" + numeroCarga.ToString();

                                        cargaIntegracao.NotasFiscais.Add(notaFiscal.NFe);

                                        cargaIntegracao.PesoBruto = pesoBruto;
                                        cargaIntegracao.PesoLiquido = pesoLiquido;

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

                                        cargasIntegracoes.Add(cargaIntegracao);

                                        Servicos.Log.TratarErro("NFe " + listaNotas[i].Chave, nomeArquivoLog);
                                    }
                                    else
                                    {
                                        integrou = false;
                                        break;
                                    }

                                    unitOfWork.FlushAndClear();
                                }
                            }
                            else
                            {
                                retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.NaoEncontradaFilial, listaNotas.FirstOrDefault().Emitente.CPFCNPJ);
                                integrou = false;
                            }

                            Servicos.Log.TratarErro("8 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");
                        }

                        Servicos.Log.TratarErro("9 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");

                        if (integrou)
                        {
                            Servicos.Log.TratarErro("Integrando pedidos " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), nomeArquivoLog);

                            //List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                            List<int> cargas = new List<int>();
                            Servicos.Log.TratarErro("10 - Quantidade cargas Integração " + cargasIntegracoes.Count.ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");

                            for (int ci = 0; ci < cargasIntegracoes.Count; ci++)
                            {

                                unitOfWork.FlushAndClear();

                                Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = cargasIntegracoes[ci];

                                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;
                                Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> protocolo = AdicionarCarga(cargaIntegracao, false, unitOfWork, out cargaPedido);
                                //Servicos.Log.TratarErro("10.1 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");
                                if (protocolo.Status && cargaPedido != null)
                                {
                                    Servicos.Log.TratarErro("Carga " + cargasIntegracoes[ci].NumeroCarga + " Pedido " + cargasIntegracoes[ci].NumeroPedidoEmbarcador, nomeArquivoLog);

                                    //List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> listaCargaPedido = repCargaPedido.BuscarPorProtocoloCargaEProtocoloPedido(protocolo.Objeto.protocoloIntegracaoCarga, protocolo.Objeto.protocoloIntegracaoPedido);
                                    //Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = listaCargaPedido != null ? listaCargaPedido.FirstOrDefault() : null;
                                    //Servicos.Log.TratarErro("10.2 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");
                                    //if (!cargas.Any(obj => obj.Codigo == cargaPedido.Carga.Codigo))
                                    //    cargas.Add(cargaPedido.Carga);
                                    if (!cargas.Contains(cargaPedido.Carga.Codigo))
                                        cargas.Add(cargaPedido.Carga.Codigo);
                                }
                                else if (!protocolo.Mensagem.Contains("foi integrado a outro carga") && !protocolo.Mensagem.Contains("foi integrado a outra carga")) //Quando pedido já está em outra carga ignora o mesmo.
                                {
                                    Servicos.Log.TratarErro("Carga " + cargasIntegracoes[ci].NumeroCarga + " Pedido " + cargasIntegracoes[ci].NumeroPedidoEmbarcador + " " + protocolo.Mensagem, nomeArquivoLog);
                                    integrou = false;
                                    RejeicaoEDI(codigoControle, protocolo.Mensagem, unitOfWork);
                                    //Servicos.Log.TratarErro("10.2 Erro - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");
                                    break;
                                }
                                else if (protocolo.Mensagem.Contains("foi integrado a outro carga") || protocolo.Mensagem.Contains("foi integrado a outra carga"))
                                {
                                    Servicos.Log.TratarErro("Carga " + cargasIntegracoes[ci].NumeroCarga + " Pedido " + cargasIntegracoes[ci].NumeroPedidoEmbarcador + " " + protocolo.Mensagem, nomeArquivoLog);
                                }
                            }
                            //Servicos.Log.TratarErro("10.3 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");

                            Servicos.Log.TratarErro("11 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");

                            if (integrou)
                            {
                                Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI nControleIntegracaoCargaEDI = repControleIntegracaoCargaEDI.BuscarPorCodigo(codigoControle);
                                nControleIntegracaoCargaEDI.Cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
                                Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

                                Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Embarcador.Carga.RateioFrete(unitOfWork);
                                bool fechou = true;

                                Servicos.Log.TratarErro("12 - Quantidade Cargas " + cargas.Count.ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");

                                for (int c = 0; c < cargas.Count; c++)
                                {
                                    unitOfWork.FlushAndClear();

                                    Dominio.Entidades.Embarcador.Cargas.Carga cargaInformacao = repCarga.BuscarPorCodigo(cargas[c]);

                                    try
                                    {
                                        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaInformacao.Codigo);
                                        if (!carga.CargaFechada)
                                        {
                                            unitOfWork.Start();

                                            int.TryParse(carga.CodigoCargaEmbarcador, out int codigoCargaEmbarcador);

                                            if (configuracaoTMS.FecharCargaPorThread)
                                                carga.FechandoCarga = true;
                                            else
                                            {
                                                serCarga.FecharCarga(carga, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, null);
                                                carga.CargaFechada = true;
                                                carga.ProcessandoDocumentosFiscais = true;
                                                carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
                                                Servicos.Log.TratarErro("3 - Fechou Carga (" + carga.CodigoCargaEmbarcador + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");
                                            }

                                            carga.NumeroSequenciaCarga = codigoCargaEmbarcador;
                                            if (carga.Filial?.EmitirMDFeManualmente ?? false)
                                                carga.NaoGerarMDFe = true;

                                            repCarga.Atualizar(carga);

                                            Dominio.Entidades.Embarcador.Cargas.CargaIntegracao cargaIntegracao = repCargaIntegracao.BuscarPorCargaETipo(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Magalog);
                                            if (cargaIntegracao != null)
                                                repCargaIntegracao.Deletar(cargaIntegracao);

                                            nControleIntegracaoCargaEDI.Cargas.Add(carga);
                                            nControleIntegracaoCargaEDI.NumeroDT += carga.CodigoCargaEmbarcador;
                                            if (cargas.Count > 1 && cargas.LastOrDefault() != carga.Codigo)
                                                nControleIntegracaoCargaEDI.NumeroDT += ", ";

                                            unitOfWork.CommitChanges();
                                        }
                                        else
                                        {
                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                                            serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracaoTMS, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        unitOfWork.Rollback();
                                        Servicos.Log.TratarErro(ex);
                                        fechou = false;
                                        break;
                                    }
                                }

                                Servicos.Log.TratarErro("13 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");

                                if (fechou)
                                {
                                    nControleIntegracaoCargaEDI.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Integrado;
                                    nControleIntegracaoCargaEDI.NumeroTentativas++;
                                    nControleIntegracaoCargaEDI.MensagemRetorno = "";
                                    repControleIntegracaoCargaEDI.Atualizar(nControleIntegracaoCargaEDI);
                                }
                                else
                                {
                                    RejeicaoEDI(codigoControle, Localization.Resources.Cargas.ControleGeracaoEDI.NaoFoiPossivelFinalizarViagem, unitOfWork);
                                }

                                Servicos.Log.TratarErro("14 - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");
                            }
                            else
                                Servicos.Log.TratarErro("12 Mão gerou - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");
                        }
                        else
                        {
                            Servicos.Log.TratarErro("10 - Rejeiçao imp " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "IntegrarNotfisPendente");
                            RejeicaoEDI(codigoControle, retorno, unitOfWork);
                        }
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        unitOfWork.Clear();
                        RejeicaoEDI(codigoControle, "O arquivo está inconsistente, por favor verifique.", unitOfWork);
                        Servicos.Log.TratarErro(ex);
                    }
                }
            }
            catch (Exception ex2)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex2);
            }
            finally
            {
                unitOfWork.FlushAndClear();
            }

        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> AdicionarCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, bool gerarCargaSegundoTrecho, Repositorio.UnitOfWork unitOfWork, out Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedidoRetorno)
        {
            unitOfWork.FlushAndClear();
            Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos>();

            StringBuilder stMensagem = new StringBuilder();
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Servicos.WebService.Carga.Pedido serPedidoWS = new Servicos.WebService.Carga.Pedido(unitOfWork);
            Servicos.WebService.Carga.Carga serCargaWS = new Servicos.WebService.Carga.Carga(unitOfWork);
            Servicos.WebService.Carga.ProdutosPedido serProdutoPedidoWS = new Servicos.WebService.Carga.ProdutosPedido(unitOfWork);
            Servicos.Embarcador.Carga.RateioFrete serRateioFrete = new Embarcador.Carga.RateioFrete(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null;

            try
            {
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(cargaIntegracao.Filial?.CodigoIntegracao ?? "");
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = (cargaIntegracao.TipoOperacao != null) ? repTipoOperacao.BuscarPorCodigoIntegracao(cargaIntegracao.TipoOperacao.CodigoIntegracao) : null;
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                unitOfWork.Start();
                int codigoCargaExistente = 0;
                int protocoloPedidoExistente = 0;

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = serPedidoWS.CriarPedido(cargaIntegracao, filial, tipoOperacao, ref stMensagem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, ref protocoloPedidoExistente, ref codigoCargaExistente, false);
                int cargaCodigo = 0;
                if (stMensagem.Length == 0 || protocoloPedidoExistente > 0)
                {
                    if (protocoloPedidoExistente == 0)
                        serProdutoPedidoWS.AdicionarProdutosPedido(pedido, configuracao, cargaIntegracao, ref stMensagem, unitOfWork);

                    cargaPedido = serCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref stMensagem, ref codigoCargaExistente, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, true, false, null, configuracao, null, "", filial, tipoOperacao);

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

                if (stMensagem.Length > 0)
                {

                    Servicos.Log.TratarErro("Carga: " + cargaIntegracao.NumeroCarga + " Retornou essa mensagem: " + stMensagem.ToString());
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
            cargaPedidoRetorno = cargaPedido;
            return retorno;
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

        public void RejeicaoEDI(int codigo, string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.Start();
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
            Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI rControleIntegracaoCargaEDI = repControleIntegracaoCargaEDI.BuscarPorCodigo(codigo);
            rControleIntegracaoCargaEDI.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Falha;
            rControleIntegracaoCargaEDI.MensagemRetorno = mensagem;
            rControleIntegracaoCargaEDI.NumeroTentativas++;
            repControleIntegracaoCargaEDI.Atualizar(rControleIntegracaoCargaEDI);
            unitOfWork.CommitChanges();
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
                    AdminMultisoftware.Dominio.Entidades.Localidades.Endereco enderecoCEP = repEndereco.BuscarCEP(int.Parse(Utilidades.String.OnlyNumbers(endereco.CEP)).ToString());
                    if (enderecoCEP != null)
                    {
                        if (endereco.Cidade.IBGE == 0)
                            endereco.Cidade.IBGE = int.Parse(enderecoCEP.Localidade.CodigoIBGE);

                        if (string.IsNullOrWhiteSpace(endereco.Bairro))
                            endereco.Bairro = enderecoCEP.Bairro?.Descricao;
                    }
                    else
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

