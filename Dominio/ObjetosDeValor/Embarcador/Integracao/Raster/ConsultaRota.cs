namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Raster
{
    public class ConsultaRota
    {
        public string Ambiente { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string TipoRetorno { get; set; }
        public int Codigo { get; set; }
        public int CodIBGECidadeOrigem { get; set; }
        public int CodIBGECidadeDestino { get; set; }
        public string DevolverKML { get; set; }
        public string DetalharRota { get; set; }
        public string CriarSeNaoExistir { get; set; }
    }
}
