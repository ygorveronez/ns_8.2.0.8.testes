using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.GestaoEntregas;
using Microsoft.AspNetCore.Mvc;
using RestSharp.Extensions;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Logistica
{
    public class RastreamentoMonitoramentoController : MonitoramentoControllerBase
    {
        #region Construtores
        public RastreamentoMonitoramentoController(Conexao conexao) : base(conexao) { }

        #endregion
        #region Metodos Privados
        private void DefineParametrosView(string token, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente repConfiguracaoAmbiente = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAmbiente(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAmbiente configuracaoAmbiente = repConfiguracaoAmbiente.BuscarPrimeiroRegistro();
            string protocolo = (Request.IsHttps ? "https" : "http");
            if (configuracaoAmbiente?.TipoProtocolo != null && configuracaoAmbiente?.TipoProtocolo.ObterProtocolo() != "")
                protocolo = configuracaoAmbiente?.TipoProtocolo.ObterProtocolo();
            ViewBag.HTTPConnection = protocolo;
            ViewBag.Token = token;
            ViewBag.APIKeyGoogle = configuracaoIntegracao?.APIKeyGoogle ?? "AIzaSyB6e6zUspWGFYrLmABRgI3rsMss_nKW_s4";
            ViewBag.ConfiguracaoTMS = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                Geocodificacao = new
                {
                    GeoServiceGeocoding = configuracaoIntegracao?.GeoServiceGeocoding ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding.Google,
                    NominatimServerUrl = configuracaoIntegracao?.ServidorNominatim ?? "http://20.206.78.118:8080/nominatim/{0}?"
                }
            });
        }


        private async Task<RetornoRastreamentoMonitoramento> BuscarDadosMapa(int codigoCarga, Dominio.Entidades.Embarcador.Logistica.Monitoramento? monitoramento)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);

                JsonpResult dadosMapa = base.DadosMapa(unitOfWork, monitoramento?.Codigo ?? 0, codigoCarga, carga.Veiculo?.Codigo ?? 0);
                return new RetornoRastreamentoMonitoramento { DadosRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(new { dadosMapa.Data, dadosMapa.Success, dadosMapa.Authorized }) };
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return null;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Metodos Publicos 


        [AllowAnonymous]
        public async Task<IActionResult> RastreamentoVisualizacaoMapa(string token)
        {
            string caminhoBaseViews = "~/Views/GestaoEntregas/";

            try
            {
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
                {
                    DefineParametrosView(token, unitOfWork);
                    Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                    int codigoCarga = Convert.ToInt32(Servicos.Criptografia.Descriptografar(token.UrlDecode(), "46ad3d5c9f7f34e502c5c0caa196e91d98b23bba7e6d8a89b2282a92b957d1fd"));

                    Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repositorioMonitoramento.BuscarPorCodigoCarga(codigoCarga);
                    if (monitoramento == null)
                        return View(caminhoBaseViews + "Acompanhamento/Erro.cshtml");

                    return View(caminhoBaseViews + "Acompanhamento/RastreamentoMonitoramento.cshtml", await BuscarDadosMapa(codigoCarga, monitoramento));
                }
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro(e);
                return View(caminhoBaseViews + "Acompanhamento/Erro.cshtml");
            }
        }
        #endregion

    }
}