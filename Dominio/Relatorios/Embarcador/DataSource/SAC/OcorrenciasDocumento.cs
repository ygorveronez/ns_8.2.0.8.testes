using System;

namespace Dominio.Relatorios.Embarcador.DataSource.SAC
{
    public class OcorrenciasDocumento
    {
        public int Codigo { get; set; }
        public int CodigoCTe { get; set; }
        public string TipoOcorrencia { get; set; }
        public string Observacao { get; set; }
        public DateTime DataOcorrencia { get; set; }
    }
}
