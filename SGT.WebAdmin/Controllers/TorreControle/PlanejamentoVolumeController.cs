using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
namespace SGT.WebAdmin.Controllers.TorreControle
{
    [CustomAuthorize("TorreControle/PlanejamentoVolume")]

    public class PlanejamentoVolumeController : BaseController
    {
        #region Construtores

        public PlanejamentoVolumeController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisa(unitOfWork));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPlanejamento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.TorreControle.PlanejamentoVolume repositorioPlanejamentoVolume = new Repositorio.Embarcador.TorreControle.PlanejamentoVolume(unitOfWork, cancellationToken);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume planejamentoVolume = await repositorioPlanejamentoVolume.BuscarPorCodigoAsync(codigo, auditavel: true)
                    ?? throw new ControllerException("Erro ao buscar o planejamento de volume selecionado.");

                await repositorioPlanejamentoVolume.DeletarAsync(planejamentoVolume, Auditado);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(true);
            }
            catch (BaseException ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao deletar dados.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.TorreControle.PlanejamentoVolume repositorioPlanejamentoVolume = new Repositorio.Embarcador.TorreControle.PlanejamentoVolume(unitOfWork);
                Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume planejamentoVolume = new Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume();

                PreencherEntidade(planejamentoVolume, unitOfWork);
                SalvarRemetentes(planejamentoVolume, unitOfWork);
                SalvarDestinatarios(planejamentoVolume, unitOfWork);
                SalvarOrigens(planejamentoVolume, unitOfWork);
                SalvarDestinos(planejamentoVolume, unitOfWork);

                repositorioPlanejamentoVolume.Inserir(planejamentoVolume);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                unitOfWork.Start();

                Repositorio.Embarcador.TorreControle.PlanejamentoVolume repositorioPlanejamentoVolume = new Repositorio.Embarcador.TorreControle.PlanejamentoVolume(unitOfWork);
                Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume planejamentoVolume = repositorioPlanejamentoVolume.BuscarPorCodigo(codigo);

                PreencherEntidade(planejamentoVolume, unitOfWork);
                SalvarRemetentes(planejamentoVolume, unitOfWork);
                SalvarDestinatarios(planejamentoVolume, unitOfWork);
                SalvarOrigens(planejamentoVolume, unitOfWork);
                SalvarDestinos(planejamentoVolume, unitOfWork);

                repositorioPlanejamentoVolume.Atualizar(planejamentoVolume);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                bool duplicar = Request.GetBoolParam("Duplicar");

                Repositorio.Embarcador.TorreControle.PlanejamentoVolume repositorioPlanejamentoVolume = new Repositorio.Embarcador.TorreControle.PlanejamentoVolume(unitOfWork);

                Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume planejamentoVolume = repositorioPlanejamentoVolume.BuscarPorCodigo(codigo);
                List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeRemetente> remetentes = repositorioPlanejamentoVolume.BuscarRemetentesPorCodigosPlanejamento(new List<int> { codigo });
                List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeDestinatario> destinatarios = repositorioPlanejamentoVolume.BuscarDestinatariosPorCodigosPlanejamento(new List<int> { codigo });
                List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeOrigem> origens = repositorioPlanejamentoVolume.BuscarOrigensPorCodigosPlanejamento(new List<int> { codigo });
                List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeDestino> destinos = repositorioPlanejamentoVolume.BuscarDestinosPorCodigosPlanejamento(new List<int> { codigo });

                if (planejamentoVolume == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    Codigo = duplicar ? 0 : planejamentoVolume.Codigo,
                    TipoDeCarga = new { planejamentoVolume.TipoDeCarga?.Codigo, Descricao = planejamentoVolume.TipoDeCarga?.Descricao ?? string.Empty },
                    Transportador = new { planejamentoVolume.Transportador?.Codigo, Descricao = planejamentoVolume.Transportador?.Descricao ?? string.Empty },
                    ModeloVeicular = new { planejamentoVolume.ModeloVeicular?.Codigo, Descricao = planejamentoVolume.ModeloVeicular?.Descricao ?? string.Empty },
                    TipoOperacao = new { planejamentoVolume.TipoOperacao?.Codigo, Descricao = planejamentoVolume.TipoOperacao?.Descricao ?? string.Empty },
                    DataProgramacaoCargaInicial = planejamentoVolume.DataProgramacaoCargaInicial.ToString("dd/MM/yyyy") ?? string.Empty,
                    DataProgramacaoCargaFinal = planejamentoVolume.DataProgramacaoCargaFinal.ToString("dd/MM/yyyy") ?? string.Empty,
                    TotalToneladasMes = planejamentoVolume.TotalToneladasMes.ToString("n2") ?? string.Empty,
                    NumeroContrato = planejamentoVolume.NumeroContrato?.ToString() ?? string.Empty,
                    DisponibilidadePlacas = planejamentoVolume.DisponibilidadePlacas.ToString() ?? string.Empty,
                    TotalTransferenciaEntrePlantas = planejamentoVolume?.TotalTransferenciaEntrePlantas ?? 0,
                    Remetentes = (
                    from remetente in remetentes
                    select new
                    {
                        remetente.CPFCNPJRemetente,
                        remetente.NomeRemetente
                    }).ToList(),
                    Destinatarios = (
                    from destinatario in destinatarios
                    select new
                    {
                        destinatario.CPFCNPJDestinatario,
                        destinatario.NomeDestinatario
                    }).ToList(),
                    Origens = (
                    from origem in origens
                    select new
                    {
                        origem.CodigoLocalidade,
                        origem.DescricaoLocalidade
                    }).ToList(),
                    Destinos = (
                    from destino in destinos
                    select new
                    {
                        destino.CodigoLocalidade,
                        destino.DescricaoLocalidade
                    }).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPlanejamentoVolume();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Repositorio.Embarcador.TorreControle.PlanejamentoVolume repositorioPlanejamentoVolume = new Repositorio.Embarcador.TorreControle.PlanejamentoVolume(unitOfWork);


                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume> planejamentos = new List<Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume>();

                int importados = 0;

                RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request, planejamentos, ((dados) =>
                {
                    Servicos.Embarcador.TorreControle.ImportacaoPlanejamentoVolume servicoImportacaoPlanejamentoVolume = new Servicos.Embarcador.TorreControle.ImportacaoPlanejamentoVolume(unitOfWork, TipoServicoMultisoftware, dados);

                    importados++;

                    return servicoImportacaoPlanejamentoVolume.ObterPlanejamentoVolumeImportar();
                }));

                if (retorno == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao Importar o Arquivo.");

                foreach (Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume planejamento in planejamentos)
                    repositorioPlanejamentoVolume.Inserir(planejamento);


                unitOfWork.CommitChanges();

                retorno.Importados = importados;

                return new JsonpResult(retorno);
            }
            catch (ImportacaoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao Importar o Arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = ObterGridPesquisa(unitOfWork);

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        #endregion

        #region Métodos Privados
        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };

            grid.AdicionarCabecalho("Codigo", "Codigo", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo de carga", "TipoDeCarga", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo de operação", "TipoOperacao", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Total de toneladas por mês", "TotalTonMes", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Mês/Ano inicial", "DataProgramacaoCargaInicial", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Remetentes", "Remetentes", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destinatários", "Destinatarios", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 7, Models.Grid.Align.left, true);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaPlanejamentoVolume filtroPesquisa = ObterFiltrosPesquisa();

            Repositorio.Embarcador.TorreControle.PlanejamentoVolume repositorioPlanejamentoVolume = new Repositorio.Embarcador.TorreControle.PlanejamentoVolume(unitOfWork);
            int totalRegistros = repositorioPlanejamentoVolume.ContarConsulta(filtroPesquisa);
            List<Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume> lista = totalRegistros > 0 ? repositorioPlanejamentoVolume.Consultar(filtroPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume>();
            List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeOrigem> origens = new List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeOrigem>();
            List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeDestino> destinos = new List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeDestino>();
            List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeRemetente> remetentes = new List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeRemetente>();
            List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeDestinatario> destinatarios = new List<Dominio.ObjetosDeValor.Embarcador.TorreControle.PlanejamentoVolumeDestinatario>();

            if (totalRegistros > 0)
            {
                List<int> codigosPlanejamento = (from item in lista select item.Codigo).ToList();
                origens = repositorioPlanejamentoVolume.BuscarOrigensPorCodigosPlanejamento(codigosPlanejamento);
                destinos = repositorioPlanejamentoVolume.BuscarDestinosPorCodigosPlanejamento(codigosPlanejamento);
                remetentes = repositorioPlanejamentoVolume.BuscarRemetentesPorCodigosPlanejamento(codigosPlanejamento);
                destinatarios = repositorioPlanejamentoVolume.BuscarDestinatariosPorCodigosPlanejamento(codigosPlanejamento);
            }

            var listaRetornar = (
                from obj in lista
                select new
                {
                    obj.Codigo,
                    TipoDeCarga = obj.TipoDeCarga?.Descricao ?? string.Empty,
                    Transportador = obj.Transportador?.Descricao ?? string.Empty,
                    TipoOperacao = obj.TipoOperacao?.Descricao ?? string.Empty,
                    TotalTonMes = obj.TotalToneladasMes.ToString("n2") ?? string.Empty,
                    DataProgramacaoCargaInicial = obj.DataProgramacaoCargaInicial.ToString("MM/yyyy") ?? string.Empty,
                    Remetentes = string.Join(", ", remetentes.Where(o => o.CodigoPlanejamento == obj.Codigo).Select(remetente => $"{remetente?.NomeRemetente ?? string.Empty} ({remetente?.CPFCNPJFormatado ?? string.Empty})" ?? string.Empty)),
                    Destinatarios = string.Join(", ", destinatarios.Where(o => o.CodigoPlanejamento == obj.Codigo).Select(destinatario => $"{destinatario?.NomeDestinatario ?? string.Empty} ({destinatario?.CPFCNPJFormatado ?? string.Empty})" ?? string.Empty)),
                }
            ).ToList();

            grid.AdicionaRows(listaRetornar);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaPlanejamentoVolume ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaPlanejamentoVolume()
            {
                CodigoTipoCarga = Request.GetIntParam("TipoDeCarga"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                DataProgramacaoCargaInicial = Request.GetDateTimeParam("DataProgramacaoCargaInicial"),
                DataProgramacaoCargaFinal = Request.GetDateTimeParam("DataProgramacaoCargaFinal"),
                Remetentes = Request.GetNullableListParam<double>("Remetentes"),
                Destinatarios = Request.GetNullableListParam<double>("Destinatarios"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
            };
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume planejamentoVolume, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            int codigoTipoCarga = Request.GetIntParam("TipoDeCarga");
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
            int codigoModeloVeicular = Request.GetIntParam("ModeloVeicular");
            int codigoTransportador = Request.GetIntParam("Transportador");

            planejamentoVolume.TipoDeCarga = codigoTipoCarga > 0 ? repositorioTipoDeCarga.BuscarPorCodigo(codigoTipoCarga) : null;
            planejamentoVolume.DataProgramacaoCargaInicial = Request.GetDateTimeParam("DataProgramacaoCargaInicial");
            planejamentoVolume.DataProgramacaoCargaFinal = Request.GetDateTimeParam("DataProgramacaoCargaFinal");
            planejamentoVolume.TotalToneladasMes = Request.GetDecimalParam("TotalToneladasMes");
            planejamentoVolume.TotalTransferenciaEntrePlantas = Request.GetIntParam("TotalTransferenciaEntrePlantas");
            planejamentoVolume.DisponibilidadePlacas = Request.GetIntParam("DisponibilidadePlacas");
            planejamentoVolume.NumeroContrato = Request.GetStringParam("NumeroContrato");
            planejamentoVolume.TipoOperacao = codigoTipoOperacao > 0 ? repositorioTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;
            planejamentoVolume.ModeloVeicular = codigoModeloVeicular > 0 ? repModeloVeicular.BuscarPorCodigo(codigoModeloVeicular) : null;
            planejamentoVolume.Transportador = codigoTransportador > 0 ? repEmpresa.BuscarPorCodigo(codigoTransportador) : null;
        }

        private void SalvarRemetentes(Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume planejamentoVolume, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            dynamic remetentes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Remetentes"));

            if (planejamentoVolume.Remetentes == null)
                planejamentoVolume.Remetentes = new List<Dominio.Entidades.Cliente>();
            else
            {
                List<double> codigosRemetentes = new List<double>();

                foreach (dynamic remetente in remetentes)
                    codigosRemetentes.Add((double)remetente.Codigo);

                List<Dominio.Entidades.Cliente> remetentesDeletar = planejamentoVolume.Remetentes.Where(o => !codigosRemetentes.Contains(o.CPF_CNPJ)).ToList();

                foreach (Dominio.Entidades.Cliente remetenteDeletar in remetentesDeletar)
                    planejamentoVolume.Remetentes.Remove(remetenteDeletar);
            }

            foreach (var remetente in remetentes)
            {
                double codigo = remetente.Codigo ?? 0;

                Dominio.Entidades.Cliente remetenteAdicionar = repositorioCliente.BuscarPorCPFCNPJ(codigo);

                if (!planejamentoVolume.Remetentes.Any(o => o.CPF_CNPJ == (double)remetente.Codigo))
                    planejamentoVolume.Remetentes.Add(remetenteAdicionar);
            }
        }

        private void SalvarDestinatarios(Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume planejamentoVolume, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            dynamic destinatarios = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Destinatarios"));

            if (planejamentoVolume.Destinatarios == null)
                planejamentoVolume.Destinatarios = new List<Dominio.Entidades.Cliente>();
            else
            {
                List<double> codigosDestinatarios = new List<double>();

                foreach (dynamic destinatario in destinatarios)
                    codigosDestinatarios.Add((double)destinatario.Codigo);

                List<Dominio.Entidades.Cliente> destinatariosDeletar = planejamentoVolume.Destinatarios.Where(o => !codigosDestinatarios.Contains(o.CPF_CNPJ)).ToList();

                foreach (Dominio.Entidades.Cliente destinatarioDeletar in destinatariosDeletar)
                    planejamentoVolume.Destinatarios.Remove(destinatarioDeletar);
            }

            foreach (var destinatario in destinatarios)
            {
                double codigo = destinatario.Codigo ?? 0;

                Dominio.Entidades.Cliente destinatarioAdicionar = repositorioCliente.BuscarPorCPFCNPJ(codigo);

                if (!planejamentoVolume.Destinatarios.Any(o => o.CPF_CNPJ == (double)destinatario.Codigo))
                    planejamentoVolume.Destinatarios.Add(destinatarioAdicionar);
            }
        }

        private void SalvarOrigens(Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume planejamentoVolume, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);

            dynamic origens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Origens"));

            if (planejamentoVolume.Origens == null)
                planejamentoVolume.Origens = new List<Dominio.Entidades.Localidade>();
            else
            {
                List<int> codigosOrigens = new List<int>();

                foreach (dynamic origem in origens)
                    codigosOrigens.Add((int)origem.Codigo);

                List<Dominio.Entidades.Localidade> origensDeletar = planejamentoVolume.Origens.Where(o => !codigosOrigens.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Localidade origemDeletar in origensDeletar)
                    planejamentoVolume.Origens.Remove(origemDeletar);
            }

            foreach (var origem in origens)
            {
                int codigo = origem.Codigo ?? 0;

                Dominio.Entidades.Localidade origemAdicionar = repositorioLocalidade.BuscarPorCodigo(codigo);

                if (!planejamentoVolume.Origens.Any(o => o.Codigo == (int)origem.Codigo))
                    planejamentoVolume.Origens.Add(origemAdicionar);
            }
        }

        private void SalvarDestinos(Dominio.Entidades.Embarcador.TorreControle.PlanejamentoVolume planejamentoVolume, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(unitOfWork);

            dynamic destinos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Destinos"));

            if (planejamentoVolume.Destinos == null)
                planejamentoVolume.Destinos = new List<Dominio.Entidades.Localidade>();
            else
            {
                List<int> codigosDestinos = new List<int>();

                foreach (dynamic destino in destinos)
                    codigosDestinos.Add((int)destino.Codigo);

                List<Dominio.Entidades.Localidade> destinosDeletar = planejamentoVolume.Destinos.Where(o => !codigosDestinos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Localidade destinoDeletar in destinosDeletar)
                    planejamentoVolume.Destinos.Remove(destinoDeletar);
            }

            foreach (var destino in destinos)
            {
                int codigo = destino.Codigo ?? 0;

                Dominio.Entidades.Localidade destinoAdicionar = repositorioLocalidade.BuscarPorCodigo(codigo);

                if (!planejamentoVolume.Destinos.Any(o => o.Codigo == (int)destino.Codigo))
                    planejamentoVolume.Destinos.Add(destinoAdicionar);
            }
        }

        private List<ConfiguracaoImportacao> ConfiguracaoImportacaoPlanejamentoVolume()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            var configuracoes = new List<ConfiguracaoImportacao>();
            int tamanho = 150;

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 1, Descricao = "Cidade de Destino", Propriedade = "NomeCidadeDestino", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 2, Descricao = "UF Cidade de Destino", Propriedade = "UFCidadeDestino", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 3, Descricao = "Cidade de Origem", Propriedade = "NomeCidadeOrigem", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 4, Descricao = "UF Cidade de Origem", Propriedade = "UFCidadeOrigem", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 5, Descricao = "Total de toneladas (mês)", Propriedade = "TotalToneladasMes", Tamanho = tamanho, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = "Data Programação Carga Inicial", Propriedade = "DataProgramacaoCargaInicial", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 7, Descricao = "Data Programação Carga Final", Propriedade = "DataProgramacaoCargaFinal", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 8, Descricao = "Remetente (CPF/CNPJ)", Propriedade = "Remetente", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 9, Descricao = "Destinatário (CPF/CNPJ)", Propriedade = "Destinatario", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 10, Descricao = "Total de transferência entre plantas", Propriedade = "TotalTransferenciaEntrePlantas", Tamanho = tamanho });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 12, Descricao = "Tipo de Carga (código de integração)", Propriedade = "TipoDeCarga", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 13, Descricao = "Tipo de Operação (código de integração)", Propriedade = "TipoOperacao", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 14, Descricao = "Codigo", Propriedade = "Codigo", Tamanho = tamanho, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 15, Descricao = "Modelo Veicular", Propriedade = "ModeloVeicular", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 16, Descricao = "Disponibilidade de Placas", Propriedade = "DisponibilidadePlacas", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 17, Descricao = "Número do Contrato", Propriedade = "NumeroContrato", Tamanho = tamanho, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 18, Descricao = "Transportador", Propriedade = "Transportador", Tamanho = tamanho, CampoInformacao = true });

            return configuracoes;
        }
        #endregion
    }
}
