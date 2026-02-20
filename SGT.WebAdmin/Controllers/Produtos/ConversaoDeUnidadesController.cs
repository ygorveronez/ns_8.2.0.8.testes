using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Produtos
{
    public class ConversaoDeUnidadesController : BaseController
    {
		#region Construtores

		public ConversaoDeUnidadesController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Publicos

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Produtos.ConversaoDeUnidade repositorioConversaoDeUnidade = new Repositorio.Embarcador.Produtos.ConversaoDeUnidade(unitOfWork);
                Repositorio.UnidadeDeMedida repositorioUnidadeMedida = new Repositorio.UnidadeDeMedida(unitOfWork);

                List<Dominio.Entidades.Embarcador.Produtos.ConversaoDeUnidade> listaConversoes = repositorioConversaoDeUnidade.BuscarTodos();

                dynamic conversoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Conversoes"));

                if (listaConversoes.Count > 0)
                {
                    List<int> codigosConversoes = new List<int>();

                    foreach (var conversao in conversoes)
                        if ((int)conversao.Codigo > 0)
                            codigosConversoes.Add((int)conversao.Codigo);

                    List<Dominio.Entidades.Embarcador.Produtos.ConversaoDeUnidade> listaConversoesRemover = listaConversoes.Where(c => !codigosConversoes.Contains(c.Codigo)).ToList();

                    foreach (Dominio.Entidades.Embarcador.Produtos.ConversaoDeUnidade conversaoRemover in listaConversoesRemover)
                        repositorioConversaoDeUnidade.Deletar(conversaoRemover);
                }

                foreach (var conversao in conversoes)
                {
                    Dominio.Entidades.Embarcador.Produtos.ConversaoDeUnidade existeConversao = repositorioConversaoDeUnidade.BuscarPorCodigo((int)conversao.Codigo, false);

                    if (existeConversao == null)
                        existeConversao = new Dominio.Entidades.Embarcador.Produtos.ConversaoDeUnidade();

                    Dominio.Entidades.UnidadeDeMedida unidadeDeMedida = repositorioUnidadeMedida.BuscarPorCodigo((int)conversao.CodigoUnidadePara);

                    if (unidadeDeMedida == null)
                        throw new ControllerException("Unidade de Medida (para) não existe");

                    existeConversao.Descricao = (string)conversao.DescricaoDe;
                    existeConversao.Sigla = (string)conversao.SiglaDe;
                    existeConversao.UnidadeDeMedida = unidadeDeMedida;

                    if (existeConversao.Codigo > 0)
                        repositorioConversaoDeUnidade.Atualizar(existeConversao);
                    else
                        repositorioConversaoDeUnidade.Inserir(existeConversao);
                }
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar adicionar um registro");
            }
        }

        public async Task<IActionResult> RemoverPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Produtos.ConversaoDeUnidade repositorioConversaoDeUnidade = new Repositorio.Embarcador.Produtos.ConversaoDeUnidade(unitOfWork);
                Dominio.Entidades.Embarcador.Produtos.ConversaoDeUnidade existeConversao = repositorioConversaoDeUnidade.BuscarPorCodigo(codigo, false);

                if (existeConversao == null)
                    return new JsonpResult("Não foi possivel achar o registro");

                repositorioConversaoDeUnidade.Deletar(existeConversao);
                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Resgistro Removido com sucesso.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar adicionar um registro");
            }
        }

        public async Task<IActionResult> ObterConversoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Produtos.ConversaoDeUnidade repositorioConversaoDeUnidade = new Repositorio.Embarcador.Produtos.ConversaoDeUnidade(unitOfWork);
                List<Dominio.Entidades.Embarcador.Produtos.ConversaoDeUnidade> listaConversoes = repositorioConversaoDeUnidade.BuscarTodos();

                dynamic listaConversoesExistentes = (from obj in listaConversoes
                                                     select new
                                                     {
                                                         obj.Codigo,
                                                         CodigoUnidadePara = obj.UnidadeDeMedida.Codigo,
                                                         SiglaDe = obj.Sigla,
                                                         DescricaoDe = obj.Descricao,
                                                         SiglaPara = obj.UnidadeDeMedida.Sigla,
                                                         DescricaoPara = obj.UnidadeDeMedida.Descricao
                                                     }).ToList();

                return new JsonpResult(listaConversoesExistentes);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar obter as conversões");
            }
        }

        public async Task<IActionResult> ObterListaConversoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Produtos.ConversaoDeUnidade repositorioConversaoDeUnidade = new Repositorio.Embarcador.Produtos.ConversaoDeUnidade(unitOfWork);
                List<Dominio.Entidades.Embarcador.Produtos.ConversaoDeUnidade> listaConversoes = repositorioConversaoDeUnidade.BuscarTodos();

                Models.Grid.Grid grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Conversão (de)", "ConversaoDe", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Conversão (para)", "ConversaoPara", 20, Models.Grid.Align.left, true);

                grid.setarQuantidadeTotal(listaConversoes.Count);
                grid.AdicionaRows((from obj in listaConversoes
                                   select new
                                   {
                                       obj.Codigo,
                                       ConversaoDe = $"({obj.Sigla}) {obj.Descricao}",
                                       ConversaoPara = $"({obj.UnidadeDeMedida.Sigla}) {obj.UnidadeDeMedida.Descricao}"
                                   }).ToList());

                return new JsonpResult(grid);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar obter as conversões");
            }
            #endregion
        }
    }
}
