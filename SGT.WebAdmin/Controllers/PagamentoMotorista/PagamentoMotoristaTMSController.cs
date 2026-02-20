using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.PagamentoMotorista
{
    [CustomAuthorize("PagamentosMotoristas/PagamentoMotoristaTMS", "Financeiros/MovimentoFinanceiro")]
    public class PagamentoMotoristaTMSController : BaseController
    {
		#region Construtores

		public PagamentoMotoristaTMSController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais        

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                // Retorna Dados
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaAdiantamentoAgregado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);

                int codigoMotorista, codigoPagamentoAgregado, numero;
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("PagamentoAgregado"), out codigoPagamentoAgregado);
                int.TryParse(Request.Params("Numero"), out numero);

                double cliente;
                double.TryParse(Request.Params("Cliente"), out cliente);

                DateTime dataPagamento, dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataPagamento"), out dataPagamento);
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoAdiantamento", false);
                grid.AdicionarCabecalho("CodigoPagamento", false);
                grid.AdicionarCabecalho("CodigoAdiantamentoPagamentoAgregado", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "Valor", 12, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data", "Data", 12, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Motorista", "Motorista", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("GerarTituloPagar", false);
                grid.AdicionarCabecalho("MoedaCotacaoBancoCentral", false);
                grid.AdicionarCabecalho("DataBaseCRT", false);
                grid.AdicionarCabecalho("ValorMoedaCotacao", false);
                grid.AdicionarCabecalho("ValorOriginalMoedaEstrangeira", false);
                grid.AdicionarCabecalho("DesabilitarAlteracaoDosPlanosDeContas", false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                var dynListaCarga = repPagamentoMotoristaTMS.BuscarPorAdiantamentoAgregado(numero, dataPagamento, dataFinal, codigoMotorista, cliente, codigoPagamentoAgregado, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                var dynRetorno = (from obj in dynListaCarga
                                  select new
                                  {
                                      Codigo = obj.Codigo,
                                      CodigoAdiantamentoPagamentoAgregado = 0,
                                      CodigoAdiantamento = obj.Codigo,
                                      CodigoPagamento = codigoPagamentoAgregado,
                                      Numero = obj.Numero.ToString("n0"),
                                      Valor = obj.Valor.ToString("n2"),
                                      obj.PagamentoMotoristaTipo.Descricao,
                                      Data = obj.DataPagamento.ToString("dd/MM/yyyy HH:mm"),
                                      Motorista = obj.Motorista.Nome,
                                      obj.MoedaCotacaoBancoCentral,
                                      DataBaseCRT = obj.DataBaseCRT.HasValue ? obj.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                      ValorMoedaCotacao = obj.ValorMoedaCotacao.ToString("n10"),
                                      ValorOriginalMoedaEstrangeira = obj.ValorOriginalMoedaEstrangeira.ToString("n2"),
                                      DesabilitarAlteracaoDosPlanosDeContas = obj.PagamentoMotoristaTipo?.DesabilitarAlteracaoDosPlanosDeContas ?? false,
                                      GerarTituloPagar = obj.PagamentoMotoristaTipo?.GerarTituloPagar ?? false,
                                  }).ToList();

                grid.setarQuantidadeTotal(repPagamentoMotoristaTMS.ContarBuscarPorAdiantamentoAgregado(numero, dataPagamento, dataFinal, codigoMotorista, cliente, codigoPagamentoAgregado));
                grid.AdicionaRows(dynRetorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPendenciaMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                int codigoMotorista, codigoPagamentoAgregado, numero;
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("PagamentoAgregado"), out codigoPagamentoAgregado);
                int.TryParse(Request.Params("Numero"), out numero);
                bool.TryParse(Request.Params("Pendente"), out bool pendente);
                double cliente;
                double.TryParse(Request.Params("Cliente"), out cliente);

                DateTime dataPagamento, dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataPagamento"), out dataPagamento);
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);
                Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista situacao);

                // Busca Dados
                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                // Consulta
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> listaGrid = repPagamentoMotoristaTMS.BuscarPagamentoPendenciaMotorista(numero, dataPagamento, dataFinal, codigoMotorista, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite, pendente, true, situacao);

                var lista = (from p in listaGrid
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 DataPagamento = p.DataPagamento.ToString("dd/MM/yyyy HH:mm"),
                                 CodigoCargaEmbarcador = p.Carga?.CodigoCargaEmbarcador,
                                 Motorista = p.Motorista?.Nome + " (" + p.Motorista?.CPF_Formatado + ")",
                                 PagamentoMotoristaTipo = p.PagamentoMotoristaTipo?.Descricao,
                                 Valor = p.Valor.ToString("n2"),
                                 Saldo = p.TotalPagamento(ConfiguracaoEmbarcador.NaoDescontarValorSaldoMotorista).ToString("n2"),
                                 p.DescricaoSituacao,
                                 p.StatusFinanceiro,
                                 p.DescricaoEtapa,
                                 GerarTituloPagar = p.PagamentoMotoristaTipo?.GerarTituloPagar ?? false,
                                 p.MoedaCotacaoBancoCentral,
                                 DataBaseCRT = p.DataBaseCRT.HasValue ? p.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                 ValorMoedaCotacao = p.ValorMoedaCotacao.ToString("n10"),
                                 ValorOriginalMoedaEstrangeira = p.ValorOriginalMoedaEstrangeira.ToString("n2"),
                                 DesabilitarAlteracaoDosPlanosDeContas = p.PagamentoMotoristaTipo?.DesabilitarAlteracaoDosPlanosDeContas ?? false,
                                 Chamado = p.Chamado?.Numero
                             }).ToList();

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(repPagamentoMotoristaTMS.ContarBuscarPagamentoPendenciaMotorista(numero, dataPagamento, dataFinal, codigoMotorista, pendente, true, situacao));
                // Retorna Dados
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno repPagamentoMotoristaIntegracaoRetorno = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno(unidadeDeTrabalho);

                int.TryParse(Request.Params("Codigo"), out int codigo);


                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "DescricaoRetorno", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Código Retorno", "CodigoRetorno", 10, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno> integracoesArquivos = repPagamentoMotoristaIntegracaoRetorno.ConsultarPorEnvio(codigo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repPagamentoMotoristaIntegracaoRetorno.ContarConsultaPorEnvio(codigo));

                var retorno = (from obj in integracoesArquivos
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),                                   
                                   obj.DescricaoRetorno,
                                   obj.CodigoRetorno
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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno repPagamentoMotoristaIntegracaoRetorno = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno pagamentoMotoristaIntegracaoRetorno = repPagamentoMotoristaIntegracaoRetorno.BuscarPorCodigo(codigo);

                if (pagamentoMotoristaIntegracaoRetorno == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                if (pagamentoMotoristaIntegracaoRetorno.ArquivoRequisicao == null && pagamentoMotoristaIntegracaoRetorno.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { pagamentoMotoristaIntegracaoRetorno.ArquivoRequisicao, pagamentoMotoristaIntegracaoRetorno.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public String empresaDescricao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Empresa Empresa, Dominio.Entidades.Embarcador.Cargas.Carga carga) {
            Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento = repAgendamentoColeta.BuscarPorCarga(carga.Codigo);
            return string.IsNullOrWhiteSpace(carga.DadosSumarizados?.PortalRetiraEmpresa) ? string.IsNullOrWhiteSpace(agendamento?.TransportadorManual) ? $"{carga.Empresa?.CodigoIntegracao ?? string.Empty} {carga.Empresa?.RazaoSocial} ({carga.Empresa?.Localidade.DescricaoCidadeEstado})" : agendamento.TransportadorManual : carga.DadosSumarizados.PortalRetiraEmpresa;
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterEmpresa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                

                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);


                var retorno = new
                {
                    Codigo = carga.Empresa?.Codigo ?? 0,
                    Descricao = this.empresaDescricao(unitOfWork, carga.Empresa, carga)
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter a empresa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int.TryParse(Request.Params("Carga"), out int codigoCarga);
                int.TryParse(Request.Params("Chamado"), out int codigoChamado);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;
                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = null;
                Dominio.Entidades.Usuario motorista = null;

                if (codigoCarga > 0)
                    carga = repCarga.BuscarPorCodigo(codigoCarga);
                else if (codigoChamado > 0)
                    chamado = repChamado.BuscarPorCodigo(codigoChamado);

                if (carga != null && carga.Motoristas != null && carga.Motoristas.Count > 0)
                    motorista = carga.Motoristas.FirstOrDefault();
                if (chamado != null && chamado.Carga != null && chamado.Carga.Motoristas != null && chamado.Carga.Motoristas.Count > 0)
                    motorista = carga.Motoristas.FirstOrDefault();

                if (motorista != null && motorista.Status != "A")
                    motorista = null;

                var retorno = new
                {
                    Codigo = motorista?.Codigo ?? 0,
                    Nome = motorista?.Nome ?? string.Empty
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter o motorista para o pagamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("PagamentosMotoristas/PagamentoMotoristaTMS");

                unitOfWork.Start();

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unitOfWork);
                Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repConfiguracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro repConfiguracaoFinanceiro = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFinanceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracaoMotorista = repConfiguracaoMotorista.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFinanceiro configuracaoFinanceiro = repConfiguracaoFinanceiro.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS();

                PreencherEntidadeComRequest(pagamentoMotorista, unitOfWork);

                if (configuracaoMotorista?.NaoValidarHoraNoPagamentoMotorista ?? false)
                {
                    if (pagamentoMotorista.DataPagamento.Date < DateTime.Now.Date && !Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PagamentoMotorista_PermiteInformarDataPagamentoRetroativa) && !pagamentoMotorista.PagamentoMotoristaTipo.PermitirLancarComDataRetroativa)
                        throw new ControllerException("Você não possui permissão para informar a Data do Pagamento retroativa.");
                }
                else if (pagamentoMotorista.DataPagamento.AddSeconds(59) < DateTime.Now && !Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PagamentoMotorista_PermiteInformarDataPagamentoRetroativa) && !pagamentoMotorista.PagamentoMotoristaTipo.PermitirLancarComDataRetroativa)
                    throw new ControllerException("Você não possui permissão para informar a Data do Pagamento retroativa.");

                TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotorista.PagamentoMotoristaTipo?.TipoIntegracaoPagamentoMotorista ?? TipoIntegracaoPagamentoMotorista.SemIntegracao;

                if (pagamentoMotorista.PagamentoMotoristaTipo != null && pagamentoMotorista.PagamentoMotoristaTipo.GerarMovimentoAutomatico && (pagamentoMotorista.PlanoDeContaCredito == null || pagamentoMotorista.PlanoDeContaDebito == null))
                    throw new ControllerException("Favor selecione os Planos de Contas para este Tipo de Pagamento.");

                if (pagamentoMotorista.PagamentoMotoristaTipo != null && pagamentoMotorista.Motorista != null && pagamentoMotorista.PagamentoMotoristaTipo.PermitirLancarPagamentoContendoAcertoEmAndamento)
                {
                    if (!repAcertoViagem.ContemAcertoEmAndamento(pagamentoMotorista.Motorista.Codigo))
                        throw new ControllerException("Não foi localizado nenhum Acerto de Viagem com o status Em Andamento para este motorista.");
                }

                if ((pagamentoMotorista?.PagamentoMotoristaTipo?.ReterImpostoPagamentoMotorista ?? false) && pagamentoMotorista.Carga == null)
                {
                    throw new ControllerException("Favor selecione uma Carga para este Tipo de Pagamento.");
                }

                if (!ConfiguracaoEmbarcador.PermitirPagamentoMotoristaSemCarga && pagamentoMotorista.Carga == null && pagamentoMotorista.PagamentoMotoristaTipo != null
                    && (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCard || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PagBem || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Extratta
                        || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Target || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.RepomFrete))
                    throw new ControllerException("Favor selecione uma Carga para este Tipo de Pagamento.");

                if (ConfiguracaoEmbarcador.PermitirPagamentoMotoristaSemCarga && pagamentoMotorista.Motorista != null && pagamentoMotorista.Carga == null && pagamentoMotorista.PagamentoMotoristaTipo != null &&
                    (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCard || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PagBem || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Extratta
                    || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Target || tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.RepomFrete))
                {
                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorMotorista(pagamentoMotorista.Motorista.Codigo, "0");
                    if (veiculo == null)
                        throw new ControllerException("O motorista selecionado não está vinculado a nenhum veículo do tipo tração.");
                }

                if (repPagamentoMotorista.ContemPagamentoEmAberto(pagamentoMotorista.Motorista.Codigo))
                    throw new ControllerException("Já existe um pagamento em aberto para este motorista, favor finalize o mesmo antes de iniciar um novo.");

                if (repPagamentoMotorista.ContemPagamentoIdentico(pagamentoMotorista.DataPagamento.Date, pagamentoMotorista.Motorista.Codigo, pagamentoMotorista.PagamentoMotoristaTipo.Codigo, pagamentoMotorista.Valor))
                    throw new ControllerException("Já existe um pagamento com a mesma Data do Pagamento, Motorista, Tipo do Pagamento e Valor.");

                Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.CalcularImpostos(ref pagamentoMotorista, unitOfWork, TipoServicoMultisoftware);

                repPagamentoMotorista.Inserir(pagamentoMotorista, Auditado);

                if (Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarRegrasAutorizacaoPagamentoMotorista(pagamentoMotorista, TipoServicoMultisoftware, unitOfWork, this.Usuario, _conexao.StringConexao, Auditado, out bool contemAprovadorIgualAoOperador))
                {
                    pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AutorizacaoPendente;
                    pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.AgAutorizacao;
                }
                else
                {
                    pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AgIntegracao;
                    pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Integracao;
                }

                if (contemAprovadorIgualAoOperador)
                {
                    Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);
                    Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao pagamentoMotoristaAutorizacao = repPagamentoMotoristaAutorizacao.BuscarPrimeiroPorPagamentoUsuario(pagamentoMotorista.Codigo, pagamentoMotorista.Usuario.Codigo);

                    Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.EfetuarAprovacao(pagamentoMotoristaAutorizacao, pagamentoMotorista.Usuario, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                    string msgRetornoSit = "";
                    Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarSituacaoPagamento(pagamentoMotoristaAutorizacao.PagamentoMotoristaTMS, unitOfWork, ref msgRetornoSit, TipoServicoMultisoftware, Auditado, _conexao.StringConexao, ConfiguracaoEmbarcador, pagamentoMotorista.Usuario);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamentoMotorista, null, "Aprovou o pagamento pelo mesmo operadora da alçada.", unitOfWork);
                }

                Repositorio.Embarcador.Configuracoes.IntegracaoKMM repositorioIntegracaoKMM = new Repositorio.Embarcador.Configuracoes.IntegracaoKMM(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoKMM configuracaoIntegracaoKMM = repositorioIntegracaoKMM.BuscarPrimeiroRegistro();

                string msgRetorno = "";
                if (tipoIntegracaoPagamentoMotorista.PossuiIntegracao())
                {
                    Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoMotoristaIntegracaoEnvio = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio();
                    pagamentoMotoristaIntegracaoEnvio.Data = DateTime.Now.Date;
                    pagamentoMotoristaIntegracaoEnvio.NumeroTentativas = 0;
                    pagamentoMotoristaIntegracaoEnvio.PagamentoMotoristaTMS = pagamentoMotorista;
                    pagamentoMotoristaIntegracaoEnvio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    pagamentoMotoristaIntegracaoEnvio.TipoIntegracaoPagamentoMotorista = tipoIntegracaoPagamentoMotorista;

                    repPagamentoMotoristaIntegracaoEnvio.Inserir(pagamentoMotoristaIntegracaoEnvio);

                    if (configuracaoIntegracaoKMM?.PossuiIntegracao ?? false)
                        Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotorista, unitOfWork);
                }
                else if (pagamentoMotorista.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.AgIntegracao)
                {
                    if (configuracaoIntegracaoKMM?.PossuiIntegracao ?? false)
                    {
                        Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS.AdicionarIntegracaoKMM(pagamentoMotorista, unitOfWork);
                    }
                    else
                    {
                        pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;
                        pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Integracao;

                        if (ConfiguracaoEmbarcador.ConfirmarPagamentoMotoristaAutomaticamente)
                            Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamentoMotorista.Codigo, ConfiguracaoEmbarcador.TipoMovimentoPagamentoMotorista, Auditado, pagamentoMotorista.Usuario, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware);
                    }
                }

                if (pagamentoMotorista.Motorista.TipoMotorista == TipoMotorista.Terceiro && 
                    (pagamentoMotorista.PagamentoMotoristaTipo.NaoPermitirGerarPagamentoMotoristaTerceiro || configuracaoFinanceiro.NaoGerarPagamentoParaMotoristaTerceiro))
                    throw new ControllerException("Não é permitido gerar pagamento para um motorista do tipo terceiro. Verifique as configurações.");

                unitOfWork.CommitChanges();

                dynamic retorno = retornarPagamentoMotorista(pagamentoMotorista, unitOfWork, msgRetorno);
                return new JsonpResult(retorno);
            }
            catch (BaseException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotoristaTMS = repPagamentoMotoristaTMS.BuscarPorCodigo(codigo);

                if (pagamentoMotoristaTMS == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar os dados.");

                return new JsonpResult(retornarPagamentoMotorista(pagamentoMotoristaTMS, unitOfWork));
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarSaldoDescontadoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista servicoAutorizacaoPagamentoMotorista = new Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista(unitOfWork);

                int codigoMotorista = Request.GetIntParam("Motorista");
                int codigoTipoPagamentoMotorista = Request.GetIntParam("TipoPagamentoMotorista");

                decimal valor = Request.GetDecimalParam("Valor");

                return new JsonpResult(servicoAutorizacaoPagamentoMotorista.ObterSaldoDescontadoMotorista(codigoMotorista, codigoTipoPagamentoMotorista, valor, ConfiguracaoEmbarcador));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o saldo descontado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = GridConsultarAutorizacoes(unitOfWork);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosIntegracoes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPagamentoMotorista = Request.GetIntParam("PagamentoMotorista");

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = repPagamentoMotoristaTMS.BuscarPorCodigo(codigoPagamentoMotorista);
                TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotorista.PagamentoMotoristaTipo?.TipoIntegracaoPagamentoMotorista ?? TipoIntegracaoPagamentoMotorista.SemIntegracao;
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoEnvio = repPagamentoMotoristaIntegracaoEnvio.BuscarPorPagamentoETipoIntegracao(codigoPagamentoMotorista, TipoIntegracaoPagamentoMotorista.KMM);

                return new JsonpResult(new
                {
                    ComIntegracao = tipoIntegracaoPagamentoMotorista.PossuiIntegracao() || pagamentoEnvio != null
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados das integrações.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaEnvio()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista? tipo = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoPagamentoMotorista tipoAux;
                if (Enum.TryParse(Request.Params("Tipo"), out tipoAux))
                    tipo = tipoAux;

                int codigoPagamentoMotorista;
                int.TryParse(Request.Params("PagamentoMotorista"), out codigoPagamentoMotorista);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo Integração", "TipoIntegracaoPagamentoMotorista", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Cancelado", "Cancelado", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "Retorno", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "Data", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("PodeReenviar", false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repIntegracao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio> listaIntegracao = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio>();

                int countCTes = repIntegracao.ContarConsulta(codigoPagamentoMotorista, situacao, tipo);
                listaIntegracao = repIntegracao.Consultar(codigoPagamentoMotorista, situacao, tipo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(countCTes);

                grid.AdicionaRows((from obj in listaIntegracao
                                   select new
                                   {
                                       obj.Codigo,
                                       TipoIntegracaoPagamentoMotorista = obj.DescricaoTipoIntegracaoPagamentoMotorista,
                                       SituacaoIntegracao = obj.DescricaoSituacaoIntegracao,
                                       Cancelado = obj.Cancelado ? "Sim" : "Não",
                                       obj.Retorno,
                                       NumeroTentativas = obj.NumeroTentativas.ToString("n0"),
                                       Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                       DT_RowColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde :
                                                     obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho :
                                                     obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo :
                                                     Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Azul,
                                       DT_FontColor = obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : "",
                                       PodeReenviar = obj.TipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Extratta ? obj.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao : true,
                                   }).ToList());

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
        public async Task<IActionResult> ObterTotaisEnvio()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPagamentoMotorista;
                int.TryParse(Request.Params("PagamentoMotorista"), out codigoPagamentoMotorista);

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unidadeDeTrabalho);

                int totalAguardandoIntegracao = repPagamentoMotoristaIntegracaoEnvio.ContarPorPagamento(codigoPagamentoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repPagamentoMotoristaIntegracaoEnvio.ContarPorPagamento(codigoPagamentoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repPagamentoMotoristaIntegracaoEnvio.ContarPorPagamento(codigoPagamentoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repPagamentoMotoristaIntegracaoEnvio.ContarPorPagamento(codigoPagamentoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno);
                bool podeReenviarTodos = repPagamentoMotoristaIntegracaoEnvio.ContarPorPagamentoDiffSituacao(codigoPagamentoMotorista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, TipoIntegracaoPagamentoMotorista.Extratta) == 0;

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno,
                    PodeReenviarTodos = podeReenviarTodos,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais das integrações de CT-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaRetorno()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPagamentoMotorista;
                int.TryParse(Request.Params("PagamentoMotorista"), out codigoPagamentoMotorista);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Código Retorno", "CodigoRetorno", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "DescricaoRetorno", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "Data", 10, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno repIntegracao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno> listaIntegracao = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno>();

                int countCTes = repIntegracao.ContarConsulta(codigoPagamentoMotorista);
                listaIntegracao = repIntegracao.Consultar(codigoPagamentoMotorista, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(countCTes);

                grid.AdicionaRows((from obj in listaIntegracao
                                   select new
                                   {
                                       obj.Codigo,
                                       obj.CodigoRetorno,
                                       obj.DescricaoRetorno,
                                       Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss")
                                   }).ToList());

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
        public async Task<IActionResult> AtualizarRegrasEtapas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                // Repositoriso
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);

                // Converte parametros
                int codigo = 0;
                int.TryParse(Request.Params("PagamentoMotorista"), out codigo);

                // Busca Ocorrencia
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = repPagamentoMotorista.BuscarPorCodigo(codigo);

                // Valida
                if (pagamentoMotorista == null)
                    return new JsonpResult(false, true, "Pagamento não encontrado.");

                // Verifica qual regras consultar
                bool atualizarOcorrencia = false;
                if (pagamentoMotorista.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.SemRegraAprovacao)
                {
                    // Busca se ha regras e cria
                    if (Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarRegrasAutorizacaoPagamentoMotorista(pagamentoMotorista, TipoServicoMultisoftware, unitOfWork, this.Usuario, _conexao.StringConexao, Auditado, out bool contemAprovadorIgualAoOperador))
                    {
                        atualizarOcorrencia = true;
                        pagamentoMotorista.SituacaoPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AutorizacaoPendente;
                        pagamentoMotorista.EtapaPagamentoMotorista = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista.AgAutorizacao;
                    }

                    if (contemAprovadorIgualAoOperador)
                    {
                        Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);
                        Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao pagamentoMotoristaAutorizacao = repPagamentoMotoristaAutorizacao.BuscarPrimeiroPorPagamentoUsuario(pagamentoMotorista.Codigo, pagamentoMotorista.Usuario.Codigo);

                        Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.EfetuarAprovacao(pagamentoMotoristaAutorizacao, pagamentoMotorista.Usuario, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                        string msgRetornoSit = "";
                        Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarSituacaoPagamento(pagamentoMotoristaAutorizacao.PagamentoMotoristaTMS, unitOfWork, ref msgRetornoSit, TipoServicoMultisoftware, Auditado, _conexao.StringConexao, ConfiguracaoEmbarcador, pagamentoMotorista.Usuario);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamentoMotorista, null, "Aprovou o pagamento pelo mesmo operadora da alçada.", unitOfWork);
                    }
                }

                // Retorno de informacoes
                var retorno = new
                {
                    SituacaoOcorrencia = pagamentoMotorista.SituacaoPagamentoMotorista
                };

                // Atualiza a ocorrencia
                if (atualizarOcorrencia)
                    repPagamentoMotorista.Atualizar(pagamentoMotorista);
                else
                    retorno = null;

                // Finaliza instancia
                unitOfWork.CommitChanges();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar informações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("PagamentoMotorista"), out int codigoPagamentoMotorista);

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = repPagamentoMotorista.BuscarPorCodigo(codigoPagamentoMotorista, true);

                if (pagamentoMotorista == null)
                    return new JsonpResult(false, true, "Pagamento não encontrado.");

                if (pagamentoMotorista.SituacaoPagamentoMotorista != SituacaoPagamentoMotorista.AgIntegracao)
                    return new JsonpResult(false, true, "A situação do pagamento não permite a finalização da etapa.");

                unidadeDeTrabalho.Start();

                pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;

                string msgRetorno = "";
                if (ConfiguracaoEmbarcador.ConfirmarPagamentoMotoristaAutomaticamente)
                {
                    if (!Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamentoMotorista.Codigo, ConfiguracaoEmbarcador.TipoMovimentoPagamentoMotorista, Auditado, pagamentoMotorista.Usuario, unidadeDeTrabalho, _conexao.StringConexao, TipoServicoMultisoftware))
                        return new JsonpResult(false, true, msgRetorno);
                }

                repPagamentoMotorista.Atualizar(pagamentoMotorista);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamentoMotorista, null, "Finalizou Pagamento.", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(new { MensagemRetorno = msgRetorno });
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar a etapa.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> CancelarPagamento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPagamentoMotorista = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = repPagamentoMotorista.BuscarPorCodigo(codigoPagamentoMotorista, true);

                if (pagamentoMotorista == null)
                    return new JsonpResult(false, true, "Pagamento não encontrado.");

                if (pagamentoMotorista.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FinalizadoPagamento)
                    return new JsonpResult(false, true, "A situação do pagamento não permite a cancelar o mesmo.");

                bool naoPermitirCancelamento = false;
                if (pagamentoMotorista.PagamentoMotoristaTipo != null)
                    naoPermitirCancelamento = pagamentoMotorista.PagamentoMotoristaTipo.NaoPermitirCancelamento;

                if (naoPermitirCancelamento && (pagamentoMotorista.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.Finalizada ||
                    (pagamentoMotorista.EtapaPagamentoMotorista != EtapaPagamentoMotorista.Integracao && pagamentoMotorista.SituacaoPagamentoMotorista != SituacaoPagamentoMotorista.FalhaIntegracao)))
                    return new JsonpResult(false, true, "O tipo de pagamento não permite realizar o cancelamento.");

                unidadeDeTrabalho.Start();

                pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Cancelada;

                repPagamentoMotorista.Atualizar(pagamentoMotorista);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamentoMotorista, null, "Cancelou Pagamento.", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao cancelar o pagamento.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ReverterPagamento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoPagamentoMotorista = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = repPagamentoMotorista.BuscarPorCodigo(codigoPagamentoMotorista, true);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("PagamentosMotoristas/PagamentoMotoristaTMS");
                if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PagamentoMotorista_PermiteReverterPagamentoMotorista))
                    return new JsonpResult(false, true, "Não possui permissão para reverter o Pagamento");

                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista servicoAutorizacaoPagamentoMotorista = new Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista(unidadeDeTrabalho);

                servicoAutorizacaoPagamentoMotorista.CancelarIntegracaoPagamentoMotorista(pagamentoMotorista, Usuario, Auditado, TipoServicoMultisoftware);

                unidadeDeTrabalho.Start();

                servicoAutorizacaoPagamentoMotorista.ReverterPagamentoMotorista(pagamentoMotorista, Usuario, Auditado, TipoServicoMultisoftware);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reverter o faturamento.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarPagamento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoPagamentoMotorista);
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                string msgRetorno = "";
                if (!Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, codigoPagamentoMotorista, ConfiguracaoEmbarcador.TipoMovimentoPagamentoMotorista, Auditado, this.Usuario, unidadeDeTrabalho, _conexao.StringConexao, TipoServicoMultisoftware))
                    return new JsonpResult(false, true, msgRetorno);
                else
                {
                    unidadeDeTrabalho.CommitChanges();

                    Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = repPagamentoMotorista.BuscarPorCodigo(codigoPagamentoMotorista);
                    var retorno = retornarPagamentoMotorista(pagamentoMotorista, unidadeDeTrabalho, msgRetorno);
                    return new JsonpResult(retorno);
                }
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao confirmar o pagamento.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadRetorno()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno repIntegracao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoRetorno arquivoIntegracao = repIntegracao.BuscarPorCodigo(codigo);

                if (arquivoIntegracao == null)
                    return new JsonpResult(false, true, "Retorno não encontrado.");

                if (arquivoIntegracao == null || arquivoIntegracao.ArquivoRetorno == null || string.IsNullOrWhiteSpace(arquivoIntegracao.ArquivoRetorno))
                    return new JsonpResult(false, true, "Não há registros de arquivos salvos para este histórico de consulta.");

                MemoryStream arquivo = new MemoryStream();
                StreamWriter writer = new StreamWriter(arquivo);
                writer.Write(arquivoIntegracao.ArquivoRetorno);
                writer.Flush();
                arquivo.Position = 0;

                string extensao = ".json";
                if (arquivoIntegracao.PagamentoMotoristaTMS.PagamentoMotoristaTipo.TipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCard)
                    extensao = ".xml";

                return Arquivo(arquivo, "application/txt", "Arquivo de Retorno " + arquivoIntegracao.PagamentoMotoristaTMS.Numero.ToString() + extensao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do arquivo de retorno.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadEnvio()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repIntegracao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio arquivoIntegracao = repIntegracao.BuscarPorCodigo(codigo);

                if (arquivoIntegracao == null)
                    return new JsonpResult(false, true, "Envio não encontrado.");

                if (arquivoIntegracao == null || arquivoIntegracao.ArquivoEnvio == null || string.IsNullOrWhiteSpace(arquivoIntegracao.ArquivoEnvio))
                    return new JsonpResult(false, true, "Não há registros de arquivos salvos para este histórico de consulta.");

                MemoryStream arquivo = new MemoryStream();
                StreamWriter writer = new StreamWriter(arquivo);
                writer.Write(arquivoIntegracao.ArquivoEnvio);
                writer.Flush();
                arquivo.Position = 0;

                string extensao = ".json";
                if (arquivoIntegracao.PagamentoMotoristaTMS.PagamentoMotoristaTipo.TipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCard)
                    extensao = ".xml";

                return Arquivo(arquivo, "application/txt", "Arquivo de Envio " + arquivoIntegracao.PagamentoMotoristaTMS.Numero.ToString() + extensao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download do arquivo de envio.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unidadeDeTrabalho.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unidadeDeTrabalho);
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoMotoristaIntegracaoEnvio = repPagamentoMotoristaIntegracaoEnvio.BuscarPorCodigo(codigo);
                if (pagamentoMotoristaIntegracaoEnvio == null)
                    return new JsonpResult(false, true, "Integração não encontrada.");

                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento = pagamentoMotoristaIntegracaoEnvio.PagamentoMotoristaTMS;
                if (pagamento == null)
                    return new JsonpResult(false, true, "Nenhum pagamento localizado.");

                TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotoristaIntegracaoEnvio.TipoIntegracaoPagamentoMotorista;
                if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.SemIntegracao)
                    return new JsonpResult(false, true, "Não é possível enviar o pagamento pois o tipo está como sem integração.");

                //if (pagamento.PagamentoMotoristaTipo.TipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.SemIntegracao)
                    //return new JsonpResult(false, true, "Tipo do pagamento não possui integração.");

                if (pagamento.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.AgAprovacao || pagamento.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.AutorizacaoPendente)
                    return new JsonpResult(false, true, "Favor realizar a aprovação do pagamento.");

                if (pagamento.SituacaoPagamentoMotorista != SituacaoPagamentoMotorista.AgIntegracao && pagamento.SituacaoPagamentoMotorista != SituacaoPagamentoMotorista.FalhaIntegracao)
                    if (!(pagamentoMotoristaIntegracaoEnvio.SituacaoIntegracao == SituacaoIntegracao.Integrado && tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Email))
                        return new JsonpResult(false, true, "Situação do pagamento não permite realizar a integração.");

                pagamentoMotoristaIntegracaoEnvio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                pagamentoMotoristaIntegracaoEnvio.NumeroTentativas += 1;

                repPagamentoMotoristaIntegracaoEnvio.Atualizar(pagamentoMotoristaIntegracaoEnvio);

                if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PagBem)
                {
                    Servicos.Embarcador.CIOT.Pagbem svcPagbem = new Servicos.Embarcador.CIOT.Pagbem();
                    svcPagbem.RealizarPagamentoMotorista(pagamento, Auditado, TipoServicoMultisoftware, unidadeDeTrabalho, out string mensagem);
                }
                else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCard)
                {
                    Servicos.Embarcador.PagamentoMotorista.PamCard svcPamCard = new Servicos.Embarcador.PagamentoMotorista.PamCard(unidadeDeTrabalho);
                    svcPamCard.EmitirPagamentoMotorista(pagamento.Codigo, unidadeDeTrabalho, TipoServicoMultisoftware, Auditado);
                }
                else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCardCorporativo)
                {
                    Servicos.Embarcador.PagamentoMotorista.PamCard svcPamCard = new Servicos.Embarcador.PagamentoMotorista.PamCard(unidadeDeTrabalho);
                    svcPamCard.EmitirPagamentoMotoristaPamcardCorporativo(pagamento.Codigo, unidadeDeTrabalho, TipoServicoMultisoftware, Auditado);
                }
                else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Email)
                {
                    Servicos.Embarcador.PagamentoMotorista.IntegracaoEmailPagamentoMotorista svcIntegracaoEmail = new Servicos.Embarcador.PagamentoMotorista.IntegracaoEmailPagamentoMotorista(unidadeDeTrabalho);
                    svcIntegracaoEmail.EnviarEmailPagamentoMotorista(pagamento.Codigo, unidadeDeTrabalho, TipoServicoMultisoftware, Auditado);
                }
                else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Target)
                {
                    Servicos.Embarcador.PagamentoMotorista.Target svcTarget = new Servicos.Embarcador.PagamentoMotorista.Target(unidadeDeTrabalho);
                    svcTarget.EmitirPagamentoMotorista(pagamento.Codigo, TipoServicoMultisoftware, Auditado);
                }
                else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Extratta)
                {
                    Servicos.Embarcador.CIOT.Extratta svcExtratta = new Servicos.Embarcador.CIOT.Extratta();
                    svcExtratta.EmitirPagamentoMotorista(pagamento.Codigo, unidadeDeTrabalho, TipoServicoMultisoftware, Auditado);
                }
                else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.RepomFrete)
                {
                    Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete svcRepomFrete = new Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete();
                    svcRepomFrete.EmitirPagamentoMotorista(pagamento.Codigo, unidadeDeTrabalho, TipoServicoMultisoftware, Auditado);
                }
                else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.KMM)
                {
                    Servicos.Embarcador.Integracao.KMM.IntegracaoKMM svcKMM = new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unidadeDeTrabalho);
                    svcKMM.IntegrarPagamentoMotorista(pagamento.Codigo, unidadeDeTrabalho, TipoServicoMultisoftware, Auditado);
                }

                #region Verificar integração pendente
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio> listaIntegracaoPendente = repPagamentoMotoristaIntegracaoEnvio.BuscarIntegracoesPendenteDeEnvio(pagamento.Codigo, null);

                if (listaIntegracaoPendente.Count == 0)
                {
                    pagamento = repPagamentoMotoristaTMS.BuscarPorCodigo(pagamento.Codigo);
                    pagamento.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;
                    repPagamentoMotoristaTMS.Atualizar(pagamento);

                    if (configuracaoTMS.ConfirmarPagamentoMotoristaAutomaticamente)
                    {
                        string msgRetorno = "";
                        if (!Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamento.Codigo, configuracaoTMS.TipoMovimentoPagamentoMotorista, Auditado, pagamento.Usuario, unidadeDeTrabalho, unidadeDeTrabalho.StringConexao, TipoServicoMultisoftware))
                            Servicos.Log.TratarErro($"Ocorreu uma falha ao confirmar o pagamento do motorista código {pagamento.Codigo}: {msgRetorno}");
                    }
                }
                #endregion

                unidadeDeTrabalho.CommitChanges();
                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ReenviarTodos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                SituacaoIntegracao? situacao = null;
                SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                TipoIntegracaoPagamentoMotorista? tipo = null;
                TipoIntegracaoPagamentoMotorista tipoAux;
                if (Enum.TryParse(Request.Params("Tipo"), out tipoAux))
                    tipo = tipoAux;

                int codigoPagamentoMotorista = Request.GetIntParam("PagamentoMotorista");

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unidadeDeTrabalho);
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio> integracoes = repPagamentoMotoristaIntegracaoEnvio.BuscarPorPagamento(codigoPagamentoMotorista, situacao, tipo);
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamento = repPagamentoMotoristaTMS.BuscarPorCodigo(codigoPagamentoMotorista);

                if (pagamento == null)
                    return new JsonpResult(false, true, "Nenhum pagamento localizado.");

                TipoIntegracaoPagamentoMotorista tipoIntegracao = pagamento.PagamentoMotoristaTipo.TipoIntegracaoPagamentoMotorista;
                if (tipoIntegracao == TipoIntegracaoPagamentoMotorista.SemIntegracao)
                    return new JsonpResult(false, true, "Tipo do pagamento não possui integração.");

                if (pagamento.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.AgAprovacao || pagamento.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.AutorizacaoPendente)
                    return new JsonpResult(false, true, "Favor realizar a aprovação do pagamento.");

                if (pagamento.SituacaoPagamentoMotorista != SituacaoPagamentoMotorista.FalhaIntegracao && pagamento.SituacaoPagamentoMotorista != SituacaoPagamentoMotorista.AgIntegracao && !(integracoes.TrueForAll(o => o.SituacaoIntegracao == SituacaoIntegracao.Integrado) && tipoIntegracao == TipoIntegracaoPagamentoMotorista.Email))
                    return new JsonpResult(false, true, "Situação do pagamento não permite realizar a integração.");

                if (integracoes.Exists(o => o.SituacaoIntegracao != SituacaoIntegracao.ProblemaIntegracao && o.TipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Extratta))
                    return new JsonpResult(false, true, "Situação de uma integreção não permite realizar a integração.");


                unidadeDeTrabalho.Start();

                foreach (Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio integracao in integracoes)
                {
                    integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    integracao.NumeroTentativas += 1;

                    repPagamentoMotoristaIntegracaoEnvio.Atualizar(integracao);

                    TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = integracao.TipoIntegracaoPagamentoMotorista;
                    if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PagBem)
                    {
                        Servicos.Embarcador.CIOT.Pagbem svcPagbem = new Servicos.Embarcador.CIOT.Pagbem();
                        svcPagbem.RealizarPagamentoMotorista(pagamento, Auditado, TipoServicoMultisoftware, unidadeDeTrabalho, out string mensagem);
                    }
                    else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCard)
                    {
                        Servicos.Embarcador.PagamentoMotorista.PamCard svcPamCard = new Servicos.Embarcador.PagamentoMotorista.PamCard(unidadeDeTrabalho);
                        svcPamCard.EmitirPagamentoMotorista(pagamento.Codigo, unidadeDeTrabalho, TipoServicoMultisoftware, Auditado);
                    }
                    else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.PamCardCorporativo)
                    {
                        Servicos.Embarcador.PagamentoMotorista.PamCard svcPamCard = new Servicos.Embarcador.PagamentoMotorista.PamCard(unidadeDeTrabalho);
                        svcPamCard.EmitirPagamentoMotoristaPamcardCorporativo(pagamento.Codigo, unidadeDeTrabalho, TipoServicoMultisoftware, Auditado);
                    }
                    else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Email)
                    {
                        Servicos.Embarcador.PagamentoMotorista.IntegracaoEmailPagamentoMotorista svcIntegracaoEmail = new Servicos.Embarcador.PagamentoMotorista.IntegracaoEmailPagamentoMotorista(unidadeDeTrabalho);
                        svcIntegracaoEmail.EnviarEmailPagamentoMotorista(pagamento.Codigo, unidadeDeTrabalho, TipoServicoMultisoftware, Auditado);
                    }
                    else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Target)
                    {
                        Servicos.Embarcador.PagamentoMotorista.Target svcTarget = new Servicos.Embarcador.PagamentoMotorista.Target(unidadeDeTrabalho);
                        svcTarget.EmitirPagamentoMotorista(pagamento.Codigo, TipoServicoMultisoftware, Auditado);
                    }
                    else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.Extratta)
                    {
                        Servicos.Embarcador.CIOT.Extratta svcExtratta = new Servicos.Embarcador.CIOT.Extratta();
                        svcExtratta.EmitirPagamentoMotorista(pagamento.Codigo, unidadeDeTrabalho, TipoServicoMultisoftware, Auditado);
                    }
                    else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.RepomFrete)
                    {
                        Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete svcRepomFrete = new Servicos.Embarcador.CIOT.RepomFrete.IntegracaoRepomFrete();
                        svcRepomFrete.EmitirPagamentoMotorista(pagamento.Codigo, unidadeDeTrabalho, TipoServicoMultisoftware, Auditado);
                    }
                    else if (tipoIntegracaoPagamentoMotorista == TipoIntegracaoPagamentoMotorista.KMM)
                    {
                        Servicos.Embarcador.Integracao.KMM.IntegracaoKMM svcKMM = new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unidadeDeTrabalho);
                        svcKMM.IntegrarPagamentoMotorista(pagamento.Codigo, unidadeDeTrabalho, TipoServicoMultisoftware, Auditado);
                    }
                }

                #region Verificar integração pendente
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio> listaIntegracaoPendente = repPagamentoMotoristaIntegracaoEnvio.BuscarIntegracoesPendenteDeEnvio(pagamento.Codigo, null);

                if (listaIntegracaoPendente.Count == 0)
                {
                    pagamento = repPagamentoMotoristaTMS.BuscarPorCodigo(pagamento.Codigo);
                    pagamento.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;
                    repPagamentoMotoristaTMS.Atualizar(pagamento);

                    if (configuracaoTMS.ConfirmarPagamentoMotoristaAutomaticamente)
                    {
                        string msgRetorno = "";
                        if (!Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamento.Codigo, configuracaoTMS.TipoMovimentoPagamentoMotorista, Auditado, pagamento.Usuario, unidadeDeTrabalho, unidadeDeTrabalho.StringConexao, TipoServicoMultisoftware))
                            Servicos.Log.TratarErro($"Ocorreu uma falha ao confirmar o pagamento do motorista código {pagamento.Codigo}: {msgRetorno}");
                    }
                }
                #endregion

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AvancarSemIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("PagamentosMotoristas/PagamentoMotoristaTMS");
                if (!Usuario.UsuarioAdministrador && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PagamentoMotorista_PermiteAvancarSemIntegracao))
                    return new JsonpResult(false, true, "Você não possui permissão para avançar sem integração.");

                int.TryParse(Request.Params("CodigoPagamentoMotorista"), out int codigo);

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista = repPagamentoMotoristaTMS.BuscarPorCodigo(codigo);

                if (pagamentoMotorista == null)
                    return new JsonpResult(false, true, "Nenhum pagamento localizado.");

                if (pagamentoMotorista.SituacaoPagamentoMotorista != SituacaoPagamentoMotorista.FalhaIntegracao)
                    return new JsonpResult(false, true, "Nessa situação do pagamento não permite avançar sem integração.");

                unidadeDeTrabalho.Start();

                pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;

                string msgRetorno = "";
                if (ConfiguracaoEmbarcador.ConfirmarPagamentoMotoristaAutomaticamente)
                {
                    if (!Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamentoMotorista.Codigo, ConfiguracaoEmbarcador.TipoMovimentoPagamentoMotorista, Auditado, pagamentoMotorista.Usuario, unidadeDeTrabalho, _conexao.StringConexao, TipoServicoMultisoftware))
                        return new JsonpResult(false, true, msgRetorno);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamentoMotorista, null, "Avançou etapa sem integração.", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(new { MensagemRetorno = msgRetorno });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao avançar sem integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidadeComRequest(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Financeiro.PlanoConta repPlanoConta = new Repositorio.Embarcador.Financeiro.PlanoConta(unitOfWork);
            Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            int.TryParse(Request.Params("Carga"), out int codigoCarga);
            int.TryParse(Request.Params("Chamado"), out int codigoChamado);
            int.TryParse(Request.Params("PlanoDeContaDebito"), out int codigoPlanoDeContaDebito);
            int.TryParse(Request.Params("PlanoDeContaCredito"), out int codigoPlanoDeContaCredito);
            int.TryParse(Request.Params("Motorista"), out int codigoMotorista);
            int.TryParse(Request.Params("TipoPagamentoMotorista"), out int codigoTipoPagamentoMotorista);
            int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);

            double pessoaTituloPagar = Request.GetDoubleParam("PessoaTituloPagar");
            double terceiro = Request.GetDoubleParam("Terceiro");

            DateTime.TryParseExact(Request.Params("DataPagamentoMotorista"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataPagamentoMotorista);
            DateTime.TryParseExact(Request.Params("DataVencimentoTituloPagar"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataVencimentoTituloPagarAux);

            DateTime? dataVencimentoTituloPagar = null;
            if (dataVencimentoTituloPagarAux > DateTime.MinValue)
                dataVencimentoTituloPagar = dataVencimentoTituloPagarAux;

            decimal.TryParse(Request.Params("Valor"), out decimal valor);
            decimal.TryParse(Request.Params("SaldoDescontado"), out decimal saldoDescontado);

            string observacao = Request.Params("Observacao") ?? string.Empty;

            if (codigoPlanoDeContaDebito > 0)
                pagamentoMotorista.PlanoDeContaDebito = repPlanoConta.BuscarPorCodigo(codigoPlanoDeContaDebito);
            else
                pagamentoMotorista.PlanoDeContaDebito = null;
            if (codigoPlanoDeContaCredito > 0)
                pagamentoMotorista.PlanoDeContaCredito = repPlanoConta.BuscarPorCodigo(codigoPlanoDeContaCredito);
            else
                pagamentoMotorista.PlanoDeContaCredito = null;
            if (codigoCarga > 0)
                pagamentoMotorista.Carga = repCarga.BuscarPorCodigo(codigoCarga);
            else
                pagamentoMotorista.Carga = null;
            if (codigoChamado > 0)
                pagamentoMotorista.Chamado = repChamado.BuscarPorCodigo(codigoChamado);
            else
                pagamentoMotorista.Chamado = null;
            if (codigoMotorista > 0)
                pagamentoMotorista.Motorista = repUsuario.BuscarPorCodigo(codigoMotorista);
            if (codigoTipoPagamentoMotorista > 0)
                pagamentoMotorista.PagamentoMotoristaTipo = repPagamentoMotoristaTipo.BuscarPorCodigo(codigoTipoPagamentoMotorista);
            pagamentoMotorista.DataPagamento = dataPagamentoMotorista;
            pagamentoMotorista.Valor = valor;
            pagamentoMotorista.Observacao = observacao;
            pagamentoMotorista.DataVencimentoTituloPagar = dataVencimentoTituloPagar != null && dataVencimentoTituloPagar.Value > DateTime.MinValue ? dataVencimentoTituloPagar : null;
            pagamentoMotorista.SaldoDescontado = saldoDescontado;
            pagamentoMotorista.SaldoDiariaMotorista = Request.GetDecimalParam("SaldoDiariaMotorista");
            pagamentoMotorista.PessoaTituloPagar = pessoaTituloPagar > 0 ? repCliente.BuscarPorCPFCNPJ(pessoaTituloPagar) : null;
            pagamentoMotorista.Terceiro = terceiro > 0 ? repCliente.BuscarPorCPFCNPJ(terceiro) : null;

            if (pagamentoMotorista.Codigo == 0)
            {
                pagamentoMotorista.Usuario = this.Usuario;
                pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AgInformacoes;
                pagamentoMotorista.Data = DateTime.Now.Date;
                pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Iniciada;
                pagamentoMotorista.Numero = repPagamentoMotorista.BuscarProximoNumero();
                pagamentoMotorista.PagamentoLiberado = true;
            }

            pagamentoMotorista.MoedaCotacaoBancoCentral = Request.GetNullableEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
            pagamentoMotorista.DataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");
            pagamentoMotorista.ValorMoedaCotacao = Request.GetDecimalParam("ValorMoedaCotacao");
            pagamentoMotorista.ValorOriginalMoedaEstrangeira = Request.GetDecimalParam("ValorOriginalMoedaEstrangeira");
            pagamentoMotorista.DataEfetivacao = Request.GetNullableDateTimeParam("DataEfetivacao");

            if (!pagamentoMotorista.DataEfetivacao.HasValue)
                pagamentoMotorista.DataEfetivacao = pagamentoMotorista.DataPagamento;

            if (codigoEmpresa > 0)
                pagamentoMotorista.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

        }

        private void PropOrdena(ref string propOrdena)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */
            if (propOrdena == "CodigoCargaEmbarcador")
                propOrdena = "Carga." + propOrdena;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotoristaTMS = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
            Repositorio.Embarcador.Acerto.AcertoViagem repAcertoViagem = new Repositorio.Embarcador.Acerto.AcertoViagem(unitOfWork);
            Dominio.Entidades.Embarcador.Acerto.AcertoViagem acertoViagem = null;


            // Dados do filtro
            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicio);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFim);

            Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista situacao);
            Enum.TryParse(Request.Params("Etapa"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPagamentoMotorista etapa);
            Enum.TryParse(Request.Params("TipoPagamentoMotorista"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMotorista tipoPagamentoMotorista);

            bool pagamentosParaAcertoViagem = Request.GetBoolParam("PagamentosParaAcertoViagem");

            int.TryParse(Request.Params("Codigo"), out int codigo);
            int numero = Request.GetIntParam("Numero");
            int.TryParse(Request.Params("Operador"), out int codigoOperador);
            List<int> codigoTipoPagamento = Request.GetListParam<int>("TipoPagamento");
            int.TryParse(Request.Params("Motorista"), out int codigoMotorista);
            bool.TryParse(Request.Params("Pendente"), out bool pendente);
            int.TryParse(Request.Params("AcertoViagem"), out int codigoAcertoViagem);
            List<int> empresa = Request.GetListParam<int>("Empresa");

            if (codigoAcertoViagem > 0)
            {
                acertoViagem = repAcertoViagem.BuscarPorCodigo(codigoAcertoViagem);
                codigoMotorista = acertoViagem.Motorista.Codigo;
                situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.FinalizadoPagamento;
            }

            string numeroCarga = Request.Params("NumeroCarga") ?? string.Empty;

            // Consulta
            List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> listaGrid = repPagamentoMotoristaTMS.Consultar(tipoPagamentoMotorista, ConfiguracaoEmbarcador.BuscarAdiantamentosSemDataInicialAcertoViagem, pagamentosParaAcertoViagem, codigo, numeroCarga, codigoTipoPagamento, codigoMotorista, numero, dataInicio, dataFim, codigoOperador, situacao, etapa, codigoAcertoViagem, propOrdenar, dirOrdena, inicio, limite, pendente, empresa);
            totalRegistros = repPagamentoMotoristaTMS.ContarConsulta(tipoPagamentoMotorista, ConfiguracaoEmbarcador.BuscarAdiantamentosSemDataInicialAcertoViagem, pagamentosParaAcertoViagem, codigo, numeroCarga, codigoTipoPagamento, codigoMotorista, numero, dataInicio, dataFim, codigoOperador, situacao, etapa, codigoAcertoViagem, pendente, empresa);

            var lista = (from p in listaGrid
                         select new
                         {
                             p.Codigo,
                             p.Numero,
                             DataPagamento = p.DataPagamento.ToString("dd/MM/yyyy HH:mm"),
                             CodigoCargaEmbarcador = p.Carga?.CodigoCargaEmbarcador,
                             Motorista = p.Motorista?.Nome + " (" + p.Motorista?.CPF_Formatado + ")",
                             PagamentoMotoristaTipo = p.PagamentoMotoristaTipo?.Descricao,
                             Valor = p.Valor.ToString("n2"),
                             Saldo = p.TotalPagamento(ConfiguracaoEmbarcador.NaoDescontarValorSaldoMotorista).ToString("n2"),
                             p.DescricaoSituacao,
                             p.StatusFinanceiro,
                             p.DescricaoEtapa,
                             GerarTituloPagar = p.PagamentoMotoristaTipo?.GerarTituloPagar ?? false,
                             p.MoedaCotacaoBancoCentral,
                             DataBaseCRT = p.DataBaseCRT.HasValue ? p.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                             ValorMoedaCotacao = p.ValorMoedaCotacao.ToString("n10"),
                             ValorOriginalMoedaEstrangeira = p.ValorOriginalMoedaEstrangeira.ToString("n2"),
                             DesabilitarAlteracaoDosPlanosDeContas = p.PagamentoMotoristaTipo?.DesabilitarAlteracaoDosPlanosDeContas ?? false,
                             Chamado = p.Chamado?.Numero
                         }).ToList();

            return lista.ToList();
        }

        private dynamic retornarPagamentoMotorista(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Repositorio.UnitOfWork unitOfWork, string mensagemRetorno = "")
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMSAnexo repPagamentoMotoristaTMSAnexo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMSAnexo(unitOfWork);

            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao autorizacao = repPagamentoMotoristaAutorizacao.BuscarAutorizacaoPagamento(pagamentoMotorista.Codigo);
            List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMSAnexo> anexos = repPagamentoMotoristaTMSAnexo.BuscarPorPagamentoMotorista(pagamentoMotorista.Codigo);

            var dynOcorrencia = new
            {
                pagamentoMotorista.Codigo,
                SaldoDiariaMotorista = pagamentoMotorista.SaldoDiariaMotorista.ToString("n2"),
                SaldoDescontado = pagamentoMotorista.SaldoDescontado.ToString("n2"),
                TotalPagamento = pagamentoMotorista.TotalPagamento(ConfiguracaoEmbarcador.NaoDescontarValorSaldoMotorista).ToString("n2"),
                DescricaoSituacao = pagamentoMotorista.DescricaoSituacao,
                Situacao = pagamentoMotorista.SituacaoPagamentoMotorista,
                NumeroPagamentoMotorista = pagamentoMotorista.Numero.ToString("n0"),
                PlanoDeContaCredito = pagamentoMotorista.PlanoDeContaCredito != null ? new { pagamentoMotorista.PlanoDeContaCredito.Codigo, Descricao = pagamentoMotorista.PlanoDeContaCredito.BuscarDescricao } : new { Codigo = 0, Descricao = "" },
                PlanoDeContaDebito = pagamentoMotorista.PlanoDeContaDebito != null ? new { pagamentoMotorista.PlanoDeContaDebito.Codigo, Descricao = pagamentoMotorista.PlanoDeContaDebito.BuscarDescricao } : new { Codigo = 0, Descricao = "" },
                Carga = pagamentoMotorista.Carga != null ? new { pagamentoMotorista.Carga.Codigo, Descricao = pagamentoMotorista.Carga.CodigoCargaEmbarcador } : new { Codigo = 0, Descricao = "" },
                Chamado = pagamentoMotorista.Chamado != null ? new { pagamentoMotorista.Chamado.Codigo, Descricao = pagamentoMotorista.Chamado.Descricao } : new { Codigo = 0, Descricao = "" },
                Motorista = pagamentoMotorista.Motorista != null ? new { pagamentoMotorista.Motorista.Codigo, Descricao = pagamentoMotorista.Motorista.Nome } : new { Codigo = 0, Descricao = "" },
                DataPagamentoMotorista = pagamentoMotorista.DataPagamento.ToString("dd/MM/yyyy HH:mm"),
                TipoPagamentoMotorista = pagamentoMotorista.PagamentoMotoristaTipo != null ? new { pagamentoMotorista.PagamentoMotoristaTipo.Codigo, Descricao = pagamentoMotorista.PagamentoMotoristaTipo.Descricao } : new { Codigo = 0, Descricao = "" },
                Valor = pagamentoMotorista.Valor.ToString("n2"),
                ValorPagamentoMotorista = pagamentoMotorista.Valor.ToString("n2"),
                pagamentoMotorista.Observacao,
                SolicitacaoCredito = ObterSolicitacaoCredito(pagamentoMotorista, autorizacao),
                NumeroCarga = pagamentoMotorista.Carga != null ? pagamentoMotorista.Carga.CodigoCargaEmbarcador : string.Empty,
                NomeMotorista = pagamentoMotorista.Motorista != null ? pagamentoMotorista.Motorista.Nome : string.Empty,
                DescricaoTipoPagamentoMotorista = pagamentoMotorista.PagamentoMotoristaTipo != null ? pagamentoMotorista.PagamentoMotoristaTipo.Descricao : string.Empty,
                SituacaoPagamentoMotorista = pagamentoMotorista.SituacaoPagamentoMotorista,
                DataVencimentoTituloPagar = pagamentoMotorista.DataVencimentoTituloPagar != null && pagamentoMotorista.DataVencimentoTituloPagar.Value > DateTime.MinValue ? pagamentoMotorista.DataVencimentoTituloPagar.Value.ToString("dd/MM/yyyy") : string.Empty,
                GerarTituloPagar = pagamentoMotorista.PagamentoMotoristaTipo?.GerarTituloPagar ?? false,
                pagamentoMotorista.MoedaCotacaoBancoCentral,
                DataBaseCRT = pagamentoMotorista.DataBaseCRT.HasValue ? pagamentoMotorista.DataBaseCRT.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                ValorMoedaCotacao = pagamentoMotorista.ValorMoedaCotacao.ToString("n10"),
                ValorOriginalMoedaEstrangeira = pagamentoMotorista.ValorOriginalMoedaEstrangeira.ToString("n2"),
                GerarTituloAPagarAoMotorista = pagamentoMotorista.PagamentoMotoristaTipo?.GerarTituloAPagarAoMotorista ?? false,
                DesabilitarAlteracaoDosPlanosDeContas = pagamentoMotorista.PagamentoMotoristaTipo?.DesabilitarAlteracaoDosPlanosDeContas ?? false,
                MensagemRetorno = mensagemRetorno,
                PessoaSeraInformadaGeracaoPagamento = pagamentoMotorista.PagamentoMotoristaTipo?.PessoaSeraInformadaGeracaoPagamento ?? false,
                PessoaTituloPagar = new { Codigo = pagamentoMotorista.PessoaTituloPagar?.CPF_CNPJ ?? 0d, Descricao = pagamentoMotorista.PessoaTituloPagar?.Descricao ?? string.Empty },
                Terceiro = new { Codigo = pagamentoMotorista.Terceiro?.CPF_CNPJ ?? 0d, Descricao = pagamentoMotorista.Terceiro?.Descricao ?? string.Empty },
                Anexos = (
                        from anexo in anexos
                        select new
                        {
                            anexo.Codigo,
                            anexo.Descricao,
                            anexo.NomeArquivo,
                        }
                    ).ToList(),
                TipoPagamentoMotoristaEnum = pagamentoMotorista.PagamentoMotoristaTipo?.TipoPagamentoMotorista ?? TipoPagamentoMotorista.Nenhum,
                DataEfetivacao = pagamentoMotorista.DataEfetivacao.HasValue ? pagamentoMotorista.DataEfetivacao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                ValorINSS = pagamentoMotorista.ValorINSS,
                ValorSEST = pagamentoMotorista.ValorSEST,
                ValorSENAT = pagamentoMotorista.ValorSENAT,
                ValorIRRF = pagamentoMotorista.ValorIRRF,
                ValorLiquido = pagamentoMotorista.Valor - pagamentoMotorista.ValorINSS - pagamentoMotorista.ValorSEST - pagamentoMotorista.ValorSENAT - pagamentoMotorista.ValorIRRF,
                ReterImpostoPagamentoMotorista = pagamentoMotorista?.PagamentoMotoristaTipo?.ReterImpostoPagamentoMotorista ?? false,
                Empresa = pagamentoMotorista.Empresa != null ? new { pagamentoMotorista.Empresa.Codigo, Descricao = pagamentoMotorista.Empresa.RazaoSocial } : pagamentoMotorista.Carga != null && pagamentoMotorista.Carga.Empresa != null ? new { pagamentoMotorista.Carga.Empresa.Codigo, Descricao = this.empresaDescricao(unitOfWork, pagamentoMotorista.Carga.Empresa, pagamentoMotorista.Carga) } : new { Codigo = 0, Descricao = "" },
            };

            return dynOcorrencia;
        }

        private dynamic ObterSolicitacaoCredito(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista, Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao autorizacao)
        {
            var retorno = new
            {
                SituacaoSolicitacao = SituacaoSolicitacao(pagamentoMotorista),
                DescricaoSituacao = pagamentoMotorista.DescricaoSituacao,
                DataSolicitacao = pagamentoMotorista.DataPagamento.ToString("dd/MM/yyyy"),
                Solicitado = pagamentoMotorista.Usuario.Nome,
                Motorista = pagamentoMotorista.Motorista.Nome,
                TipoPagamentoMotorista = pagamentoMotorista.PagamentoMotoristaTipo.Descricao,
                ValorSolicitado = pagamentoMotorista.TotalPagamento(ConfiguracaoEmbarcador.NaoDescontarValorSaldoMotorista).ToString("n2"),
                Observacao = pagamentoMotorista.Observacao,
                DataRetorno = autorizacao?.Data.ToString("dd/MM/yyyy") ?? string.Empty,
                Creditor = autorizacao?.Usuario.Nome,
                ValorLiberado = autorizacao != null && autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada ? pagamentoMotorista.TotalPagamento(ConfiguracaoEmbarcador.NaoDescontarValorSaldoMotorista).ToString("n2") : string.Empty,
                RetornoSolicitacao = autorizacao?.Motivo ?? string.Empty,
                ComRegraAutorizacao = autorizacao != null
            };

            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito SituacaoSolicitacao(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista)
        {
            if (pagamentoMotorista.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgAprovacao || pagamentoMotorista.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AutorizacaoPendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.AgLiberacao;
            else if (pagamentoMotorista.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Rejeitado;
            else if (pagamentoMotorista.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.AgIntegracao)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Liberado;
            else if (pagamentoMotorista.SituacaoPagamentoMotorista == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamentoMotorista.Finalizada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Utilizado;
            else
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSolicitacaoCredito.Todos;
        }

        private Models.Grid.Grid GridConsultarAutorizacoes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);

            int codOcorrencia = int.Parse(Request.Params("Codigo"));

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Regra", false);
            grid.AdicionarCabecalho("Data", false);
            grid.AdicionarCabecalho("Motivo", false);
            grid.AdicionarCabecalho("Justificativa", false);
            grid.AdicionarCabecalho("DT_RowColor", false);
            grid.AdicionarCabecalho("DT_FontColor", false);

            string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

            List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao> listaPagamentoMotoristaAutorizacao = repCargaOcorrenciaAutorizacao.ConsultarAutorizacoesPorPagamento(codOcorrencia, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
            grid.setarQuantidadeTotal(repCargaOcorrenciaAutorizacao.ContarConsultarAutorizacoesPorPagamento(codOcorrencia));

            var lista = (from obj in listaPagamentoMotoristaAutorizacao
                         select new
                         {
                             obj.Codigo,
                             Situacao = obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente ? "Pendente" : obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada ? "Aprovada" : obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada ? "Rejeitada" : string.Empty,
                             Usuario = obj.Usuario?.Nome,
                             Regra = TituloRegra(obj),
                             Data = obj.Data != null ? obj.Data.ToString() : string.Empty,
                             Motivo = !string.IsNullOrWhiteSpace(obj.Motivo) ? obj.Motivo : string.Empty,
                             Justificativa = obj.Motivo,
                             DT_RowColor = obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde : obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho : obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo : "",
                             DT_FontColor = obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Branco : ""
                         }).ToList();
            grid.AdicionaRows(lista);

            return grid;
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao regra)
        {
            return regra.RegrasPagamentoMotorista?.Descricao;
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Nº Pagamento", "Numero", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Data do Pagamento", "DataPagamento", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Motorista", "Motorista", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo do Pagamento", "PagamentoMotoristaTipo", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Valor", "Valor", 9, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Saldo", "Saldo", 9, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Chamado", "Chamado", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Situação Financeira", "StatusFinanceiro", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Etapa", "DescricaoEtapa", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("GerarTituloPagar", false);
            grid.AdicionarCabecalho("MoedaCotacaoBancoCentral", false);
            grid.AdicionarCabecalho("DataBaseCRT", false);
            grid.AdicionarCabecalho("ValorMoedaCotacao", false);
            grid.AdicionarCabecalho("ValorOriginalMoedaEstrangeira", false);
            grid.AdicionarCabecalho("DesabilitarAlteracaoDosPlanosDeContas", false);

            return grid;
        }

        #endregion

        #region Importação

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                unitOfWork.Start();

                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPagamentoMotorista();
                List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS> pagamentosMotoristas = new List<Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS>();

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request, pagamentosMotoristas, ((dados) =>
               {
                   Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaImportacao servicoPagamentoMotoristaImportacao = new Servicos.Embarcador.PagamentoMotorista.PagamentoMotoristaImportacao(unitOfWork, TipoServicoMultisoftware, Empresa, dados, configuracao);

                   return servicoPagamentoMotoristaImportacao.ObterPagamentoMotoristaImportar(this.Usuario);
               }));

                if (retorno == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);

                int totalRegistrosImportados = 0;
                dynamic parametro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametro"));
                bool permiteInserir = (bool)parametro.Inserir;
                bool permiteAtualizar = (bool)parametro.Atualizar;

                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS repositorioPagamentoMotorista = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS(unitOfWork);
                Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio repPagamentoMotoristaIntegracaoEnvio = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio(unitOfWork);

                foreach (Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTMS pagamentoMotorista in pagamentosMotoristas)
                {
                    if ((pagamentoMotorista.Codigo > 0) && permiteAtualizar)
                    {
                        repositorioPagamentoMotorista.Atualizar(pagamentoMotorista, Auditado);
                        totalRegistrosImportados++;
                    }
                    else if ((pagamentoMotorista.Codigo == 0) && permiteInserir)
                    {
                        pagamentoMotorista.Numero = repositorioPagamentoMotorista.BuscarProximoNumero();
                        repositorioPagamentoMotorista.Inserir(pagamentoMotorista, Auditado);

                        TipoIntegracaoPagamentoMotorista tipoIntegracaoPagamentoMotorista = pagamentoMotorista.PagamentoMotoristaTipo?.TipoIntegracaoPagamentoMotorista ?? TipoIntegracaoPagamentoMotorista.SemIntegracao;

                        if (Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarRegrasAutorizacaoPagamentoMotorista(pagamentoMotorista, TipoServicoMultisoftware, unitOfWork, this.Usuario, _conexao.StringConexao, Auditado, out bool contemAprovadorIgualAoOperador))
                        {
                            pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AutorizacaoPendente;
                            pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.AgAutorizacao;
                        }
                        else
                        {
                            pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.AgIntegracao;
                            pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Integracao;
                        }

                        if (contemAprovadorIgualAoOperador)
                        {
                            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao repPagamentoMotoristaAutorizacao = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao(unitOfWork);
                            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaAutorizacao pagamentoMotoristaAutorizacao = repPagamentoMotoristaAutorizacao.BuscarPrimeiroPorPagamentoUsuario(pagamentoMotorista.Codigo, pagamentoMotorista.Usuario.Codigo);

                            Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.EfetuarAprovacao(pagamentoMotoristaAutorizacao, pagamentoMotorista.Usuario, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                            string msgRetornoSit = "";
                            Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.VerificarSituacaoPagamento(pagamentoMotoristaAutorizacao.PagamentoMotoristaTMS, unitOfWork, ref msgRetornoSit, TipoServicoMultisoftware, Auditado, _conexao.StringConexao, ConfiguracaoEmbarcador, pagamentoMotorista.Usuario);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, pagamentoMotorista, null, "Aprovou o pagamento pelo mesmo operadora da alçada.", unitOfWork);
                        }

                        string msgRetorno = "";
                        if (tipoIntegracaoPagamentoMotorista.PossuiIntegracao())
                        {
                            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio pagamentoMotoristaIntegracaoEnvio = new Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaIntegracaoEnvio();
                            pagamentoMotoristaIntegracaoEnvio.Data = DateTime.Now.Date;
                            pagamentoMotoristaIntegracaoEnvio.NumeroTentativas = 0;
                            pagamentoMotoristaIntegracaoEnvio.PagamentoMotoristaTMS = pagamentoMotorista;
                            pagamentoMotoristaIntegracaoEnvio.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                            pagamentoMotoristaIntegracaoEnvio.TipoIntegracaoPagamentoMotorista = tipoIntegracaoPagamentoMotorista;

                            repPagamentoMotoristaIntegracaoEnvio.Inserir(pagamentoMotoristaIntegracaoEnvio);
                        }
                        else if (pagamentoMotorista.SituacaoPagamentoMotorista == SituacaoPagamentoMotorista.AgIntegracao)
                        {
                            pagamentoMotorista.SituacaoPagamentoMotorista = SituacaoPagamentoMotorista.Finalizada;
                            pagamentoMotorista.EtapaPagamentoMotorista = EtapaPagamentoMotorista.Integracao;

                            if (ConfiguracaoEmbarcador.ConfirmarPagamentoMotoristaAutomaticamente)
                                Servicos.Embarcador.PagamentoMotorista.AutorizacaoPagamentoMotorista.ConfirmarPagamentoMotorista(ref msgRetorno, pagamentoMotorista.Codigo, ConfiguracaoEmbarcador.TipoMovimentoPagamentoMotorista, Auditado, pagamentoMotorista.Usuario, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware);
                        }

                        repositorioPagamentoMotorista.Atualizar(pagamentoMotorista, Auditado);

                        totalRegistrosImportados++;
                    }
                }

                unitOfWork.CommitChanges();

                retorno.Importados = totalRegistrosImportados;

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPagamentoMotorista();

            return new JsonpResult(configuracoes.ToList());
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoPagamentoMotorista()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Data do pagamento", Propriedade = "DataPagamento", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Motorista (CPF)", Propriedade = "CpfMotorista", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Valor", Propriedade = "Valor", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Tipo do pagamento", Propriedade = "TipoPagamento", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Observação", Propriedade = "Observacao", Tamanho = 500, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Chamado", Propriedade = "Chamado", Tamanho = 150, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Carga", Propriedade = "Carga", Tamanho = 150, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Plano de Saída", Propriedade = "PlanoSaida", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Plano de Entrada", Propriedade = "PlanoEntrada", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "Vencimento Tit. Pagar", Propriedade = "DataVencimentoTituloPagar", Tamanho = 150, Obrigatorio = true, Regras = new List<string> { } });

            return configuracoes;
        }

        private Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha RetornarFalhaLinha(string mensagem, int indice)
        {
            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retorno = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha() { indice = indice, mensagemFalha = mensagem, processou = false };
            return retorno;
        }

        #endregion
    }
}
