using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{
    public enum enumPessoaDocumentoTipo
    {
        CPF = 1,
        CNPJ = 2
    }

    public class envTransportador
    {
        public int? id { get; set; }
        public string documento { get; set; }
        public enumPessoaDocumentoTipo documentoTipoID { get; set; }
        public string razaoSocial { get; set; }
        public string nomeFantasia { get; set; }
        public string inscricaoMunicipal { get; set; }
        public string inscricaoEstadual { get; set; }
        public bool inssSimplificado { get; set; }
        public string numeroRNTRC { get; set; }
        public string dataEmissaoRNTRC { get; set; }
        public string dataVencimentoRNTRC { get; set; }
        public string dataUltimaConsultaRNTRC { get; set; }
        public bool validadoANTT { get; set; }
        public string numeroDependente { get; set; }
        public string tipoRNTRC { get; set; }
        public int? cartaoID { get; set; }
        public int? CartaoCombustivelID { get; set; }
        public List<TransportadorContato> contatos { get; set; }
    }

    public class TransportadorContato
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string area { get; set; }
        public string email { get; set; }
        public string codigoPais { get; set; }
        public string celular1 { get; set; }
        public string celular2 { get; set; }
        public string telefone1 { get; set; }
        public string telefone2 { get; set; }
    }
}