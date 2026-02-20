using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{
    public enum enumPessoaMotoristaDocumentoTipo
    {
        CPF = 1,
        CNPJ = 2,
        RG = 3,
        CNH = 4
    }

    public class envMotorista
    {
        public int? id { get; set; }
        public int? pessoaID { get; set; }
        public string nome { get; set; }
        public string documento { get; set; }
        public string dataInclusao { get; set; }
        public enumPessoaMotoristaDocumentoTipo pessoaDocumentoTipoID { get; set; }
        public int? cartaoID { get; set; }
        public int? cartaoCombustivelID { get; set; }
        public List<motorista> motorista { get; set; }
        public List<pessoaDocumento> pessoaDocumentos { get; set; }
        public List<pessoaContato> pessoaContatos { get; set; }
    }

    public class motorista
    {
        public int? id { get; set; }
        public int? pessoaID { get; set; }
        public string sexo { get; set; }
        public string nomeMae { get; set; }
        public string nomePai { get; set; }
        public string dataNascimento { get; set; }
        public string dataInclusao { get; set; }
        public string naturalidade { get; set; }
        public string ufNascimento { get; set; }
        public int? transportadorID { get; set; }
    }

    public class pessoaDocumento
    {
        public int? id { get; set; }
        public int? pessoaID { get; set; }
        public enumPessoaMotoristaDocumentoTipo pessoaDocumentoTipoID { get; set; }
        public string numeroDocumento { get; set; }
        public string dataEmissao { get; set; }
        public string dataInclusao { get; set; }
        public string orgaoEmissor { get; set; }
        public string ufEmissor { get; set; }
        public string dataValidade { get; set; }
    }

    public class pessoaContato
    {
        public int? id { get; set; }
        public int? pessoaID { get; set; }
        public string email { get; set; }
        public int? codigoPais { get; set; }
        public string celular1 { get; set; }
        public string celular2 { get; set; }
        public string telefone1 { get; set; }
        public string telefone2 { get; set; }
    }
}