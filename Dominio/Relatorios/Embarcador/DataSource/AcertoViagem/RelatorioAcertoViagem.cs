using System;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class RelatorioAcertoViagem
    {
        public int Codigo { get; set; }
        public int NumeroAcerto { get; set; }
        public DateTime DataAcerto { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public string Observacao { get; set; }
        public int Situacao { get; set; }
        //public string DescricaoSituacao
        //{
        //    get
        //    {
        //        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem situacao;
        //        Enum.TryParse(Convert.ToString(Situacao), out situacao);
        //        switch (situacao)
        //        {
        //            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.EmAntamento:
        //                return "Em Andamento";
        //            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Fechado:
        //                return "Fechado";
        //            default:
        //                return "";
        //        }
        //    }
        //}
        public int Etapa { get; set; }
        //public string DescricaoEtapa
        //{
        //    get
        //    {
        //        Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem etapa;
        //        Enum.TryParse(Convert.ToString(Etapa), out etapa);
        //        switch (etapa)
        //        {
        //            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Abastecimentos:
        //                return "Abastecimento";
        //            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Acerto:
        //                return "Acerto";
        //            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Cargas:
        //                return "Carga";
        //            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Fechamento:
        //                return "Fechamento";
        //            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.OutrasDespesas:
        //                return "Outras Despesas";
        //            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapasAcertoViagem.Pedagios:
        //                return "Pedágio";
        //            default:
        //                return "";
        //        }
        //    }
        //}
        public bool AprovadoAbastecimento { get; set; }
        //public string DescricaoAbastecimento
        //{
        //    get
        //    {                
        //        switch (AprovadoAbastecimento)
        //        {
        //            case true:
        //                return "Aprovado";
        //            case false:
        //                return "Não aprovado";
        //            default:
        //                return "";
        //        }
        //    }
        //}
        public bool AprovadoPedagio { get; set; }
        //public string DescricaoPedagio
        //{
        //    get
        //    {
        //        switch (AprovadoPedagio)
        //        {
        //            case true:
        //                return "Aprovado";
        //            case false:
        //                return "Não aprovado";
        //            default:
        //                return "";
        //        }
        //    }
        //}
        public string Placa { get; set; }
        public string Chassi { get; set; }
        public int Ano { get; set; }
        public int AnoModelo { get; set; }
        public string Renavam { get; set; }
        public string NumeroFrota { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public int KMInicial { get; set; }
        public int KMFinal { get; set; }
        public int KMTotal { get; set; }
        public int KMTotalAjustado { get; set; }
        public int Despesa { get; set; }
        public int Pedagio { get; set; }
        public int Abastecimento { get; set; }
        public int Carga { get; set; }
        public int Ocorrencias { get; set; }
        public int FichaMotorista { get; set; }
        public int Documentos { get; set; }

    }
}
