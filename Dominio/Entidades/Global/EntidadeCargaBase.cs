namespace Dominio.Entidades
{
    public abstract class EntidadeCargaBase : EntidadeBase
    {
        public virtual Embarcador.Cargas.Carga Carga { get; set; }

        public virtual Embarcador.PreCargas.PreCarga PreCarga { get; set; }

        public virtual Embarcador.Cargas.CargaBase CargaBase
        {
            get
            {
                if (Carga != null)
                    return Carga;

                return PreCarga;
            }
            set
            {
                if (value.IsCarga())
                    Carga = (Embarcador.Cargas.Carga)value;
                else
                    PreCarga = (Embarcador.PreCargas.PreCarga)value;
            }
        }
    }
}
