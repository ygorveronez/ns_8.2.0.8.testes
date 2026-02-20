namespace Dominio.ObjetosDeValor.Embarcador.Notificacao
{
    public class OneSignalHeadings
    {
        public string pt;
        public string en;
        public string es;

        public OneSignalHeadings(string pt = null, string en = null, string es = null)
        {
            this.pt = pt;
            this.en = en;
            this.es = es;
            // Garante que "en" esteja sempre populado
            if (string.IsNullOrEmpty(en))
            {
                this.en = pt ?? es;
            }
        }
    }
}
