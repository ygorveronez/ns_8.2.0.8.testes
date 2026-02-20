using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota.OrdemServico
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Fretes/AutorizacaoTaxaDescarga")]
    public class AutorizacaoTaxaDescargaController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga,
        Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga,
        Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente
    >
    {
		#region Construtores

		public AutorizacaoTaxaDescargaController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais Sobrescritos

		public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente repositorio = new Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente taxaDescarga = repositorio.BuscarPorCodigo(codigo);

                if (taxaDescarga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    taxaDescarga.Codigo,
                    ModeloVeicular = taxaDescarga.ModeloVeicular?.Descricao ?? "",
                    DataInicial = taxaDescarga.InicioVigencia?.ToString("dd/MM/yyyy"),
                    DataFinal = taxaDescarga.FimVigencia?.ToString("dd/MM/yyyy"),
                    Valor = taxaDescarga.Valor.ToString("n2"),
                    ValorPorPallet = taxaDescarga.ValorPallet.ToString("n2"),
                    ValorPorTonelada = taxaDescarga.ValorTonelada.ToString("n2"),
                    ValorPorUnidade = taxaDescarga.ValorUnidade.ToString("n2"),
                    Situacao = taxaDescarga.Situacao,
                    SituacaoDescricao = taxaDescarga.Situacao.ObterDescricao(),
                    Filial = taxaDescarga.Filial?.Descricao ?? string.Empty,
                    Status = taxaDescarga.Ativo.ObterDescricaoAtivo(),
                    TipoCarga = taxaDescarga.TipoCarga?.Descricao ?? string.Empty,
                    Clientes = taxaDescarga.Clientes.Count > 0 ? string.Join(", ", from obj in taxaDescarga.Clientes select obj.Descricao) : string.Empty,
                    GrupoCliente = taxaDescarga.GrupoPessoas.Count > 0 ? string.Join(", ", from obj in taxaDescarga.GrupoPessoas select obj.Descricao) : string.Empty,
                    TipoOperacao = taxaDescarga.TiposOperacoes.Count > 0 ? string.Join(", ", from obj in taxaDescarga.TiposOperacoes select obj.Descricao) : string.Empty
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

        public async Task<IActionResult> ReprocessarTaxaDescarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente repositorioTaxaDescarga = new Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente taxaDescarga = repositorioTaxaDescarga.BuscarPorCodigo(codigo, auditavel: false);

                if (taxaDescarga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (taxaDescarga.Situacao != SituacaoAjusteConfiguracaoDescargaCliente.SemRegraAprovacao)
                    return new JsonpResult(new { RegraReprocessada = true });

                unitOfWork.Start();
                new Servicos.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork).CriarAprovacao(taxaDescarga, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegraReprocessada = taxaDescarga.Situacao != SituacaoAjusteConfiguracaoDescargaCliente.SemRegraAprovacao });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar a taxa de descarga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarMultiplasTaxasDescarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                List<int> codigos = ObterCodigosOrigensSelecionadas(unitOfWork);
                Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente repositorioTaxaDescarga = new Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);
                Servicos.Embarcador.Frete.ConfiguracaoDescargaCliente servicoAprovacao = new Servicos.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> registrosSemRegraAprovacao = repositorioTaxaDescarga.BuscarPorCodigosSemRegraAprovacao(codigos);
                int totalRegrasReprocessadas = 0;

                foreach (Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente taxaDescarga in registrosSemRegraAprovacao)
                {
                    servicoAprovacao.CriarAprovacao(taxaDescarga, TipoServicoMultisoftware);

                    if (taxaDescarga.Situacao != SituacaoAjusteConfiguracaoDescargaCliente.SemRegraAprovacao)
                        totalRegrasReprocessadas++;
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegrasReprocessadas = totalRegrasReprocessadas });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar as taxas de descarga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTaxaDescarga ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTaxaDescarga()
            {
                CodigoUsuario = Request.GetIntParam("Usuario"),
                CodigoOperador = Request.GetIntParam("Operador"),
                CpfCnpjClientes = Request.GetListParam<double>("Cliente"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataLimite = Request.GetDateTimeParam("DataLimite"),
                CodigoFilial = Request.GetIntParam("Filial"),
                Valor = Request.GetDecimalParam("Valor"),
                Situacao = Request.GetNullableEnumParam<SituacaoAjusteConfiguracaoDescargaCliente>("Situacao"),
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

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente origem)
        {
            return origem.Situacao == SituacaoAjusteConfiguracaoDescargaCliente.AgAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> taxasDescarga;
            bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                dynamic listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTaxaDescarga filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga repositorioAprovacaoAlcada = new Repositorio.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga(unitOfWork);

                taxasDescarga = repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    taxasDescarga.Remove(new Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente repositorioTaxaDescarga = new Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);
                dynamic listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                taxasDescarga = new List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente>();

                foreach (var itemSelecionado in listaItensSelecionados)
                {
                    taxasDescarga.Add(repositorioTaxaDescarga.BuscarPorCodigo((int)itemSelecionado.Codigo));
                }
            }

            return (from obj in taxasDescarga select obj.Codigo).ToList();
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
                grid.AdicionarCabecalho(descricao: "Modelo Veicular", propriedade: "ModeloVeicular", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Valor Unitário", propriedade: "ValorUnidade", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Valor Tonelada", propriedade: "ValorTonelada", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Valor Pallet", propriedade: "ValorPallet", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Valor", propriedade: "Valor", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Data Inicio", propriedade: "DataInicial", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Data Final", propriedade: "DataFinal", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTaxaDescarga filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga repositorio = new Repositorio.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> taxasDescarga = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente>();

                var lista = (
                    from obj in taxasDescarga
                    select new
                    {
                        obj.Codigo,
                        ValorUnidade = obj.ValorUnidade.ToString("n2"),
                        ValorTonelada = obj.ValorTonelada.ToString("n2"),
                        ValorPallet = obj.ValorPallet.ToString("n2"),
                        Valor = obj.Valor.ToString("n2"),
                        ModeloVeicular = obj.ModeloVeicular?.Descricao ?? "",
                        DataInicial = obj.InicioVigencia?.ToString("dd/MM/yyyy"),
                        DataFinal = obj.FimVigencia?.ToString("dd/MM/yyyy"),
                        Situacao = obj.Situacao.ObterDescricao(),
                        DT_RowColor = obj.Situacao.ObterCor()
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

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Situacao == SituacaoAjusteConfiguracaoDescargaCliente.AgAprovacao)
            {
                SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);
                origem.DataUltimaAlteracao = DateTime.Now;
                if (situacaoRegrasAutorizacao != SituacaoRegrasAutorizacao.Aguardando)
                {
                    Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente repositorioTaxaDescarga = new Repositorio.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);

                    if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
                    {
                        Servicos.Embarcador.Frete.ConfiguracaoDescargaCliente servicoTaxaDescarga = new Servicos.Embarcador.Frete.ConfiguracaoDescargaCliente(unitOfWork);

                        if (servicoTaxaDescarga.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                        {
                            origem.Situacao = SituacaoAjusteConfiguracaoDescargaCliente.Aprovada;
                            origem.DataAprovacao = DateTime.Now;
                            repositorioTaxaDescarga.Atualizar(origem);
                        }
                    }
                    else
                    {
                        origem.Situacao = SituacaoAjusteConfiguracaoDescargaCliente.RejeitadaAutorizacao;
                        repositorioTaxaDescarga.Atualizar(origem);
                    }
                }
            }
        }

        #endregion
    }
}