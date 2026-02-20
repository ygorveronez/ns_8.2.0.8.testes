using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Frotas
{
    public class FiltroPesquisaTabelaPrecoCombustivel
    {     
        public string Descricao { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoTipoOleo { get; set; }
        public DateTime DataInicioVigencia { get; set; }
    }
}
