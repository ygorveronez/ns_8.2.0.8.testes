using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Alcadas
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Cargas/AutorizacaoCargaCancelamento")]
    public class AutorizacaoCargaCancelamentoController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.AprovacaoAlcadaCargaCancelamento,
        Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.RegraAutorizacaoCargaCancelamento,
        Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao
    >
    {
		#region Construtores

		public AutorizacaoCargaCancelamentoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		public async Task<IActionResult> ReprocessarCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                int codigoCargaCancelamentoSolicitacao = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao repositorioCargaCancelamentoSolicitacao = new Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao cargaCancelamentoSolicitacao = repositorioCargaCancelamentoSolicitacao.BuscarPorCodigo(codigoCargaCancelamentoSolicitacao, auditavel: false);

                if (cargaCancelamentoSolicitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (cargaCancelamentoSolicitacao.Situacao != SituacaoCargaCancelamentoSolicitacao.SemRegraAprovacao)
                    return new JsonpResult(new { RegraReprocessada = true });

                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
                Servicos.Embarcador.Carga.CargaCancelamentoAprovacao servicoCargaCancelamentoAprovacao = new Servicos.Embarcador.Carga.CargaCancelamentoAprovacao(unitOfWork);

                servicoCargaCancelamentoAprovacao.CriarAprovacao(cargaCancelamentoSolicitacao, TipoServicoMultisoftware);
                repositorioCargaCancelamento.Atualizar(cargaCancelamentoSolicitacao.CargaCancelamento);
                repositorioCargaCancelamentoSolicitacao.Atualizar(cargaCancelamentoSolicitacao);


                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegraReprocessada = cargaCancelamentoSolicitacao.Situacao != SituacaoCargaCancelamentoSolicitacao.SemRegraAprovacao });
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
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarMultiplasCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                List<int> codigosCargaCancelamentoSolicitacao = ObterCodigosOrigensSelecionadas(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao repositorioCargaCancelamentoSolicitacao = new Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao> listaCargaCancelamentoSolicitacao = repositorioCargaCancelamentoSolicitacao.BuscarSemRegraAprovacaoPorCodigos(codigosCargaCancelamentoSolicitacao);
                Servicos.Embarcador.Carga.CargaCancelamentoAprovacao servicoCargaCancelamentoAprovacao = new Servicos.Embarcador.Carga.CargaCancelamentoAprovacao(unitOfWork);
                int totalRegrasReprocessadas = 0;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao cargaCancelamentoSolicitacao in listaCargaCancelamentoSolicitacao)
                {
                    servicoCargaCancelamentoAprovacao.CriarAprovacao(cargaCancelamentoSolicitacao, TipoServicoMultisoftware);

                    if (cargaCancelamentoSolicitacao.Situacao != SituacaoCargaCancelamentoSolicitacao.SemRegraAprovacao)
                    {
                        repositorioCargaCancelamento.Atualizar(cargaCancelamentoSolicitacao.CargaCancelamento);
                        repositorioCargaCancelamentoSolicitacao.Atualizar(cargaCancelamentoSolicitacao);
                        totalRegrasReprocessadas++;
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegrasReprocessadas = totalRegrasReprocessadas });
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
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar as cargas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Globais Sobrescritos

        public override IActionResult BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao repositorio = new Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao cargaCancelamentoSolicitacao = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (cargaCancelamentoSolicitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaCancelamentoSolicitacao.CargaCancelamento.Carga;
                bool exibirDetalhesFreteCarga = false;
                string informacaoTipoFreteEscolhido = "";
                string valorFrete = carga.ValorFreteAPagar.ToString("n2");

                if (carga.ValorFreteAPagar > 0)
                {
                    exibirDetalhesFreteCarga = true;

                    if ((TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS) && ((carga.SituacaoCarga == SituacaoCarga.Nova) || (carga.SituacaoCarga == SituacaoCarga.AgNFe)))
                        informacaoTipoFreteEscolhido = "(Prévia do Frete)";
                    else if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Operador)
                        informacaoTipoFreteEscolhido = "(Valor informado pelo Operador)";
                    else if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Embarcador)
                        informacaoTipoFreteEscolhido = "(Valor informado pelo embarcador)";
                }
                else if (carga.TipoFreteEscolhido == TipoFreteEscolhido.Cliente)
                {
                    informacaoTipoFreteEscolhido = "Frete por Conta do Cliente";
                    valorFrete = "";
                }
                else
                    valorFrete = "Pendente";

                return new JsonpResult(new
                {
                    cargaCancelamentoSolicitacao.Codigo,
                    CodigoCarga = carga.Codigo,
                    carga.CodigoCargaEmbarcador,
                    ExibirDetalhesFreteCarga = exibirDetalhesFreteCarga,
                    Filial = carga.Filial?.Descricao,
                    InformacaoTipoFreteEscolhido = informacaoTipoFreteEscolhido,
                    carga.TipoFreteEscolhido,
                    ModeloVeicularCarga = carga.ModeloVeicularCarga?.Descricao,
                    Motoristas = carga.NomeMotoristas,
                    Operador = carga.Operador?.Descricao,
                    Peso = carga.DadosSumarizados?.PesoTotal.ToString("n2") ?? "",
                    Placas = carga.PlacasVeiculos,
                    Rota = carga.Rota?.Descricao,
                    Situacao = carga.SituacaoCarga.ObterDescricao(),
                    SituacaoCargaCancelamentoSolicitacao = cargaCancelamentoSolicitacao.Situacao,
                    TipoCarga = carga.TipoDeCarga?.Descricao,
                    TipoOperacao = carga.TipoOperacao?.Descricao,
                    Transportador = carga.Empresa?.Descricao,
                    ValorFrete = valorFrete,
                    PortoOrigem = carga.PortoOrigem?.Descricao,
                    PortoDestino = carga.PortoDestino?.Descricao,
                    Tomador = carga.DadosSumarizados?.Tomadores ?? "",
                    MotivoCancelamento = cargaCancelamentoSolicitacao.CargaCancelamento?.MotivoCancelamento ?? string.Empty,
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCancelamentoAprovacao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCancelamentoAprovacao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCancelamentoAprovacao()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigosFilial = Request.GetListParam<int>("Filial"),
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                SituacaoCargaCancelamentoSolicitacao = Request.GetNullableEnumParam<SituacaoCargaCancelamentoSolicitacao>("SituacaoCargaCancelamentoSolicitacao"),
                CpfCnpjTomador = Request.GetDoubleParam("Tomador"),
                CodigoPortoDestino = Request.GetIntParam("PortoDestino"),
                CodigoPortoOrigem = Request.GetIntParam("PortoOrigem"),
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                filtrosPesquisa.CodigoTransportador = Usuario.Empresa?.Codigo ?? 0;
                filtrosPesquisa.CodigoUsuario = Usuario.Codigo;
                filtrosPesquisa.TipoAprovadorRegra = TipoAprovadorRegra.Transportador;
            }
            else
                filtrosPesquisa.TipoAprovadorRegra = TipoAprovadorRegra.Usuario;

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Filial")
                return "CargaCancelamento.Carga.Filial.Descricao";

            if (propriedadeOrdenar == "ModeloVeicular")
                return "CargaCancelamento.Carga.ModeloVeicularCarga.Descricao";

            if (propriedadeOrdenar == "TipoCarga")
                return "CargaCancelamento.Carga.TipoDeCarga.Descricao";

            if (propriedadeOrdenar == "PortoOrigem")
                return "CargaCancelamento.Carga.PortoOrigem.Descricao";

            if (propriedadeOrdenar == "PortoDestino")
                return "CargaCancelamento.Carga.PortoDestino.Descricao";

            if (propriedadeOrdenar == "Transportador")
                return "CargaCancelamento.Carga.Empresa.RazaoSocial";

            return propriedadeOrdenar;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao origem)
        {
            return origem.Situacao == SituacaoCargaCancelamentoSolicitacao.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao> cargasCancelamentoSolicitacao;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCancelamentoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Cargas.AlcadasCargaCancelamento.AprovacaoAlcadaCargaCancelamento repositorioAprovacaoAlcadaCarga = new Repositorio.Embarcador.Cargas.AlcadasCargaCancelamento.AprovacaoAlcadaCargaCancelamento(unitOfWork);

                cargasCancelamentoSolicitacao = repositorioAprovacaoAlcadaCarga.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    cargasCancelamentoSolicitacao.Remove(new Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao repositorioCargaCancelamentoSolicitacao = new Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                cargasCancelamentoSolicitacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    cargasCancelamentoSolicitacao.Add(repositorioCargaCancelamentoSolicitacao.BuscarPorCodigo((int)itemSelecionado.Codigo, auditavel: false));
            }

            return (from cargaCancelamentoSolicitacao in cargasCancelamentoSolicitacao select cargaCancelamentoSolicitacao.Codigo).ToList();
        }

        protected override Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                if (!ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal)
                {
                    grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                    grid.AdicionarCabecalho(descricao: "Nº da Carga", propriedade: "CodigoCargaEmbarcador", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Tipo de Carga", propriedade: "TipoCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Modelo Veícular", propriedade: "ModeloVeicular", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                    if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        grid.AdicionarCabecalho(descricao: "CT-e", propriedade: "NumeroCTes", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                        grid.AdicionarCabecalho(descricao: "Empresa/Filial", propriedade: "Transportador", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    }
                    else
                    {
                        grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                        grid.AdicionarCabecalho(descricao: "Transportador", propriedade: "Transportador", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    }

                    grid.AdicionarCabecalho(descricao: "Veículo", propriedade: "Veiculo", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Motorista", propriedade: "Motorista", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Situação da Solicitação de Cancelamento", propriedade: "SituacaoCargaCancelamentoSolicitacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                }
                else
                {
                    grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                    grid.AdicionarCabecalho(descricao: "Nº da Carga", propriedade: "CodigoCargaEmbarcador", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(propriedade: "TipoCarga", visivel: false);
                    grid.AdicionarCabecalho(propriedade: "ModeloVeicular", visivel: false);
                    grid.AdicionarCabecalho(propriedade: "NumeroCTes", visivel: false);
                    grid.AdicionarCabecalho(propriedade: "Transportador", visivel: false);
                    grid.AdicionarCabecalho(propriedade: "Filial", visivel: false);
                    grid.AdicionarCabecalho(propriedade: "Veiculo", visivel: false);
                    grid.AdicionarCabecalho(propriedade: "Motorista", visivel: false);
                    grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(propriedade: "SituacaoCargaCancelamentoSolicitacao", visivel: false);
                    grid.AdicionarCabecalho(descricao: "Porto de Origem", propriedade: "PortoOrigem", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Porto de Destino", propriedade: "PortoDestino", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Tipo Tomador", propriedade: "TiposTomador", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Remetente(s)", propriedade: "Remetentes", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Destinatário(s)", propriedade: "Destinatarios", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Container(es)", propriedade: "Containeres", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Data Carga", propriedade: "DataCriacaoCarga", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: "Modal da Operação", propriedade: "ModaisCarga", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                }

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaCargaCancelamentoAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Cargas.AlcadasCargaCancelamento.AprovacaoAlcadaCargaCancelamento repositorio = new Repositorio.Embarcador.Cargas.AlcadasCargaCancelamento.AprovacaoAlcadaCargaCancelamento(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao> cargasCancelamentoSolicitacao = (totalRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao>();

                var lista = (
                    from cargaCancelamentoSolicitacao in cargasCancelamentoSolicitacao
                    select new
                    {
                        cargaCancelamentoSolicitacao.Codigo,
                        cargaCancelamentoSolicitacao.CargaCancelamento.Carga.CodigoCargaEmbarcador,
                        Filial = cargaCancelamentoSolicitacao.CargaCancelamento.Carga.Filial?.Descricao,
                        ModeloVeicular = cargaCancelamentoSolicitacao.CargaCancelamento.Carga.ModeloVeicularCarga?.Descricao,
                        Motorista = cargaCancelamentoSolicitacao.CargaCancelamento.Carga.NomeMotoristas,
                        NumeroCTes = cargaCancelamentoSolicitacao.CargaCancelamento.Carga.NumerosCTes,
                        SituacaoCargaCancelamentoSolicitacao = cargaCancelamentoSolicitacao.Situacao.ObterDescricao(),
                        Situacao = cargaCancelamentoSolicitacao.CargaCancelamento.Carga.SituacaoCarga.ObterDescricao(),
                        TipoCarga = cargaCancelamentoSolicitacao.CargaCancelamento.Carga.TipoDeCarga?.Descricao,
                        Transportador = cargaCancelamentoSolicitacao.CargaCancelamento.Carga.Empresa?.RazaoSocial,
                        Veiculo = cargaCancelamentoSolicitacao.CargaCancelamento.Carga.PlacasVeiculos,
                        PortoOrigem = cargaCancelamentoSolicitacao.CargaCancelamento.Carga.PortoOrigem?.Descricao ?? "",
                        PortoDestino = cargaCancelamentoSolicitacao.CargaCancelamento.Carga.PortoDestino?.Descricao ?? "",
                        cargaCancelamentoSolicitacao.CargaCancelamento.Carga.TiposTomador,
                        Remetentes = cargaCancelamentoSolicitacao.CargaCancelamento.Carga.DadosSumarizados?.Remetentes ?? "",
                        Destinatarios = cargaCancelamentoSolicitacao.CargaCancelamento.Carga.DadosSumarizados?.Destinatarios ?? "",
                        cargaCancelamentoSolicitacao.CargaCancelamento.Carga.Containeres,
                        DataCriacaoCarga = cargaCancelamentoSolicitacao.CargaCancelamento.Carga.DataCriacaoCarga.ToString("dd/MM/yyyy"),
                        cargaCancelamentoSolicitacao.CargaCancelamento.Carga.ModaisCarga
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Cargas.CargaCancelamentoSolicitacao origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Situacao != SituacaoCargaCancelamentoSolicitacao.AguardandoAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao repositorioCargaCancelamentoSolicitacao = new Repositorio.Embarcador.Cargas.CargaCancelamentoSolicitacao(unitOfWork);
            Servicos.Embarcador.Carga.CargaCancelamentoAprovacao servicoCargaCancelamentoAprovacao = new Servicos.Embarcador.Carga.CargaCancelamentoAprovacao(unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {

                if (!servicoCargaCancelamentoAprovacao.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                    return;

                origem.Situacao = SituacaoCargaCancelamentoSolicitacao.Aprovada;
                origem.CargaCancelamento.Situacao = SituacaoCancelamentoCarga.EmCancelamento;

                repositorioCargaCancelamento.Atualizar(origem.CargaCancelamento);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem.CargaCancelamento, "Cancelamento da carga aprovado", unitOfWork);
            }
            else
            {
                origem.Situacao = SituacaoCargaCancelamentoSolicitacao.Reprovada;
                origem.CargaCancelamento.Situacao = SituacaoCancelamentoCarga.SolicitacaoReprovada;

                repositorioCargaCancelamento.Atualizar(origem.CargaCancelamento);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem.CargaCancelamento, "Cancelamento da carga reprovado", unitOfWork);
            }

            repositorioCargaCancelamentoSolicitacao.Atualizar(origem);
            servicoCargaCancelamentoAprovacao.NotificarSituacaoAprovacaoAoOperadorCarga(origem, TipoServicoMultisoftware);
        }

        #endregion
    }
}