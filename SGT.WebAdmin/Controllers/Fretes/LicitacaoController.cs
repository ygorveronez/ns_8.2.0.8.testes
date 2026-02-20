using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/Licitacao")]
    public class LicitacaoController : BaseController
    {
		#region Construtores

		public LicitacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Frete.Licitacao licitacao = new Dominio.Entidades.Embarcador.Frete.Licitacao();

                try
                {
                    PreencherLicitacao(unitOfWork, licitacao);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                Repositorio.Embarcador.Frete.Licitacao repositorio = new Repositorio.Embarcador.Frete.Licitacao(unitOfWork);

                licitacao.Numero = repositorio.BuscarProximoNumero();
                licitacao.Usuario = this.Usuario;

                SalvarTabelasCliente(licitacao, unitOfWork);

                repositorio.Inserir(licitacao, Auditado);

                AtualizarTransportadores(licitacao, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { licitacao.Codigo });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.Licitacao repositorio = new Repositorio.Embarcador.Frete.Licitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Licitacao licitacao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (licitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                try
                {
                    PreencherLicitacao(unitOfWork, licitacao);
                }
                catch (ControllerException excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                repositorio.Atualizar(licitacao, Auditado);

                AtualizarTransportadores(licitacao, unitOfWork);
                SalvarTabelasCliente(licitacao, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.Frete.Licitacao repositorio = new Repositorio.Embarcador.Frete.Licitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Licitacao licitacao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (licitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    licitacao.Codigo,
                    DataFim = licitacao.DataFim.ToString("dd/MM/yyyy"),
                    DataInicio = licitacao.DataInicio.ToString("dd/MM/yyyy"),
                    licitacao.Descricao,
                    licitacao.LiberarTodosTransportadores,
                    licitacao.Numero,
                    licitacao.Observacao,
                    TabelaFrete = new { licitacao.TabelaFrete.Codigo, licitacao.TabelaFrete.Descricao },
                    SolicitacaoLicitacao = licitacao.SolicitacaoLicitacao != null ? new { licitacao.SolicitacaoLicitacao.Codigo, licitacao.SolicitacaoLicitacao.Descricao } : null,
                    Transportadores = (
                        from transportadorLicitacao in licitacao.Transportadores
                        select new
                        {
                            transportadorLicitacao.Transportador.Codigo,
                            CNPJ = transportadorLicitacao.Transportador.CNPJ_Formatado,
                            transportadorLicitacao.Transportador.RazaoSocial,
                            Localidade = transportadorLicitacao.Transportador.Localidade?.DescricaoCidadeEstado ?? ""
                        }
                    ).ToList(),
                    Anexos = (
                        from anexo in licitacao.Anexos
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo,
                        }
                    ).ToList(),
                    TabelasFreteCliente = (from obj in licitacao.TabelasFreteCliente
                                           select new
                                           {
                                               obj.Codigo,
                                               Origem = obj.DescricaoOrigem,
                                               Destino = obj.DescricaoDestino
                                           }).ToList()
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.Licitacao repositorio = new Repositorio.Embarcador.Frete.Licitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Licitacao licitacao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (licitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(licitacao, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
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
                var grid = ObterGridPesquisa();

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

        #endregion

        #region Métodos Privados

        private void AtualizarTransportadores(Dominio.Entidades.Embarcador.Frete.Licitacao licitacao, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic transportadores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Transportadores"));

            ExcluirTransportadoresRemovidos(licitacao, transportadores, unitOfWork);
            SalvarTransportadoresAdicionados(licitacao, transportadores, unitOfWork);
        }

        private void ExcluirTransportadoresRemovidos(Dominio.Entidades.Embarcador.Frete.Licitacao licitacao, dynamic transportadores, Repositorio.UnitOfWork unitOfWork)
        {
            if (licitacao.Transportadores != null)
            {
                Repositorio.Embarcador.Frete.LicitacaoTransportador repositorio = new Repositorio.Embarcador.Frete.LicitacaoTransportador(unitOfWork);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var transportador in transportadores)
                {
                    listaCodigosAtualizados.Add(((string)transportador.Codigo).ToInt());
                }

                List<Dominio.Entidades.Embarcador.Frete.LicitacaoTransportador> listaLicitacaoTransportadorRemover = (from licitacaoTransportador in licitacao.Transportadores where !listaCodigosAtualizados.Contains(licitacaoTransportador.Transportador.Codigo) select licitacaoTransportador).ToList();

                foreach (var licitacaoTransportador in listaLicitacaoTransportadorRemover)
                {
                    repositorio.Deletar(licitacaoTransportador);
                }

                if (listaLicitacaoTransportadorRemover.Count > 0)
                {
                    string descricaoAcao = listaLicitacaoTransportadorRemover.Count == 1 ? "Transportador removido" : "Múltiplos Transportadores removidos";

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, licitacao, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaLicitacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaLicitacao()
            {
                CodigoTabelaFrete = Request.GetIntParam("TabelaFrete"),
                Descricao = Request.GetNullableStringParam("Descricao"),
                Numero = Request.GetIntParam("Numero")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Início", "DataInicio", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Fim", "DataFim", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tabela de Frete", "TabelaFrete", 20, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaLicitacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frete.Licitacao repositorio = new Repositorio.Embarcador.Frete.Licitacao(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.Licitacao> listaLicitacao = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.Licitacao>();

                var listaLicitacaoRetornar = (
                    from licitacao in listaLicitacao
                    select new
                    {
                        licitacao.Codigo,
                        licitacao.Descricao,
                        DataFim = licitacao.DataFim.ToString("dd/MM/yyyy"),
                        DataInicio = licitacao.DataInicio.ToString("dd/MM/yyyy"),
                        licitacao.Numero,
                        TabelaFrete = licitacao.TabelaFrete.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(listaLicitacaoRetornar);
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

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "TabelaFrete")
                return "TabelaFrete.Descricao";

            return propriedadeOrdenar;
        }

        private Dominio.Entidades.Empresa ObterTransportador(int codigoTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repositorio = new Repositorio.Empresa(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoTransportador) ?? throw new ControllerException("O transportador não foi encontrado.");
        }

        private void PreencherLicitacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frete.Licitacao licitacao)
        {
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
            Repositorio.Embarcador.Frete.SolicitacaoLicitacao repSolicitacaoLicitacao = new Repositorio.Embarcador.Frete.SolicitacaoLicitacao(unitOfWork);

            licitacao.DataInicio = Request.GetNullableDateTimeParam("DataInicio") ?? throw new ControllerException("A data início deve ser informada.");
            licitacao.DataFim = Request.GetNullableDateTimeParam("DataFim") ?? throw new ControllerException("A data fim deve ser informada.");
            licitacao.Descricao = Request.GetStringParam("Descricao");
            licitacao.LiberarTodosTransportadores = Request.GetBoolParam("LiberarTodosTransportadores");
            licitacao.Observacao = Request.GetNullableStringParam("Observacao");

            int codigoTabelaFrete = Request.GetIntParam("TabelaFrete");
            int codigoSolicitacaoLicitacao = Request.GetIntParam("SolicitacaoLicitacao");

            licitacao.TabelaFrete = repTabelaFrete.BuscarPorCodigo(codigoTabelaFrete) ?? throw new ControllerException("A tabela de frete informada não foi encontrada.");
            licitacao.SolicitacaoLicitacao = codigoSolicitacaoLicitacao > 0 ? repSolicitacaoLicitacao.BuscarPorCodigo(codigoSolicitacaoLicitacao, false) : null;
        }

        private void SalvarTransportadoresAdicionados(Dominio.Entidades.Embarcador.Frete.Licitacao licitacao, dynamic transportadores, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.LicitacaoTransportador repositorio = new Repositorio.Embarcador.Frete.LicitacaoTransportador(unitOfWork);
            List<int> listaCodigosAdicionar = new List<int>();

            foreach (var transportador in transportadores)
            {
                int codigoTransportador = ((string)transportador.Codigo).ToInt();
                bool transportadorNaoCadastrado = (licitacao.Transportadores != null ? (from licitacaoTransportador in licitacao.Transportadores
                                                                                        where licitacaoTransportador?.Transportador?.Codigo == codigoTransportador
                                                                                        select licitacaoTransportador).FirstOrDefault() == null : true);

                if (transportadorNaoCadastrado)
                    listaCodigosAdicionar.Add(codigoTransportador);
            }

            foreach (var codigoTransportador in listaCodigosAdicionar)
            {
                Dominio.Entidades.Embarcador.Frete.LicitacaoTransportador licitacaoTransportador = new Dominio.Entidades.Embarcador.Frete.LicitacaoTransportador()
                {
                    Licitacao = licitacao,
                    Transportador = ObterTransportador(codigoTransportador, unitOfWork)
                };

                repositorio.Inserir(licitacaoTransportador);
            }

            if (licitacao.IsInitialized() && (listaCodigosAdicionar.Count > 0))
            {
                string descricaoAcao = listaCodigosAdicionar.Count == 1 ? "Transportador adicionado" : "Múltiplos transportadores adicionados";

                Servicos.Auditoria.Auditoria.Auditar(Auditado, licitacao, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }

        private void SalvarTabelasCliente(Dominio.Entidades.Embarcador.Frete.Licitacao licitacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unidadeDeTrabalho);

            if (licitacao.TabelasFreteCliente == null)
                licitacao.TabelasFreteCliente = new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente>();
            else
                licitacao.TabelasFreteCliente.Clear();

            dynamic dynTabelasFreteCliente = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TabelasFreteCliente"));

            foreach (var dynTabelaFreteCliente in dynTabelasFreteCliente)
                licitacao.TabelasFreteCliente.Add(repTabelaFreteCliente.BuscarPorCodigo((int)dynTabelaFreteCliente.Codigo));
        }

        #endregion
    }
}
