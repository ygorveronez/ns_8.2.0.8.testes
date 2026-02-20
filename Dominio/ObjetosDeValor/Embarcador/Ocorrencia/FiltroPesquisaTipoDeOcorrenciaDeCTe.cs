using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public sealed class FiltroPesquisaTipoDeOcorrenciaDeCTe
    {
        public string Descricao { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public int CodigoGrupoPessoasIgual { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public double CpfCnpjPessoa { get; set; }
        public double CpfCnpjPessoaIgual { get; set; }
        public bool FiltrarContratoFrete { get; set; }
        public OrigemOcorrencia? OrigemOcorrencia { get; set; }
        public TipoAplicacaoColetaEntrega TipoAplicacaoColetaEntrega { get; set; }
        public bool TipoOcorrenciaControleEntrega { get; set; }
        public List<FinalidadeTipoOcorrencia?> Finalidades { get; set; }
        public bool ValidarSomenteDisponiveisParaCarga { get; set; }
        public bool AcessoTerceiro { get; set; }
        public int Transportador { get; set; }
        public TipoDocumentoCreditoDebito? TipoDocumentoCreditoDebito { get; set; }
        public SituacaoAtivoPesquisa Ativo { get; set; }
        public bool BloquearVisualizacaoTransportador { get; set; }
        public bool UsuarioAdministrador { get; set; }
        public int CodigoPerfilAcesso { get; set; }
        public bool SomenteOcorrenciaUtilizadaControleEntrega { get; set; }
        public bool SomenteOcorrenciasQueNaoUtilizamControleEntrega { get; set; }
        public List<int> CodigosMotivoChamado { get; set; }
        public List<int> CodigosTipoOperacaoColeta { get; set; }
        public bool NaoPermitirQueTransportadorSelecioneTipoOcorrencia { get; set; }
        public bool NaoUtilizarFlagsControleEntrega { get; set; }
        public bool FiltrarApenasOcorrenciasPermitidasNoPortalDoCliente { get; set; }
    }
}
