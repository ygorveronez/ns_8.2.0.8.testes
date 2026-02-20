using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.CIOT
{
    public partial class Ambipar
    {
        #region Métodos Globais

        public bool AutorizarPagamentoCIOT(out string mensagemErro, Dominio.Entidades.Embarcador.Documentos.CIOT ciot, Dominio.Entidades.Usuario usuario, Repositorio.UnitOfWork unitOfWork)
        {
            return AutorizarPagamentoContrato(ciot, out mensagemErro);
        }

        #endregion

        #region Métodos Privados

        private bool AutorizarPagamentoContrato(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, out string mensagem)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);

            this.ObterConfiguracaoAmbipar(ciot.ConfiguracaoCIOT);

            mensagem = null;
            string jsonRequisicao = "";
            string jsonRetorno = "";
            bool sucesso = false;

            try
            {
                this.ObterToken(out mensagem);
                if (string.IsNullOrWhiteSpace(this.token))
                    return false;

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoFrete ContratoFrete = ObterObjEmitirContratoFrete(ciot.CargaCIOT.FirstOrDefault(), null);
                string url = $"{this.urlWebService}mso-cargo-frete/api​/Contrato/AutorizarPagamento";
                HttpClient requisicao = CriarRequisicao(url);
                jsonRequisicao = JsonConvert.SerializeObject(ContratoFrete, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;


                if (!retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException($"Falha ao autorizar pagamento saldo contrato CIOT Ambipar: {retornoRequisicao.StatusCode}");

                sucesso = true;
                mensagem = "Pagamento autorizado com sucesso.";
                ciot.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCIOT.PagamentoAutorizado;
                ciot.Mensagem = mensagem;
                ciot.DataAutorizacaoPagamento = DateTime.Now;
                repCIOT.Atualizar(ciot);
            }
            catch (ServicoException ex)
            {
                mensagem = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                mensagem = "Ocorreu uma falha ao realizar a integração CIOT contrato de frete da Ambipar";
            }

            ciot.Mensagem = mensagem;
            GravarArquivoIntegracao(ciot, jsonRequisicao, jsonRetorno, "json");

            return sucesso;
        }

        #endregion
    }
}