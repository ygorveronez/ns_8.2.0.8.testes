using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.TipoCarga
{
    [CustomAuthorize(new string[] { "BuscarIntegracoes" }, "Cargas/TipoCarga", "Cargas/FaixaTemperatura")]
    public class TipoCargaController : BaseController
    {
        #region Construtores

        public TipoCargaController(Conexao conexao) : base(conexao) { }

        #endregion Construtores

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                int codigoTipoCarga = Request.GetIntParam("CodigoTipoCarga");
                int codigoTipoOperacaoEmissao = Request.GetIntParam("TipoOperacaoEmissao");

                string descricao = Request.Params("Descricao");
                string codigoTipoCargaEmbarcador = Request.Params("CodigoTipoCargaEmbarcador");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                int codigoGrupoPessoas = 0;
                int.TryParse(Request.Params("GrupoPessoas"), out codigoGrupoPessoas);

                double pessoa;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out pessoa);

                Dominio.Entidades.Cliente cliente = null;
                if (pessoa > 0)
                    cliente = repCliente.BuscarPorCPFCNPJ(pessoa);

                int codCarga;
                int.TryParse(Request.Params("Carga"), out codCarga);

                long codigoDestinatario = 0;
                long.TryParse(Request.Params("Destinatario"), out codigoDestinatario);
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = null;

                if (codigoDestinatario > 0)
                    centroDescarregamento = repositorioCentroDescarregamento.BuscarPorDestinatario(codigoDestinatario);

                List<int> codigosFornecedor = new List<int>();
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedor = Usuario.ClienteFornecedor != null ? repModalidadeFornecedorPessoas.BuscarPorCliente(Usuario.ClienteFornecedor.CPF_CNPJ) : null;
                    codigosFornecedor = modalidadeFornecedor?.TipoCargas.Select(o => o.Codigo).ToList() ?? null;
                }

                if (codigoTipoCarga > 0)
                {
                    if (codigosFornecedor.Count > 0)
                        codigosFornecedor = new List<int>() { codigoTipoCarga };
                    else
                        codigosFornecedor.Add(codigoTipoCarga);
                }

                if (codCarga > 0)
                {
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codCarga);
                    cliente = carga.Pedidos.FirstOrDefault().ObterTomador();
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("PermiteInformarRecebedor", false);
                grid.AdicionarCabecalho("ExigirQueCDDestinoSejaInformadoAgendamento", false);
                grid.AdicionarCabecalho("FretePorContaDoCliente", false);
                grid.AdicionarCabecalho("Paletizado", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.CodigoIntegracao, "CodigoTipoCargaEmbarcador", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoCarga.TipoPrincipal, "TipoCargaPrincipal", 30, Models.Grid.Align.left, true);

                if (configuracaoGeralCarga.UsarPrioridadeDaCargaParaImpressaoDeObservacaoNoCTE)
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.TipoCarga.PrioridadeCarga, "PrioridadeCarga", 30, Models.Grid.Align.left, true);

                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                List<int> codigosTipoCarga = null;

                if (Request.GetBoolParam("FiltrarPorConfiguracaoOperadorLogistica", valorPadrao: true))
                    codigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);

                bool.TryParse(Request.Params("FiltrarSomenteDisponiveisMontagemCarga"), out bool filtrarSomenteDispMontagemCarga);
                bool.TryParse(Request.Params("FiltrarTiposPrincipais"), out bool filtrarTiposPrincipais);

                List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCarga = null;
                int quantidadeTotal = 0;
                if (centroDescarregamento == null)
                {
                    bool isFornecedor = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor;
                    tiposCarga = repTipoCarga.Consultar(descricao, ativo, codigoGrupoPessoas, cliente, codigosTipoCarga, isFornecedor, codigoTipoOperacaoEmissao, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, codigoTipoCargaEmbarcador, filtrarSomenteDispMontagemCarga, filtrarTiposPrincipais, codigosFornecedor);
                    quantidadeTotal = repTipoCarga.ContarConsulta(descricao, ativo, codigoGrupoPessoas, cliente, codigosTipoCarga, isFornecedor, codigoTipoOperacaoEmissao, codigoTipoCargaEmbarcador, filtrarSomenteDispMontagemCarga, filtrarTiposPrincipais, codigosFornecedor);
                }
                else
                {
                    Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga repositorioQuantidadePorTipoDeCargaDescarregamentoTipoCarga = new Repositorio.Embarcador.Logistica.QuantidadePorTipoDeCargaDescarregamentoTipoCarga(unitOfWork);

                    tiposCarga = repositorioQuantidadePorTipoDeCargaDescarregamentoTipoCarga.BuscarTiposDeCargaPorCentroDescarregamento(centroDescarregamento.Codigo);
                    quantidadeTotal = tiposCarga.Count;
                }

                grid.setarQuantidadeTotal(quantidadeTotal);

                // if ((from obj in tiposCarga where obj.GrupoPessoas != null || obj.Pessoa != null select obj).FirstOrDefault() != null)
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoCarga.Embarcador, "Embarcador", 35, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> tiposDeOperacao = tiposCarga.Count > 0 ? repositorioTipoOperacao.BuscarTiposOperacaoPorTipoDeCarga(tiposCarga.Select(o => o.Codigo).ToList()) : new List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>();

                var retorno = (from obj in tiposCarga
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao,
                                   obj.DescricaoAtivo,
                                   obj.Paletizado,
                                   Embarcador = (obj.Pessoa != null) ? obj.Pessoa.Nome + "(" + obj.Pessoa.CPF_CNPJ_Formatado + ")" : ((obj.GrupoPessoas != null) ? obj.GrupoPessoas.Descricao : ""),
                                   obj.CodigoTipoCargaEmbarcador,
                                   obj.PrioridadeCarga,
                                   TipoCargaPrincipal = obj.TipoCargaPrincipal?.Descricao ?? "",
                                   PermiteInformarRecebedor = (from o in tiposDeOperacao where o.TipoDeCargaPadraoOperacao.Codigo == obj.Codigo select (o?.PermiteInformarRecebedorAgendamento ?? false)).FirstOrDefault(),
                                   FretePorContaDoCliente = (from o in tiposDeOperacao where o.TipoDeCargaPadraoOperacao.Codigo == obj.Codigo select (o?.FretePorContadoCliente ?? false)).FirstOrDefault(),
                                   ExigirQueCDDestinoSejaInformadoAgendamento = (from o in tiposDeOperacao where o.TipoDeCargaPadraoOperacao.Codigo == obj.Codigo select (o?.ConfiguracaoAgendamentoColetaEntrega?.ExigirQueCDDestinoSejaInformadoAgendamento ?? false)).FirstOrDefault(),
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPorFilial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                int codigoFilial = Request.GetIntParam("Filial");
                List<int> codigosFilial = Request.GetListParam<int>("Filiais");
                List<int> codigosTipoCarga = null;
                string descricao = Request.GetStringParam("descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = Request.GetEnumParam("Ativo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                if (codigoFilial > 0)
                    codigosFilial.Add(codigoFilial);

                if (Request.GetBoolParam("FiltrarPorConfiguracaoOperadorLogistica", valorPadrao: true))
                    codigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);

                bool.TryParse(Request.Params("FiltrarSomenteDisponiveisMontagemCarga"), out bool filtrarSomenteDispMontagemCarga);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Paletizado", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 45, Models.Grid.Align.left, true);

                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                int totalRegistros = repositorioTipoCarga.ContarConsultaPorFilial(codigosFilial, descricao, ativo, codigosTipoCarga, filtrarSomenteDispMontagemCarga);
                List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga> tiposCarga = (totalRegistros > 0) ? repositorioTipoCarga.ConsultarPorFilial(codigosFilial, descricao, ativo, codigosTipoCarga, parametrosConsulta, filtrarSomenteDispMontagemCarga) : new List<Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>();

                var tiposCargaRetornar = (
                    from tipoCarga in tiposCarga
                    select new
                    {
                        tipoCarga.Codigo,
                        tipoCarga.Descricao,
                        tipoCarga.DescricaoAtivo,
                        tipoCarga.Paletizado
                    }
                ).ToList();

                grid.AdicionaRows(tiposCargaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

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
        public async Task<IActionResult> PesquisaModelosVeiculares()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                decimal? capacidada = null;
                if (!String.IsNullOrEmpty(Request.Params("CapacidadePesoTransporte")))
                {
                    capacidada = decimal.Parse(Request.Params("CapacidadePesoTransporte"));
                }

                int codigoTipoCarga = int.Parse(Request.Params("TipoCarga"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 46, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoCarga.Capacidade, "CapacidadePesoTransporte", 13, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoCarga.ToleranciaExtra, "ToleranciaPesoExtra", 15, Models.Grid.Align.right, false, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoCarga.ToleranciaMinima, "ToleranciaPesoMenor", 15, Models.Grid.Align.right, false, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoCarga.Eixos, "NumeroEixos", 6, Models.Grid.Align.right);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.TipoCarga.Reboques, "NumeroReboques", 6, Models.Grid.Align.right);
                grid.AdicionarCabecalho("VeiculoPaletizado", false);
                grid.AdicionarCabecalho("NumeroPaletes", false);
                grid.AdicionarCabecalho("OcupacaoCubicaPaletes", false);
                grid.AdicionarCabecalho("ToleranciaMinimaPaletes", false);
                grid.AdicionarCabecalho("ModeloControlaCubagem", false);
                grid.AdicionarCabecalho("ExigirDefinicaoReboquePedido", false);
                grid.AdicionarCabecalho("Cubagem", false);
                grid.AdicionarCabecalho("ToleranciaMinimaCubagem", false);
                grid.AdicionarCabecalho("ModeloCalculoFranquiaCodigo", false);
                grid.AdicionarCabecalho("ModeloCalculoFranquiaDescricao", false);
                grid.AdicionarCabecalho("GrupoModeloVeicularCodigo", false);
                grid.AdicionarCabecalho("GrupoModeloVeicularDescricao", false);
                grid.AdicionarCabecalho("UnidadeCapacidade", false);

                Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> listaModeloVeicularCarga = repTipoCargaModeloVeicular.Consultar(codigoTipoCarga, descricao, capacidada, "ModeloVeicularCarga." + grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repTipoCargaModeloVeicular.ContarConsulta(codigoTipoCarga, descricao, capacidada));

                dynamic lista = (
                    from p in listaModeloVeicularCarga
                    select new
                    {
                        Codigo = p.ModeloVeicularCarga.Codigo,
                        p.ModeloVeicularCarga.Descricao,
                        p.ModeloVeicularCarga.CapacidadePesoTransporte,
                        p.ModeloVeicularCarga.ToleranciaPesoExtra,
                        p.ModeloVeicularCarga.ToleranciaPesoMenor,
                        p.ModeloVeicularCarga.NumeroEixos,
                        p.ModeloVeicularCarga.NumeroReboques,
                        p.ModeloVeicularCarga.Cubagem,
                        p.ModeloVeicularCarga.NumeroPaletes,
                        p.ModeloVeicularCarga.OcupacaoCubicaPaletes,
                        p.ModeloVeicularCarga.ToleranciaMinimaPaletes,
                        p.ModeloVeicularCarga.ToleranciaMinimaCubagem,
                        p.ModeloVeicularCarga.VeiculoPaletizado,
                        p.ModeloVeicularCarga.ModeloControlaCubagem,
                        p.ModeloVeicularCarga.ExigirDefinicaoReboquePedido,
                        ModeloCalculoFranquiaCodigo = p.ModeloVeicularCarga.ModeloCalculoFranquia?.Codigo ?? 0,
                        ModeloCalculoFranquiaDescricao = p.ModeloVeicularCarga.ModeloCalculoFranquia?.Descricao ?? "",
                        GrupoModeloVeicularCodigo = p.ModeloVeicularCarga.GrupoModeloVeicular?.Codigo ?? 0,
                        GrupoModeloVeicularDescricao = p.ModeloVeicularCarga.GrupoModeloVeicular?.Descricao ?? "",
                        UnidadeCapacidade = p.ModeloVeicularCarga.UnidadeCapacidade == UnidadeCapacidade.Unidade
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out double cpfCnpjPessoa);

                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga();

                PreencherTipoCarga(tipoCarga, unitOfWork);

                tipoCarga.GrupoPessoas = codigoGrupoPessoas > 0 ? new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas() { Codigo = codigoGrupoPessoas } : null;
                tipoCarga.Pessoa = codigoGrupoPessoas <= 0 && cpfCnpjPessoa > 0 ? new Dominio.Entidades.Cliente() { CPF_CNPJ = cpfCnpjPessoa } : null;

                int.TryParse(Request.Params("TipoCargaPrincipal"), out int codigoTipoCargaPrincipal);
                tipoCarga.Principal = bool.Parse(Request.Params("Principal"));
                if (!tipoCarga.Principal)
                    tipoCarga.TipoCargaPrincipal = (codigoTipoCargaPrincipal > 0 ? new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga() { Codigo = codigoTipoCargaPrincipal } : null);
                else
                    tipoCarga.TipoCargaPrincipal = null;

                bool novo = true;
                if (!string.IsNullOrEmpty(tipoCarga.CodigoTipoCargaEmbarcador))
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga existeTipoCarga = await repTipoCarga.BuscarPorCodigoEmbarcadorAsync(tipoCarga.CodigoTipoCargaEmbarcador);

                    if (existeTipoCarga != null)
                        novo = false;
                }
                else
                    tipoCarga.CodigoTipoCargaEmbarcador = Guid.NewGuid().ToString().Replace("-", "");

                if (!novo)
                    throw new ControllerException(Localization.Resources.Cargas.TipoCarga.JaExisteUmTipoDeCargaCadastradoParaCodigoInformado);

                await repTipoCarga.InserirAsync(tipoCarga, Auditado);

                dynamic listaModelosVeiculares = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ModelosVeicularesCargas"));

                foreach (var modelo in listaModelosVeiculares)
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular tipoCargaModeloVeicular = new Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular();
                    tipoCargaModeloVeicular.Posicao = (int)modelo.Posicao;
                    tipoCargaModeloVeicular.ModeloVeicularCarga = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga() { Codigo = (int)modelo.Codigo };
                    tipoCargaModeloVeicular.TipoDeCarga = tipoCarga;

                    await repTipoCargaModeloVeicular.InserirAsync(tipoCargaModeloVeicular);
                }

                SalvarSensorOpentech(tipoCarga, unitOfWork);
                CriarFaixasTempoDescargaPorPeso(tipoCarga, unitOfWork);
                SalvarIntegracoes(tipoCarga, unitOfWork);
                SalvarTipoLicenca(tipoCarga, unitOfWork);
                await SalvarCodigosIntegracaoAsync(tipoCarga, unitOfWork, cancellationToken);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.TipoCarga.OcorreuUmaFalhaAoAdicionar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Atualizar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int.TryParse(Request.Params("GrupoPessoas"), out int codigoGrupoPessoas);

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("Pessoa")), out double cpfCnpjPessoa);

                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = await repTipoCarga.BuscarPorCodigoAsync(int.Parse(Request.Params("Codigo")), true);

                if (tipoCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherTipoCarga(tipoCarga, unitOfWork);

                int.TryParse(Request.Params("TipoCargaPrincipal"), out int codigoTipoCargaPrincipal);
                bool.TryParse(Request.Params("Principal"), out bool principal);
                //Se for principal e estiver vindo como não principal nao podeoms deixar salvar se existir algum outro tipo relacionaod a esse tipo
                if (tipoCarga.Principal && !principal)
                {
                    var tipos = repTipoCarga.BuscarPorTipoCargaPrincipal(tipoCarga.Codigo);
                    if (tipos?.Count > 0)
                        throw new ControllerException(Localization.Resources.Cargas.TipoCarga.NaoPossivelAlterarTipoDeCargaDePrincipalParaNaoPrincipalPoisExistemOutrosTiposRelacionadosEste);
                }
                tipoCarga.Principal = principal;
                if (!tipoCarga.Principal)
                    tipoCarga.TipoCargaPrincipal = (codigoTipoCargaPrincipal > 0 ? new Dominio.Entidades.Embarcador.Cargas.TipoDeCarga() { Codigo = codigoTipoCargaPrincipal } : null);
                else
                    tipoCarga.TipoCargaPrincipal = null;

                if ((tipoCarga.GrupoPessoas != null && codigoGrupoPessoas != tipoCarga.GrupoPessoas.Codigo) ||
                    (tipoCarga.GrupoPessoas == null && codigoGrupoPessoas > 0) ||
                    (tipoCarga.Pessoa != null && cpfCnpjPessoa != tipoCarga.Pessoa.CPF_CNPJ) ||
                    (tipoCarga.Pessoa == null && cpfCnpjPessoa > 0d))
                {
                    Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);

                    if (repTabelaFrete.ContarPorTipoCarga(tipoCarga.Codigo) > 0)
                        throw new ControllerException(Localization.Resources.Cargas.TipoCarga.NaoPossivelAlterarGrupoDePessoasOuPessoaDoTipoDeCargaPoisMesmoEstaVinculadoUmaOuMaisTabelasDeFrete);
                }

                tipoCarga.GrupoPessoas = codigoGrupoPessoas > 0 ? new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas() { Codigo = codigoGrupoPessoas } : null;
                tipoCarga.Pessoa = codigoGrupoPessoas <= 0 && cpfCnpjPessoa > 0 ? new Dominio.Entidades.Cliente() { CPF_CNPJ = cpfCnpjPessoa } : null;

                bool novo = true;
                if (!string.IsNullOrEmpty(tipoCarga.CodigoTipoCargaEmbarcador))
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoDeCarga existeTipoCarga = repTipoCarga.BuscarPorCodigoEmbarcador(tipoCarga.CodigoTipoCargaEmbarcador);

                    if (existeTipoCarga != null)
                        if (existeTipoCarga.Codigo != tipoCarga.Codigo)
                            novo = false;
                }
                if (!novo)
                    throw new ControllerException(Localization.Resources.Cargas.TipoCarga.JaExisteUmTipoDeCargaCadastradoParaCodigoInformado);

                Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto = await repTipoCarga.AtualizarAsync(tipoCarga, Auditado);

                List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tipoCargaModelosAtivos = new List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular>();

                dynamic listaModelosVeiculares = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ModelosVeicularesCargas"));
                foreach (var modelo in listaModelosVeiculares)
                {
                    Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular tipoCargaModeloVeicular = repTipoCargaModeloVeicular.ConsultarPorModeloVeicular(tipoCarga.Codigo, (int)modelo.Codigo);

                    int tempoDescarga = 0;
                    int.TryParse((string)modelo.TempoDescarga, out tempoDescarga);

                    if (tipoCargaModeloVeicular == null)
                    {
                        tipoCargaModeloVeicular = new Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular();
                        tipoCargaModeloVeicular.Posicao = (int)modelo.Posicao;
                        tipoCargaModeloVeicular.ModeloVeicularCarga = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga() { Codigo = (int)modelo.Codigo };
                        tipoCargaModeloVeicular.TipoDeCarga = tipoCarga;
                        tipoCargaModeloVeicular.TempoDescarga = tempoDescarga;
                        await repTipoCargaModeloVeicular.InserirAsync(tipoCargaModeloVeicular, Auditado, historicoObjeto);
                    }
                    else
                    {
                        tipoCargaModeloVeicular.Initialize();
                        tipoCargaModeloVeicular.Posicao = (int)modelo.Posicao;
                        tipoCargaModeloVeicular.ModeloVeicularCarga = new Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga() { Codigo = (int)modelo.Codigo };
                        tipoCargaModeloVeicular.TipoDeCarga = tipoCarga;
                        tipoCargaModeloVeicular.TempoDescarga = tempoDescarga;
                        await repTipoCargaModeloVeicular.AtualizarAsync(tipoCargaModeloVeicular, Auditado, historicoObjeto);
                    }
                    tipoCargaModelosAtivos.Add(tipoCargaModeloVeicular);
                }

                List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> tipoCargaModelosSalvosNoBanco = repTipoCargaModeloVeicular.ConsultarPorTipoCarga(tipoCarga.Codigo);
                foreach (Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular tipoCargaModeloSalvoNoBanco in tipoCargaModelosSalvosNoBanco)
                {
                    if (!tipoCargaModelosAtivos.Exists(obj => obj.Codigo == tipoCargaModeloSalvoNoBanco.Codigo))
                    {
                        await repTipoCargaModeloVeicular.DeletarAsync(tipoCargaModeloSalvoNoBanco, Auditado, historicoObjeto);
                    }
                }

                SalvarSensorOpentech(tipoCarga, unitOfWork);
                CriarFaixasTempoDescargaPorPeso(tipoCarga, unitOfWork);
                SalvarIntegracoes(tipoCarga, unitOfWork);
                SalvarTipoLicenca(tipoCarga, unitOfWork);
                await SalvarCodigosIntegracaoAsync(tipoCarga, unitOfWork, cancellationToken);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.TipoCarga.OcorreuUmaFalhaAoAtualizar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = await repTipoCarga.BuscarPorCodigoAsync(codigo, auditavel: false);

                if (tipoCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso repTipoCargaTempoDescargaFaixaPeso = new Repositorio.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCargaSensorOpentech repTipoDeCargaSensorOpentech = new Repositorio.Embarcador.Cargas.TipoDeCargaSensorOpentech(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicular> listaTipoCarga = repTipoCargaModeloVeicular.ConsultarPorTipoCarga(tipoCarga.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso> listaTipoCargaTempoDescargaFaixaPeso = repTipoCargaTempoDescargaFaixaPeso.BuscarPorTipoCarga(tipoCarga.Codigo);
                Dominio.Entidades.Embarcador.Cargas.TipoDeCargaSensorOpentech tipoDeCargaSensorOpentech = repTipoDeCargaSensorOpentech.ConsultarPorTipoCarga(tipoCarga.Codigo);

                var dynTipoCarga = new
                {
                    NCM = new
                    {
                        Codigo = tipoCarga.NCM,
                        Descricao = tipoCarga.NCM
                    },
                    tipoCarga.NBS,
                    tipoCarga.Ativo,
                    tipoCarga.Codigo,
                    tipoCarga.Descricao,
                    tipoCarga.TipoTempoDescarga,
                    tipoCarga.ModalProposta,
                    CodigoTipoCargaEmbarcador = tipoCarga.CodigoTipoCargaEmbarcador != null ? tipoCarga.CodigoTipoCargaEmbarcador : "",
                    tipoCarga.ControlaTemperatura,
                    tipoCarga.ExigeVeiculoRastreado,
                    tipoCarga.TipoCargaMDFe,
                    tipoCarga.IdentificacaoMercadoriaInfolog,
                    IdentificacaoMercadoriaKrona = new { Codigo = tipoCarga.IdentificacaoMercadoriaKrona?.Codigo ?? 0, Descricao = tipoCarga.IdentificacaoMercadoriaKrona?.Descricao ?? "" },
                    GrupoPessoas = new
                    {
                        Codigo = tipoCarga.GrupoPessoas?.Codigo ?? 0,
                        Descricao = tipoCarga.GrupoPessoas?.Descricao ?? string.Empty
                    },
                    Pessoa = new
                    {
                        Codigo = tipoCarga.Pessoa?.CPF_CNPJ ?? 0f,
                        Descricao = tipoCarga.Pessoa?.Nome ?? string.Empty
                    },
                    ModelosVeicularesCargas = (from p in listaTipoCarga
                                               select new
                                               {
                                                   Codigo = p.ModeloVeicularCarga.Codigo,
                                                   Descricao = p.ModeloVeicularCarga.Descricao,
                                                   Posicao = p.Posicao,
                                                   TempoDescarga = p.TempoDescarga
                                               }).ToList(),
                    ListaFaixasTempoDescargaPorPeso = (from o in listaTipoCargaTempoDescargaFaixaPeso
                                                       select new
                                                       {
                                                           Codigo = o.Codigo,
                                                           Inicio = o.Inicio,
                                                           Fim = o.Fim,
                                                           Tempo = o.TempoDescarga
                                                       }).ToList(),
                    FaixaTemperatura = tipoCarga.FaixaDeTemperatura?.Codigo,
                    PossuiCargaPerigosa = tipoCarga.PossuiCargaPerigosa.HasValue ? tipoCarga.PossuiCargaPerigosa.Value : false,
                    tipoCarga.IndisponivelMontagemCarregamento,
                    tipoCarga.BloquearLiberacaoParaTransportadores,
                    tipoCarga.NaoPermitirFornecedorEscolherNoAgendamento,
                    tipoCarga.NaoValidarDataCheckList,
                    tipoCarga.Paletizado,
                    tipoCarga.ValidarLicencasNCM,
                    tipoCarga.ClasseONU,
                    tipoCarga.SequenciaONU,
                    tipoCarga.CodigoPsnONU,
                    tipoCarga.ObservacaoONU,
                    CodigoTipoSensorOpentech = tipoDeCargaSensorOpentech?.CodigoTipoSensorOpentech ?? -1,
                    QuantidadeSensores = tipoDeCargaSensorOpentech?.QuantidadeSensores ?? 1,
                    ToleranciaTemperaturaSuperior = tipoDeCargaSensorOpentech?.ToleranciaTemperaturaSuperior.ToString("n0") ?? "-999",
                    ToleranciaTemperaturaInferior = tipoDeCargaSensorOpentech?.ToleranciaTemperaturaInferior.ToString("n0") ?? "-999",
                    TemperaturaIdealSuperior = tipoDeCargaSensorOpentech?.TemperaturaIdealSuperior.ToString("n0") ?? "-999",
                    TemperaturaIdealInferior = tipoDeCargaSensorOpentech?.TemperaturaIdealInferior.ToString("n0") ?? "-999",
                    tipoCarga.Principal,
                    tipoCarga.ProdutoPredominante,
                    tipoCarga.CodigoNaturezaCIOT,
                    tipoCarga.PrioridadeCarga,
                    tipoCarga.BloquearMontagemCargaComPedidoProvisorio,
                    TipoCargaPrincipal = new
                    {
                        Codigo = tipoCarga.TipoCargaPrincipal?.Codigo ?? 0,
                        Descricao = tipoCarga.TipoCargaPrincipal?.Descricao ?? string.Empty
                    },
                    Integracoes = ObterIntegracoes(tipoCarga.Codigo, unitOfWork, cancellationToken),
                    CodigosIntegracao = await ObterCodigosIntegracaoAsync(tipoCarga.Codigo, unitOfWork, cancellationToken),
                    ListaTiposLicenca = (from obj in tipoCarga.TiposLicenca
                                         orderby obj.Descricao
                                         select new
                                         {
                                             obj.Codigo,
                                             obj.Descricao,
                                             obj.DescricaoTipo
                                         }).ToList(),
                    tipoCarga.TipoDeCargaEFrete,
                    tipoCarga.ImprimirTabelaTemperaturaNoVersoCTe,
                };

                return new JsonpResult(dynTipoCarga);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.TipoCarga.OcorreuUmaFalhaAoBuscarPorCodigo);
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
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.TipoDeCarga repTipoCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular repTipoCargaModeloVeicular = new Repositorio.Embarcador.Cargas.TipoCargaModeloVeicular(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = repTipoCarga.BuscarPorCodigo(codigo);

                if (tipoCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                repTipoCargaModeloVeicular.DeletarPorTipoCarga(codigo);
                repTipoCarga.Deletar(tipoCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.TipoCarga.NaoFoiPossivelExcluirRegistroPoisMesmoJaPossuiVinculoComOutrosRecursosDoSistemaRecomendamosQueVoceInativeRegistroCasoNaoDesejaMaisUtilizaLo);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Cargas.TipoCarga.OcorreuUmaFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarTipoSensorOpenTech()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string mensagemErro = string.Empty;

                List<object> produtos = Servicos.Embarcador.Integracao.OpenTech.IntegracaoProdutoOpenTech.ObterTipoSensoresOpenTech(unidadeTrabalho, out mensagemErro);

                if (produtos == null && !string.IsNullOrWhiteSpace(mensagemErro))
                    return new JsonpResult(false, "Ocorreu uma falha ao obter os tipos de sensor da OpenTech: " + mensagemErro);

                return new JsonpResult(produtos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os Tipos de Sensor OpenTech");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> tipoIntegracaos = repTipoIntegracao.BuscarTodos();

                var retornoIntegracoes = (
                        from obj in tipoIntegracaos
                        select new
                        {
                            obj.Codigo,
                            obj.Tipo,
                            Descricao = obj.Tipo.ObterDescricao(),
                        }
                    ).ToList();
                return new JsonpResult(new
                {
                    Integracoes = retornoIntegracoes
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.TipoCarga.OcorreuUmaFalhaAoBuscarIntegracoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private void PreencherTipoCarga(Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.IdentificacaoMercadoriaKrona repIdentificacaoMercadoriaKrona = new Repositorio.Embarcador.Integracao.IdentificacaoMercadoriaKrona(unitOfWork);

            bool.TryParse(Request.Params("ControlaTemperatura"), out bool controlaTemperatura);
            bool.TryParse(Request.Params("ExigeVeiculoRastreado"), out bool exigeVeiculoRastreado);
            bool.TryParse(Request.Params("PossuiCargaPerigosa"), out bool possuiCargaPerigosa);
            bool.TryParse(Request.Params("IndisponivelMontagemCarregamento"), out bool indispMontagemCarga);

            string ncm = Request.GetStringParam("NCM");
            string nbs = Request.GetStringParam("NBS");
            string classeONU = Request.GetStringParam("ClasseONU");
            string sequenciaONU = Request.GetStringParam("SequenciaONU");
            string codigoPsnONU = Request.GetStringParam("CodigoPsnONU");
            string observacaoONU = Request.GetStringParam("ObservacaoONU");

            int codigoFaixaTemperatura = Request.GetIntParam("FaixaTemperatura");
            int codigoIdentificacaoMercadoriaKrona = Request.GetIntParam("IdentificacaoMercadoriaKrona");

            tipoCarga.NCM = ncm;
            tipoCarga.NBS = nbs;
            tipoCarga.Ativo = bool.Parse(Request.Params("Ativo"));
            tipoCarga.Descricao = Request.Params("Descricao");
            tipoCarga.TipoTempoDescarga = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTempoDescargaTipoCarga>("TipoTempoDescarga");
            tipoCarga.ModalProposta = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal>("ModalProposta");

            tipoCarga.CodigoTipoCargaEmbarcador = Request.Params("CodigoTipoCargaEmbarcador");
            tipoCarga.ControlaTemperatura = controlaTemperatura;

            if (tipoCarga.ControlaTemperatura)
                tipoCarga.FaixaDeTemperatura = ObterFaixaTemperatura(codigoFaixaTemperatura, unitOfWork);
            else
                tipoCarga.FaixaDeTemperatura = null;

            tipoCarga.ExigeVeiculoRastreado = exigeVeiculoRastreado;
            tipoCarga.PossuiCargaPerigosa = possuiCargaPerigosa;
            tipoCarga.IndisponivelMontagemCarregamento = indispMontagemCarga;
            tipoCarga.BloquearLiberacaoParaTransportadores = Request.GetBoolParam("BloquearLiberacaoParaTransportadores");
            tipoCarga.NaoPermitirFornecedorEscolherNoAgendamento = Request.GetBoolParam("NaoPermitirFornecedorEscolherNoAgendamento");
            tipoCarga.NaoValidarDataCheckList = Request.GetBoolParam("NaoValidarDataCheckList");
            tipoCarga.Paletizado = Request.GetBoolParam("Paletizado");
            tipoCarga.ValidarLicencasNCM = Request.GetBoolParam("ValidarLicencasNCM");
            tipoCarga.ClasseONU = classeONU;
            tipoCarga.SequenciaONU = sequenciaONU;
            tipoCarga.CodigoPsnONU = codigoPsnONU;
            tipoCarga.ObservacaoONU = observacaoONU;
            tipoCarga.TipoCargaMDFe = Request.GetNullableEnumParam<Dominio.Enumeradores.TipoCargaMDFe>("TipoCargaMDFe");
            tipoCarga.IdentificacaoMercadoriaKrona = codigoIdentificacaoMercadoriaKrona > 0 ? repIdentificacaoMercadoriaKrona.BuscarPorCodigo(codigoIdentificacaoMercadoriaKrona, false) : null;
            tipoCarga.IdentificacaoMercadoriaInfolog = Request.GetStringParam("IdentificacaoMercadoriaInfolog");
            tipoCarga.ProdutoPredominante = Request.GetStringParam("ProdutoPredominante");
            tipoCarga.PrioridadeCarga = Request.GetIntParam("PrioridadeCarga");
            tipoCarga.CodigoNaturezaCIOT = Request.GetIntParam("CodigoNaturezaCIOT");
            tipoCarga.TipoDeCargaEFrete = Request.GetIntParam("TipoDeCargaEFrete");
            tipoCarga.BloquearMontagemCargaComPedidoProvisorio = Request.GetBoolParam("BloquearMontagemCargaComPedidoProvisorio");
            tipoCarga.ImprimirTabelaTemperaturaNoVersoCTe = Request.GetBoolParam("ImprimirTabelaTemperaturaNoVersoCTe");
        }

        private Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura ObterFaixaTemperatura(int codigo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.FaixaTemperatura repFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);

            return repFaixaTemperatura.BuscarPorCodigo(codigo);
        }

        private void CriarFaixasTempoDescargaPorPeso(Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso repTipoCargaTempoDescargaFaixaPeso = new Repositorio.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso(unitOfWork);

            // Deleta outras faixas já existentes do TipoCarga
            var faixasAntigas = repTipoCargaTempoDescargaFaixaPeso.BuscarPorTipoCarga(tipoCarga.Codigo);

            foreach (var faixaAntiga in faixasAntigas)
            {
                repTipoCargaTempoDescargaFaixaPeso.Deletar(faixaAntiga);
            }

            dynamic listaFaixas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ListaFaixasTempoDescargaPorPeso"));

            foreach (var faixaDoFront in listaFaixas)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso faixa = new Dominio.Entidades.Embarcador.Cargas.TipoCargaTempoDescargaFaixaPeso();
                faixa.TipoDeCarga = tipoCarga;
                faixa.Inicio = (int)faixaDoFront.Inicio;
                faixa.Fim = (int)faixaDoFront.Fim;
                faixa.TempoDescarga = (int)faixaDoFront.Tempo;
                repTipoCargaTempoDescargaFaixaPeso.Inserir(faixa);
            }
        }

        private void SalvarSensorOpentech(Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoDeCargaSensorOpentech repTipoDeCargaSensorOpentech = new Repositorio.Embarcador.Cargas.TipoDeCargaSensorOpentech(unitOfWork);

            int.TryParse(Request.Params("CodigoTipoSensorOpentech"), out int codigoTipoSensorOpentech);
            int.TryParse(Request.Params("QuantidadeSensores"), out int quantidadeSensores);
            int.TryParse(Request.Params("ToleranciaTemperaturaSuperior"), out int toleranciaTemperaturaSuperior);
            int.TryParse(Request.Params("ToleranciaTemperaturaInferior"), out int toleranciaTemperaturaInferior);
            int.TryParse(Request.Params("TemperaturaIdealSuperior"), out int temperaturaIdealSuperior);
            int.TryParse(Request.Params("TemperaturaIdealInferior"), out int temperaturaIdealInferior);

            Dominio.Entidades.Embarcador.Cargas.TipoDeCargaSensorOpentech tipoDeCargaSensorOpentech = repTipoDeCargaSensorOpentech.ConsultarPorTipoCarga(tipoCarga.Codigo);

            if (tipoDeCargaSensorOpentech == null && codigoTipoSensorOpentech > 0)
            {
                tipoDeCargaSensorOpentech = new Dominio.Entidades.Embarcador.Cargas.TipoDeCargaSensorOpentech();
                tipoDeCargaSensorOpentech.CodigoTipoSensorOpentech = codigoTipoSensorOpentech;
                tipoDeCargaSensorOpentech.QuantidadeSensores = quantidadeSensores;
                tipoDeCargaSensorOpentech.ToleranciaTemperaturaSuperior = toleranciaTemperaturaSuperior;
                tipoDeCargaSensorOpentech.ToleranciaTemperaturaInferior = toleranciaTemperaturaInferior;
                tipoDeCargaSensorOpentech.TemperaturaIdealSuperior = temperaturaIdealSuperior;
                tipoDeCargaSensorOpentech.TemperaturaIdealInferior = temperaturaIdealInferior;
                tipoDeCargaSensorOpentech.TipoDeCarga = tipoCarga;
                repTipoDeCargaSensorOpentech.Inserir(tipoDeCargaSensorOpentech);
            }
            else if (tipoDeCargaSensorOpentech != null && codigoTipoSensorOpentech > 0)
            {
                tipoDeCargaSensorOpentech.CodigoTipoSensorOpentech = codigoTipoSensorOpentech;
                tipoDeCargaSensorOpentech.QuantidadeSensores = quantidadeSensores;
                tipoDeCargaSensorOpentech.ToleranciaTemperaturaSuperior = toleranciaTemperaturaSuperior;
                tipoDeCargaSensorOpentech.ToleranciaTemperaturaInferior = toleranciaTemperaturaInferior;
                tipoDeCargaSensorOpentech.TemperaturaIdealSuperior = temperaturaIdealSuperior;
                tipoDeCargaSensorOpentech.TemperaturaIdealInferior = temperaturaIdealInferior;
                repTipoDeCargaSensorOpentech.Atualizar(tipoDeCargaSensorOpentech);
            }
            else if (tipoDeCargaSensorOpentech != null && codigoTipoSensorOpentech == 0)
                repTipoDeCargaSensorOpentech.Deletar(tipoDeCargaSensorOpentech);
        }

        private void SalvarIntegracoes(Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoDeCargaIntegracao repositorioIntegracao = new Repositorio.Embarcador.Cargas.TipoDeCargaIntegracao(unitOfWork);

            dynamic dynIntegracoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Integracoes"));

            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCargaIntegracao> integracoes = repositorioIntegracao.BuscarPorTipoCarga(tipoCarga.Codigo);

            if (integracoes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic integracao in dynIntegracoes)
                {
                    int codigo = ((string)integracao.Codigo).ToInt();
                    if (codigo > 0)
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Cargas.TipoDeCargaIntegracao> integracoesDeletar = (from obj in integracoes where !codigos.Contains(obj.Codigo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCargaIntegracao integracaoDeletar in integracoesDeletar)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoCarga, "Removeu a integração " + integracaoDeletar.Tipo.ObterDescricao(), unitOfWork);
                    repositorioIntegracao.Deletar(integracaoDeletar);
                }
            }

            foreach (dynamic dynIntegracao in dynIntegracoes)
            {
                int codigo = ((string)dynIntegracao.Codigo).ToInt();

                Dominio.Entidades.Embarcador.Cargas.TipoDeCargaIntegracao integracao = codigo > 0 ? repositorioIntegracao.BuscarPorCodigo(codigo, false) : null;
                if (integracao == null)
                {
                    integracao = new Dominio.Entidades.Embarcador.Cargas.TipoDeCargaIntegracao()
                    {
                        TipoDeCarga = tipoCarga,
                        Tipo = ((string)dynIntegracao.Tipo).ToEnum<TipoIntegracao>()
                    };

                    repositorioIntegracao.Inserir(integracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, tipoCarga, "Adicionou a integração " + integracao.Tipo.ObterDescricao(), unitOfWork);
                }
            }
        }

        private void SalvarTipoLicenca(Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.Licenca repositorioTipoLicenca = new Repositorio.Embarcador.Configuracoes.Licenca(unitOfWork);
            dynamic tiposLicenca = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaTiposLicenca"));

            if (tipoCarga.TiposLicenca == null)
                tipoCarga.TiposLicenca = new List<Dominio.Entidades.Embarcador.Configuracoes.Licenca>();
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic tipoLicenca in tiposLicenca)
                    codigos.Add((int)tipoLicenca.Codigo);

                List<Dominio.Entidades.Embarcador.Configuracoes.Licenca> tiposDeletar = tipoCarga.TiposLicenca.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Configuracoes.Licenca tipoLicencaDeletar in tiposDeletar)
                    tipoCarga.TiposLicenca.Remove(tipoLicencaDeletar);
            }

            foreach (var tipoLicenca in tiposLicenca)
            {
                if (tipoCarga.TiposLicenca.Any(o => o.Codigo == (int)tipoLicenca.Codigo))
                    continue;

                Dominio.Entidades.Embarcador.Configuracoes.Licenca tipoDeCarga = repositorioTipoLicenca.BuscarPorCodigo((int)tipoLicenca.Codigo);
                tipoCarga.TiposLicenca.Add(tipoDeCarga);
            }
        }

        private async Task SalvarCodigosIntegracaoAsync(Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.TipoDeCargaCodigoIntegracao repositorioTipoDeCargaCodigoIntegracao = new Repositorio.Embarcador.Cargas.TipoDeCargaCodigoIntegracao(unitOfWork, cancellationToken);

            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCargaCodigoIntegracao> listaCodigosIntegracao = await repositorioTipoDeCargaCodigoIntegracao.BuscarPorTipoDeCargaAsync(tipoDeCarga.Codigo);
            dynamic[] codigosIntegracaoParametros = Request.GetArrayParam<dynamic>("CodigosIntegracao");

            foreach (dynamic codigoIntegracaoParametro in codigosIntegracaoParametros)
            {
                int codigo = ((string)codigoIntegracaoParametro.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Cargas.TipoDeCargaCodigoIntegracao codigoIntegracao = null;

                if (codigo > 0)
                    codigoIntegracao = listaCodigosIntegracao.First(o => o.Codigo == codigo);

                if (codigoIntegracao != null && listaCodigosIntegracao.Contains(codigoIntegracao))
                {
                    listaCodigosIntegracao.Remove(codigoIntegracao);
                    continue;
                }

                Dominio.Entidades.Embarcador.Cargas.TipoDeCargaCodigoIntegracao tipoDeCargaCodigoIntegracao = new Dominio.Entidades.Embarcador.Cargas.TipoDeCargaCodigoIntegracao()
                {
                    TipoDeCarga = tipoDeCarga,
                    CodigoIntegracao = ((string)codigoIntegracaoParametro.CodigoIntegracao).ToString(),
                    EtapaCarga = codigoIntegracaoParametro.EtapaCarga
                };

                await repositorioTipoDeCargaCodigoIntegracao.InserirAsync(tipoDeCargaCodigoIntegracao);
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, tipoDeCarga, $"Adicionou o código de integração {(string)codigoIntegracaoParametro.CodigoIntegracao}.", unitOfWork, cancellationToken);
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoDeCargaCodigoIntegracao codigoIntegracaoARemover in listaCodigosIntegracao)
            {
                await repositorioTipoDeCargaCodigoIntegracao.DeletarAsync(codigoIntegracaoARemover);
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, tipoDeCarga, $"Removeu o código de integração {codigoIntegracaoARemover.CodigoIntegracao}.", unitOfWork, cancellationToken);
            }
        }

        private dynamic ObterIntegracoes(int codigoTipoDeCarga, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.TipoDeCargaIntegracao repositorioIntegracao = new Repositorio.Embarcador.Cargas.TipoDeCargaIntegracao(unitOfWork, cancellationToken);
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCargaIntegracao> integracoesTipoCarga = repositorioIntegracao.BuscarPorTipoCarga(codigoTipoDeCarga);

            return (from o in integracoesTipoCarga
                    select new
                    {
                        o.Codigo,
                        o.Tipo,
                        Descricao = o.Tipo.ObterDescricao()
                    }).ToList();
        }

        private async Task<dynamic> ObterCodigosIntegracaoAsync(int codigoTipoDeCarga, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Cargas.TipoDeCargaCodigoIntegracao repositorioTipoDeCargaCodigoIntegracao = new Repositorio.Embarcador.Cargas.TipoDeCargaCodigoIntegracao(unitOfWork, cancellationToken);
            List<Dominio.Entidades.Embarcador.Cargas.TipoDeCargaCodigoIntegracao> codigosIntegracao = await repositorioTipoDeCargaCodigoIntegracao.BuscarPorTipoDeCargaAsync(codigoTipoDeCarga);

            return (from obj in codigosIntegracao
                    select new
                    {
                        obj.Codigo,
                        obj.CodigoIntegracao,
                        obj.EtapaCarga,
                        EtapaCargaDescricao = obj.EtapaCarga.ObterDescricaoIntegracaoApisul()
                    }).ToList();
        }

        #endregion Métodos Privados
    }
}
