using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao.A52.V170;
using Utilidades.Extensions;
using static Google.Apis.Requests.BatchRequest;

namespace Servicos.Embarcador.Integracao.A52.V170
{
    public partial class IntegracaoA52
    {

        #region Métodos Públicos

        public void IntegrarSituacaoColaborador(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao situacaoColaborador, Dominio.Entidades.Veiculo veiculo)
        {
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao repColaboradorSituacaoLancamentoIntegracao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao(_unitOfWork);

            situacaoColaborador.NumeroTentativas += 1;
            situacaoColaborador.DataIntegracao = DateTime.Now;

            if (!PossuiIntegracaoA52(_configuracaoIntegracao))
            {
                situacaoColaborador.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                situacaoColaborador.ProblemaIntegracao = "Não existe configuração de integração disponível para a A52.";
                repColaboradorSituacaoLancamentoIntegracao.Atualizar(situacaoColaborador);
                return;
            }

            string mensagemErro = string.Empty;

            if (ObterToken(out mensagemErro) &&
                IntegrarOcorrencia(situacaoColaborador, veiculo, out mensagemErro))
            {
                situacaoColaborador.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                situacaoColaborador.ProblemaIntegracao = "Registro integrado com sucesso.";
            }
            else
            {
                situacaoColaborador.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                situacaoColaborador.ProblemaIntegracao = mensagemErro;
            }

            repColaboradorSituacaoLancamentoIntegracao.Atualizar(situacaoColaborador);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private bool IntegrarOcorrencia(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao situacaoColaborador, Dominio.Entidades.Veiculo veiculo, out string mensagemErro)
        {
            bool sucesso = false;
            mensagemErro = string.Empty;

            try
            {
                if (situacaoColaborador.ColaboradorLancamento == null || situacaoColaborador.ColaboradorLancamento.Colaborador == null || situacaoColaborador.ColaboradorLancamento.ColaboradorSituacao == null || string.IsNullOrWhiteSpace(situacaoColaborador.ColaboradorLancamento.ColaboradorSituacao.CodigoIntegracao))
                {
                    mensagemErro = "Não foi informado a situação para a geração da ocorrência.";
                    return false;
                }

                object envioWS = ObterOcorrencia(situacaoColaborador, veiculo);

                //Transmite o arquivo
                retornoWebService retornoWS = retornoWS = this.TransmitirRepom(enumTipoWS.POST, envioWS, "v1/ocorrencias", this.tokenAutenticacao);

                SalvarArquivosIntegracao(situacaoColaborador, retornoWS.jsonEnvio, retornoWS.jsonRetorno);

                if (retornoWS.erro && !string.IsNullOrEmpty(retornoWS.jsonRetorno))
                    mensagemErro = string.Format("Message: Ocorreu uma falha ao incluir a ocorrência; RetornoWS {0}.", retornoWS.jsonRetorno);
                else if (retornoWS.erro)
                    mensagemErro = "Ocorreu uma falha ao incluir a ocorrência.";
                else
                    sucesso = true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                mensagemErro = "Ocorreu uma falha ao incluir a ocorrência.";
            }

            return sucesso;
        }

        private envOcorrencia ObterOcorrencia(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao situacaoColaborador, Dominio.Entidades.Veiculo veiculo)
        {
            envOcorrencia retorno = new envOcorrencia();
            retorno.cpf = situacaoColaborador.ColaboradorLancamento.Colaborador.CPF;
            retorno.placa = veiculo?.Placa ?? "";
            retorno.status = situacaoColaborador.ColaboradorLancamento.SituacaoLancamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoColaborador.Cancelado ? "239" : situacaoColaborador.ColaboradorLancamento.ColaboradorSituacao.CodigoIntegracao;
            retorno.data_inicial = situacaoColaborador.ColaboradorLancamento.DataInicial.ToString("yyyy-MM-dd HH:mm:ss");
            retorno.data_final = situacaoColaborador.ColaboradorLancamento.DataFinal.ToString("yyyy-MM-dd HH:mm:ss");
            return retorno;
        }

        private void SalvarArquivosIntegracao(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao situacaoColaborador, string jsonRequisicao, string jsonRetorno)
        {
            Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo arquivoIntegracao = AdicionarArquivoTransacaoSituacao(situacaoColaborador, jsonRequisicao, jsonRetorno, situacaoColaborador.ProblemaIntegracao);

            if (arquivoIntegracao == null)
                return;

            //if (situacaoColaborador.ArquivosTransacao == null)
            //    situacaoColaborador.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo>();

            //situacaoColaborador.ArquivosTransacao.Add(arquivoIntegracao);
        }

        private Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo AdicionarArquivoTransacaoSituacao(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao colaboradorSituacaoLancamentoIntegracao, string jsonRequisicao, string jsonRetorno, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonRetorno))
                return null;

            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo()
            {
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", _unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRetorno, "json", _unitOfWork),
                Data = DateTime.Now,
                Mensagem = mensagem,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                ColaboradorSituacaoLancamentoIntegracao = colaboradorSituacaoLancamentoIntegracao
            };

            repositorioCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            return arquivoIntegracao;
        }

        #endregion Métodos Privados

    }
}