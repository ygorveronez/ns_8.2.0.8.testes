using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Veiculos
{
    public class PainelVeiculo
    {
        public int Codigo { get; set; }

        public int SituacaoVeiculo { get; set; }

        public string NumeroFrota { get; set; }

        public string Placa { get; set; }

        public string Proprietario { get; set; }

        public int TipoFrota { get; set; }

        public string Carga { get; set; }

        public string CentroCarregamento { get; set; }

        public string Modelo { get; set; }

        public string Reboque { get; set; }

        public string Motorista { get; set; }

        public string Transportador { get; set; }

        public string Cliente { get; set; }

        public DateTime? PrevisaoDisponivel { get; set; }

        public string LocalPrevisto { get; set; }

        public bool StatusVazio { get; set; }

        public bool StatusAvisado { get; set; }
        
        public bool Ativo { get; set; }

        #region Atributos Virtuais

        public virtual string Placa_Formatada
        {
            get
            {
                return string.Concat(this.Placa.Substring(0, 3), "-", this.Placa.Substring(3, 4));
            }
        }

        public string StatusVazioDescricao
        {
            get
            {
                return StatusVazio ? "SIM" : "NÃO";
            }
        }
        public string StatusAvisadoDescricao
        {
            get
            {
                return StatusAvisado ? "SIM" : "NÃO";
            }
        }
        public string TipoFrotaDescricao
        {
            get
            {
                return ((TipoFrota)TipoFrota).ObterDescricao();
            }
        }
        public virtual SituacaoVeiculo? enumSituacao
        {
            get
            {
                return SituacaoVeiculo > 0 ? (SituacaoVeiculo)SituacaoVeiculo : null;
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                if (SituacaoVeiculo == 0) return Ativo ? "Disponível" : "Indisponível";

                switch (enumSituacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao:
                        return "Em Manutenção";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem:
                        return "Em Viagem";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Disponivel:
                        return "Disponível";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.Indisponivel:
                        return "Indisponível";
                    default:
                        return Ativo ? "Disponível" : "Indisponível";
                }
            }
        }
        public virtual string ClienteViagem
        {
            get
            {
                return enumSituacao != null && enumSituacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem ?
                    Cliente : string.Empty;
            }
        }
        #endregion

        #region Atributos para Grid
        public string DT_RowClass
        {
            get
            {
                switch (enumSituacao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmManutencao:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Danger(IntensidadeCor._100);
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoVeiculo.EmViagem:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClasseCorFundo.Warning(IntensidadeCor._100);
                    default:
                        return string.Empty;
                }
            }
        }
        #endregion
    }
}
