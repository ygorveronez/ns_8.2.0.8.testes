using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Ambipar.ValePedagio
{
    public enum enumMidiaCargaTipoID
    {
        Vale_Pedagio_Ambipar_Visa_Cargo = 1,
        Pedagio_Eletronico_Tag_Eletrônica = 2
    }

    public enum enumTipoTag
    {
        Tag_Ambipar = 1,
        Tag_SemParar = 2
    }

    public class envViagem
    {
        public int embarcadorFilialID { get; set; }
        public int? cartaoID { get; set; }
        public int? motoristaID { get; set; }
        public int? transportadorID { get; set; }
        public int? veiculoID { get; set; }
        public int? carretaID { get; set; }
        public int? roteiroID { get; set; }
        public enumMidiaCargaTipoID midiaCargaTipoID { get; set; }
        public int quantidadeEixosVeiculo { get; set; }
        public int quantidadeEixosCarreta { get; set; }
        public int eixoSuspensoIda { get; set; }
        public int eixoSuspensoVolta { get; set; }
        public string midiaIdentificador { get; set; }
        public bool ignorarPreConfiguracao { get; set; }
        public string tag { get; set; }
        public enumTipoTag? tipoTag { get; set; }
        public int? tagId { get; set; }
        public string dataTermino { get; set; }
        public List<eixoSuspensoParadasIda> eixoSuspensoParadasIda { get; set; }
        public List<eixoSuspensoParadasVolta> eixoSuspensoParadasVolta { get; set; }
        public List<carretaAdicional> carretaAdicional { get; set; }
    }

    public class eixoSuspensoParadasIda
    {
        public int ordem { get; set; }
        public int eixosSuspenso { get; set; }
    }

    public class eixoSuspensoParadasVolta
    {
        public int ordem { get; set; }
        public int eixosSuspenso { get; set; }
    }

    public class carretaAdicional
    {
        public int carretaID { get; set; }
    }
}