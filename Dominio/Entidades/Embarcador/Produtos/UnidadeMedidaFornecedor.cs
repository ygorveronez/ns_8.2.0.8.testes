using System;

namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_UNIDADE_MEDIDA_FORNECEDOR", EntityName = "UnidadeMedidaFornecedor", Name = "Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor", NameType = typeof(UnidadeMedidaFornecedor))]
    public class UnidadeMedidaFornecedor : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "UMF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoFornecedor", Column = "UMF_DESCRICAO_FORNECEDOR", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DescricaoFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeDeMedida", Column = "UMF_UNIDADE_MEDIDA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida UnidadeDeMedida { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }

        public virtual string DescricaoUnidadeDeMedida
        {
            get
            {
                switch (this.UnidadeDeMedida)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Quilograma:
                        return "KG - Quilograma";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Tonelada:
                        return "TON - Tonelada";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Unidade:
                        return "UNID - Unidade";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.UnidadeUN:
                        return "UN - Unidade";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Litros:
                        return "L - Litros";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MetroCubico:
                        return "M3 - Metro Cúbico";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MMBTU:
                        return "MMBTU";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Servico:
                        return "SERV - Serviço";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Caixa:
                        return "CX - Caixa";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Ampola:
                        return "AMPOLA - Ampola";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Balde:
                        return "BALDE - Balde";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Bandeja:
                        return "BANDEJ - Bandeja";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Barra:
                        return "BARRA - Barra";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Bisnaga:
                        return "BISNAG - Bisnaga";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Bloco:
                        return "BLOCO - Bloco";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Bobina:
                        return "BOBINA - Bobina";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Bombona:
                        return "BOMB - Bombona";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Capsula:
                        return "CAPS - Capsula";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Cartela:
                        return "CART - Cartela";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Cento:
                        return "CENTO - Cento";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Conjunto:
                        return "CJ - Conjunto";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Centimetro:
                        return "CM - Centimetro";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CentimetroQuadrado:
                        return "CM2 - Centimetro Quadrado";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom2Unidades:
                        return "CX2 - Caixa Com 2 Unidades";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom3Unidades:
                        return "CX3 - Caixa Com 3 Unidades";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom5Unidades:
                        return "CX5 - Caixa Com 5 Unidades";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom10Unidades:
                        return "CX10 - Caixa Com 10 Unidades";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom15Unidades:
                        return "CX15 - Caixa Com 15 Unidades";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom20Unidades:
                        return "CX20 - Caixa Com 20 Unidades";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom25Unidades:
                        return "CX25 - Caixa Com 25 Unidades";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom50Unidades:
                        return "CX50 - Caixa Com 50 Unidades";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom100Unidades:
                        return "CX100 - Caixa Com 100 Unidades";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Display:
                        return "DISP - Display";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Duzia:
                        return "DUZIA - Duzia";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Embalagem:
                        return "EMBAL - Embalagem";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Fardo:
                        return "FARDO - Fardo";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Folha:
                        return "FOLHA - Folha";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Frasco:
                        return "FRASCO - Frasco";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Galao:
                        return "GALAO - Galão";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Garrafa:
                        return "GF - Garrafa";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Gramas:
                        return "GRAMAS - Gramas";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Jogo:
                        return "JOGO - Jogo";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Kit:
                        return "KIT - Kit";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Lata:
                        return "LATA - Lata";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Metro:
                        return "M - Metro";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MetroQuadrado:
                        return "M2 - Metro Quadrado";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Milheiro:
                        return "MILHEI - Milheiro";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Mililitro:
                        return "MM - Mililitro";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MegawattHora:
                        return "MWH - Megawatt Hora";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Pacote:
                        return "PACOTE - Pacote";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Palete:
                        return "PALETE - Palete";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Pares:
                        return "PARES - Pares";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Peca:
                        return "PC - Peça";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Pote:
                        return "POTE - Pote";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Quilate:
                        return "K - Quilate";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Resma:
                        return "RESMA - Resma";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Rolo:
                        return "ROLO - Rolo";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Saco:
                        return "SACO - Saco";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Sacola:
                        return "SACOLA - Sacola";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Tambor:
                        return "TAMBOR - Tambor";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Tanque:
                        return "TANQUE - Tanque";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Tubo:
                        return "TUBO - Tubo";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Vasilhame:
                        return "VASIL - Vasilhame";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Vidro:
                        return "VIDRO - Vidro";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Cone:
                        return "CN - Cone";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Bolsa:
                        return "BO - Bolsa";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Dose:
                        return "DS - Dose";
                    default:
                        return "";
                }
            }
        }

        public virtual string SiglaUnidadeDeMedida
        {
            get
            {
                switch (this.UnidadeDeMedida)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Quilograma:
                        return "KG";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Tonelada:
                        return "TON";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Unidade:
                        return "UNID";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.UnidadeUN:
                        return "UN";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Litros:
                        return "L";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MetroCubico:
                        return "M3";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MMBTU:
                        return "MMBTU";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Servico:
                        return "SERV";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Caixa:
                        return "CX";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Ampola:
                        return "AMPOLA";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Balde:
                        return "BALDE";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Bandeja:
                        return "BANDEJ";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Barra:
                        return "BARRA";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Bisnaga:
                        return "BISNAG";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Bloco:
                        return "BLOCO";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Bobina:
                        return "BOBINA";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Bombona:
                        return "BOMB";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Capsula:
                        return "CAPS";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Cartela:
                        return "CART";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Cento:
                        return "CENTO";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Conjunto:
                        return "CJ";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Centimetro:
                        return "CM";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CentimetroQuadrado:
                        return "CM2";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom2Unidades:
                        return "CX2";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom3Unidades:
                        return "CX3";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom5Unidades:
                        return "CX5";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom10Unidades:
                        return "CX10";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom15Unidades:
                        return "CX15";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom20Unidades:
                        return "CX20";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom25Unidades:
                        return "CX25";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom50Unidades:
                        return "CX50";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.CaixaCom100Unidades:
                        return "CX100";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Display:
                        return "DISP";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Duzia:
                        return "DUZIA";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Embalagem:
                        return "EMBAL";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Fardo:
                        return "FARDO";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Folha:
                        return "FOLHA";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Frasco:
                        return "FRASCO";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Galao:
                        return "GALAO";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Garrafa:
                        return "GF";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Gramas:
                        return "GRAMAS";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Jogo:
                        return "JOGO";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Kit:
                        return "KIT";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Lata:
                        return "LATA";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Metro:
                        return "M";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MetroQuadrado:
                        return "M2";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Milheiro:
                        return "MM";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Mililitro:
                        return "MILHEI";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MegawattHora:
                        return "MWH";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Pacote:
                        return "PACOTE";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Palete:
                        return "PALETE";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Pares:
                        return "PARES";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Peca:
                        return "PC";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Pote:
                        return "POTE";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Quilate:
                        return "K";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Resma:
                        return "RESMA";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Rolo:
                        return "ROLO";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Saco:
                        return "SACO";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Sacola:
                        return "SACOLA";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Tambor:
                        return "TAMBOR";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Tanque:
                        return "TANQUE";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Tubo:
                        return "TUBO";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Vasilhame:
                        return "VASIL";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Vidro:
                        return "VIDRO";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Cone:
                        return "CN";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Bolsa:
                        return "BO";
                    case ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Dose:
                        return "DS";
                    default:
                        return "";
                }
            }
        }

        public virtual bool Equals(UnidadeMedidaFornecedor other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }


    }
}

