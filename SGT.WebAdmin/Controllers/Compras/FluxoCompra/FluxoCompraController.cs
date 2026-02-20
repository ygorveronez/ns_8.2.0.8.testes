using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Compras.FluxoCompra
{
    [CustomAuthorize("Compras/FluxoCompra")]
    public class FluxoCompraController : BaseController
    {
        #region Construtores

        public FluxoCompraController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaFluxoCompra filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Operador", "Operador", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "Valor", 30, Models.Grid.Align.left, false);
                if (filtrosPesquisa.EtapaAtual == EtapaFluxoCompra.Todos)
                    grid.AdicionarCabecalho("Etapa Atual", "EtapaAtual", 15, Models.Grid.Align.center, false);
                if (filtrosPesquisa.SituacaoTratativa == SituacaoTratativaFluxoCompra.Todos)
                    grid.AdicionarCabecalho("Tratativa", "SituacaoTratativa", 15, Models.Grid.Align.center, false);
                if (filtrosPesquisa.Situacao == SituacaoFluxoCompra.Todos)
                    grid.AdicionarCabecalho("Situação", "Situacao", 15, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Compras.FluxoCompra> listaContratoFreteAcrescimoDesconto = repFluxoCompra.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repFluxoCompra.ContarConsulta(filtrosPesquisa));

                grid.AdicionaRows((from obj in listaContratoFreteAcrescimoDesconto
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.Numero,
                                       Data = obj.Data.ToDateTimeString(),
                                       Operador = obj.Usuario.Nome,
                                       obj.Valor,
                                       EtapaAtual = obj.EtapaAtual.ObterDescricao(),
                                       SituacaoTratativa = ObterSituacaoTratativa(obj.OrdensCompra),
                                       Situacao = obj.Situacao.ObterDescricao(),
                                       Fornecedor = ObterNomesFornecedores(obj.OrdensCompra)
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
        public async Task<IActionResult> PesquisaRequisicaoMercadoria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFluxoCompra = Request.GetIntParam("Codigo");
                int numeroInicial = Request.GetIntParam("NumeroInicial");
                int numeroFinal = Request.GetIntParam("NumeroFinal");
                int codigoMotivo = Request.GetIntParam("Motivo");
                int codigoProduto = Request.GetIntParam("Produto");
                int codigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : 0;

                bool clicouNaPesquisa = Request.GetBoolParam("ClicouNaPesquisa");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motivo", "MotivoCompra", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "Data", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("CodigoUsuario", false);

                Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarRequisicaoMercadoria);
                List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> listaRegistros = new List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria>();

                int quantidadeRegistros = !clicouNaPesquisa ? 0 : repFluxoCompra.ContarConsultaRequisicaoMercadoria(codigoFluxoCompra, numeroInicial, numeroFinal, codigoMotivo, codigoProduto, codigoEmpresa);
                if (quantidadeRegistros > 0)
                    listaRegistros = repFluxoCompra.ConsultarRequisicaoMercadoria(codigoFluxoCompra, numeroInicial, numeroFinal, codigoMotivo, codigoProduto, codigoEmpresa, parametrosConsulta);
                grid.setarQuantidadeTotal(quantidadeRegistros);

                var lista = (from p in listaRegistros
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 Filial = p.Filial.RazaoSocial,
                                 Usuario = p.Usuario.Nome,
                                 MotivoCompra = p.MotivoCompra.Descricao,
                                 Data = p.Data.ToDateString(),
                                 p.DescricaoSituacao,
                                 CodigoUsuario = p.Usuario.Codigo
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as requisições de mercadoria.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Iniciar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);
                Servicos.Embarcador.Compras.FluxoCompra serFluxoCompra = new Servicos.Embarcador.Compras.FluxoCompra(unitOfWork);

                int numeroInicial = Request.GetIntParam("NumeroInicial");
                int numeroFinal = Request.GetIntParam("NumeroFinal");
                int codigoMotivo = Request.GetIntParam("Motivo");
                int codigoProduto = Request.GetIntParam("Produto");
                int codigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : 0;

                bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");
                List<int> codigosRequisicoes = JsonConvert.DeserializeObject<List<int>>(Request.Params("ListaRequisicoesMercadoria"));

                if (codigosRequisicoes == null)
                    return new JsonpResult(false, true, "É necessário selecionar ao menos uma requisição.");

                List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> listaRequisicoes = repFluxoCompra.ObterRequisicoesMercadoria(selecionarTodos, codigosRequisicoes, numeroInicial, numeroFinal, codigoMotivo, codigoProduto, codigoEmpresa);

                if (listaRequisicoes.Count == 0)
                    return new JsonpResult(false, true, "Nenhuma requisição selecionada.");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra = new Dominio.Entidades.Embarcador.Compras.FluxoCompra();

                fluxoCompra.Data = DateTime.Now;
                fluxoCompra.Situacao = SituacaoFluxoCompra.Aberto;
                fluxoCompra.EtapaAtual = EtapaFluxoCompra.AprovacaoRequisicao;
                fluxoCompra.Numero = repFluxoCompra.BuscarProximoNumero(codigoEmpresa);
                fluxoCompra.Usuario = Usuario;
                fluxoCompra.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa : null;

                fluxoCompra.RequisicoesMercadoria = new List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria>();
                foreach (Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria requisicaoMercadoria in listaRequisicoes)
                    fluxoCompra.RequisicoesMercadoria.Add(requisicaoMercadoria);

                repFluxoCompra.Inserir(fluxoCompra, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(serFluxoCompra.ObterDetalhesFluxoCompra(fluxoCompra));
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao iniciar o fluxo de compra.");
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

                Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);
                Servicos.Embarcador.Compras.FluxoCompra serFluxoCompra = new Servicos.Embarcador.Compras.FluxoCompra(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra = repFluxoCompra.BuscarPorCodigo(codigo);

                if (fluxoCompra == null)
                    return new JsonpResult(false, true, "Fluxo de compra não encontrado.");

                return new JsonpResult(serFluxoCompra.ObterDetalhesFluxoCompra(fluxoCompra));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Compras.FluxoCompra repFluxoCompra = new Repositorio.Embarcador.Compras.FluxoCompra(unitOfWork);

                Dominio.Entidades.Embarcador.Compras.FluxoCompra fluxoCompra = repFluxoCompra.BuscarPorCodigo(codigo);

                if (fluxoCompra == null)
                    return new JsonpResult(false, true, "Fluxo de compra não encontrado.");

                if (fluxoCompra.Situacao != SituacaoFluxoCompra.Aberto)
                    return new JsonpResult(false, true, "Fluxo de compra só pode ser cancelado quando a situação estiver aberta.");

                unitOfWork.Start();

                fluxoCompra.Situacao = SituacaoFluxoCompra.Cancelado;

                foreach (var mercadoria in fluxoCompra.RequisicoesMercadoria)
                    mercadoria.Situacao = SituacaoRequisicaoMercadoria.Cancelado;

                foreach (var ordem in fluxoCompra.OrdensCompra)
                    ordem.Situacao = SituacaoOrdemCompra.Cancelada;

                repFluxoCompra.Atualizar(fluxoCompra);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, fluxoCompra, "Cancelou o fluxo de compra", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar o fluxo de compra.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaFluxoCompra ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaFluxoCompra()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                CodigoOrdemCompra = Request.GetIntParam("OrdemCompra"),
                CodigoCotacao = Request.GetIntParam("CotacaoCompra"),
                CodigoRequisicaoMercadoria = Request.GetIntParam("RequisicaoMercadoria"),
                CodigoUsuario = Request.GetIntParam("Operador"),
                SituacaoTratativa = Request.GetEnumParam<SituacaoTratativaFluxoCompra>("SituacaoTratativa"),
                Situacao = Request.GetEnumParam<SituacaoFluxoCompra>("Situacao"),
                EtapaAtual = Request.GetEnumParam<EtapaFluxoCompra>("EtapaAtual"),
                Produto = Request.GetIntParam("Produto"),
                Fornecedor = Request.GetDoubleParam("Fornecedor")
            };
        }

        private string ObterPropriedadeOrdenarRequisicaoMercadoria(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Equals("Filial"))
                propriedadeOrdenar = "Filial.RazaoSocial";
            else if (propriedadeOrdenar.Equals("Usuario"))
                propriedadeOrdenar = "Usuario.Nome";
            else if (propriedadeOrdenar.Equals("MotivoCompra"))
                propriedadeOrdenar = "MotivoCompra.Descricao";

            return propriedadeOrdenar;
        }

        private string ObterNomesFornecedores(ICollection<Dominio.Entidades.Embarcador.Compras.OrdemCompra> listaordemcompra)
        {
            string retorno = "";
            foreach (var ordemcompra in listaordemcompra)
                retorno += ordemcompra.Fornecedor.Nome + ", ";


            if (retorno != "")
                retorno = retorno.Remove(retorno.Length - 2, 2);

            return retorno;
        }

        private string ObterSituacaoTratativa(ICollection<Dominio.Entidades.Embarcador.Compras.OrdemCompra> listaOrdemCompra)
        {
            string retorno;

            retorno = listaOrdemCompra.Select(x => x.SituacaoTratativa.ObterDescricao()).FirstOrDefault();

            if (retorno == null)
                return string.Empty;

            return retorno;
        }

        #endregion
    }
}
