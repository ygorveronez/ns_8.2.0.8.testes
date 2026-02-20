namespace Dominio.ObjetosDeValor.Embarcador.Integracao.SkyWorld
{
    public class RetornoRequestToken
    {
        public string Status { get; set; }
        public Dados Result { get; set; }

        public bool Ok => Status == "ok";
    }
}
