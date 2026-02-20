using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Integracao
{
    public sealed class IndicadorIntegracaoCTe
    {
        #region Propriedades

        public string ChaveCTe { get; set; }

        public int Codigo { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public string DataEmissaoCTe { get; set; }

        public string Filial { get; set; }

        public int NumeroCTe { get; set; }

        public int SerieCTe { get; set; }

        public string Transportador { get; set; }

        public string Integradoras { get; set; }

        public decimal ValorFreteSemImposto { get; set; }

        private Dominio.Enumeradores.TipoDocumento TipoDocumento { get; set; }

        private string StatusDocumentos { get; set; }


        #endregion

        #region Propriedades com Regra

        public string DataIntegracao1
        {
            get
            {
                string[] indicadorIntegradoraDados = ObterIntegradoraDados(1);

                return (indicadorIntegradoraDados?.Length > 1) ? indicadorIntegradoraDados[1] : "";
            }
        }

        public string DataIntegracao2
        {
            get
            {
                string[] indicadorIntegradoraDados = ObterIntegradoraDados(2);

                return (indicadorIntegradoraDados?.Length > 1) ? indicadorIntegradoraDados[1] : "";
            }
        }

        public string DataIntegracao3
        {
            get
            {
                string[] indicadorIntegradoraDados = ObterIntegradoraDados(3);

                return (indicadorIntegradoraDados?.Length > 1) ? indicadorIntegradoraDados[1] : "";
            }
        }

        public string DataIntegracao4
        {
            get
            {
                string[] indicadorIntegradoraDados = ObterIntegradoraDados(4);

                return (indicadorIntegradoraDados?.Length > 1) ? indicadorIntegradoraDados[1] : "";
            }
        }

        public string DataIntegracao5
        {
            get
            {
                string[] indicadorIntegradoraDados = ObterIntegradoraDados(5);

                return (indicadorIntegradoraDados?.Length > 1) ? indicadorIntegradoraDados[1] : "";
            }
        }

        public string Integrado1
        {
            get
            {
                return string.IsNullOrWhiteSpace(DataIntegracao1) ? "Não" : "Sim";
            }
        }

        public string Integrado2
        {
            get
            {
                return string.IsNullOrWhiteSpace(DataIntegracao2) ? "Não" : "Sim";
            }
        }

        public string Integrado3
        {
            get
            {
                return string.IsNullOrWhiteSpace(DataIntegracao3) ? "Não" : "Sim";
            }
        }

        public string Integrado4
        {
            get
            {
                return string.IsNullOrWhiteSpace(DataIntegracao4) ? "Não" : "Sim";
            }
        }

        public string Integrado5
        {
            get
            {
                return string.IsNullOrWhiteSpace(DataIntegracao5) ? "Não" : "Sim";
            }
        }

        public string TipoDocumentoDescripcao
        {
            get { return Dominio.Enumeradores.TipoDocumentoHelper.ObterDescricao(TipoDocumento); }
        }

        public string StatusDocumentosDescripcao
        {
            get { return StatusDocumentos == "A" ? "Autorizado" : "Não Autorizado"; }
        }
        #endregion

        #region Métodos Privados

        private string[] ObterIntegradoraDados(int numeroIntegradora)
        {
            string[] indicadorIntegradoras = Integradoras.Split(',');

            if (indicadorIntegradoras.Length < numeroIntegradora)
                return null;

            return indicadorIntegradoras[(numeroIntegradora - 1)].Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion
    }
}
