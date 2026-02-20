using Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento;
using Servicos.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool IncluirAtualizarCertificado(out string mensagemErro, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork, bool atualizarCerticado = true)
        {
            mensagemErro = string.Empty;
            string idIntegracao = string.Empty;
            bool sucesso = false;

            try
            {
                if (!Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado))
                {
                    Servicos.Log.TratarErro($"{empresa.CNPJ} O arquivo do certificado não existe em {empresa.NomeCertificado}", "CertificadoNstech");

                    return true;
                }

                Byte[] arquivoCertificado = null;
                using (Stream file = Utilidades.IO.FileStorageService.Storage.OpenRead(empresa.NomeCertificado))
                {
                    int len = (int)file.Length;
                    arquivoCertificado = new Byte[len];
                    file.Read(arquivoCertificado, 0, len);
                }

                if (arquivoCertificado == null)
                    return true;

                if (!this.ConsultarCertificado(out mensagemErro, out idIntegracao, empresa))
                    return false;

                if (!string.IsNullOrEmpty(idIntegracao))
                {
                    if (!atualizarCerticado)
                        return true;

                    if (!this.RemoverCertificado(out mensagemErro, empresa, idIntegracao))
                        return false;
                }

                certificadoDigital envioWS = this.ObterIncluirAtualizarCertificado(arquivoCertificado, empresa);

                //Transmite
                var retornoWS = this.TransmitirEmissor(enumTipoWS.POST, envioWS, "certificate", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPICertificado);

                if (retornoWS.erro)
                {
                    dynamic retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<dynamic>();

                        if (retorno?.type == "certificate_exists")
                            return true;
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar JSON de retorno do webservice Nstech - IntegrarCertificado: {ex.ToString()}", "CatchNoAction");
                    }

                    mensagemErro = string.Concat($"{empresa.CNPJ} Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    sucesso = false;
                }
                else
                {
                    dynamic retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<dynamic>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta do envio de certificado digital Nstech: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format($"{empresa.CNPJ} Message: Ocorreu uma falha ao efetuar o envio do certificado digital; RetornoWS {{0}}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        idIntegracao = (string)retorno.id;
                        sucesso = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = $"{empresa.CNPJ} Ocorreu uma falha ao integrar o certificado digital.";
                sucesso = false;
            }

            return sucesso;
        }

        public bool RemoverCertificado(out string mensagemErro, Dominio.Entidades.Empresa empresa, string idIntegracao)
        {
            mensagemErro = string.Empty;
            bool sucesso = false;

            try
            {
                if (string.IsNullOrEmpty(idIntegracao))
                {
                    if (!this.ConsultarCertificado(out mensagemErro, out idIntegracao, empresa))
                        return false;
                }

                if (string.IsNullOrEmpty(idIntegracao))
                    return true;

                //Transmite
                var retornoWS = this.TransmitirEmissor(enumTipoWS.DELETE, null, $@"certificate?by=externalId&externalId={empresa.Codigo}", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPICertificado);

                if (retornoWS.erro)
                {
                    mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    sucesso = false;
                }
                else
                    sucesso = true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao remover o certificado digital.";
                sucesso = false;
            }

            return sucesso;
        }

        #endregion

        #region Métodos Privados

        private bool ConsultarCertificado(out string mensagemErro, out string idIntegracao, Dominio.Entidades.Empresa empresa)
        {
            mensagemErro = string.Empty;
            idIntegracao = string.Empty;
            bool sucesso = false;

            try
            {
                object envioWS = null;

                //Transmitir
                var retornoWS = this.TransmitirEmissor(enumTipoWS.GET, envioWS, $"certificate?by=externalId&externalId={empresa.Codigo}", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPICertificado);

                if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    sucesso = false;
                }
                else
                {
                    if (retornoWS.erro && retornoWS.StatusCode == 404)
                    {
                        sucesso = true;
                    }
                    else if (retornoWS.erro)
                    {
                        mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                        sucesso = false;
                    }
                    else
                    {
                        List<certificadoDigital> retorno = null;

                        try
                        {
                            retorno = retornoWS.jsonRetorno.FromJson<List<certificadoDigital>>();
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar consulta de certificado digital Nstech: {ex.ToString()}", "CatchNoAction");
                        }

                        if (retorno == null)
                        {
                            mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar a consulta do certificado digital; RetornoWS {0}.", retornoWS.jsonRetorno);
                            sucesso = false;
                        }
                        else
                        {
                            certificadoDigital retIdIntegracao = retorno.FirstOrDefault();
                            if (retIdIntegracao != null)
                                idIntegracao = retIdIntegracao.id;
                            sucesso = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao efetuar a consulta do certificado digital.";
                sucesso = false;
            }

            return sucesso;
        }

        private certificadoDigital ObterIncluirAtualizarCertificado(Byte[] arquivoCertificado, Dominio.Entidades.Empresa empresa)
        {
            certificadoDigital retorno = new certificadoDigital();

            retorno.externalId = empresa.Codigo.ToString();

            retorno.ownerDocument = empresa.CNPJ;

            retorno.certificate = new certificadoDigital.certificateCertificate()
            {
                type = "pfx",
                password = empresa.SenhaCertificado,
                certificate = Convert.ToBase64String(arquivoCertificado)
            };

            return retorno;
        }

        #endregion
    }
}
