using Dominio.Excecoes.Embarcador;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/Cliente")]
    public class ClienteController : BaseController
    {
		#region Construtores

		public ClienteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracao = repConfiguracao.BuscarConfiguracaoPadrao();


                string nome = Request.GetStringParam("Nome");
                string cpfCnpj = Utilidades.String.OnlyNumbers(Request.GetStringParam("CPF_CNPJ"));

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade> modalidades = null;

                if (!string.IsNullOrWhiteSpace(Request.Params("Modalidades")))
                    modalidades = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade>>(Request.Params("Modalidades"));

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimentoAux;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento? tipoAbastecimento = null;
                if (Enum.TryParse(Request.Params("TipoAbastecimento"), out tipoAbastecimentoAux))
                    tipoAbastecimento = tipoAbastecimentoAux;

                Dominio.Entidades.Localidade localidade = null;
                int codigoLocalidade = Request.GetIntParam("Localidade");

                if (codigoLocalidade > 0)
                    localidade = new Dominio.Entidades.Localidade() { Codigo = codigoLocalidade };

                Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = null;
                int codigoBaixaPagar = Request.GetIntParam("BaixaTituloPagar");

                if (codigoBaixaPagar > 0)
                {
                    Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unitOfWork);

                    tituloBaixa = repTituloBaixa.BuscarPorCodigo(codigoBaixaPagar);
                }

                List<string> listaRaizCnpj = null;
                int codigoGrupoPessoaRaizCNPJ = Request.GetIntParam("CodigoGrupoPessoaRaizCNPJ");

                if (codigoGrupoPessoaRaizCNPJ > 0)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repositorioGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                    listaRaizCnpj = repositorioGrupoPessoas.BuscarListaCNPJRaiz(codigoGrupoPessoaRaizCNPJ);
                }

                if (string.IsNullOrWhiteSpace(cpfCnpj))
                {
                    string cpfCnpjInformadoCampoNome = Utilidades.String.OnlyNumbers(nome);

                    if (!string.IsNullOrWhiteSpace(cpfCnpjInformadoCampoNome) && (Utilidades.Validate.ValidarCNPJ(cpfCnpjInformadoCampoNome) || Utilidades.Validate.ValidarCPF(cpfCnpjInformadoCampoNome)))
                    {
                        cpfCnpj = cpfCnpjInformadoCampoNome;
                        nome = string.Empty;
                    }
                }

                List<double> cpfCnpjDestinatarios = new List<double>();
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                {
                    Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedor = Usuario.ClienteFornecedor != null ? repModalidadeFornecedorPessoas.BuscarPorCliente(Usuario.ClienteFornecedor.CPF_CNPJ) : null;
                    cpfCnpjDestinatarios = modalidadeFornecedor?.Destinatarios.Select(o => o.CPF_CNPJ).ToList() ?? null;
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CPF_CNPJ_SemFormato", false);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    grid.AdicionarCabecalho("CodigoIntegracao", false);
                else
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "CodigoIntegracao", 8, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.RazaoSocial, "Nome", 26, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.NomeFantasia, "NomeFantasia", 10, Models.Grid.Align.left, true, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.CPFCNPJ, "CPF_CNPJ", 15, Models.Grid.Align.left, true, false, true);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.IE, "IE_RG", 12, Models.Grid.Align.left, false, true, true);
                else
                    grid.AdicionarCabecalho("IE_RG", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.Endereco, "Endereco", 10, Models.Grid.Align.left, false, true, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Cidade, "Localidade", 17, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CodigoLocalidade", false);
                grid.AdicionarCabecalho("CodigoAtividade", false);
                grid.AdicionarCabecalho("Atividade", false);
                grid.AdicionarCabecalho("Estado", false);
                grid.AdicionarCabecalho("CEP", false);
                grid.AdicionarCabecalho("Email", false);
                grid.AdicionarCabecalho("Numero", false);
                grid.AdicionarCabecalho("CodigoGrupo", false);
                grid.AdicionarCabecalho("DescricaoGrupo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("Latitude", false);
                grid.AdicionarCabecalho("Longitude", false);
                grid.AdicionarCabecalho("CodigoIBGE", false);
                grid.AdicionarCabecalho("CodigoCombustivel", false);
                grid.AdicionarCabecalho("DescricaoCombustivel", false);
                grid.AdicionarCabecalho("ValorCombustivel", false);
                grid.AdicionarCabecalho("ObservacaoEmissaoCarga", false);
                grid.AdicionarCabecalho("GerarPedidoColeta", false);
                grid.AdicionarCabecalho("CodigoRecebedorColeta", false);
                grid.AdicionarCabecalho("DescricaoRecebedorColeta", false);
                grid.AdicionarCabecalho("CodigoTransportador", false);
                grid.AdicionarCabecalho("DescricaoTransportador", false);
                grid.AdicionarCabecalho("Bloqueado", false);
                grid.AdicionarCabecalho("MotivoBloqueio", false);
                grid.AdicionarCabecalho("GrupoPessoasBloqueado", false);
                grid.AdicionarCabecalho("GrupoPessoasMotivoBloqueio", false);
                grid.AdicionarCabecalho("Ordem", false);
                grid.AdicionarCabecalho("FormaTituloFornecedor", false);
                grid.AdicionarCabecalho("TempoMedioPermanenciaFronteira", false);
                grid.AdicionarCabecalho("TempoMedioPermanenciaFronteiraMinutos", false);
                grid.AdicionarCabecalho("TempoCarregamento", false);
                grid.AdicionarCabecalho("TempoDescarregamento", false);
                grid.AdicionarCabecalho("DescricaoPais", false);
                grid.AdicionarCabecalho("AbreviacaoPais", false);
                grid.AdicionarCabecalho("PossuiOutrosEnderecos", false);
                grid.AdicionarCabecalho("CodigoOutroEndereco", false);
                grid.AdicionarCabecalho("DescricaoOutroEndereco", false);
                grid.AdicionarCabecalho("GerarAgendamentoPedidosExistentes", false);
                grid.AdicionarCabecalho("ObservacaoInterna", false);
                grid.AdicionarCabecalho("Bairro", false);
                grid.AdicionarCabecalho("Complemento", false);
                grid.AdicionarCabecalho("Telefone1", false);
				grid.AdicionarCabecalho("CondicaoPagamentoCodigo", false);
				grid.AdicionarCabecalho("CondicaoPagamentoDescricao", false);
				grid.AdicionarCabecalho("CondicaoPagamentoQuantidadeParcelas", false);
				grid.AdicionarCabecalho("CondicaoPagamentoIntervaloDias", false);

				string propriadedaOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propriadedaOrdenacao == "Codigo")
                    propriadedaOrdenacao = "CPF_CNPJ";

                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores repPostoCombustivelTabelaValores = new Repositorio.Embarcador.Pessoas.PostoCombustivelTabelaValores(unitOfWork);

                Dominio.Entidades.Empresa empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa : null;
                Dominio.ObjetosDeValor.FiltroPesquisaCliente filtrosPesquisa = new Dominio.ObjetosDeValor.FiltroPesquisaCliente()
                {
                    Ativo = Request.GetEnumParam("Ativo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo),
                    BaixaPagar = null,
                    CodigoGrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                    FiltrarPorCodigoDeIntegracaoNaPesquisaPorNomePessoaDentroDeEnderecos = configuracao.FiltrarPorCodigoDeIntegracaoNaPesquisaPorNomePessoaDentroDeEnderecos,
                    CpfCnpj = cpfCnpj.ToDouble(),
                    ListaCnpj = RetornaCodigosCNPJ(),
                    ListaRaizCnpj = listaRaizCnpj,
                    Localidade = localidade,
                    Modalidades = modalidades,
                    Nome = nome,
                    RaizCNPJ = Request.GetStringParam("RaizCNPJ"),
                    Tipo = Request.GetStringParam("TipoPessoa"),
                    NomeFantasia = Request.GetStringParam("NomeFantasia"),
                    SomenteFilial = Request.GetBoolParam("SomenteFilial"),
                    SomenteFronteira = Request.GetBoolParam("SomenteFronteira"),
                    SomenteAreaRedex = Request.GetBoolParam("PossuiAreaRedex"),
                    SomenteArmador = Request.GetBoolParam("PossuiArmador"),
                    SomenteSupridores = Request.GetBoolParam("SomenteSupridores"),
                    ApenasVinculadosACentroDescarregamento = Request.GetBoolParam("ApenasVinculadosACentroDescarregamento"),
                    GeoLocalizacaoTipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoLocalizacaoTipo.Todos,
                    CodigoEmpresa = empresa != null && empresa.VisualizarSomenteClientesAssociados ? empresa.Codigo : 0,
                    CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                };

                if (Request.GetBoolParam("FiltrarPorConfiguracaoOperadorLogistica"))
                {
                    filtrosPesquisa.ListaCodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);
                    filtrosPesquisa.ListaCodigosExpedidores = ObterListaCnpjCpfExpedidorPermitidosOperadorLogistica(unitOfWork);
                }

                if (tituloBaixa?.QuantidadeGrupoPessoa > 0 && tituloBaixa?.CodigoGrupoPessoa > 0)
                    filtrosPesquisa.CodigoGrupoPessoas = tituloBaixa?.CodigoGrupoPessoa ?? 0;

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && Usuario.ClienteFornecedor != null && Request.GetBoolParam("FiltrarGrupoFornecedor"))
                {
                    if (Usuario.ClienteFornecedor?.GrupoPessoas != null)
                        filtrosPesquisa.CodigoGrupoPessoas = Usuario.ClienteFornecedor.GrupoPessoas.Codigo;
                    else
                        filtrosPesquisa.CpfCnpj = Usuario.ClienteFornecedor.CPF_CNPJ;
                }

                List<Dominio.Entidades.Cliente> listaClientes = repositorioCliente.Consultar(filtrosPesquisa, propriadedaOrdenacao, grid.dirOrdena, grid.inicio, grid.limite, cpfCnpjDestinatarios);
                int totalRegistros = repositorioCliente.ContarConsulta(filtrosPesquisa);

                if (filtrosPesquisa.SomenteFilial)
                    totalRegistros = listaClientes.Count();

                if (!string.IsNullOrEmpty(nome) && listaClientes.Count > 0)
                {
                    Dominio.Entidades.Cliente clienteIntegracao = (from obj in listaClientes where obj.CodigoIntegracao == nome select obj).FirstOrDefault();
                    if (clienteIntegracao != null)
                    {
                        listaClientes = new List<Dominio.Entidades.Cliente>();
                        listaClientes.Add(clienteIntegracao);
                        totalRegistros = 1;
                    }
                }

                List<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores> combustiveis = new List<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores>();
                if (tipoAbastecimento.HasValue && tipoAbastecimento.Value > 0)
                    combustiveis = repPostoCombustivelTabelaValores.BuscarCombustiveisPorClientes((from obj in listaClientes select obj.CPF_CNPJ).ToList(), tipoAbastecimento.Value);

                grid.setarQuantidadeTotal(totalRegistros);

                dynamic lista = (from p in listaClientes
                                 select new
                                 {
                                     Codigo = p.CPF_CNPJ,
                                     GrupoPessoasBloqueado = p.GrupoPessoas?.Bloqueado ?? false,
                                     GrupoPessoasMotivoBloqueio = p.GrupoPessoas?.MotivoBloqueio ?? string.Empty,
                                     p.Bloqueado,
                                     p.MotivoBloqueio,
                                     p.CPF_CNPJ_SemFormato,
                                     p.Descricao,
                                     p.Nome,
                                     p.CodigoIntegracao,
                                     p.Numero,
                                     p.NomeFantasia,
                                     CPF_CNPJ = p.CPF_CNPJ_Formatado,
                                     p.Endereco,
                                     p.CEP,
                                     p.Localidade.CodigoIBGE,
                                     p.Latitude,
                                     p.Longitude,
                                     Localidade = p.Localidade?.DescricaoCidadeEstado ?? string.Empty,
                                     CodigoLocalidade = p.Localidade?.Codigo ?? 0,
                                     CodigoAtividade = p.Atividade?.Codigo ?? 0,
                                     Atividade = p.Atividade?.Descricao ?? string.Empty,
                                     p.IE_RG,
                                     Estado = p.Localidade?.Estado?.Sigla ?? string.Empty,
                                     p.Email,
                                     CodigoGrupo = p.GrupoPessoas?.Codigo ?? 0,
                                     DescricaoGrupo = p.GrupoPessoas?.Descricao ?? string.Empty,
                                     CodigoCombustivel = (from obj in combustiveis where obj.ModalidadeFornecedorPessoas.ModalidadePessoas.Cliente.CPF_CNPJ == p.CPF_CNPJ select obj).FirstOrDefault()?.Produto?.Codigo ?? 0,
                                     DescricaoCombustivel = (from obj in combustiveis where obj.ModalidadeFornecedorPessoas.ModalidadePessoas.Cliente.CPF_CNPJ == p.CPF_CNPJ select obj).FirstOrDefault()?.Produto?.Descricao ?? "",
                                     ValorCombustivel = (from obj in combustiveis where obj.ModalidadeFornecedorPessoas.ModalidadePessoas.Cliente.CPF_CNPJ == p.CPF_CNPJ select obj).FirstOrDefault()?.ValorFixo.ToString("n4") ?? "",
                                     string.Empty,
                                     ObservacaoEmissaoCarga = p.NaoUsarConfiguracaoEmissaoGrupo ? p.ObservacaoEmissaoCarga : p.GrupoPessoas?.ObservacaoEmissaoCarga ?? string.Empty,
                                     GerarPedidoColeta = p.GerarPedidoColeta == true ? p.GerarPedidoColeta : (p?.GrupoPessoas?.GerarPedidoColeta ?? false),
                                     CodigoRecebedorColeta = p.GerarPedidoColeta == true && p.RecebedorColeta != null ? p.RecebedorColeta.Codigo : (p?.GrupoPessoas?.GerarPedidoColeta == true && p?.GrupoPessoas?.RecebedorColeta != null ? p.GrupoPessoas.RecebedorColeta.Codigo : 0),
                                     DescricaoRecebedorColeta = p.GerarPedidoColeta == true && p.RecebedorColeta != null ? p.RecebedorColeta.Descricao : (p?.GrupoPessoas?.GerarPedidoColeta == true && p?.GrupoPessoas?.RecebedorColeta != null ? p.GrupoPessoas.RecebedorColeta.Descricao : string.Empty),
                                     CodigoTransportador = p.Empresa != null ? p.Empresa.Codigo : (p?.GrupoPessoas?.Empresa != null ? p.GrupoPessoas.Empresa.Codigo : 0),
                                     DescricaoTransportador = p.Empresa != null ? p.Empresa.Descricao : (p?.GrupoPessoas?.Empresa != null ? p.GrupoPessoas.Empresa.Descricao : string.Empty),
                                     Ordem = 0,
                                     FormaTituloFornecedor = p.FormaTituloFornecedor ?? p.GrupoPessoas?.FormaTituloFornecedor ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Outros,
                                     TempoMedioPermanenciaFronteira = $"{p.TempoMedioPermanenciaFronteira / 60:D2}:{p.TempoMedioPermanenciaFronteira % 60:D2}",
                                     TempoMedioPermanenciaFronteiraMinutos = p.TempoMedioPermanenciaFronteira,
                                     TempoCarregamento = p.TempoCarregamento,
                                     TempoDescarregamento = p.TempoDescarregamento,
                                     DescricaoPais = p.Localidade?.Pais?.Descricao ?? string.Empty,
                                     AbreviacaoPais = p.Localidade?.Pais?.Abreviacao ?? string.Empty,
                                     PossuiOutrosEnderecos = p.Enderecos?.Count > 0 ? true : false,
                                     CodigoOutroEndereco = 0,
                                     DescricaoOutroEndereco = "",
                                     GerarAgendamentoPedidosExistentes = p.Modalidades.Any(obj => obj.TipoModalidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor && obj.ModalidadesFornecedores.Any(o => o.GerarAgendamentoSomentePedidosExistentes)),
                                     p.ObservacaoInterna,
                                     p.Bairro,
                                     p.Complemento,
                                     p.Telefone1,
									 CondicaoPagamentoCodigo = p.CondicaoPagamentoPadrao?.Codigo ?? 0,
									 CondicaoPagamentoDescricao = p.CondicaoPagamentoPadrao?.Descricao ?? string.Empty,
                                     CondicaoPagamentoQuantidadeParcelas = p.CondicaoPagamentoPadrao?.QuantidadeParcelas ?? 0,
									 CondicaoPagamentoIntervaloDias = p.CondicaoPagamentoPadrao?.IntervaloDias ?? string.Empty
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
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaTomadorCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                string nome = Request.Params("Nome");
                string cpfcnpj = Utilidades.String.OnlyNumbers(Request.Params("CPF_CNPJ"));
                int.TryParse(Request.Params("Carga"), out int carga);

                if (string.IsNullOrWhiteSpace(cpfcnpj))
                {
                    string cpfCnpjAux = Utilidades.String.OnlyNumbers(nome);
                    if (!string.IsNullOrWhiteSpace(cpfCnpjAux) && (Utilidades.Validate.ValidarCNPJ(cpfCnpjAux) || Utilidades.Validate.ValidarCPF(cpfCnpjAux)))
                    {
                        cpfcnpj = cpfCnpjAux;
                        nome = string.Empty;
                    }
                }

                double dCPFCNPJ = 0;
                if (!string.IsNullOrEmpty(cpfcnpj))
                    dCPFCNPJ = Double.Parse(cpfcnpj);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.RazaoSocial, "Nome", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.CPFCNPJ, "CPF_CNPJ", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.Endereco, "Endereco", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Cidade, "Localidade", 18, Models.Grid.Align.left, false, false, true);
                grid.AdicionarCabecalho("CodigoLocalidade", false);
                grid.AdicionarCabecalho("CodigoAtividade", false);
                grid.AdicionarCabecalho("Atividade", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("IE_RG", false);
                grid.AdicionarCabecalho("Estado", false);
                grid.AdicionarCabecalho("Email", false);
                grid.AdicionarCabecalho("CodigoGrupo", false);
                grid.AdicionarCabecalho("DescricaoGrupo", false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenacao == "Codigo")
                    propOrdenacao = "CPF_CNPJ";

                bool destinatariosDaCarga = !ConfiguracaoEmbarcador.ChamadoOcorrenciaUsaRemetente;

                int totalRegistros = 0;
                List<Dominio.Entidades.Cliente> listaClientes = null;
                if (ConfiguracaoEmbarcador.FiltrarCargasSemDocumentosParaChamados)
                {
                    listaClientes = repCargaPedido.ConsultarTomadorCarga(destinatariosDaCarga, carga, nome, dCPFCNPJ, "", null, "", 0, grid.inicio, grid.limite, propOrdenacao, grid.dirOrdena);
                    totalRegistros = repCargaPedido.ContarConsultarTomadorCarga(destinatariosDaCarga, carga, nome, dCPFCNPJ, "", null, "", 0);
                }
                else
                {
                    listaClientes = repCargaCTe.ConsultarTomadorCarga(destinatariosDaCarga, carga, nome, dCPFCNPJ, "", null, "", 0, grid.inicio, grid.limite, propOrdenacao, grid.dirOrdena);
                    totalRegistros = repCargaCTe.ContarConsultarTomadorCarga(destinatariosDaCarga, carga, nome, dCPFCNPJ, "", null, "", 0);
                }

                grid.setarQuantidadeTotal(totalRegistros);

                dynamic lista = (from p in listaClientes
                                 select new
                                 {
                                     Codigo = p.CPF_CNPJ,
                                     p.Nome,
                                     p.NomeFantasia,
                                     CPF_CNPJ = p.CPF_CNPJ_Formatado,
                                     p.Endereco,
                                     p.Descricao,
                                     Localidade = p.Localidade?.DescricaoCidadeEstado ?? string.Empty,
                                     CodigoLocalidade = p.Localidade?.Codigo ?? 0,
                                     CodigoAtividade = p.Atividade?.Codigo ?? 0,
                                     Atividade = p.Atividade?.Descricao ?? string.Empty,
                                     p.IE_RG,
                                     Estado = p.Localidade?.Estado?.Sigla ?? string.Empty,
                                     p.Email,
                                     CodigoGrupo = p.GrupoPessoas?.Codigo ?? 0,
                                     DescricaoGrupo = p.GrupoPessoas?.Descricao ?? string.Empty
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
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaDestinatarioCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                string nome = Request.Params("Nome");
                string cpfcnpj = Utilidades.String.OnlyNumbers(Request.Params("CPF_CNPJ"));
                int.TryParse(Request.Params("Carga"), out int carga);

                if (string.IsNullOrWhiteSpace(cpfcnpj))
                {
                    string cpfCnpjAux = Utilidades.String.OnlyNumbers(nome);
                    if (!string.IsNullOrWhiteSpace(cpfCnpjAux) && (Utilidades.Validate.ValidarCNPJ(cpfCnpjAux) || Utilidades.Validate.ValidarCPF(cpfCnpjAux)))
                    {
                        cpfcnpj = cpfCnpjAux;
                        nome = string.Empty;
                    }
                }

                double dCPFCNPJ = 0;
                if (!string.IsNullOrEmpty(cpfcnpj))
                    dCPFCNPJ = Double.Parse(cpfcnpj);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.RazaoSocial, "Nome", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.CPFCNPJ, "CPF_CNPJ", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.Endereco, "Endereco", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Cidade, "Localidade", 18, Models.Grid.Align.left, false, false, true);
                grid.AdicionarCabecalho("CodigoLocalidade", false);
                grid.AdicionarCabecalho("CodigoAtividade", false);
                grid.AdicionarCabecalho("Atividade", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("IE_RG", false);
                grid.AdicionarCabecalho("Estado", false);
                grid.AdicionarCabecalho("Email", false);
                grid.AdicionarCabecalho("CodigoGrupo", false);
                grid.AdicionarCabecalho("DescricaoGrupo", false);
                grid.AdicionarCabecalho("CodigoBanco", false);
                grid.AdicionarCabecalho("Banco", false);
                grid.AdicionarCabecalho("Agencia", false);
                grid.AdicionarCabecalho("NumeroConta", false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenacao == "Codigo")
                    propOrdenacao = "CPF_CNPJ";

                bool destinatariosDaCarga = !ConfiguracaoEmbarcador.ChamadoOcorrenciaUsaRemetente;
                bool buscarPorCargaOrigem = !ConfiguracaoEmbarcador.GerarOcorrenciaParaCargaAgrupada;

                List<Dominio.Entidades.Cliente> listaClientes = null;
                int totalRegistros = 0;
                if (ConfiguracaoEmbarcador.FiltrarCargasSemDocumentosParaChamados)
                {
                    listaClientes = repCargaPedido.ConsultarDestinatarioCarga(destinatariosDaCarga, carga, nome, dCPFCNPJ, "", null, "", 0, buscarPorCargaOrigem, grid.inicio, grid.limite, propOrdenacao, grid.dirOrdena);
                    totalRegistros = repCargaPedido.ContarConsultarDestinatarioCarga(destinatariosDaCarga, carga, nome, dCPFCNPJ, "", null, "", 0, buscarPorCargaOrigem);
                }
                else
                {
                    listaClientes = repCargaCTe.ConsultarDestinatarioCarga(destinatariosDaCarga, carga, nome, dCPFCNPJ, "", null, "", 0, buscarPorCargaOrigem, grid.inicio, grid.limite, propOrdenacao, grid.dirOrdena);
                    totalRegistros = repCargaCTe.ContarConsultarDestinatarioCarga(destinatariosDaCarga, carga, nome, dCPFCNPJ, "", null, "", 0, buscarPorCargaOrigem);
                }

                grid.setarQuantidadeTotal(totalRegistros);

                dynamic lista = (from p in listaClientes
                                 select new
                                 {
                                     Codigo = p.CPF_CNPJ,
                                     p.Nome,
                                     p.NomeFantasia,
                                     CPF_CNPJ = p.CPF_CNPJ_Formatado,
                                     p.Endereco,
                                     p.Descricao,
                                     Localidade = p.Localidade?.DescricaoCidadeEstado ?? string.Empty,
                                     CodigoLocalidade = p.Localidade?.Codigo ?? 0,
                                     CodigoAtividade = p.Atividade?.Codigo ?? 0,
                                     Atividade = p.Atividade?.Descricao ?? string.Empty,
                                     p.IE_RG,
                                     Estado = p.Localidade?.Estado?.Sigla ?? string.Empty,
                                     p.Email,
                                     CodigoGrupo = p.GrupoPessoas?.Codigo ?? 0,
                                     DescricaoGrupo = p.GrupoPessoas?.Descricao ?? string.Empty,
                                     CodigoBanco = p.Banco?.Codigo ?? 0,
                                     Banco = p.Banco?.Descricao ?? string.Empty,
                                     p.Agencia,
                                     p.NumeroConta
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
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                string nome = Request.Params("Nome");
                string cpfcnpj = Utilidades.String.OnlyNumbers(Request.Params("CPF_CNPJ"));
                int.TryParse(Request.Params("Carga"), out int carga);

                bool cargaOrigem = Request.GetBoolParam("CargaOrigem");

                if (string.IsNullOrWhiteSpace(cpfcnpj))
                {
                    string cpfCnpjAux = Utilidades.String.OnlyNumbers(nome);
                    if (!string.IsNullOrWhiteSpace(cpfCnpjAux) && (Utilidades.Validate.ValidarCNPJ(cpfCnpjAux) || Utilidades.Validate.ValidarCPF(cpfCnpjAux)))
                    {
                        cpfcnpj = cpfCnpjAux;
                        nome = string.Empty;
                    }
                }

                double dCPFCNPJ = 0;
                if (!string.IsNullOrEmpty(cpfcnpj))
                    dCPFCNPJ = double.Parse(cpfcnpj);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.RazaoSocial, "Nome", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.CPFCNPJ, "CPF_CNPJ", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.Endereco, "Endereco", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Cidade, "Localidade", 18, Models.Grid.Align.left, false, false, true);
                grid.AdicionarCabecalho("CodigoLocalidade", false);
                grid.AdicionarCabecalho("CodigoAtividade", false);
                grid.AdicionarCabecalho("Atividade", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("IE_RG", false);
                grid.AdicionarCabecalho("Estado", false);
                grid.AdicionarCabecalho("Email", false);
                grid.AdicionarCabecalho("CodigoGrupo", false);
                grid.AdicionarCabecalho("DescricaoGrupo", false);
                grid.AdicionarCabecalho("CodigoBanco", false);
                grid.AdicionarCabecalho("Banco", false);
                grid.AdicionarCabecalho("Agencia", false);
                grid.AdicionarCabecalho("NumeroConta", false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenacao == "Codigo")
                    propOrdenacao = "CPF_CNPJ";

                bool destinatariosDaCarga = !ConfiguracaoEmbarcador.ChamadoOcorrenciaUsaRemetente;

                List<Dominio.Entidades.Cliente> listaClientes = repCarga.ConsultarClientesCarga(cargaOrigem, destinatariosDaCarga, carga, nome, dCPFCNPJ, "", null, "", 0, grid.inicio, grid.limite, propOrdenacao, grid.dirOrdena);
                int totalRegistros = repCarga.ContarClientesCarga(cargaOrigem, destinatariosDaCarga, carga, nome, dCPFCNPJ, "", null, "", 0);

                grid.setarQuantidadeTotal(totalRegistros);

                dynamic lista = (from p in listaClientes
                                 select new
                                 {
                                     Codigo = p.CPF_CNPJ,
                                     p.Nome,
                                     p.NomeFantasia,
                                     CPF_CNPJ = p.CPF_CNPJ_Formatado,
                                     p.Endereco,
                                     p.Descricao,
                                     Localidade = p.Localidade?.DescricaoCidadeEstado ?? string.Empty,
                                     CodigoLocalidade = p.Localidade?.Codigo ?? 0,
                                     CodigoAtividade = p.Atividade?.Codigo ?? 0,
                                     Atividade = p.Atividade?.Descricao ?? string.Empty,
                                     p.IE_RG,
                                     Estado = p.Localidade?.Estado?.Sigla ?? string.Empty,
                                     p.Email,
                                     CodigoGrupo = p.GrupoPessoas?.Codigo ?? 0,
                                     DescricaoGrupo = p.GrupoPessoas?.Descricao ?? string.Empty,
                                     CodigoBanco = p.Banco?.Codigo ?? 0,
                                     Banco = p.Banco?.Descricao ?? string.Empty,
                                     p.Agencia,
                                     p.NumeroConta
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
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaRemetenteDestinatarioCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                string nome = Request.GetStringParam("Nome");
                string cpfcnpj = Utilidades.String.OnlyNumbers(Request.GetStringParam("CPF_CNPJ"));
                int carga = Request.GetIntParam("Carga");

                bool cargaOrigem = Request.GetBoolParam("CargaOrigem");

                if (string.IsNullOrWhiteSpace(cpfcnpj))
                {
                    string cpfCnpjAux = Utilidades.String.OnlyNumbers(nome);
                    if (!string.IsNullOrWhiteSpace(cpfCnpjAux) && (Utilidades.Validate.ValidarCNPJ(cpfCnpjAux) || Utilidades.Validate.ValidarCPF(cpfCnpjAux)))
                    {
                        cpfcnpj = cpfCnpjAux;
                        nome = string.Empty;
                    }
                }

                double dCPFCNPJ = 0;
                if (!string.IsNullOrEmpty(cpfcnpj))
                    dCPFCNPJ = double.Parse(cpfcnpj);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.RazaoSocial, "Nome", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.CPFCNPJ, "CPF_CNPJ", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.Cliente.Endereco, "Endereco", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Cidade, "Localidade", 18, Models.Grid.Align.left, false, false, true);
                grid.AdicionarCabecalho("Descricao", false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenacao == "Codigo")
                    propOrdenacao = "CPF_CNPJ";

                //Faz as buscas separadas de Remetente e Destinatário com base no método PesquisaCarga(), devido não ser possível usar Union via linq
                List<Dominio.Entidades.Cliente> listaClientes = new List<Dominio.Entidades.Cliente>();
                int totalRegistros = 0;

                //Remetentes
                listaClientes.AddRange(repCarga.ConsultarClientesCarga(cargaOrigem, false, carga, nome, dCPFCNPJ, "", null, "", 0, grid.inicio, grid.limite, propOrdenacao, grid.dirOrdena));
                totalRegistros += repCarga.ContarClientesCarga(cargaOrigem, false, carga, nome, dCPFCNPJ, "", null, "", 0);

                //Destinatários
                listaClientes.AddRange(repCarga.ConsultarClientesCarga(cargaOrigem, true, carga, nome, dCPFCNPJ, "", null, "", 0, grid.inicio, grid.limite, propOrdenacao, grid.dirOrdena));
                totalRegistros += repCarga.ContarClientesCarga(cargaOrigem, true, carga, nome, dCPFCNPJ, "", null, "", 0);

                grid.setarQuantidadeTotal(totalRegistros);

                dynamic lista = (from p in listaClientes
                                 select new
                                 {
                                     Codigo = p.CPF_CNPJ,
                                     p.Nome,
                                     CPF_CNPJ = p.CPF_CNPJ_Formatado,
                                     p.Endereco,
                                     p.Descricao,
                                     Localidade = p.Localidade?.DescricaoCidadeEstado ?? string.Empty
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
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesPorCPFCNPJ()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("CPF_CNPJ")), out double cpfcnpj);
                double codigo = Request.GetDoubleParam("Codigo");


                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Embarcador.Pessoas.ModalidadePessoas repModalidadePessoas = new Repositorio.Embarcador.Pessoas.ModalidadePessoas(unidadeDeTrabalho);
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unidadeDeTrabalho);
                Repositorio.Cliente repGrupoPessoas = new Repositorio.Cliente(unidadeDeTrabalho);

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(codigo > 0 ? codigo : cpfcnpj);

                if (cliente == null)
                    return new JsonpResult(true, "Pessoa não encontrada.");

                Dominio.Entidades.Embarcador.Pessoas.ModalidadePessoas modalidadePessoas = repModalidadePessoas.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.TransportadorTerceiro, cliente.CPF_CNPJ);

                modalidadePessoas = repModalidadePessoas.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalidade.Fornecedor, cliente.CPF_CNPJ);
                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedorPessoas = null;
                if (modalidadePessoas != null)
                    modalidadeFornecedorPessoas = repModalidadeFornecedorPessoas.BuscarPorModalidade(modalidadePessoas.Codigo);

                var retorno = new
                {
                    Codigo = cliente.CPF_CNPJ,
                    CPFCNPJ = cliente.CPF_CNPJ_SemFormato,
                    IE = cliente.IE_RG,
                    RazaoSocial = cliente.Nome,
                    NomeFantasia = cliente.NomeFantasia,
                    TelefonePrincipal = cliente.Telefone1,
                    TelefoneSecundario = cliente.Telefone2,
                    Atividade = new
                    {
                        Codigo = cliente.Atividade?.Codigo,
                        Descricao = cliente.Atividade?.Descricao
                    },
                    CEP = cliente.CEP,
                    Endereco = cliente.Endereco,
                    Numero = cliente.Numero,
                    Bairro = cliente.Bairro,
                    Complemento = cliente.Complemento,
                    Localidade = new
                    {
                        Codigo = cliente.Localidade.Codigo,
                        Descricao = cliente.Localidade.DescricaoCidadeEstado,
                        Estado = cliente.Localidade.Estado.Sigla
                    },
                    LocalidadeExterior = cliente.Localidade.Descricao,
                    Pais = new
                    {
                        Codigo = cliente.Localidade.Pais?.Codigo ?? 0,
                        Descricao = cliente.Localidade.Pais?.Descricao ?? string.Empty
                    },
                    EmailGeral = cliente.Email,
                    EnviarXMLEmailGeral = cliente.EmailStatus == "A" ? true : false,
                    EmailContato = cliente.EmailContato,
                    EnviarXMLEmailContato = cliente.EmailContatoStatus == "A" ? true : false,
                    EmailContador = cliente.EmailContador,
                    EnviarXMLEmailContador = cliente.EmailContadorStatus == "A" ? true : false,
                    ObrigarLocalArmazenamentoNoLancamentoDeAbastecimento = modalidadeFornecedorPessoas?.ObrigarLocalArmazenamentoNoLancamentoDeAbastecimento ?? false
                };

                return new JsonpResult(retorno);
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarLocalidadeCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                double cpfcnpj = double.Parse(Request.Params("Codigo"));
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfcnpj);

                var dynLocalidade = new
                {
                    CPFCNPJ = cliente.CPF_CNPJ,
                    Localidade = new
                    {
                        cliente.Localidade.Codigo,
                        cliente.Localidade.CodigoIBGE,
                        cliente.Localidade.CodigoLocalidadeEmbarcador,
                        cliente.Localidade.Descricao,
                        cliente.Localidade.DescricaoCidadeEstado,
                        Estado = new
                        {
                            cliente.Localidade.Estado.Sigla,
                            cliente.Localidade.Estado.Nome
                        },
                        Pais = cliente.Localidade.Pais != null ? new
                        {
                            cliente.Localidade.Pais.Codigo,
                            cliente.Localidade.Pais.Abreviacao,
                            cliente.Localidade.Pais.Nome,
                            cliente.Localidade.Pais.Sigla
                        } : null,
                        LocalidadePolo = cliente.Localidade.LocalidadePolo != null ? new
                        {
                            cliente.Localidade.LocalidadePolo.Codigo,
                            cliente.Localidade.LocalidadePolo.CodigoIBGE,
                            cliente.Localidade.LocalidadePolo.CodigoLocalidadeEmbarcador,
                            cliente.Localidade.LocalidadePolo.Descricao,
                            cliente.Localidade.LocalidadePolo.DescricaoCidadeEstado,
                            Estado = new
                            {
                                cliente.Localidade.LocalidadePolo.Estado.Sigla,
                                cliente.Localidade.LocalidadePolo.Estado.Nome
                            },
                            Pais = cliente.Localidade.LocalidadePolo.Pais != null ? new
                            {
                                cliente.Localidade.LocalidadePolo.Pais.Codigo,
                                cliente.Localidade.LocalidadePolo.Pais.Abreviacao,
                                cliente.Localidade.LocalidadePolo.Pais.Nome,
                                cliente.Localidade.LocalidadePolo.Pais.Sigla
                            } : null
                        } : null
                    },
                    cliente.Bairro,
                    cliente.CEP,
                    cliente.Cidade,
                    cliente.Complemento,
                    cliente.Endereco,
                    cliente.Telefone1,
                    cliente.Telefone2,
                    cliente.Numero
                };

                return new JsonpResult(dynLocalidade);
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
        public async Task<IActionResult> BuscarFiliaisRelacionadas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<double> cpfCnpjClientes = Request.GetListParam<double>("Clientes");
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                List<Dominio.Entidades.Cliente> clientes = repositorioCliente.BuscarPorCPFCNPJ(cpfCnpjClientes);
                List<Dominio.Entidades.Cliente> clientesComFiliaisCliente = new Servicos.Cliente().ObterFiliaisClientesRelacionadas(clientes, unitOfWork);

                dynamic clientesRetornar = (
                    from cliente in clientesComFiliaisCliente
                    select new
                    {
                        Codigo = cliente.Codigo,
                        Descricao = cliente.Descricao
                    }
                ).ToList();

                return new JsonpResult(clientesRetornar);
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
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params("CPF_CNPJ")), out double cpfCNPJ);

                Dominio.Entidades.Cliente clienteExiste = repCliente.BuscarPorCPFCNPJ(cpfCNPJ);

                if (clienteExiste != null)
                {
                    string tipo = clienteExiste.Tipo == "J" ? "CNPJ" : "CPF";
                    return new JsonpResult(false, true, "Já existe um cliente cadastrado com esse " + tipo + " (" + clienteExiste.Nome + ")");
                }

                Dominio.Entidades.Cliente cliente = new Dominio.Entidades.Cliente();

                int.TryParse(Request.Params("Localidade"), out int codigoLocalidade);
                int.TryParse(Request.Params("Atividade"), out int codigoAtividade);
                int codigoGrupoPessoas = Request.GetIntParam("GrupoPessoas");

                string nome = Request.Params("Nome");
                string endereco = Request.Params("Endereco");
                string cep = Request.Params("CEP");
                string telefonePrincipal = Request.Params("TelefonePrincipal");
                string telefoneSecundario = Request.Params("TelefoneSecundario");
                string numero = Request.Params("Numero");
                string bairro = Request.Params("Bairro");
                string complemento = Request.Params("Complemento");
                string email = Request.Params("Email");

                bool.TryParse(Request.Params("EnviarEmail"), out bool enviarEmail);
                bool.TryParse(Request.Params("PossuiRestricaoTrafego"), out bool possuiRestricaoTrafego);
                bool.TryParse(Request.Params("EnderecoDigitado"), out bool enderecoDigitado);

                Enum.TryParse(Request.Params("TipoLogradouro"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogradouro tipoLogradouro);
                Enum.TryParse(Request.Params("TipoPessoa"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoaCadastro tipoPessoa);

                if (tipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoaCadastro.Fisica)
                {
                    if (!Utilidades.Validate.ValidarCPF(string.Format("{0:00000000000}", cpfCNPJ)))
                        return new JsonpResult(false, true, "O CPF é inválido.");

                    cliente.Tipo = "F";
                }
                else if (tipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoaCadastro.Juridica)
                {
                    if (!Utilidades.Validate.ValidarCNPJ(string.Format("{0:00000000000000}", cpfCNPJ)))
                        return new JsonpResult(false, true, "O CNPJ é inválido.");

                    cliente.Tipo = "J";
                }
                else
                    return new JsonpResult(false, true, "O tipo de pessoa é inválido.");

                if (string.IsNullOrWhiteSpace(nome) || nome.Length <= 3)
                    return new JsonpResult(false, true, "O nome informado é inválido (deve possuir no mínimo 3 caracteres).");

                cliente.CPF_CNPJ = cpfCNPJ;
                cliente.Localidade = repLocalidade.BuscarPorCodigo(codigoLocalidade);
                cliente.Nome = nome;
                cliente.Atividade = repAtividade.BuscarPorCodigo(codigoAtividade);
                cliente.DataCadastro = DateTime.Now;
                cliente.NomeFantasia = "";
                cliente.Telefone1 = telefonePrincipal;
                cliente.Telefone2 = telefoneSecundario;
                cliente.Endereco = endereco;
                cliente.EnderecoDigitado = enderecoDigitado;
                cliente.Numero = numero;
                cliente.PossuiRestricaoTrafego = possuiRestricaoTrafego;
                cliente.TipoLogradouro = tipoLogradouro;
                cliente.Complemento = complemento;
                cliente.Bairro = bairro;
                cliente.Email = email;
                cliente.EmailStatus = enviarEmail ? "A" : "I";
                cliente.CEP = cep;
                cliente.Ativo = true;
                cliente.NomeSocio = Request.GetStringParam("NomeSocio");
                cliente.CPFSocio = Utilidades.String.OnlyNumbers(Request.GetStringParam("CPFSocio"));
                cliente.DataNascimento = Request.GetNullableDateTimeParam("DataNascimento");
                cliente.GrupoPessoas = codigoGrupoPessoas > 0 ? repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoas) : null;

                if (!string.IsNullOrWhiteSpace(cliente.CPFSocio) && !Utilidades.Validate.ValidarCPF(cliente.CPFSocio))
                    throw new ControllerException("CPF do Sócio é inválido");

                repCliente.Inserir(cliente, Auditado);

                var dynCliente = new
                {
                    Codigo = cliente.CPF_CNPJ,
                    Nome = cliente.Nome
                };

                return new JsonpResult(dynCliente);
            }
            catch (ControllerException ex)
            {
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private List<double> RetornaCodigosCNPJ()
        {
            List<double> listaCNPJ = new List<double>();
            if (!string.IsNullOrWhiteSpace(Request.Params("ListaCNPJConsultar")))
            {
                dynamic listaCliente = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaCNPJConsultar"));
                if (listaCliente != null)
                {
                    foreach (var cnpj in listaCliente)
                    {
                        listaCNPJ.Add(double.Parse((string)cnpj.CodigoFornecedor));
                    }
                }
            }
            return listaCNPJ;
        }

        #endregion
    }
}

