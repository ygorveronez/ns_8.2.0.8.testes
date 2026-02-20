using Dominio.ObjetosDeValor.Embarcador.RH;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.RH
{
    [CustomAuthorize("RH/ComissaoFuncionario")]
    public class ComissaoFuncionarioMotoristaDocumentoController : BaseController
    {
		#region Construtores

		public ComissaoFuncionarioMotoristaDocumentoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("RH/ComissaoFuncionario");
                Repositorio.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista repositorioConfiguracaoComissaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista configuracaoComissaoMotorista = repositorioConfiguracaoComissaoMotorista.BuscarConfiguracaoPadrao();
                Models.Grid.EditableCell editableValorLiquido = null;

                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ComissaoFuncionario_PermiteEditarDadosGerados))
                {
                    editableValorLiquido = new Models.Grid.EditableCell(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoColunaGrid.aDecimal, 9);
                }

                Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento repComissaoFuncionarioMotoristaDocumento = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento(unitOfWork);

                int comissaoFuncionarioMotorista = 0;

                int.TryParse(Request.Params("ComissaoFuncionarioMotorista"), out comissaoFuncionarioMotorista);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("T. Doc", "TipoDocumento", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("UF Origem", "UFOrigem", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Remetente", "Remetente", 15, Models.Grid.Align.left, false);
                if (ConfiguracaoEmbarcador.UtilizarComissaoPorCargo)
                {
                    grid.AdicionarCabecalho("PercentualExecucao", false);
                    grid.AdicionarCabecalho("PercentualUtilizado", false);
                }
                else
                {
                    grid.AdicionarCabecalho("% Exec", "PercentualExecucao", 5, Models.Grid.Align.right, true, false, false, false, true, editableValorLiquido);
                    grid.AdicionarCabecalho("% Uti", "PercentualUtilizado", 5, Models.Grid.Align.right, true, false, false, false, true);
                }
                grid.AdicionarCabecalho("UF Destino", "UFDestino", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Total Frete", "ValorTotalFrete", 8, Models.Grid.Align.right, true);
                if (ConfiguracaoEmbarcador.UtilizarComissaoPorCargo)
                    grid.AdicionarCabecalho("Frete Líquido", "ValoFreteLiquido", 8, Models.Grid.Align.right, true, false, false, false, true);
                else
                    editableValorLiquido = configuracaoComissaoMotorista.BloquearAlteracaoValorFreteLiquido ? null : editableValorLiquido;
                    grid.AdicionarCabecalho("Frete Líquido", "ValoFreteLiquido", 8, Models.Grid.Align.right, true, false, false, false, true, editableValorLiquido);
                grid.AdicionarCabecalho("Frete ICMS", "ValorICMS", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Frete Pedágio", "ValorPedagio", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Frete Outros", "OutrosValores", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("B. Calculo", "ValoBaseCalculo", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Comissão", "ValorComissao", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Data de Carregamento", "DataDeCarregamento", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Origem", "Origem", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Destino", "Destino", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Placa", "Placa", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 8, Models.Grid.Align.right, true);

                FiltroPesquisaDocumenotosComissaoFuncionario filtroPesquisa = ObterFiltrosPesquisa();

                List<Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento> listaComissaoFuncionarioMotoristaDocumento = repComissaoFuncionarioMotoristaDocumento.Consultar(filtroPesquisa, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repComissaoFuncionarioMotoristaDocumento.ContarConsulta(filtroPesquisa));
                var lista = (
                    from obj in listaComissaoFuncionarioMotoristaDocumento 
                    select retornarComissaoFuncionarioDocumentosDadosGrid(obj)).ToList();
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

        public async Task<IActionResult> AdicionarDocumentoComissaoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento repComissaoFuncionarioMotoristaDocumento = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaComponentesFrete repCargaComponentesFrete = new Repositorio.Embarcador.Cargas.CargaComponentesFrete(unitOfWork);
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista repComissaoFuncionarioMotorista = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotorista(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete repCargaCTeComponentesFrete = new Repositorio.Embarcador.Cargas.CargaCTeComponentesFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            try
            {
                unitOfWork.Start();

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                int codigoOcorrencia = Request.GetIntParam("CodigoOcorrencia");

                Servicos.Embarcador.RH.ComissaoFuncionario serComissaoFuncionario = new Servicos.Embarcador.RH.ComissaoFuncionario(unitOfWork);
                Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista comissaoFuncionarioMotorista = repComissaoFuncionarioMotorista.BuscarPorCodigo(codigo);

                if (comissaoFuncionarioMotorista != null && comissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Cancelada
                   && comissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Finalizada)
                {
                    if (codigoCarga > 0)
                    {
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                        if (carga == null)
                            return new JsonpResult(false, true, "Carga nao localizada.");

                        decimal percentualComissaoParcial = 100m;

                        if (carga.TipoOperacao?.GerarComissaoParcialMotorista ?? false)
                            percentualComissaoParcial = carga.TipoOperacao.PercentualComissaoParcialMotorista;

                        List<Dominio.Entidades.Embarcador.Cargas.CargaComponentesFrete> cargaComponentesFrete = repCargaComponentesFrete.BuscarPorCargaSemComponenteCompoeFreteValor(carga.Codigo);
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCargaCTe.BuscarPrimeiroCTePorCarga(carga.Codigo);

                        Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento comissaoFuncionarioMotoristaDocumento = new Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento();
                        comissaoFuncionarioMotoristaDocumento.Carga = carga;
                        comissaoFuncionarioMotoristaDocumento.Numero = carga.CodigoCargaEmbarcador;
                        comissaoFuncionarioMotoristaDocumento.DataEmissao = cte?.DataEmissao ?? DateTime.Now;
                        comissaoFuncionarioMotoristaDocumento.Veiculo = carga.Veiculo;
                        comissaoFuncionarioMotoristaDocumento.PercentualComissaoParcial = percentualComissaoParcial;

                        comissaoFuncionarioMotoristaDocumento.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                        if (carga.VeiculosVinculados != null)
                        {
                            foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados)
                            {
                                comissaoFuncionarioMotoristaDocumento.VeiculosVinculados.Add(reboque);
                            }
                        }
                        decimal valorFreteLiquido = carga.ValorFreteLiquido;

                        comissaoFuncionarioMotoristaDocumento.TipoDocumentoComissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoComissao.carga;
                        comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados = carga.DadosSumarizados;
                        comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista = comissaoFuncionarioMotorista;

                        comissaoFuncionarioMotoristaDocumento.OutrosValores = (from obj in cargaComponentesFrete where obj.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO && !obj.SomarComponenteFreteLiquido && !obj.DescontarComponenteFreteLiquido select obj.ValorComponente).Sum();
                        comissaoFuncionarioMotorista.OutrosValores += comissaoFuncionarioMotoristaDocumento.OutrosValores;

                        comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo = Math.Round(((valorFreteLiquido * (percentualComissaoParcial / 100)) * (comissaoFuncionarioMotorista.ComissaoFuncionario.PercentualBaseCalculoComissao / 100)), 2, MidpointRounding.AwayFromZero);
                        comissaoFuncionarioMotoristaDocumento.ValoBaseCalculoOriginal = comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo;
                        comissaoFuncionarioMotorista.ValoBaseCalculo += comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo;

                        comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido = valorFreteLiquido; //carga.ValorFreteLiquido;
                        comissaoFuncionarioMotoristaDocumento.ValoFreteLiquidoOriginal = comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido;
                        comissaoFuncionarioMotorista.ValoFreteLiquido += comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido;

                        comissaoFuncionarioMotoristaDocumento.ValorComissao = Math.Round(comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo * (comissaoFuncionarioMotorista.ComissaoFuncionario.PercentualComissao / 100), 2, MidpointRounding.AwayFromZero);
                        comissaoFuncionarioMotoristaDocumento.ValorComissaoOriginal = comissaoFuncionarioMotoristaDocumento.ValorComissao;
                        comissaoFuncionarioMotoristaDocumento.PercentualExecucao = 100;
                        comissaoFuncionarioMotorista.ValorComissao += comissaoFuncionarioMotoristaDocumento.ValorComissao;

                        comissaoFuncionarioMotoristaDocumento.ValorICMS = repCargaCTeComponentesFrete.BuscarValorComponentePorCarga(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ICMS);// carga.ValorICMS;
                        comissaoFuncionarioMotorista.ValorICMS += comissaoFuncionarioMotoristaDocumento.ValorICMS;

                        comissaoFuncionarioMotoristaDocumento.ValorPedagio = (from obj in cargaComponentesFrete where obj.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.PEDAGIO && !obj.SomarComponenteFreteLiquido && !obj.DescontarComponenteFreteLiquido select obj.ValorComponente).Sum();
                        comissaoFuncionarioMotorista.ValorPedagio += comissaoFuncionarioMotoristaDocumento.ValorPedagio;

                        comissaoFuncionarioMotoristaDocumento.ValorTotalFrete = repCargaCTe.BuscarValorAReceberPorCarga(carga.Codigo);
                        comissaoFuncionarioMotoristaDocumento.ValorTotalFreteOriginal = comissaoFuncionarioMotoristaDocumento.ValorTotalFrete;
                        comissaoFuncionarioMotorista.ValorTotalFrete += comissaoFuncionarioMotoristaDocumento.ValorTotalFrete;

                        decimal percentualExecutacaoOutrasComissoes = 0m;
                        if (comissaoFuncionarioMotoristaDocumento.Carga != null)
                            percentualExecutacaoOutrasComissoes = repComissaoFuncionarioMotoristaDocumento.TotalExecucaoPorCarga(comissaoFuncionarioMotoristaDocumento.Carga.Codigo, comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.Codigo);
                        else if (comissaoFuncionarioMotoristaDocumento.CargaOcorrencia != null)
                            percentualExecutacaoOutrasComissoes = repComissaoFuncionarioMotoristaDocumento.TotalExecucaoPorOcorrencia(comissaoFuncionarioMotoristaDocumento.CargaOcorrencia.Codigo, comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.Codigo);

                        if ((percentualExecutacaoOutrasComissoes) >= 100)
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "O percentual de execução (entre todas as cargas/ocorrências lançadas nas comissões) não pode ser superior a 100%.");
                        }
                        comissaoFuncionarioMotoristaDocumento.PercentualExecucao = 100 - percentualExecutacaoOutrasComissoes;

                        if (comissaoFuncionarioMotoristaDocumento.PercentualExecucao <= 0)
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "O percentual de execução (entre todas as cargas/ocorrências lançadas nas comissões) não pode ser superior a 100%.");
                        }

                        repComissaoFuncionarioMotoristaDocumento.Inserir(comissaoFuncionarioMotoristaDocumento);

                        if (comissaoFuncionarioMotoristaDocumento.PercentualExecucao < 100)
                        {
                            decimal valoFreteLiquido = (comissaoFuncionarioMotoristaDocumento.ValoFreteLiquidoOriginal * comissaoFuncionarioMotoristaDocumento.PercentualExecucao) / 100;

                            if (!AtualizarValoresComissao(comissaoFuncionarioMotoristaDocumento, valoFreteLiquido, unitOfWork, out string msgRetorno))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, msgRetorno);
                            }
                        }

                        comissaoFuncionarioMotorista.TabelaProdutividadeValores = RetornarTabelaProdutividadeValores(comissaoFuncionarioMotorista, unitOfWork);
                        //serComissaoFuncionario.AjustarValoresComissaoMotorista(comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista, (comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido), (comissaoFuncionarioMotoristaDocumento.OutrosValores), (comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo), (comissaoFuncionarioMotoristaDocumento.ValorComissao), unitOfWork, 0m);

                        repComissaoFuncionarioMotorista.Atualizar(comissaoFuncionarioMotorista);
                    }
                    else
                    {
                        Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repCargaOcorrencia.BuscarPorCodigo(codigoOcorrencia);

                        if (ocorrencia == null)
                            return new JsonpResult(false, true, "Ocorrência nao localizada.");

                        decimal percentualComissaoParcial = 100m;

                        if (ocorrencia?.Carga?.TipoOperacao?.GerarComissaoParcialMotorista ?? false)
                            percentualComissaoParcial = ocorrencia.Carga.TipoOperacao.PercentualComissaoParcialMotorista;

                        Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento comissaoFuncionarioMotoristaDocumento = new Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento();
                        comissaoFuncionarioMotoristaDocumento.CargaOcorrencia = ocorrencia;
                        comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista = comissaoFuncionarioMotorista;
                        comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados = ocorrencia.Carga?.DadosSumarizados;
                        comissaoFuncionarioMotoristaDocumento.Numero = ocorrencia.NumeroOcorrencia.ToString();
                        comissaoFuncionarioMotoristaDocumento.DataEmissao = ocorrencia.DataOcorrencia;
                        comissaoFuncionarioMotoristaDocumento.Veiculo = ocorrencia.Carga?.Veiculo;
                        comissaoFuncionarioMotoristaDocumento.VeiculosVinculados = ocorrencia.Carga?.VeiculosVinculados.ToList();
                        comissaoFuncionarioMotoristaDocumento.TipoDocumentoComissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoComissao.ocorrencia;
                        comissaoFuncionarioMotoristaDocumento.PercentualComissaoParcial = percentualComissaoParcial;

                        comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo = Math.Round(((ocorrencia.ValorOcorrenciaLiquida * (percentualComissaoParcial / 100)) * (comissaoFuncionarioMotorista.ComissaoFuncionario.PercentualBaseCalculoComissao / 100)), 2, MidpointRounding.AwayFromZero);
                        comissaoFuncionarioMotoristaDocumento.ValoBaseCalculoOriginal = comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo;
                        comissaoFuncionarioMotorista.ValoBaseCalculo += comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo;

                        comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido = ocorrencia.ValorOcorrenciaLiquida;
                        comissaoFuncionarioMotoristaDocumento.ValoFreteLiquidoOriginal = comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido;
                        comissaoFuncionarioMotorista.ValoFreteLiquido += comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido;

                        comissaoFuncionarioMotoristaDocumento.ValorComissao = Math.Round(comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo * (comissaoFuncionarioMotorista.ComissaoFuncionario.PercentualComissao / 100), 2, MidpointRounding.AwayFromZero);
                        comissaoFuncionarioMotoristaDocumento.ValorComissaoOriginal = comissaoFuncionarioMotoristaDocumento.ValorComissao;
                        comissaoFuncionarioMotoristaDocumento.PercentualExecucao = 100;
                        comissaoFuncionarioMotorista.ValorComissao += comissaoFuncionarioMotoristaDocumento.ValorComissao;

                        comissaoFuncionarioMotoristaDocumento.ValorICMS = repCargaCTeComplementoInfo.BuscarTotalICMSPorOcorrencia(ocorrencia.Codigo);
                        comissaoFuncionarioMotorista.ValorICMS += comissaoFuncionarioMotoristaDocumento.ValorICMS;

                        comissaoFuncionarioMotoristaDocumento.ValorTotalFrete = ocorrencia.ValorOcorrencia + comissaoFuncionarioMotoristaDocumento.ValorICMS;
                        comissaoFuncionarioMotoristaDocumento.ValorTotalFreteOriginal = comissaoFuncionarioMotoristaDocumento.ValorTotalFrete;
                        comissaoFuncionarioMotorista.ValorTotalFrete += comissaoFuncionarioMotoristaDocumento.ValorTotalFrete;

                        decimal percentualExecutacaoOutrasComissoes = 0m;
                        if (comissaoFuncionarioMotoristaDocumento.Carga != null)
                            percentualExecutacaoOutrasComissoes = repComissaoFuncionarioMotoristaDocumento.TotalExecucaoPorCarga(comissaoFuncionarioMotoristaDocumento.Carga.Codigo, comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.Codigo);
                        else if (comissaoFuncionarioMotoristaDocumento.CargaOcorrencia != null)
                            percentualExecutacaoOutrasComissoes = repComissaoFuncionarioMotoristaDocumento.TotalExecucaoPorOcorrencia(comissaoFuncionarioMotoristaDocumento.CargaOcorrencia.Codigo, comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.Codigo);

                        if ((percentualExecutacaoOutrasComissoes) >= 100)
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "O percentual de execução (entre todas as cargas/ocorrências lançadas nas comissões) não pode ser superior a 100%.");
                        }
                        comissaoFuncionarioMotoristaDocumento.PercentualExecucao = 100 - percentualExecutacaoOutrasComissoes;
                        if (comissaoFuncionarioMotoristaDocumento.PercentualExecucao <= 0)
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "O percentual de execução (entre todas as cargas/ocorrências lançadas nas comissões) não pode ser superior a 100%.");
                        }

                        repComissaoFuncionarioMotoristaDocumento.Inserir(comissaoFuncionarioMotoristaDocumento);

                        if (comissaoFuncionarioMotoristaDocumento.PercentualExecucao < 100)
                        {
                            decimal valoFreteLiquido = (comissaoFuncionarioMotoristaDocumento.ValoFreteLiquidoOriginal * comissaoFuncionarioMotoristaDocumento.PercentualExecucao) / 100;

                            if (!AtualizarValoresComissao(comissaoFuncionarioMotoristaDocumento, valoFreteLiquido, unitOfWork, out string msgRetorno))
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, msgRetorno);
                            }
                        }

                        comissaoFuncionarioMotorista.TabelaProdutividadeValores = RetornarTabelaProdutividadeValores(comissaoFuncionarioMotorista, unitOfWork);
                        //serComissaoFuncionario.AjustarValoresComissaoMotorista(comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista, (comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido), (comissaoFuncionarioMotoristaDocumento.OutrosValores), (comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo), (comissaoFuncionarioMotoristaDocumento.ValorComissao), unitOfWork, 0m);

                        repComissaoFuncionarioMotorista.Atualizar(comissaoFuncionarioMotorista);
                    }

                    unitOfWork.CommitChanges();
                    return new JsonpResult(true, true, "Sucesso");
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "A atual situação da comissão (" + (comissaoFuncionarioMotorista?.ComissaoFuncionario.DescricaoSituacaoComissaoFuncionario ?? "Não gerada") + ") não permite que ela seja alterada ");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar carga/ocorrência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverComissaoMotoristaDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento repComissaoFuncionarioMotoristaDocumento = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento(unitOfWork);

            try
            {
                unitOfWork.Start();

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Servicos.Embarcador.RH.ComissaoFuncionario serComissaoFuncionario = new Servicos.Embarcador.RH.ComissaoFuncionario(unitOfWork);
                Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento comissaoFuncionarioMotoristaDocumento = repComissaoFuncionarioMotoristaDocumento.BuscarPorCodigo(codigo);

                if (comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Cancelada
                   && comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Finalizada)
                {
                    serComissaoFuncionario.AjustarValoresComissaoMotorista(comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista, (comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido * -1), (comissaoFuncionarioMotoristaDocumento.OutrosValores * -1), (comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo * -1), (comissaoFuncionarioMotoristaDocumento.ValorComissao * -1), unitOfWork, (comissaoFuncionarioMotoristaDocumento.ValorTotalFrete * -1));
                    repComissaoFuncionarioMotoristaDocumento.Deletar(comissaoFuncionarioMotoristaDocumento);

                    unitOfWork.CommitChanges();
                    return new JsonpResult(true, true, "Sucesso");
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "A atual situação da comissão (" + comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.ComissaoFuncionario.DescricaoSituacaoComissaoFuncionario + ") não permite que ela seja alterada ");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover carga/ocorrência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDadosComissaoMotoristaDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento repComissaoFuncionarioMotoristaDocumento = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento(unitOfWork);

            try
            {
                unitOfWork.Start();

                int codigo = 0;
                decimal valoFreteLiquido = 0, percentualExecucao = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                decimal.TryParse(Request.Params("ValoFreteLiquido"), out valoFreteLiquido);
                decimal.TryParse(Request.Params("PercentualExecucao"), out percentualExecucao);

                Servicos.Embarcador.RH.ComissaoFuncionario serComissaoFuncionario = new Servicos.Embarcador.RH.ComissaoFuncionario(unitOfWork);
                Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento comissaoFuncionarioMotoristaDocumento = repComissaoFuncionarioMotoristaDocumento.BuscarPorCodigo(codigo);

                if (comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Cancelada
                   && comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.ComissaoFuncionario.SituacaoComissaoFuncionario != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoComissaoFuncionario.Finalizada)
                {
                    decimal percentualAnterior = comissaoFuncionarioMotoristaDocumento.PercentualExecucao;
                    if (percentualExecucao > 0 && percentualExecucao != percentualAnterior)
                    {
                        if (percentualExecucao > 100)
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "O percentual de execução não pode ser superior a 100%");
                        }

                        decimal percentualExecutacaoOutrasComissoes = 0m;
                        if (comissaoFuncionarioMotoristaDocumento.Carga != null)
                            percentualExecutacaoOutrasComissoes = repComissaoFuncionarioMotoristaDocumento.TotalExecucaoPorCarga(comissaoFuncionarioMotoristaDocumento.Carga.Codigo, comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.Codigo);
                        else if (comissaoFuncionarioMotoristaDocumento.CargaOcorrencia != null)
                            percentualExecutacaoOutrasComissoes = repComissaoFuncionarioMotoristaDocumento.TotalExecucaoPorOcorrencia(comissaoFuncionarioMotoristaDocumento.CargaOcorrencia.Codigo, comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.Codigo);

                        if ((percentualExecutacaoOutrasComissoes + percentualExecucao) > 100)
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "O percentual de execução (entre todas as cargas/ocorrências lançadas nas comissões) não pode ser superior a 100%. Soma atual: " + (percentualExecutacaoOutrasComissoes + percentualExecucao).ToString("n2"));
                        }

                        if (comissaoFuncionarioMotoristaDocumento.ValorTotalFreteOriginal == 0)
                            comissaoFuncionarioMotoristaDocumento.ValorTotalFreteOriginal = comissaoFuncionarioMotoristaDocumento.ValorTotalFrete;
                        if (comissaoFuncionarioMotoristaDocumento.ValoFreteLiquidoOriginal == 0)
                            comissaoFuncionarioMotoristaDocumento.ValoFreteLiquidoOriginal = comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido;
                        if (comissaoFuncionarioMotoristaDocumento.ValoBaseCalculoOriginal == 0)
                            comissaoFuncionarioMotoristaDocumento.ValoBaseCalculoOriginal = comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo;
                        if (comissaoFuncionarioMotoristaDocumento.ValorComissaoOriginal == 0)
                            comissaoFuncionarioMotoristaDocumento.ValorComissaoOriginal = comissaoFuncionarioMotoristaDocumento.ValorComissao;

                        comissaoFuncionarioMotoristaDocumento.PercentualExecucao = percentualExecucao;
                        valoFreteLiquido = (comissaoFuncionarioMotoristaDocumento.ValoFreteLiquidoOriginal * percentualExecucao) / 100;

                        if (!AtualizarValoresComissao(comissaoFuncionarioMotoristaDocumento, valoFreteLiquido, unitOfWork, out string msgRetorno))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, msgRetorno);
                        }
                        else
                        {
                            var dyncomissaoFuncionarioMotoristaDocumento = retornarComissaoFuncionarioDocumentosDadosGrid(comissaoFuncionarioMotoristaDocumento);
                            var dynComissaoFuncionarioMotorista = serComissaoFuncionario.RetornarComissaoFuncionarioDadosGrid(comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista);

                            var retorno = new
                            {
                                dyncomissaoFuncionarioMotoristaDocumento,
                                dynComissaoFuncionarioMotorista
                            };

                            unitOfWork.CommitChanges();
                            return new JsonpResult(retorno);
                        }

                    }
                    else
                    {
                        if (!AtualizarValoresComissao(comissaoFuncionarioMotoristaDocumento, valoFreteLiquido, unitOfWork, out string msgRetorno))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, msgRetorno);
                        }
                        else
                        {
                            var dyncomissaoFuncionarioMotoristaDocumento = retornarComissaoFuncionarioDocumentosDadosGrid(comissaoFuncionarioMotoristaDocumento);
                            var dynComissaoFuncionarioMotorista = serComissaoFuncionario.RetornarComissaoFuncionarioDadosGrid(comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista);

                            var retorno = new
                            {
                                dyncomissaoFuncionarioMotoristaDocumento,
                                dynComissaoFuncionarioMotorista
                            };

                            unitOfWork.CommitChanges();
                            return new JsonpResult(retorno);
                        }
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "A atual situação da comissão (" + comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.ComissaoFuncionario.DescricaoSituacaoComissaoFuncionario + ") não permite que ela seja alterada ");
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar os dias em viagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.Entidades.Embarcador.RH.TabelaProdutividadeValores RetornarTabelaProdutividadeValores(Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotorista comissaoFuncionarioMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.RH.TabelaProdutividadeValores repValores = new Repositorio.Embarcador.RH.TabelaProdutividadeValores(unitOfWork);
            return repValores.BuscarPorValor(comissaoFuncionarioMotorista.ValoFreteLiquido);
        }

        private dynamic retornarComissaoFuncionarioDocumentosDadosGrid(Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento comissaoFuncionarioMotoristaDocumento)
        {
            var retorno = new
            {
                comissaoFuncionarioMotoristaDocumento.Codigo,
                TipoDocumento = comissaoFuncionarioMotoristaDocumento.TipoDocumentoComissao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoComissao.carga ? "Carga" : "Ocorrência",
                Numero = comissaoFuncionarioMotoristaDocumento.Numero,
                //Frota = comissaoFuncionarioMotoristaDocumento.Carga != null ? comissaoFuncionarioMotoristaDocumento.Carga.Veiculo.NumeroFrota : comissaoFuncionarioMotoristaDocumento.CargaOcorrencia.Carga.Veiculo.NumeroFrota.ToString(),
                Remetente = comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados != null ? comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados.Remetentes : "",
                Destinatario = comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados != null ? comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados.Destinatarios : "",
                UFOrigem = comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados != null ? comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados.UFOrigens : "",
                UFDestino = comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados != null ? comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados.UFDestinos : "",
                PercentualExecucao = comissaoFuncionarioMotoristaDocumento.PercentualExecucao.ToString("n2"),
                PercentualUtilizado = comissaoFuncionarioMotoristaDocumento.PercentualUtilizado.ToString("n2"),
                OutrosValores = comissaoFuncionarioMotoristaDocumento.OutrosValores.ToString("n2"),
                ValorTotalFrete = comissaoFuncionarioMotoristaDocumento.ValorTotalFrete.ToString("n2"),
                ValoBaseCalculo = comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo.ToString("n2"),
                ValoFreteLiquido = comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido.ToString("n2"),
                ValorICMS = comissaoFuncionarioMotoristaDocumento.ValorICMS.ToString("n2"),
                ValorPedagio = comissaoFuncionarioMotoristaDocumento.ValorPedagio.ToString("n2"),
                ValorComissao = comissaoFuncionarioMotoristaDocumento.ValorComissao.ToString("n2"),
                DataDeCarregamento = comissaoFuncionarioMotoristaDocumento.Carga != null ? comissaoFuncionarioMotoristaDocumento.Carga.DataCarregamentoCarga != null ? comissaoFuncionarioMotoristaDocumento.Carga.DataCarregamentoCarga?.ToString("dd/MM/yyy") : "" : "",
                Tomador = comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados != null ? comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados.Tomadores : "",
                Origem = comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados != null ? comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados.Origens : "",
                Destino = comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados != null ? comissaoFuncionarioMotoristaDocumento.CargaDadosSumarizados.Destinos : "",
                Placa = comissaoFuncionarioMotoristaDocumento.Veiculo != null ? comissaoFuncionarioMotoristaDocumento.Veiculo.Placa : "",
                Motorista = comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista != null ? comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.Motorista != null ? comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.Motorista.Nome : "" : "",
                DT_Enable = comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.GerarComissao

            };
            return retorno;
        }

        private bool AtualizarValoresComissao(Dominio.Entidades.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento comissaoFuncionarioMotoristaDocumento, decimal valoFreteLiquido, Repositorio.UnitOfWork unitOfWork, out string msgRetorno)
        {
            Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento repComissaoFuncionarioMotoristaDocumento = new Repositorio.Embarcador.RH.ComissaoFuncionarioMotoristaDocumento(unitOfWork);
            Servicos.Embarcador.RH.ComissaoFuncionario serComissaoFuncionario = new Servicos.Embarcador.RH.ComissaoFuncionario(unitOfWork);
            msgRetorno = string.Empty;
            decimal diferencaFreteLiquido = valoFreteLiquido - comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido;
            decimal novoOutrosValor = comissaoFuncionarioMotoristaDocumento.OutrosValores - diferencaFreteLiquido;

            //if (novoOutrosValor >= 0)
            //{
            //if (novoOutrosValor <= valoFreteLiquido)
            //{
            if (comissaoFuncionarioMotoristaDocumento.ValorTotalFreteOriginal == 0)
                comissaoFuncionarioMotoristaDocumento.ValorTotalFreteOriginal = comissaoFuncionarioMotoristaDocumento.ValorTotalFrete;
            if (comissaoFuncionarioMotoristaDocumento.ValoFreteLiquidoOriginal == 0)
                comissaoFuncionarioMotoristaDocumento.ValoFreteLiquidoOriginal = comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido;
            if (comissaoFuncionarioMotoristaDocumento.ValoBaseCalculoOriginal == 0)
                comissaoFuncionarioMotoristaDocumento.ValoBaseCalculoOriginal = comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo;
            if (comissaoFuncionarioMotoristaDocumento.ValorComissaoOriginal == 0)
                comissaoFuncionarioMotoristaDocumento.ValorComissaoOriginal = comissaoFuncionarioMotoristaDocumento.ValorComissao;

            comissaoFuncionarioMotoristaDocumento.PercentualExecucao = (valoFreteLiquido * 100) / comissaoFuncionarioMotoristaDocumento.ValoFreteLiquidoOriginal;
            comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido = valoFreteLiquido;

            decimal diferencaOutroValor = novoOutrosValor - comissaoFuncionarioMotoristaDocumento.OutrosValores;
            comissaoFuncionarioMotoristaDocumento.OutrosValores = novoOutrosValor;

            decimal novaBC = Math.Round(((comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido * (comissaoFuncionarioMotoristaDocumento.PercentualComissaoParcial / 100)) * (comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.ComissaoFuncionario.PercentualBaseCalculoComissao / 100)), 2, MidpointRounding.AwayFromZero);
            decimal diferencaoBC = novaBC - comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo;
            comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo = novaBC;
            decimal novaComissao = Math.Round(comissaoFuncionarioMotoristaDocumento.ValoBaseCalculo * (comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.PercentualComissao / 100), 2, MidpointRounding.AwayFromZero);
            decimal diferencaoComissao = novaComissao - comissaoFuncionarioMotoristaDocumento.ValorComissao;
            comissaoFuncionarioMotoristaDocumento.ValorComissao = novaComissao;
            serComissaoFuncionario.AjustarValoresComissaoMotorista(comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista, diferencaFreteLiquido, diferencaOutroValor, diferencaoBC, diferencaoComissao, unitOfWork, 0m);
            repComissaoFuncionarioMotoristaDocumento.Atualizar(comissaoFuncionarioMotoristaDocumento);

            Servicos.Auditoria.Auditoria.Auditar(Auditado, comissaoFuncionarioMotoristaDocumento.ComissaoFuncionarioMotorista.ComissaoFuncionario, null, "Alterou Dados Documento " + comissaoFuncionarioMotoristaDocumento.Descricao + ".", unitOfWork);

            return true;
            //}
            //else
            //{
            //    msgRetorno = "O valor líquido do frete não pode ser menor que " + (comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido - comissaoFuncionarioMotoristaDocumento.OutrosValores).ToString("n2");
            //    return false;
            //}
            //}
            //else
            //{
            //    msgRetorno = "O valor líquido do frete não pode ser maior que " + (comissaoFuncionarioMotoristaDocumento.OutrosValores + comissaoFuncionarioMotoristaDocumento.ValoFreteLiquido).ToString("n2");
            //    return false;
            //}
        }

        private Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaDocumenotosComissaoFuncionario ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaDocumenotosComissaoFuncionario()
            {
                codigoComissaoMotorista = Request.GetIntParam("ComissaoFuncionarioMotorista"),
            };
        }
    }


}
