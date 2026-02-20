namespace Dominio.ObjetosDeValor.EDI.EBS
{
    public class ComissaoMotoristaComissao
    {
        public string Codigo { get; set; }

        private int? _codigoConvertido = null;
        public int CodigoConvertido
        {
            get
            {
                if (_codigoConvertido.HasValue)
                    return _codigoConvertido.Value;

                int.TryParse(Codigo, out int codigo);

                _codigoConvertido = codigo;
                return _codigoConvertido.Value;
            }
        }
        public int Evento { get; set; }
        public decimal Horas { get; set; }
        public decimal Valor { get; set; }
        public string Vinculo { get; set; }
        public int Evento2 { get; set; }
    }
}
