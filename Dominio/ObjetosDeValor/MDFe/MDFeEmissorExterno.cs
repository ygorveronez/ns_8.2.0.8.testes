using System;

namespace Dominio.ObjetosDeValor.MDFe
{
    public class MDFeEmissorExterno
    {
        public Enumeradores.TipoAmbiente Ambiente { get; set; }
        public string Chave { get; set; }
        public int CodigoMunicipioEncerramento { get; set; }
        public int CodigoUFEncerramento { get; set; }
        public DateTime DataEncerramento { get; set; }
        public DateTime DataEvento { get; set; }
        public Entidades.Empresa Empresa { get; set; }
        public string Protocolo { get; set; }
        public string FusoHorario { get; set; }
    }
}
