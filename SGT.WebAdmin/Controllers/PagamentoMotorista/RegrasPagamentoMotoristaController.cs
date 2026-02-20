using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace SGT.WebAdmin.Controllers.PagamentoMotorista
{
    [CustomAuthorize("PagamentosMotoristas/RegrasPagamentoMotorista")]
    public class RegrasPagamentoMotoristaController : BaseController
    {
		#region Construtores

		public RegrasPagamentoMotoristaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region ObjetosJson
        private class ObjetoEntidade
        {
            public dynamic Codigo { get; set; } // dynamic pois o codigo pode ser também um cpf/cnpj
            public string Descricao { get; set; }
        }
        private class ObjetoAprovadores
        {
            public int Codigo { get; set; }
            public string Nome { get; set; }
        }
        private class RegrasPorTipo
        {
            public dynamic Codigo { get; set; }
            public int Ordem { get; set; }
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia Condicao { get; set; }
            public Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia Juncao { get; set; }
            public ObjetoEntidade Entidade { get; set; }
            public dynamic Valor { get; set; }
        }
        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vigência", "Vigencia", 15, Models.Grid.Align.center, true);

                // Converte parametros
                int codigoAprovador = 0;
                int.TryParse(Request.Params("Aprovador"), out codigoAprovador);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia etapaAutorizacaoOcorrencia;
                Enum.TryParse(Request.Params("EtapaAutorizacao"), out etapaAutorizacaoOcorrencia);

                DateTime dataInicioAux, dataFimAux;
                DateTime? dataInicio = null, dataFim = null;

                if (DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicioAux))
                    dataInicio = dataInicioAux;

                if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFimAux))
                    dataFim = dataFimAux;

                string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : "";

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Instancia repositorios
                Repositorio.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista repRegrasPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.Usuario aprovador = repUsuario.BuscarPorCodigo(codigoAprovador);

                List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista> listaRegras = repRegrasPagamentoMotorista.ConsultarRegras(dataInicio, dataFim, aprovador, descricao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repRegrasPagamentoMotorista.ContarConsultaRegras(dataInicio, dataFim, aprovador, descricao));

                var lista = (from obj in listaRegras
                             select new
                             {
                                 obj.Codigo,
                                 Descricao = !string.IsNullOrWhiteSpace(obj.Descricao) ? obj.Descricao : string.Empty,
                                 Vigencia = obj.Vigencia.HasValue ? obj.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
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

                // Instancia Repositorios/Entidade
                Repositorio.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista repRegrasPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaEmpresa repRegraPagamentoMotoristaEmpresa = new Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaEmpresa(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaTipo repRegraPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaTipo(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaValor repRegraPagamentoMotoristaValor = new Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaValor(unitOfWork);

                // Nova entidade
                Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista regrasPagamentoMotorista = new Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista();
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaEmpresa> regrasPagamentoMotoristaEmpresa = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaEmpresa>();
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaTipo> regrasPagamentoMotoristaTipo = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaTipo>();
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaValor> regrasPagamentoMotoristaValor = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaValor>();

                // Preenche a entidade
                PreencherEntidade(ref regrasPagamentoMotorista, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasPagamentoMotorista, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }


                #region Regras
                List<string> errosRegras = new List<string>();

                #region Empresa
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasPagamentoMotorista.RegraPorEmpresa)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Empresa", "RegrasEmpresa", false, ref regrasPagamentoMotoristaEmpresa, ref regrasPagamentoMotorista, ((codigo) => {
                            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repEmpresa.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Empresa");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Empresa", "Empresa", regrasPagamentoMotoristaEmpresa, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region TipoPagamento
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasPagamentoMotorista.RegraPorTipo)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("PagamentoMotoristaTipo", "RegrasTipo", false, ref regrasPagamentoMotoristaTipo, ref regrasPagamentoMotorista, ((codigo) => {
                            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repPagamentoMotoristaTipo.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Tipo Pagamento");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Tipo Pagamento", "PagamentoMotoristaTipo", regrasPagamentoMotoristaTipo, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Valor
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasPagamentoMotorista.RegraPorValor)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Valor", "RegrasValor", true, ref regrasPagamentoMotoristaValor, ref regrasPagamentoMotorista);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Valor do Pagamento");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Valor do Pagamento", "Valor", regrasPagamentoMotoristaValor, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion
                

                #endregion


                // Insere Entidade
                repRegrasPagamentoMotorista.Inserir(regrasPagamentoMotorista);

                // Insere regras
                for (var i = 0; i < regrasPagamentoMotoristaEmpresa.Count(); i++) repRegraPagamentoMotoristaEmpresa.Inserir(regrasPagamentoMotoristaEmpresa[i]);
                for (var i = 0; i < regrasPagamentoMotoristaTipo.Count(); i++) repRegraPagamentoMotoristaTipo.Inserir(regrasPagamentoMotoristaTipo[i]);
                for (var i = 0; i < regrasPagamentoMotoristaValor.Count(); i++) repRegraPagamentoMotoristaValor.Inserir(regrasPagamentoMotoristaValor[i]);

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

                // Instancia Repositorios/Entidade

                Repositorio.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista repRegrasPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaEmpresa repRegraPagamentoMotoristaEmpresa = new Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaEmpresa(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaTipo repRegraPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaTipo(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaValor repRegraPagamentoMotoristaValor = new Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaValor(unitOfWork);

                // Nova entidade
                // Codigo da busca 
                int codigoRegra = 0;
                int.TryParse(Request.Params("Codigo"), out codigoRegra);

                // Busca entidade
                Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista regrasPagamentoMotorista = repRegrasPagamentoMotorista.BuscarPorCodigo(codigoRegra);

                if (regrasPagamentoMotorista == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");


                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaEmpresa> regraPagamentoMotoristaEmpresa = repRegraPagamentoMotoristaEmpresa.BuscarPorRegras(codigoRegra);
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaTipo> regraPagamentoMotoristaTipo = repRegraPagamentoMotoristaTipo.BuscarPorRegras(codigoRegra);                
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaValor> regraPagamentoMotoristaValor = repRegraPagamentoMotoristaValor.BuscarPorRegras(codigoRegra);
                #endregion


                #region DeletaRegras
                for (var i = 0; i < regraPagamentoMotoristaEmpresa.Count(); i++) repRegraPagamentoMotoristaEmpresa.Deletar(regraPagamentoMotoristaEmpresa[i]);
                for (var i = 0; i < regraPagamentoMotoristaTipo.Count(); i++) repRegraPagamentoMotoristaTipo.Deletar(regraPagamentoMotoristaTipo[i]);
                for (var i = 0; i < regraPagamentoMotoristaValor.Count(); i++) repRegraPagamentoMotoristaValor.Deletar(regraPagamentoMotoristaValor[i]);
                #endregion

                #region NovasRegras
                regraPagamentoMotoristaEmpresa = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaEmpresa>();
                regraPagamentoMotoristaTipo = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaTipo>();
                regraPagamentoMotoristaValor = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaValor>();
                #endregion


                // Preenche a entidade
                PreencherEntidade(ref regrasPagamentoMotorista, unitOfWork);

                List<string> erros = new List<string>();
                // Validar entidade
                if (!ValidarEntidade(regrasPagamentoMotorista, out erros))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, String.Join("<br>", erros));
                }

                // Atualiza Entidade
                repRegrasPagamentoMotorista.Atualizar(regrasPagamentoMotorista);

                #region Regras
                List<string> errosRegras = new List<string>();

                #region Empresa
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasPagamentoMotorista.RegraPorEmpresa)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Empresa", "RegrasEmpresa", false, ref regraPagamentoMotoristaEmpresa, ref regrasPagamentoMotorista, ((codigo) => {
                            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repEmpresa.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Empresa");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Empresa", "Empresa", regraPagamentoMotoristaEmpresa, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Tipo
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasPagamentoMotorista.RegraPorTipo)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("PagamentoMotoristaTipo", "RegrasTipo", false, ref regraPagamentoMotoristaTipo, ref regrasPagamentoMotorista, ((codigo) => {
                            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unitOfWork);

                            int codigoInt = 0;
                            int.TryParse(codigo.ToString(), out codigoInt);

                            return repPagamentoMotoristaTipo.BuscarPorCodigo(codigoInt);
                        }));
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("TipoPagamento");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Tipo Pagamento", "PagamentoMotoristaTipo", regraPagamentoMotoristaTipo, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion

                #region Valor
                // Preenche as regras apenas se a flag de uso for verdadeira
                if (regrasPagamentoMotorista.RegraPorValor)
                {
                    // Preenche regra
                    try
                    {
                        PreencherEntidadeRegra("Valor", "RegrasValor", true, ref regraPagamentoMotoristaValor, ref regrasPagamentoMotorista);
                    }
                    catch (Exception e)
                    {
                        Servicos.Log.TratarErro(e);
                        errosRegras.Add("Valor do Pagamento");
                    }

                    erros = new List<string>();
                    // Valida regra
                    if (!ValidarEntidadeRegra("Valor do Pagamento", "Valor", regraPagamentoMotoristaValor, out erros))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, String.Join("<br>", erros));
                    }
                }
                #endregion                

                #endregion


                // Insere regras
                for (var i = 0; i < regraPagamentoMotoristaEmpresa.Count(); i++) repRegraPagamentoMotoristaEmpresa.Inserir(regraPagamentoMotoristaEmpresa[i]);
                for (var i = 0; i < regraPagamentoMotoristaTipo.Count(); i++) repRegraPagamentoMotoristaTipo.Inserir(regraPagamentoMotoristaTipo[i]);
                for (var i = 0; i < regraPagamentoMotoristaValor.Count(); i++) repRegraPagamentoMotoristaValor.Inserir(regraPagamentoMotoristaValor[i]);

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
                // Instancia Repositorios/Entidade

                Repositorio.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista repRegrasPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaEmpresa repRegraPagamentoMotoristaEmpresa = new Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaEmpresa(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaTipo repRegraPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaTipo(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaValor repRegraPagamentoMotoristaValor = new Repositorio.Embarcador.PagamentoMotorista.RegraPagamentoMotoristaValor(unitOfWork);                

                // Codigo da busca 
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista regrasPagamentoMotorista = repRegrasPagamentoMotorista.BuscarPorCodigo(codigo);

                if (regrasPagamentoMotorista == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                #region BuscaRegras
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaEmpresa> regraPagamentoMotoristaEmpresa = repRegraPagamentoMotoristaEmpresa.BuscarPorRegras(codigo);
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaTipo> regraPagamentoMotoristaTipo = repRegraPagamentoMotoristaTipo.BuscarPorRegras(codigo);                
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaValor> regraPagamentoMotoristaValor = repRegraPagamentoMotoristaValor.BuscarPorRegras(codigo);
                #endregion


                var dynRegra = new
                {
                    regrasPagamentoMotorista.Codigo,
                    regrasPagamentoMotorista.NumeroAprovadores,
                    Vigencia = regrasPagamentoMotorista.Vigencia.HasValue ? regrasPagamentoMotorista.Vigencia.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Descricao = !string.IsNullOrWhiteSpace(regrasPagamentoMotorista.Descricao) ? regrasPagamentoMotorista.Descricao : string.Empty,
                    Observacao = !string.IsNullOrWhiteSpace(regrasPagamentoMotorista.Observacoes) ? regrasPagamentoMotorista.Observacoes : string.Empty,

                    Aprovadores = (from o in regrasPagamentoMotorista.Aprovadores select new { o.Codigo, o.Nome }).ToList(),

                    RegraPorEmpresa = regrasPagamentoMotorista.RegraPorEmpresa,
                    Empresa = (from obj in regraPagamentoMotoristaEmpresa select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaEmpresa>(obj, "Empresa", "Descricao")).ToList(),

                    RegraPorTipo = regrasPagamentoMotorista.RegraPorTipo,
                    TipoPagamento = (from obj in regraPagamentoMotoristaTipo select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaTipo>(obj, "PagamentoMotoristaTipo", "Descricao")).ToList(),

                    RegraPorValor = regrasPagamentoMotorista.RegraPorValor,
                    Valor = (from obj in regraPagamentoMotoristaValor select RetornaRegraPorTipoDyn<Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotoristaValor>(obj, "Valor", "Valor", true)).ToList()
                };

                return new JsonpResult(dynRegra);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar.");
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
                // Instancia Repositorios/Entidade
                Repositorio.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista repRegrasPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista(unitOfWork);

                // Codigo da busca 
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca entidade
                Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista regrasPagamentoMotorista = repRegrasPagamentoMotorista.BuscarPorCodigo(codigo);

                if (regrasPagamentoMotorista == null)
                    return new JsonpResult(false, "Não foi possível buscar a regra.");

                // Inicia transicao
                unitOfWork.Start();

                regrasPagamentoMotorista.Aprovadores.Clear();
                regrasPagamentoMotorista.RegrasPagamentoMotoristaEmpresa.Clear();
                regrasPagamentoMotorista.RegrasPagamentoMotoristaTipo.Clear();
                regrasPagamentoMotorista.RegrasPagamentoMotoristaValor.Clear();

                repRegrasPagamentoMotorista.Deletar(regrasPagamentoMotorista);

                // Comita alteracoes
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Já existem solicitações vinculadas à regra.");
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion


        #region Métodos Privados
        private void PreencherEntidade(ref Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista regrasPagamentoMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia Repositorios/Entidade
            Repositorio.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);


            // Converte parametros
            string descricao = !string.IsNullOrWhiteSpace(Request.Params("Descricao")) ? Request.Params("Descricao") : string.Empty;
            string observacao = !string.IsNullOrWhiteSpace(Request.Params("Observacao")) ? Request.Params("Observacao") : string.Empty;

            DateTime dataVigenciaAux;
            DateTime? dataVigencia = null;

            if (DateTime.TryParseExact(Request.Params("Vigencia"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVigenciaAux))
                dataVigencia = dataVigenciaAux;            

            int numeroAprovadores = 0;
            int.TryParse(Request.Params("NumeroAprovadores"), out numeroAprovadores);            

            bool usarRegraPorEmpresa;
            bool.TryParse(Request.Params("RegraPorEmpresa"), out usarRegraPorEmpresa);
            bool usarRegraPorTipo;
            bool.TryParse(Request.Params("RegraPorTipo"), out usarRegraPorTipo);
            bool usarRegraPorValor;
            bool.TryParse(Request.Params("RegraPorValor"), out usarRegraPorValor);

            List<int> codigosUsuarios = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Aprovadores")))
            {
                List<ObjetoAprovadores> dynAprovadores = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ObjetoAprovadores>>(Request.Params("Aprovadores"));

                for (var i = 0; i < dynAprovadores.Count(); i++)
                    codigosUsuarios.Add(dynAprovadores[i].Codigo);
            }
            List<Dominio.Entidades.Usuario> listaAprovadores = repUsuario.BuscarUsuariosPorCodigos(codigosUsuarios.ToArray(), null);

            // Seta na entidade
            regrasPagamentoMotorista.Descricao = descricao;
            regrasPagamentoMotorista.Observacoes = observacao;
            regrasPagamentoMotorista.Vigencia = dataVigencia;
            regrasPagamentoMotorista.NumeroAprovadores = numeroAprovadores;
            regrasPagamentoMotorista.Aprovadores = listaAprovadores;

            regrasPagamentoMotorista.RegraPorEmpresa = usarRegraPorEmpresa;
            regrasPagamentoMotorista.RegraPorTipo = usarRegraPorTipo;
            regrasPagamentoMotorista.RegraPorValor = usarRegraPorValor;
        }

        private void PreencherEntidadeRegra<T>(string nomePropriedade, string parametroJson, bool usarDynamic, ref List<T> regrasProTipo, ref Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista regrasPagamentoMotorista, Func<dynamic, object> lambda = null)
        {
            /* Descricao
             * RegrasAutorizacaoOcorrencia é passado com ref, pois é vinculado a regra específica (RegraPorTipo) e após inserir no banco, a referencia permanece com o Codigo válido
             * 
             * Esse método facilita a instancia de novas regras, já que todas possuem o mesmo padra
             * - RegraOcorrencia (Entidade Pai)
             * - Ordem
             * - Codicao
             * - Juncao
             * - TIPO
             * 
             * Esse último, é instanciado com o retorno do callback, já que é o único parametro que é modificado
             * Mas quando não for uma enteidade, mas um valor simples, basta usar a flag usarDynamic = true,
             * Fazendo isso é setado o valor que vem no RegrasPorTipo.Valor
             */

            // Converte json (com o parametro get)
            List<RegrasPorTipo> dynRegras = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RegrasPorTipo>>(Request.Params(parametroJson));

            if (dynRegras == null)
                throw new Exception("Erro ao converter os dados recebidos.");

            // Variavel auxiliar
            PropertyInfo prop;

            // Itera retornos
            for (var i = 0; i < dynRegras.Count(); i++)
            {
                // Instancia o objeto T (T não possui construor new)
                T regra = default(T);
                regra = Activator.CreateInstance<T>();

                // Seta as propriedas da entidade
                int codigoRegra = 0;
                int.TryParse(dynRegras[i].Codigo.ToString(), out codigoRegra);
                prop = regra.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, codigoRegra, null);

                prop = regra.GetType().GetProperty("RegrasPagamentoMotorista", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, regrasPagamentoMotorista, null);

                prop = regra.GetType().GetProperty("Ordem", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Ordem, null);

                prop = regra.GetType().GetProperty("Condicao", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Condicao, null);

                prop = regra.GetType().GetProperty("Juncao", BindingFlags.Public | BindingFlags.Instance);
                prop.SetValue(regra, dynRegras[i].Juncao, null);

                if (!usarDynamic)
                {
                    // Executa lambda
                    var result = dynRegras[i].Entidade != null ? lambda(dynRegras[i].Entidade.Codigo) : null;

                    prop = regra.GetType().GetProperty(nomePropriedade, BindingFlags.Public | BindingFlags.Instance);
                    prop.SetValue(regra, result, null);
                }
                else
                {
                    prop = regra.GetType().GetProperty(nomePropriedade, BindingFlags.Public | BindingFlags.Instance);
                    if (prop.PropertyType.Name.Equals("Decimal"))
                    {
                        decimal valorDecimal = 0;
                        decimal.TryParse(dynRegras[i].Valor.ToString(), out valorDecimal);

                        prop.SetValue(regra, valorDecimal, null);
                    }
                    else
                    {
                        prop.SetValue(regra, dynRegras[i].Valor, null);
                    }
                }

                // Adiciona lista de retorno
                regrasProTipo.Add(regra);
            }

        }

        private bool ValidarEntidade(Dominio.Entidades.Embarcador.PagamentoMotorista.RegrasPagamentoMotorista regrasPagamentoMotorista, out List<string> erros)
        {
            erros = new List<string>();

            if (string.IsNullOrWhiteSpace(regrasPagamentoMotorista.Descricao))
                erros.Add("Descrição é obrigatória.");

            if (regrasPagamentoMotorista.NumeroAprovadores < 0)
                erros.Add("Número de Aprovadores é obrigatória.");

            if (regrasPagamentoMotorista.Aprovadores.Count() < regrasPagamentoMotorista.NumeroAprovadores)
                erros.Add("O número de aprovadores selecionados deve ser maior ou igual a " + regrasPagamentoMotorista.NumeroAprovadores.ToString());

            return erros.Count() == 0;
        }

        private bool ValidarEntidadeRegra<T>(string nomeRegra, string nomePropriedade, List<T> regrasProTipo, out List<string> erros)
        {
            erros = new List<string>();

            if (regrasProTipo.Count() == 0)
                erros.Add("Nenhuma regra " + nomeRegra + " cadastrada.");
            else
            {
                // Variavel auxiliar
                PropertyInfo prop;

                // Itera validacao
                for (var i = 0; i < regrasProTipo.Count(); i++)
                {
                    var regra = regrasProTipo[i];
                    prop = regra.GetType().GetProperty(nomePropriedade, BindingFlags.Public | BindingFlags.Instance);

                    if (prop.GetValue(regra) == null)
                        erros.Add(nomeRegra + " da regra é obrigatório.");
                }
            }

            return erros.Count() == 0;
        }

        private RegrasPorTipo RetornaRegraPorTipoDyn<T>(dynamic obj, string paramentro, string paramentroDescricaoValor, bool usarValor = false)
        {
            // Variavel auxiliar
            PropertyInfo prop;


            prop = obj.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
            int codigo = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Ordem", BindingFlags.Public | BindingFlags.Instance);
            int ordem = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Juncao", BindingFlags.Public | BindingFlags.Instance);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia juncao = prop.GetValue(obj);

            prop = obj.GetType().GetProperty("Condicao", BindingFlags.Public | BindingFlags.Instance);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia condicao = prop.GetValue(obj);


            ObjetoEntidade objetoEntidade = new ObjetoEntidade();
            dynamic valor = null;
            if (!usarValor)
            {
                prop = obj.GetType().GetProperty(paramentro, BindingFlags.Public | BindingFlags.Instance);
                dynamic entidade = prop.GetValue(obj);

                prop = entidade.GetType().GetProperty("Codigo", BindingFlags.Public | BindingFlags.Instance);
                dynamic codigoEntidade = prop.GetValue(entidade);

                prop = entidade.GetType().GetProperty(paramentroDescricaoValor, BindingFlags.Public | BindingFlags.Instance);
                string descricaoEntidade = prop.GetValue(entidade);

                objetoEntidade.Codigo = codigoEntidade;
                objetoEntidade.Descricao = descricaoEntidade;
            }
            else
            {
                prop = obj.GetType().GetProperty(paramentroDescricaoValor, BindingFlags.Public | BindingFlags.Instance);
                valor = prop.GetValue(obj);
            }

            RegrasPorTipo restorno = new RegrasPorTipo()
            {
                Codigo = codigo,
                Ordem = ordem,
                Juncao = juncao,
                Condicao = condicao,
                Entidade = objetoEntidade,
                Valor = valor,
            };
            return restorno;
        }
        #endregion
    }
}
