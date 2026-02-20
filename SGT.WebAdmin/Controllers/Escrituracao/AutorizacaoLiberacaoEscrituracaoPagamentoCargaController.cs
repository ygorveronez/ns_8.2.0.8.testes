using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Escrituracao/AutorizacaoLiberacaoEscrituracaoPagamentoCarga")]
    public class AutorizacaoLiberacaoEscrituracaoPagamentoCargaController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga,
        Dominio.Entidades.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.RegraAutorizacaoLiberacaoEscrituracaoPagamentoCarga,
        Dominio.Entidades.Embarcador.Cargas.Carga
    >
    {
		#region Construtores

		public AutorizacaoLiberacaoEscrituracaoPagamentoCargaController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		public async Task<IActionResult> ReprocessarCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                int codigoCarga = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (carga.SituacaoLiberacaoEscrituracaoPagamentoCarga != SituacaoLiberacaoEscrituracaoPagamentoCarga.SemRegraAprovacao)
                    return new JsonpResult(new { RegraReprocessada = true });

                unitOfWork.Start();

                Servicos.Embarcador.Escrituracao.LiberacaoEscrituracaoPagamento servicoLiberacaoEscrituracaoPagamento = new Servicos.Embarcador.Escrituracao.LiberacaoEscrituracaoPagamento(unitOfWork);

                servicoLiberacaoEscrituracaoPagamento.CriarAprovacao(carga, TipoServicoMultisoftware);
                repositorioCarga.Atualizar(carga);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegraReprocessada = carga.SituacaoLiberacaoEscrituracaoPagamentoCarga != SituacaoLiberacaoEscrituracaoPagamentoCarga.SemRegraAprovacao });
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

        public async Task<IActionResult> ReprocessarMultiplasCargas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, "Você não possui permissão para executar essa ação.");

                unitOfWork.Start();

                List<int> codigosCarga = ObterCodigosOrigensSelecionadas(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> listaCarga = repositorioCarga.BuscarSemRegraAprovacaoLiberacaoEscrituracaoPagamentoPorCodigos(codigosCarga);
                Servicos.Embarcador.Escrituracao.LiberacaoEscrituracaoPagamento servicoLiberacaoEscrituracaoPagamento = new Servicos.Embarcador.Escrituracao.LiberacaoEscrituracaoPagamento(unitOfWork);
                int totalRegrasReprocessadas = 0;

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in listaCarga)
                {
                    servicoLiberacaoEscrituracaoPagamento.CriarAprovacao(carga, TipoServicoMultisoftware);

                    if (carga.SituacaoLiberacaoEscrituracaoPagamentoCarga != SituacaoLiberacaoEscrituracaoPagamentoCarga.SemRegraAprovacao)
                    {
                        repositorioCarga.Atualizar(carga);
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
                Repositorio.Embarcador.Cargas.Carga repositorio = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorio.BuscarPorCodigo(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                bool freteCalculadoPorFilialEmissora = (carga.EmpresaFilialEmissora != null && (configuracaoEmbarcador.CalcularFreteFilialEmissoraPorTabelaDeFrete || (carga.TipoOperacao?.CalculaFretePorTabelaFreteFilialEmissora ?? false)));
                decimal valorFrete = freteCalculadoPorFilialEmissora ? carga.ValorFreteAPagarFilialEmissora : carga.ValorFreteAPagar;

                return new JsonpResult(new
                {
                    carga.Codigo,
                    carga.CodigoCargaEmbarcador,
                    Filial = carga.Filial?.Descricao,
                    ModeloVeicularCarga = carga.ModeloVeicularCarga?.Descricao,
                    Motoristas = carga.NomeMotoristas,
                    Operador = carga.Operador?.Descricao,
                    Peso = carga.DadosSumarizados?.PesoTotal.ToString("n2") ?? "",
                    Placas = carga.PlacasVeiculos,
                    Rota = carga.Rota?.Descricao,
                    Situacao = carga.SituacaoCarga.ObterDescricao(),
                    carga.SituacaoLiberacaoEscrituracaoPagamentoCarga,
                    TipoCarga = carga.TipoDeCarga?.Descricao,
                    TipoOperacao = carga.TipoOperacao?.Descricao,
                    Transportador = carga.Empresa?.Descricao,
                    ValorFrete = valorFrete.ToString("n2")
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

        private void LiberarEscrituracaoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao repositorioDocumentoEscrituracao = new Repositorio.Embarcador.Escrituracao.DocumentoEscrituracao(unitOfWork);

            if (carga.CargaAgrupada)
            {
                if (carga.CargasAgrupamento?.Count > 0)
                {
                    List<int> cargas = (from o in carga.CargasAgrupamento select o.Codigo).ToList();
                    cargas.Add(carga.Codigo);
                    repositorioDocumentoEscrituracao.LiberarEscrituracaoPorCargas(cargas);
                }
            }
            else
                repositorioDocumentoEscrituracao.LiberarEscrituracaoPorCarga(carga.Codigo);
        }

        private void LiberarPagamentoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

            repositorioDocumentoFaturamento.LiberarPagamentoPorCarga(carga.Codigo);
        }

        private Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaLiberacaoEscrituracaoPagamentoCarga ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaLiberacaoEscrituracaoPagamentoCarga()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                SituacaoLiberacaoEscrituracaoPagamentoCarga = Request.GetNullableEnumParam<SituacaoLiberacaoEscrituracaoPagamentoCarga>("SituacaoLiberacaoEscrituracaoPagamentoCarga")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Filial")
                return "Filial.Descricao";

            if (propriedadeOrdenar == "ModeloVeicular")
                return "ModeloVeicularCarga.Descricao";

            if (propriedadeOrdenar == "TipoCarga")
                return "TipoDeCarga.Descricao";

            if (propriedadeOrdenar == "Transportador")
                return "Empresa.RazaoSocial";

            return propriedadeOrdenar;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Cargas.Carga origem)
        {
            return origem.SituacaoLiberacaoEscrituracaoPagamentoCarga == SituacaoLiberacaoEscrituracaoPagamentoCarga.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaLiberacaoEscrituracaoPagamentoCarga filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga repositorioAprovacaoAlcada = new Repositorio.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga(unitOfWork);

                cargas = repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    cargas.Remove(new Dominio.Entidades.Embarcador.Cargas.Carga() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    cargas.Add(repositorioCarga.BuscarPorCodigo((int)itemSelecionado.Codigo));
            }

            return (from carga in cargas select carga.Codigo).ToList();
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
                grid.AdicionarCabecalho(descricao: "Situação da Liberação", propriedade: "SituacaoLiberacaoEscrituracaoPagamentoCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaLiberacaoEscrituracaoPagamentoCarga filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga repositorio = new Repositorio.Embarcador.Escrituracao.AlcadasLiberacaoEscrituracaoPagamentoCarga.AprovacaoAlcadaLiberacaoEscrituracaoPagamentoCarga(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                var lista = (
                    from carga in cargas
                    select new
                    {
                        carga.Codigo,
                        carga.CodigoCargaEmbarcador,
                        Filial = carga.Filial?.Descricao,
                        ModeloVeicular = carga.ModeloVeicularCarga?.Descricao,
                        Motorista = carga.NomeMotoristas,
                        NumeroCTes = carga.NumerosCTes,
                        SituacaoLiberacaoEscrituracaoPagamentoCarga = carga.SituacaoLiberacaoEscrituracaoPagamentoCarga.ObterDescricao(),
                        Situacao = carga.SituacaoCarga.ObterDescricao(),
                        TipoCarga = carga.TipoDeCarga?.Descricao,
                        Transportador = carga.Empresa?.RazaoSocial,
                        Veiculo = carga.PlacasVeiculos
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
        
        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Cargas.Carga origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.SituacaoCarga == SituacaoCarga.Cancelada || origem.SituacaoCarga == SituacaoCarga.Anulada)
                return;

            if (origem.SituacaoLiberacaoEscrituracaoPagamentoCarga != SituacaoLiberacaoEscrituracaoPagamentoCarga.AguardandoAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.Escrituracao.LiberacaoEscrituracaoPagamento servicoLiberacaoEscrituracaoPagamento = new Servicos.Embarcador.Escrituracao.LiberacaoEscrituracaoPagamento(unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                if (!servicoLiberacaoEscrituracaoPagamento.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                    return;

                origem.SituacaoLiberacaoEscrituracaoPagamentoCarga = SituacaoLiberacaoEscrituracaoPagamentoCarga.Aprovada;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Liberação de escrituração e pagamento aprovada", unitOfWork);

                LiberarEscrituracaoCarga(origem, unitOfWork);
                LiberarPagamentoCarga(origem, unitOfWork);
            }
            else
            {
                origem.SituacaoLiberacaoEscrituracaoPagamentoCarga = SituacaoLiberacaoEscrituracaoPagamentoCarga.Reprovada;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Liberação de escrituração e pagamento reprovada", unitOfWork);
            }

            repositorioCarga.Atualizar(origem);
            servicoLiberacaoEscrituracaoPagamento.NotificarSituacaoAprovacaoAoOperadorCarga(origem, TipoServicoMultisoftware);
        }

        #endregion
    }
}