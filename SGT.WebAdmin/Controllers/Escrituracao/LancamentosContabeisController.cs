using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/LancamentosContabeis")]
    public class LancamentosContabeisController : BaseController
    {
		#region Construtores

		public LancamentosContabeisController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        public async Task<IActionResult> ObterLancamentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                
                // Parametros
                int nota = Request.GetIntParam("Nota");
                int cte = Request.GetIntParam("CTe");
                int carga = Request.GetIntParam("Carga");
                int lotesProvisao = Request.GetIntParam("LotesProvisao");
                int pagamentos = Request.GetIntParam("Pagamentos");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNota = repXMLNotaFiscal.BuscarXMLPorCodigo(nota);

                // Valida
                if (xmlNota == null)
                    return new JsonpResult(false, true, "Documento não encontrado.");

                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> dadosNota = repDocumentoContabil.ConsultarPorChave(xmlNota.Chave);

                if (dadosNota.Count == 0 && !string.IsNullOrWhiteSpace(xmlNota.Chave))
                    return new JsonpResult(false, true, "Nenhum lançamento encontrado para o documento.");

                // Formata retorno
                var retorno = new
                {
                    xmlNota.Chave,
                    xmlNota.Numero,
                    Serie = xmlNota.Serie ?? string.Empty,
                    DataEmissao = xmlNota.DataEmissao.ToString("dd/MM/yyyy") ?? string.Empty,
                    Emitente = xmlNota.Emitente?.Descricao ?? string.Empty,
                    Destinatario = xmlNota.Destinatario?.Descricao ?? string.Empty,
                    Lancamentos = ProcessarLancamentosDocumento(dadosNota, unitOfWork)
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarLancamentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid()
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.Prop("Descricao").Nome("Descrição").Tamanho(15);
                grid.Prop("TipoContaContabilDescricao").Nome("Tipo Conta Contábil").Tamanho(10);
                grid.Prop("ContaContabil").Nome("Conta Contábil").Tamanho(20);
                grid.Prop("CentroCusto").Nome("Centro Custo").Tamanho(10);
                grid.Prop("TipoContabilizacaoDescricao").Nome("Tipo Contabilização").Tamanho(10);
                grid.Prop("ValorDescricao").Nome("Valor").Tamanho(10).Align(Models.Grid.Align.right);
                grid.Prop("DataLancamento").Nome("Data Lançamento").Tamanho(10).Align(Models.Grid.Align.center);

                int nota = Request.GetIntParam("Nota");
                int cte = Request.GetIntParam("CTe");
                int carga = Request.GetIntParam("Carga");
                int lotesProvisao = Request.GetIntParam("LotesProvisao");
                int pagamentos = Request.GetIntParam("Pagamentos");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNota = repXMLNotaFiscal.BuscarXMLPorCodigo(nota);

                // Valida
                if (xmlNota == null)
                    return new JsonpResult(false, true, "Documento não encontrado.");

                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> dadosNota = repDocumentoContabil.ConsultarPorChave(xmlNota.Chave);

                if (dadosNota.Count == 0)
                    return new JsonpResult(false, true, "Nenhum lançamento encontrado para o documento.");

                // Busca Dados
                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.LancamentoContabil> dadosProcessados = ProcessarLancamentosDocumento(dadosNota, unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.LancamentoContabilDetalhe> lista = new List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.LancamentoContabilDetalhe>();
                foreach (var linha in dadosProcessados)
                {
                    for(int i = 0, s = linha.Lancamentos.Count; i < s; i++)
                        linha.Lancamentos[i].Descricao = linha.Descricao;

                    lista.AddRange(linha.Lancamentos);
                }

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "csv", xmlNota.Chave + ".csv");
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        #endregion

        #region Métodos Privados
        public List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.LancamentoContabil> ProcessarLancamentosDocumento(List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.DocumentoContabil> dadosNota, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.LancamentoContabil> listas = new List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.LancamentoContabil>();

            List<int> provisoes = (from o in dadosNota where o.CancelamentoProvisao == null && o.Provisao != null select o.Provisao.Codigo).Distinct().ToList();
            List<int> cancelamentosProvisao = (from o in dadosNota where o.CancelamentoProvisao != null select o.CancelamentoProvisao.Codigo).Distinct().ToList();
            List<int> pagamentos = (from o in dadosNota where o.CancelamentoPagamento == null && o.Pagamento != null select o.Pagamento.Codigo).Distinct().ToList();
            List<int> cancelamentosPagamento = (from o in dadosNota where o.CancelamentoPagamento != null select o.CancelamentoPagamento.Codigo).Distinct().ToList();


            var dadosProvisoes = (from o in dadosNota where o.Provisao != null && o.CancelamentoProvisao == null && provisoes.Contains(o.Provisao.Codigo) select o).ToList();
            if (dadosProvisoes.Count > 0)
                listas.Add(new Dominio.ObjetosDeValor.Embarcador.Escrituracao.LancamentoContabil(
                    "Provisão", 
                    dadosProvisoes.FirstOrDefault().Provisao.DocumentosProvisao, 
                    dadosProvisoes
                ));
                      
            
            var dadosCancelamentosProvisao = (from o in dadosNota where o.CancelamentoProvisao != null && cancelamentosProvisao.Contains(o.CancelamentoProvisao.Codigo) select o).ToList();
            if (dadosCancelamentosProvisao.Count > 0)
                listas.Add(new Dominio.ObjetosDeValor.Embarcador.Escrituracao.LancamentoContabil(
                    "Cancelamento Provisão", 
                    dadosCancelamentosProvisao.FirstOrDefault().CancelamentoProvisao.DocumentosProvisao,
                    dadosCancelamentosProvisao
                ));


            var dadosPagamentos = (from o in dadosNota where o.Pagamento != null && o.CancelamentoPagamento == null && pagamentos.Contains(o.Pagamento.Codigo) select o).ToList();
            if (dadosPagamentos.Count > 0)
                listas.Add(new Dominio.ObjetosDeValor.Embarcador.Escrituracao.LancamentoContabil(
                    "Pagamento", 
                    dadosPagamentos.FirstOrDefault().Pagamento.DocumentosFaturamento, 
                    dadosPagamentos
                ));


            var dadosCancelamentos = (from o in dadosNota where o.CancelamentoPagamento != null && cancelamentosPagamento.Contains(o.CancelamentoPagamento.Codigo) select o).ToList();
            if (dadosCancelamentos.Count > 0)
                listas.Add(new Dominio.ObjetosDeValor.Embarcador.Escrituracao.LancamentoContabil(
                    "Cancelamento Pagamento", 
                    dadosCancelamentos.FirstOrDefault().CancelamentoPagamento.DocumentosFaturamento,
                    dadosCancelamentos
                ));

            return listas;
        }
        #endregion

    }
}
