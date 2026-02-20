using System.Collections.Generic;

namespace Dominio.ObjetosDeValor
{
    public class MunicipioDescarregamentoMDFe
    {
        public int Codigo { get; set; }

        public int CodigoMunicipio { get; set; }

        public string DescricaoMunicipio { get; set; }

        public List<Dominio.ObjetosDeValor.DocumentoMunicipioDescarregamentoMDFe> Documentos { get; set; }

        public List<Dominio.ObjetosDeValor.NotaFiscalEletronicaMDFe> NFes { get; set; }

        public List<Dominio.ObjetosDeValor.ChaveCTes> ChaveCTes { get; set; }

        public bool Excluir { get; set; }
    }
}
