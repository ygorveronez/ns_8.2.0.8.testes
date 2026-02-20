namespace Dominio.Relatorios.Embarcador.DataSource.Chamados.ChamadoOcorrencia
{
    public class GraficoChamado
    {
        public int Quantidade { get; set; }

        public string Descricao { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado Situacao { get; set; }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Aberto:
                        return "Aberto";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Finalizado:
                        return "Finalizado";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.SemRegra:
                        return "Sem Regra";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.LiberadaOcorrencia:
                        return "Liberada Ocorrência";
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Cancelada:
                        return "Cancelada";
                    default:
                        return "";
                }
            }
        }
    }
}
