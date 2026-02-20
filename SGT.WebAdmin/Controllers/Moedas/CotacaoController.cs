using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Moedas
{
    [CustomAuthorize("Moedas/Cotacao")]
    public class CotacaoController : BaseController
    {
        #region Construtores

        public CotacaoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("CotacaoAtiva")) ? (SituacaoAtivoPesquisa)int.Parse(Request.Params("CotacaoAtiva")) : SituacaoAtivoPesquisa.Ativo;
                MoedaCotacaoBancoCentral moedaBancoCentral = (MoedaCotacaoBancoCentral)int.Parse(Request.Params("MoedaCotacaoBancoCentral"));
                double cliente = double.Parse(Request.Params("Cliente"));
                int codigoGrupo = Request.GetIntParam("GrupoPessoa");
                DateTime dataVigencia = Request.GetDateTimeParam("DataVigencia");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Moeda", "DescricaoMoedaBancoCentral", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "ValorMoeda", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Data da Cotação", "DataCotacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Vigência Inicial", "DataVigenciaInicial", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Vigência Final", "DataVigenciaFinal", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Cliente", "Cliente", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Grupo de Pessoa", "GrupoPessoas", 15, Models.Grid.Align.left, true);
                if (ativo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdena == "Cliente")
                    propOrdena += ".Nome";
                else if (propOrdena == "GrupoPessoas")
                    propOrdena += ".Descricao";

                Repositorio.Embarcador.Moedas.Cotacao repositorioCotacao = new Repositorio.Embarcador.Moedas.Cotacao(unitOfWork, cancellationToken);

                List<Dominio.Entidades.Embarcador.Moedas.Cotacao> listaCotacao = await repositorioCotacao.ConsultarAsync(codigoGrupo, cliente, ativo, moedaBancoCentral, dataVigencia, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(await repositorioCotacao.ContarConsultaAsync(codigoGrupo, cliente, ativo, moedaBancoCentral, dataVigencia));

                var lista = (from p in listaCotacao
                             select new
                             {
                                 p.Codigo,
                                 DescricaoMoedaBancoCentral = p.MoedaCotacaoBancoCentral.ObterDescricao(),
                                 DataCotacao = p.DataCotacao.ToString("dd/MM/yyyy HH:mm"),
                                 ValorMoeda = p.CotacaoAutomaticaViaWS ? "Cotação do Dia" : p.ValorMoeda.ToString("n10"),
                                 Cliente = p.Cliente?.Nome ?? string.Empty,
                                 GrupoPessoas = p.GrupoPessoas?.Descricao ?? string.Empty,
                                 DescricaoAtivo = p.CotacaoAtiva ? "Cotação Ativa" : "Inativa",
                                 DataVigenciaInicial = p.DataVigenciaInicial.HasValue ? p.DataVigenciaInicial.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                 DataVigenciaFinal = p.DataVigenciaFinal.HasValue ? p.DataVigenciaFinal.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                 DT_RowColor = p.CotacaoAtiva ? "#dff0d8" : ""
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
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("CotacaoAtiva")) ? (SituacaoAtivoPesquisa)int.Parse(Request.Params("CotacaoAtiva")) : SituacaoAtivoPesquisa.Ativo;
                MoedaCotacaoBancoCentral moedaBancoCentral = (MoedaCotacaoBancoCentral)int.Parse(Request.Params("MoedaCotacaoBancoCentral"));
                double cliente = double.Parse(Request.Params("Cliente"));
                int codigoGrupo = Request.GetIntParam("GrupoPessoa");
                DateTime dataVigencia = Request.GetDateTimeParam("DataVigencia");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Moeda", "DescricaoMoedaBancoCentral", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "ValorMoeda", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Data da Cotação", "DataCotacao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Vigência Inicial", "DataVigenciaInicial", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Vigência Final", "DataVigenciaFinal", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Cliente", "Cliente", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Grupo de Pessoa", "GrupoPessoas", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdena == "Cliente")
                    propOrdena += ".Nome";
                else if (propOrdena == "GrupoPessoas")
                    propOrdena += ".Descricao";

                Repositorio.Embarcador.Moedas.Cotacao repositorioCotacao = new Repositorio.Embarcador.Moedas.Cotacao(unitOfWork, cancellationToken);
                List<Dominio.Entidades.Embarcador.Moedas.Cotacao> listaCotacao = await repositorioCotacao.ConsultarAsync(codigoGrupo, cliente, ativo, moedaBancoCentral, dataVigencia, propOrdena, grid.dirOrdena, grid.inicio, 0);

                var lista = (from p in listaCotacao
                             select new
                             {
                                 p.Codigo,
                                 DescricaoMoedaBancoCentral = p.MoedaCotacaoBancoCentral.ObterDescricao(),
                                 DataCotacao = p.DataCotacao.ToString("dd/MM/yyyy HH:mm"),
                                 ValorMoeda = p.CotacaoAutomaticaViaWS ? "Cotação do Dia" : p.ValorMoeda.ToString("n10"),
                                 Cliente = p.Cliente?.Nome ?? string.Empty,
                                 GrupoPessoas = p.GrupoPessoas?.Descricao ?? string.Empty,
                                 DescricaoAtivo = p.CotacaoAtiva ? "Cotação Ativa" : "Inativa",
                                 DataVigenciaInicial = p.DataVigenciaInicial.HasValue ? p.DataVigenciaInicial.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                 DataVigenciaFinal = p.DataVigenciaFinal.HasValue ? p.DataVigenciaFinal.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                 DT_RowColor = p.CotacaoAtiva ? "#dff0d8" : ""
                             }).ToList();

                grid.AdicionaRows(lista);

                byte[] bArquivo = grid.GerarExcel();

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Adicionar(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                Repositorio.Embarcador.Moedas.Cotacao repositorioCotacao = new Repositorio.Embarcador.Moedas.Cotacao(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Moedas.Cotacao cotacao = new Dominio.Entidades.Embarcador.Moedas.Cotacao();

                await PreencherCotacao(cotacao, unitOfWork, cancellationToken);

                await repositorioCotacao.InserirAsync(cotacao, Auditado);
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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Moedas.Cotacao repositorioCotacao = new Repositorio.Embarcador.Moedas.Cotacao(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Moedas.Cotacao cotacao = await repositorioCotacao.BuscarPorCodigoAsync(codigo, true);

                await PreencherCotacao(cotacao, unitOfWork, cancellationToken);

                await repositorioCotacao.AtualizarAsync(cotacao, Auditado);
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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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
                Repositorio.Embarcador.Moedas.Cotacao repCotacao = new Repositorio.Embarcador.Moedas.Cotacao(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Moedas.Cotacao cotacao = await repCotacao.BuscarPorCodigoAsync(codigo);

                var dynCotacao = new
                {
                    cotacao.Codigo,
                    cotacao.CotacaoAutomaticaViaWS,
                    cotacao.UtilizarCotacaoRetroativa,
                    Cliente = new { Codigo = cotacao.Cliente != null ? cotacao.Cliente.CPF_CNPJ : 0, Descricao = cotacao.Cliente != null ? cotacao.Cliente.Nome : "" },
                    GrupoPessoa = new { Codigo = cotacao.GrupoPessoas != null ? cotacao.GrupoPessoas.Codigo : 0, Descricao = cotacao.GrupoPessoas != null ? cotacao.GrupoPessoas.Descricao : "" },
                    cotacao.CotacaoAtiva,
                    DataCotacao = cotacao.DataCotacao.ToString("dd/MM/yyyy HH:mm"),
                    cotacao.MoedaCotacaoBancoCentral,
                    cotacao.TipoPessoa,
                    ValorMoeda = cotacao.ValorMoeda.ToString("n10"),
                    Usuario = cotacao.Usuario.Nome,
                    DataVigenciaInicial = cotacao.DataVigenciaInicial.HasValue ? cotacao.DataVigenciaInicial.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    DataVigenciaFinal = cotacao.DataVigenciaFinal.HasValue ? cotacao.DataVigenciaFinal.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty
                };

                return new JsonpResult(dynCotacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("codigo");

                Repositorio.Embarcador.Moedas.Cotacao repCotacao = new Repositorio.Embarcador.Moedas.Cotacao(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Moedas.Cotacao cotacao = await repCotacao.BuscarPorCodigoAsync(codigo);

                await repCotacao.DeletarAsync(cotacao, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarCotacaoMoedaBancoCentral()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                MoedaCotacaoBancoCentral moeda = Request.GetEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                if (moeda != MoedaCotacaoBancoCentral.DolarCompra && moeda != MoedaCotacaoBancoCentral.DolarVenda)
                    return new JsonpResult(false, "Moeda selecionada não disponível para consulta pelo Banco Central.");

                Servicos.Embarcador.Moedas.Cotacao serCotacao = new Servicos.Embarcador.Moedas.Cotacao(unitOfWork);
                decimal valorBC = serCotacao.BuscarCotacaoDiaWSBancoCentral(moeda, unitOfWork);

                if (valorBC > 0)
                    return new JsonpResult(valorBC);
                else
                    return new JsonpResult(false, "Não foi possível buscar a cotação da moeda pelo WS do banco central.");
            }
            catch (Exception ex)
            {

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConverterMoedaEstrangeira(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Moedas.Cotacao repositorioCotacao = new Repositorio.Embarcador.Moedas.Cotacao(unitOfWork, cancellationToken);

                MoedaCotacaoBancoCentral? moeda = Request.GetNullableEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
                DateTime? dataBaseCRT = Request.GetNullableDateTimeParam("DataBaseCRT");

                if (moeda.HasValue && dataBaseCRT.HasValue && dataBaseCRT.Value > DateTime.MinValue && moeda != MoedaCotacaoBancoCentral.Real)
                {
                    Dominio.Entidades.Embarcador.Moedas.Cotacao cotacao = await repositorioCotacao.BuscarCotacaoAsync(moeda.Value, dataBaseCRT.Value);
                    if (cotacao != null)
                        return new JsonpResult(cotacao.ValorMoeda);
                    else
                        return new JsonpResult(0m);
                }
                return new JsonpResult(0m);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConverterMoedaEstrangeiraPedido(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Moedas.Cotacao repositorioCotacao = new Repositorio.Embarcador.Moedas.Cotacao(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork, cancellationToken);

                int codigoCargaPedido = Request.GetIntParam("CargaPedido");
                int codigoCarga = Request.GetIntParam("Carga");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? moeda = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("Moeda");

                DateTime? dataBaseCRT = null;

                if (codigoCargaPedido > 0)
                    dataBaseCRT = await repositorioCargaPedido.BuscarDataBaseCRTPedidoAsync(codigoCargaPedido);
                else if (codigoCarga > 0)
                    dataBaseCRT = await repositorioCargaPedido.BuscarDataBaseCRTCargaAsync(codigoCarga);

                if (!moeda.HasValue || !dataBaseCRT.HasValue)
                    return new JsonpResult(0.ToString("n10"));

                Dominio.Entidades.Embarcador.Moedas.Cotacao cotacao = await repositorioCotacao.BuscarCotacaoAsync(moeda.Value, dataBaseCRT.Value);

                if (cotacao == null)
                    return new JsonpResult(0.ToString("n10"));

                return new JsonpResult(cotacao.ValorMoeda.ToString("n10"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao converter os valores.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConverterMoedaEstrangeiraPedidoPorData(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Moedas.Cotacao repositorioCotacao = new Repositorio.Embarcador.Moedas.Cotacao(unitOfWork, cancellationToken);

                DateTime data = Request.GetDateTimeParam("DataEmissao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? moeda = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral>("Moeda");

                DateTime? dataBaseCRT = data;

                if (!moeda.HasValue || !dataBaseCRT.HasValue)
                    return new JsonpResult(0.ToString("n10"));

                Dominio.Entidades.Embarcador.Moedas.Cotacao cotacao = await repositorioCotacao.BuscarCotacaoAsync(moeda.Value, dataBaseCRT.Value);

                if (cotacao == null)
                    return new JsonpResult(0.ToString("n10"));

                return new JsonpResult(cotacao.ValorMoeda.ToString("n10"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, true, "Ocorreu uma falha ao converter os valores.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task PreencherCotacao(Dominio.Entidades.Embarcador.Moedas.Cotacao cotacao, Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Moedas.Cotacao repositorioCotacao = new Repositorio.Embarcador.Moedas.Cotacao(unitOfWork, cancellationToken);

            double cliente = Request.GetDoubleParam("Cliente");
            cotacao.Cliente = cliente > 0 ? new Dominio.Entidades.Cliente() { CPF_CNPJ = cliente } : null;
            int grupoPessoa = Request.GetIntParam("GrupoPessoa");
            cotacao.GrupoPessoas = grupoPessoa > 0 ? new Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas() { Codigo = grupoPessoa } : null;

            cotacao.CotacaoAtiva = Request.GetBoolParam("CotacaoAtiva");
            cotacao.CotacaoAutomaticaViaWS = Request.GetBoolParam("CotacaoAutomaticaViaWS");
            cotacao.UtilizarCotacaoRetroativa = Request.GetBoolParam("UtilizarCotacaoRetroativa");
            cotacao.DataCotacao = DateTime.Now;
            cotacao.MoedaCotacaoBancoCentral = Request.GetEnumParam<MoedaCotacaoBancoCentral>("MoedaCotacaoBancoCentral");
            cotacao.TipoPessoa = Request.GetEnumParam<TipoPessoa>("TipoPessoa");
            cotacao.Usuario = this.Usuario;
            cotacao.ValorMoeda = Request.GetDecimalParam("ValorMoeda");
            cotacao.DataVigenciaInicial = Request.GetNullableDateTimeParam("DataVigenciaInicial");
            cotacao.DataVigenciaFinal = Request.GetNullableDateTimeParam("DataVigenciaFinal");

            if (cotacao.DataVigenciaInicial.HasValue && cotacao.DataVigenciaFinal.HasValue && await repositorioCotacao.PossuiCotacaoMesmoPeriodoAsync(cotacao.Codigo, cliente, grupoPessoa, cotacao.DataVigenciaInicial, cotacao.DataVigenciaFinal, cotacao.MoedaCotacaoBancoCentral))
                throw new ControllerException("Já existe uma outra cotação ativa no período!");
        }

        #endregion
    }
}
