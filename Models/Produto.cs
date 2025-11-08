using System.Collections.Generic;
using WpfApp.Services.Validators;

namespace WpfApp.Models
{
    public class Produto : ValidatableModelBase
    {
        private int _id;
        private string _nome;
        private string _codigo;
        private decimal _valor;

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

        public string Codigo
        {
            get => _codigo;
            set => SetProperty(ref _codigo, value);
        }

        public decimal Valor
        {
            get => _valor;
            set => SetProperty(ref _valor, value);
        }

        // ativo/inativo (soft delete)
        bool _ativo = true;
        public bool Ativo 
        { 
            get => _ativo; 
            set 
            { 
                _ativo = value; OnPropertyChanged(nameof(Ativo)); 
            } 
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

                case nameof(Codigo):
                    ClearErrors(nameof(Codigo));
                    if (string.IsNullOrWhiteSpace(Codigo))
                        AddError(nameof(Codigo), "Código é obrigatório.");
                    else if (Codigo.Length > 50)
                        AddError(nameof(Codigo), "Código deve ter no máximo 50 caracteres.");
                    break;

                case nameof(Valor):
                    ClearErrors(nameof(Valor));
                    if (Valor <= 0)
                        AddError(nameof(Valor), "Valor deve ser maior que zero.");
                break;
            }
        }

        protected override IEnumerable<string> GetValidatableProperties()
            => new[] { nameof(Codigo), nameof(Nome), nameof(Valor) };
    }
}