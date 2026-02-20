using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	public class PosicaoDocumentoReceberController : BaseController
    {
		#region Construtores

		public PosicaoDocumentoReceberController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R071_PosicaoDocumentoReceber;

        private decimal TamanhoColumaExtraPequena = 1m;
        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaGrande = 5.50m;
        private decimal TamanhoColunaMedia = 3m;

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Série", "Serie", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Autorização", "DataAutorizacao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Cancelamento", "DataCancelamento", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Anulação", "DataAnulacao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Empresa/Filial", "Empresa", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Tomador", "Tomador", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoasTomador", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Origem", "Origem", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Destino", "Destino", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Veículo", "Veiculo", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Vl. Frete", "ValorLiquidoDocumento", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Vl. ICMS", "ValorICMS", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Vl. a Receber", "ValorBrutoDocumento", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Vl. a Faturar", "ValorAFaturar", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Vl. em Título", "ValorDocumentoEmTitulo", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Vl. Ac. Geração", "ValorAcrescimoGeracao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Vl. Des. Geração", "ValorDescontoGeracao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Vl. Tot. Título", "ValorTotalTitulo", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Vl. Pend. Pgto", "ValorPendentePagamento", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Vl. Pago", "ValorPago", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Vl. Ac. Baixa", "ValorAcrescimoBaixa", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Vl. Des. Baixa", "ValorDescontoBaixa", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Vl. Baixa Doc.", "ValorBaixadoDocumento", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Vl. Baixa Ac.", "ValorBaixadoAcrescimo", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Posição de Documentos a Receber", "Financeiros", "PosicaoDocumentoReceber.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Numero", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                DateTime.TryParseExact(Request.Params("DataPosicao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataPosicao);

                int.TryParse(Request.Params("GrupoPessoasTomador"), out int codigoGrupoPessoas);
                int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);
                int.TryParse(Request.Params("Origem"), out int codigoOrigem);
                int.TryParse(Request.Params("Destino"), out int codigoDestino);

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Remetente")), out double cpfCnpjRemetente);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out double cpfCnpjDestinatario);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Tomador")), out double cpfCnpjTomador);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFaturamentoPosicao? tipoFaturamento = null;
                if (Enum.TryParse(Request.Params("TipoFaturamento"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFaturamentoPosicao tipoFaturamentoAux))
                    tipoFaturamento = tipoFaturamentoAux;

                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoDocumentoReceber> listaPosicaoDocumentoReceber = repDocumentoFaturamento.ConsultarRelatorioPosicaoDocumentoReceber(agrupamentos, dataPosicao, tipoFaturamento, codigoEmpresa, codigoGrupoPessoas, codigoOrigem, codigoDestino, cpfCnpjRemetente, cpfCnpjDestinatario, cpfCnpjTomador, propAgrupa, grid.group.dirOrdena, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                int countPosicao = repDocumentoFaturamento.ContarConsultaRelatorioPosicaoDocumentoReceber(agrupamentos, dataPosicao, tipoFaturamento, codigoEmpresa, codigoGrupoPessoas, codigoOrigem, codigoDestino, cpfCnpjRemetente, cpfCnpjDestinatario, cpfCnpjTomador, propAgrupa, grid.group.dirOrdena, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena);

                grid.setarQuantidadeTotal(countPosicao);

                grid.AdicionaRows(listaPosicaoDocumentoReceber);

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
                DateTime.TryParseExact(Request.Params("DataPosicao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataPosicao);

                int.TryParse(Request.Params("GrupoPessoasTomador"), out int codigoGrupoPessoas);
                int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);
                int.TryParse(Request.Params("Origem"), out int codigoOrigem);
                int.TryParse(Request.Params("Destino"), out int codigoDestino);

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Remetente")), out double cpfCnpjRemetente);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out double cpfCnpjDestinatario);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Tomador")), out double cpfCnpjTomador);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFaturamentoPosicao? tipoFaturamento = null;
                if (Enum.TryParse(Request.Params("TipoFaturamento"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFaturamentoPosicao tipoFaturamentoAux))
                    tipoFaturamento = tipoFaturamentoAux;

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

                _ = Task.Factory.StartNew(() => GerarRelatorioPosicaoDocumentoReceber(agrupamentos, dataPosicao, tipoFaturamento, codigoEmpresa, codigoGrupoPessoas, codigoOrigem, codigoDestino, cpfCnpjRemetente, cpfCnpjDestinatario, cpfCnpjTomador, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioPosicaoDocumentoReceber(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, DateTime dataPosicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFaturamentoPosicao? tipoFaturamento, int codigoEmpresa, int codigoGrupoPessoas, int codigoOrigem, int codigoDestino, double cpfCnpjRemetente, double cpfCnpjDestinatario, double cpfCnpjTomador, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoDocumentoReceber> listaPosicaoDocumentoReceber = repDocumentoFaturamento.ConsultarRelatorioPosicaoDocumentoReceber(propriedades, dataPosicao, tipoFaturamento, codigoEmpresa, codigoGrupoPessoas, codigoOrigem, codigoDestino, cpfCnpjRemetente, cpfCnpjDestinatario, cpfCnpjTomador, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0);

                Dominio.Entidades.Empresa empresaRelatorio = repEmpresa.BuscarPorCodigo(Empresa.Codigo);


                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPosicao", dataPosicao.ToString("dd/MM/yyyy"), true));

                if (tipoFaturamento.HasValue)
                {
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoFaturamento", tipoFaturamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFaturamentoPosicao.Faturado ? "Faturado" : "Não Faturado", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoFaturamento", false));

                if (codigoEmpresa > 0)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", false));

                if (cpfCnpjTomador > 0d)
                {
                    Dominio.Entidades.Cliente pessoa = repCliente.BuscarPorCPFCNPJ(cpfCnpjTomador);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", pessoa.CPF_CNPJ_Formatado + " - " + pessoa.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", false));

                if (cpfCnpjRemetente > 0d)
                {
                    Dominio.Entidades.Cliente pessoa = repCliente.BuscarPorCPFCNPJ(cpfCnpjRemetente);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", pessoa.CPF_CNPJ_Formatado + " - " + pessoa.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", false));

                if (cpfCnpjDestinatario > 0d)
                {
                    Dominio.Entidades.Cliente pessoa = repCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", pessoa.CPF_CNPJ_Formatado + " - " + pessoa.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", false));

                if (codigoGrupoPessoas > 0)
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoasTomador", grupoPessoas.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoasTomador", false));

                if (codigoOrigem > 0)
                {
                    Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(codigoOrigem);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", localidade.DescricaoCidadeEstado, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", false));

                if (codigoDestino > 0)
                {
                    Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(codigoDestino);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", localidade.DescricaoCidadeEstado, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", false));

                serRelatorio.GerarRelatorioDinamico("Relatorios/Financeiros/PosicaoDocumentoReceber", parametros,relatorioControleGeracao, relatorioTemp, listaPosicaoDocumentoReceber, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
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
