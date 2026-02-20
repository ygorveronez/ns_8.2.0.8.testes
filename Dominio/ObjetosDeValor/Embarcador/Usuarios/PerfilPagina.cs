namespace Dominio.ObjetosDeValor.Embarcador.Usuarios
{
    public class PerfilPagina
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public Modulo Modulo { get; set; }
        public bool ApenasLeitura { get; set; }
    }
}
