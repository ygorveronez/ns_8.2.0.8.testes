using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Simulacoes
{
    [CustomAuthorize("Simulacoes/GrupoBonificacao")]
    public class GrupoBonificacaoController : BaseController
    {
		#region Construtores

		public GrupoBonificacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Simulacoes.FiltroPesquisaGrupoBonificacao filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 10, Models.Grid.Align.center, true);

                if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.Simulacoes.GrupoBonificacao repositorioGrupoBonificacao = new Repositorio.Embarcador.Simulacoes.GrupoBonificacao(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao> GrupoBonificacaos = repositorioGrupoBonificacao.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repositorioGrupoBonificacao.ContarConsulta(filtrosPesquisa));

                var lista = (from p in GrupoBonificacaos
                             select new
                             {
                                 p.Codigo,
                                 p.CodigoIntegracao,
                                 p.Descricao,
                                 p.DescricaoSituacao
                             }).ToList();

                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Simulacoes.GrupoBonificacao repositorioGrupoBonificacao = new Repositorio.Embarcador.Simulacoes.GrupoBonificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao grupoBonificacao = new Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao();

                PreencherGrupoBonificacao(grupoBonificacao);

                repositorioGrupoBonificacao.Inserir(grupoBonificacao, Auditado);

                //Lista de Set
                SalvarVeiculos(grupoBonificacao, unitOfWork);
                SalvarMetas(grupoBonificacao, unitOfWork);
                SalvarVigencias(grupoBonificacao, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Simulacoes.GrupoBonificacao repositorioGrupoBonificacao = new Repositorio.Embarcador.Simulacoes.GrupoBonificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao grupoBonificacao = repositorioGrupoBonificacao.BuscarPorCodigo(codigo, true);

                if (grupoBonificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherGrupoBonificacao(grupoBonificacao);

                //Lista de Set
                SalvarVeiculos(grupoBonificacao, unitOfWork);
                SalvarMetas(grupoBonificacao, unitOfWork);
                SalvarVigencias(grupoBonificacao, unitOfWork);

                repositorioGrupoBonificacao.Atualizar(grupoBonificacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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

                Repositorio.Embarcador.Simulacoes.GrupoBonificacao repositorioGrupoBonificacao = new Repositorio.Embarcador.Simulacoes.GrupoBonificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao grupoBonificacao = repositorioGrupoBonificacao.BuscarPorCodigo(codigo, false);

                Repositorio.Embarcador.Simulacoes.GrupoBonificacaoMeta repositorioGrupoBonificacaoMeta = new Repositorio.Embarcador.Simulacoes.GrupoBonificacaoMeta(unitOfWork);
                Repositorio.Embarcador.Simulacoes.GrupoBonificacaoVigencia repositorioGrupoBonificacaoVigencia = new Repositorio.Embarcador.Simulacoes.GrupoBonificacaoVigencia(unitOfWork);

                if (grupoBonificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoMeta> grupoBonificacaoMeta = repositorioGrupoBonificacaoMeta.BuscarPorGrupoBonificacao(grupoBonificacao.Codigo);
                List<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoVigencia> grupoBonificacaoVigencia = repositorioGrupoBonificacaoVigencia.BuscarPorGrupoBonificacao(grupoBonificacao.Codigo);

                var dynGrupoBonificacao = new
                {
                    grupoBonificacao.Codigo,
                    grupoBonificacao.Descricao,
                    grupoBonificacao.CodigoIntegracao,
                    grupoBonificacao.Observacao,
                    grupoBonificacao.Situacao,
                    Veiculos = (from obj in grupoBonificacao.Veiculos
                                select new
                                {
                                    obj.Codigo,
                                    obj.Placa,
                                    obj.NumeroFrota,
                                    Empresa = obj.Empresa.Descricao
                                }).ToList(),
                    Metas = (from obj in grupoBonificacaoMeta
                             select new
                             {
                                 obj.Codigo,
                                 CodigoRegiao = obj.Regiao?.Codigo,
                                 Regiao = obj.Regiao?.Descricao,
                                 obj.QuantidadeCargasIdaPrevista,
                                 obj.QuantidadeCargasIdaRealizada,
                                 obj.QuantidadeCargasRetornoPrevista,
                                 obj.QuantidadeCargasRetornoRealizada
                             }).ToList(),
                    Vigencia = (from obj in grupoBonificacaoVigencia
                                select new
                                {
                                    obj.Codigo,
                                    DataInicial = obj.DataInicial.ToDateString(),
                                    DataFinal = obj.DataFinal.ToDateString(),
                                    obj.Situacao
                                }).ToList()
                };

                return new JsonpResult(dynGrupoBonificacao);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Simulacoes.GrupoBonificacao repositorioGrupoBonificacao = new Repositorio.Embarcador.Simulacoes.GrupoBonificacao(unitOfWork);
                Repositorio.Embarcador.Simulacoes.GrupoBonificacaoMeta repositorioGrupoBonificacaoMeta = new Repositorio.Embarcador.Simulacoes.GrupoBonificacaoMeta(unitOfWork);
                Repositorio.Embarcador.Simulacoes.GrupoBonificacaoVigencia repositorioGrupoBonificacaoVigencia = new Repositorio.Embarcador.Simulacoes.GrupoBonificacaoVigencia(unitOfWork);

                Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao grupoBonificacao = repositorioGrupoBonificacao.BuscarPorCodigo(codigo, true);

                if (grupoBonificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoMeta> grupoBonificacaoMetas = repositorioGrupoBonificacaoMeta.BuscarPorGrupoBonificacao(grupoBonificacao.Codigo);

                foreach (Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoMeta grupoBonificacaoMeta in grupoBonificacaoMetas)
                    repositorioGrupoBonificacaoMeta.Deletar(grupoBonificacaoMeta);

                List<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoVigencia> grupoBonificacaoVigencias = repositorioGrupoBonificacaoVigencia.BuscarPorGrupoBonificacao(grupoBonificacao.Codigo);

                foreach (Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoVigencia grupoBonificacaoVigencia in grupoBonificacaoVigencias)
                    repositorioGrupoBonificacaoVigencia.Deletar(grupoBonificacaoVigencia);

                repositorioGrupoBonificacao.Deletar(grupoBonificacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherGrupoBonificacao(Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao grupoBonificacao)
        {
            grupoBonificacao.Descricao = Request.GetStringParam("Descricao");
            grupoBonificacao.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            grupoBonificacao.Observacao = Request.GetStringParam("Observacao");
            grupoBonificacao.Situacao = Request.GetBoolParam("Situacao");
        }

        private Dominio.ObjetosDeValor.Embarcador.Simulacoes.FiltroPesquisaGrupoBonificacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Simulacoes.FiltroPesquisaGrupoBonificacao()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Situacao = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
            };
        }

        private void SalvarVeiculos(Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao grupoBonificacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);

            if (grupoBonificacao.Veiculos == null)
                grupoBonificacao.Veiculos = new List<Dominio.Entidades.Veiculo>();
            else
                grupoBonificacao.Veiculos.Clear();

            dynamic dynVeiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Veiculos"));

            foreach (var dynVeiculo in dynVeiculos)
                grupoBonificacao.Veiculos.Add(repositorioVeiculo.BuscarPorCodigo((int)dynVeiculo.Codigo));
        }

        private void SalvarMetas(Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao grupoBonificacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Simulacoes.GrupoBonificacaoMeta repositorioGrupoBonificacaoMeta = new Repositorio.Embarcador.Simulacoes.GrupoBonificacaoMeta(unitOfWork);
            Repositorio.Embarcador.Localidades.Regiao repositorioRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);

            dynamic metas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Metas"));

            List<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoMeta> listaMetas = repositorioGrupoBonificacaoMeta.BuscarPorGrupoBonificacao(grupoBonificacao.Codigo);

            if (listaMetas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic meta in metas)
                    if (meta.Codigo > 0)
                        codigos.Add(((string)meta.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoMeta> listaMetasRemover = listaMetas.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoMeta grupoBonificacaoMeta in listaMetasRemover)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoBonificacao, $"Excluiu a meta da região {grupoBonificacaoMeta.Regiao.Descricao} no grupo de bonificação.", unitOfWork);
                    repositorioGrupoBonificacaoMeta.Deletar(grupoBonificacaoMeta);
                }
            }

            foreach (dynamic dynMeta in metas)
            {
                int codigo = ((string)dynMeta.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoMeta grupoBonificacaoMeta = codigo > 0 ? listaMetas.FirstOrDefault(o => o.Codigo == codigo) : null;

                if (grupoBonificacaoMeta == null)
                {
                    grupoBonificacaoMeta = new Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoMeta();
                    grupoBonificacaoMeta.GrupoBonificacao = grupoBonificacao;
                }
                else
                    grupoBonificacaoMeta.Initialize();

                grupoBonificacaoMeta.Regiao = repositorioRegiao.BuscarPorCodigo(((string)dynMeta.CodigoRegiao).ToInt());
                grupoBonificacaoMeta.QuantidadeCargasIdaPrevista = ((string)dynMeta.QuantidadeCargasIdaPrevista).ToInt();
                grupoBonificacaoMeta.QuantidadeCargasIdaRealizada = ((string)dynMeta.QuantidadeCargasIdaRealizada).ToInt();
                grupoBonificacaoMeta.QuantidadeCargasRetornoPrevista = ((string)dynMeta.QuantidadeCargasRetornoPrevista).ToInt();
                grupoBonificacaoMeta.QuantidadeCargasRetornoRealizada = ((string)dynMeta.QuantidadeCargasRetornoRealizada).ToInt();

                if (grupoBonificacaoMeta.Codigo > 0)
                {
                    repositorioGrupoBonificacaoMeta.Atualizar(grupoBonificacaoMeta);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = grupoBonificacaoMeta.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoBonificacao, alteracoes, $"Alterou a meta da região {grupoBonificacaoMeta.Regiao.Descricao} do grupo de bonificação.", unitOfWork);
                }
                else
                {
                    repositorioGrupoBonificacaoMeta.Inserir(grupoBonificacaoMeta);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoBonificacao, $"Adicionou a meta da região {grupoBonificacaoMeta.Regiao.Descricao} no grupo de bonificação.", unitOfWork);
                }
            }
        }

        private void SalvarVigencias(Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacao grupoBonificacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Simulacoes.GrupoBonificacaoVigencia repositorioGrupoBonificacaoVigencia = new Repositorio.Embarcador.Simulacoes.GrupoBonificacaoVigencia(unitOfWork);

            dynamic vigencias = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Vigencia"));

            List<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoVigencia> listaVigencias = repositorioGrupoBonificacaoVigencia.BuscarPorGrupoBonificacao(grupoBonificacao.Codigo);

            if (listaVigencias.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic vigencia in vigencias)
                    if (vigencia.Codigo > 0)
                        codigos.Add(((string)vigencia.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoVigencia> listaMetasRemover = listaVigencias.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoVigencia grupoBonificacaoVigencia in listaMetasRemover)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoBonificacao, $"Excluiu a vigência {grupoBonificacaoVigencia.Descricao} no grupo de bonificação.", unitOfWork);
                    repositorioGrupoBonificacaoVigencia.Deletar(grupoBonificacaoVigencia);
                }
            }

            foreach (dynamic dynVigencia in vigencias)
            {
                int codigo = ((string)dynVigencia.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoVigencia grupoBonificacaoVigencia = codigo > 0 ? listaVigencias.FirstOrDefault(o => o.Codigo == codigo) : null;

                if (grupoBonificacaoVigencia == null)
                {
                    grupoBonificacaoVigencia = new Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoVigencia();
                    grupoBonificacaoVigencia.GrupoBonificacao = grupoBonificacao;
                }
                else
                    grupoBonificacaoVigencia.Initialize();

                grupoBonificacaoVigencia.DataInicial = ((string)dynVigencia.DataInicial).ToDateTime();
                grupoBonificacaoVigencia.DataFinal = ((string)dynVigencia.DataFinal).ToDateTime();
                grupoBonificacaoVigencia.Situacao = ((string)dynVigencia.Situacao).ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusLicenca>();

                if (grupoBonificacaoVigencia.Codigo > 0)
                {
                    repositorioGrupoBonificacaoVigencia.Atualizar(grupoBonificacaoVigencia);
                    List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = grupoBonificacaoVigencia.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoBonificacao, alteracoes, $"Alterou a vigência {grupoBonificacaoVigencia.Descricao} do grupo de bonificação.", unitOfWork);
                }
                else
                {
                    repositorioGrupoBonificacaoVigencia.Inserir(grupoBonificacaoVigencia);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, grupoBonificacao, $"Adicionou a vigência {grupoBonificacaoVigencia.Descricao} no grupo de bonificação.", unitOfWork);
                }
            }
        }

        #endregion
    }
}
