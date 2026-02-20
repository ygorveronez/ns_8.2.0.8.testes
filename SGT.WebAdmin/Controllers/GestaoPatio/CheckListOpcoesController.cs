using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/CheckListOpcoes")]
    public class CheckListOpcoesController : BaseController
    {
		#region Construtores

		public CheckListOpcoesController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
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
                // Instancia repositorios
                Repositorio.Embarcador.GestaoPatio.CheckListOpcoes repCheckListOpcoes = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(unitOfWork);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes opcao = repCheckListOpcoes.BuscarPorCodigo(codigo);

                // Valida
                if (opcao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(TipoFluxoGestaoPatio.Origem, opcao.Filial?.Codigo ?? 0);

                List<Dominio.Entidades.Embarcador.GestaoPatio.ChecklistRelacaoPergunta> listaRelacao = sequenciaGestaoPatio != null && sequenciaGestaoPatio.TipoChecklistImpressao != null ? repCheckListOpcoes.BuscarRelacaoPergunta(sequenciaGestaoPatio.TipoChecklistImpressao.Codigo) : new List<Dominio.Entidades.Embarcador.GestaoPatio.ChecklistRelacaoPergunta>();

                // Formata retorno
                var retorno = new
                {
                    opcao.Codigo,
                    opcao.Descricao,
                    opcao.Categoria,
                    opcao.Assunto,
                    opcao.Aplicacao,
                    opcao.CodigoIntegracao,
                    opcao.Obrigatorio,
                    opcao.TipoData,
                    opcao.TipoHora,
                    opcao.TipoDecimal,
                    opcao.TagIntegracao,
                    opcao.PermiteNaoAplica,
                    opcao.ExibirSomenteParaFretesOndeRemetenteForTomador,
                    TipoOpcao = opcao.Tipo,
                    opcao.EtapaCheckList,
                    CheckListTipo = opcao.CheckListTipo != null ? new { opcao.CheckListTipo.Codigo, opcao.CheckListTipo.Descricao } : null,
                    //opcao.TipoCheckListGuarita,                    
                    opcao.Ordem,
                    Filial = opcao.Filial != null ? new { opcao.Filial.Codigo, opcao.Filial.Descricao } : null,
                    opcao.RespostaImpeditiva,
                    RelacaoPerguntas = (from o in listaRelacao
                                        select new
                                        {
                                            o.Codigo,
                                            o.Descricao
                                        }).ToList(),
                    RelacaoPergunta = opcao.RelacaoPergunta?.Codigo ?? 0,
                    RelacaoCampo = opcao.RelacaoCampo?.Codigo ?? 0,
                    Opcoes = (from o in opcao.Alternativas
                              select new
                              {
                                  o.Codigo,
                                  o.Ordem,
                                  o.Valor,
                                  o.CodigoIntegracao,
                                  o.Descricao,
                                  o.OpcaoImpeditiva
                              }).ToList()
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.GestaoPatio.CheckListOpcoes repCheckListOpcoes = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes opcao = new Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes();

                // Preenche entidade com dados
                PreencheEntidade(ref opcao, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(opcao, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repCheckListOpcoes.Inserir(opcao, Auditado);
                SalvarOpcoes(opcao, unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.GestaoPatio.CheckListOpcoes repCheckListOpcoes = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes opcao = repCheckListOpcoes.BuscarPorCodigo(codigo, true);

                // Valida
                if (opcao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref opcao, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(opcao, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                SalvarOpcoes(opcao, unitOfWork);
                repCheckListOpcoes.Atualizar(opcao, Auditado);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.GestaoPatio.CheckListOpcoes repCheckListOpcoes = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes opcao = repCheckListOpcoes.BuscarPorCodigo(codigo);

                // Valida
                if (opcao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repCheckListOpcoes.Deletar(opcao, Auditado);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarRelacaoPergunta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoPatio.CheckListOpcoes repCheckListOpcoes = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(unitOfWork);

                int tipoChecklistImpressao = Request.GetIntParam("TipoChecklistImpressao");

                List<Dominio.Entidades.Embarcador.GestaoPatio.ChecklistRelacaoPergunta> listaRelacao = repCheckListOpcoes.BuscarRelacaoPergunta(tipoChecklistImpressao);

                if (listaRelacao.Count == 0)
                    return new JsonpResult(false, true, "Nenhuma relação de pergunta encontrada.");

                var retorno = new
                {
                    RelacaoPergunta = (from o in listaRelacao
                                       select new
                                       {
                                           o.Codigo,
                                           o.Descricao
                                       }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar relação de perguntas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Categoria", "Categoria", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Aplicação", "Aplicacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo", "CheckListTipo", 10, Models.Grid.Align.left, true);
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.left, true);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.GestaoPatio.CheckListOpcoes repCheckListOpcoes = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(unitOfWork);

            // Dados do filtro
            string descricao = Request.Params("Descricao") ?? string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaOpcaoCheckList? categoria = null;
            if (Enum.TryParse(Request.Params("Categoria"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaOpcaoCheckList categoriaAux))
                categoria = categoriaAux;

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoOpcaoCheckList? aplicacao = null;
            if (Enum.TryParse(Request.Params("Categoria"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoOpcaoCheckList aplicacaoAux))
                aplicacao = aplicacaoAux;

            //Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCheckListGuarita? tipo = null;
            //if (Enum.TryParse(Request.Params("TipoCheckListGuarita"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCheckListGuarita tipoAux))
            //    tipo = tipoAux;

            int.TryParse(Request.Params("CheckListTipo"), out int codigoCheckListTipo);
            int.TryParse(Request.Params("Filial"), out int codigoFilial);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> listaGrid = repCheckListOpcoes.Consultar(descricao, categoria, aplicacao, codigoCheckListTipo, codigoEmpresa, codigoFilial, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repCheckListOpcoes.ContarConsulta(descricao, categoria, aplicacao, codigoCheckListTipo, codigoEmpresa, codigoFilial);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Descricao,
                            Categoria = obj.Categoria.ObterDescricao(),
                            Aplicacao = obj.Aplicacao.ObterDescricao(),
                            //TipoCheckListGuarita = obj.DescricaoTipoCheckListGuarita
                            CheckListTipo = obj.CheckListTipo?.Descricao ?? string.Empty,
                            Filial = obj.Filial?.Descricao ?? string.Empty
                        };

            return lista.ToList();
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes opcao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListTipo repCheckListTipo = new Repositorio.Embarcador.GestaoPatio.CheckListTipo(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.CheckListOpcoes repCheckListOpcoes = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);

            int codigoCheckListTipo = Request.GetIntParam("CheckListTipo");
            int codigoFilial = Request.GetIntParam("Filial");
            int codigoRelacaoCampo = Request.GetIntParam("RelacaoCampo");
            int codigoRelacaoPergunta = Request.GetIntParam("RelacaoPergunta");
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(TipoFluxoGestaoPatio.Origem, opcao.Filial?.Codigo ?? 0);

            bool existePerguntaComRelacaoSelecionada = false;
            if (sequenciaGestaoPatio?.TipoChecklistImpressao != null && codigoFilial > 0 && codigoRelacaoPergunta > 0)
                existePerguntaComRelacaoSelecionada = repCheckListOpcoes.ValidarRelacaoPergunta(codigoFilial, codigoRelacaoPergunta, opcao.Codigo);

            if (existePerguntaComRelacaoSelecionada)
                throw new ControllerException("Já existe uma pergunta com essa relação de pergunta cadastrada para essa filial.");

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            // Vincula dados
            opcao.Descricao = Request.GetStringParam("Descricao");
            opcao.Categoria = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaOpcaoCheckList>("Categoria");
            opcao.Aplicacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.AplicacaoOpcaoCheckList>("Aplicacao");
            opcao.CodigoIntegracao = Request.GetIntParam("CodigoIntegracao");
            opcao.Tipo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList>("TipoOpcao");
            opcao.RespostaImpeditiva = opcao.Tipo == TipoOpcaoCheckList.SimNao ? Request.GetNullableEnumParam<CheckListResposta>("RespostaImpeditiva") : null;
            opcao.Obrigatorio = Request.GetBoolParam("Obrigatorio");
            opcao.TipoData = Request.GetBoolParam("TipoData");
            opcao.TipoHora = Request.GetBoolParam("TipoHora");
            opcao.TipoDecimal = Request.GetBoolParam("TipoDecimal");
            opcao.PermiteNaoAplica = Request.GetBoolParam("PermiteNaoAplica");
            opcao.ExibirSomenteParaFretesOndeRemetenteForTomador = Request.GetBoolParam("ExibirSomenteParaFretesOndeRemetenteForTomador");
            opcao.Ordem = Request.GetIntParam("Ordem");
            opcao.Assunto = Request.GetStringParam("Assunto");
            opcao.CheckListTipo = codigoCheckListTipo > 0 ? repCheckListTipo.BuscarPorCodigo(codigoCheckListTipo) : null;
            opcao.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
            opcao.Filial = codigoFilial > 0 ? repFilial.BuscarPorCodigo(codigoFilial) : null;
            opcao.RelacaoPergunta = codigoRelacaoPergunta > 0 && sequenciaGestaoPatio?.TipoChecklistImpressao != null ? repCheckListOpcoes.BuscarRelacaoPerguntaPorCodigo(codigoRelacaoPergunta) : null;
            opcao.EtapaCheckList = Request.GetEnumParam<EtapaCheckList>("EtapaCheckList");
            opcao.RelacaoCampo = codigoRelacaoCampo > 0 ? repCheckListOpcoes.BuscarRelacaoCampoPorCodigo(codigoRelacaoCampo) : null;
            opcao.TagIntegracao = Request.GetStringParam("TagIntegracao");
        }

        private void SalvarOpcoes(Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes checklist, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.CheckListAlternativa repCheckListAlternativa = new Repositorio.Embarcador.GestaoPatio.CheckListAlternativa(unitOfWork);

            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade>();

            List<dynamic> dynOpcoes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("Opcoes"));
            if (dynOpcoes == null) return;

            List<int> codigosOpcoes = new List<int>();
            foreach (dynamic codigo in dynOpcoes)
            {
                int.TryParse((string)codigo.Codigo, out int intcodigo);
                codigosOpcoes.Add(intcodigo);
            }
            codigosOpcoes = codigosOpcoes.Where(o => o > 0).Distinct().ToList();

            List<int> codigosOpcoesExcluir = repCheckListAlternativa.BuscarOpcoesNaoPesentesNaLista(checklist.Codigo, codigosOpcoes);

            foreach (dynamic dynOpcao in dynOpcoes)
            {
                int.TryParse((string)dynOpcao.Codigo, out int codigo);
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa opcaoCheckList = repCheckListAlternativa.BuscarPorCheckListEOpcao(checklist.Codigo, codigo);

                if (opcaoCheckList == null)
                    opcaoCheckList = new Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa();
                else
                    opcaoCheckList.Initialize();

                int.TryParse((string)dynOpcao.Valor, out int valor);
                opcaoCheckList.CheckListOpcoes = checklist;
                opcaoCheckList.Descricao = (string)dynOpcao.Descricao;
                opcaoCheckList.Valor = valor;
                int.TryParse((string)dynOpcao.CodigoIntegracao, out int codigoIntegracao);
                opcaoCheckList.CodigoIntegracao = ((string)dynOpcao.CodigoIntegracao).ToInt();
                int.TryParse((string)dynOpcao.Ordem, out int ordem);
                opcaoCheckList.Ordem = ordem;
                opcaoCheckList.OpcaoImpeditiva = ((string)dynOpcao.OpcaoImpeditiva).ToBool();

                if (opcaoCheckList.Codigo > 0)
                {
                    alteracoes.AddRange(opcaoCheckList.GetCurrentChanges());

                    repCheckListAlternativa.Atualizar(opcaoCheckList);
                }
                else
                {
                    alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                    {
                        Propriedade = "Opções",
                        De = "",
                        Para = $"{opcaoCheckList.Descricao} - Código Integração: {opcaoCheckList.CodigoIntegracao}"
                    });

                    repCheckListAlternativa.Inserir(opcaoCheckList);
                }
            }

            foreach (int codigo in codigosOpcoesExcluir)
            {
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListAlternativa entidade = repCheckListAlternativa.BuscarPorCheckListEOpcao(checklist.Codigo, codigo);

                alteracoes.Add(new Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade()
                {
                    Propriedade = "Opções",
                    De = $"{entidade.Descricao} - Código Integração: {entidade.CodigoIntegracao}",
                    Para = ""
                });

                if (entidade != null) repCheckListAlternativa.Deletar(entidade);
            }

            checklist.SetExternalChanges(alteracoes);
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes opcao, out string msgErro)
        {
            msgErro = "";

            if (opcao.Descricao.Length == 0)
            {
                msgErro = "Descrição é obrigatório.";
                return false;
            }

            return true;
        }

        private void PropOrdena(ref string propOrdenar)
        {
        }

        #endregion
    }
}
