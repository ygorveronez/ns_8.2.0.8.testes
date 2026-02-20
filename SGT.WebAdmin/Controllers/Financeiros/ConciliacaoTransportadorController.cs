using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;
using System.Security.Cryptography.X509Certificates;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/ConciliacaoTransportador")]
    public class ConciliacaoTransportadorController : BaseController
    {
		#region Construtores

		public ConciliacaoTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> CodigosTransportador = Request.GetListParam<int>("Transportador");

                string raizCnpj = "";

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    //codigoTransportador = this.Usuario.Empresa.Codigo;
                    raizCnpj = this.Usuario.Empresa.RaizCnpj;
                }

                SituacaoConciliacaoTransportador situacaoConciliacaoTransportador = Request.GetEnumParam<SituacaoConciliacaoTransportador>("Situacao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 6, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho("Transportador", "Transportador", 25, Models.Grid.Align.center, false);

                grid.AdicionarCabecalho("Período", "Periodo", 17, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Periodicidade", "Periodicidade", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Anuência disponível", "DataAnuenciaDisponivel", 13, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data de assinatura", "DataAssinatura", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Total", "ValorTotal", 11, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor em aberto", "ValorEmAberto", 11, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Numero ocorrências", "NumeroOcorrencias", 12, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterParametroOrdenacao);

                string propOrdenar = parametrosConsulta.PropriedadeOrdenar;

                Repositorio.Embarcador.Financeiro.ConciliacaoTransportador repConciliacaoTransportador = new Repositorio.Embarcador.Financeiro.ConciliacaoTransportador(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                if (CodigosTransportador != null && CodigosTransportador.Count > 0)
                {
                    Servicos.Embarcador.Transportadores.Empresa servicoEmpresa = new Servicos.Embarcador.Transportadores.Empresa(unitOfWork);
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    List<Dominio.Entidades.Empresa> empresasSelecionadas = repEmpresa.BuscarPorCodigos(CodigosTransportador);

                    foreach (Dominio.Entidades.Empresa emp in empresasSelecionadas)
                    {
                        List<Dominio.Entidades.Empresa> empresas = servicoEmpresa.BuscarEmpresasPorRaizCnpj(emp);
                        CodigosTransportador.AddRange(empresas.Select(x => x.Codigo).ToList());
                    }
                }


                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoTransportador filtro = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoTransportador
                {
                    RaizCnpj = raizCnpj,
                    CodigosTransportador = CodigosTransportador,
                    SituacaoConciliacaoTransportador = situacaoConciliacaoTransportador,
                    DataInicial = Request.GetDateMonthParam("DataInicial", '/', true),
                    DataFinal = Request.GetDateMonthParam("DataFinal", '/', false),
                    DataInicialAssinatura = Request.GetDateMonthParam("DataInicialAssinatura", '/', true),
                    DataFinalAssinatura = Request.GetDateMonthParam("DataFinalAssinatura", '/', false),
                    AnuenciaDisponivelInicio = Request.GetDateMonthParam("AnuenciaDisponivelInicio", '/', true),
                    AnuenciaDisponivelFinal = Request.GetDateMonthParam("AnuenciaDisponivelFinal", '/', false),
                    NumeroCarta = Request.GetIntParam("NumeroCarta")
                };

                List<Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador> listaConciliacaoTransportador = repConciliacaoTransportador.Consultar(filtro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repConciliacaoTransportador.ContarConsulta(filtro));

                List<int> codigosConciliacao = (from o in listaConciliacaoTransportador select o.Codigo).ToList();
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCtes = repCte.BuscarPorCodigosConciliacaoTransportador(codigosConciliacao);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCteComTitulo = (from cte in listaCtes where cte.Titulo != null select cte).ToList();

                var lista = (from p in listaConciliacaoTransportador
                             select new
                             {
                                 p.Codigo,
                                 Periodo = p.DataInicial.ToString("dd/MM/yyyy") + " a " + p.DataFinal.ToString("dd/MM/yyyy"),
                                 Transportador = p.PrimeiroTransportador?.RazaoSocial ?? "",
                                 Numero = p.Codigo.ToString("n0"),
                                 Periodicidade = p.Periodicidade.ObterDescricao(),
                                 Situacao = p.SituacaoConciliacaoTransportador.ObterDescricao(),
                                 DataAnuenciaDisponivel = p.DataAnuenciaDisponivel.ToString("dd/MM/yyyy"),
                                 DataAssinatura = p.DataAssinaturaAnuencia?.ToString("dd/MM/yyyy") ?? "",
                                 ValorTotal = "R$ " + ObterValorTotalDevido((from o in listaCteComTitulo where o.ConciliacaoTransportador.Codigo == p.Codigo select o).ToList()).ToString("0.00"),
                                 ValorEmAberto = "R$ " + ObterValorEmAberto((from o in listaCteComTitulo where o.ConciliacaoTransportador.Codigo == p.Codigo select o).ToList(), out int numeroDocumentosPendentes).ToString("0.00"),
                                 NumeroOcorrencias = 0, // TODO quando essa parte for especificada melhor
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
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> CodigosTransportador = Request.GetListParam<int>("Transportador");

                string raizCnpj = "";

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    //codigoTransportador = this.Usuario.Empresa.Codigo;
                    raizCnpj = this.Usuario.Empresa.RaizCnpj;
                }

                SituacaoConciliacaoTransportador situacaoConciliacaoTransportador = Request.GetEnumParam<SituacaoConciliacaoTransportador>("Situacao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 6, Models.Grid.Align.center, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho("Transportador", "Transportador", 25, Models.Grid.Align.center, false);

                grid.AdicionarCabecalho("Período", "Periodo", 17, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Periodicidade", "Periodicidade", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Anuência disponível", "DataAnuenciaDisponivel", 13, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data de assinatura", "DataAssinatura", 12, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Total", "ValorTotal", 11, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor em aberto", "ValorEmAberto", 11, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Numero ocorrências", "NumeroOcorrencias", 12, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterParametroOrdenacao);

                string propOrdenar = parametrosConsulta.PropriedadeOrdenar;

                Repositorio.Embarcador.Financeiro.ConciliacaoTransportador repConciliacaoTransportador = new Repositorio.Embarcador.Financeiro.ConciliacaoTransportador(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoTransportador filtro = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoTransportador
                {
                    RaizCnpj = raizCnpj,
                    CodigosTransportador = CodigosTransportador,
                    SituacaoConciliacaoTransportador = situacaoConciliacaoTransportador,
                    DataFinal = Request.GetDateTimeParam("DataFinal"),
                    DataInicial = Request.GetDateTimeParam("DataInicial"),
                    DataFinalAssinatura = Request.GetDateTimeParam("DataFinalAssinatura"),
                    DataInicialAssinatura = Request.GetDateTimeParam("DataInicialAssinatura"),
                    AnuenciaDisponivelFinal = Request.GetDateTimeParam("AnuenciaDisponivelFinal"),
                    AnuenciaDisponivelInicio = Request.GetDateTimeParam("AnuenciaDisponivelInicio"),
                    NumeroCarta = Request.GetIntParam("NumeroCarta")
                };

                List<Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador> listaConciliacaoTransportador = repConciliacaoTransportador.Consultar(filtro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repConciliacaoTransportador.ContarConsulta(filtro));

                List<int> codigosConciliacao = (from o in listaConciliacaoTransportador select o.Codigo).ToList();
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCtes = repCte.BuscarPorCodigosConciliacaoTransportador(codigosConciliacao);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCteComTitulo = (from cte in listaCtes where cte.Titulo != null select cte).ToList();

                var lista = (from p in listaConciliacaoTransportador
                             select new
                             {
                                 p.Codigo,
                                 Periodo = p.DataInicial.ToString("dd/MM/yyyy") + " a " + p.DataFinal.ToString("dd/MM/yyyy"),
                                 Transportador = p.PrimeiroTransportador?.RazaoSocial ?? "",
                                 Numero = p.Codigo.ToString("n0"),
                                 Periodicidade = p.Periodicidade.ObterDescricao(),
                                 Situacao = p.SituacaoConciliacaoTransportador.ObterDescricao(),
                                 DataAnuenciaDisponivel = p.DataAnuenciaDisponivel.ToString("dd/MM/yyyy"),
                                 DataAssinatura = p.DataAssinaturaAnuencia?.ToString("dd/MM/yyyy") ?? "",
                                 ValorTotal = "R$ " + ObterValorTotalDevido((from o in listaCteComTitulo where o.ConciliacaoTransportador.Codigo == p.Codigo select o).ToList()).ToString("0.00"),
                                 ValorEmAberto = "R$ " + ObterValorEmAberto((from o in listaCteComTitulo where o.ConciliacaoTransportador.Codigo == p.Codigo select o).ToList(), out int numeroDocumentosPendentes).ToString("0.00"),
                                 NumeroOcorrencias = 0, // TODO quando essa parte for especificada melhor
                             }).ToList();

                grid.AdicionaRows(lista);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar a pesquisa.");
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Financeiro.ConciliacaoTransportador repConciliacaoTransportador = new Repositorio.Embarcador.Financeiro.ConciliacaoTransportador(unitOfWork);
                var conciliacao = repConciliacaoTransportador.BuscarPorCodigo(codigo, false);

                if (conciliacao == null)
                {
                    throw new ControllerException("Conciliação não encontrada");
                }

                var usuarioAtualPodeAssinar = true;

                try
                {
                    ValidarEmpresaPodeAssinar(conciliacao, unitOfWork);
                }
                catch
                {
                    usuarioAtualPodeAssinar = false;
                }

                var objeto = new
                {
                    conciliacao.Codigo,
                    Fechada = DateTime.Now > conciliacao.DataFinal,
                    DisponivelParaAssinatura = DateTime.Now > conciliacao.DataAnuenciaDisponivel,
                    Assinada = conciliacao.DataAssinaturaAnuencia.HasValue,
                    UsuarioAtualPodeAssinar = usuarioAtualPodeAssinar,
                    DescricaoTransportador = conciliacao.PrimeiroTransportador?.RazaoSocial + " - " + conciliacao.PrimeiroTransportador?.RaizCnpj ?? ""
                };
                return new JsonpResult(objeto);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCtes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoConciliacao);

                int.TryParse(Request.Params("Transportador"), out int codigoTransportador);

                Dominio.Enumeradores.TipoPagamento? tipoPagamento = Request.GetNullableEnumParam<Dominio.Enumeradores.TipoPagamento>("TipoPagamento");
                Dominio.Enumeradores.TipoDocumento tipoDocumento = Request.GetEnumParam<Dominio.Enumeradores.TipoDocumento>("TipoDocumento");

                // Converte parametros
                DateTime? dataInicio = null, dataFim = null;

                if (DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicioAux))
                    dataInicio = dataInicioAux;

                if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFimAux))
                    dataFim = dataFimAux;

                Models.Grid.Grid grid = ObterGridPesquisa();
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe
                {
                    CodigoConciliacaoTransportador = codigoConciliacao,
                    CodigoTransportador = codigoTransportador,
                    TipoDocumentoEmissao = tipoDocumento,
                    TipoCTe = Dominio.Enumeradores.TipoCTE.Todos,
                    StatusCTe = new List<string> { "P", "E", "R", "A", "C", "I", "D", "S", "K", "L", "Z", "X", "V", "B", "M", "F", "Q", "Y", "N" },
                };

                if (tipoPagamento.HasValue)
                    filtrosPesquisa.TipoPagamento = tipoPagamento.Value;

                if (dataInicio.HasValue)
                    filtrosPesquisa.DataEmissaoInicial = dataInicio.Value;
                if (dataFim.HasValue)
                    filtrosPesquisa.DataEmissaoFinal = dataFim.Value;

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCtes = repCte.Consultar(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCte.ContarConsulta(filtrosPesquisa));

                var lista = (from p in listaCtes
                             select new
                             {
                                 p.Codigo,
                                 CodigoCarga = repCargaCte.BuscarCargaPorCTe(p.Codigo)?.CodigoCargaEmbarcador ?? "",
                                 Numero = p.Numero,
                                 ModeloDocumentoFiscal = p.ModeloDocumentoFiscal?.Abreviacao ?? "Não encontrado",
                                 Status = p.DescricaoStatus,
                                 CNPJ = p.Empresa?.CNPJ_Formatado ?? "",
                                 DataEmissao = p.DataEmissao?.ToString("dd/MM/yyyy HH:mm") ?? "",
                                 DataVencimento = p.Titulo?.DataVencimento?.ToString("dd/MM/yyyy") ?? "",
                                 DataLiquidacao = p.Titulo?.DataLiquidacao?.ToString("dd/MM/yyyy") ?? "",
                                 StatusTitulo = p.Titulo?.DescricaoSituacao ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto.ObterDescricao(),
                                 ValorOriginal = "R$ " + (p.Titulo?.ValorOriginal.ToString("n2") ?? "0,00"),
                                 Acrescimos = "R$ " + (p.Titulo?.Acrescimo.ToString("n2") ?? "0,00"),
                                 Decrescimos = "R$ " + (p.Titulo?.Desconto.ToString("n2") ?? "0,00"),
                                 ValorDevido = (p.Titulo != null ? "R$ " + (p.Titulo.ValorOriginal + p.Titulo.ValorAcrescimo - p.Titulo.ValorDesconto).ToString("n2") : "R$ 0,00"),
                                 ValorPago = "R$ " + (p.Titulo?.ValorPago.ToString("n2") ?? "0,00"),
                                 ValorTotal = "R$ " + (p.Titulo?.ValorTotalCalculado.ToString("n2") ?? "0,00"),
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
        public async Task<IActionResult> ExportarPesquisaCtes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoConciliacao);

                int.TryParse(Request.Params("Transportador"), out int codigoTransportador);

                Dominio.Enumeradores.TipoPagamento? tipoPagamento = Request.GetNullableEnumParam<Dominio.Enumeradores.TipoPagamento>("TipoPagamento");
                Dominio.Enumeradores.TipoDocumento tipoDocumento = Request.GetEnumParam<Dominio.Enumeradores.TipoDocumento>("TipoDocumento");

                // Converte parametros
                DateTime? dataInicio = null, dataFim = null;

                if (DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicioAux))
                    dataInicio = dataInicioAux;

                if (DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFimAux))
                    dataFim = dataFimAux;

                Models.Grid.Grid grid = ObterGridPesquisa();

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaConsultaCTe
                {
                    CodigoConciliacaoTransportador = codigoConciliacao,
                    CodigoTransportador = codigoTransportador,
                    TipoDocumentoEmissao = tipoDocumento,
                    TipoCTe = Dominio.Enumeradores.TipoCTE.Todos,
                    StatusCTe = new List<string> { "P", "E", "R", "A", "C", "I", "D", "S", "K", "L", "Z", "X", "V", "B", "M", "F", "Q", "Y", "N" },
                };

                if (tipoPagamento.HasValue)
                    filtrosPesquisa.TipoPagamento = tipoPagamento.Value;

                if (dataInicio.HasValue)
                    filtrosPesquisa.DataEmissaoInicial = dataInicio.Value;
                if (dataFim.HasValue)
                    filtrosPesquisa.DataEmissaoFinal = dataFim.Value;

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCtes = repCte.Consultar(filtrosPesquisa, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCte.ContarConsulta(filtrosPesquisa));

                var lista = (from p in listaCtes
                             select new
                             {
                                 p.Codigo,
                                 CodigoCarga = repCargaCte.BuscarCargaPorCTe(p.Codigo)?.CodigoCargaEmbarcador ?? "",
                                 Numero = p.Numero,
                                 ModeloDocumentoFiscal = p.ModeloDocumentoFiscal?.Abreviacao ?? "Não encontrado",
                                 Status = p.DescricaoStatus,
                                 CNPJ = p.Empresa?.CNPJ_Formatado ?? "",
                                 DataEmissao = p.DataEmissao?.ToString("dd/MM/yyyy HH:mm") ?? "",
                                 DataVencimento = p.Titulo?.DataVencimento?.ToString("dd/MM/yyyy") ?? "",
                                 DataLiquidacao = p.Titulo?.DataLiquidacao?.ToString("dd/MM/yyyy") ?? "",
                                 StatusTitulo = p.Titulo?.DescricaoSituacao ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto.ObterDescricao(),
                                 ValorOriginal = "R$ " + (p.Titulo?.ValorOriginal.ToString("n2") ?? "0,00"),
                                 Acrescimos = "R$ " + (p.Titulo?.Acrescimo.ToString("n2") ?? "0,00"),
                                 Decrescimos = "R$ " + (p.Titulo?.Desconto.ToString("n2") ?? "0,00"),
                                 ValorDevido = (p.Titulo != null ? "R$ " + (p.Titulo.ValorOriginal + p.Titulo.ValorAcrescimo - p.Titulo.ValorDesconto).ToString("n2") : "R$ 0,00"),
                                 ValorPago = "R$ " + (p.Titulo?.ValorPago.ToString("n2") ?? "0,00"),
                                 ValorTotal = "R$ " + (p.Titulo?.ValorTotalCalculado.ToString("n2") ?? "0,00"),
                             }).ToList();

                grid.AdicionaRows(lista);
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar a pesquisa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaAcrescimoDecrescimo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Financeiro.TituloAcrescimoDecrescimo repTituloAcrescimoDecrescimo = new Repositorio.Embarcador.Financeiro.TituloAcrescimoDecrescimo(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigoCte);

                var cte = repCte.BuscarPorCodigo(codigoCte);

                if (cte == null)
                    throw new ControllerException("CTe não encontrado");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", "Tipo", 2, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Valor", "Valor", 2, Models.Grid.Align.center, false);

                var listaAcrescimosDecrescimos = repTituloAcrescimoDecrescimo.BuscarPorTitulo(cte.Titulo?.Codigo ?? 0);

                grid.setarQuantidadeTotal(listaAcrescimosDecrescimos.Count);

                var lista = (from p in listaAcrescimosDecrescimos
                             select new
                             {
                                 p.Codigo,
                                 Tipo = p.Tipo.ObterDescricao(),
                                 Descricao = p.Descricao,
                                 Valor = "R$ " + (p.Valor.ToString("0.00") ?? "0,00"),
                             }).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
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

        public async Task<IActionResult> ObterResumoPeriodo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoConciliacao);

                Repositorio.Embarcador.Financeiro.ConciliacaoTransportador repConciliacaoTransportador = new Repositorio.Embarcador.Financeiro.ConciliacaoTransportador(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador conciliacao = repConciliacaoTransportador.BuscarPorCodigo(codigoConciliacao, false);

                if (conciliacao == null)
                    throw new ControllerException("Conciliação não encontrada");

                Dominio.Entidades.Empresa transportadorParaAssinar = repConciliacaoTransportador.ObterTransportadorParaAssinatura(conciliacao);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCte = repCte.BuscarPorCodigosConciliacaoTransportador(new List<int> { conciliacao.Codigo });
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCteComTitulo = (from cte in listaCte where cte.Titulo != null select cte).ToList();

                decimal valorTotalDocumentos = ObterValorTotalDevido(listaCteComTitulo);
                decimal acrescimos = (from cte in listaCteComTitulo select cte.Titulo.Acrescimo).Sum();
                decimal decrescimos = (from cte in listaCteComTitulo select cte.Titulo.Desconto).Sum();
                decimal saldoTotal = valorTotalDocumentos + acrescimos - decrescimos;
                decimal valorPago = ObterValorPago(listaCteComTitulo);
                decimal valorEmAberto = (valorTotalDocumentos + acrescimos - decrescimos) - valorPago;

                string dataELocal = "";
                dataELocal += transportadorParaAssinar.LocalidadeUF;

                var dataAssinatura = conciliacao.DataAssinaturaAnuencia ?? DateTime.Today;
                dataELocal += ", " + dataAssinatura.ToString("dd") + " de " + dataAssinatura.ToString("MMMM") + " de " + dataAssinatura.ToString("yyyy");

                return new JsonpResult(new
                {
                    Periodo = conciliacao.PeriodoFormatado,
                    DescricaoTransportador = conciliacao.PrimeiroTransportador != null ? conciliacao.PrimeiroTransportador.RazaoSocial + " - CNPJ Raiz " + conciliacao.PrimeiroTransportador.RaizCnpj : "",
                    NumeroCarta = conciliacao.Codigo,
                    ValorTotalDevido = "R$ " + (valorTotalDocumentos.ToString("n2") ?? "0,00"),
                    Acrescimos = "R$ " + (acrescimos.ToString("N2") ?? "0,00"),
                    Decrescimos = "R$ " + (decrescimos.ToString("n2") ?? "0,00"),
                    SaldoTotal = "R$ " + (saldoTotal.ToString("n2") ?? "0,00"),
                    ValorPago = "R$ " + (valorPago.ToString("n2") ?? "0,00"),
                    ValorEmAberto = "R$ " + (valorEmAberto.ToString("n2") ?? "0,00"),
                    Periodicidade = conciliacao.Periodicidade.ObterDescricao(),
                    NumeroDocumentos = listaCte.Count(),
                    NumeroDocumentosPendentes = listaCte.Where(x => x.Titulo == null || (x.Titulo != null && x.Titulo.StatusTitulo != StatusTitulo.Quitada)).Count(),
                    DataAnuenciaDisponivel = conciliacao.DataAnuenciaDisponivel.ToString("dd/MM/yyyy"),

                    // Dados laterais
                    DataELocal = dataELocal,
                    NomeTransportador = transportadorParaAssinar.RazaoSocial,
                    CnpjTransportador = transportadorParaAssinar.CNPJ_Formatado,
                });
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
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

        public async Task<IActionResult> ObterDadosTermo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoConciliacao = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.ConciliacaoTransportador repConciliacaoTransportador = new Repositorio.Embarcador.Financeiro.ConciliacaoTransportador(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador conciliacao = repConciliacaoTransportador.BuscarPorCodigo(codigoConciliacao, false);

                if (conciliacao == null)
                    throw new ControllerException("Conciliação não encontrada");

                Dominio.Entidades.Empresa transportadorParaAssinar = repConciliacaoTransportador.ObterTransportadorParaAssinatura(conciliacao);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCte = repCte.BuscarPorCodigosConciliacaoTransportador(new List<int> { conciliacao.Codigo });
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCteComTitulo = (from cte in listaCte where cte.Titulo != null select cte).ToList();

                decimal valorTotalDevido = ObterValorTotalDevido(listaCteComTitulo);
                decimal acrescimos = (from cte in listaCteComTitulo select cte.Titulo.Acrescimo).Sum();
                decimal decrescimos = (from cte in listaCteComTitulo select cte.Titulo.Desconto).Sum();
                decimal saldoTotal = valorTotalDevido + acrescimos - decrescimos;

                return new JsonpResult(new
                {
                    TransportadorParaAssinar = new
                    {
                        NomeEmpresa = transportadorParaAssinar.RazaoSocial,
                        CNPJ = transportadorParaAssinar.CNPJ_Formatado,
                        transportadorParaAssinar.Endereco,
                        transportadorParaAssinar.Bairro,
                        transportadorParaAssinar.Numero,
                        Cidade = transportadorParaAssinar.Localidade.Descricao,
                        Estado = transportadorParaAssinar.Localidade.Estado.Sigla
                    },
                    DataInicial = conciliacao.DataInicial.ToDateString(),
                    DataFinal = conciliacao.DataFinal.ToDateString(),
                    ValorConciliacao = saldoTotal.ToString("n2")
                });
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
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

        public async Task<IActionResult> AssinarAnuencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoConciliacao = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.ConciliacaoTransportador repConciliacaoTransportador = new Repositorio.Embarcador.Financeiro.ConciliacaoTransportador(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador conciliacao = repConciliacaoTransportador.BuscarPorCodigo(codigoConciliacao, false);

                if (conciliacao == null)
                    throw new ControllerException("Conciliação não encontrada");

                if (conciliacao.Assinado)
                    throw new ControllerException("Anuência já foi assinada");

                ValidarEmpresaPodeAssinar(conciliacao, unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCte = repCte.BuscarPorCodigosConciliacaoTransportador(new List<int> { conciliacao.Codigo });
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCteComTitulo = (from cte in listaCte where cte.Titulo != null select cte).ToList();

                decimal valorTotalDevido = ObterValorTotalDevido(listaCteComTitulo);
                decimal acrescimos = (from cte in listaCteComTitulo select cte.Titulo.Acrescimo).Sum();
                decimal decrescimos = (from cte in listaCteComTitulo select cte.Titulo.Desconto).Sum();
                decimal saldoTotal = valorTotalDevido + acrescimos - decrescimos;

                X509Certificate2 certificado = new X509Certificate2(Empresa.NomeCertificado, Empresa.SenhaCertificado);

                conciliacao.DataAssinaturaAnuencia = DateTime.Now;
                repConciliacaoTransportador.Atualizar(conciliacao);

                // Gera a anuência
                Servicos.Embarcador.Financeiro.ConciliacaoTransportador ser = new Servicos.Embarcador.Financeiro.ConciliacaoTransportador(unitOfWork);
                ser.GerarPdfAnuencia(conciliacao, saldoTotal, Empresa.Codigo);

                // Assina a anuência
                string anuenciaPath = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "AnuenciaTransportador" }), conciliacao.Codigo + ".pdf");
                string anuenciaAssinadaPath = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "AnuenciaTransportador" }), conciliacao.Codigo + "_assinado.pdf");

                Servicos.AssinaturaPdf serAssinatura = new Servicos.AssinaturaPdf();
                serAssinatura.AssinarPdf(anuenciaPath, anuenciaAssinadaPath, certificado);

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Houve um erro ao assinar a anuência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DownloadAnuencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoConciliacao = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.ConciliacaoTransportador repConciliacaoTransportador = new Repositorio.Embarcador.Financeiro.ConciliacaoTransportador(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador conciliacao = repConciliacaoTransportador.BuscarPorCodigo(codigoConciliacao, false);

                if (conciliacao == null)
                    throw new ControllerException("Conciliação não encontrada");

                var fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "AnuenciaTransportador" }), conciliacao.Codigo + ".pdf");
                byte[] buffer = null;

                if (!Utilidades.IO.FileStorageService.Storage.Exists(fileLocation))
                    return new JsonpResult(false, true, "Não foi possível baixar a anuência, atualize a página e tente novamente.");

                buffer = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(fileLocation);

                if (buffer != null)
                    return Arquivo(buffer, "application/pdf", "anuencia.pdf");
                else
                    return new JsonpResult(false, true, "Não foi possível baixar a anuência, atualize a página e tente novamente.");
            }
            catch (ControllerException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, ex.Message);
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

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Carga", "CodigoCarga", 6, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Número", "Numero", 6, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tipo", "ModeloDocumentoFiscal", 6, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Status", "Status", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("CNPJ", "CNPJ", 15, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Data emissão", "DataEmissao", 12, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 12, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Data Pagamento", "DataLiquidacao", 12, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Situação", "StatusTitulo", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Valor do Documento", "ValorOriginal", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Acréscimos", "Acrescimos", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Decréscimos", "Decrescimos", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Valor Devido", "ValorDevido", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Valor Pago", "ValorPago", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Saldo a Pagar", "ValorTotal", 10, Models.Grid.Align.center, false);

            return grid;
        }

        private void ValidarEmpresaPodeAssinar(Dominio.Entidades.Embarcador.Financeiro.ConciliacaoTransportador conciliacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.ConciliacaoTransportador repConciliacaoTransportador = new Repositorio.Embarcador.Financeiro.ConciliacaoTransportador(unitOfWork);
            Dominio.Entidades.Empresa transportadorParaAssinar = repConciliacaoTransportador.ObterTransportadorParaAssinatura(conciliacao);

            if (Empresa.Codigo != transportadorParaAssinar.Codigo)
                throw new ControllerException("Você não tem autorização para assinar esse documento.");

            if (!Usuario.PermiteAssinarAnuencia)
                throw new ControllerException("Você não tem autorização para assinar esse documento.");

            if (string.IsNullOrEmpty(Empresa.NomeCertificado) || string.IsNullOrEmpty(Empresa.SenhaCertificado))
                throw new ControllerException("Essa conta não tem um certificado para assinatura.");
        }

        private decimal ObterValorEmAberto(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCteComTitulo, out int numeroDocumentosPendentes)
        {
            var list = (from cte in listaCteComTitulo where cte.Titulo.ValorPendente > 0 select cte.Titulo.ValorPendente);
            numeroDocumentosPendentes = list.Count();
            return list.Sum();
        }

        private decimal ObterValorTotalDevido(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCteComTitulo)
        {
            return (from cte in listaCteComTitulo select cte.Titulo.ValorOriginal).Sum();
        }
        private decimal ObterValorPago(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCteComTitulo)
        {
            return (from cte in listaCteComTitulo select cte.Titulo.ValorPago).Sum();
        }
        private string ObterParametroOrdenacao(string propriedade)
        {
            if (propriedade == "Numero")
                return "Codigo";

            if (propriedade == "Periodo")
                return "DataInicial";

            if (propriedade == "Situacao")
                return "SituacaoConciliacaoTransportador";

            if (propriedade == "DataAssinatura")
                return "DataAssinaturaAnuencia";

            return propriedade;
        }

        #endregion
    }
}
