using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Fretes/AutorizacaoRecusaCheckin")]
    public class AutorizacaoRecusaCheckinController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin,
        Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.RegraAutorizacaoRecusaCheckin,
        Dominio.Entidades.Embarcador.Cargas.CargaCTe
    >
    {
		#region Construtores

		public AutorizacaoRecusaCheckinController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais

		public async Task<IActionResult> ReprocessarRecusaCheckin()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repositorioCargaCTe.BuscarPorCodigo(codigo);

                if (cargaCTe == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (cargaCTe.SituacaoCheckin != SituacaoCheckin.SemRegraAprovacao)
                    return new JsonpResult(new { RegraReprocessada = true });

                unitOfWork.Start();

                new Servicos.Embarcador.Frete.RecusaCheckinAprovacao(unitOfWork, Auditado).CriarAprovacao(cargaCTe, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { RegraReprocessada = cargaCTe.SituacaoCheckin != SituacaoCheckin.SemRegraAprovacao });
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
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar a recusa de checkin.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarMultiplasRecusasCheckin()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                List<int> codigos = ObterCodigosOrigensSelecionadas(unitOfWork);
                Repositorio.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin repositorioAprovacao = new Repositorio.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin(unitOfWork);
                Servicos.Embarcador.Frete.RecusaCheckinAprovacao servicoAprovacao = new Servicos.Embarcador.Frete.RecusaCheckinAprovacao(unitOfWork, Auditado);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTe = repositorioAprovacao.BuscarSemRegraAprovacaoPorCodigos(codigos);
                int totalRegrasReprocessadas = 0;

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in listaCargaCTe)
                {
                    servicoAprovacao.CriarAprovacao(cargaCTe, TipoServicoMultisoftware);

                    if (cargaCTe.SituacaoCheckin != SituacaoCheckin.SemRegraAprovacao)
                        totalRegrasReprocessadas++;
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
                return new JsonpResult(false, "Ocorreu uma falha ao reprocessar as recusas de checkin.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Globais Sobrescritos

        public override IActionResult BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repositorioCargaCTe.BuscarPorCodigo(codigo);

                if (cargaCTe == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    cargaCTe.Codigo,
                    Situacao = cargaCTe.SituacaoCheckin,
                    NumeroCTe = cargaCTe.CTe.Numero,
                    Valor = cargaCTe.CTe.ValorTotalMercadoria.ToString("n2"),
                    Peso = cargaCTe.CTe.Peso.ToString("n2")
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

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRecusaCheckinAprovacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRecusaCheckinAprovacao()
            {
                CodigoUsuario = Request.GetIntParam("Usuario"),
                NumeroCTe = Request.GetIntParam("NumeroCTe"),
                SituacaoCheckin = Request.GetNullableEnumParam<SituacaoCheckin>("Situacao"),
                CodigoCarga = Request.GetStringParam("Carga"),
                DataCriacaoCarga = Request.GetNullableDateTimeParam("DataCriacaoCarga"),
                Filial = Request.GetIntParam("Filial"),
                SerieCte = Request.GetIntParam("Serie"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                Transportador = Request.GetIntParam("Transportador")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Numero")
                return "CTe.Numero";

            return propriedadeOrdenar;
        }

        #endregion

        #region Métodos Protegidos SobrescritosS

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe)
        {
            return cargaCTe.SituacaoCheckin == SituacaoCheckin.AguardandoAprovacao;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCte;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRecusaCheckinAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin repositorioAprovacaoAlcada = new Repositorio.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin(unitOfWork);

                listaCargaCte = repositorioAprovacaoAlcada.Consultar(filtrosPesquisa, new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta() { PropriedadeOrdenar = "Codigo" });

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    listaCargaCte.Remove(new Dominio.Entidades.Embarcador.Cargas.CargaCTe() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                listaCargaCte = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

                foreach (var itemSelecionado in listaItensSelecionados)
                    listaCargaCte.Add(repositorioCargaCTe.BuscarPorCodigo((int)itemSelecionado.Codigo));
            }

            return (from cargaCTe in listaCargaCte select cargaCTe.Codigo).ToList();
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

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número do CT-e", "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Série do CTe", "SerieCte", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nº Carga", "NumeroCarga", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Inicial", "DataInicialCarga", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "Valor", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Peso", "Peso", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação da Aprovação", "SituacaoCheckin", 15, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRecusaCheckinAprovacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin repositorio = new Repositorio.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> listaCargaCTes = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

                var listaCargaCTesRetornar = (
                    from cargaCTe in listaCargaCTes
                    select new
                    {
                        cargaCTe.Codigo,
                        Numero = cargaCTe.CTe.Numero,
                        SerieCte = cargaCTe.CTe?.Serie?.Numero ?? 0,
                        NumeroCarga = cargaCTe.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                        DataInicialCarga = cargaCTe.Carga.DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm:ss"),
                        Filial = cargaCTe.Carga?.Filial?.Descricao ?? string.Empty,
                        Transportador = cargaCTe?.Carga?.Empresa?.Descricao ?? string.Empty,
                        TipoOperacao = cargaCTe?.Carga?.TipoOperacao?.Descricao ?? string.Empty,
                        Valor = cargaCTe.CTe.ValorAReceber.ToString("n2"),
                        Peso = cargaCTe.CTe.Peso.ToString("n2"),
                        SituacaoCheckin = cargaCTe.SituacaoCheckin.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaCargaCTesRetornar);
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

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe, Repositorio.UnitOfWork unitOfWork)
        {
            if (cargaCTe.SituacaoCheckin != SituacaoCheckin.AguardandoAprovacao)
                return;

            SituacaoRegrasAutorizacao situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(cargaCTe.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                if (new Servicos.Embarcador.Frete.RecusaCheckinAprovacao(unitOfWork).LiberarProximaPrioridadeAprovacao(cargaCTe, TipoServicoMultisoftware))
                {
                    cargaCTe.SituacaoCheckin = SituacaoCheckin.RecusaAprovada;
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe, "Recusa do checkin aprovada", unitOfWork);
                    new Servicos.Embarcador.Frete.RecusaCheckin(unitOfWork).RecusarCheckin(cargaCTe);
                    new Servicos.Embarcador.Carga.Carga(unitOfWork).CriarRegistroIntegracaoCargaFrete(cargaCTe.Carga, unitOfWork, TipoIntegracao.RejeicaoCte, Servicos.Embarcador.Pedido.Stage.BuscarStagePorCargaCte(cargaCTe.Codigo, unitOfWork), new List<string>());
                }

                new Servicos.Embarcador.Carga.Carga(unitOfWork).CriarRegistroIntegracaoConsolidado(cargaCTe.Carga, unitOfWork, Auditado, TipoServicoMultisoftware, WebServiceConsultaCTe);
                new Servicos.Embarcador.Carga.Carga(unitOfWork).AvancarEtapaSubCargasConsolidado(cargaCTe.Carga, cargaCTe, unitOfWork, Auditado, TipoServicoMultisoftware, WebServiceConsultaCTe);
            }
            else
            {
                cargaCTe.SituacaoCheckin = SituacaoCheckin.RecusaReprovada;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTe, "Recusa do checkin reprovada", unitOfWork);
            }

            repositorioCargaCTe.Atualizar(cargaCTe);
        }

        #endregion
    }
}
