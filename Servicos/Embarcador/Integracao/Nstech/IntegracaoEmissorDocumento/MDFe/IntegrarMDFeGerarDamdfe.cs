using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Servicos.Extensions;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool SolicitarDamdfe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            bool sucesso = false;
            string mensagemErro = string.Empty;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.solicitarDamdfe envioWS = this.obterSolicitarDamdfe(mdfe, unitOfWork);

                //Transmitir
                var retornoWS = this.TransmitirEmissor(Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTipoWS.POST, envioWS, $"mdfe-v3/generate-damdfe", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPIMDFe);

                if (retornoWS.erro)
                {
                    mensagemErro = string.Concat("Emissor NSTech: Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta de geração de DAMDFE Nstech: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Emissor NSTech: Ocorreu uma falha ao efetuar ao solicitar a Damdfe; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        var RetornoConsulta = retorno[0];
                        sucesso = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Exception Solicitar Damdfe");
                Servicos.Log.TratarErro(ex);

                throw;
            }

            return sucesso;
        }

        #endregion Métodos Globais

        #region Métodos Privados

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.solicitarDamdfe obterSolicitarDamdfe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.solicitarDamdfe retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.solicitarDamdfe();
            retorno.by = "externalId";
            retorno.externalId = mdfe.Codigo.ToString();
            retorno.notifications = this.obterEnvioMDFeOptionsDamdfeNotifications(mdfe, unitOfWork);
            return retorno;
        }

        #endregion Métodos Privados
    }
}
