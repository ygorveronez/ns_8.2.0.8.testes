using System;

namespace Servicos.Embarcador.CTe
{
    public class Duplicatas : ServicoBase
    {        
        public Duplicatas(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.ObjetosDeValor.Embarcador.CTe.Duplicata ConverterDynamicParaDuplicata(dynamic dynDuplicata, Repositorio.UnitOfWork unitOfWork)
        {
            if (dynDuplicata != null && dynDuplicata.Valor != null)
            {
                Dominio.ObjetosDeValor.Embarcador.CTe.Duplicata duplicata = new Dominio.ObjetosDeValor.Embarcador.CTe.Duplicata();
                DateTime dataVencimento = new DateTime();
                DateTime.TryParseExact((string)dynDuplicata.DataVencimento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimento);
                duplicata.DataDuplicata = dataVencimento;

                int parcela;
                int.TryParse(dynDuplicata.Parcela.ToString(), out parcela);
                duplicata.Parcela = parcela;

                decimal valorDuplicada;
                decimal.TryParse(dynDuplicata.Valor.ToString(), out valorDuplicada);
                duplicata.ValorDuplicata = valorDuplicada;
                return duplicata;
            }
            else
            {
                return null;
            }
        }
    }
}
