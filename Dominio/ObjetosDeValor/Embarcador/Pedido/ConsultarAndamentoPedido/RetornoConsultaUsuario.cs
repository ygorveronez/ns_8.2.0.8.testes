using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido
{
    public class RetornoUsuario
    {
        public List<Usuario> ITEM { get; set; }
    }

    public class Usuario
    {
        public string codUsuario { get; set; }
        public string Empresa { get; set; }
        public string nomeUsuario { get; set; }
        public string tipoUsuario { get; set; }
        public string eMail { get; set; }
        public bool userCadastrado { get; set; }
        public List<RETURN> RETURN { get; set; }
    }

    public class RETURN
    {
        public string descMessagem { get; set; }
        public string tipoMsg { get; set; }
        public string classMsgSap { get; set; }
        public string numMsgSap { get; set; }
        public string msgSap1 { get; set; }
        public string msgSap2 { get; set; }
        public string msgSap3 { get; set; }
        public string msgSap4 { get; set; }
    }

}
