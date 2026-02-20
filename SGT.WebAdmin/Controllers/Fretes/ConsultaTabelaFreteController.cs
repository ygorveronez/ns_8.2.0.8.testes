using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/ConsultaTabelaFrete")]
    public class ConsultaTabelaFreteController : BaseController
    {
		#region Construtores

		public ConsultaTabelaFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R004_TabelaFreteValor;
        private readonly int _numeroMaximoComplementos = 15;
        private readonly decimal _tamanhoColunasValores = (decimal)1.75;
        private readonly decimal _tamanhoColunasParticipantes = (decimal)5.50;
        private readonly decimal _tamanhoColunasEnderecoParticipantes = 3;
        private int _ultimaColunaDinamica = 1;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTabelaFrete = 0;
                int.TryParse(Request.Params("TabelaFrete"), out codigoTabelaFrete);

                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);

                if (tabelaFrete == null)
                    return new JsonpResult(false, "Tabela de frete não encontrada.");

                unitOfWork.Start();

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                int codigoRelatorio = int.Parse(Request.Params("Codigo"));

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = serRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Tabelas de Frete", "Fretes", "ConsultaTabelaFrete.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Remetente", "asc", "", "", codigoRelatorio, unitOfWork, true, true, 8);

                Models.Grid.Grid grid = ObterGridPadrao(tabelaFrete);

                if (!mdlRelatorio.ConferirColunasDinamicas(ref relatorio, ref grid, ref _ultimaColunaDinamica, _numeroMaximoComplementos, _tamanhoColunasValores, "DescricaoValorComponente"))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Não é possível carregar esse relatório pois existem componentes que foram inativados, por favor crie um novo padrão.");
                }

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(grid, relatorio);

                unitOfWork.CommitChanges();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                int totalRegistros = repositorioTabelaFreteCliente.ContarConsulta(filtrosPesquisa, propriedades);
                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete> listaConsultaTabelaFrete = (totalRegistros > 0) ? repositorioTabelaFreteCliente.Consultar(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConsultaTabelaFrete>();

                grid.AdicionaRows(listaConsultaTabelaFrete);
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

        public async Task<IActionResult> GerarRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                filtrosPesquisa.IsRelatorio = true;

                unitOfWork.Start();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = repositorioRelatorio.BuscarPorCodigo(dynRelatorio.Codigo);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = servicoRelatorio.AdicionarRelatorioParaGeracao(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                unitOfWork.CommitChanges();

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemporario.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);
                string stringConexao = _conexao.StringConexao;

                if (relatorioControleGeracao.TipoArquivoRelatorio == Dominio.Enumeradores.TipoArquivoRelatorio.XLS)
                    filtrosPesquisa.IsCSV = true;

                Task.Factory.StartNew(() => GerarRelatorioConsultaTabelaFrete(filtrosPesquisa, propriedades, parametrosConsulta, relatorioControleGeracao, relatorioTemporario, stringConexao));

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarValorItem()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoItem = Request.GetIntParam("CodigoItem");
                Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repositorioItem = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item = repositorioItem.BuscarPorCodigo(codigoItem);

                if (item == null)
                    return new JsonpResult(false, "O item não foi encontrado para edição. Atualize a página e tente novamente.");

                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = item.ParametroBaseCalculo?.TabelaFrete ?? item.TabelaFrete;

                if (new Servicos.Embarcador.Frete.MensagemAlertaTabelaFreteCliente(unidadeDeTrabalho).IsMensagemSemConfirmacao(tabelaFreteCliente, TipoMensagemAlerta.AjusteTabelaFreteCliente))
                    return new JsonpResult(false, "Não é possível alterar valores da tabela de frete com ajuste aguardando retorno");

                Servicos.Embarcador.Frete.TabelaFreteCliente servcoTabelaFreteCliente = new Servicos.Embarcador.Frete.TabelaFreteCliente(unidadeDeTrabalho);

                servcoTabelaFreteCliente.DefinirValorItem(item, Request.GetDecimalParam("ValorItem"), Auditado);
                repositorioItem.Atualizar(item);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o valor do item.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarConfiguracaoRelatorio()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTabelaFrete = 0;
                int.TryParse(Request.Params("TabelaFrete"), out codigoTabelaFrete);

                string jsonRelatorio = Request.Params("Relatorio");
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(jsonRelatorio);

                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);

                if (tabelaFrete == null)
                    return new JsonpResult(false, "Tabela de frete não encontrada para salvar a configuração de impressão. Atualize a página e tente novamente.");

                Models.ServicoRelatorio svcRelatorio = new Models.ServicoRelatorio();

                dynRelatorio.Descricao = tabelaFrete.Descricao;

                if (tabelaFrete.Relatorios.Count > 0)
                {
                    dynRelatorio.NovoRelatorio = false;
                    dynRelatorio.Codigo = tabelaFrete.Relatorios.First().Codigo;
                }
                else
                {
                    dynRelatorio.NovoRelatorio = true;
                }

                unidadeDeTrabalho.Start();

                int codigoRelatorio = svcRelatorio.SalvarConfiguracaoRelatorio(jsonRelatorio, this.Usuario, unidadeDeTrabalho, TipoServicoMultisoftware, ConfiguracaoEmbarcador.UsaPermissaoControladorRelatorios);
                tabelaFrete.Relatorios = new List<Dominio.Entidades.Embarcador.Relatorios.Relatorio>
                {
                    new Dominio.Entidades.Embarcador.Relatorios.Relatorio() { Codigo = codigoRelatorio }
                };

                repTabelaFrete.Atualizar(tabelaFrete);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(codigoRelatorio);
            }
            catch (ServicoException ex)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a configuração do relatório.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void GerarRelatorioConsultaTabelaFrete(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            try
            {
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                var result = ReportRequest.WithType(ReportType.ConsultaTabelaFrete)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("filtrosPesquisa", filtrosPesquisa.ToJson())
                    .AddExtraData("propriedades", propriedades.ToJson())
                    .AddExtraData("parametrosConsulta", parametrosConsulta.ToJson())
                    .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("relatorioTemporario", servicoRelatorio.ObterConfiguracaoRelatorio(relatorioTemporario).ToJson())
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                    servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, result.ErrorMessage);

            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoTabelaFrete = Request.GetIntParam("TabelaFrete");
            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
            Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repositorioTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);

            Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaTabelaFreteCliente()
            {
                ParametroBase = tabelaFrete.ParametroBase,
                CodigoTabelaFrete = tabelaFrete.Codigo,
                CodigosEmpresa = Request.GetListParam<int>("Empresa"),
                CodigoEstadoDestino = Request.GetStringParam("EstadoDestino"),
                CodigoEstadoOrigem = Request.GetStringParam("EstadoOrigem"),
                CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                CodigoLocalidadeDestino = Request.GetIntParam("LocalidadeDestino"),
                CodigoLocalidadeOrigem = Request.GetIntParam("LocalidadeOrigem"),
                CodigoModeloTracao = Request.GetIntParam("ModeloTracao"),
                CodigoRegiaoDestino = Request.GetIntParam("RegiaoDestino"),
                CodigoRotaFreteOrigem = Request.GetIntParam("RotaOrigem"),
                CodigoTipoCarga = Request.GetIntParam("TipoCarga"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigosComplemento = Request.GetListParam<int>("ComponentesFrete"),
                CodigosModeloReboque = Request.GetListParam<int>("ModeloReboque"),
                CodigosRotaFreteDestino = Request.GetListParam<int>("RotaDestino"),
                CodigoStatusAceiteValor = Request.GetIntParam("StatusAceiteValor"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                CpfCnpjRemetente = Request.GetDoubleParam("Remetente"),
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                CpfCnpjTransportadorTerceiro = Request.GetDoubleParam("TransportadorTerceiro"),
                DataFinalAlteracao = Request.GetNullableDateTimeParam("DataFinalAlteracao"),
                DataFinalVigencia = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicialAlteracao = Request.GetNullableDateTimeParam("DataInicialAlteracao"),
                DataInicialVigencia = Request.GetNullableDateTimeParam("DataInicial"),
                ExibirHistoricoAlteracao = Request.GetBoolParam("ExibirHistoricoAlteracao"),
                TabelaComCargaRealizada = Request.GetBoolParam("TabelaComCargaRealizada"),
                TipoPagamentoEmissao = Request.GetNullableEnumParam<TipoPagamentoEmissao>("TipoPagamento"),
                SomenteEmVigencia = Request.GetBoolParam("SomenteEmVigencia"),
                SituacaoAlteracao = Request.GetEnumParam<SituacaoAlteracaoTabelaFrete>("SituacaoAlteracao"),
                SomenteRegistrosComValores = Request.GetBoolParam("SomenteRegistrosComValores"),
                CodigosContratoTransportador = Request.GetListParam<int>("ContratoTransportador"),
                CodigoIntegracaoTabelaFreteCliente = Request.GetStringParam("CodigoIntegracaoTabelaFreteCliente"),
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            _ultimaColunaDinamica = 1;

            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Código", "CodigoIntegracao", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código de Integração", "CodigoIntegracaoTabelaFreteCliente", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data da Alteração", "DataAlteracao", _tamanhoColunasEnderecoParticipantes, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Usuário da Alteração", "UsuarioAlteracao", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tabela de Frete", "TabelaFrete", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Remetente", "DescricaoRemetente", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Nome Fantasia Remetente", "NomeFantasiaRemetente", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Remetente", "CPFCNPJRemetente", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Origem", "Origem", _tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Estado Origem", "EstadoOrigem", _tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Região Origem", "RegiaoOrigem", _tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CEP Origem", "CEPOrigem", _tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("País Origem", "PaisOrigem", _tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Rota Origem", "RotaFreteOrigem", _tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Destinatário", "DescricaoDestinatario", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Nome Fantasia Destinatário", "NomeFantasiaDestinatario", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CPF/CNPJ Destinatário", "CPFCNPJDestinatario", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Destino", "Destino", _tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Região Destino", "RegiaoDestino", _tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Estado Destino", "EstadoDestino", _tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CEP Destino", "CEPDestino", _tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("País Destino", "PaisDestino", _tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Rota Destino", "RotaFrete", _tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Tomador", "Tomador", _tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Transportador", "DescricaoEmpresa", _tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Lead Time", "LeadTime", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Status Aprovação", "StatusAprovacao", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);

            if (tabelaFrete.ParametroBase.HasValue)
                grid.AdicionarCabecalho("Base (" + tabelaFrete.ParametroBase.Value.ObterDescricao() + ")", "ParametroBase", _tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);

            grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", _tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Inicio Vigência", "DataInicial", _tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Fim Vigência", "DataFinal", _tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Pagamento", "DescricaoTipoPagamento", _tamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", _tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);

            if (tabelaFrete.NumeroEntregas.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != TipoParametroBaseTabelaFrete.NumeroEntrega))
            {
                grid.AdicionarCabecalho("Entrega", "NumeroEntrega", _tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Entrega", "DescricaoValorEntrega", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Entrega", "DescricaoAntigoValorEntrega", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);

                if (tabelaFrete.PermiteValorAdicionalPorEntregaExcedente)
                {
                    grid.AdicionarCabecalho("Valor a Cada Entrega Excedente", "DescricaoValorEntregaExcedente", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                    grid.AdicionarCabecalho("Antigo Valor a Cada Entrega Excedente", "DescricaoAntigoValorEntregaExcedente", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                }
            }

            if (tabelaFrete.TipoEmbalagens.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != TipoParametroBaseTabelaFrete.TipoEmbalagem))
            {
                grid.AdicionarCabecalho("Tipo de Embalagem", "TipoEmbalagem", _tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tipo de Embalagem", "DescricaoValorTipoEmbalagem", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Tipo de Embalagem", "DescricaoAntigoValorTipoEmbalagem", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            }

            if (tabelaFrete.PesosTransportados.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != TipoParametroBaseTabelaFrete.Peso))
            {
                grid.AdicionarCabecalho("Peso", "DescricaoPeso", _tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Peso", "DescricaoValorPeso", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Peso", "DescricaoAntigoValorPeso", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);

                if (tabelaFrete.PermiteValorAdicionalPorPesoExcedente)
                {
                    grid.AdicionarCabecalho($"Valor a Cada {tabelaFrete.PesoExcecente.ToString("n3")}{tabelaFrete.PesosTransportados.Select(o => o.UnidadeMedida.Sigla).FirstOrDefault()} Excedente", "DescricaoValorPesoExcedente", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                    grid.AdicionarCabecalho($"Antigo Valor a Cada {tabelaFrete.PesoExcecente.ToString("n3")}{tabelaFrete.PesosTransportados.Select(o => o.UnidadeMedida.Sigla).FirstOrDefault()} Excedente", "DescricaoAntigoValorPesoExcedente", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                }
            }

            if (tabelaFrete.Pallets.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != TipoParametroBaseTabelaFrete.Pallets))
            {
                grid.AdicionarCabecalho("Pallets", "NumeroPallets", _tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Pallets", "DescricaoValorPallets", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Pallets", "DescricaoAntigoValorPallets", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);

                if (tabelaFrete.PermiteValorAdicionalPorPalletExcedente)
                {
                    grid.AdicionarCabecalho("Valor a Cada Pallet Excedente", "DescricaoValorPalletsExcedente", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                    grid.AdicionarCabecalho("Antigo Valor a Cada Pallet Excedente", "DescricaoAntigoValorPalletsExcedente", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                }
            }

            if (tabelaFrete.Distancias.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != TipoParametroBaseTabelaFrete.Distancia))
            {
                grid.AdicionarCabecalho("Distância", "DescricaoDistancia", _tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Distância", "DescricaoValorDistancia", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Distância", "DescricaoAntigoValorDistancia", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);

                if (tabelaFrete.PermiteValorAdicionalPorQuilometragemExcedente)
                {
                    grid.AdicionarCabecalho($"Valor a Cada {tabelaFrete.QuilometragemExcedente.ToString("n2")}km Excedente", "DescricaoValorDistanciaExcedente", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                    grid.AdicionarCabecalho($"Antigo Valor a Cada {tabelaFrete.QuilometragemExcedente.ToString("n2")}km Excedente", "DescricaoAntigoValorDistanciaExcedente", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                }
            }

            if (tabelaFrete.Ajudantes.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Ajudante))
            {
                grid.AdicionarCabecalho("Ajudante", "NumeroAjudante", _tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Ajudante", "DescricaoValorAjudante", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Ajudante", "DescricaoAntigoValorAjudante", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);

                if (tabelaFrete.PermiteValorAdicionalPorAjudanteExcedente)
                {
                    grid.AdicionarCabecalho("Valor a Cada Ajudante Excedente", "DescricaoValorAjudanteExcedente", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                    grid.AdicionarCabecalho("Antigo Valor a Cada Ajudante Excedente", "DescricaoAntigoValorAjudanteExcedente", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                }
            }

            if (!tabelaFrete.NaoPermitirLancarValorPorTipoDeCarga && tabelaFrete.TiposCarga.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != TipoParametroBaseTabelaFrete.TipoCarga))
            {
                grid.AdicionarCabecalho("Carga", "TipoCarga", _tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Carga", "DescricaoValorTipoCarga", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Carga", "DescricaoAntigoValorTipoCarga", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            }

            if (tabelaFrete.ModelosReboque.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != TipoParametroBaseTabelaFrete.ModeloReboque))
            {
                grid.AdicionarCabecalho("Reboque", "ModeloReboque", _tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Reboque", "DescricaoValorModeloReboque", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Reboque", "DescricaoAntigoValorModeloReboque", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            }

            if (tabelaFrete.ModelosTracao.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != TipoParametroBaseTabelaFrete.ModeloTracao))
            {
                grid.AdicionarCabecalho("Tração", "ModeloTracao", _tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tração", "DescricaoValorModeloTracao", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Tração", "DescricaoAntigoValorModeloTracao", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            }

            grid.AdicionarCabecalho("Valor Total", "ValorTotal", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Antigo Valor Total", "AntigoValorTotal", _tamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);

            if (tabelaFrete.PossuiMinimoGarantido)
            {
                grid.AdicionarCabecalho("Valor Mínimo", "ValorMinimo", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Mínimo", "AntigoValorMinimo", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            }

            if (tabelaFrete.PossuiValorMaximo)
            {
                grid.AdicionarCabecalho("Valor Máximo", "ValorMaximo", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Máximo", "AntigoValorMaximo", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            }

            if (tabelaFrete.PossuiValorBase)
            {
                grid.AdicionarCabecalho("Valor Base", "ValorBase", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
                grid.AdicionarCabecalho("Antigo Valor Base", "AntigoValorBase", _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            }

            grid.AdicionarCabecalho("Terceiro", "TransportadorTerceiro", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Prazo Dias Úteis", "PrazoDiasUteis", _tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Canal de Entrega", "CanalEntrega", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Canal de Venda", "CanalVenda", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Grupo de Carga", "DescricaoGrupoCarga", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Gerenciar Capacidade", "DescricaoGerenciarCapacidade", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Estrutura de Tabela", "DescricaoEstruturaTabela", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Item", "ItemCodigoFormatado", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Item Retorno Integração", "ItemCodigoRetornoIntegracao", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Mensagem Retorno Integração", "MensagemRetornoIntegracao", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Descrição Contrato", "DescricaoContrato", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Contrato", "NumeroContrato", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Situação Tabela", "DescricaoSituacaoTabela", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tabela Com Vinculo Carga", "DescricaoTabelaComVinculoCarga", _tamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            if (tabelaFrete.ContratosTransporteFrete.Count > 0)
            {
                grid.AdicionarCabecalho("Capacidade OTM", "DescricaoCapacidadeOTM", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Percentual Rota", "PercentualRotaFormatado", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Quantidade Entregas", "QuantidadeEntregas", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Ponto Planejamento Transporte", "PontoPlanejamentoTransporteDescricao", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Pis Embarcador", "PisEmbarcadorFormatado", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Cofins Embarcador", "CofinsEmbarcadorFormatado", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Líquido (Embarcador)", "ValorLiquidoEmbarcadorFormatado", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Pis Transportador", "PisTransportadorFormatado", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Cofins Transportador", "CofinsTransportadorFormatado", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho("Valor Líquido (Transportador)", "ValorLiquidoTransportadorFormatado", _tamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);
            }

            //Colunas montadas dinamicamente
            for (int i = 0; i < tabelaFrete.Componentes.Count; i++)
            {
                if (i < _numeroMaximoComplementos)
                {
                    grid.AdicionarCabecalho(tabelaFrete.Componentes[i].ComponenteFrete.Descricao, "DescricaoValorComponente" + _ultimaColunaDinamica.ToString(), _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.nenhum, tabelaFrete.Componentes[i].Codigo, 0, "Decimal");
                    grid.AdicionarCabecalho("Antigo Valor " + tabelaFrete.Componentes[i].ComponenteFrete.Descricao, "DescricaoAntigoValorComponente" + _ultimaColunaDinamica.ToString(), _tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.nenhum, tabelaFrete.Componentes[i].Codigo, 0, "Decimal");
                    _ultimaColunaDinamica++;
                }
                else
                    break;
            }

            return grid;
        }

        
        #endregion
    }
}
