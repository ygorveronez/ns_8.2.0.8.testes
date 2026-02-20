using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class Resultado
    {
        public int idViagem { get; set; }
        public string CIOT { get; set; }
        public decimal idCargaCartao { get; set; }
        public int idRota { get; set; }
        public decimal pedagioTotal { get; set; }
        public bool isRNTRCAtivo { get; set; }
        public List<tag> TAGs { get; set; }
        public utilizavelCom utilizavelCom { get; set; }
        public dadosReciboPedagio dadosReciboPedagio { get; set; }
    }

    public class utilizavelCom
    {
        public bool? TAG_SemParar { get; set; }
    }

    public class tag
    {
        public bool? isAptoPedagio { get; set; }
    }

    public class dadosReciboPedagio
    {
        public decimal valorTotalPedagio { get; set; }
        public bool isPedagioAvulso { get; set; }
        public string contrato { get; set; }
        public string idVpo { get; set; }
    }
}