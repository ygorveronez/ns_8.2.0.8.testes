using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/DocumentoHavan")]
    public class DocumentoHavanController : BaseController
    {
		#region Construtores

		public DocumentoHavanController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> GerarArquivoRota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> codigosTabelaFrete = new List<int> { 2, 399, 467 }; //TABELA HAVAN = 2, TABELA HAVAN RETORNO MEIO = 399, TABELA HAVAN RETORNO CHEIO = 467  de código fixo da TRANSBEN

                DateTime dataVigencia = Request.GetDateTimeParam("DataVigencia");

                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.DocumentoHavan> dadosTabelaFrete = repTabelaFreteCliente.BuscarRelatorioConsultaTabelaFretePorRota(codigosTabelaFrete, dataVigencia);

                MemoryStream arquivoINPUT = new MemoryStream();
                StreamWriter x = new StreamWriter(arquivoINPUT, new UTF8Encoding(false));
                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");

                x.WriteLine("ROTEIRO;TIPO;PRECO;VALIDADE");
                for (int i = 0; i < dadosTabelaFrete.Count(); i++)
                {
                    Dominio.Relatorios.Embarcador.DataSource.Fretes.DocumentoHavan arquivo = dadosTabelaFrete[i];

                    x.WriteLine(
                        arquivo.CodigoIntegracao.Replace("#:", "").Replace("#", "").Trim() + ";" + //ROTEIRO
                        arquivo.DescricaoParametroBase.Replace("CARRETA3E", "CARRETA").Trim() + ";" + //TIPO
                        arquivo.ValorTipoCarga.ToString("f2", cultureInfo) + ";" + //PRECO
                        arquivo.DataFinal //VALIDADE
                    );
                }
                x.Flush();

                return Arquivo(arquivoINPUT.ToArray(), "text/txt", string.Concat("Rota_Havan_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo de Rota.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarArquivoPedagio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTabelaFrete = 2; //TABELA HAVAN de código fixo da TRANSBEN

                DateTime dataVigencia = Request.GetDateTimeParam("DataVigencia");

                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.DocumentoHavan> dadosTabelaFrete = repTabelaFreteCliente.BuscarRelatorioConsultaTabelaFretePorPedagio(codigoTabelaFrete, dataVigencia);

                MemoryStream arquivoINPUT = new MemoryStream();
                StreamWriter x = new StreamWriter(arquivoINPUT, new UTF8Encoding(false));
                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");

                x.WriteLine("ROTEIRO;TIPO;VALOR;DATA_INICIO_VIGENCIA");
                for (int i = 0; i < dadosTabelaFrete.Count(); i++)
                {
                    Dominio.Relatorios.Embarcador.DataSource.Fretes.DocumentoHavan arquivo = dadosTabelaFrete[i];

                    x.WriteLine(
                        arquivo.CodigoIntegracao.Replace("#:", "").Replace("#", "").Trim() + ";" + //ROTEIRO
                        arquivo.DescricaoParametroBase.Replace("CARRETA3E", "CARRETA").Trim() + ";" + //TIPO
                        arquivo.ValorComponente1.ToString("f2", cultureInfo) + ";" + //VALOR
                        arquivo.DataInicial //DATA_INICIO_VIGENCIA
                    );
                }
                x.Flush();

                return Arquivo(arquivoINPUT.ToArray(), "text/txt", string.Concat("Pedagio_Havan_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo de Pedágio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarExcelValorFreteCargaModeloVeicular()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTabelaFrete = 2; //TABELA HAVAN de código fixo da TRANSBEN

                DateTime dataVigencia = Request.GetDateTimeParam("DataVigencia");

                Models.Grid.Grid grid = new Models.Grid.Grid();
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Código", "CodigoIntegracao", 20, Models.Grid.Align.left);

                grid.AdicionarCabecalho("Cidade Origem", "CidadeOrigem", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Bairro Origem", "BairroOrigem", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("UF Origem", "UFOrigem", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Cidade Destino", "CidadeDestino", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Bairro Destino", "BairroDestino", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("UF Destino", "UFDestino", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("KM", "Quilometragem", 10, Models.Grid.Align.left);

                grid.AdicionarCabecalho("BITREM", "BITREM", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("PEDÁGIO BITREM", "PEDAGIOBITREM", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("BITREM HAVAN", "BITREMHAVAN", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("PEDÁGIO BITREM HAVAN", "PEDAGIOBITREMHAVAN", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("CARRETA2E", "CARRETA2E", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("PEDÁGIO CARRETA2E", "PEDAGIOCARRETA2E", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("CARRETA3E", "CARRETA3E", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("PEDÁGIO CARRETA3E", "PEDAGIOCARRETA3E", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("TOCO", "TOCO", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("PEDÁGIO TOCO", "PEDAGIOTOCO", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("TRUCK", "TRUCK", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("PEDÁGIO TRUCK", "PEDAGIOTRUCK", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("CARRETA 6 EIXOS LS", "CARRETA6EIXOSLS", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("PEDÁGIO CARRETA 6 EIXOS LS", "PEDAGIOCARRETA6EIXOSLS", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("CARRETA 6 EIXOS VANDERLEIA", "CARRETA6EIXOSVANDERLEIA", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("PEDÁGIO CARRETA 6 EIXOS VANDERLEIA", "PEDAGIOCARRETA6EIXOSVANDERLEIA", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("CARRETA HAVAN", "CARRETAHAVAN", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("PEDÁGIO CARRETA HAVAN", "PEDAGIOCARRETAHAVAN", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("CARRETA RODOTREM", "CARRETARODOTREM", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("PEDÁGIO CARRETA RODOTREM", "PEDAGIOCARRETARODOTREM", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("CARRETA RODOTREM HAVAN", "CARRETARODOTREMHAVAN", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("PEDÁGIO CARRETA RODOTREM HAVAN", "PEDAGIOCARRETARODOTREMHAVAN", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("RODOTREM", "RODOTREM", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("PEDÁGIO RODOTREM", "PEDAGIORODOTREM", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("RODOTREM HAVAN", "RODOTREMHAVAN", 10, Models.Grid.Align.right);
                grid.AdicionarCabecalho("PEDÁGIO RODOTREM HAVAN", "PEDAGIORODOTREMHAVAN", 10, Models.Grid.Align.right);

                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.DocumentoHavanModeloVeicular> dadosTabelaFrete = repTabelaFreteCliente.BuscarRelatorioConsultaTabelaFretePorModeloVeicular(codigoTabelaFrete, dataVigencia);
                int count = dadosTabelaFrete.Count();

                grid.AdicionaRows(dadosTabelaFrete);
                grid.setarQuantidadeTotal(count);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", string.Concat("Valor_Frete_Carga_Modelo_Veicular_Havan_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".", grid.extensaoCSV));
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar os Dados Salvos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarExcelTabelaPagamentoTerceiros()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoTabelaFrete = 51; //Tabela PAGAMENTO DE TERCEIROS de código fixo da TRANSBEN

                DateTime dataVigencia = Request.GetDateTimeParam("DataVigencia");

                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
                Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete repComponenteFreteTabelaFrete = new Repositorio.Embarcador.Frete.ComponenteFreteTabelaFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);
                List<string> componentesFrete = repComponenteFreteTabelaFrete.BuscarDescricaoComponentesPorTabelaFrete(codigoTabelaFrete);

                Models.Grid.Grid grid = new Models.Grid.Grid();
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Código", "CodigoIntegracao", 20, Models.Grid.Align.left);

                grid.AdicionarCabecalho("Cidade Origem", "CidadeOrigem", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("UF Origem", "UFOrigem", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("Cidade Destino", "CidadeDestino", 20, Models.Grid.Align.left);
                grid.AdicionarCabecalho("UF Destino", "UFDestino", 10, Models.Grid.Align.left);
                grid.AdicionarCabecalho("KM", "Quilometragem", 10, Models.Grid.Align.left);

                List<string> modelosReboque = tabelaFrete.ModelosReboque.OrderBy(o => o.Descricao).Select(o => o.Descricao).ToList();
                foreach (string modelo in modelosReboque)
                    grid.AdicionarCabecalho(modelo, "1-" + modelo, 10, Models.Grid.Align.left);

                foreach (string componente in componentesFrete)
                    grid.AdicionarCabecalho(componente, "2-" + componente, 10, Models.Grid.Align.left);

                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                IList<dynamic> dadosTabelaFrete = repTabelaFreteCliente.BuscarRelatorioConsultaTabelaFretePagamentoTerceiros(codigoTabelaFrete, dataVigencia);
                int count = dadosTabelaFrete.Count();

                var lista = (from obj in dadosTabelaFrete
                             select ObterDetalhesTabelaPagamentoTerceiros(obj, modelosReboque, componentesFrete)).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(count);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", string.Concat("Tabela_Pagamento_Terceiros_", Utilidades.String.OnlyNumbers(DateTime.Now.ToString()), ".", grid.extensaoCSV));
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar os dados de pagamento de terceiros.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterDetalhesTabelaPagamentoTerceiros(dynamic dadosTabelaCliente, List<string> modelosReboque, List<string> componentesFrete)
        {
            IDictionary<string, object> retorno = new ExpandoObject();

            int qtdColunas = 0;

            retorno.Add("Codigo", dadosTabelaCliente[qtdColunas++]);
            retorno.Add("CodigoIntegracao", dadosTabelaCliente[qtdColunas++]);
            retorno.Add("CidadeOrigem", dadosTabelaCliente[qtdColunas++]);
            retorno.Add("UFOrigem", dadosTabelaCliente[qtdColunas++]);
            retorno.Add("CidadeDestino", dadosTabelaCliente[qtdColunas++]);
            retorno.Add("UFDestino", dadosTabelaCliente[qtdColunas++]);
            retorno.Add("Quilometragem", dadosTabelaCliente[qtdColunas++]);

            foreach (string modelo in modelosReboque)
                retorno.Add("1-" + modelo, dadosTabelaCliente[qtdColunas++] ?? 0.00);

            foreach (string componente in componentesFrete)
                retorno.Add("2-" + componente, dadosTabelaCliente[qtdColunas++] ?? 0.00);

            return retorno;
        }

        #endregion
    }
}
