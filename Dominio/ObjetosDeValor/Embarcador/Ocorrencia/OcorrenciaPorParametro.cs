using System;

namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public class OcorrenciaPorParametro
    {
        public int Codigo { get; set; }

        public int ComponenteFrete { get; set; }

        public string Titulo { get; set; }

        public int Minutos { get; set; }

        public decimal Valor { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCalculoOcorrencia TipoParametro { get; set; }

        public virtual string Descricao
        {
            get
            {
                switch (TipoParametro)
                {
                    case Enumeradores.TipoParametroCalculoOcorrencia.HorasExtra:
                    case Enumeradores.TipoParametroCalculoOcorrencia.Estadia:
                        TimeSpan ts = new TimeSpan(0, this.Minutos, 0);
                        return ts.ToString(@"hh\:mm") + " " + this.Titulo;

                    case Enumeradores.TipoParametroCalculoOcorrencia.Pernoite:
                    case Enumeradores.TipoParametroCalculoOcorrencia.Escolta:
                        return this.Titulo;

                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoValor
        {
            get
            {
                return this.Valor.ToString("n2");
            }
        }
    }
}
