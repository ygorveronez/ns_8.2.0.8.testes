using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Servicos.Extensions;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool ConsultarCCe(ref Dominio.Entidades.CartaDeCorrecaoEletronica cce, Repositorio.UnitOfWork unitOfWork, string wsOracle = "")
        {
            bool sucesso = false;
            string mensagemErro = string.Empty;

            try
            {
                object envioWS = null;

                //Transmitir
                var retornoWS = this.TransmitirEmissor(Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTipoWS.GET, envioWS, $"cte-v4/events?by=externalId&externalId={cce.Codigo}", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPICte);

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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar consulta CCe Nstech: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Emissor NSTech: Ocorreu uma falha ao efetuar a consulta do cte; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        var RetornoConsulta = retorno[0];
                        sucesso = true;

                        if (cce.Status == Dominio.Enumeradores.StatusCCe.Enviado && RetornoConsulta.eventName == "correction_letter")
                        {
                            if (!ProcessarEventoCTe(out mensagemErro, "correction_letter", cce.CTe, ref cce, RetornoConsulta, null, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS, unitOfWork))
                                sucesso = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Exception Sincronizar Documento");
                Servicos.Log.TratarErro(ex);

                throw;
            }

            return sucesso;
        }

        #endregion Métodos Globais

        #region Métodos Privados

        #endregion Métodos Privados
    }
}
