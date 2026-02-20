using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Utilidades
{
    public class Conversor
    {
        public static DataTable ListToDataTable<T>(List<T> data, string tableName)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable(tableName);
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public static DataSet ListToDataSet<T>(List<T> list, string tableName)
        {
            Type type = typeof(T);
            DataSet ds = new DataSet(tableName);
            DataTable dt = new DataTable(tableName);

            ds.Tables.Add(dt);
            var propertyInfos = type.GetProperties().ToList();
            propertyInfos.ForEach(propertyInfo =>
            {
                Type columnType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                dt.Columns.Add(propertyInfo.Name, columnType);
            });
            list.ForEach(item =>
            {
                DataRow row = dt.NewRow();
                propertyInfos.ForEach(
                                       propertyInfo => row[propertyInfo.Name] = propertyInfo.GetValue(item, null) ?? DBNull.Value
                                     );
                dt.Rows.Add(row);
            });
            return ds;
        }

        public static System.DateTime ChangeTime(System.DateTime dateTime, int hours, int minutes, int seconds, int milliseconds)
        {
            return new System.DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                milliseconds,
                dateTime.Kind);
        }

        public static System.DateTime ExtraiDateTime(string parameter)
        {
            if (parameter == null)
                parameter = "";

            if (parameter.Length == 10)
                parameter += " 00:00";

            System.DateTime.TryParseExact(parameter, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out System.DateTime dataConvertida);

            return dataConvertida;
        }

        public static string Ordinal(int ordem)
        {
            if (ordem <= 0 | ordem >= 1000)
                return "Valor não suportado pelo sistema.";

            string ordinal = "";

            int centenas = (int)Math.Floor(ordem / 100m);
            if (centenas > 0) ordinal += OrdinalCentenas(centenas) + " ";
            ordem = ordem - (centenas * 100);

            int dezenas = (int)Math.Floor(ordem / 10m);
            if (dezenas > 0) ordinal += OrdinalDezenas(dezenas) + " ";
            ordem = ordem - (dezenas * 10);

            if (ordem > 0) ordinal += OrdinalUnidades(ordem);

            return ordinal.Trim();
        }

        private static string OrdinalCentenas(int centenas)
        {
            string ordinal = "";

            if (centenas == 1) ordinal = "Cen";
            else if (centenas == 2) ordinal = "Ducen";
            else if (centenas == 3) ordinal = "Tricen";
            else if (centenas == 4) ordinal = "Quadringen";
            else if (centenas == 5) ordinal = "Quingen";
            else if (centenas == 6) ordinal = "Sexcen";
            else if (centenas == 7) ordinal = "Setingen";
            else if (centenas == 8) ordinal = "Octingen";
            else ordinal = "Nongen";

            return ordinal + "tésimo";
        }

        private static string OrdinalDezenas(int dezenas)
        {
            string ordinal = "";

            if (dezenas == 1) return "Décimo";
            else if (dezenas == 2) ordinal = "Vi";
            else if (dezenas == 3) ordinal = "Tri";
            else if (dezenas == 4) ordinal = "Quadra";
            else if (dezenas == 5) ordinal = "Quinqua";
            else if (dezenas == 6) ordinal = "Sexa";
            else if (dezenas == 7) ordinal = "Setua";
            else if (dezenas == 8) ordinal = "Octo";
            else ordinal = "Nona";

            return ordinal + "gésimo";
        }

        private static string OrdinalUnidades(int unidades)
        {
            if (unidades == 1) return "Primeiro";
            else if (unidades == 2) return "Segundo";
            else if (unidades == 3) return "Terceiro";
            else if (unidades == 4) return "Quarto";
            else if (unidades == 5) return "Quinto";
            else if (unidades == 6) return "Sexto";
            else if (unidades == 7) return "Sétimo";
            else if (unidades == 8) return "Oitavo";
            else return "Nono";
        }

        public static void ObterDescricaoMoeda(string moeda, out string descricaoSingular, out string descricaoPlural)
        {
            moeda = Utilidades.String.RemoveDiacritics(moeda).ToLower();

            switch (moeda)
            {
                case "dolar":
                    descricaoSingular = "DÓLAR";
                    descricaoPlural = "DOLARES";
                    break;
                case "peso argentino":
                    descricaoSingular = "PESO ARGENTINO";
                    descricaoPlural = "PESOS ARGENTINOS";
                    break;
                case "peso uruguaio":
                    descricaoSingular = "PESO URUGUAYO";
                    descricaoPlural = "PESOS URUGUAYOS";
                    break;
                case "peso chileno":
                    descricaoSingular = "PESO CHILENO";
                    descricaoPlural = "PESOS CHILENOS";
                    break;
                case "guarani":
                    descricaoSingular = "GUARANÍ";
                    descricaoPlural = "GUARANIS";
                    break;
                case "real":
                    descricaoSingular = "REAL";
                    descricaoPlural = "REAIS";
                    break;
                default:
                    descricaoSingular = "REAL";
                    descricaoPlural = "REAIS";
                    break;
            }
        }

        public static string DecimalToWords(decimal valor, string moeda = "real", bool traduzirMoeda = false)
        {
            moeda = Utilidades.String.RemoveDiacritics(moeda).ToLower();

            switch (moeda)
            {
                case "dolar":
                //return DecimalToWordsDolar(valor);

                case "peso argentino":
                case "peso uruguaio":
                case "peso chileno":
                case "guarani":
                    return DecimalToWordsPeso(valor, moeda);

                case "real":
                default:
                    return DecimalToWordsReal(valor, traduzirMoeda);
            }
        }

        public static string DecimalToWordsReal(decimal valor, bool traduzirMoeda)
        {
            if (valor <= 0 | valor >= 1000000000000000)
                return "Valor não suportado pelo sistema.";
            else
            {
                string strValor = valor.ToString("000000000000000.00");
                string valor_por_extenso = string.Empty;

                for (int i = 0; i <= 15; i += 3)
                {
                    valor_por_extenso += EscreverParteReal(Convert.ToDecimal(strValor.Substring(i, 3)));

                    if (i == 0 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(0, 3)) == 1)
                            valor_por_extenso += " TRILHÃO" + ((Convert.ToDecimal(strValor.Substring(3, 12)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(0, 3)) > 1)
                            valor_por_extenso += " TRILHÕES" + ((Convert.ToDecimal(strValor.Substring(3, 12)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 3 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(3, 3)) == 1)
                            valor_por_extenso += " BILHÃO" + ((Convert.ToDecimal(strValor.Substring(6, 9)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(3, 3)) > 1)
                            valor_por_extenso += " BILHÕES" + ((Convert.ToDecimal(strValor.Substring(6, 9)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 6 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(6, 3)) == 1)
                            valor_por_extenso += " MILHÃO" + ((Convert.ToDecimal(strValor.Substring(9, 6)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(6, 3)) > 1)
                            valor_por_extenso += " MILHÕES" + ((Convert.ToDecimal(strValor.Substring(9, 6)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 9 & valor_por_extenso != string.Empty)
                        if (Convert.ToInt32(strValor.Substring(9, 3)) > 0)
                            valor_por_extenso += " MIL" + ((Convert.ToDecimal(strValor.Substring(12, 3)) > 0) ? " E " : string.Empty);

                    if (i == 12)
                    {
                        if (valor_por_extenso.Length > 8)
                            if (valor_por_extenso.Substring(valor_por_extenso.Length - 6, 6) == "BILHÃO" | valor_por_extenso.Substring(valor_por_extenso.Length - 6, 6) == "MILHÃO")
                                valor_por_extenso += " DE";
                            else
                                if (valor_por_extenso.Substring(valor_por_extenso.Length - 7, 7) == "BILHÕES" | valor_por_extenso.Substring(valor_por_extenso.Length - 7, 7) == "MILHÕES" | valor_por_extenso.Substring(valor_por_extenso.Length - 8, 7) == "TRILHÕES")
                                valor_por_extenso += " DE";
                            else
                                    if (valor_por_extenso.Substring(valor_por_extenso.Length - 8, 8) == "TRILHÕES")
                                valor_por_extenso += " DE";

                        if (Convert.ToInt64(strValor.Substring(0, 15)) == 1)
                            valor_por_extenso += traduzirMoeda ? " DOLAR" : " REAL";
                        else if (Convert.ToInt64(strValor.Substring(0, 15)) > 1)
                            valor_por_extenso += traduzirMoeda ? " DOLARES" : " REAIS";

                        if (Convert.ToInt32(strValor.Substring(16, 2)) > 0 && valor_por_extenso != string.Empty)
                            valor_por_extenso += " E ";
                    }

                    if (i == 15)
                        if (Convert.ToInt32(strValor.Substring(16, 2)) == 1)
                            valor_por_extenso += " CENTAVO";
                        else if (Convert.ToInt32(strValor.Substring(16, 2)) > 1)
                            valor_por_extenso += " CENTAVOS";
                }
                return valor_por_extenso;
            }
        }

        private static string EscreverParteReal(decimal valor)
        {
            if (valor <= 0)
                return string.Empty;
            else
            {
                string montagem = string.Empty;
                if (valor > 0 & valor < 1)
                {
                    valor *= 100;
                }
                string strValor = valor.ToString("000");
                int a = Convert.ToInt32(strValor.Substring(0, 1));
                int b = Convert.ToInt32(strValor.Substring(1, 1));
                int c = Convert.ToInt32(strValor.Substring(2, 1));

                if (a == 1) montagem += (b + c == 0) ? "CEM" : "CENTO";
                else if (a == 2) montagem += "DUZENTOS";
                else if (a == 3) montagem += "TREZENTOS";
                else if (a == 4) montagem += "QUATROCENTOS";
                else if (a == 5) montagem += "QUINHENTOS";
                else if (a == 6) montagem += "SEISCENTOS";
                else if (a == 7) montagem += "SETECENTOS";
                else if (a == 8) montagem += "OITOCENTOS";
                else if (a == 9) montagem += "NOVECENTOS";

                if (b == 1)
                {
                    if (c == 0) montagem += ((a > 0) ? " E " : string.Empty) + "DEZ";
                    else if (c == 1) montagem += ((a > 0) ? " E " : string.Empty) + "ONZE";
                    else if (c == 2) montagem += ((a > 0) ? " E " : string.Empty) + "DOZE";
                    else if (c == 3) montagem += ((a > 0) ? " E " : string.Empty) + "TREZE";
                    else if (c == 4) montagem += ((a > 0) ? " E " : string.Empty) + "QUATORZE";
                    else if (c == 5) montagem += ((a > 0) ? " E " : string.Empty) + "QUINZE";
                    else if (c == 6) montagem += ((a > 0) ? " E " : string.Empty) + "DEZESSEIS";
                    else if (c == 7) montagem += ((a > 0) ? " E " : string.Empty) + "DEZESSETE";
                    else if (c == 8) montagem += ((a > 0) ? " E " : string.Empty) + "DEZOITO";
                    else if (c == 9) montagem += ((a > 0) ? " E " : string.Empty) + "DEZENOVE";
                }
                else if (b == 2) montagem += ((a > 0) ? " E " : string.Empty) + "VINTE";
                else if (b == 3) montagem += ((a > 0) ? " E " : string.Empty) + "TRINTA";
                else if (b == 4) montagem += ((a > 0) ? " E " : string.Empty) + "QUARENTA";
                else if (b == 5) montagem += ((a > 0) ? " E " : string.Empty) + "CINQUENTA";
                else if (b == 6) montagem += ((a > 0) ? " E " : string.Empty) + "SESSENTA";
                else if (b == 7) montagem += ((a > 0) ? " E " : string.Empty) + "SETENTA";
                else if (b == 8) montagem += ((a > 0) ? " E " : string.Empty) + "OITENTA";
                else if (b == 9) montagem += ((a > 0) ? " E " : string.Empty) + "NOVENTA";

                if (strValor.Substring(1, 1) != "1" & c != 0 & montagem != string.Empty) montagem += " E ";

                if (strValor.Substring(1, 1) != "1")
                    if (c == 1) montagem += "UM";
                    else if (c == 2) montagem += "DOIS";
                    else if (c == 3) montagem += "TRÊS";
                    else if (c == 4) montagem += "QUATRO";
                    else if (c == 5) montagem += "CINCO";
                    else if (c == 6) montagem += "SEIS";
                    else if (c == 7) montagem += "SETE";
                    else if (c == 8) montagem += "OITO";
                    else if (c == 9) montagem += "NOVE";

                return montagem;
            }
        }

        //public static string DecimalToWordsDolar(decimal valor)
        //{
        //    if (valor <= 0 | valor >= 1000000000000000)
        //        return "Valor não suportado pelo sistema.";
        //    else
        //    {
        //        string strValor = valor.ToString("000000000000000.00");
        //        string valor_por_extenso = string.Empty;

        //        for (int i = 0; i <= 15; i += 3)
        //        {
        //            valor_por_extenso += EscreverParteDolar(Convert.ToDecimal(strValor.Substring(i, 3)));

        //            if (i == 0 & valor_por_extenso != string.Empty)
        //            {
        //                if (Convert.ToInt32(strValor.Substring(0, 3)) >= 1)
        //                    valor_por_extenso += " TRILLION" + ((Convert.ToDecimal(strValor.Substring(3, 12)) > 0) ? " AND " : string.Empty);
        //            }
        //            else if (i == 3 & valor_por_extenso != string.Empty)
        //            {
        //                if (Convert.ToInt32(strValor.Substring(3, 3)) >= 1)
        //                    valor_por_extenso += " BILLION" + ((Convert.ToDecimal(strValor.Substring(6, 9)) > 0) ? " AND " : string.Empty);

        //            }
        //            else if (i == 6 & valor_por_extenso != string.Empty)
        //            {
        //                if (Convert.ToInt32(strValor.Substring(6, 3)) >= 1)
        //                    valor_por_extenso += " MILLION" + ((Convert.ToDecimal(strValor.Substring(9, 6)) > 0) ? " AND " : string.Empty);
        //            }
        //            else if (i == 9 & valor_por_extenso != string.Empty)
        //                if (Convert.ToInt32(strValor.Substring(9, 3)) > 0)
        //                    valor_por_extenso += " THOUSAND" + ((Convert.ToDecimal(strValor.Substring(12, 3)) > 0) ? " AND " : string.Empty);

        //            if (i == 12)
        //            {                            
        //                if (Convert.ToInt64(strValor.Substring(0, 15)) == 1)
        //                    valor_por_extenso += " DOLLAR";
        //                else if (Convert.ToInt64(strValor.Substring(0, 15)) > 1)
        //                    valor_por_extenso += " DOLLARS";

        //                if (Convert.ToInt32(strValor.Substring(16, 2)) > 0 && valor_por_extenso != string.Empty)
        //                    valor_por_extenso += " AND ";
        //            }

        //            if (i == 15)
        //                if (Convert.ToInt32(strValor.Substring(16, 2)) == 1)
        //                    valor_por_extenso += " CENT";
        //                else if (Convert.ToInt32(strValor.Substring(16, 2)) > 1)
        //                    valor_por_extenso += " CENTS";
        //        }
        //        return valor_por_extenso;
        //    }
        //}

        //private static string EscreverParteDolar(decimal valor)
        //{
        //    if (valor <= 0)
        //        return string.Empty;
        //    else
        //    {
        //        string montagem = string.Empty;
        //        if (valor > 0 & valor < 1)
        //        {
        //            valor *= 100;
        //        }
        //        string strValor = valor.ToString("000");
        //        int a = Convert.ToInt32(strValor.Substring(0, 1));
        //        int b = Convert.ToInt32(strValor.Substring(1, 1));
        //        int c = Convert.ToInt32(strValor.Substring(2, 1));

        //        if (a == 1) montagem += "ONE HUNDRED";
        //        else if (a == 2) montagem += "TWO HUNDRED";
        //        else if (a == 3) montagem += "THREE HUNDRED";
        //        else if (a == 4) montagem += "FOUR HUNDRED";
        //        else if (a == 5) montagem += "FIVE HUNDRED";
        //        else if (a == 6) montagem += "SIX HUNDRED";
        //        else if (a == 7) montagem += "SEVEN HUNDRED";
        //        else if (a == 8) montagem += "EIGHT HUNDRED";
        //        else if (a == 9) montagem += "NINE HUNDRED";

        //        if (b == 1)
        //        {
        //            if (c == 0) montagem += ((a > 0) ? " E " : string.Empty) + "TEN";
        //            else if (c == 1) montagem += ((a > 0) ? " E " : string.Empty) + "ELEVEN";
        //            else if (c == 2) montagem += ((a > 0) ? " E " : string.Empty) + "TWELVE";
        //            else if (c == 3) montagem += ((a > 0) ? " E " : string.Empty) + "THIRTEEN";
        //            else if (c == 4) montagem += ((a > 0) ? " E " : string.Empty) + "FOURTEEN";
        //            else if (c == 5) montagem += ((a > 0) ? " E " : string.Empty) + "FIFTEEN";
        //            else if (c == 6) montagem += ((a > 0) ? " E " : string.Empty) + "SIXTEEN";
        //            else if (c == 7) montagem += ((a > 0) ? " E " : string.Empty) + "SEVENTEEN";
        //            else if (c == 8) montagem += ((a > 0) ? " E " : string.Empty) + "EIGHTEEN";
        //            else if (c == 9) montagem += ((a > 0) ? " E " : string.Empty) + "NINETEEN";
        //        }
        //        else if (b == 2) montagem += ((a > 0) ? " E " : string.Empty) + "TWENTY";
        //        else if (b == 3) montagem += ((a > 0) ? " E " : string.Empty) + "THIRTY";
        //        else if (b == 4) montagem += ((a > 0) ? " E " : string.Empty) + "FOURTY";
        //        else if (b == 5) montagem += ((a > 0) ? " E " : string.Empty) + "FIFTY";
        //        else if (b == 6) montagem += ((a > 0) ? " E " : string.Empty) + "SIXTY";
        //        else if (b == 7) montagem += ((a > 0) ? " E " : string.Empty) + "SEVENTY";
        //        else if (b == 8) montagem += ((a > 0) ? " E " : string.Empty) + "EIGHTY";
        //        else if (b == 9) montagem += ((a > 0) ? " E " : string.Empty) + "NINETY";

        //        if (strValor.Substring(1, 1) != "1" & c != 0 & montagem != string.Empty) montagem += " AND ";

        //        if (strValor.Substring(1, 1) != "1")
        //            if (c == 1) montagem += "ONE";
        //            else if (c == 2) montagem += "TWO";
        //            else if (c == 3) montagem += "THREE";
        //            else if (c == 4) montagem += "FOUR";
        //            else if (c == 5) montagem += "FIVE";
        //            else if (c == 6) montagem += "SIX";
        //            else if (c == 7) montagem += "SEVEN";
        //            else if (c == 8) montagem += "EIGHT";
        //            else if (c == 9) montagem += "NINE";

        //        return montagem;
        //    }
        //}

        public static string DecimalToWordsPeso(decimal valor, string moeda)
        {
            if (valor <= 0 | valor >= 1000000000000000)
                return "Valor não suportado pelo sistema.";
            else
            {
                string strValor = valor.ToString("000000000000000.00");
                string valor_por_extenso = string.Empty;

                ObterDescricaoMoeda(moeda, out string decricaoMoedaSingular, out string descricaoMoedaPlural);

                for (int i = 0; i <= 15; i += 3)
                {
                    valor_por_extenso += EscreverPartePeso(Convert.ToDecimal(strValor.Substring(i, 3)));

                    if (i == 0 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(0, 3)) == 1)
                            valor_por_extenso += " BILLÓN" + ((Convert.ToDecimal(strValor.Substring(3, 12)) > 0) ? " Y " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(0, 3)) > 1)
                            valor_por_extenso += " BILLONES" + ((Convert.ToDecimal(strValor.Substring(3, 12)) > 0) ? " Y " : string.Empty);
                    }
                    else if (i == 3 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(3, 3)) >= 1)
                            valor_por_extenso += " MIL MILLONES" + ((Convert.ToDecimal(strValor.Substring(6, 9)) > 0) ? " Y " : string.Empty);
                    }
                    else if (i == 6 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(6, 3)) == 1)
                            valor_por_extenso += " MILLON" + ((Convert.ToDecimal(strValor.Substring(9, 6)) > 0) ? " Y " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(6, 3)) > 1)
                            valor_por_extenso += " MILLONES" + ((Convert.ToDecimal(strValor.Substring(9, 6)) > 0) ? " Y " : string.Empty);
                    }
                    else if (i == 9 & valor_por_extenso != string.Empty)
                        if (Convert.ToInt32(strValor.Substring(9, 3)) > 0)
                            valor_por_extenso += " MIL" + ((Convert.ToDecimal(strValor.Substring(12, 3)) > 0) ? " Y " : string.Empty);

                    if (i == 12)
                    {
                        if (Convert.ToInt64(strValor.Substring(0, 15)) == 1)
                            valor_por_extenso += " " + decricaoMoedaSingular;
                        else if (Convert.ToInt64(strValor.Substring(0, 15)) > 1)
                            valor_por_extenso += " " + descricaoMoedaPlural;

                        if (Convert.ToInt32(strValor.Substring(16, 2)) > 0 && valor_por_extenso != string.Empty)
                            valor_por_extenso += " Y ";
                    }

                    if (i == 15)
                        if (Convert.ToInt32(strValor.Substring(16, 2)) == 1)
                            valor_por_extenso += " CENTAVO";
                        else if (Convert.ToInt32(strValor.Substring(16, 2)) > 1)
                            valor_por_extenso += " CENTAVOS";
                }
                return valor_por_extenso;
            }
        }

        private static string EscreverPartePeso(decimal valor)
        {
            if (valor <= 0)
                return string.Empty;
            else
            {
                string montagem = string.Empty;
                if (valor > 0 & valor < 1)
                {
                    valor *= 100;
                }
                string strValor = valor.ToString("000");
                int a = Convert.ToInt32(strValor.Substring(0, 1));
                int b = Convert.ToInt32(strValor.Substring(1, 1));
                int c = Convert.ToInt32(strValor.Substring(2, 1));

                if (a == 1) montagem += (b + c == 0) ? "CIEN" : "CIENTO";
                else if (a == 2) montagem += "DOSCIENTOS";
                else if (a == 3) montagem += "TRESCIENTOS";
                else if (a == 4) montagem += "CUATROCIENTOS";
                else if (a == 5) montagem += "QUINIENTOS";
                else if (a == 6) montagem += "SEISCIENTOS";
                else if (a == 7) montagem += "SETECIENTOS";
                else if (a == 8) montagem += "OCHOCIENTOS";
                else if (a == 9) montagem += "NOVECIENTOS";

                if (b == 1)
                {
                    if (c == 0) montagem += ((a > 0) ? " E " : string.Empty) + "DIEZ";
                    else if (c == 1) montagem += ((a > 0) ? " E " : string.Empty) + "ONCE";
                    else if (c == 2) montagem += ((a > 0) ? " E " : string.Empty) + "DOCE";
                    else if (c == 3) montagem += ((a > 0) ? " E " : string.Empty) + "TRECE";
                    else if (c == 4) montagem += ((a > 0) ? " E " : string.Empty) + "CATORCE";
                    else if (c == 5) montagem += ((a > 0) ? " E " : string.Empty) + "QUINCE";
                    else if (c == 6) montagem += ((a > 0) ? " E " : string.Empty) + "DIECISÉIS";
                    else if (c == 7) montagem += ((a > 0) ? " E " : string.Empty) + "DIECISIETE";
                    else if (c == 8) montagem += ((a > 0) ? " E " : string.Empty) + "DIECIOCHO";
                    else if (c == 9) montagem += ((a > 0) ? " E " : string.Empty) + "DIECINUEVE";
                }
                else if (b == 2) montagem += ((a > 0) ? " E " : string.Empty) + "VEINTE";
                else if (b == 3) montagem += ((a > 0) ? " E " : string.Empty) + "TREINTA";
                else if (b == 4) montagem += ((a > 0) ? " E " : string.Empty) + "CUARENTA";
                else if (b == 5) montagem += ((a > 0) ? " E " : string.Empty) + "CINCUENTA";
                else if (b == 6) montagem += ((a > 0) ? " E " : string.Empty) + "SESENTA";
                else if (b == 7) montagem += ((a > 0) ? " E " : string.Empty) + "SETENTA";
                else if (b == 8) montagem += ((a > 0) ? " E " : string.Empty) + "OCHENTA";
                else if (b == 9) montagem += ((a > 0) ? " E " : string.Empty) + "NOVENTA";

                if (strValor.Substring(1, 1) != "1" & c != 0 & montagem != string.Empty) montagem += " Y ";

                if (strValor.Substring(1, 1) != "1")
                    if (c == 1) montagem += "UNO";
                    else if (c == 2) montagem += "DOS";
                    else if (c == 3) montagem += "TRES";
                    else if (c == 4) montagem += "CUATRO";
                    else if (c == 5) montagem += "CINCO";
                    else if (c == 6) montagem += "SEIS";
                    else if (c == 7) montagem += "SIETE";
                    else if (c == 8) montagem += "OCHO";
                    else if (c == 9) montagem += "NUEVE";

                return montagem;
            }
        }

        public static string DecimalToWordsDolar(decimal value)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");
            string decimals = "";
            string input = Math.Round(value, 2).ToString(cultura);

            if (input.Contains("."))
            {
                decimals = input.Substring(input.IndexOf(".") + 1);
                // remove decimal part from input
                input = input.Remove(input.IndexOf("."));
            }

            // Convert input into words. save it into strWords
            string strWords = GetWords(input);

            if (input == "1")
                strWords += "DOLLAR";
            else
                strWords += "DOLLARS";

            if (decimals.Length > 0 && decimals != "00")
            {
                // if there is any decimal part convert it to words and add it to strWords.
                strWords += " AND " + GetWords(decimals);

                if (decimals == "01")
                    strWords += "CENT";
                else
                    strWords += "CENTS";
            }

            return strWords;
        }

        private static string GetWords(string input)
        {
            // these are seperators for each 3 digit in numbers. you can add more if you want convert beigger numbers.
            string[] seperators = { "", "THOUSAND ", "MILLION ", "BILLION " };

            // Counter is indexer for seperators. each 3 digit converted this will count.
            int i = 0;

            string strWords = "";

            while (input.Length > 0)
            {
                // get the 3 last numbers from input and store it. if there is not 3 numbers just use take it.
                string _3digits = input.Length < 3 ? input : input.Substring(input.Length - 3);
                // remove the 3 last digits from input. if there is not 3 numbers just remove it.
                input = input.Length < 3 ? "" : input.Remove(input.Length - 3);

                int no = int.Parse(_3digits);
                // Convert 3 digit number into words.
                _3digits = GetWord(no);

                // apply the seperator.
                _3digits += seperators[i];
                // since we are getting numbers from right to left then we must append resault to strWords like this.
                strWords = _3digits + strWords;

                // 3 digits converted. count and go for next 3 digits
                i++;
            }
            return strWords;
        }

        // your method just to convert 3digit number into words.
        private static string GetWord(int no)
        {
            string[] Ones = {
                "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN",
                "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN"
            };

            string[] Tens = { "TEN", "TWENTY", "THIRTY", "FOURTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };

            string word = "";

            if (no > 99 && no < 1000)
            {
                int i = no / 100;
                word = word + Ones[i - 1] + " HUNDRED ";
                no = no % 100;
            }

            if (no > 19 && no < 100)
            {
                int i = no / 10;
                word = word + Tens[i - 1];
                no = no % 10;

                if (no > 0)
                    word += "-";
                else
                    word += " ";
            }

            if (no > 0 && no < 20)
            {
                word = word + Ones[no - 1] + " ";
            }

            return word;
        }


    }
}
