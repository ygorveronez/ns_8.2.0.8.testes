using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/AcompanhamentoFilaCarregamentoReversa")]
    public class AcompanhamentoFilaCarregamentoReversaController : BaseController
    {
		#region Construtores

		public AcompanhamentoFilaCarregamentoReversaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> PesquisaGraficoProximidade()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoFilaCarregamentoReversa filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);

                return new JsonpResult(repositorio.ConsultaGraficoProximidadeAcompanhamentoFilaCarregamentoReversa(filtrosPesquisa));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados do gráfico de proximidade.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaGraficoReversa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoFilaCarregamentoReversa filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);

                return new JsonpResult(repositorio.ConsultaGraficoReversaAcompanhamentoFilaCarregamentoReversa(filtrosPesquisa));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados do gráfico de reversa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoFilaCarregamentoReversa ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoFilaCarregamentoReversa()
            {
                CodigosCentroCarregamento = Request.GetListParam<int>("CentroCarregamento"),
                LojaProximidade = Request.GetNullableBoolParam("Proximidade")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false, adicionarAoAgrupamentoQuandoInvisivel: true);
                grid.AdicionarCabecalho(descricao: "Tração", propriedade: "Tracao", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Reboques", propriedade: "Reboques", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Modelo Veicular", propriedade: "DescricaoModeloVeicularCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Grupo Modelo Veicular", propriedade: "DescricaoGrupoModeloVeicularCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Motorista", propriedade: "NomeMotorista", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Telefone", propriedade: "TelefoneMotorista", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Transportador", propriedade: "NomeTransportador", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Proximidade", propriedade: "LojaProximidadeDescricao", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Entrada na Fila", propriedade: "DataEntradaFilaFormatada", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "CD", propriedade: "DescricaoCentroCarregamento", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Distância", propriedade: "DistanciaCentroCarregamentoFormatada", tamanho: 10, alinhamento: Models.Grid.Align.right, permiteOrdenacao: true);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAcompanhamentoFilaCarregamentoReversa filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaAcompanhamentoReversa(filtrosPesquisa, propriedades);
                IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.AcompanhamentoFilaCarregamentoReversa> listaFilaCarregamento = totalRegistros > 0 ? repositorio.ConsultarAcompanhamentoReversa(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.AcompanhamentoFilaCarregamentoReversa>();

                grid.AdicionaRows(listaFilaCarregamento);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DataEntradaFilaFormatada")
                return "DataEntradaFila";

            if (propriedadeOrdenar == "DistanciaCentroCarregamentoFormatada")
                return "DistanciaCentroCarregamento";

            if (propriedadeOrdenar == "LojaProximidadeDescricao")
                return "LojaProximidade";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
