using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.Transbordo
{
    [CustomAuthorize("Cargas/Transbordo")]
    public class TransbordoController : BaseController
    {
        #region Construtores

        public TransbordoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTransbordo filtroPesquisa = ObterFiltroPesquisaTransbordo();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Transbordo.NumeroTransbordo, "NumeroTransbordo", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Transbordo.DataTransbordo, "DataTransbordo", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Transbordo.LocalidadeTransbordo, "LocalidadeTransbordo", 20, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Empresa, "Empresa", 20, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Veiculo, "Veiculo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Motorista, "Motorista", 20, Models.Grid.Align.left, false);
                if (filtroPesquisa.SituacaoTransbordo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.Todas)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "Situacao", 15, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);

                List<Dominio.Entidades.Embarcador.Cargas.Transbordo> transbordos = repTransbordo.Consultar(filtroPesquisa, parametrosConsulta);
                grid.setarQuantidadeTotal(repTransbordo.ContarConsulta(filtroPesquisa));
                var lista = (from p in transbordos
                             select new
                             {
                                 p.Codigo,
                                 p.NumeroTransbordo,
                                 DataTransbordo = p.DataTransbordo.ToString("dd/MM/yyyy"),
                                 LocalidadeTransbordo = p.localidadeTransbordo.DescricaoCidadeEstado,
                                 Empresa = p.Empresa != null ? p.Empresa.RazaoSocial : "",
                                 Veiculo = BuscarPlacas(p.Veiculo, p.VeiculosVinculados.ToList()),
                                 Motorista = BuscarMotorista(p.Motoristas.ToList()),
                                 Situacao = p.SituacaoTransbordo.ObterDescricao()
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

        public async Task<IActionResult> ConsultarCTesParaTransbordo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int codigoCarga = int.Parse(Request.Params("Carga"));
                string statusCTe = Request.Params("Status");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                string propOrdenacao = ObterGridCTes(ref grid, carga);

                bool buscarPorCargaOrigem = true;
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaMontarConsultaCtes filtro = new()
                {
                    Carga = carga.Codigo,
                    NumeroDocumento = 0,
                    NumeroNF = 0,
                    StatusCTe = !string.IsNullOrWhiteSpace(statusCTe) ? new string[] { statusCTe } : null,
                    ApenasCTesNormais = true,
                    CtesSubContratacaoFilialEmissora = true,
                    CtesSemSubContratacaoFilialEmissora = false,
                    EmpresasFilialEmissora = new List<int>(),
                    ProprietarioVeiculo = string.Empty,
                    Destinatario = 0,
                    BuscarPorCargaOrigem = buscarPorCargaOrigem,
                    RetornarPreCtes = false,
                };

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = await repCargaCTe.ConsultarCTes(filtro,
                                                                                                         propOrdenacao,
                                                                                                         grid.dirOrdena,
                                                                                                         grid.inicio,
                                                                                                         grid.limite);

                int countConsultaCTes = await repCargaCTe.ContarConsultaCTes(filtro);

                grid.setarQuantidadeTotal(countConsultaCTes);
                grid.AdicionaRows(ObterListaObjetosCTe(cargaCTes));
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

        public async Task<IActionResult> ConsultarCTesDoTransbordo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Transbordo repCargaTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);


                int codigoCarga = int.Parse(Request.Params("Carga"));
                int codigoTransbordo = int.Parse(Request.Params("Codigo"));
                //string statusCTe = Request.Params("Status");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                string propOrdenacao = ObterGridCTes(ref grid, carga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repCargaTransbordo.ConsultarCTesTransbordo(codigoTransbordo, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCargaTransbordo.ContarConsultaCTesTransbordo(codigoTransbordo));
                grid.AdicionaRows(ObterListaObjetosCTe(cargaCTes));
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

        public async Task<IActionResult> ConsultarEntregasDoTransbordo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.Transbordo repositorioTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                int codigoCarga = int.Parse(Request.Params("Carga"));
                int codigoTransbordo = int.Parse(Request.Params("Codigo"));

                Models.Grid.Grid grid = ObterGridEntregas(configuracaoGeralCarga);

                Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo = repositorioTransbordo.BuscarPorCodigo(codigoTransbordo);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

                if (transbordo.Entregas.Count > 0)
                    cargasEntrega = transbordo.Entregas.ToList();
                else
                    cargasEntrega = repositorioCargaEntrega.BuscarPorCarga(codigoCarga);

                grid.setarQuantidadeTotal(cargasEntrega.Count);
                grid.AdicionaRows(ObterListaObjetosEntregas(cargasEntrega, configuracaoGeralCarga));

                return new JsonpResult(grid);
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

        public async Task<IActionResult> ConsultarEntregasParaTransbordo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                Models.Grid.Grid grid = ObterGridEntregas(configuracaoGeralCarga);

                int codigoCarga = Request.GetIntParam("Carga");
                List<int> codigosCargas = codigoCarga > 0 ? new List<int>() { codigoCarga } : Request.GetListParam<int>("Cargas");

                int totalRegistros = repositorioCargaEntrega.ContarConsultaEntregasTransbordo(codigosCargas);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> registros = totalRegistros > 0 ? repositorioCargaEntrega.ConsultarEntregasTransbordo(codigosCargas, grid.ObterParametrosConsulta()) : new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(ObterListaObjetosEntregas(registros, configuracaoGeralCarga));

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar as entregas do transbordo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarTransbordo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

                Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();

                int codigoCarga, codigoVeiculo, codigoLocalidade, codEmpresa, codigoMotorista = 0;
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Empresa"), out codEmpresa);
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("LocalidadeTransbordo"), out codigoLocalidade);


                List<int> codigosCargas = Request.GetListParam<int>("Cargas");
                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                int codigoTipoCarga = Request.GetIntParam("TipoCarga");

                Dominio.Entidades.Usuario motorista = await repUsuario.BuscarPorCodigoAsync(codigoMotorista);

                if (ConfiguracaoEmbarcador.ValidarDataLiberacaoSeguradora && !(motorista?.DataValidadeLiberacaoSeguradora.HasValue ?? false))
                    throw new ControllerException("O motorista não possui uma data de limite da seguradora configurada, por isso não é possível informar este motorista para transportar essa carga, verifique e tente novamente.");

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();

                if (codigosCargas.Count > 0)
                    cargas = await repCarga.BuscarPorCodigosAsync(codigosCargas);
                else if (codigoCarga > 0)
                    cargas.Add(await repCarga.BuscarPorCodigoAsync(codigoCarga));

                if (cargas.Count == 0)
                    throw new ControllerException("Nenhuma carga encontrada");

                Dominio.Entidades.Localidade localidadeTransbordo = await repLocalidade.BuscarPorCodigoAsync(codigoLocalidade);
                Dominio.Entidades.Empresa empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? cargas.FirstOrDefault().Empresa : await repEmpresa.BuscarPorCodigoAsync(codEmpresa);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = codigoTipoOperacao > 0 ? await repositorioTipoOperacao.BuscarPorCodigoAsync(codigoTipoOperacao) : null;
                Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga = codigoTipoCarga > 0 ? await repositorioTipoDeCarga.BuscarPorCodigoAsync(codigoTipoCarga) : null;

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas = ConverterEntregas(cargas.Select(o => o.Codigo).ToList(), unitOfWork);

                if (entregas.Count == 0)
                    throw new ControllerException("Nenhuma entrega selecionada");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configMonitoramento = await repConfiguracaoMonitoramento.BuscarConfiguracaoPadraoAsync();
                Dominio.Entidades.Veiculo veiculo = await repVeiculo.BuscarPorCodigoAsync(codigoVeiculo);

                await unitOfWork.StartAsync();

                List<(Dominio.Entidades.Embarcador.Cargas.Transbordo Transbordo, Dominio.Entidades.Embarcador.Cargas.Carga Carga)> listaTransbordo = new List<(Dominio.Entidades.Embarcador.Cargas.Transbordo, Dominio.Entidades.Embarcador.Cargas.Carga)>();
                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaAntiga in cargas)
                {
                    if (cargaAntiga.SituacaoCarga == SituacaoCarga.Cancelada || cargaAntiga.SituacaoCarga == SituacaoCarga.Anulada)
                        throw new ControllerException($"A Carga {cargaAntiga.CodigoCargaEmbarcador} possui situação cancelada na qual não é possível gerar transbordo");

                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregasCarga = entregas.Where(o => o.Carga.Codigo == cargaAntiga.Codigo).ToList();
                    if (entregasCarga.Count == 0)
                        throw new ControllerException($"Nenhuma entrega selecionada da carga {cargaAntiga.CodigoCargaEmbarcador}");

                    listaTransbordo.Add(GerarTransbordoCarga(cargaAntiga, localidadeTransbordo, empresa, veiculo, motorista, tipoOperacao, tipoCarga, entregasCarga, unitOfWork));

                    if (configMonitoramento.FinalizarMonitoramentoAoGerarTransbordoCarga)
                        Servicos.Embarcador.Monitoramento.Monitoramento.FinalizarMonitoramento(cargaAntiga, DateTime.Now, ConfiguracaoEmbarcador, Auditado, "Monitoramento finalizado ao gerar carga de transbordo", unitOfWork, MotivoFinalizacaoMonitoramento.FinalizadoAoGerarCarga);
                }

                if (listaTransbordo.Count > 1)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga cargaAgrupada = new Servicos.Embarcador.Carga.CargaAgrupada(unitOfWork).AgruparCargas(null, listaTransbordo.Where(o => o.Carga != null).Select(o => o.Carga).ToList(), TipoServicoMultisoftware, Cliente);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaAgrupada, "Criada pelo agrupamento automático do transbordo das cargas " + string.Join(", ", (from obj in cargaAgrupada.CodigosAgrupados select obj).ToList()), unitOfWork);
                }

                await unitOfWork.CommitChangesAsync();

                foreach ((Dominio.Entidades.Embarcador.Cargas.Transbordo Transbordo, Dominio.Entidades.Embarcador.Cargas.Carga Carga) transbordoGerado in listaTransbordo)
                    serHubCarga.InformarTransbordoCargaAtualizada(transbordoGerado.Transbordo, TipoAcaoCarga.Alterada, Usuario);

                return new JsonpResult(listaTransbordo.Count == 1 ? ObterTransbordo(listaTransbordo.FirstOrDefault().Transbordo, unitOfWork) : true);
            }
            catch (BaseException ex)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        //public async Task<IActionResult> CancelarTransbordo()
        //{
        //    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
        //    Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();

        //    try
        //    {
        //        unitOfWork.Start();
        //        Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
        //        Repositorio.Embarcador.Cargas.TransbordoMDFe repTransbordoMDFe = new Repositorio.Embarcador.Cargas.TransbordoMDFe(unitOfWork);
        //        Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
        //        Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

        //        Servicos.MDFe serMDFe = new Servicos.MDFe(unitOfWork);
        //        Servicos.Embarcador.Carga.MDFe serCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);

        //        int codigo = int.Parse(Request.Params("Codigo"));
        //        Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
        //        Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo = repTransbordo.BuscarPorCodigo(codigo);
        //        if (transbordo.SituacaoTransbordo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.EmTransporte || transbordo.SituacaoTransbordo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.CancelamentoRejeitado)
        //        {
        //            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFreteTerceiro.BuscarPorTransbordo(transbordo.Codigo);
        //            if (contratoFrete != null)
        //            {
        //                Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorContratoFrete(contratoFrete.Codigo);

        //                if (titulo == null || titulo.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada)
        //                {
        //                    if (contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado || contratoFrete.SituacaoContratoFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Cancelado)
        //                    {
        //                        contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Cancelado;
        //                        repContratoFreteTerceiro.Atualizar(contratoFrete);

        //                        if (titulo != null)
        //                            repTitulo.Deletar(titulo); //todo:desfazer movimentos

        //                    }
        //                    else
        //                    {
        //                        unitOfWork.Rollback();
        //                        return new JsonpResult(false, true, "Não é possível cancelar o transbordo pois a atual situação do contrato não permite");
        //                    }
        //                }
        //                else
        //                {
        //                    unitOfWork.Rollback();
        //                    return new JsonpResult(false, true, "Não é Cancelar o Transbordo, pois o título com o terceiro já foi quitado");
        //                }
        //            }


        //            List<Dominio.Entidades.Embarcador.Cargas.TransbordoMDFe> transbordoMDFes = repTransbordoMDFe.BuscarPorTransbordo(transbordo.Codigo);
        //            List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfeParaFila = new List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

        //            if (transbordoMDFes.Count > 0)
        //            {
        //                transbordo.SituacaoTransbordo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.EmCancelamento;

        //                foreach (Dominio.Entidades.Embarcador.Cargas.TransbordoMDFe transbordoMDFe in transbordoMDFes)
        //                {
        //                    TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(transbordoMDFe.MDFe.Empresa.FusoHorario);
        //                    DateTime dataEvento = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);

        //                    if (transbordoMDFe.MDFe.DataAutorizacao >= dataEvento.AddDays(-1))
        //                    {
        //                        if (transbordoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
        //                        {
        //                            if (serMDFe.Cancelar(transbordoMDFe.MDFe.Codigo, transbordo.Carga.Empresa.Codigo, "Solcitado o cancelamento do Transbordo gerado", unitOfWork, dataEvento))
        //                                mdfeParaFila.Add(transbordoMDFe.MDFe);
        //                            else
        //                            {
        //                                unitOfWork.Rollback();
        //                                return new JsonpResult(false, true, "Não foi possível cancelar o MDF-e.");
        //                            }
        //                        }
        //                        else
        //                        {
        //                            unitOfWork.Rollback();
        //                            return new JsonpResult(false, true, "A atual situação do MDF-e não permite seu cancelamento.");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (transbordoMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
        //                        {
        //                            transbordoMDFe.MDFe.MunicipioEncerramento = transbordo.localidadeTransbordo;
        //                            repMDFe.Atualizar(transbordoMDFe.MDFe);

        //                            if (serMDFe.Encerrar(transbordoMDFe.MDFe.Codigo, transbordo.Carga.Empresa.Codigo, DateTime.Now, unitOfWork, dataEvento))
        //                                mdfeParaFila.Add(transbordoMDFe.MDFe);
        //                            else
        //                            {
        //                                unitOfWork.Rollback();
        //                                return new JsonpResult(false, true, "Não foi possível encerrar o MDF-e.");
        //                            }
        //                        }
        //                        else
        //                        {
        //                            unitOfWork.Rollback();
        //                            return new JsonpResult(false, true, "A atual situação do MDF-e não permite seu encerramento.");
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                transbordo.SituacaoTransbordo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.Cancelado;
        //            }

        //            unitOfWork.CommitChanges();

        //            foreach (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe in mdfeParaFila)
        //            {
        //                if (!serCargaMDFe.AdicionarMDFeNaFilaDeConsulta(mdfe, WebServiceConsultaCTe))
        //                    return new JsonpResult(false, true, "Não foi possível adicionar o MDFe a fila.");
        //            }
        //            serHubCarga.InformarTransbordoCargaAtualizada(transbordo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao, this.Usuario);
        //            return new JsonpResult(ObterTransbordo(transbordo, unitOfWork));
        //        }
        //        else
        //        {
        //            unitOfWork.Rollback();
        //            return new JsonpResult(false, true, "Não é possível solicitar o cancelamento do transbordo em sua atual situação");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        unitOfWork.Rollback();
        //        Servicos.Log.TratarErro(ex);
        //        return new JsonpResult(false, "Ocorreu uma falha solicitar o cancelamento.");
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //}

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo = repTransbordo.BuscarPorCodigo(codigo);
                return new JsonpResult(ObterTransbordo(transbordo, unitOfWork));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> SalvarFreteTerceiroTransbordo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Transbordo"));
                Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
                Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo = repTransbordo.BuscarPorCodigo(codigo, true);

                Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFreteTerceiro.BuscarPorTransbordo(transbordo.Codigo);

                bool inserir = false;
                if (contratoFrete == null)
                {
                    inserir = true;
                    contratoFrete = new Dominio.Entidades.Embarcador.Terceiros.ContratoFrete
                    {
                        Carga = transbordo.Carga,
                        NumeroContrato = repContratoFreteTerceiro.BuscarProximoCodigo(),
                        SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aberto
                    };
                }
                else
                {
                    contratoFrete.Initialize();
                }

                contratoFrete.TipoFreteEscolhido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFreteEscolhido.Operador;
                contratoFrete.DataEmissaoContrato = DateTime.Now;
                contratoFrete.TransportadorTerceiro = transbordo.Veiculo.Proprietario;
                contratoFrete.Usuario = this.Usuario;
                contratoFrete.Transbordo = transbordo;

                decimal descontos = 0m, percentualAdiantamentoFretesTerceiro = 0m, valorOutrosAdiantamento = 0m, valorFreteSubcontratacao = 0m, percentualAbastecimentoFretesTerceiro = 0m;
                decimal.TryParse(Request.Params("PercentualAdiantamento"), out percentualAdiantamentoFretesTerceiro);
                decimal.TryParse(Request.Params("PercentualAbastecimento"), out percentualAbastecimentoFretesTerceiro);
                decimal.TryParse(Request.Params("ValorOutrosAdiantamento"), out valorOutrosAdiantamento);
                decimal.TryParse(Request.Params("ValorFreteSubcontratacao"), out valorFreteSubcontratacao);
                decimal.TryParse(Request.Params("Descontos"), out descontos);

                contratoFrete.ValorFreteSubcontratacao = valorFreteSubcontratacao;
                contratoFrete.Descontos = descontos;
                contratoFrete.PercentualAdiantamento = percentualAdiantamentoFretesTerceiro;
                contratoFrete.PercentualAbastecimento = percentualAbastecimentoFretesTerceiro;
                decimal valorTotal = contratoFrete.ValorFreteSubcontratacao + contratoFrete.ValorPedagio - contratoFrete.Descontos;

                contratoFrete.ValorAdiantamento = (valorTotal * percentualAdiantamentoFretesTerceiro) / 100;
                contratoFrete.ValorAbastecimento = valorTotal * (percentualAbastecimentoFretesTerceiro / 100);
                contratoFrete.ValorOutrosAdiantamento = valorOutrosAdiantamento;
                contratoFrete.Observacao = Request.Params("Observacao");

                if (inserir)
                    repContratoFreteTerceiro.Inserir(contratoFrete, Auditado);
                else
                    repContratoFreteTerceiro.Atualizar(contratoFrete, Auditado);


                //if(inserir)
                //{
                //    List<Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro> listaFiltrada = Servicos.Embarcador.Terceiros.ContratoFrete.VerificarRegrasAutorizacao(contratoFrete, unitOfWork);
                //    if (listaFiltrada.Count() > 0)
                //    {
                //        if (!Servicos.Embarcador.Terceiros.ContratoFrete.CriarRegrasAutorizacao(listaFiltrada, contratoFrete, contratoFrete.Carga.Operador, TipoServicoMultisoftware, unitOfWork.StringConexao, unitOfWork))
                //            contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.Aprovado;
                //        else
                //            contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.AgAprovacao;
                //    }
                //    else
                //        contratoFrete.SituacaoContratoFrete = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFrete.SemRegra;

                //    repContratoFreteTerceiro.Atualizar(contratoFrete);
                //}

                return new JsonpResult(ObterTransbordo(transbordo, unitOfWork));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> FinalizarIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Carga.Transbordo svcTransbordo = new Servicos.Embarcador.Carga.Transbordo(unitOfWork, TipoServicoMultisoftware, Auditado);

                Repositorio.Embarcador.Cargas.Transbordo repTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);
                Repositorio.Embarcador.Cargas.TransbordoIntegracao repTransbordoIntegracao = new Repositorio.Embarcador.Cargas.TransbordoIntegracao(unitOfWork);

                int codigoTransbordo = Request.GetIntParam("Transbordo");

                Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo = repTransbordo.BuscarPorCodigo(codigoTransbordo);

                if (transbordo == null)
                    throw new ControllerException(Localization.Resources.Cargas.Transbordo.NaoFoiPossivelEncontrarTransbordo);

                unitOfWork.Start();

                svcTransbordo.GerarCargaTransbordo(transbordo, Cliente);

                transbordo.SituacaoTransbordo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo.Finalizado;
                repTransbordo.Atualizar(transbordo, Auditado);

                List<Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao> listaIntegracaoPendente = repTransbordoIntegracao.BuscarPendentesPorTransbordo(codigoTransbordo);
                if (listaIntegracaoPendente != null && listaIntegracaoPendente.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.TransbordoIntegracao integracao in listaIntegracaoPendente)
                    {
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        integracao.ProblemaIntegracao = Localization.Resources.Cargas.Transbordo.EtapaFinalizadaManualmente;
                        repTransbordoIntegracao.Atualizar(integracao);
                    }
                }
                Servicos.Auditoria.Auditoria.Auditar(Auditado, transbordo, null, "Avançou a etapa de integração manualmente", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, true, Localization.Resources.Cargas.Transbordo.EtapaFinalizadaManualmente);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Transbordo.OcorreuUmaFalhaFinalizarEtapa);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void AvancarCargaConformeConfiguracao(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (configuracao.CargaTransbordoNaEtapaInicial)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                carga.SituacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Nova;
                repCarga.Atualizar(carga);

                svcCarga.FecharCarga(carga, unitOfWork, TipoServicoMultisoftware, this.Cliente, true);
            }
        }

        private Models.Grid.Grid ObterGridEntregas(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", 8, Models.Grid.Align.right, false, configuracaoGeralCarga.PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo);
            grid.AdicionarCabecalho("Número Entrega", "NumeroEntrega", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Cliente", "Cliente", 8, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Destino", "Destino", 8, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Tipo", "Tipo", 8, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Notas", "Notas", 8, Models.Grid.Align.left, false, configuracaoGeralCarga.PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo);
            grid.AdicionarCabecalho("Pedidos", "Pedidos", 8, Models.Grid.Align.left, false, configuracaoGeralCarga.PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo);

            return grid;
        }

        private string ObterGridCTes(ref Models.Grid.Grid grid, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {

            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("SituacaoCTe", false);
            grid.AdicionarCabecalho("CodigoCTE", false);
            grid.AdicionarCabecalho("CodigoEmpresa", false);
            grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Serie", "Serie", 5, Models.Grid.Align.center, true);
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho("T. Pagamento", "DescricaoTipoPagamento", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, true);
            }

            grid.AdicionarCabecalho("T. Serviço", "DescricaoTipoServico", 10, Models.Grid.Align.center, true);

            grid.AdicionarCabecalho("Destinatário", "Destinatario", 18, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Valor a Receber", "ValorFrete", 8, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Alíquota", "Aliquota", 5, Models.Grid.Align.right, false);

            string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

            if (propOrdenacao == "Remetente" || propOrdenacao == "Destinatario")
                propOrdenacao += ".Nome";
            if (propOrdenacao == "Destino")
                propOrdenacao = "LocalidadeTerminoPrestacao.Descricao";

            if (propOrdenacao == "DescricaoTipoPagamento")
                propOrdenacao = "TipoPagamento";

            if (propOrdenacao == "DescricaoTipoServico")
                propOrdenacao = "TipoServico";

            propOrdenacao = "CTe." + propOrdenacao;

            return propOrdenacao;
        }

        private dynamic ObterListaObjetosEntregas(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga)
        {
            var lista = (from obj in entregas
                         select new
                         {
                             obj.Codigo,
                             Tipo = obj.Coleta ? "Coleta" : "Entrega",
                             NumeroEntrega = obj.Codigo,
                             Cliente = obj.Cliente?.Descricao,
                             Destino = obj.Cliente?.Localidade?.Descricao,
                             NumeroCarga = obj.Carga.CodigoCargaEmbarcador,
                             Notas = configuracaoGeralCarga.PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo ? string.Join(", ", obj.NotasFiscais.Select(o => o.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero)) : string.Empty,
                             Pedidos = configuracaoGeralCarga.PermitirSelecionarMultiplasCargasParaAgruparNoTransbordo ? string.Join(", ", obj.Pedidos.Select(o => o.CargaPedido.Pedido.NumeroPedidoEmbarcador)) : string.Empty
                         }).ToList();

            return lista;
        }

        private dynamic ObterListaObjetosCTe(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes)
        {
            var lista = (from obj in cargaCTes
                         select new
                         {
                             obj.Codigo,
                             CodigoCTE = obj.CTe.Codigo,
                             obj.CTe.DescricaoTipoServico,
                             CodigoEmpresa = obj.CTe.Empresa.Codigo,
                             obj.CTe.Numero,
                             SituacaoCTe = obj.CTe.Status,
                             Serie = obj.CTe.Serie.Numero,
                             obj.CTe.DescricaoTipoPagamento,
                             Remetente = obj.CTe.Remetente != null ? obj.CTe.Remetente.Nome + "(" + obj.CTe.Remetente.CPF_CNPJ_Formatado + ")" : "",
                             Destinatario = obj.CTe.Remetente != null ? obj.CTe.Destinatario.Nome + "(" + obj.CTe.Destinatario.CPF_CNPJ_Formatado + ")" : "",
                             Destino = obj.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                             ValorFrete = obj.CTe.ValorAReceber.ToString("n2"),
                             Aliquota = obj.CTe.AliquotaICMS.ToString("n2"),
                         }).ToList();

            return lista;
        }

        private dynamic ObterTransbordo(Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TransbordoMDFe repTransbordoMDFe = new Repositorio.Embarcador.Cargas.TransbordoMDFe(unitOfWork);
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFreteTerceiro = new Repositorio.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

            int numeroMDFe = repTransbordoMDFe.ContarConsulta(transbordo.Codigo);
            Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contratoFrete = repContratoFreteTerceiro.BuscarPorTransbordo(transbordo.Codigo);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();
            List<Dominio.Entidades.Veiculo> reboques = transbordo.VeiculosVinculados.ToList();
            Dominio.Entidades.Veiculo reboque = reboques.Count > 0 ? reboques.ElementAt(0) : null;
            Dominio.Entidades.Veiculo segundoReboque = reboques.Count > 1 ? reboques.ElementAt(1) : null;
            Dominio.Entidades.Veiculo terceiroReboque = reboques.Count > 2 ? reboques.ElementAt(2) : null;

            var dynTransbordo = new
            {
                Carga = new
                {
                    transbordo.Carga.Codigo,
                    transbordo.Carga.CodigoCargaEmbarcador,
                    Filial = transbordo.Carga.Filial != null ? transbordo.Carga.Filial.Descricao : "",
                    NumeroReboques = transbordo.Carga.ModeloVeicularCarga?.NumeroReboques ?? 0
                },
                PossuiContrato = transbordo.Veiculo.Tipo == "T" ? true : false,
                ContratoFrete = serContratoFrete.ObterDetalhescontratoFrete(contratoFrete, unitOfWork),
                transbordo.Codigo,
                DataTransbordo = transbordo.DataTransbordo.ToString("dd/MM/yyyy HH:mm:ss"),
                transbordo.NumeroTransbordo,
                Motoristas = (from obj in transbordo.Motoristas.ToList()
                              select new
                              {
                                  Codigo = obj.Codigo,
                                  Descricao = obj.Nome
                              }).ToList(),
                Motorista = new { Codigo = transbordo.Motoristas.FirstOrDefault().Codigo, Descricao = BuscarMotorista(transbordo.Motoristas.ToList()) },
                Veiculo = new { Codigo = transbordo.Veiculo.Codigo, Descricao = BuscarPlacas(transbordo.Veiculo, reboques, configuracaoGeral.PermiteSelecionarPlacaPorTipoVeiculoTransbordo) },
                Reboque = new { Codigo = reboque?.Codigo, Descricao = reboque?.Descricao ?? string.Empty },
                SegundoReboque = new { Codigo = segundoReboque?.Codigo, Descricao = segundoReboque?.Descricao ?? string.Empty },
                TerceiroReboque = new { Codigo = terceiroReboque?.Codigo, Descricao = terceiroReboque?.Descricao ?? string.Empty },
                Empresa = new { Codigo = transbordo.Empresa != null ? transbordo.Empresa.Codigo : 0, Descricao = transbordo.Empresa != null ? transbordo.Empresa.RazaoSocial : "" },
                LocalidadeTransbordo = new { Codigo = transbordo.localidadeTransbordo.Codigo, Descricao = transbordo.localidadeTransbordo.DescricaoCidadeEstado },
                TipoOperacao = new { Codigo = transbordo.TipoOperacao?.Codigo ?? 0, Descricao = transbordo.TipoOperacao?.Descricao ?? string.Empty },
                TipoCarga = new { Codigo = transbordo.TipoDeCarga?.Codigo ?? 0, Descricao = transbordo.TipoDeCarga?.Descricao ?? string.Empty },
                transbordo.MotivoTransbordo,
                transbordo.SituacaoTransbordo,
                PossuiMDFe = numeroMDFe > 0 ? true : false,
                LancamentoEntregas = transbordo.ClonaLancamentoEntregas,
                LancamentoColetas = transbordo.ClonaLancamentoColetas,
                DataInicioViagem = transbordo.ClonaDataInicioViagemEntrega,
            };

            return dynTransbordo;
        }

        private string BuscarMotorista(List<Dominio.Entidades.Usuario> motoristas)
        {
            Dominio.Entidades.Usuario ultimoMotorista = motoristas.Last();
            string strMotorista = "";
            foreach (Dominio.Entidades.Usuario motorista in motoristas)
            {
                strMotorista += motorista.Nome;
                if (ultimoMotorista.Codigo != motorista.Codigo)
                {
                    strMotorista += ",";
                }
            }
            return strMotorista;
        }

        private string BuscarPlacas(Dominio.Entidades.Veiculo tracao, List<Dominio.Entidades.Veiculo> reboques, bool somenteTracao = false)
        {
            if (somenteTracao)
                return tracao.Placa;

            List<string> placas = new List<string> { tracao.Placa };

            if (reboques.Count > 0)
                placas.AddRange(reboques.Select(reboque => reboque.Placa));

            return string.Join("/", placas);
        }

        private (Dominio.Entidades.Embarcador.Cargas.Transbordo, Dominio.Entidades.Embarcador.Cargas.Carga) GerarTransbordoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Localidade localidadeTransbordo, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Veiculo veiculo, Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Transbordo repositorioTransbordo = new Repositorio.Embarcador.Cargas.Transbordo(unitOfWork);

            Servicos.Embarcador.Integracao.IntegracaoTransbordo servicoIntegracaoTransbordo = new Servicos.Embarcador.Integracao.IntegracaoTransbordo(unitOfWork);
            Servicos.Embarcador.Carga.Transbordo servicoTransbordo = new Servicos.Embarcador.Carga.Transbordo(unitOfWork, TipoServicoMultisoftware, Auditado);

            CamposControleEntregaColeta jsonControleEntrega = Newtonsoft.Json.JsonConvert.DeserializeObject<CamposControleEntregaColeta>(Request.Params("CamposControleEntregaColeta"));


            Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo = new Dominio.Entidades.Embarcador.Cargas.Transbordo()
            {
                NumeroTransbordo = repositorioTransbordo.BuscarProximoCodigo(),
                DataTransbordo = Request.GetDateTimeParam("DataTransbordo"),
                MotivoTransbordo = Request.GetStringParam("MotivoTransbordo"),
                SituacaoTransbordo = SituacaoTransbordo.Finalizado,
                Carga = carga,
                localidadeTransbordo = localidadeTransbordo,
                Empresa = empresa,
                Veiculo = veiculo,
                SegmentoGrupoPessoas = veiculo.GrupoPessoas,
                TipoOperacao = tipoOperacao,
                TipoDeCarga = tipoCarga,
                Entregas = entregas,
                ClonaDataInicioViagemEntrega = jsonControleEntrega.DataInicioViagem,
                ClonaLancamentoColetas = jsonControleEntrega.LancamentoColetas,
                ClonaLancamentoEntregas = jsonControleEntrega.LancamentoEntregas,
            };

            VincularReboques(transbordo, veiculo, unitOfWork);

            transbordo.Motoristas = new List<Dominio.Entidades.Usuario>() { motorista };

            Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Gerou Transbordo da Carga", unitOfWork);

            repositorioTransbordo.Inserir(transbordo, Auditado);

            Dominio.Entidades.Embarcador.Cargas.Carga cargaTransbordo = null;

            bool gerouIntegracoes = servicoIntegracaoTransbordo.AdicionarIntegracoesTransbordo(transbordo, unitOfWork);

            if (gerouIntegracoes)
                transbordo.SituacaoTransbordo = SituacaoTransbordo.AgIntegracao;
            else
                cargaTransbordo = servicoTransbordo.GerarCargaTransbordo(transbordo, Cliente);

            return ValueTuple.Create(transbordo, cargaTransbordo);
        }

        private List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> ConverterEntregas(List<int> codigosCargas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            dynamic dynCargaEntregaTransbordados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Entrega"));

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

            bool selecionouTodas = bool.Parse(Request.Params("SelecionarTodos"));
            if (!selecionouTodas)
            {
                foreach (dynamic dynCodigoCargaEntrega in dynCargaEntregaTransbordados)
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo((int)dynCodigoCargaEntrega.Codigo);
                    entregas.Add(cargaEntrega);
                }
            }
            else
            {
                entregas = repositorioCargaEntrega.BuscarPorCargaNaoRealizada(codigosCargas);

                foreach (dynamic dynCodigoCargaEntrega in dynCargaEntregaTransbordados)
                {
                    int codigo = (int)dynCodigoCargaEntrega.Codigo;
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = (from obj in entregas where obj.Codigo == codigo select obj).FirstOrDefault();

                    if (cargaEntrega != null)
                        entregas.Remove(cargaEntrega);
                }
            }

            return entregas;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTransbordo ObterFiltroPesquisaTransbordo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTransbordo filtroPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaTransbordo()
            {
                CodigoCarga = Request.GetIntParam("Carga"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                LocalidadeTransbordo = Request.GetIntParam("LocalidadeTransbordo"),
                NumeroTransbordo = Request.GetIntParam("NumeroTransbordo"),
                SituacaoTransbordo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTransbordo>("SituacaoTransbordo"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                CodigosFiliais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork),
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                filtroPesquisa.CodigosEmpresa = new List<int>() { Empresa.Codigo };
                filtroPesquisa.CodigosEmpresa.AddRange(Empresa.Filiais.ToList().Select(o => o.Codigo).ToList());
            }
            else
            {
                int codigoEmpresa = Request.GetIntParam("Empresa");
                if (codigoEmpresa > 0)
                    filtroPesquisa.CodigosEmpresa = new List<int>() { codigoEmpresa };
            }

            return filtroPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "LocalidadeTransbordo")
                propriedadeOrdenar += ".Descricao";

            if (propriedadeOrdenar == "Empresa")
                propriedadeOrdenar += ".RazaoSocial";

            return propriedadeOrdenar;
        }

        private void VincularReboques(Dominio.Entidades.Embarcador.Cargas.Transbordo transbordo, Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoReboque = Request.GetIntParam("Reboque");
            int codigoSegundoReboque = Request.GetIntParam("SegundoReboque");
            int codigoTerceiroReboque = Request.GetIntParam("TerceiroReboque");

            List<int> codigosReboques = new List<int>();

            if (codigoReboque > 0)
                codigosReboques.Add(codigoReboque);

            if (codigoSegundoReboque > 0)
                codigosReboques.Add(codigoSegundoReboque);

            if (codigoTerceiroReboque > 0)
                codigosReboques.Add(codigoTerceiroReboque);

            if (codigosReboques.Count > 0)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                transbordo.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                List<Dominio.Entidades.Veiculo> reboques = repVeiculo.BuscarPorCodigo(codigosReboques);

                foreach (Dominio.Entidades.Veiculo reboque in reboques)
                    transbordo.VeiculosVinculados.Add(reboque);

                return;
            }

            transbordo.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>(veiculo.VeiculosVinculados);
        }

        #endregion
    }
}
