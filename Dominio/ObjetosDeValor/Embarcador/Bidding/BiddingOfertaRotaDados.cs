namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public class BiddingOfertaRotaDados
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string FiliaisCodigos { get; set;}
        public string FiliaisDescricoes { get; set; }
        public string OrigensCodigos { get; set; }
        public string OrigensRegiaoCodigo { get; set; }
        public string OrigensRegiaoDescricao { get; set; }
        public string OrigensDescricoes { get; set; }
        public string DestinosCodigos { get; set; }
        public string DestinosRegiaoCodigo { get; set; }
        public string DestinosRegiaoDescricao { get; set; }
        public string DestinosDescricoes { get; set; }
        public string RegioesDestinoCodigos { get; set; }
        public string RegioesDestinoDescricoes { get; set; }
        public string RegioesOrigemCodigos { get; set; }
        public string RegioesOrigemDescricoes { get; set; }
        public string ModelosVeicularesCodigos { get; set; }
        public string ModelosVeicularesDescricoes { get; set; }
        public string MesorregioesDestinoCodigo { get; set; }
        public string MesorregioesOrigemCodigo { get; set; }
        public string MesorregioesOrigem { get; set;}
        public string MesorregioesDestino { get; set; }
        public string ClienteDestinoCodigo { get; set;}
        public string ClienteDestinoRegiaoCodigo { get; set;}
        public string ClienteDestinoRegiaoDescricao { get; set;}
        public string ClienteDestino { get; set; }
        public string ClienteOrigemCodigo { get; set; }
        public string ClienteOrigemRegiaoCodigo { get; set; }
        public string ClienteOrigemRegiaoDescricao { get; set; }
        public string ClienteOrigem { get; set; }
        public string RotaDestinoCodigo { get; set;}
        public string RotaDestinoRegiaoCodigo { get; set;}
        public string RotaDestinoRegiaoDescricao { get; set;}
        public string RotaDestino { get; set; }
        public string RotaOrigemCodigo { get; set; }
        public string RotaOrigemRegiaoCodigo { get; set; }
        public string RotaOrigemRegiaoDescricao { get; set; }
        public string RotaOrigem { get; set; }
        public string EstadoDestinoCodigo { get; set;}
        public string EstadoDestinoRegiaoCodigo { get; set;}
        public string EstadoDestinoRegiaoDescricao { get; set;}
        public string EstadoDestino { get; set; }
        public string EstadoOrigemCodigo { get; set; }
        public string EstadoOrigemRegiaoCodigo { get; set; }
        public string EstadoOrigemRegiaoDescricao { get; set; }
        public string EstadoOrigem { get; set; }
        public string PaisDestinoCodigo { get; set;}
        public string PaisDestino { get; set; }
        public string PaisOrigemCodigo { get; set; }
        public string PaisOrigem { get; set; }

        public int PossuiCEPDestino { get; set; }
        public int PossuiCEPOrigem { get; set; }

        public int QuantidadeEntregas { get; set; }
        public int QuantidadeAjudantes { get; set; }
        public int QuantidadeViagensAno { get; set; }
    }
}
