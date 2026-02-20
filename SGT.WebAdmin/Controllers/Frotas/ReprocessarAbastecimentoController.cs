using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SGT.WebAdmin.Controllers.Frotas
{
    [CustomAuthorize("Frotas/ReprocessarAbastecimento")]
    public class ReprocessarAbastecimentoController : BaseController
    {
		#region Construtores

		public ReprocessarAbastecimentoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaReprocessarAbastecimento filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.EditableCell editableValorString = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aString, 1000);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Veículo", "Veiculo", 5, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Posto", "Posto", 15, Models.Grid.Align.center, false, false, false, false, true);
                grid.AdicionarCabecalho("Data Abastecimento", "DataAbastecimento", 8, Models.Grid.Align.center, true, false, false, false, true);
                grid.AdicionarCabecalho("Equipamento", "Equipamento", 10, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("KM", "KM", 5, Models.Grid.Align.right, false, false, false, false, true);
                grid.AdicionarCabecalho("Horímetro", "Horimetro", 5, Models.Grid.Align.right, false, false, false, false, true);
                grid.AdicionarCabecalho("Litros", "Litros", 5, Models.Grid.Align.right, false, false, false, false, true);
                grid.AdicionarCabecalho("Tipo Abastecimento", "TipoAbastecimento", 10, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Motivo Inconsistência", "Motivo", 20, Models.Grid.Align.left, false, false, false, false, true);

                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                List<Dominio.Entidades.Abastecimento> listaAbastecimentos = repAbastecimento.ConsultarReprocessarAbastecimento(filtrosPesquisa, "Data", "desc", grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAbastecimento.ContarConsultarReprocessarAbastecimento(filtrosPesquisa));

                var lista = (from p in listaAbastecimentos
                             select new
                             {
                                 p.Codigo,
                                 Veiculo = p.Veiculo != null ? p.Veiculo.Placa : string.Empty,
                                 Posto = p.Posto != null ? p.Posto?.Descricao : string.Empty,
                                 DataAbastecimento = p.Data.HasValue ? p.Data.Value.ToString("dd/MM/yyyy") : "",
                                 Equipamento = p.Equipamento != null ? p.Equipamento.Descricao : string.Empty,
                                 KM = p.Kilometragem.ToString("n0") ?? "",
                                 Horimetro = p.Horimetro.ToString("n2") ?? "",
                                 Litros = p.Litros.ToString("n4") ?? "",
                                 TipoAbastecimento = p.DescricaoTipoAbastecimento ?? "",
                                 Situacao = p.DescricaoSituacao ?? "",
                                 Motivo = p.MotivoInconsistencia ?? "",
                             }).ToList();

                grid.AdicionaRows(lista);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ReprocessarAbastecimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Abastecimento repAbastecimento = new Repositorio.Abastecimento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaReprocessarAbastecimento filtrosPesquisa = ObterFiltrosPesquisa();
                bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");

                List<int> codigosAbastecimentos = new List<int>();

                if (!selecionarTodos)
                {
                    codigosAbastecimentos = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaAbastecimentos"));

                    if (codigosAbastecimentos.Count > 0)
                    {
                        List<Dominio.Entidades.Abastecimento> listaAbastecimentos = repAbastecimento.BuscarPorCodigos(codigosAbastecimentos);

                        if (listaAbastecimentos != null && listaAbastecimentos.Count > 0)
                        {
                            int count = listaAbastecimentos.Count;
                            for (int i = 0; i < count; i++)
                            {
                                Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(listaAbastecimentos[i].Codigo);

                                unitOfWork.Start();

                                abastecimento.Situacao = "A";
                                Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unitOfWork, abastecimento.Veiculo, null, configuracaoTMS);

                                abastecimento.Integrado = false;

                                repAbastecimento.Atualizar(abastecimento);

                                unitOfWork.CommitChanges();

                                unitOfWork.FlushAndClear();
                            }
                        }
                    }
                }
                else
                {
                    List<Dominio.Entidades.Abastecimento> listaAbastecimentos = repAbastecimento.ConsultarReprocessarAbastecimento(filtrosPesquisa, "", "", 0, 0);

                    if (listaAbastecimentos != null && listaAbastecimentos.Count > 0)
                    {
                        int count = listaAbastecimentos.Count;
                        for (int i = 0; i < count; i++)
                        {
                            Dominio.Entidades.Abastecimento abastecimento = repAbastecimento.BuscarPorCodigo(listaAbastecimentos[i].Codigo);

                            unitOfWork.Start();

                            abastecimento.Situacao = "A";
                            Servicos.Embarcador.Abastecimento.Abastecimento.ValidarAbastecimentoInconsistente(ref abastecimento, unitOfWork, abastecimento.Veiculo, null, configuracaoTMS);

                            abastecimento.Integrado = false;

                            repAbastecimento.Atualizar(abastecimento);

                            unitOfWork.CommitChanges();

                            unitOfWork.FlushAndClear();
                        }
                    }
                }
                return new JsonpResult(true, true, "Abastecimentos reprocessados!");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar os abastecimentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaReprocessarAbastecimento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaReprocessarAbastecimento()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                CodigosVeiculos = Request.GetListParam<int>("Veiculo"),
                CodigosEquipamentos = Request.GetListParam<int>("Equipamento"),
                SituacaoAbastecimento = Request.GetStringParam("SituacaoAbastecimento")
            };
        }
        #endregion
    }
}
