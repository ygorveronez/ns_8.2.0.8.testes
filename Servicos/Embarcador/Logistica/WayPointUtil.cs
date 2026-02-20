namespace Servicos.Embarcador.Logistica
{

    public class WayPointUtil
    {

        #region Astributos privados

        private const double ZERO = 0;
        private const double MINLATITUDE = -90;
        private const double MAXLATITUDE = 90;
        private const double MINLONGITUDE = -180;
        private const double MAXLONGITUDE = 180;

        #endregion

        #region Métodos públicos

        public static bool ValidarWayPoint(Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wp)
        {
            return ValidarLatitude(wp.Latitude) && ValidarLongitude(wp.Longitude);
        }

        public static bool ValidarCoordenadas(double latitude, double longitude)
        {
            return ValidarLatitude(latitude) && ValidarLongitude(longitude);
        }

        public static bool ValidarLatitude(double latitude)
        {
            return latitude != ZERO && latitude >= MINLATITUDE && latitude <= MAXLATITUDE;
        }

        public static bool ValidarLongitude(double longitude)
        {
            return longitude != ZERO && longitude >= MINLONGITUDE && longitude <= MAXLONGITUDE;
        }

        #endregion

    }

}
