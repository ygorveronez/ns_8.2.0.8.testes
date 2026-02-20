using System;
using System.Xml.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.NDDCargo.Enumeradores
{
    [Serializable]
    public enum CategoriaPedagio
    {
        [XmlEnum("0")]
        Isento = 0,

        [XmlEnum("1")]
        MotocicletasMotonetasEBicicletasAMotor = 1,

        [XmlEnum("2")]
        AutomovelCaminhonetaEFurgaoDoisEixosSimples = 2,

        [XmlEnum("3")]
        AutomovelCaminhonetaComSemirreboqueTresEixosSimples = 3,

        [XmlEnum("4")]
        AutomovelCaminhonetaComReboqueQuatroEixosSimples = 4,

        [XmlEnum("5")]
        OnibusDoisEixosDuplos = 5,

        [XmlEnum("6")]
        OnibusComReboqueTresEixosDuplos = 6,

        [XmlEnum("7")]
        CaminhaoLeveFurgaoECavaloMecanicoDoisEixosDuplos = 7,

        [XmlEnum("8")]
        CaminhaoCaminhaoTratorECavaloMecanicoComSemirreboqueTresEixosDuplos = 8,

        [XmlEnum("9")]
        CaminhaoComReboqueECavaloMecanicoComSemirreboqueQuatroEixosDuplos = 9,

        [XmlEnum("10")]
        CaminhaoComReboqueECavaloMecanicoComSemirreboqueCincoEixosDuplos = 10,

        [XmlEnum("11")]
        CaminhaoComReboqueECavaloMecanicoComSemirreboqueSeisEixosDuplos = 11,

        [XmlEnum("12")]
        CaminhaoComReboqueECavaloMecanicoComSemirreboqueSeteEixosDuplos = 12,

        [XmlEnum("13")]
        CaminhaoComReboqueECavaloMecanicoComSemirreboqueOitoEixosDuplos = 13,

        [XmlEnum("14")]
        CaminhaoComReboqueECavaloMecanicoComSemirreboqueNoveEixosDuplos = 14,

        [XmlEnum("15")]
        CaminhaoComReboqueECavaloMecanicoComSemirreboqueDezEixos = 15,

        [XmlEnum("16")]
        CaminhaoComReboqueECavaloMecanicoComSemirreboqueAcimaDeDezEixos = 16,
    }

}
