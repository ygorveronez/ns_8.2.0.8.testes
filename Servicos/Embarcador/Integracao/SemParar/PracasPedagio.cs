using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Servicos.Embarcador.Integracao.SemParar
{
    public class PracasPedagio : IntegracaoClientBase<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>
    {
        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial Autenticar(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Frota.ValePedagio servicoValePedagio = new Frota.ValePedagio(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial = new Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial();

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSemParar integracaoSemParar = servicoValePedagio.ObterIntegracaoSemPararParaAutenticacao(tipoServicoMultisoftware);

                if (integracaoSemParar == null)
                {
                    credencial.Autenticado = false;
                    credencial.Retorno = "Sem parar não está configurado, por favor, entre em contato com a Multisoftware";
                }
                else
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

                    SemPararValePedagio.Identificador identifador = valePedagioClient.autenticarUsuario(integracaoSemParar.CNPJ, integracaoSemParar.Usuario, integracaoSemParar.Senha);
                    string request = inspector.LastRequestXML;
                    string response = inspector.LastResponseXML;

                    Servicos.Log.TratarErro(request, "IntegracaoSemParar");
                    Servicos.Log.TratarErro(response, "IntegracaoSemParar");

                    if (identifador.status == 0)
                    {
                        credencial.Autenticado = true;
                        credencial.Retorno = "Autenticado com sucesso";
                        credencial.Sessao = identifador.sessao;
                    }
                    else
                    {
                        credencial.Autenticado = false;
                        credencial.Retorno = ValePedagio.ObterMensagemRetorno(identifador.status);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                credencial.Autenticado = false;
                credencial.Retorno = "O WS da sem parar não está disponivel no momento.";
            }
            return credencial;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> ObterPontosPracaDePedagioPorPontosDaRota(string pontosDaRota)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>>(pontosDaRota);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> pontosRetorno = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();

            foreach (var ponto in pontos)
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint
                {
                    Latitude = ponto.lat,
                    Longitude = ponto.lng
                };

                pontosRetorno.Add(wayPoint);
            }

            return pontosRetorno;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> ObterPontosPracaDePedagioPorPolilinha(string polilinha, int distanciaMinima)
        {
            if (distanciaMinima == 0)
                distanciaMinima = 15000;

            var listapontos = Servicos.Embarcador.Logistica.Polilinha.ObterPontosPolilinha(polilinha, distanciaMinima);


            while (listapontos.Count > 150)
            {
                distanciaMinima += 1000;
                listapontos = Servicos.Embarcador.Logistica.Polilinha.ObterPontosPolilinha(polilinha, distanciaMinima);

            }
            return listapontos;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> ObterPracasPedagioIda(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, string pontosDaRota, out string erro, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota tipoConsultaRota, out string request, out string response, Dominio.Entidades.RotaFrete rotaFrete = null)
        {
            request = "";
            response = "";

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>>(pontosDaRota);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> pontosOrigem = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
            for (int i = 0; i < pontos.Count; i++)
            {
                if (pontos[i].tipoponto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Retorno)
                {
                    pontosOrigem.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint()
                    {
                        Latitude = pontos[i].lat,
                        Longitude = pontos[i].lng
                    });
                }
            }

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio> pracasPedagiosSemPararIda = ObterPracasPedagioPorPontos(credencial, pontosOrigem, out erro, unitOfWork, tipoConsultaRota, out request, out response, rotaFrete);
            List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagioIda = ValidarCadastrosPracasPedagioSemParar(pracasPedagiosSemPararIda, unitOfWork);
            return pracasPedagioIda;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> ObterPracasPedagioVolta(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, string pontosDaRota, out string erro, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota tipoConsultaRota, out string request, out string response, Dominio.Entidades.RotaFrete rotaFrete = null)
        {
            request = "";
            response = "";

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota> pontos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota>>(pontosDaRota);
            int indiceUltimaEntrega = -1;
            if (pontos?.Count > 0)
            {
                indiceUltimaEntrega = pontos.Count - 1;
                if (pontos[pontos.Count - 1].tipoponto == TipoPontoPassagem.Coleta || pontos[pontos.Count - 1].tipoponto == TipoPontoPassagem.Retorno && pontos.Count > 1)
                    indiceUltimaEntrega = pontos.Count - 2;
            }

            Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.PontosDaRota origem = (from ponto in pontos where ponto.tipoponto == TipoPontoPassagem.Coleta select ponto).FirstOrDefault();
            if (origem == null)
                origem = (from ponto in pontos where ponto.tipoponto == TipoPontoPassagem.Retorno select ponto).FirstOrDefault();

            if (pontos?.Count > 0)
            {
                indiceUltimaEntrega = pontos.Count - 1;
                if (pontos[pontos.Count - 1].tipoponto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Coleta || pontos[pontos.Count - 1].tipoponto == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Retorno && pontos.Count > 1)
                    indiceUltimaEntrega = pontos.Count - 2;
            }
            List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagioRetorno = new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();
            if (indiceUltimaEntrega >= 0 && origem != null)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> pontosDeVolta = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
                pontosDeVolta.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint() { Latitude = pontos[indiceUltimaEntrega].lat, Longitude = pontos[indiceUltimaEntrega].lng });
                pontosDeVolta.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint() { Latitude = origem.lat, Longitude = origem.lng });

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio> pracasPedagiosSemPararVolta = ObterPracasPedagioPorPontos(credencial, pontosDeVolta, out erro, unitOfWork, tipoConsultaRota, out request, out response, rotaFrete);

                pracasPedagioRetorno = ValidarCadastrosPracasPedagioSemParar(pracasPedagiosSemPararVolta, unitOfWork);
            }
            else
            {
                erro = "Não foi localizado um destino para a rota na busca das praças de pedágio.";
            }
            return pracasPedagioRetorno;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> ObterPracasPedagioPorPolilinha(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, string polilinha, int distanciaMinimaQuadrante, out string erro, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota tipoConsultaRota, Dominio.Entidades.RotaFrete rotaFrete = null)
        {
            string request = "";
            string response = "";

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPoints = ObterPontosPracaDePedagioPorPolilinha(polilinha, distanciaMinimaQuadrante);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio> pracasPedagiosSemPararVolta = ObterPracasPedagioPorPontos(credencial, wayPoints, out erro, unitOfWork, tipoConsultaRota, out request, out response, rotaFrete);

            return ValidarCadastrosPracasPedagioSemParar(pracasPedagiosSemPararVolta, unitOfWork);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> ValidarCadastrosPracasPedagioSemParar(List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio> pracasPedagiosSemParar, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Logistica.PracaPedagio repPracaPedagio = new Repositorio.Embarcador.Logistica.PracaPedagio(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> pracasPedagio = new List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();

            foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio praca in pracasPedagiosSemParar)
            {
                Dominio.Entidades.Embarcador.Logistica.PracaPedagio pracaPedagio = repPracaPedagio.BuscarPorCodigoIntegracao(praca.Id.ToString());
                if (pracaPedagio == null)
                {
                    pracaPedagio = new Dominio.Entidades.Embarcador.Logistica.PracaPedagio();
                    pracaPedagio.Ativo = true;
                    pracaPedagio.CodigoIntegracao = praca.Id.ToString();
                    pracaPedagio.Concessionaria = praca.Concessionaria;
                    pracaPedagio.Descricao = praca.Praca;
                    pracaPedagio.KM = praca.KM;
                    pracaPedagio.Observacao = "";
                    pracaPedagio.Rodovia = praca.Rodovia;
                    repPracaPedagio.Inserir(pracaPedagio);
                }
                pracasPedagio.Add(pracaPedagio);
            }

            return pracasPedagio;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio> ObterPracasPedagioPorPontos(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> listaPontos, out string erro, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota tipoConsultaRota, out string request, out string response, Dominio.Entidades.RotaFrete rotaFrete = null)
        {
            //unitOfWork.Start();

            erro = "";
            request = "";
            response = "";

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio> pracas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio>();

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

            Servicos.SemPararValePedagio.PontosParada pontosParada = new SemPararValePedagio.PontosParada();
            Servicos.SemPararValePedagio.OpcoesRota opcoesRota = new SemPararValePedagio.OpcoesRota();
            opcoesRota.alternativas = false;
            opcoesRota.tipoRota = (int)tipoConsultaRota;

            List<Servicos.SemPararValePedagio.PontoParada> pontos = new List<SemPararValePedagio.PontoParada>();

            foreach (var listaPonto in listaPontos)
            {
                Servicos.SemPararValePedagio.PontoParada ponto = new SemPararValePedagio.PontoParada();
                ponto.latLong = new SemPararValePedagio.LatLong
                {
                    latitude = listaPonto.Latitude,
                    longitude = listaPonto.Longitude
                };
                pontos.Add(ponto);
            }

            try
            {
                pontosParada.pontoParada = pontos.ToArray();
                SemPararValePedagio.InfoRoteirizacao infoRoteirizacao = valePedagioClient.roteirizarPracasPedagio(pontosParada, opcoesRota, credencial.Sessao);

                request = inspector.LastRequestXML;
                response = inspector.LastResponseXML;

                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
                {
                    Data = DateTime.Now,
                    Mensagem = infoRoteirizacao.statusMensagem,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request, "xml", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(response, "xml", unitOfWork),
                };

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                if (rotaFrete != null)
                {
                    rotaFrete.ArquivosTransacaoRotaFrete.Add(arquivoIntegracao);
                    repRotaFrete.Atualizar(rotaFrete);
                }

                if (infoRoteirizacao.status == 0 || infoRoteirizacao.status == 808)
                {
                    if (infoRoteirizacao.pracaPedagio != null)
                    {
                        for (int i = 0; i < infoRoteirizacao.pracaPedagio.Length; i++)
                        {
                            SemPararValePedagio.InfoPracaPedagio infoPracaPedagio = infoRoteirizacao.pracaPedagio[i];
                            Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio praca = new Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio();
                            praca.Concessionaria = !string.IsNullOrWhiteSpace(infoPracaPedagio.rodovia) ? infoPracaPedagio.concessionaria : "";
                            praca.KM = (decimal)infoPracaPedagio.km;
                            praca.Rodovia = !string.IsNullOrWhiteSpace(infoPracaPedagio.rodovia) ? infoPracaPedagio.rodovia : "";
                            praca.Praca = !string.IsNullOrWhiteSpace(infoPracaPedagio.praca) ? infoPracaPedagio.praca : "";
                            praca.Id = infoPracaPedagio.id;
                            pracas.Add(praca);
                        }
                    }
                }
                else
                {
                    erro = ValePedagio.ObterMensagemRetorno(infoRoteirizacao.status);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(inspector.LastRequestXML, "IntegracaoSemParar");
                Servicos.Log.TratarErro(inspector.LastResponseXML, "IntegracaoSemParar");
                Servicos.Log.TratarErro(ex, "IntegracaoSemParar");

                //unitOfWork.Rollback();
                erro = "Falha na consulta das praças com o Sem Parar";
            }

            //unitOfWork.CommitChanges();
            return pracas;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio> ObterPracasPedagio(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, List<int> CodigosIBGE, out string erro, out string xmlRequest, out string xmlResponse, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaRota tipoConsultaRota, Dominio.Entidades.RotaFrete rotaFrete = null)
        {
            //unitOfWork.Start();

            erro = "";
            xmlRequest = "";
            xmlResponse = "";

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio> pracas = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio>();

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

            Servicos.SemPararValePedagio.PontosParada pontosParada = new SemPararValePedagio.PontosParada();
            Servicos.SemPararValePedagio.OpcoesRota opcoesRota = new SemPararValePedagio.OpcoesRota();
            opcoesRota.alternativas = false;
            opcoesRota.tipoRota = (int)tipoConsultaRota;

            List<Servicos.SemPararValePedagio.PontoParada> pontos = new List<SemPararValePedagio.PontoParada>();

            foreach (int codigoIBGE in CodigosIBGE)
            {
                if (codigoIBGE > 0)
                {
                    Dominio.Entidades.Localidade localidade = repLocalidade.BuscarPorCodigoIBGE(codigoIBGE);
                    Servicos.SemPararValePedagio.PontoParada ponto = new SemPararValePedagio.PontoParada();
                    ponto.codigoIBGE = codigoIBGE;
                    ponto.descricao = localidade != null ? localidade.Descricao + "," + localidade.Estado.Sigla : string.Empty;
                    ponto.latLong = localidade != null && localidade.Latitude.HasValue && localidade.Longitude.HasValue && localidade.Latitude != 0 && localidade.Longitude != 0 ? new SemPararValePedagio.LatLong()
                    {
                        latitude = (double)(localidade.Latitude ?? 0),
                        longitude = (double)(localidade.Longitude ?? 0),
                    } : null;
                    pontos.Add(ponto);
                }
            }

            pontosParada.pontoParada = pontos.ToArray();
            try
            {
                SemPararValePedagio.InfoRoteirizacao infoRoteirizacao = valePedagioClient.roteirizarPracasPedagio(pontosParada, opcoesRota, credencial.Sessao);

                xmlRequest = inspector.LastRequestXML;
                xmlResponse = inspector.LastResponseXML;

                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
                Repositorio.RotaFrete repRotaFrete = new Repositorio.RotaFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo
                {
                    Data = DateTime.Now,
                    Mensagem = infoRoteirizacao.statusMensagem,
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork),
                };

                repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                if (rotaFrete != null)
                {
                    rotaFrete.ArquivosTransacaoRotaFrete.Add(arquivoIntegracao);
                    repRotaFrete.Atualizar(rotaFrete);
                }

                if (infoRoteirizacao.status == 0)
                {
                    for (int i = 0; i < infoRoteirizacao.pracaPedagio.Length; i++)
                    {
                        SemPararValePedagio.InfoPracaPedagio infoPracaPedagio = infoRoteirizacao.pracaPedagio[i];
                        Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio praca = new Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio();
                        praca.Concessionaria = !string.IsNullOrWhiteSpace(infoPracaPedagio.rodovia) ? infoPracaPedagio.concessionaria : "";
                        praca.KM = (decimal)infoPracaPedagio.km;
                        praca.Rodovia = !string.IsNullOrWhiteSpace(infoPracaPedagio.rodovia) ? infoPracaPedagio.rodovia : "";
                        praca.Praca = !string.IsNullOrWhiteSpace(infoPracaPedagio.praca) ? infoPracaPedagio.praca : "";
                        praca.Id = infoPracaPedagio.id;
                        pracas.Add(praca);
                    }
                }
                else if (infoRoteirizacao.status == 808)
                {
                    return pracas;
                }
                else
                {
                    erro = ValePedagio.ObterMensagemRetorno(infoRoteirizacao.status);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(inspector.LastRequestXML, "IntegracaoSemParar");
                Servicos.Log.TratarErro(inspector.LastResponseXML, "IntegracaoSemParar");
                Servicos.Log.TratarErro(ex, "IntegracaoSemParar");

                //unitOfWork.Rollback();
                erro = "Falha na consulta das praças com o Sem Parar";
            }

            //unitOfWork.CommitChanges();
            return pracas;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.TarifaPracaPedagio> ObterValoresPracas(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, int quantidadeEixos, out string erro, out string xmlRequest, out string xmlResponse, Repositorio.UnitOfWork unitOfWork)
        {

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.TarifaPracaPedagio> tarifasPracaPedagio = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.TarifaPracaPedagio>();

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

            Servicos.SemPararValePedagio.CategoriaVeiculo categoria = new SemPararValePedagio.CategoriaVeiculo { eixos = quantidadeEixos, tipoRodagem = 1 };
            Servicos.SemPararValePedagio.TipoVeiculo tipoVeiculo = new SemPararValePedagio.TipoVeiculo { codigo = 4 /*Caminhão*/ };
            Servicos.SemPararValePedagio.ValoresPracas valoresPracas = valePedagioClient.obterValoresPracas(categoria, tipoVeiculo, credencial.Sessao);

            erro = "";
            xmlRequest = inspector.LastRequestXML;
            xmlResponse = inspector.LastResponseXML;
            Servicos.Log.TratarErro(xmlRequest, "IntegracaoSemParar");
            Servicos.Log.TratarErro(xmlResponse, "IntegracaoSemParar");

            if (valoresPracas.status == 0)
            {
                int total = valoresPracas.pracaRota?.Length ?? 0;
                for (int i = 0; i < total; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.TarifaPracaPedagio tarifaPracaPedagio = new Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.TarifaPracaPedagio();
                    tarifaPracaPedagio.NomeConcessionaria = !string.IsNullOrWhiteSpace(valoresPracas.pracaRota[i].nomeConcessionaria) ? valoresPracas.pracaRota[i].nomeConcessionaria : "";
                    tarifaPracaPedagio.NomeRodovia = !string.IsNullOrWhiteSpace(valoresPracas.pracaRota[i].nomeRodovia) ? valoresPracas.pracaRota[i].nomeRodovia : "";
                    tarifaPracaPedagio.NomePraca = !string.IsNullOrWhiteSpace(valoresPracas.pracaRota[i].nomePraca) ? valoresPracas.pracaRota[i].nomePraca : "";
                    tarifaPracaPedagio.Placa = valoresPracas.pracaRota[i].placa;
                    tarifaPracaPedagio.Tarifa = (valoresPracas.pracaRota[i].tarifa.HasValue) ? valoresPracas.pracaRota[i].tarifa.Value : 0;
                    tarifasPracaPedagio.Add(tarifaPracaPedagio);
                }
            }
            else
            {
                erro = ValePedagio.ObterMensagemRetorno(valoresPracas.status);
            }

            return tarifasPracaPedagio;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio> ListaPracasPedagio(Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.Credencial credencial, out string erro, out string xmlRequest, out string xmlResponse, Repositorio.UnitOfWork unitOfWork)
        {

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio> pracasPedagio = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio>();

            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            SemPararValePedagio.ValePedagioClient valePedagioClient = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unitOfWork).ObterClient<SemPararValePedagio.ValePedagioClient, SemPararValePedagio.ValePedagio>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.SemParar_ValePedagio, out Servicos.Models.Integracao.InspectorBehavior inspector);

            Servicos.SemPararValePedagio.InfoPracaPedagio[] infoPracaPedagio = valePedagioClient.listaPracasPedagio(credencial.Sessao);

            erro = "";
            xmlRequest = inspector.LastRequestXML;
            xmlResponse = inspector.LastResponseXML;
            Servicos.Log.TratarErro(xmlRequest, "IntegracaoSemParar");
            Servicos.Log.TratarErro(xmlResponse, "IntegracaoSemParar");

            int total = infoPracaPedagio?.Length ?? 0;
            for (int i = 0; i < total; i++)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio pracaPedagio = new Dominio.ObjetosDeValor.Embarcador.Integracao.SemParar.PracasPedagio();
                pracaPedagio.Id = infoPracaPedagio[i].id;
                pracaPedagio.KM = (decimal)infoPracaPedagio[i].km;
                pracaPedagio.Concessionaria = !string.IsNullOrWhiteSpace(infoPracaPedagio[i].concessionaria) ? infoPracaPedagio[i].concessionaria : "";
                pracaPedagio.Praca = !string.IsNullOrWhiteSpace(infoPracaPedagio[i].praca) ? infoPracaPedagio[i].praca : "";
                pracaPedagio.Rodovia = !string.IsNullOrWhiteSpace(infoPracaPedagio[i].rodovia) ? infoPracaPedagio[i].rodovia : "";
                pracasPedagio.Add(pracaPedagio);
            }

            return pracasPedagio;
        }

        #endregion
    }
}
