using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/SolicitacaoVeiculo", "GestaoPatio/FluxoPatio")]
    public class SolicitacaoVeiculoController : BaseController
    {
        #region Construtores

        public SolicitacaoVeiculoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> AvancarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.GestaoPatio.SolicitacaoVeiculo servicoSolicitacaoVeiculo = new Servicos.Embarcador.GestaoPatio.SolicitacaoVeiculo(unitOfWork, Auditado, Cliente);

                servicoSolicitacaoVeiculo.Avancar(codigo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao solicitar o veículo.");
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
                int codigoSolicitacaoVeiculo = Request.GetIntParam("Codigo");
                int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");
                Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo repositorioSolicitacaoVeiculo = new Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo = null;

                if (codigoSolicitacaoVeiculo > 0)
                    solicitacaoVeiculo = repositorioSolicitacaoVeiculo.BuscarPorCodigo(codigoSolicitacaoVeiculo);
                else if (codigoFluxoGestaoPatio > 0)
                    solicitacaoVeiculo = repositorioSolicitacaoVeiculo.BuscarPorFluxoGestaoPatio(codigoFluxoGestaoPatio);

                if (solicitacaoVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (solicitacaoVeiculo.Carga != null)
                    return new JsonpResult(ObterSolicitacaoVeiculoPorCarga(unitOfWork, solicitacaoVeiculo));

                return new JsonpResult(ObterSolicitacaoVeiculoPorPreCarga(unitOfWork, solicitacaoVeiculo));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
                string descricaoEtapaGuarita = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(EtapaFluxoGestaoPatio.Guarita)?.Descricao ?? string.Empty;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Carga", "Carga", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Cargas Agrupadas", "CodigosAgrupadosCarga", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data da Solicitação", "DataSolicitacaoVeiculoIniciada", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(descricaoEtapaGuarita, "DataEntregaGuarita", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Destino", "Destino", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Doca", "Doca", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tempo Janela", "TempoJanela", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modelo", "ModeloVeiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Observação Janela", "ObservacaoFluxoPatio", 10, Models.Grid.Align.left, false);

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta servicoCargaJanelaCarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoConsulta(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaSolicitacaoVeiculo filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo repositorioSolicitacaoVeiculo = new Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo(unitOfWork);
                int totalRegistros = repositorioSolicitacaoVeiculo.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo> listaSolicitacaoVeiculo = null;
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento = null;

                if (totalRegistros > 0)
                {
                    listaSolicitacaoVeiculo = repositorioSolicitacaoVeiculo.Consultar(filtrosPesquisa, parametrosConsulta);
                    List<int> codigosCargas = (from o in listaSolicitacaoVeiculo where o.Carga != null select o.Carga.Codigo).Distinct().ToList();
                    listaCargaJanelaCarregamento = servicoCargaJanelaCarregamentoConsulta.ObterCargasJanelaCarregamentoPorCargas(codigosCargas);
                }
                else
                {
                    listaSolicitacaoVeiculo = new List<Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo>();
                    listaCargaJanelaCarregamento = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento>();
                }

                var listaSolicitacaoVeiculoRetornar = (
                    from solicitacaoVeiculo in listaSolicitacaoVeiculo
                    select ObterSolicitacaoVeiculo(solicitacaoVeiculo, listaCargaJanelaCarregamento, configuracaoEmbarcador, unitOfWork)
                ).ToList();

                grid.AdicionaRows(listaSolicitacaoVeiculoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReabrirFluxo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo repositorioSolicitacaoVeiculo = new Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo = repositorioSolicitacaoVeiculo.BuscarPorCodigo(codigo);

                if (solicitacaoVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                if (solicitacaoVeiculo.FluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio == SituacaoEtapaFluxoGestaoPatio.Aguardando)
                    return new JsonpResult(false, true, "Não foi possível reabrir o fluxo nessa situação.");

                unitOfWork.Start();

                servicoFluxoGestaoPatio.ReabrirFluxo(solicitacaoVeiculo.FluxoGestaoPatio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reabrir o fluxo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo repositorioSolicitacaoVeiculo = new Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo = repositorioSolicitacaoVeiculo.BuscarPorCodigo(codigo);

                if (solicitacaoVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                servicoFluxoGestaoPatio.RejeitarEtapa(solicitacaoVeiculo.FluxoGestaoPatio, EtapaFluxoGestaoPatio.SolicitacaoVeiculo);
                servicoFluxoGestaoPatio.Auditar(solicitacaoVeiculo.FluxoGestaoPatio, "Rejeitou o fluxo.");

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar a etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VoltarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo repositorioSolicitacaoVeiculo = new Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo = repositorioSolicitacaoVeiculo.BuscarPorCodigo(codigo);

                if (solicitacaoVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, Auditado, Cliente);

                unitOfWork.Start();

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasFluxoPatio = ObterPermissoesPersonalizadas("GestaoPatio/FluxoPatio");
                servicoFluxoGestaoPatio.VoltarEtapa(solicitacaoVeiculo.FluxoGestaoPatio, EtapaFluxoGestaoPatio.SolicitacaoVeiculo, this.Usuario, permissoesPersonalizadasFluxoPatio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao voltar a etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarSMSMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);


            try
            {
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo repositorioSolicitacaoVeiculo = new Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo = repositorioSolicitacaoVeiculo.BuscarPorCodigo(codigo);
                string veiculos = solicitacaoVeiculo.Carga?.RetornarPlacas;
                string placa = "";

                foreach (char c in veiculos)
                {
                    if (c == ',')
                        break;

                    placa += c;
                }

                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorPlaca(placa);
                if (veiculo != null)
                    return new JsonpResult(false, "Veículo não encontrado.");

                Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);

                if (veiculoMotorista == null)
                    return new JsonpResult(false, "O veículo não possui motorista.");

                Servicos.SMS srvSMS = new Servicos.SMS(unitOfWork);
                string msgErro;
                string texto = "Entrada liberada - Favor se apresentar novamente na Portaria, com sua Nota Fiscal e senha em mãos!";
                string numeroCliente = veiculoMotorista.Celular;
                string tokenSMS = ConfiguracaoEmbarcador.TokenSMS;
                string cnpjRemetente = ConfiguracaoEmbarcador.SenderSMS;

                if (!srvSMS.EnviarSMS(tokenSMS, cnpjRemetente, numeroCliente, texto, unitOfWork, out msgErro))
                {
                    return new JsonpResult(false, "Falha ao enviar SMS");
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar SMS");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDadosTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoSolicitacaoVeiculo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.DadosTransporteFluxoPatio repDadosTransporteFluxoPatio = new Repositorio.Embarcador.GestaoPatio.DadosTransporteFluxoPatio(unitOfWork);

                Dominio.Entidades.Embarcador.GestaoPatio.DadosTransporteFluxoPatio dadosTransporteFluxoPatio = repDadosTransporteFluxoPatio.BuscarPorSolicitacaoVeiculo(codigoSolicitacaoVeiculo);

                if (dadosTransporteFluxoPatio == null)
                    return new JsonpResult(new
                    {
                        SolicitacaoVeiculo = codigoSolicitacaoVeiculo,
                    });

                var dadosTransporteFluxoPatioRetornar = new
                {
                    dadosTransporteFluxoPatio.Codigo,
                    SolicitacaoVeiculo = dadosTransporteFluxoPatio.SolicitacaoVeiculo.Codigo,
                    Motorista = new { dadosTransporteFluxoPatio.Motorista.Codigo, dadosTransporteFluxoPatio.Motorista.Descricao },
                    Veiculo = new { dadosTransporteFluxoPatio.Veiculo.Codigo, dadosTransporteFluxoPatio.Veiculo.Descricao },
                    FotoMotorista = ObterFotoBase64(dadosTransporteFluxoPatio.FotoMotorista, unitOfWork),
                    FotoMotoristaArquivo = dadosTransporteFluxoPatio.FotoMotorista,
                    Ajudantes = (
                            from ajudante in dadosTransporteFluxoPatio.Ajudantes
                            select new
                            {
                                ajudante.Codigo,
                                CPF = ajudante.CPF_Formatado,
                                Descricao = ajudante.DescricaoTelefone
                            }
                        ).ToList(),
                };

                return new JsonpResult(dadosTransporteFluxoPatioRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarDadosTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo repSolicitacaoVeiculo = new Repositorio.Embarcador.GestaoPatio.SolicitacaoVeiculo(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.DadosTransporteFluxoPatio repDadosTransporteFluxoPatio = new Repositorio.Embarcador.GestaoPatio.DadosTransporteFluxoPatio(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                int codigoSolicitacaoVeiculo = Request.GetIntParam("SolicitacaoVeiculo");
                int codigoMotorista = Request.GetIntParam("Motorista");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                string fotoMotoristaArquivo = Request.GetStringParam("FotoMotoristaArquivo");

                dynamic ajudantes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Ajudantes"));
                List<int> codigosAjudantes = new List<int>();

                foreach (var ajudante in ajudantes)
                    codigosAjudantes.Add((int)ajudante.Codigo);

                Dominio.Entidades.Embarcador.GestaoPatio.DadosTransporteFluxoPatio dadosTransporteFluxoPatio = repDadosTransporteFluxoPatio.BuscarPorCodigo(codigo) ?? new Dominio.Entidades.Embarcador.GestaoPatio.DadosTransporteFluxoPatio();
                Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo = repSolicitacaoVeiculo.BuscarPorCodigo(codigoSolicitacaoVeiculo);
                Dominio.Entidades.Usuario motorista = repMotorista.BuscarPorCodigo(codigoMotorista);
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                unitOfWork.Start();

                dadosTransporteFluxoPatio.SolicitacaoVeiculo = solicitacaoVeiculo;
                dadosTransporteFluxoPatio.Motorista = motorista;
                dadosTransporteFluxoPatio.Veiculo = veiculo;
                dadosTransporteFluxoPatio.FotoMotorista = fotoMotoristaArquivo;

                if (dadosTransporteFluxoPatio.Ajudantes?.Count > 0)
                    dadosTransporteFluxoPatio.Ajudantes.Clear();
                else
                    dadosTransporteFluxoPatio.Ajudantes = new List<Dominio.Entidades.Usuario>();

                if (codigosAjudantes.Count > 0)
                {
                    foreach (int codigoAjudante in codigosAjudantes)
                    {
                        Dominio.Entidades.Usuario ajudante = repMotorista.BuscarPorCodigo(codigoAjudante);

                        dadosTransporteFluxoPatio.Ajudantes.Add(ajudante);
                    }
                }

                if (dadosTransporteFluxoPatio.Codigo > 0)
                    repDadosTransporteFluxoPatio.Atualizar(dadosTransporteFluxoPatio);
                else
                    repDadosTransporteFluxoPatio.Inserir(dadosTransporteFluxoPatio);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar os dados de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarFoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.GestaoPatio.DadosTransporteFluxoPatio repDadosTransporteFluxoPatio = new Repositorio.Embarcador.GestaoPatio.DadosTransporteFluxoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoPatio.DadosTransporteFluxoPatio dadosTransporteFluxoPatio = repDadosTransporteFluxoPatio.BuscarPorSolicitacaoVeiculo(codigo);

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("ArquivoFoto");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Motorista.NenhumaFotoSelecionadaParaAdicionar);

                Servicos.DTO.CustomFile arquivoFoto = arquivos[0];
                string extensaoArquivo = System.IO.Path.GetExtension(arquivoFoto.FileName).ToLower();
                string caminho = ObterCaminhoArquivoFoto(unitOfWork);
                string guidArquivo = Guid.NewGuid().ToString().Replace("-", "");

                string nomeArquivoFotoExistente = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{dadosTransporteFluxoPatio?.FotoMotorista ?? ""}.*").FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(nomeArquivoFotoExistente))
                    Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivoFotoExistente);

                arquivoFoto.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guidArquivo}{extensaoArquivo}"));

                return new JsonpResult(new
                {
                    FotoMotorista = ObterFotoBase64(guidArquivo, unitOfWork),
                    FotoMotoristaArquivo = guidArquivo
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.OcorreuUmaFalhaAoAdicionarFotoDoMotorista);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirFoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string guidArquivo = Request.GetStringParam("FotoMotoristaArquivo");

                string caminho = ObterCaminhoArquivoFoto(unitOfWork);
                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{guidArquivo}.*").FirstOrDefault();

                if (string.IsNullOrWhiteSpace(nomeArquivo))
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Motorista.FotoNaoEncontrada);
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivo);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.OcorreuUmaFalhaAoRemoverFotoDoMotorista);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaSolicitacaoVeiculo ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaSolicitacaoVeiculo()
            {
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataLimite = Request.GetNullableDateTimeParam("DataFinal"),
                NumeroCarga = Request.GetStringParam("Carga"),
                Situacao = Request.GetNullableEnumParam<SituacaoSolicitacaoVeiculo>("Situacao")
            };
        }

        private dynamic ObterSolicitacaoVeiculo(Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento> listaCargaJanelaCarregamento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            int codigoCargaFiltrarJanelaCarregamento = solicitacaoVeiculo.Carga?.Codigo ?? 0;
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = (from o in listaCargaJanelaCarregamento where o.Carga.Codigo == codigoCargaFiltrarJanelaCarregamento select o).FirstOrDefault();

            return new
            {
                Codigo = solicitacaoVeiculo.Codigo,
                Carga = servicoCarga.ObterNumeroCarga(solicitacaoVeiculo.Carga, configuracaoEmbarcador),
                CodigosAgrupadosCarga = solicitacaoVeiculo.Carga == null ? "" : string.Join(", ", solicitacaoVeiculo.Carga.CodigosAgrupados),
                DataSolicitacaoVeiculoIniciada = solicitacaoVeiculo.DataSolicitacaoVeiculoIniciada?.ToString("dd/MM/yyyy") ?? "",
                DataEntregaGuarita = solicitacaoVeiculo.FluxoGestaoPatio?.DataEntregaGuarita?.ToString("dd/MM/yyyy HH:mm") ?? "",
                Situacao = solicitacaoVeiculo.Situacao.ObterDescricao(),
                Destino = solicitacaoVeiculo.Carga?.DadosSumarizados?.Destinos ?? "",
                Doca = !string.IsNullOrWhiteSpace(solicitacaoVeiculo.Carga?.NumeroDocaEncosta) ? solicitacaoVeiculo.Carga?.NumeroDocaEncosta : solicitacaoVeiculo.Carga?.NumeroDoca ?? string.Empty,
                TempoJanela = cargaJanelaCarregamento?.InicioCarregamento.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                Veiculo = solicitacaoVeiculo.Carga?.RetornarPlacas,
                Transportador = solicitacaoVeiculo.Carga?.Empresa?.Descricao ?? string.Empty,
                ModeloVeiculo = solicitacaoVeiculo.Carga?.ModeloVeicularCarga?.Descricao ?? string.Empty,
                TipoOperacao = solicitacaoVeiculo.Carga?.TipoOperacao?.Descricao ?? string.Empty,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? string.Empty
            };
        }

        private dynamic ObterSolicitacaoVeiculoPorCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo)
        {

            int codigoFluxoGestaoPatio = Request.GetIntParam("FluxoGestaoPatio");

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorCarga(solicitacaoVeiculo.Carga.Codigo);

            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repFluxoGestaoPatio.BuscarPorCodigo(codigoFluxoGestaoPatio);

            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciasGestaoPatio = ObterSequenciaGestaoPatio(unitOfWork, fluxoGestaoPatio);

            bool permitirEditarEtapa = IsPermitirEditarEtapa(solicitacaoVeiculo.FluxoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                solicitacaoVeiculo.Codigo,
                solicitacaoVeiculo.Situacao,
                Carga = solicitacaoVeiculo.Carga.Codigo,
                PreCarga = solicitacaoVeiculo.PreCarga?.Codigo ?? 0,
                NumeroCarga = servicoCarga.ObterNumeroCarga(solicitacaoVeiculo.Carga, unitOfWork),
                NumeroPreCarga = solicitacaoVeiculo.PreCarga?.NumeroPreCarga ?? "",
                CargaData = solicitacaoVeiculo.Carga.DataCarregamentoCarga?.ToString($"dd/MM/yyyy") ?? "",
                CargaHora = solicitacaoVeiculo.Carga.DataCarregamentoCarga?.ToString($"HH:mm") ?? "",
                Transportador = solicitacaoVeiculo.Carga.Empresa?.Descricao ?? string.Empty,
                Veiculo = solicitacaoVeiculo.Carga.RetornarPlacas,
                Remetente = solicitacaoVeiculo.Carga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = solicitacaoVeiculo.Carga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = solicitacaoVeiculo.Carga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = solicitacaoVeiculo.Carga.DadosSumarizados?.Destinatarios ?? string.Empty,
                CodigoIntegracaoDestinatario = solicitacaoVeiculo.Carga.DadosSumarizados?.CodigoIntegracaoDestinatarios ?? string.Empty,
                PermitirEditarEtapa = permitirEditarEtapa,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? "",
                NaoPermitirEnviarSMS = sequenciasGestaoPatio?.NaoPermitirEnviarSMS ?? false,
                PermitirInformarDadosTransporte = sequenciasGestaoPatio?.SolicitacaoVeiculoPermitirInformarDadosTransporteCarga ?? false,
            };

            return docaCarregamentoRetornar;
        }

        private Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio ObterSequenciaGestaoPatio(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            Repositorio.Embarcador.Filiais.SequenciaGestaoPatio repositorioSequenciaGestaoPatio = new Repositorio.Embarcador.Filiais.SequenciaGestaoPatio(unitOfWork);
            List<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio> sequenciasGestaoPatio = repositorioSequenciaGestaoPatio.BuscarTodosPorFilialETipo(fluxoGestaoPatio.Filial.Codigo, fluxoGestaoPatio.Tipo);

            return sequenciasGestaoPatio.Where(o => o.TipoOperacao == null || o.TipoOperacao.Codigo == fluxoGestaoPatio.CargaBase.TipoOperacao?.Codigo).OrderBy(o => o.TipoOperacao == null).FirstOrDefault();
        }

        private dynamic ObterSolicitacaoVeiculoPorPreCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.GestaoPatio.SolicitacaoVeiculo solicitacaoVeiculo)
        {
            Repositorio.Embarcador.Cargas.CargaJanelaCarregamento repositorioCargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = repositorioCargaJanelaCarregamento.BuscarPorPreCarga(solicitacaoVeiculo.PreCarga.Codigo);
            DateTime? dataCarregamento = cargaJanelaCarregamento?.InicioCarregamento;
            bool permitirEditarEtapa = IsPermitirEditarEtapa(solicitacaoVeiculo.FluxoGestaoPatio);

            var docaCarregamentoRetornar = new
            {
                solicitacaoVeiculo.Codigo,
                solicitacaoVeiculo.Situacao,
                Carga = 0,
                PreCarga = solicitacaoVeiculo.PreCarga.Codigo,
                NumeroCarga = "",
                NumeroPreCarga = solicitacaoVeiculo.PreCarga.NumeroPreCarga ?? "",
                CargaData = dataCarregamento?.ToString($"dd/MM/yyyy") ?? "",
                CargaHora = dataCarregamento?.ToString($"HH:mm") ?? "",
                Transportador = solicitacaoVeiculo.PreCarga.Empresa?.Descricao ?? string.Empty,
                Veiculo = solicitacaoVeiculo.PreCarga.RetornarPlacas,
                Remetente = solicitacaoVeiculo.PreCarga.DadosSumarizados?.Remetentes ?? string.Empty,
                TipoCarga = solicitacaoVeiculo.PreCarga.TipoDeCarga?.Descricao ?? string.Empty,
                TipoOperacao = solicitacaoVeiculo.PreCarga.TipoOperacao?.Descricao ?? string.Empty,
                Destinatario = solicitacaoVeiculo.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.Nome ?? string.Empty,
                CodigoIntegracaoDestinatario = solicitacaoVeiculo.PreCarga.Pedidos?.FirstOrDefault()?.Destinatario?.CodigoIntegracao ?? string.Empty,
                PermitirEditarEtapa = permitirEditarEtapa,
                ObservacaoFluxoPatio = cargaJanelaCarregamento?.ObservacaoFluxoPatio ?? ""
            };

            return docaCarregamentoRetornar;
        }

        private bool IsPermitirEditarEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if ((fluxoGestaoPatio == null) || (fluxoGestaoPatio.SituacaoEtapaFluxoGestaoPatio != SituacaoEtapaFluxoGestaoPatio.Aguardando))
                return false;

            return (fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual == EtapaFluxoGestaoPatio.SolicitacaoVeiculo);
        }

        private string ObterFotoBase64(string guidArquivo, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = ObterCaminhoArquivoFoto(unitOfWork);

            string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{guidArquivo}.*").FirstOrDefault();

            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return "";

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return base64ImageRepresentation;
        }

        private string ObterCaminhoArquivoFoto(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "DadosTransporteFluxoPatio" });
        }
        #endregion
    }
}
