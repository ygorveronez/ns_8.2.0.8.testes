using Newtonsoft.Json.Linq;
using Servicos.Extensions;
using System;
using System.Linq;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool ConsultarCte(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            bool sucesso = false;
            string mensagemErro = string.Empty;

            try
            {
                object envioWS = null;

                //Transmitir
                var retornoWS = this.TransmitirEmissor(Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTipoWS.GET, envioWS, $"cte-v4?by=externalId&externalId={cte.Codigo}", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPICte);

                if (retornoWS.erro)
                {
                    mensagemErro = string.Concat("Emissor NSTech: Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    sucesso = false;

                    try
                    {
                        if (retornoWS.StatusCode == 404 && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                        {
                            dynamic objetoRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(retornoWS.jsonRetorno);
                            if (objetoRetorno?.type == "cte_not_found")
                            {
                                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                                cte.Status = "R";
                                cte.MensagemRetornoSefaz = "CT-e não encontrado no emissor NSTech.";
                                repCte.Atualizar(cte);
                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Emissor NSTech: Ocorreu uma falha ao processar o retorno da consulta do cte");
                        Servicos.Log.TratarErro(ex);
                    }
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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao deserializar retorno JSON CTe consulta Nstech - primeiro catch: {ex.ToString()}", "CatchNoAction");
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

                        if (cte.Status == "E")
                        {
                            if (!ProcessarCTe(out mensagemErro, "com.nstech.issuance-engine.cte", cte, RetornoConsulta, null, Auditado, tipoServicoMultisoftware, unitOfWork))
                                sucesso = false;
                        }
                        else if (cte.Status == "K")
                        {
                            if (RetornoConsulta?.events != null)
                            {
                                // Filtrando eventos com eventName "cancel"
                                var eventoCancelado = ((JArray)RetornoConsulta["events"])
                                    .FirstOrDefault(e => (string)e["eventName"] == "cancel");

                                if (eventoCancelado != null)
                                {
                                    Dominio.Entidades.CartaDeCorrecaoEletronica cce = null;
                                    if (!ProcessarEventoCTe(out mensagemErro, "cancel", cte, ref cce, eventoCancelado, null, Auditado, tipoServicoMultisoftware, unitOfWork))
                                        sucesso = false;
                                }
                            }
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

        public byte[] ObterXMLCte(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool retornarXMLCancelamento = false)
        {
            if (!retornarXMLCancelamento && !string.IsNullOrWhiteSpace(cte.UrlDownloadXml))
            {
                //TODO: Precisa mudar essa busca pra guardar em Cache a configuração
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoSGT = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoSGT = repConfiguracaoSGT.BuscarConfiguracaoPadrao();
                bool armazenarEmArquivo = configuracaoSGT?.ArmazenarXMLCTeEmArquivo ?? false;

                string retornoXml = ObterESalvarXmlUrlEmissor(cte, null, armazenarEmArquivo, unitOfWork);

                if (!string.IsNullOrWhiteSpace(retornoXml))
                {
                    byte[] data = System.Text.Encoding.Default.GetBytes(retornoXml);
                    return data;
                }
            }

            string mensagemErro = string.Empty;

            try
            {
                object envioWS = null;

                //Transmitir
                var retornoWS = this.TransmitirEmissor(Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTipoWS.GET, envioWS, $"cte-v4?by=externalId&externalId={cte.Codigo}", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPICte);

                if (retornoWS.erro)
                {
                    mensagemErro = string.Concat("Emissor NSTech: Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    throw new Exception(mensagemErro);

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
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao deserializar retorno JSON CTe consulta Nstech - segundo catch: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Emissor NSTech: Ocorreu uma falha ao efetuar a consulta do cte; RetornoWS {0}.", retornoWS.jsonRetorno);
                        throw new Exception(mensagemErro);
                    }
                    else
                    {
                        var RetornoConsulta = retorno[0];


                        if (cte.Status == "A" || cte.Status == "C")
                        {
                            string retornoXml = ProcessarXMLCTe(out mensagemErro, "com.nstech.issuance-engine.cte", cte, RetornoConsulta, null, Auditado, tipoServicoMultisoftware, unitOfWork);

                            if (!string.IsNullOrWhiteSpace(retornoXml))
                            {
                                byte[] data = System.Text.Encoding.Default.GetBytes(retornoXml);
                                return data;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Exception Baixar XML");
                Servicos.Log.TratarErro(ex);

                throw;
            }

            return null;
        }

        #endregion Métodos Globais

        #region Métodos Privados

        #endregion Métodos Privados
    }
}
