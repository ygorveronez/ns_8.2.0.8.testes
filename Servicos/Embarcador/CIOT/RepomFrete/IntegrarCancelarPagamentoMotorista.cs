using System;
using System.Linq;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.CIOT.RepomFrete;
using Utilidades.Extensions;

namespace Servicos.Embarcador.CIOT.RepomFrete
{
    public partial class IntegracaoRepomFrete
    {
        #region Métodos Globais

        public bool EstornarPagamentoMotorista(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, out string mensagemErro, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno repPagamentoMotoristaIntegracaoRetorno = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoExtratta repIntegracaoExtratta = new Repositorio.Embarcador.Configuracoes.IntegracaoExtratta(unitOfWork);

            if (pagamentoMotorista.CodigoViagem == 0)
                throw new ServicoException("Processo Abortado! ID da viagem não definido.");

            bool sucesso = false;
            string codigoRetorno = string.Empty;
            string jsonPost = string.Empty;
            string jsonResult = string.Empty;

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoRetorno = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno();

            this.configuracaoIntegracaoRepomFrete = this.ObterConfiguracaoRepomFrete(null, unitOfWork);

            // Efetua o login na administradora e gera o token
            if (!this.ObterToken(out mensagemErro))
                throw new ServicoException(mensagemErro);

            //Shipping ID
            string cEnvio = pagamentoMotorista.CodigoViagem.ToString();

            //Transmite o arquivo
            var retornoWS = this.TransmitirRepom(enumTipoWS.PATCH, cEnvio, "Shipping/Cancel", this.tokenAutenticacao);

            if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
            {
                mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                sucesso = false;
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
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta do cancelamento de pagamento motorista RepomFrete: {ex.ToString()}", "CatchNoAction");
                }

                if (retorno == null)
                {
                    mensagemErro = string.Format("Message: Ocorreu uma falha ao efetuar o cancelamento do contrato de frete; RetornoWS {0}.", retornoWS.jsonRetorno);
                    sucesso = false;
                }
                else
                {
                    if (retorno?.Response?.StatusCode == 200 || retorno?.Response?.StatusCode == 201)
                    {
                        mensagemErro = "Cancelamento realizado com sucesso.";
                        sucesso = true;
                    }
                    else
                    {
                        string mensagemRetorno = "Rejeitado:";

                        if (retorno.Errors != null && retorno.Errors.Count() > 0)
                            retorno.Errors.ForEach(x => mensagemRetorno += System.Environment.NewLine + x.Message);
                        else
                            mensagemRetorno += " Ocorreu uma falha ao efetuar o cancelamento do contrato de frete.";

                        mensagemErro = mensagemRetorno;
                        sucesso = false;
                    }
                }
            }

            pagamentoRetorno.CodigoRetorno = codigoRetorno;
            pagamentoRetorno.DescricaoRetorno = mensagemErro;
            pagamentoRetorno.Data = DateTime.Now;
            pagamentoRetorno.PagamentoMotoristaTMS = pagamentoMotorista;
            pagamentoRetorno.PagamentoMotoristaIntegracaoEnvio = pagamentoEnvio;
            pagamentoRetorno.ArquivoRetorno = jsonResult;

            pagamentoRetorno.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork);
            pagamentoRetorno.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork);

            repPagamentoMotoristaIntegracaoRetorno.Inserir(pagamentoRetorno);

            return sucesso;
        }

        #endregion
    }
}

