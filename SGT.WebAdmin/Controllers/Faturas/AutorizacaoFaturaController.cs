using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Frota.OrdemServico
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Faturas/AutorizacaoFatura", "SAC/AtendimentoCliente")]
    public class AutorizacaoFaturaController : RegraAutorizacao.AutorizacaoController<
            Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura,
            Dominio.Entidades.Embarcador.Fatura.AlcadasFatura.RegraAutorizacaoFatura,
            Dominio.Entidades.Embarcador.Fatura.Fatura
      >
    {
        #region Construtores

        public AutorizacaoFaturaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais Sobrescritos

        public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Fatura.Fatura repositorio = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = fatura = repositorio.BuscarPorCodigo(codigo);

                if (fatura == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    fatura.Codigo,
                    Data = fatura.DataFatura.ToString("dd/MM/yyyy"),
                    fatura.Numero,
                    fatura.Situacao,
                    SituacaoDescricao = fatura.Situacao.ObterDescricao(),
                    Operador = fatura.Usuario.Descricao,
                    Pessoa = fatura.Cliente?.Descricao ?? string.Empty
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

        private Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFaturaAprovacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFaturaAprovacao()
            {
                CodigoUsuario = Request.GetIntParam("Usuario"),
                CodigoOperador = Request.GetIntParam("Operador"),
                CpfCnpjFornecedor = Request.GetDoubleParam("Pessoa"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataLimite = Request.GetDateTimeParam("DataLimite"),
                Numero = Request.GetIntParam("Numero"),
                Situacao = Request.GetNullableEnumParam<SituacaoFatura>("Situacao"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? Empresa.Codigo : 0
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Equals("Data"))
                propriedadeOrdenar = "DataProgramada";

            return propriedadeOrdenar;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Fatura.Fatura origem)
        {
            return origem.Situacao == SituacaoFatura.EmFechamento;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Fatura.Fatura> faturas;
            bool selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                dynamic listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFaturaAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura repositorioAprovacaoAlcada = new Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura(unitOfWork);

                faturas = repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    faturas.Remove(new Dominio.Entidades.Embarcador.Fatura.Fatura() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Fatura.Fatura repositorioFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);
                dynamic listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                faturas = new List<Dominio.Entidades.Embarcador.Fatura.Fatura>();

                foreach (var itemSelecionado in listaItensSelecionados)
                {
                    faturas.Add(repositorioFatura.BuscarPorCodigo((int)itemSelecionado.Codigo));
                }
            }

            return (from obj in faturas select obj.Codigo).ToList();
        }

        protected override Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Número", propriedade: "Numero", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Data", propriedade: "Data", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situação", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                Dominio.ObjetosDeValor.Embarcador.Fatura.FiltroPesquisaFaturaAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura repositorio = new Repositorio.Embarcador.Fatura.AlcadasFatura.AprovacaoAlcadaFatura(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Fatura.Fatura> faturas = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Fatura.Fatura>();

                var lista = (
                    from obj in faturas
                    select new
                    {
                        obj.Codigo,
                        obj.Numero,
                        Data = obj.DataFechamento?.ToString("dd/MM/yyyy"),
                        Situacao = obj.Situacao.ObterDescricao()
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

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Fatura.Fatura origem, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Fatura.Fatura servFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork);

            if (origem.Situacao != SituacaoFatura.AguardandoAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Fatura.Fatura repositorioFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                Servicos.Embarcador.Fatura.Fatura servicoFatura = new Servicos.Embarcador.Fatura.Fatura(unitOfWork);

                if (servicoFatura.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                {
                    origem.Situacao = SituacaoFatura.EmFechamento;
                    origem.Etapa = EtapaFatura.Fechamento;

                    repositorioFatura.Atualizar(origem);

                    if (IsPrazoFaturamentoAprovacao(origem))
                        servFatura.LancarParcelaFatura(origem, unitOfWork, ConfiguracaoEmbarcador.UtilizaEmissaoMultimodal, null, DateTime.Now);

                    servFatura.GerarIntegracoesFatura(origem, unitOfWork, TipoServicoMultisoftware, Auditado, ConfiguracaoEmbarcador);
                    servFatura.SalvarTituloVencimentoDocumentoFaturamento(origem, unitOfWork);
                    serCargaDadosSumarizados.AtualizarDadosCTesFaturados(origem.Codigo, unitOfWork);

                }
            }
            else
            {
                origem.Situacao = SituacaoFatura.AprovacaoRejeitada;

                repositorioFatura.Atualizar(origem);
            }
        }

        private bool IsPrazoFaturamentoAprovacao(Dominio.Entidades.Embarcador.Fatura.Fatura origem)
        {
            if (origem.Cliente != null && !origem.Cliente.NaoUsarConfiguracaoFaturaGrupo && origem.GrupoPessoas != null && origem.GrupoPessoas.TipoPrazoFaturamento == TipoPrazoFaturamento.ApartirAprovacaoFatura)
                return true;

            if (origem.Cliente != null && origem.Cliente.TipoPrazoFaturamento == TipoPrazoFaturamento.ApartirAprovacaoFatura)
                return true;


            return false;
        }

        #endregion
    }
}