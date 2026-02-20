using System;

namespace Dominio.ObjetosDeValor.Embarcador.Administrativo
{
    public class FiltroPesquisaRelatorioLogEnvioEmail
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int CodigoEmpresa { get; set; }
    }
}
