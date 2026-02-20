using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Impressao
{
    [CustomAuthorize("Cargas/Carga", "Cargas/CargaAnexo")]
    public class CargaAnexoController : Anexo.AnexoController<Dominio.Entidades.Embarcador.Cargas.CargaAnexo, Dominio.Entidades.Embarcador.Cargas.Carga>
    {
		#region Construtores

		public CargaAnexoController(Conexao conexao) : base(conexao) { }

		#endregion

		[AllowAuthenticate]
        public async Task<IActionResult> ContarAnexosCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int qtde = 0;

                if (codigo > 0)
                {
                    Repositorio.Embarcador.Cargas.CargaAnexo repositorioAnexo = new Repositorio.Embarcador.Cargas.CargaAnexo(unitOfWork);
                    qtde = repositorioAnexo.ContarPorCarga(codigo);
                }

                return new JsonpResult(qtde);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao verificar anexos da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        protected override bool IsPermitirAdicionarAnexo(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return carga.SituacaoCarga.IsSituacaoCargaNaoEmitida();
        }

        protected override bool IsPermitirExcluirAnexo(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            return carga.SituacaoCarga.IsSituacaoCargaNaoEmitida();
        }

    }
}