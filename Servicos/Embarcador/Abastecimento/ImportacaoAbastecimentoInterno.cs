using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace Servicos.Embarcador.Abastecimento
{
    public class ImportacaoAbastecimentoInterno : ServicoBase
    {
        public ImportacaoAbastecimentoInterno(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        private int tempoThread = 1000;
        private string caminhoArquivos = Servicos.FS.GetPath(@"C:\Arquivos Embarcador\FTP");
        private string caminhoRaiz = Servicos.FS.GetPath(@"C:\inetpub\ftproot\Tombini\Abastecimentos");
        private string TipoArmazenamento = "pasta";
        private string EnderecoFTP = "";
        private string UsuarioFTP = "";
        private string SenhaFTP = "";
        private string CaminhoRaizFTP = "";
        private bool FTPPassivo = true;
        private string PortaFTP = "21";
        private bool UtilizaSFTP = false;
        private string AdminStringConexao = "";
        private string caminhoArmazenamento = "";

        #region Métodos Públicos

        public void Iniciar(string caminhoRaizArquivos, string tipoArmazenamento, string enderecoFTP, string usuarioFTP, string senhaFTP, string caminhoRaizFTP, bool ftpPassivo, string portaFTP, bool utilizaSFTP, string adminMultisoftware, int TempoThread, string CaminhoArmazenamento)
        {
            Thread thread = new Thread(new ThreadStart(ExecutarThread));
            thread.CurrentUICulture = new System.Globalization.CultureInfo("pt-BR");
            thread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            thread.IsBackground = true;
            caminhoRaiz = caminhoRaizArquivos;
            caminhoArquivos = caminhoRaizArquivos;
            TipoArmazenamento = tipoArmazenamento;
            EnderecoFTP = enderecoFTP;
            UsuarioFTP = usuarioFTP;
            SenhaFTP = senhaFTP;
            CaminhoRaizFTP = caminhoRaizFTP;
            FTPPassivo = ftpPassivo;
            PortaFTP = portaFTP;
            utilizaSFTP = UtilizaSFTP;
            AdminStringConexao = adminMultisoftware;
            tempoThread = TempoThread;
            caminhoArmazenamento = CaminhoArmazenamento;
            thread.Start();
        }

        #endregion

        #region Métodos Privados

        private void ExecutarThread()
        {
            while (true)
            {
                try
                {
                    BuscarXMLAbastecimento();
                    System.Threading.Thread.Sleep(tempoThread);
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

        private void BuscarXMLAbastecimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento repConfiguracaoFinanceiraAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento configuracaoFinanceiraAbastecimento = repConfiguracaoFinanceiraAbastecimento.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (configuracaoFinanceiraAbastecimento == null)
            {
                Servicos.Log.TratarErro("Nenhum movimento financeiro padrão configurado para essa operação.");
                return;
            }

            string pasta = "";
            string pastaFTP = "";
            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, pasta);

            if (TipoArmazenamento == "ftp")
            {
                string caminhoFTP = CaminhoRaizFTP + pastaFTP.Replace(@"\", "/");
                string erro = "";
                Servicos.FTP.DownloadArquivosPasta(EnderecoFTP, PortaFTP, caminhoFTP, UsuarioFTP, SenhaFTP, FTPPassivo, false, caminho, out erro, UtilizaSFTP, false, "", true, false, true);
                if (!string.IsNullOrWhiteSpace(erro))
                {
                    Servicos.Log.TratarErro(erro);
                    return;
                }
            }

            IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, "*.xml", SearchOption.AllDirectories).AsParallel();

            try
            {
                foreach (string arquivo in arquivos)
                {
                    string fileName = Path.GetFileName(arquivo);

                    unitOfWork.Start();

                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        string data = Utilidades.IO.FileStorageService.Storage.ReadAllText(arquivo).Replace("&", "");
                        byte[] bytes = Encoding.ASCII.GetBytes(data);
                        Stream s = new MemoryStream(bytes);
                        doc.Load(s);

                        string contents = doc.InnerXml;
                        double CNPJPostoInterno = 82809088000666;
                        Dominio.ObjetosDeValor.Embarcador.Abastecimento.Raiz raizXML;
                        Dominio.Entidades.Abastecimento abastecimento = new Dominio.Entidades.Abastecimento();

                        raizXML = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.Embarcador.Abastecimento.Raiz>(contents);

                        int kilometragem = raizXML.Transacoes.Odometro;
                        Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(raizXML.Transacoes.Placa);

                        double.TryParse(raizXML.Transacoes.EstabelecimentoCNPJ, out double CNPJPostoInternoXML);
                        if (CNPJPostoInternoXML > 0)
                            CNPJPostoInterno = CNPJPostoInternoXML;


                        if (!string.IsNullOrEmpty(raizXML?.Header.HoraGeracao))
                            abastecimento.Data = DateTime.Parse($"{raizXML.Transacoes.DataTransacao} {raizXML.Header.HoraGeracao}");
                        else
                            abastecimento.Data = DateTime.Parse(raizXML.Transacoes.DataTransacao);

                        abastecimento.Kilometragem = kilometragem;
                        abastecimento.Litros = decimal.Parse(raizXML.Transacoes.Litros);
                        abastecimento.NomePosto = raizXML.Transacoes.NomeReduzido;
                        abastecimento.Pago = false;
                        abastecimento.Situacao = "F";
                        abastecimento.DataAlteracao = DateTime.Now;
                        abastecimento.Status = "A";
                        abastecimento.ValorUnitario = decimal.Parse(raizXML.Transacoes.ValorTransacao) / (decimal.Parse(raizXML.Transacoes.Litros) > 0 ? decimal.Parse(raizXML.Transacoes.Litros) : 1);
                        abastecimento.Veiculo = veiculo;
                        abastecimento.Posto = repCliente.BuscarPorCPFCNPJ(CNPJPostoInterno);
                        abastecimento.Produto = repProduto.BuscarPorPostoTabelaDeValor(CNPJPostoInterno, raizXML.Transacoes.TipoCombustivel);
                        abastecimento.TipoMovimento = configuracaoFinanceiraAbastecimento.TipoMovimentoLancamentoAbastecimentoBombaPropria;
                        abastecimento.Documento = raizXML.Transacoes.CodigoTransacao;

                        Servicos.Embarcador.Abastecimento.Abastecimento.ProcessarViradaKMHorimetro(abastecimento, abastecimento.Veiculo, abastecimento.Equipamento);

                        if (abastecimento.Veiculo != null && abastecimento.Posto != null && abastecimento.Produto != null)
                        {
                            if (abastecimento.Veiculo.TipoVeiculo == "1" && abastecimento.Veiculo.Equipamentos != null && abastecimento.Veiculo.Equipamentos.Count > 0)
                                abastecimento.Equipamento = abastecimento.Veiculo.Equipamentos[0];

                            //if (abastecimento.Veiculo.Tipo == "T")
                            //    abastecimento.Situacao = "I";
                            Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unitOfWork, veiculo, null, configuracaoTMS);

                            if (abastecimento.Produto.CodigoNCM.StartsWith("310210"))
                                abastecimento.TipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla;
                            else
                                abastecimento.TipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;

                            if (!repAbastecimento.ContemAbastecimento(abastecimento.Veiculo.Codigo, abastecimento.Kilometragem, abastecimento.Documento, abastecimento.Litros, abastecimento.TipoAbastecimento))
                            {
                                repAbastecimento.Inserir(abastecimento);
                                BaixaEstoqueCombustivel(unitOfWork, abastecimento);
                                unitOfWork.CommitChanges();
                            }

                            MoverParaPastaProcessados(Guid.NewGuid().ToString() + fileName, arquivo);
                            Utilidades.IO.FileStorageService.Storage.Delete(arquivo);
                        }
                        else if (abastecimento.Veiculo == null)
                        {
                            Servicos.Log.TratarErro("Veículo: " + raizXML.Transacoes.Placa + " não cadastrado. Arquivo: " + fileName);
                            MoverParaPastaErro(fileName, arquivo);
                            Utilidades.IO.FileStorageService.Storage.Delete(arquivo);
                        }
                        else if (abastecimento.Posto == null)
                        {
                            Servicos.Log.TratarErro("Posto: " + raizXML.Transacoes.RazaoSocial + " CNPJ: " + CNPJPostoInterno.ToString() + " não cadastrado. Arquivo: " + fileName);
                            MoverParaPastaErro(fileName, arquivo);
                            Utilidades.IO.FileStorageService.Storage.Delete(arquivo);
                        }
                        else if (abastecimento.Produto == null)
                        {
                            Servicos.Log.TratarErro("Posto: " + abastecimento.Posto.Nome + " CNPJ: " + CNPJPostoInterno.ToString() + " Código de Integração: " + raizXML.Transacoes.TipoCombustivel + " não cadastrado. Arquivo: " + fileName);
                            MoverParaPastaErro(fileName, arquivo);
                            Utilidades.IO.FileStorageService.Storage.Delete(arquivo);
                        }
                    }
                    catch (Exception ex2)
                    {
                        Servicos.Log.TratarErro("Não foi possível interpretar o arquivo . " + fileName + " da bomba interna");
                        Servicos.Log.TratarErro(ex2);
                        unitOfWork.Rollback();
                        unitOfWork.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                unitOfWork.Dispose();
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void BaixaEstoqueCombustivel(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Abastecimento abastecimento)
        {
            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unitOfWork);

            servicoEstoque.MovimentarEstoque(out string erro, abastecimento.Produto, abastecimento.Litros, Dominio.Enumeradores.TipoMovimento.Saida, "ABS", abastecimento.Codigo.ToString(), abastecimento.ValorUnitario, abastecimento.Empresa, DateTime.Now, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
        }

        private void MoverParaPastaProcessados(string nomeArquivo, string fullName)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(Utilidades.IO.FileStorageService.Storage.ReadAllText(fullName));

            string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArmazenamento, "Processados", "XMLAbastecimento", nomeArquivo);

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoCompleto, bytes);
        }

        private void MoverParaPastaErro(string nomeArquivo, string fullName)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(Utilidades.IO.FileStorageService.Storage.ReadAllText(fullName));

            string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArmazenamento, "ComErro", "XMLAbastecimento", nomeArquivo);

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoCompleto, bytes);
        }

        #endregion
    }
}
