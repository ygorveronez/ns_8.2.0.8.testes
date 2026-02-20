using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170;
using Utilidades.Extensions;

namespace Servicos.Embarcador.Integracao.A52.V170
{
    public partial class IntegracaoA52
    {

        #region Métodos Públicos

        public void CancelarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao)
        {
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao repCargaCancelamentoCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCancelamentoCargaIntegracao(_unitOfWork);

            cargaCancelamentoIntegracao.NumeroTentativas += 1;
            cargaCancelamentoIntegracao.DataIntegracao = DateTime.Now;

            if (!PossuiIntegracaoA52(_configuracaoIntegracao))
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a A52.";
                repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);
                return;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = repCargaIntegracao.BuscarPorCargaETipoIntegracao(cargaCancelamentoIntegracao.CargaCancelamento.Carga.Codigo, cargaCancelamentoIntegracao.TipoIntegracao.Codigo);

            if (cargaIntegracao == null ||
               cargaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Não existe integração realizada com sucesso para a A52 nesta carga.";
                repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);
                return;
            }

            if (cargaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                cargaIntegracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno)
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "A integração de viagem não foi concluída na carga, aguarde e reenvie a integração de cancelamento.";
                repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);
                return;
            }

            if (string.IsNullOrWhiteSpace(cargaIntegracao.Protocolo))
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "O protocolo de integração da viagem não existe, não sendo possível efetuar o cancelamento.";

                repCargaCancelamentoCargaIntegracao.Atualizar(cargaCancelamentoIntegracao);

                return;
            }

            string mensagemErro = string.Empty;

            if (ObterToken(out mensagemErro) &&
                IntegrarCancelamentoViagem(cargaCancelamentoIntegracao, cargaIntegracao.Protocolo, out mensagemErro))
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaCancelamentoIntegracao.ProblemaIntegracao = "Viagem cancelada com sucesso.";
            }
            else
            {
                cargaCancelamentoIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaCancelamentoIntegracao.ProblemaIntegracao = mensagemErro;
            }

            repCargaIntegracao.Atualizar(cargaIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private bool IntegrarCancelamentoViagem(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao, string protocolo, out string mensagemErro)
        {
            bool sucesso = false;
            mensagemErro = string.Empty;

            try
            {
                //Transmite o arquivo
                retornoWebService retornoWS = retornoWS = this.TransmitirRepom(enumTipoWS.PUT, null, $"viagens/cancelar/{protocolo}", this.tokenAutenticacao);

                SalvarArquivosIntegracao(cargaCancelamentoIntegracao, retornoWS.jsonEnvio, retornoWS.jsonRetorno);

                if (retornoWS.erro && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                {
                    retError retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<retError>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta de cancelamento de carga A52: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                        mensagemErro = string.Format("Message: Ocorreu uma falha ao cancelar a viagem; RetornoWS {0}.", retornoWS.jsonRetorno);
                    else
                    {
                        string mensagem = null;

                        int count = 0;
                        foreach (object message in retorno.message)
                        {
                            count++;

                            if (count == 1)
                                mensagem = message.ToString();
                        }

                        if (string.IsNullOrEmpty(mensagem))
                            mensagemErro = "Ocorreu uma falha ao cancelar a viagem.";
                        else
                            mensagemErro = string.Format("Message: Ocorreu uma falha ao cancelar a viagem; RetornoWS {0}.", mensagem);
                    }
                }
                else if (retornoWS.erro)
                    mensagemErro = "Ocorreu uma falha ao cancelar a viagem.";
                else
                    sucesso = true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemErro = "Ocorreu uma falha ao cancelar a viagem.";
            }

            return sucesso;
        }

        private void SalvarArquivosIntegracao(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoCargaIntegracao cargaCancelamentoIntegracao, string jsonRequisicao, string jsonRetorno)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacao(jsonRequisicao, jsonRetorno, cargaCancelamentoIntegracao.ProblemaIntegracao);

            if (arquivoIntegracao == null)
                return;

            if (cargaCancelamentoIntegracao.ArquivosTransacao == null)
                cargaCancelamentoIntegracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

            cargaCancelamentoIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        #endregion Métodos Privados

    }
}
