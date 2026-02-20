using System;
using System.IO;
using System.Timers;


namespace GerenciadorAppService
{
    public class Servico
    {
        public void Start()
        {
            GravarArquivo("Iniciando");
            Timer();
        }

        private void Timer()
        {
            var timer = new Timer
            {
                Interval = 1000,
                Enabled = true,
            };

            timer.Elapsed += new ElapsedEventHandler(TimerClick);
        }

        private void TimerClick(object sender, ElapsedEventArgs e)
        {
            GravarArquivo($"Hora {DateTime.Now.ToString()}");
        }

        private void GravarArquivo(string texto)
        {
            string path = @"C:\projetos\teste.txt";
            FileStream fs = new FileStream(path,  FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(texto);
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        public void Stop()
        {
            GravarArquivo("Finalizando");
        }

    }
}
