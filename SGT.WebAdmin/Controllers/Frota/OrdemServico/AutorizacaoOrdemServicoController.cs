using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota.OrdemServico
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Frota/AutorizacaoOrdemServico")]
    public class AutorizacaoOrdemServicoController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.AprovacaoAlcadaOrdemServico,
        Dominio.Entidades.Embarcador.Frota.AlcadasOrdemServico.RegraAutorizacaoOrdemServico,
        Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota
    >
    {
		#region Construtores

		public AutorizacaoOrdemServicoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		public async Task<IActionResult> PesquisarServicosOrdemServico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOrdemServico = Request.GetIntParam("Codigo");


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Serviço", "Servico", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Custo Médio", "CustoMedio", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Custo Estimado", "CustoEstimado", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Observação", "Observacao", 15, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo> servicosOrdemServico = repServicoOrdemServico.Consultar(codigoOrdemServico, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros);

                grid.setarQuantidadeTotal(repServicoOrdemServico.ContarConsulta(codigoOrdemServico));

                var lista = (from p in servicosOrdemServico
                             select new
                             {
                                 p.Codigo,
                                 Servico = p.Servico.Descricao,
                                 p.CustoMedio,
                                 p.CustoEstimado,
                                 p.Observacao
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
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

        #region Métodos Globais Sobrescritos

        public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frota.OrdemServicoFrota repositorio = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = repositorio.BuscarPorCodigo(codigo);

                Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo> servicosOrdemServico = repServicoOrdemServico.BuscarPorOrdemServico(codigo);

                if (ordemServico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    ordemServico.Codigo,
                    Data = ordemServico.DataProgramada.ToString("dd/MM/yyyy"),
                    ordemServico.Numero,
                    ordemServico.Situacao,
                    SituacaoDescricao = ordemServico.Situacao.ObterDescricao(),
                    Operador = ordemServico.Operador.Descricao,
                    Fornecedor = ordemServico.LocalManutencao?.Descricao ?? string.Empty,
                    Placa = ordemServico?.Veiculo?.Placa ?? string.Empty,
                    Servicos = servicosOrdemServico.Count > 0 ? string.Join(", ", from obj in servicosOrdemServico select obj.Descricao) : string.Empty,
                    ValorProdutos = ordemServico?.Orcamento?.ValorTotalProdutos.ToString("n2"),
                    ValorMaoDeObra = ordemServico?.Orcamento?.ValorTotalMaoObra.ToString("n2"),
                    ValorTotalServicos = ordemServico?.Orcamento?.ValorTotalOrcado.ToString("n2"),
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

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServicoAprovacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServicoAprovacao()
            {
                CodigoUsuario = Request.GetIntParam("Usuario"),
                CodigoOperador = Request.GetIntParam("Operador"),
                CpfCnpjFornecedor = Request.GetDoubleParam("Fornecedor"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataLimite = Request.GetDateTimeParam("DataLimite"),
                Numero = Request.GetIntParam("Numero"),
                Situacao = Request.GetNullableEnumParam<SituacaoOrdemServicoFrota>("Situacao"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : 0
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Equals("Data"))
                propriedadeOrdenar = "DataProgramada";

            return propriedadeOrdenar;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota origem)
        {
            return origem.Situacao == SituacaoOrdemServicoFrota.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> ordensServico;
            bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                dynamic listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServicoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Frota.AlcadasOrdemServico.AprovacaoAlcadaOrdemServico repositorioAprovacaoAlcada = new Repositorio.Embarcador.Frota.AlcadasOrdemServico.AprovacaoAlcadaOrdemServico(unitOfWork);

                ordensServico = repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    ordensServico.Remove(new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Frota.OrdemServicoFrota repositorioOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
                dynamic listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                ordensServico = new List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

                foreach (var itemSelecionado in listaItensSelecionados)
                {
                    ordensServico.Add(repositorioOrdemServicoFrota.BuscarPorCodigo((int)itemSelecionado.Codigo));
                }
            }

            return (from obj in ordensServico select obj.Codigo).ToList();
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
                grid.AdicionarCabecalho(descricao: "Tipo", propriedade: "DescricaoTipoManutencao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Local Manutenção", propriedade: "LocalManutencao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaOrdemServicoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frota.AlcadasOrdemServico.AprovacaoAlcadaOrdemServico repositorio = new Repositorio.Embarcador.Frota.AlcadasOrdemServico.AprovacaoAlcadaOrdemServico(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota> ordensServico = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota>();

                var lista = (
                    from obj in ordensServico
                    select new
                    {
                        obj.Codigo,
                        obj.Numero,
                        obj.DescricaoTipoManutencao,
                        LocalManutencao = obj.LocalManutencao?.Nome ?? string.Empty,
                        Data = obj.DataProgramada.ToString("dd/MM/yyyy"),
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

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Situacao == SituacaoOrdemServicoFrota.AguardandoAprovacao)
            {
                SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

                if (situacaoRegrasAutorizacao != SituacaoRegrasAutorizacao.Aguardando)
                {
                    Repositorio.Embarcador.Frota.OrdemServicoFrota repositorioOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);

                    if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
                    {
                        Servicos.Embarcador.Frota.OrdemServico servicoOrdemServico = new Servicos.Embarcador.Frota.OrdemServico(unitOfWork);

                        if (servicoOrdemServico.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                        {
                            origem.Situacao = SituacaoOrdemServicoFrota.EmManutencao;

                            repositorioOrdemServicoFrota.Atualizar(origem);

                            servicoOrdemServico.EnviarEmailOrdemServico(origem);
                        }
                    }
                    else
                    {
                        origem.Situacao = SituacaoOrdemServicoFrota.AprovacaoRejeitada;

                        repositorioOrdemServicoFrota.Atualizar(origem);
                    }
                }
            }
        }

        #endregion
    }
}