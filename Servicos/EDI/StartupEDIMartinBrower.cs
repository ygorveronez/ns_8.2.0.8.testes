using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Servicos.EDI
{
    public class StartupEDIMartinBrower : ServicoBase
    {
        public StartupEDIMartinBrower(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

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
            SenhaFTP = @"uC7VR6&CcQfQink#";// senhaFTP; //WebConfig da erro por causa do &
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
                Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, "Enviados", "Notfis\\");

                if (TipoArmazenamento == "ftp")
                {
                    string caminhoFTP = CaminhoRaizFTP.Replace(@"\", "/");
                    string erro = "";

                    Servicos.FTP.DownloadArquivosPasta(EnderecoFTP, PortaFTP, caminhoFTP, UsuarioFTP, SenhaFTP, FTPPassivo, true, caminho, out erro, UtilizaSFTP, false, "", true, false, true);
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

                IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, "*.txt", SearchOption.AllDirectories).AsParallel();

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
            }
            finally
            {
                unitOfWork.FlushAndClear();
            }
        }


        private void IntegrarNotfisPendente(Repositorio.UnitOfWork unitOfWork)
        {
            try
            {

                string caminho = caminhoRaiz + @"\Processados\";
                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork);

                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPorTipo(Dominio.Enumeradores.TipoLayoutEDI.NOTFIS).FirstOrDefault();

#if DEBUG
                caminho = Servicos.FS.GetPath(@"C:\Temp\");
                //layoutEDI = repLayoutEDI.BuscarPorCodigo(9);

#endif
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Servicos.Embarcador.Carga.Carga serCarga = new Embarcador.Carga.Carga(unitOfWork);
                Servicos.WebService.Empresa.Empresa serEmpresa = new WebService.Empresa.Empresa(unitOfWork);

                List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> controleIntegracoesCargaEDIs = repControleIntegracaoCargaEDI.BuscarPendenteIntegracao(0, 1);

                List<int> codigosControle = (from obj in controleIntegracoesCargaEDIs select obj.Codigo).ToList();

                foreach (int codigoControle in codigosControle)
                {
                    try
                    {
                        bool integrou = true;
                        Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI = repControleIntegracaoCargaEDI.BuscarPorCodigo(codigoControle);
                        List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao> cargasIntegracoes = new List<Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao>();

                        string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, controleIntegracaoCargaEDI.GuidArquivo);
                        System.IO.MemoryStream ms = new MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoCompleto));
                        ms.Position = 0;
                        Servicos.LeituraEDI leituraEDI = new Servicos.LeituraEDI(null, layoutEDI, ms, unitOfWork, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.GetEncoding("iso-8859-1"));
                        var listaNotas = leituraEDI.GerarNotasFiscais();
                        var retorno = string.Empty;

                        if (integrou)
                        {
                            Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador("NOTFIS");//listaNotas.FirstOrDefault().Emitente.CPFCNPJ

                            if (filial != null && !string.IsNullOrWhiteSpace(filial.CodigoFilialEmbarcador))
                            {
                                string numeroCargaAutomatico = Servicos.Embarcador.Cargas.CargaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork, filial?.Codigo ?? 0).ToString();

                                for (int i = 0; i < listaNotas.Count; i++)
                                {
                                    Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();
                                    cargaIntegracao.DataCriacaoCarga = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                    cargaIntegracao.NumeroCarga = !string.IsNullOrWhiteSpace(listaNotas[i].NumeroDT) ? listaNotas[i].NumeroDT : string.Empty;
                                    if (string.IsNullOrWhiteSpace(cargaIntegracao.NumeroCarga))
                                        cargaIntegracao.NumeroCarga = !string.IsNullOrWhiteSpace(listaNotas[i].NumeroRomaneio) ? listaNotas[i].NumeroRomaneio : numeroCargaAutomatico;

                                    string numeroCargaAuxiliar = string.Empty;
                                    if (cargaIntegracao.NumeroCarga.Contains("-"))
                                    {
                                        List<string> listaNumeros = cargaIntegracao.NumeroCarga.Split('-').ToList();
                                        if (listaNotas.Count > 0)
                                            cargaIntegracao.NumeroCarga = listaNumeros[0];
                                        if (listaNotas.Count > 1)
                                            numeroCargaAuxiliar = listaNumeros[1];
                                    }

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

                                    string cnpjTransportador = Utilidades.String.OnlyNumbers(listaNotas[i].Transportador.CNPJ);
                                    Dominio.Entidades.Empresa transportador = repEmpresa.BuscarPorCNPJ(cnpjTransportador);
                                    if (transportador == null)
                                        retorno = "Transportador " + cnpjTransportador + " não possui cadastro/configuração.";

                                    if (string.IsNullOrWhiteSpace(retorno))
                                    {
                                        cargaIntegracao.Remetente = listaNotas[i].Emitente;
                                        cargaIntegracao.Destinatario = listaNotas[i].Destinatario;
                                        cargaIntegracao.TransportadoraEmitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa { CNPJ = cnpjTransportador };
                                        cargaIntegracao.FecharCargaAutomaticamente = false;

                                        if (listaNotas[i].Expedidor != null)
                                            cargaIntegracao.Expedidor = listaNotas[i].Expedidor;
                                        if (listaNotas[i].Recebedor != null)
                                            cargaIntegracao.Recebedor = listaNotas[i].Recebedor;

                                        cargaIntegracao.Motoristas = new List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista>();

                                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete modalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;

                                        if (listaNotas[i].ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago)
                                            modalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;
                                        else
                                            modalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;

                                        string codigoIntegracaoTipoOperacao = listaNotas[i].TipoOperacao;
                                        cargaIntegracao.TipoCargaEmbarcador = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador() { CodigoIntegracao = "MartinBrower" };
                                        cargaIntegracao.TipoOperacao = new Dominio.ObjetosDeValor.Embarcador.Carga.TipoOperacao() { CodigoIntegracao = !string.IsNullOrWhiteSpace(codigoIntegracaoTipoOperacao) ? codigoIntegracaoTipoOperacao.Trim() : "MartinBrower" };
                                        string codigoIntegracaoModeloVeicular = listaNotas[i].ModeloVeicular;
                                        cargaIntegracao.ModeloVeicular = new Dominio.ObjetosDeValor.Embarcador.Carga.ModeloVeicular() { CodigoIntegracao = !string.IsNullOrWhiteSpace(codigoIntegracaoModeloVeicular) ? codigoIntegracaoModeloVeicular.Trim() : "1" };

                                        //cargaIntegracao.ObservacaoCTe = !string.IsNullOrWhiteSpace(notfis.CabecalhoDocumento.IdCTe) && notfis.CabecalhoDocumento.IdCTe.Length > 10 ? notfis.CabecalhoDocumento.IdCTe.Substring(0, 10) : notfis.CabecalhoDocumento.IdCTe;
                                        //cargaIntegracao.ObservacaoCTe = string.Concat(cargaIntegracao.ObservacaoCTe, " / Dtzão:", cargaIntegracao.NumeroCarga, " / Loja: ", notfis.Destinatario);
                                        cargaIntegracao.ObservacaoCTe = string.Concat(cargaIntegracao.ObservacaoCTe, " / Rota: ", listaNotas[i].Rota);
                                        cargaIntegracao.RotaEmbarcador = !string.IsNullOrWhiteSpace(numeroCargaAuxiliar) ? numeroCargaAuxiliar + "-" + listaNotas[i].Rota : listaNotas[i].Rota;

                                        cargaIntegracao.NotasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal>();
                                        cargaIntegracao.CTes = new List<Dominio.ObjetosDeValor.Embarcador.CTe.CTe>();

                                        cargaIntegracao.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();
                                        Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();
                                        produto.CodigoProduto = "DIVERSOS";
                                        produto.DescricaoProduto = "DIVERSOS";
                                        produto.CodigoGrupoProduto = "1";
                                        produto.DescricaoGrupoProduto = "DIVERSOS";
                                        produto.Quantidade = 1;
                                        produto.PesoUnitario = 1;
                                        cargaIntegracao.Produtos.Add(produto);

                                        bool possuiCTe = false;

                                        if (!string.IsNullOrWhiteSpace(listaNotas[i].Chave))
                                            possuiCTe = listaNotas[i].Chave.Substring(20, 2) == "57";
                                        else
                                        {
                                            integrou = false;
                                            retorno = Localization.Resources.Cargas.ControleGeracaoEDI.ChaveInformadaNaoPertenceNF;
                                            break;
                                        }

                                        Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.EDI.Notfis.NotaFiscal();
                                        if (possuiCTe)
                                        {
                                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(listaNotas[i].Chave);
                                            if (cte == null)
                                            {
                                                integrou = false;
                                                retorno = "CT-e chave " + listaNotas[i].Chave + " não existe na base para emissão de subcontratação.";
                                                break;
                                            }

                                            notaFiscal.CTe = new Dominio.ObjetosDeValor.Embarcador.CTe.CTe();
                                            notaFiscal.CTe.Chave = listaNotas[i].Chave;

                                            pesoBruto += listaNotas[i].PesoBruto;
                                            pesoLiquido += listaNotas[i].PesoLiquido;

                                            cargaIntegracao.NumeroPedidoEmbarcador = listaNotas[i].Numero.ToString() + "_" + listaNotas[i].Serie;

                                            cargaIntegracao.CTes.Add(notaFiscal.CTe);
                                        }
                                        else
                                        {
                                            notaFiscal.NFe = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();
                                            notaFiscal.NFe.Chave = listaNotas[i].Chave;
                                            notaFiscal.NFe.Modelo = "55";
                                            notaFiscal.NFe.Numero = listaNotas[i].Numero;
                                            notaFiscal.NFe.Emitente = listaNotas[i].Emitente;
                                            notaFiscal.NFe.Emitente.RazaoSocial = listaNotas[i].Emitente.RazaoSocial;
                                            notaFiscal.NFe.Destinatario = listaNotas[i].Destinatario;
                                            notaFiscal.NFe.SituacaoNFeSefaz = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz.Autorizada;
                                            notaFiscal.NFe.TipoOperacaoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida;
                                            notaFiscal.NFe.PesoBruto = listaNotas[i].PesoBruto;
                                            notaFiscal.NFe.Valor = listaNotas[i].Valor;
                                            notaFiscal.NFe.Serie = Utilidades.String.OnlyNumbers(listaNotas[i].Serie);

                                            pesoBruto += listaNotas[i].PesoBruto;
                                            pesoLiquido += listaNotas[i].PesoLiquido;

                                            cargaIntegracao.NumeroPedidoEmbarcador = listaNotas[i].Numero.ToString() + "_" + listaNotas[i].Serie;

                                            cargaIntegracao.NotasFiscais.Add(notaFiscal.NFe);
                                        }

                                        //listaNotas[i].ValorFrete ​VALOR TOTAL DE FRETE
                                        //listaNotas[i].ValorFreteLiquido VALOR FRETE LIQUIDO DE ADICIONAIS
                                        //listaNotas[i].ValorComponenteFreteCrossDocking FRETE CROSS DOCKING
                                        //listaNotas[i].ValorComponenteAdValorem VALOR AD VALOREM
                                        //listaNotas[i].ValorComponenteDescarga VALOR DESCARGA

                                        if (listaNotas[i].ValorFreteLiquido > 0)
                                        {
                                            cargaIntegracao.ValorFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.FreteValor();
                                            cargaIntegracao.ValorFrete.FreteProprio = listaNotas[i].ValorFreteLiquido;
                                            //cargaIntegracao.ValorFrete.ValorPrestacaoServico = listaNotas[i].ValorFrete;
                                            //cargaIntegracao.ValorFrete.ValorTotalAReceber = listaNotas[i].ValorFrete;

                                            cargaIntegracao.ValorFrete.ComponentesAdicionais = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional>();

                                            if (listaNotas[i].ValorComponenteFreteCrossDocking > 0)
                                            {
                                                Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                                                componenteFrete.ValorComponente = listaNotas[i].ValorComponenteFreteCrossDocking;
                                                componenteFrete.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                                                componenteFrete.Componente.CodigoIntegracao = "CROSS";
                                                componenteFrete.IncluirBaseCalculoICMS = true;
                                                componenteFrete.IncluirTotalReceber = true;
                                                cargaIntegracao.ValorFrete.ComponentesAdicionais.Add(componenteFrete);
                                            }
                                            if (listaNotas[i].ValorComponenteAdValorem > 0)
                                            {
                                                Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                                                componenteFrete.ValorComponente = listaNotas[i].ValorComponenteAdValorem;
                                                componenteFrete.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                                                componenteFrete.Componente.CodigoIntegracao = "ADVALOREM";
                                                componenteFrete.IncluirBaseCalculoICMS = true;
                                                componenteFrete.IncluirTotalReceber = true;
                                                cargaIntegracao.ValorFrete.ComponentesAdicionais.Add(componenteFrete);
                                            }
                                            if (listaNotas[i].ValorComponenteDescarga > 0)
                                            {
                                                Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                                                componenteFrete.ValorComponente = listaNotas[i].ValorComponenteDescarga;
                                                componenteFrete.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                                                componenteFrete.Componente.CodigoIntegracao = "DESCARGA";
                                                componenteFrete.IncluirBaseCalculoICMS = true;
                                                componenteFrete.IncluirTotalReceber = true;
                                                cargaIntegracao.ValorFrete.ComponentesAdicionais.Add(componenteFrete);
                                            }
                                            if (listaNotas[i].ValorComponentePedagio > 0)
                                            {
                                                Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional componenteFrete = new Dominio.ObjetosDeValor.Embarcador.Frete.ComponenteAdicional();
                                                componenteFrete.ValorComponente = listaNotas[i].ValorComponentePedagio;
                                                componenteFrete.Componente = new Dominio.ObjetosDeValor.Embarcador.Frete.Componente();
                                                componenteFrete.Componente.CodigoIntegracao = "PEDAGIO";
                                                componenteFrete.IncluirBaseCalculoICMS = true;
                                                componenteFrete.IncluirTotalReceber = true;
                                                cargaIntegracao.ValorFrete.ComponentesAdicionais.Add(componenteFrete);
                                            }


                                        }

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

                                        if (repCargaPedido.ExistePorNumeroPedido(cargaIntegracao.NumeroPedidoEmbarcador))
                                        {
                                            retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.PedidoImportadoAnteriormente, cargaIntegracao.NumeroPedidoEmbarcador);
                                            integrou = false;
                                            break;
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
                            else
                            {
                                retorno = string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.NaoEncontradaFilial, listaNotas.FirstOrDefault().Emitente.CPFCNPJ);
                                integrou = false;
                                //break;
                            }
                        }
                        else
                        {
                            //break;
                        }


                        if (integrou)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                            for (int ci = 0; ci < cargasIntegracoes.Count; ci++)
                            {
                                Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao = cargasIntegracoes[ci];

                                unitOfWork.FlushAndClear();

                                Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> protocolo = AdicionarCarga(cargaIntegracao, false, unitOfWork);
                                if (protocolo.Status)
                                {
                                    //Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(protocolo.Objeto.protocoloIntegracaoCarga, protocolo.Objeto.protocoloIntegracaoPedido);
                                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorProtocoloCargaOrigemEProtocoloPedido(protocolo.Objeto.protocoloIntegracaoCarga, protocolo.Objeto.protocoloIntegracaoPedido);

                                    if (!cargas.Any(obj => obj.Codigo == cargaPedido.Carga.Codigo))
                                        cargas.Add(cargaPedido.Carga);

                                }
                                else
                                {
                                    integrou = false;
                                    RejeicaoEDI(codigoControle, protocolo.Mensagem, unitOfWork);
                                    break;
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
                                            unitOfWork.Start();
                                            //Servicos.Embarcador.Seguro.Seguro.SetarDadosSeguroCarga(carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, unitOfWork);

                                            serCarga.FecharCarga(carga, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, null);
                                            carga.CargaFechada = true;
                                            Servicos.Log.TratarErro("3 - Fechou Carga (" + carga.CodigoCargaEmbarcador + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "TempoProcessamento");


                                            if (carga.Filial?.EmitirMDFeManualmente ?? false)
                                                carga.NaoGerarMDFe = true;

                                            repCarga.Atualizar(carga);

                                            nControleIntegracaoCargaEDI.Cargas.Add(carga);
                                            nControleIntegracaoCargaEDI.NumeroDT += carga.CodigoCargaEmbarcador;
                                            if (cargas.Count > 1 && cargas.LastOrDefault().Codigo != carga.Codigo)
                                                nControleIntegracaoCargaEDI.NumeroDT += ", ";
                                            unitOfWork.CommitChanges();
                                        }
                                        else
                                        {
                                            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                                            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracao.BuscarConfiguracaoPadrao();
                                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                                            //Dominio.ObjetosDeValor.Embarcador.Carga.RetornoRotasCarga dynRota = ObterDadosRotas(carga, unitOfWork);
                                            serCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracaoTMS, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
                                            //Servicos.Embarcador.Seguro.Seguro.SetarDadosSeguroCarga(carga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, unitOfWork);
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
                            }
                        }
                        else
                        {
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

        public Retorno<Dominio.ObjetosDeValor.WebService.Carga.Protocolos> AdicionarCarga(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao, bool gerarCargaSegundoTrecho, Repositorio.UnitOfWork unitOfWork)
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

                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = serCargaWS.CriarCarga(pedido, cargaIntegracao, ref protocoloPedidoExistente, ref stMensagem, ref codigoCargaExistente, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador, true, false, null, configuracao, null, "", filial, tipoOperacao);

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
                                retorno = "Não existe uma cidade com o nome " + endereco.Cidade.Descricao + " - " + endereco.Cidade.SiglaUF + " cadastrada na base multisoftware";
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

