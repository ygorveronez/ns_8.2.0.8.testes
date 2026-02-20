using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/CTe/Tomador")]
    public class TomadorController : BaseController
    {
		#region Construtores

		public TomadorController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R063_Tomador;

        private decimal TamanhoColumaExtraPequena = 1m;
        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaGrande = 5.50m;
        private decimal TamanhoColunaMedia = 3m;

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("CPF/CNPJ", "CPFCNPJFormatado", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("IE", "InscricaoEstadual", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Razão Social", "RazaoSocial", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Nome Fantasia", "NomeFantasia", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Telefone", "Telefone", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("CEP", "CEP", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Endereco", "Endereco", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Bairro", "Bairro", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Complemento", "Complemento", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Número", "Numero", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Cidade", "Cidade", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Estado", "Estado", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data do Cadastro", "DataCadastro", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("E-mail", "Email", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Sit. E-mail", "SituacaoEmail", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("E-mail Grupo", "EmailGrupo", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Sit. E-mail Grupo", "SituacaoEmailGrupo", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);

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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Tomadores", "CTe", "Tomador.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "RazaoSocial", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);

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

                DateTime dataCadastroInicial, dataCadastroFinal;
                DateTime.TryParseExact(Request.Params("DataCadastroInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataCadastroInicial);
                DateTime.TryParseExact(Request.Params("DataCadastroFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataCadastroFinal);

                double cpfCnpjTomador;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Tomador")), out cpfCnpjTomador);

                int codigoGrupoPessoas;
                int.TryParse(Request.Params("GrupoPessoas"), out codigoGrupoPessoas);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "CPFCNPJFormatado")
                    propOrdena = "CPFCNPJ";

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                IList<Dominio.Relatorios.Embarcador.DataSource.CTe.Tomador> listaTomadores = repCTe.ConsultarRelatorioTomadores(agrupamentos, dataCadastroInicial, dataCadastroFinal, cpfCnpjTomador, codigoGrupoPessoas, propAgrupa, grid.group.dirOrdena, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCTe.ContarConsultaRelatorioTomadores(agrupamentos, dataCadastroInicial, dataCadastroFinal, cpfCnpjTomador, codigoGrupoPessoas, propAgrupa, grid.group.dirOrdena, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite));

                grid.AdicionaRows(listaTomadores);

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
                DateTime dataCadastroInicial, dataCadastroFinal;
                DateTime.TryParseExact(Request.Params("DataCadastroInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataCadastroInicial);
                DateTime.TryParseExact(Request.Params("DataCadastroFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataCadastroFinal);

                double cpfCnpjTomador;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Tomador")), out cpfCnpjTomador);

                int codigoGrupoPessoas;
                int.TryParse(Request.Params("GrupoPessoas"), out codigoGrupoPessoas);

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                if (propOrdena == "CPFCNPJFormatado")
                    propOrdena = "CPFCNPJ";

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorioTomadores(agrupamentos, dataCadastroInicial, dataCadastroFinal, cpfCnpjTomador, codigoGrupoPessoas, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioTomadores(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, DateTime dataCadastroInicial, DateTime dataCadastroFinal, double cpfCnpjTomador, int codigoGrupoPessoas, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork, cancellationToken);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork, cancellationToken);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);

                IList<Dominio.Relatorios.Embarcador.DataSource.CTe.Tomador> listaTomadores = await repCTe.ConsultarRelatorioTomadoresAsync(propriedades, dataCadastroInicial, dataCadastroFinal, cpfCnpjTomador, codigoGrupoPessoas, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0);

                Dominio.Entidades.Empresa empresaRelatorio = await repEmpresa.BuscarPorCodigoAsync(Empresa.Codigo);

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                if (dataCadastroInicial != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCadastroInicial", dataCadastroInicial.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCadastroInicial", false));

                if (dataCadastroFinal != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCadastroFinal", dataCadastroFinal.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCadastroFinal", false));

                if (cpfCnpjTomador > 0d)
                {
                    Dominio.Entidades.Cliente pessoa = await repCliente.BuscarPorCPFCNPJAsync(cpfCnpjTomador);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", pessoa.CPF_CNPJ_Formatado + " - " + pessoa.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", false));

                if (codigoGrupoPessoas > 0)
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = await repGrupoPessoas.BuscarPorCodigoAsync(codigoGrupoPessoas);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", grupoPessoas.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", false));

                serRelatorio.GerarRelatorioDinamico("Relatorios/CTe/Tomador", parametros,relatorioControleGeracao, relatorioTemp, listaTomadores, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
            }
            catch (Exception ex)
            {
                await serRelatorio.RegistrarFalhaGeracaoRelatorioAsync(relatorioControleGeracao, unitOfWork, ex, cancellationToken);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }
    }
}
