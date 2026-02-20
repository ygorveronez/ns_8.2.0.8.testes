using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc
{
    public class RecebimentoPedidoExtra
    {
        //Tipo Registro 01
        public string NumeroSolicitacao { get; set; }
        public string TipoGeracaoSolicitacao { get; set; }
        public string TipoSolicitacao { get; set; }
        public string Planta { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataColeta { get; set; }
        public string ResponsavelSolicitacao { get; set; }
        public string TipoVeiculo { get; set; }
        public string Transportadora { get; set; }
        public string Conta { get; set; }
        public string CentroDeCusto { get; set; }
        public string FornecedorResponsavel { get; set; }
        public string Motivo { get; set; }
        public string Consolidar { get; set; }
        public string Reservado { get; set; }

        //Tipo Registro 2
        public List<DadosRecebimentoPedidoExtraProduto> Produtos { get; set; }

        //Tipo Registro 3
        public List<DadosRecebimentoPedidoExtraEmbalagem> Embalagens { get; set; }
    }
}
