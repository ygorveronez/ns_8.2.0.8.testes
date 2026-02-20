using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize(new string[] { "RegrasAprovacao", "ConsultarAutorizacoes" }, "Veiculos/AutorizacaoCadastroVeiculo", "Veiculos/Veiculo")]
    public class AutorizacaoCadastroVeiculoController : RegraAutorizacao.AutorizacaoController<
        Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo,
        Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.RegraAutorizacaoCadastroVeiculo,
        Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo
    >
    {
		#region Construtores

		public AutorizacaoCadastroVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Métodos Globais Sobrescritos

		public override IActionResult BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo(unitOfWork);
                var cadastroVeiculo = repositorio.BuscarPorCodigo(codigo);

                if (cadastroVeiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    cadastroVeiculo.Codigo,
                    Solicitante = cadastroVeiculo.Usuario.Nome,
                    DataCadastro = cadastroVeiculo.DataCadastro.ToDateTimeString(),
                    cadastroVeiculo.Veiculo.Placa,
                    ModeloVeicular = cadastroVeiculo.Veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    Transportador = cadastroVeiculo.Veiculo.Empresa.Descricao,
                    cadastroVeiculo.Veiculo.Renavam,
                    cadastroVeiculo.Veiculo.SituacaoCadastro,
                    Tara = cadastroVeiculo.Veiculo.Tara.ToString("n0"),
                    CapacidadeKG = cadastroVeiculo.Veiculo.CapacidadeKG.ToString("n0"),
                    TipoRodado = cadastroVeiculo.Veiculo.DescricaoTipoRodado,
                    TipoCarroceria = cadastroVeiculo.Veiculo.DescricaoTipoCarroceria,
                    ModeloCarroceria = cadastroVeiculo.Veiculo.ModeloCarroceria?.Descricao ?? string.Empty,
                    Reboques = string.Join(", ", cadastroVeiculo.Veiculo.VeiculosVinculados.Select(o => o.Placa)),
                    CodigoVeiculo = cadastroVeiculo.Veiculo.Codigo
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

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo repAprovacaoAlcadaCadastroVeiculo = new Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);

                int codigoCadastroVeiculos = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("DT_RowColor", false);
                grid.AdicionarCabecalho("DT_FontColor", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 7, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo> listaAprovacoes = repAprovacaoAlcadaCadastroVeiculo.ConsultarAutorizacoes(codigoCadastroVeiculos, parametrosConsulta);
                grid.setarQuantidadeTotal(repAprovacaoAlcadaCadastroVeiculo.ContarAutorizacoes(codigoCadastroVeiculos));

                var lista = (from obj in listaAprovacoes
                             select new
                             {
                                 obj.Codigo,
                                 PrioridadeAprovacao = obj.RegraAutorizacao?.PrioridadeAprovacao ?? 0,
                                 Situacao = obj.Situacao.ObterDescricao(),
                                 Usuario = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? obj.Usuario?.ExibirUsuarioAprovacao ?? false ? obj.Usuario?.Nome ?? string.Empty : obj.RegraAutorizacao?.Descricao ?? string.Empty : obj.Usuario?.Nome ?? string.Empty,
                                 DT_RowColor = obj.Bloqueada ? CorGrid.Cinza : (obj.Situacao == SituacaoAlcadaRegra.Aprovada ? CorGrid.Verde : obj.Situacao == SituacaoAlcadaRegra.Rejeitada ? CorGrid.Vermelho : obj.Situacao == SituacaoAlcadaRegra.Pendente ? CorGrid.Amarelo : ""),
                                 DT_FontColor = obj.Bloqueada ? CorGrid.Branco : (obj.Situacao == SituacaoAlcadaRegra.Rejeitada ? CorGrid.Branco : "")
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

        #endregion

        #region Métodos Privados

        private string ObterPropriedadeOrdenar(Models.Grid.Grid grid)
        {
            if (grid.header[grid.indiceColunaOrdena].data == "Placa")
                return "Veiculo.Placa";

            if (grid.header[grid.indiceColunaOrdena].data == "Transportador")
                return "Veiculo.Empresa.RazaoSocial";

            if (grid.header[grid.indiceColunaOrdena].data == "ModeloVeicular")
                return "Veiculo.ModeloVeicularCarga.Descricao";

            if (grid.header[grid.indiceColunaOrdena].data == "Usuario")
                return "Usuario.Nome";

            return grid.header[grid.indiceColunaOrdena].data;
        }

        private List<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo> Pesquisar(ref int totalRegistros, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaCadastroVeiculoAprovacao()
            {
                Codigo = Request.GetIntParam("Codigo"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoModeloVeicular = Request.GetIntParam("ModeloVeicular"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                Situacao = Request.GetEnumParam<SituacaoCadastroVeiculo>("Situacao")
            };

            var repositorio = new Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo.AprovacaoAlcadaCadastroVeiculo(unitOfWork);
            var listaEntidadesAprovacao = repositorio.Consultar(filtrosPesquisa, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);

            totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

            return listaEntidadesAprovacao;
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override bool IsPermitirDelegar(Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo cadastroVeiculo)
        {
            return cadastroVeiculo.Veiculo.SituacaoCadastro == SituacaoCadastroVeiculo.Pendente;
        }

        protected override List<int> ObterCodigosOrigensSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo> cadastrosVeiculos;
            var selecionarTodos = Request.GetBoolParam("SelecionarTodos");

            if (selecionarTodos)
            {
                int totalRegistros = 0;
                var listaItensNaoSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensNaoSelecionados"));

                cadastrosVeiculos = Pesquisar(ref totalRegistros, propriedadeOrdenar: "Codigo", direcaoOrdenacao: "", inicioRegistros: 0, maximoRegistros: 0, unitOfWork: unitOfWork);

                foreach (var itemNaoSelecionado in listaItensNaoSelecionados)
                    cadastrosVeiculos.Remove(new Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo() { Codigo = (int)itemNaoSelecionado.Codigo });
            }
            else
            {
                var repositorioEntidade = new Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo(unitOfWork);
                var listaItensSelecionados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ItensSelecionados"));

                cadastrosVeiculos = new List<Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo>();

                foreach (var itemSelecionado in listaItensSelecionados)
                {
                    cadastrosVeiculos.Add(repositorioEntidade.BuscarPorCodigo((int)itemSelecionado.Codigo));
                }
            }

            return cadastrosVeiculos.Select(o => o.Codigo).ToList();
        }

        protected override Models.Grid.Grid ObterGridPesquisa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Placa", propriedade: "Placa", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Transportador", propriedade: "Transportador", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Modelo Veicular", propriedade: "ModeloVeicular", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Usuário", propriedade: "Usuario", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Data Cadastro", propriedade: "DataCadastro", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid);
                int totalRegistros = 0;

                var cadastrosVeiculos = Pesquisar(ref totalRegistros, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                var lista = (
                    from cadastroVeiculo in cadastrosVeiculos
                    select new
                    {
                        cadastroVeiculo.Codigo,
                        cadastroVeiculo.Veiculo.Placa,
                        Transportador = cadastroVeiculo.Veiculo.Empresa?.RazaoSocial ?? string.Empty,
                        ModeloVeicular = cadastroVeiculo.Veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty,
                        Usuario = cadastroVeiculo.Usuario.Nome,
                        DataCadastro = cadastroVeiculo.DataCadastro.ToString("dd/MM/yyyy"),
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

        protected override void VerificarSituacaoOrigem(Dominio.Entidades.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo origem, Repositorio.UnitOfWork unitOfWork)
        {
            if (origem.Veiculo.SituacaoCadastro != SituacaoCadastroVeiculo.Pendente)
                return;

            var situacaoRegrasAutorizacao = ObterSituacaoRegrasAutorizacao(origem.Codigo, unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aguardando)
                return;

            var repVeiculo = new Repositorio.Veiculo(unitOfWork);
            var repCadastroVeiculo = new Repositorio.Embarcador.Veiculos.AlcadasCadastroVeiculo.CadastroVeiculo(unitOfWork);

            if (situacaoRegrasAutorizacao == SituacaoRegrasAutorizacao.Aprovadas)
            {
                origem.Finalizado = true;
                origem.Veiculo.SituacaoCadastro = SituacaoCadastroVeiculo.Aprovado;
            }
            else
                origem.Veiculo.SituacaoCadastro = SituacaoCadastroVeiculo.Rejeitada;

            repCadastroVeiculo.Atualizar(origem);
            repVeiculo.Atualizar(origem.Veiculo);
        }

        #endregion
    }
}