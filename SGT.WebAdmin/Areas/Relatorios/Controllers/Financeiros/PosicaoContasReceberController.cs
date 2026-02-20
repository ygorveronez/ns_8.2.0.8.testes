using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/PosicaoContasReceber")]
    public class PosicaoContasReceberController : BaseController
    {
		#region Construtores

		public PosicaoContasReceberController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R042_PosicaoContasReceber;

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Filial", "Filial", 5, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Número CTe", "NumeroCTe", 8, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Série", "SerieCTe", 8, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Componentes", "ComponentesFrete", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Modelo", "Modelo", 5, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Tomador", "Tomador", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Cidade Tomador", "CidadeTomador", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Grupo Tomador", "DescricaoGrupo", 15, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", 15, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Remetente", "CNPJRemetente", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Destinatario", "Destinatario", 15, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Destinatario", "CNPJDestinatario", 15, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor a Receber do CTe", "ValorReceber", 10, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Frota", "Frotas", 8, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Veículo", "Placas", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Próprio / Terceiro", "ProprioTerceiro", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Motorista", "Motoristas", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Emissão do CTe", "DataEmissaoCTe", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Número das Notas", "Notas", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Origem", "Origem", 10, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("UF Origem", "UFOrigem", 5, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("UF Destino", "UFDestino", 5, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Nº DT/Minuta", "NumeroDTMinuta", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Número Carga", "NumeroCarga", 8, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Emissão Carga", "DataEmissaoCarga", 8, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Número Fatura", "NumeroFatura", 8, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Número Pré Fatura", "NumeroPreFatura", 8, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Grupo Fatura", "GrupoFatura", 15, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Cliente Fatura", "ClienteFatura", 15, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Emissão Fatura", "DataEmissaoFatura", 8, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Vencimento Fatura", "DataVencimentoTitulo", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Valor Título", "ValorTitulo", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor Pendente Título", "ValorPendenteTitulo", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Cliente Título", "ClienteTitulo", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Emissão Título", "DataEmissaoTitulo", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Quitação Título", "DataBaseBaixa", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Data Movimento", "DataMovimento", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Status", "StatusTitulo", 8, Models.Grid.Align.center, false, false);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Posição de Contas a Receber", "Financeiros", "PosicaoContasReceber.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroCTe", "asc", "", "", Codigo, unitOfWork, true, false);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);
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

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber situacao;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo statusCTe;
                Enum.TryParse(Request.Params("Situacao"), out situacao);
                Enum.TryParse(Request.Params("Status"), out status);
                Enum.TryParse(Request.Params("StatusCTe"), out statusCTe);

                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                int codigoCTe = 0, codigoFatura = 0, codigoGrupoPessoa = 0, codigoGrupoCTe = 0;
                int.TryParse(Request.Params("ConhecimentoDeTransporte"), out codigoCTe);
                int.TryParse(Request.Params("Fatura"), out codigoFatura);
                int.TryParse(Request.Params("GrupoPessoa"), out codigoGrupoPessoa);
                int.TryParse(Request.Params("GrupoPessoaCTe"), out codigoGrupoCTe);

                DateTime dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento;
                DateTime.TryParse(Request.Params("DataInicialEmissao"), out dataInicialEmissao);
                DateTime.TryParse(Request.Params("DataFinalEmissao"), out dataFinalEmissao);
                DateTime.TryParse(Request.Params("DataInicialVencimento"), out dataInicialVencimento);
                DateTime.TryParse(Request.Params("DataFinalVencimento"), out dataFinalVencimento);

                DateTime dataInicialQuitacao, dataFinalQuitacao, dataInicialMovimento, dataFinalMovimento;
                DateTime.TryParse(Request.Params("DataInicialQuitacao"), out dataInicialQuitacao);
                DateTime.TryParse(Request.Params("DataFinalQuitacao"), out dataFinalQuitacao);
                DateTime.TryParse(Request.Params("DataInicialMovimento"), out dataInicialMovimento);
                DateTime.TryParse(Request.Params("DataFinalMovimento"), out dataFinalMovimento);

                decimal valorCTeInicial = 0, valorCTeFinal = 0;
                decimal.TryParse(Request.Params("ValorCTeInicial"), out valorCTeInicial);
                decimal.TryParse(Request.Params("ValorCTeFinal"), out valorCTeFinal);

                bool? cteVinculadoACarga = null;
                bool cteVinculadoACargaAux;
                if (bool.TryParse(Request.Params("CTeVinculadoACarga"), out cteVinculadoACargaAux))
                    cteVinculadoACarga = cteVinculadoACargaAux;

                List<int> gruposPessoas = JsonConvert.DeserializeObject<List<int>>(Request.Params("GruposPessoas"));

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                var propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string ordenacao = grid.header[grid.indiceColunaOrdena].data;

                Servicos.Log.TratarErro("INICIO Preview do relatório de Posição de Contas a Receber");
                            Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref ordenacao, string.Empty);

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoContasReceber> listaPosicaoContasReceber = repTitulo.RelatorioPosicaoContasReceber2(agrupamentos, gruposPessoas, valorCTeInicial, valorCTeFinal, dataInicialQuitacao, dataFinalQuitacao, dataInicialMovimento, dataFinalMovimento, statusCTe, codigoGrupoCTe, situacao, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento, cteVinculadoACarga, propAgrupa, grid.group.dirOrdena, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTitulo.ContarPosicaoContasReceber2(agrupamentos, gruposPessoas, valorCTeInicial, valorCTeFinal, dataInicialQuitacao, dataFinalQuitacao, dataInicialMovimento, dataFinalMovimento, statusCTe, codigoGrupoCTe, situacao, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento, cteVinculadoACarga, propAgrupa, grid.group.dirOrdena, ordenacao, grid.dirOrdena, grid.inicio, grid.limite));

                //List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoContasReceber> listaPosicaoContasReceber = repTitulo.RelatorioPosicaoContasReceber(gruposPessoas, valorCTeInicial, valorCTeFinal, dataInicialQuitacao, dataFinalQuitacao, dataInicialMovimento, dataFinalMovimento, statusCTe, codigoGrupoCTe, situacao, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento, propAgrupa, grid.group.dirOrdena, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                //grid.setarQuantidadeTotal(repTitulo.ContarPosicaoContasReceber(gruposPessoas, valorCTeInicial, valorCTeFinal, dataInicialQuitacao, dataFinalQuitacao, dataInicialMovimento, dataFinalMovimento, statusCTe, codigoGrupoCTe, situacao, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento));

                var lista = (from obj in listaPosicaoContasReceber
                             select new
                             {
                                 obj.Filial,
                                 obj.NumeroCTe,
                                 obj.SerieCTe,
                                 obj.ComponentesFrete,
                                 obj.Modelo,
                                 obj.Tomador,
                                 obj.CidadeTomador,
                                 obj.DescricaoGrupo,
                                 obj.Remetente,
                                 obj.CNPJRemetente,
                                 obj.Destinatario,
                                 obj.CNPJDestinatario,
                                 obj.ValorReceber,
                                 obj.Frotas,
                                 obj.Placas,
                                 obj.ProprioTerceiro,
                                 obj.Motoristas,
                                 DataEmissaoCTe = obj.DataEmissaoCTe != null && obj.DataEmissaoCTe > DateTime.MinValue ? obj.DataEmissaoCTe.ToString("dd/MM/yyyy") : string.Empty,
                                 obj.Notas,
                                 obj.Origem,
                                 obj.UFOrigem,
                                 obj.Destino,
                                 obj.UFDestino,
                                 obj.NumeroDTMinuta,
                                 obj.NumeroCarga,
                                 DataEmissaoCarga = obj.DataEmissaoCarga != null && obj.DataEmissaoCarga > DateTime.MinValue ? obj.DataEmissaoCarga.ToString("dd/MM/yyyy") : string.Empty,
                                 obj.NumeroFatura,
                                 obj.NumeroPreFatura,
                                 obj.GrupoFatura,
                                 obj.ClienteFatura,
                                 DataEmissaoFatura = obj.DataEmissaoFatura != null && obj.DataEmissaoFatura > DateTime.MinValue ? obj.DataEmissaoFatura.ToString("dd/MM/yyyy") : string.Empty,
                                 DataVencimentoTitulo = obj.DataVencimentoTitulo != null && obj.DataVencimentoTitulo > DateTime.MinValue ? obj.DataVencimentoTitulo.ToString("dd/MM/yyyy") : string.Empty,
                                 obj.ValorTitulo,
                                 obj.ValorPendenteTitulo,
                                 obj.ClienteTitulo,
                                 DataEmissaoTitulo = obj.DataEmissaoTitulo != null && obj.DataEmissaoTitulo > DateTime.MinValue ? obj.DataEmissaoTitulo.ToString("dd/MM/yyyy") : string.Empty,
                                 DataBaseBaixa = obj.DataBaseBaixa != null && obj.DataBaseBaixa > DateTime.MinValue ? obj.DataBaseBaixa.ToString("dd/MM/yyyy") : string.Empty,
                                 DataMovimento = obj.DataMovimento != null && obj.DataMovimento > DateTime.MinValue ? obj.DataMovimento.ToString("dd/MM/yyyy") : string.Empty,
                                 obj.StatusTitulo
                             }).ToList();

                Servicos.Log.TratarErro("FIM Preview do relatório de Posição de Contas a Receber");
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



        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber situacao;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo statusCTe;
                Enum.TryParse(Request.Params("Situacao"), out situacao);
                Enum.TryParse(Request.Params("Status"), out status);
                Enum.TryParse(Request.Params("StatusCTe"), out statusCTe);

                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                int codigoCTe = 0, codigoFatura = 0, codigoGrupoPessoa = 0, codigoGrupoCTe = 0;
                int.TryParse(Request.Params("ConhecimentoDeTransporte"), out codigoCTe);
                int.TryParse(Request.Params("Fatura"), out codigoFatura);
                int.TryParse(Request.Params("GrupoPessoa"), out codigoGrupoPessoa);
                int.TryParse(Request.Params("GrupoPessoaCTe"), out codigoGrupoCTe);

                DateTime dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento;

                DateTime.TryParse(Request.Params("DataInicialEmissao"), out dataInicialEmissao);
                DateTime.TryParse(Request.Params("DataFinalEmissao"), out dataFinalEmissao);
                DateTime.TryParse(Request.Params("DataInicialVencimento"), out dataInicialVencimento);
                DateTime.TryParse(Request.Params("DataFinalVencimento"), out dataFinalVencimento);

                DateTime dataInicialQuitacao, dataFinalQuitacao, dataInicialMovimento, dataFinalMovimento;
                DateTime.TryParse(Request.Params("DataInicialQuitacao"), out dataInicialQuitacao);
                DateTime.TryParse(Request.Params("DataFinalQuitacao"), out dataFinalQuitacao);
                DateTime.TryParse(Request.Params("DataInicialMovimento"), out dataInicialMovimento);
                DateTime.TryParse(Request.Params("DataFinalMovimento"), out dataFinalMovimento);

                decimal valorCTeInicial = 0, valorCTeFinal = 0;
                decimal.TryParse(Request.Params("ValorCTeInicial"), out valorCTeInicial);
                decimal.TryParse(Request.Params("ValorCTeFinal"), out valorCTeFinal);

                bool? cteVinculadoACarga = null;
                bool cteVinculadoACargaAux;
                if (bool.TryParse(Request.Params("CTeVinculadoACarga"), out cteVinculadoACargaAux))
                    cteVinculadoACarga = cteVinculadoACargaAux;

                List<int> gruposPessoas = JsonConvert.DeserializeObject<List<int>>(Request.Params("GruposPessoas"));

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                string stringConexao = _conexao.StringConexao;
                _ = Task.Factory.StartNew(() => GerarRelatorioPosicaoContasReceber(agrupamentos, gruposPessoas, valorCTeInicial, valorCTeFinal, dataInicialQuitacao, dataFinalQuitacao, dataInicialMovimento, dataFinalMovimento, statusCTe, codigoGrupoCTe, situacao, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento, cteVinculadoACarga, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioPosicaoContasReceber(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, List<int> gruposPessoas, decimal valorCTeInicial, decimal valorCTeFinal, DateTime dataInicialQuitacao, DateTime dataFinalQuitacao, DateTime dataInicialMovimento, DateTime dataFinalMovimento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo statusCTe, int codigoGrupoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status, double cnpjPessoa, int codigoCTe, int codigoFatura, int codigoGrupoPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, DateTime dataInicialVencimento, DateTime dataFinalVencimento, bool? cteVinculadoACarga, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                Servicos.Log.TratarErro("INICIO View do relatório de Posição de Contas a Receber");
                //List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoContasReceber> listaPosicaoContasReceber = repTitulo.RelatorioPosicaoContasReceber(gruposPessoas, valorCTeInicial, valorCTeFinal, dataInicialQuitacao, dataFinalQuitacao, dataInicialMovimento, dataFinalMovimento, statusCTe, codigoGrupoCTe, situacao, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, false);
                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoContasReceber> listaPosicaoContasReceber = repTitulo.RelatorioPosicaoContasReceber2(propriedades, gruposPessoas, valorCTeInicial, valorCTeFinal, dataInicialQuitacao, dataFinalQuitacao, dataInicialMovimento, dataFinalMovimento, statusCTe, codigoGrupoCTe, situacao, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, dataInicialVencimento, dataFinalVencimento, cteVinculadoACarga, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, false);
                //var lista = (from obj in listaPosicaoContasReceber
                //             select new
                //             {
                //                 obj.Filial,
                //                 obj.NumeroCTe,
                //                 obj.SerieCTe,
                //                 obj.ComponentesFrete,
                //                 obj.Modelo,
                //                 obj.Tomador,
                //                 obj.CidadeTomador,
                //                 obj.DescricaoGrupo,
                //                 obj.Remetente,
                //                 obj.CNPJRemetente,
                //                 obj.Destinatario,
                //                 obj.CNPJDestinatario,
                //                 obj.ValorReceber,
                //                 obj.Frotas,
                //                 obj.Placas,
                //                 obj.ProprioTerceiro,
                //                 obj.Motoristas,
                //                 DataEmissaoCTe = obj.DataEmissaoCTe != null && obj.DataEmissaoCTe > DateTime.MinValue ? obj.DataEmissaoCTe.ToString("dd/MM/yyyy") : string.Empty,
                //                 obj.Notas,
                //                 obj.Origem,
                //                 obj.UFOrigem,
                //                 obj.Destino,
                //                 obj.UFDestino,
                //                 obj.NumeroDTMinuta,
                //                 obj.NumeroCarga,
                //                 DataEmissaoCarga = obj.DataEmissaoCarga != null && obj.DataEmissaoCarga > DateTime.MinValue ? obj.DataEmissaoCarga.ToString("dd/MM/yyyy") : string.Empty,
                //                 obj.NumeroFatura,
                //                 obj.NumeroPreFatura,
                //                 obj.GrupoFatura,
                //                 obj.ClienteFatura,
                //                 DataEmissaoFatura = obj.DataEmissaoFatura != null && obj.DataEmissaoFatura > DateTime.MinValue ? obj.DataEmissaoFatura.ToString("dd/MM/yyyy") : string.Empty,
                //                 DataVencimentoTitulo = obj.DataVencimentoTitulo != null && obj.DataVencimentoTitulo > DateTime.MinValue ? obj.DataVencimentoTitulo.ToString("dd/MM/yyyy") : string.Empty,
                //                 obj.ValorTitulo,
                //                 obj.ValorPendenteTitulo,
                //                 obj.ClienteTitulo,
                //                 DataEmissaoTitulo = obj.DataEmissaoTitulo != null && obj.DataEmissaoTitulo > DateTime.MinValue ? obj.DataEmissaoTitulo.ToString("dd/MM/yyyy") : string.Empty,
                //                 DataBaseBaixa = obj.DataBaseBaixa != null && obj.DataBaseBaixa > DateTime.MinValue ? obj.DataBaseBaixa.ToString("dd/MM/yyyy") : string.Empty,
                //                 DataMovimento = obj.DataMovimento != null && obj.DataMovimento > DateTime.MinValue ? obj.DataMovimento.ToString("dd/MM/yyyy") : string.Empty,
                //                 obj.StatusTitulo
                //             }).ToList();

                Servicos.Log.TratarErro("FIM View do relatório de Posição de Contas a Receber");

                Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
                identificacaoCamposRPT.PrefixoCamposSum = "";
                identificacaoCamposRPT.IndiceSumGroup = "3";
                identificacaoCamposRPT.IndiceSumReport = "4";
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                if (codigoCTe > 0)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTe", "(" + cte.Numero + ") " + cte.Chave, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTe", false));

                if (codigoFatura > 0)
                {
                    Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fatura", "(" + fatura.Codigo + ") " + fatura.Numero, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fatura", false));

                if (codigoGrupoPessoa > 0)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupo = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", grupo.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", false));

                if (cnpjPessoa > 0)
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cnpjPessoa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", "(" + cliente.CPF_CNPJ_Formatado + ") " + cliente.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", false));

                if ((int)situacao > 0)
                {
                    if (situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPosicaoContasReceber.CTeComFatura)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", "CT-e Com Fatura", true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", "CT-e Sem Fatura", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", "Todos", true));

                if ((int)status > 0)
                {
                    if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", "Em Aberto", true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", "Quitado", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", "Todos", true));

                if (cteVinculadoACarga.HasValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTeVinculadoACarga", cteVinculadoACarga.Value? "Sim": "Não", true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTeVinculadoACarga", false));

                if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "De " + dataInicialEmissao.ToString("dd/MM/yyyy") + " até " + dataFinalEmissao.ToString("dd/MM/yyyy"), true));
                else if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao == DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "De " + dataInicialEmissao.ToString("dd/MM/yyyy"), true));
                else if (dataInicialEmissao == DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "Até " + dataFinalEmissao.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "Todos", true));

                if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimento", "De " + dataInicialVencimento.ToString("dd/MM/yyyy") + " até " + dataFinalVencimento.ToString("dd/MM/yyyy"), true));
                else if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento == DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimento", "De " + dataInicialVencimento.ToString("dd/MM/yyyy"), true));
                else if (dataInicialVencimento == DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimento", "Até " + dataFinalVencimento.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimento", "Todos", true));

                if (!string.IsNullOrWhiteSpace(relatorioTemp.PropriedadeAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", relatorioTemp.PropriedadeAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

                if (gruposPessoas != null && gruposPessoas.Count > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoaCTe", "Multiplos grupos", true));
                else if (codigoGrupoCTe > 0)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupo = repGrupoPessoas.BuscarPorCodigo(codigoGrupoCTe);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoaCTe", grupo.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoaCTe", false));

                if ((int)statusCTe > 0)
                {
                    if (statusCTe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusCTe", "Em Aberto", true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusCTe", "Quitado", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusCTe", "Todos", true));

                if (dataInicialMovimento > DateTime.MinValue && dataFinalMovimento > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataMovimento", "De " + dataInicialMovimento.ToString("dd/MM/yyyy") + " até " + dataFinalMovimento.ToString("dd/MM/yyyy"), true));
                else if (dataInicialMovimento > DateTime.MinValue && dataFinalMovimento == DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataMovimento", "De " + dataInicialMovimento.ToString("dd/MM/yyyy"), true));
                else if (dataInicialMovimento == DateTime.MinValue && dataFinalMovimento > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataMovimento", "Até " + dataFinalMovimento.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataMovimento", "Todos", true));

                if (dataInicialQuitacao > DateTime.MinValue && dataFinalQuitacao > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataQuitacao", "De " + dataInicialQuitacao.ToString("dd/MM/yyyy") + " até " + dataFinalQuitacao.ToString("dd/MM/yyyy"), true));
                else if (dataInicialQuitacao > DateTime.MinValue && dataFinalQuitacao == DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataQuitacao", "De " + dataInicialQuitacao.ToString("dd/MM/yyyy"), true));
                else if (dataInicialQuitacao == DateTime.MinValue && dataFinalQuitacao > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataQuitacao", "Até " + dataFinalQuitacao.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataQuitacao", "Todos", true));

                if (valorCTeInicial > 0 && valorCTeFinal > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorCTe", "De " + valorCTeInicial.ToString("n2") + " até " + valorCTeFinal.ToString("n2"), true));
                else if (valorCTeInicial > 0 && valorCTeFinal == 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorCTe", "De " + valorCTeInicial.ToString("n2"), true));
                else if (valorCTeInicial == 0 && valorCTeFinal > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorCTe", "Até " + valorCTeFinal.ToString("n2"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorCTe", "Todos", true));

                serRelatorio.GerarRelatorioDinamico( "Relatorios/Financeiros/PosicaoContasReceber", parametros,relatorioControleGeracao, relatorioTemp, listaPosicaoContasReceber, unitOfWork, identificacaoCamposRPT);

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
