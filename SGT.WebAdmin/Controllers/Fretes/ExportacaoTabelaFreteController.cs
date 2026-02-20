using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/ExportacaoTabelaFrete")]
    public class ExportacaoTabelaFreteController : BaseController
    {
		#region Construtores

		public ExportacaoTabelaFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadValorFrete()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);

                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");

                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                // TODO: ToList cast
                List<Dominio.ObjetosDeValor.Embarcador.Frete.ExportacaoTabelaFrete> dados = repTabelaFreteCliente.ObterValoresFreteExportacao().ToList();

                StringBuilder sb = new StringBuilder();

                sb.Append("ROTEIRO;TIPO;PRECO;VALIDADE").AppendLine();

                dados.ForEach((Dominio.ObjetosDeValor.Embarcador.Frete.ExportacaoTabelaFrete dado) =>
                {
                    sb.Append(dado.CodigoIntegracao).Append(";")
                      .Append(dado.DescricaoParametroBase).Append(";")
                      .Append(dado.Valor.ToString("F2", culture)).Append(";")
                      .Append(dado.DataFinal)
                      .AppendLine();
                });

                byte[] arquivo = System.Text.Encoding.UTF8.GetBytes(sb.ToString());

                return Arquivo(arquivo, "text/plain", "Tabela de Valores de Frete.txt");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadValorPedagio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);

                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");

                Repositorio.Embarcador.Frete.TabelaFreteCliente repTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                // TODO: ToList cast
                List<Dominio.ObjetosDeValor.Embarcador.Frete.ExportacaoTabelaFrete> dados = repTabelaFreteCliente.ObterValoresPedagioExportacao().ToList();

                StringBuilder sb = new StringBuilder();

                sb.Append("IBGE_ORIGEM;IBGE_DESTINO;ROTEIRO;TIPO;VALOR;DATA_INICIO_VIGENCIA").AppendLine();

                dados.ForEach((Dominio.ObjetosDeValor.Embarcador.Frete.ExportacaoTabelaFrete dado) =>
                {
                    sb.Append(";;")
                      .Append(dado.CodigoIntegracao).Append(";")
                      .Append(dado.DescricaoParametroBase).Append(";")
                      .Append(dado.Valor.ToString("F2", culture)).Append(";")
                      .Append(dado.DataInicial)
                      .AppendLine();
                });

                byte[] arquivo = System.Text.Encoding.UTF8.GetBytes(sb.ToString());

                return Arquivo(arquivo, "text/plain", "Tabela de Valores de Pedágio.txt");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do lote de XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
