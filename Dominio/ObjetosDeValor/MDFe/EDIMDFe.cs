using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.MDFe
{
    public class EDIMDFe
    {
        #region Propriedades

        public Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        #endregion

        #region Construtores

        public EDIMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            this.MDFe = mdfe;
        }

        private List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes { get; set; }
        public List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTes
        {
            get
            {
                if (ctes == null)
                    ctes = (from obj in this.MDFe.MunicipiosDescarregamento select obj.Documentos).SelectMany(o => (from doc in o select doc.CTe)).ToList();

                return ctes;
            }
        }

        private List<Dominio.Entidades.DocumentosCTE> notasFiscais { get; set; }
        public List<Dominio.Entidades.DocumentosCTE> NotasFiscais
        {
            get
            {
                if (notasFiscais == null)
                    notasFiscais = (from obj in this.CTes select obj.Documentos).SelectMany(o => o).ToList();

                return notasFiscais;
            }
        }

        public string PlacaTracao
        {
            get
            {
                return MDFe.Veiculos.FirstOrDefault()?.Placa;
            }
        }

        public string UFTracao
        {
            get
            {
                return MDFe.Veiculos.FirstOrDefault()?.UF?.Sigla;
            }
        }

        public int TaraTracao
        {
            get
            {
                int tara = MDFe.Veiculos.FirstOrDefault()?.Tara ?? 0;
                return tara > 1000 ? tara : 1001;
            }
        }

        public string PlacaReboqueOuTracao
        {
            get
            {
                return MDFe.Reboques.FirstOrDefault()?.Placa ?? MDFe.Veiculos.FirstOrDefault()?.Placa;
            }
        }

        public string PlacaSegundoReboqueOuTracao
        {
            get
            {
                return MDFe.Reboques.LastOrDefault()?.Placa ?? MDFe.Veiculos.FirstOrDefault()?.Placa;
            }
        }

        public string CPFMotorista
        {
            get
            {
                return MDFe.Motoristas.FirstOrDefault()?.CPF;
            }
        }

        public decimal ValorTotalNotasFiscais
        {
            get
            {
                decimal valor = NotasFiscais.Sum(o => o.Valor);

                if (valor > 0m)
                    return valor;

                if (MDFe.ValorTotalMercadoria > 0m)
                    return MDFe.ValorTotalMercadoria;

                valor = CTes.Sum(o => o.ValorTotalMercadoria);

                if (valor > 0m)
                    return valor;

                return NotasFiscais.Sum(o => o.ValorMaiorQueZero);
            }
        }

        public decimal PesoNotasFiscais
        {
            get
            {
                decimal peso = NotasFiscais.Sum(o => o.Peso);

                if (peso > 0m)
                    return peso;

                if (MDFe.PesoBrutoMercadoria > 0m)
                    return MDFe.PesoBrutoMercadoria;

                peso = CTes.Sum(o => o.QuantidadesCarga.Where(q => q.UnidadeMedida == "01").Sum(i => i.Quantidade));

                return peso > 0m ? peso : 1m;
            }
        }

        public int VolumesNotasFiscais
        {
            get
            {
                int volumes = NotasFiscais.Sum(o => o.Volume);

                return volumes > 0 ? volumes : 1;
            }
        }

        public int QuantidadeNotasFiscais
        {
            get
            {
                return NotasFiscais.Count();
            }
        }

        #endregion
    }
}
