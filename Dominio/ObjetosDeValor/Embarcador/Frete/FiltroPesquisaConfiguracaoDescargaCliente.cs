using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public sealed class FiltroPesquisaConfiguracaoDescargaCliente
    {
        public int CodigoFilial { get; set; }

        public double CpfCnpjCliente { get; set; }

        public Enumeradores.SituacaoAtivoPesquisa Status { get; set; }

        public Enumeradores.SituacaoAjusteConfiguracaoDescargaCliente? Situacao { get; set; }

        public bool SomenteVigentes { get; set; }

        public DateTime DataVigenciaInicial { get; set; }

        public DateTime DataVigenciaFinal { get; set; }

        public List<int> CodigosModelosVeiculares { get; set; }
    }
}
