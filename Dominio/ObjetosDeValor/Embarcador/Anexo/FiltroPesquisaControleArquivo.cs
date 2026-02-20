using System;

namespace Dominio.ObjetosDeValor.Embarcador.Anexo
{
    public sealed class FiltroPesquisaControleArquivo
    {
        public double CpfCnpjPessoa { get; set; }
        public int CodigoEmpresa { get; set; }
        public string Descricao { get; set; }
        public DateTime DataVencimentoInicial { get; set; }
        public DateTime DataVencimentoFinal { get; set; }
    }
}
