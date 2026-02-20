using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize("Pedidos/LoteLiberacaoComercialPedido")]
    public class LoteLiberacaoComercialPedidoController : BaseController
    {
		#region Construtores

		public LoteLiberacaoComercialPedidoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.LoteLiberacaoComercialPedido.FiltroPesquisaLoteLiberacaoComercialPedido filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Operador", "Operador", 20, Models.Grid.Align.left, false);
                if (filtrosPesquisa.Situacao == SituacaoLoteLiberacaoComercialPedido.Todos)
                    grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido repLoteLiberacaoComercialPedido = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido> listaLoteLiberacaoComercialPedido = repLoteLiberacaoComercialPedido.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repLoteLiberacaoComercialPedido.ContarConsulta(filtrosPesquisa));

                grid.AdicionaRows((from obj in listaLoteLiberacaoComercialPedido
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Numero,
                                       Data = obj.Data.ToDateTimeString(),
                                       Operador = obj.Usuario.Nome,
                                       Situacao = obj.Situacao.ObterDescricao()
                                   }).ToList());

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
        public async Task<IActionResult> PesquisaPedidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.LoteLiberacaoComercialPedido.FiltroPesquisaLoteLiberacaoComercialPedidoBloqueado filtrosPesquisa = ObterFiltrosPesquisaPedido();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("DT_RowId", false);
                grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nº Pedido", "Pedido", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Canal de Entrega", "CanalEntrega", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Regiao", "Regiao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Vendedor", "Vendedor", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Gerente", "Gerente", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Supervisor", "Supervisor", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoa", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Categoria", "Categoria", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Produto", "Produto", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação Comercial do Pedido", "SituacaoComercialPedido", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação Estoque do Pedido", "SituacaoEstoquePedido", 10, Models.Grid.Align.center, true);

                Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado repLoteLiberacaoComercialPedidoBloqueados = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado(unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.LoteLiberacaoComercialPedidoBloqueado> listaLoteLiberacaoComercialPedido = null;
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int codigo = Request.GetIntParam("Codigo");

                if (codigo > 0)
                {
                    listaLoteLiberacaoComercialPedido = BuscarPedidosPorCodigo(codigo, parametrosConsulta);
                    grid.setarQuantidadeTotal(listaLoteLiberacaoComercialPedido.Count());
                }
                else
                {
                    int quantidadeRegistros = repLoteLiberacaoComercialPedidoBloqueados.ContarConsultarPedidos(filtrosPesquisa, parametrosConsulta);
                    grid.setarQuantidadeTotal(quantidadeRegistros);

                    if (quantidadeRegistros > 0)
                        listaLoteLiberacaoComercialPedido = repLoteLiberacaoComercialPedidoBloqueados.ConsultarPedidos(filtrosPesquisa, parametrosConsulta);
                }

                grid.AdicionaRows(listaLoteLiberacaoComercialPedido);

                return new JsonpResult(grid);
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, true, ex.Message);
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
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido repLoteLiberacaoComercialPedido = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido(unitOfWork);

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = ObterPedidos(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido loteLiberacaoComercialPedido = new Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido()
                {
                    Data = DateTime.Now,
                    Situacao = SituacaoLoteLiberacaoComercialPedido.EmIntegracao,
                    Numero = repLoteLiberacaoComercialPedido.BuscarProximoNumero(),
                    Usuario = Usuario,
                };

                repLoteLiberacaoComercialPedido.Inserir(loteLiberacaoComercialPedido, Auditado);

                SalvarPedidos(loteLiberacaoComercialPedido, listaPedidos, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(repLoteLiberacaoComercialPedido);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o lote de pedidos bloqueados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }      

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Pedido.LoteLiberacaoComercialPedido.FiltroPesquisaLoteLiberacaoComercialPedido ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.LoteLiberacaoComercialPedido.FiltroPesquisaLoteLiberacaoComercialPedido()
            {
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                Situacao = Request.GetEnumParam<SituacaoLoteLiberacaoComercialPedido>("Situacao"),
                CodigosPedidos = Request.GetListParam<int>("Pedido"),
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.LoteLiberacaoComercialPedido.FiltroPesquisaLoteLiberacaoComercialPedidoBloqueado ObterFiltrosPesquisaPedido()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pedido.LoteLiberacaoComercialPedido.FiltroPesquisaLoteLiberacaoComercialPedidoBloqueado
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                DataInicial = Request.GetDateTimeParam("DataInicialPedido"),
                DataFinal = Request.GetDateTimeParam("DataFinalPedido"),
                CodigosPedidos = Request.GetListParam<int>("Pedido"),
                CodigosDestinatarios = Request.GetListParam<double>("Destinatario"),
                CodigosSituacaoComercialPedido = Request.GetListParam<int>("SituacaoComercialPedido"),
                CodigosVendedores = Request.GetListParam<int>("Vendedor"),
                CodigosGerentes = Request.GetListParam<int>("Gerente"),
                CodigosSupervisores = Request.GetListParam<int>("Supervisor"),
                CodigosCanalEntregas = Request.GetListParam<int>("CanalEntrega"),
                CodigosGrupoPessoas = Request.GetListParam<int>("GrupoPessoas"),
                CodigosCategorias = Request.GetListParam<int>("Categoria"),
                CodigosRegioes = Request.GetListParam<int>("Regiao"),
            };
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ObterPedidos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado repLoteLiberacaoComercialPedidoBloqueados = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

            List<int> codigosPedidos = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaPedidos"));
            bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (codigosPedidos?.Count() == 0 && !selecionarTodos)
                throw new ControllerException("Pedidos selecionados não foram encontrados");

            List<int> listaCodigosPedidos = repLoteLiberacaoComercialPedidoBloqueados.ObterPedidos(ObterFiltrosPesquisaPedido(), selecionarTodos, codigosPedidos);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = repPedido.BuscarPorCodigos(listaCodigosPedidos);

            if (listaPedidos.Count == 0)
                throw new ControllerException("Nenhum pedido selecionada.");

            return listaPedidos;
        }

        private void SalvarPedidos(Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedido loteLiberacaoComercialPedido, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado repLoteLiberacaoComercialPedidoBloqueados = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado(unitOfWork);
            Servicos.Embarcador.Pedido.LoteLiberacaoComercialPedido servicoLoteLiberacaoComercialPedido = new Servicos.Embarcador.Pedido.LoteLiberacaoComercialPedido(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido item in listaPedidos)
            {
                repLoteLiberacaoComercialPedidoBloqueados.Inserir(new Dominio.Entidades.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado()
                {
                    LoteLiberacaoComercialPedido = loteLiberacaoComercialPedido,
                    Pedido = item
                });
            }

            servicoLoteLiberacaoComercialPedido.AdicionarIntegracao(loteLiberacaoComercialPedido);
        }

        private IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.LoteLiberacaoComercialPedidoBloqueado> BuscarPedidosPorCodigo(int codigo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado repLoteLiberacaoComercialPedidoBloqueado = new Repositorio.Embarcador.Pedidos.LoteLiberacaoComercialPedido.LoteLiberacaoComercialPedidoBloqueado(unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.LoteLiberacaoComercialPedidoBloqueado> loteLiberacaoComercialPedido = repLoteLiberacaoComercialPedidoBloqueado.ConsultarPedidosPorCodigo(repLoteLiberacaoComercialPedidoBloqueado.BuscarLoteBloqueadoPedidosPorCodigo(codigo), parametrosConsulta);

                if (loteLiberacaoComercialPedido?.Count() == 0)
                    throw new ControllerException("Lote não encontrado.");

                return loteLiberacaoComercialPedido;
            }
            catch (ControllerException ex)
            {
                throw new ControllerException(ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw new ControllerException("Ocorreu uma falha ao buscar os dados.");
            }
        }
        #endregion
    }
}
