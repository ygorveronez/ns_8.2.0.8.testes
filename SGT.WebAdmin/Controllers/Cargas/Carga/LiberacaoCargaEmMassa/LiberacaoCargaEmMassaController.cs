using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.LiberacaoCargaEmMassa
{
    public class LiberacaoCargaEmMassaController : BaseController
    {
		#region Construtores

		public LiberacaoCargaEmMassaController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> LiberarCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                dynamic codigosCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CodigosCargas"));

                foreach (dynamic codigoCarga in codigosCarga)
                {
                    string mesagem = string.Empty;
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo((int)codigoCarga);

                    if (carga == null)
                        throw new ControllerException($"Carga Com o codigo {codigoCarga} não encontrada ");

                    if (!servicoCarga.LiberarEtapaEmisao(carga, unitOfWork, Auditado, TipoServicoMultisoftware, WebServiceConsultaCTe, out mesagem))
                        throw new ControllerException(mesagem);
                }
                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Metodos Privados
   

        #endregion

    }
}
