using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Escrituracao;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Escrituracao
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Escrituracao/FreteContabil")]
    public class FreteContabilController : Relatorios.AutomatizacaoGeracaoRelatorioController<Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioFreteContabil>
    {
        #region Construtores

        public FreteContabilController(Conexao conexao) : base(conexao) { }

        #endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R155_FreteContabil;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Frete Contábil", "Escrituracao", "FreteContabil.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Tomador", "asc", "", "", codigoRelatorio, unitOfWork, false, true);
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

                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioFreteContabil filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                if (!ValidaFiltro(out string msg, unitOfWork))
                    return new JsonpResult(true, false, msg);

                Servicos.Embarcador.Relatorios.Escrituracao.FreteContabil servicoRelatorioFreteContabil = new Servicos.Embarcador.Relatorios.Escrituracao.FreteContabil(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioFreteContabil.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.FreteContabil> listaFreteContabil, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaFreteContabil);

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
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioFreteContabil filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                if (!ValidaFiltro(out string msg, unitOfWork))
                    return new JsonpResult(true, false, msg);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Tomador").Nome("Tomador").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("TipoOperacao").Nome("Tipo Operação").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Filial").Nome("Filial").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Empresa").Nome("Empresa").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Origem").Nome("Origem").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false).DateTimeOnlyDate();
            grid.Prop("CidadeOrigem").Nome("Cidade Origem").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("UFOrigem").Nome("UF Origem").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Destino").Nome("Destino").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("CidadeDestino").Nome("Cidade Destino").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("UFDestino").Nome("UF Destino").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Carga").Nome("Carga").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("NumeroOcorrencia").Nome("Ocorrencia").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Pagamento").Nome("Pagamento").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("CancelamentoPagamento").Nome("Cancelamento de Pagamento").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Provisao").Nome("Provisão").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("CancelamentoProvisao").Nome("Cancelamento da Provisão").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("TipoDeContabilizacao").Nome("Tipo De Contabilização").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataEmissao").Nome("Data Emissão").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center).DateTimeOnlyDate();
            grid.Prop("Remetente").Nome("Remetente").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("Destinatario").Nome("Destinatário").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left);
            grid.Prop("Numero").Nome("Número").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("Serie").Nome("Série").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("Modelo").Nome("Modelo").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false).Visibilidade(false);
            grid.Prop("NumeroCTe").Nome("Número CT-e").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("SerieCTe").Nome("Série CT-e").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Visibilidade(false);
            grid.Prop("CodigoCentroResultado").Nome("Código Centro Resultado").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("CentroResultado").Nome("Centro Resultado").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("DataLancamento").Nome("Data Lançamento").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center).DateTimeOnlyDate();
            grid.Prop("AnoLancamento").Nome("Ano Lançamento").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center);
            grid.Prop("MesLancamento").Nome("Mês Lançamento").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center);
            grid.Prop("DataLancamentoCancelamentoProvisao").Nome("Data Lançamento Cancelamento Provisão").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center).DateTimeOnlyDate();
            grid.Prop("CodigoPlanoConta").Nome("Código Plano de Conta").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("PlanoConta").Nome("Plano de Conta").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("ValorLancamento").Nome("Valor Lançamento").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("Credito").Nome("Crédito").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("Debito").Nome("Débito").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum).Visibilidade(false);
            grid.Prop("TipoContabilizacao").Nome("Tipo Contabilização").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("ICMS").Nome("ICMS").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("ICMSRetido").Nome("ICMS Retido").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("ValorISS").Nome("ISS").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("ValorISSRetido").Nome("ISS Retido").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("Aliquota").Nome("Aliquota").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("AliquotaISS").Nome("Aliquota ISS").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Sumarizar(TipoSumarizacao.sum);
            grid.Prop("CST").Nome("CST").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("MotivoCancelamento").Nome("Motivo Cancelamento").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("TipoCarga").Nome("Tipo Carga").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ModeloVeicularCarga").Nome("Modelo Veicular").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Veiculo").Nome("Veiculo").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("PesoCTe").Nome("Peso").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("RemetenteCTe").Nome("Nome Remetente").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DestinatarioCTe").Nome("Nome Destinatario").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("NumeroVP").Nome("Número VP").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("ValorVPFormatado").Nome("Valor VP").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("Expedidor").Nome("Expedidor").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Recebedor").Nome("Recebedor").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("TipoOcorrencia").Nome("Tipo de Ocorrência").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataCriacaoCargaFormatada").Nome("Data Criação da Carga").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataEmissaoCTeFormatada").Nome("Data Emissão do CT-e").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("CNPJTransportadorFormatado").Nome("CNPJ Transportador").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("CNPJRemetenteFormatado").Nome("CNPJ do Remetente").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("CNPJDestinatarioFormatado").Nome("CNPJ do Destinatário").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("KMCargaFormatado").Nome("KM da Carga").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("PesoNF").Nome("Peso NF").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("PesoCarga").Nome("Peso Carga").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("KmRodado").Nome("KM Rodado").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);

            return grid;
        }

        private bool ValidaFiltro(out string msg, Repositorio.UnitOfWork unitOfWork)
        {
            msg = "";
            DateTime dataLancamentoInicial = Request.GetDateTimeParam("DataLancamentoInicial");
            DateTime dataLancamentoFinal = Request.GetDateTimeParam("DataLancamentoFinal");
            DateTime dataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial");
            DateTime dataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal");


            if (dataLancamentoFinal == DateTime.MinValue || dataLancamentoInicial == DateTime.MinValue)
            {
                msg = "Datas de lançamentos são obrigatórias.";
                return false;
            }

            if (dataLancamentoFinal < dataLancamentoInicial)
            {
                msg = "Data Final de lançamento deve ser maior que a data inicial.";
                return false;
            }

            if ((dataLancamentoFinal - dataLancamentoInicial).Days > 30)
            {
                msg = "Intervalo de lançamento não pode ser maior que 30 dias.";
                return false;
            }

            if (dataEmissaoFinal < dataEmissaoInicial)
            {
                msg = "Data Final de emissão deve ser maior que a data inicial de emissão.";
                return false;
            }

            if ((dataEmissaoFinal - dataEmissaoInicial).Days > 31)
            {
                msg = "Intervalo de emissão não pode ser maior que 31 dias.";
                return false;
            }

            return true;
        }

        protected override FiltroRelatorioFreteContabil ObterFiltrosPesquisa(UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroRelatorioFreteContabil
            {
                Filial = Request.GetIntParam("Filial"),
                Transportador = Request.GetIntParam("Transportador"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                Tomador = Request.GetDoubleParam("Tomador"),
                CentroResultado = Request.GetListParam<int>("CentroResultado"),
                DataLancamentoInicial = Request.GetDateTimeParam("DataLancamentoInicial"),
                DataLancamentoFinal = Request.GetDateTimeParam("DataLancamentoFinal"),
                ContaContabil = Request.GetListParam<int>("ContaContabil"),
                CodigoCarga = Request.GetIntParam("Carga"),
            };
        }

        protected override Task<FiltroRelatorioFreteContabil> ObterFiltrosPesquisaAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}