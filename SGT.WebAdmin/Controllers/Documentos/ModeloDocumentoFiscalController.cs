using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Documentos
{
    [CustomAuthorize("Documentos/ModeloDocumentoFiscal")]
    public class ModeloDocumentoFiscalController : BaseController
    {
		#region Construtores

		public ModeloDocumentoFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                string numero = Request.Params("Numero");

                bool? somenteModeloEditavel = null, incluirNFSe = null, incluirNFSManual = null;
                bool somenteModeloEditavelAux, incluirNFSeAux, incluirNFSManualAux;

                if (bool.TryParse(Request.Params("SomenteEditavel"), out somenteModeloEditavelAux))
                    somenteModeloEditavel = somenteModeloEditavelAux;

                if (bool.TryParse(Request.Params("IncluirNFSe"), out incluirNFSeAux))
                    incluirNFSe = incluirNFSeAux;

                if (bool.TryParse(Request.Params("IncluirNFSManual"), out incluirNFSManualAux))
                    incluirNFSManual = incluirNFSManualAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.ModeloDocumentoFiscal.Numero, "Numero", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.ModeloDocumentoFiscal.Abreviacao, "Abreviacao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.ModeloDocumentoFiscal.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("DocumentoComMoedaEstrangeira", false);
                grid.AdicionarCabecalho("MoedaCotacaoBancoCentral", false);
                grid.AdicionarCabecalho("TipoDocumentoEmissao", false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);

                List<Dominio.Entidades.ModeloDocumentoFiscal> listaModeloDocumento = repModeloDocumentoFiscal.Consultar(descricao, numero, somenteModeloEditavel, incluirNFSe, incluirNFSManual, ativo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repModeloDocumentoFiscal.ContarConsulta(descricao, numero, somenteModeloEditavel, incluirNFSe, incluirNFSManual, ativo));

                var retorno = (from obj in listaModeloDocumento
                               select new
                               {
                                   obj.Codigo,
                                   obj.Numero,
                                   obj.Abreviacao,
                                   obj.Descricao,
                                   obj.DocumentoComMoedaEstrangeira,
                                   obj.MoedaCotacaoBancoCentral,
                                   obj.TipoDocumentoEmissao
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Consultas.ModeloDocumentoFiscal.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool.TryParse(Request.Params("Ativo"), out bool ativo);
                bool.TryParse(Request.Params("UtilizarNumeracaoCTe"), out bool utilizarNumeracaoCTe);
                bool.TryParse(Request.Params("GerarMovimentoAutomatico"), out bool gerarMovimentoAutomatico);
                bool.TryParse(Request.Params("AverbarDocumento"), out bool averbarDocumento);

                bool.TryParse(Request.Params("GerarMovimentoAutomaticoEntrada"), out bool gerarMovimentoAutomaticoEntrada);
                bool.TryParse(Request.Params("DiferenciarMovimentosParaImpostos"), out bool diferenciarMovimentosParaImpostos);
                bool.TryParse(Request.Params("DiferenciarMovimentosParaPIS"), out bool diferenciarMovimentosParaPIS);
                bool.TryParse(Request.Params("DiferenciarMovimentosParaCOFINS"), out bool diferenciarMovimentosParaCOFINS);
                bool.TryParse(Request.Params("DiferenciarMovimentosParaIR"), out bool diferenciarMovimentosParaIR);
                bool.TryParse(Request.Params("DiferenciarMovimentosParaCSLL"), out bool diferenciarMovimentosParaCSLL);
                bool.TryParse(Request.Params("DiferenciarMovimentosParaValorLiquido"), out bool diferenciarMovimentosParaValorLiquido);
                bool.TryParse(Request.Params("NaoGerarFaturamento"), out bool naoGerarFaturamento);
                bool.TryParse(Request.Params("GerarISSAutomaticamente"), out bool gerarISSAutomaticamente);

                bool.TryParse(Request.Params("NaoGerarEscrituracao"), out bool naoGerarEscrituracao);
                bool.TryParse(Request.Params("GerarMovimentoBaseSTRetido"), out bool gerarMovimentoBaseSTRetido);
                bool.TryParse(Request.Params("GerarMovimentoValorSTRetido"), out bool gerarMovimentoValorSTRetido);

                string descricao = Request.Params("Descricao");
                string abreviacao = Request.Params("Abreviacao");
                string numero = Request.Params("Numero");

                int.TryParse(Request.Params("TipoMovimentoUsoEntrada"), out int codigoTipoMovimentoUsoEntrada);
                int.TryParse(Request.Params("TipoMovimentoReversaoEntrada"), out int codigoTipoMovimentoReversaoEntrada);
                int.TryParse(Request.Params("TipoMovimentoEmissao"), out int codigoTipoMovimentoEmissao);
                int.TryParse(Request.Params("TipoMovimentoCancelamento"), out int codigoTipoMovimentoCancelamento);
                int.TryParse(Request.Params("TipoMovimentoAnulacao"), out int codigoTipoMovimentoAnulacao);
                int.TryParse(Request.Params("TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador"), out int codigoTipoMovimentoAnulacaoComNotaAnulacaoEmbarcador);

                int.TryParse(Request.Params("TipoMovimentoImpostoEmissao"), out int codigoTipoMovimentoImpostoEmissao);
                int.TryParse(Request.Params("TipoMovimentoImpostoCancelamento"), out int codigoTipoMovimentoImpostoCancelamento);
                int.TryParse(Request.Params("TipoMovimentoImpostoAnulacao"), out int codigoTipoMovimentoImpostoAnulacao);
                int.TryParse(Request.Params("TipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador"), out int codigoTipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador);

                int.TryParse(Request.Params("TipoMovimentoValorLiquidoEmissao"), out int codigoTipoMovimentoValorLiquidoEmissao);
                int.TryParse(Request.Params("TipoMovimentoValorLiquidoCancelamento"), out int codigoTipoMovimentoValorLiquidoCancelamento);
                int.TryParse(Request.Params("TipoMovimentoValorLiquidoAnulacao"), out int codigoTipoMovimentoValorLiquidoAnulacao);
                int.TryParse(Request.Params("TipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador"), out int codigoTipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador);

                int.TryParse(Request.Params("TipoMovimentoPISEmissao"), out int codigoTipoMovimentoPISEmissao);
                int.TryParse(Request.Params("TipoMovimentoPISCancelamento"), out int codigoTipoMovimentoPISCancelamento);
                int.TryParse(Request.Params("TipoMovimentoPISAnulacao"), out int codigoTipoMovimentoPISAnulacao);
                int.TryParse(Request.Params("TipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador"), out int codigoTipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador);

                int.TryParse(Request.Params("TipoMovimentoCOFINSEmissao"), out int codigoTipoMovimentoCOFINSEmissao);
                int.TryParse(Request.Params("TipoMovimentoCOFINSCancelamento"), out int codigoTipoMovimentoCOFINSCancelamento);
                int.TryParse(Request.Params("TipoMovimentoCOFINSAnulacao"), out int codigoTipoMovimentoCOFINSAnulacao);
                int.TryParse(Request.Params("TipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador"), out int codigoTipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador);

                int.TryParse(Request.Params("TipoMovimentoIREmissao"), out int codigoTipoMovimentoIREmissao);
                int.TryParse(Request.Params("TipoMovimentoIRCancelamento"), out int codigoTipoMovimentoIRCancelamento);
                int.TryParse(Request.Params("TipoMovimentoIRAnulacao"), out int codigoTipoMovimentoIRAnulacao);
                int.TryParse(Request.Params("TipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador"), out int codigoTipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador);

                int.TryParse(Request.Params("TipoMovimentoCSLLEmissao"), out int codigoTipoMovimentoCSLLEmissao);
                int.TryParse(Request.Params("TipoMovimentoCSLLCancelamento"), out int codigoTipoMovimentoCSLLCancelamento);
                int.TryParse(Request.Params("TipoMovimentoCSLLAnulacao"), out int codigoTipoMovimentoCSLLAnulacao);
                int.TryParse(Request.Params("TipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador"), out int codigoTipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador);

                int.TryParse(Request.Params("TipoMovimentoBaseSTRetidoEmissao"), out int codigoTipoMovimentoBaseSTRetidoEmissao);
                int.TryParse(Request.Params("TipoMovimentoBaseSTRetidoReversao"), out int codigoTipoMovimentoBaseSTRetidoReversao);
                int.TryParse(Request.Params("TipoMovimentoValorSTRetidoEmissao"), out int codigoTipoMovimentoValorSTRetidoEmissao);
                int.TryParse(Request.Params("TipoMovimentoValorSTRetidoReversao"), out int codigoTipoMovimentoValorSTRetidoReversao);

                Enum.TryParse(Request.Params("TipoDocumentoCreditoDebito"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito tipoDocumentoCreditoDebito);
                Enum.TryParse(Request.Params("ModeloImpressao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoModeloImpressao tipoDocumentoModeloImpressao);

                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

                Dominio.Entidades.ModeloDocumentoFiscal modeloExistente = repModeloDocumentoFiscal.BuscarPorModelo(numero);

                if (modeloExistente != null)
                    return new JsonpResult(false, true, "Já existe um modelo de documento cadastrado com o número " + numero);

                unitOfWork.Start();

                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = new Dominio.Entidades.ModeloDocumentoFiscal();
                modeloDocumentoFiscal.Codigo = repModeloDocumentoFiscal.BuscarProximoCodigo();
                modeloDocumentoFiscal.Status = ativo ? "A" : "I";
                modeloDocumentoFiscal.Descricao = descricao;
                modeloDocumentoFiscal.Abreviacao = abreviacao;
                modeloDocumentoFiscal.Especie = Request.GetStringParam("Especie");
                modeloDocumentoFiscal.Data = DateTime.Now;
                modeloDocumentoFiscal.UtilizarNumeracaoCTe = utilizarNumeracaoCTe;
                modeloDocumentoFiscal.UtilizarNumeracaoNFe = Request.GetBoolParam("UtilizarNumeracaoNFe");
                modeloDocumentoFiscal.Editavel = true;
                modeloDocumentoFiscal.Numero = numero;
                modeloDocumentoFiscal.TipoDocumentoEmissao = Dominio.Enumeradores.TipoDocumento.Outros;
                modeloDocumentoFiscal.TipoDocumentoCreditoDebito = tipoDocumentoCreditoDebito;

                modeloDocumentoFiscal.NaoGerarFaturamento = naoGerarFaturamento;
                modeloDocumentoFiscal.GerarISSAutomaticamente = gerarISSAutomaticamente;
                modeloDocumentoFiscal.NaoGerarEscrituracao = naoGerarEscrituracao;

                modeloDocumentoFiscal.TipoMovimentoAnulacao = codigoTipoMovimentoAnulacao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoAnulacao) : null;
                modeloDocumentoFiscal.TipoMovimentoAnulacaoNotaAnulacaoEmbarcador = codigoTipoMovimentoAnulacaoComNotaAnulacaoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoAnulacaoComNotaAnulacaoEmbarcador) : null;
                modeloDocumentoFiscal.TipoMovimentoCancelamento = codigoTipoMovimentoCancelamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCancelamento) : null;
                modeloDocumentoFiscal.TipoMovimentoEmissao = codigoTipoMovimentoEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoEmissao) : null;
                modeloDocumentoFiscal.TipoMovimentoImpostoAnulacao = codigoTipoMovimentoImpostoAnulacao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoImpostoAnulacao) : null;
                modeloDocumentoFiscal.TipoMovimentoImpostoAnulacaoNotaAnulacaoEmbarcador = codigoTipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador) : null;
                modeloDocumentoFiscal.TipoMovimentoImpostoCancelamento = codigoTipoMovimentoImpostoCancelamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoImpostoCancelamento) : null;
                modeloDocumentoFiscal.TipoMovimentoImpostoEmissao = codigoTipoMovimentoImpostoEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoImpostoEmissao) : null;
                modeloDocumentoFiscal.TipoMovimentoUsoEntrada = codigoTipoMovimentoUsoEntrada > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoEntrada) : null;
                modeloDocumentoFiscal.TipoMovimentoReversaoEntrada = codigoTipoMovimentoReversaoEntrada > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoEntrada) : null;
                modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacao = codigoTipoMovimentoValorLiquidoAnulacao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorLiquidoAnulacao) : null;
                modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacaoNotaAnulacaoEmbarcador = codigoTipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador) : null;
                modeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamento = codigoTipoMovimentoValorLiquidoCancelamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorLiquidoCancelamento) : null;
                modeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissao = codigoTipoMovimentoValorLiquidoEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorLiquidoEmissao) : null;

                modeloDocumentoFiscal.TipoMovimentoPISAnulacao = codigoTipoMovimentoPISAnulacao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoPISAnulacao) : null;
                modeloDocumentoFiscal.TipoMovimentoPISAnulacaoNotaAnulacaoEmbarcador = codigoTipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador) : null;
                modeloDocumentoFiscal.TipoMovimentoPISCancelamento = codigoTipoMovimentoPISCancelamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoPISCancelamento) : null;
                modeloDocumentoFiscal.TipoMovimentoPISEmissao = codigoTipoMovimentoPISEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoPISEmissao) : null;

                modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacao = codigoTipoMovimentoCOFINSAnulacao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCOFINSAnulacao) : null;
                modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacaoNotaAnulacaoEmbarcador = codigoTipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador) : null;
                modeloDocumentoFiscal.TipoMovimentoCOFINSCancelamento = codigoTipoMovimentoCOFINSCancelamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCOFINSCancelamento) : null;
                modeloDocumentoFiscal.TipoMovimentoCOFINSEmissao = codigoTipoMovimentoCOFINSEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCOFINSEmissao) : null;

                modeloDocumentoFiscal.TipoMovimentoIRAnulacao = codigoTipoMovimentoIRAnulacao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoIRAnulacao) : null;
                modeloDocumentoFiscal.TipoMovimentoIRAnulacaoNotaAnulacaoEmbarcador = codigoTipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador) : null;
                modeloDocumentoFiscal.TipoMovimentoIRCancelamento = codigoTipoMovimentoIRCancelamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoIRCancelamento) : null;
                modeloDocumentoFiscal.TipoMovimentoIREmissao = codigoTipoMovimentoIREmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoIREmissao) : null;

                modeloDocumentoFiscal.TipoMovimentoCSLLAnulacao = codigoTipoMovimentoCSLLAnulacao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCSLLAnulacao) : null;
                modeloDocumentoFiscal.TipoMovimentoCSLLAnulacaoNotaAnulacaoEmbarcador = codigoTipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador) : null;
                modeloDocumentoFiscal.TipoMovimentoCSLLCancelamento = codigoTipoMovimentoCSLLCancelamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCSLLCancelamento) : null;
                modeloDocumentoFiscal.TipoMovimentoCSLLEmissao = codigoTipoMovimentoCSLLEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCSLLEmissao) : null;

                modeloDocumentoFiscal.GerarMovimentoAutomaticoEntrada = gerarMovimentoAutomaticoEntrada;
                modeloDocumentoFiscal.GerarMovimentoAutomatico = gerarMovimentoAutomatico;
                modeloDocumentoFiscal.AverbarDocumento = averbarDocumento;
                modeloDocumentoFiscal.DiferenciarMovimentosParaImpostos = diferenciarMovimentosParaImpostos;
                modeloDocumentoFiscal.DiferenciarMovimentosParaValorLiquido = diferenciarMovimentosParaValorLiquido;
                modeloDocumentoFiscal.DiferenciarMovimentosParaPIS = diferenciarMovimentosParaPIS;
                modeloDocumentoFiscal.DiferenciarMovimentosParaCOFINS = diferenciarMovimentosParaCOFINS;
                modeloDocumentoFiscal.DiferenciarMovimentosParaIR = diferenciarMovimentosParaIR;
                modeloDocumentoFiscal.DiferenciarMovimentosParaCSLL = diferenciarMovimentosParaCSLL;

                modeloDocumentoFiscal.GerarMovimentoBaseSTRetido = gerarMovimentoBaseSTRetido;
                modeloDocumentoFiscal.GerarMovimentoValorSTRetido = gerarMovimentoValorSTRetido;

                modeloDocumentoFiscal.TipoMovimentoBaseSTRetidoEmissao = codigoTipoMovimentoBaseSTRetidoEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoBaseSTRetidoEmissao) : null;
                modeloDocumentoFiscal.TipoMovimentoBaseSTRetidoReversao = codigoTipoMovimentoBaseSTRetidoReversao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoBaseSTRetidoReversao) : null;
                modeloDocumentoFiscal.TipoMovimentoValorSTRetidoEmissao = codigoTipoMovimentoValorSTRetidoEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorSTRetidoEmissao) : null;
                modeloDocumentoFiscal.TipoMovimentoValorSTRetidoReversao = codigoTipoMovimentoValorSTRetidoReversao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorSTRetidoReversao) : null;

                modeloDocumentoFiscal.DocumentoComMoedaEstrangeira = Request.GetBoolParam("DocumentoComMoedaEstrangeira");
                modeloDocumentoFiscal.MoedaCotacaoBancoCentral = Request.GetEnumParam("MoedaCotacaoBancoCentral", Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.DolarVenda);
                modeloDocumentoFiscal.CalcularImpostos = Request.GetBoolParam("CalcularImpostos");
                modeloDocumentoFiscal.DocumentoTipoCRT = Request.GetBoolParam("DocumentoTipoCRT");
                modeloDocumentoFiscal.DescontarValorDesseDocumentoFatura = Request.GetBoolParam("DescontarValorDesseDocumentoFatura");

                if (gerarMovimentoAutomatico)
                {
                    if (modeloDocumentoFiscal.TipoMovimentoAnulacao == null || modeloDocumentoFiscal.TipoMovimentoCancelamento == null || modeloDocumentoFiscal.TipoMovimentoEmissao == null || (modeloDocumentoFiscal.Numero == "57" && modeloDocumentoFiscal.TipoMovimentoAnulacaoNotaAnulacaoEmbarcador == null))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para gerar os movimentos automaticamente.");
                    }

                    if (diferenciarMovimentosParaImpostos)
                    {
                        if (modeloDocumentoFiscal.TipoMovimentoImpostoAnulacao == null || modeloDocumentoFiscal.TipoMovimentoImpostoCancelamento == null || modeloDocumentoFiscal.TipoMovimentoImpostoEmissao == null || (modeloDocumentoFiscal.Numero == "57" && modeloDocumentoFiscal.TipoMovimentoImpostoAnulacaoNotaAnulacaoEmbarcador == null))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para diferenciar os movimentos para os impostos.");
                        }
                    }

                    if (diferenciarMovimentosParaValorLiquido)
                    {
                        if (modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacao == null || modeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissao == null || modeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamento == null || (modeloDocumentoFiscal.Numero == "57" && modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacaoNotaAnulacaoEmbarcador == null))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para diferenciar os movimentos para os impostos.");
                        }
                    }

                    if (diferenciarMovimentosParaPIS)
                    {
                        if (modeloDocumentoFiscal.TipoMovimentoPISAnulacao == null || modeloDocumentoFiscal.TipoMovimentoPISCancelamento == null || modeloDocumentoFiscal.TipoMovimentoPISEmissao == null || (modeloDocumentoFiscal.Numero == "57" && modeloDocumentoFiscal.TipoMovimentoPISAnulacaoNotaAnulacaoEmbarcador == null))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para diferenciar os movimentos para o PIS.");
                        }
                    }

                    if (diferenciarMovimentosParaCOFINS)
                    {
                        if (modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacao == null || modeloDocumentoFiscal.TipoMovimentoCOFINSCancelamento == null || modeloDocumentoFiscal.TipoMovimentoCOFINSEmissao == null || (modeloDocumentoFiscal.Numero == "57" && modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacaoNotaAnulacaoEmbarcador == null))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para diferenciar os movimentos para a COFINS.");
                        }
                    }

                    if (diferenciarMovimentosParaIR)
                    {
                        if (modeloDocumentoFiscal.TipoMovimentoIRAnulacao == null || modeloDocumentoFiscal.TipoMovimentoIRCancelamento == null || modeloDocumentoFiscal.TipoMovimentoIREmissao == null || (modeloDocumentoFiscal.Numero == "57" && modeloDocumentoFiscal.TipoMovimentoIRAnulacaoNotaAnulacaoEmbarcador == null))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para diferenciar os movimentos para o IR.");
                        }
                    }

                    if (diferenciarMovimentosParaCSLL)
                    {
                        if (modeloDocumentoFiscal.TipoMovimentoCSLLAnulacao == null || modeloDocumentoFiscal.TipoMovimentoCSLLCancelamento == null || modeloDocumentoFiscal.TipoMovimentoCSLLEmissao == null || (modeloDocumentoFiscal.Numero == "57" && modeloDocumentoFiscal.TipoMovimentoCSLLAnulacaoNotaAnulacaoEmbarcador == null))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para diferenciar os movimentos para a CSLL.");
                        }
                    }
                }

                if (gerarMovimentoAutomaticoEntrada)
                {
                    if (modeloDocumentoFiscal.TipoMovimentoUsoEntrada == null || modeloDocumentoFiscal.TipoMovimentoReversaoEntrada == null)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para gerar os movimentos de entrada automaticamente.");
                    }
                }

                if (gerarMovimentoBaseSTRetido)
                {
                    if (modeloDocumentoFiscal.TipoMovimentoBaseSTRetidoEmissao == null || modeloDocumentoFiscal.TipoMovimentoBaseSTRetidoReversao == null)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para gerar os movimentos de Base de ST retido.");
                    }
                }

                if (gerarMovimentoValorSTRetido)
                {
                    if (modeloDocumentoFiscal.TipoMovimentoValorSTRetidoEmissao == null || modeloDocumentoFiscal.TipoMovimentoValorSTRetidoReversao == null)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para gerar os movimentos de Valor de ST retido.");
                    }
                }

                repModeloDocumentoFiscal.Inserir(modeloDocumentoFiscal, Auditado);

                SalvarListaSeries(ref modeloDocumentoFiscal, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool.TryParse(Request.Params("Ativo"), out bool ativo);
                bool.TryParse(Request.Params("UtilizarNumeracaoCTe"), out bool utilizarNumeracaoCTe);
                bool.TryParse(Request.Params("GerarMovimentoAutomatico"), out bool gerarMovimentoAutomatico);
                bool.TryParse(Request.Params("DiferenciarMovimentosParaImpostos"), out bool diferenciarMovimentosParaImpostos);
                bool.TryParse(Request.Params("DiferenciarMovimentosParaPIS"), out bool diferenciarMovimentosParaPIS);
                bool.TryParse(Request.Params("DiferenciarMovimentosParaCOFINS"), out bool diferenciarMovimentosParaCOFINS);
                bool.TryParse(Request.Params("DiferenciarMovimentosParaIR"), out bool diferenciarMovimentosParaIR);
                bool.TryParse(Request.Params("DiferenciarMovimentosParaCSLL"), out bool diferenciarMovimentosParaCSLL);
                bool.TryParse(Request.Params("GerarMovimentoAutomaticoEntrada"), out bool gerarMovimentoAutomaticoEntrada);
                bool.TryParse(Request.Params("AverbarDocumento"), out bool averbarDocumento);
                bool.TryParse(Request.Params("DiferenciarMovimentosParaValorLiquido"), out bool diferenciarMovimentosParaValorLiquido);
                bool.TryParse(Request.Params("NaoGerarFaturamento"), out bool naoGerarFaturamento);
                bool.TryParse(Request.Params("GerarISSAutomaticamente"), out bool gerarISSAutomaticamente);

                bool.TryParse(Request.Params("NaoGerarEscrituracao"), out bool naoGerarEscrituracao);

                bool.TryParse(Request.Params("GerarMovimentoBaseSTRetido"), out bool gerarMovimentoBaseSTRetido);
                bool.TryParse(Request.Params("GerarMovimentoValorSTRetido"), out bool gerarMovimentoValorSTRetido);

                string descricao = Request.Params("Descricao");
                string abreviacao = Request.Params("Abreviacao");
                string numero = Request.Params("Numero");

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("TipoMovimentoUsoEntrada"), out int codigoTipoMovimentoUsoEntrada);
                int.TryParse(Request.Params("TipoMovimentoReversaoEntrada"), out int codigoTipoMovimentoReversaoEntrada);
                int.TryParse(Request.Params("TipoMovimentoEmissao"), out int codigoTipoMovimentoEmissao);
                int.TryParse(Request.Params("TipoMovimentoCancelamento"), out int codigoTipoMovimentoCancelamento);
                int.TryParse(Request.Params("TipoMovimentoAnulacao"), out int codigoTipoMovimentoAnulacao);
                int.TryParse(Request.Params("TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador"), out int codigoTipoMovimentoAnulacaoComNotaAnulacaoEmbarcador);

                int.TryParse(Request.Params("TipoMovimentoImpostoEmissao"), out int codigoTipoMovimentoImpostoEmissao);
                int.TryParse(Request.Params("TipoMovimentoImpostoCancelamento"), out int codigoTipoMovimentoImpostoCancelamento);
                int.TryParse(Request.Params("TipoMovimentoImpostoAnulacao"), out int codigoTipoMovimentoImpostoAnulacao);
                int.TryParse(Request.Params("TipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador"), out int codigoTipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador);

                int.TryParse(Request.Params("TipoMovimentoValorLiquidoEmissao"), out int codigoTipoMovimentoValorLiquidoEmissao);
                int.TryParse(Request.Params("TipoMovimentoValorLiquidoCancelamento"), out int codigoTipoMovimentoValorLiquidoCancelamento);
                int.TryParse(Request.Params("TipoMovimentoValorLiquidoAnulacao"), out int codigoTipoMovimentoValorLiquidoAnulacao);
                int.TryParse(Request.Params("TipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador"), out int codigoTipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador);

                int.TryParse(Request.Params("TipoMovimentoPISEmissao"), out int codigoTipoMovimentoPISEmissao);
                int.TryParse(Request.Params("TipoMovimentoPISCancelamento"), out int codigoTipoMovimentoPISCancelamento);
                int.TryParse(Request.Params("TipoMovimentoPISAnulacao"), out int codigoTipoMovimentoPISAnulacao);
                int.TryParse(Request.Params("TipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador"), out int codigoTipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador);

                int.TryParse(Request.Params("TipoMovimentoCOFINSEmissao"), out int codigoTipoMovimentoCOFINSEmissao);
                int.TryParse(Request.Params("TipoMovimentoCOFINSCancelamento"), out int codigoTipoMovimentoCOFINSCancelamento);
                int.TryParse(Request.Params("TipoMovimentoCOFINSAnulacao"), out int codigoTipoMovimentoCOFINSAnulacao);
                int.TryParse(Request.Params("TipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador"), out int codigoTipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador);

                int.TryParse(Request.Params("TipoMovimentoIREmissao"), out int codigoTipoMovimentoIREmissao);
                int.TryParse(Request.Params("TipoMovimentoIRCancelamento"), out int codigoTipoMovimentoIRCancelamento);
                int.TryParse(Request.Params("TipoMovimentoIRAnulacao"), out int codigoTipoMovimentoIRAnulacao);
                int.TryParse(Request.Params("TipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador"), out int codigoTipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador);

                int.TryParse(Request.Params("TipoMovimentoCSLLEmissao"), out int codigoTipoMovimentoCSLLEmissao);
                int.TryParse(Request.Params("TipoMovimentoCSLLCancelamento"), out int codigoTipoMovimentoCSLLCancelamento);
                int.TryParse(Request.Params("TipoMovimentoCSLLAnulacao"), out int codigoTipoMovimentoCSLLAnulacao);
                int.TryParse(Request.Params("TipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador"), out int codigoTipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador);

                int.TryParse(Request.Params("TipoMovimentoBaseSTRetidoEmissao"), out int codigoTipoMovimentoBaseSTRetidoEmissao);
                int.TryParse(Request.Params("TipoMovimentoBaseSTRetidoReversao"), out int codigoTipoMovimentoBaseSTRetidoReversao);
                int.TryParse(Request.Params("TipoMovimentoValorSTRetidoEmissao"), out int codigoTipoMovimentoValorSTRetidoEmissao);
                int.TryParse(Request.Params("TipoMovimentoValorSTRetidoReversao"), out int codigoTipoMovimentoValorSTRetidoReversao);

                Enum.TryParse(Request.Params("TipoDocumentoCreditoDebito"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito tipoDocumentoCreditoDebito);
                Enum.TryParse(Request.Params("ModeloImpressao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoModeloImpressao tipoDocumentoModeloImpressao);

                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(codigo);
                modeloDocumentoFiscal.Initialize();
                Dominio.Entidades.ModeloDocumentoFiscal modeloExistente = repModeloDocumentoFiscal.BuscarPorModelo(modeloDocumentoFiscal.Numero);

                if (modeloExistente != null && modeloExistente.Codigo != codigo)
                    return new JsonpResult(false, true, "Já existe um modelo de documento cadastrado com o número " + modeloExistente.Numero);

                unitOfWork.Start();

                if (modeloDocumentoFiscal.Editavel)
                {
                    modeloDocumentoFiscal.UtilizarNumeracaoCTe = utilizarNumeracaoCTe;
                    modeloDocumentoFiscal.UtilizarNumeracaoNFe = Request.GetBoolParam("UtilizarNumeracaoNFe");
                    modeloDocumentoFiscal.Status = ativo ? "A" : "I";
                    modeloDocumentoFiscal.Descricao = descricao;
                    modeloDocumentoFiscal.Abreviacao = abreviacao;
                    modeloDocumentoFiscal.Data = DateTime.Now;
                    modeloDocumentoFiscal.Numero = numero;
                }

                modeloDocumentoFiscal.Especie = Request.GetStringParam("Especie");
                modeloDocumentoFiscal.NaoGerarFaturamento = naoGerarFaturamento;
                modeloDocumentoFiscal.GerarISSAutomaticamente = gerarISSAutomaticamente;
                modeloDocumentoFiscal.NaoGerarEscrituracao = naoGerarEscrituracao;
                modeloDocumentoFiscal.TipoDocumentoCreditoDebito = tipoDocumentoCreditoDebito;

                modeloDocumentoFiscal.TipoMovimentoAnulacao = codigoTipoMovimentoAnulacao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoAnulacao) : null;
                modeloDocumentoFiscal.TipoMovimentoAnulacaoNotaAnulacaoEmbarcador = codigoTipoMovimentoAnulacaoComNotaAnulacaoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoAnulacaoComNotaAnulacaoEmbarcador) : null;
                modeloDocumentoFiscal.TipoMovimentoCancelamento = codigoTipoMovimentoCancelamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCancelamento) : null;
                modeloDocumentoFiscal.TipoMovimentoEmissao = codigoTipoMovimentoEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoEmissao) : null;
                modeloDocumentoFiscal.TipoMovimentoImpostoAnulacao = codigoTipoMovimentoImpostoAnulacao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoImpostoAnulacao) : null;
                modeloDocumentoFiscal.TipoMovimentoImpostoAnulacaoNotaAnulacaoEmbarcador = codigoTipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador) : null;
                modeloDocumentoFiscal.TipoMovimentoImpostoCancelamento = codigoTipoMovimentoImpostoCancelamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoImpostoCancelamento) : null;
                modeloDocumentoFiscal.TipoMovimentoImpostoEmissao = codigoTipoMovimentoImpostoEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoImpostoEmissao) : null;
                modeloDocumentoFiscal.TipoMovimentoUsoEntrada = codigoTipoMovimentoUsoEntrada > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoUsoEntrada) : null;
                modeloDocumentoFiscal.TipoMovimentoReversaoEntrada = codigoTipoMovimentoReversaoEntrada > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversaoEntrada) : null;
                modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacao = codigoTipoMovimentoValorLiquidoAnulacao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorLiquidoAnulacao) : null;
                modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacaoNotaAnulacaoEmbarcador = codigoTipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador) : null;
                modeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamento = codigoTipoMovimentoValorLiquidoCancelamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorLiquidoCancelamento) : null;
                modeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissao = codigoTipoMovimentoValorLiquidoEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorLiquidoEmissao) : null;

                modeloDocumentoFiscal.TipoMovimentoPISAnulacao = codigoTipoMovimentoPISAnulacao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoPISAnulacao) : null;
                modeloDocumentoFiscal.TipoMovimentoPISAnulacaoNotaAnulacaoEmbarcador = codigoTipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador) : null;
                modeloDocumentoFiscal.TipoMovimentoPISCancelamento = codigoTipoMovimentoPISCancelamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoPISCancelamento) : null;
                modeloDocumentoFiscal.TipoMovimentoPISEmissao = codigoTipoMovimentoPISEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoPISEmissao) : null;

                modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacao = codigoTipoMovimentoCOFINSAnulacao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCOFINSAnulacao) : null;
                modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacaoNotaAnulacaoEmbarcador = codigoTipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador) : null;
                modeloDocumentoFiscal.TipoMovimentoCOFINSCancelamento = codigoTipoMovimentoCOFINSCancelamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCOFINSCancelamento) : null;
                modeloDocumentoFiscal.TipoMovimentoCOFINSEmissao = codigoTipoMovimentoCOFINSEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCOFINSEmissao) : null;

                modeloDocumentoFiscal.TipoMovimentoIRAnulacao = codigoTipoMovimentoIRAnulacao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoIRAnulacao) : null;
                modeloDocumentoFiscal.TipoMovimentoIRAnulacaoNotaAnulacaoEmbarcador = codigoTipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador) : null;
                modeloDocumentoFiscal.TipoMovimentoIRCancelamento = codigoTipoMovimentoIRCancelamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoIRCancelamento) : null;
                modeloDocumentoFiscal.TipoMovimentoIREmissao = codigoTipoMovimentoIREmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoIREmissao) : null;

                modeloDocumentoFiscal.TipoMovimentoCSLLAnulacao = codigoTipoMovimentoCSLLAnulacao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCSLLAnulacao) : null;
                modeloDocumentoFiscal.TipoMovimentoCSLLAnulacaoNotaAnulacaoEmbarcador = codigoTipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador) : null;
                modeloDocumentoFiscal.TipoMovimentoCSLLCancelamento = codigoTipoMovimentoCSLLCancelamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCSLLCancelamento) : null;
                modeloDocumentoFiscal.TipoMovimentoCSLLEmissao = codigoTipoMovimentoCSLLEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCSLLEmissao) : null;

                modeloDocumentoFiscal.GerarMovimentoAutomaticoEntrada = gerarMovimentoAutomaticoEntrada;
                modeloDocumentoFiscal.GerarMovimentoAutomatico = gerarMovimentoAutomatico;
                modeloDocumentoFiscal.AverbarDocumento = averbarDocumento;
                modeloDocumentoFiscal.DiferenciarMovimentosParaImpostos = diferenciarMovimentosParaImpostos;
                modeloDocumentoFiscal.DiferenciarMovimentosParaValorLiquido = diferenciarMovimentosParaValorLiquido;
                modeloDocumentoFiscal.DiferenciarMovimentosParaPIS = diferenciarMovimentosParaPIS;
                modeloDocumentoFiscal.DiferenciarMovimentosParaCOFINS = diferenciarMovimentosParaCOFINS;
                modeloDocumentoFiscal.DiferenciarMovimentosParaIR = diferenciarMovimentosParaIR;
                modeloDocumentoFiscal.DiferenciarMovimentosParaCSLL = diferenciarMovimentosParaCSLL;

                modeloDocumentoFiscal.GerarMovimentoBaseSTRetido = gerarMovimentoBaseSTRetido;
                modeloDocumentoFiscal.GerarMovimentoValorSTRetido = gerarMovimentoValorSTRetido;

                modeloDocumentoFiscal.TipoMovimentoBaseSTRetidoEmissao = codigoTipoMovimentoBaseSTRetidoEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoBaseSTRetidoEmissao) : null;
                modeloDocumentoFiscal.TipoMovimentoBaseSTRetidoReversao = codigoTipoMovimentoBaseSTRetidoReversao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoBaseSTRetidoReversao) : null;
                modeloDocumentoFiscal.TipoMovimentoValorSTRetidoEmissao = codigoTipoMovimentoValorSTRetidoEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorSTRetidoEmissao) : null;
                modeloDocumentoFiscal.TipoMovimentoValorSTRetidoReversao = codigoTipoMovimentoValorSTRetidoReversao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoValorSTRetidoReversao) : null;

                modeloDocumentoFiscal.DocumentoComMoedaEstrangeira = Request.GetBoolParam("DocumentoComMoedaEstrangeira");
                modeloDocumentoFiscal.MoedaCotacaoBancoCentral = Request.GetEnumParam("MoedaCotacaoBancoCentral", Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral.DolarVenda);
                modeloDocumentoFiscal.CalcularImpostos = Request.GetBoolParam("CalcularImpostos");
                modeloDocumentoFiscal.DocumentoTipoCRT = Request.GetBoolParam("DocumentoTipoCRT");
                modeloDocumentoFiscal.DescontarValorDesseDocumentoFatura = Request.GetBoolParam("DescontarValorDesseDocumentoFatura");

                if (gerarMovimentoAutomatico)
                {
                    if (modeloDocumentoFiscal.TipoMovimentoAnulacao == null || modeloDocumentoFiscal.TipoMovimentoCancelamento == null || modeloDocumentoFiscal.TipoMovimentoEmissao == null || (modeloDocumentoFiscal.Numero == "57" && modeloDocumentoFiscal.TipoMovimentoAnulacaoNotaAnulacaoEmbarcador == null))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para gerar os movimentos automaticamente.");
                    }

                    if (diferenciarMovimentosParaImpostos)
                    {
                        if (modeloDocumentoFiscal.TipoMovimentoImpostoAnulacao == null || modeloDocumentoFiscal.TipoMovimentoImpostoCancelamento == null || modeloDocumentoFiscal.TipoMovimentoImpostoEmissao == null || (modeloDocumentoFiscal.Numero == "57" && modeloDocumentoFiscal.TipoMovimentoImpostoAnulacaoNotaAnulacaoEmbarcador == null))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para diferenciar os movimentos para os impostos.");
                        }
                    }

                    if (diferenciarMovimentosParaValorLiquido)
                    {
                        if (modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacao == null || modeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissao == null || modeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamento == null || (modeloDocumentoFiscal.Numero == "57" && modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacaoNotaAnulacaoEmbarcador == null))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para diferenciar os movimentos para os impostos.");
                        }
                    }

                    if (diferenciarMovimentosParaPIS)
                    {
                        if (modeloDocumentoFiscal.TipoMovimentoPISAnulacao == null || modeloDocumentoFiscal.TipoMovimentoPISCancelamento == null || modeloDocumentoFiscal.TipoMovimentoPISEmissao == null || (modeloDocumentoFiscal.Numero == "57" && modeloDocumentoFiscal.TipoMovimentoPISAnulacaoNotaAnulacaoEmbarcador == null))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para diferenciar os movimentos para o PIS.");
                        }
                    }

                    if (diferenciarMovimentosParaCOFINS)
                    {
                        if (modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacao == null || modeloDocumentoFiscal.TipoMovimentoCOFINSCancelamento == null || modeloDocumentoFiscal.TipoMovimentoCOFINSEmissao == null || (modeloDocumentoFiscal.Numero == "57" && modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacaoNotaAnulacaoEmbarcador == null))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para diferenciar os movimentos para a COFINS.");
                        }
                    }

                    if (diferenciarMovimentosParaIR)
                    {
                        if (modeloDocumentoFiscal.TipoMovimentoIRAnulacao == null || modeloDocumentoFiscal.TipoMovimentoIRCancelamento == null || modeloDocumentoFiscal.TipoMovimentoIREmissao == null || (modeloDocumentoFiscal.Numero == "57" && modeloDocumentoFiscal.TipoMovimentoIRAnulacaoNotaAnulacaoEmbarcador == null))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para diferenciar os movimentos para o IR.");
                        }
                    }

                    if (diferenciarMovimentosParaCSLL)
                    {
                        if (modeloDocumentoFiscal.TipoMovimentoCSLLAnulacao == null || modeloDocumentoFiscal.TipoMovimentoCSLLCancelamento == null || modeloDocumentoFiscal.TipoMovimentoCSLLEmissao == null || (modeloDocumentoFiscal.Numero == "57" && modeloDocumentoFiscal.TipoMovimentoCSLLAnulacaoNotaAnulacaoEmbarcador == null))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para diferenciar os movimentos para a CSLL.");
                        }
                    }
                }

                if (gerarMovimentoAutomaticoEntrada)
                {
                    if (modeloDocumentoFiscal.TipoMovimentoUsoEntrada == null || modeloDocumentoFiscal.TipoMovimentoReversaoEntrada == null)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para gerar os movimentos de entrada automaticamente.");
                    }
                }

                if (gerarMovimentoBaseSTRetido)
                {
                    if (modeloDocumentoFiscal.TipoMovimentoBaseSTRetidoEmissao == null || modeloDocumentoFiscal.TipoMovimentoBaseSTRetidoReversao == null)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para gerar os movimentos de Base de ST retido.");
                    }
                }

                if (gerarMovimentoValorSTRetido)
                {
                    if (modeloDocumentoFiscal.TipoMovimentoValorSTRetidoEmissao == null || modeloDocumentoFiscal.TipoMovimentoValorSTRetidoReversao == null)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para gerar os movimentos de Valor de ST retido.");
                    }
                }

                repModeloDocumentoFiscal.Atualizar(modeloDocumentoFiscal, Auditado);
                SalvarListaSeries(ref modeloDocumentoFiscal, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(int.Parse(Request.Params("Codigo")));
                var dynModeloDocumentoFiscal = new
                {
                    modeloDocumentoFiscal.Codigo,
                    modeloDocumentoFiscal.Abreviacao,
                    modeloDocumentoFiscal.Especie,
                    modeloDocumentoFiscal.Descricao,
                    modeloDocumentoFiscal.Numero,
                    modeloDocumentoFiscal.Editavel,
                    Ativo = modeloDocumentoFiscal.Status == "A" ? true : false,
                    modeloDocumentoFiscal.GerarMovimentoAutomatico,
                    modeloDocumentoFiscal.DiferenciarMovimentosParaImpostos,
                    modeloDocumentoFiscal.DiferenciarMovimentosParaPIS,
                    modeloDocumentoFiscal.DiferenciarMovimentosParaCOFINS,
                    modeloDocumentoFiscal.DiferenciarMovimentosParaIR,
                    modeloDocumentoFiscal.DiferenciarMovimentosParaCSLL,
                    modeloDocumentoFiscal.GerarMovimentoAutomaticoEntrada,
                    modeloDocumentoFiscal.AverbarDocumento,
                    modeloDocumentoFiscal.UtilizarNumeracaoCTe,
                    modeloDocumentoFiscal.UtilizarNumeracaoNFe,
                    modeloDocumentoFiscal.DiferenciarMovimentosParaValorLiquido,
                    modeloDocumentoFiscal.NaoGerarFaturamento,
                    modeloDocumentoFiscal.GerarISSAutomaticamente,
                    modeloDocumentoFiscal.NaoGerarEscrituracao,
                    modeloDocumentoFiscal.TipoDocumentoCreditoDebito,
                    modeloDocumentoFiscal.DocumentoComMoedaEstrangeira,
                    modeloDocumentoFiscal.MoedaCotacaoBancoCentral,
                    modeloDocumentoFiscal.CalcularImpostos,
                    modeloDocumentoFiscal.DocumentoTipoCRT,
                    modeloDocumentoFiscal.DescontarValorDesseDocumentoFatura,
                    TipoMovimentoAnulacao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoAnulacao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoAnulacao?.Codigo ?? 0
                    },
                    TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoAnulacaoNotaAnulacaoEmbarcador?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoAnulacaoNotaAnulacaoEmbarcador?.Codigo ?? 0
                    },
                    TipoMovimentoCancelamento = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoCancelamento?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoCancelamento?.Codigo ?? 0
                    },
                    TipoMovimentoEmissao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoEmissao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoEmissao?.Codigo ?? 0
                    },
                    TipoMovimentoImpostoAnulacao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoImpostoAnulacao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoImpostoAnulacao?.Codigo ?? 0
                    },
                    TipoMovimentoImpostoAnulacaoComNotaAnulacaoEmbarcador = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoImpostoAnulacaoNotaAnulacaoEmbarcador?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoImpostoAnulacaoNotaAnulacaoEmbarcador?.Codigo ?? 0
                    },
                    TipoMovimentoImpostoCancelamento = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoImpostoCancelamento?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoImpostoCancelamento?.Codigo ?? 0
                    },
                    TipoMovimentoImpostoEmissao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoImpostoEmissao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoImpostoEmissao?.Codigo ?? 0
                    },
                    TipoMovimentoPISAnulacao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoPISAnulacao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoPISAnulacao?.Codigo ?? 0
                    },
                    TipoMovimentoPISAnulacaoComNotaAnulacaoEmbarcador = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoPISAnulacaoNotaAnulacaoEmbarcador?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoPISAnulacaoNotaAnulacaoEmbarcador?.Codigo ?? 0
                    },
                    TipoMovimentoPISCancelamento = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoPISCancelamento?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoPISCancelamento?.Codigo ?? 0
                    },
                    TipoMovimentoPISEmissao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoPISEmissao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoPISEmissao?.Codigo ?? 0
                    },
                    TipoMovimentoCOFINSAnulacao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacao?.Codigo ?? 0
                    },
                    TipoMovimentoCOFINSAnulacaoComNotaAnulacaoEmbarcador = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacaoNotaAnulacaoEmbarcador?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoCOFINSAnulacaoNotaAnulacaoEmbarcador?.Codigo ?? 0
                    },
                    TipoMovimentoCOFINSCancelamento = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoCOFINSCancelamento?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoCOFINSCancelamento?.Codigo ?? 0
                    },
                    TipoMovimentoCOFINSEmissao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoCOFINSEmissao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoCOFINSEmissao?.Codigo ?? 0
                    },
                    TipoMovimentoIRAnulacao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoIRAnulacao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoIRAnulacao?.Codigo ?? 0
                    },
                    TipoMovimentoIRAnulacaoComNotaAnulacaoEmbarcador = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoIRAnulacaoNotaAnulacaoEmbarcador?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoIRAnulacaoNotaAnulacaoEmbarcador?.Codigo ?? 0
                    },
                    TipoMovimentoIRCancelamento = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoIRCancelamento?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoIRCancelamento?.Codigo ?? 0
                    },
                    TipoMovimentoIREmissao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoIREmissao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoIREmissao?.Codigo ?? 0
                    },
                    TipoMovimentoCSLLAnulacao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoCSLLAnulacao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoCSLLAnulacao?.Codigo ?? 0
                    },
                    TipoMovimentoCSLLAnulacaoComNotaAnulacaoEmbarcador = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoCSLLAnulacaoNotaAnulacaoEmbarcador?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoCSLLAnulacaoNotaAnulacaoEmbarcador?.Codigo ?? 0
                    },
                    TipoMovimentoCSLLCancelamento = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoCSLLCancelamento?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoCSLLCancelamento?.Codigo ?? 0
                    },
                    TipoMovimentoCSLLEmissao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoCSLLEmissao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoCSLLEmissao?.Codigo ?? 0
                    },
                    TipoMovimentoUsoEntrada = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoUsoEntrada?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoUsoEntrada?.Codigo ?? 0
                    },
                    TipoMovimentoReversaoEntrada = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoReversaoEntrada?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoReversaoEntrada?.Codigo ?? 0
                    },
                    TipoMovimentoValorLiquidoAnulacao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacao?.Codigo ?? 0
                    },
                    TipoMovimentoValorLiquidoAnulacaoComNotaAnulacaoEmbarcador = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacaoNotaAnulacaoEmbarcador?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoValorLiquidoAnulacaoNotaAnulacaoEmbarcador?.Codigo ?? 0
                    },
                    TipoMovimentoValorLiquidoCancelamento = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamento?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoValorLiquidoCancelamento?.Codigo ?? 0
                    },
                    TipoMovimentoValorLiquidoEmissao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoValorLiquidoEmissao?.Codigo ?? 0
                    },
                    modeloDocumentoFiscal.GerarMovimentoBaseSTRetido,
                    modeloDocumentoFiscal.GerarMovimentoValorSTRetido,
                    TipoMovimentoBaseSTRetidoEmissao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoBaseSTRetidoEmissao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoBaseSTRetidoEmissao?.Codigo ?? 0
                    }
                    ,
                    TipoMovimentoBaseSTRetidoReversao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoBaseSTRetidoReversao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoBaseSTRetidoReversao?.Codigo ?? 0
                    }
                    ,
                    TipoMovimentoValorSTRetidoEmissao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoValorSTRetidoEmissao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoValorSTRetidoEmissao?.Codigo ?? 0
                    }
                    ,
                    TipoMovimentoValorSTRetidoReversao = new
                    {
                        Descricao = modeloDocumentoFiscal.TipoMovimentoValorSTRetidoReversao?.Descricao ?? string.Empty,
                        Codigo = modeloDocumentoFiscal.TipoMovimentoValorSTRetidoReversao?.Codigo ?? 0
                    },
                    Series = modeloDocumentoFiscal.Series != null && modeloDocumentoFiscal.Series.Count > 0 ? (from obj in modeloDocumentoFiscal.Series
                                                                                                               select new
                                                                                                               {
                                                                                                                   obj.Codigo,
                                                                                                                   Serie = new
                                                                                                                   {
                                                                                                                       val = obj.Numero,
                                                                                                                       codEntity = obj.Codigo
                                                                                                                   },
                                                                                                                   Empresa = new
                                                                                                                   {
                                                                                                                       val = obj.Empresa?.RazaoSocial ?? string.Empty,
                                                                                                                       codEntity = obj.Empresa?.Codigo ?? 0
                                                                                                                   }
                                                                                                               }).ToList() : null
                };
                return new JsonpResult(dynModeloDocumentoFiscal);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(int.Parse(Request.Params("Codigo")));
                if (modeloDocumentoFiscal.Editavel)
                {
                    repModeloDocumentoFiscal.Deletar(modeloDocumentoFiscal, Auditado);
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Não é permitido excluir esse modelo de documento.");
                }
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTodos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool opcaoSemGrupo;
                bool.TryParse(Request.Params("OpcaoSemGrupo"), out opcaoSemGrupo);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);

                List<Dominio.Entidades.ModeloDocumentoFiscal> modelos = repModeloDocumentoFiscal.Consultar("", "", null, null, null, ativo, "Descricao", "asc", 0, 999);

                var retorno = (from obj in modelos select new { value = obj.Codigo, text = obj.Descricao + (!string.IsNullOrWhiteSpace(obj.Abreviacao) ? " (" + obj.Abreviacao + ")" : "") }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarListaSeries(ref Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);

            //Lista das series
            dynamic dynSeries = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ListaSeries"));
            if (modeloDocumentoFiscal.Series == null)
                modeloDocumentoFiscal.Series = new List<Dominio.Entidades.EmpresaSerie>();

            List<int> codigosSeries = new List<int>();
            if (dynSeries.Count > 0)
            {
                foreach (var ser in dynSeries)
                {
                    int.TryParse((string)ser.Serie.codEntity, out int codigoEmpresaSerie);
                    Dominio.Entidades.EmpresaSerie empresaSerie;
                    if (codigoEmpresaSerie > 0)
                    {
                        empresaSerie = repEmpresaSerie.BuscarPorCodigo(codigoEmpresaSerie);

                        if (!modeloDocumentoFiscal.Series.Contains(empresaSerie))
                            modeloDocumentoFiscal.Series.Add(empresaSerie);

                        codigosSeries.Add(empresaSerie.Codigo);
                    }
                }
            }
            RemoverModeloDocumentoFiscal(ref modeloDocumentoFiscal, codigosSeries);
            //foreach (var ser in modeloDocumentoFiscal.Series)
            //{
            //    if (!codigosSeries.Contains(ser.Codigo))
            //        modeloDocumentoFiscal.Series.Remove(ser);
            //}
        }

        private bool RemoverModeloDocumentoFiscal(ref Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal, List<int> codigosSeries)
        {
            foreach (var ser in modeloDocumentoFiscal.Series)
            {
                if (!codigosSeries.Contains(ser.Codigo))
                {
                    modeloDocumentoFiscal.Series.Remove(ser);
                    return RemoverModeloDocumentoFiscal(ref modeloDocumentoFiscal, codigosSeries);
                }
            }
            return true;
        }

        #endregion
    }
}
