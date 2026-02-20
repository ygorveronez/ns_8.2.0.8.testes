using System;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.CIOT.TruckPad;
using Utilidades.Extensions;
using Newtonsoft.Json.Linq;

namespace Servicos.Embarcador.CIOT.TruckPad
{
    public partial class IntegracaoTruckPad
    {
        #region Métodos Globais

        public bool EncerrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, out string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork);

            this._configuracaoIntegracaoTruckPad = this.ObterConfiguracaoTruckPad(ciot.ConfiguracaoCIOT, unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);

            if (string.IsNullOrEmpty(cargaCIOT.CIOT.Numero))
            {
                mensagemErro = "Processo Abortado! Nro do CIOT não definido.";
                if (cargaCIOT.CIOT.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Pendencia)
                    return true;
                else
                    return false;
            }

            // Efetua o login na administradora e gera o token
            if (!this.ObterToken(out mensagemErro))
                return false;

            envEncerramento envioWS = this.ObterEncerramento(cargaCIOT, unitOfWork);

            //Transmite o arquivo
            var retornoWS = this.TransmitirTruckPad(enumTipoWS.PATCH, envioWS, $"ciot/default/{cargaCIOT.CIOT.Numero}/close?locale=pt_BR", this.tokenAutenticacao);

            #region Salvar JSON

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

            ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork);
            ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork);
            ciotIntegracaoArquivo.Data = DateTime.Now;
            ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;
            ciotIntegracaoArquivo.Mensagem = "Envio do Encerramento do CIOT.";

            repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);

            cargaCIOT.CIOT.ArquivosTransacao.Add(ciotIntegracaoArquivo);

            repCIOT.Atualizar(cargaCIOT.CIOT);

            #endregion

            if (retornoWS.erro)
            {
                mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                return false;
            }
            else
            {
                retCancel retorno = null;

                try
                {
                    retorno = retornoWS.jsonRetorno.FromJson<retCancel>();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao processar JSON de retorno do webservice TruckPad: {ex.ToString()}", "CatchNoAction");
                }

                if (retorno == null)
                {
                    mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o encerramento do contrato de frete; RetornoWS {0}.", retornoWS.jsonRetorno);
                    return false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(retorno.antt_protocol))
                    {
                        ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.Encerrado;
                        ciot.Mensagem = "Quitação realizada com sucesso.";
                        ciot.DataAutorizacaoPagamento = DateTime.Now;
                        ciot.DataEncerramento = DateTime.Now;

                        repCIOT.Atualizar(ciot);
                        return true;
                    }
                    else
                    {
                        string mensagemRetorno = "Rejeitado:";
                        mensagemRetorno += " Ocorreu uma falha ao efetuar o encerramento do contrato de frete.";

                        mensagemErro = mensagemRetorno;
                        ciot.Mensagem = mensagemRetorno;
                        repCIOT.Atualizar(ciot);
                        return false;
                    }
                }
            }
        }

        #endregion

        #region Métodos Privados

        private envEncerramento ObterEncerramento(Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Repositorio.UnitOfWork unitOfWork)
        {
            envEncerramento retorno = new envEncerramento();
            retorno.delivered_weight = (cargaCIOT.ContratoFrete.ValorLiquidoSemAdiantamentoEImpostos - cargaCIOT.ContratoFrete.ValorTotalDescontoSaldo - cargaCIOT.ContratoFrete.ValorTotalDescontoAdiantamento + cargaCIOT.ContratoFrete.ValorTotalAcrescimoSaldo + cargaCIOT.ContratoFrete.ValorTotalAcrescimoAdiantamento); ;
            return retorno;
        }

        #endregion
    }
}
