using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao" }, "Veiculos/Veiculo")]
    public class VeiculoIntegracaoController : BaseController
    {
        #region Construtores

        public VeiculoIntegracaoController(Conexao conexao) : base(conexao) { }

        #endregion Construtores

        #region Métodos Globais

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

                int.TryParse(Request.Params("Codigo"), out int codigo);


                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> integracoesArquivos = repVeiculo.BuscarArquivosPorIntergacao(codigo, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repVeiculo.ContarBuscarArquivosPorIntergacao(codigo));

                var retorno = (from obj in integracoesArquivos
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = repVeiculo.BuscarIntergacaoPorCodigo(codigo);

                if (arquivoIntegracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                if (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Veiculo.zip");
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

        public async Task<IActionResult> Integrar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Veiculos.VeiculoIntegracao repositorioVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao = await repositorioVeiculoIntegracao.BuscarPorCodigoAsync(codigo, auditavel: false);

                if (integracao == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (integracao.Veiculo.SituacaoCadastro == SituacaoCadastroVeiculo.Pendente)
                    throw new ControllerException(Localization.Resources.Veiculos.Veiculo.NaoFoiPossivelReenviarIntegracaoEnquantoCadastroDoVeiculoEstiverPendente);

                await unitOfWork.StartAsync(cancellationToken);

                integracao.DataIntegracao = DateTime.Now;

                switch (integracao.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.Email:
                        Servicos.Embarcador.Veiculo.Veiculo.IntegrarEmailVeiculoNovo(integracao, unitOfWork);
                        break;
                    case TipoIntegracao.Buonny:
                        {
                            integracao.NumeroTentativas++;
                            await IntegracaoBuonny(integracao, unitOfWork, cancellationToken);

                            break;
                        }
                    case TipoIntegracao.BuonnyRNTRC:
                        {
                            integracao.NumeroTentativas++;
                            await IntegracaoBuonnyRNRTC(integracao, unitOfWork, cancellationToken);
                            break;
                        }
                    case TipoIntegracao.BrasilRiskGestao:
                        {
                            integracao.NumeroTentativas++;
                            await IntegracaoBrasilRiskGestao(integracao, unitOfWork, cancellationToken);

                            break;
                        }
                    case TipoIntegracao.Frota162:
                        {
                            Servicos.Embarcador.Integracao.Frota162.IntegracaoFrota162 servicoFrota162 = new Servicos.Embarcador.Integracao.Frota162.IntegracaoFrota162(unitOfWork);

                            if (integracao.Veiculo.Status == "I")
                            {
                                servicoFrota162.InativarVeiculo(integracao);
                                break;
                            }

                            servicoFrota162.IntegrarVeiculo(integracao);
                            break;
                        }
                    case TipoIntegracao.MultiEmbarcador:
                        Servicos.Embarcador.Integracao.MultiEmbarcador.Veiculo.IntegrarVeiculoEmbarcador(integracao, unitOfWork);
                        break;
                    case TipoIntegracao.Ultragaz:
                        new Servicos.Embarcador.Integracao.Ultragaz.IntegracaoUltragaz(unitOfWork, TipoServicoMultisoftware).IntegrarVeiculo(integracao);
                        break;
                    case TipoIntegracao.SemParar:
                    case TipoIntegracao.EFrete:
                        new Servicos.Embarcador.Integracao.SemParar.ConsultaCadastroVeiculo(unitOfWork, TipoServicoMultisoftware).GerarIntegracaoCadastroVeiculo(integracao.Veiculo);
                        break;
                    case TipoIntegracao.KMM:
                        new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unitOfWork, TipoServicoMultisoftware).IntegrarVeiculo(integracao);
                        break;
                    case TipoIntegracao.BrasilRiskVeiculoMotorista:
                        new Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk(unitOfWork).CadastrarVeiculoAnalisePerfil(integracao);
                        break;
                    default:
                        {
                            integracao.NumeroTentativas++;
                            integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                            integracao.ProblemaIntegracao = Localization.Resources.Gerais.Geral.IntegracaoNaoDisponivel;

                            break;
                        }
                }

                await repositorioVeiculoIntegracao.AtualizarAsync(integracao);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                await unitOfWork.RollbackAsync(cancellationToken);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaVeiculoIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Placa, "Placa", 13, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Integracao, "TipoIntegracao", 13, Models.Grid.Align.left, true);

                if (repTipoIntegracao.ExistePorTipo(TipoIntegracao.MultiEmbarcador))
                    grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.GrupoPessoas, "GrupoPessoas", 13, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.Tentativas, "NumeroTentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Veiculos.Veiculo.DataDoEnvio, "DataIntegracao", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "SituacaoIntegracao", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Mensagem, "Retorno", 30, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                Repositorio.Embarcador.Veiculos.VeiculoIntegracao repositorioVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork);
                string propriedadeOrdenar = ObterPropriedadeOrdenarPesquisaVeiculoIntegracoes(grid.header[grid.indiceColunaOrdena].data);
                List<Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao> listaIntegracoes = repositorioVeiculoIntegracao.Consultar(codigo, situacao, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalIntegracoes = repositorioVeiculoIntegracao.ContarConsulta(codigo, situacao);

                var listaIntegracoesRetornar = (
                    from integracao in listaIntegracoes
                    select new
                    {
                        integracao.Codigo,
                        Situacao = integracao.SituacaoIntegracao,
                        SituacaoIntegracao = integracao.DescricaoSituacaoIntegracao,
                        TipoIntegracao = integracao.TipoIntegracao.DescricaoTipo,
                        GrupoPessoas = integracao.GrupoPessoas?.Descricao ?? string.Empty,
                        Retorno = integracao.ProblemaIntegracao,
                        integracao.NumeroTentativas,
                        DataIntegracao = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                        DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                        DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte(),
                        Placa = integracao.Veiculo.Placa_Formatada ?? ""
                    }
                ).ToList();

                grid.AdicionaRows(listaIntegracoesRetornar);
                grid.setarQuantidadeTotal(totalIntegracoes);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTotaisIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Veiculos.VeiculoIntegracao repositorioVeiculoIntegracao = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repositorioVeiculoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repositorioVeiculoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repositorioVeiculoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repositorioVeiculoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgRetorno);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoObterOsTotaisDasIntegracoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ProblemaIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string motivo = Request.GetStringParam("Motivo");

                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, true, Localization.Resources.Veiculos.Veiculo.MotivoDeveSerInformado);

                Repositorio.Embarcador.Veiculos.VeiculoIntegracao repositorio = new Repositorio.Embarcador.Veiculos.VeiculoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao = repositorio.BuscarPorCodigo(codigo);

                if (integracao == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas += 1;
                integracao.ProblemaIntegracao = motivo.Trim();
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                repositorio.Atualizar(integracao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private string ObterPropriedadeOrdenarPesquisaVeiculoIntegracoes(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "TipoIntegracao")
                return "TipoIntegracao.Tipo";

            return propriedadeOrdenar;
        }

        private async Task IntegracaoBrasilRiskGestao(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork, cancellationToken);

            string mensagemErro = string.Empty;
            string xmlRequest = string.Empty;
            string xmlResponse = string.Empty;

            Servicos.ServicoBrasilRisk.GestaoAnaliseDePerfil.RetornoConsulta retorno = Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk.ConsultaVeiculo(integracao.Veiculo.Placa, ref mensagemErro, ref xmlRequest, ref xmlResponse, unitOfWork);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
            {
                integracao.ProblemaIntegracao = mensagemErro.Length > 300 ? mensagemErro.Substring(0, 300) : mensagemErro;
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            else if (retorno == null)
            {
                integracao.ProblemaIntegracao = Localization.Resources.Gerais.Geral.IntegracaoNaoTeveRetorno;
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            else if (retorno.Status)
            {
                integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
            else
            {
                integracao.ProblemaIntegracao = retorno.Mensagem;
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }

            string stringRetorno = string.Empty;

            if (retorno != null)
                stringRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();

            integracaoArquivo.Mensagem = !string.IsNullOrWhiteSpace(stringRetorno) ? Utilidades.String.Left(stringRetorno, 400) : integracao.ProblemaIntegracao;
            integracaoArquivo.Data = DateTime.Now;
            integracaoArquivo.Tipo = TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

            integracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
            integracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork);

            await repositorioCargaCTeIntegracaoArquivo.InserirAsync(integracaoArquivo);

            integracao.ArquivosTransacao ??= new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            integracao.ArquivosTransacao.Add(integracaoArquivo);
        }

        private async Task IntegracaoBuonnyRNRTC(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork, cancellationToken);

            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusRNTRC retorno = Servicos.Embarcador.Integracao.Buonny.IntegracaoBuonny.StatusRNTRC(integracao.Veiculo.Placa, ref mensagemErro, unitOfWork);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
            {
                integracao.ProblemaIntegracao = mensagemErro.Length > 300 ? mensagemErro.Substring(0, 300) : mensagemErro;
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            else if (retorno == null)
            {
                integracao.ProblemaIntegracao = Localization.Resources.Gerais.Geral.IntegracaoNaoTeveRetorno;
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            else if (!string.IsNullOrWhiteSpace(retorno.validado) && retorno.validado.ToUpper() == "S")
            {
                integracao.ProblemaIntegracao = Localization.Resources.Veiculos.Veiculo.RntrcValida;
                integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
            else
            {
                integracao.ProblemaIntegracao = Localization.Resources.Veiculos.Veiculo.VeiculoNaoPossuiRntrcValidaNaBuonny;
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }

            string stringRetorno = string.Empty;

            if (retorno != null)
                stringRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();

            integracaoArquivo.Mensagem = !string.IsNullOrWhiteSpace(stringRetorno) ? Utilidades.String.Left(stringRetorno, 400) : integracao.ProblemaIntegracao;
            integracaoArquivo.Data = DateTime.Now;
            integracaoArquivo.Tipo = TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

            await repositorioCargaCTeIntegracaoArquivo.InserirAsync(integracaoArquivo);

            integracao.ArquivosTransacao ??= new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            integracao.ArquivosTransacao.Add(integracaoArquivo);
        }

        private async Task IntegracaoBuonny(Dominio.Entidades.Embarcador.Veiculos.VeiculoIntegracao integracao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork, cancellationToken);

            string mensagemErro = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Buonny.RetornoStatusChecklist retorno = Servicos.Embarcador.Integracao.Buonny.IntegracaoBuonny.StatusChecklist(integracao.Veiculo.Placa, ref mensagemErro, unitOfWork);

            if (!string.IsNullOrWhiteSpace(mensagemErro))
            {
                integracao.ProblemaIntegracao = mensagemErro.Length > 300 ? mensagemErro.Substring(0, 300) : mensagemErro;
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            else if (retorno == null)
            {
                integracao.ProblemaIntegracao = Localization.Resources.Gerais.Geral.IntegracaoNaoTeveRetorno;
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            else if (!string.IsNullOrWhiteSpace(retorno.status) && retorno.status.ToUpper() == "S")
            {
                integracao.ProblemaIntegracao = string.Format(Localization.Resources.Gerais.Geral.ChecklistValido, retorno.data_checklist);
                integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
            }
            else
            {
                integracao.ProblemaIntegracao = Localization.Resources.Veiculos.Veiculo.VeiculoNaoPossuiChecklistValidoNaBuonny;
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }

            string stringRetorno = string.Empty;

            if (retorno != null)
                stringRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();

            integracaoArquivo.Mensagem = !string.IsNullOrWhiteSpace(stringRetorno) ? Utilidades.String.Left(stringRetorno, 400) : integracao.ProblemaIntegracao;
            integracaoArquivo.Data = DateTime.Now;
            integracaoArquivo.Tipo = TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

            await repositorioCargaCTeIntegracaoArquivo.InserirAsync(integracaoArquivo);

            integracao.ArquivosTransacao ??= new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            integracao.ArquivosTransacao.Add(integracaoArquivo);
        }

        #endregion Métodos Privados
    }
}
