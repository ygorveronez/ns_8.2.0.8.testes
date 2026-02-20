using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Servicos.EDI
{
    public class StartupEDINatura : ServicoBase
    {
        public StartupEDINatura(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        delegate void AtualizarStatusDelegate(string value);
        private int tempoThread = 4000;
        //Dominio.Entidades.Empresa Empresa;
        private int CodigoEmpresa = 0;
        private DateTime ultimaConsultaDT = DateTime.MinValue;
        private int minutosParaConsulta = 15;
        private string caminhoArquivos = Servicos.FS.GetPath(@"D:\Arquivos\FTP");
        private string caminhoRaiz = Servicos.FS.GetPath(@"D:\Arquivos");
        private List<string> FiliaisNatura;
        private string TipoArmazenamento = "pasta";
        private string EnderecoFTP = "";
        private string UsuarioFTP = "";
        private string SenhaFTP = "";
        private string CaminhoRaizFTP = "";
        private bool FTPPassivo = true;
        private string PortaFTP = "21";
        private bool UtilizaSFTP = false;
        private bool GerarNotFisPorNota = false;
        private string Emails = "";

        public void Iniciar(int empresa, int minutosParaConsultaDT, string caminhoRaizArquivos, List<string> filiaisNatura, string tipoArmazenamento, string enderecoFTP, string usuarioFTP, string senhaFTP, string caminhoRaizFTP, string emails, bool ftpPassivo, string portaFTP, bool utilizaSFTP, bool gerarNotFisPorNota)
        {
            Thread thread = new Thread(new ThreadStart(ExecutarThread));
            thread.CurrentUICulture = new System.Globalization.CultureInfo("pt-BR");
            thread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            thread.IsBackground = true;
            CodigoEmpresa = empresa;
            caminhoRaiz = caminhoRaizArquivos;
            caminhoArquivos = caminhoRaizArquivos + @"\FTP";
            minutosParaConsulta = minutosParaConsultaDT;
            FiliaisNatura = filiaisNatura;
            TipoArmazenamento = tipoArmazenamento;
            EnderecoFTP = enderecoFTP;
            UsuarioFTP = usuarioFTP;
            SenhaFTP = senhaFTP;
            CaminhoRaizFTP = caminhoRaizFTP;
            FTPPassivo = ftpPassivo;
            PortaFTP = portaFTP;
            utilizaSFTP = UtilizaSFTP;
            GerarNotFisPorNota = gerarNotFisPorNota;
            Emails = emails;

            thread.Start();
        }

        private void ExecutarThread()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(tempoThread);
                try
                {
                    if (DateTime.Now.Hour > 21 || DateTime.Now.Hour < 4)
                        tempoThread = 90000;
                    else
                        tempoThread = TipoArmazenamento == "ftp" ? 60000 : 4000;

                    buscarNotfisContingenciaNatura();
                    buscarDocumentosTransportesNatura();
                    if (GerarNotFisPorNota)
                        gerarNOTFISDocumentosTransporteNaturaPorNota();//Gerar um notfis para cada nota
                    else
                        gerarNOTFISDocumentosTransporteNaturaPorDT();//Gerar um notfis para cada DT
                    buscarCTesAutorizados();
                    buscarCTesCancelados();
                    envarCTesAutorizadosNatura();
                    gerarOcorrenciaNatura();
                    enviarOcorrenciasNatura();
                    gerarFaturaNatura();
                    enviarFaturasNatura();
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

        private void buscarNotfisContingenciaNatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            Servicos.Natura serNatura = new Servicos.Natura(unitOfWork);
            string pasta = @"\Enviados\Notfis_Contingencia\";
            string caminho = caminhoArquivos + pasta;

            if (TipoArmazenamento == "ftp")
            {
                string caminhoFTP = CaminhoRaizFTP + pasta.Replace(@"\", "/");
                string erro = "";
                Servicos.FTP.DownloadArquivosPasta(EnderecoFTP, PortaFTP, caminhoFTP, UsuarioFTP, SenhaFTP, FTPPassivo, false, caminho, out erro, UtilizaSFTP, false, "", true);
                if (!string.IsNullOrWhiteSpace(erro))
                {
                    Servicos.Log.TratarErro(erro);
                    return;
                }
            }

            IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, "*.txt", SearchOption.AllDirectories).AsParallel();

            try
            {
                foreach (string arquivo in arquivos)
                {
                    string fileName = Path.GetFileName(arquivo);

                    using System.IO.MemoryStream ms = new MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo));

                    unitOfWork.Start();

                    try
                    {
                        serNatura.GerarDocumentosTransporteViaArquivoContingencia(ms, unitOfWork);

                        MoverParaPastaProcessados(fileName, arquivo);

                        unitOfWork.CommitChanges();
                    }
                    catch (Exception ex2)
                    {
                        Servicos.Log.TratarErro("Não foi possível interpretar o arquivo . " + fileName + " de contingencia da Natura");
                        Servicos.Log.TratarErro(ex2);
                        MoverParaPastaNaoProcessados(fileName, arquivo);
                        unitOfWork.Rollback();
                        unitOfWork.Dispose();
                    }
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);
                    ms.Close();
                    ms.Dispose();

                }
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                unitOfWork.Dispose();
                throw;
            }

            IEnumerable<string> arquivosExcel = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, "*.xlsx", SearchOption.AllDirectories).AsParallel();

            try
            {
                foreach (string arquivo in arquivosExcel)
                {
                    string fileName = Path.GetFileName(arquivo);
                    using System.IO.MemoryStream msExcel = new MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo));

                    unitOfWork.Start();

                    DataSet ds = new DataSet();

                    string excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + arquivo + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    using OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);

                    try
                    {
                        excelConnection.Open();
                        DataTable dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            Servicos.Log.TratarErro("Não foi possível ler a planilha excel");
                            MoverParaPastaNaoProcessados(fileName, arquivo);
                            unitOfWork.Rollback();
                            break;
                        }

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        using OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        using OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1);
                        dataAdapter.Fill(ds);

                        foreach (DataRow linha in ds.Tables[0].Rows)
                        {
                            serNatura.ProcessarLinha(linha[0].ToString(), unitOfWork);
                        }
                        excelConnection.Close();

                        MoverParaPastaProcessados(fileName, arquivo);
                        unitOfWork.CommitChanges();
                    }
                    catch (Exception ex2)
                    {
                        Servicos.Log.TratarErro("Não foi possível interpretar o arquivo . " + fileName + " de contingencia da Natura");
                        Servicos.Log.TratarErro(ex2);
                        MoverParaPastaNaoProcessados(fileName, arquivo);
                        unitOfWork.Rollback();
                        unitOfWork.Dispose();
                    }

                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);
                    msExcel.Close();
                    msExcel.Dispose();
                }
                unitOfWork.Dispose();

            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                unitOfWork.Dispose();
                throw;
            }

        }

        private void enviarFaturasNatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            try
            {
                Repositorio.FaturaNatura repFaturaNatura = new Repositorio.FaturaNatura(unitOfWork);

                Servicos.Natura serNatura = new Natura(unitOfWork);

                foreach (string filial in FiliaisNatura)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigoTransportadorNatura(filial);
                    if (empresa != null)
                    {
                        int count = repFaturaNatura.ContarPorSituacao(empresa.Codigo, Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Pendente);
                        for (int i = 0; i < count; i += 10)
                        {
                            List<Dominio.Entidades.FaturaNatura> faturasNatura = repFaturaNatura.BuscarPorSituacao(empresa.Codigo, Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Pendente, i, 10);
                            if (faturasNatura.Count > 0)
                            {
                                foreach (Dominio.Entidades.FaturaNatura fatura in faturasNatura)
                                {
                                    try
                                    {
                                        unitOfWork.Start();
                                        ServicoNaturaNovo.ProcessaFatura.DT_ProcessaFaturaResponseDados[] retorno = serNatura.EmitirFatura(fatura.Codigo, unitOfWork);

                                        System.Text.StringBuilder st = new StringBuilder();

                                        string assunto = "Fatura " + fatura.Numero + " Enviada para a Natura";
                                        st.Append("Fatura: " + fatura.Numero).AppendLine();

                                        for (int j = 0; j < retorno.Length; j++)
                                            st.Append("Retorno da Natura : " + retorno[j].number + " - " + retorno[j].message).AppendLine();

                                        unitOfWork.CommitChanges();

                                        EnviarEmail(assunto, st.ToString(), unitOfWork);

                                        System.Threading.Thread.Sleep(60000);
                                    }
                                    catch (Exception)
                                    {
                                        unitOfWork.Rollback();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void EnviarEmail(string assunto, string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            if (!string.IsNullOrWhiteSpace(Emails))
            {
                string[] splitEmail = Emails.Split(';');
                string email = splitEmail[0];
                string cc = "";
                string bcc = "";
                if (splitEmail.Length > 1)
                    cc = splitEmail[1];
                if (splitEmail.Length > 2)
                    bcc = splitEmail[2];

                Servicos.Email serEnviarEmail = new Email(unitOfWork);
                serEnviarEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, email, bcc, cc, assunto, mensagem, string.Empty, null, "", true, string.Empty, 0, unitOfWork);
            }
        }

        //private void enviarFaturasNatura()
        //{

        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
        //    try
        //    {
        //        unitOfWork.Start();
        //        Repositorio.FaturaNatura repFaturaNatura = new Repositorio.FaturaNatura(StringConexao);

        //        Servicos.Natura serNatura = new Natura(StringConexao);

        //        foreach (string filial in FiliaisNatura)
        //        {
        //            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
        //            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigoTransportadorNatura(filial);
        //            if (empresa != null)
        //            {
        //                int count = repFaturaNatura.ContarPorSituacao(empresa.Codigo, Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Pendente);
        //                for (int i = 0; i < count; i += 10)
        //                {
        //                    List<Dominio.Entidades.FaturaNatura> faturasNatura = repFaturaNatura.BuscarPorSituacao(empresa.Codigo, Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Pendente, i, 10);
        //                    if (faturasNatura.Count > 0)
        //                    {
        //                        foreach (Dominio.Entidades.FaturaNatura fatura in faturasNatura)
        //                        {
        //                            serNatura.EmitirFatura(fatura.Codigo);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        unitOfWork.CommitChanges();
        //    }
        //    catch (Exception)
        //    {
        //        unitOfWork.Rollback();
        //        throw;
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //}

        private void gerarFaturaNatura()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa Empresa = repEmpresa.BuscarPorCodigo(CodigoEmpresa);
                Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscalDocumentoTransporteNatura = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);
                Repositorio.FaturaNatura repFaturaNatura = new Repositorio.FaturaNatura(unitOfWork);
                Repositorio.ItemFaturaNatura repItemFaturaNatura = new Repositorio.ItemFaturaNatura(unitOfWork);

                string pasta = @"\Enviados\Doccob\";
                string caminho = caminhoArquivos + pasta;

                if (TipoArmazenamento == "ftp")
                {
                    string caminhoFTP = CaminhoRaizFTP + pasta.Replace(@"\", "/");
                    string erro = "";
                    Servicos.FTP.DownloadArquivosPasta(EnderecoFTP, PortaFTP, caminhoFTP, UsuarioFTP, SenhaFTP, FTPPassivo, false, caminho, out erro, UtilizaSFTP, false, "", true);
                    if (!string.IsNullOrWhiteSpace(erro))
                    {
                        Servicos.Log.TratarErro(erro);
                        return;
                    }
                }

                IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, "*.txt", SearchOption.AllDirectories).AsParallel();

                Dominio.Entidades.LayoutEDI layoutEDI = (from obj in Empresa.LayoutsEDI where obj.Tipo == Dominio.Enumeradores.TipoLayoutEDI.DOCCOB select obj).FirstOrDefault();

                List<Dominio.Entidades.DocumentoTransporteNatura> dtOcorrencias = new List<Dominio.Entidades.DocumentoTransporteNatura>();
                foreach (string arquivo in arquivos)
                {
                    try
                    {
                        string fileName = Path.GetFileName(arquivo);
                        using System.IO.MemoryStream ms = new MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo));

                        unitOfWork.Start();

                        bool gerouFaturaNatura = false;

                        Servicos.LeituraEDI leituraEDI = new LeituraEDI(Empresa, layoutEDI, ms, unitOfWork);
                        List<Dominio.ObjetosDeValor.EDI.Fatura> faturas = leituraEDI.GerarFaturasNatura();

                        foreach (Dominio.ObjetosDeValor.EDI.Fatura fatura in faturas)
                        {
                            List<Dominio.Entidades.ItemFaturaNatura> itemFaturaNaturaValido = new List<Dominio.Entidades.ItemFaturaNatura>();
                            foreach (Dominio.ObjetosDeValor.EDI.FaturaCTe faturaCTe in fatura.faturaCTes)
                            {
                                Dominio.Entidades.ItemFaturaNatura itemFaturaNatura = new Dominio.Entidades.ItemFaturaNatura();

                                Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscalDocumentoTransporteNatura = null;

                                foreach (string filial in FiliaisNatura)
                                {
                                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigoTransportadorNatura(filial);
                                    if (empresa != null)
                                    {
                                        if (notaFiscalDocumentoTransporteNatura == null)
                                            notaFiscalDocumentoTransporteNatura = repNotaFiscalDocumentoTransporteNatura.BuscarNumeroSerieCTe(empresa.Codigo, !string.IsNullOrWhiteSpace(faturaCTe.Serie) ? int.Parse(faturaCTe.Serie) : 0, int.Parse(faturaCTe.Numero));
                                    }
                                }

                                if (notaFiscalDocumentoTransporteNatura != null)
                                {
                                    itemFaturaNatura.AliquotaCOFINS = notaFiscalDocumentoTransporteNatura.CTe.AliquotaCOFINS;
                                    itemFaturaNatura.AliquotaICMS = notaFiscalDocumentoTransporteNatura.CTe.AliquotaICMS;
                                    itemFaturaNatura.AliquotaPIS = notaFiscalDocumentoTransporteNatura.CTe.AliquotaPIS;
                                    itemFaturaNatura.BaseCalculoICMS = notaFiscalDocumentoTransporteNatura.CTe.BaseCalculoICMS;
                                    itemFaturaNatura.BaseCalculoPIS = notaFiscalDocumentoTransporteNatura.CTe.BasePIS;
                                    itemFaturaNatura.ValorCOFINS = notaFiscalDocumentoTransporteNatura.CTe.ValorCOFINS;
                                    itemFaturaNatura.ValorICMS = notaFiscalDocumentoTransporteNatura.CTe.ValorICMS;
                                    itemFaturaNatura.ValorPIS = notaFiscalDocumentoTransporteNatura.CTe.ValorPIS;
                                    itemFaturaNatura.NotaFiscal = notaFiscalDocumentoTransporteNatura;
                                    itemFaturaNaturaValido.Add(itemFaturaNatura);
                                }
                            }

                            if (itemFaturaNaturaValido.Count > 0)
                            {
                                Dominio.Entidades.FaturaNatura faturaNatura = new Dominio.Entidades.FaturaNatura();
                                faturaNatura.DataEmissao = fatura.DataEmissao;
                                faturaNatura.DataVencimento = fatura.DataVencimento;
                                faturaNatura.Empresa = Empresa;
                                faturaNatura.Numero = fatura.Numero;
                                faturaNatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusFaturaNatura.Pendente;
                                faturaNatura.ValorFrete = fatura.ValorDocumentoCobranca;
                                faturaNatura.DataPreFatura = DateTime.Now;
                                repFaturaNatura.Inserir(faturaNatura);
                                foreach (Dominio.Entidades.ItemFaturaNatura itemFaturaNatura in itemFaturaNaturaValido)
                                {
                                    itemFaturaNatura.Fatura = faturaNatura;
                                    itemFaturaNatura.ValorDoDesconto = fatura.ValorDesconto / itemFaturaNaturaValido.Count;
                                    repItemFaturaNatura.Inserir(itemFaturaNatura);
                                }
                                gerouFaturaNatura = true;
                            }
                        }
                        if (gerouFaturaNatura)
                            MoverParaPastaProcessados(fileName, arquivo);
                        else
                        {
                            Servicos.Log.TratarErro("Não foi possível gerar a fatura do Proceda Doccob dos arquivos: " + fileName);
                            MoverParaPastaNaoProcessados(fileName, arquivo);
                        }

                        Utilidades.IO.FileStorageService.Storage.Delete(arquivo);
                        ms.Close();
                        ms.Dispose();

                        unitOfWork.CommitChanges();
                    }
                    catch (Exception)
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void enviarOcorrenciasNatura()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            try
            {

                Repositorio.DocumentoTransporteNatura repDocumentoTransporteNatura = new Repositorio.DocumentoTransporteNatura(unitOfWork);
                Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscalDocumentoTransporteNatura = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);
                Repositorio.OcorrenciaDeCTe repOcorrenciaDeCTe = new Repositorio.OcorrenciaDeCTe(unitOfWork);
                Servicos.Natura serNatura = new Natura(unitOfWork);

                foreach (string filial in FiliaisNatura)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigoTransportadorNatura(filial);
                    if (empresa != null)
                    {
                        int count = repDocumentoTransporteNatura.ContarPorSituacao(empresa.Codigo, Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Retornado);
                        for (int i = 0; i < count; i += 10)
                        {
                            List<Dominio.Entidades.DocumentoTransporteNatura> documentosTransporteNatura = repDocumentoTransporteNatura.BuscarPorSituacao(empresa.Codigo, Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Retornado, i, 10);
                            if (documentosTransporteNatura.Count > 0)
                            {
                                foreach (Dominio.Entidades.DocumentoTransporteNatura dtNatura in documentosTransporteNatura)
                                {
                                    unitOfWork.Start();
                                    try
                                    {
                                        bool possuiTodasOcorrencias = true;
                                        foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura nfNatura in dtNatura.NotasFiscais)
                                        {
                                            if (repOcorrenciaDeCTe.NumeroOcorrenciasPorCTe(nfNatura.CTe.Codigo) <= 0)
                                            {
                                                possuiTodasOcorrencias = false;
                                                break;
                                            }
                                        }
                                        if (possuiTodasOcorrencias)
                                        {
                                            serNatura.EnviarOcorrenciasDocumentoTransporte(dtNatura.Codigo, unitOfWork);
                                        }
                                        unitOfWork.CommitChanges();



                                        if (possuiTodasOcorrencias)
                                        {
                                            string assunto = "Ocorrência da DT " + dtNatura.NumeroDT + " enviada para a Natura";
                                            System.Text.StringBuilder st = new StringBuilder();

                                            st.Append("DT Natura: " + dtNatura.NumeroDT).AppendLine();
                                            st.Append("Notas Fiscais: " + dtNatura.NumeroNotas).AppendLine().AppendLine();
                                            st.Append("A Natura não possui um retorno para as ocorrências.").AppendLine();

                                            EnviarEmail(assunto, st.ToString(), unitOfWork);

                                            Log.TratarErro("Enviou ocorrencia DT" + dtNatura.NumeroDT);
                                            System.Threading.Thread.Sleep(30000);
                                        }

                                    }
                                    catch (Exception)
                                    {
                                        unitOfWork.Rollback();
                                    }
                                }
                            }
                        }
                    }
                }


            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void gerarOcorrenciaNatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Dominio.Entidades.Empresa Empresa = repEmpresa.BuscarPorCodigo(CodigoEmpresa);
                Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
                Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscalDocumentoTransporteNatura = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);

                string pasta = @"\Enviados\Ocoren\";
                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, "Enviados", "Ocoren");

                if (TipoArmazenamento == "ftp")
                {
                    string caminhoFTP = CaminhoRaizFTP + pasta.Replace(@"\", "/");
                    string erro = "";
                    Servicos.FTP.DownloadArquivosPasta(EnderecoFTP, PortaFTP, caminhoFTP, UsuarioFTP, SenhaFTP, FTPPassivo, false, caminho, out erro, UtilizaSFTP, false, "", true);
                    if (!string.IsNullOrWhiteSpace(erro))
                    {
                        Servicos.Log.TratarErro(erro);
                        return;
                    }
                }

                IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, "*.txt", SearchOption.AllDirectories).AsParallel();

                Dominio.Entidades.LayoutEDI layoutEDI = (from obj in Empresa.LayoutsEDI where obj.Tipo == Dominio.Enumeradores.TipoLayoutEDI.OCOREN select obj).FirstOrDefault();

                List<Dominio.Entidades.DocumentoTransporteNatura> dtOcorrencias = new List<Dominio.Entidades.DocumentoTransporteNatura>();

                System.Text.StringBuilder st = new StringBuilder();

                foreach (string arquivo in arquivos)
                {
                    string fileName = Path.GetFileName(arquivo);
                    using System.IO.MemoryStream ms = new MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo));
                    ms.Position = 0;

                    StringBuilder motivo = new StringBuilder();
                    bool integrou = false;
                    foreach (string filial in FiliaisNatura)
                    {
                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigoTransportadorNatura(filial);
                        if (empresa != null)
                        {
                            Servicos.LeituraEDI leituraEDI = new LeituraEDI(empresa, layoutEDI, ms, unitOfWork);


                            List<Dominio.Entidades.OcorrenciaDeCTe> ocorrencias = leituraEDI.GerarOcorrencias();



                            if (ocorrencias.Count <= 0)
                            {
                                motivo.Append(fileName);
                                integrou = false;

                            }
                            else
                            {
                                foreach (Dominio.Entidades.OcorrenciaDeCTe ocorem in ocorrencias)
                                {
                                    Dominio.Entidades.NotaFiscalDocumentoTransporteNatura nfNatura = repNotaFiscalDocumentoTransporteNatura.BuscarPorCTe(empresa.Codigo, ocorem.CTe.Codigo);
                                    if (nfNatura != null)
                                    {
                                        if (!dtOcorrencias.Contains(nfNatura.DocumentoTransporte))
                                            dtOcorrencias.Add(nfNatura.DocumentoTransporte);

                                        integrou = true;
                                    }
                                }
                            }
                            if (integrou)
                                break;
                        }
                    }
                    if (integrou)
                        MoverParaPastaProcessados(fileName, arquivo);
                    else
                    {
                        Servicos.Log.TratarErro("Não foi possível gerar a ocorrência do Proceda Ocoren. dos arquivos " + motivo.ToString());
                        MoverParaPastaNaoProcessados(fileName, arquivo);
                    }

                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                    ms.Close();
                    ms.Dispose();

                    //se alguma dt já foi finalizada e recebeu uma nova ocorrencia deve-se mandar novamente por aqui, já que no método de enviar ocorrências envia-se apenas as ocorrencia retornadas

                    Servicos.Natura serNatura = new Natura(unitOfWork);
                    foreach (Dominio.Entidades.DocumentoTransporteNatura dtNatura in dtOcorrencias)
                    {
                        if (dtNatura.Status == Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Finalizado)
                        {
                            serNatura.EnviarOcorrenciasDocumentoTransporte(dtNatura.Codigo, unitOfWork);
                            st.Append("DT Natura: " + dtNatura.NumeroDT).AppendLine();
                            st.Append("Notas Fiscais: " + dtNatura.NumeroNotas).AppendLine().AppendLine();
                        }
                    }
                }
                unitOfWork.CommitChanges();


                if (st.Length > 0)
                {

                    string assunto = "Ocorrências Enviadas para a Natura";
                    st.Append("A Natura não possui um retorno para as ocorrências.").AppendLine().AppendLine();
                    EnviarEmail(assunto, st.ToString(), unitOfWork);
                }
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        private bool verificarSeDTJaFoiEmitidaAnteriormente(Dominio.Entidades.DocumentoTransporteNatura dtNatura, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa empresa)
        {
            Repositorio.DocumentoTransporteNatura repDocumentoTransporteNatura = new Repositorio.DocumentoTransporteNatura(unitOfWork);
            Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscalDocumentoTransporteNatura = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);
            Repositorio.DocumentosCTE repDocumentoCTe = new Repositorio.DocumentosCTE(unitOfWork);

            bool emitida = false;

            List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> notasNatura = repNotaFiscalDocumentoTransporteNatura.BuscarPorDocumentoTransporte(dtNatura.Codigo);
            foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaNatura in notasNatura)
            {
                Dominio.Entidades.DocumentosCTE documentoCTe = repDocumentoCTe.BuscarPorChaveNFe(notaNatura.Chave, empresa.Codigo);
                if (documentoCTe != null)
                {
                    Servicos.Log.TratarErro("Encontrou documento emitido anteriormente");
                    emitida = true;
                    notaNatura.CTe = documentoCTe.CTE;
                    notaNatura.ValorFrete = documentoCTe.CTE.ValorFrete;
                    notaNatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Emitido;
                    repNotaFiscalDocumentoTransporteNatura.Atualizar(notaNatura);
                }
            }
            if (emitida)
            {
                dtNatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmEmissao;
                repDocumentoTransporteNatura.Atualizar(dtNatura);
                Servicos.Log.TratarErro("Finalizou DT em emissão DT");
            }

            return emitida;
        }

        private void gerarNOTFISDocumentosTransporteNaturaPorNota()
        {

            foreach (string filial in FiliaisNatura)
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
                try
                {
                    unitOfWork.Start();
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa Empresa = repEmpresa.BuscarPorCodigo(CodigoEmpresa);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigoTransportadorNatura(filial);

                    if (empresa != null)
                    {
                        Repositorio.DocumentoTransporteNatura repDocumentoTransporteNatura = new Repositorio.DocumentoTransporteNatura(unitOfWork);

                        List<Dominio.Entidades.DocumentoTransporteNatura> documentosTransporteNatura = repDocumentoTransporteNatura.BuscarPorSituacao(empresa.Codigo, Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao, 0, 10);
                        if (documentosTransporteNatura.Count > 0)
                        {
                            Dominio.Entidades.LayoutEDI layoutEDI = (from obj in Empresa.LayoutsEDI where obj.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS select obj).FirstOrDefault();

                            foreach (Dominio.Entidades.DocumentoTransporteNatura dtNatura in documentosTransporteNatura)
                            {
                                if (!verificarSeDTJaFoiEmitidaAnteriormente(dtNatura, unitOfWork, empresa))
                                {

                                    foreach (Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notaFiscalDocumentoTransporteNatura in dtNatura.NotasFiscais)
                                    {
                                        Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unitOfWork, layoutEDI, empresa);

                                        using (System.IO.MemoryStream msNotFis = serGeracaoEDI.GerarArquivo(notaFiscalDocumentoTransporteNatura))
                                        {
                                            string pasta = Utilidades.IO.FileStorageService.Storage.Combine("Recebidos", "Notfis");
                                            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, pasta);

                                            string nomeArquivo = string.Concat("NFIS_" + notaFiscalDocumentoTransporteNatura.Destinatario.IE_RG + "_" + notaFiscalDocumentoTransporteNatura.Numero, ".txt");

                                            string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, nomeArquivo);
                                            string caminhoCompletoProcessados = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRaiz, "Processados", nomeArquivo);

                                            Utilidades.IO.FileStorageService.Storage.SaveStream(caminhoCompletoProcessados, msNotFis);

                                            msNotFis.Position = 0;

                                            if (TipoArmazenamento == "ftp")
                                            {
                                                string caminhoFTP = CaminhoRaizFTP + pasta.Replace(@"\", "/");
                                                string erro = "";

                                                Servicos.FTP.EnviarArquivo(msNotFis, nomeArquivo, EnderecoFTP, PortaFTP, caminhoFTP, UsuarioFTP, SenhaFTP, FTPPassivo, false, out erro, UtilizaSFTP);

                                                if (!string.IsNullOrWhiteSpace(erro))
                                                {
                                                    Servicos.Log.TratarErro(erro);
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                Utilidades.IO.FileStorageService.Storage.SaveStream(caminhoCompleto, msNotFis);
                                            }
                                        }
                                    }

                                    dtNatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmEmissao;

                                    repDocumentoTransporteNatura.Atualizar(dtNatura);
                                }
                            }
                        }
                    }

                    unitOfWork.CommitChanges();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                }

            }

        }

        private void gerarNOTFISDocumentosTransporteNaturaPorDT()
        {

            foreach (string filial in FiliaisNatura)
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
                try
                {
                    unitOfWork.Start();
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa Empresa = repEmpresa.BuscarPorCodigo(CodigoEmpresa);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigoTransportadorNatura(filial);
                    Servicos.Embarcador.Integracao.Natura.EDINatura serEDINatura = new Embarcador.Integracao.Natura.EDINatura();
                    if (empresa != null)
                    {
                        Repositorio.DocumentoTransporteNatura repDocumentoTransporteNatura = new Repositorio.DocumentoTransporteNatura(unitOfWork);

                        List<Dominio.Entidades.DocumentoTransporteNatura> documentosTransporteNatura = repDocumentoTransporteNatura.BuscarPorSituacao(empresa.Codigo, Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmDigitacao, 0, 10);
                        if (documentosTransporteNatura.Count > 0)
                        {
                            Dominio.Entidades.LayoutEDI layoutEDI = (from obj in Empresa.LayoutsEDI where obj.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS select obj).FirstOrDefault();

                            foreach (Dominio.Entidades.DocumentoTransporteNatura dtNatura in documentosTransporteNatura)
                            {

                                if (!verificarSeDTJaFoiEmitidaAnteriormente(dtNatura, unitOfWork, empresa))
                                {
                                    Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis NotFis = serEDINatura.ConverterDTEmNotFis(dtNatura, unitOfWork);

                                    Servicos.GeracaoEDI serGeracaoEDI = new GeracaoEDI(unitOfWork, layoutEDI, empresa);

                                    using (System.IO.MemoryStream msNotFis = serGeracaoEDI.GerarArquivoRecursivo(NotFis))
                                    {

                                        string pasta = Utilidades.IO.FileStorageService.Storage.Combine("Recebidos", "Notfis");
                                        string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, pasta);

                                        string nomeArquivo = string.Concat("NFIS_" + dtNatura.NumeroDT, ".txt");

                                        string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, nomeArquivo);
                                        string caminhoCompletoProcessados = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRaiz, "Processados", nomeArquivo);

                                        Utilidades.IO.FileStorageService.Storage.SaveStream(caminhoCompletoProcessados, msNotFis);

                                        msNotFis.Position = 0;

                                        if (TipoArmazenamento == "ftp")
                                        {
                                            string caminhoFTP = CaminhoRaizFTP + pasta.Replace(@"\", "/");
                                            string erro = "";

                                            Servicos.FTP.EnviarArquivo(msNotFis, nomeArquivo, EnderecoFTP, PortaFTP, caminhoFTP, UsuarioFTP, SenhaFTP, FTPPassivo, false, out erro, UtilizaSFTP);

                                            if (!string.IsNullOrWhiteSpace(erro))
                                            {
                                                Servicos.Log.TratarErro(erro);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            Utilidades.IO.FileStorageService.Storage.SaveStream(caminhoCompleto, msNotFis);
                                        }
                                    }

                                    dtNatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmEmissao;
                                    repDocumentoTransporteNatura.Atualizar(dtNatura);
                                }

                            }
                        }
                    }

                    unitOfWork.CommitChanges();
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                }

            }

        }

        private void envarCTesAutorizadosNatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            try
            {
                foreach (string filial in FiliaisNatura)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigoTransportadorNatura(filial);
                    if (empresa != null)
                    {
                        Repositorio.DocumentoTransporteNatura repDocumentoTransporteNatura = new Repositorio.DocumentoTransporteNatura(unitOfWork);
                        Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscalDocumentoTransporteNatura = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);
                        Servicos.Natura serNatura = new Natura(unitOfWork);
                        int count = repDocumentoTransporteNatura.ContarPorSituacao(empresa.Codigo, Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmEmissao);
                        for (int j = 0; j < count; j += 10)
                        {
                            List<Dominio.Entidades.DocumentoTransporteNatura> documentosTransporteNatura = repDocumentoTransporteNatura.BuscarPorSituacao(empresa.Codigo, Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmEmissao, j, 10);
                            if (documentosTransporteNatura.Count > 0)
                            {
                                foreach (Dominio.Entidades.DocumentoTransporteNatura dtNatura in documentosTransporteNatura)
                                {
                                    int numeroCTesNaoEmitidos = repNotaFiscalDocumentoTransporteNatura.VerificarNumeroNotasSemCTe(empresa.Codigo, dtNatura.Codigo);
                                    if (numeroCTesNaoEmitidos == 0)
                                    {
                                        int normais = (from obj in dtNatura.NotasFiscais where obj.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && obj.Status == Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Emitido select obj).Count();
                                        if (normais > 0)
                                        {
                                            ServicoNaturaNovo.ProcessaCTeNFSe.DT_EletronicDocumentResponseDados[] retorno = serNatura.EnviarRetornoDocumentoTransporte(dtNatura.Codigo, unitOfWork);
                                            if (retorno != null)
                                            {
                                                string assunto = "DT " + dtNatura.NumeroDT + " retornada para a Natura";
                                                System.Text.StringBuilder st = new StringBuilder();

                                                st.Append("DT Natura: " + dtNatura.NumeroDT).AppendLine();
                                                st.Append("Notas Fiscais: " + dtNatura.NumeroNotas).AppendLine().AppendLine();
                                                for (int i = 0; i < retorno.Length; i++)
                                                {
                                                    if (retorno[i].number != "100")
                                                        st.Append("Retorno da Natura : " + retorno[j].number + " - " + retorno[j].message).AppendLine();
                                                }

                                                EnviarEmail(assunto, st.ToString(), unitOfWork);
                                            }
                                        }
                                        int complementar = (from obj in dtNatura.NotasFiscais where obj.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento && obj.Status == Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Emitido select obj).Count();
                                        if (complementar > 0)
                                        {
                                            serNatura.EnviarRetornoDocumentoTransporteCTeComplementar(dtNatura.Codigo, unitOfWork);
                                            dtNatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Retornado;
                                            repDocumentoTransporteNatura.Atualizar(dtNatura);
                                        }
                                        Log.TratarErro("Enviou DT" + dtNatura.NumeroDT);
                                        System.Threading.Thread.Sleep(30000);

                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        private void buscarCTesCancelados()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);

            try
            {
                unitOfWork.Start();
                Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscalDocumentoTransporteNatura = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);
                Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);

                string pasta = @"\Enviados\CT-e\Cancelados\";
                string caminho = caminhoArquivos + pasta;

                if (TipoArmazenamento == "ftp")
                {
                    string caminhoFTP = CaminhoRaizFTP + pasta.Replace(@"\", "/");
                    string erro = "";
                    Servicos.FTP.DownloadArquivosPasta(EnderecoFTP, PortaFTP, caminhoFTP, UsuarioFTP, SenhaFTP, FTPPassivo, false, caminho, out erro, UtilizaSFTP, false, "", true);
                    if (!string.IsNullOrWhiteSpace(erro))
                    {
                        Servicos.Log.TratarErro(erro);
                        return;
                    }
                }

                IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, "*.xml", SearchOption.AllDirectories).AsParallel();

                List<Dominio.Entidades.DocumentoTransporteNatura> dtsNatura = new List<Dominio.Entidades.DocumentoTransporteNatura>();

                foreach (string arquivo in arquivos)
                {
                    string fileName = Path.GetFileName(arquivo);
                    bool integrouCTe = true;
                    StringBuilder motivo = new StringBuilder();
                    foreach (string filial in FiliaisNatura)
                    {
                        Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigoTransportadorNatura(filial);

                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = validarXMLCTe(arquivo, empresa, unitOfWork);
                        if (cte != null)
                        {

                            if (empresa != null)
                            {
                                List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentosCTe.BuscarPorCTe(empresa.Codigo, cte.Codigo);
                                bool erroLocal = false;
                                foreach (Dominio.Entidades.DocumentosCTE docCTe in documentosCTe)
                                {
                                    if (!erroLocal)
                                    {
                                        Dominio.Entidades.NotaFiscalDocumentoTransporteNatura nfNatura = repNotaFiscalDocumentoTransporteNatura.BuscarPorChaveNFe(empresa.Codigo, docCTe.ChaveNFE);
                                        if (nfNatura != null)
                                        {
                                            if (!dtsNatura.Contains(nfNatura.DocumentoTransporte))
                                                dtsNatura.Add(nfNatura.DocumentoTransporte);

                                            integrouCTe = true;
                                        }
                                        else
                                        {
                                            erroLocal = true;
                                            motivo.Append("Chave nfe do CT-e Cancelado não existe nas dts da natura.");
                                            integrouCTe = false;
                                        }
                                    }
                                }
                                if (integrouCTe)
                                    break;
                            }
                        }
                        else
                        {
                            integrouCTe = false;
                        }
                    }

                    if (integrouCTe)
                    {
                        unitOfWork.CommitChanges();
                        MoverParaPastaProcessados(fileName, arquivo);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro("Não foi possível buscar o CT-e Cancelado, motivo: " + motivo.ToString());
                        MoverParaPastaNaoProcessados(fileName, arquivo);

                    }
                }
                foreach (Dominio.Entidades.DocumentoTransporteNatura dtNatura in dtsNatura)
                {
                    Servicos.Natura serNatura = new Natura(unitOfWork);
                    if (dtNatura.Status == Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Retornado)
                    {
                        serNatura.EnviarRetornoDocumentoTransporte(dtNatura.Codigo, unitOfWork);
                    }
                }
            }
            catch (Exception)
            {
                unitOfWork.Rollback();
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        private void buscarCTesAutorizados()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
                Repositorio.DocumentoTransporteNatura repDocumentoTransporteNatura = new Repositorio.DocumentoTransporteNatura(unitOfWork);

                Repositorio.NotaFiscalDocumentoTransporteNatura repNotaFiscalDocumentoTransporteNatura = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);

                string pasta = @"\Enviados\CT-e\Autorizados\";
                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoArquivos, "Enviados", "CT-e", "Autorizados");

                if (TipoArmazenamento == "ftp")
                {
                    string caminhoFTP = CaminhoRaizFTP + pasta.Replace(@"\", "/");
                    string erro = "";
                    Servicos.FTP.DownloadArquivosPasta(EnderecoFTP, PortaFTP, caminhoFTP, UsuarioFTP, SenhaFTP, FTPPassivo, false, caminho, out erro, UtilizaSFTP, false, "", true);
                    if (!string.IsNullOrWhiteSpace(erro))
                    {
                        Servicos.Log.TratarErro(erro);
                        return;
                    }
                }

                IEnumerable<string> arquivos = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, "*.xml", SearchOption.AllDirectories).AsParallel();

                foreach (string arquivo in arquivos)
                {
                    string fileName = Path.GetFileName(arquivo);
                    bool integrouCTe = true;
                    bool cteValido = false;
                    string motivo = "";

                    foreach (string filial in FiliaisNatura)
                    {
                        Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigoTransportadorNatura(filial);
                        if (empresa != null)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = validarXMLCTe(arquivo, empresa, unitOfWork);

                            if (cte != null)
                            {
                                cteValido = true;
                                if (cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento)
                                {
                                    List<Dominio.Entidades.DocumentosCTE> documentosCTe = repDocumentosCTe.BuscarPorCTe(empresa.Codigo, cte.Codigo);

                                    bool erroLocal = false;
                                    foreach (Dominio.Entidades.DocumentosCTE docCTe in documentosCTe)
                                    {
                                        if (!erroLocal)
                                        {
                                            Dominio.Entidades.NotaFiscalDocumentoTransporteNatura nfNatura = repNotaFiscalDocumentoTransporteNatura.BuscarPorChaveNFe(empresa.Codigo, docCTe.ChaveNFE);
                                            if (nfNatura != null)
                                            {
                                                if (nfNatura.CTe == null)
                                                {
                                                    nfNatura.CTe = cte;
                                                    nfNatura.ValorFrete = cte.ValorFrete;
                                                    nfNatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Emitido;
                                                    repNotaFiscalDocumentoTransporteNatura.Atualizar(nfNatura);
                                                }
                                                else
                                                {
                                                    if (nfNatura.CTe.Chave != cte.Chave)//caso seja enviado mais de um CT_e por Nota
                                                    {
                                                        Dominio.Entidades.NotaFiscalDocumentoTransporteNatura cloneNFNatura = nfNatura.Clonar();
                                                        cloneNFNatura.Codigo = 0;
                                                        cloneNFNatura.CTe = cte;
                                                        cloneNFNatura.ValorFrete = cte.ValorFrete;
                                                        repNotaFiscalDocumentoTransporteNatura.Inserir(cloneNFNatura);
                                                    }
                                                }
                                                integrouCTe = true;
                                            }
                                            else
                                            {
                                                motivo += "Chave nfe não existe nas dts da natura.";
                                                integrouCTe = false;
                                                erroLocal = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Dominio.Entidades.NotaFiscalDocumentoTransporteNatura nfNatura = repNotaFiscalDocumentoTransporteNatura.BuscarPorChaveCTe(empresa.Codigo, cte.ChaveCTESubComp);
                                    if (nfNatura != null)
                                    {
                                        Dominio.Entidades.NotaFiscalDocumentoTransporteNatura cloneNFNatura = nfNatura.Clonar();
                                        cloneNFNatura.Codigo = 0;
                                        cloneNFNatura.Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Emitido;
                                        cloneNFNatura.CTe = cte;
                                        repNotaFiscalDocumentoTransporteNatura.Inserir(cloneNFNatura);
                                        nfNatura.DocumentoTransporte.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.EmEmissao;
                                        repDocumentoTransporteNatura.Atualizar(nfNatura.DocumentoTransporte);
                                        integrouCTe = true;
                                    }
                                    else
                                    {
                                        motivo += "Nâo foi localizada o CT-e para o CT-e complementar chave." + cte.ChaveCTESubComp;
                                        integrouCTe = false;
                                    }
                                }
                                if (integrouCTe)
                                    break;
                            }
                        }
                        else
                        {
                            integrouCTe = false;
                        }
                    }

                    if (integrouCTe)
                    {
                        unitOfWork.CommitChanges();
                        MoverParaPastaProcessados(fileName, arquivo);
                    }
                    else
                    {
                        if (!cteValido)
                            unitOfWork.Rollback();
                        else
                            unitOfWork.CommitChanges();
                        Servicos.Log.TratarErro("Não foi possível integrar o CT-e motivo: " + motivo);
                        MoverParaPastaNaoProcessados(fileName, arquivo);
                    }
                    Utilidades.IO.FileStorageService.Storage.Delete(fileName);
                }
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }
        private Dominio.Entidades.ConhecimentoDeTransporteEletronico validarXMLCTe(string arquivo, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            string fileName = Path.GetFileName(arquivo);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(Utilidades.IO.FileStorageService.Storage.ReadAllText(arquivo));
            using System.IO.MemoryStream ms = new MemoryStream(bytes);

            var cteLido = MultiSoftware.CTe.Servicos.Leitura.Ler(ms);
            if (cteLido.GetType() == typeof(MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc))
            {
                try
                {
                    MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado.cteProc)cteLido;
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = repCTe.BuscarPorChave(empresa.Codigo, cteProc.protCTe.infProt.chCTe);
                    if (conhecimento == null)
                    {
                        Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                        object retorno = svcCTe.GerarCTeAnterior(ms, empresa.Codigo, string.Empty, string.Empty, unitOfWork, cteProc, false, false);
                        if (retorno != null)
                        {
                            if (retorno.GetType() == typeof(string))
                            {
                                Servicos.Log.TratarErro((string)retorno);
                            }
                            else if (retorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                            {
                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                                cte.ValorFrete = cte.ValorAReceber - cte.ValorICMS;
                                repCTe.Atualizar(cte);
                                return cte;
                            }
                            else
                            {
                                Servicos.Log.TratarErro("Conhecimento de transporte inválido.");
                                MoverParaPastaNaoProcessados(fileName, arquivo);
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("O arquivo enviado não é um CT-e válido, por favor verifique");
                            MoverParaPastaNaoProcessados(fileName, arquivo);
                        }
                    }
                    else
                    {
                        conhecimento.ValorFrete = conhecimento.ValorAReceber - conhecimento.ValorICMS;
                        return conhecimento;
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    MoverParaPastaNaoProcessados(fileName, arquivo);
                }
            }
            else if (cteLido.GetType() == typeof(MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc))
            {
                try
                {
                    MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado.cteProc)cteLido;
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = repCTe.BuscarPorChave(empresa.Codigo, cteProc.protCTe.infProt.chCTe);

                    if (conhecimento == null)
                    {
                        Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                        object retorno = svcCTe.GerarCTeAnterior(ms, empresa.Codigo, string.Empty, string.Empty, unitOfWork, cteProc, false, false);
                        if (retorno != null)
                        {
                            if (retorno.GetType() == typeof(string))
                            {
                                Servicos.Log.TratarErro((string)retorno);
                            }
                            else if (retorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                            {
                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                                cte.ValorFrete = cte.ValorAReceber - cte.ValorICMS;
                                repCTe.Atualizar(cte);
                                return cte;
                            }
                            else
                            {
                                Servicos.Log.TratarErro("Conhecimento de transporte inválido.");
                                MoverParaPastaNaoProcessados(fileName, arquivo);
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("O arquivo enviado não é um CT-e válido, por favor verifique");
                            MoverParaPastaNaoProcessados(fileName, arquivo);
                        }
                    }
                    else
                    {
                        conhecimento.ValorFrete = conhecimento.ValorAReceber - conhecimento.ValorICMS;
                        return conhecimento;
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    MoverParaPastaNaoProcessados(fileName, arquivo);
                }
            }
            else if (cteLido.GetType() == typeof(MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc))
            {
                try
                {
                    MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc cteProc = (MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado.cteProc)cteLido;
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento = repCTe.BuscarPorChave(empresa.Codigo, cteProc.protCTe.infProt.chCTe);

                    if (conhecimento == null)
                    {
                        Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                        object retorno = svcCTe.GerarCTeAnterior(ms, empresa.Codigo, string.Empty, string.Empty, unitOfWork, cteProc, false, false);
                        if (retorno != null)
                        {
                            if (retorno.GetType() == typeof(string))
                            {
                                Servicos.Log.TratarErro((string)retorno);
                            }
                            else if (retorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                            {
                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                                cte.ValorFrete = cte.ValorAReceber - cte.ValorICMS;
                                repCTe.Atualizar(cte);
                                return cte;
                            }
                            else
                            {
                                Servicos.Log.TratarErro("Conhecimento de transporte inválido.");
                                MoverParaPastaNaoProcessados(fileName, arquivo);
                            }
                        }
                        else
                        {
                            Servicos.Log.TratarErro("O arquivo enviado não é um CT-e válido, por favor verifique");
                            MoverParaPastaNaoProcessados(fileName, arquivo);
                        }
                    }
                    else
                    {
                        conhecimento.ValorFrete = conhecimento.ValorAReceber - conhecimento.ValorICMS;
                        return conhecimento;
                    }
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    MoverParaPastaNaoProcessados(fileName, arquivo);
                }
            }

            Servicos.Log.TratarErro("O arquivo enviado não é um CT-e válido, por favor verifique");
            MoverParaPastaNaoProcessados(fileName, arquivo);

            ms.Close();
            ms.Dispose();
            return null;
        }

        private void MoverParaPastaNaoProcessados(string nomeArquivo, string fullName)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(Utilidades.IO.FileStorageService.Storage.ReadAllText(fullName));

            string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRaiz, "NaoProcessados", nomeArquivo);

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoCompleto, bytes);
        }

        private void MoverParaPastaProcessados(string nomeArquivo, string fullName)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(Utilidades.IO.FileStorageService.Storage.ReadAllText(fullName));

            string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRaiz, "Processados", nomeArquivo);

            Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoCompleto, bytes);
        }

        private void buscarDocumentosTransportesNatura()
        {

            if (ultimaConsultaDT == DateTime.MinValue || ultimaConsultaDT.AddMinutes(minutosParaConsulta) <= DateTime.Now)
            {
                foreach (string filial in FiliaisNatura)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(StringConexao);
                    try
                    {
                        unitOfWork.Start();
                        Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                        Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigoTransportadorNatura(filial);
                        if (empresa != null)
                        {
                            Servicos.Natura serNatura = new Natura(unitOfWork);
                            serNatura.ConsultarDocumentosTransporte(empresa.Codigo, 0, DateTime.Now.AddDays(-3), DateTime.Now, unitOfWork, 0, true);
                            ultimaConsultaDT = DateTime.Now;
                            unitOfWork.CommitChanges();
                        }
                        else
                        {
                            unitOfWork.Rollback();
                        }
                    }
                    catch (Exception)
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                    }
                }
            }
        }
    }
}
