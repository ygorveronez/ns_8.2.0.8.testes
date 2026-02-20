using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.WebService.TicketLog
{
    public class Abastecimento
    {
        public bool Sucesso { get; set; }
        public DateTime DataChamada { get; set; }
        public List<Retorno> Transacoes { get; set; }
    }      

    public class Retorno
    {
        public string CodigoTransacao { get; set; }
        public string ValorTransacao { get; set; }
        public string DataTransacao { get; set; }
        public string CnpjEstabelecimento { get; set; }
        public string Litros { get; set; }
        public string TipoCombustivel { get; set; }
        public string Placa { get; set; }
        public string ValorLitro { get; set; }
        public string Quilometragem { get; set; }
    }

}
