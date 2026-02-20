using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class FiltroPesquisaConfiguracaoTabelaFrete
    {
        public bool? TabelasVigentes { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public List< int> CodigoGrupoPessoas { get; set; }
        public bool? Situacao { get; set; }
    }
}
