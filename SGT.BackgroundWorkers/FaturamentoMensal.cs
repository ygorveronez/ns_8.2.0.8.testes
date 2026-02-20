using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao;
using SGT.BackgroundWorkers.Utils;
using System.Threading.Tasks;
using System.Threading;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]

    public class FaturamentoMensal : LongRunningProcessBase<FaturamentoMensal>
    {
        #region Métodos Protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            ProcessarAutorizacoesPendentes(unitOfWork, unitOfWorkAdmin, _codigoEmpresa, _stringConexao, _tipoServicoMultisoftware, _stringConexaoAdmin, _clienteMultisoftware.Codigo);
            ProcessarEnvioEmailPendentes(unitOfWork, unitOfWorkAdmin, _codigoEmpresa, _stringConexao, _tipoServicoMultisoftware, _stringConexaoAdmin, _clienteMultisoftware.Codigo);
        }

        #endregion

        #region Métodos Privados

        private void ProcessarAutorizacoesPendentes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, int codigoEmpresa, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexaoAdmin, int clienteCodigo)
        {
            try
            {

                AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(adminUnitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(clienteCodigo);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Servicos.NFSe servicoNFSe = new Servicos.NFSe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, cliente, tipoServicoMultisoftware, stringConexaoAdmin);

                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal> faturamentos = repFaturamentoMensal.BuscarPorStatus(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.AguardandoAutorizacaoDocumento);
                foreach (var fat in faturamentos)
                {
                    unitOfWork.Start();
                    fat.StatusFaturamentoMensal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.EmGeracaoAutorizacaoDocumento;
                    repFaturamentoMensal.Atualizar(fat);
                    unitOfWork.CommitChanges();
                }

                string urlBase = "";
                if (cliente != null && cliente.ClienteURLsAcesso != null && cliente.ClienteURLsAcesso.Count > 0)
                    urlBase = cliente.ClienteURLsAcesso.FirstOrDefault()?.URLAcesso;

                for (int i = 0; i < faturamentos.Count(); i++)
                {
                    unitOfWork.Start();

                    List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico> listaFaturamentos = repFaturamentoMensalClienteServico.BuscarPorFaturamento(faturamentos[i].Codigo);
                    serNotificacao.InfomarPercentualProcessamento(faturamentos[i].Usuario, faturamentos[i].Codigo, "FaturamentosMensais/FaturamentoMensal", 5, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.faturamentoMensalDocumentos, tipoServicoMultisoftware, unitOfWork);

                    string retorno = "";
                    bool temNotaServico = false;
                    int indiceLinha = 1;
                    int totalLinhas = listaFaturamentos.Count() + 1;
                    //Servicos.Log.TratarErro("0 - " + faturamentos[i].Codigo.ToString());
                    //Servicos.Log.TratarErro("1 - " + totalLinhas.ToString());
                    for (int a = 0; a < listaFaturamentos.Count(); a++)
                    {
                        if (listaFaturamentos[a].NotaFiscal != null)
                        {
                            //Servicos.Log.TratarErro("NOTA ELETRONICA");
                            if (listaFaturamentos[a].NotaFiscal.Status != Dominio.Enumeradores.StatusNFe.Autorizado)
                                retorno += " " + EmitirNFe(listaFaturamentos[a].NotaFiscal.Codigo, unitOfWork, "", Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), faturamentos[i].Usuario, tipoServicoMultisoftware, urlBase);
                        }
                        else if (listaFaturamentos[a].NotaFiscalServico != null)
                        {
                            // Servicos.Log.TratarErro("NOTA SERVIÇO");
                            temNotaServico = true;
                            if (listaFaturamentos[a].NotaFiscalServico.Status == "S" || listaFaturamentos[a].NotaFiscalServico.Status == "R") //Em digitação ou rejeitado
                            {
                                // Servicos.Log.TratarErro("NOTA SERVIÇO 2");
                                bool sucesso = servicoNFSe.EmitirNFSe(listaFaturamentos[a].NotaFiscalServico.Codigo, unitOfWork);
                                if (!sucesso)
                                {
                                    //Servicos.Log.TratarErro("ERRO");
                                    Dominio.Entidades.ConhecimentoDeTransporteEletronico nfseRejeicao = repCTe.BuscarPorCodigo(listaFaturamentos[a].NotaFiscalServico.Codigo);
                                    retorno += " " + nfseRejeicao.MensagemRetornoSefaz;
                                }
                            }
                            // else
                            //     Servicos.Log.TratarErro("STATUS " + listaFaturamentos[a].NotaFiscalServico.Status);
                        }

                        indiceLinha++;
                        int processados = (int)(100 * indiceLinha) / totalLinhas;
                        if (processados > 100)
                            processados = 100;
                        serNotificacao.InfomarPercentualProcessamento(faturamentos[i].Usuario, faturamentos[i].Codigo, "FaturamentosMensais/FaturamentoMensal", (decimal)processados, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.faturamentoMensalDocumentos, tipoServicoMultisoftware, unitOfWork);
                    }

                    if (retorno.Trim().TrimEnd().TrimStart() != "")
                    {
                        faturamentos[i].StatusFaturamentoMensal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.GeradoDocumentos;
                        string retornoNotificacao = string.Format(Localization.Resources.Threads.FaturamentoMensal.OcorreuFalhaAutorizarDocumentos, retorno);
                        if (retornoNotificacao.Length > 149)
                            retornoNotificacao = retornoNotificacao.Substring(0, 149);
                        serNotificacao.GerarNotificacao(faturamentos[i].Usuario, faturamentos[i].Codigo, "FaturamentosMensais/FaturamentoMensal", retornoNotificacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.faturamentoMensalDocumentos, tipoServicoMultisoftware, unitOfWork);
                    }
                    else if (temNotaServico)
                    {
                        faturamentos[i].StatusFaturamentoMensal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.DocumentosAutorizados;
                        serNotificacao.GerarNotificacao(faturamentos[i].Usuario, faturamentos[i].Codigo, "FaturamentosMensais/FaturamentoMensal", Localization.Resources.Threads.FaturamentoMensal.PrefeituraProcessandoNFSes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.faturamentoMensalDocumentos, tipoServicoMultisoftware, unitOfWork);
                    }
                    else
                    {
                        faturamentos[i].StatusFaturamentoMensal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.DocumentosAutorizados;
                        serNotificacao.GerarNotificacao(faturamentos[i].Usuario, faturamentos[i].Codigo, "FaturamentosMensais/FaturamentoMensal", Localization.Resources.Threads.FaturamentoMensal.ProcessoAutorizacaoDocumentosConcluido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.sucesso, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.faturamentoMensalDocumentos, tipoServicoMultisoftware, unitOfWork);
                    }

                    repFaturamentoMensal.Atualizar(faturamentos[i]);

                    unitOfWork.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ProcessarEnvioEmailPendentes(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, int codigoEmpresa, string stringConexao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexaoAdmin, int clienteCodigo)
        {
            try
            {
                AdminMultisoftware.Repositorio.Pessoas.Cliente repCliente = new AdminMultisoftware.Repositorio.Pessoas.Cliente(adminUnitOfWork);
                AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente = repCliente.BuscarPorCodigo(clienteCodigo);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Pessoas.ClienteOutroEmail repClienteOutroEmail = new Repositorio.Embarcador.Pessoas.ClienteOutroEmail(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);

                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, cliente, tipoServicoMultisoftware, stringConexaoAdmin);

                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal> faturamentos = repFaturamentoMensal.BuscarPorStatus(Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.AguardandoEnvioEmail);
                foreach (var fat in faturamentos)
                {
                    unitOfWork.Start();
                    fat.StatusFaturamentoMensal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.EmGeracaoEnvioEmail;
                    repFaturamentoMensal.Atualizar(fat);
                    unitOfWork.CommitChanges();
                }
                for (int i = 0; i < faturamentos.Count(); i++)
                {
                    unitOfWork.Start();

                    serNotificacao.InfomarPercentualProcessamento(faturamentos[i].Usuario, faturamentos[i].Codigo, "FaturamentosMensais/FaturamentoMensal", 5, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.faturamentoMensalEmail, tipoServicoMultisoftware, unitOfWork);

                    string caminhoRelatorio = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();

                    string assunto = "NF-e " + faturamentos[i].Empresa.NomeFantasia;
                    string mensagemEmail = "";
                    string mensagemErro = "Erro ao enviar e-mail";
                    string retornoEnvioEmail = "";

                    List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico> listaTitulos = repFaturamentoMensalClienteServico.BuscarPorFaturamento(faturamentos[i].Codigo);
                    if (listaTitulos.Count() > 0)
                    {
                        int indiceLinha = 1;
                        int totalLinhas = listaTitulos.Count() + 1;
                        for (int a = 0; a < listaTitulos.Count; a++)
                        {
                            Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico fatCliente = repFaturamentoMensalClienteServico.BuscarPorCodigo(listaTitulos[a].Codigo);
                            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(fatCliente.FaturamentoMensal.Empresa.Codigo);
                            mensagemEmail = "<br/>";
                            mensagemEmail += "----------------------------------------------------<br/>";
                            mensagemEmail += "Remetente:<br/>";
                            mensagemEmail += "Empresa: " + faturamentos[i].Empresa.NomeFantasia + "<br/>";
                            mensagemEmail += "<br/>";
                            mensagemEmail += "Destinatário:<br/>";
                            mensagemEmail += "Razão Social: " + fatCliente.FaturamentoMensalCliente.Pessoa.Nome + "<br/>";
                            mensagemEmail += "CNPJ: " + fatCliente.FaturamentoMensalCliente.Pessoa.CPF_CNPJ_Formatado + "<br/><br/>";
                            mensagemEmail += "O arquivo da Nota Fiscal eltronica esta em anexo a este e-mail<br/>";
                            mensagemEmail += "E-mail enviado automaticamente. Por favor, não responda.<br/>";
                            mensagemEmail += "Se voce recebeu este e-mail por engano, por favor entre em contato atraves do endereco remetente<br/>";
                            mensagemEmail += "----------------------------------------------------<br/>";
                            if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                                mensagemEmail += "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");

                            List<string> emails = new List<string>();
                            if (!string.IsNullOrWhiteSpace(fatCliente.FaturamentoMensalCliente.Pessoa.Email))
                            {
                                if (fatCliente.FaturamentoMensalCliente.Pessoa.Email.Contains(";"))
                                {
                                    string[] emailsSeparados = fatCliente.FaturamentoMensalCliente.Pessoa.Email.Split(';');
                                    for (int k = 0; k < emailsSeparados.Count(); k++)
                                        emails.Add(emailsSeparados[k]);
                                }
                                else
                                    emails.Add(fatCliente.FaturamentoMensalCliente.Pessoa.Email);
                            }

                            List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail> listaEmails = repClienteOutroEmail.BuscarPorCNPJCPFCliente(fatCliente.FaturamentoMensalCliente.Pessoa.CPF_CNPJ);

                            for (int j = 0; j < listaEmails.Count; j++)
                            {
                                if (!string.IsNullOrWhiteSpace(listaEmails[j].Email) && listaEmails[j].EmailStatus == "A")
                                {
                                    if (listaEmails[j].Email.Contains(";"))
                                    {
                                        string[] emailsSeparados = listaEmails[j].Email.Split(';');
                                        for (int k = 0; k < emailsSeparados.Count(); k++)
                                            emails.Add(emailsSeparados[k]);
                                    }
                                    else
                                        emails.Add(listaEmails[j].Email);
                                }
                            }

                            var emailsEmpresa = fatCliente.FaturamentoMensal.Empresa.Email;
                            if (!string.IsNullOrWhiteSpace(emailsEmpresa) && fatCliente.FaturamentoMensal.Empresa.StatusEmail == "A")
                            {
                                if (emailsEmpresa.Contains(";"))
                                {
                                    string[] emailsSeparados = emailsEmpresa.Split(';');
                                    for (int k = 0; k < emailsSeparados.Count(); k++)
                                        emails.Add(emailsSeparados[k]);
                                }
                                else
                                    emails.Add(emailsEmpresa);
                            }

                            var emailsAdmEmpresa = fatCliente.FaturamentoMensal.Empresa.EmailAdministrativo;
                            if (!string.IsNullOrWhiteSpace(emailsAdmEmpresa) && fatCliente.FaturamentoMensal.Empresa.StatusEmailAdministrativo == "A")
                            {
                                if (emailsAdmEmpresa.Contains(";"))
                                {
                                    string[] emailsSeparados = emailsAdmEmpresa.Split(';');
                                    for (int k = 0; k < emailsSeparados.Count(); k++)
                                        emails.Add(emailsSeparados[k]);
                                }
                                else
                                    emails.Add(emailsAdmEmpresa);
                            }

                            emails = emails.Distinct().ToList();
                            if (emails.Count > 0)
                            {
                                mensagemEmail += "<br/> E-mail enviado para: " + string.Join("; ", emails);
                                var listaAnexo = new List<System.Net.Mail.Attachment>();
                                byte[] pdfBoleto = null;
                                byte[] pdfNotaFiscal = null;
                                byte[] xmlNotaFiscal = null;

                                if (fatCliente.Titulo != null && !string.IsNullOrWhiteSpace(fatCliente.Titulo.CaminhoBoleto) && Utilidades.IO.FileStorageService.Storage.Exists(fatCliente.Titulo.CaminhoBoleto))
                                {
                                    pdfBoleto = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fatCliente.Titulo.CaminhoBoleto);
                                    listaAnexo.Add(new System.Net.Mail.Attachment(new System.IO.MemoryStream(pdfBoleto), System.IO.Path.GetFileName(fatCliente.Titulo.CaminhoBoleto), "application/pdf"));
                                }

                                if (fatCliente.NotaFiscal != null)
                                {
                                    if (fatCliente.NotaFiscal.Status == Dominio.Enumeradores.StatusNFe.Autorizado)
                                    {
                                        string caminhoXML = RetornaCaminhoXMLNotaFiscal(unitOfWork, fatCliente.NotaFiscal);
                                        string caminhoPDF = RetornaCaminhoDANFENotaFiscal(unitOfWork, fatCliente.NotaFiscal, caminhoRelatorio, faturamentos[i].Usuario, stringConexao);
                                        if (!string.IsNullOrWhiteSpace(caminhoXML) && Utilidades.IO.FileStorageService.Storage.Exists(caminhoXML))
                                        {
                                            xmlNotaFiscal = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoXML);
                                            listaAnexo.Add(new System.Net.Mail.Attachment(new System.IO.MemoryStream(xmlNotaFiscal), System.IO.Path.GetFileName(caminhoXML), "application/xml"));
                                        }
                                        if (!string.IsNullOrWhiteSpace(caminhoPDF) && Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                                        {
                                            pdfNotaFiscal = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                                            listaAnexo.Add(new System.Net.Mail.Attachment(new System.IO.MemoryStream(pdfNotaFiscal), System.IO.Path.GetFileName(caminhoPDF), "application/pdf"));
                                        }

                                        unitOfWork.Dispose();
                                        unitOfWork = new Repositorio.UnitOfWork(stringConexao);

                                        repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                                        repFaturamentoMensalClienteServico = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalClienteServico(unitOfWork);
                                        repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                                        serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(stringConexao, cliente, tipoServicoMultisoftware, stringConexaoAdmin);
                                        repFaturamentoMensal = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensal(unitOfWork);
                                        repClienteOutroEmail = new Repositorio.Embarcador.Pessoas.ClienteOutroEmail(unitOfWork);

                                        unitOfWork.Start();
                                    }
                                }

                                bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail,
                                    listaAnexo.Count > 0 ? listaAnexo : null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, fatCliente.FaturamentoMensal.Empresa.Codigo);
                                if (!sucesso)
                                    retornoEnvioEmail += " -> " + fatCliente.FaturamentoMensalCliente.Pessoa.Nome + " " + mensagemErro;
                            }

                            indiceLinha++;
                            int processados = (int)(100 * indiceLinha) / totalLinhas;
                            if (processados > 100)
                                processados = 100;
                            serNotificacao.InfomarPercentualProcessamento(faturamentos[i].Usuario, faturamentos[i].Codigo, "FaturamentosMensais/FaturamentoMensal", (decimal)processados, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.faturamentoMensalEmail, tipoServicoMultisoftware, unitOfWork);
                        }

                        if (string.IsNullOrWhiteSpace(retornoEnvioEmail))
                            serNotificacao.GerarNotificacao(faturamentos[i].Usuario, faturamentos[i].Codigo, "FaturamentosMensais/FaturamentoMensal", Localization.Resources.Threads.FaturamentoMensal.ProcessoEnvioEmailConcluido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.sucesso, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.faturamentoMensalEmail, tipoServicoMultisoftware, unitOfWork);
                        else
                        {
                            if (retornoEnvioEmail.Length > 100)
                                retornoEnvioEmail = retornoEnvioEmail.Substring(0, 99);
                            serNotificacao.GerarNotificacao(faturamentos[i].Usuario, faturamentos[i].Codigo, "FaturamentosMensais/FaturamentoMensal", string.Format(Localization.Resources.Threads.FaturamentoMensal.FalhaEnvioEmail, retornoEnvioEmail), Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.falha, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.faturamentoMensalEmail, tipoServicoMultisoftware, unitOfWork);
                        }
                    }

                    //alterar status para 2
                    Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao> relatoriosEmExecucao = repRelatorioControleGeracao.BuscarRelatoriosEmExecucao(faturamentos[i].Usuario.Codigo);
                    foreach (Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioEmExecucao in relatoriosEmExecucao)
                    {
                        relatorioEmExecucao.SituacaoGeracaoRelatorio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGeracaoRelatorio.Gerado;
                        repRelatorioControleGeracao.Atualizar(relatorioEmExecucao);
                    }

                    Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensal fatAlterar = repFaturamentoMensal.BuscarPorCodigo(faturamentos[i].Codigo);
                    fatAlterar.StatusFaturamentoMensal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusFaturamentoMensal.GeradoBoletos;
                    repFaturamentoMensal.Atualizar(fatAlterar);

                    unitOfWork.CommitChanges();
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
            }
        }

        private string EmitirNFe(int codigoNFe, Repositorio.UnitOfWork unitOfWork, string relatorio, string caminhoRelatoriosEmbarcador, Dominio.Entidades.Usuario usuario, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string urlBase)
        {
            string mensagem = "";

            if (usuario.Empresa.DataFinalCertificado != null && usuario.Empresa.DataFinalCertificado > DateTime.MinValue)
            {
                if (usuario.Empresa.DataFinalCertificado < DateTime.Now)
                {
                    return "O certificado digital cadastrado na empresa se encontra vencido.";
                }
            }

            Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos repNotaFiscalProdutos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos(unitOfWork);
            Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);
            List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos> itens = repNotaFiscalProdutos.BuscarPorNota(codigoNFe);

            if (nfe == null)
                mensagem = "O NF-e informado não foi localizado";
            else if (nfe != null && (itens == null || itens.Count == 0))
                mensagem = "A NF-e não possui nenhum item lançado.";
            else
            {
                Zeus.Embarcador.ZeusNFe.Zeus z = new Zeus.Embarcador.ZeusNFe.Zeus();
                mensagem = z.CriarEnviarNFe(codigoNFe, unitOfWork, tipoServicoMultisoftware, relatorio, caminhoRelatoriosEmbarcador, usuario, "55", 1, false, false, null, urlBase);
            }

            return mensagem;
        }

        private string RetornaCaminhoDANFENotaFiscal(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, string caminhoRelatoriosEmbarcador, Dominio.Entidades.Usuario usuario, string stringConexao)
        {
            string novoCaminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatoriosEmbarcador, notaFiscal.Chave + ".pdf");
            if (Utilidades.IO.FileStorageService.Storage.Exists(novoCaminho))
            {
                unidadeDeTrabalho.CommitChanges();
                return novoCaminho;
            }

            Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = repRelatorio.BuscarPorTitulo("DANFE");
            if (relatorioOrigem != null)
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unidadeDeTrabalho);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = new Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio();
                dynRelatorio.Codigo = relatorioOrigem.Codigo;
                dynRelatorio.CodigoControleRelatorios = relatorioOrigem.CodigoControleRelatorios;
                dynRelatorio.Titulo = relatorioOrigem.Titulo;
                dynRelatorio.Descricao = relatorioOrigem.Descricao;
                dynRelatorio.Padrao = relatorioOrigem.Padrao;
                dynRelatorio.ExibirSumarios = relatorioOrigem.ExibirSumarios;
                dynRelatorio.CortarLinhas = relatorioOrigem.CortarLinhas;
                dynRelatorio.FundoListrado = relatorioOrigem.FundoListrado;
                dynRelatorio.TamanhoPadraoFonte = relatorioOrigem.TamanhoPadraoFonte;
                dynRelatorio.FontePadrao = relatorioOrigem.FontePadrao;
                dynRelatorio.AgruparRelatorio = false;
                dynRelatorio.PropriedadeAgrupa = relatorioOrigem.PropriedadeAgrupa;
                dynRelatorio.OrdemAgrupamento = relatorioOrigem.OrdemAgrupamento;
                dynRelatorio.PropriedadeOrdena = relatorioOrigem.PropriedadeOrdena;
                dynRelatorio.OrdemOrdenacao = relatorioOrigem.OrdemOrdenacao;
                dynRelatorio.TipoArquivoRelatorio = Dominio.Enumeradores.TipoArquivoRelatorio.PDF;
                dynRelatorio.OrientacaoRelatorio = relatorioOrigem.OrientacaoRelatorio;
                dynRelatorio.Grid = "{\"draw\":0,\"inicio\":0,\"limite\":0,\"indiceColunaOrdena\":0,\"dirOrdena\":\"desc\",\"recordsTotal\":0,\"recordsFiltered\":0,\"group\":{\"enable\":false,\"propAgrupa\":null,\"dirOrdena\":null},\"header\":[{\"title\":\"Cód. Produto\",\"data\":\"CodigoProduto\",\"width\":\"10%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":0,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Descrição dos Produtos/Serviços\",\"data\":\"DescricaoItem\",\"width\":\"25%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-left\",\"tabletHide\":false,\"phoneHide\":false,\"position\":1,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"NCM\",\"data\":\"CodigoNCMItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":2,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"CST/CSOSN\",\"data\":\"DescricaoCSTItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":3,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"CFOP\",\"data\":\"CodigoCFOPItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":4,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Unid.\",\"data\":\"DescricaoUnidadeItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":5,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Quantidade\",\"data\":\"QuantidadeItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":6,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"V. Unitário\",\"data\":\"ValorUnitarioItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":7,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"V. Total\",\"data\":\"ValorTotalItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":8,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"B.C. ICMS\",\"data\":\"BCICMSItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":9,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Valor ICMS\",\"data\":\"ValorICMSItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":10,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"Valor IPI\",\"data\":\"ValorIPIItem\",\"width\":\"8%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-right\",\"tabletHide\":false,\"phoneHide\":false,\"position\":11,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"% ICMS\",\"data\":\"AliquotaICMSItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":12,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0},{\"title\":\"% IPI\",\"data\":\"AliquotaIPIItem\",\"width\":\"5%\",\"orderable\":false,\"visible\":true,\"className\":\"text-align-center\",\"tabletHide\":false,\"phoneHide\":false,\"position\":13,\"sumary\":0,\"enableGroup\":false,\"dynamicCode\":0}],\"data\":null,\"dataSumarizada\":null,\"order\":[{\"column\":0,\"dir\":\"desc\"}]}";
                dynRelatorio.Report = relatorioOrigem.Codigo;
                dynRelatorio.NovoRelatorio = true;

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorioOrigem, usuario, dynRelatorio.TipoArquivoRelatorio, unidadeDeTrabalho); ;

                unidadeDeTrabalho.CommitChanges();

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = null;

                relatorioTemp.PropriedadeOrdena = propOrdena;

                Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unidadeDeTrabalho);
                serNotaFiscalEletronica.GerarRelatorioDANFE(notaFiscal, agrupamentos, relatorioControleGeracao, relatorioTemp, stringConexao);

                string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatoriosEmbarcador, relatorioControleGeracao.GuidArquivo) + ".pdf";
                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoArquivo))
                {
                    if (Utilidades.IO.FileStorageService.Storage.Exists(novoCaminho))
                        Utilidades.IO.FileStorageService.Storage.Delete(novoCaminho);
                    Utilidades.IO.FileStorageService.Storage.Copy(caminhoArquivo, novoCaminho);
                    if (Utilidades.IO.FileStorageService.Storage.Exists(novoCaminho))
                        return novoCaminho;
                    else
                        return "";
                }
                else
                    return "";
            }
            else
            {
                return "";
            }

        }

        private string RetornaCaminhoXMLNotaFiscal(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal)
        {
            string caminhoXML = Servicos.FS.GetPath("C:\\XML NF-e\\") + notaFiscal.Chave + ".xml";
            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoXML))
            {
                Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos repNotaFiscalArquivos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalArquivos(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalArquivos arquivo = repNotaFiscalArquivos.BuscarPorNota(notaFiscal.Codigo);
                if (arquivo != null && !string.IsNullOrWhiteSpace(arquivo.XMLDistribuicao))
                {
                    using (Stream saida = Utilidades.IO.FileStorageService.Storage.OpenWrite(caminhoXML))
                    using (StreamWriter escritor = new StreamWriter(saida))
                        escritor.WriteLine(arquivo.XMLDistribuicao);
                }
            }
            return caminhoXML;
        }

        #endregion
    }
}