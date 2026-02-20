using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public class ConsultaBiddingOfertaAceitamento
    {
        public int Codigo { get; set; }
        public int SituacaoEnum { get; set; }
        public int Ranking { get; set; }
        public int RotaCodigo { get; set; }
        public string ProtocoloImportacao { get; set; }
        public string RotaDescricao { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string Target { get; set; }
        public string Rodada { get; set; }
        public string Situacao { get; set; }
        public StatusBiddingRota SituacaoCodigo { get; set; }
        public string Rota { get; set; }
        public int? CodigoModeloVeicular { get; set; }
        public string ModeloVeicular { get; set; }
        public string ValorAnterior { get; set; }
        public string Distancia { get; set; }
        public string QtdAjudante { get; set; }
        public string QtdEntrega { get; set; }
        public string QtdCarga { get; set; }
        public string OrigemCidade { get; set; }
        public string OrigemUF { get; set; }
        public string OrigemRegiaoBrasil { get; set; }
        public string OrigemMesorregiao { get; set; }
        public string DestinoCidade { get; set; }
        public string DestinoUF { get; set; }
        public string DestinoRegiaoBrasil { get; set; }
        public string DestinoMesorregiao { get; set; }
        public string Quilometragem { get; set; }
        public string GrupoModeloVeicular { get; set; }
        public string Carroceria { get; set; }
        public string Tomador { get; set; }
        public string MaterialTransportado { get; set; }
        public string UnidadeNegocio { get; set; }
        public string QuantidadeViagensAno { get; set; }
        public string Volume { get; set; }
        public string Incoterm { get; set; }
        public string TempoColeta { get; set; }
        public string TempoDescarga { get; set; }
        public string Compressor { get; set; }
        public string MediaValorNF { get; set; }
        public string FilialParticipante { get; set; }
        public bool NaoOfertar { get; set; }
        public string DT_RowColor { get; set; }
        public decimal? AlicotaPadraoICMS { get; set; }

        #region Colunas Dinamicas

        public string ColunaDinamicaTransportador1 { get; set; }
        public string ColunaDinamicaTransportador2 { get; set; }
        public string ColunaDinamicaTransportador3 { get; set; }
        public string ColunaDinamicaTransportador4 { get; set; }
        public string ColunaDinamicaTransportador5 { get; set; }
        public string ColunaDinamicaTransportador6 { get; set; }
        public string ColunaDinamicaTransportador7 { get; set; }
        public string ColunaDinamicaTransportador8 { get; set; }
        public string ColunaDinamicaTransportador9 { get; set; }
        public string ColunaDinamicaTransportador10 { get; set; }
        public string ColunaDinamicaTransportador11 { get; set; }
        public string ColunaDinamicaTransportador12 { get; set; }
        public string ColunaDinamicaTransportador13 { get; set; }
        public string ColunaDinamicaTransportador14 { get; set; }
        public string ColunaDinamicaTransportador15 { get; set; }
        public string ColunaDinamicaTransportador16 { get; set; }
        public string ColunaDinamicaTransportador17 { get; set; }
        public string ColunaDinamicaTransportador18 { get; set; }
        public string ColunaDinamicaTransportador19 { get; set; }
        public string ColunaDinamicaTransportador20 { get; set; }

        public string ComponenteOfertado1Transportador1 { get; set; }
        public string ComponenteOfertado1Transportador2 { get; set; }
        public string ComponenteOfertado1Transportador3 { get; set; }
        public string ComponenteOfertado1Transportador4 { get; set; }
        public string ComponenteOfertado1Transportador5 { get; set; }
        public string ComponenteOfertado1Transportador6 { get; set; }
        public string ComponenteOfertado1Transportador7 { get; set; }
        public string ComponenteOfertado1Transportador8 { get; set; }
        public string ComponenteOfertado1Transportador9 { get; set; }
        public string ComponenteOfertado1Transportador10 { get; set; }
        public string ComponenteOfertado1Transportador11 { get; set; }
        public string ComponenteOfertado1Transportador12 { get; set; }
        public string ComponenteOfertado1Transportador13 { get; set; }
        public string ComponenteOfertado1Transportador14 { get; set; }
        public string ComponenteOfertado1Transportador15 { get; set; }
        public string ComponenteOfertado1Transportador16 { get; set; }
        public string ComponenteOfertado1Transportador17 { get; set; }
        public string ComponenteOfertado1Transportador18 { get; set; }
        public string ComponenteOfertado1Transportador19 { get; set; }
        public string ComponenteOfertado1Transportador20 { get; set; }

        public string ComponenteOfertado2Transportador1 { get; set; }
        public string ComponenteOfertado2Transportador2 { get; set; }
        public string ComponenteOfertado2Transportador3 { get; set; }
        public string ComponenteOfertado2Transportador4 { get; set; }
        public string ComponenteOfertado2Transportador5 { get; set; }
        public string ComponenteOfertado2Transportador6 { get; set; }
        public string ComponenteOfertado2Transportador7 { get; set; }
        public string ComponenteOfertado2Transportador8 { get; set; }
        public string ComponenteOfertado2Transportador9 { get; set; }
        public string ComponenteOfertado2Transportador10 { get; set; }
        public string ComponenteOfertado2Transportador11 { get; set; }
        public string ComponenteOfertado2Transportador12 { get; set; }
        public string ComponenteOfertado2Transportador13 { get; set; }
        public string ComponenteOfertado2Transportador14 { get; set; }
        public string ComponenteOfertado2Transportador15 { get; set; }
        public string ComponenteOfertado2Transportador16 { get; set; }
        public string ComponenteOfertado2Transportador17 { get; set; }
        public string ComponenteOfertado2Transportador18 { get; set; }
        public string ComponenteOfertado2Transportador19 { get; set; }
        public string ComponenteOfertado2Transportador20 { get; set; }

        public string ComponenteOfertado3Transportador1 { get; set; }
        public string ComponenteOfertado3Transportador2 { get; set; }
        public string ComponenteOfertado3Transportador3 { get; set; }
        public string ComponenteOfertado3Transportador4 { get; set; }
        public string ComponenteOfertado3Transportador5 { get; set; }
        public string ComponenteOfertado3Transportador6 { get; set; }
        public string ComponenteOfertado3Transportador7 { get; set; }
        public string ComponenteOfertado3Transportador8 { get; set; }
        public string ComponenteOfertado3Transportador9 { get; set; }
        public string ComponenteOfertado3Transportador10 { get; set; }
        public string ComponenteOfertado3Transportador11 { get; set; }
        public string ComponenteOfertado3Transportador12 { get; set; }
        public string ComponenteOfertado3Transportador13 { get; set; }
        public string ComponenteOfertado3Transportador14 { get; set; }
        public string ComponenteOfertado3Transportador15 { get; set; }
        public string ComponenteOfertado3Transportador16 { get; set; }
        public string ComponenteOfertado3Transportador17 { get; set; }
        public string ComponenteOfertado3Transportador18 { get; set; }
        public string ComponenteOfertado3Transportador19 { get; set; }
        public string ComponenteOfertado3Transportador20 { get; set; }

        public string ComponenteOfertado4Transportador1 { get; set; }
        public string ComponenteOfertado4Transportador2 { get; set; }
        public string ComponenteOfertado4Transportador3 { get; set; }
        public string ComponenteOfertado4Transportador4 { get; set; }
        public string ComponenteOfertado4Transportador5 { get; set; }
        public string ComponenteOfertado4Transportador6 { get; set; }
        public string ComponenteOfertado4Transportador7 { get; set; }
        public string ComponenteOfertado4Transportador8 { get; set; }
        public string ComponenteOfertado4Transportador9 { get; set; }
        public string ComponenteOfertado4Transportador10 { get; set; }
        public string ComponenteOfertado4Transportador11 { get; set; }
        public string ComponenteOfertado4Transportador12 { get; set; }
        public string ComponenteOfertado4Transportador13 { get; set; }
        public string ComponenteOfertado4Transportador14 { get; set; }
        public string ComponenteOfertado4Transportador15 { get; set; }
        public string ComponenteOfertado4Transportador16 { get; set; }
        public string ComponenteOfertado4Transportador17 { get; set; }
        public string ComponenteOfertado4Transportador18 { get; set; }
        public string ComponenteOfertado4Transportador19 { get; set; }
        public string ComponenteOfertado4Transportador20 { get; set; }

        public string ComponenteOfertado5Transportador1 { get; set; }
        public string ComponenteOfertado5Transportador2 { get; set; }
        public string ComponenteOfertado5Transportador3 { get; set; }
        public string ComponenteOfertado5Transportador4 { get; set; }
        public string ComponenteOfertado5Transportador5 { get; set; }
        public string ComponenteOfertado5Transportador6 { get; set; }
        public string ComponenteOfertado5Transportador7 { get; set; }
        public string ComponenteOfertado5Transportador8 { get; set; }
        public string ComponenteOfertado5Transportador9 { get; set; }
        public string ComponenteOfertado5Transportador10 { get; set; }
        public string ComponenteOfertado5Transportador11 { get; set; }
        public string ComponenteOfertado5Transportador12 { get; set; }
        public string ComponenteOfertado5Transportador13 { get; set; }
        public string ComponenteOfertado5Transportador14 { get; set; }
        public string ComponenteOfertado5Transportador15 { get; set; }
        public string ComponenteOfertado5Transportador16 { get; set; }
        public string ComponenteOfertado5Transportador17 { get; set; }
        public string ComponenteOfertado5Transportador18 { get; set; }
        public string ComponenteOfertado5Transportador19 { get; set; }
        public string ComponenteOfertado5Transportador20 { get; set; }

        #endregion
    }
}
