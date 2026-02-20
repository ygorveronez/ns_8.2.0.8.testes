using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar
{
    public class retMotorista
    {
        public int? id;
        public int? pessoaID { get; set; }
        public string nome { get; set; }
        public string documento { get; set; }
        public string dataInclusao { get; set; }
        public enumPessoaMotoristaDocumentoTipo pessoaDocumentoTipoID { get; set; }
        public List<motorista> motorista { get; set; }
    }
}