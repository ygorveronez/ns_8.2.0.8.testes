using System;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool SolicitarDacte(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string emails = null)
        {
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.solicitarDacte envioWS = this.obterSolicitarDacte(cte, emails);

                //Transmitir
                var retornoWS = this.TransmitirEmissor(Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTipoWS.POST, envioWS, $"cte-v4/generate-dacte", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPICte);

                if (retornoWS.erro)
                {
                    string mensagemErro = string.Concat("Emissor NSTech: Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);

                    Servicos.Log.TratarErro(mensagemErro);
                    Servicos.Log.TratarErro(Newtonsoft.Json.JsonConvert.SerializeObject(retornoWS));

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Exception Solicitar Dacte");
                Servicos.Log.TratarErro(ex);

                throw;
            }
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.solicitarDacte obterSolicitarDacte(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string emails = null)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.solicitarDacte retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.solicitarDacte();
            retorno.by = "externalId";
            retorno.externalId = cte.Codigo.ToString();
            retorno.notifications = this.obterEnvioCteOptionsDacteNotifications(cte, emails);
            return retorno;
        }

        #endregion Métodos Globais

        #region Métodos Privados

        #endregion Métodos Privados
    }
}
