using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class CargaCancelamento
    {
        public string UsuarioMultisoftware { get; set; }
        public string DataCancelamento { get; set; }
        public string UsuarioERP { set; get; }
        public string MotivoCancelamento { get; set; }
        public string MotivoRejeicaoCancelamento { set; get; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.RetornoSolicitacaoCancelamento SituacaoCancelamento { set; get; }
        public string NumeroOS { set; get; }
        public string CodigoIntegracaoOperador { set; get; }
        public string DescricaoTipoOperacao { set; get; }
        public string StatusCustoExtra { set; get; }
    }
}
