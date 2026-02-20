using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Threading;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Cargas/Encaixe")]
    public class EncaixeController : BaseController
    {
        #region Construtores

        public EncaixeController(Conexao conexao) : base(conexao) { }

        #endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R092_Encaixe;

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            decimal TamanhoColunasValores = (decimal)3;
            decimal TamanhoColunasInformativo = (decimal)3.75;
            decimal TamanhoColunasDescritivo = (decimal)5.30;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);

            bool ocultaProp = false;

            grid.AdicionarCabecalho("Carga De Encaixe", "CargaDeEncaixe", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, true, ocultaProp);
            grid.AdicionarCabecalho("Transportador", "Transportador", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Veículo", "Veiculo", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, ocultaProp);
            grid.AdicionarCabecalho("Motorista", "Motoristas", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, ocultaProp);
            grid.AdicionarCabecalho("Carga Encaixada", "CargaEncaixada", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Pedido Encaixado", "PedidoEncaixado", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("CT-es Do Encaixe", "CTesDoEncaixe", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, ocultaProp);
            grid.AdicionarCabecalho("CT-es Encaixados", "CTesEncaixados", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Notas Encaixadas", "NotasEncaixadas", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Código Cliente Encaixado", "CodigoClienteEncaixado", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, ocultaProp);
            grid.AdicionarCabecalho("CNPJ Cliente Encaixado", "CNPJClienteEncaixado", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, ocultaProp);
            grid.AdicionarCabecalho("Cliente Encaixado", "ClienteEncaixado", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Localidade Cliente Encaixado", "LocalidadeClienteEncaixado", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, ocultaProp);
            grid.AdicionarCabecalho("Valor Prestação Encaixe", "ValorPrestacaoEncaixe", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, ocultaProp, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Encaixes", "Cargas", "Encaixe.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "CargaDeEncaixe", "desc", "CargaDeEncaixe", "desc", codigoRelatorio, unitOfWork, true, true, 8);

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

                List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Encaixe.RelatorioEncaixe> reportResult = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = null;

                int quantidade = 0;

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                await ExecutarBusca(reportResult, quantidade, parametros, propriedades, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork, cancellationToken);

                grid.setarQuantidadeTotal(quantidade);
                grid.AdicionaRows(reportResult);

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

                _ = Task.Factory.StartNew(() => GerarRelatorioPedidos(agrupamentos, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioPedidos(
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades,
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao,
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao,
            CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Empresa empresaRelatorio = await repEmpresa.BuscarPorCodigoAsync(Empresa.Codigo, cancellationToken);

                string propOrdena = relatorioTemp.PropriedadeOrdena;
                string dirOrdena = relatorioTemp.OrdemOrdenacao;
                string propAgrupa = relatorioTemp.PropriedadeAgrupa;
                string dirAgrupa = relatorioTemp.OrdemAgrupamento;

                List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Encaixe.RelatorioEncaixe> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
                int quantidade = 0;
                await ExecutarBusca(listaReport, quantidade, parametros, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0, unitOfWork, cancellationToken);
                serRelatorio.GerarRelatorioDinamico("Relatorios/Cargas/Encaixe", parametros, relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
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

        private void SetarPropriedadeOrdenacao(ref string propOrdena)
        {
        }

        private async Task ExecutarBusca(
            IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Encaixe.RelatorioEncaixe> reportResult,
            int quantidade, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros,
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades,
            string propAgrupa,
            string dirAgrupa,
            string propOrdena,
            string dirOrdena,
            int inicio, int limite,
            Repositorio.UnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);

            int.TryParse(Request.Params("Transportador"), out int codigoTransportador);
            int.TryParse(Request.Params("Filial"), out int codigoFilial);
            int.TryParse(Request.Params("Origem"), out int codigoOrigem);
            int.TryParse(Request.Params("Destino"), out int codigoDestino);
            int.TryParse(Request.Params("TipoCarga"), out int codigoTipoCarga);
            int.TryParse(Request.Params("ModeloVeiculo"), out int codigoModeloVeiculo);
            int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);
            int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);
            int.TryParse(Request.Params("TipoOperacao"), out int codigoTipoOperacao);
            int.TryParse(Request.Params("Motorista"), out int codigoMotorista);
            int.TryParse(Request.Params("NotaEncaixada"), out int notaEncaixada);

            Enum.TryParse(Request.Params("TipoLocalPrestacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao tipoLocalPrestacao);

            double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Destinatario")), out double cpfCnpjDestinatario);
            double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Remetente")), out double cpfCnpjRemetente);

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            string cargaEncaixada = (Request.Params("CargaEncaixada") ?? string.Empty).Trim();
            string pedidoEncaixado = (Request.Params("PedidoEncaixado") ?? string.Empty).Trim();

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga> situacoes = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga>>(Request.Params("Situacoes"));

            // TODO (ct-reports): Repassar CT
            List<int> codigosFilial = codigoFilial == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { codigoFilial };
            List<int> codigosTipoCarga = codigoTipoCarga == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoCarga };
            List<int> codigosTipoOperacao = codigoTipoOperacao == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoOperacao };

            #region Parametros

            if (parametros != null)
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeiculo = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                if (dataInicial != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", dataInicial.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", false));

                if (dataFinal != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", dataFinal.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", false));

                if (situacoes != null && situacoes.Count > 0)
                {
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", string.Join(",", situacoes.Select(o => o.ObterDescricao())), true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", false));

                if (cpfCnpjRemetente > 0)
                {
                    Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(cpfCnpjRemetente);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", remetente.CPF_CNPJ_Formatado + " - " + remetente.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", false));

                if (cpfCnpjDestinatario > 0)
                {
                    Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", destinatario.CPF_CNPJ_Formatado + " - " + destinatario.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", false));

                if (codigoOrigem > 0)
                {
                    Dominio.Entidades.Localidade origem = repLocalidade.BuscarPorCodigo(codigoOrigem);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", origem.DescricaoCidadeEstado, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", false));

                if (codigoDestino > 0)
                {
                    Dominio.Entidades.Localidade destino = repLocalidade.BuscarPorCodigo(codigoDestino);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", destino.DescricaoCidadeEstado, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", false));

                if (codigoTransportador > 0)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoTransportador);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

                if (codigoFilial > 0)
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(codigoFilial);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", filial.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", false));

                if (codigoGrupoPessoas > 0)
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupo = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", grupo.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", false));

                if (codigoTipoCarga > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repTipoCarga.BuscarPorCodigo(codigoTipoCarga);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", tipoCarga.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoCarga", false));

                if (codigoTipoOperacao > 0)
                {
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", false));

                if (codigoModeloVeiculo > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeiculo = repModeloVeiculo.BuscarPorCodigo(codigoModeloVeiculo);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", modeloVeiculo.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeiculo", false));

                if (codigoVeiculo > 0)
                {
                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo.Placa, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));

                if (codigoMotorista > 0)
                {
                    Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(codigoMotorista);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", motorista.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Motorista", false));

                if (tipoLocalPrestacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao.todos)
                {
                    string strLocalPrestacao = "Municipal";
                    if (tipoLocalPrestacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao.interMunicipal)
                        strLocalPrestacao = "Intermunicipal";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoLocalPrestacao", strLocalPrestacao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoLocalPrestacao", false));

                if (!string.IsNullOrWhiteSpace(cargaEncaixada))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CargaEncaixada", cargaEncaixada, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CargaEncaixada", false));

                if (!string.IsNullOrWhiteSpace(pedidoEncaixado))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PedidoEncaixado", pedidoEncaixado, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PedidoEncaixado", false));

                if (notaEncaixada > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NotaEncaixada", notaEncaixada.ToString(), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NotaEncaixada", false));
            }
            #endregion

            SetarPropriedadeOrdenacao(ref propOrdena);
            SetarPropriedadeOrdenacao(ref propAgrupa);

            reportResult = await repCargaPedido.ConsultarRelatorioEncaixe(dataInicial, dataFinal, codigoTransportador, codigosFilial, codigoOrigem, codigoDestino, codigosTipoCarga, codigoModeloVeiculo, cpfCnpjRemetente, cpfCnpjDestinatario, codigoVeiculo, codigoMotorista, codigoGrupoPessoas, situacoes, codigosTipoOperacao, tipoLocalPrestacao, cargaEncaixada, pedidoEncaixado, notaEncaixada, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            quantidade = reportResult.Count();
        }
    }

}
