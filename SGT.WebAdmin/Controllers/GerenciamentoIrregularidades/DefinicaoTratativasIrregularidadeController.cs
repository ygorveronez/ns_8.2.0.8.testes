using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.GerenciamentoIrregularidadess
{
    [CustomAuthorize("GerenciamentoIrregularidades/DefinicaoTratativasIrregularidade")]
    public class DefinicaoTratativasIrregularidadeController : BaseController
    {
		#region Construtores

		public DefinicaoTratativasIrregularidadeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Portfólio", "Portfolio", 33, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Irregularidade", "Irregularidade", 33, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 33, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaDefinicaoTratativasIrregularidade filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade repDefinicao = new Repositorio.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade(unitOfWork);

                List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade> listaDefinicoes = repDefinicao.Consultar(filtrosPesquisa, parametroConsulta);

                var lista = (from o in listaDefinicoes
                             select new
                             {
                                 o.Codigo,
                                 Portfolio = o.PortfolioModuloControle.Descricao,
                                 Irregularidade = o.Irregularidade.Descricao,
                                 Situacao = o.Ativa ? SituacaoAtivaPesquisa.Ativa.ObterDescricao() : SituacaoAtivaPesquisa.Inativa.ObterDescricao(),
                             }).ToList();

                int totalRegistros = repDefinicao.ContarConsulta(filtrosPesquisa);
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
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
                Repositorio.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade repDefinicao = new Repositorio.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade(unitOfWork);
                Repositorio.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade repTratativa = new Repositorio.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade(unitOfWork);

                int codigoDefinicao = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade Definicao = repDefinicao.BuscarPorCodigo(codigoDefinicao);
                List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> listaTratativas = repTratativa.BuscarPorDefinicaoTratativas(codigoDefinicao);

                return new JsonpResult(new
                {
                    Definicao.Codigo,
                    PortfolioModuloControle = new { Definicao.PortfolioModuloControle.Codigo, Definicao.PortfolioModuloControle.Descricao },
                    Irregularidade = new { Definicao.Irregularidade.Codigo, Definicao.Irregularidade.Descricao },
                    Situacao = Definicao.Ativa,

                    TratativasIrregularidade = (from t in listaTratativas
                                                select new
                                                {
                                                    t.Codigo,
                                                    t.Sequencia,
                                                    Gatilho = t?.DefinicaoTratativasIrregularidade?.Irregularidade?.GatilhoIrregularidade,
                                                    Setor = t.Setor.Descricao,
                                                    CodigoSetor = t.Setor.Codigo,
                                                    t.ProximaSequencia,
                                                    GrupoOperacao = t.GrupoTipoOperacao?.Descricao,
                                                    CodigoGrupoOperacao = t.GrupoTipoOperacao?.Codigo,
                                                    t.InformarMotivo,

                                                    Motivos = (from m in t.Motivos
                                                               select new
                                                               {
                                                                   m.Codigo,
                                                                   m.Descricao,
                                                                   Situacao = m.Ativa ? SituacaoAtivoPesquisa.Ativo.ObterDescricao() : SituacaoAtivoPesquisa.Inativo.ObterDescricao()
                                                               }).ToList(),

                                                    Acoes = t.Acoes.ToList(),

                                                }).ToList()
                });
            }

            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoDefinicao = Request.GetIntParam("Codigo");
                int codigoPortfolio = Request.GetIntParam("PortfolioModuloControle");
                int codigoIrregularidade = Request.GetIntParam("Irregularidade");

                Repositorio.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade repDefinicao = new Repositorio.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade(unitOfWork);
                var definicao = new Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade();
                Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle repPortfolio = new Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle portfolio = codigoPortfolio > 0 ? repPortfolio.BuscarPorCodigo(codigoPortfolio) : throw new ControllerException("Não foi possível encontrar o Portfólio");
                Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade repIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade irregularidade = codigoIrregularidade > 0 ? repIrregularidade.BuscarPorCodigo(codigoIrregularidade) : throw new ControllerException("Não foi possível encontrar o Irregularidade");

                if (repDefinicao.ExisteDuplicidade(portfolio, irregularidade))
                    throw new ControllerException("Já existe uma Definição com o mesmo Portfólio e a mesma Irregularidade");

                PreencherDefinicao(definicao, portfolio, irregularidade);

                Repositorio.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade repTratativaIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade(unitOfWork);
                List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> tratativasFront = PreencherTratativaIrregularidades(definicao, unitOfWork);

                repDefinicao.Inserir(definicao, Auditado);

                foreach (var tratativa in tratativasFront)
                {
                    repTratativaIrregularidade.Inserir(tratativa, Auditado);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoDefinicao = Request.GetIntParam("Codigo");
                int codigoPortfolio = Request.GetIntParam("PortfolioModuloControle");
                int codigoIrregularidade = Request.GetIntParam("Irregularidade");

                Repositorio.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade repDefinicao = new Repositorio.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade definicao = codigoDefinicao > 0 ? repDefinicao.BuscarPorCodigo(codigoDefinicao) : throw new ControllerException("Não foi possível encontrar a Definição de Tratativas");
                Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle repPortfolio = new Repositorio.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle portfolio = codigoPortfolio > 0 ? repPortfolio.BuscarPorCodigo(codigoPortfolio) : throw new ControllerException("Não foi possível encontrar o Portfólio");
                Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade repIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade irregularidade = codigoIrregularidade > 0 ? repIrregularidade.BuscarPorCodigo(codigoIrregularidade) : throw new ControllerException("Não foi possível encontrar o Irregularidade");

                PreencherDefinicao(definicao, portfolio, irregularidade);

                Repositorio.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade repTratativa = new Repositorio.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade(unitOfWork);
                List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> tratativasBack = repTratativa.BuscarPorDefinicaoTratativas(codigoDefinicao);
                List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> tratativasFront = PreencherTratativaIrregularidades(definicao, unitOfWork);
                List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> tratativasDeletar = ObterListaDeletar(definicao, tratativasBack, tratativasFront);

                AtualizarListaBackLocal(tratativasBack, tratativasFront, tratativasDeletar);

                foreach (var tratativa in tratativasDeletar)
                {
                    repTratativa.Deletar(tratativa, Auditado);
                }

                foreach (var tratativa in tratativasBack)
                {
                    if (tratativa.Codigo > 0)
                    {
                        repTratativa.Atualizar(tratativa, Auditado);
                    }

                    else
                    {
                        repTratativa.Inserir(tratativa, Auditado);
                    }
                }

                repDefinicao.Atualizar(definicao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados
        private Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaDefinicaoTratativasIrregularidade ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.FiltroPesquisaDefinicaoTratativasIrregularidade()
            {
                CodigoPortfolio = Request.GetIntParam("PortfolioModuloControle"),
                CodigoIrregularidade = Request.GetIntParam("Irregularidade"),
                Situacao = Request.GetEnumParam<SituacaoAtivaPesquisa>("Situacao")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricatoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        private List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> PreencherTratativaIrregularidades(Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade definicao, Repositorio.UnitOfWork unitOfWork)
        {
            List<dynamic> tratativasFrontJSON = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("Tratativas"));
            return ConverterListaFront(definicao, tratativasFrontJSON, unitOfWork);
        }

        private void PreencherDefinicao(Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade definicao, Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle portfolio, Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.Irregularidade irregularidade)
        {
            definicao.Codigo = Request.GetIntParam("Codigo");
            definicao.PortfolioModuloControle = portfolio;
            definicao.Irregularidade = irregularidade;
            definicao.Ativa = Request.GetBoolParam("Situacao");
        }

        private List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> ObterListaDeletar(Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade definicao, List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> tratativasBack, List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> tratativasFront)
        {
            List<int> codigosFront = new List<int>();
            foreach (var l in tratativasFront)
            {
                codigosFront.Add(l.Codigo);
            }
            List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> listaDeletar = tratativasBack.FindAll(x => !codigosFront.Contains(x.Codigo)).ToList();

            return tratativasBack.FindAll(x => !codigosFront.Contains(x.Codigo)).ToList();
        }

        private List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> ConverterListaFront(Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.DefinicaoTratativasIrregularidade definicao, List<dynamic> tratativasFrontJSON, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);
            Repositorio.Embarcador.Pedidos.GrupoTipoOperacao repGupoTipoOperacao = new Repositorio.Embarcador.Pedidos.GrupoTipoOperacao(unitOfWork);
            Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade repMotivo = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade(unitOfWork);

            List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> tratativasFront = new List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade>();

            foreach (var tratativaFrontJSON in tratativasFrontJSON)
            {
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade tratativaFront = new Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade();

                tratativaFront.Codigo = (int)tratativaFrontJSON.Codigo;
                tratativaFront.DefinicaoTratativasIrregularidade = definicao;
                tratativaFront.Sequencia = (int)tratativaFrontJSON.Sequencia;
                tratativaFront.Setor = repSetor.BuscarPorCodigo((int)tratativaFrontJSON.CodigoSetor);
                tratativaFront.ProximaSequencia = !string.IsNullOrEmpty((string)tratativaFrontJSON.ProximaSequencia) ? (int)tratativaFrontJSON.ProximaSequencia : 0;
                tratativaFront.GrupoTipoOperacao = !string.IsNullOrEmpty((string)tratativaFrontJSON.CodigoGrupoOperacao) ? repGupoTipoOperacao.BuscarPorCodigo((int)tratativaFrontJSON.CodigoGrupoOperacao) : null;
                tratativaFront.InformarMotivo = !string.IsNullOrEmpty((string)tratativaFrontJSON.InformarMotivo) ? (bool)tratativaFrontJSON.InformarMotivo : false;
                tratativaFront.Motivos = repMotivo.BuscarPorCodigos(ConverterListaMotivos(tratativaFrontJSON.Motivos));
                tratativaFront.Acoes = ConverterListaAcoes(tratativaFrontJSON.Acoes);

                tratativasFront.Add(tratativaFront);
            }
            return tratativasFront;
        }

        private List<int> ConverterListaMotivos(dynamic motivos)
        {
            List<int> lista = new List<int>();

            foreach (var motivo in motivos)
            {
                lista.Add((int)motivo);
            }

            return lista;
        }

        private List<AcaoTratativaIrregularidade> ConverterListaAcoes(dynamic acoes)
        {
            List<AcaoTratativaIrregularidade> lista = new List<AcaoTratativaIrregularidade>();

            foreach (var acao in acoes)
            {
                lista.Add((AcaoTratativaIrregularidade)acao);
            }

            return lista;
        }

        private void AtualizarListaBackLocal(List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> tratativasBack, List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> tratativasFront, List<Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade> tratativasDeletar)
        {
            tratativasBack.RemoveAll(x => tratativasDeletar.Contains(x));

            foreach (var tratativa in tratativasFront)
            {
                if (tratativa.Codigo > 0)
                {
                    tratativa.CopyProperties(tratativasBack.Find(x => x.Codigo == tratativa.Codigo));
                }
                else if (tratativa.Codigo == 0)
                {
                    tratativasBack.Add(tratativa);
                }
            }
        }

        #endregion
    }
}
