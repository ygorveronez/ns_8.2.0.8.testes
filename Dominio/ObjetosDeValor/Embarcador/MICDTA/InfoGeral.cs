namespace Dominio.ObjetosDeValor.Embarcador.MICDTA
{
    public class InfoGeral
    {
        public string cnpjManifestador { get; set; }
        public string paisDestino { get; set; }
        public string cidadeDestino { get; set; }
        public string indTransitoAduaneiroInternacional { get; set; }
        public DocTransporte docTransporte { get; set; }
        public LocalSaida localSaida { get; set; }
        public string observacoes { get; set; }
    }
}
