namespace Dominio.Entidades.EFPH
{
    public class _22 : Registro
    {
        #region Construtores

        public _22()
            : base("22")
        {
        }

        #endregion

        #region Propriedades

        public Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        #endregion

        #region Métodos

        public override string ObterDadosParaArquivo()
        {
            this.EscreverDado(this.CTe.Destinatario.Nome, 40); //Nome
            this.EscreverDado(this.CTe.Destinatario.CPF_CNPJ, 18); //Inscrição(CNPJ/CPF)
            this.EscreverDado(this.CTe.Destinatario.Tipo == Enumeradores.TipoPessoa.Fisica ? 2 : 1, 1); //Tipo Inscrição
            this.EscreverDado(this.CTe.Destinatario.IE_RG, 14); //Inscrição Estadual
            this.EscreverDado(this.CTe.Destinatario.Endereco, 40); //Endereço
            this.EscreverDado(this.CTe.Destinatario.Bairro, 14); //Bairro
            this.EscreverDado(this.CTe.Destinatario.CEP, 9); //CEP
            this.EscreverDado(this.CTe.Destinatario.Exterior ? "EX" : this.CTe.Destinatario.Localidade.Estado.Sigla, 2); //Estado
            this.EscreverDado(this.CTe.Destinatario.Exterior ? this.CTe.Destinatario.Cidade : this.CTe.Destinatario.Localidade.Descricao, 25); //Município
            this.EscreverDado(9998, 8); //Código Contábil
            this.EscreverDado(0, 12); //CEI/NIT
            this.EscreverDado(this.CTe.Destinatario.InscricaoMunicipal, 14); //Inscrição Municipal
            this.EscreverDado(Utilidades.String.OnlyNumbers(this.CTe.Destinatario.Telefone1), 11); //Telefone
            this.EscreverDado(this.CTe.Destinatario.NomeFantasia, 40); //Nome Fantasia
            this.EscreverDado(this.CTe.Destinatario.Exterior ? int.Parse(this.CTe.Destinatario.Pais.Sigla) : int.Parse(this.CTe.Destinatario.Localidade.Estado.Pais.Sigla), 4); //Código do País
            this.EscreverDado(6, 1); //Tipo da Entidade
            this.EscreverDado("", 1); //Espaços

            this.FinalizarRegistro();

            this.ObterRegistrosEFPHDerivados();

            return this.RegistroEFPH.ToString();
        }

        #endregion
    }
}
