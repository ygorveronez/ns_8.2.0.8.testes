using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoDadosColeta
{
    public class GestaoDadosColeta
    {
        #region Propriedades

        public int Codigo { get; set; }

        public TipoGestaoDadosColeta Tipo { get; set; }

        public SituacaoGestaoDadosColeta Situacao { get; set; }

        public OrigemGestaoDadosColeta Origem { get; set; }

        public string DestinoCarga { get; set; }

        public string OrigemCarga { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public DateTime DataAprovacao { get; set; }

        public DateTime DataInicialCriacaoCarga { get; set; }

        public DateTime DataFinalCriacaoCarga { get; set; }

        public string NomeCliente { get; set; }

        public string Filial { get; set; }

        public string DescricaoLocalidade { get; set; }

        public string NomeUsuario { get; set; }

        public bool MotoristaEstrangeiroUsuario { get; set; }

        public string CPFUsuario { get; set; }

        public DateTime DataRetornoConfirmacaoColeta { get; set; }

        public string ErroRetornoConfirmacaoColeta { get; set; }

        public string IdExternoRetornoConfirmacaoColeta { get; set; }

        public string OperacaoRetornoConfirmacaoColeta { get; set; }

        public Dominio.Enumeradores.TipoAcesso TipoAcessoUsuario { get; set; }

        public DateTime DataCriacaoCarga { get; set; }

        #endregion Propriedades

        #region Propriedades Com Regras

        public string DataAprovacaoFormatada
        {
            get { return DataAprovacao != DateTime.MinValue ? DataAprovacao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataInicial
        {
            get { return DataInicialCriacaoCarga != DateTime.MinValue ? DataInicialCriacaoCarga.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataFinal
        {
            get { return DataFinalCriacaoCarga != DateTime.MinValue ? DataFinalCriacaoCarga.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataRetornoConfirmacaoColetaFormatada
        {
            get { return DataRetornoConfirmacaoColeta != DateTime.MinValue ? DataRetornoConfirmacaoColeta.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty; }
        }

        public string Duracao
        {
            get { return ((DataAprovacao != DateTime.MinValue ? DataAprovacao : DateTime.Now) - DataInicialCriacaoCarga).ToTimeString(showSeconds: true); }
        }

        public string TipoDescricao
        {
            get { return TipoGestaoDadosColetaHelper.ObterDescricao(Tipo); }
        }

        public string SituacaoAprovacao
        {
            get { return SituacaoGestaoDadosColetaHelper.ObterDescricao(Situacao); }
        }

        public string OrigemDescricao
        {
            get { return OrigemGestaoDadosColetaHelper.ObterDescricao(Origem); }
        }

        public string Cliente
        {
            get
            {
                string descricao = string.Empty;

                if (!string.IsNullOrEmpty(NomeCliente))
                    descricao = NomeCliente;
                else if (!string.IsNullOrEmpty(DescricaoLocalidade))
                    descricao = DescricaoLocalidade;

                return descricao;
            }
        }

        public virtual string UsuarioAprovacao
        {
            get
            {
                if (TipoAcessoUsuario == Dominio.Enumeradores.TipoAcesso.Embarcador || string.IsNullOrEmpty(CPF_CNPJ_FormatadoUsuario))
                    return $"{NomeUsuario}";
                else
                    return $"{NomeUsuario} ({CPF_CNPJ_FormatadoUsuario})";
            }
        }

        public virtual string CPF_CNPJ_FormatadoUsuario
        {
            get
            {
                if (MotoristaEstrangeiroUsuario)
                    return CPFUsuario;

                string cpf = Utilidades.String.OnlyNumbers(CPFUsuario);

                if (string.IsNullOrEmpty(cpf))
                    return string.Empty;

                long.TryParse(cpf, out long cpfCnpj);

                if (cpf.Length > 11)
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", cpfCnpj);
                else
                    return String.Format(@"{0:000\.000\.000\-00}", cpfCnpj);
            }
        }

        public string DataCriacaoCargaFormatada
        {
            get { return DataCriacaoCarga != DateTime.MinValue ? DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        #endregion Propriedades Com Regras

        #region Propriedades para GridView

        public string DT_RowColor
        {
            get
            {
                return !string.IsNullOrEmpty(ErroRetornoConfirmacaoColeta) ? CorGrid.Red : CorGrid.Branco;
            }
        }

        #endregion
    }
}