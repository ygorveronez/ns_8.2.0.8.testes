using System.Collections.Generic;
using System.IO;
using CsvHelper;

namespace Utilidades
{
    public class CSV
    {
        public static byte[] GerarCSV<T>(IList<T> objetos)
        {
            using (MemoryStream streamCSV = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(streamCSV, System.Text.Encoding.Default))
                using (CsvWriter csv = new CsvWriter(writer))
                {
                    csv.Configuration.Delimiter = ";";
                    csv.Configuration.ShouldQuote = (field, context) => false;

                    csv.WriteRecords<T>(objetos);

                    writer.Flush();
                }

                return streamCSV.ToArray();
            }
        }

        public static void GerarCSV<T>(IList<T> objetos, string caminho)
        {
            using (System.IO.StreamWriter sr =  new System.IO.StreamWriter(Utilidades.IO.FileStorageService.Storage.OpenWrite(caminho), System.Text.Encoding.UTF8))
            using (CsvWriter csv = new CsvWriter(sr))
            {
                csv.Configuration.Delimiter = ";";

                csv.WriteRecords<T>(objetos);
            }
        }

        /// <summary>
        /// Procedimento para gera um CSV gerando colunas em branco de acordo com uma classe Mapping.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TMap"></typeparam>
        /// <typeparam name="TMapClass"></typeparam>
        /// <param name="objetos"></param>
        /// <returns></returns>
        public static byte[] GerarCSV<T, TMap, TMapClass>(List<T> objetos) where TMap : CsvHelper.Configuration.ClassMap<TMapClass>
        {
            using (MemoryStream streamCSV = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(streamCSV, System.Text.Encoding.Default))
                using (CsvWriter csv = new CsvWriter(writer))
                {
                    csv.Configuration.Delimiter = ";";
                    csv.Configuration.ShouldQuote = (field, context) => false;
                    csv.Configuration.RegisterClassMap<TMap>();

                    csv.WriteRecords<T>(objetos);

                    writer.Flush();
                }

                return streamCSV.ToArray();
            }
        }
    }
}
