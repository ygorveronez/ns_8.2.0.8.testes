using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace AssinadorXMLNFe
{
    public partial class AssinarXMLNFe : Form
    {
        #region Construtores

        //private string UrlWebService = "http://191.232.51.142/SGT.WebService/EmissaoNFe.svc";//MULTINFE
        //private string UrlWebService = "http://192.168.0.125:85/SGT.WebService/EmissaoNFe.svc";//DEBUG
        //private string UrlWebService = "http://179.96.230.245:8080/SGT.WebService/EmissaoNFe.svc";//MERCANTEC IP 1
        //private string UrlWebService = "http://187.103.243.18:8080/SGT.WebService/EmissaoNFe.svc";//MERCANTEC IP 2
        private string UrlWebService = "";

        public AssinarXMLNFe()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UrlWebService = System.Configuration.ConfigurationManager.AppSettings["UrlWebService"];
            this.Hide();
            notifyIcon1.ShowBalloonTip(1000, "Multi NF-e", "Assinador de XML iniciado com sucesso.", ToolTipIcon.Info);
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void fecharToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AssinarXMLNFe_Move(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.ShowBalloonTip(1000, "Multi NF-e", "Assinador de XML iniciado com sucesso.", ToolTipIcon.Info);
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        #endregion

        #region Métodos Associados a Eventos

        private void buttonAutorizar_Click(object sender, EventArgs e)
        {
            try
            {
                string cnpjCpfEmpresa;
                string serial;
                if (!ObterDadosCertificadoDigital(out cnpjCpfEmpresa, out serial))
                    return;

                EmissaoNFe.EmissaoNFeClient servico = new EmissaoNFe.EmissaoNFeClient();

                servico.Endpoint.Address = new System.ServiceModel.EndpointAddress(UrlWebService);
                Servicos.Log.TratarErro("0 - inicio");
                EmissaoNFe.RetornoOfArrayOfEmissaoNFeBgIEubPB retorno = servico.BuscarNotasAguardandoAssinatura(cnpjCpfEmpresa);
                if (retorno.Status)
                {
                    if (retorno.Objeto.Count() == 0)
                    {
                        MessageBox.Show("Nenhuma Autorização pendente de envio para a empresa selecionada.", "Autorização", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }

                    var z = new Zeus.Embarcador.ZeusNFe.Zeus();
                    string xmlAssinado = "";
                    for (int i = 0; i < retorno.Objeto.Count(); i++)
                    {
                        string xmlNaoAssinado = Descompacta(retorno.Objeto[i].XMLNaoAssinado);

                        byte[] xmlNFe = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding("ISO-8859-1"), Convert.FromBase64String(xmlNaoAssinado));
                        xmlNaoAssinado = System.Text.Encoding.Default.GetString(xmlNFe);

                        string diretorioRaiz = "C:\\XML NF-e\\";

                        xmlAssinado = z.AssinarXMLNFe(cnpjCpfEmpresa, xmlNaoAssinado, serial, retorno.Objeto[i].CodigoNFe, diretorioRaiz);

                        if (!string.IsNullOrWhiteSpace(xmlAssinado))
                        {
                            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe retornoEnvioNFe = z.EnviarNFe(xmlAssinado, retorno.Objeto[i].CodigoNFe, serial, retorno.Objeto[i].ReciboAnterior, retorno.Objeto[i].CodigoStatusAnterior, retorno.Objeto[i].CIdToken, retorno.Objeto[i].Csc, diretorioRaiz);

                            byte[] xmlNFeRetorno = null;
                            xmlNFeRetorno = Encoding.ASCII.GetBytes(retornoEnvioNFe.XML);
                            retornoEnvioNFe.XML = Compacta(Convert.ToBase64String(Encoding.Convert(Encoding.GetEncoding("ISO-8859-1"), Encoding.UTF8, xmlNFeRetorno)));

                            EmissaoNFe.RetornoOfstring retorno2 = servico.SalvarRetornoEnvioNFe(retornoEnvioNFe, retorno.Objeto[i].CodigoNFe);
                            if (!string.IsNullOrWhiteSpace(retorno2.Objeto))
                            {
                                MessageBox.Show(retorno2.Objeto, "Rejeição da Nota Fiscal", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                            else
                            {
                                MessageBox.Show("Autorizado com sucesso", "Retorno Nota Fiscal", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                        }
                    }
                }
                else
                    MessageBox.Show(retorno.Mensagem, "Problemas com a Autorização", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Problemas com a Autorização", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void buttonCancelar_Click(object sender, EventArgs e)
        {
            try
            {
                string diretorioRaiz = "C:\\XML NF-e\\";

                string cnpjCpfEmpresa;
                string serial;
                if (!ObterDadosCertificadoDigital(out cnpjCpfEmpresa, out serial))
                    return;

                EmissaoNFe.EmissaoNFeClient servico = new EmissaoNFe.EmissaoNFeClient();

                servico.Endpoint.Address = new System.ServiceModel.EndpointAddress(UrlWebService);

                EmissaoNFe.RetornoOfArrayOfEmissaoNFeBgIEubPB retorno = servico.BuscarNotasAguardandoCancelamento(cnpjCpfEmpresa);
                if (retorno.Status)
                {
                    if (retorno.Objeto.Count() == 0)
                    {
                        MessageBox.Show("Nenhum Cancelamento pendente de envio para a empresa selecionada.", "Cancelamento", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }

                    var z = new Zeus.Embarcador.ZeusNFe.Zeus();
                    for (int i = 0; i < retorno.Objeto.Count(); i++)
                    {
                        if (!string.IsNullOrWhiteSpace(retorno.Objeto[i].XMLDistribuicao) && !string.IsNullOrWhiteSpace(retorno.Objeto[i].XMLNaoAssinado))
                        {
                            string[] dadosCancelamento = retorno.Objeto[i].XMLNaoAssinado.Split('|');
                            string codigo = dadosCancelamento[0];
                            string sequencia = dadosCancelamento[1];
                            string protocolo = dadosCancelamento[2];
                            string chave = dadosCancelamento[3];
                            string justificativa = dadosCancelamento[4];
                            string cnpjCpf = dadosCancelamento[5];
                            DateTime dataEmissao = DateTime.MinValue;
                            if (dadosCancelamento.Length >= 7)
                                DateTime.TryParse(dadosCancelamento[6], out dataEmissao);
                            if (dataEmissao == DateTime.MinValue)
                                dataEmissao = DateTime.Now;

                            string xmlDistribuicao = Descompacta(retorno.Objeto[i].XMLDistribuicao);
                            byte[] xmlNFe = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding("ISO-8859-1"), Convert.FromBase64String(xmlDistribuicao));
                            xmlDistribuicao = System.Text.Encoding.Default.GetString(xmlNFe);

                            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe retornoEnvioNFe = z.CancelarNFe(dataEmissao, xmlDistribuicao, codigo, sequencia, protocolo, chave, justificativa, cnpjCpf, serial, diretorioRaiz);
                            EmissaoNFe.RetornoOfstring retorno2 = servico.SalvarRetornoEnvioNFe(retornoEnvioNFe, retorno.Objeto[i].CodigoNFe);
                            if (!string.IsNullOrWhiteSpace(retorno2.Objeto))
                            {
                                MessageBox.Show(retorno2.Objeto, "Rejeição do Cancelamento", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                            else
                            {
                                MessageBox.Show("Cancelado com sucesso", "Retorno Cancelamento", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                        }
                        else
                            MessageBox.Show("Cancelamento sem xml de distribuição e/ou sem arquivo para ser assinado", "Retorno Nota Fiscal", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                else
                    MessageBox.Show(retorno.Mensagem, "Problemas com o Cancelamento", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Problemas com o Cancelamento", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void buttonInutilizar_Click(object sender, EventArgs e)
        {
            try
            {
                string diretorioRaiz = "C:\\";
                string cnpjCpfEmpresa;
                string serial;
                if (!ObterDadosCertificadoDigital(out cnpjCpfEmpresa, out serial))
                    return;

                EmissaoNFe.EmissaoNFeClient servico = new EmissaoNFe.EmissaoNFeClient();

                servico.Endpoint.Address = new System.ServiceModel.EndpointAddress(UrlWebService);

                EmissaoNFe.RetornoOfArrayOfEmissaoNFeBgIEubPB retorno = servico.BuscarNotasAguardandoInutilizacao(cnpjCpfEmpresa);
                if (retorno.Status)
                {
                    if (retorno.Objeto.Count() == 0)
                    {
                        MessageBox.Show("Nenhuma Inutilização pendente de envio para a empresa selecionada.", "Inutilização", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }

                    var z = new Zeus.Embarcador.ZeusNFe.Zeus();
                    for (int i = 0; i < retorno.Objeto.Count(); i++)
                    {
                        if (!string.IsNullOrWhiteSpace(retorno.Objeto[i].XMLNaoAssinado))
                        {
                            string[] dadosCancelamento = retorno.Objeto[i].XMLNaoAssinado.Split('|');
                            string cnpj = dadosCancelamento[0];
                            string anoAtual = dadosCancelamento[1];
                            string serieNFe = dadosCancelamento[2];
                            string numero = dadosCancelamento[3];
                            string justificativa = dadosCancelamento[4];
                            string modelo = retorno.Objeto[i].Modelo;

                            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe retornoEnvioNFe = z.InutilizarNFe(cnpj, anoAtual, serieNFe, numero, justificativa, serial, retorno.Objeto[i].TipoAmbiente, retorno.Objeto[i].UFEmpresa, modelo, diretorioRaiz);
                            EmissaoNFe.RetornoOfstring retorno2 = servico.SalvarRetornoEnvioNFe(retornoEnvioNFe, retorno.Objeto[i].CodigoNFe);
                            if (!string.IsNullOrWhiteSpace(retorno2.Objeto))
                            {
                                MessageBox.Show(retorno2.Objeto, "Rejeição da Inutilização", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                            else
                            {
                                MessageBox.Show("Inutilizado com sucesso", "Retorno Inutilização", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                        }
                        else
                            MessageBox.Show("Inutilização sem xml de distribuição e/ou sem arquivo para ser assinado", "Retorno Nota Fiscal", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                else
                    MessageBox.Show(retorno.Mensagem, "Problemas com a Inutilização", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Problemas com a Inutilização", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void buttonCartaCorrecao_Click(object sender, EventArgs e)
        {
            try
            {
                string diretorioRaiz = "C:\\";
                string cnpjCpfEmpresa;
                string serial;
                if (!ObterDadosCertificadoDigital(out cnpjCpfEmpresa, out serial))
                    return;

                EmissaoNFe.EmissaoNFeClient servico = new EmissaoNFe.EmissaoNFeClient();

                servico.Endpoint.Address = new System.ServiceModel.EndpointAddress(UrlWebService);

                EmissaoNFe.RetornoOfArrayOfEmissaoNFeBgIEubPB retorno = servico.BuscarNotasAguardandoCartaCorrecao(cnpjCpfEmpresa);
                if (retorno.Status)
                {
                    if (retorno.Objeto.Count() == 0)
                    {
                        MessageBox.Show("Nenhuma Carta de Correção pendente de envio para a empresa selecionada.", "Carta de Correção", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return;
                    }

                    var z = new Zeus.Embarcador.ZeusNFe.Zeus();
                    for (int i = 0; i < retorno.Objeto.Count(); i++)
                    {
                        if (!string.IsNullOrWhiteSpace(retorno.Objeto[i].XMLNaoAssinado))
                        {
                            string[] dadosCartaCorrecao = retorno.Objeto[i].XMLNaoAssinado.Split('|');
                            string codigoNFe = dadosCartaCorrecao[0];
                            string sequencia = dadosCartaCorrecao[1];
                            string chave = dadosCartaCorrecao[2];
                            string correcao = dadosCartaCorrecao[3];
                            string cnpjCpf = dadosCartaCorrecao[4];
                            DateTime dataEmissao = DateTime.MinValue;
                            if (dadosCartaCorrecao.Length >= 6)
                                DateTime.TryParse(dadosCartaCorrecao[5], out dataEmissao);
                            if (dataEmissao == DateTime.MinValue)
                                dataEmissao = DateTime.Now;

                            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe retornoEnvioNFe = z.CartaCorrecaoNFe(dataEmissao, retorno.Objeto[i].XMLDistribuicao, codigoNFe, sequencia, chave, correcao, cnpjCpf, serial, diretorioRaiz);
                            EmissaoNFe.RetornoOfstring retorno2 = servico.SalvarRetornoEnvioNFe(retornoEnvioNFe, retorno.Objeto[i].CodigoNFe);
                            if (!string.IsNullOrWhiteSpace(retorno2.Objeto))
                            {
                                MessageBox.Show(retorno2.Objeto, "Rejeição da Carta de Correção", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                            else
                            {
                                MessageBox.Show("Carta de Correção gerada com sucesso", "Retorno Carta de Correção", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                        }
                        else
                            MessageBox.Show("Carta de Correção sem xml de distribuição e/ou sem arquivo para ser assinado", "Retorno Nota Fiscal", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
                else
                    MessageBox.Show(retorno.Mensagem, "Problemas com a Carta de Correção", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Problemas com a Carta de Correção", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        #endregion

        #region Métodos Privados

        private static string Compacta(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream();
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }

            ms.Position = 0;
            MemoryStream outStream = new MemoryStream();

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }

        private static string Descompacta(string compressedText)
        {
            byte[] gzBuffer = Convert.FromBase64String(compressedText);
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }
                return Encoding.UTF8.GetString(buffer);
            }
        }

        private X509Certificate2Collection ObtemUmaColecaoDeCertificados()
        {
            X509Store stores = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            try
            {
                // Abre o Store
                stores.Open(OpenFlags.ReadOnly);

                // Obtém a coleção dos certificados da Store
                X509Certificate2Collection certificados = stores.Certificates;

                // percorre a coleção de certificados
                foreach (X509Certificate2 certificado in certificados)
                {
                    // faz algo com o certificado
                }

                return certificados;
            }
            finally
            {
                stores.Close();
            }
        }

        private bool ObterDadosCertificadoDigital(out string cnpjCpfEmpresa, out string serial)
        {
            cnpjCpfEmpresa = string.Empty;
            serial = string.Empty;

            X509Certificate2Collection colecaoCertificados = ObtemUmaColecaoDeCertificados();
            X509Certificate2Collection certificadosSelecionados = X509Certificate2UI.SelectFromCollection(
                colecaoCertificados,
                "Certificados Digitais Disponíveis",
                "Selecione o certificado digital para uso no aplicativo",
                X509SelectionFlag.SingleSelection);

            if (certificadosSelecionados.Count == 0)
                return false;

            X509Certificate2 certificadoSelecionado = certificadosSelecionados[0];
            DateTime dataVencimento = DateTime.Parse(certificadoSelecionado.GetExpirationDateString());
            if (dataVencimento < DateTime.Now)
            {
                MessageBox.Show("O certificado digital selecionado está vencido!", "Validade Certificado", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }

            serial = certificadoSelecionado.GetSerialNumberString();

            cnpjCpfEmpresa = certificadoSelecionado.ObterCnpj();
            if (string.IsNullOrWhiteSpace(cnpjCpfEmpresa))
                cnpjCpfEmpresa = certificadoSelecionado.ObterCpf();

            if (string.IsNullOrWhiteSpace(cnpjCpfEmpresa))
            {
                MessageBox.Show("Não foi possível identificar a empresa do certificado digital, favor verificar o certificado selecionado!" +
                    "\n\nDúvidas entrar em contato com o suporte.", "Empresa Certificado", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }

            return true;
        }

        #endregion
    }
}
