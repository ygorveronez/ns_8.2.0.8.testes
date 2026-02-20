using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Seguros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Seguros/Apolices")]
    public class ApolicesController : BaseController
    {
		#region Construtores

		public ApolicesController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R068_Apolices;

        private decimal TamanhoColunasMedia = 4.50m;
        private decimal TamanhoColunasDescritivos = 5.50m;
        private decimal TamanhoColunasPequeno = 3;

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unidadeDeTrabalho)
        {

            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.Prop("Codigo").Visibilidade(false);
            grid.Prop("Seguradora").Nome("Seguradora").Tamanho(TamanhoColunasDescritivos);
            grid.Prop("NumeroApolice").Nome("Número Apólice").Tamanho(TamanhoColunasDescritivos);
            grid.Prop("NumeroAverbacao").Nome("Número Averbação").Tamanho(TamanhoColunasDescritivos);
            grid.Prop("InicioVigencia").Nome("Inicio Vigência").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("FimVigencia").Nome("Fim Vigência").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("ValorLimite").Nome("Valor Limite").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right);
            grid.Prop("Responsavel").Nome("Responsável").Tamanho(TamanhoColunasDescritivos);
            grid.Prop("Averbadora").Nome("Averbadora").Tamanho(TamanhoColunasDescritivos);

            grid.Prop("Empresa").Nome("Empresa").Visibilidade(false).Tamanho(TamanhoColunasDescritivos).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.Prop("Pessoa").Nome("Pessoa").Tamanho(TamanhoColunasDescritivos).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.Prop("GrupoPessoa").Nome("Grupo Pessoa").Tamanho(TamanhoColunasDescritivos).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.Prop("VencimentoCertificadoDigital").Nome("Data Vencimento Certificado Digital").Tamanho(TamanhoColunasMedia).Visibilidade(false).Align(Models.Grid.Align.center); ;

            return grid;

        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio;
                int.TryParse(Request.Params("Codigo"), out codigoRelatorio);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Apólices", "Seguros", "Apolices.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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

                List<Dominio.Relatorios.Embarcador.DataSource.Seguros.ReportApolices> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = null;

                int quantidade = 0;

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";

                SetarPropriedadeOrdenacao(ref propOrdena);

                ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.setarQuantidadeTotal(quantidade);
                grid.AdicionaRows(listaReport);

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

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                string stringConexao = _conexao.StringConexao;
                _ = Task.Factory.StartNew(() => GerarRelatorioAverbacoes(relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
        }


        #region Métodos Privados
        private async Task GerarRelatorioAverbacoes(Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
            try
            {
                List<Dominio.Relatorios.Embarcador.DataSource.Seguros.ReportApolices> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
                int quantidade = 0;

                string propOrdena = relatorioTemp.PropriedadeOrdena;
                string dirOrdena = relatorioTemp.OrdemOrdenacao;
                string propAgrupa = relatorioTemp.PropriedadeAgrupa;
                string dirAgrupa = relatorioTemp.OrdemAgrupamento;

                SetarPropriedadeOrdenacao(ref propOrdena);

                ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0, unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Seguros/Apolices",parametros,relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork);

                await unitOfWork.DisposeAsync();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
        }

        private void SetarPropriedadeOrdenacao(ref string propOrdena)
        {
            if (propOrdena == "Seguradora")
                propOrdena = "Seguradora.Nome";
            else if (propOrdena == "ValorLimite")
                propOrdena = "ValorLimiteApolice";
            else if (propOrdena == "Averbadora")
                propOrdena = "SeguradoraAverbacao";
            else if (propOrdena == "Empresa")
                propOrdena = "Empresa.CNPJ";
            else if (propOrdena == "Pessoa")
                propOrdena = "Pessoa.Nome";
            else if (propOrdena == "GrupoPessoa")
                propOrdena = "GrupoPessoas.Descricao";
            else if (propOrdena == "VencimentoCertificadoDigital")
                propOrdena = "Empresa.DataFinalCertificado";
        }

        private void ExecutarBusca(ref List<Dominio.Relatorios.Embarcador.DataSource.Seguros.ReportApolices> listaReport, ref int quantidade, ref List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            int.TryParse(Request.Params("Seguradora"), out int codSeguradora);

            bool.TryParse(Request.Params("EmVigencia"), out bool emVigencia);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro? responsavel = null;
            if (Enum.TryParse(Request.Params("Responsavel"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro responsavelAux))
                responsavel = responsavelAux;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao? averbadora = null;
            if (Enum.TryParse(Request.Params("Averbadora"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao averbadoraAux))
                averbadora = averbadoraAux;

            #region Parametros
            if (parametros != null)
            {
                Repositorio.Embarcador.Seguros.Seguradora repSeguradora = new Repositorio.Embarcador.Seguros.Seguradora(unitOfWork);
                Dominio.Entidades.Embarcador.Seguros.Seguradora seguradora = repSeguradora.BuscarPorCodigo(codSeguradora);
                if (seguradora != null)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Seguradora", seguradora.Nome, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Seguradora", false));

                if (responsavel != null)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Responsavel", FiltroParametro(responsavel.Value), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Responsavel", false));

                if (averbadora != null)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Averbadora", FiltroParametro(averbadora.Value), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Averbadora", false));

                if (emVigencia)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EmVigencia", emVigencia ? "Sim" : "Não", true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EmVigencia", false));

            }
            #endregion

            Repositorio.Embarcador.Seguros.ApoliceSeguro repApoliceSeguro = new Repositorio.Embarcador.Seguros.ApoliceSeguro(unitOfWork);

            listaReport = repApoliceSeguro.ConsultarRelatorio(codSeguradora, emVigencia, responsavel, averbadora, propOrdena, dirOrdena, inicio, limite);
            quantidade = repApoliceSeguro.ContarConsultaRelatorio(codSeguradora, emVigencia, responsavel, averbadora);
        }

        private string FiltroParametro(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro responsavel)
        {
            if (responsavel == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro.Embarcador)
                return "Embarcador";
            else if (responsavel == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ResponsavelSeguro.Transportador)
                return "Transportador";
            return "";
        }
        private string FiltroParametro(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao averbadora)
        {
            if (averbadora ==Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.ATM)
                return "ATM";
            else if (averbadora == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.Bradesco)
                return "Bradesco";
            else if (averbadora == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SeguradoraAverbacao.NaoDefinido)
                return "Não Definido";
            return "";
        }
        private string FiltroParametro(DateTime data)
        {
            return data.ToString("dd/MM/yyyy");
        }
        #endregion
    }
}
