using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Net;

namespace SGT.WebAdmin.Controllers.Faturas
{
    [CustomAuthorize("Faturas/FaturaCancelamentoLote", "SAC/AtendimentoCliente")]
    public class FaturaCancelamentoLoteController : BaseController
    {
		#region Construtores

		public FaturaCancelamentoLoteController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaFaturasParaCancelamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFatura filtroPesquisaFatura = ObterFiltrosPesquisa();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/FaturaCancelamentoLote");
                bool naoValidarCamposObrigatorios = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FaturamentoLote_RetirarCamposObrigatorios);

                if (filtroPesquisaFatura.TerminalOrigem > 0 || filtroPesquisaFatura.PedidoViagemNavio > 0)
                {
                    if (!naoValidarCamposObrigatorios && filtroPesquisaFatura.TipoPropostaMultimodal == null)
                        return new JsonpResult(false, "Favor informe ao menos um Tipo de Proposta.");
                }
                else
                    filtroPesquisaFatura.TerminalOrigem = -1;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Nº Fatura", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nº Boleto(s)", "NumeroBoletos", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Período", "DescricaoPeriodo", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Pessoa", "Pessoa", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vencimento", "PeriodoVencimento", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Emissão", "PeriodoEmissao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("VVD", "PedidoViagemNavio", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Terminal Origem", "TerminalOrigem", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº Controles", "NumerosControle", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº Fiscais", "NumerosFiscais", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CNPJPessoa", false);
                grid.AdicionarCabecalho("CodigoGrupoPessoa", false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                    Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

                List<Dominio.Entidades.Embarcador.Fatura.Fatura> listaFatura = repFatura.Consulta(filtroPesquisaFatura, parametrosConsulta);

                grid.setarQuantidadeTotal(repFatura.ContaConsulta(filtroPesquisaFatura));

                var lista = (from p in listaFatura
                             select new Dominio.ObjetosDeValor.Embarcador.Fatura.ConsultaFatura
                             {
                                 Codigo = p.Codigo,
                                 Numero = p.Numero,
                                 NumeroBoletos = p.NumeroBoletos,
                                 DescricaoPeriodo = p.DescricaoPeriodo,
                                 Pessoa = p.Cliente != null ? p.Cliente.Descricao : p.GrupoPessoas != null ? p.GrupoPessoas.Descricao : string.Empty,
                                 DescricaoSituacao = p.DescricaoSituacao,
                                 PeriodoVencimento = p.PeriodoVencimento,
                                 PeriodoEmissao = p.DataFatura.ToString("dd/MM/yyyy"),
                                 Valor = p.Total.ToString("n2"),
                                 PedidoViagemNavio = p.PedidoViagemNavio?.Descricao ?? "",
                                 TerminalOrigem = p.TerminalOrigem?.Descricao ?? "",
                                 TipoOperacao = p.TipoOperacao?.Descricao ?? "",
                                 NumerosControle = p.NumerosControle,
                                 NumerosFiscais = p.NumerosFiscais,
                                 CNPJPessoa = p.Cliente?.CPF_CNPJ ?? 0,
                                 CodigoGrupoPessoa = p.GrupoPessoas?.Codigo ?? 0
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


        public async Task<IActionResult> GerarCancelamentoLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemDirecao");
                int codigoTerminalOrigem = Request.GetIntParam("TerminalOrigem");
                string motivo = Request.Params("Motivo");
                bool consultarTodos = Request.GetBoolParam("ConsultarTodos");
                Dominio.Enumeradores.OpcaoSimNaoPesquisa faturadoAR = Request.GetEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("FaturadoAR");
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tipoPropostaMultimodal = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>("TipoPropostaMultimodal");

                motivo = WebUtility.UrlDecode(motivo);

                if ((string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length <= 20))
                    return new JsonpResult(false, false, "O motivo deve possuir mais de 20 caracteres.");

                List<int> codigosFaturas = RetornaCodigosFatura(unitOfWork, consultarTodos);

                Repositorio.Embarcador.Fatura.FaturamentoLote repFaturamentoLote = new Repositorio.Embarcador.Fatura.FaturamentoLote(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Fatura.Fatura> faturas = repFatura.BuscarPorCodigo(codigosFaturas);

                if (faturas != null && faturas.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Fatura.FaturamentoLote faturamentoLote = new Dominio.Entidades.Embarcador.Fatura.FaturamentoLote()
                    {
                        DataGeracao = DateTime.Now,
                        DataFatura = DateTime.Now,
                        Observacao = motivo,
                        PedidoViagemNavio = codigoPedidoViagemDirecao > 0 ? repPedidoViagemNavio.BuscarPorCodigo(codigoPedidoViagemDirecao) : null,
                        Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote.Aguardando,
                        TerminalOrigem = codigoTerminalOrigem > 0 ? repTerminal.BuscarPorCodigo(codigoTerminalOrigem) : null,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFaturamentoLote.Cancelamento,
                        Usuario = this.Usuario,
                        FaturamentoAutomatico = false,
                        NotificadoOperador = false
                    };

                    if (faturamentoLote.TipoPropostaMultimodal == null)
                        faturamentoLote.TipoPropostaMultimodal = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>();
                    if (tipoPropostaMultimodal != null)
                    {
                        foreach (var tipo in tipoPropostaMultimodal)
                        {
                            faturamentoLote.TipoPropostaMultimodal.Add(tipo);
                        }
                    }

                    if (faturamentoLote.Faturas == null)
                        faturamentoLote.Faturas = new List<Dominio.Entidades.Embarcador.Fatura.Fatura>();
                    foreach (var fatura in faturas)
                    {
                        faturamentoLote.Faturas.Add(fatura);
                    }

                    repFaturamentoLote.Inserir(faturamentoLote);


                    return new JsonpResult(true, "Sucesso");
                }
                else
                    return new JsonpResult(false, false, "Nenhuma fatura selecionada.");
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
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o cancelamento em lote.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }


        private List<int> RetornaCodigosFatura(Repositorio.UnitOfWork unidadeDeTrabalho, bool consultarTodos)
        {
            List<int> listaCodigos = new List<int>();
            if (!consultarTodos && !string.IsNullOrWhiteSpace(Request.Params("ListaFaturas")))
            {
                dynamic listaFatura = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaFaturas"));
                if (listaFatura != null)
                {
                    foreach (var fatura in listaFatura)
                    {
                        listaCodigos.Add(int.Parse((string)fatura.Codigo));
                    }
                }
                else
                    listaCodigos = RetornaTodosCodigosFatura(unidadeDeTrabalho);
            }
            else
            {
                listaCodigos = RetornaTodosCodigosFatura(unidadeDeTrabalho);
            }
            return listaCodigos;
        }

        private List<int> RetornaTodosCodigosFatura(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFatura filtroPesquisaFatura = ObterFiltrosPesquisa();
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unidadeDeTrabalho);
            List<int> listaCodigos = repFatura.ConsultaCodigos(filtroPesquisaFatura);

            return listaCodigos;
        }

        public Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFatura ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFatura()
            {
                Pessoa = Request.GetDoubleParam("Pessoa"),
                GrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                DataFaturaInicial = Request.GetDateTimeParam("DataFaturaInicial"),
                DataFaturaFinal = Request.GetDateTimeParam("DataFaturaFinal"),
                NumeroBooking = Request.GetStringParam("Booking"),
                TipoPessoa = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa>("TipoPessoa"),
                TipoPropostaMultimodal = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>("TipoPropostaMultimodal"),
                FaturadoAR = Request.GetNullableEnumParam<Dominio.Enumeradores.OpcaoSimNaoPesquisa>("FaturadoAR"),
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Fechado,
                TerminalOrigem = Request.GetIntParam("TerminalOrigem"),
                PedidoViagemNavio = Request.GetIntParam("PedidoViagemDirecao")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoSituacao")
                propriedadeOrdenar = "Situacao";
            else if (propriedadeOrdenar == "DescricaoPeriodo")
                propriedadeOrdenar = "DataInicial";
            else if (propriedadeOrdenar == "Valor")
                propriedadeOrdenar = "Total";

            return propriedadeOrdenar;
        }

    }
}
