using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Bidding.RFI
{
    [CustomAuthorize("Bidding/RFIConvite")]
    public class RFIConviteController : BaseController
    {
		#region Construtores

		public RFIConviteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Bidding.RFI.RFIConvite repositorioRFIConvite = new Repositorio.Embarcador.Bidding.RFI.RFIConvite(unitOfWork);

                Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite RFIConvite = new Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite();

                DateTime prazoAceite = Request.GetDateTimeParam("PrazoAceiteConvite");
                DateTime dataLimite = Request.GetDateTimeParam("DataLimite");

                if (prazoAceite > dataLimite)
                    return new JsonpResult(false, true, "Prazo para aceite do convite não pode ser maior que a Data Limite.");

                PreencherEntidade(RFIConvite);

                repositorioRFIConvite.Inserir(RFIConvite, Auditado);
                SalvarChecklist(RFIConvite, unitOfWork, false);
                SalvarConvidados(RFIConvite, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, RFIConvite, null, "Adicionou RFI convite.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    RFIConvite.Codigo,
                });

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

        public async Task<IActionResult> Pesquisar()
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                bool duplicar = Request.GetBoolParam("Duplicar");

                Repositorio.Embarcador.Bidding.RFI.RFIConvite repositorioRFIConvite = new Repositorio.Embarcador.Bidding.RFI.RFIConvite(unitOfWork);
                Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite RFIConvite = repositorioRFIConvite.BuscarPorCodigo(codigo, false);

                Repositorio.Embarcador.Bidding.RFI.RFIConviteConvidado repositorioConvidado = new Repositorio.Embarcador.Bidding.RFI.RFIConviteConvidado(unitOfWork);
                Repositorio.Embarcador.Bidding.RFI.RFIConviteAnexo repositorioConviteAnexo = new Repositorio.Embarcador.Bidding.RFI.RFIConviteAnexo(unitOfWork);

                List<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteConvidado> listaConvidados = repositorioConvidado.BuscarConvidados(RFIConvite);
                List<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteAnexo> listaAnexos = repositorioConviteAnexo.BuscarPorConvite(RFIConvite);

                if (RFIConvite == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new Dominio.ObjetosDeValor.Embarcador.Bidding.RFI.RFIConvite()
                {
                    Codigo = duplicar ? 0 : RFIConvite.Codigo,
                    Iniciado = RFIConvite.DataInicio <= DateTime.Now,
                    Situacao = RFIConvite.Situacao,
                    Descricao = RFIConvite.Descricao,
                    DataInicio = RFIConvite.DataInicio.ToString(),
                    DataLimite = RFIConvite.DataLimite.ToString(),
                    DescritivoConvite = RFIConvite.DescritivoConvite,
                    ExigirPreenchimentoChecklistConvitePeloTransportador = RFIConvite.ExigirPreenchimentoChecklistConvitePeloTransportador,
                    PrazoAceiteConvite = RFIConvite.DataPrazoAceiteConvite.ToString("dd/MM/yyyy HH:mm"),
                    TempoRestante = RFIConvite.DataPrazoAceiteConvite,
                    Etapa = RFIConvite.Status,
                    Convidados = (
                        from convidado in listaConvidados
                        select new Dominio.ObjetosDeValor.Embarcador.Bidding.RFI.RFIConvidado()
                        {
                            Codigo = convidado.Convidado.Codigo,
                            Descricao = convidado.Convidado.Descricao,
                            Status = convidado.Status
                        }
                    ).ToList(),
                    Anexos = (
                        from o in listaAnexos
                        select new Dominio.ObjetosDeValor.Embarcador.Bidding.RFI.RFIConviteAnexo
                        {
                            Codigo = o.Codigo,
                            Descricao = o.Descricao,
                            NomeArquivo = o.NomeArquivo
                        }
                    ).ToList(),
                    RFIChecklist = BuscarCheckList(RFIConvite.Codigo, unitOfWork)
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Bidding.RFI.RFIConvite repositorioRFIConvite = new Repositorio.Embarcador.Bidding.RFI.RFIConvite(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite RFIConvite = repositorioRFIConvite.BuscarPorCodigo(codigo, false);

                if (RFIConvite == null)
                    return new JsonpResult(false, true, "RFI Convite não foi encontrado.");

                RFIConvite.Initialize();

                DateTime prazoAceite = Request.GetDateTimeParam("PrazoAceiteConvite");
                DateTime dataLimite = Request.GetDateTimeParam("DataLimite");

                if (prazoAceite > dataLimite)
                    return new JsonpResult(false, true, "Prazo para aceite do convite não pode ser maior que a Data Limite.");

                PreencherEntidade(RFIConvite);
                SalvarChecklist(RFIConvite, unitOfWork);
                SalvarConvidados(RFIConvite, unitOfWork);
                repositorioRFIConvite.Atualizar(RFIConvite, Auditado);
                unitOfWork.CommitChanges();

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

        public async Task<IActionResult> ExcluirQuestionario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Bidding.RFI.RFIChecklistQuestionario repositorioRFIQuestionario = new Repositorio.Embarcador.Bidding.RFI.RFIChecklistQuestionario(unitOfWork);
                Repositorio.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAnexo repositorioRFIQuestionarioAnexo = new Repositorio.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAnexo(unitOfWork);
                Repositorio.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAlternativa repositorioRFIQuestionarioAlternativas = new Repositorio.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAlternativa(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionario RFIQuestionario = repositorioRFIQuestionario.BuscarPorCodigo(codigo, false);
                repositorioRFIQuestionarioAnexo.DeletarPorQuestionario(RFIQuestionario.Codigo);
                repositorioRFIQuestionarioAlternativas.DeletarPorQuestionario(RFIQuestionario.Codigo);
                repositorioRFIQuestionario.Deletar(RFIQuestionario);

                unitOfWork.CommitChanges();

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

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Etapa", "Status", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Inicial", "DataInicio", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Limite", "DataLimite", 10, Models.Grid.Align.center, true);

                Repositorio.Embarcador.Bidding.RFI.RFIConvite repRFIConvite = new Repositorio.Embarcador.Bidding.RFI.RFIConvite(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Bidding.RFI.FiltroPesquisaRFI filtrosPesquisa = ObterFiltrosPesquisa();

                int totalRegistros = repRFIConvite.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite> listaRFIConvite = repRFIConvite.Consultar(filtrosPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);

                var retorno = (from convite in listaRFIConvite
                               select new
                               {
                                   convite.Codigo,
                                   convite.Descricao,
                                   Status = convite.Status.ObterDescricao(),
                                   convite.DataInicio,
                                   convite.DataLimite
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Bidding.RFI.FiltroPesquisaRFI ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Bidding.RFI.FiltroPesquisaRFI()
            {
                descricao = Request.Params("Descricao"),
                dataInicio = Request.GetDateTimeParam("DataInicio"),
                dataLimite = Request.GetDateTimeParam("DataLimite"),
                empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Usuario.Empresa : null,
                situacao = Request.GetNullableListParam<StatusRFIConvite>("situacao")
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite grupoRFIConvite)
        {
            grupoRFIConvite.Situacao = Request.GetBoolParam("Situacao");
            grupoRFIConvite.Descricao = Request.GetStringParam("Descricao");
            grupoRFIConvite.DataInicio = Request.GetDateTimeParam("DataInicio");
            grupoRFIConvite.DataLimite = Request.GetDateTimeParam("DataLimite");
            grupoRFIConvite.DescritivoConvite = Request.GetStringParam("DescritivoConvite");
            grupoRFIConvite.ExigirPreenchimentoChecklistConvitePeloTransportador = Request.GetBoolParam("ExigirPreenchimentoChecklistConvitePeloTransportador");
            grupoRFIConvite.DataPrazoAceiteConvite = Request.GetDateTimeParam("PrazoAceiteConvite");
            grupoRFIConvite.Status = grupoRFIConvite.ExigirPreenchimentoChecklistConvitePeloTransportador ? StatusRFIConvite.Checklist : StatusRFIConvite.Aguardando;
        }

        private Dominio.ObjetosDeValor.Embarcador.Bidding.RFI.RFIChecklist BuscarCheckList(int codigoRFIConvite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Bidding.RFI.RFIChecklist repositorioChecklist = new Repositorio.Embarcador.Bidding.RFI.RFIChecklist(unitOfWork);
            Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklist checklist = repositorioChecklist.BuscarChecklistPorRFIConviteCodigo(codigoRFIConvite);

            if (checklist == null)
                throw new ControllerException("Não foi possível encontrar o checkList.");

            Repositorio.Embarcador.Bidding.RFI.RFIChecklistQuestionario repositorioQuestionarios = new Repositorio.Embarcador.Bidding.RFI.RFIChecklistQuestionario(unitOfWork);
            Repositorio.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAnexo repositorioQuestionariosAnexo = new Repositorio.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAnexo(unitOfWork);
            Repositorio.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAlternativa repositorioQuestionariosAlternativa = new Repositorio.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAlternativa(unitOfWork);

            List<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionario> listaQuestionarios = repositorioQuestionarios.BuscarQuestionarios(checklist);

            List<int> codigosQuestionarios = listaQuestionarios.Select(x => x.Codigo).ToList();

            List<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAnexo> questionariosAnexos = repositorioQuestionariosAnexo.BuscarPorQuestionarios(codigosQuestionarios);
            List<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAlternativa> questionariosAlternativas = repositorioQuestionariosAlternativa.BuscarPorQuestionarios(codigosQuestionarios);

            return new Dominio.ObjetosDeValor.Embarcador.Bidding.RFI.RFIChecklist()
            {
                Codigo = checklist.Codigo,
                Prazo = checklist.DataPrazo.ToString("dd/MM/yyyy HH:mm"),
                Questionarios = (
                    from questionario in listaQuestionarios
                    select new Dominio.ObjetosDeValor.Embarcador.Bidding.RFI.RFIQuestionario()
                    {
                        Codigo = questionario.Codigo,
                        Descricao = questionario.Descricao,
                        Requisito = questionario.Requisito.ObterDescricao(),
                        TipoOpcaoCheckListRFI = questionario.Tipo.ObterDescricao(),
                        GridMultiplaEscolha = (from ChecklistMultiplaEscolha in questionariosAlternativas
                                               where ChecklistMultiplaEscolha.RFIChecklistQuestionario.Codigo == questionario.Codigo
                                               select new Dominio.ObjetosDeValor.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAlternativa()
                                               {
                                                   Codigo = ChecklistMultiplaEscolha.Codigo,
                                                   Descricao = ChecklistMultiplaEscolha.Descricao,
                                                   Ordem = ChecklistMultiplaEscolha.Ordem
                                               }).ToList(),
                        ChecklistAnexo = (from ChecklistAnexo in questionariosAnexos
                                          where ChecklistAnexo.EntidadeAnexo.Codigo == questionario.Codigo
                                          select new Dominio.ObjetosDeValor.Embarcador.Bidding.RFI.RFIChecklistAnexo()
                                          {
                                              Codigo = ChecklistAnexo.Codigo,
                                              Descricao = ChecklistAnexo.Descricao,
                                              NomeArquivo = ChecklistAnexo.NomeArquivo
                                          }).ToList()
                    }
                ).ToList()
            };
        }

        private void SalvarChecklist(Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite RFIConvite, Repositorio.UnitOfWork unitOfWork, bool atualizar = true)
        {
            dynamic checklistPrazo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PrazoChecklist"));
            Repositorio.Embarcador.Bidding.RFI.RFIChecklist repositorioRFIChecklist = new Repositorio.Embarcador.Bidding.RFI.RFIChecklist(unitOfWork);

            Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklist RFIChecklist;

            if (RFIConvite.Codigo > 0 && atualizar)
                RFIChecklist = repositorioRFIChecklist.BuscarChecklistPorRFIConviteCodigo(RFIConvite.Codigo);
            else
                RFIChecklist = new Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklist();

            RFIChecklist.DataPrazo = ((string)checklistPrazo.PrazoChecklist).ToDateTime();
            RFIChecklist.RFIConvite = RFIConvite;
            RFIChecklist.DataLimite = RFIChecklist.DataPrazo;

            if (RFIConvite.ExigirPreenchimentoChecklistConvitePeloTransportador)
            {
                RFIChecklist.DataPrazo = RFIConvite.DataPrazoAceiteConvite;
                RFIChecklist.DataLimite = RFIConvite.DataPrazoAceiteConvite;
            }

            if (RFIChecklist.Codigo > 0)
            {
                repositorioRFIChecklist.Atualizar(RFIChecklist);
                RFIConvite.SetExternalChanges(RFIChecklist.GetCurrentChanges());
            }
            else
                repositorioRFIChecklist.Inserir(RFIChecklist);

            SalvarQuestionarios(RFIChecklist, unitOfWork, atualizar);
        }

        private void SalvarConvidados(Dominio.Entidades.Embarcador.Bidding.RFI.RFIConvite RFIConvite, Repositorio.UnitOfWork unitOfWork, bool atualizar = true)
        {
            dynamic convite = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Convite"));

            Repositorio.Embarcador.Bidding.RFI.RFIConviteConvidado repositorioConvidado = new Repositorio.Embarcador.Bidding.RFI.RFIConviteConvidado(unitOfWork);
            Repositorio.RepositorioBase<Dominio.Entidades.Empresa> repositorioEmpresa = new Repositorio.RepositorioBase<Dominio.Entidades.Empresa>(unitOfWork);

            Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteConvidado RFIConvidado;
            List<int> codigosConvidados = new List<int>();

            int codigoConvidado;

            foreach (var convidado in convite)
            {
                codigoConvidado = ((string)convidado.Codigo).ToInt();

                if (codigoConvidado > 0 && atualizar)
                {
                    RFIConvidado = repositorioConvidado.BuscarConvidado(RFIConvite, codigoConvidado);
                    if (RFIConvidado == null)
                        RFIConvidado = new Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteConvidado();
                    else
                        RFIConvidado.Initialize();
                }
                else
                    RFIConvidado = new Dominio.Entidades.Embarcador.Bidding.RFI.RFIConviteConvidado();

                if (RFIConvidado.Codigo == 0)
                {
                    RFIConvidado.RFIConvite = RFIConvite;
                    RFIConvidado.Convidado = repositorioEmpresa.BuscarPorCodigo(codigoConvidado, auditavel: false);
                }

                if (RFIConvite.ExigirPreenchimentoChecklistConvitePeloTransportador)
                {
                    RFIConvidado.Status = StatusRFIConviteConvidado.Aceito;
                    RFIConvidado.StatusRFI = StatusRFIConvite.Checklist;
                }
                else
                {
                    RFIConvidado.Status = StatusRFIConviteConvidado.Aguardando;
                    RFIConvidado.StatusRFI = StatusRFIConvite.Aguardando;
                }

                if (RFIConvidado.Codigo == 0)
                    repositorioConvidado.Inserir(RFIConvidado);
                else
                {
                    RFIConvite.SetExternalChanges(RFIConvidado.GetCurrentChanges());
                    repositorioConvidado.Atualizar(RFIConvidado);
                }

                codigosConvidados.Add(RFIConvidado.Codigo);
            }
            repositorioConvidado.DeletarPorConviteComExcecao(RFIConvite, codigosConvidados);
        }

        private void SalvarQuestionarios(Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklist RFIChecklist, Repositorio.UnitOfWork unitOfWork, bool atualizar = true)
        {
            Repositorio.Embarcador.Bidding.RFI.RFIChecklistQuestionario repositorioRFIChecklistQuestionario = new Repositorio.Embarcador.Bidding.RFI.RFIChecklistQuestionario(unitOfWork);

            dynamic checklist = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Checklist"));

            foreach (var questionario in checklist)
            {
                int? codigo = ((string)questionario.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionario RFIChecklistQuestionario = null;

                if (codigo.HasValue && atualizar)
                {
                    RFIChecklistQuestionario = repositorioRFIChecklistQuestionario.BuscarPorCodigo(codigo.Value, false);
                    RFIChecklistQuestionario.Initialize();
                }
                else
                    RFIChecklistQuestionario = new Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionario();

                RFIChecklistQuestionario.Checklist = RFIChecklist;
                RFIChecklistQuestionario.Descricao = ((string)questionario.Pergunta).ToString();
                RFIChecklistQuestionario.Requisito = ((string)questionario.Requisito).ToEnum<TipoRequisitoRFIChecklist>();
                RFIChecklistQuestionario.Tipo = ((string)questionario.Tipo).ToEnum<TipoOpcaoCheckListRFI>();

                if (RFIChecklistQuestionario.Codigo == 0)
                    repositorioRFIChecklistQuestionario.Inserir(RFIChecklistQuestionario);
                else
                {
                    repositorioRFIChecklistQuestionario.Atualizar(RFIChecklistQuestionario, Auditado);
                    RFIChecklist.SetExternalChanges(RFIChecklistQuestionario.GetCurrentChanges());
                }

                if (questionario.Alternativas?.Count > 0)
                    SalvarAlternativas(questionario.Alternativas, RFIChecklistQuestionario, unitOfWork, atualizar);
            }
        }

        private void SalvarAlternativas(dynamic alternativas, Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionario questionario, Repositorio.UnitOfWork unitOfWork, bool atualizar = true)
        {
            Repositorio.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAlternativa repositorioRFIChecklistQuestionarioAlternativa = new Repositorio.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAlternativa(unitOfWork);

            List<int> codigosAlternativas = new List<int>();

            foreach (dynamic alternativa in alternativas)
            {
                int? codigo = ((string)alternativa.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAlternativa RFIChecklistQuestionarioAlternativa = null;

                if (codigo.HasValue && atualizar)
                {
                    RFIChecklistQuestionarioAlternativa = repositorioRFIChecklistQuestionarioAlternativa.BuscarPorCodigo(codigo.Value, false);
                    RFIChecklistQuestionarioAlternativa.Initialize();
                }
                else
                    RFIChecklistQuestionarioAlternativa = new Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionarioAlternativa();

                RFIChecklistQuestionarioAlternativa.Descricao = (string)alternativa.Descricao;
                RFIChecklistQuestionarioAlternativa.Ordem = (int)alternativa.Ordem;
                RFIChecklistQuestionarioAlternativa.RFIChecklistQuestionario = questionario;

                if (RFIChecklistQuestionarioAlternativa.Codigo == 0)
                {
                    repositorioRFIChecklistQuestionarioAlternativa.Inserir(RFIChecklistQuestionarioAlternativa);
                    codigosAlternativas.Add(RFIChecklistQuestionarioAlternativa.Codigo);
                }
                else
                {
                    repositorioRFIChecklistQuestionarioAlternativa.Atualizar(RFIChecklistQuestionarioAlternativa);
                    codigosAlternativas.Add(RFIChecklistQuestionarioAlternativa.Codigo);
                    questionario.SetExternalChanges(RFIChecklistQuestionarioAlternativa.GetCurrentChanges());
                }
            }

            repositorioRFIChecklistQuestionarioAlternativa.DeletarPorQuestionarioComExcecao(questionario, codigosAlternativas);
        }

        #endregion
    }
}
