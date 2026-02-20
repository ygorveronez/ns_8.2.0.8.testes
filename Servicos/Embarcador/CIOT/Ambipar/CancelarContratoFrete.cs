using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Servicos.Embarcador.CIOT
{
    public partial class Ambipar
    {
        #region Métodos Globais

        public bool CancelarCIOT(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, out string mensagemErro)
        {
            return CancelarOperacaoTransporte(out mensagemErro, ciot);
        }

        #endregion

        #region Métodos Privados

        private bool CancelarOperacaoTransporte(out string mensagemErro, Dominio.Entidades.Embarcador.Documentos.CIOT ciot)
        {
            mensagemErro = null;
            string jsonRequisicao = "";
            string jsonRetorno = "";
            bool sucesso = false;

            Repositorio.Embarcador.CIOT.CIOTAmbipar repConfig = new Repositorio.Embarcador.CIOT.CIOTAmbipar(_unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCIOT repCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(_unitOfWork);

            this.ObterConfiguracaoAmbipar(ciot.ConfiguracaoCIOT);

            Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = repCargaCIOT.BuscarPrimeiroPorCIOT(ciot.Codigo);
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.Cancelamento ContratoFrete = ObterObjCancelarContrato(ciot); // pendente angelo

            this.ObterToken(out mensagemErro);
            if (string.IsNullOrWhiteSpace(this.token))
                return false;

            string url = $"{this.urlWebService}mso-cargo-frete/api/Contrato/CancelarContrato";
            HttpClient requisicao = CriarRequisicao(url);
            jsonRequisicao = JsonConvert.SerializeObject(ContratoFrete, Formatting.Indented);
            StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
            jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

            if (retornoRequisicao.StatusCode != HttpStatusCode.Created)// revisar pois não tem documentação sobre retorno 
                throw new ServicoException($"Falha ao cancelar contrato frete Ambipar: {retornoRequisicao.StatusCode}");
            else
                sucesso = true;

            // passou aqui deu sucesso  eles não possui status de retorno no objeto de retorno 
            if (sucesso)
            {
                mensagemErro = "Cancelamento realizado com sucesso.";
                //ciot.ProtocoloCancelamento = retorno.ProtocoloCancelamento;
                //ciot.DataCancelamento = retorno.DataCancelamento ?? DateTime.Now;
                ciot.Situacao = SituacaoCIOT.Cancelado;
            }
            else
                mensagemErro = "Ocorreu uma falha ao tentatar cancelamento do CIOT Ambipar.";

            ciot.Mensagem = mensagemErro;
            repCIOT.Atualizar(ciot);

            GravarArquivoIntegracao(ciot, jsonRequisicao, jsonRetorno, "json");

            return sucesso;
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.Cancelamento ObterObjCancelarContrato(Dominio.Entidades.Embarcador.Documentos.CIOT ciot)
        {
            Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.TransformerAmbipar transformer = new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.TransformerAmbipar(ciot, null);
            return new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.Cancelamento()
            {
                contratoID = transformer.ContratoID(),
                motivoCancelamento = transformer.motivoCancelamento(),
            };
        }

        #endregion
    }
}