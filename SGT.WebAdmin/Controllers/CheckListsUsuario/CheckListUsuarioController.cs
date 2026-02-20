using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Checklists
{
    [CustomAuthorize("CheckListsUsuario/CheckListUsuario")]
    public class CheckListUsuarioController : BaseController
    {
		#region Construtores

		public CheckListUsuarioController(Conexao conexao) : base(conexao) { }

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
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Resposta", "Data", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);

                DateTime dataFinal = Request.GetDateTimeParam("DataFinal");
                DateTime dataInicial = Request.GetDateTimeParam("DataInicial");

                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                Repositorio.Embarcador.Checklist.CheckListResposta repositorioChecklist = new Repositorio.Embarcador.Checklist.CheckListResposta(unitOfWork);
                int totalRegistros = repositorioChecklist.ContarConsulta(dataInicial, dataFinal);

                List<Dominio.Entidades.Embarcador.Checklist.CheckListResposta> listaChecklist = null;

                if (totalRegistros > 0)
                    listaChecklist = repositorioChecklist.Consultar(dataInicial, dataFinal, parametrosConsulta);
                else
                    listaChecklist = new List<Dominio.Entidades.Embarcador.Checklist.CheckListResposta>();

                var listaChecklistRetornar = (
                    from checklist in listaChecklist
                    select new
                    {
                        checklist.Codigo,
                        checklist.Checklist.Descricao,
                        Data = checklist.DataResposta.ToString("dd/MM/yyyy"),
                        Situacao = checklist.DescricaoSituacao,
                    }).ToList();

                grid.AdicionaRows(listaChecklistRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Checklist.CheckListResposta repCheckListRespondido = new Repositorio.Embarcador.Checklist.CheckListResposta(unitOfWork);

                int codigoChecklist = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Checklist.CheckListResposta checklistRespondido = repCheckListRespondido.BuscarPorCodigo(codigoChecklist);

                if (checklistRespondido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o checkList respondido para a data pesquisada.");

                var checklistRetornar = new
                {
                    checklistRespondido.Codigo,
                    checklistRespondido.Checklist.Descricao,
                    DataResposta = checklistRespondido.DataResposta.ToString("dd/MM/yyyy"),
                    Pergunta = (
                        from pergunta in checklistRespondido.Perguntas
                        select new
                        {
                            pergunta.Codigo,
                            pergunta.Descricao
                        }
                    ).ToList(),
                    GrupoPerguntas = ObterGrupoPerguntasRespondido(checklistRespondido, unitOfWork),
                    Situacao = checklistRespondido.Situacao,
                    ObservacoesGerais = checklistRespondido.Observacoes
                };

                return new JsonpResult(checklistRetornar);
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

        public async Task<IActionResult> BuscarCheckList()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime dataCheckList = Request.GetDateTimeParam("Data");
                Repositorio.Embarcador.Checklist.Checklist repCheckList = new Repositorio.Embarcador.Checklist.Checklist(unitOfWork);
                Repositorio.Embarcador.Checklist.CheckListResposta repCheckListRespondido = new Repositorio.Embarcador.Checklist.CheckListResposta(unitOfWork);

                if (this.Usuario?.Setor?.Checklist == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o checkList vinculado ao setor do Usuário logado.");

                Dominio.Entidades.Embarcador.Checklist.CheckListResposta checklistRespondido = repCheckListRespondido.BuscarPorUsuarioEData(this.Usuario.Codigo, dataCheckList);
                if (checklistRespondido != null)
                {
                    var checklistRespondidoRetornar = new
                    {
                        checklistRespondido.Codigo,
                        checklistRespondido.Checklist.Descricao,
                        Pergunta = (
                            from pergunta in checklistRespondido.Perguntas
                            select new
                            {
                                pergunta.Codigo,
                                pergunta.Descricao
                            }
                        ).ToList(),
                        Respondido = true,
                        DataResposta = checklistRespondido.DataResposta.ToString("dd/MM/yyyy"),
                        GrupoPerguntas = ObterGrupoPerguntasRespondido(checklistRespondido, unitOfWork),
                        Situacao = checklistRespondido.Situacao,
                        ObservacoesGerais = checklistRespondido.Observacoes
                    };

                    return new JsonpResult(checklistRespondidoRetornar);
                }
                else
                {
                    Dominio.Entidades.Embarcador.Checklist.Checklist checklistResponder = repCheckList.BuscarPorCodigo(this.Usuario.Setor.Checklist.Codigo, false);

                    var checklistRetornar = new
                    {
                        checklistResponder.Codigo,
                        checklistResponder.Descricao,
                        Pergunta = (
                            from pergunta in checklistResponder.Perguntas
                            select new
                            {
                                pergunta.Codigo,
                                pergunta.Descricao
                            }
                        ).ToList(),
                        Respondido = false,
                        DataResposta = dataCheckList.ToString("dd/MM/yyyy"),
                        GrupoPerguntas = ObterGrupoPerguntasAResponder(checklistResponder, unitOfWork),
                        Situacao = SituacaoCheckList.Aberto,
                        ObservacoesGerais = ""
                    };

                    return new JsonpResult(checklistRetornar);
                }

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


        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Checklist.Checklist repCheckList = new Repositorio.Embarcador.Checklist.Checklist(unitOfWork);
                Repositorio.Embarcador.Checklist.CheckListResposta repCheckListResposta = new Repositorio.Embarcador.Checklist.CheckListResposta(unitOfWork);
                Dominio.Entidades.Embarcador.Checklist.Checklist checklist = repCheckList.BuscarPorCodigo(codigo, false);

                Dominio.Entidades.Embarcador.Checklist.CheckListResposta checkListRespondido = new Dominio.Entidades.Embarcador.Checklist.CheckListResposta();

                if (checklist == null)
                    throw new ControllerException("Não foi possível encontrar o checkList a ser respondido.");

                PreencherChecklistSalvar(checklist, ref checkListRespondido, unitOfWork);

                repCheckListResposta.Atualizar(checkListRespondido);

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
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o checklist.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherChecklistSalvar(Dominio.Entidades.Embarcador.Checklist.Checklist checklist, ref Dominio.Entidades.Embarcador.Checklist.CheckListResposta checklistRespondido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Checklist.CheckListResposta repCheckListResposta = new Repositorio.Embarcador.Checklist.CheckListResposta(unitOfWork);
            Repositorio.Embarcador.Checklist.CheckListRespostaPergunta repositorioChecklistRespostaPergunta = new Repositorio.Embarcador.Checklist.CheckListRespostaPergunta(unitOfWork);
            Repositorio.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa repositorioChecklistRespostaPerguntaAlternativa = new Repositorio.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa(unitOfWork);

            Repositorio.Embarcador.Checklist.ChecklistPergunta repositorioChecklistPergunta = new Repositorio.Embarcador.Checklist.ChecklistPergunta(unitOfWork);
            Repositorio.Embarcador.Checklist.ChecklistPerguntaAlternativa repositorioCheckListPerguntaAlternativa = new Repositorio.Embarcador.Checklist.ChecklistPerguntaAlternativa(unitOfWork);

            checklistRespondido = new Dominio.Entidades.Embarcador.Checklist.CheckListResposta()
            {
                Observacoes = "",
                Situacao = SituacaoCheckList.Finalizado,
                DataResposta = Request.GetDateTimeParam("Data"),
                Usuario = this.Usuario,
                Checklist = checklist
            };

            repCheckListResposta.Inserir(checklistRespondido);

            List<Dominio.Entidades.Embarcador.Checklist.ChecklistPergunta> perguntas = repositorioChecklistPergunta.BuscarPorChecklist(checklist.Codigo);
            List<int> codigosPerguntas = (from o in perguntas select o.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa> alternativas = repositorioCheckListPerguntaAlternativa.BuscarPorChecklistPorPerguntas(codigosPerguntas);

            dynamic grupoPerguntas = JsonConvert.DeserializeObject<dynamic>(Request.Params("GrupoPerguntas"));
            checklistRespondido.Observacoes = Request.GetStringParam("ObservacoesGerais");

            foreach (dynamic perguntaRespodida in grupoPerguntas[0].Perguntas)
            {

                Dominio.Entidades.Embarcador.Checklist.ChecklistPergunta pergunta = (from o in perguntas where o.Codigo == ((string)perguntaRespodida.Codigo).ToInt() select o).FirstOrDefault();

                if (pergunta == null)
                    continue;

                Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPergunta checklistRespostaPergunta = new Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPergunta()
                {
                    CheckListResposta = checklistRespondido,
                    Descricao = perguntaRespodida.Descricao,
                    Tipo = perguntaRespodida.Tipo,
                    Resposta = null,
                    Obrigatorio = perguntaRespodida.Obrigatorio,
                    Observacao = ""
                };

                repositorioChecklistRespostaPergunta.Inserir(checklistRespostaPergunta);

                List<Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa> alternativasPorPergunta = (from o in alternativas where o.ChecklistPergunta.Codigo == pergunta.Codigo select o).ToList();

                switch (pergunta.TipoResposta)
                {
                    case TipoOpcaoCheckList.Aprovacao:
                        var resposta = ((string)perguntaRespodida.Resposta).ToNullableEnum<CheckListResposta>();
                        checklistRespostaPergunta.Resposta = resposta;

                        break;

                    case TipoOpcaoCheckList.Opcoes:
                        List<string> respostasMarcadasOpcoes = new List<string>();

                        foreach (dynamic alternativaPorPergunta in perguntaRespodida.Alternativas)
                        {
                            Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa checklistRespostaPerguntaAlternativa = new Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa()
                            {
                                CheckListRespostaPergunta = checklistRespostaPergunta,
                                Descricao = alternativaPorPergunta.Descricao,
                                Ordem = 0,
                                OpcaoImpeditiva = alternativaPorPergunta.RespostaImpeditiva,
                                Marcado = false,
                            };

                            repositorioChecklistRespostaPerguntaAlternativa.Inserir(checklistRespostaPerguntaAlternativa);

                            Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa alternativa = (from o in alternativas where o.Codigo == ((string)alternativaPorPergunta.Codigo).ToInt() select o).FirstOrDefault();

                            if (alternativa != null)
                            {
                                bool? alternativaMarcada = ((string)alternativaPorPergunta.Marcado).ToNullableBool();

                                if (alternativaMarcada.HasValue)
                                {
                                    var marcado = ((string)alternativaPorPergunta.Marcado).ToBool();
                                    if (marcado)
                                        checklistRespostaPerguntaAlternativa.Marcado = true;

                                    repositorioChecklistRespostaPerguntaAlternativa.Atualizar(checklistRespostaPerguntaAlternativa);
                                }
                            }
                        }

                        break;

                    case TipoOpcaoCheckList.Selecoes:
                        int codigoAlternativaSelecionada = ((string)perguntaRespodida.Resposta).ToInt();

                        foreach (dynamic alternativaPorPergunta in perguntaRespodida.Alternativas)
                        {
                            if (((string)alternativaPorPergunta.Codigo)?.ToNullableInt() != null)
                            {

                                Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa checklistRespostaPerguntaAlternativa = new Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa()
                                {
                                    CheckListRespostaPergunta = checklistRespostaPergunta,
                                    Descricao = alternativaPorPergunta.Descricao,
                                    Ordem = alternativaPorPergunta.Ordem,
                                    OpcaoImpeditiva = alternativaPorPergunta.RespostaImpeditiva,
                                    Marcado = false,
                                };

                                repositorioChecklistRespostaPerguntaAlternativa.Inserir(checklistRespostaPerguntaAlternativa);

                                Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa alternativa = (from o in alternativas where o.Codigo == ((string)alternativaPorPergunta.Codigo).ToInt() select o).FirstOrDefault();

                                if (alternativa != null)
                                {
                                    bool marcado = (codigoAlternativaSelecionada == alternativa.Codigo);

                                    if (marcado)
                                        checklistRespostaPerguntaAlternativa.Marcado = true;

                                    repositorioChecklistRespostaPerguntaAlternativa.Atualizar(checklistRespostaPerguntaAlternativa);
                                }
                            }
                        }
                        break;

                    case TipoOpcaoCheckList.SimNao:
                        bool? respostaSimNao = ((string)perguntaRespodida.Resposta).ToNullableBool();
                        checklistRespostaPergunta.Resposta = respostaSimNao.HasValue ? (CheckListResposta?)(respostaSimNao.Value ? CheckListResposta.Aprovada : CheckListResposta.Reprovada) : null;

                        break;

                    case TipoOpcaoCheckList.Informativo:
                        checklistRespostaPergunta.Observacao = string.IsNullOrWhiteSpace(((string)perguntaRespodida.Resposta).Trim()) ? "" : ((string)perguntaRespodida.Resposta).Trim();

                        break;
                }

                repositorioChecklistRespostaPergunta.Atualizar(checklistRespostaPergunta);
            }
        }

        private List<dynamic> ObterGrupoPerguntasAResponder(Dominio.Entidades.Embarcador.Checklist.Checklist checklist, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Checklist.ChecklistPerguntaAlternativa repositorioCheckListCargaPerguntaAlternativa = new Repositorio.Embarcador.Checklist.ChecklistPerguntaAlternativa(unitOfWork);
            Repositorio.Embarcador.Checklist.ChecklistPergunta repositorioCheckListPergunta = new Repositorio.Embarcador.Checklist.ChecklistPergunta(unitOfWork);
            List<Dominio.Entidades.Embarcador.Checklist.ChecklistPergunta> perguntas = repositorioCheckListPergunta.BuscarPorChecklist(checklist.Codigo);

            List<int> codigosPerguntas = (from o in perguntas select o.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa> alternativas = repositorioCheckListCargaPerguntaAlternativa.BuscarPorChecklistPorPerguntas(codigosPerguntas);
            List<dynamic> perguntasRetornar = new List<dynamic>();
            List<dynamic> retorno = new List<dynamic>();

            foreach (Dominio.Entidades.Embarcador.Checklist.ChecklistPergunta pergunta in perguntas)
            {
                List<Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa> alternativasPorPergunta = (from o in alternativas where o.ChecklistPergunta.Codigo == pergunta.Codigo select o).ToList();
                List<dynamic> alternativasRetornar = new List<dynamic>();
                string resposta = "";

                switch (pergunta.TipoResposta)
                {
                    case TipoOpcaoCheckList.Aprovacao:
                        alternativasRetornar.Add(new { Codigo = (int)CheckListResposta.Aprovada, Descricao = CheckListResposta.Aprovada.ObterDescricao() });
                        alternativasRetornar.Add(new { Codigo = (int)CheckListResposta.Reprovada, Descricao = CheckListResposta.Reprovada.ObterDescricao() });
                        resposta = "";
                        break;

                    case TipoOpcaoCheckList.Opcoes:
                        foreach (Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa alternativa in alternativasPorPergunta)
                        {
                            alternativasRetornar.Add(new
                            {
                                alternativa.Codigo,
                                alternativa.Descricao,
                                alternativa.Valor,
                                alternativa.RespostaImpeditiva
                            });
                        }
                        break;

                    case TipoOpcaoCheckList.Selecoes:
                        foreach (Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa alternativa in alternativasPorPergunta)
                        {
                            alternativasRetornar.Add(new
                            {
                                alternativa.Codigo,
                                alternativa.Descricao,
                                alternativa.RespostaImpeditiva,
                                alternativa.Ordem
                            });
                            resposta = "";
                        }
                        break;

                    case TipoOpcaoCheckList.SimNao:
                        resposta = "";
                        break;

                    case TipoOpcaoCheckList.Informativo:
                        resposta = "";
                        break;
                }

                perguntasRetornar.Add(new
                {
                    pergunta.Codigo,
                    Descricao = pergunta.Descricao,
                    Tipo = pergunta.TipoResposta,
                    pergunta.Obrigatorio,
                    Observacao = "",
                    Enable = true,
                    PermiteNaoAplica = pergunta.PermiteOpcaoNaoSeAplica,
                    Alternativas = alternativasRetornar,
                    Resposta = resposta
                });
            }

            retorno.Add(new
            {
                Perguntas = perguntasRetornar
            });


            return retorno;
        }

        private List<dynamic> ObterGrupoPerguntasRespondido(Dominio.Entidades.Embarcador.Checklist.CheckListResposta checklist, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa repositorioCheckListCargaPerguntaAlternativa = new Repositorio.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa(unitOfWork);
            Repositorio.Embarcador.Checklist.CheckListRespostaPergunta repositorioCheckListPergunta = new Repositorio.Embarcador.Checklist.CheckListRespostaPergunta(unitOfWork);
            List<Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPergunta> perguntas = repositorioCheckListPergunta.BuscarPorCheckList(checklist.Codigo);

            List<int> codigosPerguntas = (from o in perguntas select o.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa> alternativas = repositorioCheckListCargaPerguntaAlternativa.BuscarPorPerguntas(codigosPerguntas);
            List<dynamic> perguntasRetornar = new List<dynamic>();
            List<dynamic> retorno = new List<dynamic>();

            foreach (Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPergunta pergunta in perguntas)
            {
                List<Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa> alternativasPorPergunta = (from o in alternativas where o.CheckListRespostaPergunta.Codigo == pergunta.Codigo select o).ToList();
                List<dynamic> alternativasRetornar = new List<dynamic>();
                string resposta = "";

                switch (pergunta.Tipo)
                {
                    case TipoOpcaoCheckList.Aprovacao:
                        alternativasRetornar.Add(new { Codigo = (int)CheckListResposta.Aprovada, Descricao = CheckListResposta.Aprovada.ObterDescricao() });
                        alternativasRetornar.Add(new { Codigo = (int)CheckListResposta.Reprovada, Descricao = CheckListResposta.Reprovada.ObterDescricao() });
                        resposta = pergunta.Resposta.HasValue ? ((int)pergunta.Resposta.Value).ToString() : "";
                        break;

                    case TipoOpcaoCheckList.Opcoes:
                        foreach (Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa alternativa in alternativasPorPergunta)
                        {
                            alternativasRetornar.Add(new
                            {
                                alternativa.Codigo,
                                alternativa.Descricao,
                                alternativa.Marcado,
                                RespostaImpeditiva = alternativa.OpcaoImpeditiva
                            });
                        }
                        break;

                    case TipoOpcaoCheckList.Selecoes:
                        foreach (Dominio.Entidades.Embarcador.Checklist.CheckListRespostaPerguntaAlternativa alternativa in alternativasPorPergunta)
                        {
                            alternativasRetornar.Add(new
                            {
                                alternativa.Codigo,
                                alternativa.Descricao,
                                RespostaImpeditiva = alternativa.OpcaoImpeditiva
                            });

                            if (alternativa.Marcado)
                                resposta = alternativa.Codigo.ToString();
                        }
                        break;

                    case TipoOpcaoCheckList.SimNao:
                        resposta = pergunta.Resposta.HasValue ? (pergunta.Resposta.Value == CheckListResposta.Aprovada ? "true" : "false") : "";
                        break;

                    case TipoOpcaoCheckList.Informativo:
                        resposta = pergunta.Observacao;
                        break;
                }

                perguntasRetornar.Add(new
                {
                    pergunta.Codigo,
                    Descricao = pergunta.Descricao,
                    Tipo = pergunta.Tipo,
                    pergunta.Obrigatorio,
                    Observacao = "",
                    Enable = false,
                    PermiteNaoAplica = false,
                    Alternativas = alternativasRetornar,
                    Resposta = resposta
                });
            }

            retorno.Add(new
            {
                Perguntas = perguntasRetornar
            });


            return retorno;
        }

        #endregion
    }
}
