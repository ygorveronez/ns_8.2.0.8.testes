using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{
    public class retTransportador
    {
        public int id { get; set; }
        public int? embarcadorID { get; set; }
        public string documento { get; set; }
        public string documentoTipo { get; set; }
        public enumPessoaDocumentoTipo documentoTipoID { get; set; }
        public string razaoSocial { get; set; }
        public string nomeFantasia { get; set; }
        public string nome { get; set; }
        public string inscricaoMunicipal { get; set; }
        public string inscricaoEstadual { get; set; }
        public bool inssSimplificado { get; set; }
        public string numeroRNTRC { get; set; }
        public string dataEmissaoRNTRC { get; set; }
        public string dataVencimentoRNTRC { get; set; }
        public string dataUltimaConsultaRNTRC { get; set; }
        public bool validadoANTT { get; set; }
        public int? numeroDependente { get; set; }
        public bool ativo { get; set; }
        public int? statusContaID { get; set; }
        public string statusConta { get; set; }
        public string tipoRNTRC { get; set; }
        public string numeroCartao { get; set; }
        public string NumeroCartaoCombustivel { get; set; }
        public int? CartaoID { get; set; }
        public int? CartaoCombustivelID { get; set; }
        public int? PessoaID { get; set; }
        public List<TransportadorContato> contatos { get; set; }
        public TransportadorEmbarcador embarcador { get; set; }
        public TransportadorPessoa pessoa { get; set; }
    }

    public class TransportadorEmbarcador
    {
        public int? id { get; set; }
        public int? pessoaID { get; set; }
        public int? tarifaDeSaqueID { get; set; }
        public bool tms { get; set; }
        public string tmsNome { get; set; }
        public string dataInclusao { get; set; }
        public string dataAtualizacao { get; set; }
        public int? tipoCliente { get; set; }
        public bool ativo { get; set; }
        public int? statusContaID { get; set; }
        public string idContaExterno { get; set; }
        public string idPersonExterno { get; set; }
        public string numeroRNTRC { get; set; }
        public TransportadorPessoa pessoa { get; set; }
    }

    public class TransportadorPessoa 
    {
        public int? id { get; set; }
        public string documento { get; set; }
        public enumPessoaDocumentoTipo pessoaDocumentoTipoID { get; set; }
        public string razaoSocial { get; set; }
        public string nome { get; set; }
        public string nomeFantasia { get; set; }
        public string dataInclusao { get; set; }
        public string dataAtualizacao { get; set; }
        public string inscricaoMunicipal { get; set; }
        public string inscricaoEstadual { get; set; }
        public string dataInativacao { get; set; }
    }
}
