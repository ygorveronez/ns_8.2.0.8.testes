namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SmartAdminBgColor
    {
        black = 0,
        green = 1,
        greenDark = 2,
        greenLight = 3,
        purple = 4,
        magenta = 5,
        pink = 6,
        pinkDark = 7,
        blue = 8,
        blueLight = 9,
        blueDark = 10,
        teal = 11,
        yellow = 12,
        orange = 13,
        orangeDark = 14,
        red = 15,
        redLight = 16
    }

    public static class SmartAdminBgColorHelper
    {
        public static string ObterSatatusBootstrap(this SmartAdminBgColor icone)
        {
            switch (icone)
            {
                case SmartAdminBgColor.green: 
                case SmartAdminBgColor.greenDark: 
                case SmartAdminBgColor.greenLight: 
                    return "success";

                case SmartAdminBgColor.orange: 
                case SmartAdminBgColor.orangeDark: 
                case SmartAdminBgColor.yellow: 
                    return "warning";

                case SmartAdminBgColor.blue: 
                case SmartAdminBgColor.blueLight: 
                case SmartAdminBgColor.blueDark: 
                case SmartAdminBgColor.black: 
                case SmartAdminBgColor.teal: 
                    return "info";

                case SmartAdminBgColor.red: 
                case SmartAdminBgColor.redLight: 
                    return "danger";

                default: return "info";
            }
        }
    }
}
