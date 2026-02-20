using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento
{
    public class cteSimplifiedDataCargoInformation
    {
        /// <summary>
        /// Total Cargo Value(tag vCarga)
        /// </summary>
        public decimal cargoAmount { get; set; }

        /// <summary>
        /// Predominant Product(tag proPred)
        /// </summary>
        public string predominantProduct { get; set; }

        /// <summary>
        /// Other Cargo Characteristics(tag xOutCat)
        /// </summary>
        public string otherCharacteristics { get; set; }

        /// <summary>
        /// Cargo Quantity Information of the CTe(tag infQ)
        /// </summary>
        public List<cteSimplifiedDataCargoInformationQuantity> quantity { get; set; }

        /// <summary>
        /// Cargo Value for Insurance Purposes(tag vCargaAverb)
        /// </summary>
        public decimal endorsementAmount { get; set; }
    }

    public class cteSimplifiedDataCargoInformationQuantity
    {
        /// <summary>
        /// <para>Unit of Measure Code(tag cUnid)</para>
        /// <para>m3</para>
        /// <para>kg</para>
        /// <para>ton</para>
        /// <para>unidade</para>
        /// <para>litros</para>
        /// <para>mmbtu</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumQuantityUnit unit { get; set; }

        /// <summary>
        /// <para>Type of Measurement(tag tpMed)</para>
        /// <para>cubagem_nfe</para>
        /// <para>cubagem_aferida_transportador</para>
        /// <para>peso_bruto_nfe</para>
        /// <para>peso_bruto_aferido_transportador</para>
        /// <para>peso_cubado</para>
        /// <para>peso_base_calculo_frete</para>
        /// <para>peso_uso_operacional</para>
        /// <para>caixas</para>
        /// <para>paletes</para>
        /// <para>sacas</para>
        /// <para>containers</para>
        /// <para>rolos</para>
        /// <para>bombonas</para>
        /// <para>latas</para>
        /// <para>litragem</para>
        /// <para>milhao_btu</para>
        /// <para>outros</para>
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enumTypeMeasure typeMeasure { get; set; }

        /// <summary>
        /// Quantity(tag qCarga)
        /// </summary>
        public decimal quantity { get; set; }
    }
}