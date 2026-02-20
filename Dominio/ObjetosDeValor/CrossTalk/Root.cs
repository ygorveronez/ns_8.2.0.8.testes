namespace Dominio.ObjetosDeValor.CrossTalk
{
    public class Root
    {
        public Document documentField;

        public Document Document
        {
            get
            {
                return this.documentField;
            }
            set
            {
                this.documentField = value;
            }
        }
    }
}
