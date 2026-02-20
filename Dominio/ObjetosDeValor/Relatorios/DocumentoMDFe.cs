namespace Dominio.ObjetosDeValor.Relatorios
{
    public class DocumentoMDFe
    {
        public string Tipo { get; set; }

        public string CNPJEmitente { get; set; }

        public string Chave { get; set; }

        private string _chaveFormatada;
        public string ChaveFormatada
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._chaveFormatada))
                {
                    if (this.Chave != null && this.Chave.Length == 44)
                    {
                        this._chaveFormatada = this.Chave;

                        _chaveFormatada = _chaveFormatada.Insert(4, " ");
                        _chaveFormatada = _chaveFormatada.Insert(9, " ");
                        _chaveFormatada = _chaveFormatada.Insert(14, " ");
                        _chaveFormatada = _chaveFormatada.Insert(19, " ");
                        _chaveFormatada = _chaveFormatada.Insert(24, " ");
                        _chaveFormatada = _chaveFormatada.Insert(29, " ");
                        _chaveFormatada = _chaveFormatada.Insert(34, " ");
                        _chaveFormatada = _chaveFormatada.Insert(39, " ");
                        _chaveFormatada = _chaveFormatada.Insert(44, " ");
                        _chaveFormatada = _chaveFormatada.Insert(49, " ");
                    }
                    else
                    {
                        this._chaveFormatada = string.Empty;
                    }
                }

                return this._chaveFormatada;
            }
        }
    }
}
