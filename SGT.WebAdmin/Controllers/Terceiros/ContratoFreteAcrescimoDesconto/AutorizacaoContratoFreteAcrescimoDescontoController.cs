using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Terceiros.ContratoFreteAcrescimoDesconto
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Terceiros/AutorizacaoContratoFreteAcrescimoDesconto")]
    public class AutorizacaoContratoFreteAcrescimoDescontoController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto,
        Dominio.Entidades.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.RegraAutorizacaoContratoFreteAcrescimoDesconto,
        Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto
    >
    {
		#region Construtores

		public AutorizacaoContratoFreteAcrescimoDescontoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais Sobrescritos

		public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repositorio = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto contratoFreteAcrescimoDesconto = repositorio.BuscarPorCodigo(codigo);

                if (contratoFreteAcrescimoDesconto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    contratoFreteAcrescimoDesconto.Codigo,
                    Data = contratoFreteAcrescimoDesconto.Data.ToString("dd/MM/yyyy"),
                    contratoFreteAcrescimoDesconto.ContratoFrete.NumeroContrato,
                    contratoFreteAcrescimoDesconto.Situacao,
                    SituacaoDescricao = contratoFreteAcrescimoDesconto.Situacao.ObterDescricao(),
                    Operador = contratoFreteAcrescimoDesconto.Usuario.Descricao,
                    Justificativa = contratoFreteAcrescimoDesconto.Justificativa.Descricao,
                    Observacao = contratoFreteAcrescimoDesconto.Observacao,
                    Valor = contratoFreteAcrescimoDesconto.Valor.ToString("n3")
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDescontoAprovacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDescontoAprovacao()
            {
                CodigoUsuario = Request.GetIntParam("Usuario"),
                CodigoJustificativa = Request.GetIntParam("Justificativa"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataLimite = Request.GetDateTimeParam("DataLimite"),
                NumeroContrato = Request.GetIntParam("NumeroContrato"),
                Situacao = Request.GetEnumParam<SituacaoContratoFreteAcrescimoDesconto>("Situacao")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Equals("NumeroContrato"))
                propriedadeOrdenar = "ContratoFrete.NumeroContrato";
            else if (propriedadeOrdenar.Equals("Justificativa"))
                propriedadeOrdenar = "Justificativa.Descricao";

            return propriedadeOrdenar;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto origem)
        {
            return origem.Situacao == SituacaoContratoFreteAcrescimoDesconto.AgAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> contratosFreteAcrescimoDesconto;
            bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                dynamic listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));

                Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDescontoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto repositorioAprovacaoAlcada = new Repositorio.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto(unitOfWork);

                contratosFreteAcrescimoDesconto = repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    contratosFreteAcrescimoDesconto.Remove(new Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repositorioContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
                dynamic listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                contratosFreteAcrescimoDesconto = new List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();

                foreach (var itemSelecionado in listaItensSelecionados)
                {
                    contratosFreteAcrescimoDesconto.Add(repositorioContratoFreteAcrescimoDesconto.BuscarPorCodigo((int)itemSelecionado.Codigo));
                }
            }

            return (from obj in contratosFreteAcrescimoDesconto select obj.Codigo).ToList();
        }

        protected override Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Número Contrato", propriedade: "NumeroContrato", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Justificativa", propriedade: "Justificativa", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Data", propriedade: "Data", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);

                Dominio.ObjetosDeValor.Embarcador.Terceiros.FiltroPesquisaContratoFreteAcrescimoDescontoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto repositorio = new Repositorio.Embarcador.Terceiros.AlcadasContratoFreteAcrescimoDesconto.AprovacaoAlcadaContratoFreteAcrescimoDesconto(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto> contratosFreteAcrescimoDesconto = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto>();

                var lista = (
                    from obj in contratosFreteAcrescimoDesconto
                    select new
                    {
                        obj.Codigo,
                        obj.ContratoFrete.NumeroContrato,
                        Justificativa = obj.Justificativa.Descricao,
                        Data = obj.Data.ToString("dd/MM/yyyy"),
                        Situacao = obj.Situacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(lista);
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

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Situacao == SituacaoContratoFreteAcrescimoDesconto.AgAprovacao)
            {
                SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

                if (situacaoRegrasAutorizacao != SituacaoRegrasAutorizacao.Aguardando)
                {
                    Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto repositorioContratoFreteAcrescimoDesconto = new Repositorio.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);

                    if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
                    {
                        Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto servicoContratoFreteAcrescimoDesconto = new Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);

                        if (servicoContratoFreteAcrescimoDesconto.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                        {
                            if (origem.DisponibilizarFechamentoDeAgregado)
                            {
                                // Valores seram aplicados no contrato de frete ao realizar o fechamento de agregado
                                origem.Situacao = SituacaoContratoFreteAcrescimoDesconto.Finalizado;
                            }
                            else
                            {
                                origem.Situacao = SituacaoContratoFreteAcrescimoDesconto.Aprovado;
                            }

                            repositorioContratoFreteAcrescimoDesconto.Atualizar(origem);
                        }
                    }
                    else
                    {
                        origem.Situacao = SituacaoContratoFreteAcrescimoDesconto.Rejeitado;

                        repositorioContratoFreteAcrescimoDesconto.Atualizar(origem);
                    }
                }
            }
        }

        #endregion
    }
}