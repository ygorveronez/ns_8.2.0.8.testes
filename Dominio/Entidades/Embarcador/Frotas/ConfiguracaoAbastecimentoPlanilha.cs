namespace Dominio.Entidades.Embarcador.Frotas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ABASTECIMENTO_CONFIGURACAO_PLANILHA", EntityName = "ConfiguracaoAbastecimentoPlanilha", Name = "Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimentoPlanilha", NameType = typeof(ConfiguracaoAbastecimentoPlanilha))]
    public class ConfiguracaoAbastecimentoPlanilha : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ABP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoAbastecimento", Column = "ABC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frotas.ConfiguracaoAbastecimento ConfiguracaoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCampo", Column = "ABP_TIPO_CAMPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampo TipoCampo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ColunaPlanilha", Column = "ABP_COLUNA_PLANILHA", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha ColunaPlanilha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Posicao", Column = "ABP_POSICAO", TypeType = typeof(int), NotNull = false)]
        public virtual int Posicao { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.DescricaoColunaPlanilha + " - " + this.ConfiguracaoAbastecimento.Descricao;
            }
        }

        public virtual string DescricaoColunaPlanilha
        {
            get
            {
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.CNPJPosto)
                    return "CNPJ do Posto";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.CodigoProduto)
                    return "Código do Produto";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.CPFMotorista)
                    return "CPF do Motorista";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.Data)
                    return "Data";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.DataEHora)
                    return "Data e Hora";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.DescricaoProduto)
                    return "Descrição do Produto";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.Hora)
                    return "Hora";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.KMAbastecimento)
                    return "KM do Abastecimento";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.KMAnterior)
                    return "KM Anterior ao Abastecimento";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.NomeMotorista)
                    return "Nome do Motorista";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.NomePosto)
                    return "Nome do Posto";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.NumeroCupom)
                    return "Número Cupom";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.NumeroNota)
                    return "Número Nota";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.Placa)
                    return "Placa";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.Quantidade)
                    return "Quantidade";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.ValorTotal)
                    return "Valor Total";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.ValorUnitario)
                    return "Valor Unitário";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.Horimetro)
                    return "Horimetro";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.PlacaVeiculoLetras)
                    return "Placa Veículo Letras";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.PlacaVeiculoNumero)
                    return "Placa Veículo Número";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.EnderecoPosto)
                    return "Endereço do Posto";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.DataBaseCTR)
                    return "Data Base CTR";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.ValorMoedaEstrangeira)
                    return "Valor da moeda estrangeira";
                if (this.ColunaPlanilha == ObjetosDeValor.Embarcador.Enumeradores.ColunaPlanilha.LocalArmazenamento)
                    return "Local de Armazenamento";
                else
                    return "Outro";
            }
        }

        public virtual string DescricaoTipoCampo
        {
            get
            {
                if (this.TipoCampo == ObjetosDeValor.Embarcador.Enumeradores.TipoCampo.Alfanumerico)
                    return "Alfanumerico";
                if (this.TipoCampo == ObjetosDeValor.Embarcador.Enumeradores.TipoCampo.Data)
                    return "Data";
                if (this.TipoCampo == ObjetosDeValor.Embarcador.Enumeradores.TipoCampo.DataHora)
                    return "Data e Hora";
                if (this.TipoCampo == ObjetosDeValor.Embarcador.Enumeradores.TipoCampo.Decimal)
                    return "Decimal";
                if (this.TipoCampo == ObjetosDeValor.Embarcador.Enumeradores.TipoCampo.Hora)
                    return "Hora";
                if (this.TipoCampo == ObjetosDeValor.Embarcador.Enumeradores.TipoCampo.Numerico)
                    return "Numérico";
                else
                    return "Outro";
            }
        }

        public virtual bool Equals(ConfiguracaoAbastecimentoPlanilha other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
