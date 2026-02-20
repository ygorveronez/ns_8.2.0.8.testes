using Dominio.Interfaces.Database;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Servicos
{
    public class AtualizacaoEmpresa : ServicoBase
    {       
        public AtualizacaoEmpresa(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        #region Métodos Publicos

        public string Atualizar(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                ServicoCTe.uCteServiceTSSoapClient svcCTe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoCTe.uCteServiceTSSoapClient, ServicoCTe.uCteServiceTSSoap>(TipoWebServiceIntegracao.Oracle_uCTeServiceTS);

                Repositorio.ConfiguracaoAutDownloadXML repConfiguracaoAutDownloadXML = new Repositorio.ConfiguracaoAutDownloadXML(unitOfWork);

                ServicoCTe.Empresa emp = new ServicoCTe.Empresa();

                emp.Bairro = Utilidades.String.Left(empresa.Bairro, 60);
                emp.Cep = Utilidades.String.Left(Utilidades.String.OnlyNumbers(empresa.CEP), 10);
                emp.Cidade = Utilidades.String.Left(empresa.Localidade.Descricao, 60);
                emp.CNPJ = Utilidades.String.OnlyNumbers(empresa.CNPJ);
                emp.CodigoCidadeIBGE = empresa.Localidade.CodigoIBGE;
                emp.CodigoInterno = 0;
                emp.Complemento = Utilidades.String.Left(empresa.Complemento, 60);
                emp.IE = empresa.InscricaoEstadual;
                emp.IE_ST = empresa.Inscricao_ST;
                emp.IM = empresa.InscricaoMunicipal;
                emp.Logradouro = Utilidades.String.Left(empresa.Endereco, 255);
                emp.NomeRazao = Utilidades.String.Left(empresa.RazaoSocial, 60);
                emp.NomeFantasia = Utilidades.String.Left(empresa.NomeFantasia, 60);
                emp.Numero = Utilidades.String.Left(empresa.Numero, 60);
                emp.UF = empresa.Localidade.Estado.Sigla;
                emp.Telefone = Utilidades.String.Left(Utilidades.String.OnlyNumbers(empresa.Telefone), 12);
                emp.SenhaNFSe = empresa.Configuracao != null ? empresa.Configuracao.SenhaNFSe : string.Empty;
                emp.FraseSecretaNFSe = empresa.Configuracao != null ? empresa.Configuracao.FraseSecretaNFSe : string.Empty;
                emp.URLPortalNFSe = empresa.Configuracao != null ? empresa.Configuracao.URLPrefeituraNFSe : string.Empty;
                emp.NaoUtilizarLoteIncremental = empresa.NaoIncrementarNumeroLoteRPSAutomaticamente ? 1 : 0;
                emp.UtilizaNFSeNacional = empresa.NFSeNacional ? 1 : 0;

                if (!string.IsNullOrWhiteSpace(empresa.NomeCertificado) || !string.IsNullOrWhiteSpace(empresa.SerieCertificado))
                {
                    emp.NomeArquivoCertificado = empresa.NomeCertificado;
                    emp.DataValidadeInicio = empresa.DataInicialCertificado.Value.ToString("dd/MM/yyyy");
                    emp.DataValidadeFim = empresa.DataFinalCertificado.Value.ToString("dd/MM/yyyy");
                    emp.NumeroSerial = empresa.SerieCertificado;
                    emp.SenhaArquivoCertificado = empresa.SenhaCertificado;
                }

                if (!string.IsNullOrWhiteSpace(empresa.FusoHorario))
                {
                    TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);

                    bool horarioVerao = timezone.IsDaylightSavingTime(DateTime.Now);

                    if (timezone != null)
                        emp.FusoHorario = horarioVerao ? timezone.BaseUtcOffset.Hours + 1 : timezone.BaseUtcOffset.Hours;
                    else
                        emp.FusoHorario = -3;
                }
                else
                {
                    emp.FusoHorario = -3;
                }

                emp.NomeContador = Utilidades.String.Left(empresa.NomeContador, 60);
                emp.TelefoneContador = Utilidades.String.Left(Utilidades.String.OnlyNumbers(empresa.TelefoneContador), 12);
                string emails = string.Empty;
                emp.EnviaEmailEmitente = "N";

                if (empresa.StatusEmail == "A")
                {
                    emails += string.Concat(empresa.Email, ";");
                    emp.EnviaEmailEmitente = "S";
                }
                if (empresa.StatusEmailAdministrativo == "A")
                {
                    emails += empresa.EmailAdministrativo;
                    emp.EnviaEmailEmitente = "S";
                }

                emp.EmailEmitente = Utilidades.String.Left(emails, 1000);
                emp.EmailContador = Utilidades.String.Left(empresa.EmailContador, 1000);
                emp.EnviaEmailContador = empresa.StatusEmailContador == "A" ? "S" : "N";
                emp.Status = empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao && empresa.Status == "A" ? "P" : "H";
                emp.CaminhoLogo = empresa.CaminhoLogoDacte;
                emp.TipoTransportador = empresa.TipoTransportador == Dominio.Enumeradores.TipoTransportador.CTC ? "3" : empresa.TipoTransportador == Dominio.Enumeradores.TipoTransportador.TAC ? "2" : "1";
                emp.TAF = empresa.TAF;
                emp.NroRegEstadual = empresa.NroRegEstadual;
                emp.Cnae = empresa.CNAE;
                emp.cpfNFSe = empresa.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.Configuracao.NFSeCPF) ? empresa.Configuracao.NFSeCPF : string.Empty;
                emp.UtilizaCapicom = empresa.AssinaturaCapicom ? "S" : "N";
                emp.CertificadoA3 = empresa.CertificadoA3 ? "S" : "N";

                List<Dominio.Entidades.ConfiguracaoAutDownloadXML> autDownloadXML = new List<Dominio.Entidades.ConfiguracaoAutDownloadXML>();
                if (empresa.Configuracao != null)
                {
                    emp.CodigoSeguroATM = !string.IsNullOrWhiteSpace(empresa.Configuracao.CodigoSeguroATM) ? empresa.Configuracao.CodigoSeguroATM : empresa.EmpresaPai?.Configuracao?.CodigoSeguroATM;
                    emp.UsuarioSeguroATM = !string.IsNullOrWhiteSpace(empresa.Configuracao.UsuarioSeguroATM) ? empresa.Configuracao.UsuarioSeguroATM : empresa.EmpresaPai?.Configuracao?.UsuarioSeguroATM;
                    emp.SenhaSeguroATM = !string.IsNullOrWhiteSpace(empresa.Configuracao.SenhaSeguroATM) ? empresa.Configuracao.SenhaSeguroATM : empresa.EmpresaPai?.Configuracao?.SenhaSeguroATM;
                    emp.AverbaAutomaticoATM = empresa.Configuracao.AverbaAutomaticoATM != null && empresa.Configuracao.AverbaAutomaticoATM > 0 ? empresa.Configuracao.AverbaAutomaticoATM.ToString() : empresa.EmpresaPai?.Configuracao?.AverbaAutomaticoATM.ToString();
                    autDownloadXML = repConfiguracaoAutDownloadXML.BuscarPorConfiguracao(empresa.Configuracao.Codigo);
                    emp.AssinaturaEmail = !string.IsNullOrWhiteSpace(empresa.Configuracao.AssinaturaEmail) ? empresa.Configuracao.AssinaturaEmail : empresa.EmpresaPai != null && empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.EmpresaPai.Configuracao.AssinaturaEmail) ? empresa.EmpresaPai.Configuracao.AssinaturaEmail : string.Empty;
                    emp.SeguradoraAverbacao = empresa.Configuracao.SeguradoraAverbacao != null ? empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.Quorum ? "B" : empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro ? "P" : empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ATM ? "A" : string.Empty : string.Empty;
                    if (string.IsNullOrWhiteSpace(emp.SeguradoraAverbacao))
                        emp.SeguradoraAverbacao = empresa.EmpresaPai?.Configuracao != null && empresa.EmpresaPai?.Configuracao.SeguradoraAverbacao != null ? empresa.EmpresaPai?.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ATM ? "A" : empresa.EmpresaPai.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.Quorum ? "B" : empresa.EmpresaPai.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro ? "P" : string.Empty : string.Empty;
                    emp.TokenBradesco = !string.IsNullOrWhiteSpace(empresa.Configuracao.TokenAverbacaoBradesco) ? empresa.Configuracao.TokenAverbacaoBradesco : empresa.EmpresaPai?.Configuracao != null && !string.IsNullOrWhiteSpace(empresa.EmpresaPai.Configuracao.TokenAverbacaoBradesco) ? empresa.EmpresaPai.Configuracao.TokenAverbacaoBradesco : string.Empty;
                    emp.TokenIntegracao = !string.IsNullOrWhiteSpace(empresa.Configuracao.TokenIntegracaoCTe) ? empresa.Configuracao.TokenIntegracaoCTe : string.Empty;
                    emp.EmailSemTexto = empresa.Configuracao.EmailSemTexto ? "S" : "N";
                    emp.WsdlQuorum = !string.IsNullOrWhiteSpace(empresa.Configuracao.WsdlAverbacaoQuorum) ? empresa.Configuracao.WsdlAverbacaoQuorum : string.Empty;
                }
                else if (empresa.EmpresaPai?.Configuracao != null)
                {
                    emp.CodigoSeguroATM = empresa.EmpresaPai.Configuracao?.CodigoSeguroATM;
                    emp.UsuarioSeguroATM = empresa.EmpresaPai.Configuracao?.UsuarioSeguroATM;
                    emp.SenhaSeguroATM = empresa.EmpresaPai.Configuracao?.SenhaSeguroATM;
                    emp.AverbaAutomaticoATM = empresa.EmpresaPai.Configuracao?.AverbaAutomaticoATM.ToString();
                    emp.SeguradoraAverbacao = empresa.EmpresaPai.Configuracao?.SeguradoraAverbacao != null ? empresa.EmpresaPai.Configuracao?.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.Quorum ? "B" : empresa.EmpresaPai.Configuracao?.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro ? "P" : empresa.EmpresaPai.Configuracao?.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ATM ? "A" : string.Empty : string.Empty;
                    emp.TokenBradesco = empresa.EmpresaPai.Configuracao?.TokenAverbacaoBradesco.ToString();
                    emp.TokenIntegracao = !string.IsNullOrWhiteSpace(empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe) ? empresa.EmpresaPai.Configuracao.TokenIntegracaoCTe : string.Empty;
                    emp.EmailSemTexto = empresa.EmpresaPai.Configuracao.EmailSemTexto ? "S" : "N";
                }

                if (autDownloadXML.Count() > 0)
                {
                    for (var i = 0; i < autDownloadXML.Count; i++)
                        emp.CNPJAutorizado = string.IsNullOrWhiteSpace(emp.CNPJAutorizado) ? autDownloadXML[i].CNPJCPF : string.Concat(emp.CNPJAutorizado, ";", autDownloadXML[i].CNPJCPF);
                }


                ServicoCTe.ResultadoInteger retorno = svcCTe.CadastraEmpresa(emp);

                if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnviarCertificadoOracle.Value)
                {
                    try
                    {
                        if (Utilidades.IO.FileStorageService.Storage.Exists(emp.NomeArquivoCertificado))
                        {
                            Byte[] arquivo = null;

                            using (Stream file = Utilidades.IO.FileStorageService.Storage.OpenRead(emp.NomeArquivoCertificado))
                            {
                                int len = (int)file.Length;
                                arquivo = new Byte[len];
                                file.Read(arquivo, 0, len);
                            }

                            ServicoCTe.ResultadoString retornoCertificado = svcCTe.EnviarArquivoCertificadoDigital(arquivo, emp.NomeArquivoCertificado);
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Erro ao enviar certificado digital para oracle da empresa " + empresa.CNPJ + ": " + ex);
                    }
                }

                return string.Concat(retorno.Info.Mensagem, "-", retorno.Info.MensagemOriginal);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro ao integrar empresa " + empresa.CNPJ + ": " + ex);
                return string.Concat("Erro ao integrar empresa " + empresa.CNPJ);
            }
        }

        public bool EnviarCertificadoKeyVault(Dominio.Entidades.Empresa empresa, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, Repositorio.UnitOfWork unitOfWork)
        {
#if DEBUG
            return true;
#endif

            if (Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnviarCertificadoKeyVault ?? false)
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Servicos.SecretManagement.ISecretManager secretManager = new Servicos.SecretManagement.AzureKeyVaultSecretManager();
                X509Certificate2 certificado = null;
                if (!string.IsNullOrEmpty(empresa.NomeCertificadoKeyVault))
                    certificado = secretManager.GetCertificate(empresa.NomeCertificadoKeyVault);

                bool certificadoComDiferenteDeExpiracao = false;
                if (certificado != null)
                {
                    X509Certificate2 certificadoNovo = new X509Certificate2(empresa.NomeCertificado, empresa.SenhaCertificado);
                    if (certificadoNovo != null && (certificadoNovo.NotAfter.Date != certificado.NotAfter.Date))
                        certificadoComDiferenteDeExpiracao = true;
                }

                if (certificado == null || certificadoComDiferenteDeExpiracao)
                {
                    if (certificado != null)
                        secretManager.DeleteCertificate(empresa.NomeCertificadoKeyVault);

                    string nomeDoCertificadoNoKeyVaultNovo = $"{(clienteAcesso.URLHomologacao ? "H" : "P")}{clienteAcesso.Cliente.Codigo}-{empresa.CNPJ}-{Guid.NewGuid().ToString()}";

                    if (secretManager.SendCertificate(empresa.NomeCertificado, empresa.SenhaCertificado, nomeDoCertificadoNoKeyVaultNovo))
                    {
                        empresa.NomeCertificadoKeyVault = nomeDoCertificadoNoKeyVaultNovo;
                        repEmpresa.Atualizar(empresa);
                    }
                }
            }

            return true;
        }

        #endregion

    }
}
