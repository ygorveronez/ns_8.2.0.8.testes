using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/MapRequestApi")]
    public class MapRequestApiController : BaseController
    {
		#region Construtores

		public MapRequestApiController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                string origem = Request.Params("origem");
                string destino = Request.Params("destino");

                Servicos.Embarcador.Logistica.MapRequestApi serMapRequestAPI = new Servicos.Embarcador.Logistica.MapRequestApi();
                Dominio.ObjetosDeValor.Embarcador.Logistica.RouteMapRequestAPI route = serMapRequestAPI.BuscarRotaMapRequestApi(origem, destino, true);
                if (route.valido)
                {
                    return new JsonpResult(route);
                }
                else
                {
                    return new JsonpResult(false, true, route.mensagem);
                }
                
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPassagemEntreEstados()
        {
            try
            {
                dynamic localidadesConsulta = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("localidades"));
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> Passagens = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem>();
                bool valido = true;
                string mensagem = "";
                string ufSiglaOrigemAnterior = "";
                for (int i = 0; i < localidadesConsulta.Count - 1; i++)
                {
                    Servicos.Embarcador.Logistica.MapRequestApi serMapRequestAPI = new Servicos.Embarcador.Logistica.MapRequestApi();
                    Dominio.ObjetosDeValor.Embarcador.Logistica.RouteMapRequestAPI route = serMapRequestAPI.BuscarRotaMapRequestApi((string)localidadesConsulta[i].Localidade, (string)localidadesConsulta[i + 1].Localidade, true);
                    valido = route.valido;
                    if (valido)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem ultimaPassagem = null;
                        if (Passagens.Count > 0)
                            ultimaPassagem = Passagens.Last();

                        if (i > 0 && route.UFOrigem != ufSiglaOrigemAnterior)
                        {
                            if (ultimaPassagem == null || ultimaPassagem.Sigla != route.UFOrigem && route.UFDestino != route.UFOrigem) // valida se o estado a ser incluido já não é o ultimo estado da passagem;
                                Passagens.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Sigla = route.UFOrigem, Posicao = Passagens.Count() + 1 });
                        }
                        ufSiglaOrigemAnterior = route.UFOrigem;

                        foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem passageRota in route.UFPassagens)
                        {
                            Passagens.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Sigla = passageRota.Sigla, Posicao = Passagens.Count() + 1 });
                        }
                    }
                    else
                    {
                        mensagem = route.mensagem;
                        break;
                    }
                }
                if (valido)
                {
                    return new JsonpResult(Passagens);
                }
                else
                {
                    return new JsonpResult(false, true, mensagem);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar a passagem entre os Estados.");
            }
        }
    }
}
