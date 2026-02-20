using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Minutas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Minutas/Minuta")]
    public class MinutaController : BaseController
    {
		#region Construtores

		public MinutaController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R008_Minutas;

        private decimal TamanhoColunasValores = 2.25m;
        private decimal TamanhoColunasTextoMenores = 3.50m;
        private decimal TamanhoColunasTexto = 5.50m;

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", TamanhoColunasTexto, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", TamanhoColunasTexto, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", TamanhoColunasTexto, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Origem", "LocalidadeOrigem", TamanhoColunasTexto, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("UF Origem", "EstadoOrigem", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Destino", "LocalidadeDestino", TamanhoColunasTexto, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UF Destino", "EstadoDestino", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", TamanhoColunasTexto, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Veículo", "Veiculo", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Propriedade do Veículo", "PropriedadeVeiculo", TamanhoColunasTextoMenores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Proprietário do Veículo", "ProprietarioVeiculo", TamanhoColunasTexto, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Situação", "DescricaoStatus", TamanhoColunasTextoMenores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Situação Integração", "SituacaoIntegracao", TamanhoColunasTextoMenores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Sit. Int. Fatura", "SituacaoIntegracaoFatura", TamanhoColunasTextoMenores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", TamanhoColunasTexto, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data de Emissão", "DataEmissao", TamanhoColunasTextoMenores, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Minuta", "NumeroMinuta", TamanhoColunasTextoMenores, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Nº Pedido Embarcador", "NumeroPedidoEmbarcador", TamanhoColunasTextoMenores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Carga", "Carga", TamanhoColunasTextoMenores, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Fatura", "NumeroFatura", TamanhoColunasTextoMenores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Número CT-es", "NumeroCTes", TamanhoColunasTextoMenores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Peso", "Peso", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Qtd. CT-es", "QuantidadeCTes", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Qtd. CT-es Integrados", "QuantidadeCTesIntegrados", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Alíquota ICMS", "AliquotaICMS", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Valor Frete", "ValorFrete", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Serviço", "ValorServico", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Receber", "ValorReceber", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor ICMS", "ValorICMS", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Minuta", "ValorMinuta", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio;
                int.TryParse(Request.Params("Codigo"), out codigoRelatorio);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Minutas", "Minutas", "Minuta.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Remetente", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(unitOfWork), relatorio);

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
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                DateTime dataInicialMinuta, dataFinalMinuta;
                DateTime.TryParseExact(Request.Params("DataInicialMinuta"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicialMinuta);
                DateTime.TryParseExact(Request.Params("DataFinalMinuta"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinalMinuta);

                int codigoMotorista = 0, codigoFatura = 0;
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("Fatura"), out codigoFatura);

                string estadoOrigem = Request.Params("EstadoOrigem");
                string estadoDestino = Request.Params("EstadoDestino");
                string tipoPropriedadeVeiculo = Request.Params("TipoPropriedadeVeiculo");

                bool? situacaoIntegracao = null;
                bool situacaoIntegracaoAux;
                if (bool.TryParse(Request.Params("SituacaoIntegracao"), out situacaoIntegracaoAux))
                    situacaoIntegracao = situacaoIntegracaoAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta? tipoIntegradora = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta tipoIntegradoraAux;
                if (Enum.TryParse(Request.Params("TipoIntegradora"), out tipoIntegradoraAux))
                    tipoIntegradora = tipoIntegradoraAux;

                Repositorio.ManifestoAvon repManifesto = new Repositorio.ManifestoAvon(unidadeDeTrabalho);

                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                IList<Dominio.Relatorios.Embarcador.DataSource.Minutas.Minuta> listaMinutas = repManifesto.ConsultarRelatorioMinutas(codigoFatura, agrupamentos, dataInicialMinuta, dataFinalMinuta, codigoMotorista, estadoOrigem, estadoDestino, tipoIntegradora, this.Empresa.TipoAmbiente, tipoPropriedadeVeiculo, situacaoIntegracao, propAgrupa, grid.group.dirOrdena, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repManifesto.ContarConsultaRelatorioMinutas(codigoFatura, agrupamentos, dataInicialMinuta, dataFinalMinuta, codigoMotorista, estadoOrigem, estadoDestino, tipoIntegradora, this.Empresa.TipoAmbiente, tipoPropriedadeVeiculo, situacaoIntegracao, propAgrupa, grid.group.dirOrdena, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite));

                grid.AdicionaRows(listaMinutas);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime dataInicialMinuta, dataFinalMinuta;
                DateTime.TryParseExact(Request.Params("DataInicialMinuta"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicialMinuta);
                DateTime.TryParseExact(Request.Params("DataFinalMinuta"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinalMinuta);

                int codigoMotorista = 0, codigoFatura = 0;
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("Fatura"), out codigoFatura);

                string estadoOrigem = Request.Params("EstadoOrigem");
                string estadoDestino = Request.Params("EstadoDestino");
                string tipoPropriedadeVeiculo = Request.Params("TipoPropriedadeVeiculo");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta? tipoIntegradora = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta tipoIntegradoraAux;
                if (Enum.TryParse(Request.Params("TipoIntegradora"), out tipoIntegradoraAux))
                    tipoIntegradora = tipoIntegradoraAux;

                bool? situacaoIntegracao = null;
                bool situacaoIntegracaoAux;
                if (bool.TryParse(Request.Params("SituacaoIntegracao"), out situacaoIntegracaoAux))
                    situacaoIntegracao = situacaoIntegracaoAux;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                string stringConexao = _conexao.StringConexao;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = this.Empresa.TipoAmbiente;

                _ = Task.Factory.StartNew(() => GerarRelatorioMinutas(codigoFatura, agrupamentos, dataInicialMinuta, dataFinalMinuta, codigoMotorista, estadoOrigem, estadoDestino, tipoIntegradora, tipoAmbiente, tipoPropriedadeVeiculo, situacaoIntegracao, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioMinutas(int codigoFatura, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, DateTime dataInicialMinuta, DateTime dataFinalMinuta, int codigoMotorista, string estadoOrigem, string estadoDestino, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta? tipoIntegradora, Dominio.Enumeradores.TipoAmbiente ambiente, string tipoPropriedadeVeiculo, bool? situacaoIntegracao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.ManifestoAvon repManifesto = new Repositorio.ManifestoAvon(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Minutas.Minuta> listaMinutas = repManifesto.ConsultarRelatorioMinutas(codigoFatura, propriedades, dataInicialMinuta, dataFinalMinuta, codigoMotorista, estadoOrigem, estadoDestino, tipoIntegradora, ambiente, tipoPropriedadeVeiculo, situacaoIntegracao, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0);


                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                if (dataInicialMinuta != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialMinuta", dataInicialMinuta.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialMinuta", false));

                if (dataFinalMinuta != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalMinuta", dataFinalMinuta.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalMinuta", false));

                if (codigoMotorista > 0)
                {
                    Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(codigoMotorista);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", motorista.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

                if (tipoIntegradora.HasValue)
                {
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoIntegradora", tipoIntegradora == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegradoraMinuta.Avon ? "Avon" : "Natura", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoIntegradora", false));

                if (!string.IsNullOrWhiteSpace(estadoOrigem) && estadoOrigem != "0")
                {
                    Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla(estadoOrigem);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoOrigem", estado.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoOrigem", false));

                if (!string.IsNullOrWhiteSpace(estadoDestino) && estadoDestino != "0")
                {
                    Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla(estadoDestino);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoDestino", estado.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoDestino", false));

                if (!string.IsNullOrWhiteSpace(tipoPropriedadeVeiculo))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PropriedadeVeiculo", (tipoPropriedadeVeiculo == "P" ? "Próprio" : "Terceiro"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PropriedadeVeiculo", false));

                if (codigoFatura > 0)
                {
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fatura", repFatura.BuscarPorCodigo(codigoFatura).Numero.ToString(), true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fatura", false));

                serRelatorio.GerarRelatorioDinamico("Relatorios/Minutas/Minuta",parametros,relatorioControleGeracao, relatorioTemp, listaMinutas, unitOfWork);
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }
    }
}

