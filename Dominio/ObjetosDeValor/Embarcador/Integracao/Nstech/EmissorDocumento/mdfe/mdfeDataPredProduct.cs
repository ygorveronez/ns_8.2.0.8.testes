using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class mdfeDataPredProduct
    {
        /// <summary>
        /// Tag tpCarga
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumLoadType loadType { get; set; }

        /// <summary>
        /// Tag xProd
        /// </summary>
        public string loadDesc { get; set; }

        /// <summary>
        /// Tag cEAN
        /// </summary>
        public string cEAN { get; set; }

        /// <summary>
        /// Tag NCM
        /// </summary>
        public string NCM { get; set; }

        /// <summary>
        /// Tag infLotacao
        /// </summary>
        public mdfeDataPredProductInfCargoLoad infCargoLoad { get; set; }
    }

    public class mdfeDataPredProductInfCargoLoad
    {
        /// <summary>
        /// <para>Tag infLocalCarrega</para>
        /// <para>Objetos: mdfeDataPredProductInfCargoLoadPlaceInfoLatLong, mdfeDataPredProductInfCargoLoadPlaceInfoZipcode</para>
        /// </summary>
        public object loadPlaceInfo { get; set; }

        /// <summary>
        /// <para>Tag infLocalDescarrega</para>
        /// <para>Objetos: mdfeDataPredProductInfCargoLoadPlaceInfoLatLong, mdfeDataPredProductInfCargoLoadPlaceInfoZipcode</para>
        /// </summary>
        public object unloadPlaceInfo { get; set; }
    }

    public class mdfeDataPredProductInfCargoLoadPlaceInfoLatLong
    {
        /// <summary>
        /// latLong
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumPlaceType type { get; } = enumPlaceType.latLong;

        /// <summary>
        /// Tag latitude
        /// </summary>
        public string latitude { get; set; }

        /// <summary>
        /// Tag longitude
        /// </summary>
        public string longitude { get; set; }
    }

    public class mdfeDataPredProductInfCargoLoadPlaceInfoZipcode
    {
        /// <summary>
        /// zipcode
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumPlaceType type { get; } = enumPlaceType.zipcode;

        /// <summary>
        /// Tag CEP
        /// </summary>
        public string zipCode { get; set; }
    }
}