using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Fretes/AutorizacaoContratoPrestacaoServico")]
    public class AutorizacaoContratoPrestacaoServicoController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.AprovacaoAlcadaContratoPrestacaoServico,
        Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.RegraAutorizacaoContratoPrestacaoServico,
        Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico
    >
    {
		#region Construtores

		public AutorizacaoContratoPrestacaoServicoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais Sobrescritos

		public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.ContratoPrestacaoServico repositorio = new Repositorio.Embarcador.Frete.ContratoPrestacaoServico(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico = repositorio.BuscarPorCodigo(codigo);

                if (contratoPrestacaoServico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    contratoPrestacaoServico.Codigo,
                    DataFinal = contratoPrestacaoServico.DataFinal.ToString("dd/MM/yyyy"),
                    DataInicial = contratoPrestacaoServico.DataInicial.ToString("dd/MM/yyyy"),
                    contratoPrestacaoServico.Descricao,
                    contratoPrestacaoServico.Situacao,
                    SituacaoDescricao = contratoPrestacaoServico.Situacao.ObterDescricao(),
                    Status = contratoPrestacaoServico.DescricaoAtivo,
                    ValorTeto = contratoPrestacaoServico.ValorTeto.ToString("n2")
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServicoAprovacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServicoAprovacao()
            {
                CodigoUsuario = Request.GetIntParam("Usuario"),
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetNullableEnumParam<SituacaoContratoPrestacaoServico>("Situacao")
            };
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico origem)
        {
            return origem.Situacao == SituacaoContratoPrestacaoServico.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico> contratosPrestacaoServico;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServicoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoPrestacaoServico repositorioAprovacaoAlcada = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoPrestacaoServico(unitOfWork);

                contratosPrestacaoServico = repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    contratosPrestacaoServico.Remove(new Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Frete.ContratoPrestacaoServico repositorioContratoPrestacaoServico = new Repositorio.Embarcador.Frete.ContratoPrestacaoServico(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                contratosPrestacaoServico = new List<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    contratosPrestacaoServico.Add(repositorioContratoPrestacaoServico.BuscarPorCodigo((int)itemSelecionado.Codigo));
            }

            return (from contratoPrestacaoServico in contratosPrestacaoServico select contratoPrestacaoServico.Codigo).ToList();
        }

        protected override Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Inicial", "DataInicial", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Final", "DataFinal", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Teto", "ValorTeto", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação da Aprovação", "Situacao", 15, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServicoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoPrestacaoServico repositorio = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoPrestacaoServico(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico> listaContratoPrestacaoServico = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico>();

                var listaContratoPrestacaoServicoRetornar = (
                    from contratoPrestacaoServico in listaContratoPrestacaoServico
                    select new
                    {
                        contratoPrestacaoServico.Codigo,
                        contratoPrestacaoServico.Descricao,
                        DataFinal = contratoPrestacaoServico.DataFinal.ToString("dd/MM/yyyy"),
                        DataInicial = contratoPrestacaoServico.DataInicial.ToString("dd/MM/yyyy"),
                        ValorTeto = contratoPrestacaoServico.ValorTeto.ToString("N2"),
                        Situacao = contratoPrestacaoServico.Situacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaContratoPrestacaoServicoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Situacao == SituacaoContratoPrestacaoServico.AguardandoAprovacao)
            {
                SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

                if (situacaoRegrasAutorizacao != SituacaoRegrasAutorizacao.Aguardando)
                {
                    if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
                    {
                        if (new Servicos.Embarcador.Frete.ContratoPrestacaoServico(unitOfWork).LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                        {
                            origem.Situacao = SituacaoContratoPrestacaoServico.Aprovado;
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Alteração do contrato de prestação de serviço aprovada", unitOfWork);
                        }
                    }
                    else
                    {
                        origem.Situacao = SituacaoContratoPrestacaoServico.AprovacaoRejeitada;
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Alteração do contrato de prestação de serviço reprovada", unitOfWork);
                    }

                    Repositorio.Embarcador.Frete.ContratoPrestacaoServico repositorioContratoPrestacaoServico = new Repositorio.Embarcador.Frete.ContratoPrestacaoServico(unitOfWork);

                    repositorioContratoPrestacaoServico.Atualizar(origem);
                }
            }
        }

        #endregion
    }
}