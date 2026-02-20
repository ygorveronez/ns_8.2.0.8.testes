using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.CTe
{
    [CustomAuthorize("CTe/Aliquota")]
    public class AliquotaController : BaseController
    {
		#region Construtores

		public AliquotaController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTodos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.AliquotaDeICMS repAliquota = new Repositorio.AliquotaDeICMS(unidadeDeTrabalho);

                List<Dominio.Entidades.AliquotaDeICMS> aliquotas = repAliquota.BuscarPorEmpresa(this.Empresa.Codigo, "A");

                var retorno = (from obj in aliquotas select new { value = obj.Aliquota.ToString(), text = obj.Aliquota.ToString("n2") + "%" }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter as alíquotas.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
