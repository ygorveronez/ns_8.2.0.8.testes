using System;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.CIOT.TruckPad;
using Utilidades.Extensions;
using System.Collections.Generic;

namespace Servicos.Embarcador.CIOT.TruckPad
{
    public partial class IntegracaoTruckPad
    {
        #region Métodos Globais

        public bool IntegrarMovimentoFinanceiro(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valorMovimento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(unitOfWork);

            if (justificativa.TipoJustificativa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoJustificativa.Desconto)
            {
                mensagemErro = "A operadora TruckPad não possui integração de Desconto para movimento financeiro.";
                return false;
            }

            if (string.IsNullOrEmpty(cargaCIOT.CIOT.Numero))
            {
                mensagemErro = "Processo Abortado! ID da viagem não definido.";
                return false;
            }

            this._configuracaoIntegracaoTruckPad = this.ObterConfiguracaoTruckPad(cargaCIOT.CIOT.ConfiguracaoCIOT, unitOfWork);

            // Efetua o login na administradora e gera o token
            if (!this.ObterToken(out mensagemErro))
                return false;

            var envioWS = ObterMovimentoFinanceiro(cargaCIOT.CIOT, justificativa, valorMovimento, unitOfWork);

            //Transmite o arquivo
            var retornoWS = this.TransmitirTruckPad(enumTipoWS.PATCH, envioWS, $"ciot/{cargaCIOT.CIOT.Numero}/installment?locale=pt_BR", this.tokenAutenticacao);

            if (retornoWS.erro && string.IsNullOrEmpty(retornoWS.jsonRetorno))
                mensagemErro = string.Concat("Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
            else
                mensagemErro = "Movimento financeiro integrado com sucesso.";

            #region Salvar JSON
            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo
            {
                ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonEnvio, "json", unitOfWork),
                ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(retornoWS.jsonRetorno, "json", unitOfWork),
                Data = DateTime.Now,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento,
                Mensagem = mensagemErro
            };

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
                retMovimentoFinanceiro retorno = null;

                try
                {
                    retorno = retornoWS.jsonRetorno.FromJson<retMovimentoFinanceiro>();
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta JSON da movimentação financeira TruckPad: {ex.ToString()}", "CatchNoAction");
                }

                if (retorno == null)
                {
                    mensagemErro = string.Format("Message: Ocorreu uma falha ao incluir a movimentação financeira no contrato de frete; RetornoWS {0}.", retornoWS.jsonRetorno);
                    return false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(retorno.installments?.FirstOrDefault().id))
                    {
                        mensagemErro = "Movimento financeiro integrado com sucesso.";
                        return true;
                    }
                    else
                    {
                        string mensagemRetorno = "Rejeitado:";
                        mensagemRetorno += " Ocorreu uma falha ao incluir a movimentação financeira no contrato de frete.";

                        mensagemErro = mensagemRetorno;
                        return false;
                    }
                }
            }
        }

        #endregion

        #region Métodos Privados

        private envMovimentoFinanceiro ObterMovimentoFinanceiro(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valorMovimento, Repositorio.UnitOfWork unitOfWork)
        {
            envMovimentoFinanceiro retorno = new envMovimentoFinanceiro();
            retorno.installments = new List<envMovimentoFinanceiroInstallments>();
            envMovimentoFinanceiroInstallments movimentoFinanceiro = new envMovimentoFinanceiroInstallments();

            movimentoFinanceiro.type = "addition";
            movimentoFinanceiro.value_money = Convert.ToInt32(Math.Round(valorMovimento * 100));
            movimentoFinanceiro.effectiveness = "automatic";
            movimentoFinanceiro.status = null;
            movimentoFinanceiro.identification = null;
            movimentoFinanceiro.origin_address = null;
            movimentoFinanceiro.destination_address = null;
            movimentoFinanceiro.external_client_id = $"AC{ciot.Codigo}";
            movimentoFinanceiro.flexible_payment = null;

            retorno.installments.Add(movimentoFinanceiro);

            return retorno;
        }

        #endregion
    }
}

