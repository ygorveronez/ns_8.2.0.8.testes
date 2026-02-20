using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Servicos.Embarcador.Logistica;
using SGTAdmin.Controllers;
using System.Linq;
using System.Text;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/RotaFrete")]
    public class RotaFreteController : BaseController
    {
        #region Construtores

        public RotaFreteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRotaFrete filtrosPesquisa = ObterFiltrosPesquisa(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                {
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 16, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.RotaFrete.Remetente, "Origem", 16, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.RotaFrete.Destinatario, "Destino", 16, Models.Grid.Align.left, false);
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.RotaFrete.Distancia, "Quilometros", 6, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.RotaFrete.Tempo, "TempoDeViagemEmMinutos", 7, Models.Grid.Align.right, true);
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.RotaFrete.PadraoTempo, "PadraoTempo", 7, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("FilialDistribuidora", false);
                    grid.AdicionarCabecalho("Fronteiras", false);
                }
                else
                {
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 40, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.RotaFrete.FilialDistribuidora, "FilialDistribuidora", 20, Models.Grid.Align.left, true);
                    grid.AdicionarCabecalho("Origem", false);
                    grid.AdicionarCabecalho("Destino", false);
                    grid.AdicionarCabecalho("Quilometros", false);
                    grid.AdicionarCabecalho("TempoDeViagemEmMinutos", false);
                    grid.AdicionarCabecalho("PadraoTempo", false);
                    grid.AdicionarCabecalho("Fronteiras", false);
                }

                grid.AdicionarCabecalho(Localization.Resources.Consultas.RotaFrete.CodigoIntegracao, "CodigoIntegracao", 12, Models.Grid.Align.center, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.RotaFrete.GrupoDePessoa, "GrupoPessoa", 18, Models.Grid.Align.left, true);

                if (filtrosPesquisa.Ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 5, Models.Grid.Align.center, false);

                grid.AdicionarCabecalho(Localization.Resources.Consultas.RotaFrete.TipoOperacao, "TipoOperacao", 12, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "GrupoPessoa")
                    propOrdena = "GrupoPessoas.Descricao";
                else if (propOrdena == "Origem")
                    propOrdena = "Remetente.Nome";

                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unidadeDeTrabalho);
                List<Dominio.Entidades.RotaFrete> listaRotaFrete = repRotaFrete.Consultar(filtrosPesquisa, propOrdena, grid.dirOrdena, grid.inicio, grid.limite > 0 ? grid.limite : 1);
                grid.setarQuantidadeTotal(repRotaFrete.ContarConsulta(filtrosPesquisa));

                var lista = (from p in listaRotaFrete select RetornarObjetoPesquisa(p)).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Consultas.RotaFrete.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCEP()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.RotaFreteCEP repRotaFreteCEP = new Repositorio.Embarcador.Logistica.RotaFreteCEP(unidadeDeTrabalho);

                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "Codigo", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.RotaFrete.CEPInicial, "CEPInicial", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.RotaFrete.CEPFinal, "CEPFinal", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.RotaFrete.LeadTime, "LeadTime", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.RotaFrete.ADValorem, "PercentualADValorem", 40, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP> listaRotaFreteCEP = repRotaFreteCEP.Consultar(codigo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite > 0 ? grid.limite : 1);
                grid.setarQuantidadeTotal(repRotaFreteCEP.ContarConsulta(codigo));

                var lista = (from obj in listaRotaFreteCEP
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 CEPInicial = obj.CEPInicialFormatado,
                                 CEPFinal = obj.CEPFinalFormatado,
                                 LeadTime = obj.LeadTime,
                                 PercentualADValorem = obj.PercentualADValorem
                             }).ToList();

                grid.AdicionaRows(lista);

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

        public async Task<IActionResult> BuscarPracasPedagioPolilinha()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.Configuracoes.IntegracaoSemParar repIntegracaoSemParar = new Repositorio.Embarcador.Configuracoes.IntegracaoSemParar(unidadeDeTrabalho);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unidadeDeTrabalho);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = repIntegracaoSemParar.Buscar();

                string polilinha = Request.GetStringParam("polilinha");
                string pontosDaRota = Request.GetStringParam("pontosDaRota");
                int codigoRotaFrete = Request.GetIntParam("Codigo");

                bool apenasObterPracasPedagio = Request.GetBoolParam("ApenasObterPracasPedagio");

                if (string.IsNullOrEmpty(polilinha))
                    return new JsonpResult(false, Localization.Resources.Logistica.RotaFrete.AntesBuscarPracasPedagioNecessarioRoteirizar);

                TipoUltimoPontoRoteirizacao tipo = Request.GetEnumParam<TipoUltimoPontoRoteirizacao>("tipoUltimoPontoRoteirizacao", TipoUltimoPontoRoteirizacao.PontoMaisDistante);

                Dominio.Entidades.RotaFrete rotaFrete = repRotaFrete.BuscarPorCodigo(codigoRotaFrete);

                Servicos.Embarcador.Integracao.SemParar.PracasPedagio serPracasPedagio = new Servicos.Embarcador.Integracao.SemParar.PracasPedagio();
                Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = serPracasPedagio.Autenticar(unidadeDeTrabalho, TipoServicoMultisoftware);
                if (credencial.Autenticado)
                {
                    string erro = "";
                    string response = "";
                    string request = "";

                    List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagioIda = new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();
                    List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagioRetorno = new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();

                    if (apenasObterPracasPedagio || (!string.IsNullOrEmpty(polilinha) && configuracaoRoteirizacao.SempreUtilizarRotaParaBuscarPracasPedagio))
                        pracasPedagioIda = serPracasPedagio.ObterPracasPedagioPorPolilinha(credencial, polilinha, integracaoSemParar?.DistanciaMinimaQuadrante ?? 0, out erro, unidadeDeTrabalho, integracaoSemParar?.TipoConsultaRota ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota.MaisRapida, rotaFrete);
                    else
                    {
                        pracasPedagioIda = serPracasPedagio.ObterPracasPedagioIda(credencial, pontosDaRota, out erro, unidadeDeTrabalho, integracaoSemParar?.TipoConsultaRota ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota.MaisRapida, out request, out response, rotaFrete);

                        if (tipo != TipoUltimoPontoRoteirizacao.PontoMaisDistante)
                            pracasPedagioRetorno = serPracasPedagio.ObterPracasPedagioVolta(credencial, pontosDaRota, out erro, unidadeDeTrabalho, integracaoSemParar?.TipoConsultaRota ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota.MaisRapida, out request, out response, rotaFrete);
                    }

                    if (string.IsNullOrWhiteSpace(erro))
                    {
                        int ordem = 0;
                        dynamic dynPracasPedagio = new List<dynamic>();
                        foreach (Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaIda in pracasPedagioIda)
                        {
                            dynPracasPedagio.Add(ObterPracasPedagio(pracaIda, EixosSuspenso.Ida, ordem));
                            ordem++;
                        }

                        foreach (Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaRetorno in pracasPedagioRetorno)
                        {
                            dynPracasPedagio.Add(ObterPracasPedagio(pracaRetorno, EixosSuspenso.Volta, ordem));
                            ordem++;
                        }

                        return new JsonpResult(dynPracasPedagio);
                    }
                    else
                    {
                        return new JsonpResult(false, true, erro);
                    }
                }
                else
                {
                    return new JsonpResult(false, true, credencial.Retorno);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Logistica.RotaFrete.OcorreuFalhaConsultarPracasPedagio);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPracasPedagio()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Logistica.PracaPedagio repPracaPedagio = new Repositorio.Embarcador.Logistica.PracaPedagio(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.IntegracaoSemParar repIntegracaoSemParar = new Repositorio.Embarcador.Configuracoes.IntegracaoSemParar(unidadeDeTrabalho);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unidadeDeTrabalho);

                int codigoRotaFrete = Request.GetIntParam("Codigo");
                double cpfCnpjRemetente = Request.GetDoubleParam("Remetente");

                dynamic destinatarios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Destinatarios"));

                TipoUltimoPontoRoteirizacao tipo = Request.GetEnumParam<TipoUltimoPontoRoteirizacao>("tipoUltimoPontoRoteirizacao", TipoUltimoPontoRoteirizacao.PontoMaisDistante);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = repIntegracaoSemParar.BuscarPrimeira();
                Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(cpfCnpjRemetente);
                Dominio.Entidades.RotaFrete rotaFrete = repRotaFrete.BuscarPorCodigo(codigoRotaFrete);

                List<Dominio.Entidades.Cliente> clientes = new List<Dominio.Entidades.Cliente>();

                foreach (var destinatario in destinatarios)
                    clientes.Add(repCliente.BuscarPorCPFCNPJ((double)destinatario.Codigo));

                List<int> CodigosIBGE = new List<int>();
                CodigosIBGE.Add(remetente.Localidade.CodigoIBGE);
                CodigosIBGE.AddRange((from obj in clientes select obj.Localidade.CodigoIBGE).Distinct().ToList());

                Servicos.Embarcador.Integracao.SemParar.PracasPedagio serPracasPedagio = new Servicos.Embarcador.Integracao.SemParar.PracasPedagio();
                Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = serPracasPedagio.Autenticar(unidadeDeTrabalho, TipoServicoMultisoftware);
                if (credencial.Autenticado)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagio = new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();
                    List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagioRetorno = new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();

                    string erro = "";
                    string xmlRequest = "";
                    string xmlResponse = "";
                    List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio> pracasPedagiosSemParar = serPracasPedagio.ObterPracasPedagio(credencial, CodigosIBGE, out erro, out xmlRequest, out xmlResponse, unidadeDeTrabalho, integracaoSemParar?.TipoConsultaRota ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota.MaisRapida, rotaFrete);
                    if (string.IsNullOrWhiteSpace(erro))
                    {
                        pracasPedagio = serPracasPedagio.ValidarCadastrosPracasPedagioSemParar(pracasPedagiosSemParar, unidadeDeTrabalho);

                        //Se não for até o mais distante.. vamos ter que gerar a polilinha de retorno.. e ver quais são os pontos de retorno..
                        if (tipo != TipoUltimoPontoRoteirizacao.PontoMaisDistante)
                        {
                            int indiceUltimaEntrega = -1;
                            int indice = CodigosIBGE.Count - 1;
                            // Vamos localizar a última entrega... diferente da origem..
                            while (indice > 0)
                            {
                                if (CodigosIBGE[indice] != remetente.Localidade.CodigoIBGE)
                                {
                                    indiceUltimaEntrega = indice;
                                    break;
                                }
                                indice--;
                            }

                            if (indiceUltimaEntrega > 0)
                            {
                                List<int> CodigosIBGERetorno = new List<int>();
                                CodigosIBGERetorno.Add(CodigosIBGE[indiceUltimaEntrega]);
                                CodigosIBGERetorno.Add(remetente.Localidade.CodigoIBGE);

                                List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio> pracasPedagiosSemPararRetorno = serPracasPedagio.ObterPracasPedagio(credencial, CodigosIBGERetorno, out erro, out xmlRequest, out xmlResponse, unidadeDeTrabalho, integracaoSemParar?.TipoConsultaRota ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota.MaisRapida, rotaFrete);

                                pracasPedagioRetorno = serPracasPedagio.ValidarCadastrosPracasPedagioSemParar(pracasPedagiosSemPararRetorno, unidadeDeTrabalho);
                            }
                        }

                        dynamic dynPracasPedagio = new List<dynamic>();
                        for (int i = 0; i < pracasPedagio.Count; i++)
                        {
                            Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaPedagio = pracasPedagio[i];
                            EixosSuspenso sentido = EixosSuspenso.Ida;
                            if (pracasPedagioRetorno != null)
                                if (i >= pracasPedagio.Count - pracasPedagioRetorno?.Count && pracaPedagio.CodigoIntegracao == pracasPedagioRetorno[i - pracasPedagioRetorno.Count].CodigoIntegracao)
                                    sentido = EixosSuspenso.Volta;

                            dynamic item = new
                            {
                                pracaPedagio.CodigoIntegracao,
                                pracaPedagio.Codigo,
                                pracaPedagio.Descricao,
                                KM = pracaPedagio.KM.ToString("n2"),
                                pracaPedagio.Rodovia,
                                pracaPedagio.DescricaoAtivo,
                                pracaPedagio.Latitude,
                                pracaPedagio.Longitude,
                                Sentido = EixosSuspensoHelper.ObterDescricao(sentido),
                                Ordem = i
                            };

                            dynPracasPedagio.Add(item);
                        }

                        return new JsonpResult(dynPracasPedagio);
                    }
                    else
                    {
                        return new JsonpResult(false, true, erro);
                    }
                }
                else
                {
                    return new JsonpResult(false, true, credencial.Retorno);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Logistica.RotaFrete.OcorreuFalhaConsultarPracasPedagio);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string codigoIntegracao = Request.GetStringParam("CodigoIntegracao");
                string pontosDaRota = Request.GetStringParam("PontosDaRota");

                ValidarCodigoIntegracaoDuplicado(unidadeDeTrabalho, codigoIntegracao);

                unidadeDeTrabalho.Start();

                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unidadeDeTrabalho);
                Dominio.Entidades.RotaFrete rotaFrete = new Dominio.Entidades.RotaFrete();

                PreencherRotaFrete(rotaFrete, unidadeDeTrabalho);

                SetarLocalidadesOrigem(ref rotaFrete, unidadeDeTrabalho);
                //SetarPracasDePedagio(ref rotaFrete, unidadeDeTrabalho);
                SetarColetas(ref rotaFrete, unidadeDeTrabalho);
                SetarPontosPassagemPreDefinidos(ref rotaFrete, unidadeDeTrabalho);
                SetarEstadosDestino(ref rotaFrete, unidadeDeTrabalho);
                SetarEstadosOrigem(ref rotaFrete, unidadeDeTrabalho);
                SetarFiliais(ref rotaFrete, unidadeDeTrabalho);
                SetarTiposCarga(ref rotaFrete, unidadeDeTrabalho);

                repRotaFrete.Inserir(rotaFrete, Auditado);

                SetarLocalidades(ref rotaFrete, unidadeDeTrabalho);
                SetarDestinatarios(ref rotaFrete, unidadeDeTrabalho);
                SetarFronteiras(ref rotaFrete, unidadeDeTrabalho);
                SetarPracasDePedagio(ref rotaFrete, unidadeDeTrabalho);
                SetarVeiculos(ref rotaFrete, unidadeDeTrabalho);

                new Servicos.Embarcador.Carga.RotaFrete(unidadeDeTrabalho).SetarPontosPassagem(rotaFrete, pontosDaRota);
                SetarPostosFiscais(ref rotaFrete, unidadeDeTrabalho);

                AtualizarRestricoes(rotaFrete, unidadeDeTrabalho);
                AtualizarEmpresas(rotaFrete, unidadeDeTrabalho);
                AtualizarEmpresasExclusivas(rotaFrete, unidadeDeTrabalho);

                repRotaFrete.Atualizar(rotaFrete);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                string codigoIntegracao = Request.GetStringParam("CodigoIntegracao");
                string pontosDaRota = Request.GetStringParam("PontosDaRota");

                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unidadeDeTrabalho);
                Dominio.Entidades.RotaFrete rotaFrete = repRotaFrete.BuscarPorCodigo(codigo, true);

                if (rotaFrete == null)
                    return new JsonpResult(false, true, Localization.Resources.Logistica.RotaFrete.RotaNaoEncontrada);

                ValidarCodigoIntegracaoDuplicado(unidadeDeTrabalho, codigoIntegracao, codigo);

                SituacaoRoteirizacao situacaoRoteirizacaoInicial = rotaFrete.SituacaoDaRoteirizacao;

                unidadeDeTrabalho.Start();

                PreencherRotaFrete(rotaFrete, unidadeDeTrabalho);

                SetarLocalidadesOrigem(ref rotaFrete, unidadeDeTrabalho);
                SetarPracasDePedagio(ref rotaFrete, unidadeDeTrabalho);
                SetarEstadosDestino(ref rotaFrete, unidadeDeTrabalho);
                SetarEstadosOrigem(ref rotaFrete, unidadeDeTrabalho);
                SetarLocalidades(ref rotaFrete, unidadeDeTrabalho);
                SetarDestinatarios(ref rotaFrete, unidadeDeTrabalho);
                SetarFronteiras(ref rotaFrete, unidadeDeTrabalho);
                SetarColetas(ref rotaFrete, unidadeDeTrabalho);
                SetarPontosPassagemPreDefinidos(ref rotaFrete, unidadeDeTrabalho);
                SetarFiliais(ref rotaFrete, unidadeDeTrabalho);
                SetarTiposCarga(ref rotaFrete, unidadeDeTrabalho);

                SetarVeiculos(ref rotaFrete, unidadeDeTrabalho);
                AtualizarRestricoes(rotaFrete, unidadeDeTrabalho);
                AtualizarEmpresas(rotaFrete, unidadeDeTrabalho);
                AtualizarEmpresasExclusivas(rotaFrete, unidadeDeTrabalho);
                new Servicos.Embarcador.Carga.RotaFrete(unidadeDeTrabalho).SetarPontosPassagem(rotaFrete, pontosDaRota);
                SetarPostosFiscais(ref rotaFrete, unidadeDeTrabalho);

                repRotaFrete.Atualizar(rotaFrete, Auditado);

                if (rotaFrete.SituacaoDaRoteirizacao == SituacaoRoteirizacao.Concluido && situacaoRoteirizacaoInicial != SituacaoRoteirizacao.Concluido)
                    Servicos.Embarcador.Carga.RotaFrete.SetarCargaPendentesRota(rotaFrete, unidadeDeTrabalho, TipoServicoMultisoftware, ConfiguracaoEmbarcador);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unidadeDeTrabalho.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                bool duplicar = Request.GetBoolParam("Duplicar");

                Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unidadeDeTrabalho);
                Dominio.Entidades.RotaFrete rotaFrete = repositorioRotaFrete.BuscarPorCodigo(codigo);

                if (rotaFrete == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Repositorio.RotaFretePracaPedagio repRotaFretePracaPedagio = new Repositorio.RotaFretePracaPedagio(unidadeDeTrabalho);
                Repositorio.RotaFreteFrequenciaEntrega repositorioRotaFreteFrenquenciaEntrega = new Repositorio.RotaFreteFrequenciaEntrega(unidadeDeTrabalho);
                Repositorio.RotaFreteVeiculo repositorioRotaFreteVeiculo = new Repositorio.RotaFreteVeiculo(unidadeDeTrabalho);
                TimeSpan tempoEmMinutos = TimeSpan.FromMinutes(rotaFrete.TempoDeViagemEmMinutos);
                List<Dominio.Entidades.RotaFretePracaPedagio> rotaFretePracasPedagios = repRotaFretePracaPedagio.BuscarPorRotaFrete(codigo);
                List<Dominio.Entidades.RotaFreteFrequenciaEntrega> rotaFreteFrequenciaEntrega = repositorioRotaFreteFrenquenciaEntrega.BuscarPorRotaFrete(rotaFrete.Codigo);
                List<Dominio.Entidades.RotaFreteVeiculo> rotaFreteVeiculo = repositorioRotaFreteVeiculo.BuscarPorRotaFrete(rotaFrete.Codigo);
                bool ordenarLocalidades = rotaFrete.Localidades?.Any(o => o.Ordem > 0) ?? false;
                bool usarOutroEnderecoOrigem = rotaFrete.RemetenteOutroEndereco != null;
                var retorno = new
                {
                    Codigo = duplicar ? 0 : rotaFrete.Codigo,
                    rotaFrete.Descricao,
                    rotaFrete.CodigoIntegracaoNOX,
                    rotaFrete.Detalhes,
                    rotaFrete.Observacao,
                    HoraInicioCarregamento = rotaFrete.HoraInicioCarregamento.HasValue ? rotaFrete.HoraInicioCarregamento.Value.ToString(@"hh\:mm") : "",
                    HoraLimiteSaidaCD = rotaFrete.HoraLimiteSaidaCD.HasValue ? rotaFrete.HoraLimiteSaidaCD.Value.ToString(@"hh\:mm") : "",
                    TempoCarregamento = rotaFrete.TempoCarregamentoTicks > 0 ? $"{(int)rotaFrete.TempoCarregamento.TotalHours:d3}:{rotaFrete.TempoCarregamento:mm}" : "",
                    TempoDescarga = rotaFrete.TempoDescargaTicks > 0 ? $"{(int)rotaFrete.TempoDescarga.TotalHours:d3}:{rotaFrete.TempoDescarga:mm}" : "",
                    //TempoDescarga = rotaFrete.TempoDescarga.HasValue ? rotaFrete.TempoDescarga.Value.ToString(@"hh\:mm") : "",
                    rotaFrete.ApenasObterPracasPedagio,
                    UsarOutroEnderecoRemetente = usarOutroEnderecoOrigem,
                    RegiaoDestino = new
                    {
                        Codigo = rotaFrete.RegiaoDestino?.Codigo,
                        Descricao = rotaFrete.RegiaoDestino?.Descricao
                    },
                    CanalEntrega = new
                    {
                        Codigo = rotaFrete.CanalEntrega?.Codigo,
                        Descricao = rotaFrete.CanalEntrega?.Descricao
                    },
                    CanalVenda = new
                    {
                        Codigo = rotaFrete.CanalVenda?.Codigo,
                        Descricao = rotaFrete.CanalVenda?.Descricao
                    },
                    RemetenteOutroEndereco = new
                    {
                        Codigo = rotaFrete.RemetenteOutroEndereco?.Codigo,
                        Descricao = rotaFrete.RemetenteOutroEndereco?.Descricao
                    },
                    Remetente = new
                    {
                        Codigo = rotaFrete.Remetente?.Codigo,
                        rotaFrete.Remetente?.Descricao,
                        rotaFrete.Remetente?.Latitude,
                        rotaFrete.Remetente?.Longitude,
                        rotaFrete.Remetente?.Endereco,
                        rotaFrete.Remetente?.Numero,
                        Localidade = rotaFrete.Remetente?.Localidade.DescricaoCidadeEstado,
                        rotaFrete.Remetente?.CEP,
                        rotaFrete.Remetente?.Localidade.CodigoIBGE
                    },
                    Distribuidor = new
                    {
                        Codigo = rotaFrete.Distribuidor?.Codigo,
                        rotaFrete.Distribuidor?.Descricao,
                        rotaFrete.Distribuidor?.Latitude,
                        rotaFrete.Distribuidor?.Longitude,
                        rotaFrete.Distribuidor?.Endereco,
                        rotaFrete.Distribuidor?.Numero,
                        Localidade = rotaFrete.Distribuidor?.Localidade.DescricaoCidadeEstado,
                        rotaFrete.Distribuidor?.CEP,
                        rotaFrete.Distribuidor?.Localidade.CodigoIBGE
                    },
                    ExpedidorPedidosDiferenteOrigemRota = new
                    {
                        Codigo = rotaFrete.ExpedidorPedidosDiferenteOrigemRota?.CPF_CNPJ ?? 0,
                        Descricao = rotaFrete.ExpedidorPedidosDiferenteOrigemRota?.Descricao ?? "",
                        Latitude = rotaFrete.ExpedidorPedidosDiferenteOrigemRota?.Latitude?.ToDecimal() ?? 0,
                        Longitude = rotaFrete.ExpedidorPedidosDiferenteOrigemRota?.Longitude?.ToDecimal() ?? 0
                    },
                    GrupoPessoas = new
                    {
                        Codigo = rotaFrete.GrupoPessoas?.Codigo ?? 0,
                        Descricao = rotaFrete.GrupoPessoas?.Descricao ?? string.Empty
                    },
                    rotaFrete.Ativo,
                    rotaFrete.CodigoIntegracao,
                    TempoDeViagemEmMinutos = $"{(int)tempoEmMinutos.TotalHours:D2}:{tempoEmMinutos.Minutes:D2}",
                    TempoDeViagemEmDias = rotaFrete.TempoDeViagemEmMinutos > 0 ? (rotaFrete.TempoDeViagemEmMinutos / 1440) : 0,
                    PadraoTempo = rotaFrete.PadraoTempo.HasValue ? rotaFrete.PadraoTempo.Value : PadraoTempoDiasMinutos.Minutos,
                    Quilometros = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? rotaFrete.Quilometros.ToString("n0") : rotaFrete.Quilometros.ToString("n2")),
                    rotaFrete.PermiteAgruparCargas,
                    rotaFrete.FilialDistribuidora,
                    rotaFrete.VincularMotoristaFilaCarregamentoManualmente,
                    rotaFrete.GerarRedespachoAutomaticamente,
                    rotaFrete.PolilinhaRota,
                    PontosDaRota = duplicar ? "" : Servicos.Embarcador.Carga.RotaFrete.ObterRotaFreteSerializada(rotaFrete, unidadeDeTrabalho),
                    rotaFrete.VelocidadeMediaCarregado,
                    rotaFrete.VelocidadeMediaVazio,
                    rotaFrete.RotaRoteirizada,
                    rotaFrete.TipoUltimoPontoRoteirizacao,
                    rotaFrete.TipoUltimoPontoRoteirizacaoPorEstado,
                    rotaFrete.SituacaoDaRoteirizacao,
                    rotaFrete.MotivoFalhaRoteirizacao,
                    rotaFrete.TipoRota,
                    rotaFrete.TipoCarregamentoIda,
                    rotaFrete.TipoCarregamentoVolta,
                    rotaFrete.CodigoIntegracaoValePedagio,
                    rotaFrete.CodigoIntegracaoValePedagioRetorno,
                    rotaFrete.CodigoIntegracaoGerenciadoraRisco,
                    rotaFrete.RotaExclusivaCompraValePedagio,
                    rotaFrete.UtilizarDistanciaRotaCarga,
                    rotaFrete.VoltarPeloMesmoCaminhoIda,
                    TipoOperacao = new { Codigo = rotaFrete.TipoOperacao?.Codigo ?? 0, Descricao = rotaFrete.TipoOperacao?.Descricao ?? string.Empty },
                    TipoOperacaoPreCarga = new { Codigo = rotaFrete.TipoOperacaoPreCarga?.Codigo ?? 0, Descricao = rotaFrete.TipoOperacaoPreCarga?.Descricao ?? string.Empty },
                    Classificacao = new { codigo = rotaFrete.Classificacao?.Codigo ?? 0, Descricao = rotaFrete.Classificacao?.Descricao ?? "" },
                    LocalidadeOrigem = rotaFrete.LocalidadesOrigem.Count > 0 ? new
                    {
                        rotaFrete.LocalidadesOrigem.FirstOrDefault().Codigo,
                        Descricao = rotaFrete.LocalidadesOrigem.FirstOrDefault().DescricaoCidadeEstado,
                        Latitude = rotaFrete.LocalidadesOrigem.FirstOrDefault()?.Latitude ?? 0,
                        Longitude = rotaFrete.LocalidadesOrigem.FirstOrDefault()?.Longitude ?? 0,
                        UtilizaLocalidade = true
                    } : new { Codigo = 0, Descricao = "", Latitude = Convert.ToDecimal(0), Longitude = Convert.ToDecimal(0), UtilizaLocalidade = false },
                    Estados = (
                        from obj in rotaFrete.Estados
                        select new
                        {
                            Codigo = obj.Sigla,
                            Descricao = obj.Nome
                        }
                    ).ToList(),
                    EstadosOrigem = (
                        from obj in rotaFrete.EstadosOrigem
                        select new
                        {
                            Codigo = obj.Sigla,
                            Descricao = obj.Nome
                        }
                    ).ToList(),
                    TipoCargas = (
                        from obj in rotaFrete.RotaFreteTiposCarga
                        select new
                        {
                            Codigo = obj.TipoDeCarga.Codigo,
                            Descricao = obj.TipoDeCarga.Descricao
                        }
                    ).ToList(),
                    Filiais = (
                        from obj in rotaFrete.RotaFreteFiliais
                        select new
                        {
                            Codigo = obj.Filial.Codigo,
                            Descricao = obj.Filial.Descricao
                        }
                    ).ToList(),
                    DadosLocalidades = new
                    {
                        Ordenar = ordenarLocalidades,
                        Localidades = (
                            from obj in rotaFrete.Localidades
                            select new
                            {
                                obj.Ordem,
                                Codigo = obj.Localidade.Codigo,
                                Descricao = obj.Localidade.DescricaoCidadeEstado,
                                Latitude = obj.Localidade.Latitude ?? 0,
                                Longitude = obj.Localidade.Longitude ?? 0,
                                DT_RowId = obj.Localidade.Codigo,
                                utilizaLocalidades = true
                            }

                        ).ToList()
                    },
                    Destinatarios = (
                        from obj in rotaFrete.Destinatarios
                        select new
                        {
                            Codigo = obj.Cliente.Codigo,
                            obj.Cliente.Nome,
                            obj.Cliente.Descricao,
                            Latitude = obj.ClienteOutroEndereco == null ? obj.Cliente.Latitude : obj.ClienteOutroEndereco.Latitude,
                            Longitude = obj.ClienteOutroEndereco == null ? obj.Cliente.Longitude : obj.ClienteOutroEndereco.Longitude,
                            obj.Cliente.Endereco,
                            obj.Cliente.Numero,
                            Localidade = obj.Cliente.Localidade.DescricaoCidadeEstado,
                            obj.Cliente.CEP,
                            obj.Cliente.Localidade.CodigoIBGE,
                            obj.Ordem,
                            CodigoOutroEndereco = obj.ClienteOutroEndereco?.Codigo ?? 0,
                            DescricaoOutroEndereco = obj.ClienteOutroEndereco?.Descricao ?? ""
                        }
                    ).OrderBy(o => o.Ordem).ToList(),
                    Fronteiras = (
                        from obj in rotaFrete.Fronteiras
                        select new
                        {
                            Codigo = obj.Cliente.Codigo,
                            obj.Cliente.Nome,
                            obj.Cliente.Descricao,
                            obj.Cliente.Latitude,
                            obj.Cliente.Longitude,
                            obj.Cliente.Endereco,
                            obj.Cliente.Numero,
                            Localidade = obj.Cliente.Localidade.DescricaoCidadeEstado,
                            obj.Cliente.CEP,
                            obj.Cliente.Localidade.CodigoIBGE,
                            obj.Ordem,
                            TempoMedioPermanenciaFronteira = $"{obj.TempoMedioPermanenciaFronteira / 60:D2}:{obj.TempoMedioPermanenciaFronteira % 60:D2}",
                            TempoMedioPermanenciaFronteiraMinutos = obj.TempoMedioPermanenciaFronteira,
                        }
                    ).OrderBy(o => o.Ordem).ToList(),
                    Coletas = (
                        from obj in rotaFrete.Coletas
                        select new
                        {
                            Codigo = obj.Codigo,
                            obj.Descricao,
                            obj.Latitude,
                            obj.Longitude,
                            obj.Endereco,
                            obj.Numero,
                            Localidade = obj.Localidade.DescricaoCidadeEstado,
                            obj.CEP,
                            obj.Localidade.CodigoIBGE
                        }
                    ).ToList(),
                    PontosPassagemPreDefinido = (
                        from obj in rotaFrete.PontoPassagemPreDefinido
                        select new
                        {
                            Codigo = obj.Codigo,
                            CodigoCliente = obj.Cliente?.CPF_CNPJ,
                            CodigoLocalidade = obj.Localidade?.Codigo ?? 0,
                            Descricao = obj.ObterDescricao(),
                            Latitude = obj.ObterLatitude(),
                            Longitude = obj.ObterLongitude(),
                            TempoEstimadoPermanencia = obj.TempoEstimadoPermanenciaFormatado,
                            TempoEstimadoPermanenciaMinutos = obj.TempoEstimadoPermanencia,
                            LocalDeParqueamento = obj.LocalDeParqueamento ? "Sim" : "Não",
                        }
                    ).ToList(),
                    PostosFiscais = (
                        from obj in rotaFrete.PostosFiscais
                        select new
                        {
                            Codigo = obj.Codigo,
                            CodigoCliente = obj.Cliente.CPF_CNPJ,
                            Descricao = obj.Cliente.Descricao,
                            TempoEstimadoPermanencia = obj.TempoEstimadoPermanenciaFormatado,
                            Latitude = obj.Cliente.Latitude,
                            Longitude = obj.Cliente.Longitude,
                        }
                    ).ToList(),
                    PracaPedagios = (
                        from obj in rotaFretePracasPedagios
                        select new
                        {
                            Codigo = obj.PracaPedagio?.Codigo ?? 0,
                            Descricao = obj.PracaPedagio?.Descricao ?? string.Empty,
                            Latitude = obj.PracaPedagio?.Latitude ?? string.Empty,
                            Longitude = obj.PracaPedagio?.Longitude ?? string.Empty,
                            Rodovia = obj.PracaPedagio?.Rodovia ?? string.Empty,
                            KM = obj.PracaPedagio?.KM.ToString("n2") ?? string.Empty,
                            Sentido = obj.EixosSuspenso.ObterDescricao(),
                            Ordem = (rotaFretePracasPedagios.Count == 0 ? 0 : rotaFretePracasPedagios.FindIndex(x => x.Codigo == obj.Codigo))
                        }
                    ).ToList(),
                    Restricoes = ObterRestricoes(rotaFrete, duplicar),
                    Empresas = ObterEmpresas(rotaFrete, duplicar),
                    EmpresasExclusivas = ObterEmpresasExclusivas(rotaFrete, duplicar, unidadeDeTrabalho),
                    RestricaoEntrega = rotaFreteFrequenciaEntrega != null && rotaFreteFrequenciaEntrega.Count > 0 ? rotaFreteFrequenciaEntrega.Select(o => o.DiaSemana).ToList() : null,
                    Veiculos = (
                        from obj in rotaFreteVeiculo
                        select new
                        {
                            Codigo = obj.Veiculo.Codigo,
                            Descricao = obj.Veiculo.Descricao
                        }
                    ).ToList(),
                    rotaFrete.ValidarParaQualquerDestinatarioInformado
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unidadeDeTrabalho);
                Dominio.Entidades.RotaFrete rotaFrete = repositorioRotaFrete.BuscarPorCodigo(codigo);

                if (rotaFrete == null)
                    return new JsonpResult(false, true, Localization.Resources.Logistica.RotaFrete.RotaDeFreteNaoEncontrada);

                unidadeDeTrabalho.Start();

                rotaFrete.Estados = null;

                repositorioRotaFrete.Deletar(rotaFrete, Auditado);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> SalvarCEP()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0, codigoRota = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("CodigoRota"), out codigoRota);

                int cepInicial = Utilidades.String.OnlyNumbers(Request.Params("CEPInicial")).ToInt();
                int cepFinal = Utilidades.String.OnlyNumbers(Request.Params("CEPFinal")).ToInt();

                Repositorio.Embarcador.Logistica.RotaFreteCEP repRotaFreteCEP = new Repositorio.Embarcador.Logistica.RotaFreteCEP(unidadeDeTrabalho);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP rotaFreteCEP;
                if (codigo > 0)
                    rotaFreteCEP = repRotaFreteCEP.BuscarPorCodigo(codigo, true);
                else
                    rotaFreteCEP = new Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP();

                if (codigoRota <= 0)
                    return new JsonpResult(true, false, Localization.Resources.Logistica.RotaFrete.RotaNaoEncontrada);

                unidadeDeTrabalho.Start();

                rotaFreteCEP.CEPFinal = cepFinal;
                rotaFreteCEP.CEPInicial = cepInicial;
                rotaFreteCEP.LeadTime = Request.GetIntParam("LeadTime");
                rotaFreteCEP.PercentualADValorem = Request.GetDecimalParam("PercentualADValorem");
                rotaFreteCEP.RotaFrete = repRotaFrete.BuscarPorCodigo(codigoRota);

                if (codigo > 0)
                    repRotaFreteCEP.Atualizar(rotaFreteCEP, Auditado);
                else
                    repRotaFreteCEP.Inserir(rotaFreteCEP);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Logistica.RotaFrete.OcorreuUmaFalhaAoSalvarCEP);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirCEP()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Utilidades.String.OnlyNumbers(Request.Params("Codigo")));

                Repositorio.Embarcador.Logistica.RotaFreteCEP repRotaFreteCEP = new Repositorio.Embarcador.Logistica.RotaFreteCEP(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP rotaFrete = repRotaFreteCEP.BuscarPorCodigo(codigo);

                repRotaFreteCEP.Deletar(rotaFrete, Auditado);

                unidadeDeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelExcluirRegistro);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExcluir);

            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> OrdenarRota()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

                var pontos = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<dynamic>>(Request.Params("pontos"));

                if (pontos.Count < 2)
                    return new JsonpResult(false, true, Localization.Resources.Logistica.RotaFrete.ARotaDeveTerDoisPontos);

                int tipoUltimoPontoRoteirizacao = Request.GetIntParam("tipoUltimoPontoRoteirizacao");
                bool manterPontoDestino = Request.GetBoolParam("manterPontoDestino");

                //Vamos usar para quem utilizar o VRP, ou VRPTimeWindow chamar a API de otimização.
                int codigoCarregamento = Request.GetIntParam("codigoCarregamento");

                bool ordenar = true;

                bool ateOrigem = tipoUltimoPontoRoteirizacao == 2 || tipoUltimoPontoRoteirizacao == 1;//AteOrigem

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarregamentoVRP tipoMontagemCarregamentoVRP = TipoMontagemCarregamentoVRP.Nenhum;
                List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido> carregamentoPedidos = null;
                Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = null;

                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = null;

                bool roteirizacaoPedidosOrigemRecebedor = false;
                Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento = null;
                if (codigoCarregamento > 0)
                {
                    carregamento = new Repositorio.Embarcador.Cargas.MontagemCarga.Carregamento(unidadeDeTrabalho).BuscarPorCodigo(codigoCarregamento);

                    //Se existir sessão, vamos obter a FILIAL, analisar no centro de carregamento as configurações se é VRP ou TimeWindow
                    if (carregamento?.SessaoRoteirizador?.Filial?.Codigo > 0)
                    {
                        roteirizacaoPedidosOrigemRecebedor = carregamento.SessaoRoteirizador.RoteirizacaoRedespacho;

                        List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamento> centrosCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unidadeDeTrabalho).BuscarPorFiliais(new List<int>() { carregamento?.SessaoRoteirizador?.Filial?.Codigo ?? 0 });
                        if (centrosCarregamento?.Count > 0)
                        {
                            centroCarregamento = centrosCarregamento[0];
                            tipoMontagemCarregamentoVRP = (from centro in centrosCarregamento
                                                           select centro.TipoMontagemCarregamentoVRP).FirstOrDefault();

                            //Vamos obter os pedidos do carregamento...
                            if (tipoMontagemCarregamentoVRP != TipoMontagemCarregamentoVRP.Nenhum)
                            {
                                carregamentoPedidos = new Repositorio.Embarcador.Cargas.MontagemCarga.CarregamentoPedido(unidadeDeTrabalho).BuscarPorCarregamento(codigoCarregamento);
                                modeloVeicularCarga = carregamento.ModeloVeicularCarga;
                            }
                        }
                    }
                }

                List<WayPoint> rotaordenada = new List<WayPoint>();
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao respostaRoteirizacao = null;

                if (codigoCarregamento == 0 || (carregamentoPedidos?.Count ?? 0) == 0)
                {
                    Roteirizacao rota = new Roteirizacao(configuracaoIntegracao.ServidorRouteOSM);

                    rota.Clear();

                    List<WayPoint> wayPoints = new List<WayPoint>();

                    WayPoint wayPointOrigem = null;
                    foreach (dynamic ponto in pontos)
                    {
                        if ((ponto.lat == null) || (ponto.lng == null))
                            return new JsonpResult(false, true, $" {Localization.Resources.Logistica.RotaFrete.LatitudeLongitudeInvalida} {ponto.descricao}");

                        double latitude = double.Parse(((string)ponto.lat).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                        double longitude = double.Parse(((string)ponto.lng).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);

                        if (latitude < -90 || latitude > 90)
                            return new JsonpResult(false, true, $" {Localization.Resources.Logistica.RotaFrete.LatitudeLongitudeInvalida} {ponto.descricao}");

                        if (longitude < -180 || longitude > 180)
                            return new JsonpResult(false, true, $" {Localization.Resources.Logistica.RotaFrete.LatitudeLongitudeInvalida} {ponto.descricao}");

                        if (ponto.ordem != null && (int)ponto.ordem > 0)
                            ordenar = false;

                        double codigo = ((string)ponto.codigo).ToDouble();
                        TipoPontoPassagem tipoPonto = ((string)ponto.tipoponto).ToEnum(TipoPontoPassagem.Entrega);

                        if (wayPoints.Exists(w => w.Codigo == codigo && w.TipoPonto == tipoPonto && (w.Lat == latitude || w.Lng == longitude)))
                            continue;

                        wayPoints.Add(new WayPoint
                        {
                            Lat = latitude,
                            Lng = longitude,
                            Descricao = ponto.descricao,
                            Pedagio = ponto.pedagio,
                            Fronteira = ponto.fronteira ?? false,
                            Tempo = 0,
                            Distancia = 0,
                            Codigo = codigo,
                            Sequencia = ponto.sequencia ?? 0,
                            TipoPonto = tipoPonto,
                            Informacao = ponto.informacao,
                            LocalDeParqueamento = ponto.localDeParqueamento ?? false,
                            tempoEstimadoPermanencia = ponto.tempoEstimadoPermanencia ?? 0,
                            PrimeiraEntrega = ponto?.primeiraEntrega ?? false,
                            CodigoOutroEndereco = ponto?.codigoOutroEndereco ?? 0,
                            UtilizaLocalidade = ponto?.utilizaLocalidade ?? false
                        });

                        if (tipoPonto == TipoPontoPassagem.Coleta && wayPointOrigem == null)
                            wayPointOrigem = wayPoints[wayPoints.Count - 1];
                    }

                    if (ateOrigem)
                    {
                        if (wayPointOrigem != null)
                        {
                            wayPoints.Add(new WayPoint
                            {
                                Lat = wayPointOrigem.Lat,
                                Lng = wayPointOrigem.Lng,
                                Descricao = wayPointOrigem.Descricao,
                                Pedagio = wayPointOrigem.Pedagio,
                                Tempo = 0,
                                Distancia = 0,
                                Codigo = wayPointOrigem.Codigo,
                                Sequencia = wayPointOrigem.Sequencia,
                                TipoPonto = TipoPontoPassagem.Retorno,
                                Informacao = wayPointOrigem.Informacao,
                                LocalDeParqueamento = false,
                                tempoEstimadoPermanencia = wayPointOrigem.tempoEstimadoPermanencia,
                                PrimeiraEntrega = wayPointOrigem.PrimeiraEntrega,
                                CodigoOutroEndereco = wayPointOrigem.CodigoOutroEndereco,
                                UtilizaLocalidade = wayPointOrigem.UtilizaLocalidade
                            });
                        }
                        else
                        {
                            dynamic pontoOrigem = pontos[0];

                            double latitude = double.Parse(((string)pontoOrigem.lat).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                            double longitude = double.Parse(((string)pontoOrigem.lng).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                            double codigo = ((string)pontoOrigem.codigo).ToDouble();
                            TipoPontoPassagem tipoPontoOrigem = ((string)pontoOrigem.tipoponto).ToEnum(TipoPontoPassagem.Entrega);

                            wayPointOrigem = new WayPoint
                            {
                                Lat = latitude,
                                Lng = longitude,
                                Descricao = pontoOrigem.descricao,
                                Pedagio = pontoOrigem.pedagio,
                                Tempo = 0,
                                Distancia = 0,
                                Codigo = codigo,
                                Sequencia = pontoOrigem.sequencia ?? 0,
                                TipoPonto = TipoPontoPassagem.Retorno,
                                Informacao = pontoOrigem.informacao,
                                LocalDeParqueamento = false,
                                tempoEstimadoPermanencia = pontoOrigem.tempoEstimadoPermanencia ?? 0,
                                PrimeiraEntrega = pontoOrigem?.primeiraEntrega ?? false,
                                CodigoOutroEndereco = pontoOrigem.codigoOutroEndereco,
                                UtilizaLocalidade = pontoOrigem?.utilizaLocalidade ?? false
                            };

                            wayPoints.Add(wayPointOrigem);
                        }
                    }

                    bool ordenou = false;
                    int coletas = (from w in wayPoints
                                   where w.TipoPonto == TipoPontoPassagem.Coleta
                                   select w).Distinct().Count();

                    List<WayPoint> wayPointsOrdenadas = new List<WayPoint>();
                    if (ordenar && coletas == 1 && tipoUltimoPontoRoteirizacao != 3) // Ponto mais distante...
                    {
                        try
                        {
                            Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools.Api api = new Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools.Api(configuracaoIntegracao.ServidorRouteGoogleOrTools, configuracaoIntegracao.ServidorRouteOSM)
                            {
                                Veiculos = new List<Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools.Veiculo>(),
                                Locais = new List<Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools.Local>(),
                                TipoRota = Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools.EnumCargaTpRota.ComRetorno,
                                Strategy = Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools.EnumFirstSolutionStrategy.PathCheapestArc
                            };

                            api.Locais.AddRange((from obj in wayPoints
                                                 where obj.TipoPonto != TipoPontoPassagem.Retorno
                                                 select new Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools.Local()
                                                 {
                                                     Codigo = (long)obj.Codigo,
                                                     Latitude = obj.Lat,
                                                     Longitude = obj.Lng,
                                                     Deposito = (obj.TipoPonto == TipoPontoPassagem.Coleta || obj.TipoPonto == TipoPontoPassagem.Retorno)
                                                 }).ToList());

                            Servicos.Embarcador.Carga.MontagemCarga.GoogleOrTools.ApiResultadoTsp resultado = api.TSP();

                            if ((resultado?.status ?? false) == true)
                            {
                                if (!resultado.result.itens.Any(x => x.item.Deposito) && wayPoints.Any(x => x.TipoPonto == TipoPontoPassagem.Coleta))
                                    wayPointsOrdenadas.Add((from w in wayPoints where w.TipoPonto == TipoPontoPassagem.Coleta select w).FirstOrDefault());

                                for (int i = 0; i < resultado.result.itens.Count; i++)
                                    wayPointsOrdenadas.AddRange((from obj in wayPoints
                                                                 where (long)obj.Codigo == resultado.result.itens[i].item.Codigo && obj.TipoPonto != TipoPontoPassagem.Coleta && obj.TipoPonto != TipoPontoPassagem.Retorno
                                                                 select obj).ToList());

                                if (ateOrigem)
                                    wayPointsOrdenadas.Add((from w in wayPoints where w.TipoPonto == TipoPontoPassagem.Retorno select w).FirstOrDefault());

                                ordenou = true;
                            }
                            else
                                wayPointsOrdenadas = wayPoints;
                        }
                        catch { wayPointsOrdenadas = wayPoints; }
                    }
                    else
                        wayPointsOrdenadas = wayPoints;

                    rota.Add(wayPointsOrdenadas);

                    if (ordenou)
                    {
                        var opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar
                        {
                            AteOrigem = false,
                            Ordenar = false,
                            PontosNaRota = false
                        };
                        respostaRoteirizacao = rota.Roteirizar(opcoes);

                        rotaordenada = wayPointsOrdenadas;

                        respostaRoteirizacao = Servicos.Embarcador.Carga.CargaRotaFrete.AnalisarGerarRoteirizacaoComDesvioZonaExclusao(respostaRoteirizacao, string.Empty, false, unidadeDeTrabalho, ateOrigem, ordenou);
                        if (respostaRoteirizacao.Status == "OK")
                        {
                            var pontosDaRota = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>>(respostaRoteirizacao.PontoDaRota);
                            if (rota.GetWayPoints().Count != pontosDaRota.Count && pontosDaRota.Any(x => x.Descricao.ToUpper().StartsWith("DESVIO")))
                            {
                                rotaordenada.Clear();
                                rotaordenada = pontosDaRota;
                            }
                        }
                    }
                    else if (ordenar)
                    {
                        var opcoes = new Servicos.Embarcador.Logistica.OpcoesRoteirizar
                        {
                            AteOrigem = false,
                            Ordenar = false,
                            PontosNaRota = false
                        };
                        rotaordenada = rota.GetPontosEmOrdem(ateOrigem, manterPontoDestino);
                        rota.Clear();
                        rota.Add(rotaordenada);

                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> wayPointDestinosRemovidos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>();
                        Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint wayPointRetorno = null;
                        Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa = Servicos.Embarcador.Carga.CargaRotaFrete.AnalisarGerarRoteirizacaoComTrechoBalsa(rota, ref wayPointDestinosRemovidos, ref wayPointRetorno, unidadeDeTrabalho);
                        respostaRoteirizacao = rota.Roteirizar(opcoes);
                        respostaRoteirizacao = Servicos.Embarcador.Carga.CargaRotaFrete.AnalisarGerarRoteirizacaoAdicionarTrechoBalsa(respostaRoteirizacao, trechoBalsa, wayPointDestinosRemovidos, wayPointRetorno, configuracaoIntegracao.ServidorRouteOSM, unidadeDeTrabalho);
                        respostaRoteirizacao = Servicos.Embarcador.Carga.CargaRotaFrete.AnalisarGerarRoteirizacaoComDesvioZonaExclusao(respostaRoteirizacao, string.Empty, false, unidadeDeTrabalho, ateOrigem);
                        if (respostaRoteirizacao.Status == "OK")
                        {
                            var pontosDaRota = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>>(respostaRoteirizacao.PontoDaRota);
                            if (rota.GetWayPoints().Count != pontosDaRota.Count && pontosDaRota.Any(x => x.Descricao.ToUpper().StartsWith("DESVIO")))
                            {
                                rotaordenada.Clear();
                                rotaordenada = pontosDaRota;
                            }
                        }
                    }
                    else
                        rotaordenada = rota.GetWayPoints();
                }
                else
                {
                    long codigoClientePrimeiraEntrega = 0;

                    foreach (dynamic ponto in pontos)
                    {
                        if (((string)ponto.primeiraEntrega).ToBool() == true)
                            codigoClientePrimeiraEntrega = ((string)ponto.codigo).ToLong();
                    }

                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = (from ped in carregamentoPedidos
                                                                                 select ped.Pedido).ToList();

                    respostaRoteirizacao = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unidadeDeTrabalho, ConfiguracaoEmbarcador).RoteirizarPedidos(pedidos, modeloVeicularCarga, configuracaoIntegracao.ServidorRouteOSM, (TipoUltimoPontoRoteirizacao)tipoUltimoPontoRoteirizacao, centroCarregamento, carregamento, ordenar, roteirizacaoPedidosOrigemRecebedor, codigoClientePrimeiraEntrega);

                    string pontosDaRota = respostaRoteirizacao?.PontoDaRota;

                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontosResultado = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>>(pontosDaRota);
                    int seq = 0;
                    int index = 0;
                    foreach (PontosDaRota sequencia in pontosResultado)
                    {
                        seq = (from x in pontos where ((string)x.codigo).ToDouble() == sequencia.codigo && sequencia.codigoOutroEndereco == (int)x.codigoOutroEndereco select x.sequencia)?.FirstOrDefault() ?? -1;
                        if (seq < 0)
                            seq = (from x in pontos where ((string)x.codigo).ToDouble() == sequencia.codigo_cliente && sequencia.codigoOutroEndereco == (int)x.codigoOutroEndereco select x.sequencia)?.FirstOrDefault() ?? -1;
                        if (seq < 0) seq = 0;
                        if (sequencia.tipoponto == TipoPontoPassagem.Coleta && seq == 0 && codigoClientePrimeiraEntrega > 0) continue;

                        rotaordenada.Add(new WayPoint()
                        {
                            Lat = sequencia.lat,
                            Lng = sequencia.lng,
                            Descricao = sequencia.descricao,
                            Pedagio = sequencia.pedagio,
                            Tempo = sequencia.tempo,
                            Distancia = sequencia.distancia,
                            Codigo = sequencia.codigo,
                            CodigoCliente = sequencia.codigo_cliente,
                            UsarOutroEndereco = sequencia.usarOutroEndereco,
                            Sequencia = seq, // Contem a sequencia de entrada da requisição.....
                            Index = index,
                            TipoPonto = sequencia.tipoponto,
                            LocalDeParqueamento = sequencia.localDeParqueamento,
                            Informacao = sequencia.descricao,
                            PrimeiraEntrega = sequencia.primeiraEntrega,
                            CodigoOutroEndereco = sequencia.codigoOutroEndereco,
                            UtilizaLocalidade = sequencia?.utilizaLocalidade ?? false
                        });
                        index++;
                    }
                }

                return new JsonpResult(new
                {
                    RotaOrdenada = rotaordenada,
                    RespostaOSM = new
                    {
                        Distancia = respostaRoteirizacao?.Distancia ?? 0,
                        Status = respostaRoteirizacao?.Status ?? "Erro",
                        Polilinha = respostaRoteirizacao?.Polilinha ?? "",
                        TempoMinutos = respostaRoteirizacao?.TempoMinutos ?? 0
                    }
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, Localization.Resources.Logistica.RotaFrete.FalhaAoOrdenarRota);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ProcessarRotasAntigas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.Rota repRota = new Repositorio.Embarcador.Logistica.Rota(unitOfWork);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);

                List<Dominio.Entidades.Embarcador.Logistica.Rota> rotas = repRota.BuscarRotasProcessamento();

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Logistica.Rota rota in rotas)
                {
                    Dominio.Entidades.RotaFrete rotaFrete = repRotaFrete.BuscarParaProcessamento(rota.Remetente.CPF_CNPJ, rota.Destinatario.CPF_CNPJ);
                    if (rotaFrete == null)
                    {
                        rotaFrete = new Dominio.Entidades.RotaFrete()
                        {
                            Descricao = rota.Remetente.CodigoIntegracao + (!string.IsNullOrWhiteSpace(rota.Remetente.CodigoIntegracao) && !string.IsNullOrWhiteSpace(rota.Destinatario.CodigoIntegracao) ? " - " : "") + rota.Destinatario.CodigoIntegracao,
                            Remetente = rota.Remetente,
                            Destinatarios = new List<Dominio.Entidades.RotaFreteDestinatarios>(),
                            CodigoIntegracao = rota.DescricaoRotaSemParar,
                            Quilometros = rota.DistanciaKM,
                            Ativo = rota.Ativo
                        };

                        rotaFrete.Destinatarios.Add(new Dominio.Entidades.RotaFreteDestinatarios
                        {
                            Cliente = rota.Destinatario,
                            Ordem = 0
                        });

                        repRotaFrete.Inserir(rotaFrete);
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoRota();

            return new JsonpResult(configuracoes);
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                Repositorio.RotaFreteFrequenciaEntrega repRotaFreteFrequenciaEntrega = new Repositorio.RotaFreteFrequenciaEntrega(unitOfWork);
                Repositorio.RotaFreteLocalidade repositorioRotaFreteLocalidade = new Repositorio.RotaFreteLocalidade(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.RotaFreteDestinatarios repositorioRotaFreteDestinatario = new Repositorio.RotaFreteDestinatarios(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.RotaFreteEmpresaExclusiva repRotaFreteEmpresaExclusiva = new Repositorio.RotaFreteEmpresaExclusiva(unitOfWork);
                Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];
                        string retorno = "";

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoIntegracao = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracao" select obj).FirstOrDefault();
                        string codigoIntegracao = "";
                        if (colCodigoIntegracao != null)
                            codigoIntegracao = colCodigoIntegracao.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoIntegracaoValePedagio = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoValePedagio" select obj).FirstOrDefault();
                        string codigoIntegracaoValePedagio = "";
                        if (colCodigoIntegracaoValePedagio != null)
                            codigoIntegracaoValePedagio = colCodigoIntegracaoValePedagio.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDescricao = (from obj in linha.Colunas where obj.NomeCampo == "Descricao" select obj).FirstOrDefault();
                        string descricao = "";
                        if (colDescricao != null)
                            descricao = colDescricao.Valor;

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoOperacaoPreCarga = (from obj in linha.Colunas where obj.NomeCampo == "TipoOperacaoPreCarga" select obj).FirstOrDefault();
                        Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoPreCarga = null;
                        if (colTipoOperacaoPreCarga != null)
                        {
                            tipoOperacaoPreCarga = repTipoOperacao.BuscarPorCodigoIntegracao(colTipoOperacaoPreCarga.Valor);
                            if (tipoOperacaoPreCarga == null)
                                retorno = Localization.Resources.Logistica.RotaFrete.TipoOperacaoNaoLocalizadoMultisoftware;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTipoOperacao = (from obj in linha.Colunas where obj.NomeCampo == "TipoOperacao" select obj).FirstOrDefault();
                        Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = null;
                        if (colTipoOperacao != null)
                        {
                            tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(colTipoOperacao.Valor);
                            if (tipoOperacao == null)
                                retorno = Localization.Resources.Logistica.RotaFrete.TipoOperacaoNaoLocalizadoMultisoftware;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colInicioCarregamento = (from obj in linha.Colunas where obj.NomeCampo == "InicioCarregamento" select obj).FirstOrDefault();
                        string inicioCarregamento = "";
                        if (colInicioCarregamento != null)
                        {
                            inicioCarregamento = colInicioCarregamento.Valor.Trim();
                            string[] inicioCarregamentoSplit = inicioCarregamento.Split(' ');
                            if (inicioCarregamentoSplit.Length > 0)
                                inicioCarregamento = inicioCarregamentoSplit[inicioCarregamentoSplit.Length - 1];

                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLimiteSaida = (from obj in linha.Colunas where obj.NomeCampo == "LimiteSaida" select obj).FirstOrDefault();
                        string limiteSaida = "";
                        if (colLimiteSaida != null)
                        {
                            limiteSaida = colLimiteSaida.Valor.Trim();
                            string[] limiteSaidaSplit = limiteSaida.Split(' ');
                            if (limiteSaidaSplit.Length > 0)
                                limiteSaida = limiteSaidaSplit[limiteSaidaSplit.Length - 1];
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTempoCarregamento = (from obj in linha.Colunas where obj.NomeCampo == "TempoCarregamento" select obj).FirstOrDefault();
                        string tempoCarregamento = "";
                        if (colTempoCarregamento != null)
                        {
                            tempoCarregamento = colTempoCarregamento.Valor.Trim();
                            string[] tempoCarregamentoSplit = tempoCarregamento.Split(' ');
                            if (tempoCarregamentoSplit.Length > 0)
                                tempoCarregamento = tempoCarregamentoSplit[tempoCarregamentoSplit.Length - 1];
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTempoDecarga = (from obj in linha.Colunas where obj.NomeCampo == "TempoDescarga" select obj).FirstOrDefault();
                        string tempoDescarga = "";
                        if (colTempoDecarga != null)
                        {
                            tempoDescarga = colTempoDecarga.Valor.Trim();
                            string[] tempoDescargaSplit = tempoDescarga.Split(' ');
                            if (tempoDescargaSplit.Length > 0)
                                tempoDescarga = tempoDescargaSplit[tempoDescargaSplit.Length - 1];
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCNPJCPFRemetente = (from obj in linha.Colunas where obj.NomeCampo == "CNPJCPFRemetente" select obj).FirstOrDefault();
                        double cnpjCPFRemetente = 0;
                        Dominio.Entidades.Cliente remetente = null;
                        if (colCNPJCPFRemetente != null)
                        {
                            double.TryParse(Utilidades.String.OnlyNumbers(colCNPJCPFRemetente.Valor), out cnpjCPFRemetente);
                            remetente = repCliente.BuscarPorCPFCNPJ(cnpjCPFRemetente);

                            if (remetente == null)
                                remetente = repCliente.BuscarPorCodigoIntegracao(colCNPJCPFRemetente.Valor);

                            if (remetente == null)
                                retorno = Localization.Resources.Logistica.RotaFrete.NaoFoiEncontradoClienteCNJPBaseMultisoftware;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colRegiaoDestino = (from obj in linha.Colunas where obj.NomeCampo == "RegiaoDestino" select obj).FirstOrDefault();
                        string strRegiaoDestino = "";
                        Dominio.Entidades.Embarcador.Localidades.Regiao regiaoDestino = null;
                        if (colRegiaoDestino != null)
                        {
                            strRegiaoDestino = colRegiaoDestino.Valor.Trim();

                            regiaoDestino = repRegiao.BuscarPorCodigoIntegracao(strRegiaoDestino);
                            if (regiaoDestino == null)
                                regiaoDestino = repRegiao.BuscarPorDescricao(strRegiaoDestino);

                            if (regiaoDestino == null)
                                retorno = Localization.Resources.Logistica.RotaFrete.NaoFoiEncontradoRegiaoDestinoBaseMultisoftware;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLocalidadeDestino = (from obj in linha.Colunas where obj.NomeCampo == "LocalidadeDestino" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colUFDestino = (from obj in linha.Colunas where obj.NomeCampo == "UFDestino" select obj).FirstOrDefault();

                        Dominio.Entidades.Localidade destino = null;
                        if (colLocalidadeDestino != null && colUFDestino != null)
                        {
                            string localidade = Utilidades.String.RemoveDiacritics(colLocalidadeDestino.Valor);
                            string ufDestino = colUFDestino.Valor;
                            destino = repLocalidade.BuscarPorDescricaoEUF(localidade.Trim(), ufDestino.Trim());

                            if (destino == null)
                                retorno = Localization.Resources.Logistica.RotaFrete.NaoFoiEncontradoLocalidadeDestinoBaseMultisoftware;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colUFOrigem = (from obj in linha.Colunas where obj.NomeCampo == "UFOrigem" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colLocalidadeOrigem = (from obj in linha.Colunas where obj.NomeCampo == "LocalidadeOrigem" select obj).FirstOrDefault();

                        Dominio.Entidades.Localidade origem = null;
                        Dominio.Entidades.Estado estadoOrigem = null;
                        if (colUFOrigem != null && colLocalidadeOrigem != null)
                        {
                            string ufOrigem = colUFOrigem.Valor;
                            string localidade = Utilidades.String.RemoveDiacritics(colLocalidadeOrigem.Valor);
                            origem = repLocalidade.BuscarPorDescricaoEUF(localidade.Trim(), ufOrigem.Trim());

                            if (origem == null)
                                retorno = Localization.Resources.Logistica.RotaFrete.NaoFoiEncontradoLocalidadeOrigemBaseMultisoftware;
                        }
                        else if (colUFOrigem != null)
                        {
                            estadoOrigem = repEstado.BuscarPorSigla(colUFOrigem.Valor);

                            if (estadoOrigem == null)
                                retorno = Localization.Resources.Logistica.RotaFrete.NaoFoiEncontradoEstadoOrigemBaseMultisoftware;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colDistancia = (from obj in linha.Colunas where obj.NomeCampo == "Distancia" select obj).FirstOrDefault();
                        decimal distancia = 0;
                        if (colDistancia != null)
                            distancia = ((string)colDistancia.Valor).ToDecimal();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTempoDeViagemEmMinutos = (from obj in linha.Colunas where obj.NomeCampo == "TempoDeViagemEmMinutos" select obj).FirstOrDefault();
                        int tempoDeViagemEmMinutos = 0;
                        if (colTempoDeViagemEmMinutos != null)
                            tempoDeViagemEmMinutos = ((string)colTempoDeViagemEmMinutos.Valor).ToInt();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTempoDeViagemEmDias = (from obj in linha.Colunas where obj.NomeCampo == "TempoDeViagemEmDias" select obj).FirstOrDefault();
                        int tempoDeViagemEmDias = 0;
                        if (colTempoDeViagemEmDias != null)
                            tempoDeViagemEmDias = ((string)colTempoDeViagemEmDias.Valor).ToInt();

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colTransportador = (from obj in linha.Colunas where obj.NomeCampo == "Transportador" select obj).FirstOrDefault();
                        Dominio.Entidades.Empresa empresaExclusiva = null;
                        if (colTransportador != null)
                        {
                            empresaExclusiva = repEmpresa.BuscarPorCodigoIntegracao(colTransportador.Valor);
                            if (empresaExclusiva == null)
                                retorno = Localization.Resources.Logistica.RotaFrete.NaoFoiEncontradoTransportadorExclusivoBaseMultisoftware;
                        }

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCEPInicial = (from obj in linha.Colunas where obj.NomeCampo == "CEPInicial" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCEPFinal = (from obj in linha.Colunas where obj.NomeCampo == "CEPFinal" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCEPLeadTime = (from obj in linha.Colunas where obj.NomeCampo == "CEPLeadTime" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCEPADValorem = (from obj in linha.Colunas where obj.NomeCampo == "CEPADValorem" select obj).FirstOrDefault();

                        int? cepInicial = null, cepFinal = null, leadTime = null;
                        decimal? adValorem = null;

                        if (colCEPInicial != null)
                            cepInicial = Utilidades.String.OnlyNumbers((string)colCEPInicial.Valor).ToInt();

                        if (colCEPFinal != null)
                            cepFinal = Utilidades.String.OnlyNumbers((string)colCEPFinal.Valor).ToInt();

                        if (colCEPLeadTime != null)
                            leadTime = Utilidades.String.OnlyNumbers((string)colCEPLeadTime.Valor).ToInt();

                        if (colCEPADValorem != null)
                            adValorem = ((string)colCEPADValorem.Valor).ToDecimal();

                        List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana> diasEntrega = new List<DiaSemana>();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaSeg = (from obj in linha.Colunas where obj.NomeCampo == "Segunda" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaTer = (from obj in linha.Colunas where obj.NomeCampo == "Terca" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaQua = (from obj in linha.Colunas where obj.NomeCampo == "Quarta" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaQui = (from obj in linha.Colunas where obj.NomeCampo == "Quinta" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaSex = (from obj in linha.Colunas where obj.NomeCampo == "Sexta" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaSab = (from obj in linha.Colunas where obj.NomeCampo == "Sabado" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaDom = (from obj in linha.Colunas where obj.NomeCampo == "Domingo" select obj).FirstOrDefault();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCPFCNPJDestinatario = (from obj in linha.Colunas where obj.NomeCampo == "CPFCNPJDestinatario" select obj).FirstOrDefault();

                        List<Dominio.Entidades.Cliente> clienteExistentes = new List<Dominio.Entidades.Cliente>();

                        if (colCPFCNPJDestinatario?.Valor != null)
                        {
                            string codigosDestinatario = colCPFCNPJDestinatario.Valor;
                            string[] listaCodigosClientes = codigosDestinatario.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var codigoCliente in listaCodigosClientes)
                            {
                                double cpfCnpjDestinatario = 0;
                                Dominio.Entidades.Cliente cliente = null;

                                double.TryParse(Utilidades.String.OnlyNumbers(colCPFCNPJDestinatario.Valor), out cpfCnpjDestinatario);

                                cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);

                                cliente ??= repCliente.BuscarPorCodigoIntegracao(codigoCliente);

                                if (cliente == null)
                                    retorno = Localization.Resources.Logistica.RotaFrete.CodigoIntegracaoDestinatarioNaoLocalizadoMultisoftware;

                                clienteExistentes.Add(cliente);
                            }
                        }

                        if (colunaSeg != null && !string.IsNullOrWhiteSpace(colunaSeg.Valor))
                            diasEntrega.Add(DiaSemana.Segunda);
                        if (colunaTer != null && !string.IsNullOrWhiteSpace(colunaTer.Valor))
                            diasEntrega.Add(DiaSemana.Terca);
                        if (colunaQua != null && !string.IsNullOrWhiteSpace(colunaQua.Valor))
                            diasEntrega.Add(DiaSemana.Quarta);
                        if (colunaQui != null && !string.IsNullOrWhiteSpace(colunaQui.Valor))
                            diasEntrega.Add(DiaSemana.Quinta);
                        if (colunaSex != null && !string.IsNullOrWhiteSpace(colunaSex.Valor))
                            diasEntrega.Add(DiaSemana.Sexta);
                        if (colunaSab != null && !string.IsNullOrWhiteSpace(colunaSab.Valor))
                            diasEntrega.Add(DiaSemana.Sabado);
                        if (colunaDom != null && !string.IsNullOrWhiteSpace(colunaDom.Valor))
                            diasEntrega.Add(DiaSemana.Domingo);

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colunaClientesOrigem = linha.Colunas.Where(obj => obj.NomeCampo == "ClientesOrigem").FirstOrDefault();

                        PadraoTempoDiasMinutos? padraoTempoDiasMinutos = null;
                        int tempoDeViagemSalvar = 0;
                        if (tempoDeViagemEmMinutos > 0)
                        {
                            padraoTempoDiasMinutos = PadraoTempoDiasMinutos.Minutos;
                            tempoDeViagemSalvar = tempoDeViagemEmMinutos;
                        }
                        else if (tempoDeViagemEmDias > 0)
                        {
                            padraoTempoDiasMinutos = PadraoTempoDiasMinutos.Dias;
                            tempoDeViagemSalvar = tempoDeViagemEmDias * 1440;
                        }

                        if (string.IsNullOrWhiteSpace(retorno))
                        {
                            Dominio.Entidades.RotaFrete rotaFrete = null;

                            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                                rotaFrete = repRotaFrete.BuscarAtivaPorCodigoIntegracao(codigoIntegracao);

                            if (rotaFrete == null)
                            {
                                if (string.IsNullOrWhiteSpace(descricao))
                                    descricao = (remetente?.Localidade?.Descricao ?? "") + " - " + (destino?.Descricao ?? "");

                                rotaFrete = new Dominio.Entidades.RotaFrete
                                {
                                    TipoOperacaoPreCarga = tipoOperacaoPreCarga,
                                    TipoOperacao = tipoOperacao,
                                    Quilometros = distancia,
                                    Observacao = "",
                                    HoraInicioCarregamento = inicioCarregamento.ToNullableTime(),
                                    HoraLimiteSaidaCD = limiteSaida.ToNullableTime(),
                                    TempoCarregamento = tempoCarregamento.ToTime(),
                                    TempoDescarga = tempoDescarga.ToTime(),
                                    Detalhes = "",
                                    Ativo = true,
                                    Descricao = Utilidades.String.Left(descricao, 100),
                                    PermiteAgruparCargas = false,
                                    GrupoPessoas = null,
                                    Remetente = remetente,
                                    RegiaoDestino = regiaoDestino,
                                    Distribuidor = null,
                                    CodigoIntegracaoNOX = "",
                                    CodigoIntegracao = codigoIntegracao,
                                    FilialDistribuidora = "",
                                    TempoDeViagemEmMinutos = tempoDeViagemSalvar,
                                    PadraoTempo = padraoTempoDiasMinutos,
                                    VincularMotoristaFilaCarregamentoManualmente = false,
                                    PolilinhaRota = "",
                                    TipoUltimoPontoRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante,
                                    //PontosDaRota = pontosdarota,
                                    TipoRota = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete.Ida,
                                    VelocidadeMediaCarregado = 0,
                                    VelocidadeMediaVazio = 0,
                                    TipoCarregamentoIda = 0,
                                    TipoCarregamentoVolta = 0,
                                    CodigoIntegracaoValePedagio = codigoIntegracaoValePedagio,
                                    CodigoIntegracaoValePedagioRetorno = "",
                                };

                                rotaFrete.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando;
                                rotaFrete.RotaRoteirizada = false;
                                rotaFrete.RotaRoteirizadaPorLocal = true;

                                if (origem != null)
                                {
                                    rotaFrete.LocalidadesOrigem = new List<Dominio.Entidades.Localidade>();
                                    rotaFrete.LocalidadesOrigem.Add(origem);
                                }
                                else if (estadoOrigem != null)
                                {
                                    rotaFrete.EstadosOrigem = new List<Dominio.Entidades.Estado>();
                                    rotaFrete.EstadosOrigem.Add(estadoOrigem);

                                }

                                if (distancia > 0 && origem != null && destino != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(rotaFrete.PolilinhaRota))
                                    {
                                        if (new Servicos.Embarcador.Logistica.RestricaoRodagem(unitOfWork).IsPossuiRestricaoZonaExclusaoRota(rotaFrete.PolilinhaRota))
                                            rotaFrete.SituacaoDaRoteirizacao = SituacaoRoteirizacao.EmZonaExclusao;
                                        else
                                            rotaFrete.SituacaoDaRoteirizacao = SituacaoRoteirizacao.Concluido;
                                    }
                                    else
                                        rotaFrete.SituacaoDaRoteirizacao = SituacaoRoteirizacao.Concluido;
                                    rotaFrete.RotaRoteirizadaPorLocal = true;
                                }

                                repRotaFrete.Inserir(rotaFrete);
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, rotaFrete, "Adicionado via importação de planilha", unitOfWork);

                                foreach (var destinatario in clienteExistentes)
                                {
                                    Dominio.Entidades.RotaFreteDestinatarios novoDestinatario = new Dominio.Entidades.RotaFreteDestinatarios()
                                    {
                                        Cliente = destinatario,
                                        RotaFrete = rotaFrete
                                    };
                                    repositorioRotaFreteDestinatario.Inserir(novoDestinatario);
                                }

                                if (empresaExclusiva != null)
                                {
                                    Dominio.Entidades.RotaFreteEmpresaExclusiva rotaFreteEmpresaExclusiva = new Dominio.Entidades.RotaFreteEmpresaExclusiva();
                                    rotaFreteEmpresaExclusiva.RotaFrete = rotaFrete;
                                    rotaFreteEmpresaExclusiva.Empresa = empresaExclusiva;

                                    repRotaFreteEmpresaExclusiva.Inserir(rotaFreteEmpresaExclusiva);
                                }

                                if (destino != null)
                                {
                                    Dominio.Entidades.RotaFreteLocalidade rotaFreteLocalidade = new Dominio.Entidades.RotaFreteLocalidade()
                                    {
                                        Localidade = destino,
                                        RotaFrete = rotaFrete
                                    };

                                    repositorioRotaFreteLocalidade.Inserir(rotaFreteLocalidade);
                                }
                            }
                            else
                            {
                                rotaFrete.Initialize();

                                rotaFrete.TipoOperacaoPreCarga = tipoOperacaoPreCarga;
                                rotaFrete.HoraInicioCarregamento = inicioCarregamento.ToNullableTime();
                                rotaFrete.HoraLimiteSaidaCD = limiteSaida.ToNullableTime();
                                rotaFrete.TempoCarregamento = tempoCarregamento.ToTime();
                                rotaFrete.TempoDescarga = tempoDescarga.ToTime();
                                rotaFrete.TempoDeViagemEmMinutos = tempoDeViagemSalvar;
                                rotaFrete.PadraoTempo = padraoTempoDiasMinutos;
                                rotaFrete.Remetente = remetente;

                                List<Dominio.Entidades.RotaFreteDestinatarios> destinatariosDaRota = repositorioRotaFreteDestinatario.BuscarPorCodigoIntegracaoRotaFrete(rotaFrete.Codigo);

                                if (destinatariosDaRota == null && destinatariosDaRota.Count > 0)
                                {
                                    List<Dominio.Entidades.RotaFreteDestinatarios> destinatariosRemover = destinatariosDaRota.Where(x => !clienteExistentes.Contains(x.Cliente)).ToList();

                                    foreach (var itemRemover in destinatariosRemover)
                                        repositorioRotaFreteDestinatario.Deletar(itemRemover);
                                }

                                foreach (var novoDestinatario in clienteExistentes)
                                {
                                    var existeDestinatario = repositorioRotaFreteDestinatario.BuscarPorCPFCNPJCodigoRotaFrete(novoDestinatario.CPF_CNPJ, rotaFrete.Codigo);

                                    if (existeDestinatario != null)
                                        continue;

                                    existeDestinatario = new Dominio.Entidades.RotaFreteDestinatarios()
                                    {
                                        Cliente = novoDestinatario,
                                        RotaFrete = rotaFrete,
                                    };

                                    repositorioRotaFreteDestinatario.Inserir(existeDestinatario);
                                }

                                if ((destino != null) && !repositorioRotaFreteLocalidade.ExistePorRotaFrete(rotaFrete.Codigo, destino.Codigo))
                                {
                                    Dominio.Entidades.RotaFreteLocalidade rotaFreteLocalidade = new Dominio.Entidades.RotaFreteLocalidade()
                                    {
                                        Localidade = destino,
                                        RotaFrete = rotaFrete
                                    };

                                    repositorioRotaFreteLocalidade.Inserir(rotaFreteLocalidade);
                                }

                                if (!string.IsNullOrWhiteSpace(descricao))
                                    rotaFrete.Descricao = descricao;

                                repRotaFrete.Atualizar(rotaFrete);

                                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = rotaFrete.GetChanges();
                                if (alteracoes.Count > 0)
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, rotaFrete, rotaFrete.GetChanges(), "Atualizado via importação de planilha", unitOfWork);
                            }

                            if (rotaFrete.ClientesOrigem == null || rotaFrete.ClientesOrigem.Count == 0)
                            {
                                if (ObterClientes(out List<Dominio.Entidades.Cliente> clientes, out string mensagem, colunaClientesOrigem?.Valor, unitOfWork) && clientes?.Count > 0)
                                    rotaFrete.ClientesOrigem = clientes;
                                else
                                    retorno = mensagem;
                            }

                            if (cepInicial.HasValue && cepFinal.HasValue)
                            {
                                Repositorio.Embarcador.Logistica.RotaFreteCEP repRotaFreteCEP = new Repositorio.Embarcador.Logistica.RotaFreteCEP(unitOfWork);

                                Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP rotaFreteCEP = repRotaFreteCEP.BuscarPorCEP(rotaFrete, cepInicial.Value, cepFinal.Value);

                                if (rotaFreteCEP == null)
                                {
                                    rotaFreteCEP = new Dominio.Entidades.Embarcador.Logistica.RotaFreteCEP()
                                    {
                                        RotaFrete = rotaFrete
                                    };
                                }

                                rotaFreteCEP.CEPInicial = cepInicial.Value;
                                rotaFreteCEP.CEPFinal = cepFinal.Value;
                                rotaFreteCEP.LeadTime = leadTime;
                                rotaFreteCEP.PercentualADValorem = adValorem;

                                repRotaFreteCEP.Inserir(rotaFreteCEP);
                            }

                            if (diasEntrega.Count > 0)
                            {
                                repRotaFreteFrequenciaEntrega.DeletarPorRotaFrete(rotaFrete.Codigo);

                                if (diasEntrega.Count < 7)
                                {
                                    foreach (Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana diaSemana in diasEntrega)
                                    {
                                        Dominio.Entidades.RotaFreteFrequenciaEntrega rotaFreteFrequenciaEntrega = new Dominio.Entidades.RotaFreteFrequenciaEntrega
                                        {
                                            RotaFrete = rotaFrete,
                                            DiaSemana = diaSemana
                                        };

                                        repRotaFreteFrequenciaEntrega.Inserir(rotaFreteFrequenciaEntrega);
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(retorno))
                        {
                            unitOfWork.Rollback();
                            retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha(retorno, i));
                        }
                        else
                        {
                            contador++;
                            Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoSucesso(i, contar: false);
                            retornoImportacao.Retornolinhas.Add(retornoLinha);

                            unitOfWork.CommitChanges();
                        }
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha(Localization.Resources.Logistica.RotaFrete.OcorreuUmaFalhaProcessarLinha, i));
                    }
                }

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio repositorioConfiguracaoValePedagio = new Repositorio.Embarcador.Frotas.ConfiguracaoValePedagio(unitOfWork);

                return new JsonpResult(new
                {
                    TemIntegracaoRepomRest = repositorioConfiguracaoValePedagio.PossuiIntegracaoRepomRest()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.RotaFrete.OcorreuUmaFalhaObterConfiguracoesIntegracoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacaoShare()
        {

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoShare();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ImportarShare()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Servicos.Embarcador.Carga.RotaFrete servicoRotaFrete = new Servicos.Embarcador.Carga.RotaFrete(unitOfWork);

            Repositorio.RotaFreteEmpresa repositorioRotaFreteEmpresa = new Repositorio.RotaFreteEmpresa(unitOfWork);
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ObterConfiguracaoImportacaoShare();
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);
                List<Dominio.Entidades.RotaFreteEmpresa> rotaFreteEmpresas = new List<Dominio.Entidades.RotaFreteEmpresa>();

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao();
                retornoImportacao.Retornolinhas = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha>();
                int contador = 0;

                for (int i = 0; i < linhas.Count; i++)
                {
                    try
                    {
                        unitOfWork.FlushAndClear();
                        unitOfWork.Start();
                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha linha = linhas[i];

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoIntegracao = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracao" select obj).FirstOrDefault();
                        string codigoIntegracao = "";
                        if (colCodigoIntegracao != null)
                            codigoIntegracao = colCodigoIntegracao.Valor;
                        else
                            throw new ControllerException("Código de Integração da Rota Frete não informado, este campo é obrigatório.");

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoIntegracaoTransportador = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoTransportador" select obj).FirstOrDefault();
                        string codigoIntegracaoTransportador = "";
                        if (colCodigoIntegracaoTransportador != null)
                            codigoIntegracaoTransportador = colCodigoIntegracaoTransportador.Valor;
                        else
                            throw new ControllerException("Código de Integração do Transportador não informado, este campo é obrigatório.");

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colCodigoIntegracaoModeloVeicularCarga = (from obj in linha.Colunas where obj.NomeCampo == "CodigoIntegracaoModeloVeicularCarga" select obj).FirstOrDefault();
                        string codigoIntegracaoModeloVeicularCarga = "";
                        if (colCodigoIntegracaoModeloVeicularCarga != null)
                            codigoIntegracaoModeloVeicularCarga = colCodigoIntegracaoModeloVeicularCarga.Valor;
                        else
                            throw new ControllerException("Código de Integração do Modelo Veicular de Carga não informado, este campo é obrigatório.");

                        Dominio.ObjetosDeValor.Embarcador.Importacao.DadosColuna colPercentualCargas = (from obj in linha.Colunas where obj.NomeCampo == "PercentualCargas" select obj).FirstOrDefault();
                        decimal percentualCargas = 0;
                        if (colPercentualCargas != null)
                            percentualCargas = Convert.ToDecimal(colPercentualCargas.Valor);
                        else
                            throw new ControllerException("Percentual de Cargas não informado, este campo é obrigatório.");


                        Dominio.Entidades.RotaFrete rotaFrete = repositorioRotaFrete.BuscarPorCodigoIntegracao(codigoIntegracao);

                        if (rotaFrete == null)
                            throw new ControllerException("Rota de Frete não localizado pelo Código de Integração informado.");

                        Dominio.Entidades.Empresa transportador = repositorioEmpresa.BuscarPorCodigoIntegracao(codigoIntegracaoTransportador);

                        if (transportador == null)
                            throw new ControllerException("Empresa não localizado pelo Código de Integração informado.");

                        Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.buscarPorCodigoIntegracao(codigoIntegracaoModeloVeicularCarga);

                        if (modeloVeicularCarga == null)
                            throw new ControllerException("Modelo Veicular de Carga não localizado pelo Código de Integração informado.");

                        Dominio.Entidades.RotaFreteEmpresa rotaFreteEmpresa = servicoRotaFrete.ImportarShareRotaFrete(rotaFrete, transportador, modeloVeicularCarga, percentualCargas);

                        if (rotaFreteEmpresa != null)
                            rotaFreteEmpresas.Add(rotaFreteEmpresa);

                        contador++;
                        Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha retornoLinha = Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoSucesso(i, contar: false);
                        retornoImportacao.Retornolinhas.Add(retornoLinha);

                    }
                    catch (ControllerException ex)
                    {
                        unitOfWork.Rollback();
                        retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha(ex.Message, i));
                    }
                    catch (ServicoException ex2)
                    {
                        unitOfWork.Rollback();
                        retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha(ex2.Message, i));
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                        retornoImportacao.Retornolinhas.Add(Dominio.ObjetosDeValor.Embarcador.Importacao.RetonoLinha.CriarRetornoFalha(Localization.Resources.Logistica.RotaFrete.OcorreuUmaFalhaProcessarLinha, i));
                    }
                }

                if (rotaFreteEmpresas?.Count > 0)
                {
                    List<int> codigosRotasFrete = rotaFreteEmpresas.Select(o => o.RotaFrete.Codigo).Distinct().ToList();
                    List<int> codigosRotasFreteEmpresas = rotaFreteEmpresas.Select(o => o.Codigo).Distinct().ToList();

                    List<Dominio.Entidades.RotaFreteEmpresa> rotaFreteEmpresasResultantesForEach = new List<Dominio.Entidades.RotaFreteEmpresa>();

                    foreach (int codigoRotaFrete in codigosRotasFrete)
                    {
                        List<Dominio.Entidades.RotaFreteEmpresa> rotaFreteEmpresasBuscas = repositorioRotaFreteEmpresa.BuscarPorRotaFrete(codigoRotaFrete);

                        List<Dominio.Entidades.RotaFreteEmpresa> rotaFreteEmpresasDeletar = (from obj in rotaFreteEmpresasBuscas where !codigosRotasFreteEmpresas.Contains(obj.Codigo) select obj).ToList();

                        foreach (Dominio.Entidades.RotaFreteEmpresa rotaFreteEmpresaDeletar in rotaFreteEmpresasDeletar)
                        {
                            repositorioRotaFreteEmpresa.Deletar(rotaFreteEmpresaDeletar);
                        }

                    }

                }

                unitOfWork.CommitChanges();

                retornoImportacao.MensagemAviso = "";
                retornoImportacao.Total = linhas.Count();
                retornoImportacao.Importados = contador;

                return new JsonpResult(retornoImportacao);
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

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.RotaFrete rotaFrete = repRotaFrete.BuscarPorCodigo(codigo);

                if (rotaFrete == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                grid.setarQuantidadeTotal(rotaFrete.ArquivosTransacaoRotaFrete.Count());

                var retorno = (from obj in rotaFrete.ArquivosTransacaoRotaFrete.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
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
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoRotaFrete = Request.GetIntParam("CodigoRotaFrete");

                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);

                Dominio.Entidades.RotaFrete rotaFrete = repRotaFrete.BuscarPorCodigo(codigoRotaFrete);
                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = repRotaFrete.BuscarArquivoHistoricoPorCodigo(codigo);

                if (arquivoIntegracao == null || rotaFrete == null)
                    return new JsonpResult(false, true, "Histórico não encontrado.");

                if (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null)
                    return new JsonpResult(false, true, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", $"Arquivos Integração {rotaFrete.Descricao}.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o download dos arquivos de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherRotaFrete(Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Logistica.RotaFreteClassificacao repositorioRotaFreteClassificacao = new Repositorio.Embarcador.Logistica.RotaFreteClassificacao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unidadeDeTrabalho);
            Repositorio.Embarcador.Localidades.Regiao repRegiao = new Repositorio.Embarcador.Localidades.Regiao(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.CanalEntrega repositorioCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unidadeDeTrabalho);
            Repositorio.Embarcador.Pedidos.CanalVenda repositorioCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(unidadeDeTrabalho);

            bool.TryParse(Request.Params("Ativo"), out bool ativo);
            bool.TryParse(Request.Params("PermiteAgruparCargas"), out bool permiteAgruparCargas);

            int codigoGrupoPessoas, tempoDeViagemEmMinutos;
            int.TryParse(Request.Params("GrupoPessoas"), out codigoGrupoPessoas);

            PadraoTempoDiasMinutos? padraoTempo = Request.GetNullableEnumParam<PadraoTempoDiasMinutos>("PadraoTempo");

            if (padraoTempo.HasValue && padraoTempo.Value == PadraoTempoDiasMinutos.Dias)
                tempoDeViagemEmMinutos = Convert.ToInt32(TimeSpan.FromDays((double)Request.GetIntParam("TempoDeViagemEmDias")).TotalMinutes);
            else
            {
                TimeSpan tempoViagemMinutos = Request.GetTimeParam("TempoDeViagemEmMinutos");
                tempoDeViagemEmMinutos = Convert.ToInt32(Math.Ceiling(tempoViagemMinutos.TotalMinutes));
            }

            double cpfCnpjRemetente = Request.GetDoubleParam("Remetente");
            double cpfCnpjDistribuidor = Request.GetDoubleParam("Distribuidor");
            double cpfCnpjExpedidorPedidosDiferenteOrigemRota = Request.GetDoubleParam("ExpedidorPedidosDiferenteOrigemRota");
            string descricao = Request.Params("Descricao");
            string filialDistribuidora = Request.Params("FilialDistribuidora");
            string codigoIntegracao = Request.Params("CodigoIntegracao");
            string codigoIntegracaoNOX = Request.Params("CodigoIntegracaoNOX");
            string observacao = Request.Params("Observacao");
            string detalhes = Request.Params("Detalhes");
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            int codigoRegiaoDestino = Request.GetIntParam("RegiaoDestino");
            int codigoCanalVenda = Request.GetIntParam("CanalVenda");
            int codigoCanalEntrega = Request.GetIntParam("CanalEntrega");
            int codigoTipoOperacaoPreCarga = Request.GetIntParam("TipoOperacaoPreCarga");
            string codigoIntegracaoValePedagio = Request.Params("CodigoIntegracaoValePedagio");
            string codigoIntegracaoValePedagioRetorno = Request.Params("CodigoIntegracaoValePedagioRetorno");
            int codigoClassificacao = Request.GetIntParam("Classificacao");
            int codigoOutroEndereco = Request.GetIntParam("RemetenteOutroEndereco");

            TimeSpan? horaInicioCarregamento = Request.GetNullableTimeParam("HoraInicioCarregamento");
            TimeSpan? horaLimiteSaidaCD = Request.GetNullableTimeParam("HoraLimiteSaidaCD");

            decimal quilometros = Request.GetDecimalParam("Quilometros");

            int velocidadeMediaCarregado = Request.GetIntParam("VelocidadeMediaCarregado");
            int velocidadeMediaVazio = Request.GetIntParam("VelocidadeMediaVazio");

            string codigoIntegracaoGerenciadoraRisco = Request.GetStringParam("CodigoIntegracaoGerenciadoraRisco");

            TipoRotaFrete tipoRota = Request.GetEnumParam<TipoRotaFrete>("TipoRota", TipoRotaFrete.Ida);
            RetornoCargaTipo tipoCarregamentoIda = Request.GetEnumParam<RetornoCargaTipo>("TipoCarregamentoIda", RetornoCargaTipo.Vazio);
            RetornoCargaTipo tipoCarregamentoVolta = Request.GetEnumParam<RetornoCargaTipo>("TipoCarregamentoVolta", RetornoCargaTipo.Vazio);

            rotaFrete.CodigoIntegracaoNOX = codigoIntegracaoNOX;
            rotaFrete.Detalhes = detalhes;
            rotaFrete.Observacao = observacao;
            rotaFrete.HoraInicioCarregamento = horaInicioCarregamento;
            rotaFrete.HoraLimiteSaidaCD = horaLimiteSaidaCD;
            rotaFrete.TempoCarregamento = RetornarTimeSpan(Request.GetStringParam("TempoCarregamento"));//Request.GetNullableTimeParam("TempoCarregamento");
            rotaFrete.TempoDescarga = RetornarTimeSpan(Request.GetStringParam("TempoDescarga"));//Request.GetNullableTimeParam("TempoDescarga");
            rotaFrete.Quilometros = quilometros;
            rotaFrete.VelocidadeMediaCarregado = velocidadeMediaCarregado;
            rotaFrete.VelocidadeMediaVazio = velocidadeMediaVazio;
            rotaFrete.Ativo = ativo;
            rotaFrete.Descricao = descricao;
            rotaFrete.PermiteAgruparCargas = permiteAgruparCargas;
            rotaFrete.Remetente = cpfCnpjRemetente > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjRemetente) : null;
            rotaFrete.RegiaoDestino = codigoRegiaoDestino > 0D ? repRegiao.BuscarPorCodigo(codigoRegiaoDestino) : null;
            rotaFrete.CanalEntrega = codigoCanalEntrega > 0D ? repositorioCanalEntrega.BuscarPorCodigo(codigoCanalEntrega) : null;
            rotaFrete.CanalVenda = codigoCanalVenda > 0D ? repositorioCanalVenda.BuscarPorCodigo(codigoCanalVenda) : null;
            rotaFrete.Distribuidor = cpfCnpjDistribuidor > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjDistribuidor) : null;
            rotaFrete.ExpedidorPedidosDiferenteOrigemRota = cpfCnpjExpedidorPedidosDiferenteOrigemRota > 0d ? repCliente.BuscarPorCPFCNPJ(cpfCnpjExpedidorPedidosDiferenteOrigemRota) : null;
            rotaFrete.GrupoPessoas = codigoGrupoPessoas > 0 ? repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas) : null;
            rotaFrete.CodigoIntegracao = codigoIntegracao;
            rotaFrete.FilialDistribuidora = filialDistribuidora;
            rotaFrete.VincularMotoristaFilaCarregamentoManualmente = Request.GetBoolParam("VincularMotoristaFilaCarregamentoManualmente");
            rotaFrete.GerarRedespachoAutomaticamente = Request.GetBoolParam("GerarRedespachoAutomaticamente");
            rotaFrete.PolilinhaRota = Request.GetStringParam("PolilinhaRota");
            rotaFrete.TipoUltimoPontoRoteirizacao = Request.GetEnumParam<TipoUltimoPontoRoteirizacao>("TipoUltimoPontoRoteirizacao");
            rotaFrete.TipoUltimoPontoRoteirizacaoPorEstado = Request.GetNullableEnumParam<TipoUltimoPontoRoteirizacao>("TipoUltimoPontoRoteirizacaoPorEstado");
            rotaFrete.TipoOperacao = codigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;
            rotaFrete.TipoOperacaoPreCarga = codigoTipoOperacaoPreCarga > 0 ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacaoPreCarga) : null;
            rotaFrete.TipoRota = tipoRota;
            rotaFrete.TipoCarregamentoIda = tipoCarregamentoIda;
            rotaFrete.TipoCarregamentoVolta = tipoCarregamentoVolta;
            rotaFrete.CodigoIntegracaoValePedagio = codigoIntegracaoValePedagio;
            rotaFrete.CodigoIntegracaoValePedagioRetorno = codigoIntegracaoValePedagioRetorno;
            rotaFrete.CodigoIntegracaoGerenciadoraRisco = codigoIntegracaoGerenciadoraRisco;
            rotaFrete.Classificacao = (codigoClassificacao > 0) ? repositorioRotaFreteClassificacao.BuscarPorCodigo(codigoClassificacao, auditavel: false) : null;
            rotaFrete.PadraoTempo = padraoTempo;
            rotaFrete.RemetenteOutroEndereco = codigoOutroEndereco > 0 ? repClienteOutroEndereco.BuscarPorCodigo(codigoOutroEndereco) : null;
            rotaFrete.DataConsultaPracasPedido = rotaFrete?.DataConsultaPracasPedido == null ? DateTime.Now : rotaFrete?.DataConsultaPracasPedido;

            if (!string.IsNullOrWhiteSpace(rotaFrete.PolilinhaRota))
            {
                if (new Servicos.Embarcador.Logistica.RestricaoRodagem(unidadeDeTrabalho).IsPossuiRestricaoZonaExclusaoRota(rotaFrete.PolilinhaRota))
                    rotaFrete.SituacaoDaRoteirizacao = SituacaoRoteirizacao.EmZonaExclusao;
                else
                {
                    rotaFrete.SituacaoDaRoteirizacao = SituacaoRoteirizacao.Concluido;
                    rotaFrete.RotaRoteirizada = true;
                }
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    rotaFrete.ApenasObterPracasPedagio = true;
            }
            else
            {
                rotaFrete.SituacaoDaRoteirizacao = SituacaoRoteirizacao.SemDefinicao;
                rotaFrete.RotaRoteirizada = false;
            }

            rotaFrete.TempoDeViagemEmMinutos = tempoDeViagemEmMinutos;
            rotaFrete.RotaExclusivaCompraValePedagio = Request.GetBoolParam("RotaExclusivaCompraValePedagio");
            rotaFrete.UtilizarDistanciaRotaCarga = Request.GetBoolParam("UtilizarDistanciaRotaCarga");
            rotaFrete.VoltarPeloMesmoCaminhoIda = Request.GetBoolParam("VoltarPeloMesmoCaminhoIda");
            rotaFrete.ValidarParaQualquerDestinatarioInformado = Request.GetBoolParam("ValidarParaQualquerDestinatarioInformado");

            SetarRestricoesEntrega(ref rotaFrete, unidadeDeTrabalho);
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRotaFrete ObterFiltrosPesquisa(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRotaFrete filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRotaFrete()
            {
                Descricao = Request.GetStringParam("Descricao"),
                FilialDistribuidora = Request.GetStringParam("FilialDistribuidora"),
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Todos),
                Remetente = Request.GetDoubleParam("Remetente"),
                CodigoOrigem = Request.GetIntParam("Origem"),
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                RotaExclusivaCompraValePedagio = Request.GetNullableBoolParam("RotaExclusivaCompraValePedagio"),
                CEPDestino = Request.GetIntParam("CEPDestino"),
                SituacaoRoteirizacao = Request.GetEnumParam<SituacaoRoteirizacao>("SituacaoRoteirizacao", SituacaoRoteirizacao.Todas)
            };

            int codigoGrupoPessoas = Request.GetIntParam("GrupoPessoas");
            int codCarga = Request.GetIntParam("Carga");
            double codigoPessoa = Request.GetDoubleParam("Pessoa");
            int codigoTabelaFrete = Request.GetIntParam("TabelaFrete");
            List<int> codigoPedidos = Request.GetListParam<int>("pedidos");
            bool somenteGrupo = Request.GetBoolParam("SomenteGrupo");
            double codigoDestinatario = Request.GetDoubleParam("Destinatario");

            if (codigoDestinatario > 0)
                filtrosPesquisa.CodigosDestinatario.Add(codigoDestinatario);

            if (codigoPessoa > 0D && codigoGrupoPessoas == 0)
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(codigoPessoa);

                if (cliente.GrupoPessoas != null)
                    codigoGrupoPessoas = cliente.GrupoPessoas.Codigo;
            }

            if (codigoTabelaFrete > 0)
            {
                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);

                codigoGrupoPessoas = tabelaFrete?.GrupoPessoas?.Codigo ?? 0;
            }

            if (codigoPedidos?.Count > 0)
            {
                List<int> codigosCidadeRemetentes = new List<int>();
                List<int> codigosCidadeDestinatarios = new List<int>();

                Repositorio.Embarcador.Pedidos.Pedido repPedidos = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repPedidos.BuscarPorCodigos(codigoPedidos);

                codigosCidadeRemetentes.AddRange((from pedido in pedidos where pedido.Remetente?.Localidade != null && (!pedido.UsarOutroEnderecoOrigem || pedido.EnderecoOrigem?.ClienteOutroEndereco == null) select pedido.Remetente.Localidade.Codigo).Distinct().ToList());
                codigosCidadeRemetentes.AddRange((from pedido in pedidos where pedido.Expedidor?.Localidade != null select pedido.Expedidor.Localidade.Codigo).Distinct().ToList());
                codigosCidadeRemetentes.AddRange((from pedido in pedidos where pedido.UsarOutroEnderecoOrigem && pedido.EnderecoOrigem?.ClienteOutroEndereco != null && (pedido.Expedidor == null || pedido.Expedidor.CPF_CNPJ == pedido.EnderecoOrigem.ClienteOutroEndereco.Cliente.CPF_CNPJ) select pedido.EnderecoOrigem.Localidade.Codigo).Distinct().ToList());

                codigosCidadeDestinatarios.AddRange((from pedido in pedidos where pedido.Destinatario?.Localidade != null && (!pedido.UsarOutroEnderecoDestino || pedido.EnderecoDestino?.ClienteOutroEndereco == null) select pedido.Destinatario.Localidade.Codigo).Distinct().ToList());
                codigosCidadeDestinatarios.AddRange((from pedido in pedidos where pedido.Recebedor?.Localidade != null select pedido.Recebedor.Localidade.Codigo).Distinct().ToList());
                codigosCidadeDestinatarios.AddRange((from pedido in pedidos where pedido.UsarOutroEnderecoDestino && pedido.EnderecoDestino?.ClienteOutroEndereco != null && (pedido.Recebedor == null || pedido.Recebedor.CPF_CNPJ == pedido.EnderecoDestino.ClienteOutroEndereco.Cliente.CPF_CNPJ) select pedido.EnderecoDestino.Localidade.Codigo).Distinct().ToList());

                filtrosPesquisa.CodigosCidadeDestinatario = codigosCidadeDestinatarios.Distinct().ToList();
                filtrosPesquisa.CodigosCidadeRemetente = codigosCidadeRemetentes.Distinct().ToList();

                if (filtrosPesquisa.CodigosDestinatario.Count == 0)
                    filtrosPesquisa.CodigosDestinatario.AddRange(pedidos.Where(pedido => pedido.Destinatario != null).Select(pedido => pedido.Destinatario.CPF_CNPJ).Distinct());
            }

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                if (codCarga > 0)
                {
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);

                    codigoGrupoPessoas = carga.GrupoPessoaPrincipal?.Codigo ?? carga.Pedidos.FirstOrDefault().ObterTomador().GrupoPessoas?.Codigo ?? 0;
                }
            }
            else
                somenteGrupo = false;

            filtrosPesquisa.CodigoGrupoPessoas = codigoGrupoPessoas;
            filtrosPesquisa.SomenteGrupo = somenteGrupo;

            return filtrosPesquisa;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacaoRota()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Logistica.RotaFrete.CodigoIntegracao, Propriedade = "CodigoIntegracao", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Logistica.RotaFrete.CPFCNPJRemetente, Propriedade = "CNPJCPFRemetente", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 30, Descricao = Localization.Resources.Logistica.RotaFrete.CPFCNPJDestinatario, Propriedade = "CPFCNPJDestinatario", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Logistica.RotaFrete.LocalidadeDestino, Propriedade = "LocalidadeDestino", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = Localization.Resources.Logistica.RotaFrete.LocalidadeOrigem, Propriedade = "LocalidadeOrigem", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Logistica.RotaFrete.UFDestino, Propriedade = "UFDestino", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = Localization.Resources.Logistica.RotaFrete.UFOrigem, Propriedade = "UFOrigem", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Gerais.Geral.Descricao, Propriedade = "Descricao", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Logistica.RotaFrete.TipoOperacaoPreCarga, Propriedade = "TipoOperacaoPreCarga", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = Localization.Resources.Logistica.RotaFrete.InicioCarregamento, Propriedade = "InicioCarregamento", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = Localization.Resources.Logistica.RotaFrete.LimiteSaida, Propriedade = "LimiteSaida", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = Localization.Resources.Logistica.RotaFrete.Distancia, Propriedade = "Distancia", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = Localization.Resources.Logistica.RotaFrete.TipoOperacao, Propriedade = "TipoOperacao", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = Localization.Resources.Logistica.RotaFrete.TempoViagemMinutos, Propriedade = "TempoDeViagemEmMinutos", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 14, Descricao = Localization.Resources.Logistica.RotaFrete.TempoViagemDias, Propriedade = "TempoDeViagemEmDias", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 15, Descricao = Localization.Resources.Logistica.RotaFrete.Transportador, Propriedade = "Transportador", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 16, Descricao = Localization.Resources.Logistica.RotaFrete.RegiaoDestino, Propriedade = "RegiaoDestino", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 17, Descricao = Localization.Resources.Logistica.RotaFrete.ClientesOrigem, Propriedade = "ClientesOrigem", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 18, Descricao = Localization.Resources.Logistica.RotaFrete.EntregaSegunda, Propriedade = "Segunda", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 19, Descricao = Localization.Resources.Logistica.RotaFrete.EntregaTerca, Propriedade = "Terca", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 20, Descricao = Localization.Resources.Logistica.RotaFrete.EntregaQuarta, Propriedade = "Quarta", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 21, Descricao = Localization.Resources.Logistica.RotaFrete.EntregaQuinta, Propriedade = "Quinta", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 22, Descricao = Localization.Resources.Logistica.RotaFrete.EntregaSexta, Propriedade = "Sexta", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 23, Descricao = Localization.Resources.Logistica.RotaFrete.EntregaSabado, Propriedade = "Sabado", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 24, Descricao = Localization.Resources.Logistica.RotaFrete.EntregaDomingo, Propriedade = "Domingo", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 25, Descricao = Localization.Resources.Logistica.RotaFrete.CEPInicial, Propriedade = "CEPInicial", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 26, Descricao = Localization.Resources.Logistica.RotaFrete.CEPFinal, Propriedade = "CEPFinal", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 27, Descricao = Localization.Resources.Logistica.RotaFrete.LeadTime, Propriedade = "CEPLeadTime", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 28, Descricao = Localization.Resources.Logistica.RotaFrete.ADValorem, Propriedade = "CEPADValorem", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 29, Descricao = Localization.Resources.Logistica.RotaFrete.CodigoRotaValePedagio, Propriedade = "CodigoIntegracaoValePedagio", Tamanho = 200, Obrigatorio = false, Regras = new List<string> { } });

            return configuracoes;
        }

        private bool ObterClientes(out List<Dominio.Entidades.Cliente> clientes, out string mensagem, string valores, Repositorio.UnitOfWork unitOfWork)
        {
            clientes = new List<Dominio.Entidades.Cliente>();
            mensagem = null;

            if (string.IsNullOrWhiteSpace(valores))
                return true;

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            IEnumerable<string> cpfCnpjClientes = valores.Replace(" ", "").Split('/').Distinct();

            foreach (string cpfCnpjCliente in cpfCnpjClientes)
            {
                double cpfCnpj = Utilidades.String.OnlyNumbers(cpfCnpjCliente).ToDouble();

                if (cpfCnpj <= 0D)
                {
                    mensagem = $" {Localization.Resources.Logistica.RotaFrete.NaoEPossivelConverterCPFCNPJ} {cpfCnpjCliente}";
                    return false;
                }

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                if (cliente == null)
                {
                    mensagem = $" {Localization.Resources.Logistica.RotaFrete.ClienteNaoEncontradoCPJCNPJ} {cpfCnpjCliente}";
                    return false;
                }

                if (!clientes.Contains(cliente))
                    clientes.Add(cliente);
            }

            return true;
        }

        private string ObterRotaFreteSerializada(Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            string pontosRota = "";
            Repositorio.RotaFretePontosPassagem repRotaFretePontosPassagem = new Repositorio.RotaFretePontosPassagem(unitOfWork);
            List<Dominio.Entidades.RotaFretePontosPassagem> pontosPassagem = repRotaFretePontosPassagem.BuscarPorRotaFrete(rotaFrete.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontosDaRota = new List<PontosDaRota>();
            foreach (Dominio.Entidades.RotaFretePontosPassagem pontoPassagem in pontosPassagem)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota pontoRota = new PontosDaRota();

                pontoRota.descricao = pontoPassagem?.Descricao ?? string.Empty;
                if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem)
                {
                    pontoRota.codigo = pontoPassagem.Codigo;
                    pontoRota.pontopassagem = true;
                }
                else if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Pedagio)
                {
                    pontoRota.codigo = pontoPassagem.PracaPedagio.Codigo;
                    pontoRota.pontopassagem = true;
                }
                else
                    pontoRota.codigo = pontoPassagem.Cliente?.CPF_CNPJ ?? Convert.ToDouble(0);

                pontoRota.lat = (double)pontoPassagem.Latitude;
                pontoRota.lng = (double)pontoPassagem.Longitude;
                pontoRota.distancia = pontoPassagem.Distancia;
                pontoRota.tempo = pontoPassagem.Tempo;
                pontoRota.tempoEstimadoPermanencia = pontoPassagem.TempoEstimadoPermanenencia;
                pontoRota.tipoponto = pontoPassagem.TipoPontoPassagem;
                pontosDaRota.Add(pontoRota);
            }
            pontosRota = Newtonsoft.Json.JsonConvert.SerializeObject(pontosDaRota);

            return pontosRota;

        }

        private void SetarEstadosDestino(ref Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            dynamic estados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Estados"));

            if (rotaFrete.Estados == null)
                rotaFrete.Estados = new List<Dominio.Entidades.Estado>();
            else
                rotaFrete.Estados.Clear();

            foreach (var objEstado in estados)
            {
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla((string)objEstado.Codigo);
                rotaFrete.Estados.Add(estado);
            }

            if (rotaFrete.Estados.Count > 0)
                rotaFrete.RotaRoteirizadaPorLocal = true;
        }

        private void SetarEstadosOrigem(ref Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            dynamic estados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("EstadosOrigem"));

            if (rotaFrete.EstadosOrigem == null)
                rotaFrete.EstadosOrigem = new List<Dominio.Entidades.Estado>();
            else
                rotaFrete.EstadosOrigem.Clear();

            foreach (var objEstado in estados)
            {
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla((string)objEstado.Codigo);
                rotaFrete.EstadosOrigem.Add(estado);
            }

            if (rotaFrete.EstadosOrigem.Count > 0)
                rotaFrete.RotaRoteirizadaPorLocal = true;
        }

        private void SetarLocalidades(ref Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.RotaFreteLocalidade repositorioRotaFreteLocalidade = new Repositorio.RotaFreteLocalidade(unitOfWork);
            dynamic localidades = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Localidades"));
            bool ordenar = Request.GetBoolParam("OrdenarLocalidades");

            repositorioRotaFreteLocalidade.DeletarPorRotaFrete(rotaFrete.Codigo);

            foreach (var localidadeAdicionar in localidades)
            {
                Dominio.Entidades.Localidade localidade = repositorioLocalidade.BuscarPorCodigo(((string)localidadeAdicionar.Codigo).ToInt()) ?? throw new ControllerException("Localidade não encontrada.");
                Dominio.Entidades.RotaFreteLocalidade rotaFreteLocalidade = new Dominio.Entidades.RotaFreteLocalidade()
                {
                    Localidade = localidade,
                    RotaFrete = rotaFrete,
                    Ordem = ordenar ? ((string)localidadeAdicionar.Ordem).ToInt() : 0
                };

                repositorioRotaFreteLocalidade.Inserir(rotaFreteLocalidade);

                rotaFrete.RotaRoteirizadaPorLocal = true;
            }
        }

        private void SetarLocalidadesOrigem(ref Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            int codigoLocalidadeOrigem = Request.GetIntParam("LocalidadeOrigem");
            if (codigoLocalidadeOrigem == 0 && (rotaFrete.LocalidadesOrigem?.Count ?? 0) > 0)
                rotaFrete.LocalidadesOrigem.Clear();

            if (codigoLocalidadeOrigem == 0)
                return;

            if (rotaFrete.LocalidadesOrigem == null)
                rotaFrete.LocalidadesOrigem = new List<Dominio.Entidades.Localidade>();
            else
                rotaFrete.LocalidadesOrigem.Clear();

            rotaFrete.LocalidadesOrigem.Add(repLocalidade.BuscarPorCodigo(codigoLocalidadeOrigem));
        }

        private void SetarFiliais(ref Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.RotaFreteFiliais repRotaFreteFilial = new Repositorio.RotaFreteFiliais(unitOfWork);

            dynamic filiais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Filiais"));

            repRotaFreteFilial.DeletarPorRotaFrete(rotaFrete.Codigo);

            foreach (var filial in filiais)
            {
                int codigoFilial = ((string)filial.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Filiais.Filial filialSalvar = null;

                if (codigoFilial > 0d)
                {
                    filialSalvar = repFilial.BuscarPorCodigo(codigoFilial);

                    Dominio.Entidades.RotaFreteFiliais rotaFreteFilial = new Dominio.Entidades.RotaFreteFiliais()
                    {
                        Filial = filialSalvar,
                        RotaFrete = rotaFrete,
                    };
                    repRotaFreteFilial.Inserir(rotaFreteFilial);
                }

            }

        }

        private void SetarTiposCarga(ref Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.RotaFreteTiposCarga repRotaFreteTiposCarga = new Repositorio.RotaFreteTiposCarga(unitOfWork);

            dynamic tiposCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TipoCargas"));

            repRotaFreteTiposCarga.DeletarPorRotaFrete(rotaFrete.Codigo);

            foreach (var tipoCarga in tiposCarga)
            {
                int codigoTipoCarga = ((string)tipoCarga.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCargaSalvar = null;

                if (codigoTipoCarga > 0d)
                {
                    tipoCargaSalvar = repTipoDeCarga.BuscarPorCodigo(codigoTipoCarga);

                    Dominio.Entidades.RotaFreteTiposCarga rotaFreteFilial = new Dominio.Entidades.RotaFreteTiposCarga()
                    {
                        TipoDeCarga = tipoCargaSalvar,
                        RotaFrete = rotaFrete,
                    };
                    repRotaFreteTiposCarga.Inserir(rotaFreteFilial);
                }

            }

        }

        private void SetarDestinatarios(ref Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.RotaFreteDestinatarios repRotaFreteDestinatarios = new Repositorio.RotaFreteDestinatarios(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);

            dynamic destinatarios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Destinatarios"));

            repRotaFreteDestinatarios.DeletarPorRotaFrete(rotaFrete.Codigo);

            bool possuiDestinatarios = false;
            foreach (var destinatario in destinatarios)
            {
                double cpfCnpjDestinatario = ((string)destinatario.Codigo).ToDouble();
                int ordem = ((string)destinatario.Ordem).ToInt();
                int codOutroEndereco = ((string)destinatario.CodigoOutroEndereco).ToInt();

                Dominio.Entidades.Cliente destinatarioSalvar = null;

                if (cpfCnpjDestinatario > 0d)
                    destinatarioSalvar = repCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);
                else
                    destinatarioSalvar = repCliente.BuscarPorRazaoExterior((string)destinatario.Nome, (string)destinatario.Endereco);

                Dominio.Entidades.RotaFreteDestinatarios rotaFreteDestinatario = new Dominio.Entidades.RotaFreteDestinatarios()
                {
                    Cliente = destinatarioSalvar,
                    RotaFrete = rotaFrete,
                    Ordem = ordem,
                    ClienteOutroEndereco = codOutroEndereco > 0 ? repClienteOutroEndereco.BuscarPorCodigo(codOutroEndereco) : null
                };

                repRotaFreteDestinatarios.Inserir(rotaFreteDestinatario);
                possuiDestinatarios = true;
            }

            if (possuiDestinatarios)
                rotaFrete.RotaRoteirizadaPorLocal = false;
        }

        private void SetarFronteiras(ref Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.RotaFreteFronteira repRotaFreteFronteira = new Repositorio.RotaFreteFronteira(unitOfWork);

            dynamic fronteiras = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Fronteiras"));

            repRotaFreteFronteira.DeletarPorRotaFrete(rotaFrete.Codigo);

            bool possuiFronteiras = false;
            foreach (var fronteira in fronteiras)
            {
                double cpfCnpjDestinatario = ((string)fronteira.Codigo).ToDouble();
                int ordem = ((string)fronteira.Ordem).ToInt();
                var tempoMedioPermanencia = ((string)fronteira.TempoMedioPermanenciaFronteira);

                int tempoMedioPermanenciaFronteira = 0;
                if (!string.IsNullOrEmpty(tempoMedioPermanencia))
                {
                    var horaEMinuto = tempoMedioPermanencia.Split(':');
                    if (horaEMinuto.Count() == 2)
                    {
                        tempoMedioPermanenciaFronteira = int.Parse(horaEMinuto[0]) * 60 + int.Parse(horaEMinuto[1]);
                    }
                }

                Dominio.Entidades.Cliente destinatarioSalvar = null;

                if (cpfCnpjDestinatario > 0d)
                    destinatarioSalvar = repCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);
                else
                    destinatarioSalvar = repCliente.BuscarPorRazaoExterior((string)fronteira.Nome, (string)fronteira.Endereco);

                Dominio.Entidades.RotaFreteFronteira rotaFreteFronteira = new Dominio.Entidades.RotaFreteFronteira()
                {
                    Cliente = destinatarioSalvar,
                    RotaFrete = rotaFrete,
                    Ordem = ordem,
                    TempoMedioPermanenciaFronteira = tempoMedioPermanenciaFronteira
                };
                repRotaFreteFronteira.Inserir(rotaFreteFronteira);
                possuiFronteiras = true;
            }

            if (possuiFronteiras)
                rotaFrete.RotaRoteirizadaPorLocal = false;
        }

        private void SetarColetas(ref Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            dynamic coletas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Coletas"));

            if (rotaFrete.Coletas == null)
                rotaFrete.Coletas = new List<Dominio.Entidades.Cliente>();
            else
                rotaFrete.Coletas.Clear();

            foreach (var coleta in coletas)
                rotaFrete.Coletas.Add(repCliente.BuscarPorCPFCNPJ((double)coleta.Codigo));
        }

        private void SetarPontosPassagemPreDefinidos(ref Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PontoPassagemPreDefinido repPontoPassagemPreDefinido = new Repositorio.Embarcador.Logistica.PontoPassagemPreDefinido(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            dynamic pontosDePassagem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PontosPassagemPreDefinido"));

            repPontoPassagemPreDefinido.DeletarPorRotaFrete(rotaFrete.Codigo);

            foreach (dynamic pontoPassagem in pontosDePassagem)
            {
                bool localParqueamento = false;

                if ((string)pontoPassagem.LocalDeParqueamento == "Sim")
                    localParqueamento = true;

                Dominio.Entidades.Embarcador.Logistica.PontoPassagemPreDefinido novoPonto = new Dominio.Entidades.Embarcador.Logistica.PontoPassagemPreDefinido()
                {
                    RotaFrete = rotaFrete,
                    TempoEstimadoPermanencia = ((string)pontoPassagem.TempoEstimadoPermanencia).ToNullableMinutes() ?? 0,
                    LocalDeParqueamento = localParqueamento
                };

                double cpfCnpjCliente = ((string)pontoPassagem.CodigoCliente).ToDouble();
                int codigoLocalidade = ((string)pontoPassagem.CodigoLocalidade).ToInt();

                if (cpfCnpjCliente > 0d)
                {
                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpjCliente);
                    novoPonto.Cliente = cliente;
                    novoPonto.Descricao = cliente.Descricao;
                    novoPonto.Latitude = cliente.Latitude.ToDecimal();
                    novoPonto.Longitude = cliente.Longitude.ToDecimal();
                }
                else if (codigoLocalidade > 0)
                {
                    Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigo(codigoLocalidade);
                    novoPonto.Localidade = localidade;
                    novoPonto.Descricao = localidade.Descricao;
                    novoPonto.Latitude = localidade.Latitude ?? 0m;
                    novoPonto.Longitude = localidade.Longitude ?? 0m;
                }
                else
                {
                    // Se não tem um cliente, pega dos dados arbitrários que vêm. Essa forma não deveria ser usada, mas pontos de passagens antigos ainda usam.
                    novoPonto.Descricao = pontoPassagem.Descricao;
                    novoPonto.Latitude = ((string)pontoPassagem.Latitude).ToDecimal();
                    novoPonto.Longitude = ((string)pontoPassagem.Longitude).ToDecimal();
                }

                repPontoPassagemPreDefinido.Inserir(novoPonto);
            }
        }

        private void SetarPostosFiscais(ref Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PostoFiscalRotaFrete repPostoFiscal = new Repositorio.Embarcador.Logistica.PostoFiscalRotaFrete(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            dynamic postosFiscais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PostosFiscais"));

            if (rotaFrete.PostosFiscais?.Any() ?? false)
            {
                int tempoReduzir = rotaFrete.PostosFiscais.Sum(x => x.TempoEstimadoPermanencia);
                if (rotaFrete.PadraoTempo == PadraoTempoDiasMinutos.Minutos && rotaFrete.TempoDeViagemEmMinutos >= tempoReduzir)
                    rotaFrete.TempoDeViagemEmMinutos -= tempoReduzir;
            }

            repPostoFiscal.DeletarPorRotaFrete(rotaFrete.Codigo);



            foreach (dynamic pontoPassagem in postosFiscais)
            {
                Dominio.Entidades.Embarcador.Logistica.PostoFiscalRotaFrete postoFiscal = new Dominio.Entidades.Embarcador.Logistica.PostoFiscalRotaFrete()
                {
                    RotaFrete = rotaFrete,
                    TempoEstimadoPermanencia = ((string)pontoPassagem.TempoEstimadoPermanencia).ToNullableMinutes() ?? 0,
                };

                double cpfCnpjCliente = ((string)pontoPassagem.CodigoCliente).ToDouble();

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpjCliente);

                if (cliente == null)
                    throw new ControllerException("Os dados dos postos fiscais não podem ficar vazios");

                postoFiscal.Cliente = cliente;

                repPostoFiscal.Inserir(postoFiscal);

                if (rotaFrete.PadraoTempo == PadraoTempoDiasMinutos.Minutos)
                    rotaFrete.TempoDeViagemEmMinutos += postoFiscal.TempoEstimadoPermanencia;
            }
        }

        private void SetarRestricoesEntrega(ref Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.RotaFreteFrequenciaEntrega repositorioRotaFreteFrequenciaEntrega = new Repositorio.RotaFreteFrequenciaEntrega(unitOfWork);
            List<Dominio.Entidades.RotaFreteFrequenciaEntrega> rotaFreteFrequenciaEntrega = repositorioRotaFreteFrequenciaEntrega.BuscarPorRotaFrete(rotaFrete.Codigo);

            List<DiaSemana> diasSemanaRestricaoEntrega = Request.GetListEnumParam<DiaSemana>("RestricaoEntrega");

            foreach (Dominio.Entidades.RotaFreteFrequenciaEntrega frequenciaEntrega in rotaFreteFrequenciaEntrega)
            {
                if (diasSemanaRestricaoEntrega.Contains(frequenciaEntrega.DiaSemana))
                    diasSemanaRestricaoEntrega.Remove(frequenciaEntrega.DiaSemana);
                else
                    repositorioRotaFreteFrequenciaEntrega.Deletar(frequenciaEntrega);
            }

            foreach (DiaSemana diaSemana in diasSemanaRestricaoEntrega)
            {
                Dominio.Entidades.RotaFreteFrequenciaEntrega restricaoEntrega = new Dominio.Entidades.RotaFreteFrequenciaEntrega();
                restricaoEntrega.RotaFrete = rotaFrete;
                restricaoEntrega.DiaSemana = diaSemana;
                repositorioRotaFreteFrequenciaEntrega.Inserir(restricaoEntrega);
            }
        }

        private void ValidarCodigoIntegracaoDuplicado(Repositorio.UnitOfWork unitOfWork, string codigoIntegracao)
        {
            ValidarCodigoIntegracaoDuplicado(unitOfWork, codigoIntegracao, codigoRotaFrete: 0);
        }

        private void ValidarCodigoIntegracaoDuplicado(Repositorio.UnitOfWork unitOfWork, string codigoIntegracao, int codigoRotaFrete)
        {
            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
            {
                Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
                Dominio.Entidades.RotaFrete rotaFrete = repositorioRotaFrete.BuscarPorCodigoIntegracaoDuplicado(codigoRotaFrete, codigoIntegracao);

                if (rotaFrete != null)
                    throw new ControllerException($" {Localization.Resources.Logistica.RotaFrete.CodigoIntegracaoJaInformado} {rotaFrete.Descricao}");
            }
        }

        private void ValidarDiasRestricaoInformados(Dominio.Entidades.RotaFreteRestricao rotaFreteRestricao)
        {
            bool isExisteDiaInformado = (rotaFreteRestricao.Segunda || rotaFreteRestricao.Terca || rotaFreteRestricao.Quarta || rotaFreteRestricao.Quinta || rotaFreteRestricao.Sexta || rotaFreteRestricao.Sabado || rotaFreteRestricao.Domingo);

            if (!isExisteDiaInformado)
                throw new Dominio.Excecoes.Embarcador.ControllerException(Localization.Resources.Logistica.RotaFrete.NenhumDiaDeRestricaoFoiInformado);
        }

        private TimeSpan RetornarTimeSpan(string strTempo)
        {
            if (strTempo != string.Empty)
            {
                string[] HrMin = strTempo.Split(':');
                double hr = HrMin[0].ToDouble();
                double min = HrMin[1].ToDouble();
                TimeSpan tempo = TimeSpan.FromHours(hr) + TimeSpan.FromMinutes(min);

                return tempo;
            }
            else
                return TimeSpan.Zero;
        }

        private void SetarVeiculos(ref Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.RotaFreteVeiculo repositorioRotaFreteVeiculo = new Repositorio.RotaFreteVeiculo(unitOfWork);

            dynamic dynVeiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Veiculos"));

            List<Dominio.Entidades.RotaFreteVeiculo> rotaFreteVeiculos = repositorioRotaFreteVeiculo.BuscarPorRotaFrete(rotaFrete.Codigo);

            List<int> codigos = new List<int>();
            foreach (var dynVeiculo in dynVeiculos)
                if (dynVeiculo.Codigo != null)
                    codigos.Add((int)dynVeiculo.Codigo);

            rotaFrete.PossuiVeiculosInformados = codigos.Count > 0 ? true : false;

            if (rotaFreteVeiculos.Count > 0)
            {
                List<Dominio.Entidades.RotaFreteVeiculo> rotaFreteVeiculosDeletar = (from obj in rotaFreteVeiculos where !codigos.Contains(obj.Veiculo.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.RotaFreteVeiculo rotaFreteVeiculoDeletar in rotaFreteVeiculosDeletar)
                {
                    repositorioRotaFreteVeiculo.Deletar(rotaFreteVeiculoDeletar);
                    rotaFreteVeiculos.Remove(rotaFreteVeiculoDeletar);
                }
            }
            else
                rotaFreteVeiculos = new List<Dominio.Entidades.RotaFreteVeiculo>();


            foreach (Dominio.Entidades.RotaFreteVeiculo rotaFreteVeiculo in rotaFreteVeiculos)
                if (codigos.Contains(rotaFreteVeiculo.Veiculo.Codigo))
                    codigos.Remove(rotaFreteVeiculo.Veiculo.Codigo);


            foreach (int codiogVeiculo in codigos)
            {
                Dominio.Entidades.Veiculo veiculo = new Dominio.Entidades.Veiculo();

                veiculo = repositorioVeiculo.BuscarPorCodigo(codiogVeiculo);

                Dominio.Entidades.RotaFreteVeiculo rotaFreteVeiculo = new Dominio.Entidades.RotaFreteVeiculo()
                {
                    RotaFrete = rotaFrete,
                    Veiculo = veiculo,
                };

                repositorioRotaFreteVeiculo.Inserir(rotaFreteVeiculo);
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ObterConfiguracaoImportacaoShare()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Logistica.RotaFrete.CodigoIntegracao, Propriedade = "CodigoIntegracao", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Código Integração Transportador", Propriedade = "CodigoIntegracaoTransportador", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "Código Integração Modelo Veicular de Carga", Propriedade = "CodigoIntegracaoModeloVeicularCarga", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Percentual de Cargas", Propriedade = "PercentualCargas", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { } });

            return configuracoes;
        }

        private dynamic RetornarObjetoPesquisa(Dominio.Entidades.RotaFrete p)
        {
            TimeSpan tempoEmMinutos = TimeSpan.FromMinutes(p.TempoDeViagemEmMinutos);

            return new
            {
                p.Codigo,
                p.Descricao,
                Origem = p.Remetente?.Descricao ?? "",
                Destino = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? string.Join(", ", (from d in p.Destinatarios select d.Cliente.Descricao)) : string.Empty),
                p.FilialDistribuidora,
                Quilometros = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? p.Quilometros.ToString("n0") : p.Quilometros.ToString("n2")),
                TempoDeViagemEmMinutos = p.PadraoTempo == PadraoTempoDiasMinutos.Dias && p.TempoDeViagemEmMinutos > 0 ? (p.TempoDeViagemEmMinutos / 1440).ToString() : p.PadraoTempo == PadraoTempoDiasMinutos.Minutos ? $"{(int)tempoEmMinutos.TotalHours:D2}:{tempoEmMinutos.Minutes:D2}" : "00:00",
                PadraoTempo = p.PadraoTempo == PadraoTempoDiasMinutos.Dias ? Localization.Resources.Consultas.RotaFrete.Dias : p.PadraoTempo == PadraoTempoDiasMinutos.Minutos ? Localization.Resources.Consultas.RotaFrete.Minutos : string.Empty,
                GrupoPessoa = p.GrupoPessoas?.Descricao,
                p.DescricaoAtivo,
                TipoOperacao = p.TipoOperacao?.Descricao ?? "",
                CodigoIntegracao = p.CodigoIntegracao ?? "",
                Fronteiras = JsonConvert.SerializeObject(from o in p.Fronteiras
                                                         select new
                                                         {
                                                             o.Cliente.Codigo,
                                                             Descricao = o.Cliente.Descricao,
                                                         })
            };
        }

        private void ValidarDestinatariosLocalidades()
        {
            string destinatariosJson = Request.Params("Destinatarios");
            string localidadesJson = Request.Params("Localidades");

            bool temDestinatarios = false;
            bool temLocalidades = false;

            if (!string.IsNullOrEmpty(destinatariosJson))
            {
                var destinatarios = JsonConvert.DeserializeObject<JToken>(destinatariosJson);
                if (destinatarios != null && destinatarios is JArray jArrayDestinatarios)
                {
                    temDestinatarios = jArrayDestinatarios.Count > 0;
                }
            }

            if (!string.IsNullOrEmpty(localidadesJson))
            {
                var localidades = JsonConvert.DeserializeObject<JToken>(localidadesJson);
                if (localidades != null && localidades is JArray jArrayLocalidades)
                {
                    temLocalidades = jArrayLocalidades.Count > 0;
                }
            }

            if (temDestinatarios && temLocalidades)
                throw new ControllerException("Não é possível definir destinatários e localidades na mesma Rota de Frete.");

        }

        #endregion

        #region Métodos Privados - Empresas

        private void AtualizarEmpresas(Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic empresas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Empresas"));

            Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador repositorioConfiguracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = repositorioConfiguracaoTransportador.BuscarConfiguracaoPadrao();

            ExcluirEmpresasRemovidas(rotaFrete, empresas, configuracaoTransportador, unidadeDeTrabalho);
            SalvarEmpresasAdicionadasOuAtualizadas(rotaFrete, empresas, configuracaoTransportador, unidadeDeTrabalho);
        }

        private void ExcluirEmpresasRemovidas(Dominio.Entidades.RotaFrete rotaFrete, dynamic empresas, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (rotaFrete.Empresas != null)
            {
                Repositorio.RotaFreteEmpresa repositorioRotaFreteEmpresa = new Repositorio.RotaFreteEmpresa(unidadeDeTrabalho);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var empresa in empresas)
                {
                    int? codigo = ((string)empresa.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.RotaFreteEmpresa> listaEmpresasRemover = (from rescricao in rotaFrete.Empresas where !listaCodigosAtualizados.Contains(rescricao.Codigo) select rescricao).ToList();

                foreach (var empresa in listaEmpresasRemover)
                {
                    repositorioRotaFreteEmpresa.Deletar(empresa);
                }

                if (listaEmpresasRemover.Count > 0)
                {
                    string descricaoAcao = listaEmpresasRemover.Count == 1 ? Localization.Resources.Logistica.RotaFrete.TransportadorRemovido : Localization.Resources.Logistica.RotaFrete.MultiplosTransportadoresRemovidos;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, rotaFrete, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);

                    if (configuracaoTransportador.NotificarTransportadorProcessoShareRotas)
                        Task.Factory.StartNew(() => EnviarEmailTransportador(listaEmpresasRemover, TipoAcaoEmail.Excluir, unidadeDeTrabalho.StringConexao));
                }
            }
        }

        private dynamic ObterEmpresas(Dominio.Entidades.RotaFrete rotaFrete, bool duplicar)
        {
            return (
                from empresa in rotaFrete.Empresas
                select new
                {
                    Codigo = duplicar ? 0 : empresa.Codigo,
                    CodigoEmpresa = empresa.Empresa.Codigo,
                    CodigoModeloVeicularCarga = empresa.ModeloVeicularCarga?.Codigo ?? 0,
                    DescricaoEmpresa = empresa.Empresa.Descricao,
                    DescricaoModeloVeicularCarga = empresa.ModeloVeicularCarga?.Descricao ?? "",
                    PercentualCargasDaRota = empresa.PercentualCargasDaRota.ToString("n2"),
                    Prioridade = empresa.Prioridade.ToString("n0")
                }
            ).ToList();
        }

        private void SalvarEmpresasAdicionadasOuAtualizadas(Dominio.Entidades.RotaFrete rotaFrete, dynamic empresas, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unidadeDeTrabalho);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.RotaFreteEmpresa repositorioRotaFreteEmpresa = new Repositorio.RotaFreteEmpresa(unidadeDeTrabalho);

            int totalRegistrosAdicionados = 0;
            int totalRegistrosAtualizados = 0;

            Dominio.Entidades.RotaFreteEmpresa rotaFreteEmpresa = null;
            List<Dominio.Entidades.RotaFreteEmpresa> transportadoresAtualizados = new List<Dominio.Entidades.RotaFreteEmpresa>();
            List<Dominio.Entidades.RotaFreteEmpresa> transportadoresAdicionados = new List<Dominio.Entidades.RotaFreteEmpresa>();

            foreach (var empresa in empresas)
            {
                int codigoModeloVeicularCarga = ((string)empresa.CodigoModeloVeicularCarga).ToInt();
                int? codigo = ((string)empresa.Codigo).ToNullableInt();

                if (codigo.HasValue)
                {
                    rotaFreteEmpresa = repositorioRotaFreteEmpresa.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new Dominio.Excecoes.Embarcador.ControllerException("Transportador da rota de frete não encontrado");
                    rotaFreteEmpresa.Initialize();
                }
                else
                    rotaFreteEmpresa = new Dominio.Entidades.RotaFreteEmpresa() { RotaFrete = rotaFrete };

                rotaFreteEmpresa.Empresa = repositorioEmpresa.BuscarPorCodigo(((string)empresa.CodigoEmpresa).ToInt());
                rotaFreteEmpresa.ModeloVeicularCarga = (codigoModeloVeicularCarga > 0) ? repositorioModeloVeicularCarga.BuscarPorCodigo(codigoModeloVeicularCarga) : null;
                rotaFreteEmpresa.PercentualCargasDaRota = ((string)empresa.PercentualCargasDaRota).ToDecimal();
                rotaFreteEmpresa.Prioridade = ((string)empresa.Prioridade).ToInt();

                if (codigo.HasValue)
                {
                    if (rotaFreteEmpresa.GetChanges().Count > 0)
                    {
                        totalRegistrosAtualizados += 1;
                        transportadoresAtualizados.Add(rotaFreteEmpresa);
                    }

                    repositorioRotaFreteEmpresa.Atualizar(rotaFreteEmpresa);
                }
                else
                {
                    totalRegistrosAdicionados += 1;
                    transportadoresAdicionados.Add(rotaFreteEmpresa);
                    repositorioRotaFreteEmpresa.Inserir(rotaFreteEmpresa);
                }
            }

            if (rotaFrete.IsInitialized())
            {
                if (totalRegistrosAtualizados > 0)
                {
                    string descricaoAcao = totalRegistrosAtualizados == 1 ? Localization.Resources.Logistica.RotaFrete.TransportadorAtualizado : Localization.Resources.Logistica.RotaFrete.MultiplosTransportadoresAtualizados;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, rotaFrete, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);

                    if (configuracaoTransportador.NotificarTransportadorProcessoShareRotas)
                        Task.Factory.StartNew(() => EnviarEmailTransportador(transportadoresAtualizados, TipoAcaoEmail.Atualizar, unidadeDeTrabalho.StringConexao));
                }

                if (totalRegistrosAdicionados > 0)
                {
                    string descricaoAcao = totalRegistrosAdicionados == 1 ? Localization.Resources.Logistica.RotaFrete.TransportadorAdicionado : Localization.Resources.Logistica.RotaFrete.MultiplosTransportadoresAdicionados;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, rotaFrete, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);

                    if (configuracaoTransportador.NotificarTransportadorProcessoShareRotas)
                        Task.Factory.StartNew(() => EnviarEmailTransportador(transportadoresAdicionados, TipoAcaoEmail.Adicionar, unidadeDeTrabalho.StringConexao));
                }
            }
        }

        #endregion Métodos Privados - Empresas

        #region Métodos Privados - Empresas Exclusivas

        private void EnviarEmailTransportador(List<Dominio.Entidades.RotaFreteEmpresa> transportadores, TipoAcaoEmail tipoAcao, string stringConexao)
        {
            using (Repositorio.UnitOfWork _unitOfWork = new Repositorio.UnitOfWork(stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                if (transportadores.Any())
                {
                    Repositorio.RotaFreteEmpresa repositorioRotaFreteEmpresa = new Repositorio.RotaFreteEmpresa(_unitOfWork);
                    List<int> rotaFreteEmpresaCodigos = transportadores.Select(t => t.Codigo).ToList();
                    List<Dominio.Entidades.RotaFreteEmpresa> rotaFreteEmpresas = repositorioRotaFreteEmpresa.BuscarPorCodigos(rotaFreteEmpresaCodigos, auditavel: false);
                    
                    if (rotaFreteEmpresas == null || !rotaFreteEmpresas.Any())
                        return;

                    foreach (Dominio.Entidades.RotaFreteEmpresa rotaFreteEmpresa in rotaFreteEmpresas)
                    {
                        Dominio.Entidades.RotaFrete rotaFrete = rotaFreteEmpresa.RotaFrete;

                        string assunto = tipoAcao switch
                        {
                            TipoAcaoEmail.Atualizar => "Alteração de Cadastro – Atualização de Transportador",
                            TipoAcaoEmail.Adicionar => $"Inclusão de Transportador - Rota {rotaFrete.Descricao}",
                            TipoAcaoEmail.Excluir => $"Remoção de Transportador - Rota {rotaFrete.Descricao}",
                            _ => throw new ArgumentException("Tipo de ação de email inválido.")
                        };

                        StringBuilder mensagem = new StringBuilder();
                        mensagem.AppendLine($"Prezado(a), {rotaFreteEmpresa.Empresa.NomeFantasia}");
                        mensagem.AppendLine();

                        switch (tipoAcao)
                        {
                            case TipoAcaoEmail.Atualizar:
                                mensagem.AppendLine($"Informamos que houve uma atualização no percentual de participação (share) da sua empresa na rota {rotaFrete.Descricao}.");
                                mensagem.AppendLine($"- Percentual atual: {rotaFreteEmpresa.PercentualCargasDaRota}.");
                                break;

                            case TipoAcaoEmail.Adicionar:
                                mensagem.AppendLine($"Informamos que sua empresa foi incluída na rota {rotaFrete.Descricao} com o seguinte percentual de participação (share):");
                                mensagem.AppendLine("- Percentual anterior: 0%");
                                mensagem.AppendLine($"- Percentual atual: {rotaFreteEmpresa.PercentualCargasDaRota}");
                                break;

                            case TipoAcaoEmail.Excluir:
                                mensagem.AppendLine($"Informamos que sua empresa foi removida da rota {rotaFrete.Descricao}. O seu percentual de participação (share) foi ajustado para:");
                                mensagem.AppendLine("- Percentual atual: 0%");
                                break;

                            default:
                                throw new ArgumentException("Tipo de ação de email inválido.");
                        }

                        mensagem.AppendLine();
                        mensagem.AppendLine("Estamos à disposição para qualquer esclarecimento.");
                        mensagem.AppendLine();
                        mensagem.AppendLine("Atenciosamente, MultiTMS.");
                        mensagem.AppendLine();
                        mensagem.AppendLine("Envio de e-mail automático. Favor não responder.");

                        EnviarEmailTransportadores(assunto, mensagem, rotaFreteEmpresa.Empresa.Email, _unitOfWork);
                    }
                }
            }
        }

        private void EnviarEmailTransportadores(string assunto, StringBuilder mensagem, string emailEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte emailConfig = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);

            if (emailConfig == null)
                throw new Exception("Configuração de e-mail não encontrada.");

            string mensagemErro = "";
            bool sucesso = Servicos.Email.EnviarEmail(emailConfig.Email, emailConfig.Email, emailConfig.Senha, emailEmpresa, null, null, assunto, mensagem.ToString(), emailConfig.Smtp, out mensagemErro, emailConfig.DisplayEmail, null, "", emailConfig.RequerAutenticacaoSmtp, "", emailConfig.PortaSmtp);
            if (!sucesso)
                throw new Exception($"Erro ao enviar e-mail: {mensagemErro}");
        }

        private void AtualizarEmpresasExclusivas(Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.RotaFreteEmpresaExclusiva repositorioRotaFreteEmpresaExclusiva = new Repositorio.RotaFreteEmpresaExclusiva(unitOfWork);
            dynamic empresasExclusivas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("EmpresasExclusivas"));

            repositorioRotaFreteEmpresaExclusiva.DeletarPorRotaFrete(rotaFrete.Codigo);

            rotaFrete.PossuiTransportadoresExclusivos = false;

            foreach (var empresaExclusiva in empresasExclusivas)
            {
                Dominio.Entidades.RotaFreteEmpresaExclusiva empresaExclusivaSalvar = new Dominio.Entidades.RotaFreteEmpresaExclusiva()
                {
                    Empresa = repositorioEmpresa.BuscarPorCodigo(((string)empresaExclusiva.Codigo).ToInt()) ?? throw new ControllerException(Localization.Resources.Logistica.RotaFrete.NaoFoiPossivelEncontrarTransportadorExclusivo),
                    RotaFrete = rotaFrete
                };

                repositorioRotaFreteEmpresaExclusiva.Inserir(empresaExclusivaSalvar);

                rotaFrete.PossuiTransportadoresExclusivos = true;
            }
        }

        private dynamic ObterEmpresasExclusivas(Dominio.Entidades.RotaFrete rotaFrete, bool duplicar, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.RotaFreteEmpresaExclusiva repositorioRotaFreteEmpresaExclusiva = new Repositorio.RotaFreteEmpresaExclusiva(unitOfWork);
            List<Dominio.Entidades.RotaFreteEmpresaExclusiva> empresasExclusivas = repositorioRotaFreteEmpresaExclusiva.BuscarPorRotaFrete(rotaFrete.Codigo);

            return (
                from o in empresasExclusivas
                select new
                {
                    o.Empresa.Codigo,
                    o.Empresa.Descricao
                }
            ).ToList();
        }

        #endregion Métodos Privados - Empresas Exclusivas

        #region Métodos Privados - Praças de Pedágio

        private dynamic ObterPracasPedagio(Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaPedagio, EixosSuspenso sentido, int ordem)
        {
            dynamic item = new
            {
                pracaPedagio.CodigoIntegracao,
                pracaPedagio.Codigo,
                pracaPedagio.Descricao,
                KM = pracaPedagio.KM.ToString("n2"),
                pracaPedagio.Rodovia,
                pracaPedagio.DescricaoAtivo,
                pracaPedagio.Latitude,
                pracaPedagio.Longitude,
                Sentido = EixosSuspensoHelper.ObterDescricao(sentido),
                Ordem = ordem
            };
            return item;
        }

        private void SetarPracasDePedagio(ref Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            //Repositorio.Embarcador.Logistica.PracaPedagio repPracaPedagio = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork);

            dynamic pracaPedagios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PracaPedagios"));

            //if (rotaFrete.PracasPedagio == null)
            //    rotaFrete.PracasPedagio = new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();
            //else
            //    rotaFrete.PracasPedagio.Clear();

            //foreach (var pracaPedagio in pracaPedagios)
            //    rotaFrete.PracasPedagio.Add(repPracaPedagio.BuscarPorCodigo((int)pracaPedagio.Codigo));

            Repositorio.Embarcador.Logistica.PracaPedagio repPraca = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork);
            Repositorio.RotaFretePracaPedagio repRotaFretePracas = new Repositorio.RotaFretePracaPedagio(unitOfWork);

            repRotaFretePracas.DeletarPorRotaFrete(rotaFrete.Codigo);

            foreach (var praca in pracaPedagios)
            {
                int codigoPraca = ((string)praca.Codigo).ToInt();
                EixosSuspenso eixo = EixosSuspenso.Nenhum;
                if (praca.Sentido == EixosSuspensoHelper.ObterDescricao(EixosSuspenso.Ida))
                    eixo = EixosSuspenso.Ida;
                else if (praca.Sentido == EixosSuspensoHelper.ObterDescricao(EixosSuspenso.Volta))
                    eixo = EixosSuspenso.Volta;

                Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaPedagio = repPraca.BuscarPorCodigo(codigoPraca);

                Dominio.Entidades.RotaFretePracaPedagio rotaFretePracas = new Dominio.Entidades.RotaFretePracaPedagio()
                {
                    PracaPedagio = pracaPedagio,
                    RotaFrete = rotaFrete,
                    EixosSuspenso = eixo
                };

                repRotaFretePracas.Inserir(rotaFretePracas);
            }
        }

        #endregion Métodos Privados - Praças de Pedágio

        #region Métodos Privados - Restrições

        private void AtualizarRestricoes(Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            dynamic restricoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Restricoes"));

            ExcluirRestricoesRemovidas(rotaFrete, restricoes, unidadeDeTrabalho);
            SalvarRestricoesAdicionadasOuAtualizadas(rotaFrete, restricoes, unidadeDeTrabalho);
        }

        private void ExcluirRestricoesRemovidas(Dominio.Entidades.RotaFrete rotaFrete, dynamic restricoes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (rotaFrete.Restricoes != null)
            {
                Repositorio.RotaFreteRestricao repositorioRotaFreteRestricao = new Repositorio.RotaFreteRestricao(unidadeDeTrabalho);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var restricao in restricoes)
                {
                    int? codigo = ((string)restricao.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.RotaFreteRestricao> listaRestricoesRemover = (from rescricao in rotaFrete.Restricoes where !listaCodigosAtualizados.Contains(rescricao.Codigo) select rescricao).ToList();

                foreach (var restricao in listaRestricoesRemover)
                {
                    repositorioRotaFreteRestricao.Deletar(restricao);
                }

                if (listaRestricoesRemover.Count > 0)
                {
                    string descricaoAcao = listaRestricoesRemover.Count == 1 ? Localization.Resources.Logistica.RotaFrete.RestricaoRemovida : Localization.Resources.Logistica.RotaFrete.MultiplasRestricoesRemovidas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, rotaFrete, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private dynamic ObterRestricoes(Dominio.Entidades.RotaFrete rotaFrete, bool duplicar)
        {
            return (
                from restricao in rotaFrete.Restricoes
                select new
                {
                    Codigo = duplicar ? 0 : restricao.Codigo,
                    CodigoTipoCarga = restricao.TipoDeCarga?.Codigo,
                    CodigoModeloVeicular = restricao.ModeloVeicularCarga?.Codigo,
                    DescricaoDias = restricao.ObterDescricaoDias(),
                    DescricaoTipoCarga = restricao.TipoDeCarga?.Descricao,
                    DescricaoModeloVeicular = restricao.ModeloVeicularCarga?.Descricao,
                    HoraInicio = restricao.HoraInicio.ToString("hh':'mm"),
                    HoraTermino = restricao.HoraTermino.ToString("hh':'mm"),
                    restricao.Segunda,
                    restricao.Terca,
                    restricao.Quarta,
                    restricao.Quinta,
                    restricao.Sexta,
                    restricao.Sabado,
                    restricao.Domingo
                }
            ).ToList();
        }

        private void PreencherDadosRestricao(Dominio.Entidades.RotaFreteRestricao rotaFreteRestricao, dynamic restricao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repositorioModeloVeicularCarga.BuscarPorCodigo(((string)restricao.CodigoModeloVeicular).ToInt()) ?? null;
            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repositorioTipoCarga.BuscarPorCodigo(((string)restricao.CodigoTipoCarga).ToInt()) ?? null;
            TimeSpan horaInicio = ((string)restricao.HoraInicio).ToNullableTime() ?? throw new Dominio.Excecoes.Embarcador.ControllerException(Localization.Resources.Logistica.RotaFrete.HoraInicioRestricaoNaoInformada);
            TimeSpan horaTermino = ((string)restricao.HoraTermino).ToNullableTime() ?? throw new Dominio.Excecoes.Embarcador.ControllerException(Localization.Resources.Logistica.RotaFrete.HoraTerminoRestricaoNaoInformada);

            rotaFreteRestricao.HoraInicio = horaInicio;
            rotaFreteRestricao.HoraTermino = horaTermino;
            rotaFreteRestricao.ModeloVeicularCarga = modeloVeicularCarga;
            rotaFreteRestricao.TipoDeCarga = tipoCarga;
            rotaFreteRestricao.Segunda = ((string)restricao.Segunda).ToBool();
            rotaFreteRestricao.Terca = ((string)restricao.Terca).ToBool();
            rotaFreteRestricao.Quarta = ((string)restricao.Quarta).ToBool();
            rotaFreteRestricao.Quinta = ((string)restricao.Quinta).ToBool();
            rotaFreteRestricao.Sexta = ((string)restricao.Sexta).ToBool();
            rotaFreteRestricao.Sabado = ((string)restricao.Sabado).ToBool();
            rotaFreteRestricao.Domingo = ((string)restricao.Domingo).ToBool();

            ValidarDiasRestricaoInformados(rotaFreteRestricao);
        }

        private void SalvarRestricoesAdicionadasOuAtualizadas(Dominio.Entidades.RotaFrete rotaFrete, dynamic restricoes, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.RotaFreteRestricao repositorioRotaFreteRestricao = new Repositorio.RotaFreteRestricao(unidadeDeTrabalho);
            int totalRegistrosAdicionados = 0;
            int totalRegistrosAtualizados = 0;

            foreach (var restricao in restricoes)
            {
                Dominio.Entidades.RotaFreteRestricao rotaFreteRestricao;
                int? codigo = ((string)restricao.Codigo).ToNullableInt();

                if (codigo.HasValue)
                    rotaFreteRestricao = repositorioRotaFreteRestricao.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new Dominio.Excecoes.Embarcador.ControllerException("Restrição não encontrada");
                else
                    rotaFreteRestricao = new Dominio.Entidades.RotaFreteRestricao() { RotaFrete = rotaFrete };

                PreencherDadosRestricao(rotaFreteRestricao, restricao, unidadeDeTrabalho);

                if (codigo.HasValue)
                {
                    totalRegistrosAtualizados += rotaFreteRestricao.GetChanges().Count > 0 ? 1 : 0;
                    repositorioRotaFreteRestricao.Atualizar(rotaFreteRestricao);
                }
                else
                {
                    totalRegistrosAdicionados += 1;
                    repositorioRotaFreteRestricao.Inserir(rotaFreteRestricao);
                }
            }

            if (rotaFrete.IsInitialized())
            {
                if (totalRegistrosAtualizados > 0)
                {
                    string descricaoAcao = totalRegistrosAtualizados == 1 ? Localization.Resources.Logistica.RotaFrete.RestricaoAtualizada : Localization.Resources.Logistica.RotaFrete.MultiplasRestricoesAtualizadas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, rotaFrete, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }

                if (totalRegistrosAdicionados > 0)
                {
                    string descricaoAcao = totalRegistrosAdicionados == 1 ? Localization.Resources.Logistica.RotaFrete.RestricaoAdicionada : Localization.Resources.Logistica.RotaFrete.MultiplasRestricoesAdicionadas;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, rotaFrete, null, descricaoAcao, unidadeDeTrabalho, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        #endregion Métodos Privados - Restrições
    }
}
