namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioAtendimento
    {
        public int Numero { get; set; }

        public string Usuario { get; set; }

        public string Empresa { get; set; }

        public string TipoAtendimento { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSistema Sistema { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento Situacao { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.NivelSatisfacao Satisfacao { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento TipoContato { get; set; }

        public string Data { get; set; }

        public string Contato { get; set; }

        public string Descricao { get; set; }
        public string Observacao { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Aberto:
                        return "Aberto";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Cancelado:
                        return "Cancelado";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.EmAndamento:
                        return "Em Andamento";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Finalizado:
                        return "Finalizado";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoSistema
        {
            get
            {
                switch (this.Sistema)
                {
                    case Embarcador.Enumeradores.TipoSistema.Embarcador:
                        return "Embarcador";
                    case Embarcador.Enumeradores.TipoSistema.MultiCTe:
                        return "MultiCTe";
                    case Embarcador.Enumeradores.TipoSistema.TMS:
                        return "TMS";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoSatisfacao
        {
            get
            {
                switch (this.Satisfacao)
                {
                    case Embarcador.Enumeradores.NivelSatisfacao.Bom:
                        return "Bom";
                    case Embarcador.Enumeradores.NivelSatisfacao.Otimo:
                        return "Ótimo";
                    case Embarcador.Enumeradores.NivelSatisfacao.Ruim:
                        return "Ruim";
                    case Embarcador.Enumeradores.NivelSatisfacao.NaoAvaliado:
                        return "Não Avaliado";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoContato
        {
            get
            {
                switch (this.TipoContato)
                {
                    case Embarcador.Enumeradores.TipoContatoAtendimento.Celular:
                        return "Celular";
                    case Embarcador.Enumeradores.TipoContatoAtendimento.ChatWeb:
                        return "ChatWeb";
                    case Embarcador.Enumeradores.TipoContatoAtendimento.Email:
                        return "Email";
                    case Embarcador.Enumeradores.TipoContatoAtendimento.Outros:
                        return "Outros";
                    case Embarcador.Enumeradores.TipoContatoAtendimento.Skype:
                        return "Skype";
                    case Embarcador.Enumeradores.TipoContatoAtendimento.Telefone:
                        return "Telefone";
                    default:
                        return "";
                }
            }
        }
    }
}
