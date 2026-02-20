using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes.Pontuacao
{
    [CustomAuthorize("Fretes/FechamentoPontuacao")]
    public class FechamentoPontuacaoController : BaseController
    {
		#region Construtores

		public FechamentoPontuacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int ano = Request.GetIntParam("Ano");
                Mes mes = Request.GetEnumParam<Mes>("Mes");

                unitOfWork.Start();

                Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacao repositorioFechamentoPontuacao = new Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao fechamentoPontuacaoDuplicado = repositorioFechamentoPontuacao.BuscarPorAnoEMes(ano, mes);

                if (fechamentoPontuacaoDuplicado != null)
                    throw new ControllerException($"O período informado já possui um fechamento (Número: {fechamentoPontuacaoDuplicado.Numero}).");

                Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao fechamentoPontuacao = new Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao()
                {
                    Ano = ano,
                    DataCriacao = DateTime.Now,
                    Mes = mes,
                    Numero = repositorioFechamentoPontuacao.BuscarProximoNumero(),
                    Situacao = SituacaoFechamentoPontuacao.AguardandoFinalizacao,
                    Usuario = Usuario
                };

                repositorioFechamentoPontuacao.Inserir(fechamentoPontuacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { Codigo = fechamentoPontuacao.Codigo });
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o fechamento.");
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
                Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacao repositorioFechamentoPontuacao = new Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao fechamentoPontuacao = repositorioFechamentoPontuacao.BuscarPorCodigo(codigo, auditavel: true);

                if (fechamentoPontuacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var fechamentoPontuacaoRetornar = new
                {
                    fechamentoPontuacao.Ano,
                    fechamentoPontuacao.Codigo,
                    fechamentoPontuacao.Mes,
                    fechamentoPontuacao.Numero,
                    fechamentoPontuacao.Situacao
                };

                return new JsonpResult(fechamentoPontuacaoRetornar);
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

        public async Task<IActionResult> BuscarDetalhesPontuacaoPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador repositorioFechamentoPontuacaoTransportador = new Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador fechamentoPontuacaoTransportador = repositorioFechamentoPontuacaoTransportador.BuscarPorCodigo(codigo, auditavel: false);

                if (fechamentoPontuacaoTransportador == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportadorRegra repositorioFechamentoPontuacaoTransportadorRegra = new Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportadorRegra(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportadorRegra> listaRegras = repositorioFechamentoPontuacaoTransportadorRegra.BuscarPorFechamentoPontuacaoTransportador(fechamentoPontuacaoTransportador.Codigo);

                var detalhesPontuacaoRetornar = new
                {
                    fechamentoPontuacaoTransportador.Pontuacao,
                    Regras = (
                        from regra in listaRegras
                        select new
                        {
                            regra.Codigo,
                            regra.Descricao,
                            regra.Pontuacao
                        }
                    ).ToList()
                };

                return new JsonpResult(detalhesPontuacaoRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os detalhes da pontuação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacao repositorioFechamentoPontuacao = new Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao fechamentoPontuacao = repositorioFechamentoPontuacao.BuscarPorCodigo(codigo, auditavel: true);

                if (fechamentoPontuacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (fechamentoPontuacao.Situacao == SituacaoFechamentoPontuacao.Cancelado)
                    return new JsonpResult(false, true, "O fechamento já está cancelado.");

                if (fechamentoPontuacao.Situacao == SituacaoFechamentoPontuacao.AguardandoFinalizacao)
                    return new JsonpResult(false, true, "O fechamento pode ser cancelado somente após ser finalizado.");

                unitOfWork.Start();

                fechamentoPontuacao.Situacao = SituacaoFechamentoPontuacao.Cancelado;

                repositorioFechamentoPontuacao.Atualizar(fechamentoPontuacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar o fechamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPontuacao()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaPontuacao());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as pontuações.");
            }
        }

        #endregion

        #region Métodos Privados

        private void DefinirPropriedadeOrdenar(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            if (parametrosConsulta.PropriedadeOrdenar == "Descricao")
                parametrosConsulta.PropriedadeOrdenar = $"Ano {parametrosConsulta.DirecaoOrdenar}, Mes";
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaFechamentoPontuacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaFechamentoPontuacao()
            {
                Ano = Request.GetIntParam("Ano"),
                Mes = Request.GetNullableEnumParam<Mes>("Mes"),
                Numero = Request.GetIntParam("Numero"),
                Situacao = Request.GetNullableEnumParam<SituacaoFechamentoPontuacao>("Situacao")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using(Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 25, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaFechamentoPontuacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                DefinirPropriedadeOrdenar(parametrosConsulta);
                Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacao repositorioFechamentoPontuacao = new Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacao(unitOfWork);
                int totalRegistros = repositorioFechamentoPontuacao.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao> listaFechamentoPontuacao = totalRegistros > 0 ? repositorioFechamentoPontuacao.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacao>();

                var listaFechamentoPontuacaoRetornar = (
                    from fechamentoPontuacao in listaFechamentoPontuacao
                    select new
                    {
                        fechamentoPontuacao.Codigo,
                        fechamentoPontuacao.Descricao,
                        fechamentoPontuacao.Numero,
                        Situacao = fechamentoPontuacao.Situacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaFechamentoPontuacaoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private Models.Grid.Grid ObterGridPesquisaPontuacao()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 60, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Pontuação", "Pontuacao", 20, Models.Grid.Align.center, true);

                int totalRegistros = 0;
                List<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador> listaFechamentoPontuacaoTransportador;
                int codigoFechamentoPontuacao = Request.GetIntParam("Codigo");

                if (codigoFechamentoPontuacao > 0)
                {
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarPesquisaPontuacao);
                    Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador repositorioFechamentoPontuacaoTransportador = new Repositorio.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador(unitOfWork);
                    totalRegistros = repositorioFechamentoPontuacaoTransportador.ContarConsulta(codigoFechamentoPontuacao);
                    listaFechamentoPontuacaoTransportador = totalRegistros > 0 ? repositorioFechamentoPontuacaoTransportador.Consultar(codigoFechamentoPontuacao, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador>();
                }
                else
                    listaFechamentoPontuacaoTransportador = new List<Dominio.Entidades.Embarcador.Frete.Pontuacao.FechamentoPontuacaoTransportador>();

                var listaFechamentoPontuacaoTransportadorRetornar = (
                    from fechamentoPontuacao in listaFechamentoPontuacaoTransportador
                    select new
                    {
                        fechamentoPontuacao.Codigo,
                        fechamentoPontuacao.Pontuacao,
                        Transportador = fechamentoPontuacao.Transportador.Descricao,
                    }
                ).ToList();

                grid.AdicionaRows(listaFechamentoPontuacaoTransportadorRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenarPesquisaPontuacao(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Transportador")
                return "Transportador.RazaoSocial";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
