using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Repom
{
    [XmlRoot("dados_carga")]
    public class ContratoFreteDadosCarga
    {
        [XmlElement("carga_destinatario_cpf_cnpj")]
        public string CPFCNPJDestinatario { get; set; }

        [XmlElement("carga_destinatario_nome_razao_social")]
        public string NomeDestinatario { get; set; }

        [XmlElement("antt_ncm_codigo_classificacao_mercadoria")]
        public string CodigoNCMMercadoria { get; set; }

        [XmlElement("carga_peso")]
        public string Peso { get; set; }

        [XmlElement("carga_volume")]
        public string Volume { get; set; }

        [XmlElement("carga_valor")]
        public string ValorMercadoria { get; set; }

        /// <summary>
        /// 0 – Sem Descrição 
        /// 1 – Caixa(s) 
        /// 2 – Lata(s) 
        /// 3 – PET(s) 
        /// 4 – Pallet(s) 
        /// 5 – Rack(s) 
        /// 6 – Fardo(s) 
        /// 7 – Kg 
        /// 8 – Saco(s) 
        /// 9 – Toneladas(s) 
        /// 10 – Metros Cubicos 
        /// 11 – Unidades(s) 
        /// 12 – Litro(s)
        /// </summary>
        [XmlElement("carga_unidade_codigo")]
        public string UnidadeMedida { get; set; }

        /// <summary>
        /// 01 - Carga Geral
        /// 02 - Carga a Granel
        /// 03 - Carga Frigorificada
        /// 04 - Carga Perigosa
        /// 05 - Carga Neogranel
        /// </summary>
        [XmlElement("codigo_tipo_carga")]
        public string CodigoTipoCarga { get; set; }
    }
}
