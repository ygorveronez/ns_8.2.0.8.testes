using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.Entidades.Embarcador.Frotas;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/RodizioPlacas")]
    public class RodizioPlacasController : BaseController
    {
		#region Construtores

		public RodizioPlacasController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codFilial = Request.GetIntParam("Filial");
                var finalDaPlaca = Request.GetStringParam("FinalDaPlaca");
                int diaDaSemana = Request.GetIntParam("DiaDaSemana");
         
                var repoRodizio = new Repositorio.Embarcador.Frotas.RodizioPlacas(unitOfWork);
                var registros = repoRodizio.Pesquisar(codFilial, finalDaPlaca, diaDaSemana);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoFilial", false);
                grid.AdicionarCabecalho("EnumDia", false);
                grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Dia", "Dia", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Finais de Placas", "Finais", 20, Models.Grid.Align.left, false);

                var rows = registros.Select(x => new
                {
                    Codigo = x.Codigo,
                    CodigoFilial = x.Filial.Codigo,
                    EnumDia = (int)x.DiaSemana,
                    Filial = x.Filial.Descricao,
                    Dia = x.DiaSemana.ObterDescricaoResumida(),
                    Finais = x.ObterFinaisParaGrid()
                }).OrderBy(x => x.Filial)
                  .ThenBy(x => x.EnumDia == 1 ? 8 : x.EnumDia)
                  .ToList();

                grid.setarQuantidadeTotal(registros.Count);
                grid.AdicionaRows(rows);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codRegistro = Request.GetIntParam("Codigo");
                int codFilial = Request.GetIntParam("Filial");
                var finalDaPlaca = Request.GetStringParam("FinalDaPlaca");
                int diaDaSemana = Request.GetIntParam("DiaDaSemana");

                if (diaDaSemana < 1)
                    return new JsonpResult(false, "Informe o Dia da Semana.");

                List<int> listaFinaisPlacas = RodizioPlacas.ObterFinaisDePlacas(finalDaPlaca);
                if (!listaFinaisPlacas.Any())
                    return new JsonpResult(false, "Informe os finais de placa separados por vírgula.");

                var repoRodizio = new Repositorio.Embarcador.Frotas.RodizioPlacas(unitOfWork);
                
                if (codRegistro > 0)
                {
                    var rodizio = repoRodizio.BuscarPorCodigo(codRegistro, true);
                    rodizio.SetarFinaisDePlacas(listaFinaisPlacas);
                    rodizio.DiaSemana = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana)diaDaSemana;

                    repoRodizio.Atualizar(rodizio, Auditado);
                }
                else
                {
                    if(codFilial < 1)
                        return new JsonpResult(false, "Informe a Filial.");

                    var rodizio = new RodizioPlacas();
                    rodizio.SetarFinaisDePlacas(listaFinaisPlacas);
                    rodizio.DiaSemana = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana)diaDaSemana;
                    rodizio.Filial = new Dominio.Entidades.Embarcador.Filiais.Filial { Codigo = codFilial };
                    
                    repoRodizio.Inserir(rodizio, Auditado);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
#if DEBUG
                return new JsonpResult(false, ex.Message);
#else
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
#endif
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Excluir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codRegistro = Request.GetIntParam("Codigo");

                if (codRegistro < 1)
                    return new JsonpResult(false, "Selecione um registro.");

                var repoRodizio = new Repositorio.Embarcador.Frotas.RodizioPlacas(unitOfWork);
                var rodizio = repoRodizio.BuscarPorCodigo(codRegistro, true);

                if(rodizio == null)
                    return new JsonpResult(false, "Registro inexistente.");

                repoRodizio.Deletar(rodizio, Auditado);
               
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
#if DEBUG
                return new JsonpResult(false, ex.Message);
#else
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
#endif
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
