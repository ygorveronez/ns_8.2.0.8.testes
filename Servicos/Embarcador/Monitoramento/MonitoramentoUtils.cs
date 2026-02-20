using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Monitoramento
{
    public static class MonitoramentoUtils
    {

        #region Métodos públicos

        public static void AtualizarUltimoAlerta(List<Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta> ultimoAlerta, Dominio.Entidades.Embarcador.Logistica.AlertaMonitor novoAlerta)
        {

            var novoUltimoAlerta = new Dominio.ObjetosDeValor.Embarcador.Logistica.UltimoAlerta
            {
                Codigo = novoAlerta.Codigo,
                DataCadastro = novoAlerta.DataCadastro,
                Data = novoAlerta.Data,
                Veiculo = novoAlerta.Veiculo.Codigo,
                TipoAlerta = novoAlerta.TipoAlerta,
                Latitude = novoAlerta?.Latitude != null ? Convert.ToDouble(novoAlerta.Latitude) : 0,
                Longitude = novoAlerta?.Longitude != null ? Convert.ToDouble(novoAlerta.Longitude) : 0,
            };

            if (ultimoAlerta.Count == 0)
            {
                ultimoAlerta.Add(novoUltimoAlerta);
                return;
            }


            var alerta = ultimoAlerta.Where(i => i.Veiculo == novoAlerta.Veiculo.Codigo).FirstOrDefault();

            var index = ultimoAlerta.IndexOf(alerta);

            if (index != -1)
                ultimoAlerta[index] = novoUltimoAlerta;
            else
                ultimoAlerta.Add(novoUltimoAlerta);
        }

        public static void GravarLogTracking(string mensagem, string prefixo, bool dt = true)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                string arquivo = (string.IsNullOrWhiteSpace(prefixo) ? "" : prefixo + "-") + dateTime.ToString("yyyy-MM-dd") + ".txt";
                string path = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.FS.GetPath(AppDomain.CurrentDomain.BaseDirectory), "LogTracking");
                string file = Utilidades.IO.FileStorageService.Storage.Combine(path, arquivo);

                if (dt)
                {
                    mensagem = dateTime.ToLongTimeString() + " - " + mensagem;
                }
                Utilidades.IO.FileStorageService.Storage.WriteLine(file, mensagem);
            }
            catch
            {
            }

        }

        public static bool InRange(decimal inicial, decimal final, decimal valor)
        {
            return valor >= inicial && valor <= final;
        }

        public static bool VerificarLocais(List<Dominio.Entidades.Embarcador.Logistica.Locais> listaLocais, Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocal tipoLocal)
        {
            double latitude = posicao.Latitude;
            double longitude = posicao.Longitude;

            List<Dominio.Entidades.Embarcador.Logistica.Locais> locais = listaLocais.Where(o => o.Tipo == tipoLocal).ToList();

            foreach (var local in listaLocais)
            {
                if (string.IsNullOrWhiteSpace(local.Area))
                    continue;

                return Servicos.Embarcador.Logistica.Monitoramento.Monitoramento.EmArea(local.Area, latitude, longitude);
            }
            return false;
        }

        #endregion

    }

}