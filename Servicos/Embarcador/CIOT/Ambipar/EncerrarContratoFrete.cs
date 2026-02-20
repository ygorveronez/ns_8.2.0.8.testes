using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.CIOT
{
    public partial class Ambipar
    {
        #region Métodos Globais

        public bool EncerrarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, out string mensagemErro)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);
            this.ObterConfiguracaoAmbipar(ciot.ConfiguracaoCIOT);

            bool sucesso = false;
            mensagemErro = string.Empty;

            sucesso = EncerrarOperacaoTransporte(out mensagemErro, cargaCIOT);

            return sucesso;
        }

        #endregion

        #region Métodos Privados

        private bool EncerrarOperacaoTransporte(out string mensagemErro, Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT)
        {
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);

            mensagemErro = null;
            string jsonRequisicao = "";
            string jsonRetorno = "";
            bool sucesso = false;

            try
            {
                this.ObterToken(out mensagemErro);
                if (string.IsNullOrWhiteSpace(this.token))
                    return false;

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoFrete ContratoFrete = ObterObjEmitirContratoFrete(cargaCIOT, null);

                string url = $"{this.urlWebService}mso-cargo-frete/api/Contrato/QuitarContrato";

                HttpClient requisicao = CriarRequisicao(url);
                jsonRequisicao = JsonConvert.SerializeObject(ContratoFrete, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (!retornoRequisicao.IsSuccessStatusCode)
                    throw new ServicoException($"Falha no envio da quitação de contrato frete Ambipar: {retornoRequisicao.StatusCode}");

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoFreteAmbipar retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.ContratoFreteAmbipar>(jsonRetorno);
                cargaCIOT.CIOT.Situacao = SituacaoCIOT.Encerrado;
                sucesso = true;
                mensagemErro = "Encerramento realizado com sucesso.";
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
            repCIOT.Atualizar(cargaCIOT.CIOT);
            GravarArquivoIntegracao(cargaCIOT.CIOT, jsonRequisicao, jsonRetorno, "json");

            return sucesso;
        }

        #endregion
    }
}