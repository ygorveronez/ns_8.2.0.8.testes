using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Servicos.Embarcador.Integracao.Target
{
    public class ValePedagio
    {
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #region Construtores

        public ValePedagio(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ValePedagio(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Públicos

        public Servicos.ServicoTarget.ValePedagio.AutenticacaoRequest Autenticar(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool registrarErro = true)
        {
            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget integracaoTarget = servicoValePedagio.ObterIntegracaoTarget(cargaValePedagio.Carga, tipoServicoMultisoftware);

                if (integracaoTarget == null)
                {
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.ProblemaIntegracao = "Target não está configurado";
                    cargaValePedagio.NumeroTentativas++;
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    repCargaValePedagio.Atualizar(cargaValePedagio);
                    return null;
                }
                else
                {
                    Servicos.ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao = new ServicoTarget.ValePedagio.AutenticacaoRequest()
                    {
                        Usuario = integracaoTarget.Usuario,
                        Senha = integracaoTarget.Senha,
                        Token = integracaoTarget.Token // Target retornou que parametro não esta sendo utilizado, o Token é configurado/enviado na senha
                    };

                    return autenticacao;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                cargaValePedagio.DataIntegracao = DateTime.Now;
                cargaValePedagio.ProblemaIntegracao = "Não foi possível criar aturenticação Target.";
                cargaValePedagio.NumeroTentativas++;
                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                repCargaValePedagio.Atualizar(cargaValePedagio);

                return null;
            }
        }

        public Servicos.ServicoTarget.ValePedagio.AutenticacaoRequest Autenticar(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio)
        {
            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repConsultaCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget integracaoTarget = servicoValePedagio.ObterIntegracaoTarget(cargaConsultaValePedagio.Carga, _tipoServicoMultisoftware);

                if (integracaoTarget == null)
                {
                    cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
                    cargaConsultaValePedagio.ProblemaIntegracao = "Target não está configurado";
                    cargaConsultaValePedagio.NumeroTentativas++;
                    cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    repConsultaCargaValePedagio.Atualizar(cargaConsultaValePedagio);

                    return null;
                }
                else
                {
                    Servicos.ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao = new ServicoTarget.ValePedagio.AutenticacaoRequest()
                    {
                        Usuario = integracaoTarget.Usuario,
                        Senha = integracaoTarget.Senha,
                        Token = integracaoTarget.Token // Target retornou que parametro não esta sendo utilizado, o Token é configurado/enviado na senha
                    };

                    return autenticacao;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
                cargaConsultaValePedagio.ProblemaIntegracao = "Não foi possível criar autenticação Target.";
                cargaConsultaValePedagio.NumeroTentativas++;
                cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                repConsultaCargaValePedagio.Atualizar(cargaConsultaValePedagio);

                return null;
            }
        }

        public void GerarCompraValePedagio(Servicos.ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            try
            {
                Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);
                Repositorio.RotaFretePontosPassagem repositorioRotaFretePontosPassagem = new Repositorio.RotaFretePontosPassagem(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget integracaoTarget = servicoValePedagio.ObterIntegracaoTarget(cargaValePedagio.Carga, tipoServicoMultisoftware);

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();
                Servicos.Embarcador.Carga.ValePedagio.ValePedagio servicoCargaValePedagio = new Servicos.Embarcador.Carga.ValePedagio.ValePedagio(unitOfWork);

                string mensagemRetorno = string.Empty;

                cargaValePedagio.TipoCompra = Dominio.Enumeradores.TipoCompraValePedagio.Tag;

                bool eixosSuspensos = false;
                if (carga.TipoOperacao != null)
                {
                    if (carga.TipoOperacao.TipoCarregamento.HasValue && carga.TipoOperacao.TipoCarregamento.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo.Vazio)
                        eixosSuspensos = true;
                }
                int numeroEixos = 0;
                if (carga.Veiculo.ModeloVeicularCarga != null)
                {
                    numeroEixos = carga.Veiculo.ModeloVeicularCarga.NumeroEixos ?? 0;
                    if (eixosSuspensos)
                        numeroEixos -= carga.Veiculo.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
                }
                if (carga.VeiculosVinculados != null)
                {
                    foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados.ToList())
                    {
                        if (reboque.ModeloVeicularCarga != null && carga.Veiculo.ModeloVeicularCarga != null && reboque.ModeloVeicularCarga != carga.Veiculo.ModeloVeicularCarga)
                        {
                            numeroEixos += reboque.ModeloVeicularCarga.NumeroEixos ?? 0;

                            if (eixosSuspensos)
                                numeroEixos -= reboque.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
                        }
                    }
                }

                int categoriaVeiculo = this.ObterCategoriaPorEixos(numeroEixos, carga.Veiculo.ModeloVeicularCarga?.PadraoEixos);
                int ibgeInicio = 0;
                int ibgeFim = 0;
                int ibgeUltimaEntrega = 0;

                //Busca da Carga Rota Frete
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);
                if (cargaRotaFrete != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem inicio = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFreteETipoPassagem(cargaRotaFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta).FirstOrDefault();
                    ibgeInicio = (inicio?.ClienteOutroEndereco?.Localidade?.CodigoIBGE ?? 0) > 0 ? inicio?.ClienteOutroEndereco?.Localidade?.CodigoIBGE ?? 0 : inicio?.Cliente?.Localidade?.CodigoIBGE ?? 0;
                    Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem fim = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFreteETipoPassagem(cargaRotaFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Retorno).LastOrDefault();
                    ibgeFim = (fim?.ClienteOutroEndereco?.Localidade?.CodigoIBGE ?? 0) > 0 ? fim?.ClienteOutroEndereco?.Localidade?.CodigoIBGE ?? 0 : fim?.Cliente?.Localidade?.CodigoIBGE ?? 0;
                    Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem ultimaEntrega = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFreteETipoPassagem(cargaRotaFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega).LastOrDefault();
                    ibgeUltimaEntrega = (ultimaEntrega?.ClienteOutroEndereco?.Localidade?.CodigoIBGE ?? 0) > 0 ? ultimaEntrega?.ClienteOutroEndereco?.Localidade?.CodigoIBGE ?? 0 : ultimaEntrega?.Cliente?.Localidade?.CodigoIBGE ?? 0;
                    if (ibgeFim == 0)
                        ibgeFim = ibgeUltimaEntrega;
                }

                if (ibgeInicio == 0 || ibgeFim == 0)
                {
                    Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(carga.Codigo);

                    if (ibgeInicio == 0)
                    {
                        if (cargaPedido.Expedidor != null)
                            ibgeInicio = cargaPedido.Expedidor.Localidade.CodigoIBGE;
                        else
                            ibgeInicio = carga.DadosSumarizados.ClientesRemetentes.FirstOrDefault().Localidade.CodigoIBGE;
                    }

                    if (ibgeFim == 0)
                    {
                        if (cargaPedido.Recebedor != null)
                            ibgeFim = cargaPedido.Recebedor.Localidade.CodigoIBGE;
                        else
                            ibgeFim = carga.DadosSumarizados.ClientesDestinatarios.FirstOrDefault().Localidade.CodigoIBGE;
                    }
                }

                ServicoTarget.ValePedagio.ListarRotaClienteRequest consultaRotaIBGE = new ServicoTarget.ValePedagio.ListarRotaClienteRequest()
                {
                    CodigoIBGEOrigem = ibgeInicio,
                    CodigoIBGEDestino = ibgeFim
                };

                mensagemRetorno = string.Empty;
                int idRota = 0;

                if (!string.IsNullOrWhiteSpace(cargaValePedagio.RotaFrete?.CodigoIntegracaoValePedagio))
                    idRota = cargaValePedagio.RotaFrete.CodigoIntegracaoValePedagio.ToInt();
                else if (!string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagio))
                    idRota = carga.Rota.CodigoIntegracaoValePedagio.ToInt();

                if (idRota == 0)
                {
                    if (integracaoTarget.CadastrarRotaPorIBGE)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem = null;
                        if (cargaRotaFrete != null)
                            pontosPassagem = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

                        int[] codigosIBGEMunicipioParadas = null;
                        if (pontosPassagem != null && pontosPassagem.Count() > 2)
                        {
                            var pontosPassagensDistintos = pontosPassagem.Select(o => (o?.ClienteOutroEndereco?.Localidade ?? o.Cliente.Localidade).CodigoIBGE).Distinct().ToList();

                            List<int> codigosIBGES = new List<int>();
                            foreach (var ponto in pontosPassagensDistintos)
                            {
                                if (ponto != ibgeInicio && ponto != ibgeFim)
                                    codigosIBGES.Add(ponto);
                            }

                            codigosIBGEMunicipioParadas = new int[codigosIBGES.Count()];
                            int i = 0;
                            foreach (var ibgePassagem in codigosIBGES)
                            {
                                codigosIBGEMunicipioParadas[i] = ibgePassagem;
                                i += 1;
                            }

                        }
                        else if (ibgeUltimaEntrega > 0 && ibgeInicio == ibgeFim)
                        {
                            codigosIBGEMunicipioParadas = new int[1];
                            codigosIBGEMunicipioParadas[0] = ibgeUltimaEntrega;
                        }

                        Servicos.ServicoTarget.ValePedagio.RoteiroRequest roteiroRequest = new ServicoTarget.ValePedagio.RoteiroRequest()
                        {
                            CategoriaVeiculo = categoriaVeiculo,
                            CodigoIBGEMunicipioOrigem = ibgeInicio,
                            CodigoIBGEMunicipioDestino = ibgeFim,
                            CodigosIBGEMunicipioParadas = codigosIBGEMunicipioParadas,
                            NomeRoteiro = ibgeInicio.ToString() + " - " + ibgeFim.ToString()
                        };
                        Servicos.Models.Integracao.InspectorBehavior inspectorBuscarRota = new Servicos.Models.Integracao.InspectorBehavior();
                        idRota = CadastrarRoteiroIBGE(autenticacao, roteiroRequest, unitOfWork, ref mensagemRetorno, ref inspectorBuscarRota);
                        SalvarXMLIntegracao(ref cargaValePedagio, inspectorBuscarRota, "Cadastrar Rota IBGE " + mensagemRetorno, unitOfWork);
                        System.Threading.Thread.Sleep(5000);
                    }
                    else if (integracaoTarget.CadastrarRotaPorCoordenadas)
                    {
                        if (integracaoTarget.PreencherLatLongDaRotaIntegracao)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem = null;
                            if (cargaRotaFrete != null)
                                pontosPassagem = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

                            ServicoTarget.ValePedagio.RotaDetalhadaParada[] paradas = new Servicos.ServicoTarget.ValePedagio.RotaDetalhadaParada[30];
                            if (pontosPassagem.Count > 0)
                            {
                                List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> coordenadasPolilinha = Servicos.Embarcador.Logistica.Polilinha.Decodificar(cargaRotaFrete.PolilinhaRota);

                                Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem pontoPassagemInicio = pontosPassagem.FirstOrDefault();
                                Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem pontoPassagemFinal = pontosPassagem.LastOrDefault();

                                double.TryParse(pontoPassagemInicio.Latitude.ToString(), out double latitudeInicio);
                                double.TryParse(pontoPassagemInicio.Longitude.ToString(), out double longitudeInicio);
                                double.TryParse(pontoPassagemInicio.Cliente?.Latitude, out double latitudeClienteInicio);
                                double.TryParse(pontoPassagemInicio.Cliente?.Longitude, out double longitudeClienteInicio);

                                double.TryParse(pontoPassagemFinal.Latitude.ToString(), out double latitudeFinal);
                                double.TryParse(pontoPassagemFinal.Longitude.ToString(), out double longitudeFinal);
                                double.TryParse(pontoPassagemFinal.Cliente?.Latitude, out double latitudeClienteFinal);
                                double.TryParse(pontoPassagemFinal.Cliente?.Longitude, out double longitudeClienteFinal);

                                paradas[0] = new ServicoTarget.ValePedagio.RotaDetalhadaParada()
                                {
                                    LAT = latitudeInicio != 0 ? latitudeInicio : latitudeClienteInicio,
                                    LNG = longitudeInicio != 0 ? longitudeInicio : longitudeClienteInicio,
                                };

                                paradas[29] = new ServicoTarget.ValePedagio.RotaDetalhadaParada()
                                {
                                    LAT = latitudeFinal != 0 ? latitudeFinal : latitudeClienteFinal,
                                    LNG = longitudeFinal != 0 ? longitudeFinal : longitudeClienteFinal,
                                };

                                int intervaloLinhas = coordenadasPolilinha.Count() / 29;

                                for (int a = 1; a < 29; a++)
                                {
                                    paradas[a] = new ServicoTarget.ValePedagio.RotaDetalhadaParada()
                                    {
                                        LAT = coordenadasPolilinha[intervaloLinhas * a].Latitude,
                                        LNG = coordenadasPolilinha[intervaloLinhas * a].Longitude,
                                    };
                                }
                            }

                            Servicos.ServicoTarget.ValePedagio.RotaDetalhadaRequest roteiroRequest = new Servicos.ServicoTarget.ValePedagio.RotaDetalhadaRequest()
                            {
                                CategoriaVeiculo = categoriaVeiculo,
                                Paradas = paradas,
                                NomeRota = ibgeInicio.ToString() + " - " + ibgeFim.ToString(),
                                RotaTemporaria = false
                            };

                            ServicoTarget.ValePedagio.FreteTMSServiceExtendedClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceExtendedClient, ServicoTarget.ValePedagio.FreteTMSServiceExtended>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMSExtended, out inspector);

                            Servicos.ServicoTarget.ValePedagio.CadastrarRoteiroDetalhadoRequest cadastrarRoteiroDetalhadoRequest = new ServicoTarget.ValePedagio.CadastrarRoteiroDetalhadoRequest
                            {
                                auth = autenticacao,
                                rotaDetalhada = roteiroRequest
                            };

                            svcTarget.CadastrarRoteiroDetalhado(cadastrarRoteiroDetalhadoRequest);

                            Servicos.Models.Integracao.InspectorBehavior inspectorBuscarRota = new Servicos.Models.Integracao.InspectorBehavior();
                            idRota = CadastrarRoteiroCoordenadas(autenticacao, roteiroRequest, ref mensagemRetorno, ref inspectorBuscarRota, unitOfWork);
                            SalvarXMLIntegracao(ref cargaValePedagio, inspectorBuscarRota, "Cadastrar Rota Detalhada " + mensagemRetorno, unitOfWork);
                            System.Threading.Thread.Sleep(2000);
                        }
                        else if (integracaoTarget.PreencherPontosPassagemModificadoCliente)
                        {
                            List<Dominio.Entidades.RotaFretePontosPassagem> pontosPassagem = null;
                            if (carga.Rota != null)
                                pontosPassagem = repositorioRotaFretePontosPassagem.BuscarPorRotaFrete(carga.Rota.Codigo);

                            ServicoTarget.ValePedagio.RotaDetalhadaParada[] paradas = new Servicos.ServicoTarget.ValePedagio.RotaDetalhadaParada[pontosPassagem != null ? pontosPassagem.Count() : 0];
                            if (pontosPassagem != null)
                            {
                                int i = 0;
                                foreach (Dominio.Entidades.RotaFretePontosPassagem ponto in pontosPassagem)
                                {
                                    double.TryParse(ponto.Latitude.ToString(), out double latitude);
                                    double.TryParse(ponto.Longitude.ToString(), out double longitude);

                                    double.TryParse(ponto.Cliente?.Latitude, out double latitudeCliente);
                                    double.TryParse(ponto.Cliente?.Longitude, out double longitudeCliente);

                                    paradas[i] = new ServicoTarget.ValePedagio.RotaDetalhadaParada()
                                    {
                                        LAT = latitude != 0 ? latitude : latitudeCliente,
                                        LNG = longitude != 0 ? longitude : longitudeCliente,
                                    };

                                    i += 1;
                                }
                            }

                            Servicos.ServicoTarget.ValePedagio.RotaDetalhadaRequest roteiroRequest = new Servicos.ServicoTarget.ValePedagio.RotaDetalhadaRequest()
                            {
                                CategoriaVeiculo = categoriaVeiculo,
                                Paradas = paradas,
                                NomeRota = ibgeInicio.ToString() + " - " + ibgeFim.ToString(),
                                RotaTemporaria = false
                            };

                            ServicoTarget.ValePedagio.FreteTMSServiceExtendedClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceExtendedClient, ServicoTarget.ValePedagio.FreteTMSServiceExtended>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMSExtended, out inspector);

                            Servicos.ServicoTarget.ValePedagio.CadastrarRoteiroDetalhadoRequest cadastrarRoteiroDetalhadoRequest = new ServicoTarget.ValePedagio.CadastrarRoteiroDetalhadoRequest
                            {
                                auth = autenticacao,
                                rotaDetalhada = roteiroRequest
                            };

                            svcTarget.CadastrarRoteiroDetalhado(cadastrarRoteiroDetalhadoRequest);

                            Servicos.Models.Integracao.InspectorBehavior inspectorBuscarRota = new Servicos.Models.Integracao.InspectorBehavior();
                            idRota = CadastrarRoteiroCoordenadas(autenticacao, roteiroRequest, ref mensagemRetorno, ref inspectorBuscarRota, unitOfWork);
                            SalvarXMLIntegracao(ref cargaValePedagio, inspectorBuscarRota, "Cadastrar Rota Detalhada " + mensagemRetorno, unitOfWork);
                            System.Threading.Thread.Sleep(2000);
                        }
                        else
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem = null;
                            if (cargaRotaFrete != null)
                                pontosPassagem = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

                            ServicoTarget.ValePedagio.RotaDetalhadaParada[] paradas = new Servicos.ServicoTarget.ValePedagio.RotaDetalhadaParada[pontosPassagem != null ? pontosPassagem.Count() : 0];
                            if (pontosPassagem != null)
                            {
                                int i = 0;
                                foreach (Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem ponto in pontosPassagem)
                                {
                                    //if (ponto.Ordem != pontosPassagem.FirstOrDefault().Ordem && ponto.Ordem != pontosPassagem.LastOrDefault().Ordem)
                                    //{
                                    double.TryParse(ponto.Latitude.ToString(), out double latitude);
                                    double.TryParse(ponto.Longitude.ToString(), out double longitude);

                                    double.TryParse(ponto.Cliente?.Latitude, out double latitudeCliente);
                                    double.TryParse(ponto.Cliente?.Longitude, out double longitudeCliente);

                                    paradas[i] = new ServicoTarget.ValePedagio.RotaDetalhadaParada()
                                    {
                                        LAT = latitude != 0 ? latitude : latitudeCliente,
                                        LNG = longitude != 0 ? longitude : longitudeCliente,
                                    };

                                    i += 1;
                                    //}
                                }
                            }

                            Servicos.ServicoTarget.ValePedagio.RotaDetalhadaRequest roteiroRequest = new Servicos.ServicoTarget.ValePedagio.RotaDetalhadaRequest()
                            {
                                CategoriaVeiculo = categoriaVeiculo,
                                Paradas = paradas,
                                NomeRota = ibgeInicio.ToString() + " - " + ibgeFim.ToString(),
                                RotaTemporaria = false
                            };

                            ServicoTarget.ValePedagio.FreteTMSServiceExtendedClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceExtendedClient, ServicoTarget.ValePedagio.FreteTMSServiceExtended>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMSExtended, out inspector);

                            Servicos.ServicoTarget.ValePedagio.CadastrarRoteiroDetalhadoRequest cadastrarRoteiroDetalhadoRequest = new ServicoTarget.ValePedagio.CadastrarRoteiroDetalhadoRequest
                            {
                                auth = autenticacao,
                                rotaDetalhada = roteiroRequest
                            };

                            svcTarget.CadastrarRoteiroDetalhado(cadastrarRoteiroDetalhadoRequest);

                            Servicos.Models.Integracao.InspectorBehavior inspectorBuscarRota = new Servicos.Models.Integracao.InspectorBehavior();
                            idRota = CadastrarRoteiroCoordenadas(autenticacao, roteiroRequest, ref mensagemRetorno, ref inspectorBuscarRota, unitOfWork);
                            SalvarXMLIntegracao(ref cargaValePedagio, inspectorBuscarRota, "Cadastrar Rota Detalhada " + mensagemRetorno, unitOfWork);
                            System.Threading.Thread.Sleep(2000);
                        }
                    }
                    else
                    {
                        Servicos.Models.Integracao.InspectorBehavior inspectorBuscarRota = new Servicos.Models.Integracao.InspectorBehavior();
                        ServicoTarget.ValePedagio.RotaResponse rota = BuscarRotaIBGE(autenticacao, consultaRotaIBGE, carga.Rota?.CodigoIntegracaoValePedagio, unitOfWork, ref mensagemRetorno, ref inspectorBuscarRota);
                        SalvarXMLIntegracao(ref cargaValePedagio, inspectorBuscarRota, "Buscar rota " + mensagemRetorno, unitOfWork);
                        if (rota != null)
                            idRota = rota.IdRotaCliente;
                    }
                }

                if (idRota <= 0)
                {
                    cargaValePedagio.ProblemaIntegracao = idRota > 0 ? "Não existe rota cadastrada na Target com ID " + idRota.ToString() : "Não existe rota cadastrada na Target para IBGE Inicio " + ibgeInicio.ToString() + " e IBGE Fim " + ibgeFim.ToString();
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.NumeroTentativas++;
                    repCargaValePedagio.Atualizar(cargaValePedagio);

                    return;
                }

                ServicoTarget.ValePedagio.ObtencaoCustoRotaRequest consultaCustoRota = new ServicoTarget.ValePedagio.ObtencaoCustoRotaRequest()
                {
                    CategoriaVeiculo = categoriaVeiculo,
                    IdRotaModelo = idRota,
                    ModoPagamentoRota = cargaValePedagio.TipoCompra == Dominio.Enumeradores.TipoCompraValePedagio.Tag ? 2 : 1
                };

                mensagemRetorno = string.Empty;
                Servicos.Models.Integracao.InspectorBehavior inspectorCustoRota = new Servicos.Models.Integracao.InspectorBehavior();

                ServicoTarget.ValePedagio.ObtencaoCustoRotaResponse custoRota = BuscarCustoRota(autenticacao, consultaCustoRota, unitOfWork, ref mensagemRetorno, ref inspectorCustoRota);
                SalvarXMLIntegracao(ref cargaValePedagio, inspectorCustoRota, "Buscar custo rota " + mensagemRetorno, unitOfWork);

                if (custoRota != null && !string.IsNullOrWhiteSpace(mensagemRetorno))
                {
                    cargaValePedagio.ProblemaIntegracao = mensagemRetorno;
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.NumeroTentativas++;
                    repCargaValePedagio.Atualizar(cargaValePedagio);

                    return;
                }

                if (custoRota == null || custoRota.ValorPedagioTotal <= 0m)
                {
                    cargaValePedagio.ProblemaIntegracao = custoRota == null ? "Rota sem custo de Vale pedágio na Target" : "Rota sem valor de Vale pedágio na Target";
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.RotaSemCusto;
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.NumeroTentativas++;
                    repCargaValePedagio.Atualizar(cargaValePedagio);

                    return;
                }

                ServicoTarget.ValePedagio.CompraValePedagioRequest compraValePedagio = new ServicoTarget.ValePedagio.CompraValePedagioRequest();

                if (cargaValePedagio.TipoCompra == Dominio.Enumeradores.TipoCompraValePedagio.Tag && (carga.Veiculo?.ModoCompraValePedagioTarget.HasValue ?? false))
                    compraValePedagio.IdModoCompraValePedagio = ObterIdModoCompraValePedagio(carga.Veiculo?.ModoCompraValePedagioTarget);
                else
                    compraValePedagio.IdModoCompraValePedagio = (int)cargaValePedagio.TipoCompra;

                compraValePedagio.IdRotaModelo = idRota;
                compraValePedagio.CodigoCategoriaVeiculo = categoriaVeiculo;
                compraValePedagio.MunicipioOrigemCodigoIBGE = ibgeInicio;
                compraValePedagio.MunicipioDestinoCodigoIBGE = ibgeFim;
                compraValePedagio.Placa = carga.Veiculo?.Placa ?? string.Empty;

                compraValePedagio.NumeroCartao = string.Empty;
                compraValePedagio.MotoristaNome = carga.Motoristas != null && carga.Motoristas.Count > 0 ? carga.Motoristas.FirstOrDefault().Nome : string.Empty;
                compraValePedagio.MotoristaCPF = carga.Motoristas != null && carga.Motoristas.Count > 0 ? carga.Motoristas.FirstOrDefault().CPF : string.Empty;
                compraValePedagio.MotoristaRNTRC = carga.Veiculo != null && carga.Veiculo.RNTRC > 0 ? carga.Veiculo.RNTRC.ToString() : carga.Veiculo != null ? carga.Veiculo.Empresa.RegistroANTT : string.Empty;

                compraValePedagio.CpfCnpjTransportador = ObterCpfCnpjTransportador(carga);

                compraValePedagio.IdIntegrador = string.Empty;
                compraValePedagio.NumeroDocumentoEmbarque = string.Empty;
                compraValePedagio.ItemFinanceiro = carga.CodigoCargaEmbarcador + "_" + carga.Codigo.ToString();

                if (cargaValePedagio.TipoCompra == Dominio.Enumeradores.TipoCompraValePedagio.Tag)
                {
                    compraValePedagio.InicioVigencia = DateTime.Today;
                    compraValePedagio.FimVigencia = DateTime.Today.AddDays(integracaoTarget.DiasPrazo > 0 ? integracaoTarget.DiasPrazo : 15);
                }

                compraValePedagio.ValorPrevioCalculado = custoRota.ValorPedagioTotal;

                if (integracaoTarget.CodigoCentroCusto > 0)
                    compraValePedagio.CodigoCentroDeCusto = integracaoTarget.CodigoCentroCusto;

                mensagemRetorno = string.Empty;
                Servicos.Models.Integracao.InspectorBehavior inspectorCompraRota = new Servicos.Models.Integracao.InspectorBehavior();
                ServicoTarget.ValePedagio.CompraValePedagioResponse retornoCompraValePedagio = ComprarValePedagio(autenticacao, compraValePedagio, unitOfWork, ref mensagemRetorno, ref inspectorCompraRota);
                SalvarXMLIntegracao(ref cargaValePedagio, inspectorCompraRota, "Comprar Vale Pedagio " + mensagemRetorno, unitOfWork);

                if (retornoCompraValePedagio == null)
                {
                    cargaValePedagio.ProblemaIntegracao = !string.IsNullOrWhiteSpace(mensagemRetorno) ? mensagemRetorno : "Falha ao efetuar compra";
                    cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaValePedagio.DataIntegracao = DateTime.Now;
                    cargaValePedagio.NumeroTentativas++;
                    repCargaValePedagio.Atualizar(cargaValePedagio);

                    return;
                }

                cargaValePedagio.QuantidadeEixos = numeroEixos;
                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno;
                cargaValePedagio.NumeroValePedagio = retornoCompraValePedagio.CodigoRegistroValePedagio.ToString();
                cargaValePedagio.IdCompraValePedagio = retornoCompraValePedagio.IdCompraValePedagio.ToString();
                cargaValePedagio.ValorValePedagio = retornoCompraValePedagio.ValorCompra > 0 ? retornoCompraValePedagio.ValorCompra : compraValePedagio.ValorPrevioCalculado.HasValue ? compraValePedagio.ValorPrevioCalculado.Value : 0;
                cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Comprada;

                int idCompraValePedagio = retornoCompraValePedagio.IdCompraValePedagio;

                if (cargaValePedagio.TipoCompra == Dominio.Enumeradores.TipoCompraValePedagio.Tag)
                {
                    ServicoTarget.ValePedagio.ConfirmacaoPedagioRequest confirmacaoPedagioTag = new ServicoTarget.ValePedagio.ConfirmacaoPedagioRequest
                    {
                        IdCompraValePedagioViaFacil = retornoCompraValePedagio.IdCompraValePedagio
                    };

                    mensagemRetorno = string.Empty;
                    Servicos.Models.Integracao.InspectorBehavior inspectorConfirmacaoTag = new Servicos.Models.Integracao.InspectorBehavior();
                    ServicoTarget.ValePedagio.ConfirmarPedagioResponse retornoConfirmarPedagioTag = ConfirmarPedagioTag(autenticacao, confirmacaoPedagioTag, unitOfWork, ref mensagemRetorno, ref inspectorConfirmacaoTag);
                    SalvarXMLIntegracao(ref cargaValePedagio, inspectorConfirmacaoTag, "Confirmar compra tag Vale Pedagio " + mensagemRetorno, unitOfWork);

                    if (retornoConfirmarPedagioTag == null)
                    {
                        //Se não conseguir confirmar a Compra precisa Cancelar.
                        ServicoTarget.ValePedagio.CancelaCompraValePedagioRequest cancelaCompraValePedagio = new ServicoTarget.ValePedagio.CancelaCompraValePedagioRequest()
                        {
                            IdCompraValePedagio = idCompraValePedagio,
                            ViaFacil = true
                        };

                        string mensagemRetornoCancelamento = string.Empty;
                        Servicos.Models.Integracao.InspectorBehavior inspectorCancelamento = new Servicos.Models.Integracao.InspectorBehavior();
                        ServicoTarget.ValePedagio.CancelaCompraValePedagioResponse retornoCancelaCompraValePedagioResponse = CancelarCompraValePedagio(autenticacao, cancelaCompraValePedagio, unitOfWork, ref mensagemRetornoCancelamento, ref inspectorCancelamento);
                        SalvarXMLIntegracao(ref cargaValePedagio, inspectorCancelamento, "Cancelar compra Vale Pedagio " + mensagemRetornoCancelamento, unitOfWork);

                        cargaValePedagio.ProblemaIntegracao = !string.IsNullOrWhiteSpace(mensagemRetornoCancelamento) ? mensagemRetornoCancelamento : "Não foi possível confirmar compra pedagio Target, compra cancelada.";
                        cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        //Tarefa 5684, alterado para quando cancelar o vale pedagio por falha na Target deixar com situação Pendente para permitir o reenvio
                        cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Pendete; //Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Cancelada;
                        cargaValePedagio.DataIntegracao = DateTime.Now;
                        cargaValePedagio.NumeroTentativas++;

                        repCargaValePedagio.Atualizar(cargaValePedagio);

                        return;
                    }

                    cargaValePedagio.CodigoEmissaoValePedagioANTT = retornoConfirmarPedagioTag.IdVpoAntt;
                }

                cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Confirmada;
                cargaValePedagio.ProblemaIntegracao = string.Empty;
                cargaValePedagio.DataIntegracao = DateTime.Now;
                cargaValePedagio.NumeroTentativas++;

                repCargaValePedagio.Atualizar(cargaValePedagio);

                servicoCargaValePedagio.EnviarEmailTransportador(cargaValePedagio, integracaoTarget?.NotificarTransportadorPorEmail ?? false, tipoServicoMultisoftware);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cargaValePedagio.ProblemaIntegracao = "Falha no Serviço de Integração com a Target";
                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.DataIntegracao = DateTime.Now;
                cargaValePedagio.NumeroTentativas++;
                repCargaValePedagio.Atualizar(cargaValePedagio);
            }
        }

        public void SolicitarCancelamentoValePedagio(Servicos.ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo repCargaValePedagioIntegracaoArquivo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioRota repCargaValePedagioRota = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioRota(unitOfWork);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            int.TryParse(cargaValePedagio.IdCompraValePedagio, out int idCompraValePedagio);
            ServicoTarget.ValePedagio.CancelaCompraValePedagioRequest cancelaCompraValePedagio = new ServicoTarget.ValePedagio.CancelaCompraValePedagioRequest()
            {
                IdCompraValePedagio = idCompraValePedagio,
                ViaFacil = cargaValePedagio.TipoCompra == Dominio.Enumeradores.TipoCompraValePedagio.Tag ? true : false
            };

            string mensagemRetorno = string.Empty;
            Servicos.Models.Integracao.InspectorBehavior inspectorCancelamento = new Servicos.Models.Integracao.InspectorBehavior();
            ServicoTarget.ValePedagio.CancelaCompraValePedagioResponse retornoCancelaCompraValePedagioResponse = CancelarCompraValePedagio(autenticacao, cancelaCompraValePedagio, unitOfWork, ref mensagemRetorno, ref inspectorCancelamento);
            SalvarXMLIntegracao(ref cargaValePedagio, inspectorCancelamento, "Cancelar Vale Pedagio " + mensagemRetorno, unitOfWork);

            if (retornoCancelaCompraValePedagioResponse != null)
            {
                cargaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Cancelada;
            }
            else
            {
                cargaValePedagio.SituacaoValePedagio = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoValePedagio.Confirmada;
            }

            cargaValePedagio.ProblemaIntegracao = mensagemRetorno;

            cargaValePedagio.DataIntegracao = DateTime.Now;
            //cargaValePedagio.NumeroTentativas++;
            repCargaValePedagio.Atualizar(cargaValePedagio);
        }

        public bool ObterDocumento(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, ref byte[] documento, ref string mensagemRetorno, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            try
            {
                Servicos.ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao = this.Autenticar(cargaValePedagio, unidadeDeTrabalho, tipoServicoMultisoftware);

                int idTarget = 0;
                int.TryParse(cargaValePedagio.IdCompraValePedagio, out idTarget);

                if (idTarget == 0)
                    int.TryParse(cargaValePedagio.NumeroValePedagio, out idTarget);

                ServicoTarget.ValePedagio.EmissaoDocumentoRequest emissaoDocumento = new ServicoTarget.ValePedagio.EmissaoDocumentoRequest
                {
                    IdEntidade = idTarget,
                    Tipo = cargaValePedagio.TipoCompra == Dominio.Enumeradores.TipoCompraValePedagio.Tag ? 4 : 3
                };

                mensagemRetorno = string.Empty;
                Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();
                ServicoTarget.ValePedagio.EmissaoDocumentoResponse retornoEmissaoDocumentoResponse = EmitirDocumentoTarget(autenticacao, emissaoDocumento, unidadeDeTrabalho, ref mensagemRetorno, ref inspector);
                //SalvarXMLIntegracao(inspector, valePedagioMDFeCompra, Dominio.Enumeradores.TipoXMLValePedagio.BuscarDocumento, unidadeDeTrabalho);

                if (retornoEmissaoDocumentoResponse != null)
                {
                    documento = retornoEmissaoDocumentoResponse.DocumentoBinario;
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ObterDocumento Target: " + ex);
                mensagemRetorno = "Falha ObterDocumento Target: " + ex;

                return false;
            }
        }

        public decimal ConsultarValorPedagio(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValorPedagioIntegracao = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
            cargaConsultaValePedagio.NumeroTentativas++;

            try
            {
                Servicos.Embarcador.Frota.ValePedagio servicoValePedagio = new Servicos.Embarcador.Frota.ValePedagio(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget integracaoTarget = servicoValePedagio.ObterIntegracaoTarget(cargaConsultaValePedagio.Carga, _tipoServicoMultisoftware);

                Servicos.ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao = Autenticar(cargaConsultaValePedagio);

                if (autenticacao == null)
                    return 0;

                string mensagemRetorno = string.Empty;

                int numeroEixos = ObterNumeroEixos(carga);

                int categoriaVeiculo = ObterCategoriaPorEixos(numeroEixos, carga.Veiculo.ModeloVeicularCarga?.PadraoEixos);

                int idRotaModelo = ObterIdRotaValePedagio(categoriaVeiculo, autenticacao, cargaConsultaValePedagio, carga);

                ServicoTarget.ValePedagio.ObtencaoCustoRotaRequest consultaCustoRota = new ServicoTarget.ValePedagio.ObtencaoCustoRotaRequest()
                {
                    CategoriaVeiculo = categoriaVeiculo,
                    IdRotaModelo = idRotaModelo,
                    ModoPagamentoRota = 2
                };

                Servicos.Models.Integracao.InspectorBehavior inspectorCustoRota = new Servicos.Models.Integracao.InspectorBehavior();

                ServicoTarget.ValePedagio.ObtencaoCustoRotaResponse custoRota = BuscarCustoRota(autenticacao, consultaCustoRota, _unitOfWork, ref mensagemRetorno, ref inspectorCustoRota);
                servicoArquivoTransacao.Adicionar(cargaConsultaValePedagio, inspectorCustoRota.LastRequestXML, inspectorCustoRota.LastResponseXML, "xml", "BuscarCustoRota");

                if (custoRota != null) cargaConsultaValePedagio.ValorValePedagio = custoRota.ValorPedagioViaFacil;

                cargaConsultaValePedagio.DataIntegracao = DateTime.Now;
                cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaConsultaValePedagio.ProblemaIntegracao = "Integrado com sucesso";

                if (cargaConsultaValePedagio.ValorValePedagio == 0)
                    cargaConsultaValePedagio.ProblemaIntegracao = "Rota sem pedágio";

                repCargaConsultaValorPedagioIntegracao.Atualizar(cargaConsultaValePedagio);

                return cargaConsultaValePedagio.ValorValePedagio;

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ConsultaValorPedagioTarget");
                cargaConsultaValePedagio.ProblemaIntegracao = "Falha ao consultar valor pedagio Target";
                cargaConsultaValePedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                return -1;
            }
        }

        #endregion

        #region Métodos Privados

        private static int ObterNumeroEixos(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            bool eixosSuspensos = false;

            if (carga.TipoOperacao != null)
            {
                if (carga.TipoOperacao.TipoCarregamento.HasValue && carga.TipoOperacao.TipoCarregamento.Value == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoCargaTipo.Vazio)
                    eixosSuspensos = true;
            }

            int numeroEixos = 0;

            if (carga.Veiculo.ModeloVeicularCarga != null)
            {
                numeroEixos = carga.Veiculo.ModeloVeicularCarga.NumeroEixos ?? 0;
                if (eixosSuspensos)
                    numeroEixos -= carga.Veiculo.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
            }

            if (carga.VeiculosVinculados != null)
            {
                foreach (Dominio.Entidades.Veiculo reboque in carga.VeiculosVinculados.ToList())
                {
                    if (reboque.ModeloVeicularCarga != null && carga.Veiculo.ModeloVeicularCarga != null && reboque.ModeloVeicularCarga != carga.Veiculo.ModeloVeicularCarga)
                    {
                        numeroEixos += reboque.ModeloVeicularCarga.NumeroEixos ?? 0;

                        if (eixosSuspensos)
                            numeroEixos -= reboque.ModeloVeicularCarga.NumeroEixosSuspensos ?? 0;
                    }
                }
            }

            return numeroEixos;
        }

        private ServicoTarget.ValePedagio.EmissaoDocumentoResponse EmitirDocumentoTarget(ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, ServicoTarget.ValePedagio.EmissaoDocumentoRequest emissaoDocumento, Repositorio.UnitOfWork unitOfWork, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoTarget.ValePedagio.FreteTMSServiceClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceClient, ServicoTarget.ValePedagio.FreteTMSService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMS, out inspector);

            Servicos.ServicoTarget.ValePedagio.EmitirDocumentoRequest emitirDocumentoRequest = new ServicoTarget.ValePedagio.EmitirDocumentoRequest
            {
                auth = autenticacao,
                emissaoDocumento = emissaoDocumento
            };

            ServicoTarget.ValePedagio.EmissaoDocumentoResponse retornoEmissaoDocumentos = svcTarget.EmitirDocumento(emitirDocumentoRequest).EmitirDocumentoResult;

            if (retornoEmissaoDocumentos != null)
            {
                if (retornoEmissaoDocumentos.Erro != null)
                {
                    mensagemRetorno = string.Concat(retornoEmissaoDocumentos.Erro.CodigoErro.ToString(), " - ", retornoEmissaoDocumentos.Erro.MensagemErro);
                    return null;
                }
                else
                    return retornoEmissaoDocumentos;
            }
            else
            {
                mensagemRetorno = "BuscarDocumentoCompra não retornou cartão.";
                return null;
            }
        }

        private ServicoTarget.ValePedagio.ItemBuscarCartoesResponse BuscarCartaoMotorista(ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, ServicoTarget.ValePedagio.BuscaCartoesRequest buscarCartoes, Repositorio.UnitOfWork unitOfWork, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoTarget.ValePedagio.FreteTMSServiceClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceClient, ServicoTarget.ValePedagio.FreteTMSService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMS, out inspector);

            Servicos.ServicoTarget.ValePedagio.BuscarCartoesPortadorRequest buscarCartoesPortadorRequest = new ServicoTarget.ValePedagio.BuscarCartoesPortadorRequest
            {
                auth = autenticacao,
                buscaRequest = buscarCartoes
            };

            ServicoTarget.ValePedagio.BuscarCartoesResponse retornoBuscarCartoesResponse = svcTarget.BuscarCartoesPortador(buscarCartoesPortadorRequest).BuscarCartoesPortadorResult;

            if (retornoBuscarCartoesResponse != null)
            {
                if (retornoBuscarCartoesResponse.Erro != null)
                {
                    mensagemRetorno = string.Concat(retornoBuscarCartoesResponse.Erro.CodigoErro.ToString(), " - ", retornoBuscarCartoesResponse.Erro.MensagemErro);
                    return null;
                }
                else
                {
                    if (retornoBuscarCartoesResponse.ListaCartoesAtivos != null && retornoBuscarCartoesResponse.ListaCartoesAtivos.Count() > 0)
                    {
                        for (var i = 0; i < retornoBuscarCartoesResponse.ListaCartoesAtivos.Count(); i++)
                        {
                            if (retornoBuscarCartoesResponse.ListaCartoesAtivos[i].Ativo && retornoBuscarCartoesResponse.ListaCartoesAtivos[i].LiberacaoCarga == ServicoTarget.ValePedagio.LiberacaoCarga.Liberado)
                            {
                                mensagemRetorno = string.Empty;
                                return retornoBuscarCartoesResponse.ListaCartoesAtivos[i];
                            }
                            else
                            {
                                mensagemRetorno = "BuscarCartaoMotorista não retornou cartão Ativo e Liberado.";
                                return null;
                            }
                        }
                    }

                    mensagemRetorno = "BuscarCartaoMotorista não retornou cartão.";
                    return null;
                }
            }
            else
            {
                mensagemRetorno = "BuscarCartaoMotorista não retornou cartão.";
                return null;
            }
        }

        private ServicoTarget.ValePedagio.ObtencaoCustoRotaResponse BuscarCustoRota(ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, ServicoTarget.ValePedagio.ObtencaoCustoRotaRequest consultaCustoRota, Repositorio.UnitOfWork unitOfWork, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoTarget.ValePedagio.FreteTMSServiceClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceClient, ServicoTarget.ValePedagio.FreteTMSService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMS, out inspector);

            Servicos.ServicoTarget.ValePedagio.ObterCustoRotaRequest obterCustoRotaRequest = new ServicoTarget.ValePedagio.ObterCustoRotaRequest
            {
                auth = autenticacao,
                custoRotaRequest = consultaCustoRota
            };

            ServicoTarget.ValePedagio.ObtencaoCustoRotaResponse retornoCustoRota = svcTarget.ObterCustoRota(obterCustoRotaRequest).ObterCustoRotaResult;

            if (retornoCustoRota == null)
            {
                mensagemRetorno = "ObterCustoRota não retornou rotas.";
                return null;
            }

            if (retornoCustoRota.Erro == null)
                return retornoCustoRota;

            mensagemRetorno = string.Concat(retornoCustoRota.Erro.CodigoErro.ToString(), " - ", retornoCustoRota.Erro.MensagemErro);

            if (retornoCustoRota.Erro.CodigoErro == 97)
                return retornoCustoRota;

            return null;
        }

        private ServicoTarget.ValePedagio.CompraValePedagioResponse ComprarValePedagio(ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, ServicoTarget.ValePedagio.CompraValePedagioRequest compraValePedagio, Repositorio.UnitOfWork unitOfWork, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoTarget.ValePedagio.FreteTMSServiceClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceClient, ServicoTarget.ValePedagio.FreteTMSService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMS, out inspector);

            Servicos.ServicoTarget.ValePedagio.ComprarPedagioAvulsoRequest comprarPedagioAvulsoRequest = new ServicoTarget.ValePedagio.ComprarPedagioAvulsoRequest
            {
                auth = autenticacao,
                compraRequest = compraValePedagio
            };

            ServicoTarget.ValePedagio.CompraValePedagioResponse retornoCompraValePedagio = svcTarget.ComprarPedagioAvulso(comprarPedagioAvulsoRequest).ComprarPedagioAvulsoResult;

            if (retornoCompraValePedagio != null)
            {
                if (retornoCompraValePedagio.Erro != null)
                {
                    mensagemRetorno = string.Concat(retornoCompraValePedagio.Erro.CodigoErro.ToString(), " - ", retornoCompraValePedagio.Erro.MensagemErro);
                    return null;
                }
                else
                    return retornoCompraValePedagio;
            }
            else
            {
                mensagemRetorno = "ComprarValePedagio não retornou rotas.";
                return null;
            }
        }

        private ServicoTarget.ValePedagio.ConfirmarPedagioResponse ConfirmarPedagioTag(ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, ServicoTarget.ValePedagio.ConfirmacaoPedagioRequest confirmacaoPedagioTag, Repositorio.UnitOfWork unitOfWork, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoTarget.ValePedagio.FreteTMSServiceClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceClient, ServicoTarget.ValePedagio.FreteTMSService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMS, out inspector);

            Servicos.ServicoTarget.ValePedagio.ConfirmarPedagioTAGRequest confirmarPedagioTAGRequest = new ServicoTarget.ValePedagio.ConfirmarPedagioTAGRequest
            {
                auth = autenticacao,
                confirmacaoRequest = confirmacaoPedagioTag
            };

            ServicoTarget.ValePedagio.ConfirmarPedagioResponse retornoConfirmarPedagio = svcTarget.ConfirmarPedagioTAG(confirmarPedagioTAGRequest).ConfirmarPedagioTAGResult;

            if (retornoConfirmarPedagio != null)
            {
                if (retornoConfirmarPedagio.Erro != null)
                {
                    mensagemRetorno = string.Concat(retornoConfirmarPedagio.Erro.CodigoErro.ToString(), " - ", retornoConfirmarPedagio.Erro.MensagemErro);
                    return null;
                }
                else
                    return retornoConfirmarPedagio;
            }
            else
            {
                mensagemRetorno = "ObterCustoRota não retornou rotas.";
                return null;
            }

        }

        private ServicoTarget.ValePedagio.CancelaCompraValePedagioResponse CancelarCompraValePedagio(ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, ServicoTarget.ValePedagio.CancelaCompraValePedagioRequest cancelaCompraValePedagio, Repositorio.UnitOfWork unitOfWork, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoTarget.ValePedagio.FreteTMSServiceClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceClient, ServicoTarget.ValePedagio.FreteTMSService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMS, out inspector);

            Servicos.ServicoTarget.ValePedagio.CancelarCompraValePedagioRequest cancelarCompraValePedagioRequest = new ServicoTarget.ValePedagio.CancelarCompraValePedagioRequest
            {
                auth = autenticacao,
                cancelaVPRequest = cancelaCompraValePedagio
            };

            ServicoTarget.ValePedagio.CancelaCompraValePedagioResponse retornoCancelaCompraValePedagio = svcTarget.CancelarCompraValePedagio(cancelarCompraValePedagioRequest).CancelarCompraValePedagioResult;

            if (retornoCancelaCompraValePedagio != null)
            {
                if (retornoCancelaCompraValePedagio.Erro != null)
                {
                    mensagemRetorno = string.Concat(retornoCancelaCompraValePedagio.Erro.CodigoErro.ToString(), " - ", retornoCancelaCompraValePedagio.Erro.MensagemErro);
                    return null;
                }
                else
                    return retornoCancelaCompraValePedagio;
            }
            else
            {
                mensagemRetorno = "CancelarCompraValePedagio não retornou rotas.";
                return null;
            }
        }

        private ServicoTarget.ValePedagio.RotaResponse BuscarRotaIBGE(ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, ServicoTarget.ValePedagio.ListarRotaClienteRequest consultaRotaIBGE, string codigoRota, Repositorio.UnitOfWork unitOfWork, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoTarget.ValePedagio.FreteTMSServiceClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceClient, ServicoTarget.ValePedagio.FreteTMSService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMS, out inspector);

            Servicos.ServicoTarget.ValePedagio.ListarRotasRequest listarRotasRequest = new ServicoTarget.ValePedagio.ListarRotasRequest
            {
                auth = autenticacao,
                listarRotasRequest = consultaRotaIBGE
            };

            ServicoTarget.ValePedagio.ResultadoPaginadoListarRotasClienteResponse retornoRota = svcTarget.ListarRotas(listarRotasRequest).ListarRotasResult;

            int.TryParse(codigoRota, out int idRotaIntegracao);
            bool rotaLocalizada = false;

            if (retornoRota != null)
            {
                if (retornoRota.Erro != null && string.IsNullOrWhiteSpace(retornoRota.Erro.MensagemErro))
                {
                    mensagemRetorno = string.Concat(retornoRota.Erro.CodigoErro.ToString(), " - ", retornoRota.Erro.MensagemErro);
                    return null;
                }

                if (retornoRota.Itens != null && retornoRota.Itens.Count() > 0)
                {
                    string erroItem = string.Empty;
                    string erroRota = string.Empty;
                    for (var i = 0; i < retornoRota.Itens.Count(); i++)
                    {
                        if (retornoRota.Itens[i].Erro != null && String.IsNullOrWhiteSpace(retornoRota.Itens[i].Erro.MensagemErro))
                            erroItem = string.Concat(retornoRota.Itens[i].Erro.CodigoErro.ToString(), " - ", retornoRota.Itens[i].Erro.MensagemErro);
                        else
                        {
                            erroItem = string.Empty;
                            if (retornoRota.Itens[i].Rotas == null || retornoRota.Itens[i].Rotas.Count() == 0)
                                erroRota = "Sem rota cadastrada para IBGE Origem e IBGE Destino na Target.";
                            else
                            {
                                erroRota = string.Empty;
                                for (var j = 0; j < retornoRota.Itens[i].Rotas.Count(); j++)
                                {
                                    if (retornoRota.Itens[i].Rotas[j].Erro != null)
                                        erroRota = string.Concat(retornoRota.Itens[i].Rotas[j].Erro.CodigoErro.ToString(), " - ", retornoRota.Itens[i].Rotas[j].Erro.MensagemErro);
                                    else
                                    {
                                        if (idRotaIntegracao > 0)
                                        {
                                            if (idRotaIntegracao == retornoRota.Itens[i].Rotas[j].IdRotaCliente)
                                            {
                                                rotaLocalizada = true;
                                                mensagemRetorno = string.Empty;
                                                return retornoRota.Itens[i].Rotas[j];
                                            }
                                        }
                                        else
                                        {
                                            rotaLocalizada = true;
                                            mensagemRetorno = string.Empty;
                                            return retornoRota.Itens[i].Rotas[j];
                                        }
                                    }

                                }
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(erroItem))
                    {
                        mensagemRetorno = erroItem;

                        return null;
                    }
                    if (!string.IsNullOrWhiteSpace(erroRota))
                    {
                        mensagemRetorno = erroRota;

                        return null;
                    }

                    if (!rotaLocalizada)
                    {
                        mensagemRetorno = idRotaIntegracao > 0 ? "Rota com ID " + idRotaIntegracao.ToString() + " não localizada na Target." : "Rota não localizada na Target";

                        return null;
                    }
                }

                mensagemRetorno = "Nenhuma rota valida encontrada.";
                return null;
            }
            else
            {
                mensagemRetorno = "ListarRotasIBGE não retornou rotas.";
                return null;
            }
        }

        private int CadastrarRoteiroIBGE(ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, Servicos.ServicoTarget.ValePedagio.RoteiroRequest cadastrarRoteiroIBGE, Repositorio.UnitOfWork unitOfWork, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoTarget.ValePedagio.FreteTMSServiceClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceClient, ServicoTarget.ValePedagio.FreteTMSService>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMS, out inspector);

            Servicos.ServicoTarget.ValePedagio.CadastrarRoteiroRequest cadastrarRoteiroRequest = new ServicoTarget.ValePedagio.CadastrarRoteiroRequest
            {
                auth = autenticacao,
                roteiroRequest = cadastrarRoteiroIBGE
            };

            Servicos.ServicoTarget.ValePedagio.RoteiroResponse retornoRota = svcTarget.CadastrarRoteiro(cadastrarRoteiroRequest).CadastrarRoteiroResult;

            if (retornoRota != null)
            {
                if (retornoRota.Erro != null && string.IsNullOrWhiteSpace(retornoRota.Erro.MensagemErro))
                {
                    mensagemRetorno = string.Concat(retornoRota.Erro.CodigoErro.ToString(), " - ", retornoRota.Erro.MensagemErro);
                    return 0;
                }

                return retornoRota.IdRoteiro;
            }
            else
            {
                mensagemRetorno = "CadastrarRoteiro não retornou dados.";
                return 0;
            }
        }

        private int CadastrarRoteiroCoordenadas(ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, Servicos.ServicoTarget.ValePedagio.RotaDetalhadaRequest rotaDetalhada, ref string mensagemRetorno, ref Servicos.Models.Integracao.InspectorBehavior inspector, Repositorio.UnitOfWork unitOfWork)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ServicoTarget.ValePedagio.FreteTMSServiceExtendedClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceExtendedClient, ServicoTarget.ValePedagio.FreteTMSServiceExtended>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMSExtended, out inspector);

            Servicos.ServicoTarget.ValePedagio.CadastrarRoteiroDetalhadoRequest cadastrarRoteiroDetalhadoRequest = new ServicoTarget.ValePedagio.CadastrarRoteiroDetalhadoRequest
            {
                auth = autenticacao,
                rotaDetalhada = rotaDetalhada
            };

            ServicoTarget.ValePedagio.RotaDetalhadaResponse retornoRota = svcTarget.CadastrarRoteiroDetalhado(cadastrarRoteiroDetalhadoRequest).CadastrarRoteiroDetalhadoResult;

            if (retornoRota != null)
            {
                if (retornoRota.Erro != null && string.IsNullOrWhiteSpace(retornoRota.Erro.MensagemErro))
                {
                    mensagemRetorno = string.Concat(retornoRota.Erro.CodigoErro.ToString(), " - ", retornoRota.Erro.MensagemErro);
                    return 0;
                }

                return retornoRota.IdRotaCliente;
            }
            else
            {
                mensagemRetorno = "CadastrarRoteiro não retornou dados.";
                return 0;
            }
        }

        private int ObterCategoriaPorEixos(int quantidadeEixos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.PadraoEixosVeiculo? padraoEixos)
        {
            switch (quantidadeEixos)
            {
                case 0:
                    return 1; //Motocicletas, motonetas e bicicletas
                case 1:
                    return 1; //Motocicletas, motonetas e bicicletas
                case 2:
                    return padraoEixos == Dominio.ObjetosDeValor.Embarcador.Enumeradores.PadraoEixosVeiculo.Simples ? 2 : 7; //Automóvel, caminhoneta e furgão (dois eixos simples)
                case 3:
                    return 8; //Caminhão, caminhão trator e cavalo mecânico com semireboque (tres eixos duplos)
                case 4:
                    return 9; //Caminhão com reboque e cavalo mecânico com semi reboque (quatro eixos duplos)
                case 5:
                    return 10; //Caminhão com reboque e cavalo mecânico com semireboque (cinco eixos duplos)
                case 6:
                    return 11; //Caminhão com reboque e cavalo mecânico com semireboque (seis eixos duplos)
                case 7:
                    return 12; //Caminhão com reboque e cavalo mecânico com semireboque (sete eixos duplos)
                case 8:
                    return 13; //Caminhão com reboque e cavalo mecânico com semireboque (oito eixos duplos)
                case 9:
                    return 14; //Caminhão com reboque e cavalo mecânico com semireboque (nove eixos duplos)
                default:
                    return 2; //Automóvel, caminhoneta e furgão (dois eixos simples)
            }
        }

        private void SalvarXMLIntegracao(ref Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio, Servicos.Models.Integracao.InspectorBehavior inspector, string mensagemRetorno, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo repCargaValePedagioIntegracaoArquivo = new Repositorio.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo cargaValePedagioIntegracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo();

            cargaValePedagioIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastRequestXML, "xml", unidadeDeTrabalho);
            cargaValePedagioIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(inspector.LastResponseXML, "xml", unidadeDeTrabalho);
            cargaValePedagioIntegracaoArquivo.Data = DateTime.Now;
            cargaValePedagioIntegracaoArquivo.Mensagem = Utilidades.String.Left(mensagemRetorno, 400);
            cargaValePedagioIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

            repCargaValePedagioIntegracaoArquivo.Inserir(cargaValePedagioIntegracaoArquivo);

            cargaValePedagio.ArquivosTransacao.Add(cargaValePedagioIntegracaoArquivo);
        }

        private int ObterIdModoCompraValePedagio(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoCompraValePedagioTarget? ModoCompraValePedagioTarget)
        {
            switch (ModoCompraValePedagioTarget)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoCompraValePedagioTarget.CartaoTarget:
                    return 1;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoCompraValePedagioTarget.PedagioTagViaFacil:
                    return 2;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoCompraValePedagioTarget.PedagioTagVeloe:
                    return 5;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoCompraValePedagioTarget.PedagioConectCar:
                    return 6;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModoCompraValePedagioTarget.PedagioTagMoveMais:
                    return 7;
                default:
                    return 1;
            }
        }

        private int ObterIdRotaValePedagio(int categoriaVeiculo, Servicos.ServicoTarget.ValePedagio.AutenticacaoRequest autenticacao, Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaValePedagio, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            int ibgeInicio = 0;
            int ibgeFim = 0;
            int ibgeUltimaEntrega = 0;

            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo>(_unitOfWork);

            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(_unitOfWork);
            Repositorio.RotaFretePontosPassagem repositorioRotaFretePontosPassagem = new Repositorio.RotaFretePontosPassagem(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTarget integracaoTarget = servicoValePedagio.ObterIntegracaoTarget(cargaValePedagio.Carga, _tipoServicoMultisoftware);

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Servicos.Models.Integracao.InspectorBehavior inspector = new Servicos.Models.Integracao.InspectorBehavior();

            //Busca da Carga Rota Frete
            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);

            if (cargaRotaFrete != null)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem inicio = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFreteETipoPassagem(cargaRotaFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta).FirstOrDefault();
                ibgeInicio = (inicio?.ClienteOutroEndereco?.Localidade?.CodigoIBGE ?? 0) > 0 ? inicio?.ClienteOutroEndereco?.Localidade?.CodigoIBGE ?? 0 : inicio?.Cliente?.Localidade?.CodigoIBGE ?? 0;
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem fim = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFreteETipoPassagem(cargaRotaFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Retorno).LastOrDefault();
                ibgeFim = (fim?.ClienteOutroEndereco?.Localidade?.CodigoIBGE ?? 0) > 0 ? fim?.ClienteOutroEndereco?.Localidade?.CodigoIBGE ?? 0 : fim?.Cliente?.Localidade?.CodigoIBGE ?? 0;
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem ultimaEntrega = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFreteETipoPassagem(cargaRotaFrete.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega).LastOrDefault();
                ibgeUltimaEntrega = (ultimaEntrega?.ClienteOutroEndereco?.Localidade?.CodigoIBGE ?? 0) > 0 ? ultimaEntrega?.ClienteOutroEndereco?.Localidade?.CodigoIBGE ?? 0 : ultimaEntrega?.Cliente?.Localidade?.CodigoIBGE ?? 0;
                if (ibgeFim == 0)
                    ibgeFim = ibgeUltimaEntrega;
            }

            if (ibgeInicio == 0 || ibgeFim == 0)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPrimeiraPorCarga(carga.Codigo);

                if (ibgeInicio == 0)
                {
                    if (cargaPedido.Expedidor != null)
                        ibgeInicio = cargaPedido.Expedidor.Localidade.CodigoIBGE;
                    else
                        ibgeInicio = carga.DadosSumarizados.ClientesRemetentes.FirstOrDefault().Localidade.CodigoIBGE;
                }

                if (ibgeFim == 0)
                {
                    if (cargaPedido.Recebedor != null)
                        ibgeFim = cargaPedido.Recebedor.Localidade.CodigoIBGE;
                    else
                        ibgeFim = carga.DadosSumarizados.ClientesDestinatarios.FirstOrDefault().Localidade.CodigoIBGE;
                }
            }

            ServicoTarget.ValePedagio.ListarRotaClienteRequest consultaRotaIBGE = new ServicoTarget.ValePedagio.ListarRotaClienteRequest()
            {
                CodigoIBGEOrigem = ibgeInicio,
                CodigoIBGEDestino = ibgeFim
            };

            string mensagemRetorno = string.Empty;
            int idRota = 0;

            if (!string.IsNullOrWhiteSpace(cargaValePedagio.RotaFrete?.CodigoIntegracaoValePedagio))
                idRota = cargaValePedagio.RotaFrete.CodigoIntegracaoValePedagio.ToInt();
            else if (!string.IsNullOrWhiteSpace(carga.Rota?.CodigoIntegracaoValePedagio))
                idRota = carga.Rota.CodigoIntegracaoValePedagio.ToInt();

            if (idRota == 0)
            {
                if (integracaoTarget.CadastrarRotaPorIBGE)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem = null;
                    if (cargaRotaFrete != null)
                        pontosPassagem = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

                    int[] codigosIBGEMunicipioParadas = null;
                    if (pontosPassagem != null && pontosPassagem.Count > 2)
                    {
                        var pontosPassagensDistintos = pontosPassagem.Select(o => (o?.ClienteOutroEndereco?.Localidade ?? o.Cliente.Localidade).CodigoIBGE).Distinct().ToList();

                        List<int> codigosIBGES = new List<int>();
                        foreach (var ponto in pontosPassagensDistintos)
                        {
                            if (ponto != ibgeInicio && ponto != ibgeFim)
                                codigosIBGES.Add(ponto);
                        }

                        codigosIBGEMunicipioParadas = new int[codigosIBGES.Count];
                        int i = 0;
                        foreach (var ibgePassagem in codigosIBGES)
                        {
                            codigosIBGEMunicipioParadas[i] = ibgePassagem;
                            i += 1;
                        }

                    }
                    else if (ibgeUltimaEntrega > 0 && ibgeInicio == ibgeFim)
                    {
                        codigosIBGEMunicipioParadas = new int[1];
                        codigosIBGEMunicipioParadas[0] = ibgeUltimaEntrega;
                    }

                    Servicos.ServicoTarget.ValePedagio.RoteiroRequest roteiroRequest = new ServicoTarget.ValePedagio.RoteiroRequest()
                    {
                        CategoriaVeiculo = categoriaVeiculo,
                        CodigoIBGEMunicipioOrigem = ibgeInicio,
                        CodigoIBGEMunicipioDestino = ibgeFim,
                        CodigosIBGEMunicipioParadas = codigosIBGEMunicipioParadas,
                        NomeRoteiro = ibgeInicio.ToString() + " - " + ibgeFim.ToString()
                    };

                    Servicos.Models.Integracao.InspectorBehavior inspectorBuscarRota = new Servicos.Models.Integracao.InspectorBehavior();
                    idRota = CadastrarRoteiroIBGE(autenticacao, roteiroRequest, _unitOfWork, ref mensagemRetorno, ref inspectorBuscarRota);
                    servicoArquivoTransacao.Adicionar(cargaValePedagio, inspectorBuscarRota.LastRequestXML, inspectorBuscarRota.LastResponseXML, "xml", "CadastrarRoteiroIBGE");


                    System.Threading.Thread.Sleep(5000);
                }
                else if (integracaoTarget.CadastrarRotaPorCoordenadas)
                {
                    if (integracaoTarget.PreencherLatLongDaRotaIntegracao)
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem = null;
                        if (cargaRotaFrete != null)
                            pontosPassagem = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

                        ServicoTarget.ValePedagio.RotaDetalhadaParada[] paradas = new Servicos.ServicoTarget.ValePedagio.RotaDetalhadaParada[30];
                        if (pontosPassagem != null)
                        {
                            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> coordenadasPolilinha = Servicos.Embarcador.Logistica.Polilinha.Decodificar(cargaRotaFrete.PolilinhaRota);

                            Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem pontoPassagemInicio = pontosPassagem.FirstOrDefault();
                            Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem pontoPassagemFinal = pontosPassagem.LastOrDefault();

                            double.TryParse(pontoPassagemInicio.Latitude.ToString(), out double latitudeInicio);
                            double.TryParse(pontoPassagemInicio.Longitude.ToString(), out double longitudeInicio);
                            double.TryParse(pontoPassagemInicio.Cliente?.Latitude, out double latitudeClienteInicio);
                            double.TryParse(pontoPassagemInicio.Cliente?.Longitude, out double longitudeClienteInicio);

                            double.TryParse(pontoPassagemFinal.Latitude.ToString(), out double latitudeFinal);
                            double.TryParse(pontoPassagemFinal.Longitude.ToString(), out double longitudeFinal);
                            double.TryParse(pontoPassagemFinal.Cliente?.Latitude, out double latitudeClienteFinal);
                            double.TryParse(pontoPassagemFinal.Cliente?.Longitude, out double longitudeClienteFinal);

                            paradas[0] = new ServicoTarget.ValePedagio.RotaDetalhadaParada()
                            {
                                LAT = latitudeInicio != 0 ? latitudeInicio : latitudeClienteInicio,
                                LNG = longitudeInicio != 0 ? longitudeInicio : longitudeClienteInicio,
                            };

                            paradas[29] = new ServicoTarget.ValePedagio.RotaDetalhadaParada()
                            {
                                LAT = latitudeFinal != 0 ? latitudeFinal : latitudeClienteFinal,
                                LNG = longitudeFinal != 0 ? longitudeFinal : longitudeClienteFinal,
                            };

                            int intervaloLinhas = coordenadasPolilinha.Count / 29;

                            for (int a = 1; a < 29; a++)
                            {
                                paradas[a] = new ServicoTarget.ValePedagio.RotaDetalhadaParada()
                                {
                                    LAT = coordenadasPolilinha[intervaloLinhas * a].Latitude,
                                    LNG = coordenadasPolilinha[intervaloLinhas * a].Longitude,
                                };
                            }
                        }

                        Servicos.ServicoTarget.ValePedagio.RotaDetalhadaRequest roteiroRequest = new Servicos.ServicoTarget.ValePedagio.RotaDetalhadaRequest()
                        {
                            CategoriaVeiculo = categoriaVeiculo,
                            Paradas = paradas,
                            NomeRota = ibgeInicio.ToString() + " - " + ibgeFim.ToString(),
                            RotaTemporaria = false
                        };

                        ServicoTarget.ValePedagio.FreteTMSServiceExtendedClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceExtendedClient, ServicoTarget.ValePedagio.FreteTMSServiceExtended>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMSExtended, out inspector);

                        Servicos.ServicoTarget.ValePedagio.CadastrarRoteiroDetalhadoRequest cadastrarRoteiroDetalhadoRequest = new ServicoTarget.ValePedagio.CadastrarRoteiroDetalhadoRequest
                        {
                            auth = autenticacao,
                            rotaDetalhada = roteiroRequest
                        };

                        svcTarget.CadastrarRoteiroDetalhado(cadastrarRoteiroDetalhadoRequest);

                        Servicos.Models.Integracao.InspectorBehavior inspectorBuscarRota = new Servicos.Models.Integracao.InspectorBehavior();
                        idRota = CadastrarRoteiroCoordenadas(autenticacao, roteiroRequest, ref mensagemRetorno, ref inspectorBuscarRota, _unitOfWork);
                        servicoArquivoTransacao.Adicionar(cargaValePedagio, inspectorBuscarRota.LastRequestXML, inspectorBuscarRota.LastResponseXML, "xml", "CadastrarRoteiroCoordenadas");

                        System.Threading.Thread.Sleep(2000);
                    }
                    else if (integracaoTarget.PreencherPontosPassagemModificadoCliente)
                    {
                        List<Dominio.Entidades.RotaFretePontosPassagem> pontosPassagem = null;
                        if (carga.Rota != null)
                            pontosPassagem = repositorioRotaFretePontosPassagem.BuscarPorRotaFrete(carga.Rota.Codigo);

                        ServicoTarget.ValePedagio.RotaDetalhadaParada[] paradas = new Servicos.ServicoTarget.ValePedagio.RotaDetalhadaParada[pontosPassagem != null ? pontosPassagem.Count : 0];
                        if (pontosPassagem != null)
                        {
                            int i = 0;
                            foreach (Dominio.Entidades.RotaFretePontosPassagem ponto in pontosPassagem)
                            {
                                double.TryParse(ponto.Latitude.ToString(), out double latitude);
                                double.TryParse(ponto.Longitude.ToString(), out double longitude);

                                double.TryParse(ponto.Cliente?.Latitude, out double latitudeCliente);
                                double.TryParse(ponto.Cliente?.Longitude, out double longitudeCliente);

                                paradas[i] = new ServicoTarget.ValePedagio.RotaDetalhadaParada()
                                {
                                    LAT = latitude != 0 ? latitude : latitudeCliente,
                                    LNG = longitude != 0 ? longitude : longitudeCliente,
                                };

                                i += 1;
                            }
                        }

                        Servicos.ServicoTarget.ValePedagio.RotaDetalhadaRequest roteiroRequest = new Servicos.ServicoTarget.ValePedagio.RotaDetalhadaRequest()
                        {
                            CategoriaVeiculo = categoriaVeiculo,
                            Paradas = paradas,
                            NomeRota = ibgeInicio.ToString() + " - " + ibgeFim.ToString(),
                            RotaTemporaria = false
                        };

                        ServicoTarget.ValePedagio.FreteTMSServiceExtendedClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceExtendedClient, ServicoTarget.ValePedagio.FreteTMSServiceExtended>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMSExtended, out inspector);

                        Servicos.ServicoTarget.ValePedagio.CadastrarRoteiroDetalhadoRequest cadastrarRoteiroDetalhadoRequest = new ServicoTarget.ValePedagio.CadastrarRoteiroDetalhadoRequest
                        {
                            auth = autenticacao,
                            rotaDetalhada = roteiroRequest
                        };

                        svcTarget.CadastrarRoteiroDetalhado(cadastrarRoteiroDetalhadoRequest);

                        Servicos.Models.Integracao.InspectorBehavior inspectorBuscarRota = new Servicos.Models.Integracao.InspectorBehavior();
                        idRota = CadastrarRoteiroCoordenadas(autenticacao, roteiroRequest, ref mensagemRetorno, ref inspectorBuscarRota, _unitOfWork);
                        servicoArquivoTransacao.Adicionar(cargaValePedagio, inspectorBuscarRota.LastRequestXML, inspectorBuscarRota.LastResponseXML, "xml", "CadastrarRoteiroCoordenadas");

                        System.Threading.Thread.Sleep(2000);
                    }
                    else
                    {
                        List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem = null;
                        if (cargaRotaFrete != null)
                            pontosPassagem = repCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

                        ServicoTarget.ValePedagio.RotaDetalhadaParada[] paradas = new Servicos.ServicoTarget.ValePedagio.RotaDetalhadaParada[pontosPassagem != null ? pontosPassagem.Count() : 0];
                        if (pontosPassagem != null)
                        {
                            int i = 0;
                            foreach (Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem ponto in pontosPassagem)
                            {
                                double.TryParse(ponto.Latitude.ToString(), out double latitude);
                                double.TryParse(ponto.Longitude.ToString(), out double longitude);

                                double.TryParse(ponto.Cliente?.Latitude, out double latitudeCliente);
                                double.TryParse(ponto.Cliente?.Longitude, out double longitudeCliente);

                                paradas[i] = new ServicoTarget.ValePedagio.RotaDetalhadaParada()
                                {
                                    LAT = latitude != 0 ? latitude : latitudeCliente,
                                    LNG = longitude != 0 ? longitude : longitudeCliente,
                                };

                                i += 1;
                            }
                        }

                        Servicos.ServicoTarget.ValePedagio.RotaDetalhadaRequest roteiroRequest = new Servicos.ServicoTarget.ValePedagio.RotaDetalhadaRequest()
                        {
                            CategoriaVeiculo = categoriaVeiculo,
                            Paradas = paradas,
                            NomeRota = ibgeInicio.ToString() + " - " + ibgeFim.ToString(),
                            RotaTemporaria = false
                        };

                        ServicoTarget.ValePedagio.FreteTMSServiceExtendedClient svcTarget = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(_unitOfWork).ObterClient<ServicoTarget.ValePedagio.FreteTMSServiceExtendedClient, ServicoTarget.ValePedagio.FreteTMSServiceExtended>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Target_FreteTMSExtended, out inspector);

                        Servicos.ServicoTarget.ValePedagio.CadastrarRoteiroDetalhadoRequest cadastrarRoteiroDetalhadoRequest = new ServicoTarget.ValePedagio.CadastrarRoteiroDetalhadoRequest
                        {
                            auth = autenticacao,
                            rotaDetalhada = roteiroRequest
                        };

                        svcTarget.CadastrarRoteiroDetalhado(cadastrarRoteiroDetalhadoRequest);

                        Servicos.Models.Integracao.InspectorBehavior inspectorBuscarRota = new Servicos.Models.Integracao.InspectorBehavior();
                        idRota = CadastrarRoteiroCoordenadas(autenticacao, roteiroRequest, ref mensagemRetorno, ref inspectorBuscarRota, _unitOfWork);
                        servicoArquivoTransacao.Adicionar(cargaValePedagio, inspectorBuscarRota.LastRequestXML, inspectorBuscarRota.LastResponseXML, "xml", "CadastrarRoteiroCoordenadas");

                        System.Threading.Thread.Sleep(2000);
                    }
                }
                else
                {
                    Servicos.Models.Integracao.InspectorBehavior inspectorBuscarRota = new Servicos.Models.Integracao.InspectorBehavior();
                    ServicoTarget.ValePedagio.RotaResponse rota = BuscarRotaIBGE(autenticacao, consultaRotaIBGE, carga.Rota?.CodigoIntegracaoValePedagio, _unitOfWork, ref mensagemRetorno, ref inspectorBuscarRota);
                    servicoArquivoTransacao.Adicionar(cargaValePedagio, inspectorBuscarRota.LastRequestXML, inspectorBuscarRota.LastResponseXML, "xml", "BuscarRotaIBGE");

                    if (rota != null)
                        idRota = rota.IdRotaCliente;
                }
            }

            return idRota;
        }

        private string ObterCpfCnpjTransportador(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            string transportadoraCpfCnpj = string.Empty;

            if (carga.Veiculo?.Tipo == "T" && carga.Veiculo.Proprietario != null)
                transportadoraCpfCnpj = carga.Veiculo.Proprietario.CPF_CNPJ_SemFormato;
            else if (carga.Empresa != null)
                transportadoraCpfCnpj = carga.Empresa?.CNPJ_SemFormato;

            return transportadoraCpfCnpj;
        }

        #endregion
    }
}
