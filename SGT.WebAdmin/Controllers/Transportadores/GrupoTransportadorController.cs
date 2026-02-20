using Dominio.Entidades.Embarcador.Transportadores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize]
    public class GrupoTransportadorController : BaseController
    {
        #region Construtores

        public GrupoTransportadorController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Transportadores.GrupoTransportador repGrupoTransportador = new Repositorio.Embarcador.Transportadores.GrupoTransportador(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaGrupoTransportador filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Cód. Integração", "CodigoIntegracao", 40, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                List<Dominio.Entidades.Embarcador.Transportadores.GrupoTransportador> gruposTransportadores = repGrupoTransportador.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repGrupoTransportador.ContarConsulta(filtrosPesquisa));

                var lista = (from obj in gruposTransportadores
                             select new
                             {
                                 obj.Codigo,
                                 obj.Descricao,
                                 obj.DescricaoAtivo,
                                 obj.CodigoIntegracao
                             }).ToList();

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

        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Transportadores.GrupoTransportador repGrupoTransportador = new Repositorio.Embarcador.Transportadores.GrupoTransportador(unitOfWork);
                Repositorio.Embarcador.Transportadores.GrupoTransportadorEmpresa repGrupoTransportadorEmpresa = new Repositorio.Embarcador.Transportadores.GrupoTransportadorEmpresa(unitOfWork);
                Repositorio.Embarcador.Transportadores.GrupoTransportadorIntegracao repGrupoTransportadorIntegracao = new Repositorio.Embarcador.Transportadores.GrupoTransportadorIntegracao(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                GrupoTransportador grupoTransportador = repGrupoTransportador.BuscarPorCodigo(codigo);

                if (grupoTransportador == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                bool sistemaEstrangeiro = ConfiguracaoEmbarcador.Pais != TipoPais.Brasil;

                List<GrupoTransportadorEmpresa> grupoTransportadorEmpresa = repGrupoTransportadorEmpresa.BuscarPorGrupoTransportador(grupoTransportador.Codigo);
                List<GrupoTransportadorIntegracao> grupoTransportadorIntegracao = await repGrupoTransportadorIntegracao.BuscarIntegracoesPorGrupoTransportadorAsync(grupoTransportador.Codigo);

                var retorno = new
                {
                    grupoTransportador.Codigo,
                    grupoTransportador.Descricao,
                    grupoTransportador.Ativo,
                    grupoTransportador.Observacao,
                    grupoTransportador.CodigoIntegracao,
                    grupoTransportador.ParquearDoumentosAutomaticamente,
                    Empresas = (from obj in grupoTransportadorEmpresa
                                select new
                                {
                                    obj.Empresa.Codigo,
                                    obj.Empresa.Descricao,
                                    CNPJ = sistemaEstrangeiro ? obj.Empresa.CNPJ_Identificacao_Exterior : obj.Empresa.CNPJ_Formatado,
                                }).ToList(),
                    Integracoes = (from obj in grupoTransportadorIntegracao
                                   select new
                                   {
                                       obj.Tipo,
                                       Descricao = obj.Tipo.ObterDescricao(),
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
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Transportadores.GrupoTransportador repGrupoTransportador = new Repositorio.Embarcador.Transportadores.GrupoTransportador(unitOfWork, cancellationToken);

                GrupoTransportador grupoTransportador = new GrupoTransportador();

                PreencherGrupoTransportador(grupoTransportador, unitOfWork);

                await unitOfWork.StartAsync();

                await repGrupoTransportador.InserirAsync(grupoTransportador, Auditado);

                dynamic integracoesGrid = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Integracoes"));

                await new Servicos.Embarcador.Transportadores.GrupoTransportador(unitOfWork, cancellationToken).SalvarIntegracoesGrupoTransportador(grupoTransportador, integracoesGrid);

                SalvarGrupoTransportadorEmpresa(grupoTransportador, unitOfWork);

                await unitOfWork.CommitChangesAsync();
                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Transportadores.GrupoTransportador repGrupoTransportador = new Repositorio.Embarcador.Transportadores.GrupoTransportador(unitOfWork, cancellationToken);

                int codigo = Request.GetIntParam("Codigo");

                GrupoTransportador grupoTransportador = await repGrupoTransportador.BuscarPorCodigoAsync(codigo, true);

                if (grupoTransportador == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherGrupoTransportador(grupoTransportador, unitOfWork);

                SalvarGrupoTransportadorEmpresa(grupoTransportador, unitOfWork);

                dynamic integracoesGrid = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Integracoes"));

                await new Servicos.Embarcador.Transportadores.GrupoTransportador(unitOfWork, cancellationToken).SalvarIntegracoesGrupoTransportador(grupoTransportador, integracoesGrid);

                await repGrupoTransportador.AtualizarAsync(grupoTransportador, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }

            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Transportadores.GrupoTransportador repGrupoTransportador = new Repositorio.Embarcador.Transportadores.GrupoTransportador(unitOfWork);
                Repositorio.Embarcador.Transportadores.GrupoTransportadorEmpresa repGrupoTransportadorEmpresa = new Repositorio.Embarcador.Transportadores.GrupoTransportadorEmpresa(unitOfWork);
                int codigo = Request.GetIntParam("Codigo");

                GrupoTransportador grupoTransportador = repGrupoTransportador.BuscarPorCodigo(codigo);
                List<GrupoTransportadorEmpresa> grupoTransportadorEmpresas = repGrupoTransportadorEmpresa.BuscarPorGrupoTransportador(codigo);

                await unitOfWork.StartAsync();

                foreach (GrupoTransportadorEmpresa grupoTransportadorEmpresa in grupoTransportadorEmpresas)
                    await repGrupoTransportadorEmpresa.DeletarAsync(grupoTransportadorEmpresa, Auditado);

                Repositorio.Embarcador.Transportadores.GrupoTransportadorIntegracao repositorioGrupoTransportadorIntegracao = new(unitOfWork);

                new Servicos.Embarcador.Transportadores.GrupoTransportador(unitOfWork).DeletarGruposTransportadoresIntegracoes(repositorioGrupoTransportadorIntegracao, null, await repositorioGrupoTransportadorIntegracao.BuscarIntegracoesPorGrupoTransportadorAsync(grupoTransportador.Codigo));

                if (grupoTransportador == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                await repGrupoTransportador.DeletarAsync(grupoTransportador, Auditado);

                await unitOfWork.CommitChangesAsync();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
                }
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarTiposIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tipoIntegracaos = await repTipoIntegracao.BuscarTodosAtivosAsync();

                var retornoIntegracoes = (
                        from obj in tipoIntegracaos
                        select new
                        {
                            obj.Tipo,
                            Descricao = obj.Tipo.ObterDescricao(),
                        }
                    ).ToList();
                return new JsonpResult(new
                {
                    Integracoes = retornoIntegracoes
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar integracoes.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherGrupoTransportador(GrupoTransportador grupoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            grupoTransportador.Descricao = Request.GetStringParam("Descricao");
            grupoTransportador.Ativo = Request.GetBoolParam("Ativo");
            grupoTransportador.Observacao = Request.GetStringParam("Observacao");
            grupoTransportador.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            grupoTransportador.ParquearDoumentosAutomaticamente = Request.GetBoolParam("ParquearDoumentosAutomaticamente");
        }

        private void SalvarGrupoTransportadorEmpresa(GrupoTransportador grupoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Transportadores.GrupoTransportadorEmpresa repGrupoTransportadorEmpresa = new Repositorio.Embarcador.Transportadores.GrupoTransportadorEmpresa(unitOfWork);

            List<GrupoTransportadorEmpresa> grpTransportadorEmpresas = new List<GrupoTransportadorEmpresa>();
            List<Dominio.Entidades.Empresa> empresas = new List<Dominio.Entidades.Empresa>();

            dynamic dynEmpresas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Empresas"));

            grpTransportadorEmpresas = repGrupoTransportadorEmpresa.BuscarPorGrupoTransportador(grupoTransportador.Codigo);

            List<int> codigos = new List<int>();
            foreach (dynamic dynEmpresa in dynEmpresas)
            {
                if (dynEmpresa.Codigo != null)
                {
                    int codigoEmp = (int)dynEmpresa.Codigo;
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmp);

                    codigos.Add(codigoEmp);
                    empresas.Add(empresa);

                    if (repGrupoTransportadorEmpresa.ExisteEmpresaEmOutroCadastro(codigoEmp, grupoTransportador.Codigo))
                        throw new ControllerException($"A Empresa {empresa.Descricao} já está cadastrada em outro Grupo");
                }
            }

            if (grpTransportadorEmpresas.Count > 0)
            {
                //Remove os registros excluídos, quando houver
                List<GrupoTransportadorEmpresa> empresasDeletar = (from obj in grpTransportadorEmpresas where !codigos.Contains(obj.Empresa.Codigo) select obj).ToList();

                foreach (GrupoTransportadorEmpresa empresaDeletar in empresasDeletar)
                {
                    repGrupoTransportadorEmpresa.Deletar(empresaDeletar, Auditado);
                    grpTransportadorEmpresas.Remove(empresaDeletar);
                }
            }

            //Insere apenas os registros adicionados, quando houver
            List<int> empresasAnteriores = grpTransportadorEmpresas.Select(o => o.Empresa.Codigo).ToList();
            List<Dominio.Entidades.Empresa> empresasAdicionadas = (from obj in empresas where !empresasAnteriores.Contains(obj.Codigo) select obj).ToList();

            foreach (Dominio.Entidades.Empresa empresaAdicionada in empresasAdicionadas)
            {
                GrupoTransportadorEmpresa grupoTransportadorEmpresa = new GrupoTransportadorEmpresa()
                {
                    Empresa = empresaAdicionada,
                    GrupoTransportador = grupoTransportador,
                };

                repGrupoTransportadorEmpresa.Inserir(grupoTransportadorEmpresa, Auditado);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaGrupoTransportador ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaGrupoTransportador()
            {
                Descricao = Request.GetStringParam("Descricao"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("Situacao"),
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricatoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }
        #endregion
    }
}
