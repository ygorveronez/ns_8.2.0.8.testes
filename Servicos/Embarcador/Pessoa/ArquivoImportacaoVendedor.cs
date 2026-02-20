using System;

namespace Servicos.Embarcador.Pessoa
{
    public class ArquivoImportacaoVendedor
    {
        #region Métodos Globais

        public static object LerCampo<TArquivo>(Dominio.Entidades.Embarcador.Configuracoes.ArquivoImportacao<TArquivo> campo, string valor) where TArquivo : Dominio.Entidades.EntidadeBase
        {
            switch (campo.TipoPropriedade)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo.Alfanumerico:

                    return valor.Trim();

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo.Data:

                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        DateTime data = DateTime.MinValue;

                        DateTime.TryParseExact(valor, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out data);

                        if (data == DateTime.MinValue)
                            DateTime.TryParseExact(valor, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out data);
                        if (data == DateTime.MinValue)
                            DateTime.TryParseExact(valor, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out data);
                        if (data == DateTime.MinValue)
                            DateTime.TryParseExact(valor, "ddMMyyyy", null, System.Globalization.DateTimeStyles.None, out data);

                        return data;
                    }
                    else
                    {
                        return null;
                    }

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo.Decimal:

                    if (!string.IsNullOrWhiteSpace(valor))
                    {
                        //decimal dec = 0m;

                        //decimal.TryParse(valor, out dec);

                        return Utilidades.Decimal.Converter(valor);

                        //return dec;
                    }
                    else
                    {
                        return 0m;
                    }

                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo.Numerico:

                    int i = 0;

                    int.TryParse(Utilidades.String.OnlyNumbers(valor), out i);

                    return i;

                default:

                    return null;
            }
        }

        #endregion
    }
}
