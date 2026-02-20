using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Linq;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/MovimentacaoContasPagarTransportador")]
    public class MovimentacaoContasPagarTransportadorController : BaseController
    {
		#region Construtores

		public MovimentacaoContasPagarTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        #endregion

        #region Metodos Privados 
        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);


            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Código Empresa", "CodigoEmpresa", 10, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Código do Transportador", "CodigoTransportador", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Código do Documento", "CodigoDocumento", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Data Documento", "DataDocumento", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Documento de Baixa", "DocumentoBaixa", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("CNPJ Transportador", "CNPJTransportador", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Razão Social Transportador", "RazaoSocialTransportador", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Raiz Transportador", "RaizTransportador", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Região Transportador", "RegiaoTransportador", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Cidade Transportador", "CidadeTransportador", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Número CT-e", "NumeroCte", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Série CT-e", "SerieCte", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Chave CT-e", "ChaveCTe", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Valor líquido", "ValorLiquido", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Data da Baixa", "DataBaixa", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Situação Vencimento", "SituacaoVencimento", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Tipo de Arquivo", "TipoRegistro", 10, Models.Grid.Align.right, false);

            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaMovimentacaoContaPagar filtroPesquisa = ObterFiltroPesquisa();

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "MovimentacaoContasPagarTransportador/Pesquisa", "grid-movimentacao-contas-pagar-transportador");
            grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar repositorioMovimentacaoContaPagar = new Repositorio.Embarcador.Financeiro.MovimentacaoContaPagar(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> movimentacaoConta = repositorioMovimentacaoContaPagar.Pesquisar(filtroPesquisa, grid.ObterParametrosConsulta());
            int quantidade = repositorioMovimentacaoContaPagar.ContarPesquisa(filtroPesquisa);

            var retorno = (from obj in movimentacaoConta
                           select new
                           {
                               obj.Codigo,
                               CodigoEmpresa = obj.CodigoEmpresa,
                               CodigoTransportador = obj.Transportador?.CodigoIntegracao ?? string.Empty,
                               CodigoDocumento = obj.Referencia,
                               DataDocumento = obj.DataDocumento?.ToString("dd/MM/yyyy") ?? "",
                               Valor = obj.ValorMonetario,
                               DocumentoBaixa = obj?.ClrngDoc ?? string.Empty,
                               CNPJTransportador = obj.Transportador?.CNPJ_Formatado ?? string.Empty,
                               RazaoSocialTransportador = obj.Transportador?.RazaoSocial ?? string.Empty,
                               RaizTransportador = obj.Transportador?.RaizCnpj ?? string.Empty,
                               RegiaoTransportador = obj.Transportador?.Localidade?.Estado?.Sigla ?? string.Empty,
                               CidadeTransportador = obj.Transportador?.Localidade?.DescricaoCidadeEstado ?? string.Empty, 
                               ChaveCTe = obj.ChaveAcesso,
                               NumeroCte = obj.NumeroCte,
                               SerieCte = obj.SerieCte,
                               ValorLiquido = obj.NetValor,
                               DataVencimento = obj.DueData?.ToString("dd/MM/yyyy") ?? "",
                               DataBaixa = obj.DataCompensamento?.ToString("dd/MM/yyyy") ?? "",
                               SituacaoVencimento = obj.DueData.ToString() != "" ? (DateTime.Now.Date < obj.DueData ? "A vencer" : DateTime.Now.Date > obj.DueData ? "Vencido" : "Vencendo") : string.Empty,
                               TipoRegistro = obj.TipoRegistro.ObterDescricao()
                           }).ToList();

            grid.AdicionaRows(retorno);
            grid.setarQuantidadeTotal(quantidade);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaMovimentacaoContaPagar ObterFiltroPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaMovimentacaoContaPagar filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaMovimentacaoContaPagar
            {
                DataCompensacaoFinal = Request.GetNullableDateTimeParam("DataCompensacaoFinal"),
                DataCompensacaoInicial = Request.GetNullableDateTimeParam("DataCompensacaoInicial"),
                DataDocFinal = Request.GetNullableDateTimeParam("DataDocumentoFinal"),
                DataDocInicial = Request.GetNullableDateTimeParam("DataDocumentoInicial"),
                TipoArquivo = Request.GetEnumParam<TipoRegistro>("TipoArquivo"),
                CodigoNumeroDocumento = Request.GetIntParam("NumeroDocumento"),
                DocumentoCompensacao = Request.GetStringParam("DocumentoCompensacao"),
                TodasFiliaisTransportador = Request.GetBoolParam("TodasFiliaisTransportador"),
            };

            filtrosPesquisa.Transportador = this.Empresa.Codigo;

            if (filtrosPesquisa.TodasFiliaisTransportador)
                filtrosPesquisa.CodigosFiliaisTransportador = this.Empresa.Filiais.Select(o => o.Codigo).ToList();

            return filtrosPesquisa;
        }

        #endregion
    }
}
