using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "CTe/AutorizacaoIntegracaoCTe")]
    public class AutorizacaoIntegracaoCTeController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe,
        Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.RegraAutorizacaoIntegracaoCTe,
        Dominio.Entidades.Embarcador.Cargas.Carga
    >
    {
		#region Construtores

		public AutorizacaoIntegracaoCTeController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Públicos

		public async Task<IActionResult> BuscarDadosAprovacao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe repositorioAprovacao = new Repositorio.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe(unitOfWork);
                List<Dominio.Entidades.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe> listaAprovacoes = repositorioAprovacao.ConsultarAutorizacoes(codigoCarga, null);

                return new JsonpResult(new
                {
                    Aprovacoes = (from o in listaAprovacoes
                                  select new
                                  {
                                      o.Codigo,
                                      Prioridade = o.RegraAutorizacao.PrioridadeAprovacao,
                                      Usuario = o.Usuario?.Descricao ?? "",
                                      Situacao = o.Situacao.ObterDescricao(),
                                      Regra = o.RegraAutorizacao.Descricao,
                                      Motivo = o.Motivo ?? string.Empty,
                                      DT_RowColor = o.Bloqueada ? CorGrid.Cinza : (o.Situacao == SituacaoAlcadaRegra.Aprovada ? CorGrid.Verde : o.Situacao == SituacaoAlcadaRegra.Rejeitada ? CorGrid.Vermelho : o.Situacao == SituacaoAlcadaRegra.Pendente ? CorGrid.Amarelo : ""),
                                      DT_FontColor = o.Bloqueada ? CorGrid.Branco : (o.Situacao == SituacaoAlcadaRegra.Rejeitada ? CorGrid.Branco : "")
                                  }).ToList(),
                    Detalhes = new
                    {
                        NumeroAprovadoresNecessarios = listaAprovacoes.FirstOrDefault()?.RegraAutorizacao?.NumeroAprovadores ?? 0,
                        Aprovacoes = listaAprovacoes.Where(obj => obj.Situacao == SituacaoAlcadaRegra.Aprovada).Count(),
                        Rejeicoes = listaAprovacoes.Where(obj => obj.Situacao == SituacaoAlcadaRegra.Rejeitada).Count(),
                        Pendentes = listaAprovacoes.Where(obj => obj.Situacao == SituacaoAlcadaRegra.Pendente).Count(),
                        Situacao = listaAprovacoes.FirstOrDefault()?.OrigemAprovacao?.SituacaoAutorizacaoIntegracaoCTe.ObterDescricao() ?? string.Empty
                    }
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
                    carga.SituacaoAutorizacaoIntegracaoCTe,
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

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoIntegracaoCTe ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoIntegracaoCTe()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                SituacaoAutorizacaoIntegracaoCTe = Request.GetNullableEnumParam<SituacaoAutorizacaoIntegracaoCTe>("SituacaoAutorizacaoIntegracaoCTe"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao")
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
            return origem.SituacaoAutorizacaoIntegracaoCTe == SituacaoAutorizacaoIntegracaoCTe.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoIntegracaoCTe filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe repositorioAprovacaoIntegracao = new Repositorio.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe(unitOfWork);

                cargas = repositorioAprovacaoIntegracao.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

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
                grid.AdicionarCabecalho(descricao: "Situação da Autorização", propriedade: "SituacaoAutorizacaoIntegracaoCTe", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaAutorizacaoIntegracaoCTe filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe repositorio = new Repositorio.Embarcador.CTe.AlcadasAutorizacaoIntegracaoCTe.AprovacaoAlcadaIntegracaoCTe(unitOfWork);
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
                        SituacaoAutorizacaoIntegracaoCTe = carga.SituacaoAutorizacaoIntegracaoCTe.ObterDescricao(),
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

            if (origem.SituacaoAutorizacaoIntegracaoCTe != SituacaoAutorizacaoIntegracaoCTe.AguardandoAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Servicos.Embarcador.CTe.AutorizacaoIntegracaoCTe servicoAutorizacaoIntegracaoCTe = new Servicos.Embarcador.CTe.AutorizacaoIntegracaoCTe(unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                if (!servicoAutorizacaoIntegracaoCTe.LiberarProximaPrioridadeAprovacao(origem, TipoServicoMultisoftware))
                    return;

                origem.SituacaoAutorizacaoIntegracaoCTe = SituacaoAutorizacaoIntegracaoCTe.Aprovada;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Integração de CT-e autorizada.", unitOfWork);
            }
            else
            {
                origem.SituacaoAutorizacaoIntegracaoCTe = SituacaoAutorizacaoIntegracaoCTe.Reprovada;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, origem, "Integração de CT-e reprovada.", unitOfWork);
            }

            repositorioCarga.Atualizar(origem);
            servicoAutorizacaoIntegracaoCTe.NotificarSituacaoAprovacaoAoOperadorCarga(origem, TipoServicoMultisoftware);
        }

        #endregion
    }
}