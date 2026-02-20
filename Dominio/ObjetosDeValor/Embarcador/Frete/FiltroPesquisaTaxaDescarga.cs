using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaTaxaDescarga
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataLimite { get; set; }
        public int CodigoFilial { get; set; }
        public decimal Valor { get; set; }
        public SituacaoAjusteConfiguracaoDescargaCliente? Situacao { get; set; }
        public int CodigoOperador { get; set; }
        public List<double> CpfCnpjClientes { get; set; }
        public int CodigoUsuario { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoModeloVeicular { get; set; }
    }
}
