using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaDocumentosConciliacao
    {
        public int NumeroCTe { get; set; }
        public int CodigoEmpresa { get; set; }
        public string CodigoCargaEmbarcador { get; set; }

        public int NumeroFatura { get; set; }
        public StatusTitulo StatusTitulo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int CodigoFilial { get; set; }
        public double CnpjCpfRemetente { get; set; }

        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
    }
}
