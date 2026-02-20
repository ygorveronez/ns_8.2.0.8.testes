using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/PainelVeiculo")]
    public class PainelVeiculoController : BaseController
    {
        #region Construtores

        public PainelVeiculoController(Conexao conexao) : base(conexao) { }

        #endregion

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.Prop("Codigo");
                grid.Prop("NumeroFrota").Nome("Nº Frota").Align(Models.Grid.Align.right).Tamanho(8);
                grid.Prop("Placa").Nome("Placa").Align(Models.Grid.Align.left).Tamanho(12);
                grid.Prop("Modelo").Nome("Modelo").Align(Models.Grid.Align.left).Tamanho(12);
                grid.Prop("Reboque").Nome("Reboque(s)").Align(Models.Grid.Align.left).Tamanho(12).Ord(false);
                grid.Prop("Motorista").Nome("Motorista").Align(Models.Grid.Align.left).Tamanho(15);
                grid.Prop("Transportador").Nome("Transportador").Align(Models.Grid.Align.left).Tamanho(15);
                grid.Prop("SituacaoVeiculo").Nome("Situação").Align(Models.Grid.Align.left).Tamanho(15).Ord(false);
                grid.Prop("Cliente").Nome("Cliente(s) Atual").Align(Models.Grid.Align.center).Tamanho(15);
                grid.Prop("StatusVazio").Nome("Vazio").Align(Models.Grid.Align.center).Tamanho(8).Ord(false);
                grid.Prop("StatusAvisado").Nome("Avis. p/ Carregamento").Align(Models.Grid.Align.center).Tamanho(10).Ord(false);
                grid.Prop("PrevisaoDisponivel").Nome("Prev. Disponível").Align(Models.Grid.Align.center).Tamanho(12).Ord(false);
                grid.Prop("LocalPrevisto").Nome("Local Previsto").Align(Models.Grid.Align.left).Tamanho(15).Ord(false);
                grid.Prop("Carga").Nome("Carga").Align(Models.Grid.Align.left).Tamanho(15).Ord(false);
                grid.Prop("Proprietario").Nome("Proprietário").Align(Models.Grid.Align.left).Tamanho(15).Ord(false);
                grid.Prop("TipoFrota").Nome("Tipo de Frota").Align(Models.Grid.Align.left).Tamanho(15).Ord(false);
                grid.Prop("CentroCarregamento").Nome("Centro de Carregamento").Align(Models.Grid.Align.left).Tamanho(15).Ord(false);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Dados do filtro
                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaPainelVeiculo filtroPesquisa = ObterFiltrosPesquisa();

                int inicio = Request.GetIntParam("inicio");
                int limite = Request.GetIntParam("limite");
                if (inicio <= 0)
                    inicio = grid.inicio;

                if (!filtroPesquisa.HabilitarPainel)
                    limite = limite * 2;
                if (limite <= 0)
                    limite = grid.limite;
                else
                    grid.limite = limite;

                // Consulta
                IList<Dominio.ObjetosDeValor.Embarcador.Veiculos.PainelVeiculo> listaGrid = repVeiculo.ConsultaPainelVeiculoObjetoDeValor(filtroPesquisa, propOrdenar, grid.dirOrdena, inicio, limite);
                int totalRegistros = repVeiculo.ContarPainelVeiculoObjetoDeValor(filtroPesquisa);

                var lista = (from obj in listaGrid
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 SituacaoVeiculo = obj.DescricaoSituacao,
                                 obj.NumeroFrota,
                                 Placa = obj.Placa_Formatada,
                                 Proprietario = obj.Proprietario,
                                 TipoFrota = obj.TipoFrotaDescricao,
                                 Carga = obj.Carga,
                                 CentroCarregamento = obj.CentroCarregamento,
                                 Modelo = obj.Modelo,
                                 Reboque = obj.Reboque,
                                 Motorista = obj.Motorista,
                                 Transportador = obj.Transportador,
                                 Cliente = obj.ClienteViagem,
                                 StatusVazio = obj.StatusVazioDescricao,
                                 StatusAvisado = obj.StatusAvisadoDescricao,
                                 PrevisaoDisponivel = obj.PrevisaoDisponivel.HasValue ? obj.PrevisaoDisponivel.Value.ToString("dd/MM/yyyy hh:MM:ss") : string.Empty,
                                 LocalPrevisto = obj.LocalPrevisto,
                                 DT_RowClass = obj.DT_RowClass
                             }).ToList();

                grid.setarQuantidadeTotal(totalRegistros);

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

        private Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaPainelVeiculo ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaPainelVeiculo
            {
                Placa = Request.GetStringParam("Placa"),
                NumeroFrota = Request.GetStringParam("NumeroFrota"),
                TipoVeiculo = Request.GetStringParam("TipoVeiculo"),
                TipoPropriedade = Request.GetStringParam("TipoPropriedade"),

                CodigoMarcaVeiculo = Request.GetIntParam("MarcaVeiculo"),
                CodigoModeloVeiculo = Request.GetIntParam("ModeloVeiculo"),
                CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoLocalPrevisto = Request.GetIntParam("LocalPrevisto"),
                CodigoTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Usuario.Empresa.Codigo : Request.GetIntParam("Transportador"),
                CodigoProprietario = Request.GetDoubleParam("Proprietario"),
                DataInicioDisponivel = Request.GetNullableDateTimeParam("DataInicioDisponivel"),
                DataFimDisponivel = Request.GetNullableDateTimeParam("DataFimDisponivel"),
                DataSituacao = Request.GetNullableDateTimeParam("DataSituacao")?.AddDays(1).AddTicks(-1),
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                Situacao = Request.GetEnumParam<SituacaoAtivoPesquisa>("Ativo"),
                TipoFrota = Request.GetEnumParam("TipoFrota", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFrota.NaoDefinido),
                SituacaoVeiculo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo>("SituacaoVeiculo"),

                HabilitarPainel = Request.GetNullableBoolParam("HabilitarPainel") ?? false,
            };
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.SituacaoVeiculo repSituacaoVeiculo = new Repositorio.Embarcador.Veiculos.SituacaoVeiculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoLavacao repositorioVeiculoLavacao = new Repositorio.Embarcador.Veiculos.VeiculoLavacao(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigo);

                // Valida
                if (veiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo ultimaSituacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo situacaoAtual = veiculo.SituacaoVeiculo.HasValue ? veiculo.SituacaoVeiculo.Value : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Disponivel;

                if (situacaoAtual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem)
                    ultimaSituacao = repSituacaoVeiculo.BuscarUltimoPorVeiculo(codigo, situacaoAtual);
                else if (situacaoAtual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao)
                    ultimaSituacao = repSituacaoVeiculo.BuscarUltimoPorVeiculo(codigo, situacaoAtual);

                Dominio.Entidades.Usuario veiculoMotorista = repVeiculoMotorista.BuscarMotoristaPrincipal(veiculo.Codigo);
                List<Dominio.Entidades.Embarcador.Veiculos.VeiculoLavacao> veiculosLavacao = repositorioVeiculoLavacao.BuscarPorVeiculo(veiculo.Codigo);

                // Formata retorno
                var retorno = new
                {

                    veiculo.Codigo,
                    SituacaoAtual = situacaoAtual,
                    IndicacaoVeiculoVazio = false,
                    IndicacaoAvisoCarregamento = false,
                    Usuario = this.Usuario != null ? new { Codigo = this.Usuario.Codigo, Descricao = this.Usuario.Nome } : null,
                    Motorista = veiculoMotorista != null ? new { Codigo = veiculoMotorista.Codigo, Descricao = veiculoMotorista.Nome } : null,
                    LocalAtual = veiculo.LocalidadeAtual != null ? new { Codigo = veiculo.LocalidadeAtual.Codigo, Descricao = veiculo.LocalidadeAtual.Descricao } : null,
                    DataHoraIndicacao = DateTime.Now.Date.ToString("dd/MM/yyyy") + " " + string.Format("{0:00}:{1:00}", DateTime.Now.Hour, DateTime.Now.Minute),
                    VeiculoVazio = veiculo.VeiculoVazio != true,
                    AvisadoCarregamento = veiculo.AvisadoCarregamento != true,
                    PainelVeiculoViagem = new
                    {
                        DataHoraSaidaInicioViagem = situacaoAtual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem && ultimaSituacao != null && ultimaSituacao.DataHoraSaidaInicioViagem.HasValue ? ultimaSituacao.DataHoraSaidaInicioViagem.Value.Date.ToString("dd/MM/yyyy") + " " + string.Format("{0:00}:{1:00}", ultimaSituacao.DataHoraSaidaInicioViagem.Value.Hour, ultimaSituacao.DataHoraSaidaInicioViagem.Value.Minute) : string.Empty,
                        DataHoraPrevisaoRetornoInicioViagem = situacaoAtual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem && ultimaSituacao != null && ultimaSituacao.DataHoraPrevisaoRetornoInicioViagem.HasValue ? ultimaSituacao.DataHoraPrevisaoRetornoInicioViagem.Value.Date.ToString("dd/MM/yyyy") + " " + string.Format("{0:00}:{1:00}", ultimaSituacao.DataHoraPrevisaoRetornoInicioViagem.Value.Hour, ultimaSituacao.DataHoraPrevisaoRetornoInicioViagem.Value.Minute) : string.Empty,
                        LocalidadeDestinoInicioViagem = situacaoAtual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem && ultimaSituacao != null && ultimaSituacao.LocalidadeDestinoInicioViagem != null ? new { Codigo = ultimaSituacao.LocalidadeDestinoInicioViagem.Codigo, Descricao = ultimaSituacao.LocalidadeDestinoInicioViagem.Descricao } : null,
                        DataHoraRetornoViagem = string.Empty,
                        LocalidadeRetornoViagem = string.Empty
                    },
                    PainelVeiculoManutencao = new
                    {
                        DataHoraEntradaManutencao = situacaoAtual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao && ultimaSituacao != null && ultimaSituacao.DataHoraEntradaManutencao.HasValue ? ultimaSituacao.DataHoraEntradaManutencao.Value.Date.ToString("dd/MM/yyyy") + " " + string.Format("{0:00}:{1:00}", ultimaSituacao.DataHoraEntradaManutencao.Value.Hour, ultimaSituacao.DataHoraEntradaManutencao.Value.Minute) : string.Empty,
                        DataHoraPrevisaoSaidaManutencao = situacaoAtual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao && ultimaSituacao != null && ultimaSituacao.DataHoraPrevisaoSaidaManutencao.HasValue ? ultimaSituacao.DataHoraPrevisaoSaidaManutencao.Value.Date.ToString("dd/MM/yyyy") + " " + string.Format("{0:00}:{1:00}", ultimaSituacao.DataHoraPrevisaoSaidaManutencao.Value.Hour, ultimaSituacao.DataHoraPrevisaoSaidaManutencao.Value.Minute) : string.Empty,
                        DataHoraSaidaManutencao = string.Empty,
                        OrdemServicoFrota = string.Empty,
                        TipoOrdemServico = situacaoAtual == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao && ultimaSituacao != null && ultimaSituacao.TipoOrdemServico != null ? new { Codigo = ultimaSituacao.TipoOrdemServico.Codigo, Descricao = ultimaSituacao.TipoOrdemServico.Descricao } : null
                    },
                    PainelVeiculoVeiculosVinculados = new
                    {
                        ListaReboques = veiculo.VeiculosVinculados != null ? (from obj in veiculo.VeiculosVinculados
                                                                              orderby obj.Placa
                                                                              select new
                                                                              {
                                                                                  obj.Codigo,
                                                                                  Placa = obj.Placa
                                                                              }).ToList() : null
                    },
                    HistoricoLavacao = veiculosLavacao.Count > 0 ? (from veiculoLavacaoAnexo in veiculosLavacao
                                                                    select new
                                                                    {
                                                                        veiculoLavacaoAnexo.Codigo,
                                                                        DataLavacao = veiculoLavacaoAnexo.DataLavacao.ToString("d"),
                                                                        NomeArquivoAnexoAntesLavacao = veiculoLavacaoAnexo.NomeArquivoAntesLavacaoSumarizado,
                                                                        NomeArquivoAnexoDepoisLavacao = veiculoLavacaoAnexo.NomeArquivoDepoisLavacaoSumarizado,
                                                                        TipoAnexo = new { Codigo = 0, Descricao = string.Empty }
                                                                    }
                                                                    ).ToList() : null

                };
                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Embarcador.Veiculos.SituacaoVeiculo repSituacaoVeiculo = new Repositorio.Embarcador.Veiculos.SituacaoVeiculo(unitOfWork);
                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
                Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo repOrdemServicoFrotaTipo = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo(unitOfWork);
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigo, true);

                // Valida
                if (veiculo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                bool indicacaoVeiculoVazio = false, indicacaoAvisoCarregamento = false, veiculoVazio = false, avisadoCarregamento = false, indicacaoViagem = false, indicacaoManutencao = false;
                bool.TryParse(Request.Params("IndicacaoVeiculoVazio"), out indicacaoVeiculoVazio);
                bool.TryParse(Request.Params("IndicacaoAvisoCarregamento"), out indicacaoAvisoCarregamento);
                bool.TryParse(Request.Params("IndicacaoViagem"), out indicacaoViagem);
                bool.TryParse(Request.Params("IndicacaoManutencao"), out indicacaoManutencao);

                bool.TryParse(Request.Params("VeiculoVazio"), out veiculoVazio);
                bool.TryParse(Request.Params("AvisadoCarregamento"), out avisadoCarregamento);

                int codigoUsuario = 0, codigoMotorista = 0, codigoLocalAtual = 0;
                int.TryParse(Request.Params("Usuario"), out codigoUsuario);
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("LocalAtual"), out codigoLocalAtual);

                DateTime dataHoraIndicacao;
                DateTime.TryParse(Request.Params("DataHoraIndicacao"), out dataHoraIndicacao);

                DateTime dataHoraSaidaInicioViagem, dataHoraPrevisaoRetornoInicioViagem, dataHoraRetornoViagem;
                DateTime.TryParse(Request.Params("DataHoraSaidaInicioViagem"), out dataHoraSaidaInicioViagem);
                DateTime.TryParse(Request.Params("DataHoraPrevisaoRetornoInicioViagem"), out dataHoraPrevisaoRetornoInicioViagem);
                DateTime.TryParse(Request.Params("DataHoraRetornoViagem"), out dataHoraRetornoViagem);

                int codigoLocalidadeDestinoInicioViagem = 0, codigoLocalidadeRetornoViagem = 0;
                int.TryParse(Request.Params("LocalidadeDestinoInicioViagem"), out codigoLocalidadeDestinoInicioViagem);
                int.TryParse(Request.Params("LocalidadeRetornoViagem"), out codigoLocalidadeRetornoViagem);

                DateTime dataHoraEntradaManutencao, dataHoraPrevisaoSaidaManutencao, dataHoraSaidaManutencao;
                DateTime.TryParse(Request.Params("DataHoraEntradaManutencao"), out dataHoraEntradaManutencao);
                DateTime.TryParse(Request.Params("DataHoraPrevisaoSaidaManutencao"), out dataHoraPrevisaoSaidaManutencao);
                DateTime.TryParse(Request.Params("DataHoraSaidaManutencao"), out dataHoraSaidaManutencao);

                int tipoOrdemServico = Request.GetIntParam("TipoOrdemServico");

                int codigoOrdemServicoFrota = 0;
                int.TryParse(Request.Params("OrdemServicoFrota"), out codigoOrdemServicoFrota);

                unitOfWork.Start();

                if (indicacaoVeiculoVazio)
                    veiculo.VeiculoVazio = veiculoVazio;
                else if (indicacaoAvisoCarregamento)
                    veiculo.AvisadoCarregamento = avisadoCarregamento;
                if (codigoLocalAtual > 0)
                    veiculo.LocalidadeAtual = repLocalidade.BuscarPorCodigo(codigoLocalAtual);

                if (codigoMotorista > 0)
                {
                    //veiculo.Motorista = repUsuario.BuscarPorCodigo(codigoMotorista);

                    Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(codigoMotorista);
                    if (motorista != null)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, $"Removido motorista principal.", unitOfWork);
                        repVeiculoMotorista.DeletarMotoristaPrincipal(veiculo.Codigo);
                        Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista VeiculoMotoristaPrincipal = new Dominio.Entidades.Embarcador.Veiculos.VeiculoMotorista
                        {
                            CPF = motorista.CPF,
                            Motorista = motorista,
                            Nome = motorista.Nome,
                            Veiculo = veiculo,
                            Principal = true
                        };

                        repVeiculoMotorista.Inserir(VeiculoMotoristaPrincipal);
                    }
                }


                if (indicacaoViagem)
                {
                    if (dataHoraPrevisaoRetornoInicioViagem > DateTime.MinValue && codigoLocalidadeDestinoInicioViagem > 0 && dataHoraRetornoViagem <= DateTime.MinValue && codigoLocalidadeRetornoViagem <= 0)
                    {
                        veiculo.SituacaoVeiculo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem;
                        veiculo.DataHoraPrevisaoDisponivel = dataHoraPrevisaoRetornoInicioViagem;
                        veiculo.LocalidadeAtual = repLocalidade.BuscarPorCodigo(codigoLocalidadeDestinoInicioViagem);
                        veiculo.VeiculoVazio = false;
                    }
                    else if (dataHoraRetornoViagem > DateTime.MinValue && codigoLocalidadeRetornoViagem > 0)
                    {
                        veiculo.SituacaoVeiculo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Disponivel;
                        //veiculo.VeiculoVazio = true;
                        veiculo.DataHoraPrevisaoDisponivel = null;
                        veiculo.AvisadoCarregamento = false;
                        veiculo.LocalidadeAtual = repLocalidade.BuscarPorCodigo(codigoLocalidadeRetornoViagem);
                    }
                }
                else if (indicacaoManutencao)
                {
                    if (dataHoraPrevisaoSaidaManutencao > DateTime.MinValue && dataHoraSaidaManutencao <= DateTime.MinValue && codigoOrdemServicoFrota <= 0)
                    {
                        veiculo.SituacaoVeiculo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao;
                        veiculo.DataHoraPrevisaoDisponivel = dataHoraPrevisaoSaidaManutencao;
                        veiculo.LocalidadeAtual = null;
                        veiculo.VeiculoVazio = false;
                        veiculo.AvisadoCarregamento = false;
                    }
                    else if (dataHoraSaidaManutencao > DateTime.MinValue && codigoOrdemServicoFrota > 0)
                    {
                        veiculo.SituacaoVeiculo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Disponivel;
                        veiculo.DataHoraPrevisaoDisponivel = null;
                        //veiculo.VeiculoVazio = true;
                        veiculo.LocalidadeAtual = repOrdemServicoFrota.BuscarPorCodigo(codigoOrdemServicoFrota).Empresa?.Localidade ?? null;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params("ListaReboques")))
                {
                    dynamic listaReboques = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ListaReboques"));
                    if (listaReboques != null && listaReboques.Count > 0)
                    {
                        if (veiculo.VeiculosVinculados == null)
                            veiculo.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                        veiculo.VeiculosVinculados.Clear();
                    }
                    foreach (var jReboque in listaReboques)
                    {
                        int codigoReboque = 0;
                        int.TryParse((string)jReboque.Reboque.Codigo, out codigoReboque);
                        if (codigo > 0)
                        {
                            Dominio.Entidades.Veiculo reb = repVeiculo.BuscarPorCodigo(codigoReboque);
                            veiculo.VeiculosVinculados.Add(reb);
                        }
                    }
                }
                repVeiculo.Atualizar(veiculo, Auditado);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo situacaoVeiculo;
                if (indicacaoVeiculoVazio)
                    situacaoVeiculo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Vazio;
                else if (indicacaoAvisoCarregamento)
                    situacaoVeiculo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.AvisoCarregamento;
                else if (indicacaoManutencao)
                    situacaoVeiculo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao;
                else if (indicacaoViagem)
                    situacaoVeiculo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem;
                else
                    situacaoVeiculo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Disponivel;

                Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo situacao = null;
                List<Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo> situacoes = await repSituacaoVeiculo.BuscarHistoricoPorVeículoAsync(veiculo.Codigo);
                if (situacoes.Count > 0 && situacaoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem || situacaoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao)
                    situacao = situacoes.OrderByDescending(situacao => situacao.Codigo).FirstOrDefault(situacao => situacao.Situacao == situacaoVeiculo && !situacao.DataHoraSaidaInicioViagem.HasValue && !situacao.DataHoraSaidaManutencao.HasValue);
                   
                situacao ??= new Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo();

                situacao.Initialize();

                if (situacao.Codigo == 0)
                {
                    situacao.DataHoraEmissao = DateTime.Now;
                    situacao.DataHoraSituacao = dataHoraIndicacao;
                }
                situacao.Veiculo = veiculo;
                situacao.TipoOrdemServico = repOrdemServicoFrotaTipo.BuscarPorCodigo(tipoOrdemServico);

                if (codigoLocalAtual > 0)
                    situacao.Localidade = repLocalidade.BuscarPorCodigo(codigoLocalAtual);
                if (codigoMotorista > 0)
                    situacao.Motorista = repUsuario.BuscarPorCodigo(codigoMotorista);

                if (indicacaoViagem)
                {
                    if (dataHoraSaidaInicioViagem > DateTime.MinValue)
                        situacao.DataHoraSaidaInicioViagem = dataHoraSaidaInicioViagem;
                    if (dataHoraPrevisaoRetornoInicioViagem > DateTime.MinValue)
                        situacao.DataHoraPrevisaoRetornoInicioViagem = dataHoraPrevisaoRetornoInicioViagem;
                    if (dataHoraRetornoViagem > DateTime.MinValue)
                        situacao.DataHoraRetornoViagem = dataHoraRetornoViagem;
                    if (codigoLocalidadeDestinoInicioViagem > 0)
                        situacao.LocalidadeDestinoInicioViagem = repLocalidade.BuscarPorCodigo(codigoLocalidadeDestinoInicioViagem);
                    if (codigoLocalidadeRetornoViagem > 0)
                        situacao.LocalidadeRetornoViagem = repLocalidade.BuscarPorCodigo(codigoLocalidadeRetornoViagem);
                }
                else if (indicacaoManutencao)
                {
                    if (dataHoraEntradaManutencao > DateTime.MinValue)
                        situacao.DataHoraEntradaManutencao = dataHoraEntradaManutencao;
                    if (dataHoraPrevisaoSaidaManutencao > DateTime.MinValue)
                        situacao.DataHoraPrevisaoSaidaManutencao = dataHoraPrevisaoSaidaManutencao;
                    if (dataHoraSaidaManutencao > DateTime.MinValue)
                        situacao.DataHoraSaidaManutencao = dataHoraSaidaManutencao;
                    if (codigoOrdemServicoFrota > 0)
                        situacao.OrdemServicoFrota = repOrdemServicoFrota.BuscarPorCodigo(codigoOrdemServicoFrota);
                }
                else if (indicacaoVeiculoVazio)
                    situacao.VeiculoVazio = veiculoVazio;
                else if (indicacaoAvisoCarregamento)
                    situacao.AvisadoCarregamento = avisadoCarregamento;

                situacao.Situacao = situacaoVeiculo;

                if (!string.IsNullOrEmpty(Request.Params("ListaReboques")))
                {
                    dynamic listaReboques = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ListaReboques"));
                    if (listaReboques != null && listaReboques.Count > 0)
                    {
                        if (situacao.VeiculosVinculadosSituacao == null)
                            situacao.VeiculosVinculadosSituacao = new List<Dominio.Entidades.Veiculo>();
                        situacao.VeiculosVinculadosSituacao.Clear();
                    }
                    foreach (var jReboque in listaReboques)
                    {
                        int codigoReboque = 0;
                        int.TryParse((string)jReboque.Reboque.Codigo, out codigoReboque);
                        if (codigo > 0)
                        {
                            Dominio.Entidades.Veiculo reb = repVeiculo.BuscarPorCodigo(codigoReboque);
                            situacao.VeiculosVinculadosSituacao.Add(reb);
                        }
                    }
                }

                if (situacao.Codigo > 0)
                    repSituacaoVeiculo.Atualizar(situacao, Auditado);
                else
                    repSituacaoVeiculo.Inserir(situacao, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, situacao, null, "Alteração da situação do veículo pelo painel", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, null, "Alteração da situação do veículo pelo painel", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarHistoricoSituacoesVeiculo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoVeiculo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Veiculos.SituacaoVeiculo repositorioSituacaoVeiculo = new(unitOfWork, cancellationToken);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Placa", "Placa", 8, Models.Grid.Align.center, false, false, false, false, true);
                grid.AdicionarCabecalho("Data Inclusão", "DataInclusao", 12, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Data Início", "DataInicio", 10, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Data Previsão Fim", "DataPrevisaoFim", 30, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Data Fim", "DataFim", 15, Models.Grid.Align.center, false, false, false, false, true);
                grid.AdicionarCabecalho("Tipo OS", "TipoOS", 10, Models.Grid.Align.center, false, false, false, false, true);

                List<Dominio.Entidades.Embarcador.Veiculos.SituacaoVeiculo> situacoes = await repositorioSituacaoVeiculo.BuscarHistoricoPorVeículoAsync(codigoVeiculo);
                dynamic retorno = (from situacao in situacoes
                                   select new
                                   {
                                       Codigo = situacao.Codigo,
                                       Placa = situacao.Veiculo.Placa,
                                       DataInclusao = situacao.DataHoraEmissao.ToDateTimeString(),
                                       Situacao = situacao.Situacao.ObterDescricao(),
                                       TipoOS = situacao.TipoOrdemServico?.Descricao ?? string.Empty,
                                       DataInicio = (situacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem ? situacao.DataHoraSaidaInicioViagem :
                                                    situacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao ? situacao.DataHoraEntradaManutencao :
                                                    situacao.DataHoraSituacao)?.ToDateTimeString() ?? string.Empty,
                                       DataPrevisaoFim = (situacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem ? situacao.DataHoraPrevisaoRetornoInicioViagem :
                                                    situacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao ? situacao.DataHoraPrevisaoSaidaManutencao :
                                                    null)?.ToDateTimeString() ?? string.Empty,
                                       DataFim = (situacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem ? situacao.DataHoraRetornoViagem :
                                                    situacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao ? situacao.DataHoraSaidaManutencao :
                                                    null)?.ToDateTimeString() ?? string.Empty,
                                   }
                    ).ToList();
                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(retorno.Count);
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o histórico do veículo.");
            }
            finally
            {
                unitOfWork.DisposeAsync();
            }
        }
        #region Métodos Privados
        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */
            if (propOrdenar == "Modelo") propOrdenar = "Modelo.Descricao";
            else if (propOrdenar == "Motorista") propOrdenar = "Motorista.Nome";
            else if (propOrdenar == "Situacao") propOrdenar = "SituacaoVeiculo";
        }
        private string ObterCargaVeiculo(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repcarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            return repcarga.BuscaCargaEmAbertoPorVeiculo(veiculo.Placa).FirstOrDefault()?.CodigoCargaEmbarcador ?? "";
        }
        private string ObterCentroCarregamentoVeiculo(Dominio.Entidades.Veiculo veiculo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repfilacarregamento = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);
            return repfilacarregamento.BuscarAtivaPorVeiculo(veiculo.Codigo)?.CentroCarregamento?.Descricao ?? "";
        }
        private bool VeiculoPossuiCentroCarregamento(Dominio.Entidades.Veiculo veiculo, int codigoCentroCarregamento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repFilaCarregamento = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);

            var centroCarregamento = repFilaCarregamento.BuscarAtivaPorVeiculo(veiculo.Codigo)?.CentroCarregamento;

            // Retorna true se o veículo pertence ao centro especificado 
            return centroCarregamento != null && (centroCarregamento.Codigo == codigoCentroCarregamento);
        }

        #endregion
    }
}
