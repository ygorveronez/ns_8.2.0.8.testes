using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using SGTAdmin.Controllers;
using System.Globalization;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/PosicaoFrota")]
    public class PosicaoFrotaController : BaseController
    {
        #region Construtores

        public PosicaoFrotaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region constantes

        private const string NENHUM_DESCRICAO = "Nenhum";
        private const string NENHUM_COR = "#6f6f6f";

        #endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Legendas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                List<dynamic> listGrupos = new List<dynamic>();

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento)
                {
                    Repositorio.Embarcador.Pedidos.GrupoTipoOperacao repGrupoTipoOperacao = new Repositorio.Embarcador.Pedidos.GrupoTipoOperacao(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao> gruposTipoOperacao = repGrupoTipoOperacao.BuscarAtivos();
                    foreach (Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao grupo in gruposTipoOperacao)
                    {
                        listGrupos.Add(new
                        {
                            grupo.Descricao,
                            grupo.Cor
                        });
                    }
                }
                else
                {
                    Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem repMonitoramentoGrupoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem(unitOfWork);
                    Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem> gruposStatusViagem = repMonitoramentoGrupoStatusViagem.BuscarAtivos();
                    foreach (Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem grupo in gruposStatusViagem)
                    {
                        List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> status = repMonitoramentoStatusViagem.BuscarPorGrupo(grupo.Codigo);
                        listGrupos.Add(new
                        {
                            grupo.Descricao,
                            grupo.Cor,
                            StatusViagem = (
                                from obj in status
                                select new
                                {
                                    obj.Descricao,
                                    obj.Cor
                                }
                            ).ToList()
                        });
                    }
                }

                // Categorias das pessoas
                Repositorio.Embarcador.Pessoas.CategoriaPessoa repoCategoria = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa> listCategorias = repoCategoria.BuscarTodos();

                return new JsonpResult(new
                {
                    Grupos = listGrupos,
                    Categorias = listCategorias
                });

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Models.Grid.Grid grid = ObterGridPesquisa(unitOfWork);
                Repositorio.Embarcador.Logistica.Locais repLocais = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrota> listaPosicaoFrota = ConsultarPosicaoFrota(parametrosConsulta, unitOfWork, configuracao);

                bool filtrarPosicoesEmLocais = Request.GetBoolParam("FiltrarLocais");
                List<int> CodigosLocais = new List<int>();
                if (filtrarPosicoesEmLocais)
                    CodigosLocais = Request.GetListParam<int>("Locais");

                List<Dominio.Entidades.Embarcador.Logistica.Locais> locais = new List<Dominio.Entidades.Embarcador.Logistica.Locais>();
                if (filtrarPosicoesEmLocais)//carregar os locais de traking
                {
                    if (CodigosLocais.Count > 0)
                        locais = repLocais.BuscarPorCodigos(CodigosLocais);
                    else
                        locais = repLocais.BuscarTodos();


                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrota> listVeiculosResumido = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrota>();
                    foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrota veiculo in listaPosicaoFrota)
                    {
                        Dominio.Entidades.Embarcador.Logistica.Locais localAtual = Servicos.Embarcador.Monitoramento.Localizacao.ValidarArea.BuscarLocalEmArea(locais.ToArray(), veiculo.Latitude, veiculo.Longitude);
                        if (localAtual != null)
                            listVeiculosResumido.Add(veiculo);
                    }

                    listaPosicaoFrota = listVeiculosResumido;
                }


                AdicionarLinhasNoGrid(listaPosicaoFrota, grid);
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Exportar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Models.Grid.Grid grid = ObterGridPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrota> listaPosicaoFrota = ConsultarPosicaoFrota(parametrosConsulta, unitOfWork, configuracao);
                AdicionarLinhasNoGrid(listaPosicaoFrota, grid);
                byte[] bArquivo = grid.GerarExcel();
                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosMapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                bool clientesComVeiculoEmAlvo = Request.GetBoolParam("ClientesComVeiculoEmAlvo");
                bool clientesAlvosEstrategicos = Request.GetBoolParam("ClientesAlvosEstrategicos");
                bool exibirFiliaisEBases = Request.GetBoolParam("ExibirFiliaisEBases");

                bool filtrarPosicoesEmLocais = Request.GetBoolParam("FiltrarLocais");
                List<int> CodigosLocais = new List<int>();
                if (filtrarPosicoesEmLocais)
                    CodigosLocais = Request.GetListParam<int>("Locais");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Repositorio.Embarcador.Logistica.Locais repLocais = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntregas = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Logistica.PosicaoAlvo repPosicaoAlvo = new Repositorio.Embarcador.Logistica.PosicaoAlvo(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

                List<Dominio.Entidades.Embarcador.Logistica.Locais> locais = new List<Dominio.Entidades.Embarcador.Logistica.Locais>();
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";

                // Alvos (clientes) distintos
                List<double> alvosComVeiculo = new List<double>();
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrotaTotalGrupoStatusViagem> totaisGrupos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrotaTotalGrupoStatusViagem>();

                // Agrupamento
                if (configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento && configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem)
                {
                    // Por grupo de tipo de operação
                    Repositorio.Embarcador.Pedidos.GrupoTipoOperacao repGrupoTipoOperacao = new Repositorio.Embarcador.Pedidos.GrupoTipoOperacao(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pedidos.GrupoTipoOperacao> gruposTipoOperacao = repGrupoTipoOperacao.BuscarAtivos();
                    totaisGrupos = (
                        from grupo in gruposTipoOperacao
                        select new Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrotaTotalGrupoStatusViagem()
                        {
                            Codigo = grupo.Codigo,
                            Descricao = grupo.Descricao,
                            Cor = grupo.Cor,
                            Total = 0
                        }).ToList();
                }
                else
                {

                    // Por grupo de status do monitoramento
                    Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem repMonitoramentoGrupoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoGrupoStatusViagem(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem> gruposStatusViagem = repMonitoramentoGrupoStatusViagem.BuscarAtivos();
                    totaisGrupos = (
                        from grupo in gruposStatusViagem
                        select new Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrotaTotalGrupoStatusViagem()
                        {
                            Codigo = grupo.Codigo,
                            Descricao = grupo.Descricao,
                            Cor = grupo.Cor,
                            Total = 0
                        }).ToList();
                }

                totaisGrupos.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrotaTotalGrupoStatusViagem
                {
                    Codigo = 0,
                    Descricao = NENHUM_DESCRICAO,
                    Cor = NENHUM_COR,
                    Total = 0
                });
                int totalGrupos = totaisGrupos.Count;

                // Veiculos
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = null;
                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrota> listaPosicaoFrota = ConsultarPosicaoFrota(parametrosConsulta, unitOfWork, configuracao);

                if (Request.GetBoolParam("BuscarPreCarga"))
                {
                    //buscar statusViagem e grupo de pre-viagem
                    Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);
                    Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem grupoStatusViagemPreCarga = repStatusViagem.BuscarAtivoPorTipoRegra(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusViagemTipoRegra.DeslocamentoParaPlanta)?.Grupo;

                    // alterar os retornos para buscar status viagem de pre-carga
                    if (grupoStatusViagemPreCarga != null)
                        listaPosicaoFrota = ObterDadosFrotaComPreCarga(listaPosicaoFrota, grupoStatusViagemPreCarga);

                }

                List<int> codigosVeiculo = (from veiculo in listaPosicaoFrota select veiculo.CodigoVeiculo).ToList();

                List<int> paradas = ObterCodigosVeiculosComAlertasParadas(unitOfWork, codigosVeiculo);

                if (filtrarPosicoesEmLocais)//carregar os locais de traking
                {
                    if (CodigosLocais.Count > 0)
                        locais = repLocais.BuscarPorCodigos(CodigosLocais);
                    else
                        locais = repLocais.BuscarTodos();
                }

                List<dynamic> listVeiculosResumido = new List<dynamic>();
                DateTime dataLimiteRastreador = DateTime.Now.AddMinutes(-configuracao.TempoSemPosicaoParaVeiculoPerderSinal);
                foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrota veiculo in listaPosicaoFrota)
                {

                    // Extração dos alvos
                    int total;
                    if (exibirFiliaisEBases && clientesComVeiculoEmAlvo && veiculo.CodigoPosicao > 0 && veiculo.EmAlvo && !string.IsNullOrWhiteSpace(veiculo.CodigosClientesAlvos))
                    {
                        string[] codigosClientes = veiculo.CodigosClientesAlvos.Split(',');
                        total = codigosClientes.Length;
                        for (int i = 0; i < total; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(codigosClientes[i]))
                            {
                                double codigo = double.Parse(codigosClientes[i]);
                                if (!alvosComVeiculo.Contains(codigo)) alvosComVeiculo.Add(codigo);
                            }
                        }
                    }

                    // Verifica se possui parada 
                    bool possuiParada = false;
                    total = paradas.Count;
                    for (int i = 0; i < total; i++)
                    {
                        if (paradas[i] == veiculo.CodigoVeiculo)
                        {
                            possuiParada = true;
                            break;
                        }
                    }

                    // Totalização dos grupos (grupos de status ou grupo de operações)
                    for (int j = 0; j < totalGrupos; j++)
                    {
                        if (configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento)
                        {
                            if (veiculo.GrupoTipoOperacaoCodigo == totaisGrupos[j].Codigo)
                            {
                                totaisGrupos[j].Total++;
                                break;
                            }
                        }
                        else
                        {
                            if (veiculo.GrupoStatusViagemCodigo == totaisGrupos[j].Codigo)
                            {
                                totaisGrupos[j].Total++;
                                break;
                            }
                        }
                    }

                    if (exibirFiliaisEBases && filtrarPosicoesEmLocais)
                    {
                        Dominio.Entidades.Embarcador.Logistica.Locais localAtual = Servicos.Embarcador.Monitoramento.Localizacao.ValidarArea.BuscarLocalEmArea(locais.ToArray(), veiculo.Latitude, veiculo.Longitude);

                        if (localAtual != null)
                        {
                            listVeiculosResumido.Add(new
                            {
                                veiculo.CodigoVeiculo,
                                veiculo.PlacaVeiculo,
                                veiculo.ListaTipoModeloVeicular,
                                veiculo.Latitude,
                                veiculo.Longitude,
                                veiculo.SituacaoVeiculo,
                                Rastreador = (veiculo.DataDaPosicao > dataLimiteRastreador),
                                Status = (configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento) ? veiculo.GrupoTipoOperacaoDescricao ?? NENHUM_DESCRICAO : veiculo.GrupoStatusViagemDescricao ?? NENHUM_DESCRICAO,
                                Cor = (configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento) ? veiculo.GrupoTipoOperacaoCor ?? NENHUM_COR : veiculo.GrupoStatusViagemCor ?? NENHUM_COR,
                                PossuiParada = possuiParada,
                                Critico = veiculo.MonitoramentoCritico,
                                veiculo.ListaSituacaoVeiculo
                            });
                        }
                    }
                    else
                    {
                        listVeiculosResumido.Add(new
                        {
                            veiculo.CodigoVeiculo,
                            veiculo.PlacaVeiculo,
                            veiculo.ListaTipoModeloVeicular,
                            veiculo.Latitude,
                            veiculo.Longitude,
                            veiculo.SituacaoVeiculo,
                            Rastreador = (veiculo.DataDaPosicao > dataLimiteRastreador),
                            Status = (configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento) ? veiculo.GrupoTipoOperacaoDescricao ?? NENHUM_DESCRICAO : veiculo.GrupoStatusViagemDescricao ?? NENHUM_DESCRICAO,
                            Cor = (configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento) ? veiculo.GrupoTipoOperacaoCor ?? NENHUM_COR : veiculo.GrupoStatusViagemCor ?? NENHUM_COR,
                            PossuiParada = possuiParada,
                            Critico = veiculo.MonitoramentoCritico,
                            veiculo.ListaSituacaoVeiculo
                        });
                    }

                }

                List<dynamic> listAlvosResumido = new List<dynamic>();
                List<dynamic> listLocaisResumido = new List<dynamic>();

                // Clientes de veiculos que estão no raio ...
                if (clientesComVeiculoEmAlvo)
                {
                    List<Dominio.Entidades.Cliente> clientes = BuscarClientes(unitOfWork, alvosComVeiculo);
                    foreach (Dominio.Entidades.Cliente cliente in clientes)
                    {
                        if (cliente.PossuiGeolocalizacao())
                            listAlvosResumido.Add(new
                            {
                                cliente.Descricao,
                                Latitude = Convert.ToDouble(cliente.Latitude, provider),
                                Longitude = Convert.ToDouble(cliente.Longitude, provider),
                                TipoArea = cliente.TipoArea?.ToString() ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Raio.ToString(),
                                Raio = (int)((cliente.TipoArea == null || cliente.TipoArea != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Poligono || cliente.RaioEmMetros == null) ? configuracao.RaioPadrao : cliente.RaioEmMetros),
                                cliente.Area,
                                cliente.Categoria?.Cor
                            });

                        if (cliente.Enderecos != null && cliente.Enderecos.Count > 0)
                        {
                            foreach (var outroEndereco in cliente.Enderecos)
                            {
                                if (!string.IsNullOrWhiteSpace(outroEndereco.Latitude) && !string.IsNullOrWhiteSpace(outroEndereco.Longitude))
                                {
                                    listAlvosResumido.Add(new
                                    {
                                        Descricao = $"{cliente.Descricao} - {outroEndereco.Descricao}",
                                        Latitude = Convert.ToDouble(outroEndereco.Latitude, provider),
                                        Longitude = Convert.ToDouble(outroEndereco.Longitude, provider),
                                        TipoArea = outroEndereco.TipoArea.ToString() ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Raio.ToString(),
                                        Raio = (int)((outroEndereco.TipoArea == null || outroEndereco.TipoArea != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Poligono || outroEndereco.RaioEmMetros == null) ? configuracao.RaioPadrao : outroEndereco.RaioEmMetros),
                                        outroEndereco.Area,
                                        cliente.Categoria?.Cor
                                    });
                                }
                            }
                        }
                    }
                }

                // ... e alvos estratégicos
                if (exibirFiliaisEBases && clientesAlvosEstrategicos)
                {
                    List<Dominio.Entidades.Cliente> clientesAlvoEstrategico = ConsultarAlvosEstrategicos(unitOfWork);
                    foreach (Dominio.Entidades.Cliente cliente in clientesAlvoEstrategico)
                    {
                        if (!alvosComVeiculo.Contains(cliente.Codigo))
                            listAlvosResumido.Add(new
                            {
                                cliente.Descricao,
                                Latitude = Convert.ToDouble(cliente.Latitude, provider),
                                Longitude = Convert.ToDouble(cliente.Longitude, provider),
                                TipoArea = cliente.TipoArea?.ToString() ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Raio.ToString(),
                                Raio = (int)((cliente.TipoArea == null || cliente.TipoArea != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Poligono || cliente.RaioEmMetros == null) ? configuracao.RaioPadrao : cliente.RaioEmMetros),
                                cliente.Area,
                                Cor = cliente.Categoria?.Cor ?? ""
                            });
                    }
                }

                // ... Areas traking
                if (exibirFiliaisEBases && clientesAlvosEstrategicos)
                {

                    foreach (Dominio.Entidades.Embarcador.Logistica.Locais local in locais)
                    {
                        listLocaisResumido.Add(new
                        {
                            local.Descricao,
                            TipoArea = local.TipoArea,
                            Raio = configuracao.RaioPadrao,
                            local.Area,
                            Cor = "#b3d9ff"
                        });
                    }
                }

                List<double> filiais = repositorioFilial.BuscarListaDoubleCNPJAtivas();
                List<dynamic> listaClientesComGeolocalizacao = new List<dynamic>();

                if (exibirFiliaisEBases && filiais.Count > 0)
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    IList<GeoLocalizacaoClienteSubArea> clientes = repCliente.BuscarClientesGeolocalizacaoAtivosPorFilais(filiais);

                    foreach (GeoLocalizacaoClienteSubArea cliente in clientes)
                    {
                        listaClientesComGeolocalizacao.Add(new
                        {
                            cliente.Latitude,
                            cliente.Longitude,
                            DescricaoFilial = cliente.NomeFantasia ?? string.Empty,
                            Raio = (int)(((cliente.TipoAreaCliente != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArea.Poligono && cliente.RaioCliente == 0)) ? configuracao.RaioPadrao : cliente.RaioCliente),
                            cliente.AreaCliente,
                            cliente.TipoAreaCliente,
                            SubAreas = cliente.SubAreas?.Count > 0 ? cliente.SubAreas?.Select(subarea => new
                            {
                                subarea.Area,
                            }) : null
                        });
                    }

                }

                return new JsonpResult(new
                {
                    Veiculos = listVeiculosResumido,
                    Alvos = listAlvosResumido,
                    Locais = listLocaisResumido,
                    Grupos = (from grupo in totaisGrupos select grupo),
                    GruposCarrossel = totaisGrupos,
                    Filiais = listaClientesComGeolocalizacao
                });

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private IList<PosicaoFrota> ObterDadosFrotaComPreCarga(IList<PosicaoFrota> listaPosicaoFrota, Dominio.Entidades.Embarcador.Logistica.MonitoramentoGrupoStatusViagem grupoStatusViagemPreCarga)
        {
            foreach (PosicaoFrota posicao in listaPosicaoFrota)
            {
                if (posicao.CargaFechada && posicao.CargaPreCarga && posicao.ListaStatusMonitoramento.Contains(Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Iniciado))
                {
                    posicao.GrupoStatusViagemCodigo = grupoStatusViagemPreCarga.Codigo;
                    posicao.GrupoStatusViagemCor = grupoStatusViagemPreCarga.Cor;
                    posicao.GrupoStatusViagemDescricao = grupoStatusViagemPreCarga.Descricao;
                }
            }

            return listaPosicaoFrota;
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPosicaoFrota filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPosicaoFrota
                {
                    CodigoVeiculo = Request.GetIntParam("CodigoVeiculo")
                };

                Repositorio.Embarcador.Logistica.PosicaoFrota repositorioPosicaoFrota = new Repositorio.Embarcador.Logistica.PosicaoFrota(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrota veiculo = repositorioPosicaoFrota.ConsultarRegistroUnico(filtrosPesquisa, TipoServicoMultisoftware, null);

                if (veiculo == null) return new JsonpResult(false, Localization.Resources.Logistica.PosicaoFrota.VeiculoNaoEncontrado);

                // Informações da carga
                string embarcador = string.Empty;
                List<dynamic> destinos = new List<dynamic>();
                if (veiculo.CodigoCarga > 0)
                {

                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Dominio.Entidades.Cliente clienteOrigem = repCargaPedido.BuscarClienteDaPrimeiraPorCarga(veiculo.CodigoCarga);
                    embarcador = (clienteOrigem != null) ? clienteOrigem.Descricao : string.Empty;

                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntregas = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregas = repCargaEntregas.BuscarPorCargaNaoRealizada(veiculo.CodigoCarga);
                    int total = entregas.Count;
                    for (int i = 0; i < Math.Min(total, 3); i++)
                    {
                        destinos.Add(new
                        {
                            entregas[i].Ordem,
                            Descricao = entregas[i].Cliente?.Descricao ?? string.Empty
                        });
                    }
                }

                // Paradas
                List<dynamic> paradas = ObterAlertasParadas(unitOfWork, new List<int> { veiculo.CodigoVeiculo });

                // Data limite para considerar veículo sem sinal
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                DateTime dataLimiteRastreador = DateTime.Now.AddMinutes(-configuracao.TempoSemPosicaoParaVeiculoPerderSinal);

                dynamic veiculoResumido = new
                {
                    veiculo.PlacaVeiculo,
                    veiculo.ModeloVeicular,
                    veiculo.ListaTipoModeloVeicular,
                    TipoModeloVeicularDescricao = string.Join(',', veiculo.ListaTipoModeloVeicular.Select(x => Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCargaHelper.ObterDescricao(x))),
                    veiculo.Transportador,
                    veiculo.Latitude,
                    veiculo.Longitude,
                    veiculo.Descricao,
                    PrevisaoEntregaAtualizada = veiculo.PrevisaoEntregaAtualizada.ToString("dd/MM/yyyy HH:mm"),
                    DataDaPosicao = veiculo.DataDaPosicaoFormatada,
                    veiculo.TempoDaUltimaPosicaoFormatada,
                    veiculo.EnderecoDaEntrega,
                    veiculo.PercentualViagem,
                    Rastreador = (veiculo.DataDaPosicao > dataLimiteRastreador),
                    RastreadorOnlineOffline = veiculo.DataDaPosicao == DateTime.MinValue ? 1 : (veiculo.DataDaPosicao != DateTime.MinValue && (DateTime.Now - veiculo.DataDaPosicao).TotalMinutes <= configuracao.TempoSemPosicaoParaVeiculoPerderSinal) ? 3 : 4,
                    Carga = veiculo.CodigoCargaEmbarcador,
                    CargaAtual = veiculo.CodigoCarga,
                    UltimaCarga = veiculo.CodigoUltimaCargaEmbarcador,
                    CargaVinculada = veiculo.CodigoCargaVinculada,
                    veiculo.DescricaoSituacaoCarga,
                    veiculo.TempoStatusDescricao,
                    Embarcador = embarcador,
                    Destinos = destinos,
                    Paradas = paradas,
                    Status = (configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento) ? veiculo.GrupoTipoOperacaoDescricao ?? NENHUM_DESCRICAO : veiculo.GrupoStatusViagemDescricao ?? NENHUM_DESCRICAO,
                    Cor = (configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento) ? veiculo.GrupoTipoOperacaoCor ?? NENHUM_COR : veiculo.GrupoStatusViagemCor ?? NENHUM_COR,
                    veiculo.Motorista,
                    Tecnologia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreadorHelper.ObterDescricao(veiculo.Rastreador)
                };

                return new JsonpResult(veiculoResumido);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos privados

        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            //grid.AdicionarCabecalho("Latitude", false);
            //grid.AdicionarCabecalho("Longitude", false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.Placa, "PlacaVeiculo", 8, Models.Grid.Align.center, false, false, false, true, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.Situacao, "SituacaoVeiculo", 8, Models.Grid.Align.left, false, false, false, true, true);
            else
                grid.AdicionarCabecalho("SituacaoVeiculo", false);

            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.Modelo, "ModeloVeicular", 10, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.TipoModelo, "TipoModeloVeicular", 10, Models.Grid.Align.left, false, false, false, true, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.ReboqueS, "Reboque", 9, Models.Grid.Align.left, false, false, false, true, true);
            else
                grid.AdicionarCabecalho("Reboque", false);

            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.DataPosicao, "DataDaPosicaoFormatada", 10, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.TempoUltimaPosicao, "TempoDaUltimaPosicaoFormatada", 10, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.Descricao, "Descricao", 20, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.Permanencia, "Permanencia", 10, Models.Grid.Align.center, false, false, false, true, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Transportador", false);
            else
                grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.Transportador, "Transportador", 10, Models.Grid.Align.left, false, false, false, true, true);

            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.Cliente, "ClientesAlvos", 15, Models.Grid.Align.left, false, false, false, true, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("CategoriasClientesAlvos", false);
            else
                grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.Categoria, "CategoriasClientesAlvos", 8, Models.Grid.Align.left, false, false, false, true, true);

            grid.AdicionarCabecalho("SM", "SM", 5, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.CargaAtual, "CodigoCargaEmbarcador", 10, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.UltimaCarga, "CodigoUltimaCargaEmbarcador", 10, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.DataCarga, "DataDaCargaFormatada", 10, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.DestinoFinal, "DestinoCarga", 10, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.PrevisaoEntregaAtualizada, "PrevisaoEntregaAtualizadaFormatada", 10, Models.Grid.Align.center, false, false, false, false, true);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho("Filial", false);
            else
                grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.Filial, "Filial", 10, Models.Grid.Align.left, false, false, false, true, true);

            if (!configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento || !configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramentoOcultarGrupoStatusViagem)
                grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.EtapaMonitoramento, "Status", 8, Models.Grid.Align.left, false, false, false, false, true);

            if (configuracao.UsarGrupoDeTipoDeOperacaoNoMonitoramento)
                grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.TipoOperacao, "GrupoTipoOperacaoDescricao", 8, Models.Grid.Align.left, false, false, false, false, true);

            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.Operacao, "TipoDeTransporte", 10, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.Latitude, "Latitude", 10, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.Longitude, "Longitude", 10, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.PosicaoFrota.Rastreador, "RastreadorOnlineOffline", 10, Models.Grid.Align.center, false, false, false, true, true);

            return grid;

        }

        private IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrota> ConsultarPosicaoFrota(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPosicaoFrota filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaPosicaoFrota()
            {
                CodigosVeiculoFiltro = Request.GetListParam<int>("Veiculo"),
                TipoModeloVeicular = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCarga>("TipoModeloVeicular"),
                CodigosTransportador = Request.GetListParam<double>("Transportador"),
                Cliente = Request.GetDoubleParam("Cliente"),
                CategoriaPessoa = Request.GetIntParam("CategoriaPessoa"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataFim = Request.GetNullableDateTimeParam("DataFim"),
                Situacoes = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPosicaoFrota>("Situacao"),
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoUltimaCargaEmbarcador = Request.GetStringParam("CodigoUltimaCargaEmbarcador"),
                CodigosGrupoStatusViagem = Request.GetListParam<int>("GrupoStatusViagem"),
                CodigosGrupoTipoOperacao = Request.GetListParam<int>("GrupoTipoOperacao"),
                EmAlvo = Request.GetBoolParam("EmAlvo"),
                VeiculosComMonitoramento = Request.GetBoolParam("VeiculosComMonitoramento"),
                VeiculosComContratoDeFrete = Request.GetBoolParam("VeiculosComContratoDeFrete"),
                BuscarPreCarga = Request.GetBoolParam("BuscarPreCarga"),
                SituacaoVeiculo = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo>("SituacaoVeiculo"),
                StatusViagemControleEntrega = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusViagemControleEntrega>("StatusViagemControleEntrega"),
                ClientesComVeiculoEmAlvo = Request.GetBoolParam("ClientesComVeiculoEmAlvo"),
                ClientesAlvosEstrategicos = Request.GetBoolParam("ClientesAlvosEstrategicos"),
                CpfCnpjRemetentes = Request.GetListParam<double>("Remetente"),
                CpfCnpjDestinatarios = Request.GetListParam<double>("Destinatario"),
                Motoristas = Request.GetListParam<int>("Motorista"),
                CodigosCentroResultado = Request.GetListParam<int>("CentroResultado"),
                CodigosFuncionarioResponsavel = Request.GetListParam<int>("FuncionarioResponsavel"),
                GrupoPessoas = Request.GetListParam<int>("GrupoPessoas"),
                MonitoramentoCritico = Request.GetBoolParam("Critico"),
                CodigosContratoFrete = Request.GetListParam<int>("ContratoFrete"),
                CodigosTipoContratoFrete = Request.GetListParam<int>("TipoContratoFrete"),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                TecnologiaRastreador = Request.GetListParam<int>("TecnologiaRastreador"),
                VeiculosDentroDoRaioFilial = Request.GetBoolParam("VeiculosDentroDoRaioFilial"),
                RaioFilial = Request.GetIntParam("RaioFilial"),
                ExibirFiliaisEBases = Request.GetBoolParam("ExibirFiliaisEBases"),
                RastreadorOnlineOffline = Request.GetNullableBoolParam("RastreadorOnlineOffline")
            };

            if (this.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.Transportador = Empresa.Codigo;

            int tecnlogiaRastreador = Request.GetIntParam("TecnologiaRastreador");
            if (this.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && Usuario.ClienteFornecedor != null)
            {
                //para mostrar no portal cliente.
                filtrosPesquisa.CpfCnpjDestinatarios = new List<double>();
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                List<Dominio.Entidades.Cliente> clientesCnpjRaiz = repositorioCliente.BuscarPorRaizCNPJ(Usuario.ClienteFornecedor.CPF_CNPJ_SemFormato.Substring(0, 8));

                if (clientesCnpjRaiz != null && clientesCnpjRaiz.Count > 1)
                {
                    foreach (Dominio.Entidades.Cliente cliente in clientesCnpjRaiz)
                    {
                        filtrosPesquisa.CpfCnpjDestinatarios.Add(cliente.CPF_CNPJ);
                    }
                }
                else
                    filtrosPesquisa.CpfCnpjDestinatarios.Add(Usuario.ClienteFornecedor.CPF_CNPJ);

            }

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

            filtrosPesquisa.CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            filtrosPesquisa.CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosFilialVenda = ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoOperacao = codigoTipoOperacao == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoOperacao };
            filtrosPesquisa.TempoSemPosicaoParaVeiculoPerderSinal = configuracao.TempoSemPosicaoParaVeiculoPerderSinal;

            Repositorio.Embarcador.Logistica.PosicaoFrota repositorioPosicaoFrota = new Repositorio.Embarcador.Logistica.PosicaoFrota(unitOfWork);
            return repositorioPosicaoFrota.Consultar(filtrosPesquisa, TipoServicoMultisoftware, parametrosConsulta);
        }

        private void AdicionarLinhasNoGrid(IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PosicaoFrota> listaPosicaoFrota, Models.Grid.Grid grid)
        {
            dynamic rows = (
                from posicao in listaPosicaoFrota
                select new
                {
                    posicao.CodigoVeiculo,
                    posicao.PlacaVeiculo,
                    posicao.CodigoPosicao,
                    posicao.Latitude,
                    posicao.Longitude,
                    posicao.DataDaPosicao,
                    Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPosicaoFrotaHelper.ObterDescricao(posicao.Situacao),
                    posicao.Status,
                    posicao.GrupoStatusViagemCodigo,
                    posicao.GrupoStatusViagemDescricao,
                    posicao.GrupoStatusViagemCor,
                    posicao.GrupoTipoOperacaoCodigo,
                    posicao.GrupoTipoOperacaoDescricao,
                    posicao.GrupoTipoOperacaoCor,
                    posicao.Descricao,
                    posicao.EmAlvo,
                    posicao.ClientesAlvos,
                    posicao.CategoriasClientesAlvos,
                    posicao.DataDaPosicaoFormatada,
                    posicao.TempoDaUltimaPosicaoFormatada,
                    posicao.CodigoCarga,
                    posicao.CodigoCargaEmbarcador,
                    posicao.CodigoUltimaCargaEmbarcador,
                    posicao.Filial,
                    posicao.DataDaCarga,
                    posicao.DataDaCargaFormatada,
                    posicao.SM,
                    posicao.ModeloVeicular,
                    TipoModeloVeicular = string.Join(',', posicao.ListaTipoModeloVeicular.Select(x => Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModeloVeicularCargaHelper.ObterDescricao(x))),
                    Permanencia = posicao.DataDaCarga != DateTime.MinValue ? Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(posicao.DataDaPosicao - posicao.DataDaCarga) : "-",
                    posicao.TipoDeTransporte,
                    posicao.DestinoCarga,
                    posicao.PrevisaoEntregaAtualizadaFormatada,
                    posicao.Transportador,
                    posicao.Reboque,
                    posicao.SituacaoVeiculo,
                    posicao.RastreadorOnlineOffline
                }
            ).ToList();
            grid.AdicionaRows(rows);
            grid.setarQuantidadeTotal(rows.Count);
        }

        private List<Dominio.Entidades.Cliente> ConsultarAlvosEstrategicos(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repoCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.ObjetosDeValor.FiltroPesquisaCliente filtrosPesquisa = new Dominio.ObjetosDeValor.FiltroPesquisaCliente();
            filtrosPesquisa.AlvoEstrategico = true;
            filtrosPesquisa.ComGeolocalizacao = true;
            return repoCliente.Consultar(filtrosPesquisa, null, null, 0, 0);
        }

        private List<Dominio.Entidades.Cliente> BuscarClientes(Repositorio.UnitOfWork unitOfWork, List<double> cpfCpnjs)
        {
            Repositorio.Cliente repoCliente = new Repositorio.Cliente(unitOfWork);
            return repoCliente.BuscarPorCPFCNPJs(cpfCpnjs);
        }

        private List<dynamic> ObterAlertasParadas(Repositorio.UnitOfWork unitOfWork, List<int> codigosVeiculos)
        {
            List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> paradas = new List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor>();
            if (codigosVeiculos.Count > 0)
            {
                Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta>();
                tiposAlerta.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaEmAreaDeRisco);
                tiposAlerta.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaExcessiva);
                tiposAlerta.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaNaoProgramada);
                paradas = repAlertaMonitor.BuscarAlertasEmAbertoPorVeiculoETipoDeAlerta(codigosVeiculos, tiposAlerta);
            }

            return new List<dynamic>(
                    from parada in paradas
                    select new
                    {
                        parada.Veiculo.Codigo,
                        parada.Veiculo.Placa,
                        Tipo = parada.Descricao,
                        Descricao = parada.AlertaDescricao,
                        Data = parada.Data.ToString("dd/MM/yyyy HH:mm"),
                        parada.Latitude,
                        parada.Longitude,
                    }
             );

        }

        private List<int> ObterCodigosVeiculosComAlertasParadas(Repositorio.UnitOfWork unitOfWork, List<int> codigosVeiculos)
        {
            List<int> codigosVeiculosComParada = new List<int>();
            if (codigosVeiculos.Count > 0)
            {
                Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> tiposAlerta = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta>();
                tiposAlerta.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaEmAreaDeRisco);
                tiposAlerta.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaExcessiva);
                tiposAlerta.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.ParadaNaoProgramada);
                codigosVeiculosComParada = repAlertaMonitor.BuscarCodigosVeiculosComAlertaEmAbertoPorVeiculoETipoDeAlerta(codigosVeiculos, tiposAlerta);
            }
            return codigosVeiculosComParada;
        }

        #endregion
    }
}
