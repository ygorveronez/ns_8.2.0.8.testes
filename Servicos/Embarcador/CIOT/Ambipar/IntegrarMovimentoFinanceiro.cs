using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.CIOT
{
    public partial class Ambipar
    {
        #region Métodos Globais

        public bool IntegrarMovimentoFinanceiro(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valorMovimento)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);

            this.ObterConfiguracaoAmbipar(cargaCIOT.CIOT.ConfiguracaoCIOT);

            mensagemErro = null;
            string jsonRequisicao = "";
            string jsonRetorno = "";
            bool sucesso = false;

            try
            {
                this.ObterToken(out mensagemErro);
                if (string.IsNullOrWhiteSpace(this.token))
                    return false;

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.AcrescimoDesconto.ContratoLancamentoManual contratoLancamentoManual = ObterMovimentoFinanceiro(cargaCIOT.CIOT, justificativa, valorMovimento);

                string url = $"{this.urlWebService}mso-cargo-frete/api/Contrato/IncluirContratoLancamentoManual";
                HttpClient requisicao = CriarRequisicao(url);
                jsonRequisicao = JsonConvert.SerializeObject(contratoLancamentoManual, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (!retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException($"Falha ao lançar movimentos financeiros Ambipar: {retornoRequisicao.StatusCode}");

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.AcrescimoDesconto.RetornoAcrescimosDescontosAmbipar retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.AcrescimoDesconto.RetornoAcrescimosDescontosAmbipar>(jsonRetorno);
                sucesso = true;
                mensagemErro = "Movimentos financeiros CIOT integrado com sucesso";
            }
            catch (ServicoException ex)
            {
                mensagemErro = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                mensagemErro = "Ocorreu uma falha ao realizar a integração CIOT contrato de frete da Ambipar";
            }
            cargaCIOT.CIOT.Mensagem = mensagemErro;
            GravarArquivoIntegracao(cargaCIOT.CIOT, jsonRequisicao, jsonRetorno, "json");
            return sucesso;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.AcrescimoDesconto.ContratoLancamentoManual ObterMovimentoFinanceiro(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Embarcador.Fatura.Justificativa justificativa, decimal valorMovimento)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.TransformerAmbipar transformer = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.TransformerAmbipar(ciot, justificativa);
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.AcrescimoDesconto.ContratoLancamentoManual contratoLancamentoManual = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.AcrescimoDesconto.
            ContratoLancamentoManual()
            {
                ContratoID = transformer.ContratoID(),
                Data = transformer.DataMovimentoFinanceiro(),
                TipoTransacao = transformer.TipoTransacao(),
                Descricao = "",
                Valor = valorMovimento,
                UsuarioLancamento = "",
                DataInclusao = DateTime.Now,
                TipoLancamentoContratoManualID = 1
            };

            return contratoLancamentoManual;
        }

        #endregion
    }
}