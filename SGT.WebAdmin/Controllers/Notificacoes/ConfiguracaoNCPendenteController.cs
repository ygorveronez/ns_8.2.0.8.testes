using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Notificacoes
{
    [CustomAuthorize("Notificacoes/ConfiguracaoNCPendente")]
    public class ConfiguracaoNCPendenteController : BaseController
    {
		#region Construtores

		public ConfiguracaoNCPendenteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Notificacoes.ConfiguracaoNCPendente repositoriocConfiguracaoNCPendente = new Repositorio.Embarcador.Notificacoes.ConfiguracaoNCPendente(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente configuracaoNCPendente = new Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente();

                PreencherConfiguracaoNCPendente(configuracaoNCPendente, unitOfWork);

                unitOfWork.Start();

                if (!VerificarDuplicidade(configuracaoNCPendente, repositoriocConfiguracaoNCPendente))
                    repositoriocConfiguracaoNCPendente.Inserir(configuracaoNCPendente, Auditado);
                else
                    throw new ControllerException("Já existe uma configuração de e-mail com esses dados!");


                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dados.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Notificacoes.ConfiguracaoNCPendente repositoriocConfiguracaoNCPendente = new Repositorio.Embarcador.Notificacoes.ConfiguracaoNCPendente(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente configuracaoNCPendente = repositoriocConfiguracaoNCPendente.BuscarPorCodigo(codigo, auditavel: true);

                if (configuracaoNCPendente == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                PreencherConfiguracaoNCPendente(configuracaoNCPendente, unitOfWork);

                unitOfWork.Start();

                if (!VerificarDuplicidade(configuracaoNCPendente, repositoriocConfiguracaoNCPendente))
                    repositoriocConfiguracaoNCPendente.Atualizar(configuracaoNCPendente, Auditado);
                else
                    throw new ControllerException("Não foi possível atualizar, pois já existe um registro com esses dados");

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Notificacoes.ConfiguracaoNCPendente repositoriocConfiguracaoNCPendente = new Repositorio.Embarcador.Notificacoes.ConfiguracaoNCPendente(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente configuracaoNCPendente = repositoriocConfiguracaoNCPendente.BuscarPorCodigo(codigo, auditavel: true);

                if (configuracaoNCPendente == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repositoriocConfiguracaoNCPendente.Deletar(configuracaoNCPendente, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(excecao))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);

                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
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
                Repositorio.Embarcador.Notificacoes.ConfiguracaoNCPendente repositoriocConfiguracaoNCPendente = new Repositorio.Embarcador.Notificacoes.ConfiguracaoNCPendente(unitOfWork);
                Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente configuracaoNCPendente = repositoriocConfiguracaoNCPendente.BuscarPorCodigo(codigo, auditavel: false);

                if (configuracaoNCPendente == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                return new JsonpResult(new
                {
                    configuracaoNCPendente.Codigo,
                    configuracaoNCPendente.Nome,
                    configuracaoNCPendente.Tipo,
                    NotificarTransportador = configuracaoNCPendente.Ativo,
                    ListaUsuario = (
                        from usuario in configuracaoNCPendente.Usuarios
                        select new
                        {
                            usuario.Codigo,
                            usuario.Descricao
                        }).ToList(),
                    ListaSetor = (
                        from setor in configuracaoNCPendente.Setor
                        select new
                        {
                            setor.Codigo,
                            setor.Descricao
                        }).ToList(),
                    ListaFiliais = (
                        from filial in configuracaoNCPendente.Filial
                        select new
                        {
                            filial.Codigo,
                            filial.Descricao
                        }).ToList(),
                    ListaItensNC = (
                        from itemNC in configuracaoNCPendente.ItemNaoConformidade
                        select new
                        {
                            itemNC.Codigo,
                            itemNC.Descricao
                        }).ToList(),
                    ListaTiposOperacao = (
                        from tipoOperacao in configuracaoNCPendente.TipoOperacao
                        select new
                        {
                            tipoOperacao.Codigo,
                            tipoOperacao.Descricao
                        }).ToList()
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
        private bool VerificarDuplicidade(Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente configuracaoNCPendente, Repositorio.Embarcador.Notificacoes.ConfiguracaoNCPendente repositoriocConfiguracaoNCPendente)
        {
            return repositoriocConfiguracaoNCPendente.ExisteDuplicado(configuracaoNCPendente);
        }

        private void PreencherConfiguracaoNCPendente(Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente configuracaoNCPendente, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade repItemNC = new Repositorio.Embarcador.NotaFiscal.ItemNaoConformidade(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);


            List<int> codigosSetor = Request.GetListParam<int>("Setores");
            List<int> codigosFilial = Request.GetListParam<int>("Filiais");
            List<int> codigosItemNC = Request.GetListParam<int>("ItensNC");
            List<int> codigosTipoOperacao = Request.GetListParam<int>("TiposOperacao");
            List<int> codigosUsuario = Request.GetListParam<int>("Usuarios");

            List<Dominio.Entidades.Setor> listaSetores = codigosSetor.Count > 0 ? repSetor.BuscarPorCodigos(codigosSetor) : new List<Dominio.Entidades.Setor>();
            List<Dominio.Entidades.Embarcador.Filiais.Filial> listaFiliais = codigosFilial.Count > 0 ? repFilial.BuscarPorCodigos(codigosFilial) : new List<Dominio.Entidades.Embarcador.Filiais.Filial>();
            List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> listaTiposOperacao = codigosTipoOperacao.Count > 0 ? repTipoOperacao.BuscarPorCodigos(codigosTipoOperacao) : new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();
            List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade> listaItensNC = codigosItemNC.Count > 0 ? repItemNC.BuscarPorCodigos(codigosItemNC) : new List<Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade>();
            List<Dominio.Entidades.Usuario> listaUsuario = codigosUsuario.Count > 0 ? repUsuario.BuscarPorCodigos(codigosUsuario) : new List<Dominio.Entidades.Usuario>();

            configuracaoNCPendente.Nome = Request.GetStringParam("Nome");
            configuracaoNCPendente.Tipo = Request.GetEnumParam<TipoConfiguracaoNCPendente>("Tipo");
            configuracaoNCPendente.Ativo = Request.GetBoolParam("Situacao");
            configuracaoNCPendente.Setor = listaSetores;
            configuracaoNCPendente.Filial = listaFiliais;
            configuracaoNCPendente.TipoOperacao = listaTiposOperacao;
            configuracaoNCPendente.ItemNaoConformidade = listaItensNC;
            configuracaoNCPendente.Usuarios = listaUsuario;        
        }

        private Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaConfiguracaoNCPendente ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaConfiguracaoNCPendente()
            {
                Nome = Request.GetStringParam("Nome"),
                Situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("Situacao"),

            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaConfiguracaoNCPendente filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nome", "Nome", 50, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 50, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 50, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                Repositorio.Embarcador.Notificacoes.ConfiguracaoNCPendente repositorioalertaEmail = new Repositorio.Embarcador.Notificacoes.ConfiguracaoNCPendente(unitOfWork);
                int totalRegistros = repositorioalertaEmail.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente> listaConfiguracaoNC = (totalRegistros > 0) ? repositorioalertaEmail.Consultar(parametrosConsulta, filtrosPesquisa) : new List<Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoNCPendente>();

                var listaConfiguracaoNCRetorno = (
                    from configuracaoNCPendente in listaConfiguracaoNC
                    select new
                    {
                        configuracaoNCPendente.Codigo,
                        configuracaoNCPendente.Nome,
                        Tipo = configuracaoNCPendente.Tipo.ObterDescricao(),
                        Situacao = configuracaoNCPendente.Ativo ? SituacaoAtivoPesquisa.Ativo.ObterDescricao() : SituacaoAtivoPesquisa.Inativo.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaConfiguracaoNCRetorno);
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
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }
        #endregion
    }
}
