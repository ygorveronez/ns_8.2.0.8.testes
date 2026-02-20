using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioAverbacaoMDFe
    {
        public int CodigoMDFe { get; set; }

        public string AverbacaoProtocolo { get; set; }

        public DateTime? AverbacaoData { get; set; }

        public string AverbacaoRetorno { get; set; }

        public Dominio.Enumeradores.StatusAverbacaoMDFe AverbacaoStatus { get; set; }

        public Dominio.Enumeradores.TipoAverbacaoMDFe AverbacaoTipo { get; set; }

        public string AverbacaoStatusDescricao
        {
            get
            {
                switch (this.AverbacaoStatus)
                {
                    case Enumeradores.StatusAverbacaoMDFe.Pendente:
                        return "Pendente";
                    case Enumeradores.StatusAverbacaoMDFe.Sucesso:
                        return "Sucesso";
                    case Enumeradores.StatusAverbacaoMDFe.Rejeicao:
                        return "Rejeição";
                    default: return "";
                }
            }
        }

        public string AverbacaoTipoDescricao
        {
            get
            {
                switch (this.AverbacaoTipo)
                {
                    case Enumeradores.TipoAverbacaoMDFe.Autorizacao:
                        return "Autorização";
                    case Enumeradores.TipoAverbacaoMDFe.Cancelamento:
                        return "Cancelamento";
                    case Enumeradores.TipoAverbacaoMDFe.Encerramento:
                        return "Encerramento";
                    default: return "";
                }
            }
        }
    }
}

