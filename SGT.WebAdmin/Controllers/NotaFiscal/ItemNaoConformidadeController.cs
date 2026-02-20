using Dominio.Entidades.Embarcador.NotaFiscal;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.ItemNaoConformidade
{
    [CustomAuthorize("NotasFiscais/ItemNaoConformidade")]
    public class ItemNaoConformidadeController : BaseController
    {
		#region Construtores

		public ItemNaoConformidadeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Método Público

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade repositorioItemNaoConformidade = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaItemNaoConformidade filtrosPesquisa = ObterFiltrosPesquisaItemNaoConformidade();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Grupo", "Grupo", 20, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Sub Grupo", "SubGrupo", 20, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Área", "Area", 20, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Irrelevante para Não Conformidade", "IrrelevanteParaNC", 33, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Permite Contingência", "PermiteContingencia", 20, Models.Grid.Align.center);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repositorioItemNaoConformidade.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade> listaItemNaoConformidade = totalRegistros > 0 ? repositorioItemNaoConformidade.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade>();

                grid.AdicionaRows((
                    from o in listaItemNaoConformidade
                    select new
                    {
                        o.Codigo,
                        o.Descricao,
                        Grupo = o.Grupo.ObterDescricao(),
                        SubGrupo = o.SubGrupo.ObterDescricao(),
                        Area = o.Area.ObterDescricao(),
                        IrrelevanteParaNC = o.IrrelevanteParaNC.ObterDescricao(),
                        PermiteContingencia = o.PermiteContingencia.ObterDescricao()
                    }).ToList()
                );

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
        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade repItemNaoConformidade = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeParticipante repItemNaoConformidadeParticipante = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeParticipante(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao repItemNaoConformidadeTiposOperacao = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade itemNaoConformidade = repItemNaoConformidade.BuscarPorCodigo(codigo, true);

                if (itemNaoConformidade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();
                // remover itens em tabelas auxiliares 
                repItemNaoConformidadeParticipante.DeletarPorItemNaoConformidade(itemNaoConformidade.Codigo);
                repItemNaoConformidadeTiposOperacao.DeletarPorItemNaoConformidade(itemNaoConformidade.Codigo);

                PreencherEntidade(itemNaoConformidade, unitOfWork);
                PreencherTipoParticipante(itemNaoConformidade, unitOfWork);
                PreencherCamposTipoOperacao(itemNaoConformidade, unitOfWork);
                SalvarFiliaisItemNaoConformidade(itemNaoConformidade, unitOfWork);
                SalvarFornecedorItemNaoConformidade(itemNaoConformidade, unitOfWork);
                SalvarCFOPItemNaoConformidade(itemNaoConformidade, unitOfWork);

                repItemNaoConformidade.Atualizar(itemNaoConformidade, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                // Instancia repositorios
                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade repItemNaoConformidade = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeParticipante repItemNaoConformidadeParticipante = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeParticipante(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade itemNaoConformidade = new Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade();

                // Preenche entidade com dados
                PreencherEntidade(itemNaoConformidade, unitOfWork);

                // Persiste dados

                repItemNaoConformidade.Inserir(itemNaoConformidade, Auditado);

                PreencherTipoParticipante(itemNaoConformidade, unitOfWork);
                PreencherCamposTipoOperacao(itemNaoConformidade, unitOfWork);
                SalvarFiliaisItemNaoConformidade(itemNaoConformidade, unitOfWork);
                SalvarFornecedorItemNaoConformidade(itemNaoConformidade, unitOfWork);
                SalvarCFOPItemNaoConformidade(itemNaoConformidade, unitOfWork);


                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade repItemNaoConformidade = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao repItemNaoConformidadeTiposOperacao = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeParticipante repItemNaoConformidadeParticipante = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeParticipante(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade itemNaoConformidade = repItemNaoConformidade.BuscarPorCodigo(codigo);

                if (itemNaoConformidade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // remover itens em tabelas auxiliares 
                repItemNaoConformidadeParticipante.DeletarPorItemNaoConformidade(itemNaoConformidade.Codigo);
                repItemNaoConformidadeTiposOperacao.DeletarPorItemNaoConformidade(itemNaoConformidade.Codigo);

                unitOfWork.Start();
                repItemNaoConformidade.Deletar(itemNaoConformidade, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                if (((System.Data.SqlClient.SqlException)ex.InnerException).Number == 547)
                    return new JsonpResult(true, "O registro possui dependências e não pode ser excluído. Inative o item caso não queira mais utilizá-lo ");
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade repItemNaoConformidade = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(unitOfWork);


                Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade dadosItemNaoConformidade = repItemNaoConformidade.BuscarPorCodigo(codigo, true);

                if (dadosItemNaoConformidade == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
             
                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao repItemNaoConformidadeTipoOperacao = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP repItemNaoConformidadeCFOP = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeFornecedor repItemNaoConformidadeFornecedor = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeFornecedor(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeFilial repItemNaoConformidadeFilial = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeFilial(unitOfWork);

                List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP> cfopsVinculadas = repItemNaoConformidadeCFOP.BuscarPorItemNaoConformidade(codigo);
                List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao> tipoOperacaoVinculadas = repItemNaoConformidadeTipoOperacao.BuscarPorItemNaoConformidade(codigo);
                List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeFornecedor> FornecedorVinculadas = repItemNaoConformidadeFornecedor.BuscarPorItemNaoConformidade(codigo);
                List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeFilial> filiaisVinculadas = repItemNaoConformidadeFilial.BuscarPorItemNaoConformidade(codigo);

                dynamic dynPedido = new
                {
                    Descricao = dadosItemNaoConformidade.Descricao,
                    CodigoIntegracao = dadosItemNaoConformidade.CodigoIntegracao,
                    NotaFiscal = dadosItemNaoConformidade.NotaFiscal,
                    Grupo = dadosItemNaoConformidade.Grupo,
                    SubGrupo = dadosItemNaoConformidade.SubGrupo,
                    Area = dadosItemNaoConformidade.Area,
                    IrrelevanteParaNC = dadosItemNaoConformidade.IrrelevanteParaNC,
                    PermiteContingencia = dadosItemNaoConformidade.PermiteContingencia,
                    TipoRegra = dadosItemNaoConformidade.TipoRegra,
                    TipoOperacao = (from obj in tipoOperacaoVinculadas
                                    select new
                                    {
                                        obj.TipoOperacao.Codigo,
                                        obj.TipoOperacao.Descricao,
                                    }).ToList(),
                    Participacao = dadosItemNaoConformidade.TipoParticipante.Select(x => x.Participante).ToArray(),
                    CFOP = (from obj in cfopsVinculadas
                                    select new
                                    {
                                        obj.CFOP.Codigo,
                                        Descricao = obj.CFOP.CodigoCFOP.ToString(),
                                    }).ToList(),
                    Fornecedor = (from obj in FornecedorVinculadas
                                    select new
                                    {
                                        obj.Fornecedor.Codigo,
                                        obj.Fornecedor.Descricao,
                                    }).ToList(),
                    Filiais = (from obj in filiaisVinculadas
                                    select new
                                    {
                                        obj.Filial.Codigo,
                                        obj.Filial.Descricao,
                                    }).ToList(),
                    Status = dadosItemNaoConformidade.Status
                };

                return new JsonpResult(dynPedido);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Metódos Privados

        private Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaItemNaoConformidade ObterFiltrosPesquisaItemNaoConformidade()
        {
            return new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaItemNaoConformidade()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Status = Request.GetNullableBoolParam("Status"),
                Grupo = Request.GetNullableEnumParam<GrupoNC>("Grupo"),
                SubGrupo = Request.GetNullableEnumParam<SubGrupoNC>("SubGrupo"),
                Area = Request.GetNullableEnumParam<AreaNC>("Area"),
                IrrelevanteParaNC = Request.GetNullableBoolParam("IrrelevanteParaNC"),
                PermiteContingencia = Request.GetNullableBoolParam("PermiteContingencia")
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade itemNaoConformidade, Repositorio.UnitOfWork unitOfWork)
        {
            itemNaoConformidade.Descricao = Request.GetStringParam("Descricao");
            itemNaoConformidade.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            itemNaoConformidade.NotaFiscal = Request.GetStringParam("NotaFiscal");
            itemNaoConformidade.Grupo = Request.GetEnumParam<GrupoNC>("Grupo");
            itemNaoConformidade.SubGrupo = Request.GetEnumParam<SubGrupoNC>("SubGrupo");
            itemNaoConformidade.Area = Request.GetEnumParam<AreaNC>("Area");
            itemNaoConformidade.IrrelevanteParaNC = Request.GetBoolParam("IrrelevanteParaNC");
            itemNaoConformidade.PermiteContingencia = Request.GetBoolParam("PermiteContingencia");
            itemNaoConformidade.TipoRegra = Request.GetEnumParam<TipoRegraNaoConformidade>("TipoRegra");
            itemNaoConformidade.Status = Request.GetBoolParam("Status");
        }

        private void PreencherTipoParticipante(Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade itemNaoConformidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeParticipante repItemNaoConformidadeParticipante = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeParticipante(unitOfWork);
            var tr = itemNaoConformidade.TipoRegra;
            if (tr == TipoRegraNaoConformidade.EstendidoFilial || tr == TipoRegraNaoConformidade.Nacionalizacao || tr == TipoRegraNaoConformidade.ValidarRaiz
                || tr == TipoRegraNaoConformidade.ValidarCNPJ || tr == TipoRegraNaoConformidade.SituacaoCadastral)
            {
                var part = Request.GetListEnumParam<TipoParticipante>("Participacao");
                if (!part.IsNullOrEmpty())
                {

                    //itemNaoConformidade.TipoParticipante = new List<ItemNaoConformidadeParticipantes>();
                    foreach (var p in part.ToList())
                    {
                        Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeParticipantes itemParticipante = new ItemNaoConformidadeParticipantes();
                        itemParticipante.Participante = p;
                        itemParticipante.CodigoItemNaoConformidade = itemNaoConformidade;

                        repItemNaoConformidadeParticipante.Inserir(itemParticipante);
                    }
                }
            }
        }

        private void PreencherCamposTipoOperacao(Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade itemNaoConformidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao repItemNaoConformidadeOperacao = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            List<TipoRegraNaoConformidade> listaTipoRegrasPermitidos = new List<TipoRegraNaoConformidade>() {
             TipoRegraNaoConformidade.EstendidoFilial, TipoRegraNaoConformidade.Nacionalizacao,TipoRegraNaoConformidade.RecebedorNotaFiscal,TipoRegraNaoConformidade.LocalEntrega,TipoRegraNaoConformidade.RecebedorArmazenagem ,TipoRegraNaoConformidade.CapacidadeExcedida};

            if (!listaTipoRegrasPermitidos.Contains(itemNaoConformidade.TipoRegra) && !repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.Unilever))
                return;

            var tipos = Request.GetArrayParam<int>("TipoOperacao");
            if (!tipos.IsNullOrEmpty())
                foreach (var t in tipos)
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeTiposOperacao itemOperacao = new ItemNaoConformidadeTiposOperacao();
                    itemOperacao.TipoOperacao = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacao { Codigo = t };
                    itemOperacao.ItemNaoConformidade = itemNaoConformidade;

                    repItemNaoConformidadeOperacao.Inserir(itemOperacao);
                }
        }

        private void SalvarFiliaisItemNaoConformidade(Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade itemOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeFilial repositorioItemNaoConformidade = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeFilial(unitOfWork);

            List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeFilial> itemsNaoConformidadesFiliais = repositorioItemNaoConformidade.BuscarPorItemNaoConformidade(itemOperacao.Codigo);
            List<int> codigoProcessar = Request.GetListParam<int>("Filiais");

            if (itemsNaoConformidadesFiliais != null && itemsNaoConformidadesFiliais.Count > 0)
            {

                List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeFilial> filiaisRemover = itemsNaoConformidadesFiliais.Where(x => !codigoProcessar.Contains(x.Filial.Codigo)).ToList();
                foreach (var itemRemover in filiaisRemover)
                    repositorioItemNaoConformidade.Deletar(itemRemover);
            }

            foreach (var codigo in codigoProcessar)
            {
                if (itemsNaoConformidadesFiliais.Any(x => x.Filial.Codigo == codigo))
                    continue;

                Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeFilial novoRegistroItem = new ItemNaoConformidadeFilial()
                {
                    Filial = new Dominio.Entidades.Embarcador.Filiais.Filial() { Codigo = codigo },
                    ItemNaoConformidade = itemOperacao
                };

                repositorioItemNaoConformidade.Inserir(novoRegistroItem);
            }
        }

        private void SalvarFornecedorItemNaoConformidade(Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade itemOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeFornecedor repositorioItemNaoConformidade = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeFornecedor(unitOfWork);

            List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeFornecedor> itemsNaoConformidadesFiliais = repositorioItemNaoConformidade.BuscarPorItemNaoConformidade(itemOperacao.Codigo);
            List<double> codigoProcessar = Request.GetListParam<double>("Fornecedor");

            if (itemsNaoConformidadesFiliais != null && itemsNaoConformidadesFiliais.Count > 0)
            {

                List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeFornecedor> itemsRemover = itemsNaoConformidadesFiliais.Where(x => !codigoProcessar.Contains(x.Fornecedor.Codigo)).ToList();
                foreach (var itemRemover in itemsRemover)
                    repositorioItemNaoConformidade.Deletar(itemRemover);
            }

            foreach (var codigo in codigoProcessar)
            {
                if (itemsNaoConformidadesFiliais.Any(x => x.Fornecedor.Codigo == codigo))
                    continue;

                Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeFornecedor novoRegistroItem = new ItemNaoConformidadeFornecedor()
                {
                    Fornecedor = new Dominio.Entidades.Cliente() { CPF_CNPJ = codigo },
                    ItemNaoConformidade = itemOperacao
                };

                repositorioItemNaoConformidade.Inserir(novoRegistroItem);
            }
        }

        private void SalvarCFOPItemNaoConformidade(Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade itemOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP repositorioItemNaoConformidade = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP(unitOfWork);

            List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP> itemsNaoConformidadesCFOP = repositorioItemNaoConformidade.BuscarPorItemNaoConformidade(itemOperacao.Codigo);
            List<int> codigoProcessar = Request.GetListParam<int>("CFOP");

            if (itemsNaoConformidadesCFOP != null && itemsNaoConformidadesCFOP.Count > 0)
            {

                List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP> itemsRemover = itemsNaoConformidadesCFOP.Where(x => !codigoProcessar.Contains(x.CFOP.Codigo)).ToList();
                foreach (var itemRemover in itemsRemover)
                    repositorioItemNaoConformidade.Deletar(itemRemover);
            }

            foreach (var codigo in codigoProcessar)
            {
                if (itemsNaoConformidadesCFOP.Any(x => x.CFOP.Codigo == codigo))
                    continue;

                Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidadeCFOP novoRegistroItem = new ItemNaoConformidadeCFOP()
                {
                    CFOP = new Dominio.Entidades.CFOP() { Codigo = codigo },
                    ItemNaoConformidade = itemOperacao
                };

                repositorioItemNaoConformidade.Inserir(novoRegistroItem);
            }
        }


        #endregion
    }

}

