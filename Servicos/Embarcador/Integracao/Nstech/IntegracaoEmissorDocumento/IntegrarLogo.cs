using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento;
using Servicos.Extensions;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool IncluirAtualizarLogo(out string mensagemErro, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = string.Empty;
            bool sucesso = false;

            try
            {
                string logoBase64 = this.ObterLogoBase64(empresa);

                if (string.IsNullOrEmpty(logoBase64))
                    return true;

                logo envioWS = this.ObterIncluirAtualizarLogo(empresa, logoBase64);

                //Transmite
                var retornoWS = this.TransmitirEmissor(enumTipoWS.POST, envioWS, "logo", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPILogo);

                if (retornoWS.erro)
                {
                    mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta do envio de logo Nstech: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar ao enviar os dados do Logo; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        sucesso = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                mensagemErro = "Ocorreu uma falha ao integrar os dados do Logo.";
                sucesso = false;
            }

            return sucesso;
        }

        #endregion

        #region Métodos Privados

        private logo ObterIncluirAtualizarLogo(Dominio.Entidades.Empresa empresa, string logoBase64)
        {
            logo retorno = new logo();

            retorno.issuerDocument = empresa.CNPJ_SemFormato;
            retorno.logobase64 = logoBase64;

            return retorno;
        }

        private string ObterLogoBase64(Dominio.Entidades.Empresa empresa)
        {
            string caminho = empresa.CaminhoLogoDacte;
            if (string.IsNullOrWhiteSpace(caminho) || !Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                return "";

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        #endregion
    }
}
