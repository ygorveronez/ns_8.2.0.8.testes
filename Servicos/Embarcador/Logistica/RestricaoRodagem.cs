using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Servicos.Embarcador.Logistica
{
    public class RestricaoRodagem
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public RestricaoRodagem(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana ObterDiaSemana(DateTime data)
        {
            switch (data.DayOfWeek)
            {
                case DayOfWeek.Friday: return Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Sexta;
                case DayOfWeek.Monday: return Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Segunda;
                case DayOfWeek.Saturday: return Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Sabado;
                case DayOfWeek.Sunday: return Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Domingo;
                case DayOfWeek.Thursday: return Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Quinta;
                case DayOfWeek.Tuesday: return Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Terca;
                case DayOfWeek.Wednesday: return Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Quarta;
                default: return Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana.Domingo;
            }
        }

        private int ObterFinalPlaca(string placa)
        {
            return placa.LastOrDefault().ToString().ToInt();
        }

        private List<Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem> ObterRestricoesDia(int codigoCentroCarregamento, DateTime data, string placa)
        {
            int finalPlaca = ObterFinalPlaca(placa);
            Repositorio.Embarcador.Logistica.RestricaoRodagem repositorioRestricaoRodagem = new Repositorio.Embarcador.Logistica.RestricaoRodagem(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRestricaoRodagem filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRestricaoRodagem()
            {
                CodigoCentroCarregamento = codigoCentroCarregamento,
                DiaSemana = ObterDiaSemana(data),
                FinalPlaca = finalPlaca
            };

            return repositorioRestricaoRodagem.Consultar(filtrosPesquisa);
        }

        private bool PolilinhaContatoZona(Dominio.Entidades.Embarcador.Logistica.Locais zona, string polilinha)
        {
            //Pode conter mais de um "Desenho"
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea> areas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea>>(zona.Area);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> coordenadasPolilinha = Servicos.Embarcador.Logistica.Polilinha.Decodificar(polilinha);

            foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea area in areas)
            {
                //Agora vamos pegar todos os pedidos contidos na area.
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> coordenadasDentroArea = null;

                switch (area.type)
                {
                    case "circle":
                        var pontoRaio = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.center.lat, Longitude = area.center.lng };
                        coordenadasDentroArea = (from wp in coordenadasPolilinha
                                                 where Logistica.Distancia.ValidarNoRaio(wp, pontoRaio, area.radius / 1000)
                                                 select wp).ToList();

                        break;
                    case "rectangle":
                        var listaPontosRetangulo = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
                        listaPontosRetangulo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.north, Longitude = area.bounds.east });
                        listaPontosRetangulo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.north, Longitude = area.bounds.west });
                        listaPontosRetangulo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.south, Longitude = area.bounds.west });
                        listaPontosRetangulo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.south, Longitude = area.bounds.east });
                        coordenadasDentroArea = (from wp in coordenadasPolilinha
                                                 where Logistica.Distancia.ValidarPoligono(wp, listaPontosRetangulo.ToArray())
                                                 select wp).ToList();

                        break;
                    case "polygon":
                        var listaPontos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
                        foreach (var ponto in area.paths)
                            listaPontos.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = ponto.lat, Longitude = ponto.lng });
                        coordenadasDentroArea = (from wp in coordenadasPolilinha
                                                 where Logistica.Distancia.ValidarPoligono(wp, listaPontos.ToArray())
                                                 select wp).ToList();
                        break;
                }
                // Caso achou alguma coordenada da rota ... dentro de uma area  retorna true...
                if ((coordenadasDentroArea?.Count ?? 0) > 0)
                    return true;
            }

            return false;
        }

        #endregion

        #region Métodos Públicos

        public bool IsPossuiRestricao(int codigoCentroCarregamento, DateTime data, string placa, List<Dominio.Entidades.Cliente> clientesDestino)
        {
            if ((clientesDestino == null) || (clientesDestino.Count == 0))
                return false;

            List<Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem> restricoesDia = ObterRestricoesDia(codigoCentroCarregamento, data, placa);

            foreach (Dominio.Entidades.Embarcador.Logistica.RestricaoRodagem restricao in restricoesDia)
            {
                if ((data.TimeOfDay.CompareTo(restricao.HoraInicial) >= 0) && (data.TimeOfDay.CompareTo(restricao.HoraFinal) <= 0))
                {
                    foreach (Dominio.Entidades.Cliente clienteDestino in clientesDestino)
                    {
                        if (restricao.ClientesDestino?.Contains(clienteDestino) ?? false)
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Método para validar se uma polilinha está passando por alguma area de ZonaExclusaoRota.
        /// Cadastrada em Logistica/Locais.
        /// </summary>
        /// <param name="polilinha">Polinha a ser analisada.</param>
        /// <returns>True caso a polilinha passe por dentro de uma Zona de Exclusão de Rota.</returns>
        public bool IsPossuiRestricaoZonaExclusaoRota(string polilinha)
        {
            if (string.IsNullOrWhiteSpace(polilinha)) return false;

            Repositorio.Embarcador.Logistica.Locais repLocais = new Repositorio.Embarcador.Logistica.Locais(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.Locais> zonasExclusaoRota = repLocais.BuscarPorTipoDeLocalEFiliais(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.ZonaExclusaoRota, new List<int>());

            foreach (Dominio.Entidades.Embarcador.Logistica.Locais zona in zonasExclusaoRota)
            {
                if (PolilinhaContatoZona(zona, polilinha))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Método para validar se uma rota frete (Polilinha) está passando por alguma area
        /// de restrição desenhada na tela Logistica/Locais com o tipo ZonaExclusaoRota.
        /// </summary>
        /// <param name="rotaFrete">Rota de frete</param>
        public void ValidaAtualizaZonaExclusaoRota(Dominio.Entidades.RotaFrete rotaFrete)
        {
            if (rotaFrete == null) return;
            if (string.IsNullOrWhiteSpace(rotaFrete.PolilinhaRota)) return;

            if (this.IsPossuiRestricaoZonaExclusaoRota(rotaFrete.PolilinhaRota))
                rotaFrete.SituacaoDaRoteirizacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoRoteirizacao.EmZonaExclusao;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> WayPointsDesvioRestricaoZonaExclusaoRota(string polilinha)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint> resultado = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint>();

            if (string.IsNullOrWhiteSpace(polilinha)) return resultado;

            Repositorio.Embarcador.Logistica.Locais repLocais = new Repositorio.Embarcador.Logistica.Locais(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.Locais> zonasExclusaoRota = repLocais.BuscarPorTipoDeLocalEFiliais(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal.ZonaExclusaoRota, new List<int>());

            foreach (Dominio.Entidades.Embarcador.Logistica.Locais zona in zonasExclusaoRota)
            {
                if (PolilinhaContatoZona(zona, polilinha))
                {
                    int cont = 1;
                    //Pode conter mais de um "Desenho"
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea> areas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea>>(zona.Area);
                    foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea area in areas)
                    {
                        if (area.type == "marker")
                        {
                            resultado.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Roteirizacao.WayPoint()
                            {
                                Descricao = "Desvio " + cont.ToString() + " " + zona.Descricao,
                                Informacao = "Desvio " + cont.ToString() + " " + zona.Descricao,
                                Lat = area.position.lat,
                                Lng = area.position.lng,
                                TipoPonto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPontoPassagem.Passagem
                            });
                            cont++;
                        }
                    }
                }
            }

            return resultado;
        }

        #endregion
    }
}
