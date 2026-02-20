using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.PedidosVendas
{
	[Area("Relatorios")]
	public class VendaDiretaController : BaseController
    {
		#region Construtores

		public VendaDiretaController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R187_VendaDireta;

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo");
            grid.Prop("CPFCNPJPessoaFormatado").Nome("CNPJ/CPF Pessoa").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center).Ord(false);
            grid.Prop("DataNascimentoFormatada").Nome("Data Nascimento").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center);
            grid.Prop("RazaoPessoa").Nome("Razão Pessoa").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true);
            grid.Prop("Email").Nome("E-mail").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("RG").Nome("RG").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("OrgaoEmissorFormatado").Nome("Orgão Emissor").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center);
            grid.Prop("UF").Nome("UF").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center);
            grid.Prop("Telefone").Nome("Telefone").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center);
            grid.Prop("Profissao").Nome("Profissão").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("TituloEleitoral").Nome("Título Eleitoral").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ZonaEleitoral").Nome("Zona Eleitoral").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("SecaoEleitoral").Nome("Seção Eleitoral").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Cidade").Nome("Cidade").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("Estado").Nome("Estado").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("PisPasep").Nome("Pis/Pasep").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("NumeroCEI").Nome("Número CEI").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);

            grid.Prop("RazaoEmpresa").Nome("Razão Empresa").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).OrdAgr(true, true).Visibilidade(false);
            grid.Prop("CPFCNPJEmpresaFormatado").Nome("CNPJ/CPF Empresa").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Ord(false).Visibilidade(false);
            grid.Prop("NumeroCEIEmpresa").Nome("Número CEI Empresa").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("TelefoneEmpresa").Nome("Telefone Empresa").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("CidadeEmpresa").Nome("Cidade Empresa").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("UFEmpresa").Nome("UF Empresa").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center).Visibilidade(false);

            grid.Prop("ValorTotal").Nome("Valor Total").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Visibilidade(false);
            grid.Prop("DataAgendamentoFormatado").Nome("Data Agendamento").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("DataVecimentoCertificadoFormatado").Nome("Vencimento Certificado").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("DataVecimentoCobrancaFormatado").Nome("Vencimento Cobrança").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.center).Visibilidade(false);
            grid.Prop("NumeroPedido").Nome("Número do Pedido").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("StatusPedidoFormatado").Nome("Status do Pedido").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("CodigoEmissao1").Nome("Código Emissão 1").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("CodigoEmissao2").Nome("Código Emissão 2").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("TipoAssinaturaFormatado").Nome("Tipo de Assinatura").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("NecessarioGerarNFFormatado").Nome("Necessário Gerar NF?").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Observacao").Nome("Observação").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("StatusVendaFormatado").Nome("Status da Venda").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("GrupoPessoa").Nome("Grupo de Pessoa").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false).OrdAgr(true, true);
            grid.Prop("Produtos").Nome("Produtos").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Servicos").Nome("Serviços").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);

            grid.Prop("ProdutoServicoFormatado").Nome("Tipo").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataTreinamentoFormatado").Nome("Data Treinamento").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("FuncionarioTreinamento").Nome("Quem Treinou").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("StatusCadastroFormatado").Nome("Status Cadastro").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("TipoClienteVendaDiretaFormatado").Nome("Tipo Cliente").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("EmitidoDocumentosFormatado").Nome("Emitido Documento").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("PendenciaFormatado").Nome("Pendência").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("CertificadoFormatado").Nome("Certificado").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);

            grid.Prop("Numero").Nome("Número").Tamanho(TamanhoColunasPequeno).Align(Models.Grid.Align.right).Visibilidade(false);
            grid.Prop("DataFormatado").Nome("Data Lançamento").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataFinalizacaoFormatado").Nome("Data Finalização").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataCancelamentoFormatado").Nome("Data Cancelamento").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataValidacaoFormatado").Nome("Data Validação").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataContestacaoFormatado").Nome("Data Contestação").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Funcionario").Nome("Funcionário").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("FuncionarioAgendador").Nome("Agendador").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("FuncionarioContestacao").Nome("Fun. Contestação").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ObservacaoContestacao").Nome("Observação Contestação").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("NumeroBoleto").Nome("Número Boleto").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("StatusTitulo").Nome("Status Título").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);

            grid.Prop("FuncionarioValidador").Nome("Validador").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataAgendadoForaFormatado").Nome("Data Agendado Fora").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataAprovadoFormatado").Nome("Data Aprovado").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataBaixadoFormatado").Nome("Data Baixado").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataFaltaAgendarFormatado").Nome("Data Falta Agendar").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataAgendadoFormatado").Nome("Data Agendado").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataContato1Formatado").Nome("Data Contato 1").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataContato2Formatado").Nome("Data Contato 2").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataContato3Formatado").Nome("Data Contato 3").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataProblemaFormatado").Nome("Data Problema").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataReagendarFormatado").Nome("Data Reagendar").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataClienteBaixaFormatado").Nome("Data Cliente Baixa").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataAguardandoVerificacaoFormatado").Nome("Data Aguardando Verificação").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Vendas Diretas", "PedidoVenda", "VendaDireta.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, false, true);
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

                List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioVendaDireta> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = null;

                int quantidade = 0;

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                string propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propriedades, propAgrupa, grid.group.dirOrdena, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);;

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

                GerarRelatorioAsync(agrupamentos, relatorioControleGeracao, relatorioTemp, stringConexao, cancellationToken);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private async Task GerarRelatorioAsync(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Empresa empresaRelatorio = repEmpresa.BuscarPorCodigo(Empresa.Codigo);

                string propOrdena = relatorioTemp.PropriedadeOrdena;
                string dirOrdena = relatorioTemp.OrdemOrdenacao;
                string propAgrupa = relatorioTemp.PropriedadeAgrupa;
                string dirAgrupa = relatorioTemp.OrdemAgrupamento;

                List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioVendaDireta> listaReport = null;
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
                int quantidade = 0;

                ExecutarBusca(ref listaReport, ref quantidade, ref parametros, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0, unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/PedidosVendas/VendaDireta", parametros, relatorioControleGeracao, relatorioTemp, listaReport, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
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

        private void ExecutarBusca(ref List<Dominio.Relatorios.Embarcador.DataSource.PedidosVendas.RelatorioVendaDireta> reportResult, ref int quantidade, ref List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PedidoVenda.VendaDireta repVendaDireta = new Repositorio.Embarcador.PedidoVenda.VendaDireta(unitOfWork);

            DateTime dataVendaInicial = Request.GetDateTimeParam("DataVendaInicial");
			DateTime dataVendaFinal = Request.GetDateTimeParam("DataVendaFinal");
			DateTime dataFinalizacaoInicial = Request.GetDateTimeParam("DataFinalizacaoInicial");
			DateTime dataFinalizacaoFinal = Request.GetDateTimeParam("DataFinalizacaoFinal");
			DateTime dataAgendamentoInicial = Request.GetDateTimeParam("DataAgendamentoInicial");
			DateTime dataAgendamentoFinal = Request.GetDateTimeParam("DataAgendamentoFinal");
			DateTime dataVencimentoCertificadoInicial = Request.GetDateTimeParam("DataVencimentoCertificadoInicial");
			DateTime dataVencimentoCertificadoFinal = Request.GetDateTimeParam("DataVencimentoCertificadoFinal");
			DateTime dataVencimentoCobrancaInicial = Request.GetDateTimeParam("DataVencimentoCobrancaInicial");
			DateTime dataVencimentoCobrancaFinal = Request.GetDateTimeParam("DataVencimentoCobrancaFinal");

            int.TryParse(Request.Params("Funcionario"), out int codigoFuncionario);
            int.TryParse(Request.Params("Agendador"), out int codigoAgendador);
            int.TryParse(Request.Params("Validador"), out int codigoValidador);
            int.TryParse(Request.Params("Produto"), out int codigoProduto);
            int.TryParse(Request.Params("Servico"), out int codigoServico);
            int.TryParse(Request.Params("Titulo"), out int codigoTitulo);
            int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);

            double.TryParse(Request.Params("Pessoa"), out double pessoa);

            string numeroPedido = Request.Params("NumeroPedido");
            string numeroBoleto = Request.Params("NumeroBoleto");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaVendaDireta tipoCobrancaVendaDireta;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusVendaDireta statusVenda;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVendaDireta statusPedido;
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProdutoServico produtoServico;
            Enum.TryParse(Request.Params("StatusVenda"), out statusVenda);
            Enum.TryParse(Request.Params("StatusPedido"), out statusPedido);
            Enum.TryParse(Request.Params("TipoCobrancaVendaDireta"), out tipoCobrancaVendaDireta);
            Enum.TryParse(Request.Params("ProdutoServico"), out produtoServico);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                codigoEmpresa = this.Usuario.Empresa.Codigo;
                if (!this.Usuario.UsuarioAdministrador)
                    codigoFuncionario = this.Usuario.Codigo;
            }

            #region Parametros
            if (parametros != null)
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                if (dataVendaInicial != DateTime.MinValue || dataVendaFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataVendaInicial != DateTime.MinValue ? dataVendaInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataVendaFinal != DateTime.MinValue ? "até " + dataVendaFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVenda", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVenda", false));

                if (dataFinalizacaoInicial != DateTime.MinValue || dataFinalizacaoFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataFinalizacaoInicial != DateTime.MinValue ? dataFinalizacaoInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataFinalizacaoFinal != DateTime.MinValue ? "até " + dataFinalizacaoFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalizacao", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalizacao", false));

                if (dataAgendamentoInicial != DateTime.MinValue || dataAgendamentoFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataAgendamentoInicial != DateTime.MinValue ? dataAgendamentoInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataAgendamentoFinal != DateTime.MinValue ? "até " + dataAgendamentoFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAgendamento", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAgendamento", false));

                if (!string.IsNullOrWhiteSpace(numeroPedido))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroPedido", numeroPedido, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroPedido", false));

                if (!string.IsNullOrWhiteSpace(numeroBoleto))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroBoleto", numeroBoleto, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroBoleto", false));

                if ((int)statusVenda > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusVenda", Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusVendaDiretaHelper.ObterDescricao(statusVenda), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusVenda", false));

                if ((int)statusPedido > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusPedido", Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusPedidoVendaDiretaHelper.ObterDescricao(statusPedido), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusPedido", false));

                if (pessoa > 0)
                {
                    Dominio.Entidades.Cliente _pessoa = repCliente.BuscarPorCPFCNPJ(pessoa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", _pessoa.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", false));

                if (codigoFuncionario > 0)
                {
                    Dominio.Entidades.Usuario _funcionario = repUsuario.BuscarPorCodigo(codigoFuncionario);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Funcionario", _funcionario.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Funcionario", false));

                if (codigoProduto > 0)
                {
                    Dominio.Entidades.Produto _produto = repProduto.BuscarPorCodigo(codigoProduto);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", _produto.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", false));

                if (codigoServico > 0)
                {
                    Dominio.Entidades.Embarcador.NotaFiscal.Servico _servico = repServico.BuscarPorCodigo(codigoServico);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Servico", _servico.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Servico", false));

                if (codigoTitulo > 0)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo _titulo = repTitulo.BuscarPorCodigo(codigoTitulo);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Titulo", _titulo.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Titulo", false));

                if (codigoEmpresa > 0)
                {
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa.RazaoSocial, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", false));

                if (!string.IsNullOrWhiteSpace(propAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", propAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

                if (codigoGrupoPessoas > 0)
                {
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupo = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", grupo.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", false));

                if (dataVencimentoCertificadoInicial != DateTime.MinValue || dataVencimentoCertificadoFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataVencimentoCertificadoInicial != DateTime.MinValue ? dataVencimentoCertificadoInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataVencimentoCertificadoFinal != DateTime.MinValue ? "até " + dataVencimentoCertificadoFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoCertificado", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoCertificado", false));

                if (dataVencimentoCobrancaInicial != DateTime.MinValue || dataVencimentoCobrancaFinal != DateTime.MinValue)
                {
                    string data = "";
                    data += dataVencimentoCobrancaInicial != DateTime.MinValue ? dataVencimentoCobrancaInicial.ToString("dd/MM/yyyy") + " " : "";
                    data += dataVencimentoCobrancaFinal != DateTime.MinValue ? "até " + dataVencimentoCobrancaFinal.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoCobranca", data, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimentoCobranca", false));
            }
            #endregion
            // TODO: ToList cast
            reportResult = repVendaDireta.ConsultarRelatorioVendaDireta(codigoValidador, produtoServico, codigoGrupoPessoas, dataVencimentoCertificadoInicial, dataVencimentoCertificadoFinal, dataVencimentoCobrancaInicial, dataVencimentoCobrancaFinal, codigoEmpresa, dataVendaInicial, dataVendaFinal, dataFinalizacaoInicial, dataFinalizacaoFinal, dataAgendamentoInicial, dataAgendamentoFinal, numeroPedido, numeroBoleto, codigoFuncionario, codigoAgendador, codigoProduto, codigoServico, codigoTitulo, pessoa, statusVenda, statusPedido, tipoCobrancaVendaDireta, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite).ToList();
            quantidade = repVendaDireta.ContarConsultaRelatorioVendaDireta(codigoValidador, produtoServico, codigoGrupoPessoas, dataVencimentoCertificadoInicial, dataVencimentoCertificadoFinal, dataVencimentoCobrancaInicial, dataVencimentoCobrancaFinal, codigoEmpresa, dataVendaInicial, dataVendaFinal, dataFinalizacaoInicial, dataFinalizacaoFinal, dataAgendamentoInicial, dataAgendamentoFinal, numeroPedido, numeroBoleto, codigoFuncionario, codigoAgendador, codigoProduto, codigoServico, codigoTitulo, pessoa, statusVenda, statusPedido, tipoCobrancaVendaDireta, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena);
        }
    }
}
