namespace SGT.GerenciadorApp
{
    public class ConfiguracaoEBSModel : ObservableObject
    {
        #region Campos

        private int _diasRetroativos;
        private string _caminhoSalvarArquivos;
        private string _horaExecucao;
        private string _emailsEnvioArquivos;

        #endregion

        #region Propriedades

        public int DiasRetroativos
        {
            get { return _diasRetroativos; }
            set
            {
                if (value != _diasRetroativos)
                {
                    _diasRetroativos = value;
                    OnPropertyChanged("DiasRetroativos");
                }
            }
        }

        public string CaminhoSalvarArquivos
        {
            get { return _caminhoSalvarArquivos; }
            set
            {
                if (value != _caminhoSalvarArquivos)
                {
                    _caminhoSalvarArquivos = value;
                    OnPropertyChanged("CaminhoSalvarArquivos");
                }
            }
        }

        public string HoraExecucao
        {
            get { return _horaExecucao; }
            set
            {
                if (value != _horaExecucao)
                {
                    _horaExecucao = value;
                    OnPropertyChanged("HoraExecucao");
                }
            }
        }

        public string EmailsEnvioArquivos
        {
            get { return _emailsEnvioArquivos; }
            set
            {
                if (value != _emailsEnvioArquivos)
                {
                    _emailsEnvioArquivos = value;
                    OnPropertyChanged("EmailsEnvioArquivos");
                }
            }
        }

        #endregion
    }
}
