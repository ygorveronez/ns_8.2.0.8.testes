using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize("Chamados/ControleChamadoTMS")]
    public class ControleChamadoTMSController : BaseController
    {
		#region Construtores

		public ControleChamadoTMSController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 3, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Carga", "Carga", 4, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motivo Chamado", "MotivoChamado", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Criação", "DataCriacao", 6, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaControleChamadoTMS filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Chamados.ChamadoTMS repChamadoTMS = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                int totalRegistros = repChamadoTMS.ContarConsultaChamadosParaControle(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMS> listaChamado = (totalRegistros > 0) ? repChamadoTMS.ConsultarChamadosParaControle(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMS>();

                var listaChamadoRetornar = (
                    from obj in listaChamado
                    select new
                    {
                        obj.Codigo,
                        obj.Numero,
                        Carga = obj.Carga.CodigoCargaEmbarcador,
                        Motorista = obj.Motorista.Nome,
                        MotivoChamado = obj.MotivoChamado.Descricao,
                        DataCriacao = obj.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                        Situacao = obj.Situacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaChamadoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaControleChamadoTMS ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaControleChamadoTMS()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("Carga"),
                CodigoMotivoChamado = Request.GetIntParam("MotivoChamado"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroCTe = Request.GetIntParam("NumeroCTe")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Carga")
                return "Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "Motorista")
                return "Motorista.Nome";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
