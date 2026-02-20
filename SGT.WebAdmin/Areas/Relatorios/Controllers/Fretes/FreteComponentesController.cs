using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Fretes
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Fretes/FreteComponentes")]
    public class FreteComponentesController : BaseController
    {
		#region Construtores

		public FreteComponentesController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R003_FretePorDestinatario;
        private int UltimaColunaDinanica = 1;
        private int numeroMaximoComplementos = 15;

        private decimal tamanhoColunasValores = (decimal)1.75;
        private decimal tamanhoColunasParticipantes = (decimal)5.50;
        private decimal tamanhoColunasEnderecoParticipantes = 3;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Componentes de Frete", "Fretes", "FreteComponente.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Destinatario", "asc", "", "", Codigo, unitOfWork, true, true);
                Models.Grid.Grid grid = await GridPadrao(unitOfWork, cancellationToken);

                if (!mdlRelatorio.ConferirColunasDinamicas(ref relatorio, ref grid, ref UltimaColunaDinanica, numeroMaximoComplementos, tamanhoColunasValores, "ValorComponente"))
                {
                    await unitOfWork.RollbackAsync(cancellationToken);
                    return new JsonpResult(false, true, "Não é possível carregar esse relatório pois existem componentes que foram inativados, por favor crie um novo padrão.");
                }

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(grid, relatorio);
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                double remetente = double.Parse(Request.Params("Remetente"));
                double destinatario = double.Parse(Request.Params("Destinatario"));
                double tomador = double.Parse(Request.Params("Tomador"));
                double expedidor = double.Parse(Request.Params("Expedidor"));
                double recebedor = double.Parse(Request.Params("Recebedor"));
                int transportador = int.Parse(Request.Params("Transportador"));
                int filial = int.Parse(Request.Params("Filial"));
                DateTime dataInicio, dataFim;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);
                List<int> codigosFilial = filial == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { filial };
                List<double> codigosRecebedor = recebedor == 0 ? ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork) : new List<double> { recebedor };
                List<int> codigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);
                List<int> codigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);

                var propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, propAgrupa);
                propOrdena = ObterPropriedadeOrdenar(propOrdena);

                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteComponentes> listaParticipante = await repCargaPedido.BuscarRelatorioFreteComponenteAsync(agrupamentos, dataInicio, dataFim, remetente, destinatario, tomador, expedidor, codigosRecebedor, codigosFilial, transportador, codigosTipoCarga, codigosTipoOperacao, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(listaParticipante.Count);
                grid.AdicionaRows(listaParticipante);

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

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                double remetente = double.Parse(Request.Params("Remetente"));
                double destinatario = double.Parse(Request.Params("Destinatario"));
                double tomador = double.Parse(Request.Params("Tomador"));
                double expedidor = double.Parse(Request.Params("Expedidor"));
                double recebedor = double.Parse(Request.Params("Recebedor"));
                int transportador = int.Parse(Request.Params("Transportador"));
                int filial = int.Parse(Request.Params("Filial"));
                DateTime dataInicio, dataFim;
                DateTime.TryParseExact(Request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                DateTime.TryParseExact(Request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);
                List<int> codigosFilial = filial == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { filial };
                List<double> codigosRecebedor = recebedor == 0 ? ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork) : new List<double>() { recebedor };
                List<int> codigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);
                List<int> codigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);
                relatorioTemp.PropriedadeOrdena = ObterPropriedadeOrdenar(propOrdena);

                string stringConexao = _conexao.StringConexao;
                _ = Task.Factory.StartNew(() => GerarRelatorioFretesComponentes(agrupamentos, dataInicio, dataFim, remetente, destinatario, tomador, expedidor, codigosRecebedor, codigosFilial, transportador, codigosTipoCarga, codigosTipoOperacao, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorioFretesComponentes(
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos,
            DateTime dataInicial, DateTime dataFinal,
            double remetente,
            double destinatario,
            double tomador,
            double expedidor,
            List<double> codigosRecebedor,
            List<int> codigosFilial,
            int tranportador,
            List<int> codigosTipoCarga,
            List<int> codigosTipoOperacao,
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp,
            string stringConexao,
            CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            
            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);

                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteComponentes> listaParticipante = await repCargaPedido.BuscarRelatorioFreteComponenteAsync(agrupamentos, dataInicial, dataFinal, remetente, destinatario, tomador, expedidor, codigosRecebedor, codigosFilial, tranportador, codigosTipoCarga, codigosTipoOperacao, relatorioControleGeracao.Relatorio.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, false);

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                if (dataInicial != DateTime.MinValue || dataFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataInicial != DateTime.MinValue ? dataInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataFinal != DateTime.MinValue ? "até " + dataFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Periodo", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Periodo", false));

                if (tranportador > 0)
                {
                    Dominio.Entidades.Empresa empresa = await repEmpresa.BuscarPorCodigoAsync(tranportador);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", "(" + empresa.CNPJ_Formatado + ") " + empresa.RazaoSocial, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));


                if (remetente > 0)
                {
                    Dominio.Entidades.Cliente cliente = await repCliente.BuscarPorCPFCNPJAsync(remetente);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", "(" + cliente.CPF_CNPJ_Formatado + ") " + cliente.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", false));

                if (destinatario > 0)
                {
                    Dominio.Entidades.Cliente cliente = await repCliente.BuscarPorCPFCNPJAsync(destinatario);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", "(" + cliente.CPF_CNPJ_Formatado + ") " + cliente.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", false));

                if (tomador > 0)
                {
                    Dominio.Entidades.Cliente cliente = await repCliente.BuscarPorCPFCNPJAsync(tomador);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", "(" + cliente.CPF_CNPJ_Formatado + ") " + cliente.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", false));


                if (codigosRecebedor?.Count > 0)
                {
                    if (codigosRecebedor.Count == 1)
                    {
                        var lst = await repCliente.BuscarPorCPFCNPJsAsync(codigosRecebedor);
                        Dominio.Entidades.Cliente recebedor = lst.FirstOrDefault();
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Recebedor", "(" + recebedor.CPF_CNPJ_Formatado + ") " + recebedor.Descricao, true));
                    }
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Recebedor", "Múltiplos registros selecionados", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Recebedor", false));

                if (expedidor > 0)
                {
                    Dominio.Entidades.Cliente cliente = await repCliente.BuscarPorCPFCNPJAsync(expedidor);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Expedidor", "(" + cliente.CPF_CNPJ_Formatado + ") " + cliente.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Expedidor", false));


                if (codigosFilial?.Count > 0)
                {
                    if (codigosFilial.Count == 1)
                    {
                        Dominio.Entidades.Embarcador.Filiais.Filial filialEntidade = await repFilial.BuscarPorCodigoAsync(codigosFilial.FirstOrDefault());
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", "(" + filialEntidade.CNPJ_Formatado + ") " + filialEntidade.Descricao, true));
                    }
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", "Múltiplos registros selecionados", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", false));


                if (!string.IsNullOrWhiteSpace(relatorioTemp.PropriedadeAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", relatorioControleGeracao.Relatorio.PropriedadeAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

                serRelatorio.GerarRelatorioDinamico( "Relatorios/Fretes/FreteComponentes", parametros,relatorioControleGeracao, relatorioTemp, listaParticipante, unitOfWork);
            }
            catch (Exception ex)
            {
                await serRelatorio.RegistrarFalhaGeracaoRelatorioAsync(relatorioControleGeracao, unitOfWork, ex, cancellationToken);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private async Task<Models.Grid.Grid> GridPadrao(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            UltimaColunaDinanica = 1;

            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork, cancellationToken);
            List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentes = await repComponenteFrete.BuscarTodosAtivosAsync();

            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Remetente", "Remetente", tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Loc. Remetente", "RemetenteLocalidade", tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Destinatário", "Destinatario", tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Loc. Destinatário", "DestinatarioLocalidade", tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Tomador", "Tomador", tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Loc. Tomador", "TomadorLocalidade", tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Expedidor", "Expedidor", tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Loc. Expedidor", "ExpedidorLocalidade", tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Recebedor", "Recebedor", tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Loc. Recebedor", "RecebedorLocalidade", tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Transportador", "Transportador", tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Loc. Transportador", "TransportadorLocalidade", tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Filial", "Filial", tamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Pagamento", "DescricaoTipoPagamento", tamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Frota", "Frota", tamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Valor de Frete", "ValorFrete", tamanhoColunasValores, Models.Grid.Align.right, true, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("ICMS", "ValorICMS", tamanhoColunasValores, Models.Grid.Align.right, true, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Pago por ST", "DescricaoPagoPorST", tamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Descarga", "ValorDescarga", tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Pedágio", "ValorPedagio", tamanhoColunasValores, Models.Grid.Align.right, true, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Ad. Valorem", "ValorAdValorem", tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Outros Valores", "ValorOutros", tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Complemento de Frete", "ValorComplementoFrete", tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Complemento de ICMS", "ValorComplementoICMS", tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Total", "ValorAReceber", tamanhoColunasValores, Models.Grid.Align.right, true, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);

            //Colunas montadas dinamicamente
            for (int i = 0; i < componentes.Count; i++)
            {
                if (i < numeroMaximoComplementos)
                {
                    grid.AdicionarCabecalho(componentes[i].Descricao, "ValorComponente" + UltimaColunaDinanica.ToString(), tamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum, componentes[i].Codigo);
                    UltimaColunaDinanica++;
                }
                else
                    break;
            }
            return grid;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoPagoPorST")
                return "ICMSPagoPorST";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
