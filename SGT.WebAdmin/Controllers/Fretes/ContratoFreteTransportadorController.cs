using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Text;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize(new string[] { "Imprimir", "PesquisaAutorizacoes", "DetalhesAutorizacao" }, "Fretes/ContratoFreteTransportador")]
    public class ContratoFreteTransportadorController : BaseController
    {
        #region Construtores

        public ContratoFreteTransportadorController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Objetos JSON

        public class FiliaisContrato
        {
            public int Codigo { get; set; }
            public string Descricao { get; set; }
        }

        public class ClientesContrato
        {
            public double Codigo { get; set; }
            public string CPF_CNPJ { get; set; }
            public string Nome { get; set; }
            public string Localidade { get; set; }
        }

        public class OcorrenciaContrato
        {
            public int Codigo { get; set; }
            public string Descricao { get; set; }
            public string CodigoProceda { get; set; }
        }

        public class CanalEntregaContrato
        {
            public int Codigo { get; set; }
            public string Descricao { get; set; }
        }

        public class ModeloVeicularContrato
        {
            public int Codigo { get; set; }
            public string ModeloVeicular { get; set; }
            public string ValorDiaria { get; set; }
            public string ValorQuinzena { get; set; }
        }

        public class VeiculoContrato
        {
            public int Codigo { get; set; }
            public Dominio.ObjetosDeValor.Embarcador.Frete.TipoPagamentoContratoFrete TipoPagamentoContratoFrete { get; set; }
            public string Placa { get; set; }
            public string ModeloVeicularCarga { get; set; }
            public string DescricaoTipoPagamentoContratoFrete
            {
                get
                {
                    switch (TipoPagamentoContratoFrete)
                    {
                        case Dominio.ObjetosDeValor.Embarcador.Frete.TipoPagamentoContratoFrete.Diaria:
                            return "Diário";
                        case Dominio.ObjetosDeValor.Embarcador.Frete.TipoPagamentoContratoFrete.Quinzena:
                            return "Quinzenal";
                        default:
                            return "";
                    }
                }
                set
                {
                }
            }
        }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                // Cabecalhos grid
                grid.Prop("Codigo");
                grid.Prop("CPF_CNPJ").Nome("CNPJ").Tamanho(30).Align(Models.Grid.Align.center);
                grid.Prop("Nome").Nome("Nome").Tamanho(40).Align(Models.Grid.Align.left);
                grid.Prop("Localidade").Nome("Cidade").Tamanho(20).Align(Models.Grid.Align.left);

                string objClientes = Request.Params("Clientes");

                List<double> cnpjsClientes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<double>>(objClientes);
                List<Dominio.Entidades.Cliente> clientes = repCliente.BuscarPorVariosCPFCNPJ(cnpjsClientes);

                var lista = (from o in clientes
                             select new ClientesContrato
                             {
                                 CPF_CNPJ = o.CPF_CNPJ_Formatado,
                                 Nome = o.Nome,
                                 Localidade = o.Localidade.DescricaoCidadeEstado
                             }).ToList();

                // Seta valores na grid
                grid.AdicionaRows(lista);

                // Gera excel
                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", "Clientes Rateio.csv");
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

        [AllowAuthenticate]
        public async Task<IActionResult> ObterContratoFranquia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("PeriodoAcordo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Inicial", "DataInicial", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Final", "DataFinal", 12, Models.Grid.Align.left, true);

                int codigoTransportador = Request.GetIntParam("Empresa");
                string descricao = Request.GetStringParam("Descricao");
                DateTime? periodoInicio = Request.GetNullableDateTimeParam("PeriodoInicio");
                DateTime? periodoFim = Request.GetNullableDateTimeParam("PeriodoFim");
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                int totalRegistros = repositorioContratoFreteTransportador.ContarBuscaContratoFranquia(codigoTransportador, descricao, periodoInicio, periodoFim);
                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> contratos = totalRegistros > 0 ? repositorioContratoFreteTransportador.BuscarContratoFranquia(codigoTransportador, descricao, periodoInicio, periodoFim) : new List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();

                var contratosRetornar = (
                    from obj in contratos
                    select new
                    {
                        obj.Codigo,
                        obj.Numero,
                        obj.Descricao,
                        obj.PeriodoAcordo,
                        DataInicial = obj.DataInicial.ToString("dd/MM/yyyy"),
                        DataFinal = obj.DataFinal.ToString("dd/MM/yyyy"),
                    }
                ).ToList();

                grid.AdicionaRows(contratosRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha buscar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        //public async Task<IActionResult> ObterContratoFranquiadddd()
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
        //    try
        //    {
        //        // Instancia repositorios
        //        Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);

        //        // Parametros
        //        int empresa = Request.GetIntParam("Empresa");
        //        DateTime periodoInicio = Request.GetDateTimeParam("PeriodoInicio");
        //        DateTime periodoFim = Request.GetDateTimeParam("PeriodoFim");

        //        // Busca informacoes
        //        List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> contratos = repContratoFreteTransportador.BuscarContratoFranquia(empresa, periodoInicio, periodoFim);

        //        var lista = (from contrato in contratos
        //                     select new
        //                     {
        //                         contrato.Codigo,
        //                         Descricao = contrato.Descricao ?? string.Empty,
        //                     }).ToList();

        //        // Retorna informacoes
        //        return new JsonpResult(lista);
        //    }
        //    catch (Exception ex)
        //    {
        //        Servicos.Log.TratarErro(ex);
        //        return new JsonpResult(false, "Ocorreu uma falha ao buscar Contrato de Frete.");
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //}

        public async Task<IActionResult> PesquisaAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Respositorios
                Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Prioridade", "PrioridadeAprovacao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);

                // Ordenacao
                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                // Busca
                List<Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador> listaAutorizacao = repAprovacaoAlcadaContratoFreteTransportador.ConsultarAutorizacoesPorContrato(codigo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAprovacaoAlcadaContratoFreteTransportador.ContarConsultaAutorizacoesPorContrato(codigo));

                var lista = (from obj in listaAutorizacao
                             select new
                             {
                                 obj.Codigo,
                                 PrioridadeAprovacao = obj.RegraContratoFreteTransportador?.PrioridadeAprovacao ?? 0,
                                 Situacao = obj.DescricaoSituacao,
                                 obj.Bloqueada,
                                 Usuario = (obj.Usuario?.Nome ?? string.Empty) + (obj.Delegada ? " - (Delegada)" : ""),
                                 Regra = obj.Delegada ? "(Delegada)" : obj.RegraContratoFreteTransportador.Descricao,
                                 Data = obj.Data.HasValue ? obj.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                                 Motivo = obj.Motivo ?? string.Empty,
                                 DT_RowColor = CorAprovacao(obj)
                             }).ToList();
                grid.AdicionaRows(lista);

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

        public async Task<IActionResult> Imprimir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);
                bool.TryParse(Request.Params("Excell"), out bool excell);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = repContratoFreteTransportador.BuscarPorCodigo(codigo);

                // Valida
                if (contrato == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                byte[] arquivo = Servicos.Embarcador.Frete.ContratoFreteTransportador.RelatorioContratoFrete(contrato, excell, TipoServicoMultisoftware, unitOfWork);
                string extensao = "pdf";
                if (excell) extensao = "xls";
                // Retorna o arquivo
                return Arquivo(arquivo, "application/pdf", "Contrato de Frete Transportador - " + contrato.Numero + "." + extensao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = repContratoFreteTransportador.BuscarPorCodigo(codigo);

                // Valida
                if (contrato == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<string> emails = Servicos.Embarcador.Frete.ContratoFreteTransportador.ExtraiListaDeEmails(configuracao?.EmailsAvisoVencimentoCotratoFrete ?? "");

                string listaEmails = string.Join("; ", emails);

                Servicos.Embarcador.Frete.ContratoFreteTransportador.NotificarContratoProximoDoVencimento(contrato, configuracao?.DiasAvisoVencimentoCotratoFrete ?? 0, listaEmails, unitOfWork);
                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarIntegracaoContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Frete.ContratoFreteTransportadorIntegracao repContratoFreteTransportadorIntegracao = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorIntegracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao> integracaos = repContratoFreteTransportadorIntegracao.BuscarIntegracoesComFalha();

                if (integracaos != null && integracaos.Count > 0)
                {
                    foreach (var integracao in integracaos)
                    {
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                        repContratoFreteTransportadorIntegracao.Atualizar(integracao);
                    }
                }
                return new JsonpResult(true, true, "Sucesso.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Repositorio.Embarcador.Frete.ContratoFreteTransportadorIntegracao repContratoFreteTransportadorIntegracao = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorIntegracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao> integracaos = repContratoFreteTransportadorIntegracao.BuscarIntegracoesPorContrato(codigo);

                if (integracaos != null && integracaos.Count > 0)
                {
                    foreach (var integracao in integracaos)
                    {
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                        repContratoFreteTransportadorIntegracao.Atualizar(integracao);
                    }
                }
                return new JsonpResult(true, true, "Sucesso.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frete.ContratoFreteTransportadorIntegracao repIntegracao = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao integracao = repIntegracao.BuscarPorCodigo(codigo);

                if (integracao == null || integracao.ArquivosTransacao == null || integracao.ArquivosTransacao.Count == 0)
                    return new JsonpResult(true, false, "Histórico não localizado");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.LastOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Integração não possui nenhum arquivo de histórico");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "IntegracaoContratoFreteTransportador" + integracao.ContratoFreteTransportador.Descricao + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download do histórico");
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

                Repositorio.Embarcador.Frete.ContratoFreteTransportadorIntegracao repIntegracao = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorIntegracao(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Status", "Status", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modelo Veícular", "ModeloVeicular", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo de Carga", "TipoDeCarga", 15, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorIntegracao> integracao = repIntegracao.BuscarIntegracoesPorContrato(codigo);

                grid.setarQuantidadeTotal(integracao?.Count() ?? 0);

                if (integracao != null)
                {
                    var retorno = (from obj in integracao.ToList().Skip(grid.inicio).Take(grid.limite)
                                   select new
                                   {
                                       Codigo = obj?.Codigo ?? 0,
                                       Data = obj != null ? obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss") : "",
                                       Status = obj.DescricaoSituacaoIntegracao ?? "",
                                       Mensagem = obj != null ? obj.ProblemaIntegracao : "",
                                       ModeloVeicular = obj.ModeloVeicular?.Descricao ?? "",
                                       TipoDeCarga = obj.TipoDeCarga?.Descricao ?? ""
                                   }).ToList();

                    grid.AdicionaRows(retorno);
                }
                else
                    grid.AdicionaRows(null);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarRegras()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = repContratoFreteTransportador.BuscarPorCodigo(codigo);

                // Valida
                if (contrato == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
                if (contrato.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.SemRegra)
                    return new JsonpResult(false, true, "A situação não permite essa operação.");

                // Busca as regras
                EtapaAprovacao(contrato, unitOfWork);
                AtualizarDadosPorContratoAprovado(contrato, unitOfWork);
                repContratoFreteTransportador.Atualizar(contrato);
                bool possuiRegra = contrato.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.SemRegra;
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(new
                {
                    contrato.Codigo,
                    PossuiRegra = possuiRegra,
                    Situacao = contrato.Situacao,
                    Resumo = !possuiRegra ? null : ResumoAutorizacao(contrato, unitOfWork)
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia
                Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);

                // Converte dados
                int codigoAutorizacao = int.Parse(Request.Params("Codigo"));

                // Busca a autorizacao
                Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador autorizacao = repAprovacaoAlcadaContratoFreteTransportador.BuscarPorCodigo(codigoAutorizacao);

                var retorno = new
                {
                    autorizacao.Codigo,
                    Regra = autorizacao.Delegada ? "(Delegada)" : autorizacao.RegraContratoFreteTransportador.Descricao,
                    Situacao = autorizacao.DescricaoSituacao,
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,
                    PermitirDelegar = (!autorizacao.Bloqueada && (autorizacao.TipoAprovadorRegra == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAprovadorRegra.Usuario)),
                    PodeAprovar = !autorizacao.Bloqueada && (autorizacao.Usuario != null && autorizacao.Usuario.Codigo == this.Usuario.Codigo && autorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente),
                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = !string.IsNullOrWhiteSpace(autorizacao.Motivo) ? autorizacao.Motivo : string.Empty,
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                bool duplicar = Request.GetBoolParam("Duplicar");
                DateTime? dataInicial = Request.GetNullableDateTimeParam("DataInicialContrato");

                Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = repositorioContratoFreteTransportador.BuscarPorCodigo(codigo);

                if (contrato == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Frete.ContratoFreteTransportadorAnexo repositorioContratoFreteTransportadorAnexo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorAnexo(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro repositorioContratoFrete = new Repositorio.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo> anexos = repositorioContratoFreteTransportadorAnexo.BuscarPorContrato(contrato.Codigo);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoContratoFreteTerceiro existeConfiguracao = repositorioContratoFrete.BuscarConfiguracaoPadrao();
                int valorKmUtilizado = 0;
                decimal valorPago = 0m;

                if (configuracaoEmbarcador.ExibirKmUtilizadoContratoFretePorPeriodoVigenciaContrato)
                {
                    valorKmUtilizado = Servicos.Embarcador.Carga.ContratoFrete.ObterKmUtilizadoContratoFreteNoPeriodoVigencia(contrato, unitOfWork);
                    valorPago = Servicos.Embarcador.Carga.ContratoFrete.ObterValorPagoContratoFreteNoPeriodoVigencia(contrato, unitOfWork);
                }
                else
                {
                    valorKmUtilizado = Servicos.Embarcador.Carga.ContratoFrete.ObterKmUtilizadoContratoFreteNoPeriodoAtual(contrato, unitOfWork);
                    valorPago = Servicos.Embarcador.Carga.ContratoFrete.ObterValorPagoContratoFreteNoPeriodoAtual(contrato, unitOfWork);
                }

                var retorno = new
                {
                    Codigo = duplicar ? 0 : contrato.Codigo,
                    CodigoOriginario = duplicar ? contrato.Codigo : contrato.ContratoOriginario?.Codigo ?? 0,
                    Numero = duplicar ? "" : contrato.Numero,
                    contrato.Ativo,
                    contrato.Situacao,
                    OcorrenciaEmAberto = repositorioContratoFreteTransportador.OcorrenciaEmAberto(contrato.Codigo),
                    contrato.Descricao,
                    DataFinal = (duplicar && dataInicial.HasValue) ? string.Empty : contrato.DataFinal.ToString("dd/MM/yyyy"),
                    DataInicial = (duplicar && dataInicial.HasValue) ? dataInicial.Value.ToString("dd/MM/yyyy") : contrato.DataInicial.ToString("dd/MM/yyyy"),
                    contrato.Observacao,
                    contrato.TambemUtilizarContratoParaFiliaisDoTransportador,
                    OutrosValoresValorKmExcedente = contrato.OutrosValoresValorKmExcedente.ToString("n2"),
                    contrato.DescontarValoresOutrasCargas,
                    contrato.ExigeTabelaFreteComValor,
                    DiariaVeiculo = contrato.ValorDiariaPorVeiculo.ToString("n2"),
                    QuinzenaVeiculo = contrato.ValorQuinzenaPorVeiculo.ToString("n2"),
                    contrato.QuantidadeMotoristas,
                    DiariaMotorista = contrato.ValorDiariaPorMotorista.ToString("n2"),
                    QuinzenaMotorista = contrato.ValorQuinzenaPorMotorista.ToString("n2"),
                    contrato.NaoEmitirComplementoFechamentoFrete,
                    TipoFechamento = contrato.PeriodoAcordo,
                    contrato.TipoFranquia,
                    contrato.TipoEmissaoComplemento,
                    ValorPorMotorista = contrato.ValorPorMotorista.ToString("n2"),
                    ValorMensal = contrato.ValorMensal.ToString("n2"),
                    contrato.QuantidadeMensalCargas,
                    contrato.DeduzirValorPorCarga,
                    contrato.UtilizarValorFixoModeloVeicular,
                    contrato.TipoDisponibilidadeContratoFrete,
                    contrato.EstruturaTabela,
                    ComponenteFreteValorContrato = new { Codigo = contrato.ComponenteFreteValorContrato?.Codigo ?? 0, Descricao = contrato.ComponenteFreteValorContrato?.Descricao ?? "" },
                    TotalPorCavalo = contrato.FranquiaTotalPorCavalo,
                    UtilizarComponenteFreteValorContrato = contrato.ComponenteFreteValorContrato != null ? true : false,
                    ValorKmExcedente = contrato.FranquiaValorKmExcedente.ToString("n2"),
                    ValorKmUtilizado = valorKmUtilizado.ToString("n0"),
                    ValorPago = valorPago.ToString("n2"),
                    Resumo = ResumoAutorizacao(contrato, unitOfWork),
                    Transportador = new
                    {
                        Codigo = contrato.Transportador?.Codigo ?? 0,
                        Descricao = contrato.Transportador?.Descricao ?? ""
                    },
                    TipoContratoFrete = new
                    {
                        Codigo = contrato.TipoContratoFrete?.Codigo ?? 0,
                        Descricao = contrato.TipoContratoFrete?.Descricao ?? "",
                        TipoAditivo = contrato.TipoContratoFrete?.TipoAditivo ?? false,
                    },
                    ClienteTomador = contrato.ClienteTomador == null ? null : new
                    {
                        contrato.ClienteTomador.Codigo,
                        contrato.ClienteTomador.Descricao
                    },
                    ModeloDocumentoFiscal = new
                    {
                        Codigo = contrato.ModeloDocumentoFiscal?.Codigo ?? 0,
                        Descricao = contrato.ModeloDocumentoFiscal?.Descricao ?? ""
                    },
                    TiposOcorrencia = (
                        from obj in contrato.TiposOcorrencia
                        select new OcorrenciaContrato
                        {
                            Codigo = obj.Codigo,
                            CodigoProceda = obj.CodigoProceda,
                            Descricao = obj.Descricao
                        }
                    ).ToList(),
                    Clientes = (
                        from obj in contrato.Clientes
                        select new ClientesContrato
                        {
                            Codigo = obj.Cliente.CPF_CNPJ,
                            CPF_CNPJ = obj.Cliente.CPF_CNPJ_Formatado,
                            Localidade = obj.Cliente.Localidade.DescricaoCidadeEstado,
                            Nome = obj.Cliente.Nome
                        }
                    ).ToList(),
                    ValoresVeiculos = (
                        from obj in contrato.ModelosVeiculares
                        select new
                        {
                            Codigo = obj.ModeloVeicular.Codigo,
                            ModeloVeicular = obj.ModeloVeicular.Descricao,
                            ValorDiaria = obj.ValorDiaria.ToString("n2"),
                            ValorQuinzena = obj.ValorQuinzena.ToString("n2"),
                        }
                    ).ToList(),
                    Veiculos = (
                        from obj in contrato.Veiculos
                        select new
                        {
                            Codigo = obj.Veiculo.Codigo,
                            CodigoModeloVeicular = obj.Veiculo.ModeloVeicularCarga?.Codigo ?? 0,
                            ModeloVeicularCarga = obj.Veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty,
                            Placa = obj.Veiculo.Placa,
                            TipoPagamentoContratoFrete = obj.TipoPagamentoContratoFrete,
                            DescricaoTipoPagamentoContratoFrete = obj.DescricaoTipoPagamentoContratoFrete,
                            CapacidadeKG = obj.Veiculo.CapacidadeKG,
                            CapacidadeM3 = obj.Veiculo.CapacidadeM3,
                        }
                    ).ToList(),
                    Filiais = (
                        from obj in contrato.Filiais
                        select new FiliaisContrato
                        {
                            Codigo = obj.Filial.Codigo,
                            Descricao = obj.Filial.Descricao,
                        }
                    ).ToList(),
                    Acordos = (
                        from obj in contrato.Acordos
                        select new
                        {
                            Codigo = obj.Codigo,
                            FranquiaPorKm = obj.FranquiaPorKm,
                            ModeloVeicular = new { obj.ModeloVeicular.Codigo, obj.ModeloVeicular.Descricao },
                            ModeloCalculoFranquia = new { Codigo = obj.ModeloVeicular.ModeloCalculoFranquia?.Codigo ?? 0, Descricao = obj.ModeloVeicular.ModeloCalculoFranquia?.Descricao ?? string.Empty },
                            Periodo = obj.Periodo,
                            Rotulo = obj.Rotulo,
                            RotuloDescricao = obj.Rotulo.ToString("n2"),
                            Quantidade = obj.Quantidade.ToString(),
                            Total = obj.Total.ToString("n2"),
                            ValorAcordado = obj.ValorAcordado.ToString("n2"),
                        }
                    ).ToList(),
                    ValoresOutrosRecursos = (
                    from obj in contrato.ValoresOutrosRecursos
                    select new
                    {
                        Codigo = obj.Codigo,
                        TipoMaoDeObra = obj.TipoMaoDeObra,
                        PrecoAtual = obj.PrecoAtual.ToString("n2")
                    }
                    ).ToList(),
                    TipoOperacao = (
                        from obj in contrato.TipoOperacoes
                        select new
                        {
                            Codigo = obj.TipoOperacao.Codigo,
                            Descricao = obj.TipoOperacao.Descricao,
                        }
                    ).ToList(),
                    TipoCarga = (
                        from obj in contrato.TipoCargas
                        select new
                        {
                            Codigo = obj.TipoDeCarga.Codigo,
                            Descricao = obj.TipoDeCarga.Descricao,
                        }
                    ).ToList(),
                    CanalEntrega = (
                        from obj in contrato.CanaisEntrega
                        select new CanalEntregaContrato
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Descricao
                        }
                    ).ToList(),
                    Anexos = (
                        from obj in anexos
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.NomeArquivo,
                        }
                    ).ToList(),
                    TabelasFrete = ObterTabelaFrete(contrato),
                    FaixasKmFranquia = (
                        from faixaKmFranquia in contrato.FaixasKmFranquia
                        select new
                        {
                            faixaKmFranquia.Codigo,
                            faixaKmFranquia.Descricao,
                            QuilometragemInicial = faixaKmFranquia.QuilometragemInicial.ToString("n0"),
                            QuilometragemFinal = faixaKmFranquia.QuilometragemFinal.ToString("n0"),
                            ValorPorQuilometro = faixaKmFranquia.ValorPorQuilometro.ToString("n2")
                        }
                    ).ToList(),
                    ValorFreteMinimo = ObterValorFreteMinimo(contrato, unitOfWork),
                    ContratoTransporteFrete = new { Codigo = contrato.ContratoTransporteFrete?.Codigo ?? 0, Descricao = contrato.ContratoTransporteFrete?.Descricao ?? string.Empty },

                    PercentualRota = contrato.PercentualRota,
                    QuantidadeEntregas = contrato.QuantidadeEntregas,
                    CapacidadeOTM = contrato.CapacidadeOTM,
                    DominioOTM = contrato.DominioOTM,
                    PontoPlanejamentoTransporte = contrato.PontoPlanejamentoTransporte,
                    TipoIntegracao = contrato.TipoIntegracao,
                    IDExterno = contrato.IDExterno,
                    StatusAceiteContrato = new { Codigo = contrato.StatusAceiteContrato?.Codigo ?? 0, Descricao = contrato.StatusAceiteContrato?.Descricao ?? string.Empty },
                    GrupoCarga = contrato.GrupoCarga,
                    GerenciarCapacidade = contrato.GerenciarCapacidade,
                    BloqueioEdicao = BloquearEdicao(contrato),
                    PermiteEdicaoIndependenteDaSituacao = existeConfiguracao?.PermiteAlterarDadosContratoIndependenteSituacao ?? false,
                    EncerrarVigenciaContratoOriginario = (duplicar && dataInicial.HasValue)
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoContratoOriginario = Request.GetIntParam("CodigoOriginario");
                bool encerrarContratoOriginario = Request.GetBoolParam("EncerrarVigenciaContratoOriginario");
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = new Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador();

                contrato.ContratoOriginario = (codigoContratoOriginario > 0) ? repositorioContratoFreteTransportador.BuscarPorCodigo(codigoContratoOriginario) : null;

                if (contrato.ContratoOriginario != null)
                {
                    bool permitirDuplicar = PermitirDuplicarContratoComDuplicacao(unitOfWork) || !repositorioContratoFreteTransportador.ExisteDuplicacaoPorCodigo(contrato.ContratoOriginario.Codigo);

                    if (!permitirDuplicar)
                        throw new ControllerException("Não é possível duplicar um contrato que já foi duplicado");
                }

                PreencheEntidade(contrato, unitOfWork, encerrarContratoOriginario);

                repositorioContratoFreteTransportador.Inserir(contrato, Auditado);

                contrato.Filiais = new List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFilial>();

                SalvarOcorrencias(contrato, unitOfWork);
                SalvarCanalEntrega(contrato, unitOfWork);
                SalvarClientes(contrato, unitOfWork);
                SalvarModelosVeiculares(contrato, unitOfWork);
                SalvarVeiculos(contrato, unitOfWork);
                SalvarFiliais(contrato, unitOfWork);
                SalvarAcordos(contrato, unitOfWork);
                SalvarValoresOutrosRecursos(contrato, unitOfWork);
                SalvarTipoOperacao(contrato, unitOfWork);
                SalvarTipoCargas(contrato, unitOfWork);
                SalvarTabelaFrete(contrato, unitOfWork);
                AdicionarOuAtualizarFaixasKmFranquia(contrato, unitOfWork);
                AdicionarOuAtualizarValoresFreteMinimo(contrato, unitOfWork);

                ValidaEntidade(contrato, unitOfWork);

                if (!ValidaBags(contrato, out string bag, out string erroBags, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(new { Bag = bag, Msg = erroBags });
                }

                EtapaAprovacao(contrato, unitOfWork);
                AtualizarDadosPorContratoAprovado(contrato, unitOfWork);

                if (encerrarContratoOriginario)
                {
                    var contratoOriginario = contrato.ContratoOriginario;
                    DateTime dataFinalAux = contratoOriginario.DataFinal;
                    contratoOriginario.DataFinal = contrato.DataInicial;
                    repositorioContratoFreteTransportador.Atualizar(contratoOriginario);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoOriginario, null, $"Data final do contrato alterada de {dataFinalAux.ToString("g")} para {contratoOriginario.DataFinal.ToString("g")} ao duplicar o contrato", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);
                }

                repositorioContratoFreteTransportador.Atualizar(contrato);

                unitOfWork.CommitChanges();

                return new JsonpResult(contrato.Codigo);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = repContratoFreteTransportador.BuscarPorCodigo(codigo, true);

                // Valida
                if (contrato == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                if (BloquearEdicao(contrato))
                    throw new ControllerException("O status do aceite do contrato não permite a alteração.");

                // Preenche entidade com dados
                DateTime vigenciaInicial = contrato.DataInicial;
                DateTime vigenciaFinal = contrato.DataFinal;

                PreencheEntidade(contrato, unitOfWork);
                SalvarCanalEntrega(contrato, unitOfWork);

                ValidaEntidade(contrato, unitOfWork);
                SalvarOcorrencias(contrato, unitOfWork);
                SalvarTabelaFrete(contrato, unitOfWork);

                Dominio.Entidades.Auditoria.HistoricoObjeto historioPai = repContratoFreteTransportador.Atualizar(contrato, Auditado);

                SalvarClientes(contrato, unitOfWork, Auditado, historioPai);
                SalvarModelosVeiculares(contrato, unitOfWork, Auditado, historioPai);
                SalvarVeiculos(contrato, unitOfWork, Auditado, historioPai);
                SalvarFiliais(contrato, unitOfWork, Auditado, historioPai);
                SalvarAcordos(contrato, unitOfWork, Auditado, historioPai);
                SalvarValoresOutrosRecursos(contrato, unitOfWork, Auditado, historioPai);
                SalvarTipoOperacao(contrato, unitOfWork, Auditado, historioPai);
                SalvarTipoCargas(contrato, unitOfWork, Auditado, historioPai);
                AdicionarOuAtualizarFaixasKmFranquia(contrato, unitOfWork);
                AdicionarOuAtualizarValoresFreteMinimo(contrato, unitOfWork);

                // Valida bags
                if (!ValidaBags(contrato, out string bag, out string erroBags, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(new { Bag = bag, Msg = erroBags });
                }

                if (PermitirSolicitarAprovacao(contrato, unitOfWork))
                    EtapaAprovacao(contrato, unitOfWork);

                AtualizarDadosPorContratoAprovado(contrato, unitOfWork);

                repContratoFreteTransportador.Atualizar(contrato);

                unitOfWork.CommitChanges();

                //if (!EnviarEmailAlteracoes(contrato, historioPai, unitOfWork, out string msg))
                //    Servicos.Log.TratarErro(msg, "EMAILALTERACOESCONTRATOFRETE");

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.Frete.ContratoFreteTransportador repContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
                Repositorio.Embarcador.Frete.ContratoFreteTransportadorAnexo repContratoFreteTransportadorAnexo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorAnexo(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato = repContratoFreteTransportador.BuscarPorCodigo(codigo);

                // Valida
                if (contrato == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo> anexos = repContratoFreteTransportadorAnexo.BuscarPorContrato(contrato.Codigo);

                // Persiste dados
                for (var i = 0; i < anexos.Count(); i++) repContratoFreteTransportadorAnexo.Deletar(anexos[i]);
                repContratoFreteTransportador.Deletar(contrato);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacaoClientes()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacao();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ImportarClientes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

                // Configucarção de importacao
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacao();

                // Lista integrada em cada linha
                List<Dictionary<string, dynamic>> dadosLinhas = new List<Dictionary<string, dynamic>>();

                // Entidade para importacao
                List<Dominio.Entidades.Cliente> clientes = new List<Dominio.Entidades.Cliente>();

                // Chama serviço de importação
                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.ImportarInformacoes(Request, configuracoes, ref clientes, ref dadosLinhas, out string erro, ((dicionario) =>
                {
                    Dominio.Entidades.Cliente cliente = null;

                    if (!dicionario.TryGetValue("CNPJ", out dynamic strCnpj)) strCnpj = "0";
                    strCnpj = Utilidades.String.OnlyNumbers(strCnpj);

                    double.TryParse((string)strCnpj, out double cnpj);
                    if (cnpj > 0)
                        cliente = repCliente.BuscarPorCPFCNPJ(cnpj);

                    return cliente;
                }));

                if (!string.IsNullOrWhiteSpace(erro))
                    return new JsonpResult(false, true, erro);
                else if (retorno == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");

                retorno.Importados = (from obj in clientes where obj.Codigo > 0 select obj.CPF_CNPJ).Count();
                retorno.Retorno = (from obj in clientes
                                   where obj.Codigo > 0
                                   select new
                                   {
                                       Codigo = obj.CPF_CNPJ,
                                       CPF_CNPJ = obj.CPF_CNPJ_Formatado,
                                       Localidade = obj.Localidade.DescricaoCidadeEstado,
                                       Nome = obj.Nome
                                   }).ToList();

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao importar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaContratoFreteTransportadorCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteTransportadorCliente filtrosPesquisa = ObterFiltrosPesquisaContratoFreteTransportadorCliente();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nome", "Nome", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CNPJ/CPF", "CPF_CNPJ", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Endereço", "Endereco", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Descricao", false);

                Repositorio.Embarcador.Frete.ContratoFreteTransportadorCliente repContratoFreteTransportadorCliente = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorCliente(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarOuAgrupar);
                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente> tomadorCliente = repContratoFreteTransportadorCliente.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repContratoFreteTransportadorCliente.ContarConsulta(filtrosPesquisa));

                var lista = (from p in tomadorCliente
                             select new
                             {
                                 p.Cliente.Codigo,
                                 p.Cliente.Nome,
                                 CPF_CNPJ = p.Cliente.CPF_CNPJ_Formatado,
                                 p.Cliente.Endereco,
                                 p.Cliente.Descricao
                             }).ToList();

                grid.AdicionaRows(lista);
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
        public async Task<IActionResult> PesquisaContratoFreteValoresOutrosRecursos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteValoresOutrosRecursos filtrosPesquisa = ObterFiltrosPesquisaContratoFreteValoresOutrosRecursos();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo Mão de Obra", "TipoMaoDeObra", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Preço Atual", "PrecoAtual", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descricao", false);

                Repositorio.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos repContratoFreteValoresOutrosRecursos = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos> valoresOutrosRecursos = repContratoFreteValoresOutrosRecursos.Consultar(filtrosPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repContratoFreteValoresOutrosRecursos.ContarConsulta(filtrosPesquisa));

                var lista = (from p in valoresOutrosRecursos
                             select new
                             {
                                 p.Codigo,
                                 p.PrecoAtual,
                                 p.TipoMaoDeObra,
                                 Descricao = p.TipoMaoDeObra,
                             }).ToList();

                grid.AdicionaRows(lista);
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
        public async Task<IActionResult> BuscarAcordosPorContrato()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoContrato = Request.GetIntParam("Contrato");
                Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo repositorioAcordo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo> acordos = repositorioAcordo.BuscarPorContrato(codigoContrato);

                dynamic acordosRetornar = acordos.Select(o => new
                {
                    CodigoModeloVeicular = o.ModeloVeicular.Codigo,
                    Valor = o.ValorAcordado.ToString("n2")
                });

                return new JsonpResult(acordosRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os acordos do contrato.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private bool PermitirSolicitarAprovacao(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork)
        {
            if (!new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LBC))
                return true;

            List<Dominio.ObjetosDeValor.Auditoria.HistoricoPropriedade> alteracoes = contrato.GetCurrentChanges();
            List<string> propriedadesAlteradasDesconsiderar = new List<string>()
            {
                "DataAlteracao",
                "PercentualRota",
                "CapacidadeOTM",
                "TipoIntegracao"
            };

            return alteracoes.Any(o => !propriedadesAlteradasDesconsiderar.Contains(o.Propriedade));
        }

        private bool BloquearEdicao(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato)
        {
            if (contrato.StatusAceiteContrato == null)
                return false;

            return (contrato.StatusAceiteContrato.CodigoIntegracao == "D") || (contrato.StatusAceiteContrato.CodigoIntegracao == "I") || (contrato.StatusAceiteContrato.CodigoIntegracao == "P") || (contrato.StatusAceiteContrato.CodigoIntegracao == "R");
        }

        private bool PermitirDuplicarContratoComDuplicacao(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repostitorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            return !repostitorioTipoIntegracao.ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LBC);
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "CNPJ", Propriedade = "CNPJ", Tamanho = 150, CampoInformacao = true, Obrigatorio = true, Regras = new List<string> { "required" } }
            };

            return configuracoes;
        }

        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("PossuiFranquia", false);
            grid.AdicionarCabecalho("PeriodoAcordo", false);
            grid.AdicionarCabecalho("EnumPeriodoAcordo", false);
            grid.AdicionarCabecalho("TipoFranquia", false);
            grid.AdicionarCabecalho("TipoEmissaoComplemento", false);
            grid.AdicionarCabecalho("ValorPorMotorista", false);
            grid.AdicionarCabecalho("PermitirDuplicar", false);
            grid.AdicionarCabecalho("PermitirDuplicarComNovaVigencia", false);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.ContratoFreteTransportador.Numero, "Numero", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.ContratoFreteTransportador.Descricao, "Descricao", 20, Models.Grid.Align.left, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho(Localization.Resources.Consultas.ContratoFreteTransportador.EmpresaFilial, "Transportador", 20, Models.Grid.Align.left, true);
            else
                grid.AdicionarCabecalho(Localization.Resources.Consultas.ContratoFreteTransportador.Transportador, "Transportador", 20, Models.Grid.Align.left, true);

            grid.AdicionarCabecalho(Localization.Resources.Consultas.ContratoFreteTransportador.TipoContrato, "TipoContratoFrete", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.ContratoFreteTransportador.DataInicial, "DataInicial", 12, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.ContratoFreteTransportador.DataFinal, "DataFinal", 12, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Status", "DescricaoStatus", 8, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Consultas.ContratoFreteTransportador.Situacao, "DescricaoSituacao", 8, Models.Grid.Align.center, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                grid.AdicionarCabecalho("Integração", "DescricaoSituacaoIntegracao", 8, Models.Grid.Align.left, false);

            grid.AdicionarCabecalho(Localization.Resources.Consultas.ContratoFreteTransportador.StatusAceite, "StatusAceiteContrato", 8, Models.Grid.Align.left, false);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteTransportador filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContrato = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador> contratos = new List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador>();
            List<int> codigosContratosComDuplicacao = new List<int>();
            bool permitirDuplicarContratoComDuplicacao = PermitirDuplicarContratoComDuplicacao(unitOfWork);

            totalRegistros = repositorioContrato.ContarConsulta(filtrosPesquisa);

            if (totalRegistros > 0)
            {
                contratos = repositorioContrato.Consultar(filtrosPesquisa, propOrdenar, dirOrdena, inicio, limite);
                codigosContratosComDuplicacao = repositorioContrato.BuscarCodigosContratosComDuplicacaoPorCodigos(contratos.Select(contrato => contrato.Codigo).ToList());
            }

            var contratosRetornar = (
                from contrato in contratos
                select new
                {
                    contrato.Codigo,
                    PeriodoAcordo = contrato.DescricaoPeriodoAcordo,
                    EnumPeriodoAcordo = contrato.PeriodoAcordo,
                    TipoFranquia = contrato.DescricaoTipoFranquia,
                    contrato.TipoEmissaoComplemento,
                    ValorPorMotorista = contrato.ValorPorMotorista.ToString("n2"),
                    PossuiFranquia = contrato.FranquiaContratoMensal > 0,
                    contrato.Numero,
                    TipoContratoFrete = (contrato.TipoContratoFrete?.Descricao ?? "") + ((contrato.TipoContratoFrete?.TipoAditivo ?? false) ? " - " + contrato.NumeroAditivo : ""),
                    contrato.Descricao,
                    Transportador = contrato.Transportador?.Descricao ?? "",
                    DataInicial = contrato.DataInicial.ToString("dd/MM/yyyy"),
                    DataFinal = contrato.DataFinal.ToString("dd/MM/yyyy"),
                    StatusAceiteContrato = contrato.StatusAceiteContrato?.Descricao ?? "",
                    DescricaoStatus = contrato.DescricaoAtivo,
                    contrato.DescricaoSituacao,
                    contrato.DescricaoSituacaoIntegracao,
                    PermitirDuplicar = permitirDuplicarContratoComDuplicacao || !codigosContratosComDuplicacao.Contains(contrato.Codigo),
                    PermitirDuplicarComNovaVigencia = (permitirDuplicarContratoComDuplicacao || !codigosContratosComDuplicacao.Contains(contrato.Codigo)) && contrato.StatusAceiteContrato != null
                }
            ).ToList();

            return contratosRetornar.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork, bool encerrarContratoOriginario = false)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repositorioContratoFreteTransportador = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
            Repositorio.Embarcador.Frete.TipoContratoFrete repositorioTipoContratoFrete = new Repositorio.Embarcador.Frete.TipoContratoFrete(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Frete.ComponenteFrete repositorioComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
            Repositorio.ModeloDocumentoFiscal repositorioModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoTransporteFrete repContratoTransporteFrete = new Repositorio.Embarcador.Frete.ContratoTransporteFrete(unitOfWork);

            int codigoComponenteFreteValorContrato = Request.GetIntParam("ComponenteFreteValorContrato");
            int codigoModeloDocumentoFiscal = Request.GetIntParam("ModeloDocumentoFiscal");
            int codigoTipoContratoFrete = Request.GetIntParam("TipoContratoFrete");
            int codigoTransportador = Request.GetIntParam("Transportador");
            double cpfCnpjTomador = Request.GetDoubleParam("ClienteTomador");
            int codigoContratoTransportadorFrete = Request.GetIntParam("ContratoTransporteFrete");

            contrato.Ativo = Request.GetBoolParam("Ativo");
            contrato.Descricao = Request.GetStringParam("Descricao");
            contrato.DataFinal = Request.GetDateTimeParam("DataFinal");
            contrato.DataInicial = Request.GetDateTimeParam("DataInicial");
            contrato.DeduzirValorPorCarga = Request.GetBoolParam("DeduzirValorPorCarga");
            contrato.UtilizarValorFixoModeloVeicular = Request.GetBoolParam("UtilizarValorFixoModeloVeicular");
            contrato.Observacao = Request.GetStringParam("Observacao");
            contrato.TambemUtilizarContratoParaFiliaisDoTransportador = Request.GetBoolParam("TambemUtilizarContratoParaFiliaisDoTransportador");
            contrato.QuantidadeMensalCargas = Request.GetIntParam("QuantidadeMensalCargas");
            contrato.DescontarValoresOutrasCargas = Request.GetBoolParam("DescontarValoresOutrasCargas");
            contrato.ExigeTabelaFreteComValor = Request.GetBoolParam("ExigeTabelaFreteComValor");
            contrato.ValorDiariaPorVeiculo = Request.GetDecimalParam("DiariaVeiculo");
            contrato.ValorQuinzenaPorVeiculo = Request.GetDecimalParam("QuinzenaVeiculo");
            contrato.QuantidadeMotoristas = Request.GetIntParam("QuantidadeMotoristas");
            contrato.ValorDiariaPorMotorista = Request.GetDecimalParam("DiariaMotorista");
            contrato.ValorQuinzenaPorMotorista = Request.GetDecimalParam("QuinzenaMotorista");
            contrato.NaoEmitirComplementoFechamentoFrete = Request.GetBoolParam("NaoEmitirComplementoFechamentoFrete");
            contrato.TipoFranquia = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador>("TipoFranquia");
            contrato.TipoEmissaoComplemento = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoComplementoContratoFreteTransportador>("TipoEmissaoComplemento");
            contrato.ValorPorMotorista = Request.GetDecimalParam("ValorPorMotorista");
            contrato.ValorMensal = Request.GetDecimalParam("ValorMensal");
            contrato.ValorFreteMinimo = Request.GetDecimalParam("ValorFreteMinimo");
            contrato.OutrosValoresValorKmExcedente = Request.GetDecimalParam("OutrosValoresValorKmExcedente");
            contrato.FranquiaTotalPorCavalo = Request.GetIntParam("TotalPorCavalo");
            contrato.FranquiaTotalKM = Request.GetIntParam("TotalKm");
            contrato.FranquiaContratoMensal = Request.GetDecimalParam("ContratoMensal");
            contrato.FranquiaValorKmExcedente = Request.GetDecimalParam("ValorKmExcedente");
            contrato.FranquiaValorKM = (contrato.FranquiaTotalKM > 0) ? Math.Round((contrato.FranquiaContratoMensal / contrato.FranquiaTotalKM), 6, MidpointRounding.AwayFromZero) : 0m;
            contrato.ClienteTomador = (cpfCnpjTomador > 0) ? repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjTomador) : null;
            contrato.ComponenteFreteValorContrato = (codigoComponenteFreteValorContrato > 0) ? repositorioComponenteFrete.BuscarPorCodigo(codigoComponenteFreteValorContrato) : null;
            contrato.ModeloDocumentoFiscal = (codigoModeloDocumentoFiscal > 0) ? repositorioModeloDocumentoFiscal.BuscarPorId(codigoModeloDocumentoFiscal) : null;
            contrato.TipoContratoFrete = (codigoTipoContratoFrete > 0) ? repositorioTipoContratoFrete.BuscarPorCodigo(codigoTipoContratoFrete) : null;
            contrato.Transportador = (codigoTransportador > 0) ? repositorioEmpresa.BuscarPorCodigo(codigoTransportador) : null;
            contrato.TipoDisponibilidadeContratoFrete = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDisponibilidadeContratoFrete>("TipoDisponibilidadeContratoFrete");
            contrato.EstruturaTabela = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.EstruturaTabela>("EstruturaTabela");
            contrato.CustoAcessorio = Request.GetStringParam("CustoAcessorio");
            contrato.TipoCusto = Request.GetStringParam("TipoCusto");
            contrato.ContratoTransporteFrete = codigoContratoTransportadorFrete > 0 ? repContratoTransporteFrete.BuscarPorCodigo(codigoContratoTransportadorFrete) : null;

            contrato.PercentualRota = Request.GetDecimalParam("PercentualRota");
            contrato.QuantidadeEntregas = Request.GetIntParam("QuantidadeEntregas");
            contrato.CapacidadeOTM = Request.GetBoolParam("CapacidadeOTM");
            contrato.DominioOTM = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DominioOTM>("DominioOTM");
            contrato.PontoPlanejamentoTransporte = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PontoPlanejamentoTransporte>("PontoPlanejamentoTransporte");
            contrato.TipoIntegracao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoUnilever>("TipoIntegracao");
            contrato.GrupoCarga = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGrupoCarga>("GrupoCarga");
            contrato.GerenciarCapacidade = Request.GetBoolParam("GerenciarCapacidade");

            if (contrato.ContratoTransporteFrete != null && contrato.Transportador != null && contrato.Transportador.Codigo != contrato.ContratoTransporteFrete.Transportador.Codigo)
                throw new ControllerException("O transportador do Contrato Transportador deve ser o mesmo selecionado no campo Transportador");

            if (contrato.ContratoTransporteFrete != null && contrato.DataInicial < contrato.ContratoTransporteFrete.DataInicio)
                throw new ControllerException("A Data Inicial não pode ser menor do que a data inicial do Contrato Transportador");

            if (contrato.ContratoTransporteFrete != null && contrato.DataFinal > contrato.ContratoTransporteFrete.DataFim && encerrarContratoOriginario)
                throw new ControllerException("A Data Final não pode ser maior do que a data final do Contrato Transportador");

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador? periodoAcordoContratoFreteTransportador = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador>("TipoFechamento");

            if (!periodoAcordoContratoFreteTransportador.HasValue || periodoAcordoContratoFreteTransportador == null)
                throw new ControllerException("Não foi escolhida uma opção de Tipo de Fechamento no Acordo");

            contrato.PeriodoAcordo = periodoAcordoContratoFreteTransportador.Value;

            if (contrato.Codigo == 0)
            {
                contrato.Usuario = this.Usuario;
                contrato.NumeroAditivo = repositorioContratoFreteTransportador.BuscarProximoAditivo(codigoTransportador, codigoTipoContratoFrete);
                contrato.NumeroSequencial = repositorioContratoFreteTransportador.BuscarProximoNumero();
                contrato.NumeroEmbarcador = Request.GetStringParam("Numero");

                if (string.IsNullOrWhiteSpace(contrato.NumeroEmbarcador))
                    contrato.NumeroEmbarcador = contrato.NumeroSequencial.ToString();
            }
            else
                contrato.DataAlteracao = DateTime.Now;
        }

        private void SalvarFiliais(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai = null)
        {
            // Esse método busca todos os filiaiss que não estão na lista passada por request
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorFilial repContratoFilial = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorFilial(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            List<FiliaisContrato> filiaisContrato = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FiliaisContrato>>(Request.Params("Filiais"));
            if (filiaisContrato == null) return;

            List<int> codigosFiliais = (from o in filiaisContrato select o.Codigo).Distinct().ToList();

            List<int> codigosFiliaisExcluir = repContratoFilial.BuscarFiliaisNaoPesentesNaLista(contrato.Codigo, codigosFiliais);

            foreach (int codigoFilial in codigosFiliais)
            {
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFilial contratoFilial = repContratoFilial.BuscarPorContratoEFilial(contrato.Codigo, codigoFilial);

                if (contratoFilial != null)
                    continue;

                contratoFilial = new Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFilial()
                {
                    Filial = repFilial.BuscarPorCodigo(codigoFilial),
                    ContratoFrete = contrato
                };

                if (contratoFilial.Filial != null)
                    repContratoFilial.Inserir(contratoFilial, auditado, historicoPai);
            }

            foreach (int codigoFilial in codigosFiliaisExcluir)
            {
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFilial contratoCliente = repContratoFilial.BuscarPorContratoEFilial(contrato.Codigo, codigoFilial);

                if (contratoCliente != null)
                    repContratoFilial.Deletar(contratoCliente, auditado, historicoPai);
            }
        }

        private void SalvarAcordos(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai = null)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo repContratoAcordo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            List<dynamic> acordosContrato = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("Acordos"));

            if (acordosContrato == null || acordosContrato.Count == 0) return;

            int periodoMaximoPermitido;

            // O Periodo é definido de 0 a 3, onde 0 é mensal, 1 é quinzenal, 2 é decendial e 3 é semanal.
            switch (contrato.PeriodoAcordo)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador.Mensal:
                    periodoMaximoPermitido = 0;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador.Quinzenal:
                    periodoMaximoPermitido = 1;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador.Decendial:
                    periodoMaximoPermitido = 2;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.PeriodoAcordoContratoFreteTransportador.Semanal:
                    periodoMaximoPermitido = 3;
                    break;
                default:
                    periodoMaximoPermitido = 0;
                    break;
            }

            acordosContrato = acordosContrato.Where(acordo => (int)acordo.Periodo <= periodoMaximoPermitido).ToList();

            List<int> codigosAcordos = new List<int>();

            foreach (dynamic codigo in acordosContrato)
            {
                int.TryParse((string)codigo.Codigo, out int intcodigo);
                codigosAcordos.Add(intcodigo);
            }

            codigosAcordos = codigosAcordos.Where(o => o > 0).Distinct().ToList();

            List<int> codigosAcordosExcluir = repContratoAcordo.BuscarAcordosNaoPesentesNaLista(contrato.Codigo, codigosAcordos);

            foreach (dynamic acordo in acordosContrato)
            {
                int.TryParse((string)acordo.Codigo, out int codigo);
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo contratoAcordo = repContratoAcordo.BuscarPorContratoEAcordo(contrato.Codigo, codigo);

                if (contratoAcordo == null)
                    contratoAcordo = new Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo();
                else
                    contratoAcordo.Initialize();

                decimal valorAcordado = Utilidades.Decimal.Converter((string)acordo.ValorAcordado);
                decimal rotulo = Utilidades.Decimal.Converter((string)acordo.Rotulo);
                decimal total = Utilidades.Decimal.Converter((string)acordo.Total);

                contratoAcordo.ContratoFrete = contrato;
                contratoAcordo.ModeloVeicular = repModeloVeicularCarga.BuscarPorCodigo((int)acordo.ModeloVeicular.Codigo);
                contratoAcordo.FranquiaPorKm = (bool)acordo.FranquiaPorKm;
                contratoAcordo.Periodo = (int)acordo.Periodo;
                contratoAcordo.Quantidade = (int)acordo.Quantidade;
                contratoAcordo.Rotulo = rotulo;
                contratoAcordo.ValorAcordado = valorAcordado;
                contratoAcordo.Total = total;

                if (contratoAcordo.ModeloVeicular != null)
                {
                    if (contratoAcordo.Codigo == 0)
                        repContratoAcordo.Inserir(contratoAcordo, auditado, historicoPai);
                    else
                        repContratoAcordo.Atualizar(contratoAcordo, auditado, historicoPai);
                }
            }

            foreach (int codigoAcordo in codigosAcordosExcluir)
            {
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo contratoCliente = repContratoAcordo.BuscarPorContratoEAcordo(contrato.Codigo, codigoAcordo);
                if (contratoCliente != null) repContratoAcordo.Deletar(contratoCliente, auditado, historicoPai);
            }

            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAcordo> contratosAcordo = repContratoAcordo.BuscarPorContrato(contrato.Codigo);

            contrato.ValorMensal = contratosAcordo.Sum(o => o.ValorAcordado * o.Quantidade);
        }

        private void SalvarValoresOutrosRecursos(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai = null)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos repContratoValoresOutrosRecursos = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos> valoresOutrosRecursosList = repContratoValoresOutrosRecursos.BuscarPorContrato(contrato.Codigo);
            List<dynamic> valoresOutrosRecursosAdicionarOuAtualizar = Request.GetListParam<dynamic>("ValoresOutrosRecursos");

            if (valoresOutrosRecursosList.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (dynamic valorOutroRecurso in valoresOutrosRecursosAdicionarOuAtualizar)
                {
                    int? codigo = ((string)valorOutroRecurso.Codigo).ToNullableInt();
                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos> valoresOutrosRecursoRemover = (from valorOutroRecurso in valoresOutrosRecursosList where !listaCodigosAtualizados.Contains(valorOutroRecurso.Codigo) select valorOutroRecurso).ToList();

                foreach (Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos valorOutroRecurso in valoresOutrosRecursoRemover)
                    repContratoValoresOutrosRecursos.Deletar(valorOutroRecurso);
            }

            foreach (dynamic valorOutroRecurso in valoresOutrosRecursosAdicionarOuAtualizar)
            {
                int? codigo = ((string)valorOutroRecurso.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos valoresOutrosRecursosSalvar;

                if (codigo.HasValue)
                    valoresOutrosRecursosSalvar = repContratoValoresOutrosRecursos.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException("Não encontrou nenhum registro!");
                else
                    valoresOutrosRecursosSalvar = new Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValoresOutrosRecursos()
                    {
                        ContratoFrete = contrato
                    };

                valoresOutrosRecursosSalvar.PrecoAtual = ((string)valorOutroRecurso.PrecoAtual).ToDecimal();
                valoresOutrosRecursosSalvar.TipoMaoDeObra = (string)valorOutroRecurso.TipoMaoDeObra;

                if (valoresOutrosRecursosSalvar.Codigo > 0)
                    repContratoValoresOutrosRecursos.Atualizar(valoresOutrosRecursosSalvar);
                else
                    repContratoValoresOutrosRecursos.Inserir(valoresOutrosRecursosSalvar);
            }
        }

        private void SalvarTipoOperacao(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai = null)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorTipoOperacao repContratoTipoOperacao = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorTipoOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            List<dynamic> tipoOperacoesFranquia = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("TipoOperacao"));
            if (tipoOperacoesFranquia == null) return;

            List<int> codigosOperacoes = new List<int>();
            foreach (dynamic codigo in tipoOperacoesFranquia)
            {
                int.TryParse((string)codigo.Codigo, out int intcodigo);
                codigosOperacoes.Add(intcodigo);
            }
            codigosOperacoes = codigosOperacoes.Where(o => o > 0).Distinct().ToList();

            List<int> codigosTipoOperacoesExcluir = repContratoTipoOperacao.BuscarTipoOperacoesNaoPesentesNaLista(contrato.Codigo, codigosOperacoes);

            foreach (dynamic tipoOperacao in tipoOperacoesFranquia)
            {
                int.TryParse((string)tipoOperacao.Codigo, out int codigo);
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoOperacao contratoTipoOperacao = repContratoTipoOperacao.BuscarPorContratoETipoOperacao(contrato.Codigo, codigo);

                if (contratoTipoOperacao == null)
                    contratoTipoOperacao = new Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoOperacao();
                else
                    contratoTipoOperacao.Initialize();

                contratoTipoOperacao.ContratoFrete = contrato;
                contratoTipoOperacao.TipoOperacao = repTipoOperacao.BuscarPorCodigo((int)tipoOperacao.Codigo);

                if (contratoTipoOperacao.TipoOperacao != null)
                {
                    if (contratoTipoOperacao.Codigo == 0)
                        repContratoTipoOperacao.Inserir(contratoTipoOperacao, auditado, historicoPai);
                    else
                        repContratoTipoOperacao.Atualizar(contratoTipoOperacao, auditado, historicoPai);
                }
            }

            foreach (int codigo in codigosTipoOperacoesExcluir)
            {
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoOperacao entidade = repContratoTipoOperacao.BuscarPorContratoETipoOperacao(contrato.Codigo, codigo);
                if (entidade != null) repContratoTipoOperacao.Deletar(entidade, auditado, historicoPai);
            }
        }

        private void SalvarTipoCargas(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai = null)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorTipoCarga repContratoTipoCarga = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorTipoCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao reTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracaoLBC = reTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LBC);

            List<dynamic> tipoCargasFranquia = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("TipoCarga"));

            if ((integracaoLBC?.Ativo ?? false) && tipoCargasFranquia.Count() <= 0)
                throw new ControllerException("Não é permitdo salvar tipos de carga vazio!");

            if (tipoCargasFranquia == null) return;

            List<int> codigosCargas = new List<int>();
            foreach (dynamic codigo in tipoCargasFranquia)
            {
                int.TryParse((string)codigo.Codigo, out int intcodigo);
                codigosCargas.Add(intcodigo);
            }
            codigosCargas = codigosCargas.Where(o => o > 0).Distinct().ToList();

            List<int> codigosTipoCargasExcluir = repContratoTipoCarga.BuscarTipoCargasNaoPesentesNaLista(contrato.Codigo, codigosCargas);

            foreach (dynamic tipoCarga in tipoCargasFranquia)
            {
                int.TryParse((string)tipoCarga.Codigo, out int codigo);
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoCarga contratoTipoCarga = repContratoTipoCarga.BuscarPorContratoETipoCarga(contrato.Codigo, codigo);

                if (contratoTipoCarga == null)
                    contratoTipoCarga = new Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoCarga();
                else
                    contratoTipoCarga.Initialize();

                contratoTipoCarga.ContratoFrete = contrato;
                contratoTipoCarga.TipoDeCarga = repTipoCarga.BuscarPorCodigo((int)tipoCarga.Codigo);

                if (contratoTipoCarga.TipoDeCarga != null)
                {
                    if (contratoTipoCarga.Codigo == 0)
                        repContratoTipoCarga.Inserir(contratoTipoCarga, auditado, historicoPai);
                    else
                        repContratoTipoCarga.Atualizar(contratoTipoCarga, auditado, historicoPai);
                }
            }

            foreach (int codigo in codigosTipoCargasExcluir)
            {
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoCarga entidade = repContratoTipoCarga.BuscarPorContratoETipoCarga(contrato.Codigo, codigo);
                if (entidade != null) repContratoTipoCarga.Deletar(entidade, auditado, historicoPai);
            }
        }

        private void SalvarClientes(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai = null)
        {
            // Esse método busca todos os clientes que não estão na lista passada por request
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorCliente repContratoCliente = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorCliente(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            List<ClientesContrato> clientesContrato = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ClientesContrato>>(Request.Params("Clientes"));
            if (clientesContrato == null) return;

            // CNPJs dos clientes selecionados do json
            List<double> CNPJsClientes = (from o in clientesContrato select o.Codigo).Distinct().ToList();

            // Remove os registros de clientes desse contrato que não estão na relação do JSON
            List<double> CNPJsClientesExcluir = repContratoCliente.BuscarCNPJsNaoPesentesNaLista(contrato.Codigo, CNPJsClientes);

            foreach (double CNPJCliente in CNPJsClientes)
            {
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente contratoCliente = repContratoCliente.BuscarPorContratoECliente(contrato.Codigo, CNPJCliente);

                if (contratoCliente != null)
                    continue;

                contratoCliente = new Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente()
                {
                    Cliente = repCliente.BuscarPorCPFCNPJ(CNPJCliente),
                    ContratoFrete = contrato
                };

                if (contratoCliente.Cliente != null)
                    repContratoCliente.Inserir(contratoCliente, auditado, historicoPai);
            }

            foreach (double CNPJCliente in CNPJsClientesExcluir)
            {
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorCliente contratoCliente = repContratoCliente.BuscarPorContratoECliente(contrato.Codigo, CNPJCliente);

                if (contratoCliente != null)
                    repContratoCliente.Deletar(contratoCliente, auditado, historicoPai);
            }
        }

        private void SalvarModelosVeiculares(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai = null)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular repContratoModeloVeicular = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            List<ModeloVeicularContrato> modelosVeicularesContrato = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ModeloVeicularContrato>>(Request.Params("ValoresVeiculos"));
            if (modelosVeicularesContrato == null) return;

            List<int> codigosModelos = (from o in modelosVeicularesContrato select o.Codigo).Distinct().ToList();
            List<int> codigoModelosExcluir = repContratoModeloVeicular.BuscarCodigosNaoPesentesNaLista(contrato.Codigo, codigosModelos);

            foreach (ModeloVeicularContrato modeloContrato in modelosVeicularesContrato)
            {
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular modelo = repContratoModeloVeicular.BuscarPorContratoEModelo(contrato.Codigo, modeloContrato.Codigo);

                decimal.TryParse(modeloContrato.ValorDiaria, out decimal valorDiaria);
                decimal.TryParse(modeloContrato.ValorQuinzena, out decimal valorQuinzena);

                if (modelo != null)
                {
                    modelo.Initialize();

                    modelo.ValorDiaria = valorDiaria;
                    modelo.ValorQuinzena = valorQuinzena;

                    repContratoModeloVeicular.Atualizar(modelo, auditado, historicoPai);
                }
                else
                {
                    modelo = new Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular()
                    {
                        ModeloVeicular = repModeloVeicularCarga.BuscarPorCodigo(modeloContrato.Codigo),
                        ValorDiaria = valorDiaria,
                        ValorQuinzena = valorQuinzena,
                        ContratoFrete = contrato
                    };

                    if (modelo.ModeloVeicular != null)
                        repContratoModeloVeicular.Inserir(modelo, auditado, historicoPai);
                }
            }

            foreach (int codigoModelo in codigoModelosExcluir)
            {
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular modelo = repContratoModeloVeicular.BuscarPorContratoEModelo(contrato.Codigo, codigoModelo);

                if (modelo != null)
                    repContratoModeloVeicular.Deletar(modelo, auditado, historicoPai);
            }
        }

        private void SalvarVeiculos(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, Dominio.Entidades.Auditoria.HistoricoObjeto historicoPai = null)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorVeiculo repositorioContratoVeiculo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorVeiculo(unitOfWork);

            if (contrato.TipoDisponibilidadeContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDisponibilidadeContratoFrete.TodosVeiculos)
            {
                repositorioContratoVeiculo.DeletarPorContrato(contrato.Codigo);
                return;
            }

            List<VeiculoContrato> veiculosContrato = Newtonsoft.Json.JsonConvert.DeserializeObject<List<VeiculoContrato>>(Request.Params("Veiculos"));

            if (veiculosContrato == null)
                return;

            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            List<int> codigosVeiculos = (from o in veiculosContrato select o.Codigo).Distinct().ToList();
            List<int> codigosVeiculosExcluir = repositorioContratoVeiculo.BuscarCodigosNaoPresentesNaLista(contrato.Codigo, codigosVeiculos);

            foreach (VeiculoContrato veiculoContrato in veiculosContrato)
            {
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo veiculo = repositorioContratoVeiculo.BuscarPorContratoEVeiculo(contrato.Codigo, veiculoContrato.Codigo);

                if (veiculo != null)
                    veiculo.TipoPagamentoContratoFrete = veiculoContrato.TipoPagamentoContratoFrete;
                else
                {
                    veiculo = new Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo()
                    {
                        Veiculo = repositorioVeiculo.BuscarPorCodigo(veiculoContrato.Codigo),
                        TipoPagamentoContratoFrete = veiculoContrato.TipoPagamentoContratoFrete,
                        ContratoFrete = contrato
                    };

                    if (veiculo.Veiculo != null)
                        repositorioContratoVeiculo.Inserir(veiculo, auditado, historicoPai);
                }
            }

            foreach (int codigoVeiculo in codigosVeiculosExcluir)
            {
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo veiculo = repositorioContratoVeiculo.BuscarPorContratoEVeiculo(contrato.Codigo, codigoVeiculo);

                if (veiculo != null)
                    repositorioContratoVeiculo.Deletar(veiculo, auditado, historicoPai);
            }
        }

        private void SalvarOcorrencias(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

            List<OcorrenciaContrato> tipoOcorrenciaContrato = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OcorrenciaContrato>>(Request.Params("TiposOcorrencia"));
            if (tipoOcorrenciaContrato == null) return;
            if (contrato.TiposOcorrencia == null) contrato.TiposOcorrencia = new List<Dominio.Entidades.TipoDeOcorrenciaDeCTe>();

            List<int> codigosTipoOcorrencia = (from o in tipoOcorrenciaContrato select o.Codigo).Distinct().ToList();

            List<int> codigosTipoOcorrenciaExcluir = (from o in contrato.TiposOcorrencia where !codigosTipoOcorrencia.Contains(o.Codigo) select o.Codigo).ToList();

            foreach (int codigoTipoOcorrencia in codigosTipoOcorrencia)
            {
                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repTipoOcorrencia.BuscarPorCodigo(codigoTipoOcorrencia);

                // Insere
                if (tipoOcorrencia != null && !contrato.TiposOcorrencia.Contains(tipoOcorrencia))
                {
                    contrato.TiposOcorrencia.Add(tipoOcorrencia);
                }
            }

            foreach (int codigoTipoOcorrencia in codigosTipoOcorrenciaExcluir)
            {
                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repTipoOcorrencia.BuscarPorCodigo(codigoTipoOcorrencia);

                // Deleta
                if (tipoOcorrencia != null)
                    contrato.TiposOcorrencia.Remove(tipoOcorrencia);
            }
        }

        private void SalvarCanalEntrega(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.CanalEntrega repositorioCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao reTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracaoLBC = reTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LBC);

            List<CanalEntregaContrato> tipoCanalEntrega = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CanalEntregaContrato>>(Request.Params("CanalEntrega"));

            if ((integracaoLBC?.Ativo ?? false) && tipoCanalEntrega.Count() <= 0)
                throw new ControllerException("Não é permitdo salvar canal entregas vazio!");
            if (tipoCanalEntrega == null) return;
            if (contrato.CanaisEntrega == null) contrato.CanaisEntrega = new List<Dominio.Entidades.Embarcador.Pedidos.CanalEntrega>();

            List<int> codigosCanaisEntrega = (from o in tipoCanalEntrega select o.Codigo).Distinct().ToList();

            List<int> codigosCanalEntregaExcluir = (from o in contrato.CanaisEntrega where !codigosCanaisEntrega.Contains(o.Codigo) select o.Codigo).ToList();

            foreach (int codigoCanalEntrega in codigosCanaisEntrega)
            {
                Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega = repositorioCanalEntrega.BuscarPorCodigo(codigoCanalEntrega);

                // Insere
                if (canalEntrega != null && !contrato.CanaisEntrega.Contains(canalEntrega))
                {
                    contrato.CanaisEntrega.Add(canalEntrega);
                }
            }

            foreach (int codigoCanalEntrega in codigosCanalEntregaExcluir)
            {
                Dominio.Entidades.Embarcador.Pedidos.CanalEntrega canalEntrega = repositorioCanalEntrega.BuscarPorCodigo(codigoCanalEntrega);

                // Deleta
                if (canalEntrega != null)
                    contrato.CanaisEntrega.Remove(canalEntrega);
            }
        }

        private void SalvarTabelaFrete(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao reTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracaoLBC = reTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LBC);

            dynamic tabelasFrete = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("TabelasFrete"));

            if ((integracaoLBC?.Ativo ?? false) && tabelasFrete.Count <= 0)
                throw new ControllerException("Não é permitido salvar tabelas frete vazio!");

            if (tabelasFrete == null) return;
            if (contrato.TabelasFrete == null) contrato.TabelasFrete = new List<Dominio.Entidades.Embarcador.Frete.TabelaFrete>();

            if (contrato.TabelasFrete.Count > 0)
            {
                List<int> codigosCanaisEntrega = new List<int>();
                foreach (var item in tabelasFrete)
                    codigosCanaisEntrega.Add((int)item.Codigo);

                List<Dominio.Entidades.Embarcador.Frete.TabelaFrete> tabelasRemover = contrato.TabelasFrete.Where(x => !codigosCanaisEntrega.Contains(x.Codigo)).ToList();

                foreach (var tabelaFrete in tabelasRemover)
                    contrato.TabelasFrete.Remove(tabelaFrete);
            }

            foreach (dynamic tabela in tabelasFrete)
            {
                Dominio.Entidades.Embarcador.Frete.TabelaFrete existeTabelaFrete = repositorioTabelaFrete.BuscarPorCodigo((int)tabela.Codigo);

                if (existeTabelaFrete == null)
                    continue;

                contrato.TabelasFrete.Add(existeTabelaFrete);
            }
        }

        private void EtapaAprovacao(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Frete.RegraContratoFreteTransportador> regras = Servicos.Embarcador.Frete.ContratoFreteTransportador.VerificarRegrasAutorizacao(contrato, unitOfWork);

            if (regras.Count > 0)
            {
                bool aguardandoAprovacao = Servicos.Embarcador.Frete.ContratoFreteTransportador.CriarRegrasAutorizacao(regras, contrato, this.Usuario, TipoServicoMultisoftware, _conexao.StringConexao, unitOfWork);

                contrato.Situacao = aguardandoAprovacao ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.AgAprovacao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.Aprovado;
            }
            else
                contrato.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador.SemRegra;
        }

        private void AtualizarDadosPorContratoAprovado(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Frete.ContratoFreteTransportador.ContratoAprovado(contrato, Auditado, unitOfWork);
            new Servicos.Embarcador.Frete.ContratoTransporteFrete(unitOfWork).GerarRegistroIntegracaoContratoFreteCustoFixo(contrato);
        }

        private bool ValidaBags(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, out string bag, out string msgErro, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular repContratoModeloVeicular = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorVeiculo repContratoVeiculo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorVeiculo(unitOfWork);
            msgErro = "";
            bag = "";

            // Jeferson pediu pra tirar na tarefa 49951. Caso necessário falar com Leonardo
            /* Valida tipos de ocorrencia 
            foreach (Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia in contrato.TiposOcorrencia)
            {
                if (tipoOcorrencia.OrigemOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorContrato)
                {
                    bag = "Ocorrência";
                    msgErro = "Só é possível selecionar tipo de ocorrências de origem Por Contrato.";
                    return false;
                }
            }*/

            // Valida modelos veículos
            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular> modelos = repContratoModeloVeicular.BuscarPorContrato(contrato.Codigo);
            foreach (Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular modelo in modelos)
            {
                if (modelo.ValorDiaria == 0 && modelo.ValorQuinzena == 0)
                {
                    bag = "Valores Veículos";
                    msgErro = "Nenhum valor informado para o modelo " + modelo.ModeloVeicular.Descricao + ".";
                    return false;
                }
            }

            // Valida modelos veículos
            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo> veiculos = repContratoVeiculo.BuscarPorContrato(contrato.Codigo);
            bool validarModelo = modelos.Count > 0;
            foreach (Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorVeiculo veiculo in veiculos)
            {
                if (validarModelo && veiculo.Veiculo.ModeloVeicularCarga == null)
                {
                    bag = "Veículos";
                    msgErro = "O veículo " + veiculo.Veiculo.Placa + " não possui modelo veicular.";
                    return false;
                }

                if (validarModelo && !modelos.Exists(m => m.ModeloVeicular.Codigo == veiculo.Veiculo.ModeloVeicularCarga.Codigo))
                {
                    bag = "Veículos";
                    msgErro = "O modelo veicular " + veiculo.Veiculo.ModeloVeicularCarga.Descricao + " não está cadastrado na aba Valores Veículos.";
                    return false;
                }

                if (validarModelo)
                {
                    Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular modelo = (from m in modelos where m.ModeloVeicular.Codigo == veiculo.Veiculo.ModeloVeicularCarga.Codigo select m).FirstOrDefault();

                    if (veiculo.TipoPagamentoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Frete.TipoPagamentoContratoFrete.Diaria && modelo.ValorDiaria == 0 && contrato.ValorDiariaPorVeiculo == 0)
                    {
                        bag = "Veículos";
                        msgErro = "O modelo veicular " + veiculo.Veiculo.ModeloVeicularCarga.Descricao + " não possui valor de diária.";
                        return false;
                    }

                    if (veiculo.TipoPagamentoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Frete.TipoPagamentoContratoFrete.Quinzena && modelo.ValorQuinzena == 0 && contrato.ValorQuinzenaPorVeiculo == 0)
                    {
                        bag = "Veículos";
                        msgErro = "O modelo veicular " + veiculo.Veiculo.ModeloVeicularCarga.Descricao + " não possui valor de quinzena.";
                        return false;
                    }
                    //((veiculo.TipoPagamentoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Frete.TipoPagamentoContratoFrete.Diaria && modelos.Any(m => m.ValorDiaria)) )
                }
            }

            return true;
        }

        private dynamic ObterTabelaFrete(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato)
        {
            return (from obj in contrato.TabelasFrete
                    select new
                    {
                        Codigo = obj.Codigo,
                        Descricao = obj.Descricao
                    }).ToList();
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteValoresOutrosRecursos ObterFiltrosPesquisaContratoFreteValoresOutrosRecursos()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteValoresOutrosRecursos()
            {
                CodigoContratoFreteTransportador = Request.GetIntParam("ContratoFreteTransportador"),
                TipoMaoDeObra = Request.GetStringParam("TipoMaoDeObra")
            };
        }

        private void ValidaEntidade(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportador repContrato = new Repositorio.Embarcador.Frete.ContratoFreteTransportador(unitOfWork);
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo repContratoModeloVeicular = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorAcordo(unitOfWork);
            List<int> modelosDoContrato = repContratoModeloVeicular.BuscarCodigosModeloVeicularPorContrato(contrato.Codigo);

            if (contrato.DataInicial == DateTime.MinValue || contrato.DataFinal == DateTime.MinValue)
                throw new ControllerException("Data Inicial e Data Final são obrigatórias.");

            if (contrato.DataInicial > contrato.DataFinal)
                throw new ControllerException("Data Inicial deve ser maior que a Data Final.");

            if (contrato.DataFinal < DateTime.Today && ConfiguracaoEmbarcador.IgnorarTipoContratoNoContratoFreteTransportador)
                throw new ControllerException("Não é possível informar um vigência com data menor que a data atual.");

            if (!string.IsNullOrWhiteSpace(contrato.NumeroEmbarcador) && contrato.NumeroEmbarcador != "0" && repContrato.ContarPorNumero(contrato.NumeroEmbarcador) > 1)
                throw new ControllerException($"Já existe um contrato de frete cadastrado com o número {contrato.NumeroEmbarcador}.");

            if ((contrato.Transportador == null) && (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                throw new ControllerException("Transportador é obrigatório.");

            if (contrato.QuantidadeMotoristas > 0 && contrato.ValorDiariaPorMotorista == 0 && contrato.ValorQuinzenaPorMotorista == 0)
                throw new ControllerException("Nenhum valor informado para motorista.");

            List<int> codigosTiposOcorrencia = (contrato.TiposOcorrencia != null) ? (from o in contrato.TiposOcorrencia.ToList() select o.Codigo).ToList() : new List<int>();
            List<int> codigosDesconsiderar = new List<int>() { contrato.Codigo };
            int tipoContratoFrete = ConfiguracaoEmbarcador.IgnorarTipoContratoNoContratoFreteTransportador ? 0 : contrato.TipoContratoFrete?.Codigo ?? 0;

            if ((contrato.ContratoOriginario != null) && !PermitirDuplicarContratoComDuplicacao(unitOfWork))
                codigosDesconsiderar.Add(contrato.ContratoOriginario.Codigo);

            Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoConflito = repContrato.VerificarContratoExistente(codigosDesconsiderar, contrato.Transportador?.Codigo ?? 0, tipoContratoFrete, contrato.DataInicial, contrato.DataFinal, codigosTiposOcorrencia, contrato.CanaisEntrega.Select(x => x.Codigo).ToList(), modelosDoContrato);

            if (contratoConflito != null)
                throw new ControllerException($"O contrato nº {contratoConflito.Numero} esta conflitando com o período informado.");
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Transportador")
                propOrdenar = "Transportador.RazaoSocial";
            if (propOrdenar == "DescricaoSituacao")
                propOrdenar = "Situacao";
            if (propOrdenar == "Numero")
                propOrdenar = "NumeroEmbarcador";
        }

        private dynamic ResumoAutorizacao(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador repAprovacaoAlcadaContratoFreteTransportador = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador(unitOfWork);

            int aprovacoesNecessarias = 0;
            var alcadas = repAprovacaoAlcadaContratoFreteTransportador.BuscarPorContrato(contrato.Codigo);
            var regras = (from o in alcadas where o.RegraContratoFreteTransportador != null select o.RegraContratoFreteTransportador).Distinct().ToList();
            foreach (var regra in regras)
            {
                aprovacoesNecessarias += (from o in alcadas where o.RegraContratoFreteTransportador != null && o.RegraContratoFreteTransportador.Codigo == regra.Codigo select o.NumeroAprovadores).FirstOrDefault();
            }

            int aprovacoes = repAprovacaoAlcadaContratoFreteTransportador.ContarAprovacoes(contrato.Codigo);
            int reprovacoes = repAprovacaoAlcadaContratoFreteTransportador.ContarReprovacoes(contrato.Codigo);
            return new
            {
                Solicitante = contrato.Usuario?.Nome ?? string.Empty,
                DataSolicitacao = contrato.DataAlteracao?.ToString("dd/MM/yyyy") ?? string.Empty,
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                Reprovacoes = reprovacoes,
                Situacao = contrato.DescricaoSituacao,
            };
        }

        private string CorAprovacao(Dominio.Entidades.Embarcador.Frete.AprovacaoAlcadaContratoFreteTransportador contrato)
        {

            if (contrato.Bloqueada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Grey;

            if (contrato.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;

            if (contrato.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;

            if (contrato.Delegada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Info;

            if (contrato.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Warning;


            return "";
        }

        private bool EnviarEmailAlteracoes(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Dominio.Entidades.Auditoria.HistoricoObjeto historioPai, Repositorio.UnitOfWork unitOfWork, out string msg)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table cellpadding=\"4\" border=\"1\" cellspacing=\"0\">");
            /**/
            sb.Append("<thead>");
            /**//**/
            sb.Append("<tr>");
            /**//**//**/
            sb.Append("<th>Campo</th>");
            /**//**//**/
            sb.Append("<th>De</th>");
            /**//**//**/
            sb.Append("<th>Para</th>");
            /**//**/
            sb.Append("</tr>");
            /**/
            sb.Append("</thead>");

            /**/
            sb.Append("<tbody>");
            foreach (var prop in historioPai.Propriedades)
            {
                if (this.NaoExibir(prop.Propriedade)) continue;

                sb.Append("<tr>");
                /**/
                sb.Append("<td>" + this.DescricaoPropriedade(prop.Propriedade) + "</td>");
                /**/
                sb.Append("<td>" + prop.De + "</td>");
                /**/
                sb.Append("<td>" + prop.Para + "</td>");
                sb.Append("</tr>");
            }
            /**/
            sb.Append("</tbody>");
            sb.Append("</table>");

            string para = "jeferson_santos@carrefour.com";
            string assunto = "Contrato de Frete nº " + contrato.Numero + " foi alterado";
            string corpo = "<p>O usuário " + historioPai.Usuario.Nome + " atualizou as informações do Contrato de Frete nº " + contrato.Numero + "</p>" + sb.ToString();

            return Servicos.Email.EnviarEmailAutenticado(para, assunto, corpo, unitOfWork, out msg, "");
        }

        private string DescricaoPropriedade(string prop)
        {
            if (prop == "Descricao") prop = "Descrição";
            else if (prop == "TipoContratoFrete") prop = "Tipo do Contrato de Frete";
            else if (prop == "DataInicial") prop = "Data Inicial";
            else if (prop == "DataFinal") prop = "Data Final";
            else if (prop == "Observacao") prop = "Observação";
            else if (prop == "Ativo") prop = "Ativo";
            else if (prop == "DescontarValoresOutrasCargas") prop = "Descontar Valores de Outras Cargas";
            else if (prop == "ValorDiariaPorVeiculo") prop = "Valor Diária Por Veículo";
            else if (prop == "ValorQuinzenaPorVeiculo") prop = "Valor Quinzena Por Veiculo";
            else if (prop == "ValorDiariaPorMotorista") prop = "Valor Diária Por Motorista";
            else if (prop == "ValorQuinzenaPorMotorista") prop = "Valor Quinzena Por Motorista";
            else if (prop == "ValorFreteMinimo") prop = "Valor de Frete Mínimo";
            else if (prop == "TotalOcorrenciaMotorista") prop = "Total Ocorrência Motorista";
            else if (prop == "QuantidadeMotoristas") prop = "Quantidade Motoristas";
            else if (prop == "ClienteTomador") prop = "Cliente Tomador";
            else if (prop == "OutrosValoresValorKmExcedente") prop = "Outros Valores Valor Km Excedente";
            else if (prop == "FranquiaTotalPorCavalo") prop = "Franquia Total Por Cavalo";
            else if (prop == "FranquiaTotalKM") prop = "Franquia Total KM";
            else if (prop == "FranquiaContratoMensal") prop = "Franquia Contrato Mensal";
            else if (prop == "FranquiaValorKM") prop = "Franquia Valor KM";
            else if (prop == "FranquiaValorKmExcedente") prop = "Franquia Valor Km Excedente";
            else if (prop == "PeriodoAcordo") prop = "Período Acordo";
            else if (prop == "TipoFranquia") prop = "Tipo da Franquia";
            else if (prop == "Situacao") prop = "Situação";
            else if (prop == "ContratoAutorizacoes") prop = "Contrato Autorizações";
            else if (prop == "TiposOcorrencia") prop = "Tipos Ocorrência";
            else if (prop == "Clientes") prop = "Clientes";
            else if (prop == "ModelosVeiculares") prop = "Modelos Veiculares";
            else if (prop == "Veiculos") prop = "Veículos";
            else if (prop == "Filiais") prop = "Filiais";
            else if (prop == "Acordos") prop = "Acordos";
            else if (prop == "TipoOperacoes") prop = "Tipo de Operações";
            else if (prop == "TipoCargas") prop = "Tipo de Cargas";
            else if (prop == "ContratoFreteTransportadorVeiculo") prop = "Veículo";
            else if (prop == "ContratoFreteTransportadorFilial") prop = "Filial";
            else if (prop == "ContratoFreteTransportadorAcordo") prop = "Acordo";

            return prop;
        }

        private bool NaoExibir(string prop)
        {
            List<string> camposOcultos = new List<string>()
            {
                "DataAlteracao"
            };

            return camposOcultos.Contains(prop);
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteTransportador ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteTransportador()
            {
                CodigoTransportador = Request.GetListParam<int>("Transportador"),
                Descricao = Request.GetStringParam("Descricao"),
                NumeroContrato = Request.GetStringParam("NumeroContrato"),
                Status = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa>("Ativo"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                Situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFreteTransportador>("Situacao"),
                Placa = Request.GetStringParam("Placa"),
                TipoContratoFrete = Request.GetListParam<int>("TipoContratoFrete"),
                CodigoStatusAceiteContrato = Request.GetIntParam("StatusAceite"),
                SituacaoIntegracao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("SituacaoIntegracao")
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteTransportadorCliente ObterFiltrosPesquisaContratoFreteTransportadorCliente()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoFreteTransportadorCliente()
            {
                CnpjCpf = Request.GetStringParam("CpfCnpj").ObterSomenteNumeros().ToDouble(),
                CodigoContratoFreteTransportador = Request.GetIntParam("ContratoFreteTransportador"),
                Nome = Request.GetStringParam("Nome"),
            };
        }

        private string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return "Cliente." + propriedadeOrdenarOuAgrupar;
        }

        #endregion Métodos Privados

        #region Métodos Privados - Faixa de KM

        private void AdicionarOuAtualizarFaixasKmFranquia(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic faixasKmFranquia = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaFaixaKmFranquia"));

            ExcluirFranquiaFaixasKmFranquiaRemovidas(contrato, faixasKmFranquia, unitOfWork);
            InserirFaixasKmFranquiaAdicionadas(contrato, faixasKmFranquia, unitOfWork);
        }

        private void ExcluirFranquiaFaixasKmFranquiaRemovidas(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, dynamic faixasKmFranquia, Repositorio.UnitOfWork unitOfWork)
        {
            if (contrato.FaixasKmFranquia?.Count > 0)
            {
                Repositorio.Embarcador.Frete.ContratoFreteTransportadorFaixaKmFranquia repositorioFaixaKmFranquia = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorFaixaKmFranquia(unitOfWork);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var faixaKmFranquia in faixasKmFranquia)
                {
                    int? codigo = ((string)faixaKmFranquia.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFaixaKmFranquia> listaFaixasKmFranquiaRemover = (from o in contrato.FaixasKmFranquia where !listaCodigosAtualizados.Contains(o.Codigo) select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFaixaKmFranquia faixaKmFranquia in listaFaixasKmFranquiaRemover)
                    repositorioFaixaKmFranquia.Deletar(faixaKmFranquia);

                if (listaFaixasKmFranquiaRemover.Count > 0)
                {
                    string descricaoAcao = listaFaixasKmFranquiaRemover.Count == 1 ? "Faixa de KM removida" : "Múltiplas faixas de KM removidas";

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, contrato, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void InserirFaixasKmFranquiaAdicionadas(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contrato, dynamic faixasKmFranquia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorFaixaKmFranquia repositorioFaixaKmFranquia = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorFaixaKmFranquia(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFaixaKmFranquia> faixasKmFranquiaCadastradasOuAtualizadas = new List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFaixaKmFranquia>();

            foreach (var faixaKmFranquia in faixasKmFranquia)
            {
                int? codigo = ((string)faixaKmFranquia.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFaixaKmFranquia faixaKmFranquiaSalvar;

                if (codigo.HasValue)
                    faixaKmFranquiaSalvar = repositorioFaixaKmFranquia.BuscarPorCodigo(codigo.Value, auditavel: false) ?? throw new ControllerException("Faixa de KM não encontrada");
                else
                    faixaKmFranquiaSalvar = new Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFaixaKmFranquia();

                faixaKmFranquiaSalvar.ContratoFrete = contrato;
                faixaKmFranquiaSalvar.QuilometragemInicial = ((string)faixaKmFranquia.QuilometragemInicial).ToInt();
                faixaKmFranquiaSalvar.QuilometragemFinal = ((string)faixaKmFranquia.QuilometragemFinal).ToInt();
                faixaKmFranquiaSalvar.ValorPorQuilometro = ((string)faixaKmFranquia.ValorPorQuilometro).ToDecimal();

                ValidarFaixaKmFranquiaDuplicada(faixasKmFranquiaCadastradasOuAtualizadas, faixaKmFranquiaSalvar);

                if (codigo.HasValue)
                    repositorioFaixaKmFranquia.Atualizar(faixaKmFranquiaSalvar);
                else
                    repositorioFaixaKmFranquia.Inserir(faixaKmFranquiaSalvar);

                faixasKmFranquiaCadastradasOuAtualizadas.Add(faixaKmFranquiaSalvar);
            }

            if (contrato.IsInitialized() && (faixasKmFranquiaCadastradasOuAtualizadas.Count() > 0))
            {
                string descricaoAcao = faixasKmFranquiaCadastradasOuAtualizadas.Count() == 1 ? "Faixa de KM adicionada ou atualizada" : "Múltiplas faixas de KM adicionadas ou atualizadas";

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contrato, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }

        private void ValidarFaixaKmFranquiaDuplicada(List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFaixaKmFranquia> faixasKmFranquiaCadastradasOuAtualizadas, Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFaixaKmFranquia faixaKmFranquiaSalvar)
        {
            Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFaixaKmFranquia faixaKmFranquiaDuplicada = (
                from faixaKmFranquia in faixasKmFranquiaCadastradasOuAtualizadas
                where (
                    (faixaKmFranquiaSalvar.QuilometragemInicial >= faixaKmFranquia.QuilometragemInicial && faixaKmFranquiaSalvar.QuilometragemInicial <= faixaKmFranquia.QuilometragemFinal) ||
                    (faixaKmFranquiaSalvar.QuilometragemFinal >= faixaKmFranquia.QuilometragemInicial && faixaKmFranquiaSalvar.QuilometragemFinal <= faixaKmFranquia.QuilometragemFinal) ||
                    (faixaKmFranquia.QuilometragemInicial >= faixaKmFranquiaSalvar.QuilometragemInicial && faixaKmFranquia.QuilometragemInicial <= faixaKmFranquiaSalvar.QuilometragemFinal) ||
                    (faixaKmFranquia.QuilometragemFinal >= faixaKmFranquiaSalvar.QuilometragemInicial && faixaKmFranquia.QuilometragemFinal <= faixaKmFranquiaSalvar.QuilometragemFinal)
                )
                select faixaKmFranquia
            ).FirstOrDefault();

            if (faixaKmFranquiaDuplicada != null)
                throw new ControllerException($"Já existe um cadastro que contém a faixa de KM {faixaKmFranquiaSalvar.Descricao.ToLower()}");
        }

        #endregion Métodos Privados - Faixa de KM

        #region Métodos Privados - Valor de Frete Mínimo

        private void AdicionarOuAtualizarValoresFreteMinimo(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo repositorioValorFreteMinimo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo(unitOfWork);
            List<dynamic> valoresFreteMinimo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(Request.Params("ListaValorFreteMinimo"));

            foreach (dynamic valorFreteMinimo in valoresFreteMinimo)
            {
                Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo contratoValorFreteMinimo;
                int? codigoValorFreteMinimo = ((string)valorFreteMinimo.Codigo).ToNullableInt();

                if (codigoValorFreteMinimo.HasValue)
                    contratoValorFreteMinimo = repositorioValorFreteMinimo.BuscarPorCodigo(codigoValorFreteMinimo.Value, auditavel: false) ?? throw new ControllerException("Valor de frete mínimo não encontrado");
                else
                    contratoValorFreteMinimo = new Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo();

                contratoValorFreteMinimo.ContratoFreteTransportador = contratoFreteTransportador;
                contratoValorFreteMinimo.ValorMinimo = ((string)valorFreteMinimo.ValorMinimo).ToDecimal();

                AtualizarClientesDestino(contratoValorFreteMinimo, valorFreteMinimo.ClientesDestino, unitOfWork);
                AtualizarClientesOrigem(contratoValorFreteMinimo, valorFreteMinimo.ClientesOrigem, unitOfWork);
                AtualizarEstadosDestino(contratoValorFreteMinimo, valorFreteMinimo.EstadosDestino, unitOfWork);
                AtualizarEstadosOrigem(contratoValorFreteMinimo, valorFreteMinimo.EstadosOrigem, unitOfWork);
                AtualizarLocalidadesDestino(contratoValorFreteMinimo, valorFreteMinimo.LocalidadesDestino, unitOfWork);
                AtualizarLocalidadesOrigem(contratoValorFreteMinimo, valorFreteMinimo.LocalidadesOrigem, unitOfWork);
                AtualizarModelosVeicularesCarga(contratoValorFreteMinimo, valorFreteMinimo.ModelosVeicularesCarga, unitOfWork);
                AtualizarTiposCarga(contratoValorFreteMinimo, valorFreteMinimo.TiposCarga, unitOfWork);

                if (contratoValorFreteMinimo.Codigo > 0)
                    repositorioValorFreteMinimo.Atualizar(contratoValorFreteMinimo);
                else
                    repositorioValorFreteMinimo.Inserir(contratoValorFreteMinimo);
            }
        }

        private void AtualizarClientesDestino(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo valorFreteMinimo, dynamic clientesDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            if (valorFreteMinimo.ClientesDestino == null)
                valorFreteMinimo.ClientesDestino = new List<Dominio.Entidades.Cliente>();
            else
                valorFreteMinimo.ClientesDestino.Clear();

            valorFreteMinimo.PossuiClientesDestino = false;

            foreach (var clienteDestino in clientesDestino)
            {
                Dominio.Entidades.Cliente clienteDestinoAdicionar = repositorioCliente.BuscarPorCPFCNPJ(((string)clienteDestino.Codigo).ToDouble()) ?? throw new ControllerException("Cliente não encontrado");

                valorFreteMinimo.ClientesDestino.Add(clienteDestinoAdicionar);
                valorFreteMinimo.PossuiClientesDestino = true;
            }
        }

        private void AtualizarClientesOrigem(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo valorFreteMinimo, dynamic clientesOrigem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            if (valorFreteMinimo.ClientesOrigem == null)
                valorFreteMinimo.ClientesOrigem = new List<Dominio.Entidades.Cliente>();
            else
                valorFreteMinimo.ClientesOrigem.Clear();

            valorFreteMinimo.PossuiClientesOrigem = false;

            foreach (var clienteOrigem in clientesOrigem)
            {
                Dominio.Entidades.Cliente clienteOrigemAdicionar = repositorioCliente.BuscarPorCPFCNPJ(((string)clienteOrigem.Codigo).ToDouble()) ?? throw new ControllerException("Cliente não encontrado");

                valorFreteMinimo.ClientesOrigem.Add(clienteOrigemAdicionar);
                valorFreteMinimo.PossuiClientesOrigem = true;
            }
        }

        private void AtualizarEstadosDestino(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo valorFreteMinimo, dynamic estadosDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Estado repositorioEstado = new Repositorio.Estado(unitOfWork);

            if (valorFreteMinimo.EstadosDestino == null)
                valorFreteMinimo.EstadosDestino = new List<Dominio.Entidades.Estado>();
            else
                valorFreteMinimo.EstadosDestino.Clear();

            valorFreteMinimo.PossuiEstadosDestino = false;

            foreach (var estadoDestino in estadosDestino)
            {
                Dominio.Entidades.Estado estadoDestinoAdicionar = repositorioEstado.BuscarPorSigla((string)estadoDestino.Codigo) ?? throw new ControllerException("Estado não encontrado");

                valorFreteMinimo.EstadosDestino.Add(estadoDestinoAdicionar);
                valorFreteMinimo.PossuiEstadosDestino = true;
            }
        }

        private void AtualizarEstadosOrigem(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo valorFreteMinimo, dynamic estadosOrigem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Estado repositorioEstado = new Repositorio.Estado(unitOfWork);

            if (valorFreteMinimo.EstadosOrigem == null)
                valorFreteMinimo.EstadosOrigem = new List<Dominio.Entidades.Estado>();
            else
                valorFreteMinimo.EstadosOrigem.Clear();

            valorFreteMinimo.PossuiEstadosOrigem = false;

            foreach (var estadoOrigem in estadosOrigem)
            {
                Dominio.Entidades.Estado estadoOrigemAdicionar = repositorioEstado.BuscarPorSigla((string)estadoOrigem.Codigo) ?? throw new ControllerException("Estado não encontrado");

                valorFreteMinimo.EstadosOrigem.Add(estadoOrigemAdicionar);
                valorFreteMinimo.PossuiEstadosOrigem = true;
            }
        }

        private void AtualizarLocalidadesDestino(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo valorFreteMinimo, dynamic localidadesDestino, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);

            if (valorFreteMinimo.LocalidadesDestino == null)
                valorFreteMinimo.LocalidadesDestino = new List<Dominio.Entidades.Localidade>();
            else
                valorFreteMinimo.LocalidadesDestino.Clear();

            valorFreteMinimo.PossuiLocalidadesDestino = false;

            foreach (var localidadeDestino in localidadesDestino)
            {
                Dominio.Entidades.Localidade localidadeDestinoAdicionar = repositorioLocalidade.BuscarPorCodigo(((string)localidadeDestino.Codigo).ToInt()) ?? throw new ControllerException("Cidade não encontrada");

                valorFreteMinimo.LocalidadesDestino.Add(localidadeDestinoAdicionar);
                valorFreteMinimo.PossuiLocalidadesDestino = true;
            }
        }

        private void AtualizarLocalidadesOrigem(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo valorFreteMinimo, dynamic localidadesOrigem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);

            if (valorFreteMinimo.LocalidadesOrigem == null)
                valorFreteMinimo.LocalidadesOrigem = new List<Dominio.Entidades.Localidade>();
            else
                valorFreteMinimo.LocalidadesOrigem.Clear();

            valorFreteMinimo.PossuiLocalidadesOrigem = false;

            foreach (var localidadeOrigem in localidadesOrigem)
            {
                Dominio.Entidades.Localidade localidadeOrigemAdicionar = repositorioLocalidade.BuscarPorCodigo(((string)localidadeOrigem.Codigo).ToInt()) ?? throw new ControllerException("Cidade não encontrada");

                valorFreteMinimo.LocalidadesOrigem.Add(localidadeOrigemAdicionar);
                valorFreteMinimo.PossuiLocalidadesOrigem = true;
            }
        }

        private void AtualizarModelosVeicularesCarga(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo valorFreteMinimo, dynamic modelosVeicularesCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            if (valorFreteMinimo.ModelosVeicularesCarga == null)
                valorFreteMinimo.ModelosVeicularesCarga = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();
            else
                valorFreteMinimo.ModelosVeicularesCarga.Clear();

            valorFreteMinimo.PossuiModelosVeicularesCarga = false;

            foreach (var modeloVeicularCarga in modelosVeicularesCarga)
            {
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCargaAdicionar = repositorioModeloVeicularCarga.BuscarPorCodigo(((string)modeloVeicularCarga.Codigo).ToInt()) ?? throw new ControllerException("Modelo veicular de carga não encontrado");

                valorFreteMinimo.ModelosVeicularesCarga.Add(modeloVeicularCargaAdicionar);
                valorFreteMinimo.PossuiModelosVeicularesCarga = true;
            }
        }

        private void AtualizarTiposCarga(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo valorFreteMinimo, dynamic tiposCargaOrigem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao reTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao integracaoLBC = reTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.LBC);

            if ((integracaoLBC?.Ativo ?? false) && tiposCargaOrigem.Count() <= 0)
                throw new ControllerException("Não é permitdo atualizar tipos de carga estando vazio!");

            if (valorFreteMinimo.TiposCarga == null)
                valorFreteMinimo.TiposCarga = new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();
            else
                valorFreteMinimo.TiposCarga.Clear();

            valorFreteMinimo.PossuiTiposCarga = false;

            foreach (var tipoCarga in tiposCargaOrigem)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCargaAdicionar = repositorioTipoCarga.BuscarPorCodigo(((string)tipoCarga.Codigo).ToInt()) ?? throw new ControllerException("Tipo de carga não encontrada");

                valorFreteMinimo.TiposCarga.Add(tipoCargaAdicionar);
                valorFreteMinimo.PossuiTiposCarga = true;
            }
        }

        private dynamic ObterValorFreteMinimo(Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportador contratoFreteTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo repositorioValorFreteMinimo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorValorFreteMinimo> valoresFreteMinimo = repositorioValorFreteMinimo.BuscarPorContratoFreteTransportador(contratoFreteTransportador.Codigo);

            return new
            {
                ValorFreteMinimo = (contratoFreteTransportador.ValorFreteMinimo > 0) ? contratoFreteTransportador.ValorFreteMinimo.ToString("n2") : "",
                ListaValorFreteMinimo = (
                    from valorFreteMinimo in valoresFreteMinimo
                    select new
                    {
                        valorFreteMinimo.Codigo,
                        ClientesDestino = (from o in valorFreteMinimo.ClientesDestino select new { Codigo = o.CPF_CNPJ, o.Descricao }).ToList(),
                        ClientesOrigem = (from o in valorFreteMinimo.ClientesOrigem select new { Codigo = o.CPF_CNPJ, o.Descricao }).ToList(),
                        EstadosDestino = (from o in valorFreteMinimo.EstadosDestino select new { Codigo = o.Sigla, o.Descricao }).ToList(),
                        EstadosOrigem = (from o in valorFreteMinimo.EstadosOrigem select new { Codigo = o.Sigla, o.Descricao }).ToList(),
                        LocalidadesDestino = (from o in valorFreteMinimo.LocalidadesDestino select new { o.Codigo, o.Descricao }).ToList(),
                        LocalidadesOrigem = (from o in valorFreteMinimo.LocalidadesOrigem select new { o.Codigo, o.Descricao }).ToList(),
                        ModelosVeicularesCarga = (from o in valorFreteMinimo.ModelosVeicularesCarga select new { o.Codigo, o.Descricao }).ToList(),
                        TiposCarga = (from o in valorFreteMinimo.TiposCarga select new { o.Codigo, o.Descricao }).ToList(),
                        ValorMinimo = valorFreteMinimo.ValorMinimo.ToString("n2")
                    }
                ).ToList()
            };
        }

        #endregion Métodos Privados - Valor de Frete Mínimo
    }
}

