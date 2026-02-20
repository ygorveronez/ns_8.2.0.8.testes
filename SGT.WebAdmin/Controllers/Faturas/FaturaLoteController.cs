using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Faturas
{
    [CustomAuthorize("Cargas/CancelamentoCargaLote", "Faturas/FaturaLote", "SAC/AtendimentoCliente")]
    public class FaturaLoteController : BaseController
    {
		#region Construtores

		public FaturaLoteController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> GerarFaturaLoteClientesExclusivos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Fatura.FaturamentoLote repFaturamentoLote = new Repositorio.Embarcador.Fatura.FaturamentoLote(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/FaturaLote");
                bool naoValidarCamposObrigatorios = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FaturamentoLote_RetirarCamposObrigatorios);

                Enum.TryParse(Request.Params("TipoPessoa"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa tipoPessoa);
                DateTime.TryParse(Request.Params("DataFatura"), out DateTime dataFatura);

                DateTime? dataFinal = Request.GetNullableDateTimeParam("dataFinal");
                DateTime? dataInicial = Request.GetNullableDateTimeParam("DataInicial");

                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                int codigoOrigem = Request.GetIntParam("Origem");
                int codigoDestino = Request.GetIntParam("Destino");
                int codigoGrupoPessoa = Request.GetIntParam("GrupoPessoas");

                double cnpjPessoa = Request.GetDoubleParam("Pessoa");

                string numeroBooking = Request.GetStringParam("NumeroBooking");
                int codigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemDirecao");

                int codigoTerminalOrigem = Request.GetIntParam("TerminalOrigem");
                int codigoTerminalDestino = Request.GetIntParam("TerminalDestino");

                string observacao = Request.GetStringParam("Observacao");

                Dominio.Enumeradores.TipoCTE tipoCTe = Request.GetEnumParam<Dominio.Enumeradores.TipoCTE>("TipoCTe");

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tipoPropostaMultimodal = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>("TipoPropostaMultimodal");
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null;

                if (!naoValidarCamposObrigatorios && (tipoPropostaMultimodal == null || tipoPropostaMultimodal.Count == 0))
                    return new JsonpResult(false, "Favor informe ao menos um Tipo de Proposta.");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Fatura.FaturamentoLote faturamentoLote = new Dominio.Entidades.Embarcador.Fatura.FaturamentoLote()
                {
                    Cliente = cnpjPessoa > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjPessoa) : null,
                    DataFatura = dataFatura,
                    DataFinal = dataFinal,
                    DataGeracao = DateTime.Now,
                    DataInicial = dataInicial,
                    Destino = codigoDestino > 0 ? repLocalidade.BuscarPorCodigo(codigoDestino) : null,
                    GrupoPessoas = codigoGrupoPessoa > 0 ? repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoa) : null,
                    NumeroBooking = numeroBooking,
                    Observacao = observacao,
                    Origem = codigoOrigem > 0 ? repLocalidade.BuscarPorCodigo(codigoOrigem) : null,
                    PedidoViagemNavio = codigoPedidoViagemDirecao > 0 ? repPedidoViagemNavio.BuscarPorCodigo(codigoPedidoViagemDirecao) : null,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote.Aguardando,
                    TerminalDestino = codigoTerminalDestino > 0 ? repTerminal.BuscarPorCodigo(codigoTerminalDestino) : null,
                    TerminalOrigem = codigoTerminalOrigem > 0 ? repTerminal.BuscarPorCodigo(codigoTerminalOrigem) : null,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFaturamentoLote.Faturamento,
                    TipoOperacao = codigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null,
                    TipoPessoa = tipoPessoa,
                    Usuario = this.Usuario,
                    FaturamentoAutomatico = false,
                    NotificadoOperador = false,
                    GerarFaturamentoParaClientesExclusivos = true,
                    TipoCTe = tipoCTe
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
                repFaturamentoLote.Inserir(faturamentoLote);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso, por gentileza aguarde o processamento.");

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o faturamento em lote.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> GerarFaturaLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Fatura.FaturamentoLote repFaturamentoLote = new Repositorio.Embarcador.Fatura.FaturamentoLote(unitOfWork);
                Repositorio.Embarcador.Fatura.FaturamentoLoteCTe repFaturamentoLoteCTe = new Repositorio.Embarcador.Fatura.FaturamentoLoteCTe(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoViagemNavio repPedidoViagemNavio = new Repositorio.Embarcador.Pedidos.PedidoViagemNavio(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoTerminalImportacao repTerminal = new Repositorio.Embarcador.Pedidos.TipoTerminalImportacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/FaturaLote");
                bool naoValidarCamposObrigatorios = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FaturamentoLote_RetirarCamposObrigatorios);

                Enum.TryParse(Request.Params("TipoPessoa"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa tipoPessoa);
                DateTime.TryParse(Request.Params("DataFatura"), out DateTime dataFatura);

                DateTime? dataFinal = Request.GetNullableDateTimeParam("dataFinal");
                DateTime? dataInicial = Request.GetNullableDateTimeParam("DataInicial");

                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                int codigoOrigem = Request.GetIntParam("Origem");
                int codigoDestino = Request.GetIntParam("Destino");
                int codigoGrupoPessoa = Request.GetIntParam("GrupoPessoas");
                bool apenasFaturaExclusiva = Request.GetBoolParam("ApenasFaturaExclusiva");

                double cnpjPessoa = Request.GetDoubleParam("Pessoa");

                string numeroBooking = Request.GetStringParam("NumeroBooking");
                int codigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemDirecao");

                int codigoTerminalOrigem = Request.GetIntParam("TerminalOrigem");
                int codigoTerminalDestino = Request.GetIntParam("TerminalDestino");

                string observacao = Request.GetStringParam("Observacao");
                bool consultarTodos = Request.GetBoolParam("ConsultarTodos");

                Dominio.Enumeradores.TipoCTE tipoCTe = Request.GetEnumParam<Dominio.Enumeradores.TipoCTE>("TipoCTe");

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tipoPropostaMultimodal = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>("TipoPropostaMultimodal");
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null;

                if (!naoValidarCamposObrigatorios && (tipoPropostaMultimodal == null || tipoPropostaMultimodal.Count == 0))
                    return new JsonpResult(false, "Favor informe ao menos um Tipo de Proposta.");

                List<int> codigosCTes = new List<int>();
                codigosCTes = RetornaCodigosConhecimentos(unitOfWork, consultarTodos);

                if (!consultarTodos && codigosCTes.Count == 0)
                    return new JsonpResult(false, "Favor selecione ao menos um documento para gerar o faturamento em lote.");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Fatura.FaturamentoLote faturamentoLote = new Dominio.Entidades.Embarcador.Fatura.FaturamentoLote()
                {
                    Cliente = cnpjPessoa > 0 ? repCliente.BuscarPorCPFCNPJ(cnpjPessoa) : null,
                    DataFatura = dataFatura,
                    DataFinal = dataFinal,
                    DataGeracao = DateTime.Now,
                    DataInicial = dataInicial,
                    Destino = codigoDestino > 0 ? repLocalidade.BuscarPorCodigo(codigoDestino) : null,
                    GrupoPessoas = codigoGrupoPessoa > 0 ? repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoa) : null,
                    NumeroBooking = numeroBooking,
                    Observacao = observacao,
                    Origem = codigoOrigem > 0 ? repLocalidade.BuscarPorCodigo(codigoOrigem) : null,
                    PedidoViagemNavio = codigoPedidoViagemDirecao > 0 ? repPedidoViagemNavio.BuscarPorCodigo(codigoPedidoViagemDirecao) : null,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEnvioDocumentacaoLote.Aguardando,
                    TerminalDestino = codigoTerminalDestino > 0 ? repTerminal.BuscarPorCodigo(codigoTerminalDestino) : null,
                    TerminalOrigem = codigoTerminalOrigem > 0 ? repTerminal.BuscarPorCodigo(codigoTerminalOrigem) : null,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFaturamentoLote.Faturamento,
                    TipoOperacao = codigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null,
                    TipoPessoa = tipoPessoa,
                    Usuario = this.Usuario,
                    FaturamentoAutomatico = false,
                    NotificadoOperador = false,
                    GerarFaturamentoParaClientesExclusivos = apenasFaturaExclusiva,
                    ConsultarTodos = consultarTodos,
                    TipoCTe = tipoCTe
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
                repFaturamentoLote.Inserir(faturamentoLote);

                if (!consultarTodos && codigosCTes != null && codigosCTes.Count > 0)
                {
                    foreach (var codigoCTe in codigosCTes)
                    {
                        Dominio.Entidades.Embarcador.Fatura.FaturamentoLoteCTe faturamentoLoteCTe = new Dominio.Entidades.Embarcador.Fatura.FaturamentoLoteCTe()
                        {
                            FaturamentoLote = faturamentoLote,
                            ConhecimentoDeTransporteEletronico = repCTe.BuscarPorCodigo(codigoCTe)
                        };
                        repFaturamentoLoteCTe.Inserir(faturamentoLoteCTe);
                    }
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Sucesso, por gentileza aguarde o processamento.");

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o faturamento em lote.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaConhecimentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Faturas/FaturaLote");
                bool naoValidarCamposObrigatorios = !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FaturamentoLote_RetirarCamposObrigatorios);

                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

                Enum.TryParse(Request.Params("TipoPessoa"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa tipoPessoa);
                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);
                DateTime.TryParse(Request.Params("DataFatura"), out DateTime dataFatura);

                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                int codigoOrigem = Request.GetIntParam("Origem");
                int codigoDestino = Request.GetIntParam("Destino");
                int codigoGrupoPessoa = Request.GetIntParam("GrupoPessoas");
                bool apenasFaturaExclusiva = Request.GetBoolParam("ApenasFaturaExclusiva");

                double cnpjPessoa = Request.GetDoubleParam("Pessoa");

                string numeroBooking = Request.GetStringParam("NumeroBooking");
                int codigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemDirecao");

                int codigoTerminalOrigem = Request.GetIntParam("TerminalOrigem");
                int codigoTerminalDestino = Request.GetIntParam("TerminalDestino");

                string observacao = Request.GetStringParam("Observacao");

                Dominio.Enumeradores.TipoCTE tipoCTe = Request.GetEnumParam<Dominio.Enumeradores.TipoCTE>("TipoCTe");

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tipoPropostaMultimodal = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>("TipoPropostaMultimodal");
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null;

                if (!naoValidarCamposObrigatorios && (tipoPropostaMultimodal == null || tipoPropostaMultimodal.Count == 0))
                    codigoTipoOperacao = -1;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nº Controle", "NumeroControle", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Serie", "Serie", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Doc.", "AbreviacaoModeloDocumentoFiscal", 6, Models.Grid.Align.center, true); ;
                grid.AdicionarCabecalho("T. Pagamento", "DescricaoTipoPagamento", 9, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("T. Modal", "DescricaoTipoModal", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("T. Serviço", "DescricaoTipoServico", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left, true);
                Models.Grid.EditableCell editableValorFrete = null; //new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 9);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true, false, false, false, true, editableValorFrete);
                grid.AdicionarCabecalho("CST", "CST", 4, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Alíquota", "Aliquota", 5, Models.Grid.Align.right, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                    propOrdenacao += ".Nome";
                if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                if (propOrdenacao == "DescricaoTipoPagamento")
                    propOrdenacao = "TipoPagamento";

                if (propOrdenacao == "DescricaoTipoServico")
                    propOrdenacao = "TipoServico";

                if (propOrdenacao == "DescricaoTipoModal")
                    propOrdenacao = "TipoModal";

                if (propOrdenacao == "CodigoCTE")
                    propOrdenacao = "Codigo";

                if (propOrdenacao == "AbreviacaoModeloDocumentoFiscal")
                    propOrdenacao = "ModeloDocumentoFiscal.Abreviacao";

                int quantidade = 0;

                Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros = new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura()
                {
                    CodigoGrupoPessoas = codigoGrupoPessoa,
                    CPFCNPJTomador = cnpjPessoa,
                    DataInicial = dataInicial,
                    DataFinal = dataFinal,
                    TipoOperacao = codigoTipoOperacao,
                    PedidoViagemNavio = codigoPedidoViagemDirecao,
                    TerminalOrigem = codigoTerminalOrigem,
                    TerminalDestino = codigoTerminalDestino,
                    Origem = codigoOrigem,
                    Destino = codigoDestino,
                    NumeroBooking = numeroBooking,
                    TipoPropostaMultimodal = tipoPropostaMultimodal,
                    TiposPropostasMultimodal = tiposPropostasMultimodal,
                    ApenasFaturaExclusiva = apenasFaturaExclusiva,
                    TipoCTe = tipoCTe
                };

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repDocumentoFaturamento.ConsultarConhecimentosPendenteFaturamento(filtros, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                quantidade = repDocumentoFaturamento.ContarConsultarConhecimentosPendenteFaturamento(filtros);

                grid.setarQuantidadeTotal(quantidade);
                var lista = (from obj in ctes
                             select new
                             {
                                 obj.Codigo,
                                 CodigoCTE = obj.Codigo,
                                 obj.DescricaoTipoServico,
                                 obj.DescricaoTipoModal,
                                 AbreviacaoModeloDocumentoFiscal = obj.ModeloDocumentoFiscal?.Abreviacao ?? "",
                                 CodigoEmpresa = obj.Empresa?.Codigo ?? 0,
                                 obj.Numero,
                                 obj.NumeroControle,
                                 Serie = obj.Serie?.Numero ?? 0,
                                 obj.DescricaoTipoPagamento,
                                 Remetente = obj.Remetente != null ? obj.Remetente.Nome + (!obj.Remetente.Exterior ? " (" + obj.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty) : string.Empty,
                                 Destinatario = obj.Destinatario != null ? obj.Destinatario.Cliente?.Descricao ?? obj.Destinatario.Nome : string.Empty,
                                 Destino = obj.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? "",
                                 ValorFrete = obj.ValorAReceber > 0 ? obj.ValorAReceber.ToString("n2") : "0,00",
                                 obj.CST,
                                 Aliquota = obj.AliquotaICMS > 0 ? obj.AliquotaICMS.ToString("n2") : obj.AliquotaISS.ToString("n4")
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

        private List<int> RetornaCodigosConhecimentos(Repositorio.UnitOfWork unidadeDeTrabalho, bool consultarTodos)
        {
            List<int> listaCodigos = new List<int>();
            if (!consultarTodos && !string.IsNullOrWhiteSpace(Request.Params("ListaConhecimentos")))
            {
                dynamic listaCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaConhecimentos"));
                if (listaCarga != null)
                {
                    foreach (var carga in listaCarga)
                    {
                        listaCodigos.Add(int.Parse((string)carga.Codigo));
                    }
                }
                else
                    listaCodigos = RetornaTodosCodigosConhecimentos(unidadeDeTrabalho);
            }
            else
            {
                listaCodigos = RetornaTodosCodigosConhecimentos(unidadeDeTrabalho);
            }
            return listaCodigos;
        }

        private List<int> RetornaTodosCodigosConhecimentos(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Enum.TryParse(Request.Params("TipoPessoa"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa tipoPessoa);
            DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
            DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);
            DateTime.TryParse(Request.Params("DataFatura"), out DateTime dataFatura);

            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            int codigoOrigem = Request.GetIntParam("Origem");
            int codigoDestino = Request.GetIntParam("Destino");
            int codigoGrupoPessoa = Request.GetIntParam("GrupoPessoas");
            bool apenasFaturaExclusiva = Request.GetBoolParam("ApenasFaturaExclusiva");

            double cnpjPessoa = Request.GetDoubleParam("Pessoa");

            string numeroBooking = Request.GetStringParam("NumeroBooking");
            int codigoPedidoViagemDirecao = Request.GetIntParam("PedidoViagemDirecao");

            int codigoTerminalOrigem = Request.GetIntParam("TerminalOrigem");
            int codigoTerminalDestino = Request.GetIntParam("TerminalDestino");

            string observacao = Request.GetStringParam("Observacao");

            Dominio.Enumeradores.TipoCTE tipoCTe = Request.GetEnumParam<Dominio.Enumeradores.TipoCTE>("TipoCTe");

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tipoPropostaMultimodal = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal>("TipoPropostaMultimodal");
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> tiposPropostasMultimodal = this.Usuario != null && this.Usuario.PerfilAcesso != null && this.Usuario.PerfilAcesso.TiposPropostasMultimodal != null ? this.Usuario.PerfilAcesso.TiposPropostasMultimodal.ToList() : null;

            Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura filtros = new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroDocumentosParaFatura()
            {
                CodigoGrupoPessoas = codigoGrupoPessoa,
                CPFCNPJTomador = cnpjPessoa,
                DataInicial = dataInicial,
                DataFinal = dataFinal,
                TipoOperacao = codigoTipoOperacao,
                PedidoViagemNavio = codigoPedidoViagemDirecao,
                TerminalOrigem = codigoTerminalOrigem,
                TerminalDestino = codigoTerminalDestino,
                Origem = codigoOrigem,
                Destino = codigoDestino,
                NumeroBooking = numeroBooking,
                TipoPropostaMultimodal = tipoPropostaMultimodal,
                TiposPropostasMultimodal = tiposPropostasMultimodal,
                ApenasFaturaExclusiva = apenasFaturaExclusiva,
                TipoCTe = tipoCTe
            };

            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeDeTrabalho);
            List<int> listaCodigos = repDocumentoFaturamento.ConsultarCodigosConhecimentosPendenteFaturamento(filtros);

            return listaCodigos;
        }
    }
}
