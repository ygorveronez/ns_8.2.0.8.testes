namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class Quitacao
    {
        public decimal pesoTotalChegadaKg { get; set; }
        public decimal valorTotalAvarias { get; set; }
        public bool documentosEntregues { get; set; }
        public string observacoes { get; set; }
    }
}
