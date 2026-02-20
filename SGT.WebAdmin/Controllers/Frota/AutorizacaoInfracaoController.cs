using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Frota/AutorizacaoInfracao")]
    public class AutorizacaoInfracaoController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AprovacaoAlcadaInfracao,
        Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao,
        Dominio.Entidades.Embarcador.Frota.Infracao
    >
    {
		#region Construtores

		public AutorizacaoInfracaoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais Sobrescritos

		public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.Infracao repositorio = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.Infracao infracao = repositorio.BuscarPorCodigo(codigo);

                if (infracao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    infracao.Codigo,
                    Cidade = infracao.Cidade?.Descricao ?? string.Empty,
                    Data = infracao.Data.ToString("dd/MM/yyyy"),
                    infracao.Numero,
                    infracao.Situacao,
                    SituacaoDescricao = infracao.Situacao.ObterDescricao(),
                    TipoInfracao = infracao.TipoInfracao.Descricao
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

        private void GerarTitulo(Dominio.Entidades.Embarcador.Frota.Infracao origem, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Frota.Infracao servicoInfracao = new Servicos.Embarcador.Frota.Infracao(unitOfWork);

            servicoInfracao.GerarOperacoesFinalizacao(origem, TipoServicoMultisoftware, Empresa, ConfiguracaoEmbarcador.UtilizarValorDescontatoComissaoMotoristaInfracao);
        }

        private string ObterPropriedadeOrdenar(Models.Grid.Grid grid)
        {
            return grid.header[grid.indiceColunaOrdena].data;
        }

        private List<Dominio.Entidades.Embarcador.Frota.Infracao> Pesquisar(ref int totalRegistros, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaInfracaoAprovacao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaInfracaoAprovacao()
            {
                CodigoTipoInfracao = Request.GetIntParam("TipoInfracao"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                Numero = Request.GetIntParam("Numero"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                Situacao = Request.GetNullableEnumParam<SituacaoInfracao>("Situacao")
            };
            Repositorio.Embarcador.Frota.AprovacaoAlcadaInfracao repositorio = new Repositorio.Embarcador.Frota.AprovacaoAlcadaInfracao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frota.Infracao> listaInfracao = repositorio.Consultar(filtrosPesquisa, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);

            totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

            return listaInfracao;
        }

        private string ObterAprovadores(Dominio.Entidades.Embarcador.Frota.Infracao infracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.AprovacaoAlcadaInfracao repositorio = new Repositorio.Embarcador.Frota.AprovacaoAlcadaInfracao(unitOfWork);
            var aprovacoes = repositorio.BuscarDesbloqueadaPorInfracao(infracao.Codigo);
            string aprovadores = "";
            int count = 1;
            foreach (var aprovador in aprovacoes)
            {
                aprovadores += $" {aprovador.Usuario?.Nome}";
                if (aprovacoes.Count > count && aprovacoes.Count != 1)
                    aprovadores += ", ";

                count++;
            }
            return aprovadores;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Frota.Infracao origem)
        {
            return origem.Situacao == SituacaoInfracao.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Frota.Infracao> infracoes;
            bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                int totalRegistros = 0;
                dynamic listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));

                infracoes = Pesquisar(ref totalRegistros, propriedadeOrdenar: "Codigo", direcaoOrdenacao: "", inicioRegistros: 0, maximoRegistros: 0, unitOfWork: unitOfWork);

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    infracoes.Remove(new Dominio.Entidades.Embarcador.Frota.Infracao() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Frota.Infracao repositorioInfracao = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);
                dynamic listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                infracoes = new List<Dominio.Entidades.Embarcador.Frota.Infracao>();

                foreach (var itemSelecionado in listaItensSelecionados)
                {
                    infracoes.Add(repositorioInfracao.BuscarPorCodigo((int)itemSelecionado.Codigo));
                }
            }

            return (from infracao in infracoes select infracao.Codigo).ToList();
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
                grid.AdicionarCabecalho(descricao: "Número", propriedade: "Numero", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Data", propriedade: "Data", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situacao", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Tipo Infração", propriedade: "TipoInfracao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Aprovadores", propriedade: "Aprovadores", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid);
                int totalRegistros = 0;
                List<Dominio.Entidades.Embarcador.Frota.Infracao> infracoes = Pesquisar(ref totalRegistros, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                var lista = (
                    from infracao in infracoes
                    select new
                    {
                        infracao.Codigo,
                        infracao.Numero,
                        Data = infracao.Data.ToString("dd/MM/yyyy"),
                        Situacao = infracao.Situacao.ObterDescricao(),
                        TipoInfracao = infracao.TipoInfracao?.Descricao ?? string.Empty,
                        Aprovadores = ObterAprovadores(infracao, unitOfWork),
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

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Frota.Infracao origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Situacao == SituacaoInfracao.AguardandoAprovacao)
            {
                SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

                if (situacaoRegrasAutorizacao != SituacaoRegrasAutorizacao.Aguardando)
                {
                    Repositorio.Embarcador.Frota.Infracao repositorioInfracao = new Repositorio.Embarcador.Frota.Infracao(unitOfWork);

                    if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
                    {
                        if (new Servicos.Embarcador.Frota.Infracao(unitOfWork).LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                        {
                            origem.Situacao = SituacaoInfracao.Finalizada;

                            repositorioInfracao.Atualizar(origem);

                            GerarTitulo(origem, unitOfWork);
                        }
                    }
                    else
                    {
                        origem.Situacao = SituacaoInfracao.AprovacaoRejeitada;

                        repositorioInfracao.Atualizar(origem);
                    }
                }
            }
        }

        #endregion
    }
}