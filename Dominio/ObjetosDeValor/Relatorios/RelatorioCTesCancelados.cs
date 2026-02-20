using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioCTesCancelados
    {
        public int CodigoCTe { get; set; }
        public int CodigoEmpresa { get; set; }
        public string NomeEmpresa { get; set; }
        public int NumeroCTe { get; set; }
        public int Serie { get; set; }
        public DateTime? DataEmissao { get; set; }
        public string HistoricoDeAlteracoes { get; set; }
        public string Justificativa { get; set; }
        public string Cobrar { get; set; }
    }
}
