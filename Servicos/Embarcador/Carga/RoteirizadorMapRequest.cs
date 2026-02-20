using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga
{
    public class RoteirizadorMapRequest : ServicoBase
    {
        public RoteirizadorMapRequest(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public void AlterarPontoMaisDistante(ref List<Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location> pontos, ref List<Dominio.Entidades.Cliente> pessoas)
        {

            Servicos.Embarcador.Integracao.MapRequest.RouteMatrix serRouteMatrix = new Integracao.MapRequest.RouteMatrix(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.routeMatrix routeMatrix = serRouteMatrix.BuscarDistancias(pontos);
            decimal maiorDistancia = routeMatrix.distance.Max();
            int index = routeMatrix.distance.FindIndex(obj => obj == maiorDistancia);
            Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location ultimoPonto = pontos.ElementAt(index);
            pontos.RemoveAt(index);
            pontos.Add(ultimoPonto);

            Dominio.Entidades.Cliente pessoa = pessoas.ElementAt(index);
            pessoas.RemoveAt(index);
            pessoas.Add(pessoa);

        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa> CriarRotaViaMapRequest(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao, string tipoRota, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.MapRequestApi serMapRequest = new Logistica.MapRequestApi(unitOfWork);

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);

            Servicos.Embarcador.Integracao.MapRequest.Directions serDirectionsAPI = new Servicos.Embarcador.Integracao.MapRequest.Directions(unitOfWork);
            List<Dominio.Entidades.Cliente> pessoas = (from obj in cargaPedidos select obj.Pedido.Remetente).Distinct().ToList();
            pessoas.AddRange((from obj in cargaPedidos select obj.Pedido.Destinatario).Distinct().ToList());

            if (tipoUltimoPontoRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Retornando || tipoUltimoPontoRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.AteOrigem)
            {
                pessoas.Add(pessoas[0]);
            }
            validarLocalidadesPessoas(pessoas, unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa> rotasInformacaoPessoa = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa>();

            bool todosPontos = true;
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location> pontos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location>();
            foreach (Dominio.Entidades.Cliente pessoa in pessoas)
            {
                if (pessoa.TipoLocalizacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.naoEncontrado)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location ponto = new Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.location();
                    ponto.latLng = new Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.latLng();
                    ponto.latLng.lat = decimal.Parse(pessoa.Latitude.Replace(".", ","));
                    ponto.latLng.lng = decimal.Parse(pessoa.Longitude.Replace(".", ","));
                    pontos.Add(ponto);
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa rotaInformacao = new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa();
                    rotaInformacao.pessoa = serPessoa.ConverterObjetoPessoa(pessoa);
                    rotaInformacao.coordenadas = new Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas();
                    rotaInformacao.coordenadas.tipoLocalizacao = pessoa.TipoLocalizacao;
                    rotasInformacaoPessoa.Add(rotaInformacao);
                    todosPontos = false;
                }
            }

            if (todosPontos)
            {
                if (tipoUltimoPontoRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.PontoMaisDistante)
                {
                    AlterarPontoMaisDistante(ref pontos, ref pessoas);
                }

                Dominio.ObjetosDeValor.Embarcador.Integracao.MapRequest.route route = serDirectionsAPI.OtimizarRota(pontos, tipoRota);
                int indexLimite = 0;
                if (tipoUltimoPontoRoteirizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Retornando)
                    indexLimite = 1;

                for (int i = 0; i < route.locationSequence.Count - indexLimite; i++)
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa rotaInformacao = new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa();
                    rotaInformacao.coordenadas = new Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas();
                    int indice = route.locationSequence[i];

                    Dominio.Entidades.Cliente pessoa = pessoas[indice];
                    rotaInformacao.coordenadas.tipoLocalizacao = pessoa.TipoLocalizacao;
                    rotaInformacao.pessoa = serPessoa.ConverterObjetoPessoa(pessoa);
                    rotaInformacao.coordenadas.latitude = pessoa.Latitude;
                    rotaInformacao.coordenadas.longitude = pessoa.Longitude;

                    if (i != route.locationSequence.Count - (indexLimite + 1))
                    {
                        rotaInformacao.distancia = route.legs[i].distance;
                        rotaInformacao.tempo = route.legs[i].time;
                    }
                    rotasInformacaoPessoa.Add(rotaInformacao);
                }
            }

            return rotasInformacaoPessoa;
        }

        private void validarLocalidadesPessoas(List<Dominio.Entidades.Cliente> pessoas, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.MapRequest.Geocoding serGeocoding = new Integracao.MapRequest.Geocoding();
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            foreach (Dominio.Entidades.Cliente pessoa in pessoas)
            {
                if (pessoa.TipoLocalizacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalizacao.naoEncontrado)
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas coordenadas = serGeocoding.BuscarCoordenadasEndereco(pessoa.Localidade, pessoa.Endereco, pessoa.Numero);
                    pessoa.TipoLocalizacao = coordenadas.tipoLocalizacao;
                    pessoa.Latitude = coordenadas.latitude;
                    pessoa.Longitude = coordenadas.longitude;
                    pessoa.DataUltimaAtualizacao = DateTime.Now;
                    pessoa.Integrado = false;
                    repCliente.Atualizar(pessoa);
                }
            }
        }
    }
}

