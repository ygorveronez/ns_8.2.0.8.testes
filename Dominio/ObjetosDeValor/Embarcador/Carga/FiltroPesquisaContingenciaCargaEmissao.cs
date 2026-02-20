using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaContingenciaCargaEmissao
    {
        public string NumeroCarga { get; set; }
        public DateTime DataCriacaoInicial {  get; set; }
        public DateTime DataCriacaoFinal {  get; set; }
        public List<SituacaoCarga> SituacaoCarga {  get; set; }
        public List<int> CodigosFilial { get; set; }
        public List<int> CodigosEmpresa { get; set; }
        public List<int> CodigosTipoCarga { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public bool CargasEmContingencia { get; set; }
    }
}
