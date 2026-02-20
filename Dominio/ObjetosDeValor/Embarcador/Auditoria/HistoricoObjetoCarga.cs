namespace Dominio.ObjetosDeValor.Embarcador.Auditoria
{
    public sealed class HistoricoObjetoCarga
    {
        public string Acao { get; set; }

        public string Carga { get; set; }

        public long Codigo { get; set; }

        public string Data { get; set; }

        public string Integradora { get; set; }

        public string IP { get; set; }
    }
}
