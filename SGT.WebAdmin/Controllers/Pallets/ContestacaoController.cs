using SGT.WebAdmin.Controllers.Anexo;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    public class ContestacaoController : AnexoController<Dominio.Entidades.Embarcador.Pallets.ContestacaoAnexo, Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet>
    {
		#region Construtores

		public ContestacaoController(Conexao conexao) : base(conexao) { }

		#endregion

		public async Task<IActionResult> SolicitarContestacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pallets.DevolucaoPallet repositorioDevolucao = new Repositorio.Embarcador.Pallets.DevolucaoPallet(unitOfWork);
                int codigoDevolucao = Request.GetIntParam("Codigo");
                string motivo = Request.GetStringParam("Motivo");

                if (codigoDevolucao == 0)
                    return new JsonpResult(false,"Registro da devolução não encontrado");

                Dominio.Entidades.Embarcador.Pallets.DevolucaoPallet devolucao = repositorioDevolucao.BuscarPorCodigo(codigoDevolucao);

                devolucao.MotivoContestacao = motivo;

                repositorioDevolucao.Atualizar(devolucao);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);

            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar o(s) arquivo(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}