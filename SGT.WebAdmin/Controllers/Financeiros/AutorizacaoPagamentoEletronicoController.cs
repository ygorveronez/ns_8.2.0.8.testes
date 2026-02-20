using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Financeiros/AutorizacaoPagamentoEletronico")]
    public class AutorizacaoPagamentoEletronicoController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico,
        Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico,
        Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico
    >
    {
		#region Construtores

		public AutorizacaoPagamentoEletronicoController(Conexao conexao) : base(conexao) { }

		#endregion

		[AllowAuthenticate]
        public async Task<IActionResult> Titulos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPagamentoEletronico = Request.GetIntParam("Codigo");
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaAutorizacaoPagamento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaAutorizacaoPagamento()
                {
                    CodigoPagamentoEletronico = codigoPagamentoEletronico
                };

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Nº Título", "Codigo", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nº Documento", "NumeroDocumentoTituloOriginal", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Parcela", "Sequencia", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Fornecedor", "Pessoa", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Vencimento", "DataVencimento", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "ValorOriginal", 10, Models.Grid.Align.right, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.ConsultarAutorizacaoPagamento(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTitulo.ContarConsultaAutorizacaoPagamento(filtrosPesquisa));

                var lista = (from p in listaTitulos
                             select new
                             {
                                 p.Codigo,
                                 p.NumeroDocumentoTituloOriginal,
                                 p.Sequencia,
                                 Pessoa = p.Pessoa.Descricao,
                                 DataEmissao = p.DataEmissao.HasValue ? p.DataEmissao.Value.ToString("dd/MM/yyyy") : "",
                                 DataVencimento = p.DataVencimento.HasValue ? p.DataVencimento.Value.ToString("dd/MM/yyyy") : "",
                                 ValorOriginal = p.ValorOriginal.ToString("n2")
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

        #region Métodos Globais Sobrescritos

        public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Financeiro.PagamentoEletronico repositorio = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = repositorio.BuscarPorCodigo(codigo);

                if (pagamentoEletronico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    pagamentoEletronico.Codigo,
                    Data = pagamentoEletronico.DataPagamento.Value.ToString("dd/MM/yyyy"),
                    pagamentoEletronico.Numero,
                    Situacao = pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico,
                    SituacaoDescricao = pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico.HasValue ? pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico.Value.ObterDescricao() : string.Empty,
                    BoletoConfiguracao = pagamentoEletronico.BoletoConfiguracao.Descricao,
                    Operador = pagamentoEletronico.Usuario?.Descricao ?? string.Empty,
                    DataGeracao = pagamentoEletronico.DataGeracao.Value.ToString("dd/MM/yyyy"),
                    Modalidade = pagamentoEletronico.ModalidadePagamentoEletronico.ObterDescricao(),
                    TipoConta = pagamentoEletronico.TipoContaPagamentoEletronico.ObterDescricao(),
                    Finalidade = pagamentoEletronico.FinalidadePagamentoEletronico.ObterDescricao(),
                    ValorTotal = pagamentoEletronico.ValorTotal.ToString("n2"),
                    QtdTitulos = pagamentoEletronico.QuantidadeTitulos.ToString("n0"),
                    Empresa = pagamentoEletronico.Empresa?.Descricao ?? string.Empty
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

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaPagamentoEletronicoAprovacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaPagamentoEletronicoAprovacao()
            {
                CodigoUsuario = Request.GetIntParam("Usuario"),
                CodigoBoletoConfiguracao = Request.GetIntParam("BoletoConfiguracao"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataLimite = Request.GetDateTimeParam("DataLimite"),
                Numero = Request.GetIntParam("Numero"),
                Situacao = Request.GetNullableEnumParam<SituacaoAutorizacaoPagamentoEletronico>("Situacao"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : 0
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Equals("Data"))
                propriedadeOrdenar = "DataPagamento";
            if (propriedadeOrdenar.Equals("Situacao"))
                propriedadeOrdenar = "SituacaoAutorizacaoPagamentoEletronico";

            return propriedadeOrdenar;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico origem)
        {
            return origem.SituacaoAutorizacaoPagamentoEletronico == SituacaoAutorizacaoPagamentoEletronico.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico> pagamentosEletronicos;
            bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                dynamic listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaPagamentoEletronicoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico repositorioAprovacaoAlcada = new Repositorio.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico(unitOfWork);

                pagamentosEletronicos = repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    pagamentosEletronicos.Remove(new Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Financeiro.PagamentoEletronico repositorioOrdemServicoFrota = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unitOfWork);
                dynamic listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                pagamentosEletronicos = new List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico>();

                foreach (var itemSelecionado in listaItensSelecionados)
                {
                    pagamentosEletronicos.Add(repositorioOrdemServicoFrota.BuscarPorCodigo((int)itemSelecionado.Codigo));
                }
            }

            return (from obj in pagamentosEletronicos select obj.Codigo).ToList();
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
                grid.AdicionarCabecalho(descricao: "Número", propriedade: "Numero", tamanho: 8, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Data Pagamento", propriedade: "Data", tamanho: 11, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Config. Boleto", propriedade: "BoletoConfiguracao", tamanho: 12, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Data Geração", propriedade: "DataGeracao", tamanho: 11, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Modalidade", propriedade: "ModalidadePagamentoEletronico", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Tipo da Conta", propriedade: "TipoContaPagamentoEletronico", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Finalidade", propriedade: "FinalidadePagamentoEletronico", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Valor Total", propriedade: "ValorTotal", tamanho: 10, alinhamento: Models.Grid.Align.right, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Qtd. Títulos", propriedade: "QuantidadeTitulos", tamanho: 9, alinhamento: Models.Grid.Align.right, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 12, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaPagamentoEletronicoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico repositorio = new Repositorio.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico> pagamentosEletronicos = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico>();

                var lista = (
                    from obj in pagamentosEletronicos
                    select new
                    {
                        obj.Codigo,
                        obj.Numero,
                        Data = obj.DataPagamento.Value.ToString("dd/MM/yyyy"),
                        BoletoConfiguracao = obj.BoletoConfiguracao?.Descricao ?? "",
                        Usuario = obj.Usuario?.Descricao ?? "",
                        DataGeracao = obj.DataGeracao.Value.ToString("dd/MM/yyyy"),
                        ModalidadePagamentoEletronico = obj.ModalidadePagamentoEletronico.ObterDescricao(),
                        TipoContaPagamentoEletronico = obj.TipoContaPagamentoEletronico.ObterDescricao(),
                        FinalidadePagamentoEletronico = obj.FinalidadePagamentoEletronico.ObterDescricao(),
                        ValorTotal = obj.ValorTotal.ToString("n2"),
                        QuantidadeTitulos = obj.QuantidadeTitulos.ToString("n0"),
                        Situacao = obj.SituacaoAutorizacaoPagamentoEletronico.HasValue ? obj.SituacaoAutorizacaoPagamentoEletronico.Value.ObterDescricao() : string.Empty
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

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.SituacaoAutorizacaoPagamentoEletronico == SituacaoAutorizacaoPagamentoEletronico.AguardandoAprovacao)
            {
                SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

                if (situacaoRegrasAutorizacao != SituacaoRegrasAutorizacao.Aguardando)
                {
                    Repositorio.Embarcador.Financeiro.PagamentoEletronico repositorio = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unitOfWork);

                    if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
                    {
                        Servicos.Embarcador.Financeiro.PagamentoEletronico servicoPagamentoEletronico = new Servicos.Embarcador.Financeiro.PagamentoEletronico(unitOfWork);

                        if (servicoPagamentoEletronico.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                        {
                            origem.SituacaoAutorizacaoPagamentoEletronico = SituacaoAutorizacaoPagamentoEletronico.Finalizada;

                            repositorio.Atualizar(origem);
                        }
                    }
                    else
                    {
                        origem.SituacaoAutorizacaoPagamentoEletronico = SituacaoAutorizacaoPagamentoEletronico.AprovacaoRejeitada;

                        //repositorio.Atualizar(origem);

                        Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unitOfWork);
                        Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo repPagamentoEletronicoTitulo = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo(unitOfWork);
                        Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                        Repositorio.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico repAlcada = new Repositorio.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico(unitOfWork);
                        Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico, Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico, Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico> repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico, Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico, Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico>(unitOfWork);

                        Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = repPagamentoEletronico.BuscarPorCodigo(origem.Codigo);

                        List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repPagamentoEletronicoTitulo.BuscarPorTitulosRemessa(pagamentoEletronico.Codigo);
                        for (int i = 0; i < listaTitulo.Count(); i++)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTitulo[i];
                            titulo.Historico += " REMOVIDO DA REMESSA DE PAGAMENTO " + this.Usuario.Nome + " " + DateTime.Now.ToString("dd/MM/yyyy");
                            titulo.Historico += " Nº ANTERIOR: " + pagamentoEletronico.Numero.ToString();

                            repTitulo.Atualizar(titulo);
                        }

                        List<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico> alcadasPagamentoEletronicos = repositorioAprovacao.BuscarTodos(origem.Codigo);
                        foreach (var alcada in alcadasPagamentoEletronicos)
                            repAlcada.Deletar(alcada);

                        //List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> listaPagamentoEletronicoTitulo = repPagamentoEletronicoTitulo.BuscarPorPagamento(pagamentoEletronico.Codigo);
                        //foreach (var pagTitulo in listaPagamentoEletronicoTitulo)
                        //    repPagamentoEletronicoTitulo.Deletar(pagTitulo);

                        repPagamentoEletronico.Deletar(pagamentoEletronico, Auditado);
                    }
                }
            }
        }

        #endregion
    }
}