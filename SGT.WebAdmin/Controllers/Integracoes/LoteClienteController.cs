using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/LoteCliente")]
    public class LoteClienteController : BaseController
    {
		#region Construtores

		public LoteClienteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int numero = Request.GetIntParam("Numero");
                DateTime dataInicial = Request.GetDateTimeParam("DataInicio");
                DateTime dataFinal = Request.GetDateTimeParam("DataFim");

                SituacaoLoteCliente? situacao = Request.GetNullableEnumParam<SituacaoLoteCliente>("Situacao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Inicial", "DataInicial", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Final", "DataFinal", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Integracao.LoteCliente repLoteCliente = new Repositorio.Embarcador.Integracao.LoteCliente(unitOfWork);

                List<Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente> lotes = repLoteCliente.Consultar(numero, dataInicial, dataFinal, situacao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);

                int totalRegistros = repLoteCliente.ContarConsulta(numero, dataInicial, dataFinal, situacao);

                var retorno = (from obj in lotes
                               select new
                               {
                                   obj.Codigo,
                                   obj.Numero,
                                   DataInicial = obj.DataInicial?.ToString("dd/MM/yyyy") ?? string.Empty,
                                   DataFinal = obj.DataFinal?.ToString("dd/MM/yyyy") ?? string.Empty,
                                   Situacao = obj.Situacao.ObterDescricao()
                               }).ToList();

                grid.AdicionaRows(retorno);
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCliente()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaCliente());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<Dominio.Entidades.Cliente> clientes = BuscarClientesSelecionados(unitOfWork);

                if (clientes.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum cliente encontrado para criar o lote.");

                Repositorio.Embarcador.Integracao.LoteCliente repLoteCliente = new Repositorio.Embarcador.Integracao.LoteCliente(unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente loteCliente = new Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente();

                unitOfWork.Start();

                loteCliente.DataInicial = Request.GetNullableDateTimeParam("DataInicio");
                loteCliente.DataFinal = Request.GetNullableDateTimeParam("DataFim");
                loteCliente.DataGeracaoLote = DateTime.Now;
                loteCliente.Situacao = SituacaoLoteCliente.AgIntegracao;
                loteCliente.Usuario = Usuario;
                loteCliente.Numero = repLoteCliente.BuscarUltimoNumero() + 1;

                loteCliente.Clientes = new List<Dominio.Entidades.Cliente>(clientes);

                repLoteCliente.Inserir(loteCliente, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { loteCliente.Codigo });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.LoteCliente repLoteCliente = new Repositorio.Embarcador.Integracao.LoteCliente(unitOfWork);

                Dominio.Entidades.Embarcador.Integracao.LoteCliente.LoteCliente loteCliente = repLoteCliente.BuscarPorCodigo(codigo, false);

                if (loteCliente == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    loteCliente.Codigo,
                    loteCliente.Numero,
                    loteCliente.Situacao,
                    DataInicial = loteCliente.DataInicial?.ToString("dd/MM/yyyy") ?? string.Empty,
                    DataFinal = loteCliente.DataFinal?.ToString("dd/MM/yyyy") ?? string.Empty,
                    DataGeracaoLote = loteCliente.DataGeracaoLote.ToString("dd/MM/yyyy")
                });
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

        private dynamic ExecutaPesquisaCliente(ref List<Dominio.Entidades.Cliente> listaGrid, ref int totalRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Repositorio.UnitOfWork unitOfWork, bool selecaoParaAdicionar = false)
        {
            Repositorio.Embarcador.Integracao.LoteCliente repLoteCliente = new Repositorio.Embarcador.Integracao.LoteCliente(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaClienteLoteCliente filtrosPesquisa = ObterFiltrosConsulta(selecaoParaAdicionar);

            listaGrid = repLoteCliente.ConsultarClientes(filtrosPesquisa, parametrosConsulta);
            totalRegistros = repLoteCliente.ContarConsultaClientes(filtrosPesquisa);

            if (listaGrid.Count > 0)
            {
                return (from p in listaGrid
                        select new
                        {
                            Codigo = p.CPF_CNPJ,
                            p.Nome,
                            CPF_CNPJ = p.CPF_CNPJ_Formatado,
                            p.IE_RG,
                            p.Endereco,
                            Localidade = p.Localidade.DescricaoCidadeEstado,
                            p.Telefone1,
                            DataCadastro = p.DataCadastro?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty
                        }).ToList();
            }

            return new List<dynamic>();
        }

        private Models.Grid.Grid ObterGridPesquisaCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Razão Social", "Nome", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CPF/CNPJ", "CPF_CNPJ", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("IE", "IE_RG", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Endereço", "Endereco", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Localidade", "Localidade", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Telefone", "Telefone1", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Atualização", "DataCadastro", 10, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<Dominio.Entidades.Cliente> listaGrid = new List<Dominio.Entidades.Cliente>();
                int totalRegistros = 0;

                dynamic lista = ExecutaPesquisaCliente(ref listaGrid, ref totalRegistros, parametrosConsulta, unitOfWork);

                grid.AdicionaRows(lista);
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

        private List<Dominio.Entidades.Cliente> BuscarClientesSelecionados(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Cliente> listaBusca = new List<Dominio.Entidades.Cliente>();

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
            {
                PropriedadeOrdenar = "Nome",
                DirecaoOrdenar = "asc"
            };

            int totalRegistros = 0;

            ExecutaPesquisaCliente(ref listaBusca, ref totalRegistros, parametrosConsulta, unitOfWork, true);

            return listaBusca;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaClienteLoteCliente ObterFiltrosConsulta(bool incluirMultiplaSelecao = false)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaClienteLoteCliente filtros = new Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaClienteLoteCliente()
            {
                CodigoLote = Request.GetIntParam("Codigo"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                RazaoSocial = Request.GetStringParam("RazaoSocial"),
                CNPJ = Utilidades.String.OnlyNumbers(Request.Params("CNPJ")),
                InscricaoEstadual = Request.GetStringParam("InscricaoEstadual"),
                Localidade = Request.GetIntParam("Localidade"),
                Endereco = Request.GetStringParam("Endereco")
            };

            if (incluirMultiplaSelecao)
            {
                filtros.SelecionarTodos = Request.GetBoolParam("SelecionarTodos");
                filtros.CodigosSelecionados = Request.GetListParam<double>("ClientesSelecionados");
            }

            return filtros;
        }

        #endregion
    }
}
