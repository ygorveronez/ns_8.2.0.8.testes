using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Faturas
{
    [CustomAuthorize("Faturas/Fatura", "SAC/AtendimentoCliente")]
    public class FaturaAprovacaoController : BaseController
    {
		#region Construtores

		public FaturaAprovacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura repositorioAprovacao = new Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura(unitOfWork);
                Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura autorizacao = repositorioAprovacao.BuscarPorCodigo(codigo);

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

                Repositorio.Embarcador.Fatura.Fatura repositorioFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repositorioFatura.BuscarPorCodigo(codigo);

                if (fatura == null)
                    return new JsonpResult(false, true, "Fatura não encontrada.");

                return new JsonpResult(Servicos.Embarcador.Fatura.Fatura.ObterDetalhesAprovacao(fatura, unitOfWork));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes da aprovação da Fatura.");
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
                grid.AdicionarCabecalho("Prioridade", "PrioridadeAprovacao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura repositorioAprovacao = new Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura(unitOfWork);
                List<Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura> listaAutorizacao = repositorioAprovacao.ConsultarAutorizacoes(codigo, parametrosConsulta);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Fatura.Fatura repositorioFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repositorioFatura.BuscarPorCodigo(codigo);

                if (fatura == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (fatura.Situacao != SituacaoFatura.SemRegraAprovacao)
                    return new JsonpResult(false, true, "A situação não permite esta operação.");

                Servicos.Embarcador.Fatura.Fatura servicoFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                servicoFatura.EtapaAprovacao(fatura, TipoServicoMultisoftware,unitOfWork);

                repositorioFatura.Atualizar(fatura);

                unitOfWork.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Fatura.Fatura.ObterDetalhesAprovacao(fatura, unitOfWork));
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarRegrasCadastradas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura repositorioAprovacaoAlcadaFatura = new Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura(unitOfWork);

                bool possuiRegrasAprovacao = repositorioAprovacaoAlcadaFatura.PossuiRegrasCadastradas();

                return new JsonpResult(new
                {
                    PossuiRegrasAprovacao = possuiRegrasAprovacao
                });
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
