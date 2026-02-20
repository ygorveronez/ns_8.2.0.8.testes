using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao", "ObterTotaisIntegracoes", "ObterCodigosMotoristasCargas" }, "Transportadores/Motorista")]
    public class MotoristaIntegracaoController : BaseController
    {
        #region Construtores

        public MotoristaIntegracaoController(Conexao conexao) : base(conexao) { }

        #endregion Construtores

        #region Métodos Globais

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCTeIntegracao repCargaCTeIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracao(unidadeDeTrabalho);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);

                int.TryParse(Request.Params("Codigo"), out int codigo);


                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> integracoesArquivos = repMotorista.BuscarArquivosPorIntergacao(codigo, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repMotorista.ContarBuscarArquivosPorIntergacao(codigo));

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

                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = repMotorista.BuscarIntergacaoPorCodigo(codigo);

                if (arquivoIntegracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                if (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Motorista.zip");
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

                Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorioMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao = await repositorioMotoristaIntegracao.BuscarPorCodigoAsync(codigo, auditavel: false);

                if (integracao == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.RegistroNaoEncontrado);

                await unitOfWork.StartAsync(cancellationToken);

                integracao.DataIntegracao = DateTime.Now;
                integracao.ProblemaIntegracao = string.Empty;
                integracao.Protocolo = string.Empty;
                integracao.Mensagem = string.Empty;
                integracao.DescricaoTipo = string.Empty;

                switch (integracao.TipoIntegracao.Tipo)
                {
                    case TipoIntegracao.Buonny:
                        {
                            integracao.NumeroTentativas++;

                            Servicos.Embarcador.Integracao.Buonny.IntegracaoBuonny.StatusMotorista(ref integracao, TipoServicoMultisoftware, unitOfWork);
                            break;
                        }
                    case TipoIntegracao.Telerisco:
                        {
                            integracao.NumeroTentativas++;

                            await IntegracaoTeleriscoAsync(integracao, unitOfWork, cancellationToken);
                            break;
                        }
                    case TipoIntegracao.BrasilRiskGestao:
                        {
                            integracao.NumeroTentativas++;

                            await IntegracaoBrasilRiskGestaoAsync(integracao, unitOfWork, cancellationToken);
                            break;
                        }
                    case TipoIntegracao.Frota162:
                        new Servicos.Embarcador.Integracao.Frota162.IntegracaoFrota162(unitOfWork).IntegrarMotorista(integracao);
                        break;
                    case TipoIntegracao.KMM:
                        new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unitOfWork).IntegrarPessoaMotorista(integracao);
                        break;
                    case TipoIntegracao.BrasilRiskVeiculoMotorista:
                        new Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk(unitOfWork).CadastrarMotoristaAnalisePerfil(integracao);
                        break;
                    default:
                        {
                            integracao.NumeroTentativas++;
                            integracao.ProblemaIntegracao = Localization.Resources.Gerais.Geral.IntegracaoNaoDisponivel;
                            integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                            break;
                        }
                }

                await repositorioMotoristaIntegracao.AtualizarAsync(integracao);

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

                return new JsonpResult(false, Localization.Resources.Transportadores.Motorista.OcorreuUmaFalhaAoAdicionarIntegracao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaMotoristaIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Nome, "NomeMotorista", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CPF", "CPFMotorista", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Motorista.Integracao, "TipoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Motorista.Tentativas, "NumeroTentativas", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Motorista.DataDoEnvio, "DataIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "SituacaoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Mensagem, "Retorno", 30, Models.Grid.Align.left, false);


                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                List<int> codigos = ObterCodigosMotoristas();

                Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorioMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unitOfWork);
                string propriedadeOrdenar = ObterPropriedadeOrdenarPesquisaMotoristaIntegracoes(grid.header[grid.indiceColunaOrdena].data);
                List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> listaIntegracoes = repositorioMotoristaIntegracao.Consultar(codigos, situacao, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalIntegracoes = repositorioMotoristaIntegracao.ContarConsulta(codigos, situacao);

                var listaIntegracoesRetornar = (
                    from integracao in listaIntegracoes
                    select new
                    {
                        integracao.Codigo,
                        Situacao = integracao.SituacaoIntegracao,
                        SituacaoIntegracao = integracao.DescricaoSituacaoIntegracao,
                        TipoIntegracao = integracao.TipoIntegracao.DescricaoTipo,
                        Retorno = integracao.ProblemaIntegracao,
                        integracao.NumeroTentativas,
                        DataIntegracao = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                        DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                        DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte(),
                        NomeMotorista = integracao.Motorista?.Nome ?? "",
                        CPFMotorista = integracao.Motorista.CPF_CNPJ_Formatado ?? ""
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

        public async Task<IActionResult> ObterCodigosMotoristasCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                int codigo = Request.GetIntParam("CodigoCarga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorio = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigo);

                if (carga == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);

                List<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> motoristaIntegracao = repositorio.BuscarPorMotoristas(carga.Motoristas.Select(m => m.Codigo).ToList());

                var retorno = (
                    from motorista in motoristaIntegracao
                    select new
                    {
                        motorista.Motorista.Codigo
                    }
                ).ToList();

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> ObterTotaisIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                List<int> codigos = ObterCodigosMotoristas();

                Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorioMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repositorioMotoristaIntegracao.ContarConsulta(codigos, SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repositorioMotoristaIntegracao.ContarConsulta(codigos, SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repositorioMotoristaIntegracao.ContarConsulta(codigos, SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repositorioMotoristaIntegracao.ContarConsulta(codigos, SituacaoIntegracao.AgRetorno);

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
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Motorista.MotivoDeveSerInformado);

                Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorio = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao = repositorio.BuscarPorCodigo(codigo);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private string ObterPropriedadeOrdenarPesquisaMotoristaIntegracoes(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "TipoIntegracao")
                return "TipoIntegracao.Tipo";

            return propriedadeOrdenar;
        }

        private List<int> ObterCodigosMotoristas()
        {
            int codigo = Request.GetIntParam("Codigo");
            dynamic listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.GetStringParam("Codigos"));

            List<int> codigos = new List<int>();

            if (codigo > 0 && codigos.Count == 0)
                codigos.Add(codigo);

            if (listaItensSelecionados != null)
                foreach (dynamic codigoMotorista in listaItensSelecionados)
                    codigos.Add((int)codigoMotorista);

            return codigos;
        }

        private async Task IntegracaoBrasilRiskGestaoAsync(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork, cancellationToken);

            string mensagemErro = string.Empty;
            string xmlRequest = string.Empty;
            string xmlResponse = string.Empty;

            Servicos.ServicoBrasilRisk.GestaoAnaliseDePerfil.RetornoConsulta retorno = Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk.ConsultaMotorista(integracao.Motorista.CPF, ref mensagemErro, ref xmlRequest, ref xmlResponse, unitOfWork);

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
                integracao.Mensagem = retorno.Mensagem;
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

        private async Task IntegracaoTeleriscoAsync(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork, cancellationToken);
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork, cancellationToken);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork, cancellationToken);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = await repIntegracao.BuscarPrimeiroRegistroAsync();
            Dominio.Entidades.Embarcador.Filiais.Filial matriz = await repositorioFilial.BuscarMatrizAsync();
            Dominio.Entidades.Veiculo veiculo = await repositorioVeiculo.BuscarPorMotoristaAsync(integracao.Motorista.Codigo);

            string mensagemErro = string.Empty;
            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.ConsultaMotoristaResponse retorno = Servicos.Embarcador.Integracao.Telerisco.IntegracaoTelerisco.ConsultaMotorista(integracao.Motorista, matriz, DateTime.MinValue, ref mensagemErro, ref jsonRequest, ref jsonResponse, TipoServicoMultisoftware, unitOfWork, veiculo?.Placa);

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
            else if (retorno.retornoWs == "1")
            {
                if (retorno.consulta == "1")
                {
                    if (configuracaoIntegracao != null && !string.IsNullOrWhiteSpace(configuracaoIntegracao.CodigosAceitosRetornoTelerisco))
                    {
                        if (configuracaoIntegracao.CodigosAceitosRetornoTelerisco.Contains(retorno.categoriaResultado))
                        {
                            integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : "Consulta retornada com sucesso";
                            integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                            integracao.Protocolo = retorno.protocolo;
                            integracao.Mensagem = retorno.consultaMensagem;
                            integracao.DescricaoTipo = retorno.tipoMotorista?.ToString() ?? retorno.mensagemRetorno;
                        }
                        else
                        {
                            integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : !string.IsNullOrWhiteSpace(retorno.resultado) ? retorno.resultado : "Consulta sem mensagem de retorno";
                            integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                        }
                    }
                    else if (retorno.categoriaResultado == "350" || retorno.categoriaResultado == "250" || retorno.categoriaResultado == "500" || retorno.categoriaResultado == "400" || retorno.categoriaResultado == "280" || retorno.categoriaResultado == "200" || retorno.categoriaResultado == "100")
                    {
                        integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : Localization.Resources.Transportadores.Motorista.ConsultaRetornadaComSucesso;
                        integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        integracao.Protocolo = retorno.protocolo;
                        integracao.Mensagem = retorno.consultaMensagem;
                        integracao.DescricaoTipo = retorno.tipoMotorista.ToString();
                    }
                    else
                    {
                        integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : Localization.Resources.Transportadores.Motorista.ConsultaSemMensagemDeRetorno;
                        integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
                else if (retorno.consulta == "2")
                {
                    integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : Localization.Resources.Transportadores.Motorista.ConsultaNaoLocalizada;
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                else if (retorno.consulta == "3")
                {
                    integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : Localization.Resources.Transportadores.Motorista.PerfilAtualDoMotoristaDiferenteDaUltimaConsutaNecessarioNovaConsulta;
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                else if (retorno.consulta == "4")
                {
                    integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : Localization.Resources.Transportadores.Motorista.MotoristaComRestricaoComEmpresaTransportadorEmbarcador;
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consulta + " - " + retorno.consultaMensagem : string.Format(Localization.Resources.Transportadores.Motorista.CodigoConsultaNaoPrevistoNoManualDaTelerisco, retorno.consulta);
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            else
            {
                integracao.ProblemaIntegracao = retorno.mensagemRetorno;
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }

            string stringRetorno = string.Empty;
            if (retorno != null)
                stringRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
            integracaoArquivo.Mensagem = !string.IsNullOrWhiteSpace(stringRetorno) ? Utilidades.String.Left(stringRetorno, 400) : integracao.ProblemaIntegracao;
            integracaoArquivo.Data = DateTime.Now;
            integracaoArquivo.Tipo = TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

            integracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
            integracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

            await repositorioCargaCTeIntegracaoArquivo.InserirAsync(integracaoArquivo);

            integracao.ArquivosTransacao ??= new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();
            integracao.ArquivosTransacao.Add(integracaoArquivo);
        }

        #endregion Métodos Privados
    }
}
