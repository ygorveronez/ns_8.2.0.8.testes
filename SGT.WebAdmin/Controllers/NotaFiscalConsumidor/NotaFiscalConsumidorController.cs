using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dominio.Excecoes.Embarcador;
using Dominio.Enumeradores;

namespace SGT.WebAdmin.Controllers.NotaFiscalConsumidor
{
    [CustomAuthorize("NotasFiscaisConsumidores/NotaFiscalConsumidor")]
    public class NotaFiscalConsumidorController : BaseController
    {
		#region Construtores

		public NotaFiscalConsumidorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);

                int.TryParse(Request.Params("NumeroInicial"), out int numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out int numeroFinal);
                int.TryParse(Request.Params("Serie"), out int serie);

                DateTime.TryParse(Request.Params("DataInicial"), out DateTime dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out DateTime dataFinal);

                Enum.TryParse(Request.Params("Status"), out Dominio.Enumeradores.StatusNFe status);

                string chave = Request.Params("Chave");
                string cnpjCpfPessoa = Utilidades.String.OnlyNumbers(Request.Params("CnpjCpfPessoa"));
                string nomePessoa = Request.Params("NomePessoa");

                int empresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nº Nota", "Numero", 9, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Série", "Serie", 5, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 14, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Saída", "DataSaida", 14, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Último Retorno SEFAZ", "UltimoStatusSEFAZ", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Pessoa", "NomePessoa", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Chave", "Chave", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "ValorTotalNota", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Status", false);

                string ordenacao = grid.header[grid.indiceColunaOrdena].data;
                if (ordenacao == "Codigo")
                    ordenacao = "Numero";
                else if (ordenacao == "NomePessoa")
                    ordenacao = "NomeConsumidorFinal";

                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal> listaNotaFiscal = repNotaFiscal.ConsultarNFC(numeroInicial, numeroFinal, serie, dataInicial, dataFinal, empresa, this.Usuario.Empresa.TipoAmbiente, chave, cnpjCpfPessoa, nomePessoa, status, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repNotaFiscal.ContarConsultaNFC(numeroInicial, numeroFinal, serie, dataInicial, dataFinal, empresa, this.Usuario.Empresa.TipoAmbiente, chave, cnpjCpfPessoa, nomePessoa, status));
                var lista = (from p in listaNotaFiscal
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 Serie = p.EmpresaSerie?.Numero ?? 0,
                                 p.DataEmissao,
                                 p.DataSaida,
                                 p.DescricaoStatus,
                                 p.UltimoStatusSEFAZ,
                                 NomePessoa = p.NomeConsumidorFinal,
                                 p.Chave,
                                 ValorTotalNota = p.ValorTotalNota.ToString("n2"),
                                 p.Status
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start(IsolationLevel.ReadUncommitted);
                //unitOfWork.Start();

                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos repNotaFiscalProdutos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNotaFiscal = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal();
                Dominio.Entidades.Empresa empresa = this.Usuario.Empresa;

                if (string.IsNullOrWhiteSpace(empresa.IdTokenNFCe) || Utilidades.String.OnlyNumbers(empresa.IdTokenNFCe).Length != 6)
                    return new JsonpResult(false, true, "Token não está configurado ou a quantidade de números está diferente de 6! Favor ajustar no Cadastro da Empresa");
                else if (string.IsNullOrWhiteSpace(empresa.IdCSCNFCe))
                    return new JsonpResult(false, true, "CSC do Token não está configurado! Favor ajustar no Cadastro da Empresa");

                decimal valorTotal = 0, valorTroco = 0, valorPago = 0;
                decimal.TryParse(Request.Params("ValorTotal"), out valorTotal);
                decimal.TryParse(Request.Params("ValorTroco"), out valorTroco);
                decimal.TryParse(Request.Params("ValorPago"), out valorPago);
                List<int> codigosPedidoVenda = RetornaCodigoPedidoVenda(unitOfWork, Request.GetStringParam("CodigosPedidoVenda"));

                var cnpjCpf = Utilidades.String.OnlyNumbers(Request.Params("CPFConsumidor"));
                if (!string.IsNullOrWhiteSpace(cnpjCpf) && cnpjCpf.Length == 14)
                {
                    if (!Utilidades.Validate.ValidarCNPJ(cnpjCpf))
                        return new JsonpResult(false, "CNPJ do Cliente é inválido!");
                }
                else if (!string.IsNullOrWhiteSpace(cnpjCpf) && cnpjCpf.Length == 11)
                {
                    if (!Utilidades.Validate.ValidarCPF(cnpjCpf))
                        return new JsonpResult(false, "CPF do Cliente é inválido!");
                }
                else if (!string.IsNullOrWhiteSpace(cnpjCpf) && cnpjCpf.Length > 0)
                    return new JsonpResult(false, "CNPJ/CPF do Cliente é inválido!");

                nfe.Atividade = repAtividade.BuscarPrimeiraAtividade();
                nfe.BCCOFINS = 0;
                nfe.BCDeducao = 0;
                nfe.BCICMS = 0;
                nfe.BCICMSST = 0;
                nfe.BCISSQN = 0;
                nfe.BCPIS = 0;
                nfe.Cliente = null;
                nfe.CPFCNPJConsumidorFinal = cnpjCpf;
                nfe.DataEmissao = DateTime.Now;
                nfe.DataPrestacaoServico = DateTime.Now;
                nfe.DataSaida = DateTime.Now;
                nfe.Empresa = empresa;
                nfe.EmpresaSerie = repEmpresaSerie.BuscarPorEmpresaTipo(empresa.Codigo, Dominio.Enumeradores.TipoSerie.NFCe);
                if (nfe.EmpresaSerie == null)
                    nfe.EmpresaSerie = repEmpresaSerie.BuscarPorEmpresaTipo(empresa.Codigo, Dominio.Enumeradores.TipoSerie.NFe);
                nfe.Finalidade = Dominio.Enumeradores.FinalidadeNFe.Normal;
                nfe.ICMSDesonerado = 0;
                nfe.IndicadorPresenca = Dominio.Enumeradores.IndicadorPresencaNFe.Presencial;
                nfe.IndicadorIntermediador = IndicadorIntermediadorNFe.SemIntermediador;
                nfe.InformacaoCompraContrato = "";
                nfe.InformacaoCompraNotaEmpenho = "";
                nfe.InformacaoCompraPedido = "";
                nfe.LocalDespacho = "";
                nfe.LocalEmbarque = "";
                nfe.LocalidadePrestacaoServico = empresa.Localidade;
                nfe.LocalidadeTranspMunicipio = null;
                nfe.ModeloDocumentoFiscal = null;
                nfe.ModeloNotaFiscal = "65";
                nfe.VersaoNFe = nfe.Empresa.VersaoNFe;
                nfe.NaturezaDaOperacao = null;
                nfe.NomeConsumidorFinal = Request.Params("NomeConsumidor");
                nfe.Numero = repNotaFiscal.BuscarUltimoNumero(nfe.Empresa.Codigo, nfe.EmpresaSerie.Numero, empresa.TipoAmbiente, "65") + 1;
                int proximoNumeroSerie = repEmpresaSerie.BuscarProximoNumeroDocumentoPorSerie(nfe.Empresa.Codigo, nfe.EmpresaSerie.Numero, Dominio.Enumeradores.TipoSerie.NFCe);
                if (nfe.Numero < proximoNumeroSerie)
                    nfe.Numero = proximoNumeroSerie;
                nfe.ObservacaoNota = "";
                nfe.ObservacaoTributaria = "";
                nfe.Status = Dominio.Enumeradores.StatusNFe.Emitido;
                nfe.TipoAmbiente = empresa.TipoAmbiente;
                nfe.TipoEmissao = Dominio.Enumeradores.TipoEmissaoNFe.Saida;
                nfe.TipoFrete = Dominio.Enumeradores.ModalidadeFrete.SemFrete;
                nfe.FormaPagamento = Request.GetEnumParam<Dominio.Enumeradores.FormaPagamento>("FormaPagamento");
                nfe.TranspANTTVeiculo = "";
                nfe.TranspCNPJCPF = "";
                nfe.TranspEmail = "";
                nfe.TranspEndereco = "";
                nfe.TranspEspecie = "";
                nfe.TranspIE = "";
                nfe.TranspMarca = "";
                nfe.TranspMunicipio = "";
                nfe.TranspNome = "";
                nfe.Transportadora = null;
                nfe.TranspPesoBruto = 0;
                nfe.TranspPesoLiquido = 0;
                nfe.TranspPlacaVeiculo = "";
                nfe.TranspQuantidade = "";
                nfe.TranspUF = "";
                nfe.TranspUFVeiculo = "";
                nfe.TranspVolume = "";
                nfe.UFEmbarque = "";
                nfe.ValorCOFINS = 0;
                nfe.ValorDesconto = 0;
                nfe.ValorDescontoCondicional = 0;
                nfe.ValorDescontoIncondicional = 0;
                nfe.ValorFCP = 0;
                nfe.ValorFrete = 0;
                nfe.ValorICMS = 0;
                nfe.ValorICMSDestino = 0;
                nfe.ValorICMSRemetente = 0;
                nfe.ValorICMSST = 0;
                nfe.ValorII = 0;
                nfe.ValorImpostoIBPT = 0;
                nfe.ValorIPI = 0;
                nfe.ValorISSQN = 0;
                nfe.ValorOutrasDespesas = 0;
                nfe.ValorOutrasRetencoes = 0;
                nfe.ValorPIS = 0;
                nfe.ValorProdutos = 0;
                nfe.ValorRetencaoISS = 0;
                nfe.ValorSeguro = 0;
                nfe.ValorServicos = 0;
                nfe.ValorTotalNota = valorTotal;
                nfe.ValorTroco = valorTroco;
                nfe.Veiculo = null;
                nfe.Usuario = this.Usuario;

                repNotaFiscal.Inserir(nfe, Auditado);
                unitOfWork.CommitChanges();

                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = empresa.TipoAmbiente;

                decimal valorIBPT = 0, valorCreditoICMS = 0, valorICMS = 0, baseICMS = 0, basePIS = 0, valorPIS = 0, baseCOFINS = 0, valorCOFINS = 0;
                SalvarListaItens(nfe, unitOfWork, out valorIBPT, out valorCreditoICMS, out valorICMS, out baseICMS, out valorPIS, out valorCOFINS, out basePIS, out baseCOFINS);
                SalvarListaParcelas(nfe, unitOfWork);

                int empresaPai = 0;
                if (empresa.EmpresaPai != null)
                    empresaPai = empresa.EmpresaPai.Codigo;

                //PREENCHE OBSERVAÇÕES DA NF
                if (empresa.OptanteSimplesNacional && !string.IsNullOrWhiteSpace(empresa.ObservacaoSimplesNacional))
                {
                    var observacaoEmpresa = empresa.ObservacaoSimplesNacional;
                    if (valorCreditoICMS > 0)
                        observacaoEmpresa = observacaoEmpresa.Replace("#ValorCreditoICMS", "R$ " + valorCreditoICMS.ToString("n2"));
                    else if (observacaoEmpresa.Contains("#ValorCreditoICMS"))
                        observacaoEmpresa = observacaoEmpresa.Replace("#ValorCreditoICMS", string.Empty);

                    if (empresa.AliquotaICMSSimples > 0)
                        observacaoEmpresa = observacaoEmpresa.Replace("#AliquotaSimples", empresa.AliquotaICMSSimples.ToString("n2") + "%");
                    else if (observacaoEmpresa.Contains("#AliquotaSimples"))
                        observacaoEmpresa = observacaoEmpresa.Replace("#AliquotaSimples", string.Empty);

                    nfe.ObservacaoNota += observacaoEmpresa;
                }

                decimal valorIPBTNacional = ValorImpostoIBPT(empresaPai, nfe, unitOfWork, 0, empresa.Codigo);
                decimal valorIPBTEstadual = ValorImpostoIBPT(empresaPai, nfe, unitOfWork, 1, empresa.Codigo);
                decimal valorIPBTMunicipal = ValorImpostoIBPT(empresaPai, nfe, unitOfWork, 2, empresa.Codigo);
                decimal valorProdutos = ValorProdutos(nfe, unitOfWork);
                decimal valorServicos = ValorServicos(nfe, unitOfWork);

                nfe.ValorICMS = valorICMS;
                nfe.BCICMS = baseICMS;
                nfe.ValorPIS = valorPIS;
                nfe.ValorCOFINS = valorCOFINS;
                nfe.BCPIS = basePIS;
                nfe.BCCOFINS = baseCOFINS;
                nfe.ValorImpostoIBPT = valorIBPT;
                nfe.ValorServicos = valorServicos;
                nfe.ValorProdutos = valorProdutos;

                if (valorIBPT > 0)
                    nfe.ObservacaoNota += " Valor aproximado dos tributos com base na Lei 12.741/2012 - R$ " + valorIBPT.ToString("n2") + " (" + ((valorIBPT * 100) / nfe.ValorTotalNota).ToString("n2") + " %) - Fonte: IBPT";
                if (valorIPBTNacional > 0)
                    nfe.ObservacaoNota += " - Nacional R$ " + valorIPBTNacional.ToString("n2");
                if (valorIPBTEstadual > 0)
                    nfe.ObservacaoNota += " - Estadual R$ " + valorIPBTEstadual.ToString("n2");
                if (valorIPBTMunicipal > 0)
                    nfe.ObservacaoNota += " - Municipal R$ " + valorIPBTMunicipal.ToString("n2");
                nfe.ObservacaoNota = nfe.ObservacaoNota.Trim();

                repNotaFiscal.Atualizar(nfe);

                unitOfWork.CommitChanges();

                if (codigosPedidoVenda.Count > 0)
                {
                    if (!SalvarVinculoNotaPedidoVenda(nfe, codigosPedidoVenda, unitOfWork))
                        return new JsonpResult(false, "Problema ao salvar o Vínculo do Pedido com a Nota! Favor contatar o suporte imediatamente.");
                }

                var dynRetorno = new
                {
                    nfe.Codigo
                };
                return new JsonpResult(dynRetorno);
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

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarNFCe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start(IsolationLevel.ReadUncommitted);

                int codigoNFe = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);

                if (nfe == null)
                    throw new ControllerException("A NFC-e informada não foi localizada!");

                if (nfe.Status == StatusNFe.Autorizado || nfe.Status == StatusNFe.Inutilizado || nfe.Status == StatusNFe.Cancelado || nfe.Status == StatusNFe.Denegado)
                    throw new ControllerException($"A NFC-e está com o Status { nfe.Status.ObterDescricao() }! Não sendo mais possível emitir.");

                unitOfWork.CommitChanges();

                string retorno = EmitirNFCe(codigoNFe, unitOfWork, Request.Params("Relatorio"), Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath(), this.Usuario);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfe, null, "Enviou NFC-e.", unitOfWork);

                if (string.IsNullOrWhiteSpace(retorno))
                {
                    nfe = repNFe.BuscarPorCodigo(codigoNFe);
                    var dynRetorno = new
                    {
                        nfe.Codigo,
                        nfe.Chave,
                        nfe.Status
                    };
                    return new JsonpResult(dynRetorno);
                }
                else
                    return new JsonpResult(false, true, retorno);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir o NFC-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadDANFENFCe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoNFe = Request.GetIntParam("Codigo");
                string chave = Request.GetStringParam("Chave");

                Zeus.Embarcador.ZeusNFe.Zeus zeus = new Zeus.Embarcador.ZeusNFe.Zeus();

                byte[] arquivoPdf = zeus.ObterPdfDANFCe(codigoNFe, chave, unitOfWork, out string nomeArquivoPdf, out string retorno);
                if (!string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(false, true, retorno);

                return Arquivo(arquivoPdf, "application/pdf", nomeArquivoPdf);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do PDF.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> InutilizarNFCe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoNFe = Request.GetIntParam("Codigo");

                Zeus.Embarcador.ZeusNFe.Zeus z = new Zeus.Embarcador.ZeusNFe.Zeus();
                string retorno = z.InutilizarNFe(codigoNFe, unitOfWork);

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfe, null, "Inutilizou o NFC-e.", unitOfWork);

                unitOfWork.CommitChanges();

                if (string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(true);
                else
                    return new JsonpResult(false, true, retorno);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar inutilizar a nota selecionada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarEmailNFCe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoNFe = Request.GetIntParam("Codigo");

                Zeus.Embarcador.ZeusNFe.Zeus zeus = new Zeus.Embarcador.ZeusNFe.Zeus();

                string retorno = zeus.EnviarEmailNFCe(codigoNFe, unitOfWork);
                if (!string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(false, true, retorno);

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfe, null, "Enviou E-mail NFC-e.", unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar enviar e-mail da NFC-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> CarregarPedidoVendaPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                List<int> codigos = new List<int>();
                codigos = RetornaCodigosPedidos(unitOfWork);

                Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);
                List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> pedidosVenda = repPedidoVenda.BuscarPorCodigos(codigos);

                if (pedidosVenda == null || pedidosVenda.Count <= 0)
                    return new JsonpResult(false, "Pedido venda não encontrado.");

                var retorno = new
                {
                    NFCe = ObterDetalhesPedidoVenda(pedidosVenda[0]),
                    ProdutosServicos = ObterItensPedidoVenda(pedidosVenda, codigos, unitOfWork),
                    Totalizador = ObterTotalizadorPedidoVenda(pedidosVenda),
                };

                unitOfWork.CommitChanges();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao carregar os dados do pedido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string EmitirNFCe(int codigoNFe, Repositorio.UnitOfWork unitOfWork, string relatorio, string caminhoRelatoriosEmbarcador, Dominio.Entidades.Usuario usuario)
        {
            Zeus.Embarcador.ZeusNFe.Zeus z = new Zeus.Embarcador.ZeusNFe.Zeus();
			string urlBase = _conexao.ObterHost;
			return z.CriarEnviarNFe(codigoNFe, unitOfWork, TipoServicoMultisoftware, relatorio, caminhoRelatoriosEmbarcador, usuario, "65", 1, true, true, null, urlBase);
        }

        private decimal ValorImpostoIBPT(int empresaPai, Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, Repositorio.UnitOfWork unidadeDeTrabalho, int tipoImposto, int codigoEmpresa)
        {
            Servicos.Embarcador.NotaFiscal.NotaFiscalProduto serNotaFiscalProduto = new Servicos.Embarcador.NotaFiscal.NotaFiscalProduto(unidadeDeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeDeTrabalho);

            decimal valorImposto = 0;
            dynamic listaItens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaItens"));
            if (listaItens != null)
            {
                foreach (var item in listaItens)
                {
                    int codigoProduto = 0;
                    int.TryParse((string)item.Item.CodigoProduto, out codigoProduto);

                    decimal valorTotal = 0;
                    decimal.TryParse((string)item.Item.ValorTotal, out valorTotal);

                    if (codigoProduto > 0)
                    {
                        Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(codigoProduto);
                        string ncm = produto.CodigoNCM;
                        valorImposto += serNotaFiscalProduto.RetornaValorIBPT(empresaPai, codigoEmpresa, ncm, valorTotal, unidadeDeTrabalho, tipoImposto);
                    }
                }
            }
            return valorImposto;
        }

        private decimal ValorServicos(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            decimal valorTotalServicos = 0;
            dynamic listaItens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaItens"));
            if (listaItens != null)
            {
                foreach (var item in listaItens)
                {
                    int codigoServico = 0;
                    int.TryParse((string)item.Item.CodigoServico, out codigoServico);

                    decimal valorTotal = 0;
                    decimal.TryParse((string)item.Item.ValorTotal, out valorTotal);

                    if (codigoServico > 0)
                        valorTotalServicos += valorTotal;
                }
            }
            return valorTotalServicos;
        }

        private decimal ValorProdutos(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            decimal valorTotalProdutos = 0;
            dynamic listaItens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaItens"));
            if (listaItens != null)
            {
                foreach (var item in listaItens)
                {
                    int codigoProduto = 0;
                    int.TryParse((string)item.Item.CodigoProduto, out codigoProduto);

                    decimal valorTotal = 0;
                    decimal.TryParse((string)item.Item.ValorTotal, out valorTotal);

                    if (codigoProduto > 0)
                        valorTotalProdutos += valorTotal;
                }
            }
            return valorTotalProdutos;
        }

        private void SalvarListaItens(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, Repositorio.UnitOfWork unidadeDeTrabalho, out decimal valorIBPT, out decimal valorCreditoICMS, out decimal valorICMS, out decimal baseICMS, out decimal valorPIS, out decimal valorCOFINS, out decimal basePIS, out decimal baseCOFINS)
        {
            valorIBPT = 0;
            valorCreditoICMS = 0;
            valorICMS = 0;
            baseICMS = 0;
            valorPIS = 0;
            valorCOFINS = 0;
            basePIS = 0;
            baseCOFINS = 0;

            Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos repNotaFiscalProdutos = new Repositorio.Embarcador.NotaFiscal.NotaFiscalProdutos(unidadeDeTrabalho);
            Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unidadeDeTrabalho);
            Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens repGrupoImpostoItens = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Produto repProduto = new Repositorio.Produto(unidadeDeTrabalho);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unidadeDeTrabalho);

            Servicos.Embarcador.NotaFiscal.NotaFiscalProduto serNotaFiscalProduto = new Servicos.Embarcador.NotaFiscal.NotaFiscalProduto(unidadeDeTrabalho);

            Dominio.Entidades.Produto produto = null;
            Dominio.Entidades.CFOP cfop = null;
            Dominio.Entidades.Embarcador.NotaFiscal.Servico servico = null;
            Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens imposto = null;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? cstICMS = null;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? cstCOFINS = null;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? cstPIS = null;
            decimal aliquotaICMS = 0;
            decimal aliquotaPIS = 0;
            decimal aliquotaCOFINS = 0;
            int cfopServico = 0;
            int cfopProduto = 0;
            int cfopUtilizar = 0;
            int empresaPai = 0;
            if (notaFiscal.Empresa.EmpresaPai != null)
                empresaPai = notaFiscal.Empresa.EmpresaPai.Codigo;

            dynamic listaItens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaItens"));
            if (listaItens != null)
            {
                foreach (var item in listaItens)
                {
                    int codigoProduto = 0, codigoServico = 0;
                    int.TryParse((string)item.Item.CodigoProduto, out codigoProduto);
                    int.TryParse((string)item.Item.CodigoServico, out codigoServico);

                    decimal valorTotal = 0, valorUnitario = 0, quantidade = 0;

                    decimal.TryParse((string)item.Item.ValorTotal, out valorTotal);
                    decimal.TryParse((string)item.Item.ValorUnitario, out valorUnitario);
                    decimal.TryParse((string)item.Item.Quantidade, out quantidade);

                    if (codigoProduto > 0)
                    {
                        produto = repProduto.BuscarPorCodigo(codigoProduto);
                        if (produto.GrupoImposto != null)
                            imposto = repGrupoImpostoItens.BuscarPorEstadosAtividade(this.Empresa.Localidade.Estado.Sigla, this.Empresa.Localidade.Estado.Sigla, 7, produto.GrupoImposto.Codigo);
                    }
                    else
                        servico = repServico.BuscarPorCodigo(codigoServico);

                    cfop = repCFOP.BuscarPorCFOPEmpresa(5101, notaFiscal.Empresa.Codigo);
                    Dominio.Entidades.Cliente cliente = null;
                    if (!string.IsNullOrWhiteSpace(notaFiscal.CPFCNPJConsumidorFinal))
                        cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(notaFiscal.CPFCNPJConsumidorFinal));

                    cfopProduto = 0;
                    cfopServico = 0;
                    cfopUtilizar = cfop.CodigoCFOP;
                    if (servico != null)
                    {
                        if (cliente != null)
                            if (this.Usuario.Empresa.Localidade.Estado.Sigla == cliente.Localidade.Estado.Sigla)
                                cfopServico = servico.CFOPVendaDentroEstado != null ? servico.CFOPVendaDentroEstado.CodigoCFOP : 0;
                            else
                                cfopServico = servico.CFOPVendaForaEstado != null ? servico.CFOPVendaForaEstado.CodigoCFOP : 0;
                    }
                    else if (imposto != null)
                        cfopProduto = !string.IsNullOrWhiteSpace(imposto.CFOPVenda) ? repCFOP.BuscarPorNumero(Convert.ToInt16(imposto.CFOPVenda)).CodigoCFOP : 0;

                    if (cfopProduto > 0)
                        cfopUtilizar = cfopProduto;
                    else if (cfopServico > 0)
                        cfopUtilizar = cfopServico;

                    if (cfopUtilizar > 0)
                        cfop = repCFOP.BuscarPorCFOPEmpresa(cfopUtilizar, notaFiscal.Empresa.Codigo);

                    cstICMS = servico != null ? 0 : cfop != null && cfop.CSTICMS != null ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS)cfop.CSTICMS : imposto != null && imposto.CSTICMSVenda != null ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS)imposto.CSTICMSVenda : 0;
                    if (cstICMS == null || (int)cstICMS == 0)
                        cstICMS = notaFiscal.Empresa.OptanteSimplesNacional && !notaFiscal.Empresa.OptanteSimplesNacionalComExcessoReceitaBruta ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN400 : Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST41;

                    cstCOFINS = cfop != null && cfop.CSTCOFINS != null ? cfop.CSTCOFINS : imposto != null && imposto.CSTCOFINSVenda != null ? imposto.CSTCOFINSVenda : 0;
                    if (cstCOFINS == null || (int)cstCOFINS == 0)
                        cstCOFINS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01;

                    cstPIS = cfop != null && cfop.CSTPIS != null ? cfop.CSTPIS : imposto != null && imposto.CSTPISVenda != null ? imposto.CSTPISVenda : 0;
                    if (cstPIS == null || (int)cstPIS == 0)
                        cstPIS = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01;

                    aliquotaICMS = cfop != null && cfop.AliquotaICMSInterna > 0 ? cfop.AliquotaICMSInterna : imposto != null && imposto.AliquotaICMSInternaVenda > 0 ? imposto.AliquotaICMSInternaVenda : 0;
                    aliquotaCOFINS = cfop != null && cfop.AliquotaCOFINS > 0 ? cfop.AliquotaCOFINS : imposto != null && imposto.AliquotaCOFINSVenda > 0 ? imposto.AliquotaCOFINSVenda : 0;
                    aliquotaPIS = cfop != null && cfop.AliquotaPIS > 0 ? cfop.AliquotaPIS : imposto != null && imposto.AliquotaPISVenda > 0 ? imposto.AliquotaPISVenda : 0;

                    decimal baseICMSEfetivo = 0, aliquotaICMSEfetivo = 0, valorICMSEfetivo = 0;
                    if (imposto != null && (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500 ||
                        cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60))
                    {
                        //Código Atividade 7 - Não Contribuinte
                        baseICMSEfetivo = valorTotal;
                        aliquotaICMSEfetivo = imposto.AliquotaICMSInternaVenda;

                        if (cliente != null && this.Usuario.Empresa?.Localidade.Estado.Sigla != cliente.Localidade.Estado.Sigla)
                            aliquotaICMSEfetivo = imposto.AliquotaICMSInterestadualVenda;

                        if (baseICMSEfetivo > 0 && aliquotaICMSEfetivo > 0)
                            valorICMSEfetivo = Math.Round(baseICMSEfetivo * (aliquotaICMSEfetivo / 100), 2);
                        else
                        {
                            baseICMSEfetivo = 0;
                            aliquotaICMSEfetivo = 0;
                        }
                    }

                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos prod = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalProdutos();
                    prod.AliquotaCOFINS = aliquotaCOFINS;
                    prod.AliquotaFCP = 0;
                    prod.AliquotaICMS = aliquotaICMS;
                    prod.AliquotaICMSDestino = 0;
                    prod.AliquotaICMSInterno = 0;
                    prod.AliquotaICMSOperacao = 0;
                    prod.AliquotaICMSST = 0;
                    prod.AliquotaICMSSTInterestadual = 0;
                    prod.AliquotaIPI = 0;
                    prod.AliquotaISS = 0;
                    prod.AliquotaPIS = aliquotaPIS;
                    prod.BaseII = 0;
                    prod.BaseISS = 0;
                    prod.BCCOFINS = 0;
                    prod.BCDeducao = 0;
                    prod.BCICMS = 0;
                    prod.BCICMSDestino = 0;
                    prod.BCICMSST = 0;
                    prod.BCIPI = 0;
                    prod.BCPIS = 0;
                    prod.CFOP = repCFOP.BuscarPorCFOPEmpresa(cfopUtilizar, notaFiscal.Empresa.Codigo);// repCFOP.BuscarPorCFOPEmpresa(5101, notaFiscal.Empresa.Codigo);
                    prod.CNPJAdquirente = "";
                    prod.CSTCOFINS = cstCOFINS;// Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01;
                    prod.CSTICMS = cstICMS;// notaFiscal.Empresa.OptanteSimplesNacional ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN400 : Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST41;
                    prod.CSTIPI = null;
                    prod.CSTPIS = cstPIS;// Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS.CST01;
                    prod.DataDesembaraco = null;
                    prod.DataRegistroImportacao = null;
                    prod.DescontoCondicional = 0;
                    prod.DescontoIncondicional = 0;
                    prod.LocalDesembaraco = "";
                    prod.MVAICMSST = 0;
                    prod.NotaFiscal = notaFiscal;
                    prod.NumeroDocImportacao = "";
                    prod.NumeroItemOrdemCompra = "";
                    prod.NumeroOrdemCompra = "";
                    prod.OutrasRetencoes = 0;
                    prod.PercentualPartilha = 0;
                    prod.ProcessoJudicial = "";
                    prod.Produto = codigoProduto > 0 ? repProduto.BuscarPorCodigo(codigoProduto) : null;
                    prod.Quantidade = quantidade;
                    prod.ReducaoBCCOFINS = 0;
                    prod.ReducaoBCICMS = 0;
                    prod.ReducaoBCICMSST = 0;
                    prod.ReducaoBCIPI = 0;
                    prod.ReducaoBCPIS = 0;
                    prod.RetencaoISS = 0;
                    prod.Servico = codigoServico > 0 ? repServico.BuscarPorCodigo(codigoServico) : null;
                    prod.UFDesembaraco = "";
                    prod.ValorCOFINS = 0;
                    prod.ValorDesconto = 0;
                    prod.ValorDespesaII = 0;
                    prod.ValorFCP = 0;
                    prod.ValorFrete = 0;
                    prod.ValorFreteMarinho = 0;
                    prod.ValorICMS = 0;
                    prod.ValorICMSDesonerado = 0;
                    prod.ValorICMSDestino = 0;
                    prod.ValorICMSDiferido = 0;
                    prod.ValorICMSOperacao = 0;
                    prod.ValorICMSRemetente = 0;
                    prod.ValorICMSST = 0;
                    prod.ValorII = 0;
                    if (prod.Produto != null)
                        if (!string.IsNullOrWhiteSpace(prod.Produto.CodigoNCM))
                            prod.ValorImpostoIBPT = serNotaFiscalProduto.RetornaValorIBPT(empresaPai, notaFiscal.Empresa.Codigo, prod.Produto.CodigoNCM, valorTotal, unidadeDeTrabalho, 3);
                        else
                            prod.ValorImpostoIBPT = 0;
                    else
                        prod.ValorImpostoIBPT = 0;
                    prod.ValorImpostoIBPT = Math.Round(prod.ValorImpostoIBPT, 2);
                    valorIBPT += prod.ValorImpostoIBPT;

                    prod.ValorIOFII = 0;
                    prod.ValorIPI = 0;
                    prod.ValorISS = 0;
                    prod.ValorOutrasDespesas = 0;
                    prod.ValorPIS = 0;
                    prod.ValorSeguro = 0;
                    prod.ValorTotal = valorTotal;
                    prod.ValorUnitario = valorUnitario;
                    prod.DescricaoItem = prod.Produto != null ? prod.Produto.DescricaoNotaFiscal : prod.Servico.DescricaoNFE;
                    prod.BCICMSEfetivo = baseICMSEfetivo;
                    prod.AliquotaICMSEfetivo = aliquotaICMSEfetivo;
                    prod.ReducaoBCICMSEfetivo = 0;
                    prod.ValorICMSEfetivo = valorICMSEfetivo;

                    prod.CodigoANP = prod.Produto?.CodigoANP ?? null;
                    prod.PercentualGLP = prod.Produto?.PercentualGLP ?? null;
                    prod.PercentualGNN = prod.Produto?.PercentualGNN ?? null;
                    prod.PercentualGNI = prod.Produto?.PercentualGNI ?? null;
                    prod.PercentualOrigemComb = prod.Produto?.PercentualOrigemCombustivel ?? null;                    
                    prod.PercentualMisturaBiodiesel = prod.Produto?.PercentualMisturaBiodiesel ?? null;
                    prod.ValorPartidaANP = prod.Produto?.ValorPartidaANP ?? null;
                    prod.OrigemMercadoria = prod.Produto?.OrigemMercadoria ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemMercadoria.Origem0;

                    if (notaFiscal.Empresa.OptanteSimplesNacional && notaFiscal.Empresa.AliquotaICMSSimples > 0 && (prod.CSTICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101 ||
                        prod.CSTICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201 || prod.CSTICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900))
                    {
                        prod.AliquotaICMSSimples = notaFiscal.Empresa.AliquotaICMSSimples;
                        prod.ValorICMSSimples = Math.Round((prod.AliquotaICMSSimples / 100) * prod.ValorTotal, 2);
                        valorCreditoICMS += prod.ValorICMSSimples;
                    }
                    else
                    {
                        prod.AliquotaICMSSimples = 0;
                        prod.ValorICMSSimples = 0;
                    }

                    if (prod.AliquotaICMS > 0 && valorTotal > 0)
                    {
                        prod.BCICMS = valorTotal;
                        prod.ValorICMS = (prod.BCICMS * (prod.AliquotaICMS / 100));
                    }
                    if (prod.AliquotaPIS > 0 && valorTotal > 0)
                    {
                        prod.BCPIS = valorTotal - prod.ValorICMS;
                        prod.ValorPIS = (prod.BCPIS * (prod.AliquotaPIS / 100));
                    }
                    if (prod.AliquotaCOFINS > 0 && valorTotal > 0)
                    {
                        prod.BCCOFINS = valorTotal - prod.ValorICMS;
                        prod.ValorCOFINS = (prod.BCCOFINS * (prod.AliquotaCOFINS / 100));
                    }

                    valorICMS += prod.ValorICMS;
                    baseICMS += prod.BCICMS;
                    valorPIS += prod.ValorPIS;
                    valorCOFINS += prod.ValorCOFINS;
                    baseCOFINS += prod.BCCOFINS;
                    basePIS += prod.BCPIS;

                    repNotaFiscalProdutos.Inserir(prod);
                }
            }
        }

        private void SalvarListaParcelas(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.NotaFiscal.NotaFiscalParcela repNotaFiscalParcela = new Repositorio.Embarcador.NotaFiscal.NotaFiscalParcela(unidadeDeTrabalho);

            dynamic listaParcelas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaParcelas"));
            if (listaParcelas != null)
            {
                foreach (var parcela in listaParcelas)
                {
                    int sequencia = 0;
                    int.TryParse((string)parcela.Parcela.Sequencia, out sequencia);

                    decimal valor = 0;
                    decimal.TryParse((string)parcela.Parcela.Valor, out valor);

                    DateTime dataVencimento = new DateTime();
                    DateTime.TryParseExact((string)parcela.Parcela.DataVencimento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimento);

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo formaTitulo;
                    Enum.TryParse((string)parcela.Parcela.FormaTitulo, out formaTitulo);

                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela parc = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalParcela();
                    parc.Acrescimo = 0;
                    parc.DataEmissao = DateTime.Now;
                    parc.DataVencimento = dataVencimento;
                    parc.Desconto = 0;
                    parc.NotaFiscal = notaFiscal;
                    parc.Sequencia = sequencia;
                    parc.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFaturaParcela.EmAberto;
                    parc.Valor = valor;
                    parc.Forma = formaTitulo;

                    repNotaFiscalParcela.Inserir(parc);
                }
            }
        }

        private List<int> RetornaCodigosPedidos(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<int> listaCodigos = new List<int>();
            if (!string.IsNullOrWhiteSpace(Request.Params("Codigos")))
            {
                dynamic listaPedidos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Codigos"));
                if (listaPedidos != null)
                {
                    foreach (var pedido in listaPedidos)
                    {
                        listaCodigos.Add(int.Parse(Utilidades.String.OnlyNumbers((string)pedido.Codigo)));
                    }
                }
            }
            return listaCodigos;
        }

        private object ObterDetalhesPedidoVenda(Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda)
        {
            var retorno = new
            {
                Pessoa = new
                {
                    CNPJCPFCliente = pedidoVenda.Cliente.CPF_CNPJ_Formatado,
                    NomeCliente = pedidoVenda.Cliente.Nome
                },
            };

            return retorno;
        }

        private object ObterItensPedidoVenda(List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> pedidosVenda, List<int> codigosPedido, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PedidoVenda.PedidoVendaItens repPedidoVendaItens = new Repositorio.Embarcador.PedidoVenda.PedidoVendaItens(unitOfWork);
            List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVendaItens> itens = repPedidoVendaItens.BuscarPorPedidos(codigosPedido);

            Dominio.Entidades.Empresa empresa = this.Usuario.Empresa;

            List<object> retorno = new List<object>();
            for (int i = 0; i < itens.Count; i++)
            {
                var obj = itens[i];

                retorno.Add(new
                {
                    CodigoProduto = obj.Produto != null ? obj.Produto.Codigo : 0,
                    CodigoServico = obj.Servico != null ? obj.Servico.Codigo : 0,
                    Descricao = obj.Produto != null ? obj.Produto.Descricao : obj.Servico != null ? obj.Servico.Descricao : string.Empty,
                    Qtd = obj.Quantidade.ToString("n" + empresa.CasasQuantidadeProdutoNFe.ToString()),
                    ValorUnitario = obj.ValorUnitario.ToString("n" + empresa.CasasValorProdutoNFe.ToString()),
                    ValorTotal = obj.ValorTotal.ToString("n2"),
                });
            }

            return retorno;
        }

        private object ObterTotalizadorPedidoVenda(List<Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda> pedidosVenda)
        {
            var retorno = new
            {
                ValorTotal = pedidosVenda.Sum(obj => obj.ValorTotal).ToString("n2"),
            };

            return retorno;
        }

        private bool SalvarVinculoNotaPedidoVenda(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe, List<int> codigosPedidoVenda, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                for (int i = 0; i < codigosPedidoVenda.Count; i++)
                {
                    int codigoPedidoVenda = codigosPedidoVenda[i];

                    Repositorio.Embarcador.NotaFiscal.NotaFiscalPedido repNFePedido = new Repositorio.Embarcador.NotaFiscal.NotaFiscalPedido(unitOfWork);
                    Repositorio.Embarcador.PedidoVenda.PedidoVenda repPedidoVenda = new Repositorio.Embarcador.PedidoVenda.PedidoVenda(unitOfWork);

                    Dominio.Entidades.Embarcador.PedidoVenda.PedidoVenda pedidoVenda = repPedidoVenda.BuscarPorCodigo(codigoPedidoVenda);
                    Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalPedido notaPedido = new Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalPedido();

                    notaPedido.NotaFiscal = nfe;
                    notaPedido.PedidoVenda = pedidoVenda;
                    repNFePedido.Inserir(notaPedido);
                }

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private List<int> RetornaCodigoPedidoVenda(Repositorio.UnitOfWork unidadeDeTrabalho, string codigoPedidoVenda)
        {
            List<int> listaCodigos = new List<int>();
            if (!string.IsNullOrWhiteSpace(codigoPedidoVenda))
            {
                dynamic listaPedidos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(codigoPedidoVenda);
                if (listaPedidos != null)
                {
                    foreach (var pedido in listaPedidos)
                    {
                        listaCodigos.Add(int.Parse(Utilidades.String.OnlyNumbers((string)pedido.Codigo)));
                    }
                }
            }
            return listaCodigos;
        }

        #endregion
    }
}
