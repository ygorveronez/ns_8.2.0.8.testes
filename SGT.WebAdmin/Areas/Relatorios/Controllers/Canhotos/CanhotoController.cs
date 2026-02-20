using Dominio.ObjetosDeValor.Embarcador.Canhoto;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System.Threading;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Canhotos
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Canhotos/Canhoto")]
    public class CanhotoController : Relatorios.AutomatizacaoGeracaoRelatorioController<Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto>
    {
        #region Construtores

        public CanhotoController(Conexao conexao) : base(conexao) { }

        #endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R031_Canhotos;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                string propAgrupa = "";
                string dirAgrupa = "";

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    dirAgrupa = "asc";
                    propAgrupa = "Empresa";
                }

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, Localization.Resources.Relatorios.Canhotos.Canhoto.RelatorioDeCanhotos, "Canhotos", "Canhoto.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "DataEmissao", "desc", propAgrupa, dirAgrupa, Codigo, unitOfWork, true, true, 8);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(await ObterGridPadrao(unitOfWork, cancellationToken), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Relatorios.Canhotos.Canhoto.OcorreuUmaFalhaAoBuscarDadosRelatorio);
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

                Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);

                if ((filtrosPesquisa.Situacoes != null && filtrosPesquisa.Situacoes.Count > 0) && (!filtrosPesquisa.DataInicialHistorico.HasValue || !filtrosPesquisa.DataFinalHistorico.HasValue))
                    return new JsonpResult(false, Localization.Resources.Relatorios.Canhotos.Canhoto.DataInicialFinalDevemInformadas);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Canhotos.Canhoto servicoRelatorioCanhoto = new Servicos.Embarcador.Relatorios.Canhotos.Canhoto(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioCanhoto.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Canhotos.Canhoto.Canhoto> listaCanhotos, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.AdicionaRows(listaCanhotos);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Relatorios.Canhotos.Canhoto.OcorreuUmaFalhaAoBuscarDadosRelatorio);
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
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);

                if ((filtrosPesquisa.Situacoes != null && filtrosPesquisa.Situacoes.Count > 0) && (!filtrosPesquisa.DataInicialHistorico.HasValue || !filtrosPesquisa.DataFinalHistorico.HasValue))
                    return new JsonpResult(false, Localization.Resources.Relatorios.Canhotos.Canhoto.DataInicialFinalDevemInformadas);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Relatorios.Canhotos.Canhoto.OcorreuFalhaGerarRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task<Models.Grid.Grid> ObterGridPadrao(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork, cancellationToken);

            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            bool visibilidadeTMS = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? true : false;
            bool visibilidadeEmbarcador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? true : false;

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Numero, "Numero", 2.5m, Models.Grid.Align.left, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Tipo, "DescricaoTipoCanhoto", 2m, Models.Grid.Align.left, true, true);

            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Serie, "Serie", 1.5m, Models.Grid.Align.left, false, visibilidadeTMS);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.DataDeEmissao, "DescricaoDataEmissao", 3m, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.ChaveNFe, "ChaveNF", 7m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.NumeroDaCarga, "NumeroCarga", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.NumeroPedido, "NumeroPedido", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.TipoPagamento, "DescricaoModalidadeFrete", 2.5m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.NaturezaOP, "NaturezaOP", 5m, Models.Grid.Align.left, true, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.GrupoPessoas, "GrupoPessoa", 7m, Models.Grid.Align.left, true, true, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.CNPJTerceiro, "CPFCNPJTerceiroResponsavelFormatado", 3.5m, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.TerceiroResponsavel, "TerceiroResponsavel", 7m, Models.Grid.Align.left, true, false, false, true);
            }

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.CPFCNPJEmitente, "CPFCNPJEmitenteFormatado", 3.5m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.Emitente, "Emitente", 7m, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.CPFCNPJDestinatario, "CPFCNPJDestinatarioFormatado", 3.5m, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.Destinatario, "Destinatario", 7m, Models.Grid.Align.left, true, visibilidadeEmbarcador, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.CPFCNPJRecebedor, "CpfCnpjRecebedorFormatado", 3.5m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.Recebedor, "Recebedor", 7m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.CPFCNPJRemetente, "CpfCnpjRemetenteFormatado", 3.5m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.Remetente, "Remetente", 7m, Models.Grid.Align.left, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.Filial, "Filial", 7m, Models.Grid.Align.left, true, false, false, true, false);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.CNPJTransportador, "CNPJTransportadorFormatado", 3.5m, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.Transportador, "Empresa", 7m, Models.Grid.Align.left, true, false, true, true, false);
            }

            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoSituacaoCanhoto", 3m, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.Digitalizacao, "DescricaoSituacaoDigitalizacaoCanhoto", 2.5m, Models.Grid.Align.left, true, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.DataDigitalizaçao, "DataDigitalizacaoFormatada", 3m, Models.Grid.Align.left, true, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.MotivoRejeicaoImagem, "MotivoRejeicaoDigitalizacao", 7m, Models.Grid.Align.left, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.LocCanhoto, "LocalArmazenamentoCanhoto", 5m, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.Pacote, "Pacote", 2.5m, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.Posicao, "Posicao", 2.5m, Models.Grid.Align.left, true);
            }

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.DataEnvioCanhoto, "DescricaoDataEnvioCanhoto", 3m, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.TipoDeCarga, "TipoCarga", 7m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.TipoDeOperacao, "TipoOperacao", 7m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.PesoBruto, "PesoBruto", 3m, Models.Grid.Align.right, true, TipoSumarizacao.sum, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.ValorNFe, "Valor", 3m, Models.Grid.Align.right, true, TipoSumarizacao.sum);

            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Justificativa, "Justificativa", 10m, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho("Motorista", "Motorista", 3.5m, Models.Grid.Align.left, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.ObsRecebimentoFísico, "ObservacaoRecebimentoFisico", 10m, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Usuario, "Usuario", 3m, Models.Grid.Align.left, true, false);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.UFOrigem, "UFOrigem", 2.5m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.UFDestino, "UFDestino", 2.5m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.Veiculo, "Veiculo", 3.5m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.Frota, "Frota", 3.5m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Motorista, "Motorista", 3.5m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.TelMotorista, "TelefoneMotorista", 3.5m, Models.Grid.Align.left, false, false, false, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.ChaveCTe, "ChaveCTe", 7m, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.ValorCTe, "ValorCTe", 3m, Models.Grid.Align.right, false, TipoSumarizacao.sum, false);
            }
            else
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.UFDestino, "UFDestino", 2.5m, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("CT-e", "NumeroCTe", 3.5m, Models.Grid.Align.left, false, false, false, false, false);

            if (ConfiguracaoEmbarcador.UtilizaPgtoCanhoto && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.SituacaoPgto, "DescricaoSituacaoPagamento", 3m, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.OrigemDigitalizacao, "DescricaoOrigemDigitalizacao", 5m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.DataConfirmaçaoEntregaTransportador, "DescricaoDataConfirmacaoEntregaTransportador", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.DataConfirmacaoEntrega, "DescricaoDataConfirmacaoEntrega", 3m, Models.Grid.Align.left, false, false);

            if (await repTipoIntegracao.ExistePorTipoAsync(TipoIntegracao.Riachuelo))
                grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.DataIntegracaoEntrega, "DataIntegracaoEntregaFormatada", 3m, Models.Grid.Align.left, true, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.ResponsavelDigitalizacao, "ResponsavelDigitalizacao", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.ResponsavelEnvioFisico, "ResponsavelEnvioFisico", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.ResponsavelLiberacaoPagamento, "ResponsavelLiberacaoPagamento", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.DataLiberaçaoPagamento, "DataLiberacaoPagamentoFormatada", 3m, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.Protocolo, "Protocolo", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.DataMalote, "DataMaloteFormatada", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.NLoteLiberado, "NumeroLoteLiberado", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.NLoteBloqueado, "NumeroLoteBloqueado", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.Operador, "Operador", 3m, Models.Grid.Align.left, false, false);

            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.Tomador, "Tomador", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.CPFCNPJTomador, "CPFCNPJTomadorFormatado", 3m, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.TipoDoCTe, "TipoCTe", 3m, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.PlacaVeiculoResponsavelEntrega, "PlacaVeiculoResponsavelEntrega", 3m, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.DataDoHistorico, "DataHistorico", 3m, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.DataEntregaNoCliente, "DataEntregaNotaClienteFormatada", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Origem, "Origem", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Destino, "Destino", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.RazaoSocialExpedidor, "RazaoExpedidor", 7m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.NomeFantasiaExpedidor, "NomeFantasiaExpedidor", 7m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.ValorFreteNF, "ValorFreteNF", 2.5m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.EnderecoOrigem, "EnderecoDeOrigem", 2.5m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.EnderecoDestino, "EnderecoDeDestino", 2.5m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.CPFMotorista, "CPFMotorista", 2.5m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.CodigoTransportadora, "CodigoDaTransportadora", 2.5m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.CodigoDestino, "CodigoDestino", 2.5m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.DataDigitacaoAprovacaoCD, "DataDigitacaoAprovacao", 2.5m, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.DataRecebimentoFisico, "DataRecebimentoFisico", 2.5m, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.DataRecebimento, "DataRecebimentoFormatada", 2.5m, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.NumeroProtocolo, "NumeroProtocolo", 2.5m, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.Malote, "MaloteProtocolo", 2.5m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.DataEmissaoCte, "DataEmissaoCteFormatada", 2.5m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.SituacaoNotaFiscal, "SituacaoNotaFiscalDescricao", 2.5m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.DataAlteracao, "DataAlteracaoFormatada", 6m, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.CodigoRastreio, "CodigoRastreio", 4m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.ValidacaoViaOCR, "ValidacaoViaOCRFormatado", 4m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.SituacaoViagem, "SituacaoViagemFormatada", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.CentroResultadoCarga, "CentroResultadoCarga", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.MDFesCarga, "MDFesCarga", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.DataEmissaoMdfe, "DescricaoDataEmissaoMdfe", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.SerieCTe, "SerieCTe", 3m, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Canhotos.Canhoto.OrigemCarga, "OrigemDaCarga", 3m, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        protected override async Task<Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto> ObterFiltrosPesquisaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
            List<int> empresas = Request.GetListParam<int>("Empresa");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                empresas = new List<int>() { Empresa.Codigo };

            double recebedor = Request.GetDoubleParam("Recebedor");

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            codigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            codigosFilial.AddRange(ObterListaCodigoFilialPermitidasOperadorCanhoto(unitOfWork));

            List<int> codigosTiposOperacao = Request.GetListParam<int>("TipoOperacao");
            codigosTiposOperacao.AddRange(ObterListaCodigoTipoOperacaoPermitidosOperadorCanhoto(unitOfWork));

            List<int> codigosTiposCarga = Request.GetListParam<int>("TipoCarga");
            codigosTiposCarga.AddRange(ObterListaCodigoTipoCargaPermitidosOperadorCanhoto(unitOfWork));

            Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtroPesquisaCanhoto = new Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto
            {
                TipoCanhoto = Request.GetNullableEnumParam<TipoCanhoto>("TipoCanhoto"),
                OrigemDigitalizacao = Request.GetEnumParam<CanhotoOrigemDigitalizacao>("OrigemDigitalizacao"),
                Situacoes = Request.GetListEnumParam<SituacaoCanhoto>("SituacaoCanhoto"),
                SituacaoDigitalizacaoCanhoto = Request.GetNullableEnumParam<SituacaoDigitalizacaoCanhoto>("SituacaoDigitalizacaoCanhoto"),
                CodigosCargaEmbarcador = Request.GetListParam<int>("CodigoCargaEmbarcador"),
                Motorista = Request.GetIntParam("Motorista"),
                Pessoa = Request.GetStringParam("Emitente").ObterSomenteNumeros().ToDouble(),
                GrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                Chave = Request.GetStringParam("Chave"),
                Numeros = Request.GetListParam<int>("Numero"),
                Serie = Request.GetIntParam("Serie"),
                Filiais = codigosFilial,
                Terceiro = Request.GetStringParam("Terceiro").ObterSomenteNumeros().ToDouble(),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                CodigoLocalArmazenamento = Request.GetIntParam("LocalArmazenamento"),
                Pacote = Request.GetIntParam("Pacote"),
                Posicao = Request.GetIntParam("Posicao"),
                TiposOperacao = codigosTiposOperacao,
                TiposCarga = codigosTiposCarga,
                Recebedor = recebedor,
                Recebedores = recebedor == 0 ? ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork) : new List<double>() { recebedor },
                Destinatario = Request.GetListParam<double>("Destinatario"),
                DataEmissaoCTeInicial = Request.GetNullableDateTimeParam("DataEmissaoCTeInicial"),
                DataEmissaoCTeFinal = Request.GetNullableDateTimeParam("DataEmissaoCTeFinal"),
                SituacaoPgtoCanhoto = Request.GetNullableEnumParam<SituacaoPgtoCanhoto>("SituacaoPgtoCanhoto"),
                DataInicioDigitalizacao = Request.GetNullableDateTimeParam("DataInicioDigitalizacao"),
                DataFimDigitalizacao = Request.GetNullableDateTimeParam("DataFimDigitalizacao"),
                DataCriacaoCargaInicial = Request.GetNullableDateTimeParam("DataCriacaoCargaInicial"),
                DataCriacaoCargaFinal = Request.GetNullableDateTimeParam("DataCriacaoCargaFinal"),
                DataInicioEnvio = Request.GetNullableDateTimeParam("DataInicioEnvio"),
                DataFimEnvio = Request.GetNullableDateTimeParam("DataFimEnvio"),
                Usuario = Request.GetIntParam("Usuario"),
                PlacaVeiculoResponsavelEntrega = Request.GetStringParam("PlacaVeiculoResponsavelEntrega"),
                Transportador = this.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Empresa?.Codigo ?? 0 : 0,
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                SituacaoHistorico = Request.GetNullableEnumParam<SituacaoCanhoto>("SituacaoHistorico"),
                DataInicialHistorico = Request.GetNullableDateTimeParam("DataInicialHistorico"),
                DataFinalHistorico = Request.GetNullableDateTimeParam("DataFinalHistorico"),
                CodigoLocalidadeOrigem = Request.GetIntParam("Origem"),
                CodigoLocalidadeDestino = Request.GetIntParam("Destino"),
                CnpjExpedidor = Request.GetDoubleParam("Expedidor"),
                CodigoGrupoPessoaTomador = Request.GetIntParam("GrupoPessoaTomador"),
                CodigoMalote = Request.GetIntParam("Malote"),
                SituacaoViagem = Request.GetNullableEnumParam<StatusViagemControleEntrega>("SituacaoViagem"),
            };


            filtroPesquisaCanhoto.Empresas = (Usuario?.Empresa?.Codigo ?? 0) > 0 && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? await repositorioEmpresa.BuscarCodigoMatrizEFiliaisAsync(Usuario.Empresa?.CNPJ_SemFormato) : Request.GetListParam<int>("Empresa");

            return filtroPesquisaCanhoto;
        }

        protected override FiltroPesquisaCanhoto ObterFiltrosPesquisa(UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            List<int> empresas = Request.GetListParam<int>("Empresa");

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                empresas = new List<int>() { Empresa.Codigo };

            double recebedor = Request.GetDoubleParam("Recebedor");

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            codigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            codigosFilial.AddRange(ObterListaCodigoFilialPermitidasOperadorCanhoto(unitOfWork));

            List<int> codigosTiposOperacao = Request.GetListParam<int>("TipoOperacao");
            codigosTiposOperacao.AddRange(ObterListaCodigoTipoOperacaoPermitidosOperadorCanhoto(unitOfWork));

            List<int> codigosTiposCarga = Request.GetListParam<int>("TipoCarga");
            codigosTiposCarga.AddRange(ObterListaCodigoTipoCargaPermitidosOperadorCanhoto(unitOfWork));

            Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto filtroPesquisaCanhoto = new Dominio.ObjetosDeValor.Embarcador.Canhoto.FiltroPesquisaCanhoto
            {
                TipoCanhoto = Request.GetNullableEnumParam<TipoCanhoto>("TipoCanhoto"),
                OrigemDigitalizacao = Request.GetEnumParam<CanhotoOrigemDigitalizacao>("OrigemDigitalizacao"),
                Situacoes = Request.GetListEnumParam<SituacaoCanhoto>("SituacaoCanhoto"),
                SituacaoDigitalizacaoCanhoto = Request.GetNullableEnumParam<SituacaoDigitalizacaoCanhoto>("SituacaoDigitalizacaoCanhoto"),
                CodigosCargaEmbarcador = Request.GetListParam<int>("CodigoCargaEmbarcador"),
                Motorista = Request.GetIntParam("Motorista"),
                Pessoa = Request.GetStringParam("Emitente").ObterSomenteNumeros().ToDouble(),
                GrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                Chave = Request.GetStringParam("Chave"),
                Numeros = Request.GetListParam<int>("Numero"),
                Serie = Request.GetIntParam("Serie"),
                Filiais = codigosFilial,
                Terceiro = Request.GetStringParam("Terceiro").ObterSomenteNumeros().ToDouble(),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                CodigoLocalArmazenamento = Request.GetIntParam("LocalArmazenamento"),
                Pacote = Request.GetIntParam("Pacote"),
                Posicao = Request.GetIntParam("Posicao"),
                TiposOperacao = codigosTiposOperacao,
                TiposCarga = codigosTiposCarga,
                Recebedor = recebedor,
                Recebedores = recebedor == 0 ? ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork) : new List<double>() { recebedor },
                Destinatario = Request.GetListParam<double>("Destinatario"),
                DataEmissaoCTeInicial = Request.GetNullableDateTimeParam("DataEmissaoCTeInicial"),
                DataEmissaoCTeFinal = Request.GetNullableDateTimeParam("DataEmissaoCTeFinal"),
                SituacaoPgtoCanhoto = Request.GetNullableEnumParam<SituacaoPgtoCanhoto>("SituacaoPgtoCanhoto"),
                DataInicioDigitalizacao = Request.GetNullableDateTimeParam("DataInicioDigitalizacao"),
                DataFimDigitalizacao = Request.GetNullableDateTimeParam("DataFimDigitalizacao"),
                DataCriacaoCargaInicial = Request.GetNullableDateTimeParam("DataCriacaoCargaInicial"),
                DataCriacaoCargaFinal = Request.GetNullableDateTimeParam("DataCriacaoCargaFinal"),
                DataInicioEnvio = Request.GetNullableDateTimeParam("DataInicioEnvio"),
                DataFimEnvio = Request.GetNullableDateTimeParam("DataFimEnvio"),
                Usuario = Request.GetIntParam("Usuario"),
                PlacaVeiculoResponsavelEntrega = Request.GetStringParam("PlacaVeiculoResponsavelEntrega"),
                Transportador = this.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Empresa?.Codigo ?? 0 : 0,
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                SituacaoHistorico = Request.GetNullableEnumParam<SituacaoCanhoto>("SituacaoHistorico"),
                DataInicialHistorico = Request.GetNullableDateTimeParam("DataInicialHistorico"),
                DataFinalHistorico = Request.GetNullableDateTimeParam("DataFinalHistorico"),
                CodigoLocalidadeOrigem = Request.GetIntParam("Origem"),
                CodigoLocalidadeDestino = Request.GetIntParam("Destino"),
                CnpjExpedidor = Request.GetDoubleParam("Expedidor"),
                CodigoGrupoPessoaTomador = Request.GetIntParam("GrupoPessoaTomador"),
                CodigoMalote = Request.GetIntParam("Malote"),
                SituacaoViagem = Request.GetNullableEnumParam<StatusViagemControleEntrega>("SituacaoViagem"),
            };


            filtroPesquisaCanhoto.Empresas = (Usuario?.Empresa?.Codigo ?? 0) > 0 && TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? repositorioEmpresa.BuscarCodigoMatrizEFiliais(Usuario.Empresa?.CNPJ_SemFormato) : Request.GetListParam<int>("Empresa");

            return filtroPesquisaCanhoto;
        }


        #endregion
    }
}
