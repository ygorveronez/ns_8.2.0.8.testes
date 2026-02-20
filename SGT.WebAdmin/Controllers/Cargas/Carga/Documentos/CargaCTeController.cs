using Dominio.Entidades.Embarcador.Configuracao;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Carga.OcultarInformacoesCarga;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.AspNetCore.Mvc;
using Servicos.Extensions;
using SGTAdmin.Controllers;
using System.Text;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Documentos
{
    [CustomAuthorize(new string[] { "BuscarPermissoesCargaCTes", "BuscarCTeCarga", "ConsultarCargaCTe", "DownloadDacte", "DownloadDacteComplemento", "DownloadXML", "DownloadLoteDACTE", "DownloadLoteDANFE", "DownloadLoteXML" }, "Cargas/ControleEntrega", "Logistica/Monitoramento", "Cargas/Carga", "Logistica/JanelaCarregamento", "GestaoPatio/FluxoPatio", "Ocorrencias/AutorizacaoOcorrencia")]
    public class CargaCTeController : BaseController
    {
        #region Construtores

        public CargaCTeController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarCTesParaEmissaoMDFe(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoOrigem", false);
                grid.AdicionarCabecalho("Origem", false);
                grid.AdicionarCabecalho("CodigoDestino", false);
                grid.AdicionarCabecalho("CNPJSeguradora", false);
                grid.AdicionarCabecalho("NomeSeguradora", false);
                grid.AdicionarCabecalho("NumeroApolice", false);
                grid.AdicionarCabecalho("NumeroAverbacao", false);
                grid.AdicionarCabecalho("TipoSeguro", false);
                grid.AdicionarCabecalho("CNPJEmpresa", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);

                grid.AdicionarCabecalho("Carga", "Carga", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Rota de Frete", "RotaFrete", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("CT-e", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Notas", "Notas", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left, true);

                Models.Grid.EditableCell editableValorFrete = null; //new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 9);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true, false, false, false, true, editableValorFrete);

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaCTeCargaMDFeManual filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                int countCargaCTes = await repCargaCTe.ContarConsultaCTesParaEmissaoMDFeAsync(filtrosPesquisa, parametrosConsulta);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

                string cnpjSeguradoraPadrao = string.Empty;
                string nomeSeguradoraPadrao = string.Empty;
                string apoliceSeguroPadrao = string.Empty;

                if (countCargaCTes > 0)
                {
                    cargaCTes = await repCargaCTe.ConsultarCTesParaEmissaoMDFeAsync(filtrosPesquisa, parametrosConsulta);

                    if (cargaCTes != null && cargaCTes.Count > 0)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTes.FirstOrDefault().CTe;

                        cnpjSeguradoraPadrao = ObterCnpjSeguradoraPadrao(cte);

                        nomeSeguradoraPadrao = ObterNomeSeguradoraPadrao(cte);

                        apoliceSeguroPadrao = ObterApoliceSeguroPadrao(cte);
                    }
                }

                grid.setarQuantidadeTotal(countCargaCTes);

                List<(string DescricaoRotaFrete, int CodigoCargaCTe)> descricaoRotaFretes = await repCargaCTe.BuscarDescricaoRotaFretePorCargaCTeAsync(cargaCTes.Select(o => o.Codigo).ToList());

                var lista = (from obj in cargaCTes
                             select new
                             {
                                 obj.Codigo,
                                 CodigoOrigem = obj.CTe.LocalidadeInicioPrestacao.Codigo,
                                 Origem = obj.CTe.LocalidadeInicioPrestacao.DescricaoCidadeEstado,
                                 CodigoDestino = obj.CTe.LocalidadeTerminoPrestacao.Codigo,
                                 CNPJSeguradora = obj.CTe.Seguros != null && obj.CTe.Seguros.Count > 0 && !string.IsNullOrWhiteSpace(obj.CTe.Seguros[0].CNPJSeguradora) ? obj.CTe.Seguros[0].CNPJSeguradora : cnpjSeguradoraPadrao,
                                 NomeSeguradora = obj.CTe.Seguros != null && obj.CTe.Seguros.Count > 0 && !string.IsNullOrWhiteSpace(obj.CTe.Seguros[0].NomeSeguradora) ? obj.CTe.Seguros[0].NomeSeguradora : nomeSeguradoraPadrao,
                                 NumeroApolice = obj.CTe.Seguros != null && obj.CTe.Seguros.Count > 0 && !string.IsNullOrWhiteSpace(obj.CTe.Seguros[0].NumeroApolice) ? obj.CTe.Seguros[0].NumeroApolice : apoliceSeguroPadrao,
                                 NumeroAverbacao = obj.CTe.Seguros != null && obj.CTe.Seguros.Count > 0 ? !string.IsNullOrWhiteSpace(obj.CTe.Seguros[0].NumeroAverbacao) ? obj.CTe.Seguros[0].NumeroAverbacao : svcMDFe.BuscarAverbacaoCTe(obj.CTe.Codigo, obj.CTe.Empresa.Codigo, unitOfWork) : svcMDFe.BuscarAverbacaoCTe(obj.CTe.Codigo, obj.CTe.Empresa.Codigo, unitOfWork),
                                 TipoSeguro = obj.CTe.Seguros != null && obj.CTe.Seguros.Count > 0 ? obj.CTe.Seguros[0].Tipo == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente,
                                 CNPJEmpresa = obj.CTe.Empresa.CNPJ_SemFormato,
                                 CodigoCTE = obj.CTe.Codigo,
                                 Carga = obj.Carga.CodigoCargaEmbarcador,
                                 Numero = obj.CTe.Numero + " - " + obj.CTe.Serie.Numero,
                                 CodigoEmpresa = obj.CTe.Empresa.Codigo,
                                 Serie = obj.CTe.Serie.Numero,
                                 Notas = string.Join(", ", obj.CTe.XMLNotaFiscais.Select(o => o.Numero.ToString())),
                                 Remetente = obj.CTe?.Remetente?.Nome + "(" + obj.CTe?.Remetente?.CPF_CNPJ_Formatado + ")",
                                 Destinatario = obj.CTe?.Destinatario?.Cliente?.Descricao, //obj.CTe.Destinatario.Nome + "(" + obj.CTe.Destinatario.CPF_CNPJ_Formatado + ")",
                                 Destino = obj.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                 ValorFrete = obj.CTe.ValorAReceber.ToString("n2"),
                                 RotaFrete = descricaoRotaFretes.FirstOrDefault(x => x.CodigoCargaCTe == obj.Codigo).DescricaoRotaFrete ?? string.Empty,

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
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarCTesParaEmissaoMDFeMultiCTe(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoOrigem", false);
                grid.AdicionarCabecalho("Origem", false);
                grid.AdicionarCabecalho("CodigoDestino", false);
                grid.AdicionarCabecalho("CNPJSeguradora", false);
                grid.AdicionarCabecalho("NomeSeguradora", false);
                grid.AdicionarCabecalho("NumeroApolice", false);
                grid.AdicionarCabecalho("NumeroAverbacao", false);
                grid.AdicionarCabecalho("TipoSeguro", false);
                grid.AdicionarCabecalho("CNPJEmpresa", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);

                grid.AdicionarCabecalho("Carga", "Carga", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Rota de Frete", "RotaFrete", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("CT-e", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Notas", "Notas", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left, true);

                Models.Grid.EditableCell editableValorFrete = null;
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true, false, false, false, true, editableValorFrete);

                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaCTeCargaMDFeManualMultiCTe filtrosPesquisa = ObterFiltrosPesquisaMultiCTe();

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

                int quantidadeRegistros = await repositorioCargaCTe.ContarConsultaCTesParaEmissaoMDFeMultiCTeAsync(filtrosPesquisa);

                string cnpjSeguradoraPadrao = string.Empty;
                string nomeSeguradoraPadrao = string.Empty;
                string apoliceSeguroPadrao = string.Empty;

                List<(string DescricaoRotaFrete, int CodigoCargaCTe)> descricaoRotaFretes = new List<(string DescricaoRotaFrete, int CodigoCargaCTe)>();

                if (quantidadeRegistros > 0)
                {
                    cargaCTes = await repositorioCargaCTe.ConsultarCTesParaEmissaoMDFeMultiCTeAsync(filtrosPesquisa, parametrosConsulta);

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTes.FirstOrDefault().CTe;

                    cnpjSeguradoraPadrao = ObterCnpjSeguradoraPadrao(cte);

                    nomeSeguradoraPadrao = ObterNomeSeguradoraPadrao(cte);

                    apoliceSeguroPadrao = ObterApoliceSeguroPadrao(cte);

                    descricaoRotaFretes = await repositorioCargaCTe.BuscarDescricaoRotaFretePorCargaCTeAsync(cargaCTes.Select(o => o.Codigo).ToList());
                }

                grid.setarQuantidadeTotal(quantidadeRegistros);

                var lista = (from obj in cargaCTes
                             select new
                             {
                                 obj.Codigo,
                                 CodigoOrigem = obj.CTe.LocalidadeInicioPrestacao.Codigo,
                                 Origem = obj.CTe.LocalidadeInicioPrestacao.DescricaoCidadeEstado,
                                 CodigoDestino = obj.CTe.LocalidadeTerminoPrestacao.Codigo,
                                 CNPJSeguradora = obj.CTe.Seguros != null && obj.CTe.Seguros.Count > 0 && !string.IsNullOrWhiteSpace(obj.CTe.Seguros[0].CNPJSeguradora) ? obj.CTe.Seguros[0].CNPJSeguradora : cnpjSeguradoraPadrao,
                                 NomeSeguradora = obj.CTe.Seguros != null && obj.CTe.Seguros.Count > 0 && !string.IsNullOrWhiteSpace(obj.CTe.Seguros[0].NomeSeguradora) ? obj.CTe.Seguros[0].NomeSeguradora : nomeSeguradoraPadrao,
                                 NumeroApolice = obj.CTe.Seguros != null && obj.CTe.Seguros.Count > 0 && !string.IsNullOrWhiteSpace(obj.CTe.Seguros[0].NumeroApolice) ? obj.CTe.Seguros[0].NumeroApolice : apoliceSeguroPadrao,
                                 NumeroAverbacao = obj.CTe.Seguros != null && obj.CTe.Seguros.Count > 0 ? !string.IsNullOrWhiteSpace(obj.CTe.Seguros[0].NumeroAverbacao) ? obj.CTe.Seguros[0].NumeroAverbacao : svcMDFe.BuscarAverbacaoCTe(obj.CTe.Codigo, obj.CTe.Empresa.Codigo, unitOfWork) : svcMDFe.BuscarAverbacaoCTe(obj.CTe.Codigo, obj.CTe.Empresa.Codigo, unitOfWork),
                                 TipoSeguro = obj.CTe.Seguros != null && obj.CTe.Seguros.Count > 0 ? obj.CTe.Seguros[0].Tipo == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente,
                                 CNPJEmpresa = obj.CTe.Empresa.CNPJ_SemFormato,
                                 CodigoCTE = obj.CTe.Codigo,
                                 Carga = obj.Carga.CodigoCargaEmbarcador,
                                 Numero = obj.CTe.Numero + " - " + obj.CTe.Serie.Numero,
                                 CodigoEmpresa = obj.CTe.Empresa.Codigo,
                                 Serie = obj.CTe.Serie.Numero,
                                 Notas = string.Join(", ", obj.CTe.XMLNotaFiscais.Select(o => o.Numero.ToString())),
                                 Remetente = obj.CTe.Remetente.Nome + "(" + obj.CTe.Remetente.CPF_CNPJ_Formatado + ")",
                                 Destinatario = obj.CTe.Destinatario.Nome + "(" + obj.CTe.Destinatario.CPF_CNPJ_Formatado + ")",
                                 Destino = obj.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                 ValorFrete = obj.CTe.ValorAReceber.ToString("n2"),
                                 RotaFrete = descricaoRotaFretes.FirstOrDefault(x => x.CodigoCargaCTe == obj.Codigo).DescricaoRotaFrete ?? string.Empty,
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
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarCargaCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
            Repositorio.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga repositorio = new Repositorio.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamento repCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);

            Servicos.Embarcador.Carga.OcultarInformacoesCarga.OcultarInformacoesCarga serOcultarInformacoesCarga = new Servicos.Embarcador.Carga.OcultarInformacoesCarga.OcultarInformacoesCarga(unitOfWork);

            bool possuiOcultarInformacoesCarga = serOcultarInformacoesCarga.PossuiOcultarInformacoesCarga(this.Usuario.Codigo);
            Dominio.Entidades.Embarcador.Cargas.OcultarInformacoesCarga.OcultarInformacoesCarga ocultarInformacoesCarga = null;
            if (possuiOcultarInformacoesCarga)
                ocultarInformacoesCarga = serOcultarInformacoesCarga.ObterOcultarInformacoesCarga(this.Usuario.Codigo);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int codigoCarga = int.Parse(Request.Params("Carga"));

                int numeroNF, numeroDocumento = 0, cargaPedido;
                int.TryParse(Request.Params("NumeroNF"), out numeroNF);
                int.TryParse(Request.Params("NumeroDocumento"), out numeroDocumento);
                int.TryParse(Request.Params("CargaPedido"), out cargaPedido);

                List<int> cargaPedidos = Request.GetListParam<int>("CargaPedidos");

                double destinatario = 0;
                double.TryParse(Request.Params("Destinatario"), out destinatario);

                bool ocultarStatusCTe = false;
                bool ctesSubContratacaoFilialEmissora = false;
                bool ctesSemSubContratacaoFilialEmissora = false;
                bool ctesFactura = false;
                bool cargaMercosul = false;

                bool.TryParse(Request.Params("OcultarStatusCTe"), out ocultarStatusCTe);
                bool.TryParse(Request.Params("CTesSubContratacaoFilialEmissora"), out ctesSubContratacaoFilialEmissora);
                bool.TryParse(Request.Params("CTesSemSubContratacaoFilialEmissora"), out ctesSemSubContratacaoFilialEmissora);
                bool.TryParse(Request.Params("CTesFactura"), out ctesFactura);
                bool.TryParse(Request.Params("CargaMercosul"), out cargaMercosul);
                bool cargaPortoPortoTimelineHabilitado = Request.GetBoolParam("CargaPortoPortoTimelineHabilitado");
                bool cargaPortaPortaTimelineHabilitado = Request.GetBoolParam("CargaPortaPortaTimelineHabilitado");
                bool retornarDocumentoOperacaoContainer = Request.GetBoolParam("RetornarDocumentoOperacaoContainer");
                bool cargaSVM = Request.GetBoolParam("CargaSVM");

                bool validarPendenciaTransportador = repCarga.PossuiPendenciaTransportadorContribuinte(codigoCarga);

                string statusCTe = Request.Params("Status");
                string numeroContainer = Request.Params("NumeroContainer");

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("NumeroModeloDocumentoFiscal", false);
                grid.AdicionarCabecalho("TipoDocumentoEmissao", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    grid.AdicionarCabecalho("Nº Controle", "NumeroControle", 8, Models.Grid.Align.center, true);
                else
                    grid.AdicionarCabecalho("NumeroControle", false);

                grid.AdicionarCabecalho("Serie", "Serie", 5, Models.Grid.Align.center, true);

                if (cargaSVM)
                    grid.AdicionarCabecalho("Nº CTM", "NumeroCTM", 8, Models.Grid.Align.left, false);
                else
                    grid.AdicionarCabecalho("NumeroCTM", false);

                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    grid.AdicionarCabecalho("Doc.", "AbreviacaoModeloDocumentoFiscal", 6, Models.Grid.Align.center, true);
                else
                    grid.AdicionarCabecalho("Doc.", "AbreviacaoModeloDocumentoFiscal", 10, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                        grid.AdicionarCabecalho("T. Pagamento", "DescricaoTipoPagamento", 9, Models.Grid.Align.center, true);
                    else
                        grid.AdicionarCabecalho("T. Pagamento", "DescricaoTipoPagamento", 10, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, true);
                }
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                {
                    grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, true);
                }

                grid.AdicionarCabecalho("Emissão", "DataEmissao", 10, Models.Grid.Align.left, true);

                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                    grid.AdicionarCabecalho("T. Modal", "DescricaoTipoModal", 8, Models.Grid.Align.center, true);
                else
                    grid.AdicionarCabecalho("DescricaoTipoModal", false);

                if (codigoCarga > 0)
                {
                    //Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                    //if (carga.TipoContratacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.NormalESubContratada)
                    grid.AdicionarCabecalho("T. Serviço", "DescricaoTipoServico", 8, Models.Grid.Align.center, true);
                }

                grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left, true);

                Models.Grid.EditableCell editableValorFrete = null; //new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 9);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true, false, false, false, true, editableValorFrete);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros && !ocultarStatusCTe)
                {
                    grid.AdicionarCabecalho("CST", "CST", 4, Models.Grid.Align.right, false);
                    grid.AdicionarCabecalho("Alíquota", "Aliquota", 5, Models.Grid.Align.right, false);
                    grid.AdicionarCabecalho("Valor IBS UF", "ValorIBSUF", 5, Models.Grid.Align.right, false);
                    grid.AdicionarCabecalho("Valor IBS Mun.", "ValorIBSMunicipal", 5, Models.Grid.Align.right, false);
                    grid.AdicionarCabecalho("Valor CBS", "ValorCBS", 5, Models.Grid.Align.right, false);
                    grid.AdicionarCabecalho("Status", "Status", 8, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho("Retorno Sefaz", "RetornoSefaz", 13, Models.Grid.Align.left, false);
                }

                if (ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal && !cargaPortoPortoTimelineHabilitado && !cargaPortaPortaTimelineHabilitado && !cargaSVM && integracaoIntercab.HabilitarTimelineCargaFeeder)
                    grid.AdicionarCabecalho("Nº SVM", "NumeroControleSVM", 6, Models.Grid.Align.center, true);
                else
                    grid.AdicionarCabecalho("NumeroControleSVM", false);

                grid.AdicionarCabecalho("ContemContainer", false);
                grid.AdicionarCabecalho("ContainerADefinir", false);
                grid.AdicionarCabecalho("Observacao", false);
                grid.AdicionarCabecalho("HabilitarSincronizarDocumento", false);
                grid.AdicionarCabecalho("HabilitarDesvincularCTeEGerarCopia", false);
                grid.AdicionarCabecalho("SistemaEmissor", false);

                if (validarPendenciaTransportador)
                    grid.AdicionarCabecalho("Situação Doc Transportador", "SituacaoDocumentoTransportador", 10, Models.Grid.Align.center, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                    propOrdenacao += ".Nome";
                if (propOrdenacao == "Destino")
                    propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

                if (propOrdenacao == "DescricaoTipoPagamento")
                    propOrdenacao = "TipoPagamento";

                if (propOrdenacao == "DescricaoTipoServico")
                    propOrdenacao = "TipoServico";

                if (propOrdenacao == "AbreviacaoModeloDocumentoFiscal")
                    propOrdenacao = "ModeloDocumentoFiscal.Abreviacao";

                propOrdenacao = "CTe." + propOrdenacao;

                string proprietarioVeiculo = string.Empty;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                    proprietarioVeiculo = Usuario.ClienteTerceiro != null ? Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato : null;
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    proprietarioVeiculo = Usuario.Empresa != null ? Usuario.Empresa.CNPJ_SemFormato : null;

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes;
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = null;
                int quantidade = 0;
                bool containerADefinir = false;

                if (codigoCarga > 0)
                {
                    bool buscarPorCargaOrigem = false;
                    cargaCancelamento = repCargaCancelamento.BuscarPorCarga(codigoCarga);

                    List<int> empresasFilialEmissora = new List<int>();
                    if (ctesSemSubContratacaoFilialEmissora)
                        empresasFilialEmissora = repCargaPedido.ObterEmpresasCargaFilialEmissora(codigoCarga);

                    containerADefinir = repCargaPedido.ContainerADefinirCarga(codigoCarga);

                    Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMontarConsultaCtes filtro = new()
                    {
                        Carga = codigoCarga,
                        NumeroDocumento = numeroDocumento,
                        NumeroNF = numeroNF,
                        StatusCTe = !string.IsNullOrWhiteSpace(statusCTe) ? new string[] { statusCTe } : null,
                        ApenasCTesNormais = true,
                        CtesSubContratacaoFilialEmissora = ctesSubContratacaoFilialEmissora,
                        CtesSemSubContratacaoFilialEmissora = ctesSemSubContratacaoFilialEmissora,
                        EmpresasFilialEmissora = empresasFilialEmissora,
                        ProprietarioVeiculo = proprietarioVeiculo,
                        Destinatario = destinatario,
                        BuscarPorCargaOrigem = buscarPorCargaOrigem,
                        RetornarPreCtes = false,
                        TiposDocumentosDoCte = null,
                        CTesFactura = ctesFactura,
                        CargaMercosul = cargaMercosul,
                        RetornarDocumentoOperacaoContainer = retornarDocumentoOperacaoContainer,
                        NumeroContainer = numeroContainer
                    };

                    cargaCTes = await repCargaCTe.ConsultarCTes(filtro,
                                                          propOrdenacao,
                                                          grid.dirOrdena,
                                                          grid.inicio,
                                                          grid.limite);

                    quantidade = await repCargaCTe.ContarConsultaCTes(filtro);
                }
                else
                {
                    if (cargaPedidos.Count == 0)
                        cargaPedidos.Add(cargaPedido);

                    containerADefinir = repCargaPedido.ContainerADefinir(cargaPedidos);

                    cargaCTes = repCargaPedidoXMLNotaFiscalCTe.BuscarCargaCTesPorCargaPedidoDaCarga(cargaPedidos, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                    quantidade = repCargaPedidoXMLNotaFiscalCTe.ContarCargaCTesPorCargaPedidoDaCarga(cargaPedidos);
                }

                grid.setarQuantidadeTotal(quantidade);
                Servicos.Embarcador.Carga.CargaCTe servicoCargaCTe = new Servicos.Embarcador.Carga.CargaCTe(unitOfWork);
                var lista = (from obj in cargaCTes
                             select new
                             {
                                 obj.Codigo,
                                 CodigoCTE = obj.CTe.Codigo,
                                 obj.CTe.DescricaoTipoServico,
                                 obj.CTe.DescricaoTipoModal,
                                 obj.CTe.NumeroCTM,
                                 NumeroModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Numero,
                                 TipoDocumentoEmissao = obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao,
                                 AbreviacaoModeloDocumentoFiscal = obj.CTe.ModeloDocumentoFiscal.Abreviacao,
                                 CodigoEmpresa = obj.CTe.Empresa.Codigo,
                                 Numero = !string.IsNullOrWhiteSpace(obj.CTe.NumeroCRT) ? obj.CTe.NumeroCRT : obj.CTe.Numero.ToString(),
                                 obj.CTe.NumeroControle,
                                 obj.CTe.NumeroControleSVM,
                                 obj.CTe.DataEmissao,
                                 SituacaoCTe = obj.CTe.Status,
                                 Serie = obj.CTe.Serie.Numero,
                                 obj.CTe.DescricaoTipoPagamento,
                                 Remetente = obj.CTe.Remetente != null ? obj.CTe.Remetente.Nome + (!obj.CTe.Remetente.Exterior ? " (" + obj.CTe.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty) : string.Empty,
                                 Destinatario = obj.CTe.Destinatario != null ? obj.CTe.Destinatario.Cliente?.Descricao ?? obj.CTe.Destinatario.Nome : string.Empty,
                                 Destino = obj.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                 ValorFrete = possuiOcultarInformacoesCarga ? serOcultarInformacoesCarga.ValidarOcultarValor(ocultarInformacoesCarga, TipoValorOcultarInformacoesCarga.ValorFrete, ObterValorFrete(obj, ConfiguracaoEmbarcador)) : ObterValorFrete(obj, ConfiguracaoEmbarcador),
                                 obj.CTe.CST,
                                 Aliquota = obj.CTe.AliquotaICMS > 0 ? obj.CTe.AliquotaICMS.ToString("n2") : obj.CTe.AliquotaISS.ToString("n4"),
                                 Status = obj.CTe.DescricaoStatus,
                                 RetornoSefaz = obj.CTe.MensagemStatus != null ? obj.CTe.MensagemStatus.MensagemDoErro + (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? "" : " (" + (!string.IsNullOrWhiteSpace(obj.CTe.MensagemRetornoSefaz) ? (obj.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.CTe.MensagemRetornoSefaz).Replace("&lt", "").Replace("&#39", "") : "") : "") + ")") : (!string.IsNullOrWhiteSpace(obj.CTe.MensagemRetornoSefaz) ? (obj.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.CTe.MensagemRetornoSefaz).Replace("&lt", "").Replace("&#39", "") : "") : ""),
                                 ContemContainer = (obj.CTe.Containers != null && obj.CTe.Containers.Count > 0),
                                 ContainerADefinir = containerADefinir,
                                 Observacao = obj.CTe.ObservacoesGerais,
                                 SituacaoDocumentoTransportador = obj.SituacaoDocumentoContribuinte.ObterDescricao(),
                                 HabilitarSincronizarDocumento = servicoCargaCTe.ObterHabilitarSincronizarDocumento(obj.CTe, cargaCancelamento),
                                 HabilitarDesvincularCTeEGerarCopia = obj.CTe.Status == "R" && obj.CTe.CodStatusProtocolo == "204" && obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? true : false,
                                 obj.CTe.SistemaEmissor,
                                 DT_RowColor = !ocultarStatusCTe ? ObterCorLinhaCTe(obj, statusCTe, validarPendenciaTransportador) : "",
                                 DT_FontColor = obj.VinculoManual ? "#FFFFFF" : !ocultarStatusCTe ? (string.IsNullOrWhiteSpace(statusCTe) ? ((obj.CTe.Status == "R" || obj.CTe.Status == "C" || obj.CTe.Status == "I" || obj.CTe.Status == "D") ? "#FFFFFF" : "") : "") : "",
                                 ValorIBSUF = obj.CTe.ValorIBSEstadual.ToString("n2"),
                                 ValorIBSMunicipal = obj.CTe.ValorIBSMunicipal.ToString("n2"),
                                 ValorCBS = obj.CTe.ValorCBS.ToString("n2"),
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

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarCargaNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Repositorios
                Repositorio.Embarcador.Cargas.CargaNFe repCargaNFe = new Repositorio.Embarcador.Cargas.CargaNFe(unitOfWork);

                // Converte parametros
                int carga = 0;
                int.TryParse(Request.Params("Carga"), out carga);

                // Grid
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCarga", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("Nº Nota", "Numero", 9, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Série", "Serie", 5, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 14, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Saída", "DataSaida", 14, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Último Retorno SEFAZ", "UltimoStatusSEFAZ", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Pessoa", "NomePessoa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Natureza da Operação", "NaturezaOperacao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Chave", "Chave", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "ValorTotalNota", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho("CodigoNaturezaOperacao", false);
                grid.AdicionarCabecalho("AtivarEnvioDanfeSMS", false);

                // Ordenacao
                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                if (string.IsNullOrWhiteSpace(propOrdenacao) || propOrdenacao == "Codigo")
                    propOrdenacao = "Numero";

                // Busca os valores
                List<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal> listaNotaFiscal = repCargaNFe.ConsultarPorCarga(carga, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repCargaNFe.ContarConsultaPorCarga(carga);

                // Formata retorno
                var lista = (from p in listaNotaFiscal
                             select new
                             {
                                 p.Codigo,
                                 CodigoCarga = carga,
                                 Descricao = p.Numero,
                                 p.Numero,
                                 Serie = p.EmpresaSerie?.Numero ?? 0,
                                 p.DataEmissao,
                                 DataSaida = p.DataSaida != null && p.DataSaida.HasValue ? p.DataSaida.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 p.DescricaoStatus,
                                 p.UltimoStatusSEFAZ,
                                 NomePessoa = p.Cliente?.Nome ?? string.Empty,
                                 NaturezaOperacao = p.NaturezaDaOperacao?.Descricao ?? string.Empty,
                                 p.Chave,
                                 ValorTotalNota = p.ValorTotalNota.ToString("n2"),
                                 p.Status,
                                 CodigoNaturezaOperacao = p.NaturezaDaOperacao != null ? p.NaturezaDaOperacao.Codigo : 0,
                                 AtivarEnvioDanfeSMS = p.Empresa?.AtivarEnvioDanfeSMS ?? false,
                                 DT_RowColor = p.Status == Dominio.Enumeradores.StatusNFe.Autorizado ? "#dff0d8" : p.Status == Dominio.Enumeradores.StatusNFe.Rejeitado ? "rgba(193, 101, 101, 1)" : (p.Status == Dominio.Enumeradores.StatusNFe.Cancelado || p.Status == Dominio.Enumeradores.StatusNFe.Inutilizado || p.Status == Dominio.Enumeradores.StatusNFe.Denegado) ? "#777" : "",
                                 DT_FontColor = (p.Status == Dominio.Enumeradores.StatusNFe.Rejeitado || p.Status == Dominio.Enumeradores.StatusNFe.Cancelado || p.Status == Dominio.Enumeradores.StatusNFe.Inutilizado || p.Status == Dominio.Enumeradores.StatusNFe.Denegado) ? "#FFFFFF" : ""
                             }).ToList();

                // Vincula à grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadMicDta()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaMICDTA repCargaMICDTA = new Repositorio.Embarcador.Cargas.CargaMICDTA(unidadeDeTrabalho);
                Servicos.Embarcador.Carga.MICDTA serMICDTA = new Servicos.Embarcador.Carga.MICDTA(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaMICDTA cargaMICDTA = await repCargaMICDTA.BuscarPorCodigoAsync(codigo, false);

                if (cargaMICDTA == null)
                    return new JsonpResult(false, true, "MIC/DTA não encontrada.");

                return Arquivo(serMICDTA.ObterPdfMicDta(cargaMICDTA.Carga, unidadeDeTrabalho, cargaMICDTA, null), "application/pdf", $"MIC/DTA {cargaMICDTA.Numero}.pdf");
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao baixar o MIC/DTA.");
            }
            finally
            {
                await unidadeDeTrabalho.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarCargaMICDTA()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaMICDTA repCargaMICDTA = new Repositorio.Embarcador.Cargas.CargaMICDTA(unitOfWork);

                int codigoCarga = Request.GetIntParam("Carga");
                string numero = Request.GetStringParam("NumeroMICDTA");


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 10, Models.Grid.Align.center, false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Cargas.CargaMICDTA> mics = repCargaMICDTA.ConsultaPorCarga(codigoCarga, numero, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repCargaMICDTA.ContarConsultaPorCarga(codigoCarga, numero);

                var lista = (from obj in mics
                             select new
                             {
                                 obj.Codigo,
                                 obj.Numero,
                                 SituacaoIntegracao = obj.SituacaoIntegracao.ObterDescricao(),
                                 DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                 DT_RowColor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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


        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarCargaCTeAverbacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);

                int codigoCarga = Request.GetIntParam("Carga");
                int codigoCancelamentoCarga = Request.GetIntParam("CancelamentoCarga");
                int numero = Request.GetIntParam("NumeroCTe");

                Dominio.Enumeradores.StatusAverbacaoCTe? situacao = null;
                if (Enum.TryParse(Request.Params("SituacaoAverbacao"), out Dominio.Enumeradores.StatusAverbacaoCTe situacaoAux))
                    situacao = situacaoAux;

                string apolice = Request.Params("Apolice");

                bool.TryParse(Request.Params("CTesSubContratacaoFilialEmissora"), out bool ctesSubContratacaoFilialEmissora);
                bool.TryParse(Request.Params("CTesSemSubContratacaoFilialEmissora"), out bool ctesSemSubContratacaoFilialEmissora);
                bool.TryParse(Request.Params("RetornarDocumentoOperacaoContainer"), out bool retornarDocumentoOperacaoContainer);

                if (string.IsNullOrWhiteSpace(apolice))
                    apolice = string.Empty;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "NumeroCTe", 5, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Nº Averbação", "Averbacao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Apólice", "Apolice", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Seguradora", "Seguradora", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Averbadora", "DescricaoSeguradoraAverbacao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoStatus", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Forma", "DescricaoForma", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho("Data do Retorno", "DataRetorno", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Retorno", "MensagemRetorno", 30, Models.Grid.Align.left, true);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "NumeroCTe") propOrdenacao = "CTe.Numero";
                else if (propOrdenacao == "Apolice") propOrdenacao = "ApoliceSeguroAverbacao.ApoliceSeguro.NumeroApolice";
                else if (propOrdenacao == "Seguradora") propOrdenacao = "ApoliceSeguroAverbacao.ApoliceSeguro.Seguradora.Nome";
                else if (propOrdenacao == "DescricaoSeguradoraAverbacao") propOrdenacao = "ApoliceSeguroAverbacao.ApoliceSeguro.SeguradoraAverbacao";
                else if (propOrdenacao == "DescricaoStatus") propOrdenacao = "Status";
                else if (propOrdenacao == "DescricaoForma") propOrdenacao = "Forma";

                bool? filtrarFilialEmissora = null;
                if (ctesSemSubContratacaoFilialEmissora || ctesSubContratacaoFilialEmissora)
                    filtrarFilialEmissora = ctesSemSubContratacaoFilialEmissora;

                List<Dominio.Entidades.AverbacaoCTe> averbacoesCarga = repAverbacaoCTe.ConsultaPorCarga(codigoCarga, codigoCancelamentoCarga, numero, apolice, situacao, filtrarFilialEmissora, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite, retornarDocumentoOperacaoContainer);
                int totalRegistros = repAverbacaoCTe.ContarConsultaPorCarga(codigoCarga, codigoCancelamentoCarga, numero, apolice, situacao, filtrarFilialEmissora, retornarDocumentoOperacaoContainer);

                var lista = (from obj in averbacoesCarga
                             select new
                             {
                                 obj.Codigo,
                                 NumeroCTe = (obj.CTe.NumeroSequencialCRT > 0 ? obj.CTe.NumeroSequencialCRT.ToString() : obj.CTe?.Numero.ToString()) ?? obj.XMLNotaFiscal.Numero.ToString(),
                                 obj.Averbacao,
                                 Apolice = obj.ApoliceSeguroAverbacao.ApoliceSeguro.NumeroApolice,
                                 Seguradora = obj.ApoliceSeguroAverbacao.ApoliceSeguro.Seguradora?.Nome ?? string.Empty,
                                 DescricaoSeguradoraAverbacao = obj.ApoliceSeguroAverbacao.ApoliceSeguro.DescricaoSeguradoraAverbacao,
                                 obj.DescricaoStatus,
                                 obj.DescricaoForma,
                                 obj.Status,
                                 DataRetorno = obj.DataRetorno?.ToString("dd/MM/yyy") ?? "",
                                 obj.MensagemRetorno,
                                 DT_RowColor = CorAverbacaoCTe(obj),
                                 DT_FontColor = CorFonteAverbacaoCTe(obj)
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadEDI()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe;
                int.TryParse(Request.Params("CodigoCTe"), out codigoCTe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                ctes.Add(cte);

                string extensao = ".txt";

                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unidadeDeTrabalho);

                if (cte.TomadorPagador.Cliente.GrupoPessoas != null)
                {

                    Dominio.Entidades.LayoutEDI layoutEDI = (from obj in cte.TomadorPagador.Cliente.GrupoPessoas.LayoutsEDI where obj.LayoutEDI.Tipo == Dominio.Enumeradores.TipoLayoutEDI.CONEMB select obj.LayoutEDI).FirstOrDefault();

                    if (layoutEDI != null)
                    {
                        Servicos.GeracaoEDI svcEDI = new Servicos.GeracaoEDI(unidadeDeTrabalho, layoutEDI, cte.Empresa, ctes.Distinct().ToList());
                        string nomeArquivo = Servicos.Embarcador.Integracao.IntegracaoEDI.ObterNomeArquivoEDI(cte, layoutEDI, extensao, unidadeDeTrabalho);
                        System.IO.MemoryStream edi = svcEDI.GerarArquivo();
                        return Arquivo(edi, "plain/text", nomeArquivo);
                    }
                    else
                    {
                        return new JsonpResult(false, false, "Não Existe um Layout EDI configurado para esse tomador.");
                    }
                }
                else
                {
                    return new JsonpResult(false, false, "Não Existe um Layout EDI configurado para esse tomador.");
                }
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao baixar o EDI.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarEmailProvedor()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAverbacao;
                int.TryParse(Request.Params("Codigo"), out codigoAverbacao);

                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unidadeDeTrabalho);
                Dominio.Entidades.AverbacaoCTe averbacao = repAverbacaoCTe.BuscarPorCodigo(codigoAverbacao);

                if (averbacao != null)
                {

                    string protocolo = averbacao.Protocolo;
                    string numeroAverbacao = averbacao.Averbacao;
                    string stringConexao = _conexao.StringConexao;

                    Task.Run(() => Servicos.Embarcador.Integracao.ATM.ATMIntegracao.EnviarEmailAverbacao(codigoAverbacao, protocolo, numeroAverbacao, stringConexao, false));
                    //Servicos.Embarcador.Integracao.ATM.ATMIntegracao.EnviarEmailAverbacao(averbacao, averbacao.Protocolo, averbacao.Averbacao, unidadeDeTrabalho, false);
                }

                else
                    return new JsonpResult(false, false, "Averbação não localizada..");

                return new JsonpResult(true, true, "Sucesso");
            }
            catch (Exception ex)
            {

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao enviar o e-mail ao Provedor.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadDacteComplemento()
        {
            string nomeDocumento = "DACTE";

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe, codigoEmpresa = 0;
                int.TryParse(Request.Params("CodigoCTe"), out codigoCTe);
                int.TryParse(Request.Params("CodigoEmpresa"), out codigoEmpresa);

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);
                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unidadeTrabalho);
                Servicos.DACTE svcDACTE = new Servicos.DACTE(unidadeTrabalho);
                Servicos.CCe svcCCe = new Servicos.CCe(unidadeTrabalho);

                Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(unidadeTrabalho);
                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteOriginal = repCTe.BuscarPorCodigo(codigoEmpresa, codigoCTe);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (cteOriginal == null)
                    return new JsonpResult(true, false, "CT-e não encontrado, atualize a página e tente novamente.");

                if (cteOriginal.PossuiCTeComplementar == false)
                    return new JsonpResult(true, false, "Este CT-e não possui complemento lançado.");

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarCTeComplementarPorChave(cteOriginal.Chave);
                if (cte == null)
                    return new JsonpResult(true, false, "CT-e não encontrado, atualize a página e tente novamente.");

                if (cte.Status != "A" && cte.Status != "C" && cte.Status != "K" && cte.Status != "Z" && cte.Status != "F")
                    return new JsonpResult(true, false, "O status do CT-e complementar não permite a geração do " + nomeDocumento + ".");

                string nomeArquivo = cte.Chave;
                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS)
                {
                    nomeDocumento = cte.ModeloDocumentoFiscal.Abreviacao;
                    nomeArquivo = cte.Numero + "_" + cte.Serie.Numero + "_" + cte.ModeloDocumentoFiscal.Abreviacao;

                    if (!string.IsNullOrWhiteSpace(cte.ModeloDocumentoFiscal.Relatorio))
                    {
                        byte[] arquivo = new Servicos.Embarcador.Relatorios.OutrosDocumentos(unidadeTrabalho).ObterPdf(cte);

                        return Arquivo(arquivo, "application/pdf", nomeArquivo + ".pdf");
                    }
                }

                if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para os download da " + nomeDocumento + " não está disponível. Contate o suporte técnico.");

                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                    nomeArquivo = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString();

                if (configuracaoTMS.GerarPDFCTeCancelado && cte.Status == "C" && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    nomeArquivo = nomeArquivo + "_Canc";

                if (cte.Status == "F")
                    nomeArquivo = nomeArquivo + "_FSDA";

                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios, cte.Empresa.CNPJ, nomeArquivo) + ".pdf";

                byte[] pdf = null;

                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                {
                    Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeTrabalho);

                    pdf = svcNFSe.ObterDANFSECTe(cte.Codigo);
                }
                else
                {
                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                    {
                        if (string.IsNullOrWhiteSpace(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoGeradorRelatorios))
                            return new JsonpResult(true, false, "O gerador da " + nomeDocumento + " não está disponível. Contate o suporte técnico.");

                        pdf = svcDACTE.GerarPorProcesso(cte.Codigo, null, configuracaoTMS.GerarPDFCTeCancelado);
                    }
                    else
                    {
                        pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                    }
                }
                byte[] arquivoCCe = null;
                Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarUltimaCCeAutorizadaPorCTe(cte.Codigo);
                if (cce != null && configuracaoTMS.ImprimirDACTEeCartaCorrecaoJunto)
                {
                    var resultReportApi = ReportRequest.WithType(ReportType.CCe)
                                                       .WithExecutionType(ExecutionType.Sync)
                                                       .AddExtraData("codigoCCe", cce.Codigo)
                                                       .CallReport();

                    if (resultReportApi == null)
                        throw new Exception();

                    arquivoCCe = resultReportApi.GetContentFile();
                }

                if (pdf != null)
                {
                    if (arquivoCCe != null)
                    {
                        List<byte[]> sourceFiles = new List<byte[]>();
                        sourceFiles.Add(pdf);
                        sourceFiles.Add(arquivoCCe);

                        byte[] pdfAgrupado = svcDACTE.MergeFiles(sourceFiles);

                        return Arquivo(pdfAgrupado, "application/pdf", System.IO.Path.GetFileName(caminhoPDF));
                    }
                    else
                        return Arquivo(pdf, "application/pdf", System.IO.Path.GetFileName(caminhoPDF));
                }
                else
                    return new JsonpResult(false, false, "Não foi possível gerar o " + nomeDocumento + ", atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download da " + nomeDocumento);
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadDacte()
        {
            string nomeDocumento = "DACTE";

            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe, codigoEmpresa, codigoOcorrencia = 0;
                int.TryParse(Request.Params("CodigoCTe"), out codigoCTe);
                int.TryParse(Request.Params("CodigoEmpresa"), out codigoEmpresa);
                int.TryParse(Request.Params("CodigoOcorrencia"), out codigoOcorrencia);

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);
                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unidadeTrabalho);
                Servicos.DACTE svcDACTE = new Servicos.DACTE(unidadeTrabalho);
                Servicos.CCe svcCCe = new Servicos.CCe(unidadeTrabalho);

                Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(unidadeTrabalho);
                Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unidadeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoEmpresa, codigoCTe);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Cargas.CargaCTe repCarga = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarCargaPorCTe(codigoCTe);

                if (cte == null)
                    return new JsonpResult(true, false, "CT-e não encontrado, atualize a página e tente novamente.");

                string[] statusCTePermitidos = ["A", "C", "K", "Z", "F", "Q"];

                if (!statusCTePermitidos.Contains(cte.Status))
                    return new JsonpResult(true, false, $"O status do CT-e não permite a geração do {nomeDocumento}.");

                List<int> CodigosCte = new List<int> { codigoCTe };
                // TODO: ToList cast
                List<(int CodigoCTe, bool ImprimirTabelaTemperaturaVersoCTe)> ctesImprimirTabelaTemperatura = repCTe.BuscarInformacaoImpressaoTabelaTemperaturaVersoCTe(CodigosCte).ToList();
                bool imprimirTabelaTemperaturaNoVersoCTe = ctesImprimirTabelaTemperatura.Exists(cteImprimirTabelaTemperatura => cteImprimirTabelaTemperatura.CodigoCTe == cte.Codigo && cteImprimirTabelaTemperatura.ImprimirTabelaTemperaturaVersoCTe);
                string nomeArquivoFisico = cte.Chave;
                byte[] pdf = null;

                if (cte.ModeloDocumentoFiscal.DocumentoTipoCRT)
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repOcorrencia.BuscarPorCodigo(codigoOcorrencia);
                    if (cargaOcorrencia != null)
                    {
                        pdf = new Servicos.Embarcador.Carga.Impressao(unidadeTrabalho).ObterPDFCRTComplementar(cte, cargaOcorrencia);
                        nomeArquivoFisico = $"BR{cargaOcorrencia.NumeroOcorrencia}";
                    }
                    else
                    {
                        pdf = new Servicos.Embarcador.Carga.Impressao(unidadeTrabalho).ObterPDFCRT(cte, carga);
                        nomeArquivoFisico = cte.NumeroCRT;
                    }

                    return Arquivo(pdf, "application/pdf", nomeArquivoFisico + ".pdf");

                }
                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe &&
                    cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFSe &&
                    cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.NFS)
                {
                    nomeDocumento = cte.ModeloDocumentoFiscal.Abreviacao;
                    nomeArquivoFisico = cte.Numero + "_" + cte.Serie.Numero + "_" + cte.ModeloDocumentoFiscal.Abreviacao;

                    if (!string.IsNullOrWhiteSpace(cte.ModeloDocumentoFiscal.Relatorio))
                    {
                        byte[] arquivo = new Servicos.Embarcador.Relatorios.OutrosDocumentos(unidadeTrabalho).ObterPdf(cte);

                        return Arquivo(arquivo, "application/pdf", nomeArquivoFisico + ".pdf");
                    }
                }

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para os download da " + nomeDocumento + " não está disponível. Contate o suporte técnico.");

                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                    nomeArquivoFisico = cte.Numero.ToString() + "_" + cte.Serie.Numero.ToString();

                if (configuracaoTMS.GerarPDFCTeCancelado && cte.Status == "C" && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                    nomeArquivoFisico = nomeArquivoFisico + "_Canc";

                if (cte.Status == "F")
                    nomeArquivoFisico = nomeArquivoFisico + "_FSDA";

                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRelatorios, cte.Empresa.CNPJ, nomeArquivoFisico) + ".pdf";

                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                {
                    Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeTrabalho);
                    if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                        pdf = svcNFSe.ObterDANFSECTe(cte.Codigo, null, true);
                    else
                        pdf = svcNFSe.ObterDANFSECTe(cte.Codigo);
                }
                else
                {
                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                    {
                        if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                            return new JsonpResult(true, false, "O gerador da " + nomeDocumento + " não está disponível. Contate o suporte técnico.");

                        pdf = svcDACTE.GerarPorProcesso(cte.Codigo, null, configuracaoTMS.GerarPDFCTeCancelado);
                    }
                    else
                    {
                        pdf = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                    }
                }

                byte[] arquivoCCe = null;

                if (configuracaoTMS.ImprimirDACTEeCartaCorrecaoJunto)
                {
                    Dominio.Entidades.CartaDeCorrecaoEletronica cce = repCCe.BuscarUltimaCCeAutorizadaPorCTe(cte.Codigo);

                    if (cce != null)
                    {
                        var resultReportApi = ReportRequest.WithType(ReportType.CCe)
                                                    .WithExecutionType(ExecutionType.Sync)
                                                    .AddExtraData("codigoCCe", cce.Codigo)
                                                    .CallReport();

                        if (resultReportApi == null)
                            throw new Exception();

                        arquivoCCe = resultReportApi.GetContentFile();
                    }
                }

                byte[] versoCTe = null;

                if (imprimirTabelaTemperaturaNoVersoCTe)
                {
                    versoCTe = ReportRequest.WithType(ReportType.RegistroTemperaturaETrocaDeGelo)
                        .WithExecutionType(ExecutionType.Sync)
                        .CallReport()
                        .GetContentFile();
                }

                if (pdf != null)
                {
                    string nomeArquivoDownload = Servicos.Embarcador.CTe.CTe.ObterNomeArquivoDownloadCTe(cte, "pdf");

                    if (string.IsNullOrWhiteSpace(nomeArquivoDownload))
                        nomeArquivoDownload = System.IO.Path.GetFileName(caminhoPDF);

                    List<byte[]> sourceFiles = new List<byte[]>();
                    sourceFiles.Add(pdf);
                    if (arquivoCCe != null)
                        sourceFiles.Add(arquivoCCe);
                    if (versoCTe != null)
                        sourceFiles.Add(versoCTe);

                    byte[] pdfAgrupado = svcDACTE.MergeFiles(sourceFiles);

                    return Arquivo(pdfAgrupado, "application/pdf", nomeArquivoDownload);
                }
                else
                    return new JsonpResult(false, false, "Não foi possível gerar o " + nomeDocumento + ", atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download da " + nomeDocumento);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadLoteDocumentos()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                bool ctesSubContratacaoFilialEmissora = false;
                bool ctesSemSubContratacaoFilialEmissora = false;

                bool.TryParse(Request.Params("CTesSubContratacaoFilialEmissora"), out ctesSubContratacaoFilialEmissora);
                bool.TryParse(Request.Params("CTesSemSubContratacaoFilialEmissora"), out ctesSemSubContratacaoFilialEmissora);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para o download das DACTEs não está disponível. Contate o suporte técnico.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> ctes = repCargaCTe.BuscarCodigosCTesAutorizadosPorCarga(codigoCarga, ctesSubContratacaoFilialEmissora, ctesSemSubContratacaoFilialEmissora);

                if (ctes.Count <= 0)
                    return new JsonpResult(false, true, "Não foram encontrados CT-es autorizados para esta carga.");

                int codigoUsuario = this.Usuario.Codigo;
                string stringConexao = _conexao.StringConexao;
                string caminhoArquivos = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoArquivos;
                string diretorioDocumentosFiscais = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoDocumentosFiscaisEmbarcador;
                string caminhoArquivosAnexos = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Pedido" });

                Task.Run(() => Zeus.Embarcador.ZeusNFe.Zeus.GerarPDFTodosDocumentos(0, ctes, stringConexao, codigoUsuario, caminhoRelatorios, caminhoArquivos, diretorioDocumentosFiscais, "Cargas/Carga", caminhoArquivosAnexos));

                return new JsonpResult(true, true, "Sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote dos documentos.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLoteDANFE()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaNFe repCargaNFe = new Repositorio.Embarcador.Cargas.CargaNFe(unidadeTrabalho);

                string caminhoRelatorioEmbarcador = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatoriosEmbarcador.ConvertToOSPlatformPath();

                if (string.IsNullOrWhiteSpace(caminhoRelatorioEmbarcador))
                    return new JsonpResult(true, false, "O caminho para o download das DACTEs não está disponível. Contate o suporte técnico.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> nfes = repCargaNFe.BuscarCodigosNFesAutorizadosPorCarga(codigoCarga);

                if (nfes.Count <= 0)
                    return new JsonpResult(false, true, "Não foram encontrados documentos autorizados para esta carga.");

                if (nfes.Count > 2000)
                    return new JsonpResult(false, true, "Não é permitido o download de mais de 2000 arquivos.");

                Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unidadeTrabalho);

                System.IO.MemoryStream arquivo = serNotaFiscalEletronica.ObterLoteDeDANFE(nfes, caminhoRelatorioEmbarcador, this.Usuario, unidadeTrabalho);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_DANFE.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de DACTE.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLoteDACTE()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                bool ctesSubContratacaoFilialEmissora = false;
                bool ctesSemSubContratacaoFilialEmissora = false;

                bool.TryParse(Request.Params("CTesSubContratacaoFilialEmissora"), out ctesSubContratacaoFilialEmissora);
                bool.TryParse(Request.Params("CTesSemSubContratacaoFilialEmissora"), out ctesSemSubContratacaoFilialEmissora);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);

                string caminhoRelatorios = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeTrabalho).ObterConfiguracaoArquivo().CaminhoRelatorios;

                if (string.IsNullOrWhiteSpace(caminhoRelatorios))
                    return new JsonpResult(true, false, "O caminho para o download das DACTEs não está disponível. Contate o suporte técnico.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> ctes = repCargaCTe.BuscarCodigosCTesAutorizadosPorCarga(codigoCarga, ctesSubContratacaoFilialEmissora, ctesSemSubContratacaoFilialEmissora, false);
                List<int> nfses = repCargaCTe.BuscarCodigosNFSePorCarga(codigoCarga);

                if (ctes.Count <= 0 && nfses.Count <= 0)
                    return new JsonpResult(false, true, "Não foram encontrados documentos autorizados para esta carga.");

                if (ctes.Count + nfses.Count > 2000)
                    return new JsonpResult(false, true, "Não é permitido o download de mais de 2000 arquivos.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                System.IO.MemoryStream arquivo = svcCTe.ObterLoteDeDACTE(ctes, nfses, unidadeTrabalho);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_DACTE.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de DACTE.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXML()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe, codigoEmpresa = 0;
                int.TryParse(Request.Params("CodigoCTe"), out codigoCTe);
                int.TryParse(Request.Params("CodigoEmpresa"), out codigoEmpresa);

                if (codigoCTe > 0)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoEmpresa, codigoCTe);

                    if (cte != null && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ||
                                       cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe ||
                                       cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                    {
                        Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                        byte[] data = svcCTe.ObterXMLAutorizacao(cte, unitOfWork, Auditado, TipoServicoMultisoftware);

                        if (data != null)
                        {
                            string nomeArquivoDownload = Servicos.Embarcador.CTe.CTe.ObterNomeArquivoDownloadCTe(cte, "xml");

                            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                            {
                                if (string.IsNullOrWhiteSpace(nomeArquivoDownload))
                                    nomeArquivoDownload = string.Concat(cte.Chave, ".xml");

                                return Arquivo(data, "text/xml", nomeArquivoDownload);
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(nomeArquivoDownload))
                                    nomeArquivoDownload = string.Concat("NFSe_", cte.Numero, ".xml");

                                return Arquivo(data, "text/xml", nomeArquivoDownload);
                            }
                        }
                    }
                    else
                    {
                        return new JsonpResult(false, false, "XML é possível baixar o xml de um arquivo que não é um CT-e/NFS-e.");
                    }
                }

                return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadXMLMigrate()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe, codigoEmpresa = 0;
                int.TryParse(Request.Params("CodigoCTe"), out codigoCTe);
                int.TryParse(Request.Params("CodigoEmpresa"), out codigoEmpresa);

                if (codigoCTe > 0)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoEmpresa, codigoCTe);

                    if (cte != null && cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe || cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS)
                    {
                        Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                        System.IO.MemoryStream arquivo = svcCTe.ObterXMLIntegracao(cte, unitOfWork);

                        if (arquivo != null)
                            return Arquivo(arquivo, "application/octet-stream", $"xmlIntegracaoMigrate_NFS{cte.Numero}.zip");
                    }
                    else
                    {
                        return new JsonpResult(false, false, "XML é possível baixar o xml de um arquivo que não é um CT-e/NFS-e.");
                    }
                }

                return new JsonpResult(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadLoteXML()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                bool ctesSubContratacaoFilialEmissora = false;
                bool ctesSemSubContratacaoFilialEmissora = false;

                bool.TryParse(Request.Params("CTesSubContratacaoFilialEmissora"), out ctesSubContratacaoFilialEmissora);
                bool.TryParse(Request.Params("CTesSemSubContratacaoFilialEmissora"), out ctesSemSubContratacaoFilialEmissora);

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
                Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Carga não encontrada.");

                List<int> codigosCTes = repCargaCTe.BuscarCodigosCTesAutorizadosPorCarga(codigoCarga, ctesSubContratacaoFilialEmissora, ctesSemSubContratacaoFilialEmissora, false);
                List<int> codigosNFSes = repCargaCTe.BuscarCodigosNFSePorCarga(codigoCarga);
                for (int i = 0; i < codigosNFSes.Count(); i++)
                    codigosCTes.Add(codigosNFSes[i]);

                if (codigosCTes.Count <= 0 && codigosNFSes.Count <= 0)
                    return new JsonpResult(false, true, "Não há CT-es/NFS-es disponíveis para esta carga.");

                //if (codigosCTes.Count > 500)
                //    return new JsonpResult(false, true, "Não é possível realizar o download de mais de 500 arquivos.");

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                System.IO.MemoryStream arquivo = svcCTe.ObterLoteDeXML(codigosCTes, 0, unidadeTrabalho);

                return Arquivo(arquivo, "application/octet-stream", string.Concat(Utilidades.File.GetValidFilename(carga.CodigoCargaEmbarcador), "_XML.zip"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de XML.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPermissoesCargaCTes()
        {
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoesEmDigitacao = BuscarPermissoesCTeEmDigitacaoCarga();
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoesRejeicao = BuscarPermissoesCTeRejeitado();
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoesOutrosDocumentosEmDigitacao = BuscarPermissoesOutrosDocumentosEmDigitacaoCarga();

                var retorno = new
                {
                    EmDigitacao = permissoesEmDigitacao,
                    Rejeicao = permissoesRejeicao,
                    OutrosDocumentosEmDigitacao = permissoesOutrosDocumentosEmDigitacao
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as pemissões do CT-e.");
            }

        }

        public async Task<IActionResult> AutorizarEmissaoCTes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_AutorizarEmissaoDocumentos) && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codCarga;
                int.TryParse(Request.Params("Carga"), out codCarga);

                string status = Request.Params("Status");

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                Servicos.Embarcador.Carga.CTe serCargaCTE = new Servicos.Embarcador.Carga.CTe(unitOfWork);
                Servicos.Embarcador.Pedido.NotaFiscal serPedidoNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCte.BuscarCargaCteRejeitadoPorCarga(codCarga);
                if (configuracaoGeralCarga?.AtualizarDataEmissaoParaDataAtualQuandoReemitirCTeRejeitado ?? false)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte in cargaCTes)
                    {
                        cargaCte.CTe.DataEmissao = DateTime.Now;
                        repCargaCte.Atualizar(cargaCte);
                    }
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && carga.Empresa.Codigo != Usuario.Empresa.Codigo)
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                if (serPedidoNotaFiscal.VerificarSeCargaPossuiNotaCanceladaPeloEmitente(carga.Codigo))
                    return new JsonpResult(false, true, "Existem notas fiscais canceladas pelo emitente na carga, por favor remova as notas cancelada antes de emitir os documentos.");

                string retornoVerificarOperador = serCarga.VerificarOperadorPodeConfigurarCarga(this.Usuario, carga, TipoServicoMultisoftware);
                if (!string.IsNullOrWhiteSpace(retornoVerificarOperador))
                    return new JsonpResult(false, true, retornoVerificarOperador);

                if (carga.EmitindoCTes)
                    return new JsonpResult(false, true, "A emissão dos CT-es já está sendo realizada.");

                if (status != "S" && status != "R")
                    return new JsonpResult(false, true, "O status para emissão dos CT-es é inválido.");

                if (carga.PossuiPendencia)
                {
                    carga.PossuiPendencia = false;
                    carga.problemaCTE = false;
                    carga.EmitindoNFeRemessa = true;
                    carga.problemaEmissaoNFeRemessa = false;
                    carga.MotivoPendencia = "";
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte in cargaCTes)
                {
                    AtualizarImpostosIBSCBS(cargaCte.CTe, carga?.TipoOperacao?.Codigo ?? 0, unitOfWork);

                    var remarkSped = $"Receita de {carga?.TipoOperacao?.Descricao} {cargaCte?.CTe?.Descricao.Split("-")[0].Trim()}";
                    cargaCte.CTe.UsuarioEmissaoCTe = carga?.Operador;
                    repCTe.Atualizar(cargaCte.CTe);
                }

                carga.EmitindoCTes = true;
                repCarga.Atualizar(carga);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, Localization.Resources.Cargas.Carga.AutorizouEmissaoDosDocumentos, unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarComProblemaAverbacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_LiberarAverbacaoRejeitada))
                return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                // Valida informações
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();
                if (carga.PossuiPendencia)
                {
                    carga.LiberarComProblemaAverbacao = true;
                    carga.PossuiPendencia = false;
                    carga.problemaAverbacaoCTe = false;
                    carga.MotivoPendencia = "";
                    repCarga.Atualizar(carga);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Avançou Etapa com Averbação Rejeitada.", unitOfWork);
                }
                unitOfWork.CommitChanges();

                svcHubCarga.InformarCargaAtualizada(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao avançar etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarComProblemaCIOT()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_LiberarAverbacaoRejeitada))
                return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                // Valida informações
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();
                if (carga.PossuiPendencia)
                {
                    carga.LiberadoComProblemaCIOT = true;
                    carga.PossuiPendencia = false;
                    carga.ProblemaIntegracaoCIOT = false;
                    carga.MotivoPendencia = "";
                    repCarga.Atualizar(carga);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Avançou Etapa com CIOT Rejeitado.", unitOfWork);
                }
                unitOfWork.CommitChanges();

                svcHubCarga.InformarCargaAtualizada(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao avançar etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDadosGeracaoCIOT(int codigoCargaCiot, int carga, CancellationToken cancellationToken)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("pt-BR");
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCIOT repositorioCargaCIOT = new Repositorio.Embarcador.Cargas.CargaCIOT(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaInformacoesBancarias repositorioCargaInformacoesBancarias = new Repositorio.Embarcador.Cargas.CargaInformacoesBancarias(unitOfWork, cancellationToken);

                Dominio.Entidades.Global.CargaInformacoesBancarias informacoesBancarias = await repositorioCargaInformacoesBancarias.BuscarPorCargaAsync(carga);
                Dominio.Entidades.Embarcador.Cargas.CargaCIOT cargaCIOT = await repositorioCargaCIOT.BuscarPorCodigoAsync(codigoCargaCiot);

                if (informacoesBancarias == null || cargaCIOT?.CIOT == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do CIOT.");

                var dadosCIOT = new
                {
                    ValorAdiantamento = cargaCIOT.ValorAdiantamento.ToString("N2", cultura),
                    ValorFrete = cargaCIOT.ValorFrete.ToString("N2", cultura),
                    Numero = cargaCIOT.CIOT.Numero,
                    DataVencimento = cargaCIOT.CIOT.DataParaFechamento?.ToString("dd/MM/yyyy"),
                    Agencia = informacoesBancarias.Agencia,
                    ChavePIX = informacoesBancarias.ChavePIX,
                    TipoChavePIX = informacoesBancarias.TipoChavePIX,
                    Conta = informacoesBancarias.Conta,
                    Descricao = informacoesBancarias.Descricao,
                    Ipef = informacoesBancarias.Ipef,
                    TipoInformacaoBancaria = informacoesBancarias.TipoInformacaoBancaria,
                    TipoPagamento = informacoesBancarias.TipoPagamento
                };

                return new JsonpResult(dadosCIOT);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do CIOT.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarLiberacaoCIOT(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
            Repositorio.MDFeCIOT repositorioMDFeCIOT = new Repositorio.MDFeCIOT(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Cargas.CargaMDFe repositorioCargaMDFe = new Repositorio.Embarcador.Cargas.CargaMDFe(unitOfWork, cancellationToken);
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                FormasPagamento? formaPagamentoCIOT = Request.GetNullableEnumParam<FormasPagamento>("FormaPagamento");
                TipoPagamentoMDFe? tipoPagamentoCIOT = Request.GetNullableEnumParam<TipoPagamentoMDFe>("TipoPagamento");
                decimal valorAdiantamento = Request.GetDecimalParam("ValorAdiantamento");
                decimal valorFrete = Request.GetDecimalParam("ValorFrete");
                DateTime? dataVencimento = Request.GetNullableDateTimeParam("DataVencimento");
                string cnpjInstituicaoPagamento = Request.GetStringParam("CNPJInstituicaoPagamento");
                string contaCIOT = Request.GetStringParam("ContaCIOT");
                string agenciaCIOT = Request.GetStringParam("AgenciaCIOT");
                string chavePIXCIOT = Request.GetStringParam("ChavePIXCIOT");
                string numeroCIOT = Request.GetStringParam("CIOT");
                string numeroCIOTAntigo = Request.GetStringParam("CIOTAntigo");
                Dominio.ObjetosDeValor.Enumerador.TipoChavePix tipoChavePIX = Request.GetEnumParam<Dominio.ObjetosDeValor.Enumerador.TipoChavePix>("TipoChavePIX");

                await unitOfWork.StartAsync(cancellationToken);

                Servicos.Embarcador.CIOT.CIOT servicoCIOT = new Servicos.Embarcador.CIOT.CIOT();
                bool retorno = servicoCIOT.GerarCIOTAutomatico(
                    carga,
                    formaPagamentoCIOT,
                    valorFrete,
                    valorAdiantamento,
                    dataVencimento,
                    tipoPagamentoCIOT,
                    cnpjInstituicaoPagamento,
                    agenciaCIOT,
                    contaCIOT,
                    chavePIXCIOT,
                    numeroCIOT,
                    numeroCIOTAntigo,
                    TipoServicoMultisoftware,
                    unitOfWork,
                    tipoChavePIX
                );

                List<Dominio.Entidades.MDFeCIOT> listaMDFeCIOT = await repositorioMDFeCIOT.BuscarPorCargaCIOTAsync(codigoCarga, numeroCIOTAntigo);


                foreach (var obj in listaMDFeCIOT)
                {
                    obj.NumeroCIOT = numeroCIOT;
                    await repositorioMDFeCIOT.AtualizarAsync(obj);
                }


                if (!retorno)
                    throw new ServicoException("Erro ao gerar CIOT");

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message.ToString());
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao avançar etapa.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }


        public async Task<IActionResult> ReaverbarRejeitadas()
        {
            string stringConexao = _conexao.StringConexao;
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                // Busca averbacao
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Atualiza situação da carga
                if (carga.PossuiPendencia)
                {
                    carga.PossuiPendencia = false;
                    carga.problemaAverbacaoCTe = false;
                    carga.MotivoPendencia = "";

                    repCarga.Atualizar(carga);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Reenviou averbações rejeitadas.", unitOfWork);

                Task.Factory.StartNew(() => AsyncAverbacaoRejeitados(codigoCarga, stringConexao));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar averbar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReaverbarPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                if (codigoCarga == 0)
                    throw new ControllerException(Localization.Resources.Cargas.Carga.CargaNaoEncontrada);

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.AverbacaoCTe repositorioAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);

                List<Dominio.Entidades.AverbacaoCTe> averbacoes = repositorioAverbacaoCTe.BuscarPorCargaESituacao(codigoCarga, Dominio.Enumeradores.StatusAverbacaoCTe.Pendente);

                if (averbacoes.Count == 0)
                    throw new ControllerException("Não foi possível encontrar nenhum registro");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga) ?? throw new ControllerException("Não foi possível encontrar o registro.");

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, alteracoes: null, "Reenviou averbações pendentes.", unitOfWork);

                foreach (Dominio.Entidades.AverbacaoCTe averbacao in averbacoes)
                {
                    if ((averbacao.CTe != null && averbacao.CTe.Status != "A"))
                        continue;

                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.AgEmissao;

                    repositorioAverbacaoCTe.Atualizar(averbacao);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, averbacao, alteracoes: null, "Enviou averbação.", unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, averbacao.CTe, alteracoes: null, "Enviou averbação.", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar enviar averbações dos CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarAvervacaoCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Repositorios
                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);

                // Parâmetros
                int codigoAverbacaoCTe;
                int.TryParse(Request.Params("Codigo"), out codigoAverbacaoCTe);

                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                // Busca averbacao
                Dominio.Entidades.AverbacaoCTe averbacao = repAverbacaoCTe.BuscarPorCodigoECarga(codigoAverbacaoCTe, codigoCarga);

                // Valida informações
                if (averbacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                if (averbacao.CTe != null && averbacao.CTe.Status != "C")
                    return new JsonpResult(false, true, "O CT-e dessa averbação não está cancelado");

                if (averbacao.Status != Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso)
                    return new JsonpResult(false, true, "A situação da averbação (" + averbacao.DescricaoStatus + ") não permite essa ação");

                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.AgCancelamento;
                repAverbacaoCTe.Atualizar(averbacao);

                // Envia pro ORACLE
                //Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);

                //svcCTe.CancelarAverbacoesOracle(averbacao.CTe.Empresa.Codigo, averbacao.CTe.Codigo, averbacao.Status, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, averbacao, null, "Solicitou cancelamento da averbação.", unitOfWork);

                if (averbacao.CTe != null)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, averbacao.CTe, null, "Solicitou cancelamento da averbação.", unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar cancelar a averbação do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AverbarCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Repositorios
                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);

                // Parâmetros
                int codigoAverbacaoCTe;
                int.TryParse(Request.Params("Codigo"), out codigoAverbacaoCTe);

                int codigoCarga;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                // Busca averbacao
                Dominio.Entidades.AverbacaoCTe averbacao = repAverbacaoCTe.BuscarPorCodigoECarga(codigoAverbacaoCTe, codigoCarga);

                // Valida informações
                if (averbacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro");

                if (averbacao.CTe != null && averbacao.CTe.Status != "A")
                    return new JsonpResult(false, true, "O CT-e dessa averbação não está autorizado");

                if (averbacao.Status != Dominio.Enumeradores.StatusAverbacaoCTe.Pendente && averbacao.Status != Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao)
                    return new JsonpResult(false, true, "A situação da averbação (" + averbacao.DescricaoStatus + ") não permite essa ação");

                // Envia pro ORACLE
                //Servicos.CTe svcCTe = new Servicos.CTe(unidadeTrabalho);
                //svcCTe.EmitirAverbacoesOracle(averbacao.CTe.Empresa.Codigo, averbacao.CTe.Codigo, averbacao.Status, unitOfWork);

                unitOfWork.Start();

                averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.AgEmissao;
                repAverbacaoCTe.Atualizar(averbacao);

                VerificaAverbacao(averbacao.Carga, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, averbacao, null, "Enviou averbação.", unitOfWork);

                if (averbacao.CTe != null)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, averbacao.CTe, null, "Enviou averbação.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao tentar averbar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracaoAverbacao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unidadeDeTrabalho);


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.AverbacaoCTe integracao = repAverbacaoCTe.BuscarPorCodigo(codigo);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count());

                var retorno = (from obj in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracaoAverbacao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unidadeDeTrabalho);

                Dominio.Entidades.AverbacaoCTe integracao = repAverbacaoCTe.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta da Averbação " + integracao.Carga.CodigoCargaEmbarcador + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracaoCancelamentoAverbacao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unidadeDeTrabalho);


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.AverbacaoCTe integracao = repAverbacaoCTe.BuscarPorCodigo(codigo);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacaoCancelamento.Count());

                var retorno = (from obj in integracao.ArquivosTransacaoCancelamento.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracaoCancelamentoAverbacao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unidadeDeTrabalho);

                Dominio.Entidades.AverbacaoCTe integracao = repAverbacaoCTe.BuscarPorCodigoArquivoCancelamento(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacaoCancelamento.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta da Averbação " + integracao.Carga.CodigoCargaEmbarcador + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ReemitirCTesRejeitadosOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoOcorrencia = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargasCTesComplementoInfo = repositorioCargaCTeComplementoInfo.BuscarCTesRejeitadosPorOcorrencia(codigoOcorrencia);

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);


                int ctesReenviadosComSucesso = 0;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargasCTesComplementoInfo)
                {
                    if (cargaCTeComplementoInfo.CargaCTeComplementado?.Carga.PossuiPendencia ?? false)
                    {
                        cargaCTeComplementoInfo.CargaCTeComplementado.Carga.PossuiPendencia = false;
                        cargaCTeComplementoInfo.CargaCTeComplementado.Carga.problemaCTE = false;
                        cargaCTeComplementoInfo.CargaCTeComplementado.Carga.MotivoPendencia = "";
                        repositorioCarga.Atualizar(cargaCTeComplementoInfo.CargaCTeComplementado.Carga);
                    }

                    var remarkSped = $"Receita de {cargaCTeComplementoInfo?.CargaOcorrencia?.TipoOcorrencia?.Descricao} {cargaCTeComplementoInfo?.Descricao.Split("-")[0].Trim()}";

                    if (cargaCTeComplementoInfo.CTe == null)
                        continue;

                    string retorno = "";
                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                        retorno = EmitirCTe(cargaCTeComplementoInfo.CTe.Codigo, unitOfWork);

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                        retorno = EmitirNFSe(cargaCTeComplementoInfo.CTe.Codigo, unitOfWork);

                    if (string.IsNullOrWhiteSpace(retorno))
                    {
                        cargaCTeComplementoInfo.CTe.UsuarioEmissaoCTe = Usuario;
                        repCTe.Atualizar(cargaCTeComplementoInfo.CTe);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTeComplementoInfo.CargaCTeComplementado, null, "Enviou para Emissão.", unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTeComplementoInfo.CTe, null, "Enviou para Emissão.", unitOfWork);
                        ctesReenviadosComSucesso += 1;
                        unitOfWork.CommitChanges();
                    }
                    else
                        unitOfWork.Rollback();
                }

                return new JsonpResult(true, true, $"{ctesReenviadosComSucesso} de {cargasCTesComplementoInfo.Count} CT-e(s) foram reenviado(s) com sucesso!");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar os CT-es para emissão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ReemitirTodosCTesRejeitadosOcorrencias()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

                unitOfWork.Start();

                int registrosAtualizados = repositorioCargaCTeComplementoInfo.DefinirCtesComplementaresRejeitadosParaReemissao();

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar os CT-es para emissão.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EmitirNovamente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                int codigoCTe;
                int.TryParse(Request.Params("CodigoCTe"), out codigoCTe);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCte.BuscarPorCTe(codigoCTe);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                if ((configuracaoGeralCarga?.AtualizarDataEmissaoParaDataAtualQuandoReemitirCTeRejeitado ?? false) && cargaCTe.CTe.CodStatusProtocolo != "204")
                    cargaCTe.CTe.DataEmissao = DateTime.Now;

                if (cargaCTe != null && cargaCTe.Carga.PossuiPendencia)
                {
                    cargaCTe.Carga.PossuiPendencia = false;
                    cargaCTe.Carga.problemaCTE = false;
                    cargaCTe.Carga.MotivoPendencia = "";
                    repCarga.Atualizar(cargaCTe.Carga);
                }

                var remarkSped = $"Receita de {cargaCTe?.CargaCTeComplementoInfo?.CargaOcorrencia?.TipoOcorrencia?.Descricao} {cargaCTe?.CargaCTeComplementoInfo?.Descricao.Split("-")[0].Trim()}";

                if (cargaCTe != null)
                {
                    if (codigoCTe > 0)
                    {
                        string retorno = "";
                        if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                            retorno = EmitirCTe(codigoCTe, unitOfWork);

                        if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                            retorno = EmitirNFSe(codigoCTe, unitOfWork);

                        cargaCTe.CTe.UsuarioEmissaoCTe = cargaCTe.Carga?.Operador;
                        repCTe.Atualizar(cargaCTe.CTe);

                        if (string.IsNullOrWhiteSpace(retorno))
                        {
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe, null, "Enviou para Emissão.", unitOfWork);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe.CTe, null, "Enviou para Emissão.", unitOfWork);
                            return new JsonpResult(true);
                        }
                        else
                            return new JsonpResult(false, true, retorno);
                    }
                    else
                    {
                        return new JsonpResult(false, "O Documento informado não foi localizado");
                    }
                }
                else
                {
                    if (codigoCTe > 0)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                        if (cte != null)
                        {
                            string retorno = "";
                            if (cte.ModeloDocumentoFiscal.Numero == "39") //NFSe
                                retorno = EmitirNFSe(codigoCTe, unitOfWork);
                            else
                                retorno = EmitirCTe(codigoCTe, unitOfWork);

                            cte.UsuarioEmissaoCTe = Usuario;
                            repCTe.Atualizar(cte);

                            if (string.IsNullOrWhiteSpace(retorno))
                            {
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Enviou para Emissão.", unitOfWork);
                                return new JsonpResult(true);
                            }
                            else
                                return new JsonpResult(false, true, retorno);
                        }
                        else
                        {
                            return new JsonpResult(false, "O Documento informado não foi localizado");
                        }

                    }
                    else
                    {
                        return new JsonpResult(false, "O Documento informado não foi localizado");
                    }

                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir o Documento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SincronizarDocumentoEmProcessamento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe = Request.GetIntParam("CodigoCTe");

                Servicos.Embarcador.Carga.CargaCTe servicoCargaCTe = new Servicos.Embarcador.Carga.CargaCTe(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                string retorno = await servicoCargaCTe.SincronizarDocumentoEmProcessamentoAsync(codigoCTe, Auditado, TipoServicoMultisoftware, cancellationToken);

                if (!string.IsNullOrWhiteSpace(retorno.ToString()))
                    return new JsonpResult(false, true, retorno.ToString());

                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCargaCTe.BuscarCargaPorCodigoCTeAsync(codigoCTe, cancellationToken);

                if (carga != null)
                    await servicoCargaCTe.RetirarPendenciaCargaAsync(carga, cancellationToken);

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar sincronizar o Documento.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SincronizarLoteDocumentoEmProcessamento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                Servicos.Embarcador.Carga.CargaCTe servicoCargaCTe = new Servicos.Embarcador.Carga.CargaCTe(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.SincronizacaoLoteResultado resultado = servicoCargaCTe.SincronizarLoteDocumentoEmProcessamento(codigoCarga, null, Auditado, TipoServicoMultisoftware);

                if (!resultado.Sucesso)
                    return new JsonpResult(false, true, resultado.Mensagem);

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar sincronizar o Documento.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> DesvincularCTeEGerarCopia()
        {
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                int codigoCTe;
                int.TryParse(Request.Params("CodigoCTe"), out codigoCTe);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCte.BuscarPorCTe(codigoCTe);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                if (cargaCTe != null)
                {
                    if (codigoCTe > 0)
                    {
                        string mensagemErro = "";
                        if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                        {
                            Servicos.Embarcador.CTe.CTe.GerarCTeCopia(out mensagemErro, cargaCTe, cargaCTe.Codigo, ConfiguracaoEmbarcador, true, Auditado, unitOfWork);
                        }
                        else
                        {
                            mensagemErro = "Ação não permitida para o tipo de documento selecionado.";
                        }

                        if (string.IsNullOrWhiteSpace(mensagemErro))
                        {
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe.Carga, null, $"Documento nro {cargaCTe.CTe.Numero} desvinculado da carga.", unitOfWork);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe.CTe, null, "Gerada cópia do documento na carga.", unitOfWork);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe.CTe, null, "Documento desvinculado da carga.", unitOfWork);
                            return new JsonpResult(true);
                        }
                        else
                            return new JsonpResult(false, true, mensagemErro);
                    }
                    else
                    {
                        return new JsonpResult(false, "O Documento informado não foi localizado");
                    }
                }
                else
                {
                    return new JsonpResult(false, "O Documento informado não foi localizado");
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar sincronizar o Documento.");
            }
        }

        public async Task<IActionResult> InutilizarCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                int.TryParse(Request.Params("CodigoCTe"), out int codigoCTe);
                string justificativa = Request.Params("Justificativa");

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte != null)
                {
                    string retorno = InutilizarCTe(cte.Codigo, justificativa, unitOfWork);
                    if (string.IsNullOrWhiteSpace(retorno))
                    {
                        cte.UsuarioEmissaoCTe = Usuario;
                        repCTe.Atualizar(cte);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Inutilizou CT-e.", unitOfWork);
                        return new JsonpResult(true);
                    }
                    else
                        return new JsonpResult(false, true, retorno);
                }
                else
                {
                    return new JsonpResult(false, "O CT-e informado não foi localizado");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar inutilizar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> NaoEnviarParaMercante()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
                Repositorio.Embarcador.Contabeis.JustificativaMercante repJustificativa = new Repositorio.Embarcador.Contabeis.JustificativaMercante(unidadeTrabalho);

                int codigoCTe = Request.GetIntParam("CodigoCTe");
                int codigoJustificativa = Request.GetIntParam("JustificativaMercante");

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe, true);
                Dominio.Entidades.Embarcador.Contabeis.JustificativaMercante justificativaMercante = repJustificativa.BuscarPorCodigo(codigoJustificativa);

                if (cte == null)
                    return new JsonpResult(false, true, "O CT-e informado não foi localizado.");

                if (justificativaMercante == null)
                    return new JsonpResult(false, false, "A Justificativa Mercante informada não foi localizado.");

                cte.NaoEnviarParaMercante = true;
                cte.JustificativaMercante = justificativaMercante;

                repCTe.Atualizar(cte, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Informou o CT-e para não enviar ao mercante.", unidadeTrabalho);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao tentar cancelar o CT-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> CancelarCTe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);

                int codigoCTe = Request.GetIntParam("CodigoCTe");
                int codigoCarga = Request.GetIntParam("CodigoCarga");

                string justificativa = Request.GetStringParam("Justificativa");

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return new JsonpResult(false, true, "O CT-e informado não foi localizado.");


                if (codigoCarga > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                    if (carga?.CargaBloqueadaParaEdicaoIntegracao ?? false)
                        return new JsonpResult(false, false, "Não é possivel cancelar carga bloqueada.");
                    if (carga.Empresa != null && !Usuario.Empresas.Contains(carga.Empresa) && Usuario.LimitarOperacaoPorEmpresa)
                        return new JsonpResult(false, true, "Você não possui permissão para fazer operações nessa Empresa da carga.");

                }

                string retorno = string.Empty;

                if (CancelarCTe(out retorno, codigoCTe, justificativa, unidadeTrabalho))
                {
                    cte.UsuarioEmissaoCTe = Usuario;
                    repCTe.Atualizar(cte);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Cancelou CT-e.", unidadeTrabalho);
                    return new JsonpResult(true);
                }
                else
                    return new JsonpResult(false, true, retorno);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao tentar cancelar o CT-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EmitirCargaCTeRejeição()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoCargaCTe;
                int.TryParse(Request.Params("CodigoCargaCTe"), out codigoCargaCTe);

                Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork); ;
                dynamic dynCTe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CTe"));

                if (codigoCargaCTe > 0)
                {
                    Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTe.CTe;

                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoes = BuscarPermissoesCTeRejeitado();

                    if (cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada || cargaCTe.CTe.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.ContingenciaFSDA)
                    {
                        if (cargaCTe.Carga.PossuiPendencia)
                        {
                            cargaCTe.Carga.PossuiPendencia = false;
                            cargaCTe.Carga.problemaCTE = false;
                            cargaCTe.Carga.MotivoPendencia = "";
                            repCarga.Atualizar(cargaCTe.Carga);
                        }

                        Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = serCTe.ConverterDynamicParaCTe(dynCTe, unitOfWork);

                        cte.Initialize();

                        int numeroCFOPOriginal = cte.CFOP.CodigoCFOP;
                        int numeroCFOPNovo = cteIntegracao.CFOP;

                        serCTe.SalvarDadosCTe(ref cte, cteIntegracao, cte.SituacaoCTeSefaz, permissoes, this.Usuario, unitOfWork, cargaCTe?.Carga?.CargaSVMTerceiro ?? false, Auditado);

                        List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoesCTe = cte.GetChanges();

                        if (alteracoesCTe != null && alteracoesCTe.Any(x => x.Propriedade == "CFOP"))
                        {
                            Dominio.Entidades.Auditoria.HistoricoPropriedade alteracaoCFOP = alteracoesCTe.Where(x => x.Propriedade == "CFOP").FirstOrDefault();
                            alteracaoCFOP.De = numeroCFOPOriginal.ToString();
                            alteracaoCFOP.Para = numeroCFOPNovo.ToString();
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe, alteracoesCTe, "Emitiu Documento Rejeitado.", unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe.CTe, alteracoesCTe, "Emitiu Documento Rejeitado.", unitOfWork);

                        unitOfWork.CommitChanges();
                        string retorno = "";
                        if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                            EmitirCTe(cte.Codigo, unitOfWork);

                        if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                            EmitirNFSe(cte.Codigo, unitOfWork);

                        if (string.IsNullOrWhiteSpace(retorno))
                            return new JsonpResult(true);
                        else
                            return new JsonpResult(false, true, retorno);
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "A atual situação do CT-e (" + cargaCTe.CTe.DescricaoStatus + ") não permite que ele seje re-enviado.");
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "O CT-e Informado não é válido. ");
                }
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
                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarCargaCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Servicos.Embarcador.Carga.RateioCTe serRateioCTe = new Servicos.Embarcador.Carga.RateioCTe(unitOfWork);
            Servicos.Embarcador.Carga.ComponetesFrete serComponentesFrete = new Servicos.Embarcador.Carga.ComponetesFrete(unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            try
            {
                unitOfWork.Start();

                int codigoCargaCTe;
                int.TryParse(Request.Params("CodigoCargaCTe"), out codigoCargaCTe);

                Servicos.Embarcador.CTe.CTe serCTe = new Servicos.Embarcador.CTe.CTe(unitOfWork); ;
                dynamic dynCTe = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CTe"));

                if (codigoCargaCTe <= 0)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "O CT-e Informado não é válido.");
                }

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTe.CTe;
                decimal valorInicialAReceber = cte.ValorAReceber;
                decimal valorInicialICMS = cte.ValorICMS;

                if (cte.ValorAReceber != cargaCTe.CTe.ValorAReceber || cte.ValorFrete != cargaCTe.CTe.ValorFrete || cte.ValorICMS != cargaCTe.CTe.ValorICMS)
                {
                    if (cargaCTe.Carga.TabelaFrete != null && cargaCTe.Carga.TabelaFrete.TipoCalculo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorCarga)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Não é possível alterar um CT-e em que os valores do frete sejam calculados por documento emitido.");
                    }
                }

                if (cargaCTe.CTe.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmDigitacao)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "A atual situação do documento (" + cargaCTe.CTe.DescricaoStatus + ") não permite que ele seja atualizado.");
                }

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoes = null;

                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros)
                    permissoes = BuscarPermissoesOutrosDocumentosEmDigitacaoCarga();
                else
                    permissoes = BuscarPermissoesCTeEmDigitacaoCarga();

                Dominio.ObjetosDeValor.Embarcador.CTe.CTe cteIntegracao = serCTe.ConverterDynamicParaCTe(dynCTe, unitOfWork);

                if (cargaCTe.Carga.TabelaFrete != null && cargaCTe.Carga.TabelaFrete.TipoCalculo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoTabelaFrete.PorCarga)
                {
                    if (cte.ValorAReceber != cteIntegracao.ValorFrete.ValorTotalAReceber ||
                        cte.ValorFrete != cteIntegracao.ValorFrete.FreteProprio ||
                        cte.ValorICMS != cteIntegracao.ValorFrete.ICMS.ValorICMS ||
                        cte.ValorPrestacaoServico != cteIntegracao.ValorFrete.ValorPrestacaoServico)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "Não é possível alterar um CT-e em que os valores do frete sejam calculados por documento emitido.");
                    }
                }

                cte.Initialize();
                serCTe.SalvarDadosCTe(ref cte, cteIntegracao, cte.SituacaoCTeSefaz, permissoes, this.Usuario, unitOfWork, cargaCTe?.Carga?.CargaSVMTerceiro ?? false, Auditado);

                bool alterouComponente = serComponentesFrete.MudarComplementoDaCargaCTe(cargaCTe, cteIntegracao.ValorFrete.ComponentesAdicionais, unitOfWork);

                if (valorInicialAReceber != cte.ValorAReceber || valorInicialICMS != cte.ValorICMS || alterouComponente)
                    serRateioCTe.AtualizarRateiosPorCargaCTe(cargaCTe, unitOfWork, TipoServicoMultisoftware);

                dynamic retorno = serCarga.ObterDetalhesDaCarga(cargaCTe.Carga, TipoServicoMultisoftware, unitOfWork);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoesCTe = cte.GetChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe, alteracoesCTe, "Alterou o CT-e.", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe.CTe, alteracoesCTe, "Alterou o CT-e pela carga " + cargaCTe.Carga.Descricao + ".", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(retorno);
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
                return new JsonpResult(false, "Ocorreu uma falha ao tentar atualizar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarContainer()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCargaCTe;
                int.TryParse(Request.Params("CodigoCargaCTe"), out codigoCargaCTe);
                int.TryParse(Request.Params("Container"), out int codigoContainer);
                int.TryParse(Request.Params("CodigoCTe"), out int codigoCTe);

                string lacreContainerUm = Request.Params("LacreContainerUm");
                string lacreContainerDois = Request.Params("LacreContainerDois");
                string lacreContainerTres = Request.Params("LacreContainerTres");

                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Pedidos.Container repContainer = new Repositorio.Embarcador.Pedidos.Container(unitOfWork);
                Repositorio.ContainerCTE repContainerCTE = new Repositorio.ContainerCTE(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.CTe.CTeContainerDocumento repCTeContainerDocumento = new Repositorio.Embarcador.CTe.CTeContainerDocumento(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscalEletronica = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCTe.CTe;
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiroPedidoPorCarga(cargaCTe.Carga.Codigo);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(cargaCTe.Carga.Codigo);
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

                if (codigoContainer > 0)
                    pedido.Container = repContainer.BuscarPorCodigo(codigoContainer);
                if (!string.IsNullOrWhiteSpace(lacreContainerDois))
                    pedido.LacreContainerDois = lacreContainerDois;
                if (!string.IsNullOrWhiteSpace(lacreContainerTres))
                    pedido.LacreContainerTres = lacreContainerTres;
                if (!string.IsNullOrWhiteSpace(lacreContainerUm))
                    pedido.LacreContainerUm = lacreContainerUm;

                repPedido.Atualizar(pedido);

                Dominio.Entidades.ContainerCTE containerCTE = new Dominio.Entidades.ContainerCTE()
                {
                    Container = repContainer.BuscarPorCodigo(codigoContainer),
                    CTE = cte,
                    DataPrevista = cte.DataPrevistaContainer,
                    Lacre1 = lacreContainerUm,
                    Lacre2 = lacreContainerDois,
                    Lacre3 = lacreContainerTres,
                    Numero = pedido.Container?.Numero ?? ""
                };
                repContainerCTE.Inserir(containerCTE);

                if (containerCTE != null && containerCTE.Container != null)
                {
                    containerCTE.Numero = containerCTE.Container.Numero;
                    repContainerCTE.Inserir(containerCTE);

                    if (cte.Documentos != null && cte.Documentos.Count > 0)
                    {
                        foreach (Dominio.Entidades.DocumentosCTE nota in cte.Documentos)
                        {
                            Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento containerDocumento = new Dominio.Entidades.Embarcador.CTe.CTeContainerDocumento()
                            {
                                DocumentosCTE = nota,
                                Chave = nota.ChaveNFE,
                                ContainerCTE = containerCTE,
                                Numero = !string.IsNullOrEmpty(nota.Numero) ? nota.Numero : "1",
                                Serie = !string.IsNullOrEmpty(nota.Serie) ? nota.Serie : "1",
                                TipoDocumento = !string.IsNullOrEmpty(nota.ChaveNFE) && nota.ChaveNFE.Length == 44 ? Dominio.Enumeradores.TipoDocumentoCTe.NFe : Dominio.Enumeradores.TipoDocumentoCTe.NF,
                                UnidadeMedidaRateada = 0
                            };

                            if (!string.IsNullOrWhiteSpace(containerDocumento.Chave))
                                containerDocumento.XMLNotaFiscal = repXMLNotaFiscalEletronica.BuscarPorChave(containerDocumento.Chave);

                            repCTeContainerDocumento.Inserir(containerDocumento);
                        }
                    }
                }

                Servicos.Log.TratarErro($"Atualização de CAR_CARGA_INTEGRADA_EMBARCADOR na carga {carga.Codigo} para false pelo CargaCTeControlle", "AtualizacaoCargaIntegradaEmbarcador");

                carga.CargaIntegradaEmbarcador = false;
                repCarga.Atualizar(carga);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, Localization.Resources.Cargas.Carga.AlterouCargaParaPendenteDeIntegracaoDevidoTerInformadoContainer, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, string.Format(Localization.Resources.Cargas.Carga.AlterouContainterPelaCarga, cargaCTe.Carga.Descricao), unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, string.Format(Localization.Resources.Cargas.Carga.AlterouContainterPelaCarga, cargaCTe.Carga.Descricao), unitOfWork);

                unitOfWork.CommitChanges();

                string msgRetorno = "";
                cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte.Containers != null && cte.Containers.Count > 0)
                {
                    if (!GerarCartaCorrecaoContainer(cte, unitOfWork, out msgRetorno))
                        return new JsonpResult(false, "Falha ao processar a carta de correção. " + msgRetorno);
                    else
                        return new JsonpResult(true, true, "Sucesso");
                }
                else
                    return new JsonpResult(true, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar alterar o container do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarObservacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCargaCTe;
                int.TryParse(Request.Params("CodigoCargaCTe"), out codigoCargaCTe);
                int.TryParse(Request.Params("CodigoCTe"), out int codigoCTe);

                string observacao = Request.Params("Observacao");

                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(cargaCTe.CTe.Codigo, true);

                cte.ObservacoesGerais = observacao;

                repCTe.Atualizar(cte, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, "Alterou a observação pela carga " + cargaCTe.Carga.Descricao + ".", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe, "Alterou a observação do CT-e.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar alterar o container do CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EnviarNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoNFe = 0;
                int.TryParse(Request.Params("Codigo"), out codigoNFe);
                int codigoCarga = Request.GetIntParam("CodigoCarga");

                Repositorio.Embarcador.NotaFiscal.NotaFiscal repNFe = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal nfe = repNFe.BuscarPorCodigo(codigoNFe);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                unitOfWork.Start();

                nfe.Status = Dominio.Enumeradores.StatusNFe.Emitido;
                repNFe.Atualizar(nfe);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfe, null, "Reenviou NF-e.", unitOfWork);

                carga.MotivoPendencia = string.Empty;
                carga.PossuiPendencia = false;
                carga.EmitindoNFeRemessa = false;
                carga.problemaEmissaoNFeRemessa = false;
                repCarga.Atualizar(carga);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfe, null, "Reenvio NF-e.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir o NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarDocumentoCargaCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.CargaPreCte repCargaPreCte = new Repositorio.Embarcador.Cargas.CargaPreCte(unitOfWork);

            try
            {

                int codigoCarga = int.Parse(Request.Params("Carga"));

                List<Dominio.Entidades.Embarcador.Cargas.CargaPreCte> dadosCte = repCargaPreCte.BuscarDocumentosPorCarga(codigoCarga);

                if (dadosCte == null)
                    return new JsonpResult(false, "Dados da carga não encontrados");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Numero CTe", "NumeroCte", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Chave CTe", "ChaveCte", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("StatusDocumento", "StatusDocumento", 5, Models.Grid.Align.center, true);
                grid.setarQuantidadeTotal(dadosCte.Count);

                var lista = (from obj in dadosCte
                             select new
                             {
                                 obj.Codigo,
                                 obj.ChaveCte,
                                 obj.NumeroCte,
                                 StatusDocumento = obj.StatusDocumento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusDocumento.OK ? "OK" : "NÃO OK",
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

        [AllowAuthenticate]
        public async Task<IActionResult> CalcularValorComponenteCTes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repositorioCargaCTeComponenteFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);

                int codigoComponenteFrete = Request.GetIntParam("CodigoComponenteFrete");
                dynamic cargaCTesSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CTes"));

                List<int> codigosCargaCTe = new List<int>();
                foreach (dynamic cargaCTe in cargaCTesSelecionados)
                {
                    if (((string)cargaCTe.Codigo).ToInt() > 0)
                        codigosCargaCTe.Add(((string)cargaCTe.Codigo).ToInt());
                }

                decimal valorComponenteExistente = codigosCargaCTe.Count > 0 ? repositorioCargaCTeComponenteFrete.BuscarValorCompenenteFretePorCTeEComponenteFrete(codigosCargaCTe, codigoComponenteFrete) : 0m;

                return new JsonpResult(valorComponenteExistente.ToString("N2"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(0m, false, "Ocorreu uma falha ao calcular o valor do componente dos CT-es selecionados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarAnexosContribuinte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaCTe = Request.GetIntParam("CargaCTe");

                Repositorio.Embarcador.Cargas.ContribuinteCargaCTeAnexo repContribuinteCargaCTeAnexo = new Repositorio.Embarcador.Cargas.ContribuinteCargaCTeAnexo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ContribuinteCargaCTeAnexo> anexos = repContribuinteCargaCTeAnexo.BuscarPorCargaCTe(codigoCargaCTe);

                var listaDinamicaAnexos = (
                                            from anexo in anexos
                                            select new
                                            {
                                                anexo.Codigo,
                                                anexo.Descricao,
                                                anexo.NomeArquivo
                                            }
                                        ).ToList();

                return new JsonpResult(new
                {
                    Anexos = listaDinamicaAnexos
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarDocumentoContribuinte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int codigoCargaCTe = Request.GetIntParam("Codigo");
                bool todosAprovados = false;

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                unitOfWork.Start();

                cargaCTe.SituacaoDocumentoContribuinte = SituacaoDocumentoContribuinte.Aprovado;

                repCargaCTe.Atualizar(cargaCTe);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe, null, $"Documento do Contribunte Aprovado", unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCTe.Carga;
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCtes = repCargaCTe.BuscarPorCarga(carga.Codigo);

                if (cargaCtes.TrueForAll(o => o.SituacaoDocumentoContribuinte == SituacaoDocumentoContribuinte.Aprovado))
                {
                    carga.PossuiPendencia = false;
                    carga.PendenciaDocumentoTransportador = false;
                    carga.MotivoPendencia = string.Empty;

                    repCarga.Atualizar(carga);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, $"Todos os documentos do Contribunte Aprovados.", unitOfWork);

                    todosAprovados = true;
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    TodosDocumentosAprovados = todosAprovados
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar o Documento do contribuinte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprovarDocumentoContribuinte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                int codigoCargaCTe = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                unitOfWork.Start();

                cargaCTe.SituacaoDocumentoContribuinte = SituacaoDocumentoContribuinte.Rejeitado;

                repCargaCTe.Atualizar(cargaCTe);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe, null, $"Documento do Contribunte Reprovado.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar o Documento do contribuinte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterCargaCTeOcorrencia(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidosXMLsNotaFiscalCTe, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscalPorCargaCTe = (from o in cargaPedidosXMLsNotaFiscalCTe where o.CargaCTe.Codigo == cargaCTe.Codigo select o.PedidoXMLNotaFiscal).ToList();
            List<int> codigosCargaPedidos = (from o in pedidosXMLNotaFiscalPorCargaCTe select o.CargaPedido.Codigo).Distinct().ToList();
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedidoPorCargaCTe = (from o in cargaEntregaPedidos where !o.CargaEntrega.Coleta && codigosCargaPedidos.Contains(o.CargaPedido.Codigo) select o).FirstOrDefault();
            string dataEntrada = "";
            string dataEntradaRaio = "";
            string dataSaidaRaio = "";

            if (cargaEntregaPedidoPorCargaCTe != null)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargaEntregaPedidoPorCargaCTe.CargaEntrega;

                if (cargaEntrega.DataEntradaRaio.HasValue && cargaEntrega.DataSaidaRaio.HasValue)
                {
                    Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaEntregaPedidoPorCargaCTe?.CargaPedido?.Pedido;

                    dataEntrada = (pedido.PrevisaoEntrega.HasValue && (pedido.PrevisaoEntrega > cargaEntrega.DataEntradaRaio)) ? pedido.PrevisaoEntrega.Value.ToString("dd/MM/yyyy HH:mm") : cargaEntrega.DataEntradaRaio.Value.ToString("dd/MM/yyyy HH:mm");
                    dataEntradaRaio = cargaEntrega.DataEntradaRaio.Value.ToString("dd/MM/yyyy HH:mm");
                    dataSaidaRaio = cargaEntrega.DataSaidaRaio.Value.ToString("dd/MM/yyyy HH:mm");
                }
            }

            return new
            {
                cargaCTe.Codigo,
                CodigoCargaCTe = cargaCTe.Codigo,
                CodigoCTE = cargaCTe.CTe?.Codigo ?? 0,
                DescricaoTipoServico = cargaCTe.CTe?.DescricaoTipoServico ?? cargaCTe.PreCTe.DescricaoTipoServico,
                NumeroModeloDocumentoFiscal = cargaCTe.CTe?.ModeloDocumentoFiscal.Numero ?? "",
                AbreviacaoModeloDocumentoFiscal = cargaCTe.CTe?.ModeloDocumentoFiscal.Abreviacao ?? "Pré CT-e",
                CodigoEmpresa = cargaCTe.CTe?.Empresa.Codigo ?? cargaCTe.PreCTe.Empresa.Codigo,
                Numero = cargaCTe.CTe?.Numero.ToString() ?? "",
                DataEmissao = cargaCTe.CTe?.DataEmissao ?? cargaCTe.PreCTe.DataEmissao,
                SituacaoCTe = cargaCTe.CTe?.Status ?? "",
                Serie = cargaCTe.CTe?.Serie.Numero.ToString() ?? "",
                DescricaoTipoPagamento = cargaCTe.CTe?.DescricaoTipoPagamento ?? "",
                Remetente = cargaCTe.CTe != null ? (cargaCTe.CTe.Remetente?.Cliente?.Descricao ?? cargaCTe.CTe.Remetente?.Descricao ?? string.Empty) : cargaCTe.PreCTe.Remetente.Cliente.Descricao,
                Destinatario = cargaCTe.CTe != null ? (cargaCTe.CTe.Destinatario?.Cliente?.Descricao ?? cargaCTe.CTe.Destinatario?.Descricao ?? string.Empty) : cargaCTe.PreCTe.Destinatario.Cliente.Descricao,
                Destino = cargaCTe.CTe?.LocalidadeTerminoPrestacao.DescricaoCidadeEstado ?? cargaCTe.PreCTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                ValorFrete = cargaCTe.CTe?.ValorAReceber.ToString("n2") ?? cargaCTe.PreCTe.ValorAReceber.ToString("n2"),
                Aliquota = cargaCTe.CTe != null ? (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cargaCTe.CTe.AliquotaICMS.ToString("n2") : cargaCTe.CTe.AliquotaISS.ToString("n4")) : cargaCTe.PreCTe.AliquotaICMS.ToString("n2"),
                Status = cargaCTe.CTe?.DescricaoStatus ?? "",
                NumeroNotas = string.Join(", ", (from o in pedidosXMLNotaFiscalPorCargaCTe select o.XMLNotaFiscal.Numero).Distinct().ToList()),
                RetornoSefaz = cargaCTe.CTe != null ? (!string.IsNullOrWhiteSpace(cargaCTe.CTe.MensagemRetornoSefaz) ? cargaCTe.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(cargaCTe.CTe.MensagemRetornoSefaz) : "" : "") : "",
                DataEntrada = dataEntrada,
                DataEntradaRaio = dataEntradaRaio,
                DataSaidaRaio = dataSaidaRaio,
                CTeGlobalizado = cargaCTe.CTe != null ? cargaCTe.CTe.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim : false,
                DT_RowColor = cargaCTe.CTe != null ? ((cargaCTe.CTe.Status == "R" ? "rgba(193, 101, 101, 1)" : ((cargaCTe.CTe.Status == "C" || cargaCTe.CTe.Status == "I" || cargaCTe.CTe.Status == "D" || cargaCTe.CTe.Status == "Z") ? "#777" : ""))) : "",
                DT_FontColor = cargaCTe.CTe != null ? (((cargaCTe.CTe.Status == "R" || cargaCTe.CTe.Status == "C" || cargaCTe.CTe.Status == "I" || cargaCTe.CTe.Status == "D" || cargaCTe.CTe.Status == "Z") ? "#FFFFFF" : "")) : "",
                CodigoNotaFiscal = 0
            };
        }

        private List<dynamic> ObterConsultaCargaCTeOcorrenciaPorNota(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargasCTe, List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe> cargaPedidosXMLsNotaFiscalCTe, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos, ref int totalRegistros)
        {
            List<dynamic> retorno = new List<dynamic>();
            totalRegistros = 0;

            foreach (var cargaCTe in cargasCTe)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidosXMLNotaFiscalPorCargaCTe = (from o in cargaPedidosXMLsNotaFiscalCTe where o.CargaCTe.Codigo == cargaCTe.Codigo select o.PedidoXMLNotaFiscal).ToList();

                foreach (var pedidoXMLNotaFiscalPorCargaCTe in pedidosXMLNotaFiscalPorCargaCTe)
                {
                    totalRegistros++;

                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedidoPorCargaCTe = (from o in cargaEntregaPedidos where !o.CargaEntrega.Coleta && o.CargaPedido.Codigo == pedidoXMLNotaFiscalPorCargaCTe.CargaPedido.Codigo select o).FirstOrDefault();
                    string dataEntrada = "";
                    string dataEntradaRaio = "";
                    string dataSaidaRaio = "";

                    if (cargaEntregaPedidoPorCargaCTe != null)
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = cargaEntregaPedidoPorCargaCTe.CargaEntrega;

                        if (cargaEntrega.DataEntradaRaio.HasValue && cargaEntrega.DataSaidaRaio.HasValue)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaEntregaPedidoPorCargaCTe?.CargaPedido?.Pedido;

                            dataEntrada = (pedido.PrevisaoEntrega.HasValue && (pedido.PrevisaoEntrega > cargaEntrega.DataEntradaRaio)) ? pedido.PrevisaoEntrega.Value.ToString("dd/MM/yyyy HH:mm") : cargaEntrega.DataEntradaRaio.Value.ToString("dd/MM/yyyy HH:mm");
                            dataEntradaRaio = cargaEntrega.DataEntradaRaio.Value.ToString("dd/MM/yyyy HH:mm");
                            dataSaidaRaio = cargaEntrega.DataSaidaRaio.Value.ToString("dd/MM/yyyy HH:mm");
                        }
                    }

                    retorno.Add(new
                    {
                        Codigo = pedidoXMLNotaFiscalPorCargaCTe.XMLNotaFiscal.Codigo,
                        CodigoCargaCTe = cargaCTe.Codigo,
                        CodigoCTE = cargaCTe.CTe?.Codigo ?? 0,
                        DescricaoTipoServico = cargaCTe.CTe?.DescricaoTipoServico ?? cargaCTe.PreCTe.DescricaoTipoServico,
                        NumeroModeloDocumentoFiscal = cargaCTe.CTe?.ModeloDocumentoFiscal.Numero ?? "",
                        AbreviacaoModeloDocumentoFiscal = cargaCTe.CTe?.ModeloDocumentoFiscal.Abreviacao ?? "Pré CT-e",
                        CodigoEmpresa = cargaCTe.CTe?.Empresa.Codigo ?? cargaCTe.PreCTe.Empresa.Codigo,
                        Numero = cargaCTe.CTe?.Numero.ToString() ?? "",
                        DataEmissao = cargaCTe.CTe?.DataEmissao ?? cargaCTe.PreCTe.DataEmissao,
                        SituacaoCTe = cargaCTe.CTe?.Status ?? "",
                        Serie = cargaCTe.CTe?.Serie.Numero.ToString() ?? "",
                        DescricaoTipoPagamento = cargaCTe.CTe?.DescricaoTipoPagamento ?? "",
                        Remetente = cargaCTe.CTe != null ? (cargaCTe.CTe.Remetente?.Cliente?.Descricao ?? cargaCTe.CTe.Remetente?.Descricao ?? string.Empty) : cargaCTe.PreCTe.Remetente.Cliente.Descricao,
                        Destinatario = cargaCTe.CTe != null ? (cargaCTe.CTe.Destinatario?.Cliente?.Descricao ?? cargaCTe.CTe.Destinatario?.Descricao ?? string.Empty) : cargaCTe.PreCTe.Destinatario.Cliente.Descricao,
                        Destino = cargaCTe.CTe?.LocalidadeTerminoPrestacao.DescricaoCidadeEstado ?? cargaCTe.PreCTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                        ValorFrete = cargaCTe.CTe?.ValorAReceber.ToString("n2") ?? cargaCTe.PreCTe.ValorAReceber.ToString("n2"),
                        Aliquota = cargaCTe.CTe != null ? (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe ? cargaCTe.CTe.AliquotaICMS.ToString("n2") : cargaCTe.CTe.AliquotaISS.ToString("n4")) : cargaCTe.PreCTe.AliquotaICMS.ToString("n2"),
                        Status = cargaCTe.CTe?.DescricaoStatus ?? "",
                        NumeroNotas = pedidoXMLNotaFiscalPorCargaCTe.XMLNotaFiscal.Numero,
                        RetornoSefaz = cargaCTe.CTe != null ? (!string.IsNullOrWhiteSpace(cargaCTe.CTe.MensagemRetornoSefaz) ? cargaCTe.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(cargaCTe.CTe.MensagemRetornoSefaz) : "" : "") : "",
                        DataEntrada = dataEntrada,
                        DataEntradaRaio = dataEntradaRaio,
                        DataSaidaRaio = dataSaidaRaio,
                        CTeGlobalizado = cargaCTe.CTe != null ? cargaCTe.CTe.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim : false,
                        DT_RowColor = cargaCTe.CTe != null ? ((cargaCTe.CTe.Status == "R" ? "rgba(193, 101, 101, 1)" : ((cargaCTe.CTe.Status == "C" || cargaCTe.CTe.Status == "I" || cargaCTe.CTe.Status == "D" || cargaCTe.CTe.Status == "Z") ? "#777" : ""))) : "",
                        DT_FontColor = cargaCTe.CTe != null ? (((cargaCTe.CTe.Status == "R" || cargaCTe.CTe.Status == "C" || cargaCTe.CTe.Status == "I" || cargaCTe.CTe.Status == "D" || cargaCTe.CTe.Status == "Z") ? "#FFFFFF" : "")) : "",
                        CodigoNotaFiscal = pedidoXMLNotaFiscalPorCargaCTe.XMLNotaFiscal.Codigo
                    });
                }
            }

            return retorno;
        }

        private string EmitirNFSe(int codigoCTe, Repositorio.UnitOfWork unitOfWork)
        {
            string mensagem = "";
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

            if (cte != null)
            {
                if (cte.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada || cte.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmDigitacao)
                {
                    Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);

                    cte.Status = "P";
                    repCTe.Atualizar(cte);

                    Servicos.Embarcador.Carga.CTe serCargaCTE = new Servicos.Embarcador.Carga.CTe(unitOfWork);
                    bool sucesso = svcNFSe.EmitirNFSe(cte.Codigo);
                    if (!sucesso)
                    {
                        mensagem = "A NFS-e nº " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.";
                    }
                    else
                    {
                        //sucesso = serCargaCTE.AdicionarCTeNaFilaDeConsulta(cte, WebServiceConsultaCTe);
                        //if (!sucesso)
                        //{
                        //    mensagem += "Protocolo (" + cte.Codigo + ") não foi possível adicionar o CTE a fila de Envio, tente novamente.";
                        //}
                    }
                }
                else
                {
                    mensagem = "A atual situação da NFS-e (" + cte.DescricaoStatus + ") não permite sua emissão.";
                }
            }
            else
            {
                mensagem = "O CT-e informado não foi localizado";
            }

            return mensagem;
        }


        private void AtualizarImpostosIBSCBS(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, int codigoTipoOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento repConfiguracaoIntegracaoEmissorDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento configuracaoIntegracaoEmissorDocumento = repConfiguracaoIntegracaoEmissorDocumento.BuscarConfiguracaoPadrao();
           
            if (cte.Empresa.OptanteSimplesNacional || !(((string.IsNullOrEmpty(cte.CSTIBSCBS) || string.IsNullOrEmpty(cte.ClassificacaoTributariaIBSCBS)) && (DateTime.Now >= configuracaoIntegracaoEmissorDocumento.DataLiberacaoImpostos || (cte.Empresa?.Configuracao?.EnviarNovoImposto ?? false)))))
                return;

            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Servicos.Embarcador.Imposto.ImpostoIBSCBS servicoImpostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBS = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS { BaseCalculo = cte.ValorPrestacaoServico - cte.ValorICMS, CodigoLocalidade = cte.Destinatario.Localidade.Codigo, SiglaUF = cte.Destinatario.Localidade.Estado.Sigla, CodigoTipoOperacao = codigoTipoOperacao, Empresa = cte.Empresa });

            if (impostoIBSCBS == null || impostoIBSCBS.CodigoOutraAliquota == 0)
                return;

            cte.ValorIBSEstadual = impostoIBSCBS.ValorIBSEstadual;
            cte.ValorIBSMunicipal = impostoIBSCBS.ValorIBSMunicipal;
            cte.ValorCBS = impostoIBSCBS.ValorCBS;
            cte.AliquotaCBS = impostoIBSCBS.AliquotaCBS;
            cte.AliquotaIBSMunicipal = impostoIBSCBS.AliquotaIBSMunicipal;
            cte.AliquotaIBSEstadual = impostoIBSCBS.AliquotaIBSEstadual;
            cte.BaseCalculoIBSCBS = impostoIBSCBS.BaseCalculo;
            cte.PercentualReducaoCBS = impostoIBSCBS.PercentualReducaoCBS;
            cte.PercentualReducaoIBSEstadual = impostoIBSCBS.PercentualReducaoIBSEstadual;
            cte.PercentualReducaoIBSMunicipal = impostoIBSCBS.PercentualReducaoIBSMunicipal;
            cte.CSTIBSCBS = impostoIBSCBS.CST;
            cte.ClassificacaoTributariaIBSCBS = impostoIBSCBS.ClassificacaoTributaria;
            cte.CodigoIndicadorOperacao = impostoIBSCBS.CodigoIndicadorOperacao;
            cte.NBS = impostoIBSCBS.NBS;

            cte.SetarRegraOutraAliquota(impostoIBSCBS.CodigoOutraAliquota);

            repositorioCTe.Atualizar(cte);
        }

        private string EmitirCTe(int codigoCTe, Repositorio.UnitOfWork unitOfWork)
        {
            string mensagem = "";
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = cte?.CargaCTes?.FirstOrDefault()?.Carga ?? null;

            if (cte != null)
            {
                if (cte.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada || cte.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmDigitacao || cte.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.ContingenciaFSDA)
                {

                    AtualizarImpostosIBSCBS(cte, carga?.TipoOperacao?.Codigo ?? 0, unitOfWork);

                    Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                    if (cte.DataEmissao.HasValue && cte.DataEmissao.Value < DateTime.Now.AddDays(-6) && cte.CodStatusProtocolo != "204")
                    {
                        TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(cte.Empresa.FusoHorario);
                        cte.DataEmissao = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);
                    }

                    cte.Status = "P";
                    repCTe.Atualizar(cte);

                    Servicos.Embarcador.Carga.CTe serCargaCTE = new Servicos.Embarcador.Carga.CTe(unitOfWork);
                    bool sucesso = svcCTe.Emitir(cte.Codigo, cte.Empresa.Codigo, null, "E", WebServiceOracle);
                    if (!sucesso)
                    {
                        mensagem = "O CT-e nº " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.";
                    }
                    else
                    {
                        //sucesso = serCargaCTE.AdicionarCTeNaFilaDeConsulta(cte, WebServiceConsultaCTe);
                        //if (!sucesso)
                        //{
                        //    mensagem += "Protocolo (" + cte.Codigo + ") não foi possível adicionar o CTE a fila de Envio, tente novamente.";
                        //}
                    }
                }
                else
                {
                    mensagem = "A atual situação do CT-e (" + cte.DescricaoStatus + ") não permite sua emissão.";
                }
            }
            else
            {
                mensagem = "O CT-e informado não foi localizado";
            }

            return mensagem;
        }

        private string InutilizarCTe(int codigoCTe, string justificativa, Repositorio.UnitOfWork unitOfWork)
        {
            string mensagem = "";
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
            Servicos.Embarcador.Carga.CTe serCargaCTe = new Servicos.Embarcador.Carga.CTe(unitOfWork);

            if (cte != null)
            {

                if (cte.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Rejeitada || cte.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.EmDigitacao)
                {
                    Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                    Servicos.Embarcador.Carga.CTe serCargaCTE = new Servicos.Embarcador.Carga.CTe(unitOfWork);
                    bool sucesso = svcCTe.Inutilizar(cte.Codigo, cte.Empresa.Codigo, justificativa, TipoServicoMultisoftware);
                    if (!sucesso)
                    {
                        mensagem = "O CT-e nº " + cte.Numero.ToString() + " da empresa " + cte.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao inutilizá-lo.";
                    }
                    else
                    {
                        //if (!serCargaCTe.AdicionarCTeNaFilaDeConsulta(cte, WebServiceConsultaCTe))
                        //    mensagem = "Não foi possível adicionar o CTE a fila de Inutilização";
                    }
                }
                else
                {
                    mensagem = "A atual situação do CT-e (" + cte.DescricaoStatus + ") não permite sua inutilização.";
                }
            }
            else
            {
                mensagem = "O CT-e informado não foi localizado";
            }

            return mensagem;
        }

        private bool CancelarCTe(out string erro, int codigoCTe, string justificativa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
            Servicos.Embarcador.Carga.CTe serCargaCTe = new Servicos.Embarcador.Carga.CTe(unidadeTrabalho);

            if (cte != null)
            {
                unidadeTrabalho.Start();

                if (!Servicos.Embarcador.CTe.CTe.CancelarOuAnularCTe(out erro, cte.Codigo, justificativa, unidadeTrabalho, _conexao.StringConexao, WebServiceConsultaCTe, TipoServicoMultisoftware))
                {
                    unidadeTrabalho.Rollback();
                    return false;
                }

                cte.CanceladoManualmente = true;
                repCTe.Atualizar(cte);

                unidadeTrabalho.CommitChanges();

                return true;
            }
            else
            {
                erro = "O CT-e informado não foi localizado.";
                return false;
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> BuscarPermissoesCTeEmDigitacaoCarga()
        {
            //todo: ver para tornar dinâmico se necessario;

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe>();

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.TotalServico);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarSerie);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Componente);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarTipoPagamento);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Seguro);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.ObservacaoGeral);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarProdutoPredominante);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarCaracteristicaAdicionalTransporte);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarValorTotalCarga);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.QuantidadeCarga);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.ObservacaoContribuinte);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.ObservacaoFisco);
            }

            return permissoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> BuscarPermissoesOutrosDocumentosEmDigitacaoCarga()
        {
            //todo: ver para tornar dinâmico se necessario;
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe>();

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.ObservacaoGeral);

            return permissoes;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> BuscarPermissoesCTeRejeitado()
        {
            //todo: ver para tornar dinâmico se necessario;

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe> permissoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe>();

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarCFOP);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarSerie);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarTipoPagamento);
                //permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Documento);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarIEParticipanete);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Seguro);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.ObservacaoGeral);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarProdutoPredominante);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarCaracteristicaAdicionalTransporte);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.ObservacaoContribuinte);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.ObservacaoFisco);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.DocumentoTransporteAnteriorEletronico);
            }
            else
            {
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarCFOP);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Remetente);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Tomador);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.AlterarIEParticipanete);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Recebedor);
                //permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Documento);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.QuantidadeCarga);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Expedidor);
                permissoes.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.PermissoesEdicaoCTe.Destinatario);
            }

            return permissoes;
        }

        private void AsyncAverbacaoRejeitados(int codigoCarga, string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

            try
            {
                // Repositorios e servicos
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                // Contador da unity
                int count = 0;

                // Itera todos e averba
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaCTe.BuscarPorCarga(codigoCarga);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cte in cargaCTes)
                {
                    // Manda pro Oracle
                    svcCTe.EmitirAverbacoesOracle(cte.CTe.Empresa.Codigo, cte.CTe.Codigo, Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao, unitOfWork);

                    // Incrementa operação
                    count++;

                    // Reinicia a unit
                    if (count == 25)
                    {
                        count = 0;
                        repCargaCTe = null;

                        unitOfWork.Dispose();
                        unitOfWork = new Repositorio.UnitOfWork(stringConexao);

                        repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                    }
                }

                if (cargaCTes.Count > 0)
                    VerificaAverbacao(cargaCTes.FirstOrDefault().Carga, unitOfWork);

                svcHubCarga.InformarCargaAtualizada(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, stringConexao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string CorFonteAverbacaoCTe(Dominio.Entidades.AverbacaoCTe obj)
        {
            if (obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Cancelado)
                return "#FFF";

            return "";
        }

        private string CorAverbacaoCTe(Dominio.Entidades.AverbacaoCTe obj)
        {
            if (obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;
            if (obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;
            if (obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Cancelado)
                return "#777";

            return "";
        }

        private void VerificaAverbacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            // Repositorios
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            // Atualiza situação da carga
            if (repAverbacaoCTe.BuscarPorCargaESituacao(carga.Codigo, Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao).Count == 0)
            {
                carga.PossuiPendencia = false;
                carga.problemaAverbacaoCTe = false;
                carga.MotivoPendencia = "";

                repCarga.Atualizar(carga);
            }
        }

        private bool GerarCartaCorrecaoContainer(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork, out string msgErro)
        {
            msgErro = "";
            Servicos.CCe svcCCe = new Servicos.CCe(unitOfWork);
            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);

            unitOfWork.Start();

            Dominio.Entidades.CartaDeCorrecaoEletronica cce = new Dominio.Entidades.CartaDeCorrecaoEletronica();

            if (!PreencherEntidade(cte, cce, unitOfWork, out string mensagemErro))
            {
                unitOfWork.Rollback();
                msgErro = mensagemErro;
                return false;
            }

            unitOfWork.CommitChanges();

            if (!Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.EmissorDocumentoCTe.EmitirCCe(cce, cce.CTe.Empresa.Codigo, unitOfWork))
                return false;
            else
                return true;
        }

        private bool PreencherEntidade(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.CartaDeCorrecaoEletronica cce, Repositorio.UnitOfWork unitOfWork, out string mensagemErro)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unitOfWork);

            if (cce.Codigo > 0 && cce.Status != Dominio.Enumeradores.StatusCCe.EmDigitacao && cce.Status != Dominio.Enumeradores.StatusCCe.Rejeicao)
            {
                mensagemErro = "A situação da carta de correção não permite que a mesma seja alterada.";
                return false;
            }

            int codigoCTe = cte.Codigo;
            DateTime dataEmissao = DateTime.Now;

            cce.CTe = repCTe.BuscarPorCodigo(codigoCTe);
            if (cce.CTe == null)
            {
                mensagemErro = "CT-e não encontrado.";
                return false;
            }
            if (cce.CTe.Status != "A")
            {
                mensagemErro = "É necessário que o CT-e esteja autorizado para gerar uma carta de correção.";
                return false;
            }

            cce.DataEmissao = dataEmissao;
            cce.Status = Dominio.Enumeradores.StatusCCe.Pendente;

            Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto = null;
            cce.NumeroSequencialEvento = repCCe.BuscarUltimoNumeroSequencial(cce.CTe.Codigo) + 1;
            repCCe.Inserir(cce);

            return SalvarItensCCe(cte, cce, unitOfWork, historicoObjeto, out mensagemErro);
        }

        private bool SalvarItensCCe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.CartaDeCorrecaoEletronica cce, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto, out string mensagemErro)
        {
            Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(unitOfWork);
            Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(unitOfWork);
            mensagemErro = "";

            if (cte.Containers != null && cte.Containers.Count > 0)
            {
                foreach (Dominio.Entidades.ContainerCTE container in cte.Containers)
                {
                    Dominio.Entidades.ItemCCe itemCCe = null;
                    itemCCe = new Dominio.Entidades.ItemCCe();
                    itemCCe.CCe = cce;
                    itemCCe.CampoAlterado = repCampoCCe.BuscarPorNomeCampo("idUnidCarga");
                    if (itemCCe.CampoAlterado == null)
                    {
                        mensagemErro = "Campo de container não cadastrado para realizar a carta de correção de forma automática.";
                        return false;
                    }
                    itemCCe.NumeroItemAlterado = 1;
                    itemCCe.ValorAlterado = container.Numero;
                    repItemCCe.Inserir(itemCCe, Auditado, historicoObjeto);

                    if (!string.IsNullOrWhiteSpace(container.Lacre1))
                    {
                        itemCCe = new Dominio.Entidades.ItemCCe();
                        itemCCe.CCe = cce;
                        itemCCe.CampoAlterado = repCampoCCe.BuscarPorNomeCampo("nLacre");
                        if (itemCCe.CampoAlterado == null)
                        {
                            mensagemErro = "Campo de lacre não cadastrado para realizar a carta de correção de forma automática.";
                            return false;
                        }
                        itemCCe.NumeroItemAlterado = 1;
                        itemCCe.ValorAlterado = container.Lacre1;
                        repItemCCe.Inserir(itemCCe, Auditado, historicoObjeto);
                    }

                    if (!string.IsNullOrWhiteSpace(container.Lacre2))
                    {
                        itemCCe = new Dominio.Entidades.ItemCCe();
                        itemCCe.CCe = cce;
                        itemCCe.CampoAlterado = repCampoCCe.BuscarPorNomeCampo("nLacre");
                        if (itemCCe.CampoAlterado == null)
                        {
                            mensagemErro = "Campo de lacre não cadastrado para realizar a carta de correção de forma automática.";
                            return false;
                        }
                        itemCCe.NumeroItemAlterado = 2;
                        itemCCe.ValorAlterado = container.Lacre2;
                        repItemCCe.Inserir(itemCCe, Auditado, historicoObjeto);
                    }

                    if (!string.IsNullOrWhiteSpace(container.Lacre3))
                    {
                        itemCCe = new Dominio.Entidades.ItemCCe();
                        itemCCe.CCe = cce;
                        itemCCe.CampoAlterado = repCampoCCe.BuscarPorNomeCampo("nLacre");
                        if (itemCCe.CampoAlterado == null)
                        {
                            mensagemErro = "Campo de lacre não cadastrado para realizar a carta de correção de forma automática.";
                            return false;
                        }
                        itemCCe.NumeroItemAlterado = 3;
                        itemCCe.ValorAlterado = container.Lacre3;
                        repItemCCe.Inserir(itemCCe, Auditado, historicoObjeto);
                    }
                }
            }

            return true;
        }

        private string ObterValorFrete(Dominio.Entidades.Embarcador.Cargas.CargaCTe obj, ConfiguracaoTMS configuracaoEmbarcador)
        {
            decimal valorRetornar;

            if (obj.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                if (ConfiguracaoEmbarcador.VisualizarValorNFSeDescontandoISSRetido && obj.CTe.BaseCalculoISS > 0)
                    valorRetornar = (obj.CTe.BaseCalculoISS - obj.CTe.ValorISSRetido);
                else
                    valorRetornar = obj.CTe.BaseCalculoISS;

                if (valorRetornar <= 0)
                    valorRetornar = obj.CTe.ValorAReceber;
            }
            else
                valorRetornar = obj.CTe.ValorAReceber;

            return valorRetornar.ToString("n2");
        }

        private string ObterCorLinhaCTe(Dominio.Entidades.Embarcador.Cargas.CargaCTe obj, string statusCTe, bool validarPendenciaTransportador)
        {
            if (obj.VinculoManual)
                return "#006400";

            if (!string.IsNullOrWhiteSpace(statusCTe))
                return "";

            if (obj.CTe.PossuiCTeComplementar)
                return "#ddd8f0";

            if (obj.CTe.PossuiAnulacaoSubstituicao)
                return "#f0e9d8";

            if (obj.CTe.PossuiCartaCorrecao)
                return "#d8e4f0";

            if (obj.CTe.Status == "A")
            {
                if (obj.SituacaoCheckin == SituacaoCheckin.RecusaAprovada)
                    return "#ffcc99";

                if (validarPendenciaTransportador)
                {
                    if (obj.SituacaoDocumentoContribuinte == SituacaoDocumentoContribuinte.AgDocTransportador || obj.SituacaoDocumentoContribuinte == SituacaoDocumentoContribuinte.Rejeitado)
                        return Cores.Amarelo.Descricao();

                    if (obj.SituacaoDocumentoContribuinte == SituacaoDocumentoContribuinte.AgAnaliseDocumentacao)
                        return Cores.Azul.Descricao();
                }

                return "#dff0d8";
            }

            if (obj.CTe.Status == "R")
                return "rgba(193, 101, 101, 1)";

            if (obj.CTe.Status == "C" || obj.CTe.Status == "I" || obj.CTe.Status == "D")
                return "#777";

            return "";
        }

        private Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaCTeCargaMDFeManual ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

            bool todosOsCTes = false;
            string raiz = "";

            int codEmpresa = Request.GetIntParam("Empresa");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                codEmpresa = Empresa.Codigo;
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                todosOsCTes = true;

            if (codEmpresa > 0)
            {
                Dominio.Entidades.Empresa empresa = repositorioEmpresa.BuscarPorCodigo(codEmpresa);
                raiz = (empresa.CNPJ_SemFormato).Remove(8, 6);
            }

            Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaCTeCargaMDFeManual filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaCTeCargaMDFeManual()
            {
                NumeroNF = Request.GetIntParam("NumeroNF"),
                CTe = Request.GetIntParam("CTe"),
                Carga = Request.GetIntParam("Carga"),
                Empresa = codEmpresa,
                SomenteCTesAquaviarios = Request.GetBoolParam("SomenteCTesAquaviarios"),
                Raiz = raiz,
                TodosOsCTes = todosOsCTes,
                RotasFrete = Request.GetListParam<int>("RotasFrete"),
            };

            return filtrosPesquisa;
        }

        private Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaCTeCargaMDFeManualMultiCTe ObterFiltrosPesquisaMultiCTe()
        {
            Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaCTeCargaMDFeManualMultiCTe filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.MDFe.FiltroPesquisaCTeCargaMDFeManualMultiCTe()
            {
                NumeroNF = Request.GetIntParam("NumeroNF"),
                CTe = Request.GetIntParam("CTe"),
                Carga = Request.GetIntParam("Carga"),
                Empresa = this.Usuario.Empresa.Codigo,
                RotasFrete = Request.GetListParam<int>("RotasFrete"),
            };

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propOrdenacao)
        {
            if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                propOrdenacao += ".Nome";

            if (propOrdenacao == "Destino")
                propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

            propOrdenacao = "CTe." + propOrdenacao;

            return propOrdenacao;
        }

        private string ObterCnpjSeguradoraPadrao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            if (cte.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.CNPJSeguro))
                return cte.Empresa.Configuracao.CNPJSeguro;

            if (cte.Empresa.Configuracao != null && !cte.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cte.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.EmpresaPai.Configuracao.CNPJSeguro))
                return cte.Empresa.EmpresaPai.Configuracao.CNPJSeguro;

            if (cte.Empresa.Configuracao != null && !cte.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim)
                return cte.Empresa.CNPJ;

            return string.Empty;
        }

        private string ObterNomeSeguradoraPadrao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            if (cte.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.NomeSeguro))
                return cte.Empresa.Configuracao.NomeSeguro.Left(30);

            if (cte.Empresa.Configuracao != null && !cte.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cte.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.EmpresaPai.Configuracao.NomeSeguro))
                return cte.Empresa.EmpresaPai.Configuracao.NomeSeguro.Left(30);

            if (cte.Empresa.Configuracao != null && !cte.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim)
                return cte.Empresa.RazaoSocial.Left(30);

            return string.Empty;
        }

        private string ObterApoliceSeguroPadrao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            if (cte.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.Configuracao.NumeroApoliceSeguro))
                return cte.Empresa.Configuracao.NumeroApoliceSeguro.Left(30);

            if (cte.Empresa.Configuracao != null && !cte.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cte.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(cte.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro))
                return cte.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Left(30);

            return string.Empty;
        }

        #endregion
    }
}
