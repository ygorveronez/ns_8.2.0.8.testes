using System;

namespace Servicos.Global
{
    public class CalculadoraGeolocalizacao
    {
        #region Métodos Privados

        private double DegToRad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private double RadToDeg(double rad)
        {
            return (rad * 180 / Math.PI);
        }

        #endregion

        #region Métodos Públicos

        public double ObterDistanciaEmKilometros(string latitudeBase, string longitudeBase, string latitudeComparar, string longitudeComparar)
        {
            return ObterDistanciaEmKilometros(latitudeBase.Replace(".", ",").ToDouble(), longitudeBase.Replace(".", ",").ToDouble(), latitudeComparar.Replace(".", ",").ToDouble(), longitudeComparar.Replace(".", ",").ToDouble());
        }

        public double ObterDistanciaEmKilometros(double latitudeBase, double longitudeBase, double latitudeComparar, double longitudeComparar)
        {
            var fatorConvercaoKilometros = 1.609344;
            var anguloTetaEmGraus = longitudeBase - longitudeComparar;
            var cossenoAnguloTetaEmRadianos = Math.Cos(DegToRad(anguloTetaEmGraus));
            var senoLatutide = Math.Sin(DegToRad(latitudeBase)) * Math.Sin(DegToRad(latitudeComparar));
            var cossenoLatitude = Math.Cos(DegToRad(latitudeBase)) * Math.Cos(DegToRad(latitudeComparar)) * cossenoAnguloTetaEmRadianos;
            var arcoCossenoEmGraus = RadToDeg(Math.Acos(senoLatutide + cossenoLatitude));
            var distanciaEmKilometros = (arcoCossenoEmGraus * 60 * 1.1515) * fatorConvercaoKilometros;

            return distanciaEmKilometros;
        }

        #endregion
    }
}
