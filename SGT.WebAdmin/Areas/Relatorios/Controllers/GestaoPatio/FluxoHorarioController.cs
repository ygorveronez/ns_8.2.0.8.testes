using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.GestaoPatio
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/GestaoPatio/FluxoHorario")]
    public class FluxoHorarioController : BaseController
    {
		#region Construtores

		public FluxoHorarioController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R123_FluxoHorario;
        private readonly int TotalHoras = 24;

        private Models.Grid.Grid GridPadrao(DateTime dataInicio, DateTime dataFinal)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Hora").Nome("Hora Chegada").Tamanho(10).Align(Models.Grid.Align.center).Ord(false);

            // Escreve os dias dentor do range Ex: 14/01/2000 até 13/02/2000
            for (DateTime dia = dataInicio; dia <= dataFinal; dia = dia.AddDays(1))
            {
                grid.Prop("Dia" + dia.Day.ToString())
                    .Nome(dia.Day.ToString())
                    .Tamanho(3)
                    .Align(Models.Grid.Align.center)
                    .Ord(false)
                    .Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum)
                ;
            }

            if (dataFinal.Day + 1 < dataInicio.Day)
            {
                OcultarDias(ref grid, dataFinal.Day + 1, dataInicio.Day);
            }
            else
            {
                if (dataInicio.Day > 1)
                    OcultarDias(ref grid, 1, dataInicio.Day);

                if (dataFinal.Day < 31)
                    OcultarDias(ref grid, dataFinal.Day + 1, 32);
            }
            //for (DateTime dia = new DateTime(1, dataInicio.Month, dataInicio.Year); dia < dataInicio; dia = dia.AddDays(1))
            //{
            //    grid.Prop("Dia" + dia.ToString())
            //        .Ocultar(true)
            //        .Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum)
            //    ;
            //}

            grid.Prop("TotalHora").Nome("Total").Tamanho(5).Align(Models.Grid.Align.center).Ord(false).Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);

            return grid;
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorarioMatriz> reportResult = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = null;

                ExecutarBusca(Request, ref reportResult, ref parametros, out DateTime dataInicio, out DateTime dataFim, unitOfWork);

                dynamic lista = FormataMatrizPreview(reportResult);

                Models.Grid.Grid grid = GridPadrao(dataInicio, dataFim);

                grid.setarQuantidadeTotal(TotalHoras);
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
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao); ;
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Fluxo no Pátio", "GestaoPatio", "FluxoHorario.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Hora", "asc", "", "", Codigo, unitOfWork, true, true, 7);

                Models.Grid.Grid grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>()
                };
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
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorioAsync(Request, agrupamentos, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
        }

        private async Task GerarRelatorioAsync(HttpRequest request, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Empresa empresaRelatorio = repEmpresa.BuscarPorCodigo(Empresa.Codigo);

                List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorarioMatriz> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                ExecutarBusca(request, ref listaReport, ref parametros, out DateTime datainicio, out DateTime datafim, unitOfWork);

                Models.Grid.Grid grid = GridPadrao(datainicio, datafim);
                relatorioTemp.Colunas = new List<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna>();
                foreach (Models.Grid.Head head in grid.header)
                {
                    Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna coluna = mdlRelatorio.BuscarColunaPeloHead(head, relatorioTemp);
                    relatorioTemp.Colunas.Add(coluna);
                }


                var result = ReportRequest.WithType(ReportType.FluxoHorario)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("relatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("listaReport", listaReport.ToJson())
                    .AddExtraData("CaminhoLogoDacte", empresaRelatorio.CaminhoLogoDacte)
                    .AddExtraData("relatorioTemp", serRelatorio.ObterConfiguracaoRelatorio(relatorioTemp).ToJson())
                    .AddExtraData("parametros", parametros.ToJson())
                    .AddExtraData("caminho", "Relatorios/GestaoPatio/FluxoHorario")
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                    serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, result.ErrorMessage);

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

        private void SetaDiaMatriz(string dia, int quantidade, object obj)
        {
            System.Reflection.PropertyInfo propertyInfo = obj.GetType().GetProperty(dia);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, quantidade, null);
            }
        }

        private void ExecutarBusca(HttpRequest request, ref List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorarioMatriz> reportResult, ref List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros, out DateTime dataInicio, out DateTime dataFim, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);

            DateTime.TryParseExact(request.Params("DataInicio"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
            DateTime.TryParseExact(request.Params("DataFim"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

            int.TryParse(request.Params("Filial"), out int filial);

            Enum.TryParse(request.Params("EtapaFluxoGestaoPatio"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapaFluxoGestaoPatio);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoPatio tipoFluxoGestaoPatio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoPatio.Origem;
            string campo = CampoEntidadeReferenteAEtapa(etapaFluxoGestaoPatio);

            if (parametros != null)
            {
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicio", dataInicio.ToString("dd/MM/yyyy"), true));

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFim", dataFim.ToString("dd/MM/yyyy"), true));

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EtapaFluxoGestaoPatio", new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork).ObterDescricaoEtapa(etapaFluxoGestaoPatio, tipoFluxoGestaoPatio).Descricao, true));

                if (filial > 0)
                {
                    var aux = repFilial.BuscarPorCodigo(filial);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", aux.Descricao, true));
                }
                else
                {
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", false));
                }
            }

            IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorario> fluxoHorarios = repFluxoGestaoPatio.ConsultarRelatorioFluxoHorario(campo, dataInicio, dataFim, etapaFluxoGestaoPatio, filial, tipoFluxoGestaoPatio);
            reportResult = ConverteDadosEmMatriz(fluxoHorarios);
        }

        private string CampoEntidadeReferenteAEtapa(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapaFluxoGestaoPatio)
        {
            switch (etapaFluxoGestaoPatio)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InformarDoca: return "FGP_DOCA_INFORMADA";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.ChegadaVeiculo: return "FGP_CHEGADA_VEICULO";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Guarita: return "FGP_DATA_ENTRADA_GUARITA";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.CheckList: return "FGP_DATA_FIM_CHECKLIST";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.TravamentoChave: return "FGP_TRAVA_CHAVE";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Expedicao: return "FGP_DATA_INICIO_CARREGAMENTO";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.LiberacaoChave: return "FGP_LIBERACAO_CHAVE";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Faturamento: return "FGP_FATURAMENTO";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioViagem: return "FGP_DATA_SAIDA_GUARITA";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Posicao: return "FGP_POSICAO";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.ChegadaLoja: return "FGP_CHEGADA_LOJA";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.DeslocamentoPatio: return "FGP_DESLOCAMENTO_PATIO";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.DocumentoFiscal: return "FGP_DOCUMENTO_FISCAL";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.DocumentosTransporte: return "FGP_DOCUMENTOS_TRANSPORTE";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.SaidaLoja: return "FGP_SAIDA_LOJA";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimViagem: return "FGP_FIM_VIAGEM";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimCarregamento: return "FGP_DATA_FIM_CARREGAMENTO";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimDescarregamento: return "FGP_DATA_FIM_DESCARREGAMENTO";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimHigienizacao: return "FGP_DATA_FIM_HIGIENIZACAO";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioCarregamento: return "FGP_DATA_INICIO_CARREGAMENTO";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioDescarregamento: return "FGP_DATA_INICIO_DESCARREGAMENTO";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioHigienizacao: return "FGP_DATA_INICIO_HIGIENIZACAO";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.MontagemCarga: return "FGP_DATA_MONTAGEM_CARGA";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.SolicitacaoVeiculo: return "FGP_DATA_SOLICITACAO_VEICULO";
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.SeparacaoMercadoria: return "FGP_DATA_SEPARACAO_MERCADORIA";
                default: return "";
            }
        }

        private List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorarioMatriz> ConverteDadosEmMatriz(IList<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorario> fluxoHorarios)
        {
            List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorarioMatriz> matriz = new List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorarioMatriz>();

            for (int hora = 0; hora < TotalHoras; hora++)
            {
                IEnumerable<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorario> dadosDiasDaHora = (from o in fluxoHorarios where o.Hora == hora select o);

                Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorarioMatriz dadosHora = new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorarioMatriz()
                {
                    Hora = hora,
                    TotalHora = dadosDiasDaHora.Sum(o => o.Quantidade)
                };

                foreach (Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorario dia in dadosDiasDaHora)
                {
                    string campo = "Dia" + dia.Dia.ToString();
                    SetaDiaMatriz(campo, dia.Quantidade, dadosHora);
                }

                matriz.Add(dadosHora);
            }

            return matriz;
        }

        private dynamic FormataMatrizPreview(List<Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorarioMatriz> reportResult)
        {
            List<dynamic> listaFormatada = new List<dynamic>();
            string semValor = "-";

            foreach (Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.FluxoHorarioMatriz hora in reportResult)
            {
                listaFormatada.Add(new
                {
                    Hora = hora.Hora.ToString("00"),

                    Dia1 = (hora.Dia1 > 0 ? hora.Dia1.ToString() : semValor),
                    Dia2 = (hora.Dia2 > 0 ? hora.Dia2.ToString() : semValor),
                    Dia3 = (hora.Dia3 > 0 ? hora.Dia3.ToString() : semValor),
                    Dia4 = (hora.Dia4 > 0 ? hora.Dia4.ToString() : semValor),
                    Dia5 = (hora.Dia5 > 0 ? hora.Dia5.ToString() : semValor),
                    Dia6 = (hora.Dia6 > 0 ? hora.Dia6.ToString() : semValor),
                    Dia7 = (hora.Dia7 > 0 ? hora.Dia7.ToString() : semValor),
                    Dia8 = (hora.Dia8 > 0 ? hora.Dia8.ToString() : semValor),
                    Dia9 = (hora.Dia9 > 0 ? hora.Dia9.ToString() : semValor),
                    Dia10 = (hora.Dia10 > 0 ? hora.Dia10.ToString() : semValor),
                    Dia11 = (hora.Dia11 > 0 ? hora.Dia11.ToString() : semValor),
                    Dia12 = (hora.Dia12 > 0 ? hora.Dia12.ToString() : semValor),
                    Dia13 = (hora.Dia13 > 0 ? hora.Dia13.ToString() : semValor),
                    Dia14 = (hora.Dia14 > 0 ? hora.Dia14.ToString() : semValor),
                    Dia15 = (hora.Dia15 > 0 ? hora.Dia15.ToString() : semValor),
                    Dia16 = (hora.Dia16 > 0 ? hora.Dia16.ToString() : semValor),
                    Dia17 = (hora.Dia17 > 0 ? hora.Dia17.ToString() : semValor),
                    Dia18 = (hora.Dia18 > 0 ? hora.Dia18.ToString() : semValor),
                    Dia19 = (hora.Dia19 > 0 ? hora.Dia19.ToString() : semValor),
                    Dia20 = (hora.Dia20 > 0 ? hora.Dia20.ToString() : semValor),
                    Dia21 = (hora.Dia21 > 0 ? hora.Dia21.ToString() : semValor),
                    Dia22 = (hora.Dia22 > 0 ? hora.Dia22.ToString() : semValor),
                    Dia23 = (hora.Dia23 > 0 ? hora.Dia23.ToString() : semValor),
                    Dia24 = (hora.Dia24 > 0 ? hora.Dia24.ToString() : semValor),
                    Dia25 = (hora.Dia25 > 0 ? hora.Dia25.ToString() : semValor),
                    Dia26 = (hora.Dia26 > 0 ? hora.Dia26.ToString() : semValor),
                    Dia27 = (hora.Dia27 > 0 ? hora.Dia27.ToString() : semValor),
                    Dia28 = (hora.Dia28 > 0 ? hora.Dia28.ToString() : semValor),
                    Dia29 = (hora.Dia29 > 0 ? hora.Dia29.ToString() : semValor),
                    Dia30 = (hora.Dia30 > 0 ? hora.Dia30.ToString() : semValor),
                    Dia31 = (hora.Dia31 > 0 ? hora.Dia31.ToString() : semValor),

                    hora.TotalHora
                });
            }

            return listaFormatada;
        }

        private void OcultarDias(ref Models.Grid.Grid grid, int inicio, int fim)
        {
            for (int dia = inicio; dia < fim; dia++)
            {
                grid.Prop("Dia" + dia.ToString())
                    .Ocultar(true)
                    .Sumarizar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum)
                ;
            }
        }
    }
}
