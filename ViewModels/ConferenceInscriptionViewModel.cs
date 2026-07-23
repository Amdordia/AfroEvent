using System.ComponentModel.DataAnnotations;

namespace AfroEvent.ViewModels
{
    public enum TypePass { Etudiant, Professionnel, VIP }
    public class ConferenceInscriptionViewModel
    {
        [Display(Name = "Nom complet")]
        [Required(ErrorMessage = "Le nom complet est obligatoire.")]
        [MinLength(3, ErrorMessage = "Le nom complet doit contenir au moins 3 caractères.")]
        public string NomComplet { get; set; } = string.Empty;

        [Display(Name = "Adresse e-mail")]
        [Required(ErrorMessage = "L'e-mail est obligatoire.")]
        [EmailAddress(ErrorMessage = "Le format de l'e-mail est invalide.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Type de pass")]
        [Required(ErrorMessage = "Le type de pass est obligatoire.")]
        //[EnumDataType()]
        [EnumDataType(typeof(TypePass))]
        public TypePass TypeP { get; set; }
        //public enum TypePass { Unknown=-3, Late=-1, OnTime=0, Early=1 };
        // public string TypePass { get; set; } = string.Empty;
        //public enum TypePassed {Etudiant, Professionnel, VIP};


        [Display(Name = "Nombre de places")]
        [Range(1, 5, ErrorMessage = "Le nombre de places doit être compris entre 1 et 5.")]
        public int NombrePlaces { get; set; }

        [Display(Name = "J'accepte les conditions")]
        // [Range(typeof(bool), "true", "true", ErrorMessage = "Vous devez accepter les conditions")]
        [MustBeTrue(ErrorMessage = "Vous devez accepter les conditions.")]
        public bool AccepteConditions { get; set; }
    }

    public class MustBeTrueAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return value is bool b && b;
        }
    }
}
