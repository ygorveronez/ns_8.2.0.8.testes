using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas
{
    public class FiltroPesquisaParametrosOfertas
    {
        public virtual string Descricao { get; set; }

        public virtual string CodigoIntegracao { get; set; }

        public virtual bool? Ativo { get; set; }
    }
}
