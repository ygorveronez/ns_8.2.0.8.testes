using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.FaturamentoMensal
{
    [CustomAuthorize("FaturamentosMensais/FaturamentoMensalGrupo")]
    public class FaturamentoMensalGrupoController : BaseController
    {
		#region Construtores

		public FaturamentoMensalGrupoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                string descricao = Request.Params("Descricao");
                int codigo = 0;
                int.TryParse(descricao, out codigo);
                if (codigo > 0)
                    descricao = "";
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Código", "Codigo", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "DescricaoAtivo")
                    propOrdenar = "Ativo";

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo repFaturamentoMensalGrupo = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo(unitOfWork);
                List<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo> listaFaturamentoMensalGrupo = repFaturamentoMensalGrupo.Consultar(codigo, descricao, ativo, this.Usuario.Empresa.Codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFaturamentoMensalGrupo.ContarConsulta(codigo, descricao, ativo, this.Usuario.Empresa.Codigo));

                var lista = (from p in listaFaturamentoMensalGrupo
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoAtivo
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

                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo repFaturamentoMensalGrupo = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);

                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo faturamentoMensalGrupo = new Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo();

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal tipoObservacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Nenhum;
                Enum.TryParse(Request.Params("TipoObservacao"), out tipoObservacao);
                faturamentoMensalGrupo.Ativo = bool.Parse(Request.Params("Ativo"));
                faturamentoMensalGrupo.FaturamentoAutomatico = bool.Parse(Request.Params("FaturamentoAutomatico"));
                faturamentoMensalGrupo.Descricao = Request.Params("Descricao");
                faturamentoMensalGrupo.Observacao = Request.Params("Observacao");
                faturamentoMensalGrupo.TipoObservacaoFaturamentoMensal = tipoObservacao;
                faturamentoMensalGrupo.Empresa = this.Usuario.Empresa;
                faturamentoMensalGrupo.ObservacaoAdesao = Request.Params("ObservacaoAdesao");

                int diaFatura = 0, codigoServico = 0, codigoNaturezaOperacaoDentroEstado = 0, codigoTipoMovimento = 0, codigoBoletoConfiguracao = 0, codigoNaturezaOperacaoForaEstado = 0;
                int.TryParse(Request.Params("DiaFatura"), out diaFatura);
                int.TryParse(Request.Params("Servico"), out codigoServico);
                int.TryParse(Request.Params("NaturezaDaOperacaoDentroEstado"), out codigoNaturezaOperacaoDentroEstado);
                int.TryParse(Request.Params("NaturezaDaOperacaoForaEstado"), out codigoNaturezaOperacaoForaEstado);
                int.TryParse(Request.Params("TipoMovimento"), out codigoTipoMovimento);
                int.TryParse(Request.Params("BoletoConfiguracao"), out codigoBoletoConfiguracao);

                if (diaFatura > 31)
                    return new JsonpResult(false, "Favor informe uma dia da fatura menor que 31.");

                if (diaFatura > 0 && faturamentoMensalGrupo.FaturamentoAutomatico)
                    faturamentoMensalGrupo.DiaFatura = diaFatura;
                else
                    faturamentoMensalGrupo.DiaFatura = 0;
                if (codigoServico > 0 && faturamentoMensalGrupo.FaturamentoAutomatico)
                    faturamentoMensalGrupo.Servico = repServico.BuscarPorCodigo(codigoServico);
                else
                    faturamentoMensalGrupo.Servico = null;
                if (codigoNaturezaOperacaoDentroEstado > 0 && faturamentoMensalGrupo.FaturamentoAutomatico)
                    faturamentoMensalGrupo.NaturezaDaOperacaoDentroEstado = repNaturezaDaOperacao.BuscarPorId(codigoNaturezaOperacaoDentroEstado);
                else
                    faturamentoMensalGrupo.NaturezaDaOperacaoDentroEstado = null;
                if (codigoNaturezaOperacaoForaEstado > 0 && faturamentoMensalGrupo.FaturamentoAutomatico)
                    faturamentoMensalGrupo.NaturezaDaOperacaoForaEstado = repNaturezaDaOperacao.BuscarPorId(codigoNaturezaOperacaoForaEstado);
                else
                    faturamentoMensalGrupo.NaturezaDaOperacaoForaEstado = null;
                if (codigoTipoMovimento > 0 && faturamentoMensalGrupo.FaturamentoAutomatico)
                    faturamentoMensalGrupo.TipoMovimento = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento);
                else
                    faturamentoMensalGrupo.TipoMovimento = null;
                if (codigoBoletoConfiguracao > 0 && faturamentoMensalGrupo.FaturamentoAutomatico)
                    faturamentoMensalGrupo.BoletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao);
                else
                    faturamentoMensalGrupo.BoletoConfiguracao = null;

                repFaturamentoMensalGrupo.Inserir(faturamentoMensalGrupo, Auditado);

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
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo repFaturamentoMensalGrupo = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
                Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(unitOfWork);
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);

                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo faturamentoMensalGrupo = repFaturamentoMensalGrupo.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal tipoObservacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Nenhum;
                Enum.TryParse(Request.Params("TipoObservacao"), out tipoObservacao);
                faturamentoMensalGrupo.Ativo = bool.Parse(Request.Params("Ativo"));
                faturamentoMensalGrupo.FaturamentoAutomatico = bool.Parse(Request.Params("FaturamentoAutomatico"));
                faturamentoMensalGrupo.Descricao = Request.Params("Descricao");
                faturamentoMensalGrupo.Observacao = Request.Params("Observacao");
                faturamentoMensalGrupo.TipoObservacaoFaturamentoMensal = tipoObservacao;
                faturamentoMensalGrupo.Empresa = this.Usuario.Empresa;
                faturamentoMensalGrupo.ObservacaoAdesao = Request.Params("ObservacaoAdesao");

                int diaFatura = 0, codigoServico = 0, codigoNaturezaOperacaoDentroEstado = 0, codigoTipoMovimento = 0, codigoBoletoConfiguracao = 0, codigoNaturezaOperacaoForaEstado = 0;
                int.TryParse(Request.Params("DiaFatura"), out diaFatura);
                int.TryParse(Request.Params("Servico"), out codigoServico);
                int.TryParse(Request.Params("NaturezaDaOperacaoDentroEstado"), out codigoNaturezaOperacaoDentroEstado);
                int.TryParse(Request.Params("NaturezaDaOperacaoForaEstado"), out codigoNaturezaOperacaoForaEstado);
                int.TryParse(Request.Params("TipoMovimento"), out codigoTipoMovimento);
                int.TryParse(Request.Params("BoletoConfiguracao"), out codigoBoletoConfiguracao);

                if (diaFatura > 31)
                    return new JsonpResult(false, "Favor informe uma dia da fatura menor que 31.");

                if (diaFatura > 0 && faturamentoMensalGrupo.FaturamentoAutomatico)
                    faturamentoMensalGrupo.DiaFatura = diaFatura;
                else
                    faturamentoMensalGrupo.DiaFatura = 0;
                if (codigoServico > 0 && faturamentoMensalGrupo.FaturamentoAutomatico)
                    faturamentoMensalGrupo.Servico = repServico.BuscarPorCodigo(codigoServico);
                else
                    faturamentoMensalGrupo.Servico = null;
                if (codigoNaturezaOperacaoDentroEstado > 0 && faturamentoMensalGrupo.FaturamentoAutomatico)
                    faturamentoMensalGrupo.NaturezaDaOperacaoDentroEstado = repNaturezaDaOperacao.BuscarPorId(codigoNaturezaOperacaoDentroEstado);
                else
                    faturamentoMensalGrupo.NaturezaDaOperacaoDentroEstado = null;
                if (codigoNaturezaOperacaoForaEstado > 0 && faturamentoMensalGrupo.FaturamentoAutomatico)
                    faturamentoMensalGrupo.NaturezaDaOperacaoForaEstado = repNaturezaDaOperacao.BuscarPorId(codigoNaturezaOperacaoForaEstado);
                else
                    faturamentoMensalGrupo.NaturezaDaOperacaoForaEstado = null;
                if (codigoTipoMovimento > 0 && faturamentoMensalGrupo.FaturamentoAutomatico)
                    faturamentoMensalGrupo.TipoMovimento = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento);
                else
                    faturamentoMensalGrupo.TipoMovimento = null;
                if (codigoBoletoConfiguracao > 0 && faturamentoMensalGrupo.FaturamentoAutomatico)
                    faturamentoMensalGrupo.BoletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(codigoBoletoConfiguracao);
                else
                    faturamentoMensalGrupo.BoletoConfiguracao = null;

                repFaturamentoMensalGrupo.Atualizar(faturamentoMensalGrupo, Auditado);

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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo repFaturamentoMensalGrupo = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo(unitOfWork);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo faturamentoMensalGrupo = repFaturamentoMensalGrupo.BuscarPorCodigo(codigo);

                var dynEquipamento = new
                {
                    faturamentoMensalGrupo.Ativo,
                    faturamentoMensalGrupo.FaturamentoAutomatico,
                    faturamentoMensalGrupo.Codigo,
                    faturamentoMensalGrupo.Descricao,
                    faturamentoMensalGrupo.Observacao,
                    TipoObservacao = faturamentoMensalGrupo.TipoObservacaoFaturamentoMensal,
                    faturamentoMensalGrupo.ObservacaoAdesao,
                    DiaFatura = faturamentoMensalGrupo.DiaFatura.ToString("n0"),
                    Servico = new
                    {
                        Codigo = faturamentoMensalGrupo.Servico != null ? faturamentoMensalGrupo.Servico.Codigo : 0,
                        Descricao = faturamentoMensalGrupo.Servico != null ? faturamentoMensalGrupo.Servico.Descricao : ""
                    },
                    NaturezaDaOperacaoDentroEstado = new
                    {
                        Codigo = faturamentoMensalGrupo.NaturezaDaOperacaoDentroEstado != null ? faturamentoMensalGrupo.NaturezaDaOperacaoDentroEstado.Codigo : 0,
                        Descricao = faturamentoMensalGrupo.NaturezaDaOperacaoDentroEstado != null ? faturamentoMensalGrupo.NaturezaDaOperacaoDentroEstado.Descricao : ""
                    },
                    NaturezaDaOperacaoForaEstado = new
                    {
                        Codigo = faturamentoMensalGrupo.NaturezaDaOperacaoForaEstado != null ? faturamentoMensalGrupo.NaturezaDaOperacaoForaEstado.Codigo : 0,
                        Descricao = faturamentoMensalGrupo.NaturezaDaOperacaoForaEstado != null ? faturamentoMensalGrupo.NaturezaDaOperacaoForaEstado.Descricao : ""
                    },
                    TipoMovimento = new
                    {
                        Codigo = faturamentoMensalGrupo.TipoMovimento != null ? faturamentoMensalGrupo.TipoMovimento.Codigo : 0,
                        Descricao = faturamentoMensalGrupo.TipoMovimento != null ? faturamentoMensalGrupo.TipoMovimento.Descricao : ""
                    },
                    BoletoConfiguracao = new
                    {
                        Codigo = faturamentoMensalGrupo.BoletoConfiguracao != null ? faturamentoMensalGrupo.BoletoConfiguracao.Codigo : 0,
                        Descricao = faturamentoMensalGrupo.BoletoConfiguracao != null ? faturamentoMensalGrupo.BoletoConfiguracao.DescricaoBanco : ""
                    }
                };

                return new JsonpResult(dynEquipamento);
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

                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo repFaturamentoMensalGrupo = new Repositorio.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo(unitOfWork);
                Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalGrupo faturamentoMensalGrupo = repFaturamentoMensalGrupo.BuscarPorCodigo(codigo);

                repFaturamentoMensalGrupo.Deletar(faturamentoMensalGrupo, Auditado);

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


    }
}
