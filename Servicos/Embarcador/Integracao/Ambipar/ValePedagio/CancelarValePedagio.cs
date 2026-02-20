using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System;

namespace Servicos.Embarcador.Integracao.Ambipar
{
    public partial class ValePedagio
    {
        public void CancelarValePedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);

            _integracaoAmbipar = servicoValePedagio.ObterIntegracaoAmbipar(cargaValePedagio.Carga, tipoServicoMultisoftware);
            if (!ValidarConfiguracaoIntegracao(cargaValePedagio))
                return;

            //Não encontrei método para cancelamento no manual da Ambipar
            cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
            cargaValePedagio.ProblemaIntegracao = "Não possui metodo de cancelamento implementado.";
            repositorioCargaValePedagio.Atualizar(cargaValePedagio); 
            return;

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            string jsonRequisicao = "";
            string jsonRetorno = "";

            cargaValePedagio.DataIntegracao = DateTime.Now;
            cargaValePedagio.NumeroTentativas++;

            try
            {
                string urlConsulta = $"{this.urlWebService}mso-cargo-cadastrounico-transportador/api/xxx&idViagem={cargaValePedagio.IdCompraValePedagio}";
                string jsonRetornoConsulta = "";

                HttpClient requisicaoConsulta = CriarRequisicao(urlConsulta);
                HttpResponseMessage retornoRequisicaoConsulta = requisicaoConsulta.GetAsync(urlConsulta).Result;
                jsonRetornoConsulta = retornoRequisicaoConsulta.Content.ReadAsStringAsync().Result;

                if (!retornoRequisicaoConsulta.IsSuccessStatusCode)
                    throw new ServicoException($"Falha ao conectar no WS Ambipar: {retornoRequisicaoConsulta.StatusCode}");

                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Cancelada;
                cargaValePedagio.ProblemaIntegracao = "Vale Pedágio Cancelado com Sucesso";
            }
            catch (ServicoException excecao)
            {
                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                cargaValePedagio.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                cargaValePedagio.SituacaoValePedagio = SituacaoValePedagio.Comprada;
                cargaValePedagio.ProblemaIntegracao = "Ocorreu uma falha ao realizar o cancelamento do vale pedágio";
            }

            servicoArquivoTransacao.Adicionar(cargaValePedagio, jsonRequisicao, jsonRetorno, "json");

            repositorioCargaValePedagio.Atualizar(cargaValePedagio);
        }
    }
}