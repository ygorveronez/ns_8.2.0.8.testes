using System;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete;
using Utilidades.Extensions;

namespace Servicos.Embarcador.CIOT.RepomFrete
{
    public partial class IntegracaoRepomFrete
    {
        #region Métodos Globais

        public bool InterromperCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            mensagemErro = null;

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            this.configuracaoIntegracaoRepomFrete = this.ObterConfiguracaoRepomFrete(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            if (string.IsNullOrEmpty(cargaCIOT.CIOT.ProtocoloAutorizacao))
            {
                mensagemErro = "Processo Abortado! ID da viagem não definido.";
                return false;
            }

            // Efetua o login na administradora e gera o token
            if (!this.ObterToken(out mensagemErro))
                return false;

            //Shipping ID
            string cEnvio = cargaCIOT.CIOT.ProtocoloAutorizacao;

            //Transmite o arquivo
            var retornoWS = this.TransmitirRepom(enumTipoWS.PATCH, cEnvio, "Shipping/Interruption", this.tokenAutenticacao);

            #region Salvar JSON
            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

            ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork);
            ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork);
            ciotIntegracaoArquivo.Data = DateTime.Now;
            ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
            ciotIntegracaoArquivo.Mensagem = "Envio da interrupção do CIOT.";

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);
            #endregion

            if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
            {
                mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                return false;
            }
            else
            {
                retPadrao retorno = null;

                try
                {
                    retorno = retornoWS.jsonRetorno.FromJson<retPadrao>();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta da interrupção de CIOT RepomFrete: {ex.ToString()}", "CatchNoAction");
                }

                if (retorno == null)
                {
                    mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar a interrupção do contrato de frete; RetornoWS {0}.", retornoWS.jsonRetorno);
                    return false;
                }
                else
                {
                    if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                    {
                        cargaCIOT.CIOT.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Cancelado;
                        cargaCIOT.CIOT.Mensagem = "Interrupção realizada com sucesso.";
                        cargaCIOT.CIOT.DataCancelamento = DateTime.Now;

                        repCIOT.Atualizar(cargaCIOT.CIOT);

                        return true;
                    }
                    else
                    {
                        string mensagemRetorno = "Rejeitado:";

                        if (retorno.Errors != null && retorno.Errors.Count() > 0)
                            retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                        else
                            mensagemRetorno += " Ocorreu uma falha ao efetuar a interrupção do contrato de frete.";

                        mensagemErro = mensagemRetorno;
                        return false;
                    }
                }
            }
        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}
