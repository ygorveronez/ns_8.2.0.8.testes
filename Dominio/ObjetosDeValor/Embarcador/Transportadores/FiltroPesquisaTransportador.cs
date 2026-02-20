using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Transportadores
{
    public sealed class FiltroPesquisaTransportador
    {
        public FiltroPesquisaTransportador()
        {
            TipoAmbiente = Dominio.Enumeradores.TipoAmbiente.Producao;
            SistemaEmissor = Enumeradores.SistemaEmissor.Todos;
        }

        public List<int> Codigos { get; set; }

        public int CodigoEmpresa { get; set; }

        public int CodigoFilial { get; set; }

        public string RazaoSocial { get; set; }

        public string NomeFantasia { get; set; }

        public string CNPJ { get; set; }

        public bool BuscarFiliais { get; set; }

        public string PlacaVeiculo { get; set; }

        public bool SomenteProducao { get; set; }

        public Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }

        public bool SemEmpresaPai { get; set; }

        public Enumeradores.SistemaEmissor SistemaEmissor { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa SituacaoPesquisa { get; set; }

        public string UFFilialTransportador { get; set; }

        public bool StatusTodos { get; set; }

        public string RaizCnpj { get; set; }

        public List<int> ListaCodigoTransportadorPermitidos { get; set; }

        public List<int> CodigosEmpresa { get; set; }
        public List<int> CodigosTransportadores { get; set; }

        public bool SomenteEmpresaNaoTransportadoraPadraoContratacao { get; set; }
        public bool SomenteTransportadoresPermitidosCadastroAgendamentoColeta { get; set; }
        public int CodigoEmpresaTransportadoraPadraoContratacao { get; set; }
        public bool SomenteSubEmpresasTransportadoraPadraoContratacao { get; set; }
        public bool SomenteTransportadoresManuais { get; set; }
        public int CodigoEmpresaMatriz { get; set; }
        public bool TelaMontagemCargaMapa { get; set; }
        public bool SomenteTransportadorHabilitadoTransportarParaFilial { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Localidade { get; set; }
        public string Estado { get; set; }

        public int CodigoTipoOperacao { get; set; }
        public int CodigoGrupoTransportador { get; set; }
    }
}
