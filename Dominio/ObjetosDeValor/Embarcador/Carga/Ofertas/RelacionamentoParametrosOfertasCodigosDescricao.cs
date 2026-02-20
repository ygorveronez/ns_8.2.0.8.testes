using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.Ofertas
{
    public class RelacionamentoParametrosOfertasCodigosDescricao
    {
        public int Codigo { get; set; }
        public int CodigoRelacionamento { get; set; }
        public int CodigoEntidadeFraca { get; set; }
        public virtual string Descricao { get; set; }
        public string CNPJ { get; set; }
        public string CNPJ_Formatado
        {
            get
            {
                string formatado = Convert.ToUInt64(this.CNPJ).ToString(@"00\.000\.000\/0000\-00");

                return formatado;
            }
        }
    }
}
