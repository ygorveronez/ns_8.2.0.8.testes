using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/FilaCarregamentoReversa")]
    public class FilaCarregamentoReversaController : BaseController
    {
		#region Construtores

		public FilaCarregamentoReversaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AdicionarFila()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
            int codigoVeiculo = Request.GetIntParam("Veiculo");
            int codigoTipoRetornoCarga = Request.GetIntParam("Tipo");

            try
            {
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculoReversa servicoFilaCarregamentoVeiculoReversa = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculoReversa(unitOfWork, Usuario);

                servicoFilaCarregamentoVeiculoReversa.Adicionar(codigoVeiculo, codigoCentroCarregamento, codigoTipoRetornoCarga);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar registro na fila de carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarDescarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            int codigoFilaCarregamentoReversa = Request.GetIntParam("Codigo");

            try
            {
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculoReversa servicoFilaCarregamentoVeiculoReversa = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculoReversa(unitOfWork, Usuario);

                servicoFilaCarregamentoVeiculoReversa.CancelarDescarregamento(codigoFilaCarregamentoReversa);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao finalizar o descarregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarDescarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            int codigoFilaCarregamentoReversa = Request.GetIntParam("Codigo");

            try
            {
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculoReversa servicoFilaCarregamentoVeiculoReversa = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculoReversa(unitOfWork, Usuario);

                servicoFilaCarregamentoVeiculoReversa.FinalizarDescarregamento(codigoFilaCarregamentoReversa);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao finalizar o descarregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> IniciarDescarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            int codigoFilaCarregamentoReversa = Request.GetIntParam("Codigo");
            int codigoLocal = Request.GetIntParam("Local");

            try
            {
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculoReversa servicoFilaCarregamentoVeiculoReversa = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculoReversa(unitOfWork, Usuario);

                servicoFilaCarregamentoVeiculoReversa.IniciarDescarregamento(codigoFilaCarregamentoReversa, codigoLocal);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao iniciar o descarregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculoReversa = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversaHistorico repositorioHistorico = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversaHistorico(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversaHistorico> listaHistorico = repositorioHistorico.BuscarPorFilaCarregamentoVeiculoReversa(codigoFilaCarregamentoVeiculoReversa);

                var listaHistoricoRetornar = (
                    from historico in listaHistorico
                    select new
                    {
                        DataOrdenar = historico.Data.ToString("yyyyMMddHHmm"),
                        Data = historico.Data.ToString("dd/MM/yyyy HH:mm"),
                        historico.Descricao,
                        Usuario = historico.Usuario?.Descricao
                    }
                ).ToList();

                return new JsonpResult(listaHistoricoRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o histórico da reversa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

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

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoReversa ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoReversa()
            {
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                CodigoGrupoModeloVeicularCarga = Request.GetIntParam("GrupoModeloVeicular"),
                CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicular"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                Situacoes = Request.GetListEnumParam<SituacaoFilaCarregamentoVeiculoReversa>("Situacao")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoReversa filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoTipoRetornoCarga", visivel: false);
                grid.AdicionarCabecalho(propriedade: "DataCriacao", visivel: false);
                grid.AdicionarCabecalho(propriedade: "Situacao", visivel: false);
                grid.AdicionarCabecalho(propriedade: "PermitirRemocao", visivel: false);

                if (filtrosPesquisa.CodigoModeloVeicularCarga == 0)
                    grid.AdicionarCabecalho(descricao: "Modelo Veicular", propriedade: "ModeloVeicularCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);

                grid.AdicionarCabecalho(descricao: "Tração", propriedade: "Tracao", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Reboques", propriedade: "Reboques", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Tipo de Retorno", propriedade: "TipoRetornoCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Local Atual", propriedade: "LocalAtual", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Local Descarregamento", propriedade: "LocalDescarregamento", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Tempo na Fila", propriedade: "Tempo", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "asc",
                    InicioRegistros = grid.inicio,
                    LimiteRegistros = grid.limite,
                    PropriedadeOrdenar = "DataCriacao"
                };
                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversa repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoReversa(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa> listaFilaCarregamentoVeiculoReversa = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoReversa>();

                var listaFilaCarregamentoRetornar = (
                    from fila in listaFilaCarregamentoVeiculoReversa
                    select new
                    {
                        fila.Codigo,
                        fila.DataCriacao,
                        fila.Situacao,
                        CodigoTipoRetornoCarga = fila.TipoRetornoCarga?.Codigo ?? 0,
                        PermitirRemocao = fila.IsPermiteRemocao(),
                        ModeloVeicularCarga = fila.ConjuntoVeiculo.ModeloVeicularCarga?.Descricao,
                        Tracao = fila.ConjuntoVeiculo.Tracao?.Placa_Formatada,
                        Reboques = string.Join(", ", (from reboque in fila.ConjuntoVeiculo.Reboques select reboque.Placa_Formatada).ToList()),
                        TipoRetornoCarga = fila.TipoRetornoCarga?.Descricao,
                        LocalAtual = fila.ConjuntoVeiculo.ObterLocalAtual()?.DescricaoAcao ?? "",
                        LocalDescarregamento = fila.LocalDescarregamento?.DescricaoAcao,
                        Tempo = fila.ObterTempo(),
                        DT_RowColor = fila.Situacao.ObterCorLinha()
                    }
                ).ToList();

                grid.AdicionaRows(listaFilaCarregamentoRetornar);
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

        #endregion
    }
}
