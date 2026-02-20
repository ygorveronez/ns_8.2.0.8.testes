using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Maps.Google
{
    public class RoteirizarCarga : ServicoBase
    {        

        public RoteirizarCarga(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
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

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa> CriarRotaViaGoogleMaps(Dominio.Entidades.Embarcador.Cargas.Carga carga, List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao, string tipoRota, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.MapRequestApi serMapRequest = new Logistica.MapRequestApi(unitOfWork);

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);

            Servicos.Embarcador.Integracao.Google.GoogleMaps serGoogleMapsAPI = new Servicos.Embarcador.Integracao.Google.GoogleMaps(unitOfWork);
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

                Dominio.ObjetosDeValor.Embarcador.Integracao.Google.routes route = serGoogleMapsAPI.OtimizarRota(pontos, tipoRota, tipoUltimoPontoRoteirizacao);
         

                rotasInformacaoPessoa.Add(retornarPessoaRota(pessoas[0], 0, 0));

                for (int i = 0; i < route.waypoint_order.Count; i++)
                {
                    int indice = route.waypoint_order[i];
                    rotasInformacaoPessoa.Add(retornarPessoaRota(pessoas[indice + 1], route.legs[i].distance.value, route.legs[i].duration.value));
                }
                
                if (tipoUltimoPontoRoteirizacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Retornando)
                    rotasInformacaoPessoa.Add(retornarPessoaRota(pessoas[pessoas.Count -1], 0, 0));
            }

            return rotasInformacaoPessoa;
        }

        public Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa retornarPessoaRota(Dominio.Entidades.Cliente pessoa, int distancia, int duracao)
        {
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa rotaInformacao = new Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa();
            rotaInformacao.coordenadas = new Dominio.ObjetosDeValor.Embarcador.Logistica.Coordenadas();

            rotaInformacao.coordenadas.tipoLocalizacao = pessoa.TipoLocalizacao;
            rotaInformacao.pessoa = serPessoa.ConverterObjetoPessoa(pessoa);
            rotaInformacao.coordenadas.latitude = pessoa.Latitude;
            rotaInformacao.coordenadas.longitude = pessoa.Longitude;
            if (distancia > 0)
                rotaInformacao.distancia = distancia / 1000;
            if (duracao > 0)
                rotaInformacao.tempo = (duracao / 60);

            return rotaInformacao;
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


        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.RotaInformacaoPessoa> CriarRotaCarregamentoViaGoogleMaps(Dominio.Entidades.Embarcador.Cargas.MontagemCarga.Carregamento carregamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao tipoUltimoPontoRoteirizacao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.MapRequestApi serMapRequest = new Logistica.MapRequestApi(unitOfWork);

            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Servicos.WebService.Pessoas.Pessoa serPessoa = new WebService.Pessoas.Pessoa(unitOfWork);

            Servicos.Embarcador.Integracao.Google.GoogleMaps serGoogleMapsAPI = new Servicos.Embarcador.Integracao.Google.GoogleMaps(unitOfWork);
            List<Dominio.Entidades.Cliente> pessoas = (from obj in carregamento.Pedidos select obj.Pedido.Remetente).Distinct().ToList();
            pessoas.AddRange((from obj in carregamento.Pedidos select obj.Pedido.Destinatario).Distinct().ToList());

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

                Dominio.ObjetosDeValor.Embarcador.Integracao.Google.routes route = serGoogleMapsAPI.OtimizarRota(pontos, "", tipoUltimoPontoRoteirizacao);


                rotasInformacaoPessoa.Add(retornarPessoaRota(pessoas[0], 0, 0));

                for (int i = 0; i < route.waypoint_order.Count; i++)
                {
                    int indice = route.waypoint_order[i];
                    rotasInformacaoPessoa.Add(retornarPessoaRota(pessoas[indice + 1], route.legs[i].distance.value, route.legs[i].duration.value));
                }

                if (tipoUltimoPontoRoteirizacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoUltimoPontoRoteirizacao.Retornando)
                    rotasInformacaoPessoa.Add(retornarPessoaRota(pessoas[pessoas.Count - 1], 0, 0));
            }

            return rotasInformacaoPessoa;
        }

    }
}
