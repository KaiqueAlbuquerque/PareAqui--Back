namespace TCC.MVC.ViewModels
{
    public class GaragemViewModel
    {
        public string GaragemId { get; set; }

        public string Cep { get; set; }

        public string Endereco { get; set; }

        public string Bairro { get; set; }

        public string NumeroRua { get; set; }

        public string Complemento { get; set; }

        public string Cidade { get; set; }

        public string Estado { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public bool Condominio { get; set; }

        public bool? Ativo { get; set; }
    }
}