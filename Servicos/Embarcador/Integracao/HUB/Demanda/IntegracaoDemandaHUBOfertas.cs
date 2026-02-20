using Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Hub;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.Enumeradores;
using Newtonsoft.Json;
using Servicos.Embarcador.Integracao.HUB.Base;
using Servicos.Embarcador.Integracao.HUB.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.HUB.Demanda
{
    public class IntegracaoDemandaHUBOfertas : IntegracaoHUBOfertasBase
    {
        #region Propriedades Privadas
        private readonly Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas _repositorioGrupoTransportadoresHUBOfertas;
        private readonly Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas _repositorioTransportadorGrupoTransportadoresHUBOfertas;
        private readonly Repositorio.Embarcador.Pedidos.Pedido _repositorioPedido;
        private readonly Repositorio.Embarcador.Logistica.ConfiguracaoRotaFrete _repositorioConfiguracaoRotaFrete;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly HubHttpClient _hubHttp;
        protected readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas _repositorioCargaIntegracaoHUB;
        #endregion

        #region Construtores
        public IntegracaoDemandaHUBOfertas(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware) : base()
        {
            _unitOfWork = unitOfWork;
            _repositorioTransportadorGrupoTransportadoresHUBOfertas = new Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas(_unitOfWork);
            _repositorioGrupoTransportadoresHUBOfertas = new Repositorio.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas(_unitOfWork);
            _repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            _repositorioCargaIntegracaoHUB = new Repositorio.Embarcador.Cargas.CargaIntegracaoHUBOfertas(_unitOfWork);
            _repositorioConfiguracaoRotaFrete = new Repositorio.Embarcador.Logistica.ConfiguracaoRotaFrete(_unitOfWork);
            _hubHttp = new HubHttpClient(new Repositorio.Embarcador.Configuracoes.IntegracaoHUB(unitOfWork).BuscarPrimeiroRegistro());
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }
        #endregion

        #region Metodos Publicos
        public async Task<HttpRequisicaoResposta> GerarIntegracaoHUBDemanda(Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas cargaIntegracaoHUB)
        {
            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();

            if (cargaIntegracaoHUB?.Carga?.DataCarregamentoCarga is null)
                throw new ServicoException("Uma data de carregamento deve ser informada.");

            respostaHttp = await _hubHttp.PostAsync("/api/TransportDemands/Queue", await GerarDemanda(cargaIntegracaoHUB));

            Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.RetornoIntegracao retornoIntegracao = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.RetornoIntegracao>(respostaHttp.conteudoResposta);

            if (respostaHttp == null && respostaHttp.httpStatusCode != HttpStatusCode.OK || !retornoIntegracao.Sucesso)
                throw new ServicoException(!retornoIntegracao.Erros.IsNullOrEmpty() ? string.Join(",", retornoIntegracao.Erros) : "Problema ao tentar integrar com Hub de Ofertas.");

            cargaIntegracaoHUB.Protocolo = retornoIntegracao.ProtocoloIntegracao;

            return respostaHttp;
        }

        #endregion 

        #region Metodos Privados

        private async Task<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.DadosEnvioDemanda> GerarDemanda(Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoHUBOfertas cargaIntegracaoHUB)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = _repositorioPedido.BuscarPorCarga(cargaIntegracaoHUB.Carga.Codigo);

            Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete = ObterConfiguracaoRotaFrete(cargaIntegracaoHUB.Carga, pedidos.FirstOrDefault());
            List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas> gruposTransportadoresHUBOfertas = await _repositorioGrupoTransportadoresHUBOfertas.BuscarPorConfiguracaoRotaFrete(configuracaoRotaFrete?.Codigo ?? 0);

            bool freteDedicado = (cargaIntegracaoHUB.Carga.Empresa != null && cargaIntegracaoHUB.Carga.Veiculo != null);

            int contagem = 0;

            StatusDemandaTransporte statusDemandaTransporte = cargaIntegracaoHUB.TipoEnvioHUBOfertas != TipoEnvioHUBOfertas.CancelamentoDemandaOferta && cargaIntegracaoHUB.TipoEnvioHUBOfertas != TipoEnvioHUBOfertas.FinalizacaoDemandaOferta ? (freteDedicado) ? StatusDemandaTransporte.Confirmada : !gruposTransportadoresHUBOfertas.IsNullOrEmpty() || cargaIntegracaoHUB.Carga.Empresa != null ? StatusDemandaTransporte.Ofertada : StatusDemandaTransporte.Pendente : cargaIntegracaoHUB.TipoEnvioHUBOfertas == TipoEnvioHUBOfertas.CancelamentoDemandaOferta ? StatusDemandaTransporte.Cancelada : StatusDemandaTransporte.Finalizada;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.DadosEnvioDemanda demanda = PopularDadosEnvioDemanda(cargaIntegracaoHUB.Carga, pedidos, freteDedicado, statusDemandaTransporte);

            demanda.DemandaTransporte.Rotas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Rota>();

            demanda.DemandaTransporte.Rotas.AddRange(PopularRotasOrigens(cargaIntegracaoHUB.Carga, ref contagem));

            demanda.DemandaTransporte.Rotas.AddRange(PopularRotasEntrega(cargaIntegracaoHUB.Carga, ref contagem));

            if (statusDemandaTransporte != StatusDemandaTransporte.Cancelada) await GerarOfertas(demanda, statusDemandaTransporte, pedidos, cargaIntegracaoHUB.Carga, gruposTransportadoresHUBOfertas, configuracaoRotaFrete);

            demanda.DemandaTransporte.Id = (await _repositorioCargaIntegracaoHUB.ConsultarIntegracaoCargaEnviadaHUB(cargaIntegracaoHUB.Carga.Codigo))?.IdVinculoDemanda ?? null;

            return demanda;
        }

        private Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete ObterConfiguracaoRotaFrete(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            Dominio.Entidades.Localidade localidadeOrigem = pedido.Origem;

            if (localidadeOrigem == null)
                return null;

            Carga.CargaDadosSumarizados servicoCargaDadosSumarizados = new Carga.CargaDadosSumarizados(_unitOfWork);
            List<Dominio.ObjetosDeValor.Localidade> localidadesDestino = servicoCargaDadosSumarizados.ObterDestinos(carga, _unitOfWork, _tipoServicoMultisoftware);
            List<int> listaCodigoLocalidadeDestino = (from o in localidadesDestino select o.Codigo).Distinct().ToList();
            List<string> listaUfDestino = (from o in localidadesDestino where !string.IsNullOrWhiteSpace(o.SiglaUF) select o.SiglaUF).Distinct().ToList();

            if (listaUfDestino.Count == 0)
            {
                if (!(carga.DadosSumarizados?.Destinos?.EndsWith(" - EX") ?? false))
                    return null;

                listaUfDestino.Add("EX");
            }

            Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete = _repositorioConfiguracaoRotaFrete.BuscarPrimeiraDisponivel(localidadeOrigem.Codigo, carga.Filial?.Codigo ?? 0, listaUfDestino, listaCodigoLocalidadeDestino, carga.TipoDeCarga?.Codigo ?? 0, carga.ModeloVeicularCarga?.Codigo ?? 0, false, true);

            return configuracaoRotaFrete;
        }

        private async Task<bool> GerarOfertas(Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.DadosEnvioDemanda demanda, StatusDemandaTransporte statusDemandaTransporte, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas> gruposTransportadoresHUBOfertas, Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete)
        {
            try
            {
                if (carga.Empresa != null && carga.Veiculo != null)
                {
                    demanda.DemandaTransporte.Ofertas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Oferta>()
                    {
                        PopularOfertaDemandaDedicada(carga)
                    };
                }
                else if (carga.Empresa != null)
                {
                    demanda.DemandaTransporte.Ofertas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Oferta>()
                    {
                        PopularOfertaDemandaContrato(carga.Empresa)
                    };
                }
                else if (!gruposTransportadoresHUBOfertas.IsNullOrEmpty())
                {
                    demanda.DemandaTransporte.Ofertas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Oferta>();

                    DateTime? dataInicio =
                        configuracaoRotaFrete.HoraEnvioOfertaHUBOfertas != null && configuracaoRotaFrete.HoraEnvioOfertaHUBOfertas != TimeSpan.MinValue && configuracaoRotaFrete.DiasAntecedenciaEnvioTransportadorRota > 0
                        ? (carga.DataCarregamentoCarga ?? DateTime.Now).AddDays(configuracaoRotaFrete.DiasAntecedenciaHUBOfertas > 0 ? -configuracaoRotaFrete.DiasAntecedenciaHUBOfertas : 0).Date.Add(configuracaoRotaFrete.HoraEnvioOfertaHUBOfertas ?? TimeSpan.MinValue)
                        : configuracaoRotaFrete.HoraEnvioOfertaHUBOfertas != null && configuracaoRotaFrete.HoraEnvioOfertaHUBOfertas != TimeSpan.MinValue ? ValidarHoraOferta(DateTime.Now.Date.Add(configuracaoRotaFrete.HoraEnvioOfertaHUBOfertas ?? TimeSpan.MinValue)) : DateTime.Now;

                    if (configuracaoRotaFrete.HoraEnvioOfertaHUBOfertas != null && configuracaoRotaFrete.HoraEnvioOfertaHUBOfertas != TimeSpan.MinValue && configuracaoRotaFrete.DiasAntecedenciaEnvioTransportadorRota > 0) dataInicio = ValidarDiaOferta(dataInicio ?? DateTime.Now, configuracaoRotaFrete);

                    if (configuracaoRotaFrete.TipoOferta == 1)
                    {
                        int tempoSomadoGrupos = gruposTransportadoresHUBOfertas.Sum(x => x.TempoOfertarExclusivamenteParaGrupo);
                        DateTime? dataFimGrupo = (dataInicio ?? DateTime.Now).AddMinutes(tempoSomadoGrupos);

                        foreach (Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas grupoTransportadores in gruposTransportadoresHUBOfertas.OrderBy(x => x.SequenciaOferta).ToList())
                        {
                            List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas> transportadoresHUBOfertas =
                                await _repositorioTransportadorGrupoTransportadoresHUBOfertas.BuscarPorGrupoTransportadorAsync(grupoTransportadores.Codigo);

                            foreach (Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas transportadorHUB in transportadoresHUBOfertas)
                            {
                                demanda.DemandaTransporte.Ofertas.Add(PopularOfertaDemandaAdiciona(grupoTransportadores, transportadorHUB, configuracaoRotaFrete, dataFimGrupo, ref dataInicio));
                            }

                            dataInicio = (dataInicio ?? DateTime.Now).AddMinutes(grupoTransportadores.TempoOfertarExclusivamenteParaGrupo);
                        }
                    }
                    else if (configuracaoRotaFrete.TipoOferta == 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas grupoTransportadores in gruposTransportadoresHUBOfertas.OrderBy(x => x.SequenciaOferta).ToList())
                        {
                            List<Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas> transportadoresHUBOfertas =
                                await _repositorioTransportadorGrupoTransportadoresHUBOfertas.BuscarPorGrupoTransportadorAsync(grupoTransportadores.Codigo);

                            DateTime? dataInicioGrupo = dataInicio;
                            DateTime? dataFimGrupo = (dataInicioGrupo ?? DateTime.Now).AddMinutes(grupoTransportadores.TempoOfertarExclusivamenteParaGrupo);

                            foreach (Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas transportadorHUB in transportadoresHUBOfertas)
                            {
                                demanda.DemandaTransporte.Ofertas.Add(PopularOfertaDemandaExclusiva(grupoTransportadores, transportadorHUB, configuracaoRotaFrete, dataInicioGrupo, ref dataFimGrupo));
                            }

                            dataInicio = dataFimGrupo;
                        }
                    }

                    demanda.DemandaTransporte.PrazoAceite = demanda.DemandaTransporte.Ofertas.Max(x => x.DataFim);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }

        private DateTime ValidarDiaOferta(DateTime date, Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete)
        {
            if (!configuracaoRotaFrete.EnviarTransportadorRotaSegundaHUB && !configuracaoRotaFrete.EnviarTransportadorRotaTercaHUB && !configuracaoRotaFrete.EnviarTransportadorRotaQuartaHUB && !configuracaoRotaFrete.EnviarTransportadorRotaQuintaHUB && !configuracaoRotaFrete.EnviarTransportadorRotaSextaHUB && !configuracaoRotaFrete.EnviarTransportadorRotaSabadoHUB && !configuracaoRotaFrete.EnviarTransportadorRotaDomingoHUB)
                return date;

            while (true)
            {
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        if (configuracaoRotaFrete.EnviarTransportadorRotaDomingoHUB)
                            return date;
                        break;

                    case DayOfWeek.Monday:
                        if (configuracaoRotaFrete.EnviarTransportadorRotaSegundaHUB)
                            return date;
                        break;

                    case DayOfWeek.Tuesday:
                        if (configuracaoRotaFrete.EnviarTransportadorRotaTercaHUB)
                            return date;
                        break;

                    case DayOfWeek.Wednesday:
                        if (configuracaoRotaFrete.EnviarTransportadorRotaQuartaHUB)
                            return date;
                        break;

                    case DayOfWeek.Thursday:
                        if (configuracaoRotaFrete.EnviarTransportadorRotaQuintaHUB)
                            return date;
                        break;

                    case DayOfWeek.Friday:
                        if (configuracaoRotaFrete.EnviarTransportadorRotaSextaHUB)
                            return date;
                        break;

                    case DayOfWeek.Saturday:
                        if (configuracaoRotaFrete.EnviarTransportadorRotaSabadoHUB)
                            return date;
                        break;
                }

                date = date.AddDays(-1);
            }
        }

        private DateTime ValidarHoraOferta(DateTime date)
        {
            if (date.Hour < DateTime.Now.Hour)
                return date.AddDays(1);

            return date;
        }
        #endregion 

        #region Metodos Popular Envio Demanda
        private Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.DadosEnvioDemanda PopularDadosEnvioDemanda(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, bool freteDedicado, StatusDemandaTransporte statusDemandaTransporte)
        {
            string observacao = (!string.IsNullOrEmpty(carga.Observacao) ? $"{carga.Observacao}" : "") + (!pedidos.IsNullOrEmpty() ? $" - {string.Join("-", pedidos.Select(x => x.ObservacaoCTe))}" : "");
            Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.DadosEnvioDemanda demanda = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.DadosEnvioDemanda
            {
                DemandaTransporte = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.DemandaTransporte()
                {
                    IdExterno = carga.Codigo.ToString(),
                    NumeroExterno = carga.CodigoCargaEmbarcador,
                    DataCarregamento = carga.DataCarregamentoCarga,
                    TipoDestinatarioOferta = new Tipo
                    {
                        Id = "0efc62f8-c8e1-480e-9ddd-c2c2920c64d0"
                    },
                    Carga = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Carga
                    {
                        Numero = carga.CodigoCargaEmbarcador
                    },
                    TipoCarga = new Tipo
                    {
                        Id = RetornarTipoCarga(carga?.TipoDeCarga?.TipoCargaMDFe ?? TipoCargaMDFe.NaoDefinido)
                    },
                    TipoNegociacao = new Tipo
                    {
                        Id = RetornarTipoFrete()
                    },
                    PrecoFreteReferencia = carga.ValorFreteAPagar,
                    DescricaoTipoOperacaoRemetente = carga?.TipoOperacao?.Descricao ?? "",
                    DescricaoTipoVeiculoRemetente = carga?.ModeloVeicularCarga?.Descricao ?? "",
                    PrazoAceite = (carga.DataCarregamentoCarga ?? DateTime.Now).AddMinutes(-1),
                    PrazoConfirmacao = carga.DataCarregamentoCarga,
                    DataEntrega = pedidos.Max(x => x.PrevisaoEntrega) ?? pedidos.Max(x => x.DataPrevisaoChegadaDestinatario),
                    Status = statusDemandaTransporte.GetHashCode(),
                    Observacao = observacao,
                    UnidadesMedida = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.UnidadeMedida>()
                }
            };

            if (!pedidos.IsNullOrEmpty())
            {

                if (pedidos.Sum(x => x.PesoTotal) > 0)
                    demanda.DemandaTransporte.UnidadesMedida.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.UnidadeMedida
                    {
                        Capacidade = pedidos.Sum(x => x.PesoTotal),
                        Unidade = new Tipo
                        {
                            Id = "61ff0ced-d627-477a-9758-fbb873f8fc1f"
                        }
                    });

                if (pedidos.Sum(x => x.CubagemTotal) > 0)
                    demanda.DemandaTransporte.UnidadesMedida.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.UnidadeMedida
                    {
                        Capacidade = pedidos.Sum(x => x.CubagemTotal),
                        Unidade = new Tipo
                        {
                            Id = "c22f586c-037c-4d72-a3f8-802849cef1c8"
                        }
                    });

                if (pedidos.Sum(x => x.TotalPallets) > 0)
                    demanda.DemandaTransporte.UnidadesMedida.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.UnidadeMedida
                    {
                        Capacidade = pedidos.Sum(x => x.TotalPallets),
                        Unidade = new Tipo
                        {
                            Id = "06704a33-3e99-409e-9d9c-91c07a4ebb5e"
                        }
                    });
            }

            return demanda;
        }

        private string RetornarTipoCarga(TipoCargaMDFe tipoCarga)
        {
            switch (tipoCarga)
            {
                case TipoCargaMDFe.CargaGeral:
                case TipoCargaMDFe.PerigosaCargaGeral:
                case TipoCargaMDFe.PerigosaGranelSolido:
                case TipoCargaMDFe.PerigosaConteinerizada:
                case TipoCargaMDFe.GranelPressurizada:
                case TipoCargaMDFe.GranelSolido:
                case TipoCargaMDFe.Conteinerizada:
                case TipoCargaMDFe.Neogranel:
                    return "2fe5f675-5bf5-4422-82b9-3200f4eb8bdf";
                case TipoCargaMDFe.GranelLiquido:
                case TipoCargaMDFe.PerigosaGranelLiquido:
                    return "9e053171-0bae-4603-bb91-a67595204681";
                case TipoCargaMDFe.Frigorificada:
                case TipoCargaMDFe.PerigosaFrigorificada:
                    return "5ee956c8-b204-4df2-a769-b2f0f9e012a6";
                default:
                    throw new ServicoException($"{tipoCarga.ObterDescricao()} não mapeado, revise o Tipo de carga.");
            }
        }

        private string RetornarTipoFrete()
        {
            //if (freteDedicado)
            //    return "f6dec25f-c353-4ac2-be48-d2657141f889";

            //if (spot)
            //    return "22f39d5e-fe18-444f-b335-dd022e74c301";

            return "b018518a-dc89-46b3-bd96-d12aa07df201";
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Veiculo AdicionarVeiculo(Dominio.Entidades.Veiculo veiculo)
        {
            if (veiculo == null) return null;

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Veiculo
            {
                VeiculoTerrestre = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.VeiculoTerrestre
                {
                    Placa = veiculo.Placa
                }
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Oferta PopularOfertaDemandaDedicada(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Oferta oferta = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Oferta
            {
                Transportadora = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Transportadora
                {
                    Documentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Documento>()
                }
            };

            Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Veiculo reboquesEncadeados = null;

            if (carga.VeiculosVinculados != null && carga.VeiculosVinculados.Any())
            {
                foreach (Dominio.Entidades.Veiculo veiculo in carga.VeiculosVinculados)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Veiculo veiculoAtual = AdicionarVeiculo(veiculo);
                    veiculoAtual.Reboque = reboquesEncadeados;
                    reboquesEncadeados = veiculoAtual;
                }
            }

            Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Veiculo veiculoPrincipal = AdicionarVeiculo(carga.Veiculo);
            veiculoPrincipal.Reboque = reboquesEncadeados;

            oferta.Veiculo = veiculoPrincipal;


            oferta.Transportadora.Documentos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Documento
            {
                NumeroDocumento = carga.Empresa.CNPJ_SemFormato,
                Tipo = new Tipo
                {
                    Id = "a4b0f7e8-8bf3-48ef-a3ca-e736fb68277a"
                }
            });
            oferta.Validacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Validacao
            {
                VeiculosValidados = true,
                ObservacaoVeiculosValidados = carga.Veiculo.Observacao,
                ObservacaoTripulacaoValidada = carga.Veiculo.Observacao,

            };

            if (!carga.Motoristas.IsNullOrEmpty())
            {
                oferta.Veiculo.Tripulacao = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Tripulacao>()
                {
                    new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Tripulacao
                    {
                        OperadorTransporte = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Transportadora
                        {
                            Documentos = carga.Motoristas.Select(x => new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Documento
                            {
                                NumeroDocumento = x.CPF,
                                Tipo = new Tipo
                                {
                                    Id = x.CPF.Length > 11 ? "a4b0f7e8-8bf3-48ef-a3ca-e736fb68277a" : "8f917d47-b613-47d9-9284-89c5366e2406"
                                }
                            }).ToList()
                        },
                        TipoOperadorTransporte = new Tipo
                        {
                            Id = "759859ca-f199-4c5f-9ed1-7404c3ac54d4"
                        }
                    }
                };

                oferta.Validacao.TripulacaoValidada = true;
            }



            return oferta;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Oferta PopularOfertaDemandaAdiciona(Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas grupoTransportadorHUBOfertas, Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas transportador, Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete, DateTime? dataFim, ref DateTime? dataInicioOferta)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Oferta oferta = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Oferta
            {
                Transportadora = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Transportadora
                {
                    Documentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Documento>
                    {
                        new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Documento
                        {
                            NumeroDocumento = transportador.Empresa.CNPJ_SemFormato,
                            Tipo = new Tipo
                            {
                                Id = "a4b0f7e8-8bf3-48ef-a3ca-e736fb68277a"
                            }
                        }
                    }
                },
                DataInicio = dataInicioOferta ?? DateTime.Now,
                DataFim = dataFim ?? DateTime.Now
            };

            return oferta;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Oferta PopularOfertaDemandaExclusiva(Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.GrupoTransportadoresHUBOfertas grupoTransportadorHUBOfertas, Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete.TransportadorGrupoTransportadoresHUBOfertas transportador, Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotaFrete configuracaoRotaFrete, DateTime? dataInicioOferta, ref DateTime? dataFimOferta)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Oferta oferta = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Oferta
            {
                Transportadora = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Transportadora
                {
                    Documentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Documento>
                    {
                        new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Documento
                        {
                            NumeroDocumento = transportador.Empresa.CNPJ_SemFormato,
                            Tipo = new Tipo
                            {
                                Id = "a4b0f7e8-8bf3-48ef-a3ca-e736fb68277a"
                            }
                        }
                    }
                },
                DataInicio = dataInicioOferta,
                DataFim = dataFimOferta ?? DateTime.Now
            };

            return oferta;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Oferta PopularOfertaDemandaContrato(Dominio.Entidades.Empresa empresa)
        {

            Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Oferta oferta = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Oferta();
            oferta.Transportadora = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Transportadora();
            oferta.Transportadora.Documentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Documento>();

            oferta.Transportadora.Documentos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Documento
            {
                NumeroDocumento = empresa.CNPJ_SemFormato,
                Tipo = new Tipo
                {
                    Id = "a4b0f7e8-8bf3-48ef-a3ca-e736fb68277a"
                }
            });

            return oferta;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Rota> PopularRotasEntrega(Dominio.Entidades.Embarcador.Cargas.Carga carga, ref int sequencia)
        {

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Rota> rotas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Rota>();

            foreach (IGrouping<double, Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoAgrupado in carga.Pedidos.GroupBy(x => x.Pedido.Destinatario.CPF_CNPJ).OrderBy(x => x.FirstOrDefault().OrdemEntrega).ToList())
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidoAgrupado.FirstOrDefault();

                string observacao = (!string.IsNullOrEmpty(cargaPedido.Pedido.Observacao) ? $"{cargaPedido.Pedido.Observacao}" : "") + (!string.IsNullOrEmpty(cargaPedido.Pedido.ObservacaoInterna) ? $" - {cargaPedido.Pedido.ObservacaoInterna}" : "");

                sequencia++;
                rotas.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Rota
                {
                    Tipo = 1,
                    Sequencia = sequencia,
                    Entidade = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Entidade
                    {
                        Documentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Documento>
                        {
                            new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Documento
                            {
                                NumeroDocumento = cargaPedido.Pedido.Destinatario.CPF_CNPJ_SemFormato,
                                Tipo = new Tipo
                                {
                                    Id = "a4b0f7e8-8bf3-48ef-a3ca-e736fb68277a"
                                }
                            }
                        },
                        PessoaJuridica = new EntidadeLegal
                        {
                            RazaoSocial = cargaPedido.Pedido.Destinatario.NomeCNPJ,
                            NomeFantasia = string.IsNullOrEmpty(cargaPedido.Pedido.Destinatario.NomeFantasia) ? cargaPedido.Pedido.Destinatario.Nome : cargaPedido.Pedido.Destinatario.NomeFantasia,
                            TelefoneEmpresa = cargaPedido.Pedido.Destinatario.Telefone1
                        },
                        Enderecos = new List<Endereco> {
                            PopularEndereco(cargaPedido.Pedido.Destinatario.Localidade, cargaPedido.Pedido.Destinatario.Endereco, cargaPedido.Pedido.Destinatario.Numero, cargaPedido.Pedido.Destinatario.Complemento, cargaPedido.Pedido.Destinatario.CEP)
                        },
                        Telefone = cargaPedido.Pedido.Destinatario.Telefone1
                    },
                    Observacao = observacao
                });
            }
            return rotas;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Rota> PopularRotasOrigens(Dominio.Entidades.Embarcador.Cargas.Carga carga, ref int sequencia)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Rota> rotas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Rota>();

            foreach (IGrouping<double, Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidoAgrupado in carga.Pedidos.GroupBy(x => x.Pedido.Remetente.CPF_CNPJ).OrderBy(x => x.FirstOrDefault().OrdemColeta).ToList())
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = cargaPedidoAgrupado.FirstOrDefault();

                sequencia++;
                rotas.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Rota
                {
                    Tipo = 0,
                    Sequencia = sequencia,
                    Entidade = new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Entidade
                    {
                        Documentos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Documento>
                        {
                            new Dominio.ObjetosDeValor.Embarcador.Integracao.Hub.EnvioDemanda.Documento
                            {
                                NumeroDocumento = cargaPedido.Pedido.Remetente.CPF_CNPJ_SemFormato,
                                Tipo = new Tipo
                                {
                                    Id = "a4b0f7e8-8bf3-48ef-a3ca-e736fb68277a"
                                }
                            }
                        },
                        PessoaJuridica = new EntidadeLegal
                        {
                            RazaoSocial = cargaPedido.Pedido.Remetente.NomeCNPJ,
                            NomeFantasia = string.IsNullOrEmpty(cargaPedido.Pedido.Remetente.NomeFantasia) ? cargaPedido.Pedido.Remetente.Nome : cargaPedido.Pedido.Remetente.NomeFantasia,
                            TelefoneEmpresa = cargaPedido.Pedido.Remetente.Telefone1
                        },
                        Enderecos = new List<Endereco> {
                            PopularEndereco(cargaPedido.Pedido.Remetente.Localidade, cargaPedido.Pedido.Remetente.Endereco, cargaPedido.Pedido.Remetente.Numero, cargaPedido.Pedido.Remetente.Complemento, cargaPedido.Pedido.Remetente.CEP)
                        },
                        Telefone = cargaPedido.Pedido.Remetente.Telefone1
                    },
                });
            }
            return rotas;
        }

        #endregion

    }
}
