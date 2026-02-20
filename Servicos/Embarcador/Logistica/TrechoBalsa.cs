using System.Collections.Generic;
using Newtonsoft.Json;

namespace Servicos.Embarcador.Logistica
{
    public class TrechoBalsa
    {
        #region Atributos Privados Somente Leitura

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public TrechoBalsa(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private bool OrigemDestinoEntreLocaisDeTrechoBalsa(Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint pointOrigem, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint pointDestino)
        {
            //Pode conter mais de um "Desenho"
            List<Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea> areas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea>>(trechoBalsa.Area);

            bool pontoOrigemDentroArea = false;
            bool pontoDestinoDentroArea = false;

            // Vamos ter que validar origem e destino em areas distintas.. pois se a origem e o destino estiverem dentro da mesma area.. não passa pela balsa.
            foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.LocalArea area in areas)
            {
                switch (area.type)
                {
                    case "circle":
                        var pontoRaio = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.center.lat, Longitude = area.center.lng };
                        if (!pontoOrigemDentroArea)
                        {
                            pontoOrigemDentroArea = Logistica.Distancia.ValidarNoRaio(pointOrigem, pontoRaio, area.radius / 1000);
                            if (pontoOrigemDentroArea) break;
                        }
                        if (!pontoDestinoDentroArea)
                            pontoDestinoDentroArea = Logistica.Distancia.ValidarNoRaio(pointDestino, pontoRaio, area.radius / 1000);
                        break;
                    case "rectangle":
                        var listaPontosRetangulo = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
                        listaPontosRetangulo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.north, Longitude = area.bounds.east });
                        listaPontosRetangulo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.north, Longitude = area.bounds.west });
                        listaPontosRetangulo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.south, Longitude = area.bounds.west });
                        listaPontosRetangulo.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = area.bounds.south, Longitude = area.bounds.east });
                        if (!pontoOrigemDentroArea)
                        {
                            pontoOrigemDentroArea = Logistica.Distancia.ValidarPoligono(pointOrigem, listaPontosRetangulo.ToArray());
                            if (pontoOrigemDentroArea) break;
                        }
                        if (!pontoDestinoDentroArea)
                            pontoDestinoDentroArea = Logistica.Distancia.ValidarPoligono(pointDestino, listaPontosRetangulo.ToArray());
                        break;
                    case "polygon":
                        var listaPontos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint>();
                        foreach (var ponto in area.paths)
                            listaPontos.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = ponto.lat, Longitude = ponto.lng });
                        if (!pontoOrigemDentroArea)
                        {
                            pontoOrigemDentroArea = Logistica.Distancia.ValidarPoligono(pointOrigem, listaPontos.ToArray());
                            if (pontoOrigemDentroArea) break;
                        }
                        if (!pontoDestinoDentroArea)
                            pontoDestinoDentroArea = Logistica.Distancia.ValidarPoligono(pointDestino, listaPontos.ToArray());
                        break;
                }
            }

            return (pontoOrigemDentroArea && pontoDestinoDentroArea);
        }

        #endregion

        #region Métodos Públicos 

        public bool IsPossuiTrechoBalsa(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint origem, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint destino)
        {
            if (origem == null || destino == null) return false;

            Repositorio.Embarcador.Logistica.TrechoBalsa repositorioTrechoBalsa = new Repositorio.Embarcador.Logistica.TrechoBalsa(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTrechoBalsa filtro = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTrechoBalsa();
            filtro.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

            List<Dominio.Entidades.Embarcador.Logistica.TrechoBalsa> trechosBalsa = repositorioTrechoBalsa.BuscarTrechosBalsa(filtro);

            foreach (Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa in trechosBalsa)
            {
                if (OrigemDestinoEntreLocaisDeTrechoBalsa(trechoBalsa, origem, destino))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Retorna um trechoBalsa que a origem da roteirizaçõe e o destino estão dentro de areas desenhadas.
        /// Neste caso, deve-se substituir o destino da roteirização pela origem da balsa, depois de roteirizado, descifrar a polilinha
        /// e adicionar o Porto de Destino do cadastros, alem de adicionar o tempo e km do trecho balsa nos pontos da rota.
        /// </summary>
        /// <param name="origem">Ponto de origem da roteirização</param>
        /// <param name="destino">Ponto de destino.</param>
        /// <returns>Trecho de balsa quando existir</returns>
        public Dominio.Entidades.Embarcador.Logistica.TrechoBalsa TrechoBalsaRoteirizacao(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint origem, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint destino)
        {
            if (origem == null || destino == null) return null;

            Repositorio.Embarcador.Logistica.TrechoBalsa repositorioTrechoBalsa = new Repositorio.Embarcador.Logistica.TrechoBalsa(_unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTrechoBalsa filtro = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTrechoBalsa();
            filtro.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

            List<Dominio.Entidades.Embarcador.Logistica.TrechoBalsa> trechosBalsa = repositorioTrechoBalsa.BuscarTrechosBalsa(filtro);

            foreach (Dominio.Entidades.Embarcador.Logistica.TrechoBalsa trechoBalsa in trechosBalsa)
            {
                if (OrigemDestinoEntreLocaisDeTrechoBalsa(trechoBalsa, origem, destino))
                    return trechoBalsa;
            }

            return null;
        }

        #endregion
    }
}
