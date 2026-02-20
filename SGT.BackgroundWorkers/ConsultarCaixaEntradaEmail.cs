using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos.Embarcador.Pedido;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]

    public class ConsultarCaixaEntradaEmail : LongRunningProcessBase<ConsultarCaixaEntradaEmail>
    {
        int _lerEmail = 0;

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            if (_lerEmail > 5)
            {
                await VerificarCaixaDeEntradaAsync(unitOfWork, cancellationToken);
                await VerificarEmailsAsync(unitOfWork, cancellationToken);
                await VerificarFTPAsync(unitOfWork, cancellationToken);
                await VerificarFTPTMSAsync(unitOfWork, unitOfWorkAdmin, cancellationToken);
                VerificarIntegracoesClienteParcial(unitOfWork, cancellationToken);

                _lerEmail = 0;
            }
            else
                _lerEmail++;

            VerificarIntegracoes(unitOfWork, cancellationToken);
        }

        #region Métodos Privados

        private async Task VerificarCaixaDeEntradaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork, cancellationToken);
            Repositorio.ConfiguracaoEmissaoEmail repositorioConfiguracaoEmissaoEmail = new Repositorio.ConfiguracaoEmissaoEmail(unitOfWork, cancellationToken);

            List<Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte> emails = await repositorioConfigEmailDocTransporte.BuscarEmailLerDocumentosAsync();

            foreach (Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configEmail in emails)
            {
                Dominio.Entidades.ConfiguracaoEmissaoEmail configuracaoEmissaoEmail = await repositorioConfiguracaoEmissaoEmail.BuscarPorEmailAsync(configEmail.Codigo);

                if (configuracaoEmissaoEmail == null)
                {
                    Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                    serEmail.ReceberEmail(configEmail, _tipoServicoMultisoftware, configEmail.Email, configEmail.Senha, configEmail.Pop3, configEmail.RequerAutenticacaoPop3, configEmail.PortaPop3, unitOfWork);
                }
            }
        }

        private async Task VerificarEmailsAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.LeituraEmail,
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema
            };

            Repositorio.Embarcador.Email.EmailCaixaEntrada repositorioEmailCaixaEntrada = new Repositorio.Embarcador.Email.EmailCaixaEntrada(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado repositorioConfigDocumentoDestinado = new Repositorio.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado(unitOfWork, cancellationToken);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repositorioConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentoDestinado configuracaoDocumentoDestinado = await repositorioConfigDocumentoDestinado.BuscarPrimeiroRegistroAsync();
            Servicos.Embarcador.CTe.CTEsImportados servicoCTeImportado = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork, _tipoServicoMultisoftware, cancellationToken);
            Servicos.Embarcador.NFe.NFe servicoNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork, cancellationToken);

            List<Dominio.Entidades.Embarcador.Email.EmailCaixaEntrada> emails = await repositorioEmailCaixaEntrada.BuscarPorTipoServicoAsync(_tipoServicoMultisoftware, 0, 100, cancellationToken);

            foreach (Dominio.Entidades.Embarcador.Email.EmailCaixaEntrada email in emails)
            {
                if (email.Anexos != null && email.Anexos.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Email.EmailAnexos anexo in email.Anexos)
                    {
                        string extensao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                        string caminho = System.IO.Path.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos, "Entrada");
                        string fileLocation = System.IO.Path.Combine(caminho, anexo.GuidNomeArquivo + extensao);

                        if (System.IO.File.Exists(fileLocation))
                        {
                            if (anexo.ArquivoZipado)
                            {
                                await enviarEmailProblemaCTeAsync(email.Remetente, "O arquivo (Enviado por anexo com o nome " + anexo.NomeArquivo + ") está compactado, por favor envie os arquivos descompactados ou compactados na extensão .zip .", unitOfWork, cancellationToken);
                            }
                            else
                            {
                                if (extensao == ".xml")
                                {
                                    using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(File.ReadAllBytes(fileLocation)))
                                    {
                                        try
                                        {
                                            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;
                                            try
                                            {
                                                xmlNotaFiscal = await ProcessarXMLNFeAsync(memoryStream, email.Remetente, email.ConfigEmail?.Email ?? string.Empty, unitOfWork, cancellationToken, false, true);
                                            }
                                            catch (ServicoException ex)
                                            {
                                                Servicos.Log.TratarErro("assunto email " + email.Assunto, "XMLEmail");
                                                Servicos.Log.TratarErro("erro: " + ex.Message, "XMLEmail");
                                            }
                                            catch (Exception ex)
                                            {
                                                Servicos.Log.TratarErro("assunto email " + email.Assunto, "XMLEmail");
                                                Servicos.Log.TratarErro("erro: " + ex.Message, "XMLEmail");
                                            }

                                            if (xmlNotaFiscal == null)
                                            {
                                                if (!Servicos.Embarcador.CTe.CTe.ProcessarXMLCTe(memoryStream, unitOfWork, unitOfWork.StringConexao, _tipoServicoMultisoftware, anexo.NomeArquivo, false, false, auditado))
                                                {
                                                    if (!await servicoCTeImportado.ProcessarXMLMDFeAsync(memoryStream, auditado))
                                                    {
                                                        var objIntercement = Servicos.Embarcador.Integracao.Intercement.IntegracaoIntercement.Ler(memoryStream);

                                                        if (objIntercement != null)
                                                        {
                                                            string placaVeiculo = (string)objIntercement.item.SIGNI;

                                                            if (!string.IsNullOrWhiteSpace(placaVeiculo))
                                                            {
                                                                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorVeiculoTipoIntegracao(placaVeiculo, SituacaoCarga.AgNFe, TipoIntegracao.Intercement);
                                                                if (cargaPedido != null)

                                                                {
                                                                    string retorno = Servicos.Embarcador.Integracao.Intercement.IntegracaoIntercement.ProcessarXMLIntercement(objIntercement, cargaPedido, unitOfWork, auditado, configuracao, unitOfWork.StringConexao, _tipoServicoMultisoftware);

                                                                    if (!string.IsNullOrWhiteSpace(retorno))
                                                                    {
                                                                        Servicos.Log.TratarErro("assunto email " + email.Assunto, "XMLEmail");
                                                                        Servicos.Log.TratarErro("erro: " + retorno, "XMLEmail");
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Servicos.Log.TratarErro("assunto email " + email.Assunto, "XMLEmail");
                                                                    Servicos.Log.TratarErro("erro: nenhuma carga pedido localizada", "XMLEmail");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Servicos.Log.TratarErro("assunto email " + email.Assunto, "XMLEmail");
                                                                Servicos.Log.TratarErro("erro: nenhuma placa informada", "XMLEmail");
                                                            }
                                                        }
                                                        else
                                                            await ProcessarEDINotFisAsync(memoryStream, unitOfWork, anexo.NomeArquivo, email.Remetente, cancellationToken);
                                                    }
                                                }
                                            }


                                            if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || configuracaoDocumentoDestinado.SalvarDocumentosRecebidosEmailDestinados)
                                            {
                                                try
                                                {
                                                    System.IO.MemoryStream memoryStreamDestinados = new System.IO.MemoryStream(File.ReadAllBytes(fileLocation));
                                                    System.IO.StreamReader stReaderXML = new StreamReader(memoryStreamDestinados);

                                                    Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa xmlNotaFiscalDestinado = servicoNFe.BuscarDadosNotaFiscalDestinada(stReaderXML, unitOfWork, null);

                                                    if (xmlNotaFiscalDestinado == null)
                                                    {
                                                        xmlNotaFiscalDestinado = servicoNFe.BuscarDadosCTeDestinada(stReaderXML, unitOfWork, null);
                                                        if (xmlNotaFiscalDestinado != null)
                                                        {
                                                            string caminhoDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;

                                                            caminhoDocumentosFiscais = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, "CTe", xmlNotaFiscalDestinado.Empresa.CNPJ_SemFormato, xmlNotaFiscalDestinado.Chave + ".xml");

                                                            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDocumentosFiscais))
                                                                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoDocumentosFiscais, memoryStreamDestinados.ToArray());
                                                        }
                                                        else
                                                            Servicos.Log.TratarErro("Não foi possível converter o arquivo do e-mail para NF-e e CT-e aos Destinados", "XMLEmail");
                                                    }
                                                    else
                                                    {
                                                        try
                                                        {
                                                            string caminhoDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                                                            caminhoDocumentosFiscais = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDocumentosFiscais, "NFe", "nfeProc", xmlNotaFiscalDestinado.Chave + ".xml");

                                                            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoDocumentosFiscais))
                                                                Utilidades.IO.FileStorageService.Storage.WriteAllBytes(caminhoDocumentosFiscais, memoryStreamDestinados.ToArray());
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            Servicos.Log.TratarErro("Não foi possível salvar a NF-e da consulta de documentos destinados: " + ex.ToString(), "XMLEmail");
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Servicos.Log.TratarErro("Falha conversão para Destinados. assunto email " + email.Assunto, "XMLEmail");
                                                    Servicos.Log.TratarErro("Falha conversão para Destinados. erro: " + ex, "XMLEmail");
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Servicos.Log.TratarErro("assunto email " + email.Assunto, "XMLEmail");
                                            Servicos.Log.TratarErro("erro: " + ex, "XMLEmail");
                                        }
                                    }
                                }
                                else if (extensao == ".txt")
                                {
                                    using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(File.ReadAllBytes(fileLocation)))
                                    {
                                        try
                                        {
                                            await ProcessarEDINotFisAsync(memoryStream, unitOfWork, anexo.NomeArquivo, email.Remetente, cancellationToken);
                                        }
                                        catch (Exception ex)
                                        {
                                            Servicos.Log.TratarErro("assunto email " + email.Assunto, "XMLEmail");
                                            Servicos.Log.TratarErro("erro: " + ex, "XMLEmail");
                                        }
                                    }
                                }
                            }

                            if (System.IO.File.Exists(fileLocation))
                                System.IO.File.Delete(fileLocation);
                        }
                    }
                    email.Anexos.Clear();
                    await repositorioEmailCaixaEntrada.DeletarAsync(email);
                }
                else
                {
                    await repositorioEmailCaixaEntrada.DeletarAsync(email);
                }
            }
        }

        private void MoverParaPastaProcessados(string nomeArquivo, System.IO.Stream arquivo, Repositorio.UnitOfWork unitOfWork)
        {
            arquivo.Position = 0;

            string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracaoEDI, "Processados", nomeArquivo);

            using (StreamReader reader = new StreamReader(arquivo))
                Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoCompleto, reader.ReadToEnd(), Encoding.Default);

            arquivo.Close();
            arquivo.Dispose();

        }

        private void VerificarIntegracoesClienteParcial(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Pessoas.ClienteParcial repClienteParcial = new Repositorio.Embarcador.Pessoas.ClienteParcial(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork, cancellationToken);
            List<Dominio.Entidades.Embarcador.Pessoas.ClienteParcial> clienteParcials = repClienteParcial.BuscarPorSituacaoIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao, 2);

            foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteParcial clienteParcial in clienteParcials)
            {
                try
                {
                    Dominio.Entidades.Cliente pessoa = Servicos.Embarcador.Pessoa.Pessoa.CriarPessoa(String.Format(@"{0:00000000000000}", Utilidades.String.OnlyNumbers(clienteParcial.CNPJ)), "ClienteParcial", unitOfWork);
                    if (pessoa != null)
                    {
                        if (!string.IsNullOrEmpty(clienteParcial.InscricaoEstadual))
                            pessoa.IE_RG = clienteParcial.InscricaoEstadual;

                        if (!string.IsNullOrEmpty(clienteParcial.InscricaoMunicipal))
                            pessoa.InscricaoMunicipal = clienteParcial.InscricaoMunicipal;

                        if (!string.IsNullOrEmpty(clienteParcial.CodigoIntegracao))
                            pessoa.CodigoIntegracao = clienteParcial.CodigoIntegracao;

                        if (!string.IsNullOrEmpty(clienteParcial.Email))
                            pessoa.Email = clienteParcial.Email;

                        if (!string.IsNullOrEmpty(clienteParcial.NomeFantasia))
                            pessoa.NomeFantasia = clienteParcial.NomeFantasia;

                        if (!string.IsNullOrEmpty(clienteParcial.Telefone))
                            pessoa.Telefone1 = clienteParcial.Telefone;

                        clienteParcial.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                        pessoa.DataUltimaAtualizacao = DateTime.Now;
                        pessoa.Integrado = false;
                        repCliente.Atualizar(pessoa);
                    }
                    else
                        clienteParcial.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                }
                catch (Exception ex)
                {
                    clienteParcial.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    Servicos.Log.TratarErro(ex, "XMLEmail");
                }
                repClienteParcial.Atualizar(clienteParcial);
            }
        }

        private void VerificarIntegracoes(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec servicoIntegracaoOrtec = new Servicos.Embarcador.Integracao.Ortec.IntegracaoOrtec(unitOfWork);
            Servicos.Embarcador.Integracao.GSW.IntegracaoGSW servicoIntegracaoGSW = new Servicos.Embarcador.Integracao.GSW.IntegracaoGSW(unitOfWork);
            Servicos.Embarcador.Integracao.Arquivei.IntegracaoArquivei servicoIntegracaoArquivei = new Servicos.Embarcador.Integracao.Arquivei.IntegracaoArquivei(unitOfWork);

            if (servicoIntegracaoOrtec.IsPossuiIntegracaoOrtec())
            {
                servicoIntegracaoOrtec.VincularCargasAosAgrupamentosAguardandoProcessamento();
                servicoIntegracaoOrtec.GerarCarregamentos(_tipoServicoMultisoftware);
            }

            if (servicoIntegracaoGSW.IsPossuiIntegracaoGSW())
                servicoIntegracaoGSW.ConsultarXMLCTes(_tipoServicoMultisoftware);

            if (servicoIntegracaoArquivei.IsPossuiIntegracaoArquivei())
                servicoIntegracaoArquivei.ConsultarXMLCTes(_tipoServicoMultisoftware);
        }

        private async Task VerificarFTPTMSAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, CancellationToken cancellationToken)
        {
            if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Pessoas.ClienteLayoutEDI repositorioClienteLayoutEDI = new Repositorio.Embarcador.Pessoas.ClienteLayoutEDI(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoasLayoutEDI repositorioGrupoPessoasLayoutEDI = new Repositorio.Embarcador.Pessoas.GrupoPessoasLayoutEDI(unitOfWork);
            Repositorio.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados repositorioIntegracaoFTPDocumentosDestinados = new Repositorio.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoMoniloc repositorioIntegracaoMoniloc = new Repositorio.Embarcador.Configuracoes.IntegracaoMoniloc(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> clienteLayoutEDIs = repositorioClienteLayoutEDI.BuscarDisponiveisParaLeituraFTP();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> grupoPessoasLayoutEDIs = repositorioGrupoPessoasLayoutEDI.BuscarDisponiveisParaLeituraFTP();
            List<Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados> empresasFTP = repositorioIntegracaoFTPDocumentosDestinados.BuscarTodos();

            foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI clienteLayoutEDI in clienteLayoutEDIs)
            {
                string host = clienteLayoutEDI.EnderecoFTP;
                string porta = clienteLayoutEDI.Porta;
                string diretorio = clienteLayoutEDI.Diretorio;
                string usuario = clienteLayoutEDI.Usuario;
                string senha = clienteLayoutEDI.Senha;
                bool passivo = clienteLayoutEDI.Passivo;
                bool utilizaSFTP = clienteLayoutEDI.UtilizarSFTP;
                bool ssl = clienteLayoutEDI.SSL;

                List<string> arquivosDisponiveis = Servicos.FTP.ObterListagemArquivos(host, porta, diretorio, usuario, senha, passivo, ssl, out string erro, utilizaSFTP, true);

                foreach (string arquivoDisponivel in arquivosDisponiveis)
                {
                    System.IO.Stream arquivo = Servicos.FTP.DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);

                    ProcessarArquivoEDITMS(arquivo, clienteLayoutEDI.LayoutEDI, arquivoDisponivel, unitOfWork, _tipoServicoMultisoftware, clienteLayoutEDI.EmailsAlertaLeituraEDI, adminUnitOfWork, clienteLayoutEDI, null);

                    Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);
                }
            }

            foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI grupoPessoasLayoutEDI in grupoPessoasLayoutEDIs)
            {
                string host = grupoPessoasLayoutEDI.EnderecoFTP;
                string porta = grupoPessoasLayoutEDI.Porta;
                string diretorio = grupoPessoasLayoutEDI.Diretorio;
                string usuario = grupoPessoasLayoutEDI.Usuario;
                string senha = grupoPessoasLayoutEDI.Senha;
                bool passivo = grupoPessoasLayoutEDI.Passivo;
                bool utilizaSFTP = grupoPessoasLayoutEDI.UtilizarSFTP;
                bool ssl = grupoPessoasLayoutEDI.SSL;

                List<string> arquivosDisponiveis = Servicos.FTP.ObterListagemArquivos(host, porta, diretorio, usuario, senha, passivo, ssl, out string erro, utilizaSFTP, true);

                foreach (string arquivoDisponivel in arquivosDisponiveis)
                {
                    System.IO.Stream arquivo = Servicos.FTP.DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);

                    ProcessarArquivoEDITMS(arquivo, grupoPessoasLayoutEDI.LayoutEDI, arquivoDisponivel, unitOfWork, _tipoServicoMultisoftware, grupoPessoasLayoutEDI.EmailsAlertaLeituraEDI, adminUnitOfWork, null, grupoPessoasLayoutEDI);

                    Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);
                }
            }

            foreach (Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados empresaFTP in empresasFTP)
            {
                string host = empresaFTP.EnderecoFTP;
                string porta = empresaFTP.Porta;
                string diretorio = empresaFTP.DiretorioXML;
                string usuario = empresaFTP.Usuario;
                string senha = empresaFTP.Senha;
                bool passivo = empresaFTP.Passivo;
                bool utilizaSFTP = empresaFTP.UtilizarSFTP;
                bool ssl = empresaFTP.SSL;

                List<string> arquivosDisponiveis = Servicos.FTP.ObterListagemArquivos(host, porta, diretorio, usuario, senha, passivo, ssl, out string erro, utilizaSFTP, true);

                foreach (string arquivoDisponivel in arquivosDisponiveis)
                {
                    System.IO.Stream arquivo = Servicos.FTP.DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);

                    await ProcessarArquivoXMLAsync(arquivo, arquivoDisponivel, unitOfWork, cancellationToken);

                    Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);
                }
            }

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMoniloc integracaoMoniloc = await repositorioIntegracaoMoniloc.BuscarPrimeiroRegistroAsync();

            if (integracaoMoniloc != null && (integracaoMoniloc?.PossuiIntegracaoMoniloc ?? false))
            {
                string host = integracaoMoniloc.HostFTP;
                string porta = integracaoMoniloc.PortaFTP;
                string diretorio = integracaoMoniloc.DiretorioConsumoCargasDiarias;
                string usuario = integracaoMoniloc.UsuarioFTP;
                string senha = integracaoMoniloc.SenhaFTP;
                bool passivo = integracaoMoniloc.FTPPassivo;
                bool utilizaSFTP = integracaoMoniloc.SFTP;
                bool ssl = integracaoMoniloc.SSL;

                List<string> arquivosDisponiveis = Servicos.FTP.ObterListagemArquivos(host, porta, diretorio, usuario, senha, passivo, ssl, out string erro, utilizaSFTP, true);

                foreach (string arquivoDisponivel in arquivosDisponiveis)
                {
                    System.IO.Stream arquivo = Servicos.FTP.DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);

                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
                    new Servicos.Embarcador.Integracao.Moniloc.IntegracaoMoniloc(unitOfWork).ProcessarArquivoConsumo(arquivo, arquivoDisponivel, unitOfWork.StringConexao, auditado);

                    Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);
                }
            }

            if (integracaoMoniloc != null && (integracaoMoniloc?.PossuiIntegracaoMoniloc ?? false))
            {
                string host = integracaoMoniloc.HostFTP;
                string porta = integracaoMoniloc.PortaFTP;
                string diretorio = integracaoMoniloc.DiretorioConsumo;
                string usuario = integracaoMoniloc.UsuarioFTP;
                string senha = integracaoMoniloc.SenhaFTP;
                bool passivo = integracaoMoniloc.FTPPassivo;
                bool utilizaSFTP = integracaoMoniloc.SFTP;
                bool ssl = integracaoMoniloc.SSL;

                List<string> arquivosDisponiveis = Servicos.FTP.ObterListagemArquivos(host, porta, diretorio, usuario, senha, passivo, ssl, out string erro, utilizaSFTP, true);

                foreach (string arquivoDisponivel in arquivosDisponiveis)
                {
                    System.IO.Stream arquivo = Servicos.FTP.DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);

                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
                    new Servicos.Embarcador.Integracao.Moniloc.IntegracaoMoniloc(unitOfWork).ProcessarArquivoConsumoExtra(arquivo, arquivoDisponivel, unitOfWork.StringConexao, auditado);

                    Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);
                }
            }
        }

        private async Task VerificarFTPAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {

            if (_tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            Repositorio.Embarcador.Pessoas.ClienteLayoutEDI repositorioClienteLayoutEDI = new Repositorio.Embarcador.Pessoas.ClienteLayoutEDI(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoasLayoutEDI repositorioGrupoPessoasLayoutEDI = new Repositorio.Embarcador.Pessoas.GrupoPessoasLayoutEDI(unitOfWork);
            Repositorio.Embarcador.Canhotos.ConfiguracaoFTPBaixaCanhoto repositorioConfiguracaoFTPBaixaCanhoto = new Repositorio.Embarcador.Canhotos.ConfiguracaoFTPBaixaCanhoto(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.ConfiguracaoFTPEnvioOcoren repositorioConfiguracaoFTPEnvioOcoren = new Repositorio.Embarcador.Ocorrencias.ConfiguracaoFTPEnvioOcoren(unitOfWork);
            Repositorio.Embarcador.Pedidos.ConfiguracaoFTPImportacaoPedido repositorioConfiguracaoFTPImportacaoPedido = new Repositorio.Embarcador.Pedidos.ConfiguracaoFTPImportacaoPedido(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.ConfiguracaoFTPImportacaoNotaFiscal repositorioConfiguracaoFTPImportacaoNotaFiscal = new Repositorio.Embarcador.NotaFiscal.ConfiguracaoFTPImportacaoNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Canhotos.Canhoto repositorioCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
            Repositorio.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados repositorioIntegracaoFTPDocumentosDestinados = new Repositorio.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI> clienteLayoutEDIs = repositorioClienteLayoutEDI.BuscarDisponiveisParaLeituraFTP();
            List<Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI> grupoPessoasLayoutEDIs = repositorioGrupoPessoasLayoutEDI.BuscarDisponiveisParaLeituraFTP();

            foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI clienteLayoutEDI in clienteLayoutEDIs)
            {
                string host = clienteLayoutEDI.EnderecoFTP;
                string porta = clienteLayoutEDI.Porta;
                string diretorio = clienteLayoutEDI.Diretorio;
                string usuario = clienteLayoutEDI.Usuario;
                string senha = clienteLayoutEDI.Senha;
                bool passivo = clienteLayoutEDI.Passivo;
                bool utilizaSFTP = clienteLayoutEDI.UtilizarSFTP;
                bool ssl = clienteLayoutEDI.SSL;

                List<string> arquivosDisponiveis = Servicos.FTP.ObterListagemArquivos(host, porta, diretorio, usuario, senha, passivo, ssl, out string erro, utilizaSFTP, true);

                foreach (string arquivoDisponivel in arquivosDisponiveis)
                {
                    if (!string.IsNullOrWhiteSpace(clienteLayoutEDI.LayoutEDI.Nomenclatura) && !arquivoDisponivel.Contains(clienteLayoutEDI.LayoutEDI.Nomenclatura))
                    {
                        Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);
                        continue;
                    }

                    System.IO.Stream arquivo = Servicos.FTP.DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);

                    await ProcessarArquivoEDIAsync(arquivo, clienteLayoutEDI.LayoutEDI, arquivoDisponivel, unitOfWork, cancellationToken);

                    Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);
                }
            }

            foreach (Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI grupoPessoasLayoutEDI in grupoPessoasLayoutEDIs)
            {
                Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI servicoGrupoPessoaLayoutEDI = new Servicos.Embarcador.Pessoa.GrupoPessoasLayoutEDI(unitOfWork, cancellationToken);

                string host = grupoPessoasLayoutEDI.EnderecoFTP;
                string porta = grupoPessoasLayoutEDI.Porta;
                string diretorio = grupoPessoasLayoutEDI.Diretorio;
                string usuario = grupoPessoasLayoutEDI.Usuario;
                string senha = grupoPessoasLayoutEDI.Senha;
                bool passivo = grupoPessoasLayoutEDI.Passivo;
                bool utilizaSFTP = grupoPessoasLayoutEDI.UtilizarSFTP;
                bool ssl = grupoPessoasLayoutEDI.SSL;
                List<string> prefixos = new List<string>();

                string certificado = await servicoGrupoPessoaLayoutEDI.ObtemCertificadoChavePrivadaAsync(grupoPessoasLayoutEDI);

                if (!string.IsNullOrWhiteSpace(grupoPessoasLayoutEDI.Prefixos))
                {
                    string[] splitPrefixos = grupoPessoasLayoutEDI.Prefixos.Split(';');
                    for (int i = 0; i < splitPrefixos.Length; i++)
                        prefixos.Add(splitPrefixos[i]);
                }

                List<string> arquivosDisponiveis = Servicos.FTP.ObterListagemArquivos(host, porta, diretorio, usuario, senha, passivo, ssl, out string erro, utilizaSFTP, true, prefixos, certificado);

                foreach (string arquivoDisponivel in arquivosDisponiveis)
                {
                    if (!string.IsNullOrWhiteSpace(grupoPessoasLayoutEDI.LayoutEDI.Nomenclatura) && !arquivoDisponivel.Contains(grupoPessoasLayoutEDI.LayoutEDI.Nomenclatura))
                    {
                        Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP, certificado);
                        continue;
                    }

                    System.IO.Stream arquivo = Servicos.FTP.DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP, certificado);

                    await ProcessarArquivoEDIAsync(arquivo, grupoPessoasLayoutEDI.LayoutEDI, arquivoDisponivel, unitOfWork, cancellationToken);

                    Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP, certificado);
                }
            }

            Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoFTPImportacaoPedido configuracaoFTPImportacaoPedido = await repositorioConfiguracaoFTPImportacaoPedido.BuscarPrimeiroRegistroAsync();

            if (configuracaoFTPImportacaoPedido != null)
            {
                List<int> idsColunas = new List<int>();
                List<string> prefixos = new List<string>();

                string host = configuracaoFTPImportacaoPedido.EnderecoFTP;
                string porta = configuracaoFTPImportacaoPedido.Porta;
                string diretorio = configuracaoFTPImportacaoPedido.Diretorio;
                string usuario = configuracaoFTPImportacaoPedido.Usuario;
                string senha = configuracaoFTPImportacaoPedido.Senha;
                bool passivo = configuracaoFTPImportacaoPedido.Passivo;
                bool utilizaSFTP = configuracaoFTPImportacaoPedido.UtilizarSFTP;
                bool ssl = configuracaoFTPImportacaoPedido.SSL;
                prefixos = !string.IsNullOrEmpty(configuracaoFTPImportacaoPedido.Prefixo) ? configuracaoFTPImportacaoPedido.Prefixo.Split(';').ToList() : new List<string>();
                Dominio.Entidades.Embarcador.Importacoes.ConfiguracaoImportacao configuracaoImportacao = configuracaoFTPImportacaoPedido.ConfiguracaoImportacao;
                idsColunas = configuracaoImportacao.Ordem.Split('|').Select(Int32.Parse).ToList();

                List<string> arquivosDisponiveis = Servicos.FTP.ObterListagemArquivos(host, porta, diretorio, usuario, senha, passivo, ssl, out string erro, utilizaSFTP, true, prefixos);

                foreach (string arquivoDisponivel in arquivosDisponiveis)
                {
                    System.IO.Stream arquivo = Servicos.FTP.DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, ssl, arquivoDisponivel, out erro, utilizaSFTP);

                    MemoryStream streamArquivo = new MemoryStream();
                    arquivo.CopyTo(streamArquivo);

                    string extensaoArquivo = System.IO.Path.GetExtension(arquivoDisponivel).ToLower();

                    System.Data.DataTable dt = Servicos.Embarcador.NotaFiscal.ArquivoImportacao.ObterDataTable(extensaoArquivo, streamArquivo, unitOfWork);

                    if (dt != null)
                        Servicos.Embarcador.Pedido.ImportacaoPedido.GerarImportacaoPedidoPorFTP(arquivoDisponivel, dt, idsColunas, unitOfWork);

                    Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, ssl, arquivoDisponivel, out erro, utilizaSFTP);
                }
            }

            Dominio.Entidades.Embarcador.NotaFiscal.ConfiguracaoFTPImportacaoNotaFiscal configuracaoFTPImportacaoNotaFiscal = await repositorioConfiguracaoFTPImportacaoNotaFiscal.BuscarPrimeiroRegistroAsync();

            if (configuracaoFTPImportacaoNotaFiscal != null)
            {
                Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscal repositorioImportacaoNotaFiscal = new Repositorio.Embarcador.NotaFiscal.ImportacaoNotaFiscal(unitOfWork);
                Servicos.Embarcador.NotaFiscal.NotaFiscalImportacao servicoNotaFiscalImportacao = new Servicos.Embarcador.NotaFiscal.NotaFiscalImportacao(unitOfWork, _auditado, _tipoServicoMultisoftware, new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarPrimeiroRegistro(), null);

                List<int> idsColunas = new List<int>();
                List<string> prefixos = new List<string>();

                string host = configuracaoFTPImportacaoNotaFiscal.EnderecoFTP;
                string porta = configuracaoFTPImportacaoNotaFiscal.Porta;
                string diretorio = configuracaoFTPImportacaoNotaFiscal.Diretorio;
                string usuario = configuracaoFTPImportacaoNotaFiscal.Usuario;
                string senha = configuracaoFTPImportacaoNotaFiscal.Senha;
                bool passivo = configuracaoFTPImportacaoNotaFiscal.Passivo;
                bool utilizaSFTP = configuracaoFTPImportacaoNotaFiscal.UtilizarSFTP;
                bool ssl = configuracaoFTPImportacaoNotaFiscal.SSL;
                prefixos = configuracaoFTPImportacaoNotaFiscal.Prefixo.Split(';').ToList();
                Dominio.Entidades.Embarcador.Importacoes.ConfiguracaoImportacao configuracaoImportacao = configuracaoFTPImportacaoNotaFiscal.ConfiguracaoImportacao;
                idsColunas = configuracaoImportacao.Ordem.Split('|').Select(Int32.Parse).ToList();

                List<string> arquivosDisponiveis = Servicos.FTP.ObterListagemArquivos(host, porta, diretorio, usuario, senha, passivo, ssl, out string erro, utilizaSFTP, true, prefixos);

                foreach (string arquivoDisponivel in arquivosDisponiveis)
                {
                    if (repositorioImportacaoNotaFiscal.VerificarExistenciaPorNomeArquivo(arquivoDisponivel))
                    {
                        Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);
                        continue;
                    }

                    System.IO.Stream arquivo = Servicos.FTP.DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, ssl, arquivoDisponivel, out erro, utilizaSFTP);

                    MemoryStream streamArquivo = new MemoryStream();
                    arquivo.CopyTo(streamArquivo);

                    string extensaoArquivo = System.IO.Path.GetExtension(arquivoDisponivel).ToLower();

                    try
                    {
                        await unitOfWork.StartAsync();

                        System.Data.DataTable dt = Servicos.Embarcador.NotaFiscal.ArquivoImportacao.ObterDataTable(extensaoArquivo, streamArquivo, unitOfWork);

                        if (dt != null)
                            servicoNotaFiscalImportacao.GerarImportacaoNotaFiscalPorFTP(arquivoDisponivel, dt, idsColunas, unitOfWork);

                        unitOfWork.CommitChanges();
                    }
                    catch (Exception excecao)
                    {
                        await unitOfWork.RollbackAsync();
                        continue;
                    }

                    Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);
                }
            }

            Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoFTPBaixaCanhoto configuracaoFTPBaixaCanhoto = await repositorioConfiguracaoFTPBaixaCanhoto.BuscarPrimeiroRegistroAsync();

            if (configuracaoFTPBaixaCanhoto != null)
            {
                string host = configuracaoFTPBaixaCanhoto.EnderecoFTP;
                string porta = configuracaoFTPBaixaCanhoto.Porta;
                string diretorio = configuracaoFTPBaixaCanhoto.Diretorio;
                string usuario = configuracaoFTPBaixaCanhoto.Usuario;
                string senha = configuracaoFTPBaixaCanhoto.Senha;
                bool passivo = configuracaoFTPBaixaCanhoto.Passivo;
                bool utilizaSFTP = configuracaoFTPBaixaCanhoto.UtilizarSFTP;
                bool ssl = configuracaoFTPBaixaCanhoto.SSL;

                List<string> arquivosDisponiveis = Servicos.FTP.ObterListagemArquivos(host, porta, diretorio, usuario, senha, passivo, ssl, out string erro, utilizaSFTP, true);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repositorioConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();

                foreach (string arquivoDisponivel in arquivosDisponiveis)
                {
                    System.IO.Stream arquivo = Servicos.FTP.DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);

                    MemoryStream streamArquivo = new MemoryStream();
                    arquivo.CopyTo(streamArquivo);

                    string extencaoArquivo = System.IO.Path.GetExtension(arquivoDisponivel).ToLower();

                    System.Data.DataTable dt = Servicos.Embarcador.NotaFiscal.ArquivoImportacao.ObterDataTable(extencaoArquivo, streamArquivo, unitOfWork);

                    if (dt != null)
                    {
                        for (var i = 0; i < dt.Rows.Count; i++)
                        {
                            Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscal = new Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal();

                            string serie = "";
                            string chaveCanhoto = "";
                            string strNumero = "";
                            string strEmitente = "";

                            int numero = 0;
                            double cnpjEmitente = 0;

                            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = null;

                            for (var j = 0; j < configuracaoFTPBaixaCanhoto.ArquivoImportacaoNotaFiscal.Campos.Count; j++)
                            {
                                Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacaoNotaFiscalCampo campo = configuracaoFTPBaixaCanhoto.ArquivoImportacaoNotaFiscal.Campos[j];

                                object valor = Servicos.Embarcador.NotaFiscal.ArquivoImportacao.LerCampo(campo, dt.Rows[i][(campo.Posicao - 1)].ToString());

                                if (campo.Propriedade == "Serie")
                                    serie = (string)valor;

                                if (campo.Propriedade == "Numero")
                                {
                                    strNumero = (string)valor;
                                    int.TryParse(strNumero, out numero);
                                }

                                if (campo.Propriedade == "CNPJEmitente")
                                {
                                    strEmitente = (string)valor;
                                    double.TryParse(strEmitente, out cnpjEmitente);
                                }


                                if (campo.Propriedade == "ChaveCanhoto")
                                    chaveCanhoto = (string)valor;
                            }

                            if (!string.IsNullOrWhiteSpace(chaveCanhoto))
                                canhoto = await repositorioCanhoto.BuscarPorChaveAsync(chaveCanhoto);
                            else
                                canhoto = repositorioCanhoto.BuscarPorNumeroSerieEmitenteNFe(numero, serie, cnpjEmitente);

                            if (canhoto == null)
                            {
                                Servicos.Log.TratarErro("Não foi possivel localizado um canhoto para a linha " + i.ToString() + " da planilha " + arquivoDisponivel, "Importacao Baixa Canhoto");
                                continue;
                            }

                            await unitOfWork.StartAsync();

                            canhoto.SituacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCanhoto.Cancelado;
                            canhoto.SituacaoDigitalizacaoCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDigitalizacaoCanhoto.Cancelada;

                            Servicos.Embarcador.Canhotos.Canhoto.CanhotoLiberado(canhoto, configuracao, unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware);

                            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };

                            Servicos.Auditoria.Auditoria.Auditar(auditado, canhoto, "Cancelou por importação via FTP.", unitOfWork);

                            await repositorioCanhoto.AtualizarAsync(canhoto);

                            unitOfWork.CommitChanges();
                        }
                    }
                    else
                        Servicos.Log.TratarErro("Leitura baixa canhoto FTP, não foi possivel interpretar a extensão " + extencaoArquivo, "Importacao Baixa Canhoto");

                    Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);
                }
            }

            Dominio.Entidades.Embarcador.Ocorrencias.ConfiguracaoFTPEnvioOcoren configuracaoFTPEnvioOcoren = await repositorioConfiguracaoFTPEnvioOcoren.BuscarPrimeiroRegistroAsync();

            if (configuracaoFTPEnvioOcoren != null)
            {
                string host = configuracaoFTPEnvioOcoren.EnderecoFTP;
                string porta = configuracaoFTPEnvioOcoren.Porta;
                string diretorio = configuracaoFTPEnvioOcoren.Diretorio;
                string usuario = configuracaoFTPEnvioOcoren.Usuario;
                string senha = configuracaoFTPEnvioOcoren.Senha;
                bool passivo = configuracaoFTPEnvioOcoren.Passivo;
                bool utilizaSFTP = configuracaoFTPEnvioOcoren.UtilizarSFTP;
                bool ssl = configuracaoFTPEnvioOcoren.SSL;

                List<string> arquivosDisponiveis = Servicos.FTP.ObterListagemArquivos(host, porta, diretorio, usuario, senha, passivo, ssl, out string erro, utilizaSFTP, true);

                Servicos.Embarcador.CargaOcorrencia.Ocorrencia serOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia();

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repositorioConfiguracao.BuscarConfiguracaoPadraoAsync();

                Dominio.Entidades.LayoutEDI layoutEDI = repLayoutEDI.BuscarPrimeiroPorTipo(Dominio.Enumeradores.TipoLayoutEDI.OCOREN);

                foreach (string arquivoDisponivel in arquivosDisponiveis)
                {
                    System.IO.Stream arquivo = Servicos.FTP.DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);

                    MemoryStream streamArquivo = new MemoryStream();
                    arquivo.CopyTo(streamArquivo);

                    string extencaoArquivo = System.IO.Path.GetExtension(arquivoDisponivel).ToLower();
                    string retorno = serOcorrencia.ProcessarOcorren(layoutEDI, streamArquivo, arquivoDisponivel, _tipoServicoMultisoftware, configuracao, null, TipoEnvioArquivo.FTP, _clienteMultisoftware, unitOfWork, _auditado);
                    if (!string.IsNullOrEmpty(retorno))
                    {
                        Servicos.Log.TratarErro("Erro ao processar EDI Ocoren por FTP" + retorno);
                    }

                    Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);
                }
            }

            List<Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados> empresasFTP = await repositorioIntegracaoFTPDocumentosDestinados.BuscarTodosAsync();

            foreach (Dominio.Entidades.Embarcador.Documentos.IntegracaoFTPDocumentosDestinados empresaFTP in empresasFTP)
            {
                string host = empresaFTP.EnderecoFTP;
                string porta = empresaFTP.Porta;
                string diretorio = empresaFTP.DiretorioXML;
                string usuario = empresaFTP.Usuario;
                string senha = empresaFTP.Senha;
                bool passivo = empresaFTP.Passivo;
                bool utilizaSFTP = empresaFTP.UtilizarSFTP;
                bool ssl = empresaFTP.SSL;

                List<string> arquivosDisponiveis = Servicos.FTP.ObterListagemArquivos(host, porta, diretorio, usuario, senha, passivo, ssl, out string erro, utilizaSFTP, true);

                if (!string.IsNullOrWhiteSpace(erro))
                    Servicos.Log.TratarErro(erro, "VerificarFTP");

                foreach (string arquivoDisponivel in arquivosDisponiveis)
                {
                    System.IO.Stream arquivo = Servicos.FTP.DownloadArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);

                    await ProcessarArquivoXMLAsync(arquivo, arquivoDisponivel, unitOfWork, cancellationToken);

                    Servicos.FTP.DeletarArquivo(host, porta, diretorio, usuario, senha, passivo, false, arquivoDisponivel, out erro, utilizaSFTP);
                }
            }
        }

        private async Task<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ProcessarXMLNFeAsync(System.IO.Stream xml, string remetente, string destinatario, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken, bool documentoRecebidoPorFTP = false, bool documentoRecebidoPorEmail = false)
        {
            Servicos.Embarcador.NFe.NFe servicoNFe = new Servicos.Embarcador.NFe.NFe(unitOfWork, cancellationToken);
            Servicos.Embarcador.Pedido.NotaFiscal servicoCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork, cancellationToken);
            Servicos.Embarcador.Financeiro.DocumentoEntrada servicoDocumentoEntrada = new Servicos.Embarcador.Financeiro.DocumentoEntrada(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork, cancellationToken);
            Repositorio.Abastecimento repositorioAbastecimento = new Repositorio.Abastecimento(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repositorioTabelaValores = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Veiculos.VeiculoMotorista repositorioVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento repositorioConfiguracaoFinanceiraAbastecimento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork, cancellationToken);


            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await repositorioConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiraAbastecimento configuracaoAbastecimento = await repositorioConfiguracaoFinanceiraAbastecimento.BuscarPrimeiroRegistroAsync();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistroAsync();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebservice = await repositorioConfiguracaoWebService.BuscarPrimeiroRegistroAsync();

            xml.Position = 0;

            System.IO.StreamReader stReaderXML = new StreamReader(xml);

            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            try
            {
                if (!servicoNFe.BuscarDadosNotaFiscal(out string erro, out xmlNotaFiscal, stReaderXML, unitOfWork, null, true, configuracaoTMS.GerarCargaDeNotasRecebidasPorEmail, false, _tipoServicoMultisoftware, configuracaoTMS.ImportarEmailCliente, configuracaoTMS.UtilizarValorFreteNota, null, null, null, configuracaoWebservice?.CadastroAutomaticoPessoaExterior ?? false))
                    Servicos.Log.TratarErro(erro, "XMLEmail");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "XMLEmail");
            }

            if (xmlNotaFiscal == null)
                return null;

            await unitOfWork.StartAsync();

            Servicos.Embarcador.Integracao.IndicadorIntegracaoNFe servicoIndicadorIntegracaoNFe = new Servicos.Embarcador.Integracao.IndicadorIntegracaoNFe(unitOfWork, configuracaoTMS);

            try
            {
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repositorioXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork, cancellationToken);

                Servicos.Embarcador.Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork, cancellationToken);
                PedidoXMLNotaFiscal servicoPedidoXMLNotaFiscal = new PedidoXMLNotaFiscal(unitOfWork, configuracaoTMS, configuracaoGeralCarga);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                    Texto = "Adicionado via e-mail"
                };

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaExiste = await repositorioXMLNotaFiscal.BuscarPorChaveAsync(xmlNotaFiscal.Chave, cancellationToken);

                xmlNotaFiscal.DocumentoRecebidoViaEmail = documentoRecebidoPorEmail;
                xmlNotaFiscal.DocumentoRecebidoViaFTP = documentoRecebidoPorFTP;

                if (xmlNotaFiscal.FormaIntegracao == FormaIntegracao.OKColeta && (xmlNotaFiscal.DocumentoRecebidoViaNOTFIS || xmlNotaFiscal.DocumentoRecebidoViaEmail || xmlNotaFiscal.DocumentoRecebidoViaFTP))
                    xmlNotaFiscal.FormaIntegracao = FormaIntegracao.ClienteFTPOKColeta;
                if (xmlNotaFiscal.FormaIntegracao == FormaIntegracao.OKColeta && xmlNotaFiscal.NotaJaEstavaNaBase)
                    xmlNotaFiscal.FormaIntegracao = FormaIntegracao.OKColetaManual;
                else if (xmlNotaFiscal.NotaJaEstavaNaBase && (xmlNotaFiscal.DocumentoRecebidoViaNOTFIS || xmlNotaFiscal.DocumentoRecebidoViaEmail || xmlNotaFiscal.DocumentoRecebidoViaFTP))
                    xmlNotaFiscal.FormaIntegracao = FormaIntegracao.ClienteFTPManual;
                else if (xmlNotaFiscal.DocumentoRecebidoViaNOTFIS || xmlNotaFiscal.DocumentoRecebidoViaEmail || xmlNotaFiscal.DocumentoRecebidoViaFTP)
                    xmlNotaFiscal.FormaIntegracao = FormaIntegracao.ClienteFTP;
                else if (xmlNotaFiscal.FormaIntegracao == FormaIntegracao.OKColeta)
                    xmlNotaFiscal.FormaIntegracao = FormaIntegracao.OKColeta;
                else
                    xmlNotaFiscal.FormaIntegracao = FormaIntegracao.Manual;

                if (xmlNotaExiste == null)
                {
                    xmlNotaFiscal.DataRecebimento = DateTime.Now;
                    await repositorioXMLNotaFiscal.InserirAsync(xmlNotaFiscal);
                }
                else
                {
                    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFisca = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                    if (repPedidoXMLNotaFisca.ContarXMLPorNotaFiscal(xmlNotaExiste.Codigo) <= 0)
                        xmlNotaFiscal = xmlNotaExiste;
                    await repositorioXMLNotaFiscal.AtualizarAsync(xmlNotaFiscal);
                }

                if (xmlNotaFiscal == null)
                    return null;

                string kmObservacao = "";
                string placaObservacao = "";
                string horimetroObservacao = "";
                string chassiObservacao = "";

                servicoDocumentoEntrada.RetornaTagAbastecimento(out kmObservacao, out placaObservacao, out horimetroObservacao, out chassiObservacao, xmlNotaFiscal.Observacao, xmlNotaFiscal.Emitente);

                Dominio.Entidades.Veiculo veiculoObs = null;

                if (!string.IsNullOrWhiteSpace(placaObservacao))
                    veiculoObs = await repositorioVeiculo.BuscarPorPlacaAsync(placaObservacao, cancellationToken);
                else if (!string.IsNullOrWhiteSpace(chassiObservacao))
                    veiculoObs = await repositorioVeiculo.BuscarPorChassiAsync(chassiObservacao, cancellationToken);

                if (veiculoObs != null && xmlNotaFiscal.Emitente != null && xmlNotaFiscal.Emitente.ProcessarAbastecimentoAutomaticamenteAoReceberXMLdaNfe.HasValue && xmlNotaFiscal.Emitente.ProcessarAbastecimentoAutomaticamenteAoReceberXMLdaNfe.Value && !await repositorioAbastecimento.ContemAbastecimentoPorChaveAsync(xmlNotaFiscal.Chave, cancellationToken))
                {
                    await servicoPedidoXMLNotaFiscal.ArmazenarProdutosXMLAsync(xmlNotaFiscal.XML, xmlNotaFiscal, auditado, _tipoServicoMultisoftware);

                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> produtos = await repositorioXMLNotaFiscalProduto.BuscarPorNotaFiscalAsync(xmlNotaFiscal.Codigo, cancellationToken);

                    if (produtos != null && produtos.Count > 0)
                    {
                        for (int i = 0; i < produtos.Count; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(produtos[i].cProd))
                            {
                                Dominio.Entidades.Produto produto = await repositorioTabelaValores.BuscarProdutoPorPessoaAsync(produtos[i].cProd, xmlNotaFiscal.Emitente.CPF_CNPJ, cancellationToken);

                                if (produto != null && !await repositorioAbastecimento.AbastecimentoDuplicadoAsync(xmlNotaFiscal.DataEmissao, xmlNotaFiscal.Numero.ToString(), xmlNotaFiscal.Emitente?.CPF_CNPJ ?? 0d, produto.Codigo, produtos[i].Quantidade, produtos[i].ValorProduto, cancellationToken))
                                {
                                    Dominio.Entidades.Abastecimento abastecimento = new Dominio.Entidades.Abastecimento();

                                    abastecimento.Veiculo = veiculoObs;
                                    abastecimento.Kilometragem = kmObservacao.ToInt();
                                    abastecimento.Equipamento = null;
                                    abastecimento.Horimetro = horimetroObservacao.ToInt();

                                    Servicos.Embarcador.Abastecimento.Abastecimento.ProcessarViradaKMHorimetro(abastecimento, abastecimento.Veiculo, abastecimento.Equipamento);

                                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento;
                                    if (produto.CodigoNCM.StartsWith("271121") || produto.CodigoNCM.StartsWith("271019") || produto.CodigoNCM.StartsWith("271012"))
                                        tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel;
                                    else
                                        tipoAbastecimento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla;

                                    abastecimento.Motorista = null;
                                    abastecimento.Posto = xmlNotaFiscal.Emitente;
                                    abastecimento.Produto = produto;
                                    abastecimento.TipoAbastecimento = tipoAbastecimento;
                                    abastecimento.Litros = produtos[i].Quantidade;
                                    abastecimento.ValorUnitario = produtos[i].ValorProduto;
                                    abastecimento.Status = "A";
                                    abastecimento.Situacao = "A";
                                    abastecimento.DataAlteracao = DateTime.Now;
                                    abastecimento.Data = xmlNotaFiscal.DataEmissao;
                                    abastecimento.Documento = xmlNotaFiscal.Numero.ToString();
                                    abastecimento.ChaveNotaFiscal = xmlNotaFiscal.Chave;
                                    abastecimento.TipoRecebimentoAbastecimento = TipoRecebimentoAbastecimento.ImportacaoXML;

                                    if (abastecimento.Veiculo != null)
                                        abastecimento.Motorista = await repositorioVeiculoMotorista.BuscarMotoristaPrincipalAsync(abastecimento.Veiculo.Codigo, cancellationToken);

                                    if (abastecimento.Motorista == null && abastecimento.Equipamento != null)
                                    {
                                        Dominio.Entidades.Veiculo veiculoEquipamento = await repositorioVeiculo.BuscarPorEquipamentoAsync(abastecimento.Equipamento.Codigo, cancellationToken);
                                        Dominio.Entidades.Usuario MotoristaEquipamento = veiculoEquipamento != null ? await repositorioVeiculoMotorista.BuscarMotoristaPrincipalAsync(veiculoEquipamento.Codigo, cancellationToken) : null;

                                        if (veiculoEquipamento != null && MotoristaEquipamento != null)
                                            abastecimento.Motorista = MotoristaEquipamento;
                                        else if (veiculoEquipamento != null)
                                        {
                                            Dominio.Entidades.Veiculo veiculoTracao = await repositorioVeiculo.BuscarPorReboqueAsync(veiculoEquipamento.Codigo, cancellationToken);

                                            if (veiculoTracao != null)
                                                abastecimento.Motorista = repositorioVeiculoMotorista.BuscarMotoristaPrincipal(veiculoTracao.Codigo);
                                        }
                                    }
                                    else if (abastecimento.Motorista == null && abastecimento.Veiculo != null)
                                    {
                                        Dominio.Entidades.Veiculo veiculoTracao = await repositorioVeiculo.BuscarPorReboqueAsync(abastecimento.Veiculo.Codigo, cancellationToken);

                                        if (veiculoTracao != null)
                                            abastecimento.Motorista = await repositorioVeiculoMotorista.BuscarMotoristaPrincipalAsync(veiculoTracao.Codigo, cancellationToken);
                                    }

                                    abastecimento.TipoMovimento = configuracaoAbastecimento.TipoMovimentoLancamentoAbastecimentoPosto;

                                    Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unitOfWork, abastecimento.Veiculo, null, configuracaoTMS);

                                    await repositorioAbastecimento.InserirAsync(abastecimento);

                                    await Servicos.Auditoria.Auditoria.AuditarAsync(auditado, abastecimento, null, "Abastecimento inserido por uma nota fiscal recebida do e-mail.", unitOfWork);
                                }
                            }
                        }
                    }

                }


                Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail repositorioRegraAutomatizacaoEmissoesEmail = new Repositorio.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.RegraAutomatizacaoEmissoesEmail regraAutomatizacaoEmissoesEmail = !string.IsNullOrWhiteSpace(destinatario) ? repositorioRegraAutomatizacaoEmissoesEmail.BuscarPorRegraAutomatizacaoEmissoesEmail(destinatario, xmlNotaFiscal.Emitente.CPF_CNPJ, xmlNotaFiscal.Destinatario.CPF_CNPJ) : null;

                if (regraAutomatizacaoEmissoesEmail != null)
                {
                    bool criouCarga = false;
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarCargaPedidoPorCargaePedidoCodigoIntegracao(xmlNotaFiscal.NumeroDT, xmlNotaFiscal.NumeroDT, xmlNotaFiscal.Emitente.CPF_CNPJ_SemFormato);

                    if (cargaPedido == null)
                    {
                        string retorno = "";
                        cargaPedido = servicoCargaNotaFiscal.CriarCargaPorNotaFiscalPorRegraAutomatizacaoEmail(xmlNotaFiscal, ref retorno, _tipoServicoMultisoftware, regraAutomatizacaoEmissoesEmail, unitOfWork.StringConexao);

                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            if (!string.IsNullOrWhiteSpace(configuracaoTMS.EmailsRetornoProblemaGerarCargaEmail))
                                enviarEmailProblemaImportarCargaNFe(xmlNotaFiscal.Chave, retorno, configuracaoTMS.EmailsRetornoProblemaGerarCargaEmail, unitOfWork);

                            Servicos.Log.TratarErro(retorno);
                            servicoIndicadorIntegracaoNFe.AdicionarIntegracaoPorEmailRejeitada(xmlNotaFiscal, remetente, retorno);
                        }
                        criouCarga = true;
                    }
                    else if (!cargaPedido.Carga.CargaFechada)
                    {
                        Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;

                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = await repositorioCargaPedido.BuscarPorCargaAsync(carga.Codigo);

                        serCarga.FecharCarga(cargaPedido.Carga, unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware);
                        carga.CargaFechada = true;
                        Servicos.Log.TratarErro("20 - Fechou Carga (" + cargaPedido.Carga.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");

                        servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracaoTMS, unitOfWork, _tipoServicoMultisoftware);

                        await repCarga.AtualizarAsync(carga);

                        Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Solicitou o fechamento da carga por recebimento de xml das notas fiscais por e-mail (enviada pelo e-mail " + remetente + " ).", unitOfWork);
                    }

                    if (cargaPedido != null)
                    {
                        if (cargaPedido.Carga.SituacaoCarga.IsSituacaoCargaNaoEmitida()) //&& !cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete)
                        {
                            if (xmlNotaFiscal.Peso == 0)
                            {
                                xmlNotaFiscal.Peso = (decimal)0.1;
                                await repositorioXMLNotaFiscal.AtualizarAsync(xmlNotaFiscal);
                            }
                            bool sumarizar = false;
                            if (criouCarga || !repositorioPedidoXMLNotaFiscal.VerificarSeExisteNotaPorDestinatario(cargaPedido.Carga.Codigo, xmlNotaFiscal.Destinatario.CPF_CNPJ))
                                sumarizar = true;

                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = servicoCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, _tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracaoTMS, false, out bool alteradoTipoDeCarga);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, xmlNotaFiscal, "Adicionado via e-mail", unitOfWork);

                            servicoIndicadorIntegracaoNFe.AdicionarIntegracaoPorEmailComSucesso(pedidoXMLNotaFiscal, remetente);

                            if (sumarizar)
                            {
                                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
                                servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracaoTMS, unitOfWork, _tipoServicoMultisoftware);
                                await repCarga.AtualizarAsync(carga);

                                if (criouCarga)
                                    Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Gerou Carga por recebimento de xml das notas fiscais por e-mail (enviada pelo e-mail " + remetente + " ).", unitOfWork);
                            }
                            await unitOfWork.CommitChangesAsync();
                        }
                    }
                    else
                    {
                        await unitOfWork.RollbackAsync();
                        Servicos.Log.TratarErro("Rollback 01 ", "XMLEmail");
                    }
                }
                else if (!configuracaoTMS.GerarCargaDeNotasRecebidasPorEmail)
                {
                    if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        if (!VincularNotaFiscalCarga(xmlNotaFiscal, configuracaoTMS, unitOfWork))
                            xmlNotaFiscal.SemCarga = true;

                        await repositorioXMLNotaFiscal.AtualizarAsync(xmlNotaFiscal);

                        if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.NumeroControlePedido))
                            servicoCargaNotaFiscal.VincularNotaFiscalAPedidosPorNumeroControle(xmlNotaFiscal, configuracaoTMS, auditado, _tipoServicoMultisoftware);
                    }
                    else
                    {
                        Servicos.Log.TratarErro($"VincularXMLNotaFiscal : {xmlNotaFiscal.Chave}  - {DateTime.Now.ToString("dd/MM/yyyy HH:mm:sss")}", $"ProcessamentoArquivoXMLNotaFiscalIntegracao");
                        servicoCargaNotaFiscal.VincularXMLNotaFiscal(xmlNotaFiscal, configuracaoTMS, _tipoServicoMultisoftware, auditado, true, false);
                    }
                    await unitOfWork.CommitChangesAsync();
                }
                else
                {
                    bool criouCarga = false;
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarCargaPedidoPorCargaePedidoCodigoIntegracao(xmlNotaFiscal.NumeroDT, xmlNotaFiscal.NumeroDT, xmlNotaFiscal.Emitente.CPF_CNPJ_SemFormato);

                    if (cargaPedido == null)
                    {
                        string retorno = "";
                        cargaPedido = servicoCargaNotaFiscal.CriarCargaPorNotaFiscal(xmlNotaFiscal, ref retorno, _tipoServicoMultisoftware, unitOfWork.StringConexao);

                        if (!string.IsNullOrWhiteSpace(retorno))
                            throw new ServicoException(retorno);

                        criouCarga = true;
                    }
                    else if (!cargaPedido.Carga.CargaFechada)
                    {
                        Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;

                        List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

                        serCarga.FecharCarga(cargaPedido.Carga, unitOfWork, _tipoServicoMultisoftware, _clienteMultisoftware);
                        carga.CargaFechada = true;
                        Servicos.Log.TratarErro("20 - Fechou Carga (" + cargaPedido.Carga.Codigo + ") " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff"), "FechamentoCarga");

                        servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracaoTMS, unitOfWork, _tipoServicoMultisoftware);
                        repCarga.Atualizar(carga);

                        Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Solicitou o fechamento da carga por recebimento de xml das notas fiscais por e-mail (enviada pelo e-mail " + remetente + " ).", unitOfWork);
                    }


                    if (cargaPedido != null)
                    {
                        if (cargaPedido.Carga.SituacaoCarga.IsSituacaoCargaNaoEmitida()) //&& !cargaPedido.Carga.ExigeNotaFiscalParaCalcularFrete)
                        {
                            if (xmlNotaFiscal.Peso == 0)
                            {
                                xmlNotaFiscal.Peso = (decimal)0.1;
                                repositorioXMLNotaFiscal.Atualizar(xmlNotaFiscal);
                            }
                            bool sumarizar = false;
                            if (criouCarga || !repositorioPedidoXMLNotaFiscal.VerificarSeExisteNotaPorDestinatario(cargaPedido.Carga.Codigo, xmlNotaFiscal.Destinatario.CPF_CNPJ))
                                sumarizar = true;

                            Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = servicoCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, _tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracaoTMS, false, out bool alteradoTipoDeCarga);
                            Servicos.Auditoria.Auditoria.Auditar(auditado, xmlNotaFiscal, "Adicionado via e-mail", unitOfWork);

                            servicoIndicadorIntegracaoNFe.AdicionarIntegracaoPorEmailComSucesso(pedidoXMLNotaFiscal, remetente);

                            if (sumarizar)
                            {
                                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaPedido.Carga;
                                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);
                                servicoCargaDadosSumarizados.AlterarDadosSumarizadosCarga(ref carga, cargaPedidos, configuracaoTMS, unitOfWork, _tipoServicoMultisoftware);
                                repCarga.Atualizar(carga);

                                if (criouCarga)
                                    Servicos.Auditoria.Auditoria.Auditar(auditado, carga, "Gerou Carga por recebimento de xml das notas fiscais por e-mail (enviada pelo e-mail " + remetente + " ).", unitOfWork);
                            }

                            unitOfWork.CommitChanges();
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro("Rollback 02 ", "XMLEmail");
                    }
                }
            }
            catch (ServicoException excecao)
            {
                try
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(excecao, "XMLEmail");
                    if (!string.IsNullOrWhiteSpace(configuracaoTMS.EmailsRetornoProblemaGerarCargaEmail))
                        enviarEmailProblemaImportarCargaNFe(xmlNotaFiscal.Chave, excecao.Message, configuracaoTMS.EmailsRetornoProblemaGerarCargaEmail, unitOfWork);
                    servicoIndicadorIntegracaoNFe.AdicionarIntegracaoPorEmailRejeitada(xmlNotaFiscal, remetente, excecao.Message);
                }
                catch (Exception)
                {
                    Servicos.Log.TratarErro(excecao.Message, "XMLEmail");
                }
            }
            catch (Exception excecao)
            {
                try
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(excecao, "XMLEmail");
                }
                catch (Exception)
                {
                    Servicos.Log.TratarErro(excecao.Message, "XMLEmail");
                }
            }
            return xmlNotaFiscal;
        }

        private bool VincularNotaFiscalCarga(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork)
        {
            bool vincularNotaFiscal = xmlNotaFiscal?.Emitente?.GrupoPessoas?.VincularNotaFiscalEmailNaCarga ?? false;

            if (!vincularNotaFiscal)
                return false;

            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEspelhoIntercement repositorioPedidoEspelhoIntercement = new Repositorio.Embarcador.Pedidos.PedidoEspelhoIntercement(unitOfWork);
            Repositorio.Embarcador.Pedidos.Container repositorioContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repositorioPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaIntegracao(unitOfWork);

            Servicos.WebService.Carga.Pedido servicoWSPedido = new Servicos.WebService.Carga.Pedido(unitOfWork);

            if (repositorioPedidoXMLNotaFiscal.VerificarSeExisteEmAlgumPedido(xmlNotaFiscal.Codigo))
                return false;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.NumeroDT))
            {
                cargaPedidos = repositorioCargaPedido.BuscarCargaPedidoParaVincularNotasPorNumeroPedido(xmlNotaFiscal.NumeroDT);

                if (cargaPedidos.Count > 1)
                    cargaPedidos = cargaPedidos.Where(o => (o.Pedido.Destinatario?.CPF_CNPJ ?? 0D) == (xmlNotaFiscal.Destinatario?.CPF_CNPJ ?? 0D)).ToList();
            }

            Dominio.Entidades.Embarcador.Pedidos.Container containerNota = null;

            if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.NumeroContainer))
                containerNota = repositorioContainer.BuscarPorNumero(xmlNotaFiscal.NumeroContainer);

            bool notaPossuiBooking = false;
            if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.NumeroBooking) && containerNota != null)
            {
                cargaPedidos = repositorioCargaPedido.BuscarCargaPedidoParaVincularNotasPorNumeroBooking(xmlNotaFiscal.NumeroBooking, xmlNotaFiscal.NumeroContainer);
                if (cargaPedidos != null && cargaPedidos.Count > 0)
                    notaPossuiBooking = true;
                else if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.NumeroBooking))
                {
                    cargaPedidos = repositorioCargaPedido.BuscarCargaPedidoParaVincularNotasPorNumeroBooking(xmlNotaFiscal.NumeroBooking, true);
                    if (cargaPedidos != null && cargaPedidos.Count > 0)
                        notaPossuiBooking = true;
                    else
                    {
                        cargaPedidos = repositorioCargaPedido.BuscarCargaPedidoParaVincularNotasPorNumeroBooking(xmlNotaFiscal.NumeroBooking, false);
                        if (cargaPedidos != null && cargaPedidos.Count > 0)
                            notaPossuiBooking = true;
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(xmlNotaFiscal.NumeroBooking))
            {
                cargaPedidos = repositorioCargaPedido.BuscarCargaPedidoParaVincularNotasPorNumeroBooking(xmlNotaFiscal.NumeroBooking, true);
                if (cargaPedidos != null && cargaPedidos.Count > 0)
                    notaPossuiBooking = true;
                else
                {
                    cargaPedidos = repositorioCargaPedido.BuscarCargaPedidoParaVincularNotasPorNumeroBooking(xmlNotaFiscal.NumeroBooking, false);
                    if (cargaPedidos != null && cargaPedidos.Count > 0)
                        notaPossuiBooking = true;
                }
            }

            if (cargaPedidos.Count() <= 0)
                cargaPedidos = repositorioCargaPedido.BuscarCargaPedidoParaVincularNotasPorOrigemDestinoDataEVeiculo(xmlNotaFiscal.Emitente.CPF_CNPJ, xmlNotaFiscal.Destinatario.CPF_CNPJ, xmlNotaFiscal.DataEmissao.Date, xmlNotaFiscal.PlacaVeiculoNotaFiscal);

            if (cargaPedidos.Count() <= 0 && !string.IsNullOrWhiteSpace(xmlNotaFiscal.NumeroDT))
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement pedidoEspelho = repositorioPedidoEspelhoIntercement.BuscarPorNumeroRemessa(xmlNotaFiscal.NumeroDT, SituacaoCarga.AgNFe);
                if (pedidoEspelho != null)
                    cargaPedidos.Add(pedidoEspelho.CargaPedido);
            }

            if (!notaPossuiBooking && cargaPedidos.Count != 1) //tratar para enviar e-mail ou mandar um sinal de fumaça
                return false;

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidos[0];
            if (cargaPedido != null && cargaPedido.Pedido != null)
            {
                new Servicos.Embarcador.Pessoa.GrupoPessoasObservacaoNfe().AdicionarDadosNfePorGrupoPessoasEmitente(cargaPedido, xmlNotaFiscal.Emitente, xmlNotaFiscal.Observacao, _tipoServicoMultisoftware, configuracaoTMS, unitOfWork, xmlNotaFiscal, _auditado);
                repositorioPedido.Atualizar(cargaPedido.Pedido);
            }

            Servicos.Embarcador.Pedido.NotaFiscal serCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

            if (cargaPedido.Carga.TipoOperacao != null && cargaPedido.Carga.TipoOperacao.UtilizarDadosPedidoParaNotasExterior && xmlNotaFiscal.Destinatario != null && xmlNotaFiscal.Destinatario.Tipo == "E")
            {
                switch (cargaPedido.Pedido.TipoPagamento)
                {
                    case Dominio.Enumeradores.TipoPagamento.Pago:
                        xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago;
                        break;
                    case Dominio.Enumeradores.TipoPagamento.A_Pagar:
                        xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar;
                        break;
                    case Dominio.Enumeradores.TipoPagamento.Outros:
                        xmlNotaFiscal.ModalidadeFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Outros;
                        break;
                    default:
                        break;
                }

                if (xmlNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada)
                {
                    xmlNotaFiscal.Destinatario = cargaPedido.Pedido.Remetente;
                    if (configuracaoTMS.UtilizaEmissaoMultimodal)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco enderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                        servicoWSPedido.PreecherEnderecoPedidoPorCliente(enderecoOrigem, cargaPedido.Pedido.Remetente);
                        repositorioPedidoEndereco.Inserir(enderecoOrigem);
                        cargaPedido.Pedido.EnderecoOrigem = enderecoOrigem;
                    }
                }
                else
                    xmlNotaFiscal.Destinatario = cargaPedido.Pedido.Destinatario;
            }

            xmlNotaFiscal.Filial = cargaPedido.Pedido.Filial;

            bool msgAlertaObservacao = false;
            bool notaFiscalEmOutraCarga = false;
            string retorno = serCargaNotaFiscal.ValidarRegrasNota(xmlNotaFiscal, cargaPedido, _tipoServicoMultisoftware, out msgAlertaObservacao, out notaFiscalEmOutraCarga);
            if (msgAlertaObservacao && !string.IsNullOrWhiteSpace(retorno))
                retorno = "";

            if (string.IsNullOrEmpty(retorno))
            {
                if (repositorioCargaIntegracao.ExistePorCargaETipo(cargaPedido.Carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercement))
                {
                    if (!serCargaNotaFiscal.InformarComponentesOperacaoIntercement(out string msgErro, cargaPedido, xmlNotaFiscal))
                        return false;
                    else
                        repositorioXMLNotaFiscal.Atualizar(xmlNotaFiscal);
                }

                serCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, cargaPedido, _tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracaoTMS, notaFiscalEmOutraCarga, out bool alteradoTipoDeCarga);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditar = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };
                Servicos.Auditoria.Auditoria.Auditar(auditar, xmlNotaFiscal, "Adicionado via e-mail.", unitOfWork);

                if (configuracaoTMS.AvancarCargaAoReceberNotasPorEmail)
                {
                    cargaPedido.Carga.DataInicioConfirmacaoDocumentosFiscais = DateTime.Now;
                    cargaPedido.Carga.ProcessandoDocumentosFiscais = true;

                    repositorioCarga.Atualizar(cargaPedido.Carga);
                }

                return true;
            }

            return false;
        }

        private async Task<bool> ProcessarEDINotFisAsync(System.IO.Stream edi, Repositorio.UnitOfWork unitOfWork, string nomeArquivo, string email, CancellationToken cancellationToken)
        {
            try
            {
                Repositorio.LayoutEDI repositorioLayoutEDI = new Repositorio.LayoutEDI(unitOfWork, cancellationToken);
                Dominio.Entidades.LayoutEDI layoutEDI = repositorioLayoutEDI.BuscarPorEmailRemetente(email, Dominio.Enumeradores.TipoLayoutEDI.NOTFIS);

                if (layoutEDI == null)
                    layoutEDI = repositorioLayoutEDI.BuscarPorEmailRemetente(email, Dominio.Enumeradores.TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO);

                if (layoutEDI != null)
                    await ProcessarArquivoEDIAsync(edi, layoutEDI, nomeArquivo, unitOfWork, cancellationToken);

                return false;
            }
            catch
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        private void ProcessarArquivoEDITMS(System.IO.Stream arquivo, Dominio.Entidades.LayoutEDI layoutEDI, string arquivoDisponivel, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware, string emailErroProcessamentoEDI, AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork, Dominio.Entidades.Embarcador.Pessoas.ClienteLayoutEDI clienteLayoutEDI, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoasLayoutEDI grupoPessoasLayoutEDI)
        {

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado() { TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema };

            Servicos.Embarcador.Pedido.Pedido svcPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork);
            Servicos.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP servicoIntegracaoEDI = new Servicos.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP(unitOfWork);

            if ((clienteLayoutEDI != null && (clienteLayoutEDI.UtilizarLeituraArquivos && clienteLayoutEDI.AdicionarEDIFilaProcessamento)) || (grupoPessoasLayoutEDI != null && (grupoPessoasLayoutEDI.UtilizarLeituraArquivos && grupoPessoasLayoutEDI.AdicionarEDIFilaProcessamento)))
            {
                Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP integracaoProcessamentoEDIFTP = new Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP();
                Repositorio.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP repIntegracao = new Repositorio.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP(unitOfWork);
                dynamic retornoArquivo = servicoIntegracaoEDI.SalvarArquivoImportacaoTemporario(arquivo, arquivoDisponivel, unitOfWork);

                integracaoProcessamentoEDIFTP.Cliente = clienteLayoutEDI != null ? clienteLayoutEDI.Cliente : null;
                integracaoProcessamentoEDIFTP.DataIntegracao = DateTime.Now;
                integracaoProcessamentoEDIFTP.GrupoPessoas = grupoPessoasLayoutEDI != null ? grupoPessoasLayoutEDI.GrupoPessoas : null;
                integracaoProcessamentoEDIFTP.LayoutEDI = layoutEDI;
                integracaoProcessamentoEDIFTP.GuidArquivo = retornoArquivo.Token;
                integracaoProcessamentoEDIFTP.NomeArquivo = retornoArquivo.NomeOriginal;
                integracaoProcessamentoEDIFTP.MensagemRetorno = "";
                integracaoProcessamentoEDIFTP.NumeroTentativas = 0;
                integracaoProcessamentoEDIFTP.SituacaoIntegracaoEDIFTP = SituacaoIntegracaoProcessamentoEDIFTP.AgIntegracao;

                repIntegracao.Inserir(integracaoProcessamentoEDIFTP);

                return;
            }

            string retorno = "";
            bool notasProcessadas = svcPedido.ImportarNotasFiscaisNOTFIS(layoutEDI, arquivo, null, _tipoServicoMultisoftware, auditado, unitOfWork, out retorno, adminUnitOfWork);
            if (!string.IsNullOrWhiteSpace(retorno))
            {
                Servicos.Log.TratarErro("Retorno leitura EDI via FTP " + retorno);
                if (!string.IsNullOrWhiteSpace(emailErroProcessamentoEDI))
                    enviarEmailProblemaImportarCargaEDI(arquivoDisponivel, retorno, emailErroProcessamentoEDI, unitOfWork);
            }
        }

        private async Task ProcessarArquivoXMLAsync(System.IO.Stream arquivo, string arquivoDisponivel, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            try
            {
                Servicos.Embarcador.CTe.CTEsImportados svcCTeImportado = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork, _tipoServicoMultisoftware, cancellationToken);

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = null;

                try
                {
                    xmlNotaFiscal = await ProcessarXMLNFeAsync(arquivo, "", "", unitOfWork, cancellationToken, true, false);
                }
                catch (ServicoException ex)
                {
                    Servicos.Log.TratarErro("arquivo disponivel " + arquivoDisponivel, "XMLEmail");
                    Servicos.Log.TratarErro("erro: " + ex.ToString(), "XMLEmail");
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("arquivo disponivel " + arquivoDisponivel, "XMLEmail");
                    Servicos.Log.TratarErro("erro: " + ex.ToString(), "XMLEmail");
                }

                if (xmlNotaFiscal == null)
                {
                    if (!Servicos.Embarcador.CTe.CTe.ProcessarXMLCTe(arquivo, unitOfWork, unitOfWork.StringConexao, _tipoServicoMultisoftware, ""))
                    {
                        if (!await svcCTeImportado.ProcessarXMLMDFeAsync(arquivo, _auditado))
                        {
                            await ProcessarEDINotFisAsync(arquivo, unitOfWork, "", "", cancellationToken);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("arquivo disponivel " + arquivoDisponivel, "XMLEmail");
                Servicos.Log.TratarErro("erro: " + ex, "XMLEmail");
            }
        }

        private async Task ProcessarArquivoEDIAsync(System.IO.Stream arquivo, Dominio.Entidades.LayoutEDI layoutEDI, string arquivoDisponivel, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await repositorioConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = await new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork, cancellationToken).BuscarPrimeiroRegistroAsync();

            Servicos.Embarcador.Pedido.Pedido srvicoPedido = new Servicos.Embarcador.Pedido.Pedido(unitOfWork, cancellationToken);

            string retorno = "";

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = srvicoPedido.ImportarPedidoNOTFIS(layoutEDI, arquivo, null, _tipoServicoMultisoftware, _clienteMultisoftware, _auditado, unitOfWork, out retorno, configuracaoGeralCarga);

            if (!string.IsNullOrWhiteSpace(retorno))
            {
                Servicos.Log.TratarErro("Retorno leitura EDI via FTP/E-mail: " + retorno, "XMLEmail");
                if (!string.IsNullOrWhiteSpace(configuracaoTMS.EmailsRetornoProblemaGerarCargaEmail))
                    enviarEmailProblemaImportarCargaEDI(arquivoDisponivel, retorno, configuracaoTMS.EmailsRetornoProblemaGerarCargaEmail, unitOfWork);
            }


            await unitOfWork.StartAsync();

            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repositorioControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);

            Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI = new Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI();

            controleIntegracaoCargaEDI.Data = DateTime.Now;
            controleIntegracaoCargaEDI.MensagemRetorno = "";
            controleIntegracaoCargaEDI.NumeroDT = cargas != null ? string.Join(",", (from obj in cargas select obj.CodigoCargaEmbarcador).ToList()) : "";
            controleIntegracaoCargaEDI.NomeArquivo = arquivoDisponivel;
            controleIntegracaoCargaEDI.MensagemRetorno = retorno;
            controleIntegracaoCargaEDI.GuidArquivo = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(arquivoDisponivel);
            controleIntegracaoCargaEDI.NumeroTentativas = 1;
            controleIntegracaoCargaEDI.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Integrado;
            controleIntegracaoCargaEDI.Cargas = cargas;

            await repositorioControleIntegracaoCargaEDI.InserirAsync(controleIntegracaoCargaEDI);

            MoverParaPastaProcessados(controleIntegracaoCargaEDI.GuidArquivo, arquivo, unitOfWork);

            await unitOfWork.CommitChangesAsync();
        }

        private async Task enviarEmailProblemaCTeAsync(string remetente, string conteudo, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork, cancellationToken);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = await repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivoAsync();

            if (email != null)
            {
                Servicos.Email servicoEmail = new Servicos.Email(unitOfWork);

                List<System.Net.Mail.Attachment> anexos = null;

                conteudo += "<hr/>";

                if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                {
                    conteudo += email.MensagemRodape.Replace("#qLinha#", "<br/>");
                }

                servicoEmail.EnviarEmail(email.Email, email.Email, email.Senha, remetente, "", "", "Divergências ao processar o CT-e", conteudo, email.Smtp, anexos, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork);
            }
        }

        private string enviarEmailProblemaImportarCargaNFe(string chaveNFe, string mensagem, string emails, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();
            string mensagemErro = "";

            if (email != null)
            {
                string assunto = chaveNFe + " não importada.";

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("<p>Não foi possivel criar a carga da nota fiscal ").Append(chaveNFe).Append("<br /><br />");

                sb.Append("<p>Motivo: ").Append(mensagem).Append("</p><br /><br />");

                sb.Append("<p>*E-mail enviado automáticamente, favor não responder.</p>");
                Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails, null, null, assunto, sb.ToString(), email.Smtp, out mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeTrabalho);

            }
            return mensagemErro;
        }

        private string enviarEmailProblemaImportarCargaEDI(string edi, string mensagem, string emails, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();
            string mensagemErro = "";

            if (email != null)
            {
                string assunto = edi + " não importada.";

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("<p>Não foi possivel criar a carga do arquivo EDI ").Append(edi).Append("<br /><br />");

                sb.Append("<p>Motivo: ").Append(mensagem).Append("</p><br /><br />");

                sb.Append("<p>*E-mail enviado automáticamente, favor não responder.</p>");


                List<string> cc = new List<string>();

                string[] emailsSplit = emails.Split(';');
                if (emailsSplit.Length > 0)
                {
                    emails = emailsSplit[0];
                    for (int i = 1; i < emailsSplit.Length; i++)
                        cc.Add(emailsSplit[i]);
                }

                Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails, null, emailsSplit, assunto, sb.ToString(), email.Smtp, out mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeTrabalho);

            }
            return mensagemErro;
        }

        #endregion
    }
}