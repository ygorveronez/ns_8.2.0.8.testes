using Newtonsoft.Json;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Contatos
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Contatos/ContatoCliente")]
    public class ContatoClienteController : BaseController
    {
		#region Construtores

		public ContatoClienteController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R094_ContatoCliente;

        private decimal TamanhoColumaExtraPequena = 1m;
        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaGrande = 5.50m;
        private decimal TamanhoColunaMedia = 3m;

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.Prop("Codigo").Visibilidade(false);
            grid.Prop("Numero").Nome("Número").Tamanho(TamanhoColumaExtraPequena);
            grid.Prop("Documento").Nome("Documentos").Tamanho(TamanhoColunaMedia).Agr(true);
            grid.Prop("Contato").Nome("Contato").Tamanho(TamanhoColunaMedia).Agr(true);
            grid.Prop("DataContato").Nome("Data Contato").Tamanho(TamanhoColunaPequena).Agr(true);
            grid.Prop("DataPrevistaRetorno").Nome("Data Prev. Retorno").Tamanho(TamanhoColunaPequena).Agr(true);
            grid.Prop("Usuario").Nome("Usuário").Tamanho(TamanhoColunaMedia).Agr(true);
            grid.Prop("Descricao").Nome("Descrição").Tamanho(TamanhoColunaGrande);
            grid.Prop("GrupoPessoas").Nome("Grupo de Pessoas").Tamanho(TamanhoColunaMedia).Agr(true);
            grid.Prop("CPFCNPJPessoaFormatado").Nome("CPF/CNPJ Pessoa").Tamanho(TamanhoColunaMedia).Agr(true);
            grid.Prop("Pessoa").Nome("Pessoa").Tamanho(TamanhoColunaGrande).Agr(true);
            grid.Prop("Situacao").Nome("Situação").Tamanho(TamanhoColunaMedia).Agr(true);
            grid.Prop("Tipo").Nome("Tipo").Tamanho(TamanhoColunaMedia).Agr(true);

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Contatos Realizados", "Contatos", "ContatoCliente.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Numero", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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

                int.TryParse(Request.Params("Fatura"), out int codigoFatura);
                int.TryParse(Request.Params("Titulo"), out int codigoTitulo);
                int.TryParse(Request.Params("Bordero"), out int codigoBordero);
                int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out double cpfCnpjPessoa);

                List<int> situacoes = JsonConvert.DeserializeObject<List<int>>("[" + Request.Params("Situacao") + "]" ?? "");
                List<int> tipos = JsonConvert.DeserializeObject<List<int>>("[" + Request.Params("Tipo") + "]" ?? "");

                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

                Repositorio.Embarcador.Contatos.ContatoCliente repContatoCliente = new Repositorio.Embarcador.Contatos.ContatoCliente(unitOfWork);

                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                // TODO (ct-reports): Repassar CT
                IList<Dominio.Relatorios.Embarcador.DataSource.Contatos.ContatoCliente.ContatoCliente> listaComissaoProduto = repContatoCliente.ConsultarRelatorio(codigoEmpresa, agrupamentos, dataInicial, dataFinal, codigoTitulo, codigoFatura, codigoBordero, codigoGrupoPessoas, cpfCnpjPessoa, tipos, situacoes, propAgrupa, grid.group.dirOrdena, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repContatoCliente.ContarConsultaRelatorio(codigoEmpresa, agrupamentos, dataInicial, dataFinal, codigoTitulo, codigoFatura, codigoBordero, codigoGrupoPessoas, cpfCnpjPessoa, tipos, situacoes, propAgrupa, grid.group.dirOrdena, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite));

                grid.AdicionaRows(listaComissaoProduto);

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
                int.TryParse(Request.Params("Fatura"), out int codigoFatura);
                int.TryParse(Request.Params("Titulo"), out int codigoTitulo);
                int.TryParse(Request.Params("Bordero"), out int codigoBordero);
                int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out double cpfCnpjPessoa);

                List<int> situacoes = JsonConvert.DeserializeObject<List<int>>(Request.Params("Situacao") ?? "");
                List<int> tipos = JsonConvert.DeserializeObject<List<int>>(Request.Params("Tipo") ?? "");

                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorioContatoCliente(codigoEmpresa, agrupamentos, dataInicial, dataFinal, codigoTitulo, codigoFatura, codigoBordero, codigoGrupoPessoas, cpfCnpjPessoa, tipos, situacoes, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorioContatoCliente(int codigoEmpresa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, DateTime dataInicial, DateTime dataFinal, int codigoTitulo, int codigoFatura, int codigoBordero, int codigoGrupoPessoas, double cpfCnpjPessoa, List<int> tipos, List<int> situacoes, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.Contatos.ContatoCliente repContatoCliente = new Repositorio.Embarcador.Contatos.ContatoCliente(unitOfWork);
                Repositorio.Embarcador.Contatos.SituacaoContato repSituacaoContato = new Repositorio.Embarcador.Contatos.SituacaoContato(unitOfWork);
                Repositorio.Embarcador.Contatos.TipoContato repTipoContato = new Repositorio.Embarcador.Contatos.TipoContato(unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Contatos.ContatoCliente.ContatoCliente> listaContatoCliente = repContatoCliente.ConsultarRelatorio(codigoEmpresa, propriedades, dataInicial, dataFinal, codigoTitulo, codigoFatura, codigoBordero, codigoGrupoPessoas, cpfCnpjPessoa, tipos, situacoes, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0);

                Dominio.Entidades.Empresa empresaRelatorio = await repEmpresa.BuscarPorCodigoAsync(Empresa.Codigo, cancellationToken);

                //CrystalDecisions.CrystalReports.Engine.ReportDocument report = serRelatorio.CriarRelatorio(relatorioControleGeracao, relatorioTemp, listaContatoCliente, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                if(dataInicial != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", dataInicial.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", false));

                if (dataFinal != DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", dataFinal.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", false));

                if (cpfCnpjPessoa > 0d)
                {
                    Dominio.Entidades.Cliente pessoa = await repCliente.BuscarPorCPFCNPJAsync(cpfCnpjPessoa);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", pessoa.CPF_CNPJ_Formatado + " - " + pessoa.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", false));

                if (codigoGrupoPessoas > 0)
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = await repGrupoPessoas.BuscarPorCodigoAsync(codigoGrupoPessoas);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", grupoPessoas.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", false));

                if(codigoBordero > 0)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = repBordero.BuscarPorCodigo(codigoBordero);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Bordero", bordero.Numero.ToString(), true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Bordero", false));

                if (codigoFatura > 0)
                {
                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);

                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fatura", fatura.Numero.ToString(), true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fatura", false));

                if (codigoTitulo > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Titulo", codigoTitulo.ToString(), true));
                                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Titulo", false));

                if(situacoes != null && situacoes.Count > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", string.Join(", ", repSituacaoContato.BuscarDescricaoPorCodigo(situacoes)), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", false));

                if (tipos != null && tipos.Count > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", string.Join(", ", repTipoContato.BuscarDescricaoPorCodigo(tipos)), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", false));

                // serRelatorio.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);

                // serRelatorio.GerarRelatorio(report, relatorioControleGeracao, "Relatorios/Contatos/ContatoCliente", unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Contatos/ContatoCliente", parametros, relatorioControleGeracao, relatorioTemp, listaContatoCliente, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
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
    }
}
