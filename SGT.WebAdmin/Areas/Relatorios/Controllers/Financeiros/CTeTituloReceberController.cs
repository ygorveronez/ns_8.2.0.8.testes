using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/CTeTituloReceber", "Financeiros/DocumentosConciliacao")]
    public class CTeTituloReceberController : BaseController
    {
		#region Construtores

		public CTeTituloReceberController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R069_CTeTituloReceber;

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de CT-es a Receber", "Financeiros", "CTeTituloReceber.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "", "", codigo, unitOfWork, false, true, 8);
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

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCTeTituloReceber filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.CTeTituloReceber svcRelatorioCTeTituloReceber = new Servicos.Embarcador.Relatorios.Financeiros.CTeTituloReceber(unitOfWork, TipoServicoMultisoftware, Cliente);

                svcRelatorioCTeTituloReceber.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.CTeTituloReceber> listaCTeTituloReceber, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaCTeTituloReceber);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
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
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCTeTituloReceber filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
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

        #endregion

        #region Métodos de Integração

        // INTEGRACAO CTE TITULOS A RECEBER MARFRIG (visível quando possuir integracao marfrig)
        [AllowAuthenticate]
        public async Task<IActionResult> IntegrarTitulo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceber repIntegracaoMarfrigCteTitulo = new Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceber(unidadeTrabalho);
                Servicos.Embarcador.Integracao.Marfrig.IntegracaoMarfrig servicoMarfrig = new Servicos.Embarcador.Integracao.Marfrig.IntegracaoMarfrig(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber integracaoMarfigCteTituloReceber = repIntegracaoMarfrigCteTitulo.BuscarPorCodigoCTe(codigo);
                //manual sempre vai consultar
                if (integracaoMarfigCteTituloReceber != null)
                {
                    integracaoMarfigCteTituloReceber.Situacao = SituacaoIntegracao.AgIntegracao;
                    repIntegracaoMarfrigCteTitulo.Atualizar(integracaoMarfigCteTituloReceber);
                }
                else
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCtE = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);

                    //ainda nao existe vamos criar e consultar;
                    integracaoMarfigCteTituloReceber = new Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber();
                    integracaoMarfigCteTituloReceber.CTe = repCtE.BuscarPorCodigo(codigo);
                    integracaoMarfigCteTituloReceber.NumeroTentativas = 0;
                    integracaoMarfigCteTituloReceber.DataConsulta = DateTime.Now;
                    integracaoMarfigCteTituloReceber.DataCadastro = DateTime.Now;
                    integracaoMarfigCteTituloReceber.Situacao = SituacaoIntegracao.AgIntegracao;
                    integracaoMarfigCteTituloReceber.Retorno = "";

                    repIntegracaoMarfrigCteTitulo.Inserir(integracaoMarfigCteTituloReceber);
                }

                servicoMarfrig.IntegrarCTeTituloReceber(integracaoMarfigCteTituloReceber, true, this.Usuario);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracaoMarfigCteTituloReceber, null, "Enviou integração", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                unidadeTrabalho.Rollback();

                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar reenviar o arquivo para integração.");
            }
            finally
            {

                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceber repIntegracaoMarfrigCteTitulo = new Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceber(unidadeDeTrabalho);
                Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceberArquivos repIntegracaoMarfrigCteTituloArquivos = new Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceberArquivos(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 60, Models.Grid.Align.left, false);


                Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber integracao = repIntegracaoMarfrigCteTitulo.BuscarPorCodigoCTe(codigo);
                List<Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfrigCteTituloReceberArquivo> integracoesArquivos = repIntegracaoMarfrigCteTituloArquivos.BuscarArquivosPorIntergacao(integracao != null ? integracao.Codigo : 0, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(integracoesArquivos.Count());

                var retorno = (from obj in integracoesArquivos
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);

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

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoIntegracao = 0;
                int.TryParse(Request.Params("CodigoIntegracao"), out codigoIntegracao);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceberArquivos repIntegracaoMarfrigCteTituloArquivo = new Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceberArquivos(unidadeDeTrabalho);
                Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceber repIntegracaoMarfrigCteTitulo = new Repositorio.Embarcador.Integracao.IntegracaoMarfrigCteTitulosReceber(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfigCteTituloReceber integracao = repIntegracaoMarfrigCteTitulo.BuscarPorCodigoCTe(codigoIntegracao);
                if (integracao == null)
                {
                    return new JsonpResult(true, false, "Histórico não encontrado.");
                }


                Dominio.Entidades.Embarcador.Integracao.IntegracaoMarfrigCteTituloReceberArquivo arquivoIntegracao = repIntegracaoMarfrigCteTituloArquivo.BuscarPorCodigo(codigo, false);

                if (arquivoIntegracao == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");


                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivo Integração Titulos " + integracao.CTe.Descricao + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do arquivo de integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número CT-e", "Numero", 8, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Série CT-e", "Serie", 8, Models.Grid.Align.center, false, false, false, false, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true, false, false, true, true);
                grid.AdicionarCabecalho("CNPJ Transportador", "CNPJTransportador", 15, Models.Grid.Align.left, false, false, false, false, false);
            }

            grid.AdicionarCabecalho("Origem", "Origem", 20, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Destino", "Destino", 20, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Remetente", "Remetente", 20, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 20, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data de Emissão", "DescricaoDataEmissao", 15, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data de Vencimento", "DescricaoDataVencimento", 15, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data do Pagamento", "DescricaoDataLiquidacao", 15, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Situação Título", "DescricaoSituacao", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Valor a Receber", "ValorAReceber", 10, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Status", "StatusFormatada", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Peso", "Peso", 20, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Nota Fiscal", "NotaFiscal", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Fatura", "NumeroFatura", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Carga", "NumeroCarga", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Acréscimos", "Acrescimos", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Decréscimos", "Decrescimos", 20, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Chave CTe", "ChaveCTe", 20, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCTeTituloReceber ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCTeTituloReceber()
            {
                NumeroCTe = Request.GetIntParam("NumeroCTe"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Empresa.Codigo : Request.GetIntParam("Empresa"),
                StatusTitulo = Request.GetEnumParam<StatusTitulo>("StatusTitulo"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                SomenteTitulosLiberados = false,// TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? true : false,
                CodigosFiliais = Request.GetIntParam("Filial") == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : new List<int>() { Request.GetIntParam("Filial") },
                CnpjCpfRemetente = Request.GetDoubleParam("Remetente"),
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                TipoServicoMultisoftware = TipoServicoMultisoftware,
                NumeroFatura = Request.GetIntParam("NumeroFatura")
            };
        }

        #endregion
    }
}
