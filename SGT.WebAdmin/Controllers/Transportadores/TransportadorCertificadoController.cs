using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize("Transportadores/TransportadorCertificado")]
    public class TransportadorCertificadoController : BaseController
    {
		#region Construtores

		public TransportadorCertificadoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> EnviarCertificado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.Entidades.Empresa empresa;

                int codigoTransportador = Usuario?.Empresa?.Codigo ?? 0;

                string senha = Request.Params("SenhaCertificado");

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                empresa = repEmpresa.BuscarPorCodigo(codigoTransportador);

                if (empresa == null)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.TransportadorNaoFoiEncontrado);

                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.SelecioneUmCertificadoParaEnvio);

                Servicos.DTO.CustomFile file = files[0];

                if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".pfx")
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.ExtensaoDoArquivoInvalidaSelecioneUmArquivoComExtensaoPfx);

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosEmpresas, Utilidades.String.OnlyNumbers(empresa.CNPJ), "Certificado");

                caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, empresa.CNPJ + ".pfx");

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                    Utilidades.IO.FileStorageService.Storage.Delete(caminho);

                file.SaveAs(caminho);

                byte[] certificadoDigitalByteArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);

                /// Flag MachineKeySet não tem suporte no linux, em testes ocorreu erro.
                /// Flag DefaultKeySet ocorreu erro em alguns ambientes Windows, acredito que o motivo seja por que tenta usar o perfil do usuário atual e esteja faltando alguma permissão.
                X509Certificate2 certificado = null;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    certificado = new X509Certificate2(certificadoDigitalByteArray, senha, X509KeyStorageFlags.DefaultKeySet
                                            | X509KeyStorageFlags.PersistKeySet
                                            | X509KeyStorageFlags.Exportable);
                else
                    certificado = new X509Certificate2(certificadoDigitalByteArray, senha, X509KeyStorageFlags.MachineKeySet
                                             | X509KeyStorageFlags.PersistKeySet
                                             | X509KeyStorageFlags.Exportable);

                var cnpjCpfCertificado = certificado.ObterCnpj();
                if (!string.IsNullOrWhiteSpace(cnpjCpfCertificado) && cnpjCpfCertificado != empresa.CNPJ.Substring(0, 8))
                    return new JsonpResult(false, false, Localization.Resources.Transportadores.Transportador.CNPJDoCertificado + " (" + cnpjCpfCertificado + ") " + Localization.Resources.Transportadores.Transportador.NaoPertenceAoCNPJDaEmpresa + "(" + empresa.CNPJ.Substring(0, 8) + ").");

                if (string.IsNullOrWhiteSpace(cnpjCpfCertificado))
                {
                    cnpjCpfCertificado = certificado.ObterCpf();

                    if (string.IsNullOrWhiteSpace(cnpjCpfCertificado))
                        return new JsonpResult(false, false, Localization.Resources.Transportadores.Transportador.NaofoiPossivelFdentificarEmpresaDoCertificadoDigitalFavorVerificarCertificadoSelecionado + " \n\n" + Localization.Resources.Transportadores.Transportador.DuvidasEntrarEmContatoComSuporte);
                    else if (cnpjCpfCertificado != empresa.CNPJ)
                        return new JsonpResult(false, false, Localization.Resources.Transportadores.Transportador.CNPJDoCertificado + "(" + cnpjCpfCertificado + ") " + Localization.Resources.Transportadores.Transportador.NaoPertenceAoCPFDoEmissor + " (" + empresa.CNPJ + ").");
                }

                empresa.SenhaCertificado = senha;
                empresa.SerieCertificado = certificado.SerialNumber;
                empresa.DataInicialCertificado = certificado.NotBefore;
                empresa.DataFinalCertificado = certificado.NotAfter;
                empresa.Initialize();
                empresa.NomeCertificado = caminho;

                repEmpresa.Atualizar(empresa);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, empresa.GetChanges(), Localization.Resources.Transportadores.Transportador.AtualizouCertificadoDaEmpresa, unitOfWork);

                Servicos.AtualizacaoEmpresa svcAtualizaEmpresa = new Servicos.AtualizacaoEmpresa(unitOfWork);
                svcAtualizaEmpresa.Atualizar(empresa, unitOfWork);

                if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoCTeType == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.NSTech || Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoMDFeType == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.NSTech)
                {
                    string mensagemErro = string.Empty;
                    Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech integracaoNStech = new Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unitOfWork);
                    if (!integracaoNStech.IncluirAtualizarCertificado(out mensagemErro, empresa, unitOfWork))
                    {
                        Servicos.Log.TratarErro($"Ocorreu um erro ao enviar o certificado para o emissor NStech: {mensagemErro}");
                        return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoSalvarCertificado);
                    }
                }

                //Enviar Certificado Digital Key Vault
                svcAtualizaEmpresa.EnviarCertificadoKeyVault(empresa, ClienteAcesso, unitOfWork);

                return new JsonpResult(new
                {
                    empresa.SerieCertificado,
                    DataInicialCertificado = empresa.DataInicialCertificado.Value.ToString("dd/MM/yyyy"),
                    DataFinalCertificado = empresa.DataFinalCertificado.Value.ToString("dd/MM/yyyy"),
                    empresa.SenhaCertificado
                });
            }
            catch (System.Security.Cryptography.CryptographicException ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.SenhaInvalidaNaofoiPossivelExtrairAsInformacoesDoCertificado);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoSalvarCertificado);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverCertificado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTransportador = Usuario?.Empresa?.Codigo ?? 0;

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoTransportador);

                if (empresa == null)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.TransportadorNaoEncontrado);

                if (!string.IsNullOrWhiteSpace(empresa.NomeCertificado) && Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado))
                    Utilidades.IO.FileStorageService.Storage.Delete(empresa.NomeCertificado);

                if (!string.IsNullOrEmpty(empresa.NomeCertificadoKeyVault) && Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnviarCertificadoKeyVault.Value)
                {
                    Servicos.SecretManagement.ISecretManager secretManager = new Servicos.SecretManagement.AzureKeyVaultSecretManager();
                    secretManager.DeleteCertificate(empresa.NomeCertificadoKeyVault);
                }

                empresa.DataInicialCertificado = null;
                empresa.DataFinalCertificado = null;
                empresa.SerieCertificado = string.Empty;
                empresa.SenhaCertificado = string.Empty;
                empresa.Initialize();
                empresa.NomeCertificado = string.Empty;
                empresa.NomeCertificadoKeyVault = string.Empty;

                repEmpresa.Atualizar(empresa);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, empresa.GetChanges(), Localization.Resources.Transportadores.Transportador.RemoveuCertificadoDaEmpresa, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoRemoverCertificado);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadCertificado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTransportador = Usuario?.Empresa?.Codigo ?? 0;

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoTransportador);

                if (empresa == null)
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.TransportadorNaoEncontrado);

                if (string.IsNullOrWhiteSpace(empresa.NomeCertificado) || !Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado))
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.CertificadoNaoEncontrado);

                byte[] arquivoRetorno = null;
                if (!string.IsNullOrEmpty(empresa.NomeCertificadoKeyVault) && Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().EnviarCertificadoKeyVault.Value)
                {
                    Servicos.SecretManagement.ISecretManager secretManager = new Servicos.SecretManagement.AzureKeyVaultSecretManager();
                    var certificado = secretManager.GetCertificate(empresa.NomeCertificadoKeyVault);
                    if (certificado != null)
                        arquivoRetorno = certificado.Export(X509ContentType.Pfx, empresa.SenhaCertificado);
                }

                if (arquivoRetorno == null)
                    arquivoRetorno = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(empresa.NomeCertificado);

                return Arquivo(arquivoRetorno, "application/x-pkcs12", empresa.CNPJ + ".pfx");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoObterCertificadoDigitalDoTransportador);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Usuario?.Empresa?.Codigo ?? 0;

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigo);

                if (empresa == null)
                    return new JsonpResult(false, "Usuário não possui transportador vinculado");

                var dynEmpresa = new
                {
                    DataInicialCertificado = empresa.DataInicialCertificado != null ? empresa.DataInicialCertificado.Value.ToString("dd/MM/yyyy") : "",
                    DataFinalCertificado = empresa.DataFinalCertificado != null ? empresa.DataFinalCertificado.Value.ToString("dd/MM/yyyy") : "",
                    empresa.SerieCertificado,
                    empresa.SenhaCertificado,
                    PossuiCertificado = !string.IsNullOrWhiteSpace(empresa.NomeCertificado) ? Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado) : false,
                };

                return new JsonpResult(dynEmpresa);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}
