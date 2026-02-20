namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class TipoContainer
    {
        public int Codigo { get; set; }
        public string CodigoDocumento { get; set; }
        public int CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public bool Atualizar { get; set; }
        public bool InativarCadastro { get; set; }

        public decimal MetrosCubicos { get; set; }
        public decimal Tara { get; set; }
        public decimal PesoLiquido { get; set; }
        public string TEU { get; set; }
        public string FFE { get; set; }
        public decimal PesoMaximo { get; set; }
    }
}
