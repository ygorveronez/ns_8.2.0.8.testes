namespace Dominio.ObjetosDeValor.WebServiceCarrefour.Frota
{
    public sealed class Modelo
    {
        public string Descricao { get; set; }

        public string CodigoIntegracao { get; set; }

        public int NumeroEixos { get; set; }

        public bool PossuiArla32 { get; set; }

        public bool Ativo { get; set; }

        public string CodigoFIPE { get; set; }

        public Marca Marca { get; set; }
    }
}
