using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/ExportacaoArquivoIntegracao")]
    public class ExportacaoArquivoIntegracaoController : BaseController
    {
		#region Construtores

		public ExportacaoArquivoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> GerarArquivoContratos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime dataEmissao = Request.GetDateTimeParam("DataEmissao");
                DateTime dataAberturaCIOT = Request.GetDateTimeParam("DataAberturaCIOT");

                Models.Grid.Grid grid = new Models.Grid.Grid();
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Data abertura CIOT", "DataAberturaCIOTFormatada", 10, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Adicional CT-e", "AdicionalCTe", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Núm. Formulário", "NumeroFormulario", 20, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Série Formulário", "SerieFormulario", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissaoFormatada", 10, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Remetente CNPJ/CPF", "CNPJCPFRemetente", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Destinatário CNPJ/CPF", "CNPJCPFDestinatario", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Peso", "Peso", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Volume", "Volume", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("M3", "MetrosCubicos", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Valor Unitário", "ValorUnitario", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Valor Frete Bruto", "ValorFreteBruto", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Valor Frete Líquido", "ValorFreteLiquido", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Motorista CPF", "CPFMotorista", 15, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Placa Controle", "PlacaControle", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Placa Referência", "PlacaReferencia", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Valor Item Adiantamento", "ValorItemAdiantamento", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Valor Pedágio", "ValorPedagio", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Número CRT Sistema Externo", "NumeroCRTSistemaExterno", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Centro de Custo Gerencial", "CentroCustoGerencial", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Data Cancelamento", "DataCancelamento", 10, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Unidade Negócio", "UnidadeNegocio", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Total Acréscimos", "TotalAcrescimos", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Total Descontos", "TotalDescontos", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Número Documento", "NumeroDocumento", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("Nome Cliente", "NomeCliente", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("CNPJ Cliente", "CNPJCPFCliente", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Centro Custo Original", "CentroCustoOriginal", 10, Models.Grid.Align.left);

                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Terceiros.ExportacaoContratoFrete> dadosContratoFrete = repContratoFrete.ConsultarRelatorioExportacaoContratoFrete(dataEmissao, dataAberturaCIOT);
                int count = dadosContratoFrete.Count();

                grid.setarQuantidadeTotal(count);
                grid.AdicionaRows(dadosContratoFrete);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", string.Concat("Arquivo_Exportacao_Contratos_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".", grid.extensaoCSV));
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar os dados salvos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
