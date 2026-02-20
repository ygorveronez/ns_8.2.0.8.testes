using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb
{
    public class envParticipante
    {
        public string Cidade { get; set; }
        public string CPF_CNPJ { get; set; }
        public string CodigoExterno { get; set; }
        public bool? Condutor { get; set; }
        public string Nome { get; set; }
        public int? UF { get; set; }
        public envComplemento Complemento { get; set; }
        public envEndereco Endereco { get; set; }
        public envJuridicaComplemento JuridicaComplemento { get; set; }
    }
}
