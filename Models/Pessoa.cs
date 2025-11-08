using System.Collections.Generic;
using WpfApp.Services;
using WpfApp.Services.Validators;

namespace WpfApp.Models
{
    public class Pessoa : ValidatableModelBase
    {
        private int _id;
        private string _nome;
        private string _cpf;
        private string _endereco;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Nome
        {
            get => _nome;
            set => SetProperty(ref _nome, value);
        }

        public string CPF
        {
            get => _cpf;
            set => SetProperty(ref _cpf, value);
        }

        public string Endereco
        {
            get => _endereco;
            set => SetProperty(ref _endereco, value);
        }

        protected override void ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Nome):
                    ClearErrors(nameof(Nome));
                    if (string.IsNullOrWhiteSpace(Nome))
                        AddError(nameof(Nome), "Nome é obrigatório.");
                    else if (Nome.Length < 3)
                        AddError(nameof(Nome), "Nome deve ter pelo menos 3 caracteres.");
                    else if (Nome.Length > 255)
                        AddError(nameof(Nome), "Nome deve ter no máximo 255 caracteres.");
                    break;

                case nameof(CPF):
                    ClearErrors(nameof(CPF));
                    if (string.IsNullOrWhiteSpace(CPF))
                        AddError(nameof(CPF), "CPF é obrigatório.");
                    else if (!CpfValidator.IsValid(CPF))
                        AddError(nameof(CPF), "CPF inválido.");
                break;                
            }
        }

        protected override IEnumerable<string> GetValidatableProperties()
            => new[] { nameof(Nome), nameof(CPF), nameof(Endereco) };
    }
}