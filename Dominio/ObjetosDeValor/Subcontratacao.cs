using System.Collections.Generic;

namespace Dominio.ObjetosDeValor
{
    public class Subcontratacao
    {
        public string Cliente;
        public string AssinaturaDigital;
        public string DataViagem;
        public string CNPJCTe;
        public string CNPJSubcontratado;
        public string CNPJExpedidor;
        public string CNPJRecebedor;
        public List<SubcontratacaoDocumentos> Documentos;
        public string ValorFrete;
        public string codigoCliente;
        public string codigoProcessoTransporte;
        public Dominio.Enumeradores.TipoServico TipoServico;
        public string observacaoSubcontratacao;
        public string DataEmissaoContrato;
        public string DescricaoPercurso;
    }
}
