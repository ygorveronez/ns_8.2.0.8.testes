using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;
using Zen.Barcode;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DocumentosEmissao
{
    public class CargaPedidoDocumentoCTeController : BaseController
    {
		#region Construtores

		public CargaPedidoDocumentoCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize(new string[] { "ConsultarCTesParaTransbordo", "ConsultarCTesSemCarga", "DownloadDacte", "DownloadXML" }, "Cargas/Carga", "Logistica/JanelaCarregamento")]
        public async Task<IActionResult> Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTeParaSubContratacao = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(unitOfWork);

                int.TryParse(Request.Params("CargaPedido"), out int cargaPedido);

                Models.Grid.EditableCell editableValorInteiro = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aInt, 9);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCargaPedido", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("NumeroModeloDocumentoFiscal", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Ordem", "Ordem", 7, Models.Grid.Align.left, true, false, false, false, true, editableValorInteiro);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 8, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Nº NF", "NotasFiscais", 8, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", 12, Models.Grid.Align.left, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Origem", "Origem", 12, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Destino", "Destino", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao != "Ordem")
                {
                    if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                        propOrdenacao += ".Nome";
                    if (propOrdenacao == "Destino")
                        propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                    if (propOrdenacao == "DescricaoTipoPagamento")
                        propOrdenacao = "TipoPagamento";

                    if (propOrdenacao == "DescricaoTipoServico")
                        propOrdenacao = "TipoServico";

                    if (propOrdenacao == "Origem")
                        propOrdenacao = "LocalidadeInicioPrestacao.Descricao";

                    if (propOrdenacao == "AbreviacaoModeloDocumentoFiscal")
                        propOrdenacao = "ModeloDocumentoFiscal.Abreviacao";

                    propOrdenacao = "CTe." + propOrdenacao;
                }

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cPedido = null;
                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    cPedido = repCargaPedido.BuscarPorCodigo(cargaPedido);

                if (cPedido != null && cPedido.Carga.CargaSVM && cPedido.Carga.CargaRecebidaDeIntegracao)
                {
                    propOrdenacao = "Codigo";
                    List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> CTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarCTesPorCargaPedido(cargaPedido, true, propOrdenacao, "", grid.inicio, grid.limite);

                    if (CTesParaSubContratacao.Count == 0)
                        CTesParaSubContratacao = repPedidoCTeParaSubContratacao.BuscarCTesPorCargaPedidoPacote(cargaPedido, true, propOrdenacao, "", grid.inicio, grid.limite);

                    int quantidadeTotal = repPedidoCTeParaSubContratacao.ContarCTesPorCargaPedido(cargaPedido, true) > 0 ? repPedidoCTeParaSubContratacao.ContarCTesPorCargaPedido(cargaPedido, true) : repPedidoCTeParaSubContratacao.ContarPorCargaPedidoPacoteComCodigoCargaPedido(cargaPedido);

                    grid.setarQuantidadeTotal(quantidadeTotal);

                    var lista = (from obj in CTesParaSubContratacao
                                 select new
                                 {
                                     obj.Codigo,
                                     CodigoCargaPedido = cargaPedido,
                                     CodigoCTE = obj.Codigo,
                                     obj.DescricaoTipoServico,
                                     NumeroModeloDocumentoFiscal = "57",
                                     AbreviacaoModeloDocumentoFiscal = "CTe",
                                     CodigoEmpresa = cPedido.Carga.Empresa.Codigo,
                                     Ordem = 0,
                                     obj.Numero,
                                     DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy HH:mm"),
                                     SituacaoCTe = "A",
                                     Serie = obj.Serie,
                                     Veiculo = "",
                                     GrupoPessoas = obj.Tomador?.GrupoPessoas?.Descricao ?? string.Empty,
                                     DescricaoTipoPagamento = "",
                                     Origem = obj.LocalidadeInicioPrestacao.DescricaoCidadeEstado,
                                     Remetente = obj.Remetente != null ? obj.Remetente.Nome + "(" + obj.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty,
                                     Destinatario = obj.Destinatario != null ? obj.Destinatario.Nome + "(" + obj.Destinatario.CPF_CNPJ_Formatado + ")" : string.Empty,
                                     Destino = obj.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                     ValorFrete = obj.ValorAReceber.ToString("n2"),
                                     Aliquota = obj.AliquotaICMS.ToString("n2"),
                                     NotasFiscais = ""
                                 }).ToList();
                    grid.AdicionaRows(lista);
                    return new JsonpResult(grid);

                }
                else
                {

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> cargaCTes = repCargaPedidoDocumentoCTe.Consultar(cargaPedido, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

                    grid.setarQuantidadeTotal(repCargaPedidoDocumentoCTe.ContarConsulta(cargaPedido));
                    var lista = (from obj in cargaCTes
                                 select new
                                 {
                                     obj.Codigo,
                                     CodigoCargaPedido = obj.CargaPedido.Codigo,
                                     CodigoCTE = obj.CTe.Codigo,
                                     obj.CTe.DescricaoTipoServico,
                                     NumeroModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Numero,
                                     AbreviacaoModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Abreviacao,
                                     CodigoEmpresa = obj.CTe.Empresa.Codigo,
                                     obj.Ordem,
                                     obj.CTe.Numero,
                                     DataEmissao = obj.CTe.DataEmissao?.ToString("dd/MM/yyyy HH:mm"),
                                     SituacaoCTe = obj.CTe.Status,
                                     Serie = obj.CTe.Serie.Numero,
                                     Veiculo = BuscarPlacas(obj.CTe.Veiculos.ToList()),
                                     GrupoPessoas = obj.CTe.TomadorPagador?.GrupoPessoas?.Descricao ?? string.Empty,
                                     obj.CTe.DescricaoTipoPagamento,
                                     Origem = obj.CTe.LocalidadeInicioPrestacao.DescricaoCidadeEstado,
                                     Remetente = obj.CTe.Remetente != null ? obj.CTe.Remetente.Nome + "(" + obj.CTe.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty,
                                     Destinatario = obj.CTe.Destinatario != null ? obj.CTe.Destinatario.Nome + "(" + obj.CTe.Destinatario.CPF_CNPJ_Formatado + ")" : string.Empty,
                                     Destino = obj.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                     ValorFrete = obj.CTe.ValorAReceber.ToString("n2"),
                                     Aliquota = obj.CTe.AliquotaICMS.ToString("n2"),
                                     NotasFiscais = obj.CTe.NumeroNotas
                                 }).ToList();
                    grid.AdicionaRows(lista);
                    return new JsonpResult(grid);
                }
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

        public async Task<IActionResult> ConsultarCTesParaTransbordo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Carga"), out int carga);
                int.TryParse(Request.Params("NumeroNotaFiscal"), out int numeroNotaFiscal);
                int.TryParse(Request.Params("NumeroCTe"), out int numeroCTe);

                bool cargaSVM = Request.GetBoolParam("CargaSVM");
                bool consultaInicial = Request.GetBoolParam("ConsultaInicial");

                string numeroPedidoEmbarcador = Request.GetStringParam("NumeroPedidoEmbarcador");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("NumeroModeloDocumentoFiscal", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Serie", "Serie", 5, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Origem", "Origem", 15, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Destinatário", "Destinatario", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 8, Models.Grid.Align.right, true);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                    propOrdenacao += ".Nome";
                if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                if (propOrdenacao == "Origem")
                    propOrdenacao = "LocalidadeInicioPrestacao.Descricao";

                if (propOrdenacao == "DescricaoTipoPagamento")
                    propOrdenacao = "TipoPagamento";

                if (propOrdenacao == "DescricaoTipoServico")
                    propOrdenacao = "TipoServico";

                if (propOrdenacao == "AbreviacaoModeloDocumentoFiscal")
                    propOrdenacao = "ModeloDocumentoFiscal.Abreviacao";

                propOrdenacao = "CTe." + propOrdenacao;


                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes;
                if (cargaSVM)
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                    {
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.PendeciaDocumentos

                    };

                    cargaCTes = repCargaCTe.ConsultarMultiModal(carga, numeroCTe, numeroNotaFiscal, situacoesPermitidas, consultaInicial, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

                    grid.setarQuantidadeTotal(repCargaCTe.ContarConsultaMultiModal(carga, numeroCTe, numeroNotaFiscal, situacoesPermitidas, consultaInicial));

                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoesPermitidas = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[]
                    {
                         Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgIntegracao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada
                    };


                    cargaCTes = repCargaCTe.ConsultarParaTransbordo(carga, numeroCTe, numeroNotaFiscal, numeroPedidoEmbarcador, situacoesPermitidas, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

                    grid.setarQuantidadeTotal(repCargaCTe.ContarConsultaParaTransbordo(carga, numeroCTe, numeroNotaFiscal, numeroPedidoEmbarcador, situacoesPermitidas));
                }

                var lista = (from obj in cargaCTes
                             select new
                             {
                                 obj.Codigo,
                                 CodigoCTE = obj.CTe.Codigo,
                                 obj.CTe.DescricaoTipoServico,
                                 NumeroModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Numero,
                                 AbreviacaoModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Abreviacao,
                                 CodigoEmpresa = obj.CTe.Empresa.Codigo,
                                 obj.CTe.Numero,
                                 SituacaoCTe = obj.CTe.Status,
                                 Serie = obj.CTe.Serie?.Numero ?? 0,
                                 obj.CTe.DescricaoTipoPagamento,
                                 Motorista = BuscarMotoristas(obj.CTe.Motoristas?.ToList()),
                                 Veiculo = BuscarPlacas(obj.CTe.Veiculos?.ToList()),
                                 Remetente = obj.CTe.Remetente?.Nome + "(" + obj.CTe.Remetente?.CPF_CNPJ_Formatado + ")",
                                 Destinatario = obj.CTe.Destinatario?.Nome + "(" + obj.CTe.Destinatario?.CPF_CNPJ_Formatado + ")",
                                 Origem = obj.CTe.LocalidadeInicioPrestacao?.DescricaoCidadeEstado,
                                 Destino = obj.CTe.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado,
                                 ValorFrete = obj.CTe.ValorAReceber.ToString("n2"),
                                 Aliquota = obj.CTe.AliquotaICMS.ToString("n2"),
                                 Situacao = obj.CTe.DescricaoStatus
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

        public async Task<IActionResult> ConsultarCTesSemCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaCTeSemCarga()
                {
                    CodigoEmpresa = 0,
                    NumeroInicial = Request.GetIntParam("NumeroInicial"),
                    NumeroFinal = Request.GetIntParam("NumeroFinal"),
                    DataEmissaoInicial = Request.GetNullableDateTimeParam("DataEmissaoInicial"),
                    DataEmissaoFinal = Request.GetNullableDateTimeParam("DataEmissaoFinal"),
                    CodigoVeiculo = Request.GetIntParam("Veiculo"),
                    CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                    StatusCTe = "A",
                    CodigoRemetente = Request.GetDoubleParam("Remetente"),
                    NumeroNF = Request.GetIntParam("NumeroNF"),
                    ChaveNF = Request.GetStringParam("ChaveNF")
                };

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Chave", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("Observacao", false);
                grid.AdicionarCabecalho("NumeroModeloDocumentoFiscal", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Nº NF", "NotasFiscais", 8, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", 20, Models.Grid.Align.left, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Origem", "Origem", 15, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 10, Models.Grid.Align.right, true);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                    propOrdenacao += ".Nome";
                if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                if (propOrdenacao == "Origem")
                    propOrdenacao = "LocalidadeInicioPrestacao.Descricao";

                if (propOrdenacao == "DescricaoTipoPagamento")
                    propOrdenacao = "TipoPagamento";

                if (propOrdenacao == "DescricaoTipoServico")
                    propOrdenacao = "TipoServico";

                if (propOrdenacao == "AbreviacaoModeloDocumentoFiscal")
                    propOrdenacao = "ModeloDocumentoFiscal.Abreviacao";

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    filtrosPesquisa.CodigoEmpresa = this.Usuario.Empresa.Codigo;

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                grid.setarQuantidadeTotal(repCTe.ContarConsultaCTesSemCarga(filtrosPesquisa));

                grid.AdicionaRows(repCTe.ConsultarCTesSemCarga(filtrosPesquisa, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite));

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

        private string BuscarMotoristas(List<Dominio.Entidades.MotoristaCTE> motoristas)
        {
            if (motoristas == null || motoristas.Count <= 0)
                return string.Empty;

            string motora = "";
            if (motoristas.Count > 0)
            {
                Dominio.Entidades.MotoristaCTE ultimoMotorista = motoristas.LastOrDefault();
                foreach (Dominio.Entidades.MotoristaCTE motorista in motoristas)
                {
                    motora += motorista.NomeMotorista;
                    if (motorista.Codigo != ultimoMotorista.Codigo)
                        motora += ", ";
                }
            }

            return motora;

        }

        private string BuscarPlacas(List<Dominio.Entidades.VeiculoCTE> veiculos)
        {
            if (veiculos == null || veiculos.Count <= 0)
                return string.Empty;

            Dominio.Entidades.Veiculo tracao = null;
            List<Dominio.Entidades.Veiculo> reboques = new List<Dominio.Entidades.Veiculo>();
            foreach (Dominio.Entidades.VeiculoCTE veiculo in veiculos)
            {
                if (veiculo.Veiculo.TipoVeiculo == "0")
                    tracao = veiculo.Veiculo;
                else
                    reboques.Add(veiculo.Veiculo);

            }

            string placa = "";
            if (tracao != null)
                placa = tracao.Placa;

            foreach (Dominio.Entidades.Veiculo reboque in reboques)
            {
                placa += ", " + reboque.Placa;
            }

            return placa;
        }

        public async Task<IActionResult> BuscarCTesCompativeis()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.CTe.CTEsImportados serCTEsImportados = new Servicos.Embarcador.CTe.CTEsImportados(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                int codigoCargaPedido = 0;
                int.TryParse(Request.Params("CargaPedido"), out codigoCargaPedido);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);
                serCTEsImportados.VerificarSeCargaPossuiAlgumCTe(cargaPedido, unitOfWork, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarCTesParaTransbordo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                int.TryParse(Request.Params("CargaPedido"), out int codigoCargaPedido);
                int.TryParse(Request.Params("CargaPesquisada"), out int cargaPesquisada);

                bool todosSelecionados = false;
                bool.TryParse(Request.Params("SelecionarTodos"), out todosSelecionados);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                List<int> CodigosCTes = new List<int>();
                if (todosSelecionados)
                {
                    dynamic listaCTesNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CTesNaoSelecionados"));
                    List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(cargaPesquisada);

                    foreach (var cteNaoSelecionado in listaCTesNaoSelecionados)
                        cargaCTes.Remove(new Dominio.Entidades.Embarcador.Cargas.CargaCTe() { Codigo = (int)cteNaoSelecionado.Codigo });

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTes)
                        CodigosCTes.Add(cargaCTe.CTe.Codigo);
                }
                else
                {
                    dynamic listaCTesTransbordo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CTesSelecionados"));
                    foreach (var cteTransbordado in listaCTesTransbordo)
                        CodigosCTes.Add((int)cteTransbordado.CodigoCTE);
                }

                if (CodigosCTes.Count > 200)
                    return new JsonpResult(false, true, "Não é possível realizar um transbordo de mais de 200 CT-es. Contate o suporte técnico.");

                unitOfWork.Start();

                foreach (int codigoCTeTransbordado in CodigosCTes)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTeTransbordado);

                    if (cte != null)
                    {
                        if (!repCargaCTe.ExisteCTeEmCarga(cte.Codigo))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, $"O CT-e {cte.Numero} não está vinculado à uma carga, não podendo ser utilizado para um transbordo.");
                        }

                        if ((cargaPedido.Carga.TipoOperacao?.NaoPermitirCTesParaTransbordoComDestinoDiferenteDoPedido ?? false) && cargaPedido.Destino.Codigo != cte.LocalidadeTerminoPrestacao.Codigo)
                        {
                            string mensagem = $"O CT-e {cte.Numero} possui o destino ({cte.LocalidadeTerminoPrestacao.DescricaoCidadeEstado}) diferente do destino do pedido ({cargaPedido.Destino.DescricaoCidadeEstado}), não podendo ser utilizado para este transbordo.";
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, mensagem);
                        }

                        Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = repCargaPedidoDocumentoCTe.BuscarPorCTeECargaPedido(codigoCTeTransbordado, codigoCargaPedido);

                        if (cargaPedidoDocumentoCTe == null)
                        {
                            cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe
                            {
                                CargaPedido = cargaPedido,
                                CTe = cte,
                                Ordem = repCargaPedidoDocumentoCTe.BuscarProximaOrdem(codigoCargaPedido)
                            };

                            repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe, Auditado);
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Adicionou CT-e para Transbordo", unitOfWork);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "O Código do CT-e informado é invalido");
                    }
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarEtiquetaVolumeParaTransbordo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                int.TryParse(Request.Params("CargaPedido"), out int codigoCargaPedido);
                int.TryParse(Request.Params("CargaPesquisada"), out int cargaPesquisada);

                string codigoBarras = Utilidades.String.OnlyNumbers(Request.Params("Etiqueta"));

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                if (string.IsNullOrWhiteSpace(codigoBarras))
                    return new JsonpResult(false, true, "Chave do documento inválida.");

                string numeroNS = "", volume = "", cnpjRemetente = "", numeroNota = "", serieNota = "";
                string codigoBarrasSalvar = "";
                if (codigoBarras.Length == 13)
                {
                    numeroNS = codigoBarras.Substring(0, 10);
                    volume = codigoBarras.Substring(10, 3);
                    codigoBarrasSalvar = numeroNS.ToUpper();
                }
                else if (codigoBarras.Length == 31)
                {
                    cnpjRemetente = codigoBarras.Substring(0, 14);

                    numeroNota = codigoBarras.Substring(14, 9);
                    numeroNota = numeroNota.TrimStart('0');

                    volume = codigoBarras.Substring(23, 4);

                    serieNota = codigoBarras.Substring(27, 2);
                    serieNota = serieNota.TrimStart('0');
                    codigoBarrasSalvar = codigoBarras.ToUpper();
                }
                else if (codigoBarras.Length == 33)
                {
                    cnpjRemetente = codigoBarras.Substring(0, 14);

                    numeroNota = codigoBarras.Substring(14, 9);
                    numeroNota = numeroNota.TrimStart('0');

                    volume = codigoBarras.Substring(23, 4);

                    serieNota = codigoBarras.Substring(27, 3);
                    serieNota = serieNota.TrimStart('0');
                    codigoBarrasSalvar = codigoBarras.ToUpper();
                }
                else
                {
                    return new JsonpResult(false, true, "Código de barras inválido para conferência de volume.");
                }
                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoSeparacaoVolume.RelacaoSeparacaoVolume> dadosRelacaoSeparacaoVolume = repCargaPedidoDocumentoCTe.RelatorioRelacaoSeparacaoVolume(0, numeroNS, cnpjRemetente, numeroNota, serieNota);

                if (dadosRelacaoSeparacaoVolume == null || dadosRelacaoSeparacaoVolume.Count == 0)
                    return new JsonpResult(false, true, "Não foi localizado nenhum conhecimento para a etiqueta informada.");

                List<int> codigosCTes = dadosRelacaoSeparacaoVolume.Select(c => c.CodigoCTe).Distinct().ToList();

                if (codigosCTes == null || codigosCTes.Count == 0)
                    return new JsonpResult(false, true, "Não foi localizado nenhum conhecimento para a etiqueta informada.");


                foreach (var codigoCTe in codigosCTes)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                    if (cte == null)
                        return new JsonpResult(false, true, "Não foi possível encontrar um CT-e pelo documento informado.");

                    if (!repCargaCTe.ExisteCTeEmCarga(cte.Codigo))
                        return new JsonpResult(false, true, $"O CT-e {cte.Numero} não está vinculado à uma carga, não podendo ser utilizado para um transbordo.");

                    if (repCargaCTe.CargaDoCTeNaoEstaEmitida(cte.Codigo))
                        return new JsonpResult(false, true, "A situação da carga deste CT-e não permite que o mesmo seja vinculado à um transbordo. É necessário concluir a emissão da carga.");

                    if ((cargaPedido.Carga.TipoOperacao?.NaoPermitirCTesParaTransbordoComDestinoDiferenteDoPedido ?? false) && cargaPedido.Destino.Codigo != cte.LocalidadeTerminoPrestacao.Codigo)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, $"O CT-e {cte.Descricao} possui o destino ({cte.LocalidadeTerminoPrestacao.DescricaoCidadeEstado}) diferente do destino do pedido ({cargaPedido.Destino.DescricaoCidadeEstado}), não podendo ser utilizado para este transbordo.");
                    }

                    Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = repCargaPedidoDocumentoCTe.BuscarPorCTeECargaPedido(cte.Codigo, codigoCargaPedido);

                    if (cargaPedidoDocumentoCTe == null)
                    {

                        unitOfWork.Start();

                        cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe()
                        {
                            CargaPedido = cargaPedido,
                            CTe = cte,
                            Ordem = repCargaPedidoDocumentoCTe.BuscarProximaOrdem(codigoCargaPedido)
                        };

                        repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe, Auditado);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Adicionou o CT-e " + cte.Numero.ToString() + " para transbordo.", unitOfWork);

                        unitOfWork.CommitChanges();
                    }
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> AdicionarDocumentoParaTransbordo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                int.TryParse(Request.Params("CargaPedido"), out int codigoCargaPedido);
                int.TryParse(Request.Params("CargaPesquisada"), out int cargaPesquisada);

                string documento = Utilidades.String.OnlyNumbers(Request.Params("Documento"));

                bool chave = documento.Length == 44;

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                if (string.IsNullOrWhiteSpace(documento))
                    return new JsonpResult(false, true, "Chave do documento inválida.");

                if ((chave && !Utilidades.Validate.ValidarChave(documento)) || (!chave && documento.Length != 20))
                    return new JsonpResult(false, true, "Chave do documento inválida.");

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;

                if (chave)
                {
                    string modelo = documento.Substring(20, 2);

                    if (modelo == "57")
                        cte = repCTe.BuscarPorChave(documento);
                    else if (modelo == "55")
                        cte = repCTe.BuscarPorChaveNFe(documento);
                }
                else
                {
                    int.TryParse(documento.Substring(0, 10), out int codigoCarga);
                    int.TryParse(documento.Substring(10, 10), out int codigoCTe);

                    cte = repCargaCTe.BuscarCTePorCTeECarga(codigoCTe, codigoCarga);
                }

                if (cte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar um CT-e pelo documento informado.");

                if (!repCargaCTe.ExisteCTeEmCarga(cte.Codigo))
                    return new JsonpResult(false, true, $"O CT-e {cte.Numero} não está vinculado à uma carga, não podendo ser utilizado para um transbordo.");

                if (repCargaCTe.CargaDoCTeNaoEstaEmitida(cte.Codigo))
                    return new JsonpResult(false, true, "A situação da carga deste CT-e não permite que o mesmo seja vinculado à um transbordo. É necessário concluir a emissão da carga.");

                if ((cargaPedido.Carga.TipoOperacao?.NaoPermitirCTesParaTransbordoComDestinoDiferenteDoPedido ?? false) && cargaPedido.Destino.Codigo != cte.LocalidadeTerminoPrestacao.Codigo)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, $"O CT-e {cte.Descricao} possui o destino ({cte.LocalidadeTerminoPrestacao.DescricaoCidadeEstado}) diferente do destino do pedido ({cargaPedido.Destino.DescricaoCidadeEstado}), não podendo ser utilizado para este transbordo.");
                }

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = repCargaPedidoDocumentoCTe.BuscarPorCTeECargaPedido(cte.Codigo, codigoCargaPedido);

                if (cargaPedidoDocumentoCTe == null)
                {

                    unitOfWork.Start();

                    cargaPedidoDocumentoCTe = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe()
                    {
                        CargaPedido = cargaPedido,
                        CTe = cte,
                        Ordem = repCargaPedidoDocumentoCTe.BuscarProximaOrdem(codigoCargaPedido)
                    };

                    repCargaPedidoDocumentoCTe.Inserir(cargaPedidoDocumentoCTe, Auditado);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Adicionou o CT-e " + cte.Numero.ToString() + " para transbordo.", unitOfWork);

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarOrdem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);

                int codigo = 0, ordem = 0, codigoCargaPedido = 0, ordemOriginal = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("Ordem"), out ordem);
                int.TryParse(Request.Params("CodigoCargaPedido"), out codigoCargaPedido);

                if (codigo <= 0)
                    return new JsonpResult(false, true, "Registro não encontrado.");

                if (ordem <= 0)
                    return new JsonpResult(false, true, "Ordem inválida, informe a ordenação a partir do número 1 (um).");

                Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe = repCargaPedidoDocumentoCTe.BuscarPorCodigo(codigo, true);

                if (cargaPedidoDocumentoCTe == null || cargaPedidoDocumentoCTe.CargaPedido == null || cargaPedidoDocumentoCTe.CargaPedido.Carga == null)
                    return new JsonpResult(false, true, "Registro não encontrado.");

                if (cargaPedidoDocumentoCTe.CargaPedido.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
                    return new JsonpResult(false, true, "Só é possível mudar a ordenação em cargas aguardando documentos.");

                if (cargaPedidoDocumentoCTe.CargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                unitOfWork.Start();

                ordemOriginal = ordem;
                Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTeAnterior = repCargaPedidoDocumentoCTe.BuscarPorOrdemECargaPedido(ordemOriginal, codigoCargaPedido, 0);
                while (cargaPedidoDocumentoCTeAnterior != null)
                {
                    if (cargaPedidoDocumentoCTeAnterior != null)
                    {
                        cargaPedidoDocumentoCTeAnterior.Ordem = cargaPedidoDocumentoCTeAnterior.Ordem + 1;
                        ordemOriginal = cargaPedidoDocumentoCTeAnterior.Ordem;
                        repCargaPedidoDocumentoCTe.Atualizar(cargaPedidoDocumentoCTeAnterior);
                    }
                    cargaPedidoDocumentoCTeAnterior = repCargaPedidoDocumentoCTe.BuscarPorOrdemECargaPedido(ordemOriginal, codigoCargaPedido, cargaPedidoDocumentoCTeAnterior.Codigo);
                }

                cargaPedidoDocumentoCTe.Ordem = ordem;
                repCargaPedidoDocumentoCTe.Atualizar(cargaPedidoDocumentoCTe, Auditado);


                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> documentos = repCargaPedidoDocumentoCTe.BuscarCargaPedido(codigoCargaPedido);
                for (int i = 0; i < documentos.Count; i++)
                {
                    documentos[i].Ordem = i + 1;
                    repCargaPedidoDocumentoCTe.Atualizar(documentos[i]);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int.TryParse(Request.Params("CargaPedido"), out int codigoCargaPedido);

                List<int> codigosCTes = Request.GetListParam<int>("CodigosCTes");

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                bool permiteVincularCTeComplementar = Servicos.Embarcador.Carga.Carga.PermiteVincularCTeComplementoCarga(cargaPedido);

                unitOfWork.Start();

                foreach (int codigoCTe in codigosCTes)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                    if (!Servicos.Embarcador.CTe.CTEsImportados.VincularCTeACargaPedido(out string erro, codigoCTe, codigoCargaPedido, permiteVincularCTeComplementar, unitOfWork, Auditado, ConfiguracaoEmbarcador))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erro);
                    }

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, $"Adicionou ao pedido {cargaPedido.Pedido.Numero}.", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int.TryParse(Request.Params("CodigoCTe"), out int codigoCTe);
                int.TryParse(Request.Params("CargaPedido"), out int codigoCargaPedido);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                unitOfWork.Start();

                if (!Servicos.Embarcador.CTe.CTEsImportados.RemoverCTeCargaPedido(out string erro, codigoCTe, codigoCargaPedido, unitOfWork, Auditado))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Removeu do Pedido " + cargaPedido.Pedido.Numero.ToString(), unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, "Removeu o CT-e " + cte.Descricao + ".", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int.TryParse(Request.Params("CargaPedido"), out int codigoCargaPedido);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> cargaPedidoDocumentoCTes = repCargaPedidoDocumentoCTe.BuscarPorCargaPedido(cargaPedido.Codigo);

                if (cargaPedidoDocumentoCTes.Count > 0)
                {
                    unitOfWork.Start();

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe cargaPedidoDocumentoCTe in cargaPedidoDocumentoCTes)
                    {
                        if (!Servicos.Embarcador.CTe.CTEsImportados.RemoverCTeCargaPedido(out string erro, 0, 0, unitOfWork, Auditado, cargaPedidoDocumentoCTe, false))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, erro);
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedidoDocumentoCTe.CTe, null, "Removeu do pedido " + cargaPedido.Pedido.Numero.ToString() + ".", unitOfWork);
                    }

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, "Removeu todos os CT-es para vínculo.", unitOfWork);

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImprimirRelacaoEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
                Repositorio.Embarcador.Logistica.RotaFreteCEP repBuscarPorCEP = new Repositorio.Embarcador.Logistica.RotaFreteCEP(unitOfWork);
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);

                int cargaPedido = 0;
                int.TryParse(Request.Params("CargaPedido"), out cargaPedido);
                if (cargaPedido == 0)
                    return new JsonpResult(false, false, "Favor selecione uma carga para gerar a impressão do relatório.");

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R098_RelacaoEntrega, TipoServicoMultisoftware);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                if (relatorio == null)
                {
                    relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R098_RelacaoEntrega, TipoServicoMultisoftware, "Relatorio de Relação de Entrega", "Cargas", "RelacaoEntrega.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);
                }

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                BarcodeMetrics1d metricas = new BarcodeMetrics1d();
                metricas.Scale = 5;
                string stringConexao = _conexao.StringConexao;
                string nomeCliente = Cliente.NomeFantasia;
                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntrega> dadosRelacaoEntrega = repCargaPedidoDocumentoCTe.RelatorioRelacaoEntrega(cargaPedido);
                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaDocumento> dadosRelacaoEntregaDocumento = repCargaPedidoDocumentoCTe.RelatorioRelacaoEntregaDocumento(cargaPedido);
                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaMotorista> dadosRelacaoEntregaMotorista = repCargaPedidoDocumentoCTe.RelatorioRelacaoEntregaMotorista(cargaPedido);
                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaReboque> dadosRelacaoEntregaReboque = repCargaPedidoDocumentoCTe.RelatorioRelacaoEntregaReboque(cargaPedido);

                string rotasDestinos = string.Empty;
                string rotaDest = string.Empty;
                int cepDest = 0;
                List<int> codigosRotas = new List<int>();
                Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP rotaFreteCEP = null;
                for (int k = 0; k < dadosRelacaoEntregaDocumento.Count; k++)
                {
                    int.TryParse(Utilidades.String.OnlyNumbers(dadosRelacaoEntregaDocumento[k].CEPDestinatario), out cepDest);
                    if (cepDest > 0)
                    {
                        rotaFreteCEP = repBuscarPorCEP.BuscarPorCEP(cepDest);
                        if (rotaFreteCEP != null && !codigosRotas.Contains(rotaFreteCEP.RotaFrete.Codigo))
                        {
                            rotaDest = rotaFreteCEP.RotaFrete.Descricao;
                            codigosRotas.Add(rotaFreteCEP.RotaFrete.Codigo);
                            if (!string.IsNullOrWhiteSpace(rotasDestinos))
                                rotasDestinos = ", " + rotaDest;
                            else
                                rotasDestinos = rotaDest;
                        }
                    }
                    cepDest = 0;
                    rotaFreteCEP = null;
                }

                if (dadosRelacaoEntrega.Count > 0)
                {
                    byte[] codigoBarrasViagem = Utilidades.Barcode.Gerar(dadosRelacaoEntrega[0].CodigoCarga.ToString().PadLeft(8, '0'), ZXing.BarcodeFormat.CODE_128, new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Png);
                    dadosRelacaoEntrega[0].CodigoBarrasViagem = codigoBarrasViagem;
                    dadosRelacaoEntrega[0].Rota = rotasDestinos;
                    if (dadosRelacaoEntrega.Count == 1)
                    {
                        dadosRelacaoEntrega.Add(new Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntrega()
                        {
                            CodigoCarga = dadosRelacaoEntrega[0].CodigoCarga,
                            BairroEmpresa = dadosRelacaoEntrega[0].BairroEmpresa,
                            CapacidadeVeiculo = dadosRelacaoEntrega[0].CapacidadeVeiculo,
                            CEPEmpresa = dadosRelacaoEntrega[0].CEPEmpresa,
                            CidadeEmpresa = dadosRelacaoEntrega[0].CidadeEmpresa,
                            CNPJEmpresa = dadosRelacaoEntrega[0].CNPJEmpresa,
                            CNPJProprietario = dadosRelacaoEntrega[0].CNPJProprietario,
                            ContemMotoristas = dadosRelacaoEntrega[0].ContemMotoristas,
                            ContemReboque = dadosRelacaoEntrega[0].ContemReboque,
                            EnderecoEmpresa = dadosRelacaoEntrega[0].EnderecoEmpresa,
                            EstadoEmpresa = dadosRelacaoEntrega[0].EstadoEmpresa,
                            IEEmpresa = dadosRelacaoEntrega[0].IEEmpresa,
                            NomeEmpresa = dadosRelacaoEntrega[0].NomeEmpresa,
                            NomeProprietario = dadosRelacaoEntrega[0].NomeProprietario,
                            NumeroCarga = dadosRelacaoEntrega[0].NumeroCarga,
                            NumeroFrotaVeiculo = dadosRelacaoEntrega[0].NumeroFrotaVeiculo,
                            PlacaVeiculo = dadosRelacaoEntrega[0].PlacaVeiculo,
                            QtdCTe = dadosRelacaoEntrega[0].QtdCTe,
                            QtdNotas = dadosRelacaoEntrega[0].QtdNotas,
                            QtdPeso = dadosRelacaoEntrega[0].QtdPeso,
                            QtdVolumes = dadosRelacaoEntrega[0].QtdVolumes,
                            ValorFreteSemICMS = dadosRelacaoEntrega[0].ValorFreteSemICMS,
                            ValorNotas = dadosRelacaoEntrega[0].ValorNotas,
                            ANTTEmpresa = dadosRelacaoEntrega[0].ANTTEmpresa,
                            CodigoBarrasViagem = dadosRelacaoEntrega[0].CodigoBarrasViagem,
                            CIOT = dadosRelacaoEntrega[0].CIOT,
                            Rota = dadosRelacaoEntrega[0].Rota,
                            DataFinalizacaoEmissao = dadosRelacaoEntrega[0].DataFinalizacaoEmissao,
                            NomeRemetente = dadosRelacaoEntrega[0].NomeRemetente,
                            EnderecoRemetente = dadosRelacaoEntrega[0].EnderecoRemetente,
                            BairroRemetente = dadosRelacaoEntrega[0].BairroRemetente,
                            CEPRemetente = dadosRelacaoEntrega[0].CEPRemetente,
                            CNPJRemetente = dadosRelacaoEntrega[0].CNPJRemetente,
                            IERemetente = dadosRelacaoEntrega[0].IERemetente,
                            CidadeRemetente = dadosRelacaoEntrega[0].CidadeRemetente,
                            EstadoRemetente = dadosRelacaoEntrega[0].EstadoRemetente
                        });
                    }
                    for (int i = 0; i < dadosRelacaoEntregaDocumento.Count; i++)
                    {
                        //byte[] codigoBarras = Utilidades.Barcode.Gerar((dadosRelacaoEntrega[0].CodigoCarga.ToString() + dadosRelacaoEntregaDocumento[i].NumeroCTe.ToString()).PadLeft(16, '0'), ZXing.BarcodeFormat.CODE_128, new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Png);
                        byte[] codigoBarras = Utilidades.Barcode.Gerar((dadosRelacaoEntrega[0].CodigoCarga.ToString().PadLeft(10, '0') + dadosRelacaoEntregaDocumento[i].CodigoCTe.ToString().PadLeft(10, '0')).PadLeft(20, '0'), ZXing.BarcodeFormat.CODE_128, new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Png);
                        dadosRelacaoEntregaDocumento[i].CodigoBarras = codigoBarras;
                        dadosRelacaoEntregaDocumento[i].CodigoBarrasCompleto = (dadosRelacaoEntrega[0].CodigoCarga.ToString().PadLeft(10, '0') + dadosRelacaoEntregaDocumento[i].CodigoCTe.ToString().PadLeft(10, '0')).PadLeft(20, '0');
                    }
                    Task.Factory.StartNew(() => GerarRelatorioRelacao(cargaPedido, nomeCliente, stringConexao, relatorioControleGeracao, dadosRelacaoEntrega, dadosRelacaoEntregaDocumento, dadosRelacaoEntregaMotorista, dadosRelacaoEntregaReboque));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de relação de entrega para gerar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        public async Task<IActionResult> ImprimirRelacaoSeparacaoVolume()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);

                int cargaPedido = 0;
                int.TryParse(Request.Params("CargaPedido"), out cargaPedido);
                if (cargaPedido == 0)
                    return new JsonpResult(false, false, "Favor selecione uma carga para gerar a impressão do relatório.");

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPed = repCargaPedido.BuscarPorCodigo(cargaPedido);

                unitOfWork.Start();
                int codigoCarga = cargaPed.Carga.Codigo;
                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoSeparacaoVolume.RelacaoSeparacaoVolume> dadosRelacaoSeparacaoVolume = repCargaPedidoDocumentoCTe.RelatorioRelacaoSeparacaoVolume(codigoCarga, "", "", "", "");

                if (dadosRelacaoSeparacaoVolume.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga, true);

                    carga.PossuiSeparacao = true;
                    carga.PossuiSeparacaoVolume = true;

                    if (!carga.SeparacaoConferida || carga.SeparacaoConferida != true)
                        carga.SeparacaoConferida = false;

                    new Servicos.Embarcador.GestaoPatio.Expedicao(unitOfWork).Adicionar(carga);

                    repCarga.Atualizar(carga);

                    unitOfWork.CommitChanges();

                    Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R100_RelacaoSeparacaoVolume, TipoServicoMultisoftware);

                    Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                    if (relatorio == null)
                    {
                        relatorio = serRelatorio.BuscarConfiguracaoPadrao(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R100_RelacaoSeparacaoVolume, TipoServicoMultisoftware, "Relatorio de Relação de Separação de Volumes", "Cargas", "RelacaoSeparacaoVolume.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", 0, unitOfWork, false, false);
                    }

                    Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = serRelatorio.AdicionarRelatorioParaGeracao(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                    string stringConexao = _conexao.StringConexao;
                    string nomeCliente = Cliente.NomeFantasia;

                    Task.Factory.StartNew(() => GerarRelatorioRelacaoSeparacaoVolume(codigoCarga, nomeCliente, stringConexao, relatorioControleGeracao, dadosRelacaoSeparacaoVolume));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de relação de volume para gerar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDadosGerais()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int.TryParse(Request.Params("CargaPedido"), out int cargaPedido);

                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);

                decimal valorTotalMercadoria = repCargaPedidoDocumentoCTe.ObterValorTotalMercadoria(cargaPedido);

                var retorno = new
                {
                    ValorTotalMercadoria = valorTotalMercadoria.ToString("n2")
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados gerais.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void GerarRelatorioRelacaoSeparacaoVolume(int codigoCarga, string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoSeparacaoVolume.RelacaoSeparacaoVolume> dadosRelacaoSeparacaoVolume)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            try
            {
                var result = ReportRequest.WithType(ReportType.RelacaoSeparacaoVolume)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("NomeEmpresa", nomeEmpresa)
                    .AddExtraData("DadosRelacaoSeparacaoVolume", dadosRelacaoSeparacaoVolume.ToJson())
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                    serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, result.ErrorMessage);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void GerarRelatorioRelacao(int codigoCargaPedido, string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntrega> dadosRelacaoEntrega,
            IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaDocumento> dadosRelacaoEntregaDocumento,
            IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaMotorista> dadosRelacaoEntregaMotorista,
            IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.RelacaoEntrega.RelacaoEntregaReboque> dadosRelacaoEntregaReboque)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            try
            {
                var result = ReportRequest.WithType(ReportType.RelacaoEntrega)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("NomeEmpresa", nomeEmpresa)
                    .AddExtraData("DadosRelacaoEntrega", dadosRelacaoEntrega.ToJson())
                    .AddExtraData("DadosRelacaoEntregaDocumento", dadosRelacaoEntregaDocumento.ToJson())
                    .AddExtraData("DadosRelacaoEntregaMotorista", dadosRelacaoEntregaMotorista.ToJson())
                    .AddExtraData("DadosRelacaoEntregaReboque", dadosRelacaoEntregaReboque.ToJson())
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                    serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, result.ErrorMessage);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
