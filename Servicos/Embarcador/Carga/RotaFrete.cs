using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao;
using Newtonsoft.Json;
using Servicos.Embarcador.Logistica;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class RotaFrete
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public RotaFrete(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.RotaFrete> ObterRotaFreteCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool rotaExclusivaCompraValePedagio = true)
        {
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(_unitOfWork);

            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Localidade> destinosSemDestinatario = new List<Dominio.Entidades.Localidade>();
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatariosOrdenados = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem>();

            List<Dominio.Entidades.RotaFrete> rotas = ObterRotasFreteCarga(carga, cargaPedidos, configuracaoTMS, _unitOfWork, tipoServicoMultisoftware, out remetentes, out destinatariosOrdenados, rotaExclusivaCompraValePedagio, out destinosSemDestinatario);

            if (carga.TipoDeCarga != null)
                rotas = rotas.Where(rota => rota.RotaFreteTiposCarga.Any(o => o.TipoDeCarga.Codigo == carga.TipoDeCarga.Codigo) || rota.RotaFreteTiposCarga.Count == 0).ToList();

            if (carga.Filial != null)
                rotas = rotas.Where(rota => rota.RotaFreteFiliais.Any(o => o.Filial.Codigo == carga.Filial.Codigo) || rota.RotaFreteFiliais.Count == 0).ToList();

            if (carga.TipoOperacao != null)
                rotas = rotas.Where(rota => rota.TipoOperacao == null || rota.TipoOperacao.Codigo == carga.TipoOperacao.Codigo).ToList();

            if (rotaExclusivaCompraValePedagio)
                rotas = rotas.Where(rota => !string.IsNullOrWhiteSpace(rota.CodigoIntegracaoValePedagio)).ToList();

            if (rotas.Count >= 1)
            {
                List<Dominio.Entidades.RotaFrete> rotasFiltradas = repositorioRotaFrete.BuscarRotasFreteFiltradas(rotas, carga.Empresa?.Codigo ?? 0);

                return rotasFiltradas;
            }

            return rotas;
        }

        public Dominio.Entidades.RotaFreteEmpresa ImportarShareRotaFrete(Dominio.Entidades.RotaFrete rotaFrete, Dominio.Entidades.Empresa transportador, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, decimal percentualCargas)
        {
            /*
              ATENÇÃO CARO DEV
              CASO VÁ ADICIONAR MAIS UM ITEM
              NOS PARÂMETROS DO MÉTODO,
              FAVOR CRIAR UM OBJETO DE VALOR,
              O MESMO NÃO FOI CRIADO DEVIDO AOS
              POUCOS PARÂMETROS PRESENTES,
              SE TU MEXER NESSE MÉTODO E NÃO FIZER O OBJETO DE VALOR
              O LEONARDO VAI VER.
            */

            Repositorio.RotaFreteEmpresa repositorioRotaFreteEmpresa = new Repositorio.RotaFreteEmpresa(_unitOfWork);

            Dominio.Entidades.RotaFreteEmpresa rotaFreteEmpresaConsulta = repositorioRotaFreteEmpresa.BuscarPorRotaFreteTransportadorModeloVeicularCarga(rotaFrete.Codigo, transportador.Codigo, modeloVeicularCarga.Codigo);


            if (rotaFreteEmpresaConsulta != null)
            {
                rotaFreteEmpresaConsulta.PercentualCargasDaRota = percentualCargas;

                repositorioRotaFreteEmpresa.Atualizar(rotaFreteEmpresaConsulta);

                return rotaFreteEmpresaConsulta;
            }
            else
            {
                Dominio.Entidades.RotaFreteEmpresa rotaFreteEmpresa = new Dominio.Entidades.RotaFreteEmpresa();

                rotaFreteEmpresa.Empresa = transportador;
                rotaFreteEmpresa.RotaFrete = rotaFrete;
                rotaFreteEmpresa.ModeloVeicularCarga = modeloVeicularCarga;
                rotaFreteEmpresa.PercentualCargasDaRota = percentualCargas;

                repositorioRotaFreteEmpresa.Inserir(rotaFreteEmpresa);

                return rotaFreteEmpresa;
            }
        }

        public DateTime? ObtemDataEntregaComRestricao(Dominio.Entidades.RotaFrete rotaFrete, DateTime? dataEntrega, int codigoModeloVeicularCarga = 0, int codigoTipoDeCarga = 0)
        {
            bool possuiRestricao = false;
            int tentativas = 0;

            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicular = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.RotaFreteRestricao repositorioRotaFreteRestricao = new Repositorio.RotaFreteRestricao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = repModeloVeicular.BuscarPorCodigo(codigoModeloVeicularCarga);
            Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoDeCarga = repTipoDeCarga.BuscarPorCodigo(codigoTipoDeCarga);

            List<Dominio.Entidades.RotaFreteRestricao> restricoesRotaFrete = repositorioRotaFreteRestricao.BuscarPorRotaFrete(rotaFrete.Codigo);
            if (restricoesRotaFrete != null && restricoesRotaFrete.Count > 0)
            {
                possuiRestricao = true;
                while (possuiRestricao && tentativas < 10080) //Tenta todos os minutos, até todos os dias da semana.
                {
                    tentativas++;
                    possuiRestricao = PossuiRestricaoData(rotaFrete, restricoesRotaFrete, dataEntrega.Value, modeloVeicularCarga, tipoDeCarga);
                    if (possuiRestricao) dataEntrega = dataEntrega.Value.AddMinutes(1);
                }
            }
            return possuiRestricao ? null : dataEntrega;
        }

        public static void Roteirizar(out string erro, Dominio.Entidades.RotaFrete rota, Repositorio.RotaFrete repRotaFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.RotaFrete serRotaFrete = new Servicos.Embarcador.Carga.RotaFrete(unitOfWork);
            serRotaFrete.Roteirizar(out erro, rota, repRotaFrete, tipoServicoMultisoftware, configuracao);
        }

        public void Roteirizar(out string erro, Dominio.Entidades.RotaFrete rota, Repositorio.RotaFrete repRotaFrete, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            erro = string.Empty;
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipointegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SemParar);

            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();

            Repositorio.RotaFreteLocalidade repositorioRotaFreteLocalidade = new Repositorio.RotaFreteLocalidade(_unitOfWork);

            if (rota.TipoUltimoPontoRoteirizacao == 0)
                rota.TipoUltimoPontoRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante;

            if (rota.Remetente != null && rota.Destinatarios != null)
                AtualizarLatLng(rota.Remetente, rota.Destinatarios, out erro, _unitOfWork, configuracaoIntegracao);

            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.RespostaRoteirizacao resposta = null;

            //Validar pedágios de Ida e retorno..
            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint origem = null;

            if (rota.Destinatarios != null && rota.Destinatarios.Count > 0)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto pontoPartida = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto();
                pontoPartida.Codigo = rota.Remetente?.Codigo ?? 0;
                pontoPartida.Cliente = rota.Remetente;
                pontoPartida.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta;

                origem = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint()
                {
                    Lat = pontoPartida.Latitude,
                    Lng = pontoPartida.Longitude,
                    Codigo = rota.Remetente?.Codigo ?? 0,
                    Descricao = rota.Remetente?.Descricao ?? "",
                    Informacao = rota.Remetente?.Descricao ?? "",
                    TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega // vai ser o retorno
                };

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> coletas = (from obj in rota.Coletas select Servicos.Embarcador.Carga.CargaRotaFrete.ObterClienteTipoPonto(obj, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta, false, null, 0)).ToList();

                var destinatariosOrdenados = rota.Destinatarios.OrderBy(o => o.Ordem);
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> destinatarios = (from obj in destinatariosOrdenados select Servicos.Embarcador.Carga.CargaRotaFrete.ObterClienteTipoPonto(obj.Cliente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega, false, obj.ClienteOutroEndereco, 0)).ToList();

                bool ordenar = !rota.Destinatarios.Any(o => o.Ordem > 0);
                resposta = Servicos.Embarcador.Carga.CargaRotaFrete.GerarRoteirizacao(pontoPartida, destinatarios, coletas, rota.TipoUltimoPontoRoteirizacao, configuracaoIntegracao.ServidorRouteOSM, configuracaoIntegracao.APIKeyGoogle, true, false, ordenar, _unitOfWork);
            }
            else
            {
                List<Dominio.Entidades.RotaFreteLocalidade> localidades = rota.Localidades?.ToList();
                if (localidades == null)
                    localidades = repositorioRotaFreteLocalidade.BuscarPorRotaFrete(rota.Codigo);

                bool ordenar = !localidades?.Any(o => o.Ordem > 0) ?? true;
                List<Dominio.Entidades.Localidade> localidadesDestino = localidades?.OrderBy(o => o.Ordem).ThenBy(o => o.Codigo).Select(o => o.Localidade).ToList() ?? new List<Dominio.Entidades.Localidade>();

                if (rota.Remetente != null)
                {
                    origem = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint()
                    {
                        Lat = Servicos.Embarcador.Carga.CargaRotaFrete.ParseDouble(rota.Remetente.Latitude),
                        Lng = Servicos.Embarcador.Carga.CargaRotaFrete.ParseDouble(rota.Remetente.Longitude),
                        Codigo = rota.Remetente.Codigo,
                        Descricao = rota.Remetente.Descricao,
                        Informacao = rota.Remetente.Descricao,
                        TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega // vai ser o retorno
                    };

                    resposta = Servicos.Embarcador.Carga.CargaRotaFrete.GerarRoteirizacao(rota.Remetente, localidadesDestino, rota.Coletas, rota.TipoUltimoPontoRoteirizacao, configuracaoIntegracao.ServidorRouteOSM, configuracaoIntegracao.APIKeyGoogle, true, false, ordenar, _unitOfWork);

                }
                else if (rota.LocalidadesOrigem.Count > 0)
                {
                    Dominio.Entidades.Localidade localidadeOrigem = rota.LocalidadesOrigem.FirstOrDefault();
                    resposta = Servicos.Embarcador.Carga.CargaRotaFrete.GerarRoteirizacaoOrigem(localidadeOrigem, localidadesDestino, rota.Coletas, rota.TipoUltimoPontoRoteirizacao, configuracaoIntegracao.ServidorRouteOSM, configuracaoIntegracao.APIKeyGoogle, true, false, ordenar, _unitOfWork);

                    if ((resposta.Status?.ToUpper() ?? "") == "OK")
                    {
                        origem = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint()
                        {
                            Lat = (double)localidadeOrigem.Latitude,
                            Lng = (double)localidadeOrigem.Longitude,
                            Codigo = rota.Remetente?.Codigo ?? 0,
                            Descricao = rota.Remetente?.Descricao ?? "",
                            Informacao = rota.Remetente?.Descricao ?? "",
                            TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega // vai ser o retorno
                        };
                    }
                }
                else
                {
                    erro = "Sem informações para roteirizar";
                    return;
                }
            }

            if (resposta.Status == "OK")
            {
                if (tipointegracao != null)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracas = new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();
                    List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasIda = new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();
                    List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasVolta = new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();
                    if (string.IsNullOrWhiteSpace(erro))
                    {
                        string response = "";
                        string request = "";
                        Servicos.Embarcador.Integracao.SemParar.PracasPedagio serPracasPedagio = new Servicos.Embarcador.Integracao.SemParar.PracasPedagio();
                        Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = serPracasPedagio.Autenticar(_unitOfWork, tipoServicoMultisoftware);

                        Repositorio.Embarcador.Configuracoes.IntegracaoSemParar repIntegracaoSemParar = new Repositorio.Embarcador.Configuracoes.IntegracaoSemParar(_unitOfWork);

                        Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = repIntegracaoSemParar.BuscarPrimeira();
                        if (credencial.Autenticado)
                        {
                            if (rota.ApenasObterPracasPedagio || (!string.IsNullOrEmpty(rota.PolilinhaRota) && configuracaoRoteirizacao.SempreUtilizarRotaParaBuscarPracasPedagio))
                            {
                                pracas.AddRange(serPracasPedagio.ObterPracasPedagioPorPolilinha(credencial, rota.PolilinhaRota, integracaoSemParar?.DistanciaMinimaQuadrante ?? 0, out erro, _unitOfWork, integracaoSemParar?.TipoConsultaRota ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota.MaisRapida));
                            }
                            else
                            {
                                pracasIda = serPracasPedagio.ObterPracasPedagioIda(credencial, resposta.PontoDaRota, out erro, _unitOfWork, integracaoSemParar?.TipoConsultaRota ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota.MaisRapida, out request, out response);
                                pracas.AddRange(pracasIda);

                                if (rota.TipoUltimoPontoRoteirizacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante && origem != null)
                                {
                                    pracasVolta = serPracasPedagio.ObterPracasPedagioVolta(credencial, resposta.PontoDaRota, out erro, _unitOfWork, integracaoSemParar?.TipoConsultaRota ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota.MaisRapida, out request, out response);
                                    pracas.AddRange(pracasVolta);
                                }
                            }
                        }

                        if (rota.TipoUltimoPontoRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante)
                            SetarPracasDePedagio(rota, pracas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspenso.Ida, erro, _unitOfWork);
                        else
                        {
                            if (pracasVolta?.Count > 0)
                            {
                                SetarPracasDePedagio(rota, pracasIda, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspenso.Ida, erro, _unitOfWork);
                                SetarPracasDePedagio(rota, pracasVolta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspenso.Volta, erro, _unitOfWork);
                            }
                            else if (pracasIda?.Count > 0)
                                SetarPracasDePedagio(rota, pracasIda, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspenso.Ida, erro, _unitOfWork);
                            else
                                SetarPracasDePedagio(rota, pracas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspenso.Nenhum, erro, _unitOfWork);
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(erro))
                {
                    if (!rota.ApenasObterPracasPedagio)
                    {

                        rota.Quilometros = resposta.Distancia;
                        //rota.TempoDeViagemEmHoras = resposta.TempoHoras;
                        rota.TempoDeViagemEmMinutos = (configuracaoRoteirizacao?.NaoCalcularTempoDeViagemAutomatico ?? false) ? 0 : resposta.TempoMinutos;
                        rota.PolilinhaRota = resposta.Polilinha;
                        if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            rota.ApenasObterPracasPedagio = true;
                    }

                    rota.DataRoteririzacao = DateTime.Now;
                    rota.MotivoFalhaRoteirizacao = "";

                    if (!rota.ApenasObterPracasPedagio)
                    {
                        if (new Servicos.Embarcador.Logistica.RestricaoRodagem(_unitOfWork).IsPossuiRestricaoZonaExclusaoRota(rota.PolilinhaRota))
                            rota.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.EmZonaExclusao;
                        else
                            rota.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(rota.PolilinhaRota))
                        {
                            if (new Servicos.Embarcador.Logistica.RestricaoRodagem(_unitOfWork).IsPossuiRestricaoZonaExclusaoRota(rota.PolilinhaRota))
                                rota.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.EmZonaExclusao;
                            else
                                rota.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido;
                        }
                        else
                            rota.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido;
                    }

                    rota.RotaRoteirizada = (rota.SituacaoDaRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido);

                    //if (!rota.ApenasObterPracasPedagio)
                    erro = SetarPontosPassagemComRetorno(rota, resposta.PontoDaRota);

                    repRotaFrete.Atualizar(rota);

                    SetarCargaPendentesRota(rota, _unitOfWork, tipoServicoMultisoftware, configuracao);
                }
            }
            else
                erro = resposta.Status;
        }

        #endregion

        #region Métodos Públicos Estáticos

        public static void SetarCargaPendentesRota(Dominio.Entidades.RotaFrete rota, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unidadeTrabalho);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unidadeTrabalho);

            Servicos.Embarcador.Hubs.Carga serHubCarga = new Servicos.Embarcador.Hubs.Carga();
            MontagemCarga.MontagemCarga servicoMontagemCarga = new Servicos.Embarcador.Carga.MontagemCarga.MontagemCarga(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarPorRotaEmCalculoFrete(rota.Codigo);

            for (int i = 0; i < cargas.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargas[i];

                if (!carga.ExigeNotaFiscalParaCalcularFrete && carga.SituacaoCarga != SituacaoCarga.Nova)
                    carga.SituacaoCarga = SituacaoCarga.CalculoFrete;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);

                if (!(configuracao.ExigirCargaRoteirizada || !(carga.TipoOperacao?.ExigirCargaRoteirizada ?? false)) || !rota.RotaRoteirizadaPorLocal)
                {
                    SetarRotaProntaNaCarga(ref carga, cargaPedidos, pedidoXMLNotaFiscals, rota, unidadeTrabalho, configuracao, tipoServicoMultisoftware);

                    if (carga.SituacaoCarga == SituacaoCarga.CalculoFrete && !carga.DataEnvioUltimaNFe.HasValue)
                    {
                        carga.CalculandoFrete = true;
                        carga.DataInicioCalculoFrete = DateTime.Now;

                        servicoMontagemCarga.CalcularFreteTodoCarregamento(carga);

                        carga.PendenciaEmissaoAutomatica = false;

                        if (carga.Rota.SituacaoDaRoteirizacao == SituacaoRoteirizacao.Concluido)
                        {
                            carga.PossuiPendencia = false;
                            carga.MotivoPendencia = string.Empty;
                        }

                        decimal distanciaPedidos = cargaPedidos.Sum(o => o.Pedido.Distancia);

                        if (carga.DadosSumarizados != null)
                        {
                            if (distanciaPedidos > 0m)
                                carga.DadosSumarizados.Distancia = distanciaPedidos;
                            else
                                carga.DadosSumarizados.Distancia = rota.Quilometros;
                        }

                        repCarga.Atualizar(carga);
                    }

                    serHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, unidadeTrabalho.StringConexao);
                }
                else
                {
                    if (carga.SituacaoRoteirizacaoCarga != SituacaoRoteirizacao.Concluido)
                    {
                        carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Aguardando;
                        repCarga.Atualizar(carga);
                    }
                }
            }
        }

        public static void SetarRotaCarga(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals, Dominio.Entidades.RotaFrete rotaFrete, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            carga.Rota = rotaFrete;

            new Servicos.Embarcador.Logistica.RestricaoRodagem(unitOfWork).ValidaAtualizaZonaExclusaoRota(rotaFrete);

            //carga.PracasPedagio?.Clear();
            Repositorio.CargaPracaPedagio repositorioCargaPracaPedagio = new Repositorio.CargaPracaPedagio(unitOfWork);

            repositorioCargaPracaPedagio.DeletarPorCarga(carga.Codigo);

            if (rotaFrete.SituacaoDaRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido)
                SetarRotaProntaNaCarga(ref carga, cargaPedidos, pedidoXMLNotaFiscals, rotaFrete, unitOfWork, configuracao, tipoServicoMultisoftware);
            else if (!configuracao.ExigirRotaRoteirizadaNaCarga || (carga.TipoOperacao?.NaoExigeRotaRoteirizada ?? false))
            {
                Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, configuracao, unitOfWork, tipoServicoMultisoftware);

                if (carga.SituacaoRoteirizacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido || carga.Carregamento == null)
                    carga.SituacaoRoteirizacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando;

            }
        }
        public static async Task SetarRotaCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals, Dominio.Entidades.RotaFrete rotaFrete, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            carga.Rota = rotaFrete;

            new Servicos.Embarcador.Logistica.RestricaoRodagem(unitOfWork).ValidaAtualizaZonaExclusaoRota(rotaFrete);

            Repositorio.CargaPracaPedagio repositorioCargaPracaPedagio = new Repositorio.CargaPracaPedagio(unitOfWork);

            await repositorioCargaPracaPedagio.DeletarPorCargaAsync(carga.Codigo);

            if (rotaFrete.SituacaoDaRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido)
                await SetarRotaProntaNaCargaAsync(carga, cargaPedidos, pedidoXMLNotaFiscals, rotaFrete, unitOfWork, configuracao, tipoServicoMultisoftware);
            else if (!configuracao.ExigirRotaRoteirizadaNaCarga || (carga.TipoOperacao?.NaoExigeRotaRoteirizada ?? false))
            {
                await Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFreteAsync(carga, cargaPedidos, configuracao, unitOfWork, tipoServicoMultisoftware);

                if (carga.SituacaoRoteirizacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido || carga.Carregamento == null)
                    carga.SituacaoRoteirizacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando;
            }
        }

        public string SetarPontosPassagemComRetorno(Dominio.Entidades.RotaFrete rotaFrete, string pontos)
        {
            try
            {
                SetarPontosPassagem(rotaFrete, pontos);
                return string.Empty;
            }
            catch (ServicoException ex)
            {
                return ex.Message;
            }
        }

        public void SetarPontosPassagem(Dominio.Entidades.RotaFrete rotaFrete, string pontos)
        {
            Repositorio.RotaFretePontosPassagem repRotaFretePontosPassagem = new Repositorio.RotaFretePontosPassagem(_unitOfWork);
            Repositorio.Embarcador.Logistica.PracaPedagio repPracaPedagio = new Repositorio.Embarcador.Logistica.PracaPedagio(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);

            repRotaFretePontosPassagem.DeletarPorRotaFrete(rotaFrete.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontosDaRota = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>>(pontos);

            if (pontosDaRota == null || pontosDaRota.Count <= 0)
                return;

            for (int i = 0; i < pontosDaRota.Count; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota pontoRota = pontosDaRota[i];
                Dominio.Entidades.RotaFretePontosPassagem rotaFretePontosPassagem = new Dominio.Entidades.RotaFretePontosPassagem();
                rotaFretePontosPassagem.RotaFrete = rotaFrete;

                if (pontoRota.tipoponto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem)
                {
                    rotaFretePontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem;
                    rotaFretePontosPassagem.Cliente = repCliente.BuscarPorCPFCNPJ(pontoRota.codigo);
                    rotaFretePontosPassagem.LocalDeParqueamento = pontoRota.localDeParqueamento;
                }
                else if (pontoRota.pedagio)
                {
                    rotaFretePontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Pedagio;
                    rotaFretePontosPassagem.PracaPedagio = repPracaPedagio.BuscarPorCodigo((int)pontoRota.codigo);
                }
                else if (pontoRota.fronteira)
                {
                    rotaFretePontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Fronteira;
                    if (pontoRota.codigo > 0)
                        rotaFretePontosPassagem.Cliente = repCliente.BuscarPorCPFCNPJ(pontoRota.codigo);

                    if (rotaFretePontosPassagem.Cliente == null && pontoRota.codigo_cliente > 0)
                        rotaFretePontosPassagem.Cliente = repCliente.BuscarPorCPFCNPJ(pontoRota.codigo_cliente);
                }
                else
                {
                    //todo: rever regra para incluir mais que uma coleta.
                    if (i == 0)
                        rotaFretePontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta;
                    else if (i == pontosDaRota.Count - 1)
                    {

                        if (rotaFrete.TipoUltimoPontoRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante)
                            rotaFretePontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega;
                        else
                            rotaFretePontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Retorno;

                    }
                    else
                    {
                        rotaFretePontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega;

                        if (pontoRota.tipoponto > 0)
                            rotaFretePontosPassagem.TipoPontoPassagem = pontoRota.tipoponto;
                    }

                    if (pontoRota.utilizaLocalidade)
                        rotaFretePontosPassagem.Localidade = repLocalidade.BuscarPorCodigo(Convert.ToInt32(pontoRota.codigo));
                    else if (pontoRota.codigo > 0)
                        rotaFretePontosPassagem.Cliente = repCliente.BuscarPorCPFCNPJ(pontoRota.codigo);

                    if (rotaFretePontosPassagem.Cliente == null && pontoRota.codigo_cliente > 0)
                        rotaFretePontosPassagem.Cliente = repCliente.BuscarPorCPFCNPJ(pontoRota.codigo_cliente);
                }

                if (rotaFretePontosPassagem.TipoPontoPassagem != TipoPontoPassagem.Pedagio && rotaFretePontosPassagem.TipoPontoPassagem != TipoPontoPassagem.Passagem &&
                    rotaFretePontosPassagem.Cliente == null && rotaFretePontosPassagem.ClienteOutroEndereco == null && rotaFretePontosPassagem.Localidade == null)
                    throw new ServicoException($"Local não encontrado para o ponto de passagem de ordem {i}");

                rotaFretePontosPassagem.Distancia = pontoRota.distancia;
                rotaFretePontosPassagem.Tempo = pontoRota.tempo;
                rotaFretePontosPassagem.TempoEstimadoPermanenencia = pontoRota.tempoEstimadoPermanencia;
                rotaFretePontosPassagem.Ordem = i;
                rotaFretePontosPassagem.Latitude = (decimal)pontoRota.lat;
                rotaFretePontosPassagem.Longitude = (decimal)pontoRota.lng;
                repRotaFretePontosPassagem.Inserir(rotaFretePontosPassagem);
            }
        }

        public static List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> SetarPontosPassagemCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete, string pontos, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> clientes, List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> listaClienteOutroEndereco, Repositorio.UnitOfWork unitOfWork, bool gerarCarregamentoRoteirizacao = false)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem = new List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            ExculirPontosPassagem(cargaRotaFrete, unitOfWork);

            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem reCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);
            Repositorio.Embarcador.Logistica.PracaPedagio repPracaPedagio = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            bool calcularDistanciaDireta = cargaPedidos.Exists(o => o.FormulaRateio?.ParametroRateioFormula == ParametroRateioFormula.FatorPonderacaoDistanciaPeso);

            long minutosRetorno = 0;

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontosDaRota = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>>(pontos);

            if (pontosDaRota != null && pontosDaRota.Count > 0)
            {
                for (int i = 0; i < pontosDaRota.Count; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota pontoRota = pontosDaRota[i];
                    Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem cargaRotaPontosPassagem = new Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem();
                    cargaRotaPontosPassagem.Ordem = i;
                    cargaRotaPontosPassagem.CargaRotaFrete = cargaRotaFrete;
                    cargaRotaPontosPassagem.ColetaEquipamento = pontoRota.coletaEquipamento;
                    if (pontoRota.pontopassagem)
                    {
                        cargaRotaPontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem;
                        //#62188
                        if ((carga.Rota?.SituacaoDaRoteirizacao ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Erro) == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido && (carga.Rota?.RotaRoteirizadaPorLocal ?? false) && !string.IsNullOrWhiteSpace(carga.Rota?.PolilinhaRota))
                            if ((carga.Rota?.PontoPassagemPreDefinido?.Count ?? 0) > 0)
                                if (pontoRota.codigo > 0)
                                    cargaRotaPontosPassagem.Cliente = repCliente.BuscarPorCPFCNPJ(pontoRota.codigo);
                    }
                    else if (pontoRota.pedagio)
                    {
                        cargaRotaPontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Pedagio;
                        cargaRotaPontosPassagem.PracaPedagio = repPracaPedagio.BuscarPorCodigo((int)pontoRota.codigo);
                    }
                    else if (pontoRota.fronteira)
                    {
                        cargaRotaPontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Fronteira;
                        if (pontoRota.codigo > 0)
                            cargaRotaPontosPassagem.Cliente = repCliente.BuscarPorCPFCNPJ(pontoRota.codigo);

                        if (cargaRotaPontosPassagem.Cliente == null && pontoRota.codigo_cliente > 0)
                            cargaRotaPontosPassagem.Cliente = repCliente.BuscarPorCPFCNPJ(pontoRota.codigo_cliente);
                    }
                    else if (pontoRota.tipoponto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.PostoFiscal)
                    {
                        cargaRotaPontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.PostoFiscal;
                        if (pontoRota.codigo > 0)
                            cargaRotaPontosPassagem.Cliente = repCliente.BuscarPorCPFCNPJ(pontoRota.codigo);
                    }
                    else
                    {
                        //todo: rever regra para incluir mais que uma coleta.
                        if (pontoRota.tipoponto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta)
                            cargaRotaPontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta;
                        else if (i == pontosDaRota.Count - 1)
                        {

                            if (cargaRotaFrete.TipoUltimoPontoRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante)
                                cargaRotaPontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega;
                            else
                                cargaRotaPontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Retorno;
                        }
                        else
                        {
                            cargaRotaPontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega;

                            if (pontoRota.tipoponto > 0)
                                cargaRotaPontosPassagem.TipoPontoPassagem = pontoRota.tipoponto;
                        }

                        if (!pontoRota.usarOutroEndereco)
                        {
                            if (clientes != null)
                                cargaRotaPontosPassagem.Cliente = (from obj in clientes where obj.Cliente.CPF_CNPJ == pontoRota.codigo select obj.Cliente).FirstOrDefault();
                            else
                                cargaRotaPontosPassagem.Cliente = repCliente.BuscarPorCPFCNPJ(pontoRota.codigo);

                            if (cargaRotaPontosPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta)
                            {
                                if (!gerarCarregamentoRoteirizacao)
                                    repCargaPedido.SetarOrdemColeta(carga.Codigo, cargaRotaPontosPassagem.Ordem, cargaRotaPontosPassagem.Cliente?.CPF_CNPJ ?? 0);
                                repPedidoXMLNotaFiscal.SetarOrdemColeta(carga.Codigo, cargaRotaPontosPassagem.Ordem, cargaRotaPontosPassagem.Cliente?.CPF_CNPJ ?? 0);
                            }
                            else if (cargaRotaPontosPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega)
                            {
                                if (!gerarCarregamentoRoteirizacao)
                                    repCargaPedido.SetarOrdemEntrega(carga.Codigo, cargaRotaPontosPassagem.Ordem, cargaRotaPontosPassagem.Cliente?.CPF_CNPJ ?? 0);
                                repPedidoXMLNotaFiscal.SetarOrdemEntrega(carga.Codigo, cargaRotaPontosPassagem.Ordem, cargaRotaPontosPassagem.Cliente?.CPF_CNPJ ?? 0);
                            }
                        }
                        else
                        {
                            Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEndereco = null;
                            if (listaClienteOutroEndereco != null)
                                clienteOutroEndereco = (from obj in listaClienteOutroEndereco where obj.Codigo == (int)pontoRota.codigoOutroEndereco || obj.Codigo == (int)pontoRota.codigo select obj).FirstOrDefault();
                            else
                                clienteOutroEndereco = repClienteOutroEndereco.BuscarPorCodigo((int)pontoRota.codigo);

                            if (clienteOutroEndereco != null)
                            {
                                cargaRotaPontosPassagem.Cliente = clienteOutroEndereco.Cliente;
                                cargaRotaPontosPassagem.ClienteOutroEndereco = clienteOutroEndereco;
                                if (cargaRotaPontosPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta)
                                {
                                    repCargaPedido.SetarOrdemColetaOutroendereco(carga.Codigo, cargaRotaPontosPassagem.Ordem, clienteOutroEndereco.Codigo);
                                    repPedidoXMLNotaFiscal.SetarOrdemColetaOutroendereco(carga.Codigo, cargaRotaPontosPassagem.Ordem, clienteOutroEndereco.Codigo);
                                }
                                else if (cargaRotaPontosPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega)
                                {
                                    if (!gerarCarregamentoRoteirizacao)
                                        repCargaPedido.SetarOrdemEntregaOutroEndereco(carga.Codigo, cargaRotaPontosPassagem.Ordem, clienteOutroEndereco.Codigo);
                                    repPedidoXMLNotaFiscal.SetarOrdemEntregaOutroendereco(carga.Codigo, cargaRotaPontosPassagem.Ordem, clienteOutroEndereco.Codigo);
                                }
                            }
                        }

                        //alteracao para Aurora, cliente da coleta para cargas que geram coleta nao podem ficar null;
                        if (cargaRotaPontosPassagem.Cliente == null && cargaRotaPontosPassagem.TipoPontoPassagem == TipoPontoPassagem.Coleta && (carga.TipoOperacao?.GerarControleColeta ?? false))
                            cargaRotaPontosPassagem.Cliente = cargaPedidos.Select(x => x.Pedido?.Expedidor ?? x.Pedido?.Remetente).FirstOrDefault();
                    }

                    //Vamos somar os minutos de viagem.. para salvar na carga.DataPrevisaoTerminoCarga
                    if (cargaRotaPontosPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Retorno)
                        minutosRetorno += pontoRota.tempo;

                    cargaRotaPontosPassagem.Distancia = pontoRota.distancia;
                    cargaRotaPontosPassagem.DistanciaDireta = pontoRota.distanciaDireta;

                    if (calcularDistanciaDireta && cargaRotaPontosPassagem.DistanciaDireta <= 0 && pontoRota != pontosDaRota.FirstOrDefault() && pontoRota != pontosDaRota.LastOrDefault() && cargaRotaPontosPassagem.TipoPontoPassagem == TipoPontoPassagem.Entrega)
                        throw new ServicoException($"Não foi possível obter uma distância maior que zero. Carga: {carga.Codigo}");

                    cargaRotaPontosPassagem.Tempo = pontoRota.tempo;
                    cargaRotaPontosPassagem.Latitude = (decimal)pontoRota.lat;
                    cargaRotaPontosPassagem.Longitude = (decimal)pontoRota.lng;
                    reCargaRotaFretePontosPassagem.Inserir(cargaRotaPontosPassagem);
                    pontosDePassagem.Add(cargaRotaPontosPassagem);
                }
            }

            if (minutosRetorno > 0)
                Servicos.Embarcador.Carga.Carga.AjustarDataPrevisaoTerminoCarga(carga, minutosRetorno, unitOfWork);
            //carga.DataPrevisaoTerminoCarga = (carga.DataInicioViagemPrevista ?? carga.DataInicioViagem ?? DateTime.MinValue).AddMinutes(minutos);

            return pontosDePassagem;
        }
        public static async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>> SetarPontosPassagemCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete, string pontos, List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.ClienteTipoPonto> clientes, List<Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco> listaClienteOutroEndereco, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosDePassagem = new List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
            await ExculirPontosPassagemAsync(cargaRotaFrete, unitOfWork);

            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem reCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);
            Repositorio.Embarcador.Logistica.PracaPedagio repPracaPedagio = new Repositorio.Embarcador.Logistica.PracaPedagio(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteOutroEndereco repClienteOutroEndereco = new Repositorio.Embarcador.Pessoas.ClienteOutroEndereco(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            bool calcularDistanciaDireta = cargaPedidos.Exists(o => o.FormulaRateio?.ParametroRateioFormula == ParametroRateioFormula.FatorPonderacaoDistanciaPeso);

            long minutosRetorno = 0;

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontosDaRota = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>>(pontos);

            if (pontosDaRota != null && pontosDaRota.Count > 0)
            {
                for (int i = 0; i < pontosDaRota.Count; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota pontoRota = pontosDaRota[i];
                    Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem cargaRotaPontosPassagem = new Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem();
                    cargaRotaPontosPassagem.Ordem = i;
                    cargaRotaPontosPassagem.CargaRotaFrete = cargaRotaFrete;
                    cargaRotaPontosPassagem.ColetaEquipamento = pontoRota.coletaEquipamento;
                    if (pontoRota.pontopassagem)
                    {
                        cargaRotaPontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem;
                        //#62188
                        if ((carga.Rota?.SituacaoDaRoteirizacao ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Erro) == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido && (carga.Rota?.RotaRoteirizadaPorLocal ?? false) && !string.IsNullOrWhiteSpace(carga.Rota?.PolilinhaRota))
                            if ((carga.Rota?.PontoPassagemPreDefinido?.Count ?? 0) > 0)
                                if (pontoRota.codigo > 0)
                                    cargaRotaPontosPassagem.Cliente = await repCliente.BuscarPorCPFCNPJAsync(pontoRota.codigo);
                    }
                    else if (pontoRota.pedagio)
                    {
                        cargaRotaPontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Pedagio;
                        cargaRotaPontosPassagem.PracaPedagio = await repPracaPedagio.BuscarPorCodigoAsync((int)pontoRota.codigo);
                    }
                    else if (pontoRota.fronteira)
                    {
                        cargaRotaPontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Fronteira;
                        if (pontoRota.codigo > 0)
                            cargaRotaPontosPassagem.Cliente = await repCliente.BuscarPorCPFCNPJAsync(pontoRota.codigo);

                        if (cargaRotaPontosPassagem.Cliente == null && pontoRota.codigo_cliente > 0)
                            cargaRotaPontosPassagem.Cliente = await repCliente.BuscarPorCPFCNPJAsync(pontoRota.codigo_cliente);
                    }
                    else if (pontoRota.tipoponto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.PostoFiscal)
                    {
                        cargaRotaPontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.PostoFiscal;
                        if (pontoRota.codigo > 0)
                            cargaRotaPontosPassagem.Cliente = await repCliente.BuscarPorCPFCNPJAsync(pontoRota.codigo);
                    }
                    else
                    {
                        //todo: rever regra para incluir mais que uma coleta.
                        if (pontoRota.tipoponto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta)
                            cargaRotaPontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta;
                        else if (i == pontosDaRota.Count - 1)
                        {

                            if (cargaRotaFrete.TipoUltimoPontoRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante)
                                cargaRotaPontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega;
                            else
                                cargaRotaPontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Retorno;
                        }
                        else
                        {
                            cargaRotaPontosPassagem.TipoPontoPassagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega;

                            if (pontoRota.tipoponto > 0)
                                cargaRotaPontosPassagem.TipoPontoPassagem = pontoRota.tipoponto;
                        }

                        if (!pontoRota.usarOutroEndereco)
                        {
                            if (clientes != null)
                                cargaRotaPontosPassagem.Cliente = (from obj in clientes where obj.Cliente.CPF_CNPJ == pontoRota.codigo select obj.Cliente).FirstOrDefault();
                            else
                                cargaRotaPontosPassagem.Cliente = await repCliente.BuscarPorCPFCNPJAsync(pontoRota.codigo);

                            if (cargaRotaPontosPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta)
                            {
                                await repCargaPedido.SetarOrdemColetaAsync(carga.Codigo, cargaRotaPontosPassagem.Ordem, cargaRotaPontosPassagem.Cliente?.CPF_CNPJ ?? 0);
                                await repPedidoXMLNotaFiscal.SetarOrdemColetaAsync(carga.Codigo, cargaRotaPontosPassagem.Ordem, cargaRotaPontosPassagem.Cliente?.CPF_CNPJ ?? 0);
                            }
                            else if (cargaRotaPontosPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega)
                            {
                                await repCargaPedido.SetarOrdemEntregaAsync(carga.Codigo, cargaRotaPontosPassagem.Ordem, cargaRotaPontosPassagem.Cliente?.CPF_CNPJ ?? 0);
                                await repPedidoXMLNotaFiscal.SetarOrdemEntregaAsync(carga.Codigo, cargaRotaPontosPassagem.Ordem, cargaRotaPontosPassagem.Cliente?.CPF_CNPJ ?? 0);
                            }
                        }
                        else
                        {
                            Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEndereco clienteOutroEndereco = null;
                            if (listaClienteOutroEndereco != null)
                                clienteOutroEndereco = (from obj in listaClienteOutroEndereco where obj.Codigo == (int)pontoRota.codigoOutroEndereco || obj.Codigo == (int)pontoRota.codigo select obj).FirstOrDefault();
                            else
                                clienteOutroEndereco = await repClienteOutroEndereco.BuscarPorCodigoAsync((int)pontoRota.codigo);

                            if (clienteOutroEndereco != null)
                            {
                                cargaRotaPontosPassagem.Cliente = clienteOutroEndereco.Cliente;
                                cargaRotaPontosPassagem.ClienteOutroEndereco = clienteOutroEndereco;
                                if (cargaRotaPontosPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta)
                                {
                                    await repCargaPedido.SetarOrdemColetaOutroenderecoAsync(carga.Codigo, cargaRotaPontosPassagem.Ordem, clienteOutroEndereco.Codigo);
                                    await repPedidoXMLNotaFiscal.SetarOrdemColetaOutroenderecoAsync(carga.Codigo, cargaRotaPontosPassagem.Ordem, clienteOutroEndereco.Codigo);
                                }
                                else if (cargaRotaPontosPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega)
                                {
                                    await repCargaPedido.SetarOrdemEntregaOutroEnderecoAsync(carga.Codigo, cargaRotaPontosPassagem.Ordem, clienteOutroEndereco.Codigo);
                                    await repPedidoXMLNotaFiscal.SetarOrdemEntregaOutroenderecoAsync(carga.Codigo, cargaRotaPontosPassagem.Ordem, clienteOutroEndereco.Codigo);
                                }
                            }
                        }

                        //alteracao para Aurora, cliente da coleta para cargas que geram coleta nao podem ficar null;
                        if (cargaRotaPontosPassagem.Cliente == null && cargaRotaPontosPassagem.TipoPontoPassagem == TipoPontoPassagem.Coleta && (carga.TipoOperacao?.GerarControleColeta ?? false))
                            cargaRotaPontosPassagem.Cliente = cargaPedidos.Select(x => x.Pedido?.Expedidor ?? x.Pedido?.Remetente).FirstOrDefault();
                    }

                    //Vamos somar os minutos de viagem.. para salvar na carga.DataPrevisaoTerminoCarga
                    if (cargaRotaPontosPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Retorno)
                        minutosRetorno += pontoRota.tempo;

                    cargaRotaPontosPassagem.Distancia = pontoRota.distancia;
                    cargaRotaPontosPassagem.DistanciaDireta = pontoRota.distanciaDireta;

                    if (calcularDistanciaDireta && cargaRotaPontosPassagem.DistanciaDireta <= 0 && pontoRota != pontosDaRota.FirstOrDefault() && pontoRota != pontosDaRota.LastOrDefault() && cargaRotaPontosPassagem.TipoPontoPassagem == TipoPontoPassagem.Entrega)
                        throw new ServicoException($"Não foi possível obter uma distância maior que zero. Carga: {carga.Codigo}");

                    cargaRotaPontosPassagem.Tempo = pontoRota.tempo;
                    cargaRotaPontosPassagem.Latitude = (decimal)pontoRota.lat;
                    cargaRotaPontosPassagem.Longitude = (decimal)pontoRota.lng;
                    await reCargaRotaFretePontosPassagem.InserirAsync(cargaRotaPontosPassagem);
                    pontosDePassagem.Add(cargaRotaPontosPassagem);
                }
            }

            if (minutosRetorno > 0)
                await Servicos.Embarcador.Carga.Carga.AjustarDataPrevisaoTerminoCargaAsync(carga, minutosRetorno, unitOfWork);

            return pontosDePassagem;
        }
        public static string ObterPontosPassagemCargaRotaFreteSerializada(Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            string pontosRota = "";
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem reCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem = reCargaRotaFretePontosPassagem.BuscarPorCargaRotaFrete(cargaRotaFrete.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontosDaRota = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>();
            for (int i = 0; i < pontosPassagem.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem pontoPassagem = pontosPassagem[i];
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota pontoRota = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota();

                pontoRota.descricao = pontoPassagem.Descricao;
                if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem)
                {
                    pontoRota.codigo = pontoPassagem.Codigo;
                    pontoRota.localDeParqueamento = pontoPassagem.LocalDeParqueamento;
                    pontoRota.pontopassagem = true;
                }
                else if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Pedagio)
                {
                    pontoRota.codigo = pontoPassagem.PracaPedagio.Codigo;
                    pontoRota.pontopassagem = true;
                }
                else if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Fronteira)
                {
                    pontoRota.codigo = pontoPassagem.Cliente?.CPF_CNPJ ?? 0;
                    pontoRota.fronteira = true;
                }
                else
                    pontoRota.codigo = pontoPassagem.Cliente?.CPF_CNPJ ?? 0;

                pontoRota.lat = (double)pontoPassagem.Latitude;
                pontoRota.lng = (double)pontoPassagem.Longitude;
                pontoRota.distancia = pontoPassagem.Distancia;
                pontoRota.distanciaDireta = pontoPassagem.DistanciaDireta;
                pontoRota.tempo = pontoPassagem.Tempo;
                pontoRota.tipoponto = pontoPassagem.TipoPontoPassagem;
                pontosDaRota.Add(pontoRota);
            }

            pontosRota = Newtonsoft.Json.JsonConvert.SerializeObject(pontosDaRota);

            return pontosRota;

        }
        public static async Task<string> ObterPontosPassagemCargaRotaFreteSerializadaAsync(Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            string pontosRota = "";
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem reCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagem =
               await reCargaRotaFretePontosPassagem.BuscarPorCargaRotaFreteAsync(cargaRotaFrete.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontosDaRota = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>();
            for (int i = 0; i < pontosPassagem.Count; i++)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem pontoPassagem = pontosPassagem[i];
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota pontoRota = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota();

                pontoRota.descricao = pontoPassagem.Descricao;
                if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem)
                {
                    pontoRota.codigo = pontoPassagem.Codigo;
                    pontoRota.localDeParqueamento = pontoPassagem.LocalDeParqueamento;
                    pontoRota.pontopassagem = true;
                }
                else if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Pedagio)
                {
                    pontoRota.codigo = pontoPassagem.PracaPedagio.Codigo;
                    pontoRota.pontopassagem = true;
                }
                else if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Fronteira)
                {
                    pontoRota.codigo = pontoPassagem.Cliente?.CPF_CNPJ ?? 0;
                    pontoRota.fronteira = true;
                }
                else
                    pontoRota.codigo = pontoPassagem.Cliente?.CPF_CNPJ ?? 0;

                pontoRota.lat = (double)pontoPassagem.Latitude;
                pontoRota.lng = (double)pontoPassagem.Longitude;
                pontoRota.distancia = pontoPassagem.Distancia;
                pontoRota.distanciaDireta = pontoPassagem.DistanciaDireta;
                pontoRota.tempo = pontoPassagem.Tempo;
                pontoRota.tipoponto = pontoPassagem.TipoPontoPassagem;
                pontosDaRota.Add(pontoRota);
            }

            pontosRota = Newtonsoft.Json.JsonConvert.SerializeObject(pontosDaRota);

            return pontosRota;

        }

        private static void SetarRotaProntaNaCarga(ref Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals, Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Repositorio.RotaFretePontosPassagem repRotaFretePontosPassagem = new Repositorio.RotaFretePontosPassagem(unitOfWork);
            Repositorio.RotaFretePracaPedagio repRotaFretePracaPedagio = new Repositorio.RotaFretePracaPedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.CargaPracaPedagio repCargaPracaPedagio = new Repositorio.CargaPracaPedagio(unitOfWork);
            Dominio.Entidades.RotaFrete rotaInformadaCarregamento = carga?.Carregamento?.Rota;

            if (!rotaFrete.RotaRoteirizadaPorLocal || !(configuracao.ExigirCargaRoteirizada || !(carga.TipoOperacao?.ExigirCargaRoteirizada ?? false)) || (rotaInformadaCarregamento != null && configuracao.SubstituirRoteirizacaoCarregamentoPorRoteirizacaoRotaFreteCarregamento))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(carga.Codigo);
                if (cargaRotaFrete == null)
                {
                    cargaRotaFrete = new Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete();
                    cargaRotaFrete.Carga = carga;
                }

                cargaRotaFrete.PolilinhaRota = rotaFrete.PolilinhaRota;
                cargaRotaFrete.TempoDeViagemEmMinutos = rotaFrete.TempoDeViagemEmMinutos;
                cargaRotaFrete.TipoUltimoPontoRoteirizacao = rotaFrete.TipoUltimoPontoRoteirizacao;

                if (cargaRotaFrete.Codigo > 0)
                    repCargaRotaFrete.Atualizar(cargaRotaFrete);
                else
                    repCargaRotaFrete.Inserir(cargaRotaFrete);

                carga.SituacaoRoteirizacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido;

                repCargaRotaFretePontosPassagem.DeletarPorCargaRotaFrete(cargaRotaFrete.Codigo);

                List<Dominio.Entidades.RotaFretePontosPassagem> rotaFretePontosPassagem = repRotaFretePontosPassagem.BuscarPorRotaFrete(rotaFrete.Codigo);
                List<Dominio.Entidades.RotaFretePracaPedagio> rotaFretePracasPedagio = repRotaFretePracaPedagio.BuscarPorRotaFrete(rotaFrete.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagens = new List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
                for (int i = 0; i < rotaFretePontosPassagem.Count; i++)
                {
                    Dominio.Entidades.RotaFretePontosPassagem rotaFretePontoPassagem = rotaFretePontosPassagem[i];
                    Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem cargaRotaFretePontoPassagem = new Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem();
                    cargaRotaFretePontoPassagem.CargaRotaFrete = cargaRotaFrete;
                    cargaRotaFretePontoPassagem.Cliente = rotaFretePontoPassagem.Cliente;
                    cargaRotaFretePontoPassagem.Localidade = rotaFretePontoPassagem.Localidade;
                    cargaRotaFretePontoPassagem.Distancia = rotaFretePontoPassagem.Distancia;
                    cargaRotaFretePontoPassagem.Latitude = rotaFretePontoPassagem.Latitude;
                    cargaRotaFretePontoPassagem.Longitude = rotaFretePontoPassagem.Longitude;
                    cargaRotaFretePontoPassagem.Ordem = rotaFretePontoPassagem.Ordem;
                    cargaRotaFretePontoPassagem.ClienteOutroEndereco = rotaFretePontoPassagem.ClienteOutroEndereco;
                    cargaRotaFretePontoPassagem.PracaPedagio = rotaFretePontoPassagem.PracaPedagio;
                    cargaRotaFretePontoPassagem.Tempo = rotaFretePontoPassagem.Tempo;
                    cargaRotaFretePontoPassagem.TipoPontoPassagem = rotaFretePontoPassagem.TipoPontoPassagem;
                    cargaRotaFretePontoPassagem.LocalDeParqueamento = rotaFretePontoPassagem.LocalDeParqueamento;

                    repCargaRotaFretePontosPassagem.Inserir(cargaRotaFretePontoPassagem);
                    pontosPassagens.Add(cargaRotaFretePontoPassagem);

                    if (rotaFretePontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta)
                    {
                        repCargaPedido.SetarOrdemColeta(carga.Codigo, rotaFretePontoPassagem.Ordem, rotaFretePontoPassagem.Cliente?.CPF_CNPJ ?? 0);
                        repPedidoXMLNotaFiscal.SetarOrdemColeta(carga.Codigo, rotaFretePontoPassagem.Ordem, rotaFretePontoPassagem.Cliente?.CPF_CNPJ ?? 0);
                    }
                    else if (rotaFretePontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega)
                    {
                        repCargaPedido.SetarOrdemEntrega(carga.Codigo, rotaFretePontoPassagem.Ordem, rotaFretePontoPassagem.Cliente?.CPF_CNPJ ?? 0);
                        repPedidoXMLNotaFiscal.SetarOrdemColeta(carga.Codigo, rotaFretePontoPassagem.Ordem, rotaFretePontoPassagem.Cliente?.CPF_CNPJ ?? 0);
                    }
                }

                repCargaPracaPedagio.DeletarPorCarga(carga.Codigo);
                foreach (Dominio.Entidades.RotaFretePracaPedagio praca in rotaFretePracasPedagio)
                {
                    Dominio.Entidades.CargaPracaPedagio cargaPracaPedagio = new Dominio.Entidades.CargaPracaPedagio()
                    {
                        Carga = carga,
                        PracaPedagio = praca.PracaPedagio,
                        EixosSuspenso = praca.EixosSuspenso
                    };
                    repCargaPracaPedagio.Inserir(cargaPracaPedagio);
                }

                Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, configuracao, unitOfWork, tipoServicoMultisoftware);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarCargaEntrega(carga, cargaPedidos, pedidoXMLNotaFiscals, cargaRotaFrete, pontosPassagens, true, configuracao, unitOfWork, tipoServicoMultisoftware);
                //setar a flag carga.IntegrandoValePedagio para true caso exista consultas valor pedagio
                Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.SetarCargaIntegrandoConsultaValePedagio(carga, unitOfWork);
            }
            else
            {
                if (carga.SituacaoRoteirizacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido || carga.Carregamento == null)
                    carga.SituacaoRoteirizacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando;
            }
        }
        private static async Task SetarRotaProntaNaCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals, Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Repositorio.RotaFretePontosPassagem repRotaFretePontosPassagem = new Repositorio.RotaFretePontosPassagem(unitOfWork);
            Repositorio.RotaFretePracaPedagio repRotaFretePracaPedagio = new Repositorio.RotaFretePracaPedagio(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem repCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.CargaPracaPedagio repCargaPracaPedagio = new Repositorio.CargaPracaPedagio(unitOfWork);
            Dominio.Entidades.RotaFrete rotaInformadaCarregamento = carga?.Carregamento?.Rota;

            if (!rotaFrete.RotaRoteirizadaPorLocal || !(configuracao.ExigirCargaRoteirizada || !(carga.TipoOperacao?.ExigirCargaRoteirizada ?? false)) || (rotaInformadaCarregamento != null && configuracao.SubstituirRoteirizacaoCarregamentoPorRoteirizacaoRotaFreteCarregamento))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = await repCargaRotaFrete.BuscarPorCargaAsync(carga.Codigo);
                if (cargaRotaFrete == null)
                {
                    cargaRotaFrete = new Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete();
                    cargaRotaFrete.Carga = carga;
                }

                cargaRotaFrete.PolilinhaRota = rotaFrete.PolilinhaRota;
                cargaRotaFrete.TempoDeViagemEmMinutos = rotaFrete.TempoDeViagemEmMinutos;
                cargaRotaFrete.TipoUltimoPontoRoteirizacao = rotaFrete.TipoUltimoPontoRoteirizacao;

                if (cargaRotaFrete.Codigo > 0)
                {
                    await repCargaRotaFretePontosPassagem.DeletarPorCargaRotaFreteAsync(cargaRotaFrete.Codigo);
                    await repCargaRotaFrete.AtualizarAsync(cargaRotaFrete);
                }

                else
                    await repCargaRotaFrete.InserirAsync(cargaRotaFrete);

                carga.SituacaoRoteirizacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido;

                List<Dominio.Entidades.RotaFretePontosPassagem> rotaFretePontosPassagem = await repRotaFretePontosPassagem.BuscarPorRotaFreteAsync(rotaFrete.Codigo);
                List<Dominio.Entidades.RotaFretePracaPedagio> rotaFretePracasPedagio = await repRotaFretePracaPedagio.BuscarPorRotaFreteAsync(rotaFrete.Codigo);
                List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem> pontosPassagens = new List<Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem>();
                for (int i = 0; i < rotaFretePontosPassagem.Count; i++)
                {
                    Dominio.Entidades.RotaFretePontosPassagem rotaFretePontoPassagem = rotaFretePontosPassagem[i];
                    Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem cargaRotaFretePontoPassagem = new Dominio.Entidades.Embarcador.Cargas.CargaRotaFretePontosPassagem();
                    cargaRotaFretePontoPassagem.CargaRotaFrete = cargaRotaFrete;
                    cargaRotaFretePontoPassagem.Cliente = rotaFretePontoPassagem.Cliente;
                    cargaRotaFretePontoPassagem.Localidade = rotaFretePontoPassagem.Localidade;
                    cargaRotaFretePontoPassagem.Distancia = rotaFretePontoPassagem.Distancia;
                    cargaRotaFretePontoPassagem.Latitude = rotaFretePontoPassagem.Latitude;
                    cargaRotaFretePontoPassagem.Longitude = rotaFretePontoPassagem.Longitude;
                    cargaRotaFretePontoPassagem.Ordem = rotaFretePontoPassagem.Ordem;
                    cargaRotaFretePontoPassagem.ClienteOutroEndereco = rotaFretePontoPassagem.ClienteOutroEndereco;
                    cargaRotaFretePontoPassagem.PracaPedagio = rotaFretePontoPassagem.PracaPedagio;
                    cargaRotaFretePontoPassagem.Tempo = rotaFretePontoPassagem.Tempo;
                    cargaRotaFretePontoPassagem.TipoPontoPassagem = rotaFretePontoPassagem.TipoPontoPassagem;
                    cargaRotaFretePontoPassagem.LocalDeParqueamento = rotaFretePontoPassagem.LocalDeParqueamento;

                    await repCargaRotaFretePontosPassagem.InserirAsync(cargaRotaFretePontoPassagem);
                    pontosPassagens.Add(cargaRotaFretePontoPassagem);

                    if (rotaFretePontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta)
                    {
                        await repCargaPedido.SetarOrdemColetaAsync(carga.Codigo, rotaFretePontoPassagem.Ordem, rotaFretePontoPassagem.Cliente?.CPF_CNPJ ?? 0);
                        await repPedidoXMLNotaFiscal.SetarOrdemColetaAsync(carga.Codigo, rotaFretePontoPassagem.Ordem, rotaFretePontoPassagem.Cliente?.CPF_CNPJ ?? 0);
                    }
                    else if (rotaFretePontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Entrega)
                    {
                        await repCargaPedido.SetarOrdemEntregaAsync(carga.Codigo, rotaFretePontoPassagem.Ordem, rotaFretePontoPassagem.Cliente?.CPF_CNPJ ?? 0);
                        await repPedidoXMLNotaFiscal.SetarOrdemColetaAsync(carga.Codigo, rotaFretePontoPassagem.Ordem, rotaFretePontoPassagem.Cliente?.CPF_CNPJ ?? 0);
                    }
                }

                await repCargaPracaPedagio.DeletarPorCargaAsync(carga.Codigo);
                foreach (Dominio.Entidades.RotaFretePracaPedagio praca in rotaFretePracasPedagio)
                {
                    Dominio.Entidades.CargaPracaPedagio cargaPracaPedagio = new Dominio.Entidades.CargaPracaPedagio()
                    {
                        Carga = carga,
                        PracaPedagio = praca.PracaPedagio,
                        EixosSuspenso = praca.EixosSuspenso
                    };
                    await repCargaPracaPedagio.InserirAsync(cargaPracaPedagio);
                }

                await Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFreteAsync(carga, cargaPedidos, configuracao, unitOfWork, tipoServicoMultisoftware);
                await Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarCargaEntregaAsync(carga, cargaPedidos, pedidoXMLNotaFiscals, cargaRotaFrete, pontosPassagens, true, configuracao, unitOfWork, tipoServicoMultisoftware);
                //setar a flag carga.IntegrandoValePedagio para true caso exista consultas valor pedagio
                await Servicos.Embarcador.Carga.ValePedagio.CargaValePedagioRota.SetarCargaIntegrandoConsultaValePedagioAsync(carga, unitOfWork);
            }
            else
            {
                if (carga.SituacaoRoteirizacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido || carga.Carregamento == null)
                    carga.SituacaoRoteirizacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando;
            }
        }

        public static List<Dominio.Entidades.RotaFrete> ObterRotasFreteParaMesmaOrigemDestino(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<Dominio.Entidades.Cliente> remetentes;
            List<Dominio.Entidades.Localidade> destinosSemDestinatario = new List<Dominio.Entidades.Localidade>();
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatariosOrdenados;

            return ObterRotasFreteCarga(carga, cargaPedidos, configuracaoTMS, unitOfWork, tipoServicoMultisoftware, out remetentes, out destinatariosOrdenados, false, out destinosSemDestinatario);
        }

        public async Task<List<Dominio.Entidades.RotaFrete>> ObterRotasFreteParaMesmaOrigemDestinoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            return (await ObterRotasFreteCargaAsync(carga, cargaPedidos, configuracaoTMS, tipoServicoMultisoftware, false)).RotasFrete;
        }

        public static bool PossuiVariasRotasFreteParaMesmaOrigemDestino(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            List<Dominio.Entidades.Cliente> remetentes;
            List<Dominio.Entidades.Localidade> destinosSemDestinatario = new List<Dominio.Entidades.Localidade>();
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatariosOrdenados;

            return ObterRotasFreteCarga(carga, cargaPedidos, configuracaoTMS, unitOfWork, tipoServicoMultisoftware, out remetentes, out destinatariosOrdenados, false, out destinosSemDestinatario).Count > 1;
        }

        public async Task<bool> PossuiVariasRotasFreteParaMesmaOrigemDestinoAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            return (await ObterRotasFreteCargaAsync(carga, cargaPedidos, configuracaoTMS, tipoServicoMultisoftware, false)).RotasFrete.Count > 1;
        }

        private static List<Dominio.Entidades.RotaFrete> ObterRotasFreteCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out List<Dominio.Entidades.Cliente> remetentes, out List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatariosOrdenados, bool rotaExclusivaCompraValePedagio, out List<Dominio.Entidades.Localidade> destinosSemDestinatario)
        {
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes> tiposEmissaoCTeParticipantes = cargaPedidos.Select(o => o.TipoEmissaoCTeParticipantes).Distinct().ToList();
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos> tiposEmissaoCTeDocumentos = cargaPedidos.Select(o => o.TipoRateio).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestino = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem>();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();

            destinatariosOrdenados = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem>();
            destinosSemDestinatario = new List<Dominio.Entidades.Localidade>();
            remetentes = new List<Dominio.Entidades.Cliente>();

            if (tiposEmissaoCTeDocumentos.Any(o => o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado || o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual))
            {
                var naoUtilizarColetaNaBuscaRotaFrete = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().NaoUtilizarColetaNaBuscaRotaFrete.HasValue ? Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().NaoUtilizarColetaNaBuscaRotaFrete.Value : false;
                if (naoUtilizarColetaNaBuscaRotaFrete)
                {
                    if (tiposEmissaoCTeParticipantes.Any(o => o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                        remetentes = cargaPedidos.Where(o => o.OrdemColeta == 0 && o.Expedidor != null).Select(o => o.Expedidor).Distinct().ToList();
                    else
                        remetentes = cargaPedidos.Where(o => o.OrdemColeta == 0 && o.Pedido.Remetente != null).Select(o => o.Pedido.Remetente).Distinct().ToList();
                }
                else
                {
                    if (tiposEmissaoCTeParticipantes.Any(o => o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                        remetentes = cargaPedidos.Where(o => o.Expedidor != null).Select(o => o.Expedidor).Distinct().ToList();
                    else
                        remetentes = cargaPedidos.Where(o => o.Pedido.Remetente != null).Select(o => o.Pedido.Remetente).Distinct().ToList();
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    Dominio.Entidades.Cliente destinatario = cargaPedido.Pedido.Destinatario;
                    if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedido.Recebedor != null)
                        destinatario = cargaPedido.Recebedor;

                    if (destinatario != null && !destinatariosOrdenados.Any(o => o.Cliente.CPF_CNPJ == destinatario.CPF_CNPJ))
                    {
                        destinatariosOrdenados.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem()
                        {
                            Cliente = destinatario,
                            Ordem = carga.OrdemRoteirizacaoDefinida ? cargaPedido.OrdemEntrega : 0
                        });
                    }
                }
            }
            else
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    if (cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual
                        || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado || carga.EmitirCTeComplementar)))
                    {
                        if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                        {
                            if (cargaPedido.Expedidor != null && !remetentes.Contains(cargaPedido.Expedidor))
                                remetentes.Add(cargaPedido.Expedidor);
                        }
                        else
                        {
                            if (cargaPedido.Pedido.Remetente != null && !remetentes.Contains(cargaPedido.Pedido.Remetente))
                                remetentes.Add(cargaPedido.Pedido.Remetente);
                        }

                        if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                        {
                            if (cargaPedido.Recebedor != null && !destinatariosOrdenados.Any(o => o.Cliente.CPF_CNPJ == cargaPedido.Recebedor.CPF_CNPJ))
                            {
                                destinatariosOrdenados.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem()
                                {
                                    Cliente = cargaPedido.Recebedor,
                                    Ordem = carga.OrdemRoteirizacaoDefinida ? cargaPedido.OrdemEntrega : 0
                                });
                            }
                        }
                        else
                        {
                            if (cargaPedido.Pedido.Destinatario != null && !destinatariosOrdenados.Any(o => o.Cliente.CPF_CNPJ == cargaPedido.Pedido.Destinatario.CPF_CNPJ))
                            {
                                destinatariosOrdenados.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem()
                                {
                                    Cliente = cargaPedido.Pedido.Destinatario,
                                    Ordem = carga.OrdemRoteirizacaoDefinida ? cargaPedido.OrdemEntrega : 0
                                });
                            }
                        }
                    }
                    else
                    {

                        if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                        {
                            if (cargaPedido.Expedidor != null && !remetentes.Contains(cargaPedido.Expedidor))
                                remetentes.Add(cargaPedido.Expedidor);
                        }
                        else
                        {
                            List<Dominio.Entidades.Cliente> emitentesNotasFiscais = (from obj in pedidoXMLNotaFiscals where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Destinatario).Distinct().ToList();
                            emitentesNotasFiscais.AddRange((from obj in pedidoXMLNotaFiscals where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Emitente).Distinct().ToList());

                            if (emitentesNotasFiscais.Count == 0 && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
                            {
                                emitentesNotasFiscais = (from obj in pedidoXMLNotaFiscals where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Destinatario).Distinct().ToList();
                                emitentesNotasFiscais.AddRange((from obj in pedidoXMLNotaFiscals where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Emitente).Distinct().ToList());
                            }


                            for (var i = 0; i < emitentesNotasFiscais.Count; i++)
                            {
                                Dominio.Entidades.Cliente emitenteNotaFiscal = emitentesNotasFiscais[i];

                                if (emitenteNotaFiscal != null && !remetentes.Contains(emitenteNotaFiscal))
                                    remetentes.Add(emitenteNotaFiscal);
                            }
                        }

                        if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                        {
                            if (cargaPedido.Recebedor != null && !destinatariosOrdenados.Any(o => o.Cliente.CPF_CNPJ == cargaPedido.Recebedor.CPF_CNPJ))
                            {
                                destinatariosOrdenados.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem()
                                {
                                    Cliente = cargaPedido.Recebedor,
                                    Ordem = carga.OrdemRoteirizacaoDefinida ? cargaPedido.OrdemEntrega : 0
                                });
                            }
                        }
                        else
                        {
                            List<Dominio.Entidades.Cliente> destinatariosNotasFiscais = (from obj in pedidoXMLNotaFiscals where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Emitente).Distinct().ToList();
                            destinatariosNotasFiscais.AddRange((from obj in pedidoXMLNotaFiscals where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Destinatario).Distinct().ToList());

                            if (destinatariosNotasFiscais.Count == 0 && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
                            {
                                destinatariosNotasFiscais = (from obj in pedidoXMLNotaFiscals where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Emitente).Distinct().ToList();
                                destinatariosNotasFiscais.AddRange((from obj in pedidoXMLNotaFiscals where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Destinatario).Distinct().ToList());
                            }

                            for (var i = 0; i < destinatariosNotasFiscais.Count; i++)
                            {
                                Dominio.Entidades.Cliente destinatarioNotaFiscal = destinatariosNotasFiscais[i];

                                if (destinatarioNotaFiscal != null && !destinatariosOrdenados.Any(o => o.Cliente.CPF_CNPJ == destinatarioNotaFiscal.CPF_CNPJ))
                                {
                                    destinatariosOrdenados.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem()
                                    {
                                        Cliente = destinatarioNotaFiscal,
                                        Ordem = carga.OrdemRoteirizacaoDefinida ? cargaPedido.OrdemEntrega : 0
                                    });
                                }
                            }
                        }
                    }
                }
            }

            List<Dominio.Entidades.Localidade> destinos = destinatariosOrdenados.OrderBy(o => o.Ordem).Select(o => o.Cliente.Localidade).Distinct().ToList();

            if ((carga.TipoOperacao?.ConfiguracaoControleEntrega?.RecriarControleDeEntregasAoConfirmarEnvioDocumentos ?? false) && (destinos.Count == 0 || remetentes.Count == 0))
            {
                if (remetentes.Count == 0)
                {
                    remetentes = cargaPedidos
                        .Where(c => c.Pedido.Remetente != null || c.Pedido.GrupoPessoas?.Clientes != null)
                        .Select(o => o.Pedido.Remetente != null ?
                                     o.Pedido.Remetente :
                                     o.Pedido.GrupoPessoas.Clientes.FirstOrDefault(x => x.Localidade == o.Pedido.Origem))
                        .Distinct()
                        .ToList();
                }
                if (destinos.Count == 0)
                {
                    destinosSemDestinatario = destinos = cargaPedidos
                        .Where(c => c.Pedido.Destino != null)
                        .Select(o => o.Pedido.Destino)
                        .Distinct()
                        .ToList();
                }
            }

            for (int i = 0; i < destinos.Count; i++)
                localidadesDestino.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem()
                {
                    Localidade = destinos[i],
                    Ordem = configuracaoRoteirizacao.OrdenarLocalidades ? (i + 1) : 0
                });

            List<Dominio.Entidades.Estado> estadosDestino = destinos.Select(o => o.Estado).Distinct().ToList();
            List<Dominio.Entidades.RotaFrete> rotas = new List<Dominio.Entidades.RotaFrete>();

            //se passar de 50 não faz sentido criar uma rota para isso.
            if ((destinatariosOrdenados.Count > 0 && destinatariosOrdenados.Count <= 50) && (localidadesDestino.Count > 0 && localidadesDestino.Count <= 50))
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao? ultimoPontoPorTipoOperacao = new Pedido.TipoOperacao(unitOfWork).ObterTipoUltimoPontoRoteirizacao(carga.TipoOperacao, carga.Empresa);
                rotas = repRotaFrete.BuscarPorOrigemEDestinos(carga.GrupoPessoaPrincipal, remetentes, destinatariosOrdenados.Count <= 50 ? destinatariosOrdenados : new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem>(), localidadesDestino, estadosDestino, (configuracaoTMS.ExigirRotaRoteirizadaNaCarga && !configuracaoTMS.CadastrarRotaAutomaticamente) ? ultimoPontoPorTipoOperacao : null, rotaExclusivaCompraValePedagio, carga.Veiculo, carga.TipoDeCarga);
            }
            else if (destinatariosOrdenados.Count > 0)
            {
                //buscar apenas por remetentes e estados destinos
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao? ultimoPontoPorTipoOperacao = new Pedido.TipoOperacao(unitOfWork).ObterTipoUltimoPontoRoteirizacao(carga.TipoOperacao, carga.Empresa);
                rotas = repRotaFrete.BuscarPorOrigemEDestinos(carga.GrupoPessoaPrincipal, remetentes, destinatariosOrdenados.Count <= 50 ? destinatariosOrdenados : new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem>(), new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem>(), estadosDestino, (configuracaoTMS.ExigirRotaRoteirizadaNaCarga && !configuracaoTMS.CadastrarRotaAutomaticamente) ? ultimoPontoPorTipoOperacao : null, rotaExclusivaCompraValePedagio, carga.Veiculo, carga.TipoDeCarga);
            }

            return rotas;
        }

        private async Task<(List<Dominio.Entidades.RotaFrete> RotasFrete, List<Dominio.Entidades.Cliente> Remetentes, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> DestinatariosOrdenados, List<Dominio.Entidades.Localidade> DestinosSemDestinatario)> ObterRotasFreteCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool rotaExclusivaCompraValePedagio)
        {
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = await repositorioPedidoXMLNotaFiscal.BuscarPorCargaAsync(carga.Codigo);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = await repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistroAsync();

            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes> tiposEmissaoCTeParticipantes = cargaPedidos.Select(o => o.TipoEmissaoCTeParticipantes).Distinct().ToList();
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos> tiposEmissaoCTeDocumentos = cargaPedidos.Select(o => o.TipoRateio).Distinct().ToList();

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestino = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem>();
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatariosOrdenados = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem>();
            List<Dominio.Entidades.Localidade> destinosSemDestinatario = new List<Dominio.Entidades.Localidade>();
            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();

            if (tiposEmissaoCTeDocumentos.Any(o => o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoAgrupado || o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual))
            {
                var naoUtilizarColetaNaBuscaRotaFrete = Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoAmbiente().NaoUtilizarColetaNaBuscaRotaFrete.HasValue ? Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoAmbiente().NaoUtilizarColetaNaBuscaRotaFrete.Value : false;
                if (naoUtilizarColetaNaBuscaRotaFrete)
                {
                    if (tiposEmissaoCTeParticipantes.Any(o => o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                        remetentes = cargaPedidos.Where(o => o.OrdemColeta == 0 && o.Expedidor != null).Select(o => o.Expedidor).Distinct().ToList();
                    else
                        remetentes = cargaPedidos.Where(o => o.OrdemColeta == 0 && o.Pedido.Remetente != null).Select(o => o.Pedido.Remetente).Distinct().ToList();
                }
                else
                {
                    if (tiposEmissaoCTeParticipantes.Any(o => o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                        remetentes = cargaPedidos.Where(o => o.Expedidor != null).Select(o => o.Expedidor).Distinct().ToList();
                    else
                        remetentes = cargaPedidos.Where(o => o.Pedido.Remetente != null).Select(o => o.Pedido.Remetente).Distinct().ToList();
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    Dominio.Entidades.Cliente destinatario = cargaPedido.Pedido.Destinatario;
                    if ((cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor) && cargaPedido.Recebedor != null)
                        destinatario = cargaPedido.Recebedor;

                    if (cargaPedido.Pedido.UsarOutroEnderecoDestino && !destinatariosOrdenados.Any(o => o.Cliente.CPF_CNPJ == destinatario.CPF_CNPJ))
                    {
                        destinatariosOrdenados.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem()
                        {
                            Cliente = destinatario,
                            Ordem = carga.OrdemRoteirizacaoDefinida ? cargaPedido.OrdemEntrega : 0,
                            ClienteOutroEndereco = cargaPedido.Pedido.EnderecoDestino.ClienteOutroEndereco
                        });
                    }
                    else if (destinatario != null && !destinatariosOrdenados.Any(o => o.Cliente.CPF_CNPJ == destinatario.CPF_CNPJ))
                    {
                        destinatariosOrdenados.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem()
                        {
                            Cliente = destinatario,
                            Ordem = carga.OrdemRoteirizacaoDefinida ? cargaPedido.OrdemEntrega : 0
                        });
                    }
                }
            }
            else
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
                {
                    if (cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.EmitePorPedidoIndividual
                        || (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (cargaPedido.TipoRateio == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos.NaoInformado || carga.EmitirCTeComplementar)))
                    {
                        if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                        {
                            if (cargaPedido.Expedidor != null && !remetentes.Contains(cargaPedido.Expedidor))
                                remetentes.Add(cargaPedido.Expedidor);
                        }
                        else
                        {
                            if (cargaPedido.Pedido.Remetente != null && !remetentes.Contains(cargaPedido.Pedido.Remetente))
                                remetentes.Add(cargaPedido.Pedido.Remetente);
                        }

                        if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                        {
                            if (cargaPedido.Recebedor != null && !destinatariosOrdenados.Any(o => o.Cliente.CPF_CNPJ == cargaPedido.Recebedor.CPF_CNPJ))
                            {
                                destinatariosOrdenados.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem()
                                {
                                    Cliente = cargaPedido.Recebedor,
                                    Ordem = carga.OrdemRoteirizacaoDefinida ? cargaPedido.OrdemEntrega : 0
                                });
                            }
                        }
                        else
                        {
                            if (cargaPedido.Pedido.Destinatario != null && !destinatariosOrdenados.Any(o => o.Cliente.CPF_CNPJ == cargaPedido.Pedido.Destinatario.CPF_CNPJ))
                            {
                                destinatariosOrdenados.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem()
                                {
                                    Cliente = cargaPedido.Pedido.Destinatario,
                                    Ordem = carga.OrdemRoteirizacaoDefinida ? cargaPedido.OrdemEntrega : 0
                                });
                            }
                        }
                    }
                    else
                    {

                        if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                        {
                            if (cargaPedido.Expedidor != null && !remetentes.Contains(cargaPedido.Expedidor))
                                remetentes.Add(cargaPedido.Expedidor);
                        }
                        else
                        {
                            List<Dominio.Entidades.Cliente> emitentesNotasFiscais = (from obj in pedidoXMLNotaFiscals where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Destinatario).Distinct().ToList();
                            emitentesNotasFiscais.AddRange((from obj in pedidoXMLNotaFiscals where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Emitente).Distinct().ToList());

                            if (emitentesNotasFiscais.Count == 0 && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
                            {
                                emitentesNotasFiscais = (from obj in pedidoXMLNotaFiscals where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Destinatario).Distinct().ToList();
                                emitentesNotasFiscais.AddRange((from obj in pedidoXMLNotaFiscals where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Emitente).Distinct().ToList());
                            }


                            for (var i = 0; i < emitentesNotasFiscais.Count; i++)
                            {
                                Dominio.Entidades.Cliente emitenteNotaFiscal = emitentesNotasFiscais[i];

                                if (emitenteNotaFiscal != null && !remetentes.Contains(emitenteNotaFiscal))
                                    remetentes.Add(emitenteNotaFiscal);
                            }
                        }

                        if (cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || cargaPedido.TipoEmissaoCTeParticipantes == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor)
                        {
                            if (cargaPedido.Recebedor != null && !destinatariosOrdenados.Any(o => o.Cliente.CPF_CNPJ == cargaPedido.Recebedor.CPF_CNPJ))
                            {
                                destinatariosOrdenados.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem()
                                {
                                    Cliente = cargaPedido.Recebedor,
                                    Ordem = carga.OrdemRoteirizacaoDefinida ? cargaPedido.OrdemEntrega : 0
                                });
                            }
                        }
                        else
                        {
                            List<Dominio.Entidades.Cliente> destinatariosNotasFiscais = (from obj in pedidoXMLNotaFiscals where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Emitente).Distinct().ToList();
                            destinatariosNotasFiscais.AddRange((from obj in pedidoXMLNotaFiscals where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Destinatario).Distinct().ToList());

                            if (destinatariosNotasFiscais.Count == 0 && cargaPedido.TipoContratacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContratacaoCarga.Normal)
                            {
                                destinatariosNotasFiscais = (from obj in pedidoXMLNotaFiscals where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Entrada && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Emitente).Distinct().ToList();
                                destinatariosNotasFiscais.AddRange((from obj in pedidoXMLNotaFiscals where obj.XMLNotaFiscal.TipoOperacaoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal.Saida && obj.CargaPedido.Codigo == cargaPedido.Codigo && obj.TipoNotaFiscal == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.NotaDeSubcontratacao select obj.XMLNotaFiscal.Destinatario).Distinct().ToList());
                            }

                            for (var i = 0; i < destinatariosNotasFiscais.Count; i++)
                            {
                                Dominio.Entidades.Cliente destinatarioNotaFiscal = destinatariosNotasFiscais[i];

                                if (destinatarioNotaFiscal != null && !destinatariosOrdenados.Any(o => o.Cliente.CPF_CNPJ == destinatarioNotaFiscal.CPF_CNPJ))
                                {
                                    destinatariosOrdenados.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem()
                                    {
                                        Cliente = destinatarioNotaFiscal,
                                        Ordem = carga.OrdemRoteirizacaoDefinida ? cargaPedido.OrdemEntrega : 0
                                    });
                                }
                            }
                        }
                    }
                }
            }

            List<Dominio.Entidades.Localidade> destinos = destinatariosOrdenados.OrderBy(o => o.Ordem).Select(o => o.Cliente.Localidade).Distinct().ToList();

            if ((carga.TipoOperacao?.ConfiguracaoControleEntrega?.RecriarControleDeEntregasAoConfirmarEnvioDocumentos ?? false) && (destinos.Count == 0 || remetentes.Count == 0))
            {
                if (remetentes.Count == 0)
                {
                    remetentes = cargaPedidos
                        .Where(c => c.Pedido.Remetente != null || c.Pedido.GrupoPessoas?.Clientes != null)
                        .Select(o => o.Pedido.Remetente != null ?
                                     o.Pedido.Remetente :
                                     o.Pedido.GrupoPessoas.Clientes.FirstOrDefault(x => x.Localidade == o.Pedido.Origem))
                        .Distinct()
                        .ToList();
                }
                if (destinos.Count == 0)
                {
                    destinosSemDestinatario = destinos = cargaPedidos
                        .Where(c => c.Pedido.Destino != null)
                        .Select(o => o.Pedido.Destino)
                        .Distinct()
                        .ToList();
                }
            }

            for (int i = 0; i < destinos.Count; i++)
                localidadesDestino.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem()
                {
                    Localidade = destinos[i],
                    Ordem = configuracaoRoteirizacao.OrdenarLocalidades ? (i + 1) : 0
                });

            List<Dominio.Entidades.Estado> estadosDestino = destinos.Select(o => o.Estado).Distinct().ToList();
            List<Dominio.Entidades.RotaFrete> rotas = new List<Dominio.Entidades.RotaFrete>();

            //se passar de 50 não faz sentido criar uma rota para isso.
            if ((destinatariosOrdenados.Count > 0 && destinatariosOrdenados.Count <= 50) && (localidadesDestino.Count > 0 && localidadesDestino.Count <= 50))
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao? ultimoPontoPorTipoOperacao = await new Pedido.TipoOperacao(_unitOfWork).ObterTipoUltimoPontoRoteirizacaoAsync(carga.TipoOperacao, carga.Empresa);
                rotas = repositorioRotaFrete.BuscarPorOrigemEDestinos(carga.GrupoPessoaPrincipal, remetentes, destinatariosOrdenados.Count <= 50 ? destinatariosOrdenados : new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem>(), localidadesDestino, estadosDestino, (configuracaoTMS.ExigirRotaRoteirizadaNaCarga && !configuracaoTMS.CadastrarRotaAutomaticamente) ? ultimoPontoPorTipoOperacao : null, rotaExclusivaCompraValePedagio, carga.Veiculo, carga.TipoDeCarga);
            }
            else if (destinatariosOrdenados.Count > 0)
            {
                //buscar apenas por remetentes e estados destinos
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao? ultimoPontoPorTipoOperacao = await new Pedido.TipoOperacao(_unitOfWork).ObterTipoUltimoPontoRoteirizacaoAsync(carga.TipoOperacao, carga.Empresa);
                rotas = repositorioRotaFrete.BuscarPorOrigemEDestinos(carga.GrupoPessoaPrincipal, remetentes, destinatariosOrdenados.Count <= 50 ? destinatariosOrdenados : new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem>(), new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem>(), estadosDestino, (configuracaoTMS.ExigirRotaRoteirizadaNaCarga && !configuracaoTMS.CadastrarRotaAutomaticamente) ? ultimoPontoPorTipoOperacao : null, rotaExclusivaCompraValePedagio, carga.Veiculo, carga.TipoDeCarga);
            }

            return (rotas, remetentes, destinatariosOrdenados, destinosSemDestinatario);
        }

        public static void SetarRotaFreteCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            SetarRotaFreteCarga(carga, cargaPedidos, configuracaoTMS, unitOfWork, tipoServicoMultisoftware, true);
        }

        public static void SetarRotaFreteCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool validarRotaFretePedido)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

            List<Dominio.Entidades.RotaFrete> rotas = new List<Dominio.Entidades.RotaFrete>();
            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Localidade> destinosSemDestinatario = new List<Dominio.Entidades.Localidade>();
            Dominio.Entidades.Localidade destinoRotaFrete = null;
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatariosOrdenados = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
            bool rotaEncontrada = false;
            bool recriarControleDeEntregasAoConfirmarEnvioDocumentos = (carga?.TipoOperacao?.ConfiguracaoControleEntrega?.RecriarControleDeEntregasAoConfirmarEnvioDocumentos ?? false);

            if (carga?.Carregamento?.Rota != null && configuracaoTMS.SubstituirRoteirizacaoCarregamentoPorRoteirizacaoRotaFreteCarregamento)
            {
                SetarRotaCarga(ref carga, cargaPedidos, pedidoXMLNotaFiscals, carga.Carregamento.Rota, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);
                rotaEncontrada = true;
            }

            if ((configuracaoTMS.UtilizarRotaFreteInformadoPedido || (carga?.TipoOperacao?.ConfiguracaoCarga?.UtilizarRotaFreteInformadoPedido ?? false)) && validarRotaFretePedido && !rotaEncontrada)
            {
                Dominio.Entidades.RotaFrete rotaFrete = repCargaPedido.BuscarPrimeiraRotaDoPedidoPorCarga(carga.Codigo);

                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && rotaFrete == null)
                {
                    if (cargaPedidos.Count < 100)
                    {
                        List<(int CodigoOrigem, int CodigoDestino, int CodigoTipoOperacao)> origensDestinosOperacoesPedidos = cargaPedidos
                            .Select(obj => obj.Pedido)
                            .Select(obj => ValueTuple.Create(
                                obj.Origem?.Codigo ?? 0,
                                obj.Destino?.Codigo ?? 0,
                                obj.TipoOperacao?.Codigo ?? 0
                            ))
                            .Distinct()
                            .ToList();

                        Dominio.Entidades.RotaFrete rotaFretePedido = null;

                        List<Dominio.Entidades.RotaFrete> rotasFreteCompativeis = new List<Dominio.Entidades.RotaFrete>();

                        foreach ((int CodigoOrigem, int CodigoDestino, int CodigoTipoOperacao) origemDestinoOperacao in origensDestinosOperacoesPedidos)
                        {
                            rotaFretePedido = repositorioRotaFrete.BuscarPorOrigemDestinoTipoOperacaoTransportador(origemDestinoOperacao.CodigoOrigem, origemDestinoOperacao.CodigoDestino, origemDestinoOperacao.CodigoTipoOperacao, carga.Empresa?.Codigo ?? 0);

                            if (rotaFretePedido != null)
                                rotasFreteCompativeis.Add(rotaFretePedido);

                            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCompativeis = cargaPedidos.Where(obj => obj.Pedido.Origem?.Codigo == origemDestinoOperacao.CodigoOrigem && obj.Pedido.Destino?.Codigo == origemDestinoOperacao.CodigoDestino && obj.Pedido.TipoOperacao?.Codigo == origemDestinoOperacao.CodigoTipoOperacao).Select(obj => obj.Pedido).ToList();

                            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoCompativel in pedidosCompativeis)
                            {
                                if (pedidoCompativel.RotaFrete == rotaFretePedido)
                                    continue;

                                pedidoCompativel.RotaFrete = rotaFretePedido;
                                repositorioPedido.Atualizar(pedidoCompativel);
                            }
                        }

                        if (rotasFreteCompativeis.Count == 0)
                        {
                            carga.Rota = null;
                            repCarga.Atualizar(carga);
                            return;
                        }
                    }
                }

                if (rotaFrete != null)
                {
                    SetarRotaCarga(ref carga, cargaPedidos, pedidoXMLNotaFiscals, rotaFrete, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);
                    rotaEncontrada = true;
                }
            }

            if (!rotaEncontrada)
            {
                rotas = ObterRotasFreteCarga(carga, cargaPedidos, configuracaoTMS, unitOfWork, tipoServicoMultisoftware, out remetentes, out destinatariosOrdenados, false, out destinosSemDestinatario);

                if (carga.TipoOperacao != null)
                    rotas = rotas.Where(rota => rota.TipoOperacao == null || rota.TipoOperacao.Codigo == carga.TipoOperacao.Codigo).ToList();

                if (rotas.Count >= 1)
                {
                    List<Dominio.Entidades.RotaFrete> rotasFiltradas = repositorioRotaFrete.BuscarRotasFreteFiltradas(rotas, carga.Empresa?.Codigo ?? 0);

                    Dominio.Entidades.RotaFrete rotaSelecionada = null;

                    if (carga.TipoOperacao != null)
                    {
                        rotaSelecionada = rotasFiltradas.Where(o => o.TipoOperacao != null && o.TipoOperacao.Codigo == carga.TipoOperacao.Codigo).FirstOrDefault();

                        if (rotaSelecionada == null && recriarControleDeEntregasAoConfirmarEnvioDocumentos)
                        {
                            if (destinatariosOrdenados.Count > 0)
                                destinoRotaFrete = destinatariosOrdenados[0].Cliente.Localidade;

                            if (destinoRotaFrete == null && destinosSemDestinatario.Count > 0)
                                destinoRotaFrete = destinosSemDestinatario[0];

                            if (destinoRotaFrete != null)
                                rotaSelecionada = rotasFiltradas.FirstOrDefault(o => o.Remetente?.Codigo == remetentes[0].Codigo && o.Localidades.Any(l => l.Localidade.Codigo == destinoRotaFrete.Codigo));
                        }
                    }
                    else
                        rotaSelecionada = rotasFiltradas.FirstOrDefault();

                    if (rotaSelecionada == null)
                    {
                        if (recriarControleDeEntregasAoConfirmarEnvioDocumentos && destinosSemDestinatario.Count > 0 && destinatariosOrdenados.Count == 0)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                            auditado.Texto = "";

                            string erroRoteirizacao = string.Empty;
                            rotaSelecionada = GerarRotaPorRemetentesELocalidades(remetentes, destinosSemDestinatario, configuracaoTMS.TipoDescricaoRota, unitOfWork, auditado);
                            Roteirizar(out erroRoteirizacao, rotaSelecionada, repositorioRotaFrete, tipoServicoMultisoftware, configuracaoTMS, unitOfWork);
                        }
                        else
                            rotaSelecionada = rotasFiltradas.FirstOrDefault();
                    }

                    if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && !recriarControleDeEntregasAoConfirmarEnvioDocumentos)
                    {
                        if (carga.Rota != null && rotasFiltradas.Contains(carga.Rota))
                            rotaSelecionada = (from obj in rotasFiltradas where obj.Codigo == carga.Rota.Codigo select obj).FirstOrDefault();
                    }

                    if (rotaSelecionada != null)
                    {
                        SetarRotaCarga(ref carga, cargaPedidos, pedidoXMLNotaFiscals, rotaSelecionada, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);
                        rotaEncontrada = true;
                    }
                }
                else if ((tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && configuracaoTMS.CadastrarRotaAutomaticamente) ||
                         (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (configuracaoTMS.CadastrarRotaAutomaticamente || configuracaoTMS.ExigirRotaRoteirizadaNaCarga) && !(carga.TipoOperacao?.NaoExigeRotaRoteirizada ?? false)))
                {
                    if ((remetentes.Count > 0) && (destinatariosOrdenados.Count > 0 || destinosSemDestinatario.Count > 0))
                    {
                        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                        auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                        auditado.Texto = "";

                        Dominio.Entidades.RotaFrete novaRota = null;

                        if (recriarControleDeEntregasAoConfirmarEnvioDocumentos && destinosSemDestinatario.Count > 0 && destinatariosOrdenados.Count == 0)
                        {
                            string erroRoteirizacao = string.Empty;
                            novaRota = GerarRotaPorRemetentesELocalidades(remetentes, destinosSemDestinatario, configuracaoTMS.TipoDescricaoRota, unitOfWork, auditado);
                            Roteirizar(out erroRoteirizacao, novaRota, repositorioRotaFrete, tipoServicoMultisoftware, configuracaoTMS, unitOfWork);
                        }
                        else if ((configuracaoTMS.RoteirizarPorCidade) || (carga.TipoOperacao?.RoteirizarPorLocalidade ?? false))
                            novaRota = GerarRotaPorLocalidades(remetentes.Where(o => o.Localidade != null).Select(o => o.Localidade).FirstOrDefault(), destinatariosOrdenados.OrderBy(o => o.Ordem).Select(o => o.Cliente.Localidade).Distinct().ToList(), configuracaoTMS.TipoDescricaoRota, unitOfWork, auditado);
                        else
                            novaRota = GerarRota(remetentes, destinatariosOrdenados, unitOfWork, configuracaoTMS.TipoDescricaoRota, carga, auditado);

                        if (novaRota != null)
                        {
                            SetarRotaCarga(ref carga, cargaPedidos, pedidoXMLNotaFiscals, novaRota, configuracaoTMS, unitOfWork, tipoServicoMultisoftware);
                            rotaEncontrada = true;
                        }
                    }
                }
                else
                {
                    rotaEncontrada = true;

                    if ((carga.SituacaoRoteirizacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido || carga.Rota?.SituacaoDaRoteirizacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido) &&
                             (carga.SituacaoRoteirizacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido || carga.Carregamento == null))
                        carga.SituacaoRoteirizacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando;
                }
            }

            if (rotaEncontrada)
                repCarga.Atualizar(carga);
            else
            {
                //forçar roteirizacao quando necessario gerar coleta e inciar monitoramento ao gerar carga.
                if (carga.TipoOperacao != null && carga.TipoOperacao.GerarControleColeta && (configuracaoTMS.ExigirCargaRoteirizada || carga.TipoOperacao.ExigirCargaRoteirizada) && configuracaoTMS.QuandoIniciarMonitoramento == QuandoIniciarMonitoramento.AoGerarCarga && carga.SituacaoRoteirizacaoCarga != SituacaoRoteirizacao.Concluido)
                    carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Aguardando;
            }

            if (configuracaoTMS.AlterarEmpresaEmissoraAoAjustarParticipantes && remetentes.Count > 0 && !repCargaPedido.ExisteCTeEmitidoNoEmbarcador(carga.Codigo))
            {
                Dominio.Entidades.Estado estadoEmissao = remetentes.Select(o => o.Localidade.Estado).FirstOrDefault();

                if (estadoEmissao != null)
                {
                    Dominio.Entidades.Empresa empresaEmissora = repEmpresa.BuscarEmpresaEmissoraEstado(estadoEmissao);
                    if (empresaEmissora != null)
                    {
                        carga.Empresa = empresaEmissora;
                        repCarga.Atualizar(carga);
                    }
                }
            }
        }

        public async Task SetarRotaFreteCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            await SetarRotaFreteCargaAsync(carga, cargaPedidos, configuracaoTMS, tipoServicoMultisoftware, true);
        }

        public async Task SetarRotaFreteCargaAsync(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, bool validarRotaFretePedido)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repsitorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);

            List<Dominio.Entidades.RotaFrete> rotas = new List<Dominio.Entidades.RotaFrete>();
            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Localidade> destinosSemDestinatario = new List<Dominio.Entidades.Localidade>();
            Dominio.Entidades.Localidade destinoRotaFrete = null;
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatariosOrdenados = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem>();
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();

            bool rotaEncontrada = false;
            bool recriarControleDeEntregasAoConfirmarEnvioDocumentos = (carga?.TipoOperacao?.ConfiguracaoControleEntrega?.RecriarControleDeEntregasAoConfirmarEnvioDocumentos ?? false);

            if (carga?.Carregamento?.Rota != null && configuracaoTMS.SubstituirRoteirizacaoCarregamentoPorRoteirizacaoRotaFreteCarregamento)
            {
                if (carga.Carregamento.Rota.SituacaoDaRoteirizacao == SituacaoRoteirizacao.Concluido)
                    pedidoXMLNotaFiscals = await repsitorioPedidoXMLNotaFiscal.BuscarPorCargaAsync(carga.Codigo);

                await SetarRotaCargaAsync(carga, cargaPedidos, pedidoXMLNotaFiscals, carga.Carregamento.Rota, configuracaoTMS, _unitOfWork, tipoServicoMultisoftware);
                rotaEncontrada = true;
            }

            if ((configuracaoTMS.UtilizarRotaFreteInformadoPedido || (carga?.TipoOperacao?.ConfiguracaoCarga?.UtilizarRotaFreteInformadoPedido ?? false)) && validarRotaFretePedido && !rotaEncontrada)
            {
                Dominio.Entidades.RotaFrete rotaFrete = await repositorioCargaPedido.BuscarPrimeiraRotaDoPedidoPorCargaAsync(carga.Codigo);

                if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && rotaFrete == null)
                {
                    if (cargaPedidos.Count < 100)
                    {
                        List<(int CodigoOrigem, int CodigoDestino, int CodigoTipoOperacao)> origensDestinosOperacoesPedidos = cargaPedidos
                            .Select(obj => obj.Pedido)
                            .Select(obj => ValueTuple.Create(
                                obj.Origem?.Codigo ?? 0,
                                obj.Destino?.Codigo ?? 0,
                                obj.TipoOperacao?.Codigo ?? 0
                            ))
                            .Distinct()
                            .ToList();

                        Dominio.Entidades.RotaFrete rotaFretePedido = null;

                        List<Dominio.Entidades.RotaFrete> rotasFreteCompativeis = new List<Dominio.Entidades.RotaFrete>();

                        foreach ((int CodigoOrigem, int CodigoDestino, int CodigoTipoOperacao) origemDestinoOperacao in origensDestinosOperacoesPedidos)
                        {
                            rotaFretePedido = await repositorioRotaFrete.BuscarPorOrigemDestinoTipoOperacaoTransportadorAsync(origemDestinoOperacao.CodigoOrigem, origemDestinoOperacao.CodigoDestino, origemDestinoOperacao.CodigoTipoOperacao, carga.Empresa?.Codigo ?? 0);

                            if (rotaFretePedido != null)
                                rotasFreteCompativeis.Add(rotaFretePedido);

                            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosCompativeis = cargaPedidos.Where(obj => obj.Pedido.Origem?.Codigo == origemDestinoOperacao.CodigoOrigem && obj.Pedido.Destino?.Codigo == origemDestinoOperacao.CodigoDestino && obj.Pedido.TipoOperacao?.Codigo == origemDestinoOperacao.CodigoTipoOperacao).Select(obj => obj.Pedido).ToList();

                            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedidoCompativel in pedidosCompativeis)
                            {
                                if (pedidoCompativel.RotaFrete == rotaFretePedido)
                                    continue;

                                pedidoCompativel.RotaFrete = rotaFretePedido;
                                await repositorioPedido.AtualizarAsync(pedidoCompativel);
                            }
                        }

                        if (rotasFreteCompativeis.Count == 0)
                        {
                            carga.Rota = null;
                            await repositorioCarga.AtualizarAsync(carga);
                            return;
                        }
                    }
                }

                if (rotaFrete != null)
                {
                    if (rotaFrete.SituacaoDaRoteirizacao == SituacaoRoteirizacao.Concluido)
                        pedidoXMLNotaFiscals = await repsitorioPedidoXMLNotaFiscal.BuscarPorCargaAsync(carga.Codigo);

                    await SetarRotaCargaAsync(carga, cargaPedidos, pedidoXMLNotaFiscals, rotaFrete, configuracaoTMS, _unitOfWork, tipoServicoMultisoftware);
                    rotaEncontrada = true;
                }
            }

            if (!rotaEncontrada)
            {
                (rotas, remetentes, destinatariosOrdenados, destinosSemDestinatario) = await ObterRotasFreteCargaAsync(carga, cargaPedidos, configuracaoTMS, tipoServicoMultisoftware, false);

                if (carga.TipoOperacao != null)
                    rotas = rotas.Where(rota => rota.TipoOperacao == null || rota.TipoOperacao.Codigo == carga.TipoOperacao.Codigo).ToList();

                if (rotas.Count >= 1)
                {
                    List<Dominio.Entidades.RotaFrete> rotasFiltradas = repositorioRotaFrete.BuscarRotasFreteFiltradas(rotas, carga.Empresa?.Codigo ?? 0);

                    Dominio.Entidades.RotaFrete rotaSelecionada = null;

                    if (carga.TipoOperacao != null)
                    {
                        rotaSelecionada = rotasFiltradas.FirstOrDefault(o => o.TipoOperacao != null && o.TipoOperacao.Codigo == carga.TipoOperacao.Codigo);

                        if (rotaSelecionada == null && recriarControleDeEntregasAoConfirmarEnvioDocumentos)
                        {
                            if (destinatariosOrdenados.Count > 0)
                                destinoRotaFrete = destinatariosOrdenados[0].Cliente.Localidade;

                            if (destinoRotaFrete == null && destinosSemDestinatario.Count > 0)
                                destinoRotaFrete = destinosSemDestinatario[0];

                            if (destinoRotaFrete != null)
                                rotaSelecionada = rotasFiltradas.FirstOrDefault(o => o.Remetente?.Codigo == remetentes[0].Codigo && o.Localidades.Any(l => l.Localidade.Codigo == destinoRotaFrete.Codigo));
                        }
                    }
                    else
                        rotaSelecionada = rotasFiltradas.FirstOrDefault();

                    if (rotaSelecionada == null)
                    {
                        if (recriarControleDeEntregasAoConfirmarEnvioDocumentos && destinosSemDestinatario.Count > 0 && destinatariosOrdenados.Count == 0)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                            auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                            auditado.Texto = "";

                            string erroRoteirizacao = string.Empty;
                            rotaSelecionada = await GerarRotaPorRemetentesELocalidadesAsync(remetentes, destinosSemDestinatario, configuracaoTMS.TipoDescricaoRota, _unitOfWork, auditado);
                            Roteirizar(out erroRoteirizacao, rotaSelecionada, repositorioRotaFrete, tipoServicoMultisoftware, configuracaoTMS, _unitOfWork);
                        }
                        else
                            rotaSelecionada = rotasFiltradas.FirstOrDefault();
                    }

                    if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && !recriarControleDeEntregasAoConfirmarEnvioDocumentos)
                    {
                        if (carga.Rota != null && rotasFiltradas.Contains(carga.Rota))
                            rotaSelecionada = (from obj in rotasFiltradas where obj.Codigo == carga.Rota.Codigo select obj).FirstOrDefault();
                    }

                    if (rotaSelecionada != null)
                    {
                        if (rotaSelecionada.SituacaoDaRoteirizacao == SituacaoRoteirizacao.Concluido)
                            pedidoXMLNotaFiscals = await repsitorioPedidoXMLNotaFiscal.BuscarPorCargaAsync(carga.Codigo);

                        await SetarRotaCargaAsync(carga, cargaPedidos, pedidoXMLNotaFiscals, rotaSelecionada, configuracaoTMS, _unitOfWork, tipoServicoMultisoftware);
                        rotaEncontrada = true;
                    }
                }
                else if ((tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && configuracaoTMS.CadastrarRotaAutomaticamente) ||
                         (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && (configuracaoTMS.CadastrarRotaAutomaticamente || configuracaoTMS.ExigirRotaRoteirizadaNaCarga) && !(carga.TipoOperacao?.NaoExigeRotaRoteirizada ?? false)))
                {
                    if ((remetentes.Count > 0) && (destinatariosOrdenados.Count > 0 || destinosSemDestinatario.Count > 0))
                    {
                        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                        auditado.TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema;
                        auditado.Texto = "";

                        Dominio.Entidades.RotaFrete novaRota = null;

                        if (recriarControleDeEntregasAoConfirmarEnvioDocumentos && destinosSemDestinatario.Count > 0 && destinatariosOrdenados.Count == 0)
                        {
                            string erroRoteirizacao = string.Empty;
                            novaRota = await GerarRotaPorRemetentesELocalidadesAsync(remetentes, destinosSemDestinatario, configuracaoTMS.TipoDescricaoRota, _unitOfWork, auditado);
                            Roteirizar(out erroRoteirizacao, novaRota, repositorioRotaFrete, tipoServicoMultisoftware, configuracaoTMS, _unitOfWork);
                        }
                        else if ((configuracaoTMS.RoteirizarPorCidade) || (carga.TipoOperacao?.RoteirizarPorLocalidade ?? false))
                            novaRota = await GerarRotaPorLocalidadesAsync(remetentes.Where(o => o.Localidade != null).Select(o => o.Localidade).FirstOrDefault(), destinatariosOrdenados.OrderBy(o => o.Ordem).Select(o => o.Cliente.Localidade).Distinct().ToList(), configuracaoTMS.TipoDescricaoRota, _unitOfWork, auditado);
                        else
                            novaRota = await GerarRotaAsync(remetentes, destinatariosOrdenados, _unitOfWork, configuracaoTMS.TipoDescricaoRota, carga, auditado);

                        if (novaRota != null)
                        {
                            if (novaRota.SituacaoDaRoteirizacao == SituacaoRoteirizacao.Concluido)
                                pedidoXMLNotaFiscals = await repsitorioPedidoXMLNotaFiscal.BuscarPorCargaAsync(carga.Codigo);

                            await SetarRotaCargaAsync(carga, cargaPedidos, pedidoXMLNotaFiscals, novaRota, configuracaoTMS, _unitOfWork, tipoServicoMultisoftware);
                            rotaEncontrada = true;
                        }
                    }
                }
                else
                {
                    rotaEncontrada = true;

                    if ((carga.SituacaoRoteirizacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido || carga.Rota?.SituacaoDaRoteirizacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido) &&
                             (carga.SituacaoRoteirizacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido || carga.Carregamento == null))
                        carga.SituacaoRoteirizacaoCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando;
                }
            }

            if (rotaEncontrada)
            {
                await repositorioCarga.AtualizarAsync(carga);

                if (carga.TipoOperacao != null && carga.Rota != null && !(configuracaoTMS.UtilizarRotaFreteInformadoPedido || (carga?.TipoOperacao?.ConfiguracaoCarga?.UtilizarRotaFreteInformadoPedido ?? false)))
                {
                    cargaPedidos = cargaPedidos.Where(p => p.Pedido.TipoOperacao != null && p.Pedido.TipoOperacao.Codigo == carga.TipoOperacao.Codigo).ToList();

                    if (cargaPedidos.Any())
                    {
                        foreach (var cargaPedido in cargaPedidos)
                        {
                            cargaPedido.Pedido.RotaFrete = carga.Rota;
                            await repositorioPedido.AtualizarAsync(cargaPedido.Pedido);
                        }
                    }
                }
            }
            else
            {
                //forçar roteirizacao quando necessario gerar coleta e inciar monitoramento ao gerar carga.
                if (carga.TipoOperacao != null && carga.TipoOperacao.GerarControleColeta && (configuracaoTMS.ExigirCargaRoteirizada || carga.TipoOperacao.ExigirCargaRoteirizada) && configuracaoTMS.QuandoIniciarMonitoramento == QuandoIniciarMonitoramento.AoGerarCarga && carga.SituacaoRoteirizacaoCarga != SituacaoRoteirizacao.Concluido)
                    carga.SituacaoRoteirizacaoCarga = SituacaoRoteirizacao.Aguardando;
            }

            if (configuracaoTMS.AlterarEmpresaEmissoraAoAjustarParticipantes && remetentes.Count > 0 && !repositorioCargaPedido.ExisteCTeEmitidoNoEmbarcador(carga.Codigo))
            {
                Dominio.Entidades.Estado estadoEmissao = remetentes.Select(o => o.Localidade.Estado).FirstOrDefault();

                if (estadoEmissao != null)
                {
                    Dominio.Entidades.Empresa empresaEmissora = repositorioEmpresa.BuscarEmpresaEmissoraEstado(estadoEmissao);
                    if (empresaEmissora != null)
                    {
                        carga.Empresa = empresaEmissora;
                        await repositorioCarga.AtualizarAsync(carga);
                    }
                }
            }
        }

        // NOTE: Metodo de teste do Diego para Cargas GPA
        public static void SetarRotaFreteCargaPorPedidos(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscals = repPedidoXMLNotaFiscal.BuscarPorCarga(carga.Codigo);
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes> tiposEmissaoCTeParticipantes = cargaPedidos.Select(o => o.TipoEmissaoCTeParticipantes).Distinct().ToList();
            List<Dominio.Entidades.Cliente> remetentes = new List<Dominio.Entidades.Cliente>();
            List<Dominio.Entidades.Cliente> destinatarios = new List<Dominio.Entidades.Cliente>();

            if (tiposEmissaoCTeParticipantes.Any(o => o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidor || o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                remetentes = cargaPedidos.Where(o => o.Expedidor != null).Select(o => o.Expedidor).Distinct().ToList();
            else
                remetentes = cargaPedidos.Where(o => o.Pedido.Remetente != null).Select(o => o.Pedido.Remetente).Distinct().ToList();

            if (tiposEmissaoCTeParticipantes.Any(o => o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComRecebedor || o == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeParticipantes.ComExpedidorERecebedor))
                destinatarios = cargaPedidos.Where(o => o.Recebedor != null).Select(o => o.Recebedor).Distinct().ToList();
            else
                destinatarios = cargaPedidos.Where(o => o.Pedido.Destinatario != null).Select(o => o.Pedido.Destinatario).Distinct().ToList();

            if (remetentes.Count > 0 && destinatarios.Count > 0)
            {
                List<Dominio.Entidades.Localidade> destinos = destinatarios.Select(o => o.Localidade).Distinct().ToList();
                List<Dominio.Entidades.Estado> estadosDestino = destinos.Select(o => o.Estado).Distinct().ToList();
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatariosOrdenados = (from destinatario in destinatarios select new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem() { Cliente = destinatario }).ToList();
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem> localidadesDestino = (from destino in destinos select new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaFreteLocalidadeOrdem() { Localidade = destino }).ToList();
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao? ultimoPontoPorTipoOperacao = new Pedido.TipoOperacao(unitOfWork).ObterTipoUltimoPontoRoteirizacao(carga.TipoOperacao, carga.Empresa);
                List<Dominio.Entidades.RotaFrete> rotas = repRotaFrete.BuscarPorOrigemEDestinos(carga.GrupoPessoaPrincipal, remetentes, destinatariosOrdenados, localidadesDestino, estadosDestino, ultimoPontoPorTipoOperacao, false, null, carga.TipoDeCarga);

                bool rotaEncontrada = false;

                if (rotas.Count == 1)
                {
                    SetarRotaCarga(ref carga, cargaPedidos, pedidoXMLNotaFiscals, rotas.First(), configuracao, unitOfWork, tipoServicoMultisoftware);
                    rotaEncontrada = true;
                }
                else
                {
                    List<Dominio.Entidades.RotaFrete> rotasComGrupoPessoas = rotas.Where(o => o.GrupoPessoas != null).ToList();

                    if (rotasComGrupoPessoas.Count == 1)
                    {
                        SetarRotaCarga(ref carga, cargaPedidos, pedidoXMLNotaFiscals, rotasComGrupoPessoas.First(), configuracao, unitOfWork, tipoServicoMultisoftware);
                        rotaEncontrada = true;
                    }
                }

                if (rotaEncontrada)
                    repCarga.Atualizar(carga);
            }
        }

        public static Dominio.Entidades.RotaFrete GerarRota(List<Dominio.Entidades.Cliente> remetentes, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatarios, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota tipoDescricaoRota, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.Log.TratarErro($"Iniciou GerarRota - Remetentes: {(remetentes?.Count > 0 ? string.Join(", ", remetentes.Where(x => x != null).Select(x => x.Codigo)) : "Sem remetentes")}; Destinatarios: {(destinatarios?.Count > 0 ? string.Join(", ", destinatarios.Where(x => x?.Cliente != null).Select(x => x.Cliente.Codigo)) : "Sem destinatarios")}", "GerarRota");

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();


            StringBuilder descricaoRota = new StringBuilder();

            if (tipoDescricaoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota.CodigoIntegracao)
            {
                if (remetentes?.Count > 0)
                {
                    descricaoRota.Append(string.Join(" + ", from obj in remetentes where !string.IsNullOrWhiteSpace(obj.CodigoIntegracao) select obj.CodigoIntegracao));
                    descricaoRota.Append(string.Join(" + ", from obj in remetentes where string.IsNullOrWhiteSpace(obj.CodigoIntegracao) select obj.CPF_CNPJ_SemFormato));
                }

                if (destinatarios?.Count > 0)
                {
                    descricaoRota.Append(" - ");
                    descricaoRota.Append(string.Join(" + ", from obj in destinatarios where !string.IsNullOrWhiteSpace(obj.Cliente.CodigoIntegracao) select obj.Cliente.CodigoIntegracao));
                    descricaoRota.Append(string.Join(" + ", from obj in destinatarios where string.IsNullOrWhiteSpace(obj.Cliente.CodigoIntegracao) select obj.Cliente.CPF_CNPJ_SemFormato));
                }
            }

            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Servicos.Embarcador.Pedido.TipoOperacao servicoTipoOperacao = new Servicos.Embarcador.Pedido.TipoOperacao(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao? ultimoPontoPorTipoOperacao = (carga != null) ? servicoTipoOperacao.ObterTipoUltimoPontoRoteirizacao(carga.TipoOperacao, carga.Empresa) : null;

            Dominio.Entidades.RotaFrete rotaFrete = new Dominio.Entidades.RotaFrete
            {
                Ativo = true,
                Descricao = descricaoRota.ToString().Left(99),
                Remetente = remetentes.FirstOrDefault(),
                SituacaoDaRoteirizacao = configuracao.ExigirRotaRoteirizadaNaCarga ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.SemDefinicao,
                TipoUltimoPontoRoteirizacao = ultimoPontoPorTipoOperacao ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante,
                RotaRoteirizada = false,
                TipoOperacao = ((configuracao?.ExigirRotaRoteirizadaNaCarga ?? false) && (configuracaoRoteirizacao?.CadastrarNovaRotaDeveSerParaTipoOperacaoCarga ?? false)) ? (carga?.TipoOperacao ?? null) : null
            };

            GerarRementeteRotaComoEntrega(rotaFrete, remetentes);

            repositorioRotaFrete.Inserir(rotaFrete, auditado);

            GerarDestinatariisRotaFrete(rotaFrete, destinatarios, unitOfWork);

            if (tipoDescricaoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota.CodigoRota)
            {
                rotaFrete.Descricao = rotaFrete.Codigo.ToString();
                repositorioRotaFrete.Atualizar(rotaFrete);
            }

            Servicos.Log.TratarErro($"Finalizou GerarRota - Rota frete: {rotaFrete.Codigo}", "GerarRota");

            return rotaFrete;
        }

        public async static Task<Dominio.Entidades.RotaFrete> GerarRotaAsync(List<Dominio.Entidades.Cliente> remetentes, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatarios, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota tipoDescricaoRota, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Servicos.Log.TratarErro($"Iniciou GerarRota - Remetentes: {(remetentes?.Count > 0 ? string.Join(", ", remetentes.Where(x => x != null).Select(x => x.Codigo)) : "Sem remetentes")}; Destinatarios: {(destinatarios?.Count > 0 ? string.Join(", ", destinatarios.Where(x => x?.Cliente != null).Select(x => x.Cliente.Codigo)) : "Sem destinatarios")}", "GerarRota");

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = await repositorioConfiguracao.BuscarConfiguracaoPadraoAsync();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = await repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistroAsync();


            StringBuilder descricaoRota = new StringBuilder();

            if (tipoDescricaoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota.CodigoIntegracao)
            {
                if (remetentes?.Count > 0)
                {
                    descricaoRota.Append(string.Join(" + ", from obj in remetentes where !string.IsNullOrWhiteSpace(obj.CodigoIntegracao) select obj.CodigoIntegracao));
                    descricaoRota.Append(string.Join(" + ", from obj in remetentes where string.IsNullOrWhiteSpace(obj.CodigoIntegracao) select obj.CPF_CNPJ_SemFormato));
                }

                if (destinatarios?.Count > 0)
                {
                    descricaoRota.Append(" - ");
                    descricaoRota.Append(string.Join(" + ", from obj in destinatarios where !string.IsNullOrWhiteSpace(obj.Cliente.CodigoIntegracao) select obj.Cliente.CodigoIntegracao));
                    descricaoRota.Append(string.Join(" + ", from obj in destinatarios where string.IsNullOrWhiteSpace(obj.Cliente.CodigoIntegracao) select obj.Cliente.CPF_CNPJ_SemFormato));
                }
            }

            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Servicos.Embarcador.Pedido.TipoOperacao servicoTipoOperacao = new Servicos.Embarcador.Pedido.TipoOperacao(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao? ultimoPontoPorTipoOperacao = (carga != null) ? await servicoTipoOperacao.ObterTipoUltimoPontoRoteirizacaoAsync(carga.TipoOperacao, carga.Empresa) : null;

            Dominio.Entidades.RotaFrete rotaFrete = new Dominio.Entidades.RotaFrete
            {
                Ativo = true,
                Descricao = descricaoRota.ToString().Left(99),
                Remetente = remetentes.FirstOrDefault(),
                SituacaoDaRoteirizacao = configuracao.ExigirRotaRoteirizadaNaCarga ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.SemDefinicao,
                TipoUltimoPontoRoteirizacao = ultimoPontoPorTipoOperacao ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante,
                RotaRoteirizada = false,
                TipoOperacao = ((configuracao?.ExigirRotaRoteirizadaNaCarga ?? false) && (configuracaoRoteirizacao?.CadastrarNovaRotaDeveSerParaTipoOperacaoCarga ?? false)) ? (carga?.TipoOperacao ?? null) : null
            };

            GerarRementeteRotaComoEntrega(rotaFrete, remetentes);

            await repositorioRotaFrete.InserirAsync(rotaFrete, auditado);

            GerarDestinatariisRotaFrete(rotaFrete, destinatarios, unitOfWork);

            if (tipoDescricaoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota.CodigoRota)
            {
                rotaFrete.Descricao = rotaFrete.Codigo.ToString();
                await repositorioRotaFrete.AtualizarAsync(rotaFrete);
            }

            Servicos.Log.TratarErro($"Finalizou GerarRota - Rota frete: {rotaFrete.Codigo}", "GerarRota");

            return rotaFrete;
        }

        private async static Task<Dominio.Entidades.RotaFrete> GerarRotaPorLocalidadesAsync(Dominio.Entidades.Localidade origem, List<Dominio.Entidades.Localidade> destinos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota tipoDescricaoRota, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (origem == null || destinos == null || destinos.Count <= 0)
                return null;

            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.RotaFreteLocalidade repositorioRotaFreteLocalidade = new Repositorio.RotaFreteLocalidade(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = await repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistroAsync();
            List<Dominio.Entidades.Localidade> localidadesDestino = destinos.Distinct().ToList();
            string descricaoRota = $"{origem.Descricao} até {string.Join(" e ", localidadesDestino.Select(o => o.Descricao))}";

            Dominio.Entidades.RotaFrete rotaFrete = new Dominio.Entidades.RotaFrete
            {
                Ativo = true,
                Descricao = Utilidades.String.Left(descricaoRota, 100),
                SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando,
                TipoUltimoPontoRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante,
                RotaRoteirizada = false,
                LocalidadesOrigem = new List<Dominio.Entidades.Localidade> { origem },
                RotaRoteirizadaPorLocal = true
            };

            await repositorioRotaFrete.InserirAsync(rotaFrete, auditado);

            for (int i = 0; i < localidadesDestino.Count; i++)
            {
                Dominio.Entidades.RotaFreteLocalidade rotaFreteLocalidade = new Dominio.Entidades.RotaFreteLocalidade()
                {
                    Localidade = localidadesDestino[i],
                    Ordem = configuracaoRoteirizacao.OrdenarLocalidades ? (i + 1) : 0,
                    RotaFrete = rotaFrete
                };

                await repositorioRotaFreteLocalidade.InserirAsync(rotaFreteLocalidade);
            }

            if (tipoDescricaoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota.CodigoRota)
            {
                rotaFrete.Descricao = $"{rotaFrete.Codigo}";
                await repositorioRotaFrete.AtualizarAsync(rotaFrete);
            }

            return rotaFrete;
        }
        private static Dominio.Entidades.RotaFrete GerarRotaPorLocalidades(Dominio.Entidades.Localidade origem, List<Dominio.Entidades.Localidade> destinos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota tipoDescricaoRota, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (origem == null || destinos == null || destinos.Count <= 0)
                return null;

            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.RotaFreteLocalidade repositorioRotaFreteLocalidade = new Repositorio.RotaFreteLocalidade(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();
            List<Dominio.Entidades.Localidade> localidadesDestino = destinos.Distinct().ToList();
            string descricaoRota = $"{origem.Descricao} até {string.Join(" e ", localidadesDestino.Select(o => o.Descricao))}";

            Dominio.Entidades.RotaFrete rotaFrete = new Dominio.Entidades.RotaFrete
            {
                Ativo = true,
                Descricao = Utilidades.String.Left(descricaoRota, 100),
                SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando,
                TipoUltimoPontoRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante,
                RotaRoteirizada = false,
                LocalidadesOrigem = new List<Dominio.Entidades.Localidade> { origem },
                RotaRoteirizadaPorLocal = true
            };

            repositorioRotaFrete.Inserir(rotaFrete, auditado);

            for (int i = 0; i < localidadesDestino.Count; i++)
            {
                Dominio.Entidades.RotaFreteLocalidade rotaFreteLocalidade = new Dominio.Entidades.RotaFreteLocalidade()
                {
                    Localidade = localidadesDestino[i],
                    Ordem = configuracaoRoteirizacao.OrdenarLocalidades ? (i + 1) : 0,
                    RotaFrete = rotaFrete
                };

                repositorioRotaFreteLocalidade.Inserir(rotaFreteLocalidade);
            }

            if (tipoDescricaoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota.CodigoRota)
            {
                rotaFrete.Descricao = $"{rotaFrete.Codigo}";
                repositorioRotaFrete.Atualizar(rotaFrete);
            }

            return rotaFrete;
        }

        private static Dominio.Entidades.RotaFrete GerarRotaPorRemetentesELocalidades(List<Dominio.Entidades.Cliente> remetentes, List<Dominio.Entidades.Localidade> destinos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota tipoDescricaoRota, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (remetentes == null || remetentes.Count <= 0 || destinos == null || destinos.Count <= 0)
                return null;

            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.RotaFreteLocalidade repositorioRotaFreteLocalidade = new Repositorio.RotaFreteLocalidade(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();
            List<Dominio.Entidades.Localidade> localidadesDestino = destinos.Distinct().ToList();

            StringBuilder descricaoRota = new StringBuilder();

            if (tipoDescricaoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota.CodigoIntegracao)
            {
                if (remetentes?.Count > 0)
                {
                    descricaoRota.Append(string.Join(" + ", from obj in remetentes where !string.IsNullOrWhiteSpace(obj.CodigoIntegracao) select obj.CodigoIntegracao));
                    descricaoRota.Append(string.Join(" + ", from obj in remetentes where string.IsNullOrWhiteSpace(obj.CodigoIntegracao) select obj.CPF_CNPJ_SemFormato));
                }

                if (destinos?.Count > 0)
                {
                    descricaoRota.Append(" - ");
                    descricaoRota.Append(string.Join(" + ", from obj in destinos select obj.Descricao));
                }
            }

            Dominio.Entidades.RotaFrete rotaFrete = new Dominio.Entidades.RotaFrete
            {
                Ativo = true,
                Descricao = descricaoRota.ToString().Left(99),
                Remetente = remetentes.FirstOrDefault(),
                SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando,
                TipoUltimoPontoRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante,
                RotaRoteirizada = false,
                LocalidadesOrigem = new List<Dominio.Entidades.Localidade> { remetentes[0].Localidade },
                RotaRoteirizadaPorLocal = true
            };

            GerarRementeteRotaComoEntrega(rotaFrete, remetentes);

            repositorioRotaFrete.Inserir(rotaFrete, auditado);

            for (int i = 0; i < localidadesDestino.Count; i++)
            {
                Dominio.Entidades.RotaFreteLocalidade rotaFreteLocalidade = new Dominio.Entidades.RotaFreteLocalidade()
                {
                    Localidade = localidadesDestino[i],
                    Ordem = configuracaoRoteirizacao.OrdenarLocalidades ? (i + 1) : 0,
                    RotaFrete = rotaFrete
                };

                repositorioRotaFreteLocalidade.Inserir(rotaFreteLocalidade);
            }

            if (tipoDescricaoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota.CodigoRota)
            {
                rotaFrete.Descricao = $"{rotaFrete.Codigo}";
                repositorioRotaFrete.Atualizar(rotaFrete);
            }

            return rotaFrete;
        }

        private async static Task<Dominio.Entidades.RotaFrete> GerarRotaPorRemetentesELocalidadesAsync(List<Dominio.Entidades.Cliente> remetentes, List<Dominio.Entidades.Localidade> destinos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota tipoDescricaoRota, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (remetentes == null || remetentes.Count <= 0 || destinos == null || destinos.Count <= 0)
                return null;

            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.RotaFreteLocalidade repositorioRotaFreteLocalidade = new Repositorio.RotaFreteLocalidade(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = await repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistroAsync();
            List<Dominio.Entidades.Localidade> localidadesDestino = destinos.Distinct().ToList();

            StringBuilder descricaoRota = new StringBuilder();

            if (tipoDescricaoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota.CodigoIntegracao)
            {
                if (remetentes?.Count > 0)
                {
                    descricaoRota.Append(string.Join(" + ", from obj in remetentes where !string.IsNullOrWhiteSpace(obj.CodigoIntegracao) select obj.CodigoIntegracao));
                    descricaoRota.Append(string.Join(" + ", from obj in remetentes where string.IsNullOrWhiteSpace(obj.CodigoIntegracao) select obj.CPF_CNPJ_SemFormato));
                }

                if (destinos?.Count > 0)
                {
                    descricaoRota.Append(" - ");
                    descricaoRota.Append(string.Join(" + ", from obj in destinos select obj.Descricao));
                }
            }

            Dominio.Entidades.RotaFrete rotaFrete = new Dominio.Entidades.RotaFrete
            {
                Ativo = true,
                Descricao = descricaoRota.ToString().Left(99),
                Remetente = remetentes.FirstOrDefault(),
                SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando,
                TipoUltimoPontoRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante,
                RotaRoteirizada = false,
                LocalidadesOrigem = new List<Dominio.Entidades.Localidade> { remetentes[0].Localidade },
                RotaRoteirizadaPorLocal = true
            };

            GerarRementeteRotaComoEntrega(rotaFrete, remetentes);

            await repositorioRotaFrete.InserirAsync(rotaFrete, auditado);

            for (int i = 0; i < localidadesDestino.Count; i++)
            {
                Dominio.Entidades.RotaFreteLocalidade rotaFreteLocalidade = new Dominio.Entidades.RotaFreteLocalidade()
                {
                    Localidade = localidadesDestino[i],
                    Ordem = configuracaoRoteirizacao.OrdenarLocalidades ? (i + 1) : 0,
                    RotaFrete = rotaFrete
                };

                await repositorioRotaFreteLocalidade.InserirAsync(rotaFreteLocalidade);
            }

            if (tipoDescricaoRota == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDescricaoRota.CodigoRota)
            {
                rotaFrete.Descricao = $"{rotaFrete.Codigo}";
                await repositorioRotaFrete.AtualizarAsync(rotaFrete);
            }

            return rotaFrete;
        }

        public static string ObterRotaFreteSerializada(Dominio.Entidades.RotaFrete rotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            string pontosRota = "";
            Repositorio.RotaFretePontosPassagem repRotaFretePontosPassagem = new Repositorio.RotaFretePontosPassagem(unitOfWork);
            List<Dominio.Entidades.RotaFretePontosPassagem> pontosPassagem = repRotaFretePontosPassagem.BuscarPorRotaFrete(rotaFrete.Codigo);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontosDaRota = new List<PontosDaRota>();
            foreach (Dominio.Entidades.RotaFretePontosPassagem pontoPassagem in pontosPassagem)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota pontoRota = new PontosDaRota();

                pontoRota.descricao = pontoPassagem.Descricao;
                if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem)
                {
                    pontoRota.codigo = pontoPassagem.Cliente?.CPF_CNPJ ?? 0;
                    pontoRota.descricao = pontoPassagem.Cliente?.Descricao ?? pontoPassagem.Descricao;
                    pontoRota.localDeParqueamento = pontoPassagem.LocalDeParqueamento;
                    pontoRota.pontopassagem = true;
                }
                else if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Pedagio)
                {
                    pontoRota.codigo = pontoPassagem.PracaPedagio.Codigo;
                    pontoRota.pontopassagem = true;
                }
                else if (pontoPassagem.TipoPontoPassagem == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Fronteira)
                {
                    pontoRota.codigo = pontoPassagem?.Cliente?.CPF_CNPJ ?? 0;
                    pontoRota.codigo_cliente = (double)(pontoPassagem?.Cliente?.Codigo ?? 0.0);
                    pontoRota.fronteira = true;
                }
                else
                    pontoRota.codigo = pontoPassagem.Cliente?.CPF_CNPJ ?? pontoPassagem.Localidade?.Codigo ?? 0;

                pontoRota.lat = (double)pontoPassagem.Latitude;
                pontoRota.lng = (double)pontoPassagem.Longitude;
                pontoRota.distancia = pontoPassagem.Distancia;
                pontoRota.tempo = pontoPassagem.Tempo;
                pontoRota.tempoEstimadoPermanencia = pontoPassagem.TempoEstimadoPermanenencia;
                pontoRota.tipoponto = pontoPassagem.TipoPontoPassagem;
                pontoRota.utilizaLocalidade = pontoPassagem.Localidade != null;
                pontosDaRota.Add(pontoRota);
            }
            pontosRota = Newtonsoft.Json.JsonConvert.SerializeObject(pontosDaRota);

            return pontosRota;

        }

        public static bool AdicionarRotaFrete(out string mensagemErro, out Dominio.Entidades.RotaFrete rotaFrete, Dominio.Entidades.WebService.Integradora integradora, Dominio.ObjetosDeValor.WebService.Rota.Rota rotaIntegracao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork)
        {
            rotaFrete = null;
            mensagemErro = null;

            if (rotaIntegracao == null)
            {
                mensagemErro = "A rota enviada é nula.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(rotaIntegracao.Descricao))
            {
                mensagemErro = "A descrição da rota deve ser informada.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(rotaIntegracao.Codigo))
            {
                mensagemErro = "O código da rota deve ser informado.";
                return false;
            }

            if (rotaIntegracao.Remetente == null)
            {
                mensagemErro = "O remetente deve ser informado.";
                return false;
            }

            if (rotaIntegracao.Destinatarios == null || rotaIntegracao.Destinatarios.Count() <= 0)
            {
                mensagemErro = "O destinatário deve ser informado.";
                return false;
            }

            if (rotaIntegracao.Quilometros <= 0m)
            {
                mensagemErro = "O KM da rota deve ser maior que zero.";
                return false;
            }

            if (rotaIntegracao.TempoViagemMinutos <= 0m)
            {
                mensagemErro = "O tempo de viagem deve ser maior que zero.";
                return false;
            }

            Servicos.Cliente svcCliente = new Servicos.Cliente();

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.RotaFreteDestinatarios repRotaFreteDestinatarios = new Repositorio.RotaFreteDestinatarios(unitOfWork);
            Repositorio.Embarcador.Logistica.PontoPassagemPreDefinido repPontoPassagemPreDefinido = new Repositorio.Embarcador.Logistica.PontoPassagemPreDefinido(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            double.TryParse(Utilidades.String.OnlyNumbers(rotaIntegracao.Remetente.CPFCNPJ), out double cpfCnpjRemetente);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repositorioConfiguracaoWebService.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Cliente remetente = cpfCnpjRemetente > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjRemetente) : !string.IsNullOrWhiteSpace(rotaIntegracao.Remetente.CodigoIntegracao) ? repCliente.BuscarPorCodigoIntegracao(rotaIntegracao.Remetente.CodigoIntegracao) : null;

            if (remetente == null)
            {
                rotaIntegracao.Remetente.AtualizarEnderecoPessoa = false;

                Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversao = svcCliente.ConverterObjetoValorPessoa(rotaIntegracao.Remetente, "Remetente", unitOfWork, 0, false, false, auditado);

                if (!retornoConversao.Status)
                {
                    mensagemErro = retornoConversao.Mensagem;
                    return false;
                }

                remetente = retornoConversao.cliente;
            }

            if (!string.IsNullOrWhiteSpace(rotaIntegracao.Remetente?.Latitude) && !string.IsNullOrWhiteSpace(rotaIntegracao.Remetente?.Longitude))
            {
                if (!svcCliente.SalvarLatitudeLongitude(remetente, rotaIntegracao.Remetente.Latitude, rotaIntegracao.Remetente.Longitude, auditado, unitOfWork))
                {
                    mensagemErro = "Formato inválido para as coordenadas";
                    return false;
                }
            }

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatarios = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem>();

            for (int i = 0; i < rotaIntegracao.Destinatarios.Count(); i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa destinatarioIntegracao = rotaIntegracao.Destinatarios[i];

                double.TryParse(Utilidades.String.OnlyNumbers(destinatarioIntegracao.CPFCNPJ), out double cpfCnpjDestinatario);

                Dominio.Entidades.Cliente destinatario = cpfCnpjDestinatario > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario) : !string.IsNullOrWhiteSpace(destinatarioIntegracao.CodigoIntegracao) ? repCliente.BuscarPorCodigoIntegracao(destinatarioIntegracao.CodigoIntegracao) : null;

                if (destinatario == null)
                {
                    destinatarioIntegracao.AtualizarEnderecoPessoa = false;

                    Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversao = svcCliente.ConverterObjetoValorPessoa(destinatarioIntegracao, "Destinatário", unitOfWork, 0, false, false, auditado);

                    if (!retornoConversao.Status)
                    {
                        mensagemErro = retornoConversao.Mensagem;
                        return false;
                    }

                    destinatario = retornoConversao.cliente;
                }

                if (!string.IsNullOrWhiteSpace(destinatarioIntegracao.Latitude) && !string.IsNullOrWhiteSpace(destinatarioIntegracao.Longitude))
                {
                    if (!svcCliente.SalvarLatitudeLongitude(destinatario, destinatarioIntegracao.Latitude, destinatarioIntegracao.Longitude, auditado, unitOfWork))
                    {
                        mensagemErro = "Formato inválido para as coordenadas";
                        return false;
                    }
                }

                if (!destinatarios.Any(o => o.Cliente.CPF_CNPJ == destinatario.CPF_CNPJ))
                    destinatarios.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem()
                    {
                        Cliente = destinatario,
                        Ordem = 0
                    });
            }

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaPontoPassagem> pontosPassagem = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaPontoPassagem>();
            if (rotaIntegracao.PontosPassagem?.Count > 0)
            {
                foreach (Dominio.ObjetosDeValor.WebService.Rota.RotaPontoPassagem pontoPassagem in rotaIntegracao.PontosPassagem)
                {
                    if (pontoPassagem.Cliente == null)
                    {
                        mensagemErro = "O cliente do Ponto de Passagem deve ser informado.";
                        return false;
                    }

                    if (pontoPassagem.TempoEstimadoPermanencia == 0)
                    {
                        mensagemErro = "O tempo estimado de permanência (minutos) do Ponto de Passagem deve ser informado.";
                        return false;
                    }

                    double cpfCnpjCliente = pontoPassagem.Cliente.CPFCNPJ.ObterSomenteNumeros().ToDouble();
                    Dominio.Entidades.Cliente cliente = cpfCnpjCliente > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjCliente) : !string.IsNullOrWhiteSpace(pontoPassagem.Cliente.CodigoIntegracao) ? repCliente.BuscarPorCodigoIntegracao(pontoPassagem.Cliente.CodigoIntegracao) : null;

                    if (cliente == null)
                    {
                        Dominio.ObjetosDeValor.RetornoVerificacaoCliente retornoConversao = svcCliente.ConverterObjetoValorPessoa(pontoPassagem.Cliente, "Cliente do Ponto de Passagem", unitOfWork, 0, false, false, auditado);
                        if (!retornoConversao.Status)
                        {
                            mensagemErro = retornoConversao.Mensagem;
                            return false;
                        }
                        cliente = retornoConversao.cliente;
                    }

                    if (!string.IsNullOrWhiteSpace(pontoPassagem.Cliente.Latitude) && !string.IsNullOrWhiteSpace(pontoPassagem.Cliente.Longitude))
                    {
                        if (!svcCliente.SalvarLatitudeLongitude(cliente, pontoPassagem.Cliente.Latitude, pontoPassagem.Cliente.Longitude, auditado, unitOfWork))
                        {
                            mensagemErro = "Formato inválido para as coordenadas";
                            return false;
                        }
                    }

                    if (!pontosPassagem.Any(o => o.Cliente.CPF_CNPJ == cliente.CPF_CNPJ))
                        pontosPassagem.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaPontoPassagem()
                        {
                            Cliente = cliente,
                            TempoEstimadoPermanencia = pontoPassagem.TempoEstimadoPermanencia
                        });
                }
            }

            if (integradora.TipoIntegracao != null)
                rotaFrete = repRotaFrete.BuscarPorCodigoIntegracaoETipoIntegracao(rotaIntegracao.Codigo.Trim(), integradora.TipoIntegracao.Codigo, true);
            else
                rotaFrete = repRotaFrete.BuscarPorCodigoIntegracaoEIntegradora(rotaIntegracao.Codigo.Trim(), integradora.Codigo);

            List<Dominio.Entidades.RotaFreteDestinatarios> destinatariosExistentes = new List<Dominio.Entidades.RotaFreteDestinatarios>();
            List<Dominio.Entidades.Embarcador.Logistica.PontoPassagemPreDefinido> pontosPassagemExistentes = new List<Dominio.Entidades.Embarcador.Logistica.PontoPassagemPreDefinido>();
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();

            bool mudouRota = false;

            if (rotaFrete == null)
            {
                rotaFrete = new Dominio.Entidades.RotaFrete();
                rotaFrete.Ativo = true;
                rotaFrete.AdicionadoViaIntegracao = true;

                if (integradora.TipoIntegracao != null)
                    rotaFrete.TipoIntegracao = integradora.TipoIntegracao;

                rotaFrete.Quilometros = rotaIntegracao.Quilometros;
                rotaFrete.TempoDeViagemEmMinutos = (configuracaoRoteirizacao?.NaoCalcularTempoDeViagemAutomatico ?? false) ? 0 : rotaIntegracao.TempoViagemMinutos;
                rotaFrete.TipoRota = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete.Ida;
            }
            else
            {
                destinatariosExistentes = repRotaFreteDestinatarios.BuscarPorRocartaFrete(rotaFrete.Codigo);
                pontosPassagemExistentes = repPontoPassagemPreDefinido.BuscarPorRocartaFrete(rotaFrete.Codigo);
                rotaFrete.Initialize();

                if (string.IsNullOrWhiteSpace(rotaFrete.PolilinhaRota) || !configuracaoWebService.NaoRoteirizarRotaNovamente)
                {
                    rotaFrete.Quilometros = rotaIntegracao.Quilometros;
                    rotaFrete.TempoDeViagemEmMinutos = (configuracaoRoteirizacao?.NaoCalcularTempoDeViagemAutomatico ?? false) ? 0 : rotaIntegracao.TempoViagemMinutos;
                    rotaFrete.TipoRota = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete.Ida;
                }
            }

            rotaFrete.Integradora = integradora;
            rotaFrete.CodigoIntegracao = rotaIntegracao.Codigo.Trim();
            rotaFrete.Descricao = Utilidades.String.Left(rotaIntegracao.Descricao.Trim(), 100);
            if (rotaFrete.Remetente?.CPF_CNPJ != remetente?.CPF_CNPJ)
                mudouRota = true;

            rotaFrete.Remetente = remetente;

            if (rotaIntegracao.TipoOperacao != null)
                rotaFrete.TipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao(rotaIntegracao.TipoOperacao.CodigoIntegracao);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante;

            if (rotaIntegracao.TipoUltimoPontoRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Todos)
            {
                if (rotaFrete.TipoOperacao?.TipoUltimoPontoRoteirizacao != null && rotaFrete.TipoOperacao.TipoUltimoPontoRoteirizacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Todos)
                    tipoUltimoPontoRoteirizacao = rotaFrete.TipoOperacao.TipoUltimoPontoRoteirizacao.Value;
            }
            else
                tipoUltimoPontoRoteirizacao = rotaIntegracao.TipoUltimoPontoRoteirizacao;


            if (tipoUltimoPontoRoteirizacao != rotaFrete.TipoUltimoPontoRoteirizacao)
                mudouRota = true;

            rotaFrete.TipoUltimoPontoRoteirizacao = tipoUltimoPontoRoteirizacao;

            if (rotaFrete.Destinatarios == null)
                rotaFrete.Destinatarios = new List<Dominio.Entidades.RotaFreteDestinatarios>();

            if (configuracaoTMS.ExigirRotaRoteirizadaNaCarga)
            {
                if (!string.IsNullOrWhiteSpace(rotaIntegracao.Polilinha))
                {
                    if (rotaIntegracao.Polilinha != rotaFrete.PolilinhaRota)
                    {
                        rotaFrete.PolilinhaRota = rotaIntegracao.Polilinha;
                        rotaFrete.RotaRoteirizada = false;
                        rotaFrete.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando;
                        rotaFrete.ApenasObterPracasPedagio = true;
                    }
                    else
                    {
                        if (new Servicos.Embarcador.Logistica.RestricaoRodagem(unitOfWork).IsPossuiRestricaoZonaExclusaoRota(rotaFrete.PolilinhaRota))
                        {
                            rotaFrete.RotaRoteirizada = false;
                            rotaFrete.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.EmZonaExclusao;
                        }
                        else
                        {
                            rotaFrete.RotaRoteirizada = true;
                            rotaFrete.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(rotaFrete.PolilinhaRota) && configuracaoWebService.NaoRoteirizarRotaNovamente && !mudouRota)
                    {
                        if (new Servicos.Embarcador.Logistica.RestricaoRodagem(unitOfWork).IsPossuiRestricaoZonaExclusaoRota(rotaFrete.PolilinhaRota))
                        {
                            rotaFrete.RotaRoteirizada = false;
                            rotaFrete.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.EmZonaExclusao;
                        }
                        else
                        {
                            rotaFrete.RotaRoteirizada = true;
                            rotaFrete.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Concluido;
                        }
                    }
                    else
                    {
                        rotaFrete.RotaRoteirizada = false;
                        rotaFrete.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando;
                    }
                }
            }

            if (rotaIntegracao.Ativo.HasValue)
                rotaFrete.Ativo = rotaIntegracao.Ativo.Value;

            bool atualizarRota = false;
            if (rotaFrete.Codigo > 0)
                atualizarRota = true;
            else
                repRotaFrete.Inserir(rotaFrete, auditado);

            if (destinatariosExistentes.Count > 0)
            {
                List<Dominio.Entidades.RotaFreteDestinatarios> destinatariosDeletar = (from obj in destinatariosExistentes where !destinatarios.Any(o => o.Cliente.CPF_CNPJ == obj.Cliente.CPF_CNPJ) select obj).ToList();

                if (destinatariosDeletar.Count > 0)
                    mudouRota = true;

                for (var i = 0; i < destinatariosDeletar.Count; i++)
                    repRotaFreteDestinatarios.Deletar(destinatariosDeletar[i]);
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem destinatario in destinatarios)
            {
                Dominio.Entidades.RotaFreteDestinatarios destinatarioExistente = destinatariosExistentes.Where(o => o.Cliente.CPF_CNPJ == destinatario.Cliente.CPF_CNPJ).FirstOrDefault();

                if (destinatarioExistente != null)
                    continue;

                mudouRota = true;
                destinatarioExistente = new Dominio.Entidades.RotaFreteDestinatarios()
                {
                    Cliente = destinatario.Cliente,
                    Ordem = 0,
                    RotaFrete = rotaFrete
                };

                repRotaFreteDestinatarios.Inserir(destinatarioExistente);
            }

            if (pontosPassagemExistentes.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Logistica.PontoPassagemPreDefinido> pontosPassagemDeletar = (from obj in pontosPassagemExistentes where !pontosPassagem.Any(o => o.Cliente.CPF_CNPJ == obj.Cliente.CPF_CNPJ) select obj).ToList();

                if (pontosPassagemDeletar.Count > 0)
                    mudouRota = true;

                for (int i = 0; i < pontosPassagemDeletar.Count; i++)
                    repPontoPassagemPreDefinido.Deletar(pontosPassagemDeletar[i]);
            }

            foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.RotaPontoPassagem rotaPontoPassagem in pontosPassagem)
            {
                Dominio.Entidades.Embarcador.Logistica.PontoPassagemPreDefinido pontoPassagem = pontosPassagemExistentes.Where(o => o.Cliente.CPF_CNPJ == rotaPontoPassagem.Cliente.CPF_CNPJ).FirstOrDefault();

                if (pontoPassagem == null)
                {
                    pontoPassagem = new Dominio.Entidades.Embarcador.Logistica.PontoPassagemPreDefinido()
                    {
                        Cliente = rotaPontoPassagem.Cliente,
                        RotaFrete = rotaFrete
                    };
                }

                if (pontoPassagem.Codigo > 0 && (pontoPassagem.ObterLatitude() != rotaPontoPassagem.Cliente.Latitude || pontoPassagem.ObterLongitude() != rotaPontoPassagem.Cliente.Longitude))
                    mudouRota = true;

                pontoPassagem.TempoEstimadoPermanencia = rotaPontoPassagem.TempoEstimadoPermanencia;
                pontoPassagem.Descricao = rotaPontoPassagem.Cliente.Descricao;
                pontoPassagem.Latitude = rotaPontoPassagem.Cliente.Latitude.ToDecimal();
                pontoPassagem.Longitude = rotaPontoPassagem.Cliente.Longitude.ToDecimal();

                if (pontoPassagem.Codigo > 0)
                    repPontoPassagemPreDefinido.Atualizar(pontoPassagem);
                else
                    repPontoPassagemPreDefinido.Inserir(pontoPassagem);
            }

            if (atualizarRota)
            {
                if (mudouRota)
                {
                    rotaFrete.RotaRoteirizada = false;
                    rotaFrete.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.Aguardando;
                    rotaFrete.Quilometros = rotaIntegracao.Quilometros;
                    rotaFrete.TempoDeViagemEmMinutos = (configuracaoRoteirizacao?.NaoCalcularTempoDeViagemAutomatico ?? false) ? 0 : rotaIntegracao.TempoViagemMinutos;
                    rotaFrete.TipoRota = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRotaFrete.Ida;
                    repRotaFrete.Atualizar(rotaFrete, auditado);
                }
                repRotaFrete.Atualizar(rotaFrete, auditado);
            }

            return true;
        }

        public int AtualizarRotasComCliente(Dominio.Entidades.Cliente cliente, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            int totalRotasAtualizadas = 0;

            Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(_unitOfWork);
            List<Dominio.Entidades.RotaFrete> rotaFretes = repRotaFrete.BuscarPorClienteComoRemetenteOuDestinatario(cliente);

            if (rotaFretes.Count == 0)
                return totalRotasAtualizadas;

            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao repositorioConfiguracaoRoteirizacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRoteirizacao(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();
            Roteirizacao rota = new Servicos.Embarcador.Logistica.Roteirizacao(configuracaoIntegracao.ServidorRouteOSM);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRoteirizacao configuracaoRoteirizacao = repositorioConfiguracaoRoteirizacao.BuscarPrimeiroRegistro();

            int total = rotaFretes.Count;
            for (int i = 0; i < total; i++)
            {
                rota.Clear();
                List<WayPoint> listaWayPoint = new List<WayPoint>();
                //rota.Add(new WayPoint(rotaFretes[i].Remetente.Latitude, rotaFretes[i].Remetente.Longitude));
                listaWayPoint.Add(new WayPoint(rotaFretes[i].Remetente.Latitude, rotaFretes[i].Remetente.Longitude));
                int totalD = rotaFretes[i].Destinatarios.Count;
                for (int j = 0; j < totalD; j++)
                    listaWayPoint.Add(new WayPoint(rotaFretes[i].Destinatarios.ElementAt(j).Cliente.Latitude, rotaFretes[i].Destinatarios.ElementAt(j).Cliente.Longitude));
                //rota.Add(new WayPoint(rotaFretes[i].Destinatarios.ElementAt(j).Cliente.Latitude, rotaFretes[i].Destinatarios.ElementAt(j).Cliente.Longitude));

                bool ateOrigem = rotaFretes[i].TipoUltimoPontoRoteirizacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante;
                var opcoesRoteirizacao = new OpcoesRoteirizar { AteOrigem = ateOrigem, Ordenar = true, PontosNaRota = false };
                Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa = new Servicos.Embarcador.Logistica.TrechoBalsa(_unitOfWork).TrechoBalsaRoteirizacao(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint()
                {
                    Latitude = listaWayPoint[0].Lat,
                    Longitude = listaWayPoint[0].Lng
                }, new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint()
                {
                    Latitude = listaWayPoint[listaWayPoint.Count - 1].Lat,
                    Longitude = listaWayPoint[listaWayPoint.Count - 1].Lng
                });

                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";

                WayPoint destinoFinal = listaWayPoint[listaWayPoint.Count - 1];
                if (trechoBalsa != null)
                {
                    //Removendo o ponto de destino finnal...
                    listaWayPoint.RemoveAt(listaWayPoint.Count - 1);
                    // Adicionado o porto de origem...
                    listaWayPoint.Add(new WayPoint()
                    {
                        Lat = Convert.ToDouble(trechoBalsa.PortoOrigem.Latitude.Replace(",", "."), provider),
                        Lng = Convert.ToDouble(trechoBalsa.PortoOrigem.Longitude.Replace(",", "."), provider),
                        Codigo = trechoBalsa.PortoOrigem.Codigo,
                        Descricao = trechoBalsa.PortoOrigem.Descricao,
                        Informacao = trechoBalsa.PortoOrigem.Descricao,
                        TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Balsa
                    });
                }
                rota.Add(listaWayPoint);
                RespostaRoteirizacao response = rota.Roteirizar(opcoesRoteirizacao);

                response = CargaRotaFrete.AnalisarGerarRoteirizacaoComDesvioZonaExclusao(response, string.Empty, false, _unitOfWork);

                //if (response.IsSuccess())
                //{
                //    //Validando se a polilinha possui um ponto de desvio... por violação de trecho
                //    List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> desvios = new Servicos.Embarcador.Logistica.RestricaoRodagem(unitOfWork).WayPointsDesvioRestricaoZonaExclusaoRota(response.Polilinha);
                //    if ((desvios?.Count ?? 0) > 0)
                //    {
                //        rota.Clear();
                //        rota.Add(new WayPoint(rotaFretes[i].Remetente.Latitude, rotaFretes[i].Remetente.Longitude));
                //        for (int j = 0; j < desvios.Count; j++)
                //            rota.Add(desvios[j]);

                //        totalD = rotaFretes[i].Destinatarios.Count;
                //        for (int j = 0; j < totalD; j++)
                //            rota.Add(new WayPoint(rotaFretes[i].Destinatarios.ElementAt(j).Cliente.Latitude, rotaFretes[i].Destinatarios.ElementAt(j).Cliente.Longitude));

                //        opcoesRoteirizacao.AteOrigem = false;
                //        response = rota.Roteirizar(opcoesRoteirizacao);

                //        //Se for até a origem.. vamos gerar uma nova roteirização do último ponto até a origem...  e depois gerar uma com todos os pontos sem ordenar...
                //        if (ateOrigem && trechoBalsa == null)
                //        {
                //            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> pontosNaRota = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>>(response.PontoDaRota);
                //            if (pontosNaRota.Count > 0)
                //            {
                //                rota.Clear();
                //                //Adicionando o último pontos....
                //                rota.Add(pontosNaRota[pontosNaRota.Count - 1]);
                //                //Adicionando a origem... vamos roteirizar e validar se não tem mais restrição.
                //                rota.Add(pontosNaRota[0]);

                //                response = rota.Roteirizar(opcoesRoteirizacao);

                //                //validando se possui uma nova restrição
                //                desvios = new Servicos.Embarcador.Logistica.RestricaoRodagem(unitOfWork).WayPointsDesvioRestricaoZonaExclusaoRota(response.Polilinha);
                //                if ((desvios?.Count ?? 0) > 0)
                //                {
                //                    rota.Clear();
                //                    //Adicionando o último pontos....
                //                    rota.Add(pontosNaRota[pontosNaRota.Count - 1]);
                //                    //Adicionando os desvios.
                //                    for (int j = 0; j < desvios.Count; j++)
                //                        rota.Add(desvios[j]);
                //                    //Adicionando a origem... vamos roteirizar e validar se não tem mais restrição.
                //                    rota.Add(pontosNaRota[0]);

                //                    response = rota.Roteirizar(opcoesRoteirizacao);
                //                }

                //                //Obtendo os pontos da rota do retorno...
                //                List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> pontosNaRotaRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>>(response.PontoDaRota);
                //                //Agora vamos fazer o merge do retorno.
                //                rota.Clear();
                //                rota.Add(pontosNaRota);

                //                //Vamos adicionar os pontos de retorno.. exceto a posição "0" origem que é o destino do pontosNaRota de ida...
                //                for (int j = 1; j < pontosNaRotaRetorno.Count - 1; j++)
                //                    rota.Add(pontosNaRotaRetorno[j]);

                //                opcoesRoteirizacao.Ordenar = false;
                //                response = rota.Roteirizar(opcoesRoteirizacao);
                //            }
                //        }
                //    }
                //}

                if (response.IsSuccess())
                {
                    string polilinha = response.Polilinha;
                    decimal distancia = response.Distancia;
                    string pontosDaRota = response.PontoDaRota;
                    if (trechoBalsa != null)
                    {
                        distancia += (int)(trechoBalsa.Distancia);

                        Repositorio.Embarcador.Logistica.TempoBalsa repositorioTempoBalsa = new Repositorio.Embarcador.Logistica.TempoBalsa(_unitOfWork);

                        List<Dominio.Entidades.Embarcador.Logistica.TempoBalsa> temposBalsa = repositorioTempoBalsa.BuscarPorTrechoBalsa(trechoBalsa.Codigo);
                        Dominio.Entidades.Embarcador.Logistica.TempoBalsa tempoBalsaVigente = (from obj in temposBalsa
                                                                                               where obj.DataInicio <= DateTime.Now.Date && obj.DataFinal >= DateTime.Now.Date
                                                                                               select obj).FirstOrDefault();

                        // Vamos decifrar a polilinha, adicionar o Porto de Destino, depois o Endereço de destino.. e cifrar novamente.
                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> pontos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>>(pontosDaRota);
                        // Adicionado o Porto de Destino
                        pontos.Add(new WayPoint()
                        {
                            Lat = Convert.ToDouble(trechoBalsa.PortoDestino.Latitude.Replace(",", "."), provider),
                            Lng = Convert.ToDouble(trechoBalsa.PortoDestino.Longitude.Replace(",", "."), provider),
                            Codigo = trechoBalsa.PortoDestino.Codigo,
                            Descricao = trechoBalsa.PortoDestino.Descricao,
                            Distancia = (int)trechoBalsa.Distancia,
                            Index = pontos.Count,
                            Informacao = trechoBalsa.PortoDestino.Descricao,
                            Sequencia = pontos.Count,
                            Tempo = (tempoBalsaVigente?.TempoGeral ?? 0) * 24 * 60,
                            TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Balsa
                        });
                        // Adicionado o cliente final.
                        pontos.Add(destinoFinal);
                        pontosDaRota = Newtonsoft.Json.JsonConvert.SerializeObject(pontos);

                        List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> coordenadasPolilinha = Servicos.Embarcador.Logistica.Polilinha.Decodificar(polilinha);
                        coordenadasPolilinha.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(trechoBalsa.PortoDestino.Latitude, trechoBalsa.PortoDestino.Longitude));
                        coordenadasPolilinha.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(destinoFinal.Lat, destinoFinal.Lng));
                        polilinha = Servicos.Embarcador.Logistica.Polilinha.Codificar(coordenadasPolilinha);
                    }

                    rotaFretes[i].PolilinhaRota = polilinha;
                    rotaFretes[i].Quilometros = distancia;
                    //rotaFretes[i].TempoDeViagemEmHoras = response.TempoHoras;
                    rotaFretes[i].TempoDeViagemEmMinutos = (configuracaoRoteirizacao?.NaoCalcularTempoDeViagemAutomatico ?? false) ? 0 : response.TempoMinutos;
                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        rotaFretes[i].ApenasObterPracasPedagio = true;

                    SetarPontosPassagem(rotaFretes[i], pontosDaRota);

                    repRotaFrete.Atualizar(rotaFretes[i]);
                    totalRotasAtualizadas++;
                }
            }

            return totalRotasAtualizadas;
        }

        #endregion

        #region Métodos Privados

        private static void GerarRementeteRotaComoEntrega(Dominio.Entidades.RotaFrete rotaFrete, List<Dominio.Entidades.Cliente> remetentes)
        {
            if (remetentes.Count <= 1)
                return;

            if (rotaFrete.Coletas == null)
                rotaFrete.Coletas = new List<Dominio.Entidades.Cliente>();

            for (var i = 1; i < remetentes.Count; i++)
                rotaFrete.Coletas.Add(remetentes[i]);
        }

        private static void GerarDestinatariisRotaFrete(Dominio.Entidades.RotaFrete rotaFrete, List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaDestinatarioOrdem> destinatarios, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.RotaFreteDestinatarios repRotaFreteDestinatarios = new Repositorio.RotaFreteDestinatarios(unitOfWork);

            foreach (var destinatario in destinatarios)
            {
                Dominio.Entidades.RotaFreteDestinatarios rotaFreteDestinatario = new Dominio.Entidades.RotaFreteDestinatarios()
                {
                    Cliente = destinatario.Cliente,
                    RotaFrete = rotaFrete,
                    Ordem = destinatario.Ordem,
                    ClienteOutroEndereco = destinatario.ClienteOutroEndereco,
                };
                repRotaFreteDestinatarios.Inserir(rotaFreteDestinatario);
            }
        }

        private static void ExculirPontosPassagem(Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem reCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);
            reCargaRotaFretePontosPassagem.DeletarPorCargaRotaFrete(cargaRotaFrete.Codigo);
        }
        private static async Task ExculirPontosPassagemAsync(Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem reCargaRotaFretePontosPassagem = new Repositorio.Embarcador.Cargas.CargaRotaFretePontosPassagem(unitOfWork);
            await reCargaRotaFretePontosPassagem.DeletarPorCargaRotaFreteAsync(cargaRotaFrete.Codigo);
        }

        private bool PossuiRestricaoData(Dominio.Entidades.RotaFrete rotaFrete, List<Dominio.Entidades.RotaFreteRestricao> restricoesRotaFrete, DateTime data, Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga tipoCarga)
        {
            if (rotaFrete == null)
                return false;

            if (restricoesRotaFrete == null || restricoesRotaFrete.Count() == 0)
                return false;

            DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(data);

            return (
                from restricao in restricoesRotaFrete
                where (
                    (
                        diaSemana == DiaSemana.Domingo ? restricao.Domingo :
                        diaSemana == DiaSemana.Segunda ? restricao.Segunda :
                        diaSemana == DiaSemana.Terca ? restricao.Terca :
                        diaSemana == DiaSemana.Quarta ? restricao.Quarta :
                        diaSemana == DiaSemana.Quinta ? restricao.Quinta :
                        diaSemana == DiaSemana.Sexta ? restricao.Sexta :
                        diaSemana == DiaSemana.Sabado ? restricao.Sabado : restricao.Segunda
                    ) &&
                    (data.TimeOfDay >= restricao.HoraInicio && data.TimeOfDay <= restricao.HoraTermino) &&
                    (restricao.TipoDeCarga == null || restricao.TipoDeCarga == tipoCarga) &&
                    (restricao.ModeloVeicularCarga == null || restricao.ModeloVeicularCarga == modeloVeicularCarga)
                )
                select restricao
            ).Count() > 0;
        }

        private void SetarPracasDePedagio(Dominio.Entidades.RotaFrete rotaFrete, List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagio, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspenso sentido, string erro, Repositorio.UnitOfWork unitOfWork)
        {
            if (pracasPedagio == null || !string.IsNullOrWhiteSpace(erro))
                return;

            Repositorio.RotaFretePracaPedagio repRotaFretePracas = new Repositorio.RotaFretePracaPedagio(unitOfWork);

            if (sentido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EixosSuspenso.Volta)
                repRotaFretePracas.DeletarPorRotaFrete(rotaFrete.Codigo);

            foreach (Dominio.Entidades.Embarcador.Logistica.PracaPedagio praca in pracasPedagio)
            {
                Dominio.Entidades.RotaFretePracaPedagio rotaFretePracas = new Dominio.Entidades.RotaFretePracaPedagio()
                {
                    PracaPedagio = praca,
                    RotaFrete = rotaFrete,
                    EixosSuspenso = sentido
                };

                repRotaFretePracas.Inserir(rotaFretePracas);
            }
        }

        private void AtualizarLatLng(Dominio.Entidades.Cliente remetente, ICollection<Dominio.Entidades.RotaFreteDestinatarios> destinatarios, out string erro, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao)
        {
            erro = "";
            if (remetente == null)
            {
                erro = String.Format("Rementente não informado)");
                return;
            }

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            Servicos.Embarcador.Logistica.GeoCode geo = Servicos.Embarcador.Carga.CargaRotaFrete.ObterServiceGeocodingGoogle(configuracaoIntegracao);
            Servicos.Embarcador.Logistica.Nominatim.Service nominatim = Servicos.Embarcador.Carga.CargaRotaFrete.ObterServiceNominatim(configuracaoIntegracao);

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding geoServiceGeocoding = (configuracaoIntegracao?.GeoServiceGeocoding ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding.Google);

            //Rementente
            if (!Servicos.Embarcador.Carga.CargaRotaFrete.ValidarCoordenadas(remetente?.Latitude) || !Servicos.Embarcador.Carga.CargaRotaFrete.ValidarCoordenadas(remetente?.Longitude))
            {
                string enderecoGoogle = string.Empty;
                string bairroNominatim = string.Empty;
                string endereco = Servicos.Embarcador.Carga.CargaRotaFrete.ObterEndereco(remetente, configuracaoIntegracao, ref enderecoGoogle, ref bairroNominatim);

                Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint point = null;

                bool atualizarPorCidade = false;
                if (geoServiceGeocoding == Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding.Nominatim)
                {
                    Servicos.Embarcador.Logistica.Nominatim.RootObject rootObject = nominatim.Geocoding(endereco);
                    if (rootObject == null && !string.IsNullOrWhiteSpace(bairroNominatim))
                        rootObject = nominatim.Geocoding(endereco.Replace(bairroNominatim, ""));

                    if (rootObject != null)
                        point = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = rootObject.lat.ToDouble(), Lng = rootObject.lon.ToDouble() };
                    else if (remetente.Tipo == "F")
                        atualizarPorCidade = true;

                    //Se não encontrou no Nominatim, busca por CEP no cadastro MultisoftwareAdmin.
                    if (point == null && !String.IsNullOrWhiteSpace(remetente.CEP))
                        point = ObterCoordenadasPorCep(remetente.CEP.ObterSomenteNumeros().ToInt());

                    //Se não encontrou no cadastro MultisoftwareAdmin, busca por Localidade.
                    if (point == null && !String.IsNullOrEmpty(remetente.Localidade?.Latitude.ToString()) && !String.IsNullOrEmpty(remetente.Localidade?.Longitude.ToString()))
                        point = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = Convert.ToDouble(remetente.Localidade?.Latitude), Lng = Convert.ToDouble(remetente.Localidade?.Longitude) };
                }

                // Se a configuração for Google
                if (geoServiceGeocoding == Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding.Google && !atualizarPorCidade)
                    point = geo.BuscarLatLng(enderecoGoogle);

                if (point == null && atualizarPorCidade)
                    point = Servicos.Embarcador.Carga.CargaRotaFrete.ObterCoordenadasPorCidade(remetente.Localidade, configuracaoIntegracao, unitOfWork);

                if (point == null)
                {
                    erro = String.Format("Não foi possível encontrar o endereço para o remetente {0}", remetente.Nome);
                    return;
                }

                remetente.Latitude = point.Lat.ToString();
                remetente.Longitude = point.Lng.ToString();
                remetente.DataUltimaAtualizacao = DateTime.Now;
                remetente.Integrado = false;
                repCliente.Atualizar(remetente);
            }

            //Destinatarios
            foreach (Dominio.Entidades.RotaFreteDestinatarios destinatario in destinatarios)
            {
                if (!Servicos.Embarcador.Carga.CargaRotaFrete.ValidarCoordenadas(destinatario?.Cliente.Latitude) || !Servicos.Embarcador.Carga.CargaRotaFrete.ValidarCoordenadas(destinatario?.Cliente.Longitude))
                {
                    string enderecoGoogle = string.Empty;
                    string bairroNominatim = string.Empty;
                    string endereco = Servicos.Embarcador.Carga.CargaRotaFrete.ObterEndereco(destinatario.Cliente, configuracaoIntegracao, ref enderecoGoogle, ref bairroNominatim);

                    Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint point = null;

                    bool atualizarPorCidade = false;
                    if (geoServiceGeocoding == Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding.Nominatim)
                    {
                        Servicos.Embarcador.Logistica.Nominatim.RootObject rootObject = nominatim.Geocoding(endereco);
                        if (rootObject == null && !string.IsNullOrWhiteSpace(bairroNominatim))
                            rootObject = nominatim.Geocoding(endereco.Replace(bairroNominatim, ""));

                        if (rootObject != null)
                            point = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = rootObject.lat.ToDouble(), Lng = rootObject.lon.ToDouble() };
                        else if ((destinatario?.Cliente?.Tipo ?? "") == "F")
                            atualizarPorCidade = true;

                        //Se não encontrou no Nominatim, busca por CEP no cadastro MultisoftwareAdmin.
                        if (point == null && !String.IsNullOrWhiteSpace(destinatario?.Cliente?.CEP))
                            point = ObterCoordenadasPorCep(destinatario.Cliente.CEP.ObterSomenteNumeros().ToInt());

                        //Se não encontrou no cadastro MultisoftwareAdmin, busca por Localidade.
                        if (point == null && !String.IsNullOrEmpty(destinatario?.Cliente?.Localidade?.Latitude.ToString()) && !String.IsNullOrEmpty(destinatario?.Cliente?.Localidade?.Longitude.ToString()))
                            point = new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint() { Lat = Convert.ToDouble(destinatario.Cliente.Localidade.Latitude), Lng = Convert.ToDouble(destinatario.Cliente.Localidade.Longitude) };
                    }

                    // Se a configuração for Google
                    if (geoServiceGeocoding == Dominio.ObjetosDeValor.Embarcador.Enumeradores.GeoServiceGeocoding.Google && !atualizarPorCidade)
                        point = geo.BuscarLatLng(enderecoGoogle);

                    if (point == null && atualizarPorCidade)
                        point = Servicos.Embarcador.Carga.CargaRotaFrete.ObterCoordenadasPorCidade(remetente.Localidade, configuracaoIntegracao, unitOfWork);

                    if (point == null)
                    {
                        erro = String.Format("Não foi possível encontrar o endereço para o destinatário {0}", destinatario.Cliente.Nome);
                        return;
                    }

                    destinatario.Cliente.Latitude = point.Lat.ToString();
                    destinatario.Cliente.Longitude = point.Lng.ToString();
                    destinatario.Cliente.DataUltimaAtualizacao = DateTime.Now;
                    destinatario.Cliente.Integrado = false;
                    repCliente.Atualizar(destinatario.Cliente);
                }
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint ObterCoordenadasPorCep(int cep)
        {
            using (AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(StringConexaoAdmin()))
            {
                AdminMultisoftware.Repositorio.Localidades.Geo repositorioGeo = new AdminMultisoftware.Repositorio.Localidades.Geo(unitOfWorkAdmin);
                AdminMultisoftware.Dominio.ObjetosDeValor.Localidades.Geo geo = repositorioGeo.BuscarPorCEP(cep);
                //Caso não tenha achado no CEP ou a LAT LNG do cep é "0"
                if (geo == null || (geo?.latitude ?? 0) == 0) return null;
                return new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint((double)((geo?.latitude ?? 0) != 0 ? geo.latitude : geo.latitude_nominatim), (double)((geo?.latitude ?? 0) != 0 ? geo.longitude : geo.longitude_nominatim));
            }
        }

        private static string StringConexaoAdmin()
        {
            return Servicos.Database.ConnectionString.Instance.GetDatabaseConnectionString("AdminMultisoftware");
        }

        #endregion
    }
}
