using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.Fechamento
{
    [CustomAuthorize(new string[] { "ConsultarCTesFechamento", "ConsultarDocumentoParaEmissaoNFSManual", "BuscarDadosParaFechamento", "CargasFechamento", "DetalhesContrato", "AcordosContrato" }, "Fechamento/FechamentoFrete")]
    public class FechamentoFreteController : BaseController
    {
		#region Construtores

		public FechamentoFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

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

        public async Task<IActionResult> ConsultarCTesFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                int codigoFechamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Fechamento.FechamentoFrete repositorioFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamento = repositorioFechamentoFrete.BuscarPorCodigo(codigoFechamento);

                if (fechamento.Situacao == SituacaoFechamentoFrete.PendenciaEmissao)// se está com pendencia na emissão o sistema tente ver se autorizou os rejeitados para o fluxo andar.
                    new Servicos.Embarcador.Fechamento.FechamentoFrete(unitOfWork).ValidarEmissaoComplementosFechamentoFrete(fechamento, WebServiceConsultaCTe, TipoServicoMultisoftware, WebServiceOracle);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("SituacaoCTe", false);
                grid.AdicionarCabecalho("CodigoCTE", false);
                grid.AdicionarCabecalho("NumeroModeloDocumentoFiscal", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Número", "Numero", 7, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Serie", "Serie", 7, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Doc.", "AbreviacaoModeloDocumentoFiscal", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nota(s) Fiscai(s)", "NumeroNotas", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Tomador", "Tomador", 18, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                {
                    grid.AdicionarCabecalho("T. Pagamento", "DescricaoTipoPagamento", 10, Models.Grid.Align.center, true);
                    grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, true);
                }

                grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Alíquota", "Aliquota", 5, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Status", "Status", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Retorno Sefaz", "RetornoSefaz", 15, Models.Grid.Align.left, false, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarCTesFechamento);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repositorioCargaCTeComplementoInfo.BuscarCTesPorFechamento(fechamento.Codigo, parametrosConsulta);
                grid.setarQuantidadeTotal(repositorioCargaCTeComplementoInfo.ContarPorCTEsFechamento(fechamento.Codigo));
                grid.AdicionaRows(MontarListaCTes(cargaCTesComplementoInfo));
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarDocumentoParaEmissaoNFSManual()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Chave", "Chave", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Doc.", "Abreviacao", 5, Models.Grid.Align.center, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 13, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Peso", "Peso", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor do Frete", "ValorFrete", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("NFS", "NFS", 6, Models.Grid.Align.left, true);

                int codigoFechamentoFrete = Request.GetIntParam("Codigo");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarDocumentoParaEmissaoNFSManual);
                Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual repositorioCargaDocumentoParaEmissaoNFSManual = new Repositorio.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual(unitOfWork);
                int totalRegistros = repositorioCargaDocumentoParaEmissaoNFSManual.ContarConsultaDocumentoParaEmissaoNFSManualPorFechamentoFrete(codigoFechamentoFrete);
                List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual> documentosParaEmissaoNFSmanual = (totalRegistros > 0) ? repositorioCargaDocumentoParaEmissaoNFSManual.ConsultarDocumentoParaEmissaoNFSManualPorFechamentoFrete(codigoFechamentoFrete, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();

                var documentosParaEmissaoNFSmanualRetornar = (
                    from documentoParaEmissaoNFSmanual in documentosParaEmissaoNFSmanual
                    select new
                    {
                        documentoParaEmissaoNFSmanual.Codigo,
                        documentoParaEmissaoNFSmanual.Numero,
                        documentoParaEmissaoNFSmanual.Chave,
                        documentoParaEmissaoNFSmanual.ModeloDocumentoFiscal.Abreviacao,
                        Remetente = $"{documentoParaEmissaoNFSmanual.Remetente.Nome} ({documentoParaEmissaoNFSmanual.Remetente.CPF_CNPJ_Formatado})",
                        Destinatario = $"{documentoParaEmissaoNFSmanual.Destinatario.Nome} ({documentoParaEmissaoNFSmanual.Destinatario.CPF_CNPJ_Formatado})",
                        Destino = documentoParaEmissaoNFSmanual.Destinatario.Localidade.DescricaoCidadeEstado,
                        ValorFrete = documentoParaEmissaoNFSmanual.ValorFrete.ToString(),
                        Peso = documentoParaEmissaoNFSmanual.Peso.ToString(),
                        NFS = documentoParaEmissaoNFSmanual.CTe?.Numero.ToString() ?? "",
                        DT_RowColor = documentoParaEmissaoNFSmanual.CTe != null ? "#dff0d8" : "#fcf8e3"
                    }
                ).ToList();

                grid.AdicionaRows(documentosParaEmissaoNFSmanualRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Fechamento.FechamentoFrete repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamento = repFechamentoFrete.BuscarPorCodigo(codigo);

                // Valida
                if (fechamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos repositorioFechamentoValoresOutrosRecursos = new Repositorio.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos(unitOfWork);
                Repositorio.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto repFechamentoFreteAcrescimoDesconto = new Repositorio.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto(unitOfWork);

                List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos> valoresOutrosRecursosFechamento = repositorioFechamentoValoresOutrosRecursos.BuscarPorFechamento(fechamento.Codigo);
                List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto> fechamentosFreteAcrescimoDescontos = repFechamentoFreteAcrescimoDesconto.BuscarPorFechamento(fechamento.Codigo);
                var retornoDadosFechamento = new Servicos.Embarcador.Fechamento.FechamentoFrete(unitOfWork).ObterDadosSumarizados(fechamento.Codigo, fechamento.Contrato.Codigo, fechamento.DataInicio, fechamento.DataFim);
                // Formata retorno
                var retorno = new
                {
                    fechamento.Codigo,
                    Contrato = new { fechamento.Contrato.Codigo, fechamento.Contrato.Descricao },
                    ValorPorMotorista = fechamento.Contrato.ValorPorMotorista.ToString("n2"),
                    DataInicio = fechamento.DataInicio.ToString("dd/MM/yyyy"),
                    DataFim = fechamento.DataFim.ToString("dd/MM/yyyy"),
                    PeriodoAno = fechamento.Ano,
                    PeriodoMes = fechamento.Mes,
                    PeriodoSemana = fechamento.Periodo,
                    PeriodoDezena = fechamento.Periodo,
                    PeriodoQuinzena = fechamento.Periodo,
                    fechamento.Situacao,
                    fechamento.AguardandoNFSManual,
                    TipoFechamento = fechamento.Contrato.DescricaoPeriodoAcordo,
                    TipoFranquia = fechamento.Contrato.DescricaoTipoFranquia,
                    fechamento.Contrato.TipoEmissaoComplemento,
                    EnumPeriodoAcordo = fechamento.Contrato.PeriodoAcordo,
                    DadosFechamento = retornoDadosFechamento,
                    fechamento.NaoEmitirComplemento,
                    Resumo = new
                    {
                        ValorPagar = fechamento.ValorPagar.ToString("n2"),
                        TotalDescontos = fechamento.TotalDescontos.ToString("n2"),
                        TotalAcrescimos = fechamento.TotalAcrescimos.ToString("n2"),
                        TotalAcrescimosAplicar = fechamento.TotalAcrescimosAplicar.ToString("n2"),
                        TotalDescontosAplicar = fechamento.TotalDescontosAplicar.ToString("n2"),
                        fechamento.NaoEmitirComplemento
                    },
                    ValoresOutrosRecursosFechamento = (
                        from obj in valoresOutrosRecursosFechamento
                        select new
                        {
                            Codigo = obj.Codigo,
                            Quantidade = obj.Quantidade.ToString("n0"),
                            CodigoValoresOutrosRecursos = obj.ValoresOutrosRecursos.Codigo,
                            CodigoTomador = obj.Tomador?.CPF_CNPJ ?? 0d,
                            ValoresOutrosRecursos = obj.ValoresOutrosRecursos.TipoMaoDeObra,
                            Tomador = obj.Tomador?.Descricao ?? ""
                        }
                    ).ToList(),
                    FechamentoFreteAcrescimoDesconto = (
                        from obj in fechamentosFreteAcrescimoDescontos
                        select new
                        {
                            Codigo = obj.Codigo,
                            Justificativa = obj.Justificativa.Descricao,
                            CodigoJustificativa = obj.Justificativa.Codigo,
                            TipoJustificativa = obj.Justificativa.TipoJustificativa,
                            Valor = obj.Valor.ToString("n2")
                        }
                    ).ToList(),
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
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

        public async Task<IActionResult> ReabrirFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Fechamento/FechamentoFrete");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FechamentoFrete_Reabrir))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamento = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork).BuscarPorCodigo(Request.GetIntParam("Codigo"), true);

                if (fechamento == null)
                    return new JsonpResult(false, true, "Fechamento de frete não encontrado.");

                new Servicos.Embarcador.Fechamento.FechamentoFrete(unitOfWork, Auditado).Reabrir(fechamento);

                return new JsonpResult(fechamento.Codigo);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Fechamento/FechamentoFrete");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FechamentoFrete_Finalizar))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamento = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork).BuscarPorCodigo(Request.GetIntParam("Codigo"), true);

                if (fechamento == null)
                    return new JsonpResult(false, true, "Fechamento de frete não encontrado.");

                unitOfWork.Start();

                fechamento.NaoEmitirComplemento = Request.GetBoolParam("NaoEmitirComplemento");

                SalvarValoresOutrosRecursosFechamento(fechamento, unitOfWork);
                SalvarAcrescimosDescontos(fechamento, unitOfWork);

                new Servicos.Embarcador.Fechamento.FechamentoFrete(unitOfWork, Auditado, ConfiguracaoEmbarcador).Finalizar(fechamento, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult(fechamento.Codigo);
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
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar o fechamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarFechamento()
        {

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Fechamento/FechamentoFrete");
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FechamentoFrete_Cancelar))
                return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Fechamento.FechamentoFrete repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);
                Repositorio.Embarcador.Fechamento.FechamentoFreteOcorrencia repFechamentoFreteOcorrencia = new Repositorio.Embarcador.Fechamento.FechamentoFreteOcorrencia(unitOfWork);
                Repositorio.Embarcador.Fechamento.FechamentoFreteCarga repFechamentoFreteCarga = new Repositorio.Embarcador.Fechamento.FechamentoFreteCarga(unitOfWork);
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamento = repFechamentoFrete.BuscarPorCodigo(codigo, true);

                if (fechamento.Situacao != SituacaoFechamentoFrete.Aberto)
                    return new JsonpResult(false, true, "Não é possível cancelar o fechamento em sua atual situação.");

                fechamento.Situacao = SituacaoFechamentoFrete.Cancelado;

                repFechamentoFrete.Atualizar(fechamento);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, fechamento, null, "Cancelou o Fechamento.", unitOfWork);
                // Retorna sucesso
                return new JsonpResult(fechamento.Codigo);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar o fechamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int contrato = Request.GetIntParam("Contrato");
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador = repContratoFreteTransportador.BuscarPorCodigo(contrato);

                List<int> codigosTipoDeOcorrenciaDeCTe = (from obj in contratoFreteTransportador.TiposOcorrencia select obj.Codigo).ToList();

                if (contratoFreteTransportador == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o Contrato de Frete.");

                Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodo = ObterFechamentoFretePeriodo(contratoFreteTransportador, unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = ObtemCargasDoFechamento(contrato, fechamentoFretePeriodo.DataInicio, fechamentoFretePeriodo.DataFim, unitOfWork);
                List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = ObtemOcorrenciasPorPeriodo(contratoFreteTransportador, cargas, fechamentoFretePeriodo.DataInicio, fechamentoFretePeriodo.DataFim, codigosTipoDeOcorrenciaDeCTe, unitOfWork);

                Repositorio.Embarcador.Fechamento.FechamentoFrete repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoExiste = repFechamentoFrete.BuscarFechamentoExistente(contrato, fechamentoFretePeriodo.DataInicio.Year, fechamentoFretePeriodo.DataInicio.Month, fechamentoFretePeriodo.Periodo);

                if (fechamentoExiste != null)
                    return new JsonpResult(false, true, "Já existe um fechamento que conflita com o periodo deste fechamento para esse contrato de frete.");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Servicos.Embarcador.Fechamento.FechamentoFrete servicoFechamentoFrete = new Servicos.Embarcador.Fechamento.FechamentoFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                List<string> cargasPententes = (
                    from obj in cargas
                    where (
                        obj.SituacaoCarga != SituacaoCarga.EmTransporte &&
                        obj.SituacaoCarga != SituacaoCarga.AgImpressaoDocumentos &&
                        obj.SituacaoCarga != SituacaoCarga.AgIntegracao &&
                        obj.SituacaoCarga != SituacaoCarga.Encerrada &&
                        obj.SituacaoCarga != SituacaoCarga.LiberadoPagamento
                    )
                    select obj.CodigoCargaEmbarcador
                ).ToList();

                if (cargasPententes.Count > 0)
                    return new JsonpResult(false, true, "Não é possível iniciar o fechamento enquanto existirem cargas pendentes de emissão no periodo selecionado (" + string.Join(",", cargasPententes) + ").");

                Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamento = new Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete()
                {
                    Numero = repFechamentoFrete.BuscarProximoNumero(),
                    DataFechamento = DateTime.Now,
                    Situacao = SituacaoFechamentoFrete.Aberto,
                    Contrato = contratoFreteTransportador,
                    DataInicio = fechamentoFretePeriodo.DataInicio,
                    DataFim = fechamentoFretePeriodo.DataFim,
                    Usuario = this.Usuario,
                    Ano = fechamentoFretePeriodo.DataInicio.Year,
                    Mes = fechamentoFretePeriodo.DataInicio.Month,
                    Periodo = fechamentoFretePeriodo.Periodo,
                    NaoEmitirComplemento = (configuracaoEmbarcador.TipoFechamentoFrete == TipoFechamentoFrete.FechamentoPorFaixaKm),
                    TotalAcrescimos = 0,
                    TotalDescontos = 0,
                    ValorPagar = new Servicos.Embarcador.Fechamento.FechamentoFrete(unitOfWork).ObterValorPagarPorContratoFretePeriodo(contratoFreteTransportador, fechamentoFretePeriodo.DataInicio, fechamentoFretePeriodo.DataFim),
                    ValorFinal = 0
                };

                unitOfWork.Start();

                repFechamentoFrete.Inserir(fechamento, Auditado);

                Repositorio.Embarcador.Fechamento.FechamentoFreteCarga repFechamentoFreteCarga = new Repositorio.Embarcador.Fechamento.FechamentoFreteCarga(unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                {
                    Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCarga fechamentoCarga = new Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteCarga()
                    {
                        Fechamento = fechamento,
                        Carga = carga
                    };
                    repFechamentoFreteCarga.Inserir(fechamentoCarga);
                }

                decimal acrecimo = 0;
                decimal desconto = 0;
                Repositorio.Embarcador.Fechamento.FechamentoFreteOcorrencia repFechamentoFreteOcorrencia = new Repositorio.Embarcador.Fechamento.FechamentoFreteOcorrencia(unitOfWork);

                foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia in ocorrencias)
                {

                    if (ocorrencia.ValorOcorrencia > 0)
                    {
                        if (ocorrencia.ModeloDocumentoFiscal?.TipoDocumentoCreditoDebito == TipoDocumentoCreditoDebito.Debito)
                            desconto += ocorrencia.ValorOcorrencia;
                        else
                            acrecimo += ocorrencia.ValorOcorrencia;

                        Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteOcorrencia fechamentoOcorrencia = new Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteOcorrencia()
                        {
                            Fechamento = fechamento,
                            Ocorrencia = ocorrencia
                        };

                        repFechamentoFreteOcorrencia.Inserir(fechamentoOcorrencia);
                    }
                }

                SalvarMotoristasUtilizados(fechamento, unitOfWork);
                SalvarVeiculosUtilizados(fechamento, unitOfWork);

                fechamento.TotalDescontos = desconto;
                fechamento.TotalAcrescimos = acrecimo;
                fechamento.ValorFinal = fechamento.ValorPagar + acrecimo - desconto;

                servicoFechamentoFrete.AdicionarIntegracoes(fechamento, TipoServicoMultisoftware);

                repFechamentoFrete.Atualizar(fechamento);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(fechamento.Codigo);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
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

        public async Task<IActionResult> BuscarDadosParaFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Fechamento.FechamentoFrete servicoFechamentoFrete = new Servicos.Embarcador.Fechamento.FechamentoFrete(unitOfWork);
                dynamic retorno = null;
                int fechamento = Request.GetIntParam("Fechamento");

                if (fechamento == 0)
                {
                    int contrato = Request.GetIntParam("Contrato");
                    Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                    Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador = repContratoFreteTransportador.BuscarPorCodigo(contrato);

                    if (contratoFreteTransportador == null)
                        return new JsonpResult(false, true, "Não foi possível encontrar o Contrato de Frete.");

                    Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodo = ObterFechamentoFretePeriodo(contratoFreteTransportador, unitOfWork);

                    retorno = servicoFechamentoFrete.ObterDadosSumarizados(0, contrato, fechamentoFretePeriodo.DataInicio, fechamentoFretePeriodo.DataFim);
                }
                else
                {
                    Repositorio.Embarcador.Fechamento.FechamentoFrete repFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);
                    Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentoFrete = repFechamentoFrete.BuscarPorCodigo(fechamento);

                    if (fechamentoFrete == null)
                        return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                    retorno = servicoFechamentoFrete.ObterDadosSumarizados(fechamentoFrete.Codigo, fechamentoFrete.Contrato.Codigo, fechamentoFrete.DataInicio, fechamentoFrete.DataFim);
                }

                return new JsonpResult(retorno);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
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

        public async Task<IActionResult> CargasFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.Prop("Codigo");
                grid.Prop("CodigoCargaEmbarcador").Nome("Carga").Tamanho(10).Align(Models.Grid.Align.left);
                grid.Prop("Origem").Nome("Origem").Tamanho(15).Align(Models.Grid.Align.left);
                grid.Prop("Destino").Nome("Destino").Tamanho(15).Align(Models.Grid.Align.left);
                grid.Prop("DataCarregamentoCarga").Nome("Data da Carga").Tamanho(7).Align(Models.Grid.Align.center);
                grid.Prop("PlacasVeiculos").Nome("Veículos").Tamanho(10).Align(Models.Grid.Align.left);
                grid.Prop("NomeMotoristas").Nome("Motoristas").Tamanho(10).Align(Models.Grid.Align.left);
                grid.Prop("KmTotal").Nome("Km Total").Tamanho(7).Align(Models.Grid.Align.right);
                grid.Prop("ValorContrato").Nome("Valor Contrato").Tamanho(7).Align(Models.Grid.Align.right);

                int codigoContratoFrete = Request.GetIntParam("Contrato");
                int codigoFechamentoFrete = Request.GetIntParam("Codigo");
                PeriodoAcordoContratoFreteTransportador periodoAcordo = Request.GetEnumParam<PeriodoAcordoContratoFreteTransportador>("EnumPeriodoAcordo");
                List<int> listaCodigoCargaRemovida = Request.GetListParam<int>("CargasRemovidas");
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodo = ObterFechamentoFretePeriodo(periodoAcordo, unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarCargas);
                Repositorio.Embarcador.Frete.ContratoSaldoMes repositorioContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(unitOfWork);
                int totalRegistros = repositorioContratoSaldoMes.ContarBuscaCargasParaFechamento(codigoContratoFrete, codigoFechamentoFrete, fechamentoFretePeriodo.DataInicio, fechamentoFretePeriodo.DataFim);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = totalRegistros > 0 ? repositorioContratoSaldoMes.BuscarCargasParaFechamento(codigoContratoFrete, codigoFechamentoFrete, fechamentoFretePeriodo.DataInicio, fechamentoFretePeriodo.DataFim, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                var listaCargaRetornar = (
                    from carga in cargas
                    select new
                    {
                        DT_RowColor = (listaCodigoCargaRemovida.Contains(carga.Codigo) ? "#ccc" : ""),
                        carga.Codigo,
                        carga.CodigoCargaEmbarcador,
                        Origem = carga.DadosSumarizados.Origens,
                        Destino = carga.DadosSumarizados.Destinos,
                        DataCarregamentoCarga = carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy") ?? carga.DataCriacaoCarga.ToString("dd/MM/yyyy"),
                        PlacasVeiculos = carga.DadosSumarizados.Veiculos,
                        NomeMotoristas = carga.DadosSumarizados.Motoristas,
                        KmTotal = carga.DadosSumarizados.Distancia,
                        ValorContrato = configuracaoEmbarcador.TipoFechamentoFrete == TipoFechamentoFrete.FechamentoPorKm ? carga.ValorFreteContratoFreteTotal : carga.ValorFreteAPagar
                    }
                ).ToList();

                grid.AdicionaRows(listaCargaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
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

        public async Task<IActionResult> DetalhesContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Contrato");
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoAcordo = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = repositorioContratoAcordo.BuscarPorCodigo(codigo);

                if (contrato == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                int valorKmUtilizado = 0;
                decimal valorPago = 0m;

                if (configuracaoEmbarcador.ExibirKmUtilizadoContratoFretePorPeriodoVigenciaContrato)
                {
                    valorKmUtilizado = Servicos.Embarcador.Carga.ContratoFrete.ObterKmUtilizadoContratoFreteNoPeriodoVigencia(contrato, unitOfWork);
                    valorPago = Servicos.Embarcador.Carga.ContratoFrete.ObterValorPagoContratoFreteNoPeriodoVigencia(contrato, unitOfWork);
                }
                else
                {
                    valorKmUtilizado = Servicos.Embarcador.Carga.ContratoFrete.ObterKmUtilizadoContratoFreteNoPeriodoAtual(contrato, unitOfWork);
                    valorPago = Servicos.Embarcador.Carga.ContratoFrete.ObterValorPagoContratoFreteNoPeriodoAtual(contrato, unitOfWork);
                }

                var retorno = new
                {
                    TotalPorCavalo = contrato.FranquiaTotalPorCavalo,
                    TotalKM = contrato.FranquiaTotalKM,
                    ContratoMensal = contrato.FranquiaContratoMensal.ToString("n2"),
                    ValorKM = contrato.FranquiaValorKM.ToString("n6"),
                    ValorKmExcedente = contrato.FranquiaValorKmExcedente.ToString("n2"),
                    KMConsumido = valorKmUtilizado.ToString("n0"),
                    ValorPago = valorPago.ToString("n2")
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
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

        public async Task<IActionResult> Imprimir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Fechamento.FechamentoFrete repositorioFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamentofrete = repositorioFechamentoFrete.BuscarPorCodigo(codigo);

                if (fechamentofrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                bool TipoPDF = Request.GetStringParam("Tipo") == "PDF";

                var result = ReportRequest.WithType(ReportType.FechamentoFrete)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("CodigoFechamentoFrete", fechamentofrete.Codigo.ToString())
                    .AddExtraData("TipoPDF", TipoPDF.ToString())
                    .CallReport();
                
                return Arquivo(result.GetContentFile(), "application/pdf", "Fechamento de Frete - " + fechamentofrete.Numero + "." + (TipoPDF ? "pdf" : "xls"));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        
        public async Task<IActionResult> AcordosContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo repContratoAcordo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo(unitOfWork);

                int.TryParse(Request.Params("Contrato"), out int contrato);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.Prop("Codigo");
                grid.Prop("ModeloVeicular").Nome("Modelo do Veículo").Tamanho(25).Align(Models.Grid.Align.left);
                grid.Prop("ValorAcordado").Nome("Valor Acordado").Tamanho(10).Align(Models.Grid.Align.right);
                grid.Prop("Quantidade").Nome("Qtd Veículo").Tamanho(7).Align(Models.Grid.Align.center);
                grid.Prop("Rotulo").Nome("Rótulo").Tamanho(10).Align(Models.Grid.Align.right);
                grid.Prop("Total").Nome("Total").Tamanho(10).Align(Models.Grid.Align.right);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo> acordos = repContratoAcordo.ConsultarPorContrato(contrato, parametrosConsulta);
                int totalRegistros = repContratoAcordo.ContarConsultaPorContrato(contrato);

                var lista = (from p in acordos
                             select new
                             {
                                 p.Codigo,
                                 ModeloVeicular = p.ModeloVeicular.Descricao,
                                 ValorAcordado = p.ValorAcordado.ToString("n2"),
                                 p.Quantidade,
                                 Rotulo = p.Rotulo.ToString("n2"),
                                 Total = p.Total.ToString("n2")
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

        public async Task<IActionResult> AutorizarEmissaoCTes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);

                int codigoFechamento = Request.GetIntParam("Codigo");

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo = repCargaCTeComplementoInfo.BuscarCTesRejeitadosPorFechamento(codigoFechamento);

                if (cargaCTesComplementoInfo.Count == 0)
                    return new JsonpResult(false, "Nenhum CT-e rejeitado encontrado.");

                int codigoCTe = 0;
                string mensagemRetorno = string.Empty;
                List<int> ctesProblematicos = new List<int>();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCteComplementoInfo in cargaCTesComplementoInfo)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = cargaCteComplementoInfo.CargaCTeComplementado;

                    codigoCTe = cargaCteComplementoInfo.CTe?.Codigo ?? 0;

                    if (cargaCTe != null && cargaCTe.Carga.PossuiPendencia)
                    {
                        cargaCTe.Carga.PossuiPendencia = false;
                        cargaCTe.Carga.problemaCTE = false;
                        cargaCTe.Carga.MotivoPendencia = "";
                        repCarga.Atualizar(cargaCTe.Carga);
                    }

                    if (cargaCTe != null)
                    {
                        if (codigoCTe > 0)
                        {
                            string retorno = "";
                            if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe)
                                retorno = EmitirCTe(codigoCTe, unitOfWork);

                            if (cargaCTe.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                                retorno = EmitirNFSe(codigoCTe, unitOfWork);

                            if (string.IsNullOrWhiteSpace(retorno))
                            {
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe, null, "Enviou para Emissão.", unitOfWork);
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe.CTe, null, "Enviou para Emissão.", unitOfWork);
                            }
                            else
                                ctesProblematicos.Add(cargaCteComplementoInfo.CTe.Numero);
                        }
                    }
                    else
                    {
                        if (codigoCTe > 0)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = cargaCteComplementoInfo.CTe;
                            if (cte != null)
                            {
                                string retorno = "";
                                if (cte.ModeloDocumentoFiscal.Numero == "39") //NFSe
                                    retorno = EmitirNFSe(codigoCTe, unitOfWork);
                                else
                                    retorno = EmitirCTe(codigoCTe, unitOfWork);

                                if (string.IsNullOrWhiteSpace(retorno))
                                {
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Enviou para Emissão.", unitOfWork);
                                }
                                else
                                    ctesProblematicos.Add(cte.Numero);
                            }
                        }
                    }
                }

                mensagemRetorno = ctesProblematicos.Count > 0 ? $"Os Ct-es {string.Join(", ", ctesProblematicos)} não foram reenviados." : "Os CT-es foram reenviados para emissão.";

                unitOfWork.CommitChanges();

                return new JsonpResult(!(ctesProblematicos.Count > 0), mensagemRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir o Documento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo ObterFechamentoFretePeriodo(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodo = ObterFechamentoFretePeriodo(contratoFreteTransportador.PeriodoAcordo, unitOfWork);

            if (fechamentoFretePeriodo.DataInicio == DateTime.MinValue || fechamentoFretePeriodo.DataFim == DateTime.MinValue)
                throw new ControllerException("Data de Início e Fim são obrigatórias.");

            return fechamentoFretePeriodo;
        }

        private Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo ObterFechamentoFretePeriodo(PeriodoAcordoContratoFreteTransportador periodoAcordo, Repositorio.UnitOfWork unitOfWork)
        {
            int ano = Request.GetIntParam("PeriodoAno");
            int mes = Request.GetIntParam("PeriodoMes");
            DateTime dataReferencia = new DateTime(ano, mes, 1);
            int periodo = 0;

            if (periodoAcordo == PeriodoAcordoContratoFreteTransportador.Semanal)
                periodo = Request.GetIntParam("PeriodoSemana");
            else if (periodoAcordo == PeriodoAcordoContratoFreteTransportador.Decendial)
                periodo = Request.GetIntParam("PeriodoDezena");
            else if (periodoAcordo == PeriodoAcordoContratoFreteTransportador.Quinzenal)
                periodo = Request.GetIntParam("PeriodoQuinzena");

            Dominio.ObjetosDeValor.Embarcador.Fechamento.FechamentoFretePeriodo fechamentoFretePeriodo = new Servicos.Embarcador.Fechamento.FechamentoFrete(unitOfWork).ObterFechamentoFretePeriodo(periodoAcordo, dataReferencia, periodo);

            return fechamentoFretePeriodo;
        }

        private Dominio.ObjetosDeValor.Embarcador.Fechamento.FiltroPesquisaFechamentoFrete ObterFiltrosPesquisaFechamentoFrete()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Fechamento.FiltroPesquisaFechamentoFrete()
            {
                CodigoContratoFrete = Request.GetIntParam("Contrato"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataFim"),
                Numero = Request.GetIntParam("Numero"),
                Situacao = Request.GetNullableEnumParam<SituacaoFechamentoFrete>("Situacao")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.Prop("Codigo");
                grid.Prop("Descricao");
                grid.Prop("Numero").Nome("Número").Tamanho(7).Align(Models.Grid.Align.center);
                grid.Prop("Transportador").Nome(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? "Empresa/Filial" : "Transportador").Tamanho(20).Align(Models.Grid.Align.left);
                grid.Prop("Contrato").Nome("Contrato").Tamanho(20).Align(Models.Grid.Align.left);
                grid.Prop("TipoFranquia").Nome("Tipo da Franquia").Tamanho(10).Align(Models.Grid.Align.left);
                grid.Prop("PeriodoFechamento").Nome("Período do Fechamento").Tamanho(15).Align(Models.Grid.Align.center);
                grid.Prop("Valor").Nome("Valor").Tamanho(10).Align(Models.Grid.Align.right);
                grid.Prop("Situacao").Nome("Situação").Tamanho(10).Align(Models.Grid.Align.center);

                Dominio.ObjetosDeValor.Embarcador.Fechamento.FiltroPesquisaFechamentoFrete filtrosPesquisa = ObterFiltrosPesquisaFechamentoFrete();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarFechamentoFrete);
                Repositorio.Embarcador.Fechamento.FechamentoFrete repositorioFechamentoFrete = new Repositorio.Embarcador.Fechamento.FechamentoFrete(unitOfWork);
                int totalRegistros = repositorioFechamentoFrete.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete> fechamentosFrete = (totalRegistros > 0) ? repositorioFechamentoFrete.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete>();

                var fechamentosFreteRetornar = (
                    from fechamentoFrete in fechamentosFrete
                    select new
                    {
                        fechamentoFrete.Codigo,
                        fechamentoFrete.Numero,
                        Descricao = fechamentoFrete.Descricao,
                        Transportador = fechamentoFrete.Contrato.Transportador?.RazaoSocial ?? "",
                        Contrato = fechamentoFrete.Contrato.Descricao,
                        TipoFechamento = fechamentoFrete.Contrato.DescricaoPeriodoAcordo,
                        TipoFranquia = fechamentoFrete.Contrato.DescricaoTipoFranquia,
                        PeriodoFechamento = fechamentoFrete.DataInicio.ToString("dd/MM/yyyy") + " até " + fechamentoFrete.DataFim.ToString("dd/MM/yyyy"),
                        Valor = (fechamentoFrete.ValorComplementos + fechamentoFrete.ValorFinal).ToString("n2"),
                        Situacao = fechamentoFrete.DescricaoSituacao
                    }
                ).ToList();

                grid.AdicionaRows(fechamentosFreteRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenarCargas(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Carga")
                return "CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "Origem")
                return "DadosSumarizados.Origens";

            if (propriedadeOrdenar == "Destino")
                return "DadosSumarizados.Destinos";

            return propriedadeOrdenar;
        }

        private string ObterPropriedadeOrdenarCTesFechamento(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Remetente" || propriedadeOrdenar == "Destinatario")
                return $"{propriedadeOrdenar}.Nome";

            if (propriedadeOrdenar == "Destino")
                return "LocalidadeTerminoPrestacao.Descricao";

            if (propriedadeOrdenar == "DescricaoTipoPagamento")
                return "TipoPagamento";

            if (propriedadeOrdenar == "DescricaoTipoServico")
                return "TipoServico";

            if (propriedadeOrdenar == "AbreviacaoModeloDocumentoFiscal")
                return "ModeloDocumentoFiscal.Abreviacao";

            return $"CTe.{propriedadeOrdenar}";
        }

        private string ObterPropriedadeOrdenarDocumentoParaEmissaoNFSManual(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Remetente" || propriedadeOrdenar == "Destinatario")
                return $"{propriedadeOrdenar}.Nome";

            if (propriedadeOrdenar == "Destino")
                return "Destinatario.Localidade.Descricao";

            if (propriedadeOrdenar == "NFS")
                return "CTe.Numero";

            return propriedadeOrdenar;
        }

        private string ObterPropriedadeOrdenarFechamentoFrete(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Transportador")
                return "Contrato.Transportador.RazaoSocial";

            if (propriedadeOrdenar == "Contrato")
                return "Contrato.Descricao";

            if (propriedadeOrdenar == "TipoFechamento")
                return "Contrato.DescricaoPeriodoAcordo";

            if (propriedadeOrdenar == "TipoFranquia")
                return "Contrato.DescricaoTipoFranquia";

            if (propriedadeOrdenar == "PeriodoFechamento")
                return "DataInicio, DataFim";

            if (propriedadeOrdenar == "Valor")
                return "ValorFinal";

            return propriedadeOrdenar;
        }

        private List<Dominio.Entidades.Embarcador.Cargas.Carga> ObtemCargasDoFechamento(int contrato, DateTime dataInicio, DateTime dataFim, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoSaldoMes repContratoSaldoMes = new Repositorio.Embarcador.Frete.ContratoSaldoMes(unitOfWork);

            List<int> cargasRemovidas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("CargasRemovidas"));

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repContratoSaldoMes.BuscarCargasParaFechamento(contrato, dataInicio, dataFim);

            //List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarCargasParaFechamento(contrato, dataInicio, dataFim);

            cargas = (from c in cargas where !cargasRemovidas.Contains(c.Codigo) select c).ToList();

            return cargas;
        }

        private List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ObtemOcorrenciasPorPeriodo(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador, List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas, DateTime dataInicio, DateTime dataFim, List<int> codigosTipoDeOcorrenciaDeCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            List<int> codigosCargas = cargas.Select(carga => carga.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = repositorioCargaOcorrencia.BuscarOcorrenciasParaFechamento(contratoFreteTransportador.Codigo, codigosCargas, dataInicio, dataFim, codigosTipoDeOcorrenciaDeCTe);

            return ocorrencias;
        }

        private dynamic MontarListaCTes(List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargaCTesComplementoInfo)
        {
            var lista = (from obj in cargaCTesComplementoInfo
                         where obj.CTe != null
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
                             Serie = obj.CTe.Serie.Numero,
                             obj.CTe.DescricaoTipoPagamento,
                             Remetente = obj.CTe.Remetente.Nome + "(" + obj.CTe.Remetente.CPF_CNPJ_Formatado + ")",
                             Destinatario = obj.CTe.Destinatario.Nome + "(" + obj.CTe.Destinatario.CPF_CNPJ_Formatado + ")",
                             Destino = obj.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                             ValorFrete = obj.CTe.ValorAReceber.ToString("n2"),
                             Aliquota = obj.CTe.AliquotaICMS > 0 ? obj.CTe.AliquotaICMS.ToString("n2") : obj.CTe.AliquotaISS.ToString("n4"),
                             NumeroNotas = obj.CTe.ModeloDocumentoFiscal.Numero == "39" ? string.Empty : obj.CTe.NumeroNotas,
                             Status = obj.CTe.DescricaoStatus,
                             RetornoSefaz = !string.IsNullOrWhiteSpace(obj.CTe.MensagemRetornoSefaz) ? (obj.CTe.MensagemRetornoSefaz != " - " ? System.Web.HttpUtility.HtmlEncode(obj.CTe.MensagemRetornoSefaz) : "") : "",
                             Tomador = obj.CTe.TomadorPagador != null ? obj.CTe.TomadorPagador.Nome + "(" + obj.CTe.TomadorPagador.CPF_CNPJ_Formatado + ")" : string.Empty,
                         }).ToList();

            return lista;
        }

        private string EmitirNFSe(int codigoCTe, Repositorio.UnitOfWork unitOfWork)
        {
            string mensagem = "";
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

            if (cte != null)
            {
                if (cte.SituacaoCTeSefaz == SituacaoCTeSefaz.Rejeitada || cte.SituacaoCTeSefaz == SituacaoCTeSefaz.EmDigitacao)
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

        private string EmitirCTe(int codigoCTe, Repositorio.UnitOfWork unitOfWork)
        {
            string mensagem = "";
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

            if (cte != null)
            {
                if (cte.SituacaoCTeSefaz == SituacaoCTeSefaz.Rejeitada || cte.SituacaoCTeSefaz == SituacaoCTeSefaz.EmDigitacao || cte.SituacaoCTeSefaz == SituacaoCTeSefaz.ContingenciaFSDA)
                {
                    Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

                    if (cte.DataEmissao.HasValue && cte.DataEmissao.Value < DateTime.Now.AddDays(-6))
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

        private string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return "ContratoFreteTransportadorValoresOutrosRecursos." + propriedadeOrdenarOuAgrupar;
        }

        private void SalvarValoresOutrosRecursosFechamento(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos repositorioFechamentoValoresOutrosRecursos = new Repositorio.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos repositorioValoresOutrosRecursos = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos> valoresOutrosRecursosFechamentoList = repositorioFechamentoValoresOutrosRecursos.BuscarPorFechamento(fechamento.Codigo);
            List<dynamic> valoresOutrosRecursosAdicionarOuAtualizar = Request.GetListParam<dynamic>("ValoresOutrosRecursosFechamento");

            if (valoresOutrosRecursosFechamentoList.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (dynamic valorOutroRecurso in valoresOutrosRecursosAdicionarOuAtualizar)
                {
                    int? codigo = ((string)valorOutroRecurso.Codigo).ToNullableInt();
                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos> valoresOutrosRecursoRemover = (from valorOutroRecurso in valoresOutrosRecursosFechamentoList where !listaCodigosAtualizados.Contains(valorOutroRecurso.Codigo) select valorOutroRecurso).ToList();

                foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos valorOutroRecurso in valoresOutrosRecursoRemover)
                    repositorioFechamentoValoresOutrosRecursos.Deletar(valorOutroRecurso);
            }

            foreach (dynamic valorOutroRecurso in valoresOutrosRecursosAdicionarOuAtualizar)
            {
                int? codigo = ((string)valorOutroRecurso.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos valoresOutrosRecursosSalvar;

                if (codigo.HasValue)
                    valoresOutrosRecursosSalvar = repositorioFechamentoValoresOutrosRecursos.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException("Não encontrou nenhum registro!");
                else
                    valoresOutrosRecursosSalvar = new Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteValoresOutrosRecursos()
                    {
                        Fechamento = fechamento
                    };

                int codigoValoresOutrosRecursos = ((string)valorOutroRecurso.CodigoValoresOutrosRecursos).ToInt();
                double cpfCnpjTomador = ((string)valorOutroRecurso.CodigoTomador).ToDouble();

                valoresOutrosRecursosSalvar.Quantidade = ((string)valorOutroRecurso.Quantidade).ToDecimal();
                valoresOutrosRecursosSalvar.ValoresOutrosRecursos = repositorioValoresOutrosRecursos.BuscarPorCodigo(codigoValoresOutrosRecursos);
                valoresOutrosRecursosSalvar.Tomador = (cpfCnpjTomador > 0d) ? repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjTomador) : null;

                if (valoresOutrosRecursosSalvar.Codigo > 0)
                    repositorioFechamentoValoresOutrosRecursos.Atualizar(valoresOutrosRecursosSalvar);
                else
                    repositorioFechamentoValoresOutrosRecursos.Inserir(valoresOutrosRecursosSalvar);
            }
        }

        private void SalvarMotoristasUtilizados(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado repositorioMotoristaUtilizado = new Repositorio.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado> motoristasUtilizadosFechamento = repositorioMotoristaUtilizado.BuscarPorFechamento(fechamento.Codigo);
            List<dynamic> motoristasUtilizadosAdicionarOuAtualizar = Request.GetListParam<dynamic>("MotoristasUtilizados");

            if (motoristasUtilizadosFechamento.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (dynamic motoristaUtilizado in motoristasUtilizadosAdicionarOuAtualizar)
                {
                    int? codigo = ((string)motoristaUtilizado.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado> motoristasUtilizadosRemover = (from motoristaUtilizado in motoristasUtilizadosFechamento where !listaCodigosAtualizados.Contains(motoristaUtilizado.Codigo) select motoristaUtilizado).ToList();

                foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado motoristaUtilizado in motoristasUtilizadosRemover)
                    repositorioMotoristaUtilizado.Deletar(motoristaUtilizado);
            }

            foreach (dynamic motoristaUtilizado in motoristasUtilizadosAdicionarOuAtualizar)
            {
                int? codigo = ((string)motoristaUtilizado.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado motoristaUtilizadoSalvar;

                if (codigo.HasValue)
                    motoristaUtilizadoSalvar = repositorioMotoristaUtilizado.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException("Não encontrou nenhum registro!");
                else
                    motoristaUtilizadoSalvar = new Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteMotoristaUtilizado()
                    {
                        Fechamento = fechamento
                    };

                double cpfCnpjTomador = ((string)motoristaUtilizado.CodigoTomador).ToDouble();

                motoristaUtilizadoSalvar.Tomador = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjTomador);
                motoristaUtilizadoSalvar.Quantidade = ((string)motoristaUtilizado.Quantidade).ToDecimal();
                motoristaUtilizadoSalvar.Valor = ((string)motoristaUtilizado.Valor).ToDecimal();
                motoristaUtilizadoSalvar.ValorTotal = ((string)motoristaUtilizado.Total).ToDecimal();

                if (motoristaUtilizadoSalvar.Codigo > 0)
                    repositorioMotoristaUtilizado.Atualizar(motoristaUtilizadoSalvar);
                else
                    repositorioMotoristaUtilizado.Inserir(motoristaUtilizadoSalvar);
            }
        }

        private void SalvarVeiculosUtilizados(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado repositorioVeiculoUtilizado = new Repositorio.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado> veiculosUtilizadosFechamento = repositorioVeiculoUtilizado.BuscarPorFechamento(fechamento.Codigo);
            List<dynamic> veiculosUtilizadosAdicionarOuAtualizar = Request.GetListParam<dynamic>("VeiculosUtilizados");

            if (veiculosUtilizadosFechamento.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (dynamic veiculoUtilizado in veiculosUtilizadosAdicionarOuAtualizar)
                {
                    int? codigo = ((string)veiculoUtilizado.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado> veiculosUtilizadosRemover = (from veiculoUtilizado in veiculosUtilizadosFechamento where !listaCodigosAtualizados.Contains(veiculoUtilizado.Codigo) select veiculoUtilizado).ToList();

                foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado veiculoUtilizado in veiculosUtilizadosRemover)
                    repositorioVeiculoUtilizado.Deletar(veiculoUtilizado);
            }

            foreach (dynamic veiculoUtilizado in veiculosUtilizadosAdicionarOuAtualizar)
            {
                int? codigo = ((string)veiculoUtilizado.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado veiculoUtilizadoSalvar;

                if (codigo.HasValue)
                    veiculoUtilizadoSalvar = repositorioVeiculoUtilizado.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException("Não encontrou nenhum registro!");
                else
                    veiculoUtilizadoSalvar = new Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteVeiculoUtilizado()
                    {
                        Fechamento = fechamento
                    };

                int codigoModeloVeicularCarga = ((string)veiculoUtilizado.CodigoModeloVeicularCarga).ToInt();
                double cpfCnpjTomador = ((string)veiculoUtilizado.CodigoTomador).ToDouble();

                veiculoUtilizadoSalvar.ModeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicularCarga);
                veiculoUtilizadoSalvar.Tomador = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjTomador);
                veiculoUtilizadoSalvar.Quantidade = ((string)veiculoUtilizado.Quantidade).ToDecimal();
                veiculoUtilizadoSalvar.Valor = ((string)veiculoUtilizado.Valor).ToDecimal();
                veiculoUtilizadoSalvar.ValorTotal = ((string)veiculoUtilizado.Total).ToDecimal();

                if (veiculoUtilizadoSalvar.Codigo > 0)
                    repositorioVeiculoUtilizado.Atualizar(veiculoUtilizadoSalvar);
                else
                    repositorioVeiculoUtilizado.Inserir(veiculoUtilizadoSalvar);
            }
        }

        private void SalvarAcrescimosDescontos(Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamento, UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto repFechamentoFreteAcrescimoDesconto = new Repositorio.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto(unitOfWork);
            Repositorio.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto repFechamentoJustificativaAcrescimoDesconto = new Repositorio.Embarcador.Fechamento.FechamentoJustificativaAcrescimoDesconto(unitOfWork);

            List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto> fechamentosFreteAcrescimoDescontos = repFechamentoFreteAcrescimoDesconto.BuscarPorFechamento(fechamento.Codigo);
            List<dynamic> fechamentosFreteAcrescimoDescontosAdicionarOuAtualizar = Request.GetListParam<dynamic>("FechamentoFreteAcrescimoDesconto");

            if (fechamentosFreteAcrescimoDescontos.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (dynamic fechamentoFreteAcrescimoDesconto in fechamentosFreteAcrescimoDescontosAdicionarOuAtualizar)
                {
                    int? codigo = ((string)fechamentoFreteAcrescimoDesconto.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto> fechamentosFreteAcrescimoDescontosRemover = (from fechamentoFreteAcrescimoDesconto in fechamentosFreteAcrescimoDescontos where !listaCodigosAtualizados.Contains(fechamentoFreteAcrescimoDesconto.Codigo) select fechamentoFreteAcrescimoDesconto).ToList();

                foreach (Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto fechamentoFreteAcrescimoDesconto in fechamentosFreteAcrescimoDescontosRemover)
                    repFechamentoFreteAcrescimoDesconto.Deletar(fechamentoFreteAcrescimoDesconto);
            }

            List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto> listaFechamentoFreteAcrescimoDescontos = new List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto>();

            foreach (dynamic fechamentoFreteAcrescimoDesconto in fechamentosFreteAcrescimoDescontosAdicionarOuAtualizar)
            {
                int? codigo = ((string)fechamentoFreteAcrescimoDesconto.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto fechamentoFreteAcrescimoDescontoSalvar;

                if (codigo.HasValue)
                    fechamentoFreteAcrescimoDescontoSalvar = repFechamentoFreteAcrescimoDesconto.BuscarPorCodigo(codigo.Value, auditavel: false) ?? throw new ControllerException("Não encontrou nenhum registro!");
                else
                    fechamentoFreteAcrescimoDescontoSalvar = new Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto()
                    {
                        Fechamento = fechamento
                    };

                int codigoJustificativa = ((string)fechamentoFreteAcrescimoDesconto.CodigoJustificativa).ToInt();
                fechamentoFreteAcrescimoDescontoSalvar.Justificativa = repFechamentoJustificativaAcrescimoDesconto.BuscarPorCodigo(codigoJustificativa, auditavel: false);
                fechamentoFreteAcrescimoDescontoSalvar.Valor = ((string)fechamentoFreteAcrescimoDesconto.Valor).ToDecimal();

                if (fechamentoFreteAcrescimoDescontoSalvar.Codigo > 0)
                    repFechamentoFreteAcrescimoDesconto.Atualizar(fechamentoFreteAcrescimoDescontoSalvar);
                else
                    repFechamentoFreteAcrescimoDesconto.Inserir(fechamentoFreteAcrescimoDescontoSalvar);

                listaFechamentoFreteAcrescimoDescontos.Add(fechamentoFreteAcrescimoDescontoSalvar);
            }

            AtualizarTotalizadoresFechamento(listaFechamentoFreteAcrescimoDescontos, fechamento);
        }

        private void AtualizarTotalizadoresFechamento(List<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteAcrescimoDesconto> listaFechamentoFreteAcrescimoDescontos, Dominio.Entidades.Embarcador.Fechamento.FechamentoFrete fechamento)
        {
            fechamento.TotalAcrescimosAplicar = listaFechamentoFreteAcrescimoDescontos.Where(o => o.Justificativa.TipoJustificativa == TipoJustificativaPesquisa.Acrescimo).Sum(o => o.Valor);
            fechamento.TotalDescontosAplicar = listaFechamentoFreteAcrescimoDescontos.Where(o => o.Justificativa.TipoJustificativa == TipoJustificativaPesquisa.Desconto).Sum(o => o.Valor);
            fechamento.ValorFinal = fechamento.ValorPagar + fechamento.TotalAcrescimos - fechamento.TotalDescontos;

            //valorPagarComAcrescimosEDescontos
            if ((fechamento.ValorPagar - fechamento.TotalAcrescimosAplicar + fechamento.TotalDescontosAplicar) < 0)
                throw new ServicoException("Não é possível aplicar um desconto maior que o valor já pago.");
        }

        #endregion
    }
}
