using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Terceiros.ContratoFreteAcrescimoDesconto
{
    [CustomAuthorize("Terceiros/ContratoFreteAcrescimoDesconto")]
    public class ContratoFreteAcrescimoDescontoAprovacaoController : BaseController
    {
		#region Construtores

		public ContratoFreteAcrescimoDescontoAprovacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto repositorioAprovacao = new Repositorio.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto autorizacao = repositorioAprovacao.BuscarPorCodigo(codigo);

                if (autorizacao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    autorizacao.Codigo,
                    Regra = autorizacao.Descricao,
                    Situacao = autorizacao.Situacao.ObterDescricao(),
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,
                    PodeAprovar = autorizacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo),
                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
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
        public async Task<IActionResult> ObterDetalhesGeraisAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contrato = repContratoFreteAcrescimoDesconto.BuscarPorCodigo(codigo);

                if (contrato == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                return new JsonpResult(Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto.ObterDetalhesAprovacao(contrato, unitOfWork));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes da aprovação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Prioridade", "PrioridadeAprovacao", 6, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 6, Models.Grid.Align.center, false);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto repositorioAprovacao = new Repositorio.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto(unitOfWork);
                List<Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto> listaAutorizacao = repositorioAprovacao.ConsultarAutorizacoes(codigo, parametrosConsulta);
                int totalRegistros = repositorioAprovacao.ContarAutorizacoes(codigo);

                var lista = (
                    from autorizacao in listaAutorizacao
                    select new
                    {
                        autorizacao.Codigo,
                        PrioridadeAprovacao = autorizacao.RegraAutorizacao?.PrioridadeAprovacao ?? 0,
                        Situacao = autorizacao.Situacao.ObterDescricao(),
                        Usuario = autorizacao.Usuario?.Nome,
                        Regra = autorizacao.Descricao,
                        Data = autorizacao.Data.HasValue ? autorizacao.Data.ToString() : string.Empty,
                        Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
                        DT_RowColor = autorizacao.ObterCorGrid()
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
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

        public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contrato = repContratoFreteAcrescimoDesconto.BuscarPorCodigo(codigo);

                if (contrato == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (contrato.Situacao != SituacaoContratoFreteAcrescimoDesconto.SemRegra)
                    return new JsonpResult(false, true, "A situação não permite esta operação.");

                Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto servicoContratoFreteAcrescimoDesconto = new Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                servicoContratoFreteAcrescimoDesconto.EtapaAprovacao(contrato, TipoServicoMultisoftware);

                repContratoFreteAcrescimoDesconto.Atualizar(contrato);

                unitOfWork.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto.ObterDetalhesAprovacao(contrato, unitOfWork));
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar as regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
