using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.ModeloDados
{
    public class Entrega
    {
        public int Codigo { get; set; }

        public string SequenciaPrevista { get; set; }

        public string SequenciaRealizada { get; set; }

        public string Tipo { get; set; }

        public string Situacao { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        public DateTime? DataEntradaRaio { get; set; }

        public DateTime? DataSaidaRaio { get; set; }

        public DateTime? DataProgramada { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string LatitudeChegada { get; set; }

        public string LongitudeChegada { get; set; }

        public string LatitudeFinalizada { get; set; }

        public string LongitudeFinalizada { get; set; }

        public List<NotaFiscal> NotasFiscais { get; set; }

        public int Distancia { get; set; }

        public Cliente Cliente { get; set; }

        public Endereco ClienteOutroEndereco { get; set; }

        public Localidade Localidade { get; set; }

        public List<EntregaPedido> Pedidos { get; set; }

        public List<Canhoto> Canhotos { get; set; }

        public List<CheckList> CheckLists { get; set; }

        public EntregaDatasPrevistas DatasPrevistas { get; set; }
    }
}
