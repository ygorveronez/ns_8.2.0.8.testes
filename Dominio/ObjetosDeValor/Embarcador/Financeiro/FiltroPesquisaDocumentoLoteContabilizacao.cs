using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaDocumentoLoteContabilizacao
    {
        public int CodigoLoteContabilizacao { get; set; }

        public int CodigoEmpresa { get; set; }

        public int CodigoModeloDocumentoFiscal { get; set; }

        public double CpfCnpjTomador { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataLimite { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoExportacao? Tipo { get; set; }

        public bool? SelecionarTodos { get; set; }

        public List<int> CodigosSelecionados { get; set; }

        public string NumeroDocumento { get; set; }
    }
}
