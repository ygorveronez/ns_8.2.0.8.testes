using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Checklists
{
    [CustomAuthorize("CheckListsUsuario/ConfigCheckListUsuario")]
    public class ConfigCheckListUsuarioController : BaseController
    {
		#region Construtores

		public ConfigCheckListUsuarioController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Checklist.Checklist repositorioChecklist = new Repositorio.Embarcador.Checklist.Checklist(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Checklist.FiltroPesquisaChecklist filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoGrot.Descricao, "Descricao", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoGrot.CodigoIntegracao, "CodigoIntegracao", 20, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Checklist.Checklist> checklists = repositorioChecklist.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repositorioChecklist.ContarConsulta(filtrosPesquisa));

                var lista = (from checklist in checklists
                             select new
                             {
                                 checklist.Codigo,
                                 checklist.Descricao,
                                 checklist.CodigoIntegracao,
                             }
                );

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                Repositorio.Embarcador.Checklist.Checklist repositorioChecklist = new Repositorio.Embarcador.Checklist.Checklist(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Checklist.Checklist checklist = repositorioChecklist.BuscarPorCodigo(codigo, false);

                if (checklist == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                var retorno = new
                {
                    checklist.Codigo,
                    checklist.Descricao,
                    checklist.CodigoIntegracao,
                    checklist.Ativo,
                    checklist.Observacao,
                    Perguntas = (from pergunta in checklist.Perguntas
                                 select new
                                 {
                                     pergunta.Codigo,
                                     pergunta.Descricao,
                                     pergunta.Ordem,
                                     pergunta.CodigoIntegracao,
                                     pergunta.TipoResposta,
                                     pergunta.Obrigatorio,
                                     pergunta.PermiteOpcaoNaoSeAplica,
                                     OpcoesSalvar = "",
                                     Opcoes = (from alternativa in pergunta.Alternativas
                                               select new
                                               {
                                                   CodigoOpcao = alternativa.Codigo,
                                                   DescricaoOpcao = alternativa.Descricao,
                                                   OrdemOpcao = alternativa.Ordem,
                                                   ValorOpcao = alternativa.Valor,
                                                   CodigoIntegracaoOpcao = alternativa.CodigoIntegracao,
                                                   OpcaoImpeditiva = alternativa.RespostaImpeditiva
                                               }).ToList(),
                                 }).ToList(),
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Checklist.Checklist repositorioChecklist = new Repositorio.Embarcador.Checklist.Checklist(unitOfWork);

                Dominio.Entidades.Embarcador.Checklist.Checklist checklist = new Dominio.Entidades.Embarcador.Checklist.Checklist();

                unitOfWork.Start();

                PreencherChecklist(ref checklist, unitOfWork);

                repositorioChecklist.Inserir(checklist, Auditado);

                SalvarPerguntas(checklist, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
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
                Repositorio.Embarcador.Checklist.Checklist repositorioChecklist = new Repositorio.Embarcador.Checklist.Checklist(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Checklist.Checklist checklist = repositorioChecklist.BuscarPorCodigo(codigo, false);

                if (checklist == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                PreencherChecklist(ref checklist, unitOfWork);
                SalvarPerguntas(checklist, unitOfWork);

                repositorioChecklist.Atualizar(checklist, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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
                Repositorio.Embarcador.Checklist.Checklist repositorioChecklist = new Repositorio.Embarcador.Checklist.Checklist(unitOfWork);
                Repositorio.Embarcador.Checklist.ChecklistPergunta repositorioChecklistPergunta = new Repositorio.Embarcador.Checklist.ChecklistPergunta(unitOfWork);
                Repositorio.Embarcador.Checklist.ChecklistPerguntaAlternativa repositorioChecklistPerguntaAlternativa = new Repositorio.Embarcador.Checklist.ChecklistPerguntaAlternativa(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Checklist.Checklist checklist = repositorioChecklist.BuscarPorCodigo(codigo, false);

                if (checklist == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                foreach (var checklistPergunta in checklist.Perguntas)
                    repositorioChecklistPerguntaAlternativa.DeletarPorEntidade(checklistPergunta);

                repositorioChecklistPergunta.DeletarPorEntidade(checklist);
                repositorioChecklist.Deletar(checklist, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherChecklist(ref Dominio.Entidades.Embarcador.Checklist.Checklist checklist, Repositorio.UnitOfWork unitOfWork)
        {
            checklist.Descricao = Request.GetStringParam("Descricao");
            checklist.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            checklist.Ativo = Request.GetBoolParam("Ativo");
            checklist.Observacao = Request.GetStringParam("Observacao");
        }

        private void SalvarPerguntas(Dominio.Entidades.Embarcador.Checklist.Checklist checklist, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Checklist.ChecklistPergunta repositorioChecklistPergunta = new Repositorio.Embarcador.Checklist.ChecklistPergunta(unitOfWork);

            dynamic dynPerguntas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Perguntas"));

            if (checklist.Perguntas?.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var pergunta in dynPerguntas)
                    if (pergunta.Codigo != null)
                        codigos.Add((int)pergunta.Codigo);

                List<Dominio.Entidades.Embarcador.Checklist.ChecklistPergunta> perguntasDeletar = (from obj in checklist.Perguntas where !codigos.Contains(obj.Codigo) select obj).ToList();

                foreach (var perguntaDeletar in perguntasDeletar)
                    repositorioChecklistPergunta.Deletar(perguntaDeletar, Auditado);
            }
            else
                checklist.Perguntas = new List<Dominio.Entidades.Embarcador.Checklist.ChecklistPergunta>();

            foreach (var pergunta in dynPerguntas)
            {
                Dominio.Entidades.Embarcador.Checklist.ChecklistPergunta checklistPergunta = pergunta.Codigo != null ? repositorioChecklistPergunta.BuscarPorCodigo((int)pergunta.Codigo, false) : null;
                if (checklistPergunta == null)
                    checklistPergunta = new Dominio.Entidades.Embarcador.Checklist.ChecklistPergunta();

                checklistPergunta.Descricao = (string)pergunta.Descricao;
                checklistPergunta.Ordem = ((string)pergunta.Ordem).ToInt();
                checklistPergunta.CodigoIntegracao = (string)pergunta.CodigoIntegracao;
                checklistPergunta.TipoResposta = ((string)pergunta.TipoResposta).ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList.Aprovacao);
                checklistPergunta.PermiteOpcaoNaoSeAplica = ((string)pergunta.PermiteOpcaoNaoSeAplica).ToBool();
                checklistPergunta.Checklist = checklist;

                if (checklistPergunta.Codigo > 0)
                    repositorioChecklistPergunta.Atualizar(checklistPergunta, Auditado);
                else
                    repositorioChecklistPergunta.Inserir(checklistPergunta, Auditado);

                SalvarOpcoes(checklistPergunta, (string)pergunta.OpcoesSalvar, unitOfWork);
            }
        }

        private void SalvarOpcoes(Dominio.Entidades.Embarcador.Checklist.ChecklistPergunta checklistPergunta, string Opcoes, Repositorio.UnitOfWork unitOfWork)
        {
            if (Opcoes != "")
            {
                dynamic dynAlternativas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Opcoes);

                Repositorio.Embarcador.Checklist.ChecklistPerguntaAlternativa repositorioChecklistPerguntaAlternativa = new Repositorio.Embarcador.Checklist.ChecklistPerguntaAlternativa(unitOfWork);

                if (checklistPergunta.Alternativas?.Count > 0)
                {
                    List<int> codigos = new List<int>();

                    foreach (var alternativa in dynAlternativas)
                        if (alternativa.Codigo != null)
                            codigos.Add(((string)alternativa.Codigo).ToInt());

                    List<Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa> alternativasDeletar = (from obj in checklistPergunta.Alternativas where !codigos.Contains(obj.Codigo) select obj).ToList();

                    foreach (var alternativaDeletar in alternativasDeletar)
                        repositorioChecklistPerguntaAlternativa.Deletar(alternativaDeletar, Auditado);
                }
                else
                    checklistPergunta.Alternativas = new List<Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa>();

                foreach (var alternativa in dynAlternativas)
                {
                    Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa checklistPerguntaAlternativa = repositorioChecklistPerguntaAlternativa.BuscarPorCodigo(((string)alternativa.Codigo).ToInt(), false);
                    if (checklistPerguntaAlternativa == null)
                        checklistPerguntaAlternativa = new Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa();

                    checklistPerguntaAlternativa.Descricao = (string)alternativa.Descricao;
                    checklistPerguntaAlternativa.Ordem = ((string)alternativa.Ordem).ToInt();
                    checklistPerguntaAlternativa.Valor = ((string)alternativa.Valor).ToInt();
                    checklistPerguntaAlternativa.RespostaImpeditiva = ((string)alternativa.OpcaoImpeditiva).ToBool();
                    checklistPerguntaAlternativa.CodigoIntegracao = (string)alternativa.CodigoIntegracao;
                    checklistPerguntaAlternativa.ChecklistPergunta = checklistPergunta;

                    if (checklistPerguntaAlternativa.Codigo > 0)
                        repositorioChecklistPerguntaAlternativa.Atualizar(checklistPerguntaAlternativa, Auditado);
                    else
                        repositorioChecklistPerguntaAlternativa.Inserir(checklistPerguntaAlternativa, Auditado);
                }
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Checklist.FiltroPesquisaChecklist ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Checklist.FiltroPesquisaChecklist()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Ativo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Ativo"),
            };
        }

        #endregion
    }
}
