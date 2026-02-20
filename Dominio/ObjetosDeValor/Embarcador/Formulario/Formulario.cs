namespace Dominio.ObjetosDeValor.Embarcador.Formulario
{
    public class Formulario
    {
        #region Construtores

        public Formulario() { }
        public Formulario(AdminMultisoftware.Dominio.Entidades.Modulos.Formulario formulario)
        {
            Caminho = formulario.CaminhoPagina;
            Codigo = formulario.CodigoFormulario;
            Descricao = formulario.Descricao;
            Icone = formulario.Modulo?.IconeNovo;
            Modulo = formulario.Modulo?.Descricao;
            ModuloPai = ObterDadosModuloPai(formulario.Modulo);
        }

        #endregion

        #region Propriedades 

        public int Codigo { get; set; }
        public string Descricao { get; set; }
        public string Modulo { get; set; }
        public string Icone { get; set; }
        public string Caminho { get; set; }
        public object ModuloPai { get; set; }

        #endregion

        #region Métodos

        private object ObterDadosModuloPai(AdminMultisoftware.Dominio.Entidades.Modulos.Modulo modulo)
        {
            if (modulo == null)
                return null;

            if (modulo.ModuloPai == null)
            {
                return new
                {
                    Descricao = modulo.Descricao,
                    Icone = modulo.IconeNovo
                };
            }

            return ObterDadosModuloPai(modulo.ModuloPai);
        }

        #endregion
    }
}
