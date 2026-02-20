using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.BusinessIntelligence.Controllers.Chamados
{
	[Area("BusinessIntelligence")]
	[CustomAuthorize("BusinessIntelligence/Chamados/Chamado")]
    public class ChamadoController : BaseController
    {
		#region Construtores

		public ChamadoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterDadosChamado()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unidadeTrabalho);

                int.TryParse(Request.Params("Responsavel"), out int responsavel);
                int.TryParse(Request.Params("Motivo"), out int motivo);
                int.TryParse(Request.Params("Transportador"), out int transportador);

                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicio);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFim);


                IList<Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.GraficoChamado> grafico = repChamado.GraficoChamados(dataInicio, dataFim, responsavel, motivo, transportador);

                int totalChamados = (from o in grafico select o.Quantidade).Sum();
                grafico = (from o in grafico
                           select new Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia.GraficoChamado
                           {
                               Quantidade = o.Quantidade,
                               Situacao = o.Situacao,
                               Descricao = o.Quantidade + " de " + totalChamados
                           }).ToList();

                return new JsonpResult(grafico);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, true, "Ocorreu uma falha ao obter os valores.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }
    }
}
