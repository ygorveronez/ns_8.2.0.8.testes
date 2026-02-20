using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Integracoes
{
    [CustomAuthorize("Integracoes/IntegracaoEnvioProgramado")]
    public class IntegracaoEnvioProgramadoController : BaseController
    {
        #region Construtores

        public IntegracaoEnvioProgramadoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Metodos Publicos

        public async Task<IActionResult> Pesquisar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repositorioIntegracao = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

                List<TipoEntidadeIntegracao> tiposEntidadeIntegracao = repositorioIntegracao.BuscarOrigensIntegracaoDisponiveis();

                Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaIntegracaoEnvioProgramado filtroPequisa = ObterFiltrosPesquisa();
                Models.Grid.Grid grid = ObterGrid(tiposEntidadeIntegracao, filtroPequisa);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "IntegracaoEnvioProgramado/Pesquisar", "grid-integracao-envio-programado");
                Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid preferenciasGrid = gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo);
                grid.AplicarPreferenciasGrid(preferenciasGrid);

                int totalRegitrosIntegracao = repositorioIntegracao.ContarConsultar(filtroPequisa);
                List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> integracoes = totalRegitrosIntegracao > 0 ? repositorioIntegracao.Consultar(filtroPequisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado>();

                List<int> codigosCarga = integracoes.Where(i => i?.TipoEntidadeIntegracao == TipoEntidadeIntegracao.Carga).Select(i => i.Carga.Codigo).Distinct().ToList();
                List<Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoOcorrenciaCargaEnvioProgramado> ocorrenciasCarga = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoOcorrenciaCargaEnvioProgramado>();

                if (codigosCarga.Count > 0)
                {
                    ocorrenciasCarga = repositorioCargaOcorrencia.BuscarOcorrenciasPorCodigosCarga(codigosCarga);
                }                

                grid.setarQuantidadeTotal(totalRegitrosIntegracao);
                grid.AdicionaRows(FormatarRetorno(integracoes, ocorrenciasCarga));

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
        }
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repIntegracao = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.HistoricoNaoEncontrado);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.NaoHaRegistrosParaEsseHistorico);

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração - " + integracao.Carga.CodigoCargaEmbarcador + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repIntegracao = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracao = repIntegracao.BuscarPorCodigo(codigo, false);
                grid.setarQuantidadeTotal(integracao.ArquivosTransacao.Count);

                var retorno = (from obj in integracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repositorioIntegracao = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracao = repositorioIntegracao.BuscarPorCodigo(codigo, false);

                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenvio de integração", unitOfWork);

                repositorioIntegracao.Atualizar(integracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoReenviarArquivoParaIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarTodas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime dataInicial = Request.GetDateTimeParam("DataIntegracaoInicial");
                DateTime dataFinal = Request.GetDateTimeParam("DataIntegracaoFinal");

                if (dataFinal == DateTime.MinValue || dataInicial == DateTime.MinValue)
                    return new JsonpResult(false, true, "Necessário informar a Data Integração Inicial e Data Integração Final para solicitar o Reenvio.");


                Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repositorioIntegracao = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(unitOfWork);
                List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> integracoes = repositorioIntegracao.BuscarIntegracoesComFalha(dataInicial, dataFinal);

                if (integracoes.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracao in integracoes)
                    {
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenvio de integração", unitOfWork);
                        repositorioIntegracao.Atualizar(integracao);
                    }
                }
                else
                    return new JsonpResult(false, true, "Não há integrações com falha para o período de tempo informado (Data Integração Inicial e Data Integração Final)");

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoReenviarArquivoParaIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AnteciparEnvio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repositorioIntegracao = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracao = repositorioIntegracao.BuscarPorCodigo(codigo, true);

                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                integracao.EnvioAntecipado = true;
                integracao.EnvioBloqueado = false;
                integracao.DataIntegracao = DateTime.Now;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, integracao.GetChanges(), "Envio da integração antecipado", unitOfWork);

                repositorioIntegracao.Atualizar(integracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoReenviarArquivoParaIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Metodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaIntegracaoEnvioProgramado ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaIntegracaoEnvioProgramado()
            {
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroCTE = Request.GetIntParam("NumeroCTE"),
                NumeroOcorrencia = Request.GetIntParam("NumeroOcorrencia"),
                SituacaoIntegracao = Request.GetNullableEnumParam<SituacaoIntegracao>("SituacaoIntegracao"),
                TipoEntidadeIntegracao = Request.GetNullableEnumParam<TipoEntidadeIntegracao>("TipoEntidadeIntegracao"),
                DataIntegracaoInicial = Request.GetDateTimeParam("DataIntegracaoInicial"),
                DataIntegracaoFinal = Request.GetDateTimeParam("DataIntegracaoFinal"),
                DataProgramadaInicial = Request.GetDateTimeParam("DataProgramadaInicial"),
                DataProgramadaFinal = Request.GetDateTimeParam("DataProgramadaFinal"),
                DataCriacaoCargaInicial = Request.GetDateTimeParam("DataCriacaoCargaInicial"),
                DataCriacaoCargaFinal = Request.GetDateTimeParam("DataCriacaoCargaFinal"),
                DataEmissaoCTEInicial = Request.GetDateTimeParam("DataEmissaoCTEInicial"),
                DataEmissaoCTEFinal = Request.GetDateTimeParam("DataEmissaoCTEFinal"),
            };
        }

        private Models.Grid.Grid ObterGrid(List<TipoEntidadeIntegracao> tiposEntidadeIntegracao, Dominio.ObjetosDeValor.Embarcador.Integracoes.FiltroPesquisaIntegracaoEnvioProgramado filtroPequisa)
        {
            bool possuiOrigemCarga = tiposEntidadeIntegracao.Contains(TipoEntidadeIntegracao.Carga) || tiposEntidadeIntegracao.Contains(TipoEntidadeIntegracao.CTe);
            bool possuiOrigemOcorrencia = tiposEntidadeIntegracao.Contains(TipoEntidadeIntegracao.CargaOcorrencia);
            bool visualizarTempoEnvio = !filtroPequisa.SituacaoIntegracao.HasValue || filtroPequisa.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao;

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("EnvioAntecipado", false);

            if (possuiOrigemCarga)
            {
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.NumeroCarga, "NumeroCarga", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.CargasAgrupadas, "CargasAgrupamento", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.NumeroDocumentos, "NumeroCTEs", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.ProtocoloCarga, "ProtocoloCarga", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.DataCarregamento, "DataCarregamento", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.Tomador, "Tomador", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.NumeroOcorrencia, "NumeroOcorrencia", 10, Models.Grid.Align.left, false);
            }

            if (possuiOrigemOcorrencia)
            {
                grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.NumeroOcorrencia, "NumeroOcorrencia", 10, Models.Grid.Align.left, false);
            }

            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.SituacaoIntegracao, "SituacaoIntegracao", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.DataIntegracao, "DataIntegracao", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.DataEnvioPrevista, "DataEnvioProgramada", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.TempoParaEnvio, "TempoParaEnvio", 10, Models.Grid.Align.left, false, visualizarTempoEnvio);
            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.Retorno, "Retorno", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.NumeroEnvios, "NumeroEnvios", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.Mensagem, "Mensagem", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.Etapa, "Etapa", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.ValorCarga, "ValorCarga", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.OcorrenciaValorCarga, "OcorrenciaValorCarga", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.ValorTotal, "ValorTotal", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.Transportadora, "Transportadora", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.ModeloVeiculo, "ModeloVeiculo", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.Motorista, "Motorista", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.Placa, "Placa", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.Peso, "Peso", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Integracoes.IntegracaoEnvioProgramado.Filial, "Filial", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Tipo de Integração", "TipoIntegracao", 10, Models.Grid.Align.left, false, false);


            return grid;
        }

        private dynamic FormatarRetorno(List<Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado> integracoes, List<Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoOcorrenciaCargaEnvioProgramado> ocorrenciasCarga)
        {
            return integracoes.Select(obj =>
            {
                string numeroOcorrencia = string.Empty;
                decimal? valorOcorrencia = null;

                if (obj.TipoEntidadeIntegracao == TipoEntidadeIntegracao.CTe)
                {
                    numeroOcorrencia = obj.CargaOcorrencia?.NumeroOcorrencia.ToString() ?? string.Empty;
                    valorOcorrencia = obj.CargaOcorrencia?.ValorOcorrencia;
                }
                else if (obj.TipoEntidadeIntegracao == TipoEntidadeIntegracao.Carga) 
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoOcorrenciaCargaEnvioProgramado ocorrencia = ocorrenciasCarga.FirstOrDefault(o => o.CodigoCarga == obj.Carga.Codigo);
                    numeroOcorrencia = ocorrencia?.NumerosOcorrencia ?? string.Empty;
                    valorOcorrencia = ocorrencia?.ValorTotalOcorrencia;
                }

                return new
                {
                    obj.Codigo,
                    obj.EnvioAntecipado,
                    NumeroCarga = obj.Carga.CodigoCargaEmbarcador,
                    CargasAgrupamento = string.Join(", ", obj.Carga.CargasAgrupamento.Select(o => o.CodigoCargaEmbarcador)),
                    NumeroCTEs = obj.TipoEntidadeIntegracao == TipoEntidadeIntegracao.CTe ? obj.CTe?.Numero.ToString() : string.Join(", ", obj.Carga.CargaCTes.Select(o => o.CTe?.Numero)),
                    NumeroOcorrencia = numeroOcorrencia,
                    ProtocoloCarga = obj.Carga.Codigo,
                    DataCarregamento = obj.Carga.DataCarregamentoCarga?.ToString("g") ?? string.Empty,
                    SituacaoIntegracao = obj.SituacaoIntegracao.ObterDescricao(),
                    DataIntegracao = obj.DataIntegracao.ToString("g"),
                    DataEnvioProgramada = obj.DataEnvioProgramada.ToString("g"),
                    TempoParaEnvio = "",
                    Retorno = obj.ProblemaIntegracao ?? string.Empty,
                    NumeroEnvios = obj.NumeroTentativas,
                    Mensagem = obj.ProblemaIntegracao,
                    Etapa = obj.StepIntegracao,
                    Tomador = obj.CTe?.Tomador.Nome + " - " + obj.CTe?.Tomador.CPF_CNPJ_Formatado,
                    ValorCarga = obj.Carga.ValorFrete,
                    OcorrenciaValorCarga = valorOcorrencia,
                    ValorTotal = obj.Carga.ValorFrete + (valorOcorrencia ?? 0),
                    Transportadora = string.IsNullOrWhiteSpace(obj.Carga.DadosSumarizados?.PortalRetiraEmpresa) ? $"{obj.Carga.Empresa?.CodigoIntegracao ?? string.Empty} {obj.Carga.Empresa?.RazaoSocial} ({obj.Carga.Empresa?.Localidade.DescricaoCidadeEstado})" : obj.Carga.DadosSumarizados.PortalRetiraEmpresa,
                    ModeloVeiculo = obj.Carga.ModeloVeicularCarga != null ? $"({obj.Carga.ModeloVeicularCarga.CodigoIntegracao}) {obj.Carga.ModeloVeicularCarga.Descricao}" : "",
                    Motorista = obj.Carga.NomeMotoristas,
                    Placa = obj.Carga.PlacasVeiculos,
                    Peso = obj.Carga.DadosSumarizados?.PesoTotal,
                    Filial = obj.Carga.Filial?.Descricao,
                    TipoIntegracao = obj.TipoIntegracao.DescricaoTipo
                };

            });
        }

        #endregion
    }
}
