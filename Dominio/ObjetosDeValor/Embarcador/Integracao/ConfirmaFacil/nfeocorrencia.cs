using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil
{
    public class nfeocorrencia
    {
        public Embarque embarque { get; set; }
        public Embarcador embarcador { get; set; }
        public Transportadora transportadora { get; set; }
        public Ocorrencia ocorrencia { get; set; }
    }

    public class Embarque
    {
        public string numero { get; set; }
        public string serie { get; set; }
    }

    public class Embarcador
    {
        public string cnpj { get; set; }
    }

    public class Transportadora
    {
        public string nome { get; set; }
        public string cnpj { get; set; }
    }

    public class Ocorrencia
    {
        public string tipoEntrega { get; set; }
        public string dtOcorrencia { get; set; }
        public string hrOcorrencia { get; set; }        
        public string[] fotos { get; set; }
    }

}
