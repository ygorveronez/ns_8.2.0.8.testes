namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EnumTecnologiaRastreador
    {
        NaoDefinido = 0,
        Mobile = 1,
        AutoTrack = 2,
        A2 = 3,
        Sascar = 4,
        CSX = 5,
        Getrak = 6,
        GVSat = 7,
        Kaboo = 8,
        Link = 9,
        MixTelematics = 10,
        Omnilink = 11,
        OnixSat = 12,
        OpenTech = 13,
        Positron = 14,
        Ravex = 15,
        Sighra = 16,
        SmartTracking = 17,
        SpyTruck = 18,
        SystemSat = 19,
        Traffilog = 20,
        Vtrack = 21,
        Freto = 22,
        Manual = 23,
        Ituran = 24,
        TresSTecnologia = 25,
        SkyWorld = 26,
        SigaSul = 27,
        WebRotas = 28,
        ABFSat = 29,
        Autovision = 30,
        LogRisk = 31,
        Maxtrack = 32,
        TransPanorama = 33,
        Trixlog = 34,
        TrustTrack = 35,
        AutoTrackEmbarcador = 36,
        GNSBrasil = 37,
        EagleTrack = 38,
        CCNTelematica = 39,
        AngelLira = 40,
        Raster = 41,
        RotaExata = 42,
        ApisulLog = 43,
        BIND = 44,
        ViaLink = 45,
        Buonny = 46,
        Evo = 47,
        MultiPortal = 48,
        Unitop = 49,
        TotalTrac = 50,
        Cobli = 51,
        Omnicomm = 52
    }

    public static class EnumTecnologiaRastreadorHelper
    {
        public static string ObterDescricao(this EnumTecnologiaRastreador rastreador)
        {
            switch (rastreador)
            {
                case EnumTecnologiaRastreador.NaoDefinido: return "";
                case EnumTecnologiaRastreador.Mobile: return "Mobile";
                case EnumTecnologiaRastreador.AutoTrack: return "AutoTrac";
                case EnumTecnologiaRastreador.A2: return "A2";
                case EnumTecnologiaRastreador.Sascar: return "Sascar";
                case EnumTecnologiaRastreador.CSX: return "CSX";
                case EnumTecnologiaRastreador.Getrak: return "Getrak";
                case EnumTecnologiaRastreador.GVSat: return "GVSat";
                case EnumTecnologiaRastreador.Kaboo: return "Kaboo";
                case EnumTecnologiaRastreador.Link: return "Link";
                case EnumTecnologiaRastreador.MixTelematics: return "MixTelematics";
                case EnumTecnologiaRastreador.Omnilink: return "Omnilink";
                case EnumTecnologiaRastreador.OnixSat: return "OnixSat";
                case EnumTecnologiaRastreador.OpenTech: return "OpenTech";
                case EnumTecnologiaRastreador.Positron: return "Positron";
                case EnumTecnologiaRastreador.Ravex: return "Ravex";
                case EnumTecnologiaRastreador.Sighra: return "Sighra";
                case EnumTecnologiaRastreador.SmartTracking: return "SmartTracking";
                case EnumTecnologiaRastreador.SpyTruck: return "SpyTruck";
                case EnumTecnologiaRastreador.SystemSat: return "SystemSat";
                case EnumTecnologiaRastreador.Traffilog: return "Traffilog";
                case EnumTecnologiaRastreador.Vtrack: return "Vtrack";
                case EnumTecnologiaRastreador.Freto: return "Freto";
                case EnumTecnologiaRastreador.Manual: return "Manual";
                case EnumTecnologiaRastreador.Ituran: return "Ituran";
                case EnumTecnologiaRastreador.TresSTecnologia: return "3S";
                case EnumTecnologiaRastreador.SkyWorld: return "SkyWorld";
                case EnumTecnologiaRastreador.SigaSul: return "SigaSul";
                case EnumTecnologiaRastreador.WebRotas: return "WebRotas";
                case EnumTecnologiaRastreador.ABFSat: return "ABFSat";
                case EnumTecnologiaRastreador.Autovision: return "Autovision";
                case EnumTecnologiaRastreador.LogRisk: return "LogRisk";
                case EnumTecnologiaRastreador.Maxtrack: return "Maxtrack";
                case EnumTecnologiaRastreador.Trixlog: return "Trixlog";
                case EnumTecnologiaRastreador.TrustTrack: return "TrustTrack";
                case EnumTecnologiaRastreador.AutoTrackEmbarcador: return "Autotrac(Embarcador/Integrador)";
                case EnumTecnologiaRastreador.GNSBrasil: return "GNSBrasil";
                case EnumTecnologiaRastreador.EagleTrack: return "EagleTrack";
                case EnumTecnologiaRastreador.CCNTelematica: return "CCNTelematica";
                case EnumTecnologiaRastreador.AngelLira: return "AngelLira";
                case EnumTecnologiaRastreador.Raster: return "Raster";
                case EnumTecnologiaRastreador.RotaExata: return "RotaExata";
                case EnumTecnologiaRastreador.ApisulLog: return "ApiSulLog";
                case EnumTecnologiaRastreador.BIND: return "BIND";
                case EnumTecnologiaRastreador.ViaLink: return "ViaLink";
                case EnumTecnologiaRastreador.TransPanorama: return "TransPanorama";
                case EnumTecnologiaRastreador.Buonny: return "Buonny";
                case EnumTecnologiaRastreador.Evo: return "Evo";
                case EnumTecnologiaRastreador.MultiPortal: return "MultiPortal";
                case EnumTecnologiaRastreador.Unitop: return "Unitop";
                case EnumTecnologiaRastreador.TotalTrac: return "TotalTrac";
                case EnumTecnologiaRastreador.Cobli: return "Cobli";
                case EnumTecnologiaRastreador.Omnicomm: return "RastreSat (Omnicomm)";

                default: return string.Empty;
            }
        }

        public static EnumTecnologiaRastreador ObterEnumPorDescricao(string Descricao)
        {
            if (string.IsNullOrEmpty(Descricao))
                return EnumTecnologiaRastreador.NaoDefinido;


            if (Descricao.ToLower().Contains("vtrack"))
                return EnumTecnologiaRastreador.Vtrack;
            else if (Descricao.ToLower().Contains("traffilog"))
                return EnumTecnologiaRastreador.Traffilog;
            else if (Descricao.ToLower().Contains("systemsat"))
                return EnumTecnologiaRastreador.SystemSat;
            else if (Descricao.ToLower().Contains("spytruck"))
                return EnumTecnologiaRastreador.SpyTruck;
            else if (Descricao.ToLower().Contains("smarttracking"))
                return EnumTecnologiaRastreador.SmartTracking;
            else if (Descricao.ToLower().Contains("sighra"))
                return EnumTecnologiaRastreador.Sighra;
            else if (Descricao.ToLower().Contains("ravex"))
                return EnumTecnologiaRastreador.Ravex;
            else if (Descricao.ToLower().Contains("positron"))
                return EnumTecnologiaRastreador.Positron;
            else if (Descricao.ToLower().Contains("opentech"))
                return EnumTecnologiaRastreador.OpenTech;
            else if (Descricao.ToLower().Contains("onixsat"))
                return EnumTecnologiaRastreador.OnixSat;
            else if (Descricao.ToLower().Contains("omnilink") || Descricao.ToLower().Contains("ominilink"))
                return EnumTecnologiaRastreador.Omnilink;
            else if (Descricao.ToLower().Contains("mixtelematics"))
                return EnumTecnologiaRastreador.MixTelematics;
            else if (Descricao.ToLower().Contains("link") && !Descricao.ToLower().Contains("via"))
                return EnumTecnologiaRastreador.Link;
            else if (Descricao.ToLower().Contains("kaboo"))
                return EnumTecnologiaRastreador.Kaboo;
            else if (Descricao.ToLower().Contains("gvsat"))
                return EnumTecnologiaRastreador.GVSat;
            else if (Descricao.ToLower().Contains("csx"))
                return EnumTecnologiaRastreador.CSX;
            else if (Descricao.ToLower().Contains("sascar"))
                return EnumTecnologiaRastreador.Sascar;
            else if ((Descricao.ToLower().Contains("autotrac") || Descricao.ToLower().Contains("autotrack")) && Descricao.ToLower().Contains("embarcador"))
                return EnumTecnologiaRastreador.AutoTrackEmbarcador;
            else if (Descricao.ToLower().Contains("autotrac"))
                return EnumTecnologiaRastreador.AutoTrack;
            else if (Descricao.ToLower().Contains("ituran"))
                return EnumTecnologiaRastreador.Ituran;
            else if (Descricao.ToLower().Contains("3s") || Descricao.ToLower().Contains("tress"))
                return EnumTecnologiaRastreador.TresSTecnologia;
            else if (Descricao.ToLower().Contains("sky"))
                return EnumTecnologiaRastreador.SkyWorld;
            else if (Descricao.ToLower().Contains("sigasul"))
                return EnumTecnologiaRastreador.SigaSul;
            else if (Descricao.ToLower().Contains("webrotas"))
                return EnumTecnologiaRastreador.WebRotas;
            else if (Descricao.ToLower().Contains("abfsat"))
                return EnumTecnologiaRastreador.ABFSat;
            else if (Descricao.ToLower().Contains("autovision"))
                return EnumTecnologiaRastreador.Autovision;
            else if (Descricao.ToLower().Contains("logrisk"))
                return EnumTecnologiaRastreador.LogRisk;
            else if (Descricao.ToLower().Contains("trixlog"))
                return EnumTecnologiaRastreador.Trixlog;
            else if (Descricao.ToLower().Contains("trusttrack"))
                return EnumTecnologiaRastreador.TrustTrack;
            else if (Descricao.ToLower().Contains("gnsbrasil"))
                return EnumTecnologiaRastreador.GNSBrasil;
            else if (Descricao.ToLower().Contains("eagletrack"))
                return EnumTecnologiaRastreador.EagleTrack;
            else if (Descricao.ToLower().Contains("ccn"))
                return EnumTecnologiaRastreador.CCNTelematica;
            else if (Descricao.ToLower().Contains("angellira"))
                return EnumTecnologiaRastreador.AngelLira;
            else if (Descricao.ToLower().Contains("rotaexata"))
                return EnumTecnologiaRastreador.RotaExata;
            else if (Descricao.ToLower().Contains("raster"))
                return EnumTecnologiaRastreador.Raster;
            else if (Descricao.ToLower().Contains("vialink"))
                return EnumTecnologiaRastreador.ViaLink;
            else if (Descricao.ToLower().Contains("evo"))
                return EnumTecnologiaRastreador.Evo;
            else if (Descricao.ToLower().Contains("multiportal"))
                return EnumTecnologiaRastreador.MultiPortal;
            else if (Descricao.ToLower().Contains("unitop"))
                return EnumTecnologiaRastreador.Unitop;
            else if (Descricao.ToLower().Contains("totaltrac"))
                return EnumTecnologiaRastreador.TotalTrac;
            else if (Descricao.ToLower().Contains("cobli"))
                return EnumTecnologiaRastreador.Cobli;
            else if (Descricao.ToLower().Contains("omnicomm"))
                return EnumTecnologiaRastreador.Omnicomm;
            else
                return EnumTecnologiaRastreador.NaoDefinido;
        }
    }

}

